using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Reusable.Generic;
using Application = TestStack.White.Application;
using Selenium.Scripts.Pages.MergeServiceTool;
using TestStack.White.Configuration;
using TestStack.White.Factory;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.TableItems;
using TestStack.White.InputDevices;
using TestStack.White.WindowsAPI;
using static Selenium.Scripts.Pages.MergeServiceTool.ServiceTool;
using OpenQA.Selenium;
using Selenium.Scripts.Pages.HoldingPen;
using Dicom.Network;
using Dicom;
using OpenQA.Selenium.Support.UI;
using System.IO.Compression;
using OpenQA.Selenium.Interactions;
using Selenium.Scripts.Pages.eHR;

namespace Selenium.Scripts.Tests
{
	class DataMasking
	{
		public Login login { get; set; }
		public string filepath { get; set; }
		public ExamImporter ei { get; set; }
		public WpfObjects wpfobject { get; set; }
		public ServiceTool servicetool { get; set; }
		public UserPreferences userpref { get; set; }
		public Studies studies { get; set; }
		public BasePage basePage { get; set; }
		public BluRingViewer bluringviewer { get; set; }
		public Random randomnumber { get; set; }
		public EHR ehr { get; set; }
		public string EA12Url;
		public string IntegratedFrameName;
		public string patientNameWithoutDeidentificationValue = "New_Patient";

		/// <summary>
		/// Constructor - Test Suite
		/// </summary>
		public DataMasking(String classname)
		{
			login = new Login();
			BasePage.InitializeControlIdMap();
			login.DriverGoTo(login.url);
			filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
			wpfobject = new WpfObjects();
			servicetool = new ServiceTool();
			userpref = new UserPreferences();
			bluringviewer = new BluRingViewer();
			studies = new Studies();
			basePage = new BasePage();
			randomnumber = new Random();
			ehr = new EHR();
			Config.EA7 = "10.9.37.61";
			Config.EA7AETitle = "ECM_ARC_61";
			Config.vna61 = "VNA61";
			EA12Url = "https://" + Config.EA7 + "/eaweb";
			IntegratedFrameName = "IntegratorHomeFrame";
			
		}
		/// <summary>
		/// To set preconditions for Data Masking VP
		/// </summary>
		/// <returns></returns>
		public TestCaseResult DataMaskingPrecondition(String testid, String teststeps, int stepcount)
		{
			TestCaseResult result = new TestCaseResult(stepcount);
			result.SetTestStepDescription(teststeps);
			int ExecutedSteps = -1;

			try
			{
				String Username = Config.adminUserName;
				String Password = Config.adminPassword;

				servicetool.LaunchServiceTool();
				wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
				servicetool.EnableDeidentificationInDataSource(login.GetHostName(Config.EA91));
				servicetool.AddEADatasource(Config.EA7, Config.EA7AETitle, EnableDeidentification: true);
				servicetool.AddVNADatasource(Config.EA7, Config.EA7AETitle, dataSourceName: Config.vna61, EnableDeidentification: true);
				
				//Restart the service
				wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
				wpfobject.WaitTillLoad();
				servicetool.RestartIISandWindowsServices();
				wpfobject.WaitTillLoad();
				servicetool.CloseConfigTool();

				login.DriverGoTo(login.url);
				login.LoginIConnect(Username, Password);

				//Navigate to DomainManagement Tab
				var domainManagement = (DomainManagement)login.Navigate("DomainManagement");
				domainManagement.SelectDomain(Config.adminGroupName);

				//Click Edit in DomainManagement Tab
				domainManagement.ClickEditDomain();
				domainManagement.ConnectAllDatasourcesEditDomain();
				domainManagement.ClickSaveEditDomain();			
				ExecutedSteps++;

				//Return result
				result.FinalResult(ExecutedSteps);
				Logger.Instance.InfoLog("Overall Test status--" + result.status);

				return result;
			}
			catch (Exception e)
			{
				//Log Exception
				Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);


				result.FinalResult(e, ExecutedSteps);
				Logger.Instance.ErrorLog("Overall Test status--" + result.status);

				return result;
			}
		}

		/// <summary>
		/// Option to enable/disable data masking in Service tool
		/// </summary>
		/// <returns></returns>
		public TestCaseResult Test_160087(String testid, String teststeps, int stepcount)
		{
			TestCaseResult result = new TestCaseResult(stepcount);
			result.SetTestStepDescription(teststeps);
			int ExecutedSteps = -1;

			try
			{
				var DataSourceName = "DeindentificationDS" + new Random().Next(1, 1000);
				//Step1: login to iCA 7.0 Server 
				ExecutedSteps++;

				//Step2: Open service tool and navigate to Data source tab
				servicetool.LaunchServiceTool();
				wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
				servicetool.NavigateToConfigToolDataSourceTab();
				ExecutedSteps++;

				//Step3: Click on the ADD button to open the new datasource screen and select dicom from the TYPE dropdown.
				WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Add")).Click();
				Thread.Sleep(1500);
				wpfobject.GetMainWindowByTitle(DataSource.Name.AddDataSource_Window);
				//Thread.Sleep(1500);
				servicetool.SetDataSourceType("2");
				ExecutedSteps++;

				//Step4: Check Enable data masking checkbox generic tab and check the checkbox
				if (wpfobject.VerifyElement(DataSource.ID.SupportDeindentification, "Supports Data Masking"))
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				if (!wpfobject.IsCheckBoxSelected(DataSource.ID.SupportDeindentification))
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				//Result for Step-4
				if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].status = "Fail";
				}
				else
				{
					result.steps[ExecutedSteps].status = "Pass";
				}

				//Step5: Select Enable data masking and apply changes
				wpfobject.SelectCheckBox(DataSource.ID.SupportDeindentification);
				if (wpfobject.IsCheckBoxSelected(DataSource.ID.SupportDeindentification))
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
				
				//Step6: Click Apply and Select Ok button. Restart services
				servicetool.SetDataSourceName(DataSourceName);
				wpfobject.ClickButton(DataSource.ID.OkBtn);
				bool isDataSourceExists = servicetool.CheckDataSourceExists(DataSourceName);
				servicetool.RestartIISandWindowsServices();				
				if (isDataSourceExists)
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

				//Step7: Open DataSourceManagerConfiguration.xml and chcek for <ea.deidentification.supported>true</ea.deidentification.supported>
				var supportDeidentificationValue = ReadXML.ReadDataXML(Config.DSManagerFilePath, "dataSources/add[@id='" + DataSourceName + "']/parameters");
				if(supportDeidentificationValue["ea.deidentification.supported"] == "true")
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

				//Step8: Edit previously added Data source
				servicetool.NavigateToConfigToolDataSourceTab();
				servicetool.SelectDataSource(DataSourceName);
				wpfobject.GetUIItem<ITabPage, Button>(servicetool.GetCurrentTabItem(), "Details", 1, "0").Click();
				wpfobject.WaitForPopUp();
				wpfobject.GetMainWindowByTitle(ServiceTool.DataSource.Name.EditDataSource_Window);
				ExecutedSteps++;

				//Step9: uncheck the Support Deidentification checkbox, Apply changes ans Restart service
				wpfobject.UnSelectCheckBox(DataSource.ID.SupportDeindentification);
				if (!wpfobject.IsCheckBoxSelected(DataSource.ID.SupportDeindentification))
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
				wpfobject.ClickButton(DataSource.ID.OkBtn);
				wpfobject.WaitTillLoad();
				servicetool.RestartIISandWindowsServices();

				//Step10: Open DataSourceManagerConfiguration.xml and chcek for <ea.deidentification.supported>true</ea.deidentification.supported>
				servicetool.CloseServiceTool();
				var newSupportDeidentificationValue = ReadXML.ReadDataXML(Config.DSManagerFilePath, "dataSources/add[@id='" + DataSourceName + "']/parameters");
				if (!newSupportDeidentificationValue.ContainsKey("ea.deidentification.supported"))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("<ea.deidentification.supported> not available for data source: " + DataSourceName);
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					if (newSupportDeidentificationValue["ea.deidentification.supported"] == "false")
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
				}
				
				//Return result
				result.FinalResult(ExecutedSteps);
				Logger.Instance.InfoLog("Overall Test status--" + result.status);

				return result;
			}
			catch (Exception e)
			{
				//Log Exception
				Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);


				result.FinalResult(e, ExecutedSteps);
				Logger.Instance.ErrorLog("Overall Test status--" + result.status);

				return result;
			}

		}
		/// <summary>
		///  Data Mask Exams checkbox in Transfer window
		/// </summary>
		/// <returns></returns>
		public TestCaseResult Test_160088(String testid, String teststeps, int stepcount)
		{
			TestCaseResult result = new TestCaseResult(stepcount);
			result.SetTestStepDescription(teststeps);
			int ExecutedSteps = -1;
			String[] accList= null;
			try
			{
				String adminUserName = Config.adminUserName;
				String adminPassword = Config.adminPassword;
				String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
				accList = accession.Split(':');
				var studypaths = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
				String[] studypath = studypaths.Split('=');

				//Precondition - Send studies to EA12
				DicomClient client = new DicomClient();
				string filename = string.Empty;
				string[] SOPID = Enumerable.Repeat("1.2.840.113837.390681903." + System.DateTime.Now.ToString("MMddHHmmss"), 1).ToArray();
				string Name = login.RandomString(4, true) + new System.DateTime().Second + randomnumber.Next(1, 100);
				string PID = "PID" + System.DateTime.Now.ToString("ddHHmmss");
				string ACC = "ACC" + System.DateTime.Now.ToString("ddHHmmss");
				string SID = "1.2.840.113837.1845205803." + System.DateTime.Now.ToString("MMddHHmmss") + "." + Config.IConnectIP.Split('.')[3];
				string SED = "1.2.840.113837.4288965276." + System.DateTime.Now.ToString("MMddHHmmss") + "." + Config.IConnectIP.Split('.')[3];
				SOPID[0] = SOPID[0] + ".0" + 1;
				filename = BasePage.WriteDicomFile(studypath[0], new DicomTag[] { DicomTag.SOPInstanceUID, DicomTag.PatientName, DicomTag.PatientID, DicomTag.AccessionNumber, DicomTag.StudyInstanceUID, DicomTag.SeriesInstanceUID, DicomTag.InstanceNumber }, new string[] { SOPID[0], Name, PID, ACC, SID, SED, Convert.ToString(0) });
				client.AddRequest(new DicomCStoreRequest(filename));
				client.Send(Config.EA7, 12000, false, "SCU", Config.EA7AETitle);

				//Step1: Login to iCA 7.0 as privileged user (Administrator)
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminUserName, adminPassword);
				ExecutedSteps++;
				
				//Step2: Navigate to study tab and search a study from data masking enabled data source 
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(AccessionNo: ACC, Datasource: login.GetHostName(Config.EA7));
				if (studies.CheckStudy("Accession", ACC))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step3: Select a study and click "Transfer" button
				studies.SelectStudy("Accession", ACC);
				studies.TransferBtn().Click();
				PageLoadWait.WaitForFrameLoad(10);
				BasePage.Driver.SwitchTo().DefaultContent();
				//studies.SwitchTo("index", "0");
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");

				if (basePage.IsElementVisible(basePage.By_DataMaskCheckbox()) && (!basePage.DataMaskExamCheckbox().Selected))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
					if (!basePage.IsElementVisible(basePage.By_DataMaskCheckbox()))
					{
						throw new Exception("Data Mask checkbox not displayed");
					}
				}

				//Step4: Select 'Local System' from Transfer to dropdown
				studies.Dropdown_TransferTo().SelectByText("Local System");
				if (basePage.IsElementVisible(basePage.By_DataMaskCheckbox()) && (!basePage.DataMaskExamCheckbox().Selected))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
					if (!basePage.IsElementVisible(basePage.By_DataMaskCheckbox()))
					{
						throw new Exception("Data Mask checkbox not displayed");
					}
				}

				//Step5: Data Masking should not display if any non data masking data dource is selected
				studies.Dropdown_TransferTo().SelectByText(login.GetHostName(Config.EA96));
				if (!basePage.IsElementVisible(basePage.By_DataMaskCheckbox()))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step6: close transfer window
				studies.CancelBtn().Click();
				if (!basePage.IsElementVisible(basePage.By_Dropdown_TrWindow()))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step7: Transfer a study from Non data masking data source
				studies.SearchStudy(AccessionNo: accList[0], Datasource: login.GetHostName(Config.EA1));
				studies.SelectStudy("Accession", accList[0]);
				studies.TransferBtn().Click();
				PageLoadWait.WaitForFrameLoad(10);
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				if (basePage.IsElementVisible(basePage.By_Dropdown_TrWindow()) && !basePage.IsElementVisible(basePage.By_DataMaskCheckbox()))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step8: Select 'Local System' from Transfer to dropdown
				studies.Dropdown_TransferTo().SelectByText("Local System");
				if (!basePage.IsElementVisible(basePage.By_DataMaskCheckbox()))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step9: Data Masking should not display if any non data masking data dource is selected
				studies.Dropdown_TransferTo().SelectByText(login.GetHostName(Config.EA7));
				if (!basePage.IsElementVisible(basePage.By_DataMaskCheckbox()))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step10: close transfer window
				studies.CancelBtn().Click();
				if (!basePage.IsElementVisible(basePage.By_Dropdown_TrWindow()))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				login.Logout();

				//Return result
				result.FinalResult(ExecutedSteps);
				Logger.Instance.InfoLog("Overall Test status--" + result.status);

				return result;
			}
			catch (Exception e)
			{
				//Log Exception
				Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
				result.FinalResult(e, ExecutedSteps);
				Logger.Instance.ErrorLog("Overall Test status--" + result.status);
				return result;
			}
			//finally
			//{
			//	//Deleting uploaded study
			//	var hplogin = new HPLogin();
			//	BasePage.Driver.Navigate().GoToUrl("https://" + Config.EA7 + "/webadmin");
			//	var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
			//	var workflow = (WorkFlow)hphome.Navigate("Workflow");
			//	workflow.NavigateToLink("Workflow", "Archive Search");
			//	workflow.HPSearchStudy("Accession", accList[0]);
			//	workflow.HPDeleteStudy();
			//	hplogin.LogoutHPen();
			//}

		}

		/// <summary>
		///  Priors (related studies) on same datasource with data masking enabled
		/// </summary>
		/// <returns></returns>
		public TestCaseResult Test_160089(String testid, String teststeps, int stepcount)
		{
			TestCaseResult result = new TestCaseResult(stepcount);
			result.SetTestStepDescription(teststeps);
			int ExecutedSteps = -1;
			Random randomnumber = new Random();
			WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 300));
			string PID = null;
			try
			{
				String adminUserName = Config.adminUserName;
				String adminPassword = Config.adminPassword;
				var studypaths = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
				String[] studypath = studypaths.Split('=');
				
				//Precondition - Send studies to EA12
				string Name = login.RandomString(4, true) + new System.DateTime().Second + randomnumber.Next(1, 100);
				PID = "PID" + System.DateTime.Now.ToString("ddHHmmss");
				List<string> ACC = new List<string>(); ;
				string filename = string.Empty;
				string[] SOPID = Enumerable.Repeat("1.2.840.113837.390681903." + System.DateTime.Now.ToString("MMddHHmmss"), studypath.Length).ToArray();
				for (int count = 0; count < studypath.Length; count++)
				{
					DicomClient client = new DicomClient();
					ACC.Add("ACC" + System.DateTime.Now.ToString("ddHHmmss") + count);
					string SID = "1.2.840.113837.1845205803." + System.DateTime.Now.ToString("MMddHHmmss") + "." + Config.IConnectIP.Split('.')[3];
					string SED = "1.2.840.113837.4288965276." + System.DateTime.Now.ToString("MMddHHmmss") + "." + Config.IConnectIP.Split('.')[3];
					SOPID[count] = SOPID[count] + ".0" + (count + 1);
					filename = BasePage.WriteDicomFile(studypath[count], new DicomTag[] { DicomTag.SOPInstanceUID, DicomTag.PatientName, DicomTag.PatientID, DicomTag.AccessionNumber, DicomTag.StudyInstanceUID, DicomTag.SeriesInstanceUID, DicomTag.InstanceNumber }, new string[] { SOPID[count], Name, PID, ACC[count], SID, SED, Convert.ToString(count + 1) });
					client.AddRequest(new DicomCStoreRequest(filename));
					client.Send(Config.EA7, 12000, false, "SCU", Config.EA7AETitle);
					Thread.Sleep(2000);//Study sending to EA
				}

				//Step1: Login to iCA 7.0 as privileged user (Administrator)
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminUserName, adminPassword);
				ExecutedSteps++;

				//Step2: Navigate to study tab and search a study from data masking enabled data source 
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(patientID: PID, Datasource: login.GetHostName(Config.EA7));
				if (studies.CheckStudy("Accession", ACC[0]))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step3: Select a study and click "Transfer" button
				studies.SelectStudy("Accession", ACC[0]);
				studies.TransferBtn().Click();
				PageLoadWait.WaitForFrameLoad(10);
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				if (basePage.IsElementVisible(basePage.By_DataMaskCheckbox()) && (!basePage.DataMaskExamCheckbox().Selected))
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("Data Mask Exam checkbox visible and unchecked");
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("Data Mask Exam is invisible or checked");
					result.steps[ExecutedSteps].SetLogs();
					if (!basePage.IsElementVisible(basePage.By_DataMaskCheckbox()))
					{
						throw new Exception("Data Mask checkbox not displayed");
					}
				}
				//try { wait.Until(ExpectedConditions.ElementIsVisible(studies.By_RelatedStudy(2))); }
				//catch (Exception) { }
				PageLoadWait.WaitForSearchPriorStudiesMessage();
				if (basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[1])) &&
					basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[2])) &&
					basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[3])) &&
					basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[4])))
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("All prior studies are listed");
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("All prior studies not listed");
					result.steps[ExecutedSteps].SetLogs();
				}
				//Result for Step-3
				if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].status = "Fail";
				}
				else
				{
					result.steps[ExecutedSteps].status = "Pass";
				}

				//Step4: Select 'Local System' from Transfer to dropdown
				studies.Dropdown_TransferTo().SelectByText("Local System");
				studies.SelectAllButton().Click();
				if (basePage.IsElementVisible(basePage.By_DataMaskCheckbox()) && (!basePage.DataMaskExamCheckbox().Selected))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
					if (!basePage.IsElementVisible(basePage.By_DataMaskCheckbox()))
					{
						throw new Exception("Data Mask checkbox not displayed");
					}
				}


				//Step5: Data Masking should not display if any non data masking data dource is selected
				studies.Dropdown_TransferTo().SelectByText(login.GetHostName(Config.EA96));
				if (!basePage.IsElementVisible(basePage.By_DataMaskCheckbox()))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step6: close transfer window
				studies.CancelBtn().Click();
				if (!basePage.IsElementVisible(basePage.By_Dropdown_TrWindow()))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step7_1:  Select a study that has priors (all studies from DS1- Datasource with data masking enabled)
				studies.SelectStudy("Accession", ACC[0]);
				studies.TransferBtn().Click();
				PageLoadWait.WaitForFrameLoad(10);
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				studies.Dropdown_TransferTo().SelectByText("Local System");
				if (basePage.IsElementVisible(basePage.By_Dropdown_TrWindow()) && basePage.IsElementVisible(basePage.By_DataMaskCheckbox()))
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("Transfer window displayed and Data masking check box is invisible");
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				//try { wait.Until(ExpectedConditions.ElementIsVisible(studies.By_RelatedStudy(2))); }
				//catch (Exception) { }
				PageLoadWait.WaitForSearchPriorStudiesMessage();
				if (basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[1])) &&
					basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[2])) &&
					basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[3])) &&
					basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[4])))
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed-- Prior Studies not loaded in Transfer window");
					result.steps[ExecutedSteps].SetLogs();
				}
				//Step7_2: Check the "Data Mask Exam" checkbox and select transfer
				((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", basePage.DataMaskExamCheckbox());
				((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", studies.Btn_StudyPageTransferBtn());
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				var copyCheckBox = BasePage.FindElementByCss(BasePage.DataMaskCopyCheckbox);
				if (copyCheckBox.Selected && !copyCheckBox.Enabled)
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed-- Prior Studies not loaded in Transfer window");
					result.steps[ExecutedSteps].SetLogs();
				}
				//Result for Step-7
				if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].status = "Fail";
				}
				else
				{
					result.steps[ExecutedSteps].status = "Pass";
				}

				//Step8: Check the Data Masking setting window fields
				bool res1 = BasePage.FindElementByCss(BasePage.DataMaskFirstName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res2 = BasePage.FindElementByCss(BasePage.DataMaskMiddleName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res3 = BasePage.FindElementByCss(BasePage.DataMaskLastName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res4 = BasePage.FindElementByCss(BasePage.DataMaskPrefixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
				bool res5 = BasePage.FindElementByCss(BasePage.DataMaskSuffixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
				bool res6 = BasePage.FindElementByCss(BasePage.DataMaskPatientID).GetAttribute("placeholder").Equals("(auto generated)");
				bool res7 = BasePage.FindElementByCss(BasePage.DataMaskIssuerPatientID).GetAttribute("placeholder").Equals("(removed)");
				bool res8 = new SelectElement(BasePage.Driver.FindElement(By.CssSelector(BasePage.DataMaskGender))).SelectedOption.Text.Equals("Male");
				//  bool res9 = BasePage.FindElementByCss(BasePage.DataMaskDOB).GetAttribute("placeholder").Equals("(auto generated)");
				bool res10 = BasePage.FindElementByCss(BasePage.DataMaskAccessionNo).GetAttribute("placeholder").Equals("(auto generated)");
				bool res11 = BasePage.FindElementByCss(BasePage.DataMaskStudyDescription).GetAttribute("placeholder").Equals("(removed)");
				//  bool res12 = BasePage.FindElementByCss(BasePage.DataMaskStudyDate).GetAttribute("placeholder").Equals("(auto generated)");
				if (res1 && res2 && res3 && res4 && res5 && res6 && res7 && res8 && res10 & res11)
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step9: Do not change any value in Data masking setting and click on Submit button
				((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", BasePage.FindElementByCss(BasePage.DataMaskCancel));
				studies.SelectStudy("Accession", ACC[0]);
				studies.TransferStudy("Local System", SelectallPriors: false, PatientTab: true, isDataMasking: true);
				ExecutedSteps++;

				//Step10_1: Click Download button when the status updated to "Ready"
				PageLoadWait.WaitForDownload(patientNameWithoutDeidentificationValue, Config.downloadpath, "zip");
				if (BasePage.CheckFile(patientNameWithoutDeidentificationValue, Config.downloadpath, "zip"))
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("Study Transferred Successfully");
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step10_2: New study instance should also be stored on EA
				var dicomPath = BasePage.ExtractZipFiles(FileName: patientNameWithoutDeidentificationValue).Find(x => x.Contains("F00"));
				String ExtractedFilePID = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.PatientID);
				var hplogin = new HPLogin();
				basePage.DriverGoTo("https://" + Config.EA7 + "/eaweb");
				var hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
				var workflow = (WorkFlow)hphome.Navigate("Workflow");
				if (workflow.EAv12SearchStudy("PatientID", ExtractedFilePID))
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("Study Transferred Successfully");
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				hplogin.LogoutEAv12();
				BasePage.DeleteAllFileFolder(Config.downloadpath);
				
				//Result for Step-10
				if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].status = "Fail";
				}
				else
				{
					result.steps[ExecutedSteps].status = "Pass";
				}

				//Step11_1: Click on the same study and select Transfer and select data mask exam chcekbox
				//Step11_2: Update values for Patient information and Study Information and Submit
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminUserName, adminPassword);
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(patientID: PID, Datasource: login.GetHostName(Config.EA7));
				var newDataMaskValues = basePage.GetDataMaskFieldsNames();
				newDataMaskValues["LastName"] = login.RandomString(5, true);
				newDataMaskValues["PatientID"] = "PID" + System.DateTime.Now.ToString("ddHHmmss");
				newDataMaskValues["Accession"] = "ACC" + System.DateTime.Now.ToString("ddHHmmss");
				studies.SelectStudy("Accession", ACC[1]);
				basePage.TransferStudy("Local System", waittime:180, SelectallPriors: false, PatientTab: true, isDataMasking: true, DataMaskValues: newDataMaskValues);
				ExecutedSteps++;

				//Step12_1: Click Download button when the status updated to "Ready"
				PageLoadWait.WaitForDownload("_" + newDataMaskValues["LastName"], Config.downloadpath, "zip");
				if (BasePage.CheckFile("_" + newDataMaskValues["LastName"], Config.downloadpath, "zip"))
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("Study Transferred Successfully");
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step12_2: New study instance should also be stored on EA
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(patientID: newDataMaskValues["PatientID"], Datasource: login.GetHostName(Config.EA7));
				if (studies.CheckStudy("Accession", newDataMaskValues["Accession"]))
				{
					result.steps[ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				login.Logout();

				//Result for Step-12
				if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].status = "Fail";
				}
				else
				{
					result.steps[ExecutedSteps].status = "Pass";
				}

				//Step13: Unzip file and check the DICOM file values for Patient Information and Study Information
				dicomPath = BasePage.ExtractZipFiles(FileName: "_" + newDataMaskValues["LastName"]).Find(x => x.Contains("F00"));
				ExtractedFilePID = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.PatientID);
				String ExtractedFileACC = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.AccessionNumber);
				if (ExtractedFilePID == newDataMaskValues["PatientID"] && ExtractedFileACC == newDataMaskValues["Accession"])
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step14: Search for this study instance on iCA
				hplogin = new HPLogin();
				basePage.DriverGoTo("https://" + Config.EA7 + "/eaweb");
				hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
				workflow = (WorkFlow)hphome.Navigate("Workflow");
				if(workflow.EAv12SearchStudy("Accession", newDataMaskValues["Accession"]))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				hplogin.LogoutEAv12();

				//Return result
				result.FinalResult(ExecutedSteps);
				Logger.Instance.InfoLog("Overall Test status--" + result.status);
				
				return result;
			}
			catch (Exception e)
			{
				//Log Exception
				Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
				result.FinalResult(e, ExecutedSteps);
				Logger.Instance.ErrorLog("Overall Test status--" + result.status);
				return result;
			}
			finally
			{
				BasePage.DeleteAllFileFolder(Config.downloadpath);
				//Deleting uploaded study
				var hplogin = new HPLogin();
				BasePage.Driver.Navigate().GoToUrl("https://" + Config.EA7 + "/eaweb");
				var hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
				var workflow = (WorkFlow)hphome.Navigate("Workflow");
				workflow.EAv12SearchStudy("PatientID", PID, SelectStudy: true);
				workflow.EAv12DeleteStudy();
				hplogin.LogoutEAv12();
			}

		}

		/// <summary>
		///  Priors (related studies) on same datasource WITHOUT data masking
		/// </summary>
		/// <returns></returns>
		public TestCaseResult Test_160091(String testid, String teststeps, int stepcount)
		{
			TestCaseResult result = new TestCaseResult(stepcount);
			result.SetTestStepDescription(teststeps);
			int ExecutedSteps = -1;
			WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 300));
			try
			{
				String adminUserName = Config.adminUserName;
				String adminPassword = Config.adminPassword;
				String patientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
				String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
				var accList = accession.Split(':');
				
				//Step1: Login to iCA 7.0 as privileged user (Administrator)
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminUserName, adminPassword);
				ExecutedSteps++;

				//Step2: Navigate to study tab and search a study from data masking enabled data source 
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(patientID: patientID, Datasource: login.GetHostName(Config.EA1));
				ExecutedSteps++;

				//Step3: Select a study that has priors from Datasource WITHOUT data masking  and click Transfer
				studies.SelectStudy("Accession", accList[0]);
				studies.TransferBtn().Click();
				PageLoadWait.WaitForFrameLoad(10);
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				if (basePage.IsElementVisible(basePage.By_Dropdown_TrWindow()) && !basePage.IsElementVisible(basePage.By_DataMaskCheckbox()))
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("Transfer window displayed and Data masking check box is invisible");
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				//try { wait.Until(ExpectedConditions.ElementIsVisible(studies.By_RelatedStudy(2))); }
				//catch (Exception) { }
				PageLoadWait.WaitForSearchPriorStudiesMessage();
				if (BasePage.Driver.FindElement(studies.By_RelatedStudy(2)).Text.Equals(accList[1]) &&
					BasePage.Driver.FindElement(studies.By_RelatedStudy(3)).Text.Equals(accList[2]) &&
					BasePage.Driver.FindElement(studies.By_RelatedStudy(4)).Text.Equals(accList[3]))
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed-- Prior Studies not loaded in Transfer window");
					result.steps[ExecutedSteps].SetLogs();
				}
				//Result for Step-3
				if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].status = "Fail";
				}
				else
				{
					result.steps[ExecutedSteps].status = "Pass";
				}

				//Step4: Select 'Local System' from Transfer to dropdown
				studies.Dropdown_TransferTo().SelectByText("Local System");
				studies.SelectAllButton().Click();
				if (!basePage.IsElementVisible(basePage.By_DataMaskCheckbox()))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step5: Select any other value from dropdown (other then Local System)
				studies.Dropdown_TransferTo().SelectByText(login.GetHostName(Config.EA7));
				if (!basePage.IsElementVisible(basePage.By_DataMaskCheckbox()))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				studies.CancelBtn().Click();
				login.Logout();

				//Return result
				result.FinalResult(ExecutedSteps);
				Logger.Instance.InfoLog("Overall Test status--" + result.status);

				return result;
			}
			catch (Exception e)
			{
				//Log Exception
				Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
				result.FinalResult(e, ExecutedSteps);
				Logger.Instance.ErrorLog("Overall Test status--" + result.status);
				return result;
			}
		}

		/// <summary>
		///  Priors (related studies) from different datasources
		/// </summary>
		/// <returns></returns>
		public TestCaseResult Test_160092(String testid, String teststeps, int stepcount)
		{
			TestCaseResult result = new TestCaseResult(stepcount);
			result.SetTestStepDescription(teststeps);
			int ExecutedSteps = -1;
			WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 300));
			try
			{
				String adminUserName = Config.adminUserName;
				String adminPassword = Config.adminPassword;
				String patientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
				var studypaths = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
				String[] studypath = studypaths.Split('=');

				//Precondition - Send studies to EA12
				string Name = login.RandomString(4, true) + new System.DateTime().Second + randomnumber.Next(1, 100);
				string PID = "PID" + System.DateTime.Now.ToString("ddHHmmss");
				List<string> ACC = new List<string>(); ;
				string filename = string.Empty;
				string[] SOPID = Enumerable.Repeat("1.2.840.113837.390681903." + System.DateTime.Now.ToString("MMddHHmmss"), studypath.Length).ToArray();
				string host = Config.EA7, aeTitle = Config.EA7AETitle;
				for (int count = 0; count < studypath.Length; count++)
				{
					DicomClient client = new DicomClient();
					ACC.Add("ACC" + System.DateTime.Now.ToString("ddHHmmss") + count);
					string SID = "1.2.840.113837.1845205803." + System.DateTime.Now.ToString("MMddHHmmss") + "." + Config.IConnectIP.Split('.')[3];
					string SED = "1.2.840.113837.4288965276." + System.DateTime.Now.ToString("MMddHHmmss") + "." + Config.IConnectIP.Split('.')[3];
					SOPID[count] = SOPID[count] + ".0" + (count + 1);
					filename = BasePage.WriteDicomFile(studypath[count], new DicomTag[] { DicomTag.SOPInstanceUID, DicomTag.PatientName, DicomTag.PatientID, DicomTag.AccessionNumber, DicomTag.StudyInstanceUID, DicomTag.SeriesInstanceUID, DicomTag.InstanceNumber }, new string[] { SOPID[count], Name, PID, ACC[count], SID, SED, Convert.ToString(count + 1) });
					client.AddRequest(new DicomCStoreRequest(filename));
					client.Send(host, 12000, false, "SCU", aeTitle);
					if (count == 1)
					{
						host = Config.EA96; aeTitle = Config.EA96AETitle;
					}
					else if (count == 3)
					{
						host = Config.EA7; aeTitle = Config.EA7AETitle;
					}
					Thread.Sleep(2000); // Sending study to EA
				}

				//Step1: Login to iCA 7.0 as privileged user (Administrator)
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminUserName, adminPassword);
				ExecutedSteps++;

				//Step2: Navigate to study tab and search a study from data masking enabled data source 
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(patientID: PID);
				ExecutedSteps++;

				//Step3: Select a study that has priors from Datasource WITHOUT data masking  and click Transfer
				studies.SelectStudy("Accession", ACC[0]);
				studies.TransferBtn().Click();
				PageLoadWait.WaitForFrameLoad(10);
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				if (basePage.IsElementVisible(basePage.By_Dropdown_TrWindow()) && basePage.IsElementVisible(basePage.By_DataMaskCheckbox()))
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("Transfer window displayed and Data masking check box is visible");
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				//try { wait.Until(ExpectedConditions.ElementIsVisible(studies.By_RelatedStudy(2))); }
				//catch (Exception) { }
				PageLoadWait.WaitForSearchPriorStudiesMessage();
				if (basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[1])) &&
					basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[2])) &&
					basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[3])) &&
					basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[4])))
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed-- Prior Studies not loaded in Transfer window");
					result.steps[ExecutedSteps].SetLogs();
				}
				//Result for Step-3
				if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].status = "Fail";
				}
				else
				{
					result.steps[ExecutedSteps].status = "Pass";
				}
				
				//Step4: Check the "Data Mask Exam" checkbox and Click Transfer button
				((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", basePage.DataMaskExamCheckbox());
				PageLoadWait.WaitForSearchPriorStudiesMessage();
				//try { wait.Until(ExpectedConditions.ElementIsVisible(studies.By_RelatedStudy(2))); }
				//catch (Exception) { }
				if (basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[1])) &&
					basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[4])) &&
					!basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[2])) &&
					!basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[3])))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step5: Uncheck the "Data Mask Exam" checkbox and Click Transfer button
				((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", basePage.DataMaskExamCheckbox());
				if (basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[1])) &&
					basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[2])) &&
					basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[3])) &&
					basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[4])))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step6: Check Data masking setting window and Check the fields
				((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", basePage.DataMaskExamCheckbox());
				((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", studies.Btn_StudyPageTransferBtn());
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				bool res1 = BasePage.FindElementByCss(BasePage.DataMaskFirstName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res2 = BasePage.FindElementByCss(BasePage.DataMaskMiddleName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res3 = BasePage.FindElementByCss(BasePage.DataMaskLastName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res4 = BasePage.FindElementByCss(BasePage.DataMaskPrefixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
				bool res5 = BasePage.FindElementByCss(BasePage.DataMaskSuffixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
				bool res6 = BasePage.FindElementByCss(BasePage.DataMaskPatientID).GetAttribute("placeholder").Equals("(auto generated)");
				bool res7 = BasePage.FindElementByCss(BasePage.DataMaskIssuerPatientID).GetAttribute("placeholder").Equals("(removed)");
				bool  res8 = new SelectElement(BasePage.Driver.FindElement(By.CssSelector(BasePage.DataMaskGender))).SelectedOption.Text.Equals("Male");
				//  bool res9 = BasePage.FindElementByCss(BasePage.DataMaskDOB).GetAttribute("placeholder").Equals("(auto generated)");
				bool res10 = BasePage.FindElementByCss(BasePage.DataMaskAccessionNo).GetAttribute("placeholder").Equals("(auto generated)");
				bool res11 = BasePage.FindElementByCss(BasePage.DataMaskStudyDescription).GetAttribute("placeholder").Equals("(removed)");
				//  bool res12 = BasePage.FindElementByCss(BasePage.DataMaskStudyDate).GetAttribute("placeholder").Equals("(auto generated)");
				var copyCheckBox = BasePage.FindElementByCss(BasePage.DataMaskCopyCheckbox);
				bool res13 = copyCheckBox.Selected && !copyCheckBox.Enabled;						
				if (res1 && res2 && res3 && res4 && res5 && res6 && res7 && res8 && res10 & res11 && res13)
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", BasePage.FindElementByCss(BasePage.DataMaskCancel));
				login.Logout();

				//Return result
				result.FinalResult(ExecutedSteps);
				Logger.Instance.InfoLog("Overall Test status--" + result.status);

				return result;
			}
			catch (Exception e)
			{
				//Log Exception
				Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
				result.FinalResult(e, ExecutedSteps);
				Logger.Instance.ErrorLog("Overall Test status--" + result.status);
				return result;
			}
		}

		/// <summary>
		///  Data Masking Settings window
		/// </summary>
		/// <returns></returns>
		public TestCaseResult Test_160561(String testid, String teststeps, int stepcount)
		{
			TestCaseResult result = new TestCaseResult(stepcount);
			result.SetTestStepDescription(teststeps);
			int ExecutedSteps = -1;
			WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 300));
			try
			{
				String adminUserName = Config.adminUserName;
				String adminPassword = Config.adminPassword;
				String patientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
				var studypaths = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
				String[] studypath = studypaths.Split('=');

				//Precondition - Send studies to EA12
				DicomClient client = new DicomClient();
				string filename = string.Empty;
				string[] SOPID = Enumerable.Repeat("1.2.840.113837.390681903." + System.DateTime.Now.ToString("MMddHHmmss"), 1).ToArray();
				string Name = login.RandomString(4, true) + new System.DateTime().Second + randomnumber.Next(1, 100);
				string PID = "PID" + System.DateTime.Now.ToString("ddHHmmss");
				string ACC = "ACC" + System.DateTime.Now.ToString("ddHHmmss");
				string SID = "1.2.840.113837.1845205803." + System.DateTime.Now.ToString("MMddHHmmss") + "." + Config.IConnectIP.Split('.')[3];
				string SED = "1.2.840.113837.4288965276." + System.DateTime.Now.ToString("MMddHHmmss") + "." + Config.IConnectIP.Split('.')[3];
				SOPID[0] = SOPID[0] + ".0" + 1;
				filename = BasePage.WriteDicomFile(studypath[0], new DicomTag[] { DicomTag.SOPInstanceUID, DicomTag.PatientName, DicomTag.PatientID, DicomTag.AccessionNumber, DicomTag.StudyInstanceUID, DicomTag.SeriesInstanceUID, DicomTag.InstanceNumber }, new string[] { SOPID[0], Name, PID, ACC, SID, SED, Convert.ToString(0) });
				client.AddRequest(new DicomCStoreRequest(filename));
				client.Send(Config.EA7, 12000, false, "SCU", Config.EA7AETitle);

				//Step1: Login to iCA 7.0 as privileged user (Administrator)
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminUserName, adminPassword);
				ExecutedSteps++;

				//Step2: Navigate to study tab and search a study from data masking enabled data source 
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(patientID: PID);
				ExecutedSteps++;

				//Step3: Select a study and click "Transfer" button and check Checkbox "Data Mask Exams" is visible and unchecked.
				studies.SelectStudy("Accession", ACC);
				studies.TransferBtn().Click();
				PageLoadWait.WaitForFrameLoad(10);
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				var isDataMaskSelected = true;
				if (basePage.IsElementVisible(basePage.By_Dropdown_TrWindow()) && basePage.IsElementVisible(basePage.By_DataMaskCheckbox()))
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("Transfer window displayed and Data masking check box invisible");
					if (!basePage.DataMaskExamCheckbox().Selected)
					{
						isDataMaskSelected = false;
					}
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				if (!isDataMaskSelected)
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("Data masking check box is unchecked");
					
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Result for Step-3
				if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].status = "Fail";
				}
				else
				{
					result.steps[ExecutedSteps].status = "Pass";
				}

				//Step4: Select 'Local System' from Transfer to dropdown and Check the "Data Mask Exam" checkbox and Click Transfer button
				isDataMaskSelected = true;
				studies.Dropdown_TransferTo().SelectByText("Local System");
				try { isDataMaskSelected = basePage.DataMaskExamCheckbox().Selected; }
				catch (Exception) { }
				if (!isDataMaskSelected)
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("Data masking check box is unchecked");
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", basePage.DataMaskExamCheckbox());
				((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", studies.Btn_StudyPageTransferBtn());
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				if(basePage.IsElementVisible(By.CssSelector((BasePage.DataMaskSettingsWindow))))
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				//Result for Step-4
				if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].status = "Fail";
				}
				else
				{
					result.steps[ExecutedSteps].status = "Pass";
				}

				//Stpe5: 	Observe the Data masking window fields and placeholders
				bool res1 = BasePage.FindElementByCss(BasePage.DataMaskFirstName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res2 = BasePage.FindElementByCss(BasePage.DataMaskMiddleName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res3 = BasePage.FindElementByCss(BasePage.DataMaskLastName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res4 = BasePage.FindElementByCss(BasePage.DataMaskPrefixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
				bool res5 = BasePage.FindElementByCss(BasePage.DataMaskSuffixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
				bool res6 = BasePage.FindElementByCss(BasePage.DataMaskPatientID).GetAttribute("placeholder").Equals("(auto generated)");
				bool res7 = BasePage.FindElementByCss(BasePage.DataMaskIssuerPatientID).GetAttribute("placeholder").Equals("(removed)");
				bool res8 = new SelectElement(BasePage.Driver.FindElement(By.CssSelector(BasePage.DataMaskGender))).SelectedOption.Text.Equals("Male");
			//	//  bool res9 = BasePage.FindElementByCss(BasePage.DataMaskDOB).GetAttribute("placeholder").Equals("(auto generated)");
				bool res10 = BasePage.FindElementByCss(BasePage.DataMaskAccessionNo).GetAttribute("placeholder").Equals("(auto generated)");
				bool res11 = BasePage.FindElementByCss(BasePage.DataMaskStudyDescription).GetAttribute("placeholder").Equals("(removed)");
			//	//  bool res12 = BasePage.FindElementByCss(BasePage.DataMaskStudyDate).GetAttribute("placeholder").Equals("(auto generated)");
				if (res1 && res2 && res3 && res4 && res5 && res6 && res7 && res8 && res10 & res11)
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				/*
				//Step6: Add invalid date in "Date of birth" field
				var patientDOB = BasePage.FindElementByCss(BasePage.DataMaskDOB);
				patientDOB.SendKeys("78-Apr-1987");
				BasePage.FindElementByCss(BasePage.DataMaskPatientID).Click();
				if (patientDOB.GetAttribute("value").Equals(""))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step7: Add invalid date in "Study Date" field
				var studyDate = BasePage.FindElementByCss(BasePage.DataMaskStudyDate);
				studyDate.SendKeys("78-Apr-1987");
				BasePage.FindElementByCss(BasePage.DataMaskPatientID).Click();
				if (studyDate.GetAttribute("value").Equals(""))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				*/
				// Step 6_1: Add special characters/alphanumeric in Patient information
				((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", BasePage.FindElementByCss(BasePage.DataMaskCancel));
				PageLoadWait.WaitForFrameLoad(10);
				var DataMaskValues = basePage.GetDataMaskFieldsNames();
				DataMaskValues["FirstName"] = "@561" + login.RandomString(4, true); ;
				DataMaskValues["MiddleName"] = "@561" + login.RandomString(4, true); ;
				DataMaskValues["LastName"] = "@561" + login.RandomString(4, true); ;
				DataMaskValues["Prefix"] = "@561" + login.RandomString(4, true); ;
				DataMaskValues["Suffix"] = "@561" + login.RandomString(4, true); ;
				DataMaskValues["PatientID"] = "@561" + login.RandomString(4, true); ;
				studies.TransferStudy("Local System", SelectallPriors: false, isDataMasking: true, DataMaskValues: DataMaskValues);
				ExecutedSteps++;

				// Step 6_2: Click Download button when the status updated to "Ready"
				PageLoadWait.WaitForDownload("_" + DataMaskValues["LastName"], Config.downloadpath, "zip");
				if (BasePage.CheckFile("_" + DataMaskValues["LastName"], Config.downloadpath, "zip"))
				{
					result.steps[ExecutedSteps].AddPassStatusList("Study Transferred Successfully");
					//Logger.Instance.InfoLog("Study Transferred Successfully");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
				}
				//Unzip file and check the DICOM file values for Patient Information and Study Information
				var dicomPath = BasePage.ExtractZipFiles(FileName: "_" + DataMaskValues["LastName"]).Find(x => x.Contains("F00"));
				var ExtractedFilePID = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.PatientID);
				var ExtractedFileACC = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.AccessionNumber);
				var ExtractedFilePatientName = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.PatientName);
				if (ExtractedFilePID == DataMaskValues["PatientID"] && ExtractedFilePatientName.Contains(DataMaskValues["LastName"]))
				{
					Logger.Instance.InfoLog("Patient Name: " + ExtractedFilePatientName);
					result.steps[ExecutedSteps].AddPassStatusList("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
				}

				//Step 6_3: Search for this study instance on EA
				login.Logout();
				var hplogin = new HPLogin();
				basePage.DriverGoTo("https://" + Config.EA7 + "/eaweb");
				var hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
				var workflow = (WorkFlow)hphome.Navigate("Workflow");
				if (workflow.EAv12SearchStudy("PatientID", ExtractedFilePID))
				{
					result.steps[ExecutedSteps].AddPassStatusList("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
				}

				hplogin.LogoutEAv12();
				// Step 8: Result
				if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].status = "Fail";
				}
				else
				{
					result.steps[ExecutedSteps].status = "Pass";
				}
				//Step 9: Search the saved instance. It should load the study in viewer without error
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminUserName, adminPassword);
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(patientID: ExtractedFilePID);
				studies.SelectStudy("Accession", ExtractedFileACC);
				studies.LaunchStudy();
				ExecutedSteps++;
				BasePage.DeleteAllFileFolder(Config.downloadpath);

				//Step 10_1: Add special characters/alphanumeric in Study Information(ie. Study desc, accession no.) and transfer study
				studies.CloseStudy();
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(patientID: PID);
				studies.SelectStudy("Accession", ACC);
				DataMaskValues = basePage.GetDataMaskFieldsNames();
				DataMaskValues["IssuerPatientID"] = "@561" + login.RandomString(4, true); ;
				DataMaskValues["Accession"] = "@561" + login.RandomString(4, true); ;
				DataMaskValues["StudyDescription"] = "@561" + login.RandomString(4, true); ;
				studies.TransferStudy("Local System", SelectallPriors: false, isDataMasking: true, DataMaskValues: DataMaskValues, DownloadStudy: false);
				ExecutedSteps++;

				//// Step 10_2: Click Download button when the status updated to "Ready"
				//PageLoadWait.WaitForDownload("_" + Name, Config.downloadpath, "zip");
				//if (BasePage.CheckFile("_" + Name, Config.downloadpath, "zip"))
				//{
				//	result.steps[ExecutedSteps].AddPassStatusList("-->Test Step Passed-- Study downloaded");
				//}
				//else
				//{
				//	result.steps[ExecutedSteps].AddFailStatusList("-->Test Step Failed-- Study not downloaded");
				//}
				////Unzip file and check the DICOM file values for Patient Information and Study Information
				//dicomPath = BasePage.ExtractZipFiles(FileName: "_" + Name).Find(x => x.Contains("F00"));
				//ExtractedFilePID = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.PatientID);
				//ExtractedFileACC = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.AccessionNumber);
				//if (ExtractedFileACC == DataMaskValues["Accession"])
				//{
				//	result.steps[ExecutedSteps].AddPassStatusList("-->Test Step Passed-- DICOM file conatins moidifed study info");
				//}
				//else
				//{
				//	result.steps[ExecutedSteps].AddFailStatusList("-->Test Step Failed-- DICOM file conatins moidifed study info");
				//}

				//Step 10_3: Search for this study instance on EA
				login.Logout();
				hplogin = new HPLogin();
				basePage.DriverGoTo("https://" + Config.EA7 + "/eaweb");
				hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
				workflow = (WorkFlow)hphome.Navigate("Workflow");
				if (workflow.EAv12SearchStudy("PatientID", ExtractedFilePID))
				{
					result.steps[ExecutedSteps].AddPassStatusList("-->Test Step Passed-- new study instance found in EA");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList("-->Test Step Failed-- new study instance not found in EA");
				}

				hplogin.LogoutEAv12();
				// Step 10: Result
				if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].status = "Fail";
				}
				else
				{
					result.steps[ExecutedSteps].status = "Pass";
				}

				//Step 11: Search the saved instance. It should load the study in viewer without error
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminUserName, adminPassword);
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(patientID: ExtractedFilePID);
				studies.SelectStudy("Accession", ExtractedFileACC);
				studies.LaunchStudy();
				ExecutedSteps++;

				//Step 12: Login to SQL Management studio 2.Create new query: Select* from Job
				DataBaseUtil db = new DataBaseUtil("sqlserver");
				db.ConnectSQLServerDB();
				var query = "SELECT * FROM [IRWSDB].[dbo].[Job] where Detail Like '%<IsStudyDeidentified>True</IsStudyDeidentified>%<AccessionNumber>" + ACC + "</AccessionNumber>%';";
				IList<String> dbResult = db.ExecuteQuery(query);
				if (dbResult.Count != 0)
				{
					result.steps[++ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[++ExecutedSteps].StepFail();
				}
				login.Logout();

				//Return result
				result.FinalResult(ExecutedSteps);
				Logger.Instance.InfoLog("Overall Test status--" + result.status);

				return result;
			}
			catch (Exception e)
			{
				//Log Exception
				Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
				result.FinalResult(e, ExecutedSteps);
				Logger.Instance.ErrorLog("Overall Test status--" + result.status);
				return result;
			}
		}

		/// <summary>
		///  Multiple studies selected from different patient on datasource with data masking enabled
		/// </summary>
		/// <returns></returns>
		public TestCaseResult Test_160550(String testid, String teststeps, int stepcount)
		{
			TestCaseResult result = new TestCaseResult(stepcount);
			result.SetTestStepDescription(teststeps);
			int ExecutedSteps = -1;
			Random randomnumber = new Random();
			WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 300));
			List<string> PID = new List<string>();
			List<string> Name = new List<string>();
			string PrefixName = null;
			try
			{
				String adminUserName = Config.adminUserName;
				String adminPassword = Config.adminPassword;
				var studypaths = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
				String[] studypath = studypaths.Split('=');
				PrefixName = login.RandomString(4, true);

				//Precondition - Send multiple studies of diff patient to EA12
				PID.Add("PID" + System.DateTime.Now.ToString("ddHHmmss"));
				Name.Add(PrefixName + new System.DateTime().Second + randomnumber.Next(1, 100));
				List<string> ACC = new List<string>();
				int patientCount = 0;
				string filename = string.Empty;
				string[] SOPID = Enumerable.Repeat("1.2.840.113837.390681903." + System.DateTime.Now.ToString("MMddHHmmss"), studypath.Length).ToArray();
				for (int count = 0; count < studypath.Length; count++)
				{
					DicomClient client = new DicomClient();
					ACC.Add("ACC" + System.DateTime.Now.ToString("ddHHmmss") + count);
					string SID = "1.2.840.113837.1845205803." + System.DateTime.Now.ToString("MMddHHmmss") + "." + Config.IConnectIP.Split('.')[3];
					string SED = "1.2.840.113837.4288965276." + System.DateTime.Now.ToString("MMddHHmmss") + "." + Config.IConnectIP.Split('.')[3];
					SOPID[count] = SOPID[count] + ".0" + (count + 1);
					filename = BasePage.WriteDicomFile(studypath[count], new DicomTag[] { DicomTag.SOPInstanceUID, DicomTag.PatientName, DicomTag.PatientID, DicomTag.AccessionNumber, DicomTag.StudyInstanceUID, DicomTag.SeriesInstanceUID, DicomTag.InstanceNumber }, new string[] { SOPID[count], Name[patientCount], PID[patientCount], ACC[count], SID, SED, Convert.ToString(count + 1) });
					client.AddRequest(new DicomCStoreRequest(filename));
					client.Send(Config.EA7, 12000, false, "SCU", Config.EA7AETitle);
					Thread.Sleep(2000);//Study sending to EA
					if (count == 0)
					{
						PID.Add("PID" + System.DateTime.Now.ToString("ddHHmmss"));
						Name.Add(PrefixName + new System.DateTime().Second + randomnumber.Next(1, 100));
						patientCount++;
					}
				}

				//Step1: Login to iCA 7.0 as privileged user (Administrator)
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminUserName, adminPassword);
				ExecutedSteps++;

				//Step2: Navigate to study tab and search a study from data masking enabled data source 
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(LastName: PrefixName + "*", Datasource: login.GetHostName(Config.EA7));
				studies.ChooseColumns(new string[] { "Accession" });
				var Actual = BasePage.GetColumnValues("Accession");
				if (ACC.All(accession => Actual.Contains(accession)))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step3: Select Multiple study from diff patient and click "Transfer" button
				Actions action = new Actions(BasePage.Driver);
				action.KeyDown(Keys.Control).Perform();
				for (int count = 0; count < ACC.Count; count++)
				{
					studies.SelectStudy1("Accession", ACC[count],ctrlclick: true);
				}
				studies.TransferBtn().Click();
				PageLoadWait.WaitForFrameLoad(10);
				BasePage.Driver.SwitchTo().DefaultContent();
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				if (basePage.IsElementVisible(basePage.By_DataMaskCheckbox()) && (!basePage.DataMaskExamCheckbox().Selected))
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("Data Mask Exam checkbox visible and unchecked");
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("Data Mask Exam is invisible or checked");
					result.steps[ExecutedSteps].SetLogs();
					if (!basePage.IsElementVisible(basePage.By_DataMaskCheckbox()))
					{
						throw new Exception("Data Mask checkbox not displayed");
					}
				}
				var TransferStudyList = BasePage.Driver.FindElement(By.CssSelector("#ctl00_StudyTransferControl_transferDataGrid"));
				if (ACC.Any<string>( accessionNo => !TransferStudyList.Text.Contains(accessionNo)))
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("Selected studies listed in transfer window");
					result.steps[ExecutedSteps].SetLogs();
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("Selected studies are not listed in transfer window");
				}
				//Result for Step-3
				if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].status = "Fail";
				}
				else
				{
					result.steps[ExecutedSteps].status = "Pass";
				}

				//Step4: Select 'Local System' from Transfer to dropdown
				studies.Dropdown_TransferTo().SelectByText("Local System");
				if (basePage.IsElementVisible(basePage.By_DataMaskCheckbox()) && (!basePage.DataMaskExamCheckbox().Selected))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
					if (!basePage.IsElementVisible(basePage.By_DataMaskCheckbox()))
					{
						throw new Exception("Data Mask checkbox not displayed");
					}
				}

				//Step5_1: Check the Data masking setting window fields
				((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", basePage.DataMaskExamCheckbox());
				((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", studies.Btn_StudyPageTransferBtn());
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				if (basePage.IsElementVisible(By.CssSelector(BasePage.DataMaskSettingsWindow)))
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				bool res1 = BasePage.FindElementByCss(BasePage.DataMaskFirstName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res2 = BasePage.FindElementByCss(BasePage.DataMaskMiddleName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res3 = BasePage.FindElementByCss(BasePage.DataMaskLastName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res4 = BasePage.FindElementByCss(BasePage.DataMaskPrefixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
				bool res5 = BasePage.FindElementByCss(BasePage.DataMaskSuffixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
				bool res6 = BasePage.FindElementByCss(BasePage.DataMaskPatientID).GetAttribute("placeholder").Equals("(auto generated)");
				bool res7 = BasePage.FindElementByCss(BasePage.DataMaskIssuerPatientID).GetAttribute("placeholder").Equals("(removed)");
				bool res8 = new SelectElement(BasePage.Driver.FindElement(By.CssSelector(BasePage.DataMaskGender))).SelectedOption.Text.Equals("Male");
				//  bool res9 = BasePage.FindElementByCss(BasePage.DataMaskDOB).GetAttribute("placeholder").Equals("(auto generated)");
				bool res10 = BasePage.FindElementByCss(BasePage.DataMaskAccessionNo).GetAttribute("placeholder").Equals("(auto generated)");
				bool res11 = BasePage.FindElementByCss(BasePage.DataMaskStudyDescription).GetAttribute("placeholder").Equals("(removed)");
				//  bool res12 = BasePage.FindElementByCss(BasePage.DataMaskStudyDate).GetAttribute("placeholder").Equals("(auto generated)");
				if (res1 && res2 && res3 && res4 && res5 && res6 && res7 && res8 && res10 & res11)
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed-- Prior Studies not loaded in Transfer window");
					result.steps[ExecutedSteps].SetLogs();
				}
				//Step5_1: Checkbox "Copy patient attributes to all studies" should be unchecked and editable by default
				var copyCheckBox = BasePage.FindElementByCss(BasePage.DataMaskCopyCheckbox);
				if (!copyCheckBox.Selected && copyCheckBox.Enabled)
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				//Result for Step-5
				if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].status = "Fail";
				}
				else
				{
					result.steps[ExecutedSteps].status = "Pass";
				}

				//Step6: Do not change any value in Data masking setting and click on Submit button
				((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", BasePage.FindElementByCss(BasePage.DataMaskCancel));
				PageLoadWait.WaitForFrameLoad(10);
				studies.TransferStudy("Local System", SelectallPriors: false, PatientTab: true, isDataMasking: true);
				ExecutedSteps++;

				//Step7: Click Download button when the status updated to "Ready"
				PageLoadWait.WaitForDownload(patientNameWithoutDeidentificationValue, Config.downloadpath, "zip");
				if (BasePage.CheckFile(patientNameWithoutDeidentificationValue, Config.downloadpath, "zip"))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("Study Transferred Successfully");
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				login.Logout();

				//Step8: Unzip file and check the DICOM file values for Patient Information and Study Information
				var dicomPath = BasePage.ExtractZipFiles(FileName: patientNameWithoutDeidentificationValue).Find(x => x.Contains("F00"));
				String ExtractedFilePID = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.PatientID);
				String ExtractedFileACC = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.AccessionNumber);
				if (ExtractedFileACC == null) ExtractedFileACC = "";
				if (ExtractedFilePID == PID[0] || ACC.Any<string>( acc => ExtractedFileACC.Contains(acc)))
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}

				//Step9: Search for these study instances on EA
				var hplogin = new HPLogin();
				basePage.DriverGoTo("https://" + Config.EA7 + "/eaweb");
				var hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
				var workflow = (WorkFlow)hphome.Navigate("Workflow");
				if (workflow.EAv12SearchStudy("PatientID", ExtractedFilePID))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("Data Masked Study found in EA");
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				hplogin.LogoutEAv12();
				BasePage.DeleteAllFileFolder(Config.downloadpath);

				//Step10_1: Select Multiple study from diff patient and click "Transfer" button
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminUserName, adminPassword);
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(LastName: PrefixName + "*", Datasource: login.GetHostName(Config.EA7));
				for (int count = 0; count < ACC.Count; count++)
				{
					studies.SelectStudy1("Accession", ACC[count],ctrlclick: true);
				}

				//Step10_2: Update values for Patient information and Study Information and Submit
				var newDataMaskValues = basePage.GetDataMaskFieldsNames();
				newDataMaskValues["LastName"] = login.RandomString(5, true);
				newDataMaskValues["PatientID"] = "PID" + System.DateTime.Now.ToString("ddHHmmss");
				newDataMaskValues["Accession"] = "ACC" + System.DateTime.Now.ToString("ddHHmmss");
				basePage.TransferStudy("Local System", SelectallPriors: false, PatientTab: true, isDataMasking: true, DataMaskValues: newDataMaskValues, LastName: Name[0]);
				ExecutedSteps++;

				//Step11: Click Download button when the status updated to "Ready"
				PageLoadWait.WaitForDownload("_" + newDataMaskValues["LastName"], Config.downloadpath, "zip");
				if (BasePage.CheckFile("_" + newDataMaskValues["LastName"], Config.downloadpath, "zip"))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("Study Transferred Successfully");
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step12: Unzip file and check the DICOM file values for Patient Information and Study Information
				dicomPath = BasePage.ExtractZipFiles(FileName: "_" + newDataMaskValues["LastName"]).Find(x => x.Contains("F00"));
				ExtractedFilePID = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.PatientID);
				ExtractedFileACC = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.AccessionNumber);
				if (ExtractedFilePID == newDataMaskValues["PatientID"] && ExtractedFileACC == newDataMaskValues["Accession"])
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step13: New study instance should also be stored on EA
				login.Logout();
				hplogin = new HPLogin();
				basePage.DriverGoTo("https://" + Config.EA7 + "/eaweb");
				hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL:EA12Url);
				workflow = (WorkFlow)hphome.Navigate("Workflow");
				if (workflow.EAv12SearchStudy("PatientID", newDataMaskValues["PatientID"], StudiesCount: 1))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				hplogin.LogoutEAv12();
				BasePage.DeleteAllFileFolder(Config.downloadpath);

				//Step14_1: Select Multiple study from diff patient and click "Transfer" button
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminUserName, adminPassword);
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(LastName: PrefixName + "*", Datasource: login.GetHostName(Config.EA7));
				for (int count = 0; count < ACC.Count; count++)
				{
					studies.SelectStudy1("Accession", ACC[count],ctrlclick: true);
				}

				//Step14_2: Update values for Patient information and Study Information and Submit
				newDataMaskValues = basePage.GetDataMaskFieldsNames();
				newDataMaskValues["PatientID"] = "PID" + System.DateTime.Now.ToString("ddHHmmss");
				newDataMaskValues["Accession"] = "ACC" + System.DateTime.Now.ToString("ddHHmmss");
				newDataMaskValues["LastName"] = login.RandomString(6, true);				
				newDataMaskValues["CopyAttributes"] = "true";
				basePage.TransferStudy("Local System", waittime:180, SelectallPriors: false, PatientTab: true, isDataMasking: true, DataMaskValues: newDataMaskValues, LastName: Name[1]);
				ExecutedSteps++;

				//Step15: Click Download button when the status updated to "Ready"
				PageLoadWait.WaitForDownload("_" + newDataMaskValues["LastName"], Config.downloadpath, "zip");
				if (BasePage.CheckFile("_" + newDataMaskValues["LastName"], Config.downloadpath, "zip"))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("Study Transferred Successfully");
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step16: Unzip file and check the DICOM file values for Patient Information and Study Information
				dicomPath = BasePage.ExtractZipFiles(FileName: "_" + newDataMaskValues["LastName"]).Find(x => x.Contains("F00"));
				ExtractedFilePID = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.PatientID);
				ExtractedFileACC = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.AccessionNumber);
				String patient = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.PatientName).Replace("^", " ");
				String ExtractedFilePatientName = patient.Split(' ')[0];

				Logger.Instance.InfoLog("Patient Name- Expected:" + newDataMaskValues["LastName"] + ", Actual: "+ ExtractedFilePatientName);
				Logger.Instance.InfoLog("Patient ID- Expected:" + newDataMaskValues["PatientID"] + ", Actual: " + ExtractedFilePID);
				Logger.Instance.InfoLog("Patient Acc- Expected:" + newDataMaskValues["Accession"] + ", Actual: " + ExtractedFileACC);
				if (ExtractedFilePID == newDataMaskValues["PatientID"] && ExtractedFileACC != newDataMaskValues["Accession"] && ExtractedFilePatientName == newDataMaskValues["LastName"])
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step17: New study instance should also be stored on EA
				login.Logout();
				hplogin = new HPLogin();
				basePage.DriverGoTo("https://" + Config.EA7 + "/eaweb");
				hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
				workflow = (WorkFlow)hphome.Navigate("Workflow");
				if (workflow.EAv12SearchStudy("PatientID", newDataMaskValues["PatientID"], StudiesCount: 5))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				hplogin.LogoutEAv12();
				BasePage.DeleteAllFileFolder(Config.downloadpath);
				//Return result
				result.FinalResult(ExecutedSteps);
				Logger.Instance.InfoLog("Overall Test status--" + result.status);

				return result;
			}
			catch (Exception e)
			{
				//Log Exception
				Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
				result.FinalResult(e, ExecutedSteps);
				Logger.Instance.ErrorLog("Overall Test status--" + result.status);
				return result;
			}
			finally
			{
				try
				{
					BasePage.DeleteAllFileFolder(Config.downloadpath);
					//Deleting uploaded study
					var hplogin = new HPLogin();
					BasePage.Driver.Navigate().GoToUrl("https://" + Config.EA7 + "/eaweb");
					var hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
					var workflow = (WorkFlow)hphome.Navigate("Workflow");
					workflow.EAv12SearchStudy("Lastname", PrefixName + "*", SelectStudy: true);
					workflow.EAv12DeleteStudy();
					hplogin.LogoutEAv12();
				}
				catch(Exception ex)
				{
					Logger.Instance.ErrorLog("Exception in finally Block: " + ex);
				}
			}

		}

		/// <summary>
		///  Multiple studies selected for different patients(one study from unsupported Datasource)
		/// </summary>
		/// <returns></returns>
		public TestCaseResult Test_160552(String testid, String teststeps, int stepcount)
		{
			TestCaseResult result = new TestCaseResult(stepcount);
			result.SetTestStepDescription(teststeps);
			int ExecutedSteps = -1;
			Random randomnumber = new Random();
			WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 300));
			List<string> PID = new List<string>();
			List<string> Name = new List<string>();
			string PrefixName = null;
			try
			{
				String adminUserName = Config.adminUserName;
				String adminPassword = Config.adminPassword;
				var studypaths = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
				String[] studypath = studypaths.Split('=');
				PrefixName = login.RandomString(4, true);

				//Precondition - Send multiple studies of diff patient to EA12
				PID.Add("PID" + System.DateTime.Now.ToString("ddHHmmss"));
				Name.Add(PrefixName + new System.DateTime().Second + randomnumber.Next(1, 100));
				List<string> ACC = new List<string>();
				int patientCount = 0;
				string filename = string.Empty;
				string[] SOPID = Enumerable.Repeat("1.2.840.113837.390681903." + System.DateTime.Now.ToString("MMddHHmmss"), studypath.Length).ToArray();
				var host = Config.EA7;
				var aeTitle = Config.EA7AETitle;
				for (int count = 0; count < studypath.Length; count++)
				{
					DicomClient client = new DicomClient();
					ACC.Add("ACC" + System.DateTime.Now.ToString("ddHHmmss") + count);
					string SID = "1.2.840.113837.1845205803." + System.DateTime.Now.ToString("MMddHHmmss") + "." + Config.IConnectIP.Split('.')[3];
					string SED = "1.2.840.113837.4288965276." + System.DateTime.Now.ToString("MMddHHmmss") + "." + Config.IConnectIP.Split('.')[3];
					SOPID[count] = SOPID[count] + ".0" + (count + 1);
					filename = BasePage.WriteDicomFile(studypath[count], new DicomTag[] { DicomTag.SOPInstanceUID, DicomTag.PatientName, DicomTag.PatientID, DicomTag.AccessionNumber, DicomTag.StudyInstanceUID, DicomTag.SeriesInstanceUID, DicomTag.InstanceNumber }, new string[] { SOPID[count], Name[patientCount], PID[patientCount], ACC[count], SID, SED, Convert.ToString(count + 1) });
					client.AddRequest(new DicomCStoreRequest(filename));
					client.Send(host, 12000, false, "SCU", aeTitle);
					Thread.Sleep(2000);//Study sending to EA
					if (count == 0)
					{
						PID.Add("PID" + System.DateTime.Now.ToString("ddHHmmss"));
						Name.Add(PrefixName + new System.DateTime().Second + randomnumber.Next(1, 100));
						patientCount++;
					}
					if (count == 2)
					{
						PID.Add("PID" + System.DateTime.Now.ToString("ddHHmmss"));
						Name.Add(PrefixName + new System.DateTime().Second + randomnumber.Next(1, 100));
						host = Config.EA96; aeTitle = Config.EA96AETitle;
						patientCount++;
					}
				}

				//Step1: Login to iCA 7.0 as privileged user (Administrator)
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminUserName, adminPassword);
				ExecutedSteps++;

				//Step2: Navigate to study tab and search a study from data masking enabled data source 
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(LastName: PrefixName + "*");
				studies.ChooseColumns(new string[] { "Accession" });
				var Actual = BasePage.GetColumnValues("Accession");
				if (ACC.All(accession => Actual.Contains(accession)))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step3: Select Multiple study from diff patient and click "Transfer" button
				for (int count = 0; count < ACC.Count; count++)
				{
					studies.SelectStudy1("Accession", ACC[count], ctrlclick: true);
				}
				studies.TransferBtn().Click();
				PageLoadWait.WaitForFrameLoad(10);
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				var TransferStudyList = BasePage.Driver.FindElement(By.CssSelector("#ctl00_StudyTransferControl_transferDataGrid"));
				if (ACC.Any<string>(accessionNo => !TransferStudyList.Text.Contains(accessionNo)))
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("Selected studies listed in transfer window");
					result.steps[ExecutedSteps].SetLogs();
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("Selected studies are not listed in transfer window");
				}
				if (basePage.IsElementVisible(basePage.By_DataMaskCheckbox()) && (!basePage.DataMaskExamCheckbox().Selected))
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("Data Mask Exam checkbox visible and unchecked");
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("Data Mask Exam is invisible or checked");
					result.steps[ExecutedSteps].SetLogs();
					if (!basePage.IsElementVisible(basePage.By_DataMaskCheckbox()))
					{
						throw new Exception("Data Mask checkbox not displayed");
					}
				}
				//Result for Step-3
				if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].status = "Fail";
				}
				else
				{
					result.steps[ExecutedSteps].status = "Pass";
				}

				//Step4: Check Data Mask Exam checkbox and observe the studies in Data masking setting window
				studies.Dropdown_TransferTo().SelectByText("Local System");
				basePage.ClickElement(basePage.DataMaskExamCheckbox());
				var step4 = true;
				for (int count= 0; count < ACC.Count; count++)
				{
					if (count <= 2 && !TransferStudyList.Text.Contains(ACC[count]))
					{
						step4 = false;
						break;
					}
					if (count > 2 && TransferStudyList.Text.Contains(ACC[count]))
					{
						step4 = false;
						break;
					}
				}
				if (step4)
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				basePage.ClickElement(studies.Btn_StudyPageTransferBtn());
				//((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", basePage.DataMaskExamCheckbox());
				//((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", studies.Btn_StudyPageTransferBtn());

				//Step5: Check the Data masking setting window fields
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				if (basePage.IsElementVisible(By.CssSelector(BasePage.DataMaskSettingsWindow)))
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				bool res1 = BasePage.FindElementByCss(BasePage.DataMaskFirstName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res2 = BasePage.FindElementByCss(BasePage.DataMaskMiddleName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res3 = BasePage.FindElementByCss(BasePage.DataMaskLastName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res4 = BasePage.FindElementByCss(BasePage.DataMaskPrefixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
				bool res5 = BasePage.FindElementByCss(BasePage.DataMaskSuffixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
				bool res6 = BasePage.FindElementByCss(BasePage.DataMaskPatientID).GetAttribute("placeholder").Equals("(auto generated)");
				bool res7 = BasePage.FindElementByCss(BasePage.DataMaskIssuerPatientID).GetAttribute("placeholder").Equals("(removed)");
				bool res8 = new SelectElement(BasePage.Driver.FindElement(By.CssSelector(BasePage.DataMaskGender))).SelectedOption.Text.Equals("Male");
				//  bool res9 = BasePage.FindElementByCss(BasePage.DataMaskDOB).GetAttribute("placeholder").Equals("(auto generated)");
				bool res10 = BasePage.FindElementByCss(BasePage.DataMaskAccessionNo).GetAttribute("placeholder").Equals("(auto generated)");
				bool res11 = BasePage.FindElementByCss(BasePage.DataMaskStudyDescription).GetAttribute("placeholder").Equals("(removed)");
				//  bool res12 = BasePage.FindElementByCss(BasePage.DataMaskStudyDate).GetAttribute("placeholder").Equals("(auto generated)");
				if (res1 && res2 && res3 && res4 && res5 && res6 && res7 && res8 && res10 & res11)
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed-- Prior Studies not loaded in Transfer window");
					result.steps[ExecutedSteps].SetLogs();
				}
				//Result for Step-5
				if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].status = "Fail";
				}
				else
				{
					result.steps[ExecutedSteps].status = "Pass";
				}

				//Step6: Do not change any value in Data masking setting and click on Submit button
				((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", BasePage.FindElementByCss(BasePage.DataMaskCancel));
				PageLoadWait.WaitForFrameLoad(10);
				//var newDataMaskValues = basePage.GetDataMaskFieldsNames();
				//newDataMaskValues["CopyAttributes"] = "false";
				studies.TransferStudy("Local System", SelectallPriors: false, PatientTab: true, isDataMasking: true, LastName: Name[0], waittime: 180);
				ExecutedSteps++;

				//Step7: Click Download button when the status updated to "Ready"
				PageLoadWait.WaitForDownload(patientNameWithoutDeidentificationValue, Config.downloadpath, "zip");
				if (BasePage.CheckFile(patientNameWithoutDeidentificationValue, Config.downloadpath, "zip"))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("Study Transferred Successfully");
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				login.Logout();

				//Step8: Unzip file and check the DICOM file values for Patient Information and Study Information
				var dicomPath = BasePage.ExtractZipFiles(FileName: patientNameWithoutDeidentificationValue).Find(x => x.Contains("F00"));
				String ExtractedFilePID = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.PatientID);
				String ExtractedFileACC = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.AccessionNumber);
				if (ExtractedFileACC == null) ExtractedFileACC = "";
				if (ExtractedFilePID == PID[0] || ACC.Any<string>(acc => ExtractedFileACC.Contains(acc)))
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}

				//Step9: Search for these study instances on EA
				var hplogin = new HPLogin();
				basePage.DriverGoTo("https://" + Config.EA7 + "/eaweb");
				var hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
				var workflow = (WorkFlow)hphome.Navigate("Workflow");
				if (workflow.EAv12SearchStudy("PatientID", ExtractedFilePID,1))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("Data Masked Study found in EA");
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				hplogin.LogoutEAv12();
				BasePage.DeleteAllFileFolder(Config.downloadpath);
				
				//Return result
				result.FinalResult(ExecutedSteps);
				Logger.Instance.InfoLog("Overall Test status--" + result.status);

				return result;
			}
			catch (Exception e)
			{
				//Log Exception
				Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
				result.FinalResult(e, ExecutedSteps);
				Logger.Instance.ErrorLog("Overall Test status--" + result.status);
				return result;
			}
			finally
			{
				try
				{
					BasePage.DeleteAllFileFolder(Config.downloadpath);
					//Deleting uploaded study
					var hplogin = new HPLogin();
					BasePage.Driver.Navigate().GoToUrl("https://" + Config.EA7 + "/eaweb");
					var hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
					var workflow = (WorkFlow)hphome.Navigate("Workflow");
					workflow.EAv12SearchStudy("Lastname", PrefixName + "*", SelectStudy: true);
					workflow.EAv12DeleteStudy();
					hplogin.LogoutEAv12();
				}
				catch (Exception ex)
				{
					Logger.Instance.ErrorLog("Exception in finally Block: " + ex);
				}
			}

		}

		/// <summary>
		///  Data masking on datasource with EA version lower than 12
		/// </summary>
		/// <returns></returns>
		public TestCaseResult Test_161431(String testid, String teststeps, int stepcount)
		{
			TestCaseResult result = new TestCaseResult(stepcount);
			result.SetTestStepDescription(teststeps);
			int ExecutedSteps = -1;
			Random randomnumber = new Random();
			WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 300));
			string[] accList = null;
			try
			{
				String adminUserName = Config.adminUserName;
				String adminPassword = Config.adminPassword;
				String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
				accList = accession.Split(':');
				String lastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
				String firstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");

				//Step1: Login to iCA 7.0 as privileged user (Administrator)
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminUserName, adminPassword);
				ExecutedSteps++;

				//Step2: Navigate to study tab and search a study from data masking enabled data source EA11
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(LastName: lastName, FirstName: firstName, AccessionNo: accList[0], Datasource: login.GetHostName(Config.EA91));
				if (studies.CheckStudy("Accession", accList[0]))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step3: Select one study and Click transfer button
				studies.SelectStudy1("Accession", accList[0]);
				studies.TransferBtn().Click();
				PageLoadWait.WaitForFrameLoad(10);
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				if (basePage.IsElementVisible(basePage.By_DataMaskCheckbox()) && (!basePage.DataMaskExamCheckbox().Selected))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("Data Mask Exam checkbox visible and unchecked");
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("Data Mask Exam is invisible or checked");
					result.steps[ExecutedSteps].SetLogs();
					if (!basePage.IsElementVisible(basePage.By_DataMaskCheckbox()))
					{
						throw new Exception("Data Mask checkbox not displayed");
					}
				}

				//Step4: Check Data Mask Exam checkbox and observe the studies in Data masking setting window
				studies.Dropdown_TransferTo().SelectByText("Local System");
				basePage.ClickElement(basePage.DataMaskExamCheckbox());
				basePage.ClickElement(studies.Btn_StudyPageTransferBtn());
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				if (basePage.IsElementVisible(By.CssSelector(BasePage.DataMaskSettingsWindow)))
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				bool res1 = BasePage.FindElementByCss(BasePage.DataMaskFirstName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res2 = BasePage.FindElementByCss(BasePage.DataMaskMiddleName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res3 = BasePage.FindElementByCss(BasePage.DataMaskLastName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res4 = BasePage.FindElementByCss(BasePage.DataMaskPrefixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
				bool res5 = BasePage.FindElementByCss(BasePage.DataMaskSuffixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
				bool res6 = BasePage.FindElementByCss(BasePage.DataMaskPatientID).GetAttribute("placeholder").Equals("(auto generated)");
				bool res7 = BasePage.FindElementByCss(BasePage.DataMaskIssuerPatientID).GetAttribute("placeholder").Equals("(removed)");
				bool res8 = false;
				try
				{
					var option = new SelectElement(BasePage.Driver.FindElement(By.CssSelector(BasePage.DataMaskGender))).SelectedOption;					
					if (option.Text.ToLower().Contains("male"))
						res8 = true;
				}
				catch (Exception e)
				{
					Logger.Instance.InfoLog("patient gender is male: " + res8);
				}
					//res8 = true;
				//}
				//  bool res9 = BasePage.FindElementByCss(BasePage.DataMaskDOB).GetAttribute("placeholder").Equals("(auto generated)");
				bool res10 = BasePage.FindElementByCss(BasePage.DataMaskAccessionNo).GetAttribute("placeholder").Equals("(auto generated)");
				bool res11 = BasePage.FindElementByCss(BasePage.DataMaskStudyDescription).GetAttribute("placeholder").Equals("(removed)");
				//  bool res12 = BasePage.FindElementByCss(BasePage.DataMaskStudyDate).GetAttribute("placeholder").Equals("(auto generated)");
				if (res1 && res2 && res3 && res4 && res5 && res6 && res7 && res8 && res10 & res11)
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				//Result for Step-4
				if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].status = "Fail";
				}
				else
				{
					result.steps[ExecutedSteps].status = "Pass";
				}

				//Step5: Do not change any value in Data masking setting and click on Submit button
				((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", BasePage.FindElementByCss(BasePage.DataMaskCancel));
				PageLoadWait.WaitForFrameLoad(10);
				var transferStartTime = System.DateTime.Now;
				try
				{
					studies.TransferStudy("Local System", SelectallPriors: false, PatientTab: true, isDataMasking: true);
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				catch (Exception e)
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
					login.CreateNewSesion();
					login.LoginIConnect(adminUserName, adminPassword);
				}

				//Step 6:
				//to get the end time of the attachment
				var transferEndtTime = System.DateTime.Now;
				try
				{
					var loggedError = string.Empty;
					String Date = System.DateTime.Now.Date.ToString("yyyyMMdd");
					for (var i = 1; i >= 1; i++)
					{
						String LogFilePath = @"C:\Windows\Temp\WebAccessImageTransferServiceDeveloper-" + Date + "(" + i + ")" + ".log";
						//String LogFilePath = @"C:\Users\Administrator\Desktop\WebAccessDeveloper-" + "20170616" + "(1)" + ".log";
						//Stream stream = File.Open(LogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

						System.DateTime DateTime = System.DateTime.Now.Date;

						if (File.Exists(LogFilePath))
						{
							//StreamReader reader = new StreamReader(stream);
							var LogValues = basePage.ReadDevTraceLog(LogFilePath, transferStartTime, transferEndtTime);

							foreach (var entry in LogValues)
							{
								if (entry.Value["Source"].Contains("EAStudyDeidentification"))
								{
									loggedError = entry.Value["Message"];
									break;
								}
							}
						}
						else
						{
							Logger.Instance.ErrorLog("Unable to Read Log file");
							break;
						}
					}

					//Validation of message failed in log file
					if (loggedError == "Exception thrown")
					{
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
				catch (Exception e)
				{
					throw e;
				}

				//Step 7: Login to SQL Management studio 2.Create new query: Select* from Job
				DataBaseUtil db = new DataBaseUtil("sqlserver");
				db.ConnectSQLServerDB();
				var query = "SELECT * FROM [IRWSDB].[dbo].[Job] where Detail Like '%<IsStudyDeidentified>False</IsStudyDeidentified>%<AccessionNumber>" + accList[0] + "</AccessionNumber>%';";

				IList<String> dbResult = db.ExecuteQuery(query);
				if (dbResult.Count != 0)
				{
					result.steps[++ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[++ExecutedSteps].StepFail();
				}

				//Step8: Click on the same study> Transfer
				studies = (Studies)login.Navigate("Studies");
				//studies.SearchStudy(AccessionNo: accList[0]);
				studies.SearchStudy(LastName: lastName, FirstName: firstName, AccessionNo: accList[0], Datasource: login.GetHostName(Config.EA91));
				studies.SelectStudy1("Accession", accList[0]);
				ExecutedSteps++;

				//Step9: Select 'Local system' from dropdown and Click Transfer button(Do not check "Data mask exam" checkbox) 
				//Step10: Click submit button
				studies.TransferStudy("Local System", SelectallPriors: false, PatientTab: true);
				ExecutedSteps++;
				ExecutedSteps++;

				//Step11: Click Download button when the status updated to "Ready"
				PageLoadWait.WaitForDownload(firstName + "_" + lastName, Config.downloadpath, "zip");
				if (BasePage.CheckFile(firstName + "_" + lastName, Config.downloadpath, "zip"))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("Study Transferred Successfully");
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				BasePage.DeleteAllFileFolder(Config.downloadpath);

				//Return result
				result.FinalResult(ExecutedSteps);
				Logger.Instance.InfoLog("Overall Test status--" + result.status);

				return result;
			}
			catch (Exception e)
			{
				//Log Exception
				Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
				result.FinalResult(e, ExecutedSteps);
				Logger.Instance.ErrorLog("Overall Test status--" + result.status);
				return result;
			}
			finally
			{
				try
				{
					BasePage.DeleteAllFileFolder(Config.downloadpath);
				}
				catch (Exception ex)
				{
					Logger.Instance.ErrorLog("Exception in finally Block: " + ex);
				}
			}

		}

		/*
		/// <summary>
		///  Data masking from Remote Data source with 'Data masking' enabled - Universal viewer
		/// </summary>
		/// <returns></returns>
		public TestCaseResult Test_161321_1(String testid, String teststeps, int stepcount)
		{
			TestCaseResult result = new TestCaseResult(stepcount);
			result.SetTestStepDescription(teststeps);
			int ExecutedSteps = -1;
			Random randomnumber = new Random();
			WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 300));
			string PID = null;
			try
			{
				String adminUserName = Config.adminUserName;
				String adminPassword = Config.adminPassword;
				var studypaths = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
				String[] studypath = studypaths.Split('=');

				//Precondition - Send studies to EA12
				string Name = login.RandomString(4, true) + new System.DateTime().Second + randomnumber.Next(1, 100);
				PID = "PID" + System.DateTime.Now.ToString("ddHHmmss");
				List<string> ACC = new List<string>(); ;
				string filename = string.Empty;
				string[] SOPID = Enumerable.Repeat("1.2.840.113837.390681903." + System.DateTime.Now.ToString("MMddHHmmss"), studypath.Length).ToArray();
				for (int count = 0; count < studypath.Length; count++)
				{
					DicomClient client = new DicomClient();
					ACC.Add("ACC" + System.DateTime.Now.ToString("ddHHmmss") + count);
					string SID = "1.2.840.113837.1845205803." + System.DateTime.Now.ToString("MMddHHmmss") + "." + Config.IConnectIP.Split('.')[3];
					string SED = "1.2.840.113837.4288965276." + System.DateTime.Now.ToString("MMddHHmmss") + "." + Config.IConnectIP.Split('.')[3];
					SOPID[count] = SOPID[count] + ".0" + (count + 1);
					filename = BasePage.WriteDicomFile(studypath[count], new DicomTag[] { DicomTag.SOPInstanceUID, DicomTag.PatientName, DicomTag.PatientID, DicomTag.AccessionNumber, DicomTag.StudyInstanceUID, DicomTag.SeriesInstanceUID, DicomTag.InstanceNumber }, new string[] { SOPID[count], Name, PID, ACC[count], SID, SED, Convert.ToString(count + 1) });
					client.AddRequest(new DicomCStoreRequest(filename));
					client.Send(Config.EA7, 12000, false, "SCU", Config.EA7AETitle);
					Thread.Sleep(2000);//Study sending to EA
				}

				//Step1: Login to iCA 7.0 as privileged user (Administrator)
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminUserName, adminPassword);
				ExecutedSteps++;

				//Step2: Make sure user preferences has "New viewer/Universal viewer" as default viewer
				userpref.OpenUserPreferences();
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
				PageLoadWait.WaitForPageLoad(20);
				userpref.BluringViewerRadioBtn().Click();
				userpref.CloseUserPreferences();
				ExecutedSteps++;

				//Step3: Navigate to study tab and search a study from RDM data source EA12
				studies = (Studies)login.Navigate("Studies");
				studies.RDM_MouseHover(Config.rdm1);
				studies.SearchStudy(patientID: PID, DatasourceList: new string[] { Config.rdm1 + "." + login.GetHostName(Config.EA7)});
				studies.ChooseColumns(new string[] { "Accession" });
				var Actual = BasePage.GetColumnValues("Accession");				
				if (ACC.All(accession => Actual.Contains(accession)))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step4: Select one study and Click transfer button
				studies.SelectStudy1("Accession", ACC[0]);
				studies.TransferBtn().Click();
				PageLoadWait.WaitForFrameLoad(10);
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				if (basePage.IsElementVisible(basePage.By_DataMaskCheckbox()) && (!basePage.DataMaskExamCheckbox().Selected))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("Data Mask Exam checkbox visible and unchecked");
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("Data Mask Exam is invisible or checked");
					result.steps[ExecutedSteps].SetLogs();
					if (!basePage.IsElementVisible(basePage.By_DataMaskCheckbox()))
					{
						throw new Exception("Data Mask checkbox not displayed");
					}
				}

				//Step5: Check Data Mask Exam checkbox and observe the studies in Data masking setting window
				studies.Dropdown_TransferTo().SelectByText("Local System");
				basePage.ClickElement(basePage.DataMaskExamCheckbox());
				basePage.ClickElement(studies.Btn_StudyPageTransferBtn());
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				if (basePage.IsElementVisible(By.CssSelector(BasePage.DataMaskSettingsWindow)))
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				bool res1 = BasePage.FindElementByCss(BasePage.DataMaskFirstName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res2 = BasePage.FindElementByCss(BasePage.DataMaskMiddleName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res3 = BasePage.FindElementByCss(BasePage.DataMaskLastName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res4 = BasePage.FindElementByCss(BasePage.DataMaskPrefixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
				bool res5 = BasePage.FindElementByCss(BasePage.DataMaskSuffixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
				bool res6 = BasePage.FindElementByCss(BasePage.DataMaskPatientID).GetAttribute("placeholder").Equals("(auto generated)");
				bool res7 = BasePage.FindElementByCss(BasePage.DataMaskIssuerPatientID).GetAttribute("placeholder").Equals("(removed)");
				bool res8 = new SelectElement(BasePage.Driver.FindElement(By.CssSelector(BasePage.DataMaskGender))).SelectedOption.Text.Equals("Male");
				//  bool res9 = BasePage.FindElementByCss(BasePage.DataMaskDOB).GetAttribute("placeholder").Equals("(auto generated)");
				bool res10 = BasePage.FindElementByCss(BasePage.DataMaskAccessionNo).GetAttribute("placeholder").Equals("(auto generated)");
				bool res11 = BasePage.FindElementByCss(BasePage.DataMaskStudyDescription).GetAttribute("placeholder").Equals("(removed)");
				//  bool res12 = BasePage.FindElementByCss(BasePage.DataMaskStudyDate).GetAttribute("placeholder").Equals("(auto generated)");
				if (res1 && res2 && res3 && res4 && res5 && res6 && res7 && res8 && res10 & res11)
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				//Result for Step-5
				if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].status = "Fail";
				}
				else
				{
					result.steps[ExecutedSteps].status = "Pass";
				}

				//Step6: Do not change any value in Data masking setting and click on Submit button
				((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", BasePage.FindElementByCss(BasePage.DataMaskCancel));
				PageLoadWait.WaitForFrameLoad(10);
				studies.TransferStudy("Local System", SelectallPriors: false, PatientTab: true, isDataMasking: true);
				ExecutedSteps++;

				//Step7_1: Click Download button when the status updated to "Ready"
				PageLoadWait.WaitForDownload(patientNameWithoutDeidentificationValue, Config.downloadpath, "zip");
				if (BasePage.CheckFile(patientNameWithoutDeidentificationValue, Config.downloadpath, "zip"))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("Study Transferred Successfully");
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				login.Logout();

				//Step7_2: Search for these study instances on EA
				var dicomPath = BasePage.ExtractZipFiles(FileName: patientNameWithoutDeidentificationValue).Find(x => x.Contains("F00"));
				String ExtractedFilePID = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.PatientID);
				var hplogin = new HPLogin();
				basePage.DriverGoTo("https://" + Config.EA7 + "/eaweb");
				var hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
				var workflow = (WorkFlow)hphome.Navigate("Workflow");
				if (workflow.EAv12SearchStudy("PatientID", ExtractedFilePID, 1))
				{
					result.steps[ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("Data Masked Study found in EA");
				}
				else
				{
					result.steps[ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				hplogin.LogoutEAv12();
				BasePage.DeleteAllFileFolder(Config.downloadpath);

				//Step8_1: Select the same study and click "Transfer" button
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminUserName, adminPassword);
				studies = (Studies)login.Navigate("Studies");
				studies.RDM_MouseHover(Config.rdm1);
				studies.SearchStudy(patientID: PID, DatasourceList: new string[] { Config.rdm1 + "." + login.GetHostName(Config.EA7) });
				studies.SelectStudy1("Accession", ACC[0]);

				//Step8_2: Update values for Patient information and Study Information and Submit
				var newDataMaskValues = basePage.GetDataMaskFieldsNames();
				newDataMaskValues["PatientID"] = "PID" + System.DateTime.Now.ToString("ddHHmmss");
				newDataMaskValues["Accession"] = "ACC" + System.DateTime.Now.ToString("ddHHmmss");
				basePage.TransferStudy("Local System", waittime:180, SelectallPriors: false, PatientTab: true, isDataMasking: true, DataMaskValues: newDataMaskValues);
				ExecutedSteps++;

				//Step9: Click Download button when the status updated to "Ready"
				PageLoadWait.WaitForDownload("_" + Name, Config.downloadpath, "zip");
				if (BasePage.CheckFile("_" + Name, Config.downloadpath, "zip"))
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("Study Transferred Successfully");
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step10: Unzip file and check the DICOM file values for Patient Information and Study Information
				dicomPath = BasePage.ExtractZipFiles(FileName: "_" + Name).Find(x => x.Contains("F00"));
				ExtractedFilePID = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.PatientID);
				var ExtractedFileACC = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.AccessionNumber);
				if (ExtractedFilePID == newDataMaskValues["PatientID"] && ExtractedFileACC == newDataMaskValues["Accession"])
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step11: Search for this study instance on EA
				login.Logout();
				hplogin = new HPLogin();
				basePage.DriverGoTo("https://" + Config.EA7 + "/eaweb");
				hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
				workflow = (WorkFlow)hphome.Navigate("Workflow");
				if (workflow.EAv12SearchStudy("PatientID", newDataMaskValues["PatientID"], StudiesCount: 1))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				hplogin.LogoutEAv12();
				BasePage.DeleteAllFileFolder(Config.downloadpath);

				//Return result
				result.FinalResult(ExecutedSteps);
				Logger.Instance.InfoLog("Overall Test status--" + result.status);

				return result;
			}
			catch (Exception e)
			{
				//Log Exception
				Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
				result.FinalResult(e, ExecutedSteps);
				Logger.Instance.ErrorLog("Overall Test status--" + result.status);
				return result;
			}
			finally
			{
				try
				{
					BasePage.DeleteAllFileFolder(Config.downloadpath);
					//Deleting uploaded study
					var hplogin = new HPLogin();
					BasePage.Driver.Navigate().GoToUrl("https://" + Config.EA7 + "/eaweb");
					var hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
					var workflow = (WorkFlow)hphome.Navigate("Workflow");
					workflow.EAv12SearchStudy("PatientID", PID, SelectStudy: true);
					workflow.EAv12DeleteStudy();
					hplogin.LogoutEAv12();
				}
				catch (Exception ex)
				{
					Logger.Instance.ErrorLog("Exception in finally Block: " + ex);
				}
			}

		}

		/// <summary>
		///  Data masking from Remote Data source with 'Data masking' enabled - Enterprise viewer
		/// </summary>
		/// <returns></returns>
		public TestCaseResult Test_161321_2(String testid, String teststeps, int stepcount)
		{
			TestCaseResult result = new TestCaseResult(stepcount);
			result.SetTestStepDescription(teststeps);
			int ExecutedSteps = -1;
			Random randomnumber = new Random();
			WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 300));
			string PID = null;
			String adminUserName = Config.adminUserName;
			String adminPassword = Config.adminPassword;
			try
			{
				var studypaths = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
				String[] studypath = studypaths.Split('=');

				//Precondition - Send studies to EA12
				string Name = login.RandomString(4, true) + new System.DateTime().Second + randomnumber.Next(1, 100);
				PID = "PID" + System.DateTime.Now.ToString("ddHHmmss");
				List<string> ACC = new List<string>(); ;
				string filename = string.Empty;
				string[] SOPID = Enumerable.Repeat("1.2.840.113837.390681903." + System.DateTime.Now.ToString("MMddHHmmss"), studypath.Length).ToArray();
				for (int count = 0; count < studypath.Length; count++)
				{
					DicomClient client = new DicomClient();
					ACC.Add("ACC" + System.DateTime.Now.ToString("ddHHmmss") + count);
					string SID = "1.2.840.113837.1845205803." + System.DateTime.Now.ToString("MMddHHmmss") + "." + Config.IConnectIP.Split('.')[3];
					string SED = "1.2.840.113837.4288965276." + System.DateTime.Now.ToString("MMddHHmmss") + "." + Config.IConnectIP.Split('.')[3];
					SOPID[count] = SOPID[count] + ".0" + (count + 1);
					filename = BasePage.WriteDicomFile(studypath[count], new DicomTag[] { DicomTag.SOPInstanceUID, DicomTag.PatientName, DicomTag.PatientID, DicomTag.AccessionNumber, DicomTag.StudyInstanceUID, DicomTag.SeriesInstanceUID, DicomTag.InstanceNumber }, new string[] { SOPID[count], Name, PID, ACC[count], SID, SED, Convert.ToString(count + 1) });
					client.AddRequest(new DicomCStoreRequest(filename));
					client.Send(Config.EA7, 12000, false, "SCU", Config.EA7AETitle);
					Thread.Sleep(2000);//Study sending to EA
				}

				//Step1_1: Login to iCA 7.0 as privileged user (Administrator)
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminUserName, adminPassword);

				// Step1_2:	Make sure user preferences has "Enterprisel viewer" as default viewer
				userpref.OpenUserPreferences();
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
				PageLoadWait.WaitForPageLoad(20);
				userpref.HTML4RadioBtn().Click();
				userpref.CloseUserPreferences();
				ExecutedSteps++;

				//Step2: Navigate to study tab and search a study from VNA data source EA12
				studies = (Studies)login.Navigate("Studies");
				studies.RDM_MouseHover(Config.rdm1);
				studies.SearchStudy(patientID: PID, DatasourceList: new string[] { Config.rdm1 + "." + login.GetHostName(Config.EA7) });
				studies.ChooseColumns(new string[] { "Accession" });
				var Actual = BasePage.GetColumnValues("Accession");
				if (ACC.All(accession => Actual.Contains(accession)))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step3: Select one study and Click transfer button
				studies.SelectStudy1("Accession", ACC[0]);
				studies.TransferBtn().Click();
				PageLoadWait.WaitForFrameLoad(10);
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				if (basePage.IsElementVisible(basePage.By_DataMaskCheckbox()) && (!basePage.DataMaskExamCheckbox().Selected))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("Data Mask Exam checkbox visible and unchecked");
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("Data Mask Exam is invisible or checked");
					result.steps[ExecutedSteps].SetLogs();
					if (!basePage.IsElementVisible(basePage.By_DataMaskCheckbox()))
					{
						throw new Exception("Data Mask checkbox not displayed");
					}
				}

				//Step4: Check Data Mask Exam checkbox and observe the studies in Data masking setting window
				studies.Dropdown_TransferTo().SelectByText("Local System");
				basePage.ClickElement(basePage.DataMaskExamCheckbox());
				basePage.ClickElement(studies.Btn_StudyPageTransferBtn());
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				if (basePage.IsElementVisible(By.CssSelector(BasePage.DataMaskSettingsWindow)))
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				bool res1 = BasePage.FindElementByCss(BasePage.DataMaskFirstName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res2 = BasePage.FindElementByCss(BasePage.DataMaskMiddleName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res3 = BasePage.FindElementByCss(BasePage.DataMaskLastName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res4 = BasePage.FindElementByCss(BasePage.DataMaskPrefixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
				bool res5 = BasePage.FindElementByCss(BasePage.DataMaskSuffixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
				bool res6 = BasePage.FindElementByCss(BasePage.DataMaskPatientID).GetAttribute("placeholder").Equals("(auto generated)");
				bool res7 = BasePage.FindElementByCss(BasePage.DataMaskIssuerPatientID).GetAttribute("placeholder").Equals("(removed)");
				bool res8 = new SelectElement(BasePage.Driver.FindElement(By.CssSelector(BasePage.DataMaskGender))).SelectedOption.Text.Equals("Male");
				//  bool res9 = BasePage.FindElementByCss(BasePage.DataMaskDOB).GetAttribute("placeholder").Equals("(auto generated)");
				bool res10 = BasePage.FindElementByCss(BasePage.DataMaskAccessionNo).GetAttribute("placeholder").Equals("(auto generated)");
				bool res11 = BasePage.FindElementByCss(BasePage.DataMaskStudyDescription).GetAttribute("placeholder").Equals("(removed)");
				//  bool res12 = BasePage.FindElementByCss(BasePage.DataMaskStudyDate).GetAttribute("placeholder").Equals("(auto generated)");
				if (res1 && res2 && res3 && res4 && res5 && res6 && res7 && res8 && res10 & res11)
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				var copyCheckBox = BasePage.FindElementByCss(BasePage.DataMaskCopyCheckbox);
				if (copyCheckBox.Selected && !copyCheckBox.Enabled)
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--'Copy patient attributes to all studies' should be checked and grayed out by default");
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--  'Copy patient attributes to all studies' should be checked and grayed out by default");
					result.steps[ExecutedSteps].SetLogs();
				}
				//Result for Step-4
				if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].status = "Fail";
				}
				else
				{
					result.steps[ExecutedSteps].status = "Pass";
				}

				//Step5: Do not change any value in Data masking setting and click on Submit button
				((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", BasePage.FindElementByCss(BasePage.DataMaskCancel));
				PageLoadWait.WaitForFrameLoad(10);
				studies.TransferStudy("Local System", SelectallPriors: false, PatientTab: true, isDataMasking: true);
				ExecutedSteps++;

				//Step6_1: Click Download button when the status updated to "Ready"
				PageLoadWait.WaitForDownload("_" + Name, Config.downloadpath, "zip");
				if (BasePage.CheckFile("_" + Name, Config.downloadpath, "zip"))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("Study Transferred Successfully");
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				login.Logout();

				//Step6_2: Search for these study instances on EA
				var dicomPath = BasePage.ExtractZipFiles(FileName: "_" + Name).Find(x => x.Contains("F00"));
				String ExtractedFilePID = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.PatientID);
				var hplogin = new HPLogin();
				basePage.DriverGoTo("https://" + Config.EA7 + "/eaweb");
				var hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
				var workflow = (WorkFlow)hphome.Navigate("Workflow");
				if (workflow.EAv12SearchStudy("PatientID", ExtractedFilePID, 1))
				{
					result.steps[ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("Data Masked Study found in EA");
				}
				else
				{
					result.steps[ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				hplogin.LogoutEAv12();
				BasePage.DeleteAllFileFolder(Config.downloadpath);

				//Step7_1: Select the same study and click "Transfer" button
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminUserName, adminPassword);
				studies = (Studies)login.Navigate("Studies");
				studies.RDM_MouseHover(Config.rdm1);
				studies.SearchStudy(patientID: PID, DatasourceList: new string[] { Config.rdm1 + "." + login.GetHostName(Config.EA7) });
				studies.SelectStudy1("Accession", ACC[0]);

				//Step7_2: Update values for Patient information and Study Information and Submit
				var newDataMaskValues = basePage.GetDataMaskFieldsNames();
				newDataMaskValues["PatientID"] = "PID" + System.DateTime.Now.ToString("ddHHmmss");
				newDataMaskValues["Accession"] = "ACC" + System.DateTime.Now.ToString("ddHHmmss");
				basePage.TransferStudy("Local System", waittime:180, SelectallPriors: false, PatientTab: true, isDataMasking: true, DataMaskValues: newDataMaskValues);
				ExecutedSteps++;

				//Step8: Click Download button when the status updated to "Ready"
				PageLoadWait.WaitForDownload("_" + Name, Config.downloadpath, "zip");
				if (BasePage.CheckFile("_" + Name, Config.downloadpath, "zip"))
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("Study Transferred Successfully");
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step9: Unzip file and check the DICOM file values for Patient Information and Study Information
				dicomPath = BasePage.ExtractZipFiles(FileName: "_" + Name).Find(x => x.Contains("F00"));
				ExtractedFilePID = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.PatientID);
				var ExtractedFileACC = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.AccessionNumber);
				if (ExtractedFilePID == newDataMaskValues["PatientID"] && ExtractedFileACC == newDataMaskValues["Accession"])
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step10: Search for this study instance on EA
				login.Logout();
				hplogin = new HPLogin();
				basePage.DriverGoTo("https://" + Config.EA7 + "/eaweb");
				hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
				workflow = (WorkFlow)hphome.Navigate("Workflow");
				if (workflow.EAv12SearchStudy("PatientID", newDataMaskValues["PatientID"], StudiesCount: 1))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				hplogin.LogoutEAv12();
				BasePage.DeleteAllFileFolder(Config.downloadpath);

				//Return result
				result.FinalResult(ExecutedSteps);
				Logger.Instance.InfoLog("Overall Test status--" + result.status);

				return result;
			}
			catch (Exception e)
			{
				//Log Exception
				Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
				result.FinalResult(e, ExecutedSteps);
				Logger.Instance.ErrorLog("Overall Test status--" + result.status);
				return result;
			}
			finally
			{
				try
				{
					BasePage.DeleteAllFileFolder(Config.downloadpath);
					//Deleting uploaded study
					var hplogin = new HPLogin();
					BasePage.Driver.Navigate().GoToUrl("https://" + Config.EA7 + "/eaweb");
					var hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
					var workflow = (WorkFlow)hphome.Navigate("Workflow");
					workflow.EAv12SearchStudy("PatientID", PID, SelectStudy: true);
					workflow.EAv12DeleteStudy();
					hplogin.LogoutEAv12();

					// Change Viewer settings to Universal in User preference 
					login.DriverGoTo(login.url);
					login.LoginIConnect(adminUserName, adminPassword);
					userpref.OpenUserPreferences();
					BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
					PageLoadWait.WaitForPageLoad(20);
					userpref.BluringViewerRadioBtn().Click();
					userpref.CloseUserPreferences();
					login.Logout();
				}
				catch (Exception ex)
				{
					Logger.Instance.ErrorLog("Exception in finally Block: " + ex);
				}
			}

		}
		*/

		/// <summary>
		///  Data masking for VNA with 'Data masking' enabled - Universal viewer
		/// </summary>
		/// <returns></returns>
		public TestCaseResult Test_161930(String testid, String teststeps, int stepcount)
		{
			TestCaseResult result = new TestCaseResult(stepcount);
			result.SetTestStepDescription(teststeps);
			int ExecutedSteps = -1;
			Random randomnumber = new Random();
			WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 300));
			string PID = null;
			String adminUserName = Config.adminUserName;
			String adminPassword = Config.adminPassword;
			try
			{
				var studypaths = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
				String[] studypath = studypaths.Split('=');

				//Precondition - Send studies to EA12
				string Name = login.RandomString(4, true) + new System.DateTime().Second + randomnumber.Next(1, 100);
				PID = "PID" + System.DateTime.Now.ToString("ddHHmmss");
				List<string> ACC = new List<string>(); ;
				string filename = string.Empty;
				string[] SOPID = Enumerable.Repeat("1.2.840.113837.390681903." + System.DateTime.Now.ToString("MMddHHmmss"), studypath.Length).ToArray();
				for (int count = 0; count < studypath.Length; count++)
				{
					DicomClient client = new DicomClient();
					ACC.Add("ACC" + System.DateTime.Now.ToString("ddHHmmss") + count);
					string SID = "1.2.840.113837.1845205803." + System.DateTime.Now.ToString("MMddHHmmss") + "." + Config.IConnectIP.Split('.')[3];
					string SED = "1.2.840.113837.4288965276." + System.DateTime.Now.ToString("MMddHHmmss") + "." + Config.IConnectIP.Split('.')[3];
					SOPID[count] = SOPID[count] + ".0" + (count + 1);
					filename = BasePage.WriteDicomFile(studypath[count], new DicomTag[] { DicomTag.SOPInstanceUID, DicomTag.PatientName, DicomTag.PatientID, DicomTag.AccessionNumber, DicomTag.StudyInstanceUID, DicomTag.SeriesInstanceUID, DicomTag.InstanceNumber }, new string[] { SOPID[count], Name, PID, ACC[count], SID, SED, Convert.ToString(count + 1) });
					client.AddRequest(new DicomCStoreRequest(filename));
					client.Send(Config.EA7, 12000, false, "SCU", Config.EA7AETitle);
					Thread.Sleep(2000);//Study sending to EA
				}

				//Step1: Login to iCA 7.0 as privileged user (Administrator)
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminUserName, adminPassword);
				ExecutedSteps++;

				// Step2:	Make sure user preferences has "New viewer/Universal viewer" as default viewer
				userpref.OpenUserPreferences();
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
				PageLoadWait.WaitForPageLoad(20);
				userpref.BluringViewerRadioBtn().Click();
				userpref.CloseUserPreferences();
				ExecutedSteps++;

				//Step3: Navigate to study tab and search a study from VNA data source EA12
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(patientID: PID, Datasource: Config.vna61);
				studies.ChooseColumns(new string[] { "Accession" });
				var Actual = BasePage.GetColumnValues("Accession");
				if (ACC.All(accession => Actual.Contains(accession)))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step4: Select one study and Click transfer button
				studies.SelectStudy1("Accession", ACC[0]);
				studies.TransferBtn().Click();
				PageLoadWait.WaitForFrameLoad(10);
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				PageLoadWait.WaitForSearchPriorStudiesMessage();
				//try { wait.Until(ExpectedConditions.ElementIsVisible(studies.By_RelatedStudy(2))); }
				//catch (Exception) { }
				if (basePage.IsElementVisible(basePage.By_DataMaskCheckbox()) && (!basePage.DataMaskExamCheckbox().Selected))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("Data Mask Exam checkbox visible and unchecked");
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("Data Mask Exam is invisible or checked");
					result.steps[ExecutedSteps].SetLogs();
					if (!basePage.IsElementVisible(basePage.By_DataMaskCheckbox()))
					{
						throw new Exception("Data Mask checkbox not displayed");
					}
				}

				//Step5: Check Data Mask Exam checkbox and observe the studies in Data masking setting window
				studies.Dropdown_TransferTo().SelectByText("Local System");
				basePage.ClickElement(basePage.DataMaskExamCheckbox());
				basePage.ClickElement(studies.Btn_StudyPageTransferBtn());
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				if (basePage.IsElementVisible(By.CssSelector(BasePage.DataMaskSettingsWindow)))
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				bool res1 = BasePage.FindElementByCss(BasePage.DataMaskFirstName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res2 = BasePage.FindElementByCss(BasePage.DataMaskMiddleName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res3 = BasePage.FindElementByCss(BasePage.DataMaskLastName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res4 = BasePage.FindElementByCss(BasePage.DataMaskPrefixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
				bool res5 = BasePage.FindElementByCss(BasePage.DataMaskSuffixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
				bool res6 = BasePage.FindElementByCss(BasePage.DataMaskPatientID).GetAttribute("placeholder").Equals("(auto generated)");
				bool res7 = BasePage.FindElementByCss(BasePage.DataMaskIssuerPatientID).GetAttribute("placeholder").Equals("(removed)");
				bool res8 = new SelectElement(BasePage.Driver.FindElement(By.CssSelector(BasePage.DataMaskGender))).SelectedOption.Text.Equals("Male");
				//  bool res9 = BasePage.FindElementByCss(BasePage.DataMaskDOB).GetAttribute("placeholder").Equals("(auto generated)");
				bool res10 = BasePage.FindElementByCss(BasePage.DataMaskAccessionNo).GetAttribute("placeholder").Equals("(auto generated)");
				bool res11 = BasePage.FindElementByCss(BasePage.DataMaskStudyDescription).GetAttribute("placeholder").Equals("(removed)");
				//  bool res12 = BasePage.FindElementByCss(BasePage.DataMaskStudyDate).GetAttribute("placeholder").Equals("(auto generated)");
				if (res1 && res2 && res3 && res4 && res5 && res6 && res7 && res8 && res10 & res11)
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				var copyCheckBox = BasePage.FindElementByCss(BasePage.DataMaskCopyCheckbox);
				if (copyCheckBox.Selected && !copyCheckBox.Enabled)
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--'Copy patient attributes to all studies' should be checked and grayed out by default");
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--  'Copy patient attributes to all studies' should be checked and grayed out by default");
					result.steps[ExecutedSteps].SetLogs();
				}
				//Result for Step-5
				if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].status = "Fail";
				}
				else
				{
					result.steps[ExecutedSteps].status = "Pass";
				}

				//Step6: Do not change any value in Data masking setting and click on Submit button
				((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", BasePage.FindElementByCss(BasePage.DataMaskCancel));
				PageLoadWait.WaitForFrameLoad(10);
				studies.TransferStudy("Local System", SelectallPriors: false, PatientTab: true, isDataMasking: true);
				ExecutedSteps++;

				//Step7_1: Click Download button when the status updated to "Ready"
				PageLoadWait.WaitForDownload(patientNameWithoutDeidentificationValue, Config.downloadpath, "zip");
				if (BasePage.CheckFile(patientNameWithoutDeidentificationValue, Config.downloadpath, "zip"))
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("Study Transferred Successfully");
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				login.Logout();

				//Step7_2: Search for these study instances on EA
				var dicomPath = BasePage.ExtractZipFiles(FileName: patientNameWithoutDeidentificationValue).Find(x => x.Contains("F00"));
				String ExtractedFilePID = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.PatientID);
				var hplogin = new HPLogin();
				basePage.DriverGoTo("https://" + Config.EA7 + "/eaweb");
				var hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
				var workflow = (WorkFlow)hphome.Navigate("Workflow");
				if (workflow.EAv12SearchStudy("PatientID", ExtractedFilePID, 1))
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("Data Masked Study found in EA");
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].status = "Fail";
				}
				else
				{
					result.steps[ExecutedSteps].status = "Pass";
				}
				hplogin.LogoutEAv12();
				BasePage.DeleteAllFileFolder(Config.downloadpath);

				//Step8_1: Select the same study and click "Transfer" button
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminUserName, adminPassword);
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(patientID: PID, Datasource: Config.vna61);
				studies.SelectStudy1("Accession", ACC[0]);

				//Step8_2: Update values for Patient information and Study Information and Submit
				var newDataMaskValues = basePage.GetDataMaskFieldsNames();
				newDataMaskValues["LastName"] = login.RandomString(5, true);
				newDataMaskValues["PatientID"] = "PID" + System.DateTime.Now.ToString("ddHHmmss");
				newDataMaskValues["Accession"] = "ACC" + System.DateTime.Now.ToString("ddHHmmss");
				basePage.TransferStudy("Local System", waittime:180, SelectallPriors: false, PatientTab: true, isDataMasking: true, DataMaskValues: newDataMaskValues);
				ExecutedSteps++;

				//Step9: Click Download button when the status updated to "Ready"
				PageLoadWait.WaitForDownload("_" + newDataMaskValues["LastName"], Config.downloadpath, "zip");
				if (BasePage.CheckFile("_" + newDataMaskValues["LastName"], Config.downloadpath, "zip"))
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("Study Transferred Successfully");
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step10: Unzip file and check the DICOM file values for Patient Information and Study Information
				dicomPath = BasePage.ExtractZipFiles(FileName: "_" + newDataMaskValues["LastName"]).Find(x => x.Contains("F00"));
				ExtractedFilePID = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.PatientID);
				var ExtractedFileACC = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.AccessionNumber);
				if (ExtractedFilePID == newDataMaskValues["PatientID"] && ExtractedFileACC == newDataMaskValues["Accession"])
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step11: Search for this study instance on EA
				login.Logout();
				hplogin = new HPLogin();
				basePage.DriverGoTo("https://" + Config.EA7 + "/eaweb");
				hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
				workflow = (WorkFlow)hphome.Navigate("Workflow");
				if (workflow.EAv12SearchStudy("PatientID", newDataMaskValues["PatientID"], StudiesCount: 1))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				hplogin.LogoutEAv12();
				BasePage.DeleteAllFileFolder(Config.downloadpath);
			/*
				//Return result
				result.FinalResult(ExecutedSteps);
				Logger.Instance.InfoLog("Overall Test status--" + result.status);

				return result;
			}
			catch (Exception e)
			{
				//Log Exception
				Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
				result.FinalResult(e, ExecutedSteps);
				Logger.Instance.ErrorLog("Overall Test status--" + result.status);
				return result;
			}
			finally
			{
				try
				{
					BasePage.DeleteAllFileFolder(Config.downloadpath);					

					//Deleting uploaded study
					var hplogin = new HPLogin();
					BasePage.Driver.Navigate().GoToUrl("https://" + Config.EA7 + "/eaweb");
					var hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
					var workflow = (WorkFlow)hphome.Navigate("Workflow");
					workflow.EAv12SearchStudy("PatientID", PID, SelectStudy: true);
					workflow.EAv12DeleteStudy();
					hplogin.LogoutEAv12();
				}
				catch (Exception ex)
				{
					Logger.Instance.ErrorLog("Exception in finally Block: " + ex);
				}
			}

		}
	
		/// <summary>
		///  Data masking for VNA with 'Data masking' enabled - Enterprise viewer
		/// </summary>
		/// <returns></returns>
		public TestCaseResult Test_161930_2(String testid, String teststeps, int stepcount)
		{
			TestCaseResult result = new TestCaseResult(stepcount);
			result.SetTestStepDescription(teststeps);
			int ExecutedSteps = -1;
			Random randomnumber = new Random();
			WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 300));
			string PID = null;
			
			try
			{
				var studypaths = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
				String[] studypath = studypaths.Split('=');
				
				//Precondition - Send studies to EA12
				string Name2 = login.RandomString(4, true) + new System.DateTime().Second + randomnumber.Next(1, 100);
				PID = "PID" + System.DateTime.Now.ToString("ddHHmmss");
				List<string> ACC2 = new List<string>(); ;
				string filename2 = string.Empty;
				string[] SOPID2 = Enumerable.Repeat("1.2.840.113837.390681903." + System.DateTime.Now.ToString("MMddHHmmss"), studypath.Length).ToArray();
				for (int count = 0; count < studypath.Length; count++)
				{
					DicomClient client = new DicomClient();
					ACC2.Add("ACC" + System.DateTime.Now.ToString("ddHHmmss") + count);
					string SID = "1.2.840.113837.1845205803." + System.DateTime.Now.ToString("MMddHHmmss") + "." + Config.IConnectIP.Split('.')[3];
					string SED = "1.2.840.113837.4288965276." + System.DateTime.Now.ToString("MMddHHmmss") + "." + Config.IConnectIP.Split('.')[3];
					SOPID2[count] = SOPID2[count] + ".0" + (count + 1);
					filename2 = BasePage.WriteDicomFile(studypath[count], new DicomTag[] { DicomTag.SOPInstanceUID, DicomTag.PatientName, DicomTag.PatientID, DicomTag.AccessionNumber, DicomTag.StudyInstanceUID, DicomTag.SeriesInstanceUID, DicomTag.InstanceNumber }, new string[] { SOPID2[count], Name2, PID, ACC2[count], SID, SED, Convert.ToString(count + 1) });
					client.AddRequest(new DicomCStoreRequest(filename2));
					client.Send(Config.EA7, 12000, false, "SCU", Config.EA7AETitle);
					Thread.Sleep(2000);//Study sending to EA
				}
				*/
				//Step1_1: Login to iCA 7.0 as privileged user (Administrator)
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminUserName, adminPassword);

				// Step1_2:	Make sure user preferences has "Enterprisel viewer" as default viewer
				userpref.OpenUserPreferences();
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
				PageLoadWait.WaitForPageLoad(20);
				userpref.HTML4RadioBtn().Click();
				userpref.CloseUserPreferences();
				ExecutedSteps++;

				//Step2: Navigate to study tab and search a study from VNA data source EA12
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(patientID: PID, Datasource: Config.vna61);
				studies.ChooseColumns(new string[] { "Accession" });
				Actual = BasePage.GetColumnValues("Accession");
				if (ACC.All(accession => Actual.Contains(accession)))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step3: Select one study and Click transfer button
				studies.SelectStudy1("Accession", ACC[0]);
				studies.TransferBtn().Click();
				PageLoadWait.WaitForFrameLoad(10);
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				PageLoadWait.WaitForSearchPriorStudiesMessage();
				//try { wait.Until(ExpectedConditions.ElementIsVisible(studies.By_RelatedStudy(2))); }
				//catch (Exception) { }
				if (basePage.IsElementVisible(basePage.By_DataMaskCheckbox()) && (!basePage.DataMaskExamCheckbox().Selected))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("Data Mask Exam checkbox visible and unchecked");
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("Data Mask Exam is invisible or checked");
					result.steps[ExecutedSteps].SetLogs();
					if (!basePage.IsElementVisible(basePage.By_DataMaskCheckbox()))
					{
						throw new Exception("Data Mask checkbox not displayed");
					}
				}

				//Step4: Check Data Mask Exam checkbox and observe the studies in Data masking setting window
				studies.Dropdown_TransferTo().SelectByText("Local System");
				basePage.ClickElement(basePage.DataMaskExamCheckbox());
				basePage.ClickElement(studies.Btn_StudyPageTransferBtn());
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				if (basePage.IsElementVisible(By.CssSelector(BasePage.DataMaskSettingsWindow)))
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				res1 = BasePage.FindElementByCss(BasePage.DataMaskFirstName).GetAttribute("placeholder").Equals("(auto generated)");
				res2 = BasePage.FindElementByCss(BasePage.DataMaskMiddleName).GetAttribute("placeholder").Equals("(auto generated)");
				res3 = BasePage.FindElementByCss(BasePage.DataMaskLastName).GetAttribute("placeholder").Equals("(auto generated)");
				res4 = BasePage.FindElementByCss(BasePage.DataMaskPrefixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
				res5 = BasePage.FindElementByCss(BasePage.DataMaskSuffixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
				res6 = BasePage.FindElementByCss(BasePage.DataMaskPatientID).GetAttribute("placeholder").Equals("(auto generated)");
				res7 = BasePage.FindElementByCss(BasePage.DataMaskIssuerPatientID).GetAttribute("placeholder").Equals("(removed)");
				res8 = new SelectElement(BasePage.Driver.FindElement(By.CssSelector(BasePage.DataMaskGender))).SelectedOption.Text.Equals("Male");
				// res9 = BasePage.FindElementByCss(BasePage.DataMaskDOB).GetAttribute("placeholder").Equals("(auto generated)");
				res10 = BasePage.FindElementByCss(BasePage.DataMaskAccessionNo).GetAttribute("placeholder").Equals("(auto generated)");
				res11 = BasePage.FindElementByCss(BasePage.DataMaskStudyDescription).GetAttribute("placeholder").Equals("(removed)");
				// res12 = BasePage.FindElementByCss(BasePage.DataMaskStudyDate).GetAttribute("placeholder").Equals("(auto generated)");
				if (res1 && res2 && res3 && res4 && res5 && res6 && res7 && res8 && res10 & res11)
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				copyCheckBox = BasePage.FindElementByCss(BasePage.DataMaskCopyCheckbox);
				if (copyCheckBox.Selected && !copyCheckBox.Enabled)
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--'Copy patient attributes to all studies' should be checked and grayed out by default");
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--  'Copy patient attributes to all studies' should be checked and grayed out by default");
					result.steps[ExecutedSteps].SetLogs();
				}
				//Result for Step-4
				if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].status = "Fail";
				}
				else
				{
					result.steps[ExecutedSteps].status = "Pass";
				}

				//Step5: Do not change any value in Data masking setting and click on Submit button
				((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", BasePage.FindElementByCss(BasePage.DataMaskCancel));
				PageLoadWait.WaitForFrameLoad(10);
				studies.TransferStudy("Local System", SelectallPriors: false, PatientTab: true, isDataMasking: true);
				ExecutedSteps++;

				//Step6_1: Click Download button when the status updated to "Ready"
				PageLoadWait.WaitForDownload(patientNameWithoutDeidentificationValue, Config.downloadpath, "zip");
				if (BasePage.CheckFile(patientNameWithoutDeidentificationValue, Config.downloadpath, "zip"))
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("Study Transferred Successfully");
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				login.Logout();

				//Step6_2: Search for these study instances on EA
				dicomPath = BasePage.ExtractZipFiles(FileName: patientNameWithoutDeidentificationValue).Find(x => x.Contains("F00"));
				ExtractedFilePID = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.PatientID);
				hplogin = new HPLogin();
				basePage.DriverGoTo("https://" + Config.EA7 + "/eaweb");
				hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
				workflow = (WorkFlow)hphome.Navigate("Workflow");
				if (workflow.EAv12SearchStudy("PatientID", ExtractedFilePID, 1))
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("Data Masked Study found in EA");
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				hplogin.LogoutEAv12();
				BasePage.DeleteAllFileFolder(Config.downloadpath);
				
				//Result for Step-6
				if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].status = "Fail";
				}
				else
				{
					result.steps[ExecutedSteps].status = "Pass";
				}

				//Step7_1: Select the same study and click "Transfer" button
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminUserName, adminPassword);
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(patientID: PID, Datasource: Config.vna61);
				studies.SelectStudy1("Accession", ACC[0]);

				//Step7_2: Update values for Patient information and Study Information and Submit
				newDataMaskValues = basePage.GetDataMaskFieldsNames();
				newDataMaskValues["LastName"] = login.RandomString(5, true);
				newDataMaskValues["PatientID"] = "PID" + System.DateTime.Now.ToString("ddHHmmss");
				newDataMaskValues["Accession"] = "ACC" + System.DateTime.Now.ToString("ddHHmmss");
				basePage.TransferStudy("Local System", waittime:180, SelectallPriors: false, PatientTab: true, isDataMasking: true, DataMaskValues: newDataMaskValues);
				ExecutedSteps++;

				//Step8: Click Download button when the status updated to "Ready"
				PageLoadWait.WaitForDownload("_" + newDataMaskValues["LastName"], Config.downloadpath, "zip");
				if (BasePage.CheckFile("_" + newDataMaskValues["LastName"], Config.downloadpath, "zip"))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("Study Transferred Successfully");
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step9: Unzip file and check the DICOM file values for Patient Information and Study Information
				dicomPath = BasePage.ExtractZipFiles(FileName: "_" + newDataMaskValues["LastName"]).Find(x => x.Contains("F00"));
				ExtractedFilePID = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.PatientID);
				ExtractedFileACC = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.AccessionNumber);
				if (ExtractedFilePID == newDataMaskValues["PatientID"] && ExtractedFileACC == newDataMaskValues["Accession"])
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step10: Search for this study instance on EA
				login.Logout();
				hplogin = new HPLogin();
				basePage.DriverGoTo("https://" + Config.EA7 + "/eaweb");
				hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
				workflow = (WorkFlow)hphome.Navigate("Workflow");
				if (workflow.EAv12SearchStudy("PatientID", newDataMaskValues["PatientID"], StudiesCount: 1))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				hplogin.LogoutEAv12();
				BasePage.DeleteAllFileFolder(Config.downloadpath);

				//Return result
				result.FinalResult(ExecutedSteps);
				Logger.Instance.InfoLog("Overall Test status--" + result.status);

				return result;
			}
			catch (Exception e)
			{
				//Log Exception
				Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
				result.FinalResult(e, ExecutedSteps);
				Logger.Instance.ErrorLog("Overall Test status--" + result.status);
				return result;
			}
			finally
			{
				try
				{
					BasePage.DeleteAllFileFolder(Config.downloadpath);
					//Deleting uploaded study
					var hplogin = new HPLogin();
					BasePage.Driver.Navigate().GoToUrl("https://" + Config.EA7 + "/eaweb");
					var hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
					var workflow = (WorkFlow)hphome.Navigate("Workflow");
					workflow.EAv12SearchStudy("PatientID", PID, SelectStudy: true);
					workflow.EAv12DeleteStudy();
					hplogin.LogoutEAv12();

					// Change Viewer settings to Universal in User preference 
					login.DriverGoTo(login.url);
					login.LoginIConnect(adminUserName, adminPassword);
					userpref.OpenUserPreferences();
					BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
					PageLoadWait.WaitForPageLoad(20);
					userpref.BluringViewerRadioBtn().Click();
					userpref.CloseUserPreferences();
					login.Logout();
				}
				catch (Exception ex)
				{
					Logger.Instance.ErrorLog("Exception in finally Block: " + ex);
				}
			}

		}

		/// <summary>
		/// Data masking in Integrator mode on desktop WITHOUT user sharing when default viewer is HTML4 (Old viewer)
		/// </summary>
		public TestCaseResult Test_161459(String testid, String teststeps, int stepcount)
		{
			//Declare and initialize variables
			TestCaseResult result;
			result = new TestCaseResult(stepcount);
			int ExecutedSteps = -1;
			StudyViewer studyviewer = null;
			Studies studies = null;
			login = new Login();
			WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 300));

			//Set up Validation Steps
			result.SetTestStepDescription(teststeps);
			string PID = null;
			try
			{
				String adminUserName = Config.adminUserName;
				String viewerName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Viewer");
				var studypaths = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
				String[] studypath = studypaths.Split('=');

				//Precondition - Send studies to EA12
				string Name = login.RandomString(4, true) + new System.DateTime().Second + randomnumber.Next(1, 100);
				PID = "PID" + System.DateTime.Now.ToString("ddHHmmss");
				List<string> ACC = new List<string>(); ;
				string filename = string.Empty;
				string[] SOPID = Enumerable.Repeat("1.2.840.113837.390681903." + System.DateTime.Now.ToString("MMddHHmmss"), studypath.Length).ToArray();
				for (int count = 0; count < 2; count++)
				{
					DicomClient client = new DicomClient();
					ACC.Add("ACC" + System.DateTime.Now.ToString("ddHHmmss") + count);
					string SID = "1.2.840.113837.1845205803." + System.DateTime.Now.ToString("MMddHHmmss") + "." + Config.IConnectIP.Split('.')[3];
					string SED = "1.2.840.113837.4288965276." + System.DateTime.Now.ToString("MMddHHmmss") + "." + Config.IConnectIP.Split('.')[3];
					SOPID[count] = SOPID[count] + ".0" + (count + 1);
					filename = BasePage.WriteDicomFile(studypath[count], new DicomTag[] { DicomTag.SOPInstanceUID, DicomTag.PatientName, DicomTag.PatientID, DicomTag.AccessionNumber, DicomTag.StudyInstanceUID, DicomTag.SeriesInstanceUID, DicomTag.InstanceNumber }, new string[] { SOPID[count], Name, PID, ACC[count], SID, SED, Convert.ToString(count + 1) });
					client.AddRequest(new DicomCStoreRequest(filename));
					client.Send(Config.EA7, 12000, false, "SCU", Config.EA7AETitle);
					Thread.Sleep(2000);//Study sending to EA
				}

				//Initial Set-up
				servicetool.LaunchServiceTool();
				wpfobject.WaitTillLoad();
				servicetool.NavigateToTab("Integrator");
				wpfobject.WaitTillLoad();
				servicetool.EanbleUserSharing_ShadowUser(usersharing: "Always disabled");
				
				// Set Enterprise as default viewer
				wpfobject.WaitTillLoad();
				servicetool.EnableHTML5(HTML5DefaultMode: false, EnableHTML5: true);
				servicetool.CloseServiceTool();

				//login = new Login();
				//login.DriverGoTo(login.url);
				//login.LoginIConnect(Config.adminUserName, Config.adminPassword);
				//studies = login.Navigate<Studies>();
				//studies.SelectAllDateAndData();
				//studies.ClickSearchBtn();
				//login.Logout();

				//Step 1 - Launch the TestEHR application from iCA server C:\WebAccess\WebAccess\bin
				ehr.LaunchEHR();
				//Validate whether EHR application is launched or not
				if (WpfObjects._mainWindow.Visible)
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

				//Step 2 - Enter address http://iCA IP>/WebAccess/ , username-password, Security ID and update show selector to "False"
				wpfobject.GetMainWindow("Test WebAccess EHR");
				wpfobject.SelectTabFromTabItems("Image Load");
				wpfobject.WaitTillLoad();
				ehr.SetCommonParameters(user: adminUserName);
				ehr.SetSelectorOptions(showSelector: "False", enableDownlaod: "True", enableTransfer: "True");
				ExecutedSteps++;

				//Step 3 - Enter the Patient ID of the study that belongs to DS1  and click CMD line
				ehr.SetSearchKeys_Study(ACC[0], datasources: login.GetHostName(Config.EA7));
				String url_2 = ehr.clickCmdLine("ImageLoad");
				ehr.CloseEHR();
				ExecutedSteps++;

				//Step 4 - Load the generated URL in browser
				//Close browser
				basePage.CreateNewSesion();
				login = new Login();
				login.NavigateToIntegratorURL(url_2);
				studyviewer = (StudyViewer)login.NavigateToIntegratorFrame();
				PageLoadWait.WaitForThumbnailsToLoad(40);
				PageLoadWait.WaitForAllViewportsToLoad(40);
				if (studyviewer.PatientDetailsInViewer()["PatientID"].ToLower().Equals(PID.ToLower()))
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

				//Step 5 - Click "transfer study" button from toolbar
				new StudyViewer().SelectToolInToolBar(IEnum.ViewerTools.TransferStudy);
				PageLoadWait.WaitForFrameLoad(10);
				ExecutedSteps++;

				//Step 6 - Checkbox "Data mask exams" should appear and is unchecked by default.
				if (basePage.IsElementVisible(basePage.By_DataMaskCheckbox()) && (!basePage.DataMaskExamCheckbox().Selected))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("Data Mask Exam checkbox visible and unchecked");
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("Data Mask Exam is invisible or checked");
					result.steps[ExecutedSteps].SetLogs();
					if (!basePage.IsElementVisible(basePage.By_DataMaskCheckbox()))
					{
						throw new Exception("Data Mask checkbox not displayed");
					}
				}				

				//Step 7 - Check Data Mask Exam checkbox and observe the studies in Data masking setting window
				//studies.Dropdown_TransferTo().SelectByText("Local System");
				studies = new Studies();
				studies.Transfer("Local System", ReviewTool: true, isDataMasking: true, FrameName: IntegratedFrameName);
				if (basePage.IsElementVisible(By.CssSelector(BasePage.DataMaskSettingsWindow)))
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
					throw new Exception("Data Mask settings window not displayed");
				}
				bool res1 = BasePage.FindElementByCss(BasePage.DataMaskFirstName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res2 = BasePage.FindElementByCss(BasePage.DataMaskMiddleName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res3 = BasePage.FindElementByCss(BasePage.DataMaskLastName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res4 = BasePage.FindElementByCss(BasePage.DataMaskPrefixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
				bool res5 = BasePage.FindElementByCss(BasePage.DataMaskSuffixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
				bool res6 = BasePage.FindElementByCss(BasePage.DataMaskPatientID).GetAttribute("placeholder").Equals("(auto generated)");
				bool res7 = BasePage.FindElementByCss(BasePage.DataMaskIssuerPatientID).GetAttribute("placeholder").Equals("(removed)");
				bool res8 = new SelectElement(BasePage.Driver.FindElement(By.CssSelector(BasePage.DataMaskGender))).SelectedOption.Text.Equals("Male");
				//  bool res9 = BasePage.FindElementByCss(BasePage.DataMaskDOB).GetAttribute("placeholder").Equals("(auto generated)");
				bool res10 = BasePage.FindElementByCss(BasePage.DataMaskAccessionNo).GetAttribute("placeholder").Equals("(auto generated)");
				bool res11 = BasePage.FindElementByCss(BasePage.DataMaskStudyDescription).GetAttribute("placeholder").Equals("(removed)");
				//  bool res12 = BasePage.FindElementByCss(BasePage.DataMaskStudyDate).GetAttribute("placeholder").Equals("(auto generated)");
				if (res1 && res2 && res3 && res4 && res5 && res6 && res7 && res8 && res10 & res11)
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				var copyCheckBox = BasePage.FindElementByCss(BasePage.DataMaskCopyCheckbox);
				if (copyCheckBox.Selected && !copyCheckBox.Enabled)
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--'Copy patient attributes to all studies' should be checked and grayed out by default");
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--  'Copy patient attributes to all studies' should be checked and grayed out by default");
					result.steps[ExecutedSteps].SetLogs();
				}
				//Result for Step-7
				if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].status = "Fail";
				}
				else
				{
					result.steps[ExecutedSteps].status = "Pass";
				}

				//Step 8 - Do not change any value in Data masking setting and click on Submit button
				((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", BasePage.FindElementByCss(BasePage.DataMaskCancel));				
				studyviewer = (StudyViewer)login.NavigateToIntegratorFrame();
				PageLoadWait.WaitForThumbnailsToLoad(40);
				PageLoadWait.WaitForAllViewportsToLoad(40);
				studies.TransferStudy("Local System", SelectallPriors: false, ReviewTool: true, isDataMasking: true, FrameName: IntegratedFrameName);
				ExecutedSteps++;

				//Step 9_1 - Click Download button when the status updated to "Ready"
				PageLoadWait.WaitForDownload(patientNameWithoutDeidentificationValue, Config.downloadpath, "zip");
				if (BasePage.CheckFile(patientNameWithoutDeidentificationValue, Config.downloadpath, "zip"))
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("Study Transferred Successfully");
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				//login.Logout();

				//Step 9_2 - Search for these study instances on EA
				var dicomPath = BasePage.ExtractZipFiles(FileName: patientNameWithoutDeidentificationValue).Find(x => x.Contains("F00"));
				String ExtractedFilePID = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.PatientID);
				var hplogin = new HPLogin();
				basePage.DriverGoTo("https://" + Config.EA7 + "/eaweb");
				var hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
				var workflow = (WorkFlow)hphome.Navigate("Workflow");
				if (workflow.EAv12SearchStudy("PatientID", ExtractedFilePID, 1))
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("Data Masked Study found in EA");
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				hplogin.LogoutEAv12();
				BasePage.DeleteAllFileFolder(Config.downloadpath);

				//Result for Step-9
				if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].status = "Fail";
				}
				else
				{
					result.steps[ExecutedSteps].status = "Pass";
				}

				//Step10_1: Select the same study and click "Transfer" button
				//Close browser
				basePage.CreateNewSesion();
				login.NavigateToIntegratorURL(url_2);
				studyviewer = (StudyViewer)login.NavigateToIntegratorFrame();
				PageLoadWait.WaitForThumbnailsToLoad(40);
				PageLoadWait.WaitForAllViewportsToLoad(40);

				//Step10_2: Update values for Patient information and Study Information and Submit
				var newDataMaskValues = basePage.GetDataMaskFieldsNames();
				newDataMaskValues["LastName"] = login.RandomString(5, true);
				newDataMaskValues["PatientID"] = "PID" + System.DateTime.Now.ToString("ddHHmmss");
				newDataMaskValues["Accession"] = "ACC" + System.DateTime.Now.ToString("ddHHmmss");
			
				studies.TransferStudy("Local System", SelectallPriors: false, ReviewTool: true, isDataMasking: true, DataMaskValues: newDataMaskValues, FrameName: IntegratedFrameName);
				ExecutedSteps++;

				//Step11: Click Download button when the status updated to "Ready"
				PageLoadWait.WaitForDownload("_" + newDataMaskValues["LastName"], Config.downloadpath, "zip");
				if (BasePage.CheckFile("_" + newDataMaskValues["LastName"], Config.downloadpath, "zip"))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("Study Transferred Successfully");
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step12: Unzip file and check the DICOM file values for Patient Information and Study Information
				dicomPath = BasePage.ExtractZipFiles(FileName: "_" + newDataMaskValues["LastName"]).Find(x => x.Contains("F00"));
				ExtractedFilePID = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.PatientID);
				var ExtractedFileACC = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.AccessionNumber);
				if (ExtractedFilePID == newDataMaskValues["PatientID"] && ExtractedFileACC == newDataMaskValues["Accession"])
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step13: Search for this study instance on EA
				login.Logout();
				hplogin = new HPLogin();
				basePage.DriverGoTo("https://" + Config.EA7 + "/eaweb");
				hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
				workflow = (WorkFlow)hphome.Navigate("Workflow");
				if (workflow.EAv12SearchStudy("PatientID", newDataMaskValues["PatientID"], StudiesCount: 1))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				hplogin.LogoutEAv12();
				BasePage.DeleteAllFileFolder(Config.downloadpath);

				

				//Report Result
				result.FinalResult(ExecutedSteps);
				Logger.Instance.InfoLog("Overall Test status--" + result.status);
				//Return Result
				GC.Collect();
				return result;
			}
			catch (Exception e)
			{
				//Log exception
				Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);
				//Close browser
				basePage.CreateNewSesion();

				//Report Result
				result.FinalResult(e, ExecutedSteps);
				Logger.Instance.InfoLog("Overall Test status--" + result.status);

				//Return Result
				return result;
			}
			finally
			{
				try
				{
					BasePage.DeleteAllFileFolder(Config.downloadpath);

					// Set default viewer as bluring viewer in service tool
					servicetool.LaunchServiceTool();
					wpfobject.WaitTillLoad();
					servicetool.EnableHTML5(HTML5DefaultMode: true, EnableHTML5: true);
					servicetool.CloseServiceTool();

					//Close browser
					basePage.CreateNewSesion();
					
					//Deleting uploaded study
					var hplogin = new HPLogin();
					BasePage.Driver.Navigate().GoToUrl("https://" + Config.EA7 + "/eaweb");
					var hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
					var workflow = (WorkFlow)hphome.Navigate("Workflow");
					workflow.EAv12SearchStudy("PatientID", PID, SelectStudy: true);
					workflow.EAv12DeleteStudy();
					hplogin.LogoutEAv12();

				}
				catch (Exception ex)
				{
					Logger.Instance.ErrorLog("Exception in finally Block: " + ex);
				}
			}
		}

		/// <summary>
		/// Data masking in Integrator mode on desktop WITH user sharing when default viewer is HTML4 (Old viewer)
		/// </summary>
		public TestCaseResult Test_161929(String testid, String teststeps, int stepcount)
		{
			//Declare and initialize variables
			TestCaseResult result;
			result = new TestCaseResult(stepcount);
			int ExecutedSteps = -1;
			StudyViewer studyviewer = null;
			Studies studies = null;
			login = new Login();
			//Set up Validation Steps
			result.SetTestStepDescription(teststeps);
			string PID = null;
			try
			{
				String adminUserName = Config.adminUserName;
				String viewerName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Viewer");
				var studypaths = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
				String[] studypath = studypaths.Split('=');

				//Precondition - Send studies to EA12
				string Name = login.RandomString(4, true) + new System.DateTime().Second + randomnumber.Next(1, 100);
				PID = "PID" + System.DateTime.Now.ToString("ddHHmmss");
				List<string> ACC = new List<string>(); ;
				string filename = string.Empty;
				string[] SOPID = Enumerable.Repeat("1.2.840.113837.390681903." + System.DateTime.Now.ToString("MMddHHmmss"), studypath.Length).ToArray();
				for (int count = 0; count < 2; count++)
				{
					DicomClient client = new DicomClient();
					ACC.Add("ACC" + System.DateTime.Now.ToString("ddHHmmss") + count);
					string SID = "1.2.840.113837.1845205803." + System.DateTime.Now.ToString("MMddHHmmss") + "." + Config.IConnectIP.Split('.')[3];
					string SED = "1.2.840.113837.4288965276." + System.DateTime.Now.ToString("MMddHHmmss") + "." + Config.IConnectIP.Split('.')[3];
					SOPID[count] = SOPID[count] + ".0" + (count + 1);
					filename = BasePage.WriteDicomFile(studypath[count], new DicomTag[] { DicomTag.SOPInstanceUID, DicomTag.PatientName, DicomTag.PatientID, DicomTag.AccessionNumber, DicomTag.StudyInstanceUID, DicomTag.SeriesInstanceUID, DicomTag.InstanceNumber }, new string[] { SOPID[count], Name, PID, ACC[count], SID, SED, Convert.ToString(count + 1) });
					client.AddRequest(new DicomCStoreRequest(filename));
					client.Send(Config.EA7, 12000, false, "SCU", Config.EA7AETitle);
					Thread.Sleep(2000);//Study sending to EA
				}

				//Initial Set-up
				servicetool.LaunchServiceTool();
				wpfobject.WaitTillLoad();
				servicetool.NavigateToTab("Integrator");
				wpfobject.WaitTillLoad();
				servicetool.EanbleUserSharing_ShadowUser(usersharing: "Always enabled");
				wpfobject.WaitTillLoad();
				servicetool.CloseServiceTool();

				// Set Enterprise as default viewer in userpreference
				login = new Login();
				login.DriverGoTo(login.url);
				login.LoginIConnect(Config.adminUserName, Config.adminPassword);
				userpref.OpenUserPreferences();
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
				PageLoadWait.WaitForPageLoad(20);
				userpref.HTML4RadioBtn().Click();
				userpref.CloseUserPreferences();
				login.Logout();							

				//Step 1 - Launch the TestEHR application from iCA server C:\WebAccess\WebAccess\bin
				ehr.LaunchEHR();
				if (WpfObjects._mainWindow.Visible)
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

				//Step 2 - Enter address http://iCA IP>/WebAccess/ , username-password, Security ID and update show selector to "False"
				wpfobject.GetMainWindow("Test WebAccess EHR");
				wpfobject.SelectTabFromTabItems("Image Load");
				wpfobject.WaitTillLoad();
				ehr.SetCommonParameters(user: adminUserName);
				ehr.SetSelectorOptions(showSelector: "False", enableDownlaod: "True", enableTransfer: "True");
				ExecutedSteps++;

				//Step 3 - Enter the Patient ID of the study that belongs to DS1  and click CMD line
				ehr.SetSearchKeys_Study(ACC[0], datasources: login.GetHostName(Config.EA7));
				String url_2 = ehr.clickCmdLine("ImageLoad");
				ehr.CloseEHR();
				ExecutedSteps++;

				//Step 4 - Load the generated URL in browser
				//Close browser
				basePage.CreateNewSesion();
				login = new Login();
				login.NavigateToIntegratorURL(url_2);
				studyviewer = (StudyViewer)login.NavigateToIntegratorFrame();
				PageLoadWait.WaitForThumbnailsToLoad(40);
				PageLoadWait.WaitForAllViewportsToLoad(40);
				if (studyviewer.PatientDetailsInViewer()["PatientID"].ToLower().Equals(PID.ToLower()))
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

				//Step 5 - Click "transfer study" button from toolbar
				new StudyViewer().SelectToolInToolBar(IEnum.ViewerTools.TransferStudy);
				PageLoadWait.WaitForFrameLoad(10);
				ExecutedSteps++;

				//Step 6 - Checkbox "Data mask exams" should appear and is unchecked by default.
				if (basePage.IsElementVisible(basePage.By_DataMaskCheckbox()) && (!basePage.DataMaskExamCheckbox().Selected))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("Data Mask Exam checkbox visible and unchecked");
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("Data Mask Exam is invisible or checked");
					result.steps[ExecutedSteps].SetLogs();
					if (!basePage.IsElementVisible(basePage.By_DataMaskCheckbox()))
					{
						throw new Exception("Data Mask checkbox not displayed");
					}
				}

				//Step 7 - Check Data Mask Exam checkbox and observe the studies in Data masking setting window
				//studies.Dropdown_TransferTo().SelectByText("Local System");
				studies = new Studies();
				studies.Transfer("Local System", ReviewTool: true, isDataMasking: true, FrameName: IntegratedFrameName);
				if (basePage.IsElementVisible(By.CssSelector(BasePage.DataMaskSettingsWindow)))
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
					throw new Exception("Data Mask settings window not displayed");
				}
				bool res1 = BasePage.FindElementByCss(BasePage.DataMaskFirstName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res2 = BasePage.FindElementByCss(BasePage.DataMaskMiddleName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res3 = BasePage.FindElementByCss(BasePage.DataMaskLastName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res4 = BasePage.FindElementByCss(BasePage.DataMaskPrefixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
				bool res5 = BasePage.FindElementByCss(BasePage.DataMaskSuffixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
				bool res6 = BasePage.FindElementByCss(BasePage.DataMaskPatientID).GetAttribute("placeholder").Equals("(auto generated)");
				bool res7 = BasePage.FindElementByCss(BasePage.DataMaskIssuerPatientID).GetAttribute("placeholder").Equals("(removed)");
				bool res8 = new SelectElement(BasePage.Driver.FindElement(By.CssSelector(BasePage.DataMaskGender))).SelectedOption.Text.Equals("Male");
				//  bool res9 = BasePage.FindElementByCss(BasePage.DataMaskDOB).GetAttribute("placeholder").Equals("(auto generated)");
				bool res10 = BasePage.FindElementByCss(BasePage.DataMaskAccessionNo).GetAttribute("placeholder").Equals("(auto generated)");
				bool res11 = BasePage.FindElementByCss(BasePage.DataMaskStudyDescription).GetAttribute("placeholder").Equals("(removed)");
				//  bool res12 = BasePage.FindElementByCss(BasePage.DataMaskStudyDate).GetAttribute("placeholder").Equals("(auto generated)");
				if (res1 && res2 && res3 && res4 && res5 && res6 && res7 && res8 && res10 & res11)
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				var copyCheckBox = BasePage.FindElementByCss(BasePage.DataMaskCopyCheckbox);
				if (copyCheckBox.Selected && !copyCheckBox.Enabled)
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("-->Test Step Passed--'Copy patient attributes to all studies' should be checked and grayed out by default");
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--  'Copy patient attributes to all studies' should be checked and grayed out by default");
					result.steps[ExecutedSteps].SetLogs();
				}
				//Result for Step-7
				if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].status = "Fail";
				}
				else
				{
					result.steps[ExecutedSteps].status = "Pass";
				}

				//Step 8 - Do not change any value in Data masking setting and click on Submit button
				((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", BasePage.FindElementByCss(BasePage.DataMaskCancel));
				studyviewer = (StudyViewer)login.NavigateToIntegratorFrame();
				PageLoadWait.WaitForThumbnailsToLoad(40);
				PageLoadWait.WaitForAllViewportsToLoad(40);
				studies.TransferStudy("Local System", SelectallPriors: false, ReviewTool: true, isDataMasking: true, FrameName: IntegratedFrameName);
				ExecutedSteps++;

				//Step 9_1 - Click Download button when the status updated to "Ready"
				PageLoadWait.WaitForDownload(patientNameWithoutDeidentificationValue, Config.downloadpath, "zip");
				if (BasePage.CheckFile(patientNameWithoutDeidentificationValue, Config.downloadpath, "zip"))
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("Study Transferred Successfully");
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				//login.Logout();

				//Step 9_2 - Search for these study instances on EA
				var dicomPath = BasePage.ExtractZipFiles(FileName: patientNameWithoutDeidentificationValue).Find(x => x.Contains("F00"));
				String ExtractedFilePID = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.PatientID);
				var hplogin = new HPLogin();
				basePage.DriverGoTo("https://" + Config.EA7 + "/eaweb");
				var hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
				var workflow = (WorkFlow)hphome.Navigate("Workflow");
				if (workflow.EAv12SearchStudy("PatientID", ExtractedFilePID, 1))
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("Data Masked Study found in EA");
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				hplogin.LogoutEAv12();
				BasePage.DeleteAllFileFolder(Config.downloadpath);

				//Result for Step-9
				if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].status = "Fail";
				}
				else
				{
					result.steps[ExecutedSteps].status = "Pass";
				}

				//Step10_1: Select the same study and click "Transfer" button
				//Close browser
				basePage.CreateNewSesion();
				login.NavigateToIntegratorURL(url_2);
				studyviewer = (StudyViewer)login.NavigateToIntegratorFrame();
				PageLoadWait.WaitForThumbnailsToLoad(40);
				PageLoadWait.WaitForAllViewportsToLoad(40);

				//Step10_2: Update values for Patient information and Study Information and Submit
				var newDataMaskValues = basePage.GetDataMaskFieldsNames();
				newDataMaskValues["LastName"] = login.RandomString(5, true);
				newDataMaskValues["PatientID"] = "PID" + System.DateTime.Now.ToString("ddHHmmss");
				newDataMaskValues["Accession"] = "ACC" + System.DateTime.Now.ToString("ddHHmmss");
				studies.TransferStudy("Local System", SelectallPriors: false, ReviewTool: true, isDataMasking: true, DataMaskValues: newDataMaskValues, FrameName: IntegratedFrameName);
				ExecutedSteps++;

				//Step11: Click Download button when the status updated to "Ready"
				PageLoadWait.WaitForDownload("_" + newDataMaskValues["LastName"], Config.downloadpath, "zip");
				if (BasePage.CheckFile("_" + newDataMaskValues["LastName"], Config.downloadpath, "zip"))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("Study Transferred Successfully");
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step12: Unzip file and check the DICOM file values for Patient Information and Study Information
				dicomPath = BasePage.ExtractZipFiles(FileName: "_" + newDataMaskValues["LastName"]).Find(x => x.Contains("F00"));
				ExtractedFilePID = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.PatientID);
				var ExtractedFileACC = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.AccessionNumber);
				if (ExtractedFilePID == newDataMaskValues["PatientID"] && ExtractedFileACC == newDataMaskValues["Accession"])
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step13: Search for this study instance on EA
				login.Logout();
				hplogin = new HPLogin();
				basePage.DriverGoTo("https://" + Config.EA7 + "/eaweb");
				hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
				workflow = (WorkFlow)hphome.Navigate("Workflow");
				if (workflow.EAv12SearchStudy("PatientID", newDataMaskValues["PatientID"], StudiesCount: 1))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				hplogin.LogoutEAv12();
				BasePage.DeleteAllFileFolder(Config.downloadpath);



				//Report Result
				result.FinalResult(ExecutedSteps);
				Logger.Instance.InfoLog("Overall Test status--" + result.status);
				//Return Result
				GC.Collect();
				return result;
			}
			catch (Exception e)
			{
				//Log exception
				Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);
				//Close browser
				basePage.CreateNewSesion();

				//Report Result
				result.FinalResult(e, ExecutedSteps);
				Logger.Instance.InfoLog("Overall Test status--" + result.status);

				//Return Result
				return result;
			}
			finally
			{
				try
				{
					BasePage.DeleteAllFileFolder(Config.downloadpath);

					//Close browser
					basePage.CreateNewSesion();

					// Set default viewer as bluring viewer in userpreference
					login = new Login();
					login.DriverGoTo(login.url);
					login.LoginIConnect(Config.adminUserName, Config.adminPassword);
					userpref.OpenUserPreferences();
					BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
					PageLoadWait.WaitForPageLoad(20);
					userpref.BluringViewerRadioBtn().Click();
					userpref.CloseUserPreferences();
					login.Logout();

					//Deleting uploaded study
					var hplogin = new HPLogin();
					BasePage.Driver.Navigate().GoToUrl("https://" + Config.EA7 + "/eaweb");
					var hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
					var workflow = (WorkFlow)hphome.Navigate("Workflow");
					workflow.EAv12SearchStudy("PatientID", PID, SelectStudy: true);
					workflow.EAv12DeleteStudy();
					hplogin.LogoutEAv12();

				}
				catch (Exception ex)
				{
					Logger.Instance.ErrorLog("Exception in finally Block: " + ex);
				}
			}
		}


        /// <summary>
        ///  Data masking from Outbound tab
        /// </summary>
        /// <returns></returns>
        public TestCaseResult Test_161320(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            Random randomnumber = new Random();
            WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 300));
            string PID = null;
            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                var studypaths = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String[] studypath = studypaths.Split('=');

                //Precondition - Send studies to EA12
                string Name = login.RandomString(4, true) + new System.DateTime().Second + randomnumber.Next(1, 100);
                PID = "PID" + System.DateTime.Now.ToString("ddHHmmss");
                List<string> ACC = new List<string>(); ;
                string filename = string.Empty;
                string[] SOPID = Enumerable.Repeat("1.2.840.113837.390681903." + System.DateTime.Now.ToString("MMddHHmmss"), studypath.Length).ToArray();
                for (int count = 0; count < studypath.Length; count++)
                {
                    DicomClient client = new DicomClient();
                    ACC.Add("ACC" + System.DateTime.Now.ToString("ddHHmmss") + count);
                    string SID = "1.2.840.113837.1845205803." + System.DateTime.Now.ToString("MMddHHmmss") + "." + Config.IConnectIP.Split('.')[3];
                    string SED = "1.2.840.113837.4288965276." + System.DateTime.Now.ToString("MMddHHmmss") + "." + Config.IConnectIP.Split('.')[3];
                    SOPID[count] = SOPID[count] + ".0" + (count + 1);
                    filename = BasePage.WriteDicomFile(studypath[count], new DicomTag[] { DicomTag.SOPInstanceUID, DicomTag.PatientName, DicomTag.PatientID, DicomTag.AccessionNumber, DicomTag.StudyInstanceUID, DicomTag.SeriesInstanceUID, DicomTag.InstanceNumber }, new string[] { SOPID[count], Name, PID, ACC[count], SID, SED, Convert.ToString(count + 1) });
                    client.AddRequest(new DicomCStoreRequest(filename));
                    client.Send(Config.EA7, 12000, false, "SCU", Config.EA7AETitle);
                    Thread.Sleep(2000);//Study sending to EA
                }

                //Step1: Login into the Administrator
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
				var rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
				rolemanagement.SelectDomainfromDropDown(Config.adminGroupName);
				rolemanagement.SelectRole(Config.adminRoleName);
				rolemanagement.ClickEditRole();
				rolemanagement.SetCheckboxInEditRole("download", 0);
				rolemanagement.SetCheckboxInEditRole("transfer", 0);
				rolemanagement.SetCheckboxInEditRole("email", 0);
				if (!rolemanagement.GrantAccessRadioBtn_Anyone().Selected)
				{
					rolemanagement.GrantAccessRadioBtn_Anyone().Click();
				}
				rolemanagement.ClickSaveEditRole();
				ExecutedSteps++;

                //Step 2: Update the default viewer to Bluering by Options -> User preference.
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.BluringViewerRadioBtn().Click();
                userpref.CloseUserPreferences();
                ExecutedSteps++;

                //Step 3 -Navigate to study tab and search a study. Select study and Share to get dispalyed in Current user OutBound Tab.
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PID, Datasource: login.GetHostName(Config.EA7));
                studies.SelectStudy("Accession", ACC[0]);
                studies.ShareStudy(false, new string[] { Config.ph1UserName });

                Outbounds outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy("Accession", ACC[0]);
                outbounds.SelectStudy("Accession", ACC[0]);
                if (outbounds.CheckStudy("Accession", ACC[0]))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }


                //Step 4: 
                outbounds.TransferBtn().Click();
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent();
                //studies.SwitchTo("index", "0");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (basePage.IsElementVisible(basePage.By_DataMaskCheckbox()) && (!basePage.DataMaskExamCheckbox().Selected))
                {
                    result.steps[++ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Data Mask Exam checkbox visible and unchecked");
                }
                else
                {
                    result.steps[++ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Data Mask Exam is invisible or checked");
                    result.steps[ExecutedSteps].SetLogs();
                }
				PageLoadWait.WaitForSearchPriorStudiesMessage();
				//try { wait.Until(ExpectedConditions.ElementIsVisible(studies.By_RelatedStudy(2))); }
				//catch (Exception) { }
                if (basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[1])) &&
                    basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[2])) &&
                    basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[3])) &&
                    basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[4])))
                {
                    result.steps[ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("All prior studies are listed");
                }
                else
                {
                    result.steps[ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("All prior studies not listed");
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Result for Step-4
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                }

                //Step5: Check the "Data Mask Exam" checkbox and select transfer
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", basePage.DataMaskExamCheckbox());
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", studies.Btn_StudyPageTransferBtn());
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                var copyCheckBox = BasePage.FindElementByCss(BasePage.DataMaskCopyCheckbox);

                //Step5: Check the Data Masking setting window fields
                bool res1 = BasePage.FindElementByCss(BasePage.DataMaskFirstName).GetAttribute("placeholder").Equals("(auto generated)");
                bool res2 = BasePage.FindElementByCss(BasePage.DataMaskMiddleName).GetAttribute("placeholder").Equals("(auto generated)");
                bool res3 = BasePage.FindElementByCss(BasePage.DataMaskLastName).GetAttribute("placeholder").Equals("(auto generated)");
                bool res4 = BasePage.FindElementByCss(BasePage.DataMaskPrefixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
                bool res5 = BasePage.FindElementByCss(BasePage.DataMaskSuffixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
                bool res6 = BasePage.FindElementByCss(BasePage.DataMaskPatientID).GetAttribute("placeholder").Equals("(auto generated)");
                bool res7 = BasePage.FindElementByCss(BasePage.DataMaskIssuerPatientID).GetAttribute("placeholder").Equals("(removed)");
                bool res8 = new SelectElement(BasePage.Driver.FindElement(By.CssSelector(BasePage.DataMaskGender))).SelectedOption.Text.Equals("Male");
                //  bool res9 = BasePage.FindElementByCss(BasePage.DataMaskDOB).GetAttribute("placeholder").Equals("(auto generated)");
                bool res10 = BasePage.FindElementByCss(BasePage.DataMaskAccessionNo).GetAttribute("placeholder").Equals("(auto generated)");
                bool res11 = BasePage.FindElementByCss(BasePage.DataMaskStudyDescription).GetAttribute("placeholder").Equals("(removed)");
                //  bool res12 = BasePage.FindElementByCss(BasePage.DataMaskStudyDate).GetAttribute("placeholder").Equals("(auto generated)");
                if (copyCheckBox.Selected && !copyCheckBox.Enabled && res1 && res2 && res3 && res4 && res5 && res6 && res7 && res8 && res10 & res11)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 6: Do not change any value in Data masking setting and click on Submit button
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", BasePage.FindElementByCss(BasePage.DataMaskCancel));
                studies.SelectStudy("Accession", ACC[0]);
                studies.TransferStudy("Local System", SelectallPriors: false, PatientTab: true, isDataMasking: true);
                ExecutedSteps++;

                //Step 7: Click Download button when the status updated to "Ready". //New study instance should also be stored on EA
                PageLoadWait.WaitForDownload(patientNameWithoutDeidentificationValue, Config.downloadpath, "zip");
                if (BasePage.CheckFile(patientNameWithoutDeidentificationValue, Config.downloadpath, "zip"))
                {
                    result.steps[++ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Study Transferred Successfully");
                }
                else
                {
                    result.steps[++ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                var dicomPath = BasePage.ExtractZipFiles(FileName: patientNameWithoutDeidentificationValue).Find(x => x.Contains("F00"));
                String ExtractedFilePID = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.PatientID);
                var hplogin = new HPLogin();
                basePage.DriverGoTo("https://" + Config.EA7 + "/eaweb");
                var hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
                var workflow = (WorkFlow)hphome.Navigate("Workflow");
                if (workflow.EAv12SearchStudy("PatientID", ExtractedFilePID))
                {
                    result.steps[ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Study Transferred Successfully");
                }
                else
                {
                    result.steps[ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                hplogin.LogoutEAv12();
                BasePage.DeleteAllFileFolder(Config.downloadpath);

                //Result for Step-7
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                }

                //Step 8: Click on the same study and select Transfer and select data mask exam chcekbox
                //Step 8: Update values for Patient information and Study Information and Submit
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy("Accession", ACC[0]);
                outbounds.SelectStudy("Accession", ACC[0]);
                var newDataMaskValues = basePage.GetDataMaskFieldsNames();
				newDataMaskValues["LastName"] = login.RandomString(5, true);
				newDataMaskValues["PatientID"] = "PID" + System.DateTime.Now.ToString("ddHHmmss");
                newDataMaskValues["Accession"] = "ACC" + System.DateTime.Now.ToString("ddHHmmss");
                basePage.TransferStudy("Local System", SelectallPriors: false, PatientTab: true, isDataMasking: true, DataMaskValues: newDataMaskValues);
                ExecutedSteps++;

                //Step 9: Click Download button when the status updated to "Ready". New study instance should also be stored on EA
                PageLoadWait.WaitForDownload("_" + newDataMaskValues["LastName"], Config.downloadpath, "zip");
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: newDataMaskValues["PatientID"], Datasource: login.GetHostName(Config.EA7));
                if (studies.CheckStudy("Accession", newDataMaskValues["Accession"]) && BasePage.CheckFile("_" + newDataMaskValues["LastName"], Config.downloadpath, "zip"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                login.Logout();


                //Step 10: Unzip file and check the DICOM file values for Patient Information and Study Information
                dicomPath = BasePage.ExtractZipFiles(FileName: "_" + newDataMaskValues["LastName"]).Find(x => x.Contains("F00"));
                ExtractedFilePID = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.PatientID);
                String ExtractedFileACC = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.AccessionNumber);
                if (ExtractedFilePID == newDataMaskValues["PatientID"] && ExtractedFileACC == newDataMaskValues["Accession"])
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 11: Search for this study instance on EA
                hplogin = new HPLogin();
                basePage.DriverGoTo("https://" + Config.EA7 + "/eaweb");
                hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
                workflow = (WorkFlow)hphome.Navigate("Workflow");
                if (workflow.EAv12SearchStudy("Accession", newDataMaskValues["Accession"]))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                hplogin.LogoutEAv12();
				BasePage.DeleteAllFileFolder(Config.downloadpath);



				//Step12: Login into the Administrator,  Update the default viewer to HTML4 by Options -> User preference.
				login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.HTML4RadioBtn().Click();
                userpref.CloseUserPreferences();
                ExecutedSteps++;

                //Step 13 -Navigate to study tab and search a study. Select study and Share to get dispalyed in Current user OutBound Tab.
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PID, Datasource: login.GetHostName(Config.EA7));
                studies.SelectStudy("Accession", ACC[1]);
                studies.ShareStudy(false, new string[] { Config.ph1UserName });

                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy("Accession", ACC[1]);
                outbounds.SelectStudy("Accession", ACC[1]);
                if (outbounds.CheckStudy("Accession", ACC[1]))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 14: 
                outbounds.TransferBtn().Click();
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent();
                //studies.SwitchTo("index", "0");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (basePage.IsElementVisible(basePage.By_DataMaskCheckbox()) && (!basePage.DataMaskExamCheckbox().Selected))
                {
                    result.steps[++ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Data Mask Exam checkbox visible and unchecked");
                }
                else
                {
                    result.steps[++ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Data Mask Exam is invisible or checked");
                    result.steps[ExecutedSteps].SetLogs();
                }
				PageLoadWait.WaitForSearchPriorStudiesMessage();
				//try { wait.Until(ExpectedConditions.ElementIsVisible(studies.RelatedStudiesUsingAcc(ACC[2]))); }
				//catch (Exception) { }
                if (basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[0])) &&
                    basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[2])) &&
                    basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[3])) &&
                    basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[4])))
                {
                    result.steps[ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("All prior studies are listed");
                }
                else
                {
                    result.steps[ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("All prior studies not listed");
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Result for Step-4
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                }

                //Step15: Check the "Data Mask Exam" checkbox and select transfer
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", basePage.DataMaskExamCheckbox());
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", studies.Btn_StudyPageTransferBtn());
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                copyCheckBox = BasePage.FindElementByCss(BasePage.DataMaskCopyCheckbox);

                //Step15: Check the Data Masking setting window fields
                res1 = BasePage.FindElementByCss(BasePage.DataMaskFirstName).GetAttribute("placeholder").Equals("(auto generated)");
                res2 = BasePage.FindElementByCss(BasePage.DataMaskMiddleName).GetAttribute("placeholder").Equals("(auto generated)");
                res3 = BasePage.FindElementByCss(BasePage.DataMaskLastName).GetAttribute("placeholder").Equals("(auto generated)");
                res4 = BasePage.FindElementByCss(BasePage.DataMaskPrefixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
                res5 = BasePage.FindElementByCss(BasePage.DataMaskSuffixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
                res6 = BasePage.FindElementByCss(BasePage.DataMaskPatientID).GetAttribute("placeholder").Equals("(auto generated)");
                res7 = BasePage.FindElementByCss(BasePage.DataMaskIssuerPatientID).GetAttribute("placeholder").Equals("(removed)");
                res8 = new SelectElement(BasePage.Driver.FindElement(By.CssSelector(BasePage.DataMaskGender))).SelectedOption.Text.Equals("Male");
                // res9 = BasePage.FindElementByCss(BasePage.DataMaskDOB).GetAttribute("placeholder").Equals("(auto generated)");
                res10 = BasePage.FindElementByCss(BasePage.DataMaskAccessionNo).GetAttribute("placeholder").Equals("(auto generated)");
                res11 = BasePage.FindElementByCss(BasePage.DataMaskStudyDescription).GetAttribute("placeholder").Equals("(removed)");
                // res12 = BasePage.FindElementByCss(BasePage.DataMaskStudyDate).GetAttribute("placeholder").Equals("(auto generated)");
                if (copyCheckBox.Selected && !copyCheckBox.Enabled && res1 && res2 && res3 && res4 && res5 && res6 && res7 && res8 && res10 & res11)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 16: Do not change any value in Data masking setting and click on Submit button
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", BasePage.FindElementByCss(BasePage.DataMaskCancel));
                studies.SelectStudy("Accession", ACC[1]);
                studies.TransferStudy("Local System", SelectallPriors: false, PatientTab: true, isDataMasking: true);
                ExecutedSteps++;

                //Step 17: Click Download button when the status updated to "Ready". //New study instance should also be stored on EA
                PageLoadWait.WaitForDownload(patientNameWithoutDeidentificationValue, Config.downloadpath, "zip");
                if (BasePage.CheckFile(patientNameWithoutDeidentificationValue, Config.downloadpath, "zip"))
                {
                    result.steps[++ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Study Transferred Successfully");
                }
                else
                {
                    result.steps[++ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                dicomPath = BasePage.ExtractZipFiles(FileName: patientNameWithoutDeidentificationValue).Find(x => x.Contains("F00"));
                ExtractedFilePID = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.PatientID);
                hplogin = new HPLogin();
                basePage.DriverGoTo("https://" + Config.EA7 + "/eaweb");
                hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
                workflow = (WorkFlow)hphome.Navigate("Workflow");
                if (workflow.EAv12SearchStudy("PatientID", ExtractedFilePID))
                {
                    result.steps[ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Study Transferred Successfully");
                }
                else
                {
                    result.steps[ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                hplogin.LogoutEAv12();
                BasePage.DeleteAllFileFolder(Config.downloadpath);

                //Result for Step-17
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                //Step 18: Click on the same study and select Transfer and select data mask exam chcekbox
                //Step 18: Update values for Patient information and Study Information and Submit
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy("Accession", ACC[1]);
                outbounds.SelectStudy("Accession", ACC[1]);
                newDataMaskValues = basePage.GetDataMaskFieldsNames();
				newDataMaskValues["LastName"] = login.RandomString(5, true);
				newDataMaskValues["PatientID"] = "PID" + System.DateTime.Now.ToString("ddHHmmss");
                newDataMaskValues["Accession"] = "ACC" + System.DateTime.Now.ToString("ddHHmmss");
                basePage.TransferStudy("Local System", SelectallPriors: false, PatientTab: true, isDataMasking: true, DataMaskValues: newDataMaskValues);
                ExecutedSteps++;

                //Step 19: Click Download button when the status updated to "Ready". New study instance should also be stored on EA
                PageLoadWait.WaitForDownload("_" + newDataMaskValues["LastName"], Config.downloadpath, "zip");
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: newDataMaskValues["PatientID"], Datasource: login.GetHostName(Config.EA7));
                if (studies.CheckStudy("Accession", newDataMaskValues["Accession"]) && BasePage.CheckFile("_" + newDataMaskValues["LastName"], Config.downloadpath, "zip"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                login.Logout();


                //Step 20: Unzip file and check the DICOM file values for Patient Information and Study Information
                dicomPath = BasePage.ExtractZipFiles(FileName: "_" + newDataMaskValues["LastName"]).Find(x => x.Contains("F00"));
                ExtractedFilePID = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.PatientID);
                ExtractedFileACC = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.AccessionNumber);
                if (ExtractedFilePID == newDataMaskValues["PatientID"] && ExtractedFileACC == newDataMaskValues["Accession"])
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 21: Search for this study instance on EA
                hplogin = new HPLogin();
                basePage.DriverGoTo("https://" + Config.EA7 + "/eaweb");
                hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
                workflow = (WorkFlow)hphome.Navigate("Workflow");
                if (workflow.EAv12SearchStudy("Accession", newDataMaskValues["Accession"]))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                hplogin.LogoutEAv12();



                //Return result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                return result;
            }
            finally
            {
				try
				{
					BasePage.DeleteAllFileFolder(Config.downloadpath);
					//Deleting uploaded study
					var hplogin = new HPLogin();
					BasePage.Driver.Navigate().GoToUrl("https://" + Config.EA7 + "/eaweb");
					var hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
					var workflow = (WorkFlow)hphome.Navigate("Workflow");
					workflow.EAv12SearchStudy("PatientID", PID, SelectStudy: true);
					workflow.EAv12DeleteStudy();
					hplogin.LogoutEAv12();
				}
				catch (Exception e)
				{
					Logger.Instance.WarnLog("Error in Finally block");
				}
            }

        }

        /// <summary>
        ///  Data masking from Inbounds tab
        /// </summary>
        /// <returns></returns>
        public TestCaseResult Test_161319(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            Random randomnumber = new Random();
            WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 300));
            string PID = null;
			try
			{
				String adminUserName = Config.adminUserName;
				String adminPassword = Config.adminPassword;
				var studypaths = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
				String[] studypath = studypaths.Split('=');

				//Precondition - Send studies to EA12
				string Name = login.RandomString(4, true) + new System.DateTime().Second + randomnumber.Next(1, 100);
				PID = "PID" + System.DateTime.Now.ToString("ddHHmmss");
				List<string> ACC = new List<string>(); ;
				string filename = string.Empty;
				string[] SOPID = Enumerable.Repeat("1.2.840.113837.390681903." + System.DateTime.Now.ToString("MMddHHmmss"), studypath.Length).ToArray();
				for (int count = 0; count < studypath.Length; count++)
				{
					DicomClient client = new DicomClient();
					ACC.Add("ACC" + System.DateTime.Now.ToString("ddHHmmss") + count);
					string SID = "1.2.840.113837.1845205803." + System.DateTime.Now.ToString("MMddHHmmss") + "." + Config.IConnectIP.Split('.')[3];
					string SED = "1.2.840.113837.4288965276." + System.DateTime.Now.ToString("MMddHHmmss") + "." + Config.IConnectIP.Split('.')[3];
					SOPID[count] = SOPID[count] + ".0" + (count + 1);
					filename = BasePage.WriteDicomFile(studypath[count], new DicomTag[] { DicomTag.SOPInstanceUID, DicomTag.PatientName, DicomTag.PatientID, DicomTag.AccessionNumber, DicomTag.StudyInstanceUID, DicomTag.SeriesInstanceUID, DicomTag.InstanceNumber }, new string[] { SOPID[count], Name, PID, ACC[count], SID, SED, Convert.ToString(count + 1) });
					client.AddRequest(new DicomCStoreRequest(filename));
					client.Send(Config.EA7, 12000, false, "SCU", Config.EA7AETitle);
					Thread.Sleep(2000);//Study sending to EA
				}

				login.DriverGoTo(login.url);
				login.LoginIConnect(adminUserName, adminPassword);
				var rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
				rolemanagement.SelectDomainfromDropDown(Config.adminGroupName);
				rolemanagement.SelectRole(Config.adminRoleName);
				rolemanagement.ClickEditRole();
				rolemanagement.SetCheckboxInEditRole("download", 0);
				rolemanagement.SetCheckboxInEditRole("transfer", 0);
				rolemanagement.SetCheckboxInEditRole("email", 0);
				if (!rolemanagement.GrantAccessRadioBtn_Anyone().Selected)
				{
					rolemanagement.GrantAccessRadioBtn_Anyone().Click();
				}
				rolemanagement.ClickSaveEditRole();
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(patientID: PID, Datasource: login.GetHostName(Config.EA7));
				studies.SelectStudy("Accession", ACC[0]);
                studies.ShareStudy(false, new string[] { Config.ph1UserName });
				login.Logout();


				//Step1: Login into the Administrator
				login.DriverGoTo(login.url);
				login.LoginIConnect(Config.ph1UserName, Config.ph1Password);
				ExecutedSteps++;

				//Step 2: Update the default viewer to Bluering by Options -> User preference.
				userpref.OpenUserPreferences();
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
				PageLoadWait.WaitForPageLoad(20);
				userpref.BluringViewerRadioBtn().Click();
				userpref.CloseUserPreferences();
				ExecutedSteps++;

				//Step 3 -Navigate to study tab and search a study. Select study and Share to get dispalyed in Current user OutBound Tab.
				Inbounds inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("Accession", ACC[0]);
				inbounds.SelectStudy("Accession", ACC[0]);
				if (inbounds.CheckStudy("Accession", ACC[0]))
				{
					result.steps[++ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[++ExecutedSteps].StepFail();
				}


				//Step 4: 
				inbounds.TransferBtn().Click();
				PageLoadWait.WaitForFrameLoad(10);
				BasePage.Driver.SwitchTo().DefaultContent();
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				if (basePage.IsElementVisible(basePage.By_DataMaskCheckbox()) && (!basePage.DataMaskExamCheckbox().Selected))
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("Data Mask Exam checkbox visible and unchecked");
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("Data Mask Exam is invisible or checked");
					result.steps[ExecutedSteps].SetLogs();
				}

				PageLoadWait.WaitForSearchPriorStudiesMessage();
				//try { wait.Until(ExpectedConditions.ElementIsVisible(studies.By_RelatedStudy(2))); }
				//catch (Exception) { }
				if (basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[1])) &&
					basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[2])) &&
					basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[3])) &&
					basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[4])))
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("All prior studies are listed");
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("All prior studies not listed");
					result.steps[ExecutedSteps].SetLogs();
				}
				//Result for Step-4
				if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].status = "Fail";
				}
				else
				{
					result.steps[ExecutedSteps].status = "Pass";
				}

				//Step5: Check the "Data Mask Exam" checkbox and select transfer
				((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", basePage.DataMaskExamCheckbox());
				((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", studies.Btn_StudyPageTransferBtn());
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				var copyCheckBox = BasePage.FindElementByCss(BasePage.DataMaskCopyCheckbox);

				//Step5: Check the Data Masking setting window fields
				bool res1 = BasePage.FindElementByCss(BasePage.DataMaskFirstName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res2 = BasePage.FindElementByCss(BasePage.DataMaskMiddleName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res3 = BasePage.FindElementByCss(BasePage.DataMaskLastName).GetAttribute("placeholder").Equals("(auto generated)");
				bool res4 = BasePage.FindElementByCss(BasePage.DataMaskPrefixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
				bool res5 = BasePage.FindElementByCss(BasePage.DataMaskSuffixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
				bool res6 = BasePage.FindElementByCss(BasePage.DataMaskPatientID).GetAttribute("placeholder").Equals("(auto generated)");
				bool res7 = BasePage.FindElementByCss(BasePage.DataMaskIssuerPatientID).GetAttribute("placeholder").Equals("(removed)");
				bool res8 = new SelectElement(BasePage.Driver.FindElement(By.CssSelector(BasePage.DataMaskGender))).SelectedOption.Text.Equals("Male");
				//  bool res9 = BasePage.FindElementByCss(BasePage.DataMaskDOB).GetAttribute("placeholder").Equals("(auto generated)");
				bool res10 = BasePage.FindElementByCss(BasePage.DataMaskAccessionNo).GetAttribute("placeholder").Equals("(auto generated)");
				bool res11 = BasePage.FindElementByCss(BasePage.DataMaskStudyDescription).GetAttribute("placeholder").Equals("(removed)");
				//  bool res12 = BasePage.FindElementByCss(BasePage.DataMaskStudyDate).GetAttribute("placeholder").Equals("(auto generated)");
				if (copyCheckBox.Selected && !copyCheckBox.Enabled && res1 && res2 && res3 && res4 && res5 && res6 && res7 && res8 && res10 & res11)
				{
					result.steps[++ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[++ExecutedSteps].StepFail();
				}

				//Step 6: Do not change any value in Data masking setting and click on Submit button
				((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", BasePage.FindElementByCss(BasePage.DataMaskCancel));
				studies.SelectStudy("Accession", ACC[0]);
				studies.TransferStudy("Local System", SelectallPriors: false, PatientTab: true, isDataMasking: true);
				ExecutedSteps++;

				//Step 7: Click Download button when the status updated to "Ready". //New study instance should also be stored on EA
				PageLoadWait.WaitForDownload(patientNameWithoutDeidentificationValue, Config.downloadpath, "zip");
				if (BasePage.CheckFile(patientNameWithoutDeidentificationValue, Config.downloadpath, "zip"))
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("Study Transferred Successfully");
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				var dicomPath = BasePage.ExtractZipFiles(FileName: patientNameWithoutDeidentificationValue).Find(x => x.Contains("F00"));
				String ExtractedFilePID = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.PatientID);
				var hplogin = new HPLogin();
				basePage.DriverGoTo("https://" + Config.EA7 + "/eaweb");
				var hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
				var workflow = (WorkFlow)hphome.Navigate("Workflow");
				if (workflow.EAv12SearchStudy("PatientID", ExtractedFilePID))
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("Study Transferred Successfully");
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				hplogin.LogoutEAv12();
				BasePage.DeleteAllFileFolder(Config.downloadpath);

				//Result for Step-7
				if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].status = "Fail";
				}
				else
				{
					result.steps[ExecutedSteps].status = "Pass";
				}

				//Step 8: Click on the same study and select Transfer and select data mask exam chcekbox
				//Step 8: Update values for Patient information and Study Information and Submit
				login.DriverGoTo(login.url);
                login.LoginIConnect(Config.ph1UserName, Config.ph1Password);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("Accession", ACC[0]);
				inbounds.SelectStudy("Accession", ACC[0]);
				var newDataMaskValues = basePage.GetDataMaskFieldsNames();
				newDataMaskValues["PatientID"] = "PID" + System.DateTime.Now.ToString("ddHHmmss");
				newDataMaskValues["Accession"] = "ACC" + System.DateTime.Now.ToString("ddHHmmss");
				basePage.TransferStudy("Local System", SelectallPriors: false, PatientTab: true, isDataMasking: true, DataMaskValues: newDataMaskValues);
				ExecutedSteps++;

				//Step 9: Click Download button when the status updated to "Ready". New study instance should also be stored on EA
				PageLoadWait.WaitForDownload(patientNameWithoutDeidentificationValue, Config.downloadpath, "zip");
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(patientID: newDataMaskValues["PatientID"], Datasource: login.GetHostName(Config.EA7));
				if (studies.CheckStudy("Accession", newDataMaskValues["Accession"]) && BasePage.CheckFile(patientNameWithoutDeidentificationValue, Config.downloadpath, "zip"))
				{
					result.steps[++ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[++ExecutedSteps].StepFail();
				}
				login.Logout();


				//Step 10: Unzip file and check the DICOM file values for Patient Information and Study Information
				dicomPath = BasePage.ExtractZipFiles(FileName: patientNameWithoutDeidentificationValue).Find(x => x.Contains("F00"));
				ExtractedFilePID = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.PatientID);
				String ExtractedFileACC = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.AccessionNumber);
				if (ExtractedFilePID == newDataMaskValues["PatientID"] && ExtractedFileACC == newDataMaskValues["Accession"])
				{
					result.steps[++ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[++ExecutedSteps].StepFail();
				}

				//Step 11: Search for this study instance on EA
				hplogin = new HPLogin();
				basePage.DriverGoTo("https://" + Config.EA7 + "/eaweb");
				hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
				workflow = (WorkFlow)hphome.Navigate("Workflow");
				if (workflow.EAv12SearchStudy("Accession", newDataMaskValues["Accession"]))
				{
					result.steps[++ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[++ExecutedSteps].StepFail();
				}
				hplogin.LogoutEAv12();
				BasePage.DeleteAllFileFolder(Config.downloadpath);



				//Step12: Login into the Administrator,  Update the default viewer to HTML4 by Options -> User preference.
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminUserName, adminPassword);
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(patientID: PID, Datasource: login.GetHostName(Config.EA7));
				studies.SelectStudy("Accession", ACC[1]);
				studies.ShareStudy(false, new string[] { Config.ph1UserName });
                login.Logout();
                ExecutedSteps++;

                //Step 13 -Navigate to study tab and search a study. Select study and Share to get dispalyed in Current user OutBound Tab.
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.ph1UserName, Config.ph1Password);
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.HTML4RadioBtn().Click();
                userpref.CloseUserPreferences();
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("Accession", ACC[1]);
				inbounds.SelectStudy("Accession", ACC[1]);
				if (inbounds.CheckStudy("Accession", ACC[1]))
				{
					result.steps[++ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[++ExecutedSteps].StepFail();
				}

				//Step 14: 
				inbounds.TransferBtn().Click();
				PageLoadWait.WaitForFrameLoad(10);
				BasePage.Driver.SwitchTo().DefaultContent();
				//studies.SwitchTo("index", "0");
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				if (basePage.IsElementVisible(basePage.By_DataMaskCheckbox()) && (!basePage.DataMaskExamCheckbox().Selected))
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("Data Mask Exam checkbox visible and unchecked");
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("Data Mask Exam is invisible or checked");
					result.steps[ExecutedSteps].SetLogs();
				}

				PageLoadWait.WaitForSearchPriorStudiesMessage();
				//try { wait.Until(ExpectedConditions.ElementIsVisible(studies.RelatedStudiesUsingAcc(ACC[2]))); }
				//catch (Exception) { }
				if (basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[0])) &&
					basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[2])) &&
					basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[3])) &&
					basePage.IsElementPresent(studies.RelatedStudiesUsingAcc(ACC[4])))
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("All prior studies are listed");
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("All prior studies not listed");
					result.steps[ExecutedSteps].SetLogs();
				}
				//Result for Step-4
				if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].status = "Fail";
				}
				else
				{
					result.steps[ExecutedSteps].status = "Pass";
				}

				//Step15: Check the "Data Mask Exam" checkbox and select transfer
				((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", basePage.DataMaskExamCheckbox());
				((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", studies.Btn_StudyPageTransferBtn());
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				copyCheckBox = BasePage.FindElementByCss(BasePage.DataMaskCopyCheckbox);

				//Step15: Check the Data Masking setting window fields
				res1 = BasePage.FindElementByCss(BasePage.DataMaskFirstName).GetAttribute("placeholder").Equals("(auto generated)");
				res2 = BasePage.FindElementByCss(BasePage.DataMaskMiddleName).GetAttribute("placeholder").Equals("(auto generated)");
				res3 = BasePage.FindElementByCss(BasePage.DataMaskLastName).GetAttribute("placeholder").Equals("(auto generated)");
				res4 = BasePage.FindElementByCss(BasePage.DataMaskPrefixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
				res5 = BasePage.FindElementByCss(BasePage.DataMaskSuffixTextbox).GetAttribute("placeholder").Equals("(auto generated)");
				res6 = BasePage.FindElementByCss(BasePage.DataMaskPatientID).GetAttribute("placeholder").Equals("(auto generated)");
				res7 = BasePage.FindElementByCss(BasePage.DataMaskIssuerPatientID).GetAttribute("placeholder").Equals("(removed)");
				res8 = new SelectElement(BasePage.Driver.FindElement(By.CssSelector(BasePage.DataMaskGender))).SelectedOption.Text.Equals("Male");
				// res9 = BasePage.FindElementByCss(BasePage.DataMaskDOB).GetAttribute("placeholder").Equals("(auto generated)");
				res10 = BasePage.FindElementByCss(BasePage.DataMaskAccessionNo).GetAttribute("placeholder").Equals("(auto generated)");
				res11 = BasePage.FindElementByCss(BasePage.DataMaskStudyDescription).GetAttribute("placeholder").Equals("(removed)");
				// res12 = BasePage.FindElementByCss(BasePage.DataMaskStudyDate).GetAttribute("placeholder").Equals("(auto generated)");
				if (copyCheckBox.Selected && !copyCheckBox.Enabled && res1 && res2 && res3 && res4 && res5 && res6 && res7 && res8 && res10 & res11)
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step 16: Do not change any value in Data masking setting and click on Submit button
				((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", BasePage.FindElementByCss(BasePage.DataMaskCancel));
				studies.SelectStudy("Accession", ACC[1]);
				studies.TransferStudy("Local System", SelectallPriors: false, PatientTab: true, isDataMasking: true);
				ExecutedSteps++;

				//Step 17: Click Download button when the status updated to "Ready". //New study instance should also be stored on EA
				PageLoadWait.WaitForDownload(patientNameWithoutDeidentificationValue, Config.downloadpath, "zip");
				if (BasePage.CheckFile(patientNameWithoutDeidentificationValue, Config.downloadpath, "zip"))
				{
					result.steps[++ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("Study Transferred Successfully");
				}
				else
				{
					result.steps[++ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				dicomPath = BasePage.ExtractZipFiles(FileName: patientNameWithoutDeidentificationValue).Find(x => x.Contains("F00"));
				ExtractedFilePID = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.PatientID);
				hplogin = new HPLogin();
				basePage.DriverGoTo("https://" + Config.EA7 + "/eaweb");
				hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
				workflow = (WorkFlow)hphome.Navigate("Workflow");
				if (workflow.EAv12SearchStudy("PatientID", ExtractedFilePID))
				{
					result.steps[ExecutedSteps].statuslist.Add("Pass");
					Logger.Instance.InfoLog("Study Transferred Successfully");
				}
				else
				{
					result.steps[ExecutedSteps].statuslist.Add("Fail");
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				hplogin.LogoutEAv12();
				BasePage.DeleteAllFileFolder(Config.downloadpath);

				//Result for Step-17
				if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				//Step 18: Click on the same study and select Transfer and select data mask exam chcekbox
				//Step 18: Update values for Patient information and Study Information and Submit
				login.DriverGoTo(login.url);
                login.LoginIConnect(Config.ph1UserName, Config.ph1Password);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("Accession", ACC[1]);
				inbounds.SelectStudy("Accession", ACC[1]);
				newDataMaskValues = basePage.GetDataMaskFieldsNames();
				newDataMaskValues["LastName"] = login.RandomString(5, true);
				newDataMaskValues["PatientID"] = "PID" + System.DateTime.Now.ToString("ddHHmmss");
				newDataMaskValues["Accession"] = "ACC" + System.DateTime.Now.ToString("ddHHmmss");
				basePage.TransferStudy("Local System", SelectallPriors: false, PatientTab: true, isDataMasking: true, DataMaskValues: newDataMaskValues);
				ExecutedSteps++;

				//Step 19: Click Download button when the status updated to "Ready". New study instance should also be stored on EA
				PageLoadWait.WaitForDownload("_" + newDataMaskValues["LastName"], Config.downloadpath, "zip");
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(patientID: newDataMaskValues["PatientID"], Datasource: login.GetHostName(Config.EA7));
				if (studies.CheckStudy("Accession", newDataMaskValues["Accession"]) && BasePage.CheckFile("_" + newDataMaskValues["LastName"], Config.downloadpath, "zip"))
				{
					result.steps[++ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[++ExecutedSteps].StepFail();
				}
				login.Logout();


				//Step 20: Unzip file and check the DICOM file values for Patient Information and Study Information
				dicomPath = BasePage.ExtractZipFiles(FileName: "_" + newDataMaskValues["LastName"]).Find(x => x.Contains("F00"));
				ExtractedFilePID = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.PatientID);
				ExtractedFileACC = BasePage.ReadDicomFile<String>(dicomPath, DicomTag.AccessionNumber);
				if (ExtractedFilePID == newDataMaskValues["PatientID"] && ExtractedFileACC == newDataMaskValues["Accession"])
				{
					result.steps[++ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[++ExecutedSteps].StepFail();
				}

				//Step 21: Search for this study instance on EA
				hplogin = new HPLogin();
				basePage.DriverGoTo("https://" + Config.EA7 + "/eaweb");
				hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
				workflow = (WorkFlow)hphome.Navigate("Workflow");
				if (workflow.EAv12SearchStudy("Accession", newDataMaskValues["Accession"]))
				{
					result.steps[++ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[++ExecutedSteps].StepFail();
				}
				hplogin.LogoutEAv12();



				//Return result
				result.FinalResult(ExecutedSteps);
				Logger.Instance.InfoLog("Overall Test status--" + result.status);

				return result;
			}
			catch (Exception e)
			{
				//Log Exception
				Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
				result.FinalResult(e, ExecutedSteps);
				Logger.Instance.ErrorLog("Overall Test status--" + result.status);
				return result;
			}
			finally
			{
				try
				{
					BasePage.DeleteAllFileFolder(Config.downloadpath);
					//Deleting uploaded study
					var hplogin = new HPLogin();
					BasePage.Driver.Navigate().GoToUrl("https://" + Config.EA7 + "/eaweb");
					var hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
					var workflow = (WorkFlow)hphome.Navigate("Workflow");
					workflow.EAv12SearchStudy("PatientID", PID, SelectStudy: true);
					workflow.EAv12DeleteStudy();
					hplogin.LogoutEAv12();
				}
				catch (Exception e)
				{
					Logger.Instance.WarnLog("Error in Finally block");
				}
			}

        }

		/// <summary>
		///  Data Masking log verification for failed transfers - Standalone
		/// </summary>
		/// <returns></returns>
		public TestCaseResult Test_163570(String testid, String teststeps, int stepcount)
		{
			TestCaseResult result = new TestCaseResult(stepcount);
			result.SetTestStepDescription(teststeps);
			int ExecutedSteps = -1;
			string PID = null;
			try
			{
				String adminUserName = Config.adminUserName;
				String adminPassword = Config.adminPassword;
				var studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
				PID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
				string ImageTransfercongif = @"C:\WebAccess\WindowsService\ImageTransfer\bin\ImageTransfer.exe.config";

				//Precondition
				try
				{
					var hplogin = new HPLogin();
					BasePage.Driver.Navigate().GoToUrl("https://" + Config.EA7 + "/eaweb");
					var hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
					var workflow = (WorkFlow)hphome.Navigate("Workflow");
					if (workflow.EAv12SearchStudy("PatientID", PID, SelectStudy: true))
					{
						workflow.EAv12DeleteStudy();
						hplogin.LogoutEAv12();
					}
				}
				catch (Exception e)
				{
					Logger.Instance.InfoLog("Error while deleting study from EA. " + e);
				}

				//Step 1 - Send studies to EA12
				DicomClient client = new DicomClient();
				client.AddRequest(new DicomCStoreRequest(Config.TestDataPath + studypath));
				client.Send(Config.EA7, 12000, false, "SCU", Config.EA7AETitle);
				Thread.Sleep(2000);//Study sending to EA

				//update the Config values.
				string DeveloperTraceSwitchValuesBeforeUpdate = basePage.GetAttributeValue(ImageTransfercongif, "/configuration/system.diagnostics/switches/add[@name='DeveloperTraceSwitch']", "value");
				basePage.ChangeAttributeValue(ImageTransfercongif, "configuration/system.diagnostics/switches/add[@name='DeveloperTraceSwitch']", "value", "Verbose");
				string DeveloperTraceSwitchValuesAfterUpdate = basePage.GetAttributeValue(ImageTransfercongif, "/configuration/system.diagnostics/switches/add[@name='DeveloperTraceSwitch']", "value");

				string WebAccessTraceSwitchValuesBeforeUpdate = basePage.GetAttributeValue(ImageTransfercongif, "/configuration/system.diagnostics/switches/add[@name='WebAccessTraceSwitch']", "value");
				basePage.ChangeAttributeValue(ImageTransfercongif, "configuration/system.diagnostics/switches/add[@name='WebAccessTraceSwitch']", "value", "Verbose");
				string WebAccessTraceSwitchValuesAfterUpdate = basePage.GetAttributeValue(ImageTransfercongif, "/configuration/system.diagnostics/switches/add[@name='WebAccessTraceSwitch']", "value");

				string PerformanceTraceSwitchValuesBeforeUpdate = basePage.GetAttributeValue(ImageTransfercongif, "/configuration/system.diagnostics/switches/add[@name='PerformanceTraceSwitch']", "value");
				basePage.ChangeAttributeValue(ImageTransfercongif, "configuration/system.diagnostics/switches/add[@name='PerformanceTraceSwitch']", "value", "Verbose");
				string PerformanceTraceSwitchValuesAfterUpdate = basePage.GetAttributeValue(ImageTransfercongif, "/configuration/system.diagnostics/switches/add[@name='PerformanceTraceSwitch']", "value");

				servicetool.LaunchServiceTool();
				servicetool.RestartIISandWindowsServices();
				servicetool.CloseServiceTool();
				result.steps[++ExecutedSteps].StepPass();

				//Step2: Login into the Administrator
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminUserName, adminPassword);
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(patientID: PID, Datasource: login.GetHostName(Config.EA7));
				studies.SelectStudy("Patient ID", PID);
				var transferStartTime = System.DateTime.Now;
				try
				{
					studies.TransferStudy("Local System", SelectallPriors: false, PatientTab: true, isDataMasking: true);
					result.steps[++ExecutedSteps].StepFail();
				}
				catch (Exception e)
				{
					result.steps[++ExecutedSteps].StepPass();
				}

				//Step3: Verify message in log: ica server > c:\Windows\Temp\WebAccessImageTransferServiceDeveloper
				var transferEndtTime = System.DateTime.Now;
				string ErrorMessage = "Deidentification web service returns an failure response.";
				string ErroeDetails = "reason: Only objects that have a non-supported SOPClassUID  were found Patient Name: 03MEASUREMENTS^US^^^  PatientID: PID2003204  StudyInstanceUID: 1.2.840.113697.6.1.309639.1044908455";
				try
				{
					var loggedErrorMessage = string.Empty;
					var loggedErrorDetails = string.Empty;
					String Date = System.DateTime.Now.Date.ToString("yyyyMMdd");
					var ErrorMessageFound = false;
					for (var i = 1; i >= 1; i++)
					{
						String LogFilePath = @"C:\Windows\Temp\WebAccessImageTransferServiceDeveloper-" + Date + "(" + i + ")" + ".log";
						System.DateTime DateTime = System.DateTime.Now.Date;

						if (File.Exists(LogFilePath))
						{
							var LogValues = basePage.ReadDevTraceLog(LogFilePath, transferStartTime, transferEndtTime, isDeidentification: true);
							foreach (var entry in LogValues)
							{
								if (entry.Value["Source"].Contains("EAStudyDeidentification"))
								{
									loggedErrorMessage = entry.Value["Message"];
									loggedErrorDetails = entry.Value["Detail"];
									if (loggedErrorMessage == ErrorMessage && loggedErrorDetails == ErroeDetails)
									{
										ErrorMessageFound = true;
										break;
									}
										
								}
							}
						}
						else
						{
							Logger.Instance.ErrorLog("Unable to Read Log file");
							break;
						}
					}
					if (ErrorMessageFound)
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
				}
				catch (Exception e)
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					throw e;
				}


				//Return result
				result.FinalResult(ExecutedSteps);
				Logger.Instance.InfoLog("Overall Test status--" + result.status);

				return result;
			}
			catch (Exception e)
			{
				//Log Exception
				Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
				result.FinalResult(e, ExecutedSteps);
				Logger.Instance.ErrorLog("Overall Test status--" + result.status);
				return result;
			}
			finally
			{
				//Deleting uploaded study
				var hplogin = new HPLogin();
				BasePage.Driver.Navigate().GoToUrl("https://" + Config.EA7 + "/eaweb");
				var hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
				var workflow = (WorkFlow)hphome.Navigate("Workflow");
				workflow.EAv12SearchStudy("PatientID", PID, SelectStudy: true);
				workflow.EAv12DeleteStudy();
				hplogin.LogoutEAv12();
			}

		}

		/// <summary>
		///  Data Masking log verification for failed transfers - Integrator Mode
		/// </summary>
		/// <returns></returns>
		public TestCaseResult Test_163571(String testid, String teststeps, int stepcount)
		{
			TestCaseResult result = new TestCaseResult(stepcount);
			result.SetTestStepDescription(teststeps);
			int ExecutedSteps = -1;
			string PID = null;
			try
			{
				String adminUserName = Config.adminUserName;
				String adminPassword = Config.adminPassword;
				var studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
				PID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
				string ACC = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
				string ImageTransfercongif = @"C:\WebAccess\WindowsService\ImageTransfer\bin\ImageTransfer.exe.config";

				//Precondition
				try
				{
					var hplogin = new HPLogin();
					BasePage.Driver.Navigate().GoToUrl("https://" + Config.EA7 + "/eaweb");
					var hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
					var workflow = (WorkFlow)hphome.Navigate("Workflow");
					if (workflow.EAv12SearchStudy("PatientID", PID, SelectStudy: true))
					{
						workflow.EAv12DeleteStudy();
						hplogin.LogoutEAv12();
					}
				}
				catch (Exception e)
				{
					Logger.Instance.InfoLog("Error while deleting study from EA. " + e);
				}

				//Step 1 - Send studies to EA12
				DicomClient client = new DicomClient();
				client.AddRequest(new DicomCStoreRequest(Config.TestDataPath + studypath));
				client.Send(Config.EA7, 12000, false, "SCU", Config.EA7AETitle);
				Thread.Sleep(2000);//Study sending to EA

				//update the Config values.
				string DeveloperTraceSwitchValuesBeforeUpdate = basePage.GetAttributeValue(ImageTransfercongif, "/configuration/system.diagnostics/switches/add[@name='DeveloperTraceSwitch']", "value");
				basePage.ChangeAttributeValue(ImageTransfercongif, "configuration/system.diagnostics/switches/add[@name='DeveloperTraceSwitch']", "value", "Verbose");
				string DeveloperTraceSwitchValuesAfterUpdate = basePage.GetAttributeValue(ImageTransfercongif, "/configuration/system.diagnostics/switches/add[@name='DeveloperTraceSwitch']", "value");

				string WebAccessTraceSwitchValuesBeforeUpdate = basePage.GetAttributeValue(ImageTransfercongif, "/configuration/system.diagnostics/switches/add[@name='WebAccessTraceSwitch']", "value");
				basePage.ChangeAttributeValue(ImageTransfercongif, "configuration/system.diagnostics/switches/add[@name='WebAccessTraceSwitch']", "value", "Verbose");
				string WebAccessTraceSwitchValuesAfterUpdate = basePage.GetAttributeValue(ImageTransfercongif, "/configuration/system.diagnostics/switches/add[@name='WebAccessTraceSwitch']", "value");

				string PerformanceTraceSwitchValuesBeforeUpdate = basePage.GetAttributeValue(ImageTransfercongif, "/configuration/system.diagnostics/switches/add[@name='PerformanceTraceSwitch']", "value");
				basePage.ChangeAttributeValue(ImageTransfercongif, "configuration/system.diagnostics/switches/add[@name='PerformanceTraceSwitch']", "value", "Verbose");
				string PerformanceTraceSwitchValuesAfterUpdate = basePage.GetAttributeValue(ImageTransfercongif, "/configuration/system.diagnostics/switches/add[@name='PerformanceTraceSwitch']", "value");

				//servicetool.LaunchServiceTool();
				//servicetool.RestartIISandWindowsServices();
				//servicetool.CloseServiceTool();
				result.steps[++ExecutedSteps].StepPass();

				//Step2_1: Launch EHR and Load study
				//Initial Set-up
				servicetool.LaunchServiceTool();
				wpfobject.WaitTillLoad();
				servicetool.NavigateToTab("Integrator");
				wpfobject.WaitTillLoad();
				servicetool.EanbleUserSharing_ShadowUser(usersharing: "Always disabled");
				wpfobject.WaitTillLoad();
				servicetool.EnableHTML5(HTML5DefaultMode: false, EnableHTML5: true);
				servicetool.CloseServiceTool();

				//Launch the TestEHR application from iCA server C:\WebAccess\WebAccess\bin
				ehr.LaunchEHR();
				if (WpfObjects._mainWindow.Visible)
				{
					result.steps[++ExecutedSteps].AddPassStatusList();
				}
				else
				{
					result.steps[++ExecutedSteps].AddFailStatusList();
				}

				//Step 2_2 - Enter the Patient ID of the study that belongs to DS1  and click CMD line
				wpfobject.GetMainWindow("Test WebAccess EHR");
				wpfobject.SelectTabFromTabItems("Image Load");
				wpfobject.WaitTillLoad();
				ehr.SetCommonParameters(user: adminUserName);
				ehr.SetSelectorOptions(showSelector: "False", enableDownlaod: "True", enableTransfer: "True");
				ehr.SetSearchKeys_Study(ACC, datasources: login.GetHostName(Config.EA7));
				String url = ehr.clickCmdLine("ImageLoad");
				ehr.CloseEHR();

				//Step 2_3 - Load the generated URL in browser
				//Close browser
				basePage.CreateNewSesion();
				login = new Login();
				login.NavigateToIntegratorURL(url);
				StudyViewer studyviewer = (StudyViewer)login.NavigateToIntegratorFrame();
				PageLoadWait.WaitForThumbnailsToLoad(40);
				PageLoadWait.WaitForAllViewportsToLoad(40);
				if (studyviewer.PatientDetailsInViewer()["PatientID"].ToLower().Equals(PID.ToLower()))
				{
					result.steps[ExecutedSteps].AddPassStatusList();
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList();
				}
				var transferStartTime = System.DateTime.Now;
				try
				{
					studies.TransferStudy("Local System", SelectallPriors: false, ReviewTool: true, isDataMasking: true, FrameName: IntegratedFrameName);
					result.steps[ExecutedSteps].AddFailStatusList();
				}
				catch (Exception e)
				{
					result.steps[ExecutedSteps].AddPassStatusList();					
				}

				//Result for Step-2
				if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}
				//Step3: Verify message in log: ica server > c:\Windows\Temp\WebAccessImageTransferServiceDeveloper
				var transferEndtTime = System.DateTime.Now;
				string ErrorMessage = "Deidentification web service returns an failure response.";
				string ErroeDetails = "reason: Only objects that have a non-supported SOPClassUID  were found Patient Name: 03MEASUREMENTS^US^^^  PatientID: PID2003204  StudyInstanceUID: 1.2.840.113697.6.1.309639.1044908455";
				try
				{
					var loggedErrorMessage = string.Empty;
					var loggedErrorDetails = string.Empty;
					String Date = System.DateTime.Now.Date.ToString("yyyyMMdd");
					var ErrorMessageFound = false;
					for (var i = 1; i >= 1; i++)
					{
						String LogFilePath = @"C:\Windows\Temp\WebAccessImageTransferServiceDeveloper-" + Date + "(" + i + ")" + ".log";
						System.DateTime DateTime = System.DateTime.Now.Date;

						if (File.Exists(LogFilePath))
						{
							var LogValues = basePage.ReadDevTraceLog(LogFilePath, transferStartTime, transferEndtTime, isDeidentification: true);
							foreach (var entry in LogValues)
							{
								if (entry.Value["Source"].Contains("EAStudyDeidentification"))
								{
									loggedErrorMessage = entry.Value["Message"];
									loggedErrorDetails = entry.Value["Detail"];
									if (loggedErrorMessage == ErrorMessage && loggedErrorDetails == ErroeDetails)
									{
										ErrorMessageFound = true;
										break;
									}

								}
							}
						}
						else
						{
							Logger.Instance.ErrorLog("Unable to Read Log file");
							break;
						}
					}
					if (ErrorMessageFound)
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
				}
				catch (Exception e)
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
					throw e;
				}

				//Return result
				result.FinalResult(ExecutedSteps);
				Logger.Instance.InfoLog("Overall Test status--" + result.status);

				return result;
			}
			catch (Exception e)
			{
				//Log Exception
				Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
				result.FinalResult(e, ExecutedSteps);
				Logger.Instance.ErrorLog("Overall Test status--" + result.status);
				return result;
			}
			finally
			{
				BasePage.DeleteAllFileFolder(Config.downloadpath);
				//Deleting uploaded study
				var hplogin = new HPLogin();
				BasePage.Driver.Navigate().GoToUrl("https://" + Config.EA7 + "/eaweb");
				var hphome = hplogin.LoginEAv12(Config.hpUserName, Config.hpPassword, EA_URL: EA12Url);
				var workflow = (WorkFlow)hphome.Navigate("Workflow");
				workflow.EAv12SearchStudy("PatientID", PID, SelectStudy: true);
				workflow.EAv12DeleteStudy();
				hplogin.LogoutEAv12();
			}

		}

	}
}

