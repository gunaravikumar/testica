using System;
using System.Globalization;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
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
using Application = TestStack.White.Application;
using Window = TestStack.White.UIItems.WindowItems.Window;
using TestStack.White.Factory;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.TableItems;
using TestStack.White.InputDevices;
using TestStack.White.WindowsAPI;
using TestStack.White.UIItems;
using Selenium.Scripts.Pages.MergeServiceTool;
using System.Diagnostics;
using TestStack.White.Configuration;

namespace Selenium.Scripts.Tests
{
    class LdapArchiving : BasePage
    {

        public Login login { get; set; }
        public string filepath { get; set; }
        public ExamImporter ei { get; set; }
        public HPHomePage hphomepage { get; set; }
        public HPLogin hplogin { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public ServiceTool servicetool { get; set; }
        public WpfObjects wpfobject { get; set; }
        private String LDAPUploaderToolPath = "";
        private string EIWindowName = "";

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        public LdapArchiving(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            hplogin = new HPLogin();
            hphomepage = new HPHomePage();
            mpaclogin = new MpacLogin();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            ei = new ExamImporter();
            wpfobject = new WpfObjects();
        }



        public TestCaseResult Test_27668(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            TestCaseResult result;
            BasePage basepage = new BasePage();
            servicetool = new ServiceTool();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String DomainNames = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String[] domainname = DomainNames.Split(':');
                String LDApServerHost = "10.4.38.27";


                //step-1 :in service tool                 
                servicetool.LaunchServiceTool();
                // tool.RestartIIS();

                // Open the c:\Webaccess\Webaccess\web.config and change the Image Sharing flag to"True"
                // basepage.SetWebConfigValue(Config.webconfig, "Application.EnableImageSharing", "true");
                ExecutedSteps++;

                // Step -2
                // Add EA Datasource
                /*  tool.AddEADatasource(Config.HoldingPenIP, Config.HoldingPenAETitle, "1");

                  // Add Merge pacas Datasource -- ask siva
                  tool.AddPacsDatasource(Config.PACS2, Config.PACS2AETitle, "2", Config.pacsadmin, Config.pacspassword);

                  //Add a Serveral destination datasource 
                  // need to change the ea 
                  tool.AddEADatasource(Config.EA1, Config.HoldingPenAETitle, "3");
                  tool.AddEADatasource(Config.EA77, Config.HoldingPenAETitle, "4");
                  tool.AddEADatasource(Config.EA91, Config.HoldingPenAETitle, "5");

                  // Modify Enable Features
                  tool.NavigateToEnableFeatures();

                  tool.ModifyEnableFeatures();

                  tool.EnableStudySharing();
                  tool.EnableDataTransfer();
                  tool.EnableDataDownloader();
                  tool.EnableSelfEnrollment();
                  wpfobject.WaitTillLoad();
                  tool.ApplyEnableFeatures();

                  wpfobject.WaitTillLoad();
                  wpfobject.ClickOkPopUp();
                  wpfobject.WaitTillLoad();


                  //Navigate to Security tab

                  //need to get the values under general tab
                  tool.NavigateToTab(ServiceTool.Security_Tab);
                  tool.ClickModifyButton();
                  Thread.Sleep(2500);



                  //Setting HTTPS Checkbox
                  tool.SetHTTPS();
                  // Set the password
                  tool.SetPassWordPolicy(false);
                  tool.UpdateAdminContact("Administrator@Mail.com");
                  tool.CickApplyButton();
                  wpfobject.WaitTillLoad();
                  wpfobject.ClickOkPopUp();
                  wpfobject.WaitTillLoad();

                  // Email notification
                  tool.SetEmailNotification(0, "dinobot");*/

                //  User management Database tab select both Local Database and Ldap Directory Service 
                /* servicetool.NavigateToConfigToolUserMgmtDatabaseTab();
                 servicetool.SetMode(2);*/
                ExecutedSteps++;

                //Step - 3: Edit the .csv file.
                ExecutedSteps++;

                //Step - 4: Setp Ldap- tenet final data model.               
                servicetool.LDAPTenetFinaldmSetup();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

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
        }


        public TestCaseResult Test_27669(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            DomainManagement domainmanagement;
            TestCaseResult result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            try
            {
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                string DomainNames = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainNames");
                string DomainDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainDescription");
                string ReceivingInstitutionName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ReceivingInstitutionName");
                string CheckBoxes = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainCheckboxes");
                string DomainAdminUserInfo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainAdminUserInfo");
                string[] domainnames = DomainNames.Split(':');
                string[] domaindescription = DomainDescription.Split(':');
                string[] receivinginstitutionname = ReceivingInstitutionName.Split(':');
                string[] checkboxes = CheckBoxes.Split(':');
                string[] domainadminuserinfo = DomainAdminUserInfo.Split(':');
                string[] userinfo = null;
                //Step 1: Using the URL below open the Browser to the iConnect Access login screen. <br/>https://servername.merge.com/webaccess<br/><br/>Login as Administrator/Administrator and navigate to the Domain manager tab
                login.LoginIConnect(username, password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");

                ExecutedSteps++;
                //Step 2: Click on the New Domain button <br/>Enter the Following three Domains<br/>Move all of the Datasources to the Connected side<br/>Domains:<br/>Domain Name:    MarketDomain1    <br/>*Domain Description   MD1<br/>*Receiving Institution Name:  TenetMD1<br/>Enable Data Transfer<br/>Enable Grant Access<br/>Enable Data Download<br/>Auto Inherit Group Roles (yes)<br/>Enable the Allow Domain Admin control Image Sharing preference<br/>Enable Image sharing<br/>Enable Make Java Exam Importer as Default Exam Importer<br/>Move the Datasources to the Connected side <br/>Domain Admin User Info <br/>UserID= a pwd.13579<br/>Last Name = a<br/>First Name = a<br/>Password = pwd.13579 Confirm Password = pwd.13579<br/>Role = a<br/>Role Description = a<br/>Save
                userinfo = domainadminuserinfo[0].Split(',');
                if (!(domainmanagement.SearchDomain(domainnames[0])))
                {
                    domainmanagement.CreateDomain(domainnames[0], domaindescription[0], receivinginstitutionname[0], userinfo[0], null, userinfo[1], userinfo[2], userinfo[3], userinfo[4], userinfo[5], checkbox: checkboxes[0]);
                }
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (domainmanagement.SearchDomain(domainnames[0]))
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
                //Step 3: Click on the New Domain button <br/>Enter the Following three Domains<br/><br/>Move all of the Datasources to the Connected side<br/>Domains:<br/>Domain Name:    MarketDomain2    <br/>*Domain Description   MD2<br/>*Receiving Institution Name:  TenetMD2<br/>Do not Enable Auto Inherit Group Roles<br/>Do not Enable Allow Domain Admin control Image Sharing preference<br/>Enable Image sharing<br/>Do not Enable Make Java Exam Importer as Default Exam Importer<br/>Move the Datasources to the Connected side <br/>Domain Admin User Info <br/>UserID= b<br/>Last Name = b<br/>First Name = b<br/>Password = pwd.13579 Confirm Password = pwd.13579<br/>Role = b<br/>Role Description = b<br/>Save
                userinfo = domainadminuserinfo[1].Split(',');
                if (!(domainmanagement.SearchDomain(domainnames[1])))
                {
                    domainmanagement.CreateDomain(domainnames[1], domaindescription[1], receivinginstitutionname[1], userinfo[0], null, userinfo[1], userinfo[2], userinfo[3], userinfo[4], userinfo[5], checkbox: checkboxes[1]);
                }
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (domainmanagement.SearchDomain(domainnames[1]))
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
                //Step 4: Click on the New Domain button <br/>Enter the Following three Domains<br/><br/>Move all of the Datasources to the Connected side<br/>Domains:<br/>Domain Name:    MarketDomain3   <br/>*Domain Description   MD3<br/>*Receiving Institution Name:  TenetMD3<br/>Enable Data Transfer<br/>Enable Grant Access<br/>Enable Data Download<br/>Auto Inherit Group Roles<br/>Enable Image sharing<br/>Enable Allow Domain Admin control Image Sharing preference<br/>Do not Enable Make Java Exam Importer as Default Exam Importer<br/>Move the Datasources to the Connected side <br/>Domain Admin User Info <br/>UserID= c<br/>Last Name = c<br/>First Name = c<br/>Password = pwd.13579 Confirm Password = pwd.13579<br/>Role = c<br/>Role Description = c<br/>Save
                userinfo = domainadminuserinfo[2].Split(',');
                if (!(domainmanagement.SearchDomain(domainnames[2])))
                {
                    domainmanagement.CreateDomain(domainnames[2], domaindescription[2], receivinginstitutionname[2], userinfo[0], null, userinfo[1], userinfo[2], userinfo[3], userinfo[4], userinfo[5], checkbox: checkboxes[2]);
                }
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (domainmanagement.SearchDomain(domainnames[2]))
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

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }


        public TestCaseResult Test_27670(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            DomainManagement domainmanagement;
            RoleManagement rolemanagement;
            TestCaseResult result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            try
            {
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                string DomainNames = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainNamesForRole");
                string RoleNames = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleNames");
                string RoleDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleDescription");
                string RoleCheckboxes = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleCheckboxes");
                string GrantAccess = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "GrantAccess");
                string DomainDomainNames = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainNames");
                string DomainDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainDescription");
                string ReceivingInstitutionName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ReceivingInstitutionName");
                string DomainCheckboxes = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainCheckboxes");
                string DomainAdminUserInfo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainAdminUserInfo");
                string[] domainnames = DomainNames.Split(':');
                string[] rolenames = RoleNames.Split(':');
                string[] roledescription = RoleDescription.Split(':');
                string[] rolecheckbox = RoleCheckboxes.Split(':');
                string[] grantaccess = GrantAccess.Split(':');
                string[] domaindomainnames = DomainDomainNames.Split(':');
                string[] domaindescription = DomainDescription.Split(':');
                string[] receivinginstitutionname = ReceivingInstitutionName.Split(':');
                string[] domaincheckboxes = DomainCheckboxes.Split(':');
                string[] domainadminuserinfo = DomainAdminUserInfo.Split(':');
                bool grantgroup = false;
                bool grantanyone = false;
                //Step 1: Login as Administrator/Administrator in iCA<br/>Click on the Role Management Tab and select the new Domain entered  MarketDomain1. from the dropdown window. Click 
                login.LoginIConnect(username, password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                for (int i = 0; i < domaindomainnames.Count(); i++)
                {
                    if (!domainmanagement.SearchDomain(domaindomainnames[i]))
                    {
                        string[] userinfo = domainadminuserinfo[i].Split(',');
                        domainmanagement.CreateDomain(domainnames[i], domaindescription[i], receivinginstitutionname[i], userinfo[0], null, userinfo[1], userinfo[2], userinfo[3], userinfo[4], userinfo[5], 0, null, domaincheckboxes[i]);
                    }
                }
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                ExecutedSteps++;

                //Step 2: Enter the following to create the Roles<br/>Role name: MarketDomain1Physician<br/>Role Description: MD1PH<br/>Enable the Receive Exams flag<br/>Enable the Grant access to Group only <br/>Save <br/>Role name: MarketDomain1Archivist<br/>Role Description: MD1AR<br/>Remove the Enable from the Receiving Flag<br/>Enable the Archive to PACS flag<br/>Enable the Grant access to Everyone <br/>Save<br/>Role name: MarketDomain1PACSAdmin (Group Admin)<br/>Role Description: MD1Pacs<br/>Enable the Receive Exams flag<br/>Enable the Archive to PACS flag<br/>Enable the Grant access to Group only <br/>Save<br/>Role Name:  MarketDomain1RegionalAdmin (SiteAdmin)<br/>Role Description: MD1RA<br/>Enable the Receive Exams flag<br/>Enable the Archive to PACS flag<br/>Enable the Grant access to Anyone only <br/>Save<br/>
                //Create a role Physician for MarketDomain1
                bool steps = true;
                grantanyone = string.Equals(grantaccess[0].ToLowerInvariant(), "anyone");
                grantgroup = string.Equals(grantaccess[0].ToLowerInvariant(), "group");
                if (!(rolemanagement.RoleExists(rolenames[0], domainnames[0])))
                {
                    rolemanagement.CreateRole(domainnames[0], rolenames[0], roledescription[0], rolecheckbox[0].Split(','), grantanyone, grantgroup);
                }
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (!rolemanagement.RoleExists(rolenames[0], domainnames[0]))
                {
                    Logger.Instance.ErrorLog(string.Concat("Role Name ", rolenames[0], "does not Exist in ", domainnames[0], "..."));
                    steps = false;
                }
                //Create a role Archivist for MarketDomain1
                grantanyone = string.Equals(grantaccess[1].ToLowerInvariant(), "anyone");
                grantgroup = string.Equals(grantaccess[1].ToLowerInvariant(), "group");
                if (!(rolemanagement.RoleExists(rolenames[1], domainnames[1])))
                {
                    rolemanagement.CreateRole(domainnames[1], rolenames[1], roledescription[1], rolecheckbox[1].Split(','), grantanyone, grantgroup);
                }
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (!rolemanagement.RoleExists(rolenames[1], domainnames[1]))
                {
                    Logger.Instance.ErrorLog(string.Concat("Role Name ", rolenames[1], "does not Exist in ", domainnames[1], "..."));
                    steps = false;
                }
                //Create a role PACSAdmin for MarketDomain1
                grantanyone = string.Equals(grantaccess[2].ToLowerInvariant(), "anyone");
                grantgroup = string.Equals(grantaccess[2].ToLowerInvariant(), "group");
                if (!(rolemanagement.RoleExists(rolenames[2], domainnames[2])))
                {
                    rolemanagement.CreateRole(domainnames[2], rolenames[2], roledescription[2], rolecheckbox[2].Split(','), grantanyone, grantgroup);
                }
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (!rolemanagement.RoleExists(rolenames[2], domainnames[2]))
                {
                    Logger.Instance.ErrorLog(string.Concat("Role Name ", rolenames[2], "does not Exist in ", domainnames[2], "..."));
                    steps = false;
                }
                //Create a role RegionalAdmin for MarketDomain1
                grantanyone = string.Equals(grantaccess[3].ToLowerInvariant(), "anyone");
                grantgroup = string.Equals(grantaccess[3].ToLowerInvariant(), "group");
                if (!(rolemanagement.RoleExists(rolenames[3], domainnames[3])))
                {
                    rolemanagement.CreateRole(domainnames[3], rolenames[3], roledescription[3], rolecheckbox[3].Split(','), grantanyone, grantgroup);
                }
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (!rolemanagement.RoleExists(rolenames[3], domainnames[3]))
                {
                    Logger.Instance.ErrorLog(string.Concat("Role Name ", rolenames[3], "does not Exist in ", domainnames[3], "..."));
                    steps = false;
                }
                if (steps)
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
                //Step 3: Enter the following to create the Roles<br/>Role name: MarketDomain2Physician<br/>Role Description: MD2PH<br/>Enable the Receive Exams flag<br/>Enable the Grant access to Group only <br/>Save <br/>Role name: MarketDomain2Archivist<br/>Role Description: MD2AR<br/>Remove the Enable from the Receiving Flag<br/>Enable the Archive to PACS flag<br/>Enable the Grant access to Everyone <br/>Save<br/>Role name: MarketDomain2PACSAdmin (Group Admin)<br/>Role Description: MD2Pacs<br/>Enable the Receive Exams flag<br/>Enable the Archive to PACS flag<br/>Enable the Grant access to Group only <br/>Save<br/>Role Name:  MarketDomain2RegionalAdmin (SiteAdmin)<br/>Role Description: MD2RA<br/>Enable the Receive Exams flag<br/>Enable the Archive to PACS flag<br/>Enable the Grant access to Anyone only <br/>Save<br/>
                //Create a role Physician for MarketDomain2
                steps = true;
                grantanyone = string.Equals(grantaccess[4].ToLowerInvariant(), "anyone");
                grantgroup = string.Equals(grantaccess[4].ToLowerInvariant(), "group");
                if (!(rolemanagement.RoleExists(rolenames[4], domainnames[4])))
                {
                    rolemanagement.CreateRole(domainnames[4], rolenames[4], roledescription[4], rolecheckbox[4].Split(','), grantanyone, grantgroup);
                }
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (!rolemanagement.RoleExists(rolenames[4], domainnames[4]))
                {
                    Logger.Instance.ErrorLog(string.Concat("Role Name ", rolenames[4], "does not Exist in ", domainnames[4], "..."));
                    steps = false;
                }
                //Create a role Archivist for MarketDomain2
                grantanyone = string.Equals(grantaccess[5].ToLowerInvariant(), "anyone");
                grantgroup = string.Equals(grantaccess[5].ToLowerInvariant(), "group");
                if (!rolemanagement.RoleExists(rolenames[5], domainnames[5]))
                {
                    rolemanagement.CreateRole(domainnames[5], rolenames[5], roledescription[5], rolecheckbox[5].Split(','), grantanyone, grantgroup);
                }
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (!rolemanagement.RoleExists(rolenames[5], domainnames[5]))
                {
                    Logger.Instance.ErrorLog(string.Concat("Role Name ", rolenames[5], "does not Exist in ", domainnames[5], "..."));
                    steps = false;
                }
                //Create a role PACSAdmin for MarketDomain2
                grantanyone = string.Equals(grantaccess[6].ToLowerInvariant(), "anyone");
                grantgroup = string.Equals(grantaccess[6].ToLowerInvariant(), "group");
                if (!rolemanagement.RoleExists(rolenames[6], domainnames[6]))
                {
                    rolemanagement.CreateRole(domainnames[6], rolenames[6], roledescription[6], rolecheckbox[6].Split(','), grantanyone, grantgroup);
                }
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (!rolemanagement.RoleExists(rolenames[6], domainnames[6]))
                {
                    Logger.Instance.ErrorLog(string.Concat("Role Name ", rolenames[6], "does not Exist in ", domainnames[6], "..."));
                    steps = false;
                }
                //Create a role RegionalAdmin for MarketDomain2
                grantanyone = string.Equals(grantaccess[7].ToLowerInvariant(), "anyone");
                grantgroup = string.Equals(grantaccess[7].ToLowerInvariant(), "group");
                if (!rolemanagement.RoleExists(rolenames[7], domainnames[7]))
                {
                    rolemanagement.CreateRole(domainnames[7], rolenames[7], roledescription[7], rolecheckbox[7].Split(','), grantanyone, grantgroup);
                }
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (!rolemanagement.RoleExists(rolenames[7], domainnames[7]))
                {
                    Logger.Instance.ErrorLog(string.Concat("Role Name ", rolenames[7], "does not Exist in ", domainnames[7], "..."));
                    steps = false;
                }
                if (steps)
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
                //Step 4 : Enter the following to create the Roles<br/>Role name: MarketDomain3Physician<br/>Role Description: MD3PH<br/>Enable the Receive Exams flag<br/>Enable the Grant access to Group only <br/>Save <br/>Role name: MarketDomain3Archivist<br/>Role Description: MD3AR<br/>Remove the Enable from the Receiving Flag<br/>Enable the Archive to PACS flag<br/>Enable the Grant access to Everyone <br/>Save<br/>Role name: MarketDomain3PACSAdmin (Group Admin)<br/>Role Description: MD3Pacs<br/>Enable the Receive Exams flag<br/>Enable the Archive to PACS flag<br/>Enable the Grant access to Group only <br/>Save<br/>Role Name:  MarketDomain3RegionalAdmin (SiteAdmin)<br/>Role Description: MD3RA<br/>Enable the Receive Exams flag<br/>Enable the Archive to PACS flag<br/>Enable the Grant access to Anyone only <br/>Save<br/>
                //Create a role Physician for MarketDomain3
                steps = true;
                grantanyone = string.Equals(grantaccess[8].ToLowerInvariant(), "anyone");
                grantgroup = string.Equals(grantaccess[8].ToLowerInvariant(), "group");
                if (!rolemanagement.RoleExists(rolenames[8], domainnames[8]))
                {
                    rolemanagement.CreateRole(domainnames[8], rolenames[8], roledescription[8], rolecheckbox[8].Split(','), grantanyone, grantgroup);
                }
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (!rolemanagement.RoleExists(rolenames[8], domainnames[8]))
                {
                    Logger.Instance.ErrorLog(string.Concat("Role Name ", rolenames[8], "does not Exist in ", domainnames[8], "..."));
                    steps = false;
                }
                //Create a role Archivist for MarketDomain3
                grantanyone = string.Equals(grantaccess[9].ToLowerInvariant(), "anyone");
                grantgroup = string.Equals(grantaccess[9].ToLowerInvariant(), "group");
                if (!rolemanagement.RoleExists(rolenames[9], domainnames[9]))
                {
                    rolemanagement.CreateRole(domainnames[9], rolenames[9], roledescription[9], rolecheckbox[9].Split(','), grantanyone, grantgroup);
                }
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (!rolemanagement.RoleExists(rolenames[9], domainnames[9]))
                {
                    Logger.Instance.ErrorLog(string.Concat("Role Name ", rolenames[9], "does not Exist in ", domainnames[9], "..."));
                    steps = false;
                }
                //Create a role PACSAdmin for MarketDomain3
                grantanyone = string.Equals(grantaccess[10].ToLowerInvariant(), "anyone");
                grantgroup = string.Equals(grantaccess[10].ToLowerInvariant(), "group");
                if (!rolemanagement.RoleExists(rolenames[10], domainnames[10]))
                {
                    rolemanagement.CreateRole(domainnames[10], rolenames[10], roledescription[10], rolecheckbox[10].Split(','), grantanyone, grantgroup);
                }
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (!rolemanagement.RoleExists(rolenames[10], domainnames[10]))
                {
                    Logger.Instance.ErrorLog(string.Concat("Role Name ", rolenames[10], "does not Exist in ", domainnames[10], "..."));
                    steps = false;
                }
                //Create a role RegionalAdmin for MarketDomain3
                grantanyone = string.Equals(grantaccess[11].ToLowerInvariant(), "anyone");
                grantgroup = string.Equals(grantaccess[11].ToLowerInvariant(), "group");
                if (!rolemanagement.RoleExists(rolenames[11], domainnames[11]))
                {
                    rolemanagement.CreateRole(domainnames[11], rolenames[11], roledescription[11], rolecheckbox[11].Split(','), grantanyone, grantgroup);
                }
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (!rolemanagement.RoleExists(rolenames[11], domainnames[11]))
                {
                    Logger.Instance.ErrorLog(string.Concat("Role Name ", rolenames[11], "does not Exist in ", domainnames[11], "..."));
                    steps = false;
                }
                if (steps)
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
                //Step 5: Create Roles for SubGroups <br/> Role name =   MarketDomain1Cardiologist<br/>Role Description: MD1GC<br/>Enable the Receive Exams flag<br/>Enable the Archive to PACS flag<br/>Enable the Grant access to Anyone only <br/>Save<br/><br/><br/><br/>
                //Create a role Cardiologist for MarketDomain1
                grantanyone = string.Equals(grantaccess[12].ToLowerInvariant(), "anyone");
                grantgroup = string.Equals(grantaccess[12].ToLowerInvariant(), "group");
                if (!rolemanagement.RoleExists(rolenames[12], domainnames[12]))
                {
                    rolemanagement.CreateRole(domainnames[12], rolenames[12], roledescription[12], rolecheckbox[12].Split(','), grantanyone, grantgroup);
                }
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (rolemanagement.RoleExists(rolenames[12], domainnames[12]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog(string.Concat("Role Name ", rolenames[12], "does not Exist in ", domainnames[12], "..."));
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 6: Create Roles for SubGroups <br/> Role name =   MarketDomain1Radiologist<br/>Role Description: MD1GR<br/>Enable the Receive Exams flag<br/>Enable the Archive to PACS flag<br/>Enable the Grant access to Anyone only <br/>Save
                //Create a role Radiologist for MarketDomain1
                grantanyone = string.Equals(grantaccess[13].ToLowerInvariant(), "anyone");
                grantgroup = string.Equals(grantaccess[13].ToLowerInvariant(), "group");
                if (!rolemanagement.RoleExists(rolenames[13], domainnames[13]))
                {
                    rolemanagement.CreateRole(domainnames[13], rolenames[13], roledescription[13], rolecheckbox[13].Split(','), grantanyone, grantgroup);
                }
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (rolemanagement.RoleExists(rolenames[13], domainnames[13]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog(string.Concat("Role Name ", rolenames[13], "does not Exist in ", domainnames[13], "..."));
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 7: Create Roles for SubGroups <br/> Role name =   MarketDomain2Cardiologist<br/>Role Description: MD2GC<br/>Enable the Receive Exams flag<br/>Enable the Archive to PACS flag<br/>Enable the Grant access to Anyone only <br/>Save
                //Create a role Cardiologist for MarketDomain2
                grantanyone = string.Equals(grantaccess[14].ToLowerInvariant(), "anyone");
                grantgroup = string.Equals(grantaccess[14].ToLowerInvariant(), "group");
                if (!rolemanagement.RoleExists(rolenames[14], domainnames[14]))
                {
                    rolemanagement.CreateRole(domainnames[14], rolenames[14], roledescription[14], rolecheckbox[14].Split(','), grantanyone, grantgroup);
                }
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (rolemanagement.RoleExists(rolenames[14], domainnames[14]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog(string.Concat("Role Name ", rolenames[14], "does not Exist in ", domainnames[14], "..."));
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 8: Create Roles for SubGroups <br/> Role name =   MarketDomain2Radiologist<br/>Role Description: MD2GR<br/>Enable the Receive Exams flag<br/>Enable the Archive to PACS flag<br/>Enable the Grant access to Anyone only <br/>Save
                //Create a role Radiologist for MarketDomain2
                grantanyone = string.Equals(grantaccess[15].ToLowerInvariant(), "anyone");
                grantgroup = string.Equals(grantaccess[15].ToLowerInvariant(), "group");
                if (!rolemanagement.RoleExists(rolenames[15], domainnames[15]))
                {
                    rolemanagement.CreateRole(domainnames[15], rolenames[15], roledescription[15], rolecheckbox[15].Split(','), grantanyone, grantgroup);
                }
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (rolemanagement.RoleExists(rolenames[15], domainnames[15]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog(string.Concat("Role Name ", rolenames[15], "does not Exist in ", domainnames[15], "..."));
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 9: Create Roles for SubGroups <br/> Role name =   MarketDomain3Cardiologist<br/>Role Description: MD3GC<br/>Enable the Receive Exams flag<br/>Enable the Archive to PACS flag<br/>Enable the Grant access to Anyone only <br/>Save
                //Create a role Cardiologist for MarketDomain3
                grantanyone = string.Equals(grantaccess[16].ToLowerInvariant(), "anyone");
                grantgroup = string.Equals(grantaccess[16].ToLowerInvariant(), "group");
                if (!rolemanagement.RoleExists(rolenames[16], domainnames[16]))
                {
                    rolemanagement.CreateRole(domainnames[16], rolenames[16], roledescription[16], rolecheckbox[16].Split(','), grantanyone, grantgroup);
                }
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (rolemanagement.RoleExists(rolenames[16], domainnames[16]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog(string.Concat("Role Name ", rolenames[16], "does not Exist in ", domainnames[16], "..."));
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 10: Create Roles for SubGroups <br/> Role name =   MarketDomain3Radiologist<br/>Role Description: MD3GR<br/>Enable the Receive Exams flag<br/>Enable the Archive to PACS flag<br/>Enable the Grant access to Anyone only <br/>Save
                //Create a role Radiologist for MarketDomain3
                grantanyone = string.Equals(grantaccess[17].ToLowerInvariant(), "anyone");
                grantgroup = string.Equals(grantaccess[17].ToLowerInvariant(), "group");
                if (!rolemanagement.RoleExists(rolenames[17], domainnames[17]))
                {
                    rolemanagement.CreateRole(domainnames[17], rolenames[17], roledescription[17], rolecheckbox[17].Split(','), grantanyone, grantgroup);
                }
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (rolemanagement.RoleExists(rolenames[17], domainnames[17]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog(string.Concat("Role Name ", rolenames[17], "does not Exist in ", domainnames[17], "..."));
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
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

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }


        public TestCaseResult Test_27671(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            UserManagement usermanagement = null;
            DomainManagement domain = null;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            String Rolename = "role_" + new Random().Next(1000);

            try
            {
                //Fetch required Test data                       
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;

                String DomainNames = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainNames");
                String[] DomainName = DomainNames.Split(':');

                String GroupNames = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "GroupNames");
                String[] GroupName = GroupNames.Split(':');

                String SubGroupNames = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "SubGroupNames");
                String[] SubGroupName = SubGroupNames.Split(':');


                String LDAPDomainName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LDAP DomainName");
                String LDAPDomainAdmin = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LDAP DomainAdmin");
                String LDAPInstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LDAP InstituitionName");
                String LDAPDomainDesc = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LDAP DomainDescription");
                String LDAPPassword = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LDAP Password");
                string[] datasource = { "VMSSA-4-38-131", "AUTO-SSA-001", "VMSSA-5-38-91" };

                // Step -1 
                //Login as a Super Admin <br/>super.admin/pwd.13579 and Select the User management Tab and select the MarketDomain1 from the drop down window.<br/>
                login.LoginIConnect(Username, Password);
                // Navigate to Domain Management and verify that the domain exists
                domain = (DomainManagement)login.Navigate("DomainManagement");

                if (!domain.SearchDomain(DomainName[0]))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    domain.CreateDomain(DomainName[0], Rolename, datasources: datasource);
                    PageLoadWait.WaitForPageLoad(30);
                    PageLoadWait.WaitForFrameLoad(10);
                    domain.ClickSaveDomain();
                }
                ExecutedSteps++;

                // Navigate to User Management -step 2
                PageLoadWait.WaitForPageLoad(10);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                ExecutedSteps++;

                //Step 3
                // Click on the New Group button Enter the Group name =MD1History1<br/>Group Description =  MD1G1<br/>Select the Data Sources tab in the form and move all the datasources to the right hand side <br/>Save the Group  <br/><br/>
                // Create a Group with all EA datasource
                if (!usermanagement.SearchGroup(GroupName[0], DomainName[0], 0))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateGroup(DomainName[0], GroupName[0], selectalldatasources: 0, selectallroles: 1);
                }

                if (usermanagement.SearchGroup(GroupName[0], DomainName[0], 0))
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


                // Step 4
                // Repeat and enter the other two Groups <br/>MD1History2 <br/>Select the Role tab,  all the Roles are on right hand side<br/>Select the Datasource tab abd move all of the Datasources to the right hand side <br/>
                // MD1History3 <br/>Auto Inherit is enabled <br/>Select the Datasource tab abd move all of the Datasources to the right hand side <br/>Save
                // Create a Group with all data source and roles
                usermanagement.ClearBtn().Click();
                if (!usermanagement.SearchGroup(GroupName[1], DomainName[0], 0))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateGroup(DomainName[0], GroupName[1], selectalldatasources: 0, selectallroles: 0);
                }
                usermanagement.ClearBtn().Click();
                if (!usermanagement.SearchGroup(GroupName[2], DomainName[0], 0))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateGroup(DomainName[0], GroupName[2], selectalldatasources: 0, selectallroles: 1);
                }
                usermanagement.ClearBtn().Click();
                if (usermanagement.SearchGroup(GroupName[1], DomainName[0], 0) && usermanagement.SearchGroup(GroupName[2], DomainName[0], 0))
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
                
                //Step 5
                // Select MD1History1<br/>click New Subgroup<br/>Group name: Radiology<br/>Save&Create Another Subgroup<br/>Group name: Cardiology<br/>Save&Create Another Subgroup<br/>Group name: Urology<br/>Save&Create Another Subgroup

                usermanagement.ClearBtn().Click();
                PageLoadWait.WaitForPageLoad(10);
                usermanagement.SearchGroup(GroupName[0], DomainName[0], 0);
                usermanagement.SelectGroup(GroupName[0], DomainName[0]);
                if (!usermanagement.SelectSubGroup(GroupName[0], SubGroupName[0]))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateSubGroup(GroupName[0], SubGroupName[0]);
                }

                usermanagement.ClearBtn().Click();
                PageLoadWait.WaitForPageLoad(10);
                usermanagement.SearchGroup(GroupName[0], DomainName[0], 0);
                usermanagement.SelectGroup(GroupName[0], DomainName[0]);
                if (!usermanagement.SelectSubGroup(GroupName[0], SubGroupName[1]))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateSubGroup(GroupName[0], SubGroupName[1]);
                }
                usermanagement.ClearBtn().Click();
                PageLoadWait.WaitForPageLoad(10);
                usermanagement.SearchGroup(GroupName[0], DomainName[0], 0);
                usermanagement.SelectGroup(GroupName[0], DomainName[0]);
                if (!usermanagement.SelectSubGroup(GroupName[0], SubGroupName[2]))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateSubGroup(GroupName[0], SubGroupName[2]);
                }
                usermanagement.ClearBtn().Click();
                if (usermanagement.SearchGroup(SubGroupName[0], DomainName[0], 1) && usermanagement.SearchGroup(SubGroupName[1], DomainName[0], 1) && usermanagement.SearchGroup(SubGroupName[2], DomainName[0], 1))
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

                // Step 6
                // Select MD1History2<br/>click New Subgroup<br/>Group name: Radiology<br/>Save&Create Another Subgroup<br/>Group name: Cardiology<br/>Save&Create Another Subgroup<br/>Group name: Urology<br/>Save&Create Another Subgroup
                usermanagement.ClearBtn().Click();
                PageLoadWait.WaitForPageLoad(10);
                usermanagement.SearchGroup(GroupName[1], DomainName[0], 0);
                usermanagement.SelectGroup(GroupName[1], DomainName[0]);
                if (!usermanagement.SelectSubGroup(GroupName[1], SubGroupName[0]))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateSubGroup(GroupName[1], SubGroupName[0]);
                }

                usermanagement.ClearBtn().Click();
                usermanagement.SearchGroup(GroupName[1], DomainName[0], 0);
                usermanagement.SelectGroup(GroupName[1], DomainName[0]);
                if (!usermanagement.SelectSubGroup(GroupName[1], SubGroupName[1]))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateSubGroup(GroupName[1], SubGroupName[1]);
                }
                usermanagement.ClearBtn().Click();
                usermanagement.SearchGroup(GroupName[1], DomainName[0], 0);
                usermanagement.SelectGroup(GroupName[1], DomainName[0]);
                if (!usermanagement.SelectSubGroup(GroupName[1], SubGroupName[2]))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateSubGroup(GroupName[1], SubGroupName[2]);
                }
                usermanagement.ClearBtn().Click();
                if (usermanagement.SearchGroup(SubGroupName[0], DomainName[0], 1) && usermanagement.SearchGroup(SubGroupName[1], DomainName[0], 1) && usermanagement.SearchGroup(SubGroupName[2], DomainName[0], 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                }

                // Step 7
                // Select MD1History3<br/>click New Subgroup<br/>Group name: Radiology<br/>Save&Create Another Subgroup<br/>Group name: Cardiology<br/>Save&Create Another Subgroup<br/>Group name: Urology<br/>Save&Create Another Subgroup
                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain(DomainName[0]);
                domain.SelectDomain(DomainName[0]);
                domain.EditDomainButton().Click();
                domain.SetCheckBoxInEditDomain("autoinherit", 0);
                domain.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(10);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(10);
                usermanagement.ClearBtn().Click();
                PageLoadWait.WaitForPageLoad(10);
                usermanagement.SearchGroup(GroupName[2], DomainName[0], 0);
                usermanagement.SelectGroup(GroupName[2], DomainName[0]);
                if (!usermanagement.SelectSubGroup(GroupName[2], SubGroupName[0]))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateSubGroup(GroupName[2], SubGroupName[0]);
                }
                usermanagement.ClearBtn().Click();
                usermanagement.SearchGroup(GroupName[2], DomainName[0], 0);
                usermanagement.SelectGroup(GroupName[2], DomainName[0]);
                if (!usermanagement.SelectSubGroup(GroupName[2], SubGroupName[1]))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateSubGroup(GroupName[2], SubGroupName[1]);
                }

                usermanagement.ClearBtn().Click();
                usermanagement.SearchGroup(GroupName[2], DomainName[0], 0);
                usermanagement.SelectGroup(GroupName[2], DomainName[0]);
                if (!usermanagement.SelectSubGroup(GroupName[2], SubGroupName[2]))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateSubGroup(GroupName[2], SubGroupName[2]);
                }


                if (usermanagement.SearchGroup(SubGroupName[0], DomainName[0], 1) && usermanagement.SearchGroup(SubGroupName[1], DomainName[0], 1) && usermanagement.SearchGroup(SubGroupName[2], DomainName[0], 1))
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

                //Step 8
                //Test Data: <br/> <br/>---------------------------------<br/>Go back to the User management and select the <br/>Market Domain2 and enter the three Groups as in the previous step then repeat for MarketDomain3. See Ldap-Model6 Tab for detail settings.<br/>Domain --MarketDomain2<br/>       Groups MD2History1 <br/>                      MD2History2  <br/>Domain  --MarketDomain3<br/>     Groups   MD3History1<br/>                      MD3History2<br/>
                domain = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForPageLoad(10);
                if (!domain.SearchDomain(DomainName[1]))
                {
                    domain.CreateDomain(DomainName[1], Rolename, datasources: datasource);
                    PageLoadWait.WaitForPageLoad(30);
                    PageLoadWait.WaitForFrameLoad(10);
                    domain.ClickSaveDomain();
                }

                PageLoadWait.WaitForPageLoad(10);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                // ExecutedSteps++;

                //Adding MarketDomain2 Groups and sub groups
                usermanagement.ClearBtn().Click();
                if (!usermanagement.SearchGroup(GroupName[3], DomainName[1], 0))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateGroup(DomainName[1], GroupName[3], selectalldatasources: 0);
                }
                usermanagement.ClearBtn().Click();
                if (!usermanagement.SearchGroup(GroupName[4], DomainName[1], 0))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateGroup(DomainName[1], GroupName[4], selectalldatasources: 0, selectallroles: 0);
                }

                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain(DomainName[1]);
                domain.SelectDomain(DomainName[1]);
                domain.EditDomainButton().Click();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                domain.SetCheckBoxInEditDomain("autoinherit", 0);
                domain.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(10);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.ClearBtn().Click();
                if (!usermanagement.SearchGroup(GroupName[5], DomainName[1], 0))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateGroup(DomainName[1], GroupName[5], selectalldatasources: 0);
                }

                usermanagement.ClearBtn().Click();
                usermanagement.SearchGroup(GroupName[3], DomainName[1], 0);
                usermanagement.SelectGroup(GroupName[3], DomainName[1]);
                if (!usermanagement.SelectSubGroup(GroupName[3], SubGroupName[0]))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateSubGroup(GroupName[3], SubGroupName[0]);
                }
                usermanagement.ClearBtn().Click();
                usermanagement.SearchGroup(GroupName[3], DomainName[1], 0);
                usermanagement.SelectGroup(GroupName[3], DomainName[1]);
                if (!usermanagement.SelectSubGroup(GroupName[3], SubGroupName[1]))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateSubGroup(GroupName[3], SubGroupName[1]);
                }

                usermanagement.ClearBtn().Click();
                usermanagement.SearchGroup(GroupName[3], DomainName[1], 0);
                usermanagement.SelectGroup(GroupName[3], DomainName[1]);
                if (!usermanagement.SelectSubGroup(GroupName[3], SubGroupName[2]))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateSubGroup(GroupName[3], SubGroupName[2]);
                }

                usermanagement.ClearBtn().Click();
                usermanagement.SearchGroup(GroupName[4], DomainName[1], 0);
                usermanagement.SelectGroup(GroupName[4], DomainName[1]);
                if (!usermanagement.SelectSubGroup(GroupName[4], SubGroupName[0]))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateSubGroup(GroupName[4], SubGroupName[0]);
                }

                usermanagement.ClearBtn().Click();
                usermanagement.SearchGroup(GroupName[4], DomainName[1], 0);
                usermanagement.SelectGroup(GroupName[4], DomainName[1]);
                if (!usermanagement.SelectSubGroup(GroupName[4], SubGroupName[1]))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateSubGroup(GroupName[4], SubGroupName[1]);
                }

                usermanagement.ClearBtn().Click();
                usermanagement.SearchGroup(GroupName[4], DomainName[1], 0);
                usermanagement.SelectGroup(GroupName[4], DomainName[1]);
                if (!usermanagement.SelectSubGroup(GroupName[4], SubGroupName[2]))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateSubGroup(GroupName[4], SubGroupName[2]);
                }

                usermanagement.ClearBtn().Click();
                usermanagement.SearchGroup(GroupName[5], DomainName[1], 0);
                usermanagement.SelectGroup(GroupName[5], DomainName[1]);
                if (!usermanagement.SelectSubGroup(GroupName[5], SubGroupName[0]))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateSubGroup(GroupName[5], SubGroupName[0]);
                }

                usermanagement.ClearBtn().Click();
                usermanagement.SearchGroup(GroupName[5], DomainName[1], 0);
                usermanagement.SelectGroup(GroupName[5], DomainName[1]);
                if (!usermanagement.SelectSubGroup(GroupName[5], SubGroupName[1]))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateSubGroup(GroupName[5], SubGroupName[1]);
                }

                usermanagement.ClearBtn().Click();
                usermanagement.SearchGroup(GroupName[5], DomainName[1], 0);
                usermanagement.SelectGroup(GroupName[5], DomainName[1]);
                if (!usermanagement.SelectSubGroup(GroupName[5], SubGroupName[2]))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateSubGroup(GroupName[5], SubGroupName[2]);
                }


                bool result1 = false;
                if (usermanagement.SearchGroup(GroupName[3], DomainName[1], 0) && usermanagement.SearchGroup(GroupName[4], DomainName[1], 0) && usermanagement.SearchGroup(GroupName[5], DomainName[1], 0) && usermanagement.SearchGroup(SubGroupName[0], DomainName[1], 1) && usermanagement.SearchGroup(SubGroupName[1], DomainName[1], 1) && usermanagement.SearchGroup(SubGroupName[2], DomainName[1], 1))
                {
                    result1 = true;
                }

                domain = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForPageLoad(10);
                if (!domain.SearchDomain(DomainName[2]))
                {
                    domain.CreateDomain(DomainName[2], Rolename, datasources: datasource);
                    PageLoadWait.WaitForPageLoad(30);
                    PageLoadWait.WaitForFrameLoad(10);
                    domain.ClickSaveDomain();
                }

                PageLoadWait.WaitForPageLoad(10);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                // ExecutedSteps++;

                // Adding MarketDomain3 Groups and sub groups

                usermanagement.ClearBtn().Click();
                if (!usermanagement.SearchGroup(GroupName[6], DomainName[2], 0))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateGroup(DomainName[2], GroupName[6], selectalldatasources: 0);
                }
                usermanagement.ClearBtn().Click();
                if (!usermanagement.SearchGroup(GroupName[7], DomainName[2], 0))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateGroup(DomainName[2], GroupName[7], selectalldatasources: 0, selectallroles: 0);
                }
                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain(DomainName[2]);
                domain.SelectDomain(DomainName[2]);
                domain.EditDomainButton().Click();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                domain.SetCheckBoxInEditDomain("autoinherit", 0);
                domain.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(10);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.ClearBtn().Click();
                if (!usermanagement.SearchGroup(GroupName[8], DomainName[2], 0))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateGroup(DomainName[2], GroupName[8], selectalldatasources: 0);
                }
                usermanagement.ClearBtn().Click();
                usermanagement.SearchGroup(GroupName[6], DomainName[2], 0);
                usermanagement.SelectGroup(GroupName[6], DomainName[2]);
                if (!usermanagement.SelectSubGroup(GroupName[6], SubGroupName[0]))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateSubGroup(GroupName[6], SubGroupName[0]);
                }
                usermanagement.ClearBtn().Click();
                usermanagement.SearchGroup(GroupName[6], DomainName[2], 0);
                usermanagement.SelectGroup(GroupName[6], DomainName[2]);
                if (!usermanagement.SelectSubGroup(GroupName[6], SubGroupName[1]))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateSubGroup(GroupName[6], SubGroupName[1]);
                }
                usermanagement.ClearBtn().Click();
                usermanagement.SearchGroup(GroupName[6], DomainName[2], 0);
                usermanagement.SelectGroup(GroupName[6], DomainName[2]);
                if (!usermanagement.SelectSubGroup(GroupName[6], SubGroupName[2]))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateSubGroup(GroupName[6], SubGroupName[2]);
                }
                usermanagement.ClearBtn().Click();
                usermanagement.SearchGroup(GroupName[7], DomainName[2], 0);
                usermanagement.SelectGroup(GroupName[7], DomainName[2]);
                if (!usermanagement.SelectSubGroup(GroupName[7], SubGroupName[0]))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateSubGroup(GroupName[7], SubGroupName[0]);
                }
                usermanagement.ClearBtn().Click();
                usermanagement.SearchGroup(GroupName[7], DomainName[2], 0);
                usermanagement.SelectGroup(GroupName[7], DomainName[2]);
                if (!usermanagement.SelectSubGroup(GroupName[7], SubGroupName[1]))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateSubGroup(GroupName[7], SubGroupName[1]);
                }


                usermanagement.ClearBtn().Click();
                usermanagement.SearchGroup(GroupName[7], DomainName[2], 0);
                usermanagement.SelectGroup(GroupName[7], DomainName[2]);
                if (!usermanagement.SelectSubGroup(GroupName[7], SubGroupName[2]))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateSubGroup(GroupName[7], SubGroupName[2]);
                }


                usermanagement.ClearBtn().Click();
                usermanagement.SearchGroup(GroupName[8], DomainName[2], 0);
                usermanagement.SelectGroup(GroupName[8], DomainName[2]);
                if (!usermanagement.SelectSubGroup(GroupName[8], SubGroupName[0]))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateSubGroup(GroupName[8], SubGroupName[0]);
                }
                usermanagement.ClearBtn().Click();
                usermanagement.SearchGroup(GroupName[8], DomainName[2], 0);
                usermanagement.SelectGroup(GroupName[8], DomainName[2]);
                if (!usermanagement.SelectSubGroup(GroupName[8], SubGroupName[1]))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateSubGroup(GroupName[8], SubGroupName[1]);
                }
                usermanagement.ClearBtn().Click();
                usermanagement.SearchGroup(GroupName[8], DomainName[2], 0);
                usermanagement.SelectGroup(GroupName[8], DomainName[2]);
                if (!usermanagement.SelectSubGroup(GroupName[8], SubGroupName[2]))
                {
                    PageLoadWait.WaitForPageLoad(10);
                    usermanagement.CreateSubGroup(GroupName[8], SubGroupName[2]);
                }

                bool result2 = false;
                if (usermanagement.SearchGroup(GroupName[6], DomainName[2], 0) && usermanagement.SearchGroup(GroupName[7], DomainName[2], 0) && usermanagement.SearchGroup(GroupName[8], DomainName[2], 0) && usermanagement.SearchGroup(SubGroupName[0], DomainName[2], 1) && usermanagement.SearchGroup(SubGroupName[1], DomainName[2], 1) && usermanagement.SearchGroup(SubGroupName[2], DomainName[2], 1))
                {
                    result2 = true;
                }


                if (result1 && result2)
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
        
        /// <summary>
        /// Setup and Configure Environment
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27665(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables          

            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            String destpacs = login.GetHostName(Config.DestinationPACS);


            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int Executedsteps = -1;

            try
            {
                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String user1ID = "user1.md1h1.m6";
                String user2ID = "user2.md1h1.m6";
                String pwd = "pwd.13579";
                String destination = "NORDest";
                String domainB = "MarketDomain1";
                String ReceiverName = "user1";
                String ArchivistName = "user2";
                EIWindowName = "Exam Importer for TenetMD" + new Random().Next(1, 1000) + " Setup";

                result.steps[++Executedsteps].status = "Not Automated";
                login.LoginIConnect(adminUserName, adminPassword);
                Executedsteps++;

                //Create Destination for DomainB
                var imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                var pagedestination = (Image_Sharing.Destination)imagesharing.NavigateToSubTab("Destination");
                if (!(pagedestination.SearchDestination(domainB, destination)))
                { pagedestination.CreateDestination(destpacs, user1ID, user2ID, destination, domainB, ReceiverName, ArchivistName); }


                //Generate EI Installer
                var servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Image Sharing");
                servicetool.GenerateInstallerExamImporter(domainB, EIWindowName);
                servicetool.RestartService();
                servicetool.CloseServiceTool();

                //Install EI
                var ei = new ExamImporter();
                ei.eiWinName = EIWindowName;
                LDAPUploaderToolPath = ei.EI_Installation(domainB, EIWindowName, Config.Inst1, user1ID, pwd);
                Logger.Instance.InfoLog("The Window name of EI is" + EIWindowName);
                Executedsteps++;

                //Report Result
                result.FinalResult(Executedsteps);
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
                result.FinalResult(e, Executedsteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }
        
        public TestCaseResult Test_27666(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            TestCaseResult result;
            BasePage basepage = new BasePage();
            ServiceTool tool = new ServiceTool();
            Taskbar taskbar = new Taskbar();
            ExamImporter ei = new ExamImporter();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            String UserName = Config.adminUserName;
            String Password = Config.adminPassword;
            String PhUsername = Config.ph1UserName;
            String PhPassword = Config.ph1Password;
            String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
            String Studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
            String eiWindow = "ExamImporter_" + new Random().Next(1000);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {

                /*//Install EI
                 String eipath = "C:\\Users\\Administrator\\AppData\\Local\\Apps\\EIMarketDomain1\\bin\\UploaderTool.exe";
                 ei.EI_Installation("MarketDomain1", "EI" + "MarketDomain1", Config.Inst1, "user1.md1h1.m6", "pwd.13579");
                 Logger.Instance.InfoLog("The Window name of EI is" + "EI" + "MarketDomain1");*/

                ei.eiWinName = EIWindowName;
                if (String.IsNullOrEmpty(LDAPUploaderToolPath))
                {
                    ei.EIDicomUpload("user1.md1h1.m6", "pwd.13579", "NORDest", Studypath, 1, LDAPUploaderToolPath);
                }
                else { throw new Exception("LDAP Exam Importer not installed"); }
                ExecutedSteps++;

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
        }

        /// <summary>
        /// Multiple Domains/ Image Sharing: A User archives a study from the Inbound/Viewer with no reconciliation.
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27667(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables           
            StudyViewer studyviewer = null;
            Inbounds inbounds = null;
            TestCaseResult result;
            Studies studies;
            RoleManagement rolemanagement;
            string PacsA3 = "PA-A3-WS8";
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int Executedsteps = -1;

            try
            {
                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PateintID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String[] Accession = AccessionList.Split(':');
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRole = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String NominateReason = "Interpretation Required";
                String datasource = login.GetHostName(Config.SanityPACS);
                String Testdomain = "Test83_DomainA" + new Random().Next(1, 1000);
                String TestdomainAdmin = "Test83_DomainAdminA" + new Random().Next(1, 1000);
                String Testrole1 = "Test83_Role1A" + new Random().Next(1, 1000);
                String Testrole2 = "Test83_Role2A" + new Random().Next(1, 1000);
                String Testuser1 = "Test83_User1A" + new Random().Next(1, 1000);
                String Testuser2 = "Test83_User2A" + new Random().Next(1, 1000);

                String user1ID = "user1.md1h1.m6";
                String user2ID = "user2.md1h1.m6";
                String pwd = "pwd.13579";

                //Pre-Condition: (for making user user1.md1h1.m6 as both reciever and archivist)
                login.LoginIConnect(adminUserName, adminPassword);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                rolemanagement.SearchRole("MarketDomain1Physician");
                rolemanagement.SelectRole("MarketDomain1Physician");
                rolemanagement.EditRoleByName("MarketDomain1Physician");
                rolemanagement.SetCheckboxInEditRole("archive", 0);
                rolemanagement.ClickSaveEditRole();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                login.Logout();

                //Step-1:The Receiver user1.md1h1.m6 logs in and navigates to the Inbound tab and searches for studies
                login.LoginIConnect(user1ID, pwd);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                //Search and Select Study
                inbounds.SearchStudy("Accession", "*");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                Dictionary<int, string[]> results = BasePage.GetSearchResults();
                string[] columnnames = BasePage.GetColumnNames();
                string[] columnvalues = BasePage.GetColumnValues(results, "Status", columnnames);
                if (results.Count > 1 && columnvalues != null)
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }


                //Step-2:Select a study with Status - Uploaded and Click on "Nominated For Archiving"
                inbounds.SelectStudy("Accession", Accession[0]);
                String studyStatus2;
                inbounds.GetMatchingRow("Accession", Accession[0]).TryGetValue("Status", out studyStatus2);
                IWebElement OrderField, ReasonField;
                inbounds.ClickNominateButton(out ReasonField, out OrderField);
                if (inbounds.NominateDiv().Displayed && OrderField.Text == "" && ReasonField.Text == "")
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }



                //Step-3: Enter some text in the order notes, Select a Reason and click on  OK.
                String OrderNote = "Testing";
                OrderField.SendKeys(OrderNote);
                SelectElement selector = new SelectElement(ReasonField);
                selector.SelectByText(NominateReason);
                Logger.Instance.InfoLog("Reason Field is filled Sucessfully");
                inbounds.ClickConfirmNominate();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                Dictionary<string, string> studyStatus3 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession[0], "Nominated For Archive" });
                if (studyStatus3 != null)
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[Executedsteps].description);

                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }



                //Step-4: Select a study with Status - Uploaded and Click on "Archive Study"
                inbounds.SelectStudy("Accession", Accession[1]);
                IWebElement UploadCommentsField, ArchiveOrderField;
                inbounds.ClickArchiveStudy(out UploadCommentsField, out ArchiveOrderField);

                if (UploadCommentsField.Displayed && ArchiveOrderField.Displayed)
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[Executedsteps].description);

                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }
                //result.steps[++Executedsteps].status = "Not Automated";


                //Step-5: Enter some text in the order notes, Select a Reason and click on  OK
                UploadCommentsField.SendKeys("test");
                ArchiveOrderField.SendKeys("");
                inbounds.ClickArchive();
                String studyState1;
                inbounds.GetMatchingRow("Accession", Accession[1]).TryGetValue("Status", out studyState1);

                if (studyState1 == "Archiving" || studyState1 == "Routing Completed")
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[Executedsteps].description);
                }
                login.Logout();

                //Step-6: Check the email for the Archivist and the admin.
                //In the email  there is Confirmation  the study was archived.
                result.steps[++Executedsteps].status = "Not Automated";


                //Step-7: Click on the link in the Email and continue to login and View the Study
                result.steps[++Executedsteps].status = "Not Automated";

                //Step-8:Close the browser opened from the email and log back in as the archivist.
                //(user2.md1h1.m6) Navigate to the Inbound tab

                login.LoginIConnect(user2ID, pwd);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                //Search and Select Study
                inbounds.SearchStudy("Accession", "*");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                String studyState81;
                inbounds.GetMatchingRow("Accession", Accession[0]).TryGetValue("Status", out studyState81);
                String studyState82;
                inbounds.GetMatchingRow("Accession", Accession[1]).TryGetValue("Status", out studyState82);
                Dictionary<string, string> studyState83 = inbounds.GetMatchingRow(new string[] { "Accession", "Reconciliation State" }, new string[] { Accession[0], "No Matching Order" });
                Dictionary<string, string> studyState84 = inbounds.GetMatchingRow(new string[] { "Accession", "Reconciliation State" }, new string[] { Accession[1], "No Matching Order" });
                if (studyState81 == "Nominated For Archive" && studyState82 == "Routing Completed" && studyState83 != null && studyState84 != null)
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[Executedsteps].description);
                }
                //login.Logout();

                //Step-9: Select the Study with Status Nominated for Archiving and Click on the View Order 
                //Notes on the bottom of the page.
                inbounds.SelectStudy("Accession", Accession[0]);
                inbounds.ViewOrderNotes();
                if (inbounds.ViewOrdersNoteStudyDetailsTB().Displayed
                    && inbounds.ViewOrderNotesOrderNotesTB().Displayed && inbounds.ViewOrderNotesReason().Displayed)
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[Executedsteps].description);
                }
                inbounds.CloseViewOrderNotes().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //Step-10: Select the Study with Reconciling Status Routed for Reconciling  and Click on 
                //the View Order Notes on the bottom of the page
                inbounds.SelectStudy("Accession", Accession[1]);
                inbounds.ViewOrderNotes();
                if (inbounds.ViewOrdersNoteStudyDetailsTB().Displayed
                     && inbounds.ViewOrderNotesOrderNotesTB().Displayed && inbounds.ViewOrderNotesReason().Displayed)
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[Executedsteps].description);
                }
                inbounds.CloseViewOrderNotes().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                login.Logout();

                //Step-11:Re Login in as the Receiver and Select one study from the Inbound list  with 
                //Status Uploaded  and open it in the viewer (user1.md1h1.m6)
                login.LoginIConnect(user1ID, pwd);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                //Search and Select Study
                inbounds.SearchStudy("Accession", Accession[1]);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                inbounds.SelectStudy("Accession", Accession[1]);
                studyviewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (studyviewer.SeriesViewer_1X1().Displayed)
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[Executedsteps].description);
                }


                //Step-12: click on "Archive Study"
                studyviewer.SelectToolInToolBar(IEnum.ViewerTools.ArchiveStudy);
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ReconciliationControlDialogDiv")));
                if (studyviewer.ArchiveOrderField().Displayed && studyviewer.ArchiveReasonlist().Count == 2)
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[Executedsteps].description);
                }

                //Step-13: Select a reason from the dropdown and add some notes click on nominate
                //new SelectElement(Driver.FindElement(By.CssSelector("#m_ReconciliationControl_m_reasonSelector"))).SelectByText(option);
                studyviewer.ArchiveReasonlist()[0].Click();
                studyviewer.ClickArchive();
                studyviewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                String studyState13;
                inbounds.GetMatchingRow("Accession", Accession[2]).TryGetValue("Status", out studyState13);
                if (studyState13 == "Archiving" || studyState13 == "Routed  for Reconciliation")
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[Executedsteps].description);
                }

                //Step-14: Confirm Email is received by the archivist
                result.steps[++Executedsteps].status = "Not Automated";

                //Step-15: While still logged in as the Receiver select another study with status - uploaded and 
                //open it in the viewer
                inbounds.SearchStudy("Accession", Accession[3]);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                inbounds.SelectStudy("Accession", Accession[3]);
                studyviewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (studyviewer.SeriesViewer_1X1().Displayed)
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[Executedsteps].description);
                }

                //Step-16: Select the Nominate for Archiving and enter some notes, select the reason for archiving
                //and click on OK to complete
                inbounds.Nominatestudy_toolbar();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                String studyState16;
                inbounds.GetMatchingRow("Accession", Accession[3]).TryGetValue("Status", out studyState16);
                if (studyState16 == "Nominated For Archive")
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[Executedsteps].description);
                }
                login.Logout();

                //Step-17: Login as the Archivist for this Destination and navigate to the Inbound Tab (user2.md1h1.m6)
                login.LoginIConnect(user2ID, pwd);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                //Search and Select Study
                inbounds.SearchStudy("Accession", "*");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                foreach (String Acc in Accession)
                {
                    String studyState17;
                    inbounds.GetMatchingRow("Accession", Acc).TryGetValue("Status", out studyState17);
                    if (studyState17 == "Nominated For Archive" || studyState17 == "Routing Completed" || studyState17 == "Archiving")
                    {
                        result.steps[++Executedsteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[Executedsteps].description);
                    }
                    else
                    {
                        result.steps[++Executedsteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[Executedsteps].description);
                        break;
                    }
                }


                //Step-18: Select a study that has been nominated for Archiving and Click on "Archive Study".
                inbounds.SelectStudy("Accession", Accession[3]);
                //Details of a study
                Dictionary<string, string> rowValues = inbounds.GetMatchingRow("Accession", Accession[3]);
                IWebElement UploadCommentsField18, ArchiveOrderField18;
                inbounds.ClickArchiveStudy(out UploadCommentsField18, out ArchiveOrderField18);
                inbounds.ArchiveSearch("order", "All Dates");
                //Details in Original details column
                Dictionary<String, String> OriginalDetails = inbounds.GetDataInArchive("Original Details");
                //Details in Final details column 
                Dictionary<String, String> FinalDetails = inbounds.GetDataInArchive("Final Details");
                //Validate the details in original details column are in sync with study details 
                if ((OriginalDetails["Last Name"].Equals(rowValues["Last Name"])) && (OriginalDetails["First Name"].Equals(rowValues["First Name"])) &&
                    (OriginalDetails["Gender"].Equals(rowValues["Gender"])) && (OriginalDetails["DOB"].Equals(rowValues["Patient DOB"])) &&
                    (OriginalDetails["Issuer of PID"].Equals(rowValues["Issuer of PID"])) && (OriginalDetails["PID / MRN"].Equals(rowValues["Patient ID"])) &&
                    (OriginalDetails["Description"].Equals(rowValues["Description"])) && (OriginalDetails["Study Date"].Equals(rowValues["Study Date"])) &&
                    (OriginalDetails["Accession"].Equals(rowValues["Accession"])) &&
                    (OriginalDetails["Last Name"].Equals(FinalDetails["Last Name"])) && (OriginalDetails["First Name"].Equals(FinalDetails["First Name"])) &&
                    (OriginalDetails["Gender"].Equals(FinalDetails["Gender"])) && (OriginalDetails["DOB"].Equals(FinalDetails["Patient DOB"])) &&
                    (OriginalDetails["Issuer of PID"].Equals(FinalDetails["Issuer of PID"])) && (OriginalDetails["PID / MRN"].Equals(FinalDetails["Patient ID"])) &&
                    (OriginalDetails["Description"].Equals(FinalDetails["Description"])) && (OriginalDetails["Study Date"].Equals(FinalDetails["Study Date"])) &&
                    (OriginalDetails["Accession"].Equals(FinalDetails["Accession"])))
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }


                //Step-19: Change the Created Dates to "All Dates" and click on Search.
                inbounds.ArchiveSearch("order", "All Dates");
                //Details in Matching Order column
                Dictionary<String, String> OrderDetails = inbounds.GetDataInArchive("Matching Order");

                if (OrderDetails["Last Name"].Equals(""))
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }



                //Step-20: Select one and Click on the Archive Button
                inbounds.ClickArchive();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                String studyState20;
                inbounds.GetMatchingRow("Accession", Accession[2]).TryGetValue("Status", out studyState20);
                if (studyState13 == "Archiving")
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[Executedsteps].description);
                }
                login.Logout();

                //Step-21: Login in as Administrator and Navigate to the Studies tab and
                //select the destination datasource and confirm the study was archived successfully by opening it
                //in the viewer and observing the name and other attributes, PID, DOB, Accession
                login.LoginIConnect(adminUserName, adminPassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                //Search and Select Study
                inbounds.SearchStudy("Accession", "*");
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[3], Datasource: PacsA3);
                String studyState21;
                inbounds.GetMatchingRow("Accession", Accession[3]).TryGetValue("Status", out studyState21);
                if (studyState21 == "Archiving")
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[Executedsteps].description);
                }



                //Report Result
                result.FinalResult(Executedsteps);
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
                result.FinalResult(e, Executedsteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }



    }
}