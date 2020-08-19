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
using Selenium.Scripts.Pages.MergeServiceTool;
using TestStack.White.UIItems;
using TestStack.White.UIItems.TabItems;
using Selenium.Scripts.Pages.MergeDominator;
using System.Threading;

namespace Selenium.Scripts.Tests
{
    class UnityPACSIntegration
    {
        public Login login { get; set; }
        public string filepath { get; set; }

        ServiceTool servicetool = new ServiceTool();
        WpfObjects wpfobject = new WpfObjects();

        public UnityPACSIntegration(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }


        /// <summary>
        /// Querying and retrieving DICOM content from Unity Pacs Datasource
        /// </summary>
        public TestCaseResult Test_164332(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            BluRingViewer viewer = new BluRingViewer();
            DomainManagement domain = new DomainManagement();
            RoleManagement role = new RoleManagement();
            UserManagement usermanagement = new UserManagement();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String studyDetailsList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDetails");
                String PatientName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName");
                String PatientDOB = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DOB");
                String PatientMRN = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "MRN");
                var accession = AccessionList.Split(':');
                var studyDetails = studyDetailsList.Split('=');

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
                    wpfobject.SelectCheckBox(ServiceTool.EnableFeatures.Name.EnablePDFReport, 1);
                    wpfobject.WaitTillLoad();
                    wpfobject.SelectCheckBox(ServiceTool.EnableFeatures.Name.EnableEmailStudy, 1);
                    wpfobject.WaitTillLoad();
                    servicetool.wpfobject.ClickOkPopUp();
                    servicetool.ApplyEnableFeatures();
                    wpfobject.WaitTillLoad();
                    servicetool.wpfobject.ClickOkPopUp();
                }
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
                // Restart and close the service tool
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();

                // Precondition
                //1. Not required
                //2, 3. Create new Domain and User
                login.LoginIConnect(adminUserName, adminPassword);
                String DomainName = "UnityDomain" + new Random().Next(10000);
                String Role = "UnityRole" + new Random().Next(10000);
                String User = "UnityUser" + new Random().Next(10000);
                domain.CreateDomain(DomainName, Role, datasources: null);
                domain.ClickSaveNewDomain();
                login.Navigate("UserManagement");
                usermanagement.CreateUser(User, DomainName, Role);

                //4. Enable report with all type and Email Study from System/domain/Role level settings
                //Enable Email Study in Domain Management page
                login.Navigate("DomainManagement");
                domain.SearchDomain(DomainName);
                domain.SelectDomain(DomainName);
                domain.ClickEditDomain();
                domain.SetCheckBoxInEditDomain("emailstudy", 0);
                domain.SetCheckBoxInEditDomain("pdfreport", 0);
                domain.ClickSaveEditDomain();

                //Enable Email Study in Role Management page
                login.Navigate("RoleManagement");
                role.SelectDomainfromDropDown(DomainName);
                role.SearchRole(Role);
                role.SelectRole(Role);
                role.ClickEditRole();
                role.SetCheckboxInEditRole("email", 0);
                role.SetCheckboxInEditRole("pdfreport", 0);
                role.ClickSaveEditRole();
                login.Logout();

                // Step 1 - Login to iCA webaccess as a created standard user.	
                login.LoginIConnect(User, User);
                if (login.IsTabPresent("Studies"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 2 - From the studylist, search the studies from Unity Pacs datasource based on available query combinations and Verify the retrieve information's from Unity Pacs to iCA study list column	
                Studies studies = new Studies();
                studies.SearchStudy(AccessionNo: accession[0]);
                var studyInfo = studies.GetMatchingRow("Accession", accession[0]);
                var step2_1 = studyInfo["Study Date"].Contains(studyDetails[0]);
                var step2_2 = studyInfo["Modality"].Contains(studyDetails[1]);
                var step2_3 = studyInfo["Patient Name"].Contains(studyDetails[2]);
                var step2_4 = studyInfo["Patient ID"].Contains(studyDetails[3]);
                var step2_5 = studyInfo["Description"].Contains(studyDetails[4]);
                var step2_6 = studyInfo["Accession"].Contains(studyDetails[5]);
                if (step2_1 && step2_2 && step2_3 && step2_4 && step2_5 && step2_6)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // step 3 - From iCA studylist, load different types of modality studies on Universal viewer.
                //Say as MG, US, XA, CR, NM, MR, CT, PT, SM, RF, DX, OT, XR, ECG
                bool step3 = false;
                ExecutedSteps++;
                String[] modalityList = { "MG", "US", "XA", "CR", "NM", "MR", "CT", "PT", "SM", "RF", "DX", "OT", "XR", "ECG" };
                for (int i = 1; i <= modalityList.Length; i++)
                {
                    studies.SearchStudy(AccessionNo: accession[i]);
                    studies.SelectStudy("Accession", accession[i]);
                    BluRingViewer.LaunchBluRingViewer();
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, i);
                    step3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                    viewer.CloseBluRingViewer();
                }
                if (step3)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 4 - Load a study on Universal viewer and perform few review tools
                //Ex: WL, Pan, Line Meaurement, Add text, Zoom, Ellipse
                //Note: Save series and Save annotated Images are not supported.Unity does not handle adding new data to an existing study very well. This is a known limitation and is not expected to work
                studies.SearchStudy(AccessionNo: accession[1]);
                studies.SelectStudy("Accession", accession[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.SelectViewerTool(BluRingTools.Window_Level);
                viewer.ApplyTool_WindowWidth();
                viewer.SelectViewerTool(BluRingTools.Pan);
                viewer.ApplyTool_Pan();
                viewer.SelectViewerTool(BluRingTools.Line_Measurement);
                viewer.ApplyTool_LineMeasurement();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                var step4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step4)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 5 - Close the viewer and Load a study on Universal viewer which has related studies.	
                viewer.CloseBluRingViewer();
                studies.SearchStudy(AccessionNo: accession[4]);
                studies.SelectStudy("Accession", accession[4]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                if (viewer.IsAllPriorsDisplayed())
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();


                // Step 6 - From Exam list, Verify the related studies are listed based on the iCA query related parameter configurations. [Patient ID, First Name, Last Name and DOB]	
                string[] details = { PatientName, PatientDOB, PatientMRN };
                var priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                Dictionary<IWebElement, List<string>> map = new Dictionary<IWebElement, List<string>>();
                foreach (IWebElement prior in priors)
                {
                    int i = 0;
                    Actions act = new Actions(BasePage.Driver);
                    act.MoveToElement(prior).Build().Perform();
                    String ExamListDetails = prior.GetAttribute("title");
                    String[] SplitedPat = ExamListDetails.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    List<string> PatDetails = new List<string>();
                    for (i = 0; i < 3; i++)
                        PatDetails.Add(SplitedPat[i]);
                    map.Add(prior, PatDetails);

                }
                bool flag = true;
                foreach (KeyValuePair<IWebElement, List<string>> key in map)
                {
                    int count = 0;
                    foreach (String value in key.Value)
                    {
                        for (int i = 0; i < details.Length; i++)
                            if (details[i].Contains(value))
                            {
                                count++;
                                break;
                            }
                    }
                    if (!count.Equals(details.Length))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step7 Load any related study from exam list verify that review tools are applied to the related studies.
                viewer.OpenPriors(0);
                viewer.SetViewPort(0, 2);
                viewer.SelectViewerTool(BluRingTools.Window_Level, 2, 1);
                viewer.ApplyTool_WindowWidth();
                viewer.SelectViewerTool(BluRingTools.Pan, 2, 1);
                viewer.ApplyTool_Pan();
                viewer.SelectViewerTool(BluRingTools.Line_Measurement, 2, 1);
                viewer.ApplyTool_LineMeasurement();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                var step7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.GetViewportCss(1, 0)));
                if (step7)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step8 Email the Unity Pacs study from Universal viewer and verify that emailed study is loaded from link.
                Dictionary<string, string> downloadedMail = new Dictionary<string, string>();
                EmailUtils UserMail = new EmailUtils() { EmailId = Config.CustomUser1Email, Password = Config.CustomUserEmailPassword };
                UserMail.MarkAllMailAsRead("INBOX");
                viewer.clickEmailStudyIcon(2);
                viewer.EmailStudy(Config.CustomUser1Email, "test", "Testing", isOpenEmailWindow: false);
                var pinnumber = viewer.FetchPin_BR();
                downloadedMail = UserMail.GetMailUsingIMAP("no-reply@merge.com", "Emailed Study", MarkAsRead: true, maxWaitTime: 3);
                string emaillink = UserMail.GetEmailedStudyLink(downloadedMail);
                var Viewer = LaunchEmailedStudy.LaunchStudy<BluRingViewer>(emaillink, pinnumber);
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel)).Count == 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step9 Email the Unity Pacs study with report from Universal viewer and verify that emailed study is loaded from link.
                Viewer.CreateNewSesion();
                login.LoginIConnect(User, User);
                studies.SearchStudy(AccessionNo: accession[1]);
                studies.SelectStudy("Accession", accession[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                UserMail.MarkAllMailAsRead("INBOX");
                viewer.EmailStudy(Config.CustomUser1Email, "test", "Testing", isOpenEmailWindow: true);
                pinnumber = viewer.FetchPin_BR();
                downloadedMail = UserMail.GetMailUsingIMAP("no-reply@merge.com", "Emailed Study", MarkAsRead: true, maxWaitTime: 3);
                emaillink = UserMail.GetEmailedStudyLink(downloadedMail);
                Viewer = LaunchEmailedStudy.LaunchStudy<BluRingViewer>(emaillink, pinnumber);
                IWebElement GuestreportIcon = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_priorsreportIcon));
                if (GuestreportIcon.Enabled)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step10  Close the iCA viewer.
                Viewer.CreateNewSesion();
                login.LoginIConnect(User, User);
                ExecutedSteps++;

                //step11 Load the study which has PR and Montage image (created in precondition) on Universal viewer from study list and verify that created PR and Montage are displayed correctly.
                studies.SearchStudy(AccessionNo: accession[7]);
                studies.SelectStudy("Accession", accession[7]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step11 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step11)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step12 Close the study and log out the User session.
                viewer.CloseBluRingViewer();
                login.Logout();
                bool step12 = viewer.IsElementVisible(By.CssSelector("input[id$='_LoginMasterContentPlaceHolder_Username']"));
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
        }

        /// <summary>
        /// Query and Retrieve reports from Unity Pacs Datasource
        /// </summary>
        public TestCaseResult Test_164333(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            RoleManagement role = new RoleManagement();
            UserManagement usermanagement = new UserManagement();
            ServiceTool serviceTool = new ServiceTool();
            Studies studies = new Studies();
            BluRingViewer viewer = new BluRingViewer();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String domain1 = "Domain1_" + new Random().Next(1, 1000);
                String role1 = "Role1_" + new Random().Next(1, 1000);
                String rad1 = Config.newUserName + new Random().Next(1, 1000);

                //Pre-condition
                //Enable report with all type from System/domain/Role level settings.
                // Enable Structure and Audio Reports.
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.Report);
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                if (!wpfobject.IsCheckBoxSelected(ServiceTool.EnableFeatures.Name.StructuredReports, 1))
                    wpfobject.SelectCheckBox(ServiceTool.EnableFeatures.Name.StructuredReports, 1);
                if (!wpfobject.IsCheckBoxSelected(ServiceTool.EnableFeatures.Name.AudioReports, 1))
                    wpfobject.SelectCheckBox(ServiceTool.EnableFeatures.Name.AudioReports, 1);
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableReports();
                //Enable PDF Report option
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.General);
                servicetool.ModifyEnableFeatures();
                servicetool.EnablePDFReport();
                servicetool.ApplyEnableFeatures();
                wpfobject.GetMainWindowByIndex(1);
                wpfobject.ClickButton("Yes", 1);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();
                //Enable Report from Domain and Rolemanagement
                login.LoginIConnect(adminUserName, adminPassword);
                var domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.CreateDomain(domain1, role1, datasources: null);
                domain.ClickSaveNewDomain();
                domain.SearchDomain(domain1);
                domain.SelectDomain(domain1);
                domain.ClickEditDomain();
                domain.SetCheckBoxInEditDomain("pdfreport", 0);
                domain.ClickSaveEditDomain();
                login.Navigate("Role Management");
                var rolemanagement = new RoleManagement();
                rolemanagement.SearchRole(role1, domain1);
                rolemanagement.SelectRole(role1);
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("pdfreport", 0);
                rolemanagement.ClickSaveEditRole();
                login.Navigate("UserManagement");
                usermanagement.CreateUser(rad1, domain1, role1);
                login.Logout();

                // Step 1 - Login to iCA webaccess as a created standard user.
                //login.LoginIConnect(rad1, rad1);
                login.LoginIConnect(rad1, rad1);
                ExecutedSteps++;

                //Step 2 - From ICA studylist, load the study on Universal viewer which has approved report from Unity Pacs.
                studies.SearchStudy(AccessionNo: AccessionList);
                studies.SelectStudy("Accession", AccessionList);
                viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                // Step 3 - Load the report from the exam list and verify the loaded report Content with dominator report.
                viewer.OpenReport_BR(1, "SR", accession: AccessionList);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                viewer.NavigateToReportFrame(reporttype: "SR");
                var ReportData = viewer.FetchUnityPACSReportData_BR(1);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                String PatientName = viewer.GetText("cssselector", BluRingViewer.p_PatientName);
                String PatientDOB = viewer.GetText("cssselector", BluRingViewer.span_PatientDOB);
                String PatientGender = viewer.GetText("cssselector", BluRingViewer.div_PatientGender).Substring(0, 1);
                String PatientMRN = viewer.GetText("cssselector", BluRingViewer.div_PatientID);
                var patientdobreport = Convert.ToDateTime(ReportData["DOB"].Trim());
                var patientdobdcm = Convert.ToDateTime(PatientDOB);
                string PatientName_1 = ReportData["Patient"].Trim();
                string ReportMRN = ReportData["MRN"].Trim();
                bool NameVerification = PatientName.Split(',').Length > 1 ? PatientName.Split(',')[0].Trim() == PatientName_1.Split(',')[0].Trim()
                    && PatientName.Split(',')[1].Trim() == PatientName_1.Split(',')[1].Trim() : PatientName.Equals(PatientName_1, StringComparison.InvariantCultureIgnoreCase);
                if (NameVerification && ReportData["Gender"].Equals(PatientGender) && ReportMRN.Equals(PatientMRN) && patientdobreport.Equals(patientdobdcm))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                // Step 4 - Close the study and log out from User
                viewer.CloseBluRingViewer();
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
        /// Changing Demographics from Unity Pacs
        /// </summary>
        public TestCaseResult Test_164334(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            var dominator = new Dominator();
            String AccessionList = null;
            String Lastname = null;
            String Firstname = null;
            String Middlename = null;
            String RefferingPhysicianList = null;
            String[] refferingPhysician = null;
            String firstname = null;
            String lastname = null;
            String middlename = null;
            String referingPhysicianName = null;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                Lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastNameList");
                Firstname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstNameList");
                Middlename = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "MiddleNameList");
                RefferingPhysicianList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RefferingPhysician");
                refferingPhysician = RefferingPhysicianList.Split(':');

                // Step 1 - Login to iCA webaccess as a created standard user.
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                // Step 2 - Load a study on Universal viewer from Unity Pacs datasource and verify the Patient and study details.
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: AccessionList);
                studies.SelectStudy("Accession", AccessionList);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step2_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                var PatientnameInViewer = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.p_PatientNamemedium).GetAttribute("innerHTML");
                var step2_2 = PatientnameInViewer.ToLower().Substring(0, 16).Equals(Lastname.ToLower() + ", " + Firstname.ToLower() + " " + Middlename.ToLower().Substring(0, 1)); 
                if (step2_1 && step2_2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 3 - Close the viewer and From Unity Pacs Dominator online tab, select the previously loaded study and click on Props icon
                viewer.CloseBluRingViewer();
                //login.Logout();
                dominator.LaunchDominator();
                dominator.loginToDominator();
                dominator.NavigateToTab(Dominator.online_Tab);
                wpfobject.SelectFromComboBox("103", "Accession #:");
                wpfobject.SetText("2", AccessionList);
                wpfobject.ClickButton(Dominator.refreshButton, 1);
                var textbox = wpfobject.GetTextbox("2");
                textbox.Focus();
                textbox.Click();
                System.Windows.Forms.SendKeys.SendWait("{HOME}");
                wpfobject.ClickButton(Dominator.PropsButton, 1);
                var examPropertiesPatient = wpfobject.GetMainWindowByTitle("Exam Properties (Exam ID = 00TPL4OF; Owning Location = 9888-1) for SINUS, STANLEY ANT");
                if (examPropertiesPatient != null)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 4 - Select Edit -> List -> search the same exam -> click Properties.
                examPropertiesPatient.Focus();
                wpfobject.ClickButton(Dominator.EditButton, 1);
                wpfobject.WaitTillLoad();
                var examProperties = wpfobject.GetMainWindowByTitle(Dominator.ExamPropertiesDialogue);
                examProperties.Focus();
                wpfobject.ClickButton("4584");
                wpfobject.WaitTillLoad();
                var selectPatientDialog = wpfobject.GetMainWindowByTitle(Dominator.SelectPatientDialogue);
                selectPatientDialog.Focus();
                wpfobject.SetText("2", Lastname + ", " + Firstname);
                Thread.Sleep(3000);
                System.Windows.Forms.SendKeys.SendWait("{HOME}");
                wpfobject.ClickButton(Dominator.PropertiesButton, 1);
                var patientPropertiesDialog = wpfobject.GetMainWindowByTitle(Dominator.PatientPropertiesDialogue);
                if (patientPropertiesDialog != null)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 5 - Click on edit to modify the Patient demographics like First Name, Middle name, Last name, DOB, Gender and save.
                //Close the Patient properties window.
                patientPropertiesDialog.Focus();
                wpfobject.ClickButton(Dominator.EditButton, 1);
                var editPatientRecord = wpfobject.GetMainWindowByTitle(Dominator.EditPatientRecordDialogue);
                editPatientRecord.Focus();
                firstname = wpfobject.GetTextbox("4533").Text;
                wpfobject.SetText("4533", Firstname + "_updated");
                middlename = wpfobject.GetTextbox("21176").Text;
                wpfobject.SetText("21176", middlename + "_updated");
                lastname = wpfobject.GetTextbox("4629").Text;
                wpfobject.SetText("4629", Lastname + "_updated");
                wpfobject.ClickButton(Dominator.SaveButton, 1);
                wpfobject.WaitTillLoad();
                patientPropertiesDialog.Focus();
                wpfobject.ClickButton(Dominator.CloseButton, 1);
                wpfobject.WaitTillLoad();
                selectPatientDialog.Focus();
                wpfobject.ClickButton(Dominator.OKButton, 1);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                // Step 6 - From Exam properties window, modify the study related information's like Study date, Referring Doctors, update notes (description) and apply then Ok.
                examProperties.Focus();
                referingPhysicianName = wpfobject.GetTextbox("4529").Text;
                wpfobject.ClickButton("4579");
                wpfobject.WaitTillLoad();
                var SelectReferringDoctor = wpfobject.GetMainWindowByTitle(Dominator.SelectReferrringDoctorDialogue);
                SelectReferringDoctor.Focus();
                wpfobject.SetText("2", refferingPhysician[1]);
                Thread.Sleep(3000);
                System.Windows.Forms.SendKeys.SendWait("{HOME}");
                wpfobject.ClickButton(Dominator.OKButton, 1);
                wpfobject.WaitTillLoad();
                examProperties.Focus();
                var UpdatedreferingPhysicianName = wpfobject.GetTextbox("4529").Text;
                wpfobject.ClickButton(Dominator.OKButton, 1);
                wpfobject.WaitTillLoad();
                examPropertiesPatient.Focus();
                wpfobject.ClickButton(Dominator.CloseButton, 1);
                dominator.CloseDominator();
                ExecutedSteps++;

                //Step 7 - Again load the same study on Universal viewer from Unity Pacs datasource and verify the updated Patient and study details are reflected on Universal viewer.
                studies.SearchStudy(AccessionNo: AccessionList);
                var dominatorpatientname = lastname + "_updated" + ", " + firstname + "_updated" + " " + "a";
                var patientDetails = studies.GetMatchingRow("Accession", AccessionList);
                studies.SelectStudy("Accession", AccessionList);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PatientnameInViewer = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.p_PatientNamemedium).GetAttribute("innerHTML");                
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (patientDetails["Patient Name"].ToLower().Trim().Equals(dominatorpatientname.ToLower()) && step7 &&
                    patientDetails["Refer. Physician"].ToLower().Trim().Equals(refferingPhysician[1].ToLower().Substring(0, 14))
                    && PatientnameInViewer.ToLower().Substring(0, 32).Equals(dominatorpatientname.ToLower().Substring(0, 32)))
                    result.steps[ExecutedSteps].StepPass();
                else
                {
                    result.steps[ExecutedSteps].StepFail();
					Logger.Instance.InfoLog("The expected patient name in search results is " + patientDetails["Patient Name"].ToLower() + " and the name in dominator is " + dominatorpatientname.ToLower());
                    Logger.Instance.InfoLog("The expected Refer. Physician in search results is " + patientDetails["Refer. Physician"].ToLower() + " and the name in dominator is " + refferingPhysician[1].ToLower());
                    Logger.Instance.InfoLog("The expected patient name in viewer is " + PatientnameInViewer.ToLower().Substring(0, 26) + " and the name in dominator is " + dominatorpatientname.ToLower());
                }

                // Step 8 - Close the study and logout from User
                viewer.CloseBluRingViewer();
                login.Logout();
                dominator.CloseDominator();
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
            finally
            {
                dominator.LaunchDominator();
                dominator.loginToDominator();
                dominator.NavigateToTab(Dominator.online_Tab);
                wpfobject.SelectFromComboBox("103", "Accession #:");
                wpfobject.SetText("2", AccessionList);
                wpfobject.ClickButton(Dominator.refreshButton, 1);
                var textbox = wpfobject.GetTextbox("2");
                textbox.Focus();
                textbox.Click();
                System.Windows.Forms.SendKeys.SendWait("{HOME}");
                wpfobject.ClickButton(Dominator.PropsButton, 1);
                var examPropertiesPatient = wpfobject.GetMainWindowByTitle("Exam Properties (Exam ID = 3V1YSQGE; Owning Location = 9888-1) for SINUS,STANLEY ANT");
                examPropertiesPatient.Focus();
                wpfobject.ClickButton(Dominator.EditButton, 1);
                wpfobject.WaitTillLoad();
                var examProperties = wpfobject.GetMainWindowByTitle(Dominator.ExamPropertiesDialogue);
                examProperties.Focus();
                wpfobject.ClickButton("4584");
                wpfobject.WaitTillLoad();
                var selectPatientDialog = wpfobject.GetMainWindowByTitle(Dominator.SelectPatientDialogue);
                selectPatientDialog.Focus();
                wpfobject.SetText("2", Lastname + ", " + Firstname);
                Thread.Sleep(3000);
                System.Windows.Forms.SendKeys.SendWait("{HOME}");
                wpfobject.ClickButton(Dominator.PropertiesButton, 1);
                var patientPropertiesDialog = wpfobject.GetMainWindowByTitle(Dominator.PatientPropertiesDialogue);
                patientPropertiesDialog.Focus();
                wpfobject.ClickButton(Dominator.EditButton, 1);
                var editPatientRecord = wpfobject.GetMainWindowByTitle(Dominator.EditPatientRecordDialogue);
                editPatientRecord.Focus();
                wpfobject.SetText("4533", firstname);
                wpfobject.SetText("21176", middlename);
                wpfobject.SetText("4629", lastname);
                wpfobject.ClickButton(Dominator.SaveButton, 1);
                wpfobject.WaitTillLoad();

                patientPropertiesDialog.Focus();
                wpfobject.ClickButton(Dominator.CloseButton, 1);
                wpfobject.WaitTillLoad();

                selectPatientDialog.Focus();
                wpfobject.ClickButton(Dominator.OKButton, 1);
                wpfobject.WaitTillLoad();

                examProperties.Focus();
                wpfobject.ClickButton("4579");
                wpfobject.WaitTillLoad();
                var SelectReferringDoctor = wpfobject.GetMainWindowByTitle(Dominator.SelectReferrringDoctorDialogue);
                SelectReferringDoctor.Focus();
                wpfobject.SetText("2", referingPhysicianName);
                Thread.Sleep(3000);
                System.Windows.Forms.SendKeys.SendWait("{HOME}");
                wpfobject.ClickButton(Dominator.OKButton, 1);
                wpfobject.WaitTillLoad();

                examProperties.Focus();
                var UpdatedreferingPhysicianName = wpfobject.GetTextbox("4529").Text;
                wpfobject.ClickButton(Dominator.OKButton, 1);
                wpfobject.WaitTillLoad();

                examPropertiesPatient.Focus();
                wpfobject.ClickButton(Dominator.CloseButton, 1);

                dominator.CloseDominator();
            }
        }
    }
}
