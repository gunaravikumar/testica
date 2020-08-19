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
using TestComplete;
using System.ComponentModel;
using System.Data;
using System.Runtime.InteropServices;
using Accord.Imaging;
using IronOcr;
using Accord.Imaging.Filters;
using System.Runtime.InteropServices;


namespace Selenium.Scripts.Tests
{
    class MPRResultControl : BasePage
    {

        public Login login { get; set; }
        public string filepath { get; set; }
        public DomainManagement domain { get; set; }
        public StudyViewer studyviewer { get; set; }
        public Cursor Cursor { get; private set; }

        public MPRResultControl(String classname)
        {

            login = new Login();
            //  BasePage.InitializeControlIdMap();
            login.DriverGoTo(login.url);
            domain = new DomainManagement();

            studyviewer = new StudyViewer();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }

        public TestCaseResult Test_163359(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            BluRingViewer viewer = new BluRingViewer();


            WpfObjects wpfobject = new WpfObjects();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            String sPatientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String Testdata_req = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            string[] split_testdata = Testdata_req.Split('|');
            string sHeadingPatientID = split_testdata[0];
            string sHeadingAccession = split_testdata[1];
            string sThickness4 = split_testdata[2];
            string slocationvalue = split_testdata[3];


            String thumbnailcaption = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            try
            {
                //Step 1   Launch the study INC,TEST in 3D viewer.2.Navigate to 3D tab and Click MPR mode from the dropdown.
                login.LoginIConnect(username, password);

                z3dvp.Deletefiles(testcasefolder);
                login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                login.SearchStudy(sHeadingPatientID, sPatientid);
                PageLoadWait.WaitForLoadingMessage(30);
                login.SelectStudy(sHeadingAccession, "");
                PageLoadWait.WaitForFrameLoad(5);

                var viewer1 = BluRingViewer.LaunchBluRingViewer(fieldname: "Accession", value: "");
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                bool res = z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR, "n");

                if (res == true)
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

                //Step 2  Double click on the image in MPR result control.  

                IWebElement MPRREsultpanel = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                bool step2 = z3dvp.EnableOneViewupMode(MPRREsultpanel);
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
                //Step 3 Adjust the Thickness on the bottom left hand corner of the control to 6.0 mm. 
                z3dvp.SelectControl(BluRingZ3DViewerPage.ResultPanel);
                bool btool4 = z3dvp.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.ResultPanel);
                bool bflag4 = false;
                IWebElement Iresultpanel = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                if (btool4)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width / 2 - 500, z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 2 - 500);
                    Actions act4 = new Actions(Driver);
                    //Increase the box size for lining.
                    act4.MoveToElement(Iresultpanel, Iresultpanel.Size.Width / 2, Iresultpanel.Size.Height / 2)
                    .ClickAndHold()
                    .MoveToElement(Iresultpanel, Iresultpanel.Size.Width / 2, Iresultpanel.Size.Height / 4)
                    .Release()
                    .Build()
                    .Perform();
                    if (Config.BrowserType != "firefox" && Config.BrowserType != "mozilla")
                    {
                        act4.MoveToElement(Iresultpanel, Iresultpanel.Size.Width / 2, Iresultpanel.Size.Height / 2)
                        .ClickAndHold()
                        .MoveToElement(Iresultpanel, Iresultpanel.Size.Width / 2, Iresultpanel.Size.Height / 4)
                        .Release()
                        .Build()
                        .Perform();
                    }
                    Thread.Sleep(5000);
                    z3dvp.EnterThickness(BluRingZ3DViewerPage.ResultPanel, sThickness4);
                    bflag4 = true;
                    IWebElement ViewerContainer4 = z3dvp.ViewerContainer();
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((ViewerContainer4.Location.X - 100), (ViewerContainer4.Location.Y - 100));
                    // z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, false);
                    new Actions(Driver).SendKeys("T").Build().Perform();
                    Thread.Sleep(500);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    Thread.Sleep(1000);
                    if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel),pixelTolerance:400))
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
                    //  z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, true);
                    new Actions(Driver).SendKeys("T").Build().Perform();
                    Thread.Sleep(500);
                }
                if (bflag4 == false)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 4  MPR result controls is available only on 2 views 
                //Repeat steps 2-4 on all MPR result controls on all views. 
                bool res8_mpr = z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR, "n");
                Thread.Sleep(5000);
                IWebElement IMpr = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel); bool bMPR = false;
                if (IMpr.Displayed) { bMPR = true; }

                bool res8_6 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6, "n");
                Thread.Sleep(5000);
                IWebElement IMPR6 = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                bool bMPr6 = false;
                if (IMPR6.Displayed) { bMPr6 = true; }
                Thread.Sleep(10000);
                bool res8_curved = z3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR, "n");
                Thread.Sleep(5000);
                bool bcurved_result = true;
                try { IWebElement ICurvedMPR = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel); if (ICurvedMPR == null) bcurved_result = false; } catch { bcurved_result = false; }
                Thread.Sleep(10000);

                bool res8_4 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4, "n");
                Thread.Sleep(5000);
                bool bMPR4 = true;
                try { IWebElement IMPR4 = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel); if (IMPR4 == null) bMPR4 = false; } catch { bMPR4 = false; }
                Thread.Sleep(10000);
                bool res8_calcium = z3dvp.select3dlayout(BluRingZ3DViewerPage.CalciumScoring, "n");
                Thread.Sleep(5000);
                bool bCalcium = true;
                try { IWebElement iCalcium = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel); if (iCalcium == null) bCalcium = false; } catch { bCalcium = false; }
                if (bMPR == true && bcurved_result == false && bMPR4 == false && bMPr6 == true && bCalcium == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
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


        public TestCaseResult Test_163358(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            BluRingViewer viewer = new BluRingViewer();
            

            WpfObjects wpfobject = new WpfObjects();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            String Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string popwindowwarn = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String thumbnailcaption = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            try
            {
                //Step 1 Log on to iCA with valid credentials.
                z3dvp.Deletefiles(testcasefolder);
                login.LoginIConnect(username, password);
                login.Navigate("Studies");
                login.ClearFields();
                login.SearchStudy("patient", Patientid);
                PageLoadWait.WaitForLoadingMessage(30);
                login.SelectStudy("Patient ID", Patientid);
                BluRingViewer.LaunchBluRingViewer(fieldname: "Patient ID", value: Patientid);
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 2 Select the Line measurement from the floating tool box. Perform a measurement by left clicking and dragging from one point to another on the image on iCA viewer.
                bool bflag3 = viewer.SelectViewerTool(BluRingTools.Line_Measurement);
                if (bflag3)
                {
                    IList<IWebElement> TwoDPanel = z3dvp.Viewpot2D();
                    Actions act4 = new Actions(Driver);
                    new Actions(Driver).MoveToElement(TwoDPanel[1], (TwoDPanel[1].Size.Width / 2) - 70, (TwoDPanel[1].Size.Height / 2) - 50)
                    .ClickAndHold()
                    .MoveToElement(TwoDPanel[1], (TwoDPanel[1].Size.Width / 2) - 70, (TwoDPanel[1].Size.Height / 2) - 150)
                    .Release().Build().Perform();
                    Thread.Sleep(3000);
                    
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    IWebElement Viewport2 = viewer.ViewPortContainer();
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2.Location.X / 2 + 400), (Viewport2.Location.Y / 2 + 250));
                    if (CompareImage(result.steps[ExecutedSteps], TwoDPanel[1],pixelTolerance:400))
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
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //viewer.CloseBluRingViewer();
                //Step 3 Select the MPR option from smart view drop down.
             //   z3dvp.selectthumbnail(thumbnailcaption);
                bool step5 = z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR, "n");
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
                //Step 4 : Click on the MPR navigation control 1 and position the intersection of the red and blue cross hairs at the top of the image displayed. 
                Actions act6 = new Actions(Driver);
                bool lflag6 = z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationone);
                bool bflag6 = false;
                if (lflag6)
                {

                    act6.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2)
                        .ClickAndHold()
                        .MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2 - 170)
                        .Release().Build().Perform();
                    Thread.Sleep(5000);
                    bflag6 = true;
                    //   z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, false);
                    new Actions(Driver).SendKeys("T").Build().Perform();
                    Thread.Sleep(500);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree), pixelTolerance: 400))
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
                    //   z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, true);
                    new Actions(Driver).SendKeys("T").Build().Perform();
                    Thread.Sleep(500);
                }
                if (bflag6 == false)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 5 On the MPR Result control, select "Navigation 3" to be the source control from the drop down list.
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.ResultPanel);
                List<string> Iresult5 = z3dvp.GetControlvalues(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationthree);
                if (Iresult5[3] == BluRingZ3DViewerPage.Navigationthree)
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

                //Step 6 Right click the mouse on the image in mpr result control and select the measurement tool from the 3D toolbox.
                bool btool8 = z3dvp.select3DTools(Z3DTools.Line_Measurement, BluRingZ3DViewerPage.ResultPanel);
                if (btool8)
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

                //Step 7 On the image displayed on the MPR result control, Perform a measurement on the image in MPR result control.Note: Measurement should be drawn similar to the measurement that of iCA
                bool bresultpenle9 = z3dvp.SelectControl(BluRingZ3DViewerPage.ResultPanel);
                IWebElement Iresulst7 = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                Actions act9 = new Actions(Driver);
                act9.MoveToElement(Iresulst7, (Iresulst7.Size.Width / 2) - 50, (Iresulst7.Size.Height / 2) - 50)
                .ClickAndHold()
                .MoveToElement(Iresulst7, (Iresulst7.Size.Width / 2) - 50, (Iresulst7.Size.Height / 2) - 100)
                .Release().Build().Perform();
                Thread.Sleep(5000);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
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

                //------------------------------------
                //Step 8 Create 15 more measurements. Create some on the other images displayed on the other 3 MPR Navigation controls.The user should be able to create up to 15 measurements in each control. 
                //bool bresultpenle10 = z3dvp.SelectControl(BluRingZ3DViewerPage.ResultPanel);
                int llopp = 0;
                IWebElement Iresultpanle8 = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                for (int i = 1; i < 225; i += 15)
                {
                    llopp = llopp + 1;
                    Actions act10 = new Actions(Driver);
                    int temp11 = i + 50;
                    act10.MoveToElement(Iresultpanle8, (Iresultpanle8.Size.Width / 2 + 100) - temp11, (Iresultpanle8.Size.Height / 2) - 10)
                   .ClickAndHold()
                   .MoveToElement(Iresultpanle8, (Iresultpanle8.Size.Width / 2 + 100) - temp11, (Iresultpanle8.Size.Height / 2) - 100)
                   .Release().Build().Perform();
                    Thread.Sleep(3000);

                }
                if (llopp == 15)
                {
                    Thread.Sleep(10000);
                    Actions act15 = new Actions(Driver);
                    act15.SendKeys(OpenQA.Selenium.Keys.Enter).Build().Perform();
                    Thread.Sleep(5000);
                    //  z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, false);
                    new Actions(Driver).SendKeys("T").Build().Perform();
                    Thread.Sleep(500);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
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
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //  z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, true);
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(500);
                //Step 9Select one of the measurement annotations created on the MPR result control by clicking on it.
                Actions act11 = new Actions(Driver);
                int temp = 1 + 50;
                act11.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), (z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width / 2 + 100) - temp, (z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 2) - 10)
               .ClickAndHold()
               .Release().Build().Perform();
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), pixelTolerance: 400))
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

                //step 10Click the "Delete" key on the keyboard to delete the measurement annotation.
                act11.SendKeys(OpenQA.Selenium.Keys.Delete).Build().Perform();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
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
                //Return Result
                return result;
            }
            finally
            {
                login.Logout();
            }
        }

        public TestCaseResult Test_163365(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            BluRingViewer viewer = new BluRingViewer();


            WpfObjects wpfobject = new WpfObjects();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            String sPatientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String Testdata_req = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            string[] split_testdata = Testdata_req.Split('|');
            string sHeadingPatientID = split_testdata[0];
            string sHeadingAccession = split_testdata[1];
            string sThickness4 = split_testdata[2];
            string sThickness8 = split_testdata[3];
            string sThickness12 = split_testdata[4];

            String thumbnailcaption = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            try
            {
                login.LoginIConnect(username, password);
                //Step 1   From the Universal viewer , Select a 3D supported series and Select the MPR option from the drop down.Study : INC,TEST
                z3dvp.Deletefiles(testcasefolder);
                login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                login.SearchStudy(sHeadingPatientID, sPatientid);
                PageLoadWait.WaitForLoadingMessage(30);
                login.SelectStudy(sHeadingAccession, "");
                PageLoadWait.WaitForFrameLoad(5);

                var viewer1 = BluRingViewer.LaunchBluRingViewer(fieldname: "Accession", value: "");
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                bool res = z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR, "n");
                if (res == true)
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


                //step 2 Double click on the image in MPR result control.

                IWebElement MPRREsultpanel = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                bool step2 = z3dvp.EnableOneViewupMode(MPRREsultpanel);
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

                //step 3 Create a measurement annotation from the center of the box to one of the edges of the white box displayed in the MPR result control. 
                z3dvp.SelectControl(BluRingZ3DViewerPage.ResultPanel);
                z3dvp.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.ResultPanel);
                Actions act3 = new Actions(Driver);
                act3.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 2)
                .ClickAndHold()
                .MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 4)
                .Release()
                .Build()
                .Perform();
                if (Config.BrowserType.ToLower() != "firefox" && Config.BrowserType.ToLower() != "mozilla")
                {
                    act3.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 2)
                        .ClickAndHold()
                        .MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 4)
                        .Release()
                        .Build()
                        .Perform();
                }        
                Thread.Sleep(5000);
                bool btool3 = z3dvp.select3DTools(Z3DTools.Line_Measurement, BluRingZ3DViewerPage.ResultPanel);
                
                bool bflag3 = false;
                if (btool3)
                {
                    z3dvp.SelectControl(BluRingZ3DViewerPage.ResultPanel);
                    Actions act3_annotn = new Actions(Driver);
                    act3_annotn.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), (z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width / 2), (z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 2))
                    .ClickAndHold()
                    .MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), (z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width / 2), (z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 2) - 38)
                    .Release().Build().Perform();
                    bflag3 = true;
                    Thread.Sleep(2000);
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width / 2 - 500, z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 2 - 500);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
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
                if (bflag3 == false)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 4 Adjust the Thickness on the bottom left hand corner of the control to 6.0 mm.
                Actions act4 = new Actions(Driver);
                //deltet the annotation previous draw
                act4.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), (z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width / 2), (z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 2))
                .ClickAndHold()
                .MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), (z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width / 2), (z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 2) - 35)
                .Release().Build().Perform();
                Thread.Sleep(1000);
                act4.SendKeys(OpenQA.Selenium.Keys.Delete).Build().Perform();
                Thread.Sleep(1000);
                z3dvp.EnterThickness(BluRingZ3DViewerPage.ResultPanel, sThickness4);
                IList<IWebElement> Viewport2 = z3dvp.Viewport();
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
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

                //step 5 Select the measurement tool from the 3D toolbox. 
                bool btool5 = z3dvp.select3DTools(Z3DTools.Line_Measurement, BluRingZ3DViewerPage.ResultPanel);

                if (btool5)
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

                //step 6 Create a measurement annotation by clicking on the center of the bottom left white box and dragging to the right out a little past the location of the top right box. 
                z3dvp.SelectControl(BluRingZ3DViewerPage.ResultPanel);
                Actions act6 = new Actions(Driver);
                act6.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 2)
                .ClickAndHold()
                .MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width - 95, z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 2)
                .Release()
                .Build()
                .Perform();
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
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
                //step 7 Create a second measurement annotation by clicking the center of the top right white box and dragging down until the new measurement annotation intersects with the previously created measurement annotation. 

                Actions act7 = new Actions(Driver);
                act7.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width - 140, z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 4 - 40)
                .ClickAndHold()
                .MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), (z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width - 140), (z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 2) - 2)
                .Release().Build().Perform();
                Thread.Sleep(1000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
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
                //step 8 Adjust the Thickness on the bottom left hand corner of the control to 30.0 mm.
                //precondtion for step 8
                bool bflag8 = false;
                z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.ResultPanel);
                z3dvp.EnterThickness(BluRingZ3DViewerPage.ResultPanel, sThickness8);
                Thread.Sleep(500);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
                {
                    bflag8 = true;
                }

                //step 9 Select the measurement tool from the 3D toolbox.
                bool btool9 = z3dvp.select3DTools(Z3DTools.Line_Measurement, BluRingZ3DViewerPage.ResultPanel);
                bool bflag9 = false;
                if (btool9)
                {
                    bflag9 = true;
                }

                //step 10 Create a measurement annotation by clicking on the center of the bottom left white box and dragging to the right out a little past the location of the top right box. 
                z3dvp.SelectControl(BluRingZ3DViewerPage.ResultPanel);
                Actions act10 = new Actions(Driver);
                act10.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 2)
                .ClickAndHold()
                .MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width - 95, z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 2)
                .Release()
                .Build()
                .Perform();
                Thread.Sleep(5000);
                bool bflag10 = false;
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 3, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
                {
                    bflag10 = true;
                }

                //step 11 Create a second measurement annotation by clicking the center of the top right white box and dragging down until the new measurement annotation intersects with the previously created measurement annotation. 
                Actions act11 = new Actions(Driver);
                if (Config.BrowserType.ToLower() == "firefox" || Config.BrowserType.ToLower() == "mozilla")
                {
                    act11.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width - 164, z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 4 - 36)
                 .ClickAndHold()
                 .MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), (z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width - 160), (z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height - 355))
                 .Release().Build().Perform();
                }
                else
                {
                    int oheight = 369; 
                    if (Config.BrowserType.ToLower() == "chrome") oheight = 360;

                    act11.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width - 164, z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 4 - 36)
                   .ClickAndHold()
                   .MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), (z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width - 160), (z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height - oheight))
                   .Release().Build().Perform();
                }

                Thread.Sleep(5000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 4, 1);
                bool bflag11 = false;
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
                {
                    bflag11 = true;
                }

                if (bflag8 && bflag9 && bflag10 && bflag11)
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
                z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.ResultPanel);
                //step 12 From iCA, Load the study "INC, TEST".1.Navigate to 3D tab and Click 3D 6:1 mode from the dropdown.Note: This is new design(could change)
                bool bflag12 = false;
                //precondtin for step 12 change the thickness values 
                z3dvp.EnterThickness(BluRingZ3DViewerPage.ResultPanel, sThickness12);
                bool res12 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6, "n");
                if (res12 == true)
                {
                    bflag12 = true;

                }
                //step 13 Double click on the image in MPR result control.
                //precondtion 
                PageLoadWait.WaitForFrameLoad(10);
                IList<IWebElement> Viewport14 = z3dvp.Viewport();
                bool bflag13 = false;
                bool MPRREsultpanel14 = z3dvp.EnableOneViewupMode(Viewport14[4]);
                if (MPRREsultpanel14)
                {
                    z3dvp.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.ResultPanel);
                    bflag13 = true;
                }

                //step 14 Create a measurement annotation from the center of the box to one of the edges of the white box displayed in the MPR result control.

                bool bflag14 = false;
                z3dvp.SelectControl(BluRingZ3DViewerPage.ResultPanel);
                z3dvp.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.ResultPanel);
                Actions act14 = new Actions(Driver);
                //act14.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 2)
                //.ClickAndHold()
                //.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 4)
                //.Release()
                //.Build()
                //.Perform();
                //     if (Config.BrowserType != "firefox" && Config.BrowserType != "mozilla")
                //      {
                act14.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 2)
                    .ClickAndHold()
                    .MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 4)
                    .Release()
                    .Build()
                    .Perform();
                Thread.Sleep(1000);
                if (Config.BrowserType.ToLower() != "firefox" && Config.BrowserType.ToLower() != "mozilla")
                { 
                    act14.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 2)
                   .ClickAndHold()
                   .MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 4)
                   .Release()
                   .Build()
                   .Perform();
                       }
                Thread.Sleep(5000);
                z3dvp.select3DTools(Z3DTools.Line_Measurement, BluRingZ3DViewerPage.ResultPanel);
                z3dvp.SelectControl(BluRingZ3DViewerPage.ResultPanel);
                Actions act14_annotn = new Actions(Driver);
                act14_annotn.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), (z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width / 2), (z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 2))
                .ClickAndHold()
                .MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), (z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width / 2), (z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 2) - 38)
                .Release().Build().Perform();
                bflag3 = true;
                Thread.Sleep(5000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width / 2 - 500, z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 2 - 500);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 6, 1);
                if (CompareImage(result.steps[ExecutedSteps], Viewport14[4]))
                {
                    bflag14 = true;
                }
                //step 15  Adjust the Thickness on the bottom left hand corner of the control to 6.0 mm. 

                bool bflag15 = false;
                Actions act15 = new Actions(Driver);
                act15.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), (z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width / 2), (z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 2))
                .ClickAndHold()
                .MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), (z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width / 2), (z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 2) - 38)
                .Release().Build().Perform();
                Thread.Sleep(1000);
                act15.SendKeys(OpenQA.Selenium.Keys.Delete).Build().Perform();
                Thread.Sleep(2000);
                IList<IWebElement> Viewport15 = z3dvp.Viewport();
                z3dvp.EnterThickness(BluRingZ3DViewerPage.ResultPanel, sThickness4);
                Thread.Sleep(1000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 7, 2);
                if (CompareImage(result.steps[ExecutedSteps], Viewport15[4]))
                {
                    bflag15 = true;
                }
                //step 16 Select the measurement tool from the 3D toolbox. 
                IList<IWebElement> Viewport16 = z3dvp.Viewport();
                bool btool16 = z3dvp.select3DTools(Z3DTools.Line_Measurement, BluRingZ3DViewerPage.ResultPanel);
                bool bflag16 = false;
                if (btool16)
                {
                    bflag16 = true;
                }

                //step 17 Create a measurement annotation by clicking on the center of the bottom left white box and dragging to the right out a little past the location of the top right box. Note: See attachment "Measurement 1" to see an example. 
                z3dvp.SelectControl(BluRingZ3DViewerPage.ResultPanel);
                Actions act17 = new Actions(Driver);
                act17.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 2)
                .ClickAndHold()
                .MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width - 95, z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 2)
                .Release()
                .Build()
                .Perform();
                Thread.Sleep(3000);
                bool bflag17 = false;
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 9, 3);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
                {
                    bflag17 = true;
                }
                //step 18 Create a second measurement annotation by clicking the center of the top right white box and dragging down until the new measurement annotation intersects with the previously created measurement annotation. 

                Actions act18 = new Actions(Driver);
                if (Config.BrowserType == "firefox" || (Config.BrowserType == "mozilla"))
                {
                    act18.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width - 140, z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height-565)
                    .ClickAndHold()
                    .MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), (z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width - 140), (z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height-358))
                    .Release().Build().Perform();
                }
                else
                {
                    act18.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width - 140, z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 4 - 40)
                   .ClickAndHold()
                   .MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), (z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width - 140), (z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 2) - 2)
                   .Release().Build().Perform();
                }
                Thread.Sleep(3000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 10, 4);
                bool bflag18 = false;
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
                {
                    bflag18 = true;
                }
                if (bflag12 && bflag13 && bflag14 && bflag15 && bflag16 && bflag17 && bflag18)
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

        public TestCaseResult Test_163357(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            BluRingViewer viewer = new BluRingViewer();


            WpfObjects wpfobject = new WpfObjects();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            String Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string popwindowwarn = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String thumbnailcaption = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string TestDataReq = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            string[] TestDataReqSplit = TestDataReq.Split('|');
            string step3cursorvalue = TestDataReqSplit[0];
            string slargevalue = TestDataReqSplit[1];
            string smedium = TestDataReqSplit[2];
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            try
            {
                //Step 1 Search and load a 3D supported study in universal viewer.
                z3dvp.Deletefiles(testcasefolder);
                login.LoginIConnect(username, password);
                //login.Navigate("Studies");
                //login.ClearFields();
                //login.SearchStudy("patient", Patientid);
                //PageLoadWait.WaitForLoadingMessage(30);
                //login.SelectStudy("Patient ID", Patientid);
                //BluRingViewer.LaunchBluRingViewer(fieldname: "Patient ID", value: Patientid);

                //PageLoadWait.WaitForFrameLoad(10);
                //SwitchToDefault();
                //SwitchToUserHomeFrame();
                //result.steps[++ExecutedSteps].status = "Pass";
                //Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                //Thread.Sleep(20000);
                //IList<IWebElement> thumbnaillist = Driver.FindElements(By.CssSelector(BluRingViewer.div_ThumbnailOuter));
                //Thread.Sleep(2000);
                //Step 1 Search and load a 3D supported study in universal viewer.
                //Step 2 Select the MPR option from the smart view drop down.
                bool res = z3dvp.searchandopenstudyin3D(Patientid, thumbnailcaption);
                if (res == true)
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
                    result.steps[ExecutedSteps].SetLogs();
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //tet cse update 
                ////step 2 Select the window level tool from the toolbox. Left click and drag the mouse on the image to apply the window level. Note down the values.
                //SwitchToDefault();
                //SwitchToUserHomeFrame();
                //Actions act3 = new Actions(Driver);
                //bool bflag3 = viewer.SelectViewerTool(BluRingTools.Window_Level);
                //IList<IWebElement> TwoDPanel3 = z3dvp.Viewpot2D();
                //new Actions(Driver).MoveToElement(thumbnaillist[0]).DoubleClick().Build().Perform();
                //Thread.Sleep(10000);
                //bool bflag2 = false;
                //string swindowvlaue = null;
                //if (bflag3)
                //{
                //    if (z3dvp.UserSetting().Displayed)
                //    {
                //        z3dvp.UserSetting().Click();
                //        Thread.Sleep(1000);
                //        IList<IWebElement> usersetdp = z3dvp.UserSEttingDP();
                //        Thread.Sleep(1000);
                //        if (usersetdp.Count > 0)
                //        {
                //            foreach (IWebElement svalue in usersetdp)
                //            {
                //                if (svalue.Text == slargevalue)
                //                {
                //                    svalue.Click();
                //                    break;
                //                }
                //            }
                //        }
                //    }
                //    IList<IWebElement> TwoDPanel4 = z3dvp.Viewpot2D();
                //    Actions act4 = new Actions(Driver);
                //    z3dvp.Performdragdrop(TwoDPanel4[0], 100, 100, TwoDPanel4[0].Size.Width / 2, TwoDPanel4[0].Size.Height / 2);
                //    z3dvp.Performdragdrop(TwoDPanel4[0], 100, 100, TwoDPanel4[0].Size.Width / 2, TwoDPanel4[0].Size.Height / 2);
                //    Thread.Sleep(5000);
                //    string ocrtext = z3dvp.ReadPatientDetailsUsingTesseract(TwoDPanel4[0], 4, 900, 1124, 1400, 1400);
                //    if (ocrtext.Length > 0)
                //    {
                //        int ilength = ocrtext.Length;
                //        int iindex = ocrtext.IndexOf("WL:");
                //        swindowvlaue = ocrtext.Substring(iindex + 3, ilength - (iindex + 3));
                //        bflag2 = true;
                //        result.steps[++ExecutedSteps].status = "Pass";
                //        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                //    }
                //}
                //if (bflag2 == false)
                //{
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}
                ////REvert 
                //if (z3dvp.UserSetting().Displayed)
                //{
                //    z3dvp.UserSetting().Click();
                //    Thread.Sleep(1000);
                //    IList<IWebElement> usersetdp = z3dvp.UserSEttingDP();
                //    Thread.Sleep(1000);
                //    if (usersetdp.Count > 0)
                //    {
                //        foreach (IWebElement svalue in usersetdp)
                //        {
                //            if (svalue.Text == smedium)
                //            {
                //                svalue.Click();
                //                break;
                //            }
                //        }
                //    }
                //}
                //z3dvp.selectthumbnail("Date:23-Jun-2013");
                //bool step5 = z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR, "y");
                //Thread.Sleep(5000);
              ////  Step 3 Select the MPR option from the smart view drop down.
              //  if (step5)
              //  {
              //      result.steps[++ExecutedSteps].status = "Pass";
              //      Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
              //  }
              //  else
              //  {
              //      result.steps[++ExecutedSteps].status = "Fail";
              //      Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
              //      result.steps[ExecutedSteps].SetLogs();
              //  }

                //step 4 Verify the window level values of the image in MPR result control. // jira
                //List<string> result2 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                ////get from step 5///
                //if (result2[0] == swindowvlaue.Trim())
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

                //step 5 Right click the mouse on the image in mpr result control and select the Window levle tool from the 3D toolbox.
                bool btool5 = z3dvp.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.ResultPanel);
                if (btool5)
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
                //step 6 Left click and drag the mouse on the image on MPR result control.
                bool lflag = z3dvp.SelectControl(BluRingZ3DViewerPage.ResultPanel);
                z3dvp.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.ResultPanel);
                z3dvp.Performdragdrop(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), 100, 100, z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 2);
                z3dvp.Performdragdrop(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), 100, 100, z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 2);
                Thread.Sleep(10000);
                List<string> result8 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                if (result8[3] != result8[1] && result8[3] != result8[2] && result8[3] != result8[0])
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
        public TestCaseResult Test_163356(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            WpfObjects wpfobject = new WpfObjects();
            BasePage basepage = new BasePage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            String Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String thumbnailcaption = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string slocations = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            try
            {
                login.LoginIConnect(username, password);
                //Step 1  From the Universal viewer , Select a 3D supported series and Select the MPR option from the drop down.
                z3dvp.Deletefiles(testcasefolder);
                bool res = z3dvp.searchandopenstudyin3D(Patientid, thumbnailcaption);

                if (res == true)
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
                //step 2 Click on the MPR navigation control 1 and position the intersection of the red and blue crosshairs at the top of the image displayed.
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationone);
                Actions act3 = new Actions(Driver);
                if (Config.BrowserType.ToLower() != "firefox" && Config.BrowserType.ToLower() != "mozilla")
                {
                    act3.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2)
                        .ClickAndHold()
                        .MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height - 310).Release().Build().Perform();
                }
                else
                {
                    act3.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2)
                       .ClickAndHold()
                       .MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height -300).Release().Build().Perform();

                }
                Thread.Sleep(5000);
                //  z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, false);
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(500);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Thread.Sleep(2000);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree)))
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
                //  z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, true);
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(500);
                //step 3  On the MPR Result control, select "Navigation 3" to be the source control from the drop down list.Click on the MPR result control and scroll the mouse wheel downwards.
                string bFilename_navigationoneone = testcasefolder + "step4_bNavigationone.png";
                string AFilename_navigationone = testcasefolder + "step4_ANavigationone.png";
                string bFilename_navigationtwo = testcasefolder + "step4_bNavigationtwo.png";
                string AFilename_navigationtwo = testcasefolder + "step4_ANavigationtwo.png";
                DownloadImageFile(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), bFilename_navigationoneone);
                DownloadImageFile(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo), bFilename_navigationtwo);
                bool bflag4 = false;

                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.ResultPanel);
                List<string> ResultpanelNav = z3dvp.GetControlvalues(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationthree);
                IWebElement ViewerContainer4 = z3dvp.ViewerContainer();
                if (ResultpanelNav[3] == BluRingZ3DViewerPage.Navigationthree)
                {
                    int t = 0;
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((ViewerContainer4.Location.X + 900), (ViewerContainer4.Location.Y / 2 + 600));
                    if ((Config.BrowserType.ToLower() == "firefox") || (Config.BrowserType.ToLower() == "mozilla"))
                    {
                        try
                        {
                            do
                            {
                                BasePage.mouse_event(0x01000, 0, 0, 15, 0);
                                Thread.Sleep(1000);
                                t++;
                                if (t > 100) break;
                            }
            while (double.Parse(z3dvp.checkvalue(Locators.CssSelector.LeftTopPane, 2).ToString()) >= 0.0);
                        }catch(Exception e)   {
                            Logger.Instance.ErrorLog(e.Message);
                        }
                    }
                    else {
                        do
                        {
                            BasePage.mouse_event(0x0800, 0, 0, -8, 0);
                            Thread.Sleep(1000);
                            t++;
                            if (t > 100) break;
                        }

                        while (z3dvp.checkvalue(Locators.CssSelector.LeftTopPane, 2) >= 66);
                    }
                    List<string> result4 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                    DownloadImageFile(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), AFilename_navigationone);
                    DownloadImageFile(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo), AFilename_navigationtwo);
                    bool bcompareNavigationone = CompareImage(bFilename_navigationoneone, AFilename_navigationone);
                    bool bcompareNavigationtwo = CompareImage(bFilename_navigationtwo, AFilename_navigationtwo);
                    if (bcompareNavigationone == false && bcompareNavigationtwo == false && result4[0] == result4[1] && result4[2] == result4[3])
                    {
                        bflag4 = true;
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                }
                if (bflag4 == false)
                {

                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //step 4 Scroll through the whole volume until the intersection of the red and blue crosshairs are at the bottom of the image displayed on the MPR navigation control 1.
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((ViewerContainer4.Location.X + 850), (ViewerContainer4.Location.Y / 2 + 580));
                double lvalues4 = 0;
                int i = 0;
                if (Config.BrowserType == "firefox" && Config.BrowserType == "mozilla")
                {
                    do
                    {
                        BasePage.mouse_event(0x01000, 0, 0, 50, 0);
                        Thread.Sleep(1000);
                        i++;
                        if (i > 100) break;
                    }
                    while (z3dvp.checkvalue(Locators.CssSelector.LeftTopPane, 2) >= -80);
                }
                else
                {
                    do
                    {
                        BasePage.mouse_event(0x0800, 0, 0, -50, 0);
                        Thread.Sleep(1000);
                        i++;
                        if (i > 100) break;
                    }
                    while (z3dvp.checkvalue(Locators.CssSelector.LeftTopPane, 2) >= -80);
                }
               
    
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                //  z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, false);
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(500);
                z3dvp.SelectControl(BluRingZ3DViewerPage.ResultPanel);
                Thread.Sleep(2000);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
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
                // z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, true);
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(500);
                //step 5 Select the reset option from the 3D tool box.
                z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(10000);
                List<string> result6 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result6[0] == result6[1] && result6[1] == result6[2])
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

                //step 6  Repeat step 2-5 but instead of scrolling using the mouse wheel, click the left mouse button and drag the mouse downwards. 
                //Frist step : Click on the MPR navigation control 1 and position the intersection of the red and blue crosshairs at the top of the image displayed.
                bool bflag6_1 = false;
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationone);
                Actions act7 = new Actions(Driver);
                act7.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2)
                    .ClickAndHold()
                    .MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height - 310).Release().Build().Perform();
                Thread.Sleep(2000);
                //   z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, false);
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(500);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Thread.Sleep(1000);
                try
                {
                    if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree)))
                    {
                        bflag6_1 = true;
                    }
                    if (bflag6_1 == false)
                    {
                        Logger.Instance.InfoLog("Test Step Fail bflag6_1");
                    }
                    new Actions(Driver).SendKeys("T").Build().Perform();
                    Thread.Sleep(500);
                    //Second Step : On the MPR Result control, select "Navigation 3" to be the source control from the drop down list.Click on the MPR result control and click the left mouse button and drag the mouse downwards.
                    bool bflag6_2 = false;
                    string b8Filename_navigationoneone = testcasefolder + "step8_bNavigationone.png";
                    string A8Filename_navigationone = testcasefolder + "step8_ANavigationone.png";
                    string b8Filename_navigationtwo = testcasefolder + "step8_bNavigationtwo.png";
                    string A8Filename_navigationtwo = testcasefolder + "step8_ANavigationtwo.png";
                    DownloadImageFile(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), b8Filename_navigationoneone);
                    DownloadImageFile(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo), b8Filename_navigationtwo);
                    bool bflag8 = false;
                    IWebElement Iresultpanel = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                    z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.ResultPanel);
                    List<string> ResultpanelNav8 = z3dvp.GetControlvalues(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationthree);
                    if (ResultpanelNav8[3] == BluRingZ3DViewerPage.Navigationthree)
                    {
                        bool btool10 = z3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                        z3dvp.SelectControl(BluRingZ3DViewerPage.ResultPanel);
                        this.Cursor = new Cursor(Cursor.Current.Handle);
                        Cursor.Position = new Point((ViewerContainer4.Location.X + 900), (ViewerContainer4.Location.Y / 2 + 600));
                        Actions act8 = new Actions(Driver);
                        int m = 0;
                        if (Config.BrowserType.ToLower() == "firefox" || Config.BrowserType.ToLower() == "mozilla")
                        {
                            try
                            {
                                do
                                {
                                    BasePage.mouse_event(0x01000, 0, 0, 15, 0);
                                    Thread.Sleep(1000);
                                    m++;
                                    if (m > 100) break;
                                }

                                while (double.Parse(z3dvp.checkvalue(Locators.CssSelector.LeftTopPane, 2).ToString()) >= 0.0);
                            }
                            catch (Exception e)
                            {
                                Logger.Instance.ErrorLog(e.Message);
                            }
                        }
                        else
                        {
                            try
                            {
                                do
                                {
                                    BasePage.mouse_event(0x0800, 0, 0, -15, 0);
                                    Thread.Sleep(1000);
                                    m++;
                                    if (m > 100) break;
                                }

                                while (double.Parse(z3dvp.checkvalue(Locators.CssSelector.LeftTopPane, 2).ToString()) >= 0.0);
                            }
                            catch (Exception e)
                            {
                                Logger.Instance.ErrorLog(e.Message);
                            }
                        }
                     
                        Thread.Sleep(7000);
                        DownloadImageFile(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), A8Filename_navigationone);
                        DownloadImageFile(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo), A8Filename_navigationtwo);
                        bool b8compareNavigationone = CompareImage(bFilename_navigationoneone, A8Filename_navigationone);
                        bool b8compareNavigationtwo = CompareImage(bFilename_navigationtwo, A8Filename_navigationtwo);
                        List<string> result8 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                        if (b8compareNavigationone == false && b8compareNavigationtwo == false && result8[0] == result8[1] && result8[2] == result8[3])
                        {
                            bflag6_2 = true;
                        }
                    }
                    if (bflag6_2 == false)
                    {
                        Logger.Instance.InfoLog("TEst Step fail bflag6_2=false");
                    }
                    //Third Step  Drag through the whole volume until the intersection of the red and blue crosshairs are at the bottom of the image displayed on the MPR navigation control 1.

                    bool bflag6_3 = false;
                    z3dvp.SelectControl(BluRingZ3DViewerPage.ResultPanel);
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((ViewerContainer4.Location.X + 850), (ViewerContainer4.Location.Y / 2 + 580));
                    Actions act9 = new Actions(Driver);
                    int t = 0;
                    if (Config.BrowserType.ToLower() == "firefox" || Config.BrowserType.ToLower() == "mozilla")
                    {
                        do
                        {
                            BasePage.mouse_event(0x01000, 0, 0, 50, 0);
                            Thread.Sleep(1000);
                            t++;
                            if (t > 300) break;
                        }
                        while (z3dvp.checkvalue(Locators.CssSelector.LeftTopPane, 2) >= -80);
                    }
                    else
                    {
                        do
                        {
                            BasePage.mouse_event(0x0800, 0, 0, -50, 0);
                            Thread.Sleep(1000);
                            t++;
                            if (t > 300) break;
                        }
                        while (z3dvp.checkvalue(Locators.CssSelector.LeftTopPane, 2) >= -80);
                    }
              
                   
                    Thread.Sleep(10000);
                    List<string> result_3 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                    string[] arrsplit3 = null;
                    arrsplit3 = result_3[2].Split(',');
                    string[] arrsplit_3b = null;
                    arrsplit_3b = result_3[3].Split(',');
                    string sub1 = arrsplit3[2].Substring(1, 3).Trim().Replace(".", String.Empty);
                    string sub2 = arrsplit3[2].Substring(1, 3).Trim().Replace(".", String.Empty);
                    string sub1_a = arrsplit_3b[2].Substring(1, 3).Trim().Replace(".", String.Empty);
                    string sub2_b = arrsplit_3b[2].Substring(1, 3).Trim().Replace(".", String.Empty);
                    if (int.Parse(sub1) < 45 && int.Parse(sub1) < 60 && int.Parse(sub1_a) < 45 && int.Parse(sub2_b) < 60)
                    {
                        bflag6_3 = true;
                    }
                    if (bflag6_3 == false)
                    {
                        Logger.Instance.InfoLog("Test Step Fail bflag6_3");
                    }
                    //z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, true);
                    //Four STep: Press  the reset button 
                    bool bflag6_4 = false;
                    z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                    Thread.Sleep(10000);
                    List<string> result10 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                    if (result10[0] == result10[1] && result10[1] == result10[2])
                    {
                        bflag6_4 = true;
                    }
                    if (bflag6_4 == false)
                    {
                        Logger.Instance.InfoLog("Test Step Fail bflag6_4");
                    }
                    if (bflag6_1 == true && bflag6_2 == true && bflag6_3 == true && bflag6_4 == true)
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
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Exception in  Step6" + e.ToString());
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(500);
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

                //Return Result
                return result;
            }
            finally
            {
                z3dvp.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_163355(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            WpfObjects wpfobject = new WpfObjects();
            BasePage basepage = new BasePage();
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
                login.LoginIConnect(username, password);
                //Step 1  From the Universal viewer , Select a 3D supported series and Select the MPR option from the drop down
                z3dvp.Deletefiles(testcasefolder);
                bool res = z3dvp.searchandopenstudyin3D(Patientid, thumbnailcaption);
                if (res == true)
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
                //step 2 Right click the mouse on the image in mpr result control and select the Roam tool from the 3D toolbox
                bool btool2 = z3dvp.select3DTools(Z3DTools.Pan, BluRingZ3DViewerPage.ResultPanel);

                if (btool2)
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
                //step 3 Click and hold the left mouse button on the image displayed on the result control and move the mouse.
                z3dvp.DragandDropelement(BluRingZ3DViewerPage.ResultPanel);
                Thread.Sleep(10000);
                List<string> result2 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result2[0] == result2[1] && result2[1] == result2[2] && result2[3] == result2[2])
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

        public TestCaseResult Test_163366(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            BluRingViewer viewer = new BluRingViewer();


            WpfObjects wpfobject = new WpfObjects();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            String sPatientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String Testdata_req = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            string[] split_testdata = Testdata_req.Split('|');
            string sHeadingPatientID = split_testdata[0];
            string sHeadingAccession = split_testdata[1];
            string sAccessionValue = split_testdata[2];
            string sThickness2 = split_testdata[3];
            string srestvalue = split_testdata[4];


            String thumbnailcaption = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            try
            {
                login.LoginIConnect(username, password);

                //Step 1   From iCA, Load a study in Z3D viewer. By default MPR viewing mode should dipslay
                z3dvp.Deletefiles(testcasefolder);
                login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                login.SearchStudy(sHeadingPatientID, sPatientid);
                PageLoadWait.WaitForLoadingMessage(30);
                login.SelectStudy(sHeadingAccession, sAccessionValue);
                PageLoadWait.WaitForFrameLoad(5);
                //Step 1 From iCA, Load the the study " TEST, FFS" in 3D viewer. 1.Navigate to 3D tab and Click MPR mode from the dropdown.Note: This is new design(could change)
                var viewer1 = BluRingViewer.LaunchBluRingViewer(fieldname: sHeadingAccession, value: sAccessionValue);
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                bool res = z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR, "n");
                Thread.Sleep(5000);
                bool bflag = false;
                if (res == true)
                {
                    List<string> sAnnotaion = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 1);
                    if (sAnnotaion[0] == sAccessionValue && sAnnotaion[1] == sAccessionValue && sAnnotaion[2] == sAccessionValue && sAnnotaion[3] == sAccessionValue)
                    {
                        bflag = true;
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                }
                if (bflag == false)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 2 Increase the thickness of the MPR navigation control 1 to 100.0 mm on the bottom left corner of the control.
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                IList<IWebElement> Viewport2 = z3dvp.Viewport();
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
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

                //Step 3 Select 'Navigation 1' from the render mode drop down list in the MPR result control so that it is synchronized with the MPR navigation control 1. Increase the thickness of the MPR result control to 100.0 mm on the bottom left corner of the control. 
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.ResultPanel);
                z3dvp.EnterThickness(BluRingZ3DViewerPage.ResultPanel, sThickness2);
                IWebElement ithumnail = z3dvp.IthumNail();
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
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

                //Step 4 Note the orientation marker displayed at the top center of the MPR result control are synchronized with the orientation marker in the MPR navigation control 1. Select the rotate tool from the 3D toolbox. Click the top center of the MPR navigation control 1 and drag down slowly. Note the orientation marker of the MPR result control as you drag down. 
                List<string> before_result4 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                bool btool4 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                Actions act4_a = new Actions(Driver);
                act4_a.SendKeys("X").Build().Perform();
                Actions act4 = new Actions(Driver);
                IWebElement Inavigationone = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                act4.MoveToElement(Inavigationone, Inavigationone.Size.Width / 2, Inavigationone.Size.Height / 2).ClickAndHold()
                .MoveToElement(Inavigationone, Inavigationone.Size.Width / 2, Inavigationone.Size.Height / 2 + 100)
                .Release()
                .Build()
                .Perform();
                Thread.Sleep(10000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                List<string> result4 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                if (result4[0] == result4[3] && before_result4[0] != result4[0] && before_result4[0] != result4[3])
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
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);
                //Step 5 Click the reset button 
                bool btool5 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
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
                //step 6 Increase the thickness of the MPR navigation control 1 to 100.0 mm on the bottom left corner of the control. 
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
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
                //step 7 Select 'Navigation 1' from the render mode drop down list in the MPR result control so that it is synchronized with the MPR navigation control 1. Increase the thickness of the MPR result control to 100.0 mm on the bottom left corner of the control. 
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.ResultPanel);
                z3dvp.EnterThickness(BluRingZ3DViewerPage.ResultPanel, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
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
                //step 8 Note the orientation marker displayed at the left side of the MPR result control is synchronized with the orientation marker in the MPR navigation control 1. Select the rotate tool from the Z3D toolbar. Click on the center of the MPR navigation control 1 and drag to the right slowly. Note the left orientation marker of the MPR result control as you drag to the right. 
                bool btool8 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                Actions act8_a = new Actions(Driver);
                act8_a.SendKeys("X").Build().Perform();
                List<string> Beforeresult8 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                Actions act8 = new Actions(Driver);
                act8.MoveToElement(Inavigationone, Inavigationone.Size.Width / 2, Inavigationone.Size.Height / 2).ClickAndHold()
                .MoveToElement(Inavigationone, Inavigationone.Size.Width / 2 + 100, Inavigationone.Size.Height / 2)
                .Release()
                .Build()
                .Perform();
                Thread.Sleep(10000);
                //for firefox purpose vales not updating 
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                List<string> result8 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                if (result8[0] == result8[3] && Beforeresult8[1] != result8[1] && Beforeresult8[3] != result8[3])
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
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);

                //step 9 Select the reset option from the 3D tool box
                bool btool9 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                //  z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, false);
                new Actions(Driver).SendKeys("D").Build().Perform();
                Thread.Sleep(500);
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
                //  z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, true);
                new Actions(Driver).SendKeys("D").Build().Perform();
                Thread.Sleep(500);
                //step 10  Increase the thickness of the MPR navigation control 1 to 100.0 mm on the bottom left corner of the control. 
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                IList<IWebElement> Viewport10 = z3dvp.Viewport();
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport10[0].Size.Width / 2) + 600, (Viewport10[0].Size.Height / 2 + 100));
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

                //step 11  Select 'Navigation 1' from the render mode drop down list in the MPR result control so that it is synchronized with the MPR navigation control 1. Increase the thickness of the MPR result control to 100.0 mm on the bottom left corner of the control. 
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.ResultPanel);
                Thread.Sleep(10000);
                z3dvp.EnterThickness(BluRingZ3DViewerPage.ResultPanel, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport10[0].Size.Width / 2) + 600, (Viewport10[0].Size.Height / 2 + 100));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
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

                //step 12 Note the orientation marker displayed at the right side of the MPR result control is synchronized with the orientation marker in the MPR navigation control 1. Select the rotate tool from the Z3D toolbar. Click on the center of the MPR navigation control 1 and drag to the left slowly. Note the right orientation of the MPR result control marker as you drag to the left. 
                bool btool12 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                Actions act12_a = new Actions(Driver);
                act12_a.SendKeys("X").Build().Perform();

                //Before Action 
                List<string> Beforeresult12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                Actions act12 = new Actions(Driver);
                act12.MoveToElement(Inavigationone, Inavigationone.Size.Width / 2, Inavigationone.Size.Height / 2).ClickAndHold()
                .MoveToElement(Inavigationone, Inavigationone.Size.Width / 2 - 100, Inavigationone.Size.Height / 2)
                .Release()
                .Build()
                .Perform();
                Thread.Sleep(10000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                List<string> result12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                if (result12[0] == result12[3] && Beforeresult12[2] != result12[2] && Beforeresult12[3] != result12[3])
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
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);
                //step 13 click the reset button 
                bool bool13 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(5000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport10[0].Size.Width / 2) + 600, (Viewport10[0].Size.Height / 2 + 100));
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

                //step 14  From iCA, Load the the study " TEST, FFS" in 3D viewer.1.Navigate to 3D tab and Click 3D 6:1 mode from the dropdown.Note: This is new design(could change)
                bool res14 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6, "n");
                Thread.Sleep(5000);
                bool lbflag_14 = false;
                if (res14 == true)
                {
                    List<string> sAnnotaion = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 1);
                    if (sAnnotaion[0] == sAccessionValue && sAnnotaion[1] == sAccessionValue && sAnnotaion[2] == sAccessionValue && sAnnotaion[3] == sAccessionValue && sAnnotaion[4] == sAccessionValue && sAnnotaion[5] == sAccessionValue)
                    {
                        lbflag_14 = true;
                    }
                }
                Thread.Sleep(1000);
                //step 15 Increase the thickness of the MPR navigation control 1 to 100.0 mm on the bottom left corner of the control. 
                bool lbflag_15 = false;
                IList<IWebElement> Viewport15 = z3dvp.Viewport();
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport10[0].Size.Width / 2) + 600, (Viewport10[0].Size.Height / 2 + 100));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
                {
                    lbflag_15 = true;
                }
                Thread.Sleep(1000);
                //step 16 Select 'Navigation 1' from the render mode drop down list in the MPR result control so that it is synchronized with the MPR navigation control 1. Increase the thickness of the MPR result control to 100.0 mm on the bottom left corner of the control. 
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.ResultPanel);
                z3dvp.EnterThickness(BluRingZ3DViewerPage.ResultPanel, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                bool lbflag_16 = false; this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport10[0].Size.Width / 2) + 600, (Viewport10[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
                {
                    lbflag_16 = true;
                }
                Thread.Sleep(1000);
                //step 17 Note the orientation marker displayed at the top center of the MPR result control are synchronized with the orientation marker in the MPR navigation control 1. Select the rotate tool from the 3D toolbox. Click the top center of the MPR navigation control 1 and drag up slowly. Note the orientation marker of the MPR result control as you drag up. 
                bool btool17 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                Actions act17_a = new Actions(Driver);
                act17_a.SendKeys("X").Build().Perform();
                bool lbflag_17 = false;
                //BeforeEvent Action
                List<string> Beforeresult17 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                Actions act17 = new Actions(Driver);
                act17.MoveToElement(Inavigationone, Inavigationone.Size.Width / 2, Inavigationone.Size.Height / 2).ClickAndHold()
                .MoveToElement(Inavigationone, Inavigationone.Size.Width / 2, Inavigationone.Size.Height / 2 + 100)
                .Release()
                .Build()
                .Perform();
                Thread.Sleep(10000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                List<string> result17 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                if (result17[0] == result17[4] && Beforeresult17[0] != result17[0] && Beforeresult17[4] != result17[4])
                {
                    lbflag_17 = true;
                }
                Actions act17_b = new Actions(Driver);
                act17_b.SendKeys("X").Build().Perform();
                Thread.Sleep(1000);
                //Step 18 click the reset button 
                bool btool18 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(5000);
                bool lbflag_18 = false;
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport10[0].Size.Width / 2) + 600, (Viewport10[0].Size.Height / 2 + 100));
                //z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, false);
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(500);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
                {
                    lbflag_18 = true;
                }
                Thread.Sleep(1000);
                //  z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, true);
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(500);
                //step 19 Increase the thickness of the MPR navigation control 1 to 100.0 mm on the bottom left corner of the control. 
                IList<IWebElement> Viewport19 = z3dvp.Viewport();
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                bool lbflag_19 = false;
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport10[0].Size.Width / 2) + 600, (Viewport10[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
                {
                    lbflag_19 = true;
                }
                Thread.Sleep(1000);
                //Step 20 Select 'Navigation 1' from the render mode drop down list in the MPR result control so that it is synchronized with the MPR navigation control 2. Increase the thickness of the MPR result control to 100.0 mm on the bottom left corner of the control. 
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.ResultPanel);
                z3dvp.EnterThickness(BluRingZ3DViewerPage.ResultPanel, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport10[0].Size.Width / 2) + 600, (Viewport10[0].Size.Height / 2 + 100));
                bool lbflag_20 = false;
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 5);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
                {
                    lbflag_20 = true;
                }
                Thread.Sleep(1000);
                //Step 21 Note the orientation marker displayed at the left side of the MPR result control is synchronized with the orientation marker in the MPR navigation control 2. Select the rotate tool from the 3D toolbox. Click on the center of the MPR navigation control 2 and drag to the right slowly. Note the left orientation marker of the MPR result control as you drag to the right. 
                bool btool20 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                Actions act20_a = new Actions(Driver);
                act20_a.SendKeys("X").Build().Perform();
                Thread.Sleep(1000);
                bool lbflag_21 = false;
                List<string> before_result20 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                Actions act20 = new Actions(Driver);
                act20.MoveToElement(Inavigationone, Inavigationone.Size.Width / 2, Inavigationone.Size.Height / 2).ClickAndHold()
                .MoveToElement(Inavigationone, Inavigationone.Size.Width / 2 + 100, Inavigationone.Size.Height / 2)
                .Release()
                .Build()
                .Perform();
                Thread.Sleep(10000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                List<string> result20 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                if (result20[0] == result20[4] && before_result20[1] != result20[1] && before_result20[4] != result20[4])
                {
                    lbflag_21 = true;
                }
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);
                //Step 22 Click the reset button. 
                bool btool22 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                bool lbflag_22 = false;
                Thread.Sleep(5000);
                if (btool22)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport10[0].Size.Width / 2) + 600, (Viewport10[0].Size.Height / 2 + 100));
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 6);
                    if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
                    {
                        lbflag_22 = true;
                    }
                }
                Thread.Sleep(1000);
                //Step 23 Increase the thickness of the MPR navigation control 1 to 100.0 mm on the bottom left corner of the control. 
                IList<IWebElement> Viewport23 = z3dvp.Viewport();
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                bool lbflag_23 = false;
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport10[0].Size.Width / 2) + 600, (Viewport10[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 7);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
                {
                    lbflag_23 = true;
                }
                Thread.Sleep(1000);
                //step 24 Select 'Navigation 1' from the render mode drop down list in the MPR result control so that it is synchronized with the MPR navigation control 1. Increase the thickness of the MPR result control to 100.0 mm on the bottom left corner of the control. 
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.ResultPanel);
                z3dvp.EnterThickness(BluRingZ3DViewerPage.ResultPanel, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport10[0].Size.Width / 2) + 600, (Viewport10[0].Size.Height / 2 + 100));
                bool lbflag_24 = false;
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 8);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
                {
                    lbflag_24 = true;
                }
                Thread.Sleep(1000);
                //step 25 Note the orientation marker displayed at the right side of the MPR result control is synchronized with the orientation marker in the MPR navigation control 3. Select the rotate tool from the 3D toolbox. Click on the center of the MPR navigation control 3 and drag to the left slowly. Note the right orientation of the MPR result control marker as you drag to the left. 
                bool btool25 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                Actions act25_a = new Actions(Driver);
                act25_a.SendKeys("X").Build().Perform();
                bool lbflag_25 = false;
                List<string> beforeresult25 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                Actions act25 = new Actions(Driver);
                act25.MoveToElement(Inavigationone, Inavigationone.Size.Width / 2, Inavigationone.Size.Height / 2).ClickAndHold()
                .MoveToElement(Inavigationone, Inavigationone.Size.Width / 2 - 100, Inavigationone.Size.Height / 2)
                .Release()
                .Build()
                .Perform();
                Thread.Sleep(10000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                List<string> result25 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                if (result25[0] == result25[4] && beforeresult25[3] != result25[3] && beforeresult25[3] != result25[4])
                {
                    lbflag_25 = true;
                }
                Thread.Sleep(1000);
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);
                //step 26  click the reset button 
                bool btool26 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                bool lbflag_26 = false;
                Thread.Sleep(5000);
                if (btool26)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport10[0].Size.Width / 2) + 600, (Viewport10[0].Size.Height / 2 + 100));
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 9);
                    if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
                    {
                        lbflag_26 = true;
                    }
                }
                Thread.Sleep(1000);
                if (lbflag_14 && lbflag_15 && lbflag_16 && lbflag_17 && lbflag_18 && lbflag_19 && lbflag_20 && lbflag_21 && lbflag_22 && lbflag_23 && lbflag_24 && lbflag_25 && lbflag_26)
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

        public TestCaseResult Test_163367(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            BluRingViewer viewer = new BluRingViewer();


            WpfObjects wpfobject = new WpfObjects();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            String sPatientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String Testdata_req = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            string[] split_testdata = Testdata_req.Split('|');
            string sHeadingPatientID = split_testdata[0];
            string sHeadingAccession = split_testdata[1];
            string sAccessionValue = split_testdata[2];
            string sThickness2 = split_testdata[3];
            string sresetvalue = split_testdata[4];
            //  string sThickness12 = split_testdata[4];

            String thumbnailcaption = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            try
            {
                login.LoginIConnect(username, password);

                //Step 1   From the Universal viewer , Select a 3D supported series and Select the MPR option from the drop down.Study: " HFP, ANONYMOUS".
                z3dvp.Deletefiles(testcasefolder);
                login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                login.SearchStudy(sHeadingPatientID, sPatientid);
                PageLoadWait.WaitForLoadingMessage(30);
                login.SelectStudy(sHeadingAccession, sAccessionValue);
                PageLoadWait.WaitForFrameLoad(5);
                //Step 1 From iCA, Load the the study " TEST, FFS" in 3D viewer. 1.Navigate to 3D tab and Click MPR mode from the dropdown.Note: This is new design(could change)
                var viewer1 = BluRingViewer.LaunchBluRingViewer(fieldname: sHeadingAccession, value: sAccessionValue);
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                bool res = z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR, "n");
                Thread.Sleep(5000);
                bool bflag = false;
                if (res == true)
                {
                    List<string> sAnnotaion = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 1);
                    if (sAnnotaion[0] == sAccessionValue && sAnnotaion[1] == sAccessionValue && sAnnotaion[2] == sAccessionValue && sAnnotaion[3] == sAccessionValue)
                    {
                        bflag = true;
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                }
                if (bflag == false)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 2  Increase the thickness of the MPR navigation control 1 to 100.0 mm on the bottom left corner of the control. 
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                IWebElement ithumnail = z3dvp.IthumNail();
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                IList<IWebElement> Viewport2 = z3dvp.Viewport();
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                Thread.Sleep(1000);
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

                //Step 3 Select 'Navigation 1' from the render mode drop down list in the MPR result control so that it is synchronized with the MPR navigation control 1. Increase the thickness of the MPR result control to 100.0 mm on the bottom left corner of the control. 
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.ResultPanel);
                z3dvp.EnterThickness(BluRingZ3DViewerPage.ResultPanel, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
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

                //Step 4 Note the orientation marker displayed at the top center of the MPR result control are synchronized with the orientation marker in the MPR navigation control 1. Select the rotate tool from the Z3D toolbar. Click the top center of the MPR navigation control 1 and drag down slowly. Note the orientation marker of the MPR result control as you drag down. 
                bool btool4 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                List<string> before_result4 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                IWebElement Inavigationone = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                Actions act4_a = new Actions(Driver);
                act4_a.SendKeys("X").Build().Perform();
                Actions act4 = new Actions(Driver);
                act4.MoveToElement(Inavigationone, Inavigationone.Size.Width / 2, Inavigationone.Size.Height / 2).ClickAndHold()
                .MoveToElement(Inavigationone, Inavigationone.Size.Width / 2, Inavigationone.Size.Height / 2 + 100)
                .Release()
                .Build()
                .Perform();
                Thread.Sleep(10000);
                //For firefox purpose 
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                List<string> result4 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                if (result4[0] == result4[3] && before_result4[0] != result4[0] && before_result4[0] != result4[3])
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
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);

                //Step 5 Click the reset button 
                bool btool5 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                Thread.Sleep(1000);
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

                //step 6  Increase the thickness of the MPR navigation control 1 to 100.0 mm on the bottom left corner of the control. 
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                Thread.Sleep(1000);
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

                //Step 7  Select 'Navigation 1' from the render mode drop down list in the MPR result control so that it is synchronized with the MPR navigation control 1. Increase the thickness of the MPR result control to 100.0 mm on the bottom left corner of the control. 
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.ResultPanel);
                z3dvp.EnterThickness(BluRingZ3DViewerPage.ResultPanel, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Thread.Sleep(1000);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
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

                //Step 8  Note the orientation marker displayed at the left side of the MPR result control is synchronized with the orientation marker in the MPR navigation control 2. Select the rotate tool from the 3D toolbox. Click on the center of the MPR navigation control 2 and drag to the right slowly. Note the left orientation marker of the MPR result control as you drag to the right. 
                bool btool8 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                //Before actions 
                List<string> Beforeresult8 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                Actions act8_a = new Actions(Driver);
                act8_a.SendKeys("X").Build().Perform();
                Thread.Sleep(500);
                Actions act8 = new Actions(Driver);
                act8.MoveToElement(Inavigationone, Inavigationone.Size.Width / 2, Inavigationone.Size.Height / 2).ClickAndHold()
                .MoveToElement(Inavigationone, Inavigationone.Size.Width / 2 + 100, Inavigationone.Size.Height / 2)
                .Release()
                .Build()
                .Perform();
                Thread.Sleep(10000);
                //For firefox purpose 
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                //After Actions 
                List<string> result8 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                if (result8[0] == result8[3] && Beforeresult8[1] != result8[1] && Beforeresult8[3] != result8[3])
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
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(500);
                //step 9 Click the reset Button 
                bool btool9 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                Thread.Sleep(1000);
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

                //step 10  Increase the thickness of the MPR navigation control 1 to 100.0 mm on the bottom left corner of the control. 
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                Thread.Sleep(1000);
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

                //Step 11 Select 'Navigation 1' from the render mode drop down list in the MPR result control so that it is synchronized with the MPR navigation control 1. Increase the thickness of the MPR result control to 100.0 mm on the bottom left corner of the control. 
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.ResultPanel);
                z3dvp.EnterThickness(BluRingZ3DViewerPage.ResultPanel, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                new Actions(Driver).SendKeys("T").Build().Perform();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Thread.Sleep(1000);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                }
                //z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, true);
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(500);
                //Step 12 Note the orientation marker displayed at the right side of the MPR result control is synchronized with the orientation marker in the MPR navigation control 1. Select the rotate tool from the Z3D toolbar. Click on the center of the MPR navigation control 1 and drag to the left slowly. Note the right orientation of the MPR result control marker as you drag to the left. 
                bool btool12 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                //Before Actions 
                List<string> Beaoferresult12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                Actions act12_a = new Actions(Driver);
                act12_a.SendKeys("X").Build().Perform();
                Actions act12 = new Actions(Driver);
                act12.MoveToElement(Inavigationone, Inavigationone.Size.Width / 2, Inavigationone.Size.Height / 2).ClickAndHold()
                .MoveToElement(Inavigationone, Inavigationone.Size.Width / 2 - 100, Inavigationone.Size.Height / 2)
                .Release()
                .Build()
                .Perform();
                Thread.Sleep(10000);
                //For firefox purpose 
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                //Afer Actions 
                List<string> result12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                if (result12[0] == result12[3] && Beaoferresult12[2] != result12[2] && Beaoferresult12[3] != result12[3])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(500);

                //Step 13 Click the Reset Button 
                bool btool13 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                Thread.Sleep(1000);
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

                //step 14 From iCA, Load a study " HFP, ANONYMOUS" in the 3D viewer.1.Navigate to 3D tab and Click 3D 6:1 mode from the dropdown.Note: This is new design(could change)
                bool res14 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6, "n");
                bool bflag14 = false;
                if (res14 == true)
                {
                    List<string> sAnnotaion = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 1);
                    if (sAnnotaion[0] == sAccessionValue && sAnnotaion[1] == sAccessionValue && sAnnotaion[2] == sAccessionValue && sAnnotaion[3] == sAccessionValue && sAnnotaion[4] == sAccessionValue && sAnnotaion[5] == sAccessionValue)
                    {
                        bflag14 = true;
                    }
                }
                Thread.Sleep(1000);
                // Step 15 Increase the thickness of the MPR navigation control 1 to 100.0 mm on the bottom left corner of the control. 
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                bool bflag15 = false;
                Thread.Sleep(1000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
                {
                    bflag15 = true;
                }
                Thread.Sleep(1000);
                //Step 16 Select 'Navigation 1' from the render mode drop down list in the MPR result control so that it is synchronized with the MPR navigation control 1. Increase the thickness of the MPR result control to 100.0 mm on the bottom left corner of the control. 
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.ResultPanel);
                z3dvp.EnterThickness(BluRingZ3DViewerPage.ResultPanel, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                Thread.Sleep(1000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool bflag16 = false;
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
                {
                    bflag16 = true;
                }
                Thread.Sleep(1000);
                //Step 17  Note the orientation marker displayed at the top center of the MPR result control are synchronized with the orientation marker in the MPR navigation control 1. Select the rotate tool from the 3D toolbox. Click the top center of the MPR navigation control 1 and drag up slowly. Note the orientation marker of the MPR result control as you drag up. 
                bool btool17 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                bool bflag17 = false;
                //Before action 
                List<string> Beforeresult17 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                Actions act17_a = new Actions(Driver);
                act17_a.SendKeys("X").Build().Perform();
                Actions act17 = new Actions(Driver);
                act17.MoveToElement(Inavigationone, Inavigationone.Size.Width / 2, Inavigationone.Size.Height / 2).ClickAndHold()
                .MoveToElement(Inavigationone, Inavigationone.Size.Width / 2, Inavigationone.Size.Height / 2 + 100)
                .Release()
                .Build()
                .Perform();
                Thread.Sleep(10000);
                //For firefox purpose 
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                List<string> result17 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                if (result17[0] == result17[4] && Beforeresult17[0] != result17[0] && Beforeresult17[4] != result17[4])
                {
                    bflag17 = true;
                }
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);

                //step 18 Click the reset button
                bool btool18 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                bool bflag18 = false;
                // z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, false);
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(1000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
                {
                    bflag18 = true;
                }
                Thread.Sleep(1000);
                //  z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, true);
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(500);
                //step 19 Increase the thickness of the MPR navigation control 1 to 100.0 mm on the bottom left corner of the control. 
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                bool bflag19 = false;
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
                {
                    bflag19 = true;
                }
                Thread.Sleep(1000);
                //Step 20  Select 'Navigation 1' from the render mode drop down list in the MPR result control so that it is synchronized with the MPR navigation control 2. Increase the thickness of the MPR result control to 100.0 mm on the bottom left corner of the control. 
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.ResultPanel);
                z3dvp.EnterThickness(BluRingZ3DViewerPage.ResultPanel, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                bool bflag20 = false;
                //   z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, false);
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(500);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 5);
                Thread.Sleep(1000);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
                {
                    bflag20 = true;
                }
                Thread.Sleep(1000);
                //  z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, true);
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(500);
                //Step 21 Note the orientation marker displayed at the left side of the MPR result control is synchronized with the orientation marker in the MPR navigation control 2. Select the rotate tool from the 3D toolbox. Click on the center of the MPR navigation control 2 and drag to the right slowly. Note the left orientation marker of the MPR result control as you drag to the right. 
                bool btool20 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                bool bflag21 = false;
                List<string> beoforeresult20 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                Actions act20_a = new Actions(Driver);
                act20_a.SendKeys("X").Build().Perform();
                Actions act20 = new Actions(Driver);
                act20.MoveToElement(Inavigationone, Inavigationone.Size.Width / 2, Inavigationone.Size.Height / 2).ClickAndHold()
                .MoveToElement(Inavigationone, Inavigationone.Size.Width / 2 + 100, Inavigationone.Size.Height / 2)
                .Release()
                .Build()
                .Perform();
                Thread.Sleep(10000);
                //For firefox purpose 
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                List<string> result20 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                if (result20[0] == result20[4] && beoforeresult20[1] != result20[1] && beoforeresult20[1] != result20[4])
                {
                    bflag21 = true;
                }
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);

                //Step 22 Click the reset button. 
                bool btool22 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                bool bflag22 = false;
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                Thread.Sleep(1000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 6);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
                {
                    bflag22 = true;
                }
                Thread.Sleep(1000);
                //Step 23 Increase the thickness of the MPR navigation control 1 to 100.0 mm on the bottom left corner of the control. 
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                bool bfalg23 = false;
                Thread.Sleep(1000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 7);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
                {
                    bfalg23 = true;
                }
                Thread.Sleep(1000);
                //Step 24 Select 'Navigation 1' from the render mode drop down list in the MPR result control so that it is synchronized with the MPR navigation control 3. Increase the thickness of the MPR result control to 100.0 mm on the bottom left corner of the control. 
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.ResultPanel);
                z3dvp.EnterThickness(BluRingZ3DViewerPage.ResultPanel, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 8);
                bool bflag24 = false;
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
                {
                    bflag24 = true;
                }
                Thread.Sleep(1000);
                //step 25 Note the orientation marker displayed at the right side of the MPR result control is synchronized with the orientation marker in the MPR navigation control 3. Select the rotate tool from the 3D toolbox. Click on the center of the MPR navigation control 3 and drag to the left slowly. Note the right orientation of the MPR result control marker as you drag to the left. 
                bool btool25 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                bool bflag25 = false;
                List<string> beforeresult25 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                Actions act25_a = new Actions(Driver);
                act25_a.SendKeys("X").Build().Perform();
                Actions act25 = new Actions(Driver);
                act25.MoveToElement(Inavigationone, Inavigationone.Size.Width / 2, Inavigationone.Size.Height / 2).ClickAndHold()
                .MoveToElement(Inavigationone, Inavigationone.Size.Width / 2 - 100, Inavigationone.Size.Height / 2)
                .Release()
                .Build()
                .Perform();
                Thread.Sleep(10000);
                //For firefox purpose 
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                List<string> result25 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                if (result25[3] == result25[4] && beforeresult25[3] != result25[3] && beforeresult25[4] != result25[4])
                {
                    bflag25 = true;
                }
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);

                //step 26  click the reset button 
                bool btool26 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                bool bflag26 = false;
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                Thread.Sleep(1000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 9);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
                {
                    bflag26 = true;
                }
                if (bflag14 && bflag15 && bflag16 && bflag17 && bflag18 && bflag19 && bflag20 && bflag21 && bflag22 && bfalg23 && bflag24 && bflag25 && bflag26)
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


        public TestCaseResult Test_163362(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String[] TestData = TestDataRequirements.Split('|');
            string Rendermode1 = TestData[0];
            string Rendermode2 = TestData[1];
            string Rendermode3 = TestData[2];
            string Rendermode4 = TestData[3];
            string Rendermode5 = TestData[4];
            string Rendermode6 = TestData[5];
            string Rendermode7 = TestData[6];
            String CursonName = TestData[7];
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step:1 - Navigate to 3D tab and Click MPR mode from the dropdown
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step1 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription, layout: BluRingZ3DViewerPage.MPR);
                IList<IWebElement> Viewport = Z3dViewerPage.Viewport();
                if (step1 && Viewport.Count == 4)
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

                //step:2  -Click on the render modes drop down list displayed at the bottom left corner of the MPR navigation controls
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, "none");
                IList<IWebElement> RenderModes = Z3dViewerPage.RenderModes();
                if (RenderModes[0].Text.Contains(Rendermode1) && RenderModes[1].Text.Contains(Rendermode2) && RenderModes[2].Text.Contains(Rendermode3))
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
                IWebElement Menuclose = Driver.FindElement(By.CssSelector(Locators.CssSelector.MenuClose));
                ClickElement(Menuclose);
                PageLoadWait.WaitForFrameLoad(3);
                //step: 3 -Select the first render mode (MIP) on the drop down list to apply to the volume displayed on the MPR navigation controls
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Mip);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Viewport[3]))
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

                //step:4 - Select the scroll tool from the 3D toolbox
                Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool, "Result");
                Thread.Sleep(1500);
                String step4 = Viewport[3].GetCssValue("cursor");
                if (step4.Contains(CursonName))
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

                //step:5 - Scroll through the volume displayed on the MPR result control
                IWebElement Viewercontainer = Z3dViewerPage.ViewerContainer();
                Z3dViewerPage.Performdragdrop(Viewport[3], Viewport[3].Size.Width / 2, (Viewport[3].Size.Height / 2) - 125);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    if (CompareImage(result.steps[ExecutedSteps], Viewercontainer,pixelTolerance: 100))
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

                //step:6  -Render mode (MinIP) is applied to the volume displayed on the MPR result control
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.MinIp);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Boolean step6_1 = CompareImage(result.steps[ExecutedSteps], Viewport[3]);
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Average);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                Boolean step6_2 = CompareImage(result.steps[ExecutedSteps], Viewport[3]);
                Logger.Instance.InfoLog("ICA-17184 Faded MIP");
                if (step6_1 && step6_2)
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

                //step:7 - Select the Render mode list under the top right corner options of MPR result control and select the 3D slab option
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Slab3D);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Viewport[3]))
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

        public TestCaseResult Test_163369(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string WarningMsg = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {

                //step:1 - Navigate to 3D tab and Click MPR mode from the dropdown
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step1 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription, layout: BluRingZ3DViewerPage.MPR);
                IList<IWebElement> Viewport = Z3dViewerPage.Viewport();
                if (step1 && Viewport.Count == 4)
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

                //step:2  -Click on the render modes drop down list displayed at the bottom left corner of the MPR navigation controls
                Z3dViewerPage.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center);
                Z3dViewerPage.Performdragdrop(Viewport[3], Viewport[3].Size.Width / 2, (Viewport[3].Size.Height / 2) - 125);
                Boolean step2_1 = Z3dViewerPage.PopwindowwarnMsg().Text.Equals(WarningMsg);
                Boolean step2_2 = Z3dViewerPage.checkerrormsg("y");
                if (step2_1 && step2_2)
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

                //step:3 - Select the rotate tool in 'Image Center' mode. Try to rotate the images displayed in the MPR result control. 
                Z3dViewerPage.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, "Result");
                Z3dViewerPage.Performdragdrop(Viewport[3], Viewport[3].Size.Width / 2, (Viewport[3].Size.Height / 2) - 125);
                Boolean step3_1 = Z3dViewerPage.PopwindowwarnMsg().Text.Equals(WarningMsg);
                Boolean step3_2 = Z3dViewerPage.checkerrormsg("y");
                if (step3_1 && step3_2)
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

                //step:4 - Repeat steps 2 and 3 on all views that have a MPR result control.
                //ClickElement(Z3dViewerPage.ExitIcon());
                //PageLoadWait.WaitForFrameLoad(5);
                //Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription, layout: BluRingZ3DViewerPage.Three_3d_6);
                 Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_6, "y");
                Viewport = Z3dViewerPage.Viewport();
                Z3dViewerPage.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center);
                Z3dViewerPage.Performdragdrop(Viewport[4], Viewport[4].Size.Width / 2, (Viewport[4].Size.Height / 2) - 125);
                Boolean step4_1 = Z3dViewerPage.PopwindowwarnMsg().Text.Equals(WarningMsg);
                Boolean step4_2 = Z3dViewerPage.checkerrormsg("y");
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

        public TestCaseResult Test_163354(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string CursonName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1 - Navigate to 3D tab and Click MPR mode from the dropdown. Note: This is new design(could change)
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step1 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription);
                IList<IWebElement> Viewport = Z3dViewerPage.Viewport();
                if (step1 && Viewport.Count == 4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Error in loading Study");
                }

                //step:2  -Zoom cursor shows up while hovering over the image.
                Z3dViewerPage.select3DTools(Z3DTools.Interactive_Zoom, "Result");
                Thread.Sleep(1500);
                String step3 = Viewport[3].GetCssValue("cursor");
                if (step3.Contains(CursonName))
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

                //step:3  -Pixel that was initially clicked moves to the center of the control and the image magnification increases.
                String step3_Before = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                if (Config.BrowserType.ToLower() == "mozilla" || Config.BrowserType.ToLower() == "firefox")
                {
                    new Actions(Driver).MoveToElement(Viewport[3], Viewport[3].Size.Width / 4 - 10, Viewport[3].Size.Height / 4 - 5).ClickAndHold().
         DragAndDropToOffset(Viewport[3], Viewport[3].Size.Width / 4 - 10, Viewport[3].Size.Height / 4 - 20).
         Release().Build().Perform();
                    Thread.Sleep(1000);
                }
                else
                {
                    Z3dViewerPage.Performdragdrop(Viewport[3], (Viewport[3].Size.Width / 2) - 50, (Viewport[3].Size.Height / 2) - 75, (Viewport[3].Size.Width / 2) - 50, (Viewport[3].Size.Height / 2) - 50);
                }
                String step3_After = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                if (!step3_Before.Equals(step3_After))
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

                //step:4 - Pixel that was initially clicked moves to the center of the control and the image magnification decreases.
                String step4_Before = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                if (Config.BrowserType.ToLower() == "mozilla" || Config.BrowserType.ToLower() == "firefox")
                {

                    new Actions(Driver).MoveToElement(Viewport[3], Viewport[3].Size.Width / 4 - 5, Viewport[3].Size.Height / 4 -5).ClickAndHold().
       DragAndDropToOffset(Viewport[3], Viewport[3].Size.Width /2 +5 , Viewport[3].Size.Height / 2 +5).
       Release().Build().Perform();
                    Thread.Sleep(1000);
                    Thread.Sleep(1000);
                }
                else
                {
                    Z3dViewerPage.Performdragdrop(Viewport[3], (Viewport[3].Size.Width / 2) - 50, (Viewport[3].Size.Height / 2) + 100, (Viewport[3].Size.Width / 2) - 50, (Viewport[3].Size.Height / 2) - 50);
                }
                String step4_After = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                if (!step4_Before.Equals(step4_After))
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

        public TestCaseResult Test_163371(String testid, String teststeps, int stepcount)
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
            string CursorScroll = TestData[2];
            string CursorWL = TestData[3];
            string CursorRoam = TestData[4];
            string CursorZoom = TestData[5];
            String Descr2 = TestData[6];
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1  - Navigate to 3D tab and Click MPR mode from the dropdown. Note: This is new design(could change)
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step1 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription, field: "acc", thumbimgoptional: Descr2);
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
                //step:2 - Series is loaded in the 3D viewer in MPR 4:1 viewing mode
                IList<IWebElement> Viewport = Z3dViewerPage.Viewport();
                if (Viewport.Count == 4)
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

                //step:3  -Click the Settings button from the toolbar > 3D settings and move the MPR interactive quality and 3D interactive quality sliders to 100%.
                Boolean step3_1 = Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.MPRInteractiveQuality, 100);
                Boolean step3_2 = Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.InteractiveQuality3D, 100);
                if (step3_1 && step3_2)
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

                //step:4 - Select the scroll tool from the 3D toolbox
                Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool, "Result");
                Thread.Sleep(1500);
                String step4 = Viewport[3].GetCssValue("cursor");
                if (step4.Contains(CursorScroll))
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

                //step:5 - The "Lossy Compression" annotation is displayed during the interaction on the images in MPR result control.
                Boolean step5 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.ResultPanel, 5, 5, 10);
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

                //step:6 - Select the window level tool from the 3D toolbox.
                Z3dViewerPage.select3DTools(Z3DTools.Window_Level, "Result");
                Thread.Sleep(1500);
                String step6 = Viewport[3].GetCssValue("cursor");
                if (step6.Contains(CursorWL))
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

                //step:7 - Apply the window level on MPR result control.
                Boolean step7 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.ResultPanel, 5, 5, 10);
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

                //step:8 - Select the Roam tool from the 3D toolbox
                Z3dViewerPage.select3DTools(Z3DTools.Pan, "Result");
                Thread.Sleep(1500);
                String step8 = Viewport[3].GetCssValue("cursor");
                if (step8.Contains(CursorRoam))
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

                //step:9 - Apply the window level on MPR result control.Click and hold the left mouse button on the image displayed on the Result control and move the mouse
                Boolean step9 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.ResultPanel, 5, 5, 10);
                if (step9)
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

                //step:10 - Select the zoom tool from the 3D toolbox
                Z3dViewerPage.select3DTools(Z3DTools.Interactive_Zoom, "Result");
                Thread.Sleep(1500);
                String step10 = Viewport[3].GetCssValue("cursor");
                if (step10.Contains(CursorZoom))
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

                //step11 - Apply the zoom level on MPR result control.Click and hold the left mouse button on the image displayed on the Result control and move the mouse
                Boolean step11 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.ResultPanel, 5, 5, 10);
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

                //step-12 Click the Settings button> 3D settings from the toolbar and move the MPR final quality and 3D final quality sliders lesser 100%. ( Range = 1% to 99%)
                Boolean step12_1 = Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.MPRFinalQuality, 95);
                Boolean step12_2 = Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.FinalQuality3D, 96);
                if (step12_1 && step12_2)
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

                //step:13 - Repeat steps 4-12
                Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool, "Result");
                Thread.Sleep(1500);
                String step13Cursor = Viewport[3].GetCssValue("cursor");
                Boolean step13_1 = false;
                if (step13Cursor.Contains(CursorScroll))
                    step13_1 = true;
                Boolean step13_2 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.ResultPanel, 5, 5, 10);
                Z3dViewerPage.select3DTools(Z3DTools.Window_Level, "Result");
                PageLoadWait.WaitForFrameLoad(2);
                Boolean step13_3 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.ResultPanel, 5, 5, 10);
                Z3dViewerPage.select3DTools(Z3DTools.Pan, "Result");
                PageLoadWait.WaitForFrameLoad(2);
                String step16cur = Viewport[3].GetCssValue("cursor");
                Boolean step13_4 = false;
                if (step16cur.Contains(CursorRoam))
                    step13_4 = true;
                Boolean step13_5 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.ResultPanel, 5, 5, 10);
                Z3dViewerPage.select3DTools(Z3DTools.Interactive_Zoom, "Result");
                PageLoadWait.WaitForFrameLoad(2);
                String step13zoomcursor = Viewport[3].GetCssValue("cursor");
                Boolean step13_6 = false;
                if (step13zoomcursor.Contains(CursorZoom))
                    step13_6 = true;
                Boolean step13_7 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.ResultPanel, 5, 5, 10);
                if (step13_1 && step13_2 && step13_3 && step13_4 && step13_5 && step13_6 && step13_7)
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

                //step:14  -Click on the close button from the Global toolbar
                //step:15 - Launch the study with No Lossy compressed series in Universal viewer.
                Z3dViewerPage.ExitIcon().Click();
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step15 = Z3dViewerPage.searchandopenstudyin3D(Study2PID, Study2Descr);
                if (step15)
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
                    result.steps[ExecutedSteps].SetLogs();
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step16: Series is loaded in the 3D viewer in MPR 4:1 viewing mode.
                Viewport = Z3dViewerPage.Viewport();
                if (Viewport.Count == 4)
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

                //step:17 - Click the Settings> 3D settings button from the toolbar and move the MPR interactive quality and 3D interactive quality sliders to 100%
                Boolean step21_1 = Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.MPRInteractiveQuality, 100);
                Boolean step21_2 = Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.InteractiveQuality3D, 100);
                if (step21_1 && step21_2)
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

                //step:18 - Select the scroll tool from the 3D toolbox
                Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool, "Result");
                Thread.Sleep(1500);
                Viewport = Z3dViewerPage.Viewport();
                String step22 = Viewport[3].GetCssValue("cursor");
                if (step22.Contains(CursorScroll))
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

                //step:19 - The "Lossy Compression" annotation is displayed during the interaction on the images in MPR result control.
                Boolean step23 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.ResultPanel, 5, 5, 10);
                if (!step23)
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

                //step:20 - Select the window level tool from the 3D toolbox.
                Z3dViewerPage.select3DTools(Z3DTools.Window_Level, "Result");
                Thread.Sleep(1500);
                String step24 = Viewport[3].GetCssValue("cursor");
                if (step24.Contains(CursorWL))
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

                //step:21 - Apply the window level on MPR result control.
                Boolean step25 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.ResultPanel, 5, 5, 10);
                if (!step25)
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

                //step:22 - Select the Roam tool from the 3D toolbox
                Z3dViewerPage.select3DTools(Z3DTools.Pan, "Result");
                Thread.Sleep(1500);
                String step26 = Viewport[3].GetCssValue("cursor");
                if (step26.Contains(CursorRoam))
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

                //step:23 - Apply the window level on MPR result control.Click and hold the left mouse button on the image displayed on the Result control and move the mouse
                Boolean step27 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.ResultPanel, 5, 5, 10);
                if (!step27)
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

                //step:24 - Select the zoom tool from the 3D toolbox
                Z3dViewerPage.select3DTools(Z3DTools.Interactive_Zoom, "Result");
                Thread.Sleep(1500);
                String step28 = Viewport[3].GetCssValue("cursor");
                if (step28.Contains(CursorZoom))
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

                //step25 - Apply the zoom level on MPR result control.Click and hold the left mouse button on the image displayed on the Result control and move the mouse
                Boolean step29 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.ResultPanel, 5, 5, 10);
                if (!step29)
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

                //step:26 - Click the Settings button from the toolbar and move the MPR final quality and 3D final quality sliders lesser 100%. ( Range = 1% to 99%).
                Boolean step30_1 = Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.MPRFinalQuality, 96);
                Boolean step30_2 = Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.FinalQuality3D, 96);
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

                //step:27  -Repeat steps 4-12.
               //scroll
                IWebElement Iresult = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel);
                bool Scrollingtool27 = Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool, "Result");
                int t = 0;
                do
                {
                    BasePage.mouse_event(0x0800, 0, 0, -20, 0);
                    Thread.Sleep(1000);
                    t++;
                    if (t > 100) break;
                }
                while (Z3dViewerPage.checkvalue(Locators.CssSelector.LeftTopPane, 2) >= 66);
                string step27_1 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Iresult);
                //window level 
                bool windowtool27 = Z3dViewerPage.select3DTools(Z3DTools.Window_Level, "Result");
                      new Actions(Driver).SendKeys("x").Release().Build().Perform();
                Z3dViewerPage.Performdragdrop(Iresult, 20, 30);
                Thread.Sleep(10000);
                String step27_2 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Iresult);
                new Actions(Driver).SendKeys("x").Release().Build().Perform();

                //window pan 
                bool pan27 = Z3dViewerPage.select3DTools(Z3DTools.Pan, "Result");
                new Actions(Driver).MoveToElement(Iresult, Iresult.Size.Width - 10,Iresult.Size.Height - 5)
                .ClickAndHold().DragAndDropToOffset(Iresult, 150, 150).Release().Build().Perform();
                Thread.Sleep(1000);
                string step27_3 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Iresult);

                //Interactive Zoom
                bool zoom27 = Z3dViewerPage.select3DTools(Z3DTools.Interactive_Zoom, "Result");
                Z3dViewerPage.DragandDropelement(BluRingZ3DViewerPage.ResultPanel);
                string step27_4 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Iresult);
                if (Scrollingtool27 && step27_1.Contains("Lossy") && windowtool27 && step27_2.Contains("Lossy") && pan27 && step27_3.Contains("Lossy") && zoom27 && step27_4.Contains("Lossy"))
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
                ////below is some time not working 
                //Boolean Result = false;
                //Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool, "Result");
                //Thread.Sleep(1500);
                //String step31_1 = Viewport[3].GetCssValue("cursor");
                //if (step31_1.Contains(CursorScroll))
                //    Result = true;
                //else
                //    Result = false;

                //Boolean step31_2 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.ResultPanel, 5, 5, 10);
                //if (step31_2)
                //    Result = true;
                //else
                //    Result = false;

                //Z3dViewerPage.select3DTools(Z3DTools.Window_Level, "Result");
                //Thread.Sleep(1500);
                //String step31_3 = Viewport[3].GetCssValue("cursor");
                //if (step31_3.Contains(CursorWL))
                //    Result = true;
                //else
                //    Result = false;

                //Boolean step31_4 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.ResultPanel, 5, 5, 10);
                //if (step31_4)
                //    Result = true;
                //else
                //    Result = false;

                //Z3dViewerPage.select3DTools(Z3DTools.Pan, "Result");
                //Thread.Sleep(1500);
                //String step31_5 = Viewport[3].GetCssValue("cursor");
                //if (step31_5.Contains(CursorRoam))
                //    Result = true;
                //else
                //    Result = false;

                //Boolean step31_6 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.ResultPanel, 5, 5, 10);
                //if (step31_6)
                //    Result = true;
                //else
                //    Result = false;

                //Z3dViewerPage.select3DTools(Z3DTools.Interactive_Zoom, "Result");
                //Thread.Sleep(1500);
                //String step31_7 = Viewport[3].GetCssValue("cursor");
                //if (step31_7.Contains(CursorZoom))
                //    Result = true;
                //else
                //    Result = false;

                //Boolean step31_8 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.ResultPanel, 5, 5, 10);
                //if (step31_8)
                //    Result = true;
                //else
                //    Result = false;

                //if (Result)
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

        public TestCaseResult Test_163363(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string ThicknessValue = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Log in to iCA as Administrator. From the Domain management page, Add the Undo/Redo options under the 3D tool box configuration and save.
                //undo and redo tools added in precondition 
                //Step:1 & 2 - study should be loaded without any errors.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step1 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription);
                if (step1)
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

                //step:3  -Set the thickness value to 100.00 mm on the MPR result control.
                Z3dViewerPage.EnterThickness(BluRingZ3DViewerPage.ResultPanel, ThicknessValue);
                PageLoadWait.WaitForFrameLoad(2);
                String step2_1 = Z3dViewerPage.GetThickNessValue(BluRingZ3DViewerPage.Navigationone);
                String step2_2 = Z3dViewerPage.GetThickNessValue(BluRingZ3DViewerPage.Navigationtwo);
                String step2_3 = Z3dViewerPage.GetThickNessValue(BluRingZ3DViewerPage.Navigationthree);
                String step2_4 = Z3dViewerPage.GetThickNessValue(BluRingZ3DViewerPage.ResultPanel);
                Boolean step2_5 = step2_1.Equals(ThicknessValue + " mm");
                Boolean step2_6 = step2_2.Equals(ThicknessValue + " mm");
                Boolean step2_7 = step2_3.Equals(ThicknessValue + " mm");
                Boolean step2_8 = step2_4.Equals(ThicknessValue + " mm");
                if (!step2_5 && !step2_6 && !step2_7 && step2_8)
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

                //step:4 - Click the Tissue Selection Tool button from 3D toolbox
                Z3dViewerPage.select3DTools(Z3DTools.Selection_Tool, BluRingZ3DViewerPage.ResultPanel);
                Boolean step3 = Z3dViewerPage.TissueSelectionDialog().Displayed;
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

                //step:5 - Select "Large Vessels" from the preset radio button on the Tissue Selection Tool dialog
                IWebElement ThreshholdValue = Driver.FindElement(By.CssSelector(Locators.CssSelector.ThreshholdProgressBar));
                IWebElement Radiousvalue = Driver.FindElement(By.CssSelector(Locators.CssSelector.RadiousProgressBar));
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.LargeVessels);
                String LargeVesselThresholdValue = ThreshholdValue.GetAttribute("aria-valuenow");
                String LargeVesselRadiousValue = Radiousvalue.GetAttribute("aria-valuenow");
                if (Convert.ToInt32(LargeVesselThresholdValue) < 25 && Convert.ToInt32(LargeVesselRadiousValue) == 2000)
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

                //step:6 - On the MPR Result control, select "Navigation 1" to be the source control from the drop down list.
                Z3dViewerPage.SelectNavigation(BluRingZ3DViewerPage.Navigationone);
                IList<IWebElement> Viewport = Z3dViewerPage.Viewport();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Viewport[3]))
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

                //step:7 - Click the aorta in the MPR result control.
                IWebElement resultcontrol = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel);
                String aoraLoc = "Loc: 0.0, 34.0, 0.0 mm";
                Z3dViewerPage.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.ResultPanel);
                Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage.ResultPanel, aoraLoc, scrolllevel: 34,  UseTestComplete: true);
                PageLoadWait.WaitForFrameLoad(5);
                Double Step6Volume = Z3dViewerPage.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(5);
                IWebElement ViewerContainer7 = Z3dViewerPage.ViewerContainer();
                if (Config.BrowserType.ToLower() == "chrome")
                {
                    Z3dViewerPage.Performdragdrop(resultcontrol, resultcontrol.Size.Width / 2 + 20, 30, resultcontrol.Size.Width / 2 + 20, 20);
                    Z3dViewerPage.Performdragdrop(resultcontrol, resultcontrol.Size.Width / 2 + 20, 50, resultcontrol.Size.Width / 2 + 20, 40);
                    Z3dViewerPage.Performdragdrop(resultcontrol, resultcontrol.Size.Width / 2 + 20, 70, resultcontrol.Size.Width / 2 + 20, 60);
                    Z3dViewerPage.Performdragdrop(resultcontrol, resultcontrol.Size.Width / 2 + 20, 90, resultcontrol.Size.Width / 2 + 20, 80);
                }
                else
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((ViewerContainer7.Location.X + 700), (ViewerContainer7.Location.Y / 2 + 650));
                    //  new Actions(Driver).DragAndDropToOffset(resultcontrol, resultcontrol.Size.Width / 2 + 5, resultcontrol.Size.Width / 2 + 10);
                    //    Z3dViewerPage.Performdragdrop(resultcontrol, resultcontrol.Size.Width / 2 + 5, 10, resultcontrol.Size.Height /2 , 5);
                    new Actions(Driver).MoveToElement(resultcontrol, (resultcontrol.Size.Width / 2) + 18, (resultcontrol.Size.Height / 4) - 40).ClickAndHold().
                   MoveToElement(resultcontrol, (resultcontrol.Size.Width / 2) + 18, (resultcontrol.Size.Height / 4-30)).Release().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    Thread.Sleep(500);
                    new Actions(Driver).MoveToElement(resultcontrol, (resultcontrol.Size.Width / 2) + 18, (resultcontrol.Size.Height / 4 - 30)).Click().Build().Perform();
                }
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Viewport[3]))
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

                //step:8 - On the tissue selection dialog, note the calculated volume at the bottom
                Double Step7Volume = Z3dViewerPage.GetSelectionVolume();
                if (Step6Volume != Step7Volume)
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

                //step:9 - On the tissue selection tool dialog, click "Delete Selected" button.
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.DeleteSelected);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Viewport[3]))
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

                //step:10 - Click the "Undo Selection" button
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Undo");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Viewport[3]))
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

                //step:11 - Volume in cubic centimeters is displayed at the bottom of the tissue selection tool dialog and is the same as the value noted in step 6
                Double Step9Volume = Z3dViewerPage.GetSelectionVolume();
                if (Step9Volume == Step7Volume)
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

                //step:12 - Click the "Redo Selection" button
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Redo");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Viewport[3]))
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

                //step:13  -Click the "Undo Selection" button.
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Undo");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Viewport[3]))
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

                //step:14 - Click the "Redo Selection" button from the view port top bar
                Z3dViewerPage.SelectOptionsfromViewPort(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.RedoSegmentation);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Viewport[3]))
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

                //step:15 - Click the "Undo Selection" button from the view port top bar
                Z3dViewerPage.SelectOptionsfromViewPort(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.UndoSegmentation);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Viewport[3]))
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

                //step:16  -On the tissue selection dialog note the calculated volume at the bottom
                Double Step13Volume = Z3dViewerPage.GetSelectionVolume();
                if (Step13Volume == Step7Volume)
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

                //step:17 - On the tissue selection tool dialog, click the "Delete Unselected" button.
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.DeleteUnselected);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Viewport[3]))
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

                //step:18  - 	Click the Undo button on the 3D toolbox.
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Undo");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Viewport[3]))
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

                //step:19 - On the tissue selection dialog note the calculated volume at the bottom.
                Double Step16Volume = Z3dViewerPage.GetSelectionVolume();
                if (Step16Volume == Step7Volume)
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

                //step:20 - Click the Redo button on the 3D toolbox.
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Redo");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Viewport[3]))
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

                //step:21 - On the tissue selection tool dialog, click the "Undo Selection" button.
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Undo");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Viewport[3]))
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

                //step:22 - On the tissue selection dialog note the calculated volume at the bottom
                Double Step19Volume = Z3dViewerPage.GetSelectionVolume();
                if (Step19Volume == Step7Volume)
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

                //step:23 - Click the Reset button on the 3D toolbox
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                Thread.Sleep(7500);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Viewport[3]))
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

        public TestCaseResult Test_163370(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID"); 
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string Requirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String[] testdata = Requirements.Split('|');
            
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Stepscrolli
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            try
            {
                //Step:1 -  Launch the study in 3D from ICA
                //step:2 - Navigate to 3D tab and click MPR mode from dropdown
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Z3dViewerPage.Deletefiles(testcasefolder);
                Boolean step1 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription);
                if (step1)
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
                    result.steps[ExecutedSteps].SetLogs();
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                IWebElement Iresultpan = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel);
                //step:3 - Image in the MPR result control is updated with the changes
                IWebElement Iresultpanel = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel);
                if (Config.BrowserType.ToLower()=="firefox" || Config.BrowserType.ToLower()=="mozilla")
                {
                    Iresultpanel = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel);
                    //for tissue selections
                    bool bflagtissue = false;
                    Z3dViewerPage.select3DTools(Z3DTools.Selection_Tool, BluRingZ3DViewerPage.ResultPanel);
                    Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.SmallVessels);
                    double beforevolume = Z3dViewerPage.GetSelectionVolume();
                    Actions ACt3 = new Actions(Driver);
                    ACt3.MoveToElement(Iresultpanel, Iresultpanel.Size.Width / 2, Iresultpanel.Size.Height / 2).Click().Build().Perform();
                    Thread.Sleep(1000);
                    ACt3.MoveToElement(Iresultpanel, Iresultpanel.Size.Width / 2, Iresultpanel.Size.Height / 2 - 10).Release().Build().Perform();
                    Thread.Sleep(1000);
                    new Actions(Driver).MoveToElement(Iresultpan, Iresultpan.Size.Width / 2, Iresultpanel.Size.Height / 2).ClickAndHold().
                    MoveToElement(Iresultpanel, Iresultpanel.Size.Width / 2, Iresultpanel.Size.Height / 2 - 10).Release().Build().Perform();
                    Thread.Sleep(1000);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    double aftervolume = Z3dViewerPage.GetSelectionVolume();
                    try
                    {
                    IWebElement CloseSelectedToolBox = Driver.FindElement(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox));
                    Thread.Sleep(1000);
                    Actions act = new Actions(Driver);
                    act.MoveToElement(CloseSelectedToolBox).Click().Build().Perform();
                    CloseSelectedToolBox.Click();
                    }
                    catch (Exception e) { }
                    if (beforevolume < aftervolume) bflagtissue = true;
                    //for scroll image 
                    Boolean bfalg_scroll = false;
                    Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool);
                   
                    IList<IWebElement> Viewport7 = Z3dViewerPage.Viewport();
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport7[3].Location.X / 2 + 600), (Viewport7[3].Location.Y / 2 + 400));
                    for (int i = 0; i < 10; i++)
                    {
                    BasePage.mouse_event(0x0800, 0, 0, 8, 0);
                    Thread.Sleep(1000);
                    }
                    List<string> check_scroll = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                    if (check_scroll[1] != check_scroll[3]) bfalg_scroll = true;
                    //   Z3dViewerPage.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.ResultPanel);

                    //for zoom
                    bool bflag_zoom = false;
                    Z3dViewerPage.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.ResultPanel);
                    Z3dViewerPage.DragandDropelement(BluRingZ3DViewerPage.ResultPanel);
                    Thread.Sleep(10000);
                    List<string> check_zoom = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                    if (check_zoom[1] != check_zoom[3]) bflag_zoom = true;

                    //for window level 
                    bool bflag_window = false;
                    Z3dViewerPage.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.ResultPanel);
                    new Actions(Driver).SendKeys("x").Release().Build().Perform();
                
                    Z3dViewerPage.Performdragdrop(Iresultpanel, 20, 30);
                    Thread.Sleep(10000);

                    List<string> check_window = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                    if (check_window[1] != check_window[3]) bflag_window = true;

                    //for roam 
                    bool bflag_roam = false;
                    List<string> check_beforepan = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                    Z3dViewerPage.select3DTools(Z3DTools.Pan, BluRingZ3DViewerPage.ResultPanel);
                    Z3dViewerPage.SelectControl(BluRingZ3DViewerPage.ResultPanel);
                    new Actions(Driver).MoveToElement(Iresultpanel, Iresultpanel.Size.Width - 10, Iresultpanel.Size.Height - 5)
                    .DragAndDropToOffset(Iresultpanel, 150, 150).Release().Build().Perform();
                    Thread.Sleep(7000);
                    new Actions(Driver).SendKeys("X").Build().Perform(); Thread.Sleep(1000);
                    List<string> check_afterpan = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                    if (check_beforepan[3] != check_afterpan[3]) bflag_roam = true;

                    //for line measurement
                    string beforeline = "step3line_before.bmp";
                    DownloadImageFile(Iresultpanel, testcasefolder + Path.DirectorySeparatorChar + beforeline);
                    Thread.Sleep(10000);
                    Z3dViewerPage.select3DTools(Z3DTools.Line_Measurement, BluRingZ3DViewerPage.ResultPanel);
                    Z3dViewerPage.SelectControl(BluRingZ3DViewerPage.ResultPanel);
                    new Actions(Driver).MoveToElement(Iresultpanel, (Iresultpanel.Size.Width / 2) - 50, (Iresultpanel.Size.Height / 2) - 50)
                    .ClickAndHold()
                    .MoveToElement(Iresultpanel, (Iresultpanel.Size.Width / 2) - 50, (Iresultpanel.Size.Height / 2) - 100)
                    .Release().Build().Perform();
                    Thread.Sleep(5000);
                    string Afterline = "step3line_after.bmp";
                    DownloadImageFile(Iresultpanel, testcasefolder + Path.DirectorySeparatorChar + Afterline);
                    Thread.Sleep(10000);
                    bool bflagline = CompareImage(testcasefolder + beforeline, testcasefolder + Afterline);

                 
                    Z3dViewerPage.select3DTools(Z3DTools.Reset);
                    if (bfalg_scroll && bflag_zoom && bflag_window && bflag_roam && bflagtissue && bflagline == false)
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
                //for zoom 
                else
                {
                    Boolean step3_1 = Z3dViewerPage.ApplyToolsonViewPort(BluRingZ3DViewerPage.ResultPanel, Z3DTools.Scrolling_Tool, 50, 30, 80);
                    Boolean step3_2 = Z3dViewerPage.ApplyToolsonViewPort(BluRingZ3DViewerPage.ResultPanel, Z3DTools.Interactive_Zoom, 50, 50, 100);
                    Boolean step3_3 = Z3dViewerPage.ApplyToolsonViewPort(BluRingZ3DViewerPage.ResultPanel, Z3DTools.Window_Level, 50, 50, 200, movement: "positive");
                    Z3dViewerPage.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.ResultPanel);
                    new Actions(Driver).SendKeys("x").Release().Build().Perform();
                    Boolean step3_4 = Z3dViewerPage.ApplyToolsonViewPort(BluRingZ3DViewerPage.ResultPanel, Z3DTools.Line_Measurement, 50, 50, 100, testid, ExecutedSteps + 1);
                    Boolean step3_5 = Z3dViewerPage.ApplyToolsonViewPort(BluRingZ3DViewerPage.ResultPanel, Z3DTools.Pan, 60, 60, 100);
                    string filename3_navg3_before = "step3_nav3before.jpg";
                    string filename3_navg3_after = "step3_nav1after.jpg";
                    Z3dViewerPage.select3DTools(Z3DTools.Selection_Tool, BluRingZ3DViewerPage.ResultPanel);
                    DownloadImageFile(Iresultpan, testcasefolder + Path.DirectorySeparatorChar + filename3_navg3_before);
                    Thread.Sleep(2000);
                    try
                    {
                        Thread.Sleep(5000);
                        IWebElement CloseSelectedToolBox = Driver.FindElement(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox));
                        Thread.Sleep(1000);
                        Actions act = new Actions(Driver);
                        act.MoveToElement(CloseSelectedToolBox).Click().Build().Perform();
                        Thread.Sleep(2000);
                        CloseSelectedToolBox.Click();
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.ErrorLog(e.Message);
                    }
                    if (Config.BrowserType.ToLower() != "chrome")
                    {
                        this.Cursor = new Cursor(Cursor.Current.Handle);
                        Cursor.Position = new Point((Iresultpan.Location.X + 250), (Iresultpan.Location.Y / 2 + 500));
                    }
                    Actions ACt3 = new Actions(Driver);
                    ACt3.MoveToElement(Iresultpan, Iresultpan.Size.Width / 2, Iresultpan.Size.Height / 2).Click().Build().Perform();
                    Thread.Sleep(1000);
                    ACt3.MoveToElement(Iresultpan, Iresultpan.Size.Width / 2, Iresultpan.Size.Height / 2 - 10).Release().Build().Perform();
                    Thread.Sleep(1000);
                    new Actions(Driver).MoveToElement(Iresultpan, Iresultpan.Size.Width / 2, Iresultpan.Size.Height / 2).ClickAndHold().
                       MoveToElement(Iresultpan, Iresultpan.Size.Width / 2, Iresultpan.Size.Height / 2 - 10).Release().Build().Perform();
                    Thread.Sleep(1000);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    DownloadImageFile(Iresultpan, testcasefolder + Path.DirectorySeparatorChar + filename3_navg3_after);
                    bool step3_6 = CompareImage(testcasefolder + filename3_navg3_before, testcasefolder + filename3_navg3_after);
                    if (step3_1 && step3_2 && step3_3 && step3_4 && step3_5 && step3_6 == false)
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
                //step:4  -Render mode is applied applied only to the MPR Result control
                Iresultpanel = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel);
                IWebElement ViewerContainer = Z3dViewerPage.ViewerContainer();
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Mip);
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.MinIp);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Average);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                Boolean step4_2 = CompareImage(result.steps[ExecutedSteps], Iresultpanel, pixelTolerance: 100);
                Logger.Instance.InfoLog("ICA-17184 Faded MIP");
                if (step4_2 )
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

                //step:5 - Select the Window level tool option from the 3D toolbox
                Z3dViewerPage.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.ResultPanel);
                IList<IWebElement> Viewport = Z3dViewerPage.Viewport();
                String step5 = Viewport[3].GetCssValue("cursor");
                if (step5.Contains(BluRingZ3DViewerPage.WindowLevelCursor))
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

                //step:6 - Window/level presets are applied only to the MPR Result control
                String resultpanvalbefore = Z3dViewerPage.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.ResultPanel);
                Thread.Sleep(2000);
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Brain, "Preset");
                Thread.Sleep(5000);
                String resultpanvalafter = Z3dViewerPage.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.ResultPanel);
                if(resultpanvalafter != resultpanvalbefore)
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

                //step:7  -Save cursor shows up while hovering over the images displayed on the controls
                Z3dViewerPage.select3DTools(Z3DTools.Download_Image, BluRingZ3DViewerPage.ResultPanel);
                String step7 = Viewport[3].GetCssValue("cursor");
                if (step7.Contains(BluRingZ3DViewerPage.DownloadCursor))
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

                //step:8 - Save the Image to the local drive
                Z3dViewerPage.Performdragdrop(Viewport[3], (Viewport[3].Size.Width / 2), (Viewport[3].Size.Height / 4));
                String imagename = testid + "_" + ExecutedSteps + "_" + new Random().Next(101);
                String imgLocation = Config.downloadpath + "\\" + imagename + ".jpg";
                Z3dViewerPage.downloadImageForViewport(imagename);              
                if (File.Exists(imgLocation))
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

                //step:9  -Repeat steps 3-8 for the MPR result control in Six up viewing mode
                Boolean step9 = false;
                bool step9_1 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_6, "y");
                Viewport = Z3dViewerPage.Viewport();
                if (step9_1 && Viewport.Count == 6)
                    step9 = true;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);

                bool step9_3 = false;bool step9_4 = false;
                new Actions(Driver).SendKeys("x").Release().Build().Perform();Thread.Sleep(1000);
                IList<string> before_result9 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool,BluRingZ3DViewerPage.ResultPanel);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport[4].Location.X + 150), (Viewport[4].Location.Y / 2 + 500));
                for (int i = 0; i < 15; i++)
                {
                    BasePage.mouse_event(0x0800, 0, 0, 8, 0);
                    Thread.Sleep(1000);
                }
                
                IList<string> after_result9 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if(before_result9[4] != after_result9[4] )
                {
                    step9_3 = true;
                }
                
                List<string> before_zoom9 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                Z3dViewerPage.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.ResultPanel);
                Z3dViewerPage.DragandDropelement(BluRingZ3DViewerPage.ResultPanel);
                List<string> after_zoom9 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                Thread.Sleep(10000);
                if (before_zoom9[4] != after_zoom9[4] )
                {
                    step9_4 = true;
                }

                IWebElement Iresultpanle6 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel);
                
                Z3dViewerPage.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.ResultPanel);
                Z3dViewerPage.Performdragdrop(Iresultpanle6, 20, 30);
                bool step9_5 = false;
                Thread.Sleep(10000);
                List<string> window9_3 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                if (window9_3[0] == window9_3[1] && window9_3[1] == window9_3[2] && window9_3[3] == window9_3[5] && window9_3[3] != window9_3[4])
                {
                    step9_5 = true;
                }
                new Actions(Driver).SendKeys("x").Release().Build().Perform();
                Z3dViewerPage.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.ResultPanel);
                Boolean step9_6 = Z3dViewerPage.ApplyToolsonViewPort(BluRingZ3DViewerPage.ResultPanel, Z3DTools.Line_Measurement, 50, 50, 100, testid, ExecutedSteps + 1);
                Z3dViewerPage.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.ResultPanel);
                Boolean step9_7 = Z3dViewerPage.ApplyToolsonViewPort(BluRingZ3DViewerPage.ResultPanel, Z3DTools.Pan, 60, 60, 100);
                Z3dViewerPage.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.ResultPanel);
                
                string filename9_navg3_before = "step9_nav3before.bmp";
                string filename9_navg3_after = "step9_nav1after.bmp";
                Z3dViewerPage.select3DTools(Z3DTools.Selection_Tool, BluRingZ3DViewerPage.ResultPanel);
                try
                {
                    Thread.Sleep(5000);
                    if (IsElementPresent(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox)))
                    {
                        IWebElement CloseSelectedToolBox = Driver.FindElement(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox));
                        Actions act = new Actions(Driver);
                        act.MoveToElement(CloseSelectedToolBox).Click().Build().Perform();
                        Thread.Sleep(1000);
                        CloseSelectedToolBox.Click();
                    }
                }
                catch (Exception e) { }
                DownloadImageFile(Iresultpanle6, testcasefolder + Path.DirectorySeparatorChar + filename9_navg3_before);
                new Actions(Driver).SendKeys("X").Build().Perform();Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(Iresultpanle6, Iresultpanle6.Size.Width / 2, Iresultpanle6.Size.Height / 2).ClickAndHold().
                MoveToElement(Iresultpanle6, Iresultpanle6.Size.Width / 2 - 5, Iresultpanle6.Size.Height / 3 - 5).Release().Build().Perform();
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(Iresultpanle6, Iresultpanle6.Size.Width / 2, Iresultpanle6.Size.Height / 2).ClickAndHold().
                MoveToElement(Iresultpanle6, Iresultpanle6.Size.Width / 2 - 5, Iresultpanle6.Size.Height / 3 - 5).Release().Build().Perform();
                Thread.Sleep(1000);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                Iresultpan = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel);
                new Actions(Driver).SendKeys("X").Build().Perform(); Thread.Sleep(1000);
                //  bool step3_6 = false;
                DownloadImageFile(Iresultpan, testcasefolder + Path.DirectorySeparatorChar + filename9_navg3_after);
                bool step9_8 = CompareImage(testcasefolder + filename9_navg3_before, testcasefolder + filename9_navg3_after);
                Z3dViewerPage.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.ResultPanel);
                if (step9_3 && step9_4 && step9_5 && step9_6 && step9_7 && step9_8==false)
                    step9 = true;
                ViewerContainer = Z3dViewerPage.ViewerContainer();
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Mip);
                List<string> result9_mip = Z3dViewerPage.GetControlvalues(BluRingZ3DViewerPage.RenderType, BluRingZ3DViewerPage.Mip);
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.MinIp);
                List<string> result9_minip = Z3dViewerPage.GetControlvalues(BluRingZ3DViewerPage.RenderType, BluRingZ3DViewerPage.MinIp);
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Average);
                List<string> result9_average = Z3dViewerPage.GetControlvalues(BluRingZ3DViewerPage.RenderType, BluRingZ3DViewerPage.Average);
                bool bflag9_10 = false;
                if (result9_mip[4]== BluRingZ3DViewerPage.Mip && result9_minip[4]== BluRingZ3DViewerPage.MinIp && result9_average[4]== BluRingZ3DViewerPage.Average)
                    bflag9_10 = true;

                Z3dViewerPage.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.ResultPanel);
                Viewport = Z3dViewerPage.Viewport();
                String step9cursor = Viewport[3].GetCssValue("cursor");
                bool bflag9_11 = false;
                if (step9cursor.Contains(BluRingZ3DViewerPage.WindowLevelCursor))
                    bflag9_11 = true;

                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Brain, "Preset");
                String BrainWL = testdata[21];
                List<string> BrainWLstep9_1 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Liver, "Preset");
                String LiverWL = testdata[22];
                List<string> LiverWLstep9_2 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.PFossa, "Preset");
                String PFossaWL = testdata[23];
                List<string> PFossaWLstep9_3 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                Boolean step14_1 = BrainWLstep9_1[4].Replace(" ", "").Equals(BrainWL);
                Boolean step14_2 = LiverWLstep9_2[4].Replace(" ", "").Equals(LiverWL);
                Boolean step14_3 = PFossaWLstep9_3[4].Replace(" ", "").Equals(PFossaWL);
                Boolean step14_4 = BrainWLstep9_1[2].Replace(" ", "").Equals(BrainWL);
                Boolean step14_5 = LiverWLstep9_2[2].Replace(" ", "").Equals(LiverWL);
                Boolean step14_6 = PFossaWLstep9_3[2].Replace(" ", "").Equals(PFossaWL);
                Boolean step14_7 = BrainWLstep9_1[1].Replace(" ", "").Equals(BrainWL);
                Boolean step14_8 = LiverWLstep9_2[1].Replace(" ", "").Equals(LiverWL);
                Boolean step14_9 = PFossaWLstep9_3[1].Replace(" ", "").Equals(PFossaWL);
                Boolean step14_10 = BrainWLstep9_1[0].Replace(" ", "").Equals(BrainWL);
                Boolean step14_11 = LiverWLstep9_2[0].Replace(" ", "").Equals(LiverWL);
                Boolean step14_12 = PFossaWLstep9_3[0].Replace(" ", "").Equals(PFossaWL);
                if (step14_1 && step14_2 && step14_3 && !step14_4 && !step14_5 && !step14_6 && !step14_7 && !step14_8 && !step14_9 && !step14_10 && !step14_11 && !step14_12)
                    step9 = true;

                Z3dViewerPage.select3DTools(Z3DTools.Download_Image, BluRingZ3DViewerPage.ResultPanel);
                String step15dwnld = Viewport[4].GetCssValue("cursor");
                if (step15dwnld.Contains(BluRingZ3DViewerPage.DownloadCursor))
                    step9 = true;
                Z3dViewerPage.Performdragdrop(Viewport[4], (Viewport[4].Size.Width / 2), (Viewport[4].Size.Height / 4));
                imagename = testid + "_" + ExecutedSteps + "_" + new Random().Next(101);
                imgLocation = Config.downloadpath + "\\" + imagename + ".jpg";
                Z3dViewerPage.downloadImageForViewport(imagename);
                bool bfalg9_12 = false;
                if (File.Exists(imgLocation))
                    bfalg9_12 = true;

                if (step9  && bflag9_10 && bflag9_11 && bfalg9_12)
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

                //step:10 - Launch the study in Z3D from ICA
                bool step10launch = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.MPR, "y");
                Viewport = Z3dViewerPage.Viewport();
                if (step10launch && Viewport.Count == 4)
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

                //step:11 - Select a render mode 3D Slab from MPR Result control
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Slab3D);
                List<string> result11_slab= Z3dViewerPage.GetControlvalues(BluRingZ3DViewerPage.RenderType, BluRingZ3DViewerPage.Slab3D);
                if(result11_slab[3]==BluRingZ3DViewerPage.Slab3D)
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

                //step:12 - Drop down list with the following modality specific transfer functions presets is displayed:
                    Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, "none", "Preset");
                    PageLoadWait.WaitForFrameLoad(3);
                    IList<IWebElement> RenderModes = Z3dViewerPage.RenderModes();
                    Boolean step12_1 = RenderModes[0].Text.Contains(testdata[0]);
                    Boolean step12_2 = RenderModes[1].Text.Contains(testdata[1]);
                    Boolean step12_3 = RenderModes[2].Text.Contains(testdata[2]);
                    Boolean step12_4 = RenderModes[3].Text.Contains(testdata[3]);
                    Boolean step12_5 = RenderModes[4].Text.Contains(testdata[4]);
                    Boolean step12_6 = RenderModes[5].Text.Contains(testdata[5]);
                    Boolean step12_7 = RenderModes[6].Text.Contains(testdata[6]);
                    Boolean step12_8 = RenderModes[7].Text.Contains(testdata[7]);
                    Boolean step12_9 = RenderModes[8].Text.Contains(testdata[8]);
                    Boolean step12_10 = RenderModes[9].Text.Contains(testdata[9]);
                    Boolean step12_11 = RenderModes[10].Text.Contains(testdata[10]);
                    Boolean step12_12 = RenderModes[11].Text.Contains(testdata[11]);
                    Boolean step12_13 = RenderModes[12].Text.Contains(testdata[12]);
                    Boolean step12_14 = RenderModes[13].Text.Contains(testdata[13]);
                    Boolean step12_15 = RenderModes[14].Text.Contains(testdata[14]);
                    Boolean step12_16 = RenderModes[15].Text.Contains(testdata[15]);
                    Boolean step12_17 = RenderModes[16].Text.Contains(testdata[16]);
                    Boolean step12_18 = RenderModes[17].Text.Contains(testdata[17]);
                //3D MIP A|3D MIP B| are removed  jira for below this ICA-18715
                //Boolean step12_19 = RenderModes[18].Text.Contains(testdata[18]);
                //Boolean step12_20 = RenderModes[19].Text.Contains(testdata[19]);
                Boolean step12_21 = RenderModes[18].Text.Contains(testdata[18]);
                    Boolean step12_22 = RenderModes[19].Text.Contains(testdata[19]);
                    Boolean step12_23 = RenderModes[20].Text.Contains(testdata[20]);
                    if (step12_1 && step12_2 && step12_3 && step12_4 && step12_5 && step12_6 && step12_7 && step12_8 && step12_9 && step12_10 && step12_11 && step12_12 && step12_13 && step12_14 && step12_15 && step12_16 && step12_17 && step12_18  && step12_21 && step12_22 && step12_23)
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
                Thread.Sleep(3000);
                //IWebElement ViewPort = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel);
                //IWebElement closeoptions = ViewPort.FindElement(By.CssSelector(Locators.CssSelector.menubutton));
                //ClickElement(closeoptions);
                //wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.menutable)));
                IWebElement Menuclose = Driver.FindElement(By.CssSelector(Locators.CssSelector.MenuClose));
                ClickElement(Menuclose);
                PageLoadWait.WaitForFrameLoad(3);
                Thread.Sleep(10000);

                //step:13 - Select any of the modality specific transfer function preset on the drop down list to apply to the volume displayed on the control
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, testdata[12], "Preset");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Viewport[3], pixelTolerance: 100))
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

                //step:14  -Save cursor shows up while hovering over the images displayed on the controls
                Z3dViewerPage.select3DTools(Z3DTools.Download_Image, BluRingZ3DViewerPage.ResultPanel);
                String step14 = Viewport[3].GetCssValue("cursor");
                if (step14.Contains(BluRingZ3DViewerPage.DownloadCursor))
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

                //step:15 - Save the Image to the local drive
                Z3dViewerPage.Performdragdrop(Viewport[3], (Viewport[3].Size.Width / 2), (Viewport[3].Size.Height / 4));
                imagename = testid + "_" + ExecutedSteps + "_" + new Random().Next(101);
                if(Config.BrowserType.ToLower()=="chrome")
                imgLocation = Config.downloadpath + "\\" + imagename + ".jpg";
                Z3dViewerPage.downloadImageForViewport(imagename);
                if (File.Exists(imgLocation))
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

                //step:16 - Click on the user settings, Select the 3D settings and move the MPR and 3D Final quality sliders between 1% to 99%. Click on the save button.
                Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.MPRFinalQuality, 95);
                Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.FinalQuality3D, 95);
                PageLoadWait.WaitForPageLoad(10);
                IWebElement NavigationElement = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel);
                Boolean step16 = false;
                if (Config.BrowserType.ToLower() == "firefox" || Config.BrowserType.ToLower() == "mozilla")
                {
                    IList<string> ilossy = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.CenterBottomAnnotationValue, null, null, 0);
                    if (ilossy[0] == "Lossy Compressed" && ilossy[1] == "Lossy Compressed" && ilossy[2] == "Lossy Compressed" && ilossy[3] == "Lossy Compressed")
                        step16 = true;
                }
                else
                {
                     step16 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(NavigationElement).Equals("Lossy Compressed");
                }
                if(step16)
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

                //step:17 - Select the download button from the 3D tool box. Click on the image from MPR Result control and verify the image type options in the save window
                if(Config.BrowserType.Contains("internet"))
                    Z3dViewerPage.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.ResultPanel);
                Thread.Sleep(3000);
                IWebElement resultPanel = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel);
                Boolean step17 =  Z3dViewerPage.CheckEnabledButtonInDownloadBox(BluRingZ3DViewerPage.ResultPanel);
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

                //step:18 - avigate to Sixup viewing mode. Set the Render mode to any one of the navigation control under the result control.
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                Z3dViewerPage.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.ResultPanel);
                Boolean step18 = Z3dViewerPage.CheckEnabledButtonInDownloadBox(BluRingZ3DViewerPage.ResultPanel);
                if (step18)
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

        public TestCaseResult Test_163368(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            BluRingViewer viewer = new BluRingViewer();


            WpfObjects wpfobject = new WpfObjects();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            String sPatientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String Testdata_req = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            string[] split_testdata = Testdata_req.Split('|');
            string sHeadingPatientID = split_testdata[0];
            string sHeadingAccession = split_testdata[1];
            string sAccessionValue = split_testdata[2];
            string sThickness2 = split_testdata[3];
            string sresetvalue = split_testdata[4];
            //  string sThickness12 = split_testdata[4];

            String thumbnailcaption = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            try
            {
                login.LoginIConnect(username, password);

                //Step 1   From the Universal viewer   Select a 3D supported series and Select the MPR option from the drop down Study     HFS  ANONYMOUS
                z3dvp.Deletefiles(testcasefolder);
                login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                login.SearchStudy(sHeadingPatientID, sPatientid);
                PageLoadWait.WaitForLoadingMessage(30);
                login.SelectStudy(sHeadingAccession, sAccessionValue);
                PageLoadWait.WaitForFrameLoad(5);
                //Step 1 From iCA, Load the the study " TEST, FFS" in 3D viewer. 1.Navigate to 3D tab and Click MPR mode from the dropdown.Note: This is new design(could change)
                var viewer1 = BluRingViewer.LaunchBluRingViewer(fieldname: sHeadingAccession, value: sAccessionValue);
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                bool res = z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR, "n");
                Thread.Sleep(2000);
                bool bflag = false;
                if (res == true)
                {
                    List<string> sAnnotaion = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 1);
                    if (sAnnotaion[0] == sAccessionValue && sAnnotaion[1] == sAccessionValue && sAnnotaion[2] == sAccessionValue && sAnnotaion[3] == sAccessionValue)
                    {
                        bflag = true;
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                }
                if (bflag == false)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 2  Increase the thickness of the MPR navigation control 1 to 100.0 mm on the bottom left corner of the control. 
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                Thread.Sleep(1000);
                IWebElement ithumnail = z3dvp.IthumNail();
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                IList<IWebElement> Viewport2 = z3dvp.Viewport();
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
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

                //step 3 Select 'Navigation 1' from the render mode drop down list in the MPR result control so that it is synchronized with the MPR navigation control 1. Increase the thickness of the MPR result control to 100.0 mm on the bottom left corner of the control. 
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.ResultPanel);
                z3dvp.EnterThickness(BluRingZ3DViewerPage.ResultPanel, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
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

                //Step 4 Note the orientation marker displayed at the top center of the MPR result control are synchronized with the orientation marker in the MPR navigation control 1. Select the rotate tool from the Z3D toolbar. Click the top center of the MPR navigation control 1 and drag down slowly. Note the orientation marker of the MPR result control as you drag down. 
                bool btool4 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                Actions act4_a = new Actions(Driver);
                act4_a.SendKeys("X").Build().Perform();
                List<string> before_result4 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                Actions act4 = new Actions(Driver);
                act4.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2).ClickAndHold()
                .MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2 + 100)
                .Release()
                .Build()
                .Perform();
                Thread.Sleep(10000);
                //For firefox purpose 
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                List<string> result4 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                if (result4[0] == result4[3] && before_result4[0] != result4[0] && before_result4[0] != result4[3])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(500);

                //Step 5  Click the reset button.  
                bool btool5 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
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

                //step 6 Increase the thickness of the MPR navigation control 1 to 100.0 mm on the bottom left corner of the control. 
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
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

                //Step 7 Select 'Navigation 1' from the render mode drop down list in the MPR result control so that it is synchronized with the MPR navigation control 1. Increase the thickness of the MPR result control to 100.0 mm on the bottom left corner of the control. 
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.ResultPanel);
                z3dvp.EnterThickness(BluRingZ3DViewerPage.ResultPanel, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
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

                //Step 8 Note the orientation marker displayed at the left side of the MPR result control is synchronized with the orientation marker in the MPR navigation control 1. Select the rotate tool from the Z3D toolbar. Click on the center of the MPR navigation control 1 and drag to the right slowly. Note the left orientation marker of the MPR result control as you drag to the right. 
                bool btool8 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                Actions act8_a = new Actions(Driver);
                act8_a.SendKeys("X").Build().Perform();
                //Before actions 
                List<string> Beforeresult8 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                Actions act8 = new Actions(Driver);
                IWebElement iNavigationone = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                act8.MoveToElement(iNavigationone, iNavigationone.Size.Width / 2, iNavigationone.Size.Height / 2).ClickAndHold()
                .MoveToElement(iNavigationone, iNavigationone.Size.Width / 2 + 100, iNavigationone.Size.Height / 2)
                .Release()
                .Build()
                .Perform();
                Thread.Sleep(10000);
                //For firefox purpose 
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                //After Actions 
                List<string> result8 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                if (result8[0] == result8[3] && Beforeresult8[1] != result8[1] && Beforeresult8[3] != result8[3])
                {

                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(500);

                //step 9 Click the reset button. 
                bool btool9 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
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

                //Step 10 Increase the thickness of the MPR navigation control 1 to 100.0 mm on the bottom left corner of the control. 
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
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

                //Step 11 Select 'Navigation 1' from the render mode drop down list in the MPR result control so that it is synchronized with the MPR navigation control 1. Increase the thickness of the MPR result control to 100.0 mm on the bottom left corner of the control.
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.ResultPanel);
                z3dvp.EnterThickness(BluRingZ3DViewerPage.ResultPanel, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Thread.Sleep(1000);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
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

                //Step 12 Note the orientation marker displayed at the right side of the MPR result control is synchronized with the orientation marker in the MPR navigation control 1. Select the rotate tool from the Z3D toolbar. Click on the center of the MPR navigation control 1 and drag to the left slowly. Note the right orientation of the MPR result control marker as you drag to the left.
                bool btool12 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                //Before Actions 
                List<string> Beaoferresult12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                Actions act12_a = new Actions(Driver);
                act12_a.SendKeys("X").Build().Perform();
                Actions act12 = new Actions(Driver);
                act12.MoveToElement(iNavigationone, iNavigationone.Size.Width / 2, iNavigationone.Size.Height / 2).ClickAndHold()
                .MoveToElement(iNavigationone, iNavigationone.Size.Width / 2 - 100, iNavigationone.Size.Height / 2)
                .Release()
                .Build()
                .Perform();
                Thread.Sleep(10000);
                //For firefox purpose 
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                //Afer Actions 
                List<string> result12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                if (result12[2] == result12[3] && Beaoferresult12[2] != result12[2] && Beaoferresult12[3] != result12[3])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(500);

                //Step 13 Click the reset button. 
                bool btool13 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
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

                //Step 14  From iCA, Load a study " HFS, ANONYMOUS" in Z3D viewer.  1.Navigate to 3D tab and Click 3D 6:1 mode from the dropdown.Note: This is new design(could change)
                bool res14 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6, "n");
                bool bflag14 = false;
                if (res14 == true)
                {
                    List<string> sAnnotaion = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 1);
                    if (sAnnotaion[0] == sAccessionValue && sAnnotaion[1] == sAccessionValue && sAnnotaion[2] == sAccessionValue && sAnnotaion[3] == sAccessionValue && sAnnotaion[4] == sAccessionValue && sAnnotaion[5] == sAccessionValue)
                    {
                        bflag14 = true;
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                }
                Thread.Sleep(1000);
                //step 15 Increase the thickness of the MPR navigation control 1 to 100.0 mm on the bottom left corner of the control. 
                bool bflag15 = false;
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
                {
                    bflag15 = true;
                }
                Thread.Sleep(1000);
                //Step 16  Select 'Navigation 1' from the render mode drop down list in the MPR result control so that it is synchronized with the MPR navigation control 1. Increase the thickness of the MPR result control to 100.0 mm on the bottom left corner of the control. 
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.ResultPanel);
                z3dvp.EnterThickness(BluRingZ3DViewerPage.ResultPanel, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                bool bflag16 = false;
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
                {
                    bflag16 = true;
                }
                Thread.Sleep(1000);

                //Step 17 Note the orientation marker displayed at the top center of the MPR result control are synchronized with the orientation marker in the MPR navigation control 1. Select the rotate tool from the 3D toolbox. Click the top center of the MPR navigation control 1 and drag up slowly. Note the orientation marker of the MPR result control as you drag up. 
                bool btool17 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                bool bflag17 = false;
                Actions act17_a = new Actions(Driver);
                act17_a.SendKeys("X").Build().Perform();
                //Before action 
                List<string> Beforeresult17 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                Actions act17 = new Actions(Driver);
                act17.MoveToElement(iNavigationone, iNavigationone.Size.Width / 2, iNavigationone.Size.Height / 2).ClickAndHold()
                .MoveToElement(iNavigationone, iNavigationone.Size.Width / 2, iNavigationone.Size.Height / 2 + 100)
                .Release()
                .Build()
                .Perform();
                Thread.Sleep(10000);
                //For firefox purpose 
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                List<string> result17 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                if (result17[0] == result17[4] && Beforeresult17[0] != result17[0] && Beforeresult17[4] != result17[4])
                {
                    bflag17 = true;

                }
                Thread.Sleep(1000);
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(500);
                //Step 18 Click the reset button. 

                bool btool18 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                bool bflag18 = false;
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
                {
                    bflag18 = true;
                }
                Thread.Sleep(1000);
                //Step 19 Increase the thickness of the MPR navigation control 1 to 100.0 mm on the bottom left corner of the control. 

                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                bool bflag19 = false;

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
                {
                    bflag19 = true;
                }
                Thread.Sleep(1000);

                //Step 20 Select 'Navigation 1' from the render mode drop down list in the MPR result control so that it is synchronized with the MPR navigation control 2. Increase the thickness of the MPR result control to 100.0 mm on the bottom left corner of the control. 
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.ResultPanel);
                z3dvp.EnterThickness(BluRingZ3DViewerPage.ResultPanel, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                bool bflag20 = false;
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 5);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
                {
                    bflag20 = true;
                }
                Thread.Sleep(1000);

                //step 21 Note the orientation marker displayed at the left side of the MPR result control is synchronized with the orientation marker in the MPR navigation control 2. Select the rotate tool from the 3D toolbox. Click on the center of the MPR navigation control 2 and drag to the right slowly. Note the left orientation marker of the MPR result control as you drag to the right. 
                bool btool20 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                bool bflag21 = false;
                List<string> beoforeresult20 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                Actions act21_a = new Actions(Driver);
                act21_a.SendKeys("X").Build().Perform();
                Actions act20 = new Actions(Driver);
                act20.MoveToElement(iNavigationone, iNavigationone.Size.Width / 2, iNavigationone.Size.Height / 2).ClickAndHold()
                .MoveToElement(iNavigationone, iNavigationone.Size.Width / 2 + 100, iNavigationone.Size.Height / 2)
                .Release()
                .Build()
                .Perform();
                Thread.Sleep(10000);
                //For firefox purpose 
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                List<string> result20 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                if (result20[0] == result20[4] && beoforeresult20[0] != result20[0] && beoforeresult20[0] != result20[4])
                {
                    bflag21 = true;

                }
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);

                //Step 22 Click the reset button. 
                bool btool22 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                bool bflag22 = false;
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 6);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
                {
                    bflag22 = true;
                }
                Thread.Sleep(1000);

                //Step 23 Increase the thickness of the MPR navigation control 3 to 100.0 mm on the bottom left corner of the control.

                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                bool bflag23 = false;
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 7);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
                {
                    bflag23 = true;
                }
                Thread.Sleep(1000);

                //Step 24 Select 'Navigation 1' from the render mode drop down list in the MPR result control so that it is synchronized with the MPR navigation control 3. Increase the thickness of the MPR result control to 100.0 mm on the bottom left corner of the control.
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.ResultPanel);
                z3dvp.EnterThickness(BluRingZ3DViewerPage.ResultPanel, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                bool bflag24 = false;
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 8);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
                {
                    bflag24 = true;
                }
                Thread.Sleep(1000);

                //Step 25   Note the orientation marker displayed at the right side of the MPR result control is synchronized with the orientation marker in the MPR navigation control 3. Select the rotate tool from the 3D toolbox. Click on the center of the MPR navigation control 3 and drag to the left slowly. Note the right orientation of the MPR result control marker as you drag to the left.  
                bool btool25 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                bool bflag25 = false;
                List<string> beforeresult25 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                Actions act25_a = new Actions(Driver);
                act25_a.SendKeys("X").Build().Perform();
                Actions act25 = new Actions(Driver);
                act25.MoveToElement(iNavigationone, iNavigationone.Size.Width / 2, iNavigationone.Size.Height / 2).ClickAndHold()
                .MoveToElement(iNavigationone, iNavigationone.Size.Width / 2 - 100, iNavigationone.Size.Height / 2)
                .Release()
                .Build()
                .Perform();
                Thread.Sleep(10000);
                //For firefox purpose 
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                List<string> result25 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                if (result25[0] == result25[4] && beforeresult25[3] != result25[3] && beforeresult25[4] != result25[4])
                {
                    bflag25 = true;

                }

                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);

                //Step 26 Click the reset button. 
                bool btool26 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                bool bflag26 = false;
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 9);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
                {
                    bflag26 = true;
                }
                Thread.Sleep(1000);
                if (bflag14 && bflag15 && bflag16 && bflag17 && bflag18 && bflag19 && bflag20 && bflag21 && bflag22 && bflag23 && bflag24 && bflag25 && bflag26)
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

        public TestCaseResult Test_163364(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            BluRingViewer viewer = new BluRingViewer();


            WpfObjects wpfobject = new WpfObjects();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            String sPatientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String Testdata_req = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            string[] split_testdata = Testdata_req.Split('|');
            string sHeadingPatientID = split_testdata[0];
            string sHeadingAccession = split_testdata[1];
            string sAccessionValue = split_testdata[2];
            string sThickness2 = split_testdata[3];
            string sresetvalue = split_testdata[4];


            String thumbnailcaption = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            try
            {
                login.LoginIConnect(username, password);
                z3dvp.Deletefiles(testcasefolder);
                login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                login.SearchStudy(sHeadingPatientID, sPatientid);
                PageLoadWait.WaitForLoadingMessage(30);
                login.SelectStudy(sHeadingAccession, sAccessionValue);
                PageLoadWait.WaitForFrameLoad(5);
                //Step 1   From the Universal viewer , Select a 3D supported series and Select the MPR option from the drop down.
                var viewer1 = BluRingViewer.LaunchBluRingViewer(fieldname: sHeadingAccession, value: sAccessionValue);
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                bool res = z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR, "n");
                Thread.Sleep(1000);
                bool bflag = false;
                if (res == true)
                {
                    List<string> sAnnotaion = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 1);
                    if (sAnnotaion[0] == sAccessionValue && sAnnotaion[1] == sAccessionValue && sAnnotaion[2] == sAccessionValue && sAnnotaion[3] == sAccessionValue)
                    {
                        bflag = true;
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                }
                if (bflag == false)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                IList<IWebElement> Viewport2 = z3dvp.Viewport();
                //Step 2 Increase the thickness of the MPR navigation control 1 to 100.0 mm on the bottom left corner of the control. 
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                IWebElement ithumnail = z3dvp.IthumNail();
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
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

                //Step 3 Select 'Navigation 1' as the source from the drop down list in the MPR result control so that it is synchronized with the MPR navigation control 1. Increase the thickness of the MPR result control to 100.0 mm on the bottom left corner of the control. 
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.ResultPanel);
                z3dvp.EnterThickness(BluRingZ3DViewerPage.ResultPanel, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                Thread.Sleep(1000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
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

                //Step 4 Note the orientation marker displayed at the top center of the MPR result control are synchronized with the orientation marker in the MPR navigation control 1. Select the rotate tool (Image center) from the Z3D toolbar. Click the top center of the MPR navigation control 1 and drag down slowly. Note the orientation marker of the MPR result control as you drag down. 
                bool btool4 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                Actions act4_a = new Actions(Driver);
                act4_a.SendKeys("X").Build().Perform();
                List<string> before_result4 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                Actions act4 = new Actions(Driver);
                act4.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2).ClickAndHold()
                .MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2 + 100)
                .Release()
                .Build()
                .Perform();
                Thread.Sleep(10000);
                //For firefox purpose 
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                List<string> result4 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                if (result4[0] == result4[3] && before_result4[0] != result4[0] && before_result4[0] != result4[3])
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
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(500);

                //Step 5 Click the reset button. 
                bool btool5 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
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

                //Step 6 Increase the thickness of the MPR navigation control 1 to 100.0 mm on the bottom left corner of the control.
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
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

                //Step 7 Select 'Navigation 1' as the source from the drop down list in the MPR result control so that it is synchronized with the MPR navigation control 1. Increase the thickness of the MPR result control to 100.0 mm on the bottom left corner of the control. 
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.ResultPanel);
                z3dvp.EnterThickness(BluRingZ3DViewerPage.ResultPanel, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
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

                //Step 8 Note the orientation marker displayed at the left side of the MPR result control is synchronized with the orientation marker in the MPR navigation control 1. Select the rotate tool (Image center) from the Z3D toolbar. Click on the center of the MPR navigation control 1 and drag to the right slowly. Note the left orientation marker of the MPR result control as you drag to the right. 
                bool btool8 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);

                //Before actions 
                List<string> Beforeresult8 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                Actions act8_a = new Actions(Driver);
                act8_a.SendKeys("X").Build().Perform();
                Actions act8 = new Actions(Driver);
                act8.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2).ClickAndHold()
                .MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2 + 100, z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2)
                .Release()
                .Build()
                .Perform();
                Thread.Sleep(10000);
                //For firefox purpose 
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                //After Actions 
                List<string> result8 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                if (result8[0] == result8[3] && Beforeresult8[1] != result8[1] && Beforeresult8[3] != result8[3])
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
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(500);
                //Step 9 Click the reset button 
                bool btool9 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                // z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, false);
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(1000);
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
                // z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, true);
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(500);
                //Step 10 Increase the thickness of the MPR navigation control 1 to 100.0 mm on the bottom left corner of the control. 
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
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

                //Step 11 Select 'Navigation 1' as the source from the drop down list in the MPR result control so that it is synchronized with the MPR navigation control 1. Increase the thickness of the MPR result control to 100.0 mm on the bottom left corner of the control. 
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.ResultPanel);
                z3dvp.EnterThickness(BluRingZ3DViewerPage.ResultPanel, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
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

                //step 12 Note the orientation marker displayed at the right side of the MPR result control is synchronized with the orientation marker in the MPR navigation control 1. Select the rotate tool (Image center) from the Z3D toolbar. Click on the center of the MPR navigation control 1 and drag to the left slowly. Note the right orientation of the MPR result control marker as you drag to the left. 
                bool btool12 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                Actions act12_a = new Actions(Driver);
                act12_a.SendKeys("X").Build().Perform();
                //Before Actions 
                List<string> Beaoferresult12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                Actions act12 = new Actions(Driver);
                act12.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2).ClickAndHold()
                .MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2 - 100, z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2)
                .Release()
                .Build()
                .Perform();
                Thread.Sleep(10000);
                //For firefox purpose 
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                //Afer Actions 
                List<string> result12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                if (result12[0] == result12[3] && Beaoferresult12[2] != result12[2] && Beaoferresult12[3] != result12[3])
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
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(500);
                //STep 13  Click the reset button.  
                bool btool13 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
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

                //Step 14 From iCA, Load the study " FFP, ANONYMOUS" in 3D viewer.1.Navigate to 3D tab and Click 3D 6:1 mode from the dropdown.Note: This is new design(could change)
                bool res14 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6, "n");
                Thread.Sleep(5000);
                bool bflag14 = false;
                if (res14 == true)
                {
                    List<string> sAnnotaion = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 1);
                    if (sAnnotaion[0] == sAccessionValue && sAnnotaion[1] == sAccessionValue && sAnnotaion[2] == sAccessionValue && sAnnotaion[3] == sAccessionValue && sAnnotaion[4] == sAccessionValue && sAnnotaion[5] == sAccessionValue)
                    {
                        bflag14 = true;

                    }
                }
                Thread.Sleep(1000);
                //Step 15 Increase the thickness of the MPR navigation control 1 to 100.0 mm on the bottom left corner of the control.
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                bool bflag15 = false;
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
                {
                    bflag15 = true;
                }
                Thread.Sleep(1000);
                //Step 16 Select 'Navigation 1' as the source from the drop down list in the MPR result control so that it is synchronized with the MPR navigation control 1. Increase the thickness of the MPR result control to 100.0 mm on the bottom left corner of the control. 
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.ResultPanel);
                z3dvp.EnterThickness(BluRingZ3DViewerPage.ResultPanel, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool bflag16 = false;
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
                {
                    bflag16 = true;
                }
                Thread.Sleep(1000);
                //Step 17  Note the orientation marker displayed at the top center of the MPR result control are synchronized with the orientation marker in the MPR navigation control 1. Select the rotate tool (Image center) from the 3D toolbox. Click the top center of the MPR navigation control 1 and drag up slowly. Note the orientation marker of the MPR result control as you drag up. 
                bool btool17 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                bool bflag17 = false;
                //Before action 
                List<string> Beforeresult17 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                Actions act17_a = new Actions(Driver);
                act17_a.SendKeys("X").Build().Perform();
                Actions act17 = new Actions(Driver);
                act17.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2).ClickAndHold()
                .MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2 + 100)
                .Release()
                .Build()
                .Perform();
                Thread.Sleep(10000);
                //For firefox purpose 
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                List<string> result17 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                if (result17[0] == result17[4] && Beforeresult17[0] != result17[0] && Beforeresult17[4] != result17[4])
                {
                    bflag17 = true;
                }
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);

                //Step 18 Click the reset button. 
                bool btool18 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                bool bflag18 = false;
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
                {
                    bflag18 = true;
                }
                Thread.Sleep(1000);
                //Step 19 Increase the thickness of the MPR navigation control 2 to 100.0 mm on the bottom left corner of the control.
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                bool bflag19 = false;
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
                {
                    bflag19 = true;
                }
                Thread.Sleep(1000);
                //Step 20 Select 'Navigation 1' as the source from the drop down list in the MPR result control so that it is synchronized with the MPR navigation control 2. Increase the thickness of the MPR result control to 100.0 mm on the bottom left corner of the control. 
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.ResultPanel);
                bool bflag20 = false;
                z3dvp.EnterThickness(BluRingZ3DViewerPage.ResultPanel, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 5);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
                {
                    bflag20 = true;
                }
                Thread.Sleep(1000);
                //Step 21 Note the orientation marker displayed at the left side of the MPR result control is synchronized with the orientation marker in the MPR navigation control 2. Select the rotate tool (Image center) from the 3D toolbox. Click on the center of the MPR navigation control 2 and drag to the right slowly. Note the left orientation marker of the MPR result control as you drag to the right. 
                bool btool20 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                Actions act21_a = new Actions(Driver);
                bool bflag21 = false;
                act21_a.SendKeys("X").Build().Perform();
                List<string> beoforeresult20 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                Actions act20 = new Actions(Driver);
                act20.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2).ClickAndHold()
                .MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2 + 100, z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2)
                .Release()
                .Build()
                .Perform();
                Thread.Sleep(10000);
                //For firefox purpose 
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                List<string> result20 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                if (result20[0] == result20[4] && beoforeresult20[0] != result20[0] && beoforeresult20[0] != result20[4])
                {
                    bflag21 = true;
                }
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);

                //STEp 22 Click the reset button. 
                bool btool22 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                bool bflag22 = false;
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 6);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
                {
                    bflag22 = true;
                }
                Thread.Sleep(1000);
                //Step 23  Increase the thickness of the MPR navigation control 3 to 100.0 mm on the bottom left corner of the control.  
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                bool bflag23 = false;
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 7);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
                {
                    bflag23 = true;
                }
                Thread.Sleep(1000);
                //Step 24 Select 'Navigation 1' as the source from the drop down list in the MPR result control so that it is synchronized with the MPR navigation control 3. Increase the thickness of the MPR result control to 100.0 mm on the bottom left corner of the control.
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.ResultPanel);
                z3dvp.EnterThickness(BluRingZ3DViewerPage.ResultPanel, sThickness2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                bool bflag24 = false;
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 8);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
                {
                    bflag24 = true;
                }
                Thread.Sleep(1000);
                //Step 25  Note the orientation marker displayed at the right side of the MPR result control is synchronized with the orientation marker in the MPR navigation control 3. Select the rotate tool (Image center) from the 3D toolbox. Click on the center of the MPR navigation control 3 and drag to the left slowly. Note the right orientation of the MPR result control marker as you drag to the left.  
                bool btool25 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                bool bflag25 = false;
                Actions act25_a = new Actions(Driver);
                act25_a.SendKeys("X").Build().Perform();
                List<string> beforeresult25 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                Actions act25 = new Actions(Driver);
                act25.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2).ClickAndHold()
                .MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2 - 100, z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2)
                .Release()
                .Build()
                .Perform();
                Thread.Sleep(10000);
                //For firefox purpose 
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                List<string> result25 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                if (result25[0] == result25[4] && beforeresult25[0] != result25[4] && beforeresult25[0] != result25[4])
                {
                    bflag25 = true;
                }
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);

                //Step 26 Click the reset button. 
                bool btool26 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                bool bflag26 = false;
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 9);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
                {
                    bflag26 = true;
                }
                Thread.Sleep(1000);
                if (bflag14 && bflag15 && bflag16 && bflag17 && bflag18 && bflag19 && bflag20 && bflag21 && bflag22 && bflag23 && bflag24 && bflag25 && bflag26)
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


        public TestCaseResult Test_163361(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String[] TestData = TestDataRequirements.Split('|');
            string PatientID2 = TestData[0];
            string ThumbnailDescription2 = TestData[1];
            string abdomen = TestData[2];
            string bone = TestData[3];
            string bonebody = TestData[4];
            string brain = TestData[5];
            string bronchial = TestData[6];
            string liver = TestData[7];
            string lung = TestData[8];
            string mediastinum = TestData[9];
            string pfossa = TestData[10];
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1 - Navigate to 3D tab and Click MPR mode from the dropdown. Note: This is new design(could change)
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step1 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription);
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

                //step:2 - Click on the Options at the top right corner of the navigation control 1/ navigation control 2/ navigation control 3 .
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, "none", "Preset");
                PageLoadWait.WaitForFrameLoad(3);
                IList<IWebElement> RenderModes = Z3dViewerPage.RenderModes();
                Boolean step2_1 = RenderModes[0].Text.Contains(BluRingZ3DViewerPage.Abdomen);
                Boolean step2_2 = RenderModes[1].Text.Contains(BluRingZ3DViewerPage.Bone);
                Boolean step2_3 = RenderModes[2].Text.Contains(BluRingZ3DViewerPage.BoneBody);
                Boolean step2_4 = RenderModes[3].Text.Contains(BluRingZ3DViewerPage.Brain);
                Boolean step2_5 = RenderModes[4].Text.Contains(BluRingZ3DViewerPage.Bronchial);
                Boolean step2_6 = RenderModes[5].Text.Contains(BluRingZ3DViewerPage.Liver);
                Boolean step2_7 = RenderModes[6].Text.Contains(BluRingZ3DViewerPage.Lung);
                Boolean step2_8 = RenderModes[7].Text.Contains(BluRingZ3DViewerPage.Mediastinum);
                Boolean step2_9 = RenderModes[8].Text.Contains(BluRingZ3DViewerPage.PFossa);
                if (step2_1 && step2_2 && step2_3 && step2_4 && step2_5 && step2_6 && step2_7 && step2_8 && step2_9)
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
                IWebElement Menuclose = Driver.FindElement(By.CssSelector(Locators.CssSelector.MenuClose));
                ClickElement(Menuclose);
                PageLoadWait.WaitForFrameLoad(3);

                //step:3 - Select each preset one by one from the list and verify
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Abdomen, "Preset");
                Thread.Sleep(3000);
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Abdomen, "Preset");
                List<string> step5_1 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Bone, "Preset");
                List<string> step5_2 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.BoneBody, "Preset");
                List<string> step5_3 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Brain, "Preset");
                List<string> step5_4 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Bronchial, "Preset");
                List<string> step5_5 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Liver, "Preset");
                List<string> step5_6 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Lung, "Preset");
                List<string> step5_7 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Mediastinum, "Preset");
                List<string> step5_8 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.PFossa, "Preset");
                List<string> step5_9 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                if (step5_1[3].Replace(" ", "").Equals(abdomen) && step5_2[3].Replace(" ", "").Equals(bone) && step5_3[3].Replace(" ", "").Equals(bonebody) && step5_4[3].Replace(" ", "").Equals(brain) && step5_5[3].Replace(" ", "").Equals(bronchial) && step5_6[3].Replace(" ", "").Equals(liver) && step5_7[3].Replace(" ", "").Equals(lung) && step5_8[3].Replace(" ", "").Equals(mediastinum) && step5_9[3].Replace(" ", "").Equals(pfossa))
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

                //step:4 - Universal viewer should be closed and study search list page should be displayed.
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

                //step:5 - Search and view a MR or PT study that has 3D supported series
                ExecutedSteps++;//Combined 5th and 6th step
                //step:6 - From the Universal viewer , Select a 3D supported CT series and Select the MPR option from the drop down
                Boolean step6 = Z3dViewerPage.searchandopenstudyin3D(PatientID2, ThumbnailDescription2);
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

                //step:7 - Presets are not available for MR and PT studies
                Boolean step7_1 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Abdomen, "Preset");
                Boolean step7_2 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Bone, "Preset");
                Boolean step7_3 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.BoneBody, "Preset");
                Boolean step7_4 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Brain, "Preset");
                Boolean step7_5 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Bronchial, "Preset");
                Boolean step7_6 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Liver, "Preset");
                Boolean step7_7 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Lung, "Preset");
                Boolean step7_8 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Mediastinum, "Preset");
                Boolean step7_9 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.PFossa, "Preset");
                if (!step7_1 && !step7_2 && !step7_3 && !step7_4 && !step7_5 && !step7_6 && !step7_7 && !step7_8 && !step7_9)
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

                //step8 - Click on the close button fromthe Global toolbar
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
                login.Logout();
            }
        }

        

    }
}

