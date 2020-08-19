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
using TestStack.White.InputDevices;
using TestStack.White.WindowsAPI;

namespace Selenium.Scripts.Tests
{
    class Z3D_All_Controls : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public DomainManagement domain { get; set; }
        public StudyViewer studyviewer { get; set; }
        public Cursor Cursor { get; private set; }

        public Z3D_All_Controls(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            domain = new DomainManagement();
           
            studyviewer = new StudyViewer();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }

        public TestCaseResult Test_163248(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmgmt;
            UserManagement usermgmt;
            BasePage basepage = new BasePage();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            BluRingViewer bluringviewer = new BluRingViewer();
            Imager imager = new Imager();
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
                //bool StudyLoad = brz3dvp.searchandopenstudyin3D("FFP", objthumbimg, BluRingZ3DViewerPage.MPR, field: "acc");
                bool StudyLoad = brz3dvp.searchandopenstudyin3D(objpatid, objthumbimg, BluRingZ3DViewerPage.MPR);
                if (StudyLoad)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    throw new Exception("unable to launch study in 3D Viewer");
                }
                new Actions(Driver).SendKeys("X").Build().Perform();
                //Step 2::Note that the MPR View is displayed as a 2 x 2 Grid view with 3 MPR navigation controls and 1 MPR result control.
                IWebElement navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                IWebElement navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                IWebElement navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                IWebElement ResultPanel = brz3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                //Verification::In the MPR View, make sure that from left to right and from top to bottom, the controls are labeled as follows:
                int nav1locX = navigation1.Location.X;
                int nav1locY = navigation1.Location.Y;
                int nav2locX = navigation2.Location.X;
                int nav2locY = navigation2.Location.Y;
                int nav3locX = navigation3.Location.X;
                int nav3locY = navigation3.Location.Y;
                int ResultlocX = ResultPanel.Location.X;
                int ResultlocY = ResultPanel.Location.Y;
                if(nav1locX == nav3locX && nav2locX == ResultlocX && nav1locY == nav2locY && nav3locY == ResultlocY)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 3::By default, Study information and Annotations are Toggled ON in settings.
                string LeftTopText = navigation1.FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftTop)).Text;
                string RightTopText = navigation1.FindElement(By.CssSelector(Locators.CssSelector.AnnotationRightTop)).Text;
                //Verification::Study information and Annotations are displayed in image.
                if(LeftTopText !=null && RightTopText != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 4::ight click on the image and select download Image option from the 3D tool box.
                bool DownloadTool = brz3dvp.select3DTools(Z3DTools.Download_Image);
                if (DownloadTool)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 5::From any one of the controls, Save the Image to local drive.
                String imagename = testid + "_5";
                String Step5_imgLocation = Config.downloadpath + "\\" + imagename + ".jpg";
                if (File.Exists(Step5_imgLocation))
                    File.Delete(Step5_imgLocation);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(2);
                brz3dvp.downloadImageForViewport(imagename, "jpg");
                brz3dvp.CloseDownloadInfobar();
                PageLoadWait.WaitForFrameLoad(2);
                bool CompareDownloadImage = brz3dvp.CompareDownloadimage(Step5_imgLocation);
                if (CompareDownloadImage)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 6:: From smart view drop down select the MPR 4:1 option from the drop down.
                bool Layout4x1 = brz3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                if (Layout4x1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 7::Click on Show/Hide drop down and select Hide image text.
                bool AnnotationOFF = bluringviewer.SelectShowHideValue(BluRingZ3DViewerPage.HideText);
                String LeftMprNav1 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (AnnotationOFF && LeftMprNav1 == null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 8::Right click on the image and select download Image option from the 3D tool box..
                DownloadTool = brz3dvp.select3DTools(Z3DTools.Download_Image,BluRingZ3DViewerPage.Navigation3D1);
                if (DownloadTool)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 9::From any one of the controls, Save the Image to local drive.
                imagename = testid + "_9";
                String Step9_imgLocation = Config.downloadpath + "\\" + imagename + ".jpg";
                if (File.Exists(Step9_imgLocation))
                    File.Delete(Step9_imgLocation);
                IWebElement nav1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
             //   brz3dvp.MoveAndClick(nav1, (nav1.Size.Width / 12), (nav1.Size.Height / 2));
                new Actions(Driver).MoveToElement(nav1 , (nav1.Size.Width/12) , (nav1.Size.Height/2)).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(2);
                brz3dvp.downloadImageForViewport(imagename, "jpg");
                brz3dvp.CloseDownloadInfobar();
                PageLoadWait.WaitForFrameLoad(2);
                CompareDownloadImage = brz3dvp.CompareDownloadimage(Step9_imgLocation);
                if (CompareDownloadImage)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 10::From smart view drop down select the 3D 6:1 option from the drop down.
                bool Layout6x1 = brz3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                if (Layout6x1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 11::Click on Show/Hide drop down and select show image text.
                bool AnnotationON = bluringviewer.SelectShowHideValue(BluRingZ3DViewerPage.ShowText);
                //Verification::Study information and Annotations are Toggled ON.
                LeftMprNav1 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (AnnotationON && LeftMprNav1!= null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 12::Draw some annotation on the image in any controls.
                bool LineMeasurement = brz3dvp.select3DTools(Z3DTools.Line_Measurement);
                navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                brz3dvp.Performdragdrop(navigation1, navigation1.Size.Width / 2, navigation1.Size.Height / 2 , navigation1.Size.Width / 10, navigation1.Size.Height / 2);
                //Actions step12 = new Actions(Driver);
                //step12.MoveToElement(navigation1, navigation1.Size.Width / 10, navigation1.Size.Height / 2).ClickAndHold().
                //    MoveToElement(navigation1, navigation1.Size.Width / 2, navigation1.Size.Height / 2).Build().Perform();
                //PageLoadWait.WaitForFrameLoad(5);
                //step12.Release().Build().Perform();
                //Verification::Study information and Annotations are displayed in image.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], navigation1, removeCurserFromPage:true))
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
                //Step 13::Right click on the image and select download Image option from the 3D tool box.
                DownloadTool = brz3dvp.select3DTools(Z3DTools.Download_Image);
                if (DownloadTool)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 14::From any one of the controls, Save the Image to local drive.
                imagename = testid + "_14";
                String Step14_imgLocation = Config.downloadpath + "\\" + imagename + ".jpg";
                if (File.Exists(Step14_imgLocation))
                    File.Delete(Step14_imgLocation);
                nav1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
               // brz3dvp.MoveAndClick(nav1, (nav1.Size.Width / 12), (nav1.Size.Height / 4));
                new Actions(Driver).MoveToElement(nav1, (nav1.Size.Width / 12), (nav1.Size.Height / 4)).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(2);
                brz3dvp.downloadImageForViewport(imagename, "jpg");
               // brz3dvp.CloseDownloadInfobar();
                PageLoadWait.WaitForFrameLoad(2);
                CompareDownloadImage = brz3dvp.CompareDownloadimage(Step14_imgLocation);
                if (CompareDownloadImage)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 15::From smart view drop down select the Curved MPR option from the drop down.
                bool CurvedMpr = brz3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                if (CurvedMpr)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 16::Click on Show/Hide drop down and select Hide image text.
                AnnotationOFF = bluringviewer.SelectShowHideValue(BluRingZ3DViewerPage.HideText);
                //Verification::Study information and Annotations are Toggled OFF.
                LeftMprNav1 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (AnnotationOFF && LeftMprNav1 == null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 17::Right click on the image and select download Image option from the 3D tool box.
                DownloadTool = brz3dvp.select3DTools(Z3DTools.Download_Image);
                if (DownloadTool)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 18::From any one of the controls, Save the Image to local drive.
                Logger.Instance.InfoLog("imagelocalerror");
                imagename = testid + "_18";
                String Step18_imgLocation = Config.downloadpath + "\\" + imagename + ".jpg";
                if (File.Exists(Step18_imgLocation))
                    File.Delete(Step18_imgLocation);
                nav1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                brz3dvp.MoveAndClick(nav1, (nav1.Size.Width / 12), (nav1.Size.Height / 4));
                new Actions(Driver).MoveToElement(nav1, (nav1.Size.Width / 12), (nav1.Size.Height / 4)).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(2);
                brz3dvp.downloadImageForViewport(imagename, "jpg");
               // brz3dvp.CloseDownloadInfobar();
                PageLoadWait.WaitForFrameLoad(2);
                CompareDownloadImage = brz3dvp.CompareDownloadimage(Step18_imgLocation);
                if (CompareDownloadImage)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 19::Close the viewer and select calcium scoring supported study in Universal viewer.
                result.steps[++ExecutedSteps].status = "Pass";
                //Step 20::From the Universal viewer , Select a 3D supported series and Select the Calcium scoring option from the drop down
                bool CalciumScoring = brz3dvp.select3dlayout(BluRingZ3DViewerPage.CalciumScoring);
                if (CalciumScoring)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                Thread.Sleep(5000);
                if (IsElementPresent(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox)))
                {
                    new Actions(Driver).MoveToElement(brz3dvp.CloseSelectedToolBox()).Click().Build().Perform();
                        Thread.Sleep(3000);
                    
                }
                Logger.Instance.InfoLog("Closing the selected tool Box");
                //Step 21::Apply W/L on image.
                bluringviewer.SelectShowHideValue(BluRingZ3DViewerPage.ShowText);
                bool WindowLevel = brz3dvp.select3DTools(Z3DTools.Window_Level , BluRingZ3DViewerPage.CalciumScoring);
                IWebElement Calcium = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                string BeforeWL = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.CalciumScoring);
                Thread.Sleep(3000);
                brz3dvp.Performdragdrop(Calcium, Calcium.Size.Width / 2, Calcium.Size.Height / 2 , Calcium.Size.Width / 2, Calcium.Size.Height / 10);
                //Actions step21 = new Actions(Driver);
                //step21.MoveToElement(Calcium, Calcium.Size.Width /2, Calcium.Size.Height /10).ClickAndHold().
                //       MoveToElement(Calcium, Calcium.Size.Width /2, Calcium.Size.Height /2).Build().Perform();
                //PageLoadWait.WaitForFrameLoad(5);
                //step21.Release().Build().Perform();
                string AfterWL = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.CalciumScoring);
                bluringviewer.SelectShowHideValue(BluRingZ3DViewerPage.HideText);
                Logger.Instance.InfoLog("WIndow level before  : " + BeforeWL + " Window level after apply :" + AfterWL);
                if (WindowLevel && BeforeWL != AfterWL)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 22 ::Right click on the image and select download Image option from the 3D tool box.
                DownloadTool = brz3dvp.select3DTools(Z3DTools.Download_Image , BluRingZ3DViewerPage.CalciumScoring);
                if (DownloadTool)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 23::Save the Image to local drive.
                imagename = testid + "_23";
                String Step23_imgLocation = Config.downloadpath + "\\" + imagename + ".jpg";
                if (File.Exists(Step23_imgLocation))
                    File.Delete(Step23_imgLocation);
                Calcium = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
              //  brz3dvp.MoveAndClick(Calcium, (Calcium.Size.Width / 12), (Calcium.Size.Height / 2));
                new Actions(Driver).MoveToElement(Calcium, (Calcium.Size.Width / 12), (Calcium.Size.Height / 2)).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(2);
                brz3dvp.downloadImageForViewport(imagename, "jpg");
                brz3dvp.CloseDownloadInfobar();
                PageLoadWait.WaitForFrameLoad(2);
                CompareDownloadImage = brz3dvp.CompareDownloadimage(Step23_imgLocation);
                if (CompareDownloadImage)
                {
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

        public TestCaseResult Test_163251(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result;
            BasePage basepage = new BasePage();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            BluRingViewer bluringviewer = new BluRingViewer();
            Imager imager = new Imager();
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
                string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
                string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
                //String[] TestData = TestDataRequirements.Split('|');
               


                //step 01 :: Search and load a 3D supported study in the universal viewer.
                login.LoginIConnect(adminUserName, adminPassword);
                bool StudyLoad = brz3dvp.searchandopenstudyin3D(PatientID, ThumbnailDescription, BluRingZ3DViewerPage.MPR, field: "acc", thumbimgoptional: TestDataRequirements);
                if (StudyLoad)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    throw new Exception("unable to launch study in 3D Viewer");
                }
                new Actions(Driver).SendKeys("X").Build().Perform();
                //Step 2::Double click on one of the control's of the MPR view
                IWebElement navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                DoubleClick(navigation1);
                PageLoadWait.WaitForFrameLoad(10);
                IWebElement navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                IWebElement navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                IWebElement ResultPanel = brz3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                Size nav1 = navigation1.Size;
                Size nav2 = navigation2.Size;
                Size nav3 = navigation3.Size;
                //Verification::View is switched to OneUp mode
                if(nav1.Height>nav2.Height && nav1.Width > nav2.Width && (nav2.Height == nav3.Height || nav2.Height == nav3.Height-1) && nav2.Width == nav3.Width)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 3::Right click on the image and select download Image option from the 3D tool box
                bool DownloadTool = brz3dvp.select3DTools(Z3DTools.Download_Image,BluRingZ3DViewerPage.Navigationone );
                if (DownloadTool)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 4::Save the Image to the local drive
                String imagename = testid + "_4";
                bool compare = brz3dvp.VerifyImageHeightandWidth(navigation1 , imagename ,  BluRingZ3DViewerPage.Navigationone);
                if (compare)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 5::Resize the Z3D viewer's browser window
                Size BeforeSize = Driver.Manage().Window.Size;
                PageLoadWait.WaitForFrameLoad(10);
                Driver.Manage().Window.Size = new Size(950, 750);
                PageLoadWait.WaitForFrameLoad(10);
                Size AfterSize = Driver.Manage().Window.Size;
                navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                nav1 = navigation1.Size;
                nav2 = navigation2.Size;
                nav3 = navigation3.Size;
                int Location1x = navigation1.Location.X;
                int Location1y = navigation1.Location.Y;
                int Location2x = navigation2.Location.X;
                int Location2y = navigation2.Location.Y;
                //Verification::View is switched to OneUp mode
                if (nav1.Height > nav2.Height && nav1.Width > nav2.Width && nav2.Height == nav3.Height || nav2.Height+1 == nav3.Height && nav2.Width == nav3.Width
                    && BeforeSize!=AfterSize && Location1x > Location2x && Location1y == Location2y)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 6::Right click on the image and select download Image option from the 3D tool box.
                DownloadTool = brz3dvp.select3DTools(Z3DTools.Download_Image);
                if (DownloadTool)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 7::Save the Image to the local drive
                imagename = testid + "_7";
                compare = brz3dvp.VerifyImageHeightandWidth(navigation1, imagename ,   BluRingZ3DViewerPage.Navigationone);
                if (compare)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 8 ::Repeat steps 3-8 for the following view modes :
                //3D view
                Driver.Manage().Window.Maximize();
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                //DoubleClick(navigation1);
                //PageLoadWait.WaitForFrameLoad(10);
                //navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                //navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                //navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                //nav1 = navigation1.Size;
                //nav2 = navigation2.Size;
                //nav3 = navigation3.Size;
                bool res8_1 = brz3dvp.EnableOneViewupMode(navigation1);
                PageLoadWait.WaitForFrameLoad(5);
                DownloadTool = brz3dvp.select3DTools(Z3DTools.Download_Image, BluRingZ3DViewerPage.Navigation3D1);//Step 3
                imagename = testid + "_81";
                bool Step8_1 = brz3dvp.VerifyImageHeightandWidth(navigation1, imagename, BluRingZ3DViewerPage.Navigationone);//Step 4
                BeforeSize = Driver.Manage().Window.Size;//Step 5
                PageLoadWait.WaitForFrameLoad(10);
                Driver.Manage().Window.Size = new Size(950, 750);
                PageLoadWait.WaitForFrameLoad(10);
                AfterSize = Driver.Manage().Window.Size;
                //navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                //navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                //navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                //Size nav11 = navigation1.Size;
                //Size nav22 = navigation2.Size;
                //Size nav33 = navigation3.Size;
                bool DownloadTools = brz3dvp.select3DTools(Z3DTools.Download_Image,BluRingZ3DViewerPage.Navigation3D1);//Step 6
                imagename = testid + "_82";
                bool Step8_11 = brz3dvp.VerifyImageHeightandWidth(navigation1, imagename, BluRingZ3DViewerPage.Navigationone);
                bool Steps1 = false;
                //if (nav1.Height > nav2.Height && nav1.Width > nav2.Width &&( nav2.Height == nav3.Height|| nav2.Height == nav3.Height-1) && nav2.Width == nav3.Width && DownloadTool
                //    && nav11.Height > nav22.Height && nav11.Width > nav22.Width && nav22.Height == nav33.Height|| nav22.Height+1 == nav33.Height && nav22.Width == nav33.Width
                //    && BeforeSize != AfterSize && DownloadTools && Step8_11 && Step8_1)
                if(res8_1 && DownloadTool && (AfterSize != BeforeSize) && Step8_1 && Step8_11 && DownloadTools)
                {
                    Steps1 = true;
                    Logger.Instance.InfoLog("The result of steps1 in step 8 : " + Steps1.ToString());
                }
                //SixUp view
                Driver.Manage().Window.Maximize();
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                brz3dvp.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.Navigationone);
                navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                //DoubleClick(navigation1);
                bool res8_2 = brz3dvp.EnableOneViewupMode(navigation1);
                PageLoadWait.WaitForFrameLoad(5);
                navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                //navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                //navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                //nav1 = navigation1.Size;
                //nav2 = navigation2.Size;
                //nav3 = navigation3.Size;
                DownloadTool = brz3dvp.select3DTools(Z3DTools.Download_Image, BluRingZ3DViewerPage.Navigationone);//Step 3
                imagename = testid + "_83";
                bool Step8_2 = brz3dvp.VerifyImageHeightandWidth(navigation1, imagename, BluRingZ3DViewerPage.Navigationone);//Step 4
                BeforeSize = Driver.Manage().Window.Size;//Step 5
                PageLoadWait.WaitForFrameLoad(10);
                Driver.Manage().Window.Size = new Size(950, 750);
                PageLoadWait.WaitForFrameLoad(10);
                AfterSize = Driver.Manage().Window.Size;
                //nav11 = navigation1.Size;
                //nav22 = navigation2.Size;
                //nav33 = navigation3.Size;
                DownloadTools = brz3dvp.select3DTools(Z3DTools.Download_Image, BluRingZ3DViewerPage.Navigationone);//Step 6
                imagename = testid + "_84";
                bool Step8_22 = brz3dvp.VerifyImageHeightandWidth(navigation1, imagename, BluRingZ3DViewerPage.Navigationone);
                bool Steps2 = false;
                //if (nav1.Height > nav2.Height && nav1.Width > nav2.Width && nav2.Height == nav3.Height && nav2.Width == nav3.Width && DownloadTool
                //    && nav11.Height > nav22.Height && nav11.Width > nav22.Width && nav22.Height == nav33.Height && nav22.Width == nav33.Width
                //    && BeforeSize != AfterSize && DownloadTools && Step8_22 && Step8_1)
                if(res8_2 && DownloadTool && DownloadTools && Step8_2 && Step8_22 && (BeforeSize != AfterSize))
                {
                    Steps2 = true;
                    Logger.Instance.InfoLog("The result of steps2 in step 8 : " + Steps2.ToString());
                }
                //Curved MPR view
                Driver.Manage().Window.Maximize();
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.Navigationone);
                PageLoadWait.WaitForFrameLoad(5);
                navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                //DoubleClick(navigation1);
                bool res8_3 = brz3dvp.EnableOneViewupMode(navigation1);
                PageLoadWait.WaitForFrameLoad(5);
                navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                //navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                //navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                //nav1 = navigation1.Size;
                //nav2 = navigation2.Size;
                //nav3 = navigation3.Size;
                DownloadTool = brz3dvp.select3DTools(Z3DTools.Download_Image);//Step 3
                imagename = testid + "_85";
                bool Step8_3 = brz3dvp.VerifyImageHeightandWidth(navigation1, imagename, BluRingZ3DViewerPage.Navigationone);//Step 4
                BeforeSize = Driver.Manage().Window.Size;//Step 5
                PageLoadWait.WaitForFrameLoad(10);
                Driver.Manage().Window.Size = new Size(950, 750);
                PageLoadWait.WaitForFrameLoad(10);
                AfterSize = Driver.Manage().Window.Size;
                //nav11 = navigation1.Size;
                //nav22 = navigation2.Size;
                //nav33 = navigation3.Size;
                DownloadTools = brz3dvp.select3DTools(Z3DTools.Download_Image, BluRingZ3DViewerPage.Navigationone);//Step 6
                imagename = testid + "_86";
                bool Step8_33 = brz3dvp.VerifyImageHeightandWidth(navigation1, imagename, BluRingZ3DViewerPage.Navigationone);
                bool Steps3 = false;
                //if (nav1.Height > nav2.Height && nav1.Width > nav2.Width && nav2.Height == nav3.Height && nav2.Width == nav3.Width && DownloadTool
                //    && nav11.Height > nav22.Height && nav11.Width > nav22.Width && nav22.Height == nav33.Height && nav22.Width == nav33.Width
                //    && BeforeSize != AfterSize && DownloadTools && Step8_22 && Step8_1)
                if(res8_2 && (BeforeSize != AfterSize) && Step8_3 && Step8_33 && DownloadTool && DownloadTools)
                {
                    Steps3 = true;
                    Logger.Instance.InfoLog("The result of steps3 in step 8 : " + Steps3.ToString());
                }
                if (Steps1 && Steps2 && Steps3)
                {
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
                Driver.Manage().Window.Maximize();
                brz3dvp.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_163249(String testid, String teststeps, int stepcount)
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

            string slocationvalue = ssplit[0];
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;

            try
            {
                login.LoginIConnect(username, password);
                //Step 1  & step 2   From iCA, Load a study in Z3D viewer. By default MPR viewing mode should dipslay From the universal viewer, Select and load the series in 3D viewer.
                z3dvp.Deletefiles(testcasefolder);
                bool res = z3dvp.searchandopenstudyin3D(Patientid, thumbnailcaption);
                if (!res)
                    throw new Exception("Unable to open study due to exception ");
                else
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                }
                //Step 3     Select the Download option from the 3D tool box. 
                bool btool3 = z3dvp.select3DTools(Z3DTools.Download_Image, BluRingZ3DViewerPage.Navigationone);
                if (btool3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 4 Left click on the mouse in any one of the Controls.
                new Actions(Driver).SendKeys("x").Build().Perform();
                IWebElement Inavigation1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                //for screen shots 
                bool bflag4 = false;
                new Actions(Driver).MoveToElement(Inavigation1, Inavigation1.Size.Width/2 -20,Inavigation1.Size.Height-100).ClickAndHold().Release().Build().Perform();
                //new Actions(Driver).DragAndDropToOffset(Inavigation1, Inavigation1.Size.Width / 2 - 50, Inavigation1.Size.Height - 100).Release().Build().Perform();
                Thread.Sleep(2000);
                wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.CssSelector(Locators.CssSelector.saveimgdialg)));
                IList<IWebElement> ItoolJPGPNG = z3dvp.DownloadJPGPNG();
                if (ItoolJPGPNG.Count >= 1 && ItoolJPGPNG[0].Text.ToUpper() == "JPG" && ItoolJPGPNG[1].Text.ToUpper() == "PNG")
                {
                    bflag4 = true;
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    IWebElement Iclose = z3dvp.CloseSelectedToolBox();
                    if (Iclose.Displayed)
                    {
                                ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", Iclose);
                        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("Locators.CssSelector.CloseSelectedToolBox")));
                    }
                }
                String filename4 = "Filename4_" + testid + ".jpg";
                
                Thread.Sleep(1000);
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(1000);
                DownloadImageFile(Inavigation1, testcasefolder + Path.DirectorySeparatorChar + filename4);
                if (bflag4 == false)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //STep 5 From the window, Enter the File name and Select the image type as 'JPEG' or 'PNG'
                //If the volume is compressed, only JPEG option should be available.
                bool bstatusflag = false;
                bool sstatus5 = z3dvp.MPRQuality("20", "Left");
                if (sstatus5)
                {
                    new Actions(Driver).MoveToElement(Inavigation1, Inavigation1.Size.Width/2 - 20, Inavigation1.Size.Height - 100).ClickAndHold().Release().Build().Perform();
                   // new Actions(Driver).DragAndDropToOffset(Inavigation1, Inavigation1.Size.Width / 2 - 10, Inavigation1.Size.Height - 100).Build().Perform();
                    Thread.Sleep(2000);
                    wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.CssSelector(Locators.CssSelector.saveimgdialg)));
                    IWebElement ItoolPNG = z3dvp.PNGDisbled();
                    if (ItoolPNG.Enabled)
                    {
                        IWebElement Iclose = z3dvp.CloseSelectedToolBox();
                        if (Iclose.Displayed)
                        {
                            // Iclose.Click();
                            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", Iclose);
                            bstatusflag = true;
                            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("Locators.CssSelector.CloseSelectedToolBox")));
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        Thread.Sleep(1000);
                    }
                }
                if (bstatusflag == false)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //REvert the changed from the 3dSetting
                bool srevert5 = z3dvp.MPRQuality("100", "Right");

                //step 6 Click the Save button to save the Image to local drive.
                string filename5 = "download_163249";
             //   new Actions(Driver).DragAndDropToOffset(Inavigation1, Inavigation1.Size.Width / 2 - 5, Inavigation1.Size.Height - 100).Build().Perform();
                new Actions(Driver).MoveToElement(Inavigation1, Inavigation1.Size.Width /2- 5, Inavigation1.Size.Height - 100).Click().Build().Perform();
                Thread.Sleep(1000);
                z3dvp.downloadImageForViewport(filename5, "jpg");
                if (File.Exists(Config.downloadpath + Path.DirectorySeparatorChar + filename5 + ".jpg"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 7 Open the saved image from the local drive and compare with the Screenshot from step 4.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Inavigation1))
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

                //Repeat the Steps 3-7 on all the controls of 3D viewing Mode.
                bool res7 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4, "n");
                if (res7 == true)
                {
                    Logger.Instance.InfoLog("Successfully OPend the 4:1 Layout ");
                }
                else
                {
                    Logger.Instance.InfoLog("Failed to open the threeD 4:1 Layout ");
                }

                // step 3 for threeDlayout Select the Download option from the 3D tool box threeD Layout 
                bool btool2_3 = z3dvp.select3DTools(Z3DTools.Download_Image, BluRingZ3DViewerPage.Navigation3D1);
                bool bflag2_3 = false;
                if (btool2_3)
                {
                    bflag2_3 = true;
                }
                else
                {
                    Logger.Instance.InfoLog("Failed to select the downloadtool");
                }

                //step 2_4 Left click on the mouse in any one of the Controls. for 3D tool 
                IWebElement Inavigation2_4 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                //for screen shots 
                bool bflag2_4 = false;
                                new Actions(Driver).MoveToElement(Inavigation2_4, Inavigation2_4.Size.Width/2-5, Inavigation2_4.Size.Height-100).ClickAndHold().Release().Build().Perform();
              //  new Actions(Driver).DragAndDropToOffset(Inavigation2_4, Inavigation2_4.Size.Width / 2 - 5, Inavigation2_4.Size.Height - 100).Build().Perform();
                Thread.Sleep(2000);
                wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.CssSelector(Locators.CssSelector.saveimgdialg)));
                IList<IWebElement> ItoolJPGPNG2_4 = z3dvp.DownloadJPGPNG();
                if (ItoolJPGPNG2_4.Count >= 1 && ItoolJPGPNG2_4[0].Text.ToUpper() == "JPG" && ItoolJPGPNG2_4[1].Text.ToUpper() == "PNG")
                {
                    bflag2_4 = true;
                    IWebElement Iclose = z3dvp.CloseSelectedToolBox();
                    if (Iclose.Displayed)
                    {
                        //  Iclose.Click();
                        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", Iclose);
                        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("Locators.CssSelector.CloseSelectedToolBox")));
                    }
                }

                string filename2_14 = "download163249_4_2";
                DownloadImageFile(Inavigation2_4, testcasefolder + Path.DirectorySeparatorChar + filename2_14);
                if (bflag2_4 == false)
                {
                    Logger.Instance.InfoLog("ThreeD4:1 step 2_4 is fiald");
                }


                //step 2_5 From the window, Enter the File name and Select the image type as 'JPEG' or 'PNG' 
                //If the volume is compressed, only JPEG option should be available.
                bool bstatusflag2_5 = false;
                bool sstatus2_5 = z3dvp.MPRQuality("20", "Left");
                if (sstatus2_5)
                {
                                       new Actions(Driver).MoveToElement(Inavigation2_4, Inavigation2_4.Size.Width/2 - 5, Inavigation2_4.Size.Height - 100).ClickAndHold().Release().Build().Perform();
                     //       new Actions(Driver).DragAndDropToOffset(Inavigation2_4, Inavigation2_4.Size.Width / 2 - 5, Inavigation2_4.Size.Height - 100).Build().Perform();
                    Thread.Sleep(2000);
                    wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.CssSelector(Locators.CssSelector.saveimgdialg)));
                    IWebElement ItoolPNG = z3dvp.PNGDisbled();
                    if (ItoolPNG.Enabled)
                    {
                        IWebElement Iclose = z3dvp.CloseSelectedToolBox();
                        if (Iclose.Displayed)
                        {
                            // Iclose.Click();
                            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", Iclose);
                            bstatusflag2_5 = true;
                            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("Locators.CssSelector.CloseSelectedToolBox")));
                            bstatusflag2_5 = true;
                        }
                        Thread.Sleep(1000);
                    }
                }
                if (bstatusflag2_5 == false)
                {
                    Logger.Instance.InfoLog("Failed 2_5 step ");
                }
                //REvert the changed from the 3dSetting
                bool srevert2_5 = z3dvp.MPRQuality("100", "Right");

                //Step 2_6 Click the Save button to save the Image to local drive.
                string filename2_6 = "download_163249_2_6";
                bool bflag2_6 = false;
                 new Actions(Driver).MoveToElement(Inavigation2_4, Inavigation2_4.Size.Width/2 - 5, Inavigation2_4.Size.Height /2- 100).Click().Build().Perform();
                //new Actions(Driver).DragAndDropToOffset(Inavigation2_4, Inavigation2_4.Size.Width / 2 - 5, Inavigation2_4.Size.Height - 100).Build().Perform();
                Thread.Sleep(1000);
                z3dvp.downloadImageForViewport(filename2_6, "jpg");
                PageLoadWait.WaitForFrameLoad(10);
                if (File.Exists(Config.downloadpath + Path.DirectorySeparatorChar + filename2_6 + ".jpg"))
                {
                    bflag2_6 = true;
                }
                else
                {
                    Logger.Instance.InfoLog("Failed in step 2_6");
                }

                //Step 7 Open the saved image from the local drive and compare with the Screenshot from step 4.
                bool bflag2_7 = false;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Inavigation2_4))
                {
                    bflag2_7 = true;
                }
                else
                {
                    Logger.Instance.InfoLog("Failed in Step 2_7");
                }
                if (bflag2_3 && bflag2_4 && bstatusflag2_5 && bflag2_6 && bflag2_7)
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

                //Repeat the Steps 3-7 on all the controls of Six Up viewing Mode.
                bool res3_7 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6, "n");
                if (res3_7 == true)
                {
                    Logger.Instance.InfoLog("Successfully OPend the 4:1 Layout ");
                }
                else
                {
                    Logger.Instance.InfoLog("Failed to open the threeD 4:1 Layout ");
                }


                //step 3_3 Select the download button fromt he 3dtool threed6 view
                bool btool3_3 = z3dvp.select3DTools(Z3DTools.Download_Image, BluRingZ3DViewerPage.Navigationone);
                bool bflag3_3 = false;
                if (btool3_3)
                {
                    bflag3_3 = true;
                }
                else
                {
                    Logger.Instance.InfoLog("Failed to select the downloadtool step 3_3");
                }

                //step 3_4 Left click on the mouse in any one of the Controls. for 3D tool 
                IWebElement Inavigation3_4 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                //for screen shots 
                bool bflag3_4 = false;
                new Actions(Driver).MoveToElement(Inavigation3_4, Inavigation3_4.Size.Width/2 - 5, Inavigation3_4.Size.Height - 100).ClickAndHold().Release().Build().Perform();
            //    new Actions(Driver).DragAndDropToOffset(Inavigation3_4, Inavigation3_4.Size.Width / 2 - 5, Inavigation3_4.Size.Height - 100).Build().Perform();
                Thread.Sleep(2000);
                wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.CssSelector(Locators.CssSelector.saveimgdialg)));
                IList<IWebElement> ItoolJPGPNG3_4 = z3dvp.DownloadJPGPNG();
                if (ItoolJPGPNG3_4.Count >= 1 && ItoolJPGPNG3_4[0].Text.ToUpper() == "JPG" && ItoolJPGPNG3_4[1].Text.ToUpper() == "PNG")
                {
                    bflag3_4 = true;
                    IWebElement Iclose = z3dvp.CloseSelectedToolBox();
                    if (Iclose.Displayed)
                    {
                        // Iclose.Click();
                        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", Iclose);
                        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("Locators.CssSelector.CloseSelectedToolBox")));
                    }
                }

                string filename3_4 = "download163249_3_4";
                DownloadImageFile(Inavigation3_4, testcasefolder + Path.DirectorySeparatorChar + filename3_4);
                if (bflag3_4 == false)
                {
                    Logger.Instance.InfoLog("ThreeD4:1 step 2_4 is fiald");
                }

                
                //step 3_5 From the window, Enter the File name and Select the image type as 'JPEG' or 'PNG' 
                //If the volume is compressed, only JPEG option should be available.
                bool bstatusflag3_5 = false;
                bool sstatus3_5 = z3dvp.MPRQuality("20", "Left");
                PageLoadWait.WaitForFrameLoad(10);
                if (sstatus3_5)
                {
                    new Actions(Driver).MoveToElement(Inavigation3_4, Inavigation3_4.Size.Width/2 - 5, Inavigation3_4.Size.Height - 100).ClickAndHold().Release().Build().Perform();
                    //new Actions(Driver).DragAndDropToOffset(Inavigation3_4, Inavigation3_4.Size.Width / 2 - 5, Inavigation3_4.Size.Height - 100).Build().Perform();
                    Thread.Sleep(1000);
                    wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.CssSelector(Locators.CssSelector.saveimgdialg)));
                    //     wait.Until(Driver => Driver.FindElement(By.CssSelector(Locators.CssSelector.saveimgdialg)));
                    IWebElement ItoolPNG = z3dvp.PNGDisbled();
                    if (ItoolPNG.Enabled)
                    {
                        IWebElement Iclose = z3dvp.CloseSelectedToolBox();
                        if (Iclose.Displayed)
                        {
                            //  Iclose.Click();
                            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", Iclose);
                            bstatusflag3_5 = true;
                            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("Locators.CssSelector.CloseSelectedToolBox")));

                        }
                        Thread.Sleep(1000);
                    }
                }
                if (bstatusflag3_5 == false)
                {
                    Logger.Instance.InfoLog("Failed 2_5 step ");
                }
                //REvert the changed from the 3dSetting
                bool srevert3_5 = z3dvp.MPRQuality("100", "Right");

                //Step 3_6 Click the Save button to save the Image to local drive.
                string filename3_6 = "download_163249_3_6";
                bool bflag3_6 = false;
                //   new Actions(Driver).MoveToElement(Inavigation2_4, Inavigation2_4.Size.Width - 5, Inavigation2_4.Size.Height - 100).Click().Build().Perform();
                new Actions(Driver).DragAndDropToOffset(Inavigation3_4, Inavigation3_4.Size.Width /2- 5, Inavigation3_4.Size.Height /2- 100).Build().Perform();
                Thread.Sleep(1000);
                z3dvp.downloadImageForViewport(filename3_6, "jpg");
                PageLoadWait.WaitForFrameLoad(10);
                if (File.Exists(Config.downloadpath + Path.DirectorySeparatorChar + filename3_6 + ".jpg"))
                {
                    bflag3_6 = true;
                }
                else
                {
                    Logger.Instance.InfoLog("Failed in step 2_6");
                }
                //Step 7 Open the saved image from the local drive and compare with the Screenshot from step 4.
                bool bflag3_7 = false;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Inavigation3_4))
                {
                    bflag3_7 = true;
                }
                else
                {
                    Logger.Instance.InfoLog("Failed in Step 3_7");
                }
                if (bflag3_3 && bflag3_4 && bstatusflag3_5 && bflag3_6 && bflag3_7)
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
                //Repeat the Steps 3-7 on all the controls of Curved MPR viewing Mode.
                bool res4_1 = z3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR, "n");
                if (res4_1 == true)
                {
                    Logger.Instance.InfoLog("Successfully OPend the 4:1 Layout ");
                }
                else
                {
                    Logger.Instance.InfoLog("Failed to open the threeD 4:1 Layout ");
                }

                //step 4_3 Select the Download option from the 3D tool box.
                bool bflag4_3 = false;
                bool btool4_3 = z3dvp.select3DTools(Z3DTools.Download_Image, BluRingZ3DViewerPage.Navigationone);
                if (btool4_3)
                {
                    bflag4_3 = true;
                }
                else
                {
                    Logger.Instance.InfoLog("Failed to select the downloadtool bflag4_3");
                }


                //step 4_4 Left click on the mouse in any one of the Controls. 
                IWebElement Inavigation4_4 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                //for screen shots 
                bool bflag4_4 = false;
                 new Actions(Driver).MoveToElement(Inavigation4_4, Inavigation4_4.Size.Width/2- 5, Inavigation4_4.Size.Height - 100).ClickAndHold().Release().Build().Perform();
                //new Actions(Driver).DragAndDropToOffset(Inavigation4_4, Inavigation4_4.Size.Width / 2 - 5, Inavigation4_4.Size.Height - 100).Build().Perform();
                Thread.Sleep(2000);
                wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.CssSelector(Locators.CssSelector.saveimgdialg)));
                IList<IWebElement> ItoolJPGPNG4_4 = z3dvp.DownloadJPGPNG();
                if (ItoolJPGPNG4_4.Count >= 1 && ItoolJPGPNG4_4[0].Text.ToUpper() == "JPG" && ItoolJPGPNG4_4[1].Text.ToUpper() == "PNG")
                {
                    bflag4_4 = true;
                    IWebElement Iclose = z3dvp.CloseSelectedToolBox();
                    if (Iclose.Displayed)
                    {
                        // Iclose.Click();
                        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", Iclose);
                        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("Locators.CssSelector.CloseSelectedToolBox")));
                    }
                }
                string filename4_4 = "download163249_4_4";
                DownloadImageFile(Inavigation4_4, testcasefolder + Path.DirectorySeparatorChar + filename4_4);
                if (bflag4_4 == false)
                {
                    Logger.Instance.InfoLog("ThreeD4:1 step 2_4 is fiald");
                }


                //Step 4_5 From the window, Enter the File name and Select the image type as 'JPEG' or 'PNG'
                //If the volume is compressed, only JPEG option should be available.
                bool bstatusflag4_5 = false;
                bool sstatus4_5 = z3dvp.MPRQuality("20", "Left");
                if (sstatus4_5)
                {
                    new Actions(Driver).MoveToElement(Inavigation4_4, Inavigation4_4.Size.Width/2 - 5, Inavigation4_4.Size.Height - 100).ClickAndHold().Release().Build().Perform();
                    //new Actions(Driver).DragAndDropToOffset(Inavigation4_4, Inavigation4_4.Size.Width / 2 - 5, Inavigation4_4.Size.Height - 100).Build().Perform();
                    Thread.Sleep(2000);
                    wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.CssSelector(Locators.CssSelector.saveimgdialg)));
                    IWebElement ItoolPNG = z3dvp.PNGDisbled();
                    if (ItoolPNG.Enabled)
                    {
                        IWebElement Iclose = z3dvp.CloseSelectedToolBox();
                        if (Iclose.Displayed)
                        {
                            //   Iclose.Click();
                            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", Iclose);
                            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("Locators.CssSelector.CloseSelectedToolBox")));
                            bstatusflag4_5 = true;
                        }
                        Thread.Sleep(1000);
                    }
                }
                if (bstatusflag4_5 == false)
                {
                    Logger.Instance.InfoLog("Failed 2_5 step ");
                }
                //REvert the changed from the 3dSetting
                bool srevert4_5 = z3dvp.MPRQuality("100", "Right");

                //Step 4_6 Click the Save button to save the Image to local drive.
                string filename4_6 = "download_1632494-6";
                bool bflag4_6 = false;
                  new Actions(Driver).MoveToElement(Inavigation4_4, Inavigation4_4.Size.Width - 5, Inavigation4_4.Size.Height - 100).Click().Build().Perform();
            //    new Actions(Driver).DragAndDropToOffset(Inavigation4_4, Inavigation4_4.Size.Width/2 - 5, Inavigation4_4.Size.Height - 100).Build().Perform();
                Thread.Sleep(1000);
                z3dvp.downloadImageForViewport(filename4_6, "jpg");
                PageLoadWait.WaitForFrameLoad(10);
                if (File.Exists(Config.downloadpath + Path.DirectorySeparatorChar + filename4_6 + ".jpg"))
                {
                    bflag4_6 = true;
                }
                else
                {
                    Logger.Instance.InfoLog("Failed in step 2_6");
                }

                //Step 4_7 Open the saved image from the local drive and compare with the Screenshot from step 4.
                bool bflag4_7 = false;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Inavigation4_4))
                {
                    bflag4_7 = true;
                }
                else
                {
                    Logger.Instance.InfoLog("Failed in Step 2_7");
                }
                if (bflag4_3 && bflag4_4 && bstatusflag4_5 && bflag4_6 && bflag4_7)
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


                // Steps 5-1 on Calcium Scoring mode.
                z3dvp.select3dlayout(BluRingZ3DViewerPage.CalciumScoring, "n");
                Logger.Instance.InfoLog("Successfully OPend the 4:1 Layout ");
                Thread.Sleep(10000);
                z3dvp.checkerrormsg("y");
                IWebElement Iclip = z3dvp.CloseSelectedToolBox();
                Thread.Sleep(1000);
                if (Iclip.Displayed)
                {
                    //Iclip.Click();
                    ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", Iclip);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox)));
                    Thread.Sleep(1000);
                }


                //step 5_3 select the download button 
                bool btool5_3 = z3dvp.select3DTools(Z3DTools.Download_Image, BluRingZ3DViewerPage.CalciumScoring);
                bool bflag5_3 = false;
                if (btool5_3)
                {
                    bflag5_3 = true;
                }
                else
                {
                    Logger.Instance.InfoLog("Failed to select the downloadtool");
                }


                //Step 5_4 Left click on the mouse in any one of the Controls.
                IWebElement Inavigation5_4 = z3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                //for screen shots 
                bool bflag5_4 = false;
                new Actions(Driver).MoveToElement(Inavigation5_4, Inavigation5_4.Size.Width/2- 5, Inavigation5_4.Size.Height /2- 100).ClickAndHold().Release().Build().Perform();
                //new Actions(Driver).DragAndDropToOffset(Inavigation5_4, Inavigation5_4.Size.Width / 2 - 5, Inavigation5_4.Size.Height - 100).Build().Perform();
                Thread.Sleep(2000);
                wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.CssSelector(Locators.CssSelector.saveimgdialg)));
                IList<IWebElement> ItoolJPGPNG5_4 = z3dvp.DownloadJPGPNG();
                if (ItoolJPGPNG5_4.Count >= 1 && ItoolJPGPNG5_4[0].Text.ToUpper() == "JPG" && ItoolJPGPNG5_4[1].Text.ToUpper() == "PNG")
                {
                    bflag5_4 = true;
                    IWebElement Iclose = z3dvp.CloseSelectedToolBox();
                    if (Iclose.Displayed)
                    {
                        //  Iclose.Click();
                        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", Iclose);
                        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("Locators.CssSelector.CloseSelectedToolBox")));
                    }
                }

                string filename5_4 = "download163249_5_4";
                DownloadImageFile(Inavigation5_4, testcasefolder + Path.DirectorySeparatorChar + filename5_4);
                if (bflag5_4 == false)
                {
                    Logger.Instance.InfoLog("ThreeD4:1 step 2_4 is fiald");
                }


                //step 5_5 From the window, Enter the File name and Select the image type as 'JPEG' or 'PNG' 
                //If the volume is compressed, only JPEG option should be available.
                bool bstatusflag5_5 = false;
                bool sstatus5_5 = z3dvp.MPRQuality("20", "Left");
                if (sstatus5_5)
                {
                     new Actions(Driver).MoveToElement(Inavigation5_4, Inavigation5_4.Size.Width/2- 5, Inavigation5_4.Size.Height - 100).ClickAndHold().Release().Build().Perform();
                    //new Actions(Driver).DragAndDropToOffset(Inavigation5_4, Inavigation5_4.Size.Width / 2 - 5, Inavigation5_4.Size.Height - 100).Build().Perform();
                    Thread.Sleep(2000);
                    wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.CssSelector(Locators.CssSelector.saveimgdialg)));
                    IWebElement ItoolPNG = z3dvp.PNGDisbled();
                    if (ItoolPNG.Enabled)
                    {
                        IWebElement Iclose = z3dvp.CloseSelectedToolBox();
                        if (Iclose.Displayed)
                        {
                            //  Iclose.Click();
                            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", Iclose);
                            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("Locators.CssSelector.CloseSelectedToolBox")));
                            bstatusflag5_5 = true;
                        }
                        Thread.Sleep(1000);
                    }
                }
                if (bstatusflag5_5 == false)
                {
                    Logger.Instance.InfoLog("Failed 5_5 step ");
                }
                //REvert the changed from the 3dSetting
                bool srever6_5 = z3dvp.MPRQuality("100", "Right");

                //Step 5_6 Click the Save button to save the Image to local drive.
                string filename5_6 = "download_163249_5_6";
                bool bflag5_6 = false;
                // new Actions(Driver).MoveToElement(Inavigation5_4, Inavigation5_4.Size.Width - 5, Inavigation5_4.Size.Height - 100).Click().Build().Perform();
                new Actions(Driver).DragAndDropToOffset(Inavigation5_4, Inavigation5_4.Size.Width/2 - 5, Inavigation5_4.Size.Height /2- 100).Build().Perform();
                Thread.Sleep(1000);
                z3dvp.downloadImageForViewport(filename5_6, "jpg");
                PageLoadWait.WaitForFrameLoad(10);
                if (File.Exists(Config.downloadpath + Path.DirectorySeparatorChar + filename5_6 + ".jpg"))
                {
                    bflag5_6 = true;
                }
                else
                {
                    Logger.Instance.InfoLog("Failed in step 2_6");
                }

                //Step 7 Open the saved image from the local drive and compare with the Screenshot from step 4.
                bool bflag5_7 = false;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Inavigation5_4))
                {
                    bflag5_7 = true;
                }
                else
                {
                    Logger.Instance.InfoLog("Failed in Step 5_7");
                }
                if (bflag5_3 && bflag5_4 && bstatusflag5_5 && bflag5_6 && bflag5_7)
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
                z3dvp.CloseViewer();
                login.Logout();
            }
        }
        public TestCaseResult Test_163250(String testid, String teststeps, int stepcount)
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
                //Step:1  - Series is loaded in the 3D viewer in MPR 4:1 viewing mode
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step1 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription, BluRingZ3DViewerPage.MPR, field: "acc", thumbimgoptional: Descr2);
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

                //step:2 - Note that the MPR View is displayed as a 2 x 2 Grid view with 3 MPR navigation controls and 1 MPR result control.
                IWebElement Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                IWebElement Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                IWebElement Navigation3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                IWebElement ResultPanel = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel);
                //Verification::In the MPR View, make sure that from left to right and from top to bottom, the controls are labeled as follows:
                int nav1locX = Navigation1.Location.X;
                int nav1locY = Navigation1.Location.Y;
                int nav2locX = Navigation2.Location.X;
                int nav2locY = Navigation2.Location.Y;
                int nav3locX = Navigation3.Location.X;
                int nav3locY = Navigation3.Location.Y;
                int ResultlocX = ResultPanel.Location.X;
                int ResultlocY = ResultPanel.Location.Y;
                if (nav1locX == nav3locX && nav2locX == ResultlocX && nav1locY == nav2locY && nav3locY == ResultlocY)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:3 -Click the User Settings button from the global toolbar and select 3D settings option, move the MPR final quality and 3D final quality sliders to 100%.
                Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.MPRFinalQuality, 100);
                Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.FinalQuality3D, 100);
                String step3_1 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation1);
                String step3_2 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation2);
                String step3_3 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation3);
                String step3_4 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(ResultPanel);
                if (step3_1.Equals("Lossy Compressed") && step3_2.Equals("Lossy Compressed") && step3_3.Equals("Lossy Compressed") && step3_4.Equals("Lossy Compressed"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 24");
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163351 Step 24 as Lossy Compressed annotations failed to display over the images");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:4  -From smart view select the 3D 4:1 viewing mode option from the drop down
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                IWebElement Navigation3D1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                String step4_1 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation1);
                String step4_2 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation2);
                String step4_3 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation3);
                String step4_4 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation3D1);
                if (step4_1.Equals("Lossy Compressed") && step4_2.Equals("Lossy Compressed") && step4_3.Equals("Lossy Compressed") && step4_4.Equals("Lossy Compressed"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 24");
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163351 Step 24 as Lossy Compressed annotations failed to display over the images");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:5 - From smart view select the 3D 6:1 viewing mode option from the drop down
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                ResultPanel = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel);
                Navigation3D1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                IWebElement Navigation3D2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                String step5_1 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation1);
                String step5_2 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation2);
                String step5_3 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation3);
                String step5_4 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(ResultPanel);
                String step5_5 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation3D1);
                String step5_6 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation3D2);
                if (step5_1.Equals("Lossy Compressed") && step5_2.Equals("Lossy Compressed") && step5_3.Equals("Lossy Compressed") && step5_4.Equals("Lossy Compressed") && step5_5.Equals("Lossy Compressed") && step5_6.Equals("Lossy Compressed"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 24");
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163351 Step 24 as Lossy Compressed annotations failed to display over the images");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:6 -  From smart view select the Curved MPR viewing mode option from the drop down.
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                IWebElement CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                IWebElement MPR3DNavigation = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                IWebElement MPRPathNavigation = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                String step6_1 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation1);
                String step6_2 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation2);
                String step6_3 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation3);
                String step6_4 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(CurvedMPR);
                String step6_5 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(MPR3DNavigation);
                String step6_6 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(MPRPathNavigation);
                if (step6_1.Equals("Lossy Compressed") && step6_2.Equals("Lossy Compressed") && step6_3.Equals("Lossy Compressed") && step6_4.Equals("Lossy Compressed") && step6_5.Equals("Lossy Compressed") && step6_6.Equals("Lossy Compressed"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 24");
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163351 Step 24 as Lossy Compressed annotations failed to display over the images");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:7 -From smart view select the Calcium scoring viewing mode option from the drop down.
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.CalciumScoring);
                IWebElement CalciumScoring = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                String step7 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation1);
                if (step7.Equals("Lossy Compressed"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 24");
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163351 Step 24 as Lossy Compressed annotations failed to display over the images");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:8 - Click the User Settings button from the global toolbar and select 3D settings option, move the MPR final quality and 3D final quality sliders lesser 100%. ( Range = 1% to 99%).
                PageLoadWait.WaitForPageLoad(5);
                Z3dViewerPage.checkerrormsg("y");
                PageLoadWait.WaitForPageLoad(5);
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.MPR);
                Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.MPRFinalQuality, 95);
                Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.FinalQuality3D, 95);
                Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                ResultPanel = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel);
                String step8_1 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation1);
                String step8_2 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation2);
                String step8_3 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation3);
                String step8_4 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(ResultPanel);
                if (step8_1.Equals("Lossy Compressed") && step8_2.Equals("Lossy Compressed") && step8_3.Equals("Lossy Compressed") && step8_4.Equals("Lossy Compressed"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 24");
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163351 Step 24 as Lossy Compressed annotations failed to display over the images");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:9 -  	Repeat steps 4-7.
                //4:1
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                Navigation3D1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                String step9_1 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation1);
                String step9_2 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation2);
                String step9_3 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation3);
                String step9_4 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation3D1);
                //6:1
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                ResultPanel = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel);
                Navigation3D1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                Navigation3D2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                String step9_5 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation1);
                String step9_6 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation2);
                String step9_7 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation3);
                String step9_8 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(ResultPanel);
                String step9_9 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation3D1);
                String step9_10 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation3D2);
                //CurvedMPR
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                PageLoadWait.WaitForPageLoad(5);
                Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                MPR3DNavigation = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                MPRPathNavigation = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                String step9_11 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation1);
                String step9_12 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation2);
                String step9_13 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation3);
                String step9_14 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(CurvedMPR);
                String step9_15 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(MPR3DNavigation);
                String step9_16 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(MPRPathNavigation);
                //Calcium Scoring
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.CalciumScoring);
                CalciumScoring = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                String step9_17 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation1);
                if (step9_1.Equals("Lossy Compressed") && step9_2.Equals("Lossy Compressed") && step9_3.Equals("Lossy Compressed") && step9_4.Equals("Lossy Compressed") && step9_5.Equals("Lossy Compressed") && step9_6.Equals("Lossy Compressed") && step9_7.Equals("Lossy Compressed") && step9_8.Equals("Lossy Compressed") && step9_9.Equals("Lossy Compressed") && step9_10.Equals("Lossy Compressed") && step9_11.Equals("Lossy Compressed") && step9_12.Equals("Lossy Compressed") && step9_13.Equals("Lossy Compressed") && step9_14.Equals("Lossy Compressed") && step9_15.Equals("Lossy Compressed") && step9_16.Equals("Lossy Compressed") && step9_17.Equals("Lossy Compressed"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 24");
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163351 Step 24 as Lossy Compressed annotations failed to display over the images");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:10 - From the Universal viewer ,Select a 3D supported No Lossy compressed series and Select the MPR option from the drop down.
                PageLoadWait.WaitForPageLoad(5);
                Z3dViewerPage.checkerrormsg("y");
                PageLoadWait.WaitForPageLoad(5);
                Z3dViewerPage.ExitIcon().Click();
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step10 =  Z3dViewerPage.searchandopenstudyin3D(Study2PID, Study2Descr, BluRingZ3DViewerPage.MPR);
                if(step10)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 24");
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163351 Step 24 as Lossy Compressed annotations failed to display over the images");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:11 - Note that the MPR View is displayed as a 2 x 2 Grid view with 3 MPR navigation controls and 1 MPR result control
                Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                ResultPanel = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel);
                //Verification::In the MPR View, make sure that from left to right and from top to bottom, the controls are labeled as follows:
                nav1locX = Navigation1.Location.X;
                nav1locY = Navigation1.Location.Y;
                nav2locX = Navigation2.Location.X;
                nav2locY = Navigation2.Location.Y;
                nav3locX = Navigation3.Location.X;
                nav3locY = Navigation3.Location.Y;
                ResultlocX = ResultPanel.Location.X;
                ResultlocY = ResultPanel.Location.Y;
                if (nav1locX == nav3locX && nav2locX == ResultlocX && nav1locY == nav2locY && nav3locY == ResultlocY)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:12 - Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.MPRFinalQuality, 100);
                Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.MPRFinalQuality, 100);
                Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.FinalQuality3D, 100);
                String step12_1 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation1);
                String step12_2 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation2);
                String step12_3 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation3);
                String step12_4 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(ResultPanel);
                if (!step12_1.Equals("Lossy Compressed") && !step12_2.Equals("Lossy Compressed") && !step12_3.Equals("Lossy Compressed") && !step12_4.Equals("Lossy Compressed"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 24");
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163351 Step 24 as Lossy Compressed annotations failed to display over the images");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:13 - Repeat Steps 4-7.
                //4:1
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                Thread.Sleep(10000);
                Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                Navigation3D1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                String step13_1 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation1);
                String step13_2 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation2);
                String step13_3 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation3);
                String step13_4 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation3D1);
                //6:1
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                Thread.Sleep(10000);
                Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                ResultPanel = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel);
                Navigation3D1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                Navigation3D2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                String step13_5 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation1);
                String step13_6 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation2);
                String step13_7 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation3);
                String step13_8 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(ResultPanel);
                String step13_9 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation3D1);
                String step13_10 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation3D2);
                //CurvedMPR
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                Thread.Sleep(10000);
                Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                MPR3DNavigation = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                MPRPathNavigation = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                String step13_11 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation1);
                String step13_12 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation2);
                String step13_13 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation3);
                String step13_14 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(CurvedMPR);
                String step13_15 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(MPR3DNavigation);
                String step13_16 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(MPRPathNavigation);
                //Calcium Scoring
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.CalciumScoring);
                Thread.Sleep(10000);
                CalciumScoring = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                String step13_17 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation1);
                if (!step13_1.Equals("Lossy Compressed") && !step13_2.Equals("Lossy Compressed") && !step13_3.Equals("Lossy Compressed") && !step13_4.Equals("Lossy Compressed") && !step13_5.Equals("Lossy Compressed") && !step13_6.Equals("Lossy Compressed") && !step13_7.Equals("Lossy Compressed") && !step13_8.Equals("Lossy Compressed") && !step13_9.Equals("Lossy Compressed") && !step13_10.Equals("Lossy Compressed") && !step13_11.Equals("Lossy Compressed") && !step13_12.Equals("Lossy Compressed") && !step13_13.Equals("Lossy Compressed") && !step13_14.Equals("Lossy Compressed") && !step13_15.Equals("Lossy Compressed") && !step13_16.Equals("Lossy Compressed") && !step13_17.Equals("Lossy Compressed"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 24");
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163351 Step 24 as Lossy Compressed annotations failed to display over the images");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:14 - Click the User Settings button from the global toolbar and select 3D setting option from drop down, move the MPR final quality and 3D final quality sliders lesser 100%. ( Range = 1% to 99%).
                PageLoadWait.WaitForPageLoad(5);
                Z3dViewerPage.checkerrormsg("y");
                PageLoadWait.WaitForPageLoad(5);
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.MPR);
                Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.MPRFinalQuality, 95);
                Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.FinalQuality3D, 95);
                Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                ResultPanel = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel);
                String step14_1 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation1);
                String step14_2 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation2);
                String step14_3 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation3);
                String step14_4 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(ResultPanel);
                if (step14_1.Equals("Lossy Compressed") && step14_2.Equals("Lossy Compressed") && step14_3.Equals("Lossy Compressed") && step14_4.Equals("Lossy Compressed"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 24");
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163351 Step 24 as Lossy Compressed annotations failed to display over the images");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:15 - Repeat 4-7.
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                Navigation3D1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                String step15_1 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation1);
                String step15_2 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation2);
                String step15_3 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation3);
                String step15_4 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation3D1);
                //6:1
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                ResultPanel = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel);
                Navigation3D1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                Navigation3D2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                String step15_5 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation1);
                String step15_6 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation2);
                String step15_7 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation3);
                String step15_8 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(ResultPanel);
                String step15_9 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation3D1);
                String step15_10 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation3D2);
                //CurvedMPR
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                MPR3DNavigation = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                MPRPathNavigation = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                String step15_11 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation1);
                String step15_12 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation2);
                String step15_13 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation3);
                String step15_14 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(CurvedMPR);
                String step15_15 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(MPR3DNavigation);
                String step15_16 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(MPRPathNavigation);
                //Calcium Scoring
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.CalciumScoring);
                CalciumScoring = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                String step15_17 = Z3dViewerPage.GetCenterBottomAnnotationLocationValue(Navigation1);
                if (step15_1.Equals("Lossy Compressed") && step15_2.Equals("Lossy Compressed") && step15_3.Equals("Lossy Compressed") && step15_4.Equals("Lossy Compressed") && step15_5.Equals("Lossy Compressed") && step15_6.Equals("Lossy Compressed") && step15_7.Equals("Lossy Compressed") && step15_8.Equals("Lossy Compressed") && step15_9.Equals("Lossy Compressed") && step15_10.Equals("Lossy Compressed") && step15_11.Equals("Lossy Compressed") && step15_12.Equals("Lossy Compressed") && step15_13.Equals("Lossy Compressed") && step15_14.Equals("Lossy Compressed") && step15_15.Equals("Lossy Compressed") && step15_16.Equals("Lossy Compressed") && step15_17.Equals("Lossy Compressed"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 24");
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163351 Step 24 as Lossy Compressed annotations failed to display over the images");
                    result.steps[++ExecutedSteps].status = "Fail";
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

        public TestCaseResult Test_163252(String testid, String teststeps, int stepcount)
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
                //step:1 - Series is loaded in the 3D viewer in MPR 4:1 viewing mode.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step1 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription, BluRingZ3DViewerPage.MPR, field: "acc", thumbimgoptional: Descr2);
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

                //step:2 - Verify that the images are displayed in low quality in MPR controls in all the viewing modes.
                String BeforeImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_Before.jpg";
                String AfterImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_After.jpg";
                IWebElement Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                DownloadImageFile(Navigation1, BeforeImagePath);
                var ActualFilelength = new FileInfo(BeforeImagePath).Length;
                Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.MPRFinalQuality, 1);
                DownloadImageFile(Navigation1, AfterImagePath);
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

                //step:3 - Download cursor shows up while hovering over the images displayed on the controls.
                Boolean step3 =  Z3dViewerPage.select3DTools(Z3DTools.Download_Image, BluRingZ3DViewerPage.Navigationone);
                if(step3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:4  -Image should be saved in low quality
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 4, Navigation1.Size.Height / 4);
                String imagename = testid + "_" + ExecutedSteps + "_" + new Random().Next(101);
                String imgLocation = Config.downloadpath + "\\" + imagename + ".jpg";
                Z3dViewerPage.downloadImageForViewport(imagename);
                PageLoadWait.WaitForFrameLoad(5);
                Actions Act = new Actions(Driver);
                //Thread.Sleep(3000);
                //Z3dViewerPage.CloseDownloadInfobar();
                Thread.Sleep(2000);
                var LowQualityFile = new FileInfo(imgLocation).Length;
                if(LowQualityFile != ActualFilelength)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:5 - MPR Final quality sliders all the way to the left so that they are set to 100 
                String Before5ImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_5Before.jpg";
                String After5ImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_5After.jpg";
                DownloadImageFile(Navigation1, Before5ImagePath);
                Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.MPRFinalQuality, 100);
                DownloadImageFile(Navigation1, After5ImagePath);
                if (!CompareImage(Before5ImagePath, After5ImagePath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:6 - Right click on the image and select download Image option from the 3D tool box
                Boolean step6 = Z3dViewerPage.select3DTools(Z3DTools.Download_Image, BluRingZ3DViewerPage.Navigationtwo);
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

                //step:7 - Image should be saved in high quality
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 4, Navigation1.Size.Height / 4);
                imagename = testid + "_" + ExecutedSteps + "_" + new Random().Next(101);
                imgLocation = Config.downloadpath + "\\" + imagename + ".jpg";
                Z3dViewerPage.downloadImageForViewport(imagename);
                PageLoadWait.WaitForFrameLoad(5);
                //Z3dViewerPage.CloseDownloadInfobar();
                var HighQualityFile = new FileInfo(imgLocation).Length;
                if (LowQualityFile != HighQualityFile)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:8 - 3D Final quality sliders all the way to the left so that they are set to 1
                String Before8ImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_8Before.jpg";
                String After8ImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_8After.jpg";
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                DownloadImageFile(Navigation1, Before8ImagePath);
                Thread.Sleep(2000);
                ActualFilelength = new FileInfo(Before8ImagePath).Length;
                Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.FinalQuality3D, 1);
                DownloadImageFile(Navigation1, After8ImagePath);
                if (!CompareImage(Before8ImagePath, After8ImagePath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:9 - Download cursor shows up while hovering over the images
                Boolean step9 = Z3dViewerPage.select3DTools(Z3DTools.Download_Image, BluRingZ3DViewerPage.Navigation3D1);
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

                //step:10 - From any one of the 3D controls , Save the Image to local drive
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 4, Navigation1.Size.Height / 4);
                imagename = testid + "_" + ExecutedSteps + "_" + new Random().Next(101);
                imgLocation = Config.downloadpath + "\\" + imagename + ".jpg";
                Z3dViewerPage.downloadImageForViewport(imagename);
                PageLoadWait.WaitForFrameLoad(5);Thread.Sleep(2000);
                //Z3dViewerPage.CloseDownloadInfobar();
                LowQualityFile = new FileInfo(imgLocation).Length;
                if (LowQualityFile != ActualFilelength)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:11 -  3D Final quality sliders all the way to the left so that they are set to 100
                String Before11ImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_11Before.jpg";
                String After11ImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_11After.jpg";
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                DownloadImageFile(Navigation1, Before11ImagePath);
                Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.FinalQuality3D, 100);
                DownloadImageFile(Navigation1, After11ImagePath);
                if (!CompareImage(Before11ImagePath, After11ImagePath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                }

                //step:12 - Download cursor shows up while hovering over the images displayed on the controls
                Boolean step12 = Z3dViewerPage.select3DTools(Z3DTools.Download_Image, BluRingZ3DViewerPage.Navigationone);
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

                //step:13 -  	From any one of the 3D controls , Save the Image to local drive
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 4, Navigation1.Size.Height / 4);
                imagename = testid + "_" + ExecutedSteps + "_" + new Random().Next(101);
                imgLocation = Config.downloadpath + "\\" + imagename + ".jpg";
                Z3dViewerPage.downloadImageForViewport(imagename);
                PageLoadWait.WaitForFrameLoad(5);Thread.Sleep(2000);
                //Z3dViewerPage.CloseDownloadInfobar();
                HighQualityFile = new FileInfo(imgLocation).Length;
                if (LowQualityFile != HighQualityFile)
                {
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

        public TestCaseResult Test_167156(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingViewer bluring = new BluRingViewer();
            String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String ThumbnailImage = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");

            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step:1 - From the Universal viewer , Select a 3D supported series and Select the MPR option from the smart view drop down.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool Layout = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailImage, BluRingZ3DViewerPage.MPR);
                //Hiding the image Text
                new Actions(Driver).SendKeys("T").Build().Perform();
                if (Layout)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 2 :: Verify the show/hide option from the global toolbar.
                bool Hideoptions = Z3dViewerPage.VerifyShowHideValue(BluRingZ3DViewerPage.HideCrossHair);
                if(Hideoptions)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 3 :: Hover over the MPR navigation control 1/ MPR navigation control 2/ MPR navigation control 3 and press the 'x' key on the keyboard.
                IWebElement Nav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                IWebElement Nav2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                IWebElement Nav3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                new Actions(Driver).MoveToElement(Nav1).SendKeys("X").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                //Verification ::Crosshairs on all the MPR navigation controls are toggled off.
                int Step3_1 = Z3dViewerPage.LevelOfSelectedColor(Nav1, testid, ExecutedSteps + 1, 255, 0, 0, 2);//Red
                int Step3_2 = Z3dViewerPage.LevelOfSelectedColor(Nav1, testid, ExecutedSteps + 2, 0, 255, 255, 2);//Cyan
                int Step3_3 = Z3dViewerPage.LevelOfSelectedColor(Nav2, testid, ExecutedSteps + 3, 255, 0, 0, 2);//Red
                int Step3_4 = Z3dViewerPage.LevelOfSelectedColor(Nav2, testid, ExecutedSteps + 4, 255, 255, 0, 2);//Yellow
                int Step3_5 = Z3dViewerPage.LevelOfSelectedColor(Nav3, testid, ExecutedSteps + 5, 255, 255, 0, 2);//Yellow
                int Step3_6 = Z3dViewerPage.LevelOfSelectedColor(Nav3, testid, ExecutedSteps + 6, 0, 255, 255, 2);//Cyan
                if (Step3_1 == 0 && Step3_2 == 0 && Step3_3 == 0 && Step3_4 == 0 && Step3_5 == 0 && Step3_6 == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 4 :: Select the 3D 6:1 view mode from the smart view drop down.
                bool ThreeD6x1 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                Nav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Nav2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Nav3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                IWebElement ThreeD1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                IWebElement ThreeD2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                //Verification ::
                int Step4_1 = Z3dViewerPage.LevelOfSelectedColor(Nav1, testid, ExecutedSteps + 1, 255, 0, 0, 2);//Red
                int Step4_2 = Z3dViewerPage.LevelOfSelectedColor(Nav1, testid, ExecutedSteps + 2, 0, 255, 255, 2);//Cyan
                int Step4_3 = Z3dViewerPage.LevelOfSelectedColor(Nav2, testid, ExecutedSteps + 4, 255, 255, 0, 2);//Yellow
                int Step4_4 = Z3dViewerPage.LevelOfSelectedColor(ThreeD1, testid, ExecutedSteps + 5, 255, 255, 0, 2);//Yellow
                int Step4_5 = Z3dViewerPage.LevelOfSelectedColor(ThreeD2, testid, ExecutedSteps + 6, 255, 255, 0, 2);//Yellow
                if (ThreeD6x1 && Step4_1 == 0 && Step4_2 == 0 && Step4_3 == 0 && Step4_4 == 0 && Step4_5 == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 5 :: Verify the show/hide option from the global toolbar
                bool Showoptions = Z3dViewerPage.VerifyShowHideValue(BluRingZ3DViewerPage.ShowCrossHair);
                if (Showoptions)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 6 :: Again press the 'x' key on the keyboard.
                new Actions(Driver).MoveToElement(Nav1).SendKeys("X").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                //Verification :: Crosshairs are displayed over the images in MPR navigation controls. Rotate hotspots are displayed over the images in 3D1 and 3D2 controls.
                int Step6_1 = Z3dViewerPage.LevelOfSelectedColor(Nav1, testid, ExecutedSteps + 1, 255, 0, 0, 2);//Red
                int Step6_2 = Z3dViewerPage.LevelOfSelectedColor(Nav1, testid, ExecutedSteps + 2, 0, 255, 255, 2);//Cyan
                int Step6_3 = Z3dViewerPage.LevelOfSelectedColor(Nav2, testid, ExecutedSteps + 3, 255, 0, 0, 2);//Red
                int Step6_4 = Z3dViewerPage.LevelOfSelectedColor(Nav2, testid, ExecutedSteps + 4, 255, 255, 0, 2);//Yellow
                int Step6_5 = Z3dViewerPage.LevelOfSelectedColor(Nav3, testid, ExecutedSteps + 5, 255, 255, 0, 2);//Yellow
                int Step6_6 = Z3dViewerPage.LevelOfSelectedColor(Nav3, testid, ExecutedSteps + 55, 0, 255, 255, 2);//Cyan
                int Step6_7 = Z3dViewerPage.LevelOfSelectedColor(ThreeD1, testid, ExecutedSteps + 7, 255, 255, 0, 2);//Yellow
                int Step6_8 = Z3dViewerPage.LevelOfSelectedColor(ThreeD2, testid, ExecutedSteps + 8, 255, 255, 0, 2);//Yellow
                if (Step6_1!=0 && Step6_2!=0 && Step6_3!=0 && Step6_4!=0 && Step6_5!=0 && Step6_6!=0 && Step6_7!=0 && Step6_8!=0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 7 :: Select the 3D 4:1 option from the smart view drop down.
                bool ThreeD4x1 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                ThreeD1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                //Verification :: Images are displayed in the 3D 4:1 viewing mode.Rotate hotspots are displayed over the image in 3D1 control
                ThreeD1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                int Step7_1 = Z3dViewerPage.LevelOfSelectedColor(ThreeD1, testid, ExecutedSteps + 6, 255, 255, 0, 2);//Yellow
                if (ThreeD4x1 && Step7_1!=0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 8 :: From the Global toolbar, Select show/hide option. Select HIDE 3D controls.
                bool SelectHIDE3D = bluring.SelectShowHideValue(BluRingZ3DViewerPage.HideCrossHair);
                //Verification:: Rotate hotspots (X, Y, Z) on 3D1 control are toggled off.
                int Step8_1 = Z3dViewerPage.LevelOfSelectedColor(ThreeD1, testid, ExecutedSteps + 5, 255, 255, 0, 2);//Yellow
                if (SelectHIDE3D && Step8_1 == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 9 :: Now Select the 3D 6:1 option from the smart view drop down.
                ThreeD6x1 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                Nav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Nav2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Nav3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                ThreeD1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                ThreeD2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                //Verification :: 
                int Step9_1 = Z3dViewerPage.LevelOfSelectedColor(Nav1, testid, ExecutedSteps + 1, 255, 0, 0, 2);//Red
                int Step9_2 = Z3dViewerPage.LevelOfSelectedColor(Nav1, testid, ExecutedSteps + 2, 0, 255, 255, 2);//Cyan
                int Step9_3 = Z3dViewerPage.LevelOfSelectedColor(Nav2, testid, ExecutedSteps + 4, 255, 255, 0, 2);//Yellow
                int Step9_4 = Z3dViewerPage.LevelOfSelectedColor(ThreeD1, testid, ExecutedSteps + 5, 255, 255, 0, 2);//Yellow
                int Step9_5 = Z3dViewerPage.LevelOfSelectedColor(ThreeD2, testid, ExecutedSteps + 6, 255, 255, 0, 2);//Yellow
                if (ThreeD6x1 && Step9_1 == 0 && Step9_2 == 0 && Step9_3 == 0 && Step9_4 == 0 && Step9_5 == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 10 :: From the Global toolbar, Select show/hide option. Select SHOW 3D controls.
                bool SelectSHOW3D = bluring.SelectShowHideValue(BluRingZ3DViewerPage.ShowCrossHair);
                //Verification :: Crosshairs are displayed over the images in MPR navigation controls.Rotate hotspots are displayed over the images in 3D1 and 3D2 controls.
                int Step10_1 = Z3dViewerPage.LevelOfSelectedColor(Nav1, testid, ExecutedSteps + 1, 255, 0, 0, 2);//Red
                int Step10_2 = Z3dViewerPage.LevelOfSelectedColor(Nav1, testid, ExecutedSteps + 2, 0, 255, 255, 2);//Cyan
                int Step10_3 = Z3dViewerPage.LevelOfSelectedColor(Nav2, testid, ExecutedSteps + 4, 255, 255, 0, 2);//Yellow
                int Step10_4 = Z3dViewerPage.LevelOfSelectedColor(ThreeD1, testid, ExecutedSteps + 5, 255, 255, 0, 2);//Yellow
                int Step10_5 = Z3dViewerPage.LevelOfSelectedColor(ThreeD2, testid, ExecutedSteps + 6, 255, 255, 0, 2);//Yellow
                if (SelectSHOW3D && Step10_1 != 0 && Step10_2 != 0 && Step10_3 != 0 && Step10_4 != 0 && Step10_5 != 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 11 :: Select the Curved MPR option from the smart view drop down.
                bool CurvedMpr = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                Nav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Nav2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Nav3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                //Verification :: mages are displayed in the curved MPR viewing mode.Crosshairs are displayed over the images in MPR navigation controls.
                int Step11_1 = Z3dViewerPage.LevelOfSelectedColor(Nav1, testid, ExecutedSteps + 1, 255, 0, 0, 2);//Red
                int Step11_2 = Z3dViewerPage.LevelOfSelectedColor(Nav1, testid, ExecutedSteps + 2, 0, 255, 255, 2);//Cyan
                int Step11_3 = Z3dViewerPage.LevelOfSelectedColor(Nav2, testid, ExecutedSteps + 3, 255, 0, 0, 2);//Red
                int Step11_4 = Z3dViewerPage.LevelOfSelectedColor(Nav2, testid, ExecutedSteps + 4, 255, 255, 0, 2);//Yellow
                int Step11_5 = Z3dViewerPage.LevelOfSelectedColor(Nav3, testid, ExecutedSteps + 5, 255, 255, 0, 2);//Yellow
                int Step11_6 = Z3dViewerPage.LevelOfSelectedColor(Nav3, testid, ExecutedSteps + 6, 0, 255, 255, 2);//Cyan
                if (CurvedMpr && Step11_1 != 0 && Step11_2 != 0 && Step11_3 != 0 && Step11_4 != 0 && Step11_5 != 0 && Step11_6 != 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 12 :: From the Global toolbar, Select show/hide option. Select hide 3D controls.
                bool Step12 = bluring.SelectShowHideValue(BluRingZ3DViewerPage.HideCrossHair);
                //Verification :: Crosshairs on all the MPR navigation controls are toggled OFF.
                int Step12_1 = Z3dViewerPage.LevelOfSelectedColor(Nav1, testid, ExecutedSteps + 1, 255, 0, 0, 2);//Red
                int Step12_2 = Z3dViewerPage.LevelOfSelectedColor(Nav1, testid, ExecutedSteps + 2, 0, 255, 255, 2);//Cyan
                int Step12_3 = Z3dViewerPage.LevelOfSelectedColor(Nav2, testid, ExecutedSteps + 3, 255, 0, 0, 2);//Red
                int Step12_4 = Z3dViewerPage.LevelOfSelectedColor(Nav2, testid, ExecutedSteps + 4, 255, 255, 0, 2);//Yellow
                int Step12_5 = Z3dViewerPage.LevelOfSelectedColor(Nav3, testid, ExecutedSteps + 5, 255, 255, 0, 2);//Yellow
                int Step12_6 = Z3dViewerPage.LevelOfSelectedColor(Nav3, testid, ExecutedSteps + 6, 0, 255, 255, 2);//Cyan
                if (Step12_1 == 0 && Step12_2 == 0 && Step12_3 == 0 && Step12_4 == 0 && Step12_5 == 0 && Step12_6 == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 13 :: Press the 'x' key on the keyboard.
                new Actions(Driver).MoveToElement(Nav1).SendKeys("X").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                //Verification :: Crosshairs on all the MPR navigation controls are toggled ON.
                int Step13_1 = Z3dViewerPage.LevelOfSelectedColor(Nav1, testid, ExecutedSteps + 1, 255, 0, 0, 2);//Red
                int Step13_2 = Z3dViewerPage.LevelOfSelectedColor(Nav1, testid, ExecutedSteps + 2, 0, 255, 255, 2);//Cyan
                int Step13_3 = Z3dViewerPage.LevelOfSelectedColor(Nav2, testid, ExecutedSteps + 3, 255, 0, 0, 2);//Red
                int Step13_4 = Z3dViewerPage.LevelOfSelectedColor(Nav2, testid, ExecutedSteps + 4, 255, 255, 0, 2);//Yellow
                int Step13_5 = Z3dViewerPage.LevelOfSelectedColor(Nav3, testid, ExecutedSteps + 5, 255, 255, 0, 2);//Yellow
                int Step13_6 = Z3dViewerPage.LevelOfSelectedColor(Nav3, testid, ExecutedSteps + 6, 0, 255, 255, 2);//Cyan
                if (Step13_1 != 0 && Step13_2 != 0 && Step13_3 != 0 && Step13_4 != 0 && Step13_5 != 0 && Step13_6 != 0)
                {
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
    }
}