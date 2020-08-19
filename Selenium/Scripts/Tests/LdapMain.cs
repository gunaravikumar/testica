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
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.Putty;
using System.Data;
using Selenium.Scripts.Pages.MergeServiceTool;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TabItems;
using Application = TestStack.White.Application;
using Button = TestStack.White.UIItems.Button;
using CheckBox = TestStack.White.UIItems.CheckBox;
using ComboBox = TestStack.White.UIItems.ListBoxItems.ComboBox;
using ListView = TestStack.White.UIItems.ListView;
using RadioButton = TestStack.White.UIItems.RadioButton;
using TextBox = TestStack.White.UIItems.TextBox;
using System.Xml;
namespace Selenium.Scripts.Tests
{
    class LdapMain
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public ExamImporter ei { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public HPHomePage hphomepage { get; set; }
        public HPLogin hplogin { get; set; }
        public ServiceTool servicetool { get; set; }
        public WpfObjects wpfobject;
        public BasePage basepage;

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public LdapMain(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            ei = new ExamImporter();
            mpaclogin = new MpacLogin();
            servicetool = new ServiceTool();
            wpfobject = new WpfObjects();
            basepage = new BasePage();
        }

        /// <summary>
        /// 1.0 Active Directory LDAP Identity map
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27674(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmanagement = null;
            RoleManagement rolemanagement = null;
            Studies studies = null;
            UserManagement usermanagement = null;
            UserPreferences userpreferences = null;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            string ServerName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ServerName");
            string HostName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "HostName");
            string Port = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Port"));
            int resultcount = 0;
            string[] DomainNames = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "DomainNames")).Split(':');
            string[] RoleNames = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "RoleNames")).Split(':');
            string[] Groups = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Groups")).Split(':');
            string[] Expected = null;
            string[] Actual = null;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                servicetool.CloseServiceTool();
                string LdapXML = File.ReadAllText((String)ReadExcel.GetTestData(filepath, "TestData", testid, "LdapXMLPath"));
                basepage.ChangeAttributeValue(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server", "enabled", "False");
                if (basepage.NodeExist(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server[@id='" + ServerName + "']"))
                {
                    basepage.RemoveNode(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server[@id='" + ServerName + "']");
                }
                basepage.InsertNode(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/servers", LdapXML, false);
                basepage.ChangeAttributeValue(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server[@id='" + ServerName + "']", "enabled", "True");
                basepage.InsertNode(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server[@id='" + ServerName + "']/options/hosts", string.Concat("<host name=\"", HostName, "\" port=\"", Port, "\" />"));
                servicetool.RestartIISUsingexe();

                //Step 1: Test Data: Other users may also be listed. In service tool Install license and connect datasource: * 10 licences are required. * One datasource
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 2: In the iConnect service tool select the User Management Database tab
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolUserMgmtDatabaseTab();
                Expected = new string[] { "Database Connection", "User Management Mode", "OfficePACS Settings" };
                Actual = servicetool.GetCurrentTabItem().GetMultiple(SearchCriteria.ByClassName("GroupBox")).Select(element => element.Name).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
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
                //Step 3: Select modify under User management mode select Ldap Directory Service and uncheck Local Database, do not change any other settings Apply
                servicetool.SetMode(1);
                ExecutedSteps++;
                //Step 4: In the iConnect service tool select the LDAP tab
                servicetool.NavigateToTab(ServiceTool.LDAP_Tab);
                Expected = new string[] { "Global Options", "Servers", "Ldap/Local Responsibilities" };
                Actual = wpfobject.GetTabWpf(1).Pages.Select(page => page.Name).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("LDAP contains Global Options, Servers, Ldap/Local Responsibilities tab");
                }
                if (string.Equals(wpfobject.GetTabWpf(1).SelectedTab.Name, "Global Options"))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("LDAP contains Global Options as default tab");
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
                //Step 5: Select the Servers Tab
                wpfobject.GetTabWpf(1).SelectTabPage(1);
                wpfobject.ClickButton(ServiceTool.ModifyBtn_Name, 1);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;
                //Step 6: Select ica-ldap.merge.ad and click on the Detail button
                GroupBox ldap_grp1 = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(servicetool.GetCurrentTabItem(), ServiceTool.LDAP.Name.LdapServerListGrp, 1);
                ListView datagrid1 = wpfobject.GetAnyUIItem<GroupBox, ListView>(ldap_grp1, ServiceTool.LDAP.ID.LdapServersList);
                /*foreach (var row in datagrid1.Rows)
                {
                    if (row.Cells[0].Text.ToLower().Equals(ServerName.ToLower()))
                    {
                        row.Focus();
                        wpfobject.WaitTillLoad();
                        row.Click();
                        wpfobject.WaitTillLoad();
                        break;
                    }
                }*/
                datagrid1.Rows[0].Click();
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ServiceTool.LDAP.ID.DetailsBtn);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindowByTitle("LDAP Server Control Form");//ServiceTool.LDAP.Name.LdapServerDetailWindow);
                wpfobject.WaitTillLoad();
                GroupBox ldap_grp = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(servicetool.GetCurrentTabItem(), ServiceTool.LDAP.Name.ServerHostsGrp, 1);
                ListView datagrid = wpfobject.GetAnyUIItem<GroupBox, ListView>(ldap_grp, ServiceTool.LDAP.ID.ServerHostsListList);
                resultcount = 0;
                if (string.Equals(HostName, datagrid.Rows[0].Cells[0].Text))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("The Given HostName " + HostName + " is present");
                }
                GroupBox siteDomain_grp = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(servicetool.GetCurrentTabItem(), ServiceTool.LDAP.Name.SiteDomainNamesGrp, 1);
                TextBox tb = wpfobject.GetAnyUIItem<GroupBox, TextBox>(siteDomain_grp, ServiceTool.LDAP.ID.SiteDomainNamesTxt);
                Expected = new string[] { "SuperAdminGroup", "TestDomain", "Domain1" };
                string actualvalue = tb.Text;
                if (Expected.All(exp => actualvalue.Contains(exp)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("All Site Domain Names are present");
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
                //Step 7: Select the Mapping Details tab then Select Identity from the Type drop down menu.
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 8: Click on OK , apply , Restart IIS
                servicetool.CloseServiceTool();
                ExecutedSteps++;
                //Step 9:  Login iCA as a registered user UID = ica.administrator PID = admin.13579
                login.LoginIConnect("ica.administrator", "admin.13579");
                ExecutedSteps++;
                //Step 10: Double click on the SuperAdminGroup
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain("SuperAdminGroup");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.ClickEditDomain();
                ExecutedSteps++;
                //Step 11: Select a Data Source and move it to the connected side
                domainmanagement.ConnectAllDataSources();
                ExecutedSteps++;
                //Step 12: click on Save
                domainmanagement.ClickSaveEditDomain();
                if (login.IsTabSelected("Domain Management"))
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
                //Step 13: Select the User management tab and select the SuperAdminGroup Domain, enter * click on the Search button.
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.SearchWithoutFilter("SuperAdminGroup");
                resultcount = 0;
                Expected = new string[] { "ica.administrator (IConnectAccess Administrator) Activated SuperAdmin", "victoria.dassen (Vicky Dassen) Activated User", "SALMON (Salmon Ben Judah) Activated User" };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Filter contains the users ica.administrator, victoria.dassen and salmon");
                }
                if (usermanagement.NewGrpBtn().Displayed && usermanagement.NewSubGrpBtn().Displayed && usermanagement.EditGrpBtn().Displayed && usermanagement.DelGrpBtn().Displayed && usermanagement.MoveGrpBtn().Displayed && usermanagement.DataMappingBtn().Displayed)
                {
                    resultcount++;
                    Logger.Instance.InfoLog("6 Group Buttons are displayed");
                }
                if (!usermanagement.NewUsrBtn().Displayed && !usermanagement.EditUsrBtn().Displayed && usermanagement.DelUsrBtn().Displayed)
                {
                    resultcount++;
                    Logger.Instance.InfoLog("New and Edit user Buttons are not displayed. Delete user button is displayed");
                }
                if (resultcount == 3)
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
                //Step 14: Logout and log back in as victoria.dassen/.vcd.13579
                login.Logout();
                login.LoginIConnect("victoria.dassen", ".vcd.13579");
                ExecutedSteps++;
                //Step 15: Change the Study Performed box from Last 2 Days to All Dates enter * in last name and click the search button
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(LastName: "*");
                PageLoadWait.WaitForSearchLoad();
                Dictionary<int, string[]> SearchResults = BasePage.GetSearchResults();
                if (SearchResults.Count > 0)
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
                //Step 16: Click on options User Preferences
                userpreferences = new UserPreferences();
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                if (string.Equals(userpreferences.UserPreferenceName().Text, "User Preferences"))
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
                //Step 17: Edit some parameter and click OK *^>^* click Close
                userpreferences.PNGRadioBtn().Click();
                userpreferences.EmailFormatText().Click();
                userpreferences.CloseUserPreferences();
                ExecutedSteps++;
                //Step 18: Reopen the user preferences page
                userpreferences.OpenUserPreferences();
                resultcount = 0;
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                if (!userpreferences.PNGRadioBtn().Selected)
                {
                    Logger.Instance.ErrorLog("PNGRadioBtn is not Selected");
                    resultcount++;
                }
                if (!userpreferences.EmailFormatText().Selected)
                {
                    Logger.Instance.ErrorLog("EmailFormatText is not Selected");
                    resultcount++;
                }
                userpreferences.CloseUserPreferences();
                if (resultcount == 0)
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

                //Step 19: Logout and log back in as ica.administrator/admin.13579
                login.Logout();
                login.LoginIConnect("ica.administrator", "admin.13579");
                ExecutedSteps++;
                //Step 20: Create new Domains, Roles and Groups as per the TestData Tab. Section"MergeHealthcare Root Domain"
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                Dictionary<Object, String> createDomain = domainmanagement.CreateDomainAttr();
                if (!domainmanagement.IsDomainExist(DomainNames[0]))
                {
                    createDomain[DomainManagement.DomainAttr.DomainName] = DomainNames[0];
                    createDomain[DomainManagement.DomainAttr.RoleName] = RoleNames[0];
                    domainmanagement.CreateDomain(createDomain);
                }
                if (!domainmanagement.IsDomainExist(DomainNames[1]))
                {
                    createDomain[DomainManagement.DomainAttr.DomainName] = DomainNames[1];
                    createDomain[DomainManagement.DomainAttr.RoleName] = RoleNames[1];
                    createDomain[DomainManagement.DomainAttr.UserID] = BasePage.GetUniqueDomainID();
                    domainmanagement.CreateDomain(createDomain);
                }
                if (!domainmanagement.IsDomainExist(DomainNames[2]))
                {
                    createDomain[DomainManagement.DomainAttr.DomainName] = DomainNames[2];
                    createDomain[DomainManagement.DomainAttr.RoleName] = RoleNames[2];
                    createDomain[DomainManagement.DomainAttr.UserID] = BasePage.GetUniqueDomainID();
                    domainmanagement.CreateDomain(createDomain);
                }
                if (!domainmanagement.IsDomainExist(DomainNames[3]))
                {
                    createDomain[DomainManagement.DomainAttr.DomainName] = DomainNames[3];
                    createDomain[DomainManagement.DomainAttr.RoleName] = RoleNames[3];
                    createDomain[DomainManagement.DomainAttr.UserID] = BasePage.GetUniqueDomainID();
                    domainmanagement.CreateDomain(createDomain);
                }
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                if (!rolemanagement.RoleExists(RoleNames[0], DomainNames[0]))
                {
                    rolemanagement.CreateRole(DomainNames[0], RoleNames[0], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[4], DomainNames[0]))
                {
                    rolemanagement.CreateRole(DomainNames[0], RoleNames[4], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[1], DomainNames[1]))
                {
                    rolemanagement.CreateRole(DomainNames[1], RoleNames[1], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[5], DomainNames[1]))
                {
                    rolemanagement.CreateRole(DomainNames[1], RoleNames[5], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[6], DomainNames[1]))
                {
                    rolemanagement.CreateRole(DomainNames[1], RoleNames[6], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[7], DomainNames[1]))
                {
                    rolemanagement.CreateRole(DomainNames[1], RoleNames[7], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[8], DomainNames[1]))
                {
                    rolemanagement.CreateRole(DomainNames[1], RoleNames[8], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[9], DomainNames[1]))
                {
                    rolemanagement.CreateRole(DomainNames[1], RoleNames[9], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[2], DomainNames[2]))
                {
                    rolemanagement.CreateRole(DomainNames[2], RoleNames[2], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[10], DomainNames[2]))
                {
                    rolemanagement.CreateRole(DomainNames[2], RoleNames[10], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[3], DomainNames[3]))
                {
                    rolemanagement.CreateRole(DomainNames[3], RoleNames[3], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[11], DomainNames[3]))
                {
                    rolemanagement.CreateRole(DomainNames[3], RoleNames[11], roletype: "");
                }
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                if (!usermanagement.IsGroupExist(Groups[0], DomainNames[1]))
                {
                    usermanagement.CreateGroup(DomainNames[1], Groups[0]);
                }
                if (!usermanagement.IsGroupExist(Groups[1], DomainNames[1]))
                {
                    usermanagement.CreateGroup(DomainNames[1], Groups[1]);
                }
                if (!usermanagement.IsGroupExist(Groups[3], DomainNames[1]))
                {
                    usermanagement.CreateSubGroup(Groups[1], Groups[3]);
                }
                if (!usermanagement.IsGroupExist(Groups[0], DomainNames[2]))
                {
                    usermanagement.CreateGroup(DomainNames[2], Groups[0]);
                }
                if (!usermanagement.IsGroupExist(Groups[1], DomainNames[2]))
                {
                    usermanagement.CreateGroup(DomainNames[2], Groups[1]);
                }
                if (!usermanagement.IsGroupExist(Groups[2], DomainNames[3]))
                {
                    usermanagement.CreateGroup(DomainNames[3], Groups[2]);
                }
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                usermanagement.ClearBtn().Click();
                ExecutedSteps++;
                //Step 21: Select the User management tab and select Domain1, enter * and then click on Search.
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.SearchWithoutFilter(DomainNames[0]);
                Expected = new string[] { "admin1 (Domain1 Admin) Activated SiteAdmin", "ben.kenobi (ObiWan Kenobi) Activated User" };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
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
                //Step 22: Change Domain to"Domain2", enter * and then click on Search.
                usermanagement.SearchWithoutFilter(DomainNames[2]);
                Expected = new string[] { "admin2 (Domain2 Admin) Activated SiteAdmin", "young.skywalker (Luke Skywalker) Activated User", "jabba (Jabba Hutt) Activated User" };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
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
                //Step 23: Change Domain to"Domain3", enter * and then click on Search.
                usermanagement.SearchWithoutFilter(DomainNames[3]);
                Expected = new string[] { "admin3 (Domain3 Admin) Activated SiteAdmin", "princess (Leia Organa) Activated User", "wookie (Chewbakka na) Activated User", "scoundrel (Han Solo) Activated User" };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
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
                //Step 24: Change Domain to"TestDomain", enter * and then click on Search.
                usermanagement.SearchWithoutFilter(DomainNames[1]);
                Expected = new string[] { "victoria.admin (Victoria.Admin Dassen) Activated SiteAdmin", "peter (Simon bar Jonah) Activated GroupAdmin", "john (John Boanerges) Activated User", "samuel (Samuel ben Elkanah) Activated User" };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
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
                //Step 25: Select "Clear Search"
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                usermanagement.ClearUsrBtn().Click();
                ExecutedSteps++;
                //Step 26: Select Ungrouped from the left hand side, enter * and then click on Search.
                usermanagement.SearchWithoutFilter(DomainNames[1], "Ungrouped");
                Expected = new string[] { "victoria.admin (Victoria.Admin Dassen) Activated SiteAdmin" };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
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
                //Step 27: Select G1 from the left hand side enter *and then click on Search.
                usermanagement.SearchWithoutFilter(DomainNames[1], Groups[0]);
                Expected = new string[] { "peter (Simon bar Jonah) Activated GroupAdmin", "boaz (Boaz Ben Salmon) Activated User" };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
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
                //Step 28: Select G2 from the left hand side enter *and then click on Search.
                usermanagement.SearchWithoutFilter(DomainNames[1], Groups[1]);
                Expected = new string[] { "john (John Boanerges) Activated User" };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
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
                //Step 29: Select G2.1 from the left hand side enter *and then click on Search.
                usermanagement.SearchWithoutFilter(DomainNames[1], subgroupname: Groups[3]);
                Expected = new string[] { "samuel (Samuel ben Elkanah) Activated User" };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
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
                //Step 30: Logout and login as peter/ .ptr.13579
                login.Logout();
                login.LoginIConnect("peter", ".ptr.13579");
                ExecutedSteps++;
                //Step 31: Logout from ICA
                login.Logout();
                ExecutedSteps++;
                //Step 32: Change password in LDAP
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 33: Select the Tree mode 
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 34: Log into ICA with User"peter"and the new password
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 35: reset Password
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 36: Login as Peter with password: .ptr.13579
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
            finally
            {
                servicetool.CloseServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolUserMgmtDatabaseTab();
                servicetool.SetMode(2);
                servicetool.SetEnableFeaturesGeneral();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableSelfEnrollment();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                wpfobject.ClickOkPopUp();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                if (basepage.NodeExist(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server[@id='" + ServerName + "']"))
                {
                    basepage.RemoveNode(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server[@id='" + ServerName + "']");
                }
                servicetool.RestartIISUsingexe();
            }
        }

        public TestCaseResult Test_27675(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmanagement = null;
            UserManagement usermanagement = null;
            RoleManagement rolemanagement = null;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            string ServerName = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "ServerName"));
            string HostName = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "HostName"));
            string Port = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Port"));
            string[] DomainNames = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "DomainNames")).Split(':');
            string[] RoleNames = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "RoleNames")).Split(':');
            string[] Groups = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Groups")).Split(':');
            string[] Expected = null;
            string[] Actual = null;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                //Step 1: In iConnect service tool select the LDAP Tab and then Servers.
                servicetool.CloseServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolUserMgmtDatabaseTab();
                servicetool.SetMode(1);
                servicetool.CloseServiceTool();
                string LdapXML = File.ReadAllText((String)ReadExcel.GetTestData(filepath, "TestData", testid, "LdapXMLPath"));
                basepage.ChangeAttributeValue(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server", "enabled", "False");
                if (basepage.NodeExist(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server[@id='" + ServerName + "']"))
                {
                    basepage.RemoveNode(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server[@id='" + ServerName + "']");
                }
                basepage.InsertNode(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/servers", LdapXML, false);
                basepage.ChangeAttributeValue(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server[@id='" + ServerName + "']", "enabled", "True");
                basepage.InsertNode(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server[@id='" + ServerName + "']/options/hosts", string.Concat("<host name=\"", HostName, "\" port=\"", Port, "\" />"));
                servicetool.RestartIISUsingexe();

                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.LDAP_Tab);
                wpfobject.GetTabWpf(1).SelectTabPage(1);
                wpfobject.ClickButton(ServiceTool.ModifyBtn_Name, 1);
                wpfobject.WaitTillLoad();
                GroupBox ldap_grp1 = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(servicetool.GetCurrentTabItem(), ServiceTool.LDAP.Name.LdapServerListGrp, 1);
                ListView datagrid1 = wpfobject.GetAnyUIItem<GroupBox, ListView>(ldap_grp1, ServiceTool.LDAP.ID.LdapServersList);
                /*foreach (var row in datagrid1.Rows)
                {
                    if (row.Cells[0].Text.ToLower().Equals(ServerName.ToLower()))
                    {
                        row.Focus();
                        wpfobject.WaitTillLoad();
                        row.Click();
                        wpfobject.WaitTillLoad();
                        break;
                    }
                }*/
                datagrid1.Rows[0].Click();
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ServiceTool.LDAP.ID.DetailsBtn);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindowByTitle("LDAP Server Control Form");//ServiceTool.LDAP.Name.LdapServerDetailWindow);
                wpfobject.WaitTillLoad();
                GroupBox ldap_grp = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(servicetool.GetCurrentTabItem(), ServiceTool.LDAP.Name.ServerHostsGrp, 1);
                ListView datagrid = wpfobject.GetAnyUIItem<GroupBox, ListView>(ldap_grp, ServiceTool.LDAP.ID.ServerHostsListList);
                string host = datagrid.Rows[0].Cells[0].Text;
                servicetool.CloseServiceTool();
                if (string.Equals(HostName, host))
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
                //Step 2: Login to ICA as a registered user 
                login.LoginIConnect("trillium.admin", "admin.13579");
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                Dictionary<Object, String> createDomain = domainmanagement.CreateDomainAttr();
                if (!domainmanagement.IsDomainExist(DomainNames[0]))
                {
                    createDomain[DomainManagement.DomainAttr.DomainName] = DomainNames[0];
                    createDomain[DomainManagement.DomainAttr.RoleName] = RoleNames[0];
                    domainmanagement.CreateDomain(createDomain);
                }
                if (!domainmanagement.IsDomainExist(DomainNames[1]))
                {
                    createDomain[DomainManagement.DomainAttr.DomainName] = DomainNames[1];
                    createDomain[DomainManagement.DomainAttr.RoleName] = RoleNames[1];
                    createDomain[DomainManagement.DomainAttr.UserID] = BasePage.GetUniqueDomainID();
                    domainmanagement.CreateDomain(createDomain);
                }
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                if (!rolemanagement.RoleExists(RoleNames[2], DomainNames[0]))
                {
                    rolemanagement.CreateRole(DomainNames[0], RoleNames[2], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[3], DomainNames[1]))
                {
                    rolemanagement.CreateRole(DomainNames[1], RoleNames[3], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[4], DomainNames[1]))
                {
                    rolemanagement.CreateRole(DomainNames[1], RoleNames[4], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[5], DomainNames[1]))
                {
                    rolemanagement.CreateRole(DomainNames[1], RoleNames[5], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[6], DomainNames[1]))
                {
                    rolemanagement.CreateRole(DomainNames[1], RoleNames[6], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[7], DomainNames[1]))
                {
                    rolemanagement.CreateRole(DomainNames[1], RoleNames[7], roletype: "");
                }
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                if (!usermanagement.IsGroupExist(Groups[0], DomainNames[1]))
                {
                    usermanagement.CreateGroup(DomainNames[1], Groups[0]);
                }
                if (!usermanagement.IsGroupExist(Groups[1], DomainNames[1]))
                {
                    usermanagement.CreateGroup(DomainNames[1], Groups[1]);
                }
                if (!usermanagement.IsGroupExist(Groups[2], DomainNames[1]))
                {
                    usermanagement.CreateSubGroup(Groups[1], Groups[2]);
                }
                ExecutedSteps++;
                //Step 3: Select the User management tab and select the SuperAdminGroup Domain, enter * and click on the search button
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.SearchWithoutFilter("SuperAdminGroup");
                int resultcount = 0;
                Expected = new string[] { "trillium.admin (trillium administrator) Activated SuperAdmin", "joshua (Joshua ben Nun) Activated SuperAdmin" };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Filter contains the users trilliium.admin and joshua");
                }
                if (usermanagement.NewGrpBtn().Displayed && usermanagement.NewSubGrpBtn().Displayed && usermanagement.EditGrpBtn().Displayed && usermanagement.DelGrpBtn().Displayed && usermanagement.MoveGrpBtn().Displayed && usermanagement.DataMappingBtn().Displayed)
                {
                    resultcount++;
                    Logger.Instance.InfoLog("6 Group Buttons are displayed");
                }
                if (!usermanagement.NewUsrBtn().Displayed && !usermanagement.EditUsrBtn().Displayed && usermanagement.DelUsrBtn().Displayed)
                {
                    resultcount++;
                    Logger.Instance.InfoLog("New and Edit user Buttons are not displayed. Delete user button is displayed");
                }
                if (resultcount == 3)
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
                //Step 4: Domain1 has been mapped to Sherway in this LDAP, ICA has Domain1 entered as a valid domain. Select the domain "Domain1" in the User Management, enter * and then click on the Search button
                usermanagement.SearchWithoutFilter(DomainNames[0]);
                Expected = new string[] { "shw.admin (Sherway Admin) Activated SiteAdmin", "ben.kildare (Ben Kildare) Activated User", "akagi (Naoko Akagi) Activated User" };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
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
                //Step 5: With the Domain1 still selected click on "Clear Search"in the user management to clear the display of users
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                usermanagement.ClearUsrBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForLoadingMessage();
                PageLoadWait.WaitForFrameLoad(20);
                if (usermanagement.UserList().Count == 0)
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
                //Step 6: There are no groups mapped for this domain the only group listed is the default"Ungrouped"group. Select the Ungrouped group on the left hand side enter *and search
                usermanagement.SearchWithoutFilter(DomainNames[0], "Ungrouped");
                Expected = new string[] { "shw.admin (Sherway Admin) Activated SiteAdmin", "ben.kildare (Ben Kildare) Activated User", "akagi (Naoko Akagi) Activated User" };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
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
                //Step 7: The TestDomain has been mapped to Queensway in this LDAP. In ICA the TestDomain in entered as a valid domain. Select the domain"TestDomain" in the User Management screen, enter * and then click on the Search button
                usermanagement.SearchWithoutFilter(DomainNames[1]);
                Expected = new string[] { "qw.admin (Queensway Admin) Activated SiteAdmin", "marcus.welby (Marcus Welby) Activated User", "tmhlivesay (Tracey Livesay) Activated User" };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
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
                //Step 8: With the TestDomain still selected, click on "Clear Search"
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                usermanagement.ClearUsrBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForLoadingMessage();
                PageLoadWait.WaitForFrameLoad(20);
                if (usermanagement.UserList().Count == 0)
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
                //Step 9: Click on the default Ungrouped on the left hand side enter * and search
                usermanagement.SearchWithoutFilter(DomainNames[1], "Ungrouped");
                Expected = new string[] { "qw.admin (Queensway Admin) Activated SiteAdmin" };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
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
                //Step 10: Click on G2 enter * and search
                usermanagement.SearchWithoutFilter(DomainNames[1], "G2");
                Expected = new string[] { "marcus.welby (Marcus Welby) Activated User" };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
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
                //Step 11: Click on G2.1 the subgroup enter * and search
                usermanagement.SearchWithoutFilter(DomainNames[1], subgroupname: Groups[2]);
                Expected = new string[] { "tmhlivesay (Tracey Livesay) Activated User" };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
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
                //Step 12: The G1 group is not mapped for this LDAP Click on G1,enter* and search
                usermanagement.SearchWithoutFilter(DomainNames[1], Groups[0]);
                if (usermanagement.UserList().Count == 0)
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
            finally
            {
                servicetool.CloseServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolUserMgmtDatabaseTab();
                servicetool.SetMode(2);
                servicetool.SetEnableFeaturesGeneral();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableSelfEnrollment();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                wpfobject.ClickOkPopUp();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                if (basepage.NodeExist(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server[@id='" + ServerName + "']"))
                {
                    basepage.RemoveNode(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server[@id='" + ServerName + "']");
                }
                servicetool.RestartIISUsingexe();
            }
        }

        public TestCaseResult Test_27676(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            DomainManagement domainmanagement = null;
            UserManagement usermanagement = null;
            RoleManagement rolemanagement = null;
            LdapDataMapping ldapdatamapping = null;
            string ServerName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ServerName");
            string HostName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "HostName");
            string Port = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Port"));
            int ExecutedSteps = -1;
            string[] DomainNames = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "DomainNames")).Split(':');
            string[] RoleNames = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "RoleNames")).Split(':');
            string[] Groups = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Groups")).Split(':');
            string[] Expected = null;
            string[] Actual = null;
            result = new TestCaseResult(stepcount);
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                //Step 1:  create a new LDAP server Configuration
                servicetool.CloseServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolUserMgmtDatabaseTab();
                servicetool.SetMode(1);
                servicetool.CloseServiceTool();
                string LdapXML = File.ReadAllText((String)ReadExcel.GetTestData(filepath, "TestData", testid, "LdapXMLPath"));
                basepage.ChangeAttributeValue(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server", "enabled", "False");
                if (basepage.NodeExist(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server[@id='" + ServerName + "']"))
                {
                    basepage.RemoveNode(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server[@id='" + ServerName + "']");
                }
                basepage.InsertNode(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/servers", LdapXML, false);
                basepage.ChangeAttributeValue(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server[@id='" + ServerName + "']", "enabled", "True");
                basepage.InsertNode(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server[@id='" + ServerName + "']/options/hosts", string.Concat("<host name=\"", HostName, "\" port=\"", Port, "\" />"));
                servicetool.RestartIISUsingexe();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.LDAP_Tab);
                wpfobject.GetTabWpf(1).SelectTabPage(1);
                wpfobject.ClickButton(ServiceTool.ModifyBtn_Name, 1);
                wpfobject.WaitTillLoad();
                GroupBox ldap_grp1 = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(servicetool.GetCurrentTabItem(), ServiceTool.LDAP.Name.LdapServerListGrp, 1);
                ListView datagrid1 = wpfobject.GetAnyUIItem<GroupBox, ListView>(ldap_grp1, ServiceTool.LDAP.ID.LdapServersList);
                /*string[] serverlist = datagrid1.Rows.Select(row => row.Cells[0].Text.ToLower()).ToArray();
                servicetool.CloseServiceTool();
                if (serverlist.Contains(ServerName.ToLower()))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }*/
                datagrid1.Rows[0].Click();
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ServiceTool.LDAP.ID.DetailsBtn);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindowByTitle("LDAP Server Control Form");//ServiceTool.LDAP.Name.LdapServerDetailWindow);
                wpfobject.WaitTillLoad();
                GroupBox ldap_grp = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(servicetool.GetCurrentTabItem(), ServiceTool.LDAP.Name.ServerHostsGrp, 1);
                ListView datagrid = wpfobject.GetAnyUIItem<GroupBox, ListView>(ldap_grp, ServiceTool.LDAP.ID.ServerHostsListList);
                string host = datagrid.Rows[0].Cells[0].Text;
                servicetool.CloseServiceTool();
                if (string.Equals(HostName, host))
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
                //Step 2: Login to ICA as a registered user 
                login.LoginIConnect("trillium.admin", "admin.13579");
                ExecutedSteps++;
                //Step 3: Create new Domains, Roles and Groups 
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                Dictionary<Object, String> createDomain = domainmanagement.CreateDomainAttr();
                if (!domainmanagement.IsDomainExist(DomainNames[0]))
                {
                    createDomain[DomainManagement.DomainAttr.DomainName] = DomainNames[0];
                    createDomain[DomainManagement.DomainAttr.RoleName] = RoleNames[0];
                    domainmanagement.CreateDomain(createDomain);
                }
                if (!domainmanagement.IsDomainExist(DomainNames[1]))
                {
                    createDomain[DomainManagement.DomainAttr.DomainName] = DomainNames[1];
                    createDomain[DomainManagement.DomainAttr.RoleName] = RoleNames[1];
                    createDomain[DomainManagement.DomainAttr.UserID] = BasePage.GetUniqueDomainID();
                    domainmanagement.CreateDomain(createDomain);
                }
                if (!domainmanagement.IsDomainExist(DomainNames[2]))
                {
                    createDomain[DomainManagement.DomainAttr.DomainName] = DomainNames[2];
                    createDomain[DomainManagement.DomainAttr.RoleName] = RoleNames[2];
                    createDomain[DomainManagement.DomainAttr.UserID] = BasePage.GetUniqueDomainID();
                    domainmanagement.CreateDomain(createDomain);
                }
                if (!domainmanagement.IsDomainExist(DomainNames[3]))
                {
                    createDomain[DomainManagement.DomainAttr.DomainName] = DomainNames[3];
                    createDomain[DomainManagement.DomainAttr.RoleName] = RoleNames[3];
                    createDomain[DomainManagement.DomainAttr.UserID] = BasePage.GetUniqueDomainID();
                    domainmanagement.CreateDomain(createDomain);
                }
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                if (!rolemanagement.RoleExists(RoleNames[0], DomainNames[0]))
                {
                    rolemanagement.CreateRole(DomainNames[0], RoleNames[0], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[1], DomainNames[1]))
                {
                    rolemanagement.CreateRole(DomainNames[1], RoleNames[1], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[2], DomainNames[2]))
                {
                    rolemanagement.CreateRole(DomainNames[2], RoleNames[2], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[3], DomainNames[3]))
                {
                    rolemanagement.CreateRole(DomainNames[3], RoleNames[3], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[4], DomainNames[0]))
                {
                    rolemanagement.CreateRole(DomainNames[0], RoleNames[4], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[5], DomainNames[1]))
                {
                    rolemanagement.CreateRole(DomainNames[1], RoleNames[5], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[6], DomainNames[1]))
                {
                    rolemanagement.CreateRole(DomainNames[1], RoleNames[6], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[7], DomainNames[1]))
                {
                    rolemanagement.CreateRole(DomainNames[1], RoleNames[7], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[8], DomainNames[1]))
                {
                    rolemanagement.CreateRole(DomainNames[1], RoleNames[8], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[9], DomainNames[1]))
                {
                    rolemanagement.CreateRole(DomainNames[1], RoleNames[9], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[10], DomainNames[2]))
                {
                    rolemanagement.CreateRole(DomainNames[2], RoleNames[10], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[11], DomainNames[3]))
                {
                    rolemanagement.CreateRole(DomainNames[3], RoleNames[11], roletype: "");
                }
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                if (!usermanagement.IsGroupExist(Groups[0], DomainNames[1]))
                {
                    usermanagement.CreateGroup(DomainNames[1], Groups[0]);
                }
                if (!usermanagement.IsGroupExist(Groups[1], DomainNames[1]))
                {
                    usermanagement.CreateGroup(DomainNames[1], Groups[1]);
                }
                if (!usermanagement.IsGroupExist(Groups[3], DomainNames[1]))
                {
                    usermanagement.CreateSubGroup(Groups[1], Groups[3]);
                }
                if (!usermanagement.IsGroupExist(Groups[0], DomainNames[2]))
                {
                    usermanagement.CreateGroup(DomainNames[2], Groups[0]);
                }
                if (!usermanagement.IsGroupExist(Groups[1], DomainNames[2]))
                {
                    usermanagement.CreateGroup(DomainNames[2], Groups[1]);
                }
                if (!usermanagement.IsGroupExist(Groups[3], DomainNames[2]))
                {
                    usermanagement.CreateSubGroup(Groups[1], Groups[3]);
                }
                if (!usermanagement.IsGroupExist(Groups[2], DomainNames[3]))
                {
                    usermanagement.CreateGroup(DomainNames[3], Groups[2]);
                }
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                usermanagement.ClearBtn().Click();
                ExecutedSteps++;
                //Step 4: Then map the following domain, role and group to a field in the LDAP for each of the these settings by selecting Edit DataMap button in the Domain, Role Management and Edit DataMapping button in the User Mangement.
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                ldapdatamapping = new LdapDataMapping();
                ldapdatamapping.OpenLDAPDataMap();
                ldapdatamapping.RemoveSelectedLDAPValues();
                int resultcount = 0;
                if (ldapdatamapping.AddLDAPValues(ServerName, "Queensway", "TestDomain"))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Queensway LDAP Value is successfully added to TestDomain");
                }
                if (ldapdatamapping.AddLDAPValues(ServerName, "Sherway", "Domain2"))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Sherway LDAP Value is successfully added to Domain2");
                }
                ldapdatamapping.CloseLDAPDataMap();
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                ldapdatamapping.OpenLDAPDataMap("TestDomain");
                ldapdatamapping.RemoveSelectedLDAPValues();
                if (ldapdatamapping.AddLDAPValues(ServerName, "QWDoctorsRole^Sherway", "Role1"))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("QWDoctorsRole^Sherway LDAP Value is successfully added to TestDomain->Role1");
                }
                if (ldapdatamapping.AddLDAPValues(ServerName, "QWNursesRole^Sherway", "Role2"))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("QWNursesRole^Sherway LDAP Value is successfully added to TestDomain->Role2");
                }
                ldapdatamapping.CloseLDAPDataMap();
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                ldapdatamapping.OpenLDAPDataMap("TestDomain", true);
                ldapdatamapping.RemoveSelectedLDAPValues();
                if (ldapdatamapping.AddLDAPValues(ServerName, "DiagnosticImaging^Queensway", "G2^TestDomain"))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("DiagnosticImaging^Queensway LDAP Value is successfully added to G2^TestDomain");
                }
                if (ldapdatamapping.AddLDAPValues(ServerName, "Nurses^DiagnosticImaging^Queensway", "G2.1^G2^TestDomain"))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Nurses^DiagnosticImaging^Queensway LDAP Value is successfully added to G2.1^G2^TestDomain");
                }
                ldapdatamapping.CloseLDAPDataMap();
                if (resultcount == 6)
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
                //Step 5: Select the User Management Tab and select TestDomain in the domain field, enter * and then select search.
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.SearchWithoutFilter(DomainNames[1]);
                Expected = new string[] { "qw.admin (Queensway Admin) Activated SiteAdmin", "marcus.welby (Marcus Welby) Activated User", "tmhlivesay (Tracey Livesay) Activated User" };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
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
                //Step 6: Select Group G2, enter * and then select search.
                usermanagement.SearchWithoutFilter(DomainNames[1], Groups[1]);
                Expected = new string[] { "marcus.welby (Marcus Welby) Activated User", "tmhlivesay (Tracey Livesay) Activated User" };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
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
                //Step 7: Select Group G2.1, enter * and then select search.
                usermanagement.SearchWithoutFilter(DomainNames[1], subgroupname: Groups[3]);
                Expected = new string[] { "tmhlivesay (Tracey Livesay) Activated User" };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
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
                //Step 8: Select the Domain2, enter * and then select search.
                usermanagement.SearchWithoutFilter(DomainNames[2]);
                Expected = new string[] { "shw.admin (Sherway Admin) Activated SiteAdmin", "ben.kildare (Ben Kildare) Activated User", "akagi (Naoko Akagi) Activated User" };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
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
            finally
            {
                servicetool.CloseServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolUserMgmtDatabaseTab();
                servicetool.SetMode(2);
                servicetool.SetEnableFeaturesGeneral();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableSelfEnrollment();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                wpfobject.ClickOkPopUp();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                if (basepage.NodeExist(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server[@id='" + ServerName + "']"))
                {
                    basepage.RemoveNode(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server[@id='" + ServerName + "']");
                }
                servicetool.RestartIISUsingexe();
            }
        }

        public TestCaseResult Test_27677(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            DomainManagement domainmanagement = null;
            RoleManagement rolemanagement = null;
            Studies studies = null;
            UserManagement usermanagement = null;
            UserPreferences userpreferences = null;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            string ServerName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ServerName");
            string HostName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "HostName");
            string Port = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Port"));
            string ExistingHostName = string.Empty;
            int resultcount = 0;
            string[] DomainNames = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "DomainNames")).Split(':');
            string[] RoleNames = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "RoleNames")).Split(':');
            string[] Groups = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Groups")).Split(':');
            string[] Expected = null;
            string[] Actual = null;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                servicetool.CloseServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolUserMgmtDatabaseTab();
                servicetool.SetMode(1);
                servicetool.CloseServiceTool();
                string LdapXML = File.ReadAllText((String)ReadExcel.GetTestData(filepath, "TestData", testid, "LdapXMLPath"));
                basepage.ChangeAttributeValue(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server", "enabled", "False");
                if (!basepage.NodeExist(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server[@id='" + ServerName + "']"))
                {
                    basepage.InsertNode(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/servers", LdapXML, false);
                }
                basepage.ChangeAttributeValue(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server[@id='" + ServerName + "']", "enabled", "True");
                basepage.InsertNode(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server[@id='" + ServerName + "']/options/hosts", string.Concat("<host name=\"", HostName, "\" port=\"", Port, "\" />"));
                servicetool.RestartIISUsingexe();
                //Step 1: In the iConnect service tool select the LDAP tab
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.LDAP_Tab);
                Expected = new string[] { "Global Options", "Servers", "Ldap/Local Responsibilities" };
                Actual = wpfobject.GetTabWpf(1).Pages.Select(page => page.Name).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("LDAP contains Global Options, Servers, Ldap/Local Responsibilities tab");
                }
                if (string.Equals(wpfobject.GetTabWpf(1).SelectedTab.Name, "Global Options"))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("LDAP contains Global Options as default tab");
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
                //Step 2: Select the Servers Tab
                wpfobject.GetTabWpf(1).SelectTabPage(1);
                wpfobject.ClickButton(ServiceTool.ModifyBtn_Name, 1);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;
                //Step 3: Select ica-ldap.2 and click on the Detail button
                GroupBox ldap_grp1 = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(servicetool.GetCurrentTabItem(), ServiceTool.LDAP.Name.LdapServerListGrp, 1);
                ListView datagrid1 = wpfobject.GetAnyUIItem<GroupBox, ListView>(ldap_grp1, ServiceTool.LDAP.ID.LdapServersList);
                foreach (var row in datagrid1.Rows)
                {
                    if (row.Cells[0].Text.ToLower().Equals(ServerName.ToLower()))
                    {
                        row.Focus();
                        wpfobject.WaitTillLoad();
                        row.Click();
                        wpfobject.WaitTillLoad();
                        break;
                    }
                }
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ServiceTool.LDAP.ID.DetailsBtn);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindowByTitle("LDAP Server Control Form");//ServiceTool.LDAP.Name.LdapServerDetailWindow);
                wpfobject.WaitTillLoad();
                GroupBox ldap_grp = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(servicetool.GetCurrentTabItem(), ServiceTool.LDAP.Name.ServerHostsGrp, 1);
                ListView datagrid = wpfobject.GetAnyUIItem<GroupBox, ListView>(ldap_grp, ServiceTool.LDAP.ID.ServerHostsListList);
                resultcount = 0;
                if (string.Equals(HostName, datagrid.Rows[0].Cells[0].Text))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("The Given HostName " + HostName + " is present");
                }
                GroupBox siteDomain_grp = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(servicetool.GetCurrentTabItem(), ServiceTool.LDAP.Name.SiteDomainNamesGrp, 1);
                TextBox tb = wpfobject.GetAnyUIItem<GroupBox, TextBox>(siteDomain_grp, ServiceTool.LDAP.ID.SiteDomainNamesTxt);
                Expected = new string[] { "SuperAdminGroup" };
                string actualvalue = tb.Text;
                if (Expected.All(exp => actualvalue.Contains(exp)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("All Site Domain Names are present");
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
                //Step 4: Select the Mapping Details tab then Select Identity from the Type drop down menu.
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 5: Click on OK , apply , Restart IIS
                wpfobject.ClickButton("Close", 0);
                wpfobject.WaitTillLoad();
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.ClickButton(ServiceTool.ApplyBtn_Name, 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                ExecutedSteps++;
                //Step 6:  Login iCA as a registered user UID = ica.administrator PID = admin.13579
                login.LoginIConnect("ica.administrator", "admin.13579");
                ExecutedSteps++;
                //Step 7: Double click on the SuperAdminGroup
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain("SuperAdminGroup");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.ClickEditDomain();
                ExecutedSteps++;
                //Step 8: Select a Data Source and move it to the connected side
                domainmanagement.ConnectAllDataSources();
                ExecutedSteps++;
                //Step 9: click on Save
                domainmanagement.ClickSaveEditDomain();
                if (login.IsTabSelected("Domain Management"))
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
                //Step 10: Select the User management tab and select the SuperAdminGroup Domain, enter * click on the Search button.
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.SearchWithoutFilter("SuperAdminGroup");
                resultcount = 0;
                Expected = new string[] { "ica.administrator (IConnectAccess Administrator) Activated SiteAdmin", "victoria.dassen (Vicky Dassen) Activated User", "salmon (Salmon Ben Judah) Activated User" };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Filter contains the users ica.administrator, victoria.dassen and salmon");
                }
                if (usermanagement.NewGrpBtn().Displayed && usermanagement.NewSubGrpBtn().Displayed && usermanagement.EditGrpBtn().Displayed && usermanagement.DelGrpBtn().Displayed && usermanagement.MoveGrpBtn().Displayed && usermanagement.DataMappingBtn().Displayed)
                {
                    resultcount++;
                    Logger.Instance.InfoLog("6 Group Buttons are displayed");
                }
                if (!usermanagement.NewUsrBtn().Displayed && !usermanagement.EditUsrBtn().Displayed && usermanagement.DelUsrBtn().Displayed)
                {
                    resultcount++;
                    Logger.Instance.InfoLog("New and Edit user Buttons are not displayed. Delete user button is displayed");
                }
                if (resultcount == 3)
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
                //Step 11: Logout and log back in as victoria.dassen/.vcd.13579
                login.Logout();
                login.LoginIConnect("victoria.dassen", ".vcd.13579");
                ExecutedSteps++;
                //Step 12: Change to all dates and click the search button
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(LastName: "*", Study_Performed_Period: "All Dates");
                Dictionary<int, string[]> SearchResults = BasePage.GetSearchResults();
                if (SearchResults.Count > 0)
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
                //Step 13: Click on options User Preferences
                userpreferences = new UserPreferences();
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                if (string.Equals(userpreferences.UserPreferenceName().Text, "User Preferences"))
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
                //Step 14: Edit some parameter and click OK *^>^* click Close
                userpreferences.PNGRadioBtn().Click();
                userpreferences.EmailFormatText().Click();
                userpreferences.CloseUserPreferences();
                ExecutedSteps++;
                //Step 15: Reopen the user preferences page
                userpreferences.OpenUserPreferences();
                resultcount = 0;
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                if (!userpreferences.PNGRadioBtn().Selected)
                {
                    Logger.Instance.ErrorLog("PNGRadioBtn is not Selected");
                    resultcount++;
                }
                if (!userpreferences.EmailFormatText().Selected)
                {
                    Logger.Instance.ErrorLog("EmailFormatText is not Selected");
                    resultcount++;
                }
                if (resultcount == 0)
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
                userpreferences.CloseUserPreferences();
                //Step 16: Logout and log back in as ica.administrator/admin.13579
                login.Logout();
                login.LoginIConnect("ica.administrator", "admin.13579");
                ExecutedSteps++;
                //Step 17: Create new Domains, Roles and Groups as per the TestData Tab
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                Dictionary<Object, String> createDomain = domainmanagement.CreateDomainAttr();
                if (!domainmanagement.IsDomainExist(DomainNames[0]))
                {
                    createDomain[DomainManagement.DomainAttr.DomainName] = DomainNames[0];
                    createDomain[DomainManagement.DomainAttr.RoleName] = RoleNames[0];
                    domainmanagement.CreateDomain(createDomain);
                }
                if (!domainmanagement.IsDomainExist(DomainNames[1]))
                {
                    createDomain[DomainManagement.DomainAttr.DomainName] = DomainNames[1];
                    createDomain[DomainManagement.DomainAttr.RoleName] = RoleNames[1];
                    createDomain[DomainManagement.DomainAttr.UserID] = BasePage.GetUniqueDomainID();
                    domainmanagement.CreateDomain(createDomain);
                }
                if (!domainmanagement.IsDomainExist(DomainNames[2]))
                {
                    createDomain[DomainManagement.DomainAttr.DomainName] = DomainNames[2];
                    createDomain[DomainManagement.DomainAttr.RoleName] = RoleNames[2];
                    createDomain[DomainManagement.DomainAttr.UserID] = BasePage.GetUniqueDomainID();
                    domainmanagement.CreateDomain(createDomain);
                }
                if (!domainmanagement.IsDomainExist(DomainNames[3]))
                {
                    createDomain[DomainManagement.DomainAttr.DomainName] = DomainNames[3];
                    createDomain[DomainManagement.DomainAttr.RoleName] = RoleNames[3];
                    createDomain[DomainManagement.DomainAttr.UserID] = BasePage.GetUniqueDomainID();
                    domainmanagement.CreateDomain(createDomain);
                }
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                if (!rolemanagement.RoleExists(RoleNames[0], DomainNames[0]))
                {
                    rolemanagement.CreateRole(DomainNames[0], RoleNames[0], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[1], DomainNames[1]))
                {
                    rolemanagement.CreateRole(DomainNames[1], RoleNames[1], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[2], DomainNames[2]))
                {
                    rolemanagement.CreateRole(DomainNames[2], RoleNames[2], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[3], DomainNames[3]))
                {
                    rolemanagement.CreateRole(DomainNames[3], RoleNames[3], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[4], DomainNames[0]))
                {
                    rolemanagement.CreateRole(DomainNames[0], RoleNames[4], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[5], DomainNames[1]))
                {
                    rolemanagement.CreateRole(DomainNames[1], RoleNames[5], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[6], DomainNames[1]))
                {
                    rolemanagement.CreateRole(DomainNames[1], RoleNames[6], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[7], DomainNames[1]))
                {
                    rolemanagement.CreateRole(DomainNames[1], RoleNames[7], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[8], DomainNames[1]))
                {
                    rolemanagement.CreateRole(DomainNames[1], RoleNames[8], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[9], DomainNames[1]))
                {
                    rolemanagement.CreateRole(DomainNames[1], RoleNames[9], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[10], DomainNames[2]))
                {
                    rolemanagement.CreateRole(DomainNames[2], RoleNames[10], roletype: "");
                }
                if (!rolemanagement.RoleExists(RoleNames[11], DomainNames[3]))
                {
                    rolemanagement.CreateRole(DomainNames[3], RoleNames[11], roletype: "");
                }
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                if (!usermanagement.IsGroupExist(Groups[0], DomainNames[1]))
                {
                    usermanagement.CreateGroup(DomainNames[1], Groups[0]);
                }
                if (!usermanagement.IsGroupExist(Groups[1], DomainNames[1]))
                {
                    usermanagement.CreateGroup(DomainNames[1], Groups[1]);
                }
                if (!usermanagement.IsGroupExist(Groups[3], DomainNames[1]))
                {
                    usermanagement.CreateSubGroup(Groups[1], Groups[3]);
                }
                if (!usermanagement.IsGroupExist(Groups[0], DomainNames[2]))
                {
                    usermanagement.CreateGroup(DomainNames[2], Groups[0]);
                }
                if (!usermanagement.IsGroupExist(Groups[1], DomainNames[2]))
                {
                    usermanagement.CreateGroup(DomainNames[2], Groups[1]);
                }
                if (!usermanagement.IsGroupExist(Groups[2], DomainNames[3]))
                {
                    usermanagement.CreateGroup(DomainNames[3], Groups[2]);
                }
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                usermanagement.ClearBtn().Click();
                ExecutedSteps++;
                //Step 18: Select the User management tab and select Domain1, enter * and then click on Search.
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.SearchWithoutFilter(DomainNames[0]);
                Expected = new string[] { "admin1 (Domain1 Admin) Activated SiteAdmin", "ben.kenobi (ObiWan Kenobi) Activated User" };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
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
                //Step 19: Change Domain to"Domain2", enter * and then click on Search.
                usermanagement.SearchWithoutFilter(DomainNames[2]);
                Expected = new string[] { "admin2 (Domain2 Admin) Activated SiteAdmin", "young.skywalker (Luke Skywalker) Activated User", "jabba (Jabba Hutt) Activated User" };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
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
                //Step 20: Change Domain to"Domain3", enter * and then click on Search.
                usermanagement.SearchWithoutFilter(DomainNames[3]);
                Expected = new string[] { "admin3 (Domain3 Admin) Activated SiteAdmin", "princess (Leia Organa) Activated User", "wookie (Chewbakka na) Activated User", "scoundrel (Han Solo) Activated User" };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
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
                //Step 21: Change Domain to"TestDomain", enter * and then click on Search.
                usermanagement.SearchWithoutFilter(DomainNames[1]);
                Expected = new string[] { "testdomain.admin (Victoria Admin) Activated SiteAdmin", "peter (Simon Peter ben Jonah) Activated GroupAdmin", "john (John Boanerges) Activated User", "samuel (Samuel ben Elkanah) Activated User", "boaz (Boaz Ben Salmon) Activated User", "marym (Mary Magdalene) Activated User" };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
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
                //Step 22: Change Domain to"TestDomain", enter * and then click on Clear Search button
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                usermanagement.ClearUsrBtn().Click();
                ExecutedSteps++;
                //Step 23: Enter John in the Filter Users field and click on Search.
                resultcount = 0;
                if (usermanagement.SearchUser("john"))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("User John is present after search");
                }
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                if (usermanagement.GroupList().Any(grp => string.Equals(grp.Text.Trim(), "G2")))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("The Group G2 is present after search");
                }
                if (resultcount == 2)
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
                //Step 24: Select "Clear Search"
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                usermanagement.ClearUsrBtn().Click();
                ExecutedSteps++;
                //Step 25: Select Ungrouped from the left hand side, enter * and then click on Search.
                usermanagement.SearchWithoutFilter(DomainNames[1], "Ungrouped");
                Expected = new string[] { "testdomain.admin (Victoria Admin) Activated SiteAdmin" };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
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
                //Step 26: Select G1 from the left hand side enter *and then click on Search.
                usermanagement.SearchWithoutFilter(DomainNames[1], Groups[0]);
                Expected = new string[] { "peter (Simon Peter ben Jonah) Activated GroupAdmin", "boaz (Boaz Ben Salmon) Activated User" };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
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
                //Step 27: Select G2 from the left hand side enter *and then click on Search.
                usermanagement.SearchWithoutFilter(DomainNames[1], Groups[1]);
                Expected = new string[] { "john (John Boanerges) Activated User", "marym (Mary Magdalene) Activated User" };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
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
                //Step 28: Select G2.1 from the left hand side enter *and then click on Search.
                usermanagement.SearchWithoutFilter(DomainNames[1], subgroupname: Groups[3]);
                Expected = new string[] { "samuel (Samuel ben Elkanah) Activated User" };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
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
                //Step 29: Logout and login as peter/ .ptr.13579
                login.Logout();
                login.LoginIConnect("peter", ".ptr.13579");
                ExecutedSteps++;
                //Step 30: Logout from ICA
                login.Logout();
                ExecutedSteps++;
                //Step 31: Change password in LDAP
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 32: Select the Tree mode 
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 33: Log into ICA with User"peter"and the new password
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 34: reset Password
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 35: Login as Peter with password: .ptr.13579
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
            finally
            {
                servicetool.CloseServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolUserMgmtDatabaseTab();
                servicetool.SetMode(2);
                servicetool.SetEnableFeaturesGeneral();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableSelfEnrollment();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                wpfobject.ClickOkPopUp();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                if (basepage.NodeExist(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server[@id='" + ServerName + "']"))
                {
                    basepage.RemoveNode(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server[@id='" + ServerName + "']");
                }
                servicetool.RestartIISUsingexe();
            }
        }

        public TestCaseResult Test_27678(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            Studies studies = new Studies();
            UserPreferences userpreferences = null;
            DomainManagement domainmanagement = null;
            UserManagement usermanagement = new UserManagement();
            string ServerName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ServerName");
            string HostName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "HostName");
            string Port = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Port"));
            int ExecutedSteps = -1;
            int resultcount = 0;
            string DomainName = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "DomainNames"));
            string[] RoleNames = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "RoleNames")).Split(':');
            string[] Expected = null;
            string[] Actual = null;
            result = new TestCaseResult(stepcount);
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                //Step 1: Remote Login to the ICA server
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 2: Open the Window services page and confirm the OpenLDAP service is running, if it is not running start the service.
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 3: In the Merge iConnect Access Service Tool --*^>^* LDAP tab --*^>^* Servers tab --*^>^* Select ica.ldap.1 check box and unselect the other check boxes.
                servicetool.CloseServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolUserMgmtDatabaseTab();
                servicetool.SetMode(1);
                servicetool.CloseServiceTool();
                string LdapXML = File.ReadAllText((String)ReadExcel.GetTestData(filepath, "TestData", testid, "LdapXMLPath"));
                basepage.ChangeAttributeValue(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server", "enabled", "False");
                if (!basepage.NodeExist(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server[@id='" + ServerName + "']"))
                {
                    basepage.InsertNode(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/servers", LdapXML, false);
                }
                basepage.ChangeAttributeValue(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server[@id='" + ServerName + "']", "enabled", "True");
                basepage.InsertNode(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server[@id='" + ServerName + "']/options/hosts", string.Concat("<host name=\"", HostName, "\" port=\"", Port, "\" />"));
                servicetool.RestartIISUsingexe();
                ExecutedSteps++;
                //Step 4: Login to ICA as a registered user 
                login.LoginIConnect("esau", ".esau.123");
                if (login.IsTabSelected("Studies"))
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
                //Step 5: Change the Study Performed box from Last 2 Days to All Dates and click the search button
                studies.SearchStudy(LastName: "*", Study_Performed_Period: "All Dates");
                Dictionary<int, string[]> SearchResults = BasePage.GetSearchResults();
                if (SearchResults.Count > 0)
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
                //Step 6: Click on options User Preferences
                userpreferences = new UserPreferences();
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                if (string.Equals(userpreferences.UserPreferenceName().Text, "User Preferences"))
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
                //Step 7: Edit some parameter and click OK to Close
                userpreferences.PNGRadioBtn().Click();
                userpreferences.EmailFormatText().Click();
                userpreferences.CloseUserPreferences();
                ExecutedSteps++;
                //Step 8: Reopen the user preferences page
                userpreferences.OpenUserPreferences();
                resultcount = 0;
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                if (!userpreferences.PNGRadioBtn().Selected)
                {
                    Logger.Instance.ErrorLog("PNGRadioBtn is not Selected");
                    resultcount++;
                }
                if (!userpreferences.EmailFormatText().Selected)
                {
                    Logger.Instance.ErrorLog("EmailFormatText is not Selected");
                    resultcount++;
                }
                userpreferences.CloseUserPreferences();
                if (resultcount == 0)
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
                //Step 9: Logout and Login back in as a registered user 
                login.Logout();
                login.LoginIConnect("jacob", ".jacob.123");
                if (login.IsTabSelected("User Management"))
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
                //Step 10: Click on Search
                usermanagement.SearchWithoutFilter();
                resultcount = 0;
                Expected = new string[] { "esau () Activated User" };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Filter contains the users esau");
                }
                if (login.TabsList().Count == 5)
                {
                    resultcount++;
                    Logger.Instance.InfoLog("User contains the 5 Tabs");
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
                //Step 11: Select the domain management tab
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                if (login.IsTabSelected("Domain Management"))
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
                //Step 12: Logout
                login.Logout();
                ExecutedSteps++;
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
            finally
            {
                servicetool.CloseServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolUserMgmtDatabaseTab();
                servicetool.SetMode(2);
                servicetool.SetEnableFeaturesGeneral();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableSelfEnrollment();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                wpfobject.ClickOkPopUp();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                if (basepage.NodeExist(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server[@id='" + ServerName + "']"))
                {
                    basepage.RemoveNode(@"C:\WebAccess\WebAccess\Config\DSA\DSAServerManagerConfiguration.xml", "/server[@id='" + ServerName + "']");
                }
                servicetool.RestartIISUsingexe();
            }
        }

        /// <summary>
        /// 8.0 Auto Role and Combination role mapping
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27679(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            string ServerName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ServerName");
            string HostName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "HostName");
            string Port = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Port"));
            string[] Groups = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Groups")).Split(':');
            string DomainName = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "DomainNames"));

            string Profile1 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Profile1");
            string Profile2 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Profile2");

            string[] LoginDetails = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "LoginDetails")).Split('!');
            string RoleFilters = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "RoleFilters"));
            string[] Filters = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Filters")).Split('!');
            string[] Roles = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "RoleNames")).Split(':');
            string[] Keys = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Keys")).Split(':');
            string[] Datasources = new string[] { new Login().GetHostName(Config.EA77), new Login().GetHostName(Config.PACS2), new Login().GetHostName(Config.SanityPACS) };
            string[] DignityUsers = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "DignityUsers")).Split(':');
            string LdapXMLFile = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LdapXMLPath");
            string LdapXML = File.ReadAllText(LdapXMLFile);
            string profile1path = string.Concat(Path.GetDirectoryName(LdapXMLFile), "\\", Path.GetFileName(Profile1));
            string profile2path = string.Concat(Path.GetDirectoryName(LdapXMLFile), "\\", Path.GetFileName(Profile2));
            string Profile1updated = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Profile1Update");
            string Profile2updated = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Profile2Update");
            Studies studies = new Studies();
            StudyViewer viewer = null;

            DomainManagement domainmanagement = null;
            UserManagement usermanagement = null;
            RoleManagement rolemanagement = null;
            Dictionary<int, string[]> SearchResults = null;
            int resultcount = 0;
            string[] Expected = null;
            string[] Actual = null;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                //Step 1: Copy Dignity-001.xml and Dignity-002.xml to C:\Program Files (x86)\Cedara\WebAccess in the ICA server under test.
                File.Copy(profile1path, Profile1, true);
                File.Copy(profile2path, Profile2, true);

                servicetool.CloseServiceTool();
                basepage.ChangeAttributeValue(Config.DSAServerManagerConfiguration, "/server", "enabled", "False");
                if (basepage.NodeExist(Config.DSAServerManagerConfiguration, "/server[@id='" + ServerName + "']"))
                {
                    basepage.RemoveNode(Config.DSAServerManagerConfiguration, "/server[@id='" + ServerName + "']");
                }
                basepage.InsertNode(Config.DSAServerManagerConfiguration, "/servers", LdapXML, false);
                basepage.InsertNode(Config.DSAServerManagerConfiguration, "/server[@id='" + ServerName + "']/options/hosts", string.Concat("<host name=\"", HostName, "\" port=\"", Port, "\" />"));
                servicetool.RestartIISUsingexe();
                ExecutedSteps++;
                //Step 2: 
                ExecutedSteps++;
                //Step 3: Login to ica and create a Domain "Sacramento"
                string Username = Config.adminUserName;
                string Password = Config.adminPassword;
                login.LoginIConnect(Username, Password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                Dictionary<Object, String> createDomain = domainmanagement.CreateDomainAttr();
                if (!domainmanagement.IsDomainExist(DomainName))
                {
                    createDomain[DomainManagement.DomainAttr.DomainName] = DomainName;
                    domainmanagement.CreateDomain(createDomain);
                }
                if (domainmanagement.SearchDomain(DomainName))
                {
                    domainmanagement.SelectDomain(DomainName);
                    domainmanagement.ClickEditDomain();
                    domainmanagement.SetCheckBoxInEditDomain("datatransfer", 0);
                    domainmanagement.SetCheckBoxInEditDomain("datadownload", 0);
                    domainmanagement.SetCheckBoxInEditDomain("grant", 0);
                    domainmanagement.ClickSaveEditDomain();
                }
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                foreach (string group in Groups)
                {
                    if (!usermanagement.IsGroupExist(group, DomainName))
                    {
                        usermanagement.CreateGroup(DomainName, group);
                    }
                }
                login.Logout();
                ExecutedSteps++;
                //Step 4:  Click on modify and select the enable Ldap box 
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.LDAP_Tab);
                wpfobject.GetTabWpf(1).SelectTabPage(1);
                wpfobject.ClickButton(ServiceTool.ModifyBtn_Name, 1);
                wpfobject.WaitTillLoad();
                GroupBox ldap_grp1 = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(servicetool.GetCurrentTabItem(), ServiceTool.LDAP.Name.LdapServerListGrp, 1);
                ListView datagrid1 = wpfobject.GetAnyUIItem<GroupBox, ListView>(ldap_grp1, ServiceTool.LDAP.ID.LdapServersList);
                datagrid1.Rows[0].Click();
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ServiceTool.LDAP.ID.DetailsBtn);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindowByTitle("LDAP Server Control Form");
                wpfobject.WaitTillLoad();
                ExecutedSteps++;
                //Step 5: confirm the following settings
                resultcount = 0;
                GroupBox ldap_grp = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(servicetool.GetCurrentTabItem(), ServiceTool.LDAP.Name.ServerHostsGrp, 1);
                ListView datagrid = wpfobject.GetAnyUIItem<GroupBox, ListView>(ldap_grp, ServiceTool.LDAP.ID.ServerHostsListList);
                if (string.Equals(HostName, datagrid.Rows[0].Cells[0].Text))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("The Given HostName " + HostName + " is present");
                }
                GroupBox siteDomain_grp = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(servicetool.GetCurrentTabItem(), ServiceTool.LDAP.Name.SiteDomainNamesGrp, 1);
                TextBox tb = wpfobject.GetAnyUIItem<GroupBox, TextBox>(siteDomain_grp, ServiceTool.LDAP.ID.SiteDomainNamesTxt);
                Expected = new string[] { DomainName };
                string actualvalue = tb.Text;
                if (Expected.All(exp => actualvalue.Contains(exp)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("All Site Domain Names are present");
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
                //Step 6: Select the Host name and Click on the Test Connection button
                datagrid.Rows[0].Cells[0].Click();
                wpfobject.ClickButton("Test Connection", 1);
                string Name = WpfObjects._mainWindow.Get(SearchCriteria.ByAutomationId("65535")).Name;
                WpfObjects._mainWindow.Get(SearchCriteria.ByAutomationId("2")).Click();
                if (Name.EndsWith("Succeeded"))
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
                //Step 7: Click on the Data Model Tab
                wpfobject.SelectTabFromTabItems("Data Model");
                wpfobject.WaitTillLoad();
                Expected = new string[] { "OU=DignityHealth,DC=ica,DC=internal", "OU=Sacramento,OU=Regional_Sites", "OU=Sacramento,OU=IConnectAccess,OU=Merge,OU=Applications" };
                Actual = WpfObjects._mainWindow.GetMultiple(SearchCriteria.ByClassName("TextBox")).Select(ele => ((TextBox)ele).BulkText).ToArray();
                if (Expected.All(exp => Actual.Contains(exp)))
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
                //Step 8: Click on the Generate Rules Button
                wpfobject.ClickButton("Generate Rules", 1);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindowByTitle("Ldap Role Management Form");
                wpfobject.WaitTillLoad();
                ExecutedSteps++;
                //Step 9: At the bottom of the Role Management Rules page there is a new button"Show User Account Details" Click on the button to select it.
                result.steps[++ExecutedSteps].status = "Fail";
                /*wpfobject.ClickButton("Show User Account Details", 1);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindowByTitle("User Details Search");
                wpfobject.WaitTillLoad();
                ExecutedSteps++;*/
                //Step 10: Add a"d"to the User account name box and Click on the Search button
                result.steps[++ExecutedSteps].status = "Fail";
                /*GroupBox group = WpfObjects._mainWindow.Get<GroupBox>(SearchCriteria.ByClassName("GroupBox"));
                IUIItem[] TextBox = group.GetMultiple(SearchCriteria.ByClassName("TextBox"));
                TextBox[0].SetValue("d");
                wpfobject.ClickButton("Search", 1);
                ExecutedSteps++;*/
                //Step 11: Enter merge_admin and .adm.13579 select the Ldap circle and click on OK
                result.steps[++ExecutedSteps].status = "Fail";
                /*wpfobject.GetMainWindowByTitle("User Credential Form");
                wpfobject.WaitTillLoad();
                WpfObjects._mainWindow.Get(SearchCriteria.ByClassName("TextBox")).SetValue(Username);
                WpfObjects._mainWindow.Get(SearchCriteria.ByClassName("PasswordBox")).SetValue(Password);
                wpfobject.ClickRadioButton("Local", 1);
                wpfobject.ClickButton("OKButton");
                wpfobject.GetMainWindowByTitle("User Details Search");
                wpfobject.WaitTillLoad();*/

                //Step 12: Add a"*"to the User account name box and click
                result.steps[++ExecutedSteps].status = "Fail";
                /*group = WpfObjects._mainWindow.Get<GroupBox>(SearchCriteria.ByClassName("GroupBox"));
                TextBox = group.GetMultiple(SearchCriteria.ByClassName("TextBox"));
                TextBox[0].SetValue("*");
                wpfobject.ClickButton("Search", 1);*/

                //Step 13: Click on Clear Search Details and add a DN to the Search Target DN box and click on Search
                result.steps[++ExecutedSteps].status = "Fail";
                /*wpfobject.ClickButton("Clear Search Details", 1);
                group = WpfObjects._mainWindow.Get<GroupBox>(SearchCriteria.ByClassName("GroupBox"));
                TextBox = group.GetMultiple(SearchCriteria.ByClassName("TextBox"));
                TextBox[1].SetValue("DN");
                wpfobject.ClickButton("Search", 1);*/

                //Step 14: Search for a specific user Add"Levi"in the User account name box and click on search
                result.steps[++ExecutedSteps].status = "Fail";
                /*group = WpfObjects._mainWindow.Get<GroupBox>(SearchCriteria.ByClassName("GroupBox"));
                TextBox = group.GetMultiple(SearchCriteria.ByClassName("TextBox"));
                TextBox[0].SetValue("Levi");
                wpfobject.ClickButton("Search", 1);*/

                //Step 15: Click Done to Exit
                result.steps[++ExecutedSteps].status = "Fail";
                /*wpfobject.ClickButton("Done", 1);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindowByTitle("Ldap Role Management Form");
                wpfobject.WaitTillLoad();
                ExecutedSteps++;*/
                //Step 16: Select by clicking on the"Create Role" Tab
                wpfobject.SelectTabFromTabItems("Create Roles");
                wpfobject.WaitTillLoad();
                ExecutedSteps++;
                //Step 17: Click on Get Details
                wpfobject.ClickButton("Get Details", 1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Enable Download", 1);
                wpfobject.ClickButton("Enable Transfer", 1);
                wpfobject.ClickButton("Enable Upload", 1);
                wpfobject.ClickButton("Enable Grant Access", 1);
                wpfobject.ClickRadioButton("Anyone", 1);
                resultcount = 0;
                GroupBox DS = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(servicetool.GetCurrentTabItem(), "Domain Details", 1);
                Actual = DS.GetMultiple(SearchCriteria.ByClassName("TextBlock")).Select(val => val.Name).ToArray();
                Expected = Datasources;
                if (Expected.All(exp => Actual.Contains(exp)))
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
                //Step 18: Click on Load Details From File and select and load the Previously saved Profile
                wpfobject.ClickButton("Load Details From File", 1);
                wpfobject.WaitTillLoad();
                wpfobject.SetText("File name:", Profile2, 1);
                wpfobject.ClickButton("Open", 1);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;
                //Step 19: Click on Tab Mapped Combination Roles 
                wpfobject.GetTabWpf(1).SelectTabPage(1);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;
                //Step 20: Click on the Combine Roles button with an arrow pointing down.
                IUIItem[] Button = WpfObjects._mainWindow.GetMultiple(SearchCriteria.ByClassName("Button"));
                Button[0].Click();
                ExecutedSteps++;
                //Step 21: Click on the Create Combination Roles to add these roles in the ICA server, make sure there is an ICA running before executing this function .
                TestStack.White.InputDevices.AttachedKeyboard keyboard = WpfObjects._mainWindow.Keyboard;
                for (int i = 0; i < 17; i++)
                    keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.TAB);
                keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.SPACE);
                ExecutedSteps++;
                //Step 22: Authentication
                wpfobject.GetMainWindowByTitle("User Credential Form");
                wpfobject.WaitTillLoad();
                WpfObjects._mainWindow.Get(SearchCriteria.ByClassName("TextBox")).SetValue(Username);
                WpfObjects._mainWindow.Get(SearchCriteria.ByClassName("PasswordBox")).SetValue(Password);
                wpfobject.ClickRadioButton("Local", 1);
                wpfobject.ClickButton("OKButton");
                wpfobject.GetMainWindowByTitle("Ldap Role Management Form");
                wpfobject.WaitTillLoad();
                ExecutedSteps++;
                //Step 23: Click on the Base Roles tab and in the page that opens click on the Use Combination Roles button to populate the table with the datasource and Filter entries for each base role in the ldap.
                wpfobject.GetTabWpf(1).SelectTabPage(2);
                wpfobject.ClickButton("Use Combination Roles", 1);
                ExecutedSteps++;
                //Step 24: Click on the Create Base Roles to add these roles in the ICA server, make sure there is an ICA running before executing this function
                wpfobject.ClickButton("Create Base Roles", 1);
                ExecutedSteps++;
                //Step 25: Authentication
                //Since Step 22 stores credentials, after executing step 22, step 25 will be executed internally.
                ExecutedSteps++;
                //Step 26: Click on the Common Settings TAB and click on the Save Details to File
                wpfobject.GetTabWpf(1).SelectTabPage(0);
                if (File.Exists(Profile2updated))
                {
                    File.Delete(Profile2updated);
                }
                wpfobject.ClickButton("Save Details To File", 1);
                wpfobject.WaitTillLoad();
                wpfobject.SetText("File name:", Profile2updated, 1);
                wpfobject.ClickButton("Save", 1);
                wpfobject.WaitTillLoad();
                if (File.Exists(Profile2updated))
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
                //Step 27: Click on Done then Ok until you get to the SERVERS Tab page. 
                wpfobject.ClickRadioButton("Anyone", 1);
                for (int i = 0; i < 4; i++)
                {
                    keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.TAB);
                }
                keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.SPACE);
                wpfobject.GetMainWindowByTitle("LDAP Server Control Form");
                wpfobject.WaitTillLoad();
                IUIItem[] bindgrp = WpfObjects._mainWindow.GetMultiple(SearchCriteria.ByClassName("TextBox"));
                bindgrp[1].SetValue(Profile1);
                for (int i = 0; i < 8; i++)
                    keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.TAB);
                keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.SPACE);
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.ClickButton(ServiceTool.ApplyBtn_Name, 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                ExecutedSteps++;
                //Step 28: Open a browser and login to ICA as merge_admin/ .adm.13579
                login.LoginIConnect(LoginDetails[0].Split(':')[0], LoginDetails[0].Split(':')[1]);
                ExecutedSteps++;
                //Step 29: Go to the Role management Tab and select the Sacramento domain from the dropdown. Confirm the roles created from the service tool
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                rolemanagement.DomainSelector().SelectByValue(DomainName);
                DataTable Role = basepage.CollectRecordsInAllPages(rolemanagement.RoleTable(), Row: rolemanagement.RoleTableColumn());
                Actual = basepage.GetColumnValues(Role, "Role Name");
                if (Roles.All(rle => Actual.Contains(rle)))
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
                //Step 30: Go to the User management Tab and select from the domain dropdown "Sacramento"
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                usermanagement.DomainDropDown().SelectByText(DomainName);
                Actual = usermanagement.GroupList().Select(grp => grp.Text).ToArray();
                if (Groups.All(grp => Actual.Any(act => act.Contains(grp))))
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
                //Step 31: Select each group and observer the Users listed in the right side they should match the list in the Dignity Map Tab.
                resultcount = 0;
                usermanagement.SearchWithoutFilter(Config.adminGroupName, "Ungrouped");
                Expected = new string[] { DignityUsers[0] };
                Actual = usermanagement.UserDetailList().Select(usr => usr.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("The user " + DignityUsers[0] + " is present in the group Ungrouped");
                }
                usermanagement.SearchWithoutFilter(DomainName, Groups[0]);
                Expected = new string[] { DignityUsers[1], DignityUsers[2] };
                Actual = usermanagement.UserDetailList().Select(usr => usr.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("The user " + DignityUsers[1] + " and " + DignityUsers[2] + " is present in the group MercyGeneralHospital");
                }
                usermanagement.SearchWithoutFilter(DomainName, Groups[1]);
                Expected = new string[] { DignityUsers[3], DignityUsers[4] };
                Actual = usermanagement.UserDetailList().Select(usr => usr.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("The user " + DignityUsers[3] + " and " + DignityUsers[4] + " is present in the group MercyHospitalFolsom");
                }
                usermanagement.SearchWithoutFilter(DomainName, Groups[2]);
                Expected = new string[] { DignityUsers[5], DignityUsers[6] };
                Actual = usermanagement.UserDetailList().Select(usr => usr.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("The user " + DignityUsers[5] + " and " + DignityUsers[6] + " is present in the group MercySanJuanMedicalCenter");
                }
                usermanagement.SearchWithoutFilter(DomainName, Groups[3]);
                Expected = new string[] { DignityUsers[7], DignityUsers[8] };
                Actual = usermanagement.UserDetailList().Select(usr => usr.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("The user " + DignityUsers[7] + " and " + DignityUsers[8] + " is present in the group MethodistHospitalSacramento");
                }
                usermanagement.SearchWithoutFilter(DomainName, Groups[4]);
                Expected = new string[] { DignityUsers[9], DignityUsers[10] };
                Actual = usermanagement.UserDetailList().Select(usr => usr.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("The user " + DignityUsers[9] + " and " + DignityUsers[10] + " is present in the group SierraNevadaMemorialHospital");
                }
                usermanagement.SearchWithoutFilter(DomainName, Groups[5]);
                Expected = new string[] { DignityUsers[11], DignityUsers[12] };
                Actual = usermanagement.UserDetailList().Select(usr => usr.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("The user " + DignityUsers[11] + " and " + DignityUsers[12] + " is present in the group WoodlandHealthCare");
                }
                if (resultcount == 7)
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
                //Step 32: Logout and log back in as reuben ben Jacob
                login.Logout();
                login.LoginIConnect(LoginDetails[1].Split(':')[0], LoginDetails[1].Split(':')[1]);
                login.Logout();
                ExecutedSteps++;
                //Step 33: Login as each user and verify the Datasource and filter is applied correctly based on the role.
                string[] unames = new string[] { LoginDetails[2].Split(':')[0], LoginDetails[3].Split(':')[0] };
                string[] pwds = new string[] { LoginDetails[2].Split(':')[1], LoginDetails[3].Split(':')[1] };
                string[] ExpectedModalities = string.Concat(Filters[0].Split(':')[0], ":", Filters[1]).Split(':');
                string[] ExpectedDatasources = Datasources.Take(2).ToArray();
                string[] ExpectedIssuerofPID = string.Concat(Filters[0].Split(':')[1], ":", string.Empty).Split(':');
                resultcount = 0;
                for (int i = 0; i < unames.Length; i++)
                {
                    login.LoginIConnect(unames[i], pwds[i]);
                    login.Navigate("Studies");
                    basepage.SearchStudy(LastName: "*");
                    SearchResults = BasePage.GetSearchResults();
                    if (SearchResults.Count == 0)
                    {
                        resultcount++;
                        Logger.Instance.InfoLog("No Studies found for the user " + unames[i]);
                    }
                    else
                    {
                        basepage.ChooseColumns(new String[] { "Data Source", "Issuer of PID" });
                        string[] ActualModality = BasePage.GetColumnValues("Modality");
                        string[] ActualIssuerofPID = BasePage.GetColumnValues("Issuer of PID");
                        string[] ActualDataSource = BasePage.GetColumnValues("Data Source");
                        int searchcount = 0;
                        if (string.IsNullOrWhiteSpace(ExpectedModalities[i]))
                        {
                            searchcount++;
                            Logger.Instance.InfoLog("Filter is not available for Modality");
                        }
                        else if (ActualModality.All(md => ExpectedModalities[i].Contains(md)))
                        {
                            searchcount++;
                            Logger.Instance.InfoLog("Studies found for the user " + unames[i] + " with the modality");
                        }
                        if (string.IsNullOrWhiteSpace(ExpectedIssuerofPID[i]))
                        {
                            searchcount++;
                            Logger.Instance.InfoLog("Filter is not available for Issuer of PID");
                        }
                        else if (ActualIssuerofPID.All(ipid => ExpectedIssuerofPID[i].Contains(ipid)))
                        {
                            searchcount++;
                            Logger.Instance.InfoLog("Studies found for the user " + unames[i] + " with the Issuer of PID");
                        }
                        if (ActualDataSource.All(ds => ExpectedDatasources[i].Contains(ds)))
                        {
                            searchcount++;
                            Logger.Instance.InfoLog("Studies found for the user " + unames[i] + " with the Data Source");
                        }
                        if (searchcount == 3)
                        {
                            resultcount++;
                        }
                    }
                    login.Logout();
                }
                if (resultcount == unames.Length)
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
                //Step 34: Login as judah / .jbj.13579 and load study
                login.LoginIConnect(LoginDetails[4].Split(':')[0], LoginDetails[4].Split(':')[1]);
                PageLoadWait.WaitForPageLoad(20);
                login.Navigate("Studies");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                studies.SearchStudy("Accession", "1121603V");
                studies.SelectStudy("Accession", "1121603V");
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(10);
                if (viewer.SeriesViewer_1X1().Displayed)
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
                //Step 35: Go to the Domain management for the Sacramento Domain and confirm the switches set
                login.LoginIConnect(LoginDetails[0].Split(':')[0], LoginDetails[0].Split(':')[1]);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(10);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DomainName);
                domainmanagement.SelectDomain(DomainName);
                domainmanagement.ClickEditDomain();
                bool step35_1 = domainmanagement.VerifyCheckBoxInEditDomain("reportview");
                bool step35_2 = domainmanagement.VerifyCheckBoxInEditDomain("attachmentupload");
                bool step35_3 = domainmanagement.VerifyCheckBoxInEditDomain("datadownload");
                bool step35_4 = domainmanagement.VerifyCheckBoxInEditDomain("grantaccess");
                bool step35_5 = domainmanagement.VerifyCheckBoxInEditDomain("datatransfer");
                domainmanagement.ClickSaveEditDomain();
                login.Logout();
                if (step35_1 && step35_2 && step35_3 && step35_4 && step35_5)
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
                //Step 36: Open the service tool and click on the LDAP tab, then click on the Server TAB.
                servicetool.LaunchServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToTab(ServiceTool.LDAP_Tab);
                wpfobject.GetTabWpf(1).SelectTabPage(1);
                ExecutedSteps++;
                //Step 37: Click on Modify and select the Ldap server created in the previous steps. Click on the Detail button. In the Window that opens select the Data model Tab.
                wpfobject.ClickButton(ServiceTool.ModifyBtn_Name, 1);
                wpfobject.WaitTillLoad();
                ldap_grp1 = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(servicetool.GetCurrentTabItem(), ServiceTool.LDAP.Name.LdapServerListGrp, 1);
                datagrid1 = wpfobject.GetAnyUIItem<GroupBox, ListView>(ldap_grp1, ServiceTool.LDAP.ID.LdapServersList);
                datagrid1.Rows[0].Click();
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ServiceTool.LDAP.ID.DetailsBtn);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindowByTitle("LDAP Server Control Form");
                wpfobject.WaitTillLoad();
                ExecutedSteps++;
                //Step 38: Click on the General Rules  button
                wpfobject.SelectTabFromTabItems("Data Model");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Generate Rules", 1);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindowByTitle("Ldap Role Management Form");
                wpfobject.WaitTillLoad();
                ExecutedSteps++;
                //Step 39: Edit the key name by adding a new key4 click on Generate Key Selector Combinations
                IUIItem[] KeyBox = WpfObjects._mainWindow.GetMultiple(SearchCriteria.ByClassName("TextBlock"));
                KeyBox[8].DoubleClick();
                keyboard = WpfObjects._mainWindow.Keyboard;
                keyboard.Enter(Keys[3]);
                wpfobject.ClickButton("Generate Key Selector Combinations", 1);
                wpfobject.WaitTillLoad();

                IUIItem[] SStxbox = WpfObjects._mainWindow.GetMultiple(SearchCriteria.ByClassName("DataGrid"));
                ListView view1 = wpfobject.GetAnyUIItem<ITabPage, ListView>(servicetool.GetCurrentTabItem(), "DomainDataSourceIdGridView");
                string[] Grid1 = view1.Rows.Select(v1 => v1.Cells[0].Name).ToArray();
                ListView view2 = wpfobject.GetAnyUIItem<ITabPage, ListView>(servicetool.GetCurrentTabItem(), "");
                string[] Grid2 = view2.Rows.Select(v2 => v2.Cells[0].Name).ToArray();
                resultcount = 0;
                Expected = Keys.Take(4).ToArray();
                if (Expected.All(exp => Grid1.Contains(exp)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Combination roles - Selector key contains 4 keys");
                }
                Expected = Keys;
                if (Expected.All(exp => Grid2.Contains(exp)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Combination roles - Mapped key contains 15 keys");
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
                //Step 40: In the Imported Roles Table add the Key4 to each of the MGH_xxx roles in the Associated Selector Keys, save the new profile as Profile-Mod
                String[] RoleNames = new String[] { "MGH_Admins", "MGH_Radiologists", "MGH_Physicians" };

                IUIItem item;
                for (int i = 0; i < RoleNames.Length; i++)
                {
                    item = WpfObjects._mainWindow.GetMultiple(SearchCriteria.ByClassName("TextBlock")).Single(item1 =>
                    {
                        if (string.Equals(item1.Name, RoleNames[i]))
                            return true;
                        else
                            return false;
                    });
                    item.Click();
                    keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.TAB);
                    keyboard.Enter("Key4");
                }

                if (File.Exists(Profile1updated))
                {
                    File.Delete(Profile1updated);
                }
                wpfobject.ClickButton("Save File", 1);
                wpfobject.WaitTillLoad();
                wpfobject.SetText("File name:", Profile1updated, 1);
                wpfobject.ClickButton("Save", 1);
                wpfobject.WaitTillLoad();
                if (File.Exists(Profile1updated))
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

                String NodePath = @"/server/key.selectors/keys";
                String NodePath1 = @"/server/mapped.key.selectors/selector/keys";
                String NodePath2 = @"/server/mapped.key.selectors/selector/mapped.to";
                String NodePath3 = @"/server/base.roles/role/keys";
                //Step 41: Go to the location where the file was saved and confirm the changes by opening the file in a browser and observing the new Key4 entry.
                XmlDocument xmlDocument = new XmlDocument();
                // Load the XML file in to the document
                xmlDocument.Load(Profile1updated);
                //Get Parent Node
                XmlNodeList NodeList = xmlDocument.SelectNodes("/" + NodePath);
                //Change Value 
                int keycount = 0;
                string Short_Fall = string.Empty;
                foreach (XmlNode nod1e in NodeList)
                {
                    Short_Fall = nod1e.InnerText;
                    if (Short_Fall.Contains(Keys[3]))
                    {
                        keycount++;
                    }
                }
                NodeList = xmlDocument.SelectNodes("/" + NodePath1);
                foreach (XmlNode nod2e in NodeList)
                {
                    Short_Fall = nod2e.InnerText;
                    if (Short_Fall.Contains(Keys[3]))
                    {
                        keycount++;
                    }
                }

                NodeList = xmlDocument.SelectNodes("/" + NodePath2);
                foreach (XmlNode nod3e in NodeList)
                {
                    Short_Fall = nod3e.InnerText;
                    if (Short_Fall.Contains(Keys[3]))
                    {
                        keycount++;
                    }
                }

                NodeList = xmlDocument.SelectNodes("/" + NodePath3);
                foreach (XmlNode nod4e in NodeList)
                {
                    Short_Fall = nod4e.InnerText;
                    if (Short_Fall.Contains(Keys[3]))
                    {
                        keycount++;
                    }
                }

                // TODO for XML Assertion
                if (keycount == 20)
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
                //Step 42: Open and edit an existing Mapped Combination and base Roles Profiles 
                wpfobject.SelectTabFromTabItems("Create Roles");
                wpfobject.WaitTillLoad();
                ExecutedSteps++;
                //Step 43: Click on Load Details From File and select and load the Previously saved Profile XML file
                wpfobject.ClickButton("Load Details From File", 1);
                wpfobject.WaitTillLoad();
                wpfobject.SetText("File name:", Profile2, 1);
                wpfobject.ClickButton("Open", 1);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;
                //Step 44: Click on the Enable Emergency Access flag
                wpfobject.SelectCheckBox("Enable Emergency Access", 1);
                wpfobject.WaitTillLoad();
                wpfobject.GetTabWpf(1).SelectTabPage(1);
                wpfobject.WaitTillLoad();


                item = WpfObjects._mainWindow.GetMultiple(SearchCriteria.ByClassName("TextBlock")).Single(item1 =>
                {
                    if (string.Equals(item1.Name, "Key1_Role"))
                        return true;
                    else
                        return false;
                });
                item.Click();
                keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.TAB);
                keyboard.Enter(string.Concat(Datasources[0], ",", Datasources[1]));
                keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.TAB);
                keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.TAB);
                keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.TAB);
                keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.TAB);
                keyboard.Enter(RoleFilters.Replace("DTSS", Datasources[0]));
                ExecutedSteps++;
                //Step 45: Click on the Combine Roles button with an arrow pointing down.
                Button = WpfObjects._mainWindow.GetMultiple(SearchCriteria.ByClassName("Button"));
                Button[0].Click();
                ExecutedSteps++;
                //Step 46: Click on the"Create Selected Combination Roles Only"box and select the Roles that have the Key4 entry. Click on the Create Combination Roles
                wpfobject.SelectCheckBox("Create Selected Combination Roles Only", 1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Create Combination Roles", 1);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;
                //Step 47: Enter Authentication
                wpfobject.GetMainWindowByTitle("User Credential Form");
                wpfobject.WaitTillLoad();
                WpfObjects._mainWindow.Get(SearchCriteria.ByClassName("TextBox")).SetValue(Config.adminUserName);
                WpfObjects._mainWindow.Get(SearchCriteria.ByClassName("PasswordBox")).SetValue(Config.adminPassword);
                wpfobject.ClickRadioButton("Local", 1);
                wpfobject.ClickButton("OKButton");
                wpfobject.GetMainWindowByTitle("Ldap Role Management Form");
                wpfobject.WaitTillLoad();
                ExecutedSteps++;
                //Step 48: Click on the Base Roles tab and in the page that opens click on the Use Combination Roles button, this Updates the table with the datasource and Filter entries for each base role in the ldap.
                wpfobject.GetTabWpf(1).SelectTabPage(2);
                wpfobject.ClickButton("Use Combination Roles", 1);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;
                //Step 49: Click on the"Create Selected Base Roles Only"box and select the entries containing ECMPACS and Click on Create Base Roles.
                wpfobject.SelectCheckBox("Create Selected Base Roles Only", 1);
                wpfobject.WaitTillLoad();
                for (int i = 0; i < RoleNames.Length; i++)
                {
                    item = WpfObjects._mainWindow.GetMultiple(SearchCriteria.ByClassName("TextBlock")).Single(item1 =>
                    {
                        if (string.Equals(item1.Name, RoleNames[i]))
                            return true;
                        else
                            return false;
                    });
                    item.Click();
                    wpfobject.ClickButton("Create Base Roles", 1);
                    wpfobject.WaitTillLoad();
                }
                ExecutedSteps++;
                //Step 50: Enter Authentication
                //Since Step 47 Stores Credentials, Step 50 will be executed internally
                ExecutedSteps++;
                //Step 51: Click on the Common Settings TAB and click on the Save Details to File
                wpfobject.GetTabWpf(1).SelectTabPage(0);
                if (File.Exists(Profile2updated))
                {
                    File.Delete(Profile2updated);
                }
                wpfobject.ClickButton("Save Details To File", 1);
                wpfobject.WaitTillLoad();
                wpfobject.SetText("File name:", Profile2updated, 1);
                wpfobject.ClickButton("Save", 1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickRadioButton("Anyone", 1);
                for (int i = 0; i < 4; i++)
                {
                    keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.TAB);
                }
                keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.SPACE);
                wpfobject.GetMainWindowByTitle("LDAP Server Control Form");
                wpfobject.WaitTillLoad();
                bindgrp = WpfObjects._mainWindow.GetMultiple(SearchCriteria.ByClassName("TextBox"));
                bindgrp[1].SetValue(Profile1updated);
                //bindgrp[1].Click();
                //keyboard.Enter(Profile1updated);
                for (int i = 0; i < 8; i++)
                    keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.TAB);
                keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.SPACE);
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.ClickButton(ServiceTool.ApplyBtn_Name, 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                if (File.Exists(Profile2updated))
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
                //Step 52: Open the XML file (Profile2_update.XML) and confirm the changes were saved
                String CheckRoledetails = @"/role.creation.detail/base.combination.roles/role.detail[@name = 'Key1_Role']/data.source.pref/preference/value";
                String CheckRoledetails1 = @"/role.creation.detail/base.combination.roles/role.detail[@name = 'Key2_Role']/general.filters/preference/value";

                xmlDocument = new XmlDocument();
                // Load the XML file in to the document
                xmlDocument.Load(Profile2updated);
                //Get Parent Node
                XmlNode node2 = xmlDocument.SelectSingleNode("/" + CheckRoledetails);
                //Change Value 
                keycount = 0;
                String Short_Fall1 = string.Empty;
                Short_Fall1 = node2.InnerText;

                XmlNode node = xmlDocument.SelectSingleNode("/" + CheckRoledetails1);
                String Short_Fall2 = string.Empty;
                Short_Fall2 = node.InnerText;

                if (Short_Fall1.Contains(Datasources[0]) && Short_Fall1.Contains(Datasources[1]) && RoleFilters.Contains(Short_Fall2))
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
            finally
            {
                servicetool.CloseServiceTool();
                if (basepage.NodeExist(Config.DSAServerManagerConfiguration, "/server[@id='" + ServerName + "']"))
                {
                    basepage.RemoveNode(Config.DSAServerManagerConfiguration, "/server[@id='" + ServerName + "']");
                }
            }
        }

        /// <summary>
        /// LDAP- Users list from two LDAP servers
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_119795(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            DomainManagement domainmanagement = null;
            RoleManagement rolemanagement = null;
            UserManagement usermanagement = null;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            string[] ServerName = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "ServerName")).Split(':');
            string HostName = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "HostName"));
            string Port = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Port"));
            string DomainName = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "DomainNames"));
            string RoleName = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "RoleNames"));
            string[] LdapXMLPath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "LdapXMLPath")).Split('!');
            string[] Expected = null;
            string[] Actual = null;
            string[] NotExpected = null;
            int resultcount = 0;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                string localuid = Config.adminUserName;
                string localpwd = Config.adminPassword;
                string ldapserver1uid = Config.LdapAdminUserName;
                string ldapserver1pwd = Config.LdapAdminPassword;
                string ldapserver2uid = "trillium.admin";
                string ldapserver2pwd = Config.LdapAdminPassword;
                string localuser = string.Empty;
                string ldapserver1user = "admin1 (Domain1 Admin) Activated SiteAdmin";
                string ldapserver2user = "shw.admin (Sherway Admin) Activated SiteAdmin";
                //Pre Condition
                basepage.ChangeAttributeValue(Config.DSAServerManagerConfiguration, "/server", "enabled", "False");
                servicetool.RestartIISUsingexe();
                login.LoginIConnect(localuid, localpwd);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                if (domainmanagement.DomainExists(DomainName))
                {
                    usermanagement = (UserManagement)login.Navigate("UserManagement");
                    usermanagement.SearchWithoutFilter(DomainName);
                    localuser = usermanagement.UserDetailList().Select(user => user.Text).ToArray()[0];
                    rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                    if (!rolemanagement.RoleExists(RoleName, DomainName))
                    {
                        rolemanagement.CreateRole(DomainName, RoleName, roletype: "");
                    }
                }
                else
                {
                    var domainattr = domainmanagement.CreateDomainAttr();
                    domainattr[DomainManagement.DomainAttr.DomainName] = DomainName;
                    domainattr[DomainManagement.DomainAttr.RoleName] = RoleName;
                    localuser = string.Concat(domainattr[DomainManagement.DomainAttr.UserID], " (", domainattr[DomainManagement.DomainAttr.FirstName], " ", domainattr[DomainManagement.DomainAttr.LastName], ") Activated SiteAdmin");
                    domainmanagement.CreateDomain(domainattr);
                }
                login.Logout();
                //Step 1: From iCA server, Launch iCA service tool and add two different LDAP servers under LDAP tab
                for (int i = 0; i < 2; i++)
                {
                    if (basepage.NodeExist(Config.DSAServerManagerConfiguration, "/server[@id='" + ServerName[i] + "']"))
                    {
                        basepage.RemoveNode(Config.DSAServerManagerConfiguration, "/server[@id='" + ServerName[i] + "']");
                    }
                    basepage.InsertNode(Config.DSAServerManagerConfiguration, "/servers", File.ReadAllText(LdapXMLPath[i]), false);
                    basepage.ChangeAttributeValue(Config.DSAServerManagerConfiguration, "/server[@id='" + ServerName[(i + 2)] + "']", "id", ServerName[i]);
                    basepage.ChangeAttributeValue(Config.DSAServerManagerConfiguration, "/server[@id='" + ServerName[i] + "']", "enabled", "True");
                    basepage.InsertNode(Config.DSAServerManagerConfiguration, "/server[@id='" + ServerName[i] + "']/options/hosts", string.Concat("<host name=\"", HostName, "\" port=\"", Port, "\" />"));
                }
                servicetool.RestartIISUsingexe();
                ExecutedSteps++;
                //Step 2: Navigate to User management database tab In iCA service tool and ensure that both Local database and LDAP directory service are selected
                //Step 3: Restart the IIS and window services
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolUserMgmtDatabaseTab();
                servicetool.SetMode(2);
                servicetool.CloseServiceTool();
                ExecutedSteps++;
                ExecutedSteps++;
                //Step 4: Login to iCA as Administrator then navigate to User management tab
                login.LoginIConnect(localuid, localpwd);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                ExecutedSteps++;
                //Step 5: Click on search button * in the search field and ensure that all the users from both the LDAP servers and Local database are displayed
                usermanagement.SearchWithoutFilter(DomainName);
                Expected = new string[] { localuser, ldapserver1user, ldapserver2user };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
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
                //Step 6: Logout as administrator and login again to iCA as the administrator of the first LDAP server
                login.Logout();
                login.LoginIConnect(ldapserver1uid, ldapserver1pwd);
                ExecutedSteps++;
                //Step 7: Navigate to user management database tab then click on search button * in the search field and ensure that all the users from both the LDAP servers and Local database are displayed
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.SearchWithoutFilter(DomainName);
                Expected = new string[] { localuser, ldapserver1user, ldapserver2user };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
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
                //Step 8: Logout from iCA and login again to iCA with Administrator of second LDAP server
                login.Logout();
                login.LoginIConnect(ldapserver2uid, ldapserver2pwd);
                ExecutedSteps++;
                //Step 9: Navigate to user management tab then click on search button * in the search field and ensure that all the users from both the LDAP servers and Local database are displayed
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.SearchWithoutFilter(DomainName);
                Expected = new string[] { localuser, ldapserver1user, ldapserver2user };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                if (Expected.All(expect => Actual.Contains(expect)))
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
                //Step 10: From iCA server launch iCA service tool and navigate to user management database tab and deselect LDAP directory service
                login.Logout();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolUserMgmtDatabaseTab();
                servicetool.SetMode(0);
                servicetool.CloseServiceTool();
                ExecutedSteps++;
                //Step 11: Login to iCA as Administrator then navigate to User management tab
                login.LoginIConnect(localuid, localpwd);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                ExecutedSteps++;
                //Step 12: Click on search button * in the search field and ensure that all the users from Local base are displayed
                usermanagement.SearchWithoutFilter(DomainName);
                Expected = new string[] { localuser };
                NotExpected = new string[] { ldapserver1user, ldapserver2user };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                resultcount = 0;
                if (Expected.All(expect => Actual.Contains(expect)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("System displays local user name in user management page");
                }
                if (!NotExpected.Any(ne => Actual.Contains(ne)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("System does not displays Ldap user name in user management page");
                }
                if (resultcount == 2)
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
                //Step 13: From iCA server launch iCA service tool and navigate to user management database tab and deselect Local database and select LDAP directory service
                login.Logout();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolUserMgmtDatabaseTab();
                servicetool.SetMode(1);
                servicetool.CloseServiceTool();
                ExecutedSteps++;
                //Step 14: Login to iCA as Administrator of the first LDAP server and navigate to user management tab
                login.LoginIConnect(ldapserver1uid, ldapserver1pwd);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                ExecutedSteps++;
                //Step 15: Click on search button * in the search field and ensure that all the users from both the LDAP servers are displayed
                usermanagement.SearchWithoutFilter(DomainName);
                Expected = new string[] { ldapserver1user, ldapserver2user };
                NotExpected = new string[] { localuser };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                resultcount = 0;
                if (Expected.All(expect => Actual.Contains(expect)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("System displays Ldap user name in user management page");
                }
                if (!NotExpected.Any(ne => Actual.Contains(ne)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("System does not displays Local user name in user management page");
                }
                if (resultcount == 2)
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
                //Step 16: Logout from iCA and Login again to iCA as Administrator of the second LDAP server and navigate to user management tab
                login.Logout();
                login.LoginIConnect(ldapserver2uid, ldapserver2pwd);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                ExecutedSteps++;
                //Step 17: Click on search button * in the search field and ensure that all the users from both the LDAP servers are displayed
                usermanagement.SearchWithoutFilter(DomainName);
                Expected = new string[] { ldapserver1user, ldapserver2user };
                NotExpected = new string[] { localuser };
                Actual = usermanagement.UserDetailList().Select(user => user.Text).ToArray();
                resultcount = 0;
                if (Expected.All(expect => Actual.Contains(expect)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("System displays Ldap user name in user management page");
                }
                if (!NotExpected.Any(ne => Actual.Contains(ne)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("System does not displays Local user name in user management page");
                }
                if (resultcount == 2)
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
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

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
            finally
            {
                servicetool.CloseServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolUserMgmtDatabaseTab();
                servicetool.SetMode(2);
                servicetool.SetEnableFeaturesGeneral();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableSelfEnrollment();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                wpfobject.ClickOkPopUp();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                for (int i = 0; i < 2; i++)
                {
                    if (basepage.NodeExist(Config.DSAServerManagerConfiguration, "/server[@id='" + ServerName[i] + "']"))
                    {
                        basepage.RemoveNode(Config.DSAServerManagerConfiguration, "/server[@id='" + ServerName[i] + "']");
                    }
                }
                servicetool.RestartIISUsingexe();
            }
        }

        /// <summary>
        /// LDAP - Do Not List Users with Unmapped Domains
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_119829(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            Login login = new Login();
            DomainManagement domainmanagement = null;
            RoleManagement rolemanagement = null;
            UserManagement usermanagement = null;
            ServiceTool servicetool = new ServiceTool();
            WpfObjects wpfobject = new WpfObjects();
            String DomainName = "Domain1";
            string RoleName = "AdminRole";
            String Institution = "Inst" + new Random().Next(1000);
            int Allusercount = 0;
            int mappedusercount = 0;
            String Rolename = "role" + new Random().Next(1000);
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                //Fetch required Test data                       
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;

                //Precondition:
                basepage.ChangeAttributeValue(Config.DSAServerManagerConfiguration, "/server[@id='ica.ldap.merge.ad']", "enabled", "True");
                servicetool.RestartIISUsingexe();
                // Step 1:
                // Set up an LDAP server with users in domains, groups and roles that do not exist in ICA
                login.LoginIConnect(Username, Password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForPageLoad(10);
                Dictionary<Object, String> createDomain = domainmanagement.CreateDomainAttr();
                if (domainmanagement.IsDomainExist(DomainName))
                {
                    rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                    if (!rolemanagement.RoleExists(RoleName, DomainName))
                    {
                        rolemanagement.CreateRole(DomainName, RoleName, roletype: "");
                    }
                }
                else
                {
                    createDomain[DomainManagement.DomainAttr.DomainName] = DomainName;
                    createDomain[DomainManagement.DomainAttr.RoleName] = RoleName;
                    domainmanagement.CreateDomain(createDomain);
                }
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(10);
                usermanagement.SearchWithoutFilter(DomainName);
                Allusercount = usermanagement.UserDetailList().Count;
                login.Logout();
                ExecutedSteps++;

                // Step 2:
                // 	Launch ICA service tool, navigate to LDAP tab -> Global options sub tab and set the Do Not List Users with Unmapped Domains, Groups and Roles controls to true.
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToTab(ServiceTool.LDAP_Tab);
                wpfobject.WaitTillLoad();
                servicetool.ClickModifyFromTab();
                wpfobject.WaitTillLoad();
                if (!wpfobject.IsCheckBoxSelected("WithUnmappedDomains"))
                {
                    wpfobject.SelectCheckBox("WithUnmappedDomains");
                }
                if (!wpfobject.IsCheckBoxSelected("WithUnmappedRoles"))
                {
                    wpfobject.SelectCheckBox("WithUnmappedRoles");
                }
                if (!wpfobject.IsCheckBoxSelected("WithUnmappedGroups"))
                {
                    wpfobject.SelectCheckBox("WithUnmappedGroups");
                }
                servicetool.ClickApplyButtonFromTab();
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                // Step 3
                // Login to ICA as Administrator and navigate to user management page and Verify that LDAP users are not displayed in the user management page who are not members of domains, groups and roles in iCA. For.eg. The user belongs to SuperAdmin domain should be listed, The newly created LDAP users shoud not be listed. 
                login.LoginIConnect(Username, Password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForPageLoad(10);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(10);
                usermanagement.SelectDomainFromDropdownList(DomainName);
                usermanagement.SearchUser(DomainName);
                mappedusercount = usermanagement.UserDetailList().Count;
                if (mappedusercount < Allusercount)
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

                // Step 4
                // Search for users who are members of the same domains, groups and roles that do exist. 
                if (mappedusercount < Allusercount)
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
            finally
            {
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToTab(ServiceTool.LDAP_Tab);
                wpfobject.WaitTillLoad();
                servicetool.ClickModifyFromTab();
                wpfobject.WaitTillLoad();
                if (wpfobject.IsCheckBoxSelected("WithUnmappedDomains"))
                {
                    wpfobject.UnSelectCheckBox("WithUnmappedDomains");
                }
                if (wpfobject.IsCheckBoxSelected("WithUnmappedRoles"))
                {
                    wpfobject.UnSelectCheckBox("WithUnmappedRoles");
                }
                if (wpfobject.IsCheckBoxSelected("WithUnmappedGroups"))
                {
                    wpfobject.UnSelectCheckBox("WithUnmappedGroups");
                }
                servicetool.ClickApplyButtonFromTab();
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
            }
        }

        /// <summary>
        /// Test 160949 - Registered user emails a study to Non Registered user from Inbounds / Outbounds tab
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_160949(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            Login login = new Login();
            DomainManagement domainmanagement = null;
            RoleManagement rolemanagement = null;
            Studies studies = null;
            Outbounds outbounds = null;
            UserManagement userManagement = null;
            ServiceTool servicetool = new ServiceTool();
            UserPreferences userpreferences = new UserPreferences();
            BluRingViewer bluringviewer = new BluRingViewer();
            BasePage basepage = new BasePage();
            string DomainName = string.Empty;
            string RoleName = string.Empty;
            string Group = string.Empty;
            string ServerName = string.Empty;
            string LdapUserName = string.Empty;
            string LdapPassword = string.Empty;
            string FirstName = string.Empty;
            string Accession = string.Empty;
            string pinnumber = string.Empty;
            string User = BasePage.GetUniqueUserId();
            Dictionary<Object, String> createDomain = null;
            EmailUtils CustomUser1 = new EmailUtils() { EmailId = Config.CustomUser1Email, Password = Config.CustomUserEmailPassword };
            string link = String.Empty;
            Dictionary<string, string> downloadedMail = new Dictionary<string, string>();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Accession");
                ServerName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ServerName");
                DomainName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainNames");
                RoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleNames");
                Group = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Groups");
                FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                LdapUserName = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "LoginDetails")).Split('=')[0];
                LdapPassword = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "LoginDetails")).Split('=')[1];
                //PreCondition
                servicetool.LaunchServiceTool();
                servicetool.SetEmailNotification(AdministratorEmail: Config.AdminEmail, SystemEmail: Config.SystemEmail, SMTPHost: Config.SMTPServer);
                servicetool.LDAPTenetFinaldmSetup();
                servicetool.CloseServiceTool();
                basepage.ChangeAttributeValue(Config.DSAServerManagerConfiguration, "/server[@id='" + ServerName + "']", "enabled", "True");
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolUserMgmtDatabaseTab();
                //Enable Local and LDAP Database 
                servicetool.SetMode(2);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                createDomain = domainmanagement.CreateDomainAttr();
                createDomain[DomainManagement.DomainAttr.DomainName] = DomainName;
                if (!domainmanagement.SearchDomain(DomainName))
                {
                    domainmanagement.CreateDomain(createDomain);
                }
                domainmanagement.SearchDomain(DomainName);
                domainmanagement.SelectDomain(DomainName);
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                domainmanagement.SetCheckBoxInEditDomain("grant", 0);
                domainmanagement.SetCheckBoxInEditDomain("emailstudy", 0);
                domainmanagement.SetCheckBoxInEditDomain("universalviewer", 0);
                domainmanagement.ClickSaveEditDomain();
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                if (!rolemanagement.RoleExists(RoleName, DomainName))
                {
                    rolemanagement.CreateRole(DomainName, RoleName, roletype: "");
                }
                rolemanagement.SelectDomainfromDropDown(DomainName);
                rolemanagement.SelectRole(RoleName);
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("email", 0);
                rolemanagement.SetCheckboxInEditRole("universalviewer", 0);
                if (!rolemanagement.GrantAccessRadioBtn_Anyone().Selected)
                {
                    rolemanagement.GrantAccessRadioBtn_Anyone().Click();
                }
                rolemanagement.ClickSaveEditRole();
                userManagement = (UserManagement)login.Navigate("UserManagement");
                if (!userManagement.SearchGroup(Group, DomainName, 0))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    userManagement.CreateGroup(DomainName, Group, selectalldatasources: 0, selectallroles: 1);
                }
                userManagement.CreateUser(User, DomainName, RoleName);
                login.Logout();

                //Step 1:
                login.LoginIConnect(LdapUserName, LdapPassword);
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.BluringViewerRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession, FirstName: FirstName, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession);
                studies.ShareStudy(false, new String[] { User }, domainName: DomainName);
                outbounds = (Outbounds)login.Navigate("Outbounds");
                if (login.IsTabSelected("Outbounds"))
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

                //Step 2:
                PageLoadWait.WaitForFrameLoad(60);
                outbounds.SearchStudy(AccessionNo: Accession);
                if (outbounds.CheckStudy("Accession", Accession))
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

                //Step 3:
                outbounds.SelectStudy("Accession", Accession);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                if (bluringviewer.studyPanel().Displayed)
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

                //Step 4:
                //Step 5:
                CustomUser1.MarkAllMailAsRead();
                pinnumber = bluringviewer.EmailStudy_BR(emailaddr: Config.CustomUser1Email, DeleteEmail: false);
                if (pinnumber != null && (!String.IsNullOrWhiteSpace(pinnumber)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 6:
                downloadedMail = CustomUser1.GetMailUsingIMAP(Config.SystemEmail, "Emailed Study", MarkAsRead: true, maxWaitTime: 3);
                link = CustomUser1.GetEmailedStudyLink(downloadedMail);
                if (!string.IsNullOrWhiteSpace(link))
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

                //Step 7:
                bluringviewer = LaunchEmailedStudy.LaunchStudy<BluRingViewer>(link, pinnumber);
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitForPriorsToLoad();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer()))
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
            finally
            {
                servicetool.CloseServiceTool();
                login.Logout();
            }
        }

        /// <summary>
        /// Test 160949 - Registered user emails a study to Non Registered user from Inbounds / Outbounds tab
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161355(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            Login login = new Login();
            Studies studies = null;
            Outbounds outbounds = null;
            Inbounds inbounds = null;
            Image_Sharing imagesharing = null;
            Image_Sharing.Destination destination = null;
            ServiceTool servicetool = new ServiceTool();
            UserPreferences userpreferences = new UserPreferences();
            BluRingViewer bluringviewer = new BluRingViewer();
            RanorexObjects m_RanorexObjects = new RanorexObjects();
            Web_Uploader webuploader = new Web_Uploader();
            BasePage basepage = new BasePage();
            string UploadFilePath = string.Empty;
            string RoleName = string.Empty;
            string Group = string.Empty;
            string ServerName = string.Empty;
            string Accession = string.Empty;
            string LastName = string.Empty;
            string FirstName = string.Empty;
            string DOB = string.Empty;
            string PatientID = string.Empty;
            string User = BasePage.GetUniqueUserId();
            string Dest = "LDAP";
            EmailUtils CustomUser1 = new EmailUtils() { EmailId = Config.CustomUser1Email, Password = Config.CustomUserEmailPassword };
            string link = String.Empty;
            Dictionary<string, string> downloadedMail = new Dictionary<string, string>();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            string Browser = Config.BrowserType;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            string[] LoginDetails = null;
            try
            {
                Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Accession");
                LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                DOB = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "DOB")).Split('=')[0];
                ServerName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ServerName");
                UploadFilePath = Config.TestDataPath + (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                RoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleNames");
                LoginDetails = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "LoginDetails")).Split('=');
                //PreCondition
                
                /*basepage.ChangeAttributeValue(Config.DSAServerManagerConfiguration, "/server[@id='" + ServerName + "']", "enabled", "True");
                servicetool.LaunchServiceTool();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();*/
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                destination = (Image_Sharing.Destination)imagesharing.NavigateToSubTab("Destination");
                destination.SelectDomain(Config.adminGroupName);
                if (!(destination.SearchDestination(Config.adminGroupName, Dest)))
                {
                    destination.CreateDestination(login.GetHostName(Config.DestEAsIp), LoginDetails[2], LoginDetails[4], destinationname: Dest, domain: Config.adminGroupName);
                }
                login.Logout();
                //Step 1:
                BasePage.Driver.Quit();
                Config.BrowserType = "ie";
                Logger.Instance.InfoLog("Swicthing Browser Type to Internet Explorer");
                BasePage.Driver = null;
                login = new Login();
                login.DriverGoTo(login.url);
                login.LoginIConnect(LoginDetails[0], LoginDetails[1]);
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.DefaultUploaderList().SelectByText("Java Uploader");
                userpreferences.CloseUserPreferences();
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 2 to 8:
                outbounds = (Outbounds)login.Navigate("Outbounds");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                webuploader.UploadButton().Click();
                FileUtils.AddExceptionSiteForJavaSecurity("http://" + Config.IConnectIP);
                try
                {
                    //webuploader.AcceptSecurityWarning();
                    webuploader.RunJavaApplication(50);
                }
                catch (Exception) { }
                int counterX = 0;
                do
                {
                    try
                    {
                        m_RanorexObjects.WaitForElementTobeVisible(webuploader.GetControlIdForWebUploader("ToDestination"), 100);
                        break;
                    }
                    catch (Exception)
                    {
                        counterX++;
                        Thread.Sleep(5000);
                    }
                }
                while (counterX < 10);

                webuploader.SelectDestination(Dest);
                webuploader.SelectFileFromHdd(UploadFilePath);
                webuploader.SelectAllSeriesToUpload();
                webuploader.Send();
                Thread.Sleep(60000);
                webuploader.CloseUploader();
                PageLoadWait.WaitForPageLoad(20);
                login.Logout();
                BasePage.Driver.Quit();
                Config.BrowserType = Browser;
                Logger.Instance.InfoLog("Swicthing back to original browser");
                BasePage.Driver = null;
                login = new Login();
                login.DriverGoTo(login.url);
                for (int i = 2; i <= 8; i++)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                }

                //Step 9:
                login.LoginIConnect(LoginDetails[2], LoginDetails[3]);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("Accession", Accession);
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                inbounds.ChooseColumns(new String[] { "Accession" });
                if (inbounds.CheckStudy("Accession", Accession))
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

                //Step 10:
                inbounds.SelectStudy("Accession", Accession);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                if(bluringviewer.studyPanel().Displayed)
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

                //Step 11:
                bluringviewer.CloseBluRingViewer();
                inbounds.SearchStudy("Accession", Accession);
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                inbounds.ChooseColumns(new String[] { "Accession" });
                inbounds.SelectStudy("Accession", Accession);
                IWebElement OrderField, ReasonField;
                inbounds.ClickNominateButton(out ReasonField, out OrderField);
                if (inbounds.NominateDiv().Displayed && OrderField.Displayed && ReasonField.Displayed)
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
                OrderField.SendKeys("TestOrder");
                inbounds.ClickConfirmNominate();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                inbounds.SearchStudy("Accession", Accession);
                PageLoadWait.WaitForLoadingMessage();
                PageLoadWait.WaitForSearchLoad();
                if (inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Nominated For Archive" }) != null)
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

                //Step 13:
                login.Logout();
                login.LoginIConnect(LoginDetails[4], LoginDetails[5]);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                //Search and Select Study
                inbounds.SearchStudy("Accession", Accession);
                PageLoadWait.WaitForLoadingMessage();
                PageLoadWait.WaitForSearchLoad();
                if (inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Nominated For Archive" }) != null)
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

                //Step-14:
                inbounds.SelectStudy("Accession", Accession);
                //Details of a study
                Dictionary<string, string> rowValues = inbounds.GetMatchingRow("Accession", Accession);
                IWebElement UploadCommentsField18, ArchiveOrderField18;
                inbounds.ClickArchiveStudy(out UploadCommentsField18, out ArchiveOrderField18);
                inbounds.ArchiveSearch("order", "All Dates");
                try
                {
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Inbounds.AlertDiv)));
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Inbounds.CloseAlert)));
                    BasePage.Driver.FindElement(By.CssSelector(Inbounds.CloseAlert)).Click();
                }
                catch (Exception e)
                { Logger.Instance.InfoLog("Multiple patients dialog not found.."); }
                PageLoadWait.WaitForFrameLoad(20);
                //Details in Original details column
                Dictionary<String, String> OriginalDetails = inbounds.GetDataInArchive("Original Details");
                //Details in Final details column 
                Dictionary<String, String> FinalDetails = inbounds.GetDataInArchive("Final Details");
                //Validate the details in original details column are in sync with study details 
                if ((OriginalDetails["Last Name"].Equals(FinalDetails["Last Name"])) && (OriginalDetails["First Name"].Equals(FinalDetails["First Name"])) &&
                    (OriginalDetails["Gender"].Equals(FinalDetails["Gender"])) && (OriginalDetails["DOB"].Equals(FinalDetails["DOB"])) &&
                    (OriginalDetails["Issuer of PID"].Equals(FinalDetails["Issuer of PID"])) && (OriginalDetails["PID / MRN"].Equals(FinalDetails["PID / MRN"])) &&
                    (OriginalDetails["Description"].Equals(FinalDetails["Description"])) && (OriginalDetails["Study Date"].Equals(FinalDetails["Study Date"])) &&
                    (OriginalDetails["Accession"].Equals(FinalDetails["Accession"])))
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


                //Step-15:
                basepage.ClickElement(BasePage.Driver.FindElement(basepage.By_ReconcileSearchOrderRadio));
                inbounds.ArchiveSearch("order", "All Dates");
                try
                {
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Inbounds.AlertDiv)));
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Inbounds.CloseAlert)));
                    BasePage.Driver.FindElement(By.CssSelector(Inbounds.CloseAlert)).Click();
                }
                catch (Exception e)
                { Logger.Instance.InfoLog("Multiple patients dialog not found.."); }
                PageLoadWait.WaitForFrameLoad(20);
                //Details in Matching Order column
                Dictionary<String, String> OrderDetails = inbounds.GetDataInArchive("Matching Order");
                if ((OrderDetails["Last Name"].Equals("")) && (OrderDetails["First Name"].Equals("")) &&
                    (OrderDetails["Gender"].Equals("")) && (OrderDetails["DOB"].Equals("")) &&
                    (OrderDetails["Issuer of PID"].Equals("")) && (OrderDetails["PID / MRN"].Equals("")) &&
                    (OrderDetails["Description"].Equals("")) && (OrderDetails["Study Date"].Equals("")) &&
                    (OrderDetails["Accession"].Equals("")))
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



                //Step-16:
                inbounds.ClickArchive();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                inbounds.SearchStudy("Accession", Accession);
                PageLoadWait.WaitForLoadingMessage();
                PageLoadWait.WaitForSearchLoad();
                if (inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Routing Completed" }) != null)
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


                //Step-17:
                login.Logout();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession, Datasource: login.GetHostName(Config.DestEAsIp));
                studies.SelectStudy("Accession", Accession);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                //Last Name
                bool Step17_1 = string.Equals(basepage.GetText("cssselector", BluRingViewer.p_PatientName).Replace(" ", String.Empty).Split(',')[0], LastName);
                //FirstName
                bool Step17_2 = string.Equals(basepage.GetText("cssselector", BluRingViewer.p_PatientName).Replace(" ", String.Empty).Split(',')[1], FirstName);
                //Patient ID
                bool Step17_3 = string.Equals(basepage.GetText("cssselector", BluRingViewer.div_PatientID), PatientID);
                //DOB
                bool Step17_4 = string.Equals(basepage.GetText("cssselector", BluRingViewer.span_PatientDOB), DOB);
                //Accession
                bool Step17_5 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.AccessionNumberInExamList))[0].Text.Contains(Accession);

                if (Step17_1 && Step17_2 && Step17_3 && Step17_4 && Step17_5)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step17_1 = " + Step17_1);
                    Logger.Instance.InfoLog("Step17_2 = " + Step17_2);
                    Logger.Instance.InfoLog("Step17_3 = " + Step17_3);
                    Logger.Instance.InfoLog("Step17_4 = " + Step17_4);
                    Logger.Instance.InfoLog("Step17_5 = " + Step17_5);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
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
            finally
            {
                servicetool.CloseServiceTool();
                login.Logout();
                try
                {
                    BasePage.Driver.Quit();
                    BasePage.Driver = null;
                    Config.BrowserType = "chrome";
                    login = new Login();
                    login.DriverGoTo(login.url);
                    login.LoginIConnect(LoginDetails[0], LoginDetails[1]);
                    outbounds = (Outbounds)login.Navigate("Outbounds");
                    outbounds.SearchStudy(AccessionNo: Accession);
                    PageLoadWait.WaitForLoadingMessage(60);
                    PageLoadWait.WaitForSearchLoad();
                    inbounds.ChooseColumns(new String[] { "Accession" });
                    if (inbounds.CheckStudy("Accession", Accession))
                    {
                        inbounds.SelectStudy("Accession", Accession);
                        outbounds.DeleteStudy();
                    }
                    login.Logout();
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                }
                finally
                {
                    BasePage.Driver.Quit();
                    BasePage.Driver = null;
                    Config.BrowserType = Browser;
                    login = new Login();
                    login.DriverGoTo(login.url);
                }
            }
        }
    }
}
