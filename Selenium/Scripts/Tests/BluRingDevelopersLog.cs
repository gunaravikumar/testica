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
using Selenium.Scripts.Pages.iConnect;
using System.Data;
using System.Windows;
using Selenium.Scripts.Pages.MergeServiceTool;
using static Selenium.Scripts.Pages.MergeServiceTool.ServiceTool;


namespace Selenium.Scripts.Tests
{
	class BluRingDevelopersLog
	{
		public Login login { get; set; }
		public string filepath { get; set; }								
		public ServiceTool servicetool { get; set; }
		public WpfObjects wpfobject { get; set; }

		/// <summary>
		/// Constructor - Test Suite
		/// </summary>		
		public BluRingDevelopersLog(String classname)
		{
			login = new Login();
			login.DriverGoTo(login.url);			
			filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";			
			wpfobject = new WpfObjects();
			servicetool = new ServiceTool();
		}

        /// <summary>
        ///  Error message shall not display in Studies tab with PACS and EA Datasource
        /// </summary>
        public TestCaseResult Test_160840(String testid, String teststeps, int stepcount)
		{
			//Declare and initialize variables         
			TestCaseResult result = null;
			int ExecutedSteps = -1;
			try
			{
				result = new TestCaseResult(stepcount);
				result.SetTestStepDescription(teststeps);
				String adminUserName = Config.adminUserName;
				String adminPassword = Config.adminPassword;

				String AccessionIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");				
				String[] Accession = AccessionIDList.Split(':');				

				//Step 1 - Launch the application with a client browser 				
				login.DriverGoTo(login.url);
				ExecutedSteps++;

                //Step 2 - Login to WebAccess site with any privileged user and navigate to studies tab
                login.LoginIConnect(adminUserName, adminPassword);
                var studies = (Studies)login.Navigate("Studies");                
                if (login.IsTabPresent("Studies") && login.IsTabPresent("Patients") && login.IsTabPresent("Domain Management"))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

                //Step 3 - Select any PACS Datasource in the Data source drop down box and load any study with multiple series in Universal viewer after searching
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.SanityPACS));
				studies.SelectStudy("Accession", Accession[0]);
				var viewer = BluRingViewer.LaunchBluRingViewer();
				PageLoadWait.WaitForFrameLoad(50);
				PageLoadWait.WaitForPageLoad(50);
				ExecutedSteps++;

				//Step4 - Idle for about 15 minutes and close the study by clicking "x" icon on top right corner.
				Thread.Sleep(300000);
				Thread.Sleep(300000);
				Thread.Sleep(300000);
				Thread.Sleep(10000);
				viewer.CloseBluRingViewer();
				if (login.IsTabPresent("Studies") && login.IsTabPresent("Patients") && login.IsTabPresent("Domain Management"))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

                //Step5 - Click on Search button after entering different query parameters with same PACS datasource
                studies.SearchStudy(AccessionNo: Accession[1], Datasource: login.GetHostName(Config.SanityPACS));
				IWebElement ele = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_studySearchResult));	
				if (ele.GetAttribute("innerHTML").Trim().Equals("View 1 - 1 of 1"))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

                //Step6 - Load any study from Studies tab verify that the study is loaded properly.
                studies.SelectStudy("Accession", Accession[1]);
				viewer = BluRingViewer.LaunchBluRingViewer();
				PageLoadWait.WaitForFrameLoad(50);
				PageLoadWait.WaitForPageLoad(50);
				ExecutedSteps++; 

				//Step7 - WebAccessDeveloper log verification
				DateTime localDate4 = DateTime.Now;
				String format4 = "yyyyMMdd";
				string date = localDate4.ToString(format4);
				String FilePath = "C:\\Windows\\Temp" + Path.DirectorySeparatorChar + "WebAccessDeveloper-" + date;

				// Read WebAccessDeveloper file
				List<String> text = viewer.ReadLogFile(FilePath);
				int i = 1;
				int count = text.Count();
				bool IsMessagePresent = false;
				while (i < 300 && i < count)
				{
					if (text[count - i].Trim().Contains("Connection error, retrying"))
					{
						IsMessagePresent = true;
						break;
					}
					i++;
				}
				if (IsMessagePresent)
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

                //Step8 - Close the Study by clicking on " X " icon on top right corner
                viewer.CloseBluRingViewer();
                ExecutedSteps++;

                //Step 9 - Select any EA Datasource in the Data source drop down box and load any study with multiple series in Universal viewer after searching
                studies.SearchStudy(AccessionNo: Accession[2], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[2]);
                viewer = BluRingViewer.LaunchBluRingViewer();               
                ExecutedSteps++;

                //Step10 - Idle for about 15 minutes and close the study by clicking "x" icon on top right corner.
                Thread.Sleep(300000);
                Thread.Sleep(300000);
                Thread.Sleep(300000);
                Thread.Sleep(10000);
                viewer.CloseBluRingViewer();
                if (login.IsTabPresent("Studies") && login.IsTabPresent("Patients") && login.IsTabPresent("Domain Management"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step11 - Click on Search button after entering different query parameters with same EA datasource
                studies.SearchStudy(AccessionNo: Accession[1], Datasource: login.GetHostName(Config.EA91));
                ele = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_studySearchResult));
                if (ele.GetAttribute("innerHTML").Trim().Equals("View 1 - 1 of 1"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step12 - Load the study in Universal Viewer
                studies.SelectStudy("Accession", Accession[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(50);
                PageLoadWait.WaitForPageLoad(50);
                ExecutedSteps++;

                //Step13 - WebAccessDeveloper log verification
                DateTime EA_localDate4 = DateTime.Now;
                String EA_format4 = "yyyyMMdd";
                string EA_date = EA_localDate4.ToString(EA_format4);
                String EA_FilePath = "C:\\Windows\\Temp" + Path.DirectorySeparatorChar + "WebAccessDeveloper-" + EA_date;

                // Read WebAccessDeveloper file
                List<String> EA_text = viewer.ReadLogFile(EA_FilePath);
                i = 1;
                count = EA_text.Count();
                bool EA_IsMessagePresent = false;
                while (i < 300 && i < count)
                {
                    if (EA_text[count - i].Trim().Contains("Connection error, retrying"))
                    {
                        EA_IsMessagePresent = true;
                        break;
                    }
                    i++;
                }
                if (EA_IsMessagePresent)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 14
                viewer.CloseBluRingViewer();
                ExecutedSteps++;
				login.Logout();

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
		/// In Merge PACS Data source: Error message shall not display in Studies tab
		/// </summary>
		public TestCaseResult Test2_160840(String testid, String teststeps, int stepcount)
		{
			//Declare and initialize variables         
			TestCaseResult result = null;
			int ExecutedSteps = -1;
			try
			{
				result = new TestCaseResult(stepcount);
				result.SetTestStepDescription(teststeps);
				String adminUserName = Config.adminUserName;
				String adminPassword = Config.adminPassword;

				String AccessionIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
				String[] Accession = AccessionIDList.Split(':');

				//Step 1 - Launch the application with a client browser 				
				login.DriverGoTo(login.url);
				ExecutedSteps++;

				//Step 2 - Login to WebAccess site with any privileged user and navigate to studies tab
				login.LoginIConnect(adminUserName, adminPassword);
                var studies = (Studies)login.Navigate("Studies");                
                if (login.IsTabPresent("Studies") && login.IsTabPresent("Patients") && login.IsTabPresent("Domain Management"))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

                //Step 3 - Select any EA Datasource in the Data source drop down box and load any study with multiple series in Universal viewer after searching
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.SanityPACS));
				studies.SelectStudy("Accession", Accession[0]);
				var viewer = BluRingViewer.LaunchBluRingViewer();
				PageLoadWait.WaitForFrameLoad(50);
				PageLoadWait.WaitForPageLoad(50);
				ExecutedSteps++;

				//Step4 - Idle for about 15 minutes and close the study by clicking "x" icon on top right corner.
				Thread.Sleep(300000);
				Thread.Sleep(300000);
				Thread.Sleep(300000);
				Thread.Sleep(10000);
				viewer.CloseBluRingViewer();
				if (login.IsTabPresent("Studies") && login.IsTabPresent("Patients") && login.IsTabPresent("Domain Management"))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

                //Step5 - Click on Search button after entering different query parameters with same EA datasource
                studies.SearchStudy(AccessionNo: Accession[1], Datasource: login.GetHostName(Config.SanityPACS));
				IWebElement ele = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_studySearchResult));
				if (ele.GetAttribute("innerHTML").Trim().Equals("View 1 - 1 of 1"))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step6 - Load the study in Universal Viewer
				studies.SelectStudy("Accession", Accession[1]);
				viewer = BluRingViewer.LaunchBluRingViewer();
				PageLoadWait.WaitForFrameLoad(50);
				PageLoadWait.WaitForPageLoad(50);
				ExecutedSteps++;

				//Step7 - WebAccessDeveloper log verification
				DateTime localDate4 = DateTime.Now;
				String format4 = "yyyyMMdd";
				string date = localDate4.ToString(format4);
				String FilePath = "C:\\Windows\\Temp" + Path.DirectorySeparatorChar + "WebAccessDeveloper-" + date;

				// Read WebAccessDeveloper file
				List<String> text = viewer.ReadLogFile(FilePath);
				int i = 1;
				int count = text.Count();
				bool IsMessagePresent = false;
				while (i < 300 && i < count)
				{
					if (text[count - i].Trim().Contains("Connection error, retrying"))
					{
						IsMessagePresent = true;
						break;
					}
					i++;
				}
				if (IsMessagePresent)
				{
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
				viewer.CloseBluRingViewer();
				login.Logout();

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
		/// Mergecom handler [MC3] re-directing messages to webaccess developer log
		/// </summary>
		public TestCaseResult Test_160841(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            DomainManagement domain = new DomainManagement();
            UserManagement usermanagement = new UserManagement();
            String ip = "10.1.1.52";
            String dataSourceName = "InvalidDS";
            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                String AccessionIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionIDList.Split(':');
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] PatientID = PatientIDList.Split(':');
                String ValidationMessageList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ValidationList");
                String[] ValidationMessage = ValidationMessageList.Split('=');            

                login.LoginIConnect(adminUserName, adminPassword);
                String DomainName = "LogDomain" + new Random().Next(10000);
                String Role = "LogRole" + new Random().Next(10000);
                String User = "LogRad" + new Random().Next(10000);
                domain.CreateDomain(DomainName, Role, datasources: null);
                domain.ClickSaveNewDomain();
                login.Navigate("UserManagement");
                usermanagement.CreateUser(User, DomainName, Role);

                //Step 1 - Launch iCA application and login as Administrator user 												
                login.LoginIConnect(User, User);
                if (login.IsTabPresent("Studies") && login.IsTabPresent("Patients"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 2 - Navigate to studies tab and load a study which contains invalid character in Universal Viewer. 
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();               
                var step2 = BluRingViewer.TotalStudyPanel() == 1; 
                viewer.CloseBluRingViewer();
                login.Logout();                
                DateTime localDate4 = DateTime.Now;
                String format4 = "yyyyMMdd";
                string date = localDate4.ToString(format4);
                String FilePath = "C:\\Windows\\Temp" + Path.DirectorySeparatorChar + "WebAccessDeveloper-" + date;

                // Read WebAccessDeveloper file
                var dirInfo = new DirectoryInfo(Path.GetDirectoryName(FilePath));
                string pattern = String.Format("{0}{1}", Path.GetFileName(FilePath), "*");
                var logList = ((from f in dirInfo.GetFiles(pattern) orderby f.LastWriteTime descending select f)).ToArray();
                //  BasePage.Kill_EXEProcess("w3wp");
                Stream stream = File.Open(logList[logList.Count() - 1].FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);               
                StringBuilder buffer = new StringBuilder();
                string line;
                using (StreamReader sr = new StreamReader(stream))
                {
                    while ((line = sr.ReadLine()) != null)
                        buffer.Append(line);
                }
                String text = buffer.ToString();                
                if (step2 && text.Contains(ValidationMessage[0]) && text.Contains(ValidationMessage[1]) && text.Contains(ValidationMessage[2]) && text.Contains(ValidationMessage[3]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                } 

                //Step3 - Add a new data source with invalid IP in service tool and restart it                
                servicetool.LaunchServiceTool();               
                wpfobject.WaitTillLoad();
                servicetool.NavigateToConfigToolDataSourceTab();
                wpfobject.WaitTillLoad();
                Thread.Sleep(1500);
                wpfobject.ClickButton("Add", 1);
                Thread.Sleep(1500);
                wpfobject.GetMainWindowByTitle(DataSource.Name.AddDataSource_Window);
                Thread.Sleep(1500);
                servicetool.SetDataSourceName(dataSourceName);
                servicetool.SetDataSourceType("2");
                servicetool.SetDataSourceDetails(dataSourceName, ip);
                servicetool.NavigateToDataSourceQueryRetrieveTab();
                servicetool.SetDataSourceQueryRetrieveAETitle(dataSourceName);
                Thread.Sleep(1500);
                servicetool.SetDataSourceQueryRetrieveHost(ip);
                Thread.Sleep(1500);
                wpfobject.SetSpinner(Spinner_ID, "12000");
                Thread.Sleep(3000);
                wpfobject.ClickButton(DataSource.ID.OkBtn);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindowByTitle(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();
                ExecutedSteps++;

                //Step4 - Login to application and navigate to edit domain management page and connect to the newly added data source.
                login.LoginIConnect(adminUserName, adminPassword);
                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain(DomainName);
                domain.SelectDomain(DomainName);
                domain.ClickEditDomain();
                domain.ConnectDataSource(dataSourceName);
                domain.ClickSaveEditDomain();
                ExecutedSteps++;
                login.Logout();

                //Step5 - Navigate to Studies tab and select the newly added data source as the only data source and search with last name as "a*"
                login.LoginIConnect(User, User);
                login.Navigate("Studies");
                studies.SearchStudy(LastName: "a*", Datasource: dataSourceName);
                IWebElement ele = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_studySearchResult));
                if (ele.GetAttribute("innerHTML").Trim().Equals("No records to view"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                Thread.Sleep(10000);

                //Step6 - From iCA server, navigate to C:\Windows\Temp\ and launch webaccessdeveloper log file in text editor
                dirInfo = new DirectoryInfo(Path.GetDirectoryName(FilePath));
                pattern = String.Format("{0}{1}", Path.GetFileName(FilePath), "*");
                logList = ((from f in dirInfo.GetFiles(pattern) orderby f.LastWriteTime descending select f)).ToArray();                
                stream = File.Open(logList[logList.Count() - 1].FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);                              
                buffer = new StringBuilder();
                string line1;
                using (StreamReader sr = new StreamReader(stream))
                {
                    while ((line1 = sr.ReadLine()) != null)
                        buffer.Append(line1);
                }
                String logfile = buffer.ToString();               
                if (logfile.Contains(ValidationMessage[4]) && logfile.Contains(ValidationMessage[5]) && logfile.Contains(ValidationMessage[6]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

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
            finally
            {
                servicetool.LaunchServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToConfigToolDataSourceTab();
                wpfobject.WaitTillLoad();
                servicetool.DeleteDataSource(0, dataSourceName);
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();
            }
        }

    }
}
