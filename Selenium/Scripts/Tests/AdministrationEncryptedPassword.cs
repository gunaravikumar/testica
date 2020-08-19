using System;
using System.Globalization;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Reusable.Generic;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
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
using ComboBox = TestStack.White.UIItems.ListBoxItems.ComboBox;

namespace Selenium.Scripts.Tests
{
    class AdministrationEncryptedPassword : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }

        Studies studies = new Studies();
        ServiceTool servicetool = new ServiceTool();
        StudyViewer viewer;

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public AdministrationEncryptedPassword(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            wpfobject = new WpfObjects();
        }

        /// <summary>
        /// Encrypted Password - Amicas Datasource Password
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_162373(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            BasePage basepage = new BasePage();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data      
                String AmicasUserNames = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UserName");
                String AmicasPasswords = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Password");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");

                String[] AmicasUserName = AmicasUserNames.Split(':');
                String[] AmicasPassword = AmicasPasswords.Split(':');
                String[] Accessions = AccessionList.Split(':');

                string DataSourceManagerConfig = @"C:\WebAccess\WebAccess\Config\DataSource\DataSourceManagerConfiguration.xml";
                string EncryptedPassword = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "EncryptedPassword");

                new Taskbar().Hide();

                //Step 1 - Launch IBM iConnect Access Service Tool.                
                servicetool.LaunchServiceTool();
                ExecutedSteps++;

                //Step 2 - Navigate to Datasource tab and Click on Add.
                servicetool.NavigateToConfigToolDataSourceTab();
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step 3 - Configure Amicas Datasource in service tool and Enter the Password for Amicas datasource and verify the entered password.                
                GroupBox datasource_grp = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(servicetool.GetCurrentTabItem(), ServiceTool.DataSource.Name.DataSourceList_grp, 1);
                ListView datasource_view = wpfobject.GetAnyUIItem<GroupBox, ListView>(datasource_grp, "ListView");
                foreach (var row in datasource_view.Rows)
                {
                    if (row.Cells[0].Text.Equals(login.GetHostName(Config.SanityPACS)))
                    {
                        row.Focus();
                        row.Click();
                        wpfobject.WaitTillLoad();
                        row.Cells[0].DoubleClick();
                        wpfobject.WaitTillLoad();
                        break;
                    }
                }
                wpfobject.GetMainWindowByTitle(ServiceTool.DataSource.Name.EditDataSource_Window);
                servicetool.wpfobject.WaitTillLoad();
                wpfobject.SelectTabFromTabItems(ServiceTool.DataSource.Name.Amicas_Tab);
                Thread.Sleep(3000);
                wpfobject.ClearText(ServiceTool.DataSource.ID.AmicasUserName);
                wpfobject.SetText(ServiceTool.DataSource.ID.AmicasUserName, AmicasUserName[0]);
                Thread.Sleep(1500);
                wpfobject.ClearText(ServiceTool.DataSource.ID.AmicasPassword);
                wpfobject.SetText(ServiceTool.DataSource.ID.AmicasPassword, AmicasPassword[0]);
                Thread.Sleep(1500);
                ExecutedSteps++;

                //Step 4 - save the added amicas datasource and Restart the services.
                wpfobject.GetMainWindowByTitle(ServiceTool.DataSource.Name.EditDataSource_Window);
                wpfobject.ClickButton(ServiceTool.DataSource.ID.OkBtn);
                wpfobject.WaitTillLoad();
                Thread.Sleep(3000);
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseConfigTool();
                ExecutedSteps++;

                new Taskbar().Show();
                //Step 5 
                string EncryptedPasswordInConfig = basepage.GetNodeValue( DataSourceManagerConfig, "/dataSources/add[@id='" + basepage.GetHostName(Config.SanityPACS)+ "']/parameters/amicas.password");
                if (EncryptedPasswordInConfig == EncryptedPassword)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }


                //Step 6 - From client, launch iCA and login with valid credential.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                ExecutedSteps++;

                //Step 7 - Connect the added amicas datasource in domain management page and load the studies from added Amicas datasource.
                studies = login.Navigate<Studies>();
                studies.SearchStudy(AccessionNo: Accessions[0], Datasource: login.GetHostName(Config.SanityPACS));
                studies.SelectStudy("Accession", Accessions[0]);
                BluRingViewer Viewer = new BluRingViewer();
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {

                    BluRingViewer.LaunchBluRingViewer();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 2);
                    bool step_7 = studies.CompareImage(result.steps[ExecutedSteps], Viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));

                    if (step_7)
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
                    Viewer.CloseBluRingViewer();
                }
                else
                {
                    viewer = LaunchStudy();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    bool step_7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                    if (step_7)
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
                    studies.CloseStudy();
                }
                login.Logout();

                new Taskbar().Hide();
                //Step 8 - From iCA service tool, update the another valid username and password for added amicas datasource in Amicas tab.
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolDataSourceTab();
                wpfobject.WaitTillLoad();
                GroupBox datasource_grp1 = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(servicetool.GetCurrentTabItem(), ServiceTool.DataSource.Name.DataSourceList_grp, 1);
                ListView datasource_view1 = wpfobject.GetAnyUIItem<GroupBox, ListView>(datasource_grp1, "ListView");
                foreach (var row in datasource_view1.Rows)
                {
                    if (row.Cells[0].Text.Equals(login.GetHostName(Config.SanityPACS)))
                    {
                        row.Focus();
                        row.Click();
                        wpfobject.WaitTillLoad();
                        row.Cells[0].DoubleClick();
                        wpfobject.WaitTillLoad();
                        break;
                    }
                }
                wpfobject.GetMainWindowByTitle(ServiceTool.DataSource.Name.EditDataSource_Window);
                servicetool.wpfobject.WaitTillLoad();
                wpfobject.SelectTabFromTabItems(ServiceTool.DataSource.Name.Amicas_Tab);
                Thread.Sleep(3000);
                wpfobject.ClearText(ServiceTool.DataSource.ID.AmicasUserName);
                wpfobject.SetText(ServiceTool.DataSource.ID.AmicasUserName, AmicasUserName[1]);
                Thread.Sleep(1500);
                wpfobject.ClearText(ServiceTool.DataSource.ID.AmicasPassword);
                wpfobject.SetText(ServiceTool.DataSource.ID.AmicasPassword, AmicasPassword[1]);
                Thread.Sleep(1500);
                ExecutedSteps++;

                //Step 9 - save the added amicas datasource and Restart the services.
                wpfobject.GetMainWindowByTitle(ServiceTool.DataSource.Name.EditDataSource_Window);
                wpfobject.ClickButton(ServiceTool.DataSource.ID.OkBtn);
                Thread.Sleep(3000);
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseConfigTool();
                ExecutedSteps++;
                new Taskbar().Show();

                //Step 10 - From client, launch iCA and login with valid credential.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                ExecutedSteps++;

                //Step 11 - Load the studies from added Amicas datasource.
                studies = login.Navigate<Studies>();
                studies.SearchStudy(AccessionNo: Accessions[0], Datasource: login.GetHostName(Config.SanityPACS));
                studies.SelectStudy("Accession", Accessions[0]);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    BluRingViewer.LaunchBluRingViewer();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step_11 = studies.CompareImage(result.steps[ExecutedSteps], Viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                    if (step_11)
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
                    Viewer.CloseBluRingViewer();
                }
                else
                {
                    viewer = LaunchStudy();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    bool step_11 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                    if (step_11)
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
                    viewer.CloseStudy();
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

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Encrypted Password - SMTP Password
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_162374(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            DomainManagement domainmanagement;
            RoleManagement rolemanagement;
            result = new TestCaseResult(stepcount);
            BasePage basepage = new BasePage();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data   
                String SMTPUserNames = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UserName");
                String SMTPPasswords = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Password");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String DataSources = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceList");
                String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");

                String[] Accessions = AccessionList.Split(':');
                String[] Datasource = DataSources.Split(':');
                //String[] UserName = SMTPUserNames.Split(':');
				//String[] Password = SMTPPasswords.Split(':');

				String[] UserName = { Config.CustomUser1Email, Config.CustomUser2Email };
				String[] Password = { Config.CustomUserEmailPassword, Config.CustomUserEmailPassword };				

				string EmailNotificationConfig = @"C:\WebAccess\WebAccess\EmailNotification\Web.Config";
                string EncryptedPasswordInConfig = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "EncryptedPassword");

                //Step 1 - Launch IBM iConnect Access Service Tool.                
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                CheckBox EmailStudy = wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), "Enable Email Study", 1);
                if (EmailStudy.Checked == false)
                {
                    EmailStudy.Click();
                    servicetool.ApplyEnableFeatures();
                    wpfobject.WaitTillLoad();
                    servicetool.wpfobject.ClickOkPopUp();
                    servicetool.RestartService();
                    wpfobject.WaitTillLoad();
                }
                ExecutedSteps++;

                //Step 2 - Navigate to Email Notification tab and Click on Modify.
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                servicetool.SetEmailNotificationForPOP();
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ServiceTool.ModifyBtn_Name, 1);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step 3 - Configure the Email settings and Enter the SMTP password in Password field and Verify the entered password.
                wpfobject.ClearText("UsernameTxtBx");
                wpfobject.SetText("UsernameTxtBx", UserName[0]);
                wpfobject.ClearText("BindPassword");
                wpfobject.SetText("BindPassword", Password[0]);

                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step 4 - Save the changes and restart the services
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Step 5 - verify that the entered password value.
                string EncryptedPassword = basepage.GetAttributeValue(EmailNotificationConfig, "/configuration/appSettings/add[@key='EmailNotificationASMX.MailServerPassword']", "value");
                if(EncryptedPassword == EncryptedPasswordInConfig)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }



                //Step 6 - From client, launch iCA and login with valid credential.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("emailstudy", 0);
                domainmanagement.AddToolsToToolbarByName(new string[] { "Email Study" });
                domainmanagement.ClickSaveEditDomain();
                //PreCondition - Enable Email Study for SuperRole User
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectDomainfromDropDown(DefaultDomain);
                rolemanagement.SearchRole(DefaultRoleName);
                rolemanagement.SelectRole(DefaultRoleName);
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("email", 0);
                rolemanagement.ClickSaveEditRole();
                ExecutedSteps++;

                //Step 7 - Load any study from available datasource and Send the study via Email to any non-registered user.
                studies = login.Navigate<Studies>();
                studies.SearchStudy(AccessionNo: Accessions[0], Datasource: login.GetHostName(Config.SanityPACS));
                studies.SelectStudy("Accession", Accessions[0]);
                BluRingViewer Viewer = new BluRingViewer();
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    BluRingViewer.LaunchBluRingViewer();
                    String FetchPin = Viewer.EmailStudy_BR(Email, UserName[0], "Test");
                    if (!(FetchPin == ""))
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
                    Viewer.CloseBluRingViewer();
                }
                else
                {
                    viewer = LaunchStudy();
                    viewer.SelectToolInToolBar(IEnum.ViewerTools.EmailStudy);
                    wait.Until(ExpectedConditions.ElementIsVisible(viewer.By_EmailStudyDiv()));
                    viewer.ToEmailTxtBox().SendKeys(Email);
                    viewer.ToNameTxtBox().SendKeys(UserName[0]);
                    viewer.ReasonTxtBox().SendKeys("Test");
                    viewer.EmailStudySendBtn().Click();
                    if (!(studies.FetchPin() == ""))
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
                    viewer.CloseStudy();
                }
                login.Logout();

                //Step 8 - From iCA service tool Email Notification tab, update the another valid username and password.
                servicetool.LaunchServiceTool();
                wpfobject.SelectTabFromTabItems(ServiceTool.EmailNotification_Tab);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ServiceTool.ModifyBtn_Name, 1);
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                wpfobject.ClearText("UsernameTxtBx");
                wpfobject.SetText("UsernameTxtBx", UserName[1]);
                wpfobject.ClearText("BindPassword");
                wpfobject.SetText("BindPassword", Password[1]);
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                wpfobject.ClickOkPopUp();
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step 9 - Save the changes and restart the services
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Step 10 - From client, launch iCA and login with valid credential.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                ExecutedSteps++;

                //Step 11 - Load any study from available datasource and Send the study via Email to any non-registered user.
                studies = login.Navigate<Studies>();
                studies.SearchStudy(AccessionNo: Accessions[0], Datasource: login.GetHostName(Config.SanityPACS));
                studies.SelectStudy("Accession", Accessions[0]);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    BluRingViewer.LaunchBluRingViewer();
                    String FetchPin = Viewer.EmailStudy_BR(Email, UserName[0], "Test");
                    if (!(FetchPin == ""))
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
                    Viewer.CloseBluRingViewer();
                }
                else
                {
                    viewer = LaunchStudy();
                    viewer.SelectToolInToolBar(IEnum.ViewerTools.EmailStudy);
                    wait.Until(ExpectedConditions.ElementIsVisible(viewer.By_EmailStudyDiv()));
                    viewer.ToEmailTxtBox().SendKeys(Email);
                    viewer.ToNameTxtBox().SendKeys(UserName[0]);
                    viewer.ReasonTxtBox().SendKeys("Test");
                    viewer.EmailStudySendBtn().Click();
                    if (!(studies.FetchPin() == ""))
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
                    viewer.CloseStudy();
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

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }
    }
}