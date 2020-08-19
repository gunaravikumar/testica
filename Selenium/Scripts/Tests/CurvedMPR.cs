using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
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
using System.Windows.Forms;

namespace Selenium.Scripts.Tests
{
    class CurvedMPR : BasePage
    {
        public string filepath { get; set; }
        Login login;

        public CurvedMPR(String classname)
        {
            this.login = new Login();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";

        }

        public TestCaseResult Test_124691(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            BluRingZ3DViewerPage Z3dViewer = new BluRingZ3DViewerPage();
            String adminUserName = Config.adminUserName;
            String adminPassword = Config.adminPassword;
            String patientId = "66677723";
            String thumbImg = "Date: 23 - Jun - 2013 2:20:16 PM";

            try
            {
                //Step1
                login.LoginIConnect(adminUserName, adminPassword);
                Z3dViewer.searchandopenstudyin3D(patientId, thumbImg, "Curved MPR");


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
        public TestCaseResult Test_124703(String testid, String teststeps, int stepcount)
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
            String patientId = "2009000044";
            String thumbImg = "Modality:CT";
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
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

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
                Z3dViewer.EnterThickness("MPR Path Navigation", "7");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, "body > div:nth-child(3) > blu-ring-root > div > blu-ring-study-viewer > form > div > div.studyPanelsContainer > blu-ring-study-panel-container > div > blu-ring-study-panel-control > div > div.compositeViewerContainer > div > blu-ring-z3d-composite-viewer > div > div:nth-child(7) > blu-ring-viewer-host-component > div > blu-ring-viewer3d > div > div.tilepanel.unselectable > div > blu-ring-imagetile > div > div > div.fill.unselectable"));
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

        public TestCaseResult Test_124694(String testid, String teststeps, int stepcount)
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
            String patientId = "2009000044";
            String thumbImg = "Modality:CT";
            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            try
            {

                //step1
                login.LoginIConnect(adminUserName, adminPassword);
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //step2
                login.Navigate("Studies");
                login.ClearFields();
                login.SearchStudy("Accession", "FFP");
                login.SelectStudy("Accession", "FFP");
                viewer = BluRingViewer.LaunchBluRingViewer(fieldname: "Accession", value: "FFP");
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step3
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                bool step2_1 = Z3dViewer.select3dlayout("Curved MPR");

                Cursor.Position = new System.Drawing.Point((Z3dViewer.Studypanel().Size.Width) / 2, (Z3dViewer.Studypanel().Size.Height) / 3);
                IList<IWebElement> Viewport = Z3dViewer.Viewport();
                string ele = Viewport[0].GetCssValue("cursor");
                string[] AllCursormode = ele.Split('"');
                string[] windowlevel = AllCursormode[1].Split('/');
                if (windowlevel[7] == "cursor_cobb_wht_32.png")
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

                //Step4
                string Nvigation1Img = "CrossHair" + new Random().Next(1000) + ".png";
                IWebElement Navigationnametwo = Z3dViewer.controlelement(Config.Navigationone);
                Actions act = new Actions(Driver);
                act.MoveToElement(Navigationnametwo, Navigationnametwo.Size.Width / 2, Navigationnametwo.Size.Height / 2);
                DownloadImageFile(Z3dViewer.controlelement(Config.Navigationone), testcasefolder + Path.DirectorySeparatorChar + Nvigation1Img);
                Thread.Sleep(10000);
                string RedCrossImage = "RCrossHair" + new Random().Next(1000) + ".png";
                string BlueCrossImage = "BCrossHair" + new Random().Next(1000) + ".png";
                Z3dViewer.redcolorsplitter(testcasefolder + Nvigation1Img, testcasefolder + RedCrossImage);
                Z3dViewer.bluecolorsplitter(testcasefolder + Nvigation1Img, testcasefolder + BlueCrossImage);
                Accord.Point TopPoint = Z3dViewer.intersectionpoint(testcasefolder + BlueCrossImage, "Vertical", 0);
                Accord.Point BottomPoint = Z3dViewer.intersectionpoint(testcasefolder + BlueCrossImage, "Vertical", 6);
                Accord.Point RightPoint = Z3dViewer.intersectionpoint(testcasefolder + RedCrossImage, "", 1);
                Accord.Point LeftPoint = Z3dViewer.intersectionpoint(testcasefolder + RedCrossImage, "", 6);

                Thread.Sleep(10000);
                Actions act5 = new Actions(Driver);
                act5.MoveToElement(Z3dViewer.canelepath(Config.Navigationone), (int)TopPoint.X, (int)TopPoint.Y).DoubleClick().Build().Perform();
                Thread.Sleep(10000);
                Logger.Instance.InfoLog("Point1 clicked");
                act5.MoveToElement(Z3dViewer.canelepath(Config.Navigationone), (int)TopPoint.X, (int)BottomPoint.Y).DoubleClick().Build().Perform();
                Thread.Sleep(10000);
                Logger.Instance.InfoLog("Point2 clicked");
                act5.MoveToElement(Z3dViewer.canelepath(Config.Navigationone), (int)RightPoint.X, (int)RightPoint.Y).DoubleClick().Build().Perform();
                Thread.Sleep(10000);
                Logger.Instance.InfoLog("Point3 clicked");
                act5.MoveToElement(Z3dViewer.canelepath(Config.Navigationone), (int)LeftPoint.X, (int)RightPoint.Y).DoubleClick().Build().Perform();
                Thread.Sleep(10000);
                Logger.Instance.InfoLog("Point4 clicked");
                //.ClickAndHold().MoveToElement(Z3dViewer.canelepath(Config.Navigationtwo), (int)p1.X, (int)p1.Y).Release().Build().Perform();
                Thread.Sleep(20000);


                Thread.Sleep(10000);
                Z3dViewer.ScrollTillPosition(Config.MPRPathNavigation, "RI", "down");
                //BasePage.SetCursorPos(363, 747);
                BasePage.mouse_event(0x0800, 0, 0, 100, 0);
                Z3dViewer.ScrollTillPosition(Config.MPRPathNavigation, "RI", "down");
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
    }
}
