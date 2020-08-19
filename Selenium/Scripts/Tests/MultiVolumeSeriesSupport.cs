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


namespace Selenium.Scripts.Tests
{
    class MultiVolumeSeriesSupport : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public Imager imager = new Imager();
        DirectoryInfo CurrentDir = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());
        public Cursor Cursor { get; private set; }
        public MultiVolumeSeriesSupport(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }

        public TestCaseResult Test_163375(string testid, String teststeps, int stepcount)
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
                //Step 1 :: From the Universal viewer , Select a 3D supported series and Select the MPR view option from the smart view drop down.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool step1 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR);
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
                //Step 2 :: From the View port top bar options, Select the sub volumes list and verify the number of images.
                String TotalImage = z3dvp.GetSubVolumeImageCount(BluRingZ3DViewerPage.Navigationtwo, "Sub Volumes");
               //  string TotalImage = z3dvp.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigationtwo," 112 images", "Sub Volumes");
                if (TotalImage.Contains("112 images"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 3 :: Compare the value of the number of images that was check marked with the number of images of the selected series in the ICA thumbnail bar.
                SwitchToDefault();
                SwitchToUserHomeFrame();
                IList<IWebElement> thumbnail = Driver.FindElements(By.CssSelector(Locators.CssSelector.ThumbnailImageCount));
                string ImagesCount = thumbnail[1].Text;
                if (TotalImage.Contains(ImagesCount))
                {
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

        public TestCaseResult Test_163376(string testid, String teststeps, int stepcount)
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
                //Step 1 :: From the Universal viewer , Select a 3D supported series and Select the MPR view option from the smart view drop down.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool step1 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR);
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
                //Step2 :: From the viewport top bar, Select the sub volumes option.
                IList<String> step2_Options = z3dvp.getAllMenuItems(BluRingZ3DViewerPage.Navigationone);
                if(step2_Options.Count ==17)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 3 :: Select any of the volumes from the list.
                bool Step3 = z3dvp.VerifySubVolumeLoadinBar(BluRingZ3DViewerPage.Navigationone, "1. 99 images" );
                if(Step3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 4 :: Try loading all the series from the list.
              
                String[] SubVolumearray = new String[] { "2. 98 images", "3. 98 images", "4. 98 images", "5. 98 images", "6. 98 images"
                                                        ,"7. 98 images", "8. 98 images","9. 98 images","10. 98 images","11. 98 images"
                                                        ,"12. 98 images", "13. 98 images","14. 98 images","15. 98 images","16. 98 images"};
                int counter = 0;
                foreach (String subvolume in SubVolumearray)
                {
                    bool res = z3dvp.VerifySubVolumeLoadinBar(BluRingZ3DViewerPage.Navigationone, subvolume);
                    if (res == true)
                        counter++;
                }

                if (counter > 0 && counter <= SubVolumearray.Count())
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step5 :: elect the 3D 4:1 view option from the smart view drop down.
                bool ThreeD4x1 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                if(ThreeD4x1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step6 :: Verify the clipping lines position in 3D navigation control.
                IWebElement ViewerContainer = z3dvp.ViewerContainer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer, removeCurserFromPage: true))
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
                //Step7 :: Select the volume from the list that has been already loaded.
                //Issue  :: ICA-17979
                 bool res7 = z3dvp.VerifySubVolumeLoadinBar(BluRingZ3DViewerPage.Navigationone, "13. 98 images" , Verifyselected:false);
                if (res7)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].comments = "Related to Jira ICA-17979";
                }
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
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

        public TestCaseResult Test_163377(String testid, String teststeps, int stepcount)
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
            string testdatades = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            try
            {
                //Step 1   Login as a administrator 
                login.LoginIConnect(username, password);
                z3dvp.Deletefiles(testcasefolder);

                //Step 1   From the Universal viewer , Select a 3D supported series and Select the MPR view option from the smart view drop down.
                bool res = z3dvp.searchandopenstudyin3D(Patientid, thumbnailcaption);
                IWebElement Inavigationone = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                bool bfalg1 = false;
                int subvol_chk = -1;
                string default_selvol = "";
                IWebElement MenuClose = null;
                if (res == true)
                {
                    IList<IWebElement> sysvolume = SysVolumes(BluRingZ3DViewerPage.Navigationone,true);
                    MenuClose = z3dvp.IMenuClose();
                    if (sysvolume.Count>=0)
                    {
                        bfalg1 = true;
                        default_selvol = sysvolume[0].Text;
                        default_selvol = default_selvol.Substring(0, default_selvol.LastIndexOf(" "));
                        // new Actions(Driver).MoveToElement(Inavigationone, Inavigationone.Size.Width / 2, Inavigationone.Size.Height / 2).Click().Build().Perform();
                        if (MenuClose.Displayed && MenuClose.Enabled) MenuClose.Click();Thread.Sleep(1000);
                        Thread.Sleep(1000);
                        //IWebElement closeoptions = Inavigationone.FindElement(By.CssSelector(Locators.CssSelector.menubutton));
                        //ClickElement(closeoptions);
                        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.menutable)));
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                }
                if (bfalg1 == false)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 2 Apply the Rotation tool on the image in any one of the navigation control.
                bool btool2 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).SendKeys("X").Build().Perform();Thread.Sleep(1000);
                bool bflag2 = false;
                if (btool2)
                {

                    List<string> before_result2 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                    Actions act4 = new Actions(Driver);
                    act4.MoveToElement(Inavigationone, Inavigationone.Size.Width / 2, Inavigationone.Size.Height / 2).ClickAndHold()
                    .MoveToElement(Inavigationone, Inavigationone.Size.Width / 2, Inavigationone.Size.Height / 2 + 100)
                    .Release().Build().Perform();
                    Thread.Sleep(10000);
                    List<string> After_result2 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                    if (before_result2[0] != After_result2[0])
                    {
                        bflag2 = true;
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                }
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(500);
                if (bflag2 == false)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //step 3  Select the Sub-volumes options under the top viewport bar and load another volume first one is selected. 
                bool bflag4 = false;
                IList<IWebElement> sysvolume4 = SysVolumes(BluRingZ3DViewerPage.Navigationone);
                string subone = "";
                if (sysvolume4.Count >= 0)
                {
                    subone = sysvolume4[0].Text;
                    Thread.Sleep(1000);
                    sysvolume4[0].Click(); Thread.Sleep(1000);
                    //   new Actions(Driver).MoveToElement(Inavigationone, Inavigationone.Size.Width / 2, Inavigationone.Size.Height / 2).Click().Build().Perform();
                    try { MenuClose = z3dvp.IMenuClose(); if (MenuClose.Displayed && MenuClose.Enabled) MenuClose.Click(); Thread.Sleep(1000); } catch { }
                    
                    Thread.Sleep(1000);
                      
                        IList<IWebElement> sysvolume_checkl = SysVolumes(BluRingZ3DViewerPage.Navigationone,true);
                        if(!sysvolume_checkl[0].Text.Contains(default_selvol))
                        {
                            bflag4 = true;
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                    //  new Actions(Driver).MoveToElement(Inavigationone, Inavigationone.Size.Width / 2, Inavigationone.Size.Height / 2).Click().Build().Perform();
                    try { MenuClose = z3dvp.IMenuClose(); if (MenuClose.Displayed && MenuClose.Enabled) MenuClose.Click(); Thread.Sleep(1000); } catch { }
                    Thread.Sleep(1000);
                    //IWebElement closeoptions = Inavigationone.FindElement(By.CssSelector(Locators.CssSelector.menubutton));
                    //ClickElement(closeoptions);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.menutable)));
            }
                if (bflag4 == false)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 4 Apply the tool operations zoom,w/l,measurement on the images in navigation controls.
                List<string> Beforewindow4 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                z3dvp.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.Navigationone);
                z3dvp.DragandDropelement(BluRingZ3DViewerPage.Navigationone);
                List<string> Afterwindow4 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                Thread.Sleep(5000);
                z3dvp.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.Navigationone);
                List<string> BeforeZoom4 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                z3dvp.DragandDropelement(BluRingZ3DViewerPage.Navigationone);
                List<string> AfterZoom4 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (BeforeZoom4[0] != AfterZoom4[0] && Beforewindow4[0] != Afterwindow4[0])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 5 Again load the series that has been initially under sub-volumes list in step 1 (Load 3subvolume).
                bool bflag5 = false;
                IList<IWebElement> sysvolume5 = SysVolumes(BluRingZ3DViewerPage.Navigationone);
                if (sysvolume5.Count >= 0)
                {
                    for (int i = 0; i <= sysvolume5.Count; i++)
                    {
                        if (sysvolume5[i].Text.Contains(default_selvol))
                        {
                            sysvolume5[i].Click();
                            break;
                        }
                    }
                    //  new Actions(Driver).MoveToElement(Inavigationone, Inavigationone.Size.Width / 2, Inavigationone.Size.Height / 2).Click().Build().Perform();
                    try { MenuClose = z3dvp.IMenuClose(); if (MenuClose.Displayed && MenuClose.Enabled) MenuClose.Click(); Thread.Sleep(1000); } catch { }
                    Thread.Sleep(1000);
                    IList<IWebElement> sysvolume5_selected = SysVolumes(BluRingZ3DViewerPage.Navigationone, true);
                    if (sysvolume5_selected[0].Text.Contains(default_selvol))
                    {
                        bflag5 = true;
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                }
                if(bflag5==false)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //    new Actions(Driver).MoveToElement(Inavigationone, Inavigationone.Size.Width / 2, Inavigationone.Size.Height / 2).Click().Build().Perform();
                try { MenuClose = z3dvp.IMenuClose(); if (MenuClose.Displayed && MenuClose.Enabled) MenuClose.Click(); Thread.Sleep(1000); } catch { }
                Thread.Sleep(1000);

                //Step 6  Verify the rotation is preserved on the image. 
                new Actions(Driver).MoveToElement(Inavigationone, Inavigationone.Size.Width / 2, Inavigationone.Size.Height / 2).Click().Build().Perform();
                Thread.Sleep(1000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Thread.Sleep(1000);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), removeCurserFromPage:true))
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

                //Step 7 Again load the sub- volumes-1 list in step3 (Load first subvolues).
                bool bflag7 = false; 
                List<string> VerfiyZoom7 = null; Thread.Sleep(1000);
                List<string> verfiywindow7 = null;
                IList<IWebElement> sysvolume7 = SysVolumes(BluRingZ3DViewerPage.Navigationone); Thread.Sleep(1000);
                if(sysvolume7.Count>=0)
                {
                    for(int i=0;i<sysvolume7.Count;i++)
                    {
                        if(sysvolume7[i].Text.Contains(subone))
                        {
                            sysvolume7[i].Click();
                            break;
                        }
                    }
                    VerfiyZoom7 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                    verfiywindow7 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                    IList<IWebElement> sysvolume7_check = SysVolumes(BluRingZ3DViewerPage.Navigationone,true); Thread.Sleep(1000);
                    if (sysvolume7_check.Count>=0 && sysvolume7_check[0].Text.Contains(subone))
                    {
                        bflag7 = true;
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                }
                //      new Actions(Driver).MoveToElement(Inavigationone, Inavigationone.Size.Width / 2, Inavigationone.Size.Height / 2).Click().Build().Perform();
                try { MenuClose = z3dvp.IMenuClose(); if (MenuClose.Displayed && MenuClose.Enabled) MenuClose.Click(); Thread.Sleep(1000); } catch { }
                Thread.Sleep(1000);
                if (bflag7 == false)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 8 Verify the tool operations are preserved for the loaded volumes.
                if (VerfiyZoom7[0] == AfterZoom4[0] && verfiywindow7[0] == Afterwindow4[0])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //4:1 Layout Mode Repeat steps 2-6 in all viewing modes.
                bool res2_view = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4, "y");
                IWebElement IthreeDone = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                
                string default_selvol4_1 = "";
                //get the subvaluedefault value of 4:1
                IList<IWebElement> sysvolume4_1 = SysVolumes(BluRingZ3DViewerPage.Navigation3D1, true,true);
                if (sysvolume4_1.Count >= 0)
                {
                    default_selvol4_1 = sysvolume4_1[0].Text;
                    default_selvol4_1 = default_selvol4_1.Substring(0, default_selvol4_1.LastIndexOf(" "));
                    //     new Actions(Driver).MoveToElement(IthreeDone, IthreeDone.Size.Width / 2, IthreeDone.Size.Height / 2).Click().Build().Perform();
                    try { MenuClose = z3dvp.IMenuClose(); if (MenuClose.Displayed && MenuClose.Enabled) MenuClose.Click(); Thread.Sleep(1000); } catch { }
                    Thread.Sleep(1000);
                }

                //step 2 Apply the Rotation tool on the image in any one of the navigation control.             
                bool btool4_2 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center);
                new Actions(Driver).SendKeys("X").Build().Perform();Thread.Sleep(1000);
                bool bflag4_2 = false;
              //  if (btool4_2)
            //    {

                    List<string> before41_result2 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                    Actions act41 = new Actions(Driver);
                act41.MoveToElement(IthreeDone, IthreeDone.Size.Width / 2, IthreeDone.Size.Height / 2).ClickAndHold()
                    .MoveToElement(IthreeDone, IthreeDone.Size.Width-100, IthreeDone.Size.Height- 200)
                    .Release().Build().Perform();
                    Thread.Sleep(10000);
                    List<string> After41_result2 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                    if (before41_result2[3] != After41_result2[3])
                    {
                        bflag4_2 = true;
                    }

              //  }
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(500);

                //Step 3 Select the Sub-volumes options in Hover bar and load sub volume- 4. (load sub volume 3 )
                bool bflag4_3 = false;
                IList<IWebElement> sysvolume4_3 = SysVolumes(BluRingZ3DViewerPage.Navigation3D1,four_one:true);
                string subone4_3 = "";
                if (sysvolume4_3.Count >= 0)
                {
                    subone4_3 = sysvolume4_3[2].Text;
                    Thread.Sleep(1000);
                    sysvolume4_3[2].Click(); Thread.Sleep(1000);
                    //   new Actions(Driver).MoveToElement(IthreeDone, IthreeDone.Size.Width / 2, IthreeDone.Size.Height / 2).Click().Build().Perform();
                    try { MenuClose = z3dvp.IMenuClose(); if (MenuClose.Displayed && MenuClose.Enabled) MenuClose.Click(); Thread.Sleep(1000); } catch { }
                    Thread.Sleep(1000);

                    IList<IWebElement> sysvolume_checkl = SysVolumes(BluRingZ3DViewerPage.Navigation3D1, values:true, four_one: true);
                    if(sysvolume_checkl.Count==0)
                    {
                        //   new Actions(Driver).MoveToElement(IthreeDone, IthreeDone.Size.Width / 2, IthreeDone.Size.Height / 2).Click().Build().Perform();
                        try { MenuClose = z3dvp.IMenuClose(); if (MenuClose.Displayed && MenuClose.Enabled) MenuClose.Click(); Thread.Sleep(1000); } catch { }
                        Thread.Sleep(1000);
                        sysvolume_checkl = SysVolumes(BluRingZ3DViewerPage.Navigation3D1, values: true, four_one: true);
                    }
                    if (!sysvolume_checkl[0].Text.Contains(default_selvol4_1))
                    {
                        bflag4_3 = true;
                        
                    }
                    //   new Actions(Driver).MoveToElement(IthreeDone, IthreeDone.Size.Width / 2, IthreeDone.Size.Height / 2).Click().Build().Perform();
                    try { MenuClose = z3dvp.IMenuClose(); if (MenuClose.Displayed && MenuClose.Enabled) MenuClose.Click(); Thread.Sleep(1000); } catch { }
                    Thread.Sleep(1000);
                }

                //Step 4 Apply the tool operations zoom,w/l,measurement,sculpt etc.. on the images in navigation controls.
                bool bflag4_4 = false;

                z3dvp.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.Navigation3D1);
                new Actions(Driver).SendKeys("x").Build().Perform();
                Thread.Sleep(1000);
                z3dvp.DragandDropelement(BluRingZ3DViewerPage.Navigation3D1, IthreeDone.Size.Width/2, IthreeDone.Size.Height/2);
                Thread.Sleep(2000);
                z3dvp.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.Navigation3D1);
                z3dvp.DragandDropelement(BluRingZ3DViewerPage.Navigation3D1, IthreeDone.Size.Width / 2, IthreeDone.Size.Height / 2);
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(1000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 44);
                if (CompareImage(result.steps[ExecutedSteps], IthreeDone, removeCurserFromPage: true,pixelTolerance:200))
                {
                    bflag4_4 = true;

                }
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(1000);
                new Actions(Driver).SendKeys("x").Build().Perform();
                Thread.Sleep(1000);
                //step 5 Again load the series that has been initially under sub-volumes list in step 1 (load subvolume 1)
                bool bflag4_5 = false;
                IList<IWebElement> sysvolume4_5 = SysVolumes(BluRingZ3DViewerPage.Navigation3D1, four_one: true);
                if(sysvolume4_5.Count==0)
                {
                    //  new Actions(Driver).MoveToElement(IthreeDone, IthreeDone.Size.Width / 2, IthreeDone.Size.Height / 2).Click().Build().Perform();
                    try { MenuClose = z3dvp.IMenuClose(); if (MenuClose.Displayed && MenuClose.Enabled) MenuClose.Click(); Thread.Sleep(1000); } catch { }
                    Thread.Sleep(1000);
                    sysvolume4_5 = SysVolumes(BluRingZ3DViewerPage.Navigation3D1, four_one: true);
                }
                if (sysvolume4_5.Count >= 0)
                {
                    for (int i = 0; i <= sysvolume4_5.Count; i++)
                    {
                        if (sysvolume4_5[i].Text.Contains(default_selvol4_1))
                        {
                            sysvolume4_5[i].Click();
                            break;
                        }
                    }
                    //   new Actions(Driver).MoveToElement(IthreeDone, IthreeDone.Size.Width / 2, IthreeDone.Size.Height / 2).Click().Build().Perform();
                    try { MenuClose = z3dvp.IMenuClose(); if (MenuClose.Displayed && MenuClose.Enabled) MenuClose.Click(); Thread.Sleep(1000); } catch { }
                    Thread.Sleep(1000);
                    IList<IWebElement> sysvolume5_selected = SysVolumes(BluRingZ3DViewerPage.Navigation3D1, values:true, four_one: true);
                    if(sysvolume5_selected.Count==0)
                    {
                        //   new Actions(Driver).MoveToElement(IthreeDone, IthreeDone.Size.Width / 2, IthreeDone.Size.Height / 2).Click().Build().Perform();
                        try { MenuClose = z3dvp.IMenuClose(); if (MenuClose.Displayed && MenuClose.Enabled) MenuClose.Click(); Thread.Sleep(1000); } catch { }
                        Thread.Sleep(1000);
                        sysvolume5_selected = SysVolumes(BluRingZ3DViewerPage.Navigation3D1, values: true, four_one: true);
                    }
                    if (sysvolume5_selected[0].Text.Contains(default_selvol4_1))
                    {
                        bflag4_5 = true;
                        
                    }
                }
                //Step 6  Verify the rotation is preserved on the image.
                //   new Actions(Driver).MoveToElement(IthreeDone, IthreeDone.Size.Width / 2, IthreeDone.Size.Height / 2).Click().Build().Perform();
                try { MenuClose = z3dvp.IMenuClose(); if (MenuClose.Displayed && MenuClose.Enabled) MenuClose.Click(); Thread.Sleep(1000); } catch { }
                Thread.Sleep(1000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 46);
                Thread.Sleep(1000);
                bool bflag4_6 = false;
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1), removeCurserFromPage: true,pixelTolerance:400))
                {
                    bflag4_6 = true;
                }
                //Step 7 Again load the sub- volumes-1 list in step3 (Load Third subvolues).
                bool bflag4_7 = false;
                IList<IWebElement> sysvolume4_7 = SysVolumes(BluRingZ3DViewerPage.Navigation3D1, four_one: true); Thread.Sleep(1000);
                if (sysvolume4_7.Count >= 0)
                {
                    for (int i = 0; i < sysvolume4_7.Count; i++)
                    {
                        if (sysvolume4_7[i].Text.Contains(subone4_3))
                        {
                            sysvolume4_7[i].Click();
                            break;
                        }
                    }
                    try { MenuClose = z3dvp.IMenuClose(); if (MenuClose.Displayed && MenuClose.Enabled) MenuClose.Click(); Thread.Sleep(1000); } catch { }
                    IList<IWebElement> sysvolume7_check = SysVolumes(BluRingZ3DViewerPage.Navigation3D1, values:true, four_one: true); Thread.Sleep(1000);
                    if (sysvolume7_check.Count == 0)
                    {
                        // new Actions(Driver).MoveToElement(IthreeDone, IthreeDone.Size.Width / 2, IthreeDone.Size.Height / 2).Click().Build().Perform();
                        try { MenuClose = z3dvp.IMenuClose(); if (MenuClose.Displayed && MenuClose.Enabled) MenuClose.Click(); Thread.Sleep(1000); } catch { }
                        Thread.Sleep(1000);
                        sysvolume7_check = SysVolumes(BluRingZ3DViewerPage.Navigation3D1, values: true, four_one: true); Thread.Sleep(1000);
                    }
                    if (sysvolume7_check.Count >= 0 && sysvolume7_check[0].Text.Contains(subone4_3))
                    {
                        bflag4_7 = true;
                    }
                }
                //     new Actions(Driver).MoveToElement(IthreeDone, IthreeDone.Size.Width / 2, IthreeDone.Size.Height / 2).Click().Build().Perform();
                try { MenuClose = z3dvp.IMenuClose(); if (MenuClose.Displayed && MenuClose.Enabled) MenuClose.Click(); Thread.Sleep(1000); } catch { }
                Thread.Sleep(1000);

                //step 8 Verify the tool operations are preserved for the loaded volumes.
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(1000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 48);
                Thread.Sleep(1000);
                bool bflag4_8 = false;
                if (CompareImage(result.steps[ExecutedSteps], IthreeDone, removeCurserFromPage: true,pixelTolerance:400))
                {
                    bflag4_8 = true;
                }
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(1000);
                bool bstatus4_1 = false;

                if(bflag4_2 && bflag4_3 && bflag4_4 && bflag4_5 && bflag4_6 && bflag4_7 && bflag4_8)
                {
                    bstatus4_1 = true;
                }
                //6:1 layout  view step 2 rotation apply 
                bool res2_six = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6, "y");
                IWebElement Navigsix= z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);

                string default_selvolsix = "";
                //get the subvaluedefault value of 6:1
                IList<IWebElement> sysvolume6_1 = SysVolumes(BluRingZ3DViewerPage.Navigationone, true);
                if (sysvolume6_1.Count >= 0)
                {
                    default_selvolsix = sysvolume6_1[0].Text;
                    default_selvolsix = default_selvolsix.Substring(0, default_selvolsix.LastIndexOf(" "));
                    // new Actions(Driver).MoveToElement(Navigsix, Navigsix.Size.Width / 2, Navigsix.Size.Height / 2).Click().Build().Perform();
                    try { MenuClose = z3dvp.IMenuClose(); if (MenuClose.Displayed && MenuClose.Enabled) MenuClose.Click(); Thread.Sleep(1000); } catch { }
                    Thread.Sleep(1000);
                }

                //6:1 step 2 Apply the Rotation tool on the image in any one of the navigation control.
                bool btool6_2 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).SendKeys("X").Build().Perform();Thread.Sleep(1000);
                bool bflag6_2 = false;
                if (btool6_2)
                {

                    List<string> before_result2 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                    Actions act4 = new Actions(Driver);
                    act4.MoveToElement(Navigsix, Navigsix.Size.Width / 2, Navigsix.Size.Height / 2).ClickAndHold()
                    .MoveToElement(Navigsix, Navigsix.Size.Width - 100, Navigsix.Size.Height - 200)
                    .Release().Build().Perform();
                    Thread.Sleep(10000);
                    new Actions(Driver).MoveToElement(Navigsix, Navigsix.Size.Width / 2, Navigsix.Size.Height / 2).ClickAndHold()
                .MoveToElement(Navigsix, Navigsix.Size.Width / 2, Navigsix.Size.Height / 2 + 100)
                .Release()
                .Build()
                .Perform(); Thread.Sleep(10000);
                    new Actions(Driver).MoveToElement(Navigsix, Navigsix.Size.Width / 2, Navigsix.Size.Height / 2).Click().Build().Perform();
                    Thread.Sleep(1000);
                    List<string> After_result2 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                    if (before_result2[0] != After_result2[0])
                    {
                        bflag6_2 = true;
                    }
                }
              //  new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(500);

                //6:1 //Step 3 Select the Sub-volumes options in Hover bar and load sub volume- 4. (load sub volume 1 )
                bool bflag6_3 = false;
                IList<IWebElement> sysvolume6_3 = SysVolumes(BluRingZ3DViewerPage.Navigationone);
                string subone6_3 = "";
                if (sysvolume6_3.Count >= 0)
                {
                    subone6_3 = sysvolume6_3[0].Text;
                    Thread.Sleep(1000);
                    sysvolume6_3[0].Click(); Thread.Sleep(1000);
                    //  new Actions(Driver).MoveToElement(Navigsix, Navigsix.Size.Width / 2, Navigsix.Size.Height / 2).Click().Build().Perform();
                    try { MenuClose = z3dvp.IMenuClose(); if (MenuClose.Displayed && MenuClose.Enabled) MenuClose.Click(); Thread.Sleep(1000); } catch { }
                    Thread.Sleep(1000);

                    IList<IWebElement> sysvolume_checkl = SysVolumes(BluRingZ3DViewerPage.Navigationone, values: true);
                    if (!sysvolume_checkl[0].Text.Contains(default_selvolsix))
                    {
                        bflag6_3 = true;
                      
                    }
                    //    new Actions(Driver).MoveToElement(Navigsix, Navigsix.Size.Width / 2, Navigsix.Size.Height / 2).Click().Build().Perform();
                    try { MenuClose = z3dvp.IMenuClose(); if (MenuClose.Displayed && MenuClose.Enabled) MenuClose.Click(); Thread.Sleep(1000); } catch { }
                    Thread.Sleep(1000);
                }
                //step 6:1 //Step 4 Apply the tool operations zoom,w/l,measurement,sculpt etc.. on the images in navigation controls.
                bool bflag6_4 = false;

                List<string> Beforewindow6_4 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                z3dvp.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.Navigationone);
                z3dvp.DragandDropelement(BluRingZ3DViewerPage.Navigationone);
                List<string> Afterwindow6_4 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                Thread.Sleep(5000);
                z3dvp.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.Navigationone);
                List<string> BeforeZoom6_4 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                z3dvp.DragandDropelement(BluRingZ3DViewerPage.Navigationone);
                List<string> AfterZoom6_4 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (BeforeZoom6_4[0] != AfterZoom6_4[0] && Beforewindow6_4[0] != Afterwindow6_4[0])
                {
                    bflag6_4 = true;
                }
                //Step 6:1  step 5 Again load the series that has been initially under sub-volumes list in step 1 (Load 3 subvolume).
                bool bflag6_5 = false;
                IList<IWebElement> sysvolume6_5 = SysVolumes(BluRingZ3DViewerPage.Navigationone);
                if (sysvolume6_5.Count >= 0)
                {
                    for (int i = 0; i <= sysvolume6_5.Count; i++)
                    {
                        if (sysvolume6_5[i].Text.Contains(default_selvolsix))
                        {
                            sysvolume6_5[i].Click();
                            break;
                        }
                    }
                    //   new Actions(Driver).MoveToElement(Navigsix, Navigsix.Size.Width / 2, Navigsix.Size.Height / 2).Click().Build().Perform();
                    try { MenuClose = z3dvp.IMenuClose(); if (MenuClose.Displayed && MenuClose.Enabled) MenuClose.Click(); Thread.Sleep(1000); } catch { }
                    Thread.Sleep(1000);
                    IList<IWebElement> sysvolume5_selected = SysVolumes(BluRingZ3DViewerPage.Navigationone, true);
                    if (sysvolume5_selected[0].Text.Contains(default_selvolsix))
                    {
                        bflag6_5 = true;
                    }
                }

                //step 6:1 step 6  Verify the rotation is preserved on the image. 
                bool bflag6_6 = false;
                //   new Actions(Driver).MoveToElement(Navigsix, Navigsix.Size.Width / 2, Navigsix.Size.Height / 2).Click().Build().Perform();
                try { MenuClose = z3dvp.IMenuClose(); if (MenuClose.Displayed && MenuClose.Enabled) MenuClose.Click(); Thread.Sleep(1000); } catch { }
                Thread.Sleep(1000);
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(1000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 61);
                Thread.Sleep(1000);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), removeCurserFromPage: true,pixelTolerance:400))
                {
                    bflag6_6 = true;
                }
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(1000);
                //step 6:1 step 7  Again load the sub- volumes-1 list in step3 (Load first subvolues).
                bool bflag6_7 = false;
               
                IList<IWebElement> sysvolume6_7 = SysVolumes(BluRingZ3DViewerPage.Navigationone); Thread.Sleep(1000);
                if (sysvolume6_7.Count >= 0)
                {
                    for (int i = 0; i < sysvolume6_7.Count; i++)
                    {
                        if (sysvolume6_7[i].Text.Contains(subone6_3))
                        {
                            sysvolume6_7[i].Click();
                            break;
                        }
                    }
                    try { MenuClose = z3dvp.IMenuClose(); if (MenuClose.Displayed && MenuClose.Enabled) MenuClose.Click(); Thread.Sleep(1000); } catch { }
                    IList<IWebElement> sysvolume7_check = SysVolumes(BluRingZ3DViewerPage.Navigationone, true); Thread.Sleep(1000);
                    if (sysvolume7_check.Count >= 0 && sysvolume7_check[0].Text.Contains(subone6_3))
                    {
                        bflag6_7 = true;
                      
                    }
                }
                //     new Actions(Driver).MoveToElement(Inavigationone, Inavigationone.Size.Width / 2, Inavigationone.Size.Height / 2).Click().Build().Perform();
                try { MenuClose = z3dvp.IMenuClose(); if (MenuClose.Displayed && MenuClose.Enabled) MenuClose.Click(); Thread.Sleep(1000); } catch { }
                Thread.Sleep(1000);
                //6:1 //step 8 Verify the tool operations are preserved for the loaded volumes.
                bool bflag6_8 = false;
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(1000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 68);
                Thread.Sleep(1000);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), removeCurserFromPage: true,pixelTolerance:400))
                {
                    bflag6_8 = true;
                }
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(1000);
                bool bstatus6_1 = false;
                if (bflag6_2 && bflag6_3 && bflag6_4 && bflag6_4 && bflag6_5 && bflag6_6 && bflag6_7  && bflag6_8)
                {
                    bstatus6_1 = true;
                }
                //CurveMpr 
                bool res2_cur = z3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR, "y");
                IWebElement Navigcur = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);

                string default_selvolcur = "";
                //get the subvaluedefault value of 6:1
                IList<IWebElement> sysvolume_cur = SysVolumes(BluRingZ3DViewerPage.Navigationone, true);
                if (sysvolume_cur.Count >= 0)
                {
                    default_selvolcur = sysvolume_cur[0].Text;
                    default_selvolcur = default_selvolcur.Substring(0, default_selvolcur.LastIndexOf(" "));
                    //  new Actions(Driver).MoveToElement(Navigcur, Navigcur.Size.Width / 2, Navigcur.Size.Height / 2).Click().Build().Perform();
                    try { MenuClose = z3dvp.IMenuClose(); if (MenuClose.Displayed && MenuClose.Enabled) MenuClose.Click(); Thread.Sleep(1000); } catch { }
                    Thread.Sleep(1000);
                }
                //Curved step 2 Apply the rotation 
                bool btoolcur_2 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
           //     new Actions(Driver).SendKeys("X").Build().Perform();
                bool bflagcur_2 = false;
                if (btoolcur_2)
                {

                    List<string> before_result2 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                    Actions act4 = new Actions(Driver);
                    act4.MoveToElement(Navigcur, Navigcur.Size.Width / 2, Navigcur.Size.Height / 2).ClickAndHold()
                    .MoveToElement(Navigcur, Navigcur.Size.Width - 100, Navigcur.Size.Height - 200)
                    .Release().Build().Perform();
                    Thread.Sleep(10000);
                    new Actions(Driver).MoveToElement(Navigcur, Navigcur.Size.Width / 2, Navigcur.Size.Height / 2).ClickAndHold()
               .MoveToElement(Navigcur, Navigcur.Size.Width / 2, Navigcur.Size.Height / 2 + 100)
               .Release()
               .Build()
               .Perform();Thread.Sleep(1000);
                    new Actions(Driver).MoveToElement(Navigcur, Navigcur.Size.Width / 2, Navigcur.Size.Height / 2).Click().Build().Perform();
                    Thread.Sleep(1000);
                    List<string> After_result2 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                    if (before_result2[0] != After_result2[0])
                    {
                        bflagcur_2 = true;
                    }
                }
          //      new Actions(Driver).SendKeys("X").Build().Perform();
          //      Thread.Sleep(500);

                //curved Step 3  Select the Sub-volumes options in Hover bar and load sub volume- . subolume 3
                bool bflagcur_3 = false;
                IList<IWebElement> sysvolumecur_3 = SysVolumes(BluRingZ3DViewerPage.Navigationone);
                string subonecur_3 = "";
                if (sysvolumecur_3.Count >= 0)
                {
                    subonecur_3 = sysvolumecur_3[2].Text;
                    Thread.Sleep(1000);
                    sysvolumecur_3[2].Click(); Thread.Sleep(1000);
                    //  new Actions(Driver).MoveToElement(Navigcur, Navigcur.Size.Width / 2, Navigcur.Size.Height / 2).Click().Build().Perform();
                    try { MenuClose = z3dvp.IMenuClose(); if (MenuClose.Displayed && MenuClose.Enabled) MenuClose.Click(); Thread.Sleep(1000); } catch { }
                    Thread.Sleep(1000);

                    IList<IWebElement> sysvolume_checkl = SysVolumes(BluRingZ3DViewerPage.Navigationone, values: true);
                    if (!sysvolume_checkl[0].Text.Contains(default_selvolcur))
                    {
                        bflagcur_3 = true;
                      
                    }
                    //  new Actions(Driver).MoveToElement(Navigcur, Navigcur.Size.Width / 2, Navigcur.Size.Height / 2).Click().Build().Perform();
                    try { MenuClose = z3dvp.IMenuClose(); if (MenuClose.Displayed && MenuClose.Enabled) MenuClose.Click(); Thread.Sleep(1000); } catch { }
                    Thread.Sleep(1000);
                }
                //curved step 4 Apply the tool operations zoom,w/l,measurement,sculpt etc.. on the images in navigation controls
                bool bflagcur_4 = false;

                List<string> Beforewindowcur_4 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                z3dvp.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.Navigationone);
                z3dvp.DragandDropelement(BluRingZ3DViewerPage.Navigationone);
                List<string> Afterwindowcur_4 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                Thread.Sleep(5000);
                z3dvp.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.Navigationone);
                List<string> BeforeZoomcur_4 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                z3dvp.DragandDropelement(BluRingZ3DViewerPage.Navigationone);
                List<string> AfterZoomcur_4 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (BeforeZoomcur_4[0] != AfterZoomcur_4[0] && Beforewindowcur_4[0] != Afterwindowcur_4[0])
                {
                    bflagcur_4 = true;
                }

                //curved   step 5 Again load the series that has been initially under sub-volumes list in step 1 (Load 1subvolume).
                bool bflagcur_5 = false;
                IList<IWebElement> sysvolumecur_5 = SysVolumes(BluRingZ3DViewerPage.Navigationone);
                if (sysvolumecur_5.Count >= 0)
                {
                    for (int i = 0; i <= sysvolumecur_5.Count; i++)
                    {
                        if (sysvolumecur_5[i].Text.Contains(default_selvolcur))
                        {
                            sysvolumecur_5[i].Click();
                            break;
                        }
                    }
                    //  new Actions(Driver).MoveToElement(Navigcur, Navigcur.Size.Width / 2, Navigcur.Size.Height / 2).Click().Build().Perform();
                    try { MenuClose = z3dvp.IMenuClose(); if (MenuClose.Displayed && MenuClose.Enabled) MenuClose.Click(); Thread.Sleep(1000); } catch { }
                    Thread.Sleep(1000);
                    IList<IWebElement> sysvolume5_selected = SysVolumes(BluRingZ3DViewerPage.Navigationone, true);
                    if (sysvolume5_selected[0].Text.Contains(default_selvolcur))
                    {
                        bflagcur_5 = true;
                    }
                }

                //curved step 6 Verfiy the rotation is preserverd 
                bool bflagcur_6 = false;
                //    new Actions(Driver).MoveToElement(Navigcur, Navigcur.Size.Width / 2, Navigcur.Size.Height / 2).Click().Build().Perform();
                try { MenuClose = z3dvp.IMenuClose(); if (MenuClose.Displayed && MenuClose.Enabled) MenuClose.Click(); Thread.Sleep(1000); } catch { }
                Thread.Sleep(1000);
                new Actions(Driver).SendKeys("T").Build().Perform();Thread.Sleep(1000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 65);
                Thread.Sleep(1000);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), removeCurserFromPage: true,pixelTolerance:400))
                {
                    bflagcur_6 = true;
                }
                new Actions(Driver).SendKeys("T").Build().Perform(); Thread.Sleep(1000);
                //curvedmpr step 7  Again load the  list in step3 (Load three subvolues).
                bool bflagcur_7 = false;
                List<string> VerfiyZoomcur_7 = null; 
                List<string> verfiywindowcur_7 = null;
                IList<IWebElement> sysvolumecur_7 = SysVolumes(BluRingZ3DViewerPage.Navigationone); Thread.Sleep(1000);
                if (sysvolumecur_7.Count >= 0)
                {
                    for (int i = 0; i < sysvolumecur_7.Count; i++)
                    {
                        if (sysvolumecur_7[i].Text.Contains(subonecur_3))
                        {
                            sysvolumecur_7[i].Click();
                            break;
                        }
                    }
                    try { MenuClose = z3dvp.IMenuClose(); if (MenuClose.Displayed && MenuClose.Enabled) MenuClose.Click(); Thread.Sleep(1000); } catch { }
                    VerfiyZoomcur_7 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                    verfiywindowcur_7 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                    IList<IWebElement> sysvolume7_check = SysVolumes(BluRingZ3DViewerPage.Navigationone, true); Thread.Sleep(1000);
                    if (sysvolume7_check.Count >= 0 && sysvolume7_check[0].Text.Contains(subonecur_3))
                    {
                        bflagcur_7 = true;

                    }
                }
                //   new Actions(Driver).MoveToElement(Inavigationone, Inavigationone.Size.Width / 2, Inavigationone.Size.Height / 2).Click().Build().Perform();
                try { MenuClose = z3dvp.IMenuClose(); if (MenuClose.Displayed && MenuClose.Enabled) MenuClose.Click(); Thread.Sleep(1000); } catch { }
                Thread.Sleep(1000);
                //curvedmpr //step 8 Verify the tool operations are preserved for the loaded volumes.
                bool bflagcur_8 = false;
                if (VerfiyZoomcur_7[0] ==AfterZoomcur_4[0] && verfiywindowcur_7[0] == Afterwindowcur_4[0])
                {
                    bflagcur_8 = true;
                }
                bool bstatuscurvedmpr = false;
                if(bflagcur_2 && bflagcur_3 && bflagcur_4 && bflagcur_5 && bflagcur_6 && bflagcur_7 && bflagcur_8)
                {
                    bstatuscurvedmpr = true;
                }
                if (bstatus4_1 && bstatus6_1 && bstatuscurvedmpr)
                {
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
                // login.Logout();
                //Return Result
                return result;
            }
            finally
            {
                z3dvp.CloseViewer();
                login.Logout();
                Driver.Close();
            }
        }


        public TestCaseResult Test_163378(String testid, String teststeps, int stepcount)
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
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try

            {
                //Step:1 - Navigate to 3D tab and Click MPR mode from the dropdown. Note: This is new design(could change)
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step1_1 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription);
                String TotalImage = Z3dViewerPage.GetSubVolumeImageCount(BluRingZ3DViewerPage.Navigationtwo, "Sub Volumes");
                if (step1_1 && TotalImage.Contains("17. 105 images"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:2 -  From the Universal viewer, Select a 3D supported cardiac phase CT series and Select the MPR view option from the smart view drop down.
                Z3dViewerPage.CloseViewer();
                Boolean step2_1 = Z3dViewerPage.searchandopenstudyin3D(PatientID2, ThumbnailDescription2);
                PageLoadWait.WaitForFrameLoad(10);
                TotalImage = Z3dViewerPage.GetSubVolumeImageCount(BluRingZ3DViewerPage.Navigationone, "Sub Volumes");
                if (step2_1 && TotalImage.Contains("8. 142 images"))
                {
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

        public TestCaseResult Test_163379(String testid, String teststeps, int stepcount)
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
            string PatientID3 = TestData[2];
            string ThumbnailDescription3 = TestData[3];
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try

            {
                //Step:1 - Images in the series is defined as a single volume
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step1_1 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription);
                IList<String> step1_2 = Z3dViewerPage.getAllMenuItems(BluRingZ3DViewerPage.Navigationone);
                if (step1_1 && step1_2.Count == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:2 -  Images in the series is defined as a Separate volume
                Z3dViewerPage.CloseViewer();
                Boolean step2_1 = Z3dViewerPage.searchandopenstudyin3D(PatientID2, ThumbnailDescription2);
                PageLoadWait.WaitForFrameLoad(10);
                IList<String> step2_2 = Z3dViewerPage.getAllMenuItems(BluRingZ3DViewerPage.Navigationone);
                if (step2_1 && step2_2.Count > 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:3 - nvalid images should not be included and user is able to notify the number of images that are not included under the sub-volume list from the viewport top bar
                Z3dViewerPage.CloseViewer();
                Boolean step3_1 = Z3dViewerPage.searchandopenstudyin3D(PatientID3, ThumbnailDescription3);
                PageLoadWait.WaitForFrameLoad(10);
                //IList<String> step3_2 = Z3dViewerPage.getAllMenuItems(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigationone);
                PageLoadWait.WaitForPageLoad(10);
               IList<IWebElement> SubVolume = Driver.FindElements(By.CssSelector(Locators.CssSelector.SubVolumebutton));
                ClickElement(SubVolume[0]);
                Thread.Sleep(2000);
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.subMenulayout)));
                IList<IWebElement> SubOptions = Driver.FindElements(By.CssSelector("div[class^='subvolumeDiscardedMessage ']"));
                if (step3_1 && SubOptions[0].Text.Contains("images not included in a volume"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                IWebElement ViewPort = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                IWebElement closeoptions = ViewPort.FindElement(By.CssSelector(Locators.CssSelector.menubutton));
                ClickElement(closeoptions);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.menutable)));


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

        public TestCaseResult Test_163380(String testid, String teststeps, int stepcount)
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
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try

            {
                //Step:1 - Series is loaded in the 3D viewer in MPR 4:1 viewing mode
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step1 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription, field: "acc");
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

                //step:2 - Slices are separated in Multiple sub series are listed.
                IList<String> step2 = Z3dViewerPage.getAllMenuItems(BluRingZ3DViewerPage.Navigationone);
                if (step2.Count > 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:3 -Select any of the Sub series from the list under the folder icon.
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, "1000 images", "Sub Volumes");
                Boolean step3 = Z3dViewerPage.checkerrormsg();
                if (!step3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:4 - Select and load all the sub series one by one.
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, "4000 images", "Sub Volumes");
                Boolean step4 = Z3dViewerPage.checkerrormsg();
                if (!step4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:5 - Launch any study that has volume containing more than 12000 slices in 3D viewer
                // User will be notified in the 3D UI
                Z3dViewerPage.CloseViewer();
                Z3dViewerPage.searchandopenstudyin3D(PatientID2, ThumbnailDescription2, field: "acc");
                PageLoadWait.WaitForFrameLoad(10);
                Boolean step5_1 = Z3dViewerPage.PopwindowwarnMsg().Text.Equals("Maximum number of supported 3D Slices has been exceeded by this request.");
                Boolean step5_2 = Z3dViewerPage.checkerrormsg("y");
                if (step5_1 && step5_2)
                {
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
        public static IList<IWebElement> SysVolumes(string controlname, bool values = false, bool four_one = false)
        {
            IList<IWebElement> sysvolumes =new List<IWebElement>();
            IWebElement ithumnail = Driver.FindElement(By.CssSelector(Locators.CssSelector.thumbclick));
            try
            {
                BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
                IWebElement Navigationone = z3dvp.controlelement(controlname);
                IList<IWebElement> IMenubutton = Driver.FindElements(By.CssSelector(Locators.CssSelector.IMenubutton));
                if(IMenubutton.Count>0)
                {
                    if(four_one) z3dvp.ClickElement(IMenubutton[3]);
                    else z3dvp.ClickElement(IMenubutton[0]);
                    Thread.Sleep(1000);
                    wait.Until(ExpectedConditions.ElementExists(By.CssSelector(Locators.CssSelector.IMenutable)));
                    IList<IWebElement> subvoludis = Driver.FindElements(By.CssSelector(Locators.CssSelector.IMenutable));
                    Thread.Sleep(1000);
                    for(int i=0;i<subvoludis.Count;i++)
                    {
                        if (subvoludis[i].Text.Contains(BluRingZ3DViewerPage.SubVolumes))
                        {
                            subvoludis[i + 1].Click();
                            Thread.Sleep(2000);
                            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                            Thread.Sleep(1000);
                            if (values)
                            {
                                sysvolumes.Add(subvoludis[i+1]);
                                Thread.Sleep(1000);
                                break;
                            }
                            else
                            {
                                sysvolumes = Driver.FindElements(By.CssSelector("div[class='submenuItem ng-star-inserted']>div"));
                                Thread.Sleep(1000);
                                break;
                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog(e.Message);
            }

            return sysvolumes;
        }
    }
}

