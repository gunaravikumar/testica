using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MergeServiceTool;
using Selenium.Scripts.Reusable.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Selenium.Scripts.Tests
{
    class GSPS
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        //public string EA_91 = "VMSSA-5-38-91";
        //public string EA_131 = "VMSSA-4-38-131";
        //public string PACS_A7 = "PA-A7-WS8";
        //public string DS_77 = "AUTO-SSA-001";

        public string DS_77 = "VMSSA-5-38-96";


        public static int random = new Random().Next(1000);
       
        public string User1 = "GUser1_" + random;
        public string User2 = "GUser2_" + random;
        public string User3 = "GUser3_" + random;

        public string SuperRole = "SuperRole";
        public string SuperAdminDomain = "SuperAdminGroup";

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        public GSPS(String classname)
        {
            login = new Login();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }


        /// <summary> 
        /// This Test Case is Verification of GSPS "Initial Setup"
        /// </summary>

        public TestCaseResult Test_27903(String testid, String teststeps, int stepcount)
        {
            //-Initial Setup--
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            String username = Config.adminUserName;
            String password = Config.adminPassword;

            try
            {
                //Step-1
                //Configure the iConnect Access to an online data source.
                //Added EA-77, EA-91, EA-131 and PA-A7-WS8
                //A Data source is connected.

                //Precondtion
                ExecutedSteps++;

                //Step-2
                //Login iConnect Access as Administrator. 
                //Create 3 users (e.g.. user1, user2, user3) with User type if haven't created.

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                UserManagement usermgt = (UserManagement)login.Navigate("UserManagement");
                usermgt.CreateUser(User1, SuperAdminDomain, SuperRole);
                usermgt.CreateUser(User2, SuperAdminDomain, SuperRole);
                usermgt.CreateUser(User3, SuperAdminDomain, SuperRole);

                bool u1 = usermgt.IsUserExist(User1, SuperAdminDomain);
                bool u2 = usermgt.IsUserExist(User2, SuperAdminDomain);
                bool u3 = usermgt.IsUserExist(User3, SuperAdminDomain);

                //3 users are created.
                if (u1 && u2 && u3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.ErrorLog("--u1 && u2 && u3--" + u1 + "-" + u2 + "-" + u3);

                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-3
                //On the iConnect Access server system, go to Control Panel --> Administrative Tools -->Services.

                ServiceController sc = new ServiceController("iConnect Access Part 10 Import Service");

                //The WebAccess Part 10 Import Services is started (default).
                if (sc.Status.ToString().Equals("Running"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
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
        }


        /// <summary> 
        /// This Test Case is Verification of GSPS "1.0 Saving GSPS to archive"
        /// </summary>

        public TestCaseResult Test_27904(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            String username = Config.adminUserName;
            String password = Config.adminPassword;

            int randomNo = new Random().Next(1, 1000);
            
            String NewDomain = "NewDomain_" + randomNo;
            String NewRole = "NewRole_" + randomNo;

            String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNo");
            String[] AccessionNumber = AccessionNoList.Split(':');


            String ModalityList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNo");
            String[] Modality = ModalityList.Split(':');



            try
            {
                //Step-1
                //Test Data- Saving GSPS to online data source
                //Login with account created for the testing, e.g. user1. Load any study

                //Precondition--
                //Have to upload study in EA (Runtime)

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);


                UserManagement usermgt = (UserManagement)login.Navigate("UserManagement");
                if (!usermgt.IsUserExist(User1, SuperAdminDomain)) usermgt.CreateUser(User1, SuperAdminDomain, SuperRole);
                if (!usermgt.IsUserExist(User2, SuperAdminDomain)) usermgt.CreateUser(User2, SuperAdminDomain, SuperRole);
                if (!usermgt.IsUserExist(User3, SuperAdminDomain)) usermgt.CreateUser(User3, SuperAdminDomain, SuperRole);

                Studies study = (Studies)login.Navigate("Studies");

                study.SearchStudy(AccessionNo: AccessionNumber[0], Datasource: DS_77);
                study.SelectStudy("Accession", AccessionNumber[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //The study successfully loads.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status1 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status1 && viewer.SeriesViewer_1X1().Displayed &&
                    viewer.studyPanel().Displayed)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-2
                //Check the tools bar above the image viewport.

                IWebElement saveSeries = viewer.GetReviewTool("Save Series");
                viewer.JSMouseHover(saveSeries);

                //There is a Save menu (floppy icon) with two options for saving GSPS-
                //Save Series (Saves all images in the series)
                //Save Annotated Images (Only images with annotations are saved)

                IList<IWebElement> dropdown2 = BasePage.Driver.FindElements(By.CssSelector("li[title='Save Series'] ul>li"));

                if (dropdown2.Count == 2 &&
                    dropdown2[0].Displayed && dropdown2[0].GetAttribute("title").Equals("Save Series") &&
                    dropdown2[1].Displayed && dropdown2[1].GetAttribute("title").Equals("Save Annotated Images"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-3
                //Test Data-
                //Note down the WW/WL and positions of the measurements if needed.
                //Apply W/L, Zoom, Pan and measurements on the image.

                //Apply W/L
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                IWebElement viewport1 = viewer.SeriesViewer_1X1();
                int h = viewport1.Size.Height;
                int w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                result.steps[++ExecutedSteps].SetPath(testid + "_3_1_WL_Applied", ExecutedSteps + 1);
                bool status3_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                //Apply Zoom
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                result.steps[ExecutedSteps].SetPath(testid + "_3_2_Zoom_Applied", ExecutedSteps + 1);
                bool status3_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                //Apply Pan
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, w / 2, h / 4);

                result.steps[ExecutedSteps].SetPath(testid + "_3_3_Pan_Applied", ExecutedSteps + 1);
                bool status3_3 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                //WW/WL, Zoom, Pan is applied on the image. Applied measurement is displayed.

                if (status3_1 && status3_2 && status3_3)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4
                //Interactively adjust the edge enhancement of the images, 
                //by selecting the interactive edge enhancement tool and 
                //dragging the left mouse button up/down and left/right.

                //move right to left
                viewer.SelectToolInToolBar(IEnum.ViewerTools.EdgeEnhancementInteractive);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 2, w, h / 2);

                result.steps[++ExecutedSteps].SetPath(testid + "_4_1_R_L_Edge_Applied", ExecutedSteps + 1);
                bool status4_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                //move top to bottom
                viewer.SelectToolInToolBar(IEnum.ViewerTools.EdgeEnhancementInteractive);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, 3 * w / 4, h / 2, 3 * w / 4, h);
                viewer.DragandDropImage(viewport1, 3 * w / 4, h / 2, 3 * w / 4, h);

                result.steps[ExecutedSteps].SetPath(testid + "_4_2_T_B_Edge_Applied", ExecutedSteps + 1);
                bool status4_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                //The down/up mouse movement should change the size of enhanced edges and 
                //left/right movement should change the contrast of enhanced edges.

                if (status4_1 && status4_2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                int ThumbnailCountBeforeSave = viewer.Thumbnails().Count;

                //Step-5
                //Test Data- It will change to a warning icon if the saving is failed.
                //From the save menu select Save Series.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.SaveSeries);

                //A"Spinner"is displayed at top left side of the tool bar while saving GSPS.
                //It disappears when the saving is successful; 
                //a thumbnail of the saved PR series appears in the Series Thumbnail area.

                PageLoadWait.WaitForLoadingIconToAppear_Savestudy();
                PageLoadWait.WaitForLoadingIconToDisAppear_Savestudy();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ThumbnailCaptions()[0]));
                ExecutedSteps++;


                //Step-6
                //Go to study list, check the Modality column of the study.
                //note- it takes time to reflect modality change in studylist

                study.CloseStudy();

                //There is"/PR"appended to the original modality of the study (e.g., CR/PR) under the Modality column.
                study.SearchStudy(AccessionNo: AccessionNumber[0], Datasource: DS_77);

                Dictionary<string, string> row6 = study.GetMatchingRow(new string[] { "Accession" }, new string[] { AccessionNumber[0] });

                if (row6 != null && row6["Modality"].Contains("PR"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7
                //Re-load the study.

                study.SearchStudy(AccessionNo: AccessionNumber[0], Datasource: DS_77);
                study.SelectStudy("Accession", AccessionNumber[0]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //Saved PR series is displayed in Series Thumbnail area.

                if (viewer.Thumbnails().Count == (ThumbnailCountBeforeSave + 1) &&
                    viewer.ThumbnailCaptions()[0].Text.Contains("PR") &&
                    viewer.SeriesViewer_1X1().Displayed &&
                    viewer.studyPanel().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-8
                //Verify the saved PR series.

                //Image is displayed as zoomed and paned with saved W/L. 
                //The saved measurements have the same shape and at the same position as they were saved. 
                //The edge enhancement applied previoulsy is saved as well. 
                //All images from the original series are present in the PR series.

                //-----Only Image validation done (Double check the Image)-----
                result.steps[++ExecutedSteps].SetPath(testid + "_8_Zm_WL_Pan_edge_Applied", ExecutedSteps + 1);
                bool status8 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());


                if (status8)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Stpe-9
                //Load a study that has multiple images in a series. Apply various tools 
                //(rotation, W/L, zoom, pan, measurements) on different images.
                //Select each pre-defined options from the edge enhancement tool button menu 
                //(ex. Low 5x5, Medium 3x3, etc.)Select Save Series.

                //AccessionNumber[1]=""; (study has multiple images)

                study.CloseStudy();


                study.SearchStudy(AccessionNo: AccessionNumber[1], Datasource: DS_77);
                study.SelectStudy("Accession", AccessionNumber[1]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                int ThumbnailCountBeforeSave_SecondStudy = viewer.Thumbnails().Count;

                //Apply Rotate
                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                result.steps[++ExecutedSteps].SetPath(testid + "_9_1_Rotate_Applied", ExecutedSteps + 1);
                bool status9_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                //Apply W/L
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                result.steps[ExecutedSteps].SetPath(testid + "_9_2_WL_Applied", ExecutedSteps + 1);
                bool status9_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                //Apply Zoom
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                result.steps[ExecutedSteps].SetPath(testid + "_9_3_Zoom_Applied", ExecutedSteps + 1);
                bool status9_3 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                //Apply Pan
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                result.steps[ExecutedSteps].SetPath(testid + "_9_4_Pan_Applied", ExecutedSteps + 1);
                bool status9_4 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());


                //Draw Line
                viewer.SelectToolInToolBar(IEnum.ViewerTools.LineMeasurement);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                Actions action = new Actions(BasePage.Driver);
                action.MoveToElement(viewport1, 2 * w / 3 + 30, h / 3 + 15).Click().Build().Perform();
                Thread.Sleep(3000);
                action.MoveToElement(viewport1, 2 * w / 3 + w / 6, h / 3 + h / 4).Click().Build().Perform();
                Thread.Sleep(2000);


                if (!((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("firefox"))
                {
                    action.Release(viewport1).Build().Perform();
                    Thread.Sleep(2000);
                }

                result.steps[ExecutedSteps].SetPath(testid + "_9_5_Line_Applied", ExecutedSteps + 1);
                bool status9_5 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                //Edge Enhancement Low 5x5
                viewer.SelectToolInToolBar(IEnum.ViewerTools.EdgeEnhancementLow5x5);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                result.steps[ExecutedSteps].SetPath(testid + "_9_6_Edge_5x5_Applied", ExecutedSteps + 1);
                bool status9_6 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                //Edge Enhancement Medium 3x3
                viewer.SelectToolInToolBar(IEnum.ViewerTools.EdgeEnhancementMedium3x3);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                result.steps[ExecutedSteps].SetPath(testid + "_9_7_Edge_3x3_Applied", ExecutedSteps + 1);
                bool status9_7 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.SelectToolInToolBar(IEnum.ViewerTools.SaveSeries);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //A"Spinner"is displayed at top left side of the tool bar while saving GSPS, 
                //and then it disappears when the saving is completed successfully; 
                //a thumbnail of the saved PR series appears in the Series Thumbnail area.

                PageLoadWait.WaitForLoadingIconToAppear_Savestudy();
                PageLoadWait.WaitForLoadingIconToDisAppear_Savestudy();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ThumbnailCaptions()[0]));

                //sync up and Thumbnail count--

                if (status9_1 && status9_2 && status9_3 && status9_4 && status9_5 && status9_6 && status9_7 &&
                    viewer.Thumbnails().Count == (ThumbnailCountBeforeSave_SecondStudy + 1))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-10
                //Re-load the study.

                study.CloseStudy();
                study.SearchStudy(AccessionNo: AccessionNumber[1], Datasource: DS_77);
                study.SelectStudy("Accession", AccessionNumber[1]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //There is a PR series is displayed in the Series Thumbnail area.

                if (viewer.Thumbnails().Count == (ThumbnailCountBeforeSave_SecondStudy + 1) &&
                    viewer.ThumbnailCaptions()[0].Text.Contains("PR") &&
                    viewer.SeriesViewer_1X1().Displayed &&
                    viewer.studyPanel().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-11
                //Test Data- 
                //All images of the series are saved in the PR series.
                //Verify the saved PR series.


                //Images are displayed with saved GSPS (rotation, W/L, zoom, pan). 
                //The saved measurements have the same shape and at the same position as they were saved.
                //The edge enhancement applied previoulsy is saved as well. 
                //All images from the original series are present in the PR series.


                //-----Only Image validation done (Double check the Image)-----
                result.steps[++ExecutedSteps].SetPath(testid + "_11_Ro_Zm_WL_P_ed_Applied", ExecutedSteps + 1);
                bool status11 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());


                if (status11)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                study.CloseStudy();
                login.Logout();

                //Step-12

                //On other client system, login as another user (e.g., user2 or user3), 
                //load the studies that have GSPS saved by other user.

                login.DriverGoTo(login.url);
                login.LoginIConnect(User3, User3);
                study = (Studies)login.Navigate("Studies");
                study.SearchStudy(AccessionNo: AccessionNumber[1], Datasource: DS_77);
                study.SelectStudy("Accession", AccessionNumber[1]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                result.steps[++ExecutedSteps].SetPath(testid + "_12_Ro_Zm_WL_P_ed_Applied", ExecutedSteps + 1);
                bool status12 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());


                //Images and saved GSPS can be view by another user on a different client system.
                //The saved measurements have the same shape and at the same position as they were saved.

                if (status12 &&
                    viewer.Thumbnails().Count == (ThumbnailCountBeforeSave_SecondStudy + 1) &&
                  viewer.ThumbnailCaptions()[0].Text.Contains("PR") &&
                  viewer.SeriesViewer_1X1().Displayed &&
                  viewer.studyPanel().Displayed)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-13
                //Apply various tools (rotation, W/L, zoom, pan, measurements) on images 
                //that have GSPS saved by another user and then select Save Series.

                int ThumbnailCountBeforeSave_ThirdStudy = viewer.Thumbnails().Count;


                //Apply Pan
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, 3 * w / 4, 3 * h / 4, w / 4, h / 4);

                result.steps[++ExecutedSteps].SetPath(testid + "_13_1_Pan_Applied", ExecutedSteps + 1);
                bool status13_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                //Apply Rotate
                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                result.steps[ExecutedSteps].SetPath(testid + "_13_2_Rotate_Applied", ExecutedSteps + 1);
                bool status13_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                //Apply W/L
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                result.steps[ExecutedSteps].SetPath(testid + "_13_3_WL_Applied", ExecutedSteps + 1);
                bool status13_3 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                //Apply Zoom
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                result.steps[ExecutedSteps].SetPath(testid + "_13_4_Zoom_Applied", ExecutedSteps + 1);
                bool status13_4 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                //Draw Line
                viewer.SelectToolInToolBar(IEnum.ViewerTools.LineMeasurement);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                action = new Actions(BasePage.Driver);
                action.MoveToElement(viewport1, w / 3, h / 3).Click().Build().Perform();
                Thread.Sleep(3000);
                action.MoveToElement(viewport1, w / 2, h / 2).Click().Build().Perform();
                Thread.Sleep(2000);


                if (!((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("firefox"))
                {
                    action.Release(viewport1).Build().Perform();
                    Thread.Sleep(2000);
                }

                result.steps[ExecutedSteps].SetPath(testid + "_13_5_Line_Applied", ExecutedSteps + 1);
                bool status13_5 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                //Save

                viewer.SelectToolInToolBar(IEnum.ViewerTools.SaveSeries);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //The GSPS can be saved. (no warning icon).
                //A PR series thumbnail appears in the Series Thumbnail area.


                PageLoadWait.WaitForLoadingIconToAppear_Savestudy();
                PageLoadWait.WaitForLoadingIconToDisAppear_Savestudy();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ThumbnailCaptions()[0]));

                if (status13_1 && status13_2 && status13_3 && status13_4 && status13_5 &&
                    viewer.Thumbnails().Count == (ThumbnailCountBeforeSave_ThirdStudy + 1) &&
                    viewer.ThumbnailCaptions()[0].Text.Contains("PR"))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-14
                //Test Data- 
                //It may initially display"Error, please try again.."when looking at the newly saved PR series.
                //This will happen until the PR series is imported into I-Store. 
                //Click the PR series thumbnail again until the error goes away and the images are displayed
                //Verify the newly saved PR series.

                //A new PR series is displayed with all tools applied (new and existing).

                //only image validation--Double check--
                result.steps[++ExecutedSteps].SetPath(testid + "_14_Ro_Zm_WL_P_L_Applied", ExecutedSteps + 1);
                bool status14 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status14)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                study.CloseStudy();
                login.Logout();

                //Step-15
                //On a different system login as user1, check the PR series saved by user3.

                login.DriverGoTo(login.url);
                login.LoginIConnect(User1, User1);
                study = (Studies)login.Navigate("Studies");
                study.SearchStudy(AccessionNo: AccessionNumber[1], Datasource: DS_77);
                study.SelectStudy("Accession", AccessionNumber[1]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //Images and saved GSPS can be view by another user on a different client system.

                result.steps[++ExecutedSteps].SetPath(testid + "_15_1_Ro_Zm_WL_P_L_Applied", ExecutedSteps + 1);
                bool status15_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                result.steps[ExecutedSteps].SetPath(testid + "_15_2_Ro_Zm_WL_P_L_Applied", ExecutedSteps + 1);
                bool status15_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                //Check and add viewer.Thumbnails()[1].Text.Contains("PR") also //remove contains and change to Equal
                if (status15_1 && status15_2 &&
                    viewer.Thumbnails().Count == (ThumbnailCountBeforeSave_ThirdStudy + 1) &&
                    viewer.ThumbnailCaptions()[0].Text.Contains("PR") &&
                    viewer.ThumbnailCaptions()[1].Text.Contains("PR"))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                study.CloseStudy();
                login.Logout();

                //Step-16
                //Test Data- 
                //Enabling / Disabling Saving GSPS - Configuring at the Domain level
                //Log in as the Administrator, navigate to the Domain management page,
                //select the domain used for Testing and click Edit.

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain(SuperAdminDomain);
                domain.SelectDomain(SuperAdminDomain);
                domain.ClickEditDomain();

                //The Edit Domain page is displayed. 
                //There is an option to Enable Saving GSPS that is enabled by default.

                if (domain.DomainManagementHeaderLabelEditDomain().Text.Equals("Domain Management") &&
                    domain.PageHeaderLabel().Text.Equals("Edit Domain") &&
                    domain.SaveGspsCB().Selected == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-17
                //Disable saving GSPS and save the Domain.
                domain.SetCheckBoxInEditDomain("savegsps", 1);

                //The Domain is modified and saved.
                if (domain.SaveGspsCB().Selected == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                domain.ClickSaveDomain();

                //Step-18
                //Log out and back in as a user within the test domain. Load a study.

                login.Logout();

                login.DriverGoTo(login.url);
                login.LoginIConnect(User1, User1);
                study = (Studies)login.Navigate("Studies");
                study.SearchStudy(AccessionNo: AccessionNumber[1], Datasource: DS_77);
                study.SelectStudy("Accession", AccessionNumber[1]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);


                //The Save tool is not available.
                bool step_18 = false;
                try { saveSeries = viewer.GetReviewTool("Save Series"); }
                catch (NoSuchElementException e) { step_18 = true; }

                if (step_18)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-19
                //Re-enable saving GSPS for the domain.
                study.CloseStudy();
                login.Logout();

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain(SuperAdminDomain);
                domain.SelectDomain(SuperAdminDomain);
                domain.ClickEditDomain();

                //Saving GSPS for the domain is Re-enabled

                domain.SetCheckBoxInEditDomain("savegsps", 0);

                if (domain.SaveGspsCB().Selected == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                domain.ClickSaveDomain();

                login.Logout();


                //Step-20
                //Log in as a user within the test domain and load a study.
                login.DriverGoTo(login.url);
                login.LoginIConnect(User1, User1);
                study = (Studies)login.Navigate("Studies");
                study.SearchStudy(AccessionNo: AccessionNumber[1], Datasource: DS_77);
                study.SelectStudy("Accession", AccessionNumber[1]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //The Save tool is available again.


                saveSeries = viewer.GetReviewTool("Save Series");
                viewer.JSMouseHover(saveSeries);

                IList<IWebElement> dropdown20 = BasePage.Driver.FindElements(By.CssSelector("li[title='Save Series'] ul>li"));


                if (dropdown20.Count == 2 && saveSeries.Displayed == true &&
                    dropdown20[0].Displayed && dropdown20[0].GetAttribute("title").Equals("Save Series") &&
                    dropdown20[1].Displayed && dropdown20[1].GetAttribute("title").Equals("Save Annotated Images"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-21
                //Apply various tools and select Save > Save Series.

                //Apply W/L
                viewer.SeriesViewer_2X1().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                IWebElement viewport = viewer.SeriesViewer_2X1();
                h = viewport.Size.Height;
                w = viewport.Size.Width;
                viewer.DragandDropImage(viewport, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                result.steps[++ExecutedSteps].SetPath(testid + "_21_1_WL_Applied", ExecutedSteps + 1);
                bool status21_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_2X1());


                //Apply Pan
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                viewport = viewer.SeriesViewer_2X1();
                h = viewport.Size.Height;
                w = viewport.Size.Width;
                viewer.DragandDropImage(viewport, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                result.steps[ExecutedSteps].SetPath(testid + "_21_2_Pan_Applied", ExecutedSteps + 1);
                bool status21_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_2X1());

                int ThumbnailCountBeforeSave_FourthStudy = viewer.Thumbnails().Count;

                //Select Save
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SaveSeries);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);


                PageLoadWait.WaitForLoadingIconToAppear_Savestudy();
                PageLoadWait.WaitForLoadingIconToDisAppear_Savestudy();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ThumbnailCaptions()[0]));

                //The changes are saved to a PR series.

                if (status21_1 && status21_2 &&
                    viewer.Thumbnails().Count == (ThumbnailCountBeforeSave_FourthStudy + 1) &&
                    viewer.ThumbnailCaptions()[0].Text.Contains("PR"))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                study.CloseStudy();
                login.Logout();


                //Step-22
                //Test Data- Enabling/Disabling Saving GSPS- Configuring at the System level
                // Check the web.config on the server.


                String xmlFilePath = @"C:\WebAccess\WebAccess\web.Config";
                String NodePath = "configuration/appSettings/add";
                String FirstAttribute = "key";
                String AttValue = "Application.EnableSavingGSPS";
                String SecondAttribute = "value";

                //Restart issreset 
                ServiceTool st = new ServiceTool();
                st.RestartIISUsingexe();


                String secondAttValue = login.GetAttributeValue(xmlFilePath, NodePath, FirstAttribute, AttValue, SecondAttribute);


                //By default Application.EnableSavingGSPS is set to true.
                if (secondAttValue.Equals("true"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-23
                //Log out and on the server set Application.EnableSavingGSPS in the Web.config to false.
                //Perform an iisreset.

                String secAttValue = "false";
                login.SetAttributeValue(xmlFilePath, NodePath, FirstAttribute, AttValue, SecondAttribute, secAttValue);


                //Application.EnableSavingGSPS in the Web.config to false.
                String Current_AttValue = login.GetAttributeValue(xmlFilePath, NodePath, FirstAttribute, AttValue, SecondAttribute);

                if (Current_AttValue.Equals("false"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-24
                //Log in as a user within the test domain and load a study.

                login.DriverGoTo(login.url);
                login.LoginIConnect(User1, User1);
                study = (Studies)login.Navigate("Studies");
                study.SearchStudy(AccessionNo: AccessionNumber[1], Datasource: DS_77);
                study.SelectStudy("Accession", AccessionNumber[1]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);


                //The Save tool is not available.
                bool step_24 = false;
                try { saveSeries = viewer.GetReviewTool("Save Series"); }
                catch (NoSuchElementException e) { step_24 = true; }

                if (step_24)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-25
                //Log out and log in as the Administrator. Check the settings for the test domain.

                study.CloseStudy();
                login.Logout();



                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain(SuperAdminDomain);
                domain.SelectDomain(SuperAdminDomain);
                domain.ClickEditDomain();

                //Element not visible error may come ** Remove after debug

                //The option to Enable Saving GSPS is not listed.

                bool step_25 = false;
                try { domain.SaveGspsCB(); }
                catch (NoSuchElementException e) { step_25 = true; }

                if (step_25)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                domain.ClickCloseEditDomain();

                //Step-26
                //Create a new domain.

                domain.CreateDomain(NewDomain, NewRole, datasources: new String[] { DS_77 });

                //While creating the domain, the option to Enable Saving GSPS is not listed.

                bool step_26 = false;
                try { domain.SaveGspsCB(); }
                catch (NoSuchElementException e) { step_26 = true; }

                if (step_26)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(10);
                domain.ClickSaveDomain();
                login.Logout();

                //Step-27
                //Log out and on the server set Application.EnableSavingGSPS in the Web.config to true.
                //Perform an iisreset.

                secAttValue = "true";
                login.SetAttributeValue(xmlFilePath, NodePath, FirstAttribute, AttValue, SecondAttribute, secAttValue);

                //Restart issreset
                st = new ServiceTool();
                st.RestartIISUsingexe();


                //get the value
                secondAttValue = login.GetAttributeValue(xmlFilePath, NodePath, FirstAttribute, AttValue, SecondAttribute);

                //Application.EnableSavingGSPS in the Web.config to true.
                if (secondAttValue.Equals("true"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-28
                //Log in as a user within the test domain and load a study.

                login.DriverGoTo(login.url);
                login.LoginIConnect(User1, User1);
                study = (Studies)login.Navigate("Studies");
                //AccessionNumber[2] =""
                study.SearchStudy(AccessionNo: AccessionNumber[2], Datasource: DS_77);
                study.SelectStudy("Accession", AccessionNumber[2]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //The Save tool is available again.

                saveSeries = viewer.GetReviewTool("Save Series");
                if (saveSeries.Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-29
                //Apply various tools and select Save *^>^* Save Series.


                //Apply W/L
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                result.steps[++ExecutedSteps].SetPath(testid + "_29_1_WL_Applied", ExecutedSteps + 1);
                bool status29_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());


                //Apply Pan
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                result.steps[ExecutedSteps].SetPath(testid + "_29_2_Pan_Applied", ExecutedSteps + 1);
                bool status29_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                int ThumbnailCountBeforeSave_FifthStudy = viewer.Thumbnails().Count;

                //Select Save
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SaveSeries);
                
                PageLoadWait.WaitForLoadingIconToAppear_Savestudy();
                PageLoadWait.WaitForLoadingIconToDisAppear_Savestudy();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ThumbnailCaptions()[0]));

                //The changes are saved to a PR series.

                if (status29_1 && status29_2 &&
                  viewer.Thumbnails().Count == (ThumbnailCountBeforeSave_FifthStudy + 1) &&
                  viewer.ThumbnailCaptions()[0].Text.Contains("PR"))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-30
                //Log out and log in as the Administrator. Check the settings for the test domain.
                study.CloseStudy();
                login.Logout();

                //NewDomain = "NewDomain_2";
                //NewRole = "NewRole_2";


                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain(SuperAdminDomain);
                domain.SelectDomain(SuperAdminDomain);
                domain.ClickEditDomain();


                //The option to Enable Saving GSPS is listed.

                if (domain.SaveGspsCB().Displayed == true && domain.SaveGspsCB().Selected == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                domain.ClickCloseEditDomain();

                //Step-31

                //Create a new domain.
                NewDomain = "NewDomain_2" + random;
                NewRole = "NewRole_2" + random;

                domain.CreateDomain(NewDomain, NewRole, datasources: null);

                //While creating the domain, the option to Enable Saving GSPS is listed.
                if (domain.SaveGspsCB().Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(10);
                domain.ClickSaveDomain();


                //Step-32
                //Test Data- 
                //Configuring at the System level - Scoping with PR series
                //Log in and load a study with a series that contains multiple images and set the scope to image.

                study = (Studies)login.Navigate("Studies");
                //AccessionNumber[3] =""
                study.SearchStudy(AccessionNo: AccessionNumber[3], Datasource: DS_77);
                study.SelectStudy("Accession", AccessionNumber[3]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageScope);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);


                //The study loads and the scope is set.
                if (viewer.GetReviewToolImage("Image Scope").GetAttribute("class").Contains("toggleItem_On") == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-33

                //Apply zoom, pan, rotate, and flip to multiple images so that their orientations 
                //and zoom factors are different.

                //Apply Zoom
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                result.steps[++ExecutedSteps].SetPath(testid + "_33_1_Zoom_Applied", ExecutedSteps + 1);
                bool status33_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                //bool ImageNo33_1 = viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("1");
                //Apply Pan

                viewer.ClickDownArrowbutton(1, 1);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                result.steps[ExecutedSteps].SetPath(testid + "_33_2_Pan_Applied", ExecutedSteps + 1);
                bool status33_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                bool ImageNo33_2 = viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("2");

                //Apply Rotate
                viewer.ClickDownArrowbutton(1, 1);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                result.steps[ExecutedSteps].SetPath(testid + "_33_3_Rotate_Applied", ExecutedSteps + 1);
                bool status33_3 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                bool ImageNo33_3 = viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("3");

                //Apply W/L
                viewer.ClickDownArrowbutton(1, 1);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                result.steps[ExecutedSteps].SetPath(testid + "_33_4_WL_Applied", ExecutedSteps + 1);
                bool status33_4 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                bool ImageNo33_4 = viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("4");

                //The tools are applied.

                if (status33_1 && status33_2 && status33_3 && status33_4 &&
                                  ImageNo33_2 && ImageNo33_3 && ImageNo33_4)
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


                //Step-34
                //Select Save *^>^* Save Series.
                int ThumbnailCountBeforeSave_SixthStudy = viewer.Thumbnails().Count;

                viewer.SelectToolInToolBar(IEnum.ViewerTools.SaveSeries);

                PageLoadWait.WaitForLoadingIconToAppear_Savestudy();
                PageLoadWait.WaitForLoadingIconToDisAppear_Savestudy();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ThumbnailCaptions()[0]));

                //The applied changes are saved to a PR series.

                if (viewer.Thumbnails().Count == (ThumbnailCountBeforeSave_SixthStudy + 1) &&
                  viewer.ThumbnailCaptions()[0].Text.Contains("PR"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-35
                //Load another study then reload the one with the PR series and display it.

                study.CloseStudy();
                study.SearchStudy(AccessionNo: AccessionNumber[4], Datasource: DS_77);
                study.SelectStudy("Accession", AccessionNumber[4]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                bool step35 = viewer.SeriesViewer_1X1().Displayed && viewer.studyPanel().Displayed;

                study.CloseStudy();

                study.SearchStudy(AccessionNo: AccessionNumber[3], Datasource: DS_77);
                study.SelectStudy("Accession", AccessionNumber[3]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //The study loads and the PR series is displayed.


                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status35 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status35 && step35 &&
                    viewer.Thumbnails().Count == (ThumbnailCountBeforeSave_SixthStudy + 1) &&
                    viewer.ThumbnailCaptions()[0].Text.Contains("PR"))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-36
                //Scroll through the PR series.

                //Check Zoom
                result.steps[++ExecutedSteps].SetPath(testid + "_36_1_Zoom_Applied", ExecutedSteps + 1);
                bool status36_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                //bool ImageNo36_1 = viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("1");

                //Check Pan
                viewer.ClickDownArrowbutton(1, 1);
                result.steps[ExecutedSteps].SetPath(testid + "_36_2_Pan_Applied", ExecutedSteps + 1);
                bool status36_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                bool ImageNo36_2 = viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("2");

                //Check Rotate
                viewer.ClickDownArrowbutton(1, 1);
                result.steps[ExecutedSteps].SetPath(testid + "_36_3_Rotate_Applied", ExecutedSteps + 1);
                bool status36_3 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                bool ImageNo36_3 = viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("3");

                //Check W/L
                viewer.ClickDownArrowbutton(1, 1);
                result.steps[ExecutedSteps].SetPath(testid + "_36_4_WL_Applied", ExecutedSteps + 1);
                bool status36_4 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                bool ImageNo36_4 = viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("4");


                //The changes that were made prior to saving are displayed.

                if (status36_1 && status36_2 && status36_3 && status36_4 &&
                                ImageNo36_2 && ImageNo36_3 && ImageNo36_4)
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

                //Step-37
                //Change the image layout to one with multiple images (i.e. 2x2).

                for (int i = 0; i < 4; i++)
                    viewer.ClickUpArrowbutton(1, 1);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x2);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);


                result.steps[++ExecutedSteps].SetPath(testid + "_37_Z_Pan_Rot_WL_Applied", ExecutedSteps + 1);
                bool status37 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                //The layout is changed and the images are displayed the same as they were saved.


                if (status37 && viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "layoutFormat").Equals("2x2"))
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

                //Step-38
                //Apply a tool (Zoom, Pan, W/L, etc.) to one of the images.in series scope

                viewer.SeriesViewer_1X2().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesScope);

                //Apply Zoom
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                IWebElement viewport2 = viewer.SeriesViewer_1X2();
                h = viewport2.Size.Height;
                w = viewport2.Size.Width;
                viewer.DragandDropImage(viewport2, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                //Apply Rotate
                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);


                //Apply Pan              
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                viewport2 = viewer.SeriesViewer_1X2();
                h = viewport2.Size.Height;
                w = viewport2.Size.Width;
                viewer.DragandDropImage(viewport2, w / 4, h / 4, 3 * w / 4, 3 * h / 4);



                //Apply W/L
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                viewport2 = viewer.SeriesViewer_1X2();
                h = viewport2.Size.Height;
                w = viewport2.Size.Width;
                viewer.DragandDropImage(viewport2, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                result.steps[++ExecutedSteps].SetPath(testid + "_38_1_Z_Pan_Rot_WL_Applied", ExecutedSteps + 1);
                bool status38_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                //Click down arrow second viewport
                viewer.ClickDownArrowbutton(1, 2);

                //The tool is applied and the other images that are displayed are updated to match the presentation state of the one selected.

                result.steps[ExecutedSteps].SetPath(testid + "_38_2_Z_Pan_Rot_WL_Applied", ExecutedSteps + 1);
                bool status38_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                if (status38_1 && status38_2 &&
                    viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("2"))
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

                //Step-39
                //Continue to scroll through the series.

                viewer.ClickDownArrowbutton(1, 2);

                result.steps[++ExecutedSteps].SetPath(testid + "_39_1_Z_Pan_Rot_WL_Applied", ExecutedSteps + 1);
                bool status39_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());
                bool ImageNo39_1 = viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("3");

                viewer.ClickDownArrowbutton(1, 2);

                result.steps[ExecutedSteps].SetPath(testid + "_39_2_Z_Pan_Rot_WL_Applied", ExecutedSteps + 1);
                bool status39_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());
                bool ImageNo39_2 = viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("4");

                if (status39_1 && ImageNo39_1 &&
                    status39_2 && ImageNo39_2)
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
                study.CloseStudy();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
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
                String xmlFilePath = @"C:\WebAccess\WebAccess\web.Config";
                String NodePath = "configuration/appSettings/add";
                String FirstAttribute = "key";
                String AttValue = "Application.EnableSavingGSPS";
                String SecondAttribute = "value";
                String  secAttValue = "true";
                login.SetAttributeValue(xmlFilePath, NodePath, FirstAttribute, AttValue, SecondAttribute, secAttValue);

                //Restart issreset
                ServiceTool st1 = new ServiceTool();
                st1.RestartIISUsingexe();
            }
        }



        /// <summary> 
        /// This Test Case is Verification of GSPS "Initial Setup"
        /// </summary>

        public TestCaseResult Test_27905(String testid, String teststeps, int stepcount)
        {
            //-Initial Setup--
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            String username = Config.adminUserName;
            String password = Config.adminPassword;


            String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNo");
            String[] AccessionNumber = AccessionNoList.Split(':');

            try
            {
                //Step-1
                //Load a study that contains a series with multiple images.

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                Studies study = (Studies)login.Navigate("Studies");

                study.SearchStudy(AccessionNo: AccessionNumber[0], Datasource: DS_77);
                study.SelectStudy("Accession", AccessionNumber[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //The study loads.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status1 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status1 && viewer.SeriesViewer_1X1().Displayed &&
                    viewer.studyPanel().Displayed)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-2
                //Scroll to an image in the middle of the series and
                //apply various tools (W/L, Pan, Zoom, Annotations etc.)

                IWebElement source = viewer.ViewportScrollHandle(1, 2);
                IWebElement destination = viewer.ViewportScrollBar(1, 2);

                int w = destination.Size.Width;
                int h = destination.Size.Height;

                Actions action2 = new Actions(BasePage.Driver);

                //middle position
                action2.ClickAndHold(source).MoveToElement(destination, w / 2, h / 2).Release().Build().Perform();
                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);



                //Apply W/L
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                IWebElement viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                result.steps[++ExecutedSteps].SetPath(testid + "_2_1_WL_Applied", ExecutedSteps + 1);
                bool status2_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                //Apply Zoom
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                result.steps[ExecutedSteps].SetPath(testid + "_2_2_Zoom_Applied", ExecutedSteps + 1);
                bool status2_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                //Apply Pan
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, w / 2, h / 4);

                result.steps[ExecutedSteps].SetPath(testid + "_2_3_Pan_Applied", ExecutedSteps + 1);
                bool status2_3 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                //Tools are applied.

                if (status2_1 && status2_2 && status2_3)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-3
                //From the save menu select Save Series.

                int ThumbnailCountBeforeSave_FirstStudy = viewer.Thumbnails().Count;
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SaveSeries);

                PageLoadWait.WaitForLoadingIconToAppear_Savestudy();
                PageLoadWait.WaitForLoadingIconToDisAppear_Savestudy();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ThumbnailCaptions()[0]));
                

                //The GSPS are saved and a thumbnail for the new PR series is displayed in the Series Thumbnail area.

                if (viewer.Thumbnails().Count == (ThumbnailCountBeforeSave_FirstStudy + 1) &&
                    viewer.ThumbnailCaptions()[0].Text.Contains("PR"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4
                //Close the study and check the study list.
                study.CloseStudy();
                study.SearchStudy(AccessionNo: AccessionNumber[0], Datasource: DS_77);

                //The study lists /PR after its original modality in the study list.

                Dictionary<string, string> row4 = study.GetMatchingRow(new string[] { "Accession" }, new string[] { AccessionNumber[0] });

                if (row4 != null && row4["Modality"].Contains("PR"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5
                //Load another study and re-load the first one. Select the PR series that was saved.

                study.SearchStudy(AccessionNo: AccessionNumber[1], Datasource: DS_77);
                study.SelectStudy("Accession", AccessionNumber[1]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                bool step5 = viewer.SeriesViewer_1X1().Displayed && viewer.studyPanel().Displayed;

                study.CloseStudy();

                study.SearchStudy(AccessionNo: AccessionNumber[0], Datasource: DS_77);
                study.SelectStudy("Accession", AccessionNumber[0]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //All images from the original series are present. 
                //The image that was modified is displayed the same as it was saved.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status5 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status5 && step5 &&
                    viewer.Thumbnails().Count == (ThumbnailCountBeforeSave_FirstStudy + 1) &&
                    viewer.ThumbnailCaptions()[0].Text.Contains("PR"))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                study.CloseStudy();


                //Step-6
                //Load a study and without applying any tools select Save Series from the save menu.

                study.SearchStudy(AccessionNo: AccessionNumber[2], Datasource: DS_77);
                study.SelectStudy("Accession", AccessionNumber[2]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                int ThumbnailCountBeforeSave_SecondStudy = viewer.Thumbnails().Count;

                viewer.SelectToolInToolBar(IEnum.ViewerTools.SaveSeries);

                PageLoadWait.WaitForLoadingIconToAppear_Savestudy();
                PageLoadWait.WaitForLoadingIconToDisAppear_Savestudy();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ThumbnailCaptions()[0]));

                //The unchanged series is saved to a PR series.

                if (viewer.Thumbnails().Count == (ThumbnailCountBeforeSave_SecondStudy + 1) &&
                    viewer.ThumbnailCaptions()[0].Text.Contains("PR"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7
                //Double click on the PR series thumbnail.
                var action = new Actions(BasePage.Driver);
                action.DoubleClick(viewer.Thumbnails()[0]).Build().Perform();
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);


                //The images are displayed the same as they were saved (without any changes).

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status7 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status7 &&
                    viewer.Thumbnails().Count == (ThumbnailCountBeforeSave_SecondStudy + 1) &&
                    viewer.ThumbnailCaptions()[0].Text.Contains("PR"))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                study.CloseStudy();


                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
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
        }


        /// <summary> 
        /// This Test Case is Verification of GSPS "Initial Setup"
        /// </summary>

        public TestCaseResult Test_27906(String testid, String teststeps, int stepcount)
        {
            //-Initial Setup--
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            String username = Config.adminUserName;
            String password = Config.adminPassword;


            String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNo");
            String[] AccessionNumber = AccessionNoList.Split(':');

            //String Image_count = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "imageCount");

            try
            {
                //Step-1
                //Load a study that contains a series with multiple images.

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                Studies study = (Studies)login.Navigate("Studies");

                study.SearchStudy(AccessionNo: AccessionNumber[0], Datasource: DS_77);
                study.SelectStudy("Accession", AccessionNumber[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //The study loads.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status1 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status1 && viewer.SeriesViewer_1X1().Displayed &&
                    viewer.studyPanel().Displayed)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-2
                //Apply various tools (W/L, Pan, Zoom, Flip, Annotations etc.) to the first image of the series.


                //Apply W/L
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                IWebElement viewport1 = viewer.SeriesViewer_1X2();
                int h = viewport1.Size.Height;
                int w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                result.steps[++ExecutedSteps].SetPath(testid + "_2_1_WL_Applied", ExecutedSteps + 1);
                bool status2_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                //Apply Zoom
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                viewport1 = viewer.SeriesViewer_1X2();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                result.steps[ExecutedSteps].SetPath(testid + "_2_2_Zoom_Applied", ExecutedSteps + 1);
                bool status2_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                //Apply Pan
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                viewport1 = viewer.SeriesViewer_1X2();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, w / 2, h / 4);

                result.steps[ExecutedSteps].SetPath(testid + "_2_3_Pan_Applied", ExecutedSteps + 1);
                bool status2_3 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                //Draw Line
                viewer.SelectToolInToolBar(IEnum.ViewerTools.LineMeasurement);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                var action = new Actions(BasePage.Driver);
                action.MoveToElement(viewport1, w / 3 + 30, h / 3 + 15).Click().Build().Perform();
                Thread.Sleep(3000);
                action.MoveToElement(viewport1, w / 3 + w / 6, h / 3 + h / 4).Click().Build().Perform();
                Thread.Sleep(2000);

                if (!((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("firefox"))
                {
                    action.Release(viewport1).Build().Perform();
                    Thread.Sleep(2000);
                }
                result.steps[ExecutedSteps].SetPath(testid + "_2_4_Line_Applied", ExecutedSteps + 1);
                bool status2_4 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());



                //Tools are applied.

                if (status2_1 && status2_2 && status2_3 && status2_4)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-3
                //Scroll to the last image in the series.


                IWebElement source = viewer.ViewportScrollHandle(1, 2);
                IWebElement destination = viewer.ViewportScrollBar(1, 2);

                w = destination.Size.Width;
                h = destination.Size.Height;

                Actions action3 = new Actions(BasePage.Driver);

                //middle position
                action3.ClickAndHold(source).MoveToElement(destination, w / 2, h).Release().Build().Perform();
                Thread.Sleep(4000);

                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                result.steps[++ExecutedSteps].SetPath(testid + "_3_LastImage", ExecutedSteps + 1);
                bool status3 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                //The last image is displayed.
                if (status3 &&
                    viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals(null) == false)//img no updated in DOM
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4
                //Apply various tools (W/L, Pan, Zoom, Rotate, Annotations etc.) to the last image.

                int ThumbnailCountBeforeSave_FirstStudy = viewer.Thumbnails().Count;



                //Apply W/L
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                viewport1 = viewer.SeriesViewer_1X2();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                result.steps[++ExecutedSteps].SetPath(testid + "_4_1_WL_Applied", ExecutedSteps + 1);
                bool status4_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                //Apply Pan
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                viewport1 = viewer.SeriesViewer_1X2();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, 3 * w / 4, 3 * h / 4, w / 4, h / 4);

                result.steps[ExecutedSteps].SetPath(testid + "_4_2_Pan_Applied", ExecutedSteps + 1);
                bool status4_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());


                //Apply Rotate
                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                result.steps[ExecutedSteps].SetPath(testid + "_4_3_Rotate_Applied", ExecutedSteps + 1);
                bool status4_3 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                //Apply Zoom
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                viewport1 = viewer.SeriesViewer_1X2();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                result.steps[ExecutedSteps].SetPath(testid + "_4_4_Zoom_Applied", ExecutedSteps + 1);
                bool status4_4 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());




                //Draw Line
                viewer.SelectToolInToolBar(IEnum.ViewerTools.LineMeasurement);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                action = new Actions(BasePage.Driver);
                action.MoveToElement(viewport1, w / 3 + 30, h / 3 + 15).Click().Build().Perform();
                Thread.Sleep(3000);
                action.MoveToElement(viewport1, w / 3 + w / 6, h / 3 + h / 4).Click().Build().Perform();
                Thread.Sleep(2000);

                if (!((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("firefox"))
                {
                    action.Release(viewport1).Build().Perform();
                    Thread.Sleep(2000);
                }
                result.steps[ExecutedSteps].SetPath(testid + "_4_5_Line_Applied", ExecutedSteps + 1);
                bool status4_5 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());



                if (status4_1 && status4_2 && status4_3 && status4_4)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5
                //From the save menu select Save Annotated Images.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.SaveAnnotatedImages);
                PageLoadWait.WaitForLoadingIconToAppear_Savestudy();
                PageLoadWait.WaitForLoadingIconToDisAppear_Savestudy();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ThumbnailCaptions()[0]));

                //The GSPS are saved and a thumbnail for the new PR series is displayed in the Series Thumbnail area.

                if (viewer.Thumbnails().Count == (ThumbnailCountBeforeSave_FirstStudy + 1) &&
                    viewer.ThumbnailCaptions()[0].Text.Contains("PR"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6
                //Close the study and check the study list.

                study.CloseStudy();
                study.SearchStudy(AccessionNo: AccessionNumber[0], Datasource: DS_77);

                //The study lists /PR after its original modality in the study list.
                Dictionary<string, string> row4 = study.GetMatchingRow(new string[] { "Accession" }, new string[] { AccessionNumber[0] });

                if (row4 != null && row4["Modality"].Contains("PR"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-7
                //Load another study and re-load the first one. Select the PR series that was saved.

                //AccessionNumber[1]=MS10000
                study.SearchStudy(AccessionNo: AccessionNumber[1], Datasource: DS_77);
                study.SelectStudy("Accession", AccessionNumber[1]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                bool step7 = viewer.SeriesViewer_1X1().Displayed && viewer.studyPanel().Displayed;

                study.CloseStudy();

                study.SearchStudy(AccessionNo: AccessionNumber[0], Datasource: DS_77);
                study.SelectStudy("Accession", AccessionNumber[0]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);


                //Only the two images that had annotations applied to them (the first and last image) are present. 
                //The images are displayed the same as they were saved.


                result.steps[++ExecutedSteps].SetPath(testid + "_7_1_First_Image_updated", ExecutedSteps + 1);
                bool status7_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_7_2_Second_Image_updated", ExecutedSteps + 1);
                bool status7_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());


                if (status7_1 && status7_2 && step7 &&
                     viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("2"))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                study.CloseStudy();

                //Step-8

                //Load a study and without applying any tools select Save Annotated Images from the save menu.

                //AccessionNumber[1]=""
                study.SearchStudy(AccessionNo: AccessionNumber[1], Datasource: DS_77);
                study.SelectStudy("Accession", AccessionNumber[1]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.SaveAnnotatedImages);

                //A warning icon is displayed on the left of the toolbar.
                if (viewer.SaveFailedImage().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-9
                //Hover over the warning icon.

                viewer.JSMouseHover(viewer.SaveFailedImage());


                //A tooltip is displayed stating that there are no annotations in the series.

                if (viewer.SaveFailedImage().GetAttribute("title").Equals("No annotations in the series.") &&
                    viewer.SaveFailedImage().GetAttribute("alt").Equals("No annotations in the series."))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10

                //Apply an annotation and select Save Annotated Images from the save menu again.

                int ThumbnailCountBeforeSave_SecondStudy = viewer.Thumbnails().Count;

                //Draw Line
                viewer.SelectToolInToolBar(IEnum.ViewerTools.LineMeasurement);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                action = new Actions(BasePage.Driver);
                action.MoveToElement(viewport1, w / 3 + 30, h / 3 + 15).Click().Build().Perform();
                Thread.Sleep(3000);
                action.MoveToElement(viewport1, w / 3 + w / 6, h / 3 + h / 4).Click().Build().Perform();
                Thread.Sleep(2000);

                if (!((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("firefox"))
                {
                    action.Release(viewport1).Build().Perform();
                    Thread.Sleep(2000);
                }
                result.steps[++ExecutedSteps].SetPath(testid + "_10_Line_Applied", ExecutedSteps + 1);
                bool status10 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());


                viewer.SelectToolInToolBar(IEnum.ViewerTools.SaveAnnotatedImages);
                PageLoadWait.WaitForLoadingIconToAppear_Savestudy();
                PageLoadWait.WaitForLoadingIconToDisAppear_Savestudy();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ThumbnailCaptions()[0]));

                //This time the GSPS are saved and a thumbnail for the new PR series is displayed 
                //in the Series Thumbnail area.

                if (status10 &&
                    viewer.Thumbnails().Count == (ThumbnailCountBeforeSave_SecondStudy + 1) &&
                    viewer.ThumbnailCaptions()[0].Text.Contains("PR"))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
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
        }

    }
}
