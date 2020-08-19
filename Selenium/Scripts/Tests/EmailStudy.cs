using Selenium.Scripts.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Reusable.Generic;
using OpenQA.Selenium;
using Selenium.Scripts.Pages.MergeServiceTool;
using TestStack.White.UIItems;
using TestStack.White.UIItems.TabItems;
using System.Globalization;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using System.Linq;
using ComboBox = TestStack.White.UIItems.ListBoxItems.ComboBox;
using TestStack.White.UIItems.Finders;
using System.Data;
using Selenium.Scripts.Pages.eHR;

namespace Selenium.Scripts.Tests
{
    class EmailStudy
    {
        public Login login { get; set; }
        public string filepath { get; set; }

        ServiceTool servicetool = new ServiceTool();
        WpfObjects wpfobject = new WpfObjects();
        DomainManagement domain = new DomainManagement();
        RoleManagement role = new RoleManagement();
        WebDriverWait wait1 = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 180));

        public EmailStudy(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            wpfobject = new WpfObjects();
        }

        /// <summary>
        /// Disable Email Study Icon. 
        /// </summary>
        public TestCaseResult Test_164355(String testid, String teststeps, int stepcount)
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
                DomainManagement domainmanagement = null;
                RoleManagement rolemanagement = null;
                Studies studies = null;
                BluRingViewer viewer = new BluRingViewer();
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');

                //Pre-conditions
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                CheckBox EmailStudy = wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), "Enable Email Study", 1);
                if (EmailStudy.Checked)
                {
                    EmailStudy.Click();
                    wpfobject.WaitTillLoad();
                    servicetool.ApplyEnableFeatures();
                    wpfobject.WaitTillLoad();
                    servicetool.wpfobject.ClickOkPopUp();
                    servicetool.RestartService();
                    wpfobject.WaitTillLoad();
                }
                servicetool.CloseServiceTool();

                //step1  Login to iCA application as Administrator.
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //step2 Go to super admin domain management page and verify that "Enable Email study" option is not available.
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(Config.adminGroupName);
                domainmanagement.SelectDomain(Config.adminGroupName);
                domainmanagement.ClickEditDomain();
                bool step2 = viewer.IsElementVisibleInUI(By.CssSelector("[id$='_EnableEmailStudyCB']"));
                if (!step2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step3 Go to super admin role management page and verify that "Allow Email study" option is not available.
                domainmanagement.ClickSaveEditDomain();
                rolemanagement=(RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SearchRole(Config.adminRoleName, Config.adminGroupName);
                rolemanagement.SelectRole(Config.adminRoleName);
                rolemanagement.ClickEditRole();
                bool step3 = viewer.IsElementVisibleInUI(By.CssSelector("[id$='_EnableEmailStudyCB']"));
                if (!step3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step4 Select any study which has priors(say as 5 priors) from studies tab and click on 'Universal' button.
                rolemanagement.ClickSaveEditRole();
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.SanityPACS));
                studies.SelectStudy("Accession", Accession[0]);
                BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //step5 Check for email study icon from primary study control panel.
                bool step5 = viewer.GetElement("cssselector", BluRingViewer.div_emailTitle).GetAttribute("class").Contains("disabled");
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

                //step6 Load all priors studies from the exam list by single click on each prior studies.
                viewer.OpenPriors(1);
                viewer.OpenPriors(2);
                viewer.OpenPriors(3);
                ExecutedSteps++;

                //step7  Check email study icon in all comparison study control panels. 
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_ExamsIconButton));
                Thread.Sleep(4000);
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + ":nth-of-type(2)  " + BluRingViewer.div_StudypanelMoreButton));
                BasePage.wait.Until<bool>(e => e.FindElement(By.CssSelector(BluRingViewer.div_overlayPanel)).Displayed);
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + ":nth-of-type(3)  " + BluRingViewer.div_StudypanelMoreButton));
                BasePage.wait.Until<bool>(e => e.FindElement(By.CssSelector(BluRingViewer.div_overlayPanel)).Displayed);
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + ":nth-of-type(4)  " + BluRingViewer.div_StudypanelMoreButton));
                BasePage.wait.Until<bool>(e => e.FindElement(By.CssSelector(BluRingViewer.div_overlayPanel)).Displayed);
                bool step7_1 = viewer.GetElement("cssselector", BluRingViewer.div_overlay + "0 " + BluRingViewer.StudypanelEmailStudy).GetAttribute("class").Contains("disabled");
                bool step7_2 = viewer.GetElement("cssselector", BluRingViewer.div_overlay + "1 " + BluRingViewer.StudypanelEmailStudy).GetAttribute("class").Contains("disabled");
                bool step7_3 = viewer.GetElement("cssselector", BluRingViewer.div_overlay + "2 " + BluRingViewer.StudypanelEmailStudy).GetAttribute("class").Contains("disabled");
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
        /// Administrator/User send a study to a Guest by Email without PIN enabled.. 
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_164358(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            Maintenance maintenance = null;

            DateTime now = DateTime.Now;
            string t1 = now.ToString();
            string[] st = t1.Split(' ');
            bool isPinDisabled = false;
            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String WarningMessage = "If you have been provided a PIN to access the study, please type it in the field below and click OK. If you have been provided no PIN, click OK. The study may contain protected health information. If you have received the study notification email in error, please exit without proceeding.";

                //Pre-conditions
                //Configure "E-mail Notification" in config tool
                servicetool.LaunchServiceTool();
                servicetool.SetEmailNotification(Config.AdminEmail, SMTPHost: Config.SMTPServer, port: Config.SMTPport);

                //Enable "Email Study" in config tool 
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                if (!wpfobject.IsCheckBoxSelected(ServiceTool.EnableFeatures.Name.EnableEmailStudy, 1))
                {
                    servicetool.ModifyEnableFeatures();
                    wpfobject.WaitTillLoad();
                    wpfobject.SelectCheckBox(ServiceTool.EnableFeatures.Name.EnableEmailStudy, 1);
                    wpfobject.WaitTillLoad();
                    servicetool.wpfobject.ClickOkPopUp();
                    servicetool.ApplyEnableFeatures();
                    wpfobject.WaitTillLoad();
                    servicetool.wpfobject.ClickOkPopUp();
                }

                // In Email study tab, Unchecked box: Enable PIN system to Non - Registerd User. Leave other selections as it
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                ITabPage subtab = wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.EmailStudy);               
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();

                //Create User
                String User = "U1" + new Random().Next(10000);
                login.LoginIConnect(adminUserName, adminPassword);
                var usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(User, Config.adminGroupName, Config.adminRoleName);

                //Enable Email Study in Domain Management page
                login.Navigate("DomainManagement");
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                domain.SetCheckBoxInEditDomain("emailstudy", 0);
                domain.ClickSaveEditDomain();

                //Enable Email Study in Role Management page
                login.Navigate("RoleManagement");
                role.SelectDomainfromDropDown(Config.adminGroupName);
                role.SearchRole(Config.adminRoleName);
                role.SelectRole(Config.adminRoleName);
                role.ClickEditRole();
                role.SetCheckboxInEditRole("email", 0);
                role.ClickSaveEditRole();
                login.Logout();

                Dictionary<string, string> downloadedMail = new Dictionary<string, string>();
                EmailUtils GuestMail = new EmailUtils() { EmailId = Config.CustomUser1Email, Password = Config.CustomUserEmailPassword };

                //Step1  Login to iCA as Administrator/User and load any study into the Universal Viewer from studies tab.
                login.LoginIConnect(User, User);
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Patient ID", PatientID);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel)).Count == 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step2 - Click on email study icon from study panel 
                viewer.ClickElement(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_emailstudy)));
                viewer.WaitTillEmailWindowAppears();
                if (viewer.ValidateEmailStudyDialogue(false))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step3 - Enter a valid email address, Name and Reason. Select SEND button                
                GuestMail.MarkAllMailAsRead("INBOX");
                viewer.EmailStudy(Config.CustomUser1Email, "Testing", "Testing", isOpenEmailWindow: false);
                bool step3 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_pinWindow)).Count == 0;
                if (step3)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step4 - Click on cancel button to close the Email study dialogue.
                viewer.ClickElement(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_cancelEmail)));
                viewer.WaitTillEmailWindowDisAppears();
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_emailWindow)).Count == 0)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                viewer.CloseBluRingViewer();
                login.Logout();

                //Step5 - Go to Maintenance tab and then select Audit tab. Check for Email Study To Guest entry.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                maintenance.Navigate("Audit");
                PageLoadWait.WaitForFrameLoad(20);
                maintenance.SearchInAuditTab(User, EID: "Email Study To Guest");

                int count = maintenance.AuditListTable().Count;
                IList<IWebElement> AuditEvent = maintenance.AuditListTable()[count - 1].FindElements(By.CssSelector("td"));
                string Datetime = AuditEvent[3].Text;
                string date = System.DateTime.ParseExact(Datetime.ToString(), "dd-MMM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture).ToShortDateString();
                var day = System.DateTime.ParseExact(st[0], "M/d/yyyy", CultureInfo.InvariantCulture).ToShortDateString();
                AuditEvent[2].Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(maintenance.By_MessageDetailsDiv()));
                bool IsMessageDivPresent = viewer.IsElementPresent(maintenance.By_MessageDetailsDiv());
                if (AuditEvent[1].Text.Equals("Success") && date.Contains(day)
                     && AuditEvent[2].Text.Split('/')[1].Equals(Config.adminGroupName)
                    && AuditEvent[2].Text.Split('/')[2].Equals(User) && IsMessageDivPresent)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                login.CloseBrowser();

                //Step6 - Go to the destination email and select the URL
                downloadedMail = GuestMail.GetMailUsingIMAP("no-reply@merge.com", "Emailed Study", MarkAsRead: true, maxWaitTime: 3);
                if (downloadedMail.Count > 0)
                {
                    string link = GuestMail.GetEmailedStudyLink(downloadedMail);
                    LaunchEmailedStudy.LaunchStudy<BasePage>(link);
                    if (BasePage.Driver.FindElement(By.CssSelector(LaunchEmailedStudy.span_messageInfo)).Text.Equals(WarningMessage))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("Mail is not received");
                }

                //Step7 - Click on "OK" button
                viewer.ClickElement(BasePage.Driver.FindElement(By.CssSelector(LaunchEmailedStudy.input_OK_Launchstudy)));
                WebDriverWait wait1 = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 500));
                wait1.Until(ExpectedConditions.FrameToBeAvailableAndSwitchToIt("GuestHomeFrame"));
                Thread.Sleep(15000);

                //wait for viewport to load             
                BluRingViewer.WaitforViewports();

                //wait for thumbnails to load
                BluRingViewer.WaitforThumbnails();
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel)).Count == 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step8 - Logon iConnect Access.  Select Maintenance tab and then select Audit tab. Check for Email Study To Guest / Guest Review Emailed study entry.
                login.CreateNewSesion();
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                maintenance.Navigate("Audit");
                maintenance.SelectEventID("Email Study To Guest", 0);
                PageLoadWait.WaitForFrameLoad(20);
                maintenance.Btn_Search().Click();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                count = maintenance.AuditListTable().Count;
                AuditEvent = maintenance.AuditListTable()[count - 1].FindElements(By.CssSelector("td"));
                Datetime = AuditEvent[3].Text;
                date = System.DateTime.ParseExact(Datetime.ToString(), "dd-MMM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture).ToShortDateString();
                day = System.DateTime.ParseExact(st[0], "M/d/yyyy", CultureInfo.InvariantCulture).ToShortDateString();
                AuditEvent[2].Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(maintenance.By_MessageDetailsDiv()));
                bool IsMessageDivAvailable = viewer.IsElementPresent(maintenance.By_MessageDetailsDiv());
                string textAreaContent = BasePage.Driver.FindElement(By.CssSelector(Maintenance.textarea_auditActiveParticipantDetail)).GetAttribute("value");
                bool isEmailIDAvailable = textAreaContent.Contains(Config.CustomUser1Email);
                if (AuditEvent[1].Text.Equals("Success") && date.Contains(day)
                     && AuditEvent[2].Text.Split('/')[1].Equals("Guest")
                     && IsMessageDivAvailable && isEmailIDAvailable)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

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
                if (isPinDisabled)
                {
                    servicetool.LaunchServiceTool();
                    servicetool.NavigateToEnableFeatures();
                    wpfobject.WaitTillLoad();
                    servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                    wpfobject.WaitTillLoad();
                    servicetool.ModifyEnableFeatures();
                    wpfobject.WaitTillLoad();
                    ITabPage subtab = wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.EmailStudy);                    
                    servicetool.ApplyEnableFeatures();
                    wpfobject.WaitTillLoad();
                    servicetool.RestartIISandWindowsServices();
                    wpfobject.WaitTillLoad();
                    servicetool.CloseServiceTool();
                    servicetool.RestartService();
                }
            }
        }

        /// <summary>
        ///  Email Study Validations
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_164360(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;            

            DateTime now = DateTime.Now;
            string t1 = now.ToString();
            string[] st = t1.Split(' ');            
            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String[] ErrorMessages = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "MessageList")).Split(':');               

                //Pre-conditions
                //Configure "E-mail Notification" in config tool
                servicetool.LaunchServiceTool();
                servicetool.SetEmailNotification(Config.AdminEmail, SMTPHost: Config.SMTPServer, port: Config.SMTPport);

                //Enable "Email Study" in config tool 
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                if (!wpfobject.IsCheckBoxSelected(ServiceTool.EnableFeatures.Name.EnableEmailStudy, 1))
                {
                    servicetool.ModifyEnableFeatures();
                    wpfobject.WaitTillLoad();
                    wpfobject.SelectCheckBox(ServiceTool.EnableFeatures.Name.EnableEmailStudy, 1);
                    wpfobject.WaitTillLoad();
                    servicetool.wpfobject.ClickOkPopUp();
                    servicetool.ApplyEnableFeatures();
                    wpfobject.WaitTillLoad();
                    servicetool.wpfobject.ClickOkPopUp();
                }

                // From the Email Study tab, check for: Enable Message Page is selected, Logo Path: WebAccessLoginLogo.png, Enable PIN system to Non-Registed User is selected,  PIN Character Set: Mixed, PIN Length: 6 and Is Case Sensitive is selected
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                ITabPage subtab = wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                
                //PIN Character Set: Mixed, PIN Length: 6 and Is Case Sensitive is selected                              
                ComboBox comboBox = WpfObjects._mainWindow.Get<ComboBox>(SearchCriteria.ByClassName("ComboBox"));
                comboBox.Select("Mixed");
                TextBox pinsize = wpfobject.GetUIItem<ITabPage, TextBox>(subtab, "AutoSelectTextBox", 0, "1");
                pinsize.Text = "6";
                CheckBox EnableCaseSensitive = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(subtab, "Is Case Sensitive", 1);
                if (!EnableCaseSensitive.Checked)
                    EnableCaseSensitive.Click();

                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();

                //Enable Email Study in Domain Management page
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("DomainManagement");
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                domain.SetCheckBoxInEditDomain("emailstudy", 0);
                domain.ClickSaveEditDomain();

                //Enable Email Study in Role Management page
                login.Navigate("RoleManagement");
                role.SelectDomainfromDropDown(Config.adminGroupName);
                role.SearchRole(Config.adminRoleName);
                role.SelectRole(Config.adminRoleName);
                role.ClickEditRole();
                role.SetCheckboxInEditRole("email", 0);
                role.ClickSaveEditRole();
                login.Logout();

                //Step1  Login to iCA as Administrator and select any study which has many priors (say as 10 priors)from studies tab and click on 'Universal' button
                login.LoginIConnect(adminUserName, adminPassword);
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Patient ID", PatientID);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel)).Count == 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step2 - Check for Email Study icon is displayed In primary study control panel
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_emailstudy)).Count == 1 &&
                    viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_emailstudy)))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step3 - Click on email study icon from study panel control .
                viewer.ClickElement(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_emailstudy)));
                viewer.WaitTillEmailWindowAppears();               
                bool step3_1 = viewer.ValidateEmailStudyDialogue(true);
                bool step3_2 = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.span_modalityDropdownCurrentValue)).Text == "All";
                bool step3_3 = viewer.getAttachedStudiesCount() == 1;
                if (step3_1 && step3_2 && step3_3)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step4 - Verify that the user is able to click anywhere in the universal viewer, global tool bar and exam list.                
                bool step4_1 = false;
                bool step4_2 = false;
                try
                {
                    IWebElement showhide = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_ShowHideTool);
                    showhide.Click();
                    if (BasePage.SBrowserName.ToLower().Contains("explorer"))
                    {
                        if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ShowHideDropdown)).Count == 0)
                            step4_1 = true;
                    }

                }
                catch (Exception e) { step4_1 = true; }
                try
                {
                    var priorsList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                    priorsList[1].Click();
                    if (BasePage.SBrowserName.ToLower().Contains("explorer"))
                    {
                        if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel)).Count == 1)
                            step4_2 = true;
                    }
                }
                catch (Exception e) { step4_2 = true; }

                if (step4_1 && step4_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step5 - Ensure that the primary and related studies for the patient has the below information in the Email Study panel; DD - MON - YYYY HH: MM: SS AM/ PM ACC#:(Accession number) Modality list Study Description
                String[] StudyDate = new String[] { "01-Jan-1981", "01-Jan-1981", "01-Jan-1981", "01-Jan-1981", "01-Jan-1981", "01-Jan-1981" };
                String[] StudyTime = new String[] { "12:00:00 AM", "12:00:00 AM", "12:00:00 AM", "12:00:00 AM", "12:00:00 AM", "12:00:00 AM" };
                String[] Accession = new String[] { "HOM_13ICCA3", "HOM_13ICCA4", "HOM_13ICCA5", "HOM_13ICCA1", "HOM_13ICCA2", "HOM_13ICCA6" };
                String[] Modality = new String[] { "MR", "MR", "MR", "MR", "MR", "MR" };
                String[] Description = new String[] { "582/NE", "582/NE", "582/NE", "582/NE", "582/NE", "582/NE" };

                IList<String> studyDateInEmailWindow = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_emailStudyDate)).Select<IWebElement, String>
                                                            (studyDate => studyDate.GetAttribute("innerHTML")).ToList();
                IList<String> studyTimeInEmailWindow = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_emailStudyTime)).Select<IWebElement, String>
                                                            (studyDate => studyDate.GetAttribute("innerHTML")).ToList();
                IList<String> modalityInEmailWindow = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_emailModality)).Select<IWebElement, String>
                                                           (studyDate => studyDate.GetAttribute("innerHTML")).ToList();
                IList<String> descriptionInEmailWindow = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_emailDescription)).Select<IWebElement, String>
                                                           (studyDate => studyDate.GetAttribute("innerHTML")).ToList();
                IList<String> accessionInEmailWindow = BasePage.Driver.FindElements(By.CssSelector("div.emailrelatedstudylist")).Select<IWebElement, String>
                                                            (pr => viewer.GetAccessionFromEmailStudy(pr.FindElement(By.CssSelector(BluRingViewer.div_emailAcession)))).ToList();
                if (studyDateInEmailWindow.ToArray().SequenceEqual(StudyDate) && studyTimeInEmailWindow.ToArray().SequenceEqual(StudyTime)
                        && modalityInEmailWindow.ToArray().SequenceEqual(Modality) && descriptionInEmailWindow.ToArray().SequenceEqual(Description)
                        && accessionInEmailWindow.ToArray().SequenceEqual(Accession))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step6 - Ensure that the Attached studies count should get increased when the user selects the prior studies checkboxes.
                IList<IWebElement> priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.input_priorStudiesCheckboxes));
                int studiesCountBeforeSelectPriors = viewer.getAttachedStudiesCount();
                priors[0].Click();
                priors[1].Click();
                Thread.Sleep(3000);
                if (viewer.getAttachedStudiesCount() == studiesCountBeforeSelectPriors + 2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step7 - Uncheck the selected prior study checkbox and ensure that the Attached studies count should get decreased.               
                priors[0].Click();
                priors[1].Click();
                Thread.Sleep(3000);
                if (viewer.getAttachedStudiesCount() == studiesCountBeforeSelectPriors)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step8 - Ensure that primary Study is selected as default, and the check box of the primary study is disabled always.
                IWebElement ele = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.input_primaryStudyCheckbox));
                if (ele.Selected && !ele.Enabled)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step9 - Verify that the number of selected studies count is displayed as "1" in the Attached Studies counter.
                if (viewer.getAttachedStudiesCount() == 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();
                //viewer.ClickElement(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_cancelEmail)));
                //viewer.WaitTillEmailWindowDisAppears();

                //Step10 - Keep all fields are blank and click on Send button
                viewer.EmailStudy();
                if (BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_emailErrorMessage)).Text.Trim().Equals(ErrorMessages[0]))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step11 - Enter a name in NAME field alone and click on Send button
                viewer.EmailStudy(name: "test name", isOpenEmailWindow: false);
                if (BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_emailErrorMessage)).Text.Trim().Equals(ErrorMessages[1]))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();
                viewer.ClickElement(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_cancelEmail)));
                viewer.WaitTillEmailWindowDisAppears();

                //Step12 - Enter a reason in REASON box alone and click on Send button
                viewer.EmailStudy(reason: "test reason");
                if (BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_emailErrorMessage)).Text.Trim().Equals(ErrorMessages[0]))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step13 - Enter a email address in EMAIL TO box alone and click on Send button
                viewer.EmailStudy("testing@gmail.com", isOpenEmailWindow: false);
                if (BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_emailErrorMessage)).Text.Trim().Equals(ErrorMessages[0]))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();
                viewer.ClickElement(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_cancelEmail)));
                viewer.WaitTillEmailWindowDisAppears();

                //Step14 - Enter a name and Reason without giving email address and click on Send button
                viewer.EmailStudy(name: "test name", reason: "test reason");
                if (BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_emailErrorMessage)).Text.Trim().Equals(ErrorMessages[1]))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step15 - Enter invalid email address and fill remaining details and click on Send button
                viewer.EmailStudy("invalid email", "test name", "test reason", isOpenEmailWindow: false);
                if (BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_emailErrorMessage)).Text.Trim().Equals(ErrorMessages[2]))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();
                viewer.ClickElement(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_cancelEmail)));
                viewer.WaitTillEmailWindowDisAppears();

                //Step16 - Enter valid email address alone and keep all other fields are empty
                viewer.EmailStudy("testing@gmail.com", selectAll:true);
                if (BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_emailErrorMessage)).Text.Trim().Equals(ErrorMessages[0]))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step17 - Clear all the fields and enter valid email address,name and leave reason field as empty
                viewer.EmailStudy("testing@gmail.com", "test name", isOpenEmailWindow: false);
                if (BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_emailErrorMessage)).Text.Trim().Equals(ErrorMessages[3]))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step18 - Click on 'SELECT ALL OTHER STUDIES' checkbox and verify that the number of selected studies count should be displayed in the Attached Studies counter based on the user selection
                viewer.SetCheckbox("cssselector", BluRingViewer.div_emailSelectAll);
                Thread.Sleep(3000);
                if (viewer.getAttachedStudiesCount() == 6)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step19 - Enter all fields and then click on "CANCEL" the Email study window.
                BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.input_email)).SendKeys("test@gmail.com");
                BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.input_emailName)).SendKeys("test name");
                BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.input_Notes)).SendKeys("Reason");
                viewer.ClickElement(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_cancelEmail)));
                Thread.Sleep(3000);
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_emailWindow)).Count == 0 &&
                        BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_pinWindow)).Count == 0 )
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step20 - Open Email window, Enter all fields available in the email window and select "SELECT ALL OTHER STUDIES" check box
                //Step21 - 	Click on SEND button
                Dictionary<string, string> downloadedMail = new Dictionary<string, string>();
                EmailUtils GuestMail = new EmailUtils() { EmailId = Config.CustomUser1Email, Password = Config.CustomUserEmailPassword };
                viewer.EmailStudy(Config.CustomUser1Email, "test name", "test reason", true);
                ExecutedSteps++;
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_pinWindow)).Count == 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step22 - Write down this pin number on a piece of paper.
                //Step23 - Close the PIN code dialog by clicking 'X' icon
                String pinNumber = viewer.FetchPin_BR();
                ExecutedSteps++;
                if (pinNumber != null)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step24 - Go to the destination Email and Check that the "Emailed Study" notification is received.               
                downloadedMail = GuestMail.GetMailUsingIMAP("no-reply@merge.com", "Emailed Study", MarkAsRead: true, maxWaitTime: 3);
                if (downloadedMail.Count > 0)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step25 - Click on the link available from email study to view the study.
                string link = GuestMail.GetEmailedStudyLink(downloadedMail);
                LaunchEmailedStudy.LaunchStudy<BasePage>(link);
                IList<IWebElement> pinTextBox = BasePage.Driver.FindElements(By.CssSelector(LaunchEmailedStudy.input_pin_Launchstudy));
                if (pinTextBox.Count == 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step26 - Type the Incorrect PIN number in the PIN code box.
                pinTextBox[0].SendKeys("thy214234");
                var OKButton = BasePage.Driver.FindElement(By.CssSelector(LaunchEmailedStudy.input_OK_Launchstudy));
                viewer.ClickElement(OKButton);
                Thread.Sleep(2000);
                if (BasePage.Driver.FindElement(By.CssSelector(LaunchEmailedStudy.span_pinErrorMessage)).Text.Trim().Equals(ErrorMessages[4]))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step27 - Type the PIN number that was generated from the previous steps in the PIN code box with lower case.
                Logger.Instance.InfoLog("The pin number is " + pinNumber);
                if (pinNumber.Any(char.IsUpper))
                {
                    BasePage.Driver.FindElement(By.CssSelector(LaunchEmailedStudy.input_pin_Launchstudy)).SendKeys(pinNumber.ToLower());
                }
                else
                {
                    BasePage.Driver.FindElement(By.CssSelector(LaunchEmailedStudy.input_pin_Launchstudy)).SendKeys(pinNumber.ToUpper());
                }
                viewer.ClickElement(BasePage.Driver.FindElement(By.CssSelector(LaunchEmailedStudy.input_OK_Launchstudy)));
                Thread.Sleep(2000);
                if (BasePage.Driver.FindElement(By.CssSelector(LaunchEmailedStudy.span_pinErrorMessage)).Text.Trim().Equals(ErrorMessages[4]))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step28 - Type the noted correct PIN in code box and click ok   
                LaunchEmailedStudy.LaunchStudy<BluRingViewer>(link, pinNumber);
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel)).Count == 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

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
        /// Administrator sends a study to a Guest (non-registered user) by Email
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_164354(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            String adminUserName = Config.adminUserName;
            String adminPassword = Config.adminPassword;
            DateTime now = DateTime.Now;
            string t1 = now.ToString();
            string[] st = t1.Split(' ');
            String defaultmessage = null;
            String[] path = new String[4];
            String[] originalImages = new String[2];
            DomainManagement domain = new DomainManagement();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String EmailIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");
                String NameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                String ReasonList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Reason");
                String PathList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Path");
                path = PathList.Split('#');
                String MessageList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "MessageList");
                var WarningMessage = MessageList.Split(':');
                string link = null;

                String currentDirectory = Directory.GetCurrentDirectory();
                path[0] = currentDirectory + "\\" + path[0];
                path[1] = currentDirectory + "\\" + path[1];

                originalImages[0] = "C:\\Users\\Administrator\\Desktop\\ibm-8bar-login@2x.png";
                originalImages[1] = "C:\\Users\\Administrator\\Desktop\\ica-name-login@2x.png";

                File.Copy(path[2], originalImages[0], true);
                File.Copy(path[3], originalImages[1], true);

                File.Copy(path[0], path[2], true);
                File.Copy(path[1], path[3], true);

                //setting up the service tool
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();

                // Enabling the studysharing, data transfer, and data download features                
                servicetool.EnableEmailStudy();
                wpfobject.WaitTillLoad();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                wpfobject.ClickOkPopUp();
                wpfobject.WaitTillLoad();

                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                ITabPage subtab = wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.EmailStudy); 

                // PIN character
                ComboBox comboBox2 = WpfObjects._mainWindow.Get<ComboBox>(SearchCriteria.ByClassName("ComboBox"));
                comboBox2.Select("Numeric");

                //Set pinsize to 8
                TextBox pinsize = wpfobject.GetUIItem<ITabPage, TextBox>(subtab, "AutoSelectTextBox", 0, "1");
                pinsize.Text = "8";

                // Enable case sensitive check box
                CheckBox casesenitive = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(subtab, "Is Case Sensitive", 1);
                if (!casesenitive.Checked)
                {
                    casesenitive.Click();
                }
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();

                // configuring the email notification                
                servicetool.SetEmailNotification(AdministratorEmail: Config.AdminEmail, SMTPHost: Config.IMAPServer);

                // Close the service tool
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();

                //Enabling the Email Study in DomainManagement
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("DomainManagement");
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                Thread.Sleep(3000);
                domain.SetCheckBoxInEditDomain("emailstudy", 0);
                domain.ClickSaveEditDomain();

                // Enabling the Email Study in rolemanagement
                var rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectDomainfromDropDown(Config.adminGroupName);
                rolemanagement.SearchRole(Config.adminRoleName);
                rolemanagement.SelectRole(Config.adminRoleName);
                rolemanagement.ClickEditRole();
                rolemanagement.SetPrivelage("email");
                rolemanagement.ClickSaveEditRole();
                login.Logout();

                Dictionary<string, string> downloadedMail = new Dictionary<string, string>();
                EmailUtils UserMail = new EmailUtils() { EmailId = Config.CustomUser1Email, Password = Config.CustomUserEmailPassword };

                // Step 1 - Login to iCA as Administrator then navigate to User management tab
                login.LoginIConnect(adminUserName, adminPassword);
                var usermanagement = (UserManagement)login.Navigate("UserManagement");
                if (usermanagement.WarningMsgLbl().GetAttribute("innerHTML").Equals(WarningMessage[0]))
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The Actual warning message is " + usermanagement.WarningMsgLbl().GetAttribute("innerHTML") + "The expected message is " + WarningMessage[0]);
                }

                // Step 2 - Navigate to Studies tab and load a study in Universal Viewer 
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: AccessionList, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", AccessionList);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                // Step 3 - Click on email study icon from study control panel	
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_emailstudy));
                viewer.WaitTillEmailWindowAppears(true);               
                if (viewer.ValidateEmailStudyDialogue(true))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 4 - Enter a bogus destination email.
                viewer.EmailStudy("abcd@testing.com", "testing", "testing", isOpenEmailWindow: false);
                Thread.Sleep(3000);
                if (viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_emailErrorMessage).GetAttribute("innerHTML").Equals(WarningMessage[1]))
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The Actual warming message is " + viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_emailErrorMessage).GetAttribute("innerHTML") + "The expected message is " + WarningMessage[1]);
                }

                // Step 5 & 6 - Enter a valid email addresses and Select any prior from the list and then click on "SEND" button	
                UserMail.MarkAllMailAsRead("INBOX");
                viewer.EmailStudy(Config.CustomUser1Email, "test", "Testing", isOpenEmailWindow: false);
                var pinnumber = viewer.FetchPin_BR();
                int number;
                if (int.TryParse(pinnumber, out number) && pinnumber.Length.Equals(8))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();
                viewer.CloseBluRingViewer();
                ExecutedSteps++;

                // Step 7 - Select Maintenance tab and then select Audit tab.
                Maintenance maintenance = (Maintenance)login.Navigate("Maintenance");
                maintenance.Navigate("Audit");
                PageLoadWait.WaitForFrameLoad(20);
                maintenance.SearchInAuditTab(adminUserName, EID: "Email Study To Guest");

                int count = maintenance.AuditListTable().Count;
                IList<IWebElement> AuditEvent = maintenance.AuditListTable()[count - 1].FindElements(By.CssSelector("td"));
                string Datetime = AuditEvent[3].Text;
                string date = System.DateTime.ParseExact(Datetime.ToString(), "dd-MMM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture).ToShortDateString();
                var day = System.DateTime.ParseExact(st[0], "M/d/yyyy", CultureInfo.InvariantCulture).ToShortDateString();
                viewer.ClickElement(AuditEvent[2]);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(maintenance.By_MessageDetailsDiv()));
                bool IsMessageDivPresent = viewer.IsElementPresent(maintenance.By_MessageDetailsDiv());
                if (AuditEvent[1].Text.Equals("Success") && date.Contains(day)
                     && AuditEvent[2].Text.Split('/')[1].Equals(Config.adminGroupName)
                    && AuditEvent[2].Text.Split('/')[2].Equals(adminUserName) && IsMessageDivPresent)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 8 - Go to the destination email and select the URL	
                downloadedMail = UserMail.GetMailUsingIMAP("no-reply@merge.com", "Emailed Study", MarkAsRead: true, maxWaitTime: 3);
                if (downloadedMail.Count > 0)
                {
                    link = UserMail.GetEmailedStudyLink(downloadedMail);
                    LaunchEmailedStudy.LaunchStudy<BasePage>(link);                 
                    bool step8_1 = BasePage.Driver.FindElement(By.CssSelector(LaunchEmailedStudy.span_messageInfo)).Text.Equals(WarningMessage[2]);
                    bool step8_2 = false;
                    if (BasePage.FindElementsByCss(LaunchEmailedStudy.input_pin_Launchstudy).Count != 0)
                        step8_2 = BasePage.FindElementsByCss(LaunchEmailedStudy.input_pin_Launchstudy)[0].Displayed;

                    if (step8_1 && step8_2)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("Mail is not received");
                }

                //Step9 - Verify the IBM IConnect Access Image and IBM logo
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step9_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, LaunchEmailedStudy.div_IConnectAccessImage));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step9_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, LaunchEmailedStudy.div_IBMLogo));

                if (step9_1 && step9_2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // step 10 - Type a bogus PIN number in the PIN code box.	                
                viewer.GetElement(BasePage.SelectorType.CssSelector, LaunchEmailedStudy.input_pin_Launchstudy).SendKeys("testing");
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, LaunchEmailedStudy.input_OK_Launchstudy));
                Thread.Sleep(3000);
                var wrongPingError = viewer.GetElement(BasePage.SelectorType.CssSelector, LaunchEmailedStudy.span_pinErrorMessage).GetAttribute("innerHTML");
                if (wrongPingError.Equals(WarningMessage[3]))
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The Actual warning message is " + wrongPingError + "The expected message is " + WarningMessage[3]);
                }

                // step 11 - Type the PIN number that was generated (8 digit numbers) and click ok.	
                viewer = LaunchEmailedStudy.LaunchStudy<BluRingViewer>(link, pinnumber);
                ExecutedSteps++;

                // Step 12 - Logon iConnect Access. Select Maintenance tab and then select Audit tab.
                login.CreateNewSesion();
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                maintenance.Navigate("Audit");
                maintenance.SelectEventID("Email Study To Guest", 0);
                PageLoadWait.WaitForFrameLoad(20);
                viewer.ClickElement(maintenance.Btn_Search());
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                count = maintenance.AuditListTable().Count;
                AuditEvent = maintenance.AuditListTable()[count - 1].FindElements(By.CssSelector("td"));
                Datetime = AuditEvent[3].Text;
                date = System.DateTime.ParseExact(Datetime.ToString(), "dd-MMM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture).ToShortDateString();
                day = System.DateTime.ParseExact(st[0], "M/d/yyyy", CultureInfo.InvariantCulture).ToShortDateString();
                viewer.ClickElement(AuditEvent[2]);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(maintenance.By_MessageDetailsDiv()));
                bool IsMessageDivAvailable = viewer.IsElementPresent(maintenance.By_MessageDetailsDiv());
                string textAreaContent = BasePage.Driver.FindElement(By.CssSelector(Maintenance.textarea_auditActiveParticipantDetail)).GetAttribute("value");
                bool isEmailIDAvailable = textAreaContent.Contains(Config.CustomUser1Email);
                if (AuditEvent[1].Text.Equals("Success") && date.Contains(day)
                     && AuditEvent[2].Text.Split('/')[1].Equals("Guest")
                     && IsMessageDivAvailable && isEmailIDAvailable)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Logout
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
            finally
            {
                File.Copy(originalImages[0], path[2],  true);
                File.Copy(originalImages[1], path[3],  true);
                // launch service tool
                servicetool.LaunchServiceTool();
                wpfobject.WaitTillLoad();               

                // Restart and close the service tool
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();
            }
        }

        /// <summary>
        ///  PIN generation for Email Study
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_164356(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            DateTime now = DateTime.Now;
            string t1 = now.ToString();
            string[] st = t1.Split(' ');

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String[] ErrorMessages = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "MessageList")).Split(':');

                //Pre-conditions
                //Configure "E-mail Notification" in config tool
                servicetool.LaunchServiceTool();
                servicetool.SetEmailNotification(Config.AdminEmail, SMTPHost: Config.SMTPServer, port: Config.SMTPport);

                //Enable "Email Study" in config tool 
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                if (!wpfobject.IsCheckBoxSelected(ServiceTool.EnableFeatures.Name.EnableEmailStudy, 1))
                {
                    servicetool.ModifyEnableFeatures();
                    wpfobject.WaitTillLoad();
                    wpfobject.SelectCheckBox(ServiceTool.EnableFeatures.Name.EnableEmailStudy, 1);
                    wpfobject.WaitTillLoad();
                    servicetool.wpfobject.ClickOkPopUp();
                    servicetool.ApplyEnableFeatures();
                    wpfobject.WaitTillLoad();
                    servicetool.wpfobject.ClickOkPopUp();
                }

                // From the Email Study tab, check for: Enable Message Page is selected, Logo Path: WebAccessLoginLogo.png, Enable PIN system to Non-Registed User is selected,  PIN Character Set: Mixed, PIN Length: 6 and Is Case Sensitive is selected
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                ITabPage subtab = wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.EmailStudy);                

                //PIN Character Set: Mixed, PIN Length: 6 and Is Case Sensitive is selected                              
                ComboBox comboBox = WpfObjects._mainWindow.Get<ComboBox>(SearchCriteria.ByClassName("ComboBox"));
                comboBox.Select("Mixed");
                TextBox pinsize = wpfobject.GetUIItem<ITabPage, TextBox>(subtab, "AutoSelectTextBox", 0, "1");
                pinsize.Text = "6";
                CheckBox EnableCaseSensitive = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(subtab, "Is Case Sensitive", 1);
                if (!EnableCaseSensitive.Checked)
                    EnableCaseSensitive.Click();

                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();

                //Enable Email Study in Domain Management page
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("DomainManagement");
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                domain.SetCheckBoxInEditDomain("emailstudy", 0);
                domain.ClickSaveEditDomain();

                //Enable Email Study in Role Management page
                login.Navigate("RoleManagement");
                role.SelectDomainfromDropDown(Config.adminGroupName);
                role.SearchRole(Config.adminRoleName);
                role.SelectRole(Config.adminRoleName);
                role.ClickEditRole();
                role.SetCheckboxInEditRole("email", 0);
                role.ClickSaveEditRole();
                login.Logout();

                //Step1  Login to iCA as Administrator and select any study which has many priors (say as 10 priors)from studies tab and click on 'Universal' button
                login.LoginIConnect(adminUserName, adminPassword);
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Patient ID", PatientID);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel)).Count == 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step2 - Click on email study icon from study panel
                viewer.ClickElement(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_emailstudy)));
                viewer.WaitTillEmailWindowAppears();
                Thread.Sleep(3000);
                if (viewer.ValidateEmailStudyDialogue())
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step3 - Enter all fields in email window and select "SEND" button.
                Dictionary<string, string> downloadedMail = new Dictionary<string, string>();
                EmailUtils GuestMail = new EmailUtils() { EmailId = Config.CustomUser1Email, Password = Config.CustomUserEmailPassword };
                GuestMail.MarkAllMailAsRead("INBOX");
                viewer.EmailStudy(Config.CustomUser1Email, "test name", "test reason");
                String pinNumber = viewer.FetchPin_BR();
                if ((pinNumber.Any(char.IsUpper) || pinNumber.Any(char.IsLower)) && pinNumber.Any(char.IsDigit) && pinNumber.Length == 6)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("Pin number - Mixed mode : " + pinNumber);
                }

                //Step4 - Go to the destination email and select the URL
                downloadedMail = GuestMail.GetMailUsingIMAP("no-reply@merge.com", "Emailed Study", MarkAsRead: true, maxWaitTime: 3);
                string link = GuestMail.GetEmailedStudyLink(downloadedMail);
                LaunchEmailedStudy.LaunchStudy<BasePage>(link);
                IList<IWebElement> pinTextBox = BasePage.Driver.FindElements(By.CssSelector(LaunchEmailedStudy.input_pin_Launchstudy));
                if (pinTextBox.Count == 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step5 - Type the Incorrect PIN number in the PIN code box.
                pinTextBox[0].SendKeys("thy214234");
                var OKButton = BasePage.Driver.FindElement(By.CssSelector(LaunchEmailedStudy.input_OK_Launchstudy));
                viewer.ClickElement(OKButton);
                Thread.Sleep(2000);
                if (BasePage.Driver.FindElement(By.CssSelector(LaunchEmailedStudy.span_pinErrorMessage)).Text.Trim().Equals(ErrorMessages[0]))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step6 - Type the noted correct PIN in code box and click ok
                LaunchEmailedStudy.LaunchStudy<BluRingViewer>(link, pinNumber);                
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel)).Count == 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step7 - Close the viewer.
                viewer.CloseBrowser();
                ExecutedSteps++;

                //Step8 - Launch iCA service tool -->Email study sub tab -->PIN character set.
                //   Select Numeric option from PIN Character Set,
                //   Set "PIN Length" value as 7
                //   "Is Case Sensitive" is selected.
                //   Restart IIS
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                subtab = wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.EmailStudy);

                //PIN Character Set: Numeric, PIN Length: 7 and Is Case Sensitive is selected                              
                comboBox = WpfObjects._mainWindow.Get<ComboBox>(SearchCriteria.ByClassName("ComboBox"));
                comboBox.Select("Numeric");
                pinsize = wpfobject.GetUIItem<ITabPage, TextBox>(subtab, "AutoSelectTextBox", 0, "1");
                pinsize.Text = "7";
                EnableCaseSensitive = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(subtab, "Is Case Sensitive", 1);
                if (!EnableCaseSensitive.Checked)
                    EnableCaseSensitive.Click();

                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();
                ExecutedSteps++;

                //Step9 - Login to WebAccess site with any privileged user
                //Step10 - Select a study from Studies tab and click on 'Universal' button
                login.InvokeBrowser(Config.BrowserType);
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Patient ID", PatientID);
                viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel)).Count == 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step11 - Click on email study icon from study panel
                viewer.ClickElement(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_emailstudy)));
                viewer.WaitTillEmailWindowAppears();
                Thread.Sleep(3000);
                if (viewer.ValidateEmailStudyDialogue())
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step12 - Enter all fields in email window and select "SEND" button              
                GuestMail.MarkAllMailAsRead("INBOX");
                viewer.EmailStudy(Config.CustomUser1Email, "test name", "test reason");
                String pinNumber1 = viewer.FetchPin_BR();
                if (!pinNumber1.Any(char.IsUpper) && !pinNumber1.Any(char.IsLower) && pinNumber1.Any(char.IsDigit) && pinNumber1.Length == 7)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("Pin number - Numeric mode : " + pinNumber1);
                }

                //Step13 - Go to the destination email and select the URL                
                downloadedMail = GuestMail.GetMailUsingIMAP("no-reply@merge.com", "Emailed Study", MarkAsRead: true, maxWaitTime: 3);
                string link1 = GuestMail.GetEmailedStudyLink(downloadedMail);
                LaunchEmailedStudy.LaunchStudy<BasePage>(link1);
                pinTextBox = BasePage.Driver.FindElements(By.CssSelector(LaunchEmailedStudy.input_pin_Launchstudy));
                if (pinTextBox.Count == 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step14 - Type the Incorrect PIN number in the PIN code box.
                pinTextBox[0].SendKeys("thy214234");
                OKButton = BasePage.Driver.FindElement(By.CssSelector(LaunchEmailedStudy.input_OK_Launchstudy));
                viewer.ClickElement(OKButton);
                Thread.Sleep(2000);
                if (BasePage.Driver.FindElement(By.CssSelector(LaunchEmailedStudy.span_pinErrorMessage)).Text.Trim().Equals(ErrorMessages[0]))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step15 - Type the noted correct PIN in code box and click ok.  
                LaunchEmailedStudy.LaunchStudy<BluRingViewer>(link1, pinNumber1);               
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel)).Count == 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step16 - Close the viewer.
                viewer.CloseBrowser();
                ExecutedSteps++;

                //Step17 - Launch iCA service tool under-->Email study -->PIN character set.
                //      Select Alphabetic option from "PIN Charactor Set"
                //      Change PIN Length value as 8 and
                //     "Is Case Sensitive" is selected.
                //      Restart IIS
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                subtab = wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.EmailStudy);

                //PIN Character Set: Numeric, PIN Length: 7 and Is Case Sensitive is selected                              
                comboBox = WpfObjects._mainWindow.Get<ComboBox>(SearchCriteria.ByClassName("ComboBox"));
                comboBox.Select("Alphabetic");
                pinsize = wpfobject.GetUIItem<ITabPage, TextBox>(subtab, "AutoSelectTextBox", 0, "1");
                pinsize.Text = "8";
                EnableCaseSensitive = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(subtab, "Is Case Sensitive", 1);
                if (!EnableCaseSensitive.Checked)
                    EnableCaseSensitive.Click();

                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();
                ExecutedSteps++;

                //Step18 - Login to WebAccess site with any privileged user
                //Step19 - Select a study from Studies tab and click on 'Universal' button
                login.InvokeBrowser(Config.BrowserType);
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Patient ID", PatientID);
                viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel)).Count == 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step20 - Click on email study icon from study panel
                viewer.ClickElement(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_emailstudy)));
                viewer.WaitTillEmailWindowAppears();
                Thread.Sleep(3000);
                if (viewer.ValidateEmailStudyDialogue())
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step21 - Enter all fields in email window and select "SEND" button               
                GuestMail.MarkAllMailAsRead("INBOX");
                viewer.EmailStudy(Config.CustomUser1Email, "test name", "test reason");
                String pinNumber2 = viewer.FetchPin_BR();
                if (pinNumber2.Any(char.IsUpper) && pinNumber2.Any(char.IsLower) && !pinNumber2.Any(char.IsDigit) && pinNumber2.Length == 8)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("Pin number - Alphabet mode : " + pinNumber2);
                }

                //Step22 - Go to the destination email and select the URL                
                downloadedMail = GuestMail.GetMailUsingIMAP("no-reply@merge.com", "Emailed Study", MarkAsRead: true, maxWaitTime: 3);
                string link2 = GuestMail.GetEmailedStudyLink(downloadedMail);
                LaunchEmailedStudy.LaunchStudy<BasePage>(link2);
                pinTextBox = BasePage.Driver.FindElements(By.CssSelector(LaunchEmailedStudy.input_pin_Launchstudy));
                if (pinTextBox.Count == 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step23 - Type the Incorrect PIN number in the PIN code box.
                pinTextBox[0].SendKeys("thy214234");
                OKButton = BasePage.Driver.FindElement(By.CssSelector(LaunchEmailedStudy.input_OK_Launchstudy));
                viewer.ClickElement(OKButton);
                Thread.Sleep(2000);
                if (BasePage.Driver.FindElement(By.CssSelector(LaunchEmailedStudy.span_pinErrorMessage)).Text.Trim().Equals(ErrorMessages[0]))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step24 - Type the noted correct PIN in code box and click ok.
                LaunchEmailedStudy.LaunchStudy<BluRingViewer>(link2, pinNumber2);                
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel)).Count == 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step25 - Close the viewer.
                viewer.CloseBrowser();
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
        /// Universal Viewer: User able to send study to guest user by launching shared study
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_164500(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            String adminUserName = Config.adminUserName;
            String adminPassword = Config.adminPassword;
            DomainManagement domain = new DomainManagement();
            UserManagement usermanagement = new UserManagement();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String EmailIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");
                String NameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                String ReasonList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Reason");
                String domain1 = "Domain1_" + new Random().Next(1, 1000);
                String role1 = "Role1_" + new Random().Next(1, 1000);
                String role2 = "Role2_" + new Random().Next(1, 1000);
                String rad1 = Config.newUserName + new Random().Next(1, 1000);
                String rad2 = Config.newUserName + "2" + new Random().Next(1, 1000);

                // configuring the email notification
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("E-mail Notification");
                servicetool.NavigateSubTab("General");
                servicetool.SetEmailNotification(AdministratorEmail: Config.AdminEmail, SMTPHost: Config.IMAPServer);
                wpfobject.WaitTillLoad();
                wpfobject.ClickOkPopUp();
                wpfobject.WaitTillLoad();

                // Enabling "Email Study" and "study sharing"
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableEmailStudy();
                wpfobject.WaitTillLoad();
                servicetool.EnableStudySharing();
                wpfobject.WaitTillLoad();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                wpfobject.ClickOkPopUp();
                wpfobject.WaitTillLoad();

                // Restart and close the service tool
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();

                //// Creating a domain with email study and grant access enabled
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("DomainManagement");
                domain.CreateDomain(domain1, role1, datasources: null);
                domain.SetCheckBoxInEditDomain("grant", 0);
                domain.SetCheckBoxInEditDomain("emailstudy", 0);
                domain.SetCheckBoxInEditDomain("universalviewer", 0);
                domain.ClickSaveNewDomain();

                // Enabling the grant access in rolemanagement
                var rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectDomainfromDropDown(domain1);
                rolemanagement.SearchRole(role1);
                rolemanagement.SelectRole(role1);
                rolemanagement.ClickEditRole();
                rolemanagement.ClickElement(rolemanagement.GrantAccessRadioBtn_Anyone());
                rolemanagement.SetPrivelage("emailstudy");
                rolemanagement.SetCheckboxInEditRole("universalviewer", 0);
                rolemanagement.ClickSaveEditRole();
                rolemanagement.CreateRole(domain1, role2, roletype: "email");

                // Create a new user for the above domain
                login.Navigate("UserManagement");
                usermanagement.CreateUser(rad1, domain1, role1);
                usermanagement.CreateUser(rad2, domain1, role2);
                login.Logout();

                // Step 1 & 2 & 3- Login in to iCA with user1, select any study and grant access the study to user2.
                login.LoginIConnect(rad1, rad1);
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: AccessionList, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", AccessionList);
                studies.GrantAccessToUsers(domain1, rad2);
                login.Logout();
                login.LoginIConnect(rad2, rad2);
                var inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(AccessionNo: AccessionList, Study_Performed_Period: "All Dates", Study_Received_Period: "All Dates");
                String Fromuser, studyStatus;
                inbounds.GetMatchingRow("Accession", AccessionList).TryGetValue("From User(s)", out Fromuser);
                inbounds.GetMatchingRow("Accession", AccessionList).TryGetValue("Status", out studyStatus);
                if (Fromuser.Contains(rad1) && studyStatus.Equals("Shared"))
                    result.steps[ExecutedSteps += 3].StepPass();
                else
                {
                    result.steps[ExecutedSteps += 3].StepFail();
                    Logger.Instance.InfoLog("The Expected study received from user is " + rad1 + "The Actual study received from user is " + Fromuser);
                    Logger.Instance.InfoLog("The Expected status is 'Shared' and The Actual study received from user is " + studyStatus);
                }

                // Step 4 - Select the shared study and click on the 'Universal' button.	
                inbounds.SelectStudy("Accession", AccessionList);
                BluRingViewer viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                // Step 5 - Click on email study icon from the study control panel and enter the valid mail address, name and reason and Click on "SEND" button
                Dictionary<string, string> downloadedMail = new Dictionary<string, string>();
                EmailUtils Useremail = new EmailUtils() { EmailId = Config.CustomUser1Email, Password = Config.CustomUserEmailPassword };
                Useremail.MarkAllMailAsRead("INBOX");
                viewer.EmailStudy(Config.CustomUser1Email, "test", "Testing", isOpenEmailWindow: true);
                var pinnumber = viewer.FetchPin_BR();
                if (!string.IsNullOrEmpty(pinnumber))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 6 - Go to the destination Email and Check that the "Emailed Study" notification is received.	
                downloadedMail = Useremail.GetMailUsingIMAP("no-reply@merge.com", "Emailed Study", MarkAsRead: true, maxWaitTime: 3);
                string emaillink = Useremail.GetEmailedStudyLink(downloadedMail);
                if (downloadedMail.Count > 0 && downloadedMail["Body"].ToLower().Contains(rad2))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();
                viewer.CloseBluRingViewer();

                // Step 7 - Click on the link available from email study to view the study. Enter pin number and click on OK.
                viewer = LaunchEmailedStudy.LaunchStudy<BluRingViewer>(emaillink, pinnumber);
                ExecutedSteps++;

                // Step 8 - Select active series view port and right click on mouse button to open the floating tool box.	
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_Toolbox)))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 9 - Select any tool from the floating toolbox(Say as Pan) and apply pan tool in the active series viewport
                viewer.SelectViewerTool(isOpenToolsPOPup: false);
                viewer.ApplyTool_Pan();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step9 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step9)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Logout
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Emailing a study with related studies using modality filter drop down
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_164575(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);                
                BluRingViewer viewer = new BluRingViewer();
                IntegratorStudies intgtrStudies = new IntegratorStudies();
                PatientsStudy ps = new PatientsStudy();

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");

                //Pre-conditions
                //Configure "E-mail Notification" in config tool
                servicetool.LaunchServiceTool();
                servicetool.SetEmailNotification(Config.AdminEmail, SMTPHost: Config.SMTPServer, port: Config.SMTPport);

                //Enable "Email Study" in config tool 
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                if (!wpfobject.IsCheckBoxSelected(ServiceTool.EnableFeatures.Name.EnableEmailStudy, 1))
                {
                    servicetool.ModifyEnableFeatures();
                    wpfobject.WaitTillLoad();
                    wpfobject.SelectCheckBox(ServiceTool.EnableFeatures.Name.EnableEmailStudy, 1);
                    wpfobject.WaitTillLoad();
                    servicetool.wpfobject.ClickOkPopUp();
                    servicetool.ApplyEnableFeatures();
                    wpfobject.WaitTillLoad();
                    servicetool.wpfobject.ClickOkPopUp();
                }

                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                ITabPage subtab = wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.EmailStudy);       

                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();

                //Enable Email Study in Domain Management page
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("DomainManagement");
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                domain.SetCheckBoxInEditDomain("emailstudy", 0);
                domain.ClickSaveEditDomain();

                //Enable Email Study in Role Management page
                login.Navigate("RoleManagement");
                role.SelectDomainfromDropDown(Config.adminGroupName);
                role.SearchRole(Config.adminRoleName);
                role.SelectRole(Config.adminRoleName);
                role.ClickEditRole();
                role.SetCheckboxInEditRole("email", 0);
                role.ClickSaveEditRole();
                login.Logout();

                //Step1 - Logon to iCA as Administrator and load Multi modality study[DX 1,15 Priors ] which has more priors into the Universal Viewer.
                login.LoginIConnect(adminUserName, adminPassword);
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Modality", "CR");
                viewer = BluRingViewer.LaunchBluRingViewer();
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel)).Count == 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step2 - Ensure all related studies are listed in the exam list.
                var priorsList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                if (priorsList.Count == 16)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step3 - Select the primary study panel and click on email study icon from primary study control panel               
                viewer.ClickElement(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_emailstudy)));
                viewer.WaitTillEmailWindowAppears();
                if (viewer.ValidateEmailStudyDialogue(true))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step4 - Ensure "All" option is selected by default in the modality dropdown list.
                String defaultValue = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.span_modalityDropdownCurrentValue)).Text;
                if (defaultValue.Equals("All"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("Expected Value : 'All'  - Actual Value : " + defaultValue);
                }

                //Step5 - Click on the modality dropdown list and verify that modalities[Say as CT , CR , DX] are displayed for the loaded study.
                String[] modalityList = new String[] { "All", "CR", "CT", "DX" };
                IWebElement ele = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_modalityFilter);
                ele.Click();
                BasePage.wait.Until<Boolean>(d => d.FindElement(By.CssSelector(BluRingViewer.div_modalityFilterPopup)) != null);
                String[] modalitiesInEmailStudyWindow = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_text)).Select<IWebElement, String>
                                        (element => element.GetAttribute("innerHTML")).ToArray();
                if (modalityList.SequenceEqual(modalitiesInEmailStudyWindow))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step6 - Click on any modality [Say as CR] from the drop down list.
                var options = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_text));
                foreach (var option in options)
                {
                    if (option.GetAttribute("innerHTML").Equals("CR"))
                    {
                        option.Click();
                        break;
                    }
                }
                String currentValue = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.span_modalityDropdownCurrentValue)).Text;
                if (currentValue.Equals("CR"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("Expected Value : 'CR'  - Actual Value : " + currentValue);
                }

                //Step7 - Ensure related prior studies are displayed for selected modality under SELECT ALL OTHER STUDIES
                bool isModalityCorrect = false;
                IList<IWebElement> priorsListInEmailWindow = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_emailRelativeStudiesModalities));
                foreach (IWebElement prior in priorsListInEmailWindow)
                {
                    if (!prior.GetAttribute("innerHTML").Contains("CR"))
                    {
                        isModalityCorrect = false;
                        Logger.Instance.InfoLog("Modality is incorrect in filtered priors");
                        break;
                    }
                    else
                    {
                        isModalityCorrect = true;
                    }
                }
                if (isModalityCorrect)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step8 - Select all related studies one by one and verify that the number of selected studies count is displayed in the Attached Studies counter.
                //  viewer.ClickElement(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_emailSelectAll)));
                var priorsCheckbox = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.input_priorsCheckbox));
                foreach(IWebElement prior in priorsCheckbox)
                {
                    viewer.ScrollIntoView(prior);
                    if (!prior.Selected)
                        prior.Click();
                }                    
                Thread.Sleep(5000);
                int count = viewer.getAttachedStudiesCount();
                if (count == 13)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("Expected Studies Count is 13 but actual count is " + count);
                }

                //Step9 - Enter the valid email address, name and reasons and click on "SEND" button.
                Dictionary<string, string> downloadedMail = new Dictionary<string, string>();
                EmailUtils GuestMail = new EmailUtils() { EmailId = Config.CustomUser1Email, Password = Config.CustomUserEmailPassword };
                GuestMail.MarkAllMailAsRead("INBOX");
                viewer.EmailStudy(Config.CustomUser1Email, "test name", "test reason");
                String pinNumber = viewer.FetchPin_BR();
                if (!String.IsNullOrEmpty(pinNumber))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step10 - Go to the destination Email and Check that the "Emailed Study" notification is received
                if (GuestMail.GetUnreadMailCount() == 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step11 - Click on the link available from email study to view the study. Enter pin number and click on OK
                downloadedMail = GuestMail.GetMailUsingIMAP("no-reply@merge.com", "Emailed Study", MarkAsRead: true, maxWaitTime: 3);
                string link = GuestMail.GetEmailedStudyLink(downloadedMail);
                LaunchEmailedStudy.LaunchStudy<BluRingViewer>(link, pinNumber);
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel)).Count == 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step12 - Verify that all emailed priors studies are listed based on the modality(CR) selection in the exam list.
                IList<IWebElement> priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                bool isModalityExpected = false;
                foreach (IWebElement prior in priors)
                {
                    String priormodality = prior.FindElement(By.CssSelector(BluRingViewer.div_priorModality)).GetAttribute("innerHTML");
                    Logger.Instance.InfoLog("Modality : -  " + priormodality);
                    if (!priormodality.Contains("CR"))
                    {
                        isModalityExpected = false;
                    }
                    else
                    {
                        isModalityExpected = true;
                    }
                }
                if (isModalityExpected)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();


                //Step13 - Load all prior studies by single click on each prior studies from the exam list.
                bool isPriorOpened = false;
                int priorIndex = 0;
                foreach (IWebElement prior in priors)
                {
                    int StudyPanel = viewer.AllstudyPanel().Count;
                    float studyPanelLeft = float.Parse(viewer.AllstudyPanel()[priorIndex].GetCssValue("left").Replace("px", ""));
                    viewer.ScrollIntoView(prior);
                    prior.Click();
                    Thread.Sleep(2000);
                    BluRingViewer.WaitforThumbnails();
                    BluRingViewer.WaitforViewports();
                    bool isNewstudyPanleOpened = viewer.AllstudyPanel().Count == StudyPanel + 1;
                    bool isPositionLeft = (float.Parse(viewer.AllstudyPanel()[priorIndex + 1].GetCssValue("left").Replace("px", "")) > studyPanelLeft);

                    if (!(isNewstudyPanleOpened && isPositionLeft))
                    {
                        isPriorOpened = false;
                        break;
                    }
                    else
                    {
                        isPriorOpened = true;
                    }
                    priorIndex++;
                    if (priorIndex == 2)
                    {
                        while (priorIndex != 0)
                        {
                            viewer.CloseStudypanel(priorIndex + 1);
                            priorIndex--;
                        }
                    }
                }

                if (isPriorOpened)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();


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
        /// Universal Viewer: User is able to send email study via Integrator URL
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_164501(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                EHR ehr = new EHR();
                BluRingViewer viewer = new BluRingViewer();
                IntegratorStudies intgtrStudies = new IntegratorStudies();
                PatientsStudy ps = new PatientsStudy();
                BasePage basepage = new BasePage();

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String lastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastNameList");

                //Pre-conditions
                //Configure "E-mail Notification" in config tool
                servicetool.LaunchServiceTool();
                servicetool.SetEmailNotification(Config.AdminEmail, SMTPHost: Config.SMTPServer, port: Config.SMTPport);

                //Enable "Email Study" in config tool 
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                if (!wpfobject.IsCheckBoxSelected(ServiceTool.EnableFeatures.Name.EnableEmailStudy, 1))
                {
                    servicetool.ModifyEnableFeatures();
                    wpfobject.WaitTillLoad();
                    wpfobject.SelectCheckBox(ServiceTool.EnableFeatures.Name.EnableEmailStudy, 1);
                    wpfobject.WaitTillLoad();
                    servicetool.wpfobject.ClickOkPopUp();
                    servicetool.ApplyEnableFeatures();
                    wpfobject.WaitTillLoad();
                    servicetool.wpfobject.ClickOkPopUp();
                }

                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                ITabPage subtab = wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.EmailStudy);               

                //PIN Character Set: Mixed, PIN Length: 6 and Is Case Sensitive is selected                              
                ComboBox comboBox = WpfObjects._mainWindow.Get<ComboBox>(SearchCriteria.ByClassName("ComboBox"));
                comboBox.Select("Mixed");
                TextBox pinsize = wpfobject.GetUIItem<ITabPage, TextBox>(subtab, "AutoSelectTextBox", 0, "1");
                pinsize.Text = "6";
                CheckBox EnableCaseSensitive = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(subtab, "Is Case Sensitive", 1);
                if (!EnableCaseSensitive.Checked)
                    EnableCaseSensitive.Click();

                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();

                //Enable Email Study in Domain Management page
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("DomainManagement");
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                domain.SetCheckBoxInEditDomain("emailstudy", 0);
                domain.ClickSaveEditDomain();

                //Enable Email Study in Role Management page
                login.Navigate("RoleManagement");
                role.SelectDomainfromDropDown(Config.adminGroupName);
                role.SearchRole(Config.adminRoleName);
                role.SelectRole(Config.adminRoleName);
                role.ClickEditRole();
                role.SetCheckboxInEditRole("email", 0);
                role.ClickSaveEditRole();
                login.Logout();

                //Step1 
                // 1.Configure Test EHR
                // 2.In Service Tool, Integrator tab-- > set User Sharing = Always Enabled.
                // 3.Select Allow Show Selector and Show Selector search options in integrator tab of config tool and restart IIS           
                TestFixtures.UpdateFeatureFixture("usersharing", value: "Always enabled");
                TestFixtures.UpdateFeatureFixture("allowshowselector", value: "True");
                TestFixtures.UpdateFeatureFixture("allowshowselectorsearch", value: "True");
                servicetool.LaunchServiceTool();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();
                ExecutedSteps++;

                //Step2 
                // 1.Launch Test EHR application and enter the following details:
                // a.Address:-http://<url>/WebAccess
                // b.Auth provider: Bypass
                // c.Enable Usersharing: blank
                // d.User ID: Administrator
                // e.Security ID: Administrator - Administrator
                // f.Auto End Session: True
                ehr.LaunchEHR();
                wpfobject.GetMainWindow("Test WebAccess EHR");
                wpfobject.SelectTabFromTabItems("Image Load");
                wpfobject.WaitTillLoad();
                ehr.SetCommonParameters("http://" + Config.IConnectIP + "/webaccess");
                ExecutedSteps++;

                //Step3 - Enter Patient Last Name that matches one study which is available in configured data source and Click on Cmd line button and launch the URL in a browser.
                ehr.SetSearchKeys_Patient("lastname", lastName);
                String url = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.CreateNewSesion();
                viewer.NavigateToBluringIntegratorURL(url);
                Thread.Sleep(15000);
                intgtrStudies = (IntegratorStudies)login.NavigateToIntegratorFrame("Studies");
                DataTable intgrtable = basepage.CollectRecordsInTable(intgtrStudies.ListTable(), intgtrStudies.Intgr_Header(), intgtrStudies.Intgr_Row(), intgtrStudies.Intgr_Column());
                string[] columnvalue = intgrtable.AsEnumerable().Select(r => r.Field<string>(3)).ToArray();
                bool NameExists = columnvalue.Contains(lastName);
                if (NameExists)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step4 - Select any study in the patient result list and click on Universal button
                ps.SelectPatinet("Last Name", lastName);
                viewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", showselector: true);
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel)).Count == 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step5 - Click on email study icon from the study control panel and enter the valid mail address, name and reason and Click on "SEND" button               
                Dictionary<string, string> downloadedMail = new Dictionary<string, string>();
                EmailUtils GuestMail = new EmailUtils() { EmailId = Config.CustomUser1Email, Password = Config.CustomUserEmailPassword };
                GuestMail.MarkAllMailAsRead("INBOX");
                viewer.EmailStudy(Config.CustomUser1Email, "test name", "test reason");
                String pinNumber = viewer.FetchPin_BR();
                if (!String.IsNullOrEmpty(pinNumber))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step6 - Go to the destination Email and Check that the "Emailed Study" notification is received.                
                if (GuestMail.GetUnreadMailCount() == 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step7 - Click on the link available from email study to view the study. Enter pin number and click on OK                            
                downloadedMail = GuestMail.GetMailUsingIMAP("no-reply@merge.com", "Emailed Study", MarkAsRead: true, maxWaitTime: 3);
                string link = GuestMail.GetEmailedStudyLink(downloadedMail);
                LaunchEmailedStudy.LaunchStudy<BluRingViewer>(link, pinNumber);
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel)).Count == 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step8 - 
                // 1.Configure Test EHR
                // 2.In Service Tool, Integrator tab-- > set User Sharing = Always Disabled.
                // 3.Select Allow Show Selector and Show Selector serach options in integrator tab of config tool and restart IIS           
                TestFixtures.UpdateFeatureFixture("usersharing", value: "Always disabled");
                TestFixtures.UpdateFeatureFixture("allowshowselector", value: "True");
                TestFixtures.UpdateFeatureFixture("allowshowselectorsearch", value: "True");
                servicetool.LaunchServiceTool();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();
                ExecutedSteps++;

                //Step9 - 
                // 1.Launch Test EHR application and enter the following details:
                // a.Address:-http://<url>/WebAccess
                // b.Auth provider: Bypass
                // c.Enable Usersharing: blank
                // d.User ID: Administrator
                // e.Security ID: Administrator - Administrator
                // f.Auto End Session: True
                ehr.LaunchEHR();
                wpfobject.GetMainWindow("Test WebAccess EHR");
                wpfobject.SelectTabFromTabItems("Image Load");
                wpfobject.WaitTillLoad();
                ehr.SetCommonParameters("http://" + Config.IConnectIP + "/webaccess");
                ExecutedSteps++;

                //Step10 - Enter Patient Last Name that matches one study which is available in configured data source and Click on Cmd line button and launch the URL in a browser.
                ehr.SetSearchKeys_Patient("lastname", lastName);
                url = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.CreateNewSesion();
                viewer.NavigateToBluringIntegratorURL(url);
                Thread.Sleep(20000);
                intgtrStudies = (IntegratorStudies)login.NavigateToIntegratorFrame("Studies");                
                DataTable intgrtable1 = basepage.CollectRecordsInTable(intgtrStudies.ListTable(), intgtrStudies.Intgr_Header(), intgtrStudies.Intgr_Row(), intgtrStudies.Intgr_Column());
                string[] columnvalue1 = intgrtable.AsEnumerable().Select(r => r.Field<string>(3)).ToArray();
                bool NameExists1 = columnvalue1.Contains(lastName);
                if (NameExists1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step11 - Select any study in the patient result list and click on Universal button
                ps.SelectPatinet("Last Name", lastName);
                viewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", showselector: true);
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel)).Count == 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step12 - Ensure that the Email icon is disabled in the viewer
                bool step12 = viewer.GetElement("cssselector", BluRingViewer.div_emailTitle).GetAttribute("class").Contains("disabled");
                if (step12)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

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
                //Revert to default values                
                TestFixtures.UpdateFeatureFixture("usersharing", value: "Always disabled");
                TestFixtures.UpdateFeatureFixture("allowshowselector", value: "false");
                TestFixtures.UpdateFeatureFixture("allowshowselectorsearch", value: "false");
                servicetool.LaunchServiceTool();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();
            }

        }

        /// <summary>
        /// A User send a study with report to a Guest
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_164488(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            String adminUserName = Config.adminUserName;
            String adminPassword = Config.adminPassword;
            HTML5_Uploader html5 = new HTML5_Uploader();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Message = ReadExcel.GetTestData(filepath, "TestData", testid, "MessageList").ToString().Split(':');
                String[] Accession = AccessionList.Split(':');

                // configuring the email notification
                servicetool.LaunchServiceTool();
                servicetool.SetEmailNotification(AdministratorEmail: Config.AdminEmail, SMTPHost: Config.IMAPServer);
                wpfobject.WaitTillLoad();

                // Enabling "Email Study" and "PDF Reports"
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableEmailStudy();
                wpfobject.WaitTillLoad();
                servicetool.EnablePDFReport();
                wpfobject.WaitTillLoad();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                wpfobject.ClickOkPopUp();
                wpfobject.WaitTillLoad();

                // Enable "Display reports" under Email study sub tab 
                servicetool.NavigateSubTab("Email Study");
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                ITabPage EmailStudyTab = wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                CheckBox DisplayReports = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(EmailStudyTab, "Display Reports", 1);
                if (!DisplayReports.Checked)
                {
                    DisplayReports.Click();
                }
                wpfobject.WaitTillLoad();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();

                // Navigating to Report sub tab
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.Report);
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                ITabPage ReportTab = wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report);

                // Enabling Encapsulated PDF checkbox in the Report tab if not enabled
                CheckBox EncapsulatedPDF = servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(ReportTab, ServiceTool.EnableFeatures.ID.EncapsulatedPDF);
                if (!EncapsulatedPDF.Checked)
                {
                    EncapsulatedPDF.Click();
                }

                // Enabling CardioReports checkbox in the Report tab if not enabled
                CheckBox CardioReport = servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(ReportTab, ServiceTool.EnableFeatures.ID.MergeCardioReport);
                if (!CardioReport.Checked)
                {
                    CardioReport.Click();
                }

                // Enabling Otherreports checkbox in the Report tab if not enabled
                CheckBox otherReport = servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(ReportTab, ServiceTool.EnableFeatures.ID.OtherReports);
                if (!otherReport.Checked)
                {
                    otherReport.Click();
                }

                // Enabling KOS Report checkbox in the Report tab if not enabled
                CheckBox KOSReport = servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(ReportTab, ServiceTool.EnableFeatures.ID.KOSReports);
                if (!KOSReport.Checked)
                {
                    KOSReport.Click();
                }
                wpfobject.WaitTillLoad();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();

                // Restart and close the service tool
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();

                //Enabling Email Study in Domain Management
                login.LoginIConnect(adminUserName, adminPassword);
                var domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                Thread.Sleep(3000);
                domain.SetCheckBoxInEditDomain("emailstudy", 0);
                domain.SetCheckBoxInEditDomain("reportview", 0);
                domain.SetCheckBoxInEditDomain("pdfreport", 0);
                domain.ClickSaveEditDomain();

                // Enabling the Email Study in rolemanagement
                var rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectDomainfromDropDown(Config.adminGroupName);
                rolemanagement.SearchRole(Config.adminRoleName);
                rolemanagement.SelectRole(Config.adminRoleName);
                rolemanagement.ClickEditRole();
                rolemanagement.SetPrivelage("email");
                rolemanagement.SetCheckboxInEditRole("pdfreport", 0);
                rolemanagement.ClickSaveEditRole();
                login.Logout();

                // Step 1 and 2 - Logon to iCA as Administrator and load any study which has reports(PDF,SR,Cardio) into the Universal Viewer and verify report ICon is enabled.	
                login.LoginIConnect(adminUserName, adminPassword);
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: AccessionList, Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Accession", AccessionList);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                IWebElement reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon))[0];
                if (reportIcon.Enabled)
                    result.steps[ExecutedSteps += 2].StepPass();
                else
                    result.steps[ExecutedSteps += 2].StepFail();

                // Step 3 - Load the reports from the exam list by clicking on Report icon	
                viewer.OpenReport_BR(0);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                IWebElement ReportContainer = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.ReportContainer_div));
                var step3 = ReportContainer.Displayed;
                viewer.SelectReport_BR(0, 2, "PDF");
                viewer.SelectReport_BR(0, 4, "SR");
                if (step3)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 4 - Close all opened reports.	
                viewer.CloseReport_BR(0);
                Thread.Sleep(5000);
                var step4 = false;
                try
                {
                    if (ReportContainer.Displayed)
                        step4 = false;
                    else
                        step4 = true;
                }
                catch (Exception e)
                {
                    step4 = true;
                }
                if (step4)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 5 - Verify Email Icon is enabled in the study control panel	                
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_emailstudy)))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 6 - Click on email Study icon from study control panel	
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_emailstudy));
                if (viewer.ValidateEmailStudyDialogue())
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 7 and 8 - Enter the valid guest mail address, name and reason then click on "SEND" button and note down the pin number	
                Dictionary<string, string> downloadedMail = new Dictionary<string, string>();
                EmailUtils UserMail = new EmailUtils() { EmailId = Config.CustomUser1Email, Password = Config.CustomUserEmailPassword };
                UserMail.MarkAllMailAsRead("INBOX");
                viewer.EmailStudy(Config.CustomUser1Email, "test", "Testing", isOpenEmailWindow: true);
                var pinwindow = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_pinWindow));
                String message = pinwindow.FindElement(By.Id("PinCodeInfo_Label")).GetAttribute("innerHTML");
                var pinnumber = viewer.FetchPin_BR();               
                bool step7 = message.Equals("Please provide the following PIN code to the recipient. The recipient will be required to enter the PIN code to view the study.");
                if (!string.IsNullOrEmpty(pinnumber) && step7)
                    result.steps[ExecutedSteps += 2].StepPass();
                else
                    result.steps[ExecutedSteps += 2].StepFail();
                viewer.CloseBluRingViewer();

                // Step 9 - Go to the destination Email and Check that the "Emailed Study" notification is received.	
                downloadedMail = UserMail.GetMailUsingIMAP("no-reply@merge.com", "Emailed Study", MarkAsRead: true, maxWaitTime: 3);
                string emaillink = UserMail.GetEmailedStudyLink(downloadedMail);
                if (downloadedMail.Count > 0 && downloadedMail["Body"].ToLower().Contains("superadmin"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 10 - Click the link "Click here" from the mail.	
                LaunchEmailedStudy.LaunchStudy<BasePage>(emaillink);
                String infoMessage = viewer.GetElement(BasePage.SelectorType.CssSelector, LaunchEmailedStudy.span_messageInfo).Text;
                var step10_1 = infoMessage.Equals(Message[0]);
                Logger.Instance.InfoLog("Expected : " + Message[0]);
                Logger.Instance.InfoLog("Actual : " + infoMessage);
                String warningMessage = viewer.GetElement(BasePage.SelectorType.CssSelector, LaunchEmailedStudy.span_warningMessageInfo).Text;
                var step10_2 = warningMessage.Equals(Message[1]);
                Logger.Instance.InfoLog("Expected : " + Message[1]);
                Logger.Instance.InfoLog("Actual : " + warningMessage);
                if (step10_1 && step10_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 11 - Enter the noted PIN and click on "OK".
                viewer = LaunchEmailedStudy.LaunchStudy<BluRingViewer>(emaillink, pinnumber);
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel)).Count == 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 12 - Verify that the "Report icon" is enabled in the Exam List.
                reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon))[0];
                if (reportIcon.Enabled)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 13 - From the Exam List , click on the report icon and verify that the emalied reports [PDF,SR,Cardio] are loaded properly	                
                IList<IWebElement> GuestreportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon));
                viewer.ClickElement(GuestreportIcon[0]);
                Thread.Sleep(2000);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("GuestHomeFrame");
                ReportContainer = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.ReportContainer_div));
                var step13_1 = ReportContainer.Displayed;
                var reportContainer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.ReportContainer_div))[0];
                IList<IWebElement> reportlist = reportContainer.FindElements(By.CssSelector(BluRingViewer.ReportTabList_div));
                reportlist[2].Click();
                Thread.Sleep(2000);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("GuestHomeFrame");
                var iframes = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.SRReport_iframe));
                var step13_2 = iframes[0].Displayed;
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("GuestHomeFrame");
                reportlist = reportContainer.FindElements(By.CssSelector(BluRingViewer.ReportTabList_div));
                reportlist[4].Click();
                Thread.Sleep(2000);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("GuestHomeFrame");
                iframes = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.SRReport_iframe));
                var step13_3 = iframes[0].Displayed;
                if (step13_1 && step13_2 && step13_3)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();


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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        ///  Emailing a Study from Comparision viewer
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_164490(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            String adminUserName = Config.adminUserName;
            String adminPassword = Config.adminPassword;
            DomainManagement domain = new DomainManagement();
            UserManagement usermanagement = new UserManagement();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                var Accession = AccessionList.Split(':');

                // configuring the email notification
                servicetool.LaunchServiceTool();
                servicetool.SetEmailNotification(AdministratorEmail: Config.AdminEmail, SMTPHost: Config.IMAPServer);

                // Enabling "Email Study" and "study sharing"
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableEmailStudy();
                wpfobject.WaitTillLoad();
                servicetool.EnableStudySharing();
                wpfobject.WaitTillLoad();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                wpfobject.ClickOkPopUp();
                wpfobject.WaitTillLoad();

                // Restart and close the service tool
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();

                //Create new domain with email study
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("DomainManagement");
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                domain.SetCheckBoxInEditDomain("emailstudy", 0);
                domain.ClickSaveEditDomain();

                // Enabling the Email Study in rolemanagement
                var rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectDomainfromDropDown(Config.adminGroupName);
                rolemanagement.SearchRole(Config.adminRoleName);
                rolemanagement.SelectRole(Config.adminRoleName);
                rolemanagement.ClickEditRole();
                rolemanagement.SetPrivelage("email");
                rolemanagement.ClickSaveEditRole();
                login.Logout();

                // Step 1 - Logon to iCA as Administrator and load any study into the Universal Viewer.	
                login.LoginIConnect(adminUserName, adminPassword);
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                // Step 2 - From the Exam List single click on any one listed prior study.
                viewer.OpenPriors(1);
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_studypanel + ":nth-of-type(2)")))
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The new Study panel is not opened");
                }

                // Step 3 && 4 - Select the comparision viewer and click on email study icon from comparision viewer and note the pin number
                viewer.clickEmailStudyIcon(2);
                var attachedStudiesCount = viewer.getAttachedStudiesCount();
                Dictionary<string, string> downloadedMail = new Dictionary<string, string>();
                EmailUtils UserMail = new EmailUtils() { EmailId = Config.CustomUser1Email, Password = Config.CustomUserEmailPassword };
                UserMail.MarkAllMailAsRead("INBOX");
                viewer.EmailStudy(Config.CustomUser1Email, "test", "Testing", isOpenEmailWindow: false);
                var pinnumber = viewer.FetchPin_BR();
                if (attachedStudiesCount.Equals(1) && !String.IsNullOrEmpty(pinnumber))
                    result.steps[ExecutedSteps += 2].StepPass();
                else
                {
                    result.steps[ExecutedSteps += 2].StepFail();
                    Logger.Instance.InfoLog("The expected attached study text is 1  and the actual is " + attachedStudiesCount);
                }
                viewer.CloseBluRingViewer();

                // Step 5 - Go to outbounds tab and check the Emailed study	                
                var outbound = (Outbounds)login.Navigate("Outbounds");
                outbound.SearchStudy(AccessionNo: Accession[1], Study_Performed_Period: "All Dates", Study_Received_Period: "All Dates");
                String status;
                outbound.GetMatchingRow("Accession", Accession[1]).TryGetValue("Status", out status);
                if (status.Equals("Emailed"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The Expected Status is Emailed and the Actual is " + status);
                }

                // Step 6 - Go to the destination Email and Check that the "Emailed Study" notification is received.	
                downloadedMail = UserMail.GetMailUsingIMAP("no-reply@merge.com", "Emailed Study", MarkAsRead: true, maxWaitTime: 3);
                string emaillink = UserMail.GetEmailedStudyLink(downloadedMail);
                if (downloadedMail.Count > 0 && downloadedMail["Body"].ToLower().Contains("superadmin "))
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The actual mail count is " + downloadedMail.Count);
                }

                // Step 7 - Click on the link available from email study to view the study. Enter pin number and click on OK.
                viewer = LaunchEmailedStudy.LaunchStudy<BluRingViewer>(emaillink, pinnumber);
                var thumbnailsCaption = viewer.GetStudyPanelThumbnailCaption();
                if (thumbnailsCaption[0].Equals("S2") && thumbnailsCaption[1].Equals("S3"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The actual mail count is " + downloadedMail.Count);
                }

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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        ///  Emailing a study with related studies
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_164352(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            String adminUserName = Config.adminUserName;
            String adminPassword = Config.adminPassword;
            DomainManagement domain = new DomainManagement();
            UserManagement usermanagement = new UserManagement();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');

                // configuring the email notification
                servicetool.LaunchServiceTool();
                servicetool.SetEmailNotification(AdministratorEmail: Config.AdminEmail, SMTPHost: Config.IMAPServer);
                wpfobject.WaitTillLoad();

                // Enabling "Email Study"
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableEmailStudy();
                wpfobject.WaitTillLoad();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                wpfobject.ClickOkPopUp();
                wpfobject.WaitTillLoad();

                // Restart and close the service tool
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();

                //Enabling Email Study in Domain Management
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("DomainManagement");
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                Thread.Sleep(3000);
                domain.SetCheckBoxInEditDomain("emailstudy", 0);
                domain.ClickSaveEditDomain();

                // Enabling the Email Study in rolemanagement
                var rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectDomainfromDropDown(Config.adminGroupName);
                rolemanagement.SearchRole(Config.adminRoleName);
                rolemanagement.SelectRole(Config.adminRoleName);
                rolemanagement.ClickEditRole();
                rolemanagement.SetPrivelage("email");
                rolemanagement.ClickSaveEditRole();
                login.Logout();

                // Step 1 - Logon to iCA as Administrator and load any study into the Universal Viewer.	
                login.LoginIConnect(adminUserName, adminPassword);
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[2], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[2]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                // Step 2 - Load all prior studies by single click on each prior studies from the exam list.	
                viewer.OpenPriors(1);
                viewer.OpenPriors(2);
                viewer.OpenPriors(3);                
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel)).Count == 4)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 3 - Select the primary study panel and click on email study icon from primary study control panel.	                
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_ExamsIconButton));
                Thread.Sleep(4000);
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.GetViewportCss(1, 0)));
                Thread.Sleep(2000);
                viewer.clickEmailStudyIcon(1);                
                viewer.WaitTillEmailWindowAppears();
                bool step3_1 = viewer.ValidateEmailStudyDialogue(true);                
                bool step3_2 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_emailStudyList)).Count == 6;
                if (step3_1 && step3_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The Email studies are not visible in Email Study Dialoge box and step3_1 is " + step3_1 + " and step3_2 is " + step3_2);
                }

                // Step 4 - Verify that the user unable to perform any action in the viewer
                bool step4_1 = false;
                try
                {
                    IWebElement showhide = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_ShowHideTool);
                    showhide.Click();
                    if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ShowHideDropdown)).Count == 0)
                        step4_1 = true;
                }
                catch (Exception e) { step4_1 = true; }
                if (step4_1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 5 - Click on CANCEL button in the dialogue box.	
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_cancelEmail));
                Thread.Sleep(3000);
                if (!viewer.IsElementVisible(By.CssSelector(BluRingViewer.div_emailWindow)))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 6 - Verify that the user able to perform any action.	
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();
                viewer.SelectViewerTool(BluRingTools.Get_Pixel_Value, isOpenToolsPOPup: false);

                // Step 7 - Again select the primary study control panel and click on email study icon from primary study control panel.	
                viewer.SetViewPort(0, 1);
                viewer.clickEmailStudyIcon(1);                
                bool step7_1 = viewer.ValidateEmailStudyDialogue(true);
                bool step7_2 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_emailStudyList)).Count == 6;
                if (step7_1 && step7_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The Email studies are not visible in Email Study Dialoge box and step7_1 is " + step7_1 + " and step7_2 is " + step7_2);
                }

                // Step 8 - Ensure that Primary Study is selected as default
                var defaultstudy = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.input_primaryStudyCheckbox);
                if (!defaultstudy.Enabled && defaultstudy.Selected)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 9 - Enter all the details and select all prior studies by Clicking on "SELECT ALL OTHER STUDIES" checkbox
                viewer.SendKeys(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.input_email), Config.CustomUser1Email);
                viewer.SendKeys(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.input_emailName), "test");
                viewer.SendKeys(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.input_Notes), "Testing");
                int studiesCountBeforeSelectPriors = viewer.getAttachedStudiesCount();
                viewer.ClickElement(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_emailSelectAll)));
                Thread.Sleep(3000);
                if (viewer.getAttachedStudiesCount() == studiesCountBeforeSelectPriors + 5)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 10 and 11 - Click on "SEND" button	and Note down the generated PIN number.
                Dictionary<string, string> downloadedMail = new Dictionary<string, string>();
                EmailUtils UserMail = new EmailUtils() { EmailId = Config.CustomUser1Email, Password = Config.CustomUserEmailPassword };
                UserMail.MarkAllMailAsRead("INBOX");
                viewer.ClickElement(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_sendEmail)));
                var pinnumber = viewer.FetchPin_BR();
                if (!String.IsNullOrEmpty(pinnumber))
                    result.steps[ExecutedSteps += 2].StepPass();
                else
                    result.steps[ExecutedSteps += 2].StepFail();

                // Step 12 - From iCA server SQL, query as 'Select * from dbo.Guest' in IRWSDB database
                DataBaseUtil db = new DataBaseUtil("sqlserver", "IRWSDB", InstanceName: "WEBACCESS");
                db.ConnectSQLServerDB();
                IList<String> rows = db.ExecuteQuery("select * from dbo.Guest order by GuestUid DESC");
                var OperationExColumn = db.ExecuteQuery("select OperationEx from dbo.Guest where GuestUid = " + rows[0]);
                var Step12_1 = OperationExColumn[0].Contains("<AccessionNumber>" + Accession[0] + "</AccessionNumber>");
                var Step12_2 = OperationExColumn[0].Contains("<AccessionNumber>" + Accession[1] + "</AccessionNumber>");
                var Step12_3 = OperationExColumn[0].Contains("<AccessionNumber>" + Accession[2] + "</AccessionNumber>");
                var Step12_4 = OperationExColumn[0].Contains("<AccessionNumber>" + Accession[3] + "</AccessionNumber>");
                var Step12_5 = OperationExColumn[0].Contains("<RelatedStudies>");
                if (Step12_1 && Step12_2 && Step12_3 && Step12_4 && Step12_5)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 13 - Go to the destination Email and Check that the "Emailed Study" notification is received.	
                downloadedMail = UserMail.GetMailUsingIMAP("no-reply@merge.com", "Emailed Study", MarkAsRead: true, maxWaitTime: 3);
                string emaillink = UserMail.GetEmailedStudyLink(downloadedMail);
                if (downloadedMail.Count > 0 && downloadedMail["Body"].ToLower().Contains("superadmin"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The actual mail count is " + downloadedMail.Count);
                }
                viewer.CloseBluRingViewer();

                // Step 14 - Click on the link available from email study to view the study.
                viewer = LaunchEmailedStudy.LaunchStudy<BluRingViewer>(emaillink, pinnumber);
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel)).Count == 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 15 - Verify that all emailed priors studies are listed in the exam list.	
                var priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                if (priors.Count.Equals(6))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 16 - Load all prior studies by single click on each prior studies from the exam list.	
                viewer.OpenPriors(1);
                viewer.OpenPriors(2);
                viewer.OpenPriors(3);
                viewer.OpenPriors(4);
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel)).Count == 5)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Share exams to an unregistered user from inbounds page
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_164348(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            String adminUserName = Config.adminUserName;
            String adminPassword = Config.adminPassword;
            HTML5_Uploader html5 = new HTML5_Uploader();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String UploadPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Path");
                String[] Accession = AccessionList.Split(':');

                // configuring the email notification
                servicetool.LaunchServiceTool();
                servicetool.SetEmailNotification(AdministratorEmail: Config.AdminEmail, SMTPHost: Config.IMAPServer);
                wpfobject.WaitTillLoad();

                // Enabling "Email Study" and "study sharing"
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableEmailStudy();
                wpfobject.WaitTillLoad();
                servicetool.EnableStudySharing();
                wpfobject.WaitTillLoad();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                wpfobject.ClickOkPopUp();
                wpfobject.WaitTillLoad();

                // Restart and close the service tool
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();

                // Upload few studies via Uploader tool (Say as HTML5 Uploader)
                string[] HTML5WindowHandle = login.OpenHTML5UploaderandSwitchtoIT("login");
                login.ClickElement(html5.RegisteredUserRadioBtn());
                html5.UserNameTxtBox().SendKeys(adminUserName);
                html5.PasswordTxtBox().SendKeys(adminPassword);
                login.ClickElement(html5.SignInBtn());
                Thread.Sleep(2000);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                html5.UploadFilesBtn().Click();
                login.UploadFileInBrowser(Path.Combine(Environment.CurrentDirectory, UploadPath), "file", AppendDrivepath: false);
                PageLoadWait.WaitForHTML5StudyToUpload();
                login.ClickElement(html5.ShareJobButton());
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(html5.By_DestinationDropdown()));
                html5.DestinationDropdown().SelectByText(Config.Dest1);
                login.ClickElement(html5.ShareBtn());
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(html5.By_DragFilesDiv()));
                html5.Logout_HTML5Uploader();
                login.CloseHTML5Window(HTML5WindowHandle[1], HTML5WindowHandle[0]);

                //Enabling Email Study in Domain Management
                login.LoginIConnect(adminUserName, adminPassword);
                var domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                domain.SetCheckBoxInEditDomain("emailstudy", 0);
                domain.ClickSaveEditDomain();

                // Enabling the Email Study in rolemanagement
                var rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectDomainfromDropDown(Config.adminGroupName);
                rolemanagement.SearchRole(Config.adminRoleName);
                rolemanagement.SelectRole(Config.adminRoleName);
                rolemanagement.ClickEditRole();
                rolemanagement.SetPrivelage("email");
                rolemanagement.ClickSaveEditRole();
                login.Logout();

                // Step 1 - Login in to iCA with physician, Go to inbounds page and select any study with uploaded status then click on "Universal" button.
                login.LoginIConnect(Config.ph1UserName, Config.phPassword);
                var inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(AccessionNo: Accession[0]);
                String iboundsStatus;
                inbounds.GetMatchingRow("Accession", Accession[0]).TryGetValue("Status", out iboundsStatus);
                inbounds.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                if (iboundsStatus.Equals("Uploaded"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The Expected Status is Emailed and the Actual is " + iboundsStatus);
                }

                // Step 2 - Click on email study icon from the study control panel and enter the valid mail address, name and reason and Click on "SEND" button	
                Dictionary<string, string> downloadedMail = new Dictionary<string, string>();
                EmailUtils UserMail = new EmailUtils() { EmailId = Config.CustomUser1Email, Password = Config.CustomUserEmailPassword };
                UserMail.MarkAllMailAsRead("INBOX");
                viewer.EmailStudy(Config.CustomUser1Email, "test", "Testing", isOpenEmailWindow: true);
                var pinnumber = viewer.FetchPin_BR();
                if (!string.IsNullOrEmpty(pinnumber))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();
                viewer.CloseBluRingViewer();

                // Step 3 - Go to outbound tab of physician and check the Emailed study.	
                var outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy(AccessionNo: Accession[0]);
                String OutboundsStatus;
                outbounds.GetMatchingRow("Accession", Accession[0]).TryGetValue("Status", out OutboundsStatus);
                if (OutboundsStatus.Equals("Emailed"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The Expected Status is Emailed and the Actual is " + OutboundsStatus);
                }

                // Step 4 - Go to the destination Email and Check that the "Emailed Study" notification is received.	
                downloadedMail = UserMail.GetMailUsingIMAP("no-reply@merge.com", "Emailed Study", MarkAsRead: true, maxWaitTime: 3);
                string emaillink = UserMail.GetEmailedStudyLink(downloadedMail);
                if (downloadedMail.Count > 0 && downloadedMail["Body"].ToLower().Contains(Config.ph1UserName))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 5 - Click on the link available from email study to view the study  Enter pin number and click on OK Ensure that the study is loaded in Universal Viewer
                viewer = LaunchEmailedStudy.LaunchStudy<BluRingViewer>(emaillink, pinnumber);
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel)).Count == 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 6 - Select active series view port and right click on mouse button to open the floating tool box	
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_Toolbox)))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 7 - Select any tool from the floating toolbox(Say as Line) and draw line measurement in the active series viewport.
                viewer.SelectViewerTool(BluRingTools.Line_Measurement, isOpenToolsPOPup: false);
                viewer.ApplyTool_LineMeasurement();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step9 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step9)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }


        /// <summary>
        /// Universal  Viewer : Emailing  study with only reports
        /// </summary>
        public TestCaseResult Test_167647(String testid, String teststeps, int stepcount)
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
                Studies studies = null;
                BluRingViewer viewer = new BluRingViewer();
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");

                //Pre-conditions
                //Configure Email notifications
                servicetool.LaunchServiceTool();
                servicetool.SetEmailNotification(Config.AdminEmail, SMTPHost: Config.SMTPServer, port: Config.SMTPport);

                //Enable Email Study and PDF Report
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                CheckBox EmailStudy = wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), "Enable Email Study", 1);
                if (!EmailStudy.Checked)
                {
                    EmailStudy.Click();
                    wpfobject.WaitTillLoad();
                }
                CheckBox PDFReportCheckBox = wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), "Enable PDF Report", 1);
                if (!PDFReportCheckBox.Checked)
                {
                    PDFReportCheckBox.Click();
                    wpfobject.WaitTillLoad();
                }
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.wpfobject.ClickOkPopUp();

                // Navigating to Report sub tab
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.Report);
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                ITabPage ReportTab = wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report);

                // Enabling Encapsulated PDF checkbox in the Report tab if not enabled
                CheckBox EncapsulatedPDF = servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(ReportTab, ServiceTool.EnableFeatures.ID.EncapsulatedPDF);
                if (!EncapsulatedPDF.Checked)
                {
                    EncapsulatedPDF.Click();
                }

                // Enabling CardioReports checkbox in the Report tab if not enabled
                CheckBox CardioReport = servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(ReportTab, ServiceTool.EnableFeatures.ID.MergeCardioReport);
                if (!CardioReport.Checked)
                {
                    CardioReport.Click();
                }

                // Enabling Otherreports checkbox in the Report tab if not enabled
                CheckBox otherReport = servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(ReportTab, ServiceTool.EnableFeatures.ID.OtherReports);
                if (!otherReport.Checked)
                {
                    otherReport.Click();
                }

                // Enabling KOS Report checkbox in the Report tab if not enabled
                CheckBox KOSReport = servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(ReportTab, ServiceTool.EnableFeatures.ID.KOSReports);
                if (!KOSReport.Checked)
                {
                    KOSReport.Click();
                }
                wpfobject.WaitTillLoad();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();

                // Restart and close the service tool
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();

                //Enable Email study option at Domain 
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("DomainManagement");
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                domain.SetCheckBoxInEditDomain("emailstudy", 0);
                domain.SetCheckBoxInEditDomain("reportview", 0);
                domain.SetCheckBoxInEditDomain("pdfreport", 0);
                domain.ClickSaveEditDomain();

                //Enable Email Study in Role Management page
                login.Navigate("RoleManagement");
                role.SelectDomainfromDropDown(Config.adminGroupName);
                role.SearchRole(Config.adminRoleName);
                role.SelectRole(Config.adminRoleName);
                role.ClickEditRole();
                role.SetCheckboxInEditRole("email", 0);
                role.SetCheckboxInEditRole("pdfreport", 0);
                role.ClickSaveEditRole();
                login.Logout();

                //step1 From the iCA service tool, Enable features -> Email study tab -> check the "Display reports" option and restart IIS. 
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                ITabPage subtab = wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                CheckBox DisplayReport = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(subtab, "Display Reports", 1);
                if (!DisplayReport.Checked)
                {
                    DisplayReport.Click();
                }
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();
                ExecutedSteps++;

                //step2 - Login to iCA as Administrator and load the study that has only reports into the Universal Viewer.
                login.CreateNewSesion();
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID, Datasource: login.GetHostName(Config.SanityPACS));
                studies.SelectStudy("Patient ID", PatientID);
                BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                IWebElement ele = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                bool step2_1 = studies.CompareImage(result.steps[ExecutedSteps], ele);
                IWebElement reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon))[0];
                bool step2_2 = reportIcon.Enabled;
                if (step2_1 && step2_2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step3 - Click on the "Reports" icon to load the reports and verify the reports.
                viewer.OpenReport_BR(0);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                IWebElement ReportContainer = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.ReportContainer_div));
                var step3 = ReportContainer.Displayed;
                viewer.SelectReport_BR(0, 0, "PDF");
                viewer.SelectReport_BR(0, 1, "PDF");
                if (step3)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();
                viewer.CloseReport_BR(0);
                Thread.Sleep(5000);

                //Step4 - Email the study to guest user and note down the generated PIN number.
                Dictionary<string, string> downloadedMail = new Dictionary<string, string>();
                EmailUtils GuestMail = new EmailUtils() { EmailId = Config.CustomUser1Email, Password = Config.CustomUserEmailPassword };
                GuestMail.MarkAllMailAsRead("INBOX");
                viewer.EmailStudy(Config.CustomUser1Email, "test name", "test reason");
                String pinNumber = viewer.FetchPin_BR();
                if (pinNumber != null)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step5 - From the guest user mail box, Launch the URL to load the study into the Universal Viewer
                downloadedMail = GuestMail.GetMailUsingIMAP("no-reply@merge.com", "Emailed Study", MarkAsRead: true, maxWaitTime: 3);
                string link = GuestMail.GetEmailedStudyLink(downloadedMail);
                LaunchEmailedStudy.LaunchStudy<BasePage>(link, pinNumber);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                ele = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                bool step5_1 = studies.CompareImage(result.steps[ExecutedSteps], ele);
                reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon))[0];
                bool step5_2 = reportIcon.Enabled;
                if (step5_1 && step5_2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step6 - Load and verify the reports in the universal viewer.
                viewer.OpenReport_BR(0, Guest: true);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("GuestHomeFrame");
                IWebElement reportContainer = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.ReportContainer_div));
                var step6 = reportContainer.Displayed;
                viewer.SelectReport_BR(0, 0, "PDF", Guest: true);
                viewer.SelectReport_BR(0, 1, "PDF", Guest: true);
                if (step6)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();
                viewer.CloseReport_BR(0, Guest: true);
                Thread.Sleep(5000);

                //Step7 - From the iCA service tool, Enable features -> Email study tab -> uncheck the "Display reports" option and restart IIS.
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                subtab = wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                DisplayReport = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(subtab, "Display Reports", 1);
                if (DisplayReport.Checked)
                {
                    DisplayReport.Click();
                }
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();
                ExecutedSteps++;

                //Step8 - Login to iCA as Administrator and load the study that has only reports into the Universal Viewer.
                login.CreateNewSesion();
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID, Datasource: login.GetHostName(Config.SanityPACS));
                studies.SelectStudy("Patient ID", PatientID);
                BluRingViewer.LaunchBluRingViewer();
                ele = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step8_1 = studies.CompareImage(result.steps[ExecutedSteps], ele);
                reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon))[0];
                bool step8_2 = reportIcon.Enabled;
                if (step8_1 && step8_2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step9 - Click on the "Reports" icon to load the reports and verify the reports.
                viewer.OpenReport_BR(0);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                ReportContainer = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.ReportContainer_div));
                var step9 = ReportContainer.Displayed;
                viewer.SelectReport_BR(0, 0, "PDF");
                viewer.SelectReport_BR(0, 1, "PDF");
                if (step9)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();
                viewer.CloseReport_BR(0);
                Thread.Sleep(5000);

                //Step10 - Email the study to guest user and note down the generated PIN number.
                GuestMail.MarkAllMailAsRead("INBOX");
                viewer.EmailStudy(Config.CustomUser1Email, "test name", "test reason");
                pinNumber = viewer.FetchPin_BR();
                if (pinNumber != null)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step11 - From the guest user mail box, Launch the URL to load the study and verify the emailed reports.
                downloadedMail = GuestMail.GetMailUsingIMAP("no-reply@merge.com", "Emailed Study", MarkAsRead: true, maxWaitTime: 3);
                string link1 = GuestMail.GetEmailedStudyLink(downloadedMail);
                LaunchEmailedStudy.LaunchStudy<BasePage>(link1, pinNumber);
                bool step11 = viewer.GetElement(BasePage.SelectorType.CssSelector, "div.popDialog.showDialog span").GetAttribute("innerHTML").Equals("Study could not be loaded as report viewing feature is disabled");
                IWebElement ClosePopup = viewer.GetElement(BasePage.SelectorType.CssSelector, "div.closeButton:nth-of-type(2)");
                viewer.ClickElement(ClosePopup);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                ele = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                bool step11_1 = studies.CompareImage(result.steps[ExecutedSteps], ele);
                //reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon))[0];
                var reportIcon1 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon));
                bool step11_2 = reportIcon1.Count.Equals(0);
                Logger.Instance.InfoLog("Step11 status: " + step11 + " step11_1 status: " + step11_1 + " step11_2 status: " + step11_2);
                if (step11 && step11_1 && (step11_2))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();


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
                //Disable Email Study
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                CheckBox EmailStudy = wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), "Enable Email Study", 1);
                if (EmailStudy.Checked)
                {
                    EmailStudy.Click();
                    wpfobject.WaitTillLoad();
                }
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();
            }
        }
    }
}





