using System;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Reusable.Generic;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.iConnect;
using System.Threading;

namespace Selenium.Scripts.Tests
{
    class Dicom6000Overlay
    {
        public Login login { get; set; }
        public Configure configure { get; set; }
        public HPHomePage hphomepage { get; set; }
        public string filepath { get; set; }

        public Dicom6000Overlay(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            configure = new Configure();
            hphomepage = new HPHomePage();            
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }

        /// <summary>
        /// Show/Hide DICOM 6000 Overlay
        /// </summary>
        public TestCaseResult Test_161000(String testid, String teststeps, int stepcount)
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

                //Step 1 - Login to application
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                // Step 2 -  Search and Select a study with DICOM 6000 overlay and load in BluRing Viewer
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                // Step 3 - Click on the SHOW/HIDE tool in the global toolbar.
                String[] options = { "Hide Image Text", "Hide Dicom 6000 Overlay", "Hide Thumbnails", "Hide Stack Slider" };
                bool step3 = viewer.Verify_ShowHideDropdown_Values(options);
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

                // Step 4 - Select the HIDE DICOM 6000 OVERLAY from the SHOW/HIDE drop-down list.                
                bool step4 = viewer.SelectShowHideValue("Hide Dicom 6000 Overlay", false);
                bool step4_1 = viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_ShowHideDropdown));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step4_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step4 && (!step4_1) && step4_2)
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

                // Step 5 - Click on the SHOW/HIDE tool in the global toolbar.               
                String[] values = { "Hide Image Text", "Show Dicom 6000 Overlay", "Hide Thumbnails", "Hide Stack Slider" };
                bool step5 = viewer.Verify_ShowHideDropdown_Values(values);
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

                // Step 6 - Select the SHOW DICOM 6000 OVERLAY from the SHOW/HIDE drop-down list.
                bool step6 = viewer.SelectShowHideValue("Show Dicom 6000 Overlay", false);
                bool step6_1 = viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_ShowHideDropdown));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step6_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step6 && (!step6_1) && step6_2)
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

                // Step 7 - open study in another study panel
                viewer.OpenPriors(2);
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_studypanel + ":nth-of-type(2)")))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step 8 - Click on the SHOW/HIDE tool in the global toolbar and select the HIDE DICOM 6000 OVERLAY
                bool step8 = viewer.SelectShowHideValue("Hide Dicom 6000 Overlay");
                bool step8_0 = viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_ShowHideDropdown));
                ExecutedSteps++;
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step8_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step8_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + ":nth-of-type(2) " + BluRingViewer.div_compositeViewer), totalImageCount: 2, IsFinal: 1);
                if (step8 && (!step8_0) && step8_1 && step8_2)
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

                // Step 9 
                // Close BluRing Viewer
                viewer.CloseBluRingViewer();

                // Search and Select a study with non DICOM 600 overlay
                studies.SearchStudy(AccessionNo: Accession[1], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[1]);
                BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++; 

                // Step 10 - Click on the SHOW/HIDE tool
                var step10 = viewer.SelectShowHideValue("Hide Dicom 6000 Overlay");
                var step10_1 = viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_ShowHideDropdown));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step10_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step10 && (!step10_1) && step10_2)
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

                //Logout 
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
        /// Unable to view DICOM 6000 overlay on RGB images
        /// </summary>
        public TestCaseResult Test_161002(String testid, String teststeps, int stepcount)
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

                // Step 1 - Data source configuration covered in build installation
                // Step 2 - Login to application
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps += 2;

                // Step 3 - Search for the study and launch Bluring Viewer
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForThumbnailsToLoad(20);
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_ThumbnailNextArrowButton));
                Thread.Sleep(2000);
                var thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));
                new Actions(BasePage.Driver).DoubleClick(thumbnails[13]).Build().Perform();
                Thread.Sleep(5000);
                ExecutedSteps++;

                // Step 4 - open other study in another study panel 
                viewer.OpenPriors(1);
                PageLoadWait.WaitForFrameLoad(20);

                //  Study with DICOM 6000 overlay, Study without overlay are launched in their respective Study Panels.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step4_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + ":nth-of-type(2) " + BluRingViewer.div_compositeViewer));
                if (step4 && step4_1)
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

                // Step 5 -  Select 'Hide Dicom 6000 Overlay'
                bool step5 = viewer.SelectShowHideValue("HIDE DICOM 6000 OVERLAY");
                bool step5_0 = viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_ShowHideDropdown));                
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step5_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step5_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + ":nth-of-type(2) " + BluRingViewer.div_compositeViewer));
                if (step5 && (!step5_0) && step5_1 && step5_2)
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

                // Step 6 - Close the Bluring Viewer
                viewer.CloseBluRingViewer();

                // Search the same study with MPACS data source
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.SanityPACS));
                studies.SelectStudy("Accession", Accession[0]);
                BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForThumbnailsToLoad(20);
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_ThumbnailNextArrowButton));
                Thread.Sleep(2000);
                thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));
                new Actions(BasePage.Driver).DoubleClick(thumbnails[13]).Build().Perform();
                Thread.Sleep(5000);
                ExecutedSteps++;

                // Step 7 - open other study in another study panel
                viewer.OpenPriors(1);
                PageLoadWait.WaitForFrameLoad(20);

                //  Study with DICOM 6000 overlay, Study without overlay are launched in their respective Study Panels.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step7_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + ":nth-of-type(2) " + BluRingViewer.div_compositeViewer));
                if (step7 && step7_1)
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

                // Step 8 - Select 'HIDE DICOM 6000 OVERLAY' from the drop-down list
                bool step8 = viewer.SelectShowHideValue("HIDE DICOM 6000 OVERLAY");
                bool step8_0 = viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_ShowHideDropdown));
                ExecutedSteps++;
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step8_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step8_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + ":nth-of-type(2) " + BluRingViewer.div_compositeViewer));
                if (step8 && (!step8_0) && step8_1 && step8_2)
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

                //Logout 
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
        /// Torture Test: Viewer crashes loading large study with Dicom 6000 Overlay
        /// </summary>
        public TestCaseResult Test_161001(String testid, String teststeps, int stepcount)
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
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');


                // Step 1 - Data source configuration covered in build installation
                // Step 2 - Login to application as domain user
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;
                ExecutedSteps++;

                //Step 3 - Navigate and search the study in EA data source 
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step_3)
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

                //Step 4 - Select HIDE DICOM 6000 OVERLAY from SHOW/HIDE drop-down list.
                var step4 = viewer.SelectShowHideValue("hide dicom 6000 overlay");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step4 && step_4)
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

                //Step 5 - Select SHOW DICOM 6000 OVERLAY from SHOW/HIDE drop-down list.

                var step5 = viewer.SelectShowHideValue("SHOW DICOM 6000 OVERLAY");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_5 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step5 && step_5)
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

                // Step 6 - Close the viewer and search the study in MPACS data source
                viewer.CloseBluRingViewer();
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.SanityPACS));
                studies.SelectStudy("Accession", Accession[0]);
                BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step_6)
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

                //Step 7 - Select 'HIDE DICOM 6000 OVERLAY' from SHOW/HIDE drop-down list.
                var step7 = viewer.SelectShowHideValue("hide dicom 6000 overlay");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step7 && step_7)
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

                //Step 8 - Select 'SHOW DICOM 6000 OVERLAY' from SHOW/HIDE drop-down list.

                var step8 = viewer.SelectShowHideValue("SHOW DICOM 6000 OVERLAY");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_8 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step8 && step_8)
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
    }
}
