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
using Selenium.Scripts.Pages.iConnect;
using System.Windows.Forms;
using OpenQA.Selenium.Remote;
using System.Diagnostics;
using Selenium.Scripts.Pages.MergeServiceTool;
using System.Data;

namespace Selenium.Scripts.Tests
{
    class Configuration : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public ExamImporter ei { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public MPHomePage mphomepage { get; set; }
        public Tool mpactool { get; set; }
        public static string BrowserVersion { get; set; }
        public static string SBrowserName { get; set; }
        public WpfObjects wpfObject { get; set; }
        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        public Configuration(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            ei = new ExamImporter();
            mpaclogin = new MpacLogin();
            wpfObject = new WpfObjects();

            BrowserVersion = ((RemoteWebDriver)BasePage.Driver).Capabilities.Version;
            SBrowserName = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName;
        }

        /// <summary>
        /// Emergency Access
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161362(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables          
            Inbounds inbounds = null;
            Maintenance maintenance;
            Studies studies;
            DomainManagement domainmanagement;
            UserManagement usermanagement;
            RoleManagement rolemanagement;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String adminusername = "PatientAdmin" + new Random().Next(1000);
                String adminpassword = "PatientAdminPass" + new Random().Next(1000);
                String User1 = "U1" + new Random().Next(1000);
                String domainname = "EmergencyDomain" + new Random().Next(1, 10000);
                String rolename = "EmergencyRole" + new Random().Next(1, 10000);
                String datasource = login.GetHostName(Config.SanityPACS);
                String username = "U2" + new Random().Next(1, 10000);
                String password = "U2" + new Random().Next(1, 10000);
                String role = "PatientRole" + new Random().Next(1000);
                String user = "U3" + new Random().Next(1, 10000);
                String pwd = "U3" + new Random().Next(1, 10000);
                String domain = "PatientDomain" + new Random().Next(1000);
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String PatientName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                String Genders = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Gender");
                String[] gender = Genders.Split(':');
                String DOB = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DOB");
                String StudyDate = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDate");
                String studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String[] dob = DOB.Split(':');
                String[] date1 = dob[0].Split('/');
                DateTime dt_1 = new DateTime(Int32.Parse(date1[2]), Int32.Parse(date1[0]), Int32.Parse(date1[1]));
                String FormattedDate = String.Format("{0:dd-MMM-yyyy}", dt_1);

                //Precondition:
                //From Service Tool, enable Study Sharing - Grant Access 
                //Ensure that there are 2 users in one group to Ensure at least one user who has privilege of Grant Access as well as Emergency Access 
                //Enable Data Transfer function from Service ToolEnable universal viewer from domain and role levels.


                //Step-1:Initial Setup
                /**Precondition-->Enable Emergency Access in Service Tool and create a domain and user**/
                BasePage.RunBatchFile(Config.batchfilepath, studypath + " " + Config.dicomsendpath + " " + Config.SanityPACS);

                //Login as Administrator
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                //Create Domain(Patient History)
                domainmanagement = login.Navigate<DomainManagement>();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                bool DomainFlag = domainmanagement.DomainExists(domain);

                if (!DomainFlag)
                {
                    domainmanagement.ClickNewDomainBtn();

                    login.ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Name");
                    login.SetText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Name", domain);

                    login.ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Description");
                    login.SetText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Description", domain);

                    login.SetText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_ReceivingInstitution", domain + "Inst");

                    domainmanagement.ConnectDataSourcesConsolidatedInNewDomain(datasource);
                    domainmanagement.SetViewerTypeInNewDomain();
                    PageLoadWait.WaitForPageLoad(30);

                    if (BasePage.Driver.FindElement(By.CssSelector("[id$='DomainInfo_DataTransferEnabledCB']")).Enabled &&
                        BasePage.Driver.FindElement(By.CssSelector("[id$='DomainInfo_GrantAccessEnabledCB']")).Enabled &&
                        BasePage.Driver.FindElement(By.CssSelector("[id$='DomainInfo_EmergencyAccessEnabledCB']")).Enabled &&
                        BasePage.Driver.FindElement(By.CssSelector("[id$='RoleAccessFilter_AllowEmergencyAccessCB']")).Enabled)
                    {
                        login.SetCheckbox("cssselector", "[id$='DomainInfo_DataTransferEnabledCB']");
                        login.SetCheckbox("cssselector", "[id$='DomainInfo_GrantAccessEnabledCB']");
                        login.SetCheckbox("cssselector", "[id$='DomainInfo_EmergencyAccessEnabledCB']");
                        login.SetCheckbox("cssselector", "[id$='RoleAccessFilter_AllowEmergencyAccessCB']");
                    }
                    BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_RoleAccessFilter_GrantAccessRadioButtonList_2")).Click();
                    login.SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_UserID", adminusername);

                    login.SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_LastName", domain.Replace(" ", "_") + "LastName");

                    login.SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_FirstName", domain.Replace(" ", "_") + "FirstName");

                    login.SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_Password", adminpassword);

                    login.SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_ComparisonPassword", adminpassword);

                    login.SetText("cssselector", "#ctl00_MasterContentPlaceHolder_RoleAccessFilter_Name", role);

                    login.SetText("cssselector", "#ctl00_MasterContentPlaceHolder_RoleAccessFilter_Description", role);
                    PageLoadWait.WaitForPageLoad(20);

                    //Save Changes
                    domainmanagement.ClickSaveNewDomain();
                }

                //Navigate to UserManagement tab
                usermanagement = (UserManagement)login.Navigate("UserManagement");

                //Create a new user in the same domain
                usermanagement.CreateUser(User1, domain, role);

                //Logout as Administrator
                login.Logout();

                //Login as admin of testdomain(Patient History)                
                login.LoginIConnect(adminusername, adminpassword);
                ExecutedSteps++;

                //Step-2:Navigate to Studies
                studies = (Studies)login.Navigate("Studies");

                //Emergency Access button must be validated-->in Studies tab of Admin
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");

                IWebElement Emergencyaccess = null;
                if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
                {
                    Emergencyaccess = BasePage.Driver.FindElements(By.CssSelector("#m_studySearchControl_SearchTypeDiv>table>tbody>tr>td"))[3].FindElement(By.CssSelector("span"));
                }
                else
                {
                    Emergencyaccess = BasePage.Driver.FindElement(By.CssSelector("#m_studySearchControl_SearchTypeDiv>table>tbody>tr>td:nth-child(4)>span"));    
                }

                if (Emergencyaccess.Displayed == true)
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

                //Step-3:Click on Emergency Access nd validate the warning
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                IWebElement Emergencybtn = studies.Emergencybtn();
                Emergencybtn.Click();

                //Validate that the warning gets dismiss
                IWebElement Warningdialogbox, Warningmessage;

                studies.EmergencyWarning(out Warningdialogbox, out Warningmessage);

                if ((Warningmessage.Displayed) == true && (Warningdialogbox.Displayed) == true)
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

                //Step-4:Cancel the dialog
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                IWebElement Cancelbtn = BasePage.Driver.FindElement(By.CssSelector("#m_studySearchControl_EmergencyCancelButton"));
                Cancelbtn.Click();

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                IWebElement element = BasePage.Driver.FindElement(By.CssSelector("#TabText0"));
                if (element.Displayed == true)
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

                //Navigate to Maintenance
                maintenance = (Maintenance)login.Navigate("Maintenance");

                //Navigate to Audit tab
                maintenance.Navigate("Audit");

                //Check all the checkboxes
                maintenance.SetCheckBoxInAudit();

                //Select Security Alert in Event ID
                maintenance.SelectEventID("Security Alert",0);

                //Validate whether it is logged
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");

                IWebElement log = null;

                if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
                {
                    log = BasePage.Driver.FindElements(By.CssSelector("table#m_listControl_m_dataListGrid tr"))[1].FindElement(By.CssSelector("td>span"));
                }
                else
                {
                    log = BasePage.Driver.FindElement(By.CssSelector("table#m_listControl_m_dataListGrid tr:nth-child(2)>td>span"));
                }
                

                if (log.Displayed == true)
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

                //Step-5:Navigate to Studies
                studies = (Studies)login.Navigate("Studies");

                //Enable the Emergency Access and accept
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                IWebElement Emergencybtn0 = studies.Emergencybtn();
                Emergencybtn0.Click();
                studies.EmergencyWarning(out Warningdialogbox, out Warningmessage);
                IWebElement Acceptbtn = BasePage.Driver.FindElement(By.CssSelector("#m_studySearchControl_EmergencyAcceptButton"));
                Acceptbtn.Click();
                PageLoadWait.WaitForFrameLoad(5);

                //Validate that the warning gets dismiss
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                IWebElement element0 = BasePage.Driver.FindElement(By.CssSelector("#TabText0"));
                if (element0.Displayed == true)
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


                //Step-6:Navigate to Maintenance
                maintenance = (Maintenance)login.Navigate("Maintenance");

                //Navigate to Audit tab
                maintenance.Navigate("Audit");

                //Validate whether additional audit log is traced
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                
                IWebElement addlog = null;

                if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
                {
                    addlog = BasePage.Driver.FindElements(By.CssSelector("table#m_listControl_m_dataListGrid tr"))[2].FindElement(By.CssSelector("td>span"));
                }
                else
                {
                    addlog = BasePage.Driver.FindElement(By.CssSelector("table#m_listControl_m_dataListGrid tr:nth-child(3)>td>span"));
                }

                if (addlog.Displayed == true)
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


                //Step-7:Navigate to Studies
                studies = (Studies)login.Navigate("Studies");

                //Validate Emergency search is unchecked
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                IWebElement Emergencybtn1 = studies.Emergencybtn();

                if (Emergencybtn1.Selected == false)
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

                //Logout as Admin of testdomain(Patient History)
                login.Logout();

                //Step-8:Create new domain and user without Emergency Access and admin user
                //Login as Administrator
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                //Navigate to DomainManagement
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                //Create new domain
                domainmanagement.CreateDomain(domainname, rolename, datasource);
                domainmanagement.ClickSaveNewDomain();
                //Navigate to UserManagement tab
                usermanagement = (UserManagement)login.Navigate("UserManagement");

                //Create a new user in the same domain
                usermanagement.CreateUser(username, domainname, rolename);


                //Logout as Administrator
                login.Logout();

                //Login as New User
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //Step-9:Navigate to Studies Tab
                studies = (Studies)login.Navigate("Studies");

                //Validate that Emergency Access option is not there
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                if (BasePage.Driver.FindElement(By.CssSelector("div#SearchPanelDiv")).Displayed == true)
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

                //Logout as New User
                login.Logout();


                //Step-10:Login as New Admin
                login.LoginIConnect(domainname, domainname);

                //Navigate to DomainManagement
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");

                //Enable Emergency Access                
                domainmanagement.SetCheckBoxInEditDomain("emergency", 0);
                domainmanagement.ClickSaveEditDomain();
                domainmanagement.ClickCloseEditDomain();
                ExecutedSteps++;

                //Step-11:Navigate to Studies Tab
                studies = (Studies)login.Navigate("Studies");

                //Validate that Emergency Access option is not there
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                if (BasePage.Driver.FindElement(By.CssSelector("div#SearchPanelDiv")).Displayed == true)
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

                //Step-12:Navigate to RoleManagement Tab
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");

                //Enable Emergency Access and save
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#NewRoleButton")));
                rolemanagement.SelectRole(rolename);
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("emergency", 0);

                rolemanagement.ClickSaveEditRole();
                ExecutedSteps++;


                //Step-13:Navigate to Studies Tab
                studies = (Studies)login.Navigate("Studies");

                //Validate that Emergency Access option is there
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                if (BasePage.Driver.FindElement(By.CssSelector("div#m_studySearchControl_SearchTypeDiv")).Displayed == true)
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

                //Step-14:Edit role as CR Modality
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");

                rolemanagement.SelectRole(rolename);
                rolemanagement.ClickEditRole();

                //Select CR Modality from Access filter
                rolemanagement.RoleFilter_Modality("CR");


                //Check only one datasource is connected
                String dslist = "";

                if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
                {
                    dslist = BasePage.Driver.FindElements(By.CssSelector("div[id$='_RoleDataSourceListControl'] div.dataSourceListHeader  div"))[1].Text;
                }
                else
                {
                    dslist = BasePage.Driver.FindElement(By.CssSelector("div[id$='_RoleDataSourceListControl'] div.dataSourceListHeader  div:nth-child(2)")).Text;
                }

                if ((dslist.Equals(datasource)) == true)
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
                rolemanagement.ClickSaveEditRole();


                //Step-15:Logout as  New Admin
                login.Logout();

                //Login as New User
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //Step-16:Navigate to Studies Tab
                studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //Step-17:Click on Emergency Search
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                IWebElement Emergencybtn2 = studies.Emergencybtn();
                Emergencybtn2.Click();
                studies.EmergencyWarning(out Warningdialogbox, out Warningmessage);
                if ((Warningmessage.Displayed) == true && (Warningdialogbox.Displayed) == true)
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


                //Step-18:Accept the warning

                IWebElement Acceptbtn0 = BasePage.Driver.FindElement(By.CssSelector("#m_studySearchControl_EmergencyAcceptButton"));
                Acceptbtn0.Click();
                PageLoadWait.WaitForFrameLoad(5);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                IWebElement yellowwarning = BasePage.Driver.FindElement(By.CssSelector("div#SearchWarningDiv>span"));
                if (yellowwarning.Displayed == true)
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

                //Step-19:Perform an emergency search

                studies.EmergencySearchStudy(PatientName.Split(',')[0], PatientName.Split(',')[1].Trim(), gender[0], DOB);
                //Dictionary<string, string> row = studies.GetMatchingRow(new string[] { "Accession", "Patient ID", "Modality" }, new string[] { Accession[0], PatientID, "CR" });
                Dictionary<string, string> row = studies.GetMatchingRow(new string[] { "Accession", "Patient ID" }, new string[] { Accession[0], PatientID });
                if (row != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study not found with given fields");
                }

                //Step-20:Launch the study
                studies.SelectStudy1("Accession", Accession[0]);
                BluRingViewer viewer = new BluRingViewer();
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    BluRingViewer.LaunchBluRingViewer();
                    String patientinfo = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.p_PatientName).GetAttribute("innerHTML");
                    String patientid = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_PatientID).GetAttribute("innerHTML");
                    String patientDOB = viewer.GetText("cssselector", BluRingViewer.span_PatientDOB);
                    if (patientinfo.Equals(PatientName.Trim()) && patientid.Equals(PatientID) && patientDOB.Contains(FormattedDate))
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
                }
                else
                {
                    studies.LaunchStudy();
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    String patientinfo = BasePage.Driver.FindElement(By.CssSelector("span.patientInfoDiv")).GetAttribute("innerHTML");
                    String details = patientinfo.ToLower();
                    String studyinfo = BasePage.Driver.FindElement(By.CssSelector("span.studyInfoDiv")).GetAttribute("innerHTML");
                    if (studies.ViewStudy() == true && patientinfo.Contains(row["Patient Name"].Split(',')[0]) && patientinfo.Contains(row["Patient Name"].Split(',')[1]) && patientinfo.Contains(row["Patient ID"]) && studyinfo.Contains(row["Accession"]))
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
                    studies.CloseStudy();
                }
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                IWebElement Cancelbtn0 = BasePage.Driver.FindElement(By.CssSelector("#m_studySearchControl_EmergencyCancelButton"));
                Cancelbtn0.Click();


                //Logout as New User
                login.Logout();

                //Login as New Admin
                login.LoginIConnect(domainname, domainname);

                //Navigate to Maintenance
                maintenance = (Maintenance)login.Navigate("Maintenance");

                //Step-21:Navigate to Audit tab
                maintenance.Navigate("Audit");

                //Validate whether the  log is traced
                BasePage.Driver.SwitchTo().DefaultContent();
                string Logmsg = "DICOM Instances Accessed";
                string Logmsg_1 = "Query/Document Query";
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");

                string Log = "";
                
                if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
                {
                    Log = BasePage.Driver.FindElements(By.CssSelector("table#m_listControl_m_dataListGrid tr"))[1].FindElement(By.CssSelector(" td>span")).Text;
                }
                else
                {
                    Log = BasePage.Driver.FindElement(By.CssSelector("table#m_listControl_m_dataListGrid tr:nth-child(2) td>span")).Text;
                }
                //if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                //{
                //    if (Log.Contains(Logmsg_1))
                //    {
                //        result.steps[++ExecutedSteps].status = "Pass";
                //        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                //    }
                //    else
                //    {
                //        result.steps[++ExecutedSteps].status = "Fail";
                //        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                //        result.steps[ExecutedSteps].SetLogs();
                //    }
                //}
                //else
                //{
                    if (Log.Contains(Logmsg))
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
                //}


                //Step-22:Navigate to Studies Tab
                studies = (Studies)login.Navigate("Studies");

                //**Click cancel to disable Emergency Search
                //Validate the default search is in Custom search
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                if ((BasePage.Driver.FindElement(By.CssSelector("input[id$='customFilterRadio']"))).Enabled == true)
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


                //Step-23:Emergency Search using Firstname
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                IWebElement Emergencybtn3 = studies.Emergencybtn();
                IWebElement Emergencybtn9 = studies.Emergencybtn();
                Emergencybtn3.Click();
                IWebElement Acceptbtn1 = BasePage.Driver.FindElement(By.CssSelector("#m_studySearchControl_EmergencyAcceptButton"));
                Acceptbtn1.Click();
                PageLoadWait.WaitForFrameLoad(5);

                studies.EmergencySearchStudy("", PatientName.Split(',')[1].Trim(), "", "");

                //Validate no study displayed
                if ((BasePage.Driver.FindElement(By.CssSelector("input.emptyRequiredField"))).Displayed == true)
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

                //Step-24:Search using Lastname and Firstname
                studies = (Studies)login.Navigate("Studies");
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                IWebElement CustomSearchBtn = BasePage.Driver.FindElement(By.CssSelector("input[id$='m_customFilterRadio']"));
                CustomSearchBtn.Click();
                IWebElement Emergencybtn5 = studies.Emergencybtn();
                Emergencybtn5.Click();
                IWebElement Acceptbtn3 = BasePage.Driver.FindElement(By.CssSelector("#m_studySearchControl_EmergencyAcceptButton"));
                Acceptbtn3.Click();
                PageLoadWait.WaitForFrameLoad(5);
                studies.ChooseColumns(new string[] { "Gender", "Patient DOB" });


                studies.EmergencySearchStudy(PatientName.Split(',')[0], PatientName.Split(',')[1].Trim(), "", "");
                Dictionary<string, string> row0 = studies.GetMatchingRow(new string[] { "Patient ID", "Gender", "Patient DOB" }, new string[] { PatientID, "", "unknown" });
                if (row0 != null)
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

                //Step-25:Search using Lastname,Firstname and Gender
                studies.EmergencySearchStudy(PatientName.Split(',')[0], PatientName.Split(',')[1].Trim(), "M", "");
                Dictionary<string, string> row1 = studies.GetMatchingRow(new string[] { "Patient ID", "Gender", "Patient DOB" }, new string[] { PatientID, "M", "unknown" });
                if (row1 != null)
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

                //Step-26:Search using Lastname and Gender 'O'          
                /**Study with gender O is needed**/

                studies.EmergencySearchStudy(PatientName.Split(',')[0], PatientName.Split(',')[1].Trim(), gender[1], "");
                Dictionary<string, string> row2 = studies.GetMatchingRow(new string[] { "Patient ID", "Gender", "Patient DOB" }, new string[] { PatientID, "O", "unknown" });
                if (row2 != null)
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

                //Step-27:Search using Lastname,Firstname,Gender and DOB
                studies.EmergencySearchStudy(PatientName.Split(',')[0], PatientName.Split(',')[1].Trim(), gender[0], DOB);
                DateTime dt = Convert.ToDateTime(DOB);
                TreeNode tn = new TreeNode(String.Format("{0:dd-MMM-yyyy}", dt));
                String t = tn.ToString();
                String DOB1 = t.Remove(0, 10);

                //String DOB1 = DateTime.ParseExact(DOB, "dd/MMM/yyyy", CultureInfo.InvariantCulture).ToShortDateString();

                Dictionary<string, string> row3 = studies.GetMatchingRow(new string[] { "Patient ID", "Gender", "Patient DOB" }, new string[] { PatientID, "F", DOB1 });
                if (row3 != null)
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


                //Step-28:Launch the study
                studies.SelectStudy1("Accession", Accession[0]);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    BluRingViewer.LaunchBluRingViewer();
                    String patientinfo1 = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.p_PatientName).GetAttribute("innerHTML");

                    String studyinfo1 = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.span_PatientDOB).GetAttribute("innerHTML");
                    String patientid1 = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_PatientID).GetAttribute("innerHTML");
                    if (patientinfo1.Equals(PatientName.Trim()) && patientid1.Equals(PatientID) && studyinfo1.Contains(FormattedDate))
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

                }
                else
                {
                    studies.LaunchStudy();
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    String patientinfo1 = BasePage.Driver.FindElement(By.CssSelector("span.patientInfoDiv")).GetAttribute("innerHTML");
                    String details1 = patientinfo1.ToLower();
                    String studyinfo1 = BasePage.Driver.FindElement(By.CssSelector("span.studyInfoDiv")).GetAttribute("innerHTML");
                    if (studies.ViewStudy() == true && patientinfo1.Contains(row3["Patient Name"].Split(',')[0]) && patientinfo1.Contains(row3["Patient Name"].Split(',')[1]) && patientinfo1.Contains(row3["Patient ID"]) && studyinfo1.Contains(row3["Accession"]))
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
                    studies.CloseStudy();
                }

                //Navigate to Maintenance
                maintenance = (Maintenance)login.Navigate("Maintenance");

                //Step-29:Navigate to Audit tab
                maintenance.Navigate("Audit");

                //Validate whether the  log is traced
                string Logmsg1 = "DICOM Instances Accessed/Use of Restricted Function";
                string Logmsg2 = "Document Query Success";
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                string Log1 = BasePage.Driver.FindElement(By.CssSelector("table#m_listControl_m_dataListGrid ")).Text;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    if (Log1.Contains(Logmsg2))
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
                }
                else
                {
                    if (Log1.Contains(Logmsg1))
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
                }


                //Logout as  New Admin
                login.Logout();

                /**PreCondition-->A user must be created with GrantAccess and Emergency Access(using Patient history domain)**/


                //Step-30:Login as user of Patient history
                login.LoginIConnect(User1, User1);
                ExecutedSteps++;

                //Step-31:Navigate to Studies Tab
                studies = (Studies)login.Navigate("Studies");


                //Validate that Grant Access button is there
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                IWebElement grantbtn = BasePage.Driver.FindElement(By.CssSelector("#m_grantAccessButton"));
                if (grantbtn.Enabled == false)
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

                //Step-32:Select a study
                studies.SearchStudy("Accession", Accession[0]);
                studies.SelectStudy1("Accession", Accession[0]);

                //Validate that Grant Access button is there
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                IWebElement grantbtn0 = BasePage.Driver.FindElement(By.CssSelector("#m_grantAccessButton"));
                if (grantbtn0.Enabled == true)
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

                //Step-33:Check on emergency search option
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                CustomSearchBtn = BasePage.Driver.FindElement(By.CssSelector("input[id$='m_customFilterRadio']"));
                CustomSearchBtn.Click();
                IWebElement Emergencybtn4 = studies.Emergencybtn();
                Emergencybtn4.Click();
                ExecutedSteps++;

                //Step-34:Accept the warning 

                IWebElement Acceptbtn2 = BasePage.Driver.FindElement(By.CssSelector("#m_studySearchControl_EmergencyAcceptButton"));
                Acceptbtn2.Click();
                PageLoadWait.WaitForFrameLoad(5);
                /****/
                studies.EmergencySearchStudy(PatientName.Split(',')[0], "", gender[0], DOB);
                Dictionary<string, string> row4 = studies.GetMatchingRow(new string[] { "Accession", "Patient ID", }, new string[] { Accession[0], PatientID });

                //Perform Emergency search
                studies.EmergencySearchStudy(PatientName.Split(',')[0], PatientName.Split(',')[1].Trim(), gender[0], DOB);


                //Validate Grant access and Transfer buttons are disabled
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                IWebElement grantbtn1 = BasePage.Driver.FindElement(By.CssSelector("#m_grantAccessButton"));
                IWebElement transferbtn = BasePage.Driver.FindElement(By.CssSelector("#m_transferButton"));
                if (row4 == null && grantbtn1.Enabled == false && transferbtn.Enabled == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-35:Select a study
                studies.SelectStudy1("Accession", Accession[0]);
                //Validate the buttons are disabled
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                IWebElement grantbtn2 = BasePage.Driver.FindElement(By.CssSelector("#m_grantAccessButton"));
                IWebElement transferbtn0 = BasePage.Driver.FindElement(By.CssSelector("#m_transferButton"));
                if (grantbtn2.Enabled == false && transferbtn0.Enabled == false)
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

                //Step-36:Launch the study
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    BluRingViewer.LaunchBluRingViewer();

                }
                else
                {
                    studies.LaunchStudy();
                }

                //Validate whether Grant Access and Study Transfer buttons are not visible from toolbar
                IList<String> tools = studies.GetReviewToolsFromviewer();
                var tool1 = tools.Where(tool => tool.Equals("Transfer Study"));
                var tool2 = tools.Where(tool => tool.Equals("Grant Access"));
                if (tool1.Count() == 0 && tool2.Count() == 0)
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

                //Logout as user
                login.Logout();

                /**Precondition-->New domain and user must be created with Emergerncy Search enabled in both**/
                /**Rolefilter must be updated as CT and Study Date as from 01/01/2006 to 01/01/2010**/

                //Step-37:Create a user
                //Login as domainadmin
                login.LoginIConnect(domainname, domainname);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(user, domain, rolename);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectRole(rolename);
                rolemanagement.ClickEditRole();

                ExecutedSteps++;

                //Step-38:Select Study date and set the range
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                IWebElement StudyDate1 = BasePage.Driver.FindElement(By.CssSelector("select[id$='_RoleAccessFilter_FilterDropDownList']"));
                rolemanagement.SelectFromList(StudyDate1, "Study Date", 1);
                IWebElement From = BasePage.Driver.FindElement(By.CssSelector("input#roleDateFrom"));
                IWebElement To = BasePage.Driver.FindElement(By.CssSelector("input#roleDateTo"));
                //Date format:M/d/yyyy
                From.Clear(); From.SendKeys("1/1/2006");
                To.Clear(); To.SendKeys("1/1/2010");
                IWebElement addbtn = BasePage.Driver.FindElement(By.CssSelector("input[id$='_RoleAccessFilter_AddButton']"));
                addbtn.Click();
                addbtn.Click();
                //Select CT Modality from Access filter
                rolemanagement.RoleFilter_Modality("CT");
                rolemanagement.ClickSaveEditRole();
                ExecutedSteps++;
                //Logout
                login.Logout();

                //Step-39:Login as New User
                login.LoginIConnect(user, pwd);
                ExecutedSteps++;

                //Step-40:Navigate to Studies Tab
                studies = (Studies)login.Navigate("Studies");

                //Perform Emergency search with study date by accepting the warning
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                CustomSearchBtn = BasePage.Driver.FindElement(By.CssSelector("input[id$='m_customFilterRadio']"));
                CustomSearchBtn.Click();
                IWebElement Emergencybtn6 = studies.Emergencybtn();
                PageLoadWait.WaitForFrameLoad(5);
                Emergencybtn6.Click();
                IWebElement Acceptbtn4 = BasePage.Driver.FindElement(By.CssSelector("#m_studySearchControl_EmergencyAcceptButton"));
                Acceptbtn4.Click();
                PageLoadWait.WaitForFrameLoad(5);
                studies.EmergencySearchStudy(PatientName.Split(',')[0], PatientName.Split(',')[1].Trim(), "M", "00000000");
                DateTime dt1 = Convert.ToDateTime(StudyDate);
                TreeNode tn1 = new TreeNode(String.Format("{0:dd-MMM-yyyy}", dt1));
                String t1 = tn1.ToString();
                String StudyDate2 = t1.Remove(0, 10);
                //Validate that the study listed are not in range specified
                Dictionary<string, string> study = studies.GetMatchingRow(new string[] { "Patient Name" }, new string[] { PatientName });

                if (study != null && study["Study Date"].Contains(StudyDate2))
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



                //Step-41:Launch the study
                studies.SelectStudy1("Accession", Accession[1]);
                //Dictionary<string, string> study1 = studies.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Accession" }, new string[] { PatientName, PatientID, Accession[1] });
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    BluRingViewer.LaunchBluRingViewer();
                    String patientinfo0 = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.p_PatientName).GetAttribute("innerHTML");
                    String patientid = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_PatientID).GetAttribute("innerHTML");
                    viewer.CloseBluRingViewer();
                    if (patientinfo0.Equals(PatientName.Trim()) && patientid.Equals(PatientID))
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

                else

                {
                    studies.LaunchStudy();
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    String patientinfo0 = BasePage.Driver.FindElement(By.CssSelector("span.patientInfoDiv")).GetAttribute("innerHTML");
                    String studyinfo0 = BasePage.Driver.FindElement(By.CssSelector("span.studyInfoDiv")).GetAttribute("innerHTML");
                    studies.CloseStudy();
                    if (patientinfo0.Contains(study["Patient Name"].Split(',')[0]) && patientinfo0.Contains(study["Patient Name"].Split(',')[1]) && patientinfo0.Contains(study["Patient ID"]) && studyinfo0.Contains(study["Accession"]))
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
                    IWebElement Acceptbtn5 = BasePage.Driver.FindElement(By.CssSelector("#m_studySearchControl_EmergencyAcceptButton"));
                    Acceptbtn5.Click();
                }
                
                //Logout as user
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
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                //Logout 
                login.Logout();

                //Return Result
                return result;

            }
        }


        /// <summary>
        /// Login Message
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161363(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables          
            Inbounds inbounds = null;
            DomainManagement domainmanagement;
            SystemSettings systemsettings;
            UserManagement usermanagement;
            Studies studies;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data

                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                String username1 = Config.ph1UserName;
                String password1 = Config.ph1Password;
                String urls = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "URL");
                String[] url = urls.Split(' ');
                String pid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");
                String role = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String domainname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String username = "U5" + new Random().Next(1, 10000);
                String password = "U5" + new Random().Next(1, 10000); ;
                String studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");

                //Upload a study to destination to be viewed in studies tab
                //BasePage.RunBatchFile(Config.batchfilepath, Config.TestDataPath + studypath + " " + Config.dicomsendpath + " " + Config.SanityPACS);

                /**Precondition--> Login as Administrator. 
                Empty the url from domain management page and leave the url as is from system setting. 
                Check box"Use system settings for login message". Save the settings. 
                From System Settings page, uncheck"Allow user to suppress Login Message". Save the settings.**/

                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);
                systemsettings = (SystemSettings)login.Navigate("SystemSettings");
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                //IWebElement allow0 = BasePage.Driver.FindElement(By.CssSelector("#AllowSuppressLoginMessageCheck"));
                //allow0.Click();
                systemsettings.ModifyLoginURLinSys(url[0]);
                systemsettings.SaveSystemSettings();

                login.Logout();


                //Step-1:Login as Admin
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);

                try
                {
                    BasePage.Driver.SwitchTo().DefaultContent();
                    String yahoo = BasePage.Driver.FindElement(By.CssSelector("iframe#LoginMessageIframe")).GetAttribute("src");
                    BasePage.Driver.SwitchTo().Frame("LoginMessageIframe");
                    IWebElement yahoopage = BasePage.Driver.FindElement(By.CssSelector("input#sb_form_q"));
                    if (yahoopage.Displayed && yahoo.Equals(url[0]))
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
                }
                catch (Exception e)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                BasePage.Driver.SwitchTo().DefaultContent();
                //BasePage.Driver.FindElement(By.CssSelector("#OkButton")).Click();
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector('#OkButton').click()");

                //Step-2:Navigate to SystemSettings
                systemsettings = (SystemSettings)login.Navigate("SystemSettings");

                //Validate Allow user checkbox is checked
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                IWebElement allow = BasePage.Driver.FindElement(By.CssSelector("#AllowSuppressLoginMessageCheck"));
                //allow.Click();
                if (allow.Selected == true)
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

                //Step-3:Modify the URL in Login message address box

                IWebElement reset = BasePage.Driver.FindElement(By.CssSelector("#ResetWarningCheck"));
                reset.Click();
                systemsettings.SaveSystemSettings();
                systemsettings.ModifyLoginURLinSys(url[0]);


                //Save the changes
                systemsettings.SaveSystemSettings();
                ExecutedSteps++;

                //Step-4:Logout as Admin
                login.Logout();

                //Login as Admin and Dismiss the popup
                Boolean check = login.LoginMessageBox(adminusername, adminpassword, 1);
                try
                {
                    BasePage.Driver.SwitchTo().DefaultContent();
                    string yahoo0 = BasePage.Driver.FindElement(By.CssSelector("iframe#LoginMessageIframe")).GetAttribute("src");
                    BasePage.Driver.SwitchTo().Frame("LoginMessageIframe");
                    IWebElement yahoopage4 = BasePage.Driver.FindElement(By.CssSelector("input#sb_form_q"));
                    if (check == false && yahoo0.Equals(url[0]) && yahoopage4.Displayed)
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
                }
                catch (Exception e)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5:Try to interact with the mainpage
                BasePage.Driver.SwitchTo().DefaultContent();
                IWebElement popup = BasePage.Driver.FindElement(By.CssSelector("#LoginMessageDiv"));
                if (popup.Displayed == true)
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

                //Step-6:Dismiss the Popup
                login.DismissLoginPopup();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                IWebElement element = BasePage.Driver.FindElement(By.CssSelector("#TabText0"));
                if (element.Displayed == true)
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


                //Step-7:Navigate to Studies
                studies = (Studies)login.Navigate("Studies");

                //Launch the study
                studies.SearchStudy("Accession", accession);
                studies.SelectStudy1("Accession", accession);
                studies.LaunchStudy();
                if (studies.ViewStudy() == true)
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
                //Close the study
                studies.CloseStudy();

                //Step-8:Navigate to DomainManagement Tab
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");


                //Click Edit in DomainManagement Tab
                domainmanagement.SearchDomain(domainname);
                domainmanagement.SelectDomain(domainname);
                domainmanagement.ClickEditDomain();

                //Validate that UseSystemSettings checkbox is set as default
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                IWebElement use = BasePage.Driver.FindElement(By.CssSelector("[id$='_UseSystemSettingsForLoginMessageCB']"));
                if (use.Selected == true)
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


                //Step-9:Uncheck the UseSystemSettings Login checkbox
                domainmanagement.SetCheckBoxInEditDomain("login", 1);
                //Fill in the URL
                domainmanagement.ModifyLoginURLinDomain(url[1]);

                //Check the Allow User checkbox
                domainmanagement.SetCheckBoxInEditDomain("allow", 0);

                //Save the changes
                domainmanagement.ClickSaveDomain();
                ExecutedSteps++;

                //Step-10:Logout as Admin
                login.Logout();

                //Login as Admin and Click on Dont Display Message
                login.LoginMessageBox(adminusername, adminpassword, 0);

                //Validating precedence of domainpage
                BasePage.Driver.SwitchTo().DefaultContent();
                try
                {

                    string google = BasePage.Driver.FindElement(By.CssSelector("iframe#LoginMessageIframe")).GetAttribute("src");
                    BasePage.Driver.SwitchTo().Frame("LoginMessageIframe");
                    IWebElement googlepage = Driver.FindElement(By.CssSelector("input#searchInput"));
                    if (google.Equals(url[1]) && googlepage.Displayed)
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
                }
                catch (Exception e)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }



                //Step-11:Click ok
                login.DismissLoginPopup();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                IWebElement element1 = BasePage.Driver.FindElement(By.Id("TabText0"));
                if (element1.Displayed == true)
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

                //Step-12:Logout as Admin
                login.Logout();


                //Login as Admin
                login.LoginIConnect(adminusername, adminpassword);

                //Validate that the dialog Box didnt display
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                IWebElement domaintab = BasePage.Driver.FindElement(By.Id("TabText1"));
                if (domaintab.Displayed == true)
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

                //Step-13:Navigate to UserManagement tab
                usermanagement = (UserManagement)login.Navigate("UserManagement");

                //Create a new user in the same domain
                usermanagement.CreateUser(username, domainname, role);

                //Logout as Admin
                login.Logout();
                ExecutedSteps++;

                //Step-14:Login as User  
                login.LoginMessageBox(username, password, 1);
                BasePage.Driver.SwitchTo().DefaultContent();
                try
                {

                    string google0 = BasePage.Driver.FindElement(By.CssSelector("iframe#LoginMessageIframe")).GetAttribute("src");
                    BasePage.Driver.SwitchTo().Frame("LoginMessageIframe");
                    IWebElement googlepage14 = Driver.FindElement(By.CssSelector("input#searchInput"));
                    if (google0.Equals(url[1]) && googlepage14.Displayed)
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
                }
                catch (Exception e)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-15:Dismiss the warning and load a study
                login.DismissLoginPopup();

                //Search and select study
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Accession", accession);
                studies.SelectStudy1("Accession", accession);


                //Launch study
                studies.LaunchStudy();
                if (studies.ViewStudy() == true)
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

                //Close study
                studies.CloseStudy();

                //Logout as User
                login.Logout();

                //Step-16:Login as ph
                Boolean check1 = login.LoginMessageBox(username1, password1, 1);
                BasePage.Driver.SwitchTo().DefaultContent();
                try
                {
                    string google1 = BasePage.Driver.FindElement(By.CssSelector("iframe#LoginMessageIframe")).GetAttribute("src");
                    BasePage.Driver.SwitchTo().Frame("LoginMessageIframe");
                    IWebElement googlepage16 = BasePage.Driver.FindElement(By.CssSelector("input#searchInput"));
                    //Validate that dialog box with OK button nly
                    if (check1 == false && google1.Equals(url[1]) && googlepage16.Displayed)
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
                }
                catch (Exception e)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                login.DismissLoginPopup();


                //Logout as ph
                login.Logout();

                //Step-17:Login as Admin
                login.LoginIConnect(adminusername, adminpassword);

                //Navigate to DomainManagement Tab
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");

                //Click Edit in DomainManagement Tab
                domainmanagement.SearchDomain(domainname);
                domainmanagement.SelectDomain(domainname);
                domainmanagement.ClickEditDomain();

                //Reset,check Allow user,fill url and save
                domainmanagement.ResetLoginMessage();
                domainmanagement.SetCheckBoxInEditDomain("login", 1);
                domainmanagement.SetCheckBoxInEditDomain("allow", 0);
                domainmanagement.ModifyLoginURLinDomain(url[1]);
                domainmanagement.ClickSaveDomain();
                ExecutedSteps++;


                //Step-18:Logout as Admin
                login.Logout();

                //Login as Admin
                Boolean check2 = login.LoginMessageBox(adminusername, adminpassword, 1);
                BasePage.Driver.SwitchTo().DefaultContent();
                try
                {
                    string google1 = BasePage.Driver.FindElement(By.CssSelector("iframe#LoginMessageIframe")).GetAttribute("src");
                    BasePage.Driver.SwitchTo().Frame("LoginMessageIframe");
                    IWebElement googlepage18 = Driver.FindElement(By.CssSelector("input#searchInput"));
                    //Validate the dialog box
                    if (check2 == false && google1.Equals(url[1]) && googlepage18.Displayed)
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
                }
                catch (Exception e)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-19:Dismiss by clicking OK
                login.DismissLoginPopup();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                IWebElement element0 = BasePage.Driver.FindElement(By.Id("TabText0"));
                if (element0.Displayed == true)
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

                //Logout as Admin
                login.Logout();

                //Step-20:Integrated Mode to login(not Automated)
                result.steps[++ExecutedSteps].status = "Not Automated";

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
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Logout 
                login.Logout();

                //Return Result
                return result;
            }
            finally
            {
                String domainname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                //Login as Admin                
                login.LoginMessageBox(Config.adminUserName, Config.adminPassword, 1);
                login.DismissLoginPopup();
                //Navigate to SystemSettings
                systemsettings = (SystemSettings)login.Navigate("SystemSettings");
                //Disabling the setting configured in SystemSettings and saving
                systemsettings.ModifyLoginURLinSys("");
                systemsettings.SaveSystemSettings();
                //Navigate to DomainManagement Tab
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                //Disabling the setting configured in DomainManagement and saving
                domainmanagement.SearchDomain(domainname);
                domainmanagement.SelectDomain(domainname);
                domainmanagement.ClickEditDomain();
                domainmanagement.ModifyLoginURLinDomain("");
                domainmanagement.SetCheckBoxInEditDomain("login", 0);
                domainmanagement.ClickSaveEditDomain();
                //Logout 
                login.Logout();

            }
        }

        ///<summary>
        /// Viewer Service Monitoring
        /// </summary>
        public TestCaseResult Test_161364(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Maintenance maintenance = new Maintenance();
            ServiceTool servicetool = new ServiceTool();
            Studies studies = new Studies();
            BasePage basepage = new BasePage();
            StudyViewer viewer = new StudyViewer();
            BluRingViewer bluViewer = new BluRingViewer();
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            string adminUsername = Config.adminUserName;
            string adminPassword = Config.adminPassword;
            string[] dataSource = new String[] { basepage.GetHostName(Config.EA1), basepage.GetHostName(Config.EA91) };
            string patientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
            string[] RDMIP = new String[] { Config.RDMIP, Config.RDMIP2 };
            string[] RDMIPPassword = new String[] { "PQAte$t123-"+ login.GetHostName(Config.RDMIP).ToLowerInvariant(), "PQAte$t123-" + login.GetHostName(Config.RDMIP2).ToLowerInvariant() };
            string[] patientID = patientIDList.Split(':');
            //string[] accNo = AccessionList.Split(':');
            string link = String.Empty;
            Dictionary<string, string> downloadedMail = new Dictionary<string, string>();
            EmailUtils AdminUser = new EmailUtils() { EmailId = Config.AdminEmail, Password = Config.AdminEmailPassword };
            try
            {
                //Pre-Condition
                Stopwatch stopwatch = new Stopwatch();
                TimeSpan timeout = new TimeSpan(0, 3, 0);
                BasePage.MultiDriver = new List<IWebDriver>();
                basepage.ChangeAttributeValue("C:\\WebAccess\\WebAccess\\Config\\ViewerBalancerManagementConfiguration.xml", "/ViewerServices/ResultList/Columns/Column[@Id='ServiceMonitored']", "Hidden", "n");
                servicetool.LaunchServiceTool();
                wpfObject.WaitTillLoad();
                servicetool.NavigateToViewerTab();
                wpfObject.WaitTillLoad();
                servicetool.NavigateSubTab("Viewer Service");
                wpfObject.WaitTillLoad();
                servicetool.EnableMonitoringViewerService(RDMIP[0], "120", true);
                wpfObject.WaitTillLoad();
                servicetool.EnableMonitoringViewerService(RDMIP[1], "120", true);
                wpfObject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                wpfObject.WaitTillLoad();
                servicetool.CloseServiceTool();


                //Step 1: From one browser, log in as Administrator and navigate to Maintenance *^>^ *Viewer Services
                BasePage.MultiDriver.Add(BasePage.Driver);
                BasePage.Driver = BasePage.MultiDriver[0];
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUsername, adminPassword);

                maintenance = (Maintenance)login.Navigate("Maintenance");
                PageLoadWait.WaitForFrameLoad(20);
                maintenance.Navigate("Viewer Services");
                PageLoadWait.WaitForFrameLoad(20);
                DataTable table = CollectRecordsInTable(maintenance.Tbl_ViewerServicesTable(), maintenance.TableHeader1(), maintenance.TableRow());
                string[] Status = GetColumnValues(table, "Status");
                string[] Monitored = GetColumnValues(table, "Monitored");

                bool Step1_1 = table.Rows.Count == 4;
                bool Step1_2 = Status.All(st => string.Equals(st, "Enabled", StringComparison.OrdinalIgnoreCase));
                bool Step1_3 = Monitored.All(mt => string.Equals(mt, "No", StringComparison.OrdinalIgnoreCase));

                if (Step1_1 && Step1_2 && Step1_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step1_1" + Step1_1);
                    Logger.Instance.InfoLog("Step1_2" + Step1_2);
                    Logger.Instance.InfoLog("Step1_3" + Step1_3);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //step 2: From another browser log in and load a study
                BasePage.MultiDriver.Add(login.InvokeBrowser(Config.BrowserType));
                login.SetDriver(BasePage.MultiDriver[1]);
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUsername, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: patientID[0], Datasource: dataSource[0]);
                PageLoadWait.WaitForFrameLoad(20);
                studies.SelectStudy1("Patient ID", patientID[0]);
                PageLoadWait.WaitForFrameLoad(20);                
                bluViewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (bluViewer.CompareImage(result.steps[ExecutedSteps], bluViewer.studyPanel()))
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

                //Step 3: From the browser that is viewing the Viewer Services status, click refresh.
                login.SetDriver(BasePage.MultiDriver[0]);
                PageLoadWait.WaitForFrameLoad(20);
                ClickElement(maintenance.refreshBtn());
                PageLoadWait.WaitForFrameLoad(60);
                table = CollectRecordsInTable(maintenance.Tbl_ViewerServicesTable(), maintenance.TableHeader1(), maintenance.TableRow());
                string[] Clients = GetColumnValues(table, "Clients");
                string[] ServiceLocation = GetColumnValues(table, "Service Location");
                string[] ServiceType = GetColumnValues(table, "Service Type");
                if(string.Equals(Clients[2], "1"))
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

                //Step 4:Stop iis on the remote viewer service server that is currently being used and attempt to apply tools to the loaded study.
                AdminUser.MarkAllMailAsRead();
                basepage.ResetIISOnRemoteMachine(RDMIP[0], "Administrator", RDMIPPassword[0], "stop");
                login.SetDriver(BasePage.MultiDriver[1]);
                bluViewer.CloseBluRingViewer();
                studies.SearchStudy(patientID: patientID[1], Datasource: dataSource[1]);
                PageLoadWait.WaitForFrameLoad(20);
                studies.SelectStudy1("Patient ID", patientID[1]);
                PageLoadWait.WaitForFrameLoad(20);
                bluViewer = BluRingViewer.LaunchBluRingViewer();
                //Doubleclick("cssselector", bluViewer.GetStudyPanelThumbnailCss(2));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool Step4 = bluViewer.CompareImage(result.steps[ExecutedSteps], bluViewer.studyPanel());
                if (Step4)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step4_1" + Step1_1);
                    Logger.Instance.InfoLog("Step4_2" + Step1_2);
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 5: Refresh the Viewer Services status page.
                login.SetDriver(BasePage.MultiDriver[0]);
                PageLoadWait.WaitForFrameLoad(60);
                ClickElement(maintenance.refreshBtn());
                PageLoadWait.WaitForFrameLoad(60);
                table = CollectRecordsInTable(maintenance.Tbl_ViewerServicesTable(), maintenance.TableHeader1(), maintenance.TableRow());
                Clients = GetColumnValues(table, "Clients");
                ServiceLocation = GetColumnValues(table, "Service Location");
                ServiceType = GetColumnValues(table, "Service Type");
                Monitored = GetColumnValues(table, "Monitored");
                Status = GetColumnValues(table, "Status");
                bool Step5_1 = string.Equals(Status[2], "Disabled");
                bool Step5_2 = string.Equals(Monitored[2], "Yes");
                bool Step5_3 = string.Equals(Clients[2], "0");
                bool Step5_4 = string.Equals(Clients[3], "1");

                if (Step5_1 && Step5_2 && Step5_3 && Step5_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step5_1 = " + Step5_1);
                    Logger.Instance.InfoLog("Step5_2 = " + Step5_2);
                    Logger.Instance.InfoLog("Step5_3 = " + Step5_3);
                    Logger.Instance.InfoLog("Step5_4 = " + Step5_4);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 6:
                downloadedMail = AdminUser.GetMailUsingIMAP(Config.SystemEmail, "Cannot connect to Imaging Data web service", MarkAsRead: true, maxWaitTime: 3);
                if(downloadedMail.Count > 0)
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

                //Step 7: Refresh the Viewer Services status page.
                /*Stopwatch timeToLoad = new Stopwatch();
                //bool IsIIS_Started = tool.ResetRemoteIISUsingexe(RDMIP[0], "START");
                timeToLoad = Stopwatch.StartNew();*/
                basepage.ResetIISOnRemoteMachine(RDMIP[0], "Administrator", RDMIPPassword[0], "start");
                /*ClickElement(maintenance.refreshBtn());
                ClickElement(maintenance.refreshBtn());
                double actualtimeout = 0;
                for (int i = 0; i < 14; i++)
                {
                    PageLoadWait.WaitForFrameLoad(60);
                    table = CollectRecordsInTable(maintenance.Tbl_ViewerServicesTable(), maintenance.TableHeader1(), maintenance.TableRow());
                    Status = GetColumnValues(table, "Status");
                    if(string.Equals(Status[2], "Enabled"))
                    {
                        actualtimeout = timeToLoad.Elapsed.TotalSeconds;
                        timeToLoad.Stop();
                        break;
                    }
                    else
                    {
                        ClickElement(maintenance.refreshBtn());
                        Thread.Sleep(10000);
                    }
                }*/
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout)) { /*Stay Idle for 3 Miniutes*/ }
                stopwatch.Stop();
                stopwatch.Reset();
                ClickElement(maintenance.refreshBtn());
                PageLoadWait.WaitForFrameLoad(60);
                table = CollectRecordsInTable(maintenance.Tbl_ViewerServicesTable(), maintenance.TableHeader1(), maintenance.TableRow());
                Clients = GetColumnValues(table, "Clients");
                ServiceLocation = GetColumnValues(table, "Service Location");
                ServiceType = GetColumnValues(table, "Service Type");
                Monitored = GetColumnValues(table, "Monitored");
                Status = GetColumnValues(table, "Status");
                bool Step7_1 = string.Equals(Status[2], "Enabled");
                bool Step7_2 = string.Equals(Monitored[2], "No");
                bool Step7_3 = string.Equals(Clients[2], "0");
                bool Step7_4 = string.Equals(Clients[3], "1");

                if (Step7_1 && Step7_2 && Step7_3 && Step7_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step7_1 = " + Step7_1);
                    Logger.Instance.InfoLog("Step7_2 = " + Step7_2);
                    Logger.Instance.InfoLog("Step7_3 = " + Step7_3);
                    Logger.Instance.InfoLog("Step7_4 = " + Step7_4);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 8: Log out and back in from the browser that the studies were loaded through.
                login.SetDriver(BasePage.MultiDriver[1]);
                PageLoadWait.WaitForFrameLoad(20);
                bluViewer.CloseBluRingViewer();
                login.Logout();
                login.LoginIConnect(adminUsername, adminPassword);
                ExecutedSteps++;

                //Step 9: Stop iis on the first remote viewer service server, relogin and attempt to load a study.

                basepage.ResetIISOnRemoteMachine(RDMIP[0], "Administrator", RDMIPPassword[0], "stop");
                login.Logout();
                login.LoginIConnect(adminUsername, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(20);
                studies.SearchStudy(patientID: patientID[1]);
                PageLoadWait.WaitForFrameLoad(20);
                studies.SelectStudy1("Patient ID", patientID[1]);
                PageLoadWait.WaitForFrameLoad(20);
                bluViewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (bluViewer.CompareImage(result.steps[ExecutedSteps], bluViewer.studyPanel()))
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

                //Step 10: Refresh the Viewer Services status page
                login.SetDriver(BasePage.MultiDriver[0]);
                PageLoadWait.WaitForFrameLoad(20);
                ClickElement(maintenance.refreshBtn());
                PageLoadWait.WaitForFrameLoad(20);

                table = CollectRecordsInTable(maintenance.Tbl_ViewerServicesTable(), maintenance.TableHeader1(), maintenance.TableRow());
                Clients = GetColumnValues(table, "Clients");
                ServiceLocation = GetColumnValues(table, "Service Location");
                ServiceType = GetColumnValues(table, "Service Type");
                Monitored = GetColumnValues(table, "Monitored");
                Status = GetColumnValues(table, "Status");
                bool Step10_1 = string.Equals(Status[2], "Disabled");
                bool Step10_2 = string.Equals(Monitored[2], "Yes");
                bool Step10_3 = string.Equals(Clients[2], "0");
                bool Step10_4 = string.Equals(Clients[3], "1");

                if (Step10_1 && Step10_2 && Step10_3 && Step10_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step10_1 = " + Step10_1);
                    Logger.Instance.InfoLog("Step10_2 = " + Step10_2);
                    Logger.Instance.InfoLog("Step10_3 = " + Step10_3);
                    Logger.Instance.InfoLog("Step10_4 = " + Step10_4);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 11: Double click on a thumbnail to load a series.
                BasePage.Driver = BasePage.MultiDriver[1];
                //new Actions(BasePage.Driver).MoveToElement(BasePage.Driver.FindElement(By.CssSelector(bluViewer.GetStudyPanelThumbnailCss(2)))).DoubleClick().Perform();
                Doubleclick("cssselector", bluViewer.GetStudyPanelThumbnailCss(2));
                Thread.Sleep(3000);
                Doubleclick("cssselector", bluViewer.GetStudyPanelThumbnailCss(2));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (bluViewer.CompareImage(result.steps[ExecutedSteps], bluViewer.studyPanel()))
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

                //Step 12: Start iis on the remote viewer service server that it was stopped on. Wait for 2 minutes and refresh the Viewer Services status page.
                BasePage.Driver = BasePage.MultiDriver[0];
                login.SetDriver(BasePage.MultiDriver[0]);
                //timeToLoad = Stopwatch.StartNew();
                basepage.ResetIISOnRemoteMachine(RDMIP[0], "Administrator", RDMIPPassword[0], "start");
                /*ClickElement(maintenance.refreshBtn());
                ClickElement(maintenance.refreshBtn());
                actualtimeout = 0;
                for (int i = 0; i < 14; i++)
                {
                    table = CollectRecordsInTable(maintenance.Tbl_ViewerServicesTable(), maintenance.TableHeader1(), maintenance.TableRow());
                    Status = GetColumnValues(table, "Status");
                    if (string.Equals(Status[2], "Enabled"))
                    {
                        actualtimeout = timeToLoad.Elapsed.TotalSeconds;
                        timeToLoad.Stop();
                        break;
                    }
                    else
                    {
                        ClickElement(maintenance.refreshBtn());
                        Thread.Sleep(10000);
                    }
                }*/
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout)) { /*Stay Idle for 3 Miniutes*/ }
                stopwatch.Stop();
                stopwatch.Reset();
                ClickElement(maintenance.refreshBtn());

                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                table = CollectRecordsInTable(maintenance.Tbl_ViewerServicesTable(), maintenance.TableHeader1(), maintenance.TableRow());
                Clients = GetColumnValues(table, "Clients");
                ServiceLocation = GetColumnValues(table, "Service Location");
                ServiceType = GetColumnValues(table, "Service Type");
                Monitored = GetColumnValues(table, "Monitored");
                Status = GetColumnValues(table, "Status");
                bool Step12_1 = string.Equals(Status[2], "Enabled");
                bool Step12_2 = string.Equals(Monitored[2], "No");
                bool Step12_3 = string.Equals(Clients[2], "0");
                bool Step12_4 = string.Equals(Clients[3], "1");

                if (Step12_1 && Step12_2 && Step12_3 && Step12_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step12_1 = " + Step12_1);
                    Logger.Instance.InfoLog("Step12_2 = " + Step12_2);
                    Logger.Instance.InfoLog("Step12_3 = " + Step12_3);
                    Logger.Instance.InfoLog("Step12_4 = " + Step12_4);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 13:Log in from a third browser and load a study
                BasePage.MultiDriver.Add(login.InvokeBrowser(Config.BrowserType));
                login.SetDriver(BasePage.MultiDriver[2]);
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUsername, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(20);
                studies.SearchStudy(patientID: patientID[0], Datasource: dataSource[0]);
                PageLoadWait.WaitForFrameLoad(20);
                studies.SelectStudy1("Patient ID", patientID[0]);
                PageLoadWait.WaitForFrameLoad(20);
                bluViewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (bluViewer.CompareImage(result.steps[ExecutedSteps], bluViewer.studyPanel()))
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

                //Step 14: Refresh the Viewer Services status page.
                login.SetDriver(BasePage.MultiDriver[0]);
                PageLoadWait.WaitForFrameLoad(20);
                ClickElement(maintenance.refreshBtn());
                ClickElement(maintenance.refreshBtn());
                PageLoadWait.WaitForFrameLoad(60);
                table = CollectRecordsInTable(maintenance.Tbl_ViewerServicesTable(), maintenance.TableHeader1(), maintenance.TableRow());
                Clients = GetColumnValues(table, "Clients");
                Monitored = GetColumnValues(table, "Monitored");
                Status = GetColumnValues(table, "Status");
                bool Step14_1 = string.Equals(Status[2], "Enabled");
                bool Step14_2 = string.Equals(Monitored[2], "No");
                bool Step14_3 = string.Equals(Clients[2], "1");
                bool Step14_4 = string.Equals(Clients[3], "1");

                if (Step14_1 && Step14_2 && Step14_3 && Step14_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step14_1 = " + Step14_1);
                    Logger.Instance.InfoLog("Step14_2 = " + Step14_2);
                    Logger.Instance.InfoLog("Step14_3 = " + Step14_3);
                    Logger.Instance.InfoLog("Step14_4 = " + Step14_4);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Return Result 
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Results
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
                login.closeallbrowser();
                login.Logout();
                basepage.ChangeAttributeValue(Config.ServiceFactoryConfigPath, "/parameter[@name='controlledServices']", "value", "Imaging.Managed.Local");
                basepage.ChangeAttributeValue(Config.ServiceFactoryConfigPath, "/parameter[@name='controlledBaseUrls']", "value", "Imaging.Managed.Local");
                basepage.ChangeAttributeValue(Config.ServiceFactoryConfigPath, "/parameter[@name='monitorControlledServicesEnable']", "value", "false");
                servicetool.LaunchServiceTool();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                basepage.ResetIISOnRemoteMachine(RDMIP[0], "Administrator", RDMIPPassword[0], "iisreset");
            }
        }

        ///<summary>
        /// System/Domain/Role/User Preferences - Enable Universal viewer
        /// </summary>
        public TestCaseResult Test_163598(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            DomainManagement domainManagement = new DomainManagement();
            RoleManagement roleManagement = new RoleManagement();
            UserPreferences userPreferences = new UserPreferences();
            Studies studies = new Studies();
            BasePage basepage = new BasePage();
            StudyViewer viewer = new StudyViewer();
            BluRingViewer bluViewer = new BluRingViewer();
            ServiceTool serviceTool = new ServiceTool();
            int ExecutedSteps = -1;
            string Accession = string.Empty;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            string DS = string.Empty;
            try
            {
                Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                DS = login.GetHostName(Config.PACS2);
                //Step 1: 
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 2: 
                domainManagement = (DomainManagement)login.Navigate("DomainManagement");
                domainManagement.ClickNewDomainBtn();
                PageLoadWait.WaitForPageLoad(60);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                bool Step2_1 = domainManagement.Domain_UniversalViewer().Displayed;
                if (!domainManagement.Domain_UniversalViewer().Selected)
                {
                    ClickElement(domainManagement.Domain_UniversalViewer());
                }
                bool Step3_1 = domainManagement.DefaultUniversalViewer().Displayed;
                bool Step3_2 = domainManagement.DefaultEnterpriseViewer().Displayed;
                domainManagement.CloseDomainManagement();
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                domainManagement.SearchDomain(Config.adminGroupName);
                domainManagement.SelectDomain(Config.adminGroupName);
                domainManagement.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                bool Step2_2 = domainManagement.Domain_UniversalViewer().Displayed;
                if (Step2_1 && Step2_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step2_1 = " + Step2_1);
                    Logger.Instance.InfoLog("Step2_2 = " + Step2_2);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 3:
                if (!domainManagement.Domain_UniversalViewer().Selected)
                {
                    ClickElement(domainManagement.Domain_UniversalViewer());
                }
                bool Step3_3 = domainManagement.DefaultUniversalViewer().Displayed;
                bool Step3_4 = domainManagement.DefaultEnterpriseViewer().Displayed;
                if (Step3_1 && Step3_2 && Step3_3 && Step3_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step3_1 = " + Step3_1);
                    Logger.Instance.InfoLog("Step3_2 = " + Step3_2);
                    Logger.Instance.InfoLog("Step3_3 = " + Step3_3);
                    Logger.Instance.InfoLog("Step3_4 = " + Step3_4);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 4: 
                if (!domainManagement.DefaultUniversalViewer().Selected)
                {
                    ClickElement(domainManagement.DefaultUniversalViewer());
                }
                domainManagement.ClickSaveEditDomain();
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 5:
                roleManagement = (RoleManagement)login.Navigate("RoleManagement");
                roleManagement.SearchRole(Config.adminRoleName, Config.adminGroupName);
                roleManagement.SelectRole(Config.adminRoleName);
                roleManagement.ClickEditRole();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                if (roleManagement.AllowUniversalViewer().Displayed)
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

                //Step 6:
                if (!roleManagement.AllowUniversalViewer().Selected)
                {
                    ClickElement(roleManagement.AllowUniversalViewer());
                }
                bool Step6_1 = roleManagement.DefaultUniversalViewer().Displayed;
                bool Step6_2 = roleManagement.DefaultEnterpriseViewer().Displayed;
                if (!roleManagement.DefaultUniversalViewer().Selected)
                {
                    ClickElement(roleManagement.DefaultUniversalViewer());
                }
                if (Step6_1 && Step6_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step6_1 = " + Step6_1);
                    Logger.Instance.InfoLog("Step6_2 = " + Step6_2);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 7:
                roleManagement.ClickSaveEditRole();
                userPreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userPreferences.BluringViewerRadioBtn().Click();
                userPreferences.CloseUserPreferences();
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 8:
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(20);
                if(GetElement("cssselector",BluRingViewer.btn_bluringviewer).Displayed)
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

                //Step 9:
                studies.SearchStudy(AccessionNo: Accession, Datasource: DS);
                studies.SelectStudy1("Accession", Accession, dblclick: true);
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitForPriorsToLoad();
                if (bluViewer.studyPanel().Displayed)
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

                //Step 10:
                bluViewer.CloseBluRingViewer();
                studies.SearchStudy(AccessionNo: Accession, Datasource: DS);
                studies.SelectStudy("Accession", Accession);
                bluViewer = BluRingViewer.LaunchBluRingViewer();
                if (bluViewer.studyPanel().Displayed)
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

                //Step 11:
                bluViewer.CloseBluRingViewer();
                studies.SearchStudy(AccessionNo: Accession, Datasource: DS);
                studies.SelectStudy("Accession", Accession);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                if (viewer.ViewStudy())
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

                //Step 12:
                viewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession, Datasource: DS);
                studies.SelectStudy1("Accession", Accession, dblclick: true);
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitForPriorsToLoad();
                if (bluViewer.studyPanel().Displayed)
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

                //Step 13:
                bluViewer.CloseBluRingViewer();
                roleManagement = (RoleManagement)login.Navigate("RoleManagement");
                roleManagement.SearchRole(Config.adminRoleName, Config.adminGroupName);
                roleManagement.SelectRole(Config.adminRoleName);
                roleManagement.ClickEditRole();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                if (!roleManagement.DefaultEnterpriseViewer().Selected)
                {
                    ClickElement(roleManagement.DefaultEnterpriseViewer());
                }
                roleManagement.ClickSaveEditRole();
                userPreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userPreferences.HTML4RadioBtn().Click();
                userPreferences.CloseUserPreferences();
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 14:
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession, Datasource: DS);
                studies.SelectStudy1("Accession", Accession, dblclick: true);
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                if (viewer.ViewStudy())
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

                //Step 15:
                viewer.CloseStudy();
                domainManagement = (DomainManagement)login.Navigate("DomainManagement");
                domainManagement.SearchDomain(Config.adminGroupName);
                domainManagement.SelectDomain(Config.adminGroupName);
                domainManagement.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                if (domainManagement.Domain_UniversalViewer().Selected)
                {
                    ClickElement(domainManagement.Domain_UniversalViewer());
                }
                domainManagement.ClickSaveEditDomain();
                domainManagement.SearchDomain(Config.adminGroupName);
                domainManagement.SelectDomain(Config.adminGroupName);
                domainManagement.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                bool Step15_1 = domainManagement.DefaultUniversalViewer().Displayed == false;
                bool Step15_2 = domainManagement.DefaultEnterpriseViewer().Displayed == false;
                domainManagement.ClickSaveEditDomain();
                roleManagement = (RoleManagement)login.Navigate("RoleManagement");
                roleManagement.SearchRole(Config.adminRoleName, Config.adminGroupName);
                roleManagement.SelectRole(Config.adminRoleName);
                roleManagement.ClickEditRole();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                bool Step15_3 = Driver.FindElements(roleManagement.defaultEnterpriseViewer()).Count == 0;
                bool Step15_4 = Driver.FindElements(roleManagement.defaultUniversalViewer()).Count == 0;
                bool Step15_5 = Driver.FindElements(roleManagement.allowUniversalViewer()).Count == 0;
                roleManagement.ClickSaveEditRole();
                userPreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                bool Step15_6 = userPreferences.HTML4RadioBtn().Displayed == false;
                bool Step15_7 = userPreferences.BluringViewerRadioBtn().Displayed == false;
                userPreferences.CloseUserPreferences();
                if(Step15_1 && Step15_2 && Step15_3 && Step15_4 && Step15_5 && Step15_6 && Step15_7)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step15_1 = " + Step15_1);
                    Logger.Instance.InfoLog("Step15_2 = " + Step15_2);
                    Logger.Instance.InfoLog("Step15_3 = " + Step15_3);
                    Logger.Instance.InfoLog("Step15_4 = " + Step15_4);
                    Logger.Instance.InfoLog("Step15_5 = " + Step15_5);
                    Logger.Instance.InfoLog("Step15_6 = " + Step15_6);
                    Logger.Instance.InfoLog("Step15_7 = " + Step15_7);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 16:
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession, Datasource: DS);
                studies.SelectStudy1("Accession", Accession, dblclick: true);
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                if (viewer.ViewStudy())
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

                //Step 17:
                viewer.CloseStudy();
                domainManagement = (DomainManagement)login.Navigate("DomainManagement");
                domainManagement.SearchDomain(Config.adminGroupName);
                domainManagement.SelectDomain(Config.adminGroupName);
                domainManagement.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                if (!domainManagement.Domain_UniversalViewer().Selected)
                {
                    ClickElement(domainManagement.Domain_UniversalViewer());
                }
                bool Step17_1 = domainManagement.DefaultUniversalViewer().Displayed;
                bool Step17_2 = domainManagement.DefaultEnterpriseViewer().Displayed;
                if (Step17_1 && Step17_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step17_1 = " + Step17_1);
                    Logger.Instance.InfoLog("Step17_2 = " + Step17_2);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 18:
                domainManagement.ClickSaveEditDomain();
                roleManagement = (RoleManagement)login.Navigate("RoleManagement");
                roleManagement.SearchRole(Config.adminRoleName, Config.adminGroupName);
                roleManagement.SelectRole(Config.adminRoleName);
                roleManagement.ClickEditRole();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                if (roleManagement.AllowUniversalViewer().Selected)
                {
                    ClickElement(roleManagement.AllowUniversalViewer());
                }
                bool Step18_1 = roleManagement.DefaultUniversalViewer().Displayed == false;
                bool Step18_2 = roleManagement.DefaultEnterpriseViewer().Displayed == false;
                if (Step18_1 && Step18_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step18_1 = " + Step18_1);
                    Logger.Instance.InfoLog("Step18_2 = " + Step18_2);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 19:
                roleManagement.ClickSaveEditRole();
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(20);
                if (GetElement("cssselector", BluRingViewer.btn_bluringviewer).Displayed == false)
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

                //Step 20:
                login.Logout();
                serviceTool.LaunchServiceTool();
                serviceTool.EnableHTML5(false, false);
                serviceTool.CloseServiceTool();
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 21:
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainManagement = (DomainManagement)login.Navigate("DomainManagement");
                domainManagement.SearchDomain(Config.adminGroupName);
                domainManagement.SelectDomain(Config.adminGroupName);
                domainManagement.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                bool Step21_1 = Driver.FindElements(domainManagement.domain_UniversalViewer()).Count == 0;
                domainManagement.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                domainManagement.ClickNewDomainBtn();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                bool Step21_2 = Driver.FindElements(domainManagement.domain_UniversalViewer()).Count == 0;
                domainManagement.CloseDomainManagement();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                if (Step21_1 && Step21_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step21_1 = " + Step21_1);
                    Logger.Instance.InfoLog("Step21_2 = " + Step21_2);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 22:
                userPreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                bool Step22_1 = userPreferences.HTML4RadioBtn().Displayed == false;
                bool Step22_2 = userPreferences.BluringViewerRadioBtn().Displayed == false;
                userPreferences.CloseUserPreferences();
                if (Step22_1 && Step22_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step22_1 = " + Step22_1);
                    Logger.Instance.InfoLog("Step22_2 = " + Step22_2);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 23:
                roleManagement = (RoleManagement)login.Navigate("RoleManagement");
                roleManagement.SearchRole(Config.adminRoleName, Config.adminGroupName);
                roleManagement.SelectRole(Config.adminRoleName);
                roleManagement.ClickEditRole();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                if(Driver.FindElements(roleManagement.allowUniversalViewer()).Count == 0)
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

                //Return Result 
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Results
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
                login.Logout();
                serviceTool.LaunchServiceTool();
                serviceTool.EnableHTML5(true, true);
                serviceTool.CloseServiceTool();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainManagement = (DomainManagement)login.Navigate("DomainManagement");
                domainManagement.SearchDomain(Config.adminGroupName);
                domainManagement.SelectDomain(Config.adminGroupName);
                domainManagement.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                if (!domainManagement.Domain_UniversalViewer().Selected)
                {
                    ClickElement(domainManagement.Domain_UniversalViewer());
                }
                if (!domainManagement.DefaultUniversalViewer().Selected)
                {
                    ClickElement(domainManagement.DefaultUniversalViewer());
                }
                domainManagement.ClickSaveEditDomain();
                roleManagement = (RoleManagement)login.Navigate("RoleManagement");
                roleManagement.SearchRole(Config.adminRoleName, Config.adminGroupName);
                roleManagement.SelectRole(Config.adminRoleName);
                roleManagement.ClickEditRole();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                if (!roleManagement.AllowUniversalViewer().Selected)
                {
                    ClickElement(roleManagement.AllowUniversalViewer());
                }
                if (!roleManagement.DefaultUniversalViewer().Selected)
                {
                    ClickElement(roleManagement.DefaultUniversalViewer());
                }
                roleManagement.ClickSaveEditRole();
                userPreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userPreferences.BluringViewerRadioBtn().Click();
                userPreferences.CloseUserPreferences();
                login.Logout();
            }
        }
    }
}
