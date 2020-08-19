using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Reusable.Generic;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.Putty;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.MergeServiceTool;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections;

namespace Selenium.Scripts.Tests
{
    class EnhancedStudyList
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public ExamImporter ei { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public HPHomePage hphomepage { get; set; }
        public HPLogin hplogin { get; set; }
        public ServiceTool servicetool { get; set; }
        public static string SBrowserName { get; set; }
        public static string BrowserVersion { get; set; }
        public WpfObjects wpfobject { get; set; }

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        public EnhancedStudyList(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            hplogin = new HPLogin();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            ei = new ExamImporter();
            hphomepage = new HPHomePage();
            mpaclogin = new MpacLogin();
            servicetool = new ServiceTool();
            BrowserVersion = ((RemoteWebDriver)BasePage.Driver).Capabilities.Version;
            SBrowserName = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName;
            wpfobject = new WpfObjects();
        }         

        /// <summary>
        /// Layout Configuration - Domain Level
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_162483(String testid, String teststeps, int stepcount)
        {

            #region TestSummary
            /* Summary of Test
        step 1-3
        1.Change study search field order and validate the same in studies page.
        2.Validate option available to save study list columns.

        step-4 
        Peform study search and sort all columns

        step-5-8
        Reorder study list columns in studies tab and re-login and validtate if the change is reflected. And verify columns can be dragged

        step-9-24 
        Valiadte the study list layout changes made in TestDomain edit page is getting saved and reflected in same screen.
        Plus sign - #colchooser_gridTableStudyList > div > div > div.available > ul > li> a > span[class*='ui-icon-plus']
        Minus Sign - #colchooser_gridTableStudyList > div > div > div.selected > ul > li> a > span[class*='ui-icon-minus']

        step-25-26
        Validate close button in study list layout popup in TestDomain

        step-27-28
        a) Check allow user to save study layout in Test Domain. b)Re-arrange study search fieldsLogin as Site Domain Admin and validate Study column list layout and study 

        search fileds are as per above settings.

        step-29
        Validate for a normal user 'a'(TestRoleA) the study search feild settings in studies page will be as per the Role level.

        step-30, 31
        Validate on Role Edtit page the study list layout settings will be defaulted to Domain.

        Step-32
        Login as user 'a' and validate the study list layout is as per the Domain level settings.

        step33
        Click choose column and validate selected column in pop up matched the one displayed in studies page.

        step34
        Load the study and validate.

        step35-38
        Login in as Site Admin and modify the study list layout in TestRoleA. And validate the same in studies tab.
        Login in as normal user and check the study list layout is as per the recent changes made.

        step-39 to 41
        Login in as Administrator and update study list layout and verify it as Administrator user and normal user 'a'

        step-42
        Login as user 'b' and validate study list layout */

            #endregion TestSummary

            //Declare and initialize variables            
            TestCaseResult result = new TestCaseResult(stepcount);
            RoleManagement rolemgmt;
            UserManagement usermgmt;
            Studies studies;
            int executedSteps = -1;
            BasePage basepage = new BasePage();

            String studylistcolumns_Selected = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DefaultStudyLayoutSelected");
            String[] studycolumns_selected = studylistcolumns_Selected.Split(':');
            String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
            String patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
			String lastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");

			String domainname = "SuperAdminGroup";
            String superrole = "SuperRole";
            By tablesearchfields = By.CssSelector("table[id = 'customSearchTable'] td[class='searchCriteriaMiddle'] span[id^='m_studySearch']");

            Random randomnumber = new Random();
            String TestRoleB = "TestRoleB" + new System.DateTime().Second + randomnumber.Next(1, 1000);
            String TestRoleA = "TestRoleA" + new System.DateTime().Second + randomnumber.Next(1, 1000);
            String userB = "ph1" + new System.DateTime().Second + randomnumber.Next(1, 1000);
            String userBB = "ph1" + new System.DateTime().Second + randomnumber.Next(1, 1000);
            String userA = "ar1" + new System.DateTime().Second + randomnumber.Next(1, 1000);
            String userAA = "ar1" + new System.DateTime().Second + randomnumber.Next(1, 1000);

            //Delegate - Study list column names
            Func<IWebElement, String> studylistcolumnnames = (element) =>
            {
                if (!element.GetAttribute("style").Contains("none"))
                {
                    return element.GetAttribute("title");
                }
                else
                {
                    return null;
                }
            };

            //Delegate -Study search field names
            Func<IWebElement, String> studysearchfields = (element) =>
            {
                if (!element.GetAttribute("innerHTML").Equals("My Patients Only"))
                {
                    return element.GetAttribute("innerHTML");
                }
                else
                {
                    return null;
                }

            };

            try
            {
                //Set validation steps
                result.SetTestStepDescription(teststeps);

                //Step-1-Login as Admin
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                bool searchField = basepage.LastName().Displayed  && basepage.FirstName().Displayed ; 
                if (login.IsTabSelected("Studies") && searchField)
                {
                    result.steps[++executedSteps].StepPass();
                }
                else
                {
                    result.steps[++executedSteps].StepFail();
                }

                //Step-2 - Update  settings to allow user to save study layout and rearrange search criteria fields.  
                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain(domainname);
                domain.SelectDomain(domainname);
                domain.ClickEditDomain();
                domain.SetCheckbox(domain.SaveStudyLayout());
                domain.ModifyStudySearchFields("Hide", new String[] { "First Name", "Gender", "IPID" });
                domain.ReorderStudySearchFields(0, 2, "Down");
                String[] searchfields = domain.ReorderStudySearchFields(1, 1, "Down");
                domain.ClickSaveEditDomain();
                RoleManagement role = (RoleManagement)login.Navigate("RoleManagement");
                role.SelectDomainfromDropDown(domainname);
                role.SearchRole(superrole);
                role.SelectRole(superrole);
                role.ClickEditRole();
                role.SetCheckbox(role.AllowUserToSaveStudyLayout());
                role.SetCheckbox(role.UserDomainSettingsSearchFields());
                role.ClickSaveEditRole();
                executedSteps++;

                //Step-3 - Navigate to Studies Tab and check serch fields order.
                studies = (Studies)login.Navigate("Studies");
                IList<IWebElement> studypage_searchelement = BasePage.Driver.FindElements(tablesearchfields);
                String[] searchfieldsactual = studypage_searchelement.Select<IWebElement, String>((element) =>
                {

                    if (!element.GetAttribute("innerHTML").Equals("My Patients Only"))
                    {
                        return element.GetAttribute("innerHTML");
                    }
                    else
                    {
                        return null;
                    }

                }).ToArray();
                searchfieldsactual = searchfieldsactual.Where((field) =>
                {
                    if (!String.IsNullOrEmpty(field))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }).ToArray();

                #region Logic to check column order are same
                if (searchfieldsactual.Length == searchfields.Length)
                {

                    String[] regexpressions = searchfieldsactual.Select<String, String>((fieldname) =>
                    {
                        if (!String.IsNullOrEmpty(fieldname))
                        {
                            String[] arr = fieldname.Split(' ');
                            int counter = 0;
                            String regex = "";
                            foreach (string subsfieldname in arr)
                            {
                                if (counter == 0)
                                {
                                    regex = regex + subsfieldname.Substring(0, 1) + ".*";
                                }
                                else
                                {
                                    regex = regex + @"[\s]" + subsfieldname.Substring(0, 1) + ".*";
                                }
                                counter++;
                            }
                            return regex;
                        }
                        return null;

                    }).ToArray();

                    IEnumerator<String> iterator = searchfields.ToList().GetEnumerator();
                    Boolean isfieldsinorder = true;
                    iterator.MoveNext();
                    foreach (String regex in regexpressions)
                    {
                        if (!String.IsNullOrEmpty(regex))
                        {
                            if (!Regex.IsMatch(iterator.Current, regex))
                            {
                                isfieldsinorder = false;
                            }
                        }
                        iterator.MoveNext();
                    }
                    #endregion Logic to check column order are same
                    if (isfieldsinorder)
                    {
                        result.steps[++executedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                    }
                    else
                    {
                        result.steps[++executedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                        result.steps[executedSteps].SetLogs();
                    }
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

				//Step-4 - Perform study and validate sort
				studies.SearchStudy("Last Name", lastName);
				PageLoadWait.WaitForLoadingMessage(120);
				PageLoadWait.WaitForSearchLoad();
				bool lastNameDisplayed = false;
				String[] lastNameList = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Patient Name", BasePage.GetColumnNames());
				Logger.Instance.InfoLog(string.Join(", ", lastNameList));
				if (lastNameList.Length != 0)
					lastNameDisplayed = lastNameList.All(name => name.ToLower().StartsWith(lastName.ToLower()));
				if (lastNameDisplayed)
				{
					result.steps[++executedSteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
				}
				else
				{
					result.steps[++executedSteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedSteps].description);
					result.steps[executedSteps].SetLogs();
				}

				//Step-5 - Drag Patient name to end
				studies.ChooseColumns(new String[] { "Issuer of PID" });
                String[] colmnnames = BasePage.GetColumnNames();
                studies.ReorderStudyListColumns("Patient Name", "start");
                int index_before = Array.FindIndex<String>(colmnnames, column => column.Equals("Patient Name"));
                studies.ReorderStudyListColumns("Patient Name", "end");
                Thread.Sleep(1000);
                studies.ReorderStudyListColumns("Patient Name", "end");
                colmnnames = BasePage.GetColumnNames();
                int index_after = Array.FindIndex<String>(colmnnames, column => column.Equals("Patient Name"));
                Logger.Instance.InfoLog("index_after--" + index_after);
                Logger.Instance.InfoLog("index_before--" + index_before);
                if (index_after > index_before)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Set-6 - Drag Patient name to middle
                studies.ReorderStudyListColumns("Patient Name", "middle");
                Thread.Sleep(1000);
                studies.ReorderStudyListColumns("Patient Name", "middle");
                String[] colmnnames1 = BasePage.GetColumnNames();
                int index_after2 = Array.FindIndex<String>(colmnnames1, column => column.Equals("Patient Name"));
                Logger.Instance.InfoLog("index_after2--" + index_after2);
                Logger.Instance.InfoLog("index_after--" + index_after);
                if (index_after2 < index_after)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-7 - Rearrange other column
                studies.ReorderStudyListColumns("Issuer of PID", "start");
                String[] colmnnames2 = BasePage.GetColumnNames();
                executedSteps++;

                //Step-8 - Logout and Login and check if study list is saved
                login.Logout();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                String[] columnnames3 = BasePage.GetColumnNames();
                if (columnnames3.Length == colmnnames2.Length)
                {
                    if (columnnames3.SequenceEqual(colmnnames2))
                    {
                        result.steps[++executedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                    }
                    else
                    {
                        result.steps[++executedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                        result.steps[executedSteps].SetLogs();
                    }
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Create Test Domain, Users and Role
                domain = (DomainManagement)login.Navigate("DomainManagement");
                var domainattr1 = domain.CreateDomainAttr();
                domain.CreateDomain(domainattr1);
                domain.SearchDomain(domainattr1[DomainManagement.DomainAttr.DomainName]);
                domain.SelectDomain(domainattr1[DomainManagement.DomainAttr.DomainName]);
                domain.ClickEditDomain();
                domain.ClickChooseColumns("Domain Management");
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div#colchooser_gridTableStudyList")));
                domain.ClickElement(domain.AddAllLink());
                domain.ClickElement(domain.OKButton_ChooseColumns());
                domain.ClickSaveDomain();
                rolemgmt = (RoleManagement)login.Navigate("RoleManagement");
                rolemgmt.CreateRole(domainattr1[DomainManagement.DomainAttr.DomainName], TestRoleB, "Physician");
                rolemgmt.CreateRole(domainattr1[DomainManagement.DomainAttr.DomainName], TestRoleA, "Archivist");
                usermgmt = (UserManagement)login.Navigate("UserManagement");
                usermgmt.CreateUser(userB, domainattr1[DomainManagement.DomainAttr.DomainName], TestRoleB, 1, Config.emailid, 1, userB);
                usermgmt.CreateUser(userBB, domainattr1[DomainManagement.DomainAttr.DomainName], TestRoleB, 1, Config.emailid, 1, userBB);
                usermgmt.CreateUser(userA, domainattr1[DomainManagement.DomainAttr.DomainName], TestRoleA, 1, Config.emailid, 1, userA);
                usermgmt.CreateUser(userAA, domainattr1[DomainManagement.DomainAttr.DomainName], TestRoleA, 1, Config.emailid, 1, userAA);
                String strdomainname = domainattr1[DomainManagement.DomainAttr.DomainName];

                //Step-9 - Check Default Study List Layout in Domain Management
                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain(strdomainname);
                domain.SelectDomain(strdomainname);
                domain.ClickEditDomain();
                domain.BrowserScroll(1500, 1500);
                String[] d1columnnames = domain.StudyListColumnLayout().Select<IWebElement, String>((element) =>
                {
                    if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
                    {
                        if (!element.GetAttribute("style").Contains("DISPLAY: none"))
                        {
                            return element.GetAttribute("title");
                        }
                        return null;
                    }
                    else
                    {
                        if (!element.GetAttribute("style").Contains("display: none"))
                        {
                            return element.GetAttribute("title");
                        }
                        return null;
                    }

                }).ToArray().Where((c2) => { if (!String.IsNullOrEmpty(c2)) { return true; } else { return false; } }).ToArray();

                if (d1columnnames.SequenceEqual(studycolumns_selected))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-10 - Click choose column and validate                
                domain.ClickChooseColumns("Study Layout");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[class^='ui-dialog'][role='dialog']")));
                Boolean flag1 = domain.SelectedElements().Select<IWebElement, String>((element) =>
                {
                    if (!String.IsNullOrEmpty(element.GetAttribute("title")))
                    {
                        return element.GetAttribute("title");
                    }
                    else
                    {
                        return null;
                    }
                }).ToArray().Where((c3) => { if (!String.IsNullOrEmpty(c3)) { return true; } else { return false; } }).ToArray().SequenceEqual(studycolumns_selected);
                Boolean flag2 = domain.AvailableElements().Select<IWebElement, String>((element) =>
                {
                    if (!String.IsNullOrEmpty(element.GetAttribute("title")))
                    {
                        return element.GetAttribute("title");
                    }
                    else
                    {
                        return null;
                    }
                }).ToArray().Length == 0;

                if (flag1 && flag2 && domain.OKButton_ChooseColumns().Displayed && domain.CancelButton_ChooseColumns().Displayed)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-11
                domain.RemoveAllLink().Click();
                BasePage.wait.Until(driver =>
                {
                    int count = driver.FindElements(By.CssSelector("#colchooser_gridTableStudyList > div > div > div.available > ul > li> a > span[class*='ui-icon-plus']")).Count;
                    if (count == 20)
                        return true;
                    else
                        return false;
                });
                if (domain.SelectedElements().Count == 0)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step-12
                domain.AddAllLink().Click();
                if (domain.AvailableElements().Count == 0)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step-13
                domain.RemoveAllLink().Click();
                if (domain.SelectedElements().Count == 0)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step-14
                domain.TextBox_ChooseColumns().SendKeys("Name");
                BasePage.wait.Until(driver =>
                {
                    int count = 0;
                    var items = driver.FindElements(By.CssSelector("#colchooser_gridTableStudyList > div > div > div.available > ul > li"));

                    foreach (IWebElement item in items)
                    {
                        if (!item.GetAttribute("style").Contains("display: none"))
                        {
                            try
                            {
                                if (item.FindElement(By.CssSelector("a>span[class*='ui-icon-plus']")) != null)
                                    count++;
                            }
                            catch (Exception) { }
                        }

                    }

                    if (count == 4)
                        return true;
                    else
                        return false;
                });

                if (domain.AvailableElements().All((column) =>

                {
                    if (!column.GetAttribute("style").ToLower().Contains("display: none"))
                    {
                        if (column.GetAttribute("title").Equals("Patient Name") || column.GetAttribute("title").Equals("Last Name") || column.GetAttribute("title").Equals("First Name") || column.GetAttribute("title").Equals("Middle Name"))
                        { return true; }
                        else { return false; }
                    }
                    return true;
                }))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step-15
                domain.SelectColumns(new string[] { "Last Name", "First Name" });
                Boolean islastname = domain.SelectedElements().Any<IWebElement>((element) =>
                {
                    if (element.GetAttribute("title").Equals("Last Name")) { return true; }
                    else { return false; }
                });

                Boolean isfirstname = domain.SelectedElements().Any<IWebElement>((element) =>
                {
                    if (element.GetAttribute("title").Equals("Last Name")) { return true; }
                    else { return false; }
                });

                if (islastname && isfirstname)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-16
                domain.TextBox_ChooseColumns().Clear();
                domain.TextBox_ChooseColumns().SendKeys("");
                domain.TextBox_ChooseColumns().SendKeys(Keys.Backspace);
                Thread.Sleep(1000);
                if (domain.AvailableElements().Count == 18)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-17
                string[] array1 = new string[] { "Patient ID", "Study ID", "Accession", "Modality", "Data Source", "Institutions" };
                domain.SelectColumns(array1);
                var iterator1 = domain.SelectedElements().Select((c17) =>
                {
                    if (!c17.GetAttribute("style").ToLower().Contains("display: none"))
                    {
                        return c17.GetAttribute("title");
                    }
                    else
                    {
                        return null;
                    }
                }).ToArray().Where<String>(c17 => c17 != null).ToList().GetEnumerator();

                bool iscolumnsfound = false;
                foreach (String column in array1)
                {

                    iscolumnsfound = false;
                    while (iterator1.MoveNext())
                    {
                        if (column.Equals(iterator1.Current))
                        {
                            iscolumnsfound = true;
                            break;
                        }
                    }
                }

                if (iscolumnsfound)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }


                //step-18                
                domain.SelectColumns(new string[] { "Institutions" }, "Remove", false);
                foreach (IWebElement column in domain.AvailableElements())
                {
                    Logger.Instance.InfoLog("Values in step18 AvailableElements()--" + column.GetAttribute("title"));
                }
                if (domain.AvailableElements().Any<IWebElement>((element) => element.GetAttribute("title").Equals("Institutions")))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-19
                Thread.Sleep(3000);
                var selectedcolumns19 = domain.SelectedElements().Select((element) =>
                {
                    return (String.IsNullOrEmpty(element.GetAttribute("title")) ? null : element.GetAttribute("title"));
                }).ToArray();

                domain.OKButton_ChooseColumns().Click();
                String[] columns19 = domain.StudyListColumnLayout().Select<IWebElement, String>(studylistcolumnnames).ToArray().Where<String>(c19 =>
                {
                    if (String.IsNullOrEmpty(c19) && String.IsNullOrWhiteSpace(c19))
                    { return false; }
                    else { return true; }
                }).ToArray();
                foreach (String column in columns19)
                {
                    Logger.Instance.InfoLog("Column values in variable--columns19--" + column);
                }
                foreach (String column in selectedcolumns19)
                {
                    Logger.Instance.InfoLog("Column values in variable--selectedcolumns19--" + column);
                }
                if (selectedcolumns19.SequenceEqual(columns19))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-20
                domain.ClickChooseColumns("Study Layout");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[class^='ui-dialog'][role='dialog']")));
                domain.SelectColumns(new string[] { "# Images" });
                domain.CancelButton_ChooseColumns().Click();
                String[] columns20 = domain.StudyListColumnLayout().Select<IWebElement, String>(studylistcolumnnames).ToArray().Where<String>(c19 =>
                {
                    if (String.IsNullOrEmpty(c19) && String.IsNullOrWhiteSpace(c19))
                    { return false; }
                    else { return true; }
                }).ToArray();

                if (columns19.SequenceEqual(columns20))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-21
                var groupby = new SelectElement(domain.GroupByStudyListLayout());
                domain.ClickElement(domain.GroupByStudyListLayout());
                groupby.SelectByText("Patient ID");
                executedSteps++;

                //Step-22
                domain.ReorderStudyListColumns("Data Source", "middle");
                String[] columns22 = domain.StudyListColumnLayout().Select<IWebElement, String>(studylistcolumnnames).ToArray().Where<String>(c19 =>
                {
                    if (String.IsNullOrEmpty(c19) && String.IsNullOrWhiteSpace(c19))
                    { return false; }
                    else { return true; }
                }).ToArray();
                executedSteps++;

                //Step-23
                domain.ClickSaveEditDomain();
                executedSteps++;

                //Step-24
                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain(strdomainname);
                domain.SelectDomain(strdomainname);
                domain.ClickEditDomain();
                domain.BrowserScroll(1500, 1500);
                domain.ClickEditDomain();
                String[] columns24 = domain.StudyListColumnLayout().Select<IWebElement, String>(studylistcolumnnames).ToArray().Where<String>(c19 =>
                {
                    if (String.IsNullOrEmpty(c19) && String.IsNullOrWhiteSpace(c19))
                    { return false; }
                    else { return true; }
                }).ToArray();

                if (columns22.SequenceEqual(columns24) && columns24.Length > 0)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step-25
                domain.ReorderStudyListColumns("Accession", "start");
                domain.ClickCloseEditDomain();
                executedSteps++;

                //step-26
                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain(strdomainname);
                domain.SelectDomain(strdomainname);
                domain.ClickEditDomain();
                domain.BrowserScroll(1500, 1500);
                domain.ClickEditDomain();
                String[] columns26 = domain.StudyListColumnLayout().Select<IWebElement, String>(studylistcolumnnames).ToArray().Where<String>(c19 =>
                {
                    if (String.IsNullOrEmpty(c19) && String.IsNullOrWhiteSpace(c19))
                    { return false; }
                    else { return true; }
                }).ToArray();
                if (columns24.SequenceEqual(columns26) && columns26.Length > 0)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-27
                domain.ModifyStudySearchFields("hide", new string[] { "Data Source", "Accession Number", "Study Performed Date", "Study Description", "Issuer of PID", "Date of Birth" });
                string[] studysearchcolumns27 = domain.ReorderStudySearchFields(0, 2, "Down");
                domain.ClickSaveEditDomain();
                executedSteps++;

                //Step-28
                login.Logout();
                login.LoginIConnect(domainattr1[DomainManagement.DomainAttr.UserID], domainattr1[DomainManagement.DomainAttr.Password]);
                studies = (Studies)login.Navigate("Studies");
                String[] columns28 = domain.StudyListColumnLayout().Select<IWebElement, String>(studylistcolumnnames).ToArray().Where<String>(c19 =>
                {
                    if (String.IsNullOrEmpty(c19) && String.IsNullOrWhiteSpace(c19))
                    {
                        return false;
                    }
                    else { return true; }
                }).ToArray();
                bool flag28_2 = columns28.SequenceEqual(columns22);

                IList<IWebElement> searchfields28 = BasePage.Driver.FindElements(tablesearchfields);
                String[] searchcolumnnames28 = searchfields28.Select<IWebElement, String>((element) =>
                {
                    if (!element.GetAttribute("innerHTML").Equals("My Patients Only"))
                    {
                        return element.GetAttribute("innerHTML");
                    }
                    else
                    {
                        return null;
                    }
                }).ToArray().Where((column28) =>
                {
                    if (!String.IsNullOrEmpty(column28))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }).ToArray();

                #region Logic to check column order are same
                if (searchcolumnnames28.Length == studysearchcolumns27.Length)
                {

                    String[] regexpressions28 = searchcolumnnames28.Select<String, String>((fieldname) =>
                    {
                        if (!String.IsNullOrEmpty(fieldname))
                        {
                            String[] arr = fieldname.Split(' ');
                            int counter = 0;
                            String regex = "";
                            foreach (string subsfieldname in arr)
                            {
                                if (counter == 0)
                                {
                                    regex = regex + subsfieldname.Substring(0, 1) + ".*";
                                }
                                else
                                {
                                    regex = regex + @"[\s]" + subsfieldname.Substring(0, 1) + ".*";
                                }
                                counter++;
                            }
                            return regex;
                        }
                        return null;

                    }).ToArray();

                    IEnumerator<String> iterator28 = studysearchcolumns27.ToList().GetEnumerator();
                    Boolean isfieldsinorder28 = true;
                    iterator28.MoveNext();
                    foreach (String regex in regexpressions28)
                    {
                        if (!String.IsNullOrEmpty(regex))
                        {
                            if (!Regex.IsMatch(iterator28.Current, regex))
                            {
                                isfieldsinorder28 = false;
                            }
                        }
                        iterator28.MoveNext();
                    }
                    #endregion Logic to check column order are same               

                    if (isfieldsinorder28 && flag28_2)
                    {
                        result.steps[++executedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                    }
                    else
                    {
                        result.steps[++executedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                        result.steps[executedSteps].SetLogs();
                    }
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step-29
                role = login.Navigate<RoleManagement>();
                role.SearchRole(TestRoleA);
                role.SelectRole(TestRoleA);
                role.ClickEditRole();

                String[] searchfieldsinrole29 = role.VisibleSearchField().Options.Select<IWebElement, String>((element29) =>
                { return element29.GetAttribute("innerHTML"); }).ToArray().Where((column29) =>
                { if (!String.IsNullOrEmpty(column29)) { return true; } else { return false; } }).ToArray();

                role.ClickCloseButton();
                login.Logout();
                login.LoginIConnect(userA, userA);
                PageLoadWait.WaitForFrameLoad(10);
                IList<IWebElement> studypage_searchelement29 = BasePage.Driver.FindElements(tablesearchfields);
                String[] searchfieldsactual29 = studypage_searchelement29.Select<IWebElement, String>((element) =>
                {

                    if (!element.GetAttribute("innerHTML").Equals("My Patients Only"))
                    {
                        return element.GetAttribute("innerHTML");
                    }
                    else
                    {
                        return null;
                    }

                }).ToArray();
                searchfieldsactual29 = searchfieldsactual29.Where((field) =>
                {
                    if (!String.IsNullOrEmpty(field))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }).ToArray();

                #region Logic to check column order are same
                if (searchfieldsinrole29.Length == searchfieldsactual29.Length)
                {

                    String[] regexpressions29 = searchfieldsactual29.Select<String, String>((fieldname) =>
                    {
                        if (!String.IsNullOrEmpty(fieldname))
                        {
                            String[] arr = fieldname.Split(' ');
                            int counter = 0;
                            String regex = "";
                            foreach (string subsfieldname in arr)
                            {
                                if (counter == 0)
                                {
                                    regex = regex + subsfieldname.Substring(0, 1) + ".*";
                                }
                                else
                                {
                                    regex = regex + @"[\s]" + subsfieldname.Substring(0, 1) + ".*";
                                }
                                counter++;
                            }
                            return regex;
                        }
                        return null;

                    }).ToArray();

                    IEnumerator<String> iterator29 = searchfieldsinrole29.ToList().GetEnumerator();
                    Boolean isfieldsinorder29 = true;
                    iterator29.MoveNext();
                    foreach (String regex in regexpressions29)
                    {
                        if (!String.IsNullOrEmpty(regex))
                        {
                            if (!Regex.IsMatch(iterator29.Current, regex))
                            {
                                isfieldsinorder29 = false;
                            }
                        }
                        iterator29.MoveNext();
                    }
                    #endregion Logic to check column order are same

                    if (isfieldsinorder29)
                    {
                        result.steps[++executedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                    }
                    else
                    {
                        result.steps[++executedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                        result.steps[executedSteps].SetLogs();
                    }
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step-30
                login.Logout();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                role = login.Navigate<RoleManagement>();
                role.SelectDomainfromDropDown(domainattr1[DomainManagement.DomainAttr.DomainName]);
                role.SearchRole(TestRoleA);
                role.SelectRole(TestRoleA);
                role.ClickEditRole();
                var checkbox = BasePage.Driver.FindElement(By.CssSelector("input#ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_studyGrid_2_StudyGridConfigUseDomainLayoutCheckbox"));
                if (checkbox.Selected)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }


                //Step-31
                role.SetCheckbox(checkbox);
                executedSteps++;

                //Step-32
                login.Logout();
                login.LoginIConnect(userA, userA);
                login.Navigate<Studies>();
                PageLoadWait.WaitForFrameLoad(10);
                String[] searchcolumnnames32 = domain.StudyListColumnLayout().Select<IWebElement, String>(studylistcolumnnames).ToArray().Where<String>(c19 =>
                {
                    if (String.IsNullOrEmpty(c19) && String.IsNullOrWhiteSpace(c19))
                    { return false; }
                    else { return true; }
                }).ToArray();
                foreach (String column in searchcolumnnames32)
                {
                    Logger.Instance.InfoLog("The values of searchcolumnnames32--" + column);
                }
                foreach (String column in columns22)
                {
                    Logger.Instance.InfoLog("The values of columns22--" + column);
                }
                studies.SearchStudy("LastName", "*");
                PageLoadWait.WaitForLoadingMessage(60);
                var isGrouped = studies.IsGroupedBy("Patient ID");
                if (searchcolumnnames32.SequenceEqual(columns22) && isGrouped)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }


                //Step-33
                studies.ClickChooseColumns();
                Boolean flag33 = domain.SelectedElements().Select<IWebElement, String>((element) =>
                {
                    if (!String.IsNullOrEmpty(element.GetAttribute("title")))
                    {
                        return element.GetAttribute("title");
                    }
                    else
                    {
                        return null;
                    }
                }).ToArray().Where((c3) => { if (!String.IsNullOrEmpty(c3)) { return true; } else { return false; } }).ToArray().SequenceEqual(searchcolumnnames32);

                if (flag33)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }
                studies.ClickElement(studies.CancelButton_ChooseColumns());

                //Step-34
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForLoadingMessage(25);
                studies.SearchStudy("Accession", accession);
                studies.SelectStudy1("Accession", accession, true);
                var brviewer = BluRingViewer.LaunchBluRingViewer();
                brviewer.CloseBluRingViewer();
                executedSteps++;

                //step-35
                login.Logout();
                login.LoginIConnect(domainattr1[DomainManagement.DomainAttr.UserID], domainattr1[DomainManagement.DomainAttr.Password]);
                domain = login.Navigate<DomainManagement>();
                executedSteps++;

                //Step-36
                domain.ClickChooseColumns("Study Layout");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[class^='ui-dialog'][role='dialog']")));
                domain.SelectColumns(new string[] { "# Images" });
                domain.OKButton_ChooseColumns().Click();
                var groupby36 = new SelectElement(domain.GroupByStudyListLayout());
                domain.ClickElement(domain.GroupByStudyListLayout());
                groupby36.SelectByText("Data Source");
                String[] columns36 = domain.StudyListColumnLayout().Select<IWebElement, String>(studylistcolumnnames).ToArray().Where<String>(c36 =>
                {
                    if (String.IsNullOrEmpty(c36) && String.IsNullOrWhiteSpace(c36))
                    { return false; }
                    else { return true; }
                }).ToArray();
                domain.ClickSaveEditDomain();
                executedSteps++;

                //Step-37
                String[] datasources = domain.DatasourceConnectedDropDown().Options.Select<IWebElement, String>((datasource) =>
                {
                    return (datasource.GetAttribute("innerHTML"));
                }).ToArray();
                studies = login.Navigate<Studies>();
                studies.SearchStudy("Last Name", "*");
                PageLoadWait.WaitForLoadingMessage(120);
                //String groupbytext = BasePage.Driver.FindElement(By.CssSelector("table[id='gridTableStudyList'] tr:nth-of-type(2)>td")).GetAttribute("innerHTML");
                String groupbytext = BasePage.Driver.FindElements(By.CssSelector("table[id='gridTableStudyList'] tr"))[1].FindElement(By.CssSelector("td")).GetAttribute("innerHTML");

                bool flag37_1 = datasources.Any((datasource) => groupbytext.Contains(datasource));

                String[] studycolumns37 = BasePage.GetColumnNames();
                bool flag37_2 = studycolumns37.SequenceEqual(columns36);
                if (flag37_1 && flag37_2)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-38
                login.Logout();
                login.LoginIConnect(userA, userA);
                PageLoadWait.WaitForFrameLoad(10);
                studies.SearchStudy("Last Name", "*");
                PageLoadWait.WaitForLoadingMessage(120);

                String[] studycolumns38 = BasePage.GetColumnNames();
                bool flag38_2 = studycolumns38.SequenceEqual(columns36);
                String groupbytext38 = BasePage.Driver.FindElements(By.CssSelector("table[id='gridTableStudyList'] tr"))[1].FindElement(By.CssSelector("td")).GetAttribute("innerHTML");
                bool flag38_1 = datasources.Any((datasource) => groupbytext38.Contains(datasource));

                if (flag38_1 && flag38_2)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-39
                login.Logout();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                executedSteps++;

                //step-40
                role = login.Navigate<RoleManagement>();
                role.SearchRole(TestRoleA, domainattr1[DomainManagement.DomainAttr.DomainName]);
                role.SelectRole(TestRoleA);
                role.ClickEditRole();
                BasePage.Driver.FindElements(By.CssSelector("input[id$='StudyGridConfigUseDomainLayoutCheckbox']"))[1].Click();
                role.ClickElement(role.GroupByStudyListLayoutInRole());
                new SelectElement(role.GroupByStudyListLayoutInRole()).SelectByText("Modality");
                String[] searchcolumnnames40 = domain.StudyListColumnLayout().Select<IWebElement, String>(studylistcolumnnames).ToArray().Where<String>(c19 =>
                {
                    if (String.IsNullOrEmpty(c19) && String.IsNullOrWhiteSpace(c19))
                    { return false; }
                    else { return true; }
                }).ToArray();
                role.ClickSaveEditRole();
                executedSteps++;

                //Step-41
                login.Logout();
                login.LoginIConnect(userA, userA);
                PageLoadWait.WaitForFrameLoad(10);
                studies.SearchStudy("Patient ID", patientid);
                String[] modalitylist = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Modality", BasePage.GetColumnNames());
                String[] studycolumns41 = BasePage.GetColumnNames();
                bool flag41_1 = studycolumns41.SequenceEqual(searchcolumnnames40);
                Logger.Instance.InfoLog("flag41_1" + flag41_1);
                String groupbytext41 = BasePage.Driver.FindElements(By.CssSelector("table[id='gridTableStudyList'] tr"))[1].FindElement(By.CssSelector("td")).GetAttribute("innerHTML");
                bool flag41_2 = modalitylist.Any((modality) => groupbytext41.Contains(modality));
                Logger.Instance.InfoLog("flag41_2" + flag41_2);
                if (flag41_1 && flag41_2)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-42
                login.Logout();
                login.LoginIConnect(userB, userB);
                PageLoadWait.WaitForFrameLoad(10);
                String[] studycolumns42 = BasePage.GetColumnNames();
                if (studycolumns42.SequenceEqual(columns36))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Report Result
                login.Logout();
                result.FinalResult(executedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Resultstudycolumns
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Layout Configuration - User Level
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_162431(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result = new TestCaseResult(stepcount);
            String defaultStudyLayout = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DefaultStudyLayoutSelected");
            String patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
            int executedSteps = -1;
            Studies studies;
            string[] arrDefaultStudyListcolumns = defaultStudyLayout.Split(':');
            By tablesearchfields = By.CssSelector("table[id = 'customSearchTable'] td[class='searchCriteriaMiddle'] span[id^='m_studySearch']");
            RoleManagement rolemgmt;
            UserManagement usermgmt;
            DomainManagement domain;
            Random randomnumber = new Random();
            String defaultAvailableStudyLayout = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DefaultStudyLayoutAvailable");
            String[] arrAvailableStudyLayout = defaultAvailableStudyLayout.Split(':');

            //Test Role - A and its Users
            String testroleA = "Archivist1" + new System.DateTime().Second + randomnumber.Next(1, 1000);
            String D1a = "ar1" + new System.DateTime().Second + randomnumber.Next(1, 1000);
            String D1aa = "ar1" + new System.DateTime().Second + randomnumber.Next(1, 1000);

            //Test Role -B and its Users
            String testroleB = "Physician1" + new System.DateTime().Second + randomnumber.Next(1, 1000);
            String D1b = "ph1" + new System.DateTime().Second + randomnumber.Next(1, 1000);
            String D1bb = "ph1" + new System.DateTime().Second + randomnumber.Next(1, 1000);


            try
            {
                //Set Test step description
                result.SetTestStepDescription(teststeps);

                //Create Test Domain, Users and Role -- Preconditions
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domain = login.Navigate<DomainManagement>();
                var domainattr1 = domain.CreateDomainAttr();
                domain.CreateDomain(domainattr1);
                rolemgmt = (RoleManagement)login.Navigate("RoleManagement");
                rolemgmt.CreateRole(domainattr1[DomainManagement.DomainAttr.DomainName], testroleA, "");
                rolemgmt.CreateRole(domainattr1[DomainManagement.DomainAttr.DomainName], testroleB, "");
                usermgmt = (UserManagement)login.Navigate("UserManagement");
                usermgmt.CreateUser(D1b, domainattr1[DomainManagement.DomainAttr.DomainName], testroleB, 1, Config.emailid, 1, D1b);
                Thread.Sleep(5000);
                usermgmt.CreateUser(D1bb, domainattr1[DomainManagement.DomainAttr.DomainName], testroleB, 1, Config.emailid, 1, D1bb);
                Thread.Sleep(5000);
                login.Logout();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                usermgmt = (UserManagement)login.Navigate("UserManagement");
                Thread.Sleep(5000);
                usermgmt.CreateUser(D1a, domainattr1[DomainManagement.DomainAttr.DomainName], testroleA, 1, Config.emailid, 1, D1a);
                Thread.Sleep(5000);
                usermgmt.CreateUser(D1aa, domainattr1[DomainManagement.DomainAttr.DomainName], testroleA, 1, Config.emailid, 1, D1aa);
                Thread.Sleep(5000);
                String testdomainname = domainattr1[DomainManagement.DomainAttr.DomainName];
                login.Logout();

                //Get Datasource connected to Test domain and Study List settings
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domain = login.Navigate<DomainManagement>();
                domain.SearchDomain(testdomainname);
                domain.SelectDomain(testdomainname);
                domain.ClickEditDomain();
                String[] datasources = domain.DatasourceConnectedDropDown().Options.Select<IWebElement, String>((datasource) =>
                {
                    return (datasource.GetAttribute("innerHTML"));
                }).ToArray();
                String[] studylayoutdomain = domain.GetCurrentStudyListLayout();
                domain.ClickSaveEditDomain();


                //Enable Role Settings for Test Role A and B
                rolemgmt = login.Navigate<RoleManagement>();
                rolemgmt.SelectDomainfromDropDown(testdomainname);
                rolemgmt.SearchRole(testroleA);
                rolemgmt.SelectRole(testroleA);
                rolemgmt.ClickEditRole();
                rolemgmt.UnCheckCheckbox(rolemgmt.UseDomainSettings_StudyListLayout());
                String[] studyloayoutrole = rolemgmt.GetCurrentStudyListLayout();
                rolemgmt.SetCheckbox(rolemgmt.AllowUserToSaveStudyLayout());
                rolemgmt.ClickSaveEditRole();

                rolemgmt = login.Navigate<RoleManagement>();
                rolemgmt.SelectDomainfromDropDown(testdomainname);
                rolemgmt.SearchRole(testroleB);
                rolemgmt.SelectRole(testroleB);
                rolemgmt.ClickEditRole();
                rolemgmt.UnCheckCheckbox(rolemgmt.UseDomainSettings_StudyListLayout());
                rolemgmt.SetCheckbox(rolemgmt.AllowUserToSaveStudyLayout());
                rolemgmt.ClickSaveEditRole();


                //Step-1
                login.LoginIConnect(D1b, D1b);
                PageLoadWait.WaitForFrameLoad(10);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Patient ID", patientid);
                var study = studies.GetMatchingRow("Patient ID", patientid);
                String[] studylayout1 = BasePage.GetColumnNames();
                if (study.Count > 0)
                {
                    result.steps[++executedSteps].StepPass();
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-2
                var groupby = studies.GroupByStudyListLayoutInTab();
                new Actions(BasePage.Driver).Click(groupby);
                new SelectElement(groupby).SelectByText("Data Source");
                String groupbytext;
                if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
                {
                    groupbytext = BasePage.Driver.FindElements(By.CssSelector("table[id='gridTableStudyList'] tr"))[1].FindElement(By.CssSelector("td")).GetAttribute("innerHTML");
                }
                else
                {
                    groupbytext = BasePage.Driver.FindElement(By.CssSelector("table[id='gridTableStudyList'] tr:nth-of-type(2)>td")).GetAttribute("innerHTML");
                }

                bool flag2_1 = datasources.Any((datasource) => groupbytext.Contains(datasource));
                String[] studycolumns2 = BasePage.GetColumnNames();
                bool flag2_3 = studies.Reset().Displayed;
                int count1 = studylayout1.Count();
                if (flag2_1 && (studycolumns2.Count() == count1 + 1) && flag2_3)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-3
                studies.ClickChooseColumns();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[class^='ui-dialog'][role='dialog']")));
                Boolean flag3 = studies.SelectedElements().Select<IWebElement, String>((element) =>
                {
                    if (!String.IsNullOrEmpty(element.GetAttribute("title")))
                    {
                        return element.GetAttribute("title");
                    }
                    else
                    {
                        return null;
                    }
                }).ToArray().Where((c3) => { if (!String.IsNullOrEmpty(c3)) { return true; } else { return false; } }).ToArray().SequenceEqual(studycolumns2);
                if (flag3)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-4
                studies.ClickElement(studies.RemoveAllLink());
                String[] studylayout4 = new string[] { "First Name", "Last Name", "Modality", "Study ID", "Gender", "# Images", "Accession" };
                studies.SelectColumns(studylayout4);
                Boolean flag4 = studies.SelectedElements().Select<IWebElement, String>((element) =>
                {
                    if (!String.IsNullOrEmpty(element.GetAttribute("title")))
                    {
                        return element.GetAttribute("title");
                    }
                    else
                    {
                        return null;
                    }
                }).ToArray().Where((c3) => { if (!String.IsNullOrEmpty(c3)) { return true; } else { return false; } }).ToArray().SequenceEqual(studylayout4);
                studies.ClickElement(studies.OKButton_ChooseColumns());
                if (flag4)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-5
                int columncount = BasePage.GetColumnElements().Count();
                int[] width = new int[columncount];
                int[] height = new int[columncount];
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("chrome"))
                {
                    if(studies.ResizeInstitutionCoulmns())
                    {
                        result.steps[++executedSteps].StepPass(); ;
                    }
                    else
                    {
                        result.steps[++executedSteps].StepFail();
                    }
                }
                else
                {
                    for (int iterate = 0; iterate < columncount; iterate++)
                    {
                        IWebElement element = BasePage.GetColumnElements()[iterate];
                        width[iterate] = element.Size.Width;
                        height[iterate] = element.Size.Height;
                        if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("firefox"))
                        {
                            if (iterate < columncount - 1)
                                studies.ActionsDragAndDrop(element.FindElement(By.CssSelector("span")), BasePage.GetColumnElements()[iterate + 1]);
                        }
                        else
                        {
                            studies.ActionsDragAndDrop(element.FindElement(By.CssSelector("span")), width[iterate] / 2, height[iterate]);
                        }
                    }
                    int[] widthafter = BasePage.GetColumnElements().Select<IWebElement, int>((element) => { return element.Size.Width; }).ToArray();
                    bool flag5 = false;
                    for (int i = 0; i < columncount - 1; i++)
                    {
                        if (widthafter[i] > width[i])
                        { flag5 = true; break; }
                    }

                    if (flag5)
                    {
                        result.steps[++executedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                    }
                    else
                    {
                        result.steps[++executedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                        result.steps[executedSteps].SetLogs();
                    }
                }

                //Step-6-Grouping by Patient DOB
                var groupby6 = studies.GroupByStudyListLayoutInTab();
                new Actions(BasePage.Driver).Click(groupby6);
                new SelectElement(groupby6).SelectByText("Patient DOB");
                String[] patientdob = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Patient DOB", BasePage.GetColumnNames());
                //String groupbytext6 = BasePage.Driver.FindElement(By.CssSelector("table[id='gridTableStudyList'] tr:nth-of-type(2)>td")).GetAttribute("innerHTML");
                String groupbytext6 = BasePage.Driver.FindElements(By.CssSelector("table[id='gridTableStudyList'] tr"))[1].FindElement(By.CssSelector("td")).GetAttribute("innerHTML");
                String[] studylayout6 = BasePage.GetColumnNames();
                bool flag6_1 = patientdob.Any((dob) => groupbytext6.Contains(dob));
                bool flag6_2 = studylayout6.Any<String>(columnname => columnname.Trim().Equals("Patient DOB"));
                if (flag6_1 && flag6_2)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-7
                studies.SearchStudy("Accession", accession);
                String[] studylayout7 = BasePage.GetColumnNames();
                bool flag7_1 = studylayout7.SequenceEqual(studylayout6);
                bool flag7_2 = studies.GetMatchingRow("Accession", accession) != null;
                if (flag7_1 && flag7_2)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-8
                studies.SelectStudy1("Accession", accession, true);
                var brviewer = BluRingViewer.LaunchBluRingViewer();
                brviewer.CloseBluRingViewer();
                executedSteps++;

                //Step-9
                login.Navigate("Studies");
                String[] studylayout9 = BasePage.GetColumnNames();
                if(studylayout7.SequenceEqual(studylayout9))
                {
                    result.steps[++executedSteps].StepPass();
                }
                else
                {
                    result.steps[++executedSteps].StepFail();
                }

                //Step-10
                login.Logout();
                login.LoginIConnect(D1bb, D1bb);
                PageLoadWait.WaitForFrameLoad(10);
                String[] studylayout10 = studies.GetCurrentStudyListLayout();
                if (studylayout10.SequenceEqual(studylayoutdomain))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-11
                login.Logout();
                login.LoginIConnect(D1b, D1b);
                PageLoadWait.WaitForFrameLoad(10);
                if (studylayout7.SequenceEqual(BasePage.GetColumnNames()))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-12
                studies.ClickElement(studies.Reset());
                if (BasePage.GetColumnNames().SequenceEqual(studylayoutdomain))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-13
                login.Logout();
                login.LoginIConnect(D1a, D1a);
                PageLoadWait.WaitForFrameLoad(10);
                String[] studylayout13 = BasePage.GetColumnNames();
                if (studylayout13.SequenceEqual(studyloayoutrole))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-14
                int columncount14 = BasePage.GetColumnElements().Count();
                int[] width14 = new int[columncount14];
                int[] height14 = new int[columncount14];
                for (int iterate = 0; iterate < columncount14; iterate++)
                {
                    IWebElement element = BasePage.GetColumnElements()[iterate];
                    width14[iterate] = element.Size.Width;
                    width14[iterate] = element.Size.Height;
                    studies.ActionsDragAndDrop(element.FindElement(By.CssSelector("span")), width14[iterate] / 2, height14[iterate]);
                }
                int[] widthafter14 = BasePage.GetColumnElements().Select<IWebElement, int>((element) => { return element.Size.Width; }).ToArray();
                bool flag14 = false;
                for (int i = 0; i < columncount14 - 1; i++)
                {
                    if (widthafter14[i] > width14[i])
                    { flag14 = true; break; }
                }
                if (flag14)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-15
                var studylayout15 = BasePage.GetColumnNames();
                studies.SearchStudy("Accession", accession);
                var study15 = studies.GetMatchingRow("Accession", accession);
                if (study15 != null)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-16
                studies.SelectStudy("Accession", accession);
                brviewer = BluRingViewer.LaunchBluRingViewer();
                brviewer.CloseBluRingViewer();
                executedSteps++;

                //Step-17                
                PageLoadWait.WaitForFrameLoad(10);
                var studylayout17 = BasePage.GetColumnNames();
                if (studylayout17.SequenceEqual(studylayout15))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }


                //Step-18
                login.Logout();
                login.LoginIConnect(D1a, D1a);
                PageLoadWait.WaitForFrameLoad(10);
                String[] studylayout18 = BasePage.GetColumnNames();
                if (studylayout18.SequenceEqual(studylayout15))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-19
                studies.ClickElement(studies.Reset());
                PageLoadWait.WaitForPageLoad(2);
                var studylayout19 = BasePage.GetColumnNames();
                if (studylayout19.SequenceEqual(studylayoutdomain))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Report Result                
                login.Logout();
                result.FinalResult(executedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Resultstudycolumns
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                BasePage.Driver.FindElement(By.CssSelector("td[title*='Reset']>div")).Click();
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Custom Search (Default Search)
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_162429(String testid, String teststeps, int stepcount)
        {

            //Declare Variables
            TestCaseResult result = new TestCaseResult(stepcount);
            int executedSteps = -1;

            DomainManagement domain = null;
            RoleManagement rolemgmt = null;
            UserManagement usermgmt = null;
            Studies studies = null;
            Random randomnumber = new Random();
            String D1Physician = "Physician1" + new System.DateTime().Second + randomnumber.Next(1, 1000);
            String D1ph = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RefPhysician") + " " + randomnumber.Next(99, 999);
            String lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
            String firstname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
            String patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String studyid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyID");
            String patientdob = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DOB");
            String ipid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "IPID");
            String studydescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDescription");
            String institutions = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Institutions");
            String modality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
            String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
            String gender = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Gender");
            String refphysician = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "RefPhysician")).Split(' ')[0];
            String others = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Others");

            try
            {

                //Set Test step description            
                result.SetTestStepDescription(teststeps);

                //Precondition - Make all search criteria fields visible
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domain = login.Navigate<DomainManagement>();
                domain.SearchDomain("SuperAdminGroup");
                domain.SelectDomain("SuperAdminGroup");
                domain.ClickEditDomain();
                domain.ModifyStudySearchFields("Show", new String[] {"Last Name", "First Name", "Patient ID", "Referring Physician",
                "Accession Number","Study Performed Date", "Modality", "Data Source",
                "Gender", "Study ID", "Issuer of PID", "Study Description", "Date of Birth", "Institution"});
                domain.ClickSaveEditDomain();
                studies = login.Navigate<Studies>();
                studies.SearchStudy(Datasource: "All");
                login.Logout();

                //Step-1 Service tool default setup
                executedSteps++;

                //Step-2 Login and configure data source
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                executedSteps++;

                /**Step-3 and 4 - Create  a Test Domain and create a Ref Physician
                Note - Step-4 not required as Test domain is not being used anywhere. Hence creating Ref Physician user in new domain.**/
                domain = (DomainManagement)login.Navigate("DomainManagement");
                var domainattr1 = domain.CreateDomainAttr();
                domain.CreateDomain(domainattr1);
                rolemgmt = (RoleManagement)login.Navigate("RoleManagement");
                rolemgmt.CreateRole(domainattr1[DomainManagement.DomainAttr.DomainName], D1Physician, "Physician");
                usermgmt = (UserManagement)login.Navigate("UserManagement");
                usermgmt.CreateUser(D1ph, domainattr1[DomainManagement.DomainAttr.DomainName], D1Physician, 1, Config.emailid, 1, D1ph);
                String strdomainname = domainattr1[DomainManagement.DomainAttr.DomainName];
                executedSteps++;
                executedSteps++;

                //Step-5 -- Need to check on default fields
                studies = login.Navigate<Studies>();
                PageLoadWait.WaitForFrameLoad(10);
                Func<Studies, Boolean> checksearchfields = ((study) =>
                {
                    if (study.FirstName().Displayed && study.LastName().Displayed && study.Gender().Displayed && study.PatientID().Displayed
                    && study.PatientDOB().Displayed && study.StudyID().Displayed && study.IPID().Displayed && study.StudyDescription().Displayed
                    && study.Accession().Displayed && study.Instituition().Displayed && study.StudyPerformed().Displayed && study.Modality().Displayed
                    && study.DataSource().Displayed && study.RefPhysician().Displayed && study.MyPatients().Displayed && study.Delete().Displayed
                    && study.Save().Displayed && study.ClearButton().Displayed && study.Search().Displayed && study.MySearch().Displayed && study.GroupByStudyListLayoutInTab().Displayed)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                });
                Boolean step5_1 = checksearchfields.Invoke(studies);
                if (step5_1)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-6 - Empty search
                studies.SearchStudy(LastName: "*", Datasource: login.GetHostName(Config.EA1));
                PageLoadWait.WaitForLoadingMessage(120);
                var studies6 = BasePage.GetSearchResults();
                if (studies6.Count == 200)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-7 - Select a Datasource and group by Modality
                //String datasource = BasePage.Driver.FindElements(By.CssSelector("div[id='sub_menu_multiselect']>div>a>span,div[id='sub_menu_multiselect']>div>a>div")).Select<IWebElement, String>(element => element.GetAttribute("innerHTML")).ToArray()[1];
                var datasource = studies.GetHostName(Config.EA1);
                studies.SelectGroupByInStudiesTab("Modality");
                studies.ChooseColumns(new string[] { "Data Source" });
                studies.SearchStudy(Datasource: datasource, LastName: "*");
                PageLoadWait.WaitForLoadingMessage(120);
                bool step7_1 = studies.IsGroupedBy("Modality");
                bool step7_2 = BasePage.GetColumnValues("Data Source").All<String>(columnvalue => columnvalue.Equals(datasource));

                if (step7_1 && step7_2 && (BasePage.GetSearchResults().Count > 0))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-8
                studies.SelectGroupByInStudiesTab();
                studies.ChooseColumns(new string[] { "Last Name" });
                studies.SearchStudy(LastName: lastname, Datasource: "All");
                bool step8_2 = BasePage.GetColumnValues("Last Name").All<String>(columnvalue => columnvalue.Equals(lastname));
                if (step8_2 && (BasePage.GetSearchResults().Count > 0))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-9
                studies.SelectGroupByInStudiesTab("Patient DOB");
                PageLoadWait.WaitForPageLoad(5);
                bool step9_1 = studies.IsGroupedBy("Patient DOB");
                bool step9_2 = BasePage.GetColumnValues("Last Name").All<String>(columnvalue => columnvalue.Equals(lastname));
                if (step9_2 && step9_1 && (BasePage.GetSearchResults().Count > 0))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-10
                studies.SelectGroupByInStudiesTab();
                studies.ChooseColumns(new string[] { "First Name" });
                studies.SearchStudy(FirstName: firstname, Datasource: "All");
                bool step10_1 = BasePage.GetColumnValues("First Name").All<String>(columnvalue => columnvalue.Equals(firstname));
                if (step10_1 && (BasePage.GetSearchResults().Count > 0))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-11
                studies.SelectGroupByInStudiesTab();
                studies.ChooseColumns(new string[] { "Patient ID" });
                studies.SearchStudy(patientID: patientid, Datasource: "All");
                bool step11_1 = BasePage.GetColumnValues("Patient ID").All<String>(columnvalue => columnvalue.Equals(patientid));
                if (step11_1 && (BasePage.GetSearchResults().Count > 0))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-12
                studies.SelectGroupByInStudiesTab();
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SearchStudy(studyID: studyid, Datasource: "All");
                bool step12_1 = BasePage.GetColumnValues("Study ID").All<String>(columnvalue => columnvalue.Equals(studyid));
                if (step12_1 && (BasePage.GetSearchResults().Count > 0))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-13
                studies.SelectGroupByInStudiesTab();
                studies.ChooseColumns(new string[] { "Patient DOB" });
                studies.SearchStudy(DOB: patientdob, Datasource: "All");
                bool step13_1 = BasePage.GetColumnValues("Patient DOB").All<String>(columnvalue => columnvalue.Equals(patientdob));

                if (step13_1 && (BasePage.GetSearchResults().Count > 0))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-14
                studies.SelectGroupByInStudiesTab();
                studies.ChooseColumns(new string[] { "Issuer of PID" });
                studies.SearchStudy(IPID: ipid, Datasource: "All");
                bool step14_1 = BasePage.GetColumnValues("Issuer of PID").All<String>(columnvalue => columnvalue.Equals(ipid));

                if (step14_1 && (BasePage.GetSearchResults().Count > 0))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Setp-15-Study Description
                studies.SelectGroupByInStudiesTab();
                studies.ChooseColumns(new string[] { "Description" });
                studies.ClearFields();
                studies.StudyDescription().SendKeys(studydescription);
                studies.ClickElement(studies.Search());
                PageLoadWait.WaitForLoadingMessage(60);
                bool step15_1 = BasePage.GetColumnValues("Description").All<String>(columnvalue => columnvalue.ToLower().Contains(studydescription.ToLower()));

                if (step15_1 && (BasePage.GetSearchResults().Count > 0))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }


                //Step-16 - Accession
                studies.SelectGroupByInStudiesTab();
                studies.ChooseColumns(new string[] { "Accession" });
                studies.SearchStudy(AccessionNo: accession, Datasource: "All");
                bool step16_1 = BasePage.GetColumnValues("Accession").All<String>(columnvalue => columnvalue.ToLower().Equals(accession.ToLower()));

                if (step16_1 && (BasePage.GetSearchResults().Count > 0))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }


                //Step-17- Institution
                studies.SelectGroupByInStudiesTab();
                studies.ChooseColumns(new string[] { "Institutions" });
                studies.SearchStudy(Institution: institutions, Datasource: studies.GetHostName(Config.SanityPACS));
                bool step17_1 = BasePage.GetColumnValues("Institutions").All<String>(columnvalue => columnvalue.ToLower().Contains(institutions.ToLower()));

                if (step17_1 && (BasePage.GetSearchResults().Count > 0))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }


                //step-18 - Modality
                studies.SelectGroupByInStudiesTab();
                studies.ChooseColumns(new string[] { "Modality" });
                studies.SearchStudy(Modality: modality, Datasource: studies.GetHostName(Config.SanityPACS));
                Thread.Sleep(60000);
                bool step18_1 = BasePage.GetColumnValues("Modality").All<String>(columnvalue => columnvalue.ToLower().Contains(modality.ToLower()));

                if (step18_1 && (BasePage.GetSearchResults().Count > 0))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }


                //Setp-19 - Gender
                studies.SelectGroupByInStudiesTab();
                studies.ChooseColumns(new string[] { "Gender" });
                studies.SearchStudy(Gender: gender, Datasource: "All");
                Thread.Sleep(180);
                bool step19_1 = BasePage.GetColumnValues("Gender").All<String>(columnvalue => columnvalue.ToLower().Contains(gender.ToLower()));

                if (step19_1 && (BasePage.GetSearchResults().Count > 0))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }


                //Setp-20 - Refering Physician
                studies.SelectGroupByInStudiesTab();
                studies.ChooseColumns(new string[] { "Ref. Physician" });
                studies.ClearFields();
                studies.RefPhysician().SendKeys(refphysician);
                bool step20_1 = BasePage.GetColumnValues("Ref. Physician").All<String>(columnvalue => columnvalue.ToLower().Equals(refphysician.ToLower()));

                if (step19_1 && (BasePage.GetSearchResults().Count > 0))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Setp-21 - first name and last name
                studies.SearchStudy(FirstName: firstname, LastName: lastname);
                bool step21_2 = BasePage.GetColumnValues("First Name").All<String>(columnvalue => columnvalue.ToLower().Contains(firstname.ToLower()));
                bool step21_3 = BasePage.GetColumnValues("Last Name").All<String>(columnvalue => columnvalue.ToLower().Contains(lastname.ToLower()));
                if (step21_2 && step21_3 && (BasePage.GetSearchResults().Count > 0))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }


                //setp-22 - Last Name, Study ID and Gender
                studies.SearchStudy(LastName: lastname, studyID: studyid, Gender: gender);
                bool step22_2 = BasePage.GetColumnValues("Study ID").All<String>(columnvalue => columnvalue.ToLower().Contains(studyid.ToLower()));
                bool step22_3 = BasePage.GetColumnValues("Last Name").All<String>(columnvalue => columnvalue.ToLower().Contains(lastname.ToLower()));
                bool step22_4 = BasePage.GetColumnValues("Gender").All<String>(columnvalue => columnvalue.ToLower().Equals(gender.ToLower()));
                if (step22_2 && step22_3 && step22_4 && (BasePage.GetSearchResults().Count > 0))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }


                //Step23 - Ref Physician and First name
                studies.SearchStudy(FirstName: firstname, physicianName: others);
                bool step23_2 = BasePage.GetColumnValues("Refer. Physician").All<String>(columnvalue => columnvalue.ToLower().Contains(others.ToLower()));
                bool step23_3 = BasePage.GetColumnValues("First Name").All<String>(columnvalue => columnvalue.ToLower().Contains(firstname.ToLower()));
                if (step23_2 && step23_3 && (BasePage.GetSearchResults().Count > 0))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }


                //Step-24 - logout and login as ref Physician
                login.Logout();
                login.LoginIConnect(D1ph, D1ph);
                executedSteps++;

                //Step-25   
                String refphy = D1ph.Split(' ')[0] + "," + " " + D1ph.Split(' ')[1];
                studies = new Studies();
                PageLoadWait.WaitForFrameLoad(10);
                studies.ClickElement(studies.MyPatients());
                studies.ClickElement(studies.Search());
                PageLoadWait.WaitForLoadingMessage(10);
                bool step25_2 = BasePage.GetColumnValues("Refer. Physician").All<String>(columnvalue => columnvalue.ToLower().Contains(refphy.ToLower()));
                if (step25_2 && (BasePage.GetSearchResults().Count > 0))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-26
                var studies26 = BasePage.GetSearchResults();
                studies.ChooseColumns(new string[] { "Refer. Physician" });
                studies.SelectStudy("Refer. Physician", refphy);
                var brviewer  = BluRingViewer.LaunchBluRingViewer();
                executedSteps++;


                //Step-27
                brviewer.CloseBluRingViewer();
                PageLoadWait.WaitForFrameLoad(10);
                var studies27 = BasePage.GetSearchResults();
                var dsvalue = studies.DataSource().FindElement(By.CssSelector("span")).GetAttribute("innerHTML");
                var spvalue = studies.StudyPerformed().FindElement(By.CssSelector("span")).GetAttribute("innerHTML");

                if ((studies27.Count == studies26.Count) && dsvalue.Equals("All") && spvalue.Equals("All Dates"))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }


                //Report Result                
                PageLoadWait.WaitForFrameLoad(10);
                studies.JSSelectDataSource("All");
                studies.SelectGroupByInStudiesTab();
                studies.SearchStudy("Last Name", "*");
                PageLoadWait.WaitForLoadingMessage(120);
                login.Logout();
                result.FinalResult(executedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Results
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout                
                studies.JSSelectDataSource("All");
                studies.SelectGroupByInStudiesTab();
                studies.SearchStudy("Last Name", "*");
                PageLoadWait.WaitForLoadingMessage(120);
                login.Logout();

                //Return Result
                return result;
            }
        }      

        /// <summary>
        ///Patient Name Search
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_162428(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            Patients patients = new Patients();
            Studies studies = new Studies();
            StudyViewer viewer = new StudyViewer();
            ServiceTool tool = new ServiceTool();
            BasePage basepage = new BasePage();
            DomainManagement domainManagement = new DomainManagement();
            RoleManagement roleManagement = new RoleManagement();
            UserManagement userManagement = new UserManagement();
            WpfObjects wpfobject = new WpfObjects();

            //required variables
            String adminUsername = Config.adminUserName;
            String adminPassword = Config.adminPassword;
            String domainName = "SuperAdminGroup";
            String Testdomain = "Test94_DomainA" + new Random().Next(1, 1000);
            String TestdomainAdmin = "Test94_DomainAdminA" + new Random().Next(1, 1000);
            String Testrole1 = "Test94_Role1A" + new Random().Next(1, 1000);
            String Testrole2 = "Test94_Role2A" + new Random().Next(1, 1000);
            String Testuser1 = "Test94_User1A" + new Random().Next(1, 1000);
            String Testuser2 = "Test94_User2A" + new Random().Next(1, 1000);
            String Testuser3 = "Test94_User3A" + new Random().Next(1, 1000);
            String Testuser4 = "Test94_User4A" + new Random().Next(1, 1000);
            String[] datasource = new String[] { basepage.GetHostName(Config.EA1), basepage.GetHostName(Config.EA77), basepage.GetHostName(Config.EA91), basepage.GetHostName(Config.SanityPACS) };
            String[] patientId = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID")).Split(':');
            String[] accessionNo = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID")).Split(':');
            String[] patientName = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName")).Split(':');


            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Pre-Conditions
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUsername, adminPassword);
                domainManagement = (DomainManagement)login.Navigate("DomainManagement");
                domainManagement.SearchDomain(domainName);
                PageLoadWait.WaitForFrameLoad(5);
                domainManagement.SelectDomain(domainName);
                domainManagement.ClickEditDomain();
                domainManagement.DisConnectAllDataSources();
                domainManagement.ConnectDataSource(datasource[0]);
                domainManagement.ConnectDataSource(datasource[1]);
                domainManagement.ConnectDataSource(datasource[2]);
                domainManagement.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(30);

                login.Navigate("DomainManagement");
                domainManagement.CreateDomain(Testdomain, TestdomainAdmin, datasource);
                domainManagement.ClickSaveNewDomain();
                roleManagement = (RoleManagement)login.Navigate("RoleManagement");
                roleManagement.CreateRole(Testdomain, Testrole1, "any");
                roleManagement.CreateRole(Testdomain, Testrole2, "any");
                userManagement = (UserManagement)login.Navigate("UserManagement");
                userManagement.CreateUser(Testuser1, Testdomain, Testrole1);
                userManagement.CreateUser(Testuser2, Testdomain, Testrole1);
                userManagement.CreateUser(Testuser3, Testdomain, Testrole2);
                userManagement.CreateUser(Testuser4, Testdomain, Testrole2);
                login.Logout();

                //Step 1: In Service tool- Enable Feature tab, enable Patient Name Search, restart IIS.
                //Pre -Conditions
                Taskbar taskbar = new Taskbar();
                taskbar.Hide();
                tool.InvokeServiceTool();
                tool.NavigateToEnableFeatures();
                tool.SetEnableFeaturesGeneral();
                wpfobject.WaitTillLoad();
                tool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                tool.EnablePatientNameSearch(true);
                wpfobject.WaitTillLoad();
                tool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                wpfobject.ClickOkPopUp();
                wpfobject.WaitTillLoad();
                tool.RestartIISandWindowsServices();
                tool.CloseServiceTool();
                taskbar.Show();

                ExecutedSteps++;

                //Step 2: Log in as Administrator. Go to Domain edit page 
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUsername, adminPassword);
                domainManagement = (DomainManagement)login.Navigate("DomainManagement");
                domainManagement.SearchDomain(domainName);
                PageLoadWait.WaitForFrameLoad(5);
                domainManagement.SelectDomain(domainName);
                domainManagement.ClickEditDomain();
                PageLoadWait.WaitForFrameLoad(5);


                domainManagement.SetCheckBoxInEditDomain("patientnamesearch", 1);
                bool step1 = domainManagement.VerifyCheckBoxInEditDomain("patientnamesearch");
                if (step1 == false)
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


                //Step 3: Check"Enable Patient Name Search"option, then save the change.
                domainManagement.SetCheckBoxInEditDomain("patientnamesearch", 0);
                bool step1_1 = domainManagement.VerifyCheckBoxInEditDomain("patientnamesearch");
                domainManagement.ClickSaveDomain();
                PageLoadWait.WaitForPageLoad(30);

                if (step1_1)
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


                //Step 4: Go to Study tab.
                studies = (Studies)login.Navigate("Studies");
                bool step4 = studies.RadioBtn_PatientNameSearch().Displayed;

                if (step4 && basepage.GetCurrentSelectedtab().Equals("Studies"))
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

                //Step 5: Click"Patient Name Search"option to go to Patient name search page.
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                studies.RadioBtn_PatientNameSearch().Click();

                if (studies.PatNmeField().Displayed && studies.StudyPerformed().Displayed && studies.DataSource().Displayed && studies.SearchBtn().Displayed)
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

                //step 6: Enter "a" to the Patient Search Name text field. Study Performed as All Dates Data Source: All Click"Search"button.
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                studies.PatientNameSearch("a");
                PageLoadWait.WaitForLoadingMessage();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                String[] PatientName = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Patient Name", BasePage.GetColumnNames());

                String[] FullName = PatientName.Where(e => e != null).Select(r => r.Replace(", ", " ")).ToArray();
                var testpass = FullName.Where(e => e != null).All(f => f.Split(' ')[0].ToLower().StartsWith("a") || (f.Split(' ').Count() > 1 ? f.Split(' ')[1].ToLower().StartsWith("a") : false) || (f.Split(' ').Count() > 2 ? f.Split(' ')[2].ToLower().StartsWith("a") : false));


                if (testpass)
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

                //Step 7: In the patient search text box try different type of searches.
                //1
                studies.PatientNameSearch("ma ha ch", Datasource: datasource[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                String patient = null;
                studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { patientId[0] }).TryGetValue("Patient Name", out patient);

                if (patient.ToLower().Equals(patientName[0].ToLower()))
                {
                    result.steps[++ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Returns a valid patient search");
                }
                else
                {
                    result.steps[++ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Returns an in-valid patient search");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //2
                studies.PatientNameSearch("ma Chrisd", Datasource: datasource[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { patientId[0] }).TryGetValue("Patient Name", out patient);

                if (patient.ToLower().Equals(patientName[0].ToLower()))
                {
                    result.steps[ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Returns a valid patient search");
                }
                else
                {
                    result.steps[ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Returns an in-valid patient search");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //3
                studies.PatientNameSearch("ma ch ha john", Datasource: datasource[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { patientId[0] }).TryGetValue("Patient Name", out patient);

                if (patient.ToLower().Equals(patientName[0].ToLower()))
                {
                    result.steps[ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Returns a valid patient search");
                }
                else
                {
                    result.steps[ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Returns an in-valid patient search");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //4
                studies.PatientNameSearch("ch ma ha", Datasource: datasource[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { patientId[0] }).TryGetValue("Patient Name", out patient);

                if (patient.ToLower().Equals(patientName[0].ToLower()))
                {
                    result.steps[ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Returns a valid patient search");
                }
                else
                {
                    result.steps[ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Returns an in-valid patient search");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //5
                studies.PatientNameSearch("mayr chrisd", Datasource: datasource[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { patientId[0] }).TryGetValue("Patient Name", out patient);

                if (patient.ToLower().Equals(patientName[0].ToLower()))
                {
                    result.steps[ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Returns a valid patient search");
                }
                else
                {
                    result.steps[ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Returns an in-valid patient search");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //6
                studies.PatientNameSearch("Hans Mayr Chrisd", Datasource: datasource[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { patientId[0] }).TryGetValue("Patient Name", out patient);

                if (patient.ToLower().Equals(patientName[0].ToLower()))
                {
                    result.steps[ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Returns a valid patient search");
                }
                else
                {
                    result.steps[ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Returns an in-valid patient search");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //7
                studies.PatientNameSearch("Hans Ma chris", Datasource: datasource[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { patientId[0] }).TryGetValue("Patient Name", out patient);

                if (patient.ToLower().Equals(patientName[0].ToLower()))
                {
                    result.steps[ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Returns an valid patient search");
                }
                else
                {
                    result.steps[ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Returns an in-valid patient search");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //8                
                studies.PatientNameSearch("chrisd", Datasource: datasource[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { patientId[0] }).TryGetValue("Patient Name", out patient);

                if (patient.ToLower().Equals(patientName[0].ToLower()))
                {
                    result.steps[ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Returns a valid patient search");
                }
                else
                {
                    result.steps[ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Returns an in-valid patient search");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //9
                studies.PatientNameSearch("*ayr ch", Datasource: datasource[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { patientId[0] }).TryGetValue("Patient Name", out patient);

                if (patient.ToLower().Equals(patientName[0].ToLower()))
                {
                    result.steps[ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Returns a valid patient search");
                }
                else
                {
                    result.steps[ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Returns an in-valid patient search");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //10
                studies.PatientNameSearch("*ans *risd", Datasource: datasource[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { patientId[0] }).TryGetValue("Patient Name", out patient);

                if (patient.ToLower().Equals(patientName[0].ToLower()))
                {
                    result.steps[ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Returns a valid patient search");
                }
                else
                {
                    result.steps[ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Returns an in-valid patient search");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Final Validation
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }

                //Step 8: Enter other combinations
                //1
                studies.PatientNameSearch("*ans *risd jack", Datasource: datasource[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                //studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { patientId }).TryGetValue("Patient Name", out patient);

                if (!basepage.IsElementVisible(studies.By_PatientListTable()))
                {
                    result.steps[++ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Returns a valid patient search");
                }
                else
                {
                    result.steps[++ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Returns an in-valid patient search");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //2
                studies.PatientNameSearch("a a chrisd", Datasource: datasource[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                //studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { patientId }).TryGetValue("Patient Name", out patient);

                if (!basepage.IsElementVisible(studies.By_PatientListTable()))
                {
                    result.steps[ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Returns a valid patient search");
                }
                else
                {
                    result.steps[ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Returns an in-valid patient search");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //3
                studies.PatientNameSearch("h a chr", Datasource: datasource[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                //studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { patientId }).TryGetValue("Patient Name", out patient);

                if (!basepage.IsElementVisible(studies.By_PatientListTable()))
                {
                    result.steps[ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Returns a valid patient search");
                }
                else
                {
                    result.steps[ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Returns an in-valid patient search");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //4
                studies.PatientNameSearch("chrisd a a", Datasource: datasource[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                //studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { patientId }).TryGetValue("Patient Name", out patient);

                if (!basepage.IsElementVisible(studies.By_PatientListTable()))
                {
                    result.steps[ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Returns a valid patient search");
                }
                else
                {
                    result.steps[ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Returns an in-valid patient search");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Final Validation
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }

                //Step 9: Open the study in the viewer
                studies.PatientNameSearch(patientName[1], Datasource: datasource[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                var row = studies.GetMatchingRow(new string[] { "Accession" }, new string[] { accessionNo[0] });
                // IList<IWebElement> table = BasePage.Driver.FindElement(By.CssSelector("#gridTableStudyList tbody tr[class*='ui-widget-content']")).FindElements(By.CssSelector("td"));
                studies.SelectStudy("Accession", accessionNo[0]);
                var brviewer = BluRingViewer.LaunchBluRingViewer();
                brviewer.CloseBluRingViewer();
                ExecutedSteps++;                

                var row1 = studies.GetMatchingRow(new string[] { "Accession" }, new string[] { accessionNo[0] });
                //IList<IWebElement> table1 = BasePage.Driver.FindElement(By.CssSelector("#gridTableStudyList tbody tr[class*='ui-widget-content']")).FindElements(By.CssSelector("td"));

                bool IsEqual = false;
                if (row["Patient Name"].Equals(row1["Patient Name"])) { IsEqual = true; }
                if (IsEqual && basepage.RadioBtn_PatientNameSearch().Selected)
                {
                    result.steps[ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Returns a valid patient search");
                }
                else
                {
                    result.steps[ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Returns an in-valid patient search");
                    result.steps[ExecutedSteps].SetLogs();
                }

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }

                //Step 10: Go to Domain management page. uncheck"Enable Patient Name Search"option, then save the change. Back to Study tab.
                login.Navigate("DomainManagement");
                PageLoadWait.WaitForFrameLoad(5);
                domainManagement.SearchDomain(domainName);
                PageLoadWait.WaitForFrameLoad(5);
                domainManagement.SelectDomain(domainName);
                domainManagement.ClickEditDomain();
                PageLoadWait.WaitForFrameLoad(5);

                domainManagement.SetCheckBoxInEditDomain("patientnamesearch", 1);
                bool step10 = domainManagement.VerifyCheckBoxInEditDomain("patientnamesearch");
                domainManagement.ClickSaveDomain();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                login.Navigate("Studies");

                if (step10 == false && basepage.GetCurrentSelectedtab().Equals("Studies") && !basepage.IsElementVisible(studies.By_RadioBtn_PatientNameSearch()))
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

                //Step 11:Log out Administrator user and log in as TestDomain.                
                login.Navigate("DomainManagement");
                PageLoadWait.WaitForFrameLoad(5);
                domainManagement.SearchDomain(Testdomain);
                PageLoadWait.WaitForFrameLoad(5);
                domainManagement.SelectDomain(Testdomain);
                domainManagement.ClickEditDomain();
                domainManagement.DisConnectDataSource(datasource[3]);
                //domain.DisConnectDataSource(Config.rdm);
                domainManagement.ClickSaveDomain();
                PageLoadWait.WaitForPageLoad(30);
                login.Logout();

                login.LoginIConnect(Testdomain, Testdomain);
                domainManagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForPageLoad(5);
                PageLoadWait.WaitForFrameLoad(5);
                if (basepage.GetCurrentSelectedtab().Equals("Domain Management"))
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

                //Step 12: Go to Domain management page. Check"Enable Patient Name Search"option, then save the change.
                //domainManagement.SelectDomain(Testdomain);
                //domainManagement.ClickEditDomain();
                PageLoadWait.WaitForFrameLoad(5);
                //domainManagement.DisConnectDataSource(datasource[3]);

                domainManagement.SetCheckBoxInEditDomain("patientnamesearch", 0);
                bool step12 = domainManagement.VerifyCheckBoxInEditDomain("patientnamesearch");
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
                domainManagement.ClickSaveEditDomain();


                //bool step12_1 = !domainManagement.successMsgDiv().Displayed;
                //bool step12_2 = domainManagement.successMsg().Text.Equals("Domain configuration has been successfully updated");
                //domainManagement.successDivCloseBtn().Click();

                if (step12)
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

                //Step 13: Log out as TestDomain and log in as user"a".Go to Study tab.
                login.Logout();

                login.LoginIConnect(Testuser1, Testuser1);
                studies = (Studies)login.Navigate("Studies");

                bool step13 = studies.RadioBtn_PatientNameSearch().Displayed;

                if (step13 && basepage.GetCurrentSelectedtab().Equals("Studies"))
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

                //Step 14: Click"Patient Name Search"option to go to Patient name search page.
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                studies.RadioBtn_PatientNameSearch().Click();

                if (studies.PatNmeField().Displayed && studies.StudyPerformed().Displayed && studies.DataSource().Displayed && studies.SearchBtn().Displayed)
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


                //step 15: Enter "a" to the Patient Search Name text field. Study Performed as All Dates Data Source: All Click"Search"button.
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                studies.PatientNameSearch("a");
                PageLoadWait.WaitForLoadingMessage();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                String[] PatientName1 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Patient Name", BasePage.GetColumnNames());
                String[] FullName1 = PatientName1.Where(e => e != null).Select(r => r.Replace(", ", " ")).ToArray();

                var testpass1 = FullName1.Where(e => e != null).All(f => f.Split(' ')[0].ToLower().StartsWith("a") || (f.Split(' ').Count() > 1 ? f.Split(' ')[1].ToLower().StartsWith("a") : false) || (f.Split(' ').Count() > 2 ? f.Split(' ')[2].ToLower().StartsWith("a") : false));


                if (testpass1)
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

                //Step 16: In the patient search text box try different type of searches.
                //1
                studies.PatientNameSearch("ma ha ch", Datasource: datasource[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                String patient1 = null;
                studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { patientId[0] }).TryGetValue("Patient Name", out patient1);

                if (patient1.ToLower().Equals(patientName[0].ToLower()))
                {
                    result.steps[++ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Returns a valid patient search");
                }
                else
                {
                    result.steps[++ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Returns an in-valid patient search");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //2
                studies.PatientNameSearch("ma Chrisd", Datasource: datasource[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { patientId[0] }).TryGetValue("Patient Name", out patient1);

                if (patient1.ToLower().Equals(patientName[0].ToLower()))
                {
                    result.steps[ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Returns a valid patient search");
                }
                else
                {
                    result.steps[ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Returns an in-valid patient search");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //3
                studies.PatientNameSearch("ma ch ha john", Datasource: datasource[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { patientId[0] }).TryGetValue("Patient Name", out patient1);

                if (patient1.ToLower().Equals(patientName[0].ToLower()))
                {
                    result.steps[ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Returns a valid patient search");
                }
                else
                {
                    result.steps[ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Returns an in-valid patient search");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //4
                studies.PatientNameSearch("ch ma ha", Datasource: datasource[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { patientId[0] }).TryGetValue("Patient Name", out patient1);

                if (patient1.ToLower().Equals(patientName[0].ToLower()))
                {
                    result.steps[ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Returns a valid patient search");
                }
                else
                {
                    result.steps[ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Returns an in-valid patient search");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //5
                studies.PatientNameSearch("mayr chrisd", Datasource: datasource[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { patientId[0] }).TryGetValue("Patient Name", out patient1);

                if (patient1.ToLower().Equals(patientName[0].ToLower()))
                {
                    result.steps[ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Returns a valid patient search");
                }
                else
                {
                    result.steps[ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Returns an in-valid patient search");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //6
                studies.PatientNameSearch("Hans Mayr Chrisd", Datasource: datasource[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { patientId[0] }).TryGetValue("Patient Name", out patient1);

                if (patient1.ToLower().Equals(patientName[0].ToLower()))
                {
                    result.steps[ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Returns a valid patient search");
                }
                else
                {
                    result.steps[ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Returns an in-valid patient search");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //7
                studies.PatientNameSearch("Hans Ma chris", Datasource: datasource[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { patientId[0] }).TryGetValue("Patient Name", out patient1);

                if (patient1.ToLower().Equals(patientName[0].ToLower()))
                {
                    result.steps[ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Returns an valid patient search");
                }
                else
                {
                    result.steps[ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Returns an in-valid patient search");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //8                
                studies.PatientNameSearch("chrisd", Datasource: datasource[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { patientId[0] }).TryGetValue("Patient Name", out patient1);

                if (patient1.ToLower().Equals(patientName[0].ToLower()))
                {
                    result.steps[ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Returns a valid patient search");
                }
                else
                {
                    result.steps[ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Returns an in-valid patient search");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //9
                studies.PatientNameSearch("*ayr ch", Datasource: datasource[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { patientId[0] }).TryGetValue("Patient Name", out patient1);

                if (patient1.ToLower().Equals(patientName[0].ToLower()))
                {
                    result.steps[ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Returns a valid patient search");
                }
                else
                {
                    result.steps[ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Returns an in-valid patient search");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //10
                studies.PatientNameSearch("*ans *risd", Datasource: datasource[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { patientId[0] }).TryGetValue("Patient Name", out patient1);

                if (patient1.ToLower().Equals(patientName[0].ToLower()))
                {
                    result.steps[ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Returns a valid patient search");
                }
                else
                {
                    result.steps[ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Returns an in-valid patient search");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Final Validation
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }

                //Step 17: Enter other combinations
                //1
                studies.PatientNameSearch("*ans *risd jack", Datasource: datasource[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                //studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { patientId }).TryGetValue("Patient Name", out patient);

                if (!basepage.IsElementVisible(studies.By_PatientListTable()))
                {
                    result.steps[++ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Returns a valid patient search");
                }
                else
                {
                    result.steps[++ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Returns an in-valid patient search");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //2
                studies.PatientNameSearch("a a chrisd", Datasource: datasource[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                //studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { patientId }).TryGetValue("Patient Name", out patient);

                if (!basepage.IsElementVisible(studies.By_PatientListTable()))
                {
                    result.steps[ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Returns a valid patient search");
                }
                else
                {
                    result.steps[ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Returns an in-valid patient search");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //3
                studies.PatientNameSearch("h a chr", Datasource: datasource[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                //studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { patientId }).TryGetValue("Patient Name", out patient);

                if (!basepage.IsElementVisible(studies.By_PatientListTable()))
                {
                    result.steps[ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Returns a valid patient search");
                }
                else
                {
                    result.steps[ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Returns an in-valid patient search");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //4
                studies.PatientNameSearch("chrisd a a", Datasource: datasource[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                //studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { patientId }).TryGetValue("Patient Name", out patient);

                if (!basepage.IsElementVisible(studies.By_PatientListTable()))
                {
                    result.steps[ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Returns a valid patient search");
                }
                else
                {
                    result.steps[ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Returns an in-valid patient search");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Final Validation
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                login.Logout();

                //Step 18: Configure Amicas PACS as available data source
                login.LoginIConnect(adminUsername, adminPassword);
                domainManagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForFrameLoad(5);
                domainManagement.SearchDomain(Testdomain);
                PageLoadWait.WaitForFrameLoad(5);
                domainManagement.SelectDomain(Testdomain);
                domainManagement.ClickEditDomain();
                domainManagement.ConnectDataSource(datasource[3]);
                //domain.DisConnectDataSource(Config.rdm);
                domainManagement.ClickSaveDomain();
                PageLoadWait.WaitForPageLoad(30);
                login.Logout();

                ExecutedSteps++;

                //Step 19: Enter MR to the Patient Search Name text field. Study Performed as All Dates Data Source: AmicasPACS.Click"Search"button.
                login.LoginIConnect(Testuser1, Testuser1);
                studies = (Studies)login.Navigate("Studies");
                studies.PatientNameSearch(patientName[2], Datasource: datasource[3]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                if (!basepage.IsElementVisible(studies.By_PatientListTable()))
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

                //Step 20: Enter CARDIACMR to the Patient Search Name text field. Study Performed as All Dates Data Source: AmicasPACS.Click"Search"button.
                studies.PatientNameSearch(patientName[4], Datasource: datasource[3]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                studies.GetMatchingRow(new string[] { "Accession" }, new string[] { accessionNo[2] }).TryGetValue("Patient Name", out patient1);

                if (patient1.Equals(patientName[3]))
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
                login.Logout();
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
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUsername, adminPassword);
                domainManagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForFrameLoad(5);
                domainManagement.SearchDomain(domainName);
                PageLoadWait.WaitForFrameLoad(5);
                domainManagement.SelectDomain(domainName);
                domainManagement.ClickEditDomain();
                PageLoadWait.WaitForFrameLoad(5);
                domainManagement.ConnectAllDataSources();
                domainManagement.SetCheckBoxInEditDomain("patientnamesearch", 1);
                domainManagement.ClickSaveDomain();

                domainManagement.SearchDomain(Testdomain);
                PageLoadWait.WaitForFrameLoad(5);
                domainManagement.SelectDomain(Testdomain);
                domainManagement.ClickEditDomain();
                PageLoadWait.WaitForFrameLoad(5);

                domainManagement.SetCheckBoxInEditDomain("patientnamesearch", 1);
                domainManagement.ClickSaveDomain();
                PageLoadWait.WaitForFrameLoad(5);
                login.Logout();
            }
        }

        /// <summary>
        /// Saved Search
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_162430(String testid, String teststeps, int stepcount)
        {

            //Decalre Variables
            int executedSteps = -1;
            TestCaseResult result = new TestCaseResult(stepcount);
            BasePage basepage = new BasePage();
            Studies studies;
            DomainManagement domain;
            Random randomnumber = new Random();
            String searchname1 = "Test" + randomnumber.Next(100, 10000);
            String searchname2 = "Test" + randomnumber.Next(100, 10000);
            String searchname3 = "Test" + randomnumber.Next(100, 10000);

            String firstname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
            String lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
            String studyid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyID");
            String patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
            String gender = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Gender");
            String fullname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName");
            String testdomain = "SuperAdminGroup";

            try
            {
                //Set Test step description            
                result.SetTestStepDescription(teststeps);

                //Precondition
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domain = login.Navigate<DomainManagement>();
                domain.SearchDomain(testdomain);
                domain.SelectDomain(testdomain);
                domain.ClickEditDomain();
                domain.ModifyStudySearchFields("show", new string[] { "First Name", "Last Name", "Gender", "Study ID", "Patient ID", "Accession Number" });
                domain.ClickSaveDomain();
                studies = login.Navigate<Studies>();
                //Delete all search presets
                var searchpreset_pre = new SelectElement(studies.SearchPreset());
                foreach (var element1 in searchpreset_pre.Options)
                {
                    searchpreset_pre.SelectByText(element1.GetAttribute("innerHTML"));
                    studies.Delete().Click();
                    Thread.Sleep(1000);
                }
                login.Logout();

                //Step-1
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = login.Navigate<Studies>();
                studies.SearchStudy(LastName: lastname, FirstName: firstname);
                var study1 = studies.GetMatchingRow("Patient Name", fullname);
                if (study1 != null)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }


                //Step-2
                Dictionary<int, string[]> results_BeforeViewer = BasePage.GetSearchResults();
                IList<string> columnvalues_BeforeViewer = BasePage.GetColumnValues(results_BeforeViewer, "Accession", BasePage.GetColumnNames());
                studies.SelectStudy("Patient Name", fullname);
                var brviewer = BluRingViewer.LaunchBluRingViewer();
                executedSteps++;

                //Step-3
                brviewer.CloseBluRingViewer();
                Dictionary<int, string[]> results_AfterViewer = BasePage.GetSearchResults();
                IList<string> columnvalues_AfterViewer = BasePage.GetColumnValues(results_AfterViewer, "Accession", BasePage.GetColumnNames());
                bool searchField = basepage.GetValue(basepage.LastNameTextBox) == lastname && basepage.GetValue(basepage.FirstNameTextBox) == firstname;
                if (columnvalues_AfterViewer.SequenceEqual(columnvalues_BeforeViewer) && searchField)
                {
                    result.steps[++executedSteps].StepPass();
                }
                else
                {
                    result.steps[++executedSteps].StepFail();
                }

                //Step-4
                PageLoadWait.WaitForFrameLoad(10);
                studies.ClickElement(studies.Save());
                BasePage.wait.Until((driver) =>
                {
                    if ((driver.FindElement(studies.SavePopup()).Displayed) && (driver.FindElement(studies.SaveSearch()).Displayed) &&
                        (driver.FindElement(studies.CancelSearch()).Displayed) &&
                        (driver.FindElement(studies.RadioMySearch()).Displayed) && (driver.FindElement(studies.RadioPreset()).Displayed))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                executedSteps++;

                //Setp-5
                studies.ClickElement(BasePage.Driver.FindElement(studies.CancelSearch()));
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(studies.SavePopup()));
                var study5 = studies.GetMatchingRow("Patient Name", fullname);
                if (study5 != null)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-6
                var searchpreset = new SelectElement(studies.SearchPreset());
                new Actions(BasePage.Driver).Click(studies.SearchPreset());
                if (searchpreset.Options.Count == 1 &&  String.IsNullOrEmpty(searchpreset.SelectedOption.GetAttribute("innerHTML")) )
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-7
                studies.ClickElement(studies.Save());
                BasePage.wait.Until((driver) =>
                {
                    if ((driver.FindElement(studies.SavePopup()).Displayed) && (driver.FindElement(studies.SaveSearch()).Displayed) &&
                        (driver.FindElement(studies.CancelSearch()).Displayed) &&
                        (driver.FindElement(studies.RadioMySearch()).Displayed) && (driver.FindElement(studies.RadioPreset()).Displayed))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                BasePage.Driver.FindElement(studies.SearchName()).SendKeys(searchname1);
                studies.ClickElement(BasePage.Driver.FindElement(studies.SaveSearch()));
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(studies.SavePopup()));
                executedSteps++;

                //Step-8
                var searchpreset8 = new SelectElement(studies.SearchPreset());
                new Actions(BasePage.Driver).Click(studies.SearchPreset());
                bool flag8 = searchpreset8.Options.Any<IWebElement>(element => element.GetAttribute("innerHTML").Trim().Equals(searchname1));
                if (flag8)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-9 - Create 2 Presets
                studies.ClearFields();
                studies.SearchStudy(patientID: patientid, studyID: studyid);
                studies.ClickElement(studies.Save());
                BasePage.wait.Until((driver) =>
                {
                    if ((driver.FindElement(studies.SavePopup()).Displayed) && (driver.FindElement(studies.SaveSearch()).Displayed) &&
                        (driver.FindElement(studies.CancelSearch()).Displayed) &&
                        (driver.FindElement(studies.RadioMySearch()).Displayed) && (driver.FindElement(studies.RadioPreset()).Displayed))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                BasePage.Driver.FindElement(studies.SearchName()).SendKeys(searchname2);
                studies.ClickElement(BasePage.Driver.FindElement(studies.SaveSearch()));
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(studies.SavePopup()));

                studies.ClearFields();
                studies.SearchStudy(AccessionNo: accession, Gender: gender);
                studies.ClickElement(studies.Save());
                BasePage.wait.Until((driver) =>
                {
                    if ((driver.FindElement(studies.SavePopup()).Displayed) && (driver.FindElement(studies.SaveSearch()).Displayed) &&
                        (driver.FindElement(studies.CancelSearch()).Displayed) &&
                        (driver.FindElement(studies.RadioMySearch()).Displayed) && (driver.FindElement(studies.RadioPreset()).Displayed))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                BasePage.Driver.FindElement(studies.SearchName()).SendKeys(searchname3);
                studies.ClickElement(BasePage.Driver.FindElement(studies.SaveSearch()));
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(studies.SavePopup()));
                executedSteps++;


                //Step-10 - Validate presets created and default empty presets
                var searchpreset10 = new SelectElement(studies.SearchPreset());
                new Actions(BasePage.Driver).Click(studies.SearchPreset());
                bool flag10_1 = searchpreset10.Options.Any<IWebElement>(element => element.GetAttribute("innerHTML").Trim().Equals(searchname1));
                bool flag10_2 = searchpreset10.Options.Any<IWebElement>(element => element.GetAttribute("innerHTML").Trim().Equals(searchname2));
                bool flag10_3 = searchpreset10.Options.Any<IWebElement>(element => element.GetAttribute("innerHTML").Trim().Equals(searchname3));
                bool flag10_4 = searchpreset10.Options.Any<IWebElement>(element => element.GetAttribute("innerHTML").Trim().Equals(""));
                if (flag10_1 && flag10_2 && flag10_3 && flag10_4)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-11 -- Select Preset created at step7 and validate
                login.Logout();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                login.Navigate<Studies>();
                studies.ClearFields();
                var searchpreset11 = new SelectElement(studies.SearchPreset());
                new Actions(BasePage.Driver).Click(studies.SearchPreset());
                searchpreset11.SelectByText(searchname1);
                PageLoadWait.WaitForPageLoad(10);
                String firstnameactual = (String)((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("return(arguments[0].value)", studies.FirstName());
                String lastnameactual = (String)((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("return(arguments[0].value)", studies.LastName());
                bool flag11_1 = firstnameactual.Equals(firstname);
                bool flag11_2 = lastnameactual.Equals(lastname);
                var study11_before = studies.GetMatchingRow("Patient Name", fullname);
                studies.ClickSearchBtn();
                var study11_after = studies.GetMatchingRow("Patient Name", fullname);
                if (flag11_1 && flag11_2 && (study11_before == null) && (study11_after != null))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }


                //Step-12  -- Select Preset with name searchname3
                login.Logout();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                login.Navigate<Studies>();
                studies.ClearFields();
                var searchpreset12 = new SelectElement(studies.SearchPreset());
                new Actions(BasePage.Driver).Click(studies.SearchPreset());
                searchpreset12.SelectByText(searchname3);
                PageLoadWait.WaitForPageLoad(10);
                String accessionactual = (String)((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("return(arguments[0].value)", studies.Accession());
                String genderactual = new SelectElement((BasePage.Driver.FindElement(By.CssSelector("#m_studySearchControl_m_searchInputPatientGender")))).SelectedOption.GetAttribute("innerHTML");
                bool flag12_1 = accessionactual.Equals(accession);
                bool flag12_2 = genderactual.Equals(gender);
                var study12_before = studies.GetMatchingRow("Accession", accession);
                studies.ClickSearchBtn();
                var study12_after = studies.GetMatchingRow("Accession", accession);
                if (flag12_1 && flag12_2 && (study12_before == null) && (study12_after != null))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }


                //Step-13 --Delete the preset with searchname3
                studies.ClickElement(studies.Delete());
                var searchpreset13 = new SelectElement(studies.SearchPreset());
                new Actions(BasePage.Driver).Click(studies.SearchPreset());
                if (String.IsNullOrEmpty(searchpreset13.SelectedOption.GetAttribute("innerHTML")))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-14
                studies.ClearFields();
                var searchpreset14 = new SelectElement(studies.SearchPreset());
                new Actions(BasePage.Driver).Click(studies.SearchPreset());
                searchpreset14.SelectByText(searchname2);
                studies.ClickSearchBtn();
                studies.SelectStudy("Patient ID", patientid);
                results_BeforeViewer = BasePage.GetSearchResults();
                columnvalues_BeforeViewer = BasePage.GetColumnValues(results_BeforeViewer, "Accession", BasePage.GetColumnNames());
                BluRingViewer.LaunchBluRingViewer();
                executedSteps++;

                //Step-15
                brviewer.CloseBluRingViewer();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                var studychek = studies.GetMatchingRow("Patient Name", fullname);
                if (studychek != null)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-16
                PageLoadWait.WaitForFrameLoad(10);
                var searchpreset16 = new SelectElement(studies.SearchPreset());
                new Actions(BasePage.Driver).Click(studies.SearchPreset());
                bool flag17 = searchpreset16.Options.Any(element => element.GetAttribute("innerHTML").Trim().Equals(searchname3));
                if (!flag17)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-17
                var searchpreset17 = new SelectElement(studies.SearchPreset());
                new Actions(BasePage.Driver).Click(studies.SearchPreset());
                searchpreset17.SelectByText(searchname1);
                PageLoadWait.WaitForPageLoad(10);
                studies.ClickElement(studies.Save());
                BasePage.wait.Until((driver) =>
                {
                    if ((driver.FindElement(studies.SavePopup()).Displayed) && (driver.FindElement(studies.SaveSearch()).Displayed) &&
                        (driver.FindElement(studies.CancelSearch()).Displayed) &&
                        (driver.FindElement(studies.RadioMySearch()).Displayed) && (driver.FindElement(studies.RadioPreset()).Displayed))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                executedSteps++;

                //Step-18
                studies.ClickElement(BasePage.Driver.FindElement(studies.RadioMySearch()));
                studies.ClickElement(BasePage.Driver.FindElement(studies.SaveSearch()));
                executedSteps++;

                //Step-19
                studies.SearchStudy("Last Name", "*");
                PageLoadWait.WaitForLoadingMessage(120);
                executedSteps++;

                //Step-20
                studies.ClickElement(studies.MySearch());
                var study20 = studies.GetMatchingRow("Patient Nme", fullname);
                executedSteps++;


                //Report Result                
                login.Logout();
                result.FinalResult(executedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Results
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                BasePage.Driver.FindElement(By.CssSelector("td[title*='Reset']>div")).Click();
                login.Logout();

                //Return Result
                return result;
            }

        }

    }
}