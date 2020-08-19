using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Reusable.Generic;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MergeServiceTool;
using static Selenium.Scripts.DriverScript.TestRunner;

namespace Selenium.Scripts.Tests
{
    class ExternalApplicationDownloader : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
		string updatedateandtimebatchfile = string.Empty;
		string CurrentDate = string.Empty;
		string CurrentTime = string.Empty;

		/// <summary>
		/// Constructor - Test Suite
		/// </summary>
		/// <param name="classname"></param>
		public ExternalApplicationDownloader(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
			updatedateandtimebatchfile = string.Concat(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar, "OtherFiles\\UpdateDatetime.bat");
			CurrentDate = DateTime.Now.ToString("MM/dd/yyyy");
			CurrentTime = DateTime.Now.ToString("hh:mm:ss tt");
		}

        Studies studies = new Studies();
        ServiceTool servicetool = new ServiceTool();
        WpfObjects wpfobject = new WpfObjects();
        DomainManagement domainmanagement;
        RoleManagement rolemanagement;
        UserManagement usermanagement;
        Inbounds inbounds = null;
        Outbounds outbounds = null;
        StudyViewer viewer;
        Maintenance maintenance;

        IList<bool> AuditLogList = new List<bool>();

		String D1 = null;
		String D2 = null;
		String ROLE1 = null;
		String ROLE2 = null;

		String G1 = null;
		String G2 = null;
		String G3 = null;
		String G4 = null;
		String CG1 = null;

		String U1 = null;
		String U4 = null;
		String U5 = null;
		String U6 = null;
		String U7 = null;
		String U9 = null;

		String Doctor = null;
		String Nurse = null;
		String FrontDesk = null;
		String FrontDesk1 = null;

		new String FirstName = null;
		new String LastName = null;

		String d1 = null;

		String D1_LDAP = "D1_LDAP_" + new Random().Next(1, 1000);
        String D2_LDAP = "D2_LDAP_" + new Random().Next(1, 1000);
        String ROLE1_LDAP = "ROLE1_LDAP_" + new Random().Next(1, 1000);
        String ROLE2_LDAP = "ROLE2_LDAP_" + new Random().Next(1, 1000);

        String G1_LDAP = "G1_LDAP_" + new Random().Next(1, 1000);
        String G2_LDAP = "G2_LDAP_" + new Random().Next(1, 1000);
        String G3_LDAP = "G3_LDAP_" + new Random().Next(1, 1000);
        String G4_LDAP = "G4_LDAP_" + new Random().Next(1, 1000);
        String CG1_LDAP = "CG1_LDAP_" + new Random().Next(1, 1000);

        String Doctor_LDAP = "Doctor_LDAP_" + new Random().Next(1, 1000);
        String Nurse_LDAP = "Nurse_LDAP_" + new Random().Next(1, 1000);
        String FrontDesk_LDAP = "FrontDesk_LDAP_" + new Random().Next(1, 1000);
        String FrontDesk1_LDAP = "FrontDesk1_LDAP_" + new Random().Next(1, 1000);

        String U1_LDAP = "U1_LDAP_" + new Random().Next(1, 1000);
        String U4_LDAP = "U4_LDAP_" + new Random().Next(1, 1000);
        String U5_LDAP = "U5_LDAP_" + new Random().Next(1, 1000);
        String U6_LDAP = "U6_LDAP_" + new Random().Next(1, 1000);
        String U7_LDAP = "U7_LDAP_" + new Random().Next(1, 1000);
        String U9_LDAP = "U9_LDAP_" + new Random().Next(1, 1000);

        String D1ROLE1 = "D1ROLE1_LDAP_" + new Random().Next(1, 1000);
        String D2ROLE2 = "D2ROLE2_LDAP_" + new Random().Next(1, 1000);

        String d1_LDAP = "d1_LDAP_" + new Random().Next(1, 1000);

        String FirstName_LDAP = "CARRIE";
        String LastName_LDAP = "BERLIN";

		/// <summary>
		/// Downloader - Initial Setup (Local user database)
		/// </summary>
		/// <param name="testid"></param>
		/// <param name="teststeps"></param>
		/// <param name="stepcount"></param>
		/// <returns></returns>
		//    public TestCaseResult Test_27571(String testid, String teststeps, int stepcount)
		//    {
		//        //Declare and initialize variables  
		//        TestCaseResult result;
		//        result = new TestCaseResult(stepcount);

		//        //Set up Validation Steps
		//        result.SetTestStepDescription(teststeps);
		//        int ExecutedSteps = -1;
		//        //int eventcount_before = 0;
		//        //int eventcount_after = 0;
		//        //String eventtype = "Security Alert/Object Security Attributes Changed";

		//        try
		//        {
		//            //Fetch required Test data    
		//            String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");

		//            //Step 1 - Pre-conditions
		//            ExecutedSteps++;

		//            //Step 2 - In the Service Tool*^>^*Enable Features tab Enable Study Sharing , Enable Study Transfer, Enable Downloader and Uploader.
		//            servicetool.LaunchServiceTool();
		//            servicetool.NavigateToEnableFeatures();
		//            wpfobject.WaitTillLoad();
		//            servicetool.ModifyEnableFeatures();
		//            wpfobject.WaitTillLoad();
		//            servicetool.EnableStudySharing();
		//            servicetool.EnableDataTransfer();
		//            servicetool.EnableDataDownloader();
		//            servicetool.ApplyEnableFeatures();
		//            wpfobject.WaitTillLoad();
		//            wpfobject.ClickOkPopUp();
		//            wpfobject.WaitTillLoad();

		//            servicetool.EnableUpload();
		//            servicetool.RestartIISandWindowsServices();
		//            ExecutedSteps++;

		//            //Step 3 - In the Service Tool*^>^*Enable Features*^>^*Transfer Service --"Enable Transfer Service"option by clicking on the box.
		//            servicetool.SetEnableFeaturesTransferService();
		//            servicetool.ModifyEnableFeatures();
		//            wpfobject.WaitTillLoad();
		//            servicetool.EnableTransferService();
		//            ExecutedSteps++;

		//            //Step 4 - In the Service Tool*^>^*Enable Features*^>^*Transfer Service -- at the bottom of the window, select the package tab and change the expire interval to 5 min, and the Package maximum to 10000kB
		//            servicetool.ModifyPackagerDetails("5");
		//            ExecutedSteps++;

		//            //Step 5 - Setup Email Notification in the service tool
		//            servicetool.NavigateToTab("E-mail Notification");
		//            servicetool.NavigateSubTab("General");
		////servicetool.SetEmailNotification();
		//servicetool.SetEmailNotificationForPOP();

		//servicetool.RestartIISandWindowsServices();
		//            servicetool.CloseServiceTool();
		//            ExecutedSteps++;

		//            //Step 6 - Log in iConnect Access desktop version as administrator
		//            login.DriverGoTo(login.url);
		//            login.LoginIConnect(Config.adminUserName, Config.adminPassword);
		//            domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
		//            ExecutedSteps++;

		//            //Step 7 - Select the New Domain button                
		//            ExecutedSteps++;

		//            //Step 8 - Create New Domain
		//            domainmanagement.CreateDomain(domainName: D1, roleName: ROLE1, datasources: null);
		//            domainmanagement.SetCheckBoxInEditDomain("datatransfer", 0);
		//            domainmanagement.SetCheckBoxInEditDomain("grant", 0);
		//            domainmanagement.SetCheckBoxInEditDomain("datadownload", 0);
		//            domainmanagement.ClickSaveDomain();
		//            ExecutedSteps++;

		//            //Step 9 - Repeat and create another new Domains D2, ROLE2
		//            domainmanagement.CreateDomain(domainName: D2, roleName: ROLE2, datasources: null);
		//            domainmanagement.SetCheckBoxInEditDomain("datatransfer", 0);
		//            domainmanagement.SetCheckBoxInEditDomain("grant", 0);
		//            domainmanagement.SetCheckBoxInEditDomain("datadownload", 0);
		//            domainmanagement.ClickSaveDomain();
		//            if (domainmanagement.IsDomainExist(D1) && domainmanagement.IsDomainExist(D2))
		//            {
		//                result.steps[++ExecutedSteps].status = "Pass";
		//                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
		//            }
		//            else
		//            {
		//                result.steps[++ExecutedSteps].status = "Fail";
		//                Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
		//                result.steps[ExecutedSteps].SetLogs();
		//            }                

		//            //Step 10 - Create Groups and Subgroups
		//            usermanagement = (UserManagement)login.Navigate("UserManagement");
		//            ExecutedSteps++;

		//            //Step 11 - Select Domain D1 from the drop down list.
		//            usermanagement.SelectDomainFromDropdownList(D1);
		//            ExecutedSteps++;

		//            //Step 12 - Select the "New Group" Button
		//            usermanagement.ClickButtonInUser("newgroup");
		//            ExecutedSteps++;

		//            //Step 13 - Create group G1
		//            PageLoadWait.WaitForFrameLoad(10);
		//            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#createGroupDiv")));
		//            if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
		//            {
		//                BasePage.wait.Until<Boolean>(d => d.FindElement(By.CssSelector("div#GroupProcessing")).GetAttribute("style").Contains("DISPLAY: none;"));
		//            }
		//            else
		//            {
		//                BasePage.wait.Until<Boolean>(d => d.FindElement(By.CssSelector("div#GroupProcessing")).GetAttribute("style").Contains("display: none;"));
		//            }

		//            SetText("cssselector", "#m_groupInfoDialog_m_groupName", G1);
		//            SetText("cssselector", "#m_groupInfoDialog_m_groupDescription", G1);
		//            Click("cssselector", "#m_groupInfoDialog_SaveAndCreateNewButton");
		//            PageLoadWait.WaitForFrameLoad(20);
		//            ExecutedSteps++;

		//            //Step 14 - Create group G2
		//            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#createGroupDiv")));
		//            SetText("cssselector", "#m_groupInfoDialog_m_groupName", G2);
		//            SetText("cssselector", "#m_groupInfoDialog_m_groupDescription", G2);
		//            Click("cssselector", "#m_groupInfoDialog_SaveAndCreateNewButton");
		//            PageLoadWait.WaitForFrameLoad(20);
		//            ExecutedSteps++;

		//            //Step 15 - Create group G4
		//            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#createGroupDiv")));
		//            SetText("cssselector", "#m_groupInfoDialog_m_groupName", G4);
		//            SetText("cssselector", "#m_groupInfoDialog_m_groupDescription", G4);
		//            Click("cssselector", "#m_groupInfoDialog_SaveAndViewButton");
		//            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div#CreateManagingUserDiv")));
		//            PageLoadWait.WaitForFrameLoad(20);
		//            ExecutedSteps++;

		//            //Step 16 - Select G1 from the Group list
		//            usermanagement.SearchGroup(G1, D1, 0);
		//            usermanagement.SelectGroup(G1, D1);
		//            ExecutedSteps++;

		//            //Step 17 - Select the "New Subgroup" button
		//            usermanagement.NewSubGrpBtn().Click();
		//            ExecutedSteps++;

		//            //Step 18 - Create subgroup CG1
		//            usermanagement.GroupNameTxtBox().SendKeys(CG1);
		//            usermanagement.GroupDescTxtBox().SendKeys(CG1);
		//            ClickButton("input[id='m_groupInfoDialog_SaveAndViewButton']");
		//            ExecutedSteps++;

		//            //Step 19 - Select the triangle on the right hand side of the G1
		//            ExecutedSteps++;

		//            //Step 20 - Create Roles
		//            rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
		//            ExecutedSteps++;

		//            //Step 21 - Select the New Role button
		//            ExecutedSteps++;

		//            //Step 22 - Create 'Doctor' role
		//            rolemanagement.ClickNewRoleBtn();
		//            rolemanagement.CreateRole(D1, Doctor);
		//            rolemanagement.SearchRole(Doctor);
		//            rolemanagement.EditRoleByName(Doctor);
		//            rolemanagement.SetCheckboxInEditRole("transfer", 0);
		//            rolemanagement.SetCheckboxInEditRole("download", 0);
		//            rolemanagement.GrantAccessRadioBtn_Anyone().Click();
		//            rolemanagement.ConnectAllDataSources();
		//            rolemanagement.ClickSaveEditRole();
		//            ExecutedSteps++;

		//            //Step 23 - Create 'Nurse' role
		//            rolemanagement.ClickNewRoleBtn();
		//            rolemanagement.CreateRole(D1, Nurse);
		//            rolemanagement.SearchRole(Nurse);
		//            rolemanagement.EditRoleByName(Nurse);
		//            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
		//            rolemanagement.GrantAccessRadioBtn_Anyone().Click();
		//            rolemanagement.ConnectAllDataSources();
		//            //rolemanagement.RoleFilter_RefPhysician(FirstName, LastName);
		//            rolemanagement.SetCheckboxInEditRole("transfer", 1);
		//            rolemanagement.SetCheckboxInEditRole("download", 1);
		//            rolemanagement.ClickSaveEditRole();
		//            ExecutedSteps++;

		//            //Step 24 - create a new role 'Front Desk'
		//            rolemanagement.ClickNewRoleBtn();
		//            rolemanagement.CreateRole(D1, FrontDesk);
		//            rolemanagement.SearchRole(FrontDesk);
		//            rolemanagement.EditRoleByName(FrontDesk);
		//            rolemanagement.SetCheckboxInEditRole("transfer", 0);
		//            rolemanagement.SetCheckboxInEditRole("download", 1);
		//            rolemanagement.GrantAccessRadioBtn_Disabled().Click();
		//            rolemanagement.ConnectAllDataSources(1);
		//            rolemanagement.ClickSaveEditRole();
		//            ExecutedSteps++;

		//            //Step 25 - Create a new role 'Front Desk1' 
		//            rolemanagement.ClickNewRoleBtn();
		//            rolemanagement.CreateRole(D2, FrontDesk1);
		//            rolemanagement.SearchRole(FrontDesk1);
		//            rolemanagement.EditRoleByName(FrontDesk1);
		//            rolemanagement.SetCheckboxInEditRole("transfer", 1);
		//            rolemanagement.SetCheckboxInEditRole("download", 1);
		//            rolemanagement.GrantAccessRadioBtn_Disabled().Click();
		//            rolemanagement.ConnectAllDataSources(1);
		//            rolemanagement.ClickSaveEditRole();
		//            ExecutedSteps++;

		//            //Step 26 - Select the User Management tab
		//            usermanagement = (UserManagement)login.Navigate("UserManagement");
		//            ExecutedSteps++;

		//            //Step 27 - Select the "D1" Domain from the dropdown list "Show Users From Domain"
		//            usermanagement.SelectDomainFromDropdownList(D1);
		//            ExecutedSteps++;

		//            //Step 28 - Select the G1 in the Groups list.
		//            usermanagement.SearchGroup(G1, D1, 0);
		//            usermanagement.SelectGroupByName(G1);
		//            usermanagement.EditGrpBtn().Click();
		//            wait.Until(ExpectedConditions.ElementToBeClickable(usermanagement.CreateAndEditGroupPopupWindow()));
		//            PageLoadWait.WaitForProcessingState(10);
		//            usermanagement.RolesTab_Group().Click();
		//            IWebElement table = Driver.FindElement(By.CssSelector("#m_groupInfoDialog_m_groupRolesList_hierarchyRoleList_itemList"));
		//            List<IWebElement> allRows = table.FindElements(By.CssSelector("tr:not([style*='display: none;'])")).ToList();
		//            if (allRows.Count > 0)
		//            {
		//                for (int i = 0; i < allRows.Count; i++)
		//                {
		//                    wait.Until(ExpectedConditions.ElementToBeClickable(usermanagement.Btn_RoleAdd()));
		//                    allRows[i].Click();
		//                }
		//                usermanagement.Btn_RoleAdd().Click();
		//            }
		//            usermanagement.SaveAndViewMyGroupBtn().Click();
		//            ExecutedSteps++;

		//            //Step 29 - Create user U4, Role Name - Doctor
		//            //var maint = login.Navigate<Maintenance>();
		//            //eventcount_before = maint.GetEventCount(eventtype, eventID: "Security Alert");

		//            usermanagement = (UserManagement)login.Navigate("UserManagement");
		//            usermanagement.SearchGroup(G1, D1, 0);
		//            usermanagement.SelectGroupByName(G1);
		//            usermanagement.CreateUser(U4, Doctor, 1, Email, 1, U4);

		//            //maint = login.Navigate<Maintenance>();
		//            //eventcount_after = maint.GetEventCount(eventtype, eventID: "Security Alert");
		//            //if (eventcount_after == eventcount_before + 1)
		//            //    AuditLogList.Insert(0, true);
		//            //else AuditLogList.Insert(0, false);
		//            //eventcount_before = eventcount_after;
		//            ExecutedSteps++;

		//            //Step 30 - Select the Group G1       
		//            usermanagement = (UserManagement)login.Navigate("UserManagement");
		//            if (usermanagement.IsUserExist(U4, D1))
		//            {
		//                result.steps[++ExecutedSteps].status = "Pass";
		//                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
		//            }
		//            else
		//            {
		//                result.steps[++ExecutedSteps].status = "Fail";
		//                Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
		//                result.steps[ExecutedSteps].SetLogs();
		//            }

		//            //Step 31 - Select the 'D1' Domain from the dropdown list 'Show Users From Domain'
		//            usermanagement.SelectDomainFromDropdownList(D1);
		//            ExecutedSteps++;

		//            //Step 32 - Select the 'New User' button
		//            usermanagement.SearchGroup(G2, D1, 0);
		//            usermanagement.SelectGroupByName(G2);
		//            usermanagement.EditGrpBtn().Click();
		//            wait.Until(ExpectedConditions.ElementToBeClickable(usermanagement.CreateAndEditGroupPopupWindow()));
		//            PageLoadWait.WaitForProcessingState(10);
		//            usermanagement.RolesTab_Group().Click();
		//            IWebElement table1 = Driver.FindElement(By.CssSelector("#m_groupInfoDialog_m_groupRolesList_hierarchyRoleList_itemList"));
		//            List<IWebElement> allRows1 = table1.FindElements(By.CssSelector("tr:not([style*='display: none;'])")).ToList();
		//            if (allRows1.Count > 0)
		//            {
		//                for (int i = 0; i < allRows1.Count; i++)
		//                {
		//                    wait.Until(ExpectedConditions.ElementToBeClickable(usermanagement.Btn_RoleAdd()));
		//                    allRows1[i].Click();
		//                }
		//                usermanagement.Btn_RoleAdd().Click();
		//            }
		//            usermanagement.SaveAndViewMyGroupBtn().Click();
		//            ExecutedSteps++;

		//            //Step 33 - Create user U5    
		//            //maint = login.Navigate<Maintenance>();
		//            //eventcount_before = maint.GetEventCount(eventtype, eventID: "Security Alert");

		//            usermanagement = (UserManagement)login.Navigate("UserManagement");
		//            usermanagement.SearchGroup(G2, D1, 0);
		//            usermanagement.SelectGroupByName(G2);
		//            usermanagement.CreateUser(U5, Nurse, 1, Email, 1, U5);

		//            //maint = login.Navigate<Maintenance>();
		//            //eventcount_after = maint.GetEventCount(eventtype, eventID: "Security Alert");
		//            //if (eventcount_after == eventcount_before + 1)
		//            //    AuditLogList.Insert(1, true);
		//            //else AuditLogList.Insert(1, false);
		//            ExecutedSteps++;

		//            //Step 34 - Select the Group G2 in the Groups list.   
		//            usermanagement = (UserManagement)login.Navigate("UserManagement");
		//            usermanagement.SearchGroup(G2, D1, 0);
		//            usermanagement.SelectGroupByName(G2);
		//            if (usermanagement.IsUserExist(U5, D1))
		//            {
		//                result.steps[++ExecutedSteps].status = "Pass";
		//                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
		//            }
		//            else
		//            {
		//                result.steps[++ExecutedSteps].status = "Fail";
		//                Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
		//                result.steps[ExecutedSteps].SetLogs();
		//            }

		//            //Step 35 - Create user U1
		//            //eventcount_before = eventcount_after;
		//            usermanagement.CreateUser(U1, FrontDesk, 1, Email, 1, U1);

		//            //maint = login.Navigate<Maintenance>();
		//            //eventcount_after = maint.GetEventCount(eventtype, eventID: "Security Alert");
		//            //if (eventcount_after == eventcount_before + 1)
		//            //    AuditLogList.Insert(2, true);
		//            //else AuditLogList.Insert(2, true);
		//            ExecutedSteps++;

		//            //Step 36 - Select the Group G2 in the Groups list.           
		//            usermanagement = (UserManagement)login.Navigate("UserManagement");
		//            if (usermanagement.IsUserExist(U1, D1) && usermanagement.IsUserExist(U5, D1))
		//            {
		//                result.steps[++ExecutedSteps].status = "Pass";
		//                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
		//            }
		//            else
		//            {
		//                result.steps[++ExecutedSteps].status = "Fail";
		//                Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
		//                result.steps[ExecutedSteps].SetLogs();
		//            }

		//            //Step 37 - Select the triangle on the right hand side of the G1 in the Groups list and then select CG1
		//            usermanagement.SearchGroup(G1, D1, 0);
		//            if (usermanagement.SelectSubGroup(G1, CG1))
		//            {
		//                result.steps[++ExecutedSteps].status = "Pass";
		//                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
		//            }
		//            else
		//            {
		//                result.steps[++ExecutedSteps].status = "Fail";
		//                Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
		//                result.steps[ExecutedSteps].SetLogs();
		//            }

		//            //Step 38 - Select New User button
		//            usermanagement.EditGrpBtn().Click();
		//            wait.Until(ExpectedConditions.ElementToBeClickable(usermanagement.CreateAndEditGroupPopupWindow()));
		//            PageLoadWait.WaitForProcessingState(10);
		//            usermanagement.RolesTab_Group().Click();
		//            IWebElement table2 = Driver.FindElement(By.CssSelector("#m_groupInfoDialog_m_groupRolesList_hierarchyRoleList_itemList"));
		//            List<IWebElement> allRows2 = table2.FindElements(By.CssSelector("tr:not([style*='display: none;'])")).ToList();
		//            if (allRows2.Count > 0)
		//            {
		//                for (int i = 0; i < allRows2.Count; i++)
		//                {
		//                    wait.Until(ExpectedConditions.ElementToBeClickable(usermanagement.Btn_RoleAdd()));
		//                    allRows2[i].Click();
		//                }
		//                usermanagement.Btn_RoleAdd().Click();
		//            }
		//            usermanagement.SaveAndViewMyGroupBtn().Click();
		//            ExecutedSteps++;

		//            //Step 39 - Create user U6
		//            //maint = login.Navigate<Maintenance>();
		//            //eventcount_before = maint.GetEventCount(eventtype, eventID: "Security Alert");
		//            usermanagement = (UserManagement)login.Navigate("UserManagement");

		//            usermanagement.SearchGroup(CG1, D1, 1);
		//            usermanagement.SelectGroupByName(CG1);
		//            usermanagement.CreateUser(U6, FrontDesk, 1, Email, 1, U6);

		//            //maint = login.Navigate<Maintenance>();
		//            //eventcount_after = maint.GetEventCount(eventtype, eventID: "Security Alert");
		//            //if (eventcount_after == eventcount_before + 1)
		//            //    AuditLogList.Insert(3, true);
		//            //else AuditLogList.Insert(3, false);
		//            ExecutedSteps++;

		//            //Step 40 - Select the triangle on the right hand side of the G1 in the Groups list and then select CG1
		//            usermanagement = (UserManagement)login.Navigate("UserManagement");
		//            usermanagement.SearchGroup(CG1, D1, 1);
		//            usermanagement.SelectGroupByName(CG1);
		//            if (usermanagement.IsUserExist(U6, D1))
		//            {
		//                result.steps[++ExecutedSteps].status = "Pass";
		//                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
		//            }
		//            else
		//            {
		//                result.steps[++ExecutedSteps].status = "Fail";
		//                Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
		//                result.steps[ExecutedSteps].SetLogs();
		//            }

		//            //Step 41 - Repeat the above step to add user "U7" to CG1 group with the role "Nurse".                
		//            usermanagement.CreateUser(U7, Nurse, 1, Email, 1, U7);
		//            if (usermanagement.IsUserExist(U7, D1))
		//            {
		//                result.steps[++ExecutedSteps].status = "Pass";
		//                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
		//            }
		//            else
		//            {
		//                result.steps[++ExecutedSteps].status = "Fail";
		//                Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
		//                result.steps[ExecutedSteps].SetLogs();
		//            }

		//            //Step 42 - Select the User Management Tab
		//            usermanagement = (UserManagement)login.Navigate("UserManagement");
		//            ExecutedSteps++;

		//            //Step 43 - Select Domain D2 from the drop down list.
		//            usermanagement.SelectDomainFromDropdownList(D1);
		//            ExecutedSteps++;

		//            //Step 44 - Select the"New Group"Button
		//            ExecutedSteps++;

		//            //Step 45 - Create group G3
		//            //maint = login.Navigate<Maintenance>();
		//            //eventcount_before = maint.GetEventCount(eventtype, eventID: "Security Alert");

		//            usermanagement = (UserManagement)login.Navigate("UserManagement");
		//            usermanagement.SelectDomainFromDropdownList(D1);
		//            usermanagement.CreateGroup(D2, G3, password: U9, rolename: FrontDesk1, email: Email, IsManaged: 1, rolenames: new string[] { FrontDesk1 }, GroupUser: U9);
		//            ExecutedSteps++;

		//            //Step 46 - Create user U9
		//            //maint = login.Navigate<Maintenance>();
		//            //eventcount_after = maint.GetEventCount(eventtype, eventID: "Security Alert");
		//            //if (eventcount_after == eventcount_before + 2)
		//            //    AuditLogList.Insert(4, true);
		//            //else AuditLogList.Insert(4, false);
		//            ExecutedSteps++;

		//            //Step 47 - Select Group Roles - select one or use the default
		//            ExecutedSteps++;

		//            //Step 48 - Logout and login by using Administrator account, select the User Management tab
		//            login.Logout();
		//            login.LoginIConnect(Config.adminUserName, Config.adminPassword);
		//            usermanagement = (UserManagement)login.Navigate("UserManagement");
		//            ExecutedSteps++;

		//            //Step 49 - Select group (G2) and click on Edit.
		//            usermanagement.SearchGroup(G2, D1, 1);
		//            usermanagement.SelectGroupByName(G2);
		//            ExecutedSteps++;

		//            //Step 50 - Try to modify different fields such as- group name, descriptions, Select Save.
		//            usermanagement.EditGrpBtn().Click();
		//            wait.Until(ExpectedConditions.ElementToBeClickable(usermanagement.CreateAndEditGroupPopupWindow()));
		//            PageLoadWait.WaitForProcessingState(10);
		//            usermanagement.GroupDescTxtBox().Clear();
		//            usermanagement.GroupDescTxtBox().SendKeys(G2 + " Description");
		//            usermanagement.SaveAndViewMyGroupBtn().Click();
		//            ExecutedSteps++;

		//            //Step 51 - Repeat the above step for group G1.
		//            usermanagement.SearchGroup(G1, D1, 1);
		//            usermanagement.SelectGroupByName(G1);
		//            usermanagement.EditGrpBtn().Click();
		//            wait.Until(ExpectedConditions.ElementToBeClickable(usermanagement.CreateAndEditGroupPopupWindow()));
		//            PageLoadWait.WaitForProcessingState(10);
		//            usermanagement.GroupDescTxtBox().Clear();
		//            usermanagement.GroupDescTxtBox().SendKeys(G1 + " Description");
		//            usermanagement.SaveAndViewMyGroupBtn().Click();
		//            ExecutedSteps++;

		//            //Step 52 - Select Group G4
		//            //maint = login.Navigate<Maintenance>();
		//            //eventcount_before = maint.GetEventCount(eventtype, eventID: "Security Alert");

		//            usermanagement = (UserManagement)login.Navigate("UserManagement");
		//            usermanagement.SelectDomainFromDropdownList(D1);
		//            usermanagement.SelectGroupByName(G4);
		//            ExecutedSteps++;

		//            //Step 53 - Select Delete button
		//            usermanagement.DelGrpBtn().Click();
		//            PageLoadWait.WaitForPageLoad(10);
		//            BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
		//            if (BasePage.Driver.FindElement(By.CssSelector("#DialogDiv #ConfirmationDiv")).Displayed)
		//            {
		//                result.steps[++ExecutedSteps].status = "Pass";
		//                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
		//            }
		//            else
		//            {
		//                result.steps[++ExecutedSteps].status = "Fail";
		//                Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
		//                result.steps[ExecutedSteps].SetLogs();
		//            }

		//            //Setp 54 - Select OK
		//            PageLoadWait.WaitForPageLoad(20);
		//            usermanagement.OkButtonConfirmGroupDeletionMsgBox().Click();

		//            //maint = login.Navigate<Maintenance>();
		//            //eventcount_after = maint.GetEventCount(eventtype, eventID: "Security Alert");
		//            //if (eventcount_after == eventcount_before + 1)
		//            //    AuditLogList.Insert(5, true);
		//            //else AuditLogList.Insert(5, false);
		//            ExecutedSteps++;

		//            //Logout
		//            login.Logout();

		//            //Report Result
		//            result.FinalResult(ExecutedSteps);
		//            Logger.Instance.ErrorLog("Overall Test status--" + result.status);

		//            //Return Result
		//            return result;
		//        }
		//        catch (Exception e)
		//        {
		//            //Log Exception
		//            Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);                

		//            //Report Result
		//            result.FinalResult(e, ExecutedSteps);
		//            Logger.Instance.ErrorLog("Overall Test status--" + result.status);

		//            //Return Result
		//            return result;
		//        }
		//        finally
		//        {
		//            if (AuditLogList.Count > 0)
		//            {
		//                if (String.IsNullOrEmpty(AuditLogList[0].ToString()))
		//                    AuditLogList.Insert(0, false);
		//            }
		//            else
		//            {
		//                AuditLogList.Insert(0, false);
		//            }

		//            if (AuditLogList.Count > 1)
		//            {
		//                if (String.IsNullOrEmpty(AuditLogList[1].ToString()))
		//                    AuditLogList.Insert(1, false);
		//            }
		//            else
		//            {
		//                AuditLogList.Insert(1, false);
		//            }

		//            if (AuditLogList.Count > 2)
		//            {
		//                if (String.IsNullOrEmpty(AuditLogList[2].ToString()))
		//                    AuditLogList.Insert(2, false);
		//            }
		//            else
		//            {
		//                AuditLogList.Insert(2, false);
		//            }

		//            if (AuditLogList.Count > 3)
		//            {
		//                if (String.IsNullOrEmpty(AuditLogList[3].ToString()))
		//                    AuditLogList.Insert(3, false);
		//            }
		//            else
		//            {
		//                AuditLogList.Insert(3, false);
		//            }            

		//            if (AuditLogList.Count > 4)
		//            {
		//                if (String.IsNullOrEmpty(AuditLogList[4].ToString()))
		//                    AuditLogList.Insert(4, false);
		//            }
		//            else
		//            {
		//                AuditLogList.Insert(4, false);
		//            }

		//            if (AuditLogList.Count > 5)
		//            {
		//                if (String.IsNullOrEmpty(AuditLogList[5].ToString()))
		//                    AuditLogList.Insert(5, false);
		//            }
		//            else
		//            {
		//                AuditLogList.Insert(5, false);
		//            }
		//        }
		//    }
		public TestCaseResult Test_27571(String testid, String teststeps, int stepcount)
		{
			//Declare and initialize variables  
			TestCaseResult result;
			result = new TestCaseResult(stepcount);

			//Set up Validation Steps
			result.SetTestStepDescription(teststeps);
			int ExecutedSteps = -1;
			int eventcount_before = 0;
			int eventcount_after = 0;
			maintenance = new Maintenance();

			D1 = "D1_" + new Random().Next(1, 1000);
			D2 = "D2_" + new Random().Next(1, 1000);
			ROLE1 = "ROLE1_" + new Random().Next(1, 1000);
			ROLE2 = "ROLE2_" + new Random().Next(1, 1000);

			G1 = "G1_" + new Random().Next(1, 1000);
			G2 = "G2_" + new Random().Next(1, 1000);
			G3 = "G3_" + new Random().Next(1, 1000);
			G4 = "G4_" + new Random().Next(1, 1000);
			CG1 = "CG1_" + new Random().Next(1, 1000);

			Doctor = "Doctor_" + new Random().Next(1, 1000);
			Nurse = "Nurse_" + new Random().Next(1, 1000);
			FrontDesk = "FrontDesk_" + new Random().Next(1, 1000);
			FrontDesk1 = "FrontDesk1_" + new Random().Next(1, 1000);

			FirstName = "First_" + new Random().Next(1, 1000);
			LastName = "Last_" + new Random().Next(1, 1000);

			U1 = "U1_" + new Random().Next(1, 1000);
			U4 = "U4_" + new Random().Next(1, 1000);
			U5 = "U5_" + new Random().Next(1, 1000);
			U6 = "U6_" + new Random().Next(1, 1000);
			U7 = "U7_" + new Random().Next(1, 1000);
			U9 = "U9_" + new Random().Next(1, 1000);

			d1 = "d1_" + new Random().Next(1, 1000);

			try
			{
				EmailUtils customEmail = new EmailUtils() { EmailId = Config.CustomUser1Email, Password = Config.CustomUserEmailPassword };
				customEmail.MarkAllMailAsRead("INBOX");
				EmailUtils customEmail1 = new EmailUtils() { EmailId = Config.CustomUser2Email, Password = Config.CustomUserEmailPassword };
				customEmail1.MarkAllMailAsRead("INBOX");
				EmailUtils customEmail2 = new EmailUtils() { EmailId = Config.CustomUser3Email, Password = Config.CustomUserEmailPassword };
				customEmail2.MarkAllMailAsRead("INBOX");
				//Fetch required Test data    
				String Email = Config.CustomUser1Email;// (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");

				//Step 1 - Pre-conditions
				ExecutedSteps++;

				//Step 2 - In the Service Tool*^>^*Enable Features tab Enable Study Sharing , Enable Study Transfer, Enable Downloader and Uploader.
				servicetool.LaunchServiceTool();
				servicetool.NavigateToEnableFeatures();
				wpfobject.WaitTillLoad();
				servicetool.ModifyEnableFeatures();
				wpfobject.WaitTillLoad();
				servicetool.EnableStudySharing();
				servicetool.EnableDataTransfer();
				servicetool.EnableDataDownloader();
				servicetool.ApplyEnableFeatures();
				wpfobject.WaitTillLoad();
				wpfobject.ClickOkPopUp();
				wpfobject.WaitTillLoad();

				/*servicetool.EnableUpload();
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.StudyAttachment);
                wpfobject.WaitTillLoad();
                wpfobject.GetButton(ServiceTool.ModifyBtn_Name, 1).Click();
                wpfobject.WaitTillLoad();
                wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab("Study Attachment"), ServiceTool.EnableFeatures.ID.UploadAllowed).Checked = true;

                wpfobject.GetButton(ServiceTool.ApplyBtn_Name, 1).Click();
                wpfobject.WaitTillLoad();
                Thread.Sleep(5000);
                if (wpfobject.CheckWindowExists("Confirm"))
                {
                    wpfobject.GetMainWindowByIndex(1);
                    wpfobject.GetButton(ServiceTool.YesBtn_Name, 1).Click();
                    wpfobject.WaitTillLoad();
                    Thread.Sleep(10000);
                }
                wpfobject.GetMainWindowByIndex(1);
                wpfobject.ClickButton("2");
                Thread.Sleep(2000);

                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindowByIndex(0);*/
				servicetool.RestartIISandWindowsServices();
				ExecutedSteps++;

				//Step 3 - In the Service Tool*^>^*Enable Features*^>^*Transfer Service --"Enable Transfer Service"option by clicking on the box.
				servicetool.SetEnableFeaturesTransferService();
				servicetool.ModifyEnableFeatures();
				wpfobject.WaitTillLoad();
				servicetool.EnableTransferService();
				ExecutedSteps++;

				//Step 4 - In the Service Tool*^>^*Enable Features*^>^*Transfer Service -- at the bottom of the window, select the package tab and change the expire interval to 5 min, and the Package maximum to 10000kB
				servicetool.ModifyPackagerDetails("5");
				ExecutedSteps++;

				//Step 5 - Setup Email Notification in the service tool
				servicetool.NavigateToTab("E-mail Notification");
				servicetool.NavigateSubTab("General");
				//servicetool.SetEmailNotification();
				servicetool.SetEmailNotificationForPOP();

				servicetool.RestartIISandWindowsServices();
				servicetool.CloseServiceTool();
				ExecutedSteps++;

				//Step 6 - Log in iConnect Access desktop version as administrator
				login.DriverGoTo(login.url);
				login.LoginIConnect(Config.adminUserName, Config.adminPassword);
				domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
				ExecutedSteps++;

				//Step 7 - Select the New Domain button                
				ExecutedSteps++;

				//Step 8 - Create New Domain
				domainmanagement.CreateDomain(domainName: D1, roleName: ROLE1, datasources: null);
				domainmanagement.SetCheckBoxInEditDomain("datatransfer", 0);
				domainmanagement.SetCheckBoxInEditDomain("grant", 0);
				domainmanagement.SetCheckBoxInEditDomain("datadownload", 0);
				domainmanagement.ClickSaveDomain();
				ExecutedSteps++;

				//Step 9 - Repeat and create another new Domains D2, ROLE2
				domainmanagement.CreateDomain(domainName: D2, roleName: ROLE2, datasources: null);
				domainmanagement.SetCheckBoxInEditDomain("datatransfer", 0);
				domainmanagement.SetCheckBoxInEditDomain("grant", 0);
				domainmanagement.SetCheckBoxInEditDomain("datadownload", 0);
				domainmanagement.ClickSaveDomain();
				if (domainmanagement.IsDomainExist(D1) && domainmanagement.IsDomainExist(D2))
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

				//Step 10 - Create Groups and Subgroups
				usermanagement = (UserManagement)login.Navigate("UserManagement");
				ExecutedSteps++;

				//Step 11 - Select Domain D1 from the drop down list.
				usermanagement.SelectDomainFromDropdownList(D1);
				ExecutedSteps++;

				//Step 12 - Select the "New Group" Button
				if (SBrowserName.ToLower().Equals("internet explorer"))
				{
					Click("cssselector", "#NewGroupButton", true);
					PageLoadWait.WaitForPageLoad(10);
					SwitchToDefault();
					PageLoadWait.WaitForPageLoad(10);
					SwitchTo("index", "0");
				}
				else
					usermanagement.ClickButtonInUser("newgroup");
				ExecutedSteps++;

				//Step 13 - Create group G1
				PageLoadWait.WaitForFrameLoad(10);
				wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#createGroupDiv")));
				if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
				{
					BasePage.wait.Until<Boolean>(d => d.FindElement(By.CssSelector("div#GroupProcessing")).GetAttribute("style").Contains("DISPLAY: none;"));
				}
				else
				{
					BasePage.wait.Until<Boolean>(d => d.FindElement(By.CssSelector("div#GroupProcessing")).GetAttribute("style").Contains("display: none;"));
				}

				SetText("cssselector", "#m_groupInfoDialog_m_groupName", G1);
				SetText("cssselector", "#m_groupInfoDialog_m_groupDescription", G1);
				if (SBrowserName.ToLower().Equals("internet explorer"))
					Click("cssselector", "#m_groupInfoDialog_SaveAndCreateNewButton", true);
				else
					Click("cssselector", "#m_groupInfoDialog_SaveAndCreateNewButton");
				PageLoadWait.WaitForFrameLoad(20);
				ExecutedSteps++;

				//Step 14 - Create group G2
				wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#createGroupDiv")));
				SetText("cssselector", "#m_groupInfoDialog_m_groupName", G2);
				SetText("cssselector", "#m_groupInfoDialog_m_groupDescription", G2);
				if (SBrowserName.ToLower().Equals("internet explorer"))
					Click("cssselector", "#m_groupInfoDialog_SaveAndCreateNewButton", true);
				else
					Click("cssselector", "#m_groupInfoDialog_SaveAndCreateNewButton");
				PageLoadWait.WaitForFrameLoad(20);
				ExecutedSteps++;

				//Step 15 - Create group G4
				wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#createGroupDiv")));
				SetText("cssselector", "#m_groupInfoDialog_m_groupName", G4);
				SetText("cssselector", "#m_groupInfoDialog_m_groupDescription", G4);
				if (SBrowserName.ToLower().Equals("internet explorer"))
					Click("cssselector", "#m_groupInfoDialog_SaveAndViewButton", true);
				else
					Click("cssselector", "#m_groupInfoDialog_SaveAndViewButton");
				wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div#CreateManagingUserDiv")));
				PageLoadWait.WaitForFrameLoad(20);
				ExecutedSteps++;

				//Step 16 - Select G1 from the Group list
				usermanagement.SearchGroup(G1, D1, 0);
				usermanagement.SelectGroup(G1, D1);
				ExecutedSteps++;

				//Step 17 - Select the "New Subgroup" button
				if (SBrowserName.ToLower().Equals("internet explorer"))
					Click("cssselector", "#NewSubgroupButton", true);
				else
					usermanagement.NewSubGrpBtn().Click();
				PageLoadWait.WaitForFrameLoad(10);
				wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#createGroupDiv")));
				ExecutedSteps++;

				//Step 18 - Create subgroup CG1
				usermanagement.GroupNameTxtBox().SendKeys(CG1);
				usermanagement.GroupDescTxtBox().SendKeys(CG1);
				ClickButton("input[id='m_groupInfoDialog_SaveAndViewButton']");
				ExecutedSteps++;

				//Step 19 - Select the triangle on the right hand side of the G1
				usermanagement.SearchGroup(G1, D1, 0);
				if (usermanagement.Hierarchy().Text == "▼")
				{
					usermanagement.Hierarchy().Click();
					PageLoadWait.WaitForFrameLoad(20);
					string[] grouplist = usermanagement.SubGroupLists().Select(sg => sg.Text).ToArray();
					if (grouplist.Any(sg1 => sg1.Contains(CG1)))
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
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step 20 - Create Roles
				rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
				ExecutedSteps++;

				//Step 21 - Select the New Role button
				ExecutedSteps++;

				//Step 22 - Create 'Doctor' role
				rolemanagement.ClickNewRoleBtn();
				rolemanagement.CreateRole(D1, Doctor);
				rolemanagement.SearchRole(Doctor, D1);
				rolemanagement.EditRoleByName(Doctor);
				rolemanagement.SetCheckboxInEditRole("transfer", 0);
				rolemanagement.SetCheckboxInEditRole("download", 0);
				if (SBrowserName.ToLower().Equals("internet explorer"))
					SetCheckbox(Driver.FindElement(By.CssSelector("[id$='_RoleAccessFilter_GrantAccessRadioButtonList_2']")), true);
				else
					rolemanagement.GrantAccessRadioBtn_Anyone().Click();
				rolemanagement.ConnectAllDataSources();
				rolemanagement.ClickSaveEditRole();
				ExecutedSteps++;

				//Step 23 - Create 'Nurse' role
				rolemanagement.ClickNewRoleBtn();
				rolemanagement.CreateRole(D1, Nurse);
				rolemanagement.SearchRole(Nurse, D1);
				rolemanagement.EditRoleByName(Nurse);
				Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				if (SBrowserName.ToLower().Equals("internet explorer"))
					SetCheckbox(Driver.FindElement(By.CssSelector("[id$='_RoleAccessFilter_GrantAccessRadioButtonList_2']")), true);
				else
					rolemanagement.GrantAccessRadioBtn_Anyone().Click();
				rolemanagement.ConnectAllDataSources();
				//rolemanagement.RoleFilter_RefPhysician(FirstName, LastName); 
				rolemanagement.SetCheckboxInEditRole("transfer", 0);
				rolemanagement.SetCheckboxInEditRole("download", 0);
				rolemanagement.ClickSaveEditRole();
				ExecutedSteps++;

				//Step 24 - create a new role 'Front Desk'
				rolemanagement.ClickNewRoleBtn();
				rolemanagement.CreateRole(D1, FrontDesk);
				rolemanagement.SearchRole(FrontDesk, D1);
				rolemanagement.EditRoleByName(FrontDesk);
				rolemanagement.SetCheckboxInEditRole("transfer", 0);
				rolemanagement.SetCheckboxInEditRole("download", 0);
				if (SBrowserName.ToLower().Equals("internet explorer"))
					SetCheckbox(Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_RoleAccessFilter_GrantAccessRadioButtonList_0")), true);
				else
					rolemanagement.GrantAccessRadioBtn_Disabled().Click();
				rolemanagement.ConnectAllDataSources(1);
				rolemanagement.ClickSaveEditRole();
				ExecutedSteps++;

				//Step 25 - Create a new role 'Front Desk1' 
				rolemanagement.ClickNewRoleBtn();
				rolemanagement.CreateRole(D2, FrontDesk1);
				rolemanagement.SearchRole(FrontDesk1, D2);
				rolemanagement.EditRoleByName(FrontDesk1);
				rolemanagement.SetCheckboxInEditRole("transfer", 1);
				rolemanagement.SetCheckboxInEditRole("download", 1);
				if (SBrowserName.ToLower().Equals("internet explorer"))
					SetCheckbox(Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_RoleAccessFilter_GrantAccessRadioButtonList_0")), true);
				else
					rolemanagement.GrantAccessRadioBtn_Disabled().Click();
				rolemanagement.ConnectAllDataSources(1);
				rolemanagement.ClickSaveEditRole();
				ExecutedSteps++;

				//Step 26 - Select the User Management tab
				usermanagement = (UserManagement)login.Navigate("UserManagement");
				ExecutedSteps++;

				//Step 27 - Select the "D1" Domain from the dropdown list "Show Users From Domain"
				if (usermanagement.SearchGroup(G1, D1, 0))
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

				//Step 28 - Select the G1 in the Groups list.
				usermanagement.SearchGroup(G1, D1, 0);
				usermanagement.SelectGroupByName(G1);
				if (SBrowserName.ToLower().Equals("internet explorer"))
					Click("cssselector", "#EditGroupButton", true);
				else
					usermanagement.EditGrpBtn().Click();
				wait.Until(ExpectedConditions.ElementToBeClickable(usermanagement.CreateAndEditGroupPopupWindow()));
				PageLoadWait.WaitForProcessingState(10);
				if (SBrowserName.ToLower().Equals("internet explorer"))
					Click("cssselector", "#RolesTab>a>div>span", true);
				else
					usermanagement.RolesTab_Group().Click();
				IWebElement table = Driver.FindElement(By.CssSelector("#m_groupInfoDialog_m_groupRolesList_hierarchyRoleList_itemList"));
				List<IWebElement> allRows = table.FindElements(By.CssSelector("tr:not([style*='display: none;'])")).ToList();
				if (allRows.Count > 0)
				{
					for (int i = 0; i < allRows.Count; i++)
					{
						wait.Until(ExpectedConditions.ElementToBeClickable(usermanagement.Btn_RoleAdd()));
						allRows[i].Click();
					}
					if (SBrowserName.ToLower().Equals("internet explorer"))
						Click("cssselector", "input[id*='m_groupRolesList_Button_Add']", true);
					else
						usermanagement.Btn_RoleAdd().Click();
				}
				if (SBrowserName.ToLower().Equals("internet explorer"))
					Click("cssselector", "#m_groupInfoDialog_SaveAndViewButton", true);
				else
					usermanagement.SaveAndViewMyGroupBtn().Click();
				Thread.Sleep(100000);
				ExecutedSteps++;

				//Step 29 - Create user U4, Role Name - Doctor                
				eventcount_before = TestFixtures.GetEventCount();

				usermanagement = (UserManagement)login.Navigate("UserManagement");
				usermanagement.SearchGroup(G1, D1, 0);
				usermanagement.SelectGroupByName(G1);
				usermanagement.CreateUser(U4, Doctor, 1, Email, 1, U4);
				eventcount_after = TestFixtures.GetEventCount();
				if (eventcount_after == eventcount_before + 2)
					AuditLogList.Insert(0, true);
				else AuditLogList.Insert(0, false);
				eventcount_before = eventcount_after;
				ExecutedSteps++;

				//Step 30 - Select the Group G1       
				usermanagement = (UserManagement)login.Navigate("UserManagement");
				if (usermanagement.IsUserExist(U4, D1))
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

				//Step 31 - Select the 'D1' Domain from the dropdown list 'Show Users From Domain'
				usermanagement.SelectDomainFromDropdownList(D1);
				ExecutedSteps++;

				//Step 32 - Select the 'New User' button
				usermanagement.SearchGroup(G2, D1, 0);
				usermanagement.SelectGroupByName(G2);
				if (SBrowserName.ToLower().Equals("internet explorer"))
					Click("cssselector", "#EditGroupButton", true);
				else
					usermanagement.EditGrpBtn().Click();
				wait.Until(ExpectedConditions.ElementToBeClickable(usermanagement.CreateAndEditGroupPopupWindow()));
				PageLoadWait.WaitForProcessingState(10);
				if (SBrowserName.ToLower().Equals("internet explorer"))
					Click("cssselector", "#RolesTab>a>div>span", true);
				else
					usermanagement.RolesTab_Group().Click();
				IWebElement table1 = Driver.FindElement(By.CssSelector("#m_groupInfoDialog_m_groupRolesList_hierarchyRoleList_itemList"));
				List<IWebElement> allRows1 = table1.FindElements(By.CssSelector("tr:not([style*='display: none;'])")).ToList();
				if (allRows1.Count > 0)
				{
					for (int i = 0; i < allRows1.Count; i++)
					{
						wait.Until(ExpectedConditions.ElementToBeClickable(usermanagement.Btn_RoleAdd()));
						allRows1[i].Click();
					}
					if (SBrowserName.ToLower().Equals("internet explorer"))
						Click("cssselector", "input[id*='m_groupRolesList_Button_Add']", true);
					else
						usermanagement.Btn_RoleAdd().Click();
				}
				if (SBrowserName.ToLower().Equals("internet explorer"))
					Click("cssselector", "#m_groupInfoDialog_SaveAndViewButton", true);
				else
					usermanagement.SaveAndViewMyGroupBtn().Click();
				Thread.Sleep(100000);
				ExecutedSteps++;

				//Step 33 - Create user U5                    
				eventcount_before = TestFixtures.GetEventCount();
				usermanagement = (UserManagement)login.Navigate("UserManagement");
				usermanagement.SearchGroup(G2, D1, 0);
				usermanagement.SelectGroupByName(G2);
				usermanagement.CreateUser(U5, Nurse, 1, Config.CustomUser1Email, 1, U5);

				eventcount_after = TestFixtures.GetEventCount();
				if (eventcount_after == eventcount_before + 3)
					AuditLogList.Insert(1, true);
				else AuditLogList.Insert(1, false);
				ExecutedSteps++;

				//Step 34 - Select the Group G2 in the Groups list.   
				usermanagement = (UserManagement)login.Navigate("UserManagement");
				usermanagement.SearchGroup(G2, D1, 0);
				usermanagement.SelectGroupByName(G2);
				if (usermanagement.IsUserExist(U5, D1))
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

				//Step 35 - Create user U1
				eventcount_before = eventcount_after;
				usermanagement.CreateUser(U1, FrontDesk, 1, Email, 1, U1);

				eventcount_after = TestFixtures.GetEventCount();
				if (eventcount_after == eventcount_before + 3)
					AuditLogList.Insert(2, true);
				else AuditLogList.Insert(2, true);
				ExecutedSteps++;

				//Step 36 - Select the Group G2 in the Groups list.           
				usermanagement = (UserManagement)login.Navigate("UserManagement");
				if (usermanagement.IsUserExist(U1, D1) && usermanagement.IsUserExist(U5, D1))
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

				//Step 37 - Select the triangle on the right hand side of the G1 in the Groups list and then select CG1
				usermanagement.SearchGroup(G1, D1, 0);
				if (usermanagement.SelectSubGroup(G1, CG1))
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

				//Step 38 - Select New User button
				if (SBrowserName.ToLower().Equals("internet explorer"))
					Click("cssselector", "#EditGroupButton", true);
				else
					usermanagement.EditGrpBtn().Click();
				wait.Until(ExpectedConditions.ElementToBeClickable(usermanagement.CreateAndEditGroupPopupWindow()));
				PageLoadWait.WaitForProcessingState(10);
				if (SBrowserName.ToLower().Equals("internet explorer"))
					Click("cssselector", "#RolesTab>a>div>span", true);
				else
					usermanagement.RolesTab_Group().Click();
				IWebElement table2 = Driver.FindElement(By.CssSelector("#m_groupInfoDialog_m_groupRolesList_hierarchyRoleList_itemList"));
				List<IWebElement> allRows2 = table2.FindElements(By.CssSelector("tr:not([style*='display: none;'])")).ToList();
				if (allRows2.Count > 0)
				{
					for (int i = 0; i < allRows2.Count; i++)
					{
						wait.Until(ExpectedConditions.ElementToBeClickable(usermanagement.Btn_RoleAdd()));
						allRows2[i].Click();
					}
					if (SBrowserName.ToLower().Equals("internet explorer"))
						Click("cssselector", "input[id*='m_groupRolesList_Button_Add']", true);
					else
						usermanagement.Btn_RoleAdd().Click();
				}
				if (SBrowserName.ToLower().Equals("internet explorer"))
					Click("cssselector", "#m_groupInfoDialog_SaveAndViewButton", true);
				else
					usermanagement.SaveAndViewMyGroupBtn().Click();
				Thread.Sleep(100000);
				ExecutedSteps++;

				//Step 39 - Create user U6                
				eventcount_before = TestFixtures.GetEventCount();
				usermanagement = (UserManagement)login.Navigate("UserManagement");

				usermanagement.SearchGroup(CG1, D1, 1);
				usermanagement.SelectGroupByName(CG1);
				usermanagement.CreateUser(U6, FrontDesk, 1, Config.CustomUser2Email, 1, U6);

				eventcount_after = TestFixtures.GetEventCount();
				if (eventcount_after >= eventcount_before + 1)
					AuditLogList.Insert(3, true);
				else AuditLogList.Insert(3, false);
				ExecutedSteps++;

				//Step 40 - Select the triangle on the right hand side of the G1 in the Groups list and then select CG1
				usermanagement = (UserManagement)login.Navigate("UserManagement");
				usermanagement.SearchGroup(CG1, D1, 1);
				usermanagement.SelectGroupByName(CG1);
				if (usermanagement.IsUserExist(U6, D1))
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

				//Step 41 - Repeat the above step to add user "U7" to CG1 group with the role "Nurse".                
				usermanagement.CreateUser(U7, Nurse, 1, Config.CustomUser3Email, 1, U7);
				if (usermanagement.IsUserExist(U7, D1))
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

				//Step 42 - Select the User Management Tab
				usermanagement = (UserManagement)login.Navigate("UserManagement");
				ExecutedSteps++;

				//Step 43 - Select Domain D2 from the drop down list.
				usermanagement.SelectDomainFromDropdownList(D1);
				ExecutedSteps++;

				//Step 44 - Select the"New Group"Button
				ExecutedSteps++;

				//Step 45 - Create group G3                
				eventcount_before = TestFixtures.GetEventCount();
				usermanagement = (UserManagement)login.Navigate("UserManagement");
				usermanagement.SelectDomainFromDropdownList(D1);
				usermanagement.CreateGroup(D2, G3, password: U9, rolename: FrontDesk1, email: Email, IsManaged: 1, rolenames: new string[] { FrontDesk1 }, GroupUser: U9);
				ExecutedSteps++;

				//Step 46 - Create user U9                
				eventcount_after = TestFixtures.GetEventCount();
				if (eventcount_after >= eventcount_before + 1)
					AuditLogList.Insert(4, true);
				else AuditLogList.Insert(4, false);
				ExecutedSteps++;

				//Step 47 - Select Group Roles - select one or use the default
				ExecutedSteps++;

				//Step 48 - Logout and login by using Administrator account, select the User Management tab
				login.Logout();
				login.LoginIConnect(Config.adminUserName, Config.adminPassword);
				usermanagement = (UserManagement)login.Navigate("UserManagement");
				ExecutedSteps++;

				//Step 49 - Select group (G2) and click on Edit.
				usermanagement.SearchGroup(G2, D1, 1);
				usermanagement.SelectGroupByName(G2);
				ExecutedSteps++;

				//Step 50 - Try to modify different fields such as- group name, descriptions, Select Save.
				if (SBrowserName.ToLower().Equals("internet explorer"))
					Click("cssselector", "#EditGroupButton", true);
				else
					usermanagement.EditGrpBtn().Click();
				wait.Until(ExpectedConditions.ElementToBeClickable(usermanagement.CreateAndEditGroupPopupWindow()));
				PageLoadWait.WaitForProcessingState(10);
				usermanagement.GroupDescTxtBox().Clear();
				usermanagement.GroupDescTxtBox().SendKeys(G2 + " Description");
				if (SBrowserName.ToLower().Equals("internet explorer"))
					Click("cssselector", "#m_groupInfoDialog_SaveAndViewButton", true);
				else
					usermanagement.SaveAndViewMyGroupBtn().Click();
				Thread.Sleep(100000);
				ExecutedSteps++;

				//Step 51 - Repeat the above step for group G1.
				usermanagement.SearchGroup(G1, D1, 1);
				usermanagement.SelectGroupByName(G1);
				if (SBrowserName.ToLower().Equals("internet explorer"))
					Click("cssselector", "#EditGroupButton", true);
				else
					usermanagement.EditGrpBtn().Click();
				wait.Until(ExpectedConditions.ElementToBeClickable(usermanagement.CreateAndEditGroupPopupWindow()));
				PageLoadWait.WaitForProcessingState(10);
				usermanagement.GroupDescTxtBox().Clear();
				usermanagement.GroupDescTxtBox().SendKeys(G1 + " Description");
				if (SBrowserName.ToLower().Equals("internet explorer"))
					Click("cssselector", "#m_groupInfoDialog_SaveAndViewButton", true);
				else
					usermanagement.SaveAndViewMyGroupBtn().Click();
				Thread.Sleep(100000);
				ExecutedSteps++;

				//Step 52 - Select Group G4                
				eventcount_before = TestFixtures.GetEventCount();
				usermanagement = (UserManagement)login.Navigate("UserManagement");
				usermanagement.SelectDomainFromDropdownList(D1);
				usermanagement.SelectGroupByName(G4);
				ExecutedSteps++;

				//Step 53 - Select Delete button
				if (SBrowserName.ToLower().Equals("internet explorer"))
					Click("cssselector", "#DeleteGroupButton", true);
				else
					usermanagement.DelGrpBtn().Click();
				PageLoadWait.WaitForPageLoad(10);
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				if (BasePage.Driver.FindElement(By.CssSelector("#DialogDiv #ConfirmationDiv")).Displayed)
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

				//Setp 54 - Select OK
				PageLoadWait.WaitForPageLoad(20);
				if (SBrowserName.ToLower().Equals("internet explorer"))
					Click("cssselector", "#ctl00_ConfirmButton", true);
				else
					usermanagement.OkButtonConfirmGroupDeletionMsgBox().Click();

				eventcount_after = TestFixtures.GetEventCount();
				if (eventcount_after >= eventcount_before + 1)
					AuditLogList.Insert(5, true);
				else AuditLogList.Insert(5, false);
				ExecutedSteps++;

				//Logout
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
			finally
			{
				if (AuditLogList.Count > 0)
				{
					if (String.IsNullOrEmpty(AuditLogList[0].ToString()))
						AuditLogList.Insert(0, false);
				}
				else
				{
					AuditLogList.Insert(0, false);
				}

				if (AuditLogList.Count > 1)
				{
					if (String.IsNullOrEmpty(AuditLogList[1].ToString()))
						AuditLogList.Insert(1, false);
				}
				else
				{
					AuditLogList.Insert(1, false);
				}

				if (AuditLogList.Count > 2)
				{
					if (String.IsNullOrEmpty(AuditLogList[2].ToString()))
						AuditLogList.Insert(2, false);
				}
				else
				{
					AuditLogList.Insert(2, false);
				}

				if (AuditLogList.Count > 3)
				{
					if (String.IsNullOrEmpty(AuditLogList[3].ToString()))
						AuditLogList.Insert(3, false);
				}
				else
				{
					AuditLogList.Insert(3, false);
				}

				if (AuditLogList.Count > 4)
				{
					if (String.IsNullOrEmpty(AuditLogList[4].ToString()))
						AuditLogList.Insert(4, false);
				}
				else
				{
					AuditLogList.Insert(4, false);
				}

				if (AuditLogList.Count > 5)
				{
					if (String.IsNullOrEmpty(AuditLogList[5].ToString()))
						AuditLogList.Insert(5, false);
				}
				else
				{
					AuditLogList.Insert(5, false);
				}
			}
		}

		/// <summary>
		/// Downloader - Grant Study Access in the same Domain For Desktop only
		/// </summary>
		/// <param name="testid"></param>
		/// <param name="teststeps"></param>
		/// <param name="stepcount"></param>
		/// <returns></returns>
		//public TestCaseResult Test_161145(String testid, String teststeps, int stepcount)
		//{
		//    //Declare and initialize variables  
		//    TestCaseResult result;
		//    result = new TestCaseResult(stepcount);

		//    //Set up Validation Steps
		//    result.SetTestStepDescription(teststeps);
		//    int ExecutedSteps = -1;

		//    try
		//    {
		//        //Fetch required Test data    
		//        String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
		//        String[] Accession = AccessionList.Split(':');

		//        //Step 1 - Pre-conditions
		//        ExecutedSteps++;

		//        ////Step 2 - Pre-conditions
		//        //ExecutedSteps++;

		//        //Step 3 - Login as user U1  
		//        login.LoginIConnect(U1, U1);
		//        inbounds = (Inbounds)login.Navigate("Inbounds");
		//        inbounds.SearchStudy("lastname", "*");
		//        Dictionary<int, string[]> SearchResults = GetSearchResults();
		//        if (SearchResults.Count == 0 && !IsElementVisible(By.CssSelector("#m_grantAccessButton")))
		//        {
		//            result.steps[++ExecutedSteps].status = "Pass";
		//            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
		//        }
		//        else
		//        {
		//            result.steps[++ExecutedSteps].status = "Fail";
		//            Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
		//            result.steps[ExecutedSteps].SetLogs();
		//        }
		//        login.Logout();

		//        //Step 4 - Repeat the above steps for user U6.
		//        login.LoginIConnect(U6, U6);
		//        inbounds = (Inbounds)login.Navigate("Inbounds");
		//        inbounds.SearchStudy("lastname", "*");
		//        Dictionary<int, string[]> SearchResults1 = GetSearchResults();
		//        if (SearchResults1.Count == 0 && !IsElementVisible(By.CssSelector("#m_grantAccessButton")))
		//        {
		//            result.steps[++ExecutedSteps].status = "Pass";
		//            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
		//        }
		//        else
		//        {
		//            result.steps[++ExecutedSteps].status = "Fail";
		//            Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
		//            result.steps[ExecutedSteps].SetLogs();
		//        }
		//        login.Logout();

		//        //Step 5 - Login as U5.                
		//        login.LoginIConnect(U5, U5);
		//        inbounds = (Inbounds)login.Navigate("Inbounds");
		//        inbounds.SearchStudy("lastname", "*");
		//        Dictionary<int, string[]> SearchResults2 = GetSearchResults();
		//        bool isGrantAccessEnabledForU5 = IsElementVisible(By.CssSelector("#m_grantAccessButton"));
		//        login.Logout();

		//        login.LoginIConnect(U7, U7);
		//        inbounds = (Inbounds)login.Navigate("Inbounds");
		//        inbounds.SearchStudy("lastname", "*");
		//        Dictionary<int, string[]> SearchResults3 = GetSearchResults();
		//        bool isGrantAccessEnabledForU7 = IsElementVisible(By.CssSelector("#m_grantAccessButton"));

		//        if (SearchResults2.Count == 0 && isGrantAccessEnabledForU5 &&
		//            SearchResults3.Count == 0 && isGrantAccessEnabledForU7)
		//        {
		//            result.steps[++ExecutedSteps].status = "Pass";
		//            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
		//        }
		//        else
		//        {
		//            result.steps[++ExecutedSteps].status = "Fail";
		//            Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
		//            result.steps[ExecutedSteps].SetLogs();
		//        }
		//        login.Logout();

		//        //Step 6 - Grant Access to U1 and U5
		//        login.LoginIConnect(U4, U4);
		//        studies = (Studies)login.Navigate("Studies");
		//        studies.SearchStudy(AccessionNo: Accession[0]);
		//        studies.SelectStudy("Accession", Accession[0]);
		//        studies.ShareStudy(false, new String[] { U1, U5 });
		//        ExecutedSteps++;

		//        //Step 7 - Navigate to outbounds page and Confirm that the study granted to users U5 and U1 is displayed
		//        outbounds = (Outbounds)login.Navigate("Outbounds");
		//        outbounds.SearchStudy("lastname", "*");
		//        String[] values_6 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Accession", BasePage.GetColumnNames());
		//        if (Array.IndexOf(values_6, Accession[0]) > -1)
		//        {
		//            result.steps[++ExecutedSteps].status = "Pass";
		//            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
		//        }
		//        else
		//        {
		//            result.steps[++ExecutedSteps].status = "Fail";
		//            Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
		//            result.steps[ExecutedSteps].SetLogs();
		//        }

		//        //Step 8 - Repeat test steps 5 and 6 for two more studies.
		//        studies = (Studies)login.Navigate("Studies");
		//        studies.SearchStudy(AccessionNo: Accession[1]);
		//        studies.SelectStudy("Accession", Accession[1]);
		//        studies.ShareStudy(false, new String[] { U1, U5 });

		//        studies.SearchStudy(AccessionNo: Accession[2]);
		//        studies.SelectStudy("Accession", Accession[2]);
		//        studies.ShareStudy(false, new String[] { U1, U5 });

		//        outbounds = (Outbounds)login.Navigate("Outbounds");
		//        outbounds.SearchStudy("lastname", "*");
		//        String[] values_7 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Accession", BasePage.GetColumnNames());
		//        if (Array.IndexOf(values_7, Accession[0]) > -1 &&
		//            Array.IndexOf(values_7, Accession[1]) > -1 &&
		//            Array.IndexOf(values_7, Accession[2]) > -1)
		//        {
		//            result.steps[++ExecutedSteps].status = "Pass";
		//            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
		//        }
		//        else
		//        {
		//            result.steps[++ExecutedSteps].status = "Fail";
		//            Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
		//            result.steps[ExecutedSteps].SetLogs();
		//        }

		//        //Step 9 - Attempt to grant access a study with user U9 (G3; D2)
		//        studies = (Studies)login.Navigate("Studies");
		//        studies.SearchStudy(AccessionNo: Accession[1]);
		//        studies.SelectStudy("Accession", Accession[1]);
		//        GrantAccessBtn().Click();
		//        Driver.SwitchTo().DefaultContent();
		//        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#UserHomeFrame")));
		//        Driver.SwitchTo().Frame("UserHomeFrame");
		//        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DialogContentDiv")));
		//        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id$='StudySharingControl_m_userFilterInput']")));
		//        ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"[id$='StudySharingControl_m_userFilterInput']\").click()");
		//        Driver.FindElement(By.CssSelector("[id$=StudySharingControl_m_userFilterInput]")).Clear();
		//        Driver.FindElement(By.CssSelector("[id$=StudySharingControl_m_userFilterInput]")).SendKeys(U9);
		//        wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='StudySharingControl_Button_UserSearch']")));
		//        Driver.FindElement(By.CssSelector("[id$='StudySharingControl_Button_UserSearch']")).Click();
		//        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_StudySharingControl_LabelNoRecordsFoundForUser")));
		//        if (Driver.FindElement(By.CssSelector("#ctl00_StudySharingControl_LabelNoRecordsFoundForUser")).Text.Equals("No Records Found."))
		//        {
		//            result.steps[++ExecutedSteps].status = "Pass";
		//            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
		//        }
		//        else
		//        {
		//            result.steps[++ExecutedSteps].status = "Fail";
		//            Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
		//            result.steps[ExecutedSteps].SetLogs();
		//        }
		//        Driver.FindElement(By.CssSelector("#ctl00_StudySharingControl_CloseDialogButton")).Click();
		//        login.Logout();

		//        //Step 10 - Login as U5. Confirm that the study granted in the above step is displayed in the inbounds page.
		//        login.LoginIConnect(U5, U5);
		//        inbounds = (Inbounds)login.Navigate("Inbounds");
		//        inbounds.SearchStudy("lastname", "*");
		//        String[] values_9 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Accession", BasePage.GetColumnNames());
		//        inbounds.SearchStudy(AccessionNo: Accession[1]);
		//        inbounds.SelectStudy("Accession", Accession[1]);
		//        bool step_9 = false;
		//        BluRingViewer Bluringviewer = null;
		//        StudyViewer viewer = null;
		//        if (Config.isEnterpriseViewer.ToLower().Equals("y"))
		//        {
		//            Bluringviewer = BluRingViewer.LaunchBluRingViewer();
		//            result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
		//            step_9 = studies.CompareImage(result.steps[ExecutedSteps], Bluringviewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel));
		//        }
		//        else
		//        {
		//            viewer = studies.LaunchStudy();
		//            result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
		//            step_9 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
		//        }
		//        if (Array.IndexOf(values_9, Accession[0]) > -1 &&
		//            Array.IndexOf(values_9, Accession[0]) > -1 &&
		//            Array.IndexOf(values_9, Accession[0]) > -1 && step_9)
		//        {
		//            result.steps[ExecutedSteps].status = "Pass";
		//            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
		//        }
		//        else
		//        {
		//            result.steps[ExecutedSteps].status = "Fail";
		//            Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
		//            result.steps[ExecutedSteps].SetLogs();
		//        }
		//        if (Config.isEnterpriseViewer.ToLower().Equals("y"))
		//        {
		//            Bluringviewer.CloseBluRingViewer();
		//        }
		//        else
		//        {
		//            studies.CloseStudy();
		//        }
		//        login.Logout();

		//        //Step 11 - Login as U1. Confirm that the study granted in the above step is displayed in the inbounds page.
		//        login.LoginIConnect(U5, U5);
		//        inbounds = (Inbounds)login.Navigate("Inbounds");
		//        inbounds.SearchStudy("lastname", "*");
		//        String[] values_10 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Accession", BasePage.GetColumnNames());
		//        inbounds.SearchStudy(AccessionNo: Accession[1]);
		//        inbounds.SelectStudy("Accession", Accession[1]);
		//        bool step_10 = false;
		//        if (Config.isEnterpriseViewer.ToLower().Equals("y"))
		//        {
		//            BluRingViewer.LaunchBluRingViewer();
		//            result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
		//            step_10 = studies.CompareImage(result.steps[ExecutedSteps], Bluringviewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel));
		//        }
		//        else
		//        {
		//            studies.LaunchStudy();
		//            result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
		//            step_10 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
		//        }
		//        if (Array.IndexOf(values_10, Accession[0]) > -1 &&
		//            Array.IndexOf(values_10, Accession[1]) > -1 &&
		//            Array.IndexOf(values_10, Accession[2]) > -1 && step_10)
		//        {
		//            result.steps[ExecutedSteps].status = "Pass";
		//            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
		//        }
		//        else
		//        {
		//            result.steps[ExecutedSteps].status = "Fail";
		//            Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
		//            result.steps[ExecutedSteps].SetLogs();
		//        }
		//        if (Config.isEnterpriseViewer.ToLower().Equals("y"))
		//        {
		//            Bluringviewer.CloseBluRingViewer();
		//        }
		//        else
		//        {
		//            studies.CloseStudy();
		//        }
		//        login.Logout();

		//        //Step 12 - Login as U7. Select a study. Grant an access to this study to the user U6.
		//        login.LoginIConnect(U7, U7);
		//        studies = (Studies)login.Navigate("Studies");
		//        studies.SearchStudy(AccessionNo: Accession[3]);
		//        studies.SelectStudy("Accession", Accession[3]);
		//        studies.ShareStudy(false, new String[] { U6 });
		//        result.steps[++ExecutedSteps].status = "Partially Automated";

		//        //Step 13 - Select another study on the Study List and attempt to grant access this study to the user U5.
		//        studies.SearchStudy(AccessionNo: Accession[4]);
		//        studies.SelectStudy("Accession", Accession[4]);
		//        GrantAccessBtn().Click();
		//        Driver.SwitchTo().DefaultContent();
		//        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#UserHomeFrame")));
		//        Driver.SwitchTo().Frame("UserHomeFrame");
		//        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DialogContentDiv")));
		//        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id$='StudySharingControl_m_userFilterInput']")));
		//        ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"[id$='StudySharingControl_m_userFilterInput']\").click()");
		//        Driver.FindElement(By.CssSelector("[id$=StudySharingControl_m_userFilterInput]")).Clear();
		//        Driver.FindElement(By.CssSelector("[id$=StudySharingControl_m_userFilterInput]")).SendKeys(U7);
		//        wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='StudySharingControl_Button_UserSearch']")));
		//        Driver.FindElement(By.CssSelector("[id$='StudySharingControl_Button_UserSearch']")).Click();
		//        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_StudySharingControl_LabelNoRecordsFoundForUser")));
		//        if (Driver.FindElement(By.CssSelector("#ctl00_StudySharingControl_LabelNoRecordsFoundForUser")).Text.Equals("No Records Found."))
		//        {
		//            result.steps[++ExecutedSteps].status = "Pass";
		//            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
		//        }
		//        else
		//        {
		//            result.steps[++ExecutedSteps].status = "Fail";
		//            Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
		//            result.steps[ExecutedSteps].SetLogs();
		//        }
		//        Driver.FindElement(By.CssSelector("#ctl00_StudySharingControl_CloseDialogButton")).Click();

		//        //Step 14 - Navigate to outbounds page and Confirm that the study granted to user U6 is displayed
		//        outbounds = (Outbounds)login.Navigate("Outbounds");
		//        outbounds.SearchStudy("lastname", "*");
		//        String[] values_13 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Accession", BasePage.GetColumnNames());
		//        if (Array.IndexOf(values_13, Accession[3]) > -1)
		//        {
		//            result.steps[++ExecutedSteps].status = "Pass";
		//            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
		//        }
		//        else
		//        {
		//            result.steps[++ExecutedSteps].status = "Fail";
		//            Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
		//            result.steps[ExecutedSteps].SetLogs();
		//        }
		//        login.Logout();

		//        //Step 15 - Login as U6 and confirm that the study granted above is displayed on the Inbounds page.
		//        login.LoginIConnect(U6, U6);
		//        inbounds = (Inbounds)login.Navigate("Inbounds");
		//        inbounds.SearchStudy("lastname", "*");
		//        String[] values_14 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Accession", BasePage.GetColumnNames());
		//        inbounds.SearchStudy(AccessionNo: Accession[3]);
		//        inbounds.SelectStudy("Accession", Accession[3]);
		//        bool step_14 = false;
		//        if (Config.isEnterpriseViewer.ToLower().Equals("y"))
		//        {
		//            BluRingViewer.LaunchBluRingViewer();
		//            result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
		//            step_14 = studies.CompareImage(result.steps[ExecutedSteps], Bluringviewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel));
		//        }
		//        else
		//        {
		//            studies.LaunchStudy();
		//            result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
		//            step_14 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
		//        }
		//        if (Array.IndexOf(values_14, Accession[3]) > -1 && step_14)
		//        {
		//            result.steps[ExecutedSteps].status = "Pass";
		//            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
		//        }
		//        else
		//        {
		//            result.steps[ExecutedSteps].status = "Fail";
		//            Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
		//            result.steps[ExecutedSteps].SetLogs();
		//        }
		//        if (Config.isEnterpriseViewer.ToLower().Equals("y"))
		//        {
		//            Bluringviewer.CloseBluRingViewer();
		//        }
		//        else
		//        {
		//            studies.CloseStudy();
		//        }
		//        login.Logout();

		//        //Step 16 - Login as U4. Select a study that does not match the Ref.Physician filter created for User U7.
		//        login.LoginIConnect(U4, U4);
		//        studies = (Studies)login.Navigate("Studies");
		//        studies.SearchStudy(AccessionNo: Accession[4]);
		//        studies.SelectStudy("Accession", Accession[4]);
		//        studies.ShareStudy(false, groups: new String[] { CG1 });
		//        result.steps[++ExecutedSteps].status = "Partially Automated";

		//        //Step 17 - Open Outbouds list, Confirm that the study granted to CG1 group is displayed.
		//        outbounds = (Outbounds)login.Navigate("Outbounds");
		//        outbounds.SearchStudy("lastname", "*");
		//        String[] values_16 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Accession", BasePage.GetColumnNames());
		//        if (Array.IndexOf(values_16, Accession[4]) > -1)
		//        {
		//            result.steps[++ExecutedSteps].status = "Pass";
		//            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
		//        }
		//        else
		//        {
		//            result.steps[++ExecutedSteps].status = "Fail";
		//            Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
		//            result.steps[ExecutedSteps].SetLogs();
		//        }
		//        login.Logout();

		//        //Step 18 - Login as each of the users of CG1 group (U6 and U7) and confirm the granted study is displayed on the Inbounds list
		//        login.LoginIConnect(U6, U6);
		//        inbounds = (Inbounds)login.Navigate("Inbounds");
		//        inbounds.SearchStudy("lastname", "*");
		//        String[] values_17_U6 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Accession", BasePage.GetColumnNames());
		//        login.Logout();

		//        login.LoginIConnect(U7, U7);
		//        inbounds = (Inbounds)login.Navigate("Inbounds");
		//        inbounds.SearchStudy("lastname", "*");
		//        String[] values_17_U7 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Accession", BasePage.GetColumnNames());
		//        login.Logout();

		//        if (Array.IndexOf(values_17_U6, Accession[4]) > -1 &&
		//            Array.IndexOf(values_17_U7, Accession[4]) > -1)
		//        {
		//            result.steps[++ExecutedSteps].status = "Pass";
		//            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
		//        }
		//        else
		//        {
		//            result.steps[++ExecutedSteps].status = "Fail";
		//            Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
		//            result.steps[ExecutedSteps].SetLogs();
		//        }
		//        login.Logout();

		//        //Step 19 - Log in as U4 user
		//        login.LoginIConnect(U4, U4);
		//        ExecutedSteps++;

		//        //Step 20 - Select the Outbounds tab
		//        outbounds = (Outbounds)login.Navigate("Outbounds");
		//        outbounds.SearchStudy("lastname", "*");
		//        String[] values_19 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Accession", BasePage.GetColumnNames());
		//        if (Array.IndexOf(values_19, Accession[0]) > -1 &&
		//            Array.IndexOf(values_19, Accession[4]) > -1)
		//        {
		//            result.steps[++ExecutedSteps].status = "Pass";
		//            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
		//        }
		//        else
		//        {
		//            result.steps[++ExecutedSteps].status = "Fail";
		//            Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
		//            result.steps[ExecutedSteps].SetLogs();
		//        }

		//        //Step 21 - Select one study (that has been granted with the user U5), click on the 'Remove Access' button
		//        outbounds.SearchStudy(AccessionNo: Accession[4]);
		//        outbounds.SelectStudy("Accession", Accession[4]);
		//        ExecutedSteps++;

		//        //Step 22 - Select U5, click OK button.
		//        outbounds.RemoveAccess(new String[] { CG1 }, 1);
		//        ExecutedSteps++;

		//        //Step 23 - Verify that an email is sent to the U5.
		//        result.steps[++ExecutedSteps].status = "Not Automated";

		//        //Step 24 - Log out of user U4 and Log in as user U5
		//        login.Logout();
		//        login.LoginIConnect(U5, U5);
		//        ExecutedSteps++;

		//        //Step 25 - Select the Inbounds tab
		//        inbounds = (Inbounds)login.Navigate("Inbounds");
		//        inbounds.SearchStudy("lastname", "*");
		//        String[] values_24 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Accession", BasePage.GetColumnNames());
		//        login.Logout();

		//        if (!(Array.IndexOf(values_24, Accession[4]) > -1))
		//        {
		//            result.steps[++ExecutedSteps].status = "Pass";
		//            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
		//        }
		//        else
		//        {
		//            result.steps[++ExecutedSteps].status = "Fail";
		//            Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
		//            result.steps[ExecutedSteps].SetLogs();
		//        }
		//        login.Logout();

		//        //Step 26 - Confirm that grant access duration of 1 day has been configured in Domain Management page of Administrator account
		//        login.LoginIConnect(D1, D1);
		//        domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
		//        PageLoadWait.WaitForFrameLoad(10);
		//        Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
		//        domainmanagement.GrantAccessValidDaysTxtBox().Clear();
		//        domainmanagement.GrantAccessValidDaysTxtBox().SendKeys("1");
		//        PageLoadWait.WaitForFrameLoad(20);
		//        this.ClickElement(Driver.FindElement(By.CssSelector("[id$='EditDomainControl_SaveButton']")));
		//        PageLoadWait.WaitForPageLoad(10);
		//        this.ClickElement(BasePage.Driver.FindElement(By.CssSelector("div[id='ModalDialogDiv'] input[name='CloseButton']")));
		//        login.Logout();
		//        ExecutedSteps++;

		//        //Step 27 - Login as U4, Select a study and grant access to U5. Login as U5, go to Inbounds tab
		//        login.LoginIConnect(U4, U4);
		//        studies = (Studies)login.Navigate("Studies");
		//        studies.SearchStudy(AccessionNo: Accession[5]);
		//        studies.SelectStudy("Accession", Accession[5]);
		//        studies.ShareStudy(false, new String[] { U5 });
		//        login.Logout();

		//        login.LoginIConnect(U5, U5);
		//        inbounds = (Inbounds)login.Navigate("Inbounds");
		//        inbounds.SearchStudy("lastname", "*");
		//        String[] values_26 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Accession", BasePage.GetColumnNames());
		//        if (Array.IndexOf(values_26, Accession[5]) > -1)
		//        {
		//            result.steps[++ExecutedSteps].status = "Pass";
		//            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
		//        }
		//        else
		//        {
		//            result.steps[++ExecutedSteps].status = "Fail";
		//            Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
		//            result.steps[ExecutedSteps].SetLogs();
		//        }
		//        login.Logout();

		//        //Step 28 - Confirm the access granted duration to U5 has exceeded one day. Login as U5, go to Inbounds tab.
		//        result.steps[++ExecutedSteps].status = "Not Automated";

		//        //Report Result
		//        result.FinalResult(ExecutedSteps);
		//        Logger.Instance.ErrorLog("Overall Test status--" + result.status);

		//        //Return Result
		//        return result;
		//    }
		//    catch (Exception e)
		//    {
		//        //Log Exception
		//        Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

		//        //Report Result
		//        result.FinalResult(e, ExecutedSteps);
		//        Logger.Instance.ErrorLog("Overall Test status--" + result.status);

		//        //Return Result
		//        return result;
		//    }
		//}
		public TestCaseResult Test_161145(String testid, String teststeps, int stepcount)
		{
			//Declare and initialize variables  
			TestCaseResult result;
			result = new TestCaseResult(stepcount);

			//Set up Validation Steps
			result.SetTestStepDescription(teststeps);
			int ExecutedSteps = -1;

			try
			{
				EmailUtils customEmail = new EmailUtils() { EmailId = Config.CustomUser1Email, Password = Config.CustomUserEmailPassword };
				customEmail.MarkAllMailAsRead("INBOX");
				EmailUtils customEmail1 = new EmailUtils() { EmailId = Config.CustomUser2Email, Password = Config.CustomUserEmailPassword };
				customEmail1.MarkAllMailAsRead("INBOX");
				EmailUtils customEmail2 = new EmailUtils() { EmailId = Config.CustomUser3Email, Password = Config.CustomUserEmailPassword };
				customEmail2.MarkAllMailAsRead("INBOX");

				//Fetch required Test data    
				String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
				String[] Accession = AccessionList.Split(':');

				//Step 1 - Pre-conditions
				if (U1 == null)
				{
					String testdescription = "";
					teststeps = GetTestSteps("Test_27571", "ExternalApplicationDownloader", out testid, out testdescription);
					stepcount = teststeps.Split('=')[0].Split(':').Length;
					Test_27571("27571", teststeps, stepcount);
				}
				ExecutedSteps++;

				//Step 2 - Login as user U1  
				login.LoginIConnect(U1, U1);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("lastname", "*");
				Dictionary<int, string[]> SearchResults = GetSearchResults();
				if (SearchResults.Count == 0 && !IsElementVisible(By.CssSelector("#m_grantAccessButton")))
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

				//Step 3 - Repeat the above steps for user U6.
				login.LoginIConnect(U6, U6);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("lastname", "*");
				Dictionary<int, string[]> SearchResults1 = GetSearchResults();
				if (SearchResults1.Count == 0 && !IsElementVisible(By.CssSelector("#m_grantAccessButton")))
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

				//Step 4 - Login as U5.                
				login.LoginIConnect(U5, U5);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("lastname", "*");
				Dictionary<int, string[]> SearchResults2 = GetSearchResults();
				bool isGrantAccessEnabledForU5 = IsElementVisible(By.CssSelector("#m_grantAccessButton"));
				login.Logout();

				login.LoginIConnect(U7, U7);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("lastname", "*");
				Dictionary<int, string[]> SearchResults3 = GetSearchResults();
				bool isGrantAccessEnabledForU7 = IsElementVisible(By.CssSelector("#m_grantAccessButton"));

				if (SearchResults2.Count == 0 && isGrantAccessEnabledForU5 &&
					SearchResults3.Count == 0 && isGrantAccessEnabledForU7)
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

				//Step 5 - Grant Access to U1 and U5
				login.LoginIConnect(U4, U4);
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(AccessionNo: Accession[0], Study_Performed_Period: "All Dates");
				studies.SelectStudy("Accession", Accession[0]);
				studies.ShareStudy(false, new String[] { U1, U5 });
				ExecutedSteps++;

				//Step 6 - Navigate to outbounds page and Confirm that the study granted to users U5 and U1 is displayed
				outbounds = (Outbounds)login.Navigate("Outbounds");
				outbounds.SearchStudy("lastname", "*");
				String[] values_6 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Accession", BasePage.GetColumnNames());
				if (Array.IndexOf(values_6, Accession[0]) > -1)
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

				//Step 7 - Repeat test steps 5 and 6 for two more studies.
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(AccessionNo: Accession[1]);
				studies.SelectStudy("Accession", Accession[1]);
				studies.ShareStudy(false, new String[] { U1, U5 });

				studies.SearchStudy(AccessionNo: Accession[2]);
				studies.SelectStudy("Accession", Accession[2]);
				studies.ShareStudy(false, new String[] { U1, U5 });

				outbounds = (Outbounds)login.Navigate("Outbounds");
				outbounds.SearchStudy("lastname", "*");
				String[] values_8 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Accession", BasePage.GetColumnNames());
				if (Array.IndexOf(values_8, Accession[0]) > -1 &&
					Array.IndexOf(values_8, Accession[1]) > -1 &&
					Array.IndexOf(values_8, Accession[2]) > -1)
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

				//Step 8 - Attempt to grant access a study with user U9 (G3; D2)
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(AccessionNo: Accession[1], Study_Performed_Period: "All Dates");
				studies.SelectStudy("Accession", Accession[1]);
				if (SBrowserName.ToLower().Equals("internet explorer"))
					Click("cssselector", "input[id$='m_grantAccessButton']", true);
				else
					GrantAccessBtn().Click();
				Driver.SwitchTo().DefaultContent();
				wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#UserHomeFrame")));
				Driver.SwitchTo().Frame("UserHomeFrame");
				wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DialogContentDiv")));
				wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id$='StudySharingControl_m_userFilterInput']")));
				((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"[id$='StudySharingControl_m_userFilterInput']\").click()");
				Driver.FindElement(By.CssSelector("[id$=StudySharingControl_m_userFilterInput]")).Clear();
				Driver.FindElement(By.CssSelector("[id$=StudySharingControl_m_userFilterInput]")).SendKeys(U9);
				wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='StudySharingControl_Button_UserSearch']")));
				if (SBrowserName.ToLower().Equals("internet explorer"))
					Click("cssselector", "[id$='StudySharingControl_Button_UserSearch']", true);
				else
					Driver.FindElement(By.CssSelector("[id$='StudySharingControl_Button_UserSearch']")).Click();
				wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_StudySharingControl_LabelNoRecordsFoundForUser")));
				if (Driver.FindElement(By.CssSelector("#ctl00_StudySharingControl_LabelNoRecordsFoundForUser")).Text.Equals("No Records Found."))
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
				if (SBrowserName.ToLower().Equals("internet explorer"))
					Click("cssselector", "#ctl00_StudySharingControl_CloseDialogButton", true);
				else
					Driver.FindElement(By.CssSelector("#ctl00_StudySharingControl_CloseDialogButton")).Click();
				login.Logout();

				//Step 9 - Login as U5. Confirm that the study granted in the above step is displayed in the inbounds page.
				login.LoginIConnect(U5, U5);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("lastname", "*");
                String[] values_9 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Accession", BasePage.GetColumnNames());
				inbounds.SearchStudy(AccessionNo: Accession[1]);
				inbounds.SelectStudy("Accession", Accession[1]);
                bool step_9 = false;
                BluRingViewer Bluringviewer = null;
                StudyViewer viewer = null;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    Bluringviewer = BluRingViewer.LaunchBluRingViewer();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    step_9 = studies.CompareImage(result.steps[ExecutedSteps], Bluringviewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel));
                }
                else
                {
				viewer = studies.LaunchStudy();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    step_9 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                }
                if (Array.IndexOf(values_9, Accession[0]) > -1 &&
                    Array.IndexOf(values_9, Accession[0]) > -1 &&
                    Array.IndexOf(values_9, Accession[0]) > -1 && step_9)
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
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    Bluringviewer.CloseBluRingViewer();
                }
                else
                {
				studies.CloseStudy();
                }
				login.Logout();

				//Step 10 - Login as U1. Confirm that the study granted in the above step is displayed in the inbounds page.
				login.LoginIConnect(U1, U1);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("lastname", "*");
                String[] values_10 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Accession", BasePage.GetColumnNames());
				inbounds.SearchStudy(AccessionNo: Accession[1]);
				inbounds.SelectStudy("Accession", Accession[1]);
                bool step_10 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    BluRingViewer.LaunchBluRingViewer();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    step_10 = studies.CompareImage(result.steps[ExecutedSteps], Bluringviewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel));
                }
                else
                {
                    studies.LaunchStudy();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    step_10 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                }
                if (Array.IndexOf(values_10, Accession[0]) > -1 &&
                    Array.IndexOf(values_10, Accession[1]) > -1 &&
                    Array.IndexOf(values_10, Accession[2]) > -1 && step_10)
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
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    Bluringviewer.CloseBluRingViewer();
                }
                else
                {
				studies.CloseStudy();
                }
				login.Logout();

				//Step 11 - Login as U7. Select a study. Grant an access to this study to the user U6.
				login.LoginIConnect(U7, U7);
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(AccessionNo: Accession[3]);
				studies.SelectStudy("Accession", Accession[3]);
				studies.ShareStudy(false, new String[] { U6 });
				Dictionary<string, string> downloadedMail = customEmail1.GetMailUsingIMAP(Config.SystemEmail, "Shared Study", maxWaitTime: 5);
				string emailLink = customEmail1.GetEmailedStudyLink(downloadedMail);
				Logger.Instance.InfoLog("Email link--" + emailLink);
				if (emailLink != null && downloadedMail.Count > 0)
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
				//result.steps[++ExecutedSteps].status = "Partially Automated";

				//Step 12 - Select another study on the Study List and attempt to grant access this study to the user U5.
				studies.SearchStudy(AccessionNo: Accession[4]);
				studies.SelectStudy("Accession", Accession[4]);
				if (SBrowserName.ToLower().Equals("internet explorer"))
					Click("cssselector", "input[id$='m_grantAccessButton']", true);
				else
					GrantAccessBtn().Click();
				Driver.SwitchTo().DefaultContent();
				wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#UserHomeFrame")));
				Driver.SwitchTo().Frame("UserHomeFrame");
				wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DialogContentDiv")));
				wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id$='StudySharingControl_m_userFilterInput']")));
				((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"[id$='StudySharingControl_m_userFilterInput']\").click()");
				Driver.FindElement(By.CssSelector("[id$=StudySharingControl_m_userFilterInput]")).Clear();
				Driver.FindElement(By.CssSelector("[id$=StudySharingControl_m_userFilterInput]")).SendKeys(U7);
				wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='StudySharingControl_Button_UserSearch']")));
				if (SBrowserName.ToLower().Equals("internet explorer"))
					Click("cssselector", "[id$='StudySharingControl_Button_UserSearch']", true);
				else
					Driver.FindElement(By.CssSelector("[id$='StudySharingControl_Button_UserSearch']")).Click();
				wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_StudySharingControl_LabelNoRecordsFoundForUser")));
				if (Driver.FindElement(By.CssSelector("#ctl00_StudySharingControl_LabelNoRecordsFoundForUser")).Text.Equals("No Records Found."))
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
				if (SBrowserName.ToLower().Equals("internet explorer"))
					Click("cssselector", "#ctl00_StudySharingControl_CloseDialogButton", true);
				else
					Driver.FindElement(By.CssSelector("#ctl00_StudySharingControl_CloseDialogButton")).Click();

				//Step 13 - Navigate to outbounds page and Confirm that the study granted to user U6 is displayed
				outbounds = (Outbounds)login.Navigate("Outbounds");
				outbounds.SearchStudy("lastname", "*");
				String[] values_13 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Accession", BasePage.GetColumnNames());
				if (Array.IndexOf(values_13, Accession[3]) > -1)
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
				login.Logout();

				//Step 14 - Login as U6 and confirm that the study granted above is displayed on the Inbounds page.
				login.LoginIConnect(U6, U6);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("lastname", "*");
                String[] values_14 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Accession", BasePage.GetColumnNames());
				inbounds.SearchStudy(AccessionNo: Accession[3]);
				inbounds.SelectStudy("Accession", Accession[3]);
                bool step_14 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    BluRingViewer.LaunchBluRingViewer();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    step_14 = studies.CompareImage(result.steps[ExecutedSteps], Bluringviewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel));
                }
                else
                {
                    studies.LaunchStudy();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    step_14 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                }
                if (Array.IndexOf(values_14, Accession[3]) > -1 && step_14)
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
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    Bluringviewer.CloseBluRingViewer();
                }
                else
                {
                    studies.CloseStudy();
                }
				login.Logout();

				//Step 15 - Login as U4. Select a study that does not match the Ref.Physician filter created for User U7.
				login.LoginIConnect(U4, U4);
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(AccessionNo: Accession[4]);
				studies.SelectStudy("Accession", Accession[4]);
				studies.ShareStudy(false, groups: new String[] { CG1 });
				Dictionary<string, string> downloadedMail1 = customEmail2.GetMailUsingIMAP(Config.SystemEmail, "Shared Study", maxWaitTime: 5);
				string emailLink1 = customEmail2.GetEmailedStudyLink(downloadedMail1);
				Logger.Instance.InfoLog("Email link--" + emailLink);
				Dictionary<string, string> downloadedMail_u6 = customEmail1.GetMailUsingIMAP(Config.SystemEmail, "Shared Study", maxWaitTime: 5);
				string emailLink_u6 = customEmail1.GetEmailedStudyLink(downloadedMail_u6);
				Logger.Instance.InfoLog("Email link--" + emailLink_u6);
				if (emailLink1 != null && downloadedMail1.Count > 0 && downloadedMail_u6.Count > 0 && emailLink_u6 != null)
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
				// result.steps[++ExecutedSteps].status = "Partially Automated";

				//Step 16 - Open Outbouds list, Confirm that the study granted to CG1 group is displayed.
				outbounds = (Outbounds)login.Navigate("Outbounds");
				outbounds.SearchStudy("lastname", "*");
				String[] values_17 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Accession", BasePage.GetColumnNames());
				if (Array.IndexOf(values_17, Accession[4]) > -1)
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
				login.Logout();

				//Step 17 - Login as each of the users of CG1 group (U6 and U7) and confirm the granted study is displayed on the Inbounds list
				login.LoginIConnect(U6, U6);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("lastname", "*");
				String[] values_18_U6 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Accession", BasePage.GetColumnNames());
				login.Logout();

				login.LoginIConnect(U7, U7);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("lastname", "*");
				String[] values_18_U7 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Accession", BasePage.GetColumnNames());
				login.Logout();

				if (Array.IndexOf(values_18_U6, Accession[4]) > -1 &&
					Array.IndexOf(values_18_U7, Accession[4]) > -1)
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

				//Step 18 - Log in as U4 user
				login.LoginIConnect(U4, U4);
				ExecutedSteps++;

				//Step 19 - Select the Outbounds tab
				outbounds = (Outbounds)login.Navigate("Outbounds");
				outbounds.SearchStudy("lastname", "*");
				String[] values_20 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Accession", BasePage.GetColumnNames());
				if (Array.IndexOf(values_20, Accession[0]) > -1 &&
					Array.IndexOf(values_20, Accession[4]) > -1)
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

				//Step 20 - Select one study (that has been granted with the user U5), click on the 'Remove Access' button
				outbounds.SearchStudy(AccessionNo: Accession[4]);
				outbounds.SelectStudy("Accession", Accession[4]);
				ExecutedSteps++;

				//Step 21 - Select U5, click OK button.
				outbounds.RemoveAccess(new String[] { CG1 }, 1);
				ExecutedSteps++;

				//Step 22 - Verify that an email is sent to the U5.
				Dictionary<string, string> downloadedMail2 = customEmail.GetMailUsingIMAP(Config.SystemEmail, "Shared Study", maxWaitTime: 5);
				if (downloadedMail2.Count > 0)
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
				//result.steps[++ExecutedSteps].status = "Not Automated";

				//Step 23 - Log out of user U4 and Log in as user U5
				login.Logout();
				login.LoginIConnect(U5, U5);
				ExecutedSteps++;

				//Step 24 - Select the Inbounds tab
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("lastname", "*");
				String[] values_25 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Accession", BasePage.GetColumnNames());
				login.Logout();

				if (!(Array.IndexOf(values_25, Accession[4]) > -1))
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

				//Step 25 - Confirm that grant access duration of 1 day has been configured in Domain Management page of Administrator account
				login.LoginIConnect(D1, D1);
				domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
				PageLoadWait.WaitForFrameLoad(10);
				Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
				domainmanagement.GrantAccessValidDaysTxtBox().Clear();
				domainmanagement.GrantAccessValidDaysTxtBox().SendKeys("1");
				PageLoadWait.WaitForFrameLoad(20);
				this.ClickElement(Driver.FindElement(By.CssSelector("[id$='EditDomainControl_SaveButton']")));
				PageLoadWait.WaitForPageLoad(10);
				this.ClickElement(BasePage.Driver.FindElement(By.CssSelector("div[id='ModalDialogDiv'] input[name='CloseButton']")));
				login.Logout();
				ExecutedSteps++;

				//Step 26 - Login as U4, Select a study and grant access to U5. Login as U5, go to Inbounds tab
				CurrentTime = DateTime.Now.ToString("hh:mm:ss tt");
				string time = "01:00:00";
				BasePage.RunBatchFile(updatedateandtimebatchfile, "time" + " " + time);

				login.LoginIConnect(U4, U4);
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(AccessionNo: Accession[5]);
				studies.SelectStudy("Accession", Accession[5]);
				studies.ShareStudy(false, new String[] { U5 });
				login.Logout();

				login.LoginIConnect(U5, U5);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("lastname", "*");
				String[] values_26 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Accession", BasePage.GetColumnNames());
				if (Array.IndexOf(values_26, Accession[5]) > -1)
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
				login.Logout();

				//Step 27 - Confirm the access granted duration to U5 has exceeded one day. Login as U5, go to Inbounds tab.				
				DateTime localTime = DateTime.Now.AddHours(1);
				DateTime localDate = DateTime.Now.AddDays(1);
				string Time = localTime.ToString("hh:mm:ss tt");
				string Date = localDate.ToString("MM/dd/yyyy");
				BasePage.RunBatchFile(updatedateandtimebatchfile, "date" + " " + Date);
				BasePage.RunBatchFile(updatedateandtimebatchfile, "time" + " " + Time);

				login.LoginIConnect(U5, U5);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("lastname", "*");
				String[] values_27 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Accession", BasePage.GetColumnNames());
				if (values_27.Any(acc => acc.Equals(Accession[5])))
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}

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
			finally
			{
				try

				{
					BasePage.RunBatchFile(updatedateandtimebatchfile, "date" + " " + CurrentDate);
					BasePage.RunBatchFile(updatedateandtimebatchfile, "time" + " " + CurrentTime);
				}

				catch (Exception) { }
			}
		}

		/// <summary>
		/// Downloader - Initial Setups (LDAP user database)
		/// </summary>
		/// <param name="testid"></param>
		/// <param name="teststeps"></param>
		/// <param name="stepcount"></param>
		/// <returns></returns>
		public TestCaseResult Test_27575(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data               
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");

                String[] Accession = AccessionList.Split(':');

                WebDriverWait wait = new WebDriverWait(Driver, new TimeSpan(0, 0, 120));

                //Step 1 - Pre-conditions
                ExecutedSteps++;

                //Step 2 - Download file will be a zip file
                login.LoginIConnect(U4, U4);
                login.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                if (BasePage.Driver.FindElement(By.CssSelector("#DownloadRadioButtonList_0")).Selected)
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
                login.CloseUserPreferences();
                //ExecutedSteps++;

                //Step 3 - Setting Package expire interval in Service tool.               
                ExecutedSteps++;

                //Step 4 - Select the Studies Tab    
                studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //Step 5 - Select 'Local System' from the drop down Transfer to: field. And click on Transfer button.
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);

                Driver.FindElement(By.CssSelector("div#ButtonsDiv table td>div>input#m_transferButton")).Click();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                Driver.FindElement(By.CssSelector("div.dialog_content div>input#ctl00_StudyTransferControl_m_relatedStudiesToggleButton")).Click();

                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                Driver.FindElement(By.CssSelector("div#DestinationListDiv select#ctl00_StudyTransferControl_m_destinationSources>option[value='-1']")).Click();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                Driver.FindElement(By.CssSelector("div.dialog_content input#ctl00_StudyTransferControl_TransferButton")).Click();
                ExecutedSteps++;

                //Step 6 - Click on Submit
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                Driver.FindElement(By.CssSelector("div#dataQCDiv input#ctl00_DataQCControl_m_submitButton")).Click();
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(11)>span[title*='Ready']")));
                ExecutedSteps++;

                //Step 7 - Select one studies with Status = Ready
                BasePage.Driver.FindElement(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(11)>span[title*='Ready']")).Click();
                ExecutedSteps++;

                //Step 8 - Click on the download button.                
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#ctl00_TransferJobsListControl_m_submitButton")));
                BasePage.Driver.FindElement(By.CssSelector("input#ctl00_TransferJobsListControl_m_submitButton")).Click();
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#ctl00_TransferJobPackagesListControl_m_submitButton")));
                ExecutedSteps++;

                //Step 9 - Click on the Download Button
                BasePage.Driver.FindElement(By.CssSelector("input#ctl00_TransferJobPackagesListControl_m_submitButton")).Click();
                ExecutedSteps++;

                //Step 10 - Click on Save As
                String description;
                studies.GetMatchingRow("Accession", Accession[0]).TryGetValue("Description", out description);
                PageLoadWait.WaitForDownload("_" + description, Config.downloadpath, "zip");
                ExecutedSteps++;

                //Step 11 - Select a location to save the Study, and click save  
                Boolean studydownloaded = BasePage.CheckFile("_" + description, Config.downloadpath, "zip");
                if (studydownloaded == true)
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
                //ExecutedSteps++;

                //Step 12 - Click on close               
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#ctl00_TransferJobPackagesListControl_m_closeDialogButton")));
                BasePage.Driver.FindElement(By.CssSelector("input#ctl00_TransferJobPackagesListControl_m_closeDialogButton")).Click();
                ExecutedSteps++;

                //Step 13 - Select close to end download.    
                PageLoadWait.WaitForPageLoad(20);
                ExecutedSteps++;

                //Step 14 - Select Options --*^>^* Transfer status
                studies.TransferStatus();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                new WebDriverWait(BasePage.Driver, new TimeSpan(0, 1, 0)).Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(11)>span")));
                string status = BasePage.Driver.FindElement(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(11)>span")).GetAttribute("title");
                if (status == "Ready" || status == "Done" || status == "Expired")
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
                studies.TransferStatusClose();

                //Step 15 - Go to the location where ZIP file was saved and open the ZIP file, unzip the files to a local folder. 
                //          then open the DicomDir file in the DicomTool.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 16 - Use a third party viewer (e.g. efilm viewer) to open the study just unzipped and view it.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 17 - Verify the received study
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 18 - Wait 5 min viewing the Transfer Status window. After the time observe the status.
                Thread.Sleep(200000);
                studies.TransferStatus();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(11)>span[title*='Expired']")));
                string ExpiredStatus = BasePage.Driver.FindElement(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(11)>span")).GetAttribute("title");
                if (ExpiredStatus == "Expired")
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
                studies.TransferStatusClose();

                //Step 19 - Select the expired study
                studies.SearchStudy(AccessionNo: Accession[1], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[1]);

                Driver.FindElement(By.CssSelector("div#ButtonsDiv table td>div>input#m_transferButton")).Click();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                Driver.FindElement(By.CssSelector("div.dialog_content div>input#ctl00_StudyTransferControl_m_relatedStudiesToggleButton")).Click();

                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                Driver.FindElement(By.CssSelector("div#DestinationListDiv select#ctl00_StudyTransferControl_m_destinationSources>option[value='-1']")).Click();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                Driver.FindElement(By.CssSelector("div.dialog_content input#ctl00_StudyTransferControl_TransferButton")).Click();

                //Click on Submit
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                Driver.FindElement(By.CssSelector("div#dataQCDiv input#ctl00_DataQCControl_m_submitButton")).Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(11)>span[title*='Ready']")));
                IWebElement DownloadBtn = BasePage.Driver.FindElement(By.CssSelector("input#ctl00_TransferJobsListControl_m_submitButton"));
                BasePage.Driver.FindElement(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(3)>td:nth-child(11)>span[title*='Expired']")).Click();
                if (!DownloadBtn.Enabled)
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

                //Step 20 - Select a Study that was previously Downloaded                
                BasePage.Driver.FindElement(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(11)>span[title*='Ready']")).Click();
                if (DownloadBtn.Enabled)
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

                //Step 21 - Select the Download Button
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#ctl00_TransferJobsListControl_m_submitButton")));
                BasePage.Driver.FindElement(By.CssSelector("input#ctl00_TransferJobsListControl_m_submitButton")).Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#ctl00_TransferJobPackagesListControl_m_submitButton")));
                BasePage.Driver.FindElement(By.CssSelector("input#ctl00_TransferJobPackagesListControl_m_submitButton")).Click();
                String description1;
                studies.GetMatchingRow("Accession", Accession[1]).TryGetValue("Description", out description1);
                PageLoadWait.WaitForDownload("_" + description1, Config.downloadpath, "zip");
                Boolean studydownloaded1 = BasePage.CheckFile("_" + description1, Config.downloadpath, "zip");
                if (studydownloaded1 == true)
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

                //Step 22 - Enable DICOM Force C-Move scenario 
                //servicetool.LaunchServiceTool();
                //servicetool.NavigateToEnableFeatures();
                //wpfobject.WaitTillLoad();                             
                //servicetool.SetEnableFeaturesTransferService();
                //servicetool.ModifyEnableFeatures();
                //wpfobject.WaitTillLoad();

                //servicetool.ApplyEnableFeatures();
                //wpfobject.WaitTillLoad();
                //wpfobject.SelectCheckBox(ServiceTool.EnableFeatures.Name.DicomCMove);
                //servicetool.ApplyEnableFeatures();
                //wpfobject.WaitTillLoad();

                //servicetool.RestartIISandWindowsServices();
                //servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Step 23 - Configure several datasources
                ExecutedSteps++;

                //Step 24 - Select a sudy from a Dicom source EADICOM and transfer using the simple method.
                ExecutedSteps++;

                //Step 25 - Select studies from data sources ISTOREONLINE and perform a simple data transfer.
                result.steps[++ExecutedSteps].status = "Not Applicable";

                //Step 26 - Select studies from data sources MergePACS and perform a simple data transfer.
                //studies.SearchStudy(AccessionNo: Accession[1], Datasource: login.GetHostName(Config.SanityPACS));
                //studies.SelectStudy("Accession", Accession[1]);

                //Driver.FindElement(By.CssSelector("div#ButtonsDiv table td>div>input#m_transferButton")).Click();
                //Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                //Driver.FindElement(By.CssSelector("div.dialog_content div>input#ctl00_StudyTransferControl_m_relatedStudiesToggleButton")).Click();
                //Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                //Driver.FindElement(By.CssSelector("div#DestinationListDiv select#ctl00_StudyTransferControl_m_destinationSources>option[value='-1']")).Click();
                //Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                //Driver.FindElement(By.CssSelector("div.dialog_content input#ctl00_StudyTransferControl_TransferButton")).Click();              
                //Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                //Driver.FindElement(By.CssSelector("div#dataQCDiv input#ctl00_DataQCControl_m_submitButton")).Click();
                //BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(11)>span[title*='Ready']")));                             
                //BasePage.Driver.FindElement(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(11)>span[title*='Ready']")).Click();                              
                //BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#ctl00_TransferJobsListControl_m_submitButton")));
                //BasePage.Driver.FindElement(By.CssSelector("input#ctl00_TransferJobsListControl_m_submitButton")).Click();
                //BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#ctl00_TransferJobPackagesListControl_m_submitButton")));                                
                //BasePage.Driver.FindElement(By.CssSelector("input#ctl00_TransferJobPackagesListControl_m_submitButton")).Click();     
                //String description2;
                //studies.GetMatchingRow("Accession", Accession[0]).TryGetValue("Description", out description2);
                //PageLoadWait.WaitForDownload("_" + description2, Config.downloadpath, "zip");       
                //Boolean studydownloaded2 = BasePage.CheckFile("_" + description2, Config.downloadpath, "zip");
                //if (studydownloaded2 == true)
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}               
                //Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                //BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#ctl00_TransferJobPackagesListControl_m_closeDialogButton")));
                //BasePage.Driver.FindElement(By.CssSelector("input#ctl00_TransferJobPackagesListControl_m_closeDialogButton")).Click();
                result.steps[++ExecutedSteps].status = "Hold";

                //Logout
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
        /// Downloader - View Granted Studies and searchFor Desktop only.
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27577(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data    
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');

                //Step 1 - Pre-conditions
                ExecutedSteps++;

                //Step 2 - Login as a user U4.  
                login.LoginIConnect(U4, U4);
                ExecutedSteps++;

                //Step 3 - Select a study and grant access it with any other available user.
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0]);
                studies.SelectStudy("Accession", Accession[0]);
                studies.ShareStudy(false, new String[] { U5 });

                //Repeat the same action for two more studies.
                studies.SearchStudy(AccessionNo: Accession[1]);
                studies.SelectStudy("Accession", Accession[1]);
                studies.ShareStudy(false, new String[] { U5 });

                studies.SearchStudy(AccessionNo: Accession[2]);
                studies.SelectStudy("Accession", Accession[2]);
                studies.ShareStudy(false, new String[] { U5 });
                ExecutedSteps++;

                //Step 4 - Select the Outbounds Tab
                outbounds = (Outbounds)login.Navigate("Outbounds");
                ExecutedSteps++;

                //Step 5 - Note a specific study on the list, Enter a search criteria to match this study (Name, ID, etc.) Search.
                outbounds.SearchStudy("lastname", "*");
                String[] values_5 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Accession", BasePage.GetColumnNames());
                if (Array.IndexOf(values_5, Accession[0]) > -1 &&
                    Array.IndexOf(values_5, Accession[1]) > -1 &&
                    Array.IndexOf(values_5, Accession[2]) > -1)
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
                login.Logout();

                //Step 6 - Login as user U5.
                login.LoginIConnect(U5, U5);
                ExecutedSteps++;

                //Step 7 - Select the Inbounds Tab
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;

                //Step 8 - Note a specific study on the list. Enter a search criteria to match this study (Name, ID, etc.). Search.
                inbounds.SearchStudy("lastname", "*");
                String[] values_8 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Accession", BasePage.GetColumnNames());
                if (Array.IndexOf(values_5, Accession[0]) > -1 &&
                    Array.IndexOf(values_5, Accession[1]) > -1 &&
                    Array.IndexOf(values_5, Accession[2]) > -1)
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
        /// Downloader - Initial Setups (LDAP user database)
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_106682(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data   
                String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");

                String LastNames = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String[] LName = LastNames.Split(':');

                //Step 1 - Pre-conditions
                ExecutedSteps++;

                //Step 2 - In the Service Tool\Enable Features tab Enable Study Sharing , Enable Study Transfer, Enable Downloader and Uploader.
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableStudySharing();
                servicetool.EnableDataTransfer();
                servicetool.EnableDataDownloader();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                wpfobject.ClickOkPopUp();
                wpfobject.WaitTillLoad();

                servicetool.EnableUpload();
                servicetool.RestartIISandWindowsServices();
                ExecutedSteps++;

                //Step 3 - In the Service Tool*^>^*Enable Features*^>^*Transfer Service --"Enable Transfer Service"option by clicking on the box.
                servicetool.SetEnableFeaturesTransferService();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableTransferService();
                ExecutedSteps++;

                //Step 4 - In the Service Tool*^>^*Enable Features*^>^*Transfer Service -- at the bottom of the window, select the package tab and change the expire interval to 5 min, and the Package maximum to 10000kB
                servicetool.ModifyPackagerDetails("5");
                ExecutedSteps++;

                //Step 5 - Setup Email Notification in the service tool
                servicetool.NavigateToTab("E-mail Notification");
                servicetool.NavigateSubTab("General");
                servicetool.SetEmailNotification();

                servicetool.RestartIISandWindowsServices();
                ExecutedSteps++;

                //Step 6 - Configure LDAP user database
                servicetool.LDAPSetup();
                ExecutedSteps++;

                //Step 7 - Enable Ldap Directory Service
                servicetool.NavigateToConfigToolUserMgmtDatabaseTab();
                servicetool.SetMode(1);
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Step 8 - Log in iConnect Access desktop version as administrator                
                login.LoginIConnect(Config.LdapAdminUserName, Config.LdapAdminPassword);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                ExecutedSteps++;

                //Step 9 - Create New Domain D1
                domainmanagement.CreateDomain(domainName: D1_LDAP, roleName: D1ROLE1, datasources: null);
                domainmanagement.SetCheckBoxInEditDomain("datatransfer", 0);
                domainmanagement.SetCheckBoxInEditDomain("grant", 0);
                domainmanagement.GrantAccessValidDaysTxtBox().Clear();
                domainmanagement.GrantAccessValidDaysTxtBox().SendKeys("1");
                domainmanagement.SetCheckBoxInEditDomain("datadownload", 0);
                domainmanagement.ClickSaveDomain();
                ExecutedSteps++;

                //Step 10 - Repeat and create another new Domain:D2
                domainmanagement.CreateDomain(domainName: D2_LDAP, roleName: D2ROLE2, datasources: null);
                domainmanagement.SetCheckBoxInEditDomain("datatransfer", 0);
                domainmanagement.SetCheckBoxInEditDomain("grant", 0);
                domainmanagement.GrantAccessValidDaysTxtBox().Clear();
                domainmanagement.GrantAccessValidDaysTxtBox().SendKeys("1");
                domainmanagement.SetCheckBoxInEditDomain("datadownload", 0);
                domainmanagement.ClickSaveDomain();
                if (domainmanagement.IsDomainExist(D1_LDAP) && domainmanagement.IsDomainExist(D2_LDAP))
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

                //Step 11 - Create group G1
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateGroup(D1_LDAP, G1_LDAP, selectallroles: 1);
                ExecutedSteps++;

                //Step 12 - Create group G2
                usermanagement.CreateGroup(D1_LDAP, G2_LDAP, selectallroles: 1);
                ExecutedSteps++;

                //Step 13 - Create group G4
                usermanagement.CreateGroup(D1_LDAP, G4_LDAP, selectallroles: 1);
                ExecutedSteps++;

                //Step 14 - Create subgroup CG1
                usermanagement.SearchGroup(G1_LDAP, D1_LDAP, 0);
                usermanagement.SelectGroup(G1_LDAP, D1_LDAP);
                usermanagement.CreateSubGroup(G1_LDAP, CG1_LDAP, rolename: D1ROLE1);
                usermanagement.SearchGroup(CG1_LDAP, D1_LDAP, 1);
                usermanagement.SelectGroupByName(CG1_LDAP);
                usermanagement.EditGrpBtn().Click();
                wait.Until(ExpectedConditions.ElementToBeClickable(usermanagement.CreateAndEditGroupPopupWindow()));
                PageLoadWait.WaitForProcessingState(10);
                usermanagement.RolesTab_Group().Click();
                IWebElement table2 = Driver.FindElement(By.CssSelector("#m_groupInfoDialog_m_groupRolesList_hierarchyRoleList_itemList"));
                List<IWebElement> allRows2 = table2.FindElements(By.CssSelector("tr:not([style*='display: none;'])")).ToList();
                if (allRows2.Count > 0)
                {
                    for (int i = 0; i < allRows2.Count; i++)
                    {
                        wait.Until(ExpectedConditions.ElementToBeClickable(usermanagement.Btn_RoleAdd()));
                        allRows2[i].Click();
                    }
                    usermanagement.Btn_RoleAdd().Click();
                }
                PageLoadWait.WaitForProcessingState(10);
                usermanagement.SaveAndViewMyGroupBtn().Click();
				Thread.Sleep(100000);
				ExecutedSteps++;

                //Step 15 - Create 'Doctor' role
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.ClickNewRoleBtn();
                rolemanagement.CreateRole(D1_LDAP, Doctor_LDAP);
                rolemanagement.SearchRole(Doctor_LDAP, D1_LDAP);
                rolemanagement.EditRoleByName(Doctor_LDAP);
                rolemanagement.SetCheckboxInEditRole("transfer", 0);
                rolemanagement.SetCheckboxInEditRole("download", 0);
                rolemanagement.GrantAccessRadioBtn_Anyone().Click();
                rolemanagement.ConnectAllDataSources();
                rolemanagement.ClickSaveEditRole();
                ExecutedSteps++;

                //Step 16 - Create 'Nurse' role
                rolemanagement.ClickNewRoleBtn();
                rolemanagement.CreateRole(D1_LDAP, Nurse_LDAP);
                rolemanagement.SearchRole(Nurse_LDAP, D1_LDAP);
                rolemanagement.EditRoleByName(Nurse_LDAP);
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                rolemanagement.GrantAccessRadioBtn_Anyone().Click();
                rolemanagement.ConnectAllDataSources();
                rolemanagement.RoleFilter_RefPhysician(FirstName_LDAP, LastName_LDAP);
                rolemanagement.ClickSaveEditRole();
                ExecutedSteps++;

                //Step 17 - create a new role 'Front Desk'
                rolemanagement.ClickNewRoleBtn();
                rolemanagement.CreateRole(D1_LDAP, FrontDesk_LDAP);
                rolemanagement.SearchRole(FrontDesk_LDAP, D1_LDAP);
                rolemanagement.EditRoleByName(FrontDesk_LDAP);
                rolemanagement.SetCheckboxInEditRole("transfer", 0);
                rolemanagement.SetCheckboxInEditRole("download", 1);
                rolemanagement.GrantAccessRadioBtn_Disabled().Click();
                rolemanagement.ConnectAllDataSources(1);
                rolemanagement.ClickSaveEditRole();
                ExecutedSteps++;

                //Step 18 - Create a new role 'Front Desk1' 
                rolemanagement.ClickNewRoleBtn();
                rolemanagement.CreateRole(D2_LDAP, FrontDesk1_LDAP);
                rolemanagement.SearchRole(FrontDesk1_LDAP, D2_LDAP);
                rolemanagement.EditRoleByName(FrontDesk1_LDAP);
                rolemanagement.SetCheckboxInEditRole("transfer", 1);
                rolemanagement.SetCheckboxInEditRole("download", 1);
                rolemanagement.GrantAccessRadioBtn_Disabled().Click();
                rolemanagement.ConnectAllDataSources(1);
                rolemanagement.ClickSaveEditRole();
                ExecutedSteps++;

                //Step 19 - Select the "D1" Domain from the dropdown list "Show Users From Domain"
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.SelectDomainFromDropdownList(D1_LDAP);
                usermanagement.SearchGroup(G1_LDAP, D1_LDAP, 0);
                usermanagement.SelectGroupByName(G1_LDAP);
                ExecutedSteps++;

                //Step 20 - Select user u4.
                usermanagement.CreateUser(U4_LDAP, Doctor_LDAP, 1, Email, 1, Config.LdapUserPassword, U4_LDAP, LName[0]);
                ExecutedSteps++;

                //Step 21 - Select the Group G1
                usermanagement.SearchGroup(G1_LDAP, D1_LDAP, 0);
                usermanagement.SelectGroupByName(G1_LDAP);
                if (usermanagement.IsUserExist(U4_LDAP, D1_LDAP))
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

                //Step 22 - Select the G2 in the Groups list
                usermanagement.SearchGroup(G2_LDAP, D1_LDAP, 0);
                usermanagement.SelectGroupByName(G2_LDAP);
                usermanagement.CreateUser(U5_LDAP, Nurse_LDAP, 1, Email, 1, Config.LdapUserPassword, U5_LDAP, LName[1]);
                ExecutedSteps++;

                //Step 23 - Select the Group G2 in the Groups list.
                usermanagement.SelectDomainFromDropdownList(D1_LDAP);
                usermanagement.SearchGroup(G2_LDAP, D1_LDAP, 0);
                usermanagement.SelectGroupByName(G2_LDAP);
                if (usermanagement.IsUserExist(U5_LDAP, D1_LDAP))
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

                //Step 24 - Create user u1
                usermanagement.SearchGroup(G2_LDAP, D1_LDAP, 0);
                usermanagement.SelectGroupByName(G2_LDAP);
                usermanagement.CreateUser(U1_LDAP, FrontDesk_LDAP, 1, Email, 1, Config.LdapUserPassword, U1_LDAP, LName[2]);
                ExecutedSteps++;

                //Step 25 - Select the Group G2 in the Groups list. Search users               
                usermanagement.SearchGroup(G2_LDAP, D1_LDAP, 0);
                usermanagement.SelectGroupByName(G2_LDAP);
                if (usermanagement.IsUserExist(U1_LDAP, D1_LDAP) &&
                    usermanagement.IsUserExist(U5_LDAP, D1_LDAP))
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

                //Step 26 - Select the triangle on the right hand side of the G1 in the Groups list and then select CG1                
                usermanagement.SearchGroup(G1_LDAP, D1_LDAP, 0);
                usermanagement.SelectSubGroup(G1_LDAP, CG1_LDAP);
                usermanagement.CreateUser(U6_LDAP, FrontDesk_LDAP, 1, Email, 1, Config.LdapUserPassword, U6_LDAP, LName[3]);
                ExecutedSteps++;

                //Step 27 - Select the triangle on the right hand side of the G1 in the Groups list and then select CG1
                usermanagement.SearchGroup(CG1_LDAP, D1_LDAP, 1);
                usermanagement.SelectGroupByName(CG1_LDAP);
                if (usermanagement.IsUserExist(U6_LDAP, D1_LDAP))
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

                //Step 28 - Repeat the above step to add user "U7" to CG1 group with the role "Nurse".
                usermanagement.CreateUser(U7_LDAP, Nurse_LDAP, 1, Email, 1, U7_LDAP, U7_LDAP, LName[4]);
                if (usermanagement.IsUserExist(U7_LDAP, D1_LDAP))
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

                //Step 29 - Create group G3
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.SelectDomainFromDropdownList(D2_LDAP);
                usermanagement.CreateGroup(D2_LDAP, G3_LDAP, password: Config.LdapUserPassword, rolename: FrontDesk1_LDAP, email: Email, IsManaged: 1, rolenames: new string[] { FrontDesk1_LDAP, FrontDesk_LDAP }, GroupUser: U9_LDAP, LName: LName[5], FName: U9_LDAP);
                PageLoadWait.WaitForProcessingState(20);
                ExecutedSteps++;

                //Step 30 - Create user U9
                ExecutedSteps++;

                //Step 31 - Logout and login by using ica.administrator account, select the User Management tab
                login.Logout();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                ExecutedSteps++;

                //Step 32 - Select group (G4) and click on Edit.
                usermanagement.SearchGroup(G4_LDAP, D1_LDAP, 1);
                usermanagement.SelectGroupByName(G4_LDAP);
                usermanagement.EditGrpBtn().Click();
                wait.Until(ExpectedConditions.ElementToBeClickable(usermanagement.CreateAndEditGroupPopupWindow()));
                PageLoadWait.WaitForProcessingState(10);
                ExecutedSteps++;

                //Step 33 - Try to modify different fields such as: group name, descriptions,select Save.               
                usermanagement.GroupDescTxtBox().Clear();
                usermanagement.GroupDescTxtBox().SendKeys(G4_LDAP + " Description");
                usermanagement.SaveAndViewMyGroupBtn().Click();
				Thread.Sleep(100000);
				ExecutedSteps++;

                //Step 34 - Select Group G4
                usermanagement.SearchGroup(G4_LDAP, D1_LDAP, 1);
                usermanagement.SelectGroupByName(G4_LDAP);
                ExecutedSteps++;

                //Step 35 - Select Delete button
                usermanagement.DelGrpBtn().Click();
                PageLoadWait.WaitForPageLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (BasePage.Driver.FindElement(By.CssSelector("#DialogDiv #ConfirmationDiv")).Displayed)
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

                //Setp 36 - Select OK
                PageLoadWait.WaitForPageLoad(20);
                usermanagement.OkButtonConfirmGroupDeletionMsgBox().Click();
                ExecutedSteps++;

                //Logout
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
        /// Downloader - Audit messagesFor Desktop only.
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27578(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data    
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');

                //Step 1 - Pre-conditions
                ExecutedSteps++;

                //Step 2 - Login as administrator - Go to Maintanince tab*^>^*Audit                                
                ExecutedSteps++;

                Console.WriteLine("Audit Log Count ------------- " + "\n" + AuditLogList.Count);
                //Step 3 - Create User U4.
                if (AuditLogList[0] == true)
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

                //Step 4 - Create User U5.
                if (AuditLogList[1] == true)
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

                //Step 5 - Create User U1.
                if (AuditLogList[2] == true)
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

                //Step 6 - Create User U6.
                if (AuditLogList[3] == true)
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

                //Step 7 - Create User U9.
                if (AuditLogList[4] == true)
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

                //Step 8 - Delete group G4.
                if (AuditLogList[5] == true)
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
    }
}
