using System;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Reusable.Generic;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.Putty;
using System.Globalization;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MergeServiceTool;
using TestStack.White.Factory;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.TableItems;
using TestStack.White.InputDevices;
using TestStack.White.WindowsAPI;
using Application = TestStack.White.Application;
using Button = TestStack.White.UIItems.Button;
using CheckBox = TestStack.White.UIItems.CheckBox;
using ComboBox = TestStack.White.UIItems.ListBoxItems.ComboBox;
using ListView = TestStack.White.UIItems.ListView;
using RadioButton = TestStack.White.UIItems.RadioButton;
using TextBox = TestStack.White.UIItems.TextBox;
using Window = TestStack.White.UIItems.WindowItems.Window;
using Panel = TestStack.White.UIItems.Panel;
using TestStack.White.Configuration;

namespace Selenium.Scripts.Tests
{
    class BasicTools : BasePage
    {
        public Login login { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public HPLogin hplogin { get; set; }
        public Configure configure { get; set; }
        public HPHomePage hphomepage { get; set; }
        public string filepath { get; set; }

        public BasicTools(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            configure = new Configure();
            hphomepage = new HPHomePage();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }

        Studies studies = new Studies();
        StudyViewer StudyViewer = new StudyViewer();
        UserPreferences UserPref = new UserPreferences();

        RoleManagement rolemanagement = new RoleManagement();
        UserManagement usermanagement = new UserManagement();
        DomainManagement domainmanagement = new DomainManagement();

        public string EA_91 = "VMSSA-5-38-91";
        public string EA_131 = "VMSSA-4-38-131";
        public string EA_77 = "AUTO-SSA-001";



        /// <summary>
        /// Review Tools
        /// </summary>
        public TestCaseResult Test_27851(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] toollist = { "All in One Tool","Window Level","Zoom","Pan","Line Measurement","Cobb Angle","Add Text",
                                        "Draw Ellipse","Edit Annotations","Cine","Toggle Text","Localizer Line","Link Selected",
                                        "Flip Horizontal","Print View","Save Series","Reset","Series Viewer 1x1","User Preference",
                                        "Download Document","Help","Close"};
                String[] Accession = AccessionList.Split(':');
                String[] PatientID = PatientIDList.Split(':');
                //ConferenceFolder cf;

                //Precondition
                login.LoginIConnect(adminUserName, adminPassword);
                //--Pre condition-- disable connection tool.
                login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(20);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                if (BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Selected == false)
                {
                    BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Click();
                    Logger.Instance.InfoLog("Connection tool CB un-selected successfully");
                }
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();


                /*cf = (ConferenceFolder)login.Navigate("Conference Folders");
                cf.CreateToplevelFolder("h","ph1");*/

                UserPref.OpenUserPreferences();
                UserPref.SwitchToUserPrefFrame();
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div[id='AllInOneSelectionDiv'] select[id='dpLeftButtonFunctions']")));
                new SelectElement(BasePage.Driver.FindElement(By.CssSelector("div[id='AllInOneSelectionDiv'] select[id='dpLeftButtonFunctions']"))).SelectByText("Window Level");
                new SelectElement(BasePage.Driver.FindElement(By.CssSelector("div[id='AllInOneSelectionDiv'] select[id='dpMiddleButtonFunctions']"))).SelectByText("Zoom");
                new SelectElement(BasePage.Driver.FindElement(By.CssSelector("div[id='AllInOneSelectionDiv'] select[id='dpRightButtonFunctions']"))).SelectByText("Pan");
                UserPref.CloseUserPreferences();
                login.Logout();

                //Step 1 - Edit the domain to Enable Print feature.               
                login.LoginIConnect(adminUserName, adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForFrameLoad(20);
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("print", 0);
                domainmanagement.ClickSaveDomain();
                ExecutedSteps++;

                //Step 2 - Load a MR study with multiple series into the viewer 
                studies = (Studies)login.Navigate("Studies");
                //Accession[0]=11665475
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                IWebElement toolbar = BasePage.Driver.FindElement(By.CssSelector("div[id='reviewToolbar']>ul"));
                bool step_2 = studies.CompareImage(result.steps[ExecutedSteps], toolbar);
                if (step_2)
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


                //Step 3 - Hover the mouse over each of them.
                int counter = 0;
                ExecutedSteps++;

                foreach (IWebElement toolgroup in StudyViewer.TopReviewTools())
                {
                    studies.JSMouseHover(toolgroup);
                    IList<IWebElement> toptools = StudyViewer.TopReviewTools();
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(toptools[counter]));

                    if (toptools[counter].GetAttribute("title").Equals(toollist[counter]))
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        Logger.Instance.InfoLog("Tooltip dropdown for tool group '" + counter + "' is not displayed.");
                        break;
                    }
                    counter++;
                }

                //step-4 Visually check the tool buttons.(help and return to study list)
                IList<String> tools = studies.GetReviewToolsFromviewer();
                int count = tools.Count;
                if (tools[count - 1].Equals("Close") && tools[count - 2].Equals("Help"))
                {
                    result.steps[ExecutedSteps++].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps++].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.AllinOneTool);

                //step-5 Hover the mouse over the grouped tools. Select a tool from the dropdown list.

                IList<String> toolstitle = new List<String>();
                ExecutedSteps++;
                int counter1 = 0;
                int j = 0;
                foreach (IWebElement toolgroup in StudyViewer.GroupedReviewTools())
                {
                    studies.JSMouseHover(toolgroup);
                    IList<IWebElement> dropdown = BasePage.Driver.FindElements(By.CssSelector("a[class^='AnchorClass32 toplevel']+ul>li:nth-child(2)"));
                    string title = dropdown[j].GetAttribute("title");
                    if (title.Equals("Calibration Tool"))
                    {

                        title = BasePage.Driver.FindElement(By.CssSelector("a[class^='AnchorClass32 toplevel']+ul>li[title='Transischial Measurement']:nth-child(3)")).GetAttribute("title");
                        ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"div#reviewToolbar a>img[title='" + title + "']\").click()");
                        //continue;
                    }
                    else if (title.Equals("PDF Report"))
                    {
                        j++;
                        continue;
                    }
                    else if (title.Contains("Magnifier"))
                    {
                        StudyViewer.SeriesViewer_1X1().Click();
                        /*title = BasePage.Driver.FindElement(By.CssSelector("a[class^='AnchorClass32 toplevel']+ul>li[title='Magnifier x3']:nth-child(3)")).GetAttribute("title");
                        ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"div#reviewToolbar a>img[title='" + title + "']\").click()");
                        Thread.Sleep(3000);*/
                        title = BasePage.Driver.FindElement(By.CssSelector("a[class^='AnchorClass32 toplevel']+ul>li[title='Zoom']:nth-child(1)")).GetAttribute("title");
                        ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"div#reviewToolbar a>img[title='" + title + "']\").click()");

                        /*Thread.Sleep(4000);
                        StudyViewer.CloseZoom();*/

                    }
                    else
                    {
                        ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"div#reviewToolbar a>img[title='" + title + "']\").click()");
                        //dropdown[j].Click();
                        //j++;
                    }
                    IWebElement columns = toolgroup.FindElement(By.CssSelector("a[class^='AnchorClass32 toplevel']>img"));
                    string icon = columns.GetAttribute("title");

                    //string title = toolgroup.GetAttribute("title");
                    if (title.Contains(icon))
                    {
                        j++;
                        counter1++;
                        continue;

                    }
                    else
                    {
                        counter1++;
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        Logger.Instance.InfoLog("Tooltip dropdown for tool group '" + counter1 + "' is not displayed.");
                        break;

                    }

                }
                StudyViewer.CloseStudy();

                //step-6 Apply 'All in One Tool'(left mouse button)
                studies = (Studies)login.Navigate("Studies");

                //Accession[0]=11665475
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.AllinOneTool);
                StudyViewer.DragMovement(StudyViewer.SeriesViewer_1X1());
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_6 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_1X1());
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


                //step-7 Apply 'All in One Tool'(middle wheel,right mouse button)
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step-8 Apply W/L, Zoom, Pan on selected image.
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                StudyViewer.DragMovement(StudyViewer.SeriesViewer_1X2());
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                StudyViewer.DragMovement(StudyViewer.SeriesViewer_1X2());
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                StudyViewer.DragMovement(StudyViewer.SeriesViewer_1X2());
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_7 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_1X2());
                if (step_7)
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
                StudyViewer.CloseStudy();
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);

                //step-9(i) Apply Magnifier tool ( x2, x3, x4) on selected image.

                //html-4 view
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Magnifierx2);
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("firefox"))
                {
                    StudyViewer.CloseZoom();
                }
                else
                {
                    new Actions(BasePage.Driver).Click(StudyViewer.SeriesViewer_1X1()).Release().Build().Perform();
                }
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Magnifierx3);
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("firefox"))
                {
                    StudyViewer.CloseZoom();
                }
                else
                {
                    new Actions(BasePage.Driver).Click(StudyViewer.SeriesViewer_1X1()).Release().Build().Perform();
                }
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Magnifierx4);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_10 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_1X1());
                if (step_10)
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
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("firefox"))
                {
                    StudyViewer.CloseZoom();
                }
                else
                {
                    new Actions(BasePage.Driver).Click(StudyViewer.SeriesViewer_1X1()).Release().Build().Perform();
                }
                StudyViewer.CloseStudy();


                //step-10(ii) html-5 view                
                //studies.SelectStudy("Accession", Accession[0]);
                //if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("chrome"))
                //{
                //    StudyViewer.Html5ViewStudy();
                //    PageLoadWait.WaitForFrameLoad(20);
                //    StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Magnifierx2);
                //    StudyViewer.DragMovement(StudyViewer.html5seriesViewer_1X2());
                //    StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Magnifierx3);
                //    StudyViewer.DragMovement(StudyViewer.html5seriesViewer_1X2());
                //    StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Magnifierx4);
                //    int h = StudyViewer.html5seriesViewer_1X2().Size.Height;
                //    int w = StudyViewer.html5seriesViewer_1X2().Size.Width;
                //    new Actions(BasePage.Driver).MoveToElement(StudyViewer.html5seriesViewer_1X2(), w / 2, h / 2).ClickAndHold().MoveToElement(StudyViewer.html5seriesViewer_1X2(), w / 2, h / 4).Build().Perform();
                //    Thread.Sleep(2000);
                //    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                //    bool step_11 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.html5seriesViewer_1X2());
                //    if (step_11)
                //    {
                //        result.steps[ExecutedSteps].status = "Pass";
                //        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                //    }
                //    else
                //    {
                //        result.steps[ExecutedSteps].status = "Fail";
                //        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                //        result.steps[ExecutedSteps].SetLogs();
                //    }
                //    new Actions(BasePage.Driver).Release().Build().Perform();
                //    StudyViewer.CloseStudy();
                //}
                //else
                //{
                    result.steps[++ExecutedSteps].status = "Not Automated";
                //}


                //step-11 Load study for Edge Enhancement 

                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                //StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div[id$='studyPanelDiv_1']")));
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.EdgeEnhancementInteractive);
                StudyViewer.DragMovement(StudyViewer.SeriesViewer_1X1());
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                StudyViewer.DragMovement(StudyViewer.SeriesViewer_1X1());
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_12 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_1X1());
                if (step_12)
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
                StudyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //step-12 Load studies that are US,PT,OP,PR and KO and apply the edge enhancement
                //*******tested only US and PT*******

                //Accession[1] --01492679
                studies.SearchStudy(AccessionNo: Accession[1], Modality: "US", Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[1]);

                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                IList<IWebElement> enhancetools = BasePage.Driver.FindElements(By.CssSelector("a[class^='AnchorClass32 toplevel']+ul>li[title^='Edge Enhancement']>a>img[class^='notSelected32 disable']"));
                int counter2 = 0;
                foreach (IWebElement enhancetool in enhancetools)
                {
                    if (enhancetool != null)
                    {
                        counter2++;
                    }
                }
                StudyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                //Accession[2] --1742901
                //studies.SearchStudy(AccessionNo: Accession[2], Modality: "PT", Datasource: EA_91);
                studies.SearchStudy(AccessionNo: Accession[2], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[2]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                IList<IWebElement> enhancetools1 = BasePage.Driver.FindElements(By.CssSelector("a[class^='AnchorClass32 toplevel']+ul>li[title^='Edge Enhancement']>a>img[class^='notSelected32 disable']"));
                int counter3 = 0;
                foreach (IWebElement enhancetool in enhancetools)
                {
                    if (enhancetool != null)
                    {
                        counter3++;
                    }
                }
                if (counter2 == 0 && counter3 == 0)
                {
                    result.steps[ExecutedSteps++].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps++].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                StudyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //step 13: Interactively adjust the edge enhancement of various images, by selecting the interactive 
                //edge enhancement tool and dragging the left mouse button up/down and left/right.
                //Accession[0]=11665475
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);

                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.EdgeEnhancementInteractive);
                StudyViewer.DragMovement(StudyViewer.SeriesViewer_1X1());
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.EdgeEnhancementInteractive);
                StudyViewer.DragMovement(StudyViewer.SeriesViewer_1X2());
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.EdgeEnhancementInteractive);
                StudyViewer.DragMovement(StudyViewer.SeriesViewer_2X1());
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.EdgeEnhancementInteractive);
                StudyViewer.DragMovement(StudyViewer.SeriesViewer_2X2());
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_13 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.studyPanel());
                if (step_13)
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
                studies.CloseStudy();


                //step 14: Select each of the following measurement tools and apply them on various images:
                //* Line (draw at least one line the same length as the caliper)
                //* Rectangle (draw one rectangle on 1/4 of the image, ex. top right 1/4 of the image)
                //* Ellipse
                //* Angle
                //* Cobb Angle

                //Accession[0]=11665475
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);

                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.DrawLineMeasurement(StudyViewer.SeriesViewer_1X1(), 50, 100);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.DrawEllipse);
                //StudyViewer.DrawElipse(StudyViewer.SeriesViewer_1X2());
                StudyViewer.DrawEllipse(StudyViewer.SeriesViewer_1X2(), 120, 120, 180, 150);
                StudyViewer.DrawCobbAngle(StudyViewer.SeriesViewer_2X2(), 80, 80, 150, 120, 100, 100, 120, 150);
                StudyViewer.DrawAngleMeasurement(StudyViewer.SeriesViewer_1X1(), 80, 80, 100, 100, 120, 150);
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("firefox"))
                {
                    StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.DrawRectangle);
                    StudyViewer.DrawRectangle(StudyViewer.SeriesViewer_1X2(), 90, 30, 120, 130);
                }
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_14 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.studyPanel());
                if (step_14)
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
                studies.CloseStudy();

                //step 15: Select 'Free-hand Draw' measurement tool and draw few free-hand lines on an image.
                /*studies.SelectStudy("Accession", Accession[0]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.FreeDraw);
                StudyViewer.DrawMeasurementTool(StudyViewer.SeriesViewer_1X2(), 50, 150, 20, 150);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_15 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.studyPanel());
                if (step_15)
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
                studies.CloseStudy();
                */
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 16 - With the left mouse press and drag each of the previously drawn measurements.
                result.steps[++ExecutedSteps].status = "Not Automated";


                //step 17 - Draw few more measurements, then select Edit Annotations tool button and edit each of the measurements previously drawn.
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("firefox"))
                {
                    result.steps[++ExecutedSteps].status = "Not Automated";
                }
                else
                {
                    studies.SelectStudy("Accession", Accession[0]);
                    StudyViewer.LaunchStudy();
                    PageLoadWait.WaitForFrameLoad(20);

                    //StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.DrawRectangle);
                    //StudyViewer.DrawRectangle(StudyViewer.SeriesViewer_1X1(), 90, 30, 120, 130);
                    StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.LineMeasurement);
                    StudyViewer.DrawEllipse(StudyViewer.SeriesViewer_1X2(), 50, 50, 50, 180);
                    StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.AngleMeasurement);
                    StudyViewer.DrawAngleMeasurement(StudyViewer.SeriesViewer_1X2(), 80, 80, 100, 100, 120, 150);
                    StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.EditAnnotations);
                    StudyViewer.DrawMeasurementTool(StudyViewer.SeriesViewer_1X2(), 100, 100, 105, 105);
                    StudyViewer.DrawMeasurementTool(StudyViewer.SeriesViewer_1X2(), 50, 150, 20, 150);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    bool step_16 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.studyPanel());
                    if (step_16)
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
                    studies.CloseStudy();
                }
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
        /// Plumb Lines
        /// </summary>
        public TestCaseResult Test_27852(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");

                String[] Accession = AccessionList.Split(':');
                String[] PatientID = PatientIDList.Split(':');

                //Step 1 - Login iCA as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);             
                //--Pre condition-- disable connection tool.
                login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(20);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                if (BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Selected == false)
                {
                    BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Click();
                    Logger.Instance.InfoLog("Connection tool CB un-selected successfully");
                }
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                ExecutedSteps++;

                //Step 2 - Navigate to Search                
                studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //Step 3 - Load a study 
                //PatientID[0] --C300025
                //Accession[0] -- MS10025
                studies.SearchStudy(patientID: PatientID[0], Datasource: EA_131);
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step 4 - Select the horizontal plumb line icon and click on several location on the image
                var element1 = viewer.SeriesViewer_1X1();
                element1.Click();
                viewer.DrawHorizontalPlumbLine(element1, 23, 37);
                viewer.DrawHorizontalPlumbLine(element1, 110, 50);
                viewer.DrawHorizontalPlumbLine(element1, 145, 89);
                viewer.DrawHorizontalPlumbLine(element1, 213, 123);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_4)
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
                viewer.CloseStudy();

                //Step 5 - Select the vertical plumb line icon and click on several location on the image
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer viewer2 = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                var element2 = viewer.SeriesViewer_1X1();
                element2.Click();
                viewer.DrawVerticalPlumbLine(element2, 50, 60);
                viewer.DrawVerticalPlumbLine(element2, 70, 80);
                viewer.DrawVerticalPlumbLine(element2, 90, 100);
                viewer.DrawVerticalPlumbLine(element2, 110, 120);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_5 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_5)
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

                //Step 6 - While in the vertical plumb line mode move the cursor over one of the vertical lines, click and hold with the left mouse button and drag the line.
                viewer.ClickHoldAndDrop(element2, 70, 80, 130, 70);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
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

                //Step 7 - Select other lines and try to move them       
                viewer.ClickHoldAndDrop(element2, 50, 60, 140, 70);
                viewer.ClickHoldAndDrop(element2, 90, 100, 150, 80);
                viewer.ClickHoldAndDrop(element2, 110, 120, 160, 90);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_7)
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

                //Step 8 - Close study and Logout from ICA
                studies.CloseStudy();
                login.Logout();
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
        /// Joint Lines
        /// </summary>
        public TestCaseResult Test_27853(String testid, String teststeps, int stepcount)
        {
            //Fetch the data
            String Contactinfo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ContactInformation");
            String[] info = Contactinfo.Split('=');

            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String Username = "User1" + new Random().Next(1, 10000);
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");

                String[] Accession = AccessionList.Split(':');

                //Step 1 - Login iCA as Administrator and Navigate to Search
                login.LoginIConnect(adminUserName, adminPassword);                
                //--Pre condition-- disable connection tool.
                login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(20);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                if (BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Selected == false)
                {
                    BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Click();
                    Logger.Instance.InfoLog("Connection tool CB un-selected successfully");
                }
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();

                studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //Step 2 - Load a full hip AP image (ex. Balco Stevie or Bittner Jennifer datasets, available in FORENZA)                                
                //Accession[0] =>19970912
                login.SearchStudy(AccessionNo: Accession[0], Datasource: EA_91);
                login.SelectStudy("Accession", Accession[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_2)
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


                //Step 3 - "1. Select Joint Line Measurement tool. This tool draws two lines at an angle and a third line perpendicular to the last line drawn.
                //2. Start at a point and click."
                var action = new Actions(BasePage.Driver);
                IWebElement element = viewer.SeriesViewer_1X1();
                //viewer.DrawJointLineMeasurment(element, element.Size.Width / 2, element.Size.Height / 2, (element.Size.Width / 2 + (35)), (element.Size.Height / 2 + (30)));
                viewer.DrawJointLineMeasurment(element, 200, 200, 300, 300, 400, 200);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
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

                //Step 4 - Using the left mouse button press-and hold on any of the three initial points and -drag to rotate, stretch, shorten the lines.
                /*action.ClickAndHold(element).Build().Perform();
                action.MoveByOffset(170, 130).Build().Perform();
                action.Release().Build().Perform();*/
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                new Actions(Driver).MoveToElement(element, 300, 300).ClickAndHold().Build().Perform();
                Thread.Sleep(3000);
                new Actions(Driver).ClickAndHold().MoveToElement(element, 300, 400).ClickAndHold().Build().Perform();
                Thread.Sleep(3000);
                new Actions(Driver).Release().Build().Perform();
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_4)
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

                //Step 5 - Select the end points of the perpendicular line drag and rotate the end points
                //new Actions(Driver).MoveToElement(element, 350, 300).Build().Perform();
                //Thread.Sleep(3000);
                //new Actions(Driver).ClickAndHold().Build().Perform();
                //Thread.Sleep(3000);
                //new Actions(Driver).MoveToElement(element, 375, 250).Build().Perform();
                //Thread.Sleep(3000);
                //new Actions(Driver).Release().Build().Perform();
                //Thread.Sleep(3000);
                //result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                //bool step_5 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                //if (step_5)
                //{
                //    result.steps[ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}

                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 6 - Select the middle of perpendicular line and move it
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 7 - Rotate the image and observe the joint lines
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                viewer.SelectToolInToolBar("RotateClockwise");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_7)
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

                //Step 8 - Save the GSPS series
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 9 - Change the viewer to 2x3 series and load the previously saved series in an empty viewport.       
                viewer.SelectToolInToolBar("SeriesViewer2x3");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);

                IWebElement TargetElement = viewer.SeriesViewer_2X2();
                IWebElement SourceElement = BasePage.Driver.FindElement(By.CssSelector("#ModalityDivMG>div"));
                new Actions(BasePage.Driver).DragAndDrop(SourceElement, TargetElement).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_9 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_9)
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

                //Step 10 - Close study and Logout from ICA
                login.CloseStudy();
                login.Logout();
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
        /// Calibration Tool
        /// </summary>
        public TestCaseResult Test_27854(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String UserName = Config.adminUserName;
                String Password = Config.adminPassword;


                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');



                //Step-1,2 and 3: Load Bony, Rose study (MR)
                login.LoginIConnect(UserName, Password);
              
                //--Pre condition-- disable connection tool.
                login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(20);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                if (BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Selected == false)
                {
                    BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Click();
                    Logger.Instance.InfoLog("Connection tool CB un-selected successfully");
                }
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                ExecutedSteps++;

                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Accession[0] => 11665475

                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);

                int viewports = StudyViewer.SeriesViewPorts().Count();
                if (viewports == 4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-4: Select Calibration tool.
                IWebElement CalibrationTool = BasePage.Driver.FindElement(By.CssSelector("a[class^='AnchorClass32 toplevel']+ul>li[title^='Calibration Tool']>a>img[class$='disableOnCine']"));
                if (CalibrationTool != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-5: Load a non-calibrated data, draw a measurement on one of the images and Calibrate the image.
                StudyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                //Accession[1] =>234234
                studies.SearchStudy(AccessionNo: Accession[1], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[1]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.DrawLineMeasurement(StudyViewer.SeriesViewer_1X1(), 80, 80);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.CalibrationTool);
                Thread.Sleep(4000);
                StudyViewer.CalibrationTool(StudyViewer.SeriesViewer_1X1(), "1_1", 200, 200, 250, 200, "10");
                Thread.Sleep(4000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_3 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_1X1());
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

                //step-6: For measurements such as rectangles / circles, verify their values for the area. 
                //Use geometrical formulas to calculate the area and then compare it with the displayed value.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step-7: Try to re-calibrate the image by editing the existing calibration line.
                new Actions(BasePage.Driver).MoveToElement(StudyViewer.SeriesViewer_1X1(), 249, 200).ClickAndHold().MoveToElement(StudyViewer.SeriesViewer_1X1(), 240, 200).Build().Perform();
                Thread.Sleep(3000);
                new Actions(BasePage.Driver).Release().Build().Perform();
                new Actions(BasePage.Driver).MoveToElement(StudyViewer.SeriesViewer_1X1(), 240, 200).ClickAndHold().Release().Build().Perform();
                Thread.Sleep(4000);
                IWebElement textbox = BasePage.Driver.FindElement(By.CssSelector("input[id='m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_calibrationInputBox']"));
                if (textbox.Displayed == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-8: Select Calibration tool.Draw a calibration line and cancel the calibration.
                new Actions(BasePage.Driver).MoveToElement(StudyViewer.SeriesViewer_1X1(), 150, 180).Click().Build().Perform();
                Thread.Sleep(3000);
                new Actions(BasePage.Driver).MoveToElement(StudyViewer.SeriesViewer_1X1(), 180, 180).Click().Build().Perform();
                Thread.Sleep(4000);
                new Actions(BasePage.Driver).SendKeys(Keys.Escape).Perform();
                //StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.AllinOneTool);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_6 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_1X1());
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


                //step-9: Select Calibration tool. Draw a calibration line the same length as the previous calibration line,
                //but enter a new value for its length and accept it.
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.CalibrationTool);
                StudyViewer.CalibrationTool(StudyViewer.SeriesViewer_1X1(), "1_1", 150, 180, 180, 180, "15");
                Thread.Sleep(4000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_7 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_1X1());
                if (step_7)
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


                //step-10: Draw few more measurements.
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.DrawEllipse);
                StudyViewer.DrawEllipse(StudyViewer.SeriesViewer_1X1(), 120, 120, 180, 150);
                StudyViewer.DrawAngleMeasurement(StudyViewer.SeriesViewer_1X1(), 230, 230, 250, 250, 280, 220);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_8 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_1X1());
                if (step_8)
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

                //step-11:For measurements such as rectangles / circles, verify their values for the area. 
                //Use geometrical formulas to calculate the area and then compare it with the displayed value.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step-12: Scroll through the series the calibrated image belongs and verify all the images.
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout1x2);
                StudyViewer.DragScroll(1, 1, 2, 7);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_10 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_1X1());
                if (step_10)
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
                //step-13: Save the GSPS for the image.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step-14: Re-load the data. Try to calibrate the PR series.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step-15: Load one data from each of the following modalities: PR/KO/MR/CT/PT/NM multi-frame.
                //PR
                StudyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                //PatientID 7395012 , Accession[2] 10211067
                studies.SearchStudy(AccessionNo: Accession[2],Datasource: EA_131);
                studies.SelectStudy("Accession", Accession[2]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                IWebElement CalibrationTool1 = BasePage.Driver.FindElement(By.CssSelector("a[class^='AnchorClass32 toplevel']+ul>li[title^='Calibration Tool']>a>img[class$='disableOnCine']"));

                //KO
                StudyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //Accession[3]  MR10005
                studies.SearchStudy(AccessionNo: Accession[3], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[3]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                IWebElement CalibrationTool2 = BasePage.Driver.FindElement(By.CssSelector("a[class^='AnchorClass32 toplevel']+ul>li[title^='Calibration Tool']>a>img[class$='disableOnCine']"));

                //MR
                StudyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                //Accession[4] => 11661055
                studies.SearchStudy(AccessionNo: Accession[4], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[4]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                IWebElement CalibrationTool3 = BasePage.Driver.FindElement(By.CssSelector("a[class^='AnchorClass32 toplevel']+ul>li[title^='Calibration Tool']>a>img[class$='disableOnCine']"));

                //CT              
                StudyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                // Accession[5] =>561
                studies.SearchStudy(AccessionNo: Accession[5], Datasource: EA_131);
                studies.SelectStudy("Accession", Accession[5]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                IWebElement CalibrationTool4 = BasePage.Driver.FindElement(By.CssSelector("a[class^='AnchorClass32 toplevel']+ul>li[title^='Calibration Tool']>a>img[class$='disableOnCine']"));

                //PT
                StudyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                // Accession[6] =>537103
                studies.SearchStudy(AccessionNo: Accession[6], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[6]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                IWebElement CalibrationTool5 = BasePage.Driver.FindElement(By.CssSelector("a[class^='AnchorClass32 toplevel']+ul>li[title^='Calibration Tool']>a>img[class$='disableOnCine']"));

                //NM
                StudyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                //Accession[7] =>3453451
                studies.SearchStudy(AccessionNo: Accession[7], Datasource: EA_131);
                studies.SelectStudy("Accession", Accession[7]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                IWebElement CalibrationTool6 = BasePage.Driver.FindElement(By.CssSelector("a[class^='AnchorClass32 toplevel']+ul>li[title^='Calibration Tool']>a>img[class$='disableOnCine']"));
                if (CalibrationTool1 != null && CalibrationTool2 != null && CalibrationTool3 != null && CalibrationTool4 != null && CalibrationTool5 != null && CalibrationTool6 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-16: Close study and Logout from ICA
                StudyViewer.CloseStudy();
                login.Logout();
                studies.CloseBrowser();
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
        /// Transischial Measurement tool
        /// </summary>
        public TestCaseResult Test_27855(String testid, String teststeps, int stepcount)
        {
            //Fetch the data
            String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
            String[] Accession = AccessionList.Split(':');

            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                //Step 1 - Re-login as either Administrator or user created
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");

                //--Pre condition-- disable connection tool.      
                PageLoadWait.WaitForPageLoad(20);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                if (BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Selected == false)
                {
                    BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Click();
                    Logger.Instance.InfoLog("Connection tool CB un-selected successfully");
                }
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                ExecutedSteps++;

                //Step 2 - Load a full hip AP image (ex. Balco Stevie or Bittner Jennifer datasets, available in FORENZA)
                //Accession[0] => 7817811                
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_77);
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_2)
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

                //Step 3 - 1. Select Transischial measurement tool. This tool draws a transischial line on
                //the image and measures the difference in height of the two perpendicular lines.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x3);
                PageLoadWait.WaitForFrameLoad(10);
                viewer.DrawTransischialMeasurement(viewer.SeriesViewer_1X2(), 50, 375, 290, 385, 75, 345, 275, 355);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());
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

                //Step 4 - Using the left mouse button press-and-drag to rotate, stretch, shorten the horizontal line using the holding points.
                var action = new Actions(BasePage.Driver);
                //viewer.SelectToolInToolBar(IEnum.ViewerTools.EditAnnotations);
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("chrome"))
                {
                    new Actions(Driver).MoveToElement(viewer.SeriesViewer_1X2(), 50, 375).ClickAndHold().Build().Perform();
                    Thread.Sleep(2000);
                    new Actions(Driver).ClickAndHold().MoveToElement(viewer.SeriesViewer_1X2(), 50, 250).ClickAndHold().Build().Perform();
                    Thread.Sleep(2000);
                    new Actions(Driver).Release().Build().Perform();
                    Thread.Sleep(2000);
                }
                else
                {
                    action.MoveToElement(viewer.SeriesViewer_1X2(), 50, 375).ClickAndHold().Build().Perform();
                    action.MoveToElement(viewer.SeriesViewer_1X2(), 50, 250).Build().Perform();
                    action.MoveToElement(viewer.SeriesViewer_1X2(), 50, 150).Release().Build().Perform();
                }
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());
                if (step_4)
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

                //Step 5 - Drag the end points of the perpendicular lines to change their length and position on the horizontal line.

                /*if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("chrome"))
                {
                    new Actions(Driver).MoveToElement(viewer.SeriesViewer_1X2(), 290, 385).ClickAndHold().Build().Perform();
                    Thread.Sleep(2000);
                    new Actions(Driver).ClickAndHold().MoveToElement(viewer.SeriesViewer_1X2(), 290, 250).ClickAndHold().Build().Perform();
                    Thread.Sleep(2000);
                    new Actions(Driver).Release().Build().Perform();
                    Thread.Sleep(2000);
                }*/

                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 6 - Apply image layout to change the image layout.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x2);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
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

                //Step 7 - Apply series viewer layout changes.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x3);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_7)
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

                //TODO
                //Step 8 - Apply the invert, rotate, flip tools on the selected image.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.AllinOneTool);
                viewer.SeriesViewer_1X3().Click();
                viewer.DragMovement(viewer.SeriesViewer_1X3());
                //int h = viewer.SeriesViewer_1X3().Size.Height;
                //int w = viewer.SeriesViewer_1X3().Size.Width;
                //new Actions(BasePage.Driver).MoveToElement(viewer.SeriesViewer_1X3(), (w / 6), (h / 6)).Click().Build().Perform();                    
                //new Actions(BasePage.Driver).MoveToElement(viewer.SeriesViewer_1X3(), (w / 6), (h / 6)).Click().Build().Perform();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                Thread.Sleep(3000);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.FlipVertical);
                Thread.Sleep(3000);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.FlipHorizontal);
                Thread.Sleep(3000);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                Thread.Sleep(3000);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateCounterclockwise);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_8 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X3());
                if (step_8)
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
                viewer.CloseStudy();

                //TODO
                //Step 9 - Activate one viewport, then select each pre-defined options from the edge enhancement tool button menu (ex. Low 5x5, Medium 3x3, etc.)                
                //Accession[1] => 11665475
                studies.SearchStudy(AccessionNo: Accession[1], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[1]);
                StudyViewer viewer2 = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(10);
                viewer2.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x3);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer2.SeriesViewer_1X2()));
                /*IWebElement sourceElement = BasePage.Driver.FindElement(By.CssSelector("#ModalityDivCR>div"));
                IWebElement targetElement = viewer2.SeriesViewer_1X2();

                new Actions(BasePage.Driver).DragAndDrop(sourceElement, targetElement).Build().Perform();
                PageLoadWait.WaitForFrameLoad(20);*/
                viewer2.SeriesViewer_1X2().Click();

                viewer2.SelectToolInToolBar("EdgeEnhancementInteractive");
                viewer2.SelectToolInToolBar("EdgeEnhancementLow5x5");
                viewer2.SelectToolInToolBar("EdgeEnhancementMedium11x11");
                viewer2.SelectToolInToolBar("EdgeEnhancementMedium3x3");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_9 = studies.CompareImage(result.steps[ExecutedSteps], viewer2.SeriesViewer_1X2());
                if (step_9)
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
                studies.CloseStudy();

                //Step 10 - Interactively adjust the edge enhancement of various images, by selecting the interactive edge enhancement tool and dragging the left mouse button up/down and left/right.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 11 - Load a study with multiple series.
                //Accession[2] =>89894
                studies.SearchStudy(AccessionNo: Accession[2], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[2]);
                StudyViewer viewer3 = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForAllViewportsToLoad(90);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_11 = studies.CompareImage(result.steps[ExecutedSteps], viewer3.studyPanel());
                if (step_11)
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

                //TODO
                //Step 12 - Select one viewport and apply "Next Series", then "Previous Series"
                viewer3.SeriesViewer_1X1().Click();
                viewer3.SelectToolInToolBar("NextSeries");
                PageLoadWait.WaitForFrameLoad(10);
                viewer3.SelectToolInToolBar("PreviousSeries");
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_12 = studies.CompareImage(result.steps[ExecutedSteps], viewer3.SeriesViewer_1X1());
                if (step_12)
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

                //Step 13 - Scroll the images within the series using Next Image button from the scrolling slot.
                viewer3.SeriesViewer_1X1().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div[id$=studyPanel_1_ctl03_SeriesViewer_1_1_cineToolbar] button[id='m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_cineBtnNextFrame']")));
                /*IWebElement NextBtn = Driver.FindElement(By.CssSelector("div[id$=studyPanel_1_ctl03_SeriesViewer_1_1_cineToolbar] button[id='m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_cineBtnNextFrame']"));               
                NextBtn.Click();*/
                for (int i = 0; i < 2; i++)
                {
                    viewer3.ClickDownArrowbutton(1, 1);
                }
                Thread.Sleep(5000);
                PageLoadWait.WaitForAllViewportsToLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_13 = studies.CompareImage(result.steps[ExecutedSteps], viewer3.SeriesViewer_1X1());
                if (step_13)
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

                //Step 14 - Scroll the images within the series using Previous Image button from the scrolling slot.
                /*IWebElement PrevBtn = Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_cineBtnPrevFrame"));
                PrevBtn.Click();*/
                viewer3.SeriesViewer_1X1().Click();
                for (int i = 0; i < 2; i++)
                {
                    viewer3.ClickUpArrowbutton(1, 1);
                }
                Thread.Sleep(5000);
                PageLoadWait.WaitForAllViewportsToLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_14 = studies.CompareImage(result.steps[ExecutedSteps], viewer3.SeriesViewer_1X1());
                if (step_14)
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

                //Step 15 - Scroll the images within a series using the up/down arrow keys from the keyboard or the mouse wheel.
                viewer3.SeriesViewer_1X1().Click();
                for (int i = 0; i < 3; i++)
                {
                    viewer3.ClickDownArrowbutton(1, 1);
                }
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_15 = studies.CompareImage(result.steps[ExecutedSteps], viewer3.SeriesViewer_1X1());
                if (step_15)
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

                //Step 16 - Apply Toggle Text tool.
                viewer3.SelectToolInToolBar("ToggleText");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_16 = studies.CompareImage(result.steps[ExecutedSteps], viewer3.studyPanel());
                if (step_16)
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

                //Step 17 - 1. Apply the Full Screen tool. 
                viewer3.SelectToolInToolBar("FullScreen");
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_17 = studies.CompareImage(result.steps[ExecutedSteps], viewer3.studyPanel());
                if (step_17)
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

                //Step 18 - 2. Exit the Full Screen mode.
                Thread.Sleep(10000);
                viewer3.MenusBtn().Click();
                Thread.Sleep(5000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(viewer.By_ReviewtoolBar()));
                viewer3.SelectToolInToolBar("FullScreen");
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_18 = studies.CompareImage(result.steps[ExecutedSteps], viewer3.studyPanel());
                if (step_18)
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

                //Step 19 - Close study and Logout from ICA
                CloseStudy();
                login.Logout();
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
        /// Cine Tool
        /// </summary>
        public TestCaseResult Test1_27856(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String UserName = Config.adminUserName;
                String Password = Config.adminPassword;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');


                //Step-1: Load a multiple image per series dataset. Select Cine. Adjust the speed while cine is playing.
                login.LoginIConnect(UserName, Password);
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //--Pre condition-- disable connection tool.                
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                if (BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Selected == false)
                {
                    BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Click();
                    Logger.Instance.InfoLog("Connection tool CB un-selected successfully");
                }
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();


                //Accession[0] =>89894
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Cine);
                Thread.Sleep(10000);
                IWebElement cineslider = BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_cineSliderFramespeed"));
                //int w=slider.Size.Width;
                //int h = cineslider.Size.Height;
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_frameIndicatorFps")));
                Thread.Sleep(6000);
                IWebElement framerate = BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_frameIndicatorFps"));
                IWebElement fpsCinetoolbar = BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_cineLabelFrameRate"));
                String[] Speed = framerate.Text.Split(' ');
                if (framerate.Text != null && fpsCinetoolbar.Text != null && fpsCinetoolbar.Text.Contains(framerate.Text) && framerate.Text.Contains("20"))
                {
                    StudyViewer.CineSpeedFPS(1, 1, 10);
                    Thread.Sleep(6000);
                }
                IWebElement framerate1 = BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_frameIndicatorFps"));
                IWebElement fpsCinetoolbar1 = BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_cineLabelFrameRate"));

                String[] decSpeed = framerate1.Text.Split(' ');

                if (framerate1.Text != null && fpsCinetoolbar1.Text != null && fpsCinetoolbar1.Text.Contains(framerate1.Text) && Int32.Parse(Speed[0]) > Int32.Parse(decSpeed[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-2: Apply image manipulation tools- Window/Level, Zoom, Pan, Rotate, Flip, Grayscale Inversion while cine is playing.

                /*StudyViewer.cineViewport(1, 1).Click();
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                //IWebElement canvas = BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_canvasViewerImage"));
                StudyViewer.DragMovement(StudyViewer.cineViewport(1, 1));
                StudyViewer.cineViewport(1, 1).Click();
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                Thread.Sleep(4000);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                Thread.Sleep(4000);
                StudyViewer.cineViewport(1, 1).Click();
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                StudyViewer.DragMovement(StudyViewer.cineViewport(1, 1));
                StudyViewer.cineViewport(1, 1).Click();
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                StudyViewer.DragMovement(StudyViewer.cineViewport(1, 1));
                StudyViewer.cineViewport(1, 1).Click();
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                Thread.Sleep(4000);
                StudyViewer.cineViewport(1, 1).Click();
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.FlipHorizontal);
                Thread.Sleep(4000);
                ExecutedSteps++;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_2 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.cineViewport(1, 1));
                if (step_2)
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
                */

                //can't verify pan, W/L, Rotate,.. applied or not
                result.steps[++ExecutedSteps].status = "Not Automated";


                //step-3: Pause the cine. Select Step Forward / Step Backward cine controls.
                /*BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_cineBtnPause")));
                StudyViewer.cinepause(1, 1).Click();
                StudyViewer.cineNextFramebtn(1, 1).Click();
                StudyViewer.cineNextFramebtn(1, 1).Click();
                StudyViewer.cinePrevFramebtn(1, 1).Click();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                 bool step_3 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.cineViewport(1, 1));
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
                 
                ExecutedSteps++;
                */
                //can't verify next frame/ previous frame clicked or not
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step-4: Resume cine by selecting Play button. Stop cine.
                StudyViewer.cinepause(1, 1).Click();
                Thread.Sleep(3000);
                bool step_4_1 = StudyViewer.cineplay(1, 1).Displayed && StudyViewer.cinestop(1, 1).Displayed;

                StudyViewer.cineplay(1, 1).Click();
                Thread.Sleep(3000);
                bool step_4_2 = StudyViewer.cinepause(1, 1).Displayed;

                StudyViewer.cinestop(1, 1).Click();
                Thread.Sleep(3000);
                bool step_4_3 = StudyViewer.cineplay(1, 1).Displayed && (StudyViewer.cinepause(1, 1).Displayed == false);

                if (step_4_1 && step_4_2 && step_4_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-5: Print view.
                string windowtitle = "iConnect® Access";
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.PrintView);
                var printwindow = StudyViewer.SwitchtoNewWindow(windowtitle);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[id='SeriesViewersDiv']")));
                /* result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_5 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.Printimage());
                if (step_5)
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
                if (StudyViewer.Printimage().Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }



                //step-6: Reset the image.
                BasePage.Driver.Close();
                BasePage.Driver.SwitchTo().Window(printwindow);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                Thread.Sleep(4000);
                /* result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_6 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_1X1());
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
                }*/
                ExecutedSteps++;

                //step-7: Close the viewer.
                StudyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                IWebElement studylist = BasePage.Driver.FindElement(By.CssSelector("#gview_gridTableStudyList"));
                if (studylist.Displayed == true)
                {
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
                studies.CloseBrowser();

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
        /// Cine Tool (global stack and localizer line)
        /// </summary>
        public TestCaseResult Test2_27856(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String UserName = Config.adminUserName;
                String Password = Config.adminPassword;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');


                //Step-1: Load study with multiple MR related series. (ex. Therapy Head)
                login.LoginIConnect(UserName, Password);
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //--Pre condition-- disable connection tool.                
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                if (BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Selected == false)
                {
                    BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Click();
                    Logger.Instance.InfoLog("Connection tool CB un-selected successfully");
                }
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                
                //Accession[0] => 334334
                //*** Study not correct (have only one images)
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                if (StudyViewer.studyPanel().Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-2: Change the image layout to 2x2, then select Image Scope tool. 
                //Apply W/L, pan, zoom, rotate, flip, grayscale inversion, reset on different images.
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x2);
                Thread.Sleep(3000);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.ImageScope);
                Thread.Sleep(3000);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                StudyViewer.DragMovement(StudyViewer.SeriesViewer_1X1());
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                Thread.Sleep(4000);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                Thread.Sleep(4000);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                StudyViewer.DragMovement(StudyViewer.SeriesViewer_1X1());
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                StudyViewer.DragMovement(StudyViewer.cineViewport(1, 1));
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                Thread.Sleep(4000);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.FlipHorizontal);
                Thread.Sleep(4000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_2 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_1X1());
                if (step_2)
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



                //step-3: Select Series Scope tool.
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesScope);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_3 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_1X1());
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

                //step-4: Apply W/L, pan, zoom, rotate, flip, grayscale inversion, reset on different images.
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                Thread.Sleep(4000);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                StudyViewer.DragMovement(StudyViewer.SeriesViewer_1X1());
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                Thread.Sleep(4000);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                StudyViewer.DragMovement(StudyViewer.SeriesViewer_1X1());
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                StudyViewer.DragMovement(StudyViewer.cineViewport(1, 1));
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                Thread.Sleep(4000);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.FlipHorizontal);
                Thread.Sleep(4000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_4 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_1X1());
                if (step_4)
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

                //step-5: Select Link two axial viewports (Series 2 and 3) from Linked Scrolling menu.
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                Thread.Sleep(4000);
                StudyViewer.SeriesViewer_1X1().Click();
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout1x1);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                var action = new Actions(BasePage.Driver);
                action.DragAndDrop(StudyViewer.Thumbnails()[1], StudyViewer.SeriesViewer_1X1()).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                action.DragAndDrop(StudyViewer.Thumbnails()[2], StudyViewer.SeriesViewer_1X2()).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                action.DragAndDrop(StudyViewer.Thumbnails()[3], StudyViewer.SeriesViewer_2X1()).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                action.DragAndDrop(StudyViewer.Thumbnails()[4], StudyViewer.SeriesViewer_2X2()).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.LinkSelected);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.SelectLinkedCheckBox(1, 1);
                StudyViewer.SelectLinkedCheckBox(1, 2);
                StudyViewer.LinkedScrollingCheckBtn().Click();
                StudyViewer.DragScroll(1, 1, 8, 20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_5 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.studyPanel());
                if (step_5)
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


                //step-6: Cancel the Link.
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Unlink);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_6 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.studyPanel());
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


                //step-7: Close the viewer.
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.LinkSelected);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.SelectLinkedCheckBox(2, 1);
                StudyViewer.SelectLinkedCheckBox(2, 2);
                StudyViewer.LinkedScrollingCheckBtn().Click();
                StudyViewer.DragScroll(2, 1, 8, 20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_7 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.studyPanel());
                if (step_7)
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

                //step-8: Cancel the Link.
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Unlink);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_8 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.studyPanel());
                if (step_8)
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


                //step-9: Link Selected Offset
                StudyViewer.DragScroll(1, 1, 2, 20);
                StudyViewer.DragScroll(1, 2, 4, 20);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.LinkSelectedOffset);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.SelectLinkedCheckBox(1, 1);
                StudyViewer.SelectLinkedCheckBox(1, 2);
                StudyViewer.LinkedScrollingCheckBtn().Click();
                StudyViewer.DragScroll(1, 1, 8, 20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_9 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.studyPanel());
                if (step_9)
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

                //step-10: Cancel the Link.
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Unlink);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_10 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.studyPanel());
                if (step_10)
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

                //step-11: With the axial series active select Localizer Line. Scroll through the axial series.
                action.DragAndDrop(StudyViewer.Thumbnails()[0], StudyViewer.SeriesViewer_1X2()).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.SeriesViewer_1X1().Click();
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                Thread.Sleep(5000);
                StudyViewer.DragScroll(1, 1, 8, 20);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_11 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.studyPanel());
                if (step_11)
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


                //step-12:Activate the sagittal series and scroll through its images.               
                StudyViewer.SeriesViewer_1X2().Click();
                Thread.Sleep(3000);
                StudyViewer.DragScroll(1, 2, 8, 20);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_12 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.studyPanel());
                if (step_12)
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

                //step-13: Activate one of the coronal series and scroll through its images.               
                StudyViewer.SeriesViewer_2X1().Click();
                Thread.Sleep(3000);
                StudyViewer.DragScroll(2, 1, 8, 20);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_13 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.studyPanel());
                if (step_13)
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

                //step-14: Verify the All In One Tool is working when Localizer lines are enabled.
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.AllinOneTool);
                StudyViewer.DragMovement(StudyViewer.SeriesViewer_1X1());
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_14 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_1X1());
                if (step_14)
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


                //step-15: With the axial series active select Global Stack button.                
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.GlobalStack);
                Thread.Sleep(4000);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_cineBtnNextFrame")));
                Thread.Sleep(4000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_15 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_1X1());
                if (step_15)
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

                //step-16: Scroll the images in the global stack series view.
                StudyViewer.DragScroll(1, 1, 25, 80);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_16 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_1X1());
                if (step_16)
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


                //step-17: Verify Linked scrolling is not enabled if the viewer has a stack
                IWebElement LinkSelected = BasePage.Driver.FindElement(By.CssSelector("a[class^='AnchorClass32 toplevel']+ul>li[title^='Link Selected']>a>img[class$='disableOnCine']"));
                if (LinkSelected != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-18: Activate a coronal series viewer and scroll through its images.
                StudyViewer.DragScroll(1, 1, 1, 30);
                StudyViewer.SeriesViewer_2X1().Click();
                StudyViewer.DragScroll(2, 1, 8, 22);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_18 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.studyPanel());
                if (step_18)
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


                //step-19: Select Series Scope option, if not selected already.
                //Apply WL, zoom, pan, flip horizontal/vertical, invert, rotate, reset in the global stack series view.
                //Scroll through the images after each tool is applied.
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesScope);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                StudyViewer.DragMovement(StudyViewer.SeriesViewer_1X1());
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                Thread.Sleep(3000);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                Thread.Sleep(3000);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                StudyViewer.DragMovement(StudyViewer.SeriesViewer_1X1());
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                StudyViewer.DragMovement(StudyViewer.cineViewport(1, 1));
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                Thread.Sleep(3000);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.FlipHorizontal);
                Thread.Sleep(3000);
                StudyViewer.DragScroll(1, 1, 25, 30);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_19 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_1X1());
                if (step_19)
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

                //step 20: With the global stack series view active, select Global Stack button again.
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.GlobalStack);
                Thread.Sleep(4000);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_cineBtnNextFrame")));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_20 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_1X1());
                if (step_20)
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


                //step 21: Select another viewer (ex. Coronal) and turn on the Global Stack.
                StudyViewer.SeriesViewer_2X1().Click();
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.GlobalStack);
                Thread.Sleep(4000);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_1_cineBtnNextFrame")));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_21 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_2X1());
                if (step_21)
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


                //step 22: Link scrolling is not enabled if the viewer has a stack. Verify the Link and Localizer Line tools.
                IList<IWebElement> LinkSelected1 = BasePage.Driver.FindElements(By.CssSelector("a[class^='AnchorClass32 toplevel']+ul>li[title^='Link Selected']>a>img[class='notSelected32 enabledOnCine disableOnCine']"));
                ExecutedSteps++;
                foreach (IWebElement Link in LinkSelected1)
                {
                    if (Link != null)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        break;
                    }
                }

                //step 23: Activate one of the other three viewers (ex, sagittal) and
                //verify again the Link and Localizer Line tools.
                StudyViewer.SeriesViewer_1X2().Click();
                ExecutedSteps++;
                IList<IWebElement> LinkSelected2 = BasePage.Driver.FindElements(By.CssSelector("a[class^='AnchorClass32 toplevel']+ul>li[title^='Link Selected']>a>img[class='notSelected32 enabledOnCine disableOnCine']"));
                foreach (IWebElement Link in LinkSelected2)
                {
                    if (Link != null)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        break;
                    }
                }

                //step 24: Choose Link Selected and in the little window that opens click on each of the 4 viewports
                //to put a link icon, then click OK.
                /*    StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.LinkSelected);
                    StudyViewer.SelectLinkedCheckBox(1, 1);
                    StudyViewer.SelectLinkedCheckBox(1, 2);
                    StudyViewer.SelectLinkedCheckBox(2, 1);
                    StudyViewer.SelectLinkedCheckBox(2, 2);
                    StudyViewer.LinkedScrollingCheckBtn().Click();
                    //StudyViewer.DragScroll(2, 1, 8, 20);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    bool step_24 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.studyPanel());
                    if (step_24)
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
                    */
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step 25:Make sure the thumbnail splitting is set to 'Image' for US modality,
                //then load an US's multi-frame dataset (ex. Tumne Sunanda from Forenza) into the viewer.
                StudyViewer.CloseStudy();
                UserPref.OpenUserPreferences();
                UserPref.SwitchToUserPrefFrame();
                UserPref.ModalityDropDown().SelectByText("US");
                IWebElement imageRadioBtn = BasePage.Driver.FindElement(By.CssSelector("#ViewingProtocolsControl_ThumbSplitRadioButtons_2"));
                if (imageRadioBtn.Selected != true)
                {
                    imageRadioBtn.Click();
                }
                UserPref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                //PatientID => 1444
                studies.SearchStudy(patientID: PatientID, Datasource: EA_131);
                studies.SelectStudy("Patient ID", PatientID);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                //StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.GlobalStack);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_25 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.studyPanel());
                if (step_25)
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


                //step 26: 1. In 'Series1 Image4' scroll to frame #3, then enable Global Stack.
                //2. Scroll the images in the global stack series view.
                StudyViewer.SeriesViewer_2X1().Click();
                StudyViewer.DragScroll(2, 1, 3, 4);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.GlobalStack);
                StudyViewer.DragScroll(2, 1, 5, 7);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.GlobalStack);
                Thread.Sleep(4000);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_1_cineBtnNextFrame")));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_26 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_2X1());
                if (step_26)
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


                //step 27:Scroll to 'Series1 Image3' frame#2 in the global stack viewer, then turn off the global stack.
                StudyViewer.SeriesViewer_1X2().Click();
                //StudyViewer.DragScroll(1, 2, 4, 7);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.GlobalStack);
                StudyViewer.DragScroll(1, 2, 6, 7);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.GlobalStack);
                Thread.Sleep(4000);
                //BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_cineBtnNextFrame")));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_27 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_1X2());
                if (step_27)
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

                //step 28: Repeat previous two steps for 'Series1 Image3' series.
                ExecutedSteps++;

                //step 29: Load a multiple studies dataset. From the Patient History list load the other study in a second series viewer.
                StudyViewer.CloseStudy();
                //Accession[1] => 7817811
                studies.SearchStudy(AccessionNo: Accession[1], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[1]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#patientHistory_LoadingIndicatorImg")));
                studies.NavigateToHistoryPanel();
                studies.ChooseColumns(new string[] { "Accession" });
                studies.OpenPriors(new string[] { "Accession" }, new string[] { Accession[2] });
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_29 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.StudyPanels());
                if (step_29)
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

                //step 30: Enable the Global Stack mode for both viewers.
                StudyViewer.SeriesViewer_1X1().Click();
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.GlobalStack);
                Thread.Sleep(4000);
                StudyViewer.SeriesViewer_1X1(2).Click();
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.GlobalStack);
                Thread.Sleep(4000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_30 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.StudyPanels());
                if (step_30)
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


                //step 31: Scroll through the images in the global stack series in both viewers.
                StudyViewer.DragScroll(1, 1, 50, 160);
                StudyViewer.SeriesViewer_1X1(2).Click();
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.DragScroll(1, 1, 5, 20, 2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_31 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.StudyPanels());
                if (step_31)
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


                //step 32: Go to Options  User Preferences and change the settings for the following features-
                //1. 'Set up mouse button functions for All In One Tool' as- LMB - Zoom, MMB - Pan, RMB - Window Level
                //2. 'Default Settings Per Modality' as- layout for CR - 1x2.
                StudyViewer.CloseStudy();
                UserPref.OpenUserPreferences();
                UserPref.SwitchToUserPrefFrame();
                UserPref.AllinOneLMB().SelectByText("Zoom");
                UserPref.AllinOneMMB().SelectByText("Pan");
                UserPref.AllinOneRMB().SelectByText("Window Level");
                UserPref.ModalityDropDown().SelectByText("CR");
                UserPref.LayoutDropDown().SelectByText("1x2");
                result.steps[++ExecutedSteps].status = "Partially Automated";
                UserPref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);


                //step 33:Load a CR modality dataset which has series with multiple images per series 
                //Apply All In One Tool on the images and all available W/L presets.
                result.steps[++ExecutedSteps].status = "Not Automated";


                //step 34: Load a CT modality dataset which has multiple series with multiple images per series.
                //Apply All In One Tool on the images and all available W/L presets.
                /*studies.SeaarchStudy("Accession", Accession[3]);
                studies.SelectStudy("Accession", Accession[3]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);  */
                result.steps[++ExecutedSteps].status = "Not Automated";

                //resetting UserPreferences
                UserPref.OpenUserPreferences();
                UserPref.SwitchToUserPrefFrame();
                UserPref.AllinOneLMB().SelectByText("Window Level");
                UserPref.AllinOneMMB().SelectByText("Zoom");
                UserPref.AllinOneRMB().SelectByText("Pan");
                UserPref.ModalityDropDown().SelectByText("CR");
                UserPref.LayoutDropDown().SelectByText("auto");
                UserPref.CloseUserPreferences();
                login.Logout();
                studies.CloseBrowser();




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
        /// Image Viewing Series
        /// </summary>
        public TestCaseResult Test_27858(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");

                String[] Accession = AccessionList.Split(':');
                String[] PatientID = PatientIDList.Split(':');

                //Step 1 - Login iCA as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //Step 2 - Navigate to Search                
                studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //Step 3 - Load a study with multiple series that contain multiple images. Ensure the scope is set to Image.
                //PatientID[0] => 454-54-5454
                //Accession[0] => 561
                studies.SearchStudy(patientID: PatientID[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.AllinOneTool);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageScope);
                Thread.Sleep(5000);
                if (viewer.GetElementCursorType(viewer.SeriesViewer_1X1()).Equals("default"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 4 - Select series layout 6 Series. Zoom in/out to a series.               
                viewer.SelectToolInToolBar("SeriesViewer2x3");
                //viewer.DoubleClick(viewer.SeriesViewer_1X1());
                viewer.SelectToolInToolBar("ImageScope");
                viewer.ApplyZoom(viewer.SeriesViewer_1X1());
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_4)
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

                //Step 5 - Double click on another series thumbnail.
                //viewer.SelectToolInToolBar("ImageScope");
                IWebElement element2 = BasePage.Driver.FindElement(By.CssSelector("#ModalityDivCT>div:nth-child(2)"));

                var action = new Actions(BasePage.Driver);
                action.MoveToElement(element2, element2.Size.Width / 3, element2.Size.Height / 3);
                action.DoubleClick(element2).Build().Perform();
                PageLoadWait.WaitForFrameLoad(20);
                viewer.SelectToolInToolBar("ImageScope");
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_5 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_5)
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

                //Step 6 - Double click the previous series thumbnail.                
                viewer.DoubleClick(BasePage.Driver.FindElement(By.CssSelector("#ModalityDivCT>div:nth-child(1)")));
                //viewer.SelectToolInToolBar("ImageScope");
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
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

                //Step 7 - Select series layout 1 Series, and view the image that was previously manipulated.
                viewer.SeriesViewer_1X1().Click();
                //viewer.SelectToolInToolBar("ImageScope");
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_7)
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

                //Step 8 - Scroll to another image in series.
                //viewer.SelectToolInToolBar("ImageScope");
                viewer.Scroll(1, 1, 1, "down", "click");
                Thread.Sleep(5000);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_8 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_8)
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

                //Step 9 - Scroll back to the image that was previously manipulated.
                viewer.Scroll(1, 1, 1, "up", "click");
                Thread.Sleep(5000);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_9 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_9)
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

                //Step 10 - Select Reset tool
                viewer.SelectToolInToolBar("Reset");
                Thread.Sleep(5000);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_10 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_10)
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

                //Step 11 - Select Zoom tool.
                viewer.SelectToolInToolBar("Zoom");
                Thread.Sleep(5000);
                if (viewer.GetElementCursorType(viewer.SeriesViewer_1X1()).Equals("n-resize"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 12 - Click LMB on viewport and drag cursor.
                IWebElement element3 = viewer.SeriesViewer_1X1();
                int h = element3.Size.Height;
                int w = element3.Size.Width;
                int i = 0;
                while (i < 4)
                {
                    action.MoveToElement(element3, w / 2, (h / 3)).ClickAndHold().MoveToElement(element3, w / 2, h - (h / 3)).Build().Perform();
                    Thread.Sleep(1500);
                    action.Release().Build().Perform();
                    i++;
                }
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_12 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_12)
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

                //Step 13 - Release the LMB
                if (!((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("firefox"))
                {
                    action.Release().Build().Perform();
                }
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_13 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_13)
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

                //Step 14 - Close study and Logout from ICA
                studies.CloseStudy();
                login.Logout();
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
        /// Image Viewing Series--HTML5 viewer
        /// </summary>
        public TestCaseResult Test2_27858(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");

                String[] Accession = AccessionList.Split(':');
                String[] PatientID = PatientIDList.Split(':');
                StudyViewer viewer = new StudyViewer();
                //Step 1 - Login iCA as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //Step 2 - Navigate to Search                
                studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //Step 3 - Load a study with multiple series that contain multiple images. Ensure the scope is set to Image.
                //PatientID[0] => 454-54-5454
                //Accession[0] => 561
                studies.SearchStudy(patientID: PatientID[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);

                StudyViewer.Html5ViewStudy();
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageScope);
                Thread.Sleep(5000);
                if (viewer.GetElementCursorType(viewer.html5seriesViewer_1X1()).Equals("default"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 4 - Select series layout 6 Series. Zoom in/out to a series.               
                viewer.SelectToolInToolBar("SeriesViewer2x3");
                //viewer.DoubleClick(viewer.SeriesViewer_1X1());                
                viewer.SelectToolInToolBar("ImageScope");
                viewer.SelectToolInToolBar("Zoom");
                viewer.DragMovement(viewer.html5seriesViewer_1X1());
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_4)
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

                //Step 5 - Double click on another series thumbnail.
                //viewer.SelectToolInToolBar("ImageScope");
                IWebElement element2 = BasePage.Driver.FindElement(By.CssSelector("#ModalityDivCT>div:nth-child(2)"));

                var action = new Actions(BasePage.Driver);
                action.MoveToElement(element2, element2.Size.Width / 3, element2.Size.Height / 3);
                action.DoubleClick(element2).Build().Perform();
                PageLoadWait.WaitForFrameLoad(20);
                viewer.SelectToolInToolBar("ImageScope");
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_5 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_5)
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

                //Step 6 - Double click the previous series thumbnail.                
                viewer.DoubleClick(BasePage.Driver.FindElement(By.CssSelector("#ModalityDivCT>div:nth-child(1)")));
                //viewer.SelectToolInToolBar("ImageScope");
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
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

                //Step 7 - Select series layout 1 Series, and view the image that was previously manipulated.
                viewer.html5seriesViewer_1X1().Click();
                //viewer.SelectToolInToolBar("ImageScope");
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.html5seriesViewer_1X1());
                if (step_7)
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

                //Step 8 - Scroll to another image in series.
                //viewer.SelectToolInToolBar("ImageScope");
                viewer.ScrollHTML5(1, 1,"down","click");
                Thread.Sleep(5000);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_8 = studies.CompareImage(result.steps[ExecutedSteps], viewer.html5seriesViewer_1X1());
                if (step_8)
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

                //Step 9 - Scroll back to the image that was previously manipulated.
                viewer.ScrollHTML5(1, 1, "up", "click");
                Thread.Sleep(5000);
                //viewer.ScrollHTML5(1, 1, "up", "click");              
                Thread.Sleep(5000);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_9 = studies.CompareImage(result.steps[ExecutedSteps], viewer.html5seriesViewer_1X1());
                if (step_9)
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

                //Step 10 - Select Reset tool
                viewer.SelectToolInToolBar("Reset");
                Thread.Sleep(5000);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_10 = studies.CompareImage(result.steps[ExecutedSteps], viewer.html5seriesViewer_1X1());
                if (step_10)
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

                //Step 11 - Select Zoom tool.
                viewer.SelectToolInToolBar("Zoom");
                Thread.Sleep(5000);
                if (viewer.GetElementCursorType(viewer.html5seriesViewer_1X1()).Equals("n-resize"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 12 - Click LMB on viewport and drag cursor.
                IWebElement element3 = viewer.html5seriesViewer_1X1();
                int h = element3.Size.Height;
                int w = element3.Size.Width;              
                new Actions(Driver).MoveToElement(element3, w / 2, h / 2).ClickAndHold().MoveToElement(element3, w / 2, h / 4).Build().Perform();
                Thread.Sleep(3000);                   
                
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_12 = studies.CompareImage(result.steps[ExecutedSteps], viewer.html5seriesViewer_1X1());
                if (step_12)
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

                //Step 13 - Release the LMB                
                action.Release().Build().Perform();               
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_13 = studies.CompareImage(result.steps[ExecutedSteps], viewer.html5seriesViewer_1X1());
                if (step_13)
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

                //Step 14 - Close study and Logout from ICA
                studies.CloseStudy();
                login.Logout();
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
        /// Pan Tool
        /// </summary>
        public TestCaseResult Test_27859(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");

                String[] Accession = AccessionList.Split(':');
                String[] PatientID = PatientIDList.Split(':');

                //Step 1 - Login iCA as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
            
                //--Pre condition-- disable connection tool.                
                //PageLoadWait.WaitForPageLoad(20);
                //UserPreferences userpref = new UserPreferences();
                //userpref.OpenUserPreferences();
                //BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                //PageLoadWait.WaitForPageLoad(20);
                //if (BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Selected == false)
                //{
                //    BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Click();
                //    Logger.Instance.InfoLog("Connection tool CB un-selected successfully");
                //}
                //PageLoadWait.WaitForPageLoad(20);
                //userpref.CloseUserPreferences();
                ExecutedSteps++;

                //Step 2 - Navigate to Search                
                studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //Step 3 - Load a study with multiple series that contain multiple images.Ensure the scope is set to Image.
                //PatientID[0] =454-54-5454
                //Accession[0] ==561
                studies.SearchStudy(patientID: PatientID[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step 4 - Select Pan tool
                viewer.SeriesViewer_1X1().Click();
                viewer.DoubleClick(viewer.SeriesViewer_1X1());                
                PageLoadWait.WaitForAllViewportsToLoad(120);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageScope);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                if (viewer.GetElementCursorType(viewer.SeriesViewer_1X1()).Equals("pointer"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 5 - Click LMB on viewport and drag cursor.
                IWebElement element = BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg"));
                viewer.ApplyPan(element);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_5 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_5)
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

                //Step 6 - Release the LMB     
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
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

                //Step 7 - Logout from ICA
                studies.CloseStudy();
                login.Logout();
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
        /// Check how tool affected by layout/reset/slider bar
        /// </summary>
        public TestCaseResult Test_27860(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");

                String[] Accession = AccessionList.Split(':');
                String[] PatientID = PatientIDList.Split(':');

                //Step 1 - Login iCA as Administrator and Navigate to Studies page 
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                //--Pre condition-- disable connection tool.           
                PageLoadWait.WaitForPageLoad(20);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                if (BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Selected == false)
                {
                    BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Click();
                    Logger.Instance.InfoLog("Connection tool CB un-selected successfully");
                }
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();

                ExecutedSteps++;

                //Step 2 - Load a study     
                //PatientID[0] =454-54-5454
                //Accession[0] ==561
                studies.SearchStudy(patientID: PatientID[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(10);
                ExecutedSteps++;

                //Step 3 - Precondition: Apply pan tool to the image
                viewer.SeriesViewer_1X1().Click();
                viewer.DoubleClick(viewer.SeriesViewer_1X1());
                viewer.ApplyPan(viewer.SeriesViewer_1X1());
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
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

                //Step 4 - Select series layout 6 Series                
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x3);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(viewer.BySeriesViewer_XxY(1, 1)));
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(viewer.BySeriesViewer_XxY(1, 2)));
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(viewer.BySeriesViewer_XxY(1, 3)));
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(viewer.BySeriesViewer_XxY(2, 1)));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_4)
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

                //Step 5 - Double click on another series thumbnail.                                            
                viewer.DoubleClick(viewer.Thumbnails()[1]);
                PageLoadWait.WaitForFrameLoad(10);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageScope);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_5 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());
                if (step_5)
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

                //Bug - check
                //Step 6 - Double click the previous series thumbnail.                  
                IWebElement Thambnail = BasePage.Driver.FindElement(By.CssSelector("div[id$='studyPanel_1_thumbnails']>div>div:nth-child(1)"));
                viewer.DoubleClick(viewer.Thumbnails()[0]);
                PageLoadWait.WaitForFrameLoad(10);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageScope);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
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

                //Step 7 - Select layout Series 1, and view the image that was previously manipulated.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_7)
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

                //Step 8 - Scroll to another image in series.                
                /*               IWebElement ele = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_viewerImg"));
                               IWebElement downArrow = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_m_scrollNextImageButton"));
                               ele.Click();
                               downArrow.Click();*/
                StudyViewer.DragScroll(1, 1, 2, 2);
                PageLoadWait.WaitForFrameLoad(5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_8 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_8)
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

                //Step 9 - Scroll back to the image that was previously manipulated.
                /*              IWebElement upArrow = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_m_scrollPreviousImageButton"));
                              upArrow.Click();*/
                StudyViewer.DragScroll(1, 1, 1, 2);
                PageLoadWait.WaitForFrameLoad(5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_9 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_9)
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

                //Step 10 - Select Reset tool
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                Thread.Sleep(3000);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_10 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_10)
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

                //Step 11 - Select Pan tool
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                if (viewer.GetElementCursorType(viewer.SeriesViewer_1X1()).Equals("pointer"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 12 - Click LMB on viewport and drag cursor.          
                /*IWebElement element1 = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_viewerImg"));
                viewer.ApplyPan(element1);*/
                //viewer.SeriesViewer_1X1().Click();
                //viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                viewer.DragMovement(viewer.SeriesViewer_1X1());
                PageLoadWait.WaitForFrameLoad(10);
                ExecutedSteps++;

                //Step 13 - Release the LMB      
                ExecutedSteps++;

                //Step 14 - Close study and Logout from ICA
                studies.CloseStudy();
                login.Logout();
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
        /// Measurement Tools
        /// </summary>
        public TestCaseResult Test_27861(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");

                String[] Accession = AccessionList.Split(':');
                String[] PatientID = PatientIDList.Split(':');

                //Step 1 - Login iCA as Administrator and Navigate to Studies page 
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                //--Pre condition-- disable connection tool.          
                PageLoadWait.WaitForPageLoad(20);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                if (BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Selected == false)
                {
                    BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Click();
                    Logger.Instance.InfoLog("Connection tool CB un-selected successfully");
                }
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();


                ExecutedSteps++;

                //Step 2 - Load a study with multiple series that contain multiple images. 

                //Accession[0] ==>561
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //TODO
                //Step 3 - Click LMB on viewport and hover over measurement tool.
                IList<String> aSubMenu = viewer.GetSubMenuFromReviewTools("Line Measurement");
                if (aSubMenu.Contains("Line Measurement") ||
                    aSubMenu.Contains("Calibration Tool") ||
                    aSubMenu.Contains("Transischial Measurement"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].SetLogs();
                }

                //TODO
                //Step 4 - Hover over the Text submenu.
                IList<String> bSubMenu = viewer.GetSubMenuFromReviewTools("Add Text");
                if (bSubMenu.Contains("Add Text") || bSubMenu.Contains("Edit Text"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 5 - Select the Line tool from the measurement menu.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.LineMeasurement);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForFrameLoad(10);
                if (viewer.GetElementCursorType(viewer.SeriesViewer_1X1()).Equals("crosshair"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 6 - Left click at the point where the line should start.
                int h = viewer.SeriesViewer_1X1().Size.Height;
                int w = viewer.SeriesViewer_1X1().Size.Width;

                viewer.DrawLineMeasurement(viewer.SeriesViewer_1X1(), w / 3, h / 3);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
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

                //Step 7 - Left click at the point where the line should end.
                ExecutedSteps++;

                //Step 8 - Select the Rectangle tool from the measurement menu.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.DrawRectangle);
                if (viewer.GetElementCursorType(viewer.SeriesViewer_1X1()).Equals("crosshair"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 9 - Click LMB on viewport and drag cursor.
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("firefox"))
                {
                    viewer.DrawRectangle(viewer.SeriesViewer_1X1(), 45, 111, 111, 60);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    bool step_9 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                    if (step_9)
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
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Not Automated";
                }

                //Step 10 - Release the LMB
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("firefox"))
                {
                    ExecutedSteps++;
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Not Automated";
                }

                //Step 11 - Select the Ellipse tool from the measurement menu.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x3);
                PageLoadWait.WaitForFrameLoad(10);                
                viewer.DoubleClick(viewer.SeriesViewer_1X2());
                PageLoadWait.WaitForFrameLoad(10);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.DrawEllipse);
                PageLoadWait.WaitForFrameLoad(10);
                if (viewer.GetElementCursorType(viewer.SeriesViewer_1X2()).Equals("crosshair"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 12 - Click LMB on viewport and drag cursor.                
                viewer.DrawEllipse(StudyViewer.SeriesViewer_1X1(), 120, 120, 180, 150);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_12 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_12)
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

                //Step 13 - Release the LMB               
                ExecutedSteps++;

                //Step 14 - Select the ROI tool from the measurement menu.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.DrawROI);
                PageLoadWait.WaitForFrameLoad(5);
                if (viewer.GetElementCursorType(viewer.SeriesViewer_1X2()).Equals("crosshair"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 15 - Click LMB on viewport and drag cursor.
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("firefox"))
                {
                    viewer.DrawROI(viewer.SeriesViewer_1X2(), 200, 34, 400, 210, 190, 280, 150, 330);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    bool step_15 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());
                    if (step_15)
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
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Not Automated";
                }

                //Step 16 - Release the LMB
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("firefox"))
                {
                    ExecutedSteps++;
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Not Automated";
                }


                //Step 17 - Select the Angle tool from the measurement menu.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x3);
                PageLoadWait.WaitForFrameLoad(10);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.AllinOneTool);
                viewer.DoubleClick(viewer.SeriesViewer_1X3());
                PageLoadWait.WaitForFrameLoad(10);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.AngleMeasurement);
                if (viewer.GetElementCursorType(viewer.SeriesViewer_1X3()).Equals("crosshair"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 18 - Left click at the point where the angle should start.
                viewer.DrawAngleMeasurement(viewer.SeriesViewer_1X3(), 200, 150, 120, 160, 160, 180);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_18 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X3());
                if (step_18)
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

                //Step 19 - Place two more points.
                ExecutedSteps++;

                //Step 20 - Select the Cobb Angle tool from the measurement menu.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x3);
                PageLoadWait.WaitForFrameLoad(10);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.AllinOneTool);
                viewer.DoubleClick(viewer.SeriesViewer_2X1());
                PageLoadWait.WaitForFrameLoad(10);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.CobbAngle);
                if (viewer.GetElementCursorType(viewer.SeriesViewer_2X1()).Equals("crosshair"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 21 - Left click at the point where the first line of the angle should start.
                ExecutedSteps++;

                //Step 22 - Left click at the point where the first line of the angle should end.                
                ExecutedSteps++;

                //Step 23 - Place two more points for the second line of the angle.
                viewer.DrawCobbAngle(viewer.SeriesViewer_2X1(), 300, 150, 220, 160, 270, 90, 100, 130);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_23 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_2X1());
                if (step_23)
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

                //Step 24 - Close study and Logout from ICA
                studies.CloseStudy();
                login.Logout();
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
        /// Measurement Tools
        /// </summary>
        public TestCaseResult Test2_27861(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");

                String[] Accession = AccessionList.Split(':');
                String[] PatientID = PatientIDList.Split(':');

                //Step 1 - Login iCA as Administrator and Navigate to Studies page 
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                //--Pre condition-- disable connection tool.
                
                PageLoadWait.WaitForPageLoad(20);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                if (BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Selected == false)
                {
                    BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Click();
                    Logger.Instance.InfoLog("Connection tool CB un-selected successfully");
                }
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();

                ExecutedSteps++;

                //Step 2 - Load a study with multiple series that contain multiple images. 

                //Accession[0] ==>561
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer viewer = new StudyViewer();
                StudyViewer.Html5ViewStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);

                ExecutedSteps++;

                //TODO
                //Step 3 - Click LMB on viewport and hover over measurement tool.
                IList<String> aSubMenu = viewer.GetSubMenuFromReviewTools("Line Measurement");
                if (aSubMenu.Contains("Line Measurement") ||
                    aSubMenu.Contains("Calibration Tool") ||
                    aSubMenu.Contains("Transischial Measurement"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].SetLogs();
                }

                //TODO
                //Step 4 - Hover over the Text submenu.
                IList<String> bSubMenu = viewer.GetSubMenuFromReviewTools("Add Text");
                if (bSubMenu.Contains("Add Text") || bSubMenu.Contains("Edit Text"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 5 - Select the Line tool from the measurement menu.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.LineMeasurement);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(10);
                if (viewer.GetElementCursorType(viewer.html5seriesViewer_1X1()).Equals("crosshair"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 6 - Left click at the point where the line should start.
                int h = viewer.html5seriesViewer_1X1().Size.Height;
                int w = viewer.html5seriesViewer_1X1().Size.Width;

                viewer.DrawLineMeasurement(viewer.html5seriesViewer_1X1(), w / 3, h / 3);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.html5seriesViewer_1X1());
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

                //Step 7 - Left click at the point where the line should end.
                ExecutedSteps++;

                //Step 8 - Select the Rectangle tool from the measurement menu.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.DrawRectangle);
                if (viewer.GetElementCursorType(viewer.html5seriesViewer_1X1()).Equals("crosshair"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 9 - Click LMB on viewport and drag cursor.
                
                    viewer.DrawRectangle(viewer.html5seriesViewer_1X1(), 45, 111, 111, 60);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    bool step_9 = studies.CompareImage(result.steps[ExecutedSteps], viewer.html5seriesViewer_1X1());
                    if (step_9)
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
                /*}
                else
                {
                    result.steps[++ExecutedSteps].status = "Not Automated";
                }*/

                //Step 10 - Release the LMB
                ExecutedSteps++;

                //Step 11 - Select the Ellipse tool from the measurement menu.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x3);
                PageLoadWait.WaitForFrameLoad(10);
                viewer.DoubleClick(viewer.html5seriesViewer_1X2());
                PageLoadWait.WaitForFrameLoad(10);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.DrawEllipse);
                if (viewer.GetElementCursorType(viewer.html5seriesViewer_1X2()).Equals("crosshair"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 12 - Click LMB on viewport and drag cursor.
                viewer.DrawEllipse(StudyViewer.html5seriesViewer_1X2(), 120, 120, 180, 150);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_12 = studies.CompareImage(result.steps[ExecutedSteps], viewer.html5seriesViewer_1X2());
                if (step_12)
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

                //Step 13 - Release the LMB               
                ExecutedSteps++;

                //Step 14 - Select the ROI tool from the measurement menu.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.DrawROI);
                PageLoadWait.WaitForFrameLoad(5);
                if (viewer.GetElementCursorType(viewer.html5seriesViewer_1X2()).Equals("crosshair"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 15 - Click LMB on viewport and drag cursor.
                viewer.DrawROI(viewer.html5seriesViewer_1X2(), 200, 34, 400, 210, 190, 280, 150, 330);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_15 = studies.CompareImage(result.steps[ExecutedSteps], viewer.html5seriesViewer_1X2());
                if (step_15)
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

                //Step 16 - Release the LMB
                ExecutedSteps++;

                //Step 17 - Select the Angle tool from the measurement menu.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x3);
                PageLoadWait.WaitForFrameLoad(10);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.AllinOneTool);
                viewer.DoubleClick(viewer.html5seriesViewer_1X3());
                PageLoadWait.WaitForFrameLoad(10);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.AngleMeasurement);
                if (viewer.GetElementCursorType(viewer.html5seriesViewer_1X3()).Equals("crosshair"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 18 - Left click at the point where the angle should start.
                viewer.DrawAngleMeasurement(viewer.html5seriesViewer_1X3(), 200, 150, 120, 160, 160, 180);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_18 = studies.CompareImage(result.steps[ExecutedSteps], viewer.html5seriesViewer_1X3());
                if (step_18)
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

                //Step 19 - Place two more points.
                ExecutedSteps++;

                //Step 20 - Select the Cobb Angle tool from the measurement menu.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x3);
                PageLoadWait.WaitForFrameLoad(10);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.AllinOneTool);
                viewer.DoubleClick(viewer.html5seriesViewer_2X1());
                PageLoadWait.WaitForFrameLoad(10);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.CobbAngle);
                if (viewer.GetElementCursorType(viewer.html5seriesViewer_2X1()).Equals("crosshair"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 21 - Left click at the point where the first line of the angle should start.
                ExecutedSteps++;

                //Step 22 - Left click at the point where the first line of the angle should end.                
                ExecutedSteps++;

                //Step 23 - Place two more points for the second line of the angle.
                viewer.DrawCobbAngle(viewer.html5seriesViewer_2X1(), 300, 150, 220, 160, 270, 90, 100, 130);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_23 = studies.CompareImage(result.steps[ExecutedSteps], viewer.html5seriesViewer_2X1());
                if (step_23)
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

                //Step 24 - Close study and Logout from ICA
                studies.CloseStudy();
                login.Logout();
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
        /// Edit Tool
        /// </summary>
        public TestCaseResult Test_27862(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String UserName = Config.ph1UserName;
                String Password = Config.phPassword;
                String arUsername = Config.ar1UserName;
                String arPassword = Config.arPassword;
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');
                string windowtitle = "iConnect® Access";

                //Pre-Condition
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                //--Pre condition-- disable connection tool.

                PageLoadWait.WaitForPageLoad(20);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                if (BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Selected == false)
                {
                    BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Click();
                    Logger.Instance.InfoLog("Connection tool CB un-selected successfully");
                }
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                login.Logout();
                PageLoadWait.WaitForPageLoad(20);

                login.LoginIConnect(UserName, Password);
                studies = (Studies)login.Navigate("Studies");
                //Accession[0] =89894
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x2);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.LineMeasurement);
                StudyViewer.DrawLineMeasurement(StudyViewer.SeriesViewer_1X1(), 80, 80);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.DrawEllipse);
                StudyViewer.DrawEllipse(StudyViewer.SeriesViewer_1X1(), 120, 120, 180, 150);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.AngleMeasurement);
                StudyViewer.DrawAngleMeasurement(StudyViewer.SeriesViewer_1X1(), 230, 230, 250, 250, 280, 220);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.AddText);
                StudyViewer.AddTextAnnotation(StudyViewer.SeriesViewer_1X1(), 200, 200, "rib");


                //step 1 -Select the Edit tool from the measurement menu.
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.EditAnnotations);
                if (StudyViewer.GetElementCursorType(StudyViewer.SeriesViewer_1X1()).Equals("default"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                new Actions(BasePage.Driver).MoveToElement(StudyViewer.SeriesViewer_1X1(), 300, 240).ClickAndHold().Build().Perform();

                //step-2 and 3: Click and hold LMB on a measurement,drag it to another location on viewport and release LMB
                StudyViewer.DrawMeasurementTool(StudyViewer.SeriesViewer_1X1(), 80, 80, 80, 20);
                Thread.Sleep(2000);
                ExecutedSteps++;

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_3 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_1X1());
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

                //step-4: Repeat the previous two steps for one of each type of measurement.
                StudyViewer.DrawMeasurementTool(StudyViewer.SeriesViewer_1X1(), 170, 120, 190, 170);
                Thread.Sleep(2000);
                StudyViewer.DrawMeasurementTool(StudyViewer.SeriesViewer_1X1(), 250, 250, 200, 200);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_4 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_1X1());
                if (step_4)
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

                //step-5: Click and hold LMB on a text annotation, drag it to another location, and then release the LMB.
                /*StudyViewer.DrawMeasurementTool(StudyViewer.SeriesViewer_1X1(), 200, 200, 200, 280);               
                ExecutedSteps++;
               /* new Actions(BasePage.Driver).MoveToElement(StudyViewer.SeriesViewer_1X1(), 200, 200).ClickAndHold().Build().Perform();
                Thread.Sleep(3000);
                new Actions(BasePage.Driver).ClickAndHold(StudyViewer.SeriesViewer_1X1()).DragAndDropToOffset(StudyViewer.SeriesViewer_1X1(), 200, 280).Build().Perform();
                new Actions(BasePage.Driver).Release().Build().Perform();  */
                /*result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_5 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_1X1());
                if (step_5)
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
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step-6: Perform Pan/Zoom/Window Level operation on viewport.
                StudyViewer.SeriesViewer_1X2().Click();
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                StudyViewer.DragMovement(StudyViewer.SeriesViewer_1X2());
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                StudyViewer.DragMovement(StudyViewer.SeriesViewer_1X2());
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_6 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_1X2());
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

                //step-7: Click the Printable View link.
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.PrintView);
                var printwindow7 = StudyViewer.SwitchtoNewWindow(windowtitle);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[id='SeriesViewersDiv']")));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_7 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.Printimage());
                if (step_7)
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

                //step-8: Select Layout button, and choose any layout with multiple viewports.
                BasePage.Driver.Close();
                BasePage.Driver.SwitchTo().Window(printwindow7);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x2);
                //IList<IWebElement> viewports1 = BasePage.Driver.FindElements(By.CssSelector("div [id='viewerImgDiv'] img[hadtouchevent='true']"));

                if (StudyViewer.SeriesViewPorts().Count == 4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //pre-condition
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.LineMeasurement);
                StudyViewer.DrawLineMeasurement(StudyViewer.SeriesViewer_2X1(), 80, 80);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.DrawEllipse);
                StudyViewer.DrawEllipse(StudyViewer.SeriesViewer_2X1(), 120, 120, 180, 150);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.CobbAngle);
                StudyViewer.DrawCobbAngle(StudyViewer.SeriesViewer_2X1(), 230, 230, 250, 250, 280, 220, 200, 200);

                //step-9: Select the Edit tool from the measurement menu.
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.EditAnnotations);
                if (StudyViewer.GetElementCursorType(StudyViewer.SeriesViewer_1X1()).Equals("default"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10 and 11 Click and hold LMB on a measurement, drag it to another location on viewport and release LMB.
                Thread.Sleep(2000);
                StudyViewer.DrawMeasurementTool(StudyViewer.SeriesViewer_2X1(), 280, 220, 290, 220);
                Thread.Sleep(2000);
                ExecutedSteps++;

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_11 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_2X1());
                if (step_11)
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

                //step-12: Select another viewport with measurements. 
                StudyViewer.SeriesViewer_2X2().Click();
                IWebElement activeViewport = BasePage.Driver.FindElement(By.CssSelector("img[id$='_1_ctl03_SeriesViewer_2_2_viewerImg'][class='svViewerImg ui-droppable activeSeriesViewer']"));
                if (activeViewport != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //pre-condition
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.LineMeasurement);
                //StudyViewer.DrawLineMeasurement(StudyViewer.SeriesViewer_2X2(), 80, 80);
                //StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.DrawEllipse);
                StudyViewer.DrawEllipse(StudyViewer.SeriesViewer_2X2(), 120, 120, 180, 150);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.AngleMeasurement);
                StudyViewer.DrawAngleMeasurement(StudyViewer.SeriesViewer_2X2(), 230, 230, 250, 250, 280, 220);

                //step-13: Select the Edit tool from the measurement menu.
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.EditAnnotations);
                if (StudyViewer.GetElementCursorType(StudyViewer.SeriesViewer_1X1()).Equals("default"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-14 and 15: Click and hold LMB on a measurement, drag it to another location on viewport and Release LMB.
                StudyViewer.DrawMeasurementTool(StudyViewer.SeriesViewer_2X2(), 250, 250, 250, 200);
                Thread.Sleep(2000);
                ExecutedSteps++;

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_14 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_2X2());
                if (step_14)
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


                //step-16: Select Save Series and wait for saving to complete.                
                /*int count1 = StudyViewer.Thumbnails().Count;
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.SaveSeries);
                PageLoadWait.WaitForFrameLoad(20);                
                int count2 = StudyViewer.Thumbnails().Count;
                if(count2==(count1+1))
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

                //step-17: Reload the study with PR series, and double click the new PR series.
                /*StudyViewer.CloseStudy();
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);               
                var action = new Actions(BasePage.Driver);
                action.DoubleClick(StudyViewer.Thumbnails()[count2]).Build().Perform();
                PageLoadWait.WaitForFrameLoad(20);   
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_17 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_1X1());
                if (step_17)
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
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step 18: Select the Edit tool from the measurement menu.
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.LineMeasurement);
                StudyViewer.DrawEllipse(StudyViewer.SeriesViewer_1X1(), 40, 70, 40, 105);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.EditAnnotations);
                if (StudyViewer.GetElementCursorType(StudyViewer.SeriesViewer_1X1()).Equals("default"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 19 and 20: Click and hold LMB on a measurement, drag it to another location on viewport and Release LMB.
                PageLoadWait.WaitForFrameLoad(20);

                StudyViewer.DrawMeasurementTool(StudyViewer.SeriesViewer_1X1(), 250, 200, 220, 250);
                Thread.Sleep(2000);
                ExecutedSteps++;

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_20 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_1X1());
                if (step_20)
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


                StudyViewer.CloseStudy();
                login.Logout();
                studies.CloseBrowser();

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
        /// Delete
        /// </summary>
        public TestCaseResult Test_27863(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");

                String[] Accession = AccessionList.Split(':');
                String[] PatientID = PatientIDList.Split(':');

                //Step 1 - Login iCA as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //Step 2 - Navigate to Search                
                studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //--Pre condition-- disable connection tool.
                PageLoadWait.WaitForPageLoad(20);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                if (BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Selected == false)
                {
                    BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Click();
                    Logger.Instance.InfoLog("Connection tool CB un-selected successfully");
                }
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);

                //Step 3 - Load a study
                //PatientID[0] =454-54-5454
                //Accession[0] ==561
                studies.SearchStudy(patientID: PatientID[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step 4 - Select a view port and draw a line
                viewer.SeriesViewer_1X1().Click();
                viewer.DrawLineMeasurement(viewer.SeriesViewer_1X1(), 100, 122);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_4)
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

                //Step 5 - Select the Delete tool from the measurement menu.                
                viewer.DeleteAnnotation(viewer.SeriesViewer_1X1(), 100, 122);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_5 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_5)
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

                //Step 6 - Logout from ICA
                studies.CloseStudy();
                login.Logout();
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
        /// Remove All
        /// </summary>
        public TestCaseResult Test_27864(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");

                String[] Accession = AccessionList.Split(':');
                String[] PatientID = PatientIDList.Split(':');

                //Step 1 - Login iCA as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //Step 2 - Navigate to Search                
                studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //--Pre condition-- disable connection tool.
                PageLoadWait.WaitForPageLoad(20);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                if (BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Selected == false)
                {
                    BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Click();
                    Logger.Instance.InfoLog("Connection tool CB un-selected successfully");
                }
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);


                //Step 3 - Load a study
                //PatientID[0] =454-54-5454
                //Accession[0] ==561
                studies.SearchStudy(patientID: PatientID[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step 4 - Select a view port and draw a line measurement and angle measurement
                viewer.SeriesViewer_1X1().Click();
                viewer.DrawLineMeasurement(viewer.SeriesViewer_1X1(), 100, 122);
                viewer.DrawAngleMeasurement(viewer.SeriesViewer_1X1(), 200, 150, 120, 160, 160, 150);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_4)
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

                //Step 5 - Select the Remove All tool from the measurement menu.   
                viewer.SelectToolInToolBar("AllinOneTool");
                viewer.SelectToolInToolBar("RemoveAllAnnotations");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_5 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_5)
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

                //Step 6 - Perform Pan/Zoom/Window Level operation on viewport.
                viewer.SeriesViewer_1X1().Click();
                viewer.SelectToolInToolBar("AllinOneTool");
                viewer.DoubleClick(viewer.SeriesViewer_1X1());
                viewer.SelectToolInToolBar("ImageScope");
                viewer.SelectToolInToolBar("Pan");
                IWebElement element = BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg"));
                viewer.ApplyPan(element);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
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

                //Step 7 - Select the Remove All tool from the measurement menu.
                viewer.SelectToolInToolBar("RemoveAllAnnotations");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_7)
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

                //Step 8 - Select Layout button, and choose any layout with multiple viewports.
                viewer.SelectToolInToolBar("SeriesViewer1x3");
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(viewer.BySeriesViewer_XxY(1, 1)));
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(viewer.BySeriesViewer_XxY(1, 2)));
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(viewer.BySeriesViewer_XxY(1, 3)));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_8 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_8)
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

                //Step 9 - Logout from ICA
                studies.CloseStudy();
                login.Logout();
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
        /// Text Show/Hide
        /// </summary>
        public TestCaseResult Test_27866(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");

                String[] Accession = AccessionList.Split(':');
                String[] PatientID = PatientIDList.Split(':');

                //Step 1 - Login iCA as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //Step 2 - Navigate to Search                
                studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //--Pre condition-- disable connection tool.
                PageLoadWait.WaitForPageLoad(20);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                if (BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Selected == false)
                {
                    BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Click();
                    Logger.Instance.InfoLog("Connection tool CB un-selected successfully");
                }
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);


                //Step 3 - Load a study
                //PatientID[0] =454-54-5454
                //Accession[0] ==561
                studies.SearchStudy(patientID: PatientID[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step 4 - Move the Mouse pointer to the review toolbar and select Toggle text.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ToggleText);
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_4)
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

                //Step 5 - Change layout to different forms and click on toggle text   
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x3);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ToggleText);
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_5 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_5)
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

                //Step 6 - Click on toggle text button 
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ToggleText);
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
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

                //Step 7 - Select printable view
                StudyViewer.SelectToolInToolBar("PrintView");
                PageLoadWait.WaitForFrameLoad(20);
                var PrintWindow = BasePage.Driver.WindowHandles.Last();
                var StudyWindow = BasePage.Driver.CurrentWindowHandle;
                BasePage.Driver.SwitchTo().Window(PrintWindow);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#viewerImg_1_1")));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.PrintView());
                if (step_7)
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
                BasePage.Driver.SwitchTo().Window(PrintWindow).Close();
                BasePage.Driver.SwitchTo().Window(StudyWindow);
                PageLoadWait.WaitForFrameLoad(10);

                //Step 8 - click on toggle text , draw a line to compare with caliper (if calibrated)
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ToggleText);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("img[id$='studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg']")));
                viewer.DrawLineMeasurement(viewer.SeriesViewer_1X1(1), 100, 150);
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 9 - click on toggle Text.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ToggleText);
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_9 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_9)
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

                //Step 10 - Load a study with related study and perform the same to select toggle text on or off with different forms of layout as well as with annotations on.
                NavigateToHistoryPanel();
                IWebElement elementRecord = GetElement("xpath", "//table[@id='gridTablePatientHistory']/tbody/tr[2]");

                var action = new Actions(BasePage.Driver);
                action.DoubleClick(elementRecord).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //IWebElement element = GetElement("id", GetControlId("2SeriesViewer1-1X1"));
                viewer.SeriesViewer_1X1(2).Click();
                viewer.DrawLineMeasurement(viewer.SeriesViewer_1X1(2), 100, 150);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x2);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ToggleText);
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_10 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel(2));
                if (step_10)
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

                //Step 11 - Close study and Logout from ICA
                studies.CloseStudy();
                login.Logout();
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
        /// Orientation tools
        /// </summary>
        public TestCaseResult Test_27867(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");

                String[] Accession = AccessionList.Split(':');
                String[] PatientID = PatientIDList.Split(':');

                //Step 1 - Login iCA as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //Step 2 - Navigate to Search                
                studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //--Pre condition-- disable connection tool.
                PageLoadWait.WaitForPageLoad(20);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                if (BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Selected == false)
                {
                    BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Click();
                    Logger.Instance.InfoLog("Connection tool CB un-selected successfully");
                }
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);


                //Step 3 - Load a CT or MR study with multiple series that have multiple images.
                //PatientID[0] =454-54-5454
                //Accession[0] ==561
                studies.SearchStudy(patientID: PatientID[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);

                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
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

                //Step 4 - Apply a few measurements to the current image.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                viewer.DrawLineMeasurement(viewer.SeriesViewer_1X1(), 100, 122);
                viewer.DrawAngleMeasurement(viewer.SeriesViewer_1X1(), 200, 150, 120, 160, 160, 180);
                viewer.DrawCobbAngle(viewer.SeriesViewer_1X1(), 170, 90, 80, 60, 170, 90, 50, 130);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_4)
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

                //Step 5 - Apply the Rotate Clockwise tool 4 times, checking the orientation markers and measurements each time.   
                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                Thread.Sleep(4000);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                Thread.Sleep(4000);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                Thread.Sleep(4000);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                Thread.Sleep(4000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_5 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_5)
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

                //Step 6 - Apply the Rotate Clockwise tool once. 
                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                Thread.Sleep(4000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
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

                //Step 7 - Apply various tools (zoom, pan, w/l, etc.)   
                viewer.ApplyWindowLevel(viewer.SeriesViewer_1X1());
                viewer.ApplyPan(viewer.SeriesViewer_1X1());
                viewer.ApplyZoom(viewer.SeriesViewer_1X1());
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_7)
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

                //Step 8 - Reset the current image.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                Thread.Sleep(4000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_8 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_8)
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

                //Step 9 - Close study and Logout from ICA
                studies.CloseStudy();
                login.Logout();
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
        /// Auto Window Level Test
        /// </summary>
        public TestCaseResult Test_27868(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");

                String[] Accession = AccessionList.Split(':');
                String[] PatientID = PatientIDList.Split(':');

                //Step 1 - Login iCA as Administrator and Navigate to search 
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //--Pre condition-- disable connection tool.
                PageLoadWait.WaitForPageLoad(20);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                if (BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Selected == false)
                {
                    BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Click();
                    Logger.Instance.InfoLog("Connection tool CB un-selected successfully");
                }
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);


                //Step 2 - Preconditions                
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForFrameLoad(10);
                if (!domainmanagement.IsDomainExist("TestViewingProtocolDomain"))
                {
                    domainmanagement.CreateDomain("TestViewingProtocolDomain", "TestViewingProtocolDomain", 0);
                }
                PageLoadWait.WaitForFrameLoad(10);
                domainmanagement.SelectDomain("TestViewingProtocolDomain");
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForFrameLoad(10);
                domainmanagement.AddPresetForDomain("CR", "test2", "123", "456");
                domainmanagement.AddPresetForDomain("CR", "test3", "500", "500");
                domainmanagement.AddPresetForDomain("MG", "mg1", "100", "900");
                domainmanagement.AddPresetForDomain("MG", "mg2", "200", "900");
                domainmanagement.AddPresetForDomain("MG", "mg3", "300", "900");
                domainmanagement.AddPresetForDomain("CT", "testct2", "100", "300");
                domainmanagement.AddPresetForDomain("CT", "ctvp", "200", "300");
                domainmanagement.SaveButton().Click();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                login.Logout();

                login.LoginIConnect("TestViewingProtocolDomain", "TestViewingProtocolDomain");
                studies = (Studies)login.Navigate("Studies");


                //CR Modality
                //Accession[0] =>29822041

                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step 3 - Select the Auto W/L drop down.  
                IList<String> CR_APresets = new List<String>();
                IList<IWebElement> CR_EPresets = BasePage.Driver.FindElements(By.CssSelector("#StudyToolbar div ul ul li[title]"));
                foreach (IWebElement title in CR_EPresets)
                {
                    CR_APresets.Add(title.GetAttribute("title"));
                }

                if (CR_APresets.Contains("test2:123/456") &&
                    CR_APresets.Contains("test3:500/500"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //ExecutedSteps++;

                //TODO - Dont have
                //Step 4 - Apply each viewing protocol, and scroll through each images in the series and verify image updates correctly.
                viewer.SeriesViewer_1X1().Click();
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul ul li[title='test2:123/456']\").click()");
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                result.steps[++ExecutedSteps].SetPath(testid + "_4_1", ExecutedSteps);
                bool Step_4_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.SeriesViewer_1X2().Click();
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul ul li[title='test3:500/500']\").click()");
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                result.steps[ExecutedSteps].SetPath(testid + "_4_2", ExecutedSteps);
                bool Step_4_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                if (Step_4_1 && Step_4_2)
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
                viewer.CloseStudy();
                //ExecutedSteps++;

                //Step 5 - Change image layout to have more than one image displayed. Manually apply W/L. 
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer viewer1 = StudyViewer.LaunchStudy();
                viewer1.SeriesViewer_1X1().Click();
                viewer1.SelectToolInToolBar("ImageLayout2x2");
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul ul li[title='test2:123/456']\").click()");
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                result.steps[++ExecutedSteps].SetPath(testid + "_5_1", ExecutedSteps);
                bool Step_5_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer1.SeriesViewer_1X1());

                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul ul li[title='test3:500/500']\").click()");
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                result.steps[ExecutedSteps].SetPath(testid + "_5_2", ExecutedSteps);
                bool Step_5_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer1.SeriesViewer_1X1());

                if (Step_5_1 && Step_5_2)
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
                viewer1.CloseStudy();
                //ExecutedSteps++;

                //Step 6 - Change Scope to image. Select an image and apply each preset on the image.
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer viewer2 = StudyViewer.LaunchStudy();
                viewer2.SelectToolInToolBar(IEnum.ViewerTools.ImageScope);
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul ul li[title='test2:123/456']\").click()");
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                result.steps[++ExecutedSteps].SetPath(testid + "_6_1", ExecutedSteps);
                bool Step_6_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer2.SeriesViewer_1X1());

                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul ul li[title='test3:500/500']\").click()");
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                result.steps[ExecutedSteps].SetPath(testid + "_6_2", ExecutedSteps);
                bool Step_6_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer2.SeriesViewer_1X1());
                if (Step_6_1 && Step_6_2)
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
                studies.CloseStudy();
                //ExecutedSteps++;

                //Step 7 - Select the Auto W/L drop down. 
                //MG Modality
                //Accession[1] MS10026
                studies.SearchStudy(AccessionNo: Accession[1], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[1]);
                StudyViewer viewer3 = StudyViewer.LaunchStudy();
                IList<String> MG_APresets = new List<String>();
                IList<IWebElement> MG_EPresets = BasePage.Driver.FindElements(By.CssSelector("#StudyToolbar div ul ul li[title]"));
                foreach (IWebElement title in MG_EPresets)
                {
                    MG_APresets.Add(title.GetAttribute("title"));
                }

                if (MG_APresets.Contains("mg1:100/900") &&
                    MG_APresets.Contains("mg2:200/900") &&
                    MG_APresets.Contains("mg3:300/900"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //ExecutedSteps++;

                //TODO - Study not having multiple images
                //Step 8 - Apply each viewing protocol, and scroll through each images in the series and verify image updates correctly.
                viewer3.SeriesViewer_1X1().Click();
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul ul li[title='mg1:100/900']\").click()");
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                result.steps[++ExecutedSteps].SetPath(testid + "_8_1", ExecutedSteps);
                bool Step_8_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer3.SeriesViewer_1X1());

                viewer3.SeriesViewer_1X2().Click();
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul ul li[title='mg2:200/900']\").click()");
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                result.steps[ExecutedSteps].SetPath(testid + "_8_2", ExecutedSteps);
                bool Step_8_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer3.SeriesViewer_1X2());

                viewer3.SeriesViewer_1X3().Click();
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul ul li[title='mg3:300/900']\").click()");
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                result.steps[ExecutedSteps].SetPath(testid + "_8_3", ExecutedSteps);
                bool Step_8_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer3.SeriesViewer_1X3());

                if (Step_8_1 && Step_8_2 && Step_8_3)
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
                viewer3.CloseStudy();
                //ExecutedSteps++;

                //Step 9 - Apply a preset on the series viewers. (e.g., series viewer1 has mg1, viewer2 has mg2 and viewer3 has mg3)
                studies.SelectStudy("Accession", Accession[1]);
                StudyViewer viewer4 = StudyViewer.LaunchStudy();
                viewer4.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x3);

                viewer4.SeriesViewer_1X1().Click();
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul ul li[title='mg1:100/900']\").click()");
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                result.steps[++ExecutedSteps].SetPath(testid + "_9_1", ExecutedSteps);
                bool Step_9_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer4.SeriesViewer_1X1());

                viewer4.SeriesViewer_1X2().Click();
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul ul li[title='mg2:200/900']\").click()");
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                result.steps[ExecutedSteps].SetPath(testid + "_9_2", ExecutedSteps);
                bool Step_9_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer4.SeriesViewer_1X2());

                viewer4.SeriesViewer_1X3().Click();
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul ul li[title='mg3:300/900']\").click()");
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                result.steps[ExecutedSteps].SetPath(testid + "_9_3", ExecutedSteps);
                bool Step_9_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer4.SeriesViewer_1X3());

                if (Step_9_1 && Step_9_2 && Step_9_3)
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
                studies.CloseStudy();
                //ExecutedSteps++;

                // Step 10 - Change image layout to have more than one images displayed on each viewer. 
                // Apply a different preset on the series viewer.
                studies.SelectStudy("Accession", Accession[1]);
                StudyViewer viewer5 = StudyViewer.LaunchStudy();

                viewer5.SeriesViewer_1X1().Click();
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                viewer5.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x2);
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);


                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul ul li[title='mg1:100/900']\").click()");
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                result.steps[++ExecutedSteps].SetPath(testid + "_10_1", ExecutedSteps);
                bool Step_10_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer5.SeriesViewer_1X1());

                viewer5.SeriesViewer_1X2().Click();
                Thread.Sleep(3000);
                viewer5.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x2);
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul ul li[title='mg2:200/900']\").click()");
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                result.steps[ExecutedSteps].SetPath(testid + "_10_2", ExecutedSteps);
                bool Step_10_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer5.SeriesViewer_1X2());

                viewer5.SeriesViewer_1X3().Click();
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                viewer5.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x2);
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul ul li[title='mg3:300/900']\").click()");
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                result.steps[ExecutedSteps].SetPath(testid + "_10_3", ExecutedSteps);
                bool Step_10_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer5.SeriesViewer_1X3());

                if (Step_10_1 && Step_10_2 && Step_10_3)
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
                //ExecutedSteps++;

                //Step 11 - Apply preset on each viewer.
                viewer5.SeriesViewer_2X1().Click();
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul ul li[title='mg1:100/900']\").click()");
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                result.steps[++ExecutedSteps].SetPath(testid + "_11_1", ExecutedSteps);
                bool Step_11_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer5.studyPanel());

                viewer5.SeriesViewer_2X2().Click();
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul ul li[title='mg2:200/900']\").click()");
                Thread.Sleep(3000);
                result.steps[ExecutedSteps].SetPath(testid + "_11_2", ExecutedSteps);
                bool Step_11_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer5.studyPanel());

                viewer5.SeriesViewer_2X3().Click();
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul ul li[title='mg3:300/900']\").click()");
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                result.steps[ExecutedSteps].SetPath(testid + "_11_3", ExecutedSteps);
                bool Step_11_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer5.studyPanel());

                if (Step_11_1 && Step_11_2 && Step_11_3)
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
                viewer5.CloseStudy();
                //ExecutedSteps++;

                //Step 12 - Select the Auto W/L drop down.
                //CT Modality
                //Accession[2]=> 561

                studies.SearchStudy(AccessionNo: Accession[2], Datasource: EA_131);
                studies.SelectStudy("Accession", Accession[2]);
                StudyViewer viewer6 = StudyViewer.LaunchStudy();

                IList<String> CT_APresets = new List<String>();
                IList<IWebElement> CT_EPresets = BasePage.Driver.FindElements(By.CssSelector("#StudyToolbar div ul ul li[title]"));
                foreach (IWebElement title in CT_EPresets)
                {
                    CT_APresets.Add(title.GetAttribute("title"));
                }

                if (CT_APresets.Contains("testct2:100/300") &&
                    CT_APresets.Contains("ctvp:200/300"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //ExecutedSteps++;

                //Step 13 - Apply each viewing protocol, and verify image updates correctly.
                viewer6.SeriesViewer_1X1().Click();
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul ul li[title='testct2:100/300']\").click()");
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                result.steps[++ExecutedSteps].SetPath(testid + "_13_1", ExecutedSteps);
                bool Step_13_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer6.SeriesViewer_1X1());

                viewer6.SeriesViewer_1X2().Click();
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul ul li[title='ctvp:200/300']\").click()");
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                result.steps[ExecutedSteps].SetPath(testid + "_13_2", ExecutedSteps);
                bool Step_13_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer6.SeriesViewer_1X2());
                if (Step_13_1 && Step_13_2)
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
                studies.CloseStudy();
                login.Logout();
                //ExecutedSteps++;

                //Step 14 - Click (point) on the Auto W/L button.
                login.LoginIConnect(adminUserName, adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SelectDomain("TestViewingProtocolDomain");
                domainmanagement.ClickEditDomain();
                domainmanagement.RemoveAllPresets("CR");
                Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_EditDomainControl_SaveButton")).Click();
                //domainmanagement.SaveDomain();     
                PageLoadWait.WaitForFrameLoad(10);
                login.Logout();

                login.LoginIConnect("TestViewingProtocolDomain", "TestViewingProtocolDomain");
                studies = (Studies)login.Navigate("Studies");
                //CR Modality study.
                //Accession[0] =>29822041
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer viewer7 = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                IWebElement preset = Driver.FindElement(By.CssSelector("#StudyToolbar div ul li[title]"));
                if (preset.GetAttribute("title").Equals("preset"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 15 - Apply Auto W/L
                viewer7.SeriesViewer_1X1().Click();
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                preset.Click();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step_15 = studies.CompareImage(result.steps[ExecutedSteps], viewer7.SeriesViewer_1X1());
                if (step_15)
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

                //Step 16 - Close study and Logout from ICA
                studies.CloseStudy();
                login.Logout();
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
        /// Window Width and Level Presets (Desktop / Tablet)
        /// </summary>
        public TestCaseResult Test_27869(String testid, String teststeps, int stepcount)
        {
            //Fetch the data
            String Contactinfo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ContactInformation");
            String[] info = Contactinfo.Split('=');

            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String Dr1 = "Dr1" + new Random().Next(1, 10000);
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");

                String DomainName = "Domain1" + new Random().Next(1, 10000);
                String RoleName = "Role1" + new Random().Next(1, 10000);

                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String LastNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String FirstNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");

                String[] Accession = AccessionList.Split(':');
                String[] FirstName = FirstNameList.Split(':');
                String[] LastName = LastNameList.Split(':');
                String[] Patient = PatientIDList.Split(':');

                //Step 1 - Default Preconditions                
                ExecutedSteps++;

                //Step 2
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                //--Pre condition-- disable connection tool.
                PageLoadWait.WaitForPageLoad(20);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                if (BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Selected == false)
                {
                    BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Click();
                    Logger.Instance.InfoLog("Connection tool CB un-selected successfully");
                }
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);

                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.CreateDomain(DomainName, DomainName, 0);
                PageLoadWait.WaitForFrameLoad(20);
                domainmanagement.SearchDomain(DomainName);
                domainmanagement.SelectDomain(DomainName);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("grant", 0);
                domainmanagement.AddPresetForDomain("MR", "MR DP1", "1111", "111", "2x2");
                domainmanagement.AddPresetForDomain("MR", "MR DP2", "2222", "222", "2x2");
                domainmanagement.ClickSaveDomain();
                ExecutedSteps++;

                //Step 3 
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SwitchToRoleMgmtFrame();
                rolemanagement.SelectFromList(BasePage.Driver.FindElement(By.CssSelector("#m_listResultsControl_m_resultsSelectorControl_m_selectorList")), DomainName);
                rolemanagement.DomainDropDown().SelectByText(DomainName);
                rolemanagement.ClickNewRoleBtn();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                rolemanagement.CreateRole(DomainName, RoleName);
                PageLoadWait.WaitForFrameLoad(20);

                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(Dr1, DomainName, RoleName);

                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectRole(RoleName);
                rolemanagement.ClickButtonInRole("edit");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                rolemanagement.SwitchTo("id", "UserHomeFrame");
                rolemanagement.UnCheckCheckbox("cssselector", "#ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_viewingprotocolsConfig_ViewingProcotocolsConfigUseDomainLayoutCheckbox");
                rolemanagement.AddPresetForRole("CT", "CT RP1", "3333", "333", "2x2");
                rolemanagement.AddPresetForRole("CT", "CT RP2", "4444", "444", "2x2");
                rolemanagement.SaveBtn().Click();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step 4
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.DomainDropDown().SelectByText(DomainName);
                rolemanagement.SelectRole(DomainName);
                rolemanagement.ClickButtonInRole("edit");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                rolemanagement.SwitchTo("id", "UserHomeFrame");
                rolemanagement.UnCheckCheckbox("cssselector", "#ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_viewingprotocolsConfig_ViewingProcotocolsConfigUseDomainLayoutCheckbox");

                bool MR_DP1_DL = rolemanagement.VerifyPresetsInRole("MR", "2x2", "MR DP1");
                bool MR_DP2_DL = rolemanagement.VerifyPresetsInRole("MR", "2x2", "MR DP2");

                rolemanagement.CloseBtn().Click();
                PageLoadWait.WaitForFrameLoad(20);

                rolemanagement.SelectRole(RoleName);
                rolemanagement.ClickButtonInRole("edit");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                rolemanagement.SwitchTo("id", "UserHomeFrame");
                rolemanagement.UnCheckCheckbox("cssselector", "#ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_viewingprotocolsConfig_ViewingProcotocolsConfigUseDomainLayoutCheckbox");
                bool MR_DP1_RL = rolemanagement.VerifyPresetsInRole("MR", "2x2", "MR DP1");
                bool MR_DP2_RL = rolemanagement.VerifyPresetsInRole("MR", "2x2", "MR DP2");
                bool CT_RP1_RL = rolemanagement.VerifyPresetsInRole("CT", "2x2", "CT RP1");
                bool CT_RP2_RL = rolemanagement.VerifyPresetsInRole("CT", "2x2", "CT RP2");

                if (MR_DP1_DL && MR_DP2_DL && MR_DP1_RL && MR_DP2_RL && CT_RP1_RL && CT_RP2_RL)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 5
                login.Logout();
                login.LoginIConnect(DomainName, DomainName);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");

                bool MR_DP1_DL2 = domainmanagement.VerifyPresetsInDomain("MR", "2x2", "MR DP1");
                bool MR_DP2_DL2 = domainmanagement.VerifyPresetsInDomain("MR", "2x2", "MR DP2");

                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectRole(DomainName);
                rolemanagement.ClickButtonInRole("edit");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                rolemanagement.SwitchTo("id", "UserHomeFrame");
                rolemanagement.UnCheckCheckbox("cssselector", "#ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_viewingprotocolsConfig_ViewingProcotocolsConfigUseDomainLayoutCheckbox");
                bool MR_DP1_RL2 = rolemanagement.VerifyPresetsInRole("MR", "2x2", "MR DP1");
                bool MR_DP2_RL2 = rolemanagement.VerifyPresetsInRole("MR", "2x2", "MR DP2");

                rolemanagement.CloseBtn().Click();
                PageLoadWait.WaitForFrameLoad(20);

                rolemanagement.SelectRole(RoleName);
                rolemanagement.ClickButtonInRole("edit");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                rolemanagement.SwitchTo("id", "UserHomeFrame");

                bool MR_DP1_RL1 = rolemanagement.VerifyPresetsInRole("MR", "2x2", "MR DP1");
                bool MR_DP2_RL1 = rolemanagement.VerifyPresetsInRole("MR", "2x2", "MR DP2");
                bool CT_RP1_RL2 = rolemanagement.VerifyPresetsInRole("CT", "2x2", "CT RP1");
                bool CT_RP2_RL2 = rolemanagement.VerifyPresetsInRole("CT", "2x2", "CT RP2");

                rolemanagement.CloseBtn().Click();
                PageLoadWait.WaitForFrameLoad(20);

                if (MR_DP1_DL2 && MR_DP2_DL2 && MR_DP1_RL2 && MR_DP2_RL2 && MR_DP1_RL1 && MR_DP2_RL1 && CT_RP1_RL2 && CT_RP2_RL2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 6                                
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForFrameLoad(20);
                domainmanagement.AddPresetForDomain("PT", "PT DP1", "5555", "555", "1x2", "domain");
                domainmanagement.AddPresetForDomain("PT", "PT DP2", "6666", "666", "1x2", "domain");
                domainmanagement.SaveButton().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#CloseButton")));
                domainmanagement.CloseAlertButton().Click();
                PageLoadWait.WaitForPageLoad(20);
                ExecutedSteps++;

                //Step 7
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectRole(RoleName);
                rolemanagement.ClickButtonInRole("edit");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                rolemanagement.SwitchTo("id", "UserHomeFrame");
                rolemanagement.UnCheckCheckbox("cssselector", "#ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_viewingprotocolsConfig_ViewingProcotocolsConfigUseDomainLayoutCheckbox");
                rolemanagement.AddPresetForRole("XA", "XA RP1", "7777", "777", "1x2");
                rolemanagement.AddPresetForRole("XA", "XA RP2", "8888", "888", "1x2");
                rolemanagement.SaveBtn().Click();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step 8
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectRole(DomainName);
                rolemanagement.ClickButtonInRole("edit");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                rolemanagement.SwitchTo("id", "UserHomeFrame");
                rolemanagement.UnCheckCheckbox("cssselector", "#ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_viewingprotocolsConfig_ViewingProcotocolsConfigUseDomainLayoutCheckbox");
                bool XA_RP1 = rolemanagement.VerifyPresetsInRole("XA", "1x2", "XA RP1", false);
                bool XA_RP2 = rolemanagement.VerifyPresetsInRole("XA", "1x2", "XA RP2", false);
                //bool XA_RP1 = rolemanagement.VerifyPresetsInRole("XA", "1x2", "XA RP1");
                //bool XA_RP2 = rolemanagement.VerifyPresetsInRole("XA", "1x2", "XA RP2");
                rolemanagement.CloseBtn().Click();
                PageLoadWait.WaitForFrameLoad(20);
                if (XA_RP1 && XA_RP2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 9
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectRole(RoleName);
                rolemanagement.ClickButtonInRole("edit");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                rolemanagement.SwitchTo("id", "UserHomeFrame");
                rolemanagement.UnCheckCheckbox("cssselector", "#ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_viewingprotocolsConfig_ViewingProcotocolsConfigUseDomainLayoutCheckbox");
                rolemanagement.SaveBtn().Click();
                PageLoadWait.WaitForFrameLoad(10);
                login.Logout();

                login.LoginIConnect(Dr1, Dr1);
                bool MR_DP1 = UserPref.VerifyPresetsInUserPreference("MR", "2x2", "MR DP1");
                bool MR_DP2 = UserPref.VerifyPresetsInUserPreference("MR", "2x2", "MR DP2");
                bool CT_RP1 = UserPref.VerifyPresetsInUserPreference("CT", "2x2", "CT RP1");
                bool CT_RP2 = UserPref.VerifyPresetsInUserPreference("CT", "2x2", "CT RP2");
                bool PT_DP1 = UserPref.VerifyPresetsInUserPreference("PT", "1x2", "PT DP1");
                bool PT_DP2 = UserPref.VerifyPresetsInUserPreference("PT", "1x2", "PT DP2");
                bool XA1_RP1 = UserPref.VerifyPresetsInUserPreference("XA", "1x2", "XA RP1");
                bool XA1_RP2 = UserPref.VerifyPresetsInUserPreference("XA", "1x2", "XA RP2");

                if (MR_DP1 && MR_DP2 && CT_RP1 && CT_RP2 && PT_DP1 && PT_DP2 && XA1_RP1 && XA1_RP2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 10
                login.Logout();
                PageLoadWait.WaitForPageLoad(10);

                login.LoginIConnect(DomainName, DomainName);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectRole(RoleName);
                rolemanagement.ClickButtonInRole("edit");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                rolemanagement.SwitchTo("id", "UserHomeFrame");
                rolemanagement.UnCheckCheckbox("cssselector", "#ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_viewingprotocolsConfig_ViewingProcotocolsConfigUseDomainLayoutCheckbox");
                rolemanagement.SaveBtn().Click();
                PageLoadWait.WaitForFrameLoad(20);
                login.Logout();
                PageLoadWait.WaitForPageLoad(10);

                login.LoginIConnect(Dr1, Dr1);

                bool MR_DP3 = UserPref.VerifyPresetsInUserPreference("MR", "2x2", "MR DP1");
                bool MR_DP4 = UserPref.VerifyPresetsInUserPreference("MR", "2x2", "MR DP2");
                bool PT_DP3 = UserPref.VerifyPresetsInUserPreference("PT", "2x2", "PT DP1");
                bool PT_DP4 = UserPref.VerifyPresetsInUserPreference("PT", "2x2", "PT DP2");
                //bool CT_RP3 = UserPref.VerifyPresetsInUserPreference("CT", "2x2", "CT RP1", false);
                //bool CT_RP4 = UserPref.VerifyPresetsInUserPreference("CT", "2x2", "CT RP2", false);
                //bool XA_RP3 = UserPref.VerifyPresetsInUserPreference("XA", "1x2", "XA RP1", false);
                //bool XA_RP4 = UserPref.VerifyPresetsInUserPreference("XA", "1x2", "XA RP2", false);
                bool CT_RP3 = UserPref.VerifyPresetsInUserPreference("CT", "2x2", "CT RP1");
                bool CT_RP4 = UserPref.VerifyPresetsInUserPreference("CT", "2x2", "CT RP2");
                bool XA_RP3 = UserPref.VerifyPresetsInUserPreference("XA", "1x2", "XA RP1");
                bool XA_RP4 = UserPref.VerifyPresetsInUserPreference("XA", "1x2", "XA RP2");

                if (MR_DP3 && MR_DP4 && CT_RP3 && CT_RP4 && PT_DP3 && PT_DP4 && XA_RP3 && XA_RP4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 11
                UserPref.AddPresetAtUserLevel("MR", "MR UP1", "9090", "909", "1x3");
                UserPref.AddPresetAtUserLevel("CT", "CT UP1", "1010", "101", "2x3");
                ExecutedSteps++;

                //Step 12
                bool MR_DP5 = UserPref.VerifyPresetsInUserPreference("MR", "2x2", "MR DP1");
                bool MR_DP6 = UserPref.VerifyPresetsInUserPreference("MR", "2x2", "MR DP2");
                bool MR_UP1 = UserPref.VerifyPresetsInUserPreference("MR", "1x3", "MR UP1");
                bool CT_UP1 = UserPref.VerifyPresetsInUserPreference("CT", "2x2", "CT UP1");
                bool PT_DP5 = UserPref.VerifyPresetsInUserPreference("PT", "2x2", "PT DP1");
                bool PT_DP6 = UserPref.VerifyPresetsInUserPreference("PT", "2x2", "PT DP2");
                bool XA_RP5 = UserPref.VerifyPresetsInUserPreference("XA", "1x2", "XA RP1", false);
                bool XA_RP6 = UserPref.VerifyPresetsInUserPreference("XA", "1x2", "XA RP2", false);

                if (MR_DP5 && MR_DP6 && MR_UP1 && CT_UP1 && PT_DP5 && PT_DP6 && XA_RP5 && XA_RP6)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 13
                login.Logout();
                login.LoginIConnect(DomainName, DomainName);

                //Uncheck 'Use Domain Settings' in Default Settings Per Modality section
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectRole(RoleName);
                rolemanagement.ClickButtonInRole("edit");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                rolemanagement.SwitchTo("id", "UserHomeFrame");
                rolemanagement.UnCheckCheckbox("cssselector", "#ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_viewingprotocolsConfig_ViewingProcotocolsConfigUseDomainLayoutCheckbox");
                rolemanagement.SaveBtn().Click();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                login.Logout();

                //Log back in as the non-admin user
                login.LoginIConnect(Dr1, Dr1);

                bool MR_DP7 = UserPref.VerifyPresetsInUserPreference("MR", "2x2", "MR DP1");
                bool MR_DP8 = UserPref.VerifyPresetsInUserPreference("MR", "2x2", "MR DP2");
                bool MR_UP2 = UserPref.VerifyPresetsInUserPreference("MR", "1x3", "MR UP1");
                bool CT_RP7 = UserPref.VerifyPresetsInUserPreference("CT", "2x2", "CT RP1");
                bool CT_RP8 = UserPref.VerifyPresetsInUserPreference("CT", "2x2", "CT RP2");
                bool CT_UP2 = UserPref.VerifyPresetsInUserPreference("CT", "2x2", "CT UP1");
                bool PT_DP7 = UserPref.VerifyPresetsInUserPreference("PT", "2x2", "PT DP1");
                bool PT_DP8 = UserPref.VerifyPresetsInUserPreference("PT", "2x2", "PT DP2");
                bool XA_RP7 = UserPref.VerifyPresetsInUserPreference("XA", "1x2", "XA RP1");
                bool XA_RP8 = UserPref.VerifyPresetsInUserPreference("XA", "1x2", "XA RP2");

                if (MR_DP7 && MR_DP8 && MR_UP2 && CT_RP7 && CT_RP8 && CT_UP2 && PT_DP7 && PT_DP8 && XA_RP7 && XA_RP8)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 14               
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("studyPerformed", "All Dates");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);
                studies.LaunchStudy();
                StudyViewer.SelectToolInToolBar("UserPreference");
                PageLoadWait.WaitForFrameLoad(10);
                UserPref.ModifyPresetsInToolBarUserPref("MR", "2x3", "MR DP2", "1001", "101");
                UserPref.ModifyPresetsInToolBarUserPref("CT", "1x3", "CT RP2", "2002", "202");
                UserPref.SaveToolBarUserPreferences();
                studies.CloseStudy();

                UserPref.OpenUserPreferences();
                UserPref.SwitchToUserPrefFrame();
                UserPref.ModalityDropDown().SelectByText("MR");
                UserPref.LayoutDropDown().SelectByText("2x3");
                UserPref.PresetsDropDown().SelectByText("MR DP2");

                string presetName = UserPref.PresetNameTextBox().GetAttribute("value").ToString();
                var widthField = UserPref.WidthTextBox().GetAttribute("value").ToString();
                var levelField = UserPref.LevelTextBox().GetAttribute("value").ToString();

                UserPref.ModalityDropDown().SelectByText("CT");
                UserPref.LayoutDropDown().SelectByText("1x3");
                UserPref.PresetsDropDown().SelectByText("CT RP2");

                string presetName_1 = UserPref.PresetNameTextBox().GetAttribute("value").ToString();
                var widthField_1 = UserPref.WidthTextBox().GetAttribute("value").ToString();
                var levelField_1 = UserPref.LevelTextBox().GetAttribute("value").ToString();

                if (presetName.Equals("MR DP2") && widthField.Equals("1001") && levelField.Equals("101")
                    && presetName_1.Equals("CT RP2") && widthField_1.Equals("2002") && levelField_1.Equals("202"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 15
                UserPref.ModalityDropDown().SelectByText("MR");
                UserPref.LayoutDropDown().SelectByText("2x3");
                UserPref.PresetsDropDown().SelectByText("MR DP2");
                UserPref.CloseUserPreferences();
                studies.SearchStudy(AccessionNo: Accession[3], Modality: "MR", Datasource: EA_131);
                studies.SelectStudy("Accession", Accession[3]);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_15 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_15)
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
                studies.CloseStudy();

                //Step 16                
                UserPref.OpenUserPreferences();
                UserPref.SwitchToUserPrefFrame();
                UserPref.ModalityDropDown().SelectByText("CT");
                UserPref.LayoutDropDown().SelectByText("1x3");
                UserPref.PresetsDropDown().SelectByText("CT RP2");
                UserPref.CloseUserPreferences();
                studies.SearchStudy(AccessionNo: Accession[2], Modality: "CT", Datasource: EA_131);
                studies.SelectStudy("Accession", Accession[2]);
                studies.LaunchStudy();

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_16 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_16)
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //Setp 17  
                //Open User Preferences from toolbar
                //Define a Width/Level Presets for each modality available in the Modality dropdown list.

                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.UserPreference);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                UserPref.SwitchToToolBarUserPrefFrame();

                IList<IWebElement> options = BasePage.Driver.FindElements(By.CssSelector("div[id='SystemConfigDiv'] select[id='ViewingProtocolsControl_DropDownListModalities'] option"));
                string[] modality = new string[options.Count];
                int p = 0;
                foreach (IWebElement option in options)
                {
                    string r = option.Text;
                    modality[p] = r;
                    p++;
                }
                for (int i = 0; i < modality.Length; i++)
                {
                    BasePage.Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
                    UserPref.ModalityDropDown().SelectByText(modality[i]);
                    UserPref.PresetNameTextBox().Clear();
                    UserPref.PresetNameTextBox().SendKeys("Preset " + i);
                    UserPref.WidthTextBox().Clear();
                    UserPref.WidthTextBox().SendKeys("100" + i);
                    UserPref.LevelTextBox().Clear();
                    UserPref.LevelTextBox().SendKeys("10" + i);
                    UserPref.AddModifyBtn().Click();
                }
                UserPref.SaveToolBarUserPreferences();
                studies.CloseStudy();
                login.Logout();
                ExecutedSteps++;

                //Step 18
                //1.Test Data - Aneurysm CT/MR can be used. Ensure the DOB has a valid value.
                //2. Login client as the non-admin user under the domain.
                //3. Load a MR which has CT as related study.
                login.LoginIConnect(Dr1, Dr1);
                studies = (Studies)login.Navigate("Studies");

                studies.SearchStudy(AccessionNo: Accession[3], Datasource: EA_131);
                studies.SelectStudy("Accession", Accession[3]);
                StudyViewer viewer1 = StudyViewer.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_18 = studies.CompareImage(result.steps[ExecutedSteps], viewer1.studyPanel());
                if (step_18)
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

                //Step 19 - Select a MR series view, Click the W/L Preset icon
                IList<String> APresets = new List<String>();
                IList<IWebElement> EPresets = BasePage.Driver.FindElements(By.CssSelector("#StudyToolbar div ul ul li[title]"));
                foreach (IWebElement title in EPresets)
                {
                    APresets.Add(title.GetAttribute("title"));
                }
                if (APresets.Contains("MR DP1:1111/111") &&
                    APresets.Contains("MR DP2:1001/101") &&
                    APresets.Contains("MR UP1:9090/909"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 20 - Apply each MR presets on different MR series viewer.                  
                viewer1.SeriesViewer_1X1().Click();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(10);
                Thread.Sleep(5000);
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul ul li[title='MR DP1:1111/111']\").click()");
                result.steps[++ExecutedSteps].SetPath(testid + "_1", ExecutedSteps);
                bool Step_20_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer1.SeriesViewer_1X1());

                viewer1.SeriesViewer_1X2().Click();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(10);
                Thread.Sleep(5000);
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul ul li[title='MR DP2:1001/101']\").click()");
                result.steps[ExecutedSteps].SetPath(testid + "_2", ExecutedSteps);
                bool Step_20_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer1.SeriesViewer_1X2());

                viewer1.SeriesViewer_1X3().Click();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(10);
                Thread.Sleep(5000);
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul ul li[title='MR DP2:1001/101']\").click()");
                result.steps[ExecutedSteps].SetPath(testid + "_3", ExecutedSteps);
                bool Step_20_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer1.SeriesViewer_1X3());
                if (Step_20_1 && Step_20_2 && Step_20_3)
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
                studies.CloseStudy();

                // The Aneurysm study does not have - MR which has CT as related study
                //Step 21 - Attempt to apply the W/L presets from CT presets.
                result.steps[++ExecutedSteps].status = "Hold";

                //Step 22 
                studies.ClearSearchBtn().Click();
                studies.SearchStudy(AccessionNo: Accession[2], Datasource: EA_131);
                studies.SelectStudy("Accession", Accession[2]);
                StudyViewer viewer2 = StudyViewer.LaunchStudy();
                IList<String> CT_APresets = new List<String>();
                IList<IWebElement> CT_EPresets = BasePage.Driver.FindElements(By.CssSelector("#StudyToolbar div ul ul li[title]"));
                foreach (IWebElement title in CT_EPresets)
                {
                    CT_APresets.Add(title.GetAttribute("title"));
                }
                if (CT_APresets.Contains("CT RP1:3333/333") &&
                    CT_APresets.Contains("CT RP2:2002/202") &&
                    CT_APresets.Contains("CT UP1:1010/101"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 23
                viewer2.SeriesViewer_1X1().Click();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(10);
                Thread.Sleep(5000);
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul ul li[title='CT RP1:3333/333']\").click()");
                result.steps[++ExecutedSteps].SetPath(testid + "_1", ExecutedSteps);
                bool Step_23_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer2.SeriesViewer_1X1());

                viewer2.SeriesViewer_1X2().Click();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(10);
                Thread.Sleep(5000);
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul ul li[title='CT RP2:2002/202']\").click()");
                result.steps[ExecutedSteps].SetPath(testid + "_2", ExecutedSteps);
                bool Step_23_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer2.SeriesViewer_1X2());

                viewer2.SeriesViewer_1X3().Click();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(10);
                Thread.Sleep(5000);
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul ul li[title='CT UP1:1010/101']\").click()");
                result.steps[ExecutedSteps].SetPath(testid + "_3", ExecutedSteps);
                bool Step_23_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer2.SeriesViewer_1X3());
                if (Step_23_1 && Step_23_2 && Step_23_3)
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
                studies.CloseStudy();
                login.Logout();

                //Step 24
                login.LoginIConnect(adminUserName, adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DomainName);
                domainmanagement.SelectDomain(DomainName);
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForFrameLoad(10);
                domainmanagement.AddPresetForDomain("DX", "DX1", "1111", "333");
                domainmanagement.AddPresetForDomain("MG", "MG1", "888", "444");
                domainmanagement.SaveButton().Click();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                login.Logout();

                login.LoginIConnect(DomainName, DomainName);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(LastName: LastName[2], FirstName: FirstName[0], Datasource: EA_131);
                studies.SelectStudy("Accession", Accession[7]);
                StudyViewer viewer3 = StudyViewer.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool Step_24 = studies.CompareImage(result.steps[ExecutedSteps], viewer3.studyPanel());
                if (Step_24)
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

                //Step 25
                string DX_Preset = BasePage.Driver.FindElement(By.CssSelector("#StudyToolbar div ul li[title]")).GetAttribute("title");
                viewer3.SeriesViewer_1X3().Click();
                PageLoadWait.WaitForAllViewportsToLoad(10);
                Thread.Sleep(5000);
                string MG_Preset = BasePage.Driver.FindElement(By.CssSelector("#StudyToolbar div:nth-of-type(2) ul li[title]")).GetAttribute("title");
                if (DX_Preset.Equals("DX1:1111/333") && MG_Preset.Equals("MG1:888/444"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 26
                viewer3.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x3);
                viewer3.SeriesViewer_1X1().Click();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(10);
                Thread.Sleep(5000);
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul li[title='DX1:1111/333']\").click()");
                result.steps[++ExecutedSteps].SetPath(testid + "_1", ExecutedSteps);
                bool Step_26_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer3.SeriesViewer_1X1());

                viewer3.SeriesViewer_1X3().Click();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(10);
                Thread.Sleep(5000);
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div:nth-of-type(2) ul li[title='MG1:888/444']\").click()");
                result.steps[ExecutedSteps].SetPath(testid + "_2", ExecutedSteps);
                bool Step_26_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer3.SeriesViewer_1X3());
                if (Step_26_1 && Step_26_2)
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
                studies.CloseStudy();

                //Step 27                
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForFrameLoad(20);
                domainmanagement.AddPresetForDomain("CT", "CT1", "700", "800");
                //domainmanagement.AddPresetForDomain("PT", "PT1", "600", "500");   
                domainmanagement.ClickSaveEditDomain();

                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[9], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[9]);
                StudyViewer viewer4 = StudyViewer.LaunchStudy();
                viewer4.SeriesViewer_1X1().Click();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(10);
                Thread.Sleep(5000);
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul li[title='CT1:700/800']\").click()");
                result.steps[++ExecutedSteps].SetPath(testid + "_1", ExecutedSteps);
                bool Step_27_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer4.SeriesViewer_1X1());

                viewer4.SeriesViewer_2X1().Click();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(10);
                Thread.Sleep(5000);
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div:nth-of-type(2) ul ul li[title='PT DP1:5555/555']\").click()");
                Thread.Sleep(5000);
                result.steps[ExecutedSteps].SetPath(testid + "_2", ExecutedSteps);
                bool Step_27_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer4.SeriesViewer_2X1());
                if (Step_27_1 && Step_27_2)
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
                viewer4.CloseStudy();

                //Step 28                
                ExecutedSteps++;

                //Step 29                           
                studies.SearchStudy(AccessionNo: Accession[3], Datasource: EA_131);
                studies.SelectStudy("Accession", Accession[3]);
                StudyViewer viewer5 = StudyViewer.LaunchStudy();

                viewer5.SeriesViewer_1X1().Click();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(10);
                Thread.Sleep(5000);
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul ul li[title='MR DP1:1111/111']\").click()");
                result.steps[++ExecutedSteps].SetPath(testid + "_1", ExecutedSteps);
                bool Step_29_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer5.SeriesViewer_1X1());

                viewer5.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_2", ExecutedSteps);
                bool Step_29_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer5.SeriesViewer_1X1());

                for (int i = 1; i < 14; i++)
                {
                    viewer5.ClickDownArrowbutton(1, 1);
                }
                Thread.Sleep(5000);
                result.steps[ExecutedSteps].SetPath(testid + "_3", ExecutedSteps);
                bool Step_29_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer5.SeriesViewer_1X1());

                if (Step_29_1 && Step_29_2 && Step_29_3)
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

                //Step 30
                viewer5.SeriesViewer_2X1().Click();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(10);
                Thread.Sleep(5000);
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul ul li[title='MR DP1:1111/111']\").click()");
                result.steps[++ExecutedSteps].SetPath(testid + "_1", ExecutedSteps);
                bool Step_30_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer5.SeriesViewer_2X1());

                viewer5.ClickDownArrowbutton(2, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_2", ExecutedSteps);
                bool Step_30_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer5.SeriesViewer_2X1());

                for (int i = 1; i < 80; i++)
                {
                    viewer5.ClickDownArrowbutton(2, 1);
                }
                Thread.Sleep(5000);
                result.steps[ExecutedSteps].SetPath(testid + "_3", ExecutedSteps);
                bool Step_30_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer5.SeriesViewer_2X1());

                if (Step_30_1 && Step_30_2 && Step_30_3)
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
                viewer5.CloseStudy();

                //Logout from ICA
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
        /// Invert
        /// </summary>
        public TestCaseResult Test_27870(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");

                String[] Accession = AccessionList.Split(':');
                String[] PatientID = PatientIDList.Split(':');

                //Step 1 - Login iCA as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //Step 2 - Navigate to Search                
                studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //--Pre condition-- disable connection tool.
                PageLoadWait.WaitForPageLoad(20);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                if (BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Selected == false)
                {
                    BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Click();
                    Logger.Instance.InfoLog("Connection tool CB un-selected successfully");
                }
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);


                //Step 3 - Load a study 
                //PatientID[0] => 454-54-5454
                //Accession[0] => 561
                studies.SearchStudy(patientID: PatientID[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step 4 - Select an image and click the invert tool.
                viewer.SeriesViewer_1X1().Click();
                viewer.SelectToolInToolBar("Invert");
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_4)
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

                //Step 5 - Click the invert tool again.
                viewer.SelectToolInToolBar("Invert");
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_5 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_5)
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

                //Step 6 - Apply various tools (W/L, Zoom, Pan, etc.)
                viewer.ApplyWindowLevel(viewer.SeriesViewer_1X1());
                viewer.ApplyPan(viewer.SeriesViewer_1X1());
                viewer.ApplyZoom(viewer.SeriesViewer_1X1());
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
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

                //Step 7 - Apply the invert tool.
                viewer.SelectToolInToolBar("Invert");
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_7)
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

                //Step 8 - Apply various tools (W/L, Zoom, Pan, etc.).
                viewer.ApplyWindowLevel(viewer.SeriesViewer_1X1());
                viewer.ApplyPan(viewer.SeriesViewer_1X1());
                viewer.ApplyZoom(viewer.SeriesViewer_1X1());
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_8 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_8)
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

                //Step 9 - Apply the reset tool.
                viewer.SelectToolInToolBar("Reset");
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_9 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_9)
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

                //Step 10 - Apply the invert tool.
                viewer.SelectToolInToolBar("Invert");
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_10 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_10)
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

                //Step 11 - Click the Printable View link.
                StudyViewer.SelectToolInToolBar("PrintView");
                PageLoadWait.WaitForFrameLoad(20);
                var PrintWindow = BasePage.Driver.WindowHandles.Last();
                var StudyWindow = BasePage.Driver.CurrentWindowHandle;
                BasePage.Driver.SwitchTo().Window(PrintWindow);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#viewerImg_1_1")));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_11 = studies.CompareImage(result.steps[ExecutedSteps], viewer.PrintView());
                if (step_11)
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

                //Step 12 - Close the printable view
                BasePage.Driver.SwitchTo().Window(PrintWindow).Close();
                BasePage.Driver.SwitchTo().Window(StudyWindow);
                PageLoadWait.WaitForFrameLoad(10);
                ExecutedSteps++;

                //Step 13 - Select Save *^>^* Save Series.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 14 - Display the PR series.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 15 - Load another study and then re-load the one with the saved inverted image. Check the PR series.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 16 - Close study and Logout from ICA
                studies.CloseStudy();
                login.Logout();
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
        /// Magnifier
        /// </summary>
        public TestCaseResult Test_27871(String testid, String teststeps, int stepcount)
        {
            //Fetch the data
            String Contactinfo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ContactInformation");
            String[] info = Contactinfo.Split('=');

            //Declare and initialize variables         
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

                //Step 1 - Select the 2.0x magnifier from the magnifier menu. 
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");

                //--Pre condition-- disable connection tool.
                PageLoadWait.WaitForPageLoad(20);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                if (BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Selected == false)
                {
                    BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Click();
                    Logger.Instance.InfoLog("Connection tool CB un-selected successfully");
                }
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);

                //Accession[0] => 561
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();

                //html-4 view
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Magnifierx2);
                PageLoadWait.WaitForFrameLoad(3);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_1)
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
                viewer.CloseZoom();
                StudyViewer.CloseStudy();

                //Step 2 - Select the 2.0x magnifier from the magnifier menu. html-5 view                
                studies.SelectStudy("Accession", Accession[0]);
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("chrome"))
                {
                    StudyViewer.Html5ViewStudy();
                    PageLoadWait.WaitForFrameLoad(20);
                    StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Magnifierx2);
                    int h = StudyViewer.html5seriesViewer_1X2().Size.Height;
                    int w = StudyViewer.html5seriesViewer_1X2().Size.Width;
                    new Actions(BasePage.Driver).MoveToElement(StudyViewer.html5seriesViewer_1X2(), w / 2, h / 2).ClickAndHold().MoveToElement(StudyViewer.html5seriesViewer_1X2(), w / 2, h / 4).Build().Perform();
                    Thread.Sleep(2000);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    bool step_11 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.html5seriesViewer_1X2());
                    if (step_11)
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
                    new Actions(BasePage.Driver).Release().Build().Perform();
                    StudyViewer.CloseStudy();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Not Automated";
                }

                //Step 3 - Move the cursor over the magnifier.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 4 - Move the magnifier around the viewport.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 5 - Scroll the mouse wheel forwards one click.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 6 - Move the magnifier around the viewport.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 7 - Scroll the mouse wheel forwards one click.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 8 - Move the magnifier around the viewport.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 9 - Scroll the mouse wheel backwards one click.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 10 - Scroll the mouse wheel backwards another click.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 11 - Select the 3.0x magnifier from the magnifier menu, move the cursor over it.
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                viewer.SeriesViewer_1X1().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Magnifierx3);
                PageLoadWait.WaitForFrameLoad(3);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_10 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_10)
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
                viewer.CloseZoom();

                //Step 12 - Select the 4.0x magnifier from the magnifier menu, move the cursor over it.
                viewer.SeriesViewer_1X1().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Magnifierx4);
                PageLoadWait.WaitForFrameLoad(3);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_111 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_111)
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
                viewer.CloseZoom();

                //Bug
                //Step 13 - Change the layout to one with at least 4 images displayed.                                
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                //PageLoadWait.WaitForFrameLoad(3);
                Thread.Sleep(3000);
                //new Actions(BasePage.Driver).Click(StudyViewer.SeriesViewer_1X1()).Release().Build().Perform();
                //viewer.CloseZoom();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x2);
                PageLoadWait.WaitForFrameLoad(3);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_12 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_12)
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
                //viewer.CloseZoom();

                //Step 14 - Apply one of the magnifiers on these images.
                IWebElement element13 = viewer.SeriesViewer_1X1();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Magnifierx2);
                PageLoadWait.WaitForFrameLoad(3);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_13 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_13)
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
                viewer.CloseZoom();

                //Step 15 - Change the view to 2 series and load another series in the second viewer.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x2);
                PageLoadWait.WaitForFrameLoad(5);
                Thread.Sleep(3000);
                //viewer.CloseZoom();

                IWebElement sourceElement = Driver.FindElement(By.CssSelector("#ModalityDivCT div:nth-child(2)"));
                IWebElement targetElement = viewer.SeriesViewer_1X2();

                new Actions(Driver).DragAndDrop(sourceElement, targetElement).Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Thread.Sleep(50000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_14 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_14)
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

                //Step 16 - Apply one of the magnifiers and attempt to move between series.
                viewer.SeriesViewer_1X1().Click();
                Thread.Sleep(3000);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Magnifierx2);
                PageLoadWait.WaitForFrameLoad(3);
                viewer.SeriesViewer_1X2().Click();
                PageLoadWait.WaitForFrameLoad(3);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_15 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_15)
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
                viewer.CloseZoom();

                //Step 17 - Apply the rotate and flip tools to one of the series.
                viewer.SeriesViewer_1X2().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                PageLoadWait.WaitForFrameLoad(2);
                Thread.Sleep(3000);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.FlipVertical);
                PageLoadWait.WaitForFrameLoad(2);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_16 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_16)
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

                //Step 18 - Apply one of the magnifiers to the series that the tools were applied to.
                viewer.SeriesViewer_1X2().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Magnifierx2);
                PageLoadWait.WaitForFrameLoad(2);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_17 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_17)
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
                viewer.CloseZoom();

                //Step 19 - Apply various other tools to the same series.
                viewer.SeriesViewer_1X2().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.AllinOneTool);
                PageLoadWait.WaitForFrameLoad(2);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.FlipVertical);
                PageLoadWait.WaitForFrameLoad(3);
                Thread.Sleep(3000);
                viewer.ApplyPan(viewer.SeriesViewer_1X2());
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_18 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_18)
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

                //Step 20 - Apply one of the magnifiers to the series that the tools were applied to.
                viewer.SeriesViewer_1X2().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Magnifierx2);
                PageLoadWait.WaitForFrameLoad(2);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_19 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_19)
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

                //Step 21 - With the magnifier still being applied, click the Printable View link.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_20 = studies.CompareImage(result.steps[ExecutedSteps], viewer.ReviewtoolBar());
                if (step_20)
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
                viewer.CloseZoom();

                //Close study and Logout from ICA
                studies.CloseStudy();
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
        /// Image or Series Scope Tool
        /// </summary>
        public TestCaseResult Test_27872(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");

                String[] Accession = AccessionList.Split(':');
                String[] PatientID = PatientIDList.Split(':');

                //Step 1 - Login iCA as Administrator and Navigate to Studies page
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                //--Pre condition-- disable connection tool.
                PageLoadWait.WaitForPageLoad(20);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                if (BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Selected == false)
                {
                    BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Click();
                    Logger.Instance.InfoLog("Connection tool CB un-selected successfully");
                }
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);


                ExecutedSteps++;

                //Step 2 - Load a study from a patient that has multiple studies with multiple series that contain multiple images.
                //PatientID[0] =>64684
                studies.SearchStudy(patientID: PatientID[0], Datasource: EA_131);
                studies.SelectStudy("Patient ID", PatientID[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                bool series = viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("enabled");
                if (step_2 && series)
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

                //Step 3 - Change the scope to image.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageScope);
                if (viewer.GetReviewToolImage("Image Scope").GetAttribute("class").Contains("enabled"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 4 - Change the layout to one with multiple images displayed.    
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(5);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x2);
                PageLoadWait.WaitForFrameLoad(5);
                Thread.Sleep(3000);
                if (viewer.GetReviewToolImage("Image Layout 2x2").GetAttribute("class").Contains("enabled"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 5 - Apply the W/L tool to one of the images.                                                      
                viewer.ApplyWindowLevel(viewer.SeriesViewer_1X1());
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_5 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_5)
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

                //Step 6 - Apply the Auto W/L tool to the same image.                                
                viewer.ApplyAutoWindowLevel(viewer.SeriesViewer_1X1());
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
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

                //Step 7 - Select the image and apply the Invert tool.                
                viewer.SelectToolInToolBar(IEnum.ViewerTools.AllinOneTool);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                Thread.Sleep(5000);
                //BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("img[id$= 'studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg']")));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_7)
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

                //Step 8 - Apply the Zoom tool to an image.
                viewer.ApplyZoom(viewer.SeriesViewer_1X1());
                Thread.Sleep(10000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_8 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_8)
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

                //Step 9 - Apply the Pan tool to a different image.
                viewer.SeriesViewer_1X1().Click();
                PageLoadWait.WaitForFrameLoad(3);
                viewer.ScrollByKey(1, 1, 1, "down");
                PageLoadWait.WaitForFrameLoad(2);
                Thread.Sleep(3000);
                viewer.SeriesViewer_1X1().Click();
                PageLoadWait.WaitForFrameLoad(5);
                Thread.Sleep(3000);
                viewer.ApplyPan(viewer.SeriesViewer_1X1());
                Thread.Sleep(10000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_9 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_9)
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

                //Step 10 - Apply the Rotate Clockwise tool to an image.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_10 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_10)
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

                //Step 11 - Apply the Rotate Counter Clockwise
                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateCounterclockwise);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_11 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_11)
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

                //Step 12 - Apply the Horizontal Flip tool to an image.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.FlipHorizontal);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_12 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_12)
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

                //Step 13 - Apply the Vertical Flip tool to an image.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.FlipVertical);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_13 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_13)
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

                //Step 14 - Select one of the images that had one or more tools applied to it and click the Reset.                
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_14 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_14)
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

                //Step 15 - Select another image that had one ore more tools applied to it and change the scope from image to series.
                viewer.SeriesViewer_1X1().Click();
                PageLoadWait.WaitForFrameLoad(3);
                viewer.ScrollByKey(1, 1, 1, "up");
                PageLoadWait.WaitForFrameLoad(3);
                Thread.Sleep(3000);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesScope);
                PageLoadWait.WaitForFrameLoad(3);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_15 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_15 && viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("enabled"))
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

                //Step 16 - Click Reset.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_16 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_16)
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

                //Step 17 - Apply the W/L tool to one of the images.                
                viewer.ApplyWindowLevel(viewer.SeriesViewer_1X1());
                PageLoadWait.WaitForFrameLoad(5);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_17 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_17)
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

                //Step 18 - Apply the Auto W/L tool to a different image.
                viewer.SeriesViewer_1X1().Click();
                PageLoadWait.WaitForFrameLoad(3);
                viewer.ScrollByKey(1, 1, 1, "down");
                PageLoadWait.WaitForFrameLoad(5);
                Thread.Sleep(3000);
                viewer.ApplyAutoWindowLevel(viewer.SeriesViewer_1X1());
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_18 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_18)
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

                //Step 19 - Apply the Invert tool.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.AllinOneTool);
                PageLoadWait.WaitForFrameLoad(5);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                PageLoadWait.WaitForFrameLoad(5);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_19 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_19)
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


                //Step 20 - Apply the Zoom tool to an image.
                viewer.ApplyZoom(viewer.SeriesViewer_1X1());
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_20 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_20)
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


                //Step 21 - Apply the Pan tool to a different image.
                viewer.SeriesViewer_1X1().Click();
                PageLoadWait.WaitForFrameLoad(3);
                viewer.ScrollByKey(1, 1, 1, "up");
                PageLoadWait.WaitForFrameLoad(3);
                viewer.ApplyPan(viewer.SeriesViewer_1X1());
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_21 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_21)
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

                //Step 22 - Apply the Rotate Clockwise tool to an image.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_22 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_22)
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

                //Step 23 - Apply the Rotate Counter Clockwise
                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateCounterclockwise);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_23 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_23)
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

                //Step 24 - Apply the Horizontal Flip tool to an image.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.FlipHorizontal);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_24 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_24)
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

                //Step 25 - Apply the Vertical Flip tool to an image.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.FlipVertical);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_25 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_25)
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

                //Step 26 - Set the Series view to 2 series.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_26 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_26)
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

                //Step 27 - Load another series into the new viewer.
                IWebElement sourceElement = BasePage.Driver.FindElement(By.CssSelector("div[id='ModalityDivMR']>div:nth-child(3)"));
                IWebElement targetElement = BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_viewerImg"));
                new Actions(Driver).DragAndDrop(sourceElement, targetElement).Build().Perform();
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_27 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_27 && viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("enabled"))
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

                //Step 28 - Change the layout of the second series (the one just displayed) to one with multiple images displayed.
                targetElement.Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x2);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_28 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_28)
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

                //Step 29 - Apply various tools W/L
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                viewer.DragMovement(targetElement);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_29 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_29)
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

                //Step 30 - Apply Zoom
                viewer.ApplyZoom(targetElement);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_30 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_30)
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

                //Step 31 - Apply Pan
                viewer.ApplyPan(targetElement);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_31 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_31)
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

                //Step 32 - Apply Rotate
                targetElement.Click();
                PageLoadWait.WaitForFrameLoad(2);
                viewer.ScrollByKey(1, 1, 1, "down");
                PageLoadWait.WaitForFrameLoad(2);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_32 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_32)
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

                //Step 33 - Apply Flip
                viewer.SelectToolInToolBar(IEnum.ViewerTools.FlipHorizontal);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_33 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_33)
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

                //Step 34 - Apply Invert
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_34 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_34)
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

                //Step 35 - Apply Reset
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_35 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_35)
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

                //Step 36 - Select the first series that was originally displayed.
                IWebElement series1x1 = Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg"));
                series1x1.Click();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_36 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_36 && viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("enabled"))
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

                //Step 37 - Apply various tools W/L
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                viewer.DragMovement(series1x1);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_37 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_37)
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

                //Step 38 - Apply Zoom
                viewer.ApplyZoom(series1x1);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_38 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_38)
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

                //Step 39 - Apply Pan
                viewer.ApplyPan(series1x1);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_39 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_39)
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

                //Step 40 - Apply Rotate
                series1x1.Click();
                PageLoadWait.WaitForFrameLoad(2);
                viewer.ScrollByKey(1, 1, 1, "down");
                PageLoadWait.WaitForFrameLoad(2);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_40 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_40)
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

                //Step 41 - Apply Flip
                viewer.SelectToolInToolBar(IEnum.ViewerTools.FlipHorizontal);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_41 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_41)
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

                //Step 42 - Apply Invert
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_42 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_42)
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

                //Step 43 - Apply Reset
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_43 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_43)
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

                //Step 44 - Change the scope to Image.                
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageScope);
                if (viewer.GetReviewToolImage("Image Scope").GetAttribute("class").Contains("enabled"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 45 - Re-select the second series.
                PageLoadWait.WaitForFrameLoad(3);
                IWebElement targetElement_1 = BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_viewerImg"));
                targetElement_1.Click();
                if (viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("enabled"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 46 - Change the scope to series
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesScope);
                if (viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("enabled"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 47 - Apply various tools W/L
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                viewer.DragMovement(targetElement_1);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_47 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_47)
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

                //Step 48 - Apply Zoom
                viewer.ApplyZoom(targetElement_1);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_48 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_48)
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

                //Step 49 - Apply Pan
                viewer.ApplyPan(targetElement_1);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_49 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_49)
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

                //Step 50 - Apply Rotate
                targetElement_1.Click();
                PageLoadWait.WaitForFrameLoad(2);
                viewer.ScrollByKey(1, 1, 1, "down");
                PageLoadWait.WaitForFrameLoad(2);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_50 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_50)
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

                //Step 51 - Apply Flip
                viewer.SelectToolInToolBar(IEnum.ViewerTools.FlipHorizontal);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_51 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_51)
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

                //Step 52 - Apply Invert
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_52 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_52)
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

                //Step 53 - Apply Reset
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_53 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_53)
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

                //Step 54 - Set the view to 4 series.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x2);
                PageLoadWait.WaitForFrameLoad(5);
                if (viewer.GetReviewToolImage("Series Viewer 2x2").GetAttribute("class").Contains("enabled"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 55 - Load 2 series from a different study into the bottom two viewers and set their layouts to ones with multiple images.
                IWebElement sourceElement1 = Driver.FindElement(By.CssSelector("div[id='ModalityDivMR']>div:nth-child(3)"));
                IWebElement targetElement1 = Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_1_viewerImg"));
                new Actions(Driver).DragAndDrop(sourceElement1, targetElement1).Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                targetElement1.Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x2);
                PageLoadWait.WaitForFrameLoad(5);

                IWebElement sourceElement2 = Driver.FindElement(By.CssSelector("div[id='ModalityDivMR']>div:nth-child(4)"));
                IWebElement targetElement2 = Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_2_viewerImg"));
                new Actions(Driver).DragAndDrop(sourceElement2, targetElement2).Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                targetElement2.Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x2);
                PageLoadWait.WaitForFrameLoad(5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_55 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_55 && viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("enabled"))
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

                //Step 56 - Select each series that is displayed, one by one - 1x1
                viewer.SeriesViewer_1X1().Click();
                PageLoadWait.WaitForFrameLoad(2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_56 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_56 && viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("enabled"))
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

                //Step 57 - Select each series that is displayed, one by one - 1x2
                viewer.SeriesViewer_1X2().Click();
                PageLoadWait.WaitForFrameLoad(2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_57 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_57 && viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("enabled"))
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

                //Step 58 - Select each series that is displayed, one by one - 2x1
                viewer.SeriesViewer_2X1().Click();
                PageLoadWait.WaitForFrameLoad(2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_58 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_58 && viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("enabled"))
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

                //Step 59 - Select each series that is displayed, one by one - 2x2
                viewer.SeriesViewer_2X2().Click();
                PageLoadWait.WaitForFrameLoad(2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_59 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_59 && viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("enabled"))
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

                //Step 60 - Change the scope to image 
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageScope);
                if (viewer.GetReviewToolImage("Image Scope").GetAttribute("class").Contains("enabled"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 61 - Select each series that is displayed, one by one - 1x1
                viewer.SeriesViewer_1X1().Click();
                PageLoadWait.WaitForFrameLoad(2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_61 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_61 && viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("enabled"))
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

                //Step 62 - Select each series that is displayed, one by one - 1x2
                viewer.SeriesViewer_1X2().Click();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_62 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_62 && viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("enabled"))
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

                //Step 63 - Select each series that is displayed, one by one - 2x1
                viewer.SeriesViewer_2X1().Click();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_63 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_63 && viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("enabled"))
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

                //Step 64 - Select each series that is displayed, one by one - 2x2
                viewer.SeriesViewer_2X2().Click();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_64 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_64 && viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("enabled"))
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

                //Step 65 - Apply tools to one image in each series.          
                viewer.SeriesViewer_1X1().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageScope);
                viewer.ApplyPan(viewer.SeriesViewer_1X1());
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                PageLoadWait.WaitForFrameLoad(2);

                viewer.SeriesViewer_1X2().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageScope);
                viewer.ApplyPan(viewer.SeriesViewer_1X2());
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                PageLoadWait.WaitForFrameLoad(2);

                viewer.SeriesViewer_2X1().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageScope);
                viewer.ApplyPan(viewer.SeriesViewer_2X1());
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                PageLoadWait.WaitForFrameLoad(2);

                viewer.SeriesViewer_2X2().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageScope);
                viewer.ApplyPan(viewer.SeriesViewer_2X2());
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                PageLoadWait.WaitForFrameLoad(2);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_65 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_65)
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

                //Step 66 - With one of the images that tools were applied to selected, change the scope to series.
                viewer.SeriesViewer_1X1().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageScope);
                //result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.GetReviewToolImage("Image Scope").GetAttribute("class").Contains("enabled"))
                {
                    result.steps[ExecutedSteps++].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps++].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 67 - Begin to apply a tool to a modified image in another series.
                viewer.SeriesViewer_2X1().Click();
                viewer.ApplyZoom(viewer.SeriesViewer_2X1());
                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_67 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_67)
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

                //Step 68 - Switch back and forth between image and series scope while applying tools to all of the loaded series.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 69 - Close study and Logout from ICA
                login.CloseStudy();
                login.Logout();
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
        /// Study Information
        /// </summary>
        public TestCaseResult Test_28001(String testid, String teststeps, int stepcount)
        {
            //Fetch the data
            String Contactinfo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ContactInformation");
            String[] info = Contactinfo.Split('=');

            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String Username = "User1" + new Random().Next(1, 10000);
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');

                //Step 1 - Open browser window, and go to http//*^<^*servername*^>^*/WebAccess/Default.ashx 
                login.DriverGoTo(login.url);
                ExecutedSteps++;

                //Step 2 - Login as a Administrator, and create a new test user
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                //--Pre condition-- disable connection tool.
                PageLoadWait.WaitForPageLoad(20);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                if (BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Selected == false)
                {
                    BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Click();
                    Logger.Instance.InfoLog("Connection tool CB un-selected successfully");
                }
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);

                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(Username, DefaultDomain, DefaultRoleName, 1, Email);
                ExecutedSteps++;

                //Step 3 - Logout as a Administrator user
                login.Logout();
                ExecutedSteps++;

                //Precondition new user login and logout -
                Thread.Sleep(10000);
                login.DriverGoTo(login.url);
                login.LoginIConnect(Username, Username);
                studies = (Studies)login.Navigate("Studies");
                //Accession[0] 11665475
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_91);
                PageLoadWait.WaitForPageLoad(20);
                login.Logout();


                //Step 4 - Login with account created for testing
                login.LoginIConnect(Username, Username);
                ExecutedSteps++;

                //Step 5 - Load any study.        
                studies = (Studies)login.Navigate("Studies");
                //Accession[0] 11665475
                login.SearchStudy(AccessionNo: Accession[0], Datasource: EA_91);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                login.SelectStudy("Accession", Accession[0]);
                login.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(10);
                ExecutedSteps++;

                //Step 6 - Verify the study information                
                string StudyInfo = StudyViewer.PatientInfoTab();
                if (StudyInfo.ToUpper().Equals("BONY,ROSE,4879485"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 7 - Close Study
                login.CloseStudy();
                ExecutedSteps++;

                //Step 8 - Logout from ICA
                login.Logout();
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
        /// Print Tool
        /// </summary>
        public TestCaseResult Test_28002(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String UserName = Config.ph1UserName;
                String Password = Config.phPassword;
                String arUsername = Config.ar1UserName;
                String arPassword = Config.arPassword;
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String EmailId = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");
                String Name = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                String EmailReason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Reason");
                String[] Accession = AccessionList.Split(':');
                String Printmsg = "Please note that the print settings displayed here needs to match your browser's print settings or else the print output may not be as expected.";



                //Step 1 -Pre-Condition
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                //--Pre condition-- disable connection tool.
                PageLoadWait.WaitForPageLoad(20);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                if (BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Selected == false)
                {
                    BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Click();
                    Logger.Instance.InfoLog("Connection tool CB un-selected successfully");
                }
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                login.Logout();
                PageLoadWait.WaitForPageLoad(20);

                login.LoginIConnect(UserName, Password);
                UserPref.OpenUserPreferences();
                UserPref.SwitchToUserPrefFrame();
                new SelectElement(BasePage.Driver.FindElement(By.CssSelector("select[id$='_PaperSizeDropDownList']"))).SelectByText("Letter");
                IWebElement landscape3 = BasePage.Driver.FindElement(By.CssSelector("#PrintToolSettingsControl_PaperOrientationRadioButtonList_1 "));
                landscape3.Click();
                UserPref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                login.Logout();
                ExecutedSteps++;

                //Step-2 login as ph and load a study from Studies page, change viewer layout to 1x1.
                login.LoginIConnect(UserName, Password);
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                //Accession[0] => 89894
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div[id$='studyPanelDiv_1']")));
                IWebElement studypanel = BasePage.Driver.FindElement(By.CssSelector("div[id$='studyPanelDiv_1']"));
                IList<IWebElement> viewports = BasePage.Driver.FindElements(By.CssSelector("div [id='viewerImgDiv'] img[hadtouchevent='true']"));

                if (studypanel != null && viewports.Count == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-3 Open User Preferences, verify default settings in Print Preferences.
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.UserPreference);
                UserPref.SwitchToToolBarUserPrefFrame();
                IWebElement papersize = BasePage.Driver.FindElement(By.CssSelector("div.borderClass select[id$='_PaperSizeDropDownList']>option[selected]"));
                IWebElement landscape = BasePage.Driver.FindElement(By.CssSelector("#PrintToolSettingsControl_PaperOrientationRadioButtonList_1 "));
                string pprsizeOption = papersize.Text;
                if (pprsizeOption.Equals("Letter") && landscape.Selected == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //step-4 Close User Preferences. Open Printable View from review toolbar 
                //(validate image displaying state & Print Preferences)
                UserPref.SaveToolBarUserPreferences();
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.PrintView);
                IWebElement printtool = BasePage.Driver.FindElement(By.CssSelector("ul>li[title='Print View']"));
                string windowtitle = "iConnect® Access";
                var printwindow = StudyViewer.SwitchtoNewWindow(windowtitle);
                IWebElement footer = BasePage.Driver.FindElement(By.CssSelector("div[id='PrintViewFooterDiv'] span[id='lit_NotOfClinicalUse']"));
                IWebElement printpref = BasePage.Driver.FindElement(By.CssSelector("div[id='printToolSettingsDiv']"));
                IWebElement dropdown = BasePage.Driver.FindElement(By.CssSelector("#PrintToolSettingsControl_PaperSizeDropDownList"));
                IWebElement orientation = BasePage.Driver.FindElement(By.CssSelector("#PrintToolSettingsControl_PaperOrientationRadioButtonList"));
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[id='SeriesViewersDiv']")));

                if (footer != null && printpref != null && dropdown != null && orientation != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-5 Verify images captured in the Printable View

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_5 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.Printimage());
                if (step_5)
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

                //step-6 Verify the compression message and not for diagnostic use text
                IWebElement Notfordiagnostic = BasePage.Driver.FindElement(By.CssSelector("#lit_NotOfClinicalUse"));
                IWebElement lossyCompressed = BasePage.Driver.FindElement(By.CssSelector("#PrintCompressionText"));
                if (Notfordiagnostic != null && lossyCompressed != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-7 Verify Print Preferences in the Printable View
                IList<String> OptionNames = new List<String>();
                IList<IWebElement> pprsizeOptions = BasePage.Driver.FindElements(By.CssSelector("div.borderClass select[id$='_PaperSizeDropDownList']>option"));
                foreach (IWebElement option in pprsizeOptions)
                {
                    string name = option.Text;
                    OptionNames.Add(name);
                }
                IWebElement radiobutton1 = BasePage.Driver.FindElement(By.CssSelector("label[for$='_PaperOrientationRadioButtonList_0']"));
                IWebElement radiobutton2 = BasePage.Driver.FindElement(By.CssSelector("label[for$='_PaperOrientationRadioButtonList_1']"));
                IWebElement landscapechk = BasePage.Driver.FindElement(By.CssSelector("input[id$='_PaperOrientationRadioButtonList_1']"));
                IWebElement printbutton = BasePage.Driver.FindElement(By.CssSelector("#PrintButton"));
                IWebElement papersize1 = BasePage.Driver.FindElement(By.CssSelector("div.borderClass select[id$='_PaperSizeDropDownList']>option[selected]"));
                IWebElement message = BasePage.Driver.FindElement(By.CssSelector("#PrinterSettingAttentationLabel"));
                foreach (string option in OptionNames) { Logger.Instance.InfoLog("Option in Print window -- " + option); }
                Logger.Instance.InfoLog("Paper Size Selected in Print window -- " + papersize1.Text + " -- "+ pprsizeOption.Equals(papersize1.Text));
                Logger.Instance.InfoLog("Radio Button 1 text -- " + radiobutton1.Text + " -- " + radiobutton1.Text.Equals("Portrait"));
                Logger.Instance.InfoLog("Radio Button 2 text -- " + radiobutton2.Text + " -- " + radiobutton2.Text.Equals("Landscape"));
                Logger.Instance.InfoLog("Print message -- " + message.GetAttribute("innerHTML") + message.GetAttribute("innerHTML").Equals(Printmsg));
                if (OptionNames.Contains("A3") && OptionNames.Contains("A4") && OptionNames.Contains("A5")
                    && OptionNames.Contains("Letter") && OptionNames.Contains("Legal") && radiobutton1.Text.Equals("Portrait") &&
                    radiobutton2.Text.Equals("Landscape") && pprsizeOption.Equals(papersize1.Text) && landscapechk.Selected == true
                    && message.GetAttribute("innerHTML").Equals(Printmsg))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-8 Close the Printable View.
                BasePage.Driver.Close();
                BasePage.Driver.SwitchTo().Window(printwindow);
                ExecutedSteps++;

                //step-9 Change viewer series layout to 3x3 and apply measurements, pan/zoom/WL
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x3);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                StudyViewer.SeriesViewer_1X2().Click();
                StudyViewer.DrawLine(StudyViewer.SeriesViewer_1X2(), 100, 100);
                IList<IWebElement> viewports1 = BasePage.Driver.FindElements(By.CssSelector("div [id='viewerImgDiv'] img[hadtouchevent='true']"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_9 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.SeriesViewer_1X2());
                if (StudyViewer.SeriesViewPorts().Count == 6)
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


                //step-10 Open Printable View.
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.PrintView);
                var printwindow10 = StudyViewer.SwitchtoNewWindow(windowtitle);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[id='SeriesViewersDiv']")));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_10 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.Printimage());
                if (step_10)
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

                //step-11 Press Ctrl+P.
                /*string p="\u0070";
                Actions action = new Actions(BasePage.Driver);
                action.KeyDown(Keys.Control).SendKeys(p).Perform();
                Thread.Sleep(3000);
                action.KeyDown(Keys.Escape).KeyUp(Keys.Escape).Perform();
                action.SendKeys(Keys.Escape);
                action.SendKeys(OpenQA.Selenium.Keys.Escape);
                System.Windows.Forms.SendKeys.SendWait("{ESC}");
                ExecutedSteps++;*/
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step-12 Set browser print settings the same as in Print Preferences of Printable View. Click Print.
                result.steps[++ExecutedSteps].status = "Not Automated";


                //step-13 Verify the printout.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step 14:Close the Printable View. Change image layout to 1x2. Display Report in Study Viewer.                
                BasePage.Driver.Close();
                BasePage.Driver.SwitchTo().Window(printwindow);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                //Accession[1] => DSQ00000083
                studies.SearchStudy(AccessionNo: Accession[1], Datasource: EA_131);
                studies.SelectStudy("Accession", Accession[1]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout1x2);
                IWebElement Reporticon = BasePage.Driver.FindElement(By.CssSelector("div[id$='_1_reportIcon']"));
                Reporticon.Click();
                PageLoadWait.WaitForFrameLoad(20);
                //BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("m_studyPanels_m_studyPanel_1_m_reportViewer_reportFrame");
                IWebElement Reportview = BasePage.Driver.FindElement(By.CssSelector("div[id='m_studyPanels_m_studyPanel_1_reportViewerContainer']"));
                IWebElement Imageview = BasePage.Driver.FindElement(By.CssSelector("div[id$='_1_ctl03_CompositeViewerDiv']"));
                if (Reportview.Displayed == true && Imageview.Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 15: Open Printable View.(Image Print View)
                StudyViewer.ViewerReportListButton().Click();
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div[id$='studyPanel_1_m_reportViewer_reportListContainer']")));
                StudyViewer.SelectItemInStudyViewerList("Type", "Report/SR", "StudyPanel", "report");
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div[id='m_studyPanels_m_studyPanel_1_reportViewerContainer']")));
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.PrintView);
                var printwindow15 = StudyViewer.SwitchtoNewWindow(windowtitle);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#divTextReport")));
                IWebElement reportimage = BasePage.Driver.FindElement(By.CssSelector("#divTextReport"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_15 = studies.CompareImage(result.steps[ExecutedSteps], reportimage);
                if (step_15)
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

                BasePage.Driver.Close();

                //step 16: Open Printable View.(Report Print View)
                BasePage.Driver.SwitchTo().Window(printwindow15);
                StudyViewer.SwitchtoNewWindow(windowtitle);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[id='SeriesViewersDiv']")));
                IWebElement printimage15 = BasePage.Driver.FindElement(By.CssSelector("div[id='SeriesViewersDiv']"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_16 = studies.CompareImage(result.steps[ExecutedSteps], printimage15);
                if (step_16)
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

                BasePage.Driver.Close();
                BasePage.Driver.SwitchTo().Window(printwindow15);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                //BasePage.Driver.SwitchTo().Window(printwindow2);

                //step 17:From image Printable View set Paper Orientation to Portrait. 
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step 18:Verify printout files.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step 19:Load a study with multiple series
                StudyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                //Accession[0] => 89894
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x2);
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.SeriesViewer_1X2().Click();
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout1x2);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                StudyViewer.DragMovement(StudyViewer.SeriesViewer_1X1());
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                StudyViewer.DragMovement(StudyViewer.SeriesViewer_1X2());
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.DrawEllipse);
                StudyViewer.DrawEllipse(StudyViewer.SeriesViewer_1X1(), 120, 120, 180, 150);
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.PrintView);
                var printwindow2 = StudyViewer.SwitchtoNewWindow(windowtitle);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[id='SeriesViewersDiv']")));
                IWebElement printimage = BasePage.Driver.FindElement(By.CssSelector("div[id='SeriesViewersDiv']"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_19 = studies.CompareImage(result.steps[ExecutedSteps], printimage);
                if (step_19)
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

                //step 20:Select a different Paper Orientation and Size and Print. 
                result.steps[++ExecutedSteps].status = "Not Automated";


                //step 21:Verify printout.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step 22: Load a MR or CT study with multiple series, change layout to 2x2,Turn on Localizer line, Scroll images
                BasePage.Driver.Close();
                BasePage.Driver.SwitchTo().Window(printwindow2);
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                //Accession[0] => 89894
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x2);
                PageLoadWait.WaitForFrameLoad(20);
                //StudyViewer.DragMovement(StudyViewer.SeriesViewer_1X2());
                StudyViewer.SeriesViewer_1X2().Click();
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(8000);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.DragScroll(1, 2, 8, 26);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_22 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.studyPanel());
                if (step_22)
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

                //step 23:Open Printable View by clicking Print View from review toolbar
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.PrintView);
                var printwindow3 = StudyViewer.SwitchtoNewWindow(windowtitle);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[id='SeriesViewersDiv']")));
                IWebElement printimage3 = BasePage.Driver.FindElement(By.CssSelector("div[id='SeriesViewersDiv']"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_23 = studies.CompareImage(result.steps[ExecutedSteps], printimage3);
                if (step_23)
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

                //step 24:Verify printout.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step 25:prior
                BasePage.Driver.Close();
                BasePage.Driver.SwitchTo().Window(printwindow3);
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                //Accession[0] => 89894
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                studies.NavigateToHistoryPanel();
                studies.ChooseColumns(new string[] { "Accession" });
                studies.OpenPriors(new string[] { "Accession" }, new string[] { Accession[0] });
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.SeriesViewer_1X2().Click();
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(8000);
                StudyViewer.DragScroll(1, 2, 8, 26);
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.SeriesViewer_1X2(2).Click();
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(8000);
                StudyViewer.DragScroll(1, 2, 8, 26);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.SeriesViewer_1X2().Click();
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.PrintView);
                var printwindow24 = StudyViewer.SwitchtoNewWindow(windowtitle);
                BasePage.Driver.Close();
                BasePage.Driver.SwitchTo().Window(printwindow24);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.SeriesViewer_1X2(2).Click();
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.PrintView);
                var printwindow25 = StudyViewer.SwitchtoNewWindow(windowtitle);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[id='SeriesViewersDiv']")));
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[id='SeriesViewersDiv']")));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_25 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.Printimage());
                if (step_25)
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


                //step 26:Print both Printable Views and compare printouts
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step 27:
                BasePage.Driver.Close();
                BasePage.Driver.SwitchTo().Window(printwindow25);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                //Accession[0] => 89894
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                studies.EmailStudy(EmailId, Name, EmailReason, 1);
                String pinnumber = "";
                pinnumber = studies.FetchPin();
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;



                //step 28 and 29: As the email receiver to open the link received through email and print
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step 30:Login in ICA. In User Preferences dialog Go to Print Preferences section 
                login.Logout();
                login.LoginIConnect(UserName, Password);
                UserPref.OpenUserPreferences();
                UserPref.SwitchToUserPrefFrame();
                IWebElement papersize2 = BasePage.Driver.FindElement(By.CssSelector("div.borderClass select[id$='_PaperSizeDropDownList']>option[selected]"));
                IWebElement landscape2 = BasePage.Driver.FindElement(By.CssSelector("#PrintToolSettingsControl_PaperOrientationRadioButtonList_1 "));
                string pprsizeOption2 = papersize2.Text;
                if (pprsizeOption2.Equals("Letter") && landscape2.Selected == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }



                //step 31: Change Print Preferences in User Preferences, save changes
                //Logout and Re-login as the same user. Navigate to Options--> User Preferences dialog
                new SelectElement(BasePage.Driver.FindElement(By.CssSelector("select[id$='_PaperSizeDropDownList']"))).SelectByText("A4");
                BasePage.Driver.FindElement(By.CssSelector("#PrintToolSettingsControl_PaperOrientationRadioButtonList_0")).Click();
                UserPref.CloseUserPreferences();
                login.Logout();
                login.LoginIConnect(UserName, Password);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                UserPref.OpenUserPreferences();
                UserPref.SwitchToUserPrefFrame();
                IWebElement papersize3 = BasePage.Driver.FindElement(By.CssSelector("div.borderClass select[id$='_PaperSizeDropDownList']>option[selected]"));
                IWebElement Portrait3 = BasePage.Driver.FindElement(By.CssSelector("#PrintToolSettingsControl_PaperOrientationRadioButtonList_0 "));
                string pprsizeOption3 = papersize3.Text;
                if (pprsizeOption3.Equals("A4") && Portrait3.Selected == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 32:Load study, open Printable View.
                UserPref.CloseUserPreferences();
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                //Accession[0] => 89894
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.PrintView);
                string windowtitle1 = "iConnect® Access";
                var printwindow4 = StudyViewer.SwitchtoNewWindow(windowtitle1);

                IWebElement papersize4 = BasePage.Driver.FindElement(By.CssSelector("div.borderClass select[id$='_PaperSizeDropDownList']>option[selected]"));
                IWebElement Portraitchk = BasePage.Driver.FindElement(By.CssSelector("input[id$='_PaperOrientationRadioButtonList_0']"));
                if (pprsizeOption3.Equals(papersize4.Text) && Portraitchk.Selected == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 33:Log out and login as a new user 
                login.Logout();
                login.LoginIConnect(arUsername, arPassword);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                UserPref.OpenUserPreferences();
                UserPref.SwitchToUserPrefFrame();
                IWebElement papersize5 = BasePage.Driver.FindElement(By.CssSelector("div.borderClass select[id$='_PaperSizeDropDownList']>option[selected]"));
                IWebElement landscape5 = BasePage.Driver.FindElement(By.CssSelector("#PrintToolSettingsControl_PaperOrientationRadioButtonList_1 "));
                string pprsizeOption5 = papersize5.Text;
                if (pprsizeOption5.Equals("Letter") && landscape5.Selected == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 34:Logout. In Service Tool -- Viewer Tab -- Print Tool Settings sub-tab, add a new paper 
                login.Logout();
                result.steps[++ExecutedSteps].status = "Hold";

                //step 35:Login ICA. Open User Preferences -- Print Preferences -- Standard Paper Size.
                login.LoginIConnect(UserName, Password);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                UserPref.OpenUserPreferences();
                UserPref.SwitchToUserPrefFrame();
                IWebElement papersize6 = BasePage.Driver.FindElement(By.CssSelector("div.borderClass select[id$='_PaperSizeDropDownList']>option[selected]"));
                string pprsizeOption6 = papersize6.Text;
                if (pprsizeOption6.Equals("A4"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 36:Select the newly added one page size and Landscape for orientation. Save.
                //Load a study with multiple series/images. Apply annotation, measurement, pan, zoom, 
                //series layout in Study Viewer. Open Printable View.
                IWebElement landscape6 = BasePage.Driver.FindElement(By.CssSelector("#PrintToolSettingsControl_PaperOrientationRadioButtonList_1 "));
                landscape6.Click();
                UserPref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                //Accession[0] => 89894
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                StudyViewer.SeriesViewer_1X2().Click();
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.DrawEllipse);
                StudyViewer.DrawElipse(StudyViewer.SeriesViewer_1X2());
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x2);
                StudyViewer.SelectToolInToolBar(IEnum.ViewerTools.PrintView);
                var printwindow35 = StudyViewer.SwitchtoNewWindow(windowtitle);
                IWebElement landscapechk1 = BasePage.Driver.FindElement(By.CssSelector("input[id$='_PaperOrientationRadioButtonList_1']"));
                IWebElement papersize7 = BasePage.Driver.FindElement(By.CssSelector("div.borderClass select[id$='_PaperSizeDropDownList']>option[selected]"));
                string pprsizeOption7 = papersize7.Text;
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[id='SeriesViewersDiv']")));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_36 = studies.CompareImage(result.steps[ExecutedSteps], StudyViewer.Printimage());
                if (step_36 && pprsizeOption7.Equals("A4") && landscapechk1.Selected == true)
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


                //step 37: Print it to a file and verify the printout.
                BasePage.Driver.Close();
                BasePage.Driver.SwitchTo().Window(printwindow35);
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].status = "Not Automated";

                StudyViewer.CloseStudy();
                login.Logout();
                studies.CloseBrowser();

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
        /// User Preference Settings
        /// </summary>
        public TestCaseResult Test_28008(String testid, String teststeps, int stepcount)
        {
            //Fetch the data
            String Contactinfo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ContactInformation");
            String[] info = Contactinfo.Split('=');

            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String Username = "User1" + new Random().Next(1, 10000);
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');

                //Step 1 - Re-login as either Administrator or user created
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //Step 2 - Load any study.
                studies = (Studies)login.Navigate("Studies");

                //--Pre condition-- disable connection tool.
                PageLoadWait.WaitForPageLoad(20);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                if (BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Selected == false)
                {
                    BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Click();
                    Logger.Instance.InfoLog("Connection tool CB un-selected successfully");
                }
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);

                // Accession[0] =>11665475
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);
                studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(10);
                ExecutedSteps++;

                //Step 3 - Hover the cursor over "Lossy Compressed" in the top panel beside the Layout tool                
                if (StudyViewer.LossyCompressedLable("studyview").GetAttribute("title").Equals("JPEG lossy compressed, Quality = 80"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 4 - Click on the Printable View link.
                StudyViewer.SelectToolInToolBar("PrintView");
                PageLoadWait.WaitForFrameLoad(20);
                var PrintWindow = BasePage.Driver.WindowHandles.Last();
                var StudyWindow = BasePage.Driver.CurrentWindowHandle;
                BasePage.Driver.SwitchTo().Window(PrintWindow);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#viewerImg_1_1")));
                //ICA
                //if (StudyViewer.LossyCompressedLable("printview").Text.Equals("Lossy (80)"))                
                if (StudyViewer.LossyCompressedLable("printview").Text.Equals("Lossy Compressed"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                BasePage.Driver.SwitchTo().Window(PrintWindow).Close();
                BasePage.Driver.SwitchTo().Window(StudyWindow);
                PageLoadWait.WaitForFrameLoad(10);

                //Step 5 - Print the image.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 6 - Open the User Preferences dialog from the Options menu.
                StudyViewer.SelectToolInToolBar("UserPreference");
                PageLoadWait.WaitForFrameLoad(10);
                ExecutedSteps++;

                //Step 7 - Select PNG (lossless) and click OK
                UserPref.SwitchToToolBarUserPrefFrame();
                UserPref.PNGRadioBtn().Click();
                UserPref.ClickSaveToolBarUserPreferences();
                ExecutedSteps++;

                //Step 8 - Click close in the confirmation dialog.
                if (UserPref.ResultLable().Text.Equals("Preferences have been successfully updated."))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                UserPref.CloseBtn().Click();
                PageLoadWait.WaitForFrameLoad(10);

                //Step 9 - Close Study
                studies.CloseStudy();
                ExecutedSteps++;

                //Step 10 - Load another study
                studies = (Studies)login.Navigate("Studies");
                // Accession[1] => MS10025
                studies.SearchStudy(AccessionNo: Accession[1], Datasource: EA_131);
                studies.SelectStudy("Accession", Accession[1]);
                studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(10);
                if (!StudyViewer.LossyCompressedLable("studyview").Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 11 - Click on the Printable View link.
                StudyViewer.SelectToolInToolBar("PrintView");
                var PrintWindow1 = BasePage.Driver.WindowHandles.Last();
                var StudyWindow1 = BasePage.Driver.CurrentWindowHandle;
                BasePage.Driver.SwitchTo().Window(PrintWindow1);
                PageLoadWait.WaitForFrameLoad(60);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#viewerImg_1_1")));
                if (!StudyViewer.LossyCompressedLable("printview").Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                BasePage.Driver.SwitchTo().Window(PrintWindow1).Close();
                BasePage.Driver.SwitchTo().Window(StudyWindow1);
                PageLoadWait.WaitForFrameLoad(10);

                //Step 12 - Open the User Preferences dialog from the Options menu.
                StudyViewer.SelectToolInToolBar("UserPreference");
                PageLoadWait.WaitForFrameLoad(10);
                ExecutedSteps++;

                //Step 13 - Change the Image Format back to JPEG and click OK.
                UserPref.SwitchToToolBarUserPrefFrame();
                UserPref.JPEGRadioBtn().Click();
                UserPref.SaveToolBarUserPreferences();
                ExecutedSteps++;

                //Step 14 - Close Study
                studies.CloseStudy();
                ExecutedSteps++;

                //Step 15 - Load another study
                studies = (Studies)login.Navigate("Studies");
                // Accession[0] => 11665475
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);
                studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(10);
                ExecutedSteps++;

                //Step 16 - Verify "Lossy Compressed"in the top panel beside the Layout tool
                if (StudyViewer.LossyCompressedLable("studyview").GetAttribute("title").Equals("JPEG lossy compressed, Quality = 80"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 17 - Close Study
                studies.CloseStudy();
                ExecutedSteps++;

                //Step 18 - Logout from ICA
                login.Logout();
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
    }
}

