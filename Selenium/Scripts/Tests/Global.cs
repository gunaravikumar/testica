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

namespace Selenium.Scripts.Tests
{
    class Global
    {
        public Login login { get; set; }
        public string filepath { get; set; }

        public Global(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }

        /// <summary>
        /// This is case to Verify the patient details on the Global header in BluRing viewer compared with the data source
        /// </summary>
        public TestCaseResult Test_161039(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                DomainManagement domain = new DomainManagement();


                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String patientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String accessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String dobList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DOBList");
                String lastNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastNameList");
                String firstNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstNameList");
                String genderList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "GenderList");
                String middleNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "MiddleNameList");

                String[] accession = accessionList.Split(':');
                String[] patientID = patientIDList.Split(':');
                String[] lastName = lastNameList.Split(':');
                String[] firstName = firstNameList.Split(':');
                String[] gender = genderList.Split(':');
                String[] dob = dobList.Split(':');
                String[] middleName = middleNameList.Split(':');
                String[] date1 = dob[0].Split('/');
                String[] date2 = dob[1].Split('/');


                //Step 1 - Login as Administrator in BluRing application
                login.LoginIConnect(adminUserName, adminPassword);
                if(login.IsTabPresent("Studies") && login.IsTabPresent("Patients") && login.UserId().Text.Equals(adminUserName))
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

                //Step 2 - Navigate to Search and Load the study from EA datasource into BluRing viewer
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: accession[0], Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Accession", accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);

                String PatientName = viewer.GetText("cssselector", BluRingViewer.p_PatientName);

                //if (PatientName.Equals(lastName[0] + ", " + firstName[0] + " " + middleName[0] + "."))
                if (PatientName.Equals(lastName[0] + ", " + firstName[0]))
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

                //Step 3 - In Viewer, Verify the Patient Demographics details with the details in EA Data source 
                String patientDOB = viewer.GetText("cssselector", BluRingViewer.span_PatientDOB);
                String patientAge = viewer.GetText("cssselector", BluRingViewer.div_PatientAge);
                String patientGender = viewer.GetText("cssselector", BluRingViewer.div_PatientGender);
                String pID = viewer.GetText("cssselector", BluRingViewer.div_PatientID);
                int p_Age = Int32.Parse(Regex.Replace(patientAge, @"[^0-9]", ""));
                bool Step3_1 = false;
                bool Step3_2 = false;
                bool Step3_3 = false;
                if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("chrome")) ||
                (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer")))
                {
                    Step3_2 = viewer.CheckColorOfFonts_Labels(BluRingViewer.span_PatientDetailsFont, "rgba(142, 142, 142, 1)");
                    Step3_3 = viewer.CheckColorOfFonts_Labels(BluRingViewer.span_PatientDetailsLabel, "rgba(255, 255, 255, 1)");
                }
                else
                {
                    Step3_2 = viewer.CheckColorOfFonts_Labels(BluRingViewer.span_PatientDetailsFont, "rgb(142, 142, 142)");
                    Step3_3 = viewer.CheckColorOfFonts_Labels(BluRingViewer.span_PatientDetailsLabel, "rgb(255, 255, 255)");
                }

                DateTime dt = new DateTime(Int32.Parse(date1[2]), Int32.Parse(date1[0]), Int32.Parse(date1[1]));
                String FormattedDate = String.Format("{0:dd-MMM-yyyy}", dt);
                if (p_Age == BluRingViewer.CalculateAge(dob[0]) 
                    && patientGender.Equals(gender[0]) 
                    && pID.Equals(patientID[0]) 
                    && patientDOB.Contains(FormattedDate))
                {
                    Step3_1 = true;
                }

                if(Step3_1 && Step3_2 && Step3_3)
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
                viewer.CloseBluRingViewer();

                //Step 4 - Navigate to Search and Load the study from PACS datasource into BluRing viewer
                studies.SearchStudy(patientID: patientID[1], Datasource: login.GetHostName(Config.SanityPACS));
                studies.SelectStudy("Accession", accession[1]);
                BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                String PatientName1 = viewer.GetText("cssselector", BluRingViewer.p_PatientName);

                if (PatientName1.Equals(lastName[1] + ", " + firstName[1] + " " + middleName[1] + "."))
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

                //Step5 -  In Viewer, Verify the Patient Demographics details with the details in PACS Data source
                String patientDOB1 = viewer.GetText("cssselector", BluRingViewer.span_PatientDOB);
                String patientAge1 = viewer.GetText("cssselector", BluRingViewer.div_PatientAge);
                String patientGender1 = viewer.GetText("cssselector", BluRingViewer.div_PatientGender);
                String pID1 = viewer.GetText("cssselector", BluRingViewer.div_PatientID);
                int p_Age1 = Int32.Parse(Regex.Replace(patientAge1, @"[^0-9]", ""));

                bool Step5_1 = false;
                bool Step5_2 = false;
                bool Step5_3 = false;
                if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("chrome")) ||
                (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer")))
                {
                    Step5_2 = viewer.CheckColorOfFonts_Labels(BluRingViewer.span_PatientDetailsFont, "rgba(142, 142, 142, 1)");
                    Step5_3 = viewer.CheckColorOfFonts_Labels(BluRingViewer.span_PatientDetailsLabel, "rgba(255, 255, 255, 1)");
                }
                else
                {
                    Step5_2 = viewer.CheckColorOfFonts_Labels(BluRingViewer.span_PatientDetailsFont, "rgb(142, 142, 142)");
                    Step5_3 = viewer.CheckColorOfFonts_Labels(BluRingViewer.span_PatientDetailsLabel, "rgb(255, 255, 255)");
                }

                DateTime dt1 = new DateTime(Int32.Parse(date2[2]), Int32.Parse(date2[0]), Int32.Parse(date2[1]));
                String FormattedDate1 = String.Format("{0:dd-MMM-yyyy}", dt1);

                if (p_Age1 == BluRingViewer.CalculateAge(dob[1]) && patientGender1.Equals(gender[1]) && pID1.Equals(patientID[1]) && patientDOB1.Contains(FormattedDate1))
                {
                    Step5_1 = true;
                }

                if(Step5_1 && Step5_2 && Step5_3)
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

        /// <summary>
        /// Global Header - Layout
        /// </summary>
        public TestCaseResult Test_161041(String testid, String teststeps, int stepcount)
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

                //Step 1 Launch and Login as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);

                // Verify Studies, Patients and Domain Management tabs
                if (login.IsTabPresent("Studies") && login.IsTabPresent("Patients") && login.IsTabPresent("Domain Management") && login.UserId().GetAttribute("innerHTML").Contains(adminUserName))
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

                //Step 2 - Navigate to Studies tab
                var studies = (Studies)login.Navigate("Studies");
                // Search and select a study
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step 3 - Verify Global title toolbor available
                bool isBrandExists = viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_mergeLogo));
                bool isDivider1Exists = viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_divider + ":nth-child(2)"));
                bool isPatieninfoExists = viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_patientPanel));
                bool isGlobalPrimaryToolsExists = viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_globalPrimaryTools));
                bool isDivider3Exists = viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_globalTitleBar + " div:nth-of-type(6)"));
                bool isGlobalSecondaryToolsExists = viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_globalSecondaryTools));
                int MergeLogo = Convert.ToInt32(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_mergeLogo).Location.X.ToString());
                int SectionDivider1 = Convert.ToInt32(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_divider + ":nth-child(2)").Location.X.ToString());
                int PatientDetails = Convert.ToInt32(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_patientPanel).Location.X.ToString());
                int GlobalPrimaryTools = Convert.ToInt32(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_globalPrimaryTools).Location.X.ToString());
                int GlobalSecondaryTools = Convert.ToInt32(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_globalSecondaryTools).Location.X.ToString());
                if (isBrandExists && isDivider1Exists && isPatieninfoExists
                    && isGlobalPrimaryToolsExists && isDivider3Exists
                    && isGlobalSecondaryToolsExists && SectionDivider1 > MergeLogo
                    && PatientDetails > SectionDivider1 && GlobalSecondaryTools > GlobalPrimaryTools)
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

                //Step 4 and 5 - Verifying Global Primary Tools and Global Secondary Tools				
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step5 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_globalTitleBar));
                ExecutedSteps++;
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

                //step6  Ensure all the tool icons (primary and secondary) are centered within their containers.
                bool step6_1 = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_globalTitleBar).GetCssValue("display").Equals("flex");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step6_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_globalTitleBar));
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

                //Step 7 - Resize the window and verifying the components in Global titlebar
                BasePage.Driver.Manage().Window.Size = new Size(1024, 768);
                PageLoadWait.WaitForPageLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_globalTitleBar));
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

                //Step 8 - Resize the window to Maximum and verifying the components in Global titlebar
                BasePage.Driver.Manage().Window.Maximize();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step8 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_globalTitleBar));
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

                //step9 Ensure that the (x) Exit icon is the last (most right) icon in the Global Secondary Tools.
                bool step9_1 = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_Usercontrolpanel).GetCssValue("float").Equals("right");
                int CloseStudyButton = Convert.ToInt32(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_closeStudy).Location.X.ToString());
                int ToolDivider = Convert.ToInt32(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_tooldivider + ":nth-child(6)").Location.X.ToString());
                if (step9_1 && CloseStudyButton > ToolDivider)
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

                //Step 10 - Click on the (x) Exit icon to close the Viewer, and go back to the study list.
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

                //Logout Application                
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
		///  Global Toolbar UI - Universal viewer
		/// </summary>
		public TestCaseResult Test_161040(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            UserPreferences userpref = new UserPreferences();
            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String patientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String lastNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastNameList");
                String dobList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DOBList");
                String firstNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstNameList");
                String genderList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "GenderList");
                String middleNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "MiddleNameList");                                             

                Studies study = new Studies();
                BluRingViewer viewer = new BluRingViewer();
                String[] accession = accessionList.Split(':');
                String[] patientID = patientIDList.Split(':');
                String[] lastName = lastNameList.Split(':');
                String[] firstName = firstNameList.Split(':');
                String[] gender = genderList.Split(':');
                String[] dob = dobList.Split(':');
                String[] middleName = middleNameList.Split(':');
                String[] date1 = dob[0].Split('/');
                String[] date2 = dob[1].Split('/');

                //Pre-condition 
                // In User Preferences set the Thumbnail Splitting and Viewing Scope as Image.
                login.LoginIConnect(adminUserName, adminPassword);
                study.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpref.ModalityDropDown().SelectByText("US");
                userpref.ThumbnailSplittingImageRadioBtn().Click();
                userpref.ViewingScopeImageRadioBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                study.CloseUserPreferences();
                login.Logout();


                //Step 1 - Login as Administrator
                login.LoginIConnect(adminUserName, adminPassword);
                if (login.IsTabPresent("Studies") && login.IsTabPresent("Patients") && login.UserId().Text.Equals(adminUserName))
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

                //Step 2 - Navigate to Search and Load the study from EA datasource into BluRing viewer
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: accession[0], Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Accession", accession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                String PatientName = viewer.GetText("cssselector", BluRingViewer.p_PatientName);                
                if (PatientName.Equals(lastName[0] + ", " + firstName[0]))
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

                //Step 3 - In Viewer, Verify the Patient Demographics details with the details in EA Data source 
                String patientDOB = viewer.GetText("cssselector", BluRingViewer.span_PatientDOB);
                String patientAge = viewer.GetText("cssselector", BluRingViewer.div_PatientAge);
                String patientGender = viewer.GetText("cssselector", BluRingViewer.div_PatientGender);
                String pID = viewer.GetText("cssselector", BluRingViewer.div_PatientID);
                int p_Age = Int32.Parse(Regex.Replace(patientAge, @"[^0-9]", ""));               
                DateTime dt = new DateTime(Int32.Parse(date1[2]), Int32.Parse(date1[0]), Int32.Parse(date1[1]));
                String FormattedDate = String.Format("{0:dd-MMM-yyyy}", dt);
                if (p_Age == BluRingViewer.CalculateAge(dob[0])
                    && patientGender.Equals(gender[0])
                    && pID.Equals(patientID[0])
                    && patientDOB.Contains(FormattedDate))
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
                viewer.CloseBluRingViewer();

                //Step 4 - Navigate to Search and Load the study from PACS datasource into BluRing viewer
                studies.SearchStudy(patientID: patientID[1], Datasource: login.GetHostName(Config.SanityPACS));
                studies.SelectStudy("Accession", accession[1]);
                BluRingViewer.LaunchBluRingViewer();              
                String PatientName1 = viewer.GetText("cssselector", BluRingViewer.p_PatientName);
                if (PatientName1.Equals(lastName[1] + ", " + firstName[1] + " " + middleName[1] + "."))
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

                //Step5 -  In Viewer, Verify the Patient Demographics details with the details in PACS Data source
                String patientDOB1 = viewer.GetText("cssselector", BluRingViewer.span_PatientDOB);
                String patientAge1 = viewer.GetText("cssselector", BluRingViewer.div_PatientAge);
                String patientGender1 = viewer.GetText("cssselector", BluRingViewer.div_PatientGender);
                String pID1 = viewer.GetText("cssselector", BluRingViewer.div_PatientID);
                int p_Age1 = Int32.Parse(Regex.Replace(patientAge1, @"[^0-9]", ""));
                DateTime dt1 = new DateTime(Int32.Parse(date2[2]), Int32.Parse(date2[0]), Int32.Parse(date2[1]));
                String FormattedDate1 = String.Format("{0:dd-MMM-yyyy}", dt1);
                if (p_Age1 == BluRingViewer.CalculateAge(dob[1]) && patientGender1.Equals(gender[1]) && pID1.Equals(patientID[1]) && patientDOB1.Contains(FormattedDate1))
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

                //step6   click on the Show/Hide Button and verify sub-menu is displayed.
                viewer.OpenShowHideDropdown();
                String[] options = { "Hide Image Text", "Hide Dicom 6000 Overlay", "Hide Thumbnails", "Hide Stack Slider" };
                bool step6 = viewer.Verify_ShowHideDropdown_Values(options);
                if (step6)
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

                //step7 & 8  Verify the dividers are present in the Global Toolbar and divider is displayed between Help and Close Icon.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_globalTitleBar));
                if (step7)
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

                viewer.CloseBluRingViewer();

                studies = (Studies)login.Navigate("Studies");
                studies.ClearFields();
                studies.SearchStudy(LastName: lastName[2], AccessionNo: accession[2]);
                studies.SelectStudy("Accession", accession[2]);
                viewer = BluRingViewer.LaunchBluRingViewer();                

                //step9  Verify that Thumbnail Left and Right arrows can be scrolled				
                IList<IWebElement> indicator = viewer.StudyPanelThumbnailIndicator(0);
                int FirstThumbnailImageNum1 = viewer.GetImageNumber(indicator.Where<IWebElement>(element1 => element1.Displayed).ToList<IWebElement>()[0]);

                viewer.HoverElement(By.CssSelector(BluRingViewer.div_thumbnails));
                IWebElement element = viewer.GetElement("cssselector", BluRingViewer.div_ThumbnailNextArrowButton);
                viewer.ClickElement(element);

                indicator = viewer.StudyPanelThumbnailIndicator(0);
                int FirstThumbnailImageNum2 = viewer.GetImageNumber(indicator.Where<IWebElement>(element1 => element1.Displayed).ToList<IWebElement>()[0]);

                element = viewer.GetElement("cssselector", BluRingViewer.div_ThumbnailPreviousArrowButton);
                viewer.ClickElement(element);

                indicator = viewer.StudyPanelThumbnailIndicator(0);
                int FirstThumbnailImageNum3 = viewer.GetImageNumber(indicator.Where<IWebElement>(element1 => element1.Displayed).ToList<IWebElement>()[0]);

                if (!(FirstThumbnailImageNum1 == FirstThumbnailImageNum2) && !(FirstThumbnailImageNum2 == FirstThumbnailImageNum3))
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

                //step10  Click on the User-Setting Icon and verify the sub-menu is displayed accordingly.
                viewer.ClickOnUSerSettings();
                IList<IWebElement> usersettingList = BasePage.Driver.FindElements(By.CssSelector("div[class*='globalSettingPanel'] ul li"));
                string[] subMenu = { "UI-Scale", "Large", "Medium", "Small", "Skin", "Dark", "Gray", "Settings" };
                bool step10 = true;
                int count = usersettingList.Count - 1;
                while (count >= 0)
                {
                    if(!usersettingList[count].Text.Replace(" ", "").Replace("✔", "").Replace("\r", "").Replace("\n", "").ToLower().Equals(subMenu[count].ToLower()))                  
                    {
                        step10 = false;
                    }
                    count--;
                }

                if (step10)
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

            finally
            {
                // Revert the US modality layout to auto
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpref.ModalityDropDown().SelectByText("US");
                userpref.ThumbnailSplittingSeriesRadioBtn().Click();
                userpref.ViewingScopeSeriesRadioBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                login.Logout();
            }
        }

        /// <summary>
        /// Network Connection Tool on the Global Toolbar 
        /// </summary>
        public TestCaseResult Test_161117(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables               
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            String adminUserName = Config.adminUserName;
            String adminPassword = Config.adminPassword;
            UserPreferences userPrefer = new UserPreferences();
            BluRingViewer viewer = new BluRingViewer();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');

                ServiceTool servicetool = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();
                

                //Preconditions
                servicetool.LaunchServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableConnectionTestTool();
                wpfobject.WaitTillLoad();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();

                //step1 Launch the iCA application with a client browser.
                login.DriverGoTo(login.url);
                if (login.UserIdTxtBox().Displayed && login.PasswordTxtBox().Displayed &&
                    login.LoginBtn().Displayed)
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

                //step2 Verify that connection rating icon is displayed at the bottom left corner of the login page.
                IWebElement RatingIcon = BasePage.Driver.FindElement(login.ConnectionTool());
                bool step2_1 = RatingIcon.GetCssValue("position").Equals("fixed") &&
                               RatingIcon.GetCssValue("bottom").Equals("10px") &&
                               RatingIcon.GetCssValue("left").Equals("10px");
                string color = BasePage.Driver.FindElement(login.ConnectionRatingIcon()).GetAttribute("src");
                if (step2_1 && color.Contains("FullConnection") || color.Contains("FairConnection")
                    || color.Contains("MediumConnection") || color.Contains("HighConnection")
                    || color.Contains("No connection"))
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

                //step3 Mouse hover over on connection Rating icon and ensure "Network Connection" progress bar is displayed latency and bandwidth 
                viewer.HoverElement(login.ConnectionRatingIcon());
                string title = BasePage.Driver.FindElement(login.ConnectionRatingIcon()).GetAttribute("title");
                string Latency = BasePage.Driver.FindElement(login.CurrentConnectionTime()).GetAttribute("innerHTML");
                string[] value = Latency.Split(':');
                string Bandwith = BasePage.Driver.FindElement(login.Bandwidth()).GetAttribute("innerHTML");
                string[] Values = Bandwith.Split(':');
                if (title.Equals("Connection Rating : Latency :" + value[1].Trim() + " Bandwidth :" + Values[1].Trim()))
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

                //step4 Log in to ICA with valid credentials.
                login.LoginIConnect(adminUserName, adminPassword);               
                ExecutedSteps++;

                //step5  Mouse hover over on options and select User Preference	
                userPrefer.OpenUserPreferences();
                ExecutedSteps++;

                // Step6 && 7 - Ensure "Enable Connection Test Tool" is displayed with checkbox at the bottom page of the user preference dialog window
                userPrefer.SwitchToUserPrefFrame();
                viewer.ScrollIntoView(userPrefer.EnableConnectionTestTool());
                if (!userPrefer.EnableConnectionTestTool().Selected)
                {
                    login.ClickElement(userPrefer.EnableConnectionTestTool());                    
                }
                userPrefer.CloseUserPreferences();
                ExecutedSteps += 2;

                //step8  - Navigate to Studies tab and then Search and select study and click " View Exam" button
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0]);
                studies.SelectStudy("Accession", Accession[0]);
                BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //step9 - Ensure that "Network Connection" button is appeared at the top right of the global toolbar.	
                bool step9_1 = viewer.GetElement("cssselector", BluRingViewer.div_Usercontrolpanel).GetCssValue("float").Equals("right");
                int step9_2 = Convert.ToInt32(viewer.GetElement("cssselector", BluRingViewer.div_NetworkConnectionIcon).Location.Y.ToString());
                int step9_3 = Convert.ToInt32(viewer.GetElement("cssselector", BluRingViewer.div_StudyViewerTitleBar).Location.Y.ToString());
                if (step9_1 && step9_2 < step9_3)
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

                //step10 - Mouse hover over on "Network Connection" button and verify the tool tip for "Network Connection" Tool should be displayed as "Network Connection"
                var netconnicon = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_NetworkConnectionIcon);
                viewer.HoverElement(By.CssSelector(BluRingViewer.div_NetworkConnectionIcon));
                if (netconnicon.GetAttribute("title").Equals("Network Connection"))
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

                //Step 11 - Click on "Network Connection" button and verify that the "Network connect test window" should pop up	
                var js = (IJavaScriptExecutor)BasePage.Driver;
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer"))
                {
                    js.ExecuteScript("arguments[0].click()", netconnicon);
                }
                else
                {
                    netconnicon.Click();
                }
                if (viewer.IsElementVisible(By.CssSelector(BluRingViewer.div_NetworkConnectionDetails)))
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

                // Step 12 - Verify that the "Network connect test window" should be displayed the network connection information
                var step12 = viewer.GetElement("cssselector", BluRingViewer.div_NetworkConnectionDialogTitle);
                bool step12_1 = viewer.GetText("cssselector", BluRingViewer.div_NetworkConnectionDialogTitle).Equals("NETWORK CONNECTION");
                string Text = viewer.GetText("cssselector", BluRingViewer.div_NetworkDropdownDialogConnection);
                string[] Value = Text.Split(':');
                string[] Value_1 = Value[1].Split('|');
                bool step12_2 = Text.Equals("Your Current Round Trip: " + Value_1[0].Trim() + " | Your Current Bandwidth: " + Value[2].Trim());
                bool step12_3 = viewer.GetText("cssselector", BluRingViewer.div_NetworkDropdownDialogDetails)
                               .Replace(System.Environment.NewLine, string.Empty).ToLower()
                               .Equals("full connection: 1 - 50 ms | 50+ mbpsfair connection: 50 - 100 ms | 10 - 50 mbpslow connection: 100+ ms | 0.1 - 10 mbps");
                bool step12_4 = viewer.GetText("cssselector", BluRingViewer.div_NetworkDropdownDialogFooter).ToLower().Equals("contact your administrator for a poor connection");
                bool step12_5 = false;
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("firefox"))
                {
                    step12_5 = step12.GetCssValue("color").Equals("rgb(255, 255, 255)");
                }
                else
                {
                    step12_5 = step12.GetCssValue("color").Equals("rgba(255, 255, 255, 1)");
                }
                bool step12_6 = false;
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("firefox"))
                {
                    step12_6 = viewer.GetElement("cssselector", BluRingViewer.div_NetworkDropdownDialogDetails)
                              .GetCssValue("color").Equals("rgb(255, 255, 255)");
                }
                else
                {
                    step12_6 = viewer.GetElement("cssselector", BluRingViewer.div_NetworkDropdownDialogDetails)
                              .GetCssValue("color").Equals("rgba(255, 255, 255, 1)");
                }
                if ((step12.GetCssValue("font-weight").Equals("700") || step12.GetCssValue("font-weight").Equals("bold")) && step12_1 && step12_2 && step12_3 && step12_4 && step12_5 && step12_6)
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

                //step13 - Verify that the connection details should be matched the Connection test button color.
                bool step13 = false;
                var networkConnectionColor = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_NetworkConnectionIcon + " div div.toolIconNLMedium").GetAttribute("class");
                Text = viewer.GetText("cssselector", BluRingViewer.div_NetworkDropdownDialogConnection);
                Value = Text.Split(' ');
                var ms = Convert.ToInt32(Value[4]);
                var bandwith = Convert.ToInt32(Value[10]);
                Logger.Instance.InfoLog("The Round Trip is " + ms + " and the bandwith is " + bandwith);
                if (networkConnectionColor.Contains("high"))
                {
                    Logger.Instance.InfoLog("The Network connection is high");
                    //if (ms <= 10 && bandwith >= 10)
                    if (ms <= 50)
                    {
                        step13 = true;
                    }
                }
                else if (networkConnectionColor.Contains("med"))
                {
                    Logger.Instance.InfoLog("The Network connection is Med");
                    //if (ms >= 10 && ms <= 100 && bandwith <= 10)
                    if (ms >= 50 && ms <= 100)
                    {
                        step13 = true;
                    }
                }
                else if (networkConnectionColor.Contains("low"))
                {
                    Logger.Instance.InfoLog("The Network connection is Low");
                    //if (ms >= 100 && bandwith < 1)
                    if (ms >= 100)
                    {
                        step13 = true;
                    }
                }
                if (step13)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step14 - Close the Universal viewer (click "X" EXIT in the global toolbar).
                viewer.CloseBluRingViewer();
                ExecutedSteps++;

                //step15 - After a minute,Launch the same or any study in Universal viewer using 'Universal' button and click on ''Network Connection Tool' and Verify that the connection Time and Bandwidth values should be auto updated.               
                Thread.Sleep(60000);
                studies.SelectStudy("Accession", Accession[0]);
                BluRingViewer.LaunchBluRingViewer();               
                var networkIcon = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_NetworkConnectionIcon);
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer"))
                {
                    js.ExecuteScript("arguments[0].click()", networkIcon);
                }
                else
                {
                    networkIcon.Click();
                }
                var step15_1 = viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_NetworkConnectionDetails));
                var dialogText = viewer.GetText("cssselector", BluRingViewer.div_NetworkDropdownDialogConnection);
                bool step15_2 = dialogText.Contains("Your Current Round Trip:");
                bool step15_3 = dialogText.Contains("Your Current Bandwidth:");               
                dialogText = viewer.GetText("cssselector", BluRingViewer.div_NetworkDropdownDialogConnection);
                String[] values = dialogText.Split(':');
                Value_1 = values[1].Split('|');
                bool step15_4 = dialogText.Equals("Your Current Round Trip: " + Value_1[0].Trim() + " | Your Current Bandwidth: " + values[2].Trim());
                bool step15_5 = viewer.GetText("cssselector", BluRingViewer.div_NetworkDropdownDialogDetails)
                               .Replace(System.Environment.NewLine, string.Empty).ToLower()
                               .Equals("full connection: 1 - 50 ms | 50+ mbpsfair connection: 50 - 100 ms | 10 - 50 mbpslow connection: 100+ ms | 0.1 - 10 mbps");
                if (step15_1 && step15_2 && step15_3 && step15_4 && step15_5)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                    //step16 - Again click on "Network Connection" Button and verify the "Connection Network test window" should get closed.
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer"))                
                    js.ExecuteScript("arguments[0].click()", networkIcon);
                else
                    networkIcon.Click();                
                if (!viewer.IsElementVisible(By.CssSelector(BluRingViewer.div_NetworkConnectionDetails)))
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

                //step17 - Click "Network Connection" Button and launch "Network connection test window "and click on any other button on the viewer and verify the "Connection Network test window" should get close automatically.
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer"))               
                    js.ExecuteScript("arguments[0].click()", networkIcon);                
                else
                    networkIcon.Click();                
                bool step17_1 = viewer.IsElementVisible(By.CssSelector(BluRingViewer.div_NetworkConnectionDetails));
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer"))               
                    js.ExecuteScript("arguments[0].click()", viewer.GetElement("cssselector", BluRingViewer.div_ShowHideTool));               
                else
                    networkIcon.Click();                

                bool step17_2 = viewer.IsElementVisible(By.CssSelector(BluRingViewer.div_NetworkConnectionDetails));
                if (step17_1 && !step17_2)
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

                // Step 18 - From User Preferences > disable Connection Tool.	
                viewer.CloseBluRingViewer();
                userPrefer.OpenUserPreferences();
                userPrefer.SwitchToUserPrefFrame();
                viewer.ScrollIntoView(userPrefer.EnableConnectionTestTool());
                if (userPrefer.EnableConnectionTestTool().Selected)
                {
                    viewer.ClickElement(userPrefer.EnableConnectionTestTool());
                }
                userPrefer.CloseUserPreferences();
                ExecutedSteps++;

                //Logout Application                
                login.Logout();

                //Return Result.
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
                //Disabling Connection Test Tool In UserPreferences.
                login.LoginIConnect(adminUserName, adminPassword);
                userPrefer.OpenUserPreferences();
                userPrefer.SwitchToUserPrefFrame();
                viewer.ScrollIntoView(userPrefer.EnableConnectionTestTool());
                if (userPrefer.EnableConnectionTestTool().Selected)
                {
                    viewer.ClickElement(userPrefer.EnableConnectionTestTool());
                }
                userPrefer.CloseUserPreferences();
                login.Logout();
            }
        }

        /// <summary>  
        /// Network Connection tool shall not display on the Global Toolbar  
        /// </summary>  
        public TestCaseResult Test_161032(String testid, String teststeps, int stepcount)
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

                ServiceTool servicetool = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();
                BluRingViewer viewer = new BluRingViewer();

                //Preconditions  
                servicetool.LaunchServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.DisableConnectionTestTool();
                wpfobject.WaitTillLoad();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                servicetool.CloseServiceTool();

                //step1 Launch the iCA application with a client browser  
                login.DriverGoTo(login.url);
                if (login.UserIdTxtBox().Displayed && login.PasswordTxtBox().Displayed &&
                    login.LoginBtn().Displayed)
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

                //step2  Verify that connection rating icon is not displayed at the bottom left corner of the login page  
                if (!(viewer.IsElementVisible(login.ConnectionRatingIcon())))
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

                //step3 Log in to ICA with valid credentials.  
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //step4 Navigate to studies tab and load any study into the viewer by clicking "View Exam" button.  
                var study = (Studies)login.Navigate("Studies");
                study.SearchStudy(AccessionNo: Accession[0]);
                study.SelectStudy("Accession", Accession[0]);
                BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //step5 Ensure that the Network Connection button is not appeared on global toolbar at the top right of the viewer.  
                if (!(viewer.IsElementVisible(By.CssSelector(BluRingViewer.div_NetworkConnectionIcon))))
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
                //Return Result.  
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
         /// Checked "Enable Connection Test Tool" in User Preference 
         /// </summary>
         public TestCaseResult Test_161033(String testid, String teststeps, int stepcount)
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

                ServiceTool servicetool = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();
                BluRingViewer viewer = new BluRingViewer();

                //Preconditions
                servicetool.LaunchServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableConnectionTestTool();
                wpfobject.WaitTillLoad();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                servicetool.CloseServiceTool();

                //step1 Launch the iCA application with a client browser.
                login.DriverGoTo(login.url);
                if (login.UserIdTxtBox().Displayed && login.PasswordTxtBox().Displayed &&
                    login.LoginBtn().Displayed)
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

                //step2 Login to WebAccess site with any privileged user.
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //step3 Mouse hover over on options and select User Preference.
                UserPreferences userPrefer = new UserPreferences();
                userPrefer.OpenUserPreferences();
                ExecutedSteps++;

                //step4 Ensure "Enable Connection Test Tool" is displayed with checkbox at the bottom page of the user preference dialog window.
                userPrefer.SwitchToUserPrefFrame();
                bool step4_1 = userPrefer.EnableConnectionTestTool().Displayed;
                int StudiesButton = Convert.ToInt32(userPrefer.DefaultStartPageStudies().Location.Y.ToString());
                int ConnectionToolCheckBox = Convert.ToInt32(userPrefer.EnableConnectionTestTool().Location.Y.ToString());
                int OkButton = Convert.ToInt32(userPrefer.SavePreferenceBtn().Location.Y.ToString());
                if (step4_1 && (ConnectionToolCheckBox > StudiesButton) && (ConnectionToolCheckBox < OkButton))
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

                //step5 Select checkbox of "Enable Connection Test Tool" and click on save button.
                if (!userPrefer.EnableConnectionTestTool().Selected)
                {
                    userPrefer.EnableConnectionTestTool().Click();
                }
                userPrefer.CloseUserPreferences();
                ExecutedSteps++;

                //step6 Navigate to Studies tab then Search and select study and click " View Exam" button.
                var study = (Studies)login.Navigate("Studies");
                study.SearchStudy(AccessionNo: Accession[0]);
                study.SelectStudy("Accession", Accession[0]);
                BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //step7  Ensure that "Network Connection" button is appeared at the top right of the global toolbar.
                bool step7_1 = viewer.GetElement("cssselector", BluRingViewer.div_Usercontrolpanel).GetCssValue("float").Equals("right");
                int step7_2 = Convert.ToInt32(viewer.GetElement("cssselector", BluRingViewer.div_NetworkConnectionIcon).Location.Y.ToString());
                int step7_3 = Convert.ToInt32(viewer.GetElement("cssselector", BluRingViewer.div_StudyViewerTitleBar).Location.Y.ToString());
                if (step7_1 && step7_2 < step7_3)
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
        /// Uncheck Enable Connection Test Tool in User Preference 
        /// </summary>
        public TestCaseResult Test_161034(String testid, String teststeps, int stepcount)
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

                ServiceTool servicetool = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();
                BluRingViewer viewer = new BluRingViewer();

                //Preconditions
                servicetool.LaunchServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableConnectionTestTool();
                wpfobject.WaitTillLoad();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                servicetool.CloseServiceTool();

                //step1 Launch the iCA application with a client browser.
                login.DriverGoTo(login.url);
                if (login.UserIdTxtBox().Displayed && login.PasswordTxtBox().Displayed &&
                    login.LoginBtn().Displayed)
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

                //step2 Login to WebAccess site with any privileged user.
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //step3 Mouse hover over on options and select User Preference.
                UserPreferences userPrefer = new UserPreferences();
                userPrefer.OpenUserPreferences();
                ExecutedSteps++;

                //step4 Ensure "Enable Connection Test Tool" is displayed with checkbox at the bottom page of the user preference dialog window.
                userPrefer.SwitchToUserPrefFrame();
                bool step4_1 = userPrefer.EnableConnectionTestTool().Displayed;
                int StudiesButton = Convert.ToInt32(userPrefer.DefaultStartPageStudies().Location.Y.ToString());
                int ConnectionToolCheckBox = Convert.ToInt32(userPrefer.EnableConnectionTestTool().Location.Y.ToString());
                int OkButton = Convert.ToInt32(userPrefer.SavePreferenceBtn().Location.Y.ToString());
                if (step4_1 && (ConnectionToolCheckBox > StudiesButton) && (ConnectionToolCheckBox < OkButton))
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

                //step5 Do not select checkbox of "Enable Connection Test Tool" and click on close button.
                if (userPrefer.EnableConnectionTestTool().Selected)
                {
                    userPrefer.EnableConnectionTestTool().Click();
                }
                userPrefer.CloseUserPreferences();
                ExecutedSteps++;

                //step6 Navigate to Studies tab then Search and select study and click " View Exam" button.
                var study = (Studies)login.Navigate("Studies");
                study.SearchStudy(AccessionNo: Accession[0]);
                study.SelectStudy("Accession", Accession[0]);
                BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //step7  Verify that "Network Connection" button is not appeared at the top right corner of the global toolbar.
                if (!viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_NetworkConnectionIcon)))
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
    }
}
