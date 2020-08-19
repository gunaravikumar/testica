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
using TestStack.White.InputDevices;
using System.Xml;

namespace Selenium.Scripts.Tests
{
    class DignityLdap
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

        public DignityLdap(String classname)
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
        /// Initialize
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>

        public TestCaseResult Test_161346(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            string ServerName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ServerName");
            string HostName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "HostName");
            string Port = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Port"));
            string[] Groups = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Groups")).Split(':');
            string DomainName = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName"));
            string[] Rows = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Rows")).Split(':');
            string Profile1 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Profile1");
            string Profile2 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Profile2");
            string[] AssociatedKeys = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "AssociatedKeys")).Split(':');
            string[] UserType = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UserType")).Split(':');
            string[] LoginDetails = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "LoginDetails")).Split('!');
            string[] RoleFilters = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "RoleFilters")).Split('!');
            string[] Filters = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Filters")).Split('!');
            string[] Roles = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Roles")).Split(':');
            string[] Keys = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Keys")).Split(':');
            string[] Datasources = new string[] { new Login().GetHostName(Config.EA96), new Login().GetHostName(Config.PACS2), new Login().GetHostName(Config.SanityPACS) };
            string[] DignityUsers = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "DignityUsers")).Split(':');

            DomainManagement domainmanagement = null;
            UserManagement usermanagement = null;
            RoleManagement rolemanagement = null;
            Dictionary<int, string[]> SearchResults = null;
            Studies studies = null;
            int resultcount = 0;
            string[] Expected = null;
            string[] Actual = null;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                // Step 1: Preconditions
                ExecutedSteps++;
                //Step 2: Login to ica and create a Domain "Sacramento"
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
                //Step 3: Go to the ICA service tool and select the LDAP tab Click on modify and select the enable Ldap box
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolUserMgmtDatabaseTab();
                //Enable Local and LDAP Database 
                servicetool.SetMode(2);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                string LdapXML = File.ReadAllText(Config.TestDataPath + (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LdapXMLPath"));
                basepage.ChangeAttributeValue(Config.DSAServerManagerConfiguration, "/server", "enabled", "False");
                if (basepage.NodeExist(Config.DSAServerManagerConfiguration, "/server[@id='" + ServerName + "']"))
                {
                    basepage.RemoveNode(Config.DSAServerManagerConfiguration, "/server[@id='" + ServerName + "']");
                }
                basepage.InsertNode(Config.DSAServerManagerConfiguration, "/servers", LdapXML, false);
                basepage.InsertNode(Config.DSAServerManagerConfiguration, "/server[@id='" + ServerName + "']/options/hosts", string.Concat("<host name=\"", HostName, "\" port=\"", Port, "\" />"));
                servicetool.RestartIISUsingexe();
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
                //Step 4:  confirm the following settings
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
                //Step 5: Select the Host name and Click on the Test Connection button
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
                //Step 6: Select Data Model Tab 
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
                //Step 7: Go to the user account overrides TAB
                wpfobject.SelectTabFromTabItems("User Account Overrides");
                string[] user = wpfobject.GetAnyUIItem<ITabPage, ListView>(servicetool.GetCurrentTabItem(), "UserAccountOverridesGrid").Items.ToArray();
                if (user.Contains("merge_admin", StringComparer.OrdinalIgnoreCase))
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

                //Step 8: Go back to the Data Model TAB and select the Enable Role Management Rules box.
                wpfobject.SelectTabFromTabItems("Data Model");
                wpfobject.WaitTillLoad();
                CheckBox EnableRoleManagement = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(servicetool.GetCurrentTabItem(), "EnableRoleManagementRules");
                if (!EnableRoleManagement.Checked)
                {
                    EnableRoleManagement.Checked = true;
                    ExecutedSteps++;
                }
                //Step 9: Click on the General Rules button
                wpfobject.ClickButton("Generate Rules", 1);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindowByTitle("Ldap Role Management Form");
                wpfobject.WaitTillLoad();
                ExecutedSteps++;
                //Step 10: three Key names in the Key Name filed. These will be used to create combination roles
                IUIItem[] KeyBox = WpfObjects._mainWindow.GetMultiple(SearchCriteria.ByClassName("DataGridCell"));
                KeyBox[0].DoubleClick();
                TestStack.White.InputDevices.AttachedKeyboard keyboard = WpfObjects._mainWindow.Keyboard;
                keyboard.Enter(Keys[0]);
                keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.RETURN);
                keyboard.Enter(Keys[1]);
                keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.RETURN);
                keyboard.Enter(Keys[2]);
                keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.RETURN);
                wpfobject.ClickButton("Generate Key Selector Combinations", 1);
                wpfobject.WaitTillLoad();
                IUIItem[] SStxbox = WpfObjects._mainWindow.GetMultiple(SearchCriteria.ByClassName("DataGrid"));
                string[] Grid1 = (((ListView)SStxbox[0]).Items).Select(g1 => g1.ToString()).ToArray();
                string[] Grid2 = (((ListView)SStxbox[1]).Items).Select(g1 => g1.ToString()).ToArray();
                resultcount = 0;
                Expected = Keys.Take(3).ToArray();
                if (Expected.All(exp => Grid1.Contains(exp)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Combination roles - Selector key contains 3 keys");
                }
                Expected = Keys;
                if (Expected.All(exp => Grid2.Contains(exp)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Combination roles - Mapped key contains 7 keys");
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
                //Step 11: Import Roles From Server 
                wpfobject.ClickButton("Import", 1);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;
                //Step 12: Role Common Name entries In the Import Roles table 
                IUIItem item = WpfObjects._mainWindow.GetMultiple(SearchCriteria.ByClassName("TextBlock")).Single(item1 =>
                {
                    if (string.Equals(item1.Name, "MGH_Admins"))
                        return true;
                    else
                        return false;
                });
                item.Click();
                for (int i = 0; i < 18; i++)
                {

                    keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.TAB);
                    keyboard.Enter(AssociatedKeys[i]);
                    keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.TAB);
                    keyboard.Enter(UserType[i]);
                    keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.TAB);
                }
                ExecutedSteps++;
                //Step 13:  location and name of file
                if (File.Exists(Profile1))
                {
                    File.Delete(Profile1);
                }
                wpfobject.ClickButton("Save File", 1);
                wpfobject.WaitTillLoad();
                wpfobject.SetText("File name:", Profile1, 1);
                wpfobject.ClickButton("Save", 1);
                wpfobject.WaitTillLoad();
                if (File.Exists(Profile1))
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
                //Step 14: Select by clicking on the"Create Role" Tab
                wpfobject.SelectTabFromTabItems("Create Roles");
                wpfobject.WaitTillLoad();
                ExecutedSteps++;
                //Step 15: Click on Get Details
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
                    Logger.Instance.InfoLog("Expected Datasource = " + string.Join(",", Expected.ToArray()));
                    Logger.Instance.InfoLog("Actual Datasource = " + string.Join(",", Actual.ToArray()));
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 16: Click on Tab Mapped Combination Roles
                wpfobject.GetTabWpf(1).SelectTabPage(1);
                ExecutedSteps++;
                //Step 17: In the first table Base Combination Roles
                //Step 18: In the Role Filter column double click to bring up the Role filter detail selection window. Select a filter category, currently only one is available. Double click on the Filter Name field and in th
                item = WpfObjects._mainWindow.GetMultiple(SearchCriteria.ByClassName("TextBlock")).Single(item1 =>
                {
                    if (string.Equals(item1.Name, "Key1_Role"))
                        return true;
                    else
                        return false;
                });
                item.Click();
                for (int i = 0; i < 3; i++)
                {

                    keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.TAB);
                    keyboard.Enter(Datasources[i]);
                    keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.TAB);
                    keyboard.Enter(RoleFilters[i].Replace("DTSS", Datasources[i]));
                    keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.TAB);
                }
                ExecutedSteps++;
                ExecutedSteps++;
                //Step 19: Click on the Combine Roles button with an arrow pointing down.
                IUIItem[] Button = WpfObjects._mainWindow.GetMultiple(SearchCriteria.ByClassName("Button"));
                Button[0].Click();
                ExecutedSteps++;
                //Step 20: Click on the Create Combination Roles to add these roles i
                for (int i = 0; i < 17; i++)
                    keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.TAB);
                keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.SPACE);
                ExecutedSteps++;
                //Step 21: Enter userID=merge_admin, password = .adm.13579
                wpfobject.GetMainWindowByTitle("User Credential Form");
                wpfobject.WaitTillLoad();
                WpfObjects._mainWindow.Get(SearchCriteria.ByClassName("TextBox")).SetValue(Username);
                WpfObjects._mainWindow.Get(SearchCriteria.ByClassName("PasswordBox")).SetValue(Password);
                wpfobject.ClickRadioButton("Local", 1);
                wpfobject.ClickButton("OKButton");
                wpfobject.GetMainWindowByTitle("Ldap Role Management Form");
                wpfobject.WaitTillLoad();
                ExecutedSteps++;
                //Step 22: Click on the Base Roles tab 
                wpfobject.GetTabWpf(1).SelectTabPage(2);
                wpfobject.ClickButton("Use Combination Roles", 1);
                ExecutedSteps++;
                //Step 23: Click on the Create Base Roles to add these roles in the ICA server, make sure there is an ICA running before executing this function .
                wpfobject.ClickButton("Create Base Roles", 1);
                ExecutedSteps++;
                //Step 24: Enter userID=merge_admin, password = .adm.13579 Authentication mode = LDAP click on oK
                //Since Step 21 stores credentials, after executing step 23, step 24 will be executed internally.
                ExecutedSteps++;
                //Step 25: Click on the Common Settings TAB and click on the Save Details to File, enter a file name the is different than the one used in the previous save
                wpfobject.GetTabWpf(1).SelectTabPage(0);
                if (File.Exists(Profile2))
                {
                    File.Delete(Profile2);
                }
                wpfobject.ClickButton("Save Details To File", 1);
                wpfobject.WaitTillLoad();
                wpfobject.SetText("File name:", Profile2, 1);
                wpfobject.ClickButton("Save", 1);
                wpfobject.WaitTillLoad();
                if (File.Exists(Profile2))
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
                //step 26: Click on Done then Ok until you get to the SERVERS Tab page
                //wpfobject.ClickButton("Done", 1);
                wpfobject.ClickRadioButton("Anyone", 1);
                for (int i = 0; i < 4; i++)
                {
                    keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.TAB);
                }
                keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.SPACE);
                wpfobject.GetMainWindowByTitle("LDAP Server Control Form");
                wpfobject.WaitTillLoad();
                IUIItem[] bindgrp = WpfObjects._mainWindow.GetMultiple(SearchCriteria.ByClassName("TextBox"));
                bindgrp[1].Click();
                keyboard.Enter(Profile1);
                for (int i = 0; i < 8; i++)
                    keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.TAB);
                keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.SPACE);
                //wpfobject.ClickButton("OK", 1);
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.ClickButton(ServiceTool.ApplyBtn_Name, 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                basepage.ChangeAttributeValue(Config.DSAServerManagerConfiguration, "/server[@id='" + ServerName + "']", "enabled", "True");
                servicetool.RestartIISUsingexe();
                ExecutedSteps++;
                //Step 27: Open a browser and login to ICA as merge_admin/ .adm.13579
                login.LoginIConnect(LoginDetails[0].Split(':')[0], LoginDetails[0].Split(':')[1]);
                ExecutedSteps++;
                //Step 28: Go to the Role management Tab and select the Sacramento domain from the dropdown. Confirm the roles created from the service tool
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
                //Step 29: Go to the User management Tab and select from the domain dropdown "Sacramento"
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
                //Step 30: Select each group and observer the Users listed in the right side they should match the list in the Dignity Map Tab.
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
                //Step 31: Logout and log back in as reuben ben Jacob
                login.Logout();
                login.LoginIConnect(LoginDetails[1].Split(':')[0], LoginDetails[1].Split(':')[1]);
                login.Logout();
                ExecutedSteps++;
                //Step 32: Login as each user and verify the Datasource and filter is applied correctly based on the role.
                string[] unames = new string[] { LoginDetails[3].Split(':')[0] };
                string[] pwds = new string[] { LoginDetails[3].Split(':')[1] };
                string[] ExpectedModalities = Filters[1].Split(':');
                string[] ExpectedDatasources = new string[] { Datasources[1] };
                string[] ExpectedIssuerofPID = new string[] { string.Empty };
                resultcount = 0;
                for (int i = 0; i < unames.Length; i++)
                {
                    login.LoginIConnect(unames[i], pwds[i]);
                    studies = (Studies)login.Navigate("Studies");
                    studies.SearchStudy(LastName: "*");
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
                        else if (ActualModality.All(md => md.Contains(ExpectedModalities[i])))
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

                //Step 33: Login as judah / .jbj.13579 and load study
                login.LoginIConnect(LoginDetails[4].Split(':')[0], LoginDetails[4].Split(':')[1]);
                PageLoadWait.WaitForPageLoad(20);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(LastName: "*", Modality: "MR");
                string[] columnvalue = BasePage.GetColumnValues("Accession");
                int accession = Array.IndexOf(columnvalue, columnvalue.Where(x => !string.IsNullOrWhiteSpace(x)).FirstOrDefault());
                studies.SelectStudy("Accession", columnvalue[accession]);

                bool step33 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("n"))
                {
                    StudyViewer viewer = studies.LaunchStudy();
                    PageLoadWait.WaitForFrameLoad(20);
                    step33 = viewer.SeriesViewer_1X1().Displayed;
                    studies.CloseStudy();
                }
                else
                {
                    BluRingViewer viewer = BluRingViewer.LaunchBluRingViewer();
                    PageLoadWait.WaitForFrameLoad(20);
                    step33 = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_viewport)).Displayed;
                    viewer.CloseBluRingViewer();
                }
                if (step33)
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
        /// Reloading/editing Existing Multirole Role profiles
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>

        public TestCaseResult Test_27673(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            string ServerName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ServerName");
            string HostName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "HostName");
            string Port = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Port"));
            string Profile1 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Profile1");
            string Profile2 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Profile2");
            string Profile1updated = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Profile1Update");
            string Profile2updated = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Profile2Update");
            string LdapXMLFile = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LdapXMLPath");
            string LdapXML = File.ReadAllText(LdapXMLFile);
            string[] Keys = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Keys")).Split(':');
            string RoleFilters = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleFilters");
            string profile1path = string.Concat(Path.GetDirectoryName(LdapXMLFile), "\\", Path.GetFileName(Profile1));
            string profile2path = string.Concat(Path.GetDirectoryName(LdapXMLFile), "\\", Path.GetFileName(Profile2));
            string[] Datasources = new string[] { new Login().GetHostName(Config.PACS2), new Login().GetHostName(Config.SanityPACS) };
            int resultcount = 0;
            string[] Expected = null;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                //Precondition
                File.Copy(profile1path, Profile1, true);
                File.Copy(profile2path, Profile2, true);
                servicetool.CloseServiceTool();
                if (basepage.NodeExist(Config.DSAServerManagerConfiguration, "/server[@id='" + ServerName + "']"))
                {
                    basepage.RemoveNode(Config.DSAServerManagerConfiguration, "/server[@id='" + ServerName + "']");
                }
                basepage.InsertNode(Config.DSAServerManagerConfiguration, "/servers", LdapXML, false);
                basepage.InsertNode(Config.DSAServerManagerConfiguration, "/server[@id='" + ServerName + "']/options/hosts", string.Concat("<host name=\"", HostName, "\" port=\"", Port, "\" />"));
                servicetool.RestartIISUsingexe();
                // Step 1: Open the service tool and click on the LDAP tab, then click on the Server TAB.
                servicetool.LaunchServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToTab(ServiceTool.LDAP_Tab);
                wpfobject.GetTabWpf(1).SelectTabPage(1);
                ExecutedSteps++;

                // Step 2: Click on Modify and select the Ldap server created in the previous steps. Click on the Detail  button. In the Window that opens select the Data model Tab.
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

                // Step 3
                // Click on the General Rules  button
                wpfobject.SelectTabFromTabItems("Data Model");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Generate Rules", 1);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindowByTitle("Ldap Role Management Form");
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                // Step 4
                // Edit the key name by adding a new key4 
                // click on Generate Key Selector Combinations
                IUIItem[] KeyBox = WpfObjects._mainWindow.GetMultiple(SearchCriteria.ByClassName("TextBlock"));
                KeyBox[8].DoubleClick();
                TestStack.White.InputDevices.AttachedKeyboard keyboard = WpfObjects._mainWindow.Keyboard;
                keyboard.Enter(Keys[3]);
                wpfobject.ClickButton("Generate Key Selector Combinations", 1);
                wpfobject.WaitTillLoad();
                IUIItem[] SStxbox = WpfObjects._mainWindow.GetMultiple(SearchCriteria.ByClassName("DataGrid"));
                Expected = Keys.Take(4).ToArray();
                ListView view1 = wpfobject.GetAnyUIItem<ITabPage, ListView>(servicetool.GetCurrentTabItem(), "DomainDataSourceIdGridView");
                string[] Grid1 = view1.Rows.Select(v1 => v1.Cells[0].Name).ToArray();
                ListView view2 = wpfobject.GetAnyUIItem<ITabPage, ListView>(servicetool.GetCurrentTabItem(), "");
                string[] Grid2 = view2.Rows.Select(v2 => v2.Cells[0].Name).ToArray();
                resultcount = 0;
                
                if (Expected.All(exp => Grid1.Contains(exp)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Combination roles - Selector key contains 3 keys");
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

                // Step 5: In the Imported Roles Table add the Key4 to each of the MGH_xxx roles in the Associated Selector Keys, save the new profile as Profile-Mod

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

                // Step 6
                // Go to the location where the file was saved and confirm the changes by opening the file in a browser and observing the new Key4 entry.
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

                // Step 7
                // Test Data: Open and edit an existing Mapped Combination and base Roles Profiles
                // Open the Service tool and select the LDAP tab, select Modify and 
                // select the previously created  LDAP profile, navigate to the Create Roles Tab

                wpfobject.SelectTabFromTabItems("Create Roles");
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                // Step 8
                // Click on Load Details From File and select and load the Previously saved Profile XML file

                wpfobject.ClickButton("Load Details From File", 1);
                wpfobject.WaitTillLoad();
                wpfobject.SetText("File name:", Profile2, 1);
                wpfobject.ClickButton("Open", 1);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                // Step 9
                // Click on the Enable Emergency Access flag Add a Datasource  ECMPACS
                // Click on Mapped Combination Roles and add the new datasource ECMPACS to the Key1_Role. 
                // For Key2_Role add the Modality CR to the filter settings. 

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

                // Step 10
                // Click on the Combine Roles button with an arrow pointing down.
                IUIItem[] Button = WpfObjects._mainWindow.GetMultiple(SearchCriteria.ByClassName("Button"));
                Button[0].Click();
                ExecutedSteps++;

                // Step 11
                // Click on the"Create Selected Combination Roles Only"box  and select the Roles that have the Key4 entry. 
                // Click on the Create Combination Roles.
                wpfobject.SelectCheckBox("Create Selected Combination Roles Only", 1);
                wpfobject.WaitTillLoad();

                wpfobject.ClickButton("Create Combination Roles", 1);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                // Step 12
                // Enter userID=Administrator, password = Administrator Authentication mode = LOCAL click on oK

                wpfobject.GetMainWindowByTitle("User Credential Form");
                wpfobject.WaitTillLoad();
                WpfObjects._mainWindow.Get(SearchCriteria.ByClassName("TextBox")).SetValue(Config.adminUserName);
                WpfObjects._mainWindow.Get(SearchCriteria.ByClassName("PasswordBox")).SetValue(Config.adminPassword);
                wpfobject.ClickRadioButton("Local", 1);
                wpfobject.ClickButton("OKButton");
                wpfobject.GetMainWindowByTitle("Ldap Role Management Form");
                wpfobject.WaitTillLoad();
                ExecutedSteps++;


                // Step 13
                ExecutedSteps++;
                // "NotCondition AUtomated";

                // Step 14: Click on the Base Roles tab and in the page that opens click on the Use Combination Roles button, this Updates the table with the datasource and Filter entries for each base role in the ldap.

                wpfobject.GetTabWpf(1).SelectTabPage(2);
                wpfobject.ClickButton("Use Combination Roles", 1);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                // Step 15: Click on the"Create Selected Base Roles Only"box and select the entries containing ECMPACS and Click on Create Base Roles.
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


                // Step 16: Since Step 12 stores credentials, after executing step 15, step 16 will be executed internally.
                ExecutedSteps++;

                // Step 17:  Click on the Common Settings TAB and click on the Save Details to File, enter a file name the is different than the one used in the previous save. 
                // Example = Profile2_update.XML
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
                IUIItem[] bindgrp = WpfObjects._mainWindow.GetMultiple(SearchCriteria.ByClassName("TextBox"));
                bindgrp[1].SetValue(Profile1updated);
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

                // Step 18
                // Open the XML file (Profile2_update.XML)and confirm the changes were saved.

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

                wpfobject.WaitTillLoad();
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
                servicetool.RestartIISUsingexe();
            }
        }
    }
}
