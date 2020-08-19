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
using OpenQA.Selenium.Interactions;

namespace Selenium.Scripts.Tests
{
    class StudyPanel
    {
        public Login login { get; set; }
        public string filepath { get; set; }

        public StudyPanel(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }


        /// <summary>
        /// This is case to  Verify the Study Panel Title section in the BlueRing viewer.
        /// </summary>
        public TestCaseResult Test_161030(String testid, String teststeps, int stepcount)
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
                String expPriorCountList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount");
                String lastNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastNameList");
                String firstNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstNameList");
                String patientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String[] Accession = AccessionList.Split(':');
                String[] LastName = lastNameList.Split(':');
                String[] FirstName = firstNameList.Split(':');
                String[] PatientId = patientIDList.Split(':');
                String[] ExpPriorCount = expPriorCountList.Split(':');
                String studyDescriptionFontSize = "12px";
                String studyDateFontSize = "16px";
                String studyTimeFontSize = "16px";
                String StudydatefontsizeWithoutDate = "9px";

                // Font sizes for screen resolution 1600x900
                //String studyDescriptionFontSize = "13.2px";
                //String studyDateFontSize = "17.6px";
                //String studyTimeFontSize = "13.2px";

                if (BasePage.SBrowserName.Contains("explorer"))
                {
                    studyDescriptionFontSize = "12px";
                    studyDateFontSize = "16px";
                    studyTimeFontSize = "16px";
                    StudydatefontsizeWithoutDate = "8.86px";
                }
                else if (BasePage.SBrowserName.ToLower().Contains("edge"))
                {
                    studyDescriptionFontSize = "13.6px";
                    studyDateFontSize = "18.13px";
                    studyTimeFontSize = "13.6px";
                }

                //Step 1 Launch and Login as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                if (login.IsTabPresent("Studies") && login.UserId().Text.Equals(adminUserName))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step2  Select a study, and launch it in BluRingViewer and verify the study_date,study_time,study_description font sizes
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);

                var priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                var priorCount = priors.Count;

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step2_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                Logger.Instance.InfoLog("Result of Step2_1 : " + step2_1);

                var studyDateFontWeight = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypaneldate).GetCssValue("font-weight");
                IList<IWebElement> studyDate = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypaneldate));
                var studyDateFontSizeActual = viewer.getFontSizeofElements(studyDate)[0];
                bool step2_2 = studyDateFontSizeActual.Equals(studyDateFontSize);
                Logger.Instance.InfoLog("Actual : " + studyDateFontSizeActual);
                Logger.Instance.InfoLog("Result of Step2_2 : " + step2_2);

                IList<IWebElement> studyTime = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypaneltime));
                var studyTimeFontActual = viewer.getFontSizeofElements(studyTime)[0];
                bool step2_3 = studyTimeFontActual.Equals(studyTimeFontSize);
                Logger.Instance.InfoLog("Actual : " + studyTimeFontActual);
                //Logger.Instance.InfoLog("Result of Step2_3 : " + step2_3);


                IList<IWebElement> studyDescription = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypaneldescription));
                var studyDescriptionFontActual = viewer.getFontSizeofElements(studyDescription)[0];
                bool step2_4 = studyDescriptionFontActual.Equals(studyDescriptionFontSize);
                Logger.Instance.InfoLog("Actual : " + studyDescriptionFontActual);
                Logger.Instance.InfoLog("Result of Step2_4 : " + step2_4);

                bool step2_5 = priorCount.Equals(Int32.Parse(ExpPriorCount[0]));
                Logger.Instance.InfoLog("Result of Step2_5 : " + step2_5);

                bool step2_6 = studyDateFontWeight.Equals("700") || studyDateFontWeight.Equals("bold");
                Logger.Instance.InfoLog("Result of Step2_6 : " + step2_6);

                if (step2_1 && step2_2 && step2_3 && step2_4 && step2_5 && step2_6)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step3  Select another study from the Exam List and verify the study_date,study_time,study_description font sizes
                viewer.OpenPriors(2);
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step3_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                Logger.Instance.InfoLog("Result of Step3_1 : " + step3_1);

                studyDateFontWeight = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel +
                    ":nth-of-type(2) " + BluRingViewer.div_studypaneldate).GetCssValue("font-weight");
                bool step3_2 = studyDateFontWeight.Equals("700") || studyDateFontWeight.Equals("bold");
                Logger.Instance.InfoLog("Result of Step3_2 : " + step3_2);

                studyDate = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypaneldate));
                bool step3_3 = viewer.getFontSizeofElements(studyDate)[0] == studyDateFontSize;
                Logger.Instance.InfoLog("Result of Step3_3 : " + step3_3);

                studyTime = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel +
                    ":nth-of-type(2) " + BluRingViewer.div_studypaneltime));
                bool step3_4 = viewer.getFontSizeofElements(studyTime)[0] == studyTimeFontSize;
                Logger.Instance.InfoLog("Result of Step3_4 : " + step3_4);

                studyDescription = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel +
                    ":nth-of-type(2) " + BluRingViewer.div_studypaneldescription));
                bool step3_5 = viewer.getFontSizeofElements(studyDescription)[0] == studyDescriptionFontSize;
                Logger.Instance.InfoLog("Result of Step3_5 : " + step3_5);

                if (step3_1 && step3_2 && step3_3 && step3_4 && step3_5)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step4  Close the current study by clicking on X (EXIT) 
                viewer.CloseBluRingViewer();
                if (login.IsTabPresent("Studies"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step5  Search for a study without a study_description and open it in the BlueRing viewer and verify the study_date,study_time font sizes
                studies.ClearFields();
                studies.SearchStudy(LastName: LastName[0], FirstName: FirstName[0], Datasource: login.GetHostName(Config.SanityPACS));
                studies.SelectStudy("Patient ID", PatientId[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step5_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                Logger.Instance.InfoLog("Result of Step5_1 : " + step5_1);

                priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                bool step5_2 = priors.Count.Equals(Int32.Parse(ExpPriorCount[1]));
                Logger.Instance.InfoLog("Result of Step5_2 : " + step5_2);

                studyDateFontWeight = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypaneldate).GetCssValue("font-weight");
                bool step5_3 = studyDateFontWeight.Equals("700") || studyDateFontWeight.Equals("bold");
                Logger.Instance.InfoLog("Result of Step5_3 : " + step5_3);

                studyDate = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypaneldate));
                bool step5_4 = viewer.getFontSizeofElements(studyDate)[0] == studyDateFontSize;
                Logger.Instance.InfoLog("Result of Step5_4 : " + step5_4);

                studyTime = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypaneltime));
                bool step5_5 = viewer.getFontSizeofElements(studyTime)[0] == studyTimeFontSize;
                Logger.Instance.InfoLog("Result of Step5_5 : " + step5_5);

                bool step5_6 = viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_studypaneldescription));
                Logger.Instance.InfoLog("Result of Step5_6 : " + !step5_6);

                if (step5_1 && step5_2 && step5_3 && step5_4 && step5_5 && !step5_6)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step6 Close the current study by clicking on X (EXIT) 
                viewer.CloseBluRingViewer();
                if (login.IsTabPresent("Studies"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step7  Search for a study without a study_time and open it in the BlueRing viewer and verify the study_date,study_description font sizes
                studies.ClearFields();
                IWebElement ele = BasePage.Driver.FindElement(By.Id("m_studySearchControl_m_clearButton"));
                new Actions(BasePage.Driver).MoveToElement(ele).Click().Build().Perform();
                studies.SearchStudy(patientID: PatientId[1], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Patient ID", PatientId[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step7_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                Logger.Instance.InfoLog("Result of Step7_1 : " + step7_1);

                priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                bool step7_2 = priors.Count.Equals(Int32.Parse(ExpPriorCount[2]));
                Logger.Instance.InfoLog("Result of Step7_2 : " + step7_2);

                studyDateFontWeight = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypaneldate).GetCssValue("font-weight");
                bool step7_3 = studyDateFontWeight.Equals("700") || studyDateFontWeight.Equals("bold");
                Logger.Instance.InfoLog("Result of Step7_3 : " + step7_3);

                studyDate = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypaneldate));
                var Studydatefontsize3 = viewer.getFontSizeofElements(studyDate);
                bool step7_4 = viewer.getFontSizeofElements(studyDate)[0] == studyDateFontSize;
                Logger.Instance.InfoLog("Result of Step7_4 : " + step7_4);
                Logger.Instance.InfoLog("Result of Step7_4 : " + Studydatefontsize3[0]);

                bool step7_5 = studyDate[0].GetAttribute("innerHTML").Equals("14-Sep-2014");
                Logger.Instance.InfoLog("Result of Step7_5 : " + step7_5);

                studyDescription = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypaneldescription));
                var Studydescriptionfontsize3 = viewer.getFontSizeofElements(studyDescription);
                bool step7_6 = viewer.getFontSizeofElements(studyDescription)[0] == studyDescriptionFontSize;
                Logger.Instance.InfoLog("Result of Step7_6 : " + step7_6);
                Logger.Instance.InfoLog("Result of Step7_6 : " + Studydescriptionfontsize3[0]);

                if (step7_1 && step7_2 && step7_3 && step7_4 && step7_5 && step7_6)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step8  Close the current study by clicking on X (EXIT) 
                viewer.CloseBluRingViewer();
                if (login.IsTabPresent("Studies"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // step9  Search for a study without a study_date and open it in the BlueRing viewer and verify the study_time, study_description font sizes
                studies.ClearFields();
                studies.SearchStudy(AccessionNo: Accession[1], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[1]);
                BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step9_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                Logger.Instance.InfoLog("Result of Step9_1 : " + step9_1);

                priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                bool step9_2 = priors.Count.Equals(Int32.Parse(ExpPriorCount[3]));
                Logger.Instance.InfoLog("Result of Step9_2 : " + step9_2);

                bool step9_3 = viewer.GetElement(BasePage.SelectorType.CssSelector, "div[class*='panel']>div:nth-of-type(1)").GetAttribute("innerHTML").Trim().ToLower().Contains("unknown");
                Logger.Instance.InfoLog("Result of Step9_3 : " + step9_3);

                studyDateFontWeight = viewer.GetElement(BasePage.SelectorType.CssSelector, "div[class*='panel']>div:nth-of-type(1)").GetCssValue("font-weight");
                bool step9_4 = studyDateFontWeight.Equals("400") || studyDateFontWeight.Equals("bold");
                Logger.Instance.InfoLog("Result of Step9_4 : " + step9_4);

                studyDate = BasePage.Driver.FindElements(By.CssSelector("div[class*='panel']>div:nth-of-type(1)"));
                var studyDateFontSizeActual1 = viewer.getFontSizeofElements(studyDate)[0];
                bool step9_5 = studyDateFontSizeActual1.Equals(StudydatefontsizeWithoutDate);
                Logger.Instance.InfoLog("Actual : " + studyDateFontSizeActual1);
                Logger.Instance.InfoLog("Result of Step9_5 : " + step9_5);


                studyDescription = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypaneldescription));
                var studyDescriptionFontSize1 = viewer.getFontSizeofElements(studyDescription)[0];
                var step9_6 = studyDescriptionFontSize1.Equals(studyDescriptionFontSize);
                Logger.Instance.InfoLog("Actual : " + studyDescriptionFontSize1);
                Logger.Instance.InfoLog("Result of Step9_6 : " + step9_6);


                if (step9_1 && step9_2 && step9_3 && step9_4 && step9_5 && step9_6)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Logout Application
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
        /// Study Panel Toolbar tools
        /// </summary>
        public TestCaseResult Test_164658(String testid, String teststeps, int stepcount)
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
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");


                //Step1 - Login iCA and load any study into the Universal viewer
                login.LoginIConnect(adminUserName, adminPassword);                
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Patient ID", PatientID);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                int Globalstackposition = viewer.GetElement(BasePage.SelectorType.CssSelector, "div.globalStackMode-toolbar").Location.X;
                int LayoutIconLocation = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_LayoutIcon).Location.X;
                IList<IWebElement> elements = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_separator));
                int FirstSeperatorLocation = elements[1].Location.X;
                int SecondSeperatorLocation = elements[2].Location.X;
                int PrevSeriesBtnLocation = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_CINE_PlayPrevSeriesBtn).Location.X;
                int PlayAllBtnLocation = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_CINE_PlayAllBtn).Location.X;
                int NxtSeriesBtnLocation  = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_CINE_PlayNextSeriesBtn).Location.X;
                int EmailStudyIconLocation = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_emailstudy).Location.X;
                if((LayoutIconLocation<Globalstackposition) && (Globalstackposition<FirstSeperatorLocation) && (FirstSeperatorLocation<PrevSeriesBtnLocation)
                    && (PrevSeriesBtnLocation<PlayAllBtnLocation) && (PlayAllBtnLocation<NxtSeriesBtnLocation) && (NxtSeriesBtnLocation<SecondSeperatorLocation)
                    && (SecondSeperatorLocation<EmailStudyIconLocation))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step2 - Check for Layout selector and Email Study tools should be displayed In Study panel toolbar.
                bool step2_1 = viewer.IsElementVisibleInUI(By.CssSelector("div.studyToolbar " + BluRingViewer.div_emailstudy));
                bool step2_2 = viewer.IsElementVisibleInUI(By.CssSelector("div.studyToolbar " + BluRingViewer.div_LayoutIcon));              
                if (step2_1 && step2_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step3 - Verify the separator is displayed next to the Email Study icon in the study panel toolbar.
                bool step3_1 = false;
                if (elements[2].Size.Height > 0 && elements[2].Size.Width > 0)
                {
                    step3_1 = true;
                }
                if (step3_1 && SecondSeperatorLocation < EmailStudyIconLocation)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step4 open up 2 more panels and try each tool from the opened panel.
                viewer.OpenPriors(0);
                viewer.OpenPriors(0);
                ExecutedSteps++;
                bool step4 = false;
                for (int i = 1; i < 4; i++)
                {
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps+1,i);
                    step4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, "blu-ring-study-panel-control:nth-of-type(" + i + ") div.studyViewerTitleBarContainer"));
                }
                int Layout = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_LayoutIcon)).Location.X;
                int Hamburger = BasePage.Driver.FindElement(By.CssSelector("blu-ring-study-panel-control:nth-of-type(3) button.viewerMenuButton")).Location.X;
                if (step4 && (Layout<Hamburger))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step5 Expanding the hamburger menu.
                IWebElement element = viewer.GetElement(BasePage.SelectorType.CssSelector, "blu-ring-study-panel-control:nth-of-type(3) button.viewerMenuButton");
                viewer.ClickElement(element);
                Globalstackposition = viewer.GetElement(BasePage.SelectorType.CssSelector, "div.globalStackMode-toolbar").Location.Y;
                PrevSeriesBtnLocation = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_CINE_PlayPrevSeriesBtn).Location.Y;
                PlayAllBtnLocation = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_CINE_PlayAllBtn).Location.Y;
                NxtSeriesBtnLocation = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_CINE_PlayNextSeriesBtn).Location.Y;
                EmailStudyIconLocation = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_emailstudy).Location.Y;
                if ((Globalstackposition < PrevSeriesBtnLocation) && (PrevSeriesBtnLocation < PlayAllBtnLocation)
                    && (PlayAllBtnLocation < NxtSeriesBtnLocation) && (NxtSeriesBtnLocation < EmailStudyIconLocation))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Logout Application
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
