using System;
using System.Globalization;
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
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.Putty;
using System.Data;
using Selenium.Scripts.Pages.MergeServiceTool;

namespace Selenium.Scripts.Tests
{
    class MeaningfulUse : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public ExamImporter ei { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public HPHomePage hphomepage { get; set; }
        public HPLogin hplogin { get; set; }
        public ServiceTool servicetool { get; set; }

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public MeaningfulUse(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            ei = new ExamImporter();
            mpaclogin = new MpacLogin();
        }

        public TestCaseResult Test_66136(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            DomainManagement domainmanagement = null;
            RoleManagement rolemanagement = null;
            MeaningfulUseReport meaningfulusereport = null;
            string DomainName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
            string RoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
            string Institution = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Institution");
            string FromDate = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FromDate");
            string ToDate = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ToDate");
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;
                meaningfulusereport = new MeaningfulUseReport();
                //Step 1: Start the application.
                ExecutedSteps++;
                //Step 2: Login as administrator and go to the Domain management page
                //Step 3: Select and enable the Meaningful Use function Save
                login.LoginIConnect(Username, Password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                ExecutedSteps++;
                if (meaningfulusereport.CheckMeaningfulUseInDomain(DomainName))
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
                //Step 4: Select the Role Management page
                //Step 5: Select and enable the Meaningful Use function Save
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                ExecutedSteps++;
                if (meaningfulusereport.CheckMeaningfulUseInRole(DomainName, RoleName))
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
                //Step 6: Move the mouse cursor up to the Options label
                if (IsClickElementsExists(new string[] { "User Preferences", "My Profile", "Meaningful Use Report", "Meaningful Use Report Status" }))
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
                //Step 7: Select the"Meaningful Use Report button
                meaningfulusereport.OpenMeaningfulUseReport();
                int resultcount = 0;
                if (meaningfulusereport.EligibleHospital().Selected)
                {
                    resultcount++;
                    Logger.Instance.InfoLog("EligibleHospital is Selected By Default");
                }
                if (meaningfulusereport.EligiblePhysician().Enabled)
                {
                    resultcount++;
                    Logger.Instance.InfoLog("EligiblePhysician is Enabled");
                }
                if (meaningfulusereport.Institution().Enabled)
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Institution is Enabled");
                }
                if (meaningfulusereport.AllNPI().Selected && !meaningfulusereport.AllNPI().Enabled)
                {
                    resultcount++;
                    Logger.Instance.InfoLog("AllNPI is Selected and Disabled by Default");
                }
                if (!meaningfulusereport.SpecificNPI().Displayed)
                {
                    resultcount++;
                    Logger.Instance.InfoLog("SpecificNPI is not visible by default");
                }
                if (meaningfulusereport.MeaningfulFromDate().Enabled)
                {
                    resultcount++;
                    Logger.Instance.InfoLog("MeaningfulFromDate is Enabled");
                }
                if (meaningfulusereport.MeaningfulToDate().Enabled)
                {
                    resultcount++;
                    Logger.Instance.InfoLog("MeaningfulToDate is Enabled");
                }
                if (meaningfulusereport.GenerateReport().Enabled)
                {
                    resultcount++;
                    Logger.Instance.InfoLog("GenerateReport is Enabled");
                }
                if (meaningfulusereport.Cancel().Enabled)
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Cancel is Enabled");
                }
                if (resultcount == 9)
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
                //Step 8: Select the Eligible Hospitals Button
                meaningfulusereport.EligibleHospital().Click();
                //meaningfulusereport.EligiblePhysician().Click();
                resultcount = 0;
                if (meaningfulusereport.Institution().Enabled)
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Institution is Enabled");
                }
                if (meaningfulusereport.SpecificNPI().Displayed)
                {
                    resultcount++;
                    Logger.Instance.InfoLog("SpecificNPI is visible");
                }
                if (resultcount == 2)
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
                //Step 9: Select one Institution (SITE1) and add a date range. From: 01/01/1960 To: 01/01/2014 then click on "Generate Report
                FromDate = DateTime.Parse(FromDate).ToString("dd-MMM-yyyy");
                ToDate = DateTime.Parse(ToDate).ToString("dd-MMM-yyyy");
                meaningfulusereport.GenerateEligibleHospitalReport(Institution, FromDate, ToDate);
                if (string.Equals(meaningfulusereport.MeaningfulUseReportStatusHeading().Text, "Meaningful Use Report Status"))
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
                //Step 10: Select a report and Click on Download
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 11: Compare the Report with the Sample on the Sample Page Report 1
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Logout as User
                login.Logout();
                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Logout
                login.Logout();
                //Return Result
                return result;
            }

        }

        public TestCaseResult Test_66137(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            MeaningfulUseReport meaningfulusereport = null;
            string DomainName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
            string RoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
            string Institution = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Institution");
            string SpecificNPI = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "SpecificNPI");
            string FromDate = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FromDate");
            string ToDate = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ToDate");
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                meaningfulusereport = new MeaningfulUseReport();
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;
                login.LoginIConnect(Username, Password);
                //Precondition
                if (IsClickElementsExists(new string[] { "User Preferences", "My Profile", "Meaningful Use Report", "Meaningful Use Report Status" }))
                {
                    login.Logout();
                }
                else
                {
                    meaningfulusereport.CheckMeaningfulUseInDomain(DomainName);
                    meaningfulusereport.CheckMeaningfulUseInRole(DomainName, RoleName);
                    login.Logout();
                }
                //Step 1: Login and select the Meaningful Use button. In the window that opens select the Eligible Physician button
                login.LoginIConnect(Username, Password);
                meaningfulusereport.OpenMeaningfulUseReport();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (meaningfulusereport.MeaningfulInstitution().Options.Select(opt => opt.Text).ToArray().Contains("All"))
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
                //Step 2: Select one Institution
                //Step 3: Enter an NPI (National Provider Identifier number ) in the input field , enter a date range and click on"Generate Report". NPI numbers are found in the order and references the Physician by code.
                FromDate = DateTime.Parse(FromDate).ToString("dd-MMM-yyyy");
                ToDate = DateTime.Parse(ToDate).ToString("dd-MMM-yyyy");
                ExecutedSteps++;
                meaningfulusereport.GenerateEligiblePhysicianReport(Institution, SpecificNPI, FromDate, ToDate);
                if (string.Equals(meaningfulusereport.MeaningfulUseReportStatusHeading().Text, "Meaningful Use Report Status"))
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
                //Step 4: Click on Download
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 5: Compare the Report with the Sample on the Sample Page Report 2
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Logout as User
                login.Logout();
                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Logout
                login.Logout();
                //Return Result
                return result;
            }
        }

        public TestCaseResult Test_66138(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            MeaningfulUseReport meaningfulusereport = null;
            string DomainName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
            string RoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
            string Institution = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Institution");
            string SpecificNPI = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "SpecificNPI");
            string FromDate = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FromDate");
            string ToDate = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ToDate");
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                meaningfulusereport = new MeaningfulUseReport();
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;
                //Precondition
                login.LoginIConnect(Username, Password);
                if (IsClickElementsExists(new string[] { "User Preferences", "My Profile", "Meaningful Use Report", "Meaningful Use Report Status" }))
                {
                    login.Logout();
                }
                else
                {
                    meaningfulusereport.CheckMeaningfulUseInDomain(DomainName);
                    meaningfulusereport.CheckMeaningfulUseInRole(DomainName, RoleName);
                    login.Logout();
                }
                //Step 1: Meaningful Use Report&#-3; job is submitted earlier.
                login.LoginIConnect(Username, Password);
                meaningfulusereport.OpenMeaningfulUseReport();
                FromDate = DateTime.Parse(FromDate).ToString("dd-MMM-yyyy");
                ToDate = DateTime.Parse(ToDate).ToString("dd-MMM-yyyy");
                meaningfulusereport.GenerateEligiblePhysicianReport(Institution, SpecificNPI, FromDate, ToDate);
                login.Logout();
                ExecutedSteps++;
                //Step 2: Login and select the Meaningful Use Report Status button.
                login.LoginIConnect(Username, Password);
                meaningfulusereport.OpenMeaningfulUseReportStatus();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                int resultcount = 0;
                if (string.Equals(meaningfulusereport.MeaningfulUseReportStatusHeading().Text, "Meaningful Use Report Status"))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Meaningful Use Report Status Heading is Displayed");
                }
                if (meaningfulusereport.RefreshMeaningfulStatus().Displayed)
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Refresh Button is Displayed");
                }
                /*if (meaningfulusereport.DownloadMeaningfulStatus().Displayed)
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Download Button is Displayed");
                }*/
                string[] ExpectedValue = new string[] { "Program", "Institution", "NPI", "Status", "Updated" };
                string[] ActualValue = meaningfulusereport.MeaningfulStatusTableHeading().Select(value => value.Text).ToArray();
                if (ExpectedValue.All(expectedvalue => ActualValue.Contains(expectedvalue)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("All Table Headings are Displayed");
                }
                if (resultcount == 4)
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
                //Step 3:  	On one of the Reports with Status Succeeded the Download button is enabled , click on one of the reports and click on download button
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 4: Compare the Report with the Sample on the Sample Page Report =X
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Logout as User
                login.Logout();
                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Logout
                login.Logout();
                //Return Result
                return result;
            }
        }

        public TestCaseResult Test_66139(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            string AccessionNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNo");
            string ExePATH = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ExePATH");
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                //Step 1: Send a report or order to Mirth that does not have a Study in any of the databases configured.
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 2: Observer the Study Found column = False
                DataBaseUtil db = new DataBaseUtil("sqlserver", "MU2", InstanceName:"WEBACCESS");
                db.ConnectSQLServerDB();
                string sql = string.Concat("select StudyFound, StudyFoundUpdateDate from [dbo].[Order] where AccessionNo='", AccessionNo, "'");
                DataTable order = db.ReadTable(sql);
                string[] StudyFound = GetColumnValues(order, "StudyFound");
                if(StudyFound.All(sf=>sf.ToLowerInvariant().Contains("false")))
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
                //Step 3: Send the Study associated with the Order or report to the Datasource registered
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 4: GO to the ICA system folder C:\Program Files (x86)\Cedara\WebAccess\MeaningfulUseTool\bin and run the program MeaningfulUseTool.exe
                DateTime currentdatetime = DateTime.Now;
                wpfobject.InvokeApplication(ExePATH);
                ExecutedSteps++;
                //Step 5:  	Go back to the SQL database MU2 and observer the Study Found column = true and the next column should have the date the tool was run .
                order = db.ReadTable(sql);
                StudyFound = GetColumnValues(order, "StudyFound");
                string[] StudyFoundUpdateDate = GetColumnValues(order, "StudyFoundUpdateDate");
                int resultcount = 0;
                if (StudyFound.All(sf => sf.ToLowerInvariant().Contains("true")))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("StudyFound is set to True");
                }
                if(StudyFoundUpdateDate.All(sud=> DateTime.Parse(sud) > currentdatetime))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("StudyFoundUpdateDate is set to Latest Date");
                }
                if(resultcount==2)
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
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Return Result
                return result;
            }
        }
    }
}
