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
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.Putty;
using System.Globalization;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MergeServiceTool;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Remote;
using TestStack.White.UIItems.WindowItems;
using TestStack.White;
using CheckBox = TestStack.White.UIItems.CheckBox;
using System.ServiceProcess;
using System.Diagnostics;
using Window = TestStack.White.UIItems.WindowItems.Window;
using Panel = TestStack.White.UIItems.Panel;
using TestStack.White.Configuration;
using GroupBox = TestStack.White.UIItems.GroupBox;
using TextBox = TestStack.White.UIItems.TextBox;
using Tab = TestStack.White.UIItems.TabItems.Tab;
using ITabPage = TestStack.White.UIItems.TabItems.ITabPage;

namespace Selenium.Scripts.Tests
{
    class Browser
    {
        public Login login { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public HPLogin hplogin { get; set; }
        public Configure configure { get; set; }
        public HPHomePage hphomepage { get; set; }
        public ExamImporter ei { get; set; }
        public string filepath { get; set; }
        public DomainManagement domain { get; set; }
        public StudyViewer viewer { get; set; }
        String TestUser = "User_" + new Random().Next(1, 10000);
        public WpfObjects wpfobject;
        public Browser(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            domain = new DomainManagement();
            viewer = new StudyViewer();
            ei = new ExamImporter();
            BasePage.InitializeControlIdMap();
            wpfobject = new WpfObjects();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }

        /// <summary>
        /// Studylist
        /// </summary>
        public TestCaseResult Test_27873(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables          
            ServiceTool servicetool = new ServiceTool();
            WpfObjects wpfobject = new WpfObjects();
            Studies studies = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            UserManagement usermanagement;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;
                String RoleName = Config.adminRoleName;
                String username1 = Config.ar1UserName;
                String password1 = Config.ar1Password;
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                String pacusername = Config.pacsadmin;
                String pacpassword = Config.pacspassword;

                //Preconditions: Service tool updates
                servicetool.LaunchServiceTool();

                //Enable study sharing, Connection test tool and study attachement
                servicetool.SetEnableFeaturesGeneral();
                servicetool.ClickModifyFromTab();
                wpfobject.SelectCheckBox(6);
                wpfobject.SelectCheckBox(10);
                servicetool.ApplyEnableFeatures();
                servicetool.wpfobject.ClickOkPopUp();
                servicetool.EnableStudyAttachements();
                servicetool.RestartService();
                servicetool.WaitWhileBusy();
                servicetool.CloseConfigTool();

                //Step-1

                //precondition - create test user
                login.LoginIConnect(username, password);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.SelectDomainFromDropdownList(DomainName);
                usermanagement.CreateUser(TestUser, DomainName, RoleName);

                //Update toolbar
                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain(DomainName);
                domain.SelectDomain(DomainName);
                domain.ClickEditDomain();
                PageLoadWait.WaitForElement(By.CssSelector(Locators.CssSelector.EnableAttachmentDomainMgmt), BasePage.WaitTypes.Visible);
                domain.SetCheckbox("cssselector", Locators.CssSelector.EnableAttachmentDomainMgmt);
                PageLoadWait.WaitForElement(By.CssSelector(Locators.CssSelector.AllowUploadAttachmentDomainMgmt), BasePage.WaitTypes.Visible);
                domain.SetCheckbox("cssselector", Locators.CssSelector.AllowUploadAttachmentDomainMgmt);

                //Add tools to domain
                domain.AddToolsToToolbarByName(new string[] { "Next Series", "Previous Series", "Series Scope", "Image Scope", "Image Layout 1x1", "Image Layout 1x2", "Image Layout 2x1", "Image Layout 2x2", "Image Layout 3x3", "Image Layout 4x4" }, 1, 5);
                domain.ClickSaveDomain();
                login.Logout();

                //Actual step
                login.DriverGoTo(login.url);
                ExecutedSteps++;

                //Step 2
                login.LoginIConnect(TestUser, TestUser);
                studies = (Studies)login.Navigate("Studies");
                studies.ClearFields();
                studies.SearchStudy("last", "*");
                PageLoadWait.WaitForLoadingMessage(30);
                //studies.ClickButton("input#m_studySearchControl_m_searchButton");
                PageLoadWait.WaitForLoadingMessage(30);
                bool isscrollbar = studies.IsVerticalScrollBarPresent(BasePage.Driver.FindElement(By.CssSelector("#gview_gridTableStudyList > div.ui-jqgrid-bdiv")));
                if ((BasePage.GetSearchResults().Count >= 50) && (isscrollbar))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 3
                //Hover Options -- Flakikness in this steps - Given workaround SetCursor()
                studies.SwitchToDefault();
                studies.SwitchTo("index", "0");
                BasePage.SetCursorPos(0, 0);
                studies.JSMouseHover(BasePage.Driver.FindElement(By.CssSelector("img[src ^= 'Images/options']")));
                new Actions(BasePage.Driver).MoveToElement(BasePage.Driver.FindElement(By.CssSelector("img[src^='Images/options']"))).Build().Perform();
                new Actions(BasePage.Driver).MoveToElement(BasePage.Driver.FindElement(By.CssSelector("img[src^='Images/options']"))).Build().Perform();
                Thread.Sleep(1000);
                bool test1 = studies.VerifyAnchorText("id", Locators.ID.UserPreferenceOptionstable1, "User Preferences");
                bool test2 = studies.VerifyAnchorText("id", Locators.ID.UserPreferenceOptionstable2, "My Profile");
                if (test1 && test2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 4
                studies.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                bool checkdiv = studies.VerifyElementPresence("cssselector", Locators.ID.UserPreferenceDiv);
                if (checkdiv)
                {
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
                studies.CloseUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                string checkupf = studies.GetElementAttribute("cssselector", Locators.ID.UserPreferenceDiv, "style");
                if (checkupf.Contains("display: none"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 6 -- Falkiness in the hover method. Given workaround SetCursor()
                studies.SwitchToDefault();
                studies.SwitchTo("index", "0");
                BasePage.SetCursorPos(0, 0);
                studies.JSMouseHover(BasePage.Driver.FindElement(By.CssSelector(Locators.CssSelector.HelpSelector)));
                new Actions(BasePage.Driver).MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Locators.CssSelector.HelpSelector))).Build().Perform();
                new Actions(BasePage.Driver).MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Locators.CssSelector.HelpSelector))).Build().Perform();
                bool contents = studies.VerifyAnchorText("id", Locators.ID.HelpOptionstable1, "Contents");
                bool about = studies.VerifyAnchorText("id", Locators.ID.HelpOptionstable2, "About iConnect® Access");
                if (contents && about)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 7              
                BasePage.Driver.FindElement(By.CssSelector(Locators.CssSelector.HelpSelector)).Click();
                BasePage.Driver.FindElement(By.CssSelector("table[id='help_menu_1']")).Click();
                PageLoadWait.WaitForElement(By.Id("DialogDiv"), BasePage.WaitTypes.Visible);
                PageLoadWait.WaitForElement(By.CssSelector(Locators.CssSelector.AboutICAMergeLogo), BasePage.WaitTypes.Visible);
                IWebElement element = BasePage.Driver.FindElement(By.CssSelector(Locators.CssSelector.AboutICAMergeLogo));
                var abouticonnect = BasePage.Driver.FindElement(By.Id("DialogDiv"));
                bool step7_1 = abouticonnect.Displayed;

                //Check if the page is grayed out
                bool step7_2 = false, step7_3 = false;
                element = BasePage.Driver.FindElement(By.Id("DimmerDiv"));
                string step7_attributes = element.GetAttribute("style");
                string[] step7_attr = step7_attributes.Split(';');
                foreach (string item in step7_attr)
                {
                    if (item.Contains("opacity"))
                    {
                        if (item.Contains("0.5"))
                        {
                            step7_2 = true;
                        }
                    }
                    if (item.Contains("background-color"))
                    {
                        if (item.Contains("black"))
                        {
                            step7_3 = true;
                        }
                    }
                }
                if (step7_1 && step7_2 && step7_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 8
                studies.Click("id", "ctl00_CloseHelpAboutButton");
                element = PageLoadWait.WaitForElement(By.Id("DialogDiv"), BasePage.WaitTypes.Invisible);
                if (!element.Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 9
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                studies.HoverElement(By.Id("searchStudyDropDownMenu"));
                string Study_Performed = "All Dates|Last Hour|Last 2 Hours|Last 5 Hours|After Midnight|Last 24 Hours|Last 2 Days|Last 7 Days|Last 14 Days|Last Month|Last 2 Months|Last 6 Months|Last 12 Months|Last 18 Months|Last 2 Years|Custom Date Range";
                string[] study_performed = Study_Performed.Split('|');
                element = PageLoadWait.WaitForElement(By.Id("mb_searchStudySubMenu"), BasePage.WaitTypes.Visible);
                List<IWebElement> submenu = element.FindElements(By.TagName("a")).ToList();
                if (studies.ValidateStringArrayInWebElementList(study_performed, submenu))
                {
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
                var menuPer = BasePage.Driver.FindElement(By.Id("searchStudyDropDownMenu"));
                new Actions(BasePage.Driver).MoveToElement(menuPer).Click().Build().Perform();
                BasePage.Driver.FindElement(By.LinkText("Custom Date Range")).Click();
                PageLoadWait.WaitForElement(By.Id("masterDateFrom"), BasePage.WaitTypes.Visible);
                bool step10_1 = studies.VerifyElementPresence("id", "masterDateFrom");
                bool step10_2 = studies.VerifyElementPresence("id", "masterDateTo");
                if (step10_1 && step10_2)
                {
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
                new Actions(BasePage.Driver).Click(BasePage.Driver.FindElement(By.Id("masterDateFrom"))).Build().Perform();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.FromCalendarPrevButton)));
                bool step11_1 = studies.VerifyElementPresence("cssselector", Locators.CssSelector.FromCalendarPrevButton);
                bool step11_2 = studies.VerifyElementPresence("cssselector", Locators.CssSelector.FromCalendarNextButton);
                bool step11_3 = studies.VerifyElementPresence("cssselector", Locators.CssSelector.FromCalendarMonthSelect);
                bool step11_4 = studies.VerifyElementPresence("cssselector", Locators.CssSelector.FromCalendarYearSelect);
                String currdate = BasePage.Driver.FindElement(By.CssSelector("table td[class$='curdate']")).GetAttribute("innerHTML");
                if (currdate.Length == 1) { currdate = "0" + currdate; }
                bool step11_5 = DateTime.Today.Date.ToString("dd").Equals(currdate);

                if (step11_1 && step11_2 && step11_3 && step11_4 && step11_5)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 12
                Actions action = new Actions(BasePage.Driver);
                action.Click().Build().Perform();
                action.MoveToElement(BasePage.Driver.FindElement(By.CssSelector("#StudyListDialogDiv.pythonBlueBackground"))).MoveByOffset(1000, 1000).Click().Build().Perform();
                PageLoadWait.WaitForElement(By.Id("DateRangeSelectorCalendarFrom_calendar"), BasePage.WaitTypes.Invisible);
                if (!studies.VerifyElementPresence("id", "DateRangeSelectorCalendarFrom_calendar"))
                {
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
                studies.Click("id", "m_studySearchControl_m_studyListDateRangeSelector_CancelCalenderButton");
                PageLoadWait.WaitForElement(By.Id("StudyListDialogDiv"), BasePage.WaitTypes.Invisible);
                if (!studies.VerifyElementPresence("id", "StudyListDialogDiv"))
                {
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
                studies.HoverElement(By.Id("dataSource_right"));
                PageLoadWait.WaitForElement(By.Id("mb_sub_menu_multiselect"), BasePage.WaitTypes.Visible);
                element = BasePage.Driver.FindElement(By.Id("mb_sub_menu_multiselect"));
                List<IWebElement> elementlist = element.FindElements(By.TagName("table")).ToList();
                int DSCount = elementlist.Capacity;

                //Check if all is present
                element = BasePage.Driver.FindElement(By.CssSelector("#sub_menu_multiselect_0>tbody>tr>td>a>span"));
                if (element.Text.ToLower().Equals("all") && DSCount >= 2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                BasePage.SetCursorPos(0, 0);
                PageLoadWait.WaitForElement(By.CssSelector("#m_studySearchControl_m_searchInputPatientLastName"), BasePage.WaitTypes.Visible, 10).Click();
                studies.SearchStudy(LastName: "*", Datasource: "All");
                PageLoadWait.WaitForFrameLoad(30);


                //Step 15
                studies.Click("cssselector", "div#showHideCriteriaButton>#ExpandSearchPanelButton");
                bool step15_1 = studies.VerifyElementPresence("cssselector", "div#SearchPanelDiv");
                element = studies.GetElement("cssselector", "div#showHideCriteriaButton>#ExpandSearchPanelButton");
                string step15_2 = element.GetAttribute("title");

                //Search panel should not be visible & title should read Expand
                if (!step15_1 && step15_2.Equals("Show Search Criteria"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 16
                studies.Click("cssselector", "div#showHideCriteriaButton>#ExpandSearchPanelButton");
                bool step16_1 = studies.VerifyElementPresence("cssselector", "div#SearchPanelDiv");
                element = studies.GetElement("cssselector", "div#showHideCriteriaButton>#ExpandSearchPanelButton");
                string step16_2 = element.GetAttribute("title");

                //Search panel should be visible & title should read Hide
                if (step16_1 && step16_2.Equals("Hide Search Criteria"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 17
                studies.SearchStudy(LastName: "*", Datasource: "All");
                PageLoadWait.WaitForLoadingMessage(45);

                //Study List Div before Minimizing
                int divwidth17 = BasePage.Driver.FindElement(By.CssSelector("#gview_gridTableStudyList > div.ui-jqgrid-bdiv")).Size.Width;
                int divheight17 = BasePage.Driver.FindElement(By.CssSelector("#gview_gridTableStudyList > div.ui-jqgrid-bdiv")).Size.Height;

                //Browser width before Minimizing                
                int width17 = BasePage.Driver.Manage().Window.Size.Width;
                BasePage.Driver.Manage().Window.Size = new Size(1000, 800);

                //Browser width after minimzing                
                int width17_after = BasePage.Driver.Manage().Window.Size.Width;

                //Study Div width and height after minimzing        
                int divwidth17_after = BasePage.Driver.FindElement(By.CssSelector("#gview_gridTableStudyList > div.ui-jqgrid-bdiv")).Size.Width;
                int divheight17_after = BasePage.Driver.FindElement(By.CssSelector("#gview_gridTableStudyList > div.ui-jqgrid-bdiv")).Size.Height;

                //Check that browser size is reduced, Study Search Div size is reduced and Vertical scroll bar is present
                bool step17_1 = (width17 > width17_after) ? true : false;
                bool step17_2 = studies.IsVerticalScrollBarPresent(BasePage.Driver.FindElement(By.CssSelector("#gview_gridTableStudyList > div.ui-jqgrid-bdiv")));
                bool step17_3 = (divwidth17 > divwidth17_after && divheight17 > divheight17_after) ? true : false;

                if (step17_1 && step17_2 && step17_3)
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

                //Step 18
                BasePage.Driver.Manage().Window.Maximize();
                PageLoadWait.WaitForFrameLoad(10);
                int divwidth18 = BasePage.Driver.FindElement(By.CssSelector("#gview_gridTableStudyList > div.ui-jqgrid-bdiv")).Size.Width;
                int divheight18 = BasePage.Driver.FindElement(By.CssSelector("#gview_gridTableStudyList > div.ui-jqgrid-bdiv")).Size.Height;
                int width18 = BasePage.Driver.Manage().Window.Size.Width;

                bool step18_1 = (width18 > width17_after) ? true : false;
                bool step18_2 = studies.IsVerticalScrollBarPresent(BasePage.Driver.FindElement(By.CssSelector("#gview_gridTableStudyList > div.ui-jqgrid-bdiv")));
                bool step18_3 = (divwidth18 > divwidth17_after && divheight18 > divheight17_after) ? true : false;

                if (step18_1 && step18_2 && step18_3)
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

                //Step 19
                string[] colnames = new string[] { "Modality", "Patient Name", "Patient ID", "Accession" };
                studies.SearchStudy(Modality: "MR");
                bool[] step19 = new bool[colnames.Length];
                for (int i = 0; i < colnames.Length; i++)
                {
                    studies.ClickColumnHeading(colnames[i]);
                    Thread.Sleep(700);
                    string[] step19_1 = studies.GetStudyDetails(colnames[i]);
                    step19[i] = (step19_1 == null || step19_1.Length == 0) ? false : step19_1.Select(s => s.ToLower()).SequenceEqual((step19_1.OrderBy(q => q)).Select(s => s.ToLower()));
                    //string[] step19_22step = step19_1.OrderBy(q => q).ToArray<string>();
                    //for (int ii = 0; ii < 105; ++ii)
                    //{
                    //    if (step19_1[ii] != step19_22step[ii])
                    //        Console.WriteLine(ii);
                    //}
                }

                bool step19_2 = studies.ValidateBoolArray(step19);
                if (step19_2)
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

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }


        }

        /// <summary>
        /// Viewer
        /// </summary>
        public TestCaseResult Test_27874(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables       
            Studies studies = null;
            studies = new Studies();
            Patients patients = null;
            patients = new Patients();
            Viewer viewer = null;
            viewer = new Viewer();
            Taskbar taskbar = new Taskbar();
            StudyViewer StudyVw;
            DomainManagement domainmanagement;
            RoleManagement rolemanagement = null;
            UserManagement usermanagement;
            ConferenceFolders conferencefolders;
            TestCaseResult result;
            Inbounds inbounds = null;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;
                String RoleName = Config.adminRoleName;
                String Name = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                String Description = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Description");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String EmailID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "EmailID");
                String Reason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Reason");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String DatasourceName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DatasourceName");
                String[] lastName = LastName.Split(':');
                String[] UploadFilePaths = UploadFilePath.Split(':');
                String[] patientID = PatientID.Split(':');
                String[] DSList = DatasourceName.Split(':');
                String phUsername = Config.phUserName;
                String phpassword = Config.phPassword;
                String arusername = Config.arUserName;
                String arpassword = Config.arPassword;
                String acclist = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = acclist.Split(':');
                String FileNames = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FileNames");
                String[] FileList = FileNames.Split(':');
                String[] datasources = null;
                string FolderPath = Config.downloadpath;

                //DomainB
                String TestdomainB = "27874_DomainB" + new Random().Next(1, 1000);
                String TestdomainRoleB = "27874_DomainRoleB" + new Random().Next(1, 1000);
                String TestroleB1 = "27874_Role1B" + new Random().Next(1, 1000);
                String TestuserB1 = "27874_User1B" + new Random().Next(1, 1000);

                //FolderNames for DomainB
                //Set1:
                String TopFolderB1 = "Monthly Oncology Tumor Boards_" + new Random().Next(1, 1000);
                String SubFolderB1_Level2_1 = "Hybrid, XDS only and DICOM only_" + new Random().Next(1, 1000);
                String StudyNotes_Level1_Subfolder = "Conference notes for Sub folder Hybrid xds studies_" + new Random().Next(1, 1000);

                //Step 1
                //Enable PDF report in service tool
                taskbar.Hide();
                ServiceTool servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.SetEnableFeaturesGeneral();
                servicetool.wpfobject.ClickButton(ServiceTool.ModifyBtn_Name, 1);
                servicetool.EnablePDFReport();
                wpfobject.WaitTillLoad();
                wpfobject.WaitTillLoad();
                servicetool.EnableConferenceLists();
                wpfobject.WaitTillLoad();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.wpfobject.ClickOkPopUp();
                servicetool.EnableStudyAttachements();
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                taskbar.Show();

                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Navigate to Domain Management
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DomainName);
                domainmanagement.SelectDomain(DomainName);
                domainmanagement.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                domainmanagement.SetCheckBoxInEditDomain("reportview", 0);
                domainmanagement.SetCheckBoxInEditDomain("attachment", 0);
                domainmanagement.SetCheckBoxInEditDomain("attachmentupload", 0);
                //select PDFReport flag
                domainmanagement.SetCheckBoxInEditDomain("pdfreport", 0);
                domainmanagement.SetCheckBoxInEditDomain("datatransfer", 0);
                domainmanagement.SetCheckBoxInEditDomain("datadownload", 0);
                //Enable conference list and grant access check box
                domainmanagement.SetCheckBoxInEditDomain("conferencelists", 0);
                domainmanagement.SetCheckBoxInEditDomain("grant", 0);
                domainmanagement.ClickSaveDomain();

                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectDomainfromDropDown(DomainName);
                rolemanagement.SelectRole("SuperRole");
                rolemanagement.ClickEditRole();
                PageLoadWait.WaitForFrameLoad(10);
                rolemanagement.GrantAccessRadioBtn_Anyone().Click();
                rolemanagement.SetCheckboxInEditRole("pdfreport", 0);
                rolemanagement.SetCheckboxInEditRole("conferenceuser", 0);
                rolemanagement.SetCheckboxInEditRole("transfer", 0);
                rolemanagement.SetCheckboxInEditRole("download", 0);
                rolemanagement.ClickSaveRole();

                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("last", lastName[3]);
                studies.ChooseColumns(new String[] { "Description" });
                studies.SelectStudy("Description", Description);

                studies.LaunchStudy();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                var viewport = BasePage.Driver.FindElement(By.Id("studyPanelDiv_1"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27874 = studies.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step27874)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-2
                //Select the Report viewer button
                viewer.ReportView();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_studyPanels_m_studyPanel_1_m_reportViewer_reportFrame");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("ViewerDisplay")));

                if (BasePage.Driver.FindElement(By.Id("ViewerDisplay")).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-3
                viewer.ReportView();
                viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_studyViewerContainer"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27830_3 = patients.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step27830_3)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4
                string[] WindowHandles = viewer.OpenPrintViewandSwitchtoIT();
                viewport = BasePage.Driver.FindElement(By.Id("SeriesViewersDiv"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27830_17 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step27830_17)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-6
                viewer.ClosePrintView(WindowHandles[1], WindowHandles[0]);
                ExecutedSteps++;

                //Step-7
                studies.SwitchTo("index", "0");
                studies.NavigateToHistoryPanel();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                studies.Click("id", "m_patientHistory_AttachmentButton");
                if (BasePage.Driver.FindElement(By.Id("gview_m_patientHistory_m_attachmentViewer_attachmentList")).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-8
                //for strangeways
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                studies.Click("id", "m_patientHistory_ReportButton");
                if (BasePage.Driver.FindElement(By.Id("m_patientHistory_reportViewerContainer")).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-9
                bool value = viewer.CheckData(Name);
                if (value)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10
                //Loading the first related study
                studies.OpenPriors(new String[] { "Study Description" }, new String[] { Name });
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_2_studyViewerContainer"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27874_10 = patients.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step27874_10)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-11
                patients.Click("id", "m_studyPanels_m_studyPanel_1_studyViewerContainer");
                patients.ClickElement("Series Viewer 2x2");
                PageLoadWait.WaitForElement(By.Id("m_studyPanels_m_studyPanel_1_studyViewerContainer"), BasePage.WaitTypes.Visible);
                viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_studyViewerContainer"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27830_11 = patients.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step27830_11)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-12
                StudyViewer studyview = new StudyViewer();
                studyview.DragThumbnailToViewport(2, studies.GetControlId("SeriesViewer3-2X2"));
                studyview.DragThumbnailToViewport(3, studies.GetControlId("SeriesViewer4-2X2"));
                viewport = BasePage.Driver.FindElement(By.Id("studyPanelDiv_1"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27874_12 = patients.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step27874_12)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-13
                IWebElement element1 = studies.GetElement("id", login.GetControlId("SeriesViewer1-2X2"));
                studies.Click("id", login.GetControlId("SeriesViewer1-2X2"));
                studies.ClickElement("Auto Window Level");
                viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27874_13_wl = patients.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step27874_13_wl)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-14
                IWebElement element2 = studies.GetElement("id", login.GetControlId("SeriesViewer2-2X2"));
                studies.Click("id", login.GetControlId("SeriesViewer2-2X2"));
                studies.ClickElement("Rotate Clockwise");
                viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_viewerImg"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27874_13_rotate = patients.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step27874_13_rotate)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-15
                studies.Click("id", login.GetControlId("SeriesViewer1-2X2"));
                PageLoadWait.WaitForFrameLoad(5);
                studies.ClickElement("Series Viewer 1x1");
                PageLoadWait.WaitForFrameLoad(5);
                viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27874_14 = patients.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step27874_14)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-16
                studies.SwitchTo("index", "0");
                studies.ClickPatientHistoryTab();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                studies.OpenPriors(new String[] { "Study Description" }, new String[] { Description });
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                viewport = BasePage.Driver.FindElement(By.Id("StudyPanelContainer"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27874_15 = patients.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step27874_15)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-17
                studies.ClickElement("Full Screen");
                PageLoadWait.WaitForFrameLoad(5);
                viewport = BasePage.Driver.FindElement(By.Id("StudyPanelContainer"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27874_16 = patients.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step27874_16)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-18
                BasePage.Driver.FindElement(By.CssSelector("img#m_studyPanels_m_studyPanel_2_ctl03_SeriesViewer_2_1_viewerImg")).Click();
                BasePage.Driver.FindElement(By.CssSelector("img#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg")).Click();
                Thread.Sleep(1000);
                studies.Click("id", "recallMenus");
                PageLoadWait.WaitForFrameLoad(5);
                BasePage.wait.Until<Boolean>(d =>
                {
                    if (!d.FindElement(By.CssSelector("div[id='m_studyPanels_m_studyPanel_1_thumbnailControl']")).GetAttribute("style").Contains("display: none;"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                viewport = BasePage.Driver.FindElement(By.Id("ViewerContainer"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27874_17 = patients.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step27874_17)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-19
                studies.ClickElement("Full Screen");
                PageLoadWait.WaitForFrameLoad(5);
                viewport = BasePage.Driver.FindElement(By.Id("studyPanelDiv_1"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27874_18 = patients.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step27874_18)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-20
                ReadOnlyCollection<IWebElement> elements = BasePage.Driver.FindElements(By.TagName("li"));
                bool flag20 = true;
                foreach (IWebElement t in elements)
                {

                    if (t.GetAttribute("title").Contains("Series Viewer"))
                    {
                        studies.JSMouseHover(t);
                        Thread.Sleep(1000);
                        IList<IWebElement> dropdown20 = t.FindElements(By.CssSelector("ul>li"));
                        IList<String> titles = new List<String>();
                        foreach (IWebElement dropdowntool in dropdown20)
                        {
                            titles.Add(dropdowntool.GetAttribute("title"));
                            if (!(dropdowntool.Enabled && dropdowntool.Displayed))
                            {
                                flag20 = false;
                                break;
                            }
                        }
                        break;
                    }
                }
                if (flag20)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();

                }

                //Step-21
                studies.ClickElement("Series Viewer 1x2");
                viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_compositeViewerContainer"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27874_20 = patients.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step27874_20)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-22
                studies.Click("id", login.GetControlId("SeriesViewer1-1X1"));
                studies.ClickElement("Window Level");
                viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg"));
                string step22 = studies.GetElementCursorType(viewport);
                if (step22.Contains("move"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-23
                int h = 0;
                int w = 0;
                Actions action = new Actions(BasePage.Driver);
                var element = studies.GetElement("id", login.GetControlId("SeriesViewer1-1X2"));
                studies.Click("id", login.GetControlId("SeriesViewer1-1X2"));
                if (element != null)
                {
                    h = element.Size.Height;
                    w = element.Size.Width;

                    action.ClickAndHold(element).MoveToElement(element, w / 2, h / 4).Build().Perform();
                    Thread.Sleep(2000);
                    action.Release().Build().Perform();

                }
                Thread.Sleep(1000);
                viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_SeriesViewerPanel"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27874_22 = patients.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step27874_22)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //step-24
                studies.ClickElement("Zoom");
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg"));
                string step24 = studies.GetElementCursorType(viewport);
                if (step24.Contains("n-resize"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-25
                h = 0;
                w = 0;
                action = new Actions(BasePage.Driver);
                element = studies.GetElement("id", login.GetControlId("SeriesViewer1-1X2"));
                studies.Click("id", login.GetControlId("SeriesViewer1-1X2"));
                if (element != null)
                {
                    int i = 0;
                    w = element.Size.Width;
                    h = element.Size.Height;
                    while (i < 3)
                    {
                        action.ClickAndHold(element).MoveToElement(element, w / 2, h / 3).Build().Perform();
                        Thread.Sleep(2000);
                        action.Release().Build().Perform();
                        i++;
                    }
                }
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_SeriesViewerPanel"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27874_24 = patients.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step27874_24)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //step-26
                studies.ClickElement("Pan");
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg"));
                string step26 = studies.GetElementCursorType(viewport);
                if (step26.Contains("pointer"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-27
                h = 0;
                w = 0;
                action = new Actions(BasePage.Driver);
                element = studies.GetElement("id", login.GetControlId("SeriesViewer1-1X2"));
                studies.Click("id", login.GetControlId("SeriesViewer1-1X2"));
                if (element != null)
                {
                    w = element.Size.Width;
                    h = element.Size.Height;
                    int j = 0;
                    while (j < 1)
                    {
                        action.MoveToElement(element, w / 2, h / 2).ClickAndHold().MoveToElement(element, w / 2, h / 3).Build().Perform();
                        Thread.Sleep(2000);
                        action.Release().Build().Perform();
                        j++;
                    }
                }
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_SeriesViewerPanel"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27874_26 = patients.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step27874_26)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-28
                studies.Click("id", login.GetControlId("SeriesViewer1-1X2"));
                studies.ClickElement("Rotate Clockwise");
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_SeriesViewerPanel"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27874_27 = patients.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step27874_27)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-29
                studies.Click("id", login.GetControlId("SeriesViewer1-1X2"));
                studies.ClickElement("Rotate Counterclockwise");
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_SeriesViewerPanel"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27874_28 = patients.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step27874_28)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-30
                elements = BasePage.Driver.FindElements(By.TagName("li"));
                bool flag30 = true;
                bool step30 = false;
                foreach (IWebElement t in elements)
                {

                    if (t.GetAttribute("title").Contains("Flip") || t.GetAttribute("title").Contains("Rotate"))
                    {
                        studies.JSMouseHover(t);
                        Thread.Sleep(1000);
                        IList<IWebElement> dropdown30 = t.FindElements(By.CssSelector("ul>li"));
                        IList<String> titles = new List<String>();
                        foreach (IWebElement dropdowntool in dropdown30)
                        {
                            titles.Add(dropdowntool.GetAttribute("title"));
                            if (!(dropdowntool.Enabled && dropdowntool.Displayed))
                            {
                                flag30 = false;
                                break;
                            }
                        }
                        foreach (string item in titles)
                        {
                            if (item.Equals("Flip Horizontal"))
                            {
                                step30 = true;
                            }
                            else if (item.Equals("Flip Vertical"))
                            {
                                step30 = true;
                            }
                        }
                        break;
                    }

                }
                if (flag30 && step30)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-31
                studies.Click("id", login.GetControlId("SeriesViewer1-1X2"));
                studies.ClickElement("Flip Horizontal");
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_SeriesViewerPanel"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27874_30 = patients.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step27874_30)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-32
                studies.Click("id", login.GetControlId("SeriesViewer1-1X2"));
                studies.ClickElement("Flip Vertical");
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_SeriesViewerPanel"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27874_31 = patients.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step27874_31)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-33
                studies.Click("id", login.GetControlId("SeriesViewer1-1X2"));
                studies.ClickElement("Reset");
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_SeriesViewerPanel"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27874_32 = patients.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step27874_32)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                studies.CloseStudy();

                //Step 34: Cine Playback - Not Automated
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 35: Load study with multiple images and apply global stack, scroll images and deselect global stack
                //Data - Therapy, Head
                studies.ClearFields();
                studies.SearchStudy(LastName: lastName[4], Datasource: DSList[0]);
                studies.ChooseColumns(new String[] { "Patient ID" });
                studies.SelectStudy("Patient ID", patientID[0]);
                StudyVw = studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                studies.Click("id", login.GetControlId("SeriesViewer1-1X2"));
                IWebElement viewportload = studies.GetElement("id", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_SeriesViewer_ImagesPanel");
                //Select Global stack
                studies.ClickElement("Global Stack");
                PageLoadWait.WaitForLoadInViewport(15, viewportload);
                //Scrolling Images
                viewer.ClickScrollDown("id", Locators.ID.ScrollNext1_1X1, 6);
                PageLoadWait.WaitForLoadInViewport(15, viewportload);
                PageLoadWait.WaitForFrameLoad(15);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_2X2));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step35_1 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                //Deslecting Global stack
                studies.ClickElement("Global Stack");
                PageLoadWait.WaitForLoadInViewport(15, viewportload);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_2X2));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step35_2 = studies.CompareImage(result.steps[ExecutedSteps], viewport, 2, 1);
                if (step35_1 && step35_2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //studies.CloseStudy();

                //Step 36: Linked scrolling
                //Data - Therapy, Head
                //Scroll Up
                for (int i = 0; i < 6; i++)
                {
                    StudyVw.ClickUpArrowbutton(1, 1);
                }
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.LinkSelected);
                PageLoadWait.WaitForPageLoad(5);
                PageLoadWait.WaitForFrameLoad(5);
                //Select square box then green check mark.
                StudyVw.SelectLinkedCheckBox(1, 1);
                StudyVw.SelectLinkedCheckBox(1, 2);
                StudyVw.SelectLinkedCheckBox(2, 2);
                StudyVw.LinkedScrollingCheckBtn().Click();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //Scroll Down
                for (int i = 0; i < 3; i++)
                {
                    StudyVw.ClickDownArrowbutton(1, 1);
                }
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step36_1 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());

                //Scroll Up
                for (int i = 0; i < 3; i++)
                {
                    StudyVw.ClickUpArrowbutton(1, 1);
                }
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step36_2 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel(), 2);

                //Unlink All
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.Unlink);
                PageLoadWait.WaitForPageLoad(5);
                PageLoadWait.WaitForFrameLoad(5);

                //Link Offset
                //Scroll 4th viewport before doing offset
                for (int i = 0; i < 3; i++)
                {
                    StudyVw.ClickDownArrowbutton(2, 2);
                }
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.LinkSelectedOffset);
                PageLoadWait.WaitForPageLoad(5);
                PageLoadWait.WaitForFrameLoad(5);
                //Select square box then green check mark.
                StudyVw.SelectLinkedCheckBox(1, 1);
                StudyVw.SelectLinkedCheckBox(1, 2);
                StudyVw.SelectLinkedCheckBox(2, 2);
                StudyVw.LinkedScrollingCheckBtn().Click();
                PageLoadWait.WaitForPageLoad(5);
                PageLoadWait.WaitForFrameLoad(5);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                bool step36_3 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel(), 3);

                //Unlink All
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.Unlink);
                PageLoadWait.WaitForPageLoad(5);
                PageLoadWait.WaitForFrameLoad(5);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 4);
                bool step36_4 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel(), 4, 1);
                if (step36_1 && step36_2 && step36_3 && step36_4)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                studies.CloseStudy();

                //Step 37: Calibration lines 
                //Test data - Load CR modality for calibration lines
                studies.ClearFields();
                studies.SearchStudy("last", lastName[5]);
                studies.ChooseColumns(new String[] { "Description" });
                studies.SelectStudy("Description", patientID[1]);
                StudyVw = studies.LaunchStudy();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                //Select tool
                //IWebElement CalibrationTool = BasePage.Driver.FindElement(By.CssSelector("a[class^='AnchorClass32 toplevel']+ul>li[title^='Calibration Tool']>a>img[class$='disableOnCine']"));
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.CalibrationTool);
                PageLoadWait.WaitForPageLoad(5);
                PageLoadWait.WaitForFrameLoad(5);
                //Draw calibration line
                StudyVw.CalibrationTool(StudyVw.SeriesViewer_1X1(), "1_1", StudyVw.SeriesViewer_1X1().Size.Width / 2, StudyVw.SeriesViewer_1X1().Size.Height / 3, StudyVw.SeriesViewer_1X1().Size.Width / 2, StudyVw.SeriesViewer_1X1().Size.Height / 2, "50");
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step37 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.SeriesViewer_1X1());
                if (step37)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Remove all annotations
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.RemoveAllAnnotations);
                PageLoadWait.WaitForPageLoad(5);
                PageLoadWait.WaitForFrameLoad(5);

                //Step 38: Plumb lines
                element1 = StudyVw.SeriesViewer_1X1();
                StudyVw.DrawHorizontalPlumbLine(element1, element1.Size.Width / 2, element1.Size.Height / 3);
                StudyVw.DrawVerticalPlumbLine(element1, element1.Size.Width / 2, element1.Size.Height / 2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step38 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.SeriesViewer_1X1());
                if (step38)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                studies.CloseStudy();

                //Step 39: Localizer Lines 
                //Data - Therapy, Head
                studies.ClearFields();
                studies.SearchStudy(LastName: lastName[4], Datasource: DSList[0]);
                studies.ChooseColumns(new String[] { "Patient ID" });
                studies.SelectStudy("Patient ID", patientID[0]);
                StudyVw = studies.LaunchStudy();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step39_1 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.SeriesViewer_1X1());
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                Thread.Sleep(3000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step39_2 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel(), 2, 1);
                if (step39_1 && step39_2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                studies.CloseStudy();

                //Step 40: Edge Enhancement
                //Load a study with Xray/Cardio - XA modality
                studies.ClearFields();
                studies.SearchStudy("last", lastName[6]);
                studies.ChooseColumns(new String[] { "Patient ID" });
                studies.SelectStudy("Patient ID", patientID[2]);
                StudyVw = studies.LaunchStudy();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x2);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.EdgeEnhancementInteractive);
                StudyVw.DragMovement(StudyVw.SeriesViewer_1X1());
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step40_1 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.SeriesViewer_1X1());
                //Medium 3x3
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.EdgeEnhancementMedium3x3);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step40_2 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.SeriesViewer_1X1(), 2);
                //Low 3x3
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.EdgeEnhancementLow5x5);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                bool step40_3 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.SeriesViewer_1X1(), 3);
                //Medium 11x11
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.EdgeEnhancementMedium11x11);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 4);
                bool step40_4 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.SeriesViewer_1X1(), 4, 1);
                if (step40_1 && step40_2 && step40_3 && step40_4)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Reset
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.Reset);

                //Step 41: Text and Annotation
                //viewer.DrawTextAnnotation(StudyVw.SeriesViewer_1X1(), 150, 150, By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_inputBox"), "test");
                //Text 1
                StudyVw.DrawTextAnnotation(StudyVw.SeriesViewer_1X1(), StudyVw.SeriesViewer_1X1().Size.Width / 4, StudyVw.SeriesViewer_1X1().Size.Height / 4, By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_inputBox"), "Text1");
                //Text 2
                StudyVw.DrawTextAnnotation(StudyVw.SeriesViewer_1X1(), StudyVw.SeriesViewer_1X1().Size.Width / 2, StudyVw.SeriesViewer_1X1().Size.Height / 2, By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_inputBox"), "Text2");
                PageLoadWait.WaitForLoadInViewport(5, viewportload);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_1x1));
                bool step41_1 = studies.CompareImage(result.steps[ExecutedSteps], viewport, 1);
                //Edit Annotations
                StudyVw.EditTextAnnotation(StudyVw.SeriesViewer_1X1(), StudyVw.SeriesViewer_1X1().Size.Width / 2, StudyVw.SeriesViewer_1X1().Size.Height / 2, By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_inputBox"), "Updated");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step41_2 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.SeriesViewer_1X1(), 2);
                //Delete Annotations
                StudyVw.DeleteTextAnnotation(StudyVw.SeriesViewer_1X1(), StudyVw.SeriesViewer_1X1().Size.Width / 2, StudyVw.SeriesViewer_1X1().Size.Height / 2, By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_inputBox"));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                bool step41_3 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.SeriesViewer_1X1(), 3);
                //Test delete all annotations
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.RemoveAllAnnotations);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 4);
                bool step41_4 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.SeriesViewer_1X1(), 4, 1);

                if (step41_1 && step41_2 && step41_3 && step41_4)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 42: Pixel
                StudyVw.GetPixelValueTool(StudyVw.SeriesViewer_1X1());
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step42 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.SeriesViewer_1X1());

                if (step42)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                studies.CloseStudy();

                //Step 43: Email Study
                //Check if email study enabled in service tool in environment setup

                //Enable Email study in Role
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectDomainfromDropDown("SuperAdminGroup");
                rolemanagement.SearchRole("SuperRole");
                rolemanagement.SelectRole("SuperRole");
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("email", 0);
                rolemanagement.ClickSaveEditRole();

                //Search and load study
                studies = (Studies)login.Navigate("Studies");
                studies.ClearFields();
                studies.SearchStudy("last", lastName[5]);
                studies.ChooseColumns(new String[] { "Description" });
                studies.SelectStudy("Description", patientID[1]);
                StudyVw = studies.LaunchStudy();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                //Select tool
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.EmailStudy);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#EmailStudyControl_m_emailToTextBox")));
                IWebElement sendemail = BasePage.Driver.FindElement(By.CssSelector("#EmailStudyControl_SendStudy"));
                sendemail.Click();
                String ErrorMsg = BasePage.Driver.FindElement(By.CssSelector("#EmailStudyControl_m_errorMessageLable")).Text;
                BasePage.Driver.FindElement(By.CssSelector("#EmailStudyDialogDiv>div>span.buttonRounded_small_blue")).Click();
                PageLoadWait.WaitForElement(By.CssSelector("#EmailStudyDialogDiv"), BasePage.WaitTypes.Invisible, 10);
                StudyVw.EmailStudy(EmailID, EmailID.Split('@')[0], Reason, 1);
                string PinCode = PageLoadWait.WaitForElement(By.CssSelector("#EmailStudyControl_PinCode_Label"), BasePage.WaitTypes.Visible, 10).Text;
                PageLoadWait.WaitForElement(By.CssSelector("#PinCodeDialogDiv>div.titlebar>span.buttonRounded_small_blue"), BasePage.WaitTypes.Visible, 10).Click();
                BasePage.Driver.FindElement(By.CssSelector("#EmailStudyDialogDiv>div>span.buttonRounded_small_blue")).Click();
                bool step43 = PageLoadWait.WaitForElement(By.CssSelector("#EmailStudyDialogDiv"), BasePage.WaitTypes.Invisible, 10).Displayed;
                if (!step43 && ErrorMsg == "The email address cannot be empty." && PinCode != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                studies.CloseStudy();

                //Step 44: PDF report - Enabled in service tool, domain and role at start of test case
                //Delete report if already exists
                var dir = new DirectoryInfo(FolderPath);
                foreach (var file in dir.EnumerateFiles(FileList[0].Split('.')[0] + "*." + FileList[0].Split('.')[1]))
                {
                    file.Delete();
                }

                //Search and load study
                studies = (Studies)login.Navigate("Studies");
                studies.ClearFields();
                studies.SearchStudy("last", lastName[7]);
                studies.ChooseColumns(new String[] { "Patient ID" });
                studies.SelectStudy("Patient ID", patientID[3]);
                StudyVw = studies.LaunchStudy();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                viewer.ReportView();
                //Select tool
                viewer.ClickElement("Generate PDF Report");
                PageLoadWait.WaitForFrameLoad(10);

                if (Config.BrowserType == "firefox")
                {
                    var x = Process.GetProcessesByName("firefox")[0].Id;

                    Logger.Instance.InfoLog("Application's process ID : " + x);

                    WpfObjects._application = Application.Attach(x);

                    wpfobject.GetMainWindowByIndex(1);
                    bool buttonexists = wpfobject.VerifyElement("OK", "OK", 1);

                    wpfobject.ClickButton("OK", 1);

                }

                else if (Config.BrowserType == "ie")
                {
                    var x = Process.GetProcessesByName("iexplore")[0].Id;

                    Logger.Instance.InfoLog("Application's process ID : " + x);

                    WpfObjects._application = Application.Attach(x);
                    wpfobject.GetMainWindowByIndex(0);

                    Panel pane = WpfObjects._application.GetWindows()[0].Get<TestStack.White.UIItems.Panel>(TestStack.White.UIItems.Finders.SearchCriteria.All);
                    wpfobject.WaitTillLoad();

                    TestStack.White.InputDevices.Mouse.Instance.Click(new System.Windows.Point(((pane.Items[1].Location.X + pane.Items[2].Location.X) / 2 + 1), ((pane.Items[1].Location.Y + pane.Items[2].Location.Y) / 2 + 1)));
                    wpfobject.WaitTillLoad();

                }
                //Check if file is downloaded
                if (File.Exists(FolderPath + Path.DirectorySeparatorChar + FileList[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                studies.CloseStudy();

                //Step 45: Download document
                //XDS setup should be enabled beforehand

                //Delete document if already exists
                dir = new DirectoryInfo(FolderPath);
                foreach (var file in dir.EnumerateFiles(FileList[1].Split('.')[0] + "*." + FileList[1].Split('.')[1]))
                {
                    file.Delete();
                }
                //Search and load study
                studies = (Studies)login.Navigate("Studies");
                studies.ClearFields();
                studies.SearchStudy(LastName: lastName[8], Datasource: DSList[1]);
                studies.ChooseColumns(new String[] { "Patient ID" });
                studies.SelectStudy("Patient ID", patientID[4]);
                StudyVw = studies.LaunchStudy();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                StudyVw.SeriesViewer_1X2().Click();
                PageLoadWait.WaitForFrameLoad(10);
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.DownloadDocument);
                PageLoadWait.WaitForDownload(FileList[1].Split('.')[0], FolderPath, FileList[1].Split('.')[1]);
                // Download check step to be included
                if (File.Exists(FolderPath + Path.DirectorySeparatorChar + FileList[1]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //studies.CloseStudy();

                //Step 46: External Application - Not Automated
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 47: Transfer study  - Enabled domain and role at start of test case
                //Enable in service tool - Transfer and data download
                //Delete document if already exists
                dir = new DirectoryInfo(FolderPath);
                foreach (var file in dir.EnumerateFiles(FileList[2].Split('.')[0] + "*." + FileList[2].Split('.')[1]))
                {
                    file.Delete();
                }
                //studies = (Studies)login.Navigate("Studies");
                //studies.ClearFields();
                //studies.SearchStudy("last", lastName[8]);
                //studies.SelectStudy("Patient ID", patientID[4]);
                //StudyVw = studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                StudyVw.SeriesViewer_2X1().Click();
                PageLoadWait.WaitForFrameLoad(10);
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.TransferStudy);
                StudyVw.TransferStudy("Local System", "", true);
                PageLoadWait.WaitForDownload(FileList[2].Split('.')[0], FolderPath, FileList[2].Split('.')[1]);
                // Download check step to be included
                if (File.Exists(FolderPath + Path.DirectorySeparatorChar + FileList[2]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //studies.CloseStudy();

                //Step 48: Grant Access
                //Enable Grant access at Domain and Role level- Enabled in domain and role at start of test case
                //Users to be created for selecting in grant access
                //studies = (Studies)login.Navigate("Studies");
                //studies.ClearFields();
                //studies.SearchStudy("last", lastName[8]);
                //studies.SelectStudy("Patient ID", patientID[4]);
                //StudyVw = studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.GrantAccesstoStudy);
                StudyVw.ShareStudy(false, new string[] { phUsername, arusername }, true);
                ExecutedSteps++;
                studies.CloseStudy();
                login.Logout();

                //Step 49: Nominate for archive
                //Image sharing is enabled
                //Create users and destinations
                //Login as Archivist 
                ei.EIDicomUpload(phUsername, phpassword, Config.Dest1, UploadFilePaths[0]);
                ei.EIDicomUpload(phUsername, phpassword, Config.Dest1, UploadFilePaths[1]);
                login.LoginIConnect(Config.ph1UserName, Config.ph1Password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                IWebElement NominateButton = BasePage.Driver.FindElement(By.CssSelector("#m_nominateStudyButton"));

                inbounds.ClearFields(1);
                inbounds.SearchStudy("Accession", AccessionNumbers[0]);
                studies.ChooseColumns(new String[] { "Accession" });
                inbounds.SelectStudy("Accession", AccessionNumbers[0]);

                //Nominate for Archive
                inbounds.NominateForArchive(Reason);
                login.Logout();
                ExecutedSteps++;

                //Step 50: Archive study
                login.LoginIConnect(Config.ar1UserName, Config.arPassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.ClearFields(1);
                inbounds.SearchStudy("accessionNo", AccessionNumbers[0]);
                studies.ChooseColumns(new String[] { "Accession" });
                inbounds.SelectStudy1(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[0], "Nominated For Archive" });
                inbounds.ArchiveStudy("", "Testing");
                login.Logout();
                login.LoginIConnect(Config.ar1UserName, Config.arPassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.ClearFields(1);
                inbounds.SearchStudy("accessionNo", AccessionNumbers[0]);
                studies.ChooseColumns(new String[] { "Accession" });
                Dictionary<string, string> archivedstudy = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[0], "Routing Completed" });
                if (archivedstudy != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 51: Reroute study
                //inbounds.SearchStudy("Accession", AccessionNumbers[0]);
                inbounds.SelectStudy("Accession", AccessionNumbers[0]);
                inbounds.RerouteStudy(Config.Dest2);
                ExecutedSteps++;
                login.Logout();

                //Step 52: Add Receiver
                login.LoginIConnect(Config.ph1UserName, Config.ph1Password);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.ClearFields(1);
                inbounds.SearchStudy("Accession", AccessionNumbers[1]);
                studies.ChooseColumns(new String[] { "Accession" });
                inbounds.SelectStudy("Accession", AccessionNumbers[1]);
                inbounds.LaunchStudy();
                inbounds.AddReceiverInReviewToolbar(arusername);
                ExecutedSteps++;
                inbounds.CloseStudy();
                login.Logout();

                //Step 53: Conference Folder
                //Enable in Service tool - Done at start of test case
                //Enable in Domain and role - create domain and roles
                login.LoginIConnect(username, password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                //DomainB 
                domainmanagement.CreateDomain(TestdomainB, TestdomainRoleB, datasources);
                domainmanagement.ClickSaveNewDomain();
                bool step2 = domainmanagement.SearchDomain(TestdomainB);
                domainmanagement.SetConfListFeatureForDomain(TestdomainB);

                //1 Role to be created
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.CreateRole(TestdomainB, TestroleB1, "any");

                //Enable conference list in RoleB1 
                rolemanagement.SearchRole(TestroleB1, TestdomainB);
                rolemanagement.SelectRole(TestroleB1);
                rolemanagement.ClickEditRole();
                PageLoadWait.WaitForFrameLoad(5);
                rolemanagement.SetCheckboxInEditRole("conferenceuser", 0);
                PageLoadWait.WaitForFrameLoad(5);
                bool CB2_2 = rolemanagement.ConferenceUserCB().Selected;
                rolemanagement.ClickSaveEditRole();

                //User B1 in Role B1
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(TestuserB1, TestdomainB, TestroleB1);//Conference User
                login.LoginIConnect(TestdomainB, TestdomainB);
                conferencefolders = (ConferenceFolders)login.Navigate("ConferenceFolders");
                conferencefolders.CreateToplevelFolder(TopFolderB1, TestuserB1); //First Top Folder
                conferencefolders.CreateSubFolder(TopFolderB1, SubFolderB1_Level2_1);

                //Load study and add to conference folder
                login.LoginIConnect(TestuserB1, TestuserB1);
                studies = (Studies)login.Navigate("Studies");
                studies.ClearFields();
                studies.SearchStudy("last", lastName[5]);
                studies.ChooseColumns(new String[] { "Description" });
                studies.SelectStudy("Description", patientID[1]);
                StudyVw = studies.LaunchStudy();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                //Add to conf folder
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.AddToConferenceFolder);
                StudyVw.AddStudyToStudyFolder(TopFolderB1 + "/" + SubFolderB1_Level2_1, null, StudyNotes_Level1_Subfolder);
                ExecutedSteps++;

                studies.CloseStudy();
                login.Logout();

                //Return Result
                result.FinalResult(ExecutedSteps);
                return result;

            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }

        }

        /// <summary>
        /// All-In-One Tool
        /// </summary>
        public TestCaseResult Test_27875(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            Studies studies = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;
                String RoleName = Config.adminRoleName;
                //
                String PIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String LastNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String[] Measurementtools = { "Line Measurement", "Calibration Tool", "Transischial Measurement", "Joint Line Measurement", "Horizontal Plumb Line", "Vertical Plumb Line" };

                //Step 1
                login.LoginIConnect(username, password);
                studies = (Studies)login.Navigate("Studies");
                //Search and Select Study
                studies.ClearFields();
                studies.SetText("id", "m_studySearchControl_m_searchInputPatientLastName", LastNameList);
                studies.SearchStudy("patientID", PIDList);
                studies.SelectStudy("Patient ID", PIDList);
                studies.LaunchStudy();
                studies.ClickElement("All in One Tool");
                IWebElement ele = BasePage.Driver.FindElement(By.CssSelector(Locators.CssSelector.AllinOneTool));
                if (ele.GetAttribute("class").Contains("highlight32"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 2
                IWebElement element = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_1x1));

                int h = element.Size.Height;
                int w = element.Size.Width;

                var action = new Actions(BasePage.Driver);
                action.MoveToElement(element, w / 2, h / 4).ClickAndHold().MoveToElement(element, w / 2, 3 * h / 4).Build().Perform();      //Mouse Left btn operation

                string step2 = studies.GetElementCursorType(element);
                if (step2.Contains("move"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                Thread.Sleep(1000);

                //Step 3
                action.Release().Build().Perform();
                Thread.Sleep(1000);
                string step3 = studies.GetElementCursorType(element);
                if (step3.Contains("default"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 4 & 5- Middle mouse buttons not automated
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 6 & 7 -  Right click and hold not automated
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 8
                studies.ClickElement("Window Level");
                string step8 = studies.GetElementCursorType(element);
                if (step8.Contains("move"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 9
                studies.Click("id", Locators.ID.SeriesViewer1_1x1);
                int n = 0;
                while (n < 2)
                {
                    action.MoveToElement(element, w - (w / 2), h - (h / 2)).ClickAndHold().MoveToElement(element, w - (w / 3), h - (h / 3)).Build().Perform();
                    Thread.Sleep(1000);
                    action.Release().Build().Perform();
                    Thread.Sleep(1500);
                    n++;
                }
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                var viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_1x1));
                bool step9_1 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step9_1)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 10-12: Middle and Right mouse operations - NA
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 13
                studies.ClickElement("All in One Tool");
                Thread.Sleep(750);
                string step13 = studies.GetElementCursorType(element);
                if (step3.Contains("default"))
                {
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
                viewer.DragThumbnailToViewport(2, Locators.ID.SeriesViewer1_1x1);
                ele = BasePage.Driver.FindElement(By.CssSelector(Locators.CssSelector.AllinOneTool));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_1x1));
                bool step14_1 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step14_1 && ele.GetAttribute("class").Contains("highlight32"))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 15 - 17 (Middle and right mouse button - Not automated)
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 18
                studies.ClickElement("Magnifier x2");
                ele = BasePage.Driver.FindElement(By.CssSelector(Locators.CssSelector.AllinOneTool));
                if (ele.GetAttribute("class").Contains("notSelected32 disableOnCine"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 19
                action.MoveToElement(element).MoveByOffset(-200, -100).Click();
                IAction clickNextElement = action.Build();
                Thread.Sleep(1000);
                clickNextElement.Perform();
                Thread.Sleep(1500);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_1x1));
                bool step19_1 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step19_1)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                viewer.KeyboardArrowScroll("id", Locators.ID.SeriesViewer1_1x1, 1, Keys.Escape);

                //Step 20-21 - Middle and Right click operations
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 22
                studies.ClickElement("Toggle Text");
                Thread.Sleep(1000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_1x1));
                bool step22_1 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step22_1)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 23
                studies.ClickElement("Toggle Text");
                Thread.Sleep(1000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_1x1));
                bool step23_1 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step23_1)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 24
                IWebElement measurement = viewer.GetReviewTool("Line Measurement");
                viewer.JSMouseHover(measurement);
                //Validate Elements are visible
                Thread.Sleep(1000);
                IList<IWebElement> dropdown24 = BasePage.Driver.FindElements(By.CssSelector("li[title='Line Measurement'] ul>li"));
                IList<String> title = new List<String>();
                bool flag24 = true;
                foreach (IWebElement dropdowntool in dropdown24)
                {
                    title.Add(dropdowntool.GetAttribute("title"));
                    if (!(dropdowntool.Enabled && dropdowntool.Displayed))
                    {
                        flag24 = false;
                        break;
                    }
                }
                if (flag24 && dropdown24.Count == 6)
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
                //Step 25
                element = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_1x1));
                studies.ClickElement("Line Measurement");

                string step25 = studies.GetElementCursorType(element);
                if (step25.Contains("crosshair"))
                {
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
                action = new Actions(BasePage.Driver);

                action.MoveToElement(element, 100, 100).Click().Build().Perform();
                Thread.Sleep(3000);

                action.MoveToElement(element, 200, 100).Click().Build().Perform();

                Thread.Sleep(2000);
                if (!((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("firefox"))
                {
                    action.Release(element).Build().Perform();
                    Thread.Sleep(2000);
                }
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_1x1));
                bool step26_1 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step26_1)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 27
                viewer.DrawRectangle(element, element.Size.Width / 3, element.Size.Height / 3, element.Size.Width / 2, element.Size.Height / 2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_1x1));
                bool step27_1 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step27_1)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 28
                viewer.DrawEllipse(element, 120, 120, 180, 150);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_1x1));
                bool step28_1 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step28_1)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 29
                viewer.DrawROI(viewer.SeriesViewer_1X1(), 200, 34, 400, 210, 190, 280, 150, 330);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_1x1));
                bool step29_1 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step29_1)
                {
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
                viewer.DrawAngleMeasurement(viewer.SeriesViewer_1X1(), 200, 150, 120, 160, 160, 150);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_1x1));
                bool step30_1 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step30_1)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 31
                viewer.DrawCobbAngle(viewer.SeriesViewer_1X1(), 170, 90, 80, 60, 170, 90, 50, 130);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_1x1));
                bool step31_1 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step31_1)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 32 & 33 - ENter Text
                IWebElement viewportload = studies.GetElement("id", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_SeriesViewer_ImagesPanel");
                ExecutedSteps++;
                viewer.DrawTextAnnotation(element, 150, 150, By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_inputBox"), "test");
                PageLoadWait.WaitForLoadInViewport(5, viewportload);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_1x1));
                bool step32_1 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step32_1)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 34
                studies.ClickElement("Remove All Annotations");
                PageLoadWait.WaitForLoadInViewport(5, viewportload);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_1x1));
                bool step34_1 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step34_1)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 35 & 36 - Mouse Scroll up and down
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 37
                viewer.DragScrollbarDown(1, 1);
                PageLoadWait.WaitForLoadInViewport(5, viewportload);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_1x1));
                bool step37_1 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step37_1)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 38
                studies.ClickElement("Next Series");
                PageLoadWait.WaitForLoadInViewport(5, viewportload);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_1x1));
                bool step38_1 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step38_1)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 39
                studies.ClickElement("Previous Series");
                PageLoadWait.WaitForLoadInViewport(5, viewportload);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_1x1));
                bool step39_1 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step39_1)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 40
                studies.CloseStudy();
                ExecutedSteps++;

                login.Logout();
                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }


        }

        /// <summary>
        /// Connectivity Tool
        /// </summary>
        public TestCaseResult Test_64842(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables        
            Studies studies = null;
            studies = new Studies();
            Patients patients = null;
            patients = new Patients();
            Viewer viewer = null;
            viewer = new Viewer();
            DomainManagement domainmanagement;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;
                String RoleName = Config.adminRoleName;
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String Name = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                String[] lastName = LastName.Split(':');
                String BrowserList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "BrowserList");
                String[] Browser = BrowserList.Split(':');


                //Step-1
                //Service tool precondition coded in Browser setup
                login.DriverGoTo(login.url);
                if (BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#ConnDiv"))).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //step-2 - Check Connection test tool is visible 
                login.LoginIConnect(username, password);
                studies.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                studies.SetCheckbox("id", "ConnTestToolCB");
                studies.SetCheckbox("id", "ConnTestToolCB");
                studies.CloseUserPreferences();
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                studies = (Studies)login.Navigate("Studies");
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                if (BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#ConnectionTestDiv"))).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //step-3-Verify Connect Test tool visible in after Study search
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                studies.SearchStudy("last", lastName[0]);
                studies.SelectStudy("Patient Name", Name);
                studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForFrameLoad(40);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                Thread.Sleep(8000);
                BasePage.wait.Until<Boolean>(d =>
                {
                    if (!d.FindElement(By.CssSelector("div#ConnectionTestToolDiv")).GetAttribute("style").Contains("display:none"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                if (BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div#ConnectionTestToolDiv"))).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-4,5,6-Older versions of IE Browser
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step-7
                //Add existing Chrome session to List
                BasePage.MultiDriver.Add(BasePage.Driver);

                //Open Another IE 11 Bowser.
                BasePage.MultiDriver.Add(login.InvokeBrowser(Browser[0]));
                login.SetDriver(BasePage.MultiDriver[1]);
                login.DriverGoTo(login.url);
                if (BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#ConnDiv"))).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-8
                login.LoginIConnect(username, password);
                studies = (Studies)login.Navigate("Studies");
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                if (BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#ConnectionTestDiv"))).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-9
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                studies.SearchStudy("last", lastName[0]);
                studies.SelectStudy("Patient Name", Name);
                StudyViewer viewers = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForFrameLoad(40);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewers.Thumbnails()[1]));
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                if (BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#ConnectionTestToolDiv"))).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Open Firefox Borwser                  
                BasePage.MultiDriver.Add(login.InvokeBrowser(Browser[1]));
                login.SetDriver(BasePage.MultiDriver[2]);
                login.DriverGoTo(login.url);
                if (BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#ConnDiv"))).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-11
                login.LoginIConnect(username, password);
                studies = (Studies)login.Navigate("Studies");
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                if (BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#ConnectionTestDiv"))).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-12
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                studies.SearchStudy("last", lastName[0]);
                studies.SelectStudy("Patient Name", Name);
                studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForFrameLoad(40);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                if (BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#ConnectionTestToolDiv"))).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-13,14
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                studies.CloseStudy();
                login.Logout();

                //Return Result
                result.FinalResult(ExecutedSteps);
                return result;


            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }

            finally
            {
                //Close all browsers
                login.ResetDriver();
                login.Logout();

                //Remove Connection test tool
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = login.Navigate<Studies>();
                studies.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                studies.UnCheckCheckbox("id", "ConnTestToolCB");
                studies.CloseUserPreferences();

                //Logout
                login.Logout();
            }
        }

        /// <summary>
        /// Full screen with Series Layout
        /// </summary>
        public TestCaseResult Test_72746(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables          
            Inbounds inbounds = null;
            Studies studies = null;
            studies = new Studies();

            Patients patients = null;
            patients = new Patients();

            Viewer viewer = null;
            viewer = new Viewer();
            DomainManagement domainmanagement;
            RoleManagement rolemanagement = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            String DefaultTag = "";
            UserManagement usermanagement;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;
                String RoleName = Config.adminRoleName;

                String username1 = Config.ar1UserName;
                String password1 = Config.ar1Password;
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                String pacusername = Config.pacsadmin;
                String pacpassword = Config.pacspassword;

                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String pid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String BrowserList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "BrowserList");
                String[] Browser = BrowserList.Split(':');

                //step-1
                login.DriverGoTo(login.url);
                if (BasePage.Driver.FindElement(By.CssSelector("#ctl00_LoginMasterContentPlaceHolder_LoginButton")).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //step-2
                login.LoginIConnect(username, password);
                studies = (Studies)login.Navigate("Studies");
                studies.SetText("id", "m_studySearchControl_m_searchInputPatientLastName", LastName);
                studies.SearchStudy("patientID", pid);
                studies.SelectStudy("Patient ID", pid);
                studies.LaunchStudy();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewerDiv"), BasePage.WaitTypes.Visible);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                if (BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewerDiv")).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-3
                studies.ClickElement("Full Screen");
                PageLoadWait.WaitForFrameLoad(5);
                PageLoadWait.WaitForElement(By.Id("studyPanelDiv_1"), BasePage.WaitTypes.Visible);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                BasePage.wait.Until<Boolean>(driver =>
                {
                    IList<IWebElement> tools = driver.FindElements((By.CssSelector("div#StudyToolbarContainer ul li>a>img")));
                    foreach (IWebElement tool in tools)
                    {
                        if (tool.Displayed)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return false;
                });

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                var viewport = BasePage.Driver.FindElement(By.Id("ViewerContainer"));
                bool step72746 = patients.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step72746)
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


                //step-4
                studies.GetElement("id", "recallToolsDiv").Click();
                PageLoadWait.WaitForFrameLoad(5);
                studies.ClickElement("Series Viewer 1x1");
                PageLoadWait.WaitForElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_SeriesViewerPanel"), BasePage.WaitTypes.Visible);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id("studyPanelDiv_1"));
                bool step72746_4 = patients.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step72746_4)
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


                //step-5
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step-6
                studies.GetElement("id", "recallToolsDiv").Click();
                PageLoadWait.WaitForFrameLoad(5);
                PageLoadWait.WaitForElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_SeriesViewerPanel"), BasePage.WaitTypes.Visible);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id("studyPanelDiv_1"));
                bool step72746_6 = patients.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step72746_6)
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


                //step-7
                studies.ClickElement("Series Viewer 1x2");
                PageLoadWait.WaitForElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_SeriesViewerPanel"), BasePage.WaitTypes.Visible);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id("studyPanelDiv_1"));
                bool step72746_7 = patients.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step72746_7)
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


                //step-8
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step-9
                studies.Click("id", studies.GetControlId("SeriesViewer1-1X2"));
                studies.Doubleclick("id", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_SeriesViewerPanel");
                PageLoadWait.WaitForFrameLoad(5);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_SeriesViewerPanel"));
                bool step72746_9 = patients.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step72746_9)
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


                //step-10
                studies.Doubleclick("id", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg");
                PageLoadWait.WaitForFrameLoad(5);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id("studyPanelDiv_1"));
                bool step72746_10 = patients.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step72746_10)
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

                //step-11
                studies.GetElement("id", "recallToolsDiv").Click();
                studies.ClickElement("Series Viewer 1x3");
                PageLoadWait.WaitForElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_SeriesViewerPanel"), BasePage.WaitTypes.Visible);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id("studyPanelDiv_1"));
                bool step72746_11 = patients.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step72746_11)
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

                //step-12
                studies.Doubleclick("id", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg");
                PageLoadWait.WaitForFrameLoad(5);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id("studyPanelDiv_1"));
                bool step72746_12 = patients.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step72746_12)
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

                //step-13
                studies.Doubleclick("id", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg");
                PageLoadWait.WaitForFrameLoad(5);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id("studyPanelDiv_1"));
                bool step72746_13 = patients.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step72746_13)
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

                //step-14
                studies.GetElement("id", "recallToolsDiv").Click();
                studies.ClickElement("Series Viewer 2x2");
                PageLoadWait.WaitForElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_SeriesViewerPanel"), BasePage.WaitTypes.Visible);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id("studyPanelDiv_1"));
                bool step72746_14 = patients.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step72746_14)
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


                //step-15

                studies.Doubleclick("id", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg");

                PageLoadWait.WaitForFrameLoad(5);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);

                viewport = BasePage.Driver.FindElement(By.Id("studyPanelDiv_1"));
                bool step72746_15 = patients.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step72746_15)
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

                //step-16

                studies.Doubleclick("id", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg");

                PageLoadWait.WaitForFrameLoad(5);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);

                viewport = BasePage.Driver.FindElement(By.Id("studyPanelDiv_1"));
                bool step72746_16 = patients.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step72746_16)
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

                //step-17

                studies.GetElement("id", "recallToolsDiv").Click();
                studies.ClickElement("Series Viewer 2x3");

                PageLoadWait.WaitForElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_SeriesViewerPanel"), BasePage.WaitTypes.Visible);

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");


                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);

                viewport = BasePage.Driver.FindElement(By.Id("studyPanelDiv_1"));
                bool step72746_17 = patients.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step72746_17)
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

                //step-18

                studies.Doubleclick("id", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg");

                PageLoadWait.WaitForFrameLoad(5);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);

                viewport = BasePage.Driver.FindElement(By.Id("studyPanelDiv_1"));
                bool step72746_18 = patients.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step72746_18)
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

                //step-19
                studies.Doubleclick("id", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg");

                PageLoadWait.WaitForFrameLoad(5);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);

                viewport = BasePage.Driver.FindElement(By.Id("studyPanelDiv_1"));
                bool step72746_19 = patients.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step72746_19)
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
                studies.CloseStudy();
                login.Logout();

                //Return Result
                result.FinalResult(ExecutedSteps);
                return result;


            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }

        }

        /// <summary>
        /// Keyboard Shortcuts
        /// </summary>
        public TestCaseResult Test_72747(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables        
            Studies studies = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;
                String RoleName = Config.adminRoleName;
                String PIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String LastNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");

                //Step-1-Login as Admin                
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //Step-2
                studies = (Studies)login.Navigate("Studies");
                IWebElement SearchPanel = PageLoadWait.WaitForElement(By.CssSelector("#SearchPanelDiv"), BasePage.WaitTypes.Visible, 15);
                bool step2 = SearchPanel.Displayed;
                if (step2 && BasePage.GetSearchResults().Count == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step-3
                studies.SearchStudy("last", "*");
                PageLoadWait.WaitForLoadingMessage(30);
                bool isscrollbar = studies.IsVerticalScrollBarPresent(BasePage.Driver.FindElement(By.CssSelector("#gview_gridTableStudyList > div.ui-jqgrid-bdiv")));
                if (isscrollbar)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4-Press down/up arrow to see if scrolling is done using keyboard
                Int64 scrollpostion_before = (Int64)((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("return(document.querySelector('#gview_gridTableStudyList > div.ui-jqgrid-bdiv').scrollTop)");
                for (int i = 0; i < 5; i++)
                {
                    BasePage.Driver.FindElement(By.Id("gridTableStudyList")).SendKeys(Keys.ArrowDown);
                    //BasePage.mouse_event(0x0800, 0, 0, -100, 0);
                }
                Int64 scrollpostion_after = (Int64)((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("return(document.querySelector('#gview_gridTableStudyList > div.ui-jqgrid-bdiv').scrollTop)");
                if ((scrollpostion_before == 0) && (scrollpostion_after > scrollpostion_before))
                {
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
                studies.ClearFields();
                studies.SearchStudy("accession", AccessionID);
                studies.SelectStudy("Accession", AccessionID);
                StudyViewer viewers = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);

                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewers.Thumbnails()[0]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewers.Thumbnails()[1]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewers.Thumbnails()[2]));
                PageLoadWait.WaitForFrameLoad(40);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                var viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_studyViewerContainer"));
                bool step4_1 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step4_1)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 6 - Mouse scroll                  
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-7
                //Press Down Arrow Keys - twice
                studies.Click("id", Locators.ID.SeriesViewer1_1x1);
                viewer.KeyboardArrowScroll("id", Locators.ID.SeriesViewer1_1x1, 2, Keys.ArrowDown);
                result.steps[++ExecutedSteps].SetPath(testid + "_1_down", ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_1x1));
                bool step6_1 = login.CompareImage(result.steps[ExecutedSteps], viewport);

                //Press Up Arrow Keys - twice
                studies.Click("id", Locators.ID.SeriesViewer1_1x1);
                viewer.KeyboardArrowScroll("id", Locators.ID.SeriesViewer1_1x1, 2, Keys.ArrowUp);
                result.steps[ExecutedSteps].SetPath(testid + "_2_up", ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_1x1));
                bool step6_2 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step6_1 && step6_2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-8
                //Press Right Arrow Key Once
                studies.Click("id", Locators.ID.SeriesViewer1_1x1);
                viewer.KeyboardArrowScroll("id", Locators.ID.SeriesViewer1_1x1, 1, Keys.ArrowRight);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_1x1));
                bool step7_1 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step7_1)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-9
                //Press Left Arrow Key Once
                studies.Click("id", Locators.ID.SeriesViewer1_1x1);
                viewer.KeyboardArrowScroll("id", Locators.ID.SeriesViewer1_1x1, 1, Keys.ArrowLeft);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_1x1));
                bool step8_1 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step8_1)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 10 & 11 - Cine Play and Pause                
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 12 - Perform above steps in HTML5 - NA for the time being
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }


        }

        /// <summary>
        /// Resizing and Moving Inbound Columns
        /// </summary>
        public TestCaseResult Test_72748(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables          
            Inbounds inbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;
                String RoleName = Config.adminRoleName;

                //Step 1- precondition - create test user
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //Step-2 Navigate to inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                IWebElement SearchPanel = PageLoadWait.WaitForElement(By.CssSelector("#SearchPanelDiv"), BasePage.WaitTypes.Visible, 15);
                bool step2 = SearchPanel.Displayed;
                if (step2 && BasePage.GetSearchResults().Count == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-3 Click search button - Marked NA since we have to verify if studylist is dipslayed properly, Image sharing is not configured for browser and this is validated in Image sharing VPs
                result.steps[++ExecutedSteps].status = "Not Automated";

                // Step-4 - Study date column
                IWebElement column1 = BasePage.Driver.FindElement(By.CssSelector("#gridTableInboundsStudyList_studyDateTime"));
                Size width = column1.Size;
                var cursordrag = column1.FindElement(By.CssSelector("span"));

                //Description column
                var cursordrop = BasePage.Driver.FindElement(By.CssSelector("#gridTableInboundsStudyList_description"));
                var actions = new Actions(BasePage.Driver);
                actions.ClickAndHold(cursordrag).MoveToElement(cursordrop).Release(cursordrop).Build().Perform();
                IWebElement column2 = BasePage.Driver.FindElement(By.CssSelector("#gridTableInboundsStudyList_studyDateTime"));
                Size width1 = column2.Size;

                if (width1.Width > width.Width)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5
                //Move any column from right to left and left to right
                String[] columns = BasePage.GetColumnNames();
                inbounds.ReorderStudyListColumns(columns[1], "end");
                String[] columns_afterdragright = BasePage.GetColumnNames();
                int index_afterright = Array.FindIndex<String>(columns_afterdragright, c => c.Equals(columns[1]));
                bool ismovedright = index_afterright > 1 ? true : false;

                inbounds.ReorderStudyListColumns(columns[1], "middle");
                String[] columns_afterdragleft = BasePage.GetColumnNames();
                int index_afterleft = Array.FindIndex<String>(columns_afterdragleft, c => c.Equals(columns[1]));
                bool ismovedleft = index_afterright > index_afterleft ? true : false;
                if (ismovedright && ismovedleft)
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
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("td[title^='Launch Column']>div")));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("td[title^='Launch Column']>div")));
                BasePage.Driver.FindElement(By.CssSelector("td[title^='Launch Column']>div")).Click();

                //Validation 1- Available and Selected columns are displayed with Add all and remove all
                IWebElement selected = BasePage.Driver.FindElement(By.CssSelector("#colchooser_gridTableInboundsStudyList > div > div > div.selected"));
                IWebElement available = BasePage.Driver.FindElement(By.CssSelector("#colchooser_gridTableInboundsStudyList > div > div > div.available"));
                string removeall = BasePage.Driver.FindElement(By.CssSelector("#colchooser_gridTableInboundsStudyList > div > div > div.selected > div > a")).Text;
                string addall = BasePage.Driver.FindElement(By.CssSelector("#colchooser_gridTableInboundsStudyList > div > div > div.available > div > a")).Text;

                //Validation 2- Minus and Plus sign present for each column in Available and Selected columns respectively
                List<IWebElement> minus = selected.FindElements(By.TagName("a")).ToList();
                bool[] step5_1 = new bool[minus.Capacity - 1];
                for (int i = 1; i < minus.Capacity; i++)
                {
                    IWebElement span = minus[i].FindElement(By.TagName("span"));
                    if (span.GetAttribute("class").Contains("minus"))
                    {
                        step5_1[i - 1] = true;
                    }
                }
                List<IWebElement> plus = available.FindElements(By.TagName("a")).ToList();
                bool[] step5_2 = new bool[plus.Capacity - 1];
                for (int i = 1; i < plus.Capacity; i++)
                {
                    IWebElement span = plus[i].FindElement(By.TagName("span"));
                    if (span.GetAttribute("class").Contains("plus"))
                    {
                        step5_2[i - 1] = true;
                    }
                }
                //checking if all elements are true
                bool step5_result_1 = false, step5_result_2 = false;
                foreach (bool res in step5_1)
                {
                    if (!res)
                    {
                        step5_result_1 = false;
                        break;
                    }
                    else
                    {
                        step5_result_1 = true;
                    }
                }
                foreach (bool res in step5_2)
                {
                    if (!res)
                    {
                        step5_result_2 = false;
                        break;
                    }
                    else
                    {
                        step5_result_2 = true;
                    }
                }
                if (selected.Displayed && available.Displayed && removeall.Equals("Remove all") && addall.Equals("Add all") && step5_result_1 && step5_result_2)
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
                string[] column = new string[] { "Modality" };
                inbounds.SelectColumns(column);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.ui-dialog-buttonset>button:nth-of-type(1)")));
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"div.ui-dialog-buttonset>button:nth-of-type(1)\").click()");

                PageLoadWait.WaitForPageLoad(5);
                PageLoadWait.WaitForFrameLoad(5);
                string[] step6 = BasePage.GetColumnNames();
                bool step_6 = false;
                foreach (var item in step6)
                {
                    if (item.Equals("Modality"))
                    {
                        step_6 = true;
                        break;
                    }
                }
                if (step_6)
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


                //Step-8 -- Flakiness in this step hence making as Not Automated
                column1 = BasePage.Driver.FindElement(By.CssSelector("#gridTableInboundsStudyList_modality"));
                int w1 = column1.Size.Width;
                int h1 = column1.Size.Height;
                int x = column1.Location.X;
                int y = column1.Location.Y;
                cursordrag = column1.FindElement(By.CssSelector("span"));
                inbounds.ActionsDragAndDrop(cursordrag, x + 200, y);
                column2 = BasePage.Driver.FindElement(By.CssSelector("#gridTableInboundsStudyList_modality"));
                new Actions(BasePage.Driver).Click(column2.FindElement(By.CssSelector("span"))).Build().Perform();
                int w2 = BasePage.Driver.FindElement(By.CssSelector("#gridTableInboundsStudyList_modality")).Size.Width;
                result.steps[++ExecutedSteps].status = "Not Automated";
                /* if (w2>w1)
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

                //Step-9
                //Move Newly added column from left to right and right to left
                String[] columns9 = BasePage.GetColumnNames();
                int index9 = Array.FindIndex<String>(columns9, c => c.Equals(columns9.Last()));
                inbounds.ReorderStudyListColumns(columns9.Last(), "middle");
                String[] columns_afterdragleft9 = BasePage.GetColumnNames();
                int index_afterleft9 = Array.FindIndex<String>(columns_afterdragleft9, c => c.Equals(columns9.Last()));
                bool ismovedleft9 = index_afterleft9 < index9 ? true : false;

                inbounds.ReorderStudyListColumns(columns9.Last(), "end");
                String[] columns_afterdragright9 = BasePage.GetColumnNames();
                int index_afterright9 = Array.FindIndex<String>(columns_afterdragright9, c => c.Equals(columns9.Last()));
                bool ismovedright9 = index_afterright9 > index_afterleft9 ? true : false;
                if (ismovedright && ismovedleft)
                    if (ismovedleft9 && ismovedright9)
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

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }


        }

        /// <summary>
        /// Searching Criteria
        /// </summary>
        public TestCaseResult Test_72749(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            Studies studies = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;
                String RoleName = Config.adminRoleName;
                String PIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String LastNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String FirstNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String Description = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Description");
                String IPID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "IPID");
                String Modality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String StudyID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyID");
                String PresetName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PresetName");
                String[] PID = PIDList.Split(':');
                String[] LastName = LastNameList.Split(':');
                String[] FirstName = FirstNameList.Split(':');


                //Step 1
                //precondition - Add all search study fields.
                login.LoginIConnect(username, password);
                var domain = login.Navigate<DomainManagement>();
                domain.SearchDomain("SuperAdminGroup");
                domain.SelectDomain("SuperAdminGroup");
                domain.ClickEditDomain();
                domain.ModifyStudySearchFields("show");
                domain.ClickSaveEditDomain();
                var role = login.Navigate<RoleManagement>();
                role.SelectDomainfromDropDown("SuperAdminGroup");
                role.SearchRole("SuperRole");
                role.SelectRole("SuperRole");
                role.ClickEditRole();
                role.UnCheckCheckbox(role.StudySearchFieldUseDomainSetting_CB());
                role.ModifyStudySearchFields("show");
                role.ClickSaveEditRole();
                ExecutedSteps++;

                //Step 2
                studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //Step 3
                studies.Click("id", "ExpandSearchPanelButton");
                bool step3_1 = studies.VerifyElementPresence("id", "SearchPanelDiv");
                //Search panel should not be visible 
                if (!step3_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 4
                studies.Click("id", "ExpandSearchPanelButton");
                bool step4_1 = studies.VerifyElementPresence("id", "SearchPanelDiv");
                //Search panel should be visible 
                if (step4_1)
                {
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
                studies.SearchStudy(LastName: LastName[0]);
                //studies.SearchStudy("last", LastName[0]);
                string[] step5 = studies.GetStudyDetails("Patient Name");
                bool step5_res = (step5 == null || step5.Length == 0) ? false : step5.Where(q => q.ToLower().Contains(LastName[0].ToLower())).Count() == step5.Length;
                if (step5_res)
                {
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
                studies.SearchStudy(FirstName: FirstName[0]);
                //studies.SearchStudy("first", FirstName[0]);
                string[] step6 = studies.GetStudyDetails("Patient Name");
                bool step6_res = (step6 == null || step6.Length == 0) ? false : step6.Where(q => q.ToLower().Contains(FirstName[0].ToLower())).Count() == step6.Length;
                if (step6_res)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 7
                studies.SearchStudy(patientID: PID[1]);
                //studies.SearchStudy("patient", PID[1]);
                string[] step7 = studies.GetStudyDetails("Patient Name");
                bool step7_res = (step7 == null || step7.Length == 0) ? false : step7.Where(q => q.ToLower().Contains(FirstName[0].ToLower())).Count() == step7.Length;
                if (step7_res)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 8
                studies.SearchStudy(Description: Description);
                //studies.SearchStudy("des", Description);
                string[] step8 = studies.GetStudyDetails("Description");
                bool step8_res = (step8 == null || step8.Length == 0) ? false : step8.Where(q => q.ToLower().Contains(Description.ToLower())).Count() == step8.Length;
                if (step8_res)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 9
                studies.ClearFields();
                studies.SearchStudy(LastName: "*", IPID: IPID);
                //studies.SetText("id", "m_studySearchControl_m_searchInputPatientLastName", "*");
                //studies.SearchStudy("ipid", IPID);
                string[] choosecolumn = new string[] { "Issuer of PID" };
                studies.ChooseColumns(choosecolumn);
                string[] step9 = studies.GetStudyDetails("Issuer of PID");
                bool step9_res = (step9 == null || step9.Length == 0) ? false : step9.Where(q => q.ToLower().Contains(IPID.ToLower())).Count() == step9.Length;
                if (step9_res)
                {
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
                //Search Modality
                studies.SearchStudy(LastName: "*", Modality: Modality);
                //studies.SearchStudy("modality", Modality);
                PageLoadWait.WaitForLoadingMessage(20);
                string[] step10_modality = studies.GetStudyDetails("Modality");
                bool step10_1 = (step10_modality == null || step10_modality.Length == 0) ? false : step10_modality.Where(q => q.ToLower().Contains(Modality.ToLower())).Count() == step10_modality.Length;
                //Search Accession
                studies.ClearFields();
                studies.SearchStudy("accession", AccessionID);
                string[] step10_accession = studies.GetStudyDetails("Accession");
                bool step10_2 = (step10_accession == null || step10_accession.Length == 0) ? false : step10_accession.Where(q => q.ToLower().Contains(AccessionID.ToLower())).Count() == step10_accession.Length;
                //Search Study ID
                studies.ClearFields();
                studies.SearchStudy("studyid", StudyID);
                choosecolumn = new string[] { "Study ID" };
                studies.ChooseColumns(choosecolumn);
                string[] step10_studyid = studies.GetStudyDetails("Study ID");
                bool step10_3 = (step10_studyid == null || step10_studyid.Length == 0) ? false : step10_studyid.Where(q => q.ToLower().Contains(StudyID.ToLower())).Count() == step10_studyid.Length;

                if (step10_1 && step10_2 && step10_3)
                {
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
                studies.SearchStudy(LastName: "*");
                PageLoadWait.WaitForLoadingMessage(30);
                IWebElement element = studies.GetElement("cssselector", Locators.CssSelector.SearchPageViewText);
                if (element.Text.Equals("View 1 - 200 of 200"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 12 & 13
                studies.SearchStudy(LastName: LastName[0]);
                studies.SavePreset(PresetName);
                ExecutedSteps++;
                element = studies.GetElement("id", Locators.ID.PresetDropdown);
                SelectElement ele = new SelectElement(element);
                ele.SelectByText(PresetName);
                string step13 = ele.SelectedOption.Text;
                if (step13.Equals(PresetName))
                {
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
                studies.ClearFields();
                ele.SelectByText(PresetName);
                studies.ClickSearchBtn();
                string[] step14 = studies.GetStudyDetails("Patient Name");
                bool step14_res = (step14 == null || step14.Length == 0) ? false : step14.Where(q => q.ToLower().Contains(LastName[0].ToLower())).Count() == step14.Length;
                if (step14_res)
                {
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
                studies.ClearFields();
                studies.SearchStudy("accession", AccessionID);
                PageLoadWait.WaitForElement(By.Id(Locators.ID.SavePresetButton), BasePage.WaitTypes.Visible);
                studies.Click("id", Locators.ID.SavePresetButton);
                PageLoadWait.WaitForElement(By.Id(Locators.ID.PresetTextbox), BasePage.WaitTypes.Visible);
                IWebElement PresetNamed = studies.GetElement("id", "m_savePresetRadio");
                IWebElement PresetNamedTextBox = studies.GetElement("id", "m_searchPresetNameTextBox");
                IWebElement MySearch = studies.GetElement("id", "m_saveAsMySearchRadio");
                IWebElement SaveBtn = studies.GetElement("id", "SaveSearchButton");
                IWebElement CancelBtn = studies.GetElement("id", "CancelSearchSaveButton");
                if (PresetNamed.Displayed && PresetNamedTextBox.Displayed && MySearch.Displayed && SaveBtn.Displayed && CancelBtn.Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 16
                MySearch.Click();
                SaveBtn.Click();
                PageLoadWait.WaitForElement(By.Id("SaveSearchDiv"), BasePage.WaitTypes.Invisible);
                ExecutedSteps++;

                //Step 17
                studies.ClearFields();
                studies.SearchStudy("last", "");
                studies.ClearFields();
                studies.Click("id", "m_studySearchControl_m_defaultSearchButton");
                PageLoadWait.WaitForSearchLoad();
                string[] step17 = studies.GetStudyDetails("Accession");
                bool step17_res = (step17 == null || step17.Length == 0) ? false : step17.Where(q => q.ToLower().Contains(AccessionID.ToLower())).Count() == step17.Length;
                if (step17_res)
                {
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
                login.Logout();
                result.FinalResult(ExecutedSteps);

                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }


        }

        /// <summary>
        /// Window Resizing
        /// </summary>
        public TestCaseResult Test_72750(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            Studies studies = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;
                String RoleName = Config.adminRoleName;
                String LastNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String FirstNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String Names = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                String ModalityToolbarList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ModalityToolbar");
                String PIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] ModalityToolbar = ModalityToolbarList.Split(':');
                String[] PID = PIDList.Split(':');
                String[] Name = Names.Split(':');
                String[] LastName = LastNameList.Split(':');
                String[] FirstName = FirstNameList.Split(':');

                //Step 1                
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //Step 2
                studies = (Studies)login.Navigate("Studies");
                studies.ClearFields();
                studies.SearchStudy("last", LastName[0]);
                studies.SelectStudy("Patient Name", Name[0]);
                StudyViewer viewers = StudyViewer.LaunchStudy();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewers.Thumbnails()[0]));
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                var viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.viewer));
                bool step2 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step2)
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

                //Step 3
                //1x1
                studies.ClickElement("Series Viewer 1x1");
                BasePage.Driver.Manage().Window.Size = new Size(1000, 800);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid + "_1_1x1", ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.Allviewports));
                bool step3_1 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                BasePage.Driver.Manage().Window.Maximize();
                Thread.Sleep(2000);
                //1x2
                studies.ClickElement("Series Viewer 1x2");
                BasePage.Driver.Manage().Window.Size = new Size(1000, 800);
                Thread.Sleep(2000);
                result.steps[ExecutedSteps].SetPath(testid + "_2_1x2", ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.Allviewports));
                bool step3_2 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                BasePage.Driver.Manage().Window.Maximize();
                Thread.Sleep(2000);
                //1x3
                studies.ClickElement("Series Viewer 1x3");
                BasePage.Driver.Manage().Window.Size = new Size(1000, 800);
                Thread.Sleep(2000);
                result.steps[ExecutedSteps].SetPath(testid + "_3_1x3", ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.Allviewports));
                bool step3_3 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                BasePage.Driver.Manage().Window.Maximize();
                Thread.Sleep(2000);
                //2x2
                studies.ClickElement("Series Viewer 2x2");
                BasePage.Driver.Manage().Window.Size = new Size(1000, 800);
                Thread.Sleep(2000);
                result.steps[ExecutedSteps].SetPath(testid + "_4_2x2", ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.Allviewports));
                bool step3_4 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                BasePage.Driver.Manage().Window.Maximize();
                Thread.Sleep(2000);
                //2x3
                studies.ClickElement("Series Viewer 2x3");
                BasePage.Driver.Manage().Window.Size = new Size(1000, 800);
                Thread.Sleep(2000);
                result.steps[ExecutedSteps].SetPath(testid + "_3_2x3", ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.Allviewports));
                bool step3_5 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                BasePage.Driver.Manage().Window.Maximize();
                Thread.Sleep(2000);

                if (step3_1 && step3_2 && step3_3 && step3_4 && step3_5)
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
                studies.CloseStudy();

                //Step 4
                studies.ClearFields();
                studies.SearchStudy("last", LastName[1]);
                studies.SelectStudy("Patient Name", Name[1]);
                studies.LaunchStudy();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewers.Thumbnails()[0]));
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.viewer));
                bool step4 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step4)
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

                //Step 5
                BasePage.Driver.Manage().Window.Size = new Size(800, 1000);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.Allviewports));
                bool step5 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step5)
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

                //Step 6
                BasePage.Driver.Manage().Window.Size = new Size(1000, 700);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.Allviewports));
                bool step6 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step6)
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

                //Step 7 and 8 -Cine play and verification - NA
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 9
                BasePage.Driver.Manage().Window.Size = new Size(500, 500);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.Allviewports));
                bool step9 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step9)
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
                BasePage.Driver.Manage().Window.Maximize();
                Thread.Sleep(2000);

                //Step 10
                string[] windowhandle = viewer.OpenHelpandSwitchtoIT();
                BasePage.Driver.Manage().Window.Size = new Size(1000, 800);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent();
                bool step10 = false;
                try
                {
                    viewport = BasePage.Driver.FindElement(By.CssSelector("html > frameset"));
                    step10 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                }
                catch (Exception) { }
                if (step10)
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
                viewer.CloseHelpView(windowhandle[1], windowhandle[0]);

                //Step 11
                BasePage.Driver.Manage().Window.Size = new Size(400, 400);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id("reviewToolbar"));
                bool step11 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step11)
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
                BasePage.Driver.Manage().Window.Size = new Size(1000, 800);
                Thread.Sleep(2000);
                studies.ClickElement("Full Screen");
                Thread.Sleep(1000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.viewer));
                bool step12 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step12)
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

                //Step 13
                studies.Click("id", "recallMenus");
                PageLoadWait.WaitForFrameLoad(5);
                studies.ClickElement("Full Screen");
                BasePage.Driver.Manage().Window.Maximize();
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.viewer));
                bool step13 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step13)
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

                //Step 14
                studies.ClickElement("Full Screen");
                BasePage.Driver.Manage().Window.Size = new Size(1000, 800);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                //For checking History and Menu tabs
                viewport = BasePage.Driver.FindElement(By.Id("ViewerContainer"));
                bool step14 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step14)
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
                BasePage.Driver.Manage().Window.Maximize();
                Thread.Sleep(2000);
                studies.CloseStudy();

                // Step 15
                //Update Modality toolbar
                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SelectDomain(DomainName);
                //Click Edit in DomainManagement Tab
                domain.ClickEditDomain();
                domain.AddToolsToModalityToolbar(ModalityToolbar, "MR");
                domain.ClickSaveDomain();
                //Load Study
                studies = (Studies)login.Navigate("Studies");
                studies.ClearFields();
                studies.SearchStudy("patientID", PID[0]);
                studies.SelectStudy("Patient ID", PID[0]);
                studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewers.Thumbnails()[0]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewers.Thumbnails()[1]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewers.Thumbnails()[2]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewers.Thumbnails()[5]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewers.Thumbnails()[7]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewers.Thumbnails()[8]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewers.Thumbnails()[10]));
                PageLoadWait.WaitForFrameLoad(40);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id("ViewerContainer"));
                bool step15 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step15)
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

                //Step 16
                BasePage.Driver.Manage().Window.Size = new Size(800, 800);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                //For checking Modality toolbar
                viewport = BasePage.Driver.FindElement(By.Id("ViewerContainer"));
                bool step16 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step16)
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
                studies.CloseStudy();
                BasePage.Driver.Manage().Window.Maximize();
                Thread.Sleep(2000);

                //Step 17
                studies.ClearFields();
                studies.SearchStudy("last", LastName[2]);
                studies.SelectStudy("Patient Name", Name[2]);
                studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForFrameLoad(40);
                Thread.Sleep(5000);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewers.Thumbnails()[10]));


                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id("studyPanelDiv_1"));
                bool step17 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step17)
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

                //Step 18
                studies.NavigateToHistoryPanel();
                BasePage.wait.Until<Boolean>(d =>
                {
                    if (!d.FindElement(By.CssSelector("div#m_patientHistory_drawer")).GetAttribute("style").Contains("right: 0px;"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                });
                Thread.Sleep(2000);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_patientHistory_m_reportViewer_reportFrame");
                IWebElement reportcontainer_before = BasePage.wait.Until<IWebElement>(ExpectedConditions.ElementExists(By.Id("ViewerContainer_Content")));
                int width_before = reportcontainer_before.Size.Width;
                int height_before = reportcontainer_before.Size.Height;
                BasePage.Driver.Manage().Window.Size = new Size(1000, 800);
                Thread.Sleep(2000);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_patientHistory_m_reportViewer_reportFrame");
                var reportcontainer = BasePage.Driver.FindElement(By.Id("ViewerContainer_Content"));
                int width_after = reportcontainer.Size.Width;
                int height_after = reportcontainer.Size.Height;
                bool step18 = ((width_before > width_after) || (height_after < height_before)) ? true : false;
                if (step18)
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
                studies.CloseStudy();

                //Report Result
                login.Logout();
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }


        }
    }
}
