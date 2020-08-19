using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.MPAC;
using System;
using System.Collections.Generic;
using System.IO;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Reusable.Generic;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace Selenium.Scripts.Tests
{
    class MammoHangingSupport
    {
        public Login login { get; set; }
        public string filepath { get; set; }

        public MammoHangingSupport(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }

        /// <summary>
        ///  Image Review/Mammo Hanging Support - Orientation for Breast Tomo and demographics
        /// </summary>
        public TestCaseResult Test_162299(String testid, String teststeps, int stepcount)
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

                //step1   Login ICA 7.0/BluRing viewer.
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //step2    Load the study Breast Tommosynhsis Image in Bluring viewer.
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //step3   Verify its demographics details on the image in Bluring viewer.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step3)
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

                //step4   click on exit button in BluRing viewer.
                viewer.CloseBluRingViewer();
                ExecutedSteps++;

                //step5   Load the same study in HTML 4 viewer.
                UserPreferences userPrefer = new UserPreferences();
                userPrefer.OpenUserPreferences();
                userPrefer.SwitchToUserPrefFrame();
                userPrefer.ClickElement(userPrefer.HTML4RadioBtn());                
                userPrefer.CloseUserPreferences();
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer StudyViewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                ExecutedSteps++;

                //step6    Verify its demographics details on the image in HTML4 viewer
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step6 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_1X1());
                if (step6)
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

                //step7    Click on close button in HTML4 viewer
                login.CloseStudy();
                ExecutedSteps++;

                //step8   Load the same Breast Tommosynhsis Image in BluRing viewer.
                userPrefer.OpenUserPreferences();
                userPrefer.SwitchToUserPrefFrame();
                viewer.ClickElement(userPrefer.BluringViewerRadioBtn());                
                userPrefer.CloseUserPreferences();
                studies.SelectStudy("Accession", Accession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //step9 - step11   Check the horizontal and vertical  orientation markers.
                ++ExecutedSteps;
                ++ExecutedSteps;
                viewer.CloseStudypanel(1);
                viewer.OpenPriors(3, "click");
                PageLoadWait.WaitForFrameLoad(20);
                viewer.ChangeViewerLayout("2x2");
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step9 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step9)
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

                //step12   Close the viewer.
                viewer.CloseBluRingViewer();
                ExecutedSteps++;

                //step13    Load a regular MG study.
                studies.ClearFields();
                studies.SearchStudy(AccessionNo: Accession[1]);
                studies.SelectStudy("Accession", Accession[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //step14    Verify its demographics details on the image in Bluring viewer.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step14 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step14)
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

                //step15   Click on Exit button in BluRing viewer.
                viewer.CloseBluRingViewer();
                ExecutedSteps++;

                //step16   Load the same study in HTML 4 viewer.
                userPrefer.OpenUserPreferences();
                userPrefer.SwitchToUserPrefFrame();
                userPrefer.ClickElement(userPrefer.HTML4RadioBtn());                
                userPrefer.CloseUserPreferences();
                studies.SelectStudy("Accession", Accession[1]);
                StudyViewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                ExecutedSteps++;

                //step17   Verify the demographics details displayed on the image in HTML4 viewer.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step17 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_1X1());
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

                //step18    Click on Close button in HTML4 viewer.
                login.CloseStudy();
                ExecutedSteps++;

                //step19    Load the same Breast Tommosynhsis study in Bluring viewer.
                userPrefer.OpenUserPreferences();
                userPrefer.SwitchToUserPrefFrame();
                userPrefer.ClickElement(userPrefer.BluringViewerRadioBtn());                
                userPrefer.CloseUserPreferences();
                studies.SelectStudy("Accession", Accession[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //step20 - step22   Check the horizontal and vertical orientation markers.
                ++ExecutedSteps;
                ++ExecutedSteps;
                viewer.ChangeViewerLayout("2x2");
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step20 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step20)
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

                //step23   Close the viewer. 
                viewer.CloseBluRingViewer();
                ExecutedSteps++;

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
		/// Sharpness enhancement for Grainy images
		/// </summary>
		/// <param name="testid"></param>
		/// <param name="teststeps"></param>
		/// <param name="stepcount"></param>
		/// <returns></returns>
		public TestCaseResult Test_161595(String testid, String teststeps, int stepcount)
        {

            // Declare and initialize variables
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String FirstNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstNameList");
                String LastNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastNameList");
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String[] FirstName = FirstNameList.Split(':');
                String[] LastName = LastNameList.Split(':');
                String[] PatientID = PatientIDList.Split(':');

                //step1 Log in to iCA and navigate to studies tab.
                login.LoginIConnect(adminUserName, adminPassword);
                var studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //step2 Select the study (Study name : TEST,PATIENT,Modality : MG),click on Universal button and then verify all the images from the thumbnail bar.
                studies.SearchStudy(LastName: LastName[0], FirstName: FirstName[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Patient ID", PatientID[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_allThumbnailsViewports));
                if (step2)
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

                //step3 Apply the Zoom on the images.
                IWebElement ele = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                int width = ele.Size.Width;
                int height = ele.Size.Height;
                viewer.SelectViewerTool(BluRingTools.Interactive_Zoom);
                viewer.ApplyTool_Zoom(width / 2, height, width / 4, height / 3);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step3)
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

                //step4  Close the study. 
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

                //step5 Select the study (Study name : NUTT LISA Modality: MG),click on Universal button and then verify all the images from the thumbnail bar.
                studies.SearchStudy(LastName: LastName[1], FirstName: FirstName[1], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Patient ID", PatientID[1]);
                BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step5 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_allThumbnailsViewports));
                if (step5)
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

                //step6  Apply the Zoom on the images. 
                viewer.SelectViewerTool(BluRingTools.Interactive_Zoom);
                viewer.ApplyTool_Zoom(width / 2, height, width / 4, height / 3);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step6)
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
                viewer.CloseBluRingViewer();

                //step7 Select the study (Study name : FOUR VIEW MAMMOGRAM Modality: MG),click on Universal button and then verify all the images from the thumbnail bar.
                studies.SearchStudy(LastName: LastName[2], FirstName: FirstName[2], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Patient ID", PatientID[2]);
                BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_allThumbnailsViewports));
                if (step7)
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

                //step8 Apply the Zoom on the images.
                viewer.SelectViewerTool(BluRingTools.Interactive_Zoom);
                viewer.ApplyTool_Zoom(width / 2, height, width / 4, height / 3);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step8 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step8)
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
                viewer.CloseBluRingViewer();

                //step9 Select the study (Study name : VANWAGONER BEATRICE E Modality: MG),click on Universal button and then verify all the images from the thumbnail bar.
                studies.SearchStudy(LastName: LastName[3], FirstName: FirstName[3], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Patient ID", PatientID[3]);
                BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step9 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_allThumbnailsViewports));
                if (step9)
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

                //step10  Apply the Zoom on the images. 
                viewer.SelectViewerTool(BluRingTools.Interactive_Zoom);
                viewer.ApplyTool_Zoom(width / 2, height, width / 4, height / 3);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step10 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
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
                viewer.CloseBluRingViewer();

                //step11 Select the study (Study name : Monroe MARY E Modality: MG),click on Universal button and then verify all the images from the thumbnail bar.
                studies.SearchStudy(LastName: LastName[4], FirstName: FirstName[4], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Patient ID", PatientID[4]);
                BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step11 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_allThumbnailsViewports));
                if (step11)
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

                //step12  Apply the Zoom on the images. 
                viewer.SelectViewerTool(BluRingTools.Interactive_Zoom);
                viewer.ApplyTool_Zoom(width / 2, height, width / 4, height / 3);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step12 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step12)
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
