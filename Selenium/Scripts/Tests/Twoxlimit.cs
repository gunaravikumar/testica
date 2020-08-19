using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Reusable.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Selenium.Scripts.Tests
{
    class Twoxlimit : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public Imager imager = new Imager();
        DirectoryInfo CurrentDir = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());
        public Twoxlimit(String classname)
        {
            login = new Login();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }

        public TestCaseResult Test_163226(string testid, String teststeps, int stepcount)
        {
           
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string Requirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String[] properties = { BluRingZ3DViewerPage.MPRInteractiveQuality, BluRingZ3DViewerPage.MPRFinalQuality };
            int[] values = { 25, 100 };
            int[] values2 = { 100, 50 };

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                BasePage.MultiDriver.Clear();
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                BasePage.MultiDriver.Add(login.InvokeBrowser("chromenetwork"));
                login.SetDriver(BasePage.MultiDriver[0]);
                login.DriverGoTo(login.url);

                //step 01 & 02
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool Result = brz3dvp.searchandopenstudyin3D(Patientid, ImageCount, ChangeSettings: "No");
                if (Result)
                { 
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed while launching study in 3D Viewer");
                    throw new Exception("Failed to open study in 3D Viewer");
                }

                //step 03 - 05
                Logger.Instance.InfoLog("Will be verified in the upcoming steps");
                result.steps[++ExecutedSteps].StepPass();
                result.steps[++ExecutedSteps].StepPass();
                result.steps[++ExecutedSteps].StepPass();

                //step 06
                Result = brz3dvp.change3dsettings(BluRingZ3DViewerPage.MPRInteractiveQuality, 25);
                if(Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 07
                Result = brz3dvp.change3dsettings(BluRingZ3DViewerPage.MPRFinalQuality, 100);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 08
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)).SendKeys("x").Build().Perform();
                Thread.Sleep(5000);
                bool scrollResult = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationone,Z3DTools.Scrolling_Tool, 50, 50, 100);
                Thread.Sleep(2000);
                bool networkResult = brz3dvp.verifyimagevariations("report1");
                if (scrollResult && networkResult)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    bool result8 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigationone, "report163226_8", Z3DTools.Scrolling_Tool, 50, 50, 100, reset: false);
                    if (result8)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }

                //step 09
                String locationvaluebefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                brz3dvp.select3DTools(Z3DTools.Reset);
                Thread.Sleep(3000);
                brz3dvp.select3DTools(Z3DTools.Reset);
                Thread.Sleep(3000);
                String locationvalueafter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if(locationvalueafter != locationvaluebefore && locationvalueafter.Equals(Requirements))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 10
                Result = brz3dvp.change3dsettings(BluRingZ3DViewerPage.MPRInteractiveQuality, 100);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 11
                Result = brz3dvp.change3dsettings(BluRingZ3DViewerPage.MPRFinalQuality, 50);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 12
                bool scrollResult12 = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationone, Z3DTools.Scrolling_Tool, 50, 50, 100);
                bool networkResult12 = brz3dvp.verifyimagevariations("report12", final: false);
                if (scrollResult12 && networkResult12)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 13
                String locationvaluebefore13 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                brz3dvp.select3DTools(Z3DTools.Reset);
                Thread.Sleep(3000);
                brz3dvp.select3DTools(Z3DTools.Reset);
                Thread.Sleep(3000);
                String locationvalueafter13 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (locationvalueafter13 != locationvaluebefore13 && locationvalueafter13.Equals(Requirements))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 14
                Result = brz3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                if(Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 15
                bool result15_1 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigation3D1, "report15_1", Z3DTools.Scrolling_Tool, 50, 50, 100, properties, values);
                bool result15_2 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigation3D1, "report15_2", Z3DTools.Scrolling_Tool, 50, 50, 100, properties, values2, imagevariationcheck: false);
                if(result15_1 && result15_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 16
                Result = brz3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 17
                bool result17_1 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigationone, "report17_1", Z3DTools.Scrolling_Tool, 50, 50, 100, properties, values);
                bool result17_2 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigationone, "report17_2", Z3DTools.Scrolling_Tool, 50, 50, 100, properties, values2, imagevariationcheck: false);
                bool result17_3 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigation3D1, "report17_3", Z3DTools.Scrolling_Tool, 50, 50, 100, properties, values);
                bool result17_4 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigation3D1, "report17_4", Z3DTools.Scrolling_Tool, 50, 50, 100, properties, values2, imagevariationcheck: false);
                if (result17_1 && result17_2)
                {
                    brz3dvp.ChangeViewMode();
                    bool result17_2_1 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigation3D2, "report17_2_1", Z3DTools.Scrolling_Tool, 50, 50, 100, properties, values);
                    bool result17_2_2 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigation3D2, "report17_2_2", Z3DTools.Scrolling_Tool, 50, 50, 100, properties, values2, imagevariationcheck: false);
                    brz3dvp.select3DTools(Z3DTools.Reset);
                    Thread.Sleep(3000);
                    brz3dvp.select3DTools(Z3DTools.Reset);
                    Thread.Sleep(3000);
                    if (result17_2_1 && result17_2_2)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 18
                Result = brz3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 19
                IWebElement navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(3000);
                new Actions(Driver).MoveToElement(navigation1, 3 * (navigation1.Size.Width / 4), navigation1.Size.Height / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(navigation1, 3 * (navigation1.Size.Width / 4), 3 * (navigation1.Size.Height / 4)).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int colordepthbefore = brz3dvp.LevelOfSelectedColor(navigation1, testid, ExecutedSteps + 191, 0, 0, 255);
                bool result19_1 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigationone, "report19_1", Z3DTools.Scrolling_Tool, 50, 50, 100, properties, values, reset : false);
                int colordepthafter = brz3dvp.LevelOfSelectedColor(navigation1, testid, ExecutedSteps + 192, 0, 0, 255);
                brz3dvp.select3DTools(Z3DTools.Reset);
                Thread.Sleep(3000);
                brz3dvp.select3DTools(Z3DTools.Reset);
                Thread.Sleep(3000);
                new Actions(Driver).MoveToElement(navigation1, 3 * (navigation1.Size.Width / 4), navigation1.Size.Height / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(navigation1, 3 * (navigation1.Size.Width / 4), 3 * (navigation1.Size.Height / 4)).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int colordepthbefore_2 = brz3dvp.LevelOfSelectedColor(navigation1, testid, ExecutedSteps + 193, 0, 0, 255);
                bool result19_2 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigationone, "report19_2", Z3DTools.Scrolling_Tool, 50, 50, 100, properties, values2, false, reset: false);
                int colordepthafter_2 = brz3dvp.LevelOfSelectedColor(navigation1, testid, ExecutedSteps + 194, 0, 0, 255);
                brz3dvp.select3DTools(Z3DTools.Reset);
                Thread.Sleep(3000);
                brz3dvp.select3DTools(Z3DTools.Reset);
                Thread.Sleep(3000);
                if (result19_1 && result19_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 20
                brz3dvp.CloseViewer();
                login.Logout();
                SetVMResolution("2560", "1600");
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool searchResult20 = brz3dvp.searchandopenstudyin3D(Patientid, ImageCount, ChangeSettings: "No");
                if (searchResult20)
                {
                    navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                    new Actions(Driver).MoveToElement(navigation1).SendKeys("x").Build().Perform();
                    Thread.Sleep(5000);
                    bool result20_1 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigationone, "report20_1", Z3DTools.Scrolling_Tool, 50, 50, 100, properties, values);
                    bool scrollResult20_1 = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationone, Z3DTools.Scrolling_Tool, 50, 50, 100);
                    Thread.Sleep(2000);
                    bool networkResult20_1 = brz3dvp.verifyimagevariations("report20_1");
                    bool result20_2 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigationone, "report20_2", Z3DTools.Scrolling_Tool, 50, 50, 100, properties, values2, imagevariationcheck: false);

                    brz3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                    PageLoadWait.WaitForFrameLoad(10);
                    bool result20_3 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigation3D1, "report20_3", Z3DTools.Scrolling_Tool, 50, 50, 100, properties, values);
                    bool result20_4 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigation3D1, "report20_4", Z3DTools.Scrolling_Tool, 50, 50, 100, properties, values2, imagevariationcheck: false);

                    brz3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                    PageLoadWait.WaitForFrameLoad(10);
                    bool result20_5 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigationone, "report20_5", Z3DTools.Scrolling_Tool, 50, 50, 100, properties, values);
                    bool result20_6 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigationone, "report20_6", Z3DTools.Scrolling_Tool, 50, 50, 100, properties, values2, imagevariationcheck: false);
                    bool result20_7 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigation3D1, "report20_7", Z3DTools.Scrolling_Tool, 50, 50, 100, properties, values);
                    bool result20_8 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigation3D1, "report20_8", Z3DTools.Scrolling_Tool, 50, 50, 100, properties, values2, imagevariationcheck: false);
                    brz3dvp.ChangeViewMode();
                    PageLoadWait.WaitForFrameLoad(10);
                    bool result20_11 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigation3D2, "report20_11", Z3DTools.Scrolling_Tool, 50, 50, 100, properties, values);
                    bool result20_12 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigation3D2, "report20_12", Z3DTools.Scrolling_Tool, 50, 50, 100, properties, values2, imagevariationcheck: false);
                    brz3dvp.select3DTools(Z3DTools.Reset);
                    Thread.Sleep(3000);
                    brz3dvp.select3DTools(Z3DTools.Reset);
                    Thread.Sleep(3000);
                    brz3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                    PageLoadWait.WaitForFrameLoad(10);
                    navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                    new Actions(Driver).MoveToElement(navigation1, 3 * (navigation1.Size.Width / 4), navigation1.Size.Height / 4).Click().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    new Actions(Driver).MoveToElement(navigation1, 3 * (navigation1.Size.Width / 4), 3 * (navigation1.Size.Height / 4)).Click().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    bool result20_13 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigationone, "result20_13", Z3DTools.Scrolling_Tool, 50, 50, 100, properties, values, reset: false);
                    int colordepthafter20 = brz3dvp.LevelOfSelectedColor(navigation1, testid, ExecutedSteps + 202, 0, 0, 255);
                    brz3dvp.select3DTools(Z3DTools.Reset);
                    Thread.Sleep(3000);
                    brz3dvp.select3DTools(Z3DTools.Reset);
                    Thread.Sleep(3000);
                    new Actions(Driver).MoveToElement(navigation1, 3 * (navigation1.Size.Width / 4), navigation1.Size.Height / 4).Click().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    new Actions(Driver).MoveToElement(navigation1, 3 * (navigation1.Size.Width / 4), 3 * (navigation1.Size.Height / 4)).Click().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    int colordepthbefore20_2 = brz3dvp.LevelOfSelectedColor(navigation1, testid, ExecutedSteps + 203, 0, 0, 255);
                    bool result20_14 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigationone, "result20_14", Z3DTools.Scrolling_Tool, 50, 50, 100, properties, values2, false, reset: false);
                    brz3dvp.select3DTools(Z3DTools.Reset);
                    Thread.Sleep(3000);
                    brz3dvp.select3DTools(Z3DTools.Reset);
                    Thread.Sleep(3000);
                    if (scrollResult20_1 && networkResult20_1 && result20_2 && result20_3 && result20_4 && result20_5 && result20_6 && result20_7 && result20_8 && result20_11 && result20_12 && result20_13 && result20_14)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    throw new Exception("Unable to find the study in the resolution 2560x1600");


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
                SetVMResolution("1280", "1024");
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                BasePage.MultiDriver.Add(login.InvokeBrowser(browserName));
                login.SetDriver(BasePage.MultiDriver[1]);
                login.DriverGoTo(login.url);
            }
        }

        public TestCaseResult Test_163227(string testid, String teststeps, int stepcount)
        {
            
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string Requirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                BasePage.MultiDriver.Clear();
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                BasePage.MultiDriver.Add(login.InvokeBrowser("chromenetwork"));
                login.SetDriver(BasePage.MultiDriver[0]);
                login.DriverGoTo(login.url);

                //step 01 - 03
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool Result = brz3dvp.searchandopenstudyin3D(Patientid, ImageCount, ChangeSettings: "No");
                if (Result)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed while launching study in 3D Viewer");
                    throw new Exception("Failed to open study in 3D Viewer");
                }

                //step 04 - 06
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)).SendKeys("x").Build().Perform();
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].StepPass();
                result.steps[++ExecutedSteps].StepPass();
                result.steps[++ExecutedSteps].StepPass();
                Logger.Instance.InfoLog("Network capturing will be initiated in the upcoming steps");

                //step 07
                brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationone, Z3DTools.Scrolling_Tool, 50, 50, 100);
                Thread.Sleep(2000);
                bool result7 = brz3dvp.verifyimagevariations("report163227_7_Sample");
                if (result7)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result7 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigationone, "report163227_7", Z3DTools.Scrolling_Tool, 50, 50, 100, reset: false);
                    if (result7)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }

                //step 08 & 09
                bool result8 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigationone, "report163228_8", Z3DTools.Sculpt_Tool_for_3D_1_Polygon, 150, 50, 100, reset: false, caseid: testid, executedsteps: ExecutedSteps + 8);
                if (result8)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step 10
                String annotationvaluebefore10 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                int blackcolorbefore10 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 101, 0, 0, 0);
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                int blackcolorafter10 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 102, 0, 0, 0);
                String annotationvalueafter10 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (blackcolorafter10 != blackcolorbefore10 && annotationvalueafter10 != annotationvaluebefore10 && annotationvalueafter10.Equals(Requirements))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 11 & 12
                bool result12 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigationone, "report163228_12", Z3DTools.Selection_Tool, 0, 0, 0, reset: false, caseid: testid, executedsteps: ExecutedSteps + 12);
                if (result12)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step 13
                int bluecolorbefore13 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 131, 0, 0, 255);
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.select3DTools(Z3DTools.Reset);
                Thread.Sleep(3000);
                int bluecolorafter13 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 132, 0, 0, 255);
                if (bluecolorafter13 != bluecolorbefore13)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 14
                brz3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                Thread.Sleep(2000);
                bool result14_1 = brz3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                Logger.Instance.InfoLog("The result of selecting MPR 6:1 layout is : " + result14_1);
                bool result14_2 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigationone, "report163227_14_1", Z3DTools.Scrolling_Tool, 50, 50, 100, reset: false);
                Logger.Instance.InfoLog("The result of scroll tool application over navigation 1 in MPR 6:1 layout is : " + result14_2);
                bool result14_3 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigationtwo, "report163228_14_2", Z3DTools.Sculpt_Tool_for_3D_1_Polygon, 50, 50, 100, reset: false, caseid: testid, executedsteps: ExecutedSteps + 141);
                Logger.Instance.InfoLog("The result of sculpt tool application over navigation 2 in MPR 6:1 layout is : " + result14_3);
                bool result14_4 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigationthree, "report163228_14_3", Z3DTools.Selection_Tool, 50, 0, 0, reset: false, caseid: testid, executedsteps: ExecutedSteps + 142);
                Logger.Instance.InfoLog("The result of selection tool application over navigation 3 in MPR 6:1 layout is : " + result14_4);
                String annotationvalue1before14 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                int bluecolorbefore14_1 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 144, 0, 0, 255);
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.select3DTools(Z3DTools.Reset);
                Thread.Sleep(3000);
                brz3dvp.select3DTools(Z3DTools.Reset);
                Thread.Sleep(2000);
                String annotationvalue1after14 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                int bluecolorafter14_1 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 145, 0, 0, 255);
                bool colorcheck14_1 = bluecolorafter14_1 != bluecolorbefore14_1;
                Logger.Instance.InfoLog("The result of color verification in MPR 6:1 layout is : " + colorcheck14_1);
                bool annotationcheck14_1 = annotationvalue1after14 != annotationvalue1before14 && annotationvalue1after14.Equals(Requirements);
                Logger.Instance.InfoLog("The result of annotation verification in MPR 6:1 layout is : " + annotationcheck14_1);

                brz3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                Thread.Sleep(3000);
                bool result14_5 = brz3dvp.ChangeViewMode();
                Logger.Instance.InfoLog("The result of changing 3D 6:1 layout is : " + result14_5);
                bool result14_6 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigation3D1, "report163227_14_4", Z3DTools.Scrolling_Tool, 50, 50, 100, reset: false);
                Logger.Instance.InfoLog("The result of scroll tool application over navigation 3D1 in 3D 6:1 layout is : " + result14_6);
                bool result14_7 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigation3D1, "report163228_14_5", Z3DTools.Sculpt_Tool_for_3D_1_Polygon, 50, 50, 100, reset: false, caseid: testid, executedsteps: ExecutedSteps + 142);
                Logger.Instance.InfoLog("The result of sculpt tool application over navigation 3D1 in 3D 6:1 layout is : " + result14_7);
                bool result14_8 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigation3D2, "report163228_14_6", Z3DTools.Selection_Tool, 100, 0, 0, reset: false, caseid: testid, executedsteps: ExecutedSteps + 143);
                Logger.Instance.InfoLog("The result of selection tool application over navigation 3D2 in 3D 6:1 layout is : " + result14_8);
                int bluecolorbefore14_2 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 146, 0, 0, 255);
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                int bluecolorafter14_2 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 147, 0, 0, 255);
                bool colorcheck14_2 = bluecolorafter14_2 != bluecolorbefore14_2;
                Logger.Instance.InfoLog("The result of color verification in 3D 6:1 layout is : " + colorcheck14_2);
                brz3dvp.select3DTools(Z3DTools.Scrolling_Tool);

                bool result14_9 = brz3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                Logger.Instance.InfoLog("The result of changing to Curved MPR layout is : " + result14_9);
                IWebElement Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                int bluecolorbefore14_3 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 148, 0, 0, 255);
                new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2, 3 * (Navigation1.Size.Height / 4)).Click().Build().Perform();
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                Thread.Sleep(5000);
                new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2, Navigation1.Size.Height / 4).Click().Build().Perform();
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                Thread.Sleep(5000);
                int bluecolorafter14_3 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 149, 0, 0, 255);
                bool colorcheck14_3 = bluecolorafter14_3 != bluecolorbefore14_3;
                Logger.Instance.InfoLog("The result of color verification in Curved MPR Layout: " + colorcheck14_3);
                bool networkverify = brz3dvp.verifyimagevariations("report163228_14_7");
                Logger.Instance.InfoLog("The result of network trace verification in Curved MPR Layout: " + networkverify);
                bool result14_10 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigationone, "report163227_14_8", Z3DTools.Scrolling_Tool, 50, 50, 100, reset: false, direction: "positive");
                Logger.Instance.InfoLog("The result of scroll tool application over navigation 1 in Curved MPR Layout: " + result14_10);

                String annotationvaluebefore14_2 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                String annotationvalueafter14_2 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                int bluecolorafter14_4 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 148, 0, 0, 255);
                bool colorcheck14_4 = bluecolorafter14_4 != bluecolorafter14_3;
                Logger.Instance.InfoLog("The result of color verification in Curved MPR Layout: " + colorcheck14_4);
                bool annotationcheck14_2 = annotationvaluebefore14_2 != annotationvalueafter14_2 && annotationvalueafter14_2.Equals(Requirements);
                Logger.Instance.InfoLog("The result of annotation verification in Curved MPR Layout: " + annotationcheck14_2);
                if (result14_1 && result14_2 && result14_3 && result14_4 && result14_5 && result14_6 && result14_7 && result14_8 && result14_9 && result14_10 && annotationcheck14_1 && annotationcheck14_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 15
                bool result15 = brz3dvp.select3dlayout(BluRingZ3DViewerPage.CalciumScoring);
                if (result15)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 16
                bool result16 = brz3dvp.VerifyToolSelected(BluRingZ3DViewerPage.CalciumScoring, BluRingZ3DViewerPage.CalciumScoring);
                if (result16)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 17
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA");
                PageLoadWait.WaitForFrameLoad(5);
                IWebElement CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int GreenColorBefore17 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 171, 0, 255, 0, 2);
                Actions calciumaction = new Actions(Driver);
                calciumaction.MoveToElement(CalciumScoreImage, CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Height / 2 + 10).ClickAndHold()
                    .MoveToElement(CalciumScoreImage, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Height / 2 + 200)
                    .MoveToElement(CalciumScoreImage, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Height / 2 + 200)
                    .MoveToElement(CalciumScoreImage, CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Height / 2 + 200).Release().Build().Perform();
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                int GreenColorAfter17 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 172, 0, 255, 0, 2);
                if (GreenColorAfter17 != GreenColorBefore17)
                {
                    bool result17 = brz3dvp.verifyimagevariations("report163227_17");
                    if(result17)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 18
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                int GreenColorAfter18 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 181, 0, 255, 0, 2);
                bool greencolorcheck = GreenColorAfter18 != GreenColorAfter17;
                if (greencolorcheck)
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
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                BasePage.MultiDriver.Add(login.InvokeBrowser(browserName));
                login.SetDriver(BasePage.MultiDriver[1]);
                login.DriverGoTo(login.url);
            }
        }

        public TestCaseResult Test_163228(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string Requirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                BasePage.MultiDriver.Clear();
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                BasePage.MultiDriver.Add(login.InvokeBrowser("chromenetwork"));
                login.SetDriver(BasePage.MultiDriver[0]);
                login.DriverGoTo(login.url);

                //step 01 - 03
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool Result = brz3dvp.searchandopenstudyin3D(Patientid, ImageCount, ChangeSettings: "No");
                if (Result)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed while launching study in 3D Viewer");
                    throw new Exception("Failed to open study in 3D Viewer");
                }

                //step 04 - 06
                result.steps[++ExecutedSteps].StepPass();
                result.steps[++ExecutedSteps].StepPass();
                result.steps[++ExecutedSteps].StepPass();
                Logger.Instance.InfoLog("Network capturing will be initiated in the upcoming steps");

                //step 07
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationone, Z3DTools.Scrolling_Tool, 50, 50, 100);
                Thread.Sleep(2000);
                bool result7_1 = brz3dvp.verifyimagevariations("report163228_7_sample");
                if (result7_1)
                    Logger.Instance.InfoLog("Scroll tool verification passed in step 07");
                else
                    result7_1 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigationone, "report163228_7_1", Z3DTools.Scrolling_Tool, 50, 50, 100, reset: false);
                bool result7_2 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigationone, "report163228_7_2", Z3DTools.Rotate_Tool_1_Click_Center, 50, 50, 100, reset: false);
                bool result7_3 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigationone, "report163228_7_3", Z3DTools.Interactive_Zoom, 50, 50, 100, reset: false);
                bool result7_4 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigationone, "report163228_7_4", Z3DTools.Pan, 50, 50, 100, reset: false);
                if (result7_1 && result7_2 && result7_3 && result7_4)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 08
                String annotation1before8 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                String annotation1after8 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (annotation1after8 != annotation1before8 && annotation1after8.Equals(Requirements))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 09
                bool result9 = brz3dvp.EnableOneViewupMode(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone));
                if(result9)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 10
                bool result10_1 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigationone, "report163228_10_1", Z3DTools.Scrolling_Tool, 50, 50, 100, reset: false);
                bool result10_2 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigationone, "report163228_10_2", Z3DTools.Rotate_Tool_1_Click_Center, 50, 50, 100, reset: false);
                bool result10_3 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigationone, "report163228_10_3", Z3DTools.Interactive_Zoom, 50, 50, 100, reset: false);
                bool result10_4 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigationone, "report163228_10_4", Z3DTools.Pan, 50, 50, 100, reset: false);
                if (result10_1 && result10_2 && result10_3 && result10_4)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 11
                String annotation1before11 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                String annotation1after11 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (annotation1after11 != annotation1before11 && annotation1after11.Equals(Requirements))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 12
                bool result12 = brz3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                if(result12)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 13
                bool result13_1 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigation3D1, "report163228_13_1", Z3DTools.Scrolling_Tool, 50, 50, 100, reset: false);
                bool result13_2 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigation3D1, "report163228_13_2", Z3DTools.Rotate_Tool_1_Click_Center, 50, 50, 100, reset: false);
                bool result13_3 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigation3D1, "report163228_13_3", Z3DTools.Interactive_Zoom, 50, 50, 100, reset: false);
                if (result13_1 && result13_2 && result13_3)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 14
                String annotation1before14 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                String annotation1after14 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                if (annotation1after14 != annotation1before14)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 15
                bool result15 = brz3dvp.EnableOneViewupMode(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1));
                if (result15)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 16
                bool result16_1 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigation3D1, "report163228_16_1", Z3DTools.Scrolling_Tool, 50, 50, 100, reset: false);
                bool result16_2 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigation3D1, "report163228_16_2", Z3DTools.Rotate_Tool_1_Click_Center, 50, 50, 100, reset: false);
                bool result16_3 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigation3D1, "report163228_16_3", Z3DTools.Interactive_Zoom, 50, 50, 100, reset: false);
                if (result16_1 && result16_2 && result16_3)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 17
                String annotation1before17 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                String annotation1after17 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                if (annotation1after17 != annotation1before17)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 18
                bool result18 = brz3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                if (result18)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 19
                bool result19_1 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigationone, "report163228_19_1", Z3DTools.Scrolling_Tool, 50, 50, 100, reset: false);
                bool result19_2 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigationone, "report163228_19_2", Z3DTools.Rotate_Tool_1_Click_Center, 50, 50, 100, reset: false);
                bool result19_3 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigationone, "report163228_19_3", Z3DTools.Interactive_Zoom, 50, 50, 100, reset: false);
                bool result19_4 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigationone, "report163228_19_3", Z3DTools.Pan, 50, 50, 100, reset: false);
                bool result19_5 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigation3D1, "report163228_19_1", Z3DTools.Scrolling_Tool, 50, 50, 100, reset: false);
                bool result19_6 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigation3D1, "report163228_19_2", Z3DTools.Rotate_Tool_1_Click_Center, 50, 50, 100, reset: false);
                bool result19_7 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigation3D1, "report163228_19_3", Z3DTools.Interactive_Zoom, 50, 50, 100, reset: false);
                if (result19_1 && result19_2 && result19_3 && result19_4 && result19_5 && result19_6 && result19_7)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 20
                String annotation1before20_1 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                String annotation1before20_2 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                String annotation1after20_1 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                String annotation1after20_2 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                bool annotationcheck20_1 = annotation1before20_1 != annotation1after20_1 && annotation1after20_1.Equals(Requirements);
                bool annotationcheck20_2 = annotation1before20_2 != annotation1after20_2;
                if (annotationcheck20_1 && annotationcheck20_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 21
                bool result21 = brz3dvp.EnableOneViewupMode(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1));
                if (result21)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 22
                bool result22_1 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigation3D1, "report163228_22_1", Z3DTools.Scrolling_Tool, 50, 50, 100, reset: false);
                bool result22_2 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigation3D1, "report163228_22_2", Z3DTools.Rotate_Tool_1_Click_Center, 50, 50, 100, reset: false);
                bool result22_3 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.Navigation3D1, "report163228_22_3", Z3DTools.Interactive_Zoom, 50, 50, 100, reset: false);
                if (result22_1 && result22_2 && result22_3)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 23
                String annotation1before23 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigation3D1);
                PageLoadWait.WaitForFrameLoad(10);
                String annotation1after23 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                if (annotation1after23 != annotation1before23)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 24
                bool result24 = brz3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                if(result24)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 25
                IWebElement Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                int bluecolorbefore25 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 251, 0, 0, 255);
                new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2, Navigation1.Size.Height / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2, 3 * (Navigation1.Size.Height / 4)).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int bluecolorafter25 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 252, 0, 0, 255);
                if(bluecolorafter25 != bluecolorbefore25)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 26
                bool result26_1 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.MPRPathNavigation, "report163228_26_1", Z3DTools.Scrolling_Tool, 50, 50, 100, reset: false, direction: "positive");
                if(result26_1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 27
                bool result27 = brz3dvp.EnableOneViewupMode(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation));
                if(result27)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 28
                bool result28_1 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage.MPRPathNavigation, "report163228_28_1", Z3DTools.Scrolling_Tool, 50, 50, 100, reset: false, direction: "positive");
                if (result28_1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 29
                IWebElement MPRPathNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                var sizebefore = MPRPathNavigation.Size;
                new Actions(Driver).MoveToElement(MPRPathNavigation).DoubleClick().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                var sizeafter = MPRPathNavigation.Size;
                if (sizebefore.Width > sizeafter.Width && sizebefore.Height > sizeafter.Height)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 30
                bool result30_1 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage._3DPathNavigation, "report163228_30_1", Z3DTools.Scrolling_Tool, 50, 50, 100, reset: false, direction: "positive");
                if (result30_1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 31
                bool result31 = brz3dvp.EnableOneViewupMode(brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation));
                if (result31)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 32
                bool result32_1 = brz3dvp.Applytoolandverifyimagevariation(BluRingZ3DViewerPage._3DPathNavigation, "report163228_32_1", Z3DTools.Scrolling_Tool, 50, 50, 100, reset: false, direction: "positive");
                if (result32_1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 33
                String annotation1before31_2 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage._3DPathNavigation);
                PageLoadWait.WaitForFrameLoad(10);
                String annotation1after31_2 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                bool annotationcheck31_2 = annotation1before31_2 != annotation1after31_2 && annotation1after31_2.Equals(Requirements);
                if (annotationcheck31_2)
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
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                BasePage.MultiDriver.Add(login.InvokeBrowser(browserName));
                login.SetDriver(BasePage.MultiDriver[1]);
                login.DriverGoTo(login.url);
            }
        }
    }
}
