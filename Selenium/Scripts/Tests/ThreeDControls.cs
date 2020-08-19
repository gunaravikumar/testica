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
using Accord;

namespace Selenium.Scripts.Tests
{
    class ThreeDControls : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public Imager imager = new Imager();
        DirectoryInfo CurrentDir = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());
        public ThreeDControls(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }


        public TestCaseResult Test_163230(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            string AceesionDetails = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            string SearchField = AceesionDetails.Split('|')[0];
            string AccessionNo = AceesionDetails.Split('|')[1];
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step:1
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                result.steps[++ExecutedSteps].StepPass();

                //Step:2
                bool res = z3dvp.searchandopenstudyin3D(AccessionNo, ImageCount,field: SearchField);
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    Logger.Instance.ErrorLog("Unable to open study in 3D Viewer");
                    throw new Exception("Failed to open study in 3D Viewer");
                }

                //Step:3
                res = z3dvp.select3dlayout("3D 4:1 Layout");
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:4
                IWebElement Control3D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                res = z3dvp.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.Navigation3D1);
                if (!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                {
                    Control3D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                    String BeforeImagePath = Config.downloadpath + "\\Before" + testid + "_" + ExecutedSteps + ".png";
                    String AfterImagePath = Config.downloadpath + "\\After" + testid + "_" + ExecutedSteps + ".png";
                    Logger.Instance.InfoLog("BeforeImagepath is  " + BeforeImagePath);
                    PageLoadWait.WaitForFrameLoad(5);
                    DownloadImageFile(Control3D1, BeforeImagePath, "png");
                    new Actions(Driver).MoveToElement(Control3D1, Control3D1.Size.Width / 4, Control3D1.Size.Height / 4).ClickAndHold()
                        .MoveToElement(Control3D1, (Control3D1.Size.Width / 4),(Control3D1.Size.Height / 4) + 10).Release().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    DownloadImageFile(Control3D1, AfterImagePath, "png");
                    if (!CompareImage(AfterImagePath, BeforeImagePath))
                    {
                        res = z3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigation3D1, Z3DTools.Scrolling_Tool, 50, 50, 100, movement: "positive");
                        if (!res)
                            result.steps[++ExecutedSteps].StepFail();
                        else
                        {
                            res = z3dvp.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.Navigation3D1);
                            if (!res)
                                result.steps[++ExecutedSteps].StepFail();
                            else
                            {
                                Control3D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                                BeforeImagePath = Config.downloadpath + "\\Before" + testid + "_" + ExecutedSteps + "_1.png";
                                AfterImagePath = Config.downloadpath + "\\After" + testid + "_" + ExecutedSteps + "_1.png";
                                Logger.Instance.InfoLog("BeforeImagepath is  " + BeforeImagePath);
                                PageLoadWait.WaitForFrameLoad(5);
                                DownloadImageFile(Control3D1, BeforeImagePath, "png");
                                new Actions(Driver).MoveToElement(Control3D1, Control3D1.Size.Width / 4, Control3D1.Size.Height / 4).ClickAndHold()
                                    .MoveToElement(Control3D1, (Control3D1.Size.Width / 4), (Control3D1.Size.Height / 4) + 10).Release().Build().Perform();
                                PageLoadWait.WaitForFrameLoad(10);
                                DownloadImageFile(Control3D1, AfterImagePath, "png");
                                if (!CompareImage(AfterImagePath, BeforeImagePath))
                                {
                                    res = z3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigation3D1, Z3DTools.Rotate_Tool_1_Click_Center, 50, 50, 100);
                                    if (!res)
                                        result.steps[++ExecutedSteps].StepFail();
                                    else
                                    {
                                        res = z3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigation3D1, Z3DTools.Line_Measurement, 30, 80, 20, testid, ExecutedSteps + 1);
                                        if (!res)
                                            result.steps[++ExecutedSteps].StepFail();
                                        else
                                            result.steps[++ExecutedSteps].StepPass();
                                    }
                                }
                                else
                                    result.steps[++ExecutedSteps].StepFail();
                            }
                        }
                    }
                    else
                    result.steps[++ExecutedSteps].StepFail();
                }
                
                //Step:5
                res = z3dvp.select3DTools(Z3DTools.Download_Image, BluRingZ3DViewerPage.Navigation3D1);
                if(res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:6
                Control3D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                Actions act = new Actions(Driver);
                act.MoveToElement(Control3D1, Control3D1.Size.Width / 4, Control3D1.Size.Height / 4).Click().Build().Perform();
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.saveimgdialg)));
                PageLoadWait.WaitForFrameLoad(5);
                string imgLocation = Config.downloadpath + "\\163230_06.jpg";
                if(File.Exists(imgLocation))
                {
                    Logger.Instance.InfoLog("File already exists");
                    File.Delete(imgLocation);
                }
                z3dvp.downloadImageForViewport("163230_06");
                if (!File.Exists(imgLocation))
                    result.steps[++ExecutedSteps].StepFail();
                else
                {
                    res = z3dvp.CompareDownloadimage(imgLocation);
                    if(!res)
                        result.steps[++ExecutedSteps].StepFail();
                    else
                        result.steps[++ExecutedSteps].StepPass();
                }
                if(browserName.Contains("chrome"))
                    z3dvp.CloseDownloadInfobar();
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(Control3D1).Build().Perform();

                //Step:7
                res = z3dvp.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.Navigation3D1);
                if(!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                    result.steps[++ExecutedSteps].StepPass();

                //Step:8
                String ImageLocationbefore = Config.downloadpath + "\\Before" + BluRingZ3DViewerPage.Preset13 + ".png";
                if (File.Exists(ImageLocationbefore))
                    File.Delete(ImageLocationbefore);
                DownloadImageFile(z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1), ImageLocationbefore, "png");
                res = z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Preset13, "Preset");
                if (!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                {
                    String ImageLocationafter = Config.downloadpath + "\\After" + BluRingZ3DViewerPage.Preset13 + ".png";
                    if (File.Exists(ImageLocationafter))
                        File.Delete(ImageLocationafter);
                    PageLoadWait.WaitForFrameLoad(10);
                    DownloadImageFile(z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1), ImageLocationafter, "png");
                    res = CompareImage(ImageLocationafter, ImageLocationbefore);
                    if(!res)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }

                //Step:9
                res = z3dvp.select3DTools(Z3DTools.Download_Image, BluRingZ3DViewerPage.Navigation3D1);
                if(!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                    result.steps[++ExecutedSteps].StepPass();

                //Step:10
                Control3D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                new Actions(Driver).MoveToElement(Control3D1, Control3D1.Size.Width / 4, Control3D1.Size.Height / 4).Click().Build().Perform();
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.saveimgdialg)));
                PageLoadWait.WaitForFrameLoad(5);
                imgLocation = Config.downloadpath + "\\163230_10.jpg";
                if (File.Exists(imgLocation))
                {
                    Logger.Instance.InfoLog("File already exists");
                    File.Delete(imgLocation);
                }
                z3dvp.downloadImageForViewport("163230_10");
                if (!File.Exists(imgLocation))
                    result.steps[++ExecutedSteps].StepFail();
                else
                {
                    res = z3dvp.CompareDownloadimage(imgLocation);
                    if (!res)
                        result.steps[++ExecutedSteps].StepFail();
                    else
                        result.steps[++ExecutedSteps].StepPass();
                }
                if(browserName.Contains("chrome"))
                    z3dvp.CloseDownloadInfobar();
                PageLoadWait.WaitForFrameLoad(10);

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

        public TestCaseResult Test_163242(string testid, String teststeps, int stepcount)
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
                //step:1 - Login Ica viewer
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                result.steps[++ExecutedSteps].StepPass();

                //Step2 & 3: The Series should be loaded with out any errors and MPR 4:1 viewing mode should be displayed by default..
                login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                login.SearchStudy("patient", Patientid);
                PageLoadWait.WaitForLoadingMessage(30);
                login.SelectStudy("Accession", "");
                PageLoadWait.WaitForFrameLoad(5);
                var viewer = BluRingViewer.LaunchBluRingViewer(fieldname: "Accession", value: "");
                PageLoadWait.WaitForFrameLoad(10);
                if (viewer == null)
                {
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[++ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[++ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Unable to launch the series in 3D Viewer");
                }
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }

                //Step:4 
                bool step4 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
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

                //Step:5 
                IList<IWebElement> Viewport = z3dvp.Viewport();
                bool step5 = z3dvp.EnableOneViewupMode(Viewport[3]);
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

                //Step:6
                bool Step6 = z3dvp.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.Navigation3D1);
                if (Step6)
                {
                    IWebElement ViewerPort = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                    z3dvp.Performdragdrop(ViewerPort, (ViewerPort.Size.Width / 2) - 200, (ViewerPort.Size.Height / 2) - 150, (ViewerPort.Size.Width / 2) - 200, (ViewerPort.Size.Height / 2) + 350);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    if (CompareImage(result.steps[ExecutedSteps], ViewerPort))
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
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:7
                bool Step7 = z3dvp.select3DTools(Z3DTools.Line_Measurement, BluRingZ3DViewerPage.Navigation3D1);
                if (Step7)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:8 & 9
                IWebElement Viewerport1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                //1mm
                z3dvp.Performdragdrop(Viewerport1, (Viewerport1.Size.Width / 2) - 40, (Viewerport1.Size.Height / 2) + 40, (Viewerport1.Size.Width / 2) - 14, (Viewerport1.Size.Height / 2) + 14);
                //2mm
                z3dvp.Performdragdrop(Viewerport1, (Viewerport1.Size.Width / 2) - 116, (Viewerport1.Size.Height / 2) + 116, (Viewerport1.Size.Width / 2) - 70, (Viewerport1.Size.Height / 2) + 75);
                //6mm
                z3dvp.Performdragdrop(Viewerport1, (Viewerport1.Size.Width / 2) - 306, (Viewerport1.Size.Height / 2) + 309, (Viewerport1.Size.Width / 2) - 178, (Viewerport1.Size.Height / 2) + 181);
                result.steps[ExecutedSteps += 2].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Viewerport1))
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

                //Step:10
                z3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                bool step10 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                if (step10)
                {
                    IWebElement ViewerPort1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                    z3dvp.EnableOneViewupMode(ViewerPort1);
                    PageLoadWait.WaitForFrameLoad(10);
                    bool zoom = z3dvp.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.Navigation3D1);
                    PageLoadWait.WaitForFrameLoad(10);
                    z3dvp.Performdragdrop(ViewerPort1, (ViewerPort1.Size.Width / 2) - 200, (ViewerPort1.Size.Height / 2) - 150, (ViewerPort1.Size.Width / 2) - 200, (ViewerPort1.Size.Height / 2) + 350);
                    PageLoadWait.WaitForFrameLoad(10);
                    bool linemeasure = z3dvp.select3DTools(Z3DTools.Line_Measurement, BluRingZ3DViewerPage.Navigation3D1);
                    PageLoadWait.WaitForFrameLoad(10);
                    //1mm
                    z3dvp.Performdragdrop(ViewerPort1, (ViewerPort1.Size.Width / 2) - 40, (ViewerPort1.Size.Height / 2) + 40, (ViewerPort1.Size.Width / 2) - 14, (ViewerPort1.Size.Height / 2) + 14);
                    PageLoadWait.WaitForFrameLoad(10);
                    //2mm
                    z3dvp.Performdragdrop(ViewerPort1, (ViewerPort1.Size.Width / 2) - 116, (ViewerPort1.Size.Height / 2) + 116, (ViewerPort1.Size.Width / 2) - 70, (ViewerPort1.Size.Height / 2) + 75);
                    PageLoadWait.WaitForFrameLoad(10);
                    //6mm
                    z3dvp.Performdragdrop(ViewerPort1, (ViewerPort1.Size.Width / 2) - 306, (ViewerPort1.Size.Height / 2) + 309, (ViewerPort1.Size.Width / 2) - 178, (ViewerPort1.Size.Height / 2) + 181);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    if (zoom && linemeasure && CompareImage(result.steps[ExecutedSteps], ViewerPort1))
                    {
                        z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigation3D1);
                        PageLoadWait.WaitForFrameLoad(10);
                        ViewerPort1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                        z3dvp.EnableOneViewupMode(ViewerPort1);
                        PageLoadWait.WaitForFrameLoad(10);
                        z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigation3D2, BluRingZ3DViewerPage.Preset4, "Preset");
                        PageLoadWait.WaitForFrameLoad(10);
                        zoom = z3dvp.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.Navigation3D2);
                        PageLoadWait.WaitForFrameLoad(10);
                        z3dvp.Performdragdrop(ViewerPort1, (ViewerPort1.Size.Width / 2) - 200, (ViewerPort1.Size.Height / 2) - 150, (ViewerPort1.Size.Width / 2) - 200, (ViewerPort1.Size.Height / 2) + 350);
                        PageLoadWait.WaitForFrameLoad(10);
                        linemeasure = z3dvp.select3DTools(Z3DTools.Line_Measurement, BluRingZ3DViewerPage.Navigation3D2);
                        PageLoadWait.WaitForFrameLoad(10);
                        //1mm
                        z3dvp.Performdragdrop(ViewerPort1, (ViewerPort1.Size.Width / 2) - 40, (ViewerPort1.Size.Height / 2) + 40, (ViewerPort1.Size.Width / 2) - 14, (ViewerPort1.Size.Height / 2) + 14);
                        PageLoadWait.WaitForFrameLoad(10);
                        //2mm
                        z3dvp.Performdragdrop(ViewerPort1, (ViewerPort1.Size.Width / 2) - 116, (ViewerPort1.Size.Height / 2) + 116, (ViewerPort1.Size.Width / 2) - 70, (ViewerPort1.Size.Height / 2) + 75);
                        PageLoadWait.WaitForFrameLoad(10);
                        //6mm
                        z3dvp.Performdragdrop(ViewerPort1, (ViewerPort1.Size.Width / 2) - 306, (ViewerPort1.Size.Height / 2) + 309, (ViewerPort1.Size.Width / 2) - 178, (ViewerPort1.Size.Height / 2) + 181);
                        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                        if (zoom && linemeasure && CompareImage(result.steps[ExecutedSteps], ViewerPort1))
                            result.steps[ExecutedSteps].StepPass();
                        else
                            result.steps[ExecutedSteps].StepFail();
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
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
                z3dvp.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_163240(string testid, String teststeps, int stepcount)
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

                //step:1 - Login iCA as administrator
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

                //Step:2 - The Series should be loaded with out any errors and MPR 4:1 viewing mode should be displayed by default..
                Boolean step2 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR);
                if (step2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    throw new Exception("Unable to load study successfully");
                }
                //Step:3:: From smart view drop down select 3D 6:1 view mode.
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

                //Step:4:: Hover top of the viewport 3D 1 control and select toggle 3D/MPR in order to display the 3D navigation controls.
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

                //Step:5::Select the zoom tool from floating toolbox.
                bool Step5 =  z3dvp.select3DTools(Z3DTools.Interactive_Zoom , BluRingZ3DViewerPage.Navigation3D1);
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
                //Step:6 :: Click and hold the left mouse button on 3D control 1, then move the mouse upward.
                IWebElement ViewerPort = z3dvp.ViewerContainer();
                IWebElement ThreeD1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                z3dvp.Performdragdrop(ThreeD1, (ThreeD1.Size.Width / 2) - 50, (ThreeD1.Size.Height / 2) - 80 , (ThreeD1.Size.Width / 2) - 50, (ThreeD1.Size.Height / 2) + 80);
                //new Actions(Driver).MoveToElement(ThreeD1, (ThreeD1.Size.Width / 2) - 50, (ThreeD1.Size.Height / 2) + 80).ClickAndHold().
                //MoveToElement(ThreeD1, (ThreeD1.Size.Width / 2) - 50, (ThreeD1.Size.Height / 2) - 80).Release().Build().Perform();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerPort))
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
                //Step:7 ::Click and hold the left mouse button on 3D control 2, then move the mouse downward.
                IWebElement ThreeD2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                z3dvp.Performdragdrop(ThreeD2, (ThreeD2.Size.Width / 2) - 50, (ThreeD2.Size.Height / 2) + 80 , (ThreeD2.Size.Width / 2) - 50, (ThreeD2.Size.Height / 2) - 80);
                //new Actions(Driver).MoveToElement(ThreeD2, (ThreeD2.Size.Width / 2) - 50, (ThreeD2.Size.Height / 2) - 80).ClickAndHold().
                //MoveToElement(ThreeD2, (ThreeD2.Size.Width / 2) - 50, (ThreeD2.Size.Height / 2) + 80).Release().Build().Perform();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                ViewerPort = z3dvp.ViewerContainer();
                if (CompareImage(result.steps[ExecutedSteps], ViewerPort))
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

                //Step:8
                bool Step8 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                if (Step8)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:9 :: Select the zoom tool by clicking the zoom button.
                bool Step9 = z3dvp.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.Navigation3D1);
                if (Step9)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:10 Move upwards
                ThreeD1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                z3dvp.Performdragdrop(ThreeD1, (ThreeD1.Size.Width / 2) - 50, (ThreeD1.Size.Height / 2) - 80 , (ThreeD1.Size.Width / 2) - 50, (ThreeD1.Size.Height / 2) + 80);
                //new Actions(Driver).MoveToElement(ThreeD1, (ThreeD1.Size.Width / 2) - 50, (ThreeD1.Size.Height / 2) + 80).ClickAndHold().
                //MoveToElement(ThreeD1, (ThreeD1.Size.Width / 2) - 50, (ThreeD1.Size.Height / 2) - 80).Release().Build().Perform();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                ViewerPort = z3dvp.ViewerContainer();
                if (CompareImage(result.steps[ExecutedSteps], ViewerPort))
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
                //Step:11 move downwards
                ThreeD1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                z3dvp.Performdragdrop(ThreeD1, (ThreeD1.Size.Width / 2) - 50, (ThreeD1.Size.Height / 2) + 80 , (ThreeD1.Size.Width / 2) - 50, (ThreeD1.Size.Height / 2) - 80);
                //new Actions(Driver).MoveToElement(ThreeD1, (ThreeD1.Size.Width / 2) - 50, (ThreeD1.Size.Height / 2) - 80).ClickAndHold().
                //MoveToElement(ThreeD1, (ThreeD1.Size.Width / 2) - 50, (ThreeD1.Size.Height / 2) + 80).Release().Build().Perform();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                ViewerPort = z3dvp.ViewerContainer();
                if (CompareImage(result.steps[ExecutedSteps], ViewerPort))
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

        public TestCaseResult Test_163239(string testid, String teststeps, int stepcount)
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
                //step:1 - Login Ica viewer
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                ExecutedSteps++;
                //Step2 & 3: The Series should be loaded with out any errors and MPR 4:1 viewing mode should be displayed by default..
                Boolean step3 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount);
                if (step3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
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

                //Step:4 
                bool step4 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
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

                //Step:5
                bool step5 = z3dvp.ChangeViewMode(BluRingZ3DViewerPage.Navigation3D1);
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

                //Step:6
                Boolean step6 =  z3dvp.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.Navigation3D1);
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

                //Step:7
                z3dvp.select3DTools(Z3DTools.Window_Level);
                IWebElement ViewerPort = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int ColorValBefore  =  z3dvp.LevelOfSelectedColor(ViewerPort, testid, ExecutedSteps + 1, 51, 51, 50, 3);
                z3dvp.Performdragdrop(ViewerPort, (ViewerPort.Size.Width / 2) - 50, (ViewerPort.Size.Height / 2) - 50, (ViewerPort.Size.Width / 2) + 50, (ViewerPort.Size.Height / 2) + 50);
                PageLoadWait.WaitForFrameLoad(6);
                int ColorValAfter = z3dvp.LevelOfSelectedColor(ViewerPort, testid, ExecutedSteps + 3, 51, 51, 50, 2);
                if (!ColorValBefore.Equals(ColorValAfter))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:8
                IWebElement ViewerPort1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int ColorValBefore1 = z3dvp.LevelOfSelectedColor(ViewerPort1, testid, ExecutedSteps + 1, 51, 51, 50, 3);
                z3dvp.Performdragdrop(ViewerPort1, (ViewerPort1.Size.Width / 2) - 50, (ViewerPort1.Size.Height / 2) - 50, (ViewerPort1.Size.Width / 2) + 50, (ViewerPort1.Size.Height / 2) + 50);
                Thread.Sleep(6000);
                int ColorValAfter1 = z3dvp.LevelOfSelectedColor(ViewerPort1, testid, ExecutedSteps + 3, 51, 51, 50, 3);
                IWebElement totalViewer1 = z3dvp.ViewerContainer();
                if (!ColorValBefore1.Equals(ColorValAfter1))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:9 & 10
                z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                z3dvp.select3DTools(Z3DTools.Window_Level);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                IWebElement ViewerPort2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                Thread.Sleep(2000);
                int ColorValBefore2 = z3dvp.LevelOfSelectedColor(ViewerPort2, testid, ExecutedSteps + 1, 51, 51, 50, 3);
                z3dvp.Performdragdrop(ViewerPort2, (ViewerPort2.Size.Width / 2) - 50, (ViewerPort2.Size.Height / 2) - 50, (ViewerPort2.Size.Width / 2) + 50, (ViewerPort2.Size.Height / 2) + 50);
                Thread.Sleep(4000);
                int ColorValAfter2 = z3dvp.LevelOfSelectedColor(ViewerPort2, testid, ExecutedSteps + 3, 51, 51, 50, 3);
                if (!ColorValBefore2.Equals(ColorValAfter2))
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


        public TestCaseResult Test_163238(string testid, String teststeps, int stepcount)
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
                //step:1 - Login Ica viewer
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
                //Step:2 The Series should be loaded with out any errors and MPR 4:1 viewing mode should be displayed by default..
                Boolean step2 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount , BluRingZ3DViewerPage.MPR);
                if (step2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    throw new Exception("Unable to load study successfully");
                }
                //Step:3 :: From smart view drop down select 3D 6:1 view mode.
                bool step3 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                if (step3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    throw new Exception("Unable to load study successfully");
                }
                //Step:4 :: Hover top of the viewport 3D 1 control and select toggle 3D/MPR in order to display the 3D navigation controls.
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
                //Step:5 Click the sculpting tool button from the floating toolbar and select the polygon sculpting tool from the drop down menu.
                bool step5 = z3dvp.select3DTools(Z3DTools.Sculpt_Tool_for_3D_1_Polygon , BluRingZ3DViewerPage.Navigation3D1);
                PageLoadWait.WaitForFrameLoad(10);
                IList<IWebElement> dialogsList = z3dvp.ToolBarDialogs();
                PageLoadWait.WaitForFrameLoad(10);
                if (step5 && dialogsList[0].Text.Equals(BluRingZ3DViewerPage.SculptToolPolygondialog))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step: 6
                IWebElement ViewerPane = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                int Beforevalue = z3dvp.LevelOfSelectedColor(z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 2, 51, 51, 50, 3);
                int Whiteline1 = z3dvp.LevelOfSelectedColor(z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps+1, 255, 255, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(ViewerPane, ViewerPane.Size.Width / 2 - 20, ViewerPane.Size.Height / 2 + 50).Click().
                MoveToElement(ViewerPane, ViewerPane.Size.Width / 2 + 80, ViewerPane.Size.Height / 2 + 50).Build().Perform();
                Thread.Sleep(4000);
                int Whiteline2 = z3dvp.LevelOfSelectedColor(z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 1, 255, 255, 255, 2, isMoveCursor: false);
                if (Whiteline1 != Whiteline2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 7::
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).Click().MoveToElement(ViewerPane, ViewerPane.Size.Width / 2 - 20, ViewerPane.Size.Height / 2 - 50).Build().Perform();
                Thread.Sleep(4000);
                int Whiteline3 = z3dvp.LevelOfSelectedColor(z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 1, 255, 255, 255, 2, isMoveCursor: false);
                if (Whiteline3 != Whiteline2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 8::
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).Click().MoveToElement(ViewerPane, ViewerPane.Size.Width / 2 - 20, ViewerPane.Size.Height / 2 + 50).Build().Perform();
                Thread.Sleep(4000);
                int Whiteline4 = z3dvp.LevelOfSelectedColor(z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 1, 255, 255, 255, 2, isMoveCursor: false);
                if (Whiteline4 != Whiteline3)
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
                //Step 9::
                new Actions(Driver).Click().Build().Perform();
                Thread.Sleep(4000);
                int afterValue = z3dvp.LevelOfSelectedColor(z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 2, 51, 51, 50, 3);
                if (afterValue != Beforevalue)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step:10
                Thread.Sleep(4000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolPolygondialog, "Undo Sculpt");
                PageLoadWait.WaitForFrameLoad(10);
                if (CompareImage(result.steps[ExecutedSteps], ViewerPane))
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
                //Step:11
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolPolygondialog, "Redo Sculpt");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int afterValue_step11 = z3dvp.LevelOfSelectedColor(ViewerPane, testid, ExecutedSteps + 1, 51, 51, 50, 3);
                if (CompareImage(result.steps[ExecutedSteps], ViewerPane))
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

                //Step:12
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolPolygondialog, "Close");
                z3dvp.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.UndoSegmentation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int afterValue_step12 = z3dvp.LevelOfSelectedColor(ViewerPane, testid, ExecutedSteps + 1, 51, 51, 50, 3);
                if (!afterValue_step12.Equals(afterValue_step11))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step:13
                z3dvp.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.RedoSegmentation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int afterValue_step13 = z3dvp.LevelOfSelectedColor(ViewerPane, testid, ExecutedSteps + 1, 51, 51, 50, 3);
                if (!afterValue_step13.Equals(afterValue_step12))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step:14:: Click the sculpting tool button on the floating toolbar and select the freehand sculpting tool from the drop down menu.
                bool Sculptfreehand = z3dvp.select3DTools(Z3DTools.Sculpt_Tool_for_3D_1_Freehand);
                PageLoadWait.WaitForFrameLoad(10);
                dialogsList = z3dvp.ToolBarDialogs();
                PageLoadWait.WaitForFrameLoad(10);
                //bool step15_1 = z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolFreehanddialog, "Close");
                if (Sculptfreehand && dialogsList[0].Text.Equals(BluRingZ3DViewerPage.SculptToolFreehanddialog))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step: 15::Click and hold the left mouse button on 3D image 2, then move the mouse.
                IWebElement ViewerPane1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int FreeHandWhitelineBefore = z3dvp.LevelOfSelectedColor(z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2), testid, ExecutedSteps + 1, 255, 255, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(ViewerPane1, ViewerPane1.Size.Width / 8, ViewerPane1.Size.Height / 8).ClickAndHold().
                       MoveToElement(ViewerPane1, ViewerPane1.Size.Width - 40, ViewerPane1.Size.Height - 40).
                       MoveToElement(ViewerPane1, ViewerPane1.Size.Width / 4 - 30, ViewerPane1.Size.Height - 40).Build().Perform();
                int FreeHandWhitelineAfter = z3dvp.LevelOfSelectedColor(z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2), testid, ExecutedSteps + 2, 255, 255, 255, 2);
                if (FreeHandWhitelineBefore < FreeHandWhitelineAfter)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                   
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step:16::Release the left mouse button.
                ViewerPane1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                result.steps[ExecutedSteps++].SetPath(testid, ExecutedSteps + 1, 1);
                int BeforeValue_step16 = z3dvp.LevelOfSelectedColor(ViewerPane1, testid, ExecutedSteps + 1, 51, 51, 50, 3);
                new Actions(Driver).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                int AfterValue_step16 = z3dvp.LevelOfSelectedColor(ViewerPane1, testid, ExecutedSteps + 3, 51, 51, 50, 3);
                if (BeforeValue_step16 < AfterValue_step16)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("After Value :"+ AfterValue_step16);
                    Logger.Instance.InfoLog("Before Value :" + BeforeValue_step16);
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step:17::Click the "Undo Sculpt" button on the Polygon Sculpting Tool dialog.
                ViewerPane1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int BeforeValue_step17 = z3dvp.LevelOfSelectedColor(ViewerPane1, testid, ExecutedSteps + 1, 51, 51, 50, 3);
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolFreehanddialog, "Undo Sculpt");
                PageLoadWait.WaitForFrameLoad(10);
                int AfterValue_step17 = z3dvp.LevelOfSelectedColor(ViewerPane1, testid, ExecutedSteps + 2, 51, 51, 50, 3);
                if (!AfterValue_step17.Equals(BeforeValue_step17))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                   
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:18::Click the "Redo Sculpt" button on the Polygon Sculpting Tool dialog.
                ViewerPane1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int BeforeValue_step18 = z3dvp.LevelOfSelectedColor(ViewerPane1, testid, ExecutedSteps + 1, 51, 51, 50, 3);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolFreehanddialog, "Redo Sculpt");
                int AfterValue_step18 = z3dvp.LevelOfSelectedColor(ViewerPane1, testid, ExecutedSteps + 2, 51, 51, 50, 3);
                if (!BeforeValue_step18.Equals(AfterValue_step18))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                     result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step:19::Click the undo button on the toolbar.
                ViewerPane1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolFreehanddialog, "Close");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int BeforeValue_step19 = z3dvp.LevelOfSelectedColor(ViewerPane1, testid, ExecutedSteps + 1, 51, 51, 50, 3);
                z3dvp.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigation3D2, BluRingZ3DViewerPage.UndoSegmentation);
                PageLoadWait.WaitForFrameLoad(10);
                int AfterValue_step19 = z3dvp.LevelOfSelectedColor(ViewerPane1, testid, ExecutedSteps + 2, 51, 51, 50, 3);
                if (!AfterValue_step19.Equals(BeforeValue_step19))
                {

                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step:20::Click the redo button on the toolbar.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int BeforeValue_step20 = z3dvp.LevelOfSelectedColor(ViewerPane1, testid, ExecutedSteps + 2, 51, 51, 50, 3);
                z3dvp.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigation3D2, BluRingZ3DViewerPage.RedoSegmentation);
                PageLoadWait.WaitForFrameLoad(10);
                int AfterValue_step20 = z3dvp.LevelOfSelectedColor(ViewerPane1, testid, ExecutedSteps + 1, 51, 51, 50, 3);
                if (!AfterValue_step20.Equals(BeforeValue_step20))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step:21::Click the 3D 4-up view button from the toolbar.
                bool Three4x1 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                if (Three4x1)
                {
                    result.steps[ExecutedSteps++].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps++].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step:22::Repeat steps 6-12 on 3D1 control.
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolFreehanddialog, "Close");
                Thread.Sleep(2000);
                z3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                bool Sculptpolygon = z3dvp.select3DTools(Z3DTools.Sculpt_Tool_for_3D_1_Polygon , BluRingZ3DViewerPage.Navigation3D1);
                ViewerPane = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                //Step 6::
                int Whiteline6_1 = z3dvp.LevelOfSelectedColor(z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 1, 255, 255, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(ViewerPane, ViewerPane.Size.Width / 2 - 20, ViewerPane.Size.Height / 2 + 50).Click().
                     MoveToElement(ViewerPane, ViewerPane.Size.Width / 2 + 80, ViewerPane.Size.Height / 2 + 50).Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(5);
                //Step 7::
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).Click().MoveToElement(ViewerPane, ViewerPane.Size.Width / 2 - 20, ViewerPane.Size.Height / 2 - 50).Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int Whiteline7_1 = z3dvp.LevelOfSelectedColor(z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 2, 255, 255, 255, 2, isMoveCursor: false);
                //Step 8::
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).Click().MoveToElement(ViewerPane, ViewerPane.Size.Width / 2 - 20, ViewerPane.Size.Height / 2 + 50).Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(5);
                int Whiteline8_1 = z3dvp.LevelOfSelectedColor(z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 3, 255, 255, 255, 2, isMoveCursor: false);
                //Step 9::
                new Actions(Driver).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                int afterValue9_1 = z3dvp.LevelOfSelectedColor(z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 4, 51, 51, 50, 3);
                //Step 10::
                Thread.Sleep(4000);
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolPolygondialog, "Undo Sculpt");
                PageLoadWait.WaitForFrameLoad(10);
                int afterValue_step10_1 = z3dvp.LevelOfSelectedColor(ViewerPane, testid, ExecutedSteps + 6, 51, 51, 50, 3);
                //Step 11::
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolPolygondialog, "Redo Sculpt");
                PageLoadWait.WaitForFrameLoad(10);
                int afterValue_step11_1 = z3dvp.LevelOfSelectedColor(ViewerPane, testid, ExecutedSteps + 7, 51, 51, 50, 3);
                //Step 12::
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolPolygondialog, "Close");
                z3dvp.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.UndoSegmentation);
                int afterValue_step12_1 = z3dvp.LevelOfSelectedColor(ViewerPane, testid, ExecutedSteps + 8, 51, 51, 50, 3);
                if (Sculptpolygon && Whiteline6_1 < Whiteline7_1 && Whiteline7_1< Whiteline8_1 && afterValue9_1> afterValue_step10_1
                    && afterValue_step10_1< afterValue_step11_1 && afterValue_step12_1 != afterValue_step11_1)
                {
                    result.steps[ExecutedSteps++].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps++].status = "Fail";
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


        public TestCaseResult Test_163237(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string TestReq = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            //string[] TestData = TestReq.Split('|');
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //step:1 - Login Ica viewer
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
                //Step:2 The Series should be loaded with out any errors and MPR 4:1 viewing mode should be displayed by default..
                Boolean step2 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR);
                if (step2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    throw new Exception("Unable to load study successfully");
                }

                //Step:3 
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

                //Step:4
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

                //Step:5::Select scroll tool from floating tool box .
                bool ScrollTool = z3dvp.select3DTools(Z3DTools.Scrolling_Tool, BluRingZ3DViewerPage.Navigation3D1);
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

                //Step:6:: Click on the 3D control 1 and scroll the mouse wheel downwards.
                String LocValueBefore_3d1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                String LocValueBefore_3d2 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                z3dvp.ScrollInView(BluRingZ3DViewerPage.Navigation3D1, ScrollDirection: "down", scrolllevel: 15, Thickness: "N");
                Thread.Sleep(3000);
                String LocValueAfter_3d1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                String LocValueAfter_3d2 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                if (!LocValueBefore_3d1.Equals(LocValueAfter_3d1) && !LocValueBefore_3d2.Equals(LocValueAfter_3d2) && LocValueAfter_3d1.Equals(LocValueAfter_3d2))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step:7
                IWebElement Nav3D = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                LocValueBefore_3d1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                LocValueBefore_3d2 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                z3dvp.Performdragdrop(Nav3D, Nav3D.Size.Width / 4, Nav3D.Size.Height / 2, Nav3D.Size.Width / 4, Nav3D.Size.Height / 4);
                Thread.Sleep(3000);
                LocValueAfter_3d1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                LocValueAfter_3d2 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                if (!LocValueBefore_3d1.Equals(LocValueAfter_3d1) && !LocValueBefore_3d2.Equals(LocValueAfter_3d2) && LocValueAfter_3d1.Equals(LocValueAfter_3d2))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step:8
                bool step8 = z3dvp.checkerrormsg();
                if (!step8)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:9
                String LocValueBefore1_3d1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                bool Reset = z3dvp.select3DTools(Z3DTools.Reset);
                Thread.Sleep(3000);
                String LocValueAfter1_3d1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                if (Reset && !LocValueBefore1_3d1.Equals(LocValueAfter1_3d1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:10
                IWebElement ViewerPort1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                z3dvp.Performdragdrop(ViewerPort1, (ViewerPort1.Size.Width / 2) - 110, (ViewerPort1.Size.Height / 2) - 110, (ViewerPort1.Size.Width / 2) + 110, (ViewerPort1.Size.Height / 2) + 110);
                Thread.Sleep(5000);
                z3dvp.Performdragdrop(ViewerPort1, (ViewerPort1.Size.Width / 2) - 110, (ViewerPort1.Size.Height / 2) - 110, (ViewerPort1.Size.Width / 2) + 110, (ViewerPort1.Size.Height / 2) + 110);
                Thread.Sleep(5000);
                z3dvp.Performdragdrop(ViewerPort1, (ViewerPort1.Size.Width / 2) - 110, (ViewerPort1.Size.Height / 2) - 110, (ViewerPort1.Size.Width / 2) + 110, (ViewerPort1.Size.Height / 2) + 110);
                bool step10 = z3dvp.checkerrormsg();
                if (!step10)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:11
                String LocValueBefore2_3d1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                z3dvp.select3DTools(Z3DTools.Reset);
                Thread.Sleep(3000);
                String LocValueAfter2_3d1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                if (!LocValueBefore2_3d1.Equals(LocValueAfter2_3d1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:12
                //Repeat step 7 - 11
                z3dvp.ScrollInView(BluRingZ3DViewerPage.Navigation3D1, ScrollDirection: "down", scrolllevel: 15, Thickness: "N");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step7_1 = z3dvp.checkerrormsg();
                if (!step7_1)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    throw new Exception("Step 12 repeated steps Failed");
                }
                String LocValueBefore3_3d2 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                z3dvp.select3DTools(Z3DTools.Reset);
                Thread.Sleep(3000);
                String LocValueAfter3_3d2 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                if (!LocValueBefore3_3d2.Equals(LocValueAfter3_3d2))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    throw new Exception("Step 12_2 repeated steps Failed");
                }

                IWebElement ViewerPort2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                z3dvp.Performdragdrop(ViewerPort2, (ViewerPort2.Size.Width / 2) - 110, (ViewerPort2.Size.Height / 2) - 110, (ViewerPort2.Size.Width / 2) + 110, (ViewerPort2.Size.Height / 2) + 110);
                Thread.Sleep(5000);
                z3dvp.Performdragdrop(ViewerPort2, (ViewerPort2.Size.Width / 2) - 110, (ViewerPort2.Size.Height / 2) - 110, (ViewerPort2.Size.Width / 2) + 110, (ViewerPort2.Size.Height / 2) + 110);
                Thread.Sleep(5000);
                z3dvp.Performdragdrop(ViewerPort2, (ViewerPort2.Size.Width / 2) - 110, (ViewerPort2.Size.Height / 2) - 110, (ViewerPort2.Size.Width / 2) + 110, (ViewerPort2.Size.Height / 2) + 110);
                bool step10_2 = z3dvp.checkerrormsg();
                if (!step10_2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    throw new Exception("Step 12_3 repeated steps Failed");
                }

                String LocValueBefore4_3d2 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                z3dvp.select3DTools(Z3DTools.Reset);
                Thread.Sleep(3000);
                String LocValueAfter4_3d2 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                if (!LocValueBefore4_3d2.Equals(LocValueAfter4_3d2))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    throw new Exception("Step 12_4 repeated steps Failed");
                }

                //step:13
                bool step13 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4, BluRingZ3DViewerPage.Navigation3D1);
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

                //step:14
                //Repeat step 7 - 11
                z3dvp.ScrollInView(BluRingZ3DViewerPage.Navigation3D1, ScrollDirection: "down", scrolllevel: 15, Thickness: "N");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step7_2 = z3dvp.checkerrormsg();
                if (!step7_2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    throw new Exception("Step 12 repeated steps Failed");
                }
                String LocValueBefore5_3d1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                z3dvp.select3DTools(Z3DTools.Reset);
                Thread.Sleep(3000);
                String LocValueAfter5_3d1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                if (!LocValueBefore5_3d1.Equals(LocValueAfter5_3d1))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    throw new Exception("Step 12_2 repeated steps Failed");
                }

                IWebElement ViewerPort3 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                z3dvp.Performdragdrop(ViewerPort3, (ViewerPort3.Size.Width / 2) - 110, (ViewerPort3.Size.Height / 2) - 110, (ViewerPort3.Size.Width / 2) + 110, (ViewerPort3.Size.Height / 2) + 110);
                Thread.Sleep(5000);
                z3dvp.Performdragdrop(ViewerPort3, (ViewerPort3.Size.Width / 2) - 110, (ViewerPort3.Size.Height / 2) - 110, (ViewerPort3.Size.Width / 2) + 110, (ViewerPort3.Size.Height / 2) + 110);
                Thread.Sleep(5000);
                z3dvp.Performdragdrop(ViewerPort3, (ViewerPort3.Size.Width / 2) - 110, (ViewerPort3.Size.Height / 2) - 110, (ViewerPort3.Size.Width / 2) + 110, (ViewerPort3.Size.Height / 2) + 110);
                bool step10_3 = z3dvp.checkerrormsg();
                if (!step10_3)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    throw new Exception("Step 12_3 repeated steps Failed");
                }

                String LocValueBefore6_3d1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                z3dvp.select3DTools(Z3DTools.Reset);
                Thread.Sleep(3000);
                String LocValueAfter6_3d1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                if (!LocValueBefore6_3d1.Equals(LocValueAfter6_3d1))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    throw new Exception("Step 12_4 repeated steps Failed");
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

        public TestCaseResult Test_163235(string testid, String teststeps, int stepcount)
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
                //step:1 - Login Ica viewer
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                result.steps[++ExecutedSteps].StepPass();

                //Step2 & 3: The Series should be loaded with out any errors and MPR 4:1 viewing mode should be displayed by default..
                bool step2 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.Three_3d_6);
                if (step2)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                    throw new Exception("Failed to launch study in 3D 6:1 Layout in Test_163235 Step 2 & 3");

                //Step:4 
                bool step4 = z3dvp.ChangeViewMode(BluRingZ3DViewerPage.Navigation3D1);
                if (step4)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:5
                bool step5 = z3dvp.select3DTools(Z3DTools.Line_Measurement, BluRingZ3DViewerPage.Navigation3D1);
                if (step5)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:6
                String Step5_AnnotationValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                IWebElement ViewerPane = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                int yellowcolorbefore_6 = z3dvp.LevelOfSelectedColor(ViewerPane, testid, ExecutedSteps + 61, 255, 255, 0, 2);
                new Actions(Driver).MoveToElement(ViewerPane, (ViewerPane.Size.Width / 2) - 30, (ViewerPane.Size.Height / 2) - 50).ClickAndHold()
                    .MoveToElement(ViewerPane, (ViewerPane.Size.Width / 2) - 30, (ViewerPane.Size.Height / 2) + 20).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int yellowcolorafter_6 = z3dvp.LevelOfSelectedColor(ViewerPane, testid, ExecutedSteps + 62, 255, 255, 0, 2);
                if (yellowcolorafter_6 != yellowcolorbefore_6)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:7
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool res = z3dvp.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.Navigation3D1);
                if (res)
                {
                    new Actions(Driver).MoveToElement(ViewerPane, (ViewerPane.Size.Width / 4), 3 * (ViewerPane.Size.Height / 4)).ClickAndHold()
                    .MoveToElement(ViewerPane, (ViewerPane.Size.Width / 4), (ViewerPane.Size.Height / 4)).Release().Build().Perform();
                    if (CompareImage(result.steps[ExecutedSteps], ViewerPane, ImageFormat: "png"))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step:8
                IWebElement ViewerPane2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                res = z3dvp.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.Navigation3D2);
                if (res)
                {
                    new Actions(Driver).MoveToElement(ViewerPane2, (ViewerPane2.Size.Width / 4), (ViewerPane2.Size.Height / 4)).ClickAndHold()
                    .MoveToElement(ViewerPane2, (ViewerPane2.Size.Width / 4), 3 * (ViewerPane2.Size.Height / 4)).Release().Build().Perform();
                    if (CompareImage(result.steps[ExecutedSteps], ViewerPane2, ImageFormat: "png"))
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step:9
                ViewerPane = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                z3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(15);
                int yellowcolorafter_9 = z3dvp.LevelOfSelectedColor(ViewerPane, testid, ExecutedSteps + 91, 255, 255, 0, 2);
                if (yellowcolorafter_9 != yellowcolorafter_6)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:10
                ViewerPane = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                res = z3dvp.select3DTools(Z3DTools.Line_Measurement, BluRingZ3DViewerPage.Navigation3D1);
                if (res)
                {
                    PageLoadWait.WaitForFrameLoad(10);
                    int yellowcolorBefore_10 = z3dvp.LevelOfSelectedColor(ViewerPane, testid, ExecutedSteps + 3, 255, 255, 0, 2);
                    z3dvp.Performdragdrop(ViewerPane, ViewerPane.Size.Width / 2 - 20, ViewerPane.Size.Height / 2 - 25, ViewerPane.Size.Width / 2 - 30, ViewerPane.Size.Height / 2 + 30);
                    PageLoadWait.WaitForFrameLoad(10);
                    z3dvp.Performdragdrop(ViewerPane, ViewerPane.Size.Width / 2 - 40, ViewerPane.Size.Height / 2 - 25, ViewerPane.Size.Width / 2 - 45, ViewerPane.Size.Height / 2 + 50);
                    PageLoadWait.WaitForFrameLoad(10);
                    z3dvp.Performdragdrop(ViewerPane, ViewerPane.Size.Width / 2 + 20, ViewerPane.Size.Height / 2 - 45, ViewerPane.Size.Width / 2 - 50, ViewerPane.Size.Height / 2 + 60);
                    int yellowcolorAfter_10 = z3dvp.LevelOfSelectedColor(ViewerPane, testid, ExecutedSteps + 3, 255, 255, 0, 2);
                    if (yellowcolorBefore_10 < yellowcolorAfter_10)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:11
                ViewerPane = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                ViewerPane2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                int yellowcolorBefore_11 = z3dvp.LevelOfSelectedColor(ViewerPane, testid, ExecutedSteps + 3, 255, 255, 0, 2);
                Accord.Point rotatepoints = z3dvp.GetIntersectionPoints(ViewerPane2, testid, ExecutedSteps + 111, "yellow", "vertical", 0);
                PageLoadWait.WaitForFrameLoad(5);
                //new Actions(Driver).MoveToElement(ViewerPane, (Int32)rotatepoints.X, (Int32)rotatepoints.Y).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(ViewerPane, (Int32)rotatepoints.X, (Int32)rotatepoints.Y).Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int yellowcolorAfter_11 = z3dvp.LevelOfSelectedColor(ViewerPane, testid, ExecutedSteps + 4, 255, 255, 0, 2);
                if (yellowcolorAfter_11 > yellowcolorBefore_11)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:12
                z3dvp.Performdragdrop(ViewerPane, ViewerPane.Size.Width - 5, (Int32)rotatepoints.Y, (Int32)rotatepoints.X, (Int32)rotatepoints.Y);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerPane, ImageFormat: "png"))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step:13
                z3dvp.select3DTools(Z3DTools.Reset);
                String AnnotationValue_Step13 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                int yellowcolorafter_13 = z3dvp.LevelOfSelectedColor(ViewerPane, testid, ExecutedSteps + 91, 255, 255, 0, 2);
                if (yellowcolorafter_13 < yellowcolorAfter_11)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:14
                z3dvp.select3DTools(Z3DTools.Line_Measurement);
                int yellowcolorbefore1 = z3dvp.LevelOfSelectedColor(ViewerPane, testid, ExecutedSteps + 141, 255, 255, 0, 2);
                z3dvp.Performdragdrop(ViewerPane, ViewerPane.Size.Width / 2 - 40, ViewerPane.Size.Height / 2 - 25, ViewerPane.Size.Width / 2 - 45, ViewerPane.Size.Height / 2 + 50);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.Performdragdrop(ViewerPane, ViewerPane.Size.Width / 2 + 20, ViewerPane.Size.Height / 2 - 45, ViewerPane.Size.Width / 2 - 50, ViewerPane.Size.Height / 2 + 60);
                PageLoadWait.WaitForFrameLoad(10);
                int yellowcolorAfter1 = z3dvp.LevelOfSelectedColor(ViewerPane, testid, ExecutedSteps + 142, 255, 255, 0, 2); 
                if (yellowcolorbefore1 < yellowcolorAfter1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:15
                new Actions(Driver).MoveToElement(ViewerPane, ViewerPane.Size.Width / 2 - 2, ViewerPane.Size.Height / 2).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int yellowcolorAfter2 = z3dvp.LevelOfSelectedColor(ViewerPane, testid, ExecutedSteps + 15, 255, 255, 0, 2);
                if (yellowcolorAfter1 < yellowcolorAfter2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:16
                new Actions(Driver).MoveToElement(ViewerPane, ViewerPane.Size.Width / 2 - 2, ViewerPane.Size.Height / 2).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                System.Windows.Forms.SendKeys.SendWait("{DEL}");
                //TestStack.White.InputDevices.Keyboard.Instance.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.DELETE);
                PageLoadWait.WaitForFrameLoad(10);
                int yellowcolorAfter3 = z3dvp.LevelOfSelectedColor(ViewerPane, testid, ExecutedSteps + 16, 255, 255, 0, 2);
                PageLoadWait.WaitForFrameLoad(5);
                if (yellowcolorAfter3 < yellowcolorAfter2)
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
                z3dvp.CloseViewer();
                login.Logout();
            }

        }
   
        public TestCaseResult Test_163241(string testid, String teststeps, int stepcount)
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
                //step:1 - Login Ica viewer
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
                //Step2 & 3: The Series should be loaded with out any errors and MPR 4:1 viewing mode should be displayed by default..
                bool step3 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR);
                if (step3)
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
                //Step:4::From smart view drop down select 3D 6:1 view mode.
                bool step4 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
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
                //Step:5::Hover top of the viewport 3D 1 control and select toggle 3D/MPR in order to display the 3D navigation controls.
                bool step5 = z3dvp.ChangeViewMode(BluRingZ3DViewerPage.Navigation3D1);
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
                //Step:6::From 3D 1 control and select "Bone & Minimal Vessels" preset from the preset drop down list.
                IWebElement ViewerPane = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Preset2, "Preset");
                bool step6 = z3dvp.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Preset2, "Preset");
                if (step6)
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
                //Step:7::Click the Tissue Selection Tool button on the toolbar.
                z3dvp.select3DTools(Z3DTools.Selection_Tool);
                IList<IWebElement> dialogsList = z3dvp.ToolBarDialogs();
                PageLoadWait.WaitForFrameLoad(10);
                //bool Step7 = z3dvp.Handle3dToolsDialogs("3D Tissue Selection Instructions", "Close");
                if (dialogsList[0].Text.Equals(BluRingZ3DViewerPage.SelectionTooldialog))
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

                //Step:8::Select "Large Vessels" from the Preset drop down list on the Tissue Selection Tool dialog.
                //z3dvp.select3DTools(Z3DTools.Selection_Tool);
                IWebElement ThreshholdValue = Driver.FindElement(By.CssSelector(Locators.CssSelector.ThreshholdProgressBar));
                string BeforeThresholdValue = ThreshholdValue.GetAttribute("aria-valuenow");
                IWebElement radiousvalue = Driver.FindElement(By.CssSelector(Locators.CssSelector.RadiousProgressBar));
                bool Step8 = z3dvp.Handle3dToolsDialogs("3D Tissue Selection Instructions", "Large vessels");
                String AfterThresholdValue = ThreshholdValue.GetAttribute("aria-valuenow");
                string RadiousValue = radiousvalue.GetAttribute("aria-valuenow");
                if (Convert.ToInt32(BeforeThresholdValue) > Convert.ToInt32(AfterThresholdValue) && Convert.ToInt32(RadiousValue) == 2000)
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

                //Step:9:: Click the aorta in the 3D control 1.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int BeforeBlue = z3dvp.LevelOfSelectedColor(z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 1, 0, 0, 168, 2);
                //int BeforeBlue = z3dvp.selectedcolorcheck(result.steps[ExecutedSteps].goldimagepath, 0, 0, 168);
                Thread.Sleep(2000);
                Actions act = new Actions(Driver);
                //z3dvp.MoveAndClick(ViewerPane, ViewerPane.Size.Width / 2 + 15, ViewerPane.Size.Height / 4 - 30);
                new Actions(Driver).MoveToElement(ViewerPane, ViewerPane.Size.Width / 2 + 15, ViewerPane.Size.Height / 4 - 30).Click().Build().Perform();
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int Afterblue = z3dvp.LevelOfSelectedColor(z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 3, 0, 0, 168, 2);
                Thread.Sleep(2000);
                IWebElement TissueSelectionVolume = Driver.FindElement(By.CssSelector(Locators.CssSelector.tissueSelectionVolume));
                string SelectionVolume = TissueSelectionVolume.Text;
                String[] VolumeValue = SelectionVolume.Split(' ');
                if (Convert.ToDouble(VolumeValue[0]) > 0 && VolumeValue[1].Contains("cm3") && Afterblue > BeforeBlue)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.InfoLog("Blue Highlighted:BeforeBlue =" + BeforeBlue);
                    Logger.Instance.InfoLog("Blue Highlighted:AfterBlue =" + Afterblue);
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step:10:: From the Tissue selection window, Adjust the Threshold value by moving the slider and Click on the Apply settings button.
                bool step10 = z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "", BluRingZ3DViewerPage.Threshold, 50);
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Apply);
                Thread.Sleep(3000);
                TissueSelectionVolume = Driver.FindElement(By.CssSelector(Locators.CssSelector.tissueSelectionVolume));
                string AfterSelectVolume = TissueSelectionVolume.Text;
                String[] AftVolumeValue = AfterSelectVolume.Split(' ');
                Logger.Instance.InfoLog("Noted Selection Volume of Step10 :" + AftVolumeValue[0]);
                if (Convert.ToDouble(AftVolumeValue[0]) > Convert.ToDouble(VolumeValue[0]))
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
                //Step:11::From the Tissue selection window, Adjust the Radius value by moving the slider near to 0cm value and Click on the Apply New settings button.
                bool step11 = z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "", BluRingZ3DViewerPage.Radius, 8);
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Apply);
                Thread.Sleep(3000);
                TissueSelectionVolume = Driver.FindElement(By.CssSelector(Locators.CssSelector.tissueSelectionVolume));
                string AfterRadioustVolume = TissueSelectionVolume.Text;
                String[] AftRadVolumeValue = AfterRadioustVolume.Split(' ');
                Logger.Instance.InfoLog("Noted Selection Volume of Step11 :" + AftRadVolumeValue[0]);
                if (Convert.ToDouble(AftVolumeValue[0]) != Convert.ToDouble(AftRadVolumeValue[0]))
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
                //Step:12 ::On the tissue selection tool dialog, click "Delete Selected" button.
                bool step12 = z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.DeleteSelected);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.DeleteSelected);
                z3dvp.Handle3dToolsDialogs("3D Tissue Selection Instructions", "Close");
                IWebElement WholeViewerPanel = z3dvp.ViewerContainer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int Step12Colour = z3dvp.LevelOfSelectedColor(WholeViewerPanel, testid, ExecutedSteps + 11, 0, 0, 255, 2);
                if (Step12Colour == 0)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:13
                z3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Undo");
                z3dvp.Handle3dToolsDialogs("3D Tissue Selection Instructions", "Close");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int Step13Colour = z3dvp.LevelOfSelectedColor(WholeViewerPanel, testid, ExecutedSteps + 1, 0, 0, 168, 2);
                if (Step13Colour == 0)
                {
                    z3dvp.select3DTools(Z3DTools.Selection_Tool);
                    PageLoadWait.WaitForFrameLoad(10);
                    z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Undo");
                    z3dvp.Handle3dToolsDialogs("3D Tissue Selection Instructions", "Close");
                    Step13Colour = z3dvp.LevelOfSelectedColor(WholeViewerPanel, testid, ExecutedSteps + 1, 0, 0, 168, 2);
                }
                if (Step13Colour != 0)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:14
                z3dvp.select3DTools(Z3DTools.Selection_Tool);
                Thread.Sleep(10000);
                TissueSelectionVolume = Driver.FindElement(By.CssSelector(Locators.CssSelector.tissueSelectionVolume));
                string afterUndoVolume = TissueSelectionVolume.Text;
                String[] VolumeValueNow = afterUndoVolume.Split(' ');
                Double diff = (Convert.ToDouble(AftRadVolumeValue[0]) - Convert.ToDouble(VolumeValueNow[0]));
                if (diff >= -5 && diff <= 5)
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

                //Step:15::Click the "Redo " button.
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Redo");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                z3dvp.Handle3dToolsDialogs("3D Tissue Selection Instructions", "Close");
                int Step15Colour = z3dvp.LevelOfSelectedColor(WholeViewerPanel, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                if (Step15Colour == 0)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:16::Click the "Undo " button.
                z3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Undo");
                z3dvp.Handle3dToolsDialogs("3D Tissue Selection Instructions", "Close");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int Step16Colour = z3dvp.LevelOfSelectedColor(WholeViewerPanel, testid, ExecutedSteps + 1, 0, 0, 168, 2);
                if (Step16Colour == 0)
                {
                    z3dvp.select3DTools(Z3DTools.Selection_Tool);
                    PageLoadWait.WaitForFrameLoad(10);
                    z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Undo");
                    z3dvp.Handle3dToolsDialogs("3D Tissue Selection Instructions", "Close");
                    Step16Colour = z3dvp.LevelOfSelectedColor(WholeViewerPanel, testid, ExecutedSteps + 1, 0, 0, 168, 2);
                }
                if (Step16Colour != 0)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:17 :: On the tissue selection dialog note the calculated volume at the bottom.
                z3dvp.select3DTools(Z3DTools.Selection_Tool);
                TissueSelectionVolume = Driver.FindElement(By.CssSelector(Locators.CssSelector.tissueSelectionVolume));
                string UndoVolume = TissueSelectionVolume.Text;
                String[] VolumeValueNow1 = UndoVolume.Split(' ');
                int Step11Value = Convert.ToInt32(AftRadVolumeValue[0].ToString().Substring(0, 1));
                int CurrentValue = Convert.ToInt32(VolumeValueNow1[0].ToString().Substring(0, 1));
                int diff17 = Step11Value - CurrentValue;
                if (diff17 >= -5 && diff17 <= 5)
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

                //Step:18 :: On the tissue selection tool dialog, click the "Delete Unselected" button.
                bool step18 = z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.DeleteUnselected);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.DeleteUnselected);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                z3dvp.Handle3dToolsDialogs("3D Tissue Selection Instructions", "Close");
                int Step18Colour = z3dvp.LevelOfSelectedColor(WholeViewerPanel, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                if (Step18Colour == 0)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:19::Click the Undo button on the toolbar.
                z3dvp.select3DTools(Z3DTools.Undo_Segmentation, BluRingZ3DViewerPage.Navigation3D1);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int Step19Colour = z3dvp.LevelOfSelectedColor(WholeViewerPanel, testid, ExecutedSteps + 1, 0, 0, 168, 2);
                if (Step19Colour == 0)
                {
                    z3dvp.select3DTools(Z3DTools.Undo_Segmentation, BluRingZ3DViewerPage.Navigation3D1);
                    PageLoadWait.WaitForFrameLoad(10);
                    Step19Colour = z3dvp.LevelOfSelectedColor(WholeViewerPanel, testid, ExecutedSteps + 1, 0, 0, 168, 2);
                }
                if (Step19Colour != 0)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:20::On the tissue selection dialog note the calculated volume at the bottom.
                z3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(10);
                TissueSelectionVolume = Driver.FindElement(By.CssSelector(Locators.CssSelector.tissueSelectionVolume));
                string UndoVolume_Step20 = TissueSelectionVolume.Text;
                String[] VolumeValueNow_20 = UndoVolume_Step20.Split(' ');
                int Step11Value1 = Convert.ToInt32(AftRadVolumeValue[0].ToString().Substring(0, 1));
                int CurrentValue1 = Convert.ToInt32(VolumeValueNow_20[0].ToString().Substring(0, 1));
                int diff20 = CurrentValue1 - Step11Value1;
                if (diff20 <= 5 && diff20 >= -5)
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

                //Step:21 :: Click the Redo button on the toolbar.
                z3dvp.select3DTools(Z3DTools.Redo_Segmentation, BluRingZ3DViewerPage.Navigation3D1);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                z3dvp.Handle3dToolsDialogs("3D Tissue Selection Instructions", "Close");
                int Step21Colour = z3dvp.LevelOfSelectedColor(WholeViewerPanel, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                if (Step21Colour == 0)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:22::On the tissue selection tool dialog, click the "Undo Selection" button.
                z3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(10);
                bool step22 = z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Undo");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                z3dvp.Handle3dToolsDialogs("3D Tissue Selection Instructions", "Close");
                int Step22Colour = z3dvp.LevelOfSelectedColor(WholeViewerPanel, testid, ExecutedSteps + 1, 0, 0, 168, 2);
                if (Step22Colour == 0)
                {
                    z3dvp.select3DTools(Z3DTools.Selection_Tool);
                    PageLoadWait.WaitForFrameLoad(10);
                    z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Undo");
                    z3dvp.Handle3dToolsDialogs("3D Tissue Selection Instructions", "Close");
                    Step22Colour = z3dvp.LevelOfSelectedColor(WholeViewerPanel, testid, ExecutedSteps + 1, 0, 0, 168, 2);
                }
                if (Step22Colour != 0 && step22)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step:23 ::On the tissue selection dialog note the calculated volume at the bottom.
                z3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(10);
                TissueSelectionVolume = Driver.FindElement(By.CssSelector(Locators.CssSelector.tissueSelectionVolume));
                string UndoVolume_Step23 = TissueSelectionVolume.Text;
                String[] VolumeValueNow_23 = UndoVolume_Step23.Split(' ');
                Step11Value1 = Convert.ToInt32(AftRadVolumeValue[0].ToString().Substring(0, 1));
                CurrentValue1 = Convert.ToInt32(VolumeValueNow_23[0].ToString().Substring(0, 1));
                int diff23 = Step11Value1 - CurrentValue1;
                if (diff23 <= 5 && diff23 >= -5)
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

                //Step:24 :: Click the Reset button from the floating toolbar.
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Close");
                z3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.select3DTools(Z3DTools.Reset);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int Step24Colour = z3dvp.LevelOfSelectedColor(WholeViewerPanel, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                if (Step24Colour == 0)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:25 :: From 3D 2 control and select "Bone & Minimal Vessels" preset from the preset drop down list.
                IWebElement Navigation3D2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigation3D2, BluRingZ3DViewerPage.Preset2, "Preset");
                bool step25 = z3dvp.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigation3D2, BluRingZ3DViewerPage.Preset2, "Preset");
                if (step25)
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

                //Step:26 :: Click the aorta in the 3D control 2.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                z3dvp.select3DTools(Z3DTools.Selection_Tool);
                BeforeBlue = z3dvp.LevelOfSelectedColor(z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2), testid, ExecutedSteps + 2, 0, 0, 168, 2);
                //int BeforeBlue = z3dvp.selectedcolorcheck(result.steps[ExecutedSteps].goldimagepath, 0, 0, 168);
                Thread.Sleep(2000);
                new Actions(Driver).MoveToElement(Navigation3D2, Navigation3D2.Size.Width / 2 + 15, Navigation3D2.Size.Height / 4 - 30).Click().Build().Perform();
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                Afterblue = z3dvp.LevelOfSelectedColor(z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2), testid, ExecutedSteps + 4, 0, 0, 168, 2);
                Thread.Sleep(2000);
                TissueSelectionVolume = Driver.FindElement(By.CssSelector(Locators.CssSelector.tissueSelectionVolume));
                SelectionVolume = TissueSelectionVolume.Text;
                VolumeValue = SelectionVolume.Split(' ');
                z3dvp.Handle3dToolsDialogs("3D Tissue Selection Instructions", "Close");
                if (Convert.ToDouble(VolumeValue[0]) > 0 && VolumeValue[1].Contains("cm3") && Afterblue > BeforeBlue)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.InfoLog("Blue Highlighted:BeforeBlue =" + BeforeBlue);
                    Logger.Instance.InfoLog("Blue Highlighted:AfterBlue =" + Afterblue);
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:27 :: Click the "Undo " button.
                z3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Undo");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                z3dvp.Handle3dToolsDialogs("3D Tissue Selection Instructions", "Close");
                int Step27Colour = z3dvp.LevelOfSelectedColor(WholeViewerPanel, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                if (Step27Colour == 0)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 28 :: Click the "redo" button.
                z3dvp.select3DTools(Z3DTools.Selection_Tool);
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Redo");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                z3dvp.Handle3dToolsDialogs("3D Tissue Selection Instructions", "Close");
                int Step28Colour = z3dvp.LevelOfSelectedColor(WholeViewerPanel, testid, ExecutedSteps + 1, 0, 0, 168, 2);
                if (Step28Colour != 0)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 29 :: Click the Reset button from the floating toolbar.
                z3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.select3DTools(Z3DTools.Reset);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int Step29Colour = z3dvp.LevelOfSelectedColor(WholeViewerPanel, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                if (Step29Colour == 0)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 30 :: From smart view drop down select 3D 4:1 view mode.
                bool ThreeD4x1 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                if (ThreeD4x1)
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
                //Step 31 :: Click the aorta in the 3D control .
                z3dvp.Handle3dToolsDialogs("3D Tissue Selection Instructions", "Close");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                IWebElement Navigation3D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                BeforeBlue = z3dvp.LevelOfSelectedColor(WholeViewerPanel, testid, ExecutedSteps + 22, 0, 0, 168, 2);
                //int BeforeBlue = z3dvp.selectedcolorcheck(result.steps[ExecutedSteps].goldimagepath, 0, 0, 168);
                Thread.Sleep(2000);
                new Actions(Driver).MoveToElement(Navigation3D1, Navigation3D1.Size.Width / 2 + 60, (Navigation3D1.Size.Height) * 3 / 4).Click().Build().Perform();
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                z3dvp.select3DTools(Z3DTools.Selection_Tool);
                Afterblue = z3dvp.LevelOfSelectedColor(WholeViewerPanel, testid, ExecutedSteps + 3, 0, 0, 168, 2);
                Thread.Sleep(2000);
                TissueSelectionVolume = Driver.FindElement(By.CssSelector(Locators.CssSelector.tissueSelectionVolume));
                SelectionVolume = TissueSelectionVolume.Text;
                VolumeValue = SelectionVolume.Split(' ');
                if (Convert.ToDouble(VolumeValue[0]) > 0 && VolumeValue[1].Contains("cm3") && Afterblue != BeforeBlue)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.InfoLog("Blue Highlighted:BeforeBlue =" + BeforeBlue);
                    Logger.Instance.InfoLog("Blue Highlighted:AfterBlue =" + Afterblue);
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 32 ::On the tissue selection tool dialog, click "Delete Selected" button.
                bool step32 = z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.DeleteSelected);
                z3dvp.Handle3dToolsDialogs("3D Tissue Selection Instructions", "Close");
                WholeViewerPanel = z3dvp.ViewerContainer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int Step32Colour = z3dvp.LevelOfSelectedColor(WholeViewerPanel, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                if (Step32Colour == 0)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 33 :: Click the "Undo " button.
                z3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Undo");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                z3dvp.Handle3dToolsDialogs("3D Tissue Selection Instructions", "Close");
                int Step33Colour = z3dvp.LevelOfSelectedColor(WholeViewerPanel, testid, ExecutedSteps + 1, 0, 0, 168, 2);
                if (Step33Colour != 0)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 34 :: On the tissue selection dialog note the calculated volume at the bottom.
                z3dvp.select3DTools(Z3DTools.Selection_Tool);
                TissueSelectionVolume = Driver.FindElement(By.CssSelector(Locators.CssSelector.tissueSelectionVolume));
                SelectionVolume = TissueSelectionVolume.Text;
                String[] VolumeValue34 = SelectionVolume.Split(' ');
                z3dvp.Handle3dToolsDialogs("3D Tissue Selection Instructions", "Close");
                if (Convert.ToDouble(VolumeValue[0]) == Convert.ToDouble(VolumeValue34[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.InfoLog("Blue Highlighted:BeforeBlue =" + BeforeBlue);
                    Logger.Instance.InfoLog("Blue Highlighted:AfterBlue =" + Afterblue);
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 35 :: Click the Reset button from the floating toolbar.
                z3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.select3DTools(Z3DTools.Reset);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int Step35Colour = z3dvp.LevelOfSelectedColor(WholeViewerPanel, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                if (Step35Colour == 0)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
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

        public bool Test163241_repeatedSteps_1(string testid, int stepcount, string Patientid, string ImageCount, int ExecutedSteps)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            bool Verification1, Verification2, Verification3, Verification4, Verification5, Verification6, Verification7, Verification8, Verification9, Verification10, Verification11, Verification12, Verification13;
            login.Logout();
            login.LoginIConnect(Config.adminUserName, Config.adminPassword);
            z3dvp.searchandopenstudyin3D(Patientid, ImageCount);
            z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
            z3dvp.ChangeViewMode(BluRingZ3DViewerPage.Navigation3D1);

            IWebElement ViewerPane = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
            z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigation3D2, BluRingZ3DViewerPage.Preset2, "Preset");
            result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
            if (CompareImage(result.steps[ExecutedSteps], ViewerPane))
            {
                result.steps[ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
            }
            else
            {
                
                result.steps[ExecutedSteps].status = "Fail";
                Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                result.steps[ExecutedSteps].SetLogs();
            }

            z3dvp.select3DTools(Z3DTools.Selection_Tool);
            bool Step7 = z3dvp.Handle3dToolsDialogs("3D Tissue Selection Instructions", "Close");

            z3dvp.select3DTools(Z3DTools.Selection_Tool);
            IWebElement ThreshholdValue = Driver.FindElement(By.CssSelector(Locators.CssSelector.ThreshholdProgressBar));
            string BeforeThresholdValue = ThreshholdValue.GetAttribute("aria-valuenow");
            IWebElement radiousvalue = Driver.FindElement(By.CssSelector(Locators.CssSelector.RadiousProgressBar));
            bool Step8 = z3dvp.Handle3dToolsDialogs("3D Tissue Selection Instructions", "Large vessels");
            String AfterThresholdValue = ThreshholdValue.GetAttribute("aria-valuenow");
            string RadiousValue = radiousvalue.GetAttribute("aria-valuenow");
            if (Convert.ToInt32(BeforeThresholdValue) > Convert.ToInt32(AfterThresholdValue) && Convert.ToInt32(RadiousValue) == 2000)
            {
                Verification1 = true;
                Logger.Instance.InfoLog("Verification1 result is PASS");
            }
            else
            {
                Verification1 = false;
                Logger.Instance.InfoLog("BeforeThresholdValue :"+ BeforeThresholdValue);
                Logger.Instance.InfoLog("AfterThresholdValue :" + AfterThresholdValue);
                Logger.Instance.InfoLog("RadiousValue :" + RadiousValue);
                Logger.Instance.InfoLog("Verification1 result is FAIL");
            }
            result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
            DownloadImageFile(ViewerPane, result.steps[ExecutedSteps].goldimagepath);
            int BeforeBlue = z3dvp.selectedcolorcheck(result.steps[ExecutedSteps].goldimagepath, 0, 0, 168);

            Actions act = new Actions(Driver);
            new Actions(Driver).MoveToElement(ViewerPane, ViewerPane.Size.Width / 2 + 15, ViewerPane.Size.Height / 4 - 30).Click().Build().Perform();
            wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));

            result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);

            DownloadImageFile(ViewerPane, result.steps[ExecutedSteps].goldimagepath);
            int Afterblue = z3dvp.selectedcolorcheck(result.steps[ExecutedSteps].goldimagepath, 0, 0, 168);
            bool Step9 = false;
            bool Step9_1 = false;
            if (Afterblue > BeforeBlue)
            {
                Step9 = true;
                Logger.Instance.InfoLog("Step9 result is PASS");
            }

            IWebElement TissueSelectionVolume = Driver.FindElement(By.CssSelector(Locators.CssSelector.tissueSelectionVolume));
            string SelectionVolume = TissueSelectionVolume.Text;
            String[] VolumeValue = SelectionVolume.Split(' ');
            if (Convert.ToDouble(VolumeValue[0]) > 0 && VolumeValue[1].Contains("cm3"))
            {
                Step9_1 = true;
            }

            if (Step9_1 && Step9)
            {
                Verification2 = true;
                Logger.Instance.InfoLog("Verification2 result is PASS");
            }
            else
            {
                Verification2 = false;
                Logger.Instance.InfoLog("Step9_1"+ Step9_1);
                Logger.Instance.InfoLog("Step9"+ Step9);
                Logger.Instance.InfoLog("Verification2 result is FAIL");
            }

            bool step10 = z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "", "Threshold", 50);
            z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Apply New Settings");
            Thread.Sleep(3000);
            string AfterSelectVolume = TissueSelectionVolume.Text;
            String[] AftVolumeValue = AfterSelectVolume.Split(' ');
            if (Convert.ToDouble(AftVolumeValue[0]) > Convert.ToDouble(VolumeValue[0]))
            {
                Verification3 = true;
                Logger.Instance.InfoLog("Verification3 result is PASS");
            }
            else
            {
                Verification3 = false;
                Logger.Instance.InfoLog("AftVolumeValue[0]"+ AftVolumeValue[0]);
                Logger.Instance.InfoLog("VolumeValue[0]"+ VolumeValue[0]);
                Logger.Instance.InfoLog("Verification3 result is FAIL");
            }

            bool step11 = z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "", "Radius", 0);
            z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Apply New Settings");
            Thread.Sleep(3000);
            string AfterRadioustVolume = TissueSelectionVolume.Text;
            String[] AftRadVolumeValue = AfterSelectVolume.Split(' ');
            if (Convert.ToDouble(VolumeValue[0]) < Convert.ToDouble(AftRadVolumeValue[0]))
            {
                Verification4 = true;
                Logger.Instance.InfoLog("Verification4 result is PASS");
            }
            else
            {
                Verification4 = false;
                Logger.Instance.InfoLog("VolumeValue[0]"+ VolumeValue[0]);
                Logger.Instance.InfoLog("AftRadVolumeValue[0]"+ AftRadVolumeValue[0]);
                Logger.Instance.InfoLog("Verification4 result is FAIL");
            }

            bool step12 = z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Delete Selected");
            IWebElement WholeViewerPanel = z3dvp.ViewerContainer();
            result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 4);
            if (CompareImage(result.steps[ExecutedSteps], WholeViewerPanel))
            {
                Verification5 = true;
                Logger.Instance.InfoLog("Verification5 result is PASS");
            }
            else
            {
                Verification5 = false;
                Logger.Instance.InfoLog("Verification5 result is FAIL");
            }

            z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Undo");
            result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 5);
            if (CompareImage(result.steps[ExecutedSteps], WholeViewerPanel))
            {
                Verification6 = true;
                Logger.Instance.InfoLog("Verification6 result is PASS");
            }
            else
            {
                Verification6 = false;
                Logger.Instance.InfoLog("Verification6 result is FAIL");
            }

            string afterUndoVolume = TissueSelectionVolume.Text;
            String[] VolumeValueNow = afterUndoVolume.Split(' ');
            if (Convert.ToDouble(AftRadVolumeValue[0]) >= Convert.ToDouble(VolumeValueNow[0]))
            {
                Verification7 = true;
                Logger.Instance.InfoLog("Verification7 result is PASS");
            }
            else
            {
                Logger.Instance.InfoLog("AftRadVolumeValue[0]"+ AftRadVolumeValue[0]);
                Logger.Instance.InfoLog("VolumeValueNow[0]"+ VolumeValueNow[0]);
                Logger.Instance.InfoLog("Verification7 result is FAIL");
                Verification7 = false;
            }

            z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Redo");
            result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 6);
            if (CompareImage(result.steps[ExecutedSteps], WholeViewerPanel))
            {
                Verification8 = true;
                Logger.Instance.InfoLog("Verification8 result is PASS");
            }
            else
            {
                Verification8 = false;
                Logger.Instance.InfoLog("Verification8 result is FAIL");
            }

            z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Undo");
            result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 7);
            if (CompareImage(result.steps[ExecutedSteps], WholeViewerPanel))
            {
                Verification9 = true;
                Logger.Instance.InfoLog("Verification9 result is PASS");
            }
            else
            {
                Verification9 = false;
                Logger.Instance.InfoLog("Verification9 result is FAIL");
            }

            string UndoVolume = TissueSelectionVolume.Text;
            String[] VolumeValueNow1 = UndoVolume.Split(' ');
            if (Convert.ToDouble(VolumeValueNow[0]).Equals(Convert.ToDouble(VolumeValueNow1[0])))
            {
                Verification10 = true;
                Logger.Instance.InfoLog("Verification10 result is PASS");
            }
            else
            {
                Verification10 = false;
                Logger.Instance.InfoLog("AftRadVolumeValue[0]"+ AftRadVolumeValue[0]);
                Logger.Instance.InfoLog("VolumeValueNow1[0]"+ VolumeValueNow1[0]);
                Logger.Instance.InfoLog("Verification10 result is FAIL");
            }

            bool step18 = z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Delete Unselected");
            result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 8);
            if (CompareImage(result.steps[ExecutedSteps], WholeViewerPanel))
            {
                Verification11 = true;
                Logger.Instance.InfoLog("Verification11 result is PASS");
            }
            else
            {
                Verification11 = false;
                Logger.Instance.InfoLog("Verification11 result is FAIL");
            }

            z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Undo");
            result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 9);
            if (CompareImage(result.steps[ExecutedSteps], WholeViewerPanel))
            {
                Verification12 = true;
                Logger.Instance.InfoLog("Verification12 result is PASS");
            }
            else
            {
                Verification12 = false;
                Logger.Instance.InfoLog("Verification12 result is FAIL");
            }

            string UndoVolume_Step20 = TissueSelectionVolume.Text;
            String[] VolumeValueNow_20 = UndoVolume_Step20.Split(' ');
            if (Convert.ToDouble(VolumeValueNow[0]).Equals(Convert.ToDouble(VolumeValueNow_20[0])))
            {
                Verification13 = true;
                Logger.Instance.InfoLog("Verification13 result is PASS");
            }
            else
            {
                Verification13 = false;
                Logger.Instance.InfoLog("AftRadVolumeValue[0]"+ AftRadVolumeValue[0]);
                Logger.Instance.InfoLog("VolumeValueNow_20[0]" + VolumeValueNow_20[0]);
                Logger.Instance.InfoLog("Verification13 result is FAIL");
            }

            bool Verification14;
            z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Redo");
            result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 9);
            if (CompareImage(result.steps[ExecutedSteps], WholeViewerPanel))
            {
                Verification14 = true;
                Logger.Instance.InfoLog("Verification14 result is PASS");
            }
            else
            {
                Verification14 = false;
                Logger.Instance.InfoLog("Verification14 result is FAIL");
            }
            Logger.Instance.InfoLog("Test163241 Final Result:" + Verification1 + Verification2 + Verification3 + Verification4 + Verification5 + Verification6 + Verification7 + Verification8 + Verification9 + Verification10 + Verification11 + Verification12 + Verification13 + Verification14);
            if (Verification1 && Verification2 && Verification3 && Verification4 && Verification5 && Verification6 && Verification7 && Verification8 && Verification9 && Verification10 && Verification11 && Verification12 && Verification13 && Verification14)
            {
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Close");
                return true;
            }
            else
            {
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Close");
                return false;
            }

        }

        //Repeted steps on 4:1 ViewMode
        public bool Test163241_repeatedSteps_2(string testid, int stepcount, string Patientid, string ImageCount, int ExecutedSteps)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            bool Verification1, Verification2, Verification3, Verification4, Verification5, Verification6, Verification7, Verification8, Verification9, Verification10, Verification11, Verification12, Verification13;
            login.Logout();
            login.LoginIConnect(Config.adminUserName, Config.adminPassword);
            z3dvp.searchandopenstudyin3D(Patientid, ImageCount);
            z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);


            IWebElement ViewerPane = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
            z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Preset2, "Preset");
            result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
            if (CompareImage(result.steps[ExecutedSteps], ViewerPane))
            {
                result.steps[ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
            }
            else
            {
                result.steps[ExecutedSteps].status = "Fail";
                Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                result.steps[ExecutedSteps].SetLogs();
            }

            z3dvp.select3DTools(Z3DTools.Selection_Tool);
            bool Step7 = z3dvp.Handle3dToolsDialogs("3D Tissue Selection Instructions", "Close");

            z3dvp.select3DTools(Z3DTools.Selection_Tool);
            IWebElement ThreshholdValue = Driver.FindElement(By.CssSelector(Locators.CssSelector.ThreshholdProgressBar));
            string BeforeThresholdValue = ThreshholdValue.GetAttribute("aria-valuenow");
            IWebElement radiousvalue = Driver.FindElement(By.CssSelector(Locators.CssSelector.RadiousProgressBar));
            bool Step8 = z3dvp.Handle3dToolsDialogs("3D Tissue Selection Instructions", "Large vessels");
            String AfterThresholdValue = ThreshholdValue.GetAttribute("aria-valuenow");
            string RadiousValue = radiousvalue.GetAttribute("aria-valuenow");
            if (Convert.ToInt32(BeforeThresholdValue) > Convert.ToInt32(AfterThresholdValue) && Convert.ToInt32(RadiousValue) == 2000)
            {
                Verification1 = true;
                Logger.Instance.InfoLog("Verification1 result is PASS");
            }
            else
            {
                Verification1 = false;
                Logger.Instance.InfoLog("Verification1 result is Fail");
                Logger.Instance.InfoLog("BeforeThresholdValue "+ BeforeThresholdValue);
                Logger.Instance.InfoLog("AfterThresholdValue"+ AfterThresholdValue);
                Logger.Instance.InfoLog("RadiousValue"+ RadiousValue);
            }

            result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
            DownloadImageFile(ViewerPane, result.steps[ExecutedSteps].goldimagepath);
            int BeforeBlue = z3dvp.selectedcolorcheck(result.steps[ExecutedSteps].goldimagepath, 0, 0, 168);

            Actions act = new Actions(Driver);
            new Actions(Driver).MoveToElement(ViewerPane, ViewerPane.Size.Width / 2 + 15, ViewerPane.Size.Height / 4 - 30).Click().Build().Perform();
            wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
            Thread.Sleep(6000);

            result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);

            DownloadImageFile(ViewerPane, result.steps[ExecutedSteps].goldimagepath);
            int Afterblue = z3dvp.selectedcolorcheck(result.steps[ExecutedSteps].goldimagepath, 0, 0, 168);
            bool Step9 = false;
            bool Step9_1 = false;
            if (Afterblue > BeforeBlue)
            {
                Step9 = true;
            }

            IWebElement TissueSelectionVolume = Driver.FindElement(By.CssSelector(Locators.CssSelector.tissueSelectionVolume));
            string SelectionVolume = TissueSelectionVolume.Text;
            String[] VolumeValue = SelectionVolume.Split(' ');
            if (Convert.ToDouble(VolumeValue[0]) > 0 && VolumeValue[1].Contains("cm3"))
            {
                Step9_1 = true;
            }

            if (Step9_1 && Step9)
            {
                Verification2 = true;
                Logger.Instance.InfoLog("Verification2 result is Pass");
            }
            else
            {
                Verification2 = false;
                Logger.Instance.InfoLog("Verification2 result is Fail");
                Logger.Instance.InfoLog("VolumeValue[0]"+ VolumeValue[0]);
                Logger.Instance.InfoLog("VolumeValue[1]"+ VolumeValue[1]);
            }

            bool step10 = z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "", "Threshold", 50);
            z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Apply New Settings");
            Thread.Sleep(3000);
            string AfterSelectVolume = TissueSelectionVolume.Text;
            String[] AftVolumeValue = AfterSelectVolume.Split(' ');
            if (Convert.ToDouble(AftVolumeValue[0]) > Convert.ToDouble(VolumeValue[0]))
            {
                Verification3 = true;
                Logger.Instance.InfoLog("Verification3 result is Pass");
            }
            else
            {
                Verification3 = false;
                Logger.Instance.InfoLog("Verification3 result is Fail");
                Logger.Instance.InfoLog("AftVolumeValue[0]"+ AftVolumeValue[0]);
                Logger.Instance.InfoLog("VolumeValue[0]"+ VolumeValue[0]); 
            }

            bool step11 = z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "", "Radius", 0);
            z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Apply New Settings");
            Thread.Sleep(3000);
            string AfterRadioustVolume = TissueSelectionVolume.Text;
            String[] AftRadVolumeValue = AfterSelectVolume.Split(' ');
            if (Convert.ToDouble(AftVolumeValue[0]) < Convert.ToDouble(AftRadVolumeValue[0]))
            {
                Verification4 = true;
                Logger.Instance.InfoLog("Verification4 result is Fail");
            }
            else
            {
                Verification4 = false;
                Logger.Instance.InfoLog("Verification4 result is Fail");
                Logger.Instance.InfoLog("AftVolumeValue[0]"+ AftVolumeValue[0]);
                Logger.Instance.InfoLog("AftRadVolumeValue[0]"+ AftRadVolumeValue[0]);
            }

            bool step12 = z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Delete Selected");
            IWebElement WholeViewerPanel = z3dvp.ViewerContainer();
            result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 4);
            if (CompareImage(result.steps[ExecutedSteps], WholeViewerPanel))
            {
                Verification5 = true;
                Logger.Instance.InfoLog("Verification5 result is Pass");
            }
            else
            {
                Verification5 = false;
                Logger.Instance.InfoLog("Verification5 result is Fail");
            }

            z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Undo");
            result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 5);
            if (CompareImage(result.steps[ExecutedSteps], WholeViewerPanel))
            {
                Verification6 = true;
                Logger.Instance.InfoLog("Verification6 result is PASS");
            }
            else
            {
                Verification6 = false;
                Logger.Instance.InfoLog("Verification6 result is Fail");
            }

            string afterUndoVolume = TissueSelectionVolume.Text;
            String[] VolumeValueNow = afterUndoVolume.Split(' ');
            //if (Convert.ToDouble(AftVolumeValue[0]).Equals(Convert.ToDouble(VolumeValueNow[0])))
            if (Convert.ToDouble(AftVolumeValue[0]) > (Convert.ToDouble(VolumeValueNow[0])) + 1 || Convert.ToDouble(AftVolumeValue[0]) < (Convert.ToDouble(VolumeValueNow[0])) + 1)
            {
                Logger.Instance.InfoLog("Verification7 result is Pass");
                Verification7 = true;
            }
            else
            {
                Logger.Instance.InfoLog("Verification7 result is Fail");
                Logger.Instance.InfoLog("AftVolumeValue[0]"+ AftVolumeValue[0]);
                Logger.Instance.InfoLog("VolumeValueNow[0]"+ VolumeValueNow[0]);
                Verification7 = false;
            }

            z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Redo");
            result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 6);
            if (CompareImage(result.steps[ExecutedSteps], WholeViewerPanel))
            {
                Verification8 = true;
                Logger.Instance.InfoLog("Verification8 result is Pass");
            }
            else
            {
                Verification8 = false;
                Logger.Instance.InfoLog("Verification8 result is Fail");
            }

            z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Undo");
            result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 7);
            if (CompareImage(result.steps[ExecutedSteps], WholeViewerPanel))
            {
                Verification9 = true;
                Logger.Instance.InfoLog("Verification9 result is Pass");
            }
            else
            {
                Verification9 = false;
                Logger.Instance.InfoLog("Verification9 result is Fail");
            }

            string UndoVolume = TissueSelectionVolume.Text;
            String[] VolumeValueNow1 = UndoVolume.Split(' ');
            if (Convert.ToDouble(VolumeValueNow[0]).Equals(Convert.ToDouble(VolumeValueNow1[0])))
            {
                Verification10 = true;
                Logger.Instance.InfoLog("Verification10 result is Pass");
            }
            else
            {
                Verification10 = false;
                Logger.Instance.InfoLog("Verification10 result is Fail");
                Logger.Instance.InfoLog("AftVolumeValue[0]"+ AftVolumeValue[0]);
                Logger.Instance.InfoLog("VolumeValueNow1[0]"+ VolumeValueNow1[0]);
            }

            bool step18 = z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Delete Unselected");
            result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 8);
            if (CompareImage(result.steps[ExecutedSteps], WholeViewerPanel))
            {
                Verification11 = true;
                Logger.Instance.InfoLog("Verification11 result is Pass");
            }
            else
            {
                Verification11 = false;
                Logger.Instance.InfoLog("Verification11 result is fail");
            }

            z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Undo");
            result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 9);
            if (CompareImage(result.steps[ExecutedSteps], WholeViewerPanel))
            {
                Verification12 = true;
                Logger.Instance.InfoLog("Verification12 result is true");
            }
            else
            {
                Verification12 = false;
                Logger.Instance.InfoLog("Verification12 result is fail");
            }

            string UndoVolume_Step20 = TissueSelectionVolume.Text;
            String[] VolumeValueNow_20 = UndoVolume_Step20.Split(' ');
            if (Convert.ToDouble(VolumeValueNow[0]).Equals(Convert.ToDouble(VolumeValueNow_20[0])))
            {
                Verification13 = true;
                Logger.Instance.InfoLog("Verification13 result is fail");
            }
            else
            {
                Verification13 = false;
                Logger.Instance.InfoLog("Verification13 result is fail");
                Logger.Instance.InfoLog("AftVolumeValue[0]"+ AftVolumeValue[0]);
                Logger.Instance.InfoLog("VolumeValueNow_20[0]"+ VolumeValueNow_20[0]);
            }

            bool Verification14;
            z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Redo");
            result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 10);
            if (CompareImage(result.steps[ExecutedSteps], WholeViewerPanel))
            {
                Verification14 = true;
                Logger.Instance.InfoLog("Verification14 result passed");
            }
            else
            {
                Verification14 = false;
                Logger.Instance.InfoLog("Verification14 result failed");
            }
            Logger.Instance.InfoLog("RepeatedStep_2" + Verification1 + Verification2 + Verification3 + Verification4 + Verification5 + Verification6 + Verification7 + Verification8 + Verification9 + Verification10 + Verification11 + Verification12 + Verification13 + Verification14);
            if (Verification1 && Verification2 && Verification3 && Verification4 && Verification5 && Verification6 && Verification7 && Verification8 && Verification9 && Verification10 && Verification11 && Verification12 && Verification13 && Verification14)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public TestCaseResult Test_163229(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string[] patientids = Patientid.Split('|');
            string Patientid1 = patientids[0];
            string Patientid2 = patientids[1];
            string Patientid3 = patientids[2];
            string[] ImageCounts = ImageCount.Split('|');
            string ImageCount1 = ImageCounts[0];
            string ImageCount2 = ImageCounts[1];
            string ImageCount3 = ImageCounts[2];

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //step:1 - Login Ica viewer
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                result.steps[++ExecutedSteps].StepPass();

                //Step 02 & 03:
                Boolean step2 = z3dvp.searchandopenstudyin3D(Patientid1, ImageCount1);
                if (step2)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    Logger.Instance.ErrorLog("-->Test Step Failed-- to launch the study in MPR Layout");
                    result.steps[++ExecutedSteps].SetLogs();
                    throw new Exception("Failed to launch the study");
                }
                bool test = z3dvp.select3DTools(Z3DTools.Reset);
                Logger.Instance.InfoLog(test.ToString());

                //Step:4
                bool step_4 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                if(step_4 == true)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:5
                bool step4 = z3dvp.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigation3D1, "Bone & Vessels-Dark Background A", "Preset");
                bool step4_2 = z3dvp.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigation3D2, "Skin & Monochrome Sharp", "Preset");
                if (step4 & step4_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:6
                bool step_6 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                if(step_6)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:7
                bool step_7 = z3dvp.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigation3D1,BluRingZ3DViewerPage.Preset4, "Preset");
                if(step_7)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:8
                bool Result = z3dvp.VerifyAllPreset(testid, BluRingZ3DViewerPage.Navigation3D1);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:9
                z3dvp.CloseViewer();
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                IWebElement LogoutBtn = login.LogoutBtn();
                if(LogoutBtn.Displayed)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step 10 & 11
                Result = z3dvp.searchandopenstudyin3D(Patientid2, ImageCount2);
                if (Result)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed to open study due to exception in Test_163229 Step 10 & 11");
                    throw new Exception("Failed due to exception in Test_163229 Step 10 & 11");
                }

                //Step:12
                Result = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:13
                bool step12 = z3dvp.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Preset10, "Preset");
                bool step12_2 = z3dvp.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigation3D2, BluRingZ3DViewerPage.Preset11,"Preset");
                if (step12 && step12_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:14
                Result = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:15
                bool step14 = z3dvp.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Preset10 , "Preset");
                if (step14)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:16
                Result = z3dvp.VerifyAllPreset(testid, BluRingZ3DViewerPage.Navigation3D1);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:17
                z3dvp.CloseViewer();
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                LogoutBtn = login.LogoutBtn();
                if (LogoutBtn.Displayed)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step 18 & 19
                Result = z3dvp.searchandopenstudyin3D(Patientid3, ImageCount3);
                if (Result)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed to open study in Test_163229 Step 18 & 19");
                    throw new Exception("Failed due to exception in Test_163229 Step 18 & 19");
                }

                //Step:20
                Result = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:21
                bool step20 = z3dvp.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Preset13, "preset");
                bool step20_2 = z3dvp.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigation3D2, BluRingZ3DViewerPage.Preset14, "preset");
                if (step20 & step20_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:22
                Result = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:23
                bool step23 = z3dvp.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Preset13 , "preset");
                if (step23)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:24
                Result = z3dvp.VerifyAllPreset(testid, BluRingZ3DViewerPage.Navigation3D1);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:25
                z3dvp.CloseViewer();
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                LogoutBtn = login.LogoutBtn();
                if (LogoutBtn.Displayed)
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
                //z3dvp.CloseViewer();
                login.Logout();
            }

        }

        public TestCaseResult Test_163236(string testid, String teststeps, int stepcount)
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
                //step:1 - Login Ica viewer
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                result.steps[++ExecutedSteps].StepPass();

                //step 02
                bool res = z3dvp.searchandopenstudyin3D(Patientid, ImageCount);
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                
                //Step:3
                bool step3 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                if (step3)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step 04
                res = z3dvp.ChangeViewMode();
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    throw new Exception("Failed in Test_163236 step 04 & 05 while switching to 3D Viewmode in 6:1 Layout");
                }

                //Step:5
                bool step6 = z3dvp.VerifyHighLightedBorder_ParticularVieport(BluRingZ3DViewerPage.Navigation3D1, "rgb(211, 211, 211)");
                if (step6)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:6
                bool steps = false;
                IWebElement Viewport = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                new Actions(Driver).MoveToElement(Viewport, Viewport.Size.Width / 4, Viewport.Size.Height / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                int yellowcolorbefore = z3dvp.LevelOfSelectedColor(Viewport, testid, ExecutedSteps + 61, 255, 255, 0, 2);
                new Actions(Driver).MoveToElement(Viewport, Viewport.Size.Width / 4, Viewport.Size.Height / 2).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Viewport = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                String CursorName = Viewport.GetCssValue("cursor");
                int yellowcolorafter = z3dvp.LevelOfSelectedColor(Viewport, testid, ExecutedSteps + 62, 255, 255, 0, 2, isMoveCursor: false);
                if (CursorName.Contains(BluRingZ3DViewerPage.RotateCursor))
                {
                    steps = true;
                }
                if (yellowcolorafter > yellowcolorbefore && steps)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:7
                String Leftorientation3D1before = z3dvp.GetPositionValue(BluRingZ3DViewerPage.Navigation3D1, "left");
                String Rightorientation3D1before = z3dvp.GetPositionValue(BluRingZ3DViewerPage.Navigation3D1, "right");
                String Leftorientation3D2before = z3dvp.GetPositionValue(BluRingZ3DViewerPage.Navigation3D2, "left");
                String Rightorientation3D2before = z3dvp.GetPositionValue(BluRingZ3DViewerPage.Navigation3D2, "right");
                IWebElement ThreeD1Navigation = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                new Actions(Driver).MoveToElement(ThreeD1Navigation, (ThreeD1Navigation.Size.Width / 4) - 10, ThreeD1Navigation.Size.Height / 2).ClickAndHold()
                    .MoveToElement(ThreeD1Navigation, ThreeD1Navigation.Size.Width - 5, ThreeD1Navigation.Size.Height / 2).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                String Leftorientation3D1after = z3dvp.GetPositionValue(BluRingZ3DViewerPage.Navigation3D1, "left");
                String Rightorientation3D1after = z3dvp.GetPositionValue(BluRingZ3DViewerPage.Navigation3D1, "right");
                String Leftorientation3D2after = z3dvp.GetPositionValue(BluRingZ3DViewerPage.Navigation3D2, "left");
                String Rightorientation3D2after = z3dvp.GetPositionValue(BluRingZ3DViewerPage.Navigation3D2, "right");
                bool CenterLeftannotationverification = Rightorientation3D1after.Equals(Leftorientation3D1before) && Rightorientation3D2after.Equals(Leftorientation3D2before);
                bool Rightannotationverification = Leftorientation3D1after.Equals(Rightorientation3D1before) && Leftorientation3D2after.Equals(Rightorientation3D2before);
                if (!CenterLeftannotationverification && !Rightannotationverification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:8
                res = z3dvp.select3DTools(Z3DTools.Reset);
                if (!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                {
                    PageLoadWait.WaitForFrameLoad(10);
                    String Leftorientation3D1afterreset = z3dvp.GetPositionValue(BluRingZ3DViewerPage.Navigation3D1, "left");
                    String Rightorientation3D1afterreset = z3dvp.GetPositionValue(BluRingZ3DViewerPage.Navigation3D1, "right");
                    String Leftorientation3D2afterreset = z3dvp.GetPositionValue(BluRingZ3DViewerPage.Navigation3D2, "left");
                    String Rightorientation3D2afterreset = z3dvp.GetPositionValue(BluRingZ3DViewerPage.Navigation3D2, "right");
                    CenterLeftannotationverification = Leftorientation3D1afterreset.Equals(Leftorientation3D1before) && Leftorientation3D2afterreset.Equals(Leftorientation3D2before);
                    Rightannotationverification = Rightorientation3D1afterreset.Equals(Rightorientation3D1before) && Rightorientation3D2afterreset.Equals(Rightorientation3D2before);
                    if (CenterLeftannotationverification && Rightannotationverification)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }

                //Step:9
                bool step10 = z3dvp.VerifyHighLightedBorder_ParticularVieport(BluRingZ3DViewerPage.Navigation3D2, "rgb(211, 211, 211)");
                if (step10)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:10
                bool step10_1 = false;
                IWebElement Viewport3D2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                new Actions(Driver).MoveToElement(Viewport3D2, Viewport3D2.Size.Width / 4, Viewport3D2.Size.Height / 4).Click().Build().Perform();
                int yellowcolorbefore1 = z3dvp.LevelOfSelectedColor(Viewport3D2, testid, ExecutedSteps + 101, 255, 255, 0, 2);
                Viewport3D2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                new Actions(Driver).MoveToElement(Viewport3D2, Viewport3D2.Size.Width / 4, Viewport3D2.Size.Height / 2).Build().Perform();
                Thread.Sleep(3000);
                String CursorName1 = Viewport3D2.GetCssValue("cursor");
                int yellowcolorafter1 = z3dvp.LevelOfSelectedColor(Viewport3D2, testid, ExecutedSteps + 102, 255, 255, 0, 2);
                if (CursorName1.Contains(BluRingZ3DViewerPage.RotateCursor))
                {
                    step10_1 = true;
                }
                if(yellowcolorafter1 > yellowcolorbefore1 && step10_1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:11
                String locationvalue3D1Before = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                String locationvalue3D2Before = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                String toporientation3D1before = z3dvp.GetPositionValue(BluRingZ3DViewerPage.Navigation3D1, "top");
                String toporientation3D2before = z3dvp.GetPositionValue(BluRingZ3DViewerPage.Navigation3D2, "top");
                IWebElement ThreeD2Navigation = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                new Actions(Driver).MoveToElement(ThreeD2Navigation, ThreeD2Navigation.Size.Width / 2, ThreeD2Navigation.Size.Height / 4).Build().Perform();
                String filepath = Config.downloadpath + "\\"+testid+"_Step11.png";
                if (File.Exists(filepath))
                    File.Delete(filepath);
                DownloadImageFile(ThreeD2Navigation, filepath, "png");
                String colorsplittedpath = Config.downloadpath + "\\" + testid + "_Step11_1.png";
                if (File.Exists(colorsplittedpath))
                    File.Delete(colorsplittedpath);
                z3dvp.yellowcolorsplitter(filepath, colorsplittedpath);
                IList<Accord.IntPoint> yellowquadpoints = z3dvp.ImageQuadPoints(colorsplittedpath);
                new Actions(Driver).MoveToElement(ThreeD2Navigation, Convert.ToInt32(yellowquadpoints[1].X), Convert.ToInt32(yellowquadpoints[1].Y)).ClickAndHold().
                    MoveToElement(ThreeD2Navigation, Convert.ToInt32(yellowquadpoints[1].X), ThreeD2Navigation.Size.Height - 10).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                String locationvalue3D1After = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                String locationvalue3D2After = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                String toporientation3D1after = z3dvp.GetPositionValue(BluRingZ3DViewerPage.Navigation3D1, "top");
                String toporientation3D2after = z3dvp.GetPositionValue(BluRingZ3DViewerPage.Navigation3D2, "top");
                bool topannotationverification = toporientation3D1before.Equals(toporientation3D1after) && toporientation3D2before.Equals(toporientation3D2after);
                bool LocationValueVerification = locationvalue3D1After.Equals(locationvalue3D1Before) && locationvalue3D2After.Equals(locationvalue3D2Before);
                if (!topannotationverification && !LocationValueVerification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:12
                res = z3dvp.select3DTools(Z3DTools.Reset);
                if (!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                {
                    PageLoadWait.WaitForFrameLoad(10);
                    String locationvalue3D1Afterreset = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                    String locationvalue3D2Afterreset = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                    String toporientation3D1afterreset = z3dvp.GetPositionValue(BluRingZ3DViewerPage.Navigation3D1, "top");
                    String toporientation3D2afterreset = z3dvp.GetPositionValue(BluRingZ3DViewerPage.Navigation3D2, "top");
                    topannotationverification = toporientation3D1afterreset.Equals(toporientation3D1before) && toporientation3D2afterreset.Equals(toporientation3D2before);
                    LocationValueVerification = locationvalue3D1Afterreset.Equals(locationvalue3D1Before) && locationvalue3D2Afterreset.Equals(locationvalue3D2Before);
                    if (topannotationverification && LocationValueVerification)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }

                //Step:13
                bool step14 = z3dvp.VerifyHighLightedBorder_ParticularVieport(BluRingZ3DViewerPage.Navigation3D1, "rgb(211, 211, 211)");
                if (step14)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:14
                Viewport = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                new Actions(Driver).MoveToElement(Viewport, Viewport.Size.Width / 4, Viewport.Size.Height / 4).Click().Build().Perform();
                int yellowcolorbefore14 = z3dvp.LevelOfSelectedColor(Viewport, testid, ExecutedSteps + 141, 255, 255, 0, 2);
                String BaseImages = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + "TempImages" + Path.DirectorySeparatorChar + "BaseImages" + Path.DirectorySeparatorChar + Config.BrowserType;
                Directory.CreateDirectory(BaseImages);
                String ColorSplitImages = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + "TempImages" + Path.DirectorySeparatorChar + "ColorSplitted" + Path.DirectorySeparatorChar + Config.BrowserType;
                Directory.CreateDirectory(ColorSplitImages);
                String baseimagepath = BaseImages + Path.DirectorySeparatorChar + testid + "_" + ExecutedSteps + 142 + ".png";
                colorsplittedpath = ColorSplitImages + Path.DirectorySeparatorChar + testid + "_" + ExecutedSteps + 143 + ".png";
                DownloadImageFile(Viewport, baseimagepath, "png");
                z3dvp.yellowcolorsplitter(baseimagepath, colorsplittedpath);
                IList<Accord.IntPoint> quadrilateral = z3dvp.ImageQuadPoints(colorsplittedpath, 0);
                //yellowintersection = z3dvp.GetIntersectionPoints(Viewport, testid, ExecutedSteps + 142, "yellow", "Vertical", 0);
                new Actions(Driver).MoveToElement(ThreeD1Navigation, Convert.ToInt32(quadrilateral[0].X), Convert.ToInt32(quadrilateral[0].Y)).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                int yellowcolorafter14 = z3dvp.LevelOfSelectedColor(Viewport, testid, ExecutedSteps + 144, 255, 255, 0, 2);
                if (yellowcolorafter14 > yellowcolorbefore14)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();




                /*
                Viewport = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                new Actions(Driver).MoveToElement(Viewport, Viewport.Size.Width / 4, Viewport.Size.Height / 4).Click().Build().Perform();
                int yellowcolorbefore14 = z3dvp.LevelOfSelectedColor(Viewport, testid, ExecutedSteps + 141, 255, 255, 0, 2);
                yellowintersection = z3dvp.GetIntersectionPoints(Viewport, testid, ExecutedSteps + 142, "yellow", "Vertical", 0);
                new Actions(Driver).MoveToElement(ThreeD1Navigation, , Convert.ToInt32(yellowintersection.Y)).Click().Build().Perform();
                int yellowcolorafter14 = z3dvp.LevelOfSelectedColor(Viewport, testid, ExecutedSteps + 143, 255, 255, 0, 2);
                if (yellowcolorafter14 > yellowcolorbefore14)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();*/

                //Step:15
                Viewport = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                String orientationvalue3D1before = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                String orientationvalue3D2before = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D2);
                new Actions(Driver).MoveToElement(Viewport, Convert.ToInt32(quadrilateral[0].X), Convert.ToInt32(quadrilateral[0].Y)).ClickAndHold().
                    MoveToElement(Viewport, Convert.ToInt32(yellowquadpoints[1].X), Convert.ToInt32(yellowquadpoints[1].Y)).
                    MoveToElement(Viewport, Viewport.Size.Width - 5, Convert.ToInt32(quadrilateral[0].Y)).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                String orientationvalue3D1after = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                String orientationvalue3D2after = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D2);
                if (!orientationvalue3D1before.Equals(orientationvalue3D1after) && !orientationvalue3D2before.Equals(orientationvalue3D2after))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();
                /*
                Viewport = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                String orientationvalue3D1before = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                String orientationvalue3D2before = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D2);
                new Actions(Driver).MoveToElement(Viewport, Convert.ToInt32(yellowintersection.X), Convert.ToInt32(yellowintersection.Y)).ClickAndHold().
                    MoveToElement(Viewport, Convert.ToInt32(yellowquadpoints[1].X), Convert.ToInt32(yellowquadpoints[1].Y)).
                    MoveToElement(Viewport, Viewport.Size.Width - 5, Convert.ToInt32(yellowintersection.Y)).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                String orientationvalue3D1after = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                String orientationvalue3D2after = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D2);
                if(!orientationvalue3D1before.Equals(orientationvalue3D1after) && !orientationvalue3D2before.Equals(orientationvalue3D2after))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();*/

                //Step:16
                z3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                String orientationvalue3D1afterreset = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                String orientationvalue3D2afterreset = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D2);
                if (orientationvalue3D1afterreset.Equals(orientationvalue3D1before) && orientationvalue3D2afterreset.Equals(orientationvalue3D2before))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:17
                res = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigation3D1);
                if(res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:18
                locationvalue3D1Before = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                locationvalue3D2Before = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                orientationvalue3D1before = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                orientationvalue3D2before = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D2);
                ThreeD1Navigation = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                new Actions(Driver).MoveToElement(ThreeD1Navigation, ThreeD1Navigation.Size.Width/4, (ThreeD1Navigation.Size.Height) - 5).ClickAndHold().
                    MoveToElement(ThreeD1Navigation, ThreeD1Navigation.Size.Width/4 , 5).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                locationvalue3D1After = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                locationvalue3D2After = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                orientationvalue3D1after = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                orientationvalue3D2after = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D2);
                bool Orientationvalueverification = orientationvalue3D1before.Equals(orientationvalue3D1after) && orientationvalue3D2after.Equals(orientationvalue3D2before);
                LocationValueVerification = locationvalue3D1After.Equals(locationvalue3D1Before) && locationvalue3D2After.Equals(locationvalue3D2Before);
                if (!Orientationvalueverification && !LocationValueVerification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:19
                res = z3dvp.select3DTools(Z3DTools.Reset);
                if (res)
                {
                    PageLoadWait.WaitForFrameLoad(10);
                    String locationvalue3D1Afterreset = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                    String locationvalue3D2Afterreset = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                    orientationvalue3D1afterreset = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                    orientationvalue3D2afterreset = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D2);
                    Orientationvalueverification = orientationvalue3D1before.Equals(orientationvalue3D1afterreset) && orientationvalue3D2afterreset.Equals(orientationvalue3D2before);
                    LocationValueVerification = locationvalue3D1Afterreset.Equals(locationvalue3D1Before) && locationvalue3D2Afterreset.Equals(locationvalue3D2Before);
                    if (Orientationvalueverification && LocationValueVerification)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:20
                locationvalue3D1Before = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                locationvalue3D2Before = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                orientationvalue3D1before = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                orientationvalue3D2before = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D2);
                ThreeD1Navigation = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                new Actions(Driver).MoveToElement(ThreeD1Navigation, ThreeD1Navigation.Size.Width / 4, (ThreeD1Navigation.Size.Height /2) + 10).ClickAndHold().
                    MoveToElement(ThreeD1Navigation, 3 * (ThreeD1Navigation.Size.Width / 4), (ThreeD1Navigation.Size.Height / 2 ) + 10).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                locationvalue3D1After = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                locationvalue3D2After = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                orientationvalue3D1after = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                orientationvalue3D2after = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D2);
                Orientationvalueverification = orientationvalue3D1before.Equals(orientationvalue3D1after) && orientationvalue3D2after.Equals(orientationvalue3D2before);
                LocationValueVerification = locationvalue3D1After.Equals(locationvalue3D1Before) && locationvalue3D2After.Equals(locationvalue3D2Before);
                if (!Orientationvalueverification && !LocationValueVerification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:21
                z3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                res = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                if(res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:22
                String Step22_AnnotationValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                bool step23 = z3dvp.VerifyHighLightedBorder_ParticularVieport(BluRingZ3DViewerPage.Navigation3D1, "rgb(211, 211, 211)");
                if (step23)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:23
                Viewport = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                new Actions(Driver).MoveToElement(Viewport, Viewport.Size.Width / 4, Viewport.Size.Height / 4).Build().Perform();
                int yellowcolorbefore_23 = z3dvp.LevelOfSelectedColor(Viewport, testid, ExecutedSteps + 231, 255, 255, 0, 2);
                //Disappear unwanted yellow line
                new Actions(Driver).MoveToElement(Viewport, Viewport.Size.Width / 4, Viewport.Size.Height / 2).Build().Perform();
                Viewport = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                String CursorName_1 = Viewport.GetCssValue("cursor");
                PageLoadWait.WaitForFrameLoad(5);
                int yellowcolorafter_23 = z3dvp.LevelOfSelectedColor(Viewport, testid, ExecutedSteps + 232, 255, 255, 0, 2);
                if (yellowcolorafter_23 > yellowcolorbefore_23 && CursorName_1.Contains(BluRingZ3DViewerPage.RotateCursor))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

               /* Viewport = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                //Disappear unwanted yellow line
                new Actions(Driver).MoveToElement(Viewport, Viewport.Size.Width / 4, Viewport.Size.Height / 4).Build().Perform();
                int yellowcolorbefore_23 = z3dvp.LevelOfSelectedColor(Viewport, testid, ExecutedSteps + 231, 255, 255, 0, 2);
                new Actions(Driver).MoveToElement(Viewport, Viewport.Size.Width / 2, Viewport.Size.Height / 4).Build().Perform();
                int yellowcolorafter_23 = z3dvp.LevelOfSelectedColor(Viewport, testid, ExecutedSteps + 232, 255, 255, 0, 2);
                String CursorName_1 = Viewport.GetCssValue("cursor");
                if (yellowcolorafter_23 > yellowcolorbefore_23 && CursorName_1.Contains(BluRingZ3DViewerPage.RotateCursor))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();*/

                //Step 24
                ThreeD1Navigation = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                new Actions(Driver).MoveToElement(ThreeD1Navigation, ThreeD1Navigation.Size.Width / 4, ThreeD1Navigation.Size.Height / 4).Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                String Step24OrientationValue = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                Leftorientation3D1before = z3dvp.GetPositionValue(BluRingZ3DViewerPage.Navigation3D1, "left");
                Rightorientation3D1before = z3dvp.GetPositionValue(BluRingZ3DViewerPage.Navigation3D1, "right");
                //yellowintersection = z3dvp.GetIntersectionPoints(ThreeD1Navigation, testid, ExecutedSteps + 241, "yellow", "Vertical", 0);
                new Actions(Driver).MoveToElement(ThreeD1Navigation, (ThreeD1Navigation.Size.Width / 4) - 10, ThreeD1Navigation.Size.Height / 2).ClickAndHold().
                    MoveToElement(ThreeD1Navigation, ThreeD1Navigation.Size.Width - 5, ThreeD1Navigation.Size.Height / 2).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Leftorientation3D1after = z3dvp.GetPositionValue(BluRingZ3DViewerPage.Navigation3D1, "left");
                Rightorientation3D1after = z3dvp.GetPositionValue(BluRingZ3DViewerPage.Navigation3D1, "right");
                CenterLeftannotationverification = Rightorientation3D1after.Equals(Leftorientation3D1before);
                Rightannotationverification = Leftorientation3D1after.Equals(Rightorientation3D1before);
                if (!CenterLeftannotationverification && !Rightannotationverification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                /*ThreeD1Navigation = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                new Actions(Driver).MoveToElement(ThreeD1Navigation, ThreeD1Navigation.Size.Width / 4, ThreeD1Navigation.Size.Height / 4).Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                String Step24OrientationValue = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                Leftorientation3D1before = z3dvp.GetPositionValue(BluRingZ3DViewerPage.Navigation3D1, "left");
                Rightorientation3D1before = z3dvp.GetPositionValue(BluRingZ3DViewerPage.Navigation3D1, "right");
                yellowintersection = z3dvp.GetIntersectionPoints(ThreeD1Navigation, testid, ExecutedSteps + 241, "yellow", "Vertical", 0);
                new Actions(Driver).MoveToElement(ThreeD1Navigation, Convert.ToInt32(yellowintersection.X), Convert.ToInt32(yellowintersection.Y)).ClickAndHold().
                    MoveToElement(ThreeD1Navigation, ThreeD1Navigation.Size.Width - 5, Convert.ToInt32(yellowintersection.Y)).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Leftorientation3D1after = z3dvp.GetPositionValue(BluRingZ3DViewerPage.Navigation3D1, "left");
                Rightorientation3D1after = z3dvp.GetPositionValue(BluRingZ3DViewerPage.Navigation3D1, "right");
                CenterLeftannotationverification = Rightorientation3D1after.Equals(Leftorientation3D1before);
                Rightannotationverification = Leftorientation3D1after.Equals(Rightorientation3D1before);
                if (CenterLeftannotationverification && Rightannotationverification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();*/

                //Step 25
                res = z3dvp.select3DTools(Z3DTools.Reset);
                if (!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                {
                    PageLoadWait.WaitForFrameLoad(10);
                    String Step25OrientationValue = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                    if (Step25OrientationValue.Equals(Step24OrientationValue))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }

                //Step 26
                bool step26 = z3dvp.VerifyHighLightedBorder_ParticularVieport(BluRingZ3DViewerPage.Navigation3D1, "rgb(211, 211, 211)");
                if (step26)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step 27
                bool step27 = false;
                IWebElement Viewport3D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                new Actions(Driver).MoveToElement(Viewport3D1, Viewport3D1.Size.Width / 4, Viewport3D1.Size.Height / 4).Click().Build().Perform();
                yellowcolorbefore1 = z3dvp.LevelOfSelectedColor(Viewport3D1, testid, ExecutedSteps + 271, 255, 255, 0, 2);
                new Actions(Driver).MoveToElement(Viewport3D1, Viewport3D1.Size.Width / 4, Viewport3D1.Size.Height / 2).Click().Build().Perform();
                Viewport3D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                CursorName1 = Viewport3D1.GetCssValue("cursor");
                yellowcolorafter1 = z3dvp.LevelOfSelectedColor(Viewport3D1, testid, ExecutedSteps + 272, 255, 255, 0, 2);
                if (CursorName1.Contains(BluRingZ3DViewerPage.RotateCursor))
                    step27 = true;
                if (yellowcolorafter1 > yellowcolorbefore1 && step27)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step 28
                locationvalue3D1Before = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                ThreeD1Navigation = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                new Actions(Driver).MoveToElement(ThreeD1Navigation, ThreeD1Navigation.Size.Width / 2, ThreeD1Navigation.Size.Height / 4).Build().Perform();
                filepath = Config.downloadpath + "\\" + testid + "_Step28.png";
                if (File.Exists(filepath))
                    File.Delete(filepath);
                DownloadImageFile(ThreeD1Navigation, filepath, "png");
                colorsplittedpath = Config.downloadpath + "\\" + testid + "_Step28_1.png";
                if (File.Exists(colorsplittedpath))
                    File.Delete(colorsplittedpath);
                z3dvp.yellowcolorsplitter(filepath, colorsplittedpath);
                yellowquadpoints = z3dvp.ImageQuadPoints(colorsplittedpath);
                new Actions(Driver).MoveToElement(ThreeD1Navigation, Convert.ToInt32(yellowquadpoints[1].X), Convert.ToInt32(yellowquadpoints[1].Y)).ClickAndHold().
                    MoveToElement(ThreeD1Navigation, Convert.ToInt32(yellowquadpoints[1].X), ThreeD1Navigation.Size.Height - 10).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                locationvalue3D1After = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                LocationValueVerification = locationvalue3D1After.Equals(locationvalue3D1Before);
                if (!LocationValueVerification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step 29
                res = z3dvp.select3DTools(Z3DTools.Reset);
                if (!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                {
                    PageLoadWait.WaitForFrameLoad(10);
                    String locationvalue3D1Afterreset = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                    LocationValueVerification = locationvalue3D1Afterreset.Equals(locationvalue3D1Before);
                    if (LocationValueVerification)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }

                //Step 30
                bool step30 = z3dvp.VerifyHighLightedBorder_ParticularVieport(BluRingZ3DViewerPage.Navigation3D1, "rgb(211, 211, 211)");
                if (step30)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 31
                Viewport = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                new Actions(Driver).MoveToElement(Viewport, Viewport.Size.Width / 4, Viewport.Size.Height / 4).Click().Build().Perform();
                int yellowcolorbefore31 = z3dvp.LevelOfSelectedColor(Viewport, testid, ExecutedSteps + 311, 255, 255, 0, 2);
                baseimagepath = BaseImages + Path.DirectorySeparatorChar + testid + "_" + ExecutedSteps + 312 + ".png";
                colorsplittedpath = ColorSplitImages + Path.DirectorySeparatorChar + testid + "_" + ExecutedSteps + 313 + ".png";
                DownloadImageFile(Viewport, baseimagepath, "png");
                z3dvp.yellowcolorsplitter(baseimagepath, colorsplittedpath);
                IList<IntPoint> quadrilateral2 = z3dvp.ImageQuadPoints(colorsplittedpath, 0);
                //yellowintersection = z3dvp.GetIntersectionPoints(Viewport, testid, ExecutedSteps + 142, "yellow", "Vertical", 0);
                new Actions(Driver).MoveToElement(ThreeD1Navigation, Convert.ToInt32(quadrilateral2[0].X), Convert.ToInt32(quadrilateral2[0].Y)).Click().Build().Perform();
                int yellowcolorafter31 = z3dvp.LevelOfSelectedColor(Viewport, testid, ExecutedSteps + 314, 255, 255, 0, 2);
                if (yellowcolorafter31 > yellowcolorbefore31)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step:32
                Viewport = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                orientationvalue3D1before = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                new Actions(Driver).MoveToElement(Viewport, Convert.ToInt32(quadrilateral2[0].X), Convert.ToInt32(quadrilateral2[0].Y)).ClickAndHold().
                    MoveToElement(Viewport, Convert.ToInt32(yellowquadpoints[1].X), Convert.ToInt32(yellowquadpoints[1].Y)).
                    MoveToElement(Viewport, Viewport.Size.Width - 5, Convert.ToInt32(quadrilateral2[0].Y)).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                orientationvalue3D1after = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                if (!orientationvalue3D1before.Equals(orientationvalue3D1after))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();


                /** /Step 31
                Viewport = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                new Actions(Driver).MoveToElement(Viewport, Viewport.Size.Width / 4, Viewport.Size.Height / 4).Click().Build().Perform();
                int yellowcolorbefore31 = z3dvp.LevelOfSelectedColor(Viewport, testid, ExecutedSteps + 311, 255, 255, 0, 2);
                yellowintersection = z3dvp.GetIntersectionPoints(Viewport, testid, ExecutedSteps + 312, "yellow", "Vertical", 0);
                new Actions(Driver).MoveToElement(ThreeD1Navigation, Convert.ToInt32(yellowintersection.X), Convert.ToInt32(yellowintersection.Y)).Click().Build().Perform();
                int yellowcolorafter31 = z3dvp.LevelOfSelectedColor(Viewport, testid, ExecutedSteps + 313, 255, 255, 0, 2);
                if (yellowcolorafter31 > yellowcolorbefore31)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step 32
                Viewport = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                orientationvalue3D1before = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                new Actions(Driver).MoveToElement(Viewport, Convert.ToInt32(yellowintersection.X), Convert.ToInt32(yellowintersection.Y)).ClickAndHold().
                    MoveToElement(Viewport, Convert.ToInt32(yellowquadpoints[1].X), Convert.ToInt32(yellowquadpoints[1].Y)).
                    MoveToElement(Viewport, Viewport.Size.Width - 5, Convert.ToInt32(yellowintersection.Y)).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                orientationvalue3D1after = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                if (!orientationvalue3D1before.Equals(orientationvalue3D1after))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();/**/

                //Step 33
                z3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                orientationvalue3D1afterreset = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                if (orientationvalue3D1afterreset.Equals(orientationvalue3D1before))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step 34
                res = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigation3D1);
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step 35
                locationvalue3D1Before = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                orientationvalue3D1before = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                ThreeD1Navigation = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                new Actions(Driver).MoveToElement(ThreeD1Navigation, ThreeD1Navigation.Size.Width / 4, (ThreeD1Navigation.Size.Height) - 5).ClickAndHold().
                    MoveToElement(ThreeD1Navigation, ThreeD1Navigation.Size.Width / 4, 5).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                locationvalue3D1After = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                orientationvalue3D1after = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                Orientationvalueverification = orientationvalue3D1before.Equals(orientationvalue3D1after);
                LocationValueVerification = locationvalue3D1After.Equals(locationvalue3D1Before);
                if (!Orientationvalueverification && !LocationValueVerification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step 36
                res = z3dvp.select3DTools(Z3DTools.Reset);
                if (res)
                {
                    PageLoadWait.WaitForFrameLoad(10);
                    String locationvalue3D1Afterreset = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                    orientationvalue3D1afterreset = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                    Orientationvalueverification = orientationvalue3D1before.Equals(orientationvalue3D1afterreset);
                    LocationValueVerification = locationvalue3D1Afterreset.Equals(locationvalue3D1Before);
                    if (Orientationvalueverification && LocationValueVerification)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step 37
                locationvalue3D1Before = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                orientationvalue3D1before = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                ThreeD1Navigation = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                new Actions(Driver).MoveToElement(ThreeD1Navigation, ThreeD1Navigation.Size.Width / 4, (ThreeD1Navigation.Size.Height / 2) + 10).ClickAndHold().
                    MoveToElement(ThreeD1Navigation, 3 * (ThreeD1Navigation.Size.Width / 4), (ThreeD1Navigation.Size.Height / 2) + 10).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                locationvalue3D1After = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                orientationvalue3D1after = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                Orientationvalueverification = orientationvalue3D1before.Equals(orientationvalue3D1after);
                LocationValueVerification = locationvalue3D1After.Equals(locationvalue3D1Before);
                if (!Orientationvalueverification && !LocationValueVerification)
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
                z3dvp.CloseViewer();
                login.Logout();
            }

        }

        public TestCaseResult Test_163233(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            Studies studies = new Studies();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string[] patientids = Patientid.Split('|');
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string TestRequirement = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            string[] searchvalue = TestRequirement.Split('|');
            String[] Thumbnail = ImageCount.Split('|');

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step:1  Login iCA as Administrator. 
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
                //Step:2
                bool launchStudy =  z3dvp.searchandopenstudyin3D(searchvalue[1], Thumbnail[0] , BluRingZ3DViewerPage.MPR , field: searchvalue[0], thumbnailcount: Convert.ToInt32(searchvalue[2]));
                if (launchStudy)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    throw new Exception("Unable to load study successfully");
                }
                //Step:3
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
                //Step:4 :: Hover top of the viewport 3D 1 control and select toggle 3D/MPR in order to display the 3D navigation controls.
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

                //Step:5 :: 3D interactive quality should be saved.
                bool step5 = z3dvp.change3dsettings(BluRingZ3DViewerPage.InteractiveQuality3D, 100);
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

                //Step:6
                z3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                bool scrollTool = z3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigation3D1, 5, 5, 100);
                z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center);
                bool RotateTool = z3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigation3D1, 5, 5, 100);
                z3dvp.select3DTools(Z3DTools.Window_Level);
                bool WindowTool = z3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigation3D1, 5, 5, 100);
                z3dvp.select3DTools(Z3DTools.Interactive_Zoom);
                bool zoomtool = z3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigation3D1, 5, 5, 100);
                if (scrollTool && RotateTool && WindowTool && zoomtool)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:7
                bool step7 = z3dvp.change3dsettings(BluRingZ3DViewerPage.InteractiveQuality3D, 50);
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

                //Step:8
                z3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                bool scrollTool1 = z3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigation3D1, 5, 5, 100);
                z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center);
                bool RotateTool1 = z3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigation3D1, 5, 5, 100);
                z3dvp.select3DTools(Z3DTools.Window_Level);
                bool WindowTool1 = z3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigation3D1, 5, 5, 100);
                z3dvp.select3DTools(Z3DTools.Interactive_Zoom);
                bool zoomtool1 = z3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigation3D1, 5, 5, 100);
                if (scrollTool1 && RotateTool1 && WindowTool1 && zoomtool1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:9 
                z3dvp.CloseViewer();
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                LogoutBtn = login.LogoutBtn();
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
                //Step 10
                bool LaunchSecondStudy = z3dvp.searchandopenstudyin3D(patientids[1], Thumbnail[2]);
                if (LaunchSecondStudy)
                {
                  
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                   
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:11
               bool ThreeD6x1 =  z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
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

                //Step:12
                bool step12 = z3dvp.ChangeViewMode(BluRingZ3DViewerPage.Navigation3D1);
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

                //Step:13
                bool step13 = z3dvp.change3dsettings(BluRingZ3DViewerPage.InteractiveQuality3D, 100);
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

                //Step:14
                z3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                bool scrollTool2 = z3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigation3D1, 5, 5, 5);
                z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center);
                bool RotateTool2 = z3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigation3D1, 5, 5, 5);
                z3dvp.select3DTools(Z3DTools.Window_Level);
                bool WindowTool2 = z3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigation3D1, 5, 5, 5);
                z3dvp.select3DTools(Z3DTools.Interactive_Zoom);
                bool zoomtool2 = z3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigation3D1, 5, 5, 5);
                if (!scrollTool2 && !RotateTool2 && !WindowTool2 && !zoomtool2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:15
                bool step17 = z3dvp.change3dsettings(BluRingZ3DViewerPage.InteractiveQuality3D, 50);
                if (step17)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:16
                z3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                bool scrollTool3 = z3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigation3D1, 5, 5, 100);
                z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center);
                bool RotateTool3 = z3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigation3D1, 5, 5, 100);
                z3dvp.select3DTools(Z3DTools.Window_Level);
                bool WindowTool3 = z3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigation3D1, 5, 5, 100);
                z3dvp.select3DTools(Z3DTools.Interactive_Zoom);
                bool zoomtool3 = z3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigation3D1, 5, 5, 100);
                if (scrollTool3 && RotateTool3 && WindowTool3 && zoomtool3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:17
                z3dvp.CloseViewer();
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                LogoutBtn = login.LogoutBtn();
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


        public TestCaseResult Test_163232(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string[] patientids = Patientid.Split('|');
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string TestRequirement = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            string[] searchvalue = TestRequirement.Split('|');
            String[] Thumbnail = ImageCount.Split('|');

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step:1 - Study is loaded in the universal viewer without any errors
                login.LoginIConnect(Config.adminUserName, Config.adminPassword); 
                Boolean step1 = z3dvp.searchandopenstudyin3D(patientids[0], Thumbnail[0]);
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
                //Step:2 - 3D 4:1 viewing mode should be displayed .
                bool step2 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
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

                //Step:3 - From the hover bar, Click on the preset drop down From 3D1 control
                //Step:4  -Selected preset is applied to the image in 3D 1 control
                
                bool step4 = z3dvp.VerifyAllPreset(testid, BluRingZ3DViewerPage.Navigation3D1, start:0, end: 6);
                if (step4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                }

                //Step:5 Scroll through the volume and verify the images in 3D 1 control.
                z3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                IWebElement Viewport = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                z3dvp.ScrollInView(BluRingZ3DViewerPage.Navigation3D1, scrolllevel: 50, Thickness: "N");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Viewport, pixelTolerance: 100))
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

                //Step:6 - Close the study. Search and load a MR modality study that has 3D supported series.
                //z3dvp.ExitIcon().Click();
                // ClickElement(z3dvp.ExitIcon());
                z3dvp.CloseViewer();
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step6 =  z3dvp.searchandopenstudyin3D(patientids[1], Thumbnail[1]);
                if(step6)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:7  
                bool step7 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
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

                //Step:8 - Click on the preset drop down from 3D2 control Hover bar options.
                //step:9  -Selected preset is applied to the image in 3D 2 control
                bool step9 = z3dvp.VerifyAllPreset(testid, BluRingZ3DViewerPage.Navigation3D1, start: 8, end: 14);
                if (step9)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                }

                //step:10  - 	Scroll through the volume and verify the images in 3D2 control.
                z3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                Viewport = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                z3dvp.ScrollInView(BluRingZ3DViewerPage.Navigation3D2, scrolllevel: 50, Thickness: "N");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Viewport, pixelTolerance: 100))
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

                //Step:11  -Close the study. Search and load a PT modality study that has 3D supported serie
                //ClickElement(z3dvp.ExitIcon());
                z3dvp.CloseViewer();
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step11 =  z3dvp.searchandopenstudyin3D(patientids[2], Thumbnail[2]);
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


                //Step:12
                bool step12 = z3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
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


                //step: 13 - Create a manual path by adding the points in MPR navigation control images.
                new Actions(Driver).SendKeys("X").Build().Perform();
                IWebElement Navigation1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                int BluColorBeforePoint = z3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 39, 0, 0, 255, 2);
                new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2 + 20, 60).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2 + 20, 90).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                int BluColorAfterPoint1 = z3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 40, 0, 0, 255, 2, true);
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

                //Step:14 - From the hover bar, Click on the preset drop down From 3D1 control
                //Step:15  -Selected preset is applied to the image in 3D 1 control
                String[] presetarray1 = { "Bone & Lung", "Bone & Minimal Vessels", "Bone & Vessels Bright /MRA-CTA", "Bone & Vessels-Dark Background A", "Bone & Vessels-Dark Background B", "Bone & Vessels Orange /MRA-CTA", "Bone & Vessels Red /MRA-CTA", "Bones Transulent & Metal", "Cardiac /MRA-CTA", "MRA-A", "MR-B" };
                String GoldImages = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + "TempImages" + Path.DirectorySeparatorChar + "BeforeImage" + Path.DirectorySeparatorChar + Config.BrowserType;
                Directory.CreateDirectory(GoldImages);
                String TestImages = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + "TempImages" + Path.DirectorySeparatorChar + "CompareImage" + Path.DirectorySeparatorChar + Config.BrowserType;
                Directory.CreateDirectory(TestImages);
                String goldimagepath = GoldImages + Path.DirectorySeparatorChar + testid + "_" + (ExecutedSteps + 1).ToString() + "_Before_15thstep.png";
                String testimagepath = TestImages + Path.DirectorySeparatorChar + testid + "_" + (ExecutedSteps + 1).ToString() + "_After_15thStep.png";
                if (File.Exists(goldimagepath))
                    File.Delete(goldimagepath);
                if (File.Exists(testimagepath))
                    File.Delete(testimagepath);
                IWebElement ViewerPane = z3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                Logger.Instance.InfoLog("Gold Image Path" + goldimagepath);
                Logger.Instance.InfoLog("Test Image Path" + testimagepath);
                new Actions(Driver).MoveToElement(ViewerPane, ViewerPane.Size.Width / 4, ViewerPane.Size.Height / 4).Build().Perform();
                Thread.Sleep(2000);
                DownloadImageFile(ViewerPane, goldimagepath, "png");
                Thread.Sleep(2000);
                int counter = 0;
                foreach(String presetval in presetarray1)
                {
                    bool res = z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage._3DPathNavigation, presetval, "Preset");
                    if (res)
                        counter++;
                }
                Thread.Sleep(2000);
                new Actions(Driver).MoveToElement(ViewerPane, ViewerPane.Size.Width / 4, ViewerPane.Size.Height / 4).Build().Perform();
                Thread.Sleep(2000);
                DownloadImageFile(ViewerPane, testimagepath, "png");
                Thread.Sleep(2000);
                if (!CompareImage(goldimagepath, testimagepath) && counter <= presetarray1.Length)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step:16 - Scroll through the volume and verify the images in 3D path navigation control.
                z3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                Viewport = z3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                z3dvp.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, scrolllevel: 50, Thickness: "N");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Viewport, pixelTolerance: 100))
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

                //step:17  -Select the MPR option from the smart view drop down.
                bool step17 = z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR);
                if (step17)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:18 - From the MPR result control hover bar, Select the render type to 3D slab
                Viewport = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Slab3D);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Viewport, pixelTolerance: 100))
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


                //Step:19 - From the hover bar, Click on the preset drop down From 3D1 control
                //Step:20  -Selected preset is applied to the image in 3D 1 control
                String[] presetarray2 = { "Organs & Lung", "PET", "3D X-ray B", "PET B", "Skin & Monochrome Sharp", "Skin & Monochrome Soft", "Soft Tissue Orange", "3D Colon, Trachea, Colon Outline", "3D Translucent Red", "3D X-ray A" };
                goldimagepath = GoldImages + Path.DirectorySeparatorChar + testid + (ExecutedSteps + 1).ToString() + "_Before_20thstep.png";
                testimagepath = TestImages + Path.DirectorySeparatorChar + testid + (ExecutedSteps + 1).ToString() + "_After_20thStep.png";
                if (File.Exists(goldimagepath))
                    File.Delete(goldimagepath);
                if (File.Exists(testimagepath))
                    File.Delete(testimagepath);
                ViewerPane = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                Logger.Instance.InfoLog("Gold Image Path" + goldimagepath);
                Logger.Instance.InfoLog("Test Image Path" + testimagepath);
                new Actions(Driver).MoveToElement(ViewerPane, ViewerPane.Size.Width / 4, ViewerPane.Size.Height / 4).Build().Perform();
                Thread.Sleep(2000);
                DownloadImageFile(ViewerPane, goldimagepath, "png");
                Thread.Sleep(2000);
                int counter2 = 0;
                foreach (String presetval in presetarray2)
                {
                    bool res = z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, presetval, "Preset");
                    if (res)
                        counter++;
                }
                Thread.Sleep(2000);
                new Actions(Driver).MoveToElement(ViewerPane, ViewerPane.Size.Width / 4, ViewerPane.Size.Height / 4).Build().Perform();
                Thread.Sleep(2000);
                DownloadImageFile(ViewerPane, testimagepath, "png");
                Thread.Sleep(2000);
                if (!CompareImage(goldimagepath, testimagepath) && counter2 <= presetarray2.Length)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step:21 - Scroll through the volume and verify the images in 3D path navigation control.
                z3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                Viewport = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                z3dvp.ScrollInView(BluRingZ3DViewerPage.ResultPanel, scrolllevel: 50, Thickness: "N");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Viewport, pixelTolerance: 100))
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


        public TestCaseResult Test_163231(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string[] patientids = Patientid.Split('|');
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string TestRequirement = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            string[] searchvalue = TestRequirement.Split('|');
            String[] Thumbnail = ImageCount.Split('|');

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step:1 & 2 - PreCondition & Login iCA 
                login.DriverGoTo(url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                result.steps[++ExecutedSteps].StepPass();
                result.steps[++ExecutedSteps].StepPass();
                result.steps[++ExecutedSteps].StepPass();
                //Step4  
                Boolean step4 = z3dvp.searchandopenstudyin3D(searchvalue[1], Thumbnail[0], BluRingZ3DViewerPage.MPR , field: searchvalue[0]);
                if (step4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *4 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Step1-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step:5
                bool step5 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                if (step5)
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

                //Step:6
                IWebElement Viewport = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                bool step6 = z3dvp.EnableOneViewupMode(Viewport);
                if (step6)
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

                //Step:7
                String Step7_AnnotationValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step7 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D1, "H" , "P" , "dragdown");
                if (step7)
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

                //Step:8
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool  step8 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D1, "P", "A", "dragup");
                if (step8)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *8 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("step 8 Value :" + step8 + "And step 7 Value is " + step7);
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:9
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step9 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D1, "L", "R", "dragright");
                if (step9)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *9 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:10
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step10 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D1, "R", "L", "dragleft");
                if (step10)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *10 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("step 10 Value :" + step10 + "And step 9 Value is " + step9);
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:11
                z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigation3D1);
                String Step11_AnnotationValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                if (Step7_AnnotationValue.Equals(Step11_AnnotationValue))
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
                //Step:12
                //Repeat steps 5-10 on all 3D controls on all views. 
                z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                z3dvp.ChangeViewMode(BluRingZ3DViewerPage.Navigation3D1);
                Viewport = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                bool step12 =  z3dvp.EnableOneViewupMode(Viewport);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step12_1 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D2, "H", "P", "dragdown");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step12_2 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D2, "P", "A", "dragup");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                bool step12_3 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D2, "L", "R", "dragright");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
                bool step12_4 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D2, "R", "L", "dragleft");
                if (step12 && step12_1 && step12_2 && step12_3 && step12_4)
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
                //Step:13
                z3dvp.CloseViewer();
                Boolean step13 = z3dvp.searchandopenstudyin3D(searchvalue[2], Thumbnail[0], field: searchvalue[0]);
                if (step13)
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
                //Step:14
                bool step14 = z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR);
                if (step14)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *14 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step:15
                bool step15 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                if (step15)
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
                //Step:16
                IWebElement Viewport2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                bool step16 = z3dvp.EnableOneViewupMode(Viewport2);
                if (step16)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *16 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:17
                String Bef_AnnotationValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool  step17 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D1, "H", "P", "dragdown");
                if (step17)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *17 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:18
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step18 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D1, "P", "A", "dragup");
                if (step18)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *18 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("step 18 Value :" + step18 + "And step 17 Value is " + step17);
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:19
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step19 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D1, "L", "R", "dragright");
                if (step19)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *19 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:20
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step20 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D1, "R", "L", "dragleft");
                if (step20)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *20 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:21
                z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigation3D1);
                String Step21_AnnotationValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                if (Bef_AnnotationValue.Equals(Step21_AnnotationValue))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *21 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:22
                //Repeat steps 15 - 20 on all 3D controls on all views. 
                z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                z3dvp.ChangeViewMode(BluRingZ3DViewerPage.Navigation3D1);
                Viewport = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                bool step22 = z3dvp.EnableOneViewupMode(Viewport);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step22_1 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D2, "H", "P", "dragdown");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step22_2 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D2, "P", "A", "dragup");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                bool step22_3 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D2, "L", "R", "dragright");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
                bool step22_4 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D2, "R", "L", "dragleft");
                if (step22 && step22_1 && step22_2 && step22_3 && step22_4)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *22 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:23
                z3dvp.CloseViewer();
                Boolean step23 = z3dvp.searchandopenstudyin3D(searchvalue[3], Thumbnail[0], field: searchvalue[0]);
                if (step23)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *23 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:24
                bool step24 = z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR);
                if (step24)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *24 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:25
                bool step25 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                if (step25)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *25 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step:26
                IWebElement Viewport3 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                bool step26 = z3dvp.EnableOneViewupMode(Viewport3);
                if (step26)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *26 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:27
                String Bef_AnnotationValue1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step27 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D1, "H" , "P" ,"dragdown");
                if (step27)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *27 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:28
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step28 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D1, "P" , "A" , "dragup");
                if (step28)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *28 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("step 28 Value :" + step28 + "And step 27 Value is " + step27);
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:29
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step29 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D1, "L" , "R" , "dragright");
                if (step29)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *29 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:30
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step30 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D1, "R" , "L" , "dragleft");
                if (step30)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *30 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:31
                z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigation3D1);
                String Step31_AnnotationValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                if (Bef_AnnotationValue1.Equals(Step31_AnnotationValue))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *31 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:32
                //Repeat steps 15 - 20 on all 3D controls on all views. 
                z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                z3dvp.ChangeViewMode(BluRingZ3DViewerPage.Navigation3D1);
                Viewport = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                bool step32 = z3dvp.EnableOneViewupMode(Viewport);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step32_1 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D2, "H", "P", "dragdown");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step32_2 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D2, "P", "A", "dragup");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                bool step32_3 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D2, "L", "R", "dragright");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
                bool step32_4 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D2, "R", "L", "dragleft");
                if (step32 && step32_1 && step32_2 && step32_3 && step32_4)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *32 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:33
                z3dvp.CloseViewer();
                Boolean step33 = z3dvp.searchandopenstudyin3D(searchvalue[4], Thumbnail[0], field: searchvalue[0]);
                if (step33)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *33 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:34
                bool step34 = z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR);
                if (step34)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *34 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:35
                bool step35 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                if (step35)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *35 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:36
                IWebElement Viewport4 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                bool step36 = z3dvp.EnableOneViewupMode(Viewport4);
                if (step36)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *36 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:37
                String Bef_AnnotationValue2 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step37 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D1, "H" , "P" , "dragdown");
                if (step37)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *37 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:38
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step38 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D1, "P" , "A" , "dragup");
                if (step38)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *38 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("step 28 Value :" + step28 + "And step 27 Value is " + step27);
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:39
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step39 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D1, "L" , "R" , "dragright");
                if (step39)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *39 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:40
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step40 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D1, "R" , "L" , "dragleft");
                if (step40)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *40 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:41
                z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigation3D1);
                String Step41_AnnotationValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                if (Bef_AnnotationValue2.Equals(Step41_AnnotationValue))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *41 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:42
                //Repeat steps 35-40 on all 3D controls on all views. 
                z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                z3dvp.ChangeViewMode(BluRingZ3DViewerPage.Navigation3D1);
                Viewport = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                bool step42 = z3dvp.EnableOneViewupMode(Viewport);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step42_1 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D2, "H", "P", "dragdown");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step42_2 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D2, "P", "A", "dragup");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                bool step42_3 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D2, "L", "R", "dragright");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
                bool step42_4 = z3dvp.DragAndCheckOrientation(result.steps[ExecutedSteps], BluRingZ3DViewerPage.Navigation3D2, "R", "L", "dragleft");
                if (step42 && step42_1 && step42_2 && step42_3 && step42_4)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *42 Passed--" + result.steps[ExecutedSteps].description);
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
     
           
    }
}
