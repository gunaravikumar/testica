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
using OpenQA.Selenium.Remote;
using System.Linq;
using Selenium.Scripts.Pages.MergeServiceTool;
using System.Threading;
using Selenium.Scripts.Pages.eHR;
using OpenQA.Selenium.Support.UI;

namespace Selenium.Scripts.Tests
{
    class GlobalStack
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public EHR ehr { get; set; }
        public WpfObjects wpfobject;
        public HPLogin hplogin { get; set; }
        public HPHomePage hphomepage { get; set; }
		public Studies studies { get; set; }
		public BasePage basePage { get; set; }
		public BluRingViewer bluringviewer { get; set; }
        UserPreferences userPreferences = new UserPreferences();
        public bool DefaultUserpref = true;
        public String adminUserName = Config.adminUserName;
        public String adminPassword = Config.adminPassword;

        public GlobalStack(String classname)
        {
            login = new Login();
			bluringviewer = new BluRingViewer();
			studies = new Studies();
			basePage = new BasePage();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }
        public string EA_131 = "VMSSA-4-38-131";

        /// <summary>
        /// Cross study panel loading in Global Stack mode
        /// </summary>
        public TestCaseResult Test_169903(String testid, String teststeps, int stepcount)
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
                String[] arrAccession = AccessionList.Split(':');
                String[] Accession = AccessionList.Split(':');

                string EA_91 = login.GetHostName(Config.EA91);

                ServiceTool servicetool = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();
                BluRingViewer viewer = new BluRingViewer();
                UserPreferences UserPref = new UserPreferences();
                Studies study;

                //step1 Preconditions
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                study = (Studies)login.Navigate("Studies");
                study.OpenUserPreferences();
                UserPref.SwitchToUserPrefFrame();
                UserPref.ModalityDropDown().SelectByText("MR");
                UserPref.ExamMode("1").Click();
                UserPref.ModalityDropDown().SelectByText("CT");
                UserPref.ExamMode("1").Click();
                study.CloseUserPreferences();
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //step2 Login to iCA Webaccess.
                //Load a study with multiple series and priors into two study panels.                
                study.SearchStudy(AccessionNo: arrAccession[0], Datasource: EA_91);
                PageLoadWait.WaitForPageLoad(20);
                study.SelectStudy("Accession", arrAccession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.OpenPriors(accession: arrAccession[1]);
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitforViewports();
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 3 - For Study Panel 1 - apply Global Stack mode.
                //For Study Panel 2 - Do not apply Global Stack mode - study panel is series mode.
                viewer.clickglobalstackIcon(1);
                var panels = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudyPanel));
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid + "_3_studypanel_", ExecutedSteps + 1, 1);
                bool status3_1 = study.CompareImage(result.steps[ExecutedSteps], panels[0], 1, RGBTolerance: 70);
                result.steps[ExecutedSteps].SetPath(testid + "_3_studypanel_", ExecutedSteps + 1, 2);
                bool status3_2 = study.CompareImage(result.steps[ExecutedSteps], panels[1], 2, 1, RGBTolerance: 70);
                if (status3_1 && status3_2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.ErrorLog("Image Compare--" + status3_1 + status3_2);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 4 - 	Perform cross study panel loading - i.e.. Drag one series from Study 1 Thumbnail or from Exam list to study panel 2.              
                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(3, 2)).Click(); //Viewport, study panel
                IWebElement viewPort = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                IList<IWebElement> Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(1) " + BluRingViewer.div_thumbnails)); //1st panel thumbnail list
                TestCompleteAction action = new TestCompleteAction();
                try
                {
                    Thread.Sleep(30000);
                    action.ClickAndHold(Thumbnail_list[1]);
                    action.MoveToElement(viewPort).Click().Perform();
                    Thread.Sleep(10000);
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Step 4: Exception while thumbnail drag and drop --" + e);
                    action = new TestCompleteAction();
                    Thread.Sleep(30000);
                    action.ClickAndHold(Thumbnail_list[1]);
                    action.MoveToElement(viewPort);
                    action.Click(viewPort);
                    action.MoveToElement(Thumbnail_list[1]).Perform();
                    Thread.Sleep(10000);
                }
                panels = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudyPanel));
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid + "_4_studypanel_", ExecutedSteps + 1, 1);
                bool status4_1 = study.CompareImage(result.steps[ExecutedSteps], panels[0], 1, RGBTolerance: 70);
                result.steps[ExecutedSteps].SetPath(testid + "_4_studypanel_", ExecutedSteps + 1, 2);
                bool status4_2 = study.CompareImage(result.steps[ExecutedSteps], panels[1], 2, 1, RGBTolerance: 70);
                if (status4_1 && status4_2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.ErrorLog("Image Compare--" + status4_1 + status4_2);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 5 - 	Perform cross study panel loading - i.e.. Drag one series from Study panel 2 Thumbnail or from Exam list to study panel 1.              
                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(3, 1)).Click(); //Viewport, study panel
                viewPort = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(2) " + BluRingViewer.div_thumbnails)); //2nd panel thumbnail list
                action = new TestCompleteAction();
                try
                {
                    Thread.Sleep(30000);
                    action.ClickAndHold(Thumbnail_list[1]);
                    action.MoveToElement(viewPort).Click().Perform();
                    Thread.Sleep(10000);
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Step 5: Exception while thumbnail drag and drop --" + e);
                    action = new TestCompleteAction();
                    Thread.Sleep(30000);
                    action.ClickAndHold(Thumbnail_list[1]);
                    action.MoveToElement(viewPort);
                    action.Click(viewPort);
                    action.MoveToElement(Thumbnail_list[1]).Perform();
                    Thread.Sleep(10000);
                }
                panels = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudyPanel));
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid + "_5_studypanel_", ExecutedSteps + 1, 1);
                bool status5_1 = study.CompareImage(result.steps[ExecutedSteps], panels[0], 1, RGBTolerance: 70);
                result.steps[ExecutedSteps].SetPath(testid + "_5_studypanel_", ExecutedSteps + 1, 2);
                bool status5_2 = study.CompareImage(result.steps[ExecutedSteps], panels[1], 2, 1, RGBTolerance: 70);
                if (status5_1 && status5_2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.ErrorLog("Image Compare--" + status5_1 + status5_2);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 6 - Scroll images in global stack mode and verify the highlighted thumbnails.
                IWebElement element = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                viewer.MouseScrollUsingArrowKeys(element, "down", 42);
                panels = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudyPanel));
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid + "_6_studypanel_", ExecutedSteps + 1, 1);
                bool status6_1 = study.CompareImage(result.steps[ExecutedSteps], panels[0], 1, RGBTolerance: 70);
                result.steps[ExecutedSteps].SetPath(testid + "_6_studypanel_", ExecutedSteps + 1, 2);
                bool status6_2 = study.CompareImage(result.steps[ExecutedSteps], panels[1], 2, 1, RGBTolerance: 70);
                if (status6_1 && status6_2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.ErrorLog("Image Compare--" + status6_1 + status6_2);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 7 - Apply Global Stack in study panel 2.
                viewer.clickglobalstackIcon(2);
                panels = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudyPanel));
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid + "_7_studypanel_", ExecutedSteps + 1, 1);
                bool status7_1 = study.CompareImage(result.steps[ExecutedSteps], panels[0], 1, RGBTolerance: 70);
                result.steps[ExecutedSteps].SetPath(testid + "_7_studypanel_", ExecutedSteps + 1, 2);
                bool status7_2 = study.CompareImage(result.steps[ExecutedSteps], panels[1], 2, 1, RGBTolerance: 70);
                if (status7_1 && status7_2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.ErrorLog("Image Compare--" + status7_1 + status7_2);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout Application.            
                login.Logout();

                //Return Results.
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
        /// Global Stack mode in multiple study panels
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>

        public TestCaseResult Test_169466(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            Studies studies = null;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');
                UserPreferences userPreferences = new UserPreferences();
                UserPreferences userpref = new UserPreferences();

                //Step-1 User Preferences> select modality > Exam mode = OFF
                login.LoginIConnect(adminUserName, adminPassword);
                login.OpenUserPreferences();
                userPreferences.SwitchToUserPrefFrame();
                userPreferences.ModalityDropDown().SelectByText("MR");
                userpref.ExamMode_OFF().Click();
                userPreferences.CloseUserPreferences();
                ExecutedSteps++;

                //Step-2 From the Studies tab > query and load study with multiple series and priors.
                //From the Exam list, load multiple studies in order to view multiple study panels.

                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Accession", Accession[0]);
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.OpenPriors(1);
                int Priorscount = viewer.GetStudyPanelCount();
                bool Globalstackactive = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_globalstackicon(1)).GetAttribute("class").Contains("Active");
                if (Priorscount == 2 && !Globalstackactive)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }


                //Step:3 In Study Panel 1, apply Global Stack.
                viewer.clickglobalstackIcon(1);
                Thread.Sleep(10000);
                var sliderValue_1 = viewer.GetSliderValue(1, 1);
                var sliderValue_2 = viewer.GetSliderValue(1, 2);
                var sliderValue_3 = viewer.GetSliderValue(1, 3);
                var sliderValue_4 = viewer.GetSliderValue(1, 4);
                Logger.Instance.InfoLog("Value 1 is" + sliderValue_1 + " Value 2 is:" + sliderValue_2);
                bool step3_1 = sliderValue_1.Equals(1) && sliderValue_2.Equals(20) && sliderValue_3.Equals(39) && sliderValue_4.Equals(64);
                var sliderMaxValue_1 = viewer.GetSliderMaxValue(1, 1);
                var sliderMaxValue_2 = viewer.GetSliderMaxValue(1, 2);
                var sliderMaxValue_3 = viewer.GetSliderMaxValue(1, 3);
                var sliderMaxValue_4 = viewer.GetSliderMaxValue(1, 4);
                bool step3_2 = sliderMaxValue_1.Equals(145) && sliderMaxValue_2.Equals(145) && sliderMaxValue_3.Equals(145) && sliderMaxValue_4.Equals(145);
                sliderValue_1 = viewer.GetSliderValue(2, 1);
                sliderValue_2 = viewer.GetSliderValue(2, 2);
                sliderValue_3 = viewer.GetSliderValue(2, 3);
                sliderValue_4 = viewer.GetSliderValue(2, 4);
                Logger.Instance.InfoLog("Value 1 is" + sliderValue_1 + " Value 2 is:" + sliderValue_2);
                bool step3_3 = sliderValue_1.Equals(1) && sliderValue_2.Equals(1) && sliderValue_3.Equals(1) && sliderValue_4.Equals(1);
                sliderMaxValue_1 = viewer.GetSliderMaxValue(2, 1);
                sliderMaxValue_2 = viewer.GetSliderMaxValue(2, 2);
                sliderMaxValue_3 = viewer.GetSliderMaxValue(2, 3);
                sliderMaxValue_4 = viewer.GetSliderMaxValue(2, 4);
                bool step3_4 = sliderMaxValue_1.Equals(22) && sliderMaxValue_2.Equals(48) && sliderMaxValue_3.Equals(18) && sliderMaxValue_4.Equals(19);

                if (step3_1 && step3_2 && step3_3 && step3_4)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step: 4 Apply Global Stack mode to all Study Panels.Scroll images in global stack mode and verify the highlighted thumbnails.
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 1)));
                var actions = new TestCompleteAction();
                IWebElement element_1 = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                actions.MouseScroll(element_1, "down", "20");
                //bool step4 = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_StackSlider).GetAttribute("innerHTML").Equals("21");
                bool step4_1 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(2)), "rgba(90, 170, 255, 1)");

                if (step4_1)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }


                //Step: 5  Apply Global Stack mode to all Study Panels.Scroll images in global stack mode and verify the highlighted thumbnails.
                viewer.clickglobalstackIcon(2);
                Thread.Sleep(10000);
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 2)));
                var actions1 = new TestCompleteAction();
                IWebElement element_11 = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                actions1.MouseScroll(element_11, "down", "23");
                bool step5_1 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(2, 2)), "rgba(90, 170, 255, 1)");

                if (step5_1)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                viewer.CloseBluRingViewer();

                //Step: 6 - Apply Global Stack mode to all supported modalities.Verify Global Stack mode cannot be applied to KO and PR series.

                studies.SearchStudy("Accession", Accession[1]);
                studies.SelectStudy("Accession", Accession[1]);
                var viewer6 = BluRingViewer.LaunchBluRingViewer();
                viewer6.clickglobalstackIcon(1);
                Thread.Sleep(10000);
                sliderMaxValue_1 = viewer.GetSliderMaxValue(1, 1);
                sliderMaxValue_2 = viewer.GetSliderMaxValue(1, 2);
                sliderMaxValue_3 = viewer.GetSliderMaxValue(1, 3);
                sliderMaxValue_4 = viewer.GetSliderMaxValue(1, 4);
                bool step6_1 = sliderMaxValue_1.Equals(41) && sliderMaxValue_2.Equals(41) && sliderMaxValue_3.Equals(41) && sliderMaxValue_4.Equals(41);
                bool step6_2 = viewer6.VerifyBordorColor(viewer6.GetElement("cssselector", viewer6.GetStudyPanelThumbnailCss(1)), "rgba(0, 0, 0, 1)");
                bool step6_3 = viewer6.VerifyBordorColor(viewer6.GetElement("cssselector", viewer6.GetStudyPanelThumbnailCss(2)), "rgba(0, 0, 0, 1)");
                var actions6 = new TestCompleteAction();
                IWebElement element_6 = viewer6.GetElement(BasePage.SelectorType.CssSelector, viewer6.Activeviewport);
                actions6.MouseScroll(element_6, "down", "41").Perform();
                bool step6_4 = viewer6.VerifyBordorColor(viewer6.GetElement("cssselector", viewer6.GetStudyPanelThumbnailCss(43)), "rgba(90, 170, 255, 1)");

                if (step6_1 && step6_2 && step6_3 && step6_4)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                viewer.CloseBluRingViewer();

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
        /// Apply Global Stack to studies in Integrated mode
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>

        public TestCaseResult Test_169470(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            Studies studies = null;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');
                UserPreferences userPreferences = new UserPreferences();
                UserPreferences userpref = new UserPreferences();
                ehr = new EHR();
                ServiceTool servicetool = new ServiceTool();

                //Step-1 User Preferences> select modality > Exam mode = OFF
                login.LoginIConnect(adminUserName, adminPassword);
                login.OpenUserPreferences();
                userPreferences.SwitchToUserPrefFrame();
                userPreferences.ModalityDropDown().SelectByText("MR");
                userpref.ExamMode_OFF().Click();
                userPreferences.CloseUserPreferences();
                login.Logout();
                ExecutedSteps++;

                //Step -2: From TestEHR - enter required common parameters, and search by any Search keys
                //PreCondition
                TestFixtures.UpdateFeatureFixture("bluring", value: "true:BluRing");
                TestFixtures.UpdateFeatureFixture("allowshowselector", value: "True");
                TestFixtures.UpdateFeatureFixture("allowshowselectorsearch", value: "True");
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Integrator");
                servicetool.WaitWhileBusy();
                servicetool.EanbleUserSharing_ShadowUser(usersharing: "Always Enabled");
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                ehr.LaunchEHR();
                ehr.SetCommonParameters();
                ehr.SetSearchKeys_Study(Accession[0]);
                ehr.SetSearchKeys_Study(login.GetHostName(Config.EA91), "Datasource");
                String url = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.CreateNewSesion();
                BluRingViewer viewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", url: url);
                ExecutedSteps++;

                //Step -3: Apply Global Stack mode.Scroll images in global stack mode and verify the highlighted thumbnails.

                viewer.clickglobalstackIcon(1);
                Thread.Sleep(6000);
                var sliderValue_1 = viewer.GetSliderValue(1, 1);
                var sliderValue_2 = viewer.GetSliderValue(1, 2);
                var sliderValue_3 = viewer.GetSliderValue(1, 3);
                var sliderValue_4 = viewer.GetSliderValue(1, 4);
                Logger.Instance.InfoLog("Value 1 is" + sliderValue_1 + " Value 2 is:" + sliderValue_2);
                bool step3_1 = sliderValue_1.Equals(1) && sliderValue_2.Equals(20) && sliderValue_3.Equals(39) && sliderValue_4.Equals(64);
                var sliderMaxValue_1 = viewer.GetSliderMaxValue(1, 1);
                var sliderMaxValue_2 = viewer.GetSliderMaxValue(1, 2);
                var sliderMaxValue_3 = viewer.GetSliderMaxValue(1, 3);
                var sliderMaxValue_4 = viewer.GetSliderMaxValue(1, 4);
                bool step3_2 = sliderMaxValue_1.Equals(145) && sliderMaxValue_2.Equals(145) && sliderMaxValue_3.Equals(145) && sliderMaxValue_4.Equals(145);
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 1)));
                var actions = new TestCompleteAction();
                IWebElement element_ele = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                actions.MouseScroll(element_ele, "down", "20").Perform();
                bool step3_5 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(2)), "rgba(90, 170, 255, 1)");

                if (step3_1 && step3_2 && step3_5)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step: 4 	Precondition: User Preferences> select modality > Exam mode = ON.

                login.LoginIConnect(adminUserName, adminPassword);
                login.OpenUserPreferences();
                userPreferences.SwitchToUserPrefFrame();
                userPreferences.ModalityDropDown().SelectByText("MR");
                userpref.ExamMode_ON().Click();
                userPreferences.CloseUserPreferences();
                login.Logout();
                ehr.LaunchEHR();
                ehr.SetCommonParameters();
                ehr.SetSearchKeys_Study(Accession[0]);
                ehr.SetSearchKeys_Study(login.GetHostName(Config.EA91), "Datasource");
                url = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                ExecutedSteps++;

                //Step:5 Load a study with multiple series

                login.CreateNewSesion();
                viewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", url: url);
                sliderValue_1 = viewer.GetSliderValue(1, 1);
                sliderValue_2 = viewer.GetSliderValue(1, 2);
                sliderValue_3 = viewer.GetSliderValue(1, 3);
                sliderValue_4 = viewer.GetSliderValue(1, 4);
                Logger.Instance.InfoLog("Value 1 is" + sliderValue_1 + " Value 2 is:" + sliderValue_2);
                bool step5_1 = sliderValue_1.Equals(1) && sliderValue_2.Equals(1) && sliderValue_3.Equals(1) && sliderValue_4.Equals(1);
                var isGlobalStackEnabled = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_globalStack).GetAttribute("class").Contains("disabled");

                if (step5_1 && isGlobalStackEnabled)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step:6-  Turn Exam mode ON for all viewports.

                viewer.OpenExammode(1, 1);
                var isExamModeactive = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_Exammodebuttonclick).GetAttribute("class").Contains("Active");
                sliderValue_1 = viewer.GetSliderValue(1, 1);
                sliderValue_2 = viewer.GetSliderValue(1, 2);
                sliderValue_3 = viewer.GetSliderValue(1, 3);
                sliderValue_4 = viewer.GetSliderValue(1, 4);
                Logger.Instance.InfoLog("Value 1 is" + sliderValue_1 + " Value 2 is:" + sliderValue_2);
                bool step6_1 = sliderValue_1.Equals(1) && sliderValue_2.Equals(1) && sliderValue_3.Equals(1) && sliderValue_4.Equals(1);
                var isGlobalStackEnabled_1 = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_globalStack).GetAttribute("class").Contains("disabled");

                viewer.OpenExammode(2, 1);
                var isExamModeactive_2 = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_Exammodebuttonclick).GetAttribute("class").Contains("Active");
                sliderValue_1 = viewer.GetSliderValue(1, 1);
                sliderValue_2 = viewer.GetSliderValue(1, 2);
                sliderValue_3 = viewer.GetSliderValue(1, 3);
                sliderValue_4 = viewer.GetSliderValue(1, 4);
                Logger.Instance.InfoLog("Value 1 is" + sliderValue_1 + " Value 2 is:" + sliderValue_2);
                bool step6_2 = sliderValue_1.Equals(1) && sliderValue_2.Equals(1) && sliderValue_3.Equals(1) && sliderValue_4.Equals(1);
                var isGlobalStackEnabled_2 = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_globalStack).GetAttribute("class").Contains("disabled");


                viewer.OpenExammode(3, 1);
                var isExamModeactive_3 = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_Exammodebuttonclick).GetAttribute("class").Contains("Active");
                sliderValue_1 = viewer.GetSliderValue(1, 1);
                sliderValue_2 = viewer.GetSliderValue(1, 2);
                sliderValue_3 = viewer.GetSliderValue(1, 3);
                sliderValue_4 = viewer.GetSliderValue(1, 4);
                Logger.Instance.InfoLog("Value 1 is" + sliderValue_1 + " Value 2 is:" + sliderValue_2);
                bool step6_3 = sliderValue_1.Equals(1) && sliderValue_2.Equals(1) && sliderValue_3.Equals(1) && sliderValue_4.Equals(1);
                var isGlobalStackEnabled_3 = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_globalStack).GetAttribute("class").Contains("disabled");

                viewer.OpenExammode(4, 1);
                var isExamModeactive_4 = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_Exammodebuttonclick).GetAttribute("class").Contains("Active");
                sliderValue_1 = viewer.GetSliderValue(1, 1);
                sliderValue_2 = viewer.GetSliderValue(1, 2);
                sliderValue_3 = viewer.GetSliderValue(1, 3);
                sliderValue_4 = viewer.GetSliderValue(1, 4);
                Logger.Instance.InfoLog("Value 1 is" + sliderValue_1 + " Value 2 is:" + sliderValue_2);
                bool step6_4 = sliderValue_1.Equals(1) && sliderValue_2.Equals(1) && sliderValue_3.Equals(1) && sliderValue_4.Equals(1);
                var isGlobalStackEnabled_4 = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_globalStack).GetAttribute("class").Contains("disabled");


                if (isExamModeactive && step6_1 && isGlobalStackEnabled_1 && isExamModeactive_2 && step6_2 && isGlobalStackEnabled_2 && isExamModeactive_3 && step6_3 && isGlobalStackEnabled_3 &&
                    isExamModeactive_4 && step6_4 && isGlobalStackEnabled_4)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                login.LoginIConnect(adminUserName, adminPassword);
                login.OpenUserPreferences();
                userPreferences.SwitchToUserPrefFrame();
                userPreferences.ModalityDropDown().SelectByText("MR");
                userpref.ExamMode_OFF().Click();
                userPreferences.CloseUserPreferences();
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
        /// Verify global stack mode functionality for multiple modality study .
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>

        public TestCaseResult Test_169897(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            Studies studies = null;
            String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                UserPreferences userPreferences = new UserPreferences();
                UserPreferences userpref = new UserPreferences();
                DomainManagement domain;


                //Step-1 User Preferences> select modality > Exam mode = OFF
                login.LoginIConnect(adminUserName, adminPassword);
                domain = login.Navigate<DomainManagement>();
                domain.SearchDomain("SuperAdminGroup");
                domain.SelectDomain("SuperAdminGroup");
                domain.ClickEditDomain();
                IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(11)"));
                var dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add("Save Annotated Images", group1);
                dictionary.Add("Save Series", group1);
                domain.AddToolsToToolbox(dictionary, addToolAtEnd: true);
                domain.ClickSaveEditDomain();
                login.OpenUserPreferences();
                userPreferences.SwitchToUserPrefFrame();
                userPreferences.ModalityDropDown().SelectByText("CT");
                userpref.ExamMode_OFF().Click();
                userPreferences.ModalityDropDown().SelectByText("PT");
                userpref.ExamMode_OFF().Click();
                userPreferences.CloseUserPreferences();
                ExecutedSteps++;

                //step -2 Login to ICA with any privileged user.From the Studies tab > query and load a study with multiple modalities.
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: AccessionList, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", AccessionList);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                var sliderValue_1 = viewer.GetSliderValue(1, 1);
                var sliderValue_2 = viewer.GetSliderValue(1, 2);
                var sliderValue_3 = viewer.GetSliderValue(1, 3);
                var sliderValue_4 = viewer.GetSliderValue(1, 4);
                Logger.Instance.InfoLog("Value 1 is" + sliderValue_1 + " Value 2 is:" + sliderValue_2);
                bool step2_1 = sliderValue_1.Equals(1) && sliderValue_2.Equals(1) && sliderValue_3.Equals(1) && sliderValue_4.Equals(1);
                bool isGlobalStackEnabled = viewer.GetElement("cssselector", BluRingViewer.div_globalStack).Enabled;

                if (isGlobalStackEnabled && step2_1)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step -3 : Apply Global Stack. Scroll images in global stack mode and verify the highlighted thumbnails.

                viewer.clickglobalstackIcon(1);
                Thread.Sleep(3000);
                var sliderValue_11 = viewer.GetSliderValue(1, 1);
                var sliderValue_22 = viewer.GetSliderValue(1, 2);
                var sliderValue_33 = viewer.GetSliderValue(1, 3);
                var sliderValue_44 = viewer.GetSliderValue(1, 4);
                Logger.Instance.InfoLog("Value 1 is" + sliderValue_11 + " Value 2 is:" + sliderValue_22);
                bool step3_1 = sliderValue_11.Equals(1) && sliderValue_22.Equals(5) && sliderValue_33.Equals(9) && sliderValue_44.Equals(13);
                var sliderMaxValue_1 = viewer.GetSliderMaxValue(1, 1);
                var sliderMaxValue_2 = viewer.GetSliderMaxValue(1, 2);
                var sliderMaxValue_3 = viewer.GetSliderMaxValue(1, 3);
                var sliderMaxValue_4 = viewer.GetSliderMaxValue(1, 4);
                bool step3_2 = sliderMaxValue_1.Equals(16) && sliderMaxValue_2.Equals(16) && sliderMaxValue_3.Equals(16) && sliderMaxValue_4.Equals(16);
                var actions = new TestCompleteAction();
                int StackSliderValue_1 = Int32.Parse(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_StackSlider).GetAttribute("innerHTML"));
                IWebElement element_1 = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                actions.MouseScroll(element_1, "down", "5");
                bool step3_3 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(2)), "rgba(90, 170, 255, 1)");
                if (step3_1 && step3_2 && step3_3)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step -4: Apply line measurement to CT image.

                var actions4 = new TestCompleteAction();
                int StackSliderValue_4 = Int32.Parse(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_StackSlider).GetAttribute("innerHTML"));
                IWebElement element_4 = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                actions4.MouseScroll(element_4, "up", "5").Perform();
                Thread.Sleep(2000);
                var step4 = viewer.SelectViewerTool(BluRingTools.Line_Measurement);
                viewer.ApplyTool_LineMeasurement();
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step4_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                var actions5 = new TestCompleteAction();
                int StackSliderValue_5 = Int32.Parse(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_StackSlider).GetAttribute("innerHTML"));
                IWebElement element_5 = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                actions5.MouseScroll(element_5, "down", "5").Perform();
                Thread.Sleep(2000);
                var step4_2 = viewer.SelectViewerTool(BluRingTools.Draw_Ellipse);
                viewer.ApplyTool_DrawEllipse();
                Thread.Sleep(2000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step4_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                bool isPRCreated = viewer.SavePresentationState(BluRingTools.Save_Annotated_Image, BluRingTools.Add_Text, 1, 1);
                Thread.Sleep(5000);
                viewer.CloseBluRingViewer();
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: AccessionList, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", AccessionList);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 1)));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                bool step4_4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                var actions_51 = new TestCompleteAction();
                String viewportcss = viewer.GetViewportCss(1, 0);
                int StackSliderValue_51 = Int32.Parse(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_StackSlider).GetAttribute("innerHTML"));
                IWebElement element_51 = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                actions_51.MouseScroll(element_51, "down", "1").Perform();
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
                bool step4_5 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));

                if (step4 && step4_1 && step4_2 && step4_3 && isPRCreated && step4_4 && step4_5)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                viewer.CloseBluRingViewer();
                //Step:5- Preconditions:  User Preference -Modality = CT > Exam mode ON;
                login.OpenUserPreferences();
                userPreferences.SwitchToUserPrefFrame();
                userPreferences.ModalityDropDown().SelectByText("CT");
                userpref.ExamMode_ON().Click();
                userPreferences.CloseUserPreferences();
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: AccessionList, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", AccessionList);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                var isGlobalStackEnabled_5 = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_globalStack).GetAttribute("class").Contains("disabled");

                if (isGlobalStackEnabled_5)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                viewer.CloseBluRingViewer();
                login.OpenUserPreferences();
                userPreferences.SwitchToUserPrefFrame();
                userPreferences.ModalityDropDown().SelectByText("CT");
                userpref.ExamMode_OFF().Click();
                userPreferences.CloseUserPreferences();
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
                try
                {
                    HPLogin hplogin = new HPLogin();
                    BasePage.Driver.Navigate().GoToUrl("https://" + Config.EA91 + "/webadmin");
                    HPHomePage hphome = (HPHomePage)hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, EA_URL: "https://" + Config.EA91 + "/webadmin");
                    WorkFlow workflow = (WorkFlow)hphome.Navigate("Workflow");
                    workflow.NavigateToLink("Workflow", "Archive Search");
                    workflow.HPSearchStudy("Accessionno", AccessionList);
                    if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                        workflow.DeletePaticularModality("PR");

                    hplogin.LogoutHPen();
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("PR delete exception -- " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                }
            }


        }

        /// <summary>
        /// Apply Global Stack mode to shared studies
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>

        public TestCaseResult Test_169467(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            Studies studies = null;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String User = "U1" + new Random().Next(10000);
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                ServiceTool servicetool = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();

                // Step-1 Preconditions: Install ICA.
                // All testing done in the Universal Viewer.
                // User Preferences> select modality > Exam mode = OFF
                // Studies exists in Inbound and Outbound tabs.

                //To enable study sharing
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                wpfobject.SelectCheckBox(ServiceTool.EnableFeatures.Name.EnableStudySharing, 1);
                wpfobject.WaitTillLoad();
                servicetool.wpfobject.ClickOkPopUp();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.wpfobject.ClickOkPopUp();
                // Restart and close the service tool
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();
                login.LoginIConnect(adminUserName, adminPassword);
                login.OpenUserPreferences();
                userPreferences.SwitchToUserPrefFrame();
                var status = userPreferences.ExamMode_ON().GetAttribute("checked");
                if (status == "true")
                {
                    userPreferences.ExamMode_OFF().Click();
                    DefaultUserpref = false;
                }
                userPreferences.CloseUserPreferences();
                var usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(User, Config.adminGroupName, Config.adminRoleName);
                //Enable grant access in Domain Management page
                var domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                domain.SetCheckBoxInEditDomain("grant", 0);
                domain.ClickSaveEditDomain();
                var role = (RoleManagement)login.Navigate("RoleManagement");
                role.SelectDomainfromDropDown(Config.adminGroupName);
                role.SearchRole(Config.adminRoleName);
                role.SelectRole(Config.adminRoleName);
                role.ClickEditRole();
                role.ClickElement(role.GrantAccessRadioBtn_Anyone());
                role.ClickSaveEditRole();
                ExecutedSteps++;

                //Step- 2 Login to ICA with any privileged user.
                //From the Inbounds tab  query and load study with multiple images series Apply Global Stack mode.(instead of inbounds tab verifying the outbounds tab)
                // Note: step 2 & step 4 are swapped

                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: AccessionID);
                studies.SelectStudy("Accession", AccessionID);
                studies.GrantAccessToUsers(Config.adminGroupName, User);
                login.Logout();
                login.LoginIConnect(User, User);
                var inbound = (Inbounds)login.Navigate("Inbounds");
                inbound.SearchStudy(AccessionNo: AccessionID, Study_Performed_Period: "All Dates", Study_Received_Period: "All Dates");
                inbound.SelectStudy("Accession", AccessionID);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.clickglobalstackIcon(1);
                BluRingViewer.WaitforViewports();
                var sliderValue_1 = viewer.GetSliderValue(1, 1);
                var sliderValue_2 = viewer.GetSliderValue(1, 2);
                var sliderValue_3 = viewer.GetSliderValue(1, 3);
                var sliderValue_4 = viewer.GetSliderValue(1, 4);
                Logger.Instance.InfoLog("Value 1 is" + sliderValue_1 + " Value 2 is:" + sliderValue_2);
                bool step2_1 = sliderValue_1.Equals(1) && sliderValue_2.Equals(6) && sliderValue_3.Equals(26) && sliderValue_4.Equals(46);
                var sliderMaxValue_1 = viewer.GetSliderMaxValue(1, 1);
                var sliderMaxValue_2 = viewer.GetSliderMaxValue(1, 2);
                var sliderMaxValue_3 = viewer.GetSliderMaxValue(1, 3);
                var sliderMaxValue_4 = viewer.GetSliderMaxValue(1, 4);
                bool step2_2 = sliderMaxValue_1.Equals(127) && sliderMaxValue_2.Equals(127) && sliderMaxValue_3.Equals(127) && sliderMaxValue_4.Equals(127);
                if (step2_1 && step2_2)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step- 3 Turn Global Stack off to switch back to Series Stack Mode
                viewer.clickglobalstackIcon(1);
                BluRingViewer.WaitforViewports();
                var seriessliderValue_1 = viewer.GetSliderValue(1, 1);
                var seriessliderValue_2 = viewer.GetSliderValue(1, 2);
                var seriessliderValue_3 = viewer.GetSliderValue(1, 3);
                var seriessliderValue_4 = viewer.GetSliderValue(1, 4);
                Logger.Instance.InfoLog("Value 1 is" + seriessliderValue_1 + " Value 2 is:" + seriessliderValue_2);
                bool step3_1 = seriessliderValue_1.Equals(1) && seriessliderValue_2.Equals(1) && seriessliderValue_3.Equals(1) && seriessliderValue_4.Equals(1);
                var seriessliderMaxValue_1 = viewer.GetSliderMaxValue(1, 1);
                var seriessliderMaxValue_2 = viewer.GetSliderMaxValue(1, 2);
                var seriessliderMaxValue_3 = viewer.GetSliderMaxValue(1, 3);
                var seriessliderMaxValue_4 = viewer.GetSliderMaxValue(1, 4);
                bool step3_2 = seriessliderMaxValue_1.Equals(5) && seriessliderMaxValue_2.Equals(20) && seriessliderMaxValue_3.Equals(20) && seriessliderMaxValue_4.Equals(22);
                if (step3_1 && step3_2)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                viewer.CloseBluRingViewer();
                login.Logout();
                //Step- 4  From the Outbounds tab query and load study with multiple image/series. Apply Global Stack mode.(Instead of outbounds verified in inbounds)
                // Note: step 4 & step 2 are swapped

                login.LoginIConnect(adminUserName, adminPassword);
                var outbound = (Outbounds)login.Navigate("Outbounds");
                outbound.SearchStudy(AccessionNo: AccessionID, Study_Performed_Period: "All Dates", Study_Received_Period: "All Dates");
                outbound.SelectStudy("Accession", AccessionID);
                BluRingViewer.LaunchBluRingViewer();
                viewer.clickglobalstackIcon(1);
                BluRingViewer.WaitforViewports();
                var inboundssliderValue_1 = viewer.GetSliderValue(1, 1);
                var inboundssliderValue_2 = viewer.GetSliderValue(1, 2);
                var inboundssliderValue_3 = viewer.GetSliderValue(1, 3);
                var inboundssliderValue_4 = viewer.GetSliderValue(1, 4);
                Logger.Instance.InfoLog("Value 1 is" + inboundssliderValue_1 + " Value 2 is:" + inboundssliderValue_2);
                bool step4_1 = inboundssliderValue_1.Equals(1) && inboundssliderValue_2.Equals(6) && inboundssliderValue_3.Equals(26) && inboundssliderValue_4.Equals(46);
                var inboundssliderMaxValue_1 = viewer.GetSliderMaxValue(1, 1);
                var inboundssliderMaxValue_2 = viewer.GetSliderMaxValue(1, 2);
                var inboundssliderMaxValue_3 = viewer.GetSliderMaxValue(1, 3);
                var inboundssliderMaxValue_4 = viewer.GetSliderMaxValue(1, 4);
                bool step4_2 = inboundssliderMaxValue_1.Equals(127) && inboundssliderMaxValue_2.Equals(127) && inboundssliderMaxValue_3.Equals(127) && inboundssliderMaxValue_4.Equals(127);
                if (step4_1 && step4_2)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                //Step- 5 Turn Global Stack off to switch back to Series Stack Mode

                viewer.clickglobalstackIcon(1);
                BluRingViewer.WaitforViewports();
                var inboundsseriessliderValue_1 = viewer.GetSliderValue(1, 1);
                var inboundsseriessliderValue_2 = viewer.GetSliderValue(1, 2);
                var inboundsseriessliderValue_3 = viewer.GetSliderValue(1, 3);
                var inboundsseriessliderValue_4 = viewer.GetSliderValue(1, 4);
                Logger.Instance.InfoLog("Value 1 is" + seriessliderValue_1 + " Value 2 is:" + seriessliderValue_2);
                bool step5_1 = inboundsseriessliderValue_1.Equals(1) && inboundsseriessliderValue_2.Equals(1) && inboundsseriessliderValue_3.Equals(1) && inboundsseriessliderValue_4.Equals(1);
                var inboundsseriessliderMaxValue_1 = viewer.GetSliderMaxValue(1, 1);
                var inboundsseriessliderMaxValue_2 = viewer.GetSliderMaxValue(1, 2);
                var inboundsseriessliderMaxValue_3 = viewer.GetSliderMaxValue(1, 3);
                var inboundsseriessliderMaxValue_4 = viewer.GetSliderMaxValue(1, 4);
                bool step5_2 = inboundsseriessliderMaxValue_1.Equals(5) && inboundsseriessliderMaxValue_2.Equals(20) && inboundsseriessliderMaxValue_3.Equals(20) && inboundsseriessliderMaxValue_4.Equals(22);
                if (step5_1 && step5_2)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
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
                if (DefaultUserpref == false)
                {
                    login.LoginIConnect(adminUserName, adminPassword);
                    login.OpenUserPreferences();
                    userPreferences.SwitchToUserPrefFrame();
                    userPreferences.ExamMode_ON().Click();
                    userPreferences.CloseUserPreferences();
                }
            }

        }

		/// <summary>
		/// Verify Global Stack mode when Exam mode = ON (User Preferences)
		/// </summary>
		public TestCaseResult Test_169464(String testid, String teststeps, int stepcount)
		{
			//Fetch the data
			String Contactinfo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ContactInformation");
			String[] info = Contactinfo.Split('=');

			//Declare and initialize variables         
			TestCaseResult result;
			result = new TestCaseResult(stepcount);
			int ExecutedSteps = -1;
			String adminUserName = Config.adminUserName;
			String adminPassword = Config.adminPassword;

			//Set up Validation Steps
			result.SetTestStepDescription(teststeps);
			try
			{
				String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");

				//Step 1 - User Preferences> select modality > Exam mode = ON
				login.LoginIConnect(adminUserName, adminPassword);
				PageLoadWait.WaitForPageLoad(20);
				UserPreferences userpref = new UserPreferences();
				userpref.OpenUserPreferences();
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
				PageLoadWait.WaitForPageLoad(20);
				userpref.ModalityDropDown().SelectByText("MR");
				userpref.ExamMode("0").Click();
				userpref.CloseUserPreferences();
				PageLoadWait.WaitForPageLoad(20);
				ExecutedSteps++;

				//Step 2 - Load a study with multiple series. Accession[0] =>ACCCCN02			
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(AccessionNo: Accession, Datasource: login.GetHostName(Config.EA91));
				studies.SelectStudy("Accession", Accession);
				bluringviewer = BluRingViewer.LaunchBluRingViewer();
				bluringviewer.ChangeViewerLayout("2x2");
				ExecutedSteps++;

				//Step 3 - Turn Exam mode ON for all viewports.	
				IWebElement globalStackIcon = null;
				var globalStackModeEnabled = false;
				for (int count = 1; count < 5; count++)
				{
					bluringviewer.OpenCineToolBar(viewport: count, panel: 1);
					IWebElement examModeButton = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.button_ExamMode));
					examModeButton.Click();
					globalStackIcon = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_globalstackicon()));
					if (globalStackIcon.GetAttribute("class").Contains("toggle-disabled"))
					{
						Logger.Instance.InfoLog("Global Stack diabled after selecting exam mode - on in viewport:" + count);
					}
					else
					{
						Logger.Instance.InfoLog("Global Stack enabled selecting exam mode - on in viewport:" + count);
						globalStackModeEnabled = true;
						break;
					}
				}
				if (!globalStackModeEnabled)
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step 4 - Turn Exam mode ON for 1 viewport and OFF for another viewport.				
				for (int count = 2; count <= 4; count++)
				{
					bluringviewer.OpenCineToolBar(viewport: count, panel: 1);
					IWebElement examModeButton = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.button_ExamMode));
					examModeButton.Click();
					bluringviewer.ClickOnViewPort(panelnumber: 1, viewportnumber: count);
				}
				globalStackIcon = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_globalstackicon()));
				if (globalStackIcon.GetAttribute("class").Contains("toggle-disabled"))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				bluringviewer.CloseCineToolBar();
				bluringviewer.CloseBluRingViewer();

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
				try
				{
					//Revert precondition. Disable exam mode in MR modality
					Logger.Instance.InfoLog("Finally blocl started");
					login.Logout();
					login.LoginIConnect(adminUserName, adminPassword);
					PageLoadWait.WaitForPageLoad(20);
					UserPreferences userpref = new UserPreferences();
					userpref.OpenUserPreferences();
					BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
					PageLoadWait.WaitForPageLoad(20);
					userpref.ModalityDropDown().SelectByText("MR");
					userpref.ExamMode("1").Click();
					userpref.CloseUserPreferences();
					login.Logout();
				}
				catch (Exception ex)
				{
					Logger.Instance.WarnLog("Error in finally block: " + ex.Message);
				}
			}
		}

		/// <summary>
		/// Verify Global Stack mode when Exam mode = ON (User Preferences)
		/// </summary>
		public TestCaseResult Test_169465(String testid, String teststeps, int stepcount)
		{
			//Fetch the data
			String Contactinfo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ContactInformation");
			String[] info = Contactinfo.Split('=');

			//Declare and initialize variables         
			TestCaseResult result;
			result = new TestCaseResult(stepcount);
			int ExecutedSteps = -1;
			String adminUserName = Config.adminUserName;
			String adminPassword = Config.adminPassword;

			//Set up Validation Steps
			result.SetTestStepDescription(teststeps);
			try
			{

				String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
				//String[] PatientID = PatientIDList.Split(':');
				String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
				String[] Accession = AccessionList.Split(':');
                String totalImagesInGS = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TotalImagesInGlobalStack");

                //Step 1 - User Preferences> select modality > Exam mode = OFF
                login.LoginIConnect(adminUserName, adminPassword);
				PageLoadWait.WaitForPageLoad(20);
				UserPreferences userpref = new UserPreferences();
				userpref.OpenUserPreferences();
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
				PageLoadWait.WaitForPageLoad(20);
				userpref.ModalityDropDown().SelectByText("MR");
				userpref.ExamMode("1").Click();
				userpref.ModalityDropDown().SelectByText("CT");
				userpref.ExamMode("1").Click();
				userpref.ModalityDropDown().SelectByText("PT");
				userpref.ExamMode("1").Click();
				userpref.ModalityDropDown().SelectByText("OT");
				userpref.ExamMode("1").Click();
				userpref.ModalityDropDown().SelectByText("CR");
				userpref.ExamMode("1").Click();
				userpref.CloseUserPreferences();
				PageLoadWait.WaitForPageLoad(20);
				ExecutedSteps++;

				//Step 2 - From the Studies tab > query and load a study with multiple series. Acc => ACCGS			
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA1));
				studies.SelectStudy("Accession", Accession[0]);
				bluringviewer = BluRingViewer.LaunchBluRingViewer();
				IWebElement globalStackIcon = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_globalstackicon()));
				string globalStackProperty = globalStackIcon.GetAttribute("class");
				if (!globalStackProperty.Contains("toggle-disabled") && !globalStackProperty.Contains("isToolActive"))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step 3 - Click on Global Stack to turn ON.	
				bluringviewer.ChangeViewerLayout("2x2", 1);                
				bluringviewer.clickglobalstackIcon(1);
				BluRingViewer.WaitforViewports();
				ExecutedSteps++;

				IList<IWebElement> Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
				int toatlImageCount = 0;
				for (int count = 0; count < Thumbnail_list.Count; count++)
				{
					var mod = Thumbnail_list[count].FindElement(By.CssSelector(BluRingViewer.div_thumbnailModality)).Text;
					if (mod != "KO" && mod != "PR")
					{
						toatlImageCount = toatlImageCount + Int32.Parse(Thumbnail_list[count].FindElement(By.CssSelector(BluRingViewer.div_imageFrameNumber)).Text);
					}
				}
				Logger.Instance.InfoLog("total image count without KO and PR: " + toatlImageCount);				
				for (int count = 1; count < 5; count++)
				{
					var imageCount = bluringviewer.GetSliderMaxValue(studyPanelNum: 1, viewportNum: count);
					var sliderValue = bluringviewer.GetSliderValue(studyPanelNum: 1, viewportNum: count);
					bluringviewer.ClickOnViewPort(panelnumber: 1, viewportnumber: count);
					var isThumbnailActive = false;
					int[] sliderValueList = { 1, 1, 5, 9};
					if (count <= 2)
					{
						isThumbnailActive = BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 2);
						if (imageCount == toatlImageCount && sliderValue == sliderValueList[count-1] && isThumbnailActive)
						{
							result.steps[ExecutedSteps].AddPassStatusList("Viewport:" + count+", Thumbnail-2 isActive:" + isThumbnailActive + " Total Image count-" + imageCount + ",Slider value-"+ sliderValue);
						}
						else
						{
							result.steps[ExecutedSteps].AddFailStatusList("Viewport:" + count + ", Thumbnail-2 isActive:" + isThumbnailActive + " Total Image count-" + imageCount + ",Slider value-" + sliderValue);
						}

						//if (!BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 3))
						//{
						//	result.steps[ExecutedSteps].AddFailStatusList("Thumbnail 3 is not active");
						//}
					}
					else 
					{					
						isThumbnailActive = BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, count);						
						if (imageCount == toatlImageCount && sliderValue == sliderValueList[count - 1] && isThumbnailActive)
						{
							result.steps[ExecutedSteps].AddPassStatusList("Viewport:" + count + ", Thumbnail-" + count + " isActive:" + isThumbnailActive + " Total Image count-" + imageCount + ",Slider value-" + sliderValue+", Expected: "+ sliderValueList[count - 1]);
						}
						else
						{
							result.steps[ExecutedSteps].AddFailStatusList("Viewport:" + count + ", Thumbnail-" + count + " isActive:" + isThumbnailActive + " Total Image count-" + imageCount + ",Slider value-" + sliderValue + ", Expected: " + sliderValueList[count - 1]);
						}
					}					
				}
				if (result.steps[ExecutedSteps].statuslist.All(res => res == "Pass"))
				{
					result.steps[ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[ExecutedSteps].StepFail();
				}

				//Step 4- Scroll images (back and forth) in global stack mode and verify the highlighted thumbnails.
				bluringviewer.ClickOnViewPort(panelnumber: 1, viewportnumber: 1);
				IWebElement ele = bluringviewer.GetElement(BasePage.SelectorType.CssSelector, bluringviewer.Activeviewport);
				new TestCompleteAction().MouseScroll(ele, "down", "5").Perform();
				if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 3))
				{
					result.steps[++ExecutedSteps].AddPassStatusList("Thumbnail 3 is highlighted");
				}
				else
				{
					result.steps[++ExecutedSteps].AddFailStatusList("Thumbnail 4 is not highlighted");
				}

				ele = bluringviewer.GetElement(BasePage.SelectorType.CssSelector, bluringviewer.Activeviewport);
				new TestCompleteAction().MouseScroll(ele, "up", "4").Perform();
				if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 2))
				{
					result.steps[ExecutedSteps].AddPassStatusList("Thumbnail 2 is highlighted");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList("Thumbnail 2 is not highlighted");
				}

				if (result.steps[ExecutedSteps].statuslist.All(res => res == "Pass"))
				{
					result.steps[ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[ExecutedSteps].StepFail();
				}
				//Step 5- Verify the total # of images for Stack Slider.
				ExecutedSteps++; // Covered in Step 3

				//Step 6- Scroll about halfway through the stack. Turn Global Stack off to switch back to Series Stack Mode
				ele = bluringviewer.GetElement(BasePage.SelectorType.CssSelector, bluringviewer.Activeviewport);
				new TestCompleteAction().MouseScroll(ele, "down", "9").Perform();
				bluringviewer.clickglobalstackIcon(1);
				BluRingViewer.WaitforViewports();
				if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 4))
				{
					result.steps[++ExecutedSteps].AddPassStatusList("Thumbnail 4 is highlighted");
				}
				else
				{
					result.steps[++ExecutedSteps].AddFailStatusList("Thumbnail 4 is not highlighted");
				}
				var ImageCountValueAfterDisableGS = bluringviewer.GetSliderMaxValue(studyPanelNum: 1, viewportNum: 1);
				if (ImageCountValueAfterDisableGS == 4)
				{
					result.steps[ExecutedSteps].AddPassStatusList("Total Image count is 4");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList("Total Image count is not 4, ACtual: " + ImageCountValueAfterDisableGS);
				}
				var sliderValueAfterDisableGS = bluringviewer.GetSliderValue(studyPanelNum: 1, viewportNum: 1);
				if (sliderValueAfterDisableGS == 3)
				{
					result.steps[ExecutedSteps].AddPassStatusList("Image no: 3 is displayed in viewport 1");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList("Image no: 3 is not displayed in viewport 1, Actual: " + sliderValueAfterDisableGS);
				}				
				if (result.steps[ExecutedSteps].statuslist.All(res => res == "Pass"))
				{
					result.steps[ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[ExecutedSteps].StepFail();
				}

				//Step 7-Click on Global Stack to switch back to Global stack mode
				bluringviewer.clickglobalstackIcon(1);
				BluRingViewer.WaitforViewports();
				ExecutedSteps++;
                for (int count = 1; count < 5; count++)
				{
					var imageCount = bluringviewer.GetSliderMaxValue(studyPanelNum: 1, viewportNum: count);					
					if (count < 5)
					{
						if (imageCount == toatlImageCount)
						{
							result.steps[ExecutedSteps].AddPassStatusList("Viewport:" + count + ", Total Image count-" + imageCount);
						}
						else
						{
							result.steps[ExecutedSteps].AddFailStatusList("Viewport:" + count + ", Total Image count-" + imageCount);
						}
					}
				}
				var sliderValueAfterEnableGS = bluringviewer.GetSliderValue(studyPanelNum: 1, viewportNum: 1);
				if (sliderValueAfterEnableGS == 11)
				{
					result.steps[ExecutedSteps].AddPassStatusList("Image no: 1 is displayed in viewport 1");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList("Image no: 11 is not displayed in viewport 1, Actual: " + sliderValueAfterDisableGS);
				}
				if (result.steps[ExecutedSteps].statuslist.All(res => res == "Pass"))
				{
					result.steps[ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[ExecutedSteps].StepFail();
				}

				//Step 8 - While in Global Stack mode, change the viewport layout.
				bluringviewer.ClickOnViewPort(panelnumber: 1, viewportnumber: 1);
				bluringviewer.ChangeViewerLayout("1x2", 1);
                ExecutedSteps++;
                for (int count = 1; count < 3; count++)
				{
					var imageCount = bluringviewer.GetSliderMaxValue(studyPanelNum: 1, viewportNum: count);
					if (count < 3)
					{
						if (imageCount == toatlImageCount)
						{
							result.steps[ExecutedSteps].AddPassStatusList("Viewport:" + count + ", Total Image count-" + imageCount);
						}
						else
						{
							result.steps[ExecutedSteps].AddFailStatusList("Viewport:" + count + ", Total Image count-" + imageCount);
						}
					}
				}
				if (result.steps[ExecutedSteps].statuslist.All(res => res == "Pass"))
				{
					result.steps[ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[ExecutedSteps].StepFail();
				}

				//Step 9- While in Global Stack mode, drop and drag one of the images from thumbnail. Continue scrolling through the images.
				bluringviewer.DropAndDropThumbnails(thumbnailnumber: 5, viewport: 1, studyPanelNumber: 1, UseDragDrop: true);
				PageLoadWait.WaitForLoadInViewport(10, BasePage.FindElementByCss(bluringviewer.GetViewportCss(1, 0)));
				if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 5)) //Thumbnail highlight validation
				{
					result.steps[++ExecutedSteps].AddPassStatusList(logMessage: "5th thumbnail loaded in 1st viewport using drag and drop");
				}
				else
				{
					result.steps[++ExecutedSteps].AddFailStatusList(logMessage: "5th thumbnail not loaded in 1st viewport using drag and drop");
				}
				var sliderValue_step9 = bluringviewer.GetSliderValue(studyPanelNum: 1, viewportNum: 1); //ImageValidation
				if (sliderValue_step9 == 13)
				{
					result.steps[ExecutedSteps].AddPassStatusList("Image no: 13 is displayed in viewport 1");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList("Image no: 13 is not displayed in viewport 1, Actual: " + sliderValue_step9);
				}
				ele = bluringviewer.GetElement(BasePage.SelectorType.CssSelector, bluringviewer.Activeviewport);
				new TestCompleteAction().MouseScroll(ele, "up", "4").Perform();
				if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 4)) //Thumbnail highlight validation
				{
					result.steps[ExecutedSteps].AddPassStatusList(logMessage: "4th thumbnail highlighted in 1st viewport");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList(logMessage: "4th thumbnail not highlighted in 1st viewport");
				}
				var sliderValue_step9_2 = bluringviewer.GetSliderValue(studyPanelNum: 1, viewportNum: 1); //ImageValidation
				if (sliderValue_step9_2 == 9)
				{
					result.steps[ExecutedSteps].AddPassStatusList("Image no: 9 is displayed in viewport 1");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList("Image no:  9 is not displayed in viewport 1, Actual: " + sliderValue_step9_2);
				}
				if (result.steps[ExecutedSteps].statuslist.All(res => res == "Pass"))
				{
					result.steps[ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[ExecutedSteps].StepFail();
				}

				//Step 10- From the Studies tab > query and load a large study (~5000 images) with multiple series. Select the Global stack mode.
				bluringviewer.CloseBluRingViewer();
				studies.SearchStudy(patientID: PatientID, Datasource: login.GetHostName(Config.PACS2));
				studies.SelectStudy("Patient ID", PatientID);  // MRNPAX36744
				bluringviewer = BluRingViewer.LaunchBluRingViewer();				
				bluringviewer.ChangeViewerLayout("2x2");
				bluringviewer.clickglobalstackIcon(1);
				BluRingViewer.WaitforViewports(300);				
				Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
                toatlImageCount = int.Parse(totalImagesInGS);              
				Logger.Instance.InfoLog("total image count without KO and PR: " + toatlImageCount);
				ExecutedSteps++;
				for (int count = 1; count < 5; count++)
				{
					var imageCount = bluringviewer.GetSliderMaxValue(studyPanelNum: 1, viewportNum: count);
					if (count < 5)
					{
						if (imageCount == toatlImageCount)
						{
							result.steps[ExecutedSteps].AddPassStatusList("Viewport:" + count + ", Total Image count-" + imageCount);
						}
						else
						{
							result.steps[ExecutedSteps].AddFailStatusList("Viewport:" + count + ", Total Image count-" + imageCount);
						}
					}
				}
				if (result.steps[ExecutedSteps].statuslist.All(res => res == "Pass"))
				{
					result.steps[ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[ExecutedSteps].StepFail();
				}

				//Step 11- Scroll images in global stack and verify the highlighted thumbnails. 
				ele = bluringviewer.GetElement(BasePage.SelectorType.CssSelector, bluringviewer.Activeviewport);
				new TestCompleteAction().MouseScroll(ele, "down", "25").Perform();
				if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 2)) //Thumbnail highlight validation
				{
					result.steps[++ExecutedSteps].AddPassStatusList(logMessage: "2nd thumbnail highlighted in 1st viewport");
				}
				else
				{
					result.steps[++ExecutedSteps].AddFailStatusList(logMessage: "2nd thumbnail not highlighted in 1st viewport");
				}

				//Apply applicable tools(Localizer lines, WL, zoom, pan, flip horizontal / vertical, invert, rotate, reset) 				
				bluringviewer.ClickOnViewPort(1, 1);
				bluringviewer.SelectViewerTool(BluRingTools.Window_Level);
				bluringviewer.ApplyTool_WindowWidth();
				bluringviewer.SelectViewerTool(BluRingTools.Pan);
				bluringviewer.ApplyTool_Pan();
				bluringviewer.SelectViewerTool(BluRingTools.Interactive_Zoom);
				bluringviewer.ApplyTool_Zoom();
				bluringviewer.SelectViewerTool(BluRingTools.Flip_Horizontal);
				bluringviewer.SelectViewerTool(BluRingTools.Invert);
				bluringviewer.SelectViewerTool(BluRingTools.Rotate_Clockwise);
				BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_LocalizerLinesIcon)).Click();				
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
				bool Step11_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.studyPanel(studyPanelIndex:1));				
				bluringviewer.SelectViewerTool(BluRingTools.Reset);
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
				bool Step11_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.studyPanel(studyPanelIndex: 1));
				Logger.Instance.InfoLog("Step 11: Apply tool result:" + Step11_1 + ", Reset tool result:" + Step11_2);

				//while in Global Stack mode.Note: Linked Scrolling option is greyed out when in Global Stack mode.
				BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_LocalizerLinesIcon)).Click(); // disable LocalizerLinesIcon
				IWebElement linkedScrollingIcon = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_LinkedScrollingsIcon));
				linkedScrollingIcon.Click();
				BluRingViewer.WaitforViewports();
				globalStackIcon = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_globalstackicon()));
				if (Step11_1 && Step11_2 && globalStackIcon.GetAttribute("class").Contains("toggle-disabled"))
				{
					result.steps[ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[ExecutedSteps].StepFail();
				}				
				linkedScrollingIcon.Click(); // disable linked scrolling
				BluRingViewer.WaitforViewports();

				//Step 12 - While in Global Stack mode, scroll through to the second series in the stack.
				/*
				bluringviewer.ClickOnViewPort(panelnumber: 1, viewportnumber: 1);
				globalStackIcon = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_globalstackicon()));
                globalStackProperty = globalStackIcon.GetAttribute("class");
                ExecutedSteps++;
				if (globalStackProperty.Contains("toggle-disabled"))
				{
					result.steps[ExecutedSteps].StepFail("Gloabl stack tool is disabled after disabling Linked scrolling");

				}
				else if(globalStackProperty.Contains("isToolActive"))
				{
					result.steps[ExecutedSteps].StepFail("Gloabl stack tool is active after disabling Linked scrolling");
				}
				else
				{
					*/
					bluringviewer.CloseBluRingViewer();
					ExecutedSteps++;
					studies.SearchStudy(patientID: PatientID, Datasource: login.GetHostName(Config.PACS2));
					studies.SelectStudy("Patient ID", PatientID);  // MRNPAX36744
					bluringviewer = BluRingViewer.LaunchBluRingViewer();
					bluringviewer.ChangeViewerLayout("2x2");

					bluringviewer.clickglobalstackIcon(1); // enable global stack
					BluRingViewer.WaitforViewports();
					ele = bluringviewer.GetElement(BasePage.SelectorType.CssSelector, bluringviewer.Activeviewport);
					new TestCompleteAction().MouseScroll(ele, "down", "25").Perform();

					//Switch back to the Series Stack Mode.				
					bluringviewer.ClickOnViewPort(1, 1);
					bluringviewer.clickglobalstackIcon(1);
					BluRingViewer.WaitforViewports();
					globalStackIcon = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_globalstackicon()));

					//Apply applicable tools(Localizer lines, WL, zoom, pan, flip horizontal / vertical, invert, rotate, reset).
					bluringviewer.SelectViewerTool(BluRingTools.Window_Level);
					bluringviewer.ApplyTool_WindowWidth();
					bluringviewer.SelectViewerTool(BluRingTools.Pan);
					bluringviewer.ApplyTool_Pan();
					bluringviewer.SelectViewerTool(BluRingTools.Interactive_Zoom);
					bluringviewer.ApplyTool_Zoom();
					bluringviewer.SelectViewerTool(BluRingTools.Flip_Horizontal);
					bluringviewer.SelectViewerTool(BluRingTools.Invert);
					bluringviewer.SelectViewerTool(BluRingTools.Rotate_Clockwise);
					BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_LocalizerLinesIcon)).Click();
					result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
					bool Step12_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.studyPanel(studyPanelIndex: 1));
					bluringviewer.SelectViewerTool(BluRingTools.Reset);
					result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
					bool Step12_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.studyPanel(studyPanelIndex: 1));
					Logger.Instance.InfoLog("Step 12: Apply tool result:" + Step12_1 + ", Reset tool result:" + Step12_2);
					if (Step12_1 && Step12_2 && !globalStackIcon.GetAttribute("class").Contains("isToolActive"))
					{
						result.steps[ExecutedSteps].StepPass();
					}
					else
					{
						result.steps[ExecutedSteps].StepFail();
					}
				//}
				//Step 13: Apply Global Stack mode to all supported modalities
				ExecutedSteps++; // already covered multiple modalities in this test case
                bluringviewer.CloseBluRingViewer();

				//Step 14: Load a study with KO's. Verify Global Stack mode cannot be applied to KO // ACC: 90e8a66a5
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(AccessionNo: Accession[1], Datasource: login.GetHostName(Config.EA1));
				studies.SelectStudy("Accession", Accession[1]);
				bluringviewer = BluRingViewer.LaunchBluRingViewer();
				bluringviewer.ChangeViewerLayout("1x2");
				bluringviewer.OpenExamListThumbnailPreview(prior: 0);
				bool KOThumbnail_BeforeGolbalStack = BluRingViewer.VerifyThumbnailsInExamList(1, "active");
				Logger.Instance.InfoLog("Step 14: KOThumbnail_BeforeGolbalStack is active" + KOThumbnail_BeforeGolbalStack);
				bluringviewer.clickglobalstackIcon(1);
				BluRingViewer.WaitforViewports();
				bool KOThumbnail_AfterGolbalStack = BluRingViewer.VerifyThumbnailsInExamList(1, "no border");
				Logger.Instance.InfoLog("Step 14: KOThumbnail_BeforeGolbalStack has no border" + KOThumbnail_AfterGolbalStack);
                Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
                toatlImageCount = 0;
                for (int count = 0; count < Thumbnail_list.Count; count++)
				{
					var mod = Thumbnail_list[count].FindElement(By.CssSelector(BluRingViewer.div_thumbnailModality)).Text;
					if (mod != "KO" && mod != "PR")
					{
						toatlImageCount = toatlImageCount + Int32.Parse(Thumbnail_list[count].FindElement(By.CssSelector(BluRingViewer.div_imageFrameNumber)).Text);
					}
				}
				Logger.Instance.InfoLog("Step 14: total image count without KO and PR: " + toatlImageCount);
				ExecutedSteps++;
				for (int count = 1; count < 3; count++)
				{
					var imageCount = bluringviewer.GetSliderMaxValue(studyPanelNum: 1, viewportNum: count);				
					if (imageCount == toatlImageCount)
					{
						result.steps[ExecutedSteps].AddPassStatusList("Viewport:" + count + ", Total Image count-" + imageCount);
					}
					else
					{
						result.steps[ExecutedSteps].AddFailStatusList("Viewport:" + count + ", Total Image count-" + imageCount);
					}					
				}

				if (result.steps[ExecutedSteps].statuslist.All(res => res == "Pass") && KOThumbnail_BeforeGolbalStack && KOThumbnail_AfterGolbalStack)
				{
					result.steps[ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[ExecutedSteps].StepFail();
				}
                bluringviewer.CloseBluRingViewer();

                //Step 15: Load a study with PR's. Verify Global Stack mode cannot be applied to PR's // ACC: 11643936
                //studies.SearchStudy(AccessionNo: Accession[2], Datasource: login.GetHostName(Config.EA1));
				//studies.SelectStudy("Accession", Accession[2]);				
				studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA1));
				studies.SelectStudy("Accession", Accession[0]);
				bluringviewer = BluRingViewer.LaunchBluRingViewer();
				bluringviewer.OpenExamListThumbnailPreview(accession: Accession[0]);
				var PRThumbnail_BeforeGolbalStack = BluRingViewer.VerifyThumbnailsInExamList(1, "active");
				Logger.Instance.InfoLog("Step 15: KOThumbnail_BeforeGolbalStack is active" + PRThumbnail_BeforeGolbalStack);
				bluringviewer.clickglobalstackIcon(1);
				BluRingViewer.WaitforViewports();
				var PRThumbnail_AfterGolbalStack = BluRingViewer.VerifyThumbnailsInExamList(1, "no border");
				Logger.Instance.InfoLog("Step 15: KOThumbnail_BeforeGolbalStack has no border" + PRThumbnail_AfterGolbalStack);
                Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
                toatlImageCount = 0;
                for (int count = 0; count < Thumbnail_list.Count; count++)
				{
					var mod = Thumbnail_list[count].FindElement(By.CssSelector(BluRingViewer.div_thumbnailModality)).Text;
					if (mod != "KO" && mod != "PR")
					{
						toatlImageCount = toatlImageCount + Int32.Parse(Thumbnail_list[count].FindElement(By.CssSelector(BluRingViewer.div_imageFrameNumber)).Text);
					}
				}
				Logger.Instance.InfoLog("Step 15: total image count without KO and PR: " + toatlImageCount);
				ExecutedSteps++;
				for (int count = 1; count < 3; count++)
				{
					var imageCount = bluringviewer.GetSliderMaxValue(studyPanelNum: 1, viewportNum: count);
					if (imageCount == toatlImageCount)
					{
						result.steps[ExecutedSteps].AddPassStatusList("Viewport:" + count + ", Total Image count-" + imageCount);
					}
					else
					{
						result.steps[ExecutedSteps].AddFailStatusList("Viewport:" + count + ", Total Image count-" + imageCount);
					}
				}

				if (result.steps[ExecutedSteps].statuslist.All(res => res == "Pass") && PRThumbnail_BeforeGolbalStack && PRThumbnail_AfterGolbalStack)
				{
					result.steps[ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[ExecutedSteps].StepFail();
				}
				bluringviewer.CloseBluRingViewer();

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
        /// Global Stack mode behaviour in Cine Play
        /// </summary>
        public TestCaseResult Test_169468(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');
                BluRingViewer bluRingViewer = new BluRingViewer();
                StudyViewer viewer = new StudyViewer();
                //Step 1 - Precondion
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(20);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userPreferences.ModalityDropDown().SelectByText("CR");
                userpref.ExamMode_OFF().Click();
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                login.Logout();
                ExecutedSteps++;

                //Step 2 - Login to iCA. Load a study with multiple series and PR's or KO's. Click Group Play(Cine Play All).
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(20);
                studies.SearchStudy(patientID: Accession[1], Datasource: EA_131);
                studies.SelectStudy("Patient ID", Accession[1]);
                bluRingViewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(10);
                IWebElement PlayCine = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_CINE_PlayAllBtn));
                PlayCine.Click();
                IWebElement PauseCine = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_CINE_PauseAllBtn));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(PauseCine));
                Thread.Sleep(2000);
                String GlobalIconVerify = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_GlobaliconVerify)).GetAttribute("className");
                if (bluRingViewer.IsCINEPlaying(2, 1) && GlobalIconVerify.Contains("button-toggle-disabled"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 3 - Pause Group Play.
                PauseCine.Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(PlayCine));
                Thread.Sleep(2000);
                String GlobalIconVerify_2 = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_GlobaliconVerify)).GetAttribute("className");
                if (!bluRingViewer.IsCINEPlaying(2, 1) && !GlobalIconVerify_2.Contains("button-toggle-disabled"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 4 - Select any viewport then apply Global Stack and ensure all the series are loaded in the selected viewport without PR and KO serie                
                bluRingViewer.ClickOnViewPort(panelnumber: 1, viewportnumber: 1);
                bluRingViewer.clickglobalstackIcon(1);
                Thread.Sleep(4000);
                String TestKOandPRImage = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ActiveThumbnail)).GetAttribute("title");
                if (TestKOandPRImage.Contains("KO") == false && TestKOandPRImage.Contains("PR") == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 5 - Scroll images in the viewport where global stack is applied and verify that the respective series are highlighted in thumbnails
                IWebElement ScrollViewport = bluRingViewer.GetElement(BasePage.SelectorType.CssSelector, bluRingViewer.Activeviewport);
                //IWebElement ThumbnailSeriesValue = Driver.FindElement(By.CssSelector(BluRingViewer.div_ThumbnailSeriesValue));
                String Step_1Verify = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ThumbnailSeriesValue)).Text;
                new TestCompleteAction().MouseScroll(ScrollViewport, "down", "300").Perform();
                String Step_2Verify = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ThumbnailSeriesValue)).Text;
                new TestCompleteAction().MouseScroll(ScrollViewport, "down", "200").Perform();
                String Step_3Verify = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ThumbnailSeriesValue)).Text;
                String Previous = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_PlayPrevSeriesVerify)).GetAttribute("className");
                if (Previous.Contains("disabled") == true && Step_1Verify != Step_2Verify && Step_1Verify != Step_3Verify && Step_2Verify != Step_3Verify)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                bluRingViewer.CloseBluRingViewer();

                //Step 6 - Load another study. Apply Global Stack mode.
                studies.SearchStudy(patientID: Accession[1], Datasource: EA_131);
                studies.SelectStudy("Patient ID", Accession[1]);
                bluRingViewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(10);
                bluRingViewer.clickglobalstackIcon(1);
                Thread.Sleep(6000);
                String Play = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_PlayBtnVerify)).GetAttribute("className");
                if (Play.Contains("disabled"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 7 - Switch back to Series stack mode from Global Stack (click on Global Stack icon).
                bluRingViewer.clickglobalstackIcon(1);
                Thread.Sleep(6000);
                String Play_1 = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_PlayBtnVerify)).GetAttribute("className");
                if (!Play_1.Contains("disabled"))
                {
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
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }


        /// <summary>
        /// Displayed image retained when switching to Global Stack mode
        /// </summary>
        public TestCaseResult Test_169902(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');
                BluRingViewer bluRingViewer = new BluRingViewer();

                //Step 1 - Precondion
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(20);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userPreferences.ModalityDropDown().SelectByText("CR");
                userpref.ExamMode_OFF().Click();
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                login.Logout();
                ExecutedSteps++;

                //Step 2 - Login to ICA with any privileged user. From the Studies tab > query and load a study with multiple series.
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(20);
                studies.SearchStudy(AccessionNo: Accession[1], Datasource: EA_131);
                studies.SelectStudy("Accession", Accession[1]);
                bluRingViewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(3000);
                String GlobalIconVerify = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_GlobaliconVerify)).GetAttribute("className");
                String GlobalStackIconActive = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_GlobalStackIconActive)).GetAttribute("className");
                if (!GlobalIconVerify.ToLower().Contains("button-toggle-disabled") && !GlobalStackIconActive.ToLower().Contains("toolactive"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 3 - For series 1, scroll half way through - note the image #.Apply Global Stack mode
                bluRingViewer.ClickOnViewPort(panelnumber: 1, viewportnumber: 1);
                IWebElement ScrollViewport = bluRingViewer.GetElement(BasePage.SelectorType.CssSelector, bluRingViewer.Activeviewport);
                new TestCompleteAction().MouseScroll(ScrollViewport, "down", "4").Perform();
                String ImageNumber_1 = BasePage.Driver.FindElement(By.XPath(BluRingViewer.div_ScrollImgCount)).Text;
                bluRingViewer.clickglobalstackIcon(1);
                Thread.Sleep(3000);
                String ImageNumber_2 = BasePage.Driver.FindElement(By.XPath(BluRingViewer.div_ScrollImgCount)).Text;
                if (ImageNumber_1 == ImageNumber_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 4 - Scroll further down within the same series - note the image #.Switch back to Series stack mode.
                new TestCompleteAction().MouseScroll(ScrollViewport, "down", "4").Perform();
                String ImageNumber_3 = BasePage.Driver.FindElement(By.XPath(BluRingViewer.div_ScrollImgCount)).Text;
                bluRingViewer.clickglobalstackIcon(1);
                Thread.Sleep(3000);
                String ImageNumber_4 = BasePage.Driver.FindElement(By.XPath(BluRingViewer.div_ScrollImgCount)).Text;
                if (ImageNumber_3 == ImageNumber_4)
                {
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
