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
    class MPRView : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public DomainManagement domain { get; set; }
        public StudyViewer studyviewer { get; set; }
        public Cursor Cursor { get; private set; }

        public MPRView(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            domain = new DomainManagement();

            studyviewer = new StudyViewer();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }
        public TestCaseResult Test_163372(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            WpfObjects wpfobject = new WpfObjects();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            String Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String thumbnailcaption = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            //try
            //{
            //    login.LoginIConnect(username, password);
            //    //Step 1   From iCA, Load a study in Z3D viewer. By default MPR viewing mode should dipslay
            //    bool res = z3dvp.searchandopenstudyin3D(Patientid, thumbnailcaption);
            //    if (res)
            //    {
            //        result.steps[++ExecutedSteps].status = "Pass";
            //        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
            //    }
            //    else
            //    {
            //        throw new Exception("Study Not Loaded");
            //        result.steps[++ExecutedSteps].status = "Fail";
            //        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
            //        result.steps[ExecutedSteps].SetLogs();
            //    }
            //    //step 2 All the Navigation 1,navigation 2,navigation 3, result  should display
            //    List<string> result2 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 0);
            //    if (result2[0] == BluRingZ3DViewerPage.Navigationone && result2[1] == BluRingZ3DViewerPage.Navigationtwo && result2[2] == BluRingZ3DViewerPage.Navigationthree && result2[3] == BluRingZ3DViewerPage.ResultPanel)
            //    {
            //        result.steps[++ExecutedSteps].status = "Pass";
            //        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
            //    }
            //    else
            //    {
            //        result.steps[++ExecutedSteps].status = "Fail";
            //        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
            //        result.steps[ExecutedSteps].SetLogs();
            //    }
            //    result.FinalResult(ExecutedSteps);
            //    Logger.Instance.ErrorLog("Overall Test status--" + result.status);
            //    return result;
            //}
            //catch (Exception e)

            //{
            //    //Log Exception
            //    Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
            //    //Report Result
            //    result.FinalResult(e, ExecutedSteps);
            //    Logger.Instance.ErrorLog("Overall Test status--" + result.status);
            //    //Logout
            //    login.Logout();
            //    //Return Result
            //    return result;
            //}
            //finally
            //{
            //    z3dvp.CloseViewer();
            //    login.Logout();
            //    // Driver.Close();

            //}

            result.steps[++ExecutedSteps].status = "Fail";
            result.steps[++ExecutedSteps].status = "Pass";
            result.FinalResult(ExecutedSteps);
            return result;
        }

        public TestCaseResult Test_163373(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            //  ICAZ3DViewerPage z3dvp = new ICAZ3DViewerPage();
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            WpfObjects wpfobject = new WpfObjects();
            BasePage basepage = new BasePage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            String Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String thumbnailcaption = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string testdatades = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            string[] ssplit = testdatades.Split('|');
            String sThickness = ssplit[0];
            string sabdomen = ssplit[1];
            String sPresetvalues = ssplit[2];
            String sIncrementValue = ssplit[3];
            String sIncrementValue16 = ssplit[4];
            string slocationvalue = ssplit[5];
            String username = Config.adminUserName;
            String password = Config.adminPassword;

            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;

            try
            {
                login.LoginIConnect(username, password);
                //Step 1   From iCA, Load a study in Z3D viewer. By default MPR viewing mode should dipslay
                z3dvp.Deletefiles(testcasefolder);
                bool res = z3dvp.searchandopenstudyin3D(Patientid, thumbnailcaption);

                if (res == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    throw new Exception("Study Not Loaded");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 2 Apply the zoom tool to navigation control 1.
                
                IWebElement ithumnail = Driver.FindElement(By.CssSelector(Locators.CssSelector.thumbclick));
                Thread.Sleep(1000);
                z3dvp.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.Navigationone);
                z3dvp.DragandDropelement(BluRingZ3DViewerPage.Navigationone);

                Thread.Sleep(10000);
                List<string> result2 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result2[0] == result2[1] && result2[1] == result2[2])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //step 3 Press the reset Button for initial stage
                z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
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

                //step 4Push the "print screen" key to capture the screen and paste into MS Paint.
                string filename = "PrintBt_153388_" + new Random().Next(1000) + ".jpg";
                z3dvp.CaptureScreen(filename, testid);
                Thread.Sleep(10000);
                if (File.Exists(testcasefolder + filename))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 5 Rotate the cross hairs of navigation control 2 by 180 degrees clockwise.
                IList<IWebElement> Viewport = z3dvp.Viewport();
                IList<string> checkvalue5 = new List<string>();
                if (Config.BrowserType.ToLower() == "firefox" || Config.BrowserType.ToLower() == "mozilla" )
                {
                    z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationtwo);
                    int startx = 0; int starty = 0; int endx = 0; int endy = 0;
                    startx = (Viewport[0].Size.Width / 2) - 116;
                    starty = Viewport[0].Size.Height / 2;
                    endx = (Viewport[0].Size.Width / 2) + 116;
                    endy = (Viewport[0].Size.Height / 2);
                    z3dvp.Performdragdrop(Viewport[1], endx, endy, startx, starty);
                    Thread.Sleep(7500);
                    checkvalue5 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                    if (checkvalue5[0] == "F" && checkvalue5[2] == "P" && checkvalue5[1] == "H" && checkvalue5[3] == "F")
                    {
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
                else
                {
                    IWebElement iNavigationtwo = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                    bool lflag = z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationtwo);
                    
                    if (lflag)
                    {
                        try
                        {
                            Accord.Point p1 = z3dvp.GetIntersectionPoints(iNavigationtwo, testid, ExecutedSteps, "red", "Horizontal", 0);
                            new Actions(Driver).MoveToElement(iNavigationtwo, (iNavigationtwo.Size.Width / 2), (iNavigationtwo.Size.Height / 2)).Build().Perform();
                            Thread.Sleep(2000);
                            Accord.Point p2 = z3dvp.GetIntersectionPoints(iNavigationtwo, testid, 5, "red", "Horizontal", 1);
                            Thread.Sleep(2000);
                            Actions act5 = new Actions(Driver);

                            Thread.Sleep(5000);
                            act5.MoveToElement(iNavigationtwo, (int)p1.X, (int)p1.Y).ClickAndHold().
                                MoveToElement(iNavigationtwo, (int)p2.X, (int)p2.Y).Release().Build().Perform();
                            Thread.Sleep(20000);

                            checkvalue5 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                            if (checkvalue5[0] == "F" && checkvalue5[2] == "P" && checkvalue5[1] == "H" && checkvalue5[3] == "F")
                            {
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
                        catch (Exception e)
                        {
                            result.steps[++ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }

                    }
                }
                //step 6 Apply the roam tool to navigation control 3.
                z3dvp.select3DTools(Z3DTools.Pan, BluRingZ3DViewerPage.Navigationthree);
                new Actions(Driver).SendKeys("X").Build().Perform();Thread.Sleep(1000);
                IWebElement navigationthree6=z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationthree);
                Actions act6 = new Actions(Driver);
                act6.MoveToElement(navigationthree6, navigationthree6.Size.Width - 10,
                navigationthree6.Size.Height - 5)
                .ClickAndHold().
                DragAndDropToOffset(navigationthree6, 150, 150)
                .Release().Build().Perform();
                Thread.Sleep(7000);
                new Actions(Driver).SendKeys("X").Build().Perform(); Thread.Sleep(1000);
                List<string> result6 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                if (result6[0] == checkvalue5[0] && result6[1] == checkvalue5[1] && result6[2] == checkvalue5[2] && result6[3] == checkvalue5[3])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 7Apply the scroll tool to navigation control 2 and select the Navigation 2 in zoom result control.
                z3dvp.select3DTools(Z3DTools.Scrolling_Tool, BluRingZ3DViewerPage.Navigationtwo);

                //for navigation one 
                string filename7_navg1 = "step7_nav1before.bmp";
                bool Nav_one = z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationone);
                DownloadImageFile(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testcasefolder + Path.DirectorySeparatorChar + filename7_navg1);
                Thread.Sleep(10000);

                //for navigation three 
                string filename7_navg3 = "step7_nav3before.bmp";
                bool Nav_three = z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationthree);
                DownloadImageFile(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree), testcasefolder + Path.DirectorySeparatorChar + filename7_navg3);
                Thread.Sleep(10000);

                bool scontorl7 = z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.ResultPanel);
                IWebElement ViewerContainer7 = z3dvp.ViewerContainer();
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((ViewerContainer7.Location.X + 600), (ViewerContainer7.Location.Y / 2 + 300));
                for (int i = 0; i < 15; i++)
                {
                    BasePage.mouse_event(0x0800, 0, 0, 25, 0);
                    Thread.Sleep(1000);
                }

                //Expected REsult 
                IList<string> result7 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                //for navigation one 
                string filename7_navg1_after = "step7_nav1after.bmp";
              //  bool Nav_one_ater = z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).SendKeys("x").Release().Build().Perform();
                DownloadImageFile(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testcasefolder + Path.DirectorySeparatorChar + filename7_navg1_after);
                Thread.Sleep(10000);

                //for navigation three 
                string filename7_navg3_after = "step7_nav3after.bmp";
                bool Nav_three_ater = z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationthree);
                DownloadImageFile(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree), testcasefolder + Path.DirectorySeparatorChar + filename7_navg3_after);
                Thread.Sleep(10000);

                bool b7nav1 = CompareImage(testcasefolder + filename7_navg1, testcasefolder + filename7_navg1_after);
                bool b7nav3 = CompareImage(testcasefolder + filename7_navg3, testcasefolder + filename7_navg3_after);
                if (result7[0] == result7[2] && b7nav1 == false && b7nav3 == false && result7[1] == result7[3])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                new Actions(Driver).SendKeys("x").Release().Build().Perform();
                //step 8 Apply the window/level tool to navigation control 2.
                z3dvp.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).SendKeys("x").Release().Build().Perform();
            //    z3dvp.DragandDropelement(BluRingZ3DViewerPage.Navigationtwo);
                IWebElement Inavigationtwo = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                z3dvp.Performdragdrop(Inavigationtwo, 20, 30);
                Thread.Sleep(10000);
                List<string> result8 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                if (result8[0] == result8[1] && result8[1] == result8[2])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                new Actions(Driver).SendKeys("x").Release().Build().Perform();
                //step 9  Adjust the thickness to be 10.0 mm on navigation control 2.
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationtwo, sThickness);
                //Expected REsult 
                string sExpect_result = sThickness + " mm";
            //    List<string> result10 = z3dvp.GetAttributes_Result(Locators.CssSelector.sThickness, null, null);
                List<string> result10 = z3dvp.GetControlvalues(BluRingZ3DViewerPage.Thickness, "10 mm");
                if (result10[0] == sExpect_result && result10[1] == sExpect_result && result10[2] == sExpect_result)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //step 10 Apply a window/level preset to the navigation control 3.
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationthree, sabdomen, BluRingZ3DViewerPage.Preset);
                //Expected REsult 
                List<string> result11 = z3dvp.GetControlvalues(BluRingZ3DViewerPage.Preset, sabdomen);
                if (result11[0] == result11[1] && result11[1] == result11[2] && result11[3] != result11[0])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //step 11 Press the Reset Button
                z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationthree);
                Thread.Sleep(5000);
                List<string> result12 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result12[0] == result12[1] && result12[1] == result12[2])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 12 Using the traverse tool, move the cross hair of navigation control 1 upward until the position annotation of navigation control 3 reads "0, 0, 100".
                Actions act13 = new Actions(Driver);
                string[] arrsplit13 = null;
               // new Actions(Driver).SendKeys("x").Release().Build().Perform();
                Thread.Sleep(1000);
                bool Nav13_one = z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationone);
            //    new Actions(Driver).SendKeys("x").Release().Build().Perform();
                Thread.Sleep(1000);
                act13.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2)
                .ClickAndHold().MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2 - 144).Release().Build().Perform();
                Thread.Sleep(3000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                List<string> result13 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                arrsplit13 = result13[2].Split(',');
                bool bflag12 = false;
                if (Config.BrowserType.ToLower()=="mozilla" || Config.BrowserType.ToLower()=="firefox")
                     bflag12 = arrsplit13[2].Trim().IndexOf("107") >= 0;
                else
                     bflag12 = arrsplit13[2].Trim().IndexOf("102") >= 0;
                     
                if (result13[0] == result13[1] && result13[1] == result13[3] && bflag12)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 13 take screen shots
                string filename14_screenshot = "Step14.png";
                 z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, false);
            //    new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(500);
                bool Nav14_one = z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationthree);
                DownloadImageFile(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree), testcasefolder + Path.DirectorySeparatorChar + filename14_screenshot);
                if (File.Exists(testcasefolder + filename14_screenshot))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                 z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, true);
              //  new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(500);
                //step 14 Using the traverse tool, move the cross hair of navigation control 1 back to its original position "0, 0, 0".
                Thread.Sleep(10000);
                Actions act15 = new Actions(Driver);
                string filename15_navg1_after = "step15.png";
             //   z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationone);

                //DownloadImageFile(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testcasefolder + Path.DirectorySeparatorChar + filename15_navg1_after);
                act15.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2 - 144)
                .ClickAndHold().MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2).Release().Build().Perform();
                Thread.Sleep(3000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                List<string> result15 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result15[0].Replace("-","") == result15[1].Replace("-", "") && result15[1].Replace("-", "").Replace("-", "") == result15[2].Replace("-", "") && result15[0].Replace("-", "") == slocationvalue)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 15 Apply the scroll upwareds
                Actions action17 = new Actions(Driver);
                IWebElement Navigationname17 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                bool Nav17_one = z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationthree);
                IWebElement ViewerContainer1 = z3dvp.ViewerContainer();
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((ViewerContainer1.Location.X + 300), (ViewerContainer1.Location.Y / 2 + 600));
                int t = 0;
                do
                {
                    BasePage.mouse_event(0x0800, 0, 0, 50, 0);
                    Thread.Sleep(1000);
                    t++;
                    if (t > 200) break;
                }
                while (z3dvp.checkvalue(Locators.CssSelector.LeftTopPane, 2) <= 100);

                IList<string> leftpanel17 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                double value_result = 0;
                if (leftpanel17.Count > 0)
                {

                    string[] splitvalue17 = leftpanel17[2].Trim().Split(',');
                    value_result = Convert.ToDouble(splitvalue17[2].Substring(0, 5).Trim());
                }
                  z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, false);
                //new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(500);

                double split13 = double.Parse(arrsplit13[2].Trim().Substring(0, 3));
                if (split13 == 107) split13 = 102;//firefox this  is updated 
                string sfilename17 = "step17.png";
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationthree);
                DownloadImageFile(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree), testcasefolder + Path.DirectorySeparatorChar + sfilename17);
                bool b17result = CompareImage(testcasefolder + filename14_screenshot, testcasefolder + sfilename17, 500);
                if (value_result <= 102 && split13 == value_result && b17result == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //   new Actions(Driver).SendKeys("T").Build().Perform();
                z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, true);
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
                //Logout
                // login.Logout();
                //Return Result
                return result;
            }
            finally
            {
                // z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, true);
                z3dvp.CloseViewer();
                login.Logout();
                // Driver.Close();
            }
        }



        public TestCaseResult Test_163374(String testid, String teststeps, int stepcount)
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
            string stestdata = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            string[] split_data = stestdata.Split('|');
            string spresetvalue16 = split_data[0];
            string sPresetvalue17 = split_data[1];
            string sPresetResult16 = split_data[2];
            string sThickness = split_data[3];
            string sThickness15 = split_data[4];
            string sLocationvalue = split_data[5];
            string sIncrementValue = split_data[6];
            string sIncrementValue13 = split_data[7];
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            try
            {
                login.LoginIConnect(username, password);
                z3dvp.Deletefiles(testcasefolder);
                //Step 1   From iCA, Load a study in Z3D viewer. By default MPR viewing mode should dipslay
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

                //step 2 Select the navigation control 2 in the MPR result control from the top right corner options drop down.
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.ResultPanel);
                List<string> Iresult2 = z3dvp.GetControlvalues(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationtwo);
                if (Iresult2[3] == BluRingZ3DViewerPage.Navigationtwo)
                {
                    IList<IWebElement> Viewport7 = z3dvp.Viewport();
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport7[3].Location.X / 2 + 400), (Viewport7[3].Location.Y / 2 + 250));
                    //    z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, false);
                    new Actions(Driver).SendKeys("T").Build().Perform();
                    Thread.Sleep(500);
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
                    //  z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, true);
                    new Actions(Driver).SendKeys("T").Build().Perform();
                    Thread.Sleep(500);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 3 Apply the zoom tool to navigation control 2.
                bool Btool3 = z3dvp.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.Navigationtwo);
                IList<IWebElement> Viewport3 = z3dvp.Viewport();
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport3[3].Location.X / 2 + 400), (Viewport3[3].Location.Y / 2 - 100));
                z3dvp.DragandDropelement(BluRingZ3DViewerPage.Navigationtwo);
                List<string> result3 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                List<string> result3_b = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                if (result3[1] == result3[3] && result3_b[1] == result3_b[3])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 4 Press the reset Button 
                bool lfag4 = false;
                bool Btool4 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationtwo);
                {
                    List<string> result4 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                    if (result4[1] == result4[3] && result4[3] == sLocationvalue)
                    {
                        lfag4 = true;
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }

                }
                if (lfag4 == false)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 5 Apply the cross the hair to 180  
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.ResultPanel);
                IWebElement Inavigationone5 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(Inavigationone5, (Inavigationone5.Size.Width / 2), (Inavigationone5.Size.Height / 2)).Build().Perform();
                Thread.Sleep(1000);
                Accord.Point p1 = z3dvp.GetIntersectionPoints(Inavigationone5, testid, ExecutedSteps, "red", "Horizontal", 0);
                new Actions(Driver).MoveToElement(Inavigationone5, (Inavigationone5.Size.Width / 2), (Inavigationone5.Size.Height / 2)).Build().Perform();
                Thread.Sleep(1000);
                Accord.Point p2 = z3dvp.GetIntersectionPoints(Inavigationone5, testid, 52, "red", "Horizontal", 1);

                Thread.Sleep(10000);
                Actions act5 = new Actions(Driver);
                act5.MoveToElement(Inavigationone5, (int)p1.X, (int)p1.Y).ClickAndHold().MoveToElement(Inavigationone5, (int)p2.X, (int)p2.Y).Release().Build().Perform();
                Thread.Sleep(20000);

                List<string> checkvalue5 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                List<string> iNavatitopleft5 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                bool lfalg5 = false;
                if (checkvalue5.Count > 0 && iNavatitopleft5.Count > 0)
                {
                    if (string.Equals(checkvalue5[1], checkvalue5[3]) && string.Equals(iNavatitopleft5[1], iNavatitopleft5[3]))
                    {
                        lfalg5 = true;
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                }
                if (lfalg5 == false)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 6 Apply the Roam tool and navigation 2 and result control should be same   // this doc file , not update in test case , but code corrected 
                bool btool6 = z3dvp.select3DTools(Z3DTools.Pan, BluRingZ3DViewerPage.Navigationtwo);
                IWebElement navigationtwo = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                bool lfalg6 = false;
                if (btool6)
                {
                    Actions act6 = new Actions(Driver);
                    act6.MoveToElement(navigationtwo, navigationtwo.Size.Width/4 - 10,
                    navigationtwo.Size.Height /4- 5).ClickAndHold().DragAndDropToOffset(navigationtwo, navigationtwo.Size.Width / 4 - 20, navigationtwo.Size.Height / 4 - 10).Release().Build().Perform();
                    Thread.Sleep(10000);
                    List<string> checkvalue6_loc = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                    List<string> checkvalue6_imag = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                    if (checkvalue6_loc[1] == checkvalue6_loc[3] && checkvalue6_imag[1] == checkvalue6_imag[3])
                    {
                        lfalg6 = true;
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                }
                if (lfalg6 == false)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 7 Apply the scroll tool to navigation control 2 and Select the Navigation control 2 in MPR result control
                bool btool7 = z3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                bool bflag7 = false;
                if (btool7)
                {
                    z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.ResultPanel);
                    IWebElement ViewerContainer7 = z3dvp.ViewerContainer();
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((ViewerContainer7.Location.X + 600), (ViewerContainer7.Location.Y / 2 + 300));
                    for (int i = 0; i < 10; i++)
                    {
                        BasePage.mouse_event(0x0800, 0, 0, 25, 0);
                        Thread.Sleep(1000);
                    }
                    List<string> checkvalue7_loc = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                    List<string> checkvalue7_imag = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                    if (checkvalue7_loc[1] == checkvalue7_loc[3] && checkvalue7_imag[1] == checkvalue7_imag[3])
                    {
                        bflag7 = true;
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                }
                if (bflag7 == false)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 8 Apply the window level tool in navigation 2 and should not modified in result panel
                bool btool8 = z3dvp.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.Navigationtwo);
                bool bflag8 = false;
                new Actions(Driver).SendKeys("x").Build().Perform();
                if (btool8)
                {

                    IWebElement ViewerContainer8 = z3dvp.ViewerContainer();
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((ViewerContainer8.Location.X + 900), (ViewerContainer8.Location.Y / 2 + 400));
                    Actions act8 = new Actions(Driver);
                    //  act8.MoveToElement(navigationtwo, navigationtwo.Size.Width - 10, navigationtwo.Size.Height - 5).Click().DragAndDropToOffset(navigationtwo, 200, 500).Release().Build().Perform();
                    act8.MoveToElement(navigationtwo, navigationtwo.Size.Width - 10, navigationtwo.Size.Height - 10).DragAndDropToOffset(navigationtwo, 200, 500).Build().Perform();
                    Thread.Sleep(1000);
                    if(Config.BrowserType.ToLower()!="firefox")
                    act8.MoveToElement(navigationtwo, navigationtwo.Size.Width - 10, navigationtwo.Size.Height - 5).ClickAndHold().DragAndDropToOffset(navigationtwo, 150, 150).Release().Build().Perform();
                    Thread.Sleep(5000);
                    List<string> checkvalue8 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                    if (checkvalue8.Count > 0)
                    {
                        if (checkvalue8[1] != checkvalue8[3])
                        {
                            bflag8 = true;
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                    }
                }
                if (bflag8 == false)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 9 Apply the window level tool in result panel the values should not chang in navigation 2
                z3dvp.select3DTools(Z3DTools.Reset);
                bool btool9 = z3dvp.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.ResultPanel);
                bool bflag9 = false;
                IWebElement iresult = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                if (btool9)
                {
                    Actions act9 = new Actions(Driver);
                    //    act9.MoveToElement(iresult, iresult.Size.Width - 10,iresult.Size.Height - 5).ClickAndHold().DragAndDropToOffset(iresult, 200, 500).Release().Build().Perform();
                    if(Config.BrowserType.ToLower()!="firefox" && Config.BrowserType.ToLower()!="Mozilla")
                        {
                        act9.MoveToElement(iresult, iresult.Size.Width - 10, iresult.Size.Height - 10).DragAndDropToOffset(iresult, 50, 50).Build().Perform();
                        Thread.Sleep(3000);
                         act9.MoveToElement(iresult, iresult.Size.Width - 10, iresult.Size.Height - 5).ClickAndHold().DragAndDropToOffset(iresult, 50, 50).Release().Build().Perform();
                        Thread.Sleep(5000);
                    }
                    if(Config.BrowserType.ToLower()=="firefox" || Config.BrowserType.ToLower()=="mozilla")
                    {
                        z3dvp.Performdragdrop(iresult, 20, 30);
                    }
                    List<string> checkvalue9 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                    if (checkvalue9.Count > 0)
                    {
                        if (checkvalue9[3] != checkvalue9[1])
                        {
                            bflag9 = true;
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                    }
                }
                if (bflag9 == false)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 10 Select the navigation control 3 in the MPR result control from the top right corner options drop down.
                //preconditon for 11 step
                new Actions(Driver).SendKeys("x").Build().Perform();
                bool btool10 = z3dvp.select3DTools(Z3DTools.Reset);
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.ResultPanel);
                List<string> Iresult10 = z3dvp.GetControlvalues(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationthree);
                bool bflag10 = false;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (Iresult10[3] == BluRingZ3DViewerPage.Navigationthree)
                {
                    IList<IWebElement> Viewport7 = z3dvp.Viewport();
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport7[3].Location.X / 2 + 400), (Viewport7[3].Location.Y / 2 + 250));
                    //    z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, false);
                    new Actions(Driver).SendKeys("T").Build().Perform();
                    Thread.Sleep(500);
                    Thread.Sleep(1000);
                    if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel)))
                    {
                        bflag10 = true;
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    //    z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, true);
                    new Actions(Driver).SendKeys("T").Build().Perform();
                    Thread.Sleep(500);
                }
                if (bflag10 == false)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //step 11 Using the traverse tool, move the cross hair of navigation control 1 upward until the position annotation of navigation control 3 reads "0, 0, 100".
                IWebElement ithumnail = Driver.FindElement(By.CssSelector(Locators.CssSelector.thumbclick));
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationone);
                Actions act11 = new Actions(Driver);
                string[] arrsplit11 = null;
                bool Nav13_one = z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationone);
                act11.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2)
                .ClickAndHold().MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2, z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2 - 141).Release().Build().Perform();
                Thread.Sleep(10000);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                List<string> result11 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                List<string> result11_a = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                arrsplit11 = result11[2].Split(',');
                double split11 = double.Parse(arrsplit11[2].Trim().Substring(0, 3));
               // if (split11 >= 107) split13 = 102;//firefox this  is updated 
                //if (result11[2] == result11[3] && arrsplit11[2].Trim().IndexOf("100") >= 0 && result11_a[2] == result11_a[3])
                if (result11[2] == result11[3] && split11>=100 && result11_a[2] == result11_a[3])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 12 Adjust the thickness to be 10.0 mm on navigation control 3.
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationthree, sThickness);
                // List<string> checkvalue14 = z3dvp.GetAttributes_Result(Locators.CssSelector.sThickness, null, null);
                List<string> checkvalue14 = z3dvp.GetControlvalues(BluRingZ3DViewerPage.Thickness, "10 mm");
                if (checkvalue14[0].Substring(0, 2) == sThickness && checkvalue14[1].Substring(0, 2) == sThickness && checkvalue14[2].Substring(0, 2) == sThickness && checkvalue14[3].Substring(0, 2) != sThickness)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 13 Apply the thickenesss value in result panel should not affect to remainin gpanel 
                z3dvp.EnterThickness(BluRingZ3DViewerPage.ResultPanel, sThickness15);
                //  List<string> checkvalue15 = z3dvp.GetAttributes_Result(Locators.CssSelector.sThickness, null, null);
                List<string> checkvalue15 = z3dvp.GetControlvalues(BluRingZ3DViewerPage.Thickness, "10 mm");
                if (checkvalue15[0].Substring(0, 2) != sThickness15 && checkvalue15[1].Substring(0, 2) != sThickness15 && checkvalue15[2].Substring(0, 2) != sThickness15 && checkvalue15[3].Substring(0, 2).Trim() == sThickness15)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 14 Apply the window/level preset in navigation 3 
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationthree, spresetvalue16, BluRingZ3DViewerPage.Preset);
                List<string> checkvalue16 = z3dvp.GetControlvalues(BluRingZ3DViewerPage.Preset, spresetvalue16);
                if (checkvalue16[0] == spresetvalue16 && checkvalue16[1] == spresetvalue16 && checkvalue16[2] == spresetvalue16 && checkvalue16[3] != spresetvalue16)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 15 Apply the preset values in Result panel
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, sPresetvalue17, BluRingZ3DViewerPage.Preset);
                List<string> checkvalue17 = z3dvp.GetControlvalues(BluRingZ3DViewerPage.Preset, spresetvalue16);
                List<string> checkvalue17_a = z3dvp.GetControlvalues(BluRingZ3DViewerPage.Preset, sPresetvalue17);
                if (checkvalue17[0] != sPresetvalue17 && checkvalue17[1] != sPresetvalue17 && checkvalue17[2] != sPresetvalue17 && checkvalue17_a[3] == sPresetvalue17)
                {
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
                // Driver.Close();
            }
        }

    }

}


