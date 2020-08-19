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
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;
namespace Selenium.Scripts.Tests
{
    class Curved_StraightenedMPRControl : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public DomainManagement domain { get; set; }
        public StudyViewer studyviewer { get; set; }
        public Cursor Cursor { get; private set; }

        public Curved_StraightenedMPRControl(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            domain = new DomainManagement();

            studyviewer = new StudyViewer();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }

        public TestCaseResult Test_163318(String testid, String teststeps, int stepcount)
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
                String NavLocation34 = objTestRequirement.Split('|')[0];
                String NavLocation60 = objTestRequirement.Split('|')[1];
               

                //step 01 :: Search and load a 3D supported study in the universal viewer.
                login.LoginIConnect(adminUserName, adminPassword);
                bool StudyLoad = brz3dvp.searchandopenstudyin3D(objpatid, objthumbimg, layout: BluRingZ3DViewerPage.CurvedMPR);
                if (StudyLoad)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    throw new Exception("unable to launch study in 3D Viewer");
                }
                //Steps 2::Click on MPR navigation control 1 and scroll up until the top of the aorta is visible.
                IWebElement navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(navigation1).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                String InitialLocation = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                brz3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, "1");
                brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, NavLocation34);
                PageLoadWait.WaitForFrameLoad(10);
                String LocationAfterScroll = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (LocationAfterScroll == NavLocation34 && LocationAfterScroll != InitialLocation)
                {
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
                IWebElement Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int ColorValBefore = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 60).Click().Build().Perform();
                //brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 60);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                new Actions(Driver).MoveToElement(Navigation1).Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_3 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 2, 0, 0, 255, 2, isMoveCursor: true);
                if (ColorValAfter_3 != ColorValBefore)
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
                //Steps 4::Add a 2nd point along the aorta displayed on navigation control 1.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                PageLoadWait.WaitForFrameLoad(10);
                //new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 40).Click().Build().Perform();
                brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 40);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_4 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 2, 0, 0, 255, 2);
                IWebElement ViewerContainer = brz3dvp.ViewerContainer();
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer , pixelTolerance:10) && ColorValAfter_3 != ColorValAfter_4)
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
                //Steps 5::Add the 3rd point on the aorta below the 2nd point by clicking on “Navigation 2” image this time.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                new Actions(Driver).MoveToElement(Navigation1).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Actions action = new Actions(Driver);
                //action.MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 40).ClickAndHold().Build().Perform();
                brz3dvp.MoveClickAndHold(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 40);
                PageLoadWait.WaitForFrameLoad(10);
                action.Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                IWebElement navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(navigation2).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValBefore_5 = brz3dvp.LevelOfSelectedColor(navigation2, testid, ExecutedSteps + 6, 0, 0, 255, 2);
                //new Actions(Driver).MoveToElement(navigation2, (navigation2.Size.Width / 2) + 22, (navigation2.Size.Height / 4) - 10).Click().Build().Perform();
                brz3dvp.MoveAndClick(navigation2, (navigation2.Size.Width / 2) + 22, (navigation2.Size.Height / 4) - 10);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_5 = brz3dvp.LevelOfSelectedColor(navigation2, testid, ExecutedSteps + 7, 0, 0, 255, 2);
                ViewerContainer = brz3dvp.ViewerContainer();
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer , pixelTolerance: 10) && ColorValAfter_5 != ColorValBefore_5)
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
                //Steps 6::Select the reset button from the 3D tool box.
                string BeforeNav1 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                bool Reset = brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.MPRPathNavigation);
                string AfterNav1 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                if (Reset && BeforeNav1 != AfterNav1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 7::Right click on the navigation control 1, then right click on the Curved drawing tool and select the Auto vessel option from the drop down
                bool CurveDrawingAutoVissel = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                //Verification::Curve drawing cursor shows up while hovering over the images.
                if(CurveDrawingAutoVissel)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 8::Click on navigation control 1 and scroll up until the top of the aorta is visible.
                navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                PageLoadWait.WaitForFrameLoad(10);
                InitialLocation = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                brz3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, "1");
                brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, NavLocation34);
                PageLoadWait.WaitForFrameLoad(10);
                LocationAfterScroll = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (LocationAfterScroll == NavLocation34 && LocationAfterScroll != InitialLocation)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 9::Add a point at the top of the aorta displayed on navigation control 1.
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                ColorValBefore = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 60).Click().Build().Perform();
                //brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 60);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(Navigation1).Build().Perform();
                int ColorValAfter_9 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 2, 0, 0, 255, 2, isMoveCursor: true);
                if (ColorValAfter_9 != ColorValBefore)
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
                //Step 10::Add a 2nd point along the aorta displayed on navigation control 1.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                PageLoadWait.WaitForFrameLoad(10);
                //new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 40).Click().Build().Perform();
                brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 40);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(10);
                Boolean checkissue = brz3dvp.checkerrormsg();
                if (checkissue)
                    throw new Exception("Failed to find path");
                int ColorValAfter_10 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 2, 0, 0, 255, 2);
                ViewerContainer = brz3dvp.ViewerContainer();
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer, pixelTolerance: 10) && ColorValAfter_9 != ColorValAfter_10)
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
                //Step 11::Add the 3rd point on the aorta below the 2nd point by clicking on “Navigation 2” image this time.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                new Actions(Driver).MoveToElement(Navigation1).SendKeys("x").Build().Perform();
                brz3dvp.select3DTools(Z3DTools.Pan);
                PageLoadWait.WaitForFrameLoad(10);
                Actions actions = new Actions(Driver);
                //actions.MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 40).ClickAndHold().Build().Perform();
                brz3dvp.MoveClickAndHold(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 40);
                PageLoadWait.WaitForFrameLoad(10);
                actions.Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5, BluRingZ3DViewerPage.Navigationtwo);
                PageLoadWait.WaitForFrameLoad(10);
                navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(navigation2).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValBefore_11 = brz3dvp.LevelOfSelectedColor(navigation2, testid, ExecutedSteps + 6, 0, 0, 255, 2);
                //new Actions(Driver).MoveToElement(navigation2, (navigation2.Size.Width / 2) + 22, (navigation2.Size.Height / 4) - 10).Click().Build().Perform();
                brz3dvp.MoveAndClick(navigation2, (navigation2.Size.Width / 2) + 22, (navigation2.Size.Height / 4) - 10);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_11 = brz3dvp.LevelOfSelectedColor(navigation2, testid, ExecutedSteps + 7, 0, 0, 255, 2);
                ViewerContainer = brz3dvp.ViewerContainer();
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer , pixelTolerance: 10) && ColorValAfter_11 != ColorValBefore_11)
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
                //Step 12::Select the reset button from the 3D tool box.
                BeforeNav1 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                Reset = brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationthree);
                AfterNav1 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                if (Reset && BeforeNav1 != AfterNav1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 13::Right click on the navigation control 1, then right click on the Curved drawing tool and select the Auto colon option from the drop down
                bool AutoColon = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                if (AutoColon)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 14::Click on navigation control 1 and scroll up until the colon is visible.
                navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                PageLoadWait.WaitForFrameLoad(10);
                InitialLocation = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                brz3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, "1");
                brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, NavLocation60);
                PageLoadWait.WaitForFrameLoad(10);
                LocationAfterScroll = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
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
                //Step 15::Add a point at the beginning of colon displayed on navigation control 1.
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                ColorValBefore = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                //new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2 + 7, navigation1.Size.Height / 2 + 91).Click().Build().Perform();
                brz3dvp.MoveAndClick(navigation1, navigation1.Size.Width / 2 + 7, navigation1.Size.Height / 2 + 91);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_15 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 2, 0, 0, 255, 2);
                if (ColorValAfter_15 > ColorValBefore)
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
                //Step 16::Add a 2nd point along the colon displayed on navigation control 1.
                Thread.Sleep(5000);
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                //new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2 + 7, navigation1.Size.Height / 2 + 119).Click().Build().Perform();
                brz3dvp.MoveAndClick(navigation1, navigation1.Size.Width / 2 + 7, navigation1.Size.Height / 2 + 119);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(10);
                checkissue = brz3dvp.checkerrormsg();
                if (checkissue)
                    throw new Exception("Failed to find path");
                int ColorValAfter_16 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 2, 0, 0, 255, 2);
                ViewerContainer = brz3dvp.ViewerContainer();
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer , pixelTolerance: 10) && ColorValAfter_16 > ColorValAfter_15)
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
                //Step 17::Add the 3rd point on the colon below the 2nd point by clicking on “Navigation 2” image this time.
                Thread.Sleep(5000);
                new Actions(Driver).MoveToElement(navigation1).SendKeys("x").Build().Perform();
                brz3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                PageLoadWait.WaitForFrameLoad(10);
                action = new Actions(Driver);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree)).Build().Perform();
                Thread.Sleep(2000);
                //action.MoveToElement(navigation1, navigation1.Size.Width / 2 + 7, navigation1.Size.Height / 2 + 119).ClickAndHold().Build().Perform();
                brz3dvp.MoveClickAndHold(navigation1, navigation1.Size.Width / 2 + 7, navigation1.Size.Height / 2 + 119);
                PageLoadWait.WaitForFrameLoad(10);
                action.Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5, BluRingZ3DViewerPage.Navigationtwo);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);//Config.MPRPathNavigation
                new Actions(Driver).MoveToElement(navigation2).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                ColorValBefore = brz3dvp.LevelOfSelectedColor(navigation2, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                //new Actions(Driver).MoveToElement(navigation2, navigation2.Size.Width / 2 + 33, navigation2.Size.Height / 2 + 131).Click().Build().Perform();
                brz3dvp.MoveAndClick(navigation2, navigation2.Size.Width / 2 + 33, navigation2.Size.Height / 2 + 131);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_17 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 2, 0, 0, 255, 2);
                ViewerContainer = brz3dvp.ViewerContainer();
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer,pixelTolerance:10) && ColorValAfter_17 > ColorValBefore)
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
                //Step 18::Select the reset button from the 3D tool box.
                BeforeNav1 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                Reset = brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.MPRPathNavigation);
                AfterNav1 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                if (Reset && BeforeNav1 != AfterNav1)
                {
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

        public TestCaseResult Test_163319(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result;
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
                String NavLocation34 = objTestRequirement.Split('|')[0];
                String NavLocation60 = objTestRequirement.Split('|')[1];


                //step 01 :: Search and load a 3D supported study in the universal viewer.
                login.LoginIConnect(adminUserName, adminPassword);
                bool StudyLoad = brz3dvp.searchandopenstudyin3D(objpatid, objthumbimg, layout: BluRingZ3DViewerPage.CurvedMPR);
                if (StudyLoad)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    throw new Exception("unable to launch study in 3D Viewer");
                }
                //Steps 2::Click on MPR navigation control 1 and scroll up until the top of the aorta is visible.
                IWebElement navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(navigation1).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                String InitialLocation = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, NavLocation34,ScrollDirection:"up",scrolllevel:34,Thickness:"y");
                PageLoadWait.WaitForFrameLoad(10);
                String LocationAfterScroll = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (LocationAfterScroll == NavLocation34 && LocationAfterScroll != InitialLocation)
                {
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
                IWebElement Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int ColorValBefore = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 60).Click().Build().Perform();
                //brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 60);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(Navigation1).Build().Perform();
                int ColorValAfter_3 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 2, 0, 0, 255, 2, isMoveCursor: true);
                if (ColorValAfter_3 != ColorValBefore)
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
                //Steps 4::Add a 2nd point along the aorta displayed on navigation control 1.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                PageLoadWait.WaitForFrameLoad(10);
                //new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 40).Click().Build().Perform();
                brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 40);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_4 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 2, 0, 0, 255, 2);
                if (ColorValAfter_3 != ColorValAfter_4)
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
                //Steps 5::Add the 3rd point on the aorta below the 2nd point by clicking on “Navigation 2” image this time.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                new Actions(Driver).MoveToElement(Navigation1).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Actions action = new Actions(Driver);
                //action.MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 40).ClickAndHold().Build().Perform();
                brz3dvp.MoveClickAndHold(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 40);
                PageLoadWait.WaitForFrameLoad(10);
                action.Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                IWebElement navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(navigation2).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValBefore_5 = brz3dvp.LevelOfSelectedColor(navigation2, testid, ExecutedSteps + 6, 0, 0, 255, 2);
                //new Actions(Driver).MoveToElement(navigation2, (navigation2.Size.Width / 2) + 22, (navigation2.Size.Height / 4) - 10).Click().Build().Perform();
                brz3dvp.MoveAndClick(navigation2, (navigation2.Size.Width / 2) + 22, (navigation2.Size.Height / 4) - 10);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_5 = brz3dvp.LevelOfSelectedColor(navigation2, testid, ExecutedSteps + 7, 0, 0, 255, 2);
                if (ColorValAfter_5 != ColorValBefore_5)
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
                //Steps 6::Right click on the Curved MPR control and drag left and right.
                IWebElement CurvedMpr = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                bool Step6 = brz3dvp.VerifyRightClickDragandDropImage(CurvedMpr, BluRingZ3DViewerPage.CurvedMPR);
                if (Step6)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 7::Select the rotate tool from the 3D tool box.
                bool RotateTool = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center);
                if (RotateTool)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 8::Click on the path displayed on the Curved MPR control and drag left and right.
                string BeforeNavigationLocVal = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Actions step8 = new Actions(Driver);
                step8.MoveToElement(navigation1, 175, 75).ClickAndHold()
                    .MoveToElement(navigation1, 130, 75).MoveToElement(navigation1, 230, 75).Build().Perform();
                step8.Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                string AfterNavigationLocVal = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
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
                //Steps 9::Right click and drag the image from the controls under Curved MPR view.
                string BeforeNavigation1LocVal = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigationone);
                string BeforeNavigation2LocVal = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigationtwo);
                string BeforeNavigation3LocVal = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigationthree);
                System.Drawing.Point location = brz3dvp.ControllerPoints(BluRingZ3DViewerPage.Navigationone);
                int xcoordinate = location.X;
                int ycoordinate = location.Y;
                Cursor.Position = new System.Drawing.Point(xcoordinate - 10, ycoordinate);
                Thread.Sleep(2000);
                BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                Thread.Sleep(1000);
                Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate);
                Thread.Sleep(2000);
                BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(10000);
                string AfterNavigation1LocVal = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigationone);
                string AfterNavigation2LocVal = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigationtwo);
                string AfterNavigation3LocVal = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigationthree);
                bool Step9_1 = false;
                if(BeforeNavigation1LocVal != AfterNavigation1LocVal && BeforeNavigation2LocVal != AfterNavigation2LocVal && BeforeNavigation3LocVal != AfterNavigation3LocVal)
                {
                    Step9_1 = true;
                }
                IWebElement ViewerContainer = brz3dvp.ViewerContainer();
                bool Step9_2 = brz3dvp.VerifyRightClickDragandDropImage(ViewerContainer, BluRingZ3DViewerPage._3DPathNavigation);
                bool Step9_3 = brz3dvp.VerifyRightClickDragandDropImage(CurvedMpr, BluRingZ3DViewerPage.CurvedMPR);
                if(Step9_1 && Step9_2 && Step9_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 10::Select the reset button from the 3D tool box.
                BeforeNavigationLocVal = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                bool RestetTool = brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                AfterNavigationLocVal = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
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
                //Step 11 :: Right click on the navigation control 1, then right click on the Curved drawing tool and select the Auto vessel option from the drop down
                bool AutoVissels = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                //Verification::Curve drawing cursor appears while hovering over the images.
                if (AutoVissels)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 12::Click on navigation control 1 and scroll up until the top of the aorta is visible.
                navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                PageLoadWait.WaitForFrameLoad(10);
                InitialLocation = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, NavLocation34);
                PageLoadWait.WaitForFrameLoad(10);
                LocationAfterScroll = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (LocationAfterScroll == NavLocation34 && LocationAfterScroll != InitialLocation)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 13::Add a point at the top of the aorta displayed on navigation control 1.
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                ColorValBefore = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 60).Click().Build().Perform();
                //brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 60);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(Navigation1).Build().Perform();
                int ColorValAfter_13 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 2, 0, 0, 255, 2, isMoveCursor: true);
                if (ColorValAfter_13 != ColorValBefore)
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
                //Step 14::Add a 2nd point along the aorta displayed on navigation control 1.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                PageLoadWait.WaitForFrameLoad(10);
                //new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 40).Click().Build().Perform();
                brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 40);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(10);
                Boolean checkissue = brz3dvp.checkerrormsg();
                if (checkissue)
                    throw new Exception("Failed to find path");
                int ColorValAfter_14 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 2, 0, 0, 255, 2);
                if (ColorValAfter_13 != ColorValAfter_14)
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
                //Step 15::Add the 3rd point on the aorta below the 2nd point by clicking on “Navigation 2” image this time.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                new Actions(Driver).MoveToElement(Navigation1).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.select3DTools(Z3DTools.Pan);
                Actions actions = new Actions(Driver);
                //actions.MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 40).ClickAndHold().Build().Perform();
                brz3dvp.MoveClickAndHold(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 40);
                PageLoadWait.WaitForFrameLoad(10);
                actions.Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(navigation2).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValBefore_15 = brz3dvp.LevelOfSelectedColor(navigation2, testid, ExecutedSteps + 6, 0, 0, 255, 2);
                //new Actions(Driver).MoveToElement(navigation2, (navigation2.Size.Width / 2) + 22, (navigation2.Size.Height / 4) - 10).Click().Build().Perform();
                brz3dvp.MoveAndClick(navigation2, (navigation2.Size.Width / 2) + 22, (navigation2.Size.Height / 4) - 10);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_15 = brz3dvp.LevelOfSelectedColor(navigation2, testid, ExecutedSteps + 7, 0, 0, 255, 2);
                if (ColorValAfter_15 != ColorValBefore_15)
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
                //Step 16::Right click on the Curved MPR control and drag the image.
                CurvedMpr = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                bool Step16 = brz3dvp.VerifyRightClickDragandDropImage(CurvedMpr, BluRingZ3DViewerPage.CurvedMPR);
                if (Step16)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 17::Select the rotate tool from the 3D tool box .
                RotateTool = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center);
                if (RotateTool)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 18 :: Click on the path displayed on the Curved MPR control and drag left and right.
                BeforeNavigationLocVal = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Actions step18 = new Actions(Driver);
                step18.MoveToElement(navigation1, 175, 75).ClickAndHold()
                    .MoveToElement(navigation1, 130, 75).MoveToElement(navigation1, 230, 75).Build().Perform();
                step18.Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                AfterNavigationLocVal = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
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
                //Step 19 ::Right click and drag the image from the controls under Curved MPR view.
                BeforeNavigation1LocVal = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigationone);
                BeforeNavigation2LocVal = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigationtwo);
                BeforeNavigation3LocVal = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigationthree);
                Thread.Sleep(10000);
                location = brz3dvp.ControllerPoints(BluRingZ3DViewerPage.Navigationone);
                xcoordinate = location.X;
                ycoordinate = location.Y;
                Cursor.Position = new System.Drawing.Point(xcoordinate - 10, ycoordinate);
                BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                Thread.Sleep(1000);
                Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate);
                BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                PageLoadWait.WaitForFrameLoad(10);
                AfterNavigation1LocVal = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigationone);
                AfterNavigation2LocVal = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigationtwo);
                AfterNavigation3LocVal = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigationthree);
                bool Step19_1 = false;
                if (BeforeNavigation1LocVal != AfterNavigation1LocVal && BeforeNavigation2LocVal != AfterNavigation2LocVal && BeforeNavigation3LocVal != AfterNavigation3LocVal)
                {
                    Step19_1 = true;
                }
                ViewerContainer = brz3dvp.ViewerContainer();
                bool Step19_2 = brz3dvp.VerifyRightClickDragandDropImage(ViewerContainer, BluRingZ3DViewerPage._3DPathNavigation);
                bool Step19_3 = brz3dvp.VerifyRightClickDragandDropImage(CurvedMpr, BluRingZ3DViewerPage.CurvedMPR);
                if (Step19_1 && Step19_2 && Step19_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 20::Select the reset button from the 3D tool box.
                BeforeNavigationLocVal = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                RestetTool = brz3dvp.select3DTools(Z3DTools.Reset);
                AfterNavigationLocVal = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
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
                //Step 21::Right click on the navigation control 1, then right click on the Curved drawing tool and select the Auto colon option from the drop down
                bool AutoColon = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                //Verification::Curve drawing cursor appears while hovering over the images.
                if (AutoColon)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 22::Click on navigation control 1 and scroll up until the top of the Colon is visible.
                navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                PageLoadWait.WaitForFrameLoad(10);
                InitialLocation = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, NavLocation60);
                PageLoadWait.WaitForFrameLoad(10);
                LocationAfterScroll = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
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
                //Steps 23::Add a point at the top of the colon displayed on navigation control 1.
                Thread.Sleep(5000);
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                ColorValBefore = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                //new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2 + 7, navigation1.Size.Height / 2 + 91).Click().Build().Perform();
                brz3dvp.MoveAndClick(navigation1, navigation1.Size.Width / 2 + 7, navigation1.Size.Height / 2 + 91);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_23 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 2, 0, 0, 255, 2, isMoveCursor: true);
                if (ColorValAfter_23 > ColorValBefore)
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
                //Steps 24::Add a 2nd point along the colon displayed on navigation control 1.
                Thread.Sleep(5000);
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                PageLoadWait.WaitForFrameLoad(10);
                //new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2 + 7, navigation1.Size.Height / 2 + 119).Click().Build().Perform();
                brz3dvp.MoveAndClick(navigation1, navigation1.Size.Width / 2 + 7, navigation1.Size.Height / 2 + 119);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(10);
                checkissue = brz3dvp.checkerrormsg();
                if (checkissue)
                    throw new Exception("Failed to find path");
                int ColorValAfter_24 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 2, 0, 0, 255, 2);
                if ( ColorValAfter_24 > ColorValAfter_23)
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
                //Steps 25::Add the 3rd point on the colon below the 2nd point by clicking on “Navigation 2” image this time.
                Thread.Sleep(5000);
                new Actions(Driver).MoveToElement(navigation1).SendKeys("x").Build().Perform();
                brz3dvp.select3DTools(Z3DTools.Pan);
                PageLoadWait.WaitForFrameLoad(10);
                action = new Actions(Driver);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree)).Build().Perform();
                Thread.Sleep(2000);
                //action.MoveToElement(navigation1, navigation1.Size.Width / 2 + 7, navigation1.Size.Height / 2 + 119).ClickAndHold().Build().Perform();
                brz3dvp.MoveClickAndHold(navigation1, navigation1.Size.Width / 2 + 7, navigation1.Size.Height / 2 + 119);
                PageLoadWait.WaitForFrameLoad(10);
                action.Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5, BluRingZ3DViewerPage.Navigationtwo);
                PageLoadWait.WaitForFrameLoad(10);
                navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);//Config.MPRPathNavigation
                new Actions(Driver).MoveToElement(navigation2).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                ColorValBefore = brz3dvp.LevelOfSelectedColor(navigation2, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                //new Actions(Driver).MoveToElement(navigation2, navigation2.Size.Width / 2 + 40, navigation2.Size.Height / 2 + 131).Click().Build().Perform();
                brz3dvp.MoveAndClick(navigation2, navigation2.Size.Width / 2 + 40, navigation2.Size.Height / 2 + 131);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_25 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 2, 0, 0, 255, 2);
                //int PrismColorValBefore_30 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 3, 190, 190, 255, 2);
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (ColorValAfter_25 != ColorValBefore)
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
                //Steps 26::Right click on the Curved MPR control and drag the image.
                CurvedMpr = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                bool Step26 = brz3dvp.VerifyRightClickDragandDropImage(CurvedMpr, BluRingZ3DViewerPage.CurvedMPR);
                if (Step26)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 27::Select the rotate tool from the 3D tool box.
                RotateTool = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center);
                if (RotateTool)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 28::Click on the path displayed on the Curved MPR control and drag left and right.
                BeforeNavigationLocVal = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Actions step28 = new Actions(Driver);
                step28.MoveToElement(navigation1, 175, 75).ClickAndHold()
                    .MoveToElement(navigation1, 130, 75).MoveToElement(navigation1, 230, 75).Build().Perform();
                step28.Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                AfterNavigationLocVal = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
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
                //Step 29 ::Right click and drag the image from the controls under Curved MPR view.
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                String[] controls = new String[]{ BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage._3DPathNavigation, BluRingZ3DViewerPage.CurvedMPR };
                BeforeNavigation1LocVal = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigationone);
                BeforeNavigation2LocVal = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigationtwo);
                BeforeNavigation3LocVal = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigationthree);
                PageLoadWait.WaitForFrameLoad(10);
                foreach (String control in controls)
                {
                    location = brz3dvp.ControllerPoints(control);
                    xcoordinate = location.X;
                    ycoordinate = location.Y;
                    Cursor.Position = new System.Drawing.Point(xcoordinate - 10, ycoordinate - 10);
                    Thread.Sleep(2000);
                    BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                    Thread.Sleep(2000);
                    Cursor.Position = new System.Drawing.Point(xcoordinate - 10, ycoordinate + 60);
                    Thread.Sleep(2000);
                    BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                    Thread.Sleep(2000);
                }
                PageLoadWait.WaitForFrameLoad(10);
                AfterNavigation1LocVal = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigationone);
                AfterNavigation2LocVal = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigationtwo);
                AfterNavigation3LocVal = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigationthree);
                bool Step29_1 = false;
                if (BeforeNavigation1LocVal != AfterNavigation1LocVal && BeforeNavigation2LocVal != AfterNavigation2LocVal && BeforeNavigation3LocVal != AfterNavigation3LocVal)
                {
                    Logger.Instance.InfoLog("Right click drag and drop successful in Navigation controls");
                    Step29_1 = true;
                }
                ViewerContainer = brz3dvp.ViewerContainer();
                //bool Step29_2 = brz3dvp.VerifyRightClickDragandDropImage(ViewerContainer, BluRingZ3DViewerPage._3DPathNavigation);
                //bool Step29_3 = brz3dvp.VerifyRightClickDragandDropImage(CurvedMpr, BluRingZ3DViewerPage.CurvedMPR);
                if (Step29_1 && CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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
                //Step 30::Select the reset button from the 3D tool box.
                BeforeNavigationLocVal = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                RestetTool = brz3dvp.select3DTools(Z3DTools.Reset);
                AfterNavigationLocVal = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
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

        public TestCaseResult Test_163321(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result;
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

                String MPRPathLocValBefore, ThreeDPathLocValBefore, MPRPathLocValAfter, ThreeDPathLocValAfter;
                String BaseImages = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + "TempImages" + Path.DirectorySeparatorChar + "BaseImages" + Path.DirectorySeparatorChar + Config.BrowserType;
                Directory.CreateDirectory(BaseImages);
                String ColorSplitImages = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + "TempImages" + Path.DirectorySeparatorChar + "ColorSplitted" + Path.DirectorySeparatorChar + Config.BrowserType;
                Directory.CreateDirectory(ColorSplitImages);
                IWebElement Navigation1, Navigation2, CurvedMPRNavigation, Navigation3;
                String ResetLoc = "Loc: 0.0, 0.0, 0.0 mm";

                //step 01 , 02 & 03
                login.LoginIConnect(adminUserName, adminPassword);
                bool res = brz3dvp.searchandopenstudyin3D(objpatid, objthumbimg, layout: BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    throw new Exception("unable to launch study in 3D Viewer");
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }

                //step 04
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                new Actions(Driver).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 4) * 3, Navigation1.Size.Height / 2).Click().Build().Perform();
                //brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 4) * 3, Navigation1.Size.Height / 2);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                //new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 4, Navigation1.Size.Height / 2).Click().Build().Perform();
                brz3dvp.MoveAndClick(Navigation1, Navigation1.Size.Width / 4, Navigation1.Size.Height / 2);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(10);
                //int Curvedheight_04 = brz3dvp.CurvedMPRHeight(testid,ExecutedSteps + 42)[2].Y;
                if(CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 05
                res = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.CurvedMPR);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                new Actions(Driver).MoveToElement(CurvedMPRNavigation, CurvedMPRNavigation.Size.Width / 2, CurvedMPRNavigation.Size.Height / 4).ClickAndHold()
                    .MoveToElement(CurvedMPRNavigation, CurvedMPRNavigation.Size.Width / 2, (CurvedMPRNavigation.Size.Height / 4) * 3).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                MPRPathLocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                ThreeDPathLocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                //int Curvedheight_06 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps + 61)[2].Y;
                if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation)&& res)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 06
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.EnableOneViewupMode(CurvedMPRNavigation);
                PageLoadWait.WaitForFrameLoad(10);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    //int Curvedheight_07 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps + 71)[2].Y;
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 07
                brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.CurvedMPR);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                //new Actions(Driver).MoveToElement(CurvedMPRNavigation).DoubleClick().Build().Perform();
                bool res8 = brz3dvp.DisableOneUpViewMode(CurvedMPRNavigation);
                Logger.Instance.InfoLog("The resultant of resizing the viewport is : " + res8.ToString());
                PageLoadWait.WaitForFrameLoad(10);
                MPRPathLocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                ThreeDPathLocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                if(res8 && ThreeDPathLocValAfter != ThreeDPathLocValBefore && MPRPathLocValAfter != MPRPathLocValBefore)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 08
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Manual);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    new Actions(Driver).SendKeys("x").Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    Navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                    Accord.Point yellowpoint = brz3dvp.GetIntersectionPoints(Navigation3, testid, ExecutedSteps + 91, color: "yellow");
                    PageLoadWait.WaitForFrameLoad(10);
                    Thread.Sleep(5000);
                    Accord.Point bluepoint = brz3dvp.GetIntersectionPoints(Navigation3, testid, ExecutedSteps + 92, color: "blue", displacement : "Vertical");
                    //new Actions(Driver).MoveToElement(Navigation3, Convert.ToInt32(yellowpoint.X), Convert.ToInt32(yellowpoint.Y)).ClickAndHold()
                    //    .MoveToElement(Navigation3, Convert.ToInt32(bluepoint.X), Convert.ToInt32(bluepoint.Y)).Release().Build().Perform();
                    Thread.Sleep(5000);
                    int[] x1 = { (Int32)yellowpoint.X, (Int32)bluepoint.X };
                    int[] y1 = { (Int32)yellowpoint.Y, (Int32)bluepoint.Y };
                    Thread.Sleep(5000);
                    brz3dvp.Performdragdrop(Navigation3, (Int32)yellowpoint.X, (Int32)yellowpoint.Y, (Int32)bluepoint.X, (Int32)bluepoint.Y);

                    PageLoadWait.WaitForFrameLoad(10);
                    new Actions(Driver).SendKeys("x").Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                    //new Actions(Driver).MoveToElement(Navigation2, (Navigation2.Size.Width / 4) * 3, Navigation2.Size.Height / 2).Click().Build().Perform();
                    brz3dvp.MoveAndClick(Navigation2, (Navigation2.Size.Width / 4) * 3, Navigation2.Size.Height / 2);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    //new Actions(Driver).MoveToElement(Navigation2, Navigation2.Size.Width / 4, Navigation2.Size.Height / 2).Click().Build().Perform();
                    brz3dvp.MoveAndClick(Navigation2, Navigation2.Size.Width / 4, Navigation2.Size.Height / 2);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                   //int Curvedheight_09 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps + 91)[2].Y;
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 09
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    //int Curvedheightbefore_10 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps + 101)[2].Y;
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    new Actions(Driver).MoveToElement(CurvedMPRNavigation, CurvedMPRNavigation.Size.Width / 2, CurvedMPRNavigation.Size.Height / 4).ClickAndHold()
                        .MoveToElement(CurvedMPRNavigation, CurvedMPRNavigation.Size.Width / 2, (CurvedMPRNavigation.Size.Height / 4) * 3).Release().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    MPRPathLocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                    ThreeDPathLocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                    //int Curvedheightafter_10 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps + 102)[2].Y;
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                    {
                        result.steps[ExecutedSteps].StepPass();
                       
                        
                    }
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //Step 10 
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.EnableOneViewupMode(CurvedMPRNavigation);
                PageLoadWait.WaitForFrameLoad(10);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    //int Curvedheight_10 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps + 103)[2].Y;
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 11
                brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.CurvedMPR);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                //new Actions(Driver).MoveToElement(CurvedMPRNavigation).DoubleClick().Build().Perform();
                bool res11 = brz3dvp.DisableOneUpViewMode(CurvedMPRNavigation);
                Logger.Instance.InfoLog("The resultant of resizing the viewport is : " + res11.ToString());
                PageLoadWait.WaitForFrameLoad(10);
                MPRPathLocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                ThreeDPathLocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                List<string> ResetLayout = brz3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (ResetLayout[0] == ResetLoc && ResetLayout[1] == ResetLoc && ResetLayout[2] == ResetLoc && ResetLayout[3] == ResetLoc && ResetLayout[4] == ResetLoc && ResetLayout[5] == ResetLoc)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 12
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Manual);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    Navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                    Thread.Sleep(5000);
                    //new Actions(Driver).MoveToElement(Navigation3, (Navigation3.Size.Width / 4) * 3, Navigation3.Size.Height / 2).Build().Perform();
                    //Thread.Sleep(30000);
                    //new Actions(Driver).Click().Build().Perform();
                    brz3dvp.MoveAndClick(Navigation3, (Navigation3.Size.Width / 4) * 3, Navigation3.Size.Height / 2);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    //new Actions(Driver).MoveToElement(Navigation3, Navigation3.Size.Width / 4, Navigation3.Size.Height / 2).Build().Perform();
                    //Thread.Sleep(10000);
                    //new Actions(Driver).Click().Build().Perform();
                    brz3dvp.MoveAndClick(Navigation3, Navigation3.Size.Width / 4, Navigation3.Size.Height / 2);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    //int Curvedheight_12 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps + 121)[2].Y;
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 13
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    //int Curvedheightbefore_13 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps + 131)[2].Y;
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    new Actions(Driver).MoveToElement(CurvedMPRNavigation, CurvedMPRNavigation.Size.Width / 2, CurvedMPRNavigation.Size.Height / 4).ClickAndHold()
                        .MoveToElement(CurvedMPRNavigation, CurvedMPRNavigation.Size.Width / 2, (CurvedMPRNavigation.Size.Height / 4) * 3).Release().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    MPRPathLocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                    ThreeDPathLocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                    //int Curvedheightafter_13 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps + 132)[2].Y;
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                    {
                        result.steps[ExecutedSteps].StepPass();
                    }
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //Step 14
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.EnableOneViewupMode(CurvedMPRNavigation);
                PageLoadWait.WaitForFrameLoad(10);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    //int Curvedheight_13 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps + 133)[2].Y;
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 15
                brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.CurvedMPR);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                //new Actions(Driver).MoveToElement(CurvedMPRNavigation).DoubleClick().Build().Perform();
                bool res14 = brz3dvp.DisableOneUpViewMode(CurvedMPRNavigation);
                Logger.Instance.InfoLog("The resultant of resizing the viewport is : " + res14.ToString());
                PageLoadWait.WaitForFrameLoad(10);
                MPRPathLocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                ThreeDPathLocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                if (res14 && ThreeDPathLocValAfter != ThreeDPathLocValBefore && MPRPathLocValAfter != MPRPathLocValBefore)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 16
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Manual);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    //new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2), (Navigation1.Size.Height / 4) * 3).Click().Build().Perform();
                    brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2), (Navigation1.Size.Height / 4) * 3);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    //new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2, Navigation1.Size.Height / 4).Click().Build().Perform();
                    brz3dvp.MoveAndClick(Navigation1, Navigation1.Size.Width / 2, Navigation1.Size.Height / 4);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    PageLoadWait.WaitForFrameLoad(10);
                    //int Curvedheight_15 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps + 151)[1].X;
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 17
                res = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.CurvedMPR);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                //int Curvedheightbefore_17 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps + 171)[1].X;
                CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                new Actions(Driver).MoveToElement(CurvedMPRNavigation, CurvedMPRNavigation.Size.Width / 4, CurvedMPRNavigation.Size.Height / 2).ClickAndHold()
                    .MoveToElement(CurvedMPRNavigation, (CurvedMPRNavigation.Size.Width / 4) * 3, CurvedMPRNavigation.Size.Height / 2).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                MPRPathLocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                ThreeDPathLocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                //int Curvedheightafter_17 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps + 172)[1].X;
                if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation)&&res)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 18
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.EnableOneViewupMode(CurvedMPRNavigation);
                PageLoadWait.WaitForFrameLoad(10);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    //int Curvedheight_18 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps + 181)[1].X;
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 19
                brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.CurvedMPR);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                //new Actions(Driver).MoveToElement(CurvedMPRNavigation).DoubleClick().Build().Perform();
                bool res19 = brz3dvp.DisableOneUpViewMode(CurvedMPRNavigation);
                Logger.Instance.InfoLog("The resultant of resizing the viewport is : " + res19.ToString());
                PageLoadWait.WaitForFrameLoad(10);
                MPRPathLocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                ThreeDPathLocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                ResetLayout = brz3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (ResetLayout[0] == ResetLoc && ResetLayout[1] == ResetLoc && ResetLayout[2] == ResetLoc && ResetLayout[3] == ResetLoc && ResetLayout[4] == ResetLoc && ResetLayout[5] == ResetLoc)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 20
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Manual);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    new Actions(Driver).SendKeys("x").Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    Navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                    Thread.Sleep(5000);
                    Accord.Point yellowpoint = brz3dvp.GetIntersectionPoints(Navigation3, testid, ExecutedSteps + 201, color: "yellow");
                    PageLoadWait.WaitForFrameLoad(10);
                    Thread.Sleep(5000);
                    Accord.Point bluepoint = brz3dvp.GetIntersectionPoints(Navigation3, testid, ExecutedSteps + 202, color: "blue", displacement: "Vertical");
                    //new Actions(Driver).MoveToElement(Navigation3, Convert.ToInt32(yellowpoint.X), Convert.ToInt32(yellowpoint.Y)).ClickAndHold()
                    //    .MoveToElement(Navigation3, Convert.ToInt32(bluepoint.X), Convert.ToInt32(bluepoint.Y)).Release().Build().Perform();

                    PageLoadWait.WaitForFrameLoad(5);
                    Thread.Sleep(5000);
                    brz3dvp.Performdragdrop(Navigation3, (Int32)yellowpoint.X, (Int32)yellowpoint.Y, (Int32)bluepoint.X, (Int32)bluepoint.Y);

                    PageLoadWait.WaitForFrameLoad(10);
                    new Actions(Driver).SendKeys("x").Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                    //new Actions(Driver).MoveToElement(Navigation2, Navigation2.Size.Width / 2, (Navigation2.Size.Height / 4) * 3).Click().Build().Perform();
                    brz3dvp.MoveAndClick(Navigation2, Navigation2.Size.Width / 2, (Navigation2.Size.Height / 4) * 3);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    //new Actions(Driver).MoveToElement(Navigation2, Navigation2.Size.Width / 2, Navigation2.Size.Height / 4).Click().Build().Perform();
                    brz3dvp.MoveAndClick(Navigation2, Navigation2.Size.Width / 2, Navigation2.Size.Height / 4);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    //int Curvedheight_20 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps + 203)[1].X;
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 21
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    //int Curvedheightbefore_21 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps + 211)[1].X;
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    new Actions(Driver).MoveToElement(CurvedMPRNavigation, CurvedMPRNavigation.Size.Width / 4, CurvedMPRNavigation.Size.Height / 2).ClickAndHold()
                        .MoveToElement(CurvedMPRNavigation, (CurvedMPRNavigation.Size.Width / 4) * 3, CurvedMPRNavigation.Size.Height / 2).Release().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    MPRPathLocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                    ThreeDPathLocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                    //int Curvedheightafter_21 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps + 212)[1].X;
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                    {
                        result.steps[ExecutedSteps].StepPass();
                    }
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 22
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.EnableOneViewupMode(CurvedMPRNavigation);
                PageLoadWait.WaitForFrameLoad(10);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    //int Curvedheight_21 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps + 213)[1].X;
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 23
                brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.CurvedMPR);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                //new Actions(Driver).MoveToElement(CurvedMPRNavigation).DoubleClick().Build().Perform();
                bool res22 = brz3dvp.DisableOneUpViewMode(CurvedMPRNavigation);
                Logger.Instance.InfoLog("The resultant of resizing the viewport is : " + res22.ToString());
                PageLoadWait.WaitForFrameLoad(10);
                MPRPathLocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                ThreeDPathLocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                ResetLayout = brz3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (ResetLayout[0] == ResetLoc && ResetLayout[1] == ResetLoc && ResetLayout[2] == ResetLoc && ResetLayout[3] == ResetLoc && ResetLayout[4] == ResetLoc && ResetLayout[5] == ResetLoc)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 24
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Manual, BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    Navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                    //new Actions(Driver).MoveToElement(Navigation3, Navigation3.Size.Width / 2, ((Navigation3.Size.Height / 4) * 3) - 10).Click().Build().Perform();
                    brz3dvp.MoveAndClick(Navigation3, Navigation3.Size.Width / 2, ((Navigation3.Size.Height / 4) * 3) - 10);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    //new Actions(Driver).MoveToElement(Navigation3, Navigation3.Size.Width / 2, (Navigation3.Size.Height / 4) + 10).Click().Build().Perform();
                    brz3dvp.MoveAndClick(Navigation3, Navigation3.Size.Width / 2, (Navigation3.Size.Height / 4) + 10);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    //int Curvedheight_23 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps + 231)[1].X;
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 25
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                   // int Curvedheightbefore_24 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps + 241)[1].X;
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    new Actions(Driver).MoveToElement(CurvedMPRNavigation, CurvedMPRNavigation.Size.Width / 4, CurvedMPRNavigation.Size.Height / 2).ClickAndHold()
                        .MoveToElement(CurvedMPRNavigation, (CurvedMPRNavigation.Size.Width / 4) * 3, CurvedMPRNavigation.Size.Height / 2).Release().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    MPRPathLocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                    ThreeDPathLocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                    //int Curvedheightafter_24 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps + 242)[1].X;
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                    {
                        result.steps[ExecutedSteps].StepPass();
                    }
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //Step26
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.EnableOneViewupMode(CurvedMPRNavigation);
                PageLoadWait.WaitForFrameLoad(10);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    // int Curvedheight_24 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps + 243)[1].X;
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 27
                brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.CurvedMPR);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                //new Actions(Driver).MoveToElement(CurvedMPRNavigation).DoubleClick().Build().Perform();
                bool res25 = brz3dvp.DisableOneUpViewMode(CurvedMPRNavigation);
                Logger.Instance.InfoLog("The resultant of resizing the viewport is : " + res25.ToString());
                PageLoadWait.WaitForFrameLoad(10);
                MPRPathLocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                ThreeDPathLocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                ResetLayout = brz3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (ResetLayout[0] == ResetLoc && ResetLayout[1] == ResetLoc && ResetLayout[2] == ResetLoc && ResetLayout[3] == ResetLoc && ResetLayout[4] == ResetLoc && ResetLayout[5] == ResetLoc)
                {
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

        public TestCaseResult Test_163320(String testid, String teststeps, int stepcount)
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
            String AortaLoc = "Loc: 0.0, 34.0, 0.0 mm";
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1  - Log in iCA and Navigate to studies tab
                //step:2 - Search and load the 3D supported study in the universal viewer
                //step:3 - Select the Curved MPR option from the smart view drop down
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step3 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription, BluRingZ3DViewerPage.CurvedMPR);
                if (step3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                    throw new Exception("Failed to open study in 3D Viewer");

                //step:4 - Create a path by adding points in Aorta regions on MPR navigation 1-3 controls.
                Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage.Navigationone, AortaLoc, scrolllevel: 34);
                PageLoadWait.WaitForFrameLoad(5);
                String BeforeImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_Before.jpg";
                String AfterImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_After.jpg";
                IWebElement CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                DownloadImageFile(CurvedMPR, BeforeImagePath);
                IWebElement Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 30);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2, Navigation1.Size.Height / 2).Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveClickAndHold(Navigation1, Navigation1.Size.Width / 2 + 20, 30);
                PageLoadWait.WaitForFrameLoad(5);
                bool res4 = Z3dViewerPage.checkerrormsg();
                if (res4)
                    throw new Exception("Error Message Found");
                IWebElement Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(Navigation2).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation2, Navigation2.Size.Width / 2 + 20, 60);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(Navigation2).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                //Z3dViewerPage.Performdragdrop(Navigation2, (Navigation2.Size.Width / 2) + 20, Navigation2.Size.Height / 2, Navigation2.Size.Width - 40, 30);
                new Actions(Driver).MoveToElement(Navigation2, Navigation2.Size.Width - 40, 30).ClickAndHold()
                    .MoveToElement(Navigation2, (Navigation2.Size.Width / 2) + 20, Navigation2.Size.Height / 2).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(Navigation2).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                IWebElement Navigation3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                Z3dViewerPage.MoveAndClick(Navigation3, Navigation3.Size.Width / 2 + 20, 90);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                bool res4_1 = Z3dViewerPage.checkerrormsg();
                if (res4_1)
                    throw new Exception("Failed to find path");
                new Actions(Driver).MoveToElement(Navigation2).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                DownloadImageFile(CurvedMPR, AfterImagePath);
                if (!CompareImage(BeforeImagePath, AfterImagePath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:5 - Click on the settings button from the global tool bar and select the 3D settings option
                //step:6  -Set the Final image size to 50 % by moving the slider to the left and Click on the save button.
                Boolean step5 = Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.FinalImageSize, 50);
                PageLoadWait.WaitForFrameLoad(5);
                if (step5)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:7 - Window level cursor shows up on hovering over the image.
                Boolean step7 = Z3dViewerPage.select3DTools(Z3DTools.Window_Level);
                PageLoadWait.WaitForFrameLoad(5);
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

                //step:8  -Left click and drag on the image on the Curved MPR control.
                CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                String step8_Before = Z3dViewerPage.GetWindowLevelValue(BluRingZ3DViewerPage.CurvedMPR);
                Z3dViewerPage.Performdragdrop(CurvedMPR, CurvedMPR.Size.Width / 2 + 25, CurvedMPR.Size.Height / 2 + 25, RemoveCross: true);
                String step8_After = Z3dViewerPage.GetWindowLevelValue(BluRingZ3DViewerPage.CurvedMPR);
                if (step8_Before != step8_After)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:9  -From any one of the MPR navigation controls, Right click and delete a Control point.
                CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                BeforeImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_Before.jpg";
                AfterImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_After.jpg";
                DownloadImageFile(CurvedMPR, BeforeImagePath);
                Navigation3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                Z3dViewerPage.CurvedPathdeletor(Navigation3, Navigation3.Size.Width / 2 + 20, 90, "Delete Control Point", RemoveCross: true);
                PageLoadWait.WaitForFrameLoad(5);
                DownloadImageFile(CurvedMPR, AfterImagePath);
                if (!CompareImage(BeforeImagePath, AfterImagePath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:10 - Right click on the navigation control 1, then right click on the Curved drawing tool and select the Auto vessel option from the drop down.
                Boolean step10_1 = Z3dViewerPage.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                PageLoadWait.WaitForFrameLoad(5);
                List<string> step10_2 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                IWebElement ViewerContainer = Z3dViewerPage.ViewerContainer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if(step10_1 && step10_2[5] == ResetLoc && CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:11 - Create a path by adding points in Aorta regions on MPR navigation 1-3 controls.
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage.Navigationone, AortaLoc, scrolllevel: 34);
                PageLoadWait.WaitForFrameLoad(5);
                BeforeImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_Before.jpg";
                AfterImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_After.jpg";
                CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                DownloadImageFile(CurvedMPR, BeforeImagePath);
                Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                //Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 30);
                new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2 + 20, 30).Click().Build().Perform();
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2, Navigation1.Size.Height / 2).Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool);
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveClickAndHold(Navigation1, Navigation1.Size.Width / 2 + 20, 30);
                //Actions act11 = new Actions(Driver);
                //act11.MoveToElement(Navigation1, Navigation1.Size.Width / 2 + 20, 30).ClickAndHold().Build().Perform();
                //Thread.Sleep(3000);
                //act11.Release().Build().Perform();
                Z3dViewerPage.checkerrormsg(clickok:"y");
                Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(Navigation2).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation2, Navigation2.Size.Width / 2 + 25, 60);
                //new Actions(Driver).MoveToElement(Navigation2, Navigation2.Size.Width / 2 + 25, 60).Click().Build().Perform();
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Boolean checkissue = Z3dViewerPage.checkerrormsg();
                if (checkissue)
                    throw new Exception("Failed to find path");
                new Actions(Driver).MoveToElement(Navigation2).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                //Z3dViewerPage.Performdragdrop(Navigation2, (Navigation2.Size.Width / 2) + 20, Navigation2.Size.Height / 2, Navigation2.Size.Width - 40, 30);
                new Actions(Driver).MoveToElement(Navigation2, Navigation2.Size.Width - 40, 30).ClickAndHold()
                    .MoveToElement(Navigation2, (Navigation2.Size.Width / 2) + 20, Navigation2.Size.Height / 2).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(Navigation2).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Navigation3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                Z3dViewerPage.MoveAndClick(Navigation3, Navigation3.Size.Width / 2 + 20, 90);
                //new Actions(Driver).MoveToElement(Navigation3, Navigation3.Size.Width / 2 + 20, 90).Click().Build().Perform();
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(Navigation2).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                checkissue = Z3dViewerPage.checkerrormsg();
                if (checkissue)
                    throw new Exception("Failed to find path");
                DownloadImageFile(CurvedMPR, AfterImagePath);
                if (!CompareImage(BeforeImagePath, AfterImagePath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:12 - Repeat steps 5 - 9.
                BeforeImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_Before.jpg";
                AfterImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_After.jpg";
                Boolean step12_1 = Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.FinalImageSize, 50);
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step12_2 = Z3dViewerPage.select3DTools(Z3DTools.Window_Level);
                PageLoadWait.WaitForFrameLoad(5);
                CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                String step12_Before = Z3dViewerPage.GetWindowLevelValue(BluRingZ3DViewerPage.CurvedMPR);
                Z3dViewerPage.Performdragdrop(CurvedMPR, CurvedMPR.Size.Width / 2 + 25, CurvedMPR.Size.Height / 2 + 25, RemoveCross: true);
                String step12_After = Z3dViewerPage.GetWindowLevelValue(BluRingZ3DViewerPage.CurvedMPR);
                DownloadImageFile(CurvedMPR, BeforeImagePath);
                Navigation3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                Z3dViewerPage.CurvedPathdeletor(Navigation3, Navigation3.Size.Width / 2 + 20, 90, "Delete Control Point", RemoveCross: true, PostTool: Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                PageLoadWait.WaitForFrameLoad(5);
                DownloadImageFile(CurvedMPR, AfterImagePath);
                if (step12_1 && step12_2 && step12_Before != step12_After && !CompareImage(BeforeImagePath, AfterImagePath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:13 - Select the Auto (Colon) curve drawing tool from the 3D tool boxand create a path by adding points in Colon regions on MPR navigation 1-3 controls
                Boolean step13_1 = Z3dViewerPage.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                PageLoadWait.WaitForFrameLoad(5);
                List<string> step13_2 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                AortaLoc = "Loc: 0.0, 60.0, 0.0 mm";
                Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage.Navigationone, AortaLoc, scrolllevel: 60);
                PageLoadWait.WaitForFrameLoad(5);
                BeforeImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_Before.jpg";
                AfterImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_After.jpg";
                CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                DownloadImageFile(CurvedMPR, BeforeImagePath);
                Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(Navigation2).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2, ((Navigation1.Size.Height / 4) * 3));
                //new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2, ((Navigation1.Size.Height / 4) * 3)).Click().Build().Perform();
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(Navigation2).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveClickAndHold(Navigation1, Navigation1.Size.Width / 2, ((Navigation1.Size.Height / 4) * 3));
                PageLoadWait.WaitForFrameLoad(5);
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.checkerrormsg(clickok:"y");
                Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(Navigation2).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation2, Navigation2.Size.Width / 2 + 40, ((Navigation2.Size.Height / 4) * 3) + 30);
                //new Actions(Driver).MoveToElement(Navigation2, Navigation2.Size.Width / 2 + 40, ((Navigation2.Size.Height / 4) * 3) + 35).Click().Build().Perform();
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                checkissue = Z3dViewerPage.checkerrormsg();
                if (checkissue)
                    throw new Exception("Failed to find path");
                new Actions(Driver).MoveToElement(Navigation2).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                //Z3dViewerPage.Performdragdrop(Navigation2, (Navigation2.Size.Width / 2) + 45, Navigation2.Size.Height / 2, Navigation2.Size.Width / 3, (Navigation2.Size.Height / 4) * 3);
                new Actions(Driver).MoveToElement(Navigation2, Navigation2.Size.Width / 3, (Navigation2.Size.Height / 4) * 3).ClickAndHold()
                    .MoveToElement(Navigation2, (Navigation2.Size.Width / 2) + 45, Navigation2.Size.Height / 2).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(Navigation2).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Navigation3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                Z3dViewerPage.MoveAndClick(Navigation3, Navigation3.Size.Width / 2, ((Navigation2.Size.Height / 4) * 3) + 45);
                //new Actions(Driver).MoveToElement(Navigation3, Navigation3.Size.Width / 2, ((Navigation2.Size.Height / 4) * 3) + 45).Click().Build().Perform();
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(Navigation2).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                checkissue = Z3dViewerPage.checkerrormsg();
                if (checkissue)
                    throw new Exception("Failed to find path");
                DownloadImageFile(CurvedMPR, AfterImagePath);
                if (step13_1 && step13_2[2] == ResetLoc && step13_2[4] == ResetLoc && step13_2[5] == ResetLoc && !CompareImage(BeforeImagePath, AfterImagePath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:14 - Repeat steps 5-9
                BeforeImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_Before.jpg";
                AfterImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_After.jpg";
                Boolean step14_1 = Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.FinalImageSize, 50);
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step14_2 = Z3dViewerPage.select3DTools(Z3DTools.Window_Level);
                PageLoadWait.WaitForFrameLoad(5);
                CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                String step14_Before = Z3dViewerPage.GetWindowLevelValue(BluRingZ3DViewerPage.CurvedMPR);
                Z3dViewerPage.Performdragdrop(CurvedMPR, CurvedMPR.Size.Width / 2 + 25, CurvedMPR.Size.Height / 2 + 25, RemoveCross: true);
                String step14_After = Z3dViewerPage.GetWindowLevelValue(BluRingZ3DViewerPage.CurvedMPR);
                DownloadImageFile(CurvedMPR, BeforeImagePath);
                Navigation3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                Z3dViewerPage.CurvedPathdeletor(Navigation3, Navigation3.Size.Width / 2, ((Navigation2.Size.Height / 4) * 3) + 45, "Delete Control Point", RemoveCross: true);
                PageLoadWait.WaitForFrameLoad(5);
                DownloadImageFile(CurvedMPR, AfterImagePath);
                if (step14_1 && step14_2 && step14_Before != step14_After && !CompareImage(BeforeImagePath, AfterImagePath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:15 - Select the Reset button from the 3D tool box
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                List<string> step15 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (step15[0] == ResetLoc && step15[1] == ResetLoc && step15[2] == ResetLoc && step15[3] == ResetLoc && step15[4] == ResetLoc && step15[5] == ResetLoc)
                {
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

        public TestCaseResult Test_163322(String testid, String teststeps, int stepcount)
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
                //String NavLocation34 = objTestRequirement.Split('|')[0];
                //String NavLocation60 = objTestRequirement.Split('|')[1];
                //step 01 :: Search and load a 3D supported study in the universal viewer.
                login.LoginIConnect(adminUserName, adminPassword);
                bool StudyLoad = brz3dvp.searchandopenstudyin3D("FFP", objthumbimg , BluRingZ3DViewerPage.CurvedMPR, field: "acc");
                if (StudyLoad)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    throw new Exception("unable to launch study in 3D Viewer");
                }
                //Steps 2::Create a path by adding the points from left to right.
                new Actions(Driver).SendKeys("X").Build().Perform();
                IWebElement navigation1  = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                brz3dvp.MoveAndClick(navigation1, navigation1.Size.Width / 6, navigation1.Size.Height / 2);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                //new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 6, navigation1.Size.Height / 2).Click().Build().Perform();
                brz3dvp.MoveAndClick(navigation1, navigation1.Size.Width - 10, navigation1.Size.Height / 2);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                //new Actions(Driver). MoveToElement(navigation1, navigation1.Size.Width - 10, navigation1.Size.Height / 2).Click().Build().Perform();
                Thread.Sleep(10000);
                bool check2 = brz3dvp.checkerrormsg();
                if (check2)
                    throw new Exception("Failed to find path in step 2");
                //Verification::Orientation of blue cross reference line is in Vertical position in the Curved MPR control.
                IWebElement CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                //int Curvedheight_2 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps )[2].X;
                //if(Curvedheight_2 <= 35 || Curvedheight_2 >= 30)
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
                IWebElement Curved = brz3dvp.controlelement("Curved MPR");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Curved))
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
                //Step 3::Select the Scroll tool from the 3D tool box.
                bool ScrollTool = brz3dvp.select3DTools(Z3DTools.Scrolling_Tool , BluRingZ3DViewerPage._3DPathNavigation);
                if(ScrollTool)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 4::Click the left mouse button on the image displayed in MPR path navigation control and drag the mouse downwards.
                IWebElement MprPathNavControl = brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                for (int i = 0; i < 30; i++)
                {
                    brz3dvp.Performdragdrop(MprPathNavControl, MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height / 2 , MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height / 6);
                    //new Actions(Driver).MoveToElement(MprPathNavControl, MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height / 10).ClickAndHold()
                    //    .MoveToElement(MprPathNavControl, MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height - 30).Release().Build().Perform();
                }
                //Verification::Blue reference cross line moves from left to right.
                PageLoadWait.WaitForFrameLoad(10);
                //int Curvedheight_4 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps )[3].X;
                //if (Curvedheight_4 > Curvedheight_2)
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
                Curved = brz3dvp.controlelement("Curved MPR");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Curved))
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
                //Steps 5:::Scrolling can be also done using Mouse wheel.=== Scroll up
                MprPathNavControl = brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                for (int i = 0; i < 30; i++)
                {
                    brz3dvp.Performdragdrop(MprPathNavControl, MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height / 6, MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height / 2);
                    //new Actions(Driver).MoveToElement(MprPathNavControl, MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height - 30).ClickAndHold()
                    //.MoveToElement(MprPathNavControl, MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height /10).Release().Build().Perform();
                }
                //Verification::Blue reference cross line moves from right to left.
                PageLoadWait.WaitForFrameLoad(10);
                //int Curvedheight_5 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps)[3].X;
                //if (Curvedheight_5 == Curvedheight_2)
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
                Curved = brz3dvp.controlelement("Curved MPR");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Curved))
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
                //Step 6 ::Select the Reset button from the 3D tool box.
                string BeforeLocVal = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                bool Reset = brz3dvp.select3DTools(Z3DTools.Reset , BluRingZ3DViewerPage.MPRPathNavigation);
                PageLoadWait.WaitForFrameLoad(10);
                string AfterLocVal = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                if (Reset && BeforeLocVal != AfterLocVal)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 7 :: Create a path by adding the points from right to left.
                brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Manual);
                navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                brz3dvp.MoveAndClick(navigation1, (navigation1.Size.Width / 4) * 3, navigation1.Size.Height / 2);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                //new Actions(Driver).MoveToElement(navigation1, (navigation1.Size.Width / 4) * 3, navigation1.Size.Height / 2).Click().Build().Perform();
                Thread.Sleep(2000);
                brz3dvp.MoveAndClick(navigation1, navigation1.Size.Width / 4, navigation1.Size.Height / 2);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                //new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 4, navigation1.Size.Height / 2).Click().Build().Perform();
                //Verification::Orientation of blue cross reference line is in Vertical position in the Curved MPR control.
                //int Curvedheight_7 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps)[3].X;
                //if (Curvedheight_7 <= 230 || Curvedheight_7 >= 220)
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
                Thread.Sleep(10000);
                bool check7 = brz3dvp.checkerrormsg();
                if (check7)
                    throw new Exception("Failed to find path in step 7");
                Curved = brz3dvp.controlelement("Curved MPR");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Curved))
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
                //Step 8::Select the Scroll tool from the 3D tool box.
                ScrollTool = brz3dvp.select3DTools(Z3DTools.Scrolling_Tool, BluRingZ3DViewerPage._3DPathNavigation);
                if (ScrollTool)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 9::Click the left mouse button on the image displayed in MPR path navigation control and drag the mouse downwards.
                MprPathNavControl = brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                for (int i = 0; i < 30; i++)
                {
                    brz3dvp.Performdragdrop(MprPathNavControl, MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height / 2 , MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height / 6);
                    //new Actions(Driver).MoveToElement(MprPathNavControl, MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height / 10).ClickAndHold()
                    //.MoveToElement(MprPathNavControl, MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height - 30).Release().Build().Perform();
                }
                //Verification::Blue reference cross line moves from left to right.
                PageLoadWait.WaitForFrameLoad(10);
                //int Curvedheight_9 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps)[3].X;
                //if (Curvedheight_9 < Curvedheight_7)
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
                Curved = brz3dvp.controlelement("Curved MPR");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Curved))
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
                //Step 10::Click the left mouse button on the image displayed in MPR path navigation control and drag the mouse upwards.
                MprPathNavControl = brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                for (int i = 0; i < 30; i++)
                {
                    brz3dvp.Performdragdrop(MprPathNavControl, MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height / 6 , MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height / 2);
                    //new Actions(Driver).MoveToElement(MprPathNavControl, MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height - 30).ClickAndHold()
                    //.MoveToElement(MprPathNavControl, MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height/10).Release().Build().Perform();
                }
                PageLoadWait.WaitForFrameLoad(10);
                //int Curvedheight_10 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps)[3].X;
                //if (Curvedheight_10 == Curvedheight_7)
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
                Curved = brz3dvp.controlelement("Curved MPR");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Curved))
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
                //Steps 11::Select the Reset button from the 3D tool box.
                BeforeLocVal = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                Reset = brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.MPRPathNavigation);
                PageLoadWait.WaitForFrameLoad(10);
                AfterLocVal = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                if (Reset && BeforeLocVal != AfterLocVal)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 12::Create a path by adding the points from top to bottom.
                brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Manual);
                navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                brz3dvp.MoveAndClick(navigation1, (navigation1.Size.Width / 2), navigation1.Size.Height / 4);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                // new Actions(Driver).MoveToElement(navigation1, (navigation1.Size.Width / 2), navigation1.Size.Height / 4).Click().Build().Perform();
                Thread.Sleep(2000);
                brz3dvp.MoveAndClick(navigation1, navigation1.Size.Width / 2, (navigation1.Size.Height / 4) * 3);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                //new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2, (navigation1.Size.Height / 4)*3).Click().Build().Perform();
                //Verification::Orientation of blue cross reference line is in horizontal position in the Curved MPR control.
                PageLoadWait.WaitForFrameLoad(10);
                //int Curvedheight_12 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps)[0].Y;
                //if (Curvedheight_12 <= 288 || Curvedheight_12 >= 282)
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
                Curved = brz3dvp.controlelement("Curved MPR");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Curved))
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
                //Step 13::Select the Scroll tool from the 3D tool box.
                ScrollTool = brz3dvp.select3DTools(Z3DTools.Scrolling_Tool, BluRingZ3DViewerPage._3DPathNavigation);
                if (ScrollTool)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 14::Click the left mouse button on the image displayed in MPR path navigation control and drag the mouse downwards.
                MprPathNavControl = brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                for (int i = 0; i < 30; i++)
                {
                    brz3dvp.Performdragdrop(MprPathNavControl, MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height / 2 , MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height / 6);
                    //new Actions(Driver).MoveToElement(MprPathNavControl, MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height / 10).ClickAndHold()
                    //    .MoveToElement(MprPathNavControl, MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height - 30).Release().Build().Perform();
                }
                //Verification::Blue reference cross line moves from left to right.
                PageLoadWait.WaitForFrameLoad(10);
                //int Curvedheight_14 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps)[0].Y;
                //if (Curvedheight_14 <= Curvedheight_12)
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
                Curved = brz3dvp.controlelement("Curved MPR");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Curved))
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
                //Step 15::Click the left mouse button on the image displayed in MPR path navigation control and drag the mouse Upwards.
                MprPathNavControl = brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                for (int i = 0; i < 30; i++)
                {
                    brz3dvp.Performdragdrop(MprPathNavControl, MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height / 6 , MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height / 2);
                    //new Actions(Driver).MoveToElement(MprPathNavControl, MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height - 30).ClickAndHold()
                    //.MoveToElement(MprPathNavControl, MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height / 10).Release().Build().Perform();
                }
                PageLoadWait.WaitForFrameLoad(10);
                //int Curvedheight_15 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps)[0].Y;
                ////if (Curvedheight_15 == Curvedheight_12)
                //if (Curvedheight_15 != Curvedheight_14)
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
                Curved = brz3dvp.controlelement("Curved MPR");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Curved))
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
                //Step 16::Select the Reset button from the 3D tool box.
                BeforeLocVal = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                Reset = brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.MPRPathNavigation);
                PageLoadWait.WaitForFrameLoad(10);
                AfterLocVal = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                if (Reset && BeforeLocVal != AfterLocVal)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 17::Create a path by adding the points from bottom to top.
                brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Manual);
                navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                brz3dvp.MoveAndClick(navigation1, (navigation1.Size.Width / 2), (navigation1.Size.Height / 4) * 3);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                //new Actions(Driver).MoveToElement(navigation1, (navigation1.Size.Width / 2), (navigation1.Size.Height / 4)*3).Click().Build().Perform();
                Thread.Sleep(2000);
                brz3dvp.MoveAndClick(navigation1, navigation1.Size.Width / 2, navigation1.Size.Height / 4);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                //new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2, navigation1.Size.Height / 4).Click().Build().Perform();
                //Verification::Orientation of blue cross reference line is in Vertical position in the Curved MPR control.
                //PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(10000);
                bool check17 = brz3dvp.checkerrormsg();
                if (check17)
                    throw new Exception("Failed to find path in step 17");
                int Curvedheight_17 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps)[0].Y;
                //if (Curvedheight_17 <= 82 || Curvedheight_17 >= 74)
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
                Curved = brz3dvp.controlelement("Curved MPR");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Curved))
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
                //=============================Steps need to add for selecting Scroll Tool===========================
                ScrollTool = brz3dvp.select3DTools(Z3DTools.Scrolling_Tool, BluRingZ3DViewerPage._3DPathNavigation);
                //===================================================================================================
                //Step 18::Click the left mouse button on the image displayed in MPR path navigation control and drag the mouse downwards.
                MprPathNavControl = brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                for (int i = 0; i < 30; i++)
                {
                    brz3dvp.Performdragdrop(MprPathNavControl, MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height / 2 , MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height / 6);
                    //new Actions(Driver).MoveToElement(MprPathNavControl, MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height / 10).ClickAndHold()
                    //.MoveToElement(MprPathNavControl, MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height - 30).Release().Build().Perform();
                }
                //Verification::Blue reference cross line moves from left to right.
                PageLoadWait.WaitForFrameLoad(10);
                //int Curvedheight_18 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps)[0].Y;
                //if (Curvedheight_18 > Curvedheight_17)
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
                Curved = brz3dvp.controlelement("Curved MPR");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Curved))
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
                //Step 19::Click the left mouse button on the image displayed in MPR path navigation control and drag the mouse Upwards
                MprPathNavControl = brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                for (int i = 0; i < 30; i++)
                {
                    brz3dvp.Performdragdrop(MprPathNavControl, MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height / 6 , MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height / 2);
                   // new Actions(Driver).MoveToElement(MprPathNavControl, MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height - 30).ClickAndHold()
                   //.MoveToElement(MprPathNavControl, MprPathNavControl.Size.Width / 2, MprPathNavControl.Size.Height / 10).Release().Build().Perform();
                }
                PageLoadWait.WaitForFrameLoad(10);
                int Curvedheight_19 = brz3dvp.CurvedMPRHeight(testid, ExecutedSteps)[0].Y;
                if (Curvedheight_19 == Curvedheight_17)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 20::Select the Reset button from the 3D tool box
                BeforeLocVal = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                Reset = brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.MPRPathNavigation);
                PageLoadWait.WaitForFrameLoad(10);
                AfterLocVal = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                if (Reset && BeforeLocVal != AfterLocVal)
                {
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

        public TestCaseResult Test_163323(String testid, String teststeps, int stepcount)
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
                //Step 1 From the Universal viewer , Select a 3D supported series and Select the Curved MPR option from the smart view drop down.
                z3dvp.Deletefiles(testcasefolder);
                bool res = z3dvp.searchandopenstudyin3D(Patientid, thumbnailcaption, layout: BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    throw new Exception("Unable to open study due to exception ");
                else
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                }

                //Step 2 Create a Diagonal path by adding the points from top left to bottom right in MPR navigation controls.Reference attachment "Diagonal path" for an idea of what this should look like.
                IWebElement INavigation1= z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                bool btool2=z3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Manual, BluRingZ3DViewerPage.Navigationone);
                bool bflag2 = false;
                new Actions(Driver).SendKeys("D").Build().Perform();
                Thread.Sleep(500);
                new Actions(Driver).MoveToElement(INavigation1).SendKeys("X").Build().Perform();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (btool2)
                {
                    
                    Thread.Sleep(500);
                    //     new Actions(Driver).MoveToElement(INavigation1, 0, 60).ClickAndHold().Release().Build().Perform();
                    //new Actions(Driver).MoveToElement(INavigation1, INavigation1.Size.Width - (INavigation1.Size.Width - 10), INavigation1.Size.Height - (INavigation1.Size.Height - 10)).ClickAndHold().Release().Build().Perform();
                    z3dvp.MoveAndClick(INavigation1, INavigation1.Size.Width - (INavigation1.Size.Width - 10), INavigation1.Size.Height - (INavigation1.Size.Height - 10));
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(5000);
                    //new Actions(Driver).MoveToElement(INavigation1, INavigation1.Size.Width/2, INavigation1.Size.Height/ 2).ClickAndHold().Release().Build().Perform();
                    z3dvp.MoveAndClick(INavigation1, INavigation1.Size.Width / 2, INavigation1.Size.Height / 2);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(5000);
                    //new Actions(Driver).MoveToElement(INavigation1, INavigation1.Size.Width -20, INavigation1.Size.Height-20).ClickAndHold().Release().Build().Perform();
                    z3dvp.MoveAndClick(INavigation1, INavigation1.Size.Width - 20, INavigation1.Size.Height - 80);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(10000);
                    bool check2 = z3dvp.checkerrormsg();
                    if (check2)
                        throw new Exception("Failed to find path in step 2");
                    if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), pixelTolerance: 50))
                    {
                        bflag2 = true;
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                }
                new Actions(Driver).SendKeys("D").Build().Perform();
                Thread.Sleep(500);
                //  new Actions(Driver).MoveToElement(INavigation1).SendKeys("X").Build().Perform();
                if (bflag2==false)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 3 Select the Scroll tool from the 3D tool box.
                bool btool3 = z3dvp.select3DTools(Z3DTools.Scrolling_Tool, BluRingZ3DViewerPage.MPRPathNavigation);
                if(btool3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 4 Click the left mouse button on the image displayed in MPR/3D path navigation control and drag the mouse downwards.
                IWebElement IMprPath = z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((IMprPath.Location.X + 100), (IMprPath.Location.Y + 150));
                Thread.Sleep(1000);
                for (int i = 0; i < 20; i++)
                {
                    BasePage.mouse_event(0x0800, 0, 0, -15, 0);
                    Thread.Sleep(1000);
                }
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], INavigation1, pixelTolerance: 30))
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

                //Step 5 Select the Reset button from the 3D tool box.
                z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.MPRPathNavigation);
                Thread.Sleep(10000);
                List<string> result5 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result5[0] == result5[1] && result5[2] == result5[3] && result5[4] == result5[5] && slocationvalue == (result5[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 6 Create a horizontal path by adding the points left to right.
                bool btool6 = z3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Manual, BluRingZ3DViewerPage.Navigationone);
                bool bflag6 = false;
                //new Actions(Driver).MoveToElement(INavigation1).SendKeys("X").Build().Perform();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (btool6)
                {

                    Thread.Sleep(500);
                    //new Actions(Driver).MoveToElement(INavigation1, INavigation1.Size.Width-(INavigation1.Size.Width-10), INavigation1.Size.Height / 2).ClickAndHold().Release().Build().Perform();
                    z3dvp.MoveAndClick(INavigation1, INavigation1.Size.Width - (INavigation1.Size.Width - 10), INavigation1.Size.Height / 2);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(5000);
                    //new Actions(Driver).MoveToElement(INavigation1, INavigation1.Size.Width / 2, INavigation1.Size.Height / 2).ClickAndHold().Release().Build().Perform();
                    z3dvp.MoveAndClick(INavigation1, INavigation1.Size.Width / 2, INavigation1.Size.Height / 2);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(5000);
                    //new Actions(Driver).MoveToElement(INavigation1, INavigation1.Size.Width - 20, INavigation1.Size.Height /2).ClickAndHold().Release().Build().Perform();
                    z3dvp.MoveAndClick(INavigation1, INavigation1.Size.Width - 20, INavigation1.Size.Height / 2);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(10000);
                    bool check6 = z3dvp.checkerrormsg();
                    if (check6)
                        throw new Exception("Failed to find path in step 6");
                    if (CompareImage(result.steps[ExecutedSteps], INavigation1, pixelTolerance: 50))
                    {
                        bflag6 = true;
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                }
                if (bflag6 == false)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 7 Select the Scroll tool from the 3D tool box.
                bool btool7 = z3dvp.select3DTools(Z3DTools.Scrolling_Tool, BluRingZ3DViewerPage.MPRPathNavigation);
                if (btool7)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 8 Click the left mouse button on the image displayed in MPR/3D path navigation control and drag the mouse downwards.
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((IMprPath.Location.X + 100), (IMprPath.Location.Y + 150));
                Thread.Sleep(1000);
                for (int i = 0; i < 20; i++)
                {
                    BasePage.mouse_event(0x0800, 0, 0, -15, 0);
                    Thread.Sleep(1000);
                }
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], INavigation1, pixelTolerance: 30))
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

                //Step 9 Select the Reset button from the 3D tool box
                z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.MPRPathNavigation);
                Thread.Sleep(10000);
                List<string> result9 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result9[0] == result9[1] && result9[2] == result9[3] && result9[4] == result9[5] && slocationvalue == (result9[4]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 10 Create a Vertical path by adding the points top to bottom in MPR navigation control.
                bool btool10 = z3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Manual, BluRingZ3DViewerPage.Navigationone);
                bool bflag10 = false;
            //    new Actions(Driver).MoveToElement(INavigation1).SendKeys("X").Build().Perform();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (btool10)
                {

                    Thread.Sleep(500);
                    //new Actions(Driver).MoveToElement(INavigation1, INavigation1.Size.Width/2, INavigation1.Size.Height -(INavigation1.Size.Height-10)).ClickAndHold().Release().Build().Perform();
                    z3dvp.MoveAndClick(INavigation1, INavigation1.Size.Width / 2, INavigation1.Size.Height - (INavigation1.Size.Height - 10));
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(5000);
                    //new Actions(Driver).MoveToElement(INavigation1, INavigation1.Size.Width / 2, INavigation1.Size.Height / 2).ClickAndHold().Release().Build().Perform();
                    z3dvp.MoveAndClick(INavigation1, INavigation1.Size.Width / 2, INavigation1.Size.Height / 2);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(5000);
                    //new Actions(Driver).MoveToElement(INavigation1, INavigation1.Size.Width /2, INavigation1.Size.Height -20).ClickAndHold().Release().Build().Perform();
                    z3dvp.MoveAndClick(INavigation1, INavigation1.Size.Width / 2, INavigation1.Size.Height - 80);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(10000);
                    bool check10 = z3dvp.checkerrormsg();
                    if (check10)
                        throw new Exception("Failed to find path in step 10");
                    if (CompareImage(result.steps[ExecutedSteps], INavigation1, pixelTolerance: 50))
                    {
                        bflag10 = true;
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                }
                if (bflag10 == false)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 11 Select the Scroll tool from the 3D tool box.
                bool btool11 = z3dvp.select3DTools(Z3DTools.Scrolling_Tool, BluRingZ3DViewerPage.MPRPathNavigation);
                if (btool11)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 12 Click the left mouse button on the image displayed in MPR/3D path navigation control and drag the mouse downwards.
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((IMprPath.Location.X + 100), (IMprPath.Location.Y + 150));
                Thread.Sleep(1000);
                for (int i = 0; i < 20; i++)
                {
                    BasePage.mouse_event(0x0800, 0, 0, -15, 0);
                    Thread.Sleep(1000);
                }
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], INavigation1, pixelTolerance: 30))
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

                //step 13 Select the Reset button from the 3D tool box.
                z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.MPRPathNavigation);
                Thread.Sleep(10000);
                List<string> result13 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result13[0] == result13[1] && result13[2] == result13[3] && result13[4] == result13[5] && slocationvalue == (result13[4]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 14 Create a Curved path in MPR navigation control.
                bool btool14 = z3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Manual, BluRingZ3DViewerPage.Navigationone);
                bool bflag14 = false;
          //      new Actions(Driver).MoveToElement(INavigation1).SendKeys("X").Build().Perform();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (btool14)
                {

                    Thread.Sleep(2000);
                    //new Actions(Driver).MoveToElement(INavigation1, INavigation1.Size.Width / 2, INavigation1.Size.Height - 320).ClickAndHold().Release().Build().Perform();
                    z3dvp.MoveAndClick(INavigation1, INavigation1.Size.Width / 2, INavigation1.Size.Height - 320);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(5000);
                    //new Actions(Driver).MoveToElement(INavigation1, INavigation1.Size.Width - (INavigation1.Size.Width / 2 - 80), INavigation1.Size.Height - 270).ClickAndHold().Release().Build().Perform();
                    z3dvp.MoveAndClick(INavigation1, INavigation1.Size.Width - (INavigation1.Size.Width / 2 - 80), INavigation1.Size.Height - 270);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(5000);
                    //new Actions(Driver).MoveToElement(INavigation1, INavigation1.Size.Width - (INavigation1.Size.Width / 2 - 80), INavigation1.Size.Height - 240).ClickAndHold().Release().Build().Perform();
                    z3dvp.MoveAndClick(INavigation1, INavigation1.Size.Width - (INavigation1.Size.Width / 2 - 80), INavigation1.Size.Height - 240);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(5000);
                    //new Actions(Driver).MoveToElement(INavigation1, INavigation1.Size.Width - (INavigation1.Size.Width / 2 - 80), INavigation1.Size.Height - 180).ClickAndHold().Release().Build().Perform();
                    z3dvp.MoveAndClick(INavigation1, INavigation1.Size.Width - (INavigation1.Size.Width / 2 - 80), INavigation1.Size.Height - 180);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(5000);
                    //new Actions(Driver).MoveToElement(INavigation1, INavigation1.Size.Width / 2, INavigation1.Size.Height - 20).ClickAndHold().Release().Build().Perform();
                    z3dvp.MoveAndClick(INavigation1, INavigation1.Size.Width / 2, INavigation1.Size.Height - 80);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(10000);
                    bool check14 = z3dvp.checkerrormsg();
                    if (check14)
                        throw new Exception("Failed to find path in step 14");
                    if (CompareImage(result.steps[ExecutedSteps], INavigation1, pixelTolerance: 50))
                    {
                        bflag14 = true;
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                }
                if (bflag14 == false)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 15 Select the Scroll tool from the 3D tool box.
                bool btool15 = z3dvp.select3DTools(Z3DTools.Scrolling_Tool, BluRingZ3DViewerPage.MPRPathNavigation);
                if (btool15)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 16  Click the left mouse button on the image displayed in MPR/3D path navigation control and drag the mouse downwards.
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((IMprPath.Location.X + 100), (IMprPath.Location.Y + 150));
                Thread.Sleep(1000);
                for (int i = 0; i < 20; i++)
                {
                    BasePage.mouse_event(0x0800, 0, 0, -15, 0);
                    Thread.Sleep(1000);
                }
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], INavigation1, pixelTolerance: 30))
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
                //Step 17 Click the reset button 
                z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.MPRPathNavigation);
                Thread.Sleep(10000);
                List<string> result17 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result17[0] == result17[1] && result17[2] == result17[3] && result17[4] == result17[5] && slocationvalue == (result17[4]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 18 Create a zigzag path in MPR navigation control.
                bool btool18 = z3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Manual, BluRingZ3DViewerPage.Navigationone);
                bool bflag18 = false;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (btool18)
                {

                    Thread.Sleep(500);
                    //new Actions(Driver).MoveToElement(INavigation1, INavigation1.Size.Width / 2, INavigation1.Size.Height - 320).ClickAndHold().Release().Build().Perform();
                    z3dvp.MoveAndClick(INavigation1, INavigation1.Size.Width / 2, INavigation1.Size.Height - 320);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(5000);
                    //new Actions(Driver).MoveToElement(INavigation1, INavigation1.Size.Width - (INavigation1.Size.Width / 2 - 80), INavigation1.Size.Height - 270).ClickAndHold().Release().Build().Perform();
                    z3dvp.MoveAndClick(INavigation1, INavigation1.Size.Width - (INavigation1.Size.Width / 2 - 80), INavigation1.Size.Height - 270);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(5000);
                    //new Actions(Driver).MoveToElement(INavigation1, INavigation1.Size.Width / 2, INavigation1.Size.Height - 240).ClickAndHold().Release().Build().Perform();
                    z3dvp.MoveAndClick(INavigation1, INavigation1.Size.Width / 2, INavigation1.Size.Height - 240);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(5000);
                    //new Actions(Driver).MoveToElement(INavigation1, INavigation1.Size.Width - (INavigation1.Size.Width / 2 - 80), INavigation1.Size.Height - 180).ClickAndHold().Release().Build().Perform();
                    z3dvp.MoveAndClick(INavigation1, INavigation1.Size.Width - (INavigation1.Size.Width / 2 - 80), INavigation1.Size.Height - 180);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(5000);
                    //new Actions(Driver).MoveToElement(INavigation1, INavigation1.Size.Width / 2, INavigation1.Size.Height - 200).ClickAndHold().Release().Build().Perform();
                    z3dvp.MoveAndClick(INavigation1, INavigation1.Size.Width / 2, INavigation1.Size.Height - 200);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(5000);
                    //new Actions(Driver).MoveToElement(INavigation1, INavigation1.Size.Width - (INavigation1.Size.Width / 2 - 80), INavigation1.Size.Height - 120).ClickAndHold().Release().Build().Perform();
                    z3dvp.MoveAndClick(INavigation1, INavigation1.Size.Width - (INavigation1.Size.Width / 2 - 80), INavigation1.Size.Height - 120);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(5000);
                    //new Actions(Driver).MoveToElement(INavigation1, INavigation1.Size.Width / 2, INavigation1.Size.Height - 20).ClickAndHold().Release().Build().Perform();
                    z3dvp.MoveAndClick(INavigation1, INavigation1.Size.Width / 2, INavigation1.Size.Height - 80);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(10000);
                    bool check18 = z3dvp.checkerrormsg();
                    if (check18)
                        throw new Exception("Failed to find path in step 18");
                    if (CompareImage(result.steps[ExecutedSteps], INavigation1, pixelTolerance: 50))
                    {
                        bflag18 = true;
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                }
                if (bflag18 == false)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 19 Select the Scroll tool from the 3D tool box.
                bool btool19 = z3dvp.select3DTools(Z3DTools.Scrolling_Tool, BluRingZ3DViewerPage.MPRPathNavigation);
                if (btool19)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 20 Click the left mouse button on the image displayed in MPR/3D path navigation control and drag the mouse downwards.
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((IMprPath.Location.X + 100), (IMprPath.Location.Y + 150));
                Thread.Sleep(1000);
                for (int i = 0; i < 20; i++)
                {
                    BasePage.mouse_event(0x0800, 0, 0, -15, 0);
                    Thread.Sleep(1000);
                }
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], INavigation1, pixelTolerance: 30))
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

                //Step 21 Select the Reset button from the 3D tool box.
                z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.MPRPathNavigation);
                Thread.Sleep(10000);
                List<string> result21 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result21[0] == result21[1] && result21[2] == result21[3] && result21[4] == result21[5] && slocationvalue == (result21[4]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step 22 Create a path by adding the points in MPR navigation controls.
                bool btool22 = z3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Manual, BluRingZ3DViewerPage.Navigationone);
                bool bflag22 = false;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (btool22)
                {
                    Thread.Sleep(500);
                    //new Actions(Driver).MoveToElement(INavigation1, INavigation1.Size.Width / 2, INavigation1.Size.Height - 320).ClickAndHold().Release().Build().Perform();
                    z3dvp.MoveAndClick(INavigation1, INavigation1.Size.Width / 2, INavigation1.Size.Height - 320);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(5000);
                    //new Actions(Driver).MoveToElement(INavigation1, INavigation1.Size.Width - (INavigation1.Size.Width / 2 - 80), INavigation1.Size.Height - 270).ClickAndHold().Release().Build().Perform();
                    z3dvp.MoveAndClick(INavigation1, INavigation1.Size.Width - (INavigation1.Size.Width / 2 - 80), INavigation1.Size.Height - 270);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(10000);
                    bool check22 = z3dvp.checkerrormsg();
                    if (check22)
                        throw new Exception("Failed to find path in step 22");
                    Thread.Sleep(5000);
                    if (CompareImage(result.steps[ExecutedSteps], INavigation1, pixelTolerance: 50))
                    {
                        bflag22 = true;
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                }
                if (bflag22 == false)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 23 Select the Scroll tool from the 3D tool box.
                bool btool23 = z3dvp.select3DTools(Z3DTools.Scrolling_Tool, BluRingZ3DViewerPage.MPRPathNavigation);
                if (btool23)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //step 25 Make sure that the Flip check box is unchecked.
                bool bflipflop = z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.MPRPathNavigation, "uncheck", BluRingZ3DViewerPage.Flip);
                if (bflipflop)
                {

                result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 24   Click the left mouse button on the image displayed in MPR/3D path navigation control and drag the mouse downwards.
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((IMprPath.Location.X + 100), (IMprPath.Location.Y + 150));
                Thread.Sleep(1000);
                for (int i = 0; i < 20; i++)
                {
                    BasePage.mouse_event(0x0800, 0, 0, -15, 0);
                    Thread.Sleep(1000);
                }
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], INavigation1, pixelTolerance: 30))
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
                //Step 26 Select the Reset button from the 3D tool box.
                z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.MPRPathNavigation);
                Thread.Sleep(10000);
                List<string> result26 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result26[0] == result26[1] && result26[2] == result26[3] && result26[4] == result26[5] && slocationvalue == (result26[4]))
                {
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
            }
        }

        public TestCaseResult Test_163324(String testid, String teststeps, int stepcount)
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
                //step:2 - Search and load the 3D supported study in the universal viewer
                //step:3 - Select the Curved MPR option from the smart view drop down
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step3 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription, BluRingZ3DViewerPage.CurvedMPR, field: "acc", thumbimgoptional: Descr2);
                if (step3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:4 - Select the 3D settings option and move the MPR interactive quality sliders to 100%
                Boolean step4 = Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.MPRInteractiveQuality, 100);
                if(step4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:5  -Create a path by adding the points in the navigation controls
                String BeforeImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_Before.jpg";
                String AfterImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_After.jpg";
                IWebElement ViewerContainer = Z3dViewerPage.ViewerContainer();
                DownloadImageFile(ViewerContainer, BeforeImagePath);
                IWebElement Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 40, 30);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 40, 60);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveClickAndHold(Navigation1, Navigation1.Size.Width / 2 + 40, 60);
                PageLoadWait.WaitForFrameLoad(5);
                bool checkerror = Z3dViewerPage.checkerrormsg();
                if (checkerror)
                    throw new Exception("Failed to find path");
                IWebElement Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(Navigation2).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation2, Navigation2.Size.Width / 2, 90);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                DownloadImageFile(ViewerContainer, AfterImagePath);
                if (!CompareImage(BeforeImagePath, AfterImagePath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:6 - Select the Rotate tool from the 3D tool box.
                Boolean step6 = Z3dViewerPage.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.MPRPathNavigation);
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

                //step:7 - Click and hold the left mouse button on the image displayed on the Curved MPR control and do a free rotation.
                Boolean step7 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.CurvedMPR, 5, 5, 10);
                if(step7)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:8  -Select the 3D settings option and move the MPR interactive quality sliders lesser 100%. 
                Boolean step8 = Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.MPRInteractiveQuality, 95);
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

                //step:9 - Repeat steps 5- 7.
                Boolean step9_1 = Z3dViewerPage.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.MPRPathNavigation);
                Boolean step9_2 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.CurvedMPR, 5, 5, 10);
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

                //step:10 - Universal viewer is closed and study search page is displayed.
                Z3dViewerPage.ExitIcon().Click();
                PageLoadWait.WaitForFrameLoad(5);
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

                //step:11 - From the Universal viewer , Select a 3D supported No Lossy compressed series and Select the Curved MPR option from the smart view drop down.
                Boolean step11 = Z3dViewerPage.searchandopenstudyin3D(Study2PID, Study2Descr, BluRingZ3DViewerPage.CurvedMPR);
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

                //step:12 - Select the 3D settings option and move the MPR interactive quality sliders to 100%
                Boolean step12 = Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.MPRInteractiveQuality, 100);
                if (step12)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:13  -Create a path by adding the points in the navigation controls
                BeforeImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_Before.jpg";
                AfterImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_After.jpg";
                ViewerContainer = Z3dViewerPage.ViewerContainer();
                DownloadImageFile(ViewerContainer, BeforeImagePath);
                Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 40, 30);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 40, 60);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveClickAndHold(Navigation1, Navigation1.Size.Width / 2 + 40, 60);
                PageLoadWait.WaitForFrameLoad(5);
                bool step13 = Z3dViewerPage.checkerrormsg();
                if (step13)
                    throw new Exception("Failed to find path in step 13");
                Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(Navigation2).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation2, Navigation2.Size.Width / 2, 90);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                DownloadImageFile(ViewerContainer, AfterImagePath);
                if (!CompareImage(BeforeImagePath, AfterImagePath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:14 - Select the Rotate tool from the 3D tool box.
                Boolean step14 = Z3dViewerPage.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.MPRPathNavigation);
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

                //step:15 - Click and hold the left mouse button on the image displayed on the Curved MPR control and do a free rotation.
                Boolean step15 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.CurvedMPR, 5, 5, 10);
                if (!step15)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:16  -Select the 3D settings option and move the MPR interactive quality sliders lesser 100%. 
                Boolean step16 = Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.MPRInteractiveQuality, 95);
                if (step16)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:17 - Repeat steps 5- 7.
                Boolean step17_1 = Z3dViewerPage.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.MPRPathNavigation);
                Boolean step17_2 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.CurvedMPR, 5, 5, 10);
                if (step17_1 && step17_2)
                {
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

        public TestCaseResult Test_163325(String testid, String teststeps, int stepcount)
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
                //Step:1  - Log in iCA and Navigate to studies tab
                //step:2 - Search and load the 3D supported study in the universal viewer
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step2 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription, BluRingZ3DViewerPage.CurvedMPR);
                if (step2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:3 - Draw path with more than 3 control points. Ensure that the current path position (prism) is at the last control point.
                new Actions(Driver).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage.Navigationone, AortaLoc,ScrollDirection:"up",scrolllevel:34,Thickness:"y");
                IWebElement Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                IWebElement Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                IWebElement CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                int step3_1 = Z3dViewerPage.LevelOfSelectedColor(Navigation1, testid + new Random(), ExecutedSteps + 1, 0, 0, 255, 2);
                int step3_2 = Z3dViewerPage.LevelOfSelectedColor(Navigation2, testid + new Random(), ExecutedSteps + 1, 0, 0, 255, 2);
                int step3_3 = Z3dViewerPage.LevelOfSelectedColor(CurvedMPR, testid + new Random(), ExecutedSteps + 1, 0, 0, 255, 2);
                //new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2 + 20, 50).Click().Build().Perform();
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 50);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 70);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 90);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 110);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                int step3_4 = Z3dViewerPage.LevelOfSelectedColor(Navigation1, testid + new Random(), ExecutedSteps + 1, 0, 0, 255, 2);
                int step3_5 = Z3dViewerPage.LevelOfSelectedColor(Navigation2, testid + new Random(), ExecutedSteps + 1, 0, 0, 255, 2);
                int step3_6 = Z3dViewerPage.LevelOfSelectedColor(CurvedMPR, testid + new Random(), ExecutedSteps + 1, 0, 0, 255, 2);
                if(step3_1 < step3_4 && step3_2 < step3_5 && step3_3 != step3_6)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:4 - Delete the last control point. 
                String BeforeImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_Before.jpg";
                String AfterImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_After.jpg";
                CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                DownloadImageFile(CurvedMPR, BeforeImagePath);
                Z3dViewerPage.CurvedPathdeletor(Navigation1, Navigation1.Size.Width / 2 + 20, 110, "Delete Control Point");
                DownloadImageFile(CurvedMPR, AfterImagePath);
                if (!CompareImage(BeforeImagePath, AfterImagePath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:5  -Redraw the last control point. Position of the Prism and Localizer line matches the last control point.
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 110);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Navigation1))
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

                //step:6  -Delete the last control point
                BeforeImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_Before.jpg";
                AfterImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_After.jpg";
                CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                DownloadImageFile(CurvedMPR, BeforeImagePath);
                Z3dViewerPage.CurvedPathdeletor(Navigation1, Navigation1.Size.Width / 2 + 20, 110, "Delete Control Point");
                DownloadImageFile(CurvedMPR, AfterImagePath);
                if (!CompareImage(BeforeImagePath, AfterImagePath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:7 - Redraw a third control point
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 110);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Navigation1))
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

                //step:8 - Scroll through the path so that the current path position (prism) is at the first control point.
                Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage.MPRPathNavigation, ScrollDirection: "down", scrolllevel: 50);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Navigation1))
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

                //step:9 - Delete the first control point.
                BeforeImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_Before.jpg";
                AfterImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_After.jpg";
                CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                DownloadImageFile(CurvedMPR, BeforeImagePath);
                Z3dViewerPage.CurvedPathdeletor(Navigation1, Navigation1.Size.Width / 2 + 20, 50, "Delete Control Point");
                DownloadImageFile(CurvedMPR, AfterImagePath);
                if (!CompareImage(BeforeImagePath, AfterImagePath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:10 - Redraw a third control point 
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 50);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Navigation1))
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

                //step:11 - Delete the first control point
                BeforeImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_Before.jpg";
                AfterImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_After.jpg";
                CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                DownloadImageFile(CurvedMPR, BeforeImagePath);
                Z3dViewerPage.CurvedPathdeletor(Navigation1, Navigation1.Size.Width / 2 + 20, 50, "Delete Control Point");
                DownloadImageFile(CurvedMPR, AfterImagePath);
                if (!CompareImage(BeforeImagePath, AfterImagePath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:12 - Select the Reset button from the 3D tool box
                Z3dViewerPage.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.MPRPathNavigation);
                PageLoadWait.WaitForFrameLoad(10);
                List<string> step12 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (step12[0] == ResetLoc && step12[1] == ResetLoc && step12[2] == ResetLoc && step12[3] == ResetLoc && step12[4] == ResetLoc && step12[5] == ResetLoc)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:13 - Draw the path so that it has 4 or more control points
                Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage.Navigationone, AortaLoc, scrolllevel: 34);
                Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 60);
                //new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2 + 20, 60).Click().Build().Perform();
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 80);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 100);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 120);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 140);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Navigation1))
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

                //step:14 - Delete the first control point
                BeforeImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_Before.jpg";
                AfterImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_After.jpg";
                CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                DownloadImageFile(CurvedMPR, BeforeImagePath);
                Z3dViewerPage.CurvedPathdeletor(Navigation1, Navigation1.Size.Width / 2 + 20, 60, "Delete Control Point");
                DownloadImageFile(CurvedMPR, AfterImagePath);
                if (!CompareImage(BeforeImagePath, AfterImagePath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:15 - Repeat step 13 but this time delete the last control point
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 60);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                BeforeImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_Before.jpg";
                AfterImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_After.jpg";
                CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                DownloadImageFile(CurvedMPR, BeforeImagePath);
                Z3dViewerPage.CurvedPathdeletor(Navigation1, Navigation1.Size.Width / 2 + 20, 140, "Delete Control Point");
                DownloadImageFile(CurvedMPR, AfterImagePath);
                if (!CompareImage(BeforeImagePath, AfterImagePath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:16 - Draw a manual path with more than 4 points
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage.Navigationone, AortaLoc, ScrollDirection: "up", scrolllevel: 34, Thickness: "y");
                Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                int step16_1 = Z3dViewerPage.LevelOfSelectedColor(Navigation1, testid + new Random(), ExecutedSteps + 1, 0, 0, 255, 2);
                int step16_2 = Z3dViewerPage.LevelOfSelectedColor(Navigation2, testid + new Random(), ExecutedSteps + 1, 0, 0, 255, 2);
                int step16_3 = Z3dViewerPage.LevelOfSelectedColor(CurvedMPR, testid + new Random(), ExecutedSteps + 1, 0, 0, 255, 2);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 60);
                //new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2 + 20, 60).Build().Perform();
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 75);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 90);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 105);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 120);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 145);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                int step16_4 = Z3dViewerPage.LevelOfSelectedColor(Navigation1, testid + new Random(), ExecutedSteps + 1, 0, 0, 255, 2);
                int step16_5 = Z3dViewerPage.LevelOfSelectedColor(Navigation2, testid + new Random(), ExecutedSteps + 1, 0, 0, 255, 2);
                int step16_6 = Z3dViewerPage.LevelOfSelectedColor(CurvedMPR, testid + new Random(), ExecutedSteps + 1, 0, 0, 255, 2);
                if (step16_1 < step16_4 && step16_2 < step16_5 && step16_2 != step16_6)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:17  - 	Scroll up through the path until the prism is located between the top most (first) point and the second top most point.
                Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage.MPRPathNavigation, ScrollDirection: "down", scrolllevel: 100);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Navigation1))
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

                //step:18 -  	Delete the top most point and verify that the prism moves to the top most point
                BeforeImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_Before.jpg";
                AfterImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_After.jpg";
                CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                DownloadImageFile(CurvedMPR, BeforeImagePath);
                Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool);
                PageLoadWait.WaitForFrameLoad(5);
                if (browserName.ToLower().Contains("mozilla") || browserName.ToLower().Contains("firefox"))
                    Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 60);
                Z3dViewerPage.CurvedPathdeletor(Navigation1, Navigation1.Size.Width / 2 + 20, 60, "Delete Control Point");
                DownloadImageFile(CurvedMPR, AfterImagePath);
                if (!CompareImage(BeforeImagePath, AfterImagePath))
                {
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

        public TestCaseResult Test_168891(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string ResetLoc = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1  - Log in iCA and Navigate to studies tab
                //step:2 - Search and load the 3D supported study in the universal viewer
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step2 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription, BluRingZ3DViewerPage.CurvedMPR);
                if (step2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    ////Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:3 - Draw path with more than 3 control points. Ensure that the current path position (prism) is at the last control point.
                Z3dViewerPage.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage.Navigationone, ScrollDirection: "up", scrolllevel: 34, Thickness: "y");
                IWebElement Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                IWebElement Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                IWebElement CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                int step3_1 = Z3dViewerPage.LevelOfSelectedColor(Navigation1, testid + new Random(), ExecutedSteps + 1, 0, 0, 255, 2);
                int step3_2 = Z3dViewerPage.LevelOfSelectedColor(Navigation2, testid + new Random(), ExecutedSteps + 1, 0, 0, 255, 2);
                int step3_3 = Z3dViewerPage.LevelOfSelectedColor(CurvedMPR, testid + new Random(), ExecutedSteps + 1, 0, 0, 255, 2);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 30);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 50);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 70);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 90);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                int step3_4 = Z3dViewerPage.LevelOfSelectedColor(Navigation1, testid + new Random(), ExecutedSteps + 1, 0, 0, 255, 2);
                int step3_5 = Z3dViewerPage.LevelOfSelectedColor(Navigation2, testid + new Random(), ExecutedSteps + 1, 0, 0, 255, 2);
                int step3_6 = Z3dViewerPage.LevelOfSelectedColor(CurvedMPR, testid + new Random(), ExecutedSteps + 1, 0, 0, 255, 2);
                if (step3_1 < step3_4 && step3_2 < step3_5 && step3_3 != step3_6)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:4 - Delete the last control point. 
                String BeforeImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_Before.jpg";
                String AfterImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_After.jpg";
                CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                DownloadImageFile(CurvedMPR, BeforeImagePath);
                PageLoadWait.WaitForPageLoad(5);
                Z3dViewerPage.CurvedPathdeletor(Navigation1, Navigation1.Size.Width / 2 + 20, 90, "Delete Control Point", PostTool: Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                DownloadImageFile(CurvedMPR, AfterImagePath);
                if (!CompareImage(BeforeImagePath, AfterImagePath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:5  -Redraw the last control point. Position of the Prism and Localizer line matches the last control point.
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 90);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Navigation1))
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

                //step:6  -Delete the last control point
                BeforeImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_Before.jpg";
                AfterImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_After.jpg";
                CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                DownloadImageFile(CurvedMPR, BeforeImagePath);
                Z3dViewerPage.CurvedPathdeletor(Navigation1, Navigation1.Size.Width / 2 + 20, 90, "Delete Control Point", PostTool : Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                DownloadImageFile(CurvedMPR, AfterImagePath);
                if (!CompareImage(BeforeImagePath, AfterImagePath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:7 - Select the Reset button from the 3D tool box
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                List<string> step12 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (step12[0] == ResetLoc && step12[1] == ResetLoc && step12[2] == ResetLoc && step12[3] == ResetLoc && step12[4] == ResetLoc && step12[5] == ResetLoc)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:8 - Draw the path so that it has 4 or more control points
                Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage.Navigationone, scrolllevel: 34);
                Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                //Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 20);
                new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2 + 20, 20).Build().Perform();
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                //Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 40);
                new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2 + 20, 40).Build().Perform();
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 60);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 80);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 100);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Navigation1))
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

                //step:9 - Delete the last control point. 
                BeforeImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_Before.jpg";
                AfterImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_After.jpg";
                CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                DownloadImageFile(CurvedMPR, BeforeImagePath);
                PageLoadWait.WaitForPageLoad(5);
                Z3dViewerPage.CurvedPathdeletor(Navigation1, Navigation1.Size.Width / 2 + 20, 100, "Delete Control Point", PostTool: Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                DownloadImageFile(CurvedMPR, AfterImagePath);
                if (!CompareImage(BeforeImagePath, AfterImagePath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:10  -Redraw the last control point. Position of the Prism and Localizer line matches the last control point.
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 20, 100);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Navigation1))
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

                //step:11 - Select the Reset button from the 3D tool box
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                List<string> step11 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (step11[0] == ResetLoc && step11[1] == ResetLoc && step11[2] == ResetLoc && step11[3] == ResetLoc && step11[4] == ResetLoc && step11[5] == ResetLoc)
                {
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

        public TestCaseResult Test_168892(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string ResetLoc = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1  - Log in iCA and Navigate to studies tab
                //step:2 - Search and load the 3D supported study in the universal viewer
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step2 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription, BluRingZ3DViewerPage.CurvedMPR);
                if (step2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:3 - Draw path with more than 3 control points. Ensure that the current path position (prism) is at the last control point.
                Z3dViewerPage.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage.Navigationone, ScrollDirection: "up", scrolllevel: 60, Thickness: "y");
                IWebElement Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                IWebElement Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                IWebElement CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                int step3_1 = Z3dViewerPage.LevelOfSelectedColor(Navigation1, testid + new Random(), ExecutedSteps + 1, 0, 0, 255, 2);
                int step3_2 = Z3dViewerPage.LevelOfSelectedColor(Navigation2, testid + new Random(), ExecutedSteps + 1, 0, 0, 255, 2);
                int step3_3 = Z3dViewerPage.LevelOfSelectedColor(CurvedMPR, testid + new Random(), ExecutedSteps + 1, 0, 0, 255, 2);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 5, ((Navigation1.Size.Height / 4) * 3));
                //new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2 + 5, ((Navigation1.Size.Height / 4) * 3)).Build().Perform();
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 5, ((Navigation1.Size.Height / 4) * 3) + 15);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 5, ((Navigation1.Size.Height / 4) * 3) + 30);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2, ((Navigation1.Size.Height / 4) * 3) + 45);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                int step3_4 = Z3dViewerPage.LevelOfSelectedColor(Navigation1, testid + new Random(), ExecutedSteps + 1, 0, 0, 255, 2);
                int step3_5 = Z3dViewerPage.LevelOfSelectedColor(Navigation2, testid + new Random(), ExecutedSteps + 1, 0, 0, 255, 2);
                int step3_6 = Z3dViewerPage.LevelOfSelectedColor(CurvedMPR, testid + new Random(), ExecutedSteps + 1, 0, 0, 255, 2);
                if (step3_1 < step3_4 && step3_2 < step3_5 && step3_3 != step3_6)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:4 - Delete the last control point. 
                String BeforeImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_Before.jpg";
                String AfterImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_After.jpg";
                CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                DownloadImageFile(CurvedMPR, BeforeImagePath);
                PageLoadWait.WaitForPageLoad(5);
                Z3dViewerPage.CurvedPathdeletor(Navigation1, Navigation1.Size.Width / 2, ((Navigation1.Size.Height / 4) * 3) + 45, "Delete Control Point", PostTool: Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                DownloadImageFile(CurvedMPR, AfterImagePath);
                if (!CompareImage(BeforeImagePath, AfterImagePath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:5  -Redraw the last control point. Position of the Prism and Localizer line matches the last control point.
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2, ((Navigation1.Size.Height / 4) * 3) + 45);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage.MPRPathNavigation, ScrollDirection: "down", scrolllevel: 10);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], CurvedMPR))
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

                //step:6  -Delete the last control point
                BeforeImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_Before.jpg";
                AfterImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_After.jpg";
                CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                DownloadImageFile(CurvedMPR, BeforeImagePath);
                Z3dViewerPage.CurvedPathdeletor(Navigation1, Navigation1.Size.Width / 2, ((Navigation1.Size.Height / 4) * 3) + 45, "Delete Control Point", PostTool: Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                DownloadImageFile(CurvedMPR, AfterImagePath);
                if (!CompareImage(BeforeImagePath, AfterImagePath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:7 - Select the Reset button from the 3D tool box
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                List<string> step12 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (step12[0] == ResetLoc && step12[1] == ResetLoc && step12[2] == ResetLoc && step12[3] == ResetLoc && step12[4] == ResetLoc && step12[5] == ResetLoc)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:8 - Draw the path so that it has 4 or more control points
                Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage.Navigationone, scrolllevel: 60);
                Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 5, ((Navigation1.Size.Height / 4) * 3));
                //new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2 + 5, ((Navigation1.Size.Height / 4) * 3)).Click().Build().Perform();
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 5, ((Navigation1.Size.Height / 4) * 3) + 10);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 5, ((Navigation1.Size.Height / 4) * 3) + 20);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2, ((Navigation1.Size.Height / 4) * 3) + 30);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2, ((Navigation1.Size.Height / 4) * 3) + 40);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Navigation1))
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

                //step:9 - Delete the last control point. 
                BeforeImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_Before.jpg";
                AfterImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_After.jpg";
                CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                DownloadImageFile(CurvedMPR, BeforeImagePath);
                PageLoadWait.WaitForPageLoad(5);
                Z3dViewerPage.CurvedPathdeletor(Navigation1, Navigation1.Size.Width / 2, ((Navigation1.Size.Height / 4) * 3) + 40, "Delete Control Point", PostTool: Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                DownloadImageFile(CurvedMPR, AfterImagePath);
                if (!CompareImage(BeforeImagePath, AfterImagePath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:10  -Redraw the last control point. Position of the Prism and Localizer line matches the last control point.
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2, ((Navigation1.Size.Height / 4) * 3) + 40);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Navigation1))
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

                //step:11 - Select the Reset button from the 3D tool box
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                List<string> step11 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (step11[0] == ResetLoc && step11[1] == ResetLoc && step11[2] == ResetLoc && step11[3] == ResetLoc && step11[4] == ResetLoc && step11[5] == ResetLoc)
                {
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

        public TestCaseResult Test_168909(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result;
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
                String MPRPathLocValBefore, ThreeDPathLocValBefore, MPRPathLocValAfter, ThreeDPathLocValAfter;
                String BaseImages = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + "TempImages" + Path.DirectorySeparatorChar + "BaseImages" + Path.DirectorySeparatorChar + Config.BrowserType;
                Directory.CreateDirectory(BaseImages);
                String ColorSplitImages = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + "TempImages" + Path.DirectorySeparatorChar + "ColorSplitted" + Path.DirectorySeparatorChar + Config.BrowserType;
                Directory.CreateDirectory(ColorSplitImages);
                IWebElement Navigation1, Navigation2, CurvedMPRNavigation, Navigation3;
                String objloc = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
                String location1 = objloc.Split('|')[0];
                String ResetLoc = objloc.Split('|')[1];
                String location2 = objloc.Split('|')[2];

                //step 01 , 02 & 03
                login.LoginIConnect(adminUserName, adminPassword);
                bool res = brz3dvp.searchandopenstudyin3D(objpatid, objthumbimg, layout: BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    throw new Exception("unable to launch study in 3D Viewer");
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }

                //Step 04 
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                new Actions(Driver).SendKeys("x").Build().Perform();
                res = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                bool isscrolled = brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, scrollTill: location1);
                new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) - 15, (Navigation1.Size.Height / 4) - 10).Click().Build().Perform();
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) - 15, (Navigation1.Size.Height / 4) + 10).Click().Build().Perform();
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(10);
                Boolean checkerr = brz3dvp.checkerrormsg();
                if (checkerr)
                    throw new Exception("Failed to find path Step 4" );
                PageLoadWait.WaitForFrameLoad(10);
                if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation) && res && isscrolled)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 05
                res = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.CurvedMPR);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                new Actions(Driver).MoveToElement(CurvedMPRNavigation, CurvedMPRNavigation.Size.Width / 2, CurvedMPRNavigation.Size.Height / 4).ClickAndHold()
                    .MoveToElement(CurvedMPRNavigation, CurvedMPRNavigation.Size.Width / 2, (CurvedMPRNavigation.Size.Height / 4) * 3).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                MPRPathLocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                ThreeDPathLocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation) && res)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 06
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.EnableOneViewupMode(CurvedMPRNavigation);
                PageLoadWait.WaitForFrameLoad(10);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 07
                brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.CurvedMPR);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                bool res31 = brz3dvp.DisableOneUpViewMode(CurvedMPRNavigation);
                Logger.Instance.InfoLog("The resultant of resizing the viewport is : " + res31.ToString());
                PageLoadWait.WaitForFrameLoad(10);
                MPRPathLocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                ThreeDPathLocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                List<string> ResetLayout = brz3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (ResetLayout[0] == ResetLoc && ResetLayout[1] == ResetLoc && ResetLayout[2] == ResetLoc && ResetLayout[3] == ResetLoc && ResetLayout[4] == ResetLoc && ResetLayout[5] == ResetLoc)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 08
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    PageLoadWait.WaitForFrameLoad(10);
                    Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                    isscrolled = brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, scrollTill: location2 , ScrollDirection: "down");
                    new Actions(Driver).MoveToElement(Navigation1, 3 * (Navigation1.Size.Width / 4), Navigation1.Size.Height / 2).Click().Build().Perform();
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(10000);
                    new Actions(Driver).MoveToElement(Navigation1, 3 * (Navigation1.Size.Width / 4) + 20, Navigation1.Size.Height / 2).Click().Build().Perform();
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(10000);
                    checkerr = brz3dvp.checkerrormsg();
                    if (checkerr)
                        throw new Exception("Failed to find path Step 8");
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation) && isscrolled)
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 09
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    new Actions(Driver).MoveToElement(CurvedMPRNavigation, CurvedMPRNavigation.Size.Width / 2, CurvedMPRNavigation.Size.Height / 4).ClickAndHold()
                        .MoveToElement(CurvedMPRNavigation, CurvedMPRNavigation.Size.Width / 2, (CurvedMPRNavigation.Size.Height / 4) * 3).Release().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    MPRPathLocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                    ThreeDPathLocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                    {
                        result.steps[ExecutedSteps].StepPass();
                    }
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //Step 10 
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.EnableOneViewupMode(CurvedMPRNavigation);
                PageLoadWait.WaitForFrameLoad(10);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 11
                brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.CurvedMPR);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                bool res34 = brz3dvp.DisableOneUpViewMode(CurvedMPRNavigation);
                Logger.Instance.InfoLog("The resultant of resizing the viewport is : " + res34.ToString());
                PageLoadWait.WaitForFrameLoad(10);
                MPRPathLocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                ThreeDPathLocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                ResetLayout = brz3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (ResetLayout[0] == ResetLoc && ResetLayout[1] == ResetLoc && ResetLayout[2] == ResetLoc && ResetLayout[3] == ResetLoc && ResetLayout[4] == ResetLoc && ResetLayout[5] == ResetLoc)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 12
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                    new Actions(Driver).MoveToElement(Navigation1).SendKeys("x").Build().Perform();
                    Thread.Sleep(3000);
                    Accord.Point redposition = brz3dvp.GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 121, "red");
                    Thread.Sleep(5000);
                    Accord.Point yellowposition = brz3dvp.GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 122, "yellow", "vertical");
                    Thread.Sleep(5000);
                    if (browserName.Contains("chrome") || browserName.Contains("firefox"))
                        new Actions(Driver).MoveToElement(Navigation2, (Int32)redposition.X, (Int32)redposition.Y).ClickAndHold()
                            .MoveToElement(Navigation2, (Int32)yellowposition.X, (Int32)yellowposition.Y).Release().Build().Perform();
                    else
                        new TestCompleteAction().PerformDraganddrop(Navigation2, (Int32)redposition.X, (Int32)redposition.Y, (Int32)yellowposition.X, (Int32)yellowposition.Y);
                    Thread.Sleep(5000);
                    new Actions(Driver).MoveToElement(Navigation1).SendKeys("x").Build().Perform();
                    Thread.Sleep(3000);
                    Navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                    brz3dvp.MoveAndClick(Navigation3, (Navigation3.Size.Width / 4) - 10, (Navigation3.Size.Height / 4) - 10);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(10000);
                    brz3dvp.MoveAndClick(Navigation3, (Navigation3.Size.Width / 4) - 10, (Navigation3.Size.Height / 4) + 10);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(10000);
                    bool check12 = brz3dvp.checkerrormsg();
                    if (check12)
                        throw new Exception("Failed to find path in step 12");
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 13
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    new Actions(Driver).MoveToElement(CurvedMPRNavigation, CurvedMPRNavigation.Size.Width / 2, CurvedMPRNavigation.Size.Height / 4).ClickAndHold()
                        .MoveToElement(CurvedMPRNavigation, CurvedMPRNavigation.Size.Width / 2, (CurvedMPRNavigation.Size.Height / 4) * 3).Release().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    MPRPathLocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                    ThreeDPathLocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                    {
                        result.steps[ExecutedSteps].StepPass();
                    }
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //Step 14
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.EnableOneViewupMode(CurvedMPRNavigation);
                PageLoadWait.WaitForFrameLoad(10);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 15
                brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.CurvedMPR);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                bool res40 = brz3dvp.DisableOneUpViewMode(CurvedMPRNavigation);
                Logger.Instance.InfoLog("The resultant of resizing the viewport is : " + res40.ToString());
                PageLoadWait.WaitForFrameLoad(10);
                MPRPathLocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                ThreeDPathLocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                if (res40 && ThreeDPathLocValAfter != ThreeDPathLocValBefore && MPRPathLocValAfter != MPRPathLocValBefore)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 16
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                    new Actions(Driver).MoveToElement(Navigation1).SendKeys("x").Build().Perform();
                    Thread.Sleep(3000);
                    Accord.Point redposition = brz3dvp.GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 161, "red");
                    Thread.Sleep(5000);
                    Accord.Point yellowposition = brz3dvp.GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 162, "yellow", "vertical");
                    Thread.Sleep(5000);
                    if (browserName.Contains("chrome") || browserName.Contains("firefox"))
                        new Actions(Driver).MoveToElement(Navigation2, (Int32)redposition.X, (Int32)redposition.Y).ClickAndHold()
                            .MoveToElement(Navigation2, (Int32)yellowposition.X, (Int32)yellowposition.Y).Release().Build().Perform();
                    else
                        new TestCompleteAction().PerformDraganddrop(Navigation2, (Int32)redposition.X, (Int32)redposition.Y, (Int32)yellowposition.X, (Int32)yellowposition.Y);
                    Thread.Sleep(5000);
                    new Actions(Driver).MoveToElement(Navigation1).SendKeys("x").Build().Perform();
                    Thread.Sleep(3000);
                    Navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                    brz3dvp.MoveAndClick(Navigation3, (Navigation3.Size.Width / 4) - 10, (Navigation3.Size.Height / 4) - 10);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(10000);
                    brz3dvp.MoveAndClick(Navigation3, (Navigation3.Size.Width / 4) + 10, (Navigation3.Size.Height / 4) - 10);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(10000);
                    bool check12 = brz3dvp.checkerrormsg();
                    if (check12)
                        throw new Exception("Failed to find path in step 12");
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 17
                res = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.CurvedMPR);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                new Actions(Driver).MoveToElement(CurvedMPRNavigation, CurvedMPRNavigation.Size.Width / 4, CurvedMPRNavigation.Size.Height / 2).ClickAndHold()
                    .MoveToElement(CurvedMPRNavigation, (CurvedMPRNavigation.Size.Width / 4) * 3, CurvedMPRNavigation.Size.Height / 2).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                MPRPathLocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                ThreeDPathLocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation)&&res)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 18
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.EnableOneViewupMode(CurvedMPRNavigation);
                PageLoadWait.WaitForFrameLoad(10);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 19
                brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.CurvedMPR);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                bool res45 = brz3dvp.DisableOneUpViewMode(CurvedMPRNavigation);
                Logger.Instance.InfoLog("The resultant of resizing the viewport is : " + res45.ToString());
                PageLoadWait.WaitForFrameLoad(10);
                MPRPathLocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                ThreeDPathLocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                if (res45 && ThreeDPathLocValAfter != ThreeDPathLocValBefore && MPRPathLocValAfter != MPRPathLocValBefore)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 20
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                    brz3dvp.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 30, (Navigation2.Size.Height / 4) - 10);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(3000);
                    brz3dvp.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 20, (Navigation2.Size.Height / 4) + 10);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(3000);
                    bool check20 = brz3dvp.checkerrormsg();
                    if (check20)
                        throw new Exception("Failed to find path in step 20");
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 21
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    new Actions(Driver).MoveToElement(CurvedMPRNavigation, CurvedMPRNavigation.Size.Width / 4, CurvedMPRNavigation.Size.Height / 2).ClickAndHold()
                        .MoveToElement(CurvedMPRNavigation, (CurvedMPRNavigation.Size.Width / 4) * 3, CurvedMPRNavigation.Size.Height / 2).Release().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    MPRPathLocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                    ThreeDPathLocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                    {
                        result.steps[ExecutedSteps].StepPass();
                    }
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //Step 22
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.EnableOneViewupMode(CurvedMPRNavigation);
                PageLoadWait.WaitForFrameLoad(10);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 23
                brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.CurvedMPR);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                bool res48 = brz3dvp.DisableOneUpViewMode(CurvedMPRNavigation);
                Logger.Instance.InfoLog("The resultant of resizing the viewport is : " + res48.ToString());
                PageLoadWait.WaitForFrameLoad(10);
                MPRPathLocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                ThreeDPathLocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                ResetLayout = brz3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (ResetLayout[0] == ResetLoc && ResetLayout[1] == ResetLoc && ResetLayout[2] == ResetLoc && ResetLayout[3] == ResetLoc && ResetLayout[4] == ResetLoc && ResetLayout[5] == ResetLoc)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 24
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    Navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                    Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                    new Actions(Driver).MoveToElement(Navigation2).SendKeys("x").Build().Perform();
                    Thread.Sleep(5000);
                    Accord.Point yellowpoint = brz3dvp.GetIntersectionPoints(Navigation3, testid, ExecutedSteps + 241, color: "yellow");
                    PageLoadWait.WaitForFrameLoad(10);
                    Thread.Sleep(5000);
                    Accord.Point bluepoint = brz3dvp.GetIntersectionPoints(Navigation3, testid, ExecutedSteps + 242, color: "blue", displacement: "Vertical");
                    PageLoadWait.WaitForFrameLoad(10);
                    Thread.Sleep(5000);
                    brz3dvp.Performdragdrop(Navigation3, (Int32)bluepoint.X, (Int32)bluepoint.Y, (Int32)yellowpoint.X, (Int32)yellowpoint.Y);
                    Thread.Sleep(10000);
                    new Actions(Driver).MoveToElement(Navigation2).SendKeys("x").Build().Perform();
                    Thread.Sleep(5000);
                    brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationtwo, scrolllevel: 8);
                    brz3dvp.MoveAndClick(Navigation2, (Navigation2.Size.Width / 4) - 20, (Navigation2.Size.Height / 2));
                    Thread.Sleep(3000);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(3000);
                    brz3dvp.MoveAndClick(Navigation2, (Navigation2.Size.Width / 4) - 30, (Navigation2.Size.Height / 2));
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(3000);
                    PageLoadWait.WaitForPageLoad(5);
                    Boolean checkissue = brz3dvp.checkerrormsg();
                    if (checkissue)
                        throw new Exception("Failed to find path Step 24");
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 25
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    new Actions(Driver).MoveToElement(CurvedMPRNavigation, CurvedMPRNavigation.Size.Width / 4, CurvedMPRNavigation.Size.Height / 2).ClickAndHold()
                        .MoveToElement(CurvedMPRNavigation, (CurvedMPRNavigation.Size.Width / 4) * 3, CurvedMPRNavigation.Size.Height / 2).Release().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    MPRPathLocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                    ThreeDPathLocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                    {
                        result.steps[ExecutedSteps].StepPass();
                    }
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //Step 26 
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.EnableOneViewupMode(CurvedMPRNavigation);
                PageLoadWait.WaitForFrameLoad(10);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 27
                brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.CurvedMPR);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                bool res51 = brz3dvp.DisableOneUpViewMode(CurvedMPRNavigation);
                Logger.Instance.InfoLog("The resultant of resizing the viewport is : " + res51.ToString());
                PageLoadWait.WaitForFrameLoad(10);
                MPRPathLocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                ThreeDPathLocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                brz3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (ResetLayout[0] == ResetLoc && ResetLayout[1] == ResetLoc && ResetLayout[2] == ResetLoc && ResetLayout[3] == ResetLoc && ResetLayout[4] == ResetLoc && ResetLayout[5] == ResetLoc)
                {
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

        public TestCaseResult Test_168910(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result;
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

                String BaseImages = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + "TempImages" + Path.DirectorySeparatorChar + "BaseImages" + Path.DirectorySeparatorChar + Config.BrowserType;
                Directory.CreateDirectory(BaseImages);
                String ColorSplitImages = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + "TempImages" + Path.DirectorySeparatorChar + "ColorSplitted" + Path.DirectorySeparatorChar + Config.BrowserType;
                Directory.CreateDirectory(ColorSplitImages);
                IWebElement Navigation1, Navigation2, CurvedMPRNavigation, Navigation3;
                String objloc = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
                String location1 = objloc.Split('|')[0];
                String ResetLoc = objloc.Split('|')[1];
                String location2 = objloc.Split('|')[2];

                //step 01 , 02 & 03
                login.LoginIConnect(adminUserName, adminPassword);
                bool res = brz3dvp.searchandopenstudyin3D(objpatid, objthumbimg, layout: BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    throw new Exception("unable to launch study in 3D Viewer");
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }

                //step 04
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(Navigation1).SendKeys("x").Build().Perform();
                Thread.Sleep(3000);
                new Actions(Driver).MoveToElement(Navigation1).SendKeys("t").Build().Perform();
                Thread.Sleep(3000);
                res = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 5, (3 * (Navigation1.Size.Height / 4)) + 20);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 5, (3 * (Navigation1.Size.Height / 4)) + 30);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(10);
                bool check50 = brz3dvp.checkerrormsg();
                if (check50)
                    throw new Exception("Failed to find path");
                if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation) && res)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 05
                res = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.CurvedMPR);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                new Actions(Driver).MoveToElement(CurvedMPRNavigation, CurvedMPRNavigation.Size.Width / 2, CurvedMPRNavigation.Size.Height / 4).ClickAndHold()
                    .MoveToElement(CurvedMPRNavigation, CurvedMPRNavigation.Size.Width / 2, (CurvedMPRNavigation.Size.Height / 4) * 3).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation) && res)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 06
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.EnableOneViewupMode(CurvedMPRNavigation);
                PageLoadWait.WaitForFrameLoad(10);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 07
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.CurvedMPR);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                bool res57 = brz3dvp.DisableOneUpViewMode(CurvedMPRNavigation);
                Logger.Instance.InfoLog("The resultant of resizing the viewport is : " + res57.ToString());
                PageLoadWait.WaitForFrameLoad(10);
                if (res57 && CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 08
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                    brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 4) + 20, Navigation1.Size.Height / 2);
                    Thread.Sleep(5000);
                    brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 4) + 30, Navigation1.Size.Height / 2);
                    Thread.Sleep(5000);
                    bool check8 = brz3dvp.checkerrormsg();
                    if (check8)
                        throw new Exception("Failed to find path in step 8");
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 09
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    new Actions(Driver).MoveToElement(CurvedMPRNavigation, CurvedMPRNavigation.Size.Width / 2, CurvedMPRNavigation.Size.Height / 4).ClickAndHold()
                        .MoveToElement(CurvedMPRNavigation, CurvedMPRNavigation.Size.Width / 2, (CurvedMPRNavigation.Size.Height / 4) * 3).Release().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                    {
                        result.steps[ExecutedSteps].StepPass();
                    }
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //Step 10 
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.EnableOneViewupMode(CurvedMPRNavigation);
                PageLoadWait.WaitForFrameLoad(10);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 11
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.CurvedMPR);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                bool res11 = brz3dvp.DisableOneUpViewMode(CurvedMPRNavigation);
                Logger.Instance.InfoLog("The resultant of resizing the viewport is : " + res57.ToString());
                PageLoadWait.WaitForFrameLoad(10);
                if (res11 && CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 12
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    Navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                    brz3dvp.MoveAndClick(Navigation3, (Navigation3.Size.Width / 4) + 20, (Navigation3.Size.Height / 2) - 20);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(3000);
                    brz3dvp.MoveAndClick(Navigation3, (Navigation3.Size.Width / 4) + 20, (Navigation3.Size.Height / 2));
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(10000);
                    bool check12 = brz3dvp.checkerrormsg();
                    if (check12)
                        throw new Exception("Failed to find path in step 12");
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 13
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    new Actions(Driver).MoveToElement(CurvedMPRNavigation, CurvedMPRNavigation.Size.Width / 2, CurvedMPRNavigation.Size.Height / 4).ClickAndHold()
                        .MoveToElement(CurvedMPRNavigation, CurvedMPRNavigation.Size.Width / 2, (CurvedMPRNavigation.Size.Height / 4) * 3).Release().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //Step 14 
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.EnableOneViewupMode(CurvedMPRNavigation);
                PageLoadWait.WaitForFrameLoad(10);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 15
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.CurvedMPR);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                bool res15 = brz3dvp.DisableOneUpViewMode(CurvedMPRNavigation);
                Logger.Instance.InfoLog("The resultant of resizing the viewport is : " + res57.ToString());
                PageLoadWait.WaitForFrameLoad(10);
                if (res15 && CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 16
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    Navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    Thread.Sleep(3000);
                    brz3dvp.MoveAndClick(Navigation3, (Navigation3.Size.Width / 4) + 20, Navigation3.Size.Height / 2);
                    Thread.Sleep(5000);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    brz3dvp.MoveAndClick(Navigation3, (Navigation3.Size.Width / 4) + 40, Navigation3.Size.Height / 2);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(3000);
                    PageLoadWait.WaitForFrameLoad(10);
                    bool check16 = brz3dvp.checkerrormsg();
                    if (check16)
                        throw new Exception("Failed to find path in step 16");
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 17
                res = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.CurvedMPR);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                new Actions(Driver).MoveToElement(CurvedMPRNavigation, CurvedMPRNavigation.Size.Width / 4, CurvedMPRNavigation.Size.Height / 2).ClickAndHold()
                    .MoveToElement(CurvedMPRNavigation, (CurvedMPRNavigation.Size.Width / 4) * 3, CurvedMPRNavigation.Size.Height / 2).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation)&&res)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 18
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.EnableOneViewupMode(CurvedMPRNavigation);
                PageLoadWait.WaitForFrameLoad(10);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 19
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.CurvedMPR);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                bool res19 = brz3dvp.DisableOneUpViewMode(CurvedMPRNavigation);
                Logger.Instance.InfoLog("The resultant of resizing the viewport is : " + res57.ToString());
                PageLoadWait.WaitForFrameLoad(10);
                if (res19 && CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 20
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    PageLoadWait.WaitForFrameLoad(10);
                    Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                    PageLoadWait.WaitForFrameLoad(5);
                    Thread.Sleep(5000);
                    brz3dvp.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 20, (3 * (Navigation2.Size.Height / 4)) + 10);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(3000);
                    brz3dvp.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 20, (3 * (Navigation2.Size.Height / 4)) + 20);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(3000);
                    bool check20 = brz3dvp.checkerrormsg();
                    if (check20)
                        throw new Exception("Failed to find path in step 20");
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 21
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                res = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    new Actions(Driver).MoveToElement(CurvedMPRNavigation, CurvedMPRNavigation.Size.Width / 4, CurvedMPRNavigation.Size.Height / 2).ClickAndHold()
                        .MoveToElement(CurvedMPRNavigation, (CurvedMPRNavigation.Size.Width / 4) * 3, CurvedMPRNavigation.Size.Height / 2).Release().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //Step 22
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.EnableOneViewupMode(CurvedMPRNavigation);
                PageLoadWait.WaitForFrameLoad(10);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 23
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.CurvedMPR);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                bool res23 = brz3dvp.DisableOneUpViewMode(CurvedMPRNavigation);
                Logger.Instance.InfoLog("The resultant of resizing the viewport is : " + res57.ToString());
                PageLoadWait.WaitForFrameLoad(10);
                if (res23 && CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 24
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                    Thread.Sleep(3000);
                    brz3dvp.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 20, (3 * (Navigation2.Size.Height / 4)) + 10);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(3000);
                    brz3dvp.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 40, (3 * (Navigation2.Size.Height / 4)) + 10);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(3000);
                    bool check24 = brz3dvp.checkerrormsg();
                    if (check24)
                        throw new Exception("Failed to find path in step 24");
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 25
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    new Actions(Driver).MoveToElement(CurvedMPRNavigation, CurvedMPRNavigation.Size.Width / 4, CurvedMPRNavigation.Size.Height / 2).ClickAndHold()
                        .MoveToElement(CurvedMPRNavigation, (CurvedMPRNavigation.Size.Width / 4) * 3, CurvedMPRNavigation.Size.Height / 2).Release().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                    {
                        result.steps[ExecutedSteps].StepPass();
                    }
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //Step 26
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                res = brz3dvp.EnableOneViewupMode(CurvedMPRNavigation);
                PageLoadWait.WaitForFrameLoad(10);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {                 
                    CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                    if (CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }

                //step 27
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.CurvedMPR);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                bool res27 = brz3dvp.DisableOneUpViewMode(CurvedMPRNavigation);
                Logger.Instance.InfoLog("The resultant of resizing the viewport is : " + res57.ToString());
                PageLoadWait.WaitForFrameLoad(10);
                if (res27 && CompareImage(result.steps[ExecutedSteps], CurvedMPRNavigation))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();


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

    }
}
