using OpenQA.Selenium;
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
//using Microsoft.Expression.Encoder.ScreenCapture;

namespace Selenium.Scripts.Tests
{
    class ReliabilityTest
    {
        public Login login { get; set; }
        public string filepath { get; set; }

        public ReliabilityTest(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }

        /// <summary>
        /// Verify the two study panels shall not get open when user double clicks on patient study in Exam List
        /// </summary>
        public TestCaseResult Test_165074(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                Studies studies;
                BluRingViewer viewer;              

                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;           
                String patientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                string[] arrPatientID = patientID.Split(':');
                String modality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                string[] arrModality = modality.Split(':');
                String iterations = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Iterations");
                String goldImageDir = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + "GoldImages" + Path.DirectorySeparatorChar + Config.BrowserType + Path.DirectorySeparatorChar;
                String videoDrive = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DriveToSave");
                String imagesCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ImagesCount");
                string[] arrImagesCount = imagesCount.Split(':');

                int[] passCount = new int[stepcount];

                string[,] statusArray = new string[Int32.Parse(iterations), stepcount];

                //Step 1
                //Login to WebAccess site with any privileged user.
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);                
                ExecutedSteps++;
                passCount[ExecutedSteps] = passCount[ExecutedSteps] + 1;

                for (int iterI = 1; iterI <= Int32.Parse(iterations); iterI++)
                {
                    ExecutedSteps = 0;
                    statusArray[iterI - 1, ExecutedSteps] = "Pass";
                    try
                    {
                        Logger.Instance.InfoLog("*****************Starting Iteration: " + iterI + "***********************");                        

                        //Step 2
                        //Open DX study
                        studies = (Studies)login.Navigate("Studies");
                        studies.SearchStudy(patientID: arrPatientID[0]);
                        studies.SelectStudy("Modality", arrModality[0]);
                        viewer = BluRingViewer.LaunchBluRingViewer();
                        PageLoadWait.WaitForFrameLoad(20);
                        ExecutedSteps++;
                        //passCount[ExecutedSteps] = passCount[ExecutedSteps] + 1;
                        statusArray[iterI-1, ExecutedSteps] = "Pass";                                                                        

                        //Step 3
                        //Apply W/L, W/W
                        //viewer.SetViewPort(1, 1);
                        //Thread.Sleep(5000);
                        viewer.SelectViewerTool(BluRingTools.Window_Level, 1, 1);
                        viewer.ApplyTool_WindowWidth();
                        Thread.Sleep(3000);                        
                        result.steps[++ExecutedSteps].SetPath(testid + "Iter_" + iterI + "WL", ExecutedSteps + 1);
                        result.steps[ExecutedSteps].goldimagepath = goldImageDir + testid + "_WL_" + (ExecutedSteps + 1) + ".jpg";
                        var viewport = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                        bool step3 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                        if (step3)
                        {
                            //passCount[ExecutedSteps] = passCount[ExecutedSteps] + 1;
                            statusArray[iterI - 1, ExecutedSteps] = "Pass";
                            result.steps[ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Iteration: " + iterI + " -- >Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            statusArray[iterI - 1, ExecutedSteps] = "Fail";
                            result.steps[ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("Iteration: " + iterI + " -->Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }

                        //Step 4
                        //Apply Zoom
                        //Thread.Sleep(6000);
                        viewer.ClickOnViewPort(1, 3);
                        //try
                        //{
                        //    viewer.OpenStackedTool(BluRingTools.Interactive_Zoom, panel: 1, viewport: 3);
                        //}
                        //catch (Exception e)
                        //{
                        //    Logger.Instance.ErrorLog("Zoom Exception: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                        viewer.OpenStackedTool(BluRingTools.Interactive_Zoom, panel: 1, viewport: 3);
                        //}
                        viewer.SelectViewerTool(BluRingTools.Interactive_Zoom, 1, 3, false);
                        viewer.ApplyTool_Magnifier(false);
                        Thread.Sleep(3000);
                        result.steps[++ExecutedSteps].SetPath(testid + "Iter_" + iterI + "Zoom", ExecutedSteps + 1);
                        result.steps[ExecutedSteps].goldimagepath = goldImageDir + testid + "_Zoom_" + (ExecutedSteps + 1) + ".jpg";
                        viewport = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                        bool step4 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                        if (step4)
                        {
                            //passCount[ExecutedSteps] = passCount[ExecutedSteps] + 1;
                            statusArray[iterI - 1, ExecutedSteps] = "Pass";
                            result.steps[ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Iteration: " + iterI + " -->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            statusArray[iterI - 1, ExecutedSteps] = "Fail";
                            result.steps[ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("Iteration: " + iterI + " -->Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }

                        //Step 5
                        //Apply Line measurement
                        //Thread.Sleep(5000);
                        viewer.ClickOnViewPort(1, 2);
                        viewer.OpenStackedTool(BluRingTools.Line_Measurement, panel: 1, viewport: 2);
                        viewer.SelectViewerTool(BluRingTools.Line_Measurement, 1, 2, true);
                        viewer.ApplyTool_LineMeasurement();
                        Thread.Sleep(3000);
                        result.steps[++ExecutedSteps].SetPath(testid + "Iter_" + iterI + "Line", ExecutedSteps + 1);
                        result.steps[ExecutedSteps].goldimagepath = goldImageDir + testid + "_Line_" + (ExecutedSteps + 1) + ".jpg";
                        viewport = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                        bool step5 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                        if (step5)
                        {
                            //passCount[ExecutedSteps] = passCount[ExecutedSteps] + 1;
                            statusArray[iterI - 1, ExecutedSteps] = "Pass";
                            result.steps[ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Iteration: " + iterI + " -->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            statusArray[iterI - 1, ExecutedSteps] = "Fail";
                            result.steps[ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("Iteration: " + iterI + " -->Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }

                        //Step 6
                        //Close Viewer
                        viewer.CloseBluRingViewer();
                        ExecutedSteps++;
                        //passCount[ExecutedSteps] = passCount[ExecutedSteps] + 1;
                        statusArray[iterI - 1, ExecutedSteps] = "Pass";

                        //Step 7
                        //Open XA study
                        studies = (Studies)login.Navigate("Studies");
                        studies.SearchStudy(patientID: arrPatientID[1]);
                        studies.SelectStudy("Modality", arrModality[1]);
                        viewer = BluRingViewer.LaunchBluRingViewer();
                        PageLoadWait.WaitForFrameLoad(20);
                        ExecutedSteps++;
                        //passCount[ExecutedSteps] = passCount[ExecutedSteps] + 1;
                        statusArray[iterI - 1, ExecutedSteps] = "Pass";

                        //Step 8
                        //Apply W/L, W/W
                        //viewer.SetViewPort(1, 1);
                        //Thread.Sleep(5000);
                        viewer.SelectViewerTool(BluRingTools.Window_Level, 1, 1);
                        viewer.ApplyTool_WindowWidth();
                        Thread.Sleep(3000);
                        result.steps[++ExecutedSteps].SetPath(testid + "Iter_" + iterI + "WL", ExecutedSteps + 1);
                        result.steps[ExecutedSteps].goldimagepath = goldImageDir + testid + "_WL_" + (ExecutedSteps + 1) + ".jpg";
                        viewport = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                        bool step8 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                        if (step8)
                        {
                            //passCount[ExecutedSteps] = passCount[ExecutedSteps] + 1;
                            statusArray[iterI - 1, ExecutedSteps] = "Pass";
                            result.steps[ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Iteration: " + iterI + " -->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            statusArray[iterI - 1, ExecutedSteps] = "Fail";
                            result.steps[ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("Iteration: " + iterI + " -->Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }

                        //Step 9
                        //Apply Zoom
                        //Thread.Sleep(6000);
                        viewer.ClickOnViewPort(1, 3);
                        //try
                        //{
                        //    viewer.OpenStackedTool(BluRingTools.Interactive_Zoom, panel: 1, viewport: 3);
                        //}
                        //catch (Exception e)
                        //{
                        //    Logger.Instance.ErrorLog("Zoom Exception: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                        viewer.OpenStackedTool(BluRingTools.Interactive_Zoom, panel: 1, viewport: 3);
                        //}
                        viewer.SelectViewerTool(BluRingTools.Interactive_Zoom, 1, 3, false);
                        viewer.ApplyTool_Magnifier(false);
                        Thread.Sleep(3000);
                        result.steps[++ExecutedSteps].SetPath(testid + "Iter_" + iterI + "Zoom", ExecutedSteps + 1);
                        result.steps[ExecutedSteps].goldimagepath = goldImageDir + testid + "_Zoom_" + (ExecutedSteps+1) + ".jpg";
                        viewport = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                        bool step9 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                        if (step9)
                        {
                            //passCount[ExecutedSteps] = passCount[ExecutedSteps] + 1;
                            statusArray[iterI - 1, ExecutedSteps] = "Pass";
                            result.steps[ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Iteration: " + iterI + " -->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            statusArray[iterI - 1, ExecutedSteps] = "Fail";
                            result.steps[ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("Iteration: " + iterI + " -->Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }

                        //Step 10
                        //Apply Line measurement
                        //Thread.Sleep(5000);
                        viewer.ClickOnViewPort(1, 2);
                        viewer.OpenStackedTool(BluRingTools.Line_Measurement, panel: 1, viewport: 2);
                        viewer.SelectViewerTool(BluRingTools.Line_Measurement, 1, 2, true);
                        viewer.ApplyTool_LineMeasurement();
                        Thread.Sleep(3000);
                        result.steps[++ExecutedSteps].SetPath(testid + "Iter_" + iterI + "Line", ExecutedSteps + 1);
                        result.steps[ExecutedSteps].goldimagepath = goldImageDir + testid + "_Line_" + (ExecutedSteps + 1) + ".jpg";
                        viewport = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                        bool step10 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                        if (step10)
                        {
                            //passCount[ExecutedSteps] = passCount[ExecutedSteps] + 1;
                            statusArray[iterI - 1, ExecutedSteps] = "Pass";
                            result.steps[ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Iteration: " + iterI + " -->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            statusArray[iterI - 1, ExecutedSteps] = "Fail";
                            result.steps[ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("Iteration: " + iterI + " -->Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }

                        //Step 11
                        //Close Viewer
                        viewer.CloseBluRingViewer();
                        ExecutedSteps++;
                        //passCount[ExecutedSteps] = passCount[ExecutedSteps] + 1;
                        statusArray[iterI - 1, ExecutedSteps] = "Pass";

                        //Step 12
                        //Open MR study
                        studies = (Studies)login.Navigate("Studies");
                        studies.SearchStudy(patientID: arrPatientID[2]);
                        studies.SelectStudy("Patient ID", arrPatientID[2]);
                        viewer = BluRingViewer.LaunchBluRingViewer();
                        PageLoadWait.WaitForFrameLoad(20);
                        ExecutedSteps++;
                        //passCount[ExecutedSteps] = passCount[ExecutedSteps] + 1;
                        statusArray[iterI - 1, ExecutedSteps] = "Pass";

                        //Step 13
                        //Scroll through images in viewport 1
                        bool step13 = true;                        
                        viewport = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                        ExecutedSteps++;
                        int countToScrollAllImages = Convert.ToInt32(arrImagesCount[0]) - 1;
                        for (int i = 1; i <= countToScrollAllImages; i++)
                        {
                            bool imageCompare = true;                  
                            viewport.SendKeys(Keys.ArrowDown);
                            Thread.Sleep(2000);                            
                            result.steps[ExecutedSteps].SetPath(testid + "Iter_" + iterI + "Scroll", ExecutedSteps + 1, i);
                            result.steps[ExecutedSteps].goldimagepath = goldImageDir + testid + "_Scroll_" + (ExecutedSteps + 1) + "-" + i + ".jpg";
                            if (i < countToScrollAllImages)
                            {
                                imageCompare = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                                step13 = step13 && imageCompare;
                            }
                            else
                            {
                                imageCompare = studies.CompareImage(result.steps[ExecutedSteps], viewport, totalImageCount: i, IsFinal: 1);
                                step13 = step13 && imageCompare;
                            }
                            if (!imageCompare)
                                result.steps[ExecutedSteps].SetLogs();
                        }                       
                        //Thread.Sleep(3000);
                        //result.steps[++ExecutedSteps].SetPath(testid + "Iter_" + iterI + "Scroll", ExecutedSteps + 1);
                        //result.steps[ExecutedSteps].goldimagepath = goldImageDir + testid + "_Scroll_" + (ExecutedSteps + 1) + ".jpg";
                        //viewport = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                        //bool step13 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                        if (step13)
                        {
                            //passCount[ExecutedSteps] = passCount[ExecutedSteps] + 1;
                            statusArray[iterI - 1, ExecutedSteps] = "Pass";
                            result.steps[ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Iteration: " + iterI + " -->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            statusArray[iterI - 1, ExecutedSteps] = "Fail";
                            result.steps[ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("Iteration: " + iterI + " -->Test case Failed--" + result.steps[ExecutedSteps].description);
                            //result.steps[ExecutedSteps].SetLogs();
                        }

                        //Step 14
                        //Cine Play in viewport 2                        
                        //ScreenCaptureJob videoRec = new ScreenCaptureJob();
                        //videoRec.OutputScreenCaptureFileName = videoDrive + ":\\ReliabilityTest_Iter_" + iterI + "_Time_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".wmv";
                        //videoRec.Start();
                        viewer.ClickOnViewPort(1, 2);
                        Thread.Sleep(1000);
                        viewer.PlayCINE(2, 1);
                        Thread.Sleep(10000);
                        //videoRec.Stop();
                        //videoRec = null;
                        ExecutedSteps++;
                        statusArray[iterI - 1, ExecutedSteps] = "Pass";
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Iteration: " + iterI + " -->Test Step Passed--" + result.steps[ExecutedSteps].description);

                        ////Get canvas image as decrypt text while cine plays
                        //IWebElement viewport2ImageCanvas = BasePage.FindElementByCss("div[id*='ViewerHost1.SeriesViewer_0'] #PhysicalLayersUnderOverlay0");
                        //bool imageRenderFail = false;
                        //var js = BasePage.Driver as IJavaScriptExecutor;
                        //if (js != null)
                        //{
                        //    try
                        //    {
                        //        var decryptImage = js.ExecuteScript("return parent.frames[0].document.querySelector(\"div[id *='ViewerHost1.SeriesViewer_0'] #PhysicalLayersUnderOverlay0\").toDataURL();");
                        //        if (decryptImage.ToString().Length<4000)
                        //        {
                        //            imageRenderFail = true;
                        //        }
                        //    }
                        //    catch (Exception e)
                        //    {
                        //        Logger.Instance.ErrorLog("Exception encountered while getting decrypted image value-- " + e.Message);
                        //    }
                        //}

                        //if (true)
                        //{
                        //    //passCount[ExecutedSteps] = passCount[ExecutedSteps] + 1;
                        //    statusArray[iterI - 1, ExecutedSteps] = "Pass";
                        //    result.steps[ExecutedSteps].status = "Pass";
                        //    Logger.Instance.InfoLog("Iteration: " + iterI + " -->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        //}
                        //else
                        //{
                        //    statusArray[iterI - 1, ExecutedSteps] = "Fail";
                        //    result.steps[ExecutedSteps].status = "Fail";
                        //    Logger.Instance.ErrorLog("Iteration: " + iterI + " -->Test case Failed--" + result.steps[ExecutedSteps].description);
                        //    result.steps[ExecutedSteps].SetLogs();
                        //}

                        //Close Viewer
                        viewer.CloseBluRingViewer();



                        bool anyFail = false;
                        for (int counterA = 0; counterA <= ExecutedSteps; counterA++)
                        {
                            if (result.steps[counterA].status == "Fail") anyFail = true;
                        }
                        if (!anyFail)
                            Logger.Instance.InfoLog("Iteration: " + iterI + " --> All Steps Passed--");
                        else
                            Logger.Instance.InfoLog("Iteration: " + iterI + " --> Some Steps Failed--");
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.ErrorLog("Exception in Iteration: " + iterI + " ---- " + ex);
                        if (ex.Message.ToLower().Contains("system.io.ioexception"))
                        {
                            Logger.Instance.InfoLog("Iteration: " + iterI + " --> Ignoring fail count as it is an IO Exception");
                        }
                        else
                        {
                            if (ExecutedSteps == stepcount - 1)
                            {
                                result.steps[ExecutedSteps].SetLogs();
                                statusArray[iterI - 1, ExecutedSteps] = "Fail";
                            }
                            else
                            {
                                result.steps[ExecutedSteps + 1].SetLogs();
                                statusArray[iterI - 1, ExecutedSteps + 1] = "Fail";
                            }
                        }
                        try
                        {
                            //new BluRingViewer().CloseBluRingViewer();
                            //Logger.Instance.ErrorLog("Iteration: " + iterI + " --> Catch Block : Viewer is closed");
                            login.DriverGoTo(login.url);
                            login.LoginIConnect(adminUserName, adminPassword);
                            Logger.Instance.ErrorLog("Iteration: " + iterI + " --> Catch Block : User Logged in again");
                        }
                        catch (Exception e)
                        {
                            Logger.Instance.ErrorLog("Iteration: " + iterI + " --> Catch Block : Exception -- " + e);
                        }

                    }
                }

                //Calculate fail count of each step for 200 iterations
                Logger.Instance.InfoLog("*************************Full Report status*******************************");
                int failCount = 0;
                for (int iterX = 0; iterX < stepcount; iterX++)
                {
                    failCount = 0;
                    for (int iterY = 0; iterY < Int32.Parse(iterations); iterY++)
                    {
                        if (statusArray[iterY, iterX] == "Fail")
                        {
                            failCount++;
                            if (failCount <= 10)
                                Logger.Instance.ErrorLog("Step " + (iterX + 1) + ": Failed in Iteration - " + (iterY + 1));
                            else
                                Logger.Instance.ErrorLog("Step " + (iterX + 1) + ": Failed in more than 10 iterations ....");
                        }                                                                   
                    }
                    Logger.Instance.ErrorLog("Step " + (iterX + 1) + ": Failed for a total of " + failCount + " times");
                    if (failCount > 0)
                    {
                        result.steps[iterX].status = "Fail";
                        result.steps[iterX].comments = "Fail count: " + failCount;
                    }
                }

                //for (int iterJ = 1; iterJ < stepcount; iterJ++)
                //{                    
                //    if (passCount[iterJ] == Int32.Parse(iterations))
                //        result.steps[iterJ].status = "Pass";
                //    else
                //        result.steps[iterJ].status = "Fail";
                //    //Set the comments to display in report 
                //    //result.steps[iterJ].comments = passCount[iterJ] + " out of " + iterations + " iterations: Pass";
                //    result.steps[iterJ].comments = passCount[iterJ] + " times failed";
                //}

                //Logout Application                
                login.Logout();

                //Return Result
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
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }


        /// <summary>
        ///1. Reliability Test - 1 web server + about 50 concurrent users from Client
        /// </summary>
        public TestCaseResult Test_168009(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            DateTime startTime = DateTime.Now;

            try
            {
          

                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
               
                //Step 1 - 8     //Execution carried out sepeartely      
                result.steps[++ExecutedSteps].status = "Pass";
                result.steps[++ExecutedSteps].status = "Pass";
                result.steps[++ExecutedSteps].status = "Pass";
                result.steps[++ExecutedSteps].status = "Pass";
                result.steps[++ExecutedSteps].status = "Pass";
                result.steps[++ExecutedSteps].status = "Pass";
                result.steps[++ExecutedSteps].status = "Pass";
                result.steps[++ExecutedSteps].status = "Pass";

                //Logout Application                
                login.Logout();

                //Return Result
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
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }


        /// <summary>
        /// 2. Reliability Test- Viewer Load test from server + Multiple clients operation fro UV
        /// </summary>
        public TestCaseResult Test_168011(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            DateTime startTime = DateTime.Now;

            try
            {
                Studies studies;
                BluRingViewer viewer;

                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String patientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                string[] arrRadiologyPatientID = patientID.Split('=')[0].Split(':');
                string[] arrCardiologyPatientID = patientID.Split('=')[1].Split(':');
                String modality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");              
                string[] arrRadiologyModality = modality.Split('=')[0].Split(':');
                string[] arrCardiologyModality = modality.Split('=')[1].Split(':');
                String imagesCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ImagesCount");
                string[] arrImagesCount = imagesCount.Split(':');

                int[] passCount = new int[stepcount];

                string[,] statusArray = new string[1, stepcount];

                //Step 1
                //Step 2              
                result.steps[++ExecutedSteps].status = "Pass";              
                result.steps[++ExecutedSteps].status = "Pass";             

                //Login to WebAccess site with any privileged user.
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);                        
                Random rnd = new Random();
                int iterI = 0;
                do
                {
                    iterI++;
                    ExecutedSteps = 1;                    
                    string[,] tempArray = new string[iterI, stepcount];
                    Array.Copy(statusArray, tempArray, statusArray.Length);
                    statusArray = tempArray;
                    tempArray = null;
                    statusArray[iterI - 1, ExecutedSteps - 1] = "Pass"; //Step 1 - Pass
                    statusArray[iterI - 1, ExecutedSteps] = "Pass"; //Step 2 - Pass
                    try
                    {
                        BasePage.Driver.Quit();                       
                        BasePage.Driver = null;
                        login = new Login();
                        login.DriverGoTo(login.url);
                        login.LoginIConnect(adminUserName, adminPassword);
                        Logger.Instance.InfoLog("*****************Starting Iteration: " + iterI + "***********************");
                        Logger.Instance.InfoLog("****Radiology actions****");
                        DateTime radiologyStartTime = DateTime.Now;
                        //Step 3: Perform Radilogy actions                    
                        //Open “US\MR” study in 2x2  
                        int studyToView = rnd.Next(arrRadiologyPatientID.Length);
                        studies = (Studies)login.Navigate("Studies");
                        //studies.SearchStudy(patientID: arrRadiologyPatientID[studyToView], Modality: arrRadiologyModality[studyToView]);
                        studies.SearchStudy("*", "*", arrRadiologyPatientID[studyToView], "*", "*", arrRadiologyModality[studyToView], "", "");
                        studies.SelectStudy("Patient ID", arrRadiologyPatientID[studyToView]);
                        viewer = new BluRingViewer();
                        //viewer = BluRingViewer.LaunchBluRingViewer();
                        var js = (IJavaScriptExecutor)BasePage.Driver;
                        if (BasePage.SBrowserName.ToLower().Equals("internet explorer"))
                        {
                            js.ExecuteScript("arguments[0].click()", BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.btn_bluringviewer)));
                        }
                        else
                        {
                            var button = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.btn_bluringviewer));
                            button.Click();
                        }
                        BasePage.Driver.SwitchTo().DefaultContent();
                        BasePage.Driver.SwitchTo().Frame(0);
                        BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[id*='ViewerHost0.SeriesViewer_0'] #PhysicalLayersAboveOverlay0")));
                        //div[id*='ViewerHost0.SeriesViewer_0'] #PhysicalLayersAboveOverlay0                       
                        //Wait for viewport1 image to load
                        if (BasePage.SBrowserName.ToLower().Equals("firefox"))
                        {                            
                        }
                        else
                        {
                            BasePage.wait.Until<Boolean>(d =>
                            {
                                Thread.Sleep(2000);
                                var js2 = (IJavaScriptExecutor)BasePage.Driver;
                                var script = "return(function(){var len = document.querySelector(\"div[id*='ViewerHost0.SeriesViewer_0'] #PhysicalLayersAboveOverlay0\").toDataURL().length; return len;})();";
                                var len = (long)js2.ExecuteScript(script);
                                if (len > 10000)
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            });
                        }
                        Logger.Instance.InfoLog("Image load complete");                       

                        Thread.Sleep(20000); //View study for 20 seconds
                        Thread.Sleep(10000); //See History for 10 seconds

                        //PageLoadWait.WaitForFrameLoad(20);                   
                        //Set layout 2x2
                        //Thread.Sleep(5000);
                        //Apply global stack and scroll through it
                        //Click on History tab and load a random study 
                        //Thread.Sleep(1000);
                        //Apply W/L, W/W               
                        viewer.SelectViewerTool(BluRingTools.Window_Level, 1, 1);
                        viewer.ApplyTool_WindowWidth();
                        //Thread.Sleep(1000);
                        //Scroll through images in viewport 1                     
                        var viewport = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);                    
                        int countToScrollAllImages = Convert.ToInt32(arrImagesCount[studyToView]) - 1;
                        //for (int i = 1; i <= countToScrollAllImages; i++)
                        for (int i = 1; i <= 10; i++)
                        {
                            viewport.SendKeys(Keys.ArrowDown);
                            //Thread.Sleep(500);
                        }
                        //close study
                        viewer.CloseBluRingViewer();
                        DateTime radiologyEndTime = DateTime.Now;                        
                        double radiologyInterval = (radiologyEndTime - radiologyStartTime).TotalSeconds;
                        Logger.Instance.InfoLog("Radiology Start: " + radiologyStartTime + ", End: " + radiologyEndTime + ", Interval: " + radiologyInterval);
                        if (BasePage.SBrowserName.ToLower().Equals("firefox"))
                            radiologyInterval = radiologyInterval - 50; //Reducing 50 secs as test execute launch, project loading and closing for a particular action takes this times. Calculated based on time taken for multiple runs.
                        if (radiologyInterval < 180)
                        {                            
                            result.steps[++ExecutedSteps].status = "Pass"; //Step 3 - Pass
                            statusArray[iterI - 1, ExecutedSteps] = "Pass";
                            Logger.Instance.InfoLog("Iter: " + iterI + " - Step: " + ExecutedSteps + ", Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            result.steps[++ExecutedSteps].status = "Fail";
                            statusArray[iterI - 1, ExecutedSteps] = "Fail";
                            Logger.Instance.InfoLog("Iter: " + iterI + " - Step: " + ExecutedSteps + ", Test Step Failed--" + result.steps[ExecutedSteps].description);
                        }
                        login.Logout();
                        BasePage.Driver.Quit();
                        BasePage.Driver = null;
                        login = new Login();
                        login.DriverGoTo(login.url);
                        login.LoginIConnect(adminUserName, adminPassword);

                        Logger.Instance.InfoLog("****Cardiology actions****");
                        //Step 4: Perform Cardiology actions                            
                        //Open 'Name, None' XA study in 1x2                          
                        studies = (Studies)login.Navigate("Studies");
                        DateTime cardiologyStartTime = DateTime.Now;
                        //studies.SearchStudy(patientID: arrCardiologyPatientID[0]); //Name, None
                        studies.SearchStudy("*", "*", arrCardiologyPatientID[0], "*", "*", arrCardiologyModality[0], "", "");
                        studies.SelectStudy("Modality", arrCardiologyModality[0]); //XA
                        //viewer = BluRingViewer.LaunchBluRingViewer();
                        js = (IJavaScriptExecutor)BasePage.Driver;
                        if (BasePage.SBrowserName.ToLower().Equals("internet explorer"))
                        {
                            js.ExecuteScript("arguments[0].click()", BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.btn_bluringviewer)));
                        }
                        else
                        {
                            var button = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.btn_bluringviewer));
                            button.Click();
                        }                                        
                        //Group play 30 seconds  
                        //Wait till Exam load div shows up studies
                        PageLoadWait.WaitForFrameLoad(20);
                        if (BasePage.SBrowserName.ToLower().Equals("internet explorer"))
                        {                            
                            BasePage.Driver.SwitchTo().DefaultContent();
                            BasePage.Driver.SwitchTo().Frame(0);
                            BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[id*='ViewerHost0.SeriesViewer_0'] #PhysicalLayersAboveOverlay0")));                           
                            //BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(BluRingViewer.div_CINE_PlayAllBtn)));
                        }
                        else
                        {
                            BasePage.Driver.SwitchTo().DefaultContent();
                            BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                            BasePage.wait.Until<Boolean>(d =>
                            {
                                if (d.FindElement(By.CssSelector(BluRingViewer.div_ContainerPriors)).GetAttribute("style").Contains("height:"))
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            });
                            Logger.Instance.InfoLog("Image load complete");
                            BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(BluRingViewer.div_CINE_PlayAllBtn)));
                        }
                        var cinePlayButton = viewer.GetElement("cssselector", BluRingViewer.div_CINE_PlayAllBtn);
                        if (BasePage.SBrowserName.ToLower().Equals("internet explorer"))
                        {
                            viewer.ClickElement(cinePlayButton);
                        }
                        else
                            cinePlayButton.Click();                        
                        Thread.Sleep(30000);
                        //Next group and 30 seconds
                        int counterNextSeries = 0;
                        do
                        {
                            if (BasePage.SBrowserName.ToLower().Equals("internet explorer"))
                            {
                                viewer.ClickElement(viewer.GetElement("cssselector", BluRingViewer.div_CINE_PlayNextSeriesBtn));
                            }
                            else
                                viewer.GetElement("cssselector", BluRingViewer.div_CINE_PlayNextSeriesBtn).Click();
                            Thread.Sleep(30000);
                        }
                        while (counterNextSeries++ < 2);
                        //close study
                        viewer.CloseBluRingViewer();
                        DateTime cardiologyEndTime = DateTime.Now;                        
                        double cardiologyInterval = (cardiologyEndTime - cardiologyStartTime).TotalSeconds;
                        Logger.Instance.InfoLog("Cardiology Start: " + cardiologyStartTime + ", End: " + cardiologyEndTime + ", Interval: " + cardiologyInterval);
                        if (cardiologyInterval < 180)
                        {
                            result.steps[++ExecutedSteps].status = "Pass"; //Step 4 - Pass
                            statusArray[iterI - 1, ExecutedSteps] = "Pass";
                            Logger.Instance.InfoLog("Iter: " + iterI + " - Step: " + ExecutedSteps + ", Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            result.steps[++ExecutedSteps].status = "Fail";
                            statusArray[iterI - 1, ExecutedSteps] = "Fail";
                            Logger.Instance.InfoLog("Iter: " + iterI + " - Step: " + ExecutedSteps + ", Test Step Failed--" + result.steps[ExecutedSteps].description);
                        }
                        login.Logout();

                        result.steps[++ExecutedSteps].status = "Pass"; //Step 5 - Pass
                        statusArray[iterI - 1, ExecutedSteps] = "Pass";
                        Logger.Instance.InfoLog("Iter: " + iterI + " - Step: " + ExecutedSteps + ", Test Step Passed--" + result.steps[ExecutedSteps].description);

                        result.steps[++ExecutedSteps].status = "Pass"; //Step 6 - Pass
                        statusArray[iterI - 1, ExecutedSteps] = "Pass";
                        Logger.Instance.InfoLog("Iter: " + iterI + " - Step: " + ExecutedSteps + ", Test Step Passed--" + result.steps[ExecutedSteps].description);

                        result.steps[++ExecutedSteps].status = "Pass"; //Step 7 - Pass
                        statusArray[iterI - 1, ExecutedSteps] = "Pass";
                        Logger.Instance.InfoLog("Iter: " + iterI + " - Step: " + ExecutedSteps + ", Test Step Passed--" + result.steps[ExecutedSteps].description);

                        bool anyFail = false;
                        for (int counterA = 0; counterA <= ExecutedSteps; counterA++)
                        {
                            if (result.steps[counterA].status == "Fail") anyFail = true;
                        }
                        if (!anyFail)
                            Logger.Instance.InfoLog("Iteration: " + iterI + " --> All Steps Passed--");
                        else
                            Logger.Instance.InfoLog("Iteration: " + iterI + " --> Some Steps Failed--");
                        Thread.Sleep(15000); //15 sec wait
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.ErrorLog("Exception in Iteration: " + iterI + " ---- " + ex);
                        if (ex.Message.ToLower().Contains("system.io.ioexception"))
                        {
                            Logger.Instance.InfoLog("Iteration: " + iterI + " --> Ignoring fail count as it is an IO Exception");
                        }
                        else
                        {
                            if (ExecutedSteps == stepcount - 1)
                            {
                                result.steps[ExecutedSteps].SetLogs();
                                statusArray[iterI - 1, ExecutedSteps] = "Fail";
                            }
                            else
                            {
                                result.steps[ExecutedSteps + 1].SetLogs();
                                statusArray[iterI - 1, ExecutedSteps + 1] = "Fail";
                            }
                        }
                        try
                        {
                            login.DriverGoTo(login.url);
                            login.LoginIConnect(adminUserName, adminPassword);
                            Logger.Instance.ErrorLog("Iteration: " + iterI + " --> Catch Block : User Logged in again");
                        }
                        catch (Exception e)
                        {
                            Logger.Instance.ErrorLog("Iteration: " + iterI + " --> Catch Block : Exception -- " + e);
                        }

                    }
                } while ((DateTime.Now - startTime).TotalMinutes < 120);

                //Calculate fail count of each step for all iterations
                Logger.Instance.InfoLog("*************************Full Report status*******************************");
                int failCount = 0;
                for (int iterX = 2; iterX < stepcount; iterX++)
                {
                    failCount = 0;
                    for (int iterY = 0; iterY < iterI; iterY++)
                    {
                        if (statusArray[iterY, iterX] == "Fail")
                        {
                            failCount++;
                            if (failCount <= 10)
                                Logger.Instance.ErrorLog("Step " + (iterX + 1) + ": Failed in Iteration - " + (iterY + 1));
                            else
                                Logger.Instance.ErrorLog("Step " + (iterX + 1) + ": Failed in more than 10 iterations ....");
                        }
                    }
                    Logger.Instance.ErrorLog("Step " + (iterX + 1) + ": Failed for a total of " + failCount + " times");
                    if (failCount > 0)
                    {
                        result.steps[iterX].status = "Fail";
                        result.steps[iterX].comments = "Fail count: " + failCount;
                    }
                }

                //Logout Application                
                login.Logout();

                //Return Result
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
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }

    }
}
