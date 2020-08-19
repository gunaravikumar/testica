using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MergeServiceTool;
using Selenium.Scripts.Reusable.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Selenium.Scripts.Tests
{
    class BluringCardioCINE : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }

        public string EA_91 = null;
        public string EA_131 = null;
        public string PACS_A7 = null;
        public string EA_77 = null;

        DomainManagement domainmanagement = null;
        RoleManagement rolemanagement = null;
        UserManagement usermanagement = new UserManagement();
        Studies studies = null;
        UserPreferences userpreference = null;
        BluRingViewer viewer = null;

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        public BluringCardioCINE(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            EA_91 = login.GetHostName(Config.EA91);
            EA_77 = login.GetHostName(Config.EA77);
            EA_131 = login.GetHostName(Config.EA1);
            PACS_A7 = login.GetHostName(Config.SanityPACS);

            domainmanagement = new DomainManagement();
            rolemanagement = new RoleManagement();
            usermanagement = new UserManagement();
            studies = new Studies();
            userpreference = new UserPreferences();
            viewer = new BluRingViewer();
        }

        /// <summary> 
        /// Previous/Next Series
        /// </summary>

        public TestCaseResult Test_162368(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                BasePage.SetVMResolution("1980", "1080");
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                
                String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = AccessionNoList.Split(':');

                DomainManagement domain = new DomainManagement();
                RoleManagement role = new RoleManagement();
                UserManagement user = new UserManagement();
                Studies studies = new Studies();
                UserPreferences userpref = new UserPreferences();
                BluRingViewer viewer = new BluRingViewer();

                //Step 1 - Login to BluRing/iCA Enterprise Viewer as a privilege user (i.e., Administrator/Administrator) 
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login = new Login();
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //Step 2 - In User preferences set the thumbnail split to series for the modality to which the listed study belongs
                //         Search and load for a study with multiple images in multiple series                
                studies = (Studies)login.Navigate("Studies");
                studies.OpenUserPreferences();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                domain.ModalityDropDown().SelectByText("MR");
                domain.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                userpref.BluringViewerRadioBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                studies.CloseUserPreferences();
                
                studies.SearchStudy(AccessionNo: AccessionNumbers[0], Datasource: EA_91);
                studies.SelectStudy("Accession", AccessionNumbers[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                if (viewer.studyPanel().Displayed)
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

                //Step 3 - Load first series in first viewport, second series in second viewport and so on
                viewer.SetSeriesInViewport(0, 1);
                viewer.SetSeriesInViewport(1, 1);
                viewer.SetSeriesInViewport(2, 1);
                ExecutedSteps++;

                //Step 4 - Verify the Cardio Cine Tool should not display in all viewports by default.
                IList<IWebElement> PauseBtn = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_CINE_PauseBtn));
                IList<IWebElement> PlayBtn = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_CINE_PlayBtn));
                IList<IWebElement> NextBtn = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_CINE_NextImageBtn));
                IList<IWebElement> PrevBtn = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_CINE_PreviousImageBtn));
                bool res_4 = true;
                for (int i = 0; i < PlayBtn.Count; i++)
                {
                    if (PlayBtn[i].Displayed || NextBtn[i].Displayed || PrevBtn[i].Displayed)
                    {
                        res_4 = false;
                        break;
                    }
                }

                if (res_4 && PauseBtn.Count == 0 && PlayBtn.Count == NextBtn.Count && NextBtn.Count == PrevBtn.Count)
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


                //Step 5 - Click on "Play Series" button
                if(viewer.PlayCINE(1, 1))
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

                //Step 6 - Verify the images being played
                if ( viewer.IsCINEPlaying(1, 1) )
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 7 - Click Next Series button                
                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 1)).Click();
                viewer.ClickNextSeriesCINE(1, 1);
                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 1)).Click();
                if (viewer.IsCINEPlaying(1, 1) && BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 2))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 8 - Click Previous Series button                
                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 1)).Click();
                viewer.ClickPreviousSeriesCINE(1, 1);
                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 1)).Click();
                if (viewer.IsCINEPlaying(1, 1) && BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 9 - Click Next Series button 
                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 1)).Click();
                viewer.ClickNextSeriesCINE(1, 1);
                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 1)).Click();
                if (viewer.IsCINEPlaying(1, 1) && BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 2))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 10 - Click Previous Series button                
                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 1)).Click();
                viewer.ClickPreviousSeriesCINE(1, 1);
                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 1)).Click();
                if (viewer.IsCINEPlaying(1, 1) && BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 11 - Pause cine and click Next Series
                viewer.PauseCINE(1, 1);
                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 1)).Click();
                viewer.ClickNextSeriesCINE(1, 1);
                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 1)).Click();
                if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 2))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 12 - Skeep clicking next Series till last series is loaded in viewport click next Series one more time
                int count = BluRingViewer.NumberOfThumbnailsInStudyPanel();

                for (int i = 0; i < count - 1; i++)
                {
                    viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 1)).Click();
                    viewer.ClickNextSeriesCINE(1, 1);
                   // viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 1)).Click();
                }
                if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 13 - click previous Series.
                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 1)).Click();
                viewer.ClickPreviousSeriesCINE(1, 1);
                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 1)).Click();
                if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, BluRingViewer.NumberOfThumbnailsInStudyPanel()))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Close viewer and logout
                viewer.CloseBluRingViewer();
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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
                return result;
            }
            finally
            {
                BasePage.SetVMResolution("1280", "1024");
            }
        }

        /// <summary> 
        /// Cine doesn’t appear in empty viewports or series that has one image / frame
        /// </summary>

        public TestCaseResult Test_160888(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {                
                String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = AccessionNoList.Split(':');

                //Step 1 - Login to iCA Enterprise Viewer as a privilege user (i.e., Administrator/Administrator) 
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                ExecutedSteps++;

                //Step 2 - In User preferences set the thumbnail split to series for the modality to which the listed study belongs and click on "Save" button.                                
                userpreference.OpenUserPreferences();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");            
                String[] Modality = { "CR", "CT", "MR" };
                foreach (String s in Modality)
                {
                    userpreference.ModalityDropDown().SelectByText(s);
                    userpreference.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                    PageLoadWait.WaitForPageLoad(20);                    
                }
                userpreference.CloseUserPreferences();
                ExecutedSteps++;

                //Step 3 - Search and load for a study with 2 or 3 series(e.g. 3 series) that contains multiple images
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: AccessionNumbers[0], Datasource: EA_91);
                studies.SelectStudy("Accession", AccessionNumbers[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                if (GetElement(SelectorType.CssSelector, BluRingViewer.div_StudyPanel).Displayed)
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

                //step 4 - Ensure that the first series should be loaded in the first series viewport, second series in second series viewport and third series in third series viewport.                                
                viewer.SetSeriesInViewport(0, 1);                
                viewer.SetSeriesInViewport(1, 1);                
                viewer.SetSeriesInViewport(2, 1);

                var step4 = result.steps[++ExecutedSteps];
                step4.SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(step4, GetElement(SelectorType.CssSelector, BluRingViewer.div_studypanel)))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 5 - Mouse hover on the bottom part on the first series viewport and verify the Cardio Cine Tool should get appear.
                viewer.CloseBluRingViewer();
                studies.SearchStudy(AccessionNo: AccessionNumbers[0], Datasource: EA_91);
                studies.SelectStudy("Accession", AccessionNumbers[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                if (viewer.VerifyCardioCINEToolbarOnMouseHover(1, 1))
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

                //step 6 - Mouse hover on the bottom part on the second series viewport and verify the Cardio Cine Tool should get appear.
                if (viewer.VerifyCardioCINEToolbarOnMouseHover(2, 1))
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

                //Step 7 - Mouse hover on the bottom part on the third series viewport and verify the Cardio Cine Tool should get appear.               
                if (viewer.VerifyCardioCINEToolbarOnMouseHover(3, 1))
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

                //Step 8 - Mouse hover on the bottom part on the fourth series viewport and verify the Cardio Cine Tool should get appear.
                if (!viewer.VerifyCardioCINEToolbarOnMouseHover(4, 1))
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

                //Step 9 - Click Right Mouse Button on cine area and verify that the user shall open the toolbox
                GetElement(SelectorType.CssSelector, viewer.SetViewPort(0, 1)).Click();
                IWebElement StackedTool = viewer.OpenStackedTool(BluRingTools.Draw_Ellipse);                
                if (StackedTool != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 10 - Select any tool from the floating toolbox and apply it.                
                viewer.SelectViewerTool(BluRingTools.Draw_Ellipse);
                viewer.ApplyTool_DrawEllipse();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step10 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(SelectorType.CssSelector, viewer.Activeviewport));
                if (step10)
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

                //Step 11 - Click on 'EXIT' button and Navigate to Studies tab
                viewer.CloseBluRingViewer();
                if (login.IsTabPresent("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 12 -  Load any study which has three series that contains First and second series has multiple images and third series with single image/frame.
                studies.SearchStudy(AccessionNo: AccessionNumbers[1], Datasource: EA_91);
                studies.SelectStudy("Accession", AccessionNumbers[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                if (GetElement(SelectorType.CssSelector, BluRingViewer.div_StudyPanel).Displayed)
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

                //Step 13 - Mouse hover on the bottom part on the first series viewport and verify the Cardio Cine Tool should get appear.
                if (viewer.VerifyCardioCINEToolbarOnMouseHover(1, 1))
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

                //Step 14 - Mouse hover on the bottom part on the second series viewport and verify the Cardio Cine Tool should get appear.
                if (viewer.VerifyCardioCINEToolbarOnMouseHover(2, 1))
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

                //Step 15 - Mouse hover on the bottom part on the third series viewport and verify the Cardio Cine Tool should get appear.
                if (!viewer.VerifyCardioCINEToolbarOnMouseHover(3, 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 16 - Click Right Mouse Button on cine area and verify that the user shall open the toolbox
                GetElement(SelectorType.CssSelector, viewer.SetViewPort(0, 1)).Click();
                StackedTool = viewer.OpenStackedTool(BluRingTools.Draw_Ellipse);
                if (StackedTool != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 17 - Select any tool from the floating toolbox and apply it.
                viewer.SelectViewerTool(BluRingTools.Draw_Ellipse);
                viewer.ApplyTool_DrawEllipse();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step17 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(SelectorType.CssSelector, viewer.Activeviewport));
                if (step17)
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

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
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
                return result;
            }
            finally
            {
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);                               
                userpreference.OpenUserPreferences();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                String[] Modality = { "CR"};
                foreach (String s in Modality)
                {
                    userpreference.ModalityDropDown().SelectByText(s);
                    userpreference.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                    PageLoadWait.WaitForPageLoad(20);
                }
                userpreference.CloseUserPreferences();
                login.Logout();
            }
        }

        /// <summary> 
        /// Cardio FPS Slider : Study with many series
        /// </summary>
        public TestCaseResult Test_160891(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            Stopwatch stopwatch = new Stopwatch();
            TimeSpan timeout = new TimeSpan(0, 2, 0);
            IList<int> FPSValue = null;
            try
            {
                BasePage.SetVMResolution("1980", "1080");
                String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = AccessionNoList.Split(':');

                //Preconditions: In User Preferences window,
                //1.Set default viewer as " BluRing ", and
                //2.Set Cine Default Frame Rate as 20 FPS
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login = new Login();
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                userpreference.OpenUserPreferences();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreference.CineDefaultFrameRate().Clear();
                userpreference.CineDefaultFrameRate().SendKeys("20");
                userpreference.CloseUserPreferences();
                login.Logout();

                //Step 1 - Login to iCA Enterprise Viewer as a privilege user (i.e., Administrator/Administrator) 
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);                
                ExecutedSteps++;

                //Step 2 - Search study which has many series(e.g. 4 series) with multiple images, and load into the Enterprise Viewer.                                                
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: AccessionNumbers[0], Datasource: EA_91);
                studies.SelectStudy("Accession", AccessionNumbers[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                if (GetElement(SelectorType.CssSelector, BluRingViewer.div_StudyPanel).Displayed)
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

                //Step 3 - Click on "Play series" button
                if (viewer.PlayCINE(1, 1))
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

                //Step 4 - Verify the cine plays in all the images from the selected series viewport and never stops when the default FPS value reaches in the FPS slider text box.                                
                if (viewer.IsCINEPlaying(1, 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 5 - while playing cine, Left click on FPS Slider and change fps value as (e.g. 10) by dragging the orange slider up or down                
                int fps_val = viewer.SetFPSValue("10", 1);
                ExecutedSteps++;

                //Step 6 - Verify requested fps value in the fps box while playing cine
                FPSValue = viewer.GetFPSValueInList(1, 1);
                if (FPSValue.All(v => (v >= 1) && (v <= fps_val)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 7 - Click on "Pause series" button
                if (viewer.PauseCINE(1, 1))
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

                //Step 8 - Click on "Play series" button
                if (viewer.PlayCINE(2, 1))
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

                //Step 9 - Verify the cine plays in all the images from the selected series viewport and never stops when the default FPS value reaches in the FPS box.
                if (viewer.IsCINEPlaying(2, 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 10 - while playing cine, Left click on FPS Slider and change fps value (e.g. 25) by dragging the orange slider up or down
                fps_val = viewer.SetFPSValue("25", 2);
                ExecutedSteps++;

                //Step 11 - Verify fps value in fps box while playing cine
                FPSValue = viewer.GetFPSValueInList(2, 1);
                if (FPSValue.All(v => (v >= 1) && (v <= fps_val)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 12 - Click on "Pause series" button
                if (viewer.PauseCINE(2, 1))
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

                //Step 13 - Click on "Play series" button
                if (viewer.PlayCINE(3, 1))
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

                //Step 14 - Verify the cine plays in all the images from the selected series viewport and never stops when the requested FPS value reaches in the FPS slider text box.
                if (viewer.IsCINEPlaying(3, 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 15 - while playing cine, Left click on FPS Slider and change fps value as 30 by dragging the orange slider up or down
                fps_val = viewer.SetFPSValue("30", 3);
                ExecutedSteps++;

                //Step 16 - Verify fps value while playing cine
                FPSValue = viewer.GetFPSValueInList(3, 1);
                if (FPSValue.All(v => (v >= 1) && (v <= fps_val)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 17 - Click on "Pause series" button
                if (viewer.PauseCINE(3, 1))
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

                //Step 18 - Click on "Play series" button
                if (viewer.PlayCINE(4, 1))
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

                //Step 19 - Verify the cine plays in all the images from the selected series viewport and never stops when the requested FPS value reaches in the FPS slider text box.
                if (viewer.IsCINEPlaying(4, 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 20 - while playing cine, Left click on FPS Slider and change fps value as 60 by dragging scrollbar
                fps_val = viewer.SetFPSValue("60", 4);
                ExecutedSteps++;

                //Step 21 - Verify fps value while playing cine
                FPSValue = viewer.GetFPSValueInList(4, 1);
                if (FPSValue.All(v => (v >= 1) && (v <= fps_val)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout
                viewer.CloseBluRingViewer();
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
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
                return result;
            }
            finally
            {
                BasePage.SetVMResolution("1280", "1024");
            }
        }

        /// <summary> 
        /// Test 141450 - Cine tool shall appear studies which has priors
        /// </summary>
        public TestCaseResult Test_160892(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            Studies studies = null;
            BluRingViewer viewer = new BluRingViewer();
            Stopwatch stopwatch = new Stopwatch();
            TimeSpan timeout = new TimeSpan(0, 2, 0);
            bool[] allplaypausecine = null;
            try
            {
				BasePage.SetVMResolution("1980", "1080");
                string[] Accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList")).Split('=');
                string PatientID = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList"));
                string Datasource = EA_91;
                //Step 1: Launch the iCA enterprise application with a client browser
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login = new Login();
                login.DriverGoTo(login.url);
                // result.steps[++ExecutedSteps].status = "Pass";
                //Step 2: Login to WebAccess site with any privileged user.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
               // result.steps[++ExecutedSteps].status = "Pass";
                
				//Step 1: Load the uploaded study in the iCA Universal viewer
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], patientID: PatientID, Datasource: Datasource);
                PageLoadWait.WaitForSearchLoad();
                studies.SelectStudy("Accession", Accession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                if (viewer.studyPanel().Displayed)
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
                //Step 2: Verify all the priors are listed in exam list.
                if (viewer.CheckAccession_ExamList(Accession))
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
                //Step 3: Launch one prior study which has more than 1 series with multiple images from exam list by single clicking on studies card and launch the prior study in viewer
                viewer.OpenPriors(accession: Accession[1]);
                PageLoadWait.WaitForBluRingViewportToLoad(panel: 2, viewport: 1);
                PageLoadWait.WaitForBluRingViewportToLoad(panel: 2, viewport: 2);
                PageLoadWait.WaitForBluRingViewportToLoad(panel: 2, viewport: 3);
                PageLoadWait.WaitForBluRingViewportToLoad(panel: 2, viewport: 4);
                if (viewer.studyPanel(2).Displayed)
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
                
                //Step 4: Click on "Play All in Study" button
                viewer.ClickPlayAllOrPauseAll("PlayAll", 1, 2);
                allplaypausecine = Enumerable.Repeat(false, 4).ToArray();
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        allplaypausecine[i] = viewer.IsCINEPlaying(i + 1, 2);
                    }
                    if (allplaypausecine.All(apc => apc == true))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (allplaypausecine.All(apc => apc == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 6: Cine Play All Status for Panel 2 is [" + string.Join(",", allplaypausecine) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 5: Again click on "Play All in Study" button
                viewer.ClickPlayAllOrPauseAll("PauseAll", 1, 2);
                allplaypausecine = Enumerable.Repeat(false, 4).ToArray();
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        allplaypausecine[i] = !viewer.IsCINEPlaying(i + 1, 2);
                    }
                    if (allplaypausecine.All(apc => apc == true))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (allplaypausecine.All(apc => apc == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 7: Cine Pause All Status for Panel 2 is [" + string.Join(",", allplaypausecine) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 6: Click on "Play series" button and verify the cine plays all images from the selected series and never stops
                if (viewer.PlayCINE(1, 2))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 7: Click on "Pause series" button
                if (viewer.PauseCINE(1, 2))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 8: Click on 'Prev Series' button
                //viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 2)).Click();
                viewer.ClickPreviousSeriesCINE(2, 2);
                //viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 2)).Click();
                if (viewer.IsCINEPlaying(2, 2) && BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(2, 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 9: Click on 'Next Series' button
                //viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 2)).Click();
                viewer.ClickNextSeriesCINE(2, 2);
                //viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 2)).Click();
                if (viewer.IsCINEPlaying(2, 2) && BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(2, 2))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 10:
                //viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(1, 2)).Click();
                viewer.ClickPlayAllOrPauseAll("PauseAll", 2, 2);
                allplaypausecine = Enumerable.Repeat(false, 4).ToArray();
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        allplaypausecine[i] = !viewer.IsCINEPlaying(i + 1, 2);
                    }
                    if (allplaypausecine.All(apc => apc == true))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (allplaypausecine.All(apc => apc == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 10: Cine Play All Status for Panel 2 is [" + string.Join(",", allplaypausecine) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 11: 
                //viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(1, 2)).Click();
                viewer.ClickPlayAllOrPauseAll("PlayAll", 2, 2);
                allplaypausecine = Enumerable.Repeat(false, 4).ToArray();
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        allplaypausecine[i] = viewer.IsCINEPlaying(i + 1, 2);
                    }
                    if (allplaypausecine.All(apc => apc == true))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (allplaypausecine.All(apc => apc == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 11: Cine Play All Status for Panel 2 is [" + string.Join(",", allplaypausecine) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 12: Click on 'Prev Series' button
                viewer.ClickPreviousSeriesCINE(2, 2);
                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(1, 2)).Click();
                if (viewer.IsCINEPlaying(2, 2) && BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(2, 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 13: Click on "Pause series" button
                if (viewer.PauseCINE(2, 2))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 14: From the Exam List,Launch another prior study which has more than 1 series with multiple images and mouse hover on the bottom part of the active series viewport and click on play all in this study button
                viewer.OpenPriors(accession: Accession[2]);
                PageLoadWait.WaitForBluRingViewportToLoad(panel: 3, viewport: 1);
                PageLoadWait.WaitForBluRingViewportToLoad(panel: 3, viewport: 2);
                PageLoadWait.WaitForBluRingViewportToLoad(panel: 3, viewport: 3);
                PageLoadWait.WaitForBluRingViewportToLoad(panel: 3, viewport: 4);
                viewer.ClickPlayAllOrPauseAll("PlayAll", 1, 3);
                allplaypausecine = Enumerable.Repeat(false, 4).ToArray();
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        allplaypausecine[i] = viewer.IsCINEPlaying(i + 1, 3);
                    }
                    if (allplaypausecine.All(apc => apc == true))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (allplaypausecine.All(apc => apc == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 14: Cine Play All Status for Panel 3 is [" + string.Join(",", allplaypausecine) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 15: Click on 'Next Series' button
                //viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 3)).Click();
                viewer.ClickNextSeriesCINE(1, 3);
                //viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 3)).Click();
                if (viewer.IsCINEPlaying(1, 3) && BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(3, 2))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 16: Click on "Pause series" button
                if (viewer.PauseCINE(1, 3))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
				/*
                //Step 18: From the Exam List,Launch another prior study which has more than 1 series with multiple images and mouse hover on the bottom part of the active series viewport and click on 'Play all in this study' button
                viewer.OpenPriors(accession: Accession[3]);
                PageLoadWait.WaitForBluRingViewportToLoad(panel: 4, viewport: 1);
                PageLoadWait.WaitForBluRingViewportToLoad(panel: 4, viewport: 2);
                PageLoadWait.WaitForBluRingViewportToLoad(panel: 4, viewport: 3);
                PageLoadWait.WaitForBluRingViewportToLoad(panel: 4, viewport: 4);
                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 4)).Click();
                viewer.ClickPlayAllOrPauseAll("PlayAll", 1, 4);
                allplaypausecine = Enumerable.Repeat(false, 4).ToArray();
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        allplaypausecine[i] = viewer.IsCINEPlaying(i + 1, 4);
                    }
                    if (allplaypausecine.All(apc => apc == true))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (allplaypausecine.All(apc => apc == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 21: Cine Play All Status for Panel 4 is [" + string.Join(",", allplaypausecine) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
				*/
                //Step 17: Open more than one prior study in the study panels and verify the User should be able to Play, Pause and scroll the images after Cine Paused in the series viewports.
                viewer.OpenPriors(accession: Accession[3]);
                PageLoadWait.WaitForBluRingViewportToLoad(panel: 4, viewport: 1);
                PageLoadWait.WaitForBluRingViewportToLoad(panel: 4, viewport: 2);
                PageLoadWait.WaitForBluRingViewportToLoad(panel: 4, viewport: 3);
                PageLoadWait.WaitForBluRingViewportToLoad(panel: 4, viewport: 4);
                int resultcount = 0;
                if (viewer.PlayCINE(1, 4))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Cine Started Playing on Panel 4, Viewport 1");
                }
                if (viewer.PauseCINE(1, 4))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Cine Stopped Playing on Panel 4, Viewport 1");
                }
                if (resultcount == 2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
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
                return result;
            }
            finally
            {
               BasePage.SetVMResolution("1280", "1024"); 
            }
        }

        /// <summary>
        /// Cine US Studies shall display properly in the viewer when the study has Cine loop
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_160894(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                BasePage.SetVMResolution("1980", "1080");
				String username = Config.adminUserName;
                String password = Config.adminPassword;
                DomainManagement domain = new DomainManagement();
                Studies study = new Studies();
                String AccessionNumber = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                IList<int> FPSValue = null;

                //Step-1
                //Login to BluRing/iCA Enterprise Viewer as a privilege user (i.e., Administrator/Administrator) 
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login = new Login();
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //Step-2
                //In User preferences set the thumbnail split to series for the modality to which the listed study belongs               
                study.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                domain.ModalityDropDown().SelectByText("US");
                domain.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                study.CloseUserPreferences();
                ExecutedSteps++;

                // Step 3 & 4 - search and load the study in the Bluring viewer
                login.Navigate("Studies");
                study.SearchStudy(AccessionNo: AccessionNumber, LastName: LastName, FirstName: FirstName, Datasource: EA_91);
                study.SelectStudy("Accession", AccessionNumber);
                BluRingViewer viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                // Step 4 - verifing first series is loaded in first viewport and second series is loaded in second viewport
                String[] captionList = viewer.GetStudyPanelThumbnailCaption();
                if (captionList[0].Contains("S1") && captionList[1].Contains("S2") && captionList[2].Contains("S3")
                    && captionList[3].Contains("S4") && captionList[4].Contains("S5"))
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

                // Step 5 - Click on "Play Series" button on the selected viewport.                                
                viewer.PlayCINE(1);
                ExecutedSteps++;

                // Step 6 - Check the value of the FPS slider
                //FPS will be set between 1-30 based on dicom tag(0018, 1063).
                FPSValue = viewer.GetFPSValueInList(1, 1);
                bool fps6_1 = FPSValue.All(v => (v >= 1) && (v <= 30));
                bool fps6_2 = viewer.SetFPSValue("60") == 60;
                if (fps6_1 & fps6_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 6: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step 7 - Verify the cine plays in all the images/frames from the selected series viewport and never stops when the default FPS value reaches in the FPS box.               
                if (viewer.IsCINEPlaying(1, 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                login.Logout();
                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
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
                return result;
            }
            finally
            {
                BasePage.SetVMResolution("1280", "1024");
            }
        }

        /// <summary>
        /// US Cine Clips shall not take long time to buffer/load when the user tries to load the US study which has nearly 150 frames
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_160897(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                BasePage.SetVMResolution("1980", "1080");
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                UserPreferences userpreferences = new UserPreferences();
                DomainManagement domain = new DomainManagement();
                BasePage basePage = new BasePage();
                Studies study = new Studies();
                String AccessionNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");

                //Step-1
                //Login to BluRing/iCA Enterprise Viewer as a privilege user (i.e., Administrator/Administrator) 
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login = new Login();
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //Step-2
                //In User preferences set the thumbnail split to series for the modality to which the listed study belongs  
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                basePage.SendKeys(userpreferences.CineDefaultFrameRate(), "20");
                userpreferences.ModalityDropDown().SelectByText("US");
                userpreferences.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                userpreferences.CloseUserPreferences();
                ExecutedSteps++;

                // Step 3 - search and load the study in the Bluring viewer
                login.Navigate("Studies");
                study.SearchStudy(AccessionNo: AccessionNo, Datasource: EA_91);
                study.SelectStudy("Accession", AccessionNo);
                BluRingViewer viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                // Step 4 - Click on "Play Series" button on the selected viewport.                 
                viewer.VerifyCardioCINEToolbarOnMouseHover();
                if (viewer.PlayCINE(1))
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

                // Step 5 - Verify the time should not take long time to load cine clips for US
                if (viewer.IsCINEPlaying())
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

                // Step 6 - Check the value of the FPS slider
                Thread.Sleep(2000);
                String FPSvalue = viewer.GetFPSValue();
                String[] fps = FPSvalue.Split(' ');
                int FPS = Int32.Parse(fps[0]);
                bool fps7_1 = 1 <= FPS && FPS <= 20;
                int newfps = viewer.SetFPSValue("60");
                if (fps7_1 && newfps == 60)
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

                // Step 7 - Verify Cine plays all the images from the series and reaches the default value(20FPS)
                IList<int> FPSValue = viewer.GetFPSValueInList(1, 1);
                if (FPSValue.All(v => (v >= 1) && (v <= newfps)) && viewer.IsCINEPlaying(1, 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 7: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                //Return Result;
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
                return result;
            }
            finally
            {
                BasePage.SetVMResolution("1280", "1024");
            }
        }

        /// <summary> 
        /// Cardio Features(Play Series,Pause Series,Pre series,Next Series,FPS Slider)
        /// </summary>
        public TestCaseResult Test_160901(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            Stopwatch stopwatch = new Stopwatch();
            TimeSpan timeout = new TimeSpan(0, 2, 0);
            bool[] allplaypausecine = null;
            try
            {
                BasePage.SetVMResolution("1980", "1080");
                String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = AccessionNoList.Split(':');

                //Step 1 - Login to BluRing/iCA Enterprise Viewer as a privilege user (i.e., Administrator/Administrator) 
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login = new Login();
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                ExecutedSteps++;

                //Step 2 - In User preferences set the thumbnail split to series for the modality to which the listed study belongs and then click on "Save" button.                               
                studies.OpenUserPreferences();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                domainmanagement.ModalityDropDown().SelectByText("MR");
                domainmanagement.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                PageLoadWait.WaitForPageLoad(20);
                studies.CloseUserPreferences();
                ExecutedSteps++;

                //Step 3 - Search and load for a study with multiple series multiple images having only 1 modality
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: AccessionNumbers[0], Datasource: EA_91);
                studies.SelectStudy("Accession", AccessionNumbers[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                if (viewer.studyPanel().Displayed)
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

                //Step 4 - Load first series in first viewport, second series in second viewport and so on
                viewer.SetSeriesInViewport(0, 1);
                viewer.SetSeriesInViewport(1, 1);
                viewer.SetSeriesInViewport(2, 1);
                ExecutedSteps++;

                //Step 5 - Verify the Cardio Cine Tool should not display all the visible viewports by default.
                IList<IWebElement> PauseBtn = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_CINE_PauseBtn));
                IList<IWebElement> PlayBtn = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_CINE_PlayBtn));
                IList<IWebElement> NextBtn = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_CINE_NextImageBtn));
                IList<IWebElement> PrevBtn = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_CINE_PreviousImageBtn));
                bool res_5 = true;
                for (int i = 0; i < PlayBtn.Count; i++)
                {
                    if (PlayBtn[i].Displayed || NextBtn[i].Displayed || PrevBtn[i].Displayed)
                    {
                        res_5 = false;
                        break;
                    }
                }

                if (res_5 && PauseBtn.Count == 0 && PlayBtn.Count == NextBtn.Count && NextBtn.Count == PrevBtn.Count)
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

                //Step 6 - Verify that the "Play All in Study", Previous Series,Play Image/Pause Series,Next Series and 
                //         FPS box features should be available in the Cardio Cine Tool.
                //String playorpauseall = BluRingViewer.div_studypanel + ":nth-of-type(1)" + " div.viewerContainer:nth-of-type(1) " + BluRingViewer.div_CINE_PlayAllBtn;
                /*String previouseries = BluRingViewer.div_studypanel + ":nth-of-type(1)" + " div.viewerContainer:nth-of-type(1) " + BluRingViewer.div_CINE_PreviousImageBtn;
                String nextseries = BluRingViewer.div_studypanel + ":nth-of-type(1)" + " div.viewerContainer:nth-of-type(1) " + BluRingViewer.div_CINE_NextImageBtn;
                String PlayButton = BluRingViewer.div_studypanel + ":nth-of-type(1)" + " div.viewerContainer:nth-of-type(1) " + BluRingViewer.div_CINE_PlayBtn;
                String PauseButton = BluRingViewer.div_studypanel + ":nth-of-type(1)" + " div.viewerContainer:nth-of-type(1) " + BluRingViewer.div_CINE_PauseBtn;
                String FPSBox = BluRingViewer.div_StudyPanel + ":nth-of-type(1)" + " div.viewerContainer:nth-of-type(1)" + " .FPSControlButton";
                if (IsElementPresent(By.CssSelector(previouseries)) &&
                    IsElementPresent(By.CssSelector(nextseries)) && IsElementPresent(By.CssSelector(PlayButton)) &&
                    IsElementPresent(By.CssSelector(FPSBox)))*/
                viewer.OpenCineToolBar(2, 1);
                if (IsElementPresent(By.CssSelector(BluRingViewer.div_CINE_PreviousImageBtn)) &&
                IsElementPresent(By.CssSelector(BluRingViewer.div_CINE_NextImageBtn)) && IsElementPresent(By.CssSelector(BluRingViewer.div_CINE_PlayBtn)) &&
                IsElementPresent(By.CssSelector(BluRingViewer.div_CINE_FPSControlButton)))
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
                
                //Step 7 - Verify when the cursor is hover the Play Series button , the tool tip for Play Series button is displayed as 'Play Image Series'
                if (Driver.FindElement(By.CssSelector(BluRingViewer.div_CINE_PlayBtn)).GetAttribute("title").Equals("Play Image Series"))
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

                //Step 8 - Click on "Play Series" button on the selected viewport.
                if (viewer.PlayCINE(2, 1))
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

                //Step 9 - Verify the images being played
                if (viewer.IsCINEPlaying(2, 1))
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

                //Step 10 - Verify when the cursor is hover the Pause Series button , the tool tip for Pause Series button is displayed as 'Pause Image Series'                                 
                viewer.OpenCineToolBar(2, 1);
                if (Driver.FindElement(By.CssSelector(BluRingViewer.div_CINE_PauseBtn)).GetAttribute("title").Equals("Pause Image Series"))
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

                //Step 11 - Click on "Pause Series" button in a viewport
                if (viewer.PauseCINE(2, 1))
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

                //Step 12 - Verify when cursor is hover the Previous Series button , the tool tip for Previous Series button is displayed as 'Play Previous Image Series'
                viewer.OpenCineToolBar(2, 1);
                if (Driver.FindElement(By.CssSelector(BluRingViewer.div_CINE_PreviousImageBtn)).GetAttribute("title").Equals("Play Previous Image Series"))
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

                //Step 13 - Click Previous Series button
                viewer.CloseCineToolBar();
                //viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 1)).Click();
                viewer.ClickPreviousSeriesCINE(2, 1);
                //viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 1)).Click();
                if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 14 - Verify the cine should be automatically start playing after loading the series.
                if (viewer.IsCINEPlaying(2, 1))
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

                //Step 15 - Verify when cursor is hover the Next Series button , the tool tip for Next Series button is displayed as 'Play Next Image Series'
                viewer.OpenCineToolBar(2, 1);
                if (Driver.FindElement(By.CssSelector(BluRingViewer.div_CINE_NextImageBtn)).GetAttribute("title").Equals("Play Next Image Series"))
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

                //Step 16 - Click Next Series button
                viewer.CloseCineToolBar();
                //viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 1)).Click();
                viewer.ClickNextSeriesCINE(2, 1);
                //viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 1)).Click();
                if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 2))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 17 - Verify the cine should be automatically start playing after loading the series.
                if (viewer.IsCINEPlaying(2, 1))
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

                //Step 18 - Verify when cursor is hover the FPS button , the tool tip for FPS button is displayed as 'Adjust the Frames Per Second'
                viewer.OpenCineToolBar(2, 1);
                if (GetElement(SelectorType.CssSelector, BluRingViewer.div_CINE_FPS).GetAttribute("title").Equals("Adjust the Frames Per Second"))
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

                //Step 19 - Click on "Pause Series" button in a viewport
                Thread.Sleep(5000);
                if (viewer.PauseCINE(2, 1))
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

                //Step 20 - Verify that the actual FPS is zero when cine is not playing
                if (viewer.GetFPSValue().Equals("0 FPS"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 21 - Click on FPS box and verify the Frames Per Second Slider should get appear                    
                //viewer.VerifyCardioCINEToolbarOnMouseHover(3, 1);
                //viewer.SetFPSValue("20", 1);
                viewer.OpenCineToolBar(2, 1);
                //new TestCompleteAction().Click(GetElement("cssselector", BluRingViewer.div_CINE_FPSControlButton)).Perform();
                if (SBrowserName.ToLower().Contains("firefox"))
                {
                    ClickElement(GetElement("cssselector", BluRingViewer.div_CINE_FPSControlButton));
                }
                else
                {
                    GetElement("cssselector", BluRingViewer.div_CINE_FPSControlButton).Click();
                }
                //viewer.Click("cssselector",BluRingViewer.div_CINE_FPSControlButton);
                //bool step21 = viewer.IsElementVisibleInUI(By.CssSelector("div.viewerContainer:nth-of-type(3) " + BluRingViewer.div_CINE_FPSSliderHandler));
                Thread.Sleep(3000);
                bool step21 = viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_CINE_FPSSliderHandler));
                if (step21)
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

                //Step 22 - Change fps by dragging the scrollbar on FPS Slider and verify the user able to edit the slider to have the new FPS value                                                
                viewer.SetFPSValue("50", 2);
                ExecutedSteps++;

                //Step 23 - Verify that the current maximum for the desired FPS should be 60 frames per second.                
                viewer.SetFPSValue("60", 2);
                //IWebElement slider = GetElement("cssselector", BluRingViewer.div_StudyPanel + ":nth-of-type(1) div.viewerContainer:nth-of-type(1) " +
                //BluRingViewer.div_CINE_FPSSlider);
                viewer.OpenCineToolBar(2, 1);
                if (SBrowserName.ToLower().Contains("firefox"))
                {
                    ClickElement(GetElement("cssselector", BluRingViewer.div_CINE_FPSControlButton));
                }
                else
                {
                    GetElement("cssselector", BluRingViewer.div_CINE_FPSControlButton).Click();
                }
                Thread.Sleep(3000);
                IWebElement slider = GetElement("cssselector", BluRingViewer.div_CINE_FPSSlider);
                if (slider.Text.Equals("60"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 24 - Again left click on FPS box and verify the FPS slider should get disappear                
                //Click("cssSelector", BluRingViewer.div_StudyPanel + ":nth-of-type(1)" + " div.viewerContainer:nth-of-type(1)" + " .FPSControlButton", true);
                if (SBrowserName.ToLower().Contains("firefox"))
                {
                    ClickElement(GetElement("cssselector", BluRingViewer.div_CINE_FPSControlButton));
                }
                else
                {
                    GetElement("cssselector", BluRingViewer.div_CINE_FPSControlButton).Click();
                }
                Thread.Sleep(5000);
                //if (!IsElementVisible(By.CssSelector(BluRingViewer.div_StudyPanel + ":nth-of-type(1) div.viewerContainer:nth-of-type(1) " + BluRingViewer.div_CINE_FPSSlider)))
                if (!IsElementVisible(By.CssSelector(BluRingViewer.div_CINE_FPSSlider)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 25 - Verify that the actual frame rate should be calculated based on the rendering time of the images and the actual frame rate should range from 1 to the desired frame rate.
                viewer.PlayCINE(2, 1);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    if (viewer.IsCINEPlaying(2, 1))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                IList<int> FPSValue = viewer.GetFPSValueInList(2, 1);
                if (FPSValue.All(v => (v >= 1) && (v <= 60)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 25: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 26 - Move the mouse away from the cine toolbox including the slider area and verify the FPS slider should get disappear
                //if (!IsElementVisible(By.CssSelector(BluRingViewer.div_StudyPanel + ":nth-of-type(1) div.viewerContainer:nth-of-type(2) " + BluRingViewer.div_CINE_FPSSlider)))
                if (!IsElementVisible(By.CssSelector(BluRingViewer.div_CINE_FPSSlider)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 27:
                viewer.PauseCINE(2, 1);
                if(!viewer.IsCINEPlaying(2, 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 28:
                viewer.OpenCineToolBar(2, 1);
                new TestCompleteAction().Click(viewer.GetElement("cssselector", viewer.GetViewportCss(1, 1))).Perform();
                if(Driver.FindElements(By.CssSelector(BluRingViewer.div_viewerMenuCloseButton)).Count == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 29:
                viewer.CloseCineToolBar();
                if (Driver.FindElements(By.CssSelector(BluRingViewer.div_viewerMenuCloseButton)).Count == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 30 - Mouse hover on the bottom part of the series viewport and verify when cursor is hover the Play All in Study button , the tool tip for 'Play All in Study' button is displayed as 'Play All in This Study'
                //viewer.VerifyCardioCINEToolbarOnMouseHover(1, 1);

                // playorpauseall = BluRingViewer.div_studypanel + ":nth-of-type(1)" + " div.viewerContainer:nth-of-type(1) " + BluRingViewer.div_CINE_PlayAllBtn;
                var playorpauseall = GetElement(SelectorType.CssSelector, BluRingViewer.div_CINE_PlayAllBtn);
				var playorpauseallParent = playorpauseall.FindElement(By.XPath(".."));
				if (playorpauseallParent.GetAttribute("title").Equals("Group Play"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 31 - Mouse hover on the bottom part of the series viewport and verify when cursor is hover the Play All in Study button, the tool tip for 'Play All in Study' button is displayed as 'Play All in This Study'
                viewer.ClickPlayAllOrPauseAll("PlayAll");
                allplaypausecine = Enumerable.Repeat(false, 4).ToArray();
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        allplaypausecine[i] = viewer.IsCINEPlaying(i + 1, 1);
                    }
                    if (allplaypausecine.All(apc => apc == true))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (allplaypausecine.All(apc => apc == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 32: 
                //Verify the "Pause all The Study" button should get enabled in all series viewports on the screen
                allplaypausecine = Enumerable.Repeat(false, 4).ToArray();
                for (int i = 0; i < 4; i++)
                {
					//allplaypausecine[i] = viewer.VerifyPlayAllOrPauseAll("PauseAll", i + 1, 1);
					allplaypausecine[i] = viewer.VerifyCINEPlayorPauseEnabled("pause", i + 1, 1);
                }
                if (allplaypausecine.All(apc => apc == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 33: Verify that the tool tip for the "Pause all The Study" button should be displayed as "Pause All in This Study
                viewer.OpenCineToolBar(2,1);
				//playorpauseall = BluRingViewer.div_studypanel + ":nth-of-type(1)" + " div.viewerContainer:nth-of-type(1) " + BluRingViewer.div_CINE_PauseAllBtn;
				var pauseSeriesBtn = BluRingViewer.div_CINE_PauseBtn;
				if (GetElement(SelectorType.CssSelector, pauseSeriesBtn).GetAttribute("title").Equals("Pause Image Series"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

				//Step 34 - Verify the tooltip for Cine Pause All button is displayed as "Cine Pause All" in the study Panel toolbar
				playorpauseall = GetElement(SelectorType.CssSelector, BluRingViewer.div_CINE_PauseAllBtn);
				playorpauseallParent = playorpauseall.FindElement(By.XPath(".."));
				if (playorpauseallParent.GetAttribute("title").Equals("Group Pause"))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step 35: Click on "Pause All in the Study" button on any visible series viewports.
				viewer.ClickPlayAllOrPauseAll("PauseAll");
                allplaypausecine = Enumerable.Repeat(false, 4).ToArray();
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        allplaypausecine[i] = !viewer.IsCINEPlaying(i + 1, 1);
                    }
                    if (allplaypausecine.All(apc => apc == true))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (allplaypausecine.All(apc => apc == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Close viewer and logout
                viewer.CloseBluRingViewer();
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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
                return result;
            }
            finally
            {
                BasePage.SetVMResolution("1280", "1024");
            }
        }

        /// <summary>
        /// Images shall display by scrolling the mouse wheel after Cine paused from playing
        /// </summary>
        public TestCaseResult Test_160893(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');

                // Step 1 & 2 - Launch the Application and Login as  a Administrator
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;
                if (login.IsTabPresent("Studies") && login.IsTabPresent("Patients") && login.IsTabPresent("Domain Management"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 3 - In the Studies tab, search any study that has multiple images per series/ multiple frames and click on 'View Exam' button
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step 4 - 	Mouse hover on the bottom part of the selected series viewport and verify the Cardio Cine Tool
                result.steps[++ExecutedSteps].status = "Not Automated";
                
                // step 5 - Verify the actual value of cine fps in the FPS box
                viewer.VerifyCardioCINEToolbarOnMouseHover(1, 1);
                if (viewer.GetFPSValue().Equals("0 FPS"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // step 6 - Click on "Play All in Study" button and                 
                viewer.ClickPlayAllOrPauseAll("PlayAll");
                Thread.Sleep(5000);                
                var step6 = viewer.WaitForThumbnailPercentageTo100(4);
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

                // step 7 - Check the values of FPS box while playing cine in all series viewports and verify the values in the FPS box should be change on default FPS.
                /*String FPSValue = null;
                bool step7 = true;
                for (int i = 1; i <= 4; i++)
                {
                    FPSValue = viewer.GetFPSValue(i);
                    int viewportFPS = Int32.Parse(FPSValue.Remove(FPSValue.Length - 4));
                    if (!(1 <= viewportFPS && viewportFPS <= 20))
                    {
                        step7 = false;
                    }
                }
                
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
                }*/
                //Validating FPS adjustable on all viewport will impact further test steps. Moved Test Step to Not Atomated.
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 8 - Verify the cine plays in all the series viewports in the study panel and never stops when the default FPS value reaches in the FPS box/study get buffer
                /*int j = 1;
                String[] step8 = new String[] { "false", "false", "false", "false", "false" };
                for (; j < 5; j++)
                {
                    String FPSvalue = viewer.GetFPSValue(j);
                    if (!FPSvalue.Equals("20 FPS"))
                    {
                        int k = 0;
                        for (; k <= 40; k++)
                        {
                            FPSvalue = viewer.GetFPSValue(j);
                            if (!FPSvalue.Equals("20 FPS"))
                            {
                                Thread.Sleep(2000);
                            }
                            else
                            {
                                step8[j] = "true";
                                break;
                            }
                        }
                        if (k >= 21)
                        {
                            step8[j] = "false";
                        }
                    }
                }
                if (step8[1].Equals("true") && step8[2].Equals("true") && step8[3].Equals("true") && step8[4].Equals("true"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }*/
                result.steps[++ExecutedSteps].status = "Not Automated";
                // Step 9 - Again click on "Play All in Study" button in any series viewport and verify the Cine should pause from playing
                viewer.ClickPlayAllOrPauseAll("PauseAll");
                if (!viewer.IsCINEPlaying(1, 1) && !viewer.IsCINEPlaying(2, 1) && !viewer.IsCINEPlaying(3, 1) && !viewer.IsCINEPlaying(4, 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step 10 - Click on "Play" button and verify the cine plays all images from the selected series and never stops
                // Step 11 - While playing cine verify the FPS box
                viewer.PlayCINE();
                ExecutedSteps++;
                String FPSValue = viewer.GetFPSValue();
                if (!FPSValue.Equals("0 FPS"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step 12 - While cine is playing apply tool ==> while playing CINE, cannot able to apply tool and compare image
                result.steps[++ExecutedSteps].status = "Not Automated";

                // Step 13 - Click on "Pause series" button
                viewer.PauseCINE();
                if (!viewer.IsCINEPlaying())
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step 14 - Scroll through all images in the current series viewport and verify all images are displayed correctly.
                /*IWebElement element = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                var action = new TestCompleteAction();
                action.MouseScroll(element, "up", "22");
                action.MouseScroll(element, "down", "5");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step14_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                action.MouseScroll(element, "down", "5");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step14_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                action.MouseScroll(element, "down", "5");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                bool step14_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                action.MouseScroll(element, "down", "5");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
                bool step14_4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                action.MouseScroll(element, "down", "3");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 5);
                bool step14_5 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step14_1 && step14_2 && step14_3 && step14_4 && step14_5)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }*/
                //Cannot verify image scrolled properly using Gold image validation after Cine Pause
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Logout Application
                viewer.CloseBluRingViewer();
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
        /// Test 139360 - Cine XA Studies shall appear correctly without change/loss after transferred from EA to MPACS data source.
        /// </summary>
        public TestCaseResult Test_160899(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variable
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            BasePage basePage = new BasePage();
            UserPreferences userpreferences = new UserPreferences();
            DomainManagement domainmanagement = null;
            RoleManagement rolemanagement = null;
            Studies studies = null;
            BluRingViewer viewer = null;
            string PatientId = null;
            string Accession = null;
            IList<int> FPSValue = null;
            try
            {
                BasePage.SetVMResolution("1980", "1080");
                PatientId = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                //Step 1: In the service tool select Enable --*^>^* Enable Data Transfer and Enable Data Downloader
                result.steps[++ExecutedSteps].status = "Pass";
                //Step 2: Logon to iConnect Access Enterprise viewer by using Administrator
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login = new Login();
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(Config.adminGroupName);
                domainmanagement.SelectDomain(Config.adminGroupName);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("datatransfer", 0);
                domainmanagement.SetCheckBoxInEditDomain("datadownload", 0);
                domainmanagement.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SearchRole(Config.adminRoleName, Config.adminGroupName);
                rolemanagement.SelectRole(Config.adminRoleName);
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("transfer", 0);
                rolemanagement.SetCheckboxInEditRole("download", 0);
                rolemanagement.ClickSaveEditRole();
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                result.steps[++ExecutedSteps].status = "Pass";
                //Step 3: Select the Studies Tab
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                if (login.IsTabSelected("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 4: Verify the "Transfer" button should be displayed.
                PageLoadWait.WaitForFrameLoad(30);
                if (basePage.TransferBtn().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 5: Search and select the XA modality study
                //PatientID "Anonymous-ID" Accession:ACCNXA).
                studies.SearchStudy(patientID: PatientId, AccessionNo: Accession, Datasource: EA_91, Modality: "XA");
                PageLoadWait.WaitForSearchLoad();
                if (studies.CheckStudy("Accession", Accession))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 6: Select Transfer button
                //Step 7: Select any PACS Data source in Transfer To dropdown and click on Transfer button
                //Step 8: Verify the selected study should be listed in the transfer Status / History grid and study should get transferred if status is Success.
                //Step 9: Click on "Close" button
                studies.SelectStudy("Accession", Accession);
                string status = studies.TransferStudy(login.GetHostName(Config.DestinationPACS), TimeOut: 600);
                if (string.Equals("succeeded", status, StringComparison.OrdinalIgnoreCase))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 10: Select PACS data source in which user transferred the study from EA data source in the "Data Source:" field and verify that the Cine XA Study should appear correctly without change/loss after transferred from EA to MPACS data source
                studies.SearchStudy(patientID: PatientId, AccessionNo: Accession, Datasource: login.GetHostName(Config.DestinationPACS));
                PageLoadWait.WaitForSearchLoad();
                if (studies.CheckStudy("Accession", Accession))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 11: Search and select US study with multi-frames or images and click "View Exam" button
                studies.SelectStudy("Accession", Accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                if (viewer.studyPanel().Displayed)
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
                //Step 12: Verify the Cardio Cine Tool should not display in all viewports by default.
                IList<IWebElement> PauseBtn = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_CINE_PauseBtn));
                IList<IWebElement> PlayBtn = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_CINE_PlayBtn));
                IList<IWebElement> NextBtn = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_CINE_NextImageBtn));
                IList<IWebElement> PrevBtn = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_CINE_PreviousImageBtn));
                bool res_4 = true;
                for (int i = 0; i < PlayBtn.Count; i++)
                {
                    if (PlayBtn[i].Displayed || NextBtn[i].Displayed || PrevBtn[i].Displayed)
                    {
                        res_4 = false;
                        break;
                    }
                }

                if (res_4 && PauseBtn.Count == 0 && PlayBtn.Count == NextBtn.Count && NextBtn.Count == PrevBtn.Count)
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
                
                //Step 13: Click on "Play Series" button on the selected viewport.
                if (viewer.PlayCINE(1, 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 14: Check the values of FPS slider while playing cine and verify the values in the FPS box should be change on default FPS.
                int newfps = viewer.SetFPSValue("60");
                if (newfps == 60)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 15: Verify the cine plays in all the images from the selected series viewport and never stops when the default FPS value reaches in the FPS slider text box.
                FPSValue = viewer.GetFPSValueInList(1, 1);
                Logger.Instance.InfoLog("The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                if (viewer.IsCINEPlaying(1, 1) && FPSValue.All(v => (v >= 1) && (v <= 60)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseBluRingViewer();
                //Logout
                login.Logout();
                //Report Result
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Logout
                login.Logout();
                //Return Result
                return result;
            }
            finally
            {
                BasePage.SetVMResolution("1280", "1024");
            }
        }

        /// <summary> 
        /// Test 160898 - Cine US Study which has multi-frames or images shall not miss when the user transfer the study from one data source to another data source
        /// </summary>
        public TestCaseResult Test_160898(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variable
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            BasePage basePage = new BasePage();
            UserPreferences userpreferences = new UserPreferences();
            DomainManagement domainmanagement = null;
            RoleManagement rolemanagement = null;
            Studies studies = null;
            BluRingViewer viewer = null;
            string PatientId = null;
            string Accession = null;
            IList<int> FPSValue = null;
            try
            {
                BasePage.SetVMResolution("1980", "1080");
                PatientId = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                //Step 1: In the service tool select Enable --*^>^* Enable Data Transfer and Enable Data Downloader
                result.steps[++ExecutedSteps].status = "Pass";
                //Step 2: Logon to iConnect Access Enterprise viewer by using Administrator
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login = new Login();
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(Config.adminGroupName);
                domainmanagement.SelectDomain(Config.adminGroupName);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("datatransfer", 0);
                domainmanagement.SetCheckBoxInEditDomain("datadownload", 0);
                domainmanagement.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SearchRole(Config.adminRoleName, Config.adminGroupName);
                rolemanagement.SelectRole(Config.adminRoleName);
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("transfer", 0);
                rolemanagement.SetCheckboxInEditRole("download", 0);
                rolemanagement.ClickSaveEditRole();
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                result.steps[++ExecutedSteps].status = "Pass";
                //Step 3: Select the Studies Tab
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                basePage.SendKeys(userpreferences.CineDefaultFrameRate(), "20");
                userpreferences.BluringViewerRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                if (login.IsTabSelected("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 4: Verify the "Transfer" button should be displayed.
                PageLoadWait.WaitForFrameLoad(30);
                if (basePage.TransferBtn().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 5: Search and select the US modality study
                //PatientID "PIDUS" Accession:ACCUS).
                studies.SearchStudy(patientID: PatientId, AccessionNo: Accession, Datasource: EA_91, Modality: "US");
                PageLoadWait.WaitForSearchLoad();
                if (studies.CheckStudy("Accession", Accession))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 6: Select Transfer button
                //Step 7: Select any Data source in Transfer To dropdown and click on Transfer button.
                //Step 8: Verify the selected study should be listed in the transfer Status/History grid and study should get transferred if status is Success.
                //Step 9: Click on "Close" button
                studies.SelectStudy("Accession", Accession);
                string status = studies.TransferStudy(login.GetHostName(Config.DestinationPACS), TimeOut: 900);
                if (string.Equals("succeeded", status, StringComparison.OrdinalIgnoreCase))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 10: Select the data source field in which user transferred the study from one data source to another in the "Data Source:" field.
                studies.SearchStudy(patientID: PatientId, AccessionNo: Accession, Datasource: login.GetHostName(Config.DestinationPACS));
                PageLoadWait.WaitForSearchLoad();
                if (studies.CheckStudy("Accession", Accession))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 11: Search and select US study with multi-frames or images and click "Universal" button
                //Step 12: Verify all the series/ images should be transferred with no issues.
                studies.SelectStudy("Accession", Accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                if (viewer.studyPanel().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 13: Verify the Cardio Cine Tool should not display in all viewports by default.
                IList<IWebElement> PauseBtn = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_CINE_PauseBtn));
                IList<IWebElement> PlayBtn = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_CINE_PlayBtn));
                IList<IWebElement> NextBtn = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_CINE_NextImageBtn));
                IList<IWebElement> PrevBtn = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_CINE_PreviousImageBtn));
                bool res_4 = true;
                for (int i = 0; i < PlayBtn.Count; i++)
                {
                    if (PlayBtn[i].Displayed || NextBtn[i].Displayed || PrevBtn[i].Displayed)
                    {
                        res_4 = false;
                        break;
                    }
                }

                if (res_4 && PauseBtn.Count == 0 && PlayBtn.Count == NextBtn.Count && NextBtn.Count == PrevBtn.Count)
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
                
                //Step 14: Click on "Play Series" button on the selected viewport.
                if (viewer.PlayCINE(1, 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 15: Check the values of FPS slider while playing cine and verify the values in the FPS box should be change on default FPS.
                FPSValue = viewer.GetFPSValueInList(1, 1);
                bool fps16_1 = FPSValue.All(v => (v >= 1) && (v <= 20));
                bool fps16_2 = viewer.SetFPSValue("60") == 60;
                if (fps16_1 && fps16_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 15: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 16: Verify the cine plays in all the images from the selected series viewport and never stops when the default FPS value reaches in the FPS slider text box.
                if (viewer.IsCINEPlaying(1, 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseBluRingViewer();
                //Logout
                login.Logout();
                //Report Result
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Logout
                login.Logout();
                //Return Result
                return result;
            }
            finally
            {
                BasePage.SetVMResolution("1280", "1024");
            }
        }

        /// <summary> 
        /// Test 143270 - Cine Toolbox appear in 400 milliseconds and animate when disappearing
        /// </summary>
        public TestCaseResult Test_160886(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variable
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            BasePage basePage = new BasePage();
            UserPreferences userpreferences = new UserPreferences();
            Studies studies = null;
            BluRingViewer viewer = null;
            string PatientId = null;
            string Accession = null;
            IList<int> FPSValue = null;
            bool cine = true;
            try
            {
                PatientId = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                //Step 1: Login to WebAccess site with any privileged user
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                basePage.SendKeys(userpreferences.CineDefaultFrameRate(), "30");
                userpreferences.BluringViewerRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                result.steps[++ExecutedSteps].status = "Pass";
                //Step 2: Select Studies tab
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                if (login.IsTabSelected("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 3: Search and load any study with multiple series/frame contains multiple images
                PageLoadWait.WaitForFrameLoad(30);
                studies.SearchStudy(patientID: PatientId, AccessionNo: Accession, Datasource: EA_91);
                PageLoadWait.WaitForSearchLoad();
                studies.SelectStudy("Accession", Accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                if (viewer.studyPanel().Displayed)
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
                //Step 4: Select any series viewports which has multiple images/frames and mouse hover on the bottom part of the series viewport and then verify that the Cardio Cine Tool should get appear within 400 milliseconds
                if (viewer.VerifyCardioCINEToolbarOnMouseHover(1, 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 5: Click on Play Series button and verify the Cine plays all images from the series and never stops
                if (viewer.PlayCINE(1, 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 6: Verify that the FPS values when cine is playing
                FPSValue = viewer.GetFPSValueInList();
                if (FPSValue.Contains(30))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 6: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 7: Click on Pause Series button
                if (viewer.PauseCINE(1, 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 8: Verify the cine toolbox is fade out without any delay when user moves the cursor away from the cine toolbox including slider area.
                bool hover = viewer.VerifyCardioCINEToolbarOnMouseHover(2, 1);
                string locator = BluRingViewer.div_StudyPanel + ":nth-of-type(1) " + BluRingViewer.div_compositeViewerComponent + ":nth-of-type(1) " + BluRingViewer.div_cineToolboxComponent + " ";
                int PauseBtn = BasePage.Driver.FindElements(By.CssSelector(locator + BluRingViewer.div_CINE_PauseBtn)).Count;
                IWebElement PlayBtn = BasePage.Driver.FindElement(By.CssSelector(locator + BluRingViewer.div_CINE_PlayBtn));
                IWebElement NextBtn = BasePage.Driver.FindElement(By.CssSelector(locator + BluRingViewer.div_CINE_NextImageBtn));
                IWebElement PrevBtn = BasePage.Driver.FindElement(By.CssSelector(locator + BluRingViewer.div_CINE_PreviousImageBtn));
                if (PlayBtn.Displayed || NextBtn.Displayed || PrevBtn.Displayed)
                {
                    cine = false;
                }
                if (PauseBtn == 0 && cine)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 9: Again mouse hover on the bottom part of the series viewport and verify the Cardio Cine Tool should get appear without any animation.
                if (hover)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 10: Move the cursor to any visible series viewport
                locator = BluRingViewer.div_StudyPanel + ":nth-of-type(1) " + BluRingViewer.div_compositeViewerComponent + ":nth-of-type(2) " + BluRingViewer.div_cineToolboxComponent + " ";
                viewer.VerifyCardioCINEToolbarOnMouseHover(3, 1);
                PauseBtn = BasePage.Driver.FindElements(By.CssSelector(locator + BluRingViewer.div_CINE_PauseBtn)).Count;
                PlayBtn = BasePage.Driver.FindElement(By.CssSelector(locator + BluRingViewer.div_CINE_PlayBtn));
                NextBtn = BasePage.Driver.FindElement(By.CssSelector(locator + BluRingViewer.div_CINE_NextImageBtn));
                PrevBtn = BasePage.Driver.FindElement(By.CssSelector(locator + BluRingViewer.div_CINE_PreviousImageBtn));
                cine = true;
                if (PlayBtn.Displayed || NextBtn.Displayed || PrevBtn.Displayed)
                {
                    cine = false;
                }
                if (PauseBtn == 0 && cine)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseBluRingViewer();
                //Report Result
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                login = new Login();
                //Return Result
                return result;
            }
            finally
            {
                login.Logout();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                basePage.SendKeys(userpreferences.CineDefaultFrameRate(), "20");
                userpreferences.BluringViewerRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                login.Logout();
            }
        }

        /// <summary>
        /// Cine toolbox UI
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_160887(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                BasePage.SetVMResolution("1980", "1080");
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String domain1 = "Domain1_" + new Random().Next(1, 1000);
                String role1 = "Role1_" + new Random().Next(1, 1000);
                String rad1 = Config.newUserName + new Random().Next(1, 1000);
                String AccessionNumber = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                //Precondition
                //create a domain and privilege user 
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login = new Login();
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                var domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.CreateDomain(domain1, role1, datasources: null);
                domain.ModalityDropDown().SelectByText("MR");
                domain.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                domain.ClickSaveNewDomain();
                login.Navigate("UserManagement");
                usermanagement.CreateUser(rad1, domain1, role1);
                login.Logout();

                // Step 1 - Login to iCA as a Administrator or privilege user(eg. rad/rad)	
                login.LoginIConnect(rad1, rad1);
                ExecutedSteps++;

                // Step 2 - search and load the study in the Bluring viewer
                var study = (Studies)login.Navigate("Studies");
                study.SearchStudy(AccessionNo: AccessionNumber, Datasource: EA_131);
                study.SelectStudy("Accession", AccessionNumber);
                BluRingViewer viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                // Step 3 - verifing first series is loaded in first viewport and second series is loaded in second viewport
                String[] captionList = viewer.GetStudyPanelThumbnailCaption();
                if (captionList[0].Contains("S1") && captionList[1].Contains("S2") && captionList[2].Contains("S3")
                    && captionList[3].Contains("S4") && captionList[4].Contains("S5"))
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

                // Step 4: 
                viewer.OpenCineToolBar(1, 1);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport)))
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

                // Step 5
                if(viewer.GetFPSValue(1,1)== "0 FPS")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 6:
                viewer.OpenCineToolBar(1, 1);
                if (SBrowserName.ToLower().Contains("firefox"))
                {
                    ClickElement(GetElement("cssselector", BluRingViewer.div_CINE_FPSControlButton));
                }
                else
                {
                    GetElement("cssselector", BluRingViewer.div_CINE_FPSControlButton).Click();
                }
                //new TestCompleteAction().Click(GetElement("cssselector", BluRingViewer.div_CINE_FPSControlButton)).Perform();
                if (Driver.FindElements(By.CssSelector(BluRingViewer.div_CINE_FPSSliderHandler)).Count == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 7:
                if (SBrowserName.ToLower().Contains("firefox"))
                {
                    ClickElement(GetElement("cssselector", BluRingViewer.div_CINE_FPSControlButton));
                }
                else
                {
                    GetElement("cssselector", BluRingViewer.div_CINE_FPSControlButton).Click();
                }
                Thread.Sleep(3000);
                //new TestCompleteAction().Click(GetElement("cssselector", BluRingViewer.div_CINE_FPSControlButton)).Perform();
                if (Driver.FindElements(By.CssSelector(BluRingViewer.div_CINE_FPSSliderHandler)).Count == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                viewer.CloseBluRingViewer();
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
            finally
            {
                BasePage.SetVMResolution("1280", "1024");
            }
        }

        /// <summary> 
        /// Test 141623_1 - Cardio FPS Slider : Study with many priors in Panel 2
        /// </summary>
        public TestCaseResult Test_160890(String testid, String teststeps, int stepcount) 
        {
            //Declare and initialize variable
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            BasePage basePage = new BasePage();
            UserPreferences userpreferences = new UserPreferences();
            Studies studies = null;
            BluRingViewer viewer = null;
            string PatientId = null;
            string[] Accession = null;
            IList<int> FPSValue = null;
            Stopwatch stopwatch = new Stopwatch();
            TimeSpan timeout = new TimeSpan(0, 2, 0);
            bool[] ViewPortFPS = null;
            int[] fpsval = Enumerable.Repeat(20, 4).ToArray();
            int studypanel = 2;
            try
            {
                BasePage.SetVMResolution("1980", "1080");
                PatientId = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                Accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList")).Split('=');
                //Step 1: Login to application with any privileged user
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login = new Login();
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                if (login.IsTabPresent("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 2: In User Preferences dialog, Verify that the System Cine Default Frame Rate is 20 FPS and click on "OK" button.
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                basePage.SendKeys(userpreferences.CineDefaultFrameRate(), "20");
                userpreferences.BluringViewerRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                result.steps[++ExecutedSteps].status = "Pass";
                //Step 3: From the Studies tab, search for a patient with many priors(e.g 4 priors )and load the study into the viewer.
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                studies.SearchStudy(patientID: PatientId, AccessionNo: Accession[0], Datasource: EA_91);
                PageLoadWait.WaitForSearchLoad();
                studies.SelectStudy("Accession", Accession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                if (viewer.studyPanel().Displayed)
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
                //Step 4: Launch one prior study which has many series(e.g. 3 series) with multiple images from exam list by single clicking on studies card and launch the prior study in viewer.
                viewer.OpenPriors(accession: Accession[1]);
                if (viewer.studyPanel(studypanel).Displayed)
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
                
                //Step 5: Click on "Play series" button
                if (viewer.PlayCINE(1, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 6: Check the values of FPS slider while playing cine and verify the values in the FPS box should be change on default FPS.
                FPSValue = viewer.GetFPSValueInList(1, studypanel);
                if (FPSValue.All(v => (v >= 0) && (v <= fpsval[0])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 6: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 7: Verify the cine plays in all the images from the selected series viewport and never stops when the default FPS value reaches in the FPS box.
                if (viewer.IsCINEPlaying(1, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 8: while playing cine,Left click on fps and drag the scrollbar then set value
                fpsval[0] = viewer.SetFPSValue("30", 1, studypanel);
                result.steps[++ExecutedSteps].status = "Pass";
                //Step 9: Verify requested fps value in the fps box while playing cine
                FPSValue = viewer.GetFPSValueInList(1, studypanel);
                if (FPSValue.All(v => (v >= 0) && (v <= fpsval[0])) && viewer.IsCINEPlaying(1, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 11: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 10: Click on "Pause series" button
                if (viewer.PauseCINE(1, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 11: Click on "Play all in the study"
                viewer.ClickPlayAllOrPauseAll("PlayAll", 1, studypanel);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    if (viewer.IsCINEPlaying(1, studypanel) && viewer.IsCINEPlaying(2, studypanel) & viewer.IsCINEPlaying(3, studypanel) && viewer.IsCINEPlaying(4, studypanel))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (viewer.IsCINEPlaying(1, studypanel) && viewer.IsCINEPlaying(2, studypanel) & viewer.IsCINEPlaying(3, studypanel) && viewer.IsCINEPlaying(4, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 12: In active series viewport,Verify that the requested fps value in the fps box while playing cine
                FPSValue = viewer.GetFPSValueInList(1, studypanel);
                if (FPSValue.All(v => (v >= 0) && (v <= fpsval[0])) && viewer.IsCINEPlaying(1, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 14: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 13: Verify that the remaining series viewports in the viewer and check the values of FPS slider while playing cine and verify the values in the FPS box should be change on Default Frame Rate.
                ViewPortFPS = Enumerable.Repeat(false, 3).ToArray();
                for (int i = 2; i <= 4; i++)
                {
                    FPSValue = viewer.GetFPSValueInList(i, studypanel);
                    if (FPSValue.All(v => (v >= 0) && (v <= fpsval[i - 1])))
                    {
                        ViewPortFPS[i - 2] = true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Step 15: The captured FPS Value on viewport " + i + " is [" + string.Join(",", FPSValue) + "]");
                    }
                }
                if (ViewPortFPS.All(vfp => vfp == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 14: Verify the cine plays in all the images from the selected series viewport and never stops when the default FPS value reaches in the FPS box
                if (viewer.IsCINEPlaying(2, studypanel) & viewer.IsCINEPlaying(3, studypanel) && viewer.IsCINEPlaying(4, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 15: Click on "Pause All in The Study" button in any series viewports
                viewer.ClickPlayAllOrPauseAll("PauseAll", 1, studypanel);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    if (!viewer.IsCINEPlaying(1, studypanel) && !viewer.IsCINEPlaying(2, studypanel) & !viewer.IsCINEPlaying(3, studypanel) && !viewer.IsCINEPlaying(4, studypanel))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (!viewer.IsCINEPlaying(1, studypanel) && !viewer.IsCINEPlaying(2, studypanel) & !viewer.IsCINEPlaying(3, studypanel) && !viewer.IsCINEPlaying(4, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                
                //Step 16: Click on "Play series" button
                if (viewer.PlayCINE(2, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 17: Check the values of FPS slider while playing cine and verify the values in the FPS box should be change on default FPS.
                FPSValue = viewer.GetFPSValueInList(2, studypanel);
                if (FPSValue.All(v => (v >= 0) && (v <= fpsval[1])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 21: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 18: Verify the cine plays in all the images from the selected series viewport and never stops when the default FPS value reaches in the FPS box.
                if (viewer.IsCINEPlaying(2, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 19: while playing cine,Left click on fps and drag the scrollbar then set value
                fpsval[1] = viewer.SetFPSValue("35", 2, studypanel);
                result.steps[++ExecutedSteps].status = "Pass";
                //Step 20: Verify requested fps value in the fps box while playing cine
                FPSValue = viewer.GetFPSValueInList(2, studypanel);
                if (FPSValue.All(v => (v >= 0) && (v <= fpsval[1])) && viewer.IsCINEPlaying(2, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 24: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 21: Click on "Pause series" button
                if (viewer.PauseCINE(2, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 22: Click on "Play all in the study"
                viewer.ClickPlayAllOrPauseAll("PlayAll", 2, studypanel);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    if (viewer.IsCINEPlaying(1, studypanel) && viewer.IsCINEPlaying(2, studypanel) & viewer.IsCINEPlaying(3, studypanel) && viewer.IsCINEPlaying(4, studypanel))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (viewer.IsCINEPlaying(1, studypanel) && viewer.IsCINEPlaying(2, studypanel) & viewer.IsCINEPlaying(3, studypanel) && viewer.IsCINEPlaying(4, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 23: In active series viewport,Verify that the requested fps value in the fps box while playing cine
                FPSValue = viewer.GetFPSValueInList(2, studypanel);
                if (FPSValue.All(v => (v >= 0) && (v <= fpsval[1])) && viewer.IsCINEPlaying(2, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 27: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 24: Verify that the 1st series viewports in the viewer and check the values of FPS slider while playing cine and verify the values in the FPS box should be change on requested Frame Rate.
                FPSValue = viewer.GetFPSValueInList(1, studypanel);
                if (FPSValue.All(v => (v >= 0) && (v <= fpsval[0])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 28: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 25: Verify the cine plays in all the images from the selected series viewport and never stops when the requested FPS value reaches in the FPS box.
                if (viewer.IsCINEPlaying(1, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 26: Verify that the series viewports(3 and 4) in the viewer and check the values of FPS slider while playing cine and verify the values in the FPS box should be change on Default Frame Rate
                ViewPortFPS = Enumerable.Repeat(false, 2).ToArray();
                for (int i = 3, j = 0; i <= 4; i++)
                {
                    FPSValue = viewer.GetFPSValueInList(i, studypanel);
                    if (FPSValue.All(v => (v > 0) && (v < fpsval[i - 1])))
                    {
                        ViewPortFPS[j] = true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Step 26: The captured FPS Value on viewport " + i + " is [" + string.Join(",", FPSValue) + "]");
                    }
                    j++;
                }
                if (ViewPortFPS.All(vfp => vfp == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 27: Verify the cine plays in all the images from the selected series viewport and never stops when the default FPS value reaches in the FPS box.
                if (viewer.IsCINEPlaying(3, studypanel) && viewer.IsCINEPlaying(4, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 28: Click on "Pause All in The Study" button in any series viewports.
                viewer.ClickPlayAllOrPauseAll("PauseAll", 2, studypanel);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    if (!viewer.IsCINEPlaying(1, studypanel) && !viewer.IsCINEPlaying(2, studypanel) & !viewer.IsCINEPlaying(3, studypanel) && !viewer.IsCINEPlaying(4, studypanel))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (!viewer.IsCINEPlaying(1, studypanel) && !viewer.IsCINEPlaying(2, studypanel) & !viewer.IsCINEPlaying(3, studypanel) && !viewer.IsCINEPlaying(4, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                
                //Step 29: Click on "Play series" button
                if (viewer.PlayCINE(3, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 30: Check the values of FPS slider while playing cine and verify the values in the FPS box should be change on default FPS.
                FPSValue = viewer.GetFPSValueInList(3, studypanel);
                if (FPSValue.All(v => (v >= 0) && (v <= fpsval[2])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 36: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 31: Verify the cine plays in all the images from the selected series viewport and never stops when the default FPS value reaches in the FPS box.
                if (viewer.IsCINEPlaying(3, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 32: while playing cine,Left click on fps and drag the scrollbar then set value
                fpsval[2] = viewer.SetFPSValue("40", 3, studypanel);
                result.steps[++ExecutedSteps].status = "Pass";
                //Step 33: Verify requested fps value in the fps box while playing cine
                FPSValue = viewer.GetFPSValueInList(3, studypanel);
                if (FPSValue.All(v => (v >= 0) && (v <= fpsval[2])) && viewer.IsCINEPlaying(3, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 39: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 34: Click on "Pause series" button
                if (viewer.PauseCINE(3, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 35: Click on "Play all in the study"
                viewer.ClickPlayAllOrPauseAll("PlayAll", 3, studypanel);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    if (viewer.IsCINEPlaying(1, studypanel) && viewer.IsCINEPlaying(2, studypanel) & viewer.IsCINEPlaying(3, studypanel) && viewer.IsCINEPlaying(4, studypanel))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (viewer.IsCINEPlaying(1, studypanel) && viewer.IsCINEPlaying(2, studypanel) & viewer.IsCINEPlaying(3, studypanel) && viewer.IsCINEPlaying(4, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 36: In active series viewport,Verify that the requested fps value in the fps box while playing cine
                FPSValue = viewer.GetFPSValueInList(3, studypanel);
                if (FPSValue.All(v => (v >= 0) && (v <= fpsval[2])) && viewer.IsCINEPlaying(3, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 42: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 37: Verify that the 1st series viewports in the viewer and check the values of FPS slider while playing cine and verify the values in the FPS box should be change on requested Frame Rate.
                FPSValue = viewer.GetFPSValueInList(1, studypanel);
                if (FPSValue.All(v => (v >= 0) && (v <= fpsval[0])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 43: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 38: Verify the cine plays in all the images from the selected series viewport and never stops when the requested FPS value reaches in the FPS box.
                if (viewer.IsCINEPlaying(1, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 39: Verify that the 2nd series viewports in the viewer and check the values of FPS slider while playing cine and verify the values in the FPS box should be change on requested Frame Rate.
                FPSValue = viewer.GetFPSValueInList(2, studypanel);
                if (FPSValue.All(v => (v >= 0) && (v <= fpsval[1])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 45: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 40: Verify the cine plays in all the images from the selected series viewport and never stops when the requested FPS value reaches in the FPS box.
                if (viewer.IsCINEPlaying(2, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 41: Verify that the remaining series viewports in the viewer and check the values of FPS slider while playing cine and verify the values in the FPS box should be change on Default Frame Rate.
                FPSValue = viewer.GetFPSValueInList(4, studypanel);
                if (FPSValue.All(v => (v > 0) && (v <= fpsval[3])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 47: The captured FPS Value on viewport 4 is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 42: Verify the cine plays in all the images from the selected series viewport and never stops when the default FPS value reaches in the FPS box.
                if (viewer.IsCINEPlaying(4, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 43: Click on "Pause All in The Study" button in any series viewports
                viewer.ClickPlayAllOrPauseAll("PauseAll", 4, studypanel);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    if (!viewer.IsCINEPlaying(1, studypanel) && !viewer.IsCINEPlaying(2, studypanel) & !viewer.IsCINEPlaying(3, studypanel) && !viewer.IsCINEPlaying(4, studypanel))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (!viewer.IsCINEPlaying(1, studypanel) && !viewer.IsCINEPlaying(2, studypanel) & !viewer.IsCINEPlaying(3, studypanel) && !viewer.IsCINEPlaying(4, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 44: Click on the 'EXIT' icon available on the top right corner of Universal viewer
                viewer.CloseBluRingViewer();
                ExecutedSteps++;
                //Report Result
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                login = new Login();
                //Return Result
                return result;
            }
            finally
            {
                login.Logout();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                basePage.SendKeys(userpreferences.CineDefaultFrameRate(), "20");
                userpreferences.BluringViewerRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                login.Logout();
                BasePage.SetVMResolution("1280", "1024");
            }
        }

        /// <summary> 
        /// Test 141623_2 - Cardio FPS Slider : Study with many priors in Panel 3
        /// </summary>
        public TestCaseResult Test2_160890(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variable
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            BasePage basePage = new BasePage();
            UserPreferences userpreferences = new UserPreferences();
            Studies studies = null;
            BluRingViewer viewer = null;
            string PatientId = null;
            string[] Accession = null;
            IList<int> FPSValue = null;
            Stopwatch stopwatch = new Stopwatch();
            TimeSpan timeout = new TimeSpan(0, 2, 0);
            bool[] ViewPortFPS = null;
            int[] fpsval = Enumerable.Repeat(25, 4).ToArray();
            int studypanel = 3;
            try
            {
                PatientId = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                Accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList")).Split('=');
                //Step 50: In User Preferences dialog, Set any value in the "Cine Default Frame Rate" eg. 25 FPS and click on "OK" button
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                basePage.SendKeys(userpreferences.CineDefaultFrameRate(), "25");
                userpreferences.BluringViewerRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                result.steps[++ExecutedSteps].status = "Pass";
                //Step 51: From Studies tab, search and select same study used in previous test and the click on 'View Exam' button
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                studies.SearchStudy(patientID: PatientId, AccessionNo: Accession[0], Datasource: EA_91);
                PageLoadWait.WaitForSearchLoad();
                studies.SelectStudy("Accession", Accession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                if (viewer.studyPanel().Displayed)
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
                //Step 52: Launch second prior study which has many series(e.g. 3 series) with multiple images from exam list by single clicking on studies card and launch the prior study in viewer.
                viewer.OpenPriors(accession: Accession[1]);
                viewer.OpenPriors(accession: Accession[2]);
                if (viewer.studyPanel(studypanel).Displayed)
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
                //Step 53: Select first series viewport and mouse hover on the bottom part of the series viewport and then verify the Cardio Cine Tool should get appear.
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 54: Verify the actual value of cine fps in the FPS box
                if (string.Equals("0 FPS", viewer.GetFPSValue(1, studypanel)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 55: Click on "Play series" button
                if (viewer.PlayCINE(1, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 56: Check the values of FPS slider while playing cine and verify the values in the FPS box should be change on default FPS.
                FPSValue = viewer.GetFPSValueInList(1, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[0])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 56: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 57: while playing cine,Left click on fps and drag the scrollbar then set value
                viewer.SetFPSValue("45", 1, studypanel);
                fpsval[0] = 45;
                result.steps[++ExecutedSteps].status = "Pass";
                //Step 58: Verify requested fps value in the fps box while playing cine
                FPSValue = viewer.GetFPSValueInList(1, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[0])) && viewer.IsCINEPlaying(1, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 58: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 59: Click on "Pause series" button
                if (viewer.PauseCINE(1, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 60: Click on "Play all in the study"
                viewer.ClickPlayAllOrPauseAll("PlayAll", 1, studypanel);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    if (viewer.IsCINEPlaying(1, studypanel) && viewer.IsCINEPlaying(2, studypanel) & viewer.IsCINEPlaying(3, studypanel) && viewer.IsCINEPlaying(4, studypanel))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (viewer.IsCINEPlaying(1, studypanel) && viewer.IsCINEPlaying(2, studypanel) & viewer.IsCINEPlaying(3, studypanel) && viewer.IsCINEPlaying(4, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 61: In active series viewport,Verify that the requested fps value in the fps box while playing cine
                FPSValue = viewer.GetFPSValueInList(1, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[0])) && viewer.IsCINEPlaying(1, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 61: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 62: Verify that the remaining series viewports in the viewer and check the values of FPS slider while playing cine and verify the values in the FPS box should be change on Default Frame Rate.
                ViewPortFPS = Enumerable.Repeat(false, 3).ToArray();
                for (int i = 2; i <= 4; i++)
                {
                    FPSValue = viewer.GetFPSValueInList(i, studypanel);
                    if (FPSValue.All(v => (v >= 1) && (v <= fpsval[i - 1])))
                    {
                        ViewPortFPS[i - 2] = true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Step 56: The captured FPS Value on viewport " + i + " is [" + string.Join(",", FPSValue) + "]");
                    }
                }
                if (ViewPortFPS.All(vfp => vfp == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 63: Verify the cine plays in all the images from the selected series viewport and never stops when the default FPS value reaches in the FPS box
                if (viewer.IsCINEPlaying(2, studypanel) & viewer.IsCINEPlaying(3, studypanel) && viewer.IsCINEPlaying(4, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 64: Click on "Pause series" button in all visible series viewports
                viewer.ClickPlayAllOrPauseAll("PauseAll", 1, studypanel);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    if (!viewer.IsCINEPlaying(1, studypanel) && !viewer.IsCINEPlaying(2, studypanel) & !viewer.IsCINEPlaying(3, studypanel) && !viewer.IsCINEPlaying(4, studypanel))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (!viewer.IsCINEPlaying(1, studypanel) && !viewer.IsCINEPlaying(2, studypanel) & !viewer.IsCINEPlaying(3, studypanel) && !viewer.IsCINEPlaying(4, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 65: Select second series viewport and mouse hover on the bottom part of the series viewport and then verify the Cardio Cine Tool should get appear.
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 66: Verify the actual value of cine fps in the FPS box
                if (string.Equals("0 FPS", viewer.GetFPSValue(2, studypanel)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 67: Click on "Play series" button
                if (viewer.PlayCINE(2, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 68: Check the values of FPS slider while playing cine and verify the values in the FPS box should be change on default FPS.
                FPSValue = viewer.GetFPSValueInList(2, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[1])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 68: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 69: while playing cine,Left click on fps and drag the scrollbar then set value
                viewer.SetFPSValue("50", 2, studypanel);
                fpsval[1] = 50;
                result.steps[++ExecutedSteps].status = "Pass";
                //Step 70: Verify requested fps value in the fps box while playing cine
                FPSValue = viewer.GetFPSValueInList(2, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[1])) && viewer.IsCINEPlaying(2, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 70: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 71: Click on "Pause series" button
                if (viewer.PauseCINE(2, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 72: Click on "Play all in the study"
                viewer.ClickPlayAllOrPauseAll("PlayAll", 2, studypanel);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    if (viewer.IsCINEPlaying(1, studypanel) && viewer.IsCINEPlaying(2, studypanel) & viewer.IsCINEPlaying(3, studypanel) && viewer.IsCINEPlaying(4, studypanel))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (viewer.IsCINEPlaying(1, studypanel) && viewer.IsCINEPlaying(2, studypanel) & viewer.IsCINEPlaying(3, studypanel) && viewer.IsCINEPlaying(4, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 73: In active series viewport,Verify that the requested fps value in the fps box while playing cine
                FPSValue = viewer.GetFPSValueInList(2, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[1])) && viewer.IsCINEPlaying(2, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 73: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 74: Verify that the 1st series viewports in the viewer and check the values of FPS slider while playing cine and verify the values in the FPS box should be change on requested Frame Rate.
                FPSValue = viewer.GetFPSValueInList(1, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[0])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 74: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 75: Verify the cine plays in all the images from the selected series viewport and never stops when the requested FPS value reaches in the FPS box.
                if (viewer.IsCINEPlaying(1, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 76: Verify that the third series viewport in the viewer and check the values of FPS slider while playing cine and verify the values in the FPS box should be change on Default Frame Rate.
                FPSValue = viewer.GetFPSValueInList(3, studypanel);
                if (FPSValue.All(v => (v > 0) && (v < fpsval[2])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 76: The captured FPS Value on viewport 3 is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 77: Verify the cine plays in all the images from the selected series viewport and never stops when the default FPS value reaches in the FPS box.
                if (viewer.IsCINEPlaying(3, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 78: Click on "Pause All in The Study" button in any series viewports.
                viewer.ClickPlayAllOrPauseAll("PauseAll", 2, studypanel);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    if (!viewer.IsCINEPlaying(1, studypanel) && !viewer.IsCINEPlaying(2, studypanel) & !viewer.IsCINEPlaying(3, studypanel) && !viewer.IsCINEPlaying(4, studypanel))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (!viewer.IsCINEPlaying(1, studypanel) && !viewer.IsCINEPlaying(2, studypanel) & !viewer.IsCINEPlaying(3, studypanel) && !viewer.IsCINEPlaying(4, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 79: Select third series viewport and mouse hover on the bottom part of the series viewport and then verify the Cardio Cine Tool should get appear.
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 80: Verify the actual value of cine fps in the FPS box
                if (string.Equals("0 FPS", viewer.GetFPSValue(3, studypanel)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 81: Click on "Play series" button
                if (viewer.PlayCINE(3, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 82: Check the values of FPS slider while playing cine and verify the values in the FPS box should be change on default FPS.
                FPSValue = viewer.GetFPSValueInList(3, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[2])) && viewer.IsCINEPlaying(3, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 82: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 83: while playing cine,Left click on fps and drag the scrollbar then set value
                viewer.SetFPSValue("55", 3, studypanel);
                fpsval[2] = 55;
                result.steps[++ExecutedSteps].status = "Pass";
                //Step 84: Verify requested fps value in the fps box while playing cine
                FPSValue = viewer.GetFPSValueInList(3, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[2])) && viewer.IsCINEPlaying(3, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 84: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 85: Click on "Pause series" button
                if (viewer.PauseCINE(3, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 86: Click on "Play all in the study"
                viewer.ClickPlayAllOrPauseAll("PlayAll", 3, studypanel);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    if (viewer.IsCINEPlaying(1, studypanel) && viewer.IsCINEPlaying(2, studypanel) & viewer.IsCINEPlaying(3, studypanel) && viewer.IsCINEPlaying(4, studypanel))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (viewer.IsCINEPlaying(1, studypanel) && viewer.IsCINEPlaying(2, studypanel) & viewer.IsCINEPlaying(3, studypanel) && viewer.IsCINEPlaying(4, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 87: In active series viewport,Verify that the requested fps value in the fps box while playing cine
                FPSValue = viewer.GetFPSValueInList(3, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[2])) && viewer.IsCINEPlaying(3, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 87: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 88: Verify that the 1st series viewports in the viewer and check the values of FPS slider while playing cine and verify the values in the FPS box should be change on requested Frame Rate.
                FPSValue = viewer.GetFPSValueInList(1, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[0])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 88: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 89: Verify the cine plays in all the images from the selected series viewport and never stops when the requested FPS value reaches in the FPS box.
                if (viewer.IsCINEPlaying(1, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 90: Verify that the 2nd series viewports in the viewer and check the values of FPS slider while playing cine and verify the values in the FPS box should be change on requested Frame Rate.
                FPSValue = viewer.GetFPSValueInList(2, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[1])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 90: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 91: Verify the cine plays in all the images from the selected series viewport and never stops when the requested FPS value reaches in the FPS box.
                if (viewer.IsCINEPlaying(2, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 92: Click on "Pause All in The Study" button in any series viewports.
                if (viewer.PauseCINE(1, studypanel) && viewer.PauseCINE(2, studypanel) && viewer.PauseCINE(3, studypanel) && viewer.PauseCINE(4, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Report Result
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                login = new Login();
                //Return Result
                return result;
            }
            finally
            {
                login.Logout();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                basePage.SendKeys(userpreferences.CineDefaultFrameRate(), "20");
                userpreferences.BluringViewerRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                login.Logout();
            }
        }

        /// <summary> 
        /// Test 141623_3 - Cardio FPS Slider : Study with many priors in Panel 4
        /// </summary>
        public TestCaseResult Test3_160890(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variable
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            BasePage basePage = new BasePage();
            UserPreferences userpreferences = new UserPreferences();
            Studies studies = null;
            BluRingViewer viewer = null;
            string PatientId = null;
            string[] Accession = null;
            IList<int> FPSValue = null;
            Stopwatch stopwatch = new Stopwatch();
            TimeSpan timeout = new TimeSpan(0, 2, 0);
            bool[] ViewPortFPS = null;
            int[] fpsval = Enumerable.Repeat(30, 4).ToArray();
            int studypanel = 4;
            try
            {
                PatientId = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                Accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList")).Split('=');
                //Step 93: In User Preferences dialog, Set any value in the "Cine Default Frame Rate" eg. 30 FPS and click on "OK" button.
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                basePage.SendKeys(userpreferences.CineDefaultFrameRate(), "30");
                userpreferences.BluringViewerRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                result.steps[++ExecutedSteps].status = "Pass";
                //Step 94: From Studies tab, search and select same study used in previous test and the click on 'View Exam' button
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                studies.SearchStudy(patientID: PatientId, AccessionNo: Accession[0], Datasource: EA_91);
                PageLoadWait.WaitForSearchLoad();
                studies.SelectStudy("Accession", Accession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                if (viewer.studyPanel().Displayed)
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
                //Step 95: Launch third prior study which has many series(e.g. 3 series) with multiple images from exam list by single clicking on studies card and launch the prior study in viewer.
                viewer.OpenPriors(accession: Accession[1]);
                viewer.OpenPriors(accession: Accession[2]);
                viewer.OpenPriors(accession: Accession[3]);
                if (viewer.studyPanel(studypanel).Displayed)
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
                //Step 96: Select first series viewport and mouse hover on the bottom part of the series viewport and then verify the Cardio Cine Tool should get appear.
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 97: Verify the actual value of cine fps in the FPS box
                if (string.Equals("0 FPS", viewer.GetFPSValue(1, studypanel)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 98: Click on "Play series" button
                if (viewer.PlayCINE(1, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 99: Check the values of FPS slider while playing cine and verify the values in the FPS box should be change on default FPS.
                FPSValue = viewer.GetFPSValueInList(1, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[0])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 99: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 100: while playing cine,Left click on fps and drag the scrollbar then set value
                viewer.SetFPSValue("60", 1, studypanel);
                fpsval[0] = 60;
                result.steps[++ExecutedSteps].status = "Pass";
                //Step 101: Verify requested fps value in the fps box while playing cine
                FPSValue = viewer.GetFPSValueInList(1, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[0])) && viewer.IsCINEPlaying(1, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 101: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 102: Click on "Pause series" button
                if (viewer.PauseCINE(1, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 103: Click on "Play all in the study"
                viewer.ClickPlayAllOrPauseAll("PlayAll", 1, studypanel);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    if (viewer.IsCINEPlaying(1, studypanel) && viewer.IsCINEPlaying(2, studypanel) & viewer.IsCINEPlaying(3, studypanel) && viewer.IsCINEPlaying(4, studypanel))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (viewer.IsCINEPlaying(1, studypanel) && viewer.IsCINEPlaying(2, studypanel) & viewer.IsCINEPlaying(3, studypanel) && viewer.IsCINEPlaying(4, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 104: In active series viewport,Verify that the requested fps value in the fps box while playing cine
                FPSValue = viewer.GetFPSValueInList(1, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[0])) && viewer.IsCINEPlaying(1, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 104: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 105: Verify that the remaining series viewports in the viewer and check the values of FPS slider while playing cine and verify the values in the FPS box should be change on Default Frame Rate.
                ViewPortFPS = Enumerable.Repeat(false, 3).ToArray();
                for (int i = 2; i <= 4; i++)
                {
                    FPSValue = viewer.GetFPSValueInList(i, studypanel);
                    if (FPSValue.All(v => (v >= 1) && (v <= fpsval[i - 1])))
                    {
                        ViewPortFPS[i - 2] = true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Step 105: The captured FPS Value on viewport " + i + " is [" + string.Join(",", FPSValue) + "]");
                    }
                }
                if (ViewPortFPS.All(vfp => vfp == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 106: Verify the cine plays in all the images from the selected series viewport and never stops when the default FPS value reaches in the FPS box
                if (viewer.IsCINEPlaying(2, studypanel) & viewer.IsCINEPlaying(3, studypanel) && viewer.IsCINEPlaying(4, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 107: Click on "Pause All in The Study" button in any series viewports.
                viewer.ClickPlayAllOrPauseAll("PauseAll", 1, studypanel);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    if (!viewer.IsCINEPlaying(1, studypanel) && !viewer.IsCINEPlaying(2, studypanel) & !viewer.IsCINEPlaying(3, studypanel) && !viewer.IsCINEPlaying(4, studypanel))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (!viewer.IsCINEPlaying(1, studypanel) && !viewer.IsCINEPlaying(2, studypanel) & !viewer.IsCINEPlaying(3, studypanel) && !viewer.IsCINEPlaying(4, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 108: Select second series viewport and mouse hover on the bottom part of the series viewport and then verify the Cardio Cine Tool should get appear.
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 109 Verify the actual value of cine fps in the FPS box
                if (string.Equals("0 FPS", viewer.GetFPSValue(2, studypanel)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 110: Click on "Play series" button
                if (viewer.PlayCINE(2, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 111: Check the values of FPS slider while playing cine and verify the values in the FPS box should be change on default FPS.
                FPSValue = viewer.GetFPSValueInList(2, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[1])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 111: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 112: while playing cine,Left click on fps and drag the scrollbar then set value
                viewer.SetFPSValue("42", 2, studypanel);
                fpsval[1] = 42;
                result.steps[++ExecutedSteps].status = "Pass";
                //Step 113: Verify requested fps value in the fps box while playing cine
                FPSValue = viewer.GetFPSValueInList(2, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[1])) && viewer.IsCINEPlaying(2, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 113: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 114: Click on "Pause series" button
                if (viewer.PauseCINE(2, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 115: Click on "Play all in the study"
                viewer.ClickPlayAllOrPauseAll("PlayAll", 2, studypanel);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    if (viewer.IsCINEPlaying(1, studypanel) && viewer.IsCINEPlaying(2, studypanel) & viewer.IsCINEPlaying(3, studypanel) && viewer.IsCINEPlaying(4, studypanel))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (viewer.IsCINEPlaying(1, studypanel) && viewer.IsCINEPlaying(2, studypanel) & viewer.IsCINEPlaying(3, studypanel) && viewer.IsCINEPlaying(4, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 116: In active series viewport,Verify that the requested fps value in the fps box while playing cine
                FPSValue = viewer.GetFPSValueInList(2, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[1])) && viewer.IsCINEPlaying(2, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 116: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 117: Verify that the 1st series viewports in the viewer and check the values of FPS slider while playing cine and verify the values in the FPS box should be change on requested Frame Rate.
                FPSValue = viewer.GetFPSValueInList(1, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[0])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 117: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 118: Verify the cine plays in all the images from the selected series viewport and never stops when the requested FPS value reaches in the FPS box.
                if (viewer.IsCINEPlaying(1, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 119: Verify that the third series viewports in the viewer and check the values of FPS slider while playing cine and verify the values in the FPS box should be change on Default Frame Rate.
                FPSValue = viewer.GetFPSValueInList(3, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[2])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 119: The captured FPS Value on viewport 3 is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 120: Verify the cine plays in all the images from the selected series viewport and never stops when the default FPS value reaches in the FPS box.
                if (viewer.IsCINEPlaying(3, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 121: Click on "Pause All in The Study" button in any series viewports.
                viewer.ClickPlayAllOrPauseAll("PauseAll", 2, studypanel);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    if (!viewer.IsCINEPlaying(1, studypanel) && !viewer.IsCINEPlaying(2, studypanel) & !viewer.IsCINEPlaying(3, studypanel) && !viewer.IsCINEPlaying(4, studypanel))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (!viewer.IsCINEPlaying(1, studypanel) && !viewer.IsCINEPlaying(2, studypanel) & !viewer.IsCINEPlaying(3, studypanel) && !viewer.IsCINEPlaying(4, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 122: Select third series viewport and mouse hover on the bottom part of the series viewport and then verify the Cardio Cine Tool should get appear.
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 123: Verify the actual value of cine fps in the FPS box
                if (string.Equals("0 FPS", viewer.GetFPSValue(3, studypanel)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 124: Click on "Play series" button
                if (viewer.PlayCINE(3, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 125: Check the values of FPS slider while playing cine and verify the values in the FPS box should be change on default FPS.
                FPSValue = viewer.GetFPSValueInList(3, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[2])) && viewer.IsCINEPlaying(3, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 113: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 126: while playing cine,Left click on fps and drag the scrollbar then set value
                viewer.SetFPSValue("15", 3, studypanel);
                fpsval[2] = 15;
                result.steps[++ExecutedSteps].status = "Pass";
                //Step 127: Verify requested fps value in the fps box while playing cine
                FPSValue = viewer.GetFPSValueInList(3, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[2])) && viewer.IsCINEPlaying(3, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 127: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 128: Click on "Pause series" button
                if (viewer.PauseCINE(3, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 129: Click on "Play all in the study"
                viewer.ClickPlayAllOrPauseAll("PlayAll", 3, studypanel);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    if (viewer.IsCINEPlaying(1, studypanel) && viewer.IsCINEPlaying(2, studypanel) & viewer.IsCINEPlaying(3, studypanel) && viewer.IsCINEPlaying(4, studypanel))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (viewer.IsCINEPlaying(1, studypanel) && viewer.IsCINEPlaying(2, studypanel) & viewer.IsCINEPlaying(3, studypanel) && viewer.IsCINEPlaying(4, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 130: In active series viewport,Verify that the requested fps value in the fps box while playing cine
                FPSValue = viewer.GetFPSValueInList(3, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[2])) && viewer.IsCINEPlaying(3, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 130: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 131: Verify that the 1st series viewports in the viewer and check the values of FPS slider while playing cine and verify the values in the FPS box should be change on requested Frame Rate.
                FPSValue = viewer.GetFPSValueInList(1, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[0])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 131: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 132: Verify the cine plays in all the images from the selected series viewport and never stops when the requested FPS value reaches in the FPS box.
                if (viewer.IsCINEPlaying(1, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 133: Verify that the 2nd series viewports in the viewer and check the values of FPS slider while playing cine and verify the values in the FPS box should be change on requested Frame Rate.
                FPSValue = viewer.GetFPSValueInList(2, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[1])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 133: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 134: Verify the cine plays in all the images from the selected series viewport and never stops when the requested FPS value reaches in the FPS box.
                if (viewer.IsCINEPlaying(2, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 135: Click on "Pause All in The Study" button in any series viewports.
                viewer.ClickPlayAllOrPauseAll("PauseAll", 3, studypanel);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    if (!viewer.IsCINEPlaying(1, studypanel) && !viewer.IsCINEPlaying(2, studypanel) & !viewer.IsCINEPlaying(3, studypanel) && !viewer.IsCINEPlaying(4, studypanel))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (!viewer.IsCINEPlaying(1, studypanel) && !viewer.IsCINEPlaying(2, studypanel) & !viewer.IsCINEPlaying(3, studypanel) && !viewer.IsCINEPlaying(4, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Report Result
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                login = new Login();
                //Return Result
                return result;
            }
            finally
            {
                login.Logout();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                basePage.SendKeys(userpreferences.CineDefaultFrameRate(), "20");
                userpreferences.BluringViewerRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                login.Logout();
            }
        }

        /// <summary> 
        /// Test 141623_3 - Cardio FPS Slider : Study with many priors in Panel 5
        /// </summary>
        public TestCaseResult Test4_160890(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variable
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            BasePage basePage = new BasePage();
            UserPreferences userpreferences = new UserPreferences();
            Studies studies = null;
            BluRingViewer viewer = null;
            string PatientId = null;
            string[] Accession = null;
            IList<int> FPSValue = null;
            Stopwatch stopwatch = new Stopwatch();
            TimeSpan timeout = new TimeSpan(0, 2, 0);
            bool[] ViewPortFPS = null;
            int[] fpsval = Enumerable.Repeat(15, 4).ToArray();
            int studypanel = 5;
            try
            {
                PatientId = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                Accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList")).Split('=');
                //Step 136: In User Preferences dialog, Set any value in the "Cine Default Frame Rate" eg. 15 FPS and click on "OK" button.
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                basePage.SendKeys(userpreferences.CineDefaultFrameRate(), "15");
                userpreferences.BluringViewerRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                result.steps[++ExecutedSteps].status = "Pass";
                //Step 137: From Studies tab, search and select same study used in previous test and the click on 'View Exam' button
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                studies.SearchStudy(patientID: PatientId, AccessionNo: Accession[0], Datasource: EA_91);
                PageLoadWait.WaitForSearchLoad();
                studies.SelectStudy("Accession", Accession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                if (viewer.studyPanel().Displayed)
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
                //Step 138: Launch fourth prior study which has many series(e.g. 3 series) with multiple images from exam list by single clicking on studies card and launch the prior study in viewer.
                viewer.OpenPriors(accession: Accession[1]);
                viewer.OpenPriors(accession: Accession[2]);
                viewer.OpenPriors(accession: Accession[3]);
                viewer.OpenPriors(accession: Accession[4]);
                if (viewer.studyPanel(studypanel).Displayed)
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
                //Step 139: Select first series viewport and mouse hover on the bottom part of the series viewport and then verify the Cardio Cine Tool should get appear.
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 140: Verify the actual value of cine fps in the FPS box
                if (string.Equals("0 FPS", viewer.GetFPSValue(1, studypanel)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 141: Click on "Play series" button
                if (viewer.PlayCINE(1, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 142: Check the values of FPS slider while playing cine and verify the values in the FPS box should be change on default FPS.
                FPSValue = viewer.GetFPSValueInList(1, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[0])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 142: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 143: Verify the cine plays in all the images from the selected series viewport and never stops when the default FPS value reaches in the FPS box.
                if (viewer.IsCINEPlaying(1, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 144: while playing cine,Left click on fps and drag the scrollbar then set value
                viewer.SetFPSValue("41", 1, studypanel);
                fpsval[0] = 41;
                result.steps[++ExecutedSteps].status = "Pass";
                //Step 145: Verify requested fps value in the fps box while playing cine
                FPSValue = viewer.GetFPSValueInList(1, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[0])) && viewer.IsCINEPlaying(1, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 145: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 146: Click on "Pause series" button
                if (viewer.PauseCINE(1, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 147: Click on "Play all in the study"
                viewer.ClickPlayAllOrPauseAll("PlayAll", 1, studypanel);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    if (viewer.IsCINEPlaying(1, studypanel) && viewer.IsCINEPlaying(2, studypanel) & viewer.IsCINEPlaying(3, studypanel) && viewer.IsCINEPlaying(4, studypanel))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (viewer.IsCINEPlaying(1, studypanel) && viewer.IsCINEPlaying(2, studypanel) & viewer.IsCINEPlaying(3, studypanel) && viewer.IsCINEPlaying(4, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 148: In active series viewport,Verify that the requested fps value in the fps box while playing cine
                FPSValue = viewer.GetFPSValueInList(1, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[0])) && viewer.IsCINEPlaying(1, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 148: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 149: Verify that the remaining series viewports in the viewer and check the values of FPS slider while playing cine and verify the values in the FPS box should be change on Default Frame Rate.
                ViewPortFPS = Enumerable.Repeat(false, 3).ToArray();
                for (int i = 2; i <= 4; i++)
                {
                    FPSValue = viewer.GetFPSValueInList(i, studypanel);
                    if (FPSValue.All(v => (v >= 1) && (v <= fpsval[i - 1])))
                    {
                        ViewPortFPS[i - 2] = true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Step 149: The captured FPS Value on viewport " + i + " is [" + string.Join(",", FPSValue) + "]");
                    }
                }
                if (ViewPortFPS.All(vfp => vfp == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 150: Verify the cine plays in all the images from the selected series viewport and never stops when the default FPS value reaches in the FPS box
                if (viewer.IsCINEPlaying(2, studypanel) & viewer.IsCINEPlaying(3, studypanel) && viewer.IsCINEPlaying(4, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 151: verify that the requested fps value in the fps box while playing cine
                FPSValue = viewer.GetFPSValueInList(1, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[0])) && viewer.IsCINEPlaying(1, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 151: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 152: Click on "Pause All in The Study" button in any series viewports.
                viewer.ClickPlayAllOrPauseAll("PauseAll", 1, studypanel);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    if (!viewer.IsCINEPlaying(1, studypanel) && !viewer.IsCINEPlaying(2, studypanel) & !viewer.IsCINEPlaying(3, studypanel) && !viewer.IsCINEPlaying(4, studypanel))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (!viewer.IsCINEPlaying(1, studypanel) && !viewer.IsCINEPlaying(2, studypanel) & !viewer.IsCINEPlaying(3, studypanel) && !viewer.IsCINEPlaying(4, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 153: Select second series viewport and mouse hover on the bottom part of the series viewport and then verify the Cardio Cine Tool should get appear.
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 154 Verify the actual value of cine fps in the FPS box
                if (string.Equals("0 FPS", viewer.GetFPSValue(2, studypanel)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 155: Click on "Play series" button
                if (viewer.PlayCINE(2, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 156: Check the values of FPS slider while playing cine and verify the values in the FPS box should be change on default FPS.
                FPSValue = viewer.GetFPSValueInList(2, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[1])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 156: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 157: while playing cine,Left click on fps and drag the scrollbar then set value
                viewer.SetFPSValue("45", 2, studypanel);
                fpsval[1] = 45;
                result.steps[++ExecutedSteps].status = "Pass";
                //Step 158: Verify requested fps value in the fps box while playing cine
                FPSValue = viewer.GetFPSValueInList(2, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[1])) && viewer.IsCINEPlaying(2, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 113: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 159: Click on "Pause series" button
                if (viewer.PauseCINE(2, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 160: Click on "Play all in the study"
                viewer.ClickPlayAllOrPauseAll("PlayAll", 2, studypanel);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    if (viewer.IsCINEPlaying(1, studypanel) && viewer.IsCINEPlaying(2, studypanel) & viewer.IsCINEPlaying(3, studypanel) && viewer.IsCINEPlaying(4, studypanel))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (viewer.IsCINEPlaying(1, studypanel) && viewer.IsCINEPlaying(2, studypanel) & viewer.IsCINEPlaying(3, studypanel) && viewer.IsCINEPlaying(4, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 161: In active series viewport,Verify that the requested fps value in the fps box while playing cine
                FPSValue = viewer.GetFPSValueInList(2, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[1])) && viewer.IsCINEPlaying(2, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 161: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 162: Verify that the 1st series viewports in the viewer and check the values of FPS slider while playing cine and verify the values in the FPS box should be change on requested Frame Rate.
                FPSValue = viewer.GetFPSValueInList(1, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[0])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 162: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 163: Verify the cine plays in all the images from the selected series viewport and never stops when the requested FPS value reaches in the FPS box.
                if (viewer.IsCINEPlaying(1, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 164: Verify that the third series viewports in the viewer and check the values of FPS slider while playing cine and verify the values in the FPS box should be change on Default Frame Rate.
                FPSValue = viewer.GetFPSValueInList(3, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[2])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 119: The captured FPS Value on viewport 3 is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 165: Verify the cine plays in all the images from the selected series viewport and never stops when the default FPS value reaches in the FPS box.
                if (viewer.IsCINEPlaying(3, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 166: Click on "Pause All in The Study" button in any series viewports.
                viewer.ClickPlayAllOrPauseAll("PauseAll", 2, studypanel);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    if (!viewer.IsCINEPlaying(1, studypanel) && !viewer.IsCINEPlaying(2, studypanel) & !viewer.IsCINEPlaying(3, studypanel) && !viewer.IsCINEPlaying(4, studypanel))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (!viewer.IsCINEPlaying(1, studypanel) && !viewer.IsCINEPlaying(2, studypanel) & !viewer.IsCINEPlaying(3, studypanel) && !viewer.IsCINEPlaying(4, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 167: Select third series viewport and mouse hover on the bottom part of the series viewport and then verify the Cardio Cine Tool should get appear.
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 168: Verify the actual value of cine fps in the FPS box
                if (string.Equals("0 FPS", viewer.GetFPSValue(3, studypanel)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 169: Click on "Play series" button
                if (viewer.PlayCINE(3, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 170: Check the values of FPS slider while playing cine and verify the values in the FPS box should be change on default FPS.
                FPSValue = viewer.GetFPSValueInList(3, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[2])) && viewer.IsCINEPlaying(3, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 170: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 171: while playing cine,Left click on fps and drag the scrollbar then set value
                viewer.SetFPSValue("35", 3, studypanel);
                fpsval[2] = 35;
                result.steps[++ExecutedSteps].status = "Pass";
                //Step 172: Verify requested fps value in the fps box while playing cine
                FPSValue = viewer.GetFPSValueInList(3, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[2])) && viewer.IsCINEPlaying(3, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 172: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 173: Click on "Pause series" button
                if (viewer.PauseCINE(3, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 174: Click on "Play all in the study"
                viewer.ClickPlayAllOrPauseAll("PlayAll", 3, studypanel);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    if (viewer.IsCINEPlaying(1, studypanel) && viewer.IsCINEPlaying(2, studypanel) & viewer.IsCINEPlaying(3, studypanel) && viewer.IsCINEPlaying(4, studypanel))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (viewer.IsCINEPlaying(1, studypanel) && viewer.IsCINEPlaying(2, studypanel) & viewer.IsCINEPlaying(3, studypanel) && viewer.IsCINEPlaying(4, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 175: In active series viewport,Verify that the requested fps value in the fps box while playing cine
                FPSValue = viewer.GetFPSValueInList(3, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[2])) && viewer.IsCINEPlaying(3, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 175: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 176: Verify that the 1st series viewports in the viewer and check the values of FPS slider while playing cine and verify the values in the FPS box should be change on requested Frame Rate.
                FPSValue = viewer.GetFPSValueInList(1, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[0])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 176: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 177: Verify the cine plays in all the images from the selected series viewport and never stops when the requested FPS value reaches in the FPS box.
                if (viewer.IsCINEPlaying(1, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 178: Verify that the 2nd series viewports in the viewer and check the values of FPS slider while playing cine and verify the values in the FPS box should be change on requested Frame Rate.
                FPSValue = viewer.GetFPSValueInList(2, studypanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[1])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 133: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 179: Verify the cine plays in all the images from the selected series viewport and never stops when the requested FPS value reaches in the FPS box.
                if (viewer.IsCINEPlaying(2, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 180: Click on "Pause All in The Study" button in any series viewports.
                viewer.ClickPlayAllOrPauseAll("PauseAll", 3, studypanel);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    if (!viewer.IsCINEPlaying(1, studypanel) && !viewer.IsCINEPlaying(2, studypanel) & !viewer.IsCINEPlaying(3, studypanel) && !viewer.IsCINEPlaying(4, studypanel))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (!viewer.IsCINEPlaying(1, studypanel) && !viewer.IsCINEPlaying(2, studypanel) & !viewer.IsCINEPlaying(3, studypanel) && !viewer.IsCINEPlaying(4, studypanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Report Result
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                login = new Login();
                //Return Result
                return result;
            }
            finally
            {
                login.Logout();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                basePage.SendKeys(userpreferences.CineDefaultFrameRate(), "20");
                userpreferences.BluringViewerRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                login.Logout();
            }
        }

        /// <summary> 
        ///  Play cine, scroll while playing - Dropping another series onto an actively playing Cine sequence 
        /// </summary>
        public TestCaseResult Test_160900(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                BasePage.SetVMResolution("1980", "1080");
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] accession = AccessionID.Split(':');

                DomainManagement domain = new DomainManagement();
                RoleManagement role = new RoleManagement();
                UserManagement user = new UserManagement();
                Studies study = new Studies();
                UserPreferences userpref = new UserPreferences();
                BluRingViewer viewer = new BluRingViewer();

                //Step-1  Login to application with any privileged user.
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login = new Login();
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                if (login.IsTabPresent("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step2 - In User preferences set the thumbnail split to series for the modality to which the listed study belongs
                login.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                domain.ModalityDropDown().SelectByText("MR");
                domain.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                PageLoadWait.WaitForPageLoad(20);
                study.CloseUserPreferences();
                ExecutedSteps++;

                //step3 - Search and load for a study with multiple series multiple images
                study = (Studies)login.Navigate("Studies");
                study.SearchStudy(AccessionNo: accession[1], Datasource: EA_91);
                study.SelectStudy("Accession", accession[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //step4 - Load first series in first viewport, second series in second viewport and so on
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step4)
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

                //step5 - 
                viewer.OpenCineToolBar(1, 1);
                bool PlayBtn = viewer.IsElementVisible(By.CssSelector(BluRingViewer.div_CINE_PlayBtn));
                bool NextBtn = viewer.IsElementVisible(By.CssSelector(BluRingViewer.div_CINE_NextImageBtn));
                bool PrevBtn = viewer.IsElementVisible(By.CssSelector(BluRingViewer.div_CINE_PreviousImageBtn));
                bool FPS = viewer.IsElementVisible(By.CssSelector(BluRingViewer.div_CINE_FPS));
                bool ExamMode = viewer.IsElementVisible(By.CssSelector(BluRingViewer.button_ExamMode));
                if (PlayBtn && NextBtn && PrevBtn && FPS && ExamMode)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.ErrorLog("PlayBtn " + PlayBtn);
                    Logger.Instance.ErrorLog("NextBtn " + NextBtn);
                    Logger.Instance.ErrorLog("PrevBtn " + PrevBtn);
                    Logger.Instance.ErrorLog("FPS " + FPS);
                    Logger.Instance.ErrorLog("ExamMode " + ExamMode);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step6 - Click on "Play Series" button
                bool step6 = viewer.PlayCINE(1, 1);
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

                //step7 - Verify the images being played
                Thread.Sleep(5000);
                IList<IWebElement> PercentViewedList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImageComponent + " " + BluRingViewer.div_thumbnailPercentImagesViewed));
                bool step7_1 = viewer.VerifyThumbnailPercentImagesViewed(PercentViewedList[0], 22, 22);
                bool step7_2 = viewer.IsCINEPlaying(1, 1);
                if (step7_1 && step7_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step8 - Scroll the mouse wheel in the selected series viewport
                viewer.PauseCINE(1, 1);
                int numofimg_1 = int.Parse(BasePage.Driver.FindElement(By.CssSelector("div.viewerContainer:nth-of-type(1) " + BluRingViewer.div_StackSlider)).GetAttribute("innerHTML"));
                var element = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                Thread.Sleep(Config.medTimeout);
                var action = new TestCompleteAction();
                action.MouseScroll(element, "down", "1").Perform();
                Thread.Sleep(5000);
                int numofimg_2 = int.Parse(BasePage.Driver.FindElement(By.CssSelector("div.viewerContainer:nth-of-type(1) " + BluRingViewer.div_StackSlider)).GetAttribute("innerHTML"));
                bool step8_1 = !viewer.IsCINEPlaying(1, 1);
                bool step8_2 = numofimg_1 != numofimg_2;
                if (step8_1 && step8_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.ErrorLog("step8_1 = " + step8_1);
                    Logger.Instance.ErrorLog("step8_2 = " + step8_2);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

				//Step9 - Again click on "Play Series" button and Cine plays all images from the series and never stops
				viewer.PlayCINE(1, 1);
				bool step9 = viewer.IsCINEPlaying(1, 1);
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

				//step10 - Drag and Drop or Double click another series from the thumbnail into an active viewport and verify that the Cine player mode should stop in the current(active) viewport.
				viewer.ClickOnThumbnailsInStudyPanel(1, 3, true);
                bool step10 = viewer.IsCINEPlaying(1, 1);
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

                //step11 - Click on "Play All in this Study" button
                //viewer.VerifyCardioCINEToolbarOnMouseHover();
                viewer.ClickPlayAllOrPauseAll("PlayAll", 1, 1);
                viewer.WaitForThumbnailPercentageTo100(4);
                if (viewer.IsCINEPlaying(1, 1) && viewer.IsCINEPlaying(2, 1) && viewer.IsCINEPlaying(3, 1) && viewer.IsCINEPlaying(4, 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step12 - Scroll the mouse wheel in the selected series viewport
                element = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                Thread.Sleep(Config.medTimeout);
                action = new TestCompleteAction();
                action.MouseScroll(element, "up", "1").Perform();
                Thread.Sleep(5000);
                var step11 = viewer.IsCINEPlaying(1, 1);
                if (!step11)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

				//Step13 - Again click on "Play Series" button in any series viewport which has more images and Cine plays all images from the series and never stops
				viewer.PlayCINE(1, 1);
				bool step13 = viewer.IsCINEPlaying(1, 1);
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

				//step14 - Drag and Drop or Double click another series from the thumbnail onto an actively playing cine in a series viewport and verify that the Cine player mode should stop in the current(active) series viewport.
				viewer.ClickOnThumbnailsInStudyPanel(1, 4, true);
                bool step14 = viewer.IsCINEPlaying(1, 1);
                if (!step14)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
				
                //step-15 - Click Next Series button
                viewer.ClickNextSeriesCINE();
                IList<IWebElement> Thumbnail_Outer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ThumbnailOuter));
                if (viewer.VerifyBordorColor(Thumbnail_Outer[4], "rgba(90, 170, 255, 1)") &&
                    viewer.VerifyBordorColor(Thumbnail_Outer[0], "rgba(0, 0, 0, 1)"))
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

                //step-16 -  Click Previous Series button 
                viewer.ClickPreviousSeriesCINE(1, 1);
                Thumbnail_Outer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ThumbnailOuter));
                if (viewer.VerifyBordorColor(Thumbnail_Outer[3], "rgba(90, 170, 255, 1)") &&
                    viewer.VerifyBordorColor(Thumbnail_Outer[0], "rgba(0, 0, 0, 1)"))
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

				//step17 - Click on 'Exit' button
				viewer.CloseBluRingViewer();
				ExecutedSteps++;

				//Step18 -Load any study which has priors with more series and more images in each series into the universal viewer
				study.SearchStudy("Accession", accession[0]);
				study.SelectStudy("Accession", accession[0]);
				viewer = BluRingViewer.LaunchBluRingViewer();
				ExecutedSteps++;

				/*
                //step-15 - Click Next Series button
                viewer.ClickNextSeriesCINE();
                Thumbnail_Outer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ThumbnailOuter));
                Logger.Instance.InfoLog("Thumbnail 4 border color is: " + Thumbnail_Outer[4].GetCssValue("border-color") + " border color of thumbnail 0 is: " + Thumbnail_Outer[0].GetCssValue("border-color"));
                if (viewer.VerifyBordorColor(Thumbnail_Outer[4], "rgba(90, 170, 255, 1)") &&
                    viewer.VerifyBordorColor(Thumbnail_Outer[0], "rgba(0, 0, 0, 1)"))
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

                //step-16  -  Click Previous Series button
                viewer.ClickPreviousSeriesCINE(1, 1);
                Thumbnail_Outer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ThumbnailOuter));
                Logger.Instance.InfoLog("Thumbnail 4 border color is: " + Thumbnail_Outer[3].GetCssValue("border-color") + " border color of thumbnail 0 is: " + Thumbnail_Outer[0].GetCssValue("border-color"));
                if (viewer.VerifyBordorColor(Thumbnail_Outer[3], "rgba(90, 170, 255, 1)") &&
                    viewer.VerifyBordorColor(Thumbnail_Outer[0], "rgba(0, 0, 0, 1)"))
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

                //step17 - Click on 'Exit' button and Navigate to Studies tab and then load the study which has many priors with many images/frames
                viewer.CloseBluRingViewer();
                study.SearchStudy("Accession", accession[0]);
                study.SelectStudy("Accession", accession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //step18 - Mouse hover on the bottom part of the series viewport and verify the Cardio Cine Tool should get appear.
                if (viewer.VerifyCardioCINEToolbarOnMouseHover())
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

                //step19 - Click on "Play Series" button
                bool step19 = viewer.PlayCINE();
                if (step19)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step20 - Scroll the mouse wheel in the selected series viewport
                element = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                Thread.Sleep(Config.medTimeout);
                action = new TestCompleteAction();
                action.MouseScroll(element, "up", "1").Perform();
                Thread.Sleep(5000);
                bool step20 = viewer.IsCINEPlaying();
                if (!step20)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step21 - Drag and Drop or Double click another series from the thumbnail onto an actively playing cine in a series viewport and verify that the Cine player mode should stop in the current(active) series viewport.
                viewer.ClickOnThumbnailsInStudyPanel(1, 2, true);
                bool step21 = viewer.IsCINEPlaying();
                if (!step21)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step22 - Click on "Play All in this Study" button
                viewer.VerifyCardioCINEToolbarOnMouseHover();
                viewer.ClickPlayAllOrPauseAll("PlayAll");
                viewer.WaitForThumbnailPercentageTo100(4);
                if (viewer.IsCINEPlaying(1, 1) && viewer.IsCINEPlaying(2, 1) && viewer.IsCINEPlaying(3, 1) && viewer.IsCINEPlaying(4, 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step23 - Scroll the mouse wheel in the selected series viewport
                element = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                Thread.Sleep(Config.medTimeout);
                action = new TestCompleteAction();
                action.MouseScroll(element, "up", "1").Perform();
                Thread.Sleep(5000);
                bool step23 = viewer.IsCINEPlaying();
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

                //step24 - Drag and Drop or Double click another series from the thumbnail into an active viewport and verify that the Cine player mode should stop in the current(active) viewport and load the .
                viewer.ClickOnThumbnailsInStudyPanel(1, 3, true);
                bool step24 = viewer.IsCINEPlaying();
                if (!step24)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
				*/

				//step19 - Load any prior from the exam list
				int studypanel = viewer.GetStudyPanelCount();
                float studyPanelLeft = float.Parse(viewer.AllstudyPanel()[0].GetCssValue("left").Replace("px", ""));
                viewer.OpenPriors(1, "click");
                Thread.Sleep(1000);
                bool newstudyPanleOpened = (viewer.GetStudyPanelCount() > studypanel);
                bool newStudyPosition = (float.Parse(viewer.AllstudyPanel()[1].GetCssValue("left").Replace("px", "")) > studyPanelLeft);
                if (newstudyPanleOpened && newStudyPosition)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("The study opened in a new study panel to the right of the current exam or to the right of last prior study opened.");
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.ErrorLog("The study not opened in a new study panel to the right of the current exam or to the right of last prior study opened.");
                }

				//Step20 - Select any series and click on Play series button
				viewer.PlayCINE(1, 2);
				bool step20 = viewer.IsCINEPlaying(1, 2);
				if (step20)
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//step21 - Scroll the mouse wheel in the selected series viewport
				element = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                Thread.Sleep(Config.medTimeout);
                action = new TestCompleteAction();
                action.MouseScroll(element, "up", "1").Perform();
                Thread.Sleep(5000);
                bool step21 = viewer.IsCINEPlaying(1, 2);
                if (!step21)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
				}

				//Step22 - Again click on Play series button
				viewer.PlayCINE(1, 2);
				bool step22 = viewer.IsCINEPlaying(1, 2);
				if (step22)
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//step23 - Drag and Drop or Double click another series from the thumbnail onto an actively playing cine in a series viewport and verify that the Cine player mode should stop in the current(active) series viewport.
				viewer.ClickOnThumbnailsInStudyPanel(2, 4, true);
                bool step23 = viewer.IsCINEPlaying(1, 2);
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

                //Logout Application
                viewer.CloseBluRingViewer();
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
            finally
            {
                BasePage.SetVMResolution("1280", "1024");
            }
        }

        /// <summary> 
        ///  Cine Play - Images has (0008,2144) 
        /// </summary>
        public TestCaseResult Test_160896(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            UserPreferences userpref = new UserPreferences();


            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String PatientId = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");

                Studies study = new Studies();
                BluRingViewer viewer = new BluRingViewer();

                //Pre-condition 
                // In User Preferences set the Thumbnail Splitting and Viewing Scope as Image.
                login.LoginIConnect(adminUserName, adminPassword);
                study.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpref.ModalityDropDown().SelectByText("US");
                userpref.ThumbnailSplittingImageRadioBtn().Click();
                userpref.ViewingScopeImageRadioBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                study.CloseUserPreferences();
                login.Logout();

                //step1   Login to iCA Enterprise Viewer as a privilege user 
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //step2   In User preferences set the Set viewer layout to 1x3 for the US modality and also Note the default fps (20 fps)from User preference and then click on "Save" button.
                study.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpref.ModalityDropDown().SelectByText("US");
                userpref.LayoutDropDown().SelectByText("1x3");
                PageLoadWait.WaitForPageLoad(20);
                study.CloseUserPreferences();
                ExecutedSteps++;

                //step3  Search for study (e.g., patient name "Clevin,Clark" patient Id =PID@54454 ) and load it into the viewer.
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(LastName: LastName, FirstName: FirstName, patientID: PatientId);
                studies.SelectStudy("Patient ID", PatientId);
                viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //step4 In active series viewport,verify the actual value of cine fps in the FPS box.
                if (viewer.GetFPSValue(1, 1).Equals("0 FPS"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step5 - Mouse hover on the bottom part of the first series viewport and then verify the Cardio Cine Tool should get appear.
                result.steps[++ExecutedSteps].status = "Not Automated";


                //step6 - Click on "Play Series" button on the selected viewport.
                if (viewer.PlayCINE(1, 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step7  Check the values of FPS slider while playing cine and verify the values in the FPS box should be change as mentioned in DICOM tag (0008,2144)
                /*IList<int> fpsvalue = viewer.GetFPSValueInList(1, 1);
                if (1 <= fpsvalue[0] && fpsvalue[0] <= 50)
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

                //step8 Verify the cine plays in all the images from the selected series viewport and never stops when the data FPS value reaches in the FPS slider text box.
                bool step8_1 = false;
                String FPSvalue = viewer.GetFPSValue();
                if (!FPSvalue.Equals("50 FPS"))
                {
                    for (int i = 0; i < 90; i++)
                    {
                        FPSvalue = viewer.GetFPSValue();
                        if (!FPSvalue.Equals("50 FPS"))
                        {
                            Thread.Sleep(500);
                        }
                        else
                        {
                            step8_1 = true;
                            break;
                        }
                    }
                }
                else
                {
                    step8_1 = true;
                }

                bool step8_2 = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1, 1) + " " + BluRingViewer.div_thumbnailPercentImagesViewed).GetAttribute("innerHTML").Equals("100%");
                if (step8_1 && step8_2)
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

                //step9 Verify the images being played based on Cine fps mentioned in DICOM tag (0008,2144) and also verify the images should not be played based on default fps which is noted in step2.
                IList<int> fpsvalue_1 = viewer.GetFPSValueInList(1, 1);
                if (20 < fpsvalue_1[0] && fpsvalue_1[0] <= 50)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }*/
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                //step10 Click on "Pause Series" button in a viewport
                if (viewer.PauseCINE(1, 1))
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

                //Logout Application
                viewer.CloseBluRingViewer();
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

            finally
            {
                // Revert the US modality layout to auto
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpref.ModalityDropDown().SelectByText("US");
                userpref.ThumbnailSplittingSeriesRadioBtn().Click();
                userpref.ViewingScopeSeriesRadioBtn().Click();
                userpref.LayoutDropDown().SelectByText("auto");
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                login.Logout();
            }
        }

        /// <summary> 
        /// Cine PET/CT Study: Drag and Drop the series before the study is getting cached and verify the images shall not go blank.
        /// </summary>
        public TestCaseResult Test_160895(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            UserPreferences userpref = new UserPreferences();
            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String PatientId = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");

                Studies study = new Studies();
                BluRingViewer viewer = new BluRingViewer();

                //step1 Login to iCA Enterprise Viewer as a privilege user (i.e., Administrator/Administrator) 
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //step2 In User preferences set the thumbnail split to series for the modality to which the listed study belongs and click on "Save" button.
                userpreference.OpenUserPreferences();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                String[] Modality = { "PT", "CT" };
                foreach (String s in Modality)
                {
                    userpreference.ModalityDropDown().SelectByText(s);
                    userpreference.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                    PageLoadWait.WaitForPageLoad(20);
                }
                int DefaultFpsvalue = Convert.ToInt16(userpreference.CineDefaultFrameRate().GetAttribute("value"));
                userpreference.CloseUserPreferences();
                ExecutedSteps++;

                //step3 Search for study (e.g., patient name "BLT,PT" patient Id =BLT-04) and load it into the BluRing viewer.
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(LastName: LastName, FirstName: FirstName, patientID: PatientId, Datasource: EA_131);
                studies.SelectStudy("Patient ID", PatientId);
                viewer = BluRingViewer.LaunchBluRingViewer();
                if (viewer.studyPanel().Displayed)
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

                //step4 Load first series in first viewport, second series in second viewport and so on
                viewer.SetSeriesInViewport(0, 1);
                viewer.SetSeriesInViewport(1, 1);
                viewer.SetSeriesInViewport(2, 1);
                ExecutedSteps++;

                //step5 Drag and drop the study to the viewport before the study is getting Cached and press the Cine icon from the series toolbar.
                IList<IWebElement> thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));
                viewer.ClickOnViewPort(1, 1);
                IWebElement TargetElement = viewer.GetElement("cssselector", viewer.Activeviewport);
                TestCompleteAction action = new TestCompleteAction();
                action.DragAndDrop(thumbnails.ElementAt(0), TargetElement);
                bool step5 = viewer.PlayCINE(1, 1);
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

                //step6 Verify whether viewport should not go blank and image load slowly.
                result.steps[++ExecutedSteps].status = "No Automation";

                //step7 After the buffering is complete verify the images being played.
                /*bool step7 = viewer.WaitForThumbnailPercentageTo100(1);
                bool step7_1 = false;
                String FPSvalue = viewer.GetFPSValue();
                if (!FPSvalue.Equals("20 FPS"))
                {
                    for (int i = 0; i < 90; i++)
                    {
                        FPSvalue = viewer.GetFPSValue();
                        if (!FPSvalue.Equals("20 FPS"))
                        {
                            Thread.Sleep(500);
                        }
                        else
                        {
                            step7_1 = true;
                            break;
                        }
                    }
                }
                else
                {
                    step7_1 = true;
                }
                bool step7_2 = viewer.IsCINEPlaying(1, 1);
                if (step7 && step7_1 && step7_2)
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

                //step8 Verify the default fps in cine FPS box after buffering is complete.
                IList<int> FPSValue = viewer.GetFPSValueInList(1, 1);
                if (FPSValue[0] == DefaultFpsvalue)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }*/
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Logout Application
                viewer.CloseBluRingViewer();
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
        /// Cardio Cine - Keyboard Shortcuts
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_162308(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");

                //Step 1 - Launch and Login as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //Step 2 - Search for a study which has muliple series and each series has multiple images and load it universal viewer
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step 3 - Mouse over on bottom part of active viewport and click on "Play Image Series" button                                
                String[] values = { "Hide Stack Slider" };
                viewer.OpenShowHideDropdown();
                var dropdown = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ShowHideDropdown));
                var ishidestack = dropdown[3].GetAttribute("innerHTML").Trim().ToLower().Equals(values[0].ToLower());
                if (!ishidestack)
                {
                    viewer.SelectShowHideValue("Show Stack Slider", false);
                    Thread.Sleep(1000);
                    Logger.Instance.InfoLog("Show Stack Slider is clicked successfully");
                }
                var step3_1 = viewer.PlayCINE();
                var step3_2 = viewer.IsCINEPlaying();
                if (step3_1 && step3_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // step 4 - Press Down Arrow key on keyboard while cine is running.	                
                var activeViewpot = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                activeViewpot.SendKeys(Keys.ArrowDown);
                Thread.Sleep(2000);                
                var Sliderdownvalue1 = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_StackSlider).GetAttribute("innerHTML");
                if (!viewer.IsCINEPlaying())
                {
                    result.steps[++ExecutedSteps].status = "Partially Automated";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 5 - Press Down arrow key again
                bool step5 = false;
                activeViewpot = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                activeViewpot.SendKeys(Keys.ArrowDown);
                Thread.Sleep(2000);
                var Sliderdownvalue2 = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_StackSlider).GetAttribute("innerHTML");
                int value1 = Int32.Parse(Sliderdownvalue1);
                int value2 = Int32.Parse(Sliderdownvalue2);
                Logger.Instance.InfoLog("The value of value1 is :" + value1 + " and The value of value2 is :" + value2);
                var noOfImagesList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_imageFrameNumber));
                if (Sliderdownvalue1.Equals(noOfImagesList[0].GetAttribute("innerHTML")) && value2.Equals(value1))
                {                    
                    step5 = true;
                    Logger.Instance.InfoLog("Last Image is displayed and after pressing down button first image of the series is displayed");
                }
                else
                {
                    if (value2.Equals(value1 + 1))
                        step5 = true;
                }
                if (step5)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 6 - Click on "Play Image Series" button.
                var step6_1 = viewer.PlayCINE();
                var step6_2 = viewer.IsCINEPlaying();
                if (step6_1 && step6_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 7 - Press Up Arrow key                
                activeViewpot.SendKeys(Keys.ArrowUp);
                Thread.Sleep(2000);             
                Sliderdownvalue1 = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_StackSlider).GetAttribute("innerHTML");
                if (!viewer.IsCINEPlaying())
                {
                    result.steps[++ExecutedSteps].status = "Partially Automated";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 8 - Press Up arrow key again
                bool step8 = false;
                activeViewpot.SendKeys(Keys.ArrowDown);
                Thread.Sleep(2000);
                Sliderdownvalue2 = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_StackSlider).GetAttribute("innerHTML");
                value1 = Int32.Parse(Sliderdownvalue1);
                value2 = Int32.Parse(Sliderdownvalue2);
                Logger.Instance.InfoLog("The value of value1 is :" + value1 + " and The value of value2 is :" + value2);
                if (value1.Equals(value2))
                {                    
                        step8 = true;
                    Logger.Instance.InfoLog("First Image is displayed and after pressing Up button Last image of the series is displayed");
                }
                else
                {
                    if (value1 < value2)
                        step8 = true;
                }
                if (step8)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();


                // Step 9 - Click on "Play all in this Study" button
                viewer.ClickPlayAllOrPauseAll("PlayAll");
                viewer.WaitForThumbnailPercentageTo100(4);
                if (viewer.IsCINEPlaying() && viewer.IsCINEPlaying(2))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 10 - Select any viewport and press Down arrow key                
                activeViewpot = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                activeViewpot.SendKeys(Keys.ArrowDown);
                Thread.Sleep(2000);                
                if (!viewer.IsCINEPlaying())
                {
                    result.steps[++ExecutedSteps].status = "Partially Automated";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                // step 11 - Select any viewport and press Up arrow key	
                viewer.SetViewPort(1, 1);                                
                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.GetViewportCss(1, 1)).Click();
                activeViewpot = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                activeViewpot.SendKeys(Keys.ArrowUp);
                Thread.Sleep(2000);                
                if (!viewer.IsCINEPlaying(2, 1))
                {
                    result.steps[++ExecutedSteps].status = "Partially Automated";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else              
                    result.steps[++ExecutedSteps].StepFail();

                //Logout Application
                viewer.CloseBluRingViewer();
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

        ///  
        /// Cine play does update % viewed in thumbnails
        /// 
        public TestCaseResult Test_160883(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                BasePage.SetVMResolution("1980", "1080");
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                string User = BasePage.GetUniqueUserId("U160883");

                //step1   Load a multi-frame image study into the Enterprise viewer 
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login = new Login();
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(User, Config.adminGroupName, Config.adminRoleName);
                login.Logout();

                login.LoginIConnect(User, User);
                userpreference.OpenUserPreferences();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreference.CineDefaultFrameRate().Clear();
                userpreference.CineDefaultFrameRate().SendKeys("1");
                userpreference.CloseUserPreferences();
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(LastName: LastName, AccessionNo: Accession, Datasource: EA_91);
                studies.SelectStudy("Accession", Accession);
                BluRingViewer viewer = new BluRingViewer();
                viewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_globalTitleBar));
                if (step1)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step2 Verify that the multi-frame viewport has a corresponding thumbnail with % viewed initially set to 1/num_of_frames.
                var PercentViewedList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImageComponent + " " + BluRingViewer.div_thumbnailPercentImagesViewed));
                bool step2 = viewer.VerifyThumbnailPercentImagesViewed(PercentViewedList.ElementAt(1), 123, 1);
                if (step2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step3 - Use the mouse-wheel to scroll down to view the next frame in the series viewport.
                viewer.SetViewPort(1, 1);
                IWebElement ele = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                TestCompleteAction action = new TestCompleteAction();
                action.MouseScroll(ele, "down", "1");
                bool step3 = viewer.VerifyThumbnailPercentImagesViewed(PercentViewedList.ElementAt(1), 123, 2);
                if (step3)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step4 - Play cine to go through the 'n' frames in the viewport and click on 'Pause Series' button
                //viewer.VerifyCardioCINEToolbarOnMouseHover(2, 1);
                //viewer.SetFPSValue("2", 2, 1);
                viewer.OpenCineToolBar(2, 1);
                ClickElement(Driver.FindElement(By.CssSelector(BluRingViewer.div_CINE_PlayBtn)));
                Thread.Sleep(3000);
                ClickElement(Driver.FindElement(By.CssSelector(BluRingViewer.div_CINE_PauseBtn)));
                int numofimg = int.Parse(BasePage.Driver.FindElement(By.CssSelector("div.viewerContainer:nth-of-type(2) " + BluRingViewer.div_StackSlider)).GetAttribute("innerHTML"));
                if (viewer.GetFPSValue(2, 1).Equals("0 FPS") && numofimg > 2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step5 Ensure that user should view the multi-images and that the corresponding thumbnail has it's % viewed updated according to the number of frames viewed.
                Logger.Instance.InfoLog("Number of frames/Images played: " + numofimg);
                bool step5 = viewer.VerifyThumbnailPercentImagesViewed(PercentViewedList.ElementAt(1), 123, numofimg);
                if (step5)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step6 Use the mouse-wheel to scroll up to view next frame in the series viewport.              
                action.MouseScroll(ele, "down", "1").Perform();
                numofimg = int.Parse(BasePage.Driver.FindElement(By.CssSelector("div.viewerContainer:nth-of-type(2) " + BluRingViewer.div_StackSlider)).GetAttribute("innerHTML"));
                Logger.Instance.InfoLog("Number of frames/Images played: " + numofimg);
                bool step6 = viewer.VerifyThumbnailPercentImagesViewed(PercentViewedList.ElementAt(1), 123, numofimg);
                if (step6)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step7 Again,play cine to go through the 'n' frames in the viewport
                int percentViewedinUI = Int32.Parse(PercentViewedList.ElementAt(1).GetAttribute("innerHTML").Trim().Trim('%'));
                //viewer.VerifyCardioCINEToolbarOnMouseHover(2, 1);
                viewer.PlayCINE(2, 1);
                int percentViewedinUI_1 = Int32.Parse(PercentViewedList.ElementAt(1).GetAttribute("innerHTML").Trim().Trim('%'));
                if (percentViewedinUI < percentViewedinUI_1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step8 Click on "Pause Series" button in a viewport
                viewer.PauseCINE(2, 1);
                viewer.GetFPSValue();
                if (viewer.GetFPSValue(2, 1).Equals("0 FPS"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step9 Click Next Series button
                viewer.OpenCineToolBar(2, 1);
                ClickElement(Driver.FindElement(By.CssSelector(BluRingViewer.div_CINE_NextImageBtn)));
                /*var Thumbnail_Outer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ThumbnailOuter));
                Logger.Instance.InfoLog("Thumbnail 3 border color is: " + Thumbnail_Outer[2].GetCssValue("border-color") + " border color of thumbnail 2 is: " + Thumbnail_Outer[1].GetCssValue("border-color"));
                bool step9_1 = viewer.VerifyBordorColor(Thumbnail_Outer[2], "rgba(90, 170, 255, 1)");
                bool step9_2 = viewer.VerifyBordorColor(Thumbnail_Outer[1], "rgba(0, 0, 0, 1)");
                if (step9_1 && step9_2)*/
                if (viewer.IsCINEPlaying(2, 1) && BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 3))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step10 Click the 'Pause Series' button and verify the % viewed.
                //viewer.PauseCINE(2, 1);
                viewer.OpenCineToolBar(2, 1);
                ClickElement(Driver.FindElement(By.CssSelector(BluRingViewer.div_CINE_PauseBtn)));
                numofimg = int.Parse(BasePage.Driver.FindElement(By.CssSelector("div.viewerContainer:nth-of-type(2) " + BluRingViewer.div_StackSlider)).GetAttribute("innerHTML"));
                Logger.Instance.InfoLog("Number of frames/Images played: " + numofimg);
                bool step10 = viewer.VerifyThumbnailPercentImagesViewed(PercentViewedList.ElementAt(2), 64, numofimg);
                if (step10)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step11 Click Previous Series button
                viewer.ClickPreviousSeriesCINE(2, 1);
                Thread.Sleep(5000);
                /*bool step11_1 = viewer.VerifyBordorColor(Thumbnail_Outer[1], "rgba(90, 170, 255, 1)");
                bool step11_2 = viewer.VerifyBordorColor(Thumbnail_Outer[2], "rgba(255, 255, 255, 1)");
                if (step11_1 && step11_2)*/
                if (viewer.IsCINEPlaying(2, 1) && BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 2))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step12 Review the % viewed
                bool is100percent = false;
                String thumbnailpercentage = PercentViewedList.ElementAt(1).GetAttribute("innerHTML");
                for (int j = 0; !thumbnailpercentage.Equals("100%") && j < 300; j++)
                {
                    thumbnailpercentage = PercentViewedList.ElementAt(1).GetAttribute("innerHTML");
                    if (!thumbnailpercentage.Equals("100%"))
                    {
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        is100percent = true;
                        Logger.Instance.InfoLog("Thumbnail reaches 100% ");
                        break;
                    }
                }
                if (is100percent || thumbnailpercentage.Equals("100%"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step13 Play cine by clicking " "Play All in the Study"" to go through the 'n' frames in the viewport and click on 'Pause Series' button
                //step14 Ensure that user should view the multi-images and that the corresponding thumbnail has it's % viewed updated according to the number of frames viewed.
                viewer.CloseBluRingViewer();
                userpreference.OpenUserPreferences();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreference.CineDefaultFrameRate().Clear();
                userpreference.CineDefaultFrameRate().SendKeys("20");
                userpreference.CloseUserPreferences();
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(LastName: LastName, AccessionNo: Accession, Datasource: EA_91);
                studies.SelectStudy("Accession", Accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                //viewer.VerifyCardioCINEToolbarOnMouseHover();
                viewer.ClickPlayAllOrPauseAll("PlayAll");
                viewer.WaitForThumbnailPercentageTo100(3);
                bool step13_1 = viewer.IsCINEPlaying(1, 1) && viewer.IsCINEPlaying(2, 1) && viewer.IsCINEPlaying(3, 1);
                viewer.ClickPlayAllOrPauseAll("PauseAll", 1, 1);
                bool step13_2 = !(viewer.IsCINEPlaying(1, 1) && viewer.IsCINEPlaying(2, 1) && viewer.IsCINEPlaying(3, 1));
                Logger.Instance.InfoLog("step13_1: " + step13_1 + " step13_2: " + step13_2);
                if (step13_1 && step13_2)
                    result.steps[ExecutedSteps += 2].StepPass();
                else
                    result.steps[ExecutedSteps += 2].StepFail();

                //Logout Application
                viewer.CloseBluRingViewer();
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
            finally
            {
                BasePage.SetVMResolution("1280", "1024");
            }
        }

        /// <summary>
        /// Measurement tools should be in disable state when Cine is playing
        /// </summary>
        public TestCaseResult Test_161999(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            bool isDefaultTool = false;
            UserPreferences userpref = new UserPreferences();
            var isImageSplit = " ";
            var isSeriesSplit = " ";
            var isAutoSplit = " ";

            try
            {
                BasePage.SetVMResolution("1980", "1080");
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");

                // Precondition
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login = new Login();
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                DomainManagement domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain("SuperAdminGroup");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                var ConfiguredTools = login.GetConfiguredToolsInToolBoxConfig();
                if (!(ConfiguredTools.Contains("Series Scope") && !(ConfiguredTools.Contains("Image Scope")) && !(ConfiguredTools.Contains("Save Series")) && !(ConfiguredTools.Contains("Save Annotated Image"))))
                {
                    isDefaultTool = true;
                    IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(9)"));
                    var dictionary = new Dictionary<String, IWebElement>();
                    dictionary.Add("Series Scope", group1);
                    dictionary.Add("Image Scope", group1);
                    dictionary.Add("Save Series", group1);
                    dictionary.Add("Save Annotated Image", group1);
                    domainmanagement.AddToolsToToolbox(dictionary);
                    domainmanagement.ClickElement(domainmanagement.SaveButton());
                    Logger.Instance.InfoLog("Series Scope, Image Scope, Save Series and Save Annotated Image are configured in the ToolBox");
                }
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.ModalityDropDown().SelectByText("CR");
                isImageSplit = userpref.ThumbnailSplittingImageRadioBtn().GetAttribute("checked");
                isSeriesSplit = userpref.ThumbnailSplittingSeriesRadioBtn().GetAttribute("checked");
                isAutoSplit = userpref.ThumbnailSplittingAutoRadioBtn().GetAttribute("checked");
                userpref.ClickElement(userpref.ThumbnailSplittingSeriesRadioBtn());
                PageLoadWait.WaitForFrameLoad(10);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                login.Logout();

                //Step 1 - Launch and Login as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //Step 2 - Navigate to Studies tab and search for a study which has multiple series.
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession, Datasource: login.GetHostName(Config.EA1));
                ExecutedSteps++;

                //Step 3 - Select the study from search result and click on View exam button                                      
                studies.SelectStudy("Patient ID", PatientID);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                // Step 4 - Click on Play Image Series button on first viewport
                var step4_1 = viewer.PlayCINE();
                var step4_2 = viewer.IsCINEPlaying();
                if (step4_1 && step4_2)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                // Step 5 - Right click on viewport and observe the floating toolbox.
                viewer.OpenViewerToolsPOPUp();
                IList<IWebElement> availableTools = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_viewportToolbox + " " + BluRingViewer.div_toolWrapper));
                bool isMeasurementToolsEnabled = false;
                bool isNonMeasurementToolsDisabled = false;
				String[] MeasurementTools = { "Line Measurement", "Calibration Tool", "Angle Measurement", "Cobb Angle", "Magnifier x2",
					"Add Text", "Save Series", "Save Annotated Image", "Get Pixel Value", "Draw Ellipse", "Draw Rectangle", "Draw ROI", "Free draw",
				"Remove all annotations", "Transischial Measurement", "Horizontal Plumbline", "Vertical PlumbLine", "Joint Line Measurement"};
                String[] NonMeasurementTools = { "Window Level", "Flip Vertical", "Flip Horizontal", "Rotate Clockwise", "Rotate Counterclockwise",
                    "Pan", "Invert", "Zoom", "Reset", "AutoWL", "Series Scope", "Image Scope" };
                foreach (IWebElement ele in availableTools)
                {                    
                    string tooltip = ele.GetAttribute("title");
                    var index = Array.FindIndex(MeasurementTools, x => x == tooltip);
                    var className = ele.GetAttribute("class");
                    if (index > -1 && (!className.Contains("tool-disabled")))
                    {
                        isMeasurementToolsEnabled = true;
                        Logger.Instance.ErrorLog("Measurement Tool " + tooltip + " is enabled, but it should be in disabled state");
                    }
                }
                foreach (IWebElement ele in availableTools)
                {                    
                    string tooltip = ele.GetAttribute("title");
                    var index = Array.FindIndex(NonMeasurementTools, x => x == tooltip);
                    var className = ele.GetAttribute("class");
                    if (index > -1 && (className.Contains("tool-disabled")))
                    {
                        isNonMeasurementToolsDisabled = true;
                        Logger.Instance.ErrorLog("Measurement Tool " + tooltip + " is disabled, but it should be in enabled state");
                    }
                }
                if (!isMeasurementToolsEnabled  && !isNonMeasurementToolsDisabled)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                // Step 6 - Click on Pause Image Series button
                viewer.SelectViewerTool(BluRingTools.Pan, isOpenToolsPOPup: false);               

                //viewer.VerifyCardioCINEToolbarOnMouseHover();
                var step6_1 = viewer.PauseCINE();
                var step6_2 = viewer.IsCINEPlaying();
                if (step6_1 && !step6_2)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                // Step 7 - Right click on viewport and observe the floating toolbox.
                viewer.OpenViewerToolsPOPUp();
                Thread.Sleep(3000);
                availableTools = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_viewportToolbox + " " + BluRingViewer.div_toolWrapper));
                foreach (IWebElement ele in availableTools)
                {                    
                    string tooltip = ele.GetAttribute("title");
                    var className = ele.GetAttribute("class");
                    if (className.Contains("tool-disabled"))
                    {
                        isMeasurementToolsEnabled = true;
                        Logger.Instance.ErrorLog("Measurement Tool " + tooltip + " is disabled, but it should be in enable state");
                    }
                }
                if (!isMeasurementToolsEnabled)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                // Step 8 - Click Play All in this study button in a viewport
                viewer.SelectViewerTool(BluRingTools.Pan, isOpenToolsPOPup: false);              
                viewer.ClickPlayAllOrPauseAll("PlayAll");
                viewer.WaitForThumbnailPercentageTo100(2);              
                if (viewer.IsCINEPlaying() && viewer.IsCINEPlaying(2))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                // Step 9 - Right click on viewport and observe the floating toolbox.
                viewer.OpenViewerToolsPOPUp();
                Thread.Sleep(3000);
                availableTools = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_viewportToolbox + " " + BluRingViewer.div_toolWrapper));
                isMeasurementToolsEnabled = false;
                isNonMeasurementToolsDisabled = false;                
                foreach (IWebElement ele in availableTools)
                {                    
                    string tooltip = ele.GetAttribute("title");
                    var index = Array.FindIndex(MeasurementTools, x => x == tooltip);
                    var className = ele.GetAttribute("class");
                    if (index > -1 && (!className.Contains("tool-disabled")))
                    {
                        isMeasurementToolsEnabled = true;
                        Logger.Instance.ErrorLog("Measurement Tool " + tooltip + " is enabled, but it should be in disabled state");
                    }
                }
                foreach (IWebElement ele in availableTools)
                {                    
                    string tooltip = ele.GetAttribute("title");
                    var index = Array.FindIndex(NonMeasurementTools, x => x == tooltip);
                    var className = ele.GetAttribute("class");
                    if (index > -1 && (className.Contains("tool-disabled")))
                    {
                        isNonMeasurementToolsDisabled = true;
                        Logger.Instance.ErrorLog("Measurement Tool " + tooltip + " is disabled, but it should be in enabled state");
                    }
                }
                if (!isMeasurementToolsEnabled && !isNonMeasurementToolsDisabled)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                // Step 10 - Click Pause All in this study button in a viewport
                viewer.SelectViewerTool(BluRingTools.Pan, isOpenToolsPOPup: false);              
                viewer.ClickPlayAllOrPauseAll("PauseAll");
                Thread.Sleep(2000);
                if (!viewer.IsCINEPlaying() && !viewer.IsCINEPlaying(2))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                // Step 11 - Right click on viewport and observe the floating toolbox.	
                viewer.OpenViewerToolsPOPUp();
                availableTools = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_viewportToolbox + " " + BluRingViewer.div_toolWrapper));
                foreach (IWebElement ele in availableTools)
                {                    
                    string tooltip = ele.GetAttribute("title");
                    var className = ele.GetAttribute("class");
                    if (className.Contains("tool-disabled"))
                    {
                        isMeasurementToolsEnabled = true;
                        Logger.Instance.ErrorLog("Measurement Tool " + tooltip + " is disabled, but it should be in enable state");
                    }
                }
                if (!isMeasurementToolsEnabled)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Logout Application
                viewer.CloseBluRingViewer();
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
            finally
            {
                if (isDefaultTool)
                {
                    login.LoginIConnect("Administrator", "Administrator");
                    DomainManagement domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                    domainmanagement.SearchDomain("SuperAdminGroup");
                    domainmanagement.SelectDomain("SuperAdminGroup");
                    domainmanagement.ClickEditDomain();
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    SelectElement Modality = new SelectElement(BasePage.Driver.FindElement
                                              (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                    Modality.SelectByText("default");
                    Thread.Sleep(1000);
                    IWebElement revertButton = BasePage.Driver.FindElement(By.CssSelector(DomainManagement.btn_revertToDefault));
                    if (revertButton.Enabled)
                        revertButton.Click();
                    domainmanagement.ClickSaveEditDomain();
                    userpref.OpenUserPreferences();
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                    PageLoadWait.WaitForPageLoad(20);
                    userpref.ModalityDropDown().SelectByText("CR");
                    if (!(isSeriesSplit == null))
                    {
                        userpref.ClickElement(userpref.ThumbnailSplittingSeriesRadioBtn());
                        Logger.Instance.InfoLog("The Thumbnails splitting is reverted back to series");
                    }
                    else if (!(isAutoSplit == null))
                    {
                        userpref.ClickElement(userpref.ThumbnailSplittingAutoRadioBtn());
                        Logger.Instance.InfoLog("The Thumbnails splitting is reverted back to Auto");
                    }
                    else if (!(isImageSplit == null))
                    {
                        userpref.ClickElement(userpref.ThumbnailSplittingImageRadioBtn());
                        Logger.Instance.InfoLog("The Thumbnails splitting is reverted back to Image");
                    }
                    PageLoadWait.WaitForFrameLoad(10);
                    userpref.CloseUserPreferences();
                    PageLoadWait.WaitForPageLoad(20);
                    login.Logout();
                }
                BasePage.SetVMResolution("1280", "1024");
            }
        }

        /// <summary> 
        /// Test 160881 - Prev/next frame using "Play NextImage/PreviousImage Series" button
        /// </summary>
        public TestCaseResult Test_160881(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variable
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            BasePage basePage = new BasePage();
            UserPreferences userpreferences = new UserPreferences();
            UserManagement userManagement = null;
            Studies studies = null;
            BluRingViewer viewer = null;
            string Accession = null;
            string User = BasePage.GetUniqueUserId();
            Stopwatch stopwatch = new Stopwatch();
            TimeSpan timeout = new TimeSpan(0, 2, 0);
            bool[] allplaypausecine = null;
            try
            {
                BasePage.SetVMResolution("1980", "1080");
                Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");

                //PreCondition
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login = new Login();
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                userManagement = (UserManagement)login.Navigate("UserManagement");
                userManagement.CreateUser(userId: User, domainName: Config.adminGroupName, roleName: Config.adminRoleName);
                login.Logout();

                //Step 1:
                login.LoginIConnect(User, User);
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 2 and 3:
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.ModalityDropDown().SelectByText("US");
                userpreferences.LayoutDropDown().SelectByText("1x2");
                userpreferences.ThumbnailSplittingImageRadioBtn().Click();
                userpreference.CloseUserPreferences();
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession, Datasource: login.GetHostName(Config.EA91));
                PageLoadWait.WaitForLoadingMessage();
                PageLoadWait.WaitForSearchLoad();
                studies.SelectStudy("Accession", Accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                if(viewer.studyPanel().Displayed && viewer.GetViewPortCount(1) == 2)
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
                }

                //Step 4:
                viewer.ClickPlayAllOrPauseAll("PlayAll", 1, 1);
                allplaypausecine = Enumerable.Repeat(false, 2).ToArray();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    for (int i = 0; i < 2; i++)
                    {
                        allplaypausecine[i] = viewer.IsCINEPlaying(i + 1, 1);
                    }
                    if (allplaypausecine.All(apc => apc == true))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (allplaypausecine.All(apc => apc == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 4: Cine Play All Status is [" + string.Join(",", allplaypausecine) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 5:
                //viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 1)).Click();
                viewer.ClickNextSeriesCINE(1, 1);
                //viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 1)).Click();
                BluRingViewer.WaitforViewports();
                if (viewer.IsCINEPlaying(1, 1) && BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 2))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 6 and 7:
                //viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 1)).Click();
                viewer.ClickPreviousSeriesCINE(1, 1);
                //viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 1)).Click();
                BluRingViewer.WaitforViewports();
                if (viewer.IsCINEPlaying(1, 1) && BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 1))
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

                //Step 8:
                //viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 1)).Click();
                viewer.ClickNextSeriesCINE(1, 1);
                //viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 1)).Click();
                BluRingViewer.WaitforViewports();
                if (viewer.IsCINEPlaying(1, 1) && BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 2))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 9:
                viewer.ClickPlayAllOrPauseAll("PauseAll", 1, 1);
                allplaypausecine = Enumerable.Repeat(false, 2).ToArray();
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    for (int i = 0; i < 2; i++)
                    {
                        allplaypausecine[i] = !viewer.IsCINEPlaying(i + 1, 1);
                    }
                    if (allplaypausecine.All(apc => apc == true))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (allplaypausecine.All(apc => apc == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 9: Cine Pause All Status is [" + string.Join(",", allplaypausecine) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 10:
                PageLoadWait.WaitForFrameLoad(30);
                viewer.PlayCINE(1, 1);
                BluRingViewer.WaitforViewports();
                allplaypausecine = Enumerable.Repeat(false, 2).ToArray();
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    allplaypausecine[0] = viewer.IsCINEPlaying(1, 1);
                    allplaypausecine[1] = !viewer.IsCINEPlaying(2, 1);
                    if (allplaypausecine.All(apc => apc == true))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (allplaypausecine.All(apc => apc == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 10: Cine Status is [" + string.Join(",", allplaypausecine) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 11:
                //viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 1)).Click();
                viewer.ClickPreviousSeriesCINE(1, 1);
                //viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 1)).Click();
                BluRingViewer.WaitforViewports();
                Thread.Sleep(10000);
                if (viewer.IsCINEPlaying(1, 1) && BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Logout
                login.Logout();
                //Report Result
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Logout
                login.Logout();
                //Return Result
                return result;
            }
            finally
            {
                BasePage.SetVMResolution("1280", "1024");
            }
        }

        /// <summary> 
        /// Test 164729 - Cardio FPS Slider : Study with more than two priors
        /// </summary>
        public TestCaseResult Test_164729(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variable
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            BasePage basePage = new BasePage();
            UserPreferences userpreferences = new UserPreferences();
            UserManagement userManagement = null;
            Studies studies = null;
            BluRingViewer viewer = null;
            string[] Accession = null;
            string User = BasePage.GetUniqueUserId();
            Stopwatch stopwatch = new Stopwatch();
            TimeSpan timeout = new TimeSpan(0, 2, 0);
            bool[] allplaypausecine = null;
            bool[] ViewPortFPS = null;
            int StudyPanel = 1;
            int[] fpsval = Enumerable.Repeat(25, 4).ToArray();
            IList<int> FPSValue = null;
            try
            {
                BasePage.SetVMResolution("1980", "1080");
                Accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList")).Split('=');

                //PreCondition
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login = new Login();
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                userManagement = (UserManagement)login.Navigate("UserManagement");
                userManagement.CreateUser(userId: User, domainName: Config.adminGroupName, roleName: Config.adminRoleName);
                login.Logout();

                //Step 1:
                login.LoginIConnect(User, User);
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 2:
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                basePage.SendKeys(userpreferences.CineDefaultFrameRate(), "25");
                userpreference.CloseUserPreferences();
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 3:
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                PageLoadWait.WaitForLoadingMessage();
                PageLoadWait.WaitForSearchLoad();
                studies.SelectStudy("Accession", Accession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                if (viewer.studyPanel().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                }

                //Step 4:
                if (viewer.PlayCINE(1, StudyPanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 5:
                viewer.OpenPriors(accession: Accession[1]);
                StudyPanel = 2;
                if (viewer.PlayCINE(1, StudyPanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 6:
                StudyPanel = 3;
                viewer.OpenPriors(accession: Accession[2]);
                if (viewer.studyPanel(StudyPanel).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 7:
                if (viewer.PlayCINE(1, StudyPanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 8:
                FPSValue = viewer.GetFPSValueInList(1, StudyPanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[0])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 8: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 9:
                fpsval[0] = viewer.SetFPSValue("45", 1, StudyPanel);
                if (fpsval[0]>=45)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 9: The Captured FPS Value is "+ fpsval[0]);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 10:
                FPSValue = viewer.GetFPSValueInList(1, StudyPanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[0])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 10: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 11:
                if (viewer.PauseCINE(1, StudyPanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 12:
                viewer.ClickPlayAllOrPauseAll("PlayAll", 1, StudyPanel);
                allplaypausecine = Enumerable.Repeat(false, 4).ToArray();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        allplaypausecine[i] = viewer.IsCINEPlaying(i + 1, StudyPanel);
                    }
                    if (allplaypausecine.All(apc => apc == true))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (allplaypausecine.All(apc => apc == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 12: Step Play All Status is [" + string.Join(",", allplaypausecine) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 13:
                FPSValue = viewer.GetFPSValueInList(1, StudyPanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[0])) && viewer.IsCINEPlaying(1, StudyPanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 13: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 14, 15:
                ViewPortFPS = Enumerable.Repeat(false, 3).ToArray();
                for (int i = 2; i <= 4; i++)
                {
                    FPSValue = viewer.GetFPSValueInList(i, StudyPanel);
                    if (FPSValue.All(v => (v >= 1) && (v <= fpsval[i - 1])))
                    {
                        ViewPortFPS[i - 2] = true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Step 14: The captured FPS Value on viewport " + i + " is [" + string.Join(",", FPSValue) + "]");
                    }
                }
                if (ViewPortFPS.All(vfp => vfp == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 16:
                viewer.ClickPlayAllOrPauseAll("PauseAll", 1, StudyPanel);
                allplaypausecine = Enumerable.Repeat(false, 4).ToArray();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        allplaypausecine[i] = !viewer.IsCINEPlaying(i + 1, StudyPanel);
                    }
                    if (allplaypausecine.All(apc => apc == true))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (allplaypausecine.All(apc => apc == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 16: Step Play All Status is [" + string.Join(",", allplaypausecine) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 17:
                login.Logout();
                login.LoginIConnect(User, User);
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                basePage.SendKeys(userpreferences.CineDefaultFrameRate(), "30");
                userpreferences.BluringViewerRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                fpsval = Enumerable.Repeat(30, 4).ToArray();
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 18:
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_91);
                PageLoadWait.WaitForSearchLoad();
                studies.SelectStudy("Accession", Accession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.OpenPriors(accession: Accession[1]);
                viewer.OpenPriors(accession: Accession[2]);
                viewer.OpenPriors(accession: Accession[3]);
                StudyPanel = 4;
                if (viewer.studyPanel(StudyPanel).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 19:
                if (viewer.PlayCINE(1, StudyPanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 20:
                FPSValue = viewer.GetFPSValueInList(1, StudyPanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[0])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 20: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 21:
                fpsval[0] = viewer.SetFPSValue("60", 1, StudyPanel);
                if (fpsval[0] >= 60)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 21: The Captured FPS Value is " + fpsval[0]);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 22:
                FPSValue = viewer.GetFPSValueInList(1, StudyPanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[0])) && viewer.IsCINEPlaying(1, StudyPanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 22: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 23:
                if (viewer.PauseCINE(1, StudyPanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 24:
                viewer.ClickPlayAllOrPauseAll("PlayAll", 1, StudyPanel);
                allplaypausecine = Enumerable.Repeat(false, 4).ToArray();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        allplaypausecine[i] = viewer.IsCINEPlaying(i + 1, StudyPanel);
                    }
                    if (allplaypausecine.All(apc => apc == true))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (allplaypausecine.All(apc => apc == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 24: Step Play All Status is [" + string.Join(",", allplaypausecine) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 25:
                FPSValue = viewer.GetFPSValueInList(1, StudyPanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[0])) && viewer.IsCINEPlaying(1, StudyPanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 25: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 26, 27:
                ViewPortFPS = Enumerable.Repeat(false, 3).ToArray();
                for (int i = 2; i <= 4; i++)
                {
                    FPSValue = viewer.GetFPSValueInList(i, StudyPanel);
                    if (FPSValue.All(v => (v >= 1) && (v <= fpsval[i - 1])))
                    {
                        ViewPortFPS[i - 2] = true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Step 26: The captured FPS Value on viewport " + i + " is [" + string.Join(",", FPSValue) + "]");
                    }
                }
                if (ViewPortFPS.All(vfp => vfp == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 28:
                viewer.ClickPlayAllOrPauseAll("PauseAll", 1, StudyPanel);
                allplaypausecine = Enumerable.Repeat(false, 4).ToArray();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        allplaypausecine[i] = !viewer.IsCINEPlaying(i + 1, StudyPanel);
                    }
                    if (allplaypausecine.All(apc => apc == true))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (allplaypausecine.All(apc => apc == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 28: Step Play All Status is [" + string.Join(",", allplaypausecine) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 29:
                login.Logout();
                login.LoginIConnect(User, User);
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                basePage.SendKeys(userpreferences.CineDefaultFrameRate(), "15");
                userpreferences.BluringViewerRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                fpsval = Enumerable.Repeat(15, 4).ToArray();
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 30:
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_91);
                PageLoadWait.WaitForSearchLoad();
                studies.SelectStudy("Accession", Accession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                StudyPanel = 1;
                if (viewer.studyPanel(StudyPanel).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 31:
                viewer.OpenPriors(accession: Accession[1]);
                viewer.OpenPriors(accession: Accession[2]);
                viewer.OpenPriors(accession: Accession[3]);
                viewer.OpenPriors(accession: Accession[4]);
                StudyPanel = 5;
                if (viewer.studyPanel(StudyPanel).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 32:
                if (viewer.PlayCINE(1, StudyPanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 33,34s:
                FPSValue = viewer.GetFPSValueInList(1, StudyPanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[0])) && viewer.IsCINEPlaying(1, StudyPanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 33: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 35:
                fpsval[0] = viewer.SetFPSValue("41", 1, StudyPanel);
                if (fpsval[0] >= 41)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 35: The Captured FPS Value is " + fpsval[0]);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 36:
                FPSValue = viewer.GetFPSValueInList(1, StudyPanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[0])) && viewer.IsCINEPlaying(1, StudyPanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 36: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 37:
                if (viewer.PauseCINE(1, StudyPanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 38:
                viewer.ClickPlayAllOrPauseAll("PlayAll", 1, StudyPanel);
                allplaypausecine = Enumerable.Repeat(false, 4).ToArray();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        allplaypausecine[i] = viewer.IsCINEPlaying(i + 1, StudyPanel);
                    }
                    if (allplaypausecine.All(apc => apc == true))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (allplaypausecine.All(apc => apc == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 38: Step Play All Status is [" + string.Join(",", allplaypausecine) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 39:
                FPSValue = viewer.GetFPSValueInList(1, StudyPanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[0])) && viewer.IsCINEPlaying(1, StudyPanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 39: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 40 to 42:
                ViewPortFPS = Enumerable.Repeat(false, 4).ToArray();
                for (int i = 1; i <= 4; i++)
                {
                    FPSValue = viewer.GetFPSValueInList(i, StudyPanel);
                    if (FPSValue.All(v => (v >= 1) && (v <= fpsval[i - 1])))
                    {
                        ViewPortFPS[i - 1] = true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Step 40: The captured FPS Value on viewport " + i + " is [" + string.Join(",", FPSValue) + "]");
                    }
                }
                if (ViewPortFPS.All(vfp => vfp == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 43:
                viewer.ClickPlayAllOrPauseAll("PauseAll", 1, StudyPanel);
                allplaypausecine = Enumerable.Repeat(false, 4).ToArray();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        allplaypausecine[i] = !viewer.IsCINEPlaying(i + 1, StudyPanel);
                    }
                    if (allplaypausecine.All(apc => apc == true))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (allplaypausecine.All(apc => apc == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 43: Step Play All Status is [" + string.Join(",", allplaypausecine) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 44:
                if (viewer.PlayCINE(2, StudyPanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 45:
                FPSValue = viewer.GetFPSValueInList(2, StudyPanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[1])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 45: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 46:
                fpsval[1] = viewer.SetFPSValue("45", 2, StudyPanel);
                if (fpsval[1] >= 45)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 46: The Captured FPS Value is " + fpsval[1]);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 47:
                FPSValue = viewer.GetFPSValueInList(2, StudyPanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[1])) && viewer.IsCINEPlaying(2, StudyPanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 47: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 48:
                if (viewer.PauseCINE(2, StudyPanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 49:
                viewer.ClickPlayAllOrPauseAll("PlayAll", 2, StudyPanel);
                allplaypausecine = Enumerable.Repeat(false, 4).ToArray();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        allplaypausecine[i] = viewer.IsCINEPlaying(i + 1, StudyPanel);
                    }
                    if (allplaypausecine.All(apc => apc == true))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (allplaypausecine.All(apc => apc == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 49: Step Play All Status is [" + string.Join(",", allplaypausecine) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 50:
                FPSValue = viewer.GetFPSValueInList(2, StudyPanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[1])) && viewer.IsCINEPlaying(2, StudyPanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 36: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 51, 52:
                FPSValue = viewer.GetFPSValueInList(1, StudyPanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[0])) && viewer.IsCINEPlaying(1, StudyPanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 51: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 53, 54:
                ViewPortFPS = Enumerable.Repeat(false, 2).ToArray();
                for (int i = 3; i <= 4; i++)
                {
                    FPSValue = viewer.GetFPSValueInList(i, StudyPanel);
                    if (FPSValue.All(v => (v >= 1) && (v <= fpsval[i - 1])))
                    {
                        ViewPortFPS[i - 3] = true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Step 53: The captured FPS Value on viewport " + i + " is [" + string.Join(",", FPSValue) + "]");
                    }
                }
                if (ViewPortFPS.All(vfp => vfp == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 55:
                viewer.ClickPlayAllOrPauseAll("PauseAll", 1, StudyPanel);
                allplaypausecine = Enumerable.Repeat(false, 4).ToArray();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        allplaypausecine[i] = !viewer.IsCINEPlaying(i + 1, StudyPanel);
                    }
                    if (allplaypausecine.All(apc => apc == true))
                    {
                        break;
                    } 
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (allplaypausecine.All(apc => apc == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 55: Step Play All Status is [" + string.Join(",", allplaypausecine) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 56:
                if (viewer.PlayCINE(3, StudyPanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 57:
                FPSValue = viewer.GetFPSValueInList(3, StudyPanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[2])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 57: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 58:
                fpsval[2] = viewer.SetFPSValue("35", 3, StudyPanel);
                if (fpsval[2] >= 35)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 35: The Captured FPS Value is " + fpsval[2]);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 59:
                FPSValue = viewer.GetFPSValueInList(3, StudyPanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[2])) && viewer.IsCINEPlaying(3, StudyPanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 59: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 60:
                if (viewer.PauseCINE(3, StudyPanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 61:
                viewer.ClickPlayAllOrPauseAll("PlayAll", 3, StudyPanel);
                allplaypausecine = Enumerable.Repeat(false, 4).ToArray();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        allplaypausecine[i] = viewer.IsCINEPlaying(i + 1, StudyPanel);
                    }
                    if (allplaypausecine.All(apc => apc == true))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (allplaypausecine.All(apc => apc == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 61: Step Play All Status is [" + string.Join(",", allplaypausecine) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 62:
                FPSValue = viewer.GetFPSValueInList(3, StudyPanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[2])) && viewer.IsCINEPlaying(3, StudyPanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 62: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 63, 64:
                FPSValue = viewer.GetFPSValueInList(1, StudyPanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[0])) && viewer.IsCINEPlaying(1, StudyPanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 63: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 65, 66:
                FPSValue = viewer.GetFPSValueInList(2, StudyPanel);
                if (FPSValue.All(v => (v >= 1) && (v <= fpsval[1])) && viewer.IsCINEPlaying(2, StudyPanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 65: The Captured FPS Value is [" + string.Join(",", FPSValue) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 67:
                viewer.ClickPlayAllOrPauseAll("PauseAll", 3, StudyPanel);
                allplaypausecine = Enumerable.Repeat(false, 4).ToArray();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        allplaypausecine[i] = !viewer.IsCINEPlaying(i + 1, StudyPanel);
                    }
                    if (allplaypausecine.All(apc => apc == true))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (allplaypausecine.All(apc => apc == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step 67: Step Play All Status is [" + string.Join(",", allplaypausecine) + "]");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Logout
                login.Logout();
                //Report Result
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Logout
                login.Logout();
                //Return Result
                return result;
            }
            finally
            {
                BasePage.SetVMResolution("1280", "1024");
            }
        }


    }

}
