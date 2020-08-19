using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.eHR;
using Selenium.Scripts.Pages.MergeServiceTool;
using Selenium.Scripts.Reusable.Generic;
using Selenium.Scripts.Pages.iConnect;
using TestStack.White.UIItems;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.Finders;
using System.Threading;
using System.Data;
using System.Xml;
using OpenQA.Selenium.Interactions;
using Selenium.Scripts.Pages.MPAC;
using ComboBox = TestStack.White.UIItems.ListBoxItems.ComboBox;
using Window = TestStack.White.UIItems.WindowItems.Window;
using Dicom.Network;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using Dicom.Network;


namespace Selenium.Scripts.Tests
{
    class BluringInternationalization : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public EHR ehr { get; set; }
        public ExamImporter ei { get; set; }
        public WpfObjects wpfobject { get; set; }
        public ServiceTool servicetool { get; set; }
        public UserPreferences userpref { get; set; }

        Studies studies { get; set; }
        public BasePage basepage { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public BluRingViewer bluringviewer { get; set; }
        public StudyViewer studyviewer { get; set; }

        DomainManagement domainmanagement;
        UserManagement usermanagement;
        RoleManagement rolemanagement;
        Inbounds inbounds = null;
        Outbounds outbounds = null;
        Patients patients;
        String ICA_MappingFilePath = Config.TestSuitePath + Path.DirectorySeparatorChar + Config.ica_Mappingfilepath;
        String BluringViewer_MappingFilePath = Config.BluringViewer_Mappingfilepath;

        //Global variables
        string dataSourceName = "DCM4CHEE";
        string specificCharacterSet = @",specificCharacterSet\";
        string DSAttribute = string.Empty;
        string ExcludedAttribute = string.Empty;

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        public BluringInternationalization(String classname)
        {
            login = new Login();
            BasePage.InitializeControlIdMap();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            wpfobject = new WpfObjects();
            servicetool = new ServiceTool();
            ehr = new EHR();
            ei = new ExamImporter();
            basepage = new BasePage();
            mpaclogin = new MpacLogin();
            userpref = new UserPreferences();
            bluringviewer = new BluRingViewer();
            studies = new Studies();
        }

        /// <summary>
        /// Ability of replacing static images by Locale
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161547(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            UserPreferences userpreferences = new UserPreferences();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            servicetool = new ServiceTool();

            String savedLocale = Config.Locale;
            Localization localization = new Localization();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String language = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Languages");
                string dataSource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSource");

                // set test locale
                String locale = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Locale");
                localization.UpdateLocalization(locale);

                //Step 1 - Launch iCA application, select french language, and login as any valid user
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword, language);
                ExecutedSteps++;

                //Step 2 - Search for any study and launch it in Universal viewer
                studies = (Studies)login.Navigate("Studies", 1, "Studies");
                studies.SearchStudy(AccessionNo: accession, Datasource: dataSource);
                studies.SelectStudy(GetStudyGridColName("Accession"), accession);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step 3 - Verify all the information displayed in the study panel and global toolbar are in selected language
                bool IsGlobalToolbarLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "GlobalToolbar", viewer: "bluring");
                if (IsGlobalToolbarLocalized)
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

                //Step 4 - Verify the IBM Logomark and IBM logo are displayed properly for French language
                var logo = bluringviewer.GetElement(SelectorType.CssSelector, "div.logoWrapper");
                var step = result.steps[++ExecutedSteps];
                step.SetPath(testid, ExecutedSteps);
                if (bluringviewer.CompareImage(step, logo))
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

                bluringviewer.CloseBluRingViewer();
                login.Logout();

                //Step 5 - From iCA Server, navigate to <BluRing root directory>\WebAccess\BluRingViewer\assets\images\fr\ and temporarely change file names for the images:
                //         merge-logomark-orange.svgz to _merge-logomark-orange.svgz
                //         merge-logo-orange.svgz to _merge-logo-orange.svgz
                String mergeLogomarkOld = @"C:\WebAccess\WebAccess\BluRingViewer\assets\images\" + Config.Locale.Split('-')[0].ToLower() + "\\merge-logomark-orange.svgz";
                String mergeLogoOld = @"C:\WebAccess\WebAccess\BluRingViewer\assets\images\" + Config.Locale.Split('-')[0].ToLower() + "\\merge-logo-orange.svgz";
                String mergeLogomarkNew = @"C:\WebAccess\WebAccess\BluRingViewer\assets\images\" + Config.Locale.Split('-')[0].ToLower() + "\\_merge-logomark-orange.svgz";
                String mergeLogoNew = @"C:\WebAccess\WebAccess\BluRingViewer\assets\images\" + Config.Locale.Split('-')[0].ToLower() + "\\_merge-logo-orange.svgz";
                File.Move(mergeLogomarkOld, mergeLogomarkNew);
                File.Move(mergeLogoOld, mergeLogoNew);
                if (File.Exists(mergeLogomarkNew) && File.Exists(mergeLogoNew))
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

                //Step 6 - Restart services, clear browser cache, and load the same study again in Universal viewer
                servicetool.LaunchServiceTool();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseConfigTool();

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword, language);
                studies = (Studies)login.Navigate("Studies", 1, "Studies");
                studies.SearchStudy(AccessionNo: accession, Datasource: dataSource);
                studies.SelectStudy(GetStudyGridColName("Accession"), accession);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();

                //verify that IBM Logomark and IBM logo is NOT visible on the global toolbar 
                logo = bluringviewer.GetElement(SelectorType.CssSelector, "div.logoWrapper");
                step = result.steps[++ExecutedSteps];
                step.SetPath(testid, ExecutedSteps);
                if (bluringviewer.CompareImage(step, logo))
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

                //Step 7 - Verify all the information displayed in the study panel and global toolbar are in selected language [French]
                IsGlobalToolbarLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "GlobalToolbar", viewer: "bluring");
                if (IsGlobalToolbarLocalized)
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


                //Step8: From iCA Server, change the file names back to the original names in WebAccess\BluRingViewer\assets\images\fr\ 
                File.Move(mergeLogomarkNew, mergeLogomarkOld);
                File.Move(mergeLogoNew, mergeLogoOld);
                if (File.Exists(mergeLogomarkOld) && File.Exists(mergeLogoOld))
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

                //Step9: Restart services, clear browser cache, and load the same study again in enterprise viewer
                servicetool.LaunchServiceTool();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseConfigTool();

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword, language);

                studies = (Studies)login.Navigate("Studies", 1, "Studies");

                studies.SearchStudy(AccessionNo: accession, Datasource: dataSource);
                studies.SelectStudy(GetStudyGridColName("Accession"), accession);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();

                //verify that IBM Logomark and IBM logo is NOT visible on the global toolbar 
                logo = bluringviewer.GetElement(SelectorType.CssSelector, "div.logoWrapper");
                step = result.steps[++ExecutedSteps];
                step.SetPath(testid, ExecutedSteps);
                if (bluringviewer.CompareImage(step, logo))
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

                bluringviewer.CloseBluRingViewer();
                login.Logout();

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                GC.Collect();
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Close browser
                Driver.Manage().Cookies.DeleteAllCookies();
                this.CloseBrowser();

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Localization in Modality Filter
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_141954(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            UserPreferences userpreferences = new UserPreferences();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            servicetool = new ServiceTool();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data        
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String Modality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String StudyDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDescription");

                //Step 1 - Login to WebAccess site with any privileged user. (e.g., rad/rad)
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                ExecutedSteps++;

                //Step 2 - From the Studies tab, search for a study with multiple priors and different modalities. Load the study into the Enterprise Viewer.
                //         (If recommended dataset is used, search for patient named "DX 1, 15 PRIORS", and open one the patient's study)
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(LastName: LastName, Modality: Modality, AccessionNo: Accession, Description: StudyDescription);
                PageLoadWait.WaitForSearchLoad();
                studies.SelectStudy("Accession", Accession);
                BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step 3 - Verify that the patient related studies and the current primary study (in the Study Panel) are displayed in the Results list as cards with study details.
                var viewport = bluringviewer.GetElement(SelectorType.CssSelector, BluRingViewer.div_studypanel);
                var step3 = result.steps[++ExecutedSteps];
                step3.SetPath(testid, ExecutedSteps);
                if (bluringviewer.CompareImage(step3, viewport))
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

                //Step 4 - Click on 'User Settings' tool and select 'LANG-FRANCAIS'
                bluringviewer.SelectUserSetting(2, 2);
                string GenderUI = Driver.FindElements(By.CssSelector(BluRingViewer.span_PatientDetailsLabel))[2].Text;
                if (bluringviewer.DemoDetailLabels()[0].Text.Equals("Date de naissance") &&
                   bluringviewer.DemoDetailLabels()[1].Text.Equals("âge inconnu") &&
                   GenderUI.Equals("Mâle") &&
                   bluringviewer.GlobalToolbarPanel()[0].Text.Equals("EXAMENS") &&
                   bluringviewer.GlobalToolbarPanel()[2].Text.Equals("SORTIE"))
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

                //Step 5 - Verify the following options available in the Exam List Panel should be displayed in the target language i.e. Francias.
                //         -Exam List Text, -History Text, -Exam List Filters (Modality & Sort By)
                String ExamListText = Driver.FindElement(By.CssSelector(".patientHistoryExamListTitle")).Text;
                String HistoryText = Driver.FindElement(By.CssSelector(".patientHistorySubTitle")).Text;
                String ModalityText = bluringviewer.OperationListContainer()[0].Text;
                String SortByText = bluringviewer.OperationListContainer()[1].Text;
                if (ExamListText.Equals("LISTE D'EXAMEN") && HistoryText.Equals("Histoire") &&
                    ModalityText.Equals("Modalité") && SortByText.Equals("Trier par"))
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

                //Step 6 - Click on 'Modality' dropdown and Verify the following texts are available under Modality filter should be displayed in the target language i.e. Francias.
                //         -Modality, -Clear All, -All
                bluringviewer.OpenModalityFilter();
                Thread.Sleep(5000);
                ModalityText = Driver.FindElement(By.CssSelector(".md-select-custom-label")).Text;
                String ClearAllText = Driver.FindElement(By.CssSelector(".md-select-anchor")).Text;
                String AllText = Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_ele))[0].Text;
                if (ModalityText.Equals("modalité") && ClearAllText.Equals("Trier par") && AllText.Equals("tout"))
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
                bluringviewer.CloseModalityFilter();

                //Step 7 - Mouse-hover over a study card detail. Verify the Birthdate & Sex fields should be displayed in the target language i.e. Francais:
                ExecutedSteps++;

                bluringviewer.CloseBluRingViewer();
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                GC.Collect();
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Close browser
                Driver.Manage().Cookies.DeleteAllCookies();
                this.CloseBrowser();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// French Text Translation - User Settings tool
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_148949(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            UserPreferences userpreferences = new UserPreferences();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            servicetool = new ServiceTool();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data        
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String Modality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");

                //Precondition:
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                userpref.OpenUserPreferences();
                userpref.SwitchToUserPrefFrame();
                if (!userpref.EnableConnectionTestTool().Selected)
                    userpref.EnableConnectionTestTool().Click();
                userpref.CloseUserPreferences();
                login.Logout();

                //Step 1 - Login to WebAccess site with any privileged user and go to Studies tab then search and load any study which has all patient information and report into the viewer using "view exam" button.
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession, Datasource: login.GetHostName(Config.EA1));
                PageLoadWait.WaitForSearchLoad();
                studies.SelectStudy("Accession", Accession);
                BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step 2 - Search for User Settings at the top right corner in the Viewer page and 
                //         click on "user settings" icon then ensure LANG- ENGLISH is selected by default in user controls 
                String ClassName = GetElementAttribute("cssselector", BluRingViewer.userSettings_Icon, "class");
                if (!(ClassName.Contains("isActive")))
                {
                    Driver.FindElement(By.CssSelector(BluRingViewer.userSettings_Icon)).Click();
                    Thread.Sleep(Config.ms_minTimeout);
                }
                String English = GetText("cssselector", ".globalSettingPanel div ul:nth-of-type(2) li:nth-of-type(1)").Trim();
                String French = GetText("cssselector", ".globalSettingPanel div ul:nth-of-type(2) li:nth-of-type(1)").Trim();
                if (English.Replace("\r\n", "").Equals("✔LANG - ENGLISH"))
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
                //Close usersettings
                Driver.FindElement(By.CssSelector(BluRingViewer.userSettings_Icon)).Click();
                Thread.Sleep(Config.ms_minTimeout);

                //Step 3 - Verify that the Global Toolbar menus (Patient Info,EXAMS, Show/Hide Tool ,EXIT) text should be in English by default
                bluringviewer.SelectUserSetting(2, 1);
                bool IsGlobalToolbarLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "GlobalToolbar", path: "English", viewer: "bluring");
                if (IsGlobalToolbarLocalized)
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

                //TODO - Help text is not in json file                
                //Step 4 - Verify that the tool tip text for all Global Toolbar menus (Patient Info,EXAMS, Show/Hide Tool ,Collaboration, Help, Settings and EXIT) should be in English by default
                //Covered in Step #3
                String HelpText = GetElement(SelectorType.CssSelector, "blu-ring-help div div.toolIconBoxWrapper").GetAttribute("title");
                if (HelpText.Equals("Help"))
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

                //Step 5 - Click on Network Connection tool and ensure that the connection details text should displayed in English                                
                GetElement(SelectorType.CssSelector, BluRingViewer.div_NetworkConnection).Click();
                Thread.Sleep(Config.ms_minTimeout);
                var IsNetworkConnectionLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "NetworkConnection", path: "English", viewer: "bluring");
                if (IsNetworkConnectionLocalized)
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
                //Close NC
                GetElement(SelectorType.CssSelector, BluRingViewer.div_NetworkConnection).Click();
                Thread.Sleep(Config.ms_minTimeout);

                //Step 6 - Click on "User Settings" tool at the top right corner in the Viewer page and select LANG – FRANÇAIS
                bluringviewer.SelectUserSetting(2, 2);
                ExecutedSteps++;

                //Step 7 - Verify that the Global Toolbar menus (Patient Info,EXAMS, Show/Hide Tool ,EXIT) text should changed to French.                
                IsGlobalToolbarLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "GlobalToolbar", viewer: "bluring");
                if (IsGlobalToolbarLocalized)
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

                //Help is not localized, this step fails
                //Step 8 - Verify that the tool tip text for all Global Toolbar menus (Patient Info,EXAMS, Show/Hide Tool ,Collaboration, Help, Settings and EXIT) should changed to French.
                //Covered in Step #7
                HelpText = GetElement(SelectorType.CssSelector, "blu-ring-help div div.toolIconBoxWrapper").GetAttribute("title");
                if (HelpText.Equals("aider"))
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

                //Step 9 - Click on Network Connection tool and verify the connection details text are should changed to French.
                GetElement(SelectorType.CssSelector, BluRingViewer.div_NetworkConnection).Click();
                Thread.Sleep(Config.ms_minTimeout);
                IsNetworkConnectionLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "NetworkConnection", viewer: "bluring");
                if (IsNetworkConnectionLocalized)
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

                //Step 10 - Verify the Merge LOGO should remain same
                var logo = bluringviewer.GetElement(SelectorType.CssSelector, BluRingViewer.div_mergeLogo);
                var step10 = result.steps[++ExecutedSteps];
                step10.SetPath(testid, ExecutedSteps);
                if (bluringviewer.CompareImage(step10, logo))
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

                //This step will fail - As 'All' is not getting localized until it refreshes
                //Step 11 - In Exam List panel,verify that the EXAM LIST, Modality and Sort By text should be changed to French.
                String ExamListText = Driver.FindElement(By.CssSelector(".patientHistoryExamListTitle")).Text;
                String ModalityText = bluringviewer.OperationListContainer()[0].Text;
                String SortByText = bluringviewer.OperationListContainer()[1].Text;
                if (ExamListText.Equals(ReadDataFromJsonFile(Localization.PatientHistory, "PatientHistory.Title")) &&
                    ModalityText.Equals(ReadDataFromJsonFile(Localization.PatientHistory, "PatientHistory.RelatedStudy.Filters.Modality")) &&
                    SortByText.Equals(ReadDataFromJsonFile(Localization.PatientHistory, "PatientHistory.RelatedStudy.Filters.SortBy.Title")))
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

                //Step 12 - In Modality filter, verify that the ‘All’ text in Modality filter should be displayed in French 
                bluringviewer.OpenModalityFilter();
                Thread.Sleep(5000);
                String AllText = Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_ele))[0].Text;
                if (AllText.Equals(ReadDataFromJsonFile(Localization.MultiselectFilter, "MultiSelectFilter.All")))
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

                //TODO - Fail JIRA - BLU-1124 
                //Step 13 - Verify that the ‘Modality’ text should be changed to French text when the user opens the Modality filter dropdown
                ModalityText = Driver.FindElement(By.CssSelector(".md-select-custom-label")).Text;
                if (ModalityText.Equals("modalité"))
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
                bluringviewer.CloseModalityFilter();

                //Step 14 - Mouse hover on any study info card and verify that the Study card tool tip text should be displayed in French text.
                result.steps[++ExecutedSteps].status = "No Automation";

                //Step 15 - Click on Show/Hide tool and verify that the Show/Hide tool options text should be displayed in French
                ////Covered in Step #7
                ExecutedSteps++;

                //Step 16 - In active series viewport, Right click on mouse button and verify that the Floating toolbox tools label are should be changed to French.
                result.steps[++ExecutedSteps].status = "No Automation";

                //Step 17 - Verify that all tools tool tip should be displayed in French.
                result.steps[++ExecutedSteps].status = "No Automation";

                //TODO - In User Settings Default, Green and Orange not localized, marking as fail
                //Step 18 - In User Settings tool, all options should displayed in French.
                Driver.FindElement(By.CssSelector("blu-ring-global-settings div div.toolIconBoxWrapper")).Click();
                Thread.Sleep(Config.ms_minTimeout);
                bool IsUserSettingsLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "UserSettings", viewer: "bluring");
                if (IsUserSettingsLocalized)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Close viewer
                bluringviewer.CloseBluRingViewer();
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                GC.Collect();
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Close browser
                Driver.Manage().Cookies.DeleteAllCookies();
                this.CloseBrowser();

                //Return Result
                return result;
            }
            finally
            {
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                userpref.OpenUserPreferences();
                userpref.SwitchToUserPrefFrame();
                if (userpref.EnableConnectionTestTool().Selected)
                    userpref.EnableConnectionTestTool().Click();
                userpref.CloseUserPreferences();
                login.Logout();

            }
        }

        /// <summary>
        /// Verify that all UI Elements in Global Toolbar section are localized
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_163430(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            String savedLocale = Config.Locale;
            Localization localization = new Localization();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            
            try
            {

                //Fetch required Test data        
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String Language = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Languages");
                string dataSource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSource");

                //Step 1 - Login to WebAccess site with any privileged user and required language
                ++ExecutedSteps;
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                //Enable 3D View checkbox
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.ClickEditDomain();
                domainmanagement.Enable3DView();
                domainmanagement.ClickSaveNewDomain();
                PageLoadWait.WaitForPageLoad(20);

                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectRole("SuperRole");
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("3dview", 0);
                rolemanagement.ClickSaveRole();
                PageLoadWait.WaitForPageLoad(20);

                //Enable connections in user options
                userpref.OpenUserPreferences();
                userpref.SwitchToUserPrefFrame();
                if (!userpref.EnableConnectionTestTool().Selected)
                    userpref.EnableConnectionTestTool().Click();
                userpref.CloseUserPreferences();

                // set test locale
                String locale = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Locale");
                localization.UpdateLocalization(locale);

                //Login to WebAccess site with any privileged user and required language
                login.Logout();
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword, Language);

                //Step 2 - Load any study which has all patient information and report into the viewer using "view exam" button.
                ++ExecutedSteps;
                studies = (Studies)login.Navigate("Studies", 1, "Studies");
                studies.SearchStudy(AccessionNo: Accession, Datasource: dataSource);
                studies.SelectStudy(GetStudyGridColName("Accession"), Accession);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();

                //Step 3 - Validate GlobalToolBar icons to ensure that all ToolTips of the icons are localized.
                ++ExecutedSteps;
                bool IsGlobalToolbarLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "GlobalToolbar", path: "other", viewer: "bluring");
                if (IsGlobalToolbarLocalized)
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

                //Step 4 - Select Option in User Settings and verify dropdown is localized aexcept the Colour options Default, Green and Orange
                ++ExecutedSteps;
                bool IsUserSettingsLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "UserSettings", path: "other", viewer: "bluring");
                if (IsUserSettingsLocalized)
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

                //Step 5 - Select Collaboration Window and elements are localized.
                ++ExecutedSteps;
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingCollaboration).Click();
                bool IsCollaboratgionLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "Collaboration", path: "other", viewer: "bluring");
                if (IsCollaboratgionLocalized)
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
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingCollaboration).Click();

                //Step 6 - Validate That Network Connection Window elements are localized.
                ++ExecutedSteps;
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingNetworkConnection).Click();

                bool IsNetWorkConnectionLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "NetworkConnection", path: "other", viewer: "bluring");
                if (IsNetWorkConnectionLocalized)
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
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingNetworkConnection).Click();

                //Step 7 - Validate That Show/Hide drop down menu items are localized.
                ++ExecutedSteps;
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingShowHide).Click();
                bool IsNetShowHideLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "ShowHide", path: "other", viewer: "bluring");
                if (IsNetShowHideLocalized)
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
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingShowHide).Click();

                //Step 8 - Validate That Help Panel dropdown items are localized.
                ++ExecutedSteps;
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingHelp).Click();
                bool IsHelpLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "HelpPanel", path: "other", viewer: "bluring");
                if (IsHelpLocalized)
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
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingHelp).Click();

                //Close viewer
                bluringviewer.CloseBluRingViewer();
                login.Logout();

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                GC.Collect();
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Close browser
                Driver.Manage().Cookies.DeleteAllCookies();
                this.CloseBrowser();

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Verify that all UI Elements in Exam List section are localized
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_163434(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            String savedLocale = Config.Locale;
            Localization localization = new Localization();

            try
            {

                //Fetch required Test data        
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String Language = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Languages");
                string dataSource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSource");

                //Step 1 - Login to WebAccess site with any privileged user and required language
                ++ExecutedSteps;
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                //Enable 3D View checkbox
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.ClickEditDomain();
                domainmanagement.Enable3DView();
                domainmanagement.ClickSaveNewDomain();
                PageLoadWait.WaitForPageLoad(20);

                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.ShowRolesFromDomainDropDown();
                rolemanagement.SelectDomainfromDropDown("SuperAdminGroup");
                rolemanagement.SelectRole("SuperRole");
                PageLoadWait.WaitForPageLoad(20);
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("3dview", 0);
                rolemanagement.ClickSaveRole();
                PageLoadWait.WaitForPageLoad(20);

                //Enable connections in user options
                userpref.OpenUserPreferences();
                userpref.SwitchToUserPrefFrame();
                if (!userpref.EnableConnectionTestTool().Selected)
                    userpref.EnableConnectionTestTool().Click();
                userpref.CloseUserPreferences();

                // set test locale
                String locale = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Locale");
                localization.UpdateLocalization(locale);

                //Login to WebAccess site with any privileged user and required language
                login.Logout();
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword, Language);

                //Step 2 - Load any study which has all patient information and report into the viewer using "view exam" button.
                ++ExecutedSteps;
                studies = (Studies)login.Navigate("Studies", 1, "Studies");
                studies.SearchStudy(AccessionNo: Accession, Datasource: dataSource);
                studies.SelectStudy(GetStudyGridColName("Accession"), Accession);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();

                //Step 3 - Validate that labels like EXAM LIST, HISTORY, Modality and Sort By are localized
                ++ExecutedSteps;
                bool IsPatientHistoryLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "PatientHistory", path: "other", viewer: "bluring");

                //Validate "(X) out of (Y)"
                String jsonOutOF = ReadDataFromJsonFile(Localization.DefaultLangJsonPath + "locale-patient-history-" + Config.Locale.ToLower() + ".json", "PatientHistory.RelatedStudy.StudyCountText");
                String outOfTextRetrieved =      Driver.FindElement(By.CssSelector(BluRingViewer.examCardStudyCountText)).GetAttribute("innerText");

                String expectedOutof;
                expectedOutof = Regex.Replace(jsonOutOF, @"{X}|{Y}", "").Trim();
                String outOfText = Regex.Replace(outOfTextRetrieved, @"[0-9]", "").Trim();
                bool outOfLocalized = expectedOutof.Equals(outOfText);

                if (IsPatientHistoryLocalized && outOfLocalized)
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

                //Step 4 - Validate Options "All", Title "modality" and "Clear all" are localized
                ++ExecutedSteps;
                bluringviewer.OpenModalityFilter();
                bool IsPatientHistoryModalityLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "PatientHistoryModality", path: "other", viewer: "bluring");
                if (IsPatientHistoryModalityLocalized)
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
                bluringviewer.CloseModalityFilter();

                //Step 5 - Validate Options Sort By, Date-Newest, Date-Oldest and Modality Type are Localized
                ++ExecutedSteps;

                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");

                bluringviewer.OpenSortDorpdown();
                bool IsPatientHistorySortByLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "PatientHistorySortBy", path: "other", viewer: "bluring");
                if (IsPatientHistorySortByLocalized)
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
                GetElement(SelectorType.CssSelector, BluRingViewer.cdk_OverlayContainer).Click();

                //Step 6 - Validate that the ToolTip(Birthdate:,MRN:,IPID:,Acc#:,Datasource:) in ExamList Card are localized. 
                ++ExecutedSteps;

                try
                {
                    // NOTE Using Try/Catch since code not able to correctly retrieve tooltip from title attribute.  
                    String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                    String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                    String Lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                    String Firstname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                    String gender = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Gender");
                    String dob = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DOB");
                    String modality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                    String studydesc = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDescription"));
                    String studydate = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDate"));
                    String ipid = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "IPID"));
                    String Datasource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSource");
                    String birthdate = ReadDataFromJsonFile(Localization.PatientHistory, "PatientHistory.RelatedStudy.ToolTip.DOB"); //Birthdate:
                    String sex = ReadDataFromJsonFile(Localization.PatientHistory, "PatientHistory.RelatedStudy.ToolTip.Sex"); //Sex:
                    String mrn = ReadDataFromJsonFile(Localization.PatientHistory, "PatientHistory.RelatedStudy.ToolTip.MRN"); //MRN:
                    String ipID = ReadDataFromJsonFile(Localization.PatientHistory, "PatientHistory.RelatedStudy.ToolTip.IPID");
                    String acc = ReadDataFromJsonFile(Localization.PatientHistory, "PatientHistory.RelatedStudy.ToolTip.AccessionNo");
                    String datasrc = ReadDataFromJsonFile(Localization.PatientHistory, "PatientHistory.RelatedStudy.ToolTip.DataSource");

                    var prior = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors))[0];
                    bluringviewer.HoverElement(prior);
                    var tooltip = prior.GetAttribute("title");
                    var expected_tooltip = Lastname + ", " + Firstname + Environment.NewLine +
                                           birthdate + dob + "  " + sex + gender + Environment.NewLine +
                                           mrn + PatientID + Environment.NewLine +
                                           ipID + ipid + Environment.NewLine +
                                           Environment.NewLine +
                                           studydate + Environment.NewLine +
                                           studydesc + Environment.NewLine +
                                           modality + Environment.NewLine +
                                           acc + AccessionID + Environment.NewLine + Environment.NewLine + datasrc + Datasource + Environment.NewLine;
                    Logger.Instance.InfoLog("The Expected ToolTip is--" + expected_tooltip);
                    Logger.Instance.InfoLog("The Actual ToolTip is--" + tooltip);
                    if (tooltip.Equals(expected_tooltip))
                    {
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
                catch (Exception e)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Close viewer
                bluringviewer.CloseBluRingViewer();
                login.Logout();

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                GC.Collect();
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Close browser
                Driver.Manage().Cookies.DeleteAllCookies();
                this.CloseBrowser();

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

                //Return Result
                return result;
            }
        }


        /// <summary>
        /// Verify that Multilangual texts are applied in the viewer through 'Add Text' Tool
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_163433(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            String savedLocale = Config.Locale;
            Localization localization = new Localization();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                // Fetch required Test data        
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String language = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Languages");

                // set test locale
                String locale = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Locale");
                localization.UpdateLocalization(locale);
                
                // Step 1: Launch the iCA application and select target localized language from the dropdown. Login to application.
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword, language);
                ExecutedSteps++;

                // Step 2: Navigate to Study tab and search for a study. Launch study in the Universal viewer.
                studies = (Studies)login.Navigate("Studies", 1, "Studies");
                studies.SearchStudy(AccessionNo: accession);
                studies.SelectStudy(GetStudyGridColName("Accession"), accession);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                // Step 3: Add Text to image and verify text is localized in target language.
                String addTool = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.AddText.Tooltip");
                bluringviewer.OpenViewerToolsPOPUp();
                bluringviewer.SelectViewerToolByName(addTool);
                BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_viewport)).GetCssValue("cursor").Equals("crosshair");
                bluringviewer.ApplyTool_AddText(language);
                IWebElement Viewport = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_viewport));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (studies.CompareImage(result.steps[ExecutedSteps], Viewport, totalImageCount: 1))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Close viewer
                bluringviewer.CloseBluRingViewer();
                login.Logout();

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

                //Report Result
                result.FinalResult(ExecutedSteps);

                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                GC.Collect();
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Close browser
                Driver.Manage().Cookies.DeleteAllCookies();
                this.CloseBrowser();

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

                //Return Result
                return result;
            }
            finally
            {
                Logger.Instance.InfoLog("------------- Test_163433 Complete ----------------");
            }
        }



        /// <summary>
        /// Verify that all UI Elements in Email Study Window are localized
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_163435(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            String savedLocale = Config.Locale;
            Localization localization = new Localization();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data        
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String language = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Languages");
                string dataSource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSource");
                String dateTime = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDate");
                String[] studyDateTimeArray = dateTime.Split(';');


                // Enable Email Study from the Service Tool and restart services
                servicetool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                servicetool.WaitWhileBusy();
                servicetool.EnableEmailStudy();
                servicetool.ApplyEnableFeatures();
                servicetool.AcceptDialogWindow();
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                servicetool.WaitWhileBusy();
                servicetool.CloseConfigTool();

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("emailstudy", 0);
                domainmanagement.ClickSaveNewDomain();
                PageLoadWait.WaitForPageLoad(20);

                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");

                rolemanagement.ShowRolesFromDomainDropDown();
                rolemanagement.SelectDomainfromDropDown("SuperAdminGroup");
                rolemanagement.SelectRole("SuperRole");
                PageLoadWait.WaitForPageLoad(20);
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("email", 0);
                rolemanagement.ClickSaveRole();
                PageLoadWait.WaitForPageLoad(20);
                login.Logout();

                // set test locale
                String locale = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Locale");
                localization.UpdateLocalization(locale);



                //Step 1 - Login to WebAccess site with any privileged user and go to Studies tab then search and load any study which has all patient information and report into the viewer using "view exam" button.
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword, language);
                ExecutedSteps++;

                // Step 2- Navigate to studies tab and search study using accession # from the TestData in excel sheet
                studies = (Studies)login.Navigate("Studies", 1, "Studies");
                studies.SearchStudy(AccessionNo: accession, Datasource: dataSource);
                studies.SelectStudy(GetStudyGridColName("Accession"), accession);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                // Step 3- Click the email study icon in the toolbar to open email popup window                   
                bluringviewer.ClickElement(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_toolbarEmailStudyWrapper)));
                bluringviewer.WaitTillEmailWindowAppears();
                ExecutedSteps++;

                //Step4- Validate that the Labels (Email To, Name, Reason, Modality), Option 'All' in Modality dropdown and Buttons (Send, Cancel) are localized
                if (ValidateLocalization(BluringViewer_MappingFilePath, "EmailStudy", viewer: "bluring"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step 5: Validate Attached studies label is localized in email window
                IWebElement attachedStudiesElement = Driver.FindElement(By.CssSelector(BluRingViewer.label_emailAttachedStudies));
                String attachedStudiesLabel = attachedStudiesElement.Text.Substring(0, (attachedStudiesElement.Text.IndexOf("(") - 1));
                if (attachedStudiesLabel.Equals(ReadDataFromJsonFile(Localization.LocaleUserSettingsJsonFile, "EmailStudy.Details.AttachedStudies")))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Button elements for click actions
                IWebElement sendButton = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_sendEmail));

                // Step 6: Click send and validate the Error message 'The name cannot be empty' is localized
                ClickElement(sendButton);
                String emailErrorMessage1 = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_emailErrorMessage)).Text;
                if (emailErrorMessage1.Equals(ReadDataFromJsonFile(Localization.LocaleUserSettingsJsonFile, "EmailStudy.ErrorMessage.Name")))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step 7: Enter a name and Click Send. Validate the Error message 'The email address cannot be empty.' is localized
                Driver.FindElement(By.CssSelector(BluRingViewer.input_emailName)).SendKeys("TestName");
                ClickElement(sendButton);
                String emailErrorMessage2 = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_emailErrorMessage)).Text;
                if (emailErrorMessage2.Equals(ReadDataFromJsonFile(Localization.LocaleUserSettingsJsonFile, "EmailStudy.ErrorMessage.Email")))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step 8: Enter an invalid email and Click Send. Validate the Error message 'Email Address not Valid' is localized
                Driver.FindElement(By.CssSelector(BluRingViewer.input_email)).SendKeys("TestEmail");
                ClickElement(sendButton);
                String emailErrorMessage3 = Driver.FindElement(By.CssSelector(BluRingViewer.div_emailErrorMessage)).Text;
                if (emailErrorMessage3.Equals(ReadDataFromJsonFile(Localization.LocaleUserSettingsJsonFile, "EmailStudy.ErrorMessage.NotValidEmail")))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step 9: Enter a valid email and Click Send. Validate the Error message 'The reason cannot be empty.' is localized
                Driver.FindElement(By.CssSelector(BluRingViewer.input_email)).Clear();
                Driver.FindElement(By.CssSelector(BluRingViewer.input_email)).SendKeys("no-reply@merge.com");
                ClickElement(sendButton);
                String emailErrorMessage4 = Driver.FindElement(By.CssSelector(BluRingViewer.div_emailErrorMessage)).Text;
                if (emailErrorMessage4.Equals(ReadDataFromJsonFile(Localization.LocaleUserSettingsJsonFile, "EmailStudy.ErrorMessage.Reason")))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step 10: Enter a reason and Click Send. Validate the Error message 'Failed to send email study.' is localized for case where email notification is not configured in config tool
                Driver.FindElement(By.CssSelector(BluRingViewer.input_Notes)).SendKeys("TestReason");
                ClickElement(sendButton);
                PageLoadWait.WaitForFrameLoad(30);
                if (GetElement("cssselector", BluRingViewer.div_emailErrorMessage) != null)
                {
                    String emailErrorMessage5 = Driver.FindElement(By.CssSelector(BluRingViewer.div_emailErrorMessage)).Text;
                    if (emailErrorMessage5.Equals(ReadDataFromJsonFile(Localization.LocaleUserSettingsJsonFile, "EmailStudy.ErrorMessage.Fail")))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_cancelEmail)).Click();
                }
                else
                {
                    // Email sent successfully. Validate Pin Code Dialog box
                    String emailPinCodeLabel = Driver.FindElement(By.CssSelector(BluRingViewer.label_emailPinCode)).Text;
                    String emailPinCodeInfo = Driver.FindElement(By.CssSelector(BluRingViewer.label_emailPinCodeInfo)).Text;
                    if (emailPinCodeLabel.Equals(ReadDataFromJsonFile(Localization.LocaleUserSettingsJsonFile, "EmailStudy.PinCodeDialog.PinCode"))
                        && emailPinCodeInfo.Equals(ReadDataFromJsonFile(Localization.LocaleUserSettingsJsonFile, "EmailStudy.PinCodeDialog.PinCodeInfo")))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    Driver.FindElement(By.CssSelector(BluRingViewer.div_closePinDialog)).Click();
                }

                //Step 11
                Dictionary<string, string> downloadedMail = new Dictionary<string, string>();
                EmailUtils ph1Email = new EmailUtils() { EmailId = Config.CustomUser1Email, Password = Config.CustomUserEmailPassword };
                ph1Email.MarkAllMailAsRead("INBOX");
                var pinnumber = bluringviewer.EmailStudy_BR(Config.CustomUser1Email);
                result.steps[++ExecutedSteps].StepPass();

                //Step 12
                downloadedMail = ph1Email.GetMailUsingIMAP("no-reply@merge.com", "Emailed Study", MarkAsRead: true, maxWaitTime: 3);
                string emaillink2 = ph1Email.GetEmailedStudyLink(downloadedMail);
                bluringviewer = LaunchEmailedStudy.LaunchStudy<BluRingViewer>(emaillink2, pinnumber);
                var Viewport = BasePage.Driver.FindElement(By.CssSelector(bluringviewer.Activeviewport));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);

                bool Step12_1 = studies.CompareImage(result.steps[ExecutedSteps], Viewport, totalImageCount: 1);
                bool Step12_2 = bluringviewer.ValidateLocalizationStudyTitle(studyDateTimeArray, new string[] { BluRingViewer.div_emailPrior, BluRingViewer.div_layoutPrior, BluRingViewer.div_datePrior, BluRingViewer.div_timePrior });
                bool Step12_3 = bluringviewer.ValidateLocalizationStudyToolbox();

                if (Step12_1 && Step12_2 && Step12_3)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                // Close viewer                
                bluringviewer.CloseBluRingViewer();
                login.Logout();

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

                //Report Result
                result.FinalResult(ExecutedSteps);

                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                GC.Collect();
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Close browser
                Driver.Manage().Cookies.DeleteAllCookies();
                this.CloseBrowser();

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

                //Return Result
                return result;
            }
            finally
            {
                Logger.Instance.InfoLog("------------- Test_163435 Complete ----------------");
            }
        }


        /// <summary>
        /// Verify that all UI Elements in viewer and prior viewer for Study Title, Toolbox, and Demographics
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_163436(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            String savedLocale = Config.Locale;
            Localization localization = new Localization();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                // Create new names for domain, role and user
                String domainName = "Domain_" + new Random().Next(1, 1000);
                String adminDomainRole = "AdminRole_" + new Random().Next(1, 1000);
                String roleName = "Role_" + new Random().Next(1, 1000);
                String userID = "User_" + new Random().Next(1, 1000);
                String userPass = "UserPass_" + new Random().Next(1, 1000);

                // Fetch required Test data        
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String dataSource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSource");
                String language = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Languages");
                String dateTime = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDate");
                String[] studyDateTimeArray = dateTime.Split(';');
                IWebElement Viewport;

                // Enable Email Study from the Service Tool and restart services
                servicetool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                servicetool.WaitWhileBusy();
                servicetool.EnableEmailStudy();
                servicetool.ApplyEnableFeatures();
                servicetool.AcceptDialogWindow();
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                servicetool.WaitWhileBusy();
                servicetool.CloseConfigTool();

               // Precondition: Create new domain and add all available tools to the tooltip. Create role and enable all features in domain and role pages. Create a new user for domain and roles created.
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                // Create a new domain
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.CreateDomain(domainName, adminDomainRole, datasources: new string[] { dataSource });
                domainmanagement.SetCheckBoxInEditDomain("emailstudy", 0);

                // Add available tools to tooltip
                IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(9)"));
                IWebElement group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(10)"));
                var dictionary = new Dictionary<String, IWebElement>();

                //Tooltip
                dictionary.Add("Save Series", group1);
                domainmanagement.AddToolsToToolbox(dictionary);
                dictionary.Add("Series Scope", group1);
                domainmanagement.AddToolsToToolbox(dictionary);
                dictionary.Add("Save Annotated Images", group1);
                domainmanagement.AddToolsToToolbox(dictionary);
                dictionary.Add("Image Scope", group2);
                domainmanagement.AddToolsToToolbox(dictionary);
                PageLoadWait.WaitForPageLoad(20);

                domainmanagement.ClickSaveNewDomain();
                PageLoadWait.WaitForPageLoad(20);

                // Create a new role 
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.CreateRole(domainName, roleName, "Bluring Internationalization");
                PageLoadWait.WaitForPageLoad(20);

                //Create a new user
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(userID, domainName, roleName, 0, "", 1, userPass);

                // Logout
                login.Logout();

                // set test locale
                String locale = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Locale");
                localization.UpdateLocalization(locale);

                //Step 1 - From login page select a language such as French, and login as the user created in pre-conditions.
                login.DriverGoTo(login.url);
                login.LoginIConnect(userID, userPass, language);
                ExecutedSteps++;

                // Step 2: Navigate to Studies page, and load a study in Universal Viewer.
                studies = (Studies)login.Navigate("", 1, "");
                studies.SearchStudy(AccessionNo: accession);
                studies.SelectStudy(GetStudyGridColName("Accession"), accession);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                // Step 3: On Study Panel, verify that demographics and overlay texts on each image viewport are localized.
                Viewport = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_viewport));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (studies.CompareImage(result.steps[ExecutedSteps], Viewport, totalImageCount: 1))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step 4: On Study Panel, validate Email Study Icon, Layout Icon, Date lable, and Time label in the Study Panel title bar are localized in target language.
                String[] titlePanelElements = { BluRingViewer.div_toolbarEmailStudyWrapper, BluRingViewer.div_ToolbarLayoutWrapper, BluRingViewer.div_studypaneldate, BluRingViewer.div_studypaneltime };
                if (bluringviewer.ValidateLocalizationStudyTitle(studyDateTimeArray, titlePanelElements))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step 5: On study panel, select an image viewport, right click the mouse button. Hover over each tool in the toolbox and validate the tooltip is localized in the target language.
                if (bluringviewer.ValidateLocalizationStudyToolbox())
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step 6: Load a prior by clicking the study info area in the related study list.
                bluringviewer.CloseBluRingViewer();
                PageLoadWait.WaitForFrameLoad(5);
                studies.SelectStudy(GetStudyGridColName("Accession"), accession);
                BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                bluringviewer.OpenPriors(2);
                PageLoadWait.WaitForFrameLoad(10);
                bluringviewer.SetViewPort(0, 2);
                ExecutedSteps++;

                // Step 7: On Prior Study Panel, verify that demographics and overlay texts on each image viewport are localized.
                Viewport = BasePage.Driver.FindElement(By.CssSelector(bluringviewer.Activeviewport));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (studies.CompareImage(result.steps[ExecutedSteps], Viewport, totalImageCount: 1))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step 8: On Prior Study Panel, Validate Email Study Icon, Layout Icon, Date lable, and Time label in the Study Panel title bar are localized in target language.
                if (bluringviewer.ValidateLocalizationStudyTitle(studyDateTimeArray, new string[] { BluRingViewer.div_emailPrior, BluRingViewer.div_layoutPrior, BluRingViewer.div_datePrior, BluRingViewer.div_timePrior }))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step 9: On Prior study panel select an image viewport, right click the mouse button. Hover over each tool in the toolbox and validate the tooltip is localized in the target language.
                if (bluringviewer.ValidateLocalizationStudyToolbox())
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Close viewer
                bluringviewer.CloseBluRingViewer();
                login.Logout();

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

                //Report Result
                result.FinalResult(ExecutedSteps);

                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                GC.Collect();
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Close browser
                Driver.Manage().Cookies.DeleteAllCookies();
                this.CloseBrowser();

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

                //Return Result
                return result;
            }
            finally
            {
                Logger.Instance.InfoLog("------------- Test_163436 Complete ----------------");
            }
        }


        /// <summary>
        /// Validate localization of UI elements in universal viewer by launching Shared Study between users (Grant Access) from Inbounds/Outbounds page
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_164655(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            RoleManagement rolemanagementnew;
            DomainManagement domainmanagementnew;

            String savedLocale = Config.Locale;
            Localization localization = new Localization();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            // Create new names for domain, role and user
            int random = new Random().Next(1, 1000);

            string domainName = "Domain_" + testid + "_" + random;
            string adminDomainRole = "AdminRole_" + testid + "_" + random;
            string roleName = "Role_" + testid + "_" + random;
            string userID = "User_" + testid + "_" + random;
            string userPass = "password";

            try
            {

                // Enable Study Sharing from the Service Tool and restart services
                servicetool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                servicetool.WaitWhileBusy();
                servicetool.EnableStudySharing();
                servicetool.EnableEmailStudy();
                wpfobject.WaitTillLoad();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.AcceptDialogWindow();
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                servicetool.WaitWhileBusy();
                servicetool.CloseConfigTool();

                //Fetch required Test data        
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String Language = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Languages");
                String dataSource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSource");
                String dateTime = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDate");
                String[] studyDateTimeArray = dateTime.Split(';');

                // set test locale
                String locale = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Locale");
                localization.UpdateLocalization(locale);

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword, Language);

                domainmanagement = (DomainManagement)login.Navigate("DomainManagement", 1, "Domain");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("grant", 0);
                domainmanagement.SetCheckBoxInEditDomain("emailstudy", 0);
                domainmanagement.ClickSaveNewDomain();
                PageLoadWait.WaitForPageLoad(20);

                // Create a new domain
                domainmanagementnew = (DomainManagement)login.Navigate("DomainManagement", 1, "Domain");
                domainmanagementnew.CreateDomain(domainName, adminDomainRole, datasources: new string[] { dataSource });
                domainmanagementnew.Enable3DView();
                domainmanagementnew.SetCheckBoxInEditDomain("emailstudy", 0);
                domainmanagementnew.ClickSaveNewDomain();
                PageLoadWait.WaitForPageLoad(20);

                rolemanagement = (RoleManagement)login.Navigate("RoleManagement", 1, "Role");

                rolemanagement.ShowRolesFromDomainDropDown();
                rolemanagement.SelectDomainfromDropDown("SuperAdminGroup");
                rolemanagement.SelectRole("SuperRole");
                PageLoadWait.WaitForPageLoad(20);
                rolemanagement.ClickEditRole();
                rolemanagement.GrantAccessRadioBtn_Anyone().Click();
                rolemanagement.SetCheckboxInEditRole("email", 0);
                rolemanagement.ClickSaveRole();
                PageLoadWait.WaitForPageLoad(20);

                // Create a new role, it includes enabling grant access as well.
                rolemanagementnew = (RoleManagement)login.Navigate("RoleManagement", 1, "Role");
                rolemanagementnew.CreateRole(domainName, roleName, "Bluring Internationalization");
                PageLoadWait.WaitForPageLoad(20);

                rolemanagement.SelectDomainfromDropDown(domainName);
                rolemanagement.SelectRole(roleName);
                PageLoadWait.WaitForPageLoad(20);
                rolemanagementnew.ClickEditRole();
                rolemanagementnew.SetCheckboxInEditRole("3dview", 0);
                rolemanagementnew.SetCheckboxInEditRole("email", 0);
                rolemanagementnew.ClickSaveRole();
                PageLoadWait.WaitForPageLoad(20);

                //Create a new user
                usermanagement = (UserManagement)login.Navigate("UserManagement", 1, "User");
                usermanagement.CreateUser(userID, domainName, roleName, 0, "", 1, userPass);
                PageLoadWait.WaitForPageLoad(20);

                //Find Study and Share it to outbound for test user
                studies = (Studies)login.Navigate("Studies", 1, "Studies");
                studies.SearchStudy(AccessionNo: Accession, Datasource: dataSource);
                studies.SelectStudy(GetStudyGridColName("Accession"), Accession);

                String[] usersToGrant = new string[] { userID };
                ShareStudy(false, usersToGrant, false, domainName, null, false);

                // Logout
                login.Logout();

                //Step 1 - Login to WebAccess site with user UserB and required language
                ++ExecutedSteps;
                login.DriverGoTo(login.url);
                login.LoginIConnect(userID, userPass, Language);

                //Enable connections in user options
                userpref.OpenUserPreferences();
                userpref.SwitchToUserPrefFrame();
                if (!userpref.EnableConnectionTestTool().Selected)
                    userpref.EnableConnectionTestTool().Click();
                userpref.CloseUserPreferences();

                //Step 2 - Launch InBound Study search page
                ++ExecutedSteps;
                //Select Inbounds button
                inbounds = (Inbounds)login.Navigate("Inbounds", 1, "Inbounds");
                inbounds.SearchStudy(AccessionNo: Accession, Date: "All", Study_Performed_Period: "All Dates", Study_Received_Period: "All Dates");

                //Select Study
                PageLoadWait.WaitForSearchLoad();
                inbounds.SelectStudy(GetStudyGridColName("Accession"), Accession);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();

                //Step 3 - Validate GlobalToolBar icons to ensure that all ToolTips of the icons are localized.
                ++ExecutedSteps;
                bool IsGlobalToolbarLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "GlobalToolbar", path: "other", viewer: "bluring");
                if (IsGlobalToolbarLocalized)
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

                //Step 4 - Select Option in User Settings and verify dropdown is localized aexcept the Colour options Default, Green and Orange
                ++ExecutedSteps;
                bool IsUserSettingsLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "UserSettings", path: "other", viewer: "bluring");
                if (IsUserSettingsLocalized)
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

                //Step 5 - Select Collaboration Window and elements are localized.
                ++ExecutedSteps;
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingCollaboration).Click();
                bool IsCollaboratgionLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "Collaboration", path: "other", viewer: "bluring");
                if (IsCollaboratgionLocalized)
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
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingCollaboration).Click();

                //Step 6 - Validate That Network Connection Window elements are localized.
                ++ExecutedSteps;
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingNetworkConnection).Click();
                bool IsNetWorkConnectionLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "NetworkConnection", path: "other", viewer: "bluring");
                if (IsNetWorkConnectionLocalized)
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

                //Step 7 - Validate That Show/Hide drop down menu items are localized.
                ++ExecutedSteps;
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingShowHide).Click();
                bool IsNetShowHideLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "ShowHide", path: "other", viewer: "bluring");
                if (IsNetShowHideLocalized)
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

                //Step 8 - Validate That Help Panel dropdown items are localized.
                ++ExecutedSteps;
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingHelp).Click();
                bool IsHelpLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "HelpPanel", path: "other", viewer: "bluring");
                if (IsHelpLocalized)
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

                //Step 9 - Validate that labels like EXAM LIST, HISTORY, Modality and Sort By are localized
                ++ExecutedSteps;
                bool IsPatientHistoryLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "PatientHistory", path: "other", viewer: "bluring");
                if (IsPatientHistoryLocalized)
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

                //Step 10 - Validate Options "All", Title "modality" and "Clear all" are localized
                ++ExecutedSteps;
                bluringviewer.OpenModalityFilter();
                bool IsPatientHistoryModalityLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "PatientHistoryModality", path: "other", viewer: "bluring");
                if (IsPatientHistoryModalityLocalized)
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
                bluringviewer.CloseModalityFilter();

                //Step 11 - Validate Options Sort By, Date-Newest, Date-Oldest and Modality Type are Localized
                ++ExecutedSteps;
                bluringviewer.OpenSortDorpdown();
                bool IsPatientHistorySortByLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "PatientHistorySortBy", path: "other", viewer: "bluring");
                if (IsPatientHistorySortByLocalized)
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
                GetElement(SelectorType.CssSelector, BluRingViewer.cdk_OverlayContainer).Click();

                // Step 12 - Validate Email Study Icon, Layout Icon, Date lable, in the Study Panel title bar are localized in target lanugage 
                ++ExecutedSteps;

                String[] titlePanelElements = { BluRingViewer.div_toolbarEmailStudyWrapper, BluRingViewer.div_ToolbarLayoutWrapper, BluRingViewer.div_studypaneldate, BluRingViewer.div_studypaneltime };
                if (bluringviewer.ValidateLocalizationStudyTitle(studyDateTimeArray, titlePanelElements))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Close viewer
                bluringviewer.CloseBluRingViewer();

                login.Logout();

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                GC.Collect();
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Close browser
                Driver.Manage().Cookies.DeleteAllCookies();
                this.CloseBrowser();

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Validate localization of UI elements in universal viewer by launching the transferred (other language) study from Studies Page (Transfer Study)
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_164654(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            String savedLocale = Config.Locale;
            Localization localization = new Localization();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                // Enable Study Transfer from the Service Tool and restart services
                servicetool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                servicetool.WaitWhileBusy();
                servicetool.EnableDataTransfer();
                servicetool.ApplyEnableFeatures(); 

                try
                {  
                    servicetool.AcceptDialogWindow();
                }
                catch (Exception e)
                { // Accept dialog only appears if previous option was changed or not
                }

                wpfobject.WaitTillLoad();
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.TransferService);
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                servicetool.WaitWhileBusy();
                servicetool.EnableTransferService();
                servicetool.ApplyEnableFeatures();

                try
                {
                    servicetool.AcceptDialogWindow();
                }
                catch (Exception e)
                { // Accept dialog only appears if previous option was changed or not
                }

                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                servicetool.WaitWhileBusy();
                servicetool.CloseConfigTool();

                //Fetch required Test data        
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String language = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Languages");
                String dataSource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSource");
                String[] sources = dataSource.Split(':');

                String firstImagePath = Directory.GetCurrentDirectory() + @"\164654_source.jpg";
                String secondImagePath = Directory.GetCurrentDirectory() + @"\164654_dest.jpg";

                IWebElement Viewport;

                // Precondition: Add two data sources to domain page and enable data transfer.

                // set test locale
                String locale = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Locale");
                localization.UpdateLocalization(locale);

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("datatransfer", 0);
                domainmanagement.ClickSaveNewDomain();
                PageLoadWait.WaitForPageLoad(20);

                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");

                rolemanagement.ShowRolesFromDomainDropDown();
                rolemanagement.SelectDomainfromDropDown("SuperAdminGroup");
                rolemanagement.SelectRole("SuperRole");
                PageLoadWait.WaitForPageLoad(20);
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("transfer", 0);
                rolemanagement.ClickSaveRole();
                PageLoadWait.WaitForPageLoad(20);
                login.Logout();


                // Step 1: Launch iCA application and select target localization from culture drop down. Log into application as admin user.
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword, language);
                ExecutedSteps++;

                // Step 2: Navigate to Studies tab. Search study by accession no. and load in Universal Viewer.
                studies = (Studies)login.Navigate("Studies", 1, "Studies");
                studies.SearchStudy(AccessionNo: accession, Datasource: sources[0]);
                studies.SelectStudy(GetStudyGridColName("Accession"), accession);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                // Step 3: Take a screen shot of the viewer and save it.
                Viewport = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_viewport));
                DownloadImageFile(Viewport, firstImagePath);
                ExecutedSteps++;

                // Step 4: Close the Universal Viewer and from Studies tab, transfer this study. Select another destination datasource for transfer and submit.
                bluringviewer.CloseBluRingViewer();
                studies.SelectStudy(GetStudyGridColName("Accession"), accession);
                string succeededLabel = ReadDataFromResourceFile(Localization.DataManagement, "data", "Success");
                studies.TransferStudy(sources[1], accession, SelectallPriors: false, SucceededTitle: succeededLabel);
                ExecutedSteps++;

                // Step 5: After transfer completes, go to Studies page, and search the same study in destination datasource. Launch this study in Universal viewer.
                studies.SearchStudy(AccessionNo: accession, Datasource: sources[1]);
                studies.SelectStudy(GetStudyGridColName("Accession"), accession);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                // Step 6: Take a screen shot of the viewer and save it.
                Viewport = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_viewport));
                DownloadImageFile(Viewport, secondImagePath);
                ExecutedSteps++;

                // Step 7: Compare both screen shots and check there is no difference.
                if (CompareImage(firstImagePath, secondImagePath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Close viewer
                bluringviewer.CloseBluRingViewer();
                login.Logout();

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

                //Report Result
                result.FinalResult(ExecutedSteps);

                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                GC.Collect();
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Close browser
                Driver.Manage().Cookies.DeleteAllCookies();
                this.CloseBrowser();

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

                //Return Result
                return result;
            }
            finally
            {
                Logger.Instance.InfoLog("------------- Test_164654 Complete ----------------");
            }
        }

        /// <summary>
        /// Configure Language culture in iCA
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_Precondition(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            String savedLocale = Config.Locale;
            Localization localization = new Localization();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            String cultures = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Locale");
            String[] culturesToConfigure = cultures.Split(':');

            // Save some global variables
            String saveCurrentDirectory = Directory.GetCurrentDirectory();

            // Retrieve necesasry datasource
            String retrievedDS = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSource");
            String[] dataSource2 = retrievedDS.Split(':');

            try
            {
                //Fetch required Test data                
                string ZipPath = Config.zipPath;
                string ExtractPath = Config.extractpath;
                string defaultPath = Config.defaultpath;

                string commonpath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "OtherFiles" + Path.DirectorySeparatorChar;
                Logger.Instance.InfoLog("Localization Setup:  ZipPath = " + ZipPath);
                Logger.Instance.InfoLog("Localization Setup:  ExtractPath = " + ExtractPath);
                Logger.Instance.InfoLog("Localization Setup:  defaultPath = " + defaultPath);

                Directory.SetCurrentDirectory(ExtractPath);
                Logger.Instance.InfoLog("Localization Setup:  Current Directory changed to:" + Directory.GetCurrentDirectory());

                string LocalizationPrepareFile = ExtractPath + Path.DirectorySeparatorChar + "Localization_Prepare.wsf";
                string LocalizationCompleteFile = ExtractPath + Path.DirectorySeparatorChar + "Localization_Complete.wsf";
                string PrepareOutputPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Localization_Prepare.log";
                string CompleteOutputPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Localization_Complete.log";

                string TranslationEXEpath = commonpath + "TranslationTool.exe";
                Logger.Instance.InfoLog("Localization Setup:  TranslationEXEpath = " + TranslationEXEpath);

                String xmlFilePath = @"C:\WebAccess\WebAccess\web.Config";
                String NodePath = "configuration/appSettings/add";
                String FirstAttribute = "key";
                String AttValue = "Application.Culture";
                String SecondAttribute = "value";

                ServiceTool servicetool = new ServiceTool();

                bool localizationPrepare = true;
                bool localizationComplete = true;
                bool webConfigUpdated = true;
                bool loginUIValidated = true;

                //Step 1 - Unzip Localization SDK.  Run Localization Prepare/Complete and update web.config for each culture
                bool UnzipFolder = UnZipSDKFolder(ZipPath, ExtractPath, defaultPath);

                foreach (string cultureToConfigure in culturesToConfigure)
                {
                    String culture = cultureToConfigure.Split(',')[0];
                    String LCIDCode = cultureToConfigure.Split(',')[1];

                    String EIWix = ExtractPath + Path.DirectorySeparatorChar + culture + @"\UploaderTool_Resources\WixLocalization\Language_" + culture + ".wxl";
                    String POPWix = ExtractPath + Path.DirectorySeparatorChar + culture + @"\PopConfigurationTool_Resources\WixLocalization\Language_" + culture + ".wxl";
                    String EIBoot = ExtractPath + Path.DirectorySeparatorChar + culture + @"\UploaderTool_Resources\BootStrapperLocalization\Theme_" + culture + ".wxl";
                    String POPBoot = ExtractPath + Path.DirectorySeparatorChar + culture + @"\PopConfigurationTool_Resources\BootStrapperLocalization\Theme_" + culture + ".wxl";
                    String GlobalResourcePath = @"This PC\Local Disk (C:)\WebAccess\LocalizationSDK\" + culture;

                    // Run Localiation Prepare
                    if (!servicetool.Prepare_CompleteLocalization(culture, LocalizationPrepareFile, PrepareOutputPath))
                    {
                        Logger.Instance.InfoLog("Localization Prepare failed for " + culture);
                        localizationPrepare = false;
                    }
                    
                    // Run Translation tool
                    servicetool.Translation(TranslationEXEpath, GlobalResourcePath, culture.Split('-')[0], culture.Split('-')[1]);

                    // Update Wix files
                    ChangeAttributeValue(EIBoot, "/WixLocalization", "Culture", culture, encoding: true); //Theme.wxl
                    ChangeAttributeValue(EIBoot, "/WixLocalization", "Language", LCIDCode, encoding: true);
                    ChangeAttributeValue(POPBoot, "/WixLocalization", "Culture", culture, encoding: true);
                    ChangeAttributeValue(POPBoot, "/WixLocalization", "Language", LCIDCode, encoding: true);
                    ChangeAttributeValue(EIWix, "/WixLocalization", "Culture", culture, encoding: true); //Language.wxl                
                    ChangeAttributeValue(POPWix, "/WixLocalization", "Culture", culture, encoding: true);

                    // Run Localization Completion tool
                    if (!servicetool.Prepare_CompleteLocalization(culture, LocalizationCompleteFile, CompleteOutputPath))
                    {
                        Logger.Instance.InfoLog("Localization Complete failed for " + culture);
                        localizationComplete = false;
                    }

                    // Update web.config
                    String ExistingValue = GetAttributeValue(xmlFilePath, NodePath, FirstAttribute, AttValue, SecondAttribute);
                    if (!ExistingValue.Contains(culture))
                    {
                        SetWebConfigValue(xmlFilePath, AttValue, ExistingValue + "," + culture);
                    }
                    String NewValue = GetAttributeValue(xmlFilePath, NodePath, FirstAttribute, AttValue, SecondAttribute);

                    if (!NewValue.Contains(culture))
                    {
                        Logger.Instance.InfoLog("Weg.config does not contain " + culture);
                        webConfigUpdated = false;
                    }
                }

                if (UnzipFolder && localizationPrepare && localizationComplete && webConfigUpdated)
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

                //Step 2 - Add DCM4CHEE datasource 
                ServiceTool st = new ServiceTool();
                st.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);

                st.NavigateToConfigToolDataSourceTab();
                if (!st.GetDataSourceList().ContainsKey(dataSourceName))
                {
                    st.AddDCMDataSource(dataSourceName, "Dicom", "10.4.39.48", dataSourceName, "11112");
                    wpfobject.WaitTillLoad();
                    wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                    wpfobject.WaitTillLoad();
                }

                if (!st.GetDataSourceList().ContainsKey(dataSource2[0]))
                {
                    st.AddDCMDataSource(dataSource2[0], "Dicom", dataSource2[1], dataSource2[0], dataSource2[2]);
                    wpfobject.WaitTillLoad();
                    wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                    wpfobject.WaitTillLoad();
                }

                st.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                st.CloseConfigTool();

                //Add setting in DCM4CHEE
                DSAttribute = GetNodeValue(Config.DSManagerFilePath, "/add[@id='" + dataSourceName + "" + "']/parameters/excludedAttributes");
                ExcludedAttribute = DSAttribute.Replace(specificCharacterSet, "");
                ChangeNodeValue(Config.DSManagerFilePath, "/add[@id='" + dataSourceName + "" + "']/parameters/excludedAttributes", ExcludedAttribute);
                servicetool.RestartIISUsingexe();

                //Add the datasource to superadmingroup and enable universal viewer
                login.DriverGoTo(url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();

                // Do not set inst name if already set by EnvironmentSetup.cs
                if (domain.ReceivingInstTxtBox().GetAttribute("value").ToString().Equals(""))
                {
                    domain.SetReceivingInstitution("test");
                }

                domain.SetViewerTypeInNewDomain("universal");
                domain.ConnectAllDataSources();
                domain.ClickSaveEditDomain();

                //Enable Universal viewer in role
                RoleManagement role = (RoleManagement)login.Navigate("RoleManagement");
                role.SearchRole(Config.adminRoleName, Config.adminGroupName);
                role.SelectRole("SuperRole");
                role.ClickEditRole();
                role.SetCheckboxInEditRole("universalviewer", 0);
                role.ClickSaveEditRole();

                login.Logout();
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 3 - Verfiy login page has been localized
                login.DriverGoTo(url);

                foreach (string cultureToConfigure in culturesToConfigure)
                {
                    localization.UpdateLocalization(cultureToConfigure.Split(',')[0]);

                    login.PreferredLanguageSelectList().SelectByValue(cultureToConfigure.Split(',')[0]);
                    Thread.Sleep(5000);

                    if (!ValidateLocalization(ICA_MappingFilePath, "LoginPage"))
                    {
                        Logger.Instance.InfoLog("Login UI not Localized for " + cultureToConfigure.Split(',')[0]);
                        loginUIValidated = false;
                    }
                }

                if (loginUIValidated)
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

                Directory.SetCurrentDirectory(saveCurrentDirectory);

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

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
                
                Directory.SetCurrentDirectory(saveCurrentDirectory);

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Validate localization of UI elements in Universal viewer by launching the Integrator URL with Show selector-Blank and Accession.
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_164642(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            String savedLocale = Config.Locale;
            Localization localization = new Localization();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {

                // set test locale
                String locale = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Locale");
                localization.UpdateLocalization(locale);

                //Fetch required Test data
                String AdminUsername = Config.adminUserName;
                String AdminPassword = Config.adminPassword;
                String SuperAdminGroup = Config.adminGroupName;
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String Lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String Firstname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String gender = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Gender");
                String dob = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DOB");
                String modality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String studydesc = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDescription"));
                String studydate = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDate"));
                String ipid = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "IPID"));
                String Datasource = login.GetHostName(Config.SanityPACS);//DCM4CHEE
                String AccCol = GetStudyGridColName("Accession");
                String birthdate = ReadDataFromJsonFile(Localization.PatientHistory, "PatientHistory.RelatedStudy.ToolTip.DOB"); //Birthdate:
                String sex = ReadDataFromJsonFile(Localization.PatientHistory, "PatientHistory.RelatedStudy.ToolTip.Sex"); //Sex:
                String mrn = ReadDataFromJsonFile(Localization.PatientHistory, "PatientHistory.RelatedStudy.ToolTip.MRN"); //MRN:
                String ipID = ReadDataFromJsonFile(Localization.PatientHistory, "PatientHistory.RelatedStudy.ToolTip.IPID");
                String acc = ReadDataFromJsonFile(Localization.PatientHistory, "PatientHistory.RelatedStudy.ToolTip.AccessionNo");
                String datasrc = ReadDataFromJsonFile(Localization.PatientHistory, "PatientHistory.RelatedStudy.ToolTip.DataSource");

                String securityID_Local = Config.adminUserName + "-" + Config.adminUserName;
                String URL = "http://localhost/webaccess";

                //Precondition
                login.DriverGoTo(login.url);
                login.LoginIConnect(AdminUsername, AdminPassword);
                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain(SuperAdminGroup);
                domain.SelectDomain(SuperAdminGroup);
                domain.ClickEditDomain();
                domain.SetViewerTypeInNewDomain("enterprise");
                domain.SaveButton().Click();
                RoleManagement role = (RoleManagement)login.Navigate("RoleManagement");
                role.SearchRole("SuperRole", SuperAdminGroup);
                role.SelectRole("SuperRole");
                role.ClickEditRole();
                role.DefaultEnterpriseViewer().Click();
                role.ClickSaveEditRole();
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                userpref.SwitchToUserPrefFrame();
                userpref.BluringViewerRadioBtn().Click();
                userpref.CloseUserPreferences();
                login.Logout();


                //Step-1:iCA 7.0 server under testing is configured and is able to load images from the connected data sources
                ExecutedSteps++;

                //Step-2:In TestEHR Tool,Test Data: 
                //Show Error
                //False
                //False
                TestFixtures.UpdateFeatureFixture("usersharing", value: "Always enabled");
                TestFixtures.UpdateFeatureFixture("allowshowselector", value: "False");
                TestFixtures.UpdateFeatureFixture("allowshowselectorsearch", value: "False");
                TestFixtures.UpdateFeatureFixture("multiplestudy", value: "Show Error", restart: true);
                ExecutedSteps++;

                //Step-3: In the TestEHR tool set:
                //Address = *^<^ *enter path of iCA server under testing*^>^ *
                //Show Selector = true
                //Show Selector Search = true
                //Enter Accession number to a patient with multiple priors
                //Leave the rest fields as default.
                //Click Cmd line
                //Click Load
                ehr.LaunchEHR();
                ehr.SetCommonParameters(address: URL, SecurityID: securityID_Local, culture: Config.Locale.ToLower());
                wpfobject.GetMainWindow("Test WebAccess EHR");
                ehr.SetSelectorOptions(showSelector: "True", selectorsearch: "True");
                ehr.SetSearchKeys_Study(AccessionID);
                String url_3 = ehr.clickCmdLine("ImageLoad");

                login.CreateNewSesion();
                bluringviewer.NavigateToBluringIntegratorURL(url_3);
                bluringviewer = (BluRingViewer)login.NavigateToIntegratorFrame(viewer: "bluring");
                String Step3_1 = ehr.ErrorMsg();
                String RescValue = ReadDataFromResourceFile(Localization.Integrator, "data", "Message_ShowSelectorNotAllowed");
                String RescErrorMsg = ReadDataFromResourceFile(Localization.IntegratorGuestError, "data", "Message_OperationError");
                String ExpectedMsg = RescErrorMsg + RescValue;

                if (Step3_1.Replace("\r\n", "").Trim().Equals(ExpectedMsg))
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


                //Step-4:In the TestEHR tool, change the following settings only:
                //Show Selector = false
                //Show Selector Search = false
                //Enter Accession number to search a patient
                //Click Cmd line
                //Click Load
                ehr.LaunchEHR();
                ehr.SetCommonParameters(address: URL, SecurityID: securityID_Local, culture: Config.Locale.ToLower());
                wpfobject.GetMainWindow("Test WebAccess EHR");
                ehr.SetSelectorOptions(showSelector: "False", selectorsearch: "False");
                ehr.SetSearchKeys_Study(AccessionID);
                String url_4 = ehr.clickCmdLine("ImageLoad");

                login.CreateNewSesion();
                bluringviewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", url: url_4);
                if (bluringviewer != null)
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

                //Step-5:Verify the Tooltip(Birthdate:,MRN:,IPID:,Acc#:,Datasource:) from Exam List card is localized.
                var prior = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors))[0];
                bluringviewer.HoverElement(prior);
                var tooltip = prior.GetAttribute("title");
                var expected_tooltip = Lastname + ", " + Firstname + Environment.NewLine +
                                       birthdate + dob + "  " + sex + gender + Environment.NewLine +
                                       mrn + PatientID + Environment.NewLine +
                                       ipID + ipid + Environment.NewLine +
                                       Environment.NewLine +
                                       studydate + Environment.NewLine +
                                       studydesc + Environment.NewLine +
                                       modality + Environment.NewLine +
                                       acc + AccessionID + Environment.NewLine + Environment.NewLine + datasrc + Datasource + Environment.NewLine;
                Logger.Instance.InfoLog("The Expected ToolTip is--" + expected_tooltip);
                Logger.Instance.InfoLog("The Actual ToolTip is--" + tooltip);
                if (tooltip.Equals(expected_tooltip))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

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

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Validate localization of UI elements in Universal viewer by launching the Integrator URL with Show selector-True and Patient ID(Prior study).
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_164645(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            String savedLocale = Config.Locale;
            Localization localization = new Localization();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {

                // set test locale
                String locale = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Locale");
                localization.UpdateLocalization(locale);

                //Fetch required Test data
                String AdminUsername = Config.adminUserName;
                String AdminPassword = Config.adminPassword;
                String SuperAdminGroup = Config.adminGroupName;
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String Lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String Firstname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String gender = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Gender");
                String dob = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DOB");
                String modality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String studydesc = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDescription"));
                String studydate = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDate"));
                String ipid = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "IPID"));
                String Datasource = login.GetHostName(Config.SanityPACS);//DCM4CHEE
                String AccCol = GetStudyGridColName("Accession");
                String birthdate = ReadDataFromJsonFile(Localization.PatientHistory, "PatientHistory.RelatedStudy.ToolTip.DOB"); //Birthdate:
                String sex = ReadDataFromJsonFile(Localization.PatientHistory, "PatientHistory.RelatedStudy.ToolTip.Sex"); //Sex:
                String mrn = ReadDataFromJsonFile(Localization.PatientHistory, "PatientHistory.RelatedStudy.ToolTip.MRN"); //MRN:
                String ipID = ReadDataFromJsonFile(Localization.PatientHistory, "PatientHistory.RelatedStudy.ToolTip.IPID");
                String acc = ReadDataFromJsonFile(Localization.PatientHistory, "PatientHistory.RelatedStudy.ToolTip.AccessionNo");
                String datasrc = ReadDataFromJsonFile(Localization.PatientHistory, "PatientHistory.RelatedStudy.ToolTip.DataSource");

                String securityID_Local = Config.adminUserName + "-" + Config.adminUserName;
                String URL = "http://localhost/webaccess";

                //Precondition
                login.DriverGoTo(login.url);
                login.LoginIConnect(AdminUsername, AdminPassword);
                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain(SuperAdminGroup);
                domain.SelectDomain(SuperAdminGroup);
                domain.ClickEditDomain();
                domain.SetViewerTypeInNewDomain("enterprise");
                domain.SaveButton().Click();
                RoleManagement role = (RoleManagement)login.Navigate("RoleManagement");
                role.SearchRole("SuperRole", SuperAdminGroup);
                role.SelectRole("SuperRole");
                role.ClickEditRole();
                role.DefaultEnterpriseViewer().Click();
                role.ClickSaveEditRole();
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                userpref.SwitchToUserPrefFrame();
                userpref.BluringViewerRadioBtn().Click();
                userpref.CloseUserPreferences();
                login.Logout();

                //Step-1:iCA 7.0 server under testing is configured and is able to load images from the connected data sources
                ExecutedSteps++;

                //Step-2:Set as following values in configuration files:
                //1) In web.config under webaccess\webaccess\
                //*^<^ *add key = "Integrator.OnMultipleStudy"value = "Show Selector"/*^>^*

                //or modify it from Service Tool/Integrator tab/On Multiple Study: Show Selector

                //2) In WebaccessConfiguration.xml under webaccess\webaccess\Config\
                //*^<^*AllowShowSelector*^>^*true*^<^*/AllowShowSelector *^>^ *
                //*^<^ *AllowShowSelectorSearch *^>^ *true *^<^ */ AllowShowSelectorSearch *^>^ *
                //Restart IIS and Windows Services
                TestFixtures.UpdateFeatureFixture("usersharing", value: "Always enabled");
                TestFixtures.UpdateFeatureFixture("allowshowselector", value: "True");
                TestFixtures.UpdateFeatureFixture("allowshowselectorsearch", value: "True");
                TestFixtures.UpdateFeatureFixture("multiplestudy", value: "Show Selector", restart: true);
                ExecutedSteps++;

                //Step-3:	In the TestEHR tool set:
                //Address = *^<^ *enter path of iCA server under testing*^>^ *
                //Culture = ja - jp(example)
                //Show Selector = blank
                //Show Selector Search = true
                //Enter Accession number to search a patient with multiple priors
                //Leave the rest fields as default.
                //Click Cmd line
                //Click Load
                ehr.LaunchEHR();
                ehr.SetCommonParameters(address: URL, SecurityID: securityID_Local, culture: Config.Locale.ToLower());
                wpfobject.GetMainWindow("Test WebAccess EHR");
                ehr.SetSelectorOptions(showSelector: "True", selectorsearch: "True");
                ehr.SetSearchKeys_Study(PatientID, "Patient_ID");
                String url_3 = ehr.clickCmdLine("ImageLoad");

                login.CreateNewSesion();
                var patientstudy = PatientsStudy.LaunchPatientsStudyPage(url_3);

                var studyinfo = patientstudy.GetPateintList();
                if (studyinfo[AccCol][3].Equals(AccessionID) && IsElementVisible(By.CssSelector("div#SearchPanelDiv"))) //labels should be localized
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

                //Step-4:Select a study from the list found in the Study Selector and load the study
                patientstudy.SelectPatinet(AccCol, AccessionID);
                var viewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", showselector: true);
                var viewport = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                var step4 = result.steps[++ExecutedSteps];
                step4.SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(step4, viewport))
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

                //Step-5:In the TestEHR tool, change the following settings only:
                //Show Selector = blank
                //Show Selector Search = blank
                //Leave all patient / study fields empty. Launch URL.
                ehr.LaunchEHR();
                ehr.SetCommonParameters(address: URL, SecurityID: securityID_Local, culture: Config.Locale.ToLower());
                wpfobject.GetMainWindow("Test WebAccess EHR");
                ehr.SetSelectorOptions(showSelector: "", selectorsearch: "");
                String url_5 = ehr.clickCmdLine("ImageLoad");

                login.CreateNewSesion();
                bluringviewer.NavigateToBluringIntegratorURL(url_5);
                bluringviewer = (BluRingViewer)login.NavigateToIntegratorFrame(viewer: "bluring");

                String Step5_1 = ehr.ErrorMsg();
                String RescValue = ReadDataFromResourceFile(Localization.Integrator, "data", "Message_EmptyStudyData");
                String RescErrorMsg = ReadDataFromResourceFile(Localization.IntegratorGuestError, "data", "Message_OperationError");
                String ExpectedMsg = RescErrorMsg + RescValue;

                if (Step5_1.Replace("\r\n", "").Trim().Equals(ExpectedMsg))
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

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

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

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Validate localization of UI elements in universal viewer by launching the Conference Studies from Conference Page
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_163626(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            String savedLocale = Config.Locale;
            Localization localization = new Localization();

            try
            {

                // Enable Email Study from the Service Tool and restart services
                servicetool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                servicetool.WaitWhileBusy();
                servicetool.EnableConferenceLists();
                servicetool.ApplyEnableFeatures();
                servicetool.AcceptDialogWindow();
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                servicetool.WaitWhileBusy();
                servicetool.CloseConfigTool();

                // set test locale
                String locale = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Locale");
                localization.UpdateLocalization(locale);

                //Fetch required Test data
                String AdminUsername = Config.adminUserName;
                String AdminPassword = Config.adminPassword;
                String SuperAdminGroup = Config.adminGroupName;
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String Lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String Firstname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String gender = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Gender");
                String dob = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DOB");
                String modality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String studydesc = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDescription"));
                String studydate = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDate"));
                String ipid = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "IPID"));
                String Datasource = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSource"));
                String AccCol = GetStudyGridColName("Accession");
                String birthdate = ReadDataFromJsonFile(Localization.PatientHistory, "PatientHistory.RelatedStudy.ToolTip.DOB"); //Birthdate:
                String sex = ReadDataFromJsonFile(Localization.PatientHistory, "PatientHistory.RelatedStudy.ToolTip.Sex"); //Sex:
                String mrn = ReadDataFromJsonFile(Localization.PatientHistory, "PatientHistory.RelatedStudy.ToolTip.MRN"); //MRN:
                String ipID = ReadDataFromJsonFile(Localization.PatientHistory, "PatientHistory.RelatedStudy.ToolTip.IPID");
                String acc = ReadDataFromJsonFile(Localization.PatientHistory, "PatientHistory.RelatedStudy.ToolTip.AccessionNo");
                String datasrc = ReadDataFromJsonFile(Localization.PatientHistory, "PatientHistory.RelatedStudy.ToolTip.DataSource");


                String TopFolderB1 = "NewTop_" + new Random().Next(1, 1000);
                String SubFolderB1_Level2_1 = "NewSub_" + new Random().Next(1, 1000);

                //Precondition
                login.DriverGoTo(login.url);
                login.LoginIConnect(AdminUsername, AdminPassword);
                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                 domain.SearchDomain(SuperAdminGroup);
                domain.SelectDomain(SuperAdminGroup);
                domain.ClickEditDomain();
                domain.SetViewerTypeInNewDomain("enterprise");
                domain.SetCheckBoxInEditDomain("conferencelists", 0);
                domain.SaveButton().Click();
                RoleManagement role = (RoleManagement)login.Navigate("RoleManagement");
                role.SearchRole("SuperRole", SuperAdminGroup);
                role.SelectRole("SuperRole");
                role.ClickEditRole();
                role.SetViewerTypeInNewRole("enterprise");
                role.SetCheckboxInEditRole("conferenceuser", 0);
                role.ClickSaveEditRole();
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                userpref.SwitchToUserPrefFrame();
                userpref.HTML4RadioBtn().Click();
                userpref.CloseUserPreferences();
                login.Logout();

                //Step 1 - Login as Administrator with configured language other than English, Navigate to Conference folders Tab, create Top folder and a sub folder(study folder).
                login.DriverGoTo(login.url);
                login.PreferredLanguageSelectList().SelectByValue(Config.Locale);
                Thread.Sleep(5000);
                login.LoginIConnect(AdminUsername, AdminPassword);
                ConferenceFolders conferencefolders = (ConferenceFolders)login.Navigate("ConferenceFolders", 1, "ConferenceStudies");
                bool TopFolder = conferencefolders.CreateToplevelFolder(TopFolderB1, domain: SuperAdminGroup);
                bool SubFolder = conferencefolders.CreateSubFolder(TopFolderB1, SubFolderB1_Level2_1);
                if (TopFolder && SubFolder)
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

                //Step 2 - Navigate to Studies Tab, Search for other language study and Load the study in Enter prise viewer.
                studies = (Studies)login.Navigate("Studies", 1, "Studies");
                studies.ClearFields();
                studies.SearchStudy(Datasource: Datasource, AccessionNo: AccessionID);
                studies.SelectStudy(AccCol, AccessionID);
                StudyViewer viewer = studies.LaunchStudy();
                if (viewer != null)
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

                //Step 3 - Using Add To Conference Folder tool, add the loaded study to the study folder created.
                bool addToConferenceFolderStatus = true;
                try
                {
                    viewer.SelectToolInToolBar(login.ReadDataFromResourceFile(Localization.Tooltip, "data", "AddConferenceStudy"), "review", 1);
                    viewer.AddStudyToStudyFolder(TopFolderB1 + "/" + SubFolderB1_Level2_1);
                    viewer.CloseStudy();
                }
                catch
                {
                    addToConferenceFolderStatus = false;
                }

                if (addToConferenceFolderStatus)
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



                //Step 4 - Navigate to Domain Management Tab, modify the viewer to Universal viewer under Edit domain and Save the changes. Navigate to Role Management Tab, modify the viewer to Universal viewer under Edit role and Save the changes.
                bool updateDomainRoleViewerStatus = true;
                try
                {
                    domain = (DomainManagement)login.Navigate("DomainManagement", 1, "Domain");
                    domain.SearchDomain(SuperAdminGroup);
                    domain.SelectDomain(SuperAdminGroup);
                    domain.ClickEditDomain();
                    domain.SetViewerTypeInNewDomain();
                    domain.SaveButton().Click();
                    role = (RoleManagement)login.Navigate("RoleManagement", 1, "Role");
                    role.SearchRole("SuperRole", SuperAdminGroup);
                    role.SelectRole("SuperRole");
                    role.ClickEditRole();
                    role.DefaultUniversalViewer().Click();
                    role.ClickSaveEditRole();
                    userpref = new UserPreferences();
                    userpref.OpenUserPreferences();
                    userpref.SwitchToUserPrefFrame();
                    userpref.BluringViewerRadioBtn().Click();
                    userpref.CloseUserPreferences();
                }
                catch
                {
                    updateDomainRoleViewerStatus = false;
                }

                if (updateDomainRoleViewerStatus)
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


                //Step 5 - Navigate to Conference folders Tab and load the conference study.
                conferencefolders = (ConferenceFolders)login.Navigate("ConferenceFolders", 1, "ConferenceStudies");
                conferencefolders.ExpandAndSelectFolder(TopFolderB1 + "/" + SubFolderB1_Level2_1);
                PageLoadWait.WaitForLoadingDivToAppear_Conference();
                PageLoadWait.WaitForLoadingDivToDisAppear_Conference(10);
                conferencefolders.SelectStudy(AccCol, AccessionID);
                bluringviewer = BluRingViewer.LaunchBluRingViewer(mode: "ConferenceFolders");
                if (bluringviewer != null)
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

                //Step 6 - Verify the Patient Name in Global Toolbar are localized.
                bool step7 = bluringviewer.PatientDetailsInViewer()["LastName"].Equals(Lastname);
                bool step7_1 = bluringviewer.PatientDetailsInViewer()["FirstName"].Equals(Firstname);
                bool step7_2 = bluringviewer.PatientDetailsInViewer()["PatientID"].Equals(PatientID);

                if (step7 && step7_1 && step7_2)
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

                //Step 7 - Verify the Tooltip(Birthdate:,MRN:,IPID:,Acc#:,Datasource:) from Exam List card is localized.
                var prior = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors))[0];
                bluringviewer.HoverElement(prior);
                var tooltip = prior.GetAttribute("title");
                var expected_tooltip = Lastname + ", " + Firstname + Environment.NewLine +
                                       birthdate + dob + "  " + sex + gender + Environment.NewLine +
                                       mrn + PatientID + Environment.NewLine +
                                       ipID + ipid + Environment.NewLine +
                                       Environment.NewLine +
                                       studydate + Environment.NewLine +
                                       studydesc + Environment.NewLine +
                                       modality + Environment.NewLine +
                                       acc + AccessionID + Environment.NewLine + Environment.NewLine + datasrc + Datasource + Environment.NewLine;
                Logger.Instance.InfoLog("The Expected ToolTip is--" + expected_tooltip);
                Logger.Instance.InfoLog("The Actual ToolTip is--" + tooltip);
                if (tooltip.Equals(expected_tooltip))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 8 - Apply any tool (Ex: Add Tool) to the study
                PageLoadWait.WaitForFrameLoad(20);
                bluringviewer.OpenViewerToolsPOPUp();
                var Addtexttool = ReadDataFromJsonFile(Localization.ViewportToolbar, "ViewportTools.AddText.Tooltip");
                var AddTextUI = bluringviewer.GetToolsInToolBoxByGrid(11);
                bluringviewer.SelectViewerTool(isLocalization: true, ToolName: Addtexttool);
                bluringviewer.ApplyTool_AddText("163626 -午前");
                var step8 = result.steps[++ExecutedSteps];
                var viewport2 = bluringviewer.GetElement(BasePage.SelectorType.CssSelector, bluringviewer.Activeviewport);
                step8.SetPath(testid, ExecutedSteps);
                var isToolApplied = bluringviewer.CompareImage(step8, viewport2);
                if (AddTextUI[0].Split(',')[0].Contains(Addtexttool) && isToolApplied)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                bluringviewer.CloseBluRingViewer();
                login.Logout();

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

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

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Emergency Search
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_164673(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies;
            DomainManagement domainmanagement;
            RoleManagement rolemanagement;
            Maintenance maintenance;
            String SuperAdminGroup = Config.adminGroupName;
            String SuperRole = Config.adminRoleName;
            servicetool = new ServiceTool();

            String savedLocale = Config.Locale;
            Localization localization = new Localization();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                // set test locale
                String locale = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Locale");
                localization.UpdateLocalization(locale);

                //Fetch required Test data
                String AdminUsername = Config.adminUserName;
                String AdminPassword = Config.adminPassword;
                String pacusername = Config.pacsadmin;
                String pacpassword = Config.pacspassword;
                String Languages = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Languages");
                String[] Langs = Languages.Split(':');
                String studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                //String acc = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String pid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String PatientName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String Genders = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Gender");
                String[] gender = Genders.Split(':');
                String DOB = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DOB");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String[] Accession = AccessionList.Split(':');
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String PACSA7 = login.GetHostName(Config.SanityPACS);
                //GridColumnNames
                String AccCol = GetStudyGridColName("Accession");
                String PIDCol = GetStudyGridColName("Patient ID");
                String ModCol = GetStudyGridColName("Modality");
                String PNameCOl = GetStudyGridColName("Patient Name");

                //Step:1-Enable the Emergency Access feature from Service Tool
                //Enable the feature for the user (Administrator) from Domain and Role Management page
                //Ensure at least one data source is enabled for Administrator
                //Precondition

                // Enable Emergecy Access from the Service Tool and restart services
                servicetool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                servicetool.WaitWhileBusy();
                servicetool.EnableEmergencyAccess();
                servicetool.ApplyEnableFeatures();
                servicetool.AcceptDialogWindow();
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                servicetool.WaitWhileBusy();
                servicetool.CloseConfigTool();


                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement = login.Navigate<DomainManagement>();
                domainmanagement.SearchDomain(SuperAdminGroup);
                domainmanagement.SelectDomain(SuperAdminGroup);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("emergency", 0);
                domainmanagement.ClickSaveNewDomain();
                rolemanagement = login.Navigate<RoleManagement>();
                rolemanagement.SearchRole(SuperRole, SuperAdminGroup);
                rolemanagement.SelectRole(SuperRole);
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("emergency", 0);
                rolemanagement.ClickSaveEditRole();
                login.Logout();
                ExecutedSteps++;

                //Step:2-Login as Administrator/Administrator in preferred(target language) who was set up to have Emergency Access

                login.DriverGoTo(url);
                login.PreferredLanguageSelectList().SelectByValue(Config.Locale);
                Thread.Sleep(5000);
                login.LoginIConnect(AdminUsername, AdminPassword);
                ExecutedSteps++;

                //Step:3-Go to studylist and Click on the Radio button"Emergency Search"
                studies = (Studies)login.Navigate("Studies", 1, "Studies");
                studies.Emergencybtn().Click();
                IWebElement Warningdialogbox, Warningmessage;
                studies.EmergencyWarning(out Warningdialogbox, out Warningmessage);
                //String Warningmsg = ReadDataFromResourceFile(Localization.StudySearchControl, "data", "Warning_EmergencySearch");
                String Warningdialog = ReadDataFromResourceFile(Localization.StudySearchControl, "data", "Warning_StartEmergencySearch");
                String CustomLbl = ReadDataFromResourceFile(Localization.StudySearchControl, "data", "Label_CustomSearch");
                String custom = studies.CustomSearchLbl().GetAttribute("innerHTML");

                if (Warningdialog.Replace(" ", "").Equals(Warningdialogbox.Text.Replace(" ", "")) && CustomLbl.Equals(custom))
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


                //Step:4-Click Cancel and go to Audit log from Maintenance page
                studies.Cancelbtn().Click();
                maintenance = (Maintenance)login.Navigate("Maintenance", 1, "Maintenance");
                maintenance.Navigate("Audit", 1, 1, "Maintenance");
                //maintenance.SetCheckBoxInAudit();             
                if (!(IsElementVisible(maintenance.By_EmergencyLog())))
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

                //Step:5-Goto Studylist and enable Emergency Search  and accept the warning 
                //View Audit log entry
                studies = (Studies)login.Navigate("Studies", 1, "Studies");
                studies.Emergencybtn().Click();
                studies.Acceptbtn().Click();
                PageLoadWait.WaitForFrameLoad(5);
                maintenance = (Maintenance)login.Navigate("Maintenance", 1, "Maintenance");
                maintenance.Navigate("Audit", 1, 1, "Maintenance");
                //Check all the checkboxes
                maintenance.SetCheckBoxInAudit("success");
                //maintenance.SetCheckBoxInAudit("major");
                //maintenance.SetCheckBoxInAudit("minor");
                //maintenance.SetCheckBoxInAudit("serious");
                //Select Security Alert in Event ID
                maintenance.SelectEventID("110113", 1);
                maintenance.Btn_Search().Click();
                PageLoadWait.WaitForFrameLoad(5);


                if (maintenance.EmergencyLog().Displayed)
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

                //Step-6:Perform an emergency search by filling in all required patient info (Last Name, First Name, Gender, DOB).
                //Press Search button
                studies = (Studies)login.Navigate("Studies", 1, "Studies");
                studies.Emergencybtn().Click();
                studies.Acceptbtn().Click();
                PageLoadWait.WaitForFrameLoad(5);
                studies.EmergencySearchStudy(PatientName.Split(',')[0], PatientName.Split(',')[1].Trim(), gender[0], DOB, Datasource: PACSA7);
                Dictionary<string, string> row = studies.GetMatchingRow(new string[] { AccCol, PIDCol, ModCol }, new string[] { Accession[0], PatientID, "CR" });
                if (row != null)
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

                //Step-7:Load the study to Universal  viewer to verify the patient info
                studies.SelectStudy1(AccCol, Accession[0]);
                StudyViewer viewer = new StudyViewer();
                BluRingViewer bluViewer = new BluRingViewer();
                bool step7, step7_1, step7_2 = false;

                bluViewer = BluRingViewer.LaunchBluRingViewer();
                step7 = bluViewer.PatientDetailsInViewer()["LastName"].Equals(row[PNameCOl].Split(',')[0].Trim());
                step7_1 = bluViewer.PatientDetailsInViewer()["FirstName"].Equals(row[PNameCOl].Split(',')[1].Trim());
                step7_2 = bluViewer.PatientDetailsInViewer()["PatientID"].ToLower().Equals(row[PIDCol]);

                if (step7 && step7_1 && step7_2)
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
                bluViewer.CloseBluRingViewer();
                PageLoadWait.WaitForFrameLoad(5);


                studies.Acceptbtn().Click();
                PageLoadWait.WaitForFrameLoad(5);

                //Step-8:Go to Audit log again
                maintenance = (Maintenance)login.Navigate("Maintenance", 1, "Maintenance");
                maintenance.Navigate("Audit", 1, 1, "Maintenance");
                maintenance.SelectEventID("110103", 1);
                maintenance.Btn_Search().Click();
                if (maintenance.DICOMAccesslog().Displayed)
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

                //Step-9:Exit Maintenance and go back to Studies Tab 
                studies = (Studies)login.Navigate("Studies", 1, "Studies");
                PageLoadWait.WaitForFrameLoad(20);
                //Validate the default search is in Custom search               
                if (studies.CustomSearchRadioBtn().Enabled == true)
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
                login.Logout();

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

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

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

                //Return Result
                return result;
            }
            finally
            {
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                rolemanagement = login.Navigate<RoleManagement>();
                rolemanagement.SearchRole(SuperRole, SuperAdminGroup);
                rolemanagement.SelectRole(SuperRole);
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("emergency", 1);
                rolemanagement.ClickSaveEditRole();
                domainmanagement = login.Navigate<DomainManagement>();
                domainmanagement.SearchDomain(SuperAdminGroup);
                domainmanagement.SelectDomain(SuperAdminGroup);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("emergency", 1);
                domainmanagement.ClickSaveNewDomain();
                login.Logout();
            }
        }



        /// <summary>
        /// Localization of shared study uploaded from ExamImporter or PacsGateway
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_164716(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            Taskbar taskbar = null;

            String savedLocale = Config.Locale;
            Localization localization = new Localization();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            // Create new names for domain, role and user
            int random = new Random().Next(1, 1000);

            string domainName = "Domain_" + testid + "_" + random;
            string adminDomainRole = "AdminRole_" + testid + "_" + random;
            string roleName = "Role_" + testid + "_" + random;
            string userID = "User_" + testid + "_" + random;
            string userPass = "password_" + random;

            try
            {

                //Fetch required Test data        
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String Language = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Languages");
                String dateTime = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDate");
                String StudyPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                String[] studyDateTimeArray = dateTime.Split(';');

                String EIWindowName = "EI_" + new Random().Next(1, 1000);

                //Preconditions - Generate EI
                //Standard user set created
                //dest and institution already created and configured
                
                //PreCondition - Generate Exam Importer
                taskbar = new Taskbar();
                taskbar.Hide();
                servicetool.LaunchServiceTool();
                servicetool.GenerateInstallerAllDomain(Config.adminGroupName, EIWindowName);
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                BasePage.Kill_EXEProcess("UploaderTool");

                //Delete existing installers
                new List<string>(Directory.GetFiles(Config.downloadpath)).ForEach(file =>
                {
                    if (file.IndexOf(Config.eiInstaller, StringComparison.OrdinalIgnoreCase) >= 0)
                        File.Delete(file);
                });

                // download and install EI
                ExamImporter ei = new ExamImporter();
                ei.DownloadEIinstaller(Config.adminGroupName);

                // Launch EI
                String EIInstalledPath = ei.EI_Installation(Config.adminGroupName, EIWindowName, Config.Inst1, Config.phUserName, Config.phPassword);
                ei.LaunchEI(EIInstalledPath);
                wpfobject.GetMainWindow(EIWindowName);

                ei.LoginToEi(Config.phUserName, Config.phPassword, EIWindowName: EIWindowName);
                wpfobject.GetMainWindow(EIWindowName);
                wpfobject.WaitTillLoad();

                ei.EI_SelectDestination(Config.Dest1, EIWindowName);
                ei.eiWinName = EIWindowName;

                ei.SelectFileFromHdd(StudyPath);
                WpfObjects._mainWindow.WaitWhileBusy();

                ei.SelectAllPatientsToUpload();

                try
                {
                    ei.Send(EIWindowName);
                }
                catch (Exception e)
                {
                    //Log Exception since study might already be on server
                    Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                }

                ei.CloseUploaderTool();

                taskbar.Show();

                //Step 1 - Login to WebAccess site with user as Reciever User and required language

                // set test locale
                String locale = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Locale");
                localization.UpdateLocalization(locale);

                ++ExecutedSteps;
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.ph1UserName, Config.ph1Password, Language);

                //Enable connections in user options
                userpref.OpenUserPreferences();
                userpref.SwitchToUserPrefFrame();
                if (!userpref.EnableConnectionTestTool().Selected)
                    userpref.EnableConnectionTestTool().Click();
                userpref.CloseUserPreferences();

                //Step 2 - Navigate to Inbounds page, and load an uploaded study into Universal Viewer
                ++ExecutedSteps;
                //Select Inbounds button
                inbounds = (Inbounds)login.Navigate("Inbounds", 1, "Inbounds");
                inbounds.SearchStudy(AccessionNo: Accession, Date: "All", Study_Performed_Period: "All Dates", Study_Received_Period: "All Dates");

                //Select Study
                PageLoadWait.WaitForSearchLoad();
                inbounds.SelectStudy(GetStudyGridColName("Accession"), Accession);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();

                //Step 3 - Sanity check, that UI elements on the Unversal Viewer are localized.
                ++ExecutedSteps;
                // Validate Global Toolbar
                bool IsGlobalToolbarLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "GlobalToolbar", path: "other", viewer: "bluring");

                // Select Collaboration Window and elements are localized.
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingCollaboration).Click();
                bool IsCollaboratgionLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "Collaboration", path: "other", viewer: "bluring");
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingCollaboration).Click();

                // Validate That Network Connection Window elements are localized.
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingNetworkConnection).Click();
                bool IsNetWorkConnectionLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "NetworkConnection", path: "other", viewer: "bluring");
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingNetworkConnection).Click();

                // Validate That Show/Hide drop down menu items are localized.
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingShowHide).Click();
                bool IsNetShowHideLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "ShowHide", path: "other", viewer: "bluring");
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingShowHide).Click();

                // Validate That Help Panel dropdown items are localized.
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingHelp).Click();
                bool IsHelpLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "HelpPanel", path: "other", viewer: "bluring");
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingHelp).Click();

                // Validate that labels like EXAM LIST, HISTORY, Modality and Sort By are localized
                bool IsPatientHistoryLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "PatientHistory", path: "other", viewer: "bluring");

                // Validate Options "All", Title "modality" and "Clear all" are localized
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingRelatedStudiesMultiSelect).Click();
                bool IsPatientHistoryModalityLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "PatientHistoryModality", path: "other", viewer: "bluring");
                GetElement(SelectorType.CssSelector, BluRingViewer.cdk_OverlayContainer).Click();

                // Validate Options Sort By, Date-Newest, Date-Oldest and Modality Type are Localized
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingRelatedStudiesSingleSelect).Click();
                bool IsPatientHistorySortByLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "PatientHistorySortBy", path: "other", viewer: "bluring");
                GetElement(SelectorType.CssSelector, BluRingViewer.cdk_OverlayContainer).Click();

                // Validate User Settings are Localized
                bool IsUserSettingsLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "UserSettings", path: "other", viewer: "bluring");

                //Title panel elementgs
                String[] titlePanelElements = { BluRingViewer.div_toolbarEmailStudyWrapper, BluRingViewer.div_ToolbarLayoutWrapper, BluRingViewer.div_studypaneldate, BluRingViewer.div_studypaneltime };
                bool IsTitlePanelLocaloized = bluringviewer.ValidateLocalizationStudyTitle(studyDateTimeArray, titlePanelElements);

                    if (IsGlobalToolbarLocalized &&
                    IsCollaboratgionLocalized &&
                    IsNetWorkConnectionLocalized &&
                    IsNetShowHideLocalized &&
                    IsHelpLocalized &&
                    IsPatientHistoryLocalized &&
                    IsPatientHistoryModalityLocalized &&
                    IsPatientHistorySortByLocalized &&
                    IsUserSettingsLocalized  &&
                    IsTitlePanelLocaloized)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                // Step 4 - Logout and login again as a Sender user 
                //Close viewer
                ++ExecutedSteps;
                bluringviewer.CloseBluRingViewer();
                login.Logout();

                //Login as sender
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.phUserName, Config.phPassword, Language);

                //Enable connections in user options
                userpref.OpenUserPreferences();
                userpref.SwitchToUserPrefFrame();
                if (!userpref.EnableConnectionTestTool().Selected)
                    userpref.EnableConnectionTestTool().Click();
                userpref.CloseUserPreferences();

                //Step 5 - Navigate to Outbounds page, and load an uploaded study into Universal Viewer
                ++ExecutedSteps;
                //Select Outbounds button
                outbounds = (Outbounds)login.Navigate("Outbounds", 1, "Outbounds");
                outbounds.SearchStudy(AccessionNo: Accession, Date: "All", Study_Performed_Period: "All Dates", Study_Received_Period: "All Dates");

                //Select Study
                PageLoadWait.WaitForSearchLoad();
                outbounds.SelectStudy(GetStudyGridColName("Accession"), Accession);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();


                //Step 6 - Sanity check, that UI elements on the Unversal Viewer are localized.
                ++ExecutedSteps;

                // Validate Global Toolbar
                IsGlobalToolbarLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "GlobalToolbar", path: "other", viewer: "bluring");

                // Select Collaboration Window and elements are localized.
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingCollaboration).Click();
                IsCollaboratgionLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "Collaboration", path: "other", viewer: "bluring");
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingCollaboration).Click();

                // Validate That Network Connection Window elements are localized.
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingNetworkConnection).Click();
                IsNetWorkConnectionLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "NetworkConnection", path: "other", viewer: "bluring");
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingNetworkConnection).Click();

                // Validate That Show/Hide drop down menu items are localized.
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingShowHide).Click();
                IsNetShowHideLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "ShowHide", path: "other", viewer: "bluring");
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingShowHide).Click();

                // Validate That Help Panel dropdown items are localized.
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingHelp).Click();
                IsHelpLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "HelpPanel", path: "other", viewer: "bluring");
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingHelp).Click();

                // Validate that labels like EXAM LIST, HISTORY, Modality and Sort By are localized
                IsPatientHistoryLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "PatientHistory", path: "other", viewer: "bluring");

                // Validate Options "All", Title "modality" and "Clear all" are localized
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingRelatedStudiesMultiSelect).Click();
                IsPatientHistoryModalityLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "PatientHistoryModality", path: "other", viewer: "bluring");
                GetElement(SelectorType.CssSelector, BluRingViewer.cdk_OverlayContainer).Click();

                // Validate Options Sort By, Date-Newest, Date-Oldest and Modality Type are Localized
                GetElement(SelectorType.CssSelector, BluRingViewer.div_BluRingRelatedStudiesSingleSelect).Click();
                IsPatientHistorySortByLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "PatientHistorySortBy", path: "other", viewer: "bluring");
                GetElement(SelectorType.CssSelector, BluRingViewer.cdk_OverlayContainer).Click();

                // Validate User Settings are Localized
                IsUserSettingsLocalized = ValidateLocalization(BluringViewer_MappingFilePath, "UserSettings", path: "other", viewer: "bluring");

                //Title panel elementgs
                IsTitlePanelLocaloized = bluringviewer.ValidateLocalizationStudyTitle(studyDateTimeArray, titlePanelElements);

                if (IsGlobalToolbarLocalized &&
                IsCollaboratgionLocalized &&
                IsNetWorkConnectionLocalized &&
                IsNetShowHideLocalized &&
                IsHelpLocalized &&
                IsPatientHistoryLocalized &&
                IsPatientHistoryModalityLocalized &&
                IsPatientHistorySortByLocalized &&
                IsUserSettingsLocalized &&
                IsTitlePanelLocaloized)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Close viewer
                GetElement(SelectorType.CssSelector, BluRingViewer.cdk_OverlayContainer).Click();
                bluringviewer.CloseBluRingViewer();
                login.Logout();

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                GC.Collect();
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Close browser
                Driver.Manage().Cookies.DeleteAllCookies();
                this.CloseBrowser();

                taskbar.Show();

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

                //Return Result
                return result;
            }
        }



        /// <summary>
        /// Verify Date And Time Formats in Universal Viewer with Internationalization
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_164682(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            String savedLocale = Config.Locale;
            Localization localization = new Localization();

            try
            {

                //Fetch required Test data        
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String Language = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Languages");
                string dataSource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSource");
                string studyDateTime = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDate");
                string thumbnailDate = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDate");
                string thumbnailModaility = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailModality");
                string reportDate = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ReportDate");
                string reportTime = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ReportTime");

                // set test locale
                String locale = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Locale");
                localization.UpdateLocalization(locale);

                //Step 1 - Login to WebAccess site with any privileged user and required language
                ++ExecutedSteps;
                login.Logout();
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword, Language);

                //Step 2 - Load any study in Universal viewer which has all patient information and report
                ++ExecutedSteps;
                studies = (Studies)login.Navigate("Studies", 1, "Studies");
                studies.SearchStudy(AccessionNo: Accession, Datasource: dataSource);
                studies.SelectStudy(GetStudyGridColName("Accession"), Accession);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();

                //Step 3 - Validate that the Study Date Format (Month and AM/PM) in Exam Card are localized
                ++ExecutedSteps;
                String examCardDateTimeRetrieved = Driver.FindElement(By.CssSelector(BluRingViewer.examCardDate)).GetAttribute("innerText");
                if (examCardDateTimeRetrieved.Equals(studyDateTime))
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

                //Step 4 - Validate that the ToolTip (Modality, Date) in thumbnail Preview Card are localized
                ++ExecutedSteps;
                GetElement(SelectorType.CssSelector, BluRingViewer.div_thumbnailpreviewIconActiveStudy).Click();
                GetElement(SelectorType.CssSelector, BluRingViewer.div_ExamList_thumbnails).Click();

                var prior = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList_thumbnails))[0];
                bluringviewer.HoverElement(prior);
                var tooltip = prior.GetAttribute("title");

                String expectedTooldip = Config.Locale.Split('-')[0] + "Modality:" + Config.Locale.Split('-')[1] + thumbnailModaility + Environment.NewLine + 
                                         Config.Locale.Split('-')[0] + "Date:" + Config.Locale.Split('-')[1] + thumbnailDate;
                if (tooltip.Equals(expectedTooldip))
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

                //Step 5 - Validate that Report Tab is Localized.
                ++ExecutedSteps;
                GetElement(SelectorType.CssSelector, BluRingViewer.div_thumbnailpreviewIconActiveStudy).Click();
                GetElement(SelectorType.CssSelector, "div.onMouseHover:nth-child(3)").Click();
                String reportTabLabelRetrieved = Driver.FindElement(By.CssSelector(BluRingViewer.div_Reports)).GetAttribute("innerText");

                String expectedReportTabLabel = reportDate + Environment.NewLine + reportTime + Environment.NewLine;
                if (reportTabLabelRetrieved.Equals(expectedReportTabLabel))
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

                //Close viewer
                bluringviewer.CloseBluRingViewer();
                login.Logout();

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                GC.Collect();
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Close browser
                Driver.Manage().Cookies.DeleteAllCookies();
                this.CloseBrowser();

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Study Viewer
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_164671(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            String AdminUsername = Config.adminUserName;
            String AdminPassword = Config.adminPassword;
            String SuperAdmin = Config.adminGroupName;
            Random random = new Random();

            String savedLocale = Config.Locale;
            Localization localization = new Localization();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data               
                String SuperRole = Config.adminRoleName;
                String Languages = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Languages");
                String[] Langs = Languages.Split(':');
                String StudyID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyID");
                String FilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String[] Filepaths = FilePath.Split('=');
                String Acc = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accessions = Acc.Split(':');
                String Firstname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String[] Firstnames = Firstname.Split(':');
                String Lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String[] Lastnames = Lastname.Split(':');
                String PID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] PatientIds = PID.Split(':');
                String PACSA7 = login.GetHostName(Config.SanityPACS);
                String DCM4CHEE = "DCM4CHEE";
                String EA91 = login.GetHostName(Config.EA91);
                String ICA_MappingFilePath = Config.ica_Mappingfilepath;
                String Preset = "テスト_" + random.Next(1, 1000);

                // set test locale
                String locale = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Locale");
                localization.UpdateLocalization(locale);

                //GridColumnNames
                String StudyIDCol = GetStudyGridColName("Study ID");
                String AccCol = GetStudyGridColName("Accession");
                String PatCol = GetStudyGridColName("Patient ID");
        //        String NameCol = ReadDataFromResourceFile(Localization.AttachmentViewer, "data", "AttachmentTable_NameHeading");
                String AutoLayout = ReadDataFromResourceFile(Localization.ViewingProtocolsControl, "data", "auto_layout");
                String AddTextTooltip = ReadDataFromResourceFile(Localization.Tooltip, "data", "AnnotationTextAdd");
                String EditAnnTooltip = ReadDataFromResourceFile(Localization.Tooltip, "data", "AnnotationEdit");
                String ResetTooltip = ReadDataFromResourceFile(Localization.Tooltip, "data", "Reset");
                String EllipseToolTip = ReadDataFromResourceFile(Localization.Tooltip, "data", "AnnotationEllipse");

                //Step:1-Login in iCA as Administrator with a preferred (target)language
                login.DriverGoTo(url);
                login.PreferredLanguageSelectList().SelectByText(Langs[1]);
                login.LoginIConnect(AdminUsername, AdminPassword);
                DomainManagement domainmanagement = (DomainManagement)login.Navigate("DomainManagement", 1, "Domain");
                domainmanagement.SearchDomain(SuperAdmin);
                domainmanagement.SelectDomain(SuperAdmin);
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForFrameLoad(10);
                
                // Add available tools to tooltip
                IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(9)"));
                IWebElement group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(10)"));
                var dictionary = new Dictionary<String, IWebElement>();

                string saveSeriesTool = ReadDataFromResourceFile(Localization.Tooltip, "data", "SaveSeries");
                string saveAnnotatedImageTool = ReadDataFromResourceFile(Localization.Tooltip, "data", "SaveAnnotatedImage");
                string imageScopeTool = ReadDataFromResourceFile(Localization.Tooltip, "data", "ImageScope");
                string seriesScopeTool = ReadDataFromResourceFile(Localization.Tooltip, "data", "SeriesScope");

                //Tooltip
                dictionary.Add(saveSeriesTool, group1);
                domainmanagement.AddToolsToToolbox(dictionary);
                dictionary.Add(seriesScopeTool, group1);
                domainmanagement.AddToolsToToolbox(dictionary);
                dictionary.Add(saveAnnotatedImageTool, group1);
                domainmanagement.AddToolsToToolbox(dictionary);
                dictionary.Add(imageScopeTool, group2);
                domainmanagement.AddToolsToToolbox(dictionary);
                PageLoadWait.WaitForPageLoad(20);




                domainmanagement.SaveButton().Click();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                UserManagement usermgt = (UserManagement)login.Navigate("UserManagement", 1, "User");
                usermgt.DomainDropDown().SelectByText(SuperAdmin);
                String userid = Lastnames[0] + random.Next(1, 1000);
                usermgt.CreateUser(userid, SuperRole, FName: Firstnames[0] + random.Next(1, 1000), LName: userid);
                bool user = usermgt.SearchUser(userid, SuperAdmin);
                if (user)
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

                //Step-2:Load any study to the viewer
                ++ExecutedSteps;
                studies = (Studies)login.Navigate("Studies", 1, "Studies");
                studies.ChooseColumns(new string[] { StudyIDCol }, 1);
                studies.SearchStudy(Datasource: PACSA7);
                studies.SelectStudy1(StudyIDCol, StudyID);

                bluringviewer = BluRingViewer.LaunchBluRingViewer();

                //Step-3:Checking the following tools, hover texts, dropdown menu:           
                //Check the hover text for the following button and options from Viewer:
                //--Close Viewer
                string CloseviewerTooltip = ReadDataFromJsonFile(Localization.ToolbarConfigSettingsJsonFile, "StudyPanelControlComponent.Close.Title");
                String closeToolTip = GetElement(SelectorType.CssSelector, "div.closeButton:nth-child(3)").GetAttribute("title");

                try
                {
                    if (bluringviewer.ValidateLocalizationStudyToolbox() && closeToolTip.Equals(CloseviewerTooltip))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                }
                catch (Exception e)
                {
                    //Log Exception
                    Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                bluringviewer.CloseBluRingViewer();


                //Step:4-Load a study that was created in target language
                studies = (Studies)login.Navigate("Studies", 1, "Studies");
                studies.SearchStudy(AccessionNo: Accessions[0], Datasource: DCM4CHEE);
                studies.SelectStudy1(AccCol, Accessions[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();

                String brLastname = bluringviewer.PatientDetailsInViewer()["LastName"];
                String brFirstname = bluringviewer.PatientDetailsInViewer()["FirstName"];

                if (brFirstname.Equals(Firstnames[0]) && brLastname.Equals(Lastnames[0]))
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
                bluringviewer.CloseBluRingViewer();


                //Step-5:Load studies in target language as per the test data created
                ++ExecutedSteps;
                studies = (Studies)login.Navigate("Studies", 1, "Studies");
                studies.SearchStudy(AccessionNo: Accessions[1], Datasource: DCM4CHEE);
                studies.SelectStudy1(PatCol, PatientIds[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                bluringviewer.CloseBluRingViewer();


                //Step-6:Load a study having a PR with any annotation (values present in text Attributes of the DICOM content)
                ++ExecutedSteps;
                studies = (Studies)login.Navigate("Studies", 1, "Studies");
                studies.SearchStudy(patientID: PatientIds[1], Datasource: DCM4CHEE);
                studies.SelectStudy1(PatCol, PatientIds[1]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();

                //Step-7:Input texts (in target language) on the image from Measurement tools                
                String addTool = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.AddText.Tooltip");
                bluringviewer.OpenViewerToolsPOPUp();
                bluringviewer.SelectViewerToolByName(addTool);
                BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_viewport)).GetCssValue("cursor").Equals("crosshair");
                bluringviewer.ApplyTool_AddText(Lastnames[0]);

                IWebElement Viewport = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_viewport));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);

                if (studies.CompareImage(result.steps[ExecutedSteps], Viewport, totalImageCount: 1))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-8:Save the Annotation
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-9:Reload this PR series
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-10:Logout and log back in using user created in target language (within the domain)
                login.Logout();
                login.DriverGoTo(url);
                login.PreferredLanguageSelectList().SelectByText(Langs[1]);
                login.LoginIConnect(userid, userid);
                ExecutedSteps++;

                //Step-11:Load a study that has multiple series
                ExecutedSteps++;
                studies = (Studies)login.Navigate("Studies", 1, "Studies");
                studies.SearchStudy(patientID: PatientIds[2]);
                studies.SelectStudy1(PatCol, PatientIds[2]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();

                //Step-12:Add a few measurements, combined with input text in target language on a few images from different series
                String addTool11 = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.AddText.Tooltip");
                bluringviewer.OpenViewerToolsPOPUp();
                bluringviewer.SelectViewerToolByName(addTool11);
                BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_viewportNo(1))).GetCssValue("cursor").Equals("crosshair");
                bluringviewer.ApplyTool_AddText(Lastnames[0]);

                String addTool112 = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.AddText.Tooltip");
                bluringviewer.SetViewPort1(1,2);
                bluringviewer.OpenViewerToolsPOPUp();
                bluringviewer.SelectViewerToolByName(addTool112);
                BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_viewportNo(2))).GetCssValue("cursor").Equals("crosshair");
                bluringviewer.ApplyTool_AddText(Firstnames[0]);


                IWebElement Viewport11 = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_viewportNo(1)));
                IWebElement Viewport12 = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_viewportNo(2)));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);

                if (studies.CompareImage(result.steps[ExecutedSteps], Viewport11, totalImageCount: 1))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);

                if (studies.CompareImage(result.steps[ExecutedSteps], Viewport12, totalImageCount: 1))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-13:Save series
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-14:Load a data which is not calibrated. Draw few measurements, then select to calibrate the image. and input decimal value
                ExecutedSteps++;
                bluringviewer.CloseBluRingViewer();
                studies = (Studies)login.Navigate("Studies", 1, "Studies");
                studies.SearchStudy(patientID: PatientIds[2]);
                studies.SelectStudy1(PatCol, PatientIds[2]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();

                String addTool14 = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.CobbAngle.Tooltip");
                bluringviewer.OpenViewerToolsPOPUp();
                bluringviewer.SelectViewerToolByName(addTool14);
                BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_viewport)).GetCssValue("cursor").Equals("crosshair");
                bluringviewer.ApplyTool_CobbAngle();

                IWebElement Viewport14 = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_viewport));

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);

                if (studies.CompareImage(result.steps[ExecutedSteps], Viewport14, totalImageCount: 1))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-15:Load a study which has PR series with measurements on it in target language
                bluringviewer.CloseBluRingViewer();
                studies = (Studies)login.Navigate("Studies", 1, "Studies");
                studies.SearchStudy(patientID: PatientIds[2]);
                studies.SelectStudy1(PatCol, PatientIds[2]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step-16:Input some text in the target language (ex. Chinese) from Add text
                String addTool16 = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.AddText.Tooltip");
                bluringviewer.OpenViewerToolsPOPUp();
                bluringviewer.SelectViewerToolByName(addTool16);
                BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_viewport)).GetCssValue("cursor").Equals("crosshair");
                bluringviewer.ApplyTool_AddText(Lastnames[0] + ", " + Firstnames[0]);

                IWebElement Viewport16 = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_viewport));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);

                if (studies.CompareImage(result.steps[ExecutedSteps], Viewport16, totalImageCount: 1))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-17:Save the annotation and reload the image back to active viewport
                result.steps[++ExecutedSteps].status = "Not Automated";


                //Step-18:Draw a rectangle
                String addTool18 = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.Rectangle.Tooltip");
                bluringviewer.OpenViewerToolsPOPUp();
                bluringviewer.SelectViewerToolByName(addTool18);
                BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_viewport)).GetCssValue("cursor").Equals("crosshair");
                bluringviewer.ApplyTool_DrawRectangle();
                
                IWebElement Viewport18 = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_viewport));

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);

                if (studies.CompareImage(result.steps[ExecutedSteps], Viewport18, totalImageCount: 1))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-19:Draw an Ellipse
                bluringviewer.CloseBluRingViewer();
                studies = (Studies)login.Navigate("Studies", 1, "Studies");
                studies.SearchStudy(patientID: PatientIds[2]);
                studies.SelectStudy1(PatCol, PatientIds[2]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();

                String addTool19 = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.Ellipse.Tooltip");
                bluringviewer.OpenViewerToolsPOPUp();
                bluringviewer.SelectViewerToolByName(addTool19);
                BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_viewport)).GetCssValue("cursor").Equals("crosshair");
                bluringviewer.ApplyTool_DrawEllipse();

                IWebElement Viewport19 = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_viewport));

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);

                if (studies.CompareImage(result.steps[ExecutedSteps], Viewport19, totalImageCount: 1))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-20:Save the series
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-21:Re-load the study.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //viewer.CloseStudy();
                bluringviewer.CloseBluRingViewer();
                login.Logout();

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

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

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }

        }


        /// <summary>
        /// Studies List & Search
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_164672(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables               
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            String savedLocale = Config.Locale;
            Localization localization = new Localization();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            String dataSourceName = "DCM4CHEE";
            String Firstname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
            String Lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
            String[] Firstnames = Firstname.Split(':');
            String[] Lastnames = Lastname.Split(':');

            String FromDate = "23-7-2000";
            //String todate = String.Format("{0:dd-MMM-yyyy}", DateTime.Now);
            //DateTime fromdt = DateTime.ParseExact(FromDate, "dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
            //DateTime todt = DateTime.ParseExact(todate, "dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
            String todate = "23-7-2017";

            By fromdatecalender = By.CssSelector("#DateRangeSelectorCalendarFrom_calendar");
            By todatecalender = By.CssSelector("#DateRangeSelectorCalendarTo_calendar");

            try
            {
                //Fetch required Test data                
                String Languages = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Languages");
                String Reason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Description");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String pid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String Names = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                String lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String SearchName = Names.Split(':')[0] + "_" + new Random().Next(1, 1000);

                String[] Langs = Languages.Split(':');
                String[] Accessions = AccessionList.Split(':');


                // Enable Patients from the Service Tool and restart services
                servicetool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                servicetool.WaitWhileBusy();
                servicetool.EnablePatient();
                servicetool.ApplyEnableFeatures();
                servicetool.AcceptDialogWindow();
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                servicetool.WaitWhileBusy();
                servicetool.CloseConfigTool();

                //Step 1 - Send Datasets created in Japanese ,Chinese from Japanese & Chinese OS respectively                

                //Other Precondition.
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement = login.Navigate<DomainManagement>();
                domainmanagement.SearchDomain(Config.adminGroupName);
                domainmanagement.SelectDomain(Config.adminGroupName);
                domainmanagement.ClickEditDomain();
                domainmanagement.VisibleAllStudySearchField();
                domainmanagement.ClickSaveNewDomain();

                PageLoadWait.WaitForPageLoad(20);

                rolemanagement = login.Navigate<RoleManagement>();

                rolemanagement.ShowRolesFromDomainDropDown();
                rolemanagement.SelectDomainfromDropDown("SuperAdminGroup");
                rolemanagement.SelectRole("SuperRole");
                PageLoadWait.WaitForPageLoad(20);
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckbox(rolemanagement.StudySearchFieldUseDomainSetting_CB());
                rolemanagement.ClickSaveRole();
                PageLoadWait.WaitForPageLoad(20);

                login.Logout();
                ExecutedSteps++;

                //Step 2 - Verify the label of th Tabs -7 tabs

                // set test locale
                String locale = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Locale");
                localization.UpdateLocalization(locale);

                login.DriverGoTo(login.url);
                login.PreferredLanguageSelectList().SelectByText(Langs[1]);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                String Studies = ReadDataFromResourceFile(Localization.GlobalResourceFilePath, "data", "Tab_" + "Studies");
                String Patients = ReadDataFromResourceFile(Localization.GlobalResourceFilePath, "data", "Tab_" + "Patients");
                String DomainManagement = ReadDataFromResourceFile(Localization.GlobalResourceFilePath, "data", "Tab_" + "Domain");
                String RoleManagement = ReadDataFromResourceFile(Localization.GlobalResourceFilePath, "data", "Tab_" + "Role");
                String UserManagement = ReadDataFromResourceFile(Localization.GlobalResourceFilePath, "data", "Tab_" + "User");
                String SystemSettings = ReadDataFromResourceFile(Localization.GlobalResourceFilePath, "data", "Tab_" + "System");
                String Maintenance = ReadDataFromResourceFile(Localization.GlobalResourceFilePath, "data", "Tab_" + "Maintenance");

                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");

                if (GetElement("cssselector", "div[id^='TabText0']").Text.Equals(Studies)
                   && GetElement("cssselector", "div[id^='TabText1']").Text.Equals(Patients)
                   && GetElement("cssselector", "div[id^='TabText2']").Text.Equals(DomainManagement)
                   && GetElement("cssselector", "div[id^='TabText3']").Text.Equals(RoleManagement)
                   && GetElement("cssselector", "div[id^='TabText4']").Text.Equals(UserManagement)
                   && GetElement("cssselector", "div[id^='TabText5']").Text.Equals(SystemSettings)
                   && GetElement("cssselector", "div[id^='TabText6']").Text.Equals(Maintenance))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 3 - Click on the first Tab from left (Studies) and verify the contents of the page in target language
                studies = (Studies)login.Navigate("Studies", 1, "Studies");

                //The text for Searching fields are in target language                
                //All Column headers - TODO
                //View Study ,Clear ,Search button is in target language
                //Group By text and column headers in dropdown should be in target language - TODO     
                //Total number of studies count are in target language - TODO
                //3 options (Administrator Logout, Options, Help) are all in target language - false
                bool bStudies = ValidateLocalization(ICA_MappingFilePath, "Studies", path: "resource", viewer: "bluring");

                //Hide Search Criteria text
                bool bHideSearch = GetElement("cssselector", "#ExpandSearchPanelButton").GetAttribute("title").Equals(GetVariableValueFromJSFile(Localization.GlobalResourceJSFile, "hideSearch"));

                //Warning (in red) for query items exceeded limit is in target language
                studies.SearchStudy(LastName: "*", Study_Performed_Period: "Last Hour");
                PageLoadWait.WaitForLoadingMessage(65);
                bool bWarningMsg = GetElement("cssselector", "#m_studyGrid_m_messageLabel").Text.Equals(ReadDataFromResourceFile(Localization.Study, "data", "DefaultErrorMessage"));

                //If no records displayed text No records to view should be in target language
                bool bNoRecords = GetElement("cssselector", "#gridPagerDivStudyList_right div[class='ui-paging-info']").GetAttribute("innerText").Equals(GetVariableValueFromJSFile(Localization.GridLocaleJSFile, "emptyrecords"));

                //Button label (Choose Columns ,Reset) should be in target language
                bool bChooseColumns = GetElement("cssselector", "#gridPagerDivStudyList_left table tbody tr td:nth-child(1) div[class='ui-pg-div']").GetAttribute("innerText").Equals(GetVariableValueFromJSFile(Localization.GlobalResourceJSFile, "ChooseColumnsButton"));
                bool bReset = GetElement("cssselector", "#gridPagerDivStudyList_left table tbody tr td:nth-child(3) div[class='ui-pg-div']").GetAttribute("innerText").Equals(GetVariableValueFromJSFile(Localization.GlobalResourceJSFile, "ResetButton"));

                if (bStudies && bNoRecords && bChooseColumns && bReset && bWarningMsg)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 4 - Browse through the tabs
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 5 - Click on each header per column - TODO
                studies.SearchStudy(LastName: "*", Datasource: "All");
                PageLoadWait.WaitForLoadingMessage(45);
                //string[] colnames = new string[] { "Modality", "Patient Name", "Patient ID", "Accession" };
                string[] colnames = new string[] { "Modality" };
                studies.SearchStudy(Modality: "MR");
                bool[] step5 = new bool[colnames.Length];
                for (int i = 0; i < colnames.Length; i++)
                {
                    studies.ClickColumnHeading(colnames[i]);
                    Thread.Sleep(700);
                    string[] step5_1 = studies.GetStudyDetails(colnames[i]);
                    step5[i] = (step5_1 == null || step5_1.Length == 0) ? false : step5_1.Select(s => s.ToLower()).SequenceEqual((step5_1.OrderBy(q => q)).Select(s => s.ToLower()));
                }
                if (studies.ValidateBoolArray(step5))
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

                //Step 6 - Click on Choose Columns button on left bottom corner of search results section
                ClickChooseColumns(check: 1);
                string RemoveAllLbl = GetVariableValueFromJSFile(Localization.GridLocaleJSFile, "removeAll");
                string AddAllLbl = GetVariableValueFromJSFile(Localization.GridLocaleJSFile, "addAll");
                string SelectColLbl = GetVariableValueFromJSFile(Localization.GridLocaleJSFile, "caption", "col");
                string itemsSelectedLbl = GetVariableValueFromJSFile(Localization.GridLocaleJSFile, "itemsCount");
                string OkBtn = GetVariableValueFromJSFile(Localization.GridLocaleJSFile, "bSubmit", "col");
                string CancelBtn = GetVariableValueFromJSFile(Localization.GridLocaleJSFile, "bCancel", "col");
                string itemsLbl = itemsSelected().Text;
                string SelectCol = SelectColumns().Text;
                string RemoveLbl = RemoveAllLink().GetAttribute("innerHTML");
                string AddLbl = AddAllLink().GetAttribute("innerHTML");

                if (itemsLbl.Contains(itemsSelectedLbl) && SelectColLbl.Equals(SelectCol) && RemoveAllLbl.Equals(RemoveLbl) &&
                    AddAllLbl.Equals(AddLbl) && OkBtn.Equals(OKChooseColLbl()) && CancelBtn.Equals(CancelChooseColLbl())
)                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                PageLoadWait.WaitForFrameLoad(10);
                CancelButton_ChooseColumns().Click();
                PageLoadWait.WaitForFrameLoad(10);

                //Step 7 - Verify the button label for My Search, Preset , Delete
                //Verified in Setp 3
                ExecutedSteps++;

                //Step 8 - Click on the arrow on extreme right side to Hide Search criteria Verify tool tip
                if (GetElement("cssselector", "#ExpandSearchPanelButton").GetAttribute("title").Equals(GetVariableValueFromJSFile(Localization.GlobalResourceJSFile, "hideSearch")))
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

                //Step 9 - Click on My Search                
                SwitchToDefault();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                Driver.FindElement(By.CssSelector("#m_studySearchControl_m_defaultSearchButton")).Click();
                if (GetText("id", "SearchWarningSpan").Equals(GetVariableValueFromJSFile(Localization.GlobalResourceJSFile, "noMySearchDefinedWarning")))
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

                //Step 10 - Click Save. Enter search text name in target language for specific search criteria. Click Save
                studies.SearchStudy(Modality: "US", Study_Performed_Period: "All Dates");
                PageLoadWait.WaitForFrameLoad(30);
                ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector('#m_studySearchControl_m_saveSearchButton').click()");
                studies.ClickButton("#m_studySearchControl_m_saveSearchButton");
                Driver.FindElement(studies.SearchName()).SendKeys(SearchName);
                studies.ClickElement(BasePage.Driver.FindElement(studies.SaveSearch()));
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(studies.SavePopup()));
                ExecutedSteps++;

                //Step 11 - Click on arrow for Search Preset and select saved Preset. Click Search.
                var searchpreset = new SelectElement(studies.SearchPreset());
                new Actions(BasePage.Driver).Click(studies.SearchPreset());
                bool flag11 = searchpreset.Options.Any<IWebElement>(element => element.GetAttribute("innerHTML").Trim().Equals(SearchName));
                if (flag11)
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

                //Step 12 - Select any study and click View study                
                ExecutedSteps++;
                studies = (Studies)login.Navigate("Studies", 1, "Studies");
                studies.SearchStudy(Modality: "US", patientID: "MRN_ABS@012");
                studies.SelectStudy(GetStudyGridColName("Patient ID"), "MRN_ABS@012");
                bluringviewer = BluRingViewer.LaunchBluRingViewer();


                //Step 13 - CLose study. Click on Study Performed field
                bluringviewer.CloseBluRingViewer();
                PageLoadWait.WaitForFrameLoad(30);

                new Actions(BasePage.Driver).MoveToElement(studies.StudyPerformed()).Click().Build().Perform();
                if (Driver.FindElement(By.LinkText("All Dates")).Displayed
                    && Driver.FindElement(By.LinkText("Last Hour")).Displayed
                    && Driver.FindElement(By.LinkText("Last 2 Hours")).Displayed
                    && Driver.FindElement(By.LinkText("Last 5 Hours")).Displayed
                    && Driver.FindElement(By.LinkText("After Midnight")).Displayed
                    && Driver.FindElement(By.LinkText("Last 24 Hours")).Displayed
                    && Driver.FindElement(By.LinkText("Last 2 Days")).Displayed
                    && Driver.FindElement(By.LinkText("Last 7 Days")).Displayed
                    && Driver.FindElement(By.LinkText("Last 14 Days")).Displayed
                    && Driver.FindElement(By.LinkText("Last Month")).Displayed
                    && Driver.FindElement(By.LinkText("Custom Date Range")).Displayed
                    )
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

                //Step 14 - Select Custom date Range and click OK without entering any date range
                PageLoadWait.WaitForFrameLoad(30);
                studies.ClearFields();
                PageLoadWait.WaitForFrameLoad(30);
                studies.SelectCustomeStudySearch(studies.StudyPerformed());
                studies.SubmitButton().Click();
                if (Driver.FindElement(By.CssSelector("#dateErrorLabelFrom")).GetAttribute("innerHTML").Contains("*") &&
                    Driver.FindElement(By.CssSelector("#dateErrorLabelTo")).GetAttribute("innerHTML").Contains("*"))
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

                //Step 15 - Select Custom Data Range and click the field
                studies.FromDate().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(fromdatecalender));
                if (studies.CalenderTable().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Partially Automated";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 16 - Hover on the Clear button ,previous, next arrow icon besides month dropdown
                IWebElement ClearButton = Driver.FindElement(By.CssSelector("input[title='" + ReadDataFromResourceFile(Localization.GlobalResourceFilePath, "data", "Calendar_ClearToolTip") + "']"));
                IWebElement PreviousButton = Driver.FindElement(By.CssSelector("input[title='" + ReadDataFromResourceFile(Localization.GlobalResourceFilePath, "data", "Calendar_monthdn_title") + "']"));
                IWebElement NextButton = Driver.FindElement(By.CssSelector("input[title='" + ReadDataFromResourceFile(Localization.GlobalResourceFilePath, "data", "Calendar_monthup_title") + "']"));
                if (ClearButton.Enabled && PreviousButton.Enabled && NextButton.Enabled)
                {
                    result.steps[++ExecutedSteps].status = "Partially Automated";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 17 - Set the query to a range that have some studies in target language occurred in that period of time and press Search                
                SwitchToDefault();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector('#DateRangeSelectorCalendarFrom_tbody tr:nth-child(4) td div input').click();");
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.getElementById('masterDateFrom').value='" + FromDate + "';");
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.getElementById('masterDateTo').value='" + todate + "';");

                studies.SubmitButton().Click();
                if (Driver.FindElement(By.CssSelector("#searchStudyMainText")).GetAttribute("innerHTML").Equals(FromDate + " - " + todate))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                };

                //Step 18 - Input in target language in Searching field for Patient Name (Last Nam or First Name )who is available in the datasource. Press search.
                studies.ChooseColumns(new string[] { GetStudyGridColName("Last Name"), GetStudyGridColName("First Name") }, 1);
                studies.SearchStudy(LastName: Lastnames[0], FirstName: Firstnames[0], Datasource: dataSourceName);
                Dictionary<int, string[]> results = BasePage.GetSearchResults();
                string[] columnnames = GetColumnNames();
                string[] lastnames = GetColumnValues(results, GetStudyGridColName("Last Name"), columnnames);
                string[] firstnames = GetColumnValues(results, GetStudyGridColName("First Name"), columnnames);
                if (results.Count != 0 && Array.TrueForAll(lastnames, s => s.Equals(Lastnames[0])) && Array.TrueForAll(firstnames, s => s.Equals(Firstnames[0])))
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

                //Step 19 - Load the dataset in target language (e.g.Japanese ,Chinese) to the viewer.
                ExecutedSteps++;
                studies.SelectStudy(GetStudyGridColName("Last Name"), Lastnames[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(30);
                bluringviewer.CloseBluRingViewer();

                //Step 20 - Patient name with multiple components (alphabetic=ideographic=phonetic) is present in a DS
                servicetool.LaunchServiceTool();
                servicetool.NavigateToStudySearch();
                servicetool.ClickModifyFromTab();
                servicetool.SetUISearchFilter("any");
                servicetool.ClickApplyButtonFromTab();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseConfigTool();
                Taskbar taskbar1 = new Taskbar();
                taskbar1.Show();
                ExecutedSteps++;

                //Step 21 -  Search for a patient's First name ' &#12383;&#12429;&#12358; ' (name with multiple components (alphabetic=ideographic=phonetic))
                studies.ChooseColumns(new string[] { GetStudyGridColName("Last Name"), GetStudyGridColName("First Name") }, 1);
                studies.SearchStudy(FirstName: Firstnames[1], Datasource: dataSourceName);
                string[] firstnames_21 = GetColumnValues(GetSearchResults(), GetStudyGridColName("First Name"), GetColumnNames());
                result.steps[++ExecutedSteps].status = "On Hold";

                //Step 22 - Search for a patient's last name ' &#12420;&#12414;&#12384; ' (name with multiple components (alphabetic=ideographic=phonetic))
                studies.SearchStudy(LastName: Lastnames[1], Datasource: dataSourceName);
                string[] lastnames_22 = GetColumnValues(GetSearchResults(), GetStudyGridColName("Last Name"), GetColumnNames());
                result.steps[++ExecutedSteps].status = "On Hold";

                //Step 23 - Search for a patient's ideographic first name or last name (&#23665;&#30000; or &#22826;&#37070;)
                studies.SearchStudy(LastName: Lastnames[1], FirstName: Firstnames[1], Datasource: dataSourceName);
                string[] lastnames_23 = GetColumnValues(GetSearchResults(), GetStudyGridColName("Last Name"), GetColumnNames());
                string[] firstnames_23 = GetColumnValues(GetSearchResults(), GetStudyGridColName("First Name"), GetColumnNames());
                result.steps[++ExecutedSteps].status = "On Hold";

                //Step 24 - Priors are available for the patient with name:
                result.steps[++ExecutedSteps].status = "On Hold";

                //Step 25 - Search for a patient's phonetic first or last name and launch the study to check priors
                result.steps[++ExecutedSteps].status = "On Hold";

                //Step 26 - Search for a patient's alphabetic first or last name and launch the study to check priors
                result.steps[++ExecutedSteps].status = "On Hold";

                //Step 27 - Search for a patient's ideographic first or last name and launch the study to check priors
                result.steps[++ExecutedSteps].status = "On Hold";

                //Step 28 - Perform a studies search action in the studies list with any of the supported parameters. View the wireshark tool for the C-FIND request
                result.steps[++ExecutedSteps].status = "On Hold";

                login.Logout();

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                GC.Collect();
                return result;
            }
         catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                // reset test locale to original
                localization.UpdateLocalization(savedLocale);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                servicetool.LaunchServiceTool();
                servicetool.NavigateToStudySearch();
                servicetool.ClickModifyFromTab();
                servicetool.SetUISearchFilter("exact");
                servicetool.AlphabeticRadioBtn().Click();
                wpfobject.WaitTillLoad();
                servicetool.ClickApplyButtonFromTab();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseConfigTool();
                Taskbar taskbar1 = new Taskbar();
                taskbar1.Show();
            }
        }


    }
}
