using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.MergeServiceTool;
using Selenium.Scripts.Reusable.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Xml;

namespace Selenium.Scripts.Tests
{
    class Licensing : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public ExamImporter ei { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public HPHomePage hphomepage { get; set; }
        public HPLogin hplogin { get; set; }
        public ServiceTool servicetool { get; set; }

        public Licensing(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            ei = new ExamImporter();
            mpaclogin = new MpacLogin();
        }

        // Creating Domain
        String Domain1 = "Domain1_" + new Random().Next(1, 1000);
        // Creating Role 
        String Role1 = "SuperRole";
        // Creating Normal User
        String User1 = "User1_" + new Random().Next(1, 1000);
        String User2 = "User2_" + new Random().Next(1, 1000);
        String User3 = "User3_" + new Random().Next(1, 1000);
        String User4 = "User4_" + new Random().Next(1, 1000);
        String User5 = "User5_" + new Random().Next(1, 1000);
        // Creating System admin
        String SystemAdmin1 = "SADMIN1_" + new Random().Next(1, 1000);
        String SystemAdmin2 = "SADMIN2_" + new Random().Next(1, 1000);
        // Creating Domain Admin
        String DomainAdmin1 = "DADMIN3_" + new Random().Next(1, 1000);
        String DomainAdmin2 = "DADMIN4_" + new Random().Next(1, 1000);
        String DomainAdmin3 = "DADMIN5_" + new Random().Next(1, 1000);
        // Creating Roles
        String admi = "admi" + new Random().Next(1, 1000);
        String TestRoleAdmin = "TestRoleAdmin" + new Random().Next(1, 1000);
        String TestadMiN = "TestadMiN" + new Random().Next(1, 1000);
        String damin = "damin" + new Random().Next(1, 1000);

        // Setting datasources
        String DatasourceAutoSSA = new Login().GetHostName(Config.EA77);
        String DatasourceVMSSA131 = new Login().GetHostName(Config.EA1);
        String DatasourceVMSSA91 = new Login().GetHostName(Config.EA91);
        String DatasourceSanityPACS = new Login().GetHostName(Config.SanityPACS);
        String DatasourcePACS2 = new Login().GetHostName(Config.PACS2);

        public TestCaseResult Test_27652(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            Login login = new Login();
            MultiDriver = new List<IWebDriver>();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                ExecutedSteps++;
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

        public TestCaseResult Test_27653(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmanagement = null;
            UserManagement usermanagement = null;
            MultiDriver = new List<IWebDriver>();
            ServiceTool servicetool = new ServiceTool();
            WpfObjects wpfobject = new WpfObjects();
            Studies studies = null;
            Maintenance maintenance = null;
            DataTable datatable;
            String[] actual;

            string Onebase1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "1base1adminpath");
            string Onebase0adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "1base0adminpath");
            string Zerobase1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "0base1adminpath");
            string Onebase1maxusers1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "1base1maxusers1adminpath");
            string Twobase1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "2base1adminpath");
            string Threebase1admin = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "3base1admin");
            string Threemaxusers1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "3maxusers1adminpath");
            string Fourmaxusers1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "4maxusers1adminpath");
            string Licbackpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Licbackpath");


            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data                       
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;


                if (File.Exists(@"C:\WebAccess\WebAccess\Config\C4LicensedFeatureSet.xml"))
                {
                    File.Copy(@"C:\WebAccess\WebAccess\Config\C4LicensedFeatureSet.xml", @"C:\Users\Administrator\Desktop\Test Data\Non Dicom\Licensing_Testdata\Lic backup\C4LicensedFeatureSet.xml", true);
                }

                // This is the parent driver.
                BasePage.MultiDriver.Add(BasePage.Driver);
                login.SetDriver(MultiDriver[0]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);

                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForPageLoad(10);
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(10);
                //// Step 1
                //// Create new Domain group D1 (default SuperAdminGroup) under Domain Management tab.
                //// In Domain D1:
                //// Create 5 users with Type as User.Referred to as USER1...USER5
                ////Create 2 System Admin users with Type as SuperAdmin.Referred to SADMIN1 and SADMIN2
                //// Create 3 Domain Admin users with Type as DomainAdmin.Referred to DADMIN3, DADMIN4, DADMIN5
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(10);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForPageLoad(10);
                //if (!domainmanagement.SearchDomain(Domain1))
                //{
                //    domainmanagement.CreateDomain(Domain1, Role1, DS: new string[] { "print" }, datasources: new string[] { DatasourceAutoSSA, DatasourceVMSSA131, DatasourceVMSSA91, DatasourceSanityPACS, DatasourcePACS2 }, check: 1);
                //}

                domainmanagement.SearchDomain("SuperAdminGroup");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.EditDomainButton().Click();
                Driver.SwitchTo().DefaultContent();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForPageLoad(10);
                domainmanagement.VisibleAllStudySearchField();
                domainmanagement.SaveDomainButtoninEditPage().Click();

                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(10);
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                PageLoadWait.WaitForPageLoad(10);

                if (!usermanagement.SearchUser(User1))
                {
                    usermanagement.CreateUser(User1, Role1);
                }

                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(User2))
                {
                    usermanagement.CreateUser(User2, Role1);
                }
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(User3))
                {
                    usermanagement.CreateUser(User3, Role1);
                }
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(User4))
                {
                    usermanagement.CreateUser(User4, Role1);
                }
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(User5))
                {
                    usermanagement.CreateUser(User5, Role1);
                }

                if (!usermanagement.SearchUser(SystemAdmin1, "SuperAdminGroup"))
                {
                    usermanagement.CreateSystemAdminUser(SystemAdmin1, "SuperAdminGroup");
                }

                if (!usermanagement.SearchUser(SystemAdmin2, "SuperAdminGroup"))
                {
                    usermanagement.CreateSystemAdminUser(SystemAdmin2, "SuperAdminGroup");
                }
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(DomainAdmin1, "SuperAdminGroup"))
                {
                    usermanagement.CreateDomainAdminUser(DomainAdmin1, "SuperAdminGroup");
                }
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(DomainAdmin2, "SuperAdminGroup"))
                {
                    usermanagement.CreateDomainAdminUser(DomainAdmin2, "SuperAdminGroup");
                }

                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(DomainAdmin3, "SuperAdminGroup"))
                {
                    usermanagement.CreateDomainAdminUser(DomainAdmin3, "SuperAdminGroup");
                }

                login.Logout();
                PageLoadWait.WaitForPageLoad(10);


                // Close all the iConnect Access windows on the test server and do iisreset.
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();

                // Delete..\WINDOWS\Temp\WebAccessLicense - YYYY - MM.log  where YYYY is the current year and MM is the current month
                String Month = DateTime.Now.Month.ToString();
                String Year = DateTime.Now.Year.ToString();
                if (File.Exists("C:\\Windows\\Temp\\WebAccessLicense-" + Year + "-" + Month + ".log"))
                {
                    File.Delete("C:\\Windows\\Temp\\WebAccessLicense-" + Year + "-" + Month + ".log");
                }


                String NodePath = @"Configuration/LicenseUsageLoggingInterval";
                // Change the LicenseUsageLoggingInterval from 60 minutes to 1 minute (refer to Pre-Condition 9)
                ChangeNodeValue("C:\\WebAccess\\WebAccess\\Config\\SystemConfiguration.xml", NodePath, "1");
                ExecutedSteps++;

                // Step 2
                // Test Data: If license was already installed, can do the following: 
                // -Make a backup copy of the license file and modify the License file,
                // in:"License.xml"modify one character in each CDATA[...] line so that the license is not valid.
                // Open the Merge iConnect service tool and apply this License.

                // Backuping license file
                if (File.Exists("D:\\C4LicensedFeatureSet.xml"))
                {
                    File.Copy("D:\\C4LicensedFeatureSet.xml", "C:\\Users\\Administrator\\Desktop\\Test Data\\Non Dicom\\Licensing_Testdata\\Lic backup\\C4LicensedFeatureSet.xml", true);
                }


                if (File.Exists("C:\\WebAccess\\WebAccess\\Config\\License.xml"))
                {
                    if (!System.IO.Directory.Exists("C:\\WadoWS_Temp"))
                    {
                        System.IO.Directory.CreateDirectory("C:\\WadoWS_Temp");
                    }

                    if (File.Exists("C:\\WadoWS_Temp\\License.xml"))
                    {
                        File.Delete("C:\\WadoWS_Temp\\License.xml");
                        File.Copy("C:\\WebAccess\\WebAccess\\Config\\License.xml", "C:\\WadoWS_Temp\\License.xml");
                    }
                    else
                    {
                        File.Copy("C:\\WebAccess\\WebAccess\\Config\\License.xml", "C:\\WadoWS_Temp\\License.xml");
                    }
                    ChangeCDATAValueInXml("C:\\WebAccess\\WebAccess\\Config\\License.xml", "Invalid License Key");
                    servicetool.InvokeServiceTool();
                    wpfobject.WaitTillLoad();
                    servicetool.RestartService();
                    wpfobject.WaitTillLoad();
                    servicetool.CloseServiceTool();
                }
                else
                {
                    servicetool.InvokeServiceTool();
                    wpfobject.WaitTillLoad();
                    servicetool.AddLicenseInConfigTool(ServiceTool.License.FilePath);
                    wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                    servicetool.RestartService();
                }

                login.LoginGrid(Username, Password);
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


                // Step 3
                //Login from CLIENTSYS1 as SADMIN1
                // Refer to Pre - condition 7, and verify both methods give the same result.
                Config.node = Config.Clientsys1;
                BasePage.MultiDriver.Add(login.InvokeBrowser("Remote-" + Config.BrowserType));
                login.DriverGoTo(login.url);
                login.LoginGrid(SystemAdmin1, SystemAdmin1);
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

                // Step 4
                // License the server using the 1 webaccess.base, 1 webaccess.admin license valid for 0 days.
                // Precondition 7 Perform iisreset
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.AddLicenseInConfigTool(Onebase1adminpath);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                // Step 5
                // Test Data: 
                // 0 means permanent license
                // Login from WASYS as Administrator.
                // Refer to Pre-condition 7, and verify both methods give the same result.
                login.SetDriver(BasePage.MultiDriver[0]);
                login.LoginGrid(Username, Password);
                if (login.IsTabPresent("Domain Management"))
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

                // Step 6
                // License the server using the 1 webaccess.base, 1 webaccess.admin license.
                // Precondition 7
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.AddLicenseInConfigTool(Onebase1adminpath);
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                // Step 7 
                // Login from WASYS as Administrator.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
                if (login.IsTabPresent("Domain Management"))
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

                // Step 8
                // Logout from WASYS as Administrator.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                login.Logout();
                ExecutedSteps++;

                // Step 9 
                // Login from WASYS as Administrator using an incorrect password.
                login.LoginGrid(Username, Password + "incorrect");
                PageLoadWait.WaitForPageLoad(10);
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

                // Step 10
                // Login from WASYS as a non-existant user.
                login.LoginGrid(Username + "123", Password);
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

                // Step 11
                // Test Data: 
                // Test only 1 webaccess.admin session permitted
                // Login from WASYS as Administrator.
                // Refer to Pre-condition 7, and verify both methods give the same result.
                // Expected result : Login successful. 1 webaccess.base license is used.0 webaccess.admin license is used.
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.AddLicenseInConfigTool("C:\\Users\\Administrator\\Desktop\\Test Data\\Non Dicom\\Licensing_Testdata\\0b1a\\C4LicensedFeatureSet.xml");
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();

                login.SetDriver(BasePage.MultiDriver[0]);
                PageLoadWait.WaitForPageLoad(10);
                login.LoginGrid(Username, Password);
                PageLoadWait.WaitForPageLoad(10);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                int WebAdmincount = actual.Where(val => string.Equals(val, "webaccess.admin 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (WebAdmincount == 1)
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


                // Step 12
                // Test Data: 
                // If license installed as 1 webaccess.base, 1 webaccess.admin, no webaccess.maxuser installed, 
                // the system will display a message"Using Admin Session License"
                // Login from CLIENTSYS1 as Administrator.
                // Refer to Pre-condition 7, and verify both methods give the same result.
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.AddLicenseInConfigTool(Onebase1adminpath);
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();

                login.SetDriver(BasePage.MultiDriver[0]);
                PageLoadWait.WaitForPageLoad(10);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
                PageLoadWait.WaitForPageLoad(10);

                login.SetDriver(BasePage.MultiDriver[1]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
                //BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                //bool UsingAdminLabelDisplayed = BasePage.Driver.FindElement(By.CssSelector("#ctl00_AlertMessageLabel")).Text.Equals("Using Admin License");
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                WebAdmincount = actual.Where(val => string.Equals(val, "webaccess.admin 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                int BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (WebAdmincount == 1 && BaseAdmincount == 1)
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

                // Step 13
                // Login from CLIENTSYS2 as USER2.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                Config.node = Config.Clientsys2;
                BasePage.MultiDriver.Add(login.InvokeBrowser("Remote-" + Config.BrowserType));
                login.DriverGoTo(login.url);
                login.LoginGrid(User2, User2);
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


                // Step 14 
                // Login from CLIENTSYS2 as SADMIN1.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                login.SetDriver(MultiDriver[2]);
                login.DriverGoTo(login.url);
                login.LoginGrid(SystemAdmin1, SystemAdmin1);
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

                // Step 15
                // Test Data: 
                // Generally, the webaccess.admin shouldn't be used if there are available webaccess.base licenses, 
                // but this is still used until the session on CLIENTSYS1 renews its license.
                // Logout from a CLIENTSYS1 as Administrator.
                // Refer to Pre-condition 7, and verify both methods give the same result.
                // Multidriver Index is 1 for clientsys1

                login.SetDriver(BasePage.MultiDriver[1]);
                login.Logout();

                login.SetDriver(MultiDriver[0]);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 1)
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

                // Step 16
                // Logout from WASYS as Administrator.
                // Refer to Pre-condition 7, and verify both methods give the same result.
                login.SetDriver(BasePage.MultiDriver[0]);
                login.Logout();
                if (login.UserIdTxtBox().Displayed)
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

                // Step 17
                // Test Data: 
                // Test webaccess.base used when available.
                // Login from WASYS as Administrator.
                // Refer to Pre-condition 7, and verify both methods give the same result.
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.AddLicenseInConfigTool(Onebase1adminpath);
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                wpfobject.WaitTillLoad();

                login.SetDriver(MultiDriver[0]);
                login.LoginGrid(Username, Password);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 1)
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

                // Step 18
                // Logout from WASYS as Administrator.
                //Refer to Pre-condition 7, and verify both methods give the same result.
                login.SetDriver(MultiDriver[0]);
                login.Logout();
                if (login.UserIdTxtBox().Displayed)
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

                // Step 19
                // Login from CLIENTSYS1 as USER1.
                // Refer to Pre - condition 7, and verify both methods give the same result.

                login.SetDriver(BasePage.MultiDriver[1]);
                login.DriverGoTo(login.url);
                login.LoginGrid(User1, User1);

                // MultDriver index wil be 3
                Config.node = Config.Clientsys1;
                BasePage.MultiDriver.Add(login.InvokeBrowser("Remote-" + Config.BrowserType));
                login.SetDriver(BasePage.MultiDriver[3]);
                login.DriverGoTo(login.url);
                PageLoadWait.WaitForPageLoad(10);
                login.LoginGrid(Username, Password);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                WebAdmincount = actual.Where(val => string.Equals(val, "webaccess.admin 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 1 && WebAdmincount == 1)
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

                // Step 20
                // Logout from CLIENTSYS1 as USER1.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                login.SetDriver(MultiDriver[1]);
                login.Logout();
                if (login.UserIdTxtBox().Displayed)
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

                // Step 21
                // Login from CLIENTSYS2 as USER2.
                // Refer to Pre-condition 7, and verify both methods give the same result.

                login.SetDriver(BasePage.MultiDriver[2]);
                login.DriverGoTo(login.url);
                login.LoginGrid(User2, User2);
                PageLoadWait.WaitForPageLoad(10);

                login.SetDriver(BasePage.MultiDriver[3]);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                WebAdmincount = actual.Where(val => string.Equals(val, "webaccess.admin 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 1 && WebAdmincount == 1)
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

                // Step 22
                // Logout Administrator.  Login from CLIENTSYS3 as SADMIN1.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                login.SetDriver(BasePage.MultiDriver[3]);
                login.Logout();
                PageLoadWait.WaitForPageLoad(10);

                // Login from CLIENTSYS3 as SADMIN1.
                Config.node = Config.Clientsys3;
                BasePage.MultiDriver.Add(login.InvokeBrowser("Remote-" + Config.BrowserType));
                login.SetDriver(BasePage.MultiDriver[4]);
                login.DriverGoTo(login.url);
                login.LoginGrid(SystemAdmin1, SystemAdmin1);

                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                WebAdmincount = actual.Where(val => string.Equals(val, "webaccess.admin 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 1 && WebAdmincount == 1)
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

                // Step 23
                // Login from CLIENTSYS2 as Administrator.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                Config.node = Config.Clientsys2;
                BasePage.MultiDriver.Add(login.InvokeBrowser("Remote-" + Config.BrowserType));
                login.SetDriver(BasePage.MultiDriver[5]);
                login.DriverGoTo(login.url);
                PageLoadWait.WaitForPageLoad(10);
                login.LoginGrid(Username, Password);
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

                // Step 24
                // Login from CLIENTSYS2 as USER2.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                login.DriverGoTo(login.url);
                login.LoginGrid(User1, User1);
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
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.AddLicenseInConfigTool(Licbackpath);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                Config.node = "localhost";
                closeallbrowser();
                login.InvokeBrowser(Config.BrowserType);
            }

        }

        public TestCaseResult Test_27654(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            Login login = new Login();
            result = new TestCaseResult(stepcount);
            DomainManagement domainmanagement = null;
            Maintenance maintenance = null;
            UserManagement usermanagement = null;
            Studies studies = new Studies();
            ServiceTool servicetool = new ServiceTool();
            WpfObjects wpfobject = new WpfObjects();
            int ExecutedSteps = -1;
            int BaseAdmincount = 0;
            DataTable datatable;
            String[] actual;

            string Onebase1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "1base1adminpath");
            string Onebase0adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "1base0adminpath");
            string Zerobase1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "0base1adminpath");
            string Onebase1maxusers1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "1base1maxusers1adminpath");
            string Twobase1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "2base1adminpath");
            string Threebase1admin = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "3base1admin");
            string Threemaxusers1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "3maxusers1adminpath");
            string Fourmaxusers1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "4maxusers1adminpath");
            string Licbackpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Licbackpath");
            MultiDriver = new List<IWebDriver>();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data                       
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;


                // Install License with one base 
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.AddLicenseInConfigTool(Onebase0adminpath);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();

                //Constructor driver is invoked and the driver is set with index 0
                BasePage.MultiDriver.Add(BasePage.Driver);
                login.SetDriver(MultiDriver[0]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
                Driver.SwitchTo().DefaultContent();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#UserHomeFrame")));
                Driver.SwitchTo().Frame(Driver.FindElement(By.CssSelector("#UserHomeFrame")));
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForPageLoad(10);
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(10);
                // Step 1
                // Create new Domain group D1 (default SuperAdminGroup) under Domain Management tab.
                // In Domain D1:
                // Create 5 users with Type as User.Referred to as USER1...USER5
                //Create 2 System Admin users with Type as SuperAdmin.Referred to SADMIN1 and SADMIN2
                // Create 3 Domain Admin users with Type as DomainAdmin.Referred to DADMIN3, DADMIN4, DADMIN5
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(10);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForPageLoad(10);
                //if (!domainmanagement.SearchDomain("SuperAdminGroup"))
                //{
                //    domainmanagement.CreateDomain("SuperAdminGroup", Role1, DS: new string[] { "print" }, datasources: new string[] { DatasourceAutoSSA, DatasourceVMSSA131, DatasourceVMSSA91, DatasourceSanityPACS, DatasourcePACS2 }, check: 1);
                //}

                domainmanagement.SearchDomain("SuperAdminGroup");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.EditDomainButton().Click();
                Driver.SwitchTo().DefaultContent();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForPageLoad(10);
                domainmanagement.VisibleAllStudySearchField();
                domainmanagement.SaveDomainButtoninEditPage().Click();

                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(10);
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                PageLoadWait.WaitForPageLoad(10);

                if (!usermanagement.SearchUser(User1))
                {
                    usermanagement.CreateUser(User1, Role1);
                }

                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(User2))
                {
                    usermanagement.CreateUser(User2, Role1);
                }
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(User3))
                {
                    usermanagement.CreateUser(User3, Role1);
                }
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(User4))
                {
                    usermanagement.CreateUser(User4, Role1);
                }
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(User5))
                {
                    usermanagement.CreateUser(User5, Role1);
                }

                if (!usermanagement.SearchUser(SystemAdmin1, "SuperAdminGroup"))
                {
                    usermanagement.CreateSystemAdminUser(SystemAdmin1, "SuperAdminGroup");
                }

                if (!usermanagement.SearchUser(SystemAdmin2, "SuperAdminGroup"))
                {
                    usermanagement.CreateSystemAdminUser(SystemAdmin2, "SuperAdminGroup");
                }
                //usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                //if (!usermanagement.SearchUser(DomainAdmin1))
                //{
                //    usermanagement.CreateDomainAdminUser(DomainAdmin1, "SuperAdminGroup");
                //}
                //usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                //if (!usermanagement.SearchUser(DomainAdmin2))
                //{
                //    usermanagement.CreateDomainAdminUser(DomainAdmin2, "SuperAdminGroup");
                //}
                //usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                //if (!usermanagement.SearchUser(DomainAdmin3))
                //{
                //    usermanagement.CreateDomainAdminUser(DomainAdmin3, "SuperAdminGroup");
                //}

                login.Logout();
                PageLoadWait.WaitForPageLoad(10);

                // Step 1 
                // Login from WASYS as Administrator.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                login.SetDriver(BasePage.MultiDriver[0]);
                login.DriverGoTo(login.url);
                PageLoadWait.WaitForPageLoad(10);
                login.LoginGrid(Username, Password);
                PageLoadWait.WaitForPageLoad(10);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 1)
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

                // Step 2
                // Login from CLIENTSYS1 as Administrator.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                Config.node = Config.Clientsys1;
                BasePage.MultiDriver.Add(login.InvokeBrowser("Remote-" + Config.BrowserType));
                login.SetDriver(MultiDriver[1]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
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

                // Step 3
                // Login from CLIENTSYS2 as USER2.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                Config.node = Config.Clientsys2;
                BasePage.MultiDriver.Add(login.InvokeBrowser("Remote-" + Config.BrowserType));
                login.SetDriver(BasePage.MultiDriver[2]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
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


                // Step 4
                // Logout from WASYS as Administrator.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                login.SetDriver(MultiDriver[0]);
                login.Logout();
                if (login.UserIdTxtBox().Displayed)
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

                // Step 5
                // Login from CLIENTSYS1 as USER1
                login.SetDriver(BasePage.MultiDriver[1]);
                login.DriverGoTo(login.url);
                login.LoginGrid(User1, User1);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (login.LogoutBtn().Displayed)
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

                // Step 6
                // Login from CLIENTSYS2 as USER2.
                login.SetDriver(BasePage.MultiDriver[2]);
                login.DriverGoTo(login.url);
                login.LoginGrid(User2, User2);
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

                // Step 7
                // Login from CLIENTSYS2 as Administrator.
                login.SetDriver(BasePage.MultiDriver[2]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
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
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.AddLicenseInConfigTool(Licbackpath);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                Config.node = "localhost";
                login.closeallbrowser();
                login.InvokeBrowser(Config.BrowserType);
            }
        }

        public TestCaseResult Test_27655(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            Login login = new Login();
            DomainManagement domainmanagement = null;
            UserManagement usermanagement = null;
            Maintenance maintenance = null;
            Studies studies = new Studies();
            WpfObjects wpfobject = new WpfObjects();
            ServiceTool servicetool = new ServiceTool();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            int WebAdmincount = 0;
            int BaseAdmincount = 0;
            DataTable datatable;
            String[] actual;
            MultiDriver = new List<IWebDriver>();
            string Onebase1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "1base1adminpath");
            string Onebase0adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "1base0adminpath");
            string Zerobase1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "0base1adminpath");
            string Onebase1maxusers1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "1base1maxusers1adminpath");
            string Twobase1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "2base1adminpath");
            string Threebase1admin = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "3base1admin");
            string Threemaxusers1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "3maxusers1adminpath");
            string Fourmaxusers1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "4maxusers1adminpath");
            string Licbackpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Licbackpath");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data                       
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;


                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.AddLicenseInConfigTool(Twobase1adminpath);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();

                BasePage.MultiDriver.Add(BasePage.Driver);
                login.SetDriver(MultiDriver[0]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForPageLoad(10);
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(10);
                // Step 1
                // Create new Domain group D1 (default SuperAdminGroup) under Domain Management tab.
                // In Domain D1:
                // Create 5 users with Type as User.Referred to as USER1...USER5
                //Create 2 System Admin users with Type as SuperAdmin.Referred to SADMIN1 and SADMIN2
                // Create 3 Domain Admin users with Type as DomainAdmin.Referred to DADMIN3, DADMIN4, DADMIN5
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(10);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForPageLoad(10);
                //if (!domainmanagement.SearchDomain("SuperAdminGroup"))
                //{
                //    domainmanagement.CreateDomain("SuperAdminGroup", Role1, DS: new string[] { "print" }, datasources: new string[] { DatasourceAutoSSA, DatasourceVMSSA131, DatasourceVMSSA91, DatasourceSanityPACS, DatasourcePACS2 }, check: 1);
                //}

                domainmanagement.SearchDomain("SuperAdminGroup");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.EditDomainButton().Click();
                Driver.SwitchTo().DefaultContent();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForPageLoad(10);
                domainmanagement.VisibleAllStudySearchField();
                domainmanagement.SaveDomainButtoninEditPage().Click();

                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(10);
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                PageLoadWait.WaitForPageLoad(10);

                if (!usermanagement.SearchUser(User1))
                {
                    usermanagement.CreateUser(User1, Role1);
                }

                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(User2))
                {
                    usermanagement.CreateUser(User2, Role1);
                }
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(User3))
                {
                    usermanagement.CreateUser(User3, Role1);
                }
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(User4))
                {
                    usermanagement.CreateUser(User4, Role1);
                }
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(User5))
                {
                    usermanagement.CreateUser(User5, Role1);
                }

                if (!usermanagement.SearchUser(SystemAdmin1, "SuperAdminGroup"))
                {
                    usermanagement.CreateSystemAdminUser(SystemAdmin1, "SuperAdminGroup");
                }

                if (!usermanagement.SearchUser(SystemAdmin2, "SuperAdminGroup"))
                {
                    usermanagement.CreateSystemAdminUser(SystemAdmin2, "SuperAdminGroup");
                }
                //usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                //if (!usermanagement.SearchUser(DomainAdmin1))
                //{
                //    usermanagement.CreateDomainAdminUser(DomainAdmin1, "SuperAdminGroup");
                //}
                //usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                //if (!usermanagement.SearchUser(DomainAdmin2))
                //{
                //    usermanagement.CreateDomainAdminUser(DomainAdmin2, "SuperAdminGroup");
                //}
                //usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                //PageLoadWait.waitforprocessingspinner(10);
                //if (!usermanagement.SearchUser(DomainAdmin3))
                //{
                //    usermanagement.CreateDomainAdminUser(DomainAdmin3, "SuperAdminGroup");
                //}

                login.Logout();
                PageLoadWait.WaitForPageLoad(10);

                // Step 1 
                // Login from WASYS as Administrator.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                login.SetDriver(BasePage.MultiDriver[0]);
                login.DriverGoTo(login.url);
                PageLoadWait.WaitForPageLoad(10);
                login.LoginGrid(Username, Password);
                PageLoadWait.WaitForPageLoad(10);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 1)
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


                // Step 2
                // Login from CLIENTSYS1 as Administrator.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                Config.node = Config.Clientsys1;
                BasePage.MultiDriver.Add(login.InvokeBrowser("Remote-" + Config.BrowserType));
                login.SetDriver(BasePage.MultiDriver[1]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 2)
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

                // Step 3
                // Login from CLIENTSYS2 as USER2.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                Config.node = Config.Clientsys2;
                BasePage.MultiDriver.Add(login.InvokeBrowser("Remote-" + Config.BrowserType));
                login.SetDriver(BasePage.MultiDriver[2]);
                login.DriverGoTo(login.url);
                login.LoginGrid(User2, User2);
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

                // Step 4
                // Login from CLIENTSYS2 as SADMIN1.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                login.SetDriver(BasePage.MultiDriver[2]);
                login.DriverGoTo(login.url);
                login.LoginGrid(SystemAdmin1, SystemAdmin1);
                PageLoadWait.WaitForPageLoad(10);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                WebAdmincount = actual.Where(val => string.Equals(val, "webaccess.admin 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 2 && WebAdmincount == 1)
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

                // Step 5
                // Login from CLIENTSYS3 as SADMIN2.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                Config.node = Config.Clientsys3;
                BasePage.MultiDriver.Add(login.InvokeBrowser("Remote-" + Config.BrowserType));
                login.SetDriver(MultiDriver[3]);
                login.DriverGoTo(login.url);
                login.LoginGrid(SystemAdmin2, SystemAdmin2);
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

                // Step 6
                // Test Data: Generally, the webaccess.admin shouldn't be used if there are available webaccess.base licenses, but this is still used until the session on CLIENTSYS1 renews its license. 
                //  Logout from a CLIENTSYS1 as Administrator.
                // Refer to Pre-condition 7, and verify both methods give the same result. 
                login.SetDriver(BasePage.MultiDriver[1]);
                login.Logout();

                login.SetDriver(BasePage.MultiDriver[0]);
                PageLoadWait.WaitForPageLoad(10);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
                PageLoadWait.WaitForPageLoad(10);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                WebAdmincount = actual.Where(val => string.Equals(val, "webaccess.admin 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 1 && WebAdmincount == 1)
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

                // Step 7
                // Login from CLIENTSYS3 as USER3.
                // Refer to Pre-condition 7, and verify both methods give the same result.
                login.SetDriver(BasePage.MultiDriver[3]);
                login.DriverGoTo(login.url);
                login.LoginGrid(User3, User3);

                login.SetDriver(BasePage.MultiDriver[0]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                WebAdmincount = actual.Where(val => string.Equals(val, "webaccess.admin 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 2 && WebAdmincount == 1)
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

                // Step 8
                // Login from CLIENTSYS1 as Administrator
                // Refer to Pre - condition 7, and verify both methods give the same result.
                login.SetDriver(BasePage.MultiDriver[1]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
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

                // Step 9
                // Logout from all accounts
                login.SetDriver(BasePage.MultiDriver[0]);
                login.Logout();
                login.SetDriver(BasePage.MultiDriver[2]);
                login.Logout();
                login.SetDriver(BasePage.MultiDriver[3]);
                login.Logout();
                ExecutedSteps++;

                // Step 10
                // Login from WASYS as Administrator.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                login.SetDriver(BasePage.MultiDriver[0]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 1)
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

                // Step 11
                // Logout from WASYS as Administrator.
                login.SetDriver(BasePage.MultiDriver[0]);
                login.Logout();
                ExecutedSteps++;

                // Step 12
                // Login from CLIENTSYS1 as USER1
                // Refer to Pre - condition 7, and verify both methods give the same result.
                login.SetDriver(BasePage.MultiDriver[1]);
                login.DriverGoTo(login.url);
                login.LoginGrid(User1, User1);

                login.SetDriver(BasePage.MultiDriver[0]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                // WebAdmincount = actual.Where(val => string.Equals(val, "webaccess.admin 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 2) // && WebAdmincount == 1
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


                // Step 13
                // Logout Administrator user first, then login from CLIENTSYS2 as USER2.
                // Login Administrator user again. Refer to Pre - condition 7, and verify both methods give the same result.
                login.SetDriver(MultiDriver[0]);
                login.Logout();

                login.SetDriver(MultiDriver[2]);
                login.DriverGoTo(login.url);
                login.LoginGrid(User2, User2);

                login.SetDriver(MultiDriver[0]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);

                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                WebAdmincount = actual.Where(val => string.Equals(val, "webaccess.admin 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 2 && WebAdmincount == 1)
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


                // Step 14
                // Logout Administrator. Login from CLIENTSYS3 as SADMIN1.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                login.Logout();

                login.SetDriver(BasePage.MultiDriver[3]);
                login.DriverGoTo(login.url);
                login.LoginGrid(SystemAdmin1, SystemAdmin1);

                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                WebAdmincount = actual.Where(val => string.Equals(val, "webaccess.admin 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 2 && WebAdmincount == 1)
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

                // Step 15
                // Login from WASYS as Administrator.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                login.SetDriver(BasePage.MultiDriver[0]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);

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


                // Step 16
                // Login from WASYS as USER2.
                // Refer to Pre-condition 7, and verify both methods give the same result.
                //Config.node = Config.IConnectIP;
                //BasePage.MultiDriver.Add(login.InvokeBrowser("Remote-" + Config.BrowserType));
                login.SetDriver(BasePage.MultiDriver[0]);
                login.DriverGoTo(login.url);
                login.LoginGrid(User2, User2);

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


                // Step 17
                // Logout from all accounts
                //login.SetDriver(BasePage.MultiDriver[0]);
                //login.Logout();

                login.SetDriver(BasePage.MultiDriver[1]);
                login.Logout();

                login.SetDriver(BasePage.MultiDriver[2]);
                login.Logout();

                login.SetDriver(BasePage.MultiDriver[3]);
                login.Logout();

                ExecutedSteps++;

                // Step 18
                // Login from CLIENTSYS1 as USER1
                // Refer to Pre - condition 7, and verify both methods give the same result.
                login.SetDriver(BasePage.MultiDriver[1]);
                login.DriverGoTo(login.url);
                login.LoginGrid(User1, User1);

                login.SetDriver(BasePage.MultiDriver[0]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);

                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                // WebAdmincount = actual.Where(val => string.Equals(val, "webaccess.admin 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 2) //&& WebAdmincount == 1
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

                // Step 19
                // Logout from Administrator.  Login from CLIENTSYS2 as USER2.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                login.SetDriver(BasePage.MultiDriver[0]);
                login.Logout();

                login.SetDriver(BasePage.MultiDriver[2]);
                login.DriverGoTo(login.url);
                login.LoginGrid(User2, User2);

                login.SetDriver(BasePage.MultiDriver[0]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                WebAdmincount = actual.Where(val => string.Equals(val, "webaccess.admin 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 2 && WebAdmincount == 1)
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


                // Step 20
                // Logout Administrator. Login from CLIENTSYS3 as USER3.
                login.SetDriver(BasePage.MultiDriver[3]);
                login.LoginGrid(User3, User3);

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

                // Step 21
                // Login from WASYS as Administrator.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                login.SetDriver(BasePage.MultiDriver[0]);
                login.LoginGrid(Username, Password);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                WebAdmincount = actual.Where(val => string.Equals(val, "webaccess.admin 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 2 && WebAdmincount == 1)
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


                // Step 22
                // Select Maintenance tab
                //  maintenance = (Maintenance)login.Navigate("Maintenance");
                ExecutedSteps++;


                // Step 23
                // Sort the list using each column heading.
                PageLoadWait.WaitForPageLoad(10);
                bool UserIdAscDescSortResult = false;
                MultiDriver[0].SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                DataTable TableValue = CollectRecordsInAllPages(maintenance.Tbl_LicenseTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());

                maintenance.TableHeadings()[2].Click();
                maintenance.TableHeadings()[0].Click();
                PageLoadWait.WaitForFrameLoad(20);
                TableValue = CollectRecordsInTable(maintenance.Tbl_LicenseTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                string[] AscUserName = GetColumnValues(TableValue, "User ID");

                maintenance.TableHeadings()[0].Click();
                PageLoadWait.WaitForFrameLoad(20);
                TableValue = CollectRecordsInTable(maintenance.Tbl_LicenseTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                string[] DescUserName = GetColumnValues(TableValue, "User ID");

                if (AscUserName.SequenceEqual((AscUserName.OrderBy(c => c).ToArray())) && DescUserName.SequenceEqual((DescUserName.OrderByDescending(c => c).ToArray())))
                {
                    UserIdAscDescSortResult = true;
                }

                if (UserIdAscDescSortResult)
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
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.AddLicenseInConfigTool(Licbackpath);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                Config.node = "localhost";
                closeallbrowser();
                login.InvokeBrowser(Config.BrowserType);
            }
        }

        public TestCaseResult Test_27656(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            Login login = new Login();
            DomainManagement domainmanagement = null;
            UserManagement usermanagement = null;
            Maintenance maintenance = null;
            MultiDriver = new List<IWebDriver>();
            StudyViewer viewer = new StudyViewer();
            Stopwatch stopwatch = new Stopwatch();
            Studies studies = null;
            WpfObjects wpfobject = new WpfObjects();
            ServiceTool servicetool = new ServiceTool();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            TimeSpan timeout = new TimeSpan(0, 6, 0);
            int BaseAdmincount = 0;
            int WebAdmincount = 0;
            DataTable datatable;
            String[] actual;
            String timeoutmessage = "You have not logged in yet or your session has expired. Please log in again.";

            string Onebase1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "1base1adminpath");
            string Onebase0adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "1base0adminpath");
            string Zerobase1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "0base1adminpath");
            string Onebase1maxusers1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "1base1maxusers1adminpath");
            string Twobase1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "2base1adminpath");
            string Threebase1admin = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "3base1admin");
            string Threemaxusers1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "3maxusers1adminpath");
            string Fourmaxusers1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "4maxusers1adminpath");
            string Licbackpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Licbackpath");
            string StudyID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyID");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data                       
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;


                // Pre-Condition
                //Set Session Timeout

                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.AddLicenseInConfigTool(Twobase1adminpath);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();

                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Security");
                servicetool.WaitWhileBusy();
                servicetool.ClickModifyFromTab();
                servicetool.WaitWhileBusy();
                servicetool.SetTimeout(timeout.Minutes);
                servicetool.RestartService();
                servicetool.CloseServiceTool();

                // This is the parent driver.
                BasePage.MultiDriver.Add(BasePage.Driver);
                login.SetDriver(MultiDriver[0]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForPageLoad(10);
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(10);

                // Step 1
                // Create new Domain group D1 (default SuperAdminGroup) under Domain Management tab.
                // In Domain D1:
                // Create 5 users with Type as User.Referred to as USER1...USER5
                //Create 2 System Admin users with Type as SuperAdmin.Referred to SADMIN1 and SADMIN2
                // Create 3 Domain Admin users with Type as DomainAdmin.Referred to DADMIN3, DADMIN4, DADMIN5
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(10);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForPageLoad(10);
                //if (!domainmanagement.SearchDomain("SuperAdminGroup"))
                //{
                //    domainmanagement.CreateDomain("SuperAdminGroup", Role1, DS: new string[] { "print" }, datasources: new string[] { DatasourceAutoSSA, DatasourceVMSSA131, DatasourceVMSSA91, DatasourceSanityPACS, DatasourcePACS2 }, check: 1);
                //}

                domainmanagement.SearchDomain("SuperAdminGroup");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.EditDomainButton().Click();
                Driver.SwitchTo().DefaultContent();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForPageLoad(10);
                domainmanagement.VisibleAllStudySearchField();
                domainmanagement.SaveDomainButtoninEditPage().Click();

                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(10);
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                PageLoadWait.WaitForPageLoad(10);

                if (!usermanagement.SearchUser(User1))
                {
                    usermanagement.CreateUser(User1, Role1);
                }

                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(User2))
                {
                    usermanagement.CreateUser(User2, Role1);
                }
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(User3))
                {
                    usermanagement.CreateUser(User3, Role1);
                }
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(User4))
                {
                    usermanagement.CreateUser(User4, Role1);
                }
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(User5))
                {
                    usermanagement.CreateUser(User5, Role1);
                }

                if (!usermanagement.SearchUser(SystemAdmin1, "SuperAdminGroup"))
                {
                    usermanagement.CreateSystemAdminUser(SystemAdmin1, "SuperAdminGroup");
                }

                if (!usermanagement.SearchUser(SystemAdmin2, "SuperAdminGroup"))
                {
                    usermanagement.CreateSystemAdminUser(SystemAdmin2, "SuperAdminGroup");
                }
                //usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                //if (!usermanagement.SearchUser(DomainAdmin1))
                //{
                //    usermanagement.CreateDomainAdminUser(DomainAdmin1, "SuperAdminGroup");
                //}
                //usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                //if (!usermanagement.SearchUser(DomainAdmin2))
                //{
                //    usermanagement.CreateDomainAdminUser(DomainAdmin2, "SuperAdminGroup");
                //}
                //usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                //if (!usermanagement.SearchUser(DomainAdmin3))
                //{
                //    usermanagement.CreateDomainAdminUser(DomainAdmin3, "SuperAdminGroup");
                //}

                login.Logout();
                PageLoadWait.WaitForPageLoad(10);

                // Step 1 
                // Login from WASYS as Administrator.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                login.SetDriver(BasePage.MultiDriver[0]);
                PageLoadWait.WaitForPageLoad(10);
                login.LoginGrid(Username, Password);
                PageLoadWait.WaitForPageLoad(10);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 1)
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

                // Step 2
                // Login from CLIENTSYS1 as Administrator.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                Config.node = Config.Clientsys1;
                BasePage.MultiDriver.Add(login.InvokeBrowser("Remote-" + Config.BrowserType));
                login.SetDriver(MultiDriver[1]);
                login.DriverGoTo(login.url);
                PageLoadWait.WaitForPageLoad(10);
                login.LoginGrid(Username, Password);
                PageLoadWait.WaitForPageLoad(10);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 2)
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


                // Step 3
                // Login from CLIENTSYS2 as SADMIN1.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                Config.node = Config.Clientsys2;
                BasePage.MultiDriver.Add(login.InvokeBrowser("Remote-" + Config.BrowserType));
                login.SetDriver(MultiDriver[2]);
                PageLoadWait.WaitForPageLoad(10);
                login.DriverGoTo(login.url);
                login.LoginGrid(SystemAdmin1, SystemAdmin1);
                PageLoadWait.WaitForFrameLoad(10);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                WebAdmincount = actual.Where(val => string.Equals(val, "webaccess.admin 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 2 && WebAdmincount == 1)
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
                // Perform an IISReset on WASYS
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                // Step 5
                // Log back in as Administrator. Verify the license usage.
                login.SetDriver(MultiDriver[0]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
                PageLoadWait.WaitForPageLoad(10);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 1)
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


                // Step 6
                // Login to CLIENTSYS2 as Administrator, and load a study.
                login.SetDriver(MultiDriver[2]);
                login.DriverGoTo(login.url);
                login.LoginGrid(SystemAdmin1, SystemAdmin1);
                PageLoadWait.WaitForPageLoad(10);
                studies = (Studies)login.Navigate("Studies");
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SearchStudy(studyID: StudyID);
                studies.SelectStudy1("Study ID", StudyID);
                viewer = StudyViewer.LaunchStudy();
                studies.CloseStudy();
                PageLoadWait.WaitForPageLoad(10);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 2)
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
                // Wait for session to time out on CLIENTSYS2, load a study.
                // Refer to Pre - condition 7, and verify result.
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout)) { /*For automation purpose we have kept 4 mins Stay Idle for 4 Miniutes*/ }
                int actualtimeout = stopwatch.Elapsed.Minutes;
                stopwatch.Stop();
                stopwatch.Reset();
                bool message = BasePage.Driver.FindElement(By.CssSelector("span[id$='_LoginMasterContentPlaceHolder_ErrorMessage']")).GetAttribute("innerHTML").Equals(timeoutmessage);
                if (message)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
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
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.AddLicenseInConfigTool(Licbackpath);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                Config.node = "localhost";
                closeallbrowser();
                timeout = new TimeSpan(0, 30, 0);
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Security");
                servicetool.WaitWhileBusy();
                servicetool.ClickModifyFromTab();
                servicetool.WaitWhileBusy();

                servicetool.SetTimeout(timeout.Minutes);
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                login.InvokeBrowser(Config.BrowserType);

            }

        }

        public TestCaseResult Test_27657(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            Login login = new Login();
            DomainManagement domainmanagement = null;
            UserManagement usermanagement = null;
            Maintenance maintenance = null;
            Studies studies = null;
            MultiDriver = new List<IWebDriver>();
            TimeSpan timeout = new TimeSpan(0, 2, 0);
            WpfObjects wpfobject = new WpfObjects();
            ServiceTool servicetool = new ServiceTool();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            int BaseAdmincount = 0;
            int WebeAdmincount = 0;
            DataTable datatable;
            String[] actual;

            string Onebase1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "1base1adminpath");
            string Onebase0adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "1base0adminpath");
            string Zerobase1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "0base1adminpath");
            string Onebase1maxusers1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "1base1maxusers1adminpath");
            string Twobase1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "2base1adminpath");
            string Threebase1admin = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "3base1admin");
            string Threemaxusers1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "3maxusers1adminpath");
            string Fourmaxusers1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "4maxusers1adminpath");
            string Licbackpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Licbackpath");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data                       
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;


                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.AddLicenseInConfigTool(Twobase1adminpath);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                // Pre-Condition
                //Set Session Timeout
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToTab("Security");
                wpfobject.WaitTillLoad();
                servicetool.ClickModifyFromTab();
                wpfobject.WaitTillLoad();
                servicetool.SetTimeout(timeout.Minutes);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();

                // This is the parent driver.
                BasePage.MultiDriver.Add(BasePage.Driver);
                login.SetDriver(MultiDriver[0]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForPageLoad(10);
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(10);

                // Create new Domain group D1 (default SuperAdminGroup) under Domain Management tab.
                // In Domain D1:
                // Create 5 users with Type as User.Referred to as USER1...USER5
                //Create 2 System Admin users with Type as SuperAdmin.Referred to SADMIN1 and SADMIN2
                // Create 3 Domain Admin users with Type as DomainAdmin.Referred to DADMIN3, DADMIN4, DADMIN5
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(10);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForPageLoad(10);
                //if (!domainmanagement.SearchDomain("SuperAdminGroup"))
                //{
                //    domainmanagement.CreateDomain("SuperAdminGroup", Role1, DS: new string[] { "print" }, datasources: new string[] { DatasourceAutoSSA, DatasourceVMSSA131, DatasourceVMSSA91, DatasourceSanityPACS, DatasourcePACS2 }, check: 1);
                //}

                domainmanagement.SearchDomain("SuperAdminGroup");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.EditDomainButton().Click();
                Driver.SwitchTo().DefaultContent();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForPageLoad(10);
                domainmanagement.VisibleAllStudySearchField();
                domainmanagement.SaveDomainButtoninEditPage().Click();

                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(10);
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                PageLoadWait.WaitForPageLoad(10);

                if (!usermanagement.SearchUser(User1))
                {
                    usermanagement.CreateUser(User1, Role1);
                }

                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(User2))
                {
                    usermanagement.CreateUser(User2, Role1);
                }
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(User3))
                {
                    usermanagement.CreateUser(User3, Role1);
                }
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(User4))
                {
                    usermanagement.CreateUser(User4, Role1);
                }
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(User5))
                {
                    usermanagement.CreateUser(User5, Role1);
                }

                if (!usermanagement.SearchUser(SystemAdmin1, "SuperAdminGroup"))
                {
                    usermanagement.CreateSystemAdminUser(SystemAdmin1, "SuperAdminGroup");
                }

                if (!usermanagement.SearchUser(SystemAdmin2, "SuperAdminGroup"))
                {
                    usermanagement.CreateSystemAdminUser(SystemAdmin2, "SuperAdminGroup");
                }
                //usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                //if (!usermanagement.SearchUser(DomainAdmin1))
                //{
                //    usermanagement.CreateDomainAdminUser(DomainAdmin1, "SuperAdminGroup");
                //}
                //usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                //if (!usermanagement.SearchUser(DomainAdmin2))
                //{
                //    usermanagement.CreateDomainAdminUser(DomainAdmin2, "SuperAdminGroup");
                //}
                //usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                //if (!usermanagement.SearchUser(DomainAdmin3))
                //{
                //    usermanagement.CreateDomainAdminUser(DomainAdmin3, "SuperAdminGroup");
                //}

                login.Logout();
                PageLoadWait.WaitForPageLoad(10);

                // Step 1 
                // Login from WASYS as Administrator.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                login.SetDriver(BasePage.MultiDriver[0]);
                login.LoginGrid(Username, Password);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 1)
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



                // Step 2
                // Login from CLIENTSYS1 as Administrator.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                Config.node = Config.Clientsys1;
                BasePage.MultiDriver.Add(login.InvokeBrowser("Remote-" + Config.BrowserType));
                login.SetDriver(MultiDriver[1]);
                login.DriverGoTo(login.url);
                PageLoadWait.WaitForPageLoad(10);
                login.LoginGrid(Username, Password);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 2)
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

                // Step 3
                // Login from CLIENTSYS2 as SADMIN1.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                Config.node = Config.Clientsys2;
                BasePage.MultiDriver.Add(login.InvokeBrowser("Remote-" + Config.BrowserType));
                login.SetDriver(MultiDriver[2]);
                login.DriverGoTo(login.url);
                login.LoginGrid(SystemAdmin1, SystemAdmin1);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                WebeAdmincount = actual.Where(val => string.Equals(val, "webaccess.admin 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 2 && WebeAdmincount == 1)
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
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.AddLicenseInConfigTool(Licbackpath);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                Config.node = "localhost";
                closeallbrowser();
                login.InvokeBrowser(Config.BrowserType);

                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToTab("Security");
                wpfobject.WaitTillLoad();
                servicetool.ClickModifyFromTab();
                wpfobject.WaitTillLoad();
                timeout = new TimeSpan(0, 30, 0);
                servicetool.SetTimeout(timeout.Minutes);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
            }
        }

        public TestCaseResult Test_27658(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            Login login = new Login();
            MultiDriver = new List<IWebDriver>();
            Studies studies = null;
            DomainManagement domainmanagement = null;
            UserManagement usermanagement = null;
            RoleManagement rolemanagement = null;
            Maintenance maintenance = null;
            WpfObjects wpfobject = new WpfObjects();
            ServiceTool servicetool = new ServiceTool();
            result = new TestCaseResult(stepcount);
            Stopwatch stopwatch = new Stopwatch();
            StudyViewer viewer = new StudyViewer();
            int ExecutedSteps = -1;
            int BaseAdmincount = 0;


            string Onebase1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "1base1adminpath");
            string Onebase0adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "1base0adminpath");
            string Twobase0adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "2base0adminpath");
            string Zerobase1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "0base1adminpath");
            string Onebase1maxusers1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "1base1maxusers1adminpath");
            string Twobase1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "2base1adminpath");
            string Threebase1admin = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "3base1admin");
            string Threemaxusers1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "3maxusers1adminpath");
            string Fourmaxusers1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "4maxusers1adminpath");
            string Licbackpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Licbackpath");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data                       
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;


                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.AddLicenseInConfigTool(Twobase0adminpath);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();

                BasePage.MultiDriver.Add(BasePage.Driver);
                login.SetDriver(MultiDriver[0]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForPageLoad(10);
                // Step 1
                // Create new Domain group D1 (default SuperAdminGroup) under Domain Management tab.
                // In Domain D1:
                // Create 5 users with Type as User.Referred to as USER1...USER5
                //Create 2 System Admin users with Type as SuperAdmin.Referred to SADMIN1 and SADMIN2
                // Create 3 Domain Admin users with Type as DomainAdmin.Referred to DADMIN3, DADMIN4, DADMIN5
                //if (!domainmanagement.SearchDomain("SuperAdminGroup"))
                //{
                //    domainmanagement.CreateDomain("SuperAdminGroup", Role1, DS: new string[] { "print" }, datasources: new string[] { DatasourceAutoSSA, DatasourceVMSSA131, DatasourceVMSSA91, DatasourceSanityPACS, DatasourcePACS2 }, check: 1);
                //}

                domainmanagement.SearchDomain("SuperAdminGroup");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.EditDomainButton().Click();
                Driver.SwitchTo().DefaultContent();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForPageLoad(10);
                domainmanagement.VisibleAllStudySearchField();
                domainmanagement.SaveDomainButtoninEditPage().Click();

                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(10);
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                PageLoadWait.WaitForPageLoad(10);

                if (!usermanagement.SearchUser(User1))
                {
                    usermanagement.CreateUser(User1, Role1);
                }

                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(User2))
                {
                    usermanagement.CreateUser(User2, Role1);
                }
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(User3))
                {
                    usermanagement.CreateUser(User3, Role1);
                }
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(User4))
                {
                    usermanagement.CreateUser(User4, Role1);
                }
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(User5))
                {
                    usermanagement.CreateUser(User5, Role1);
                }

                if (!usermanagement.SearchUser(SystemAdmin1, "SuperAdminGroup"))
                {
                    usermanagement.CreateSystemAdminUser(SystemAdmin1, "SuperAdminGroup");
                }

                if (!usermanagement.SearchUser(SystemAdmin2, "SuperAdminGroup"))
                {
                    usermanagement.CreateSystemAdminUser(SystemAdmin2, "SuperAdminGroup");
                }
                //usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                //if (!usermanagement.SearchUser(DomainAdmin1))
                //{
                //    usermanagement.CreateDomainAdminUser(DomainAdmin1, "SuperAdminGroup");
                //}
                //usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                //if (!usermanagement.SearchUser(DomainAdmin2))
                //{
                //    usermanagement.CreateDomainAdminUser(DomainAdmin2, "SuperAdminGroup");
                //}
                //usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                //if (!usermanagement.SearchUser(DomainAdmin3))
                //{
                //    usermanagement.CreateDomainAdminUser(DomainAdmin3, "SuperAdminGroup");
                //}

                login.Logout();
                PageLoadWait.WaitForPageLoad(10);

                // Precondition
                //Login to the server as an Administrator, and create the following roles : 
                // ROLE1 with name 'Admi', assigned to USER1.
                // ROLE2 with name 'TestRoleAdmin' assigned to USER2
                // ROLE3 with name 'TestadMiN' assigned to USER3
                // ROLE4 with name 'damin' assigned to USER4
                // Logout from all accounts

                login.LoginGrid(Username, Password);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                if (!rolemanagement.RoleExists(admi))
                {
                    rolemanagement.NewRoleBtn().Click();
                    rolemanagement.CreateRole("SuperAdminGroup", admi, GrantAccess: 0);
                }
                if (!rolemanagement.RoleExists(TestRoleAdmin))
                {
                    rolemanagement.NewRoleBtn().Click();
                    rolemanagement.CreateRole("SuperAdminGroup", TestRoleAdmin, GrantAccess: 0);
                }
                if (!rolemanagement.RoleExists(TestadMiN))
                {
                    rolemanagement.NewRoleBtn().Click();
                    rolemanagement.CreateRole("SuperAdminGroup", TestadMiN, GrantAccess: 0);
                }

                if (!rolemanagement.RoleExists(damin))
                {
                    rolemanagement.NewRoleBtn().Click();
                    rolemanagement.CreateRole("SuperAdminGroup", damin, GrantAccess: 0);
                }


                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.SearchUser(User1);
                usermanagement.SelectUser(User1);
                usermanagement.EditUsrBtn().Click();
                PageLoadWait.WaitForFrameLoad(10);
                usermanagement.GetUseModifyRoleDropDown().SelectByText(admi);
                Click("cssselector", "#ctl00_MasterContentPlaceHolder_SaveButton");

                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.SearchUser(User2);
                usermanagement.SelectUser(User2);
                usermanagement.EditUsrBtn().Click();
                PageLoadWait.WaitForFrameLoad(10);
                usermanagement.GetUseModifyRoleDropDown().SelectByText(TestRoleAdmin);
                Click("cssselector", "#ctl00_MasterContentPlaceHolder_SaveButton");

                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.SearchUser(User3);
                usermanagement.SelectUser(User3);
                usermanagement.EditUsrBtn().Click();
                PageLoadWait.WaitForFrameLoad(10);
                usermanagement.GetUseModifyRoleDropDown().SelectByText(TestadMiN);

                Click("cssselector", "#ctl00_MasterContentPlaceHolder_SaveButton");

                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.SearchUser(User4);
                usermanagement.SelectUser(User4);
                usermanagement.EditUsrBtn().Click();
                PageLoadWait.WaitForFrameLoad(10);
                usermanagement.GetUseModifyRoleDropDown().SelectByText(damin);
                Click("cssselector", "#ctl00_MasterContentPlaceHolder_SaveButton");
                login.Logout();

                // Step 1 
                // Login from WASYS as Administrator.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                login.SetDriver(MultiDriver[0]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
                PageLoadWait.WaitForPageLoad(10);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                MultiDriver[0].SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                DataTable datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                String[] actual = GetColumnValues(datatable, "Feature Name");
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 1)
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

                // Step 2
                // Login from CLIENTSYS1 as Administrator.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                Config.node = Config.Clientsys1;
                BasePage.MultiDriver.Add(login.InvokeBrowser("Remote-" + Config.BrowserType));
                login.SetDriver(MultiDriver[1]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
                PageLoadWait.WaitForPageLoad(10);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                PageLoadWait.WaitForPageLoad(10);
                MultiDriver[1].SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 2)
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

                // Step 3
                // Login from CLIENTSYS2 as USER1
                // Refer to Pre - condition 7, and verify both methods give the same result.
                Config.node = Config.Clientsys2;
                BasePage.MultiDriver.Add(login.InvokeBrowser("Remote-" + Config.BrowserType));
                login.SetDriver(MultiDriver[2]);
                login.DriverGoTo(login.url);
                login.LoginGrid(User1, User1);
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

                // Step 4
                // Login from CLIENTSYS2 as USER2.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                login.SetDriver(MultiDriver[2]);
                login.DriverGoTo(login.url);
                login.LoginGrid(User2, User2);
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
                // Step 5
                // Login from CLIENTSYS2 as USER3.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                login.SetDriver(MultiDriver[2]);
                login.DriverGoTo(login.url);
                login.LoginGrid(User3, User3);
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

                // Step 6
                // Login from CLIENTSYS2 as USER4.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                login.SetDriver(MultiDriver[2]);
                login.DriverGoTo(login.url);
                login.LoginGrid(User4, User4);
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

                // Step 7
                // Login from CLIENTSYS1 as USER1
                // Refer to Pre - condition 7, and verify both methods give the same result.
                login.SetDriver(MultiDriver[1]);
                login.DriverGoTo(login.url);
                login.LoginGrid(User1, User1);
                login.SetDriver(MultiDriver[0]);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 2)
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


                // Step 8
                // Load any study.
                login.SetDriver(MultiDriver[1]);
                ////studies = (Studies)login.Navigate("Studies");
                //studies = new Studies();
                //studies.ChooseColumns(new string[] { "Study ID" });
                //studies.SearchStudy(studyID: "1040");
                //studies.SelectStudy1("Study ID", "1040");
                //studies.LaunchStudy();
                //studies.CloseStudy();

                //String filename_12 = string.Format("Study_Resolution_27658_8.jpg");
                //String filepath_12 = "C:\\Users\\Administrator\\Desktop\\testresult\\" + filename_12;// Config.screenshotpath + Path.DirectorySeparatorChar + filename_12;

                //if (((RemoteWebDriver)BasePage.MultiDriver[1]).Capabilities.BrowserName.Contains("chrome"))
                //{
                //    studies.DownloadImageFile(MultiDriver[1].FindElement(By.CssSelector("#ViewerContainer")), filepath_12);
                //    MultiDriver[1].FindElement(By.CssSelector("#ViewerContainer"));
                //    //Screenshot driversnapshot = ((ITakesScreenshot)BasePage.MultiDriver[1]).GetScreenshot();
                //    //driversnapshot.SaveAsFile(Config.downloadpath, ImageFormat.Jpeg);
                //}

                //System.Drawing.Image img = System.Drawing.Image.FromFile(filepath_12 + "\\" +  filename_12);
                //int height = img.Height;
                //int width = img.Width;
                //bool ResolutionHeightVerify = height == 1024;
                //bool ResolutionWidthVerify = width == 786;

                //if (viewer.ViewStudy() && ResolutionHeightVerify && ResolutionWidthVerify)
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

                result.steps[++ExecutedSteps].status = "Not Automated";

                // Step 9
                // Perform various operations for more than 5 minutes so that the session shall not time out.
                TimeSpan timeout = new TimeSpan(0, 5, 0);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout)) { /*Stay Idle for 5 Miniutes*/ }
                int actualtimeout = stopwatch.Elapsed.Minutes;
                stopwatch.Stop();
                stopwatch.Reset();
                if (actualtimeout == timeout.Minutes)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step 10
                // After more than 5 minutes have passed, Refer to Pre - condition 7, and verify both methods give the same result.
                login.SetDriver(MultiDriver[0]);
                studies = (Studies)login.Navigate("Studies");
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 2)
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



                // Step 11
                // Logout from CLIENTSYS1 as USER1.
                login.SetDriver(MultiDriver[1]);
                login.Logout();
                if (login.UserIdTxtBox().Displayed)
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

                // Step 12
                // Login from CLIENTSYS2 as USER2
                // Refer to Pre - condition 7, and verify both methods give the same result.
                login.SetDriver(MultiDriver[2]);
                login.LoginGrid(User2, User2);

                login.SetDriver(MultiDriver[0]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
                studies = (Studies)login.Navigate("Studies");
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 2)
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

                // Step 13
                // Load any study.
                // Verify image is optimized for 1280x1024 resolution.
                //studies = (Studies)login.Navigate("Studies");
                //studies.ChooseColumns(new string[] { "Study ID" });
                //studies.SearchStudy(studyID: "1040");
                //studies.SelectStudy1("Study ID", "1040");
                //studies.LaunchStudy();

                //filename_12 = string.Format("Study_Resolution_27658_13.jpg");
                //filepath_12 = Config.screenshotpath + Path.DirectorySeparatorChar + filename_12;

                //if (Config.BrowserType.Contains("remote"))
                //{
                //    Screenshot driversnapshot = ((ITakesScreenshot)BasePage.Driver).GetScreenshot();
                //    driversnapshot.SaveAsFile(Config.downloadpath, ImageFormat.Jpeg);
                //}

                //img = System.Drawing.Image.FromFile(Config.downloadpath + "\\" + filename_12);
                //height = img.Height;
                //width = img.Width;
                //ResolutionHeightVerify = height == 1280;
                //ResolutionWidthVerify = width == 1024;

                //if (viewer.ViewStudy() && ResolutionHeightVerify && ResolutionWidthVerify)
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
                result.steps[++ExecutedSteps].status = "Not Automated";

                // Step 14
                // Perform various operations for more than 2 minutes so that the session shall not time out.
                timeout = new TimeSpan(0, 2, 0);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout)) { /*Stay Idle for 2 Miniutes*/ }
                actualtimeout = stopwatch.Elapsed.Minutes;
                stopwatch.Stop();
                stopwatch.Reset();
                if (actualtimeout == timeout.Minutes)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step 15
                // After more than 2 minutes have passed, Refer to Pre-condition 7, and verify both methods give the same result.
                login.SetDriver(MultiDriver[0]);
                studies = (Studies)login.Navigate("Studies");
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                BaseAdmincount = actual.Where(val => string.Equals(val, "webaccess.base 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (BaseAdmincount == 2)
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

                // Step 16
                timeout = new TimeSpan(0, 30, 0);
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Security");
                servicetool.WaitWhileBusy();
                servicetool.ClickModifyFromTab();
                servicetool.WaitWhileBusy();
                servicetool.SetTimeout(timeout.Minutes);
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

            finally
            {
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.AddLicenseInConfigTool(Licbackpath);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                Config.node = "localhost";
                closeallbrowser();
                login.InvokeBrowser(Config.BrowserType);
            }
        }

        public TestCaseResult Test_27659(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            Login login = new Login();
            MultiDriver = new List<IWebDriver>();
            DomainManagement domainmanagement = null;
            UserManagement usermanagement = null;
            Maintenance maintenance = null;
            Studies studies = null;
            WpfObjects wpfobject = new WpfObjects();
            ServiceTool servicetool = new ServiceTool();
            StudyViewer viewer = new StudyViewer();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            TimeSpan timeout = new TimeSpan(0, 30, 0);
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int WebAdmincount = 0;
            int MaxUsercount = 0;
            DataTable datatable;
            String[] actual;
            String EA131 = login.GetHostName(Config.EA1);

            string Onebase1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "1base1adminpath");
            string Onebase0adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "1base0adminpath");
            string Zerobase1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "0base1adminpath");
            string Onebase1maxusers1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "1base1maxusers1adminpath");
            string Twobase1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "2base1adminpath");
            string Threebase1admin = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "3base1admin");
            string Threemaxusers1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "3maxusers1adminpath");
            string Fourmaxusers1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "4maxusers1adminpath");
            string Licbackpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Licbackpath");
            string StudyID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyID");
            try
            {
                //Fetch required Test data                       
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;


                // Pre-condition
                // "Configure the session timeout value to be default value (30 minutes). From Merge iConnect Service Tool --*^>^* Miscellaneous tab. 
                // Perform iisreset" 

                //Set Session Timeout
                //servicetool.LaunchServiceTool();
                //servicetool.NavigateToTab("Security");
                //servicetool.WaitWhileBusy();
                //servicetool.ClickModifyFromTab();
                //servicetool.WaitWhileBusy();
                //servicetool.SetTimeout(timeout.Minutes);
                //servicetool.CloseServiceTool();

                // Create new Domain group D1 (default SuperAdminGroup) under Domain Management tab.
                // In Domain D1:
                // Create 5 users with Type as User.Referred to as USER1...USER5
                //Create 2 System Admin users with Type as SuperAdmin.Referred to SADMIN1 and SADMIN2
                // Create 3 Domain Admin users with Type as DomainAdmin.Referred to DADMIN3, DADMIN4, DADMIN5
                BasePage.MultiDriver.Add(BasePage.Driver);
                login.SetDriver(MultiDriver[0]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForPageLoad(10);
                //if (!domainmanagement.SearchDomain("SuperAdminGroup"))
                //{
                //    domainmanagement.CreateDomain("SuperAdminGroup", Role1, DS: new string[] { "print" }, datasources: new string[] { DatasourceAutoSSA, DatasourceVMSSA131, DatasourceVMSSA91, DatasourceSanityPACS, DatasourcePACS2 }, check: 1);
                //}
                // Step 1
                // Using the Same System
                // Remove all users(except Administrator) through User Management.
                //                 login.LoginGrid(Username, Password);
                // Delete all the users
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (usermanagement.SearchUser(User1))
                {
                    usermanagement.SearchUser(User1);
                    usermanagement.SelectUser(User1);
                    usermanagement.DelUsrBtn().Click();
                    PageLoadWait.WaitForPageLoad(10);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    PageLoadWait.WaitForPageLoad(20);
                    usermanagement.OkButtonConfirmGroupDeletionMsgBox().Click();
                    Driver.SwitchTo().DefaultContent();
                }

                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (usermanagement.SearchUser(User2))
                {
                    usermanagement.SearchUser(User2);
                    usermanagement.SelectUser(User2);
                    usermanagement.DelUsrBtn().Click();
                    PageLoadWait.WaitForPageLoad(10);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    PageLoadWait.WaitForPageLoad(20);
                    usermanagement.OkButtonConfirmGroupDeletionMsgBox().Click();
                    Driver.SwitchTo().DefaultContent();
                }
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (usermanagement.SearchUser(User3))
                {
                    usermanagement.SearchUser(User3);
                    usermanagement.SelectUser(User3);
                    usermanagement.DelUsrBtn().Click();
                    PageLoadWait.WaitForPageLoad(10);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    PageLoadWait.WaitForPageLoad(20);
                    usermanagement.OkButtonConfirmGroupDeletionMsgBox().Click();
                    Driver.SwitchTo().DefaultContent();
                }

                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (usermanagement.SearchUser(User4))
                {
                    usermanagement.SearchUser(User4);
                    usermanagement.SelectUser(User4);
                    usermanagement.DelUsrBtn().Click();
                    PageLoadWait.WaitForPageLoad(10);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    PageLoadWait.WaitForPageLoad(20);
                    usermanagement.OkButtonConfirmGroupDeletionMsgBox().Click();
                    Driver.SwitchTo().DefaultContent();
                }
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (usermanagement.SearchUser(User5))
                {
                    usermanagement.SearchUser(User5);
                    usermanagement.SelectUser(User5);
                    usermanagement.DelUsrBtn().Click();
                    PageLoadWait.WaitForPageLoad(10);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    PageLoadWait.WaitForPageLoad(20);
                    usermanagement.OkButtonConfirmGroupDeletionMsgBox().Click();
                    Driver.SwitchTo().DefaultContent();
                }
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (usermanagement.SearchUser(SystemAdmin1))
                {
                    usermanagement.SearchUser(SystemAdmin1);
                    usermanagement.SelectUser(SystemAdmin1);
                    usermanagement.DelUsrBtn().Click();
                    PageLoadWait.WaitForPageLoad(10);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    PageLoadWait.WaitForPageLoad(20);
                    usermanagement.OkButtonConfirmGroupDeletionMsgBox().Click();
                    Driver.SwitchTo().DefaultContent();
                }
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (usermanagement.SearchUser(SystemAdmin2))
                {
                    usermanagement.SearchUser(SystemAdmin2);
                    usermanagement.SelectUser(SystemAdmin2);
                    usermanagement.DelUsrBtn().Click();
                    PageLoadWait.WaitForPageLoad(10);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    PageLoadWait.WaitForPageLoad(20);
                    usermanagement.OkButtonConfirmGroupDeletionMsgBox().Click();
                    Driver.SwitchTo().DefaultContent();
                }

                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (usermanagement.SearchUser(DomainAdmin1))
                {
                    usermanagement.SearchUser(DomainAdmin1);
                    usermanagement.SelectUser(DomainAdmin1);
                    usermanagement.DelUsrBtn().Click();
                    PageLoadWait.WaitForPageLoad(10);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    PageLoadWait.WaitForPageLoad(20);
                    usermanagement.OkButtonConfirmGroupDeletionMsgBox().Click();
                    Driver.SwitchTo().DefaultContent();
                }


                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (usermanagement.SearchUser(DomainAdmin2))
                {
                    usermanagement.SearchUser(DomainAdmin2);
                    usermanagement.SelectUser(DomainAdmin2);
                    usermanagement.DelUsrBtn().Click();
                    PageLoadWait.WaitForPageLoad(10);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    PageLoadWait.WaitForPageLoad(20);
                    usermanagement.OkButtonConfirmGroupDeletionMsgBox().Click();
                    Driver.SwitchTo().DefaultContent();
                }
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (usermanagement.SearchUser(DomainAdmin3))
                {
                    usermanagement.SearchUser(DomainAdmin3);
                    usermanagement.SelectUser(DomainAdmin3);
                    usermanagement.DelUsrBtn().Click();
                    PageLoadWait.WaitForPageLoad(10);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    PageLoadWait.WaitForPageLoad(20);
                    usermanagement.OkButtonConfirmGroupDeletionMsgBox().Click();
                    Driver.SwitchTo().DefaultContent();
                }

                login.Logout();
                ExecutedSteps++;


                // Step 2
                if (File.Exists("C:\\WebAccess\\WebAccess\\Config\\License.xml"))
                {
                    if (!System.IO.Directory.Exists("C:\\WadoWS_Temp"))
                    {
                        System.IO.Directory.CreateDirectory("C:\\WadoWS_Temp");
                    }

                    if (File.Exists("C:\\WadoWS_Temp\\License_1.xml"))
                    {
                        File.Delete("C:\\WadoWS_Temp\\License_1.xml");
                        File.Copy("C:\\WebAccess\\WebAccess\\Config\\License.xml", "C:\\WadoWS_Temp\\License_1.xml");
                    }
                    else
                    {
                        File.Copy("C:\\WebAccess\\WebAccess\\Config\\License.xml", "C:\\WadoWS_Temp\\License_1.xml");
                    }
                    ChangeCDATAValueInXml("C:\\WebAccess\\WebAccess\\Config\\License.xml", "Invalid License Key");
                    servicetool.InvokeServiceTool();
                    wpfobject.WaitTillLoad();
                    servicetool.RestartService();
                    wpfobject.WaitTillLoad();
                    servicetool.CloseServiceTool();
                    ExecutedSteps++;
                }
                else
                {
                    servicetool.InvokeServiceTool();
                    wpfobject.WaitTillLoad();
                    servicetool.AddLicenseInConfigTool(ServiceTool.License.FilePath);
                    wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                    servicetool.RestartService();
                    ExecutedSteps++;
                }


                // Step 3 
                // Login from WASYS as Administrator.
                login.SetDriver(MultiDriver[0]);
                login.LoginGrid(Username, Password);
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

                // Step 4 
                // License the server using the 1 webaccess.base, 1 webaccess.maxuser, 1 webaccess.admin license:
                // Open the Merge iConnect service tool and apply this License.

                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.AddLicenseInConfigTool(Onebase1maxusers1adminpath);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                // Step 5
                // Login from WASYS as Administrator.
                // Refer to Pre-condition 7, and verify both methods give the same result.
                login.SetDriver(BasePage.MultiDriver[0]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                MaxUsercount = actual.Where(val => string.Equals(val, "webaccess.maxusers 2.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (MaxUsercount == 1)
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

                // Step 6
                // From User Management tab, create a user USER1
                login.SetDriver(MultiDriver[0]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(10);
                // usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                PageLoadWait.WaitForPageLoad(10);

                if (!usermanagement.SearchUser(User1))
                {
                    usermanagement.ClickNewUser();
                    Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                }
                if (usermanagement.DeleteUserErrorLabel().Text.Equals("You cannot add another registered user to the system. The licensed limit has been reached.") && !usermanagement.SearchUser(User1))
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
                // Test Data: 
                // The webaccess.admin license should not affect the number of users that can be registered.
                // From User Management tab, create a user SADMIN1 with Admin in the role name

                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(10);
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                PageLoadWait.WaitForPageLoad(10);

                if (!usermanagement.SearchUser(SystemAdmin1))
                {
                    usermanagement.ClickNewUser();
                    Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                }
                if (usermanagement.DeleteUserErrorLabel().Text.Equals("You cannot add another registered user to the system. The licensed limit has been reached.") && !usermanagement.SearchUser(SystemAdmin1))
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


                // Step 8
                // Logout all sessions.
                // License the server using the 4 webaccess.maxuser, 1 webaccess.admin license
                // Open the Merge iConnect service tool and apply this License.

                login.SetDriver(MultiDriver[0]);
                login.Logout();

                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.AddLicenseInConfigTool(Fourmaxusers1adminpath);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                // Step 9
                // Login from WASYS as Administrator.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                login.SetDriver(BasePage.MultiDriver[0]);
                PageLoadWait.WaitForPageLoad(10);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
                PageLoadWait.WaitForPageLoad(10);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                MaxUsercount = actual.Where(val => string.Equals(val, "webaccess.maxusers 2.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (MaxUsercount == 1)
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

                // Step 10
                // Login from CLIENTSYS1 as Administrator using an incorrect password.
                Config.node = Config.Clientsys1;
                BasePage.MultiDriver.Add(login.InvokeBrowser("Remote-" + Config.BrowserType));
                login.SetDriver(MultiDriver[1]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password + "incorrect");
                PageLoadWait.WaitForPageLoad(10);
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

                // Step 11
                // Login from WASYS as a non-existant user.
                login.SetDriver(MultiDriver[1]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username + "123", Password);
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

                // Step 12
                //Test Data: 
                // 2 users are now created
                // From User Management tab, create a user USER1

                login.SetDriver(MultiDriver[0]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                // usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(User1))
                {
                    usermanagement.CreateUser(User1, Role1);
                }
                if (usermanagement.SearchUser(User1))
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

                // Step 13
                // Test Data: 3 users are now created.
                // From User Management tab, create a user USER2
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                // usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(User2))
                {
                    usermanagement.CreateUser(User2, Role1);
                }
                if (usermanagement.SearchUser(User2))
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

                // Step 14
                // Test Data: 3 users are now created.
                // From User Management tab, create a user USER2
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                // usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(User3))
                {
                    usermanagement.CreateUser(User3, Role1);
                }
                if (usermanagement.SearchUser(User3))
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

                // Step 15
                // Test Data: 4 users are now created.
                // From User Management tab, create a user USER3
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                // usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(User4))
                {
                    SwitchToDefault();
                    Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
                    Click("cssselector", " table#userMainTabBarTabControl div#TabText0");
                    Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                    Click("cssselector", " #NewUserButton");
                }
                if (usermanagement.DeleteUserErrorLabel().Text.Equals("You cannot add another registered user to the system. The licensed limit has been reached.") && !usermanagement.SearchUser(User4))
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

                // Step 16
                // From User Management tab, create an admin SADMIN1
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (!usermanagement.SearchUser(SystemAdmin1))
                {
                    SwitchToDefault();
                    Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
                    Click("cssselector", " table#userMainTabBarTabControl div#TabText0");
                    Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                    Click("cssselector", " #NewUserButton");
                }
                if (usermanagement.DeleteUserErrorLabel().Text.Equals("You cannot add another registered user to the system. The licensed limit has been reached.") && !usermanagement.SearchUser(SystemAdmin1))
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

                // Step 17
                // Login from CLIENTSYS1 as USER1.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                login.SetDriver(MultiDriver[1]);
                login.DriverGoTo(login.url);
                login.LoginGrid(User1, User1);

                login.SetDriver(MultiDriver[0]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);

                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                MaxUsercount = actual.Where(val => string.Equals(val, "webaccess.maxusers 2.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (MaxUsercount == 2)
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

                // Step 18
                // Attach a file to the study.

                login.SetDriver(MultiDriver[1]);
                studies = (Studies)login.Navigate("Studies");

                //Search,Select,Launch study
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SearchStudy(studyID: StudyID, Datasource: DatasourceSanityPACS);
                studies.SelectStudy1("Study ID", StudyID);
                viewer = StudyViewer.LaunchStudy();

                // STep 19

                viewer.NavigateToHistoryPanel();
                viewer.NavigateTabInHistoryPanel("Attachment");
                PageLoadWait.WaitForFrameLoad(20);
                bool attachment = viewer.UploadAttachment("", 20);
                if (attachment)
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


                // STep 20 
                // Login from CLIENTSYS2 as USER2.
                // Refer to Pre-condition 7, and verify both methods give the same result.
                Config.node = Config.Clientsys2;
                BasePage.MultiDriver.Add(login.InvokeBrowser("Remote-" + Config.BrowserType));
                login.SetDriver(MultiDriver[2]);
                login.DriverGoTo(login.url);
                login.LoginGrid(User2, User2);
                PageLoadWait.WaitForPageLoad(10);

                login.SetDriver(MultiDriver[0]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                MaxUsercount = actual.Where(val => string.Equals(val, "webaccess.maxusers 2.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (MaxUsercount == 3)
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

                // STep 21
                // Login from CLIENTSYS3 as USER3.
                // Refer to Pre-condition 7, and verify both methods give the same result.
                Config.node = Config.Clientsys3;
                BasePage.MultiDriver.Add(login.InvokeBrowser("Remote-" + Config.BrowserType));
                login.SetDriver(MultiDriver[3]);
                login.DriverGoTo(login.url);
                login.LoginGrid(User3, User3);
                PageLoadWait.WaitForPageLoad(10);

                login.SetDriver(MultiDriver[0]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                MaxUsercount = actual.Where(val => string.Equals(val, "webaccess.maxusers 2.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (MaxUsercount == 4)
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

                // Step 22
                // Test Data: Try through a new browser tab as well as another browser window.
                // Through a new browser tab, will see an error stating there is already an active session.
                // Login from CLIENTSYS3 as USER4
                // Refer to Pre - condition 7, and verify both methods give the same result.
                login.SetDriver(MultiDriver[3]);
                login.DriverGoTo(login.url);
                login.LoginGrid(User4, User4);
                PageLoadWait.WaitForPageLoad(10);
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


                // Step 23 
                // Login from CLIENTSYS3 as Administrator.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                login.SetDriver(MultiDriver[3]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                MaxUsercount = actual.Where(val => string.Equals(val, "webaccess.maxusers 2.0", StringComparison.OrdinalIgnoreCase)).Count();
                WebAdmincount = actual.Where(val => string.Equals(val, "webaccess.admin 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (MaxUsercount == 4 && WebAdmincount == 1)
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

                // Step 24
                //Login from CLIENTSYS1 as Administrator.
                // Refer to Pre-condition 7, and verify both methods give the same result.
                login.SetDriver(MultiDriver[1]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
                PageLoadWait.WaitForPageLoad(10);
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
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.AddLicenseInConfigTool(Licbackpath);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                Config.node = "localhost";
                login.closeallbrowser();
                login.InvokeBrowser(Config.BrowserType);
            }
        }

        public TestCaseResult Test_27660(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            Login login = new Login();
            DomainManagement domainmanagement = null;
            UserManagement usermanagement = null;
            Maintenance maintenance = null;
            MultiDriver = new List<IWebDriver>();
            WpfObjects wpfobject = new WpfObjects();
            ServiceTool servicetool = new ServiceTool();
            result = new TestCaseResult(stepcount);
            Stopwatch stopwatch = new Stopwatch();
            StudyViewer viewer = new StudyViewer();
            int ExecutedSteps = -1;
            int MaxUsercount = 0;
            int WebAdmincount = 0;
            DataTable datatable;
            String[] actual;

            string Onebase1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "1base1adminpath");
            string Onebase0adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "1base0adminpath");
            string Zerobase1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "0base1adminpath");
            string Onebase1maxusers1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "1base1maxusers1adminpath");
            string Twobase1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "2base1adminpath");
            string Threebase1admin = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "3base1admin");
            string Threemaxusers1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "3maxusers1adminpath");
            string Fourmaxusers1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "4maxusers1adminpath");
            string Licbackpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Licbackpath");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data                       
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;


                BasePage.MultiDriver.Add(BasePage.Driver);
                login.SetDriver(MultiDriver[0]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForPageLoad(10);
                // Step 1
                // Create new Domain group D1 (default SuperAdminGroup) under Domain Management tab.
                // In Domain D1:
                // Create 5 users with Type as User.Referred to as USER1...USER5
                //Create 2 System Admin users with Type as SuperAdmin.Referred to SADMIN1 and SADMIN2
                // Create 3 Domain Admin users with Type as DomainAdmin.Referred to DADMIN3, DADMIN4, DADMIN5
                //if (!domainmanagement.SearchDomain("SuperAdminGroup"))
                //{
                //    domainmanagement.CreateDomain("SuperAdminGroup", Role1, DS: new string[] { "print" }, datasources: new string[] { DatasourceAutoSSA, DatasourceVMSSA131, DatasourceVMSSA91, DatasourceSanityPACS, DatasourcePACS2 }, check: 1);
                //}

                domainmanagement.SearchDomain("SuperAdminGroup");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.EditDomainButton().Click();
                Driver.SwitchTo().DefaultContent();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForPageLoad(10);
                domainmanagement.VisibleAllStudySearchField();
                domainmanagement.SaveDomainButtoninEditPage().Click();

                //// Delete all the users
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (usermanagement.SearchUser(User1))
                {
                    usermanagement.SearchUser(User1);
                    usermanagement.SelectUser(User1);
                    usermanagement.DelUsrBtn().Click();
                    PageLoadWait.WaitForPageLoad(10);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    PageLoadWait.WaitForPageLoad(20);
                    usermanagement.OkButtonConfirmGroupDeletionMsgBox().Click();
                    Driver.SwitchTo().DefaultContent();
                }

                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (usermanagement.SearchUser(User2))
                {
                    usermanagement.SearchUser(User2);
                    usermanagement.SelectUser(User2);
                    usermanagement.DelUsrBtn().Click();
                    PageLoadWait.WaitForPageLoad(10);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    PageLoadWait.WaitForPageLoad(20);
                    usermanagement.OkButtonConfirmGroupDeletionMsgBox().Click();
                    Driver.SwitchTo().DefaultContent();
                }
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (usermanagement.SearchUser(User3))
                {
                    usermanagement.SearchUser(User3);
                    usermanagement.SelectUser(User3);
                    usermanagement.DelUsrBtn().Click();
                    PageLoadWait.WaitForPageLoad(10);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    PageLoadWait.WaitForPageLoad(20);
                    usermanagement.OkButtonConfirmGroupDeletionMsgBox().Click();
                    Driver.SwitchTo().DefaultContent();
                }

                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (usermanagement.SearchUser(User4))
                {
                    usermanagement.SearchUser(User4);
                    usermanagement.SelectUser(User4);
                    usermanagement.DelUsrBtn().Click();
                    PageLoadWait.WaitForPageLoad(10);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    PageLoadWait.WaitForPageLoad(20);
                    usermanagement.OkButtonConfirmGroupDeletionMsgBox().Click();
                    Driver.SwitchTo().DefaultContent();
                }

                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (usermanagement.SearchUser(SystemAdmin1))
                {
                    usermanagement.SearchUser(SystemAdmin1);
                    usermanagement.SelectUser(SystemAdmin1);
                    usermanagement.DelUsrBtn().Click();
                    PageLoadWait.WaitForPageLoad(10);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    PageLoadWait.WaitForPageLoad(20);
                    usermanagement.OkButtonConfirmGroupDeletionMsgBox().Click();
                    Driver.SwitchTo().DefaultContent();
                }
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (usermanagement.SearchUser(SystemAdmin2))
                {
                    usermanagement.SearchUser(SystemAdmin2);
                    usermanagement.SelectUser(SystemAdmin2);
                    usermanagement.DelUsrBtn().Click();
                    PageLoadWait.WaitForPageLoad(10);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    PageLoadWait.WaitForPageLoad(20);
                    usermanagement.OkButtonConfirmGroupDeletionMsgBox().Click();
                    Driver.SwitchTo().DefaultContent();
                }

                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (usermanagement.SearchUser(DomainAdmin1))
                {
                    usermanagement.SearchUser(DomainAdmin1);
                    usermanagement.SelectUser(DomainAdmin1);
                    usermanagement.DelUsrBtn().Click();
                    PageLoadWait.WaitForPageLoad(10);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    PageLoadWait.WaitForPageLoad(20);
                    usermanagement.OkButtonConfirmGroupDeletionMsgBox().Click();
                    Driver.SwitchTo().DefaultContent();
                }


                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (usermanagement.SearchUser(DomainAdmin2))
                {
                    usermanagement.SearchUser(DomainAdmin2);
                    usermanagement.SelectUser(DomainAdmin2);
                    usermanagement.DelUsrBtn().Click();
                    PageLoadWait.WaitForPageLoad(10);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    PageLoadWait.WaitForPageLoad(20);
                    usermanagement.OkButtonConfirmGroupDeletionMsgBox().Click();
                    Driver.SwitchTo().DefaultContent();
                }
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                if (usermanagement.SearchUser(DomainAdmin3))
                {
                    usermanagement.SearchUser(DomainAdmin3);
                    usermanagement.SelectUser(DomainAdmin3);
                    usermanagement.DelUsrBtn().Click();
                    PageLoadWait.WaitForPageLoad(10);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    PageLoadWait.WaitForPageLoad(20);
                    usermanagement.OkButtonConfirmGroupDeletionMsgBox().Click();
                    Driver.SwitchTo().DefaultContent();
                }

                login.Logout();
                PageLoadWait.WaitForPageLoad(10);

                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.AddLicenseInConfigTool(Threemaxusers1adminpath);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();

                // Step 1
                // Login from WASYS as Administrator.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                login.SetDriver(BasePage.MultiDriver[0]);
                PageLoadWait.WaitForPageLoad(10);
                login.LoginGrid(Username, Password);
                PageLoadWait.WaitForPageLoad(10);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                MaxUsercount = actual.Where(val => string.Equals(val, "webaccess.maxusers 2.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (MaxUsercount == 1)
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

                // Step 2 
                // From User Management tab, create a user USER1
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(10);
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                PageLoadWait.WaitForPageLoad(10);

                if (!usermanagement.SearchUser(User1))
                {
                    usermanagement.CreateUser(User1, Role1);
                }
                bool expected = usermanagement.SearchUser(User1);

                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                MaxUsercount = actual.Where(val => string.Equals(val, "webaccess.maxusers 2.0", StringComparison.OrdinalIgnoreCase)).Count();

                if (MaxUsercount == 1 && expected)
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

                // Step 3
                // Test Data 3 users are now created.
                // From User Management tab, create an admin SADMIN1
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(10);
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                PageLoadWait.WaitForPageLoad(10);
                if (!usermanagement.SearchUser(SystemAdmin1))
                {
                    usermanagement.CreateSystemAdminUser(SystemAdmin1, "SuperAdminGroup");
                }

                expected = usermanagement.SearchUser(SystemAdmin1);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                MaxUsercount = actual.Where(val => string.Equals(val, "webaccess.maxusers 2.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (MaxUsercount == 1 && expected)
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
                // From User Management tab, create a user USER2
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(10);
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                PageLoadWait.WaitForPageLoad(10);

                if (!usermanagement.SearchUser(User2))
                {
                    SwitchToDefault();
                    Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
                    Click("cssselector", " table#userMainTabBarTabControl div#TabText0");
                    Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                    Click("cssselector", " #NewUserButton");
                }
                if (usermanagement.DeleteUserErrorLabel().Text.Equals("You cannot add another registered user to the system. The licensed limit has been reached.") && !usermanagement.SearchUser(User2))
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

                // Step 5
                // From User Management tab, create an admin SADMIN1
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(10);
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                PageLoadWait.WaitForPageLoad(10);

                if (!usermanagement.SearchUser(SystemAdmin1))
                {
                    SwitchToDefault();
                    Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
                    Click("cssselector", " table#userMainTabBarTabControl div#TabText0");
                    Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                    Click("cssselector", " #NewUserButton");
                }
                if (usermanagement.DeleteUserErrorLabel().Text.Equals("You cannot add another registered user to the system. The licensed limit has been reached."))
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

                // Step 6
                // Login from CLIENTSYS1 as USER1.
                // Refer to Pre - condition 7, and verify both methods give the same result.
                Config.node = Config.Clientsys1;
                BasePage.MultiDriver.Add(login.InvokeBrowser("Remote-" + Config.BrowserType));
                login.SetDriver(BasePage.MultiDriver[1]);
                login.DriverGoTo(login.url);
                PageLoadWait.WaitForPageLoad(10);
                login.LoginGrid(User1, User1);

                login.SetDriver(BasePage.MultiDriver[0]);
                login.DriverGoTo(login.url);
                PageLoadWait.WaitForPageLoad(10);
                login.LoginGrid(Username, Password);

                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                MaxUsercount = actual.Where(val => string.Equals(val, "webaccess.maxusers 2.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (MaxUsercount == 2)
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
                // Login from CLIENTSYS2 as USER1.
                // Refer to Pre-condition 7, and verify both methods give the same result.
                Config.node = Config.Clientsys2;
                BasePage.MultiDriver.Add(login.InvokeBrowser("Remote-" + Config.BrowserType));
                login.SetDriver(BasePage.MultiDriver[2]);
                login.DriverGoTo(login.url);
                PageLoadWait.WaitForPageLoad(10);
                login.LoginGrid(User1, User1);

                login.SetDriver(BasePage.MultiDriver[0]);
                login.DriverGoTo(login.url);
                PageLoadWait.WaitForPageLoad(10);
                login.LoginGrid(Username, Password);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                MaxUsercount = actual.Where(val => string.Equals(val, "webaccess.maxusers 2.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (MaxUsercount == 3)
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

                // Step 8 
                // Login from CLIENTSYS3 as SADMIN1.
                // Refer to Pre-condition 7, and verify both methods give the same result.
                Config.node = Config.Clientsys3;
                BasePage.MultiDriver.Add(login.InvokeBrowser("Remote-" + Config.BrowserType));
                login.SetDriver(BasePage.MultiDriver[3]);
                login.DriverGoTo(login.url);
                PageLoadWait.WaitForPageLoad(10);
                login.LoginGrid(SystemAdmin1, SystemAdmin1);

                login.SetDriver(BasePage.MultiDriver[0]);
                login.DriverGoTo(login.url);
                PageLoadWait.WaitForPageLoad(10);
                login.LoginGrid(Username, Password);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                datatable = CollectRecordsInTable(maintenance.StatisticsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                actual = GetColumnValues(datatable, "Feature Name");
                MaxUsercount = actual.Where(val => string.Equals(val, "webaccess.maxusers 2.0", StringComparison.OrdinalIgnoreCase)).Count();
                WebAdmincount = actual.Where(val => string.Equals(val, "webaccess.admin 1.0", StringComparison.OrdinalIgnoreCase)).Count();
                if (MaxUsercount == 3 && WebAdmincount == 1)
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

                // Step 9
                // Login from CLIENTSYS2 as SADMIN1.
                // Refer to Pre-condition 7, and verify both methods give the same result.
                Config.node = Config.Clientsys3;
                BasePage.MultiDriver.Add(login.InvokeBrowser("Remote-" + Config.BrowserType));
                login.SetDriver(MultiDriver[4]);
                login.DriverGoTo(login.url);
                PageLoadWait.WaitForPageLoad(10);
                login.LoginGrid(SystemAdmin1, SystemAdmin1);
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
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.AddLicenseInConfigTool(Licbackpath);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                Config.node = "localhost";
                login.closeallbrowser();
                login.InvokeBrowser(Config.BrowserType);
            }
        }

        public TestCaseResult Test_27661(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            Login login = new Login();
            MultiDriver = new List<IWebDriver>();
            TimeSpan timeout = new TimeSpan(0, 2, 0);
            Stopwatch stopwatch = new Stopwatch();
            servicetool = new ServiceTool();
            wpfobject = new WpfObjects();

            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            string Onebase1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "1base1adminpath");
            string Onebase0adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "1base0adminpath");
            string Zerobase1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "0base1adminpath");
            string Onebase1maxusers1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "1base1maxusers1adminpath");
            string Twobase1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "2base1adminpath");
            string Threebase1admin = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "3base1admin");
            string Threemaxusers1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "3maxusers1adminpath");
            string Fourmaxusers1adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "4maxusers1adminpath");
            string Licbackpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Licbackpath");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data                       
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;

                String SystemConfigXmlFileLocation = "C:\\WebAccess\\WebAccess\\Config\\SystemConfiguration.xml";
                BasePage.MultiDriver.Add(BasePage.Driver);
                login.SetDriver(MultiDriver[0]);
                String NodePath = @"Configuration/LicenseUsageLoggingInterval";

                // Current Time : 
                String Currenttime = DateTime.Now.ToString("HH:mm:ss");
                // Current Date :
                String CurrentDate = DateTime.Today.ToString("MM/dd/yy");

                // Step 1 
                // Open license usage log file:
                // ..\WINDOWS\Temp\WebAccessLicense - YYYY - MM.log  where YYYY is the current year and MM is the current month.

                if (File.Exists(SystemConfigXmlFileLocation))
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

                // Step 2
                // Check the time interval.
                XmlDocument xmlDocument = new XmlDocument();
                // Load the XML file in to the document
                xmlDocument.Load(SystemConfigXmlFileLocation);
                //Get Parent Node
                XmlNode Node = xmlDocument.SelectSingleNode("/" + NodePath);
                //Change Value 
                String value = Node.InnerText;
                if (value.Equals("1"))
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

                // Step 3
                // Close the log file.
                ExecutedSteps++;

                // Step 4 , 5 & 6
                // Select the system Date and time.
                // Select the last date of the month.
                // In the time box, type 11:58:30 PM.Select Apply button.
                var lastDayOfMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                ExecutedSteps++;
                Process proc = new Process();
                proc.StartInfo.FileName = "cmd";
                proc.StartInfo.WorkingDirectory = "C:\\Windows\\System32";
                proc.StartInfo.Arguments = "/C date " + DateTime.Now.Month + "-" + lastDayOfMonth + "-" + DateTime.Now.Year + " & time 23:58:30";
                proc.Start();
                proc.WaitForExit(20000);
                ExecutedSteps++;
                ExecutedSteps++;

                // Step 7
                // Wait for 2 minutes until the a new month is displayed.
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout)) { /*Stay Idle for 2 Miniutes*/ }
                int actualtimeout = stopwatch.Elapsed.Minutes;
                stopwatch.Stop();
                stopwatch.Reset();
                ExecutedSteps++;

                // Step 8
                // Logout and login the iConnect Access.
                // Check the log file.
                login.SetDriver(MultiDriver[0]);
                login.DriverGoTo(login.url);
                login.LoginGrid(Username, Password);
                String Month = DateTime.Now.Month.ToString("d2");
                String Year = DateTime.Now.Year.ToString();

                bool a = File.Exists("C:\\Windows\\Temp\\WebAccessLicense-" + Year + "-" + Month + ".log");
                if (a)
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


                Process proc1 = new Process();
                proc1.StartInfo.FileName = "cmd";
                proc1.StartInfo.WorkingDirectory = "C:\\Windows\\System32";
                proc1.StartInfo.Arguments = "/C date " + CurrentDate + " & time " + Currenttime;
                proc1.Start();
                proc1.WaitForExit(20000);

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
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.AddLicenseInConfigTool(Licbackpath);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                Config.node = "localhost";
                login.closeallbrowser();

                servicetool.NavigateToTab("Security");
                servicetool.WaitWhileBusy();
                servicetool.ClickModifyFromTab();
                servicetool.WaitWhileBusy();
                timeout = new TimeSpan(0, 30, 0);
                servicetool.SetTimeout(timeout.Minutes);
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
            }
        }

    }
}