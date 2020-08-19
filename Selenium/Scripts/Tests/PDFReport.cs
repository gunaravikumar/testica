using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OpenQA.Selenium;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MergeServiceTool;
using Selenium.Scripts.Reusable.Generic;
using System.Collections.ObjectModel;
using TestStack.White.UIItems.WindowItems;
using TestStack.White;
using CheckBox = TestStack.White.UIItems.CheckBox;
using System.ServiceProcess;
using System.Diagnostics;
using Window = TestStack.White.UIItems.WindowItems.Window;
using Panel = TestStack.White.UIItems.Panel;
using TestStack.White.Configuration;
using GroupBox = TestStack.White.UIItems.GroupBox;
using TextBox = TestStack.White.UIItems.TextBox;
using Tab = TestStack.White.UIItems.TabItems.Tab;
using ITabPage = TestStack.White.UIItems.TabItems.ITabPage;
using OpenQA.Selenium.Support.UI;

namespace Selenium.Scripts.Tests
{
    class PDFReport
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public WpfObjects wpfobject;
        public static IWebDriver Driver { get; set; }
        DirectoryInfo CurrentDir = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());
        string FolderPath = "";
        


        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public PDFReport(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            wpfobject = new WpfObjects();
            FolderPath = Config.downloadpath;//CurrentDir.Parent.Parent.FullName + "\\Downloads\\";
        }

        /// <summary>
        /// PDF Report Creation and download
        /// </summary>
        /// 
        public TestCaseResult Test1_28013(String testid, String teststeps, int stepcount)
        {
            Studies studies;
            DomainManagement domainmanagement;
            RoleManagement rolemanagement;
            Viewer viewer = null;
            viewer = new Viewer();

            WpfObjects wpfobject = new WpfObjects();
             
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
               
              
                String pid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                //String FilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FilePath");
                String FileName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FileName");
                String referph = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Refer. Physician");
                String lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientLastName");
                String DomainName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String RoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String Rolemanagement = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");

                ServiceTool servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.SetEnableFeaturesGeneral();
                servicetool.wpfobject.ClickButton(ServiceTool.ModifyBtn_Name, 1);
                servicetool.EnablePDFReport();
                servicetool.ApplyEnableFeatures();
                servicetool.wpfobject.ClickOkPopUp();
                servicetool.RestartService();
                servicetool.NavigateToTab(ServiceTool.Viewer_Tab);
                servicetool.wpfobject.GetTabWpf(1).SelectTabPage(4);
                servicetool.wpfobject.WaitTillLoad();
                servicetool.wpfobject.ClickButton(ServiceTool.ModifyBtn_Name, 1);
                servicetool.wpfobject.ClearText(ServiceTool.Viewer.ID.FileNameTxtBox);
                servicetool.wpfobject.SetText(ServiceTool.Viewer.ID.FileNameTxtBox, "MERGE HEALTHCARE.pdf");        
                servicetool.wpfobject.SelectCheckBox(ServiceTool.Viewer.ID.Caliper);    
                servicetool.wpfobject.SelectCheckBox(ServiceTool.Viewer.ID.OrientationLabel);      
                servicetool.wpfobject.SelectCheckBox(ServiceTool.Viewer.ID.GraphicAnnotation);
                servicetool.ApplyEnableFeatures();
                servicetool.RestartService();
                servicetool.CloseServiceTool();

                //Delete existing downloaded files if any
                var dir = new DirectoryInfo(FolderPath);

                foreach (var file in dir.EnumerateFiles("MERGE*.pdf"))
                {
                    file.Delete();
                }
                
                //Step 1
                login.DriverGoTo(login.url);
                login.LoginIConnect(Username, Password);

                //Navigate to DomainManagement Tab
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");

                
                //Click Edit in DomainManagement Tab
                domainmanagement.SearchDomain(DomainName);
                domainmanagement.SelectDomain(DomainName);
                domainmanagement.ClickEditDomain();

                //select PDFReport flag
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                domainmanagement.SetCheckBoxInEditDomain("pdfreport", 0);
                domainmanagement.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(5);

                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");

                //Click Edit in RoleManagement Tab     
                rolemanagement.SearchRole(RoleName,"SuperAdminGroup");
                rolemanagement.SelectRole(DomainName);
                rolemanagement.ClickEditRole();

                //select PDFReport flag
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                rolemanagement.SetCheckboxInEditRole("pdfreport", 0);
                rolemanagement.ClickSaveEditRole();
                PageLoadWait.WaitForPageLoad(5);

                //Navigate to Studies Tab
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("patientID", pid);
                studies.SearchStudy("ref", referph);
                studies.SearchStudy("PatientLastName", lastname);
                studies.SelectStudy1("Patient ID", pid);
                studies.LaunchStudy();
                if (studies.ViewStudy() == true)
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

                //Step 2: Select the Report viewer button
                viewer.ReportView();
                ReadOnlyCollection<IWebElement> elements = BasePage.Driver.FindElements(By.TagName("li"));
                bool step2 = false;
                foreach (IWebElement t in elements)
                 {
                     if (t.GetAttribute("title").Equals("PDF Report"))
                     {
                         IWebElement anchor = t.FindElement(By.TagName("img"));
                         var classText = anchor.GetAttribute("class");
                         if (classText.Equals("notSelected32 enabledOnCine"))
                         {
                             step2 = true;
                             break;
                         }
                     }
                     else
                     {
                         step2 = false;
                     }
                 }
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
                //Step 3
                viewer.ClickElement("PDF Report");
                PageLoadWait.WaitForPageLoad(10);
             
                if (Config.BrowserType == "chrome")
                {
                    var element = viewer.GetElement("xpath", "//img [@title='PDF Report']");  
                    if (element != null)
                    {
                        var classText = element.GetAttribute("class");

                        if (classText.Equals("notSelected32 enabledOnCine"))
                        {
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }

                        if (classText.Equals("notSelected32 enabledOnCine disableOnCine"))
                        {
                            result.steps[++ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("PDF Report icon does not exist");
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                //********POP -UP******//
                if (Config.BrowserType == "firefox")
                {
                    var x = Process.GetProcessesByName("firefox")[0].Id;

                    Logger.Instance.InfoLog("Application's process ID : " + x);

                    WpfObjects._application = Application.Attach(x);

                    wpfobject.GetMainWindowByIndex(1);
                    bool buttonexists = wpfobject.VerifyElement("OK", "OK", 1);

                    if (buttonexists)
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
                    wpfobject.ClickButton("OK", 1);
                
                }
                else if (Config.BrowserType == "ie")
                {
                    var x = Process.GetProcessesByName("iexplore")[0].Id;

                    Logger.Instance.InfoLog("Application's process ID : " + x);

                    WpfObjects._application = Application.Attach(x);
                    wpfobject.GetMainWindowByIndex(0);

                    Panel pane = WpfObjects._application.GetWindows()[0].Get<TestStack.White.UIItems.Panel>(TestStack.White.UIItems.Finders.SearchCriteria.All);
                    wpfobject.WaitTillLoad();
                    bool buttonexists = wpfobject.VerifyElement<TestStack.White.UIItems.Panel>(pane, "Open", "Open", 1);
                    if (buttonexists)
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
                    //Click at location where Save button is present
                    TestStack.White.InputDevices.Mouse.Instance.Click(new System.Windows.Point(((pane.Items[1].Location.X + pane.Items[2].Location.X) / 2 + 1), ((pane.Items[1].Location.Y + pane.Items[2].Location.Y) / 2 + 1)));
                    wpfobject.WaitTillLoad();

                }

                //Step-4
                PageLoadWait.WaitForDownload(FileName.Split('.')[0], FolderPath, FileName.Split('.')[1]);
                if (File.Exists(FolderPath + Path.DirectorySeparatorChar + FileName))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                PageLoadWait.WaitForDownload(FileName, Config.downloadpath, "pdf");
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs(); 
                }
               


                //Steps:-5 to 7 Not Automated Steps              
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 8
                //Close the study
                studies.CloseStudy();

                //Navigate to DomainManagement Tab
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");


                //Click Edit in DomainManagement Tab
                domainmanagement.SearchDomain(DomainName);
                domainmanagement.SelectDomain(DomainName);
                domainmanagement.ClickEditDomain();

                //Unselect PDFReport flag
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                domainmanagement.SetCheckBoxInEditDomain("pdfreport", 1);
                domainmanagement.ClickSaveEditDomain();
              //  domainmanagement.ClickCloseEditDomain();
                ExecutedSteps++;

                //Step 9
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("patientID", pid);
                studies.SearchStudy("ref", referph);
                studies.SearchStudy("PatientLastName", lastname);
                studies.SelectStudy1("Patient ID", pid);
                studies.LaunchStudy();

                //Select the Report viewer button
                viewer.ReportView();
                ReadOnlyCollection<IWebElement> elements1 = BasePage.Driver.FindElements(By.TagName("li"));

                bool step9 = false;
                foreach (IWebElement t in elements1)
                {
                    if (t.GetAttribute("title").Equals("PDF Report"))
                    {
                        step9 = true;
                        break;
                    }
                    else
                    {
                        step9 = false;
                    }
                }
                if (!step9)
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
                //Close the study
                studies.CloseStudy();
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
        /// PDF Report Creation and download
        /// </summary>
        /// 
        public TestCaseResult Test2_28013(String testid, String teststeps, int stepcount)
        {
            Studies studies;
            DomainManagement domainmanagement;
            Viewer viewer = null;
            viewer = new Viewer();

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

                String FilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FilePath");
                String FileName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FileName");
                String lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientLastName");
                String firstname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientFirstName");
                String DomainName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String pid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");

                //Deleting existing files
                var dir = new DirectoryInfo(FolderPath);


                foreach (var file in dir.EnumerateFiles("MERGE*.pdf"))
                {
                    file.Delete();
                }

                //Step 1
                //login.DriverGoTo(login.url);
                login.LoginIConnect(Username, Password);

                //Navigate to DomainManagement Tab
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");


                //Click Edit in DomainManagement Tab
                domainmanagement.SearchDomain(DomainName);
                domainmanagement.SelectDomain(DomainName);
                domainmanagement.ClickEditDomain();

                //select PDFReport flag
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                domainmanagement.SetCheckBoxInEditDomain("pdfreport", 0);
                domainmanagement.ClickSaveEditDomain();
              
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");

                if (BasePage.Driver.FindElement(By.CssSelector("#EditDomainButton")).Displayed)
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


                //Step 2
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("PatientFirstName", firstname);
                studies.SearchStudy("PatientLastName", lastname);
               // studies.SelectStudy1("PatientLastName", lastname);
                studies.SelectStudy1("Patient ID", pid);
                studies.LaunchStudy();
                if (studies.ViewStudy())
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

                //Select the Report viewer button
                viewer.ReportView();

                //Step 3
                viewer.ClickElement("PDF Report");
                if (Config.BrowserType == "chrome")
                {
                    var element = viewer.GetElement("xpath", "//img [@title='PDF Report']");
                    if (element != null)
                    {
                        var classText = element.GetAttribute("class");

                        if (classText.Equals("notSelected32 enabledOnCine"))
                        {
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }

                        if (classText.Equals("notSelected32 enabledOnCine disableOnCine"))
                        {
                            result.steps[++ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("PDF Report icon does not exist");
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                if (Config.BrowserType == "firefox")
                {
                    var x = Process.GetProcessesByName("firefox")[0].Id;

                    Logger.Instance.InfoLog("Application's process ID : " + x);

                    WpfObjects._application = Application.Attach(x);

                    wpfobject.GetMainWindowByIndex(1);
                    bool buttonexists = wpfobject.VerifyElement("OK", "OK", 1);

                    if (buttonexists)
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
                    wpfobject.ClickButton("OK", 1);

                }

                else if (Config.BrowserType == "ie")
                {
                    var x = Process.GetProcessesByName("iexplore")[0].Id;

                    Logger.Instance.InfoLog("Application's process ID : " + x);

                    WpfObjects._application = Application.Attach(x);
                    wpfobject.GetMainWindowByIndex(0);

                    Panel pane = WpfObjects._application.GetWindows()[0].Get<TestStack.White.UIItems.Panel>(TestStack.White.UIItems.Finders.SearchCriteria.All);
                    wpfobject.WaitTillLoad();
                    //wpfobject.GetSplitButton<TestStack.White.UIItems.Panel>(pane, "Save").Click();
                    bool buttonexists = wpfobject.VerifyElement<TestStack.White.UIItems.Panel>(pane, "Open", "Open", 1);
                    if (buttonexists)
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
                    TestStack.White.InputDevices.Mouse.Instance.Click(new System.Windows.Point(((pane.Items[1].Location.X + pane.Items[2].Location.X) / 2 + 1), ((pane.Items[1].Location.Y + pane.Items[2].Location.Y) / 2 + 1)));
                    wpfobject.WaitTillLoad();

                }
               
                //Steps:-4 
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-5  
                PageLoadWait.WaitForDownload(FileName, Config.downloadpath, "pdf");
                if (File.Exists(FolderPath + Path.DirectorySeparatorChar + FileName))
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
                             
                //Step6-8               
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Close the study
                studies.CloseStudy();
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
        /// Addendum reports
        /// </summary>
        /// 
        public TestCaseResult Test3_28013(String testid, String teststeps, int stepcount)
        {
            Studies studies;
            DomainManagement domainmanagement;
            Viewer viewer = null;
            viewer = new Viewer();

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

                String FilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FilePath");
                String FileName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FileName");
                String lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientLastName");
                String firstname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientFirstName");
                String DomainName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String pid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");

                //Deleting existing files
               
                var dir = new DirectoryInfo(FolderPath);

                foreach (var file in dir.EnumerateFiles("MERGE*.pdf"))
                {
                    file.Delete();
                }

                //Step 1 - Not automated
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 2
                login.DriverGoTo(login.url);
                login.LoginIConnect(Username, Password);

                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("PatientFirstName", firstname);
                studies.SearchStudy("PatientLastName", lastname);             
                studies.SelectStudy1("Patient ID", pid);
                studies.LaunchStudy();
                if (studies.ViewStudy())
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

                //Step 3
                //Select the Report viewer button
                viewer.ReportView();

                ReadOnlyCollection<IWebElement> elements = BasePage.Driver.FindElements(By.TagName("li"));
                bool step3 = false;
                foreach (IWebElement t in elements)
                {
                    if (t.GetAttribute("title").Equals("PDF Report"))
                    {
                        IWebElement anchor = t.FindElement(By.TagName("img"));
                        var classText = anchor.GetAttribute("class");
                        if (classText.Equals("notSelected32 enabledOnCine"))
                        {
                            step3 = true;
                            break;
                        }
                    }
                    else
                    {
                        step3 = false;
                    }
                }
                if (step3)
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
                //Step 4
                viewer.ClickElement("PDF Report");
                if (Config.BrowserType == "chrome")
                {
                    var element = viewer.GetElement("xpath", "//img [@title='PDF Report']");
                    if (element != null)
                    {
                        var classText = element.GetAttribute("class");

                        if (classText.Equals("notSelected32 enabledOnCine"))
                        {
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }

                        if (classText.Equals("notSelected32 enabledOnCine disableOnCine"))
                        {
                            result.steps[++ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("PDF Report icon does not exist");
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                if (Config.BrowserType == "firefox")
                {
                    var x = Process.GetProcessesByName("firefox")[0].Id;

                    Logger.Instance.InfoLog("Application's process ID : " + x);

                    WpfObjects._application = Application.Attach(x);

                    wpfobject.GetMainWindowByIndex(1);
                    bool buttonexists = wpfobject.VerifyElement("OK", "OK", 1);

                    if (buttonexists)
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
                    wpfobject.ClickButton("OK", 1);

                }

                else if (Config.BrowserType == "ie")
                {
                    var x = Process.GetProcessesByName("iexplore")[0].Id;

                    Logger.Instance.InfoLog("Application's process ID : " + x);

                    WpfObjects._application = Application.Attach(x);
                    wpfobject.GetMainWindowByIndex(0);

                    Panel pane = WpfObjects._application.GetWindows()[0].Get<TestStack.White.UIItems.Panel>(TestStack.White.UIItems.Finders.SearchCriteria.All);
                    wpfobject.WaitTillLoad();
                    //wpfobject.GetSplitButton<TestStack.White.UIItems.Panel>(pane, "Save").Click();
                    bool buttonexists = wpfobject.VerifyElement<TestStack.White.UIItems.Panel>(pane, "Open", "Open", 1);
                    if (buttonexists)
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
                    TestStack.White.InputDevices.Mouse.Instance.Click(new System.Windows.Point(((pane.Items[1].Location.X + pane.Items[2].Location.X) / 2 + 1), ((pane.Items[1].Location.Y + pane.Items[2].Location.Y) / 2 + 1)));
                    wpfobject.WaitTillLoad();

                    //pane = wpfobject.GetCurrentPane();
                    //wpfobject.GetButton<TestStack.White.UIItems.Panel>(pane, "Save").Click();
                    //wpfobject.WaitTillLoad();

                }



                //Step 5
                PageLoadWait.WaitForDownload(FileName, Config.downloadpath, "pdf");
                if (File.Exists(FolderPath + Path.DirectorySeparatorChar + FileName))
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
               

                //Step 6
                ServiceTool servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Viewer");
                servicetool.NavigateSubTab("Download Report");
                servicetool.wpfobject.ClickButton("Modify", 1);
                servicetool.wpfobject.SetSpinner("downloadReport_ImageWidth", "128");
                servicetool.wpfobject.SetSpinner("downloadReport_ImageHeight", "128");
                servicetool.wpfobject.SetSpinner("downloadReport_HorizontalSpacing", "8");
                servicetool.wpfobject.SetSpinner("downloadReport_VerticalSpacing", "8");
                servicetool.wpfobject.WaitTillLoad();
                servicetool.wpfobject.ClickButton("Apply", 1);
                servicetool.wpfobject.WaitTillLoad();
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Deleting existing files

                dir = new DirectoryInfo(FolderPath);

                foreach (var file in dir.EnumerateFiles("MERGE*.pdf"))
                {
                    file.Delete();
                }

                //Step 7    
                login.DriverGoTo(login.url);
                login.LoginIConnect(Username, Password);

                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("PatientFirstName", firstname);
                studies.SearchStudy("PatientLastName", lastname);           
                studies.SelectStudy1("Patient ID", pid);
                studies.LaunchStudy();
                if (studies.ViewStudy() == true)
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

                //Select the Report viewer button
                viewer.ReportView();
              
                //Step 8
                viewer.ClickElement("PDF Report");
                if (Config.BrowserType == "chrome")
                {
                    var element = viewer.GetElement("xpath", "//img [@title='PDF Report']");
                    if (element != null)
                    {
                        var classText = element.GetAttribute("class");

                        if (classText.Equals("notSelected32 enabledOnCine"))
                        {
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }

                        if (classText.Equals("notSelected32 enabledOnCine disableOnCine"))
                        {
                            result.steps[++ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("PDF Report icon does not exist");
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                if (Config.BrowserType == "firefox")
                {
                    var x = Process.GetProcessesByName("firefox")[0].Id;

                    Logger.Instance.InfoLog("Application's process ID : " + x);

                    WpfObjects._application = Application.Attach(x);

                    //wpfobject.GetMainWindow("iConnect® Access - Mozilla Firefox");
                    //     wpfobject.GetMainWindowFromDesktop("Opening MERGE+HEALTHCARE.pdf");

                    wpfobject.GetMainWindowByIndex(1);
                    bool buttonexists = wpfobject.VerifyElement("OK", "OK", 1);

                    if (buttonexists)
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
                    wpfobject.ClickButton("OK", 1);

                }

                else if (Config.BrowserType == "ie")
                {
                    var x = Process.GetProcessesByName("iexplore")[0].Id;

                    Logger.Instance.InfoLog("Application's process ID : " + x);

                    WpfObjects._application = Application.Attach(x);
                    wpfobject.GetMainWindowByIndex(0);

                    Panel pane = WpfObjects._application.GetWindows()[0].Get<TestStack.White.UIItems.Panel>(TestStack.White.UIItems.Finders.SearchCriteria.All);
                    wpfobject.WaitTillLoad();
                    //wpfobject.GetSplitButton<TestStack.White.UIItems.Panel>(pane, "Save").Click();
                    bool buttonexists = wpfobject.VerifyElement<TestStack.White.UIItems.Panel>(pane, "Open", "Open", 1);
                    if (buttonexists)
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
                    TestStack.White.InputDevices.Mouse.Instance.Click(new System.Windows.Point(((pane.Items[1].Location.X + pane.Items[2].Location.X) / 2 + 1), ((pane.Items[1].Location.Y + pane.Items[2].Location.Y) / 2 + 1)));
                    wpfobject.WaitTillLoad();

                }
                
              

                //Steps:-9
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 10:
                PageLoadWait.WaitForDownload(FileName, Config.downloadpath, "pdf");
                if (File.Exists(FolderPath + Path.DirectorySeparatorChar + FileName))
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
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Close the study
                studies.CloseStudy();
                login.Logout();
                login.CloseBrowser();

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
        /// Transfer to other datasource/remote/multiple pages
        /// </summary>
        /// 
        public TestCaseResult Test_66995(String testid, String teststeps, int stepcount)
        {
            Studies studies;
            DomainManagement domainmanagement;
            RoleManagement rolemanagement;
            Viewer viewer = null;
            viewer = new Viewer();

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

                String DomainName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String rdmserver = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RDMServer");
                String pid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientLastName");
               // string accno = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNo");

                String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNoList");
                String[] AccessionNo = AccessionNoList.Split(':');

                String DataSource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceList");
                String[] datasource = DataSource.Split(':');

                //Step-1
                //ServiceTool servicetool = new ServiceTool();
                //servicetool.LaunchServiceTool();
                //servicetool.wpfobject.WaitTillLoad();
                //servicetool.NavigateToTab(ServiceTool.DataSource_Tab);
                //servicetool.wpfobject.WaitTillLoad();
                //servicetool.wpfobject.ClickButton(ServiceTool.AddBtn_Name, 1);
                //servicetool.wpfobject.WaitTillLoad();
                //servicetool.wpfobject.GetMainWindowByIndex(1);              
                //servicetool.SetDataSourceName("RDM_Friendee");
                //servicetool.SetDataSourceType("6");
                //servicetool.NavigateToRDMTab();
                //servicetool.SetAddressInRDM(rdmserver);
                //servicetool.wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                //servicetool.RestartService();
                //servicetool.CloseServiceTool();
                ExecutedSteps++;
                //Step-2
              
                //servicetool.LaunchServiceTool();
                //servicetool.SetEnableFeaturesGeneral();
                //servicetool.wpfobject.ClickButton(ServiceTool.ModifyBtn_Name, 1);
                //servicetool.EnableDataDownloader();
                //servicetool.ApplyEnableFeatures();
                //servicetool.wpfobject.WaitTillLoad();
                //servicetool.wpfobject.ClickOkPopUp();
                //servicetool.wpfobject.WaitTillLoad();
                //servicetool.SetEnableFeaturesTransferService();
                //servicetool.wpfobject.ClickButton(ServiceTool.ModifyBtn_Name, 1);
                //servicetool.EnableTransferService();
                //servicetool.ModifyPackagerDetails("5");
                //servicetool.wpfobject.ClickOkPopUp();
                //servicetool.RestartService();           
                //servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Step-3

                login.DriverGoTo(login.url);
                login.LoginIConnect(Username, Password);

                //Navigate to DomainManagement Tab
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DomainName);
                domainmanagement.SelectDomain(DomainName);

                //Click Edit in DomainManagement Tab
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("datadownload", 0);
                domainmanagement.ConnectDataSources();
                domainmanagement.ClickSaveDomain();

                //Navigate to DomainManagement Tab
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectDomainfromDropDown(DomainName);              
                rolemanagement.SelectRole("SuperRole");

                //Click Edit in RoleManagement Tab
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("transfer", 0);   
                rolemanagement.SetCheckboxInEditRole("download", 0);              
                rolemanagement.ClickSaveRole();
               
                //Open User Preference
                rolemanagement.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
               
                rolemanagement.SetRadioButton("id", "DownloadRadioButtonList_0");
                rolemanagement.CloseUserPreferences();

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");

                if (BasePage.Driver.FindElement(By.CssSelector("#NewRoleButton")).Displayed)
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

                //Step-4             
                //Navigate to Studies Tab
                studies = (Studies)login.Navigate("Studies");
                studies.SelectAllDateAndData();

                if (BasePage.Driver.FindElement(By.CssSelector("#searchStudyDropDownMenu")).Text.Equals("All Dates"))
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

                //Step-5
                studies.ChooseColumns(new string[] { "Accession" });
                studies.SearchStudy(AccessionNo: AccessionNo[0], LastName: lastname, Datasource: Config.rdm + "." + datasource[3], rdm: true);

                studies.SelectStudy("Accession", AccessionNo[0]);
                bool StudiesSelected = true;
                if (studies.SelectedStudyrow(lastname) == null)
                {
                    StudiesSelected = false;
                }
                if (StudiesSelected)
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
              
                //Step-6
                //Click transfer Button
                studies.Click("id", "m_transferButton");
                BasePage.Driver.SwitchTo().DefaultContent();
                studies.SwitchTo("index", "0");

               if (BasePage.Driver.FindElement(By.CssSelector("#ctl00_StudyTransferControl_transferDataGrid")).Text.Contains(lastname))      
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
               
                //Step-7
                BasePage.Driver.SwitchTo().DefaultContent();              
                studies.SwitchTo("index", "0");
                studies.SelectFromList("id", "ctl00_StudyTransferControl_m_destinationSources", datasource[2], 1);                 
                studies.Click("id", "ctl00_StudyTransferControl_TransferButton");
                BasePage.Driver.SwitchTo().DefaultContent();             
                studies.SwitchTo("index", "0");

                if (studies.PatientExistsinTransfer(lastname))
                 //if (BasePage.Driver.FindElement(By.CssSelector("  #ctl00_TransferJobsListControl_RefreshTrasferButton")).Displayed)
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


                //Step 8 - Select Confirm-all in Quality Control Window
                studies.ClickConfirm_allInQCWindow();
              
                IList<IWebElement> tablerows = BasePage.Driver.FindElements(By.CssSelector("#ctl00_DataQCControl_datagrid>tbody>tr[title='']"));

                ExecutedSteps++;
                //Validate Selected studies show a Check mark - 
                foreach (IWebElement row in tablerows)
                {
                    if (row.FindElement(By.CssSelector("td>.QCData_Confirm")).Displayed == true)
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
                }

                //Step 9 - Select Submit in Quality Control Window 
                studies.ClickSubmitInQCWindow();
                ExecutedSteps++;

                //Step 10 - Select one study with ready status
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                new WebDriverWait(BasePage.Driver, new TimeSpan(0, 2, 0)).Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(10)>span[title*='Ready']")));
                BasePage.Driver.FindElement(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid > tbody > tr:nth-child(2) > td:nth-child(10) > span[title*='Ready']")).Click();
                ExecutedSteps++;

                //Step 11 - Click download button in transfer status window
                IWebElement downloadButton = BasePage.Driver.FindElement(By.CssSelector("#ctl00_TransferJobsListControl_m_submitButton"));
                downloadButton.Click();
                ExecutedSteps++;
             

                //Step-12
                studies.ClickButtonInDownloadPackagesWindow("Download");

                studies.ClickButtonInDownloadPackagesWindow("close");
                PageLoadWait.WaitForPageLoad(20);
                ExecutedSteps++;

                studies.TransferStatusClose();

                //Step-13
                studies.ChooseColumns(new string[] { "Accession" });
                studies.SearchStudy(AccessionNo: AccessionNo[0], LastName: lastname, Datasource: Config.rdm + "." + datasource[3], rdm: true);

                studies.SelectStudy("Accession", AccessionNo[0]);
                if (studies.SelectedStudyrow(lastname) == null)
                {
                    StudiesSelected = false;
                }
                if (StudiesSelected)
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

                //Step-14
                studies.Click("id", "m_transferButton");
                BasePage.Driver.SwitchTo().DefaultContent();
                studies.SwitchTo("index", "0");
                if (BasePage.Driver.FindElement(By.CssSelector("#ctl00_StudyTransferControl_transferDataGrid")).Text.Contains(lastname))                   
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
               
                //step-15
                studies.SwitchToDefault();            
                studies.SwitchTo("index", "0");
                studies.SelectFromList("id", "ctl00_StudyTransferControl_m_destinationSources", datasource[1], 1);          
                studies.Click("id", "ctl00_StudyTransferControl_TransferButton");
                BasePage.Driver.SwitchTo().DefaultContent();
                studies.SwitchTo("index", "0");
              
                if (studies.PatientExistsinTransfer(lastname))              
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

                //Step 16 - Select Confirm-all in Quality Control Window
                studies.ClickConfirm_allInQCWindow();

				tablerows = BasePage.Driver.FindElements(By.CssSelector("#ctl00_DataQCControl_datagrid>tbody>tr[title='']"));

                ExecutedSteps++;
                //Validate Selected studies show a Check mark - 
                foreach (IWebElement row in tablerows)
                {
                    if (row.FindElement(By.CssSelector("td>.QCData_Confirm")).Displayed == true)
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
                }

                
                //Step 17 - Select Submit in Quality Control Window
                studies.ClickSubmitInQCWindow();
                ExecutedSteps++;
                studies.TransferStatusClose();

                //Step-18
                studies.SelectAllDateAndData();
                studies.ChooseColumns(new string[] { "Accession" });
                studies.SearchStudy(AccessionNo: AccessionNo[1], patientID: pid, Datasource: Config.rdm + "." + datasource[3], rdm: true);

                studies.SelectStudy("Accession", AccessionNo[1]);
                if (studies.SelectedStudyrow(AccessionNo[1]) == null)
                {
                    StudiesSelected = false;
                }
                if (StudiesSelected)
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

                //Step-19
                studies.Click("id", "m_transferButton");
                BasePage.Driver.SwitchTo().DefaultContent();
                studies.SwitchTo("index", "0");
                if (BasePage.Driver.FindElement(By.CssSelector("#ctl00_StudyTransferControl_m_relatedStudiesToggleButton")).Displayed)
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

                //step-20
                studies.SwitchToDefault(); 
                studies.SwitchTo("index", "0");
                studies.SelectFromList("id", "ctl00_StudyTransferControl_m_destinationSources",datasource[2], 1);
             
                studies.Click("id", "ctl00_StudyTransferControl_TransferButton");
                BasePage.Driver.SwitchTo().DefaultContent();
                studies.SwitchTo("index", "0");
                if (BasePage.Driver.FindElement(By.CssSelector("#dataQCDiv")).Displayed)
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

                //Step 21 - Select Confirm-all in Quality Control Window
                studies.ClickConfirm_allInQCWindow();

                tablerows = BasePage.Driver.FindElements(By.CssSelector("#ctl00_DataQCControl_datagrid>tbody>tr[title='']"));

                ExecutedSteps++;
                //Validate Selected studies show a Check mark - 
                foreach (IWebElement row in tablerows)
                {
                    if (row.FindElement(By.CssSelector("td>.QCData_Confirm")).Displayed == true)
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
                }

              
                //Step 22 - Select Submit in Quality Control Window 
                studies.ClickSubmitInQCWindow();
                ExecutedSteps++;
                          
                studies.Click("id", "ctl00_TransferJobsListControl_m_closeDialogButton");
                login.Logout();
                studies.CloseBrowser();
                

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

    }
}
