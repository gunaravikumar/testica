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
using Selenium.Scripts.Pages.MergeServiceTool;
using TestStack.White.Configuration;
using TestStack.White.Factory;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.TableItems;
using TestStack.White.InputDevices;
using TestStack.White.WindowsAPI;
using Application = TestStack.White.Application;
using System.Diagnostics;
using System.Xml.Serialization;
using OpenQA.Selenium.Remote;
using Selenium.Scripts.Pages.eHR;

namespace Selenium.Scripts.Tests
{
    class DirectDicomQuery : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public ExamImporter ei { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public EHR ehr { get; set; }
        public Web_Uploader webuploader { get; set; }
        public RanorexObjects rnxobject { get; set; }
        DirectoryInfo CurrentDir = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());
        string FolderPath = "";

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public DirectDicomQuery(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            FolderPath = Config.downloadpath;//CurrentDir.Parent.Parent.FullName + "\\Downloads\\";
            ei = new ExamImporter();
            mpaclogin = new MpacLogin();
            wpfobject = new WpfObjects();
            rnxobject = new RanorexObjects();
            webuploader = new Web_Uploader();
            ehr = new EHR();
        }

        /// <summary>
        /// Direct Dicom Query - 1.0 Study List
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161504(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            Taskbar taskbar = null;
            Studies studies = null;
            BluRingViewer bluering = new BluRingViewer();
            StudyViewer StudyVw = new StudyViewer();
            Viewer viewer = new Viewer();
            DomainManagement domainmanagement;
            RoleManagement rolemanagement;
            UserManagement usermanagement;
            TestCaseResult result;
            UserPreferences userpref = new UserPreferences();
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;
                String RoleName = Config.adminRoleName;
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String PatientName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String StudyID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyID");
                String StudyDate = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDate");
                String PatientDOB = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientDOB");
                String DSList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Datasource");

                String[] lastName = LastName.Split(':');
                String[] PatName = PatientName.Split(':');
                String[] patientID = PatientID.Split(':');
                String[] studyID = StudyID.Split(':');
                String[] Datasources = DSList.Split(':');

                //Precondition - Create Domain, role and user within the domain
                login.LoginIConnect(username, password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                //Domain variables
                var domainattr = domainmanagement.CreateDomainAttr();

                //Domain
                String Testdomain = domainattr[DomainManagement.DomainAttr.DomainName];
                String TestdomainAdmin = domainattr[DomainManagement.DomainAttr.UserID];
                String TestdomainAdmin_Pwd = domainattr[DomainManagement.DomainAttr.Password];
                String Testrole1 = "161504_Role1" + new Random().Next(1, 1000);
                String Testuser1 = "161504_User1" + new Random().Next(1, 1000);

                domainmanagement.CreateDomain(domainattr, datasources: new String[] { Datasources[0], Datasources[1], Datasources[2] });
                domainmanagement.SearchDomain(Testdomain);
                domainmanagement.SelectDomain(Testdomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.ModifyStudySearchFields("show");
                domainmanagement.ClickSaveEditDomain();

                //Create Role
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.CreateRole(Testdomain, Testrole1, "any");
                rolemanagement.SearchRole(Testrole1);
                rolemanagement.SelectRole(Testrole1);
                rolemanagement.ClickEditRole();
                rolemanagement.UnCheckCheckbox(rolemanagement.StudySearchFieldUseDomainSetting_CB());
                rolemanagement.ModifyStudySearchFields("show");
                rolemanagement.ClickSaveEditRole();

                //Crete User
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(Testuser1, Testdomain, Testrole1);
                login.Logout();

                //Step-1: Log in to iConnect Access as user created in the pre-conditions
                login.LoginIConnect(Testuser1, Testuser1);
                ExecutedSteps++;

                // PreCondition
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.BluringViewerRadioBtn().Click();
                userpref.CloseUserPreferences();

                //Step-2: Click search
                studies = (Studies)login.Navigate("Studies");
                studies.ClearFields();
                studies.SearchStudy("lastname", "");
                //Validation for checking all studies via ds
                studies.ChooseColumns(new string[] { "Data Source" });
                string[] step2_DS = studies.GetStudyDetails("Data Source");
                string[] MPACS_DS = new string[] { Datasources[0], Datasources[1] };
                bool step2 = (step2_DS == null || step2_DS.Length == 0) ? false : MPACS_DS.Where(z => step2_DS.Any(q => q.ToLower().Contains(z.ToLower()))).Count() == MPACS_DS.Length && step2_DS.Where(z => MPACS_DS.Any(q => z.ToLower().Contains(q.ToLower()))).Count() == step2_DS.Length;
                if (step2)
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


                //Step-3: Search Cardiac, MR
                studies.ClearFields();
                studies.SearchStudy(LastName: PatName[1].Split(',')[0].Trim(),FirstName: PatName[1].Split(',')[1].Trim());
                PageLoadWait.WaitForLoadingMessage(30);
                studies.ChooseColumns(new string[] { "Patient Name" });
                string[] step3 = studies.GetStudyDetails("Patient Name");
                bool step3_res = (step3 == null || step3.Length == 0) ? false : step3.Where(q => q.ToLower().Contains(PatName[1].ToLower())).Count() == step3.Length;
                if (step3_res)
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


                //Step-4: Search Chest, CT
                studies.ClearFields();
                studies.SearchStudy(LastName: PatName[1].Split(',')[0].Trim(), FirstName: PatName[1].Split(',')[1].Trim());
                PageLoadWait.WaitForLoadingMessage(30);
                studies.ChooseColumns(new string[] { "Patient Name", "Patient ID", "Study Date" });
                string[] step4_patientname = studies.GetStudyDetails("Patient Name");
                bool step4 = step4_patientname.All(res => res.Split(',')[0].ToLower().Trim().Contains(PatName[1].Split(',')[0].ToLower().Trim())) && step4_patientname.All(res => res.Split(',')[1].ToLower().Trim().Contains(PatName[1].Split(',')[1].ToLower().Trim()));
                if (step4)
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

                //Step-5: Search Gamage, Mary
                studies.ClearFields();
                studies.SearchStudy(LastName: PatName[2].Split(',')[0].Trim(), FirstName: PatName[2].Split(',')[1].Trim());
                PageLoadWait.WaitForLoadingMessage(30);
                studies.ChooseColumns(new string[] { "Patient DOB" });
                string[] step5_patientname = studies.GetStudyDetails("Patient Name");
                bool step5 = step5_patientname.All(res => res.ToLower().Trim().Equals(PatName[2].ToLower().Trim()));
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

                //Step-6: Enter Study ID
                studies.ClearFields();
                studies.SearchStudy(studyID: studyID[0]);
                PageLoadWait.WaitForLoadingMessage(30);
                string[] step6_patientname = studies.GetStudyDetails("Patient Name");
                bool step6 = step6_patientname.All(res=>res.ToLower().Contains(PatName[3].ToLower()));
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

                //Step-7: Enter 4380 in search field
                studies.ClearFields();
                studies.SearchStudy(studyID: studyID[1]);
                PageLoadWait.WaitForLoadingMessage(30);
                studies.ChooseColumns(new string[] { "Study ID" });
                string[] step7_StudyID = studies.GetStudyDetails("Study ID");
                bool step7 = step7_StudyID.All(res => res.ToLower().Contains(studyID[1].ToLower()));
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

                //Step-8: Clear fields and select data source as ''.
                studies.ClearFields();
                studies.SearchStudy(Datasource: Datasources[0]);
                PageLoadWait.WaitForLoadingMessage(30);
                studies.ChooseColumns(new string[] { "Data Source" });
                string[] step8_DS = studies.GetStudyDetails("Data Source");
                bool step8 = step8_DS.All(res=>res.ToLower().Contains(Datasources[0].ToLower()));
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

                //Step-9: Select any Data Source which has more than 200 studies and Click Search.
                studies.ClearFields();
                studies.SearchStudy(LastName: "*", Datasource: Datasources[2]);
                PageLoadWait.WaitForLoadingMessage(50);
                string[] step9_DS = studies.GetStudyDetails("Data Source");
                bool step9 = step9_DS.All(res=>res.ToLower().Contains(Datasources[2].ToLower()) && GetElement(SelectorType.CssSelector, Studies.SearchPageViewText).Text.Equals("View 1 - 200 of 200"));
                if (step9)
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

                //Step-10: Add * in last name select data source as ''.
                studies.ClearFields();
                studies.SearchStudy(LastName: "*", Datasource: Datasources[1]);
                PageLoadWait.WaitForLoadingMessage(30);
                string[] step10_DS = studies.GetStudyDetails("Data Source");
                bool step10 = step10_DS.All(res=>res.ToLower().Contains(Datasources[1].ToLower()));
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

                //Step-11: Set DS as all and search
                studies.ClearFields();
                studies.SearchStudy( Datasource: "All");
                PageLoadWait.WaitForLoadingMessage(30);
                studies.ChooseColumns(new string[] { "Data Source" });
                string[] step11_DS = studies.GetStudyDetails("Data Source");
                String[] DatasourcesTemp = new string[] { Datasources[0], Datasources[1] };
                bool step11 = (step11_DS == null || step11_DS.Length == 0) ? false : DatasourcesTemp.Where(z => step11_DS.Any(q => q.ToLower().Contains(z.ToLower()))).Count() == DatasourcesTemp.Length && step11_DS.Where(z => DatasourcesTemp.Any(q => z.ToLower().Contains(q.ToLower()))).Count() == step11_DS.Length;
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

                //Step-12: Load Cardiac MR Study
                studies.SearchStudy(LastName: PatName[4].Split(',')[0]);
                PageLoadWait.WaitForLoadingMessage(30);
                studies.SelectStudy("Patient ID", patientID[2]);
                if (Config.isEnterpriseViewer.ToLower() == "y")
                    bluering = BluRingViewer.LaunchBluRingViewer();
                else
                   StudyVw = studies.LaunchStudy();

                PageLoadWait.WaitForFrameLoad(15);
                Thread.Sleep(4000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step12 = false;
                if (Config.isEnterpriseViewer.ToLower() == "y")
                  step12 = studies.CompareImage(result.steps[ExecutedSteps], bluering.studyPanel(1));
                else
                    step12 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel(1));

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


                //Step-13: Close study
                if (Config.isEnterpriseViewer.ToLower() == "y")
                    bluering.CloseBluRingViewer();
                else
                    studies.CloseStudy();

                ExecutedSteps++;

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout 
                login.Logout();

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

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Direct Dicom Query - 2.0 Loading
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161505(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            Studies studies = null;
            StudyViewer StudyVw;
            BluRingViewer viewer;
            DomainManagement domainmanagement;
            RoleManagement rolemanagement;
            UserManagement usermanagement;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;
                String RoleName = Config.adminRoleName;
                String PatientName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName");
                String StudyID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyID");
                String Description = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Description");
                String DSList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Datasource");

                String[] Datasources = DSList.Split(':');
                String[] PatName = PatientName.Split(':');
                String[] studyID = StudyID.Split(':');

                //Precondition - Create Domain, role and user within the domain
                login.LoginIConnect(username, password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                //Domain variables
                var domainattr = domainmanagement.CreateDomainAttr();

                //Domain
                String Testdomain = domainattr[DomainManagement.DomainAttr.DomainName];
                String TestdomainAdmin = domainattr[DomainManagement.DomainAttr.UserID];
                String TestdomainAdmin_Pwd = domainattr[DomainManagement.DomainAttr.Password];
                String Testrole1 = "161505_Role1" + new Random().Next(1, 1000);
                String Testuser1 = "161505_User1" + new Random().Next(1, 1000);

                domainmanagement.CreateDomain(domainattr, Datasources, AddAllStudySearchFields: true);
                //Workaround if all fields are not added
                domainmanagement.SearchDomain(Testdomain);
                domainmanagement.SelectDomain(Testdomain);
                domainmanagement.EditDomainButton().Click();
                Driver.SwitchTo().DefaultContent();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForPageLoad(10);
                domainmanagement.VisibleAllStudySearchField();
                PageLoadWait.WaitForFrameLoad(10);
                domainmanagement.SaveDomainButtoninEditPage().Click();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //Create Role
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.CreateRole(Testdomain, Testrole1, "any");
                rolemanagement.SelectDomainfromDropDown(Testdomain);
                rolemanagement.SearchRole(Testrole1);
                rolemanagement.SelectRole(Testrole1);
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckbox(rolemanagement.StudySearchFieldUseDomainSetting_CB());
                rolemanagement.ClickSaveEditRole();

                //Crete User
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(Testuser1, Testdomain, Testrole1);
                login.Logout();

                //Step-1: Log in to iConnect Access as user created in the pre-conditions
                login.LoginIConnect(Testuser1, Testuser1);
                ExecutedSteps++;

                //Step-2: Load the study Nuclear Gladys, 1-2 Phase Bone.
                studies = (Studies)login.Navigate("Studies");
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SearchStudy(LastName: PatName[0]);
                studies.SelectStudy("Study ID", studyID[0]);
                bool step2 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("n"))
                {
                    StudyVw = studies.LaunchStudy();
                    PageLoadWait.WaitForFrameLoad(15);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    step2 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                    studies.CloseStudy();
                }
                else
                {
                    viewer = BluRingViewer.LaunchBluRingViewer();
                    PageLoadWait.WaitForFrameLoad(20);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    step2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                    viewer.CloseBluRingViewer();
                }
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


                //Step-3: Load the study RAM A/L POHUMAL, 200106120952.
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SearchStudy(LastName: PatName[1]);
                studies.SelectStudy("Study ID", studyID[3]);
                bool step3 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("n"))
                {
                    StudyVw = studies.LaunchStudy();
                    PageLoadWait.WaitForFrameLoad(20);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    step3 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                    studies.CloseStudy();
                }
                else
                {
                    viewer = BluRingViewer.LaunchBluRingViewer();
                    PageLoadWait.WaitForFrameLoad(20);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    step3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                    viewer.CloseBluRingViewer();
                }
                if (step3)
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

                //Step-4: Load the study Cardiac MR, 4380.
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SearchStudy(LastName: PatName[2], studyID: studyID[1]);
                studies.SelectStudy("Study ID", studyID[1]);
                bool step4 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("n"))
                {
                    StudyVw = studies.LaunchStudy();
                    PageLoadWait.WaitForFrameLoad(15);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    step4 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                    studies.CloseStudy();
                }
                else
                {
                    viewer = BluRingViewer.LaunchBluRingViewer();
                    PageLoadWait.WaitForFrameLoad(20);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    step4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                    viewer.CloseBluRingViewer();
                }
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

                //Step-5: Load the study Oesophagus Barium.
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SearchStudy(LastName: PatName[3]);
                studies.SelectStudy("Description", Description);
                bool step5 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("n"))
                {
                    StudyVw = studies.LaunchStudy();
                    PageLoadWait.WaitForFrameLoad(15);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    step5 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                    studies.CloseStudy();
                }
                else
                {
                    viewer = BluRingViewer.LaunchBluRingViewer();
                    PageLoadWait.WaitForFrameLoad(20);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    step5 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                    viewer.CloseBluRingViewer();
                }
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

                //Step-6: Load the study Abdomen CT, 24813.
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SearchStudy(LastName: PatName[4], studyID: studyID[2]);
                studies.SelectStudy("Study ID", studyID[2]);
                bool step6 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("n"))
                {
                    StudyVw = studies.LaunchStudy();
                    PageLoadWait.WaitForFrameLoad(15);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    step6 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                    studies.CloseStudy();
                }
                else
                {
                    viewer = BluRingViewer.LaunchBluRingViewer();
                    PageLoadWait.WaitForFrameLoad(20);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    step6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                    viewer.CloseBluRingViewer();
                }
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

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout 
                login.Logout();

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

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Direct Dicom Query - 3.0 Attachment and GSPS
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_66264(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            ServiceTool servicetool = new ServiceTool();
            Taskbar taskbar = null;
            Studies studies = null;
            StudyViewer StudyVw;
            Viewer viewer = new Viewer();
            DomainManagement domainmanagement;
            RoleManagement rolemanagement;
            UserManagement usermanagement;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;
                String RoleName = Config.adminRoleName;
                String PatientName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName");
                String StudyID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyID");
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String Datasource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Datasource");
                String FilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FilePath");
                String Filename = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Filename");
                
                String[] PatName = PatientName.Split(':');

                //Pre-Conditions: Enable Attachments in Service tool
                //Preconditions: Service tool updates
                taskbar = new Taskbar();
                taskbar.Hide();
                servicetool.LaunchServiceTool();
                servicetool.SetEnableFeaturesGeneral();
                servicetool.EnableStudyAttachements();
                servicetool.RestartService();
                servicetool.WaitWhileBusy();
                servicetool.CloseConfigTool();
                taskbar.Show();

                //Enable Attachment in Domain
                login.LoginIConnect(username, password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DomainName);
                domainmanagement.SelectDomain(DomainName);
                domainmanagement.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                domainmanagement.SetCheckBoxInEditDomain("attachment", 0);
                domainmanagement.SetCheckBoxInEditDomain("attachmentupload", 0);
                domainmanagement.ClickSaveDomain();

                //Step-1: Configure the attachment feature to store attachments on DS2 - Feature enabled for data source once and the same configuration will be used everytime. Hence no need to automate
                ExecutedSteps++;

                //Step-2: Load the study LUND TAMMY J, A1035510
                login.LoginIConnect(username, password);
                studies = (Studies)login.Navigate("Studies");
                studies.ClearFields();
                studies.ChooseColumns(new string[] { "Patient Name", "Accession" });
                studies.SearchStudy(LastName: PatName[0], AccessionNo: AccessionID, Datasource: Datasource);
                PageLoadWait.WaitForLoadingMessage(30);
                studies.SelectStudy("Accession", AccessionID);
                StudyVw = studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(15);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step2 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
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

                //Step-3: Attach a file to the study.
                StudyVw.NavigateToHistoryPanel();
                StudyVw.NavigateTabInHistoryPanel("Attachment");
                PageLoadWait.WaitForFrameLoad(20);
                bool step3 = StudyVw.UploadAttachment(FilePath, 20);
                if (step3)
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

                //Step-4: Draw an annotation on one of the images, and select Save Series - Not Automated as Saving not done through automation
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-5: Reload same study.
                studies.CloseStudy();
                studies.ClearFields();
                studies.ChooseColumns(new string[] { "Patient Name", "Accession" });
                studies.SearchStudy(LastName: PatName[0], AccessionNo: AccessionID, Datasource: Datasource);
                PageLoadWait.WaitForLoadingMessage(30);
                studies.SelectStudy("Accession", AccessionID);
                StudyVw = studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(15);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step5_1 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                StudyVw.NavigateToHistoryPanel();
                StudyVw.NavigateTabInHistoryPanel("Attachment");
                bool step5_2 = false;
                PageLoadWait.WaitForFrameLoad(20);
                Dictionary<string, string> Filerow = StudyVw.StudyViewerListMatchingRow("Name", Path.GetFileName(FilePath), "patienthistory", "attachment");
                if (Filerow != null)
                {
                    step5_2 = true;
                }
                if (step5_1 && step5_2)
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

                //Step-6: Download attachment
                bool step6 = StudyVw.DownloadAttachment(Filename);
                if (step6 && File.Exists(FolderPath + Path.DirectorySeparatorChar + Filename))
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

                //Step-7: Load the study Cardiac MR, 4380.
                studies.ClearFields();
                studies.ChooseColumns(new string[] { "Patient Name", "Study ID" });
                studies.SearchStudy(LastName: PatName[1].Split(',')[0].Trim(), FirstName: PatName[1].Split(',')[1].Trim(), studyID: StudyID, Datasource: Datasource);
                PageLoadWait.WaitForLoadingMessage(30);
                studies.SelectStudy("Study ID", StudyID);
                StudyVw = studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(15);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step7 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step7)
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

                //Step-8: Attach a file to the study.
                StudyVw.NavigateToHistoryPanel();
                StudyVw.NavigateTabInHistoryPanel("Attachment");
                PageLoadWait.WaitForFrameLoad(20);
                bool step8 = StudyVw.UploadAttachment(FilePath, 20);
                if (step8)
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

                //Step-9: Draw an annotation on one of the images, and select Save Series - Not Automated as Saving not done through automation
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-10: Reload same study.
                studies.CloseStudy();
                studies.ClearFields();
                studies.ChooseColumns(new string[] { "Patient Name", "Study ID" });
                studies.SearchStudy(LastName: PatName[1].Split(',')[0].Trim(), FirstName: PatName[1].Split(',')[1].Trim(), studyID: StudyID, Datasource: Datasource);
                PageLoadWait.WaitForLoadingMessage(30);
                studies.SelectStudy("Study ID", StudyID);
                StudyVw = studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(15);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step10_1 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                bool step10_2 = false;
                StudyVw.NavigateToHistoryPanel();
                StudyVw.NavigateTabInHistoryPanel("Attachment");
                PageLoadWait.WaitForFrameLoad(20);
                Filerow = StudyVw.StudyViewerListMatchingRow("Name", Path.GetFileName(FilePath), "patienthistory", "attachment");
                if (Filerow != null)
                {
                    step10_2 = true;
                }
                if (step10_1 && step10_2)
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

                //Step-11: Download attachment
                bool step11 = StudyVw.DownloadAttachment(Filename);
                if (step11 && File.Exists(FolderPath + Path.DirectorySeparatorChar + Filename))
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
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout 
                login.Logout();

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

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Cleanup script to close browser
        /// </summary>
        public void Test_Cleanup()
        {
            login.CloseBrowser();
        }

    }
}
