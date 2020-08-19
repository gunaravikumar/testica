using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Reusable.Generic;
using System.IO;
using OpenQA.Selenium;
using Selenium.Scripts.Pages.MergeServiceTool;
using OpenQA.Selenium.Interactions;
using System.Threading;
using OpenQA.Selenium.Remote;

namespace Selenium.Scripts.Tests
{
    class ForeignSeries 
    {
        Login login { get; set; }
        public string filepath { get; set; }
        public ServiceTool servicetool { get; set; }
        public WpfObjects wpfobject { get; set; }
        public BasePage basepage { get; set; }
        public string[] datasource = null;
        public string EA_91 = null;
        public string EA_131 = null;
        public string PACS_A7 = null;
        public string EA_77 = null;

        public ForeignSeries(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            wpfobject = new WpfObjects();
            servicetool = new ServiceTool();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            EA_91 = login.GetHostName(Config.EA91);
            EA_77 = login.GetHostName(Config.EA77);
            EA_131 = login.GetHostName(Config.EA1);
            PACS_A7 = login.GetHostName(Config.SanityPACS);
            datasource = new string[] { EA_77, EA_91, PACS_A7 };
        }


        /// <summary>
        /// Foreign Series:Drag-and-drop thumbnails between study panels
        /// </summary>
        public TestCaseResult Test_161683(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            BasePage basepage = new BasePage();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                String username = Config.adminUserName;
                String password = Config.adminPassword;

                String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = AccessionNoList.Split(':');

                String DateList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DateList");
                String[] Date = DateList.Split(':');

                ServiceTool st = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();

                //Pre-conditions
                String TestDomain1 = BasePage.GetUniqueDomainID("TestDomain1_138696_");
                String Role1 = BasePage.GetUniqueRole("Role1_138696_");
                String PhysicianRole = BasePage.GetUniqueRole("PhysicianRole_138696_");
                String rad1 = BasePage.GetUniqueUserId("rad1_138696_");

                DomainManagement domain = new DomainManagement();
                RoleManagement role = new RoleManagement();
                UserManagement user = new UserManagement();
                Studies study = new Studies();
                UserPreferences userpref = new UserPreferences();
                BluRingViewer viewer = new BluRingViewer();

                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.CreateDomain(TestDomain1, Role1, datasources: datasource);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(10);
                domain.ClickSaveDomain();
                PageLoadWait.WaitForPageLoad(30);
                role = (RoleManagement)login.Navigate("RoleManagement");
                if (!role.RoleExists(PhysicianRole))
                {
                    role.CreateRole(TestDomain1, PhysicianRole, "physician");
                }
                user = (UserManagement)login.Navigate("UserManagement");
                user.CreateUser(rad1, TestDomain1, PhysicianRole, 1, Config.emailid, 1, rad1);
                login.Logout();
                //Step-1
                //Login to BlueRing application with any privileged user (rad1/rad1)
                login.DriverGoTo(login.url);
                login.LoginIConnect(rad1, rad1);
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.BluringViewerRadioBtn().Click();
                userpref.ModalityDropdown().SelectByText("MR");
                userpref.ThumbnailSplittingSeriesRadioBtn().Click();
                userpref.ViewingScopeSeriesRadioBtn().Click();
                userpref.ModalityDropdown().SelectByText("CT");
                userpref.ThumbnailSplittingSeriesRadioBtn().Click();
                userpref.ViewingScopeSeriesRadioBtn().Click();
                userpref.ModalityDropdown().SelectByText("CR");
                userpref.ThumbnailSplittingImageRadioBtn().Click();
                userpref.ViewingScopeSeriesRadioBtn().Click();
                userpref.CloseUserPreferences();
                study = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //Step 2
                //Studies tab, search for a patient with many priors 
                //patient "MICKEY, MOUSE") ACC01
                study.SearchStudy(AccessionNo: AccessionNumbers[0], Datasource: EA_91);
                PageLoadWait.WaitForPageLoad(20);
                ExecutedSteps++;

                //Step 3
                //Select one of the studies and open it in the BlueRing viewer 
                //accession ACC01)
                study.SelectStudy("Accession", AccessionNumbers[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();

                //The BlueRing viewer opens with one study in the Study Panel. 
                //This is the primary study, so, no "Primary/Non-Primary" text in the top-left corner of the viewports.
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid + "_3_NonPrimaryText_NotDisplay_Panel1", ExecutedSteps + 1);
                bool status3 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                var priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                bool ExamList_color_3 = false;

                for (int i = 0; i < priors.Count; i++)
                {
                    if (priors[i].GetAttribute("innerHTML").Contains(AccessionNumbers[0]))
                    {
                        Logger.Instance.InfoLog("Prior " + i + " border Color - " + priors[i].GetCssValue("border-top-color"));
                        //ExamList_color_3 = priors[i].GetCssValue("border-color").Equals("rgb(90, 170, 255)") || priors[i].GetCssValue("border-color").Equals("#5aaaff");
                        ExamList_color_3 =
                            (priors[i].GetCssValue("border-top-color").Equals("rgb(90, 170, 255)") &&
                            priors[i].GetCssValue("border-bottom-color").Equals("rgb(90, 170, 255)") &&
                            priors[i].GetCssValue("border-left-color").Equals("rgb(90, 170, 255)") &&
                            priors[i].GetCssValue("border-right-color").Equals("rgb(90, 170, 255)"))
                            ||
                            (priors[i].GetCssValue("border-top-color").Equals("rgba(90, 170, 255, 1)") &&
                            priors[i].GetCssValue("border-bottom-color").Equals("rgba(90, 170, 255, 1)") &&
                            priors[i].GetCssValue("border-left-color").Equals("rgba(90, 170, 255, 1)") &&
                            priors[i].GetCssValue("border-right-color").Equals("rgba(90, 170, 255, 1)")) 
                            || 
                            (priors[i].GetCssValue("border-top-color").Equals("#5aaaff") &&
                            priors[i].GetCssValue("border-bottom-color").Equals("#5aaaff") &&
                            priors[i].GetCssValue("border-left-color").Equals("#5aaaff") &&
                            priors[i].GetCssValue("border-right-color").Equals("#5aaaff"));

                        //rgba(90, 170, 255, 1)

                    }
                }

                if (ExamList_color_3 && status3)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("validation failed Examlist Colour " + ExamList_color_3 + "and Image compare is " + status3);                    
                }

                //Step 4
                //Verify the "PRIMARY" text indicator should be in green with the date and time on the next line for the Primary study.
                ExecutedSteps++;

                //Step 5
                //Open 2 prior studies for this patient by clicking 2 studies from the Exam List, 
                //one after the other (for MICKEY, MOUSE, select study dated 24-Feb-2000, then select study label 19-Aug-2004)
                viewer.OpenPriors(StudyDate: Date[0]);
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitforViewports();
                viewer.OpenPriors(StudyDate: Date[1]);
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitforViewports();

                //top-left image text block has "Non-Primary" at the bottom for all viewports in study panels.
                //The Exam List has the primary and the opened prior studies highlighted (blue border around the study info rectangle).

                IList<IWebElement> panels = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudyPanel));
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid + "_4_Studypanel_", ExecutedSteps + 1,1);
                bool status4_1 = study.CompareImage(result.steps[ExecutedSteps], panels[0],1, RGBTolerance: 70);
                result.steps[ExecutedSteps].SetPath(testid + "_4_Studypanel_", ExecutedSteps + 1,2);
                bool status4_2 = study.CompareImage(result.steps[ExecutedSteps], panels[1],2, RGBTolerance: 70);
                result.steps[ExecutedSteps].SetPath(testid + "_4_Studypanel_", ExecutedSteps + 1,3);
                bool status4_3 = study.CompareImage(result.steps[ExecutedSteps], panels[2],3,1, RGBTolerance: 70);

                priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                bool ExamList_color_4 = false;

                for (int i = 0; i < priors.Count; i++)
                {
                    //if (priors[i].GetCssValue("border-color").Equals("rgb(90, 170, 255)") || priors[i].GetCssValue("border-color").Equals("5aaaff"))
                    if (
                            (priors[i].GetCssValue("border-top-color").Equals("rgb(90, 170, 255)") &&
                            priors[i].GetCssValue("border-bottom-color").Equals("rgb(90, 170, 255)") &&
                            priors[i].GetCssValue("border-left-color").Equals("rgb(90, 170, 255)") &&
                            priors[i].GetCssValue("border-right-color").Equals("rgb(90, 170, 255)"))
                            ||
                            (priors[i].GetCssValue("border-top-color").Equals("rgba(90, 170, 255, 1)") &&
                            priors[i].GetCssValue("border-bottom-color").Equals("rgba(90, 170, 255, 1)") &&
                            priors[i].GetCssValue("border-left-color").Equals("rgba(90, 170, 255, 1)") &&
                            priors[i].GetCssValue("border-right-color").Equals("rgba(90, 170, 255, 1)"))
                            ||
                            (priors[i].GetCssValue("border-top-color").Equals("#5aaaff") &&
                            priors[i].GetCssValue("border-bottom-color").Equals("#5aaaff") &&
                            priors[i].GetCssValue("border-left-color").Equals("#5aaaff") &&
                            priors[i].GetCssValue("border-right-color").Equals("#5aaaff"))
                        )
                    {
                        ExamList_color_4 = true;
                    }
                    else
                    {
                        ExamList_color_4 = false;
                        break;
                    }
                    Logger.Instance.InfoLog("Prior " + i + " actual Color - " + priors[i].GetCssValue("border-top-color"));
                }

                if (priors.Count == 3 && ExamList_color_4 && status4_1 && status4_2 && status4_3)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("Prior Count--" + priors.Count + " ExamList Color " + ExamList_color_4 + "Image Compare--" + status4_1 + status4_2 + status4_3);
                }

                //Step 6
                //Verify the "COMPARISON" text indicator should be in orange with the date and time on the next line for the Non-Primary study.
                ExecutedSteps++;

                //Step 7
                //Drag 2nd thumbnail in the 2nd study panel (2nd from left-side) into the 3rd viewport of the primary 
                //(first from left-side) study panel (3rd viewport is the bottom-left quadrant)
                viewer.SetViewPort(2, 1);
                IWebElement viewPort = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                IList<IWebElement> Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(2) " + BluRingViewer.div_thumbnails));

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
                    Logger.Instance.ErrorLog("Step 5: Exception while thumbnail drag and drop --" + e);
                    action = new TestCompleteAction();
                    Thread.Sleep(30000);
                    action.ClickAndHold(Thumbnail_list[1]);
                    action.MoveToElement(viewPort);
                    action.Click(viewPort);
                    action.MoveToElement(Thumbnail_list[1]).Perform();
                    Thread.Sleep(10000);
                }

                //The 3rd viewport of the primary study panel displays the series from the 2nd study panel prior (the 24-Feb-2000 study), 
                //The in-focus (thick blue-border) thumbnail is the 2nd thumbnail of the 2nd study panel.

                IList<IWebElement> Viewport_Outer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(1) " + BluRingViewer.div_viewport_Outer));
                //bool Vieport_color_5 = Viewport_Outer[2].GetCssValue("border-color").Equals("rgb(90, 170, 255)") || Viewport_Outer[2].GetCssValue("border-color").Equals("5aaaff");
                bool Vieport_color_5 = (Viewport_Outer[2].GetCssValue("border-top-color").Equals("rgb(90, 170, 255)") &&
                            Viewport_Outer[2].GetCssValue("border-bottom-color").Equals("rgb(90, 170, 255)") &&
                            Viewport_Outer[2].GetCssValue("border-left-color").Equals("rgb(90, 170, 255)") &&
                            Viewport_Outer[2].GetCssValue("border-right-color").Equals("rgb(90, 170, 255)"))
                            || 
                            (Viewport_Outer[2].GetCssValue("border-top-color").Equals("rgba(90, 170, 255, 1)") &&
                            Viewport_Outer[2].GetCssValue("border-bottom-color").Equals("rgba(90, 170, 255, 1)") &&
                            Viewport_Outer[2].GetCssValue("border-left-color").Equals("rgba(90, 170, 255, 1)") &&
                            Viewport_Outer[2].GetCssValue("border-right-color").Equals("rgba(90, 170, 255, 1)"))
                            ||
                            (Viewport_Outer[2].GetCssValue("border-top-color").Equals("#5aaaff") &&
                            Viewport_Outer[2].GetCssValue("border-bottom-color").Equals("#5aaaff") &&
                            Viewport_Outer[2].GetCssValue("border-left-color").Equals("#5aaaff") &&
                            Viewport_Outer[2].GetCssValue("border-right-color").Equals("#5aaaff"));

                Logger.Instance.InfoLog("Vieport_color_5 - " + Viewport_Outer[2].GetCssValue("border-top-color"));

                IList<IWebElement> Thumbnail_Outer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(2) " + BluRingViewer.div_ThumbnailOuter));
                //bool Thumbnail_color_5 = Thumbnail_Outer[1].GetCssValue("border-color").Equals("rgb(90, 170, 255)") || Thumbnail_Outer[1].GetCssValue("border-color").Equals("5aaaff");
                bool Thumbnail_color_5 = (Thumbnail_Outer[1].GetCssValue("border-top-color").Equals("rgb(90, 170, 255)") &&
                            Thumbnail_Outer[1].GetCssValue("border-bottom-color").Equals("rgb(90, 170, 255)") &&
                            Thumbnail_Outer[1].GetCssValue("border-left-color").Equals("rgb(90, 170, 255)") &&
                            Thumbnail_Outer[1].GetCssValue("border-right-color").Equals("rgb(90, 170, 255)"))
                            || 
                            (Thumbnail_Outer[1].GetCssValue("border-top-color").Equals("rgba(90, 170, 255, 1)") &&
                            Thumbnail_Outer[1].GetCssValue("border-bottom-color").Equals("rgba(90, 170, 255, 1)") &&
                            Thumbnail_Outer[1].GetCssValue("border-left-color").Equals("rgba(90, 170, 255, 1)") &&
                            Thumbnail_Outer[1].GetCssValue("border-right-color").Equals("rgba(90, 170, 255, 1)"))
                            ||
                            (Thumbnail_Outer[1].GetCssValue("border-top-color").Equals("#5aaaff") &&
                            Thumbnail_Outer[1].GetCssValue("border-bottom-color").Equals("#5aaaff") &&
                            Thumbnail_Outer[1].GetCssValue("border-left-color").Equals("#5aaaff") &&
                            Thumbnail_Outer[1].GetCssValue("border-right-color").Equals("#5aaaff"));

                Logger.Instance.InfoLog("Thumbnail_color_5 - " + Thumbnail_Outer[1].GetCssValue("border-top-color"));

                panels = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudyPanel));
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid + "_5_studypanel_", ExecutedSteps + 1,1);
                bool status5_1 = study.CompareImage(result.steps[ExecutedSteps], panels[0],1, RGBTolerance: 70);
                result.steps[ExecutedSteps].SetPath(testid + "_5_studypanel_", ExecutedSteps + 1,2);
                bool status5_2 = study.CompareImage(result.steps[ExecutedSteps], panels[1],2, RGBTolerance: 70);
                result.steps[ExecutedSteps].SetPath(testid + "_5_studypanel_", ExecutedSteps + 1,3);
                bool status5_3 = study.CompareImage(result.steps[ExecutedSteps], panels[2],3,1, RGBTolerance: 70);

                if (Vieport_color_5 && Thumbnail_color_5 && status5_1 && status5_2 && status5_3)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("Vieport_color_5--" + Vieport_color_5 + " Thumbnail_color_5 " + Thumbnail_color_5 + "Image Compare--" + status5_1 + status5_2 + status5_3);
                }

                //Step 8
                //Drag the 2nd thumbnail of the 1st (pri) study panel into the 2nd viewport of the 3rd study panel.
                viewer.SetViewPort(1, 3);
                viewPort = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(1) " + BluRingViewer.div_thumbnails));

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
                    Logger.Instance.ErrorLog("Step 6: Exception while thumbnail drag and drop --" + e);
                    action = new TestCompleteAction();
                    Thread.Sleep(30000);
                    action.ClickAndHold(Thumbnail_list[1]);
                    action.MoveToElement(viewPort);
                    action.Click(viewPort);
                    action.MoveToElement(Thumbnail_list[1]).Perform();
                    Thread.Sleep(10000);
                }

                //2nd viewport of the 3rd study panel displays the series from 2nd thumbnail of the prim study panel (04-Feb-2000 study),
                //it is the current active viewport (blue). The top-left image text does not have "Non-Primary".

                Viewport_Outer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(3) " + BluRingViewer.div_viewport_Outer));
                //bool Vieport_color_6 = Viewport_Outer[1].GetCssValue("border-color").Equals("rgb(90, 170, 255)") || Viewport_Outer[1].GetCssValue("border-color").Equals("5aaaff");

                bool Vieport_color_6 = (Viewport_Outer[1].GetCssValue("border-top-color").Equals("rgb(90, 170, 255)") &&
                            Viewport_Outer[1].GetCssValue("border-bottom-color").Equals("rgb(90, 170, 255)") &&
                            Viewport_Outer[1].GetCssValue("border-left-color").Equals("rgb(90, 170, 255)") &&
                            Viewport_Outer[1].GetCssValue("border-right-color").Equals("rgb(90, 170, 255)"))
                            || 
                            (Viewport_Outer[1].GetCssValue("border-top-color").Equals("rgba(90, 170, 255, 1)") &&
                            Viewport_Outer[1].GetCssValue("border-bottom-color").Equals("rgba(90, 170, 255, 1)") &&
                            Viewport_Outer[1].GetCssValue("border-left-color").Equals("rgba(90, 170, 255, 1)") &&
                            Viewport_Outer[1].GetCssValue("border-right-color").Equals("rgba(90, 170, 255, 1)"))
                            ||
                            (Viewport_Outer[1].GetCssValue("border-top-color").Equals("#5aaaff") &&
                            Viewport_Outer[1].GetCssValue("border-bottom-color").Equals("#5aaaff") &&
                            Viewport_Outer[1].GetCssValue("border-left-color").Equals("#5aaaff") &&
                            Viewport_Outer[1].GetCssValue("border-right-color").Equals("#5aaaff"));

                Logger.Instance.InfoLog("Vieport_color_6 - " + Viewport_Outer[1].GetCssValue("border-top-color"));

                Thumbnail_Outer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(1) " + BluRingViewer.div_ThumbnailOuter));
                //bool Thumbnail_color_6 = Thumbnail_Outer[1].GetCssValue("border-color").Equals("rgb(90, 170, 255)") || Thumbnail_Outer[1].GetCssValue("border-color").Equals("5aaaff");
                bool Thumbnail_color_6 = (Thumbnail_Outer[1].GetCssValue("border-top-color").Equals("rgb(90, 170, 255)") &&
                            Thumbnail_Outer[1].GetCssValue("border-bottom-color").Equals("rgb(90, 170, 255)") &&
                            Thumbnail_Outer[1].GetCssValue("border-left-color").Equals("rgb(90, 170, 255)") &&
                            Thumbnail_Outer[1].GetCssValue("border-right-color").Equals("rgb(90, 170, 255)"))
                            || 
                            (Thumbnail_Outer[1].GetCssValue("border-top-color").Equals("rgba(90, 170, 255, 1)") &&
                            Thumbnail_Outer[1].GetCssValue("border-bottom-color").Equals("rgba(90, 170, 255, 1)") &&
                            Thumbnail_Outer[1].GetCssValue("border-left-color").Equals("rgba(90, 170, 255, 1)") &&
                            Thumbnail_Outer[1].GetCssValue("border-right-color").Equals("rgba(90, 170, 255, 1)"))
                            ||
                            (Thumbnail_Outer[1].GetCssValue("border-top-color").Equals("#5aaaff") &&
                            Thumbnail_Outer[1].GetCssValue("border-bottom-color").Equals("#5aaaff") &&
                            Thumbnail_Outer[1].GetCssValue("border-left-color").Equals("#5aaaff") &&
                            Thumbnail_Outer[1].GetCssValue("border-right-color").Equals("#5aaaff"));

                Logger.Instance.InfoLog("Thumbnail_color_6 - " + Thumbnail_Outer[1].GetCssValue("border-top-color"));

                panels = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudyPanel));
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid + "_6_studypanel_", ExecutedSteps + 1,1);
                bool status6_1 = study.CompareImage(result.steps[ExecutedSteps], panels[0],1, RGBTolerance: 70);
                result.steps[ExecutedSteps].SetPath(testid + "_6_studypanel_", ExecutedSteps + 1,2);
                bool status6_2 = study.CompareImage(result.steps[ExecutedSteps], panels[1],2, RGBTolerance: 70);
                result.steps[ExecutedSteps].SetPath(testid + "_6_studypanel_", ExecutedSteps + 1,3);
                bool status6_3 = study.CompareImage(result.steps[ExecutedSteps], panels[2],3,1, RGBTolerance: 70);

                if (Vieport_color_6 && Thumbnail_color_6 && status6_1 && status6_2 && status6_3)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.ErrorLog("Vieport_color_6--" + Vieport_color_6 + " Thumbnail_color_6 " + ExamList_color_4 + "Image Compare--" + status6_1 + status6_2 + status6_3);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 9
                //Drag 1st thumbnail in the 3rd study panel into the 4th viewport of the primary study panel.

                viewer.SetViewPort(3, 1);
                viewPort = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(3) " + BluRingViewer.div_thumbnails));

                action = new TestCompleteAction();
                try
                {
                    Thread.Sleep(30000);
                    action.ClickAndHold(Thumbnail_list[0]);
                    action.MoveToElement(viewPort).Click().Perform();
                    Thread.Sleep(10000);
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Step 7: Exception while thumbnail drag and drop --" + e);
                    action = new TestCompleteAction();
                    Thread.Sleep(30000);
                    action.ClickAndHold(Thumbnail_list[0]);
                    action.MoveToElement(viewPort);
                    action.Click(viewPort);
                    action.MoveToElement(Thumbnail_list[0]).Perform();
                    Thread.Sleep(10000);
                }

                //3rd study panel
                //2nd viewport of the 3rd study panel still displays the series from the 2nd thumbnail of the primary study panel (the 04-Feb-2000 study). 
                //3rd viewport of the primary study panel still contains the foreign series and has the "Non-Primary". 
                //1st and 2nd viewports of the primary study panel does not have the "Non-Primary"

                Viewport_Outer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(1) " + BluRingViewer.div_viewport_Outer));
                //bool Vieport_color_7 = Viewport_Outer[3].GetCssValue("border-color").Equals("rgb(90, 170, 255)");

                bool Vieport_color_7 = (Viewport_Outer[3].GetCssValue("border-top-color").Equals("rgb(90, 170, 255)") &&
                            Viewport_Outer[3].GetCssValue("border-bottom-color").Equals("rgb(90, 170, 255)") &&
                            Viewport_Outer[3].GetCssValue("border-left-color").Equals("rgb(90, 170, 255)") &&
                            Viewport_Outer[3].GetCssValue("border-right-color").Equals("rgb(90, 170, 255)"))
                            || 
                            (Viewport_Outer[3].GetCssValue("border-top-color").Equals("rgba(90, 170, 255, 1)") &&
                            Viewport_Outer[3].GetCssValue("border-bottom-color").Equals("rgba(90, 170, 255, 1)") &&
                            Viewport_Outer[3].GetCssValue("border-left-color").Equals("rgba(90, 170, 255, 1)") &&
                            Viewport_Outer[3].GetCssValue("border-right-color").Equals("rgba(90, 170, 255, 1)"))
                            ||
                            (Viewport_Outer[3].GetCssValue("border-top-color").Equals("#5aaaff") &&
                            Viewport_Outer[3].GetCssValue("border-bottom-color").Equals("#5aaaff") &&
                            Viewport_Outer[3].GetCssValue("border-left-color").Equals("#5aaaff") &&
                            Viewport_Outer[3].GetCssValue("border-right-color").Equals("#5aaaff"));

                Logger.Instance.InfoLog("Vieport_color_7 - " + Viewport_Outer[3].GetCssValue("border-top-color"));

                Thumbnail_Outer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(3) " + BluRingViewer.div_ThumbnailOuter));
                //bool Thumbnail_color_7 = Thumbnail_Outer[0].GetCssValue("border-color").Equals("rgb(90, 170, 255)");

                bool Thumbnail_color_7 = (Thumbnail_Outer[0].GetCssValue("border-top-color").Equals("rgb(90, 170, 255)") &&
                            Thumbnail_Outer[0].GetCssValue("border-bottom-color").Equals("rgb(90, 170, 255)") &&
                            Thumbnail_Outer[0].GetCssValue("border-left-color").Equals("rgb(90, 170, 255)") &&
                            Thumbnail_Outer[0].GetCssValue("border-right-color").Equals("rgb(90, 170, 255)"))
                            || 
                            (Thumbnail_Outer[0].GetCssValue("border-top-color").Equals("rgba(90, 170, 255, 1)") &&
                            Thumbnail_Outer[0].GetCssValue("border-bottom-color").Equals("rgba(90, 170, 255, 1)") &&
                            Thumbnail_Outer[0].GetCssValue("border-left-color").Equals("rgba(90, 170, 255, 1)") &&
                            Thumbnail_Outer[0].GetCssValue("border-right-color").Equals("rgba(90, 170, 255, 1)"))
                            ||
                            (Thumbnail_Outer[0].GetCssValue("border-top-color").Equals("#5aaaff") &&
                            Thumbnail_Outer[0].GetCssValue("border-bottom-color").Equals("#5aaaff") &&
                            Thumbnail_Outer[0].GetCssValue("border-left-color").Equals("#5aaaff") &&
                            Thumbnail_Outer[0].GetCssValue("border-right-color").Equals("#5aaaff"));

                Logger.Instance.InfoLog("Thumbnail_color_7 - " + Thumbnail_Outer[0].GetCssValue("border-top-color"));

                panels = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudyPanel));
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid + "_7_studypanel_", ExecutedSteps + 1,1);
                bool status7_1 = study.CompareImage(result.steps[ExecutedSteps], panels[0],1, RGBTolerance: 70);
                result.steps[ExecutedSteps].SetPath(testid + "_7_studypanel_", ExecutedSteps + 1,2);
                bool status7_2 = study.CompareImage(result.steps[ExecutedSteps], panels[1],2, RGBTolerance: 70);
                result.steps[ExecutedSteps].SetPath(testid + "_7_studypanel_", ExecutedSteps + 1,3);
                bool status7_3 = study.CompareImage(result.steps[ExecutedSteps], panels[2],3,1, RGBTolerance: 70);


                if (Vieport_color_7 && Thumbnail_color_7 && status7_1 && status7_2 && status7_3)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.ErrorLog("Prior Count--" + priors.Count + " ExamList Color " + ExamList_color_4 + "Image Compare--" + status7_1 + status7_1 + status7_1);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 10
                //Drag the 3rd thumbnail of the 2nd study panel to 2nd viewport of the 3rd study panel, 
                //& drag the 4th thumbnail of the 2nd study panel to the 3rd viewport of the 3rd study panel.                
                viewer.SetViewPort(1, 3);
                viewPort = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(2) " + BluRingViewer.div_thumbnails));

                action = new TestCompleteAction();
                try
                {
                    Thread.Sleep(30000);
                    action.ClickAndHold(Thumbnail_list[2]);
                    action.MoveToElement(viewPort).Click().Perform();
                    Thread.Sleep(10000);
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Step 8.1: Exception while thumbnail drag and drop --" + e);
                    action = new TestCompleteAction();
                    Thread.Sleep(30000);
                    action.ClickAndHold(Thumbnail_list[2]);
                    action.MoveToElement(viewPort);
                    action.Click(viewPort);
                    action.MoveToElement(Thumbnail_list[2]).Perform();
                    Thread.Sleep(10000);
                }

                //Click next on thumbnail bar to show the 4th thumbnail (at time may go out of visible area)
                try
                {
                    IWebElement thumbnailBarNext = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ThumbnailNavNext(2)));
                   thumbnailBarNext.Click();
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Step 8 - 'Next' click on thumbnail bar in study panel 2 failed. Ex - " + ex);
                }

                viewer.SetViewPort(2, 3);
                viewPort = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(2) " + BluRingViewer.div_thumbnails));
                action = new TestCompleteAction();
                try
                {
                    Thread.Sleep(30000);
                    action.ClickAndHold(Thumbnail_list[3]);
                    action.MoveToElement(viewPort).Click().Perform();
                    Thread.Sleep(10000);
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Step 8.2: Exception while thumbnail drag and drop --" + e);
                    action = new TestCompleteAction();
                    Thread.Sleep(30000);
                    action.ClickAndHold(Thumbnail_list[3]);
                    action.MoveToElement(viewPort);
                    action.Click(viewPort);
                    action.MoveToElement(Thumbnail_list[3]).Perform();
                    Thread.Sleep(10000);
                }

                //2nd viewport of the 3rd study panel is replaced with the series from the 3rdd thumbnail of the 2nd study panel (the 24-Feb-2000 study). has "Non-Primary".
                //3rd viewport of the primary study panel still contains the foreign series and has "Non-Primary".
                //4th viewport contains the series from the 3rd study panel (19-Aug-2004 study). It has "Non-Primary"

                Viewport_Outer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(3) " + BluRingViewer.div_viewport_Outer));
                //bool Vieport_color_8 = Viewport_Outer[2].GetCssValue("border-color").Equals("rgb(90, 170, 255)");

                bool Vieport_color_8 = (Viewport_Outer[2].GetCssValue("border-top-color").Equals("rgb(90, 170, 255)") &&
                            Viewport_Outer[2].GetCssValue("border-bottom-color").Equals("rgb(90, 170, 255)") &&
                            Viewport_Outer[2].GetCssValue("border-left-color").Equals("rgb(90, 170, 255)") &&
                            Viewport_Outer[2].GetCssValue("border-right-color").Equals("rgb(90, 170, 255)"))
                            || 
                            (Viewport_Outer[2].GetCssValue("border-top-color").Equals("rgba(90, 170, 255, 1)") &&
                            Viewport_Outer[2].GetCssValue("border-bottom-color").Equals("rgba(90, 170, 255, 1)") &&
                            Viewport_Outer[2].GetCssValue("border-left-color").Equals("rgba(90, 170, 255, 1)") &&
                            Viewport_Outer[2].GetCssValue("border-right-color").Equals("rgba(90, 170, 255, 1)"))
                            ||
                            (Viewport_Outer[2].GetCssValue("border-top-color").Equals("#5aaaff") &&
                            Viewport_Outer[2].GetCssValue("border-bottom-color").Equals("#5aaaff") &&
                            Viewport_Outer[2].GetCssValue("border-left-color").Equals("#5aaaff") &&
                            Viewport_Outer[2].GetCssValue("border-right-color").Equals("#5aaaff"));

                Logger.Instance.InfoLog("Vieport_color_8 - " + Viewport_Outer[2].GetCssValue("border-top-color"));

                Thumbnail_Outer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(2) " + BluRingViewer.div_ThumbnailOuter));
                //bool Thumbnail_color_8 = Thumbnail_Outer[3].GetCssValue("border-color").Equals("rgb(90, 170, 255)");

                bool Thumbnail_color_8 = (Thumbnail_Outer[3].GetCssValue("border-top-color").Equals("rgb(90, 170, 255)") &&
                           Thumbnail_Outer[3].GetCssValue("border-bottom-color").Equals("rgb(90, 170, 255)") &&
                           Thumbnail_Outer[3].GetCssValue("border-left-color").Equals("rgb(90, 170, 255)") &&
                           Thumbnail_Outer[3].GetCssValue("border-right-color").Equals("rgb(90, 170, 255)"))
                           || 
                           (Thumbnail_Outer[3].GetCssValue("border-top-color").Equals("rgba(90, 170, 255, 1)") &&
                           Thumbnail_Outer[3].GetCssValue("border-bottom-color").Equals("rgba(90, 170, 255, 1)") &&
                           Thumbnail_Outer[3].GetCssValue("border-left-color").Equals("rgba(90, 170, 255, 1)") &&
                           Thumbnail_Outer[3].GetCssValue("border-right-color").Equals("rgba(90, 170, 255, 1)"))
                           ||
                           (Thumbnail_Outer[3].GetCssValue("border-top-color").Equals("#5aaaff") &&
                           Thumbnail_Outer[3].GetCssValue("border-bottom-color").Equals("#5aaaff") &&
                           Thumbnail_Outer[3].GetCssValue("border-left-color").Equals("#5aaaff") &&
                           Thumbnail_Outer[3].GetCssValue("border-right-color").Equals("#5aaaff"));

                Logger.Instance.InfoLog("Thumbnail_color_8 - " + Thumbnail_Outer[3].GetCssValue("border-top-color"));

                panels = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudyPanel));
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid + "_8_studypanel_", ExecutedSteps + 1,1);
                bool status8_1 = study.CompareImage(result.steps[ExecutedSteps], panels[0],1, RGBTolerance: 70);
                result.steps[ExecutedSteps].SetPath(testid + "_8_studypanel_", ExecutedSteps + 1,2);
                bool status8_2 = study.CompareImage(result.steps[ExecutedSteps], panels[1],2, RGBTolerance: 70);
                result.steps[ExecutedSteps].SetPath(testid + "_8_studypanel_", ExecutedSteps + 1,3);
                bool status8_3 = study.CompareImage(result.steps[ExecutedSteps], panels[2],3,1, RGBTolerance: 70);

                if (Vieport_color_8 && Thumbnail_color_8 && status8_1 && status8_2 && status8_3)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.ErrorLog("Vieport_color_8-" + Vieport_color_8 + " Thumbnail_color_8 " + Thumbnail_color_8 + "Image Compare--" + status8_1 + status8_2 + status8_3);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 11
                //Select on the SHOW/HIDE > HIDE IMAGE TEXT from the global tool bar.
                viewer.SelectShowHideValue("HIDE IMAGE TEXT");

                //"Image text in all of the visible viewports are hidden, including the "Non-Primary" from the prior study panels.
                //The active viewport and in-focus thumbnail remains the same as before the hidden text

                panels = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudyPanel));
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid + "_9_studypanel_", ExecutedSteps + 1,1);
                bool status9_1 = study.CompareImage(result.steps[ExecutedSteps], panels[0],1, RGBTolerance: 70);
                result.steps[ExecutedSteps].SetPath(testid + "_9_studypanel_", ExecutedSteps + 1,2);
                bool status9_2 = study.CompareImage(result.steps[ExecutedSteps], panels[1],2, RGBTolerance: 70);
                result.steps[ExecutedSteps].SetPath(testid + "_9_studypanel_", ExecutedSteps + 1,3);
                bool status9_3 = study.CompareImage(result.steps[ExecutedSteps], panels[2],3,1, RGBTolerance: 70);

                if (status9_1 && status9_2 && status9_3)
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

                //Step 12
                //Select on the SHOW/HIDE > SHOW IMAGE TEXT from the global tool bar.

                viewer.SelectShowHideValue("SHOW IMAGE TEXT");

                //Image text in all of the visible viewports are shown again, including the "Non-Primary" from the prior study panels.
                //The viewports (including the ones with the foreign series) status remains the same as before the hidden text."
                Viewport_Outer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(3) " + BluRingViewer.div_viewport_Outer));
                //bool Vieport_color_10 = Viewport_Outer[2].GetCssValue("border-color").Equals("rgb(90, 170, 255)");

                bool Vieport_color_10 = (Viewport_Outer[2].GetCssValue("border-top-color").Equals("rgb(90, 170, 255)") &&
                           Viewport_Outer[2].GetCssValue("border-bottom-color").Equals("rgb(90, 170, 255)") &&
                           Viewport_Outer[2].GetCssValue("border-left-color").Equals("rgb(90, 170, 255)") &&
                           Viewport_Outer[2].GetCssValue("border-right-color").Equals("rgb(90, 170, 255)"))
                           || 
                           (Viewport_Outer[2].GetCssValue("border-top-color").Equals("rgba(90, 170, 255, 1)") &&
                           Viewport_Outer[2].GetCssValue("border-bottom-color").Equals("rgba(90, 170, 255, 1)") &&
                           Viewport_Outer[2].GetCssValue("border-left-color").Equals("rgba(90, 170, 255, 1)") &&
                           Viewport_Outer[2].GetCssValue("border-right-color").Equals("rgba(90, 170, 255, 1)"))
                           ||
                           (Viewport_Outer[2].GetCssValue("border-top-color").Equals("#5aaaff") &&
                           Viewport_Outer[2].GetCssValue("border-bottom-color").Equals("#5aaaff") &&
                           Viewport_Outer[2].GetCssValue("border-left-color").Equals("#5aaaff") &&
                           Viewport_Outer[2].GetCssValue("border-right-color").Equals("#5aaaff"));

                Logger.Instance.InfoLog("Vieport_color_10 - " + Viewport_Outer[2].GetCssValue("border-top-color"));

                Thumbnail_Outer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(2) " + BluRingViewer.div_ThumbnailOuter));
                //bool Thumbnail_color_10 = Thumbnail_Outer[3].GetCssValue("border-color").Equals("rgb(90, 170, 255)");

                bool Thumbnail_color_10 = (Thumbnail_Outer[3].GetCssValue("border-top-color").Equals("rgb(90, 170, 255)") &&
                           Thumbnail_Outer[3].GetCssValue("border-bottom-color").Equals("rgb(90, 170, 255)") &&
                           Thumbnail_Outer[3].GetCssValue("border-left-color").Equals("rgb(90, 170, 255)") &&
                           Thumbnail_Outer[3].GetCssValue("border-right-color").Equals("rgb(90, 170, 255)"))
                           ||
                           (Thumbnail_Outer[3].GetCssValue("border-top-color").Equals("rgba(90, 170, 255, 1)") &&
                           Thumbnail_Outer[3].GetCssValue("border-bottom-color").Equals("rgba(90, 170, 255, 1)") &&
                           Thumbnail_Outer[3].GetCssValue("border-left-color").Equals("rgba(90, 170, 255, 1)") &&
                           Thumbnail_Outer[3].GetCssValue("border-right-color").Equals("rgba(90, 170, 255, 1)"))
                           ||
                           (Thumbnail_Outer[3].GetCssValue("border-top-color").Equals("#5aaaff") &&
                           Thumbnail_Outer[3].GetCssValue("border-bottom-color").Equals("#5aaaff") &&
                           Thumbnail_Outer[3].GetCssValue("border-left-color").Equals("#5aaaff") &&
                           Thumbnail_Outer[3].GetCssValue("border-right-color").Equals("#5aaaff"));

                Logger.Instance.InfoLog("Thumbnail_color_10 - " + Thumbnail_Outer[3].GetCssValue("border-top-color"));

                panels = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudyPanel));
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid + "_10_studypanel_", ExecutedSteps + 1,1);
                bool status10_1 = study.CompareImage(result.steps[ExecutedSteps], panels[0],1, RGBTolerance: 70);
                result.steps[ExecutedSteps].SetPath(testid + "_10_studypanel_", ExecutedSteps + 1,2);
                bool status10_2 = study.CompareImage(result.steps[ExecutedSteps], panels[1],2, RGBTolerance: 70);
                result.steps[ExecutedSteps].SetPath(testid + "_10_studypanel_", ExecutedSteps + 1,3);
                bool status10_3 = study.CompareImage(result.steps[ExecutedSteps], panels[2],3,1, RGBTolerance: 70);

                if (Vieport_color_10 && Thumbnail_color_10 && status10_1 && status10_2 && status10_3)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.ErrorLog("Vieport_color_10-" + Vieport_color_10 + "Vieport_color_10 " + Vieport_color_10 + "Image Compare--" + status10_1 + status10_2 + status10_3);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 13
                //Click X (Exit) icon from the top right corner of the global toolbar menu to close the BlueRing viewer. Select the same study again and open it in the BlueRing viewer (for MICKEY, MOUSE, select accession ACC01).
                //(To show that the foreign series do not persist in the studies).
                viewer.CloseBluRingViewer();
                study.SearchStudy(AccessionNo: AccessionNumbers[0], Datasource: EA_91);
                PageLoadWait.WaitForPageLoad(20);
                study.SelectStudy("Accession", AccessionNumbers[0]);  //accession ACC01)
                viewer = BluRingViewer.LaunchBluRingViewer();

                //The BlueRing viewer opens with one study in the Study Panel. This is the primary study, 
                //The current study in the Exam List is highlighted (blue border around the study info rectangle). 
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid + "_11_studypanel_1", ExecutedSteps + 1);
                bool status11 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                bool ExamList_color_11 = false;

                for (int i = 0; i < priors.Count; i++)
                {
                    if (priors[i].GetAttribute("innerHTML").Contains(AccessionNumbers[0]))
                    {
                        //ExamList_color_11 = priors[i].GetCssValue("border-color").Equals("rgb(90, 170, 255)");
                        ExamList_color_11 = (priors[i].GetCssValue("border-top-color").Equals("rgb(90, 170, 255)") &&
                           priors[i].GetCssValue("border-bottom-color").Equals("rgb(90, 170, 255)") &&
                           priors[i].GetCssValue("border-left-color").Equals("rgb(90, 170, 255)") &&
                           priors[i].GetCssValue("border-right-color").Equals("rgb(90, 170, 255)"))
                           || 
                           (priors[i].GetCssValue("border-top-color").Equals("rgba(90, 170, 255, 1)") &&
                           priors[i].GetCssValue("border-bottom-color").Equals("rgba(90, 170, 255, 1)") &&
                           priors[i].GetCssValue("border-left-color").Equals("rgba(90, 170, 255, 1)") &&
                           priors[i].GetCssValue("border-right-color").Equals("rgba(90, 170, 255, 1)"))
                           ||
                           (priors[i].GetCssValue("border-top-color").Equals("#5aaaff") &&
                           priors[i].GetCssValue("border-bottom-color").Equals("#5aaaff") &&
                           priors[i].GetCssValue("border-left-color").Equals("#5aaaff") &&
                           priors[i].GetCssValue("border-right-color").Equals("#5aaaff"));
                        Logger.Instance.InfoLog("ExamList_color_11 - " + priors[i].GetCssValue("border-top-color"));
                    }
                }

                if (ExamList_color_11 && status11)
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
                viewer.CloseBluRingViewer();
                login.Logout();
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

                //Return Result
                return result;
            }
        }

        /// <summary> 
        /// 138729   - Foreign Series:Double-click thumbnails between study panels
        /// </summary>
        ///
        public TestCaseResult Test_161685(string testid, string teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;

            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                string Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                string StudyDateTime = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDateTime");
                string[] studydateTime = StudyDateTime.Split('@');
                string PrimaryStudyDate = studydateTime[0].Split(' ')[0];
                string PrimaryStudyTime = studydateTime[0].Split(' ')[1];
                string Prior1Date = studydateTime[1].Split(' ')[0];

                string Prior2Date = studydateTime[2].Split(' ')[0];
                string Prior2Time = studydateTime[2].Split(' ')[1];

                string Datasource = login.GetHostName(Config.EA91);

                //Step 1
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);
                Studies study = (Studies)login.Navigate("Studies");
                result.steps[++ExecutedSteps].StepPass();

                //Step 2
                study.SearchStudy(AccessionNo: Accession, Datasource: Datasource);
                result.steps[++ExecutedSteps].StepPass();

                //Step 3
                study.SelectStudy("Accession", Accession);
                BluRingViewer bluringviewer = BluRingViewer.LaunchBluRingViewer();

                //Verify that only one study is opened.
                if (BluRingViewer.TotalStudyPanel() == 1)
                {
                    result.steps[++ExecutedSteps].AddPassStatusList("BlueRing viewer opens with one study in the Study Panel");
                }
                else
                {
                    result.steps[++ExecutedSteps].AddFailStatusList("BlueRing viewer not opens with one study in the Study Panel");
                }

                //Compare images
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList[0]))
                    result.steps[ExecutedSteps].AddPassStatusList("This is the primary study, so there are no Primary / Non - Primary text indicators in the top-left corner of the viewports.");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("This is the primary study, so there are Primary / Non - Primary text indicators in the top - left corner of the viewports.");

                //Exam List
                //The current study in the Exam List is highlighted (blue border around the study info rectangle).
                if (bluringviewer.VerifyPriorsHighlightedInExamList(StudyDate: PrimaryStudyDate, StudyTime: PrimaryStudyTime))
                    result.steps[ExecutedSteps].AddPassStatusList("The current study in the Exam List is highlighted (blue border around the study info rectangle).");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The current study in the Exam List is not highlighted (blue border around the study info rectangle).");
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                //Step 4
                //open up 2 prior studies for this patient by clicking 2 studies from the Exam List, one after the other 
                bluringviewer.OpenPriors(StudyDate: Prior1Date);
                if (BluRingViewer.TotalStudyPanel() == 2 && bluringviewer.StudyPanelList.ToArray()[0].Location.X < bluringviewer.StudyPanelList.ToArray()[1].Location.X)
                {
                    result.steps[++ExecutedSteps].AddPassStatusList("Prior exams are opened in new study panels to the right of the primary study panel.");
                }
                else
                {
                    result.steps[++ExecutedSteps].AddFailStatusList("Prior exams are opened in new study panels not to the right of the primary study panel.");
                }

                bluringviewer.OpenPriors(StudyDate: Prior2Date, StudyTime: Prior2Time);
                if (BluRingViewer.TotalStudyPanel() == 3 && bluringviewer.StudyPanelList[1].Location.X < bluringviewer.StudyPanelList[2].Location.X)
                {
                    result.steps[ExecutedSteps].AddPassStatusList("Prior exams are opened in new study panels to the right of the primary study panel.");
                }
                else
                {
                    result.steps[ExecutedSteps].AddFailStatusList("Prior exams are opened in new study panels to the right of the primary study panel.");
                }
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step4ImageCompare_1 = study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[1]);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step4ImageCompare_2 = study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[2], 2, 1);
                if (step4ImageCompare_2 && step4ImageCompare_1)
                {
                    result.steps[ExecutedSteps].AddPassStatusList("The top-left image text block has Non - Primary indicator at the bottom for all the viewports in the prior study panels.");
                }
                else
                {
                    result.steps[ExecutedSteps].AddFailStatusList("The top-left image text block not has Non - Primary indicator at the bottom for all the viewports in the prior study panels.");
                }

                if (bluringviewer.VerifyViewPortIsActive(ViewPortwebObject: BasePage.Driver.FindElement(By.CssSelector(bluringviewer.SetViewPort1(3, 1)))) && BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(3, 1))
                {
                    result.steps[ExecutedSteps].AddPassStatusList("The first thumbnail and viewport are active in the last right-side study panel(i.e., they have the blue highlight border) ");
                }
                else
                {
                    result.steps[ExecutedSteps].AddFailStatusList("The first thumbnail and viewport in the last right-side study panel are not active (i.e., they not have the blue highlight border) ");
                }

                // No other thumbnail or viewport is active in the other study panels. 
                for (int studyPanel = 1; studyPanel <= bluringviewer.StudyPanelList.Count; studyPanel++)
                    for (int viewport = 1; viewport <= bluringviewer.GetViewPortCount(studyPanel); viewport++)
                        if (viewport != 1 || studyPanel != 3)
                            if (bluringviewer.VerifyViewPortIsActive(studyPanel, viewport))
                                result.steps[ExecutedSteps].AddFailStatusList("The View Port " + viewport + " of the " + studyPanel + " study panel is active and current study in the Exam List is highlighted (blue border around the study info rectangle");
                            else
                                result.steps[ExecutedSteps].AddPassStatusList();

                for (int studyPanel = 1; studyPanel <= bluringviewer.StudyPanelList.Count; studyPanel++)
                    for (int thumbnail = 1; thumbnail <= BluRingViewer.NumberOfThumbnailsInStudyPanel(studyPanel); thumbnail++)
                        if (thumbnail != 1 || studyPanel != 3)
                            if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(studyPanel, thumbnail))
                                result.steps[ExecutedSteps].AddFailStatusList("There are thumbnails that are in-focus at study panel " + studyPanel + "at thumbnail" + thumbnail);
                            else
                                result.steps[ExecutedSteps].AddPassStatusList();

                //The Exam List has the primary and the opened prior studies highlighted (blue border around the study info rectangle).
                if (bluringviewer.VerifyPriorsHighlightedInExamList(StudyDate: Prior2Date, StudyTime: Prior2Time) && bluringviewer.VerifyPriorsHighlightedInExamList(StudyDate: Prior1Date) && bluringviewer.VerifyPriorsHighlightedInExamList(StudyDate: PrimaryStudyDate, StudyTime: PrimaryStudyTime))
                    result.steps[ExecutedSteps].AddPassStatusList("All the active Exam has the Blur Border");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("Not all the active Exam has the Blur Border");

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();

                //Step 5
                //Click on the 3rd viewport of the primary (first panel from the left side) and verfiy that it is active
                if (bluringviewer.VerifyViewPortIsActive(ViewPortwebObject: bluringviewer.ClickOnViewPort(1, 3)))
                    result.steps[++ExecutedSteps].AddPassStatusList("The 3rd viewport of the primary study panel is active (3rd viewport is the bottom - left quadrant) - it has a blue border around it.");
                else
                    result.steps[++ExecutedSteps].AddFailStatusList("The 3rd viewport of the primary study panel is not active (3rd viewport is the bottom - left quadrant) - it has a not blue border around it.");

                for (int studyPanel = 1; studyPanel <= bluringviewer.StudyPanelList.Count; studyPanel++)
                    for (int thumbnail = 1; thumbnail <= BluRingViewer.NumberOfThumbnailsInStudyPanel(studyPanel); thumbnail++)
                        if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(studyPanel, thumbnail))
                            result.steps[ExecutedSteps].AddFailStatusList("There are thumbnails that are in-focus at study panel " + studyPanel + "at thumbnail" + thumbnail);
                        else
                            result.steps[ExecutedSteps].AddPassStatusList();

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();


                //Step 6 
                //Double - click the 2nd thumbnail in the 2nd study panel(2nd from left-side).
                //if (!(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("Chrome")))        
                bluringviewer.ClickOnThumbnailsInStudyPanel(2, 2, true, true);              
                //else
                //    ClickOnThumbnailsInStudyPanel(2, 2, true, false);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                if (!study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[0]))
                    result.steps[ExecutedSteps].AddFailStatusList("The 3rd viewport of the primary study panel displays the series from the 2nd study panel prior / The top-left image text block has Non - Primary indicator at the bottom for the 3rd viewport of the primary study panel/ The other viewports except 3rd view port of the primary study panel have the Non - Primary indicator. ");

                if (bluringviewer.VerifyViewPortIsActive(ViewPortwebObject: BasePage.Driver.FindElement(By.CssSelector(bluringviewer.SetViewPort1(1, 3)))))
                    result.steps[ExecutedSteps].AddPassStatusList();
                else
                    result.steps[ExecutedSteps].AddFailStatusList("3 rd view port of the primary panel is not active");


                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                Boolean Step_6_2 = study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[1]);

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                Boolean Step_6_3 = study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[2], 3, 1);

                //The in-focus (thick blue-border) thumbnail is the 2nd thumbnail of the 2nd study panel.
                if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(2, 2))
                    result.steps[ExecutedSteps].AddPassStatusList("The in-focus (thick blue-border) thumbnail is the 2nd thumbnail of the 2nd study panel.");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The in-focus (thick blue-border) thumbnail is not the 2nd thumbnail of the 2nd study panel.");

                if (!(Step_6_2 && Step_6_3))
                    result.steps[ExecutedSteps].AddFailStatusList("May All viewports in the 2nd and 3rd study panel not have the Non - Primary indicator.");

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();


                //Step 7 
                //Click on the 2nd viewport of the 3rd study panel, to make it active.

                if (bluringviewer.VerifyViewPortIsActive(ViewPortwebObject: bluringviewer.ClickOnViewPort(3, 2)))
                    result.steps[++ExecutedSteps].AddPassStatusList("The 2nd viewport of the 3rd study panel is active (it has a blue border around it).");
                else
                    result.steps[++ExecutedSteps].AddFailStatusList("The 2nd viewport of the 3rd study panell is not active (it has a blue border around it).");

                for (int studyPanel = 1; studyPanel <= bluringviewer.StudyPanelList.Count; studyPanel++)
                    for (int thumbnail = 1; thumbnail <= BluRingViewer.NumberOfThumbnailsInStudyPanel(studyPanel); thumbnail++)
                        if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(studyPanel, thumbnail))
                            result.steps[ExecutedSteps].AddFailStatusList("There are thumbnails that are in-focus at study panel " + studyPanel + "at thumbnail " + thumbnail);

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();


                //if (VerifyViewPortIsActive(ViewPortwebObject: ClickOnViewPort(3, 2)) && VerifyThumbnailsInStudyPanelIsActive(3, 2))
                //    result.steps[++ExecutedSteps].StepPass("The 2nd viewport of the 3rd study panel is active (it has a blue border around it).The 2nd thumbnail of the 3 study panel is in-focus(it has a thick blue border around it).");
                //else
                //    result.steps[++ExecutedSteps].StepFail("The 2nd viewport of the 3rd study panel is not active (it not have a blue border around it) or The 2nd thumbnail of the 3 study panel is not in-focus(it not have a thick blue border around it).");


                //Step 8
                //Double-click the 2nd thumbnail of the 1st (primary) study panel (to add a primary series into a prior study panel).
                //if (!(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("Chrome")))
                bluringviewer.ClickOnThumbnailsInStudyPanel(1, 2, true, true);
                //else
                //    ClickOnThumbnailsInStudyPanel(1, 2, true, false);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                if (study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[2]))
                    result.steps[ExecutedSteps].AddPassStatusList("The 2nd viewport of the 3rd study panel displays the series from the 2nd thumbnail of the primary study panel (the 04-Feb-2000 study),The top-left image text block does not have Non - Primary indicator.");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The 2nd viewport of the 3rd study panel not displays the series from the 2nd thumbnail of the primary study panel (the 04-Feb-2000 study),The top-left image text block  have Non - Primary indicator.");

                if (bluringviewer.VerifyViewPortIsActive(ViewPortwebObject: bluringviewer.ClickOnViewPort(3, 2)))
                    result.steps[ExecutedSteps].AddPassStatusList("The 2nd viewport of the 3rd study panel is active (it has a blue border around it).");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The 2nd viewport of the 3rd study panel is not active (it not have a blue border around it) ");

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                if (study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[1]))
                    result.steps[ExecutedSteps].AddPassStatusList("All viewports contain series belonging to the 2nds study and contains the Non - Primary indicator (i.e., this study panel contains no foreign series).");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The 2nd study panel:- All viewports contain series belonging to the 2nds study and not contains the Non-Primary indicator(i.e., this study panel contains no foreign series).");

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                if (study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[0], 3, 1))
                    result.steps[ExecutedSteps].AddPassStatusList("The 2nd study panel:- All viewports contain series belonging to the 2nds study and contains the Non-Primary indicator(i.e., this study panel contains no foreign series).");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The 2nd study panel:- All viewports contain series belonging to the 2nds study and not contains the Non-Primary indicator(i.e., this study panel contains no foreign series).");

                if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 2))
                    result.steps[ExecutedSteps].AddPassStatusList("The in-focus (thick blue-border) thumbnail is the 2nd thumbnail of the primary study panel (since the current active viewport in the 3rd study panel contains this thumbnail).");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The in-focus (thick blue-border) thumbnail is not the 2nd thumbnail of the primary study panel");


                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();


                //Step 9 - Click on the 4th viewport of the primary study panel.
                if (bluringviewer.VerifyViewPortIsActive(ViewPortwebObject: bluringviewer.ClickOnViewPort(1, 4)))
                    result.steps[++ExecutedSteps].AddPassStatusList("The 4th viewport of the primary panel is active (it has a blue border around it).");
                else
                    result.steps[++ExecutedSteps].AddFailStatusList("The 4th viewport of the primary panel is not active (it has a blue border around it).");

                for (int studyPanel = 1; studyPanel <= bluringviewer.StudyPanelList.Count; studyPanel++)
                    for (int thumbnail = 1; thumbnail <= BluRingViewer.NumberOfThumbnailsInStudyPanel(studyPanel); thumbnail++)
                        if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(studyPanel, thumbnail))
                            result.steps[ExecutedSteps].AddFailStatusList("There are thumbnails that are in-focus at study panel " + studyPanel + "at thumbnail " + thumbnail);

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();


                // Step 10
                //Double-click the 1st thumbnail in the 3rd study panel.
                //if (!(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("Chrome")))
                bluringviewer.ClickOnThumbnailsInStudyPanel(3, 1, true,true);
                //else
                //    ClickOnThumbnailsInStudyPanel(3, 1, true,false);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                if (study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[2]))
                    result.steps[ExecutedSteps].AddPassStatusList("The 2nd viewport of the 3rd study panel displays the series from the 2nd thumbnail of the primary study panel (the 04-Feb-2000 study),The top-left image text block does not have Non - Primary indicator.");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The 2nd viewport of the 3rd study panel not displays the series from the 2nd thumbnail of the primary study panel (the 04-Feb-2000 study),The top-left image text block  have Non - Primary indicator.");

                // No viewport is active in study panel 3.
                for (int viewport = 1; viewport <= bluringviewer.GetViewPortCount(3); viewport++)
                    if (bluringviewer.VerifyViewPortIsActive(3, viewport))
                        result.steps[ExecutedSteps].AddFailStatusList("The View Port " + viewport + " of the 3 study panel is active ");
                    else
                        result.steps[ExecutedSteps].AddPassStatusList();

                if (!BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(3, 1))
                    result.steps[ExecutedSteps].AddFailStatusList("The 1 thumbnails are not in-focus at study panel 3 ");

                //The 2nd study panel:
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                if (study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[1]))
                    result.steps[ExecutedSteps].AddPassStatusList();
                else
                    result.steps[ExecutedSteps].AddFailStatusList();

                // No Thumbnail and view port active at 2 study panel
                for (int thumbnail = 1; thumbnail <= BluRingViewer.NumberOfThumbnailsInStudyPanel(1); thumbnail++)
                    if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(2, thumbnail))
                        result.steps[ExecutedSteps].AddFailStatusList("There are thumbnails that are in-focus at study panel " + 1 + "at thumbnail " + thumbnail);
                    else
                        result.steps[ExecutedSteps].AddPassStatusList();

                for (int viewport = 1; viewport <= bluringviewer.GetViewPortCount(2); viewport++)
                    if (bluringviewer.VerifyViewPortIsActive(2, viewport))
                        result.steps[ExecutedSteps].AddFailStatusList("The View Port " + viewport + " of the 2 study panel is active ");
                    else
                        result.steps[ExecutedSteps].AddPassStatusList();

                //The primary(1st) study panel
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                if (study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList[0], 3, 1))
                    result.steps[ExecutedSteps].AddPassStatusList();
                else
                    result.steps[ExecutedSteps].AddFailStatusList();

                if (bluringviewer.VerifyViewPortIsActive(1,4))
                    result.steps[ExecutedSteps].AddPassStatusList("The View port 4 of studypanel 1 is  active");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The View port 4 of studypanel 1 is not active");

                if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 4))
                    result.steps[ExecutedSteps].AddFailStatusList("The 4th viewport is active (has blue border) at primary study panel");

                for (int thumbnail = 1; thumbnail <= BluRingViewer.NumberOfThumbnailsInStudyPanel(); thumbnail++)
                    if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, thumbnail))
                        result.steps[ExecutedSteps].AddFailStatusList("There are thumbnails that are in-focus at study panel " + 1 + "at thumbnail " + thumbnail);
                    else
                        result.steps[ExecutedSteps].AddPassStatusList();

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();

                
                //Step 11
                //Click on the 2nd viewport of the 3rd study panel.
                if (bluringviewer.VerifyViewPortIsActive(ViewPortwebObject: bluringviewer.ClickOnViewPort(3, 2)) && BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 2))
                    result.steps[++ExecutedSteps].StepPass("The 2nd viewport of the 3rd study panel is active (it has a blue border around it).The 2nd thumbnail of the primary study panel is in-focus(it has a thick blue border around it).");
                else
                    result.steps[++ExecutedSteps].StepFail("The 2nd viewport of the 3rd study panel is not active (it not have a blue border around it) or The 2nd thumbnail of the primary study panel is not in-focus(it not have a thick blue border around it).");


                //Step 12
                //Double-click the 3rd thumbnail of the 2nd study panel (loading a foreign series into a prior study panel containing a series in the viewport).
                //if (!(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("Chrome")))
                bluringviewer.ClickOnThumbnailsInStudyPanel(2, 3, true, true);
                //else
                //    ClickOnThumbnailsInStudyPanel(2, 3, true);

                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                if (study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[2]))
                    result.steps[ExecutedSteps].AddPassStatusList("The 2nd viewport of the 3rd study panel is replaced with the series from the 3rd thumbnail of the 2nd study panel (the 24-Feb-2000 study).");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The 2nd viewport of the 3rd study panel is not replaced with the series from the 3rd thumbnail of the 2nd study panel (the 24-Feb-2000 study).");

                if (bluringviewer.VerifyViewPortIsActive(3, 2))
                    result.steps[ExecutedSteps].AddPassStatusList("The 2nd viewport of the 3rd study panel is active (it has a blue border around it)");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The 2nd viewport of the 3rd study panel is not active (it not have a blue border around it) ");

                if (!BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(3, 1))
                    result.steps[ExecutedSteps].AddPassStatusList("the first thumbnail of the 3rd study panel is not in-focus (no blue border)");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("the first thumbnail of the 3rd study panel is not in-focus (no blue border)");

                //The 2nd study panel:
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                if (study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[1]))
                    result.steps[ExecutedSteps].AddPassStatusList();
                else
                    result.steps[ExecutedSteps].AddFailStatusList("All viewports contain series belonging to the 2nds study and not contains the Non-Primary indicator(i.e., this study panel contains no foreign series).");

                if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(2, 3))
                    result.steps[ExecutedSteps].AddPassStatusList("The 3rd thumbnail of 2nd study panel is in-focus (has thick blue border).");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The 3rd thumbnail of 2nd study panel is not in-focus  (no blue border)");

                //The primary (1st) study panel:
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                if (study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[0], 3, 1))
                    result.steps[ExecutedSteps].AddPassStatusList();
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The 3rd and 4th viewport of the primary study panel not still contains the foreign series and not contains the Non - Primary indicator.");

                for (int thumbnail = 1; thumbnail <= BluRingViewer.NumberOfThumbnailsInStudyPanel(); thumbnail++)
                    if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, thumbnail))
                        result.steps[ExecutedSteps].AddFailStatusList("There are thumbnails that are in-focus at study panel " + 1 + "at thumbnail " + thumbnail);
                    else
                        result.steps[ExecutedSteps].AddPassStatusList();

                for (int thumbnail = 1; thumbnail <= BluRingViewer.NumberOfThumbnailsInStudyPanel() &&  thumbnail < 3; thumbnail++)
                    if (BluRingViewer.VerifyThumbnailsInStudyPanelIsVisible(1, thumbnail))
                        result.steps[ExecutedSteps].AddPassStatusList();
                    else
                        result.steps[ExecutedSteps].AddFailStatusList("There are thumbnails " + thumbnail + " is not visible at study panel and not have the white border");

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();


                //Step 13 - Click on the 3rd viewport of the 3rd study panel. 
                if (bluringviewer.VerifyViewPortIsActive(ViewPortwebObject: bluringviewer.ClickOnViewPort(3, 3)))
                    result.steps[++ExecutedSteps].AddPassStatusList("The 3rd viewport of the 3rd study panel is active (it has a blue border around it).");
                else
                    result.steps[++ExecutedSteps].AddFailStatusList("The 3rd viewport of the 3rd study panel is not active (it not have a blue border around it).");

                for (int studyPanel = 1; studyPanel <= bluringviewer.StudyPanelList.Count; studyPanel++)
                    for (int thumbnail = 1; thumbnail <= BluRingViewer.NumberOfThumbnailsInStudyPanel(studyPanel); thumbnail++)
                        if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(studyPanel, thumbnail))
                            result.steps[ExecutedSteps].AddFailStatusList("There are thumbnails that are in-focus at study panel " + studyPanel + "at thumbnail " + thumbnail);

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();

                //Step 14
                //Double - click the 4th thumbnail of the 2nd study panel(Foreign series in a prior study panel, into an empty viewport scenario)
                //Click next on thumbnail bar to show the 4th thumbnail (at time may go out of visible area)
                try
                {
                    IWebElement thumbnailBarNext = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ThumbnailNavNext(2)));
                    thumbnailBarNext.Click();
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Step 14 - 'Next' click on thumbnail bar in study panel 2 failed. Ex - " + ex);
                }
                //if (!(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("Chrome")))
                bluringviewer.ClickOnThumbnailsInStudyPanel(2, 4, true, true);
                //else
                //    ClickOnThumbnailsInStudyPanel(2, 4, true);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                if (study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[2]))
                    result.steps[ExecutedSteps].AddPassStatusList();
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The 2nd viewport of the 3rd study panel not has the series from the 3rd thumbnail of the 2nd study panel(the 24 - Feb - 2000 study) any may the top - left image text block not contains the Non-PrimarY indicator.");

                if (bluringviewer.VerifyViewPortIsActive(3, 3))
                    result.steps[ExecutedSteps].AddPassStatusList("The 3rd viewport of the 3rd study panel is active (it has a blue border around it).");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The 3rd viewport of the 3rd study panel is not active (it not have a blue border around it).");

                if (!BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(3, 1))
                    result.steps[ExecutedSteps].AddPassStatusList("the first thumbnail of the 3rd study panel is not in-focus (no blue border),");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("the first thumbnail of the 3rd study panel is  in-focus (blue border),");

                if (!BluRingViewer.VerifyThumbnailsInStudyPanelIsVisible(3, 1))
                    result.steps[ExecutedSteps].AddFailStatusList("There are thumbnails " + 1 + " is not visible at study panel 3 and not have the white border");
                else
                    result.steps[ExecutedSteps].AddPassStatusList();


                //The 2nd study panel:
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                if (study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[1]))
                    result.steps[ExecutedSteps].AddPassStatusList();
                else
                    result.steps[ExecutedSteps].AddFailStatusList();

                if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(2, 4))
                    result.steps[ExecutedSteps].AddPassStatusList("The 4rd thumbnail of 2nd study panel is in-focus (has thick blue border).");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The 4rd thumbnail of 2nd study panel is not in-focus  (no blue border)");

                //The primary (1st) study panel:
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                if (study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[0], 3, 1))
                    result.steps[ExecutedSteps].AddPassStatusList();
                else
                    result.steps[ExecutedSteps].AddFailStatusList();

                for (int thumbnail = 1; thumbnail <= BluRingViewer.NumberOfThumbnailsInStudyPanel(); thumbnail++)
                    if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, thumbnail))
                        result.steps[ExecutedSteps].AddFailStatusList("There are thumbnails that are in-focus at study panel " + 1 + "at thumbnail" + thumbnail);

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();

                //Step 15
                //Select on the SHOW/HIDE > HIDE IMAGE TEXT from the global tool bar.
                BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ShowHideTool)).Click();
                BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ShowHideDropdown))[0].Click();

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                if (study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[2]))
                    result.steps[ExecutedSteps].AddPassStatusList();
                else
                    result.steps[ExecutedSteps].AddFailStatusList();

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                if (study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[1]))
                    result.steps[ExecutedSteps].AddPassStatusList();
                else
                    result.steps[ExecutedSteps].AddFailStatusList();

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                if (study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[0], 3, 1))
                    result.steps[ExecutedSteps].AddPassStatusList();
                else
                    result.steps[ExecutedSteps].AddFailStatusList();

                if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(2, 4) && bluringviewer.VerifyViewPortIsActive(3, 3))
                    result.steps[ExecutedSteps].AddPassStatusList("The active viewport and in-focus thumbnail remains the same as before the hidden text.");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The active viewport and in-focus thumbnail does not remains the same as before the hidden text.");

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();


                //Step -16
                //Select on the SHOW/HIDE > SHOW IMAGE TEXT from the global tool bar.
                BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ShowHideTool)).Click();
                BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ShowHideDropdown))[0].Click();
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                if (study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[2]))
                    result.steps[ExecutedSteps].AddPassStatusList();
                else
                    result.steps[ExecutedSteps].AddFailStatusList();

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                if (study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[1]))
                    result.steps[ExecutedSteps].AddPassStatusList();
                else
                    result.steps[ExecutedSteps].AddFailStatusList();

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                if (study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[0], 3, 1))
                    result.steps[ExecutedSteps].AddPassStatusList();
                else
                    result.steps[ExecutedSteps].AddFailStatusList();

                if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(2, 4) && bluringviewer.VerifyViewPortIsActive(3, 3))
                    result.steps[ExecutedSteps].AddPassStatusList("The active viewport and in-focus thumbnail remains the same as before the hidden text.");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The active viewport and in-focus thumbnail does not remains the same as before the hidden text.");

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();

                bluringviewer.CloseBluRingViewer();

                //Step-17
                study.SelectStudy("Accession", Accession);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();

                //Verify that only one study is opened.
                if (BluRingViewer.TotalStudyPanel() == 1)
                    result.steps[++ExecutedSteps].AddPassStatusList("BlueRing viewer opens with one study in the Study Panel");
                else
                    result.steps[++ExecutedSteps].AddFailStatusList("BlueRing viewer not opens with one study in the Study Panel");

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[0]))
                    result.steps[ExecutedSteps].AddPassStatusList();
                else
                    result.steps[ExecutedSteps].AddFailStatusList();

                if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 1) && bluringviewer.VerifyViewPortIsActive(1, 1))
                    result.steps[ExecutedSteps].AddPassStatusList("The current study in the Exam List is highlighted (blue border around the study info rectangle)");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The current study in the Exam List is highlighted (blue border around the study info rectangle)");

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();

                bluringviewer.CloseBluRingViewer();

                //Logout
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);

                Reusable.Generic.Logger.Instance.InfoLog("Overall Test status--" + result.status);
                //Return Result
                return result;
                //------------End of script---

            }
            catch (Exception e)
            {
                //Log Exception
                Reusable.Generic.Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Reusable.Generic.Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary> 
        /// 138728   - Foreign Series:Drag-and-drop foreign series from unloaded study in Exam List
        /// </summary>
        ///
        public TestCaseResult Test_161684(string testid, string teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;

            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            bool thumbnailSplittingChanged = false;
            bool autoThumbnailSplitUS = false;
            bool seriesThumbnailSplitUS = false;

            try
            {
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                string Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                string LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                string StudyDateTime = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDateTime");
                string[] studydateTime = StudyDateTime.Split('@');
               
                string Prior1Date = studydateTime[0].Split(' ')[0];
                string Prior1Time = studydateTime[0].Split(' ')[1];
                string Prior2Date = studydateTime[1].Split(' ')[0];
                string Prior2Time = studydateTime[0].Split(' ')[1];
                string PrimaryStudyDate = studydateTime[2].Split(' ')[0];
                string PrimaryStudyTime = studydateTime[2].Split(' ')[1];

                string Datasource = login.GetHostName(Config.EA91);

                //Step 1
                //Launch iCA and login as "Administrator", password "Administrator" in Chrome (Browser)   
                //Login success
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);
                result.steps[++ExecutedSteps].StepPass();
                //Set the thumbnail splitting of US to image in user prefernce. 
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);        
                userpref.ModalityDropdown().SelectByText("US");
                autoThumbnailSplitUS = userpref.ThumbnailSplittingAutoRadioBtn().Selected;
                if (!autoThumbnailSplitUS)
                    seriesThumbnailSplitUS = userpref.ThumbnailSplittingSeriesRadioBtn().Selected;
                userpref.ThumbnailSplittingImageRadioBtn().Click();  
                userpref.CloseUserPreferences();
                thumbnailSplittingChanged = true;

                //Navigate to Studies tab and search for Accession =ACC01
                //Study should appear in the search grid
                Studies study = (Studies)login.Navigate("Studies");               
                study.SearchStudy( AccessionNo: Accession, Datasource : Datasource); //Step 2
                result.steps[++ExecutedSteps].StepPass();  //Step 2 

                //Step 3
                study.SelectStudy("Accession", Accession);
                //Launch the study in new viewer(BlueRing viewer)
                BluRingViewer bluringviewer = BluRingViewer.LaunchBluRingViewer();

                //Verify that only one study is opened.
                if (BluRingViewer.TotalStudyPanel() == 1)
                    result.steps[++ExecutedSteps].AddPassStatusList("BlueRing viewer opens with one study in the Study Panel");
                else
                    result.steps[++ExecutedSteps].AddFailStatusList("BlueRing viewer not opens with one study in the Study Panel");

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList[0]))
                    result.steps[ExecutedSteps].AddPassStatusList("This is the primary study, so there are no Primary / Non - Primary text indicators in the top-left corner of the viewports.");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("This is the primary study, so there are Primary / Non - Primary text indicators in the top - left corner of the viewports.");

                //Exam List
                //The current study in the Exam List is highlighted (blue border around the study info rectangle).
                if (bluringviewer.VerifyPriorsHighlightedInExamList(AccessionNumber: Accession))
                    result.steps[ExecutedSteps].AddPassStatusList("The current study in the Exam List is highlighted (blue border around the study info rectangle).");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The current study in the Exam List is not highlighted (blue border around the study info rectangle).");

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();


                //Step 4
                //Click on the 04-Aug-1993 5:22:34 PM study entry from the Exam List to open it up in a new study panel.
                bluringviewer.OpenPriors(StudyDate: Prior1Date);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                //The top-left image text block has "Non-Primary" indicator at the bottom for all the viewports in the prior study panels.
                bool step4ImageCompare_1 = study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[1]);

                if (step4ImageCompare_1 && BluRingViewer.TotalStudyPanel() == 2 && bluringviewer.StudyPanelList.ToArray()[0].Location.X < bluringviewer.StudyPanelList.ToArray()[1].Location.X)
                    result.steps[ExecutedSteps].StepPass("Prior exams are opened in new study panels to the right of the primary study panel and The viewport for this 2nd study panel  not contains 'Non - Primary' indicator in the top-left text block.");
                else
                    result.steps[ExecutedSteps].StepFail("Prior exams are opened in new study panels not to the right of the primary study panel or The viewport for this 2nd study panel  not contains 'Non - Primary' indicator in the top-left text block.");

                //Step 5
                //Click on the Thumbnail Preview icon for the 10-May-2000 study from the Exam List.
                bluringviewer.OpenExamListThumbnailPreview(studyDate: Prior2Date);

                IWebElement ThumbnailPreviewScrollBar = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.ThumbnailPreviewScrollBar));
                if(ThumbnailPreviewScrollBar.Displayed)
                    result.steps[++ExecutedSteps].StepPass("A scroll bar exists in the preview.");
                else
                    result.steps[++ExecutedSteps].StepFail("A scroll bar not  exists  in the preview.");


                //step 6
                if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("firefox")))
                    bluringviewer.DropAndDropThumbnails(thumbnailnumber: 2, viewport: 2, studyPanelNumber: 1, ExamList: true, UseDragDrop: true);
                else
                    bluringviewer.DropAndDropThumbnails(thumbnailnumber: 2, viewport: 2, studyPanelNumber: 1, ExamList: true);
                Thread.Sleep(10000);
                //DropAndDropThumbnails(thumbnailnumber: 2, viewport: 2, studyPanelNumber: 1, ExamList: true);
                //Thread.Sleep(10000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step6ImageCompare = study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[0]);
                if (step6ImageCompare)
                    result.steps[ExecutedSteps].AddPassStatusList("The first viewport contains the same initial image without the Non-Primary indicator. and The second viewport is replaced with the image from the Exam list thumbnail (S1-2). This viewport has the 'Non - Primary' text indicator in the top-left text block.");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The first viewport is not the same initial image without the Non - Primary indicator or  The second viewport is not replaced with the image from the Exam list thumbnail(S1 - 2).This viewport has the 'Non - Primary' text indicator in the top - left text block.");


                if (bluringviewer.VerifyViewPortIsActive(1, 2))
                    result.steps[ExecutedSteps].AddPassStatusList("The 2 view port of the Primary study is active");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The 2 view port of the Primary study is not active");

                if (BluRingViewer.VerifyThumbnailsInStudyPanelIsVisible(1, 1) && (!BluRingViewer.VerifyThumbnailsInStudyPanelIsVisible(1, 2)))
                    result.steps[ExecutedSteps].AddPassStatusList("The primary study panel's thumbnails: 1st thumbnail has white border around it, indicating that it is visible in a viewport and 2nd thumbnail does not have border around it, indicating that it is not visible in a viewport");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The primary study panel's thumbnails: 1st thumbnail has white border around it, indicating that it is visible in a viewpor or 2nd thumbnail have border around it, indicating that it is visible in a viewport");
                //Exam list
                if (BluRingViewer.VerifyThumbnailsInExamList(2, "Active"))
                    result.steps[ExecutedSteps].AddPassStatusList("2nd thumbnail (S1-2) is in-focus (has a thick blue border)");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The primary study panel's thumbnails: 1st thumbnail has white border around it, indicating that it is visible in a viewpor or 2nd thumbnail have border around it, indicating that it is visible in a viewport");

                for (int thumbnail = 1; thumbnail <= BluRingViewer.NumberOfThumbnailsInExamList(); thumbnail++)
                    if (thumbnail != 2)
                        if ((BluRingViewer.VerifyThumbnailsInExamList(thumbnail, "Active")) || (BluRingViewer.VerifyThumbnailsInExamList(thumbnail, "visible")))
                            result.steps[ExecutedSteps].AddFailStatusList("The Thumbnail " + thumbnail + " at thumbnail preview is active or Visible on screen");
                        else
                            result.steps[ExecutedSteps].AddPassStatusList();

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();


                //Step 7
                IList<IWebElement> thumbnailslist = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList_thumbnails));
                int imageFrameNumber = int.Parse(thumbnailslist[1].FindElement(By.CssSelector(BluRingViewer.div_imageFrameNumber)).Text);

                var action = new TestCompleteAction();
                action.MouseScroll(BasePage.FindElementByCss(bluringviewer.SetViewPort1(1,2)), "down", "1").Perform();
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step7ImageCompare_1 = study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[0]);
                if (step7ImageCompare_1)
                    result.steps[ExecutedSteps].AddPassStatusList("The multi-framed image can be viewed by scrolling.");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The multi-framed image is not viewed by scrolling.");

                if(bluringviewer.VerifyThumbnailPercentImagesViewed(thumbnailslist[1].FindElement(By.CssSelector(BluRingViewer.div_thumbnailPercentImagesViewed)), imageFrameNumber, 2))
                    result.steps[ExecutedSteps].AddPassStatusList("The corresponding thumbnail (in the Exam List) has it's % viewed updated according to the number of frames viewed.");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The corresponding thumbnail (in the Exam List) not has it's % viewed updated according to the number of frames viewed.");

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();

                //Step 8
                if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("firefox")))
                    bluringviewer.DropAndDropThumbnails(thumbnailnumber: 3, viewport: 2, studyPanelNumber: 2, ExamList: true, UseDragDrop: true);
                else
                    bluringviewer.DropAndDropThumbnails(thumbnailnumber: 3, viewport: 2, studyPanelNumber: 2, ExamList: true);

                //DropAndDropThumbnails(thumbnailnumber: 3, viewport: 2, studyPanelNumber: 2, ExamList: true);
                Thread.Sleep(10000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step8ImageCompare_1 = study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[1]);
                if (step8ImageCompare_1)
                    result.steps[ExecutedSteps].AddPassStatusList("The first viewport contains the same initial image without the Non-Primary indicator. and The second viewport still contains the foreign image from the Exam list thumbnail (S1-2). This viewport has the 'Non - Primary' text indicator in the top-left text block.");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The first viewport is not the same initial image without the Non - Primary indicator or  The second viewport still not contains the foreign image from the Exam list thumbnail(S1 - 2).This viewport has the 'Non - Primary' text indicator in the top - left text block.");

                if (BluRingViewer.VerifyThumbnailsInStudyPanelIsVisible(1, 1) && (!BluRingViewer.VerifyThumbnailsInStudyPanelIsVisible(1, 2)))
                    result.steps[ExecutedSteps].AddPassStatusList("The primary study panel's thumbnails: 1st thumbnail has white border around it, indicating that it is visible in a viewport and 2nd thumbnail does not have border around it, indicating that it is not visible in a viewport");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The primary study panel's thumbnails: 1st thumbnail has white border around it, indicating that it is visible in a viewpor or 2nd thumbnail have border around it, indicating that it is visible in a viewport");

                //2nd study panel:
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step8ImageCompare_2 = study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[1], 2, 1);
                if (step8ImageCompare_2)
                    result.steps[ExecutedSteps].AddPassStatusList("2nd study panel: The first viewport contains the same initial image without the Comparison indicator. and The second viewport still contains the foreign image from the Exam list thumbnail (S1-2). This viewport has the 'Non - Primary' text indicator in the top-left text block.");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("2nd study panel:The first viewport is not the same initial image without the Comparison or  The second viewport still not contains the foreign image from the Exam list thumbnail(S1 - 2).This viewport has the 'Non - Primary' text indicator in the top - left text block.");

                if (bluringviewer.VerifyViewPortIsActive(2, 2))
                    result.steps[ExecutedSteps].AddPassStatusList("2nd study panel:The 2 Viewport of the 2 Study Panel is Active");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("2nd study panel: The 2 View Port of the 2 Study panle is not Active");

                if (BluRingViewer.VerifyThumbnailsInStudyPanelIsVisible(1, 1) && (!BluRingViewer.VerifyThumbnailsInStudyPanelIsVisible(1, 2)))
                    result.steps[ExecutedSteps].AddPassStatusList("The 2nd study panel's thumbnail bar has one thumbnail with a white border around it: meaning it is visible in a viewport.");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The 2nd study panel's thumbnail bar has one thumbnail with a white border around it: meaning it is visible in a viewport.");

                //Exam List Thumbnail Preview
                if (BluRingViewer.VerifyThumbnailsInExamList(3, "Active") && BluRingViewer.VerifyThumbnailsInExamList(2, "visible"))
                    result.steps[ExecutedSteps].AddPassStatusList("3rd thumbnail (S1-3) is in-focus (has a thick blue border) / 2nd thumbnail (S1-2) has white border around it (meaning it is visible in a viewport)");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("3rd thumbnail (S1-3) is not in-focus (not have a thick blue border) or 2nd thumbnail (S1-2) has not white border around it (meaning it is  not visible in a viewport)");

                for (int thumbnail = 1; thumbnail <= BluRingViewer.NumberOfThumbnailsInExamList(); thumbnail++)
                    if (thumbnail != 2 || thumbnail != 3)
                        if ((BluRingViewer.VerifyThumbnailsInExamList(thumbnail, "Active")) && (BluRingViewer.VerifyThumbnailsInExamList(thumbnail, "visible")))
                            result.steps[ExecutedSteps].AddFailStatusList("The Thumbnail " + thumbnail + " at thumbnail preview is active or Visible on screen");
                        else
                            result.steps[ExecutedSteps].AddPassStatusList("All other thumbnails have no border (not visible).");

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();


                //Step 9
                thumbnailslist = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList_thumbnails));
                 imageFrameNumber = int.Parse(thumbnailslist[2].FindElement(By.CssSelector(BluRingViewer.div_imageFrameNumber)).Text);

                 action = new TestCompleteAction();
                action.MouseScroll(BasePage.FindElementByCss(bluringviewer.SetViewPort1(2, 2)), "down", "1").Perform();
                //studyViewer.Scroll(1, 1, 1, "down", "click");
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step9ImageCompare_1 = study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[0]);
                if (step9ImageCompare_1)
                    result.steps[ExecutedSteps].AddPassStatusList("The multi-framed image can be viewed by scrolling.");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The multi-framed image is not viewed by scrolling.");

                if (bluringviewer.VerifyThumbnailPercentImagesViewed(thumbnailslist[2].FindElement(By.CssSelector(BluRingViewer.div_thumbnailPercentImagesViewed)), imageFrameNumber, 2))
                    result.steps[ExecutedSteps].AddPassStatusList("The corresponding thumbnail (in the Exam List) has it's % viewed updated according to the number of frames viewed.");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The corresponding thumbnail (in the Exam List) not has it's % viewed updated according to the number of frames viewed.");

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();

                //Step 10
                //Click on the Thumbnail Preview icon for the  "04-Aug-1993" 4:35:00 PM study from the Exam List.
                bluringviewer.OpenExamListThumbnailPreview(studyDate: Prior1Date, studyTime: Prior1Time);
                if (bluringviewer.VerifyPriorsHighlightedInExamList(StudyDate: PrimaryStudyDate) && bluringviewer.VerifyPriorsHighlightedInExamList(StudyDate: Prior1Date))
                    result.steps[++ExecutedSteps].StepPass("There are 2 items in the Exam list that have a blue border(meaning they are opened in a study panel): 13 - Jan - 2017 and 04 - Aug - 1993 5:22:34 PM");
                else
                    result.steps[++ExecutedSteps].StepFail("There are 2 items in the Exam list that have not a blue border(meaning they are not opened in a study panel): 13 - Jan - 2017 and 04 - Aug - 1993 5:22:34 PM");

                //Step 11
                if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("firefox")))
                    bluringviewer.DropAndDropThumbnails(thumbnailnumber: 1, viewport: 2, studyPanelNumber: 1, ExamList: true, UseDragDrop: true);
                else
                    bluringviewer.DropAndDropThumbnails(thumbnailnumber: 1, viewport: 2, studyPanelNumber: 1, ExamList: true);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step11ImageCompare_1 = study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[0]);
                if (step11ImageCompare_1)
                    result.steps[ExecutedSteps].AddPassStatusList("The first viewport contains the same initial image without the Non-Primary indicator.- The second viewport is replaced with the image from the Exam list thumbnail(S1).This viewport has the 'Non-Primary' text indicator in the top - left text block.");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The first viewport contains the same initial image without the Non-Primary indicator.- The second viewport is replaced with the image from the Exam list thumbnail(S1).This viewport has the 'Non-Primary' text indicator in the top - left text block.");

                if (BluRingViewer.VerifyThumbnailsInStudyPanelIsVisible(1, 1) && (!BluRingViewer.VerifyThumbnailsInStudyPanelIsVisible(1, 2)))
                    result.steps[ExecutedSteps].AddPassStatusList("The primary study panel's thumbnails: 1st thumbnail has white border around it, indicating that it is visible in a viewport and 2nd thumbnail does not have border around it, indicating that it is not visible in a viewport");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The primary study panel's thumbnails: 1st thumbnail has white border around it, indicating that it is visible in a viewpor or 2nd thumbnail have border around it, indicating that it is visible in a viewport");

                if(bluringviewer.VerifyViewPortIsActive(1, 2))
                    result.steps[ExecutedSteps].AddPassStatusList();
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The 2 viewPort of the Primary panel is active");

                if(BluRingViewer.VerifyThumbnailsInStudyPanelIsVisible(1,1) && !BluRingViewer.VerifyThumbnailsInStudyPanelIsVisible(1,2) && BluRingViewer.VerifyThumbnailsInExamList(1,"Active"))
                    result.steps[ExecutedSteps].AddPassStatusList();
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The primary study panel's thumbnails: 1st thumbnail has white border around it, indicating that it is visible in a viewport. 2nd thumbnail does not have border around it, indicating that it is not visible in a viewport.");

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();

                //Step 12
                //Click on the Thumbnail Preview icon for the primary study in the Exam List (dated 13-Jan-2017)
                bluringviewer.OpenExamListThumbnailPreview(studyDate: PrimaryStudyDate);

                if (BluRingViewer.VerifyThumbnailsInExamList(1, "Visible"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();


                //Step 13
                if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("firefox")))
                    bluringviewer.DropAndDropThumbnails(thumbnailnumber: 2, viewport: 3, studyPanelNumber: 2, ExamList: true, UseDragDrop: true);
                else
                    bluringviewer.DropAndDropThumbnails(thumbnailnumber: 2, viewport: 3, studyPanelNumber: 2, ExamList: true);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step13ImageCompare_1 = study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[0]);
                if (step13ImageCompare_1)
                    result.steps[ExecutedSteps].AddPassStatusList();
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The first viewport contains the same initial image without the Non - Primary indicator.- The second viewport has the foreign series image(S2).This viewport has the 'Non-Primary' text indicator in the top - left text block. ");

                if (BluRingViewer.VerifyThumbnailsInStudyPanelIsVisible(1, 1) && (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1,2)))
                    result.steps[ExecutedSteps].AddPassStatusList("The primary study panel's thumbnails: 1st thumbnail has white border around it, indicating that it is visible in a viewport and 2nd thumbnail does not have border around it, indicating that it is not visible in a viewport");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The primary study panel's thumbnails: 1st thumbnail has white border around it, indicating that it is visible in a viewpor or 2nd thumbnail have border around it, indicating that it is visible in a viewport");

                if(BluRingViewer.VerifyThumbnailsInExamList(1,"Visible") && BluRingViewer.VerifyThumbnailsInExamList(2, "Active"))
                    result.steps[ExecutedSteps].AddPassStatusList("1st thumbnail has white border around it, indicating that it is visible in a viewport.- 2nd thumbnail has thick blue border, indicating that it is in-focus(in an active viewport).");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("1st thumbnail has white border around it, indicating that it is visible in a viewport.- 2nd thumbnail has thick blue border, indicating that it is in-focus(in an active viewport).");

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step13ImageCompare_2 = study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[1],2,1);
                if (step13ImageCompare_2)
                    result.steps[ExecutedSteps].AddPassStatusList();
                else
                    result.steps[ExecutedSteps].AddFailStatusList("Comparision failed at the 3 view port of the 2nd study panel");

                if((!bluringviewer.VerifyViewPortIsActive(2,3)) && BluRingViewer.VerifyThumbnailsInStudyPanelIsVisible(2,1))
                    result.steps[ExecutedSteps].AddFailStatusList("The 3 view port of the 2nd study panel is the not active");

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();

                //Step -14
                //Select on the SHOW/HIDE > HIDE IMAGE TEXT from the global tool bar.
                BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ShowHideTool)).Click();
                BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ShowHideDropdown))[0].Click();
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                if (study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[0]))
                    result.steps[ExecutedSteps].AddPassStatusList();
                else
                    result.steps[ExecutedSteps].AddFailStatusList();

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                if (study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[1]))
                    result.steps[ExecutedSteps].AddPassStatusList();
                else
                    result.steps[ExecutedSteps].AddFailStatusList();

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                if (study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[0], 3, 1))
                    result.steps[ExecutedSteps].AddPassStatusList();
                else
                    result.steps[ExecutedSteps].AddFailStatusList();

                if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 2) && bluringviewer.VerifyViewPortIsActive(2, 3))
                    result.steps[ExecutedSteps].AddPassStatusList("The active viewport and in-focus thumbnail remains the same as before the hidden text.");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The active viewport and in-focus thumbnail does not remains the same as before the hidden text.");

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();

                //Step -15
                //Select on the SHOW/HIDE > SHOW IMAGE TEXT from the global tool bar.
                BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ShowHideTool)).Click();
                BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ShowHideDropdown))[0].Click();
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                if (study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[0]))
                    result.steps[ExecutedSteps].AddPassStatusList();
                else
                    result.steps[ExecutedSteps].AddFailStatusList();

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                if (study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[1]))
                    result.steps[ExecutedSteps].AddPassStatusList();
                else
                    result.steps[ExecutedSteps].AddFailStatusList();

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                if (study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[0], 3, 1))
                    result.steps[ExecutedSteps].AddPassStatusList();
                else
                    result.steps[ExecutedSteps].AddFailStatusList();

                if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 2) && bluringviewer.VerifyViewPortIsActive(2, 3))
                    result.steps[ExecutedSteps].AddPassStatusList("The active viewport and in-focus thumbnail remains the same as before the hidden text.");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The active viewport and in-focus thumbnail does not remains the same as before the hidden text.");

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();

                bluringviewer.CloseBluRingViewer();

                //Step-16
                study.SelectStudy("Accession", Accession);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                ++ExecutedSteps;
                //Verify that only one study is opened.
                if (BluRingViewer.TotalStudyPanel() == 1)
                    result.steps[ExecutedSteps].AddPassStatusList("BlueRing viewer opens with one study in the Study Panel");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("BlueRing viewer not opens with one study in the Study Panel");

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (study.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelList.ToArray()[0]))
                    result.steps[ExecutedSteps].AddPassStatusList();
                else
                    result.steps[ExecutedSteps].AddFailStatusList();

                if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 1) && bluringviewer.VerifyViewPortIsActive(1, 1))
                    result.steps[ExecutedSteps].AddPassStatusList("The current study in the Exam List is highlighted (blue border around the study info rectangle)");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("The current study in the Exam List is highlighted (blue border around the study info rectangle)");

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();

                //Logout
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);

                Reusable.Generic.Logger.Instance.InfoLog("Overall Test status--" + result.status);
                //Return Result
                return result;
                //------------End of script---

            }
            catch (Exception e)
            {
                //Log Exception
                Reusable.Generic.Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Reusable.Generic.Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
            finally
            {
                try
                {
                    //Revert US thumbnail splitting
                    if (thumbnailSplittingChanged)
                    {
                        login.DriverGoTo(login.url);
                        login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                        UserPreferences userpref = new UserPreferences();
                        userpref.OpenUserPreferences();
                        BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                        PageLoadWait.WaitForPageLoad(20);
                        userpref.ModalityDropdown().SelectByText("US");
                        if (autoThumbnailSplitUS)
                        {
                            userpref.ThumbnailSplittingAutoRadioBtn().Click();
                            Reusable.Generic.Logger.Instance.InfoLog("Finally: US thumbnail splitting 'Auto' is selected");
                        }
                        else if (seriesThumbnailSplitUS)
                        {
                            userpref.ThumbnailSplittingSeriesRadioBtn().Click();
                            Reusable.Generic.Logger.Instance.InfoLog("Finally: US thumbnail splitting 'Series' is selected");
                        }               
                        userpref.CloseUserPreferences();
                        Reusable.Generic.Logger.Instance.InfoLog("Finally: US thumbnail splitting reverted successfully");
                    }
                    else
                        Reusable.Generic.Logger.Instance.InfoLog("Finally: US thumbnail splitting has not been changed by this test case");
                }
                catch(Exception ex)
                {
                    Reusable.Generic.Logger.Instance.InfoLog("Finally: Exception while reverting US thumbnail splitting. Ex- " + ex);
                }

            }
        }

        /// <summary>
        /// Foreign Series:Double-click foreign series from unloaded study in Exam List
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161686(String testid, String teststeps, int stepcount)
        {

            // Declare and initialize variables
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            UserManagement usermanagement = new UserManagement();
            DomainManagement domain = new DomainManagement();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String domain1 = "Domain1_" + new Random().Next(1, 1000);
                String role1 = "Role1_" + new Random().Next(1, 1000);
                String rad1 = Config.newUserName + new Random().Next(1, 1000);

                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String[] Accession = AccessionList.Split(':');

                //Precondition                              
                //Create new user
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("DomainManagement");
                domain.CreateDomain(domain1, role1, datasources: null);
                domain.ModalityDropDown().SelectByText("CT");
                domain.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                domain.ClickSaveNewDomain();
                login.Navigate("UserManagement");
                usermanagement.CreateUser(rad1, domain1, role1);
                login.Logout();

                // Step 1 - Login as the tech1 user        
                login.DriverGoTo(login.url);
                login.LoginIConnect(rad1, rad1);
                ExecutedSteps++;

                // Step 2 - Navigate to Studies tab and search for the study
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0]);
                ExecutedSteps++;

                // Step 3 - select the study and launch the study in the bluring viewer
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step3_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                bool step3_2 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", BluRingViewer.div_ActiveExamPanel), "rgba(90, 170, 255, 1)");
                IWebElement firstthumbnail = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1, 1));
                bool step3_3 = viewer.VerifyBordorColor(firstthumbnail, "rgba(90, 170, 255, 1)");
                if (step3_1 && step3_2)
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

                // Step - 4 - open another study in the new study panel
                viewer.OpenPriors(2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                viewer.SetViewPort(0, 2);
                bool step4_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                bool step4_2 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1, 2)), "rgba(90, 170, 255, 1)") &&
                                viewer.verifyBackgroundColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1, 2)), "rgba(90, 170, 255, 1)");
                bool step4_3 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", BluRingViewer.div_relatedStudy + ":nth-of-type(1) " + BluRingViewer.div_ActiveExamPanel), "rgba(90, 170, 255, 1)");
                bool step4_4 = false;
                if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer")))
                {                    
                    step4_4 = viewer.GetElement("cssselector", BluRingViewer.div_relatedStudy + ":nth-of-type(3) " + BluRingViewer.div_ActiveExamPanel).GetCssValue("border-color").Equals("#5aaaff");

                }
                else
                {
                    step4_4 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", BluRingViewer.div_relatedStudy + ":nth-of-type(3) " + BluRingViewer.div_ActiveExamPanel), "rgba(255, 255, 255, 1)");
                }
                                
                new Actions(BasePage.Driver).MoveToElement(viewer.GetElement("cssselector", BluRingViewer.div_thumbnailcontainer)).Build().Perform();
                bool step4_5 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", BluRingViewer.div_relatedStudy + ":nth-of-type(3) " + BluRingViewer.div_ActiveExamPanel), "rgba(90, 170, 255, 1)");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step4_6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.GetViewportCss(1, 0)));
                if (step4_1 && step4_2 && step4_3 && step4_4 && step4_5 && step4_6)
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

                // Step 5 - Click on the Preview Icon of (10-May-2000) study in the Examlist
                viewer.ClickExamListThumbnailIcon("10-May-2000");
                var step5_1 = viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_relatedStudy + ":nth-of-type(2)" + " " + BluRingViewer.div_examListThumbnailContainer));
                //var step5_2 = viewer.IsVerticalScrollBarPresent(viewer.GetElement("cssselector", BluRingViewer.div_thumbnailContainerExamList));
                //var step5_2 = viewer.IsVerticalScrollBarPresent(viewer.GetElement("cssselector", viewer.GetExamListThumbnailContainerCss(2)));
                bool step5_3 = true;
                IList<IWebElement> examListThumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_relatedStudy + ":nth-of-type(2) " + BluRingViewer.div_examListThumbnailImages));
                for (int i = 0; i < examListThumbnails.Count; i++)
                {
                    IWebElement element = examListThumbnails.ElementAt(i).FindElement(By.XPath(".."));
                    if ((element.GetAttribute("class").Equals("thumbnailOuterDiv")))
                    {
                        if (!viewer.VerifyBordorColor(element, "rgba(0, 0, 0, 1)"))
                        {
                            step5_3 = false;
                        }
                    }
                }
                if (step5_1 && step5_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step 6 - Click on the second viewport of study panel 1
                viewer.Click("cssselector", viewer.GetViewportCss(1, 1));
                IWebElement firstStudySecondViewport = viewer.GetElement("cssselector", viewer.GetViewportCss(1, 1)).FindElement(By.XPath(".."));
                bool step6_1 = viewer.VerifyBordorColor(firstStudySecondViewport, "rgba(90, 170, 255, 1)");
                bool step6_2 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(2, 1)), "rgba(90, 170, 255, 1)") &&
                    viewer.verifyBackgroundColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(2, 1)), "rgba(90, 170, 255, 1)");
                if (step6_1 && step6_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step 7 - Double click the 2 nd thumbnail from Exam List (10-May-2000)
                TestCompleteAction action = new TestCompleteAction();
                if (!(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("Chrome")))
                {
                    action.DoubleClick(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(2, 2)));
                }
                else
                {
                    viewer.Doubleclick("cssselector", viewer.GetExamListThumbnailCss(2, 2));
                }
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step7_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                bool step7_2 = viewer.VerifyBordorColor(BasePage.Driver.FindElement(By.CssSelector(viewer.GetStudyPanelThumbnailCss(2, 1))), "rgba(0, 0, 0, 1)");
                bool step7_3 = viewer.VerifyBordorColor(BasePage.Driver.FindElement(By.CssSelector(viewer.GetStudyPanelThumbnailCss(1, 1))), "rgba(255, 255, 255, 1)");
                bool step7_4 = viewer.VerifyBordorColor(BasePage.Driver.FindElement(By.CssSelector(viewer.GetExamListThumbnailCss(2, 2))), "rgba(90, 170, 255, 1)") &&
                                viewer.verifyBackgroundColor(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(2, 2)), "rgba(90, 170, 255, 1)");
                bool step7_5 = false;
                for (int i = 0; i < examListThumbnails.Count; i++)
                {
                    IWebElement element = examListThumbnails.ElementAt(i).FindElement(By.XPath(".."));
                    if ((element.GetAttribute("class").Equals("thumbnailOuterDiv")))
                    {
                        if (viewer.VerifyBordorColor(element, "rgba(0, 0, 0, 1)"))
                        {
                            step7_5 = true;
                        }
                    }
                }
                if (step7_1 && step7_2 && step7_3 && step7_4 && step7_5)
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

                // Step 8 - Scroll through the frames of the second viewport in the study panel 1
                viewer.SetViewPort(1, 1);
                IWebElement Activeviewport = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));                
                action.MouseScroll(Activeviewport, "down", "1");
                Activeviewport = BasePage.Driver.FindElement(By.CssSelector(viewer.GetExamListThumbnailCss(2, 2) + " " + BluRingViewer.div_thumbnailPercentImagesViewed));
                bool step8 = viewer.VerifyThumbnailPercentImagesViewed(Activeviewport, 29, 2);
                if (step8)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step 9 - Make the second viewport of the study panel 2 active
                viewer.Click("cssselector", viewer.GetViewportCss(2, 1));
                IWebElement secondPanelSecondViewport = viewer.GetElement("cssselector", viewer.GetViewportCss(2, 1)).FindElement(By.XPath(".."));
                bool step9_1 = viewer.VerifyBordorColor(secondPanelSecondViewport, "rgba(90, 170, 255, 1)");
                bool step9_2 = true;
                IList<IWebElement> thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));
                int thumbnailsCount = thumbnails.Count;
                IWebElement thumbnail;
                for (int i = 1; i < thumbnailsCount; i++)
                {
                    thumbnail = thumbnails.ElementAt(i).FindElement(By.XPath(".."));
                    if (viewer.VerifyBordorColor(thumbnail, "rgba(90, 170, 255, 1)") &&
                        viewer.verifyBackgroundColor(thumbnail, "rgba(90, 170, 255, 1)"))
                    {
                        step9_2 = false;
                    }
                }
                viewer.ClickExamListThumbnailIcon("04-Aug-1993");
                bool step9_3 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(3, 1)), "rgba(90, 170, 255, 1)") &&
                    viewer.verifyBackgroundColor(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(3, 1)), "rgba(90, 170, 255, 1)");
                viewer.ClickExamListThumbnailIcon("13-Jan-2017");
                bool step9_4 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(1, 1)), "rgba(90, 170, 255, 1)") &&
                    viewer.verifyBackgroundColor(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(1, 1)), "rgba(90, 170, 255, 1)");
                bool step9_5 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(1, 2)), "rgba(90, 170, 255, 1)") &&
                    viewer.verifyBackgroundColor(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(1, 2)), "rgba(90, 170, 255, 1)");
                if (step9_1 && step9_2 && (!step9_3) && (!step9_4) && (!step9_5))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 10 - Double click on the third thumbnail of 10-May-2000 Exam List thumbnail preview screen 
                viewer.ClickExamListThumbnailIcon("10-May-2000");
                if (!(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("chrome")))
                {
                    action.DoubleClick(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(2, 3)));
                }
                else
                {
                    viewer.Doubleclick("cssselector", viewer.GetExamListThumbnailCss(2, 3));
                }                
                bool step10_1 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1, 1)), "rgba(255, 255, 255, 1)");
                bool step10_2 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(2, 1)), "rgba(0, 0, 0, 1)");
                bool step10_3 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1, 2)), "rgba(255, 255, 255, 1)");
                bool step10_4 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(2, 3)), "rgba(90, 170, 255, 1)") &&
                    viewer.verifyBackgroundColor(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(2, 3)), "rgba(90, 170, 255, 1)");
                bool step10_5 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(2, 2)), "rgba(255, 255, 255, 1)");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step10_6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step10_7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + ":nth-of-type(2) " + BluRingViewer.div_compositeViewer), totalImageCount: 2, IsFinal: 1);
                bool step10_8 = true;
                for (int i = 0; i < examListThumbnails.Count; i++)
                {
                    IWebElement element = examListThumbnails.ElementAt(i).FindElement(By.XPath(".."));
                    if (element.GetAttribute("class").Equals("thumbnailOuterDiv"))
                    {

                        if (!viewer.VerifyBordorColor(element, "rgba(0, 0, 0, 1)"))
                        {
                            step10_8 = false;
                        }
                    }
                }
                if (step10_1 && step10_2 && step10_3 && step10_4 && step10_5 && step10_6 && step10_7 && step10_8)
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

                // Step 11 - Scroll through the frames of the second viewport in the study panel 2
                viewer.SetViewPort(1, 2);
                IWebElement activeViewport = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                action.MouseScroll(activeViewport, "down", "2");
                activeViewport = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_Examlistdefaultselectedthumbnail + " " + BluRingViewer.div_thumbnailPercentImagesViewed));
                bool step11 = viewer.VerifyThumbnailPercentImagesViewed(activeViewport, 25, 3);
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

                // Step 12 - Click on the Thumbnail Preview icon for the 04-Aug-1993 4:35:00 PM study from the Exam List
                viewer.ClickExamListThumbnailIcon("04-Aug-1993");
                bool step12_1 = viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_relatedStudy + ":nth-of-type(3) " + BluRingViewer.div_examListThumbnailContainer));
                bool step12_2 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(3, 1)), "rgba(255, 255, 255, 1)");
                bool step12_3 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", BluRingViewer.div_relatedStudy + ":nth-of-type(1) " + BluRingViewer.div_ActiveExamPanel), "rgba(90, 170, 255, 1)");
                bool step12_4 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", BluRingViewer.div_relatedStudy + ":nth-of-type(3) " + BluRingViewer.div_ActiveExamPanel), "rgba(90, 170, 255, 1)");
                if (step12_1 && step12_2 && step12_3 && step12_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step 13 - Click on the 2nd viewport of the 1st study panel
                viewer.Click("cssselector", viewer.GetViewportCss(1, 1));
                bool step13_1 = viewer.VerifyBordorColor(firstStudySecondViewport, "rgba(90, 170, 255, 1)");
                bool step13_2 = true;
                IList<IWebElement> firstStudyPanelThumbnail = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-of-type(1) " + BluRingViewer.div_studyPanelThumbnailImages));
                int firstStudyPanelThumbnailsCount = firstStudyPanelThumbnail.Count;
                IWebElement Thumbnail;
                for (int i = 1; i < firstStudyPanelThumbnailsCount; i++)
                {
                    Thumbnail = firstStudyPanelThumbnail.ElementAt(i).FindElement(By.XPath(".."));
                    if (viewer.VerifyBordorColor(Thumbnail, "rgba(90, 170, 255, 1)") && viewer.verifyBackgroundColor(Thumbnail, "rgba(90, 170, 255, 1)"))
                    {
                        step13_2 = false;
                    }
                }
                if (step13_1 && step13_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step 14 - Double-click the 1st thumbnail from the 04-Aug-1993 4:35:00 PM Exam List thumbnail 
                if (!(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("chrome")))
                {
                    action.DoubleClick(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(3, 1)));
                }
                else
                {
                    viewer.Doubleclick("cssselector", viewer.GetExamListThumbnailCss(3, 1));
                }
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step14_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                bool step14_2 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1, 1)), "rgba(255, 255, 255, 1)");
                bool step14_3 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(2, 1)), "rgba(0, 0, 0, 1)");
                bool step14_4 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(3, 1)), "rgba(90, 170, 255, 1)");
                if (step14_1 && step14_2 && step14_3 && step14_4)
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

                // Step 15 - Click on thumbnail preivew Icon of the 13-Jan-2017
                viewer.ClickExamListThumbnailIcon("13-Jan-2017");
                var step15_1 = viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_relatedStudy + ":nth-of-type(1)" + " " + BluRingViewer.div_examListThumbnailContainer));
                bool step15_2 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(1, 1)), "rgba(255, 255, 255, 1)");
                if (step15_1 && step15_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step 16 - Click on the third viewport in the second study panel
                viewer.Click("cssselector", viewer.GetViewportCss(2, 2));
                IWebElement secondStudyThirdViewport = viewer.GetElement("cssselector", viewer.GetViewportCss(2, 2)).FindElement(By.XPath(".."));
                bool step16_1 = viewer.VerifyBordorColor(secondStudyThirdViewport, "rgba(90, 170, 255, 1)");
                if (step16_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Srep 17 - Double-click the second thumbnail from the 13-Jan-2017 Exam List thumbnail
                if (!(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("chrome")))
                {
                    action.DoubleClick(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(1, 2)));
                }
                else
                {
                    viewer.Doubleclick("cssselector", viewer.GetExamListThumbnailCss(1, 2));
                }
                bool step17_1 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1, 1)), "rgba(255, 255, 255, 1)");
                bool step17_2 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(2, 1)), "rgba(90, 170, 255, 1)") &&
                    viewer.verifyBackgroundColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(2, 1)), "rgba(90, 170, 255, 1)");
                bool step17_3 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1, 2)), "rgba(255, 255, 255, 1)");
                bool step17_4 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(1, 2)), "rgba(90, 170, 255, 1)") &&
                    viewer.verifyBackgroundColor(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(1, 2)), "rgba(90, 170, 255, 1)");
                bool step17_5 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(1, 1)), "rgba(255, 255, 255, 1)");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step17_6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step17_7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + ":nth-of-type(2) " + BluRingViewer.div_compositeViewer), totalImageCount: 2, IsFinal: 1);
                if (step17_1 && step17_2 && step17_3 && step17_4 && step17_5 && step17_6 && step17_7)
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

                // Step 18 - select HIDE IMAGE TEXT from the SHOW/HIDE from the global tool bar
                viewer.SelectShowHideValue("HIDE IMAGE TEXT");
                bool step18_1 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1, 1)), "rgba(255, 255, 255, 1)");
                bool step18_2 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(2, 1)), "rgba(90, 170, 255, 1)");
                bool step18_3 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1, 2)), "rgba(255, 255, 255, 1)");
                bool step18_4 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(1, 2)), "rgba(90, 170, 255, 1)");
                bool step18_5 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(1, 1)), "rgba(255, 255, 255, 1)");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step18_6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step18_7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + ":nth-of-type(2) " + BluRingViewer.div_compositeViewer), totalImageCount: 2, IsFinal: 1);
                if (step18_1 && step18_2 && step18_3 && step18_4 && step18_5 && step18_6 && step18_7)
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

                // Step 19 - select SHOW IMAGE TEXT from the SHOW/HIDE from the global tool bar
                viewer.SelectShowHideValue("SHOW IMAGE TEXT");
                bool step19_1 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1, 1)), "rgba(255, 255, 255, 1)");
                bool step19_2 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(2, 1)), "rgba(90, 170, 255, 1)");
                bool step19_3 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1, 2)), "rgba(255, 255, 255, 1)");
                bool step19_4 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(1, 2)), "rgba(90, 170, 255, 1)");
                bool step19_5 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(1, 1)), "rgba(255, 255, 255, 1)");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step19_6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step19_7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + ":nth-of-type(2) " + BluRingViewer.div_compositeViewer), totalImageCount: 2, IsFinal: 1);
                if (step19_1 && step19_2 && step19_3 && step19_4 && step19_5 && step19_6 && step19_7)
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

                // Step 20 - Close the bluring viewer and open the same study again in the bluring viewer
                viewer.CloseBluRingViewer();
                studies.SelectStudy("Accession", Accession[0]);
                BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step20_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                bool step20_2 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", BluRingViewer.div_relatedStudy + ":nth-of-type(1) " + BluRingViewer.div_ActiveExamPanel), "rgba(90, 170, 255, 1)");
                bool step20_3 = viewer.GetStudyPanelCount().Equals(1);
                if (step20_1 && step20_2 && step20_3)
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

