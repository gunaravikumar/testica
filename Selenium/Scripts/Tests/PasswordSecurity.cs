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
using Selenium.Scripts.Pages.Putty;
using System.Globalization;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MergeServiceTool;
using TestStack.White.Factory;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.TableItems;
using TestStack.White.InputDevices;
using TestStack.White.WindowsAPI;
using Application = TestStack.White.Application;
using Button = TestStack.White.UIItems.Button;
using CheckBox = TestStack.White.UIItems.CheckBox;
using ComboBox = TestStack.White.UIItems.ListBoxItems.ComboBox;
using ListView = TestStack.White.UIItems.ListView;
using RadioButton = TestStack.White.UIItems.RadioButton;
using TextBox = TestStack.White.UIItems.TextBox;
using Window = TestStack.White.UIItems.WindowItems.Window;
using Panel = TestStack.White.UIItems.Panel;
using TestStack.White.Configuration;

namespace Selenium.Scripts.Tests
{
    class PasswordSecurity
    {
        public Login login { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public HPLogin hplogin { get; set; }
        public Configure configure { get; set; }
        public HPHomePage hphomepage { get; set; }
        public ExamImporter ei { get; set; }
        public string filepath { get; set; }

        public PasswordSecurity(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            mpaclogin = new MpacLogin();
            hplogin = new HPLogin();
            configure = new Configure();
            hphomepage = new HPHomePage();
            ei = new ExamImporter();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }

        ServiceTool servicetool = new ServiceTool();
        DomainManagement domainmanagement = new DomainManagement();
        UserManagement usermanagement = new UserManagement();
        MyProfile MyProfilePage = new MyProfile();
        WpfObjects wpfobject = new WpfObjects();
        EnrollNewUser EnrollUser = new EnrollNewUser();

        /// <summary>
        /// Initial Setup - Service tool
        /// </summary>
        public TestCaseResult Test1_29373(String testid, String teststeps, int stepcount)
        {
            //Fetch the data
            String Contactinfo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ContactInformation");
            String[] info = Contactinfo.Split('=');

            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                //Step 1 - Launch Service Tool Application
                servicetool.LaunchServiceTool();
                ExecutedSteps++;

                //Step 2 - User management Database tab: Local Database is selected, Ldap Directory Service is not selected
                servicetool.NavigateToTab(ServiceTool.UserManagement_Tab);
                //servicetool.SetMode(0);
                ExecutedSteps++;

                //Step 3 -  Data Source tab: At least 1 datasource added
                ExecutedSteps++;

                //Step 4 - Option 'Enable Password Policy' from Security Tab\Password Policy sub-tab is not selected (default setting)
                servicetool.NavigateToConfigToolSecurityTab();
                wpfobject.GetTabWpf(1).SelectTabPage(1);
                Thread.Sleep(2500);
                servicetool.ClickModifyFromTab();
                servicetool.SetPassWordPolicy(true);
                servicetool.UpdateAdminContact(info[0] + Environment.NewLine + info[1] + Environment.NewLine + info[2]);
                servicetool.ClickApplyButtonFromTab();
                servicetool.AcceptDialogWindow();
                ExecutedSteps++;

                //Step 5 - E-mail Notification tab\General sub-tab: add 'dinobot.win.cedara.com' in Server Host/IP
                //servicetool.SetEmailNotification();
                ExecutedSteps++;

                //Step 6 - Enable features in Enable Features sub-tab: 1. Enable Self Enrollment 2. Enable Email Study
                servicetool.NavigateToEnableFeatures();
                Thread.Sleep(2500);
                servicetool.ModifyEnableFeatures();
                servicetool.EnableSelfEnrollment();
                servicetool.EnableEmailStudy();
                servicetool.ApplyEnableFeatures();
                Thread.Sleep(2000);
                wpfobject.ClickOkPopUp();
                Thread.Sleep(2000);
                ExecutedSteps++;

                //Apply changes and IIS reset.
                servicetool.RestartService();
                servicetool.CloseServiceTool();

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
        /// Initial Setup - ICA Web
        /// </summary>
        public TestCaseResult Test2_29373(String testid, String teststeps, int stepcount)
        {
            //Fetch the data
            String Contactinfo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ContactInformation");
            String[] info = Contactinfo.Split('=');
            String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");

            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;

                //Step 1 - Navigate to the iConnectAccess server in a web browser
                login.DriverGoTo(login.url);
                ExecutedSteps++;

                //Step 2 - Login as Administrator
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //Step 3 -  Navigate to Domain Management Page
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                ExecutedSteps++;

                //Step 4 - Ensure that the Default System Domain has datasource(s) connected.
                if (domainmanagement.IsDomainExist(DefaultDomain))
                {
                    domainmanagement.SelectDomain(DefaultDomain);
                    domainmanagement.ClickEditDomain();
                    domainmanagement.ConnectDataSourcesInNewDomain();
                    domainmanagement.ClickSaveEditDomain();
                    ExecutedSteps++;
                }

                //Step 5 - Logout as administrator
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
        /// Password Policy not applied
        /// </summary>
        public TestCaseResult Test_29374(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables
            Studies studies;
            DomainManagement domainmanagement;
            UserManagement usermanagement;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;


            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String username = "User1" + new Random().Next(1, 10000);
                String username1 = "User2" + new Random().Next(1, 10000);
                String sysadmin = "sadmin1" + new Random().Next(1, 10000);
                String newdomainadmin = "Domain2" + new Random().Next(1, 10000); ;
                String domainadm = "dadmin1" + new Random().Next(1, 10000);
                String domainname = "TESTDomain" + new Random().Next(1, 10000);
                String defaultdomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");
                String subgroupname = "TestSubgroup" + new Random().Next(1, 10000);
                String rolename = "TestRole" + new Random().Next(1, 10000);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String datasource = login.GetHostName(Config.SanityPACS);

                /*Setting the Default Setting*/
                servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.Security_Tab);
                servicetool.NavigateSubTab(ServiceTool.Security.Name.PasswordPolicy_tab);
                servicetool.ClickModifyFromTab();
                if (wpfobject.GetAnyUIItem<Window, CheckBox>(WpfObjects._mainWindow, ServiceTool.Security.Name.EnablePasswordPolicy, 1).Checked)
                {
                    servicetool.SetPassWordPolicy(false);
                    servicetool.ClickApplyButtonFromTab();
                    servicetool.AcceptDialogWindow();
                }
                else
                {
                    servicetool.ClickApplyButtonFromTab();
                }
                servicetool.RestartService();
                servicetool.CloseServiceTool();


                //Step-1:Keep the default setting in service tool & login as Administrator
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //Step-2:Create  new domainadmin for new domain from DomainManagement with simple password and validate the icon & whether domainadmin is created
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForFrameLoad(20);
                domainmanagement.ClickNewDomainBtn();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                Boolean icon2 = BasePage.Driver.FindElement(By.CssSelector("#PwdRequirementIcon")).Displayed;

                domainmanagement.ClickButton("[id$='_CloseButton']");
                PageLoadWait.WaitForFrameLoad(20);

                String Pwd4 = domainmanagement.CreateSimplePassword(5, 10, "numbers");
                domainmanagement.CreateDomain(domainname, rolename, datasource, domainadm, Pwd4 + domainadm);
                domainmanagement.ClickSaveNewDomain();
                bool searchresult3 = domainmanagement.SearchDomain(domainname);
                if (icon2 == false && searchresult3 == true)
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

                //Step-3:Create a user with simple password with length less than 3 or more than 10 and validate whether it is created
                //Navigate to UserManagement tab
                usermanagement = (UserManagement)login.Navigate("UserManagement");

                //Click new user and verify PasswordRequirement icon is not displayed
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                usermanagement.ClickButton("#NewUserButton");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                IWebElement icon = BasePage.Driver.FindElement(By.CssSelector("#PwdRequirementIcon"));

                usermanagement.ClickButton("div#NewUesrDialogDiv>div>span");
                PageLoadWait.WaitForFrameLoad(20);

                String Pwd0 = usermanagement.CreateSimplePassword(1, 2, "numbers");
                usermanagement.CreateUser(username, domainname, rolename, 1, "", 1, Pwd0);
                bool searchresult = usermanagement.SearchUser(username, domainname);

                String Pwd1 = usermanagement.CreateSimplePassword(10, 15, "characters");
                usermanagement.CreateUser(username1, domainname, rolename, 1, "", 0, Pwd1 + username1);
                bool searchresult0 = usermanagement.SearchUser(username1, domainname);
                if (icon.Displayed == false && searchresult == true && searchresult0 == true)
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

                //Step-4:Create  new system admin with simple password and validate the icon & whether sysadmin is created
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                usermanagement.ClickButton("#NewSystemAdminButton");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                Boolean icon0 = BasePage.Driver.FindElement(By.CssSelector("#PwdRequirementIcon")).Displayed;

                usermanagement.ClickButton("[id$='_CloseButton']");
                PageLoadWait.WaitForFrameLoad(20);

                String Pwd2 = usermanagement.CreateSimplePassword(5, 10, "both");
                usermanagement.CreateSystemAdminUser(sysadmin, domainname, 1, "", 0, Pwd2 + sysadmin);
                bool searchresult1 = usermanagement.SearchUser(sysadmin, defaultdomain);
                if (icon0 == false && searchresult1 == true)
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

                //Step-5:Create  new domainadmin from UserManagement with simple password and validate the icon & whether domainadmin is created
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                usermanagement.ClickButton("#NewDomainAdminButon");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                PageLoadWait.WaitForFrameLoad(30);
                Boolean icon1 = BasePage.Driver.FindElement(By.CssSelector("#PwdRequirementIcon")).Displayed;
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                usermanagement.ClickButton("[id$='_CloseButton']");
                PageLoadWait.WaitForFrameLoad(20);

                String Pwd3 = usermanagement.CreateSimplePassword(5, 10, "both");
                usermanagement.CreateDomainAdminUser(newdomainadmin, domainname, 1, "", 1, Pwd3 + newdomainadmin);
                bool searchresult2 = usermanagement.SearchUser(newdomainadmin, domainname);
                if (icon1 == false && searchresult2 == true)
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

                //Step-6:Create Group from UserManagement Page with simple password and validate the icon & whether group is created
                String groupadmin1 = "gadmin1" + new Random().Next(1, 10000);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                usermanagement.ClickButton("#NewGroupButton");
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                BasePage.Driver.FindElement(By.CssSelector("#m_groupInfoDialog_ManagedGroup_yes")).Click();
                new SelectElement(BasePage.Driver.FindElement(By.CssSelector("select#m_groupInfoDialog_ManagedByDropDownList"))).SelectByText("< New User >");
                //string Newuser = BasePage.Driver.FindElement(By.CssSelector("select#m_groupInfoDialog_ManagedByDropDownList>option[value='0']")).GetAttribute("option");
                //usermanagement.Click("cssselector", "select#m_groupInfoDialog_ManagedByDropDownList>option");
                //((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector('select#m_groupInfoDialog_ManagedByDropDownList>option[value='0']').click()");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#CreateManagingUserDiv")));
                Boolean icon3 = BasePage.Driver.FindElement(By.CssSelector("#PwdRequirementIcon")).Displayed;

                usermanagement.ClickButton("div#GroupInfoDialogDiv>div>span");
                PageLoadWait.WaitForFrameLoad(20);

                String Pwd5 = usermanagement.CreateSimplePassword(5, 10, "both");
                usermanagement.CreateGroup(domainname, groupadmin1, Pwd5, rolename, "", groupadmin1, 1, GroupUser: groupadmin1);
                bool searchresult4 = usermanagement.SearchGroup(groupadmin1, domainname, 0);
                if (icon3 == false && searchresult4 == true)
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

                //Step-7:Create subgroup from UserManagement Page with simple password and validate the icon & whether subgroup is created
                String groupadmin2 = "gadmin2" + new Random().Next(1, 10000);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                usermanagement.ClickButton("#NewGroupButton");
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#createGroupDiv")));
                usermanagement.SetText("cssselector", "#m_groupInfoDialog_m_groupName", username);
                usermanagement.ClickButton("#m_groupInfoDialog_SaveAndCreateSubButton");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                BasePage.Driver.FindElement(By.CssSelector("#m_groupInfoDialog_ManagedGroup_yes")).Click();
                //usermanagement.Click("cssselector", "select#m_groupInfoDialog_ManagedByDropDownList>option[value='0']");
                new SelectElement(BasePage.Driver.FindElement(By.CssSelector("select#m_groupInfoDialog_ManagedByDropDownList"))).SelectByText("< New User >");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#CreateManagingUserDiv")));
                Boolean icon5 = BasePage.Driver.FindElement(By.CssSelector("#PwdRequirementIcon")).Displayed;

                usermanagement.ClickButton("div#GroupInfoDialogDiv>div>span");
                PageLoadWait.WaitForFrameLoad(20);

                usermanagement.CreateGroup(domainname, groupadmin2, Pwd5, rolename, "", groupadmin2, 0, subgroupname, 1);
                bool searchresult5 = usermanagement.SearchGroup(subgroupname, domainname, 1);
                if (icon5 == false && searchresult5 == true)
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

                //Step-8:Modify the password of the user 
                //Navigate to UserManagement tab
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                //Select the user and edit
                usermanagement.SearchUser(username1, domainname);
                usermanagement.SelectUser(username1);
                usermanagement.ClickEditUser();
                PageLoadWait.WaitForFrameLoad(15);
                usermanagement.EditUser(Pwd1);
                ExecutedSteps++;

                //Step-9:Modify the password of the domainadmin
                usermanagement.SearchUser(newdomainadmin, domainname);
                usermanagement.SelectUser(newdomainadmin);
                usermanagement.ClickEditUser();
                PageLoadWait.WaitForFrameLoad(15);
                usermanagement.EditDomainAdmin(Pwd3);
                ExecutedSteps++;

                //Step-10:Modify the password of the sysadmin
                //Select the user and edit
                usermanagement.SearchUser(sysadmin, defaultdomain);
                usermanagement.SelectUser(sysadmin);
                usermanagement.ClickEditUser();
                PageLoadWait.WaitForFrameLoad(15);
                usermanagement.EditSysAdmin(Pwd2);
                ExecutedSteps++;

                //Logout as administrator
                login.Logout();

                //Step-11:Login as user1
                login.LoginIConnect(username, Pwd0);

                //Verify Password icon from My profile
                studies = (Studies)login.Navigate("Studies");
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("img[src^='Images/options']")));
                BasePage.Driver.FindElement(By.CssSelector("img[src^='Images/options']")).Click();
                //string ElementId = studies.GetElement("xpath", "//*[@id='options_menu']/a[2]").GetAttribute("id");
                string ElementId = "";
                IList<IWebElement> elements = new List<IWebElement>();
                if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                {
                    elements = BasePage.Driver.FindElements(By.CssSelector("div[id='options_menu'] a"));
                    ElementId = elements[1].GetAttribute("id");
                }
                else
                {
                    ElementId = studies.GetElement("xpath", "//*[@id='options_menu']/a[2]").GetAttribute("id");
                }
                var js = BasePage.Driver as IJavaScriptExecutor;
                if (js != null) js.ExecuteScript("document.getElementById('" + ElementId + "').click();");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                IWebElement icon6 = BasePage.Driver.FindElement(By.CssSelector("#PwdRequirementIcon"));
                if (icon6.Displayed == false)
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
                studies.ClickButton("[id$='_CloseButton']");
                PageLoadWait.WaitForFrameLoad(20);

                //Update Password from My profile
                String Pwd01 = usermanagement.CreateSimplePassword(1, 4, "numbers");
                studies.UpdateMyProfile(Pwd01);

                //Logout as user1
                login.Logout();

                //Step-12:Fill details in the Register link from login page
                login.DriverGoTo(login.url);

                //Fill the details in the enrollment form
                String newuser = "EnrollUser1" + new Random().Next(1, 1000);

                login.FillEnrollForm(newuser, domainname, groupadmin1, newuser, newuser, email);

                //Login as administrator
                login.LoginIConnect(adminUserName, adminPassword);
                //Navigate to UserManagement tab
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                //Approve the Request from Request subtab
                String Pwd = usermanagement.CreateSimplePassword(5, 15, "both");
                bool iconresult = usermanagement.AcceptRequest(newuser, Pwd + newuser);
                if (iconresult == true)
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

                //Step-13:Create a new user with valid emailid without password
                login.Logout();
                login.LoginIConnect(adminUserName, adminPassword);
                //Navigate to UserManagement tab
                usermanagement = (UserManagement)login.Navigate("UserManagement");

                String Emailuser = "Emailuser" + new Random().Next(1, 1000);
                usermanagement.CreateUser(Emailuser, domainname, rolename, 1, email);
                ExecutedSteps++;

                //Step-14:Click Link from Email notification(Not Automated)
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Logout 
                login.Logout();

                //Step-15:Check in all supported browsers
                ExecutedSteps++;

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Logout 
                login.Logout();

                //Return Result
                return result;

            }

        }

        /// <summary>
        /// Password Policy Applied - Set Password Policy to: Minimum Length: 3, Maximum Length: 10
        /// </summary>
        public TestCaseResult Test1_29375(String testid, String teststeps, int stepcount)
        {
            //Fetch the data
            String Contactinfo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ContactInformation");
            String[] info = Contactinfo.Split('=');
            String InvalidPasswordList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "InvalidPasswordList");

            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                //Step 1 - Launch Service Tool Application
                servicetool.LaunchServiceTool();
                ExecutedSteps++;

                //Step 2 - Navigate to Security Tab -> Password Policy sub-tab
                servicetool.NavigateToConfigToolSecurityTab();
                wpfobject.GetTabWpf(1).SelectTabPage(1);
                Thread.Sleep(2500);
                ExecutedSteps++;

                //Step 3 - Click on 'Modify' button
                servicetool.ClickModifyFromTab();
                ExecutedSteps++;

                //Step 4 - Set the password policy to: Minimum Length: 3, Maximum Length: 10, Invalid Password List: empty
                servicetool.SetPassWordPolicy(true);
                servicetool.SetMinPasswordLength(3);
                servicetool.SetMaxPasswordLength(10);
                servicetool.SetPasswordCriteriaCount("0");
                servicetool.UpdateAdminContact(info[0] + Environment.NewLine + info[1] + Environment.NewLine + info[2]);
                servicetool.UpdateInvalidPasswordList("removeall", new string[] { InvalidPasswordList });
                ExecutedSteps++;

                //Step 5 - Apply changes and IIS reset.
                servicetool.ClickApplyButtonFromTab();
                servicetool.AcceptDialogWindow();
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Return Result
                result.FinalResult(ExecutedSteps);

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
        /// Password Policy Applied - Create new user for the above password policy
        /// </summary>
        public TestCaseResult Test2_29375(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String User1 = "User1" + new Random().Next(1, 10000);
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");

                //Step 1 - Login iCA as Administrator in a web browser. 
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //Step 2 - Navigate to User Management page
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                ExecutedSteps++;

                //Step 3 - Click New User in User Management                   
                usermanagement.ClickNewUser();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step 4 - Click the icon beside the Password field
                usermanagement.PwdRequirementIcon().Click();
                if (usermanagement.PwdCriteriaTxt().Text.Equals("Between: 3 - 10 characters"))
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

                //Step 5 - Click on the Close icon [X]
                usermanagement.XIcon().Click();
                ExecutedSteps++;

                //Step 6 - Click the icon beside the password field twice.
                usermanagement.DoubleClick(usermanagement.PwdRequirementIcon());
                usermanagement.ClickButton("#NewUesrDialogDiv > div.titlebar > span");
                ExecutedSteps++;

                //Step 7 - Attempt to create a user and enter password which doesn't match the criteria set in Service Tool (length*^<^*3, length*^-^*10)
                usermanagement.CreateUser(User1, DefaultDomain, DefaultRoleName, 1, Email, 1, "P1", 1);
                if (usermanagement.NewUsrErrMsg().Text.Equals("The password you have entered does not meet minimum requirements."))
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
                usermanagement.ClickButton("#NewUesrDialogDiv > div.titlebar > span");

                //Step 8 - Enter password which matches with all criteria set in Service Tool. (length-3, length-10, length -7)<br/-Repeat the step for all support browsers
                usermanagement.CreateUser(User1, DefaultDomain, DefaultRoleName, 1, Email, 1, "Pass123");
                ExecutedSteps++;

                //Step 9 - Repeat the step for all support browsers
                ExecutedSteps++;

                //Step 10 - Logout 
                login.Logout();
                ExecutedSteps++;

                //Step 11 - Login iCA as the newly created user with invalid password.
                login.LoginIConnect(User1, "P1");
                PageLoadWait.WaitForPageLoad(20);
                if (login.LoginErrorMsgLabel().Text.Equals("The system cannot log you in. Please try again.:"))
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
                login.CloseBrowser();

                //Step 12 - Repeat above step, this time use valid password.
                login.LoginIConnect(User1, "Pass123");
                ExecutedSteps++;

                //Logout
                login.Logout();

                //Return Result
                result.FinalResult(ExecutedSteps);

                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

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
        /// Password Policy Applied - Set Password Policy to - Minimum Length - 1, Maximum Length - 100
        /// </summary>
        public TestCaseResult Test3_29375(String testid, String teststeps, int stepcount)
        {
            //Fetch the data
            String Contactinfo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ContactInformation");
            String[] info = Contactinfo.Split('=');
            String InvalidPasswordList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "InvalidPasswordList");


            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                //Step 1 - Launch Service Tool Application
                servicetool.LaunchServiceTool();
                ExecutedSteps++;

                //Step 2 - Navigate to Security Tab -> Password Policy sub-tab
                servicetool.NavigateToConfigToolSecurityTab();
                wpfobject.GetTabWpf(1).SelectTabPage(1);
                Thread.Sleep(2500);
                ExecutedSteps++;

                //Step 3 - Click on 'Modify' button
                servicetool.ClickModifyFromTab();
                ExecutedSteps++;

                //Step 4 - Set the password policy to: Minimum Length: 3, Maximum Length: 10, Invalid Password List: empty
                servicetool.SetPassWordPolicy(true);
                servicetool.SetMinPasswordLength(1);
                servicetool.SetMaxPasswordLength(100);
                servicetool.SetPasswordCriteriaCount("0");
                servicetool.UpdateAdminContact(info[0] + Environment.NewLine + info[1] + Environment.NewLine + info[2]);
                servicetool.UpdateInvalidPasswordList("removeall", new string[] { InvalidPasswordList });
                ExecutedSteps++;

                //Step 5 - Apply changes and IIS reset.
                servicetool.ClickApplyButtonFromTab();
                servicetool.AcceptDialogWindow();
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Return Result
                result.FinalResult(ExecutedSteps);

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
        /// Password Policy Applied - Create new user for the above password policy
        /// </summary>
        public TestCaseResult Test4_29375(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables          
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;
                String User1 = "User1" + new Random().Next(1, 10000);
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");

                //Step 1 - Login iCA as Administrator in a web browser. 
                login.LoginIConnect(Username, Password);
                ExecutedSteps++;

                //Step 2 - Navigate to User Management page
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(20);
                ExecutedSteps++;

                //Step 3 - Click the icon beside the Password field
                usermanagement.ClickNewUser();
                PageLoadWait.WaitForFrameLoad(20);
                usermanagement.PwdRequirementIcon().Click();
                if (usermanagement.PwdCriteriaTxt().Text.Equals("Between: 1 - 100 characters"))
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

                //Step 4 - Click on the Close icon [X]
                usermanagement.XIcon().Click();
                usermanagement.XBtn().Click();
                ExecutedSteps++;

                //Step 5 - Attempt to create a user and enter password which doesn't match the criteria set in Service Tool, try various passwords that are not satisfy the defined criteria.
                usermanagement.CreateUser(User1, DefaultDomain, DefaultRoleName, 1, "", 1, "", 1);
                if (usermanagement.NewUsrErrMsg().Text.Equals("A password or E-mail address is required."))
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
                usermanagement.XBtn().Click();
                //usermanagement.ClickButton("#NewUesrDialogDiv > div.titlebar > span");

                //Step 6 - Enter password which matches with criteria set in Service Tool.
                usermanagement.CreateUser(User1, DefaultDomain, DefaultRoleName, 1, Email, 1, "Pass123");
                bool IsUserExist = usermanagement.IsUserExist(User1, DefaultDomain);
                if (IsUserExist)
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

                //Step 7 - Logout. Login as the newly created user with invalid password 
                login.Logout();
                login.LoginIConnect(User1, " ");
                ExecutedSteps++;

                ////Step 8 - Login iCA as the newly created user with valid password.
                login.LoginIConnect(User1, "Pass123");
                ExecutedSteps++;

                //Logout
                login.Logout();

                //Return Result
                result.FinalResult(ExecutedSteps);

                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

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
        /// Password Policy Applied - Set Password Policy to - Minimum Length - 8, Maximum Length - 14
        /// </summary>
        public TestCaseResult Test5_29375(String testid, String teststeps, int stepcount)
        {
            //Fetch the data
            String Contactinfo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ContactInformation");
            String[] info = Contactinfo.Split('=');
            String InvalidPasswordList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "InvalidPasswordList");

            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                //Step 1 - Launch Service Tool Application
                servicetool.LaunchServiceTool();
                ExecutedSteps++;

                //Step 2 - Navigate to Security Tab -> Password Policy sub-tab
                servicetool.NavigateToConfigToolSecurityTab();
                wpfobject.GetTabWpf(1).SelectTabPage(1);
                Thread.Sleep(2500);
                ExecutedSteps++;

                //Step 3 - Click on 'Modify' button
                servicetool.ClickModifyFromTab();
                ExecutedSteps++;

                //Step 4 - Set the password policy to: Minimum Length: 8, Maximum Length: 14, Invalid Password List: empty
                servicetool.SetPassWordPolicy(true);
                servicetool.SetMinPasswordLength(8);
                servicetool.SetMaxPasswordLength(14);
                servicetool.UpdateAdminContact(info[0] + Environment.NewLine + info[1] + Environment.NewLine + info[2]);
                //servicetool.SetPasswordPreferences(PasswordPrefernce true);
                servicetool.SetPasswordCriteriaCount("3");
                servicetool.SetPasswordCriteria("lowercase");
                servicetool.SetPasswordCriteria("uppercase");
                servicetool.SetPasswordCriteria("digits");
                servicetool.SetPasswordCriteria("specialchars");
                servicetool.UpdateInvalidPasswordList("removeall", new string[] { InvalidPasswordList });
                ExecutedSteps++;

                //Step 5 - Apply changes and IIS reset.
                servicetool.ClickApplyButtonFromTab();
                servicetool.AcceptDialogWindow();
                servicetool.RestartService();
                servicetool.CloseServiceTool();
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
        /// Password Policy Applied - Create new user for the above password policy
        /// </summary>
        public TestCaseResult Test6_29375(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;
                String User1 = "User1" + new Random().Next(1, 10000);
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");

                //Step 1 - Login iCA as Administrator in a web browser. 
                login.LoginIConnect(Username, Password);
                ExecutedSteps++;

                //Step 2 - Navigate to User Management page
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                ExecutedSteps++;

                //Step 3 - Click the icon beside the Password field                
                usermanagement.ClickNewUser();
                PageLoadWait.WaitForFrameLoad(20);
                usermanagement.ClickButton("#PwdRequirementIcon");
                if (usermanagement.PasswordCriteriaText().Equals("Between: 8 - 14 characters Uppercase characters Lowercase characters Digits Special characters e.g. !, $, #, or %"))
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

                //Step 4 - Click on the Close icon [X]
                usermanagement.XIcon().Click();
                Thread.Sleep(5000);
                usermanagement.XBtn().Click();
                ExecutedSteps++;

                //Step 5 - Attempt to create a user and enter password which doesn't match the criteria set in Service Tool, try various passwords that are not satisfy the defined criteria.
                usermanagement.CreateUser(User1, DefaultDomain, DefaultRoleName, 1, Email, 1, "Pass123", 1);
                if (usermanagement.NewUsrErrMsg().Text.Equals("The password you have entered does not meet minimum requirements."))
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
                //BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                usermanagement.ClickButton("div#NewUesrDialogDiv>div>span");

                //Step 6 - From User Management page, click New User, create a new User, try various passwords that satisfy the defined criteria.
                usermanagement.CreateUser(User1, DefaultDomain, DefaultRoleName, 1, Email, 1, "Pass@123");
                if (usermanagement.IsUserExist(User1, DefaultDomain))
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

                //Step 7 - Logout
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

                //Logout 
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Password Policy Applied - Create new System and Domain admin for the above password policy
        /// </summary>
        public TestCaseResult Test7_29375(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables                
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;
                String User1 = "User1" + new Random().Next(1, 10000);
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");
                String SysAdmin = "SysAdmin1" + new Random().Next(1, 10000);
                String DomainAdmin = "DomainAdmin1" + new Random().Next(1, 10000);
                String DomainName = "Domain1" + new Random().Next(1, 10000);
                String SubgroupName = "TestSubgroup" + new Random().Next(1, 10000);
                String RoleName = "TestRole" + new Random().Next(1, 10000);

                //Step 1 - Login iCA as Administrator in a web browser. 
                login.LoginIConnect(Username, Password);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                ExecutedSteps++;

                //Step 2 - From User management page, click New Domain Admin, create a new Domain Admin, try various passwords that satisfy the defined criteria.                                
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");

                if (BasePage.SBrowserName.ToLower().Equals("internet explorer"))
                    usermanagement.ClickElement(usermanagement.NewDomainAdminBtn());
                else
                    usermanagement.NewDomainAdminBtn().Click();

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                usermanagement.ClickButton("#PwdRequirementIcon");
                PageLoadWait.WaitForFrameLoad(5);
                bool PwdCriteriaTxt = usermanagement.PasswordCriteriaText().Equals("Between: 8 - 14 characters Uppercase characters Lowercase characters Digits Special characters e.g. !, $, #, or %");
                //usermanagement.Click("cssselector", "span[class='ui-icon ui-icon-closethick']");
                PageLoadWait.WaitForFrameLoad(5);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                usermanagement.CloseBtn().Click();
                PageLoadWait.WaitForFrameLoad(20);
                usermanagement.CreateDomainAdminUser(DomainAdmin, DefaultDomain, 1, Email, 1, "Pass@123");
                Thread.Sleep(5000);
                if (PwdCriteriaTxt && usermanagement.IsUserExist(DomainAdmin, DefaultDomain))
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

                //Step 3 - Create a new System Admin, try various passwords that satisfy the defined criteria.
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");

                if (BasePage.SBrowserName.ToLower().Equals("internet explorer"))
                    usermanagement.ClickElement(usermanagement.NewSysAdminBtn());
                else
                    usermanagement.NewSysAdminBtn().Click();


                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (BasePage.SBrowserName.ToLower().Equals("internet explorer"))
                    usermanagement.ClickElement(usermanagement.PwdRequirementIcon());
                else
                    usermanagement.PwdRequirementIcon().Click();


                bool PwdCriteriaTxt1 = usermanagement.PasswordCriteriaText().Equals("Between: 8 - 14 characters Uppercase characters Lowercase characters Digits Special characters e.g. !, $, #, or %");
                //usermanagement.XIcon().Click();
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (BasePage.SBrowserName.ToLower().Equals("internet explorer"))
                    usermanagement.ClickElement(usermanagement.CloseBtn());
                else
                    usermanagement.CloseBtn().Click();


                PageLoadWait.WaitForFrameLoad(20);
                usermanagement.CreateSystemAdminUser(SysAdmin, DomainName, 0, "", 1, "Pass@123");
                PageLoadWait.WaitForFrameLoad(20);
                if (PwdCriteriaTxt1 && usermanagement.IsUserExist(SysAdmin, DefaultDomain))
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

                //Step 4 - Navigate to Domain Management Page
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                ExecutedSteps++;

                //Step 5 - Create a new domain admin, try various passwords that satisfy the defined criteria.                
                domainmanagement.ClickNewDomainBtn();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                usermanagement.ClickButton("#PwdRequirementIcon");
                bool PwdCriteriaTxt2 = domainmanagement.PasswordCriteriaText().Equals("Between: 8 - 14 characters Uppercase characters Lowercase characters Digits Special characters e.g. !, $, #, or %");
                //domainmanagement.XIcon().Click();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                domainmanagement.CloseButton().Click();
                PageLoadWait.WaitForFrameLoad(20);
                domainmanagement.CreateDomain(DomainName, RoleName, 1, "Pass@123");
                PageLoadWait.WaitForFrameLoad(20);
                if (PwdCriteriaTxt1 && domainmanagement.IsDomainExist(DomainName))
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

                //Step 6 - Logout from ICA
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

                //Logout 
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Password Policy Applied - Create new Domain, Group and Subgroup for the above password policy
        /// </summary>
        public TestCaseResult Test8_29375(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables                
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;
                String User1 = "User1" + new Random().Next(1, 10000);
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");
                String SysAdmin = "SysAdmin1" + new Random().Next(1, 10000);
                String DomainAdmin = "DomainAdmin1" + new Random().Next(1, 10000);
                String DomainName = "Domain1" + new Random().Next(1, 10000);
                String GroupName = "Group1" + new Random().Next(1, 10000);
                String SubGroupName = "TestSubgroup" + new Random().Next(1, 10000);
                String RoleName = "TestRole" + new Random().Next(1, 10000);

                //Step 1 - Login iCA as Administrator in a web browser. 
                login.LoginIConnect(Username, Password);
                ExecutedSteps++;

                //Step 2 - Click New User in User Management. 
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                ExecutedSteps++;

                //Step 3 - Create a new domain admin
                domainmanagement.CreateDomain(DomainAdmin, RoleName, 1, "Pass@123");
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step 4 - Navigate to user management page and Select a new group in the new domain
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.SelectDomainFromDropdownList(DomainAdmin);
                ExecutedSteps++;

                //Step 5 - create a new group admin, try various passwords that satisfy the defined criteria.
                usermanagement.CreateGroup(DomainAdmin, GroupName, "Pass@123", DefaultRoleName);
                PageLoadWait.WaitForFrameLoad(20);
                if (usermanagement.IsGroupExist(GroupName))
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

                //Step 6 - Create a subgroup from the new group in the domain,  create a new group admin, try various passwords that satisfy the defined criteria.
                usermanagement.CreateSubGroup(GroupName, SubGroupName);
                if (usermanagement.IsGroupExist(SubGroupName))
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

                //Step 7 - Logout from ICA
                login.Logout();
                ExecutedSteps++;

                //Return Result
                result.FinalResult(ExecutedSteps);

                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

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
        /// Password Policy Applied - Update password for Standard, System and Domain Admin users for the above password policy
        /// </summary>
        public TestCaseResult Test9_29375(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables                
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;

                String StdUser = "User1" + new Random().Next(1, 10000);
                String SysAdmin = "SysAdmin1" + new Random().Next(1, 10000);
                String DomainAdmin = "DomainAdmin1" + new Random().Next(1, 10000);

                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");

                //Step 1 - Login iCA as Administrator and navigate to User Administrator page 
                login.LoginIConnect(Username, Password);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step 2 - Create standard user, system admin user and domain users
                usermanagement.CreateUser(StdUser, DefaultDomain, DefaultRoleName, 1, Email, 1, "Pass@123");
                usermanagement.CreateSystemAdminUser(SysAdmin, DefaultDomain, 0, "", 1, "Pass@123");
                usermanagement.CreateDomainAdminUser(DomainAdmin, DefaultDomain, 1, "", 1, "Pass@123");
                ExecutedSteps++;

                //Step 3 - From User Management *^-^* Edit User page, update user's password which matches with all criteria set in Service Tool.
                if (usermanagement.IsUserExist(StdUser, DefaultDomain))
                {
                    usermanagement.SelectUser(StdUser);
                    usermanagement.ClickButtonInUser("edit");
                    usermanagement.UpdatePassword("Pass@1234");

                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 4 - From User Management *^-^* Edit Doman Admin page, update a site admin's password which matches with all criteria set in Service Tool.
                if (usermanagement.IsUserExist(DomainAdmin, DefaultDomain))
                {
                    usermanagement.SelectUser(DomainAdmin);
                    usermanagement.ClickButtonInUser("edit");
                    usermanagement.UpdatePassword("Pass@1234");

                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 5 - From User Management *^-^* Edit System Admin page, update a system admin's password which matches with all criteria set in Service Tool.
                if (usermanagement.IsUserExist(SysAdmin, DefaultDomain))
                {
                    usermanagement.SelectUser(SysAdmin);
                    usermanagement.ClickButtonInUser("edit");
                    usermanagement.UpdatePassword("Pass@1234");

                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 6 - Logout
                login.Logout();
                ExecutedSteps++;

                //Step 7 - Login back in iCA with a newly created User
                login.LoginIConnect(StdUser, "Pass@1234");
                ExecutedSteps++;

                //Step 8 -  Go to Options, select My Profile
                MyProfilePage.OpenMyProfile();
                Thread.Sleep(2000);
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step 9 - click on the icon beside Password field
                //MyProfilePage.PwdRequirementIcon().Click();
                MyProfilePage.ClickButton("#PwdRequirementIcon");
                if (usermanagement.PasswordCriteriaText().Equals("Between: 8 - 14 characters Uppercase characters Lowercase characters Digits Special characters e.g. !, $, #, or %"))
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
                //MyProfilePage.XIcon().Click();

                //Step 10 - Update password from User Home *^-^* Update Profile page.
                MyProfilePage.ChangePassword("Pass@1234");
                ExecutedSteps++;

                //Step 11 - Logout
                login.Logout();
                ExecutedSteps++;

                //Return Result
                result.FinalResult(ExecutedSteps);

                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

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
        /// Password Policy Applied - Self Enrollment
        /// </summary>
        public TestCaseResult Test10_29375(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables                
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;

                String GroupName = "Group1" + new Random().Next(1, 10000);
                String EUser1 = "EnrollUser1" + new Random().Next(1, 10000);

                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");

                //Step 1 - Launch Service Tool Application
                servicetool.LaunchServiceTool();
                ExecutedSteps++;

                //Step 2 - Navigate to 'Enable Features' tab
                servicetool.NavigateToEnableFeatures();
                Thread.Sleep(2500);
                ExecutedSteps++;

                //Step 3 - Click on 'Modify' button
                servicetool.ModifyEnableFeatures();
                ExecutedSteps++;

                //Step 4 - Select 'Enable Self Enrollment' checkbox
                servicetool.EnableSelfEnrollment();
                servicetool.EnableEmailStudy();
                ExecutedSteps++;

                //Step 5 - Apply changes and IIS reset.
                servicetool.ApplyEnableFeatures();
                Thread.Sleep(2000);
                wpfobject.ClickOkPopUp();
                Thread.Sleep(2000);
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Step 6 - Login to ICA as Administrator and Navigate to User Management Tab
                login.LoginIConnect(Username, Password);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step 7 - Select 'SupuerAdminGroup' domain and create a new group                
                if (!usermanagement.IsGroupExist(GroupName))
                {
                    usermanagement.CreateGroup(DefaultDomain, GroupName, "Pass@123", DefaultRoleName);
                    PageLoadWait.WaitForFrameLoad(20);
                }
                ExecutedSteps++;

                //Step 8 - Logout from ICA
                login.Logout();
                ExecutedSteps++;

                //Step 9 - Navigate to ICA
                login.DriverGoTo(login.url);
                PageLoadWait.WaitForPageLoad(20);
                ExecutedSteps++;

                //Step 10 - Click on Register link in login page and fill details in enrollment form and submit a request for a new account.
                login.RegisterLink().Click();
                EnrollUser.EnrollUser(EUser1, DefaultDomain, GroupName, EUser1, EUser1, "shikander.raja@aspiresys.com");
                ExecutedSteps++;

                //Step 11 - Login as administrator, go to User Management\Request and approve the request
                login.LoginIConnect(Username, Password);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForFrameLoad(20);
                usermanagement.NavigateToRequestsUserManagementTab();
                PageLoadWait.WaitForFrameLoad(20);
                usermanagement.SelectEnrollUser();
                usermanagement.RegisterBtn().Click();
                Thread.Sleep(1000);
                usermanagement.SwitchToDefault();
                usermanagement.SwitchTo("index", "0");
                usermanagement.PasswordTextBox().SendKeys("Merge@11");
                usermanagement.ConfirmPwdTextBox().SendKeys("Merge@11");
                usermanagement.SaveBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                ExecutedSteps++;

                //Step 12 - Invitation a new user using a valid email address
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 13 - Enter password, Save.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 14 - Login ICA as the newly created user with invalid password
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 15 - Logout.<br/-Login iCA as the newly created users with valid password.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Return Result
                result.FinalResult(ExecutedSteps);

                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

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
        /// Password Policy Applied -  Invalid Password List - default values
        /// </summary>
        public TestCaseResult Test11_29375(String testid, String teststeps, int stepcount)
        {
            //Fetch the data
            String Contactinfo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ContactInformation");
            String[] info = Contactinfo.Split('=');
            String InvalidPasswordList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "InvalidPasswordList");

            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                //Step 1 - Launch Service Tool Application
                servicetool.LaunchServiceTool();
                ExecutedSteps++;

                //Step 2 - Navigate to Security Tab -> Password Policy sub-tab
                servicetool.NavigateToConfigToolSecurityTab();
                wpfobject.GetTabWpf(1).SelectTabPage(1);
                Thread.Sleep(2500);
                ExecutedSteps++;

                //Step 3 - Click on 'Modify' button
                servicetool.ClickModifyFromTab();
                ExecutedSteps++;

                //Step 4 - Set the password policy to: Minimum Length: 8, Maximum Length: 14, Invalid Password List: empty
                servicetool.SetPassWordPolicy(true);
                servicetool.SetMinPasswordLength(8);
                servicetool.SetMaxPasswordLength(14);
                servicetool.SetPasswordCriteriaCount("4");
                servicetool.SetPasswordCriteria("lowercase");
                servicetool.SetPasswordCriteria("uppercase");
                servicetool.SetPasswordCriteria("digits");
                servicetool.SetPasswordCriteria("specialchars");
                servicetool.UpdateAdminContact(info[0] + Environment.NewLine + info[1] + Environment.NewLine + info[2]);
                servicetool.UpdateInvalidPasswordList("removeall", new string[] { });
                servicetool.UpdateInvalidPasswordList("add", new string[] { InvalidPasswordList });
                ExecutedSteps++;

                //Step 5 - Apply changes and IIS reset.
                servicetool.ClickApplyButtonFromTab();
                servicetool.AcceptDialogWindow();
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Return Result
                result.FinalResult(ExecutedSteps);

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
        /// Password Policy Applied - Create new user for the above password policy
        /// </summary>
        public TestCaseResult Test12_29375(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables          
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;
                String User1 = "User1" + new Random().Next(1, 10000);
                String User2 = "User2" + new Random().Next(1, 10000);
                String User3 = "User3" + new Random().Next(1, 10000);
                String User4 = "User4" + new Random().Next(1, 10000);
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");

                //Step 1 - Login iCA as Administrator and navigate to User Management page 
                login.LoginIConnect(Username, Password);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(20);
                ExecutedSteps++;

                //Step 3 - Click the icon beside the password field.
                usermanagement.ClickNewUser();
                PageLoadWait.WaitForFrameLoad(20);
                usermanagement.ClickButton("#PwdRequirementIcon");
                if (usermanagement.PasswordCriteriaText().Equals("Between: 8 - 14 characters Uppercase characters Lowercase characters Digits Special characters e.g. !, $, #, or %"))
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
                usermanagement.ClickButton("#NewUesrDialogDiv > div.titlebar > span");

                //Step 2 - Create a user, enter password which matches all the criteria set in Service Tool except it does contain the Access User ID.      
                usermanagement.CreateUser(User1, DefaultDomain, DefaultRoleName, 1, Email, 1, "Cedara1", 1);
                if (usermanagement.NewUsrErrMsg().Text.Equals("The password you have entered does not meet minimum requirements."))
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
                usermanagement.ClickButton("#NewUesrDialogDiv > div.titlebar > span");

                //Step 4 - Enter password that matches all the criteria set in Service Tool (e.g. User ID- Jane, Password- 123J@ne%)
                usermanagement.CreateUser(User2, DefaultDomain, DefaultRoleName, 1, Email, 1, "123J@ne%");
                if (usermanagement.IsUserExist(User2, DefaultDomain))
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

                //Step 5 - Enter password which matches all the criteria set in Service Tool except it does contain any of  listed in the Invalid Password list
                usermanagement.CreateUser(User3, DefaultDomain, DefaultRoleName, 1, Email, 1, "Pa$$w0rd", 1);
                if (usermanagement.NewUsrErrMsg().Text.Equals("The password typed in is considered less secure and is prohibited. Please choose a stronger password."))
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
                usermanagement.ClickButton("#NewUesrDialogDiv > div.titlebar > span");

                //Step 6 - Attempt to enter password which matches all the criteria set in Service Tool except it contains one of these Invalid values
                usermanagement.CreateUser(User4, DefaultDomain, DefaultRoleName, 1, Email, 1, "Cedara1!", 1);
                if (usermanagement.NewUsrErrMsg().Text.Equals("The password typed in is considered less secure and is prohibited. Please choose a stronger password."))
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
                usermanagement.ClickButton("#NewUesrDialogDiv > div.titlebar > span");
                login.Logout();

                //Step 7 - Login ICA as the newly created user with invalid password
                login.LoginIConnect(User2, "Pa33word");
                PageLoadWait.WaitForPageLoad(20);
                if (login.LoginErrorMsgLabel().Text.Equals("The system cannot log you in. Please try again.:"))
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

                //Step 8 - Login iCA as the newly created users with valid password.
                login.LoginIConnect(User2, "123J@ne%");
                ExecutedSteps++;

                //Logout
                login.Logout();

                //Return Result
                result.FinalResult(ExecutedSteps);

                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

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
        /// Password Policy Applied - Invalid Password List- default values - add more invalid values, e.g., @Oct202014, 20Oct@2014
        /// </summary>
        public TestCaseResult Test13_29375(String testid, String teststeps, int stepcount)
        {
            //Fetch the data
            String Contactinfo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ContactInformation");
            String[] info = Contactinfo.Split('=');
            String InvalidPasswordList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "InvalidPasswordList");

            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                //Step 1 - Launch Service Tool Application
                servicetool.LaunchServiceTool();
                ExecutedSteps++;

                //Step 2 - Navigate to Security Tab -> Password Policy sub-tab
                servicetool.NavigateToConfigToolSecurityTab();
                wpfobject.GetTabWpf(1).SelectTabPage(1);
                Thread.Sleep(2500);
                ExecutedSteps++;

                //Step 3 - Click on 'Modify' button
                servicetool.ClickModifyFromTab();
                ExecutedSteps++;

                //Step 4 - Set the password policy to: Minimum Length: 8, Maximum Length: 14, Invalid Password List: empty
                servicetool.SetPassWordPolicy(true);
                servicetool.SetMinPasswordLength(8);
                servicetool.SetMaxPasswordLength(14);
                servicetool.SetPasswordCriteriaCount("4");
                servicetool.SetPasswordCriteria("lowercase");
                servicetool.SetPasswordCriteria("uppercase");
                servicetool.SetPasswordCriteria("digits");
                servicetool.SetPasswordCriteria("specialchars");
                servicetool.UpdateAdminContact(info[0] + Environment.NewLine + info[1] + Environment.NewLine + info[2]);
                servicetool.UpdateInvalidPasswordList("append", new string[] { InvalidPasswordList });
                ExecutedSteps++;

                //Step 5 - Apply changes and IIS reset.
                servicetool.ClickApplyButtonFromTab();
                servicetool.AcceptDialogWindow();
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Return Result
                result.FinalResult(ExecutedSteps);

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
        /// Password Policy Applied - Create new user for the above password policy
        /// </summary>
        public TestCaseResult Test14_29375(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables          
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;
                String User1 = "User1" + new Random().Next(1, 10000);
                String User2 = "User2" + new Random().Next(1, 10000);
                String User3 = "User3" + new Random().Next(1, 10000);
                String User4 = "User4" + new Random().Next(1, 10000);
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");

                //Step 1 - Login iCA as Administrator and navigate to User Administrator page 
                login.LoginIConnect(Username, Password);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(20);
                ExecutedSteps++;

                //Step 2 - Create a user, enter password which matches the criteria set in Service Tool except it does contain any of these newly added Prohibited passwords
                usermanagement.CreateUser(User1, DefaultDomain, DefaultRoleName, 1, Email, 1, "20Oct@2014", 1);
                //usermanagement.CreateUser(User1, DefaultDomain, DefaultRoleName, 1, Email, 1, "Cedara1");
                if (usermanagement.NewUsrErrMsg().Text.Equals("The password typed in is considered less secure and is prohibited. Please choose a stronger password."))
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
                usermanagement.ClickButton("#NewUesrDialogDiv > div.titlebar > span");

                //Step 3 - Repeat the step for all supported browsers       
                ExecutedSteps++;

                //Step 4 - Enter password that matches all the criteria set in Service Tool (e.g. User ID- Jane, Password- 123J@ne%)
                usermanagement.CreateUser(User2, DefaultDomain, DefaultRoleName, 1, Email, 1, "123J@ne%");
                if (usermanagement.IsUserExist(User2, DefaultDomain))
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

                //Step 5 - Logout. Login ICA as the newly created user with valid password. And then repeat this step with an invalid password.
                login.Logout();
                login.LoginIConnect(User2, "123J@ne%");
                login.Logout();
                login.LoginIConnect(User2, "@Oct202014");
                PageLoadWait.WaitForPageLoad(20);
                //login.LoginIConnect(User2, "Cedara1");
                if (login.LoginErrorMsgLabel().Text.Equals("The system cannot log you in. Please try again.:"))
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

                //Logout
                login.Logout();

                //Return Result
                result.FinalResult(ExecutedSteps);

                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Logout 
                login.Logout();

                //Return Result
                return result;
            }
        }

        ///// <summary>
        ///// Password Policy Applied - Initial stup
        ///// </summary>
        //public TestCaseResult Test9_29375(String testid, String teststeps, int stepcount)
        //{
        //    //Declare and initialize variables                
        //    TestCaseResult result;
        //    result = new TestCaseResult(stepcount);
        //    int ExecutedSteps = -1;

        //    //Set up Validation Steps
        //    result.SetTestStepDescription(teststeps);

        //    try
        //    {
        //        //Fetch required Test data
        //        String Username = Config.adminUserName;
        //        String Password = Config.adminPassword;

        //        String StdUser = "User1" + new Random().Next(1, 10000);
        //        String SysAdmin = "SysAdmin1" + new Random().Next(1, 10000);
        //        String DomainAdmin = "DomainAdmin1" + new Random().Next(1, 10000);

        //        String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
        //        String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
        //        String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");

        //        //Step 1 - Login iCA as Administrator and navigate to User Administrator page 
        //        login.LoginIConnect(Username, Password);
        //        usermanagement = (UserManagement)login.Navigate("UserManagement");
        //        PageLoadWait.WaitForFrameLoad(20);
        //        ExecutedSteps++;

        //        //Step 2 - Create standard user, system admin user and domain users
        //        usermanagement.CreateUser(StdUser, DefaultDomain, DefaultRoleName, 1, Email, 1, "Pass@123");
        //        usermanagement.CreateSystemAdministratorUser(SysAdmin, DefaultDomain, 0, "", 1, "Pass@123");
        //        usermanagement.CreateDomainAdministratorUser(DomainAdmin, DefaultDomain, 1, "", 1, "Pass@123");
        //        ExecutedSteps++;

        //        //Step 3 - From User Management *^-^* Edit User page, update user's password which matches with all criteria set in Service Tool.
        //        if (usermanagement.IsUserExist(StdUser, DefaultDomain))
        //        {
        //            usermanagement.SelectUser(StdUser);
        //            usermanagement.ClickButtonInUser("edit");
        //            usermanagement.UpdatePassword("Pass@1234");

        //            result.steps[++ExecutedSteps].status = "Pass";
        //            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
        //        }
        //        else
        //        {
        //            result.steps[++ExecutedSteps].status = "Fail";
        //            Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
        //            result.steps[ExecutedSteps].SetLogs();
        //        }

        //        //Step 4 - From User Management *^-^* Edit Doman Admin page, update a site admin's password which matches with all criteria set in Service Tool.
        //        if (usermanagement.IsUserExist(DomainAdmin, DefaultDomain))
        //        {
        //            usermanagement.SelectUser(DomainAdmin);
        //            usermanagement.ClickButtonInUser("edit");
        //            usermanagement.UpdatePassword("Pass@1234");

        //            result.steps[++ExecutedSteps].status = "Pass";
        //            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
        //        }
        //        else
        //        {
        //            result.steps[++ExecutedSteps].status = "Fail";
        //            Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
        //            result.steps[ExecutedSteps].SetLogs();
        //        }

        //        //Step 5 - From User Management *^-^* Edit System Admin page, update a system admin's password which matches with all criteria set in Service Tool.
        //        if (usermanagement.IsUserExist(SysAdmin, DefaultDomain))
        //        {
        //            usermanagement.SelectUser(SysAdmin);
        //            usermanagement.ClickButtonInUser("edit");
        //            usermanagement.UpdatePassword("Pass@1234");

        //            result.steps[++ExecutedSteps].status = "Pass";
        //            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
        //        }
        //        else
        //        {
        //            result.steps[++ExecutedSteps].status = "Fail";
        //            Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
        //            result.steps[ExecutedSteps].SetLogs();
        //        }

        //        //Step 6 - Logout
        //        login.Logout();
        //        ExecutedSteps++;

        //        //Step 7 - Login back in iCA with a newly created User
        //        login.LoginIConnect(StdUser, "Pass@1234");

        //        //Step 8 -  Go to Options, select My Profile
        //        MyProfilePage.OpenMyProfile();
        //        Thread.Sleep(2000);
        //        PageLoadWait.WaitForFrameLoad(20);

        //        //Step 9 - click on the icon beside Password field
        //        MyProfilePage.PwdRequirementIcon().Click();
        //        if (usermanagement.PasswordCriteriaText().Equals("Between: 8 - 14 characters Uppercase characters Lowercase characters Digits Special characters e.g. !, $, #, or %"))
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
        //        //MyProfilePage.XIcon().Click();

        //        //Step 10 - Update password from User Home *^-^* Update Profile page.
        //        MyProfilePage.ChangePassword("Pass@1234");
        //        ExecutedSteps++;

        //        //Step 11 - Logout
        //        login.Logout();
        //        ExecutedSteps++;

        //        //Return Result
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        //Log Exception
        //        Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

        //        //Report Result
        //        result.FinalResult(e, ExecutedSteps);
        //        Logger.Instance.InfoLog("Overall Test status--" + result.status);

        //        //Logout 
        //        login.Logout();

        //        //Return Result
        //        return result;
        //    }
        //}

        /// <summary>
        /// Configuration - forgot password
        /// </summary>
        public TestCaseResult Test_29376(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables          
            ServiceTool servicetool;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;


            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch the data
                String Contactinfo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ContactInformation");
                String[] info = Contactinfo.Split('=');

                //PreCondition
                //Step-1:Perform Initial setup-->Local database is used and LDAP database is not used,Atleast 1 datasource is added,Enable Password policy is not selected
                servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.Security_Tab);
                servicetool.NavigateSubTab(ServiceTool.Security.Name.PasswordPolicy_tab);
                servicetool.ClickModifyFromTab();
                if (wpfobject.GetAnyUIItem<Window, CheckBox>(WpfObjects._mainWindow, ServiceTool.Security.Name.EnablePasswordPolicy, 1).Checked)
                {
                    servicetool.SetPassWordPolicy(false);
                    servicetool.ClickApplyButtonFromTab();
                    servicetool.AcceptDialogWindow();
                }
                else
                {
                    servicetool.ClickApplyButtonFromTab();
                }
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Step-2:Verify Forgot Password button in iCA screen
                login.DriverGoTo(login.url);

                IWebElement forgotpasswordbtn = BasePage.Driver.FindElement(By.CssSelector("input[id$='_ForgotPasswordButton']"));
                if (forgotpasswordbtn.Enabled == true)
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

                forgotpasswordbtn.Click();
                IWebElement infomsg = BasePage.Driver.FindElement(By.CssSelector("div#AdminContactDiv span"));
                if (infomsg.Displayed == true)
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

                //Step-3:Set the contact information in the service tool
                servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolSecurityTab();
                wpfobject.GetTabWpf(1).SelectTabPage(1);
                Thread.Sleep(2500);
                servicetool.ClickModifyFromTab();
                servicetool.SetPassWordPolicy(true);
                servicetool.UpdateAdminContact(info[0] + Environment.NewLine + info[1] + Environment.NewLine + info[2]);
                servicetool.ClickApplyButtonFromTab();
                servicetool.AcceptDialogWindow();
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Stpe-4:Verify the message that is given in the service tool
                login.DriverGoTo(login.url);

                IWebElement forgotpasswordbtn0 = BasePage.Driver.FindElement(By.CssSelector("input[id$='_ForgotPasswordButton']"));
                forgotpasswordbtn0.Click();
                String infomsg0 = BasePage.Driver.FindElement(By.CssSelector("div#AdminContactDiv span")).GetAttribute("innerHTML");
                String contactinfo = BasePage.Driver.FindElement(By.CssSelector("div#AdminContactDiv span[id$='_AdminContact']")).GetAttribute("innerHTML");
                if (infomsg0.Equals("Please contact Administrator:") && contactinfo.Contains(info[0]) && contactinfo.Contains(info[1]) && contactinfo.Contains(info[2]))
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

                //**Step-5:Follow all the above steps in other supporting browsers
                ExecutedSteps++;

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Logout 
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Configuration - Password Policy GUI
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_29377(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables        

            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            ServiceTool servicetool = new ServiceTool();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch the data
                String Contactinfo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ContactInformation");
                String InvalidPwd = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "InvalidPasswordList");
                String[] info = Contactinfo.Split('=');
                String[] InvalidPassword = InvalidPwd.Split('=');

                //***Step-1:Perform Initial setup-->Local database is used and LDAP database is not used,Atleast 1 datasource is added,Enable Password policy is not selected
                ExecutedSteps++;

                //***Step-2:Verify Config options are displayed for Strong password
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolSecurityTab();
                wpfobject.GetTabWpf(1).SelectTabPage(1);
                Thread.Sleep(2500);
                //servicetool.ClickModifyFromTab();
                wpfobject.ClickButton("Modify", 1);
                TextBox admintext = wpfobject.GetTextbox("TB_AdminContact");
                CheckBox cb = wpfobject.GetAnyUIItem<Window, CheckBox>(WpfObjects._mainWindow, "Enable Password Policy", 1);
                ITabPage currenttab = WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;
                GroupBox group = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(currenttab, "Password Policy", 1);
                TextBox minlen = wpfobject.GetUIItem<GroupBox, TextBox>(group, "AutoSelectTextBox", itemsequnce: "0");
                TextBox maxlen = wpfobject.GetUIItem<GroupBox, TextBox>(group, "AutoSelectTextBox", itemsequnce: "1");
                TextBox criteria = wpfobject.GetUIItem<GroupBox, TextBox>(group, "AutoSelectTextBox", itemsequnce: "2");
                TextBox invalidPwd = wpfobject.GetUIItem<GroupBox, TextBox>(group);
                CheckBox cb1 = wpfobject.GetAnyUIItem<GroupBox, CheckBox>(group, "CB_SpecialCharacters");
                CheckBox cb2 = wpfobject.GetAnyUIItem<GroupBox, CheckBox>(group, "CB_Digits0to9");
                CheckBox cb3 = wpfobject.GetAnyUIItem<GroupBox, CheckBox>(group, "CB_LowercaseCharacters");
                CheckBox cb4 = wpfobject.GetAnyUIItem<GroupBox, CheckBox>(group, "CB_UppercaseCharacters");
                Button cancelbtn = wpfobject.GetButton("Cancel", 1);
                Button applybtn = wpfobject.GetButton("Apply", 1);
                servicetool.EditAdminContact("");
                servicetool.EditInvalidPasswordList("");
                servicetool.UpdateInvalidPasswordList("add", new string[] { InvalidPassword[0] });
                servicetool.SetPassWordPolicy(false);
                servicetool.SetMinPasswordLength(1);
                servicetool.SetMaxPasswordLength(100);
                servicetool.SetPasswordCriteriaCount("0");
                servicetool.UncheckAllPasswordPreferences();
                servicetool.ClickApplyButtonFromTab();
                servicetool.AcceptDialogWindow();
                servicetool.RestartService();
                servicetool.ClickModifyFromTab();

                //var name = _mainWindow.Get>(SearchCriteria.ByAutomationId("8029")).GetType().Name;
                if (admintext.Text == "" && !(cb.Checked) && minlen.Text.Equals("1") && maxlen.Text.Equals("100") &&
                    criteria.Text.Equals("0") && invalidPwd.Text == InvalidPassword[0]
                    && cb1.Visible && cb2.Visible && cb3.Visible && cb4.Visible && cancelbtn.Visible && applybtn.Visible)
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


                //Step-3:Verify Forgot Password button is in iCA screen
                login.DriverGoTo(login.url);

                IWebElement forgotpasswordbtn = BasePage.Driver.FindElement(By.CssSelector("input[id$='_ForgotPasswordButton']"));
                if (forgotpasswordbtn.Enabled == true)
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

                //***Step-4:Contact info is set as not more than 5 lines in ServiceTool
                servicetool.wpfobject.ClickButton("Modify",1);
                servicetool.UpdateAdminContact(info[0] + Environment.NewLine + info[1] + Environment.NewLine + info[2] + Environment.NewLine + info[3] + Environment.NewLine + info[4]);
                servicetool.ClickApplyButtonFromTab();
                servicetool.AcceptDialogWindow();
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                Thread.Sleep(4500);
                ExecutedSteps++;

                //Step-5:Click Forgot Password Button and verify the info displayed
                login.DriverGoTo(login.url);
                PageLoadWait.WaitForPageLoad(20);
                IWebElement forgotpasswordbtn0 = BasePage.Driver.FindElement(By.CssSelector("input[id$='_ForgotPasswordButton']"));
                //forgotpasswordbtn0.Click();
                //added java script click IE-8
                login.ClickElement(forgotpasswordbtn0);
                
                IWebElement admincontact = BasePage.Driver.FindElement(By.CssSelector("span#ctl00_LoginMasterContentPlaceHolder_AdminContact"));
                String infomsg0 = BasePage.Driver.FindElement(By.CssSelector("div#AdminContactDiv span")).GetAttribute("innerHTML");
                if (infomsg0.Equals("Please contact Administrator:") && admincontact.Text.Contains(info[0]) && admincontact.Text.Contains(info[1]) && admincontact.Text.Contains(info[2]) &&
                    admincontact.Text.Contains(info[3]) && admincontact.Text.Contains(info[4]))
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

                //***Step-6:Contact info is set as more than 6 lines and one one of the lines with more than 60 characters in ServiceTool
                servicetool.ClickModifyFromTab();
                //TextBox admintext1 = wpfobject.GetTextbox("textBox_adminContact");
                servicetool.UpdateAdminContact(info[0] + Environment.NewLine + info[1] + Environment.NewLine + info[2] + Environment.NewLine + info[3] +
                Environment.NewLine + info[4] + Environment.NewLine + info[5]);
                servicetool.ClickApplyButtonFromTab();
                Button okbtn = wpfobject.GetButton("2");
                string dialog = servicetool.WarningDialogWindow();
                if (dialog == "Admin contact can not have more than 5 lines.")
                {
                    //admintext1.Text.Remove(0);
                    servicetool.ClickCancelButtonFromTab();
                    servicetool.ClickModifyFromTab();
                    servicetool.UpdateAdminContact(info[0] + Environment.NewLine + info[1] + Environment.NewLine + info[2] + Environment.NewLine + info[3]);
                    servicetool.ClickApplyButtonFromTab();
                    //servicetool.AcceptDialogWindow();
                    wpfobject.ClickButton("6");
                    servicetool.RestartService();
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7:Click Forgot Password Button and verify the info displayed
                login.DriverGoTo(login.url);
                PageLoadWait.WaitForPageLoad(20);
                IWebElement forgotpasswordbtn1 = BasePage.Driver.FindElement(By.CssSelector("input[id$='_ForgotPasswordButton']"));
                //forgotpasswordbtn1.Click();
                login.ClickElement(forgotpasswordbtn1);

                String infomsg1 = BasePage.Driver.FindElement(By.CssSelector("div#AdminContactDiv span[id$='_AdminContactMessage']")).GetAttribute("innerHTML");
                IWebElement admincontact1 = BasePage.Driver.FindElement(By.CssSelector("div#AdminContactDiv span[id$='_AdminContact']"));
                if (infomsg1.Equals("Please contact Administrator:") && admincontact1.Text.Contains(info[0]) && admincontact1.Text.Contains(info[1]) && admincontact1.Text.Contains(info[2]) &&
                   admincontact1.Text.Contains(info[3]))
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

                //***Step-8:Contact info is set as 90 characters in a line in ServiceTool
                //servicetool.ClickModifyFromTab();
                //TextBox admintext3 = wpfobject.GetTextbox("textBox_adminContact");
                wpfobject.ClickButton("Modify", 1);
                servicetool.UpdateAdminContact(Environment.NewLine + info[5]);
                servicetool.ClickApplyButtonFromTab();
                Button okbtn1 = wpfobject.GetButton("2");
                string dialog1 = servicetool.WarningDialogWindow();
                if (dialog1 == "Each line for Admin contact has max character limit of 60.")
                {
                    servicetool.ClickCancelButtonFromTab();
                    servicetool.ClickModifyFromTab();
                    servicetool.UpdateAdminContact(info[0] + Environment.NewLine + info[1] + Environment.NewLine + info[2] + Environment.NewLine + info[3] + Environment.NewLine + info[4]);
                    //servicetool.ClickApplyButtonFromTab();
                    //servicetool.AcceptDialogWindow();
                    wpfobject.ClickButton("Apply", 1);
                    wpfobject.ClickButton("6");
                    servicetool.RestartService();
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-9:Click Forgot Password Button and verify the info displayed
                login.DriverGoTo(login.url);
                IWebElement forgotpasswordbtn2 = BasePage.Driver.FindElement(By.CssSelector("input[id$='_ForgotPasswordButton']"));
                //forgotpasswordbtn2.Click();
                login.ClickElement(forgotpasswordbtn2);

                String infomsg2 = BasePage.Driver.FindElement(By.CssSelector("div#AdminContactDiv span[id$='_AdminContactMessage']")).GetAttribute("innerHTML");
                IWebElement admincontact2 = BasePage.Driver.FindElement(By.CssSelector("div#AdminContactDiv span[id$='_AdminContact']"));
                if (infomsg2.Equals("Please contact Administrator:") && admincontact2.Text.Contains(info[0]) && admincontact2.Text.Contains(info[1]) && admincontact2.Text.Contains(info[2]) &&
                   admincontact2.Text.Contains(info[3]) && admincontact2.Text.Contains(info[4]))
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

                //***Step-10:Validate Min and Max Length fields values are inc/dec by clicking Up/Down arrow
                //servicetool.ClickModifyFromTab();
                wpfobject.ClickButton("Modify", 1);
                int maxvalue = servicetool.GetMaxPasswordLength(99);
                int minvalue = servicetool.GetMinPasswordLength(2);
                int maxvalue1 = servicetool.GetMaxPasswordLength(100);
                int minvalue2 = servicetool.GetMinPasswordLength(1);
                servicetool.ClickApplyButtonFromTab();
                servicetool.AcceptDialogWindow();
                if (maxvalue == 99 && minvalue == 2 && maxvalue1 == 100 && minvalue2 == 1)
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


                //***Step-11:Enter Invalid values in Min and Max values and validate that service tool should not accept
                /*  servicetool.ClickModifyButton();
                  try
                  {
                      servicetool.SetSpinnerValue("minLength", 8.4);
                      wpfobject.WaitTillLoad();
                      wpfobject.WaitTillLoad();
                      Thread.Sleep(30000);
                      string dialog2 = servicetool.WarningDialogWindow();
                      if (dialog2 == "Minimum length should be a positive integer.")
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
                  catch(Exception e)
                  {
                      result.steps[++ExecutedSteps].status = "Fail";
                      Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                      result.steps[ExecutedSteps].SetLogs();
                  }*/
                result.steps[++ExecutedSteps].status = "Not Automated";


                //***Step-12:Enter Valid values in these fields and apply changes
                servicetool.ClickModifyFromTab();
                servicetool.SetSpinnerValue("minLength", 3);
                servicetool.ClickApplyButtonFromTab();
                servicetool.AcceptDialogWindow();
                ExecutedSteps++;

                //***Step-13:Click Up/Down arrow in the Password should contain [0]
                servicetool.ClickModifyFromTab();
                int count = servicetool.GetPreferenceCount(1);
                int count1 = servicetool.GetPreferenceCount(0);
                if (count == 1 && count1 == 0)
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

                //***Step-14:Enter Invalid values in Password field and validate that service tool should not accept
                ITabPage currenttab14 = WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;
                GroupBox group14 = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(currenttab14, "Password Policy", 1);
                TextBox criteria14 = wpfobject.GetUIItem<GroupBox, TextBox>(group14, "AutoSelectTextBox", itemsequnce: "2");
                criteria14.Text = "5";
                servicetool.ClickApplyButtonFromTab();
                string dialog3 = servicetool.WarningDialogWindow();
                if (dialog3 == "Minimum 4 password categories need to be selected.")
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

                //***Step-15:Verify the consistency between the number entered and number selected categories and validate whether the warning message displays
                servicetool.SetPasswordCriteriaCount("0");
                servicetool.SetPreferenceCount(2);
                servicetool.SetAllPasswordPreferences();
                ITabPage currenttab15 = WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;
                GroupBox group15 = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(currenttab15, "Password Policy", 1);
                CheckBox uppercase = wpfobject.GetAnyUIItem<GroupBox, CheckBox>(group15, "CB_UppercaseCharacters");
                CheckBox lowercase = wpfobject.GetAnyUIItem<GroupBox, CheckBox>(group15, "CB_LowercaseCharacters");
                CheckBox digits = wpfobject.GetAnyUIItem<GroupBox, CheckBox>(group15, "CB_Digits0to9");
                CheckBox specialchars = wpfobject.GetAnyUIItem<GroupBox, CheckBox>(group15, "CB_SpecialCharacters");
                List<CheckBox> CheckBoxs = new List<CheckBox>();
                int cbcount = 0;
                CheckBoxs.Add(uppercase); CheckBoxs.Add(lowercase); CheckBoxs.Add(specialchars); CheckBoxs.Add(digits);
                foreach (CheckBox chk in CheckBoxs)
                {
                    if (chk.Checked == true)
                    {
                        cbcount++;
                    }
                }

                servicetool.ClickApplyButtonFromTab();
                if (cbcount > 2)
                {
                    servicetool.AcceptDialogWindow();
                    ExecutedSteps++;
                }
                else
                {
                    string dialog4 = servicetool.WarningDialogWindow();
                    if (dialog4 == "Minimum 2 password categories need to be selected.")
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

                //***Step-16:Enable Password Policy,Min-3,Max-8,Password contains [0],all 4 categories must be selected
                servicetool.ClickModifyFromTab();
                servicetool.SetPassWordPolicy(true);
                servicetool.SetMinPasswordLength(3);
                servicetool.SetSpinnerValue("maxLength", 8);
                servicetool.SetPreferenceCount(0);
                servicetool.SetAllPasswordPreferences();
                servicetool.ClickApplyButtonFromTab();
                servicetool.AcceptDialogWindow();
                servicetool.RestartService();
                ExecutedSteps++;

                //***Step-17:Enable Password Policy,Min-1,Max-100,Password contains [4],all 4 categories must be selected
                servicetool.ClickModifyFromTab();
                servicetool.SetPassWordPolicy(true);
                servicetool.SetMinPasswordLength(1);
                servicetool.SetSpinnerValue("maxLength", 100);
                servicetool.SetPreferenceCount(4);
                servicetool.SetAllPasswordPreferences();
                servicetool.ClickApplyButtonFromTab();
                servicetool.AcceptDialogWindow();
                servicetool.RestartService();
                ExecutedSteps++;

                //***Step-18:Perform Boundary validation in the 'Invalid Password List'
                servicetool.ClickModifyFromTab();
                ITabPage currenttab18 = WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;
                GroupBox group18 = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(currenttab18, "Password Policy", 1);
                TextBox element5 = wpfobject.GetUIItem<GroupBox, TextBox>(group18);
                count = 0;
                string text = "";
                for (int i = 0; i < 4681; i++)
                {
                    text = text + InvalidPassword[1];
                }
                element5.Text = text;
                servicetool.ClickApplyButtonFromTab();
                servicetool.AcceptDialogWindow();
                ExecutedSteps++;

                servicetool.CloseServiceTool();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
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
                /*Setting the Default Setting*/
                servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.Security_Tab);
                servicetool.NavigateSubTab(ServiceTool.Security.Name.PasswordPolicy_tab);
                servicetool.ClickModifyFromTab();
                if (wpfobject.GetAnyUIItem<Window, CheckBox>(WpfObjects._mainWindow, ServiceTool.Security.Name.EnablePasswordPolicy, 1).Checked)
                {
                    servicetool.SetPassWordPolicy(false);
                    servicetool.ClickApplyButtonFromTab();
                    servicetool.AcceptDialogWindow();
                }
                else
                {
                    servicetool.ClickApplyButtonFromTab();
                }
                servicetool.RestartService();
                servicetool.CloseServiceTool();
            }
        }
    }
}
