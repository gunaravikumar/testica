using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Runtime.Serialization;

namespace Selenium.Scripts.Tests
{
	class ToolBoxConfiguration 
	{
		public Login login { get; set; }	
		public Configure configure { get; set; }		
		public string filepath { get; set; }
        public static WebDriverWait wait { get; set; }
        DomainManagement domainmanagement = new DomainManagement();
        UserManagement usermanagement = new UserManagement();
        RoleManagement rolemanagement = new RoleManagement();
        BasePage basepage = new BasePage();     
        public String DefaultTools = "Window Level,AutoWL,Invert:Interactive Zoom,Magnifier:Pan:Line Measurement,Calibration Tool,Transischial Measurement:Scroll Tool:Angle Measurement,Cobb Angle,Horizontal Plumb Line,Vertical Plumb Line,Joint Line Measurement:Draw Ellipse,Draw Rectangle,Draw ROI:Rotate Clockwise,Rotate Counterclockwise:Flip Horizontal,Flip Vertical:Get Pixel Value:Add Text,Free Draw,Remove All Annotations:Reset";        
        public String DefaultAvailableTools = "Save Annotated Images,Save Series,Image Scope,Series Scope";

        public ToolBoxConfiguration(String classname)
		{
			login = new Login();
			login.DriverGoTo(login.url);
			configure = new Configure();
            wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 60));
            wait.IgnoreExceptionTypes(new Type[] { (new StaleElementReferenceException().GetType()) });

            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
		}

		/// <summary>
		///  Default tools for all the modalities
		/// </summary>
		public TestCaseResult Test_161519(String testid, String teststeps, int stepcount)
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
                String DefaultToolsList = this.DefaultTools;
                String DefaultToolsListInViewer = this.DefaultTools;
				String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
				String[] Accession = AccessionList.Split(':');
				String[] DefaultTools = DefaultToolsList.Split(':');
				String[] DefaultToolsInViewer = DefaultToolsListInViewer.Split(':');

                String DefaultToolsListForCRModality = this.DefaultTools;

                String[] DefaultToolsForCRModality = DefaultToolsListForCRModality.Split(':');

				DomainManagement domain = new DomainManagement();
				UserManagement usermanagement = new UserManagement();

				//Step 1 - Login as Administrator 
				login.LoginIConnect(adminUserName, adminPassword);
				ExecutedSteps++;

				//Step 2 - Domain Management page should be displayed by default
				if (login.IsTabSelected("Domain Management"))
				{
                    result.steps[++ExecutedSteps].StepPass();
				}
				else
				{
                    result.steps[++ExecutedSteps].StepFail();
				}


				//Step 3 - Create new:
				// 1. Test Domain
				// 2. role(Regular User) under the Test Domain(so you will have the Test Domain Admin role and the Regular User role)
				// domain admin(Administrator Test Domain)
				// regular user(User1, belonging to Test Domain and with a Regular User
				String TestDomain = "TestDomain_145_" + new Random().Next(1, 1000); 
				String Role = "Role_145_" + new Random().Next(1, 1000);
				String DomainAdmin = "DomainAdmin_145_" + new Random().Next(1, 1000); 
				String rad1 = "Rad_145_" + new Random().Next(1, 1000);
								
				domain.CreateDomain(TestDomain, TestDomain, TestDomain, DomainAdmin, null, DomainAdmin, DomainAdmin, DomainAdmin, Role, Role);				
				login.Navigate("UserManagement");
				usermanagement.CreateUser(rad1, TestDomain, Role);
				ExecutedSteps++; 

				// Step4 - Log out from Administrator user.
				login.Logout();
                Thread.Sleep(2000);
                BasePage.Driver.SwitchTo().DefaultContent();
				if (login.IsElementVisible(By.CssSelector("input[id$= '_LoginMasterContentPlaceHolder_Username")))
				{
                    result.steps[++ExecutedSteps].StepPass();
				}
				else
				{
                    result.steps[++ExecutedSteps].StepFail();
				}

				//Step5  - Log in as Test Domain Administrator.
				login.LoginIConnect(DomainAdmin, DomainAdmin);
				ExecutedSteps++; 

				//Step6 - Go to Domain Management page.
				domain.NavigateToDomainManagementTab();
				if (login.IsTabSelected("Domain Management"))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step7 - Verify the selected value in the Modality dropdown
				//BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				BasePage.Driver.SwitchTo().Frame("TabContent");
				SelectElement Modality = new SelectElement(BasePage.Driver.FindElement
												(By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
				if (Modality.SelectedOption.Text.Equals("default"))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step8 - Verify the default tools should be displayed under Toolbox configuration.
				var groupsInUse = domain.GetGroupsInToolBoxConfig();				
				bool step8_1 = groupsInUse.Count() == 12;
				bool step8_2 = domain.VerifyConfiguredToolsInToolBoxConfig(DefaultTools);				
				if (step8_1 && step8_2)
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step9  - Navigate to Studies tab,search and select any study and then click on 'Universal' button				
				var studies = (Studies)login.Navigate("Studies");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.ClearSearchBtn()));
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
				studies.SelectStudy("Accession", Accession[0]);
				var viewer = BluRingViewer.LaunchBluRingViewer();
				PageLoadWait.WaitForFrameLoad(20);
				ExecutedSteps++;

				//Step10 - Select any series viewport and click on right mouse button
				viewer.OpenViewerToolsPOPUp();
				if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Setp11 - Verify the default tools should be displayed in the toolbox.
				var toolsInViewer = viewer.GetGroupsInToolBox();
				bool step11_1 = toolsInViewer.Count() == 12;
                bool step11_2 = viewer.GetToolsInToolBoxByGrid().SequenceEqual(DefaultToolsInViewer.ToList());
				if (step11_1 && step11_2)
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description +"ToolsCount :"+step11_1 +"ToolsVerfication");
					result.steps[ExecutedSteps].SetLogs();
				}

				// Step12 - Select any tool from the default toolbox and apply it.
				viewer.SelectViewerTool(BluRingTools.Line_Measurement, isOpenToolsPOPup:false);
				viewer.ApplyTool_LineMeasurement();
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
				bool step12 = studies.CompareImage(result.steps[ExecutedSteps],
								viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
				if (step12)
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

				//Step13 - Go to Domain Management page.
				viewer.CloseBluRingViewer();
				domain.NavigateToDomainManagementTab();
				if (login.IsTabSelected("Domain Management"))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step14 - Select any Modality from the Modality dropdown and verify the default Modality toolbox tools should be displayed.
				//BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				BasePage.Driver.SwitchTo().Frame("TabContent");
				Modality = new SelectElement(BasePage.Driver.FindElement
												(By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
				Modality.SelectByText("CR");
				Thread.Sleep(2000);
				groupsInUse = domain.GetGroupsInToolBoxConfig();
				bool step14_1 = groupsInUse.Count() == 12;
				bool step14_2 = domain.VerifyConfiguredToolsInToolBoxConfig(DefaultToolsForCRModality);

				if (step14_1 && step14_2)
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Logout Application				
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
		///  At Domain level: Configure tools to the specific modality toolbox
		/// </summary>
		public TestCaseResult Test_161520(String testid, String teststeps, int stepcount)
		{   
			//Declare and initialize variables         
			TestCaseResult result = null;
			int ExecutedSteps = -1;
			try
			{
				result = new TestCaseResult(stepcount);
				result.SetTestStepDescription(teststeps);

				DomainManagement domain = new DomainManagement();
				String adminUserName = Config.adminUserName;
				String adminPassword = Config.adminPassword;
				String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
				String[] Accession = AccessionList.Split(':');
                String[] AddNewTool = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "AddNewTools")).Split(',');

                //Step 1 - Login as Administrator
                login.DriverGoTo(login.url);
				login.LoginIConnect(adminUserName, adminPassword);
				if (login.IsTabPresent("Studies"))
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

				//Step2 - In Domain Management->Edit Domain page->Toolbox Configuration section, select Modality as CR.
				login.NavigateToDomainManagementTab();
                domain.SearchDomain("SuperAdminGroup");
				domain.SelectDomain("SuperAdminGroup");
				domain.ClickEditDomain();
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				SelectElement Modality = new SelectElement(BasePage.Driver.FindElement
												(By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
				Modality.SelectByText("CR");
				ExecutedSteps++;

                //Step3 - Drag some tools, for ex. Free Draw, Roi Draw to the toolbox configuration and Save the changes.				
                IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));// li:nth-of-type(3)"));				
                IWebElement group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)"));               
                var dictionary = new Dictionary<String, IWebElement>();

                dictionary.Add(AddNewTool[0], group1);
                dictionary.Add(AddNewTool[1], group2);
				bool step_3 = domain.AddToolsToToolbox(dictionary, "CR", true);			
				bool step3 = domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[0]) && domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[1]);
				domain.ClickSaveEditDomain();
				if (step_3 && !step3)
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step4 - Navigate to Studies tab,search and Load a study which contains series of CR Modality and then click on 'Universal' button
				var studies = (Studies)login.Navigate("Studies");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.ClearSearchBtn()));
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA1));
				studies.SelectStudy("Accession", Accession[0]);
				var viewer = BluRingViewer.LaunchBluRingViewer();
				PageLoadWait.WaitForFrameLoad(20);
				ExecutedSteps++;

				//Step5 - Select any series viewport and click on right mouse button
				viewer.OpenViewerToolsPOPUp();
				if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

                //Step6 - Verify the configured tools appear in the floating toolbox for the CR Modality as specified in Domain Management page.             
                String[] Tools = DefaultTools.Split(':');
                Tools[0] = Tools[0] + "," + AddNewTool[0];
                Tools[1] = Tools[1] + "," + AddNewTool[1];
                var toolsInViewer = viewer.GetGroupsInToolBox();
				bool step6_1 = toolsInViewer.Count() == 12;
				bool step6_2 = viewer.VerifyConfiguredTools(Tools);
				if (step6_1 && step6_2)
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				// Step7 - Select any tool from the toolbox and apply it.
				viewer.SelectViewerTool(BluRingTools.Line_Measurement, isOpenToolsPOPup:false);
				viewer.ApplyTool_LineMeasurement();
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
				bool step7 = studies.CompareImage(result.steps[ExecutedSteps],
								viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
				if (step7)
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
                viewer.CloseBluRingViewer();

                //Step8 - Edit the Domain Management Page for administrator account.
                login.NavigateToDomainManagementTab();
				domain.SelectDomain("SuperAdminGroup");
				domain.ClickEditDomain();
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
				Modality = new SelectElement(BasePage.Driver.FindElement
												(By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
				Modality.SelectByText("CR");
				ExecutedSteps++;                

                //Step9 - Change the order of the tools by drag and drop them at the desired position.				
                group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(5)"));				
                group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(10)"));

                dictionary = new Dictionary<String, IWebElement>();
				dictionary.Add(AddNewTool[0], group1);
				dictionary.Add(AddNewTool[1], group2);				
				domain.RepositionToolsInConfiguredToolsSection(dictionary, "CR", addToolAtEnd:true);
                Tools = DefaultTools.Split(':');
                Tools[4] = Tools[4] + "," + AddNewTool[0];
                Tools[9] = Tools[9] + "," + AddNewTool[1];         
				var groupsInUse = domain.GetGroupsInToolBoxConfig();
				bool step9_1 = groupsInUse.Count() == 12;
				bool step9_2 = domain.VerifyConfiguredToolsInToolBoxConfig(Tools);
				if (step9_1 && step9_2)
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step10 - Remove a tool from the New list, by selecting it then drag and drop it to the Available Items list.				
				var ToolsToBeRemoved = new List<String>();
                ToolsToBeRemoved.Add(AddNewTool[0]);
                domain.RemoveToolsFromConfiguredSection(ToolsToBeRemoved, "CR");				
                Thread.Sleep(3000);
				if (!domain.GetConfiguredToolsInToolBoxConfig().Contains(AddNewTool[0]))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step11 - Add back the removed tool to the New list.				
				group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(4)"));
				dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add(AddNewTool[0], group1);
                domain.AddToolsToToolbox(dictionary, "CR");				
                Thread.Sleep(3000);
                if (domain.GetConfiguredToolsInToolBoxConfig().Contains(AddNewTool[0]))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
                domain.ClickSaveEditDomain();

                //Step12 - Navigate to Studies tab,search and Load a study which contains series of CR Modality and then click on 'Universal' button
                studies = (Studies)login.Navigate("Studies");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.ClearSearchBtn()));
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA1));
				studies.SelectStudy("Accession", Accession[0]);
				viewer = BluRingViewer.LaunchBluRingViewer();
				PageLoadWait.WaitForFrameLoad(20);
				ExecutedSteps++;

				//Step13 - Select any series viewport and click on right mouse button
				viewer.OpenViewerToolsPOPUp();
				if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step14 - Verify the Modality toolbox updates to reflect the configuration for the CR modality as specified in Domain Management page.				
				Tools = DefaultTools.Split(':');
                Tools[3] = Tools[3] + "," + AddNewTool[0];
                Tools[9] = Tools[9] + "," + AddNewTool[1];
                toolsInViewer = viewer.GetGroupsInToolBox();
				bool step15_1 = toolsInViewer.Count() == 12;
				bool step15_2 = viewer.VerifyConfiguredTools(Tools);
				if (step15_1 && step15_2)
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				// Step15 - Select any tool from the toolbox and apply it.
				viewer.SelectViewerTool(BluRingTools.Line_Measurement, isOpenToolsPOPup:false);
                viewer.ApplyTool_LineMeasurement();
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
				bool step15 = studies.CompareImage(result.steps[ExecutedSteps],
								viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
				if (step15)
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

                //Logout Application
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
			finally
			{
				DomainManagement domain = new DomainManagement();
				login.LoginIConnect("Administrator", "Administrator");
				login.NavigateToDomainManagementTab();
				domain.SelectDomain("SuperAdminGroup");
				domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                SelectElement Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));                               
                Modality.SelectByText("CR");
                Thread.Sleep(1000);
                IWebElement revertButton = BasePage.Driver.FindElement(By.CssSelector(DomainManagement.btn_revertToDefault));
                if (revertButton.Enabled)
                    revertButton.Click();                
                domain.ClickSaveEditDomain();
                login.Logout();
            }
		}

        /// <summary>
		///  "Revert to Default” button
		/// </summary>
		public TestCaseResult Test_161524(String testid, String teststeps, int stepcount)
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
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] Accession = AccessionList.Split(':');
                String[] PatientID = PatientIDList.Split(':');
                String[] DefaultToolsList = DefaultTools.Split(':');
                String[] AddNewTool = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "AddNewTools")).Split(',');

                DomainManagement domain = new DomainManagement(); 

                //Step 1 - Login as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                if (login.IsTabPresent("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                // Precondition - Create Domain
                String TestDomain = "TestDomain_15_" + new Random().Next(1, 1000);
                String Role = "Role_15_" + new Random().Next(1, 1000);
                String DomainAdmin = "DomainAdmin_15_" + new Random().Next(1, 1000);
                String rad1 = "Rad_15_" + new Random().Next(1, 1000);

                domain.CreateDomain(TestDomain, TestDomain, TestDomain, DomainAdmin, null, DomainAdmin, DomainAdmin, DomainAdmin, Role, Role);

                //Step2 - In Domain Management->Edit Domain page->Toolbox Configuration section, select Modality as CT.
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(TestDomain);
                domain.SelectDomain(TestDomain);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                SelectElement Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("CT");             

                //Verify the default tools should be displayed under Toolbox configuration.
                var groupsInUse = domain.GetGroupsInToolBoxConfig();
                bool step2_1 = groupsInUse.Count() == 12;
                bool step2_2 = domain.VerifyConfiguredToolsInToolBoxConfig(DefaultToolsList);
                if (step2_1 && step2_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                domain.ClickCloseEditDomain();

                //Step3  - Verify the Modality default tools should be displayed on viewer				
                var studies = (Studies)login.Navigate("Studies");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.ClearSearchBtn()));
                studies.SearchStudy(patientID: PatientID[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);

                // Verify the default tools should be displayed in the toolbox.
                viewer.OpenViewerToolsPOPUp();
                var toolsInViewer = viewer.GetGroupsInToolBox();
                bool step3_1 = toolsInViewer.Count() == 12;
                bool step3_2 = viewer.VerifyConfiguredTools(DefaultToolsList);
                if (step3_1 && step3_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseBluRingViewer(); 

                // Step4 - 	Verify the "Revert to Default" should be disabled by default.
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(TestDomain);
                domain.SelectDomain(TestDomain);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("CT");
                var revertToDefaultButton = BasePage.Driver.FindElement(By.CssSelector(DomainManagement.btn_revertToDefault));
                if(!revertToDefaultButton.Enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step5 - Drag some tools, for ex. Calibration, Pixel Value, Line Measurement to the toolbox configuration and Save the changes.               
                IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));                
                IWebElement group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)"));
                var dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add(AddNewTool[0], group1);
                dictionary.Add(AddNewTool[1], group2);
               	bool step_5 = domain.AddToolsToToolbox(dictionary, "CT", true);
                bool step5 = domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[0]) || domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[1]);
                domain.ClickSaveEditDomain();
                if (step_5 && !step5)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step6 - Verify the "Revert to Default" button should get enabled
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(TestDomain);
                domain.SelectDomain(TestDomain);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("CT");
                revertToDefaultButton = BasePage.Driver.FindElement(By.CssSelector(DomainManagement.btn_revertToDefault));
                if (revertToDefaultButton.Enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step7 - Click on "Revert to Default" button
                //Step8 - Verify the "Revert to default" should revert the changes to the parent/default settings.
                revertToDefaultButton.Click();
                ExecutedSteps++;
                Thread.Sleep(4000);    
                bool step8_1 = domain.GetConfiguredToolsInToolBoxConfig().Contains(AddNewTool[0]) || domain.GetConfiguredToolsInToolBoxConfig().Contains(AddNewTool[0]);
                bool step8_2 = domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[1]) && domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[1]);               
                if (!step8_1 && step8_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step9 - Revert to Default" button should be in disabled state when the user clicks on "Revert to Default" button.
                revertToDefaultButton = BasePage.Driver.FindElement(By.CssSelector(DomainManagement.btn_revertToDefault));
                if (!revertToDefaultButton.Enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                string[] CT_tools = domain.GetToolsInToolBoxConfigByEachColumn().ToArray();

                //Step10 - Select any modality(ex. CR) from Modality Drop-down,Drag some tools, for ex. as Reset, Zoom to New box.  
                Modality = new SelectElement(BasePage.Driver.FindElement
                                               (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("CR");
                group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)"));
                dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add(AddNewTool[0], group1);
                dictionary.Add(AddNewTool[1], group2);
                bool step10_1 = domain.AddToolsToToolbox(dictionary, "CR", true);
                bool step10_2 = domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[0]) || domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[1]);
                bool step10_3 = domain.GetToolsInGroupInToolBoxConfig(group1).Contains(AddNewTool[0]);
                bool step10_4 = domain.GetToolsInGroupInToolBoxConfig(group2).Contains(AddNewTool[1]);
                if (step10_1 && !step10_2 && step10_3 && step10_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step11 - Change the order of the tools by drag and drop them at the desired position.
                dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add(AddNewTool[2], group1);
                dictionary.Add(AddNewTool[0], group2);
                domain.RepositionToolsInConfiguredToolsSection(dictionary, "CR", true);
                bool step11_1 = domain.GetToolsInGroupInToolBoxConfig(group1).Contains(AddNewTool[2]);
                bool step11_2 = domain.GetToolsInGroupInToolBoxConfig(group2).Contains(AddNewTool[0]);
                if (step11_1 && step11_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step12 - Click on 'Save' button
                string[] CR_tools = domain.GetToolsInToolBoxConfigByEachColumn().ToArray();
                domain.ClickSaveEditDomain();
                if (login.IsTabSelected("Domain Management"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step13 - Again click on Edit Domain page -> Toolbox Configuration section, select Modality as ex. US.
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(TestDomain);
                domain.SelectDomain(TestDomain);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("US");
                ExecutedSteps++;

                //Step14 - Drag some tools, for ex. as Reset, Zoom to New box.               
                group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)"));
                dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add(AddNewTool[3], group1);
                dictionary.Add(AddNewTool[4], group2);
                bool step14_1 = domain.AddToolsToToolbox(dictionary, "US", true);
                bool step14_2 = domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[3]) || domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[4]);
                bool step14_3 = domain.GetToolsInGroupInToolBoxConfig(group1).Contains(AddNewTool[3]);
                bool step14_4 = domain.GetToolsInGroupInToolBoxConfig(group2).Contains(AddNewTool[4]);
                if (step14_1 && !step14_2 && step14_3 && step14_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step15 - Change the order of the tools by drag and drop them at the desired position.
                dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add(AddNewTool[4], group1);
                dictionary.Add(AddNewTool[2], group2);
                domain.RepositionToolsInConfiguredToolsSection(dictionary, "US", true);
                bool step15_1 = domain.GetToolsInGroupInToolBoxConfig(group1).Contains(AddNewTool[4]);
                bool step15_2 = domain.GetToolsInGroupInToolBoxConfig(group2).Contains(AddNewTool[2]);
                if (step15_1 && step15_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step16 - Verify the "Revert to Default" button should get enabled
                revertToDefaultButton = BasePage.Driver.FindElement(By.CssSelector(DomainManagement.btn_revertToDefault));
                if (revertToDefaultButton.Enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step17 - Click on 'Save' button
                string[] US_tools = domain.GetToolsInToolBoxConfigByEachColumn().ToArray();
                domain.ClickSaveEditDomain();
                if (login.IsTabSelected("Domain Management"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step18 - In Domain Management-> Edit Domain page -> Toolbox Configuration section, select previous used Modality as ex. CR               
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(TestDomain);
                domain.SelectDomain(TestDomain);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("CR");             

                String[] Tools = DefaultTools.Split(':');
                Tools[0] = Tools[0] + "," + AddNewTool[2];
                Tools[1] = Tools[1] + "," + AddNewTool[1] + "," + AddNewTool[0];
                Tools[2] = "";             
                groupsInUse = domain.GetGroupsInToolBoxConfig();
                bool step18_1 = groupsInUse.Count() == 12;
                bool step18_2 = domain.VerifyConfiguredToolsInToolBoxConfig(CR_tools);
                if (step18_1 && step18_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step19 - Select the Modality as in the previous test (i.e. US) from the “Copy From” dropdown box
                Modality = new SelectElement(BasePage.Driver.FindElement
                                              (By.CssSelector(BasePage.select_toolBoxConfiguration_CopyFromDropdown)));
                Modality.SelectByText("US");
                Thread.Sleep(2000);            
                Tools = DefaultTools.Split(':');
                Tools[0] = Tools[0] + "," + AddNewTool[3] + "," + AddNewTool[4];
                Tools[1] = Tools[1] + "," + AddNewTool[2];
                Tools[2] = "";
                String[] updatedTools = Tools.Where(str => str != "").ToArray();
                groupsInUse = domain.GetGroupsInToolBoxConfig();
                bool step19_1 = groupsInUse.Count() == 12;
                bool step19_2 = domain.VerifyConfiguredToolsInToolBoxConfig(US_tools);
                if (step19_1 && step19_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step20 - Verify the "Revert to Default" button should get enabled
                revertToDefaultButton = BasePage.Driver.FindElement(By.CssSelector(DomainManagement.btn_revertToDefault));
                if (revertToDefaultButton.Enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step21 - 	Click on "Revert to Default" button
                revertToDefaultButton.Click();
                Thread.Sleep(1000);
                bool step21_1 = domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[4]);
                bool step21_2 = domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[3]);
                IWebElement group3 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(3)"));
                bool step21_3 = domain.GetToolsInGroupInToolBoxConfig(group3).Contains(AddNewTool[2]);
                if (step21_1 && step21_2 && step21_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step22 -Verify that the tools should be reverted for the modality selected in the Modality dropdown
                groupsInUse = domain.GetGroupsInToolBoxConfig();
                bool step22_1 = groupsInUse.Count() == 12;
                bool step22_2 = domain.VerifyConfiguredToolsInToolBoxConfig(DefaultToolsList);
                if (step22_1 && step22_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step23 - 	Select the US modality from the Modality dropdown and verify that the configured tools should not be reverted
                Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("US");
                Thread.Sleep(2000);
                Tools = DefaultTools.Split(':');
                Tools[0] = Tools[0] + "," + AddNewTool[3] + "," + AddNewTool[4];                
                Tools[2] = "";
                Tools[1] = Tools[1] + "," + AddNewTool[2];           
                groupsInUse = domain.GetGroupsInToolBoxConfig();
                bool step23_1 = groupsInUse.Count() == 12;
                bool step23_2 = domain.VerifyConfiguredToolsInToolBoxConfig(US_tools);
                if (step23_1 && step23_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout Application				
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
		///  'Copy From' dropdown box: User shall able to copy the Toolbox configuration from another modality by selecting another modality
		/// </summary>
		public TestCaseResult Test_161522(String testid, String teststeps, int stepcount)
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
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] AddNewTool = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "AddNewTools")).Split(',');
                String[] Accession = AccessionList.Split(':');
                String[] PatientID = PatientIDList.Split(':');
                String[] DefaultToolsList = DefaultTools.Split(':');               

                DomainManagement domain = new DomainManagement();

                //Step 1 - Login as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                if (login.IsTabPresent("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }               

                //Step2 - In Domain Management->Edit Domain page->Toolbox Configuration section, select Modality as CT.
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                SelectElement Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("CT");             

                //Verify the default tools should be displayed under Toolbox configuration.
                var groupsInUse = domain.GetGroupsInToolBoxConfig();
                bool step2_1 = groupsInUse.Count() == 12;
                bool step2_2 = domain.VerifyConfiguredToolsInToolBoxConfig(DefaultToolsList);
                if (step2_1 && step2_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                domain.ClickCloseEditDomain();

                //Step3  - Verify the default tools should be displayed on viewer.				
                var studies = (Studies)login.Navigate("Studies");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.ClearSearchBtn()));
                studies.SearchStudy(patientID: PatientID[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);

                // Verify the default tools should be displayed in the toolbox.
                viewer.OpenViewerToolsPOPUp();
                var toolsColumnInViewer = viewer.GetGroupsInToolBox();
                bool step3_1 = toolsColumnInViewer.Count() == 12;
                bool step3_2 = viewer.VerifyConfiguredTools(DefaultToolsList);
                if (step3_1 && step3_2)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.ErrorLog("ToolsInViewer" + string.Join(",", viewer.GetToolsInToolBoxByGrid().ToArray()));
                    Logger.Instance.ErrorLog("ToolsIn Domain" + string.Join(",", DefaultToolsList.ToArray()));
                    result.steps[++ExecutedSteps].StepFail();
                }
                viewer.CloseBluRingViewer(); 

                //Step4 - Drag some tools, for ex. Calibration, Pixel Value, Line Measurement to the toolbox configuration and Save the changes. 
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                //wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups)));
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.CssSelector(BasePage.ul_toolBoxConfiguration_AvailableTools)));                
                
                IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                IWebElement group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)"));
                var dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add(AddNewTool[0], group1);
                dictionary.Add(AddNewTool[1], group2);
               	bool step_4 = domain.AddToolsToToolbox(dictionary, "CT", true);               
                bool step4 = domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[0]) || domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[1]);
                domain.ClickSaveEditDomain();
                if (step_4 && !step4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step5 - Navigate to Studies tab,search and Load a study which contains series of CT Modality and then click on 'Universal' button
                studies = (Studies)login.Navigate("Studies");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.ClearSearchBtn()));
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Accession", Accession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step6 - Select any series viewport and click on right mouse button
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step7 - Verify the configured tools appear in the floating toolbox for the CT Modality as specified in Domain Management page.               
                String[] Tools = DefaultTools.Split(':');
                Tools[0] = Tools[0] + "," + AddNewTool[0];
                Tools[1] = Tools[1] + "," + AddNewTool[1];
                toolsColumnInViewer = viewer.GetGroupsInToolBox();
                bool step7_1 = toolsColumnInViewer.Count() == 12;
                bool step7_2 = viewer.VerifyConfiguredTools(Tools);
                if (step7_1 && step7_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step8 - Select any tool from the toolbox and apply it.
                viewer.SelectViewerTool(BluRingTools.Line_Measurement, isOpenToolsPOPup:false);
                viewer.ApplyTool_LineMeasurement();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step8 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step8)
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
                viewer.CloseBluRingViewer();

                //Step 9 - Go to Domain Management,Edit the Domain Management Page for administrator account.
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                ExecutedSteps++;

                //Step10 - Select another Modality(e.g. MR) from the 'Modality' dropdown box
                Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("MR");
                Thread.Sleep(2000);

                //Verify the default tools should be displayed under Toolbox configuration.
                groupsInUse = domain.GetGroupsInToolBoxConfig();
                bool step10_1 = groupsInUse.Count() == 12;
                bool step10_2 = domain.VerifyConfiguredToolsInToolBoxConfig(DefaultToolsList);
                if (step10_1 && step10_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step11 - Select the same Modality as in the previous test (i.e. CT) from the “Copy From” dropdown box
                Modality = new SelectElement(BasePage.Driver.FindElement
                                               (By.CssSelector(BasePage.select_toolBoxConfiguration_CopyFromDropdown)));
                Modality.SelectByText("CT");
                Thread.Sleep(2000);

                //Verify Configured Tools                  
                String[] ConfiguredToolsForCT = DefaultTools.Split(':');
                ConfiguredToolsForCT[0] = ConfiguredToolsForCT[0] + "," + AddNewTool[0];
                ConfiguredToolsForCT[1] = ConfiguredToolsForCT[1] + "," + AddNewTool[1];
                groupsInUse = domain.GetGroupsInToolBoxConfig();
                bool step11_1 = groupsInUse.Count() == 12;
                bool step11_2 = domain.VerifyConfiguredToolsInToolBoxConfig(ConfiguredToolsForCT);
                if (step11_1 && step11_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step12 - Click on "Save" button
                domain.ClickSaveEditDomain();
                if(BasePage.Driver.FindElements(By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)).Count == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step13 - Navigate to Studies tab, search and Load a study which contains series of MR Modality and then click on 'Universal' button
                studies = (Studies)login.Navigate("Studies");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.ClearSearchBtn()));
                studies.SearchStudy(AccessionNo: Accession[1], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step14 - Select any series viewport and click on right mouse button
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step15 - Verify the configured CT Modality toolbox should be copied to MR modality toolbox.               
                toolsColumnInViewer = viewer.GetGroupsInToolBox();
                bool step15_1 = toolsColumnInViewer.Count() == 12;
                bool step15_2 = viewer.VerifyConfiguredTools(ConfiguredToolsForCT);
                if (step15_1 && step15_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step16 - Select any tool from the modality toolbox and apply it.
                viewer.SelectViewerTool(BluRingTools.Line_Measurement, isOpenToolsPOPup:false);
                viewer.ApplyTool_LineMeasurement();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step16 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step16)
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

                //Logout Application
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
            finally
            {
                DomainManagement domain = new DomainManagement();
                login.LoginIConnect("Administrator", "Administrator");
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                SelectElement Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("MR");
                Thread.Sleep(1000);
                IWebElement ele = BasePage.Driver.FindElement(By.CssSelector(DomainManagement.btn_revertToDefault));
                if (ele.Enabled)
                    ele.Click();
                Modality.SelectByText("CT");
                Thread.Sleep(1000);
                ele = BasePage.Driver.FindElement(By.CssSelector(DomainManagement.btn_revertToDefault));
                if (ele.Enabled)
                    ele.Click();
                domain.ClickSaveEditDomain();
                login.Logout();
            }
        }


        /// <summary>
        /// 'Copy from' dropdown box: User able to overwrite the toolbox configuration for each modality
        /// </summary>
        public TestCaseResult Test_161523(String testid, String teststeps, int stepcount)
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
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] Accession = AccessionList.Split(':');
                String[] PatientID = PatientIDList.Split(':');              
                String[] AddNewTool = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "AddNewTools")).Split(',');

                DomainManagement domain = new DomainManagement();

                //Step 1 - Login as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                if (login.IsTabPresent("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step2 - In Domain Management->Edit Domain page->Toolbox Configuration section, select Modality as CT.
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                SelectElement Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("CT");

                //Verify the default tools should be displayed under Toolbox configuration.
                var groupsInUse = domain.GetGroupsInToolBoxConfig();
                bool step2_1 = groupsInUse.Count() == 12;
                bool step2_2 = domain.VerifyConfiguredToolsInToolBoxConfig(DefaultTools.Split(':'));
                if (step2_1 && step2_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                domain.ClickCloseEditDomain();

                //Step3  - Verify the default tools should be displayed on viewer.				
                var studies = (Studies)login.Navigate("Studies");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.ClearSearchBtn()));
                studies.SearchStudy(patientID: PatientID[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);

                // Verify the default tools should be displayed in the toolbox.
                viewer.OpenViewerToolsPOPUp();
                var toolsInViewer = viewer.GetGroupsInToolBox();
                bool step3_1 = toolsInViewer.Count() == 12;
                bool step3_2 = viewer.VerifyConfiguredTools(DefaultTools.Split(':'));
                if (step3_1 && step3_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseBluRingViewer(); 

                //Step4 - Drag some tools, for ex. Calibration, Pixel Value, Line Measurement to the toolbox configuration and Save the changes. 
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                //wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups))); 
                IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                IWebElement group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)"));
                var dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add(AddNewTool[0], group1);
                dictionary.Add(AddNewTool[1], group2);
                bool step_4 = domain.AddToolsToToolbox(dictionary, "CT", true);               
                bool step4 = domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[0]) || domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[1]);
                domain.ClickSaveEditDomain();
                if (step_4 && !step4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step5 - Select another Modality(e.g. MR) from the 'Modality' dropdown box
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups)));               
                Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("MR");
                Thread.Sleep(2000);
                //Verify the default tools should be displayed under Toolbox configuration.
                groupsInUse = domain.GetGroupsInToolBoxConfig();
                bool step5_1 = groupsInUse.Count() == 12;
                bool step5_2 = domain.VerifyConfiguredToolsInToolBoxConfig(DefaultTools.Split(':'));
                bool step5_3 = domain.GetAvailableToolsInToolBoxConfig().Count > 0;
                if (step5_1 && step5_2 && step5_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step6 - Select the same Modality as in the previous test (i.e. CT) from the “Copy From” dropdown box
                Modality = new SelectElement(BasePage.Driver.FindElement
                                               (By.CssSelector(BasePage.select_toolBoxConfiguration_CopyFromDropdown)));
                Modality.SelectByText("CT");
                Thread.Sleep(2000);

                //Verify Configured Tools               
                String[] ConfiguredToolsForCT = DefaultTools.Split(':');
                ConfiguredToolsForCT[0] = ConfiguredToolsForCT[0] + "," + AddNewTool[0];
                ConfiguredToolsForCT[1] = ConfiguredToolsForCT[1] + "," + AddNewTool[1];
                groupsInUse = domain.GetGroupsInToolBoxConfig();
                bool step6_1 = groupsInUse.Count() == 12;
                bool step6_2 = domain.VerifyConfiguredToolsInToolBoxConfig(ConfiguredToolsForCT);
                if (step6_1 && step6_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step7 - Click on "Save" button.
                domain.ClickSaveEditDomain();
                if (BasePage.Driver.FindElements(By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)).Count == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step8 - Navigate to Studies tab,search and Load a study which contains series of MR Modality and then click on 'Universal' button                
                studies = (Studies)login.Navigate("Studies");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.ClearSearchBtn()));
                studies.SearchStudy(AccessionNo: Accession[1], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step9 - Select any series viewport and click on right mouse button
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step10 - Verify the configured CT Modality toolbox should be copied to MR modality toolbox.             
                toolsInViewer = viewer.GetGroupsInToolBox();
                bool step10_1 = toolsInViewer.Count() == 12;
                bool step10_2 = viewer.VerifyConfiguredTools(ConfiguredToolsForCT);
                if (step10_1 && step10_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step11 - Select any tool from the modality toolbox and apply it.
                viewer.SelectViewerTool(BluRingTools.Pan, isOpenToolsPOPup:false);
                viewer.ApplyTool_Pan();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step11 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step11)
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
                viewer.CloseBluRingViewer();

                //Step12 - Navigate to Domain Management -*^>^* Edit Domain page -*^>^* Toolbox Configuration section, select CT Modality
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("CT");
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups)));
                Thread.Sleep(1000);

                //Verify Configured Tools             
                groupsInUse = domain.GetGroupsInToolBoxConfig();
                bool step12_1 = groupsInUse.Count() == 12;
                bool step12_2 = domain.VerifyConfiguredToolsInToolBoxConfig(ConfiguredToolsForCT);
                if (step12_1 && step12_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step13 - Drag some tools, for ex. as Pan, Zoom, W/L, Magnifier into the cell in the toolbox.
                group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(4)"));                
                dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add(AddNewTool[2], group1);               
                bool step_13 = domain.AddToolsToToolbox(dictionary, "CT", true);               
                bool step13 = domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[2]);              
                if (step_13 && !step13)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step14 - Change the order of the tools by drag and drop them at the desired position.               
                group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(5)"));                               
                dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add(AddNewTool[0], group1);
                dictionary.Add(AddNewTool[1], group1);
                domain.RepositionToolsInConfiguredToolsSection(dictionary, "CT", true);
                String[] Tools = DefaultTools.Split(':');
                Tools[3] = Tools[3] + "," + AddNewTool[2];
                Tools[4] = Tools[4] + "," + AddNewTool[0] + "," + AddNewTool[1];
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                groupsInUse = domain.GetGroupsInToolBoxConfig();
                bool step14_1 = groupsInUse.Count() == 12;
                bool step14_2 = domain.VerifyConfiguredToolsInToolBoxConfig(Tools);
                if (step14_1 && step14_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step15 - Click on "Save" button.
                domain.ClickSaveEditDomain();
                if (BasePage.Driver.FindElements(By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)).Count == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step16 - In Domain Management page,Edit the Domain Page and select the MR modality and verify the toolbox configuration for MR Modality
                //           still have the values from before.(i.e. User copies from CT to MR and then later changes the CT modality, the MR toolbox still 
                //            have the values from before and should not receive the new changes to CT.
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups)));
                Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("MR");
                Thread.Sleep(1000);
                //Verify Configured Tools               
                String[] ConfiguredToolsForMR = DefaultTools.Split(':');
                ConfiguredToolsForMR[0] = ConfiguredToolsForMR[0] + "," + AddNewTool[0];
                ConfiguredToolsForMR[1] = ConfiguredToolsForMR[1] + "," + AddNewTool[1];
                groupsInUse = domain.GetGroupsInToolBoxConfig();
                bool step16_1 = groupsInUse.Count() == 12;
                bool step16_2 = domain.VerifyConfiguredToolsInToolBoxConfig(ConfiguredToolsForMR);
                if (step16_1 && step16_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                domain.CloseDomainManagement();

                //Step17 - Navigate to Studies tab,search and Load a study which contains series of CT Modality and then click on 'Universal' button
                studies = (Studies)login.Navigate("Studies");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.ClearSearchBtn()));
                studies.SearchStudy(patientID: PatientID[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step18 - Select any series viewport and click on right mouse button                
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step19 - Verify the CT Modality toolbox should be updated               
                String[] ConfiguredToolsForMR1= DefaultTools.Split(':');
                ConfiguredToolsForMR1[3] = ConfiguredToolsForMR1[3] + "," + AddNewTool[2];
                ConfiguredToolsForMR1[4] = ConfiguredToolsForMR1[4] + "," + AddNewTool[0] + "," + AddNewTool[1];
                toolsInViewer = viewer.GetGroupsInToolBox();
                bool step19_1 = toolsInViewer.Count() == 12;
                bool step19_2 = viewer.VerifyConfiguredTools(ConfiguredToolsForMR1);
                if (step19_1 && step19_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step20 - Select any tool from the modality toolbox and apply it.
                viewer.SelectViewerTool(BluRingTools.Line_Measurement, isOpenToolsPOPup:false);
                viewer.ApplyTool_LineMeasurement();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step20 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step20)
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

                //Logout Application
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
            finally
            {
                DomainManagement domain = new DomainManagement();
                login.LoginIConnect("Administrator", "Administrator");
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                SelectElement Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("MR");
                Thread.Sleep(1000);
                IWebElement ele = BasePage.Driver.FindElement(By.CssSelector(DomainManagement.btn_revertToDefault));
                if (ele.Enabled)
                    ele.Click();
                Modality.SelectByText("CT");
                Thread.Sleep(1000);
                ele = BasePage.Driver.FindElement(By.CssSelector(DomainManagement.btn_revertToDefault));
                if (ele.Enabled)
                    ele.Click();
                domain.ClickSaveEditDomain();
                login.Logout();
            }
        }

        /// <summary>
        ///  Domain Management Page: Maximum of 5 tools in a stack
        /// </summary>
        public TestCaseResult Test_161514(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            BluRingViewer viewer = new BluRingViewer();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String[] AddNewTool = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "AddNewTools")).Split(',');

                DomainManagement domain = new DomainManagement();

                //Step1 - Login to iCA application as Administrator 
                //Step2 - Domain Management page should be displayed by default
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;
                if (login.IsTabSelected("Domain Management"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step3 Create new:
                // 1.Test Domain
                // 2.role(Regular User) under the Test Domain(so you will have the Test Domain Admin role and the Regular User role)
                // domain admin(Administrator Test Domain)
                // regular user(User1, belonging to Test Domain and with a Regular User
                String TestDomain = "TestDomain_tb_12_" + new Random().Next(1, 10000);
                String Role = "Role_tb_12_" + new Random().Next(1, 10000);
                String DomainAdmin = "DomainAdmin_tb_12_" + new Random().Next(1, 10000);

                domain.CreateDomain(TestDomain, TestDomain, TestDomain, DomainAdmin, null, DomainAdmin, DomainAdmin, DomainAdmin, Role, Role);               
                if (domain.IsDomainExist(TestDomain))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step4 - Log out from Administrator user.
                login.Logout();
                Thread.Sleep(2000);
                BasePage.Driver.SwitchTo().DefaultContent();
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

                //Step5 - Log in as Test Domain Administrator.
                login.LoginIConnect(DomainAdmin, DomainAdmin);
                if (login.IsTabSelected("User Management"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step6 - Navigate to Domain management page.
                login.NavigateToDomainManagementTab();
                if (login.IsTabSelected("Domain Management"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step7 - Verify the default tools should be displayed under Toolbox configuration section..
                BasePage.Driver.SwitchTo().Frame("TabContent");
                var groupsInUse = domain.GetGroupsInToolBoxConfig();
                bool step2_1 = groupsInUse.Count() == 12;
                bool step2_2 = domain.VerifyConfiguredToolsInToolBoxConfig(DefaultTools.Split(':'));
                if (step2_1 && step2_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step8 - Drag some tools from the Available Items section and drop the dragged tools to all 12 cells/slots in the toolbox configuration and verify that all 12 slots are configurable.               
                //Step9 - Place any tool in stacked tool control by drag and dropping the tool from the Available Items into the cell in the toolbox at the desired position.               
                int totalSlots = 12;
                var groups = domain.GetGroupsInToolBoxConfig();
                var dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add(AddNewTool[0], groups.ElementAt(0));
                var step8_1 = domain.AddToolsToToolbox(dictionary);
                var step8_2 = !(domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[0]));
                var step8_3 = domain.GetConfiguredToolsInToolBoxConfig().Contains(AddNewTool[0]);
                bool step8_4 = true;
                while (totalSlots != 1)
                {
                    if (domain.GetToolsInGroupInToolBoxConfig(groups.ElementAt(totalSlots - 1)).Count < 5)
                    {
                        dictionary = new Dictionary<String, IWebElement>();
                        dictionary.Add(AddNewTool[0], groups.ElementAt(totalSlots - 1));
                        domain.RepositionToolsInConfiguredToolsSection(dictionary);
                        if (!domain.GetToolsInGroupInToolBoxConfig(groups.ElementAt(totalSlots - 1)).Contains(AddNewTool[0]))
                        {
                            step8_4 = false;
                            break;
                        }
                        Thread.Sleep(Config.minTimeout);
                    }
                    totalSlots--;
                }
                ExecutedSteps++;

                if (step8_1 && step8_2 && step8_3 && step8_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                domain.ClickSaveEditDomain();      // to avoid dead object error        
                login.Navigate("Studies");                
                login.Navigate("DomainManagement");              

                //Step10 - Repeat step 09 and place the 5 tools in each cell in the toolbox
                //Step11 - Place more than 5 tools in the same cell/column by drag and dropping and verify the tool being dragged should be return to its original place where it was dragged from.                            
                bool step10_1 = true;
                bool step10_2 = true;
                String[] Tools = new String[] { "Cobb Angle", "Draw Rectangle", "Draw ROI", "Flip Vertical", "Pan", "Invert" };
                groups = domain.GetGroupsInToolBoxConfig();
                foreach (IWebElement ele in groups)
                {
                    int numberOfTools = domain.GetToolsInGroupInToolBoxConfig(ele).Count;
                    int temp = 0;
                    dictionary = new Dictionary<String, IWebElement>();
                    // Place 5 tools in a column
                    while (numberOfTools < 5)
                    {
                        dictionary.Add(Tools[temp], ele);
                        numberOfTools++;
                        temp++;
                    }
                    domain.RepositionToolsInConfiguredToolsSection(dictionary);
                    IList<String> availableToolsInGroup = domain.GetToolsInGroupInToolBoxConfig(ele);
                    if (availableToolsInGroup.Count != 5)
                    {
                        step10_1 = false;
                        Logger.Instance.InfoLog("Reposition not done correctly. Number of available tools are wrong");
                        break;
                    }
                    // Place 6th Tool 
                    dictionary = new Dictionary<String, IWebElement>();
                    dictionary.Add(AddNewTool[1], ele);
                    if (domain.AddToolsToToolbox(dictionary) && !(domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[1])))
                    {
                        step10_2 = false;
                        break;
                    }
                }
                ExecutedSteps++;
                if (step10_1 && step10_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step12 - Drag some tools to Available Items section
                var ToolsToBeRemoved = new List<String>();
                ToolsToBeRemoved.Add(AddNewTool[2]);
                ToolsToBeRemoved.Add(AddNewTool[3]);
                domain.RemoveToolsFromConfiguredSection(ToolsToBeRemoved);                
                bool step12_1 = domain.GetConfiguredToolsInToolBoxConfig().Contains(AddNewTool[2]) &&
                                    domain.GetConfiguredToolsInToolBoxConfig().Contains(AddNewTool[3]);
                bool step12_2 = domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[2]) &&
                                domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[3]);
                if (!step12_1 && step12_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step13 - Click on "Save" button.               
                domain.ClickSaveEditDomain();
                bool step13_1 = login.IsTabSelected("Domain Management");
                login.Navigate("Studies");
                bool step13_2 = login.IsTabSelected("Studies");
                login.Navigate("DomainManagement");
                bool step13_3 = domain.GetConfiguredToolsInToolBoxConfig().Contains(AddNewTool[2]) &&
                                   domain.GetConfiguredToolsInToolBoxConfig().Contains(AddNewTool[3]);
                bool step13_4 = domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[2]) &&
                                domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[3]);

                if (step13_1 && step13_2 && !step13_3 && step13_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout Application				
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
        ///  At Domain level settings without inheritance: New domain user configures the toolbox
        /// </summary>
        public TestCaseResult Test_161526(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            BluRingViewer viewer = new BluRingViewer();
            DomainManagement domain = new DomainManagement();
            UserManagement usermanagement = new UserManagement();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;               
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');
                String[] AddNewTool = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "AddNewTools")).Split(',');

                // Precondition
                // 1.Test Domain
                // 2.role(Regular User) under the Test Domain(so you will have the Test Domain Admin role and the Regular User role)
                // domain admin(Administrator Test Domain)
                // regular user(User1, belonging to Test Domain and with a Regular User
                String TestDomain = "TestDomain_tb_15_" + new Random().Next(1, 10000);
                String Role = "Role_tb_15_" + new Random().Next(1, 10000);
                String DomainAdmin = "DomainAdmin_tb_15_" + new Random().Next(1, 10000);
                String User = "Rad_tb_15_" + new Random().Next(1, 10000);

                login.LoginIConnect(adminUserName, adminPassword);
                domain.CreateDomain(TestDomain, TestDomain, TestDomain, DomainAdmin, null, DomainAdmin, DomainAdmin, DomainAdmin, Role, Role);
                login.Navigate("UserManagement");
                usermanagement.CreateUser(User, TestDomain, Role);
                login.Logout();

                //Step1 -Log in as Test Domain Administrator.                
                login.LoginIConnect(DomainAdmin, DomainAdmin);
                if (login.IsTabSelected("User Management"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step2 - Navigate to Domain management page.
                login.NavigateToDomainManagementTab();
                if (login.IsTabSelected("Domain Management"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step3 - Verify the default tools should be displayed on viewer.
                BasePage.Driver.SwitchTo().Frame("TabContent");
                var groupsInUse = domain.GetGroupsInToolBoxConfig();
                bool step3_1 = groupsInUse.Count() == 12;
                bool step3_2 = domain.VerifyConfiguredToolsInToolBoxConfig(DefaultTools.Split(':'));
                if (step3_1 && step3_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step4 - Drag some tools from the Available Items section and drop the dragged tools to any slots in the toolbox configuration.
                var group1 = viewer.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)");
                var group2 = viewer.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(10)");
                var dictionary = new Dictionary<String, IWebElement>();
                AddNewTool = domain.GetAvailableToolsInToolBoxConfig().ToArray();
                dictionary.Add(AddNewTool[0], group1);
                dictionary.Add(AddNewTool[1], group1);
                dictionary.Add(AddNewTool[2], group2);
                bool step4_1 = domain.AddToolsToToolbox(dictionary, addToolAtEnd:true);
                bool step4_2 = domain.GetToolsInGroupInToolBoxConfig(group1).Contains(AddNewTool[0]) &&
                                domain.GetToolsInGroupInToolBoxConfig(group1).Contains(AddNewTool[1]) &&
                                domain.GetToolsInGroupInToolBoxConfig(group2).Contains(AddNewTool[2]);
                bool step4_3 = !domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[0]) &&
                                !domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[1]) &&
                                !domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[2]);

                if (step4_1 && step4_2 && step4_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step5 - Drag some tools to Available Items section
                var ToolsToBeRemoved = new List<String>();
                string[] removeTools = domain.GetConfiguredToolsInToolBoxConfig().ToArray();
                ToolsToBeRemoved.Add(removeTools[3]);
                ToolsToBeRemoved.Add(removeTools[4]);
                ToolsToBeRemoved.Add(removeTools[5]);
                domain.RemoveToolsFromConfiguredSection(ToolsToBeRemoved);              
                var configuredTools = domain.GetConfiguredToolsInToolBoxConfig();
                var availableTools = domain.GetAvailableToolsInToolBoxConfig();
                bool step5_1 = !configuredTools.Contains(removeTools[3]) && !configuredTools.Contains(removeTools[4]) &&
                                !configuredTools.Contains(removeTools[5]);
                bool step5_2 = availableTools.Contains(removeTools[3]) && availableTools.Contains(removeTools[4]) &&
                                availableTools.Contains(removeTools[5]);
                IList<string> ToolsBeforeSaveDomain = basepage.GetToolsInToolBoxConfigByEachColumn();
                if (step5_1 && step5_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                

                //Step6 - Click on "Save" button.
                domain.ClickSaveEditDomain();
                if (login.IsTabSelected("Domain Management"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step7 - Navigate to any tab and come back to Domain Management page and then verify the tools available under the toolbox configuration and Available items.
                login.Navigate("Studies");
                bool step7_1 = login.IsTabSelected("Studies");
                login.Navigate("DomainManagement");
                availableTools = domain.GetAvailableToolsInToolBoxConfig();
                bool step7_2 = availableTools.Contains(removeTools[3]) && availableTools.Contains(removeTools[4]) &&
                                availableTools.Contains(removeTools[5]);

                //String[] Tools = DefaultTools.Split(':');
                //Tools[0] = Tools[0] + "," + AddNewTool[1] + "," + AddNewTool[0];
                //Tools[10] = Tools[10].Replace("," + AddNewTool[5], "") + "," + AddNewTool[2];
                //Tools[1] = Tools[1].Replace("," + AddNewTool[4], "");
                //Tools[2] = "";             
                bool step7_3 = domain.VerifyConfiguredToolsInToolBoxConfig(ToolsBeforeSaveDomain.ToArray());

                if (step7_1 && step7_2 && step7_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step8 - Modify the Toolbox Configuration for floating toolbox and click on "Save" button.
                group1 = viewer.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(3)");
                dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add(AddNewTool[3], group1);
                bool step8_1 = domain.AddToolsToToolbox(dictionary);
                bool step8_2 = domain.GetToolsInGroupInToolBoxConfig(group1).Contains(AddNewTool[3]);
                bool step8_3 = !domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[3]);
                if (step8_1 && step8_2 && step8_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                ToolsBeforeSaveDomain = basepage.GetToolsInToolBoxConfigByEachColumn();
                domain.ClickSaveEditDomain();

                //Step9 - Navigate to Studies tab
                var studies = login.Navigate("Studies");
                if (login.IsTabSelected("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step10 - Search and select a study and then click on "Universal" button
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                BluRingViewer.LaunchBluRingViewer();
                if (viewer.GetStudyPanelCount() == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step11 - Click Right Mouse Button on viewport and verify that the user shall open the toolbox
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step12 - Verify the tools available in the floating toolbox                
                //String[] ToolsList = DefaultTools.Split(':');
                //ToolsList[0] = ToolsList[0] + "," + AddNewTool[0] + "," + AddNewTool[1];
                //ToolsList[9] = AddNewTool[2];
                //ToolsList[1] = ToolsList[2].Replace(AddNewTool[4] + ",", "");
                //Tools[2] = AddNewTool[3];
                bool step12_1 = viewer.GetGroupsInToolBox().Count == 12;
                bool step12_2 = viewer.VerifyConfiguredTools(ToolsBeforeSaveDomain.ToArray());
                if (step12_1 && step12_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step13 - Verify the configured tools appear in the floating toolbox with their corresponding captions and images.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step13 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_toolboxContainer));
                if (step13)
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

                //Step14 - Select any tool from the floating toolbox (e.g. Invert or Interactive WW/WL or Line Measurement ) and verify that the floating toolbox disappears when the user selects any tool from the toolbox.
                viewer.SelectViewerTool(BluRingTools.Line_Measurement, isOpenToolsPOPup:false);
                ExecutedSteps++;

                //Step15 - User selected tool action should be applied to the viewport.
                viewer.ApplyTool_LineMeasurement();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step15 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step15)
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

                //Step16 - Log out as Test Domain Administrator
                viewer.CloseBluRingViewer();
                login.Logout();
                Thread.Sleep(2000);
                BasePage.Driver.SwitchTo().DefaultContent();
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

                //Step17, 18 - Log in as User1 and navigate to studies tab  
                login.LoginIConnect(User, User);
                ExecutedSteps += 2;               

                //Step19 - Search and select a study and then click on "Universal" button
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                BluRingViewer.LaunchBluRingViewer();
                if (viewer.GetStudyPanelCount() == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step20 - Click Right Mouse Button on viewport and verify that the user shall open the toolbox
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step21 - Verify the tools available in the floating toolbox                
                bool step21_1 = viewer.GetGroupsInToolBox().Count == 12;
                bool step21_2 = viewer.VerifyConfiguredTools(ToolsBeforeSaveDomain.ToArray());
                if (step21_1 && step21_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step22 - Verify the configured tools appear in the floating toolbox with their corresponding captions and images.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step22 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_toolboxContainer));
                if (step22)
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

                //Step23 - Select any tool from the floating toolbox (e.g. Invert or Interactive WW/WL) and verify that the floating toolbox disappears when the user selects any tool from the toolbox
                viewer.SelectViewerTool(BluRingTools.Line_Measurement, isOpenToolsPOPup:false);
                ExecutedSteps++;

                //Step24 - User selected tool action should be applied to the viewport.
                viewer.ApplyTool_LineMeasurement();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step24 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step24)
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

                //Logout Application	
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
        ///  Empty spaces in the toolbox
        /// </summary>
        public TestCaseResult Test_161528(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            DomainManagement domain = new DomainManagement();
            BluRingViewer viewer = new BluRingViewer();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String DefaultToolsList = this.DefaultTools;
                String[] DefaultTools = DefaultToolsList.Split(':');
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');

                // Precondition - Create New Domain2                
                String TestDomain = "TestDomain_tb_11_" + new Random().Next(1, 10000);
                String Role = "Role_tb_11_" + new Random().Next(1, 10000);
                String DomainAdmin = "DomainAdmin_tb_11_" + new Random().Next(1, 10000);

                String TestDomain1 = "TestDomain_tb_21_" + new Random().Next(1, 10000);
                String Role1 = "Role_tb_21_" + new Random().Next(1, 10000);
                String DomainAdmin1 = "DomainAdmin_tb_21_" + new Random().Next(1, 10000);

                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("DomainManagement");
                domain.CreateDomain(TestDomain, TestDomain, TestDomain, DomainAdmin, null, DomainAdmin, DomainAdmin, DomainAdmin, Role, Role);
                domain.CreateDomain(TestDomain1, TestDomain1, TestDomain1, DomainAdmin1, null, DomainAdmin1, DomainAdmin1, DomainAdmin1, Role1, Role1);
                login.Logout();

                //Step1 - Login to iCA application as Administrator                
                login.LoginIConnect(adminUserName, adminPassword);                
                if (login.IsTabSelected("Domain Management"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step2 - Domain Management page should be displayed by default and Click on "Edit" button.
                domain.NavigateToDomainManagementTab();
                domain.SearchDomain(TestDomain);
                domain.SelectDomain(TestDomain);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                var groupsInUse = domain.GetGroupsInToolBoxConfig();
                bool step2_1 = groupsInUse.Count() == 12;
                bool step2_2 = domain.VerifyConfiguredToolsInToolBoxConfig(DefaultTools);
                if (step2_1 && step2_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step3 - Leave empty slots/cells in between the columns.
                IWebElement group3 = viewer.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(3)");
                IWebElement group6 = viewer.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(10)");
                IWebElement group10 = viewer.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(10)");
                var dictionary = new Dictionary<String, IWebElement>();
                IList<string> ToolsToAdded = domain.GetToolsInGroupInToolBoxConfig(group3).ToArray()[0].ToString().Split(',');
                foreach (string Column in ToolsToAdded)
                {
                    dictionary.Add(Column, group10);
                }                
                domain.RepositionToolsInConfiguredToolsSection(dictionary, addToolAtEnd:true);
                bool step3_1 = domain.GetToolsInGroupInToolBoxConfig(group10).Contains(ToolsToAdded[0]);
                bool step3_2 = domain.GetToolsInGroupInToolBoxConfig(group3).Count == 0;
                if (step3_1 && step3_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                IList<string> ToolsListAfterEmptySlot = basepage.GetToolsInToolBoxConfigByEachColumn();


                //Step4 - Click on 'Save' button
                domain.ClickSaveEditDomain();
                if (login.IsTabSelected("Domain Management"))
                {
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
                Thread.Sleep(Config.medTimeout);

                //Step5 - Navigate to Studies tab.
                //Step6 - Search and select a study and then click on "Universal" button.
                login.LoginIConnect(DomainAdmin, DomainAdmin);
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;
                if (viewer.GetStudyPanelCount() == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step7 - Click Right Mouse Button on viewport and verify that the user shall open the toolbox
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step8 - Verify the tools available in the floating toolbox
                var toolsInViewer = viewer.GetGroupsInToolBox();
                bool step8_1 = toolsInViewer.Count() == 12;             
                bool step8_2 = viewer.VerifyConfiguredTools(ToolsListAfterEmptySlot.ToArray());
                if (step8_1 && step8_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step9 - Verify the toolbox should display in three columns
                bool step9 = true;
                IWebElement toolBoxComponent = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_viewportToolboxComponent);
                int expectedToolWidth = toolBoxComponent.Size.Width / 3;
                var availableGroups = viewer.GetGroupsInToolBox();
                foreach (IWebElement ele in availableGroups)
                {
                    if (!(ele.Size.Width >= expectedToolWidth - 1 && ele.Size.Width <= expectedToolWidth + 1))
                    {
                        step9 = false;
                        break;
                    }
                }
                if (step9)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step10 - Verify that blank spaces should be shown for the empty cells in the toolbox.
                bool step10 = toolBoxComponent.FindElements(By.CssSelector(BluRingViewer.fillerTool)).Count == 1;
                if (step10)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step11 - Click on 'EXIT' button
                viewer.CloseBluRingViewer();
                if (login.IsTabSelected("Studies"))
                {
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

                //step12 - 	Go to Domain Management page
                login.LoginIConnect(adminUserName, adminPassword);
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(TestDomain);
                domain.SelectDomain(TestDomain);
                domain.ClickEditDomain();
                ExecutedSteps++;

                //Step13 - Leave empty slots/cells at the end
                BasePage.FindElementByCss(DomainManagement.btn_revertToDefault).Click();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                IWebElement group1 = viewer.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(3)");
                IWebElement group2 = viewer.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(8)");
                dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add(BluRingViewer.GetToolName(BluRingTools.Reset) , group1);
                dictionary.Add(BluRingViewer.GetToolName(BluRingTools.Get_Pixel_Value), group1);
                dictionary.Add(BluRingViewer.GetToolName(BluRingTools.Add_Text) , group1);
                dictionary.Add(BluRingViewer.GetToolName(BluRingTools.Free_Draw), group1);
                dictionary.Add(BluRingViewer.GetToolName(BluRingTools.Remove_All_Annotations), group2);
                domain.RepositionToolsInConfiguredToolsSection(dictionary, addToolAtEnd:true);
                bool step13_1 = domain.GetToolsInGroupInToolBoxConfig(viewer.GetElement(BasePage.SelectorType.CssSelector,
                                        BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(10)")).Count == 0;
                bool step13_2 = domain.GetToolsInGroupInToolBoxConfig(viewer.GetElement(BasePage.SelectorType.CssSelector,
                                       BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(11)")).Count == 0;
                bool step13_3 = domain.GetToolsInGroupInToolBoxConfig(viewer.GetElement(BasePage.SelectorType.CssSelector,
                                       BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(12)")).Count == 0;
                if (step13_1 && step13_2 && step13_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                ToolsListAfterEmptySlot = basepage.GetToolsInToolBoxConfigByEachColumn();

                //Step14 - Click on 'Save' button
                domain.ClickSaveEditDomain();
                if (login.IsTabSelected("Domain Management"))
                {
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

                //Step15 - Navigate to Studies tab.
                //Step16 - Search and select a study and then click on "Universal" button.
                login.LoginIConnect(DomainAdmin, DomainAdmin);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;
                if (viewer.GetStudyPanelCount() == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step17 - Click Right Mouse Button on viewport and verify that the user shall open the toolbox
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step18 - Verify the tools available in the floating toolbox
                toolsInViewer = viewer.GetGroupsInToolBox();
                bool step18_1 = toolsInViewer.Count == 9; 
                bool step18_2 = viewer.VerifyConfiguredTools(ToolsListAfterEmptySlot.ToArray());
                if (step18_1 && step18_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step19 - Verify the toolbox should display in three columns
                bool step19 = true;
                toolBoxComponent = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_viewportToolboxComponent);
                expectedToolWidth = toolBoxComponent.Size.Width / 3;
                availableGroups = viewer.GetGroupsInToolBox();
                foreach (IWebElement ele in availableGroups)
                {
                    if (!(ele.Size.Width >= expectedToolWidth - 1 && ele.Size.Width <= expectedToolWidth + 1))
                    {
                        step19 = false;
                        break;
                    }
                }
                if (step19)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step20 - Verify that extra empty rows should not be displayed after the last tool
                bool step20_1 = viewer.GetGroupsInToolBox().Count == 9;
                bool step20_2 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.fillerTool)).Count == 0;
                if (step20_1 && step20_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseBluRingViewer();
                login.Logout(); 

                //step21 - Go to Edit Role Management page, In toolbox configurations, Drag any tools from the Available Items and drop the dragged tool into any cells in the toolbox and leave empty slots/cells in between the columns.
                login.LoginIConnect(adminUserName, adminPassword);               
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectDomainfromDropDown(TestDomain1);
                rolemanagement.SearchRole(Role1);
                rolemanagement.SelectRole(Role1);
                rolemanagement.ClickEditRole();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                group3 = viewer.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(3)");
                group6 = viewer.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(10)");
                dictionary = new Dictionary<String, IWebElement>();
                IList<string> ToolsToAddedInRole = domain.GetToolsInGroupInToolBoxConfig(group3).ToArray()[0].ToString().Split(',');              
                foreach (string Column in ToolsToAddedInRole)
                {
                    dictionary.Add(Column, group6);
                }               
                domain.RepositionToolsInConfiguredToolsSection(dictionary, addToolAtEnd: true);
                bool step21_1 = domain.GetToolsInGroupInToolBoxConfig(group6).Contains(ToolsToAddedInRole[0]);
                bool step21_2 = domain.GetToolsInGroupInToolBoxConfig(group3).Count == 0;
                rolemanagement.ClickSaveEditRole();
                login.Logout();
                login.LoginIConnect(DomainAdmin1, DomainAdmin1);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.OpenViewerToolsPOPUp();
                toolBoxComponent = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_viewportToolboxComponent);
                bool step21_3 = toolBoxComponent.FindElements(By.CssSelector(BluRingViewer.fillerTool)).Count == 1;
                if (step21_1 && step21_2 && step21_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseBluRingViewer();
                login.Logout();

                //step22 - In Edit Role Management page, Drag any tools from the Available Items and drop the dragged tool into the cells (10, 11, 12) in the toolbox and leave 1,2 & 3 rows as empty (from 1 to 9 slots/cells) and verify that empty rows should be displayed.
                login.LoginIConnect(adminUserName, adminPassword);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectDomainfromDropDown(TestDomain1);
                rolemanagement.SearchRole(Role1);
                rolemanagement.SelectRole(Role1);
                rolemanagement.ClickEditRole();               
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                BasePage.FindElementByCss(DomainManagement.btn_revertToDefault).Click();
                dictionary = new Dictionary<String, IWebElement>();
                var ToolsToBeRemoved = new List<String>();
                for(int i = 1; i <= 6; i++)
                {
                    IWebElement group = viewer.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(" + i +")");
                    IList<String> availableTools = domain.GetToolsInGroupInToolBoxConfig(group);
                    foreach (string Column in availableTools)
                    {
                        ToolsToBeRemoved.Add(Column);
                    }
                }
                domain.RemoveToolsFromConfiguredSection(ToolsToBeRemoved);
                rolemanagement.ClickSaveEditRole();
                login.Logout();
                login.LoginIConnect(DomainAdmin1, DomainAdmin1);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.OpenViewerToolsPOPUp();
                toolsInViewer = viewer.GetGroupsInToolBox();
                bool step22_1 = toolsInViewer.Count == 12;
                toolBoxComponent = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_viewportToolboxComponent);
                bool step22_2 = toolBoxComponent.FindElements(By.CssSelector(BluRingViewer.fillerTool)).Count == 6;
                if (step22_1 && step22_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout Application
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
        ///  At Domain level: Configure tools to the specific modality(CT) toolbox
        /// </summary>
        public TestCaseResult Test_161531(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                DomainManagement domain = new DomainManagement();
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');
                String[] AddNewTool = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "AddNewTools")).Split(',');

                //Step 1 - Login as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                if (login.IsTabPresent("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step2 - Navigate to domain Management ->Edit Super Admin Domain page -> Toolbox Configuration section, select CT from modality dropdown
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                SelectElement Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("CT");
                ExecutedSteps++;

                //Step3 -  Drag some tools, for ex: Magnifier,Zoom,Rotate ClockWise, Rotate Counterclockwise to the toolbox configuration.Then Click on Save button
                IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                IWebElement group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)"));
                var dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add(AddNewTool[0], group1);
                dictionary.Add(AddNewTool[1], group2);
                bool step3_1 = domain.AddToolsToToolbox(dictionary, "CT", true);
                bool step3_2 = domain.GetConfiguredToolsInToolBoxConfig().Contains(AddNewTool[0]) && domain.GetConfiguredToolsInToolBoxConfig().Contains(AddNewTool[1]);
                bool step3_3 = !domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[0]);
                bool step3_4 = !domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[1]);
                domain.ClickSaveEditDomain();
                if (step3_1 && step3_2 && step3_3 && step3_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step4 - Navigate to Studies tab,search and Load a study which contains series of CT Modality and then click on 'Universal' button
                var studies = (Studies)login.Navigate("Studies");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.ClearSearchBtn()));
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step5 - Select any series viewport and click Right Mouse Button on viewport and verify that the user shall open the toolbox
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step6 - Verify the configured tools appear in the floating toolbox for the CT Modality as specified in Domain Management page.               
                String[] Tools = DefaultTools.Split(':');
                Tools[0] = Tools[0] + "," + AddNewTool[0];
                Tools[1] = Tools[1] + "," + AddNewTool[1];
                var toolsInViewer = viewer.GetGroupsInToolBox();
                bool step6_1 = toolsInViewer.Count() == 12;
                bool step6_2 = viewer.VerifyConfiguredTools(Tools);
                if (step6_1 && step6_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step7 - Select any tool from the toolbox and apply it.
                viewer.SelectViewerTool(BluRingTools.Line_Measurement, isOpenToolsPOPup:false);
                viewer.ApplyTool_LineMeasurement();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step7 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step7)
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
                viewer.CloseBluRingViewer();

                //Step8 - Edit the Domain Management Page for administrator account.
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                ExecutedSteps++;

                //Step9 - Change the order of the tools by drag and drop them at the desired position.				
                group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(5)"));
                group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(10)"));
                dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add(AddNewTool[0], group1);
                dictionary.Add(AddNewTool[1], group2);
                domain.RepositionToolsInConfiguredToolsSection(dictionary, "CT", true);
                Tools = DefaultTools.Split(':');
                Tools[4] = Tools[4] + "," + AddNewTool[0];
                Tools[9] = Tools[9] + "," + AddNewTool[1];
                var groupsInUse = domain.GetGroupsInToolBoxConfig();
                bool step9_1 = groupsInUse.Count() == 12;
                bool step9_2 = domain.VerifyConfiguredToolsInToolBoxConfig(Tools);
                if (step9_1 && step9_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step10 - Remove a tool from the New list, by selecting it then drag and drop it to the Available Items list.				
                var ToolsToBeRemoved = new List<String>();
                ToolsToBeRemoved.Add(AddNewTool[0]);
                domain.RemoveToolsFromConfiguredSection(ToolsToBeRemoved, "CT");
                bool step10_1 = !domain.GetConfiguredToolsInToolBoxConfig().Contains(AddNewTool[0]);
                bool step10_2 = domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[0]);
                if (step10_1 && step10_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step11 - Add back the removed tool to the New list.				
                group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(4)"));
                dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add(AddNewTool[0], group1);
                bool step11_1 = domain.AddToolsToToolbox(dictionary, "CT", true);
                bool step11_2 = domain.GetToolsInGroupInToolBoxConfig(group1).Contains(AddNewTool[0]);
                bool step11_3 = !domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[0]);
                if (step11_1 && step11_2 && step11_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                domain.ClickSaveEditDomain();

                //Step12 - Navigate to Studies tab,search and Load a study which contains series of CT Modality and then click on 'Universal' button
                studies = (Studies)login.Navigate("Studies");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.ClearSearchBtn()));
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Accession", Accession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step13 - Select any series viewport and click on right mouse button
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                Tools = DefaultTools.Split(':');
                Tools[3] = Tools[3] + "," + AddNewTool[0];
                Tools[9] = Tools[9] + "," + AddNewTool[1];
                toolsInViewer = viewer.GetGroupsInToolBox();
                bool step14_1 = toolsInViewer.Count() == 12;
                bool step14_2 = viewer.VerifyConfiguredTools(Tools);
                if (step14_1 && step14_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step15 - Select any tool from the toolbox and apply it.
                viewer.SelectViewerTool(BluRingTools.Line_Measurement, isOpenToolsPOPup:false);
                viewer.ApplyTool_LineMeasurement();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step15 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step15)
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

                //Logout Application
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
            finally
            {
                DomainManagement domain = new DomainManagement();
                login.LoginIConnect("Administrator", "Administrator");
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                SelectElement Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("CT");
                Thread.Sleep(1000);
                IWebElement revertButton = BasePage.Driver.FindElement(By.CssSelector(DomainManagement.btn_revertToDefault));
                if (revertButton.Enabled)
                    revertButton.Click();
                domain.ClickSaveEditDomain();
                login.Logout();
            }
        }

        /// <summary>
        ///  At Domain level: Configure tools to the specific modality(MG) toolbox
        /// </summary>
        public TestCaseResult Test_161532(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            BluRingViewer viewer = new BluRingViewer();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                DomainManagement domain = new DomainManagement();
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");                             
                String[] Accession = AccessionList.Split(':');
                String[] AddNewTool = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "AddNewTools")).Split(',');

                //Precondition
                // 1.Test Domain
                // 2.role(Regular User) under the Test Domain(so you will have the Test Domain Admin role and the Regular User role)
                // domain admin(Administrator Test Domain)
                // regular user(User1, belonging to Test Domain and with a Regular User
                String TestDomain = "TestDomain_tb_15_" + new Random().Next(1, 10000);
                String Role = "Role_tb_15_" + new Random().Next(1, 10000);
                String DomainAdmin = "DomainAdmin_tb_15_" + new Random().Next(1, 10000);
                String User = "Rad_tb_15_" + new Random().Next(1, 10000);

                login.LoginIConnect(adminUserName, adminPassword);
                domain.CreateDomain(TestDomain, TestDomain, TestDomain, DomainAdmin, null, DomainAdmin, DomainAdmin, DomainAdmin, Role, Role);
                login.Logout();

                //Step 1 - Log in as Test Domain Administrator. 
                login.LoginIConnect(DomainAdmin, DomainAdmin);
                if (login.IsTabPresent("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step2 - Navigate to domain management page -> Toolbox Configuration section, Select MG from modality dropdown
                login.NavigateToDomainManagementTab();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                BasePage.Driver.SwitchTo().Frame("TabContent");
                SelectElement Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("MG");
                var groupsInUse = domain.GetGroupsInToolBoxConfig();
                bool step2_1 = groupsInUse.Count() == 12;
                bool step2_2 = domain.VerifyConfiguredToolsInToolBoxConfig(DefaultTools.Split(':'));
                if (step2_1 && step2_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step3 -  Drag some tools, for ex:Invert,WindowLevel ,Pan to the toolbox configuration.
                //Step4 - Place any tool in stacked tool control by drag and dropping the tool from the Available Items into the cell in the toolbox at the desired position.Then Click on Save button               
                IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                IWebElement group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)"));
                var dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add(AddNewTool[0], group1);
                dictionary.Add(AddNewTool[1], group2);
                bool step4_1 = domain.AddToolsToToolbox(dictionary, "MG", true);
                bool step4_2 = domain.GetToolsInGroupInToolBoxConfig(group1).Contains(AddNewTool[0]) && domain.GetToolsInGroupInToolBoxConfig(group2).Contains(AddNewTool[1]);
                bool step4_3 = !domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[0]);
                bool step4_4 = !domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[1]);
                if (step4_1 && step4_2 && step4_3 && step4_4)
                {
                    ExecutedSteps++;
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    ExecutedSteps++;
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                domain.ClickSaveEditDomain();

                //Step5 - Navigate to Studies tab,search and Load a study which contains series of MG Modality and then click on 'Universal' button
                var studies = (Studies)login.Navigate("Studies");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.ClearSearchBtn()));
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step6 - Select any series viewport which has MG series and click on right mouse button
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step7 - Verify the configured tools appear in the floating toolbox for MG Modality as specified in Domain Management page.                                
                String[] Tools = DefaultTools.Split(':');
                Tools[0] = Tools[0] + "," + AddNewTool[0];
                Tools[1] = Tools[1] + "," + AddNewTool[1];
                var toolsInViewer = viewer.GetGroupsInToolBox();
                bool step7_1 = toolsInViewer.Count() == 12;
                bool step7_2 = viewer.VerifyConfiguredTools(Tools);
                if (step7_1 && step7_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step8 - Select any tool(EX: Invert) from the floating toolbox and apply it.
                viewer.SelectInnerViewerTool(BluRingTools.Invert, BluRingTools.Window_Level, isOpenToolsPOPup:false);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step8 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step8)
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
                viewer.CloseBluRingViewer();

                //Step9 - Edit the Domain Management Page for new domain account.
                login.NavigateToDomainManagementTab();
                if (login.IsTabSelected("Domain Management"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step10 - Change the order of the tools by drag and drop them at the desired position.
                BasePage.Driver.SwitchTo().Frame("TabContent");
                group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(5)"));
                group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(10)"));
                dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add(AddNewTool[0], group1);
                dictionary.Add(AddNewTool[1], group2);
                domain.RepositionToolsInConfiguredToolsSection(dictionary, "MG", true);
            
                Tools = DefaultTools.Split(':');
                Tools[4] = Tools[4] + "," + AddNewTool[0];
                Tools[9] = Tools[9] + "," + AddNewTool[1];
                groupsInUse = domain.GetGroupsInToolBoxConfig();
                bool step10_1 = groupsInUse.Count() == 12;
                bool step10_2 = domain.VerifyConfiguredToolsInToolBoxConfig(Tools);
                if (step10_1 && step10_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step11 - Add a tool(Add Text) to the New list, by selecting it then drag and drop it to the toolbox configuration item.			
                dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add(AddNewTool[2], group1);
                
                bool step11_1 = domain.AddToolsToToolbox(dictionary, "MG", true);
                bool step11_2 = domain.GetToolsInGroupInToolBoxConfig(group1).Contains(AddNewTool[2]);
                bool step11_3 = !domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[2]);
                if (step11_1 && step11_2 && step11_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                domain.ClickSaveEditDomain();

                //Step12 - Navigate to Studies tab,search and Load a study which contains MG Modality series and then click on 'Universal' button				
                studies = (Studies)login.Navigate("Studies");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.ClearSearchBtn()));
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step13 - Select any series viewport and click on right mouse button
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step14 - Verify the Modality toolbox updates to reflect the configuration for the MG modality as specified in Domain Management page                             
                Tools[4] = Tools[4] + "," + AddNewTool[2];
                toolsInViewer = viewer.GetGroupsInToolBox();
                bool step14_1 = toolsInViewer.Count() == 12;
                bool step14_2 = viewer.VerifyConfiguredTools(Tools);
                if (step14_1 && step14_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step15 - Select any tool from the floating toolbox and apply it.
                viewer.SelectViewerTool(BluRingTools.Line_Measurement, isOpenToolsPOPup:false);
                viewer.ApplyTool_LineMeasurement();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step15 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step15)
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

                //Step16 - Close study by clicking on the "X" button
                viewer.CloseBluRingViewer();
                if (login.IsTabSelected("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step17 - Search and select any modality(e.g. CR) study except MG modality study and click on 'Universal" button               
                studies.SearchStudy(AccessionNo: Accession[1], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step18 - In active series viewport,click on right mouse button
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step19 - Verify the configured tools should not appear in the floating toolbox for MG Modality as specified in Domain Management page
                toolsInViewer = viewer.GetGroupsInToolBox();
                bool step19_1 = toolsInViewer.Count() == 12;
                bool step19_2 = viewer.VerifyConfiguredTools(DefaultTools.Split(':'));
                if (step19_1 && step19_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout Application
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
        ///  Tool box shall available for PR and KO series
        /// </summary>
        public TestCaseResult Test_161527(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;


            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                DomainManagement domain = new DomainManagement();

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                //String[] Accession = AccessionList.Split(':');
                String TestDomain = "Domain_249_" + new Random().Next(1, 10000);
                String Role = "Role_249_" + new Random().Next(1, 10000);
                String DomainAdmin = "DomainAdmin_249_" + new Random().Next(1, 10000);


                //PreCondition:
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                login.NavigateToDomainManagementTab();
                domain.CreateDomain(TestDomain, TestDomain, TestDomain, DomainAdmin, null, DomainAdmin, DomainAdmin, DomainAdmin, Role, Role);
                domain.SearchDomain(TestDomain);
                domain.SelectDomain(TestDomain);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                SelectElement Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));

                IWebElement column_1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                IWebElement column_2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)"));
                IList<string> AlltoolsListInConfig = domainmanagement.GetAllToolsInToolBoxConfig();
                IList<string> AllAvailableTools = domainmanagement.GetAvailableToolsInToolBoxConfig();
                var dictionaryAddTool = new Dictionary<String, IWebElement>();
                dictionaryAddTool.Add(AllAvailableTools[1], column_1);
                dictionaryAddTool.Add(AllAvailableTools[2], column_2);
                domain.AddToolsToToolbox(dictionaryAddTool, "CT", addToolAtEnd: true);

                IList<string> ToolsInDomain = new List<string>();
                foreach (IWebElement columnTools in domainmanagement.GetGroupsInToolBoxConfig())
                {
                    IList<string> toolsInEachColumn = domainmanagement.GetToolsInGroupInToolBoxConfig(columnTools);
                    ToolsInDomain.Add(String.Join(",", toolsInEachColumn));
                }

                domain.ClickSaveEditDomain();
                login.Logout();

                //Step 1 - Login to application with any privileged user
                login.DriverGoTo(login.url);
                login.LoginIConnect(DomainAdmin, DomainAdmin);
                if (login.IsTabPresent("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step2 - Load the study in viewer with PR and KO
                var studies = (Studies)login.Navigate("Studies");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.ClearSearchBtn()));
                studies.SearchStudy(AccessionNo: AccessionList, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", AccessionList);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step3 - Select PR series view port and click Right Mouse Button on viewport and verify that the user shall open the toolbox
                viewer.SetViewPort(1, 1);
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step4 - Verify the tools available in the toolbox
                //bool step4_1 = viewer.VerifyConfiguredTools(ToolsInDomain.Where(str => str != "").ToArray(), 1,2);
                Thread.Sleep(5000);
                bool step4_1 = viewer.VerifyConfiguredTools(ToolsInDomain.ToArray(), 1, 2);
                if (step4_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step5 - Apply tools(EX: Measurements,pan,zoom,WL)
                viewer.SelectViewerTool(BluRingTools.Line_Measurement, 1, 2 , isOpenToolsPOPup: false);
                viewer.ApplyTool_LineMeasurement();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                var viewport = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                bool step5 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step5)
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

                //Step6 - Select KO series view port and click Right Mouse Button on viewport and verify that the user shall open the toolbox
                viewer.SetViewPort(0, 1);
                //viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport).Click();
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step7 - Verify the tools available in the toolbox launched for KO series
                //    bool step6_1 = viewer.VerifyConfiguredTools(ToolsInDomain.Where(str => str != "").ToArray(), 1,1);
                bool step6_1 = viewer.VerifyConfiguredTools(ToolsInDomain.ToArray(), 1, 1);
                if (step6_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step8 - 	Apply tools(EX: Measurements,pan,zoom,WL)
                viewer.SelectViewerTool(BluRingTools.Line_Measurement, isOpenToolsPOPup: false);
                viewer.ApplyTool_LineMeasurement();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step8 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step8)
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

                //Step9 - Select different modality series (Ex: MR or CT) and click Right Mouse Button on viewport and verify that the user shall open the toolbox			             
                viewer.SetViewPort(2, 1);              
                viewer.OpenViewerToolsPOPUp();
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(BluRingViewer.div_toolboxContainer)));
                var toolsInViewer = viewer.GetGroupsInToolBox(3,1);
                //bool step9 = viewer.IsElementVisibleInUI(By.CssSelector(viewer.GetToolBoxCss(1)));
                if(toolsInViewer!=null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step10 - Verify the tools available in the toolbox launched for configured modality series
                //bool step10_1 = viewer.VerifyConfiguredTools(ToolsInDomain.Where(str => str != "").ToArray(), 1,3);
                bool step10_1 = viewer.VerifyConfiguredTools(ToolsInDomain.ToArray(), 1, 3);
                if (step10_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step11 - 	Apply tools(EX: Measurements,pan,zoom,WL)
                viewer.SelectViewerTool(BluRingTools.Line_Measurement , isOpenToolsPOPup: false);
                viewer.ApplyTool_LineMeasurement();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step11 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step11)
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


                //Logout Application
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
		/// At Domain level settings without inheritance: Superadmin user configures the toolbox for new domain user
		/// </summary>
		public TestCaseResult Test_161525(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                DomainManagement domain = new DomainManagement();
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');
                String[] DefaultTools = this.DefaultTools.Split(':');
                String TestDomain = "Domain_729_" + new Random().Next(1, 10000);
                String Role = "Role_729_" + new Random().Next(1, 10000);
                String DomainAdmin = "DomainAdmin_729_" + new Random().Next(1, 10000);

                //Step 1 - Login as Administrator 
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                if (login.IsTabPresent("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step2 - Domain Management page should be displayed by default
                if (login.IsTabSelected("Domain Management"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step3 - 	
                //Create new: (if not yet created new domain user)
                //1.Test Domain
                //2.role(Regular User) under the Test Domain(so
                //you will have the Test Domain Admin role and the Regular User role)
                //domain admin(Administrator Test Domain)
                domain.CreateDomain(TestDomain, TestDomain, TestDomain, DomainAdmin, null, DomainAdmin, DomainAdmin, DomainAdmin, Role, Role);
                if (domain.SearchDomain(TestDomain))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step4 - Select newly created Test Domain and click on Edit button.
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(TestDomain);
                domain.SelectDomain(TestDomain);
                domain.ClickEditDomain();
                ExecutedSteps++;


                //Step5 -Verify the default tools should be displayed on viewer.               
                var groupsInUse = domain.GetGroupsInToolBoxConfig();
                bool step2_1 = groupsInUse.Count() == 12;
                bool step2_2 = domain.VerifyConfiguredToolsInToolBoxConfig(DefaultTools);
                if (step2_1 && step2_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step6 - Drag some tools from the Available Items section and drop the dragged tools to all 12 cells/slots in the toolbox configuration and verify that all 12 slots are configurable.                
                //Step7 - Place any tool in stacked tool control by drag and dropping the tool from the Available Items into the cell in the toolbox at the desired position.
                int totalSlots = 12;
                var groups = domain.GetGroupsInToolBoxConfig();
                var dictionary = new Dictionary<String, IWebElement>();
                string Free_tool = BluRingViewer.GetToolName(BluRingTools.Image_Scope);
                dictionary.Add(Free_tool, groups.ElementAt(0));
                var step7_1 = domain.AddToolsToToolbox(dictionary, addToolAtEnd:true);
                var step7_2 = !(domain.GetAvailableToolsInToolBoxConfig().Contains(Free_tool));
                var step7_3 = domain.GetConfiguredToolsInToolBoxConfig().Contains(Free_tool);
                var step7_4 = true;
                while (totalSlots != 1)
                {
                    if (domain.GetToolsInGroupInToolBoxConfig(groups.ElementAt(totalSlots - 1)).Count < 5 )
                    {
                        dictionary = new Dictionary<String, IWebElement>();
                        dictionary.Add(Free_tool, groups.ElementAt(totalSlots - 1));
                        domain.RepositionToolsInConfiguredToolsSection(dictionary);
                        if (!domain.GetToolsInGroupInToolBoxConfig(groups.ElementAt(totalSlots - 1)).Contains(Free_tool))
                        {
                            step7_4 = false;
                            break;
                        }
                        Thread.Sleep(Config.minTimeout);
                    }
                 totalSlots--;
                }
                ExecutedSteps++;
                if (step7_1 && step7_2 && step7_3 && step7_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step8 - Verify the user able to place any tool by drag and drop into the cell in the toolbox at the desired position.   
                dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add(Free_tool, groups.ElementAt(0));
                domain.RepositionToolsInConfiguredToolsSection(dictionary);
                var step8_1 = domain.GetToolsInGroupInToolBoxConfig(groups.ElementAt(0)).Contains(Free_tool);
                if (step8_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step9 - Repeat step 7 and place the 5 tools in each cell in the toolbox.
                if (domain.AddToolsToEachColumnInGroupToolBox(5))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step10 - Drag some tools to Available Items section.
                BasePage.FindElementByCss(DomainManagement.btn_revertToDefault).Click();
                var ToolsToBeRemoved = new List<String>();
                string Cobb_Angle = BluRingViewer.GetToolName(BluRingTools.Cobb_Angle);
                string Pan = BluRingViewer.GetToolName(BluRingTools.Pan);
                ToolsToBeRemoved.Add(Cobb_Angle);
                ToolsToBeRemoved.Add(Pan);
                domain.RemoveToolsFromConfiguredSection(ToolsToBeRemoved);
                bool step10_1 = !domain.GetConfiguredToolsInToolBoxConfig().Contains(Cobb_Angle);
                bool step10_2 = !domain.GetConfiguredToolsInToolBoxConfig().Contains(Pan);
                if (step10_1 && step10_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                IList<string> ToolsBeforeSaveDomain = basepage.GetToolsInToolBoxConfigByEachColumn();
                domain.ClickSaveEditDomain();

                //Step11 - Log out from Administrator user.		
                login.Logout();
                login.DriverGoTo(login.url);
                if (login.LoginBtn().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step12 -	Log in as Test Domain Administrator.
                //login.DriverGoTo(login.url);
                login.LoginIConnect(DomainAdmin, DomainAdmin);
                ExecutedSteps++;

                //Step13 - Go to Domain Management page and verify the tools available under the toolbox configuration and Available Items.
                login.NavigateToDomainManagementTab();                
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
                bool step13_1 = domain.VerifyConfiguredToolsInToolBoxConfig(ToolsBeforeSaveDomain.ToArray());
                if (step13_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step14 - Navigate to Studies tab.
                var studies = (Studies)login.Navigate("Studies");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.ClearSearchBtn()));
                ExecutedSteps++;

                //Step15 - Search and select a study and then click on "View Exam" button.
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;


                //Step16 - Click Right Mouse Button on viewport and verify that the user shall open the toolbox
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step17 - Verify the tools available in the floating toolbox                
                bool step17_1 = viewer.VerifyConfiguredTools(ToolsBeforeSaveDomain.ToArray());
                if (step17_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout Application
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
        /// Domain Management page: Tool tips for all default tools and modality tools
        /// </summary>
        public TestCaseResult Test_161530(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            BluRingViewer viewer = new BluRingViewer();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                DomainManagement domain = new DomainManagement();
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String[] defaultToolsList = DefaultTools.Split(':');
                String[] defaultAvailableList = DefaultAvailableTools.Split(',');
                String[] AddNewTool = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "AddNewTools")).Split(',');
                IList<String> defaultAvailableTools = new List<String>();
                foreach(String str in defaultAvailableList)
                {
                    defaultAvailableTools.Add(str);
                }
                String tempString = DefaultTools.Replace(":", ",");
                String[] tooltipsOfConfiguredTools = tempString.Split(',');
                String[] tooltipsOfAvailableTools = DefaultAvailableTools.Split(',');

                //Step 1 - Log in as Test Domain Administrator. 
                login.LoginIConnect(adminUserName, adminPassword);
                if (login.IsTabSelected("Domain Management"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step2 - Navigate to Domain Management and Edit superadmin Domain then go to Toolbox Configuration section
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                ExecutedSteps++;

                //Step3 - By default, Default option should be displayed in the Modality Dropdown
                SelectElement Modality = new SelectElement(basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.select_toolBoxConfiguration_ModalityDropdown));
                if (Modality.SelectedOption.Text == "default")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step4 - Make sure the default tools are displayed in Configuration section            
                var groupsInUse = domain.GetGroupsInToolBoxConfig();
                bool step4_1 = groupsInUse.Count() == 12;
                bool step4_2 = domain.VerifyConfiguredToolsInToolBoxConfig(DefaultTools.Split(':'));
                if (step4_1 && step4_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step5 - Verify "All In One,Zoom,Free Draw,Reset" are displayed in AVailable section
                var availableTools = viewer.GetAvailableToolsInToolBoxConfig();
                bool step5_1 = availableTools.Count == defaultAvailableTools.Count;                    
                bool step5_2 = defaultAvailableTools.SequenceEqual(availableTools);                             
                if (step5_1 && step5_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step6 - Mouse hover on window level tool present in config toolbox section and verify the Tool tip of 'Window Level' tool
                var elements = BasePage.Driver.FindElements(By.CssSelector(BasePage.div_toolBoxConfiguration_ToolsInUse));
                bool step6 = elements.ElementAt(0).GetAttribute("title").Equals(tooltipsOfConfiguredTools[0]);
                if (step6)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step7 - Mouse hover on Image Scope tool present in Available box section and verify Tool tip for 'Image Scope' tool
                elements = BasePage.Driver.FindElements(By.CssSelector(BasePage.ul_toolBoxConfiguration_AvailableTools));
                bool step7 = elements.ElementAt(2).GetAttribute("title").Equals(tooltipsOfAvailableTools[2]);
                if (step7)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step8 - Mouse hover on any tools available in Toolbox section or Available Items and verify that the Tool tip should be displayed
                elements = BasePage.Driver.FindElements(By.CssSelector(BasePage.div_toolBoxConfiguration_ToolsInUse));             
                int i = 0;
                bool step8_1 = true;
                bool step8_2 = true;
                foreach (IWebElement ele in elements)
                {
                    if (!ele.GetAttribute("title").Equals(tooltipsOfConfiguredTools[i]))
                    {
                        step8_1 = false;
                        Logger.Instance.InfoLog("Tooltip mismatched. Expected: " + tooltipsOfConfiguredTools[i] + "Actual : " + ele.GetAttribute("title"));
                        break;
                    }
                    i++;
                }

                i = 0;
                elements = BasePage.Driver.FindElements(By.CssSelector(BasePage.ul_toolBoxConfiguration_AvailableTools));
                foreach (IWebElement ele in elements)
                {
                    if (!ele.GetAttribute("title").Equals(tooltipsOfAvailableTools[i]))
                    {
                        step8_2 = false;
                        Logger.Instance.InfoLog("Tooltip mismatched. Expected: " + tooltipsOfAvailableTools[i] + "Actual : " + ele.GetAttribute("title"));
                        break;
                    }
                    i++;
                }

                if (step8_1 && step8_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step9 - Select any modality(e.g. CT) from the Modality drop down   
                Modality.SelectByText("CT");
                Thread.Sleep(1000);
                if (Modality.SelectedOption.Text == "CT")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step10 - Make sure the default tools are displayed in Configuration section
                groupsInUse = domain.GetGroupsInToolBoxConfig();
                bool step10_1 = groupsInUse.Count() == 12;
                bool step10_2 = domain.VerifyConfiguredToolsInToolBoxConfig(DefaultTools.Split(':'));
                if (step10_1 && step10_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step11 - Verify "All In One,Zoom,Free Draw,Reset" are displayed in AVailable section
                availableTools = viewer.GetAvailableToolsInToolBoxConfig();
                bool step11_1 = availableTools.Count == defaultAvailableTools.Count;
                bool step11_2 = defaultAvailableTools.SequenceEqual(availableTools);
                if (step11_1 && step11_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step12 - Mouse hover on 'Calibration' tool present in config toolbox section and verify Tool tip of calibration tool
                elements = BasePage.Driver.FindElements(By.CssSelector(BasePage.div_toolBoxConfiguration_ToolsInUse));
                bool step12 = elements.ElementAt(7).GetAttribute("title").Equals(tooltipsOfConfiguredTools[7]);
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

                //Step13 -Mouse hover on "Save Series" tool present in the Available box section and verify Tool tip of Save Series tool
                elements = BasePage.Driver.FindElements(By.CssSelector(BasePage.ul_toolBoxConfiguration_AvailableTools));
                bool step13 = elements.ElementAt(1).GetAttribute("title").Equals(tooltipsOfAvailableTools[1]);
                if (step13)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step14 - Verify Tool tips for all available tools in both Toolbox section/available box
                elements = BasePage.Driver.FindElements(By.CssSelector(BasePage.div_toolBoxConfiguration_ToolsInUse));
                i = 0;
                bool step14_1 = true;
                bool step14_2 = true;
                foreach (IWebElement ele in elements)
                {
                    if (!ele.GetAttribute("title").Equals(tooltipsOfConfiguredTools[i]))
                    {
                        step14_1 = false;
                        Logger.Instance.InfoLog("Tooltip mismatched. Expected: " + tooltipsOfConfiguredTools[i] + "Actual : " + ele.GetAttribute("title"));
                        break;
                    }
                    i++;
                }

                i = 0;
                elements = BasePage.Driver.FindElements(By.CssSelector(BasePage.ul_toolBoxConfiguration_AvailableTools));
                foreach (IWebElement ele in elements)
                {
                    if (!ele.GetAttribute("title").Equals(tooltipsOfAvailableTools[i]))
                    {
                        step14_2 = false;
                        Logger.Instance.InfoLog("Tooltip mismatched. Expected: " + tooltipsOfAvailableTools[i] + "Actual : " + ele.GetAttribute("title"));
                        break;
                    }
                    i++;
                }

                if (step14_1 && step14_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step15 - Verify tool tips for some modalities(e.g. US/MG/CT)
                //Select US Modality and verify tooltips
                Modality.SelectByText("US");
                Thread.Sleep(1000);
                i = 0;
                bool step15_1 = true;
                bool step15_2 = true;
                elements = BasePage.Driver.FindElements(By.CssSelector(BasePage.div_toolBoxConfiguration_ToolsInUse));
                foreach (IWebElement ele in elements)
                {
                    if (!ele.GetAttribute("title").Equals(tooltipsOfConfiguredTools[i]))
                    {
                        step15_1 = false;
                        Logger.Instance.InfoLog("Tooltip mismatched for US. Expected: " + tooltipsOfConfiguredTools[i] + "Actual : " + ele.GetAttribute("title"));
                        break;
                    }
                    i++;
                }

                i = 0;
                elements = BasePage.Driver.FindElements(By.CssSelector(BasePage.ul_toolBoxConfiguration_AvailableTools));
                foreach (IWebElement ele in elements)
                {
                    if (!ele.GetAttribute("title").Equals(tooltipsOfAvailableTools[i]))
                    {
                        step15_2 = false;
                        Logger.Instance.InfoLog("Tooltip mismatched for US. Expected: " + tooltipsOfAvailableTools[i] + "Actual : " + ele.GetAttribute("title"));
                        break;
                    }
                    i++;
                }

                //Select MG  Modality
                Modality.SelectByText("MG");
                Thread.Sleep(1000);
                i = 0;
                bool step15_3 = true;
                bool step15_4 = true;
                elements = BasePage.Driver.FindElements(By.CssSelector(BasePage.div_toolBoxConfiguration_ToolsInUse));
                foreach (IWebElement ele in elements)
                {
                    if (!ele.GetAttribute("title").Equals(tooltipsOfConfiguredTools[i]))
                    {
                        step15_3 = false;
                        Logger.Instance.InfoLog("Tooltip mismatched for MG. Expected: " + tooltipsOfConfiguredTools[i] + "Actual : " + ele.GetAttribute("title"));
                        break;
                    }
                    i++;
                }

                i = 0;
                elements = BasePage.Driver.FindElements(By.CssSelector(BasePage.ul_toolBoxConfiguration_AvailableTools));
                foreach (IWebElement ele in elements)
                {
                    if (!ele.GetAttribute("title").Equals(tooltipsOfAvailableTools[i]))
                    {
                        step15_4 = false;
                        Logger.Instance.InfoLog("Tooltip mismatched. Expected for MG: " + tooltipsOfAvailableTools[i] + "Actual : " + ele.GetAttribute("title"));
                        break;
                    }
                    i++;
                }

                if (step15_1 && step15_2 && step15_3 && step15_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step16 - Select any modality(e.g. MR) from the Modality drop down
                Modality.SelectByText("MR");
                Thread.Sleep(1000);
                if (Modality.SelectedOption.Text == "MR")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step17 - Mouse hover on any tools available in Toolbox section or in Available Items and verify that the Tool tip should be displayed
                i = 0;
                bool step17_1 = true;
                bool step17_2 = true;
                elements = BasePage.Driver.FindElements(By.CssSelector(BasePage.div_toolBoxConfiguration_ToolsInUse));
                foreach (IWebElement ele in elements)
                {
                    if (!ele.GetAttribute("title").Equals(tooltipsOfConfiguredTools[i]))
                    {
                        step17_1 = false;
                        Logger.Instance.InfoLog("Tooltip mismatched for MR. Expected: " + tooltipsOfConfiguredTools[i] + "Actual : " + ele.GetAttribute("title"));
                        break;
                    }
                    i++;
                }

                i = 0;
                elements = BasePage.Driver.FindElements(By.CssSelector(BasePage.ul_toolBoxConfiguration_AvailableTools));
                foreach (IWebElement ele in elements)
                {
                    if (!ele.GetAttribute("title").Equals(tooltipsOfAvailableTools[i]))
                    {
                        step17_2 = false;
                        Logger.Instance.InfoLog("Tooltip mismatched for MR. Expected: " + tooltipsOfAvailableTools[i] + "Actual : " + ele.GetAttribute("title"));
                        break;
                    }
                    i++;
                }

                if (step17_1 && step17_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step18 - Drag any tools from the Available Items(e.g. Image Scope, Save Series,Series Scope) and drop the dragged tool into any cells in the toolbox 
                IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                IWebElement group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)"));
                var dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add(AddNewTool[0], group1);
                dictionary.Add(AddNewTool[1], group1);
                dictionary.Add(AddNewTool[2], group2);
                bool step18_1 = domain.AddToolsToToolbox(dictionary, "MR", true);
                bool step18_2 = domain.GetToolsInGroupInToolBoxConfig(group1).Contains(AddNewTool[0]) && domain.GetToolsInGroupInToolBoxConfig(group1).Contains(AddNewTool[0]) &&
                                domain.GetToolsInGroupInToolBoxConfig(group2).Contains(AddNewTool[2]);
                bool step18_3 = !domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[1]);
                bool step18_4 = !domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[2]);
                bool step18_5 = !domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[0]);
                if (step18_1 && step18_2 && step18_3 && step18_4 && step18_5)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step19 - Mouse hover on 'Image Scope' tool present in toolbox section and verify Tool tip of Image Scope tool
                elements = BasePage.Driver.FindElements(By.CssSelector(BasePage.div_toolBoxConfiguration_ToolsInUse));
                bool step19 = elements.ElementAt(4).GetAttribute("title").Equals(AddNewTool[0]);
                if (step19)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step20 - Mouse hover on 'Save Series' tool present in toolbox section and verify Tool tip of Save Series tool
                bool step20 = elements.ElementAt(3).GetAttribute("title").Equals(AddNewTool[1]);
                if (step20)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step21 - 	Mouse hover on 'Series Scope' tool present in toolbox section and verify Tool tip of Series Scope Tool              
                bool step21 = elements.ElementAt(7).GetAttribute("title").Equals(AddNewTool[2]);
                if (step21)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step22 - Click on 'Save' button
                domain.ClickSaveEditDomain();
                bool isSaveButtonExists = BasePage.Driver.FindElements(By.CssSelector("[id$='EditDomainControl_SaveButton']")).Count == 0;
                if (login.IsTabSelected("Domain Management") && isSaveButtonExists)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step23 - Navigate to Domain Management and Edit superadmin Domain then go to Toolbox Configuration section
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                ExecutedSteps++;

                //Step24 - Select the same modality(MR) used in previous test from the Modality drop down
                Modality = new SelectElement(BasePage.Driver.FindElement
                                               (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("MR");                
                String[] Tools = DefaultTools.Split(':');
                Tools[0] = Tools[0] + "," + AddNewTool[1] + "," + AddNewTool[0];
                Tools[1] = Tools[1] + "," + AddNewTool[2];
                groupsInUse = domain.GetGroupsInToolBoxConfig();
                bool step24_1 = groupsInUse.Count() == 12;
                bool step24_2 = domain.VerifyConfiguredToolsInToolBoxConfig(Tools);
                if (step24_1 && step24_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step25 - Mouse hover on any tools available in Toolbox section and verify that the Tool tip should be displayed.
                elements = BasePage.Driver.FindElements(By.CssSelector(BasePage.div_toolBoxConfiguration_ToolsInUse));                
                IList<String> tooltipsOfConfiguredToolsList = new List<String>();
                foreach (String str in tooltipsOfConfiguredTools)
                {
                    tooltipsOfConfiguredToolsList.Add(str);
                }
                tooltipsOfConfiguredToolsList.Insert(3, AddNewTool[1]);
                tooltipsOfConfiguredToolsList.Insert(4, AddNewTool[0]);
                tooltipsOfConfiguredToolsList.Insert(7, AddNewTool[2]);
                bool step25 = true;
                i = 0;
                foreach (IWebElement ele in elements)
                {
                    if (!ele.GetAttribute("title").Equals(tooltipsOfConfiguredToolsList[i]))
                    {
                        step25 = false;
                        break;
                    }
                    i++;
                }
                if (step25)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step26 - Click on 'Save' button
                domain.ClickSaveEditDomain();
                bool isSaveEditButtonExists = BasePage.Driver.FindElements(By.CssSelector("[id$='EditDomainControl_SaveButton']")).Count == 0;
                if (login.IsTabSelected("Domain Management") && isSaveEditButtonExists)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step27 - Create a new domain and verify tool tips of all tools present in both Toolbox Configuration /Available Items.
                String TestDomain = "TestDomain_tb_16_" + new Random().Next(1, 10000);
                String Role = "Role_tb_16_" + new Random().Next(1, 10000);
                String DomainAdmin = "DomainAdmin_tb_16_" + new Random().Next(1, 10000);
                domain.CreateDomain(TestDomain, TestDomain, TestDomain, DomainAdmin, null, DomainAdmin, DomainAdmin, DomainAdmin, Role, Role);
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(TestDomain);
                domain.SelectDomain(TestDomain);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");

                elements = BasePage.Driver.FindElements(By.CssSelector(BasePage.div_toolBoxConfiguration_ToolsInUse));           
                i = 0;
                bool step27_1 = true;
                bool step27_2 = true;
                foreach (IWebElement ele in elements)
                {
                    if (!ele.GetAttribute("title").Equals(tooltipsOfConfiguredTools[i]))
                    {
                        step27_1 = false;
                        Logger.Instance.InfoLog("Tooltip mismatched. Expected: " + tooltipsOfConfiguredTools[i] + "Actual : " + ele.GetAttribute("title"));
                        break;
                    }
                    i++;
                }

                i = 0;
                elements = BasePage.Driver.FindElements(By.CssSelector(BasePage.ul_toolBoxConfiguration_AvailableTools));
                foreach (IWebElement ele in elements)
                {
                    if (!ele.GetAttribute("title").Equals(tooltipsOfAvailableTools[i]))
                    {
                        step27_2 = false;
                        Logger.Instance.InfoLog("Tooltip mismatched. Expected: " + tooltipsOfAvailableTools[i] + "Actual : " + ele.GetAttribute("title"));
                        break;
                    }
                    i++;
                }

                if (step27_1 && step27_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout Application                
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
            finally
            {
                DomainManagement domain = new DomainManagement();
                login.LoginIConnect("Administrator", "Administrator");
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                SelectElement Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("MR");
                Thread.Sleep(1000);
                IWebElement ele = BasePage.Driver.FindElement(By.CssSelector(DomainManagement.btn_revertToDefault));
                if (ele.Enabled)
                    ele.Click();
                Modality.SelectByText("CT");
                Thread.Sleep(1000);
                ele = BasePage.Driver.FindElement(By.CssSelector(DomainManagement.btn_revertToDefault));
                if (ele.Enabled)
                    ele.Click();
                domain.ClickSaveEditDomain();
                login.Logout();
            }
        }

        /// <summary>
        ///  At Domain Level : Modality Toolbox Configuration for various modalities
        /// </summary>
        public TestCaseResult Test_161529(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                DomainManagement domain = new DomainManagement();
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String SuperAdminGroup = Config.adminGroupName;
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');
                String DefaultToolsList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DefaultTools");
                String[] DefaultTools = DefaultToolsList.Split(':');

                //Step 1 - Login as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                if (login.IsTabPresent("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step2 - Navigate to domain Management ->Edit Super Admin Domain page -> Toolbox Configuration section, select any modality (eg. CT) from modality dropdown
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(SuperAdminGroup);
                domain.SelectDomain(SuperAdminGroup);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                SelectElement Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("CT");
                ExecutedSteps++;

                //Step3 - Drag some tools, for ex: Magnifier
                //,Zoom,Rotate ClockWise, Rotate Counterclockwise to the toolbox configuration.Then
                // Click on Save button
                IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                IWebElement group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)"));
                var dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add("Free Draw", group1);
                dictionary.Add("Reset", group2);
                bool step_3 = domain.AddToolsToToolbox(dictionary, "CT");
                bool step3 = domain.GetAvailableToolsInToolBoxConfig().Contains("Free Draw") || domain.GetAvailableToolsInToolBoxConfig().Contains("Reset");
                bool step3_1 = domain.GetConfiguredToolsInToolBoxConfig().Contains("Free Draw") && domain.GetConfiguredToolsInToolBoxConfig().Contains("Reset");
                domain.ClickSaveEditDomain();
                if (step_3 && !step3 && step3_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step4 - In Domain Management ->Edit Domain page ->
                //Toolbar Configuration section->Select Modality Type as another type as (eg.CR)
                domain.SearchDomain(SuperAdminGroup);
                domain.SelectDomain(SuperAdminGroup);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("CR");
                var groupsInUse = domain.GetGroupsInToolBoxConfig();
                bool step4_1 = groupsInUse.Count() == 12;
                bool step4_2 = domain.VerifyConfiguredToolsInToolBoxConfig(DefaultTools);
                if (step4_1 && step4_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step5 - Drag some tools ex: lnteractive Zoom, Annotation Ortholine, CobbAngle to the toolbox configuration.Then
                //Click on Save button
                group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)"));
                dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add("Free Draw", group1);
                dictionary.Add("Reset", group2);
                bool step_5 = domain.AddToolsToToolbox(dictionary, "CR");
                bool step5 = domain.GetAvailableToolsInToolBoxConfig().Contains("Free Draw") || domain.GetAvailableToolsInToolBoxConfig().Contains("Reset");
                bool step5_1 = domain.GetConfiguredToolsInToolBoxConfig().Contains("Free Draw") && domain.GetConfiguredToolsInToolBoxConfig().Contains("Reset");
                domain.ClickSaveEditDomain();
                if (step_5 && !step5 && step5_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step6 - In Domain Management ->Edit Domain page ->
                //Toolbar Configuration section->Select Modality Type as another type as (eg.CR)
                domain.SearchDomain(SuperAdminGroup);
                domain.SelectDomain(SuperAdminGroup);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("MR");
                groupsInUse = domain.GetGroupsInToolBoxConfig();
                bool step6_1 = groupsInUse.Count() == 12;
                bool step6_2 = domain.VerifyConfiguredToolsInToolBoxConfig(DefaultTools);
                if (step6_1 && step6_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step7 - Drag some tools ex: lnteractive Zoom, Annotation Ortholine, CobbAngle to the toolbox configuration.Then
                //Click on Save button
                group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)"));
                dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add("Free Draw", group1);
                dictionary.Add("Reset", group2);
                bool step_7 = domain.AddToolsToToolbox(dictionary, "MR");
                bool step7 = domain.GetAvailableToolsInToolBoxConfig().Contains("Free Draw") || domain.GetAvailableToolsInToolBoxConfig().Contains("Reset");
                bool step7_1 = domain.GetConfiguredToolsInToolBoxConfig().Contains("Free Draw") && domain.GetConfiguredToolsInToolBoxConfig().Contains("Reset");
                domain.ClickSaveEditDomain();
                if (step_7 && !step7 && step7_1)
                {
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

                //Step8 - Go back to studylist, Load a study which contains series of different modality. (CR, MR, CT)                
                login.LoginIConnect(adminUserName, adminPassword);
                var studies = (Studies)login.Navigate("Studies");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.ClearSearchBtn()));
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA77));
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step9 - Select CR modality series. Right click the mouse button and verify that the user shall open the toolbox.
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step10 -	Verify the tools are present in the floating toolbox
                String ToolsList = "Free Draw,Interactive Window Width/Level:Reset,Magnifier:Flip Vertical,Rotate Clockwise,Rotate Counter Clockwise,Flip Horizontal:" +
                                    "Pan:Line Measurement:Distance Calibration:Cobb Angle:Angle Measurement:Save Annotated Image,Save Series,Add Text:" +
                                    "Invert Greyscale Image:Pixel Value:Draw Rectangle,Draw ROI,Draw Ellipse";

                String[] Tools = ToolsList.Split(':');
                var toolsInViewer = viewer.GetGroupsInToolBox();
                bool step10_1 = toolsInViewer.Count() == 12;
                bool step10_2 = viewer.VerifyConfiguredTools(Tools);
                if (step10_1 && step10_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step11 - Select any tool from the floating toolbox and apply it.
                viewer.SelectViewerTool(BluRingTools.Line_Measurement);
                viewer.ApplyTool_LineMeasurement();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step11 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step11)
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

                //Step12 - Select MR modality series. Right click the mouse button and verify that the user shall open the toolbox
                viewer.SetViewPort(2, 1);
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step13 - Verify the tools are present in the floating toolbox
                ToolsList = "Free Draw,Interactive Window Width/Level:Reset,Magnifier:Flip Vertical,Rotate Clockwise,Rotate Counter Clockwise,Flip Horizontal:" +
                                    "Pan:Line Measurement:Distance Calibration:Cobb Angle:Angle Measurement:Save Annotated Image,Save Series,Add Text:" +
                                    "Invert Greyscale Image:Pixel Value:Draw Rectangle,Draw ROI,Draw Ellipse";

                Tools = ToolsList.Split(':');
                toolsInViewer = viewer.GetGroupsInToolBox();
                bool step13_1 = toolsInViewer.Count() == 12;
                bool step13_2 = viewer.VerifyConfiguredTools(Tools);
                if (step13_1 && step13_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step14 - Select any tool from the floating toolbox and apply it.
                viewer.SelectViewerTool(BluRingTools.Line_Measurement, 1, 2);
                viewer.ApplyTool_LineMeasurement();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step14 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step14)
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

                //Step15 - Select CT modality series. Right click the mouse button and verify that the user shall open the toolbox
                viewer.SetViewPort(1, 1);
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step16 - Verify the tools are present in the floating toolbox
                ToolsList = "Free Draw,Interactive Window Width/Level:Reset,Magnifier:Flip Vertical,Rotate Clockwise,Rotate Counter Clockwise,Flip Horizontal:" +
                                   "Pan:Line Measurement:Distance Calibration:Cobb Angle:Angle Measurement:Save Annotated Image,Save Series,Add Text:" +
                                   "Invert Greyscale Image:Pixel Value:Draw Rectangle,Draw ROI,Draw Ellipse";

                Tools = ToolsList.Split(':');
                toolsInViewer = viewer.GetGroupsInToolBox();
                bool step16_1 = toolsInViewer.Count() == 12;
                bool step16_2 = viewer.VerifyConfiguredTools(Tools);
                if (step16_1 && step16_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step17 - 	Select any tool from the floating toolbox and apply it.
                viewer.SelectViewerTool(BluRingTools.Line_Measurement, 1, 1);
                viewer.ApplyTool_LineMeasurement();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step17 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step17)
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
                viewer.CloseBluRingViewer();

                //Step18 - Edit the Domain Management Page for administrator account.
                login.NavigateToDomainManagementTab();
                ExecutedSteps++;

                //Step19 - Scroll to Toolbar Configuration section at the bottom of the page.
                //Select Toolbar Type as CR.
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("CR");

                ToolsList = "Window Level,Free Draw:Magnifier,Reset:Flip Horizontal,Flip Vertical,Rotate Clockwise,Rotate Counter Clockwise:Pan:" +
                    "Annotation OrthoLine:Calibration:Cobb Angle:Measure Angle:Add Text,Save Series,Save Annotated Image:Invert:Get Pixel Value:Roi Circle,Roi Rectangle,Roi Draw";
                Tools = ToolsList.Split(':');
                bool step19 = viewer.VerifyConfiguredToolsInToolBoxConfig(Tools);
                if (step19)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step20 - Change the modality tools setting for CR modality:
                //Add / Remove few more tools to/ from available items.
                //Save the changes.			
                var ToolsToBeRemoved = new List<String>();
                ToolsToBeRemoved.Add("Pan");
                ToolsToBeRemoved.Add("Invert");
                domain.RemoveToolsFromConfiguredSection(ToolsToBeRemoved, "CR");
                bool step20_1 = domain.GetConfiguredToolsInToolBoxConfig().Contains("Pan") ||
                                    domain.GetConfiguredToolsInToolBoxConfig().Contains("Invert");
                bool step20_2 = domain.GetAvailableToolsInToolBoxConfig().Contains("Pan") &&
                                domain.GetAvailableToolsInToolBoxConfig().Contains("Invert");
                domain.ClickSaveEditDomain();
                if (!step20_1 && step20_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step21 - Repeat previous step to configure the floating toolbox
                //for MR and CT modalities.
                login.NavigateToDomainManagementTab();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                ToolsToBeRemoved = new List<String>();
                ToolsToBeRemoved.Add("Pan");
                ToolsToBeRemoved.Add("Invert");
                domain.RemoveToolsFromConfiguredSection(ToolsToBeRemoved, "CR");
                bool step21_1 = domain.GetConfiguredToolsInToolBoxConfig().Contains("Pan") ||
                                    domain.GetConfiguredToolsInToolBoxConfig().Contains("Invert");
                bool step21_2 = domain.GetAvailableToolsInToolBoxConfig().Contains("Pan") &&
                                domain.GetAvailableToolsInToolBoxConfig().Contains("Invert");

                domain.RemoveToolsFromConfiguredSection(ToolsToBeRemoved, "CR");
                bool step21_3 = domain.GetConfiguredToolsInToolBoxConfig().Contains("Pan") ||
                                    domain.GetConfiguredToolsInToolBoxConfig().Contains("Invert");
                bool step21_4 = domain.GetAvailableToolsInToolBoxConfig().Contains("Pan") &&
                                domain.GetAvailableToolsInToolBoxConfig().Contains("Invert");
                domain.ClickSaveEditDomain();

                if (!step21_1 && step21_2 && !step21_3 && step21_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step22 - Repeat the previous steps for various modalities(eg: US,NM,MG).
                //login.NavigateToDomainManagementTab();
                //BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                //Modality = new SelectElement(BasePage.Driver.FindElement
                //                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                //Modality.SelectByText("US");
                //group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                //group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)"));
                //dictionary = new Dictionary<String, IWebElement>();
                //dictionary.Add("Free Draw", group1);
                //dictionary.Add("Reset", group2);
                //bool step_22 = domain.AddToolsToToolbox(dictionary, "US");
                //bool step22_1 = domain.GetAvailableToolsInToolBoxConfig().Contains("Free Draw") && domain.GetAvailableToolsInToolBoxConfig().Contains("Reset");

                //group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                //group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)"));
                //dictionary = new Dictionary<String, IWebElement>();
                //dictionary.Add("Free Draw", group1);
                //dictionary.Add("Reset", group2);
                //bool step22 = domain.AddToolsToToolbox(dictionary, "NM");
                //bool step22_2 = domain.GetAvailableToolsInToolBoxConfig().Contains("Free Draw") && domain.GetAvailableToolsInToolBoxConfig().Contains("Reset");

                //group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                //group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)"));
                //dictionary = new Dictionary<String, IWebElement>();
                //dictionary.Add("Free Draw", group1);
                //dictionary.Add("Reset", group2);
                //bool step221 = domain.AddToolsToToolbox(dictionary, "MG");
                //bool step22_3 = domain.GetAvailableToolsInToolBoxConfig().Contains("Free Draw") && domain.GetAvailableToolsInToolBoxConfig().Contains("Reset");
                //domain.ClickSaveEditDomain();
                //if (step22_1 && step_22 && step22 && step22_2 && step221 && step22_3)
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}

                ////Step23 - Create new domain and repeat the previous steps for various modalities(eg: US,NM,MG).
                //studies = (Studies)login.Navigate("Studies");
                //BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.ClearSearchBtn()));
                //studies.SearchStudy(AccessionNo: Accession[1], Datasource: login.GetHostName(Config.EA77));
                //studies.SelectStudy("Accession", Accession[1]);
                //viewer = BluRingViewer.LaunchBluRingViewer();
                //PageLoadWait.WaitForFrameLoad(20);
                //viewer.OpenViewerToolsPOPUp();
                //ToolsList = "Free Draw,Interactive Window Width/Level:Reset,Magnifier:Flip Vertical,Rotate Clockwise,Rotate Counter Clockwise,Flip Horizontal:" +
                //                  "Pan:Line Measurement:Distance Calibration:Cobb Angle:Angle Measurement:Save Annotated Image,Save Series,Add Text:" +
                //                  "Invert Greyscale Image:Pixel Value:Draw Rectangle,Draw ROI,Draw Ellipse";

                //Tools = ToolsList.Split(':');
                //toolsInViewer = viewer.GetGroupsInToolBox();
                //bool step23_1 = toolsInViewer.Count() == 12;
                //bool step23_2 = viewer.VerifyConfiguredTools(Tools);
                //viewer.SelectViewerTool(BluRingTools.Line_Measurement); // need to mention viewport
                //viewer.ApplyTool_LineMeasurement();
                //result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                //bool step23 = studies.CompareImage(result.steps[ExecutedSteps],
                //                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                //if (step23_1 && step23_2 && step23)
                //{
                //    result.steps[ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}

                result.steps[++ExecutedSteps].status = "On-Hold";
                result.steps[++ExecutedSteps].status = "On-Hold";

                //Logout Application
                //viewer.CloseBluRingViewer();
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
            finally
            {
                DomainManagement domain = new DomainManagement();
                login.LoginIConnect("Administrator", "Administrator");
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                SelectElement Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("CT");
                Thread.Sleep(1000);
                IWebElement revertButton = BasePage.Driver.FindElement(By.CssSelector(DomainManagement.btn_revertToDefault));
                if (revertButton.Enabled)
                    revertButton.Click();
                Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("CR");
                Thread.Sleep(1000);
                if (revertButton.Enabled)
                    revertButton.Click();
                Modality = new SelectElement(BasePage.Driver.FindElement
                                               (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("MR");
                Thread.Sleep(1000);
                if (revertButton.Enabled)
                    revertButton.Click();
                domain.ClickSaveEditDomain();
                login.Logout();
            }
        }

        /// <summary>
		///  At Domain level: Configure tools to the specific modality(DX) toolbox
		/// </summary>
		public TestCaseResult Test_161533(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                DomainManagement domain = new DomainManagement();
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');
                String TestDomain = "Domain_532_" + new Random().Next(1, 10000);
                String Role = "Role_532_" + new Random().Next(1, 10000);
                String DomainAdmin = "DomainAdmin_532_" + new Random().Next(1, 10000);


                //Step 1 - Login as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                if (login.IsTabPresent("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step2 - In Domain Management ->Edit Domain page -> Toolbox Configuration section, select Modality as DX.
                login.NavigateToDomainManagementTab();
                domain.CreateDomain(TestDomain, TestDomain, TestDomain, DomainAdmin, null, DomainAdmin, DomainAdmin, DomainAdmin, Role, Role);
                domain.SearchDomain(TestDomain);
                domain.SelectDomain(TestDomain);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                SelectElement Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("DX");
                ExecutedSteps++;

                //Step3 - Drag few tools(ex.Flip Horizontal
                //,Flip Vertical, Roi Draw,Magnifier,
                //Zoom) from the Available Items box and drop it in to specified cell in toolbox configuration.				
                IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                IWebElement group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)"));
                var dictionary = new Dictionary<String, IWebElement>();
                string Series_Scope = BluRingViewer.GetToolName(BluRingTools.Series_Scope);
                string Save_Series = BluRingViewer.GetToolName(BluRingTools.Save_Series);
                dictionary.Add(Series_Scope, group1);
                dictionary.Add(Save_Series, group2);
                bool step_3 = domain.AddToolsToToolbox(dictionary, "DX", true);
                bool step3 = domain.GetAvailableToolsInToolBoxConfig().Contains(Series_Scope) || domain.GetAvailableToolsInToolBoxConfig().Contains(Save_Series);
                bool step3_1 = domain.GetConfiguredToolsInToolBoxConfig().Contains(Series_Scope) && domain.GetConfiguredToolsInToolBoxConfig().Contains(Save_Series);
                IList<string> DomainToolsInAfterEdit = basepage.GetToolsInToolBoxConfigByEachColumn();
                domain.ClickSaveEditDomain();
                if (step_3 && !step3 && step3_1)
                {
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

                //Step4 - Navigate to Studies tab,search and Load a study which contains series of DX Modality and then click on 'View Exam ' button               
                login.LoginIConnect(DomainAdmin, DomainAdmin);
                var studies = (Studies)login.Navigate("Studies");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.ClearSearchBtn()));
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step5 - Right click on stacked tool and verify the configured stacked menu tools in the Domain Management Page
                viewer.OpenViewerToolsPOPUp();
                if (viewer.GetToolsInToolBoxByGrid().SequenceEqual(DomainToolsInAfterEdit) && (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer))))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("DomainToolsInAfterEdit" + string.Join(",", DomainToolsInAfterEdit.ToArray()));
                    Logger.Instance.ErrorLog("ToolsInViewer" + string.Join(",", viewer.GetToolsInToolBoxByGrid().ToArray()));
                }

                //Step6 - Click top level tool and right click to open stack list then select any tool from the stack list and apply it
                viewer.OpenStackedTool(BluRingTools.Window_Level, isOpenToolsPOPup: false);
                var step6 = viewer.SelectViewerTool(BluRingTools.Window_Level, isOpenToolsPOPup: false);
                //var step6 = viewer.SelectInnerViewerTool( BluRingTools.Invert , BluRingTools.Window_Level, isOpenToolsPOPup: true);
                viewer.ApplyTool_WindowWidth();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step6_1 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step6 && step6_1)
                {
                    result.steps[ExecutedSteps].StepPass();
                 }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseBluRingViewer();

                //Step7 - Edit the Domain Management Page for administrator account.
                login.NavigateToDomainManagementTab();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
                Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("DX");
                ExecutedSteps++;

                //Step8 - Remove one tool from the stack list by drag and drop into the available box item
                var ToolsToBeRemoved = new List<String>();
                ToolsToBeRemoved.Add(Series_Scope);
                domain.RemoveToolsFromConfiguredSection(ToolsToBeRemoved, "DX");
                Thread.Sleep(3000);
                bool step8 = !domain.GetConfiguredToolsInToolBoxConfig().Contains(Series_Scope);
                bool step8_1 = domain.GetAvailableToolsInToolBoxConfig().Contains(Series_Scope);
                if (step8 && step8_1)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step9 - Add new tool into the stack list, by selecting it then drag it from the Available Items list.	          		
                group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                dictionary = new Dictionary<String, IWebElement>();
                string ImageScope = BluRingViewer.GetToolName(BluRingTools.Image_Scope);
                dictionary.Add(ImageScope, group1);
                domain.AddToolsToToolbox(dictionary, "DX", true);
                Thread.Sleep(3000);
                bool step9 = domain.GetConfiguredToolsInToolBoxConfig().Contains(ImageScope);
                bool step9_1 = !domain.GetAvailableToolsInToolBoxConfig().Contains(ImageScope);
                if (step9 && step9_1)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                DomainToolsInAfterEdit = basepage.GetToolsInToolBoxConfigByEachColumn();
                domain.ClickSaveEditDomain();

                //Step10 - Navigate to Studies tab,search and Load a study which contains series of DX Modality and then click on 'View Exam ' button
                studies = (Studies)login.Navigate("Studies");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.ClearSearchBtn()));
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Accession", Accession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step11 -Select any series viewport and click on right mouse button
                viewer.OpenViewerToolsPOPUp();
                PageLoadWait.WaitForFrameLoad(20);
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step12 - Verify the Modality toolbox updates to reflect the configuration for the DX modality as specified in Domain Management page. When the floating toolbox opened
                if (viewer.GetToolsInToolBoxByGrid().SequenceEqual(DomainToolsInAfterEdit))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("ToolsInViewer" + string.Join(",", viewer.GetToolsInToolBoxByGrid().ToArray()));
                    Logger.Instance.ErrorLog("DomainToolsInAfterEdit" + string.Join(",", DomainToolsInAfterEdit.ToArray()));
                }

                // Step13 -Select any tool from the modality toolbox and apply it.
                viewer.OpenStackedTool(BluRingTools.Line_Measurement,false);
                var step13 = viewer.SelectViewerTool(BluRingTools.Line_Measurement);
                viewer.ApplyTool_LineMeasurement();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step13_1 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step13 && step13_1)
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

                //Logout Application
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
        /// At Domain level: Configure tools to the specific modality(MR) toolbox
        /// </summary>
        public TestCaseResult Test_161534(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                DomainManagement domain = new DomainManagement();
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');
                String TestDomain = "Domain_533_" + new Random().Next(1, 10000);
                String Role = "Role_533_" + new Random().Next(1, 10000);
                String DomainAdmin = "DomainAdmin_533_" + new Random().Next(1, 10000);


                //Step 1 - Login as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                if (login.IsTabPresent("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step2 - In Domain Management ->Edit Domain page -> Toolbox Configuration section, select Modality as MR
                login.NavigateToDomainManagementTab();
                domain.CreateDomain(TestDomain, TestDomain, TestDomain, DomainAdmin, null, DomainAdmin, DomainAdmin, DomainAdmin, Role, Role);
                domain.SearchDomain(TestDomain);
                domain.SelectDomain(TestDomain);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                SelectElement Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("MR");
                ExecutedSteps++;

                //Step3 - Drag some tools, for ex. Free Draw, Roi Draw to the toolbox configuration and Save the changes.				
                IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                IWebElement group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)"));
                var dictionary = new Dictionary<String, IWebElement>();
                string Series_Scope = BluRingViewer.GetToolName(BluRingTools.Series_Scope);
                string Save_Series = BluRingViewer.GetToolName(BluRingTools.Save_Series);
                dictionary.Add(Series_Scope, group1);
                dictionary.Add(Save_Series, group2);
                bool step_3 = domain.AddToolsToToolbox(dictionary, "MR", true);
                bool step3 = domain.GetAvailableToolsInToolBoxConfig().Contains(Series_Scope) || domain.GetAvailableToolsInToolBoxConfig().Contains(Save_Series);
                bool step3_1 = domain.GetConfiguredToolsInToolBoxConfig().Contains(Series_Scope) && domain.GetConfiguredToolsInToolBoxConfig().Contains(Save_Series);
                if (step_3 && !step3 && step3_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step4 - Click on save button
                domain.ClickSaveEditDomain();
                login.Logout();
                ExecutedSteps++;

                //Step5 - Navigate to studies tab and load any MR modality study which has KO image by clicking on "View Exam" button
                login.LoginIConnect(DomainAdmin, DomainAdmin);
                var studies = (Studies)login.Navigate("Studies");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.ClearSearchBtn()));
                studies.SearchStudy(AccessionNo: Accession[0]);
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step6 - Select any series viewport which has MR series then click on right mouse button
                viewer.SetViewPort(1, 1);
                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport).Click();
                PageLoadWait.WaitForFrameLoad(20);
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step7 - Verify that the configured tools present under specified cells in the floating toolbox for MR Modality as specified in Domain Management page.
                String[] Tools = DefaultTools.Split(':');
                Tools[0] = Tools[0] + "," + Series_Scope;
                Tools[1] = Tools[1] + "," + Save_Series;
                var toolsInViewer = viewer.GetGroupsInToolBox(2, 1);
                bool step7_1 = toolsInViewer.Count() == 12;
                bool step7_2 = viewer.VerifyConfiguredTools(Tools,1,2);
                if (step7_1 && step7_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step8 - Select any tool from the toolbox and apply it.
                viewer.SelectViewerTool(BluRingTools.Line_Measurement, 1, 2, isOpenToolsPOPup: false);
                viewer.ApplyTool_LineMeasurement();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step8 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step8)
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
                viewer.CloseBluRingViewer();

              
                //Step9 - Navigate to Domain Management ->Edit Domain page -> Toolbox Configuration section, select Modality as MR.
                login.NavigateToDomainManagementTab();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
                Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("MR");
                ExecutedSteps++;

                //Step10 - Remove any tool from any cell by drag and drop into the available box item				
                var ToolsToBeRemoved = new List<String>();
                ToolsToBeRemoved.Add(Series_Scope);
                domain.RemoveToolsFromConfiguredSection(ToolsToBeRemoved, "MR");
                Thread.Sleep(3000);
                bool step10 = !domain.GetConfiguredToolsInToolBoxConfig().Contains(Series_Scope);
                bool step10_1 = domain.GetAvailableToolsInToolBoxConfig().Contains(Series_Scope);
                if (step10 && step10_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step11 - Add few tools to any cell in a stack control flow by drag and drop from available box			
                group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                dictionary = new Dictionary<String, IWebElement>();
                String ImageScope = BluRingViewer.GetToolName(BluRingTools.Image_Scope);
                dictionary.Add(ImageScope, group1);
                dictionary.Add(Series_Scope, group1);
                domain.AddToolsToToolbox(dictionary, "MR", true);
                Thread.Sleep(3000);
                if (domain.GetConfiguredToolsInToolBoxConfig().Contains(ImageScope) && domain.GetConfiguredToolsInToolBoxConfig().Contains(Series_Scope))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                domain.ClickSaveEditDomain();

                //Step12 -Navigate to studies tab and load any MR modality study by clicking on "View Exam" button
                studies = (Studies)login.Navigate("Studies");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.ClearSearchBtn()));
                studies.SearchStudy(AccessionNo: Accession[1], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step13 - Select any view port which has MR series then click on right mouse button
                viewer.SetViewPort(3, 1);
                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport).Click();
                PageLoadWait.WaitForFrameLoad(20);
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step14 -Verify that the configured tools present under specified cells in the floating toolbox for MR Modality as specified in Domain Management page.
                Tools = DefaultTools.Split(':');
                Tools[0] = Tools[0] + "," + ImageScope + "," + Series_Scope;
                Tools[1] = Tools[1] + "," + Save_Series;
                bool step14_1 = viewer.VerifyConfiguredTools(Tools, 1, 4);
                if (step14_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step15 - Drag and drop/double click on the PR image from thumbnail bar then load into the view port
                viewer.SetViewPort(1, 1);
                IList<IWebElement> Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));

                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport).Click();
                TestCompleteAction action = new TestCompleteAction();
                IWebElement TargetElement = viewer.GetElement("cssselector", viewer.Activeviewport);
                action.DragAndDrop(Thumbnail_list[0], TargetElement);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                var step15 = viewer.CompareImage(result.steps[ExecutedSteps], TargetElement);

                if (step15)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step16 - Select any view port which has PR image then click on right mouse button
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step17 - Verify that the configured tools present under specified cells in the floating toolbox               
                bool step17_1 = viewer.VerifyConfiguredTools(Tools, 1, 2);
                if (step17_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseBluRingViewer();

                //Step18 - Drag and drop/double click on the KO image from thumbnail bar then load into the view port
                studies = (Studies)login.Navigate("Studies");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.ClearSearchBtn()));
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Accession", Accession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                //viewer.SetViewPort(1, 1);
                //action = new TestCompleteAction();
                TargetElement = viewer.GetElement("cssselector", viewer.Activeviewport);
                //action.DragAndDrop(Thumbnail_list[0], TargetElement);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                var step18 = viewer.CompareImage(result.steps[ExecutedSteps], TargetElement);

                if (step18)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step19 - Select any view port which has KO image then click on right mouse button
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step20 - Verify that the configured tools present under specified cells in the floating toolbox               
                bool step20_1 = viewer.VerifyConfiguredTools(Tools);
                if (step20_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout Application
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
        ///  At Domain Level : Modality Toolbox Configuration for various modalities CT,CR,MR
        /// </summary>
        public TestCaseResult Test1_161529(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                DomainManagement domain = new DomainManagement();
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String SuperAdminGroup = Config.adminGroupName;
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');                                

                //Step 1 - Login as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                if (login.IsTabPresent("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step2 - Navigate to domain Management ->Edit Super Admin Domain page -> Toolbox Configuration section, select any modality (eg. CT) from modality dropdown
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(SuperAdminGroup);
                domain.SelectDomain(SuperAdminGroup);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                SelectElement Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("CT");
                ExecutedSteps++;

                //Step3 - Drag some tools, for ex: Magnifier
                //,Zoom,Rotate ClockWise, Rotate Counterclockwise to the toolbox configuration.Then
                // Click on Save button
                IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                IWebElement group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)"));
                var dictionary = new Dictionary<String, IWebElement>();
                String Tool1 = domainmanagement.GetAvailableToolsInToolBoxConfig()[0];
                String Tool2 = domainmanagement.GetAvailableToolsInToolBoxConfig()[1];
                dictionary.Add(Tool1, group1);
                dictionary.Add(Tool2, group2);
                bool step_3 = domain.AddToolsToToolbox(dictionary, "CT", true);
                bool step3 = domain.GetAvailableToolsInToolBoxConfig().Contains(Tool1) || domain.GetAvailableToolsInToolBoxConfig().Contains(Tool2);
                bool step3_1 = domain.GetConfiguredToolsInToolBoxConfig().Contains(Tool1) && domain.GetConfiguredToolsInToolBoxConfig().Contains(Tool2)
                      && domain.GetToolsInGroupInToolBoxConfig(group1).Contains(Tool1) && domain.GetToolsInGroupInToolBoxConfig(group2).Contains(Tool2);
                IList<string> ToolsInDomainAfterEdit_CT = new List<string>();
                foreach (IWebElement columnTools in domainmanagement.GetGroupsInToolBoxConfig())
                {
                    IList<string> toolsInEachColumn = domainmanagement.GetToolsInGroupInToolBoxConfig(columnTools);
                    ToolsInDomainAfterEdit_CT.Add(String.Join(",", toolsInEachColumn));
                }

                if (step_3 && !step3 && step3_1 )
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step4 - In Domain Management ->Edit Domain page ->
                //Toolbar Configuration section->Select Modality Type as another type as (eg.CR)               
                Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("CR");
                var groupsInUse = domain.GetGroupsInToolBoxConfig();
                bool step4_1 = groupsInUse.Count() == 12;
                bool step4_2 = domain.VerifyConfiguredToolsInToolBoxConfig(DefaultTools.Split(':'));
                if (step4_1 && step4_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step5 - Drag some tools ex: lnteractive Zoom, Annotation Ortholine, CobbAngle to the toolbox configuration.Then
                //Click on Save button
                IWebElement group3 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(3)"));
                dictionary = new Dictionary<String, IWebElement>();
                String Tool3 = domainmanagement.GetAvailableToolsInToolBoxConfig()[1];
                dictionary.Add(Tool3, group3);
                bool step_5 = domain.AddToolsToToolbox(dictionary, "CR", true);
                bool step5 = domain.GetAvailableToolsInToolBoxConfig().Contains(Tool3);
                bool step5_1 = domain.GetConfiguredToolsInToolBoxConfig().Contains(Tool3);
                IList<string> ToolsInDomainAfterEdit_CR = new List<string>();
                foreach (IWebElement columnTools in domainmanagement.GetGroupsInToolBoxConfig())
                {
                    IList<string> toolsInEachColumn = domainmanagement.GetToolsInGroupInToolBoxConfig(columnTools);
                    ToolsInDomainAfterEdit_CR.Add(String.Join(",", toolsInEachColumn));
                }
                if (step_5 && !step5 && step5_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step6 - In Domain Management ->Edit Domain page ->
                //Toolbar Configuration section->Select Modality Type as another type as (eg.CR)            
                Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("MR");
                groupsInUse = domain.GetGroupsInToolBoxConfig();
                bool step6_1 = groupsInUse.Count() == 12;
                bool step6_2 = domain.VerifyConfiguredToolsInToolBoxConfig(DefaultTools.Split(':'));
                if (step6_1 && step6_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step7 - Drag some tools ex: lnteractive Zoom, Annotation Ortholine, CobbAngle to the toolbox configuration.Then                
                 IWebElement group4 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(4)"));
                dictionary = new Dictionary<String, IWebElement>();
                String Tool4 = domainmanagement.GetAvailableToolsInToolBoxConfig()[2];
                dictionary.Add(Tool4, group4);
                bool step_7 = domain.AddToolsToToolbox(dictionary, "MR", true);
                bool step7 =  domain.GetAvailableToolsInToolBoxConfig().Contains(Tool4);
                bool step7_1 = domain.GetConfiguredToolsInToolBoxConfig().Contains(Tool4);
                IList<string> ToolsInDomainAfterEdit_MR = new List<string>();
                foreach (IWebElement columnTools in domainmanagement.GetGroupsInToolBoxConfig())
                {
                    IList<string> toolsInEachColumn = domainmanagement.GetToolsInGroupInToolBoxConfig(columnTools);
                    ToolsInDomainAfterEdit_MR.Add(String.Join(",", toolsInEachColumn));
                }
                if (step_7 && !step7 && step7_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step8 - Click on Save button
                domain.ClickSaveEditDomain();
                ExecutedSteps++;

                //Step9 - Go back to studylist, Load a study which contains series of different modality. (CR, MR, CT)                                
                var studies = (Studies)login.Navigate("Studies");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.ClearSearchBtn()));
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA77));
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step10 - Select CR modality series. Right click the mouse button and verify that the user shall open the toolbox.
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step11 -	Verify the tools are present in the floating toolbox                
                String[] Tools = DefaultTools.Split(':');
                var toolsInViewer = viewer.GetGroupsInToolBox();
                bool step11_1 = toolsInViewer.Count() == 12;
                bool step11_2 = viewer.VerifyConfiguredTools(ToolsInDomainAfterEdit_CR.ToArray());
                if (step11_1 && step11_2 )
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step12 - Select any tool from the floating toolbox and apply it.
                viewer.OpenStackedTool(BluRingTools.Pan, isOpenToolsPOPup: false);
                viewer.SelectViewerTool(BluRingTools.Pan, isOpenToolsPOPup: false);
                viewer.ApplyTool_Pan();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step12 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step12)
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

                //Step13 - Select MR modality series. Right click the mouse button and verify that the user shall open the toolbox
                viewer.SetViewPort(2, 1);
                viewer.OpenViewerToolsPOPUp();
                toolsInViewer = viewer.GetGroupsInToolBox(3, 1);
                bool step13_1 = toolsInViewer.Count() == 12;
                if (step13_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step14 - Verify the tools are present in the floating toolbox                
                bool step14_1 = viewer.VerifyConfiguredTools(ToolsInDomainAfterEdit_MR.ToArray(), 1, 3);
                if (step14_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step15 - Select any tool from the floating toolbox and apply it.
                viewer.SelectViewerTool(BluRingTools.Pan, 1, 3, isOpenToolsPOPup: false);
                viewer.ApplyTool_Pan();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step15 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step15)
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

                //Step16 - Select CT modality series. Right click the mouse button and verify that the user shall open the toolbox
                viewer.SetViewPort(1, 1);
                viewer.OpenViewerToolsPOPUp();
                toolsInViewer = viewer.GetGroupsInToolBox(2, 1);
                bool step16_1 = toolsInViewer.Count() == 12;
                if (step16_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step17 - Verify the tools are present in the floating toolbox
                bool step17_1 = viewer.VerifyConfiguredTools(ToolsInDomainAfterEdit_CT.ToArray(), 1, 2);
                if (step17_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step18 - 	Select any tool from the floating toolbox and apply it.
                viewer.SelectViewerTool(BluRingTools.Pan, 1, 2, isOpenToolsPOPup: false);
                viewer.ApplyTool_Pan();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step18 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step18)
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
                viewer.CloseBluRingViewer();

                //Step19 - Edit the Domain Management Page for administrator account.
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                ExecutedSteps++;

                //Step20 - Scroll to Toolbar Configuration section at the bottom of the page.
                //Select Toolbar Type as CR.
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("CR");                
                bool step20 = viewer.VerifyConfiguredToolsInToolBoxConfig(ToolsInDomainAfterEdit_CR.ToArray());
                if (step20)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step21 - Change the modality tools setting for CR modality:
                //Add / Remove few more tools to/ from available items.                		
                var ToolsToBeRemoved = new List<String>();
                string RemoveTool1 = domain.GetConfiguredToolsInToolBoxConfig()[0];
                string RemoveTool2 = domain.GetConfiguredToolsInToolBoxConfig()[1];
                ToolsToBeRemoved.Add(RemoveTool1);
                ToolsToBeRemoved.Add(RemoveTool2);
                domain.RemoveToolsFromConfiguredSection(ToolsToBeRemoved, "CR");
                domain.ClickSaveEditDomain();
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                Modality = new SelectElement(BasePage.Driver.FindElement
                                              (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("CR");

                bool step21_1 = domain.GetConfiguredToolsInToolBoxConfig().Contains(RemoveTool1) ||
                                    domain.GetConfiguredToolsInToolBoxConfig().Contains(RemoveTool2);
                bool step21_2 = domain.GetAvailableToolsInToolBoxConfig().Contains(RemoveTool1) &&
                                domain.GetAvailableToolsInToolBoxConfig().Contains(RemoveTool2);
                if (!step21_1 && step21_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step22 - Configure the floating toolbox for MR and CT modalities.
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                Modality = new SelectElement(BasePage.Driver.FindElement
                                             (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("CT");
                string RemoveTool1_CT = domain.GetConfiguredToolsInToolBoxConfig()[0];
                string RemoveTool2_CT = domain.GetConfiguredToolsInToolBoxConfig()[1];
                ToolsToBeRemoved = new List<String>();
                ToolsToBeRemoved.Add(RemoveTool1_CT);
                ToolsToBeRemoved.Add(RemoveTool2_CT);
                domain.RemoveToolsFromConfiguredSection(ToolsToBeRemoved, "CT");
                domain.ClickSaveEditDomain();
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                Modality = new SelectElement(BasePage.Driver.FindElement
                                             (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("CT");

                bool step22_1 = domain.GetConfiguredToolsInToolBoxConfig().Contains(RemoveTool1_CT) ||
                                    domain.GetConfiguredToolsInToolBoxConfig().Contains(RemoveTool2_CT);
                bool step22_2 = domain.GetAvailableToolsInToolBoxConfig().Contains(RemoveTool1_CT) &&
                                domain.GetAvailableToolsInToolBoxConfig().Contains(RemoveTool2_CT);

                Modality.SelectByText("MR");
                string RemoveTool1_MR = domain.GetConfiguredToolsInToolBoxConfig()[3];
                string RemoveTool2_MR = domain.GetConfiguredToolsInToolBoxConfig()[4];
                ToolsToBeRemoved = new List<String>();
                ToolsToBeRemoved.Add(RemoveTool1_MR);
                ToolsToBeRemoved.Add(RemoveTool2_MR);
                domain.RemoveToolsFromConfiguredSection(ToolsToBeRemoved, "MR");
                domain.ClickSaveEditDomain();
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                Modality = new SelectElement(BasePage.Driver.FindElement
                                            (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("MR");

                bool step22_3 = domain.GetConfiguredToolsInToolBoxConfig().Contains(RemoveTool1_MR) ||
                                    domain.GetConfiguredToolsInToolBoxConfig().Contains(RemoveTool2_MR);
                bool step22_4 = domain.GetAvailableToolsInToolBoxConfig().Contains(RemoveTool1_MR) &&
                                domain.GetAvailableToolsInToolBoxConfig().Contains(RemoveTool2_MR);
             

                if (!step22_1 && step22_2 && !step22_3 && step22_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                domain.ClickSaveEditDomain();


                //Logout Application               
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
            finally
            {
                DomainManagement domain = new DomainManagement();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                SelectElement Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("CT");
                Thread.Sleep(1000);
                IWebElement revertButton = BasePage.Driver.FindElement(By.CssSelector(DomainManagement.btn_revertToDefault));
                if (revertButton.Enabled)
                    revertButton.Click();
                Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("CR");
                Thread.Sleep(1000);
                if (revertButton.Enabled)
                    revertButton.Click();
                Modality = new SelectElement(BasePage.Driver.FindElement
                                               (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("MR");
                Thread.Sleep(1000);
                if (revertButton.Enabled)
                    revertButton.Click();
                domain.ClickSaveEditDomain();
                login.Logout();
            }
        }

        /// <summary>
        ///  At Domain Level : Modality Toolbox Configuration for various modalities US,NM,MG
        /// </summary>
        public TestCaseResult Test2_161529(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                DomainManagement domain = new DomainManagement();
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String SuperAdminGroup = Config.adminGroupName;
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');               

                //Step 1 - Login as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                //Configure US modality toolbar following the steps for CR; choose different tools.
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(SuperAdminGroup);
                domain.SelectDomain(SuperAdminGroup);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                SelectElement Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("US");

                //Drag some tools, for ex: Magnifier
                //,Zoom,Rotate ClockWise, Rotate Counterclockwise to the toolbox configuration.Then
                // Click on Save button
                IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                IWebElement group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)"));
                var dictionary = new Dictionary<String, IWebElement>();
                String Series_Scope = BluRingViewer.GetToolName(BluRingTools.Series_Scope);
                String Save_Series = BluRingViewer.GetToolName(BluRingTools.Save_Series);
                dictionary.Add(Series_Scope, group1);
                dictionary.Add(Save_Series, group2);
                bool step_1 = domain.AddToolsToToolbox(dictionary, "US", true);
                bool step1 = domain.GetAvailableToolsInToolBoxConfig().Contains(Series_Scope) || domain.GetAvailableToolsInToolBoxConfig().Contains(Save_Series);
                bool step1_1 = domain.GetConfiguredToolsInToolBoxConfig().Contains(Series_Scope) && domain.GetConfiguredToolsInToolBoxConfig().Contains(Save_Series);

                if (step_1 && !step1 && step1_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step2 - Configure NM modality toolbar following the steps for CR; choose different tools.
                Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("NM");
                var groupsInUse = domain.GetGroupsInToolBoxConfig();
                bool stepG2 = groupsInUse.Count() == 12;
                bool stepG2_2 = domain.VerifyConfiguredToolsInToolBoxConfig(DefaultTools.Split(':'));

                //Drag some tools ex: lnteractive Zoom, Annotation Ortholine, CobbAngle to the toolbox configuration.Then               
                group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)"));
                dictionary = new Dictionary<String, IWebElement>();
                String ImageScope = BluRingViewer.GetToolName(BluRingTools.Image_Scope);
                dictionary.Add(Series_Scope, group1);
                dictionary.Add(ImageScope, group2);
                bool step_2 = domain.AddToolsToToolbox(dictionary, "NM", true);
                bool step2 = domain.GetAvailableToolsInToolBoxConfig().Contains(Series_Scope) || domain.GetAvailableToolsInToolBoxConfig().Contains(ImageScope);
                bool step2_1 = domain.GetConfiguredToolsInToolBoxConfig().Contains(Series_Scope) && domain.GetConfiguredToolsInToolBoxConfig().Contains(ImageScope);

                if (stepG2 && stepG2_2 && step_2 && !step2 && step2_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step3 - Configure MG modality toolbar following the steps for CR; choose different tools.
                Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("MG");
                groupsInUse = domain.GetGroupsInToolBoxConfig();
                bool stepG3 = groupsInUse.Count() == 12;
                bool stepG3_2 = domain.VerifyConfiguredToolsInToolBoxConfig(DefaultTools.Split(':'));

                //Drag some tools ex: lnteractive Zoom, Annotation Ortholine, CobbAngle to the toolbox configuration.Then                
                group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)"));
                dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add(Series_Scope, group1);
                dictionary.Add(Save_Series, group2);
                bool step_3 = domain.AddToolsToToolbox(dictionary, "MG", true);
                bool step3 = domain.GetAvailableToolsInToolBoxConfig().Contains(Series_Scope) || domain.GetAvailableToolsInToolBoxConfig().Contains(Save_Series);
                bool step3_1 = domain.GetConfiguredToolsInToolBoxConfig().Contains(Series_Scope) && domain.GetConfiguredToolsInToolBoxConfig().Contains(Save_Series);

                if (stepG3 && stepG3_2 && step_3 && !step3 && step3_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step4 - Click on Save button
                domain.ClickSaveEditDomain(); //Step-26 as per testcase
                ExecutedSteps++;

                //Step5 - Go back to studylist, Load a study which contains series of different modality. (US,NM,MG)                                
                var studies = (Studies)login.Navigate("Studies");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.ClearSearchBtn()));
                studies.SearchStudy(AccessionNo: Accession[0]);
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step6 - Select US modality series. Right click the mouse button and verify that the user shall open the toolbox.
                viewer.ChangeViewerLayout("3x2", viewport: 5);
                viewer.SetViewPort(4, 1);
                viewer.OpenViewerToolsPOPUp();               
                String[] Tools = DefaultTools.Split(':');
                Tools[0] = Tools[0] + "," + Series_Scope;
                Tools[1] = Tools[1] + "," + Save_Series;
                var toolsInViewer = viewer.GetGroupsInToolBox(5, 1);
                bool step6_1 = toolsInViewer.Count() == 12;
                bool step6_2 = viewer.VerifyConfiguredTools(Tools, 1, 5);
                if (step6_1 && step6_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step7 - Select any tool from the floating toolbox and apply it.
                viewer.SelectViewerTool(BluRingTools.Line_Measurement, 1, 5, isOpenToolsPOPup: false);
                viewer.ApplyTool_LineMeasurement();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step7 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step7)
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

                //Step8 - Select NM modality series. Right click the mouse button and verify that the user shall open the toolbox
                viewer.SetViewPort(3, 1);
                viewer.OpenViewerToolsPOPUp();              
                Tools = DefaultTools.Split(':');
                Tools[0] = Tools[0] + "," + Series_Scope;
                Tools[1] = Tools[1] + "," + ImageScope;
                var toolsInViewer8 = viewer.GetGroupsInToolBox(4, 1);
                bool step8_1 = toolsInViewer8.Count() == 12;
                bool step8_2 = viewer.VerifyConfiguredTools(Tools, 1, 4);
                if (step8_1 && step8_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step9 - Select any tool from the floating toolbox and apply it.
                viewer.SelectViewerTool(BluRingTools.Line_Measurement, 1, 4, isOpenToolsPOPup: false);
                viewer.ApplyTool_LineMeasurement();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step9 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step9)
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

                //Step10 - Select MG modality series. Right click the mouse button and verify that the user shall open the toolbox
                viewer.SetViewPort(1, 1);
                viewer.OpenViewerToolsPOPUp();
                var toolsInViewer10 = viewer.GetGroupsInToolBox(2, 1);
                Tools = DefaultTools.Split(':');
                Tools[0] = Tools[0] + "," + Series_Scope;
                Tools[1] = Tools[1] + "," + Save_Series;
                bool step10_1 = toolsInViewer10.Count() == 12;
                bool step10_2 = viewer.VerifyConfiguredTools(Tools,1,2);
                if (step10_1 && step10_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step11 - 	Select any tool from the floating toolbox and apply it.
                viewer.SelectViewerTool(BluRingTools.Line_Measurement, 1, 2, isOpenToolsPOPup: false);
                viewer.ApplyTool_LineMeasurement();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step11 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step11)
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

                //Step12 - Close the study
                viewer.CloseBluRingViewer();//Step-34 as per testcase
                ExecutedSteps++;

                //Step13 - Edit the Domain Management Page for administrator account.                
                //Configure US modality toolbar following the steps for CR; choose different tools.
                //Change the modality tools setting for US modality
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(SuperAdminGroup);
                domain.SelectDomain(SuperAdminGroup);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("US");
                var ToolsToBeRemoved = new List<String>();
                String Pan = BluRingViewer.GetToolName(BluRingTools.Pan);
                String GetPixelValue = BluRingViewer.GetToolName(BluRingTools.Get_Pixel_Value);
                ToolsToBeRemoved.Add(Pan);
                ToolsToBeRemoved.Add(GetPixelValue);
                domain.RemoveToolsFromConfiguredSection(ToolsToBeRemoved, "US");
                bool step13_1 = domain.GetConfiguredToolsInToolBoxConfig().Contains(Pan) ||
                                    domain.GetConfiguredToolsInToolBoxConfig().Contains(GetPixelValue);
                bool step13_2 = domain.GetAvailableToolsInToolBoxConfig().Contains(Pan) &&
                                domain.GetAvailableToolsInToolBoxConfig().Contains(GetPixelValue);

                if (!step13_1 && step13_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step14 - Configure NM modality toolbar following the steps for CR; choose different tools.               
                ToolsToBeRemoved = new List<String>();
                ToolsToBeRemoved.Add(Pan);
                ToolsToBeRemoved.Add(GetPixelValue);
                domain.RemoveToolsFromConfiguredSection(ToolsToBeRemoved, "NM");
                bool step14_1 = domain.GetConfiguredToolsInToolBoxConfig().Contains(Pan) ||
                                    domain.GetConfiguredToolsInToolBoxConfig().Contains(GetPixelValue);
                bool step14_2 = domain.GetAvailableToolsInToolBoxConfig().Contains(Pan) &&
                                domain.GetAvailableToolsInToolBoxConfig().Contains(GetPixelValue);
                if (!step14_1 && step14_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step15 - Configure MG modality toolbar following the steps for CR; choose different tools.
                domain.RemoveToolsFromConfiguredSection(ToolsToBeRemoved, "MG");
                bool step15_1 = domain.GetConfiguredToolsInToolBoxConfig().Contains(Pan) ||
                                    domain.GetConfiguredToolsInToolBoxConfig().Contains(GetPixelValue);
                bool step15_2 = domain.GetAvailableToolsInToolBoxConfig().Contains(Pan) &&
                                domain.GetAvailableToolsInToolBoxConfig().Contains(GetPixelValue);

                if (!step15_1 && step15_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step16 - 	Click Save button.
                domain.ClickSaveEditDomain();//Step-38 as per testcase
                ExecutedSteps++;

                //Step17 - Go back to studylist, Load a study which contains series of different modality. (US,NM,MG)
                studies = (Studies)login.Navigate("Studies");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.ClearSearchBtn()));
                studies.SearchStudy(AccessionNo: Accession[0]);
                studies.SelectStudy("Accession", Accession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step18 - Select US modality series and right click the mouse button and verify that the user shall open the toolbox
                viewer.ChangeViewerLayout("3x2", viewport: 5);
                viewer.SetViewPort(4, 1);
                viewer.OpenViewerToolsPOPUp();            

                Tools = DefaultTools.Split(':');
                Tools[0] = Tools[0] + "," + Series_Scope;
                Tools[1] = Tools[1] + "," + Save_Series;
                Tools[2] = "";
                Tools[9] = "";
                var toolsInViewer18 = viewer.GetGroupsInToolBox(5, 1);
                bool step18_1 = toolsInViewer18.Count() == 12;
                bool step18_2 = viewer.VerifyConfiguredTools(Tools.ToArray(), 1, 5);
                if (step18_1 && step18_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step19 - Select any tool from the modality toolbar and apply it.
                viewer.SelectViewerTool(BluRingTools.Line_Measurement, 1, 5, isOpenToolsPOPup: false);
                viewer.ApplyTool_LineMeasurement();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step19 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step19)
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


                //Step20 - Select NM modality series and right click the mouse button and verify that the user shall open the toolbox.
                String[] ToolsNM = DefaultTools.Split(':');
                ToolsNM[0] = ToolsNM[0] + "," + Series_Scope;
                ToolsNM[1] = ToolsNM[1] + "," + ImageScope;
                ToolsNM[2] = "";
                ToolsNM[9] = "";
                viewer.SetViewPort(3, 1);
                viewer.OpenViewerToolsPOPUp();
                var toolsInViewer20 = viewer.GetGroupsInToolBox(4, 1);
                bool step20_1 = toolsInViewer20.Count() == 12;
                bool step20_2 = viewer.VerifyConfiguredTools(ToolsNM.ToArray(), 1, 4);
                if (step20_1 && step20_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step21 - Select any tool from the modality toolbar and apply it.
                viewer.SelectViewerTool(BluRingTools.Line_Measurement, 1, 4, isOpenToolsPOPup: false);
                viewer.ApplyTool_LineMeasurement();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step21 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step21)
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

                //Step22 - Select MG modality series and right click the mouse button and verify that the user shall open the toolbox
                viewer.SetViewPort(1, 1);
                viewer.OpenViewerToolsPOPUp();
                var toolsInViewer22 = viewer.GetGroupsInToolBox(2, 1);
                bool step22_1 = toolsInViewer22.Count() == 12;
                bool step22_2 = viewer.VerifyConfiguredTools(Tools.ToArray(), 1, 2);
                if (step22_1 && step22_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step23 - 	Select any tool from the modality toolbar and apply it.
                viewer.SelectViewerTool(BluRingTools.Line_Measurement, 1, 2, isOpenToolsPOPup: false);
                viewer.ApplyTool_LineMeasurement();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step23 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step23)
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

                //Logout Application
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
            finally
            {
                DomainManagement domain = new DomainManagement();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                SelectElement Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("US");
                Thread.Sleep(1000);
                IWebElement revertButton = BasePage.Driver.FindElement(By.CssSelector(DomainManagement.btn_revertToDefault));
                if (revertButton.Enabled)
                    revertButton.Click();
                Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("NM");
                Thread.Sleep(1000);
                if (revertButton.Enabled)
                    revertButton.Click();
                Modality = new SelectElement(BasePage.Driver.FindElement
                                               (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("MG");
                Thread.Sleep(1000);
                if (revertButton.Enabled)
                    revertButton.Click();
                domain.ClickSaveEditDomain();
                login.Logout();
            }
        }

        /// <summary>
        /// Toolbox configuration for default modality tool
        /// </summary>
        public TestCaseResult Test_161535(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                DomainManagement domain = new DomainManagement();
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;               
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');
                String[] AddNewTool = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "AddNewTools")).Split(',');
                String TestDomain = "Domain_tb_533_" + new Random().Next(1, 10000);
                String Role = "Role_tb_533_" + new Random().Next(1, 10000);
                String DomainAdmin = "DomainAdmin_tb_533_" + new Random().Next(1, 10000);


                //Step 1 - Login as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                if (login.IsTabPresent("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step2 - In Domain Management -> Edit Domain page -> Toolbox Configuration section
                login.NavigateToDomainManagementTab();
                domain.CreateDomain(TestDomain, TestDomain, TestDomain, DomainAdmin, null, DomainAdmin, DomainAdmin, DomainAdmin, Role, Role);
                domain.SearchDomain(TestDomain);
                domain.SelectDomain(TestDomain);
                domain.ClickEditDomain();
                ExecutedSteps++;

                //Step3 - By default, Default option should be displayed in the Modality Dropdown
                SelectElement Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                if (Modality.SelectedOption.Text.Equals("default"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step4 - Ensure that default tools are displayed in config toolbox.
                var groupsInUse = basepage.GetGroupsInToolBoxConfig();
                bool step4_1 = groupsInUse.Count() == 12;
                bool step4_2 = basepage.VerifyConfiguredToolsInToolBoxConfig(DefaultTools.Split(':'));
                if (step4_1 && step4_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step5 - Select any modality (eg. CT) from the Modality drop down and verify default tools are displayed in the Toolbox Configuration section.
                IWebElement ModalityElement = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.select_toolBoxConfiguration_ModalityDropdown);
                basepage.SelectFromList(ModalityElement, "CT");
                Thread.Sleep(1000);
                groupsInUse = basepage.GetGroupsInToolBoxConfig();
                bool step5_1 = groupsInUse.Count() == 12;
                bool step5_2 = basepage.VerifyConfiguredToolsInToolBoxConfig(DefaultTools.Split(':'));
                if (step5_1 && step5_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step6 - Select again "default" option from the Modality dropdown and change any tool position as desired or drag any tool from Available Item box 
                //and drop the dragged tool(eg. Reset tool) into the cells in the toolbox
                basepage.SelectFromList(ModalityElement, "default");
                Thread.Sleep(1000);
                IWebElement group1 = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)");
                var dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add(AddNewTool[1], group1);
                bool step6_1 = basepage.AddToolsToToolbox(dictionary, addToolAtEnd:true);
                bool step6_2 = !basepage.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[1]);
                bool step6_3 = basepage.GetToolsInGroupInToolBoxConfig(group1).Contains(AddNewTool[1]);
                if (step6_1 && step6_2 && step6_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step7 - Without clicking save button select previously selected modality (eg. CT) from the Modality: drop down and verify that
                // the new default settings are dynamically updated to the selected modality.
                basepage.SelectFromList(ModalityElement, "CT");
                Thread.Sleep(1000);
                group1 = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)");
                bool step7_1 = !basepage.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[1]);
                bool step7_2 = basepage.GetToolsInGroupInToolBoxConfig(group1).Contains(AddNewTool[1]);
                if (step7_1 && step7_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step8 - Click on 'Close' button
                domain.ClickCloseEditDomain();
                ExecutedSteps++;   // Changes saved or not is verified in next 2 steps

                //Step9 - Again go to the Superadmin Edit domain management page and then go to toolbox configuration section 
                // and verify that the previously configured tools should not be reflected to the default modality.
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(TestDomain);
                domain.SelectDomain(TestDomain);
                domain.ClickEditDomain();
                groupsInUse = basepage.GetGroupsInToolBoxConfig();
                bool step9_1 = groupsInUse.Count() == 12;
                bool step9_2 = basepage.VerifyConfiguredToolsInToolBoxConfig(DefaultTools.Split(':'));
                if (step9_1 && step9_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step10 - Select previous used modality(i.e. CT) from Modality drop down 
                // and verify that the previously configured tools for default tools should not be reflected to the selected modality.
                ModalityElement = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.select_toolBoxConfiguration_ModalityDropdown);
                basepage.SelectFromList(ModalityElement, "CT");
                Thread.Sleep(1000);
                groupsInUse = basepage.GetGroupsInToolBoxConfig();
                bool step10_1 = groupsInUse.Count() == 12;
                bool step10_2 = basepage.VerifyConfiguredToolsInToolBoxConfig(DefaultTools.Split(':'));
                if (step10_1 && step10_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step11 - Select "default" from Modality drop down and configure some tools(Ex: Draw ROI,Magnifier,Window Level)
                basepage.SelectFromList(ModalityElement, "default");
                Thread.Sleep(1000);
                group1 = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)");
                IWebElement group2 = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)");
                IWebElement group3 = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(4)");
                dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add(AddNewTool[0], group1);
                dictionary.Add(AddNewTool[1], group2);
                dictionary.Add(AddNewTool[2], group3);
                bool step11_1 = basepage.AddToolsToToolbox(dictionary, addToolAtEnd:true);
                bool step11_2 = !basepage.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[0]);
                bool step11_3 = !basepage.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[1]);
                bool step11_4 = !basepage.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[2]);
                bool step11_5 = basepage.GetToolsInGroupInToolBoxConfig(group1).Contains(AddNewTool[0]);
                bool step11_6 = basepage.GetToolsInGroupInToolBoxConfig(group2).Contains(AddNewTool[1]);
                bool step11_7 = basepage.GetToolsInGroupInToolBoxConfig(group3).Contains(AddNewTool[2]);
                if (step11_1 && step11_2 && step11_3 && step11_4 && step11_5 && step11_6 && step11_7)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step12 - Click on 'Save' button.
                domain.ClickSaveEditDomain();
                ExecutedSteps++;

                //Step13 - Go to the Superadmin Edit domain management page and select previously used modality from Modality(Ex: CT) drop down 
                // and verify tools(Ex: Draw ROI,Magnifier,Window Level) which are configured for default modality toolbox should be displayed for the selected modality.
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(TestDomain);
                domain.SelectDomain(TestDomain);
                domain.ClickEditDomain();
                ModalityElement = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.select_toolBoxConfiguration_ModalityDropdown);
                basepage.SelectFromList(ModalityElement, "CT");
                Thread.Sleep(1000);
                group1 = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)");
                group2 = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)");
                group3 = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(4)");
                bool step12_1 = !basepage.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[0]);
                bool step12_2 = !basepage.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[1]);
                bool step12_3 = !basepage.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[2]);
                bool step12_4 = basepage.GetToolsInGroupInToolBoxConfig(group1).Contains(AddNewTool[0]);
                bool step12_5 = basepage.GetToolsInGroupInToolBoxConfig(group2).Contains(AddNewTool[1]);
                bool step12_6 = basepage.GetToolsInGroupInToolBoxConfig(group3).Contains(AddNewTool[2]);
                if (step12_1 && step12_2 && step12_3 && step12_4 && step12_5 && step12_6)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step14 - Click on 'Save' button.
                domain.ClickSaveEditDomain();
                ExecutedSteps++;

                //Step15 - 	Select 'default' modality and verify tools(Ex: Draw ROI, Magnifier, Window Level ) which are configured for default modality are displayed in config toolbox
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(TestDomain);
                domain.SelectDomain(TestDomain);
                domain.ClickEditDomain();
                ModalityElement = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.select_toolBoxConfiguration_ModalityDropdown);
                basepage.SelectFromList(ModalityElement, "default");
                Thread.Sleep(1000);
                group1 = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)");
                group2 = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)");
                group3 = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(4)");
                bool step15_1 = basepage.GetToolsInGroupInToolBoxConfig(group1).Contains(AddNewTool[0]);
                bool step15_2 = basepage.GetToolsInGroupInToolBoxConfig(group2).Contains(AddNewTool[1]);
                bool step15_3 = basepage.GetToolsInGroupInToolBoxConfig(group3).Contains(AddNewTool[2]);
                if (step12_1 && step12_2 && step12_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step16 - Select previously used modality (Ex: CT) from Modality: drop down and verify that the configured default tools(Ex: Draw ROI,Magnifier,Window Level) should be displayed for the selected modality.
                basepage.SelectFromList(ModalityElement, "CT");
                Thread.Sleep(1000);
                group1 = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)");
                group2 = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)");
                group3 = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(4)");
                bool step16_1 = basepage.GetToolsInGroupInToolBoxConfig(group1).Contains(AddNewTool[0]);
                bool step16_2 = basepage.GetToolsInGroupInToolBoxConfig(group2).Contains(AddNewTool[1]);
                bool step16_3 = basepage.GetToolsInGroupInToolBoxConfig(group3).Contains(AddNewTool[2]);
                if (step16_1 && step16_2 && step16_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step17  -  Configure MR modality toolbox choose different tools.(Ex: Draw Rectangle ,Draw Circle, Ellipse)
                basepage.SelectFromList(ModalityElement, "MR");
                Thread.Sleep(1000);
                IWebElement group5 = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(5)");           
                dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add(AddNewTool[3], group5);            
                bool step17_1 = basepage.AddToolsToToolbox(dictionary, "MR", true);
                bool step17_2 = !basepage.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[3]);               
                bool step17_4 = basepage.GetToolsInGroupInToolBoxConfig(group5).Contains(AddNewTool[3]);              
                if (step17_1 && step17_2 && step17_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step18 - Click on 'Save' button.
                domain.ClickSaveEditDomain();
                ExecutedSteps++;

                //Step19 - Navigate to Edit Domain Management page and select the previously used modality (EX: MR) and verify that the Modality toolbar for selected modality should be displayed.
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(TestDomain);
                domain.SelectDomain(TestDomain);
                domain.ClickEditDomain();
                ModalityElement = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.select_toolBoxConfiguration_ModalityDropdown);
                basepage.SelectFromList(ModalityElement, "MR");
                Thread.Sleep(1000);
                group5 = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(5)");             
                bool step19_1 = basepage.GetToolsInGroupInToolBoxConfig(group5).Contains(AddNewTool[3]);             
                if (step19_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step20 - Select "default" option from modality drop down and change any tool position as desired or drag any tool from Available Item box and 
                //    drop the dragged tool(eg. Reset tool) into the cells in the toolbox
                basepage.SelectFromList(ModalityElement, "default");
                Thread.Sleep(1000);
                group5 = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(5)");
                IWebElement group6 = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(10)");
                group1 = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)");
                group2 = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)");
                dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add(AddNewTool[0], group5);
                dictionary.Add(AddNewTool[1], group6);
                basepage.RepositionToolsInConfiguredToolsSection(dictionary, addToolAtEnd:true);
                bool step20_1 = !basepage.GetToolsInGroupInToolBoxConfig(group1).Contains(AddNewTool[0]);
                bool step20_2 = !basepage.GetToolsInGroupInToolBoxConfig(group2).Contains(AddNewTool[1]);
                bool step20_3 = basepage.GetToolsInGroupInToolBoxConfig(group5).Contains(AddNewTool[0]);
                bool step20_4 = basepage.GetToolsInGroupInToolBoxConfig(group6).Contains(AddNewTool[1]);
                if (step20_1 && step20_2 && step20_3 && step20_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step21 - Click on 'Save' button
                domain.ClickSaveEditDomain();
                ExecutedSteps++;

                //Step22 - Select previously configured MR modality from Modality drop down and ensure Tools which are configured for specified modality(MR) are listed in config toolbox                
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(TestDomain);
                domain.SelectDomain(TestDomain);
                domain.ClickEditDomain();
                ModalityElement = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.select_toolBoxConfiguration_ModalityDropdown);
                basepage.SelectFromList(ModalityElement, "MR");
                Thread.Sleep(1000);
                group5 = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(5)");
                group6 = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(10)");
                group1 = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)");
                group2 = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)");
                bool step22_1 = basepage.GetToolsInGroupInToolBoxConfig(group1).Contains(AddNewTool[0]);
                bool step22_2 = basepage.GetToolsInGroupInToolBoxConfig(group2).Contains(AddNewTool[1]);
                bool step22_3 = !basepage.GetToolsInGroupInToolBoxConfig(group5).Contains(AddNewTool[0]);
                bool step22_4 = !basepage.GetToolsInGroupInToolBoxConfig(group6).Contains(AddNewTool[1]);
                bool step22_5 = basepage.GetToolsInGroupInToolBoxConfig(group5).Contains(AddNewTool[3]);             
                if (step22_1 && step22_2 && step22_3 && step22_4 && step22_5)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step23 - Select any modality from modality(DX) drop down and verify that the tools which are configured for default modality should be listed in config toolbox.
                basepage.SelectFromList(ModalityElement, "DX");
                Thread.Sleep(1000);               
                String[] Tools = DefaultTools.Split(':');
                Tools[3] = Tools[3] + "," + AddNewTool[2];
                Tools[4] = Tools[4] + "," + AddNewTool[0];
                Tools[9] = Tools[9] + "," + AddNewTool[1];
                groupsInUse = basepage.GetGroupsInToolBoxConfig();
                bool step23_1 = groupsInUse.Count() == 12;
                bool step23_2 = basepage.VerifyConfiguredToolsInToolBoxConfig(Tools);
                if (step23_1 && step23_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout Application                
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
        /// Toolbox configuration for default modality tool
        /// </summary>
        public TestCaseResult Test_161536(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            DomainManagement domain = new DomainManagement();
            //String[] AddSuperDomainDefaultTools= { }, AddSuperDomainCTModalityTools= { };
            String SuperAdminDomainName = Config.adminGroupName;
            String TestDomain = "Domain_161536_" + new Random().Next(1, 10000);
            String adminUserName = Config.adminUserName;
            String adminPassword = Config.adminPassword;
            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String DataSource = login.GetHostName(Config.EA1);
                String[] AccessionList = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList")).Split(':');
                String[] PatientIDList = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID")).Split(':');
                String Role = "Role_161536_" + new Random().Next(1, 10000);
                String DomainAdmin = "DomainAdmin_161536_" + new Random().Next(1, 10000);
               // String[] AddNewTool = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "AddNewTools")).Split('=');
              //  AddSuperDomainDefaultTools = AddNewTool[0].Split(':'); // Reset:Series Scope
               // AddSuperDomainCTModalityTools = AddNewTool[1].Split(':'); // Image Scope
              // String[] AddNewDomainCTModalityTools = AddNewTool[2].Split(':'); // Free Draw:All in One

                //Step 1 - Login as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                if (login.IsTabPresent("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step2 - In Domain Management -> Edit Domain page -> Toolbox Configuration section
                login.NavigateToDomainManagementTab();
                domain.SearchDomain(SuperAdminDomainName);
                domain.SelectDomain(SuperAdminDomainName);
                domain.ClickEditDomain();
                ExecutedSteps++;

                //Step3 - By default, Default option should be displayed in the Modality Dropdown
                SelectElement Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                if (Modality.SelectedOption.Text.Equals("default"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step4 - Place any tool (eg:Reset) in stacked tool control by drag and dropping the tool from the Available Items into the cell in the toolbox at the desired position.
                IWebElement group1 = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)");
                var dictionary = new Dictionary<String, IWebElement>();
                string Tool1 = domain.GetAvailableToolsInToolBoxConfig()[0];
                string Tool2 = domain.GetAvailableToolsInToolBoxConfig()[1];
                dictionary.Add(Tool1, group1);
                dictionary.Add(Tool2, group1);
                bool step4_1 = basepage.AddToolsToToolbox(dictionary);
                bool step4_3 = basepage.GetToolsInGroupInToolBoxConfig(group1).Contains(Tool1 )&& basepage.GetToolsInGroupInToolBoxConfig(group1).Contains(Tool2);
                if (step4_1 && step4_3)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step5 - Select any modality (eg. CT) from the Modality drop down and verify default tools are displayed in the Toolbox Configuration section.
                IWebElement revertButton = BasePage.Driver.FindElement(By.CssSelector(DomainManagement.btn_revertToDefault));
                revertButton.Click();
                IWebElement ModalityElement = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.select_toolBoxConfiguration_ModalityDropdown);
                basepage.SelectFromList(ModalityElement, "CT");
                Thread.Sleep(10000);
                group1 = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)");
                var CTModalityTools = new Dictionary<String, IWebElement>();
                Tool1 = domain.GetAvailableToolsInToolBoxConfig()[0];
                CTModalityTools.Add(Tool1, group1);
                bool step5_1 = basepage.AddToolsToToolbox(CTModalityTools, Modalityname:"CT");
                bool step5_2 = !basepage.GetAvailableToolsInToolBoxConfig().Contains(Tool1);
                bool step5_3 = basepage.GetToolsInGroupInToolBoxConfig(group1).Contains(Tool1);
                IList<string> AlltoolsListInConfig_5 = domainmanagement.GetAllToolsInToolBoxConfig();
                IList<string> AllAvailableTools_5 = domainmanagement.GetAvailableToolsInToolBoxConfig();

                if (step5_1 && step5_2 && step5_3)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                domain.ClickSaveEditDomain();

                //Step6 - Go to New Domain Management page by clicking on Edit button.
                domain.CreateDomain(TestDomain, TestDomain, TestDomain, DomainAdmin, null, DomainAdmin,DomainAdmin, DomainAdmin, Role, Role);
                domain.SearchDomain(TestDomain);
                domain.SelectDomain(TestDomain);
                domain.ClickEditDomain();
                ExecutedSteps++;

                //Step7 - Select any modality (eg: CT) from the Modality dropdown and verify that the changes in default modality 
                //and CT modality toolbox on a SuperAdminGroup domain are not reflected to new domains.
                ModalityElement = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.select_toolBoxConfiguration_ModalityDropdown);
                basepage.SelectFromList(ModalityElement, "CT");
                Thread.Sleep(1000);
                group1 = basepage.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)");
                bool step7_1 = domainmanagement.GetAllToolsInToolBoxConfig().SequenceEqual(AlltoolsListInConfig_5);
                bool step7_2 = domainmanagement.GetAvailableToolsInToolBoxConfig().SequenceEqual(AllAvailableTools_5);
                bool step7_3 = domain.VerifyConfiguredToolsInToolBoxConfig(DefaultTools.Split(':'));
                if (!step7_1 && !step7_2 && step7_3)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }


                //Step8 - Drag some tools, for ex. Free draw, Reset , to the New box. Save the changes.
                var NewDomainCTModalityTools = new Dictionary<String, IWebElement>();
                Tool1 = domain.GetAvailableToolsInToolBoxConfig()[0];
                Tool2 = domain.GetAvailableToolsInToolBoxConfig()[1];
                NewDomainCTModalityTools.Add(Tool1, group1);
                NewDomainCTModalityTools.Add(Tool2, group1);
                bool step8_1 = basepage.AddToolsToToolbox(NewDomainCTModalityTools, Modalityname:"CT");
                Logger.Instance.InfoLog("Added the tool "+ Tool1 + "to the TooBox config");
                bool step8_2 = basepage.GetToolsInGroupInToolBoxConfig(group1).Contains(Tool1) && basepage.GetToolsInGroupInToolBoxConfig(group1).Contains(Tool2);
                bool step8_3 =!basepage.GetAvailableToolsInToolBoxConfig().Contains(Tool1);
                bool step8_4 = !basepage.GetAvailableToolsInToolBoxConfig().Contains(Tool2);
                IList<string> ToolsInDomainAfterEdit = new List<string>();
                foreach (IWebElement columnTools in domainmanagement.GetGroupsInToolBoxConfig())
                {
                    IList<string> toolsInEachColumn = domainmanagement.GetToolsInGroupInToolBoxConfig(columnTools);
                    ToolsInDomainAfterEdit.Add(String.Join(",", toolsInEachColumn));
                }
                if (step8_1 && step8_2 && step8_3 && step8_4)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
               
                domain.ClickSaveEditDomain();


                //Step9 - Logout as Administrator and Login as new domain user
                login.Logout();
                login.LoginIConnect(DomainAdmin, DomainAdmin);
                ExecutedSteps++;

                //Step10 - In studies tab,search and load a study which contains series of CT Modality and then click on 'View Exam ' button.
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientIDList[0], Modality: "CT", Datasource: DataSource);
                studies.SelectStudy("Accession", AccessionList[0]);
                BluRingViewer viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step11 - Select any series viewport and click on right mouse button
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step12 - Verify the configured tools appear in the floating toolbox for the CT Modality as specified in Domain Management page.               
               bool  step12_1 = viewer.GetToolsInToolBoxByGrid().SequenceEqual(ToolsInDomainAfterEdit) ;
                if (step12_1)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                // Step13 - Select any tool from the toolbox and apply it.
                viewer.SelectViewerTool(BluRingTools.Line_Measurement, isOpenToolsPOPup: false);
                viewer.ApplyTool_LineMeasurement();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step13 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step13)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                //Step14 - Close the Enterprise viewer
                viewer.CloseBluRingViewer();
                ExecutedSteps++;

                //Step15 - Load any study which contains series of CR Modality and then click on 'View Exam ' button
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientIDList[1], Modality: "CR", Datasource: DataSource);
                studies.SelectStudy("Accession", AccessionList[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step16 - Select any series viewport and click on right mouse button
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step17 - Verify the configured tools appear in the floating toolbox for the CR Modality as specified in Domain Management page.               
                bool step17_1 = viewer.GetToolsInToolBoxByGrid().SequenceEqual(DefaultTools.Split(':'));
                if (step17_1 )
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                viewer.CloseBluRingViewer();

                //Logout Application                
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
            finally
            {
                try
                {
                    login.DriverGoTo(login.url);
                    login.LoginIConnect(adminUserName, adminPassword);
                    login.NavigateToDomainManagementTab();
                    domain.SearchDomain(SuperAdminDomainName);
                    domain.SelectDomain(SuperAdminDomainName);
                    domain.ClickEditDomain();
                    SelectElement Modality = new SelectElement(BasePage.Driver.FindElement
                                                  (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                    IWebElement revertButton = BasePage.Driver.FindElement(By.CssSelector(DomainManagement.btn_revertToDefault));
                    if (revertButton.Enabled)
                        revertButton.Click();
                    Modality.SelectByText("CT");
                    Thread.Sleep(1000);
                    revertButton = BasePage.Driver.FindElement(By.CssSelector(DomainManagement.btn_revertToDefault));
                    if (revertButton.Enabled)
                        revertButton.Click();
                    domain.ClickSaveEditDomain();
                    login.Logout();
                }
                catch (Exception e)
                {
                    Logger.Instance.InfoLog("Exceptoin in Finally block");
                }
            }
        }

        /// <summary>
        /// Role configs inheriting from the domain settings
        /// </summary>
        public TestCaseResult Test_161512(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            String TestDomain = "TestDomain_" + new Random().Next(1, 1000);
            String Role = "RegularUserRole_" + new Random().Next(1, 1000);
            String DomainAdmin = "DomainAdmin_" + new Random().Next(1, 1000);
            String User1 = "User1_" + new Random().Next(1, 1000);

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
               
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');
                String[] AddNewTool = new String[5];     

                //Pre-conditions:
                //1.iCA  installed and data sources configured. 
                //2.Data source: at least one EA and one MPACS.
                //3.Create new : (if not created)
                //  1. Test Domain
                //  2. role (Regular User) under the Test Domain (so you will have the Test Domain Admin role and the Regular User role)
                //     domain admin (Administrator Test Domain)
                //  3. regular user (User1, belonging to Test Domain and with a Regular User
                //  4.Set default viewer as 'BluRing'              
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement.NavigateToDomainManagementTab();
                domainmanagement.CreateDomain(TestDomain, TestDomain, TestDomain, DomainAdmin, null, DomainAdmin, DomainAdmin, DomainAdmin, Role, Role);
                login.Navigate("UserManagement");
                usermanagement.CreateUser(User1, TestDomain, Role);
                login.Logout();

                // Step 1 - Log in as Test Domain Administrator.                     
                login.LoginIConnect(DomainAdmin, DomainAdmin);
                ExecutedSteps++;

                //Step 2 - Go to Domain Management page by clicking on Edit button.
                domainmanagement.NavigateToDomainManagementTab();
                if (login.IsTabSelected("Domain Management"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 3 - Verify the default tools should be displayed on viewer.
                bool step3 = domainmanagement.VerifyConfiguredToolsInToolBoxConfig(DefaultTools.Split(','));
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

                //Step 4 - Drag any tool from the Available Items and drop the dragged tool to the cell 1.
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                BasePage.Driver.SwitchTo().Frame("TabContent");
                IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                var dictionary = new Dictionary<String, IWebElement>();
                AddNewTool[0] = domainmanagement.GetAvailableToolsInToolBoxConfig().ToArray()[0];
                dictionary.Add(AddNewTool[0], group1);
                bool step4 = domainmanagement.AddToolsToToolbox(dictionary, addToolAtEnd:true);
                if (step4 &&
                    domainmanagement.GetConfiguredToolsInToolBoxConfig().Contains(AddNewTool[0]) &&
                    !domainmanagement.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[0]) && domainmanagement.GetToolsInGroupInToolBoxConfig(group1).Contains(AddNewTool[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 5 - Configure the tools in all the cells			
                var groups = domainmanagement.GetGroupsInToolBoxConfig();
                Dictionary<String, IWebElement> dictionary5;
                IWebElement revertButton;
                bool step8_3 = false;
                step8_3 = domainmanagement.AddToolsToEachColumnInGroupToolBox(5);
                //foreach (var ele in groups)
                //{
                //    step8_3 = true;
                //    dictionary5 = new Dictionary<String, IWebElement>();
                //    dictionary5.Add(domainmanagement.GetAvailableToolsInToolBoxConfig()[1], ele);
                //    if (!domainmanagement.AddToolsToToolbox(dictionary5))
                //    {
                //        step8_3 = false;
                //        break;
                //    }
                //    revertButton = BasePage.Driver.FindElement(By.CssSelector(DomainManagement.btn_revertToDefault));
                //    if (revertButton.Enabled)
                //        revertButton.Click();
                //}
                revertButton = BasePage.Driver.FindElement(By.CssSelector(DomainManagement.btn_revertToDefault));
                if (revertButton.Enabled)
                    revertButton.Click();
                dictionary5 = new Dictionary<String, IWebElement>();
                string tool = domainmanagement.GetAvailableToolsInToolBoxConfig()[0];
                dictionary5.Add(tool, groups.ElementAt(2));
                bool step8_1 = domainmanagement.AddToolsToToolbox(dictionary5);
                bool step8_2 = domainmanagement.GetAvailableToolsInToolBoxConfig().Contains(tool);
                if (step8_1 && !step8_2 && step8_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                domainmanagement.ClickSaveEditDomain();

                //Step 6 - Verify the user able to drag the tool from the Available Items section and drop into the cells in the toolbox under toolbox configuration.                
                var revertToDefaultButton = BasePage.Driver.FindElement(By.CssSelector(DomainManagement.btn_revertToDefault));
                revertToDefaultButton.Click();

                IWebElement group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)"));
                IWebElement group3 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(11)"));
                dictionary = new Dictionary<String, IWebElement>();
                AddNewTool[1] = domainmanagement.GetAvailableToolsInToolBoxConfig()[0];
                AddNewTool[2] = domainmanagement.GetAvailableToolsInToolBoxConfig()[1];
                dictionary.Add(AddNewTool[1], group2);
                dictionary.Add(AddNewTool[2], group3);
                bool step6 = domainmanagement.AddToolsToToolbox(dictionary);
                if (step6 &&
                    domainmanagement.GetConfiguredToolsInToolBoxConfig().Contains(AddNewTool[1]) &&
                    !domainmanagement.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[1]) &&
                    domainmanagement.GetConfiguredToolsInToolBoxConfig().Contains(AddNewTool[2]) &&
                    !domainmanagement.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[2]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Setp 7 - Verify the user should able to configure the tool in last sot (i.e. Cell 12
                IWebElement group12 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(12)"));
                dictionary = new Dictionary<String, IWebElement>();
                AddNewTool[3] = domainmanagement.GetAvailableToolsInToolBoxConfig()[0];
                dictionary.Add(AddNewTool[3], group12);
                bool step7 = domainmanagement.AddToolsToToolbox(dictionary);
                if (step7 &&
                    domainmanagement.GetConfiguredToolsInToolBoxConfig().Contains(AddNewTool[3]) &&
                    !domainmanagement.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[3]) &&
                      domainmanagement.GetToolsInGroupInToolBoxConfig(group12).Contains(AddNewTool[3]) )
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 8 - Drag some tools to Available Items section.
                revertToDefaultButton = BasePage.Driver.FindElement(By.CssSelector(DomainManagement.btn_revertToDefault));
                revertToDefaultButton.Click();
                group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                AddNewTool[2] = domainmanagement.GetToolsInGroupInToolBoxConfig(group1)[0];
                var ToolsToBeRemoved = new List<String>();
                ToolsToBeRemoved.Add(AddNewTool[2]);
                domainmanagement.RemoveToolsFromConfiguredSection(ToolsToBeRemoved);
                Thread.Sleep(3000);
                if (!domainmanagement.GetConfiguredToolsInToolBoxConfig().Contains(AddNewTool[2]) &&
                    domainmanagement.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[2]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 9 - Click on "Save" button.
                domainmanagement.ClickSaveEditDomain();
                ExecutedSteps++;

                //Step 10 - Go back to Role Management page and open to edit the role page for Regular User role.
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SearchRole(Role);
                rolemanagement.SelectRole(Role);
                rolemanagement.ClickEditRole();
                ExecutedSteps++;

                //Step 11 - Modify the Toolbox Configuration for review toolbar. Save the changes.
                ToolsToBeRemoved = new List<String>();
                group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                AddNewTool[3] = domainmanagement.GetToolsInGroupInToolBoxConfig(group1)[0];
                ToolsToBeRemoved.Add(AddNewTool[3]);
                rolemanagement.RemoveToolsFromConfiguredSection(ToolsToBeRemoved);
                Thread.Sleep(3000);
                if (!rolemanagement.GetConfiguredToolsInToolBoxConfig().Contains(AddNewTool[3]) &&
                     rolemanagement.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[3]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                IList<string> ToolsInRole = new List<string>();
                foreach (IWebElement columnTools in domainmanagement.GetGroupsInToolBoxConfig())
                {
                    IList<string> toolsInEachColumn = domainmanagement.GetToolsInGroupInToolBoxConfig(columnTools);
                    ToolsInRole.Add(String.Join(",", toolsInEachColumn));
                }

                rolemanagement.ClickSaveRole();
                //wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[id$='_SaveButton']")));

                //Step 12 - Log out as Test Domain Administrator
                login.Logout();
                ExecutedSteps++;

                //Step 13 - Log in as User1.
                login.LoginIConnect(User1, User1);
                ExecutedSteps++;

                //Step 14 - Navigate to Studies tab.
                var studies = (Studies)login.Navigate("Studies");
                //wait.Until(ExpectedConditions.ElementToBeClickable(studies.ClearSearchBtn()));
                ExecutedSteps++;

                //Step 15 - Search and select a study and then click on "View Exam" button.
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step 16 - Click Right Mouse Button on viewport and verify that the user shall open the toolbox
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 17 - Verify the tools available in the floating toolbox               
                bool step17_1 = viewer.VerifyConfiguredTools(ToolsInRole.ToArray());
                if (step17_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.ErrorLog("ToolsinRole" + (string.Join(",", ToolsInRole.ToArray())));
                    Logger.Instance.ErrorLog("ToolInViewer"+ (string.Join(",", viewer.GetToolsInToolBoxByGrid().ToArray())) );
                   
                }


                //Step 18 - Select any tool from the floating toolbox (e.g. Invert or Interactive WW/WL) and 
                //          verify that the floating toolbox disappears when the user selects any tool from the toolbox.
                viewer.SelectViewerTool(BluRingTools.Get_Pixel_Value, isOpenToolsPOPup: false);
                Thread.Sleep(5000);
                //BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(BluRingViewer.div_toolboxContainer)));
                if (!viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 19 - User selected tool action should be applied to the viewport.                
                viewer.ApplyTool_PixelValue();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step20 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step20)
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

                //Step 20 - Logout as User1 and login as Test Domain Administrator
                viewer.CloseBluRingViewer();
                login.Logout();
                Thread.Sleep(2000);
                BasePage.Driver.SwitchTo().DefaultContent();
                login.LoginIConnect(DomainAdmin, DomainAdmin);
                studies = (Studies)login.Navigate("Studies");
                if (login.IsTabSelected("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 21 - Navigate to Edit domain management page and modify the toolbox configuration. Save the changes.
                domainmanagement.NavigateToDomainManagementTab();
                IWebElement group4 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(4)"));
                dictionary = new Dictionary<String, IWebElement>();
                AddNewTool[2] = basepage.GetAvailableToolsInToolBoxConfig()[0];
                dictionary.Add(AddNewTool[2], group4);
                bool step22 = domainmanagement.AddToolsToToolbox(dictionary, addToolAtEnd:true);
                if (step22 &&
                    domainmanagement.GetConfiguredToolsInToolBoxConfig().Contains(AddNewTool[2]) &&
                    !domainmanagement.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[2])
                    && domainmanagement.GetToolsInGroupInToolBoxConfig(group4).Contains(AddNewTool[2]) )
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                IList<string> DomainTools = domainmanagement.GetConfiguredToolsInToolBoxConfig();

                domainmanagement.ClickSaveEditDomain();


                //Step 22 - Navigate to Edit Role management page and verify that the role configs should not inherit the new domain settings.
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SearchRole(Role);
                rolemanagement.SelectRole(Role);
                rolemanagement.ClickEditRole();
                if (!rolemanagement.GetConfiguredToolsInToolBoxConfig().Contains(AddNewTool[2]) &&
                    rolemanagement.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[2]) &&
                    !rolemanagement.GetConfiguredToolsInToolBoxConfig().SequenceEqual(DomainTools) )
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.ErrorLog("DomainTools" + (string.Join(",", DomainTools.ToArray())));
                    Logger.Instance.ErrorLog("RoleTools" + (string.Join(",", rolemanagement.GetConfiguredToolsInToolBoxConfig().ToArray())));
                }

                //Step 23 -  Click on 'Revert to Default' button
                revertToDefaultButton = BasePage.Driver.FindElement(By.CssSelector(DomainManagement.btn_revertToDefault));
                revertToDefaultButton.Click();
                Thread.Sleep(4000);
                bool step24_1 = rolemanagement.GetConfiguredToolsInToolBoxConfig().Contains(AddNewTool[2]);
                bool step24_2 = rolemanagement.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[2]);
                if (step24_1 && !step24_2 && rolemanagement.GetConfiguredToolsInToolBoxConfig().SequenceEqual(DomainTools))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout Application				
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
        /// Drag all tools to Available Items section
        /// </summary>
        public TestCaseResult Test_161513(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            String TestDomain = "TestDomain_" + new Random().Next(1, 1000);
            String Role = "RegularUserRole_" + new Random().Next(1, 1000);
            String DomainAdmin = "DomainAdmin_" + new Random().Next(1, 1000);
            String User1 = "User1_" + new Random().Next(1, 1000);

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');
                String[] AddNewTool = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "AddNewTools")).Split(',');

                //Pre-conditions:
                //1.iCA  installed and data sources configured. 
                //2.Data source: at least one EA and one MPACS.
                //3.Create new : (if not created)
                //  1. Test Domain
                //  2. role (Regular User) under the Test Domain (so you will have the Test Domain Admin role and the Regular User role)
                //     domain admin (Administrator Test Domain)
                //  3. regular user (User1, belonging to Test Domain and with a Regular User
                //  4.Set default viewer as 'BluRing'              
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement.NavigateToDomainManagementTab();
                domainmanagement.CreateDomain(TestDomain, TestDomain, TestDomain, DomainAdmin, null, DomainAdmin, DomainAdmin, DomainAdmin, Role, Role);
                login.Navigate("UserManagement");
                usermanagement.CreateUser(User1, TestDomain, Role);
                login.Logout();

                // Step 1 - Log in as Test Domain Administrator.
                login.LoginIConnect(DomainAdmin, DomainAdmin);
                ExecutedSteps++;

                //Step 2 - Go to Domain Management page by clicking on Edit button and 
                //         Modify the Toolbox Configuration for floating toolbox and then click on "Save" button.
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");                
                PageLoadWait.WaitForFrameLoad(10);
                IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                var dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add( domainmanagement.GetAvailableToolsInToolBoxConfig()[0] , group1);
                bool step4 = domainmanagement.AddToolsToToolbox(dictionary, addToolAtEnd:true);
                if (step4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                domainmanagement.ClickSaveEditDomain();

                //Step 3 - Go to Role Management page and open edit role page for Regular User role. 		                
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SearchRole(Role);
                rolemanagement.SelectRole(Role);
                rolemanagement.ClickEditRole();
                ExecutedSteps++;

                //Step 4 - Verify the default tools should be displayed on viewer.
                var element = BasePage.FindElementByCss(BasePage.div_toolBoxConfiguration_Groups);
                Actions actions = new Actions(BasePage.Driver);
                actions.MoveToElement(element);
                actions.Perform();
                var groupsInUse = rolemanagement.GetGroupsInToolBoxConfig();
                bool step4_1 = groupsInUse.Count() == 12;
                bool step4_2 = rolemanagement.VerifyConfiguredToolsInToolBoxConfig(DefaultTools.Split(':'));
                if (step4_1 && step4_2)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 5 - Drag all tools to Available Items section. Save the changes..
                var tools = rolemanagement.GetConfiguredToolsInToolBoxConfig();          
                var ToolsToBeRemoved = new List<String>();
                for (int i = 0; i < tools.Count; i++)
                {
                    ToolsToBeRemoved.Add(tools[i]);
                }             
                rolemanagement.RemoveToolsFromConfiguredSection(ToolsToBeRemoved);
                rolemanagement.ClickSaveRole();
                //wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[id$='_SaveButton']")));
                ExecutedSteps++;

                //Step 6 - Log out as Test Domain Administrator
                login.Logout();
                ExecutedSteps++;

                //Step 7 - Log in as User1.
                login.LoginIConnect(User1, User1);
                ExecutedSteps++;

                //Step 8 - Navigate to Studies tab.
               // var studies = (Studies)login.Navigate("Studies");
                 Studies studies = new Studies();
                //wait.Until(ExpectedConditions.ElementToBeClickable(studies.ClearSearchBtn()));
                ExecutedSteps++;

                //Step 9 - Search and select a study and then click on "View Exam" button.                
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step 10 - Click Right Mouse Button on viewport and verify that the toolbox shall not get open
                viewer.OpenViewerToolsPOPUp();
                IWebElement Viewport = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step10 = studies.CompareImage(result.steps[ExecutedSteps], Viewport);
                if (step10)
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

                //Logout Application				
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
        /// At Role level: Configure tools to the specific modality toolbox
        /// </summary>
        public TestCaseResult Test_161521(String testid, String teststeps, int stepcount)
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
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');

                String TestDomain = "TestDomain_" + new Random().Next(1, 1000);
                String Role = "RegularUserRole_" + new Random().Next(1, 1000);
                String DomainAdmin = "DomainAdmin_" + new Random().Next(1, 1000);
                String User1 = "User1_" + new Random().Next(1, 1000);

                login.LoginIConnect(adminUserName, adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.CreateDomain(TestDomain, TestDomain, TestDomain, DomainAdmin, null, DomainAdmin, DomainAdmin, DomainAdmin, Role, Role);
                login.Logout();

                //Step 1 - Login to iCA application as Administrator or new domian
                login.DriverGoTo(login.url);
                login.LoginIConnect(DomainAdmin, DomainAdmin);
                if (login.IsTabPresent("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 2 - In Domain Management -*^>^* Edit Domain page -*^>^* Toolbox Configuration section, select Modality as eg. CR.
                login.NavigateToDomainManagementTab();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                BasePage.Driver.SwitchTo().Frame("TabContent");
                SelectElement Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("CR");
                ExecutedSteps++;

                //Step 3 - Drag some tools, for ex. Calibration, Pixel Value, Line Measurement to the toolbox configuration. Save the changes.
                IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                IWebElement group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)"));
                var dictionary = new Dictionary<String, IWebElement>();
                string tool1 = domainmanagement.GetAvailableToolsInToolBoxConfig()[1];
                string tool2 = domainmanagement.GetAvailableToolsInToolBoxConfig()[2];
                //String FreeDraw = BluRingViewer.GetToolName(BluRingTools.Free_Draw);
                //String Reset = BluRingViewer.GetToolName(BluRingTools.Reset);
                dictionary.Add(tool1, group1);
                dictionary.Add(tool2, group2);
                bool step3_1 = domainmanagement.AddToolsToToolbox(dictionary, "CR");
                bool step3_2 = domainmanagement.GetAvailableToolsInToolBoxConfig().Contains(tool1) && domainmanagement.GetAvailableToolsInToolBoxConfig().Contains(tool2);
                bool step3_3 = domainmanagement.GetToolsInGroupInToolBoxConfig(group1).Contains(tool1) && domainmanagement.GetToolsInGroupInToolBoxConfig(group2).Contains(tool2);
                if (step3_1 && !step3_2 && step3_3)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                IList<string> ToolsInDomain = basepage.GetConfiguredToolsInToolBoxConfig();
                IList<String> ToolsListInDomainForViewer = basepage.GetToolsInToolBoxConfigByEachColumn(); 

                domainmanagement.ClickSaveEditDomain();

                //Step 4 - In Role Management -*^>^* Edit SuperRole page -*^>^* Toolbox Configuration section, 
                //         select Modality as CR and make sure the toolbox configs are always inherited from the domain settings by default.
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectRole(Role);
                rolemanagement.ClickEditRole();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("CR");

                var groupsInUse = rolemanagement.GetGroupsInToolBoxConfig();
                bool step4_1 = groupsInUse.Count() == 12;
                bool step4_2 = rolemanagement.GetConfiguredToolsInToolBoxConfig().SequenceEqual(ToolsInDomain);
                if (step4_1 && step4_2)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 5 - Drag some tools, for ex. Pixel Value, Line Measurement,Angle Measurement to the toolbox configuration. Save the changes.
                IWebElement group4 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(4)"));
                IWebElement group5 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(5)"));
                dictionary = new Dictionary<String, IWebElement>();
                tool1 = rolemanagement.GetAvailableToolsInToolBoxConfig()[0];
                tool2 = rolemanagement.GetAvailableToolsInToolBoxConfig()[1];
                dictionary.Add(tool1, group4);
                dictionary.Add(tool2, group5);
                bool step5_1 = rolemanagement.AddToolsToToolbox(dictionary, "CR");
                bool step5_2 = rolemanagement.GetAvailableToolsInToolBoxConfig().Contains(tool1) &&
                               rolemanagement.GetAvailableToolsInToolBoxConfig().Contains(tool2);
                if (step5_1 && !step5_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                IList<string> ToolsInRole = basepage.GetToolsInToolBoxConfigByEachColumn();

                rolemanagement.ClickSaveEditRole();

                //Step 6 - Navigate to Studies tab, search and load a study which contains series of CR Modality and then click on 'View Exam' button
                var studies = (Studies)login.Navigate("Studies");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.ClearSearchBtn()));
                studies.SearchStudy(AccessionNo: Accession[0]);
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step 7 - Select any series viewport and click on right mouse button
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 8 - Verify the configured tools appear in the floating toolbox for the CR Modality as specified in Role Management page.                
                
                var toolsInViewer = viewer.GetGroupsInToolBox();
                bool step8_1 = toolsInViewer.Count() == 12;
                bool step8_2 = viewer.GetToolsInToolBoxByGrid().SequenceEqual(ToolsInRole);
                if (step8_1 && step8_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.ErrorLog("ToolsInViewer " + string.Join(",", viewer.GetToolsInToolBoxByGrid().ToArray()));
                    Logger.Instance.ErrorLog("ToolsInRole " + string.Join(",", ToolsInRole.ToArray()) );
                }

                //Step 9 - Verify the configured tools should not appear in the floating toolbox for the CR Modality as specified in Domain Management page.
                bool step9_1 = viewer.GetToolsInToolBoxByGrid().SequenceEqual(ToolsListInDomainForViewer);
                if (!step9_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 10 - Select any tool from the modality toolbox and apply it.
                viewer.SelectViewerTool(BluRingTools.Pan, isOpenToolsPOPup: false);
                viewer.ApplyTool_Pan();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step10 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step10)
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
                viewer.CloseBluRingViewer();

                //Step 11 - Edit the Domain Management Page for administrator account.
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");                     
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                BasePage.Driver.SwitchTo().Frame("TabContent");
                ExecutedSteps++;

                //Step 12 - Change the order of the tools by drag and drop them at the desired position.				                
                Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("CR");
                group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)"));
                group5 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(5)"));
                IWebElement group6 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(10)"));
                dictionary = new Dictionary<String, IWebElement>();
                string ToolsTogetAddedinColumn5 = basepage.GetToolsInGroupInToolBoxConfig(group1)[0];
                string ToolsTogetAddedinColumn6 = basepage.GetToolsInGroupInToolBoxConfig(group2)[0];
                dictionary.Add(ToolsTogetAddedinColumn5, group5);
                dictionary.Add(ToolsTogetAddedinColumn6, group6);
                domainmanagement.RepositionToolsInConfiguredToolsSection(dictionary, "CR");
                groupsInUse = domainmanagement.GetGroupsInToolBoxConfig();
                bool step12_1 = groupsInUse.Count() == 12;
                bool step12_2 = domainmanagement.GetToolsInGroupInToolBoxConfig(group5).Contains(ToolsTogetAddedinColumn5) && domainmanagement.GetToolsInGroupInToolBoxConfig(group6).Contains(ToolsTogetAddedinColumn6);
                if (step12_1 && step12_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 13 - Remove a tool from the New list, by selecting it then drag and drop it to the Available Items list.				
                var ToolsToBeRemoved = new List<String>();
                string toolsToRemove = basepage.GetConfiguredToolsInToolBoxConfig()[0];
                ToolsToBeRemoved.Add(toolsToRemove);
                domainmanagement.RemoveToolsFromConfiguredSection(ToolsToBeRemoved, "CR");
                Thread.Sleep(3000);
                if (!domainmanagement.GetConfiguredToolsInToolBoxConfig().Contains(toolsToRemove))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 14 - Add back the removed tool to the New list.				
                group4 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(4)"));
                dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add(toolsToRemove, group4);
                domainmanagement.AddToolsToToolbox(dictionary, "CR");
                Thread.Sleep(3000);
                if (domainmanagement.GetConfiguredToolsInToolBoxConfig().Contains(toolsToRemove))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                IList<string> ToolsInDomainAfterEdit = basepage.GetToolsInToolBoxConfigByEachColumn();

                domainmanagement.ClickSaveEditDomain();

                //Step 15 - Navigate to Studies tab,search and Load a study which contains series of CR Modality and then click on 'View Exam ' button
                studies = (Studies)login.Navigate("Studies");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.ClearSearchBtn()));
                studies.SearchStudy(AccessionNo: Accession[0]);
                studies.SelectStudy("Accession", Accession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step 16 - Select any series viewport and click on right mouse button
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 17 - Verify the Modality toolbox updates to reflect the configuration for the CR modality as specified in Domain Management page.               
                toolsInViewer = viewer.GetGroupsInToolBox();
                bool step17_1 = toolsInViewer.Count() == 12;
                bool step17_2 = viewer.GetToolsInToolBoxByGrid().SequenceEqual(ToolsInRole);
                if (step17_1 && step17_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step 18 - Select any tool from the modality toolbox and apply it.
                viewer.SelectViewerTool(BluRingTools.Pan, isOpenToolsPOPup: false);
                viewer.ApplyTool_Pan();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step18 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step18)
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

                //Logout Application
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
        /// 161517 - Superadmin user configures the toolbox for new domain user at Role level settings
        /// </summary>
        public TestCaseResult Test_161517(String testid, String teststeps, int stepcount)
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
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] DefaultTools = this.DefaultTools.Split(':');
                String TestDomain = "TestDomain_161517_" + new Random().Next(1, 1000);
                String Role = "RegularUserRole_161517_" + new Random().Next(1, 1000);
                String DomainAdmin = "DomainAdmin_161517_" + new Random().Next(1, 1000);
                String User1 = "User1_161517_" + new Random().Next(1, 1000);

                //Step 1
                login.LoginIConnect(adminUserName, adminPassword);
                result.steps[++ExecutedSteps].StepPass();

                //Step 2
                if (login.IsTabSelected("Domain Management"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 3
                DomainManagement domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.CreateDomain(TestDomain, TestDomain, TestDomain, DomainAdmin, null, DomainAdmin, DomainAdmin, DomainAdmin, Role, Role);
                result.steps[++ExecutedSteps].StepPass();

                //step 4
                login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(TestDomain);
                domainmanagement.SelectDomain(TestDomain);
                domainmanagement.ClickEditDomain();
                if (domainmanagement.PageHeaderLabel().Displayed)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 5
                if (domainmanagement.VerifyConfiguredToolsInToolBoxConfig(DefaultTools))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("Default tools should that be displayed on viewer are mismatch");
                }


                //step 6
                int i = 0;
                bool step6 = false;
                BasePage.FindElementByCss(DomainManagement.btn_revertToDefault).Click(); //Click on revert to default
                IList<string> availableToolsList = domainmanagement.GetAvailableToolsInToolBoxConfig();
                IList<IWebElement> columninToolBoxConfig = domainmanagement.GetGroupsInToolBoxConfig();
                Dictionary<string, IWebElement> dictionary = new Dictionary<string, IWebElement>();
                for (i = 0; i < availableToolsList.Count; i++)
                {
                    IWebElement columnTemp = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(" + (i + 1) + ")"));
                    Dictionary<string, IWebElement> dictionarytemp = new Dictionary<string, IWebElement>();
                    dictionarytemp.Add(availableToolsList[i], columnTemp);
                    domainmanagement.AddToolsToToolbox(dictionarytemp);
                    if (domainmanagement.GetToolsInGroupInToolBoxConfig(domainmanagement.GetGroupsInToolBoxConfig()[i]).Contains(availableToolsList[i]))
                    {
                        step6 = true;
                    }
                    else
                    {
                        step6 = false;
                        break;
                    }
                }
                if(step6)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("Tool " + availableToolsList[i] + "is not added to the toolbox under Toolbox Configuration.");
                }

                //step 7 - Place any tool in stacked tool control by drag and dropping the tool from the Available Items into the cell in the toolbox at the desired position.
                BasePage.FindElementByCss(DomainManagement.btn_revertToDefault).Click();
                dictionary = new Dictionary<String, IWebElement>();
                IList<string> ToolsListInFirstColumn = basepage.GetToolsInGroupInToolBoxConfig(basepage.GetGroupsInToolBoxConfig()[0]);
                availableToolsList = domainmanagement.GetAvailableToolsInToolBoxConfig();
                dictionary.Add(availableToolsList[0], basepage.GetGroupsInToolBoxConfig()[0]); //Add Item to First column
                domainmanagement.AddToolsToToolbox(dictionary);
                ToolsListInFirstColumn = basepage.GetToolsInGroupInToolBoxConfig(basepage.GetGroupsInToolBoxConfig()[0]);
                
                if (basepage.GetToolsInGroupInToolBoxConfig(basepage.GetGroupsInToolBoxConfig()[0]).Contains(availableToolsList[0]))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step8,9 - Place the 5 tools in each cell in the toolbox.
                BasePage.FindElementByCss(DomainManagement.btn_revertToDefault).Click();
                if (domainmanagement.AddToolsToEachColumnInGroupToolBox())
                {
                    result.steps[++ExecutedSteps].status = "Pass"; //Step 8
                    result.steps[++ExecutedSteps].status = "Pass"; //Step 9
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                BasePage.FindElementByCss(DomainManagement.btn_revertToDefault).Click(); //Click on revert to default
                IList<string> AllAvailableTools = domainmanagement.GetAvailableToolsInToolBoxConfig();
                IList<string> AlltoolsListInConfig = domainmanagement.GetAllToolsInToolBoxConfig();

                //step 10
                domainmanagement.SaveButton().Click();
                result.steps[++ExecutedSteps].StepPass();

                //Step 11
                login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(TestDomain);
                domainmanagement.SelectDomain(TestDomain);
                domainmanagement.ClickEditDomain();
                if (domainmanagement.GetAvailableToolsInToolBoxConfig().SequenceEqual(AllAvailableTools) && domainmanagement.GetAllToolsInToolBoxConfig().SequenceEqual(AlltoolsListInConfig))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                domainmanagement.CloseDomainManagement();

                //Step 12
                RoleManagement rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SearchRole(Role, TestDomain);
                rolemanagement.SelectRole(Role);
                rolemanagement.ClickEditRole();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(10);
                if (rolemanagement.GetAvailableToolsInToolBoxConfig().SequenceEqual(AllAvailableTools) && rolemanagement.GetAllToolsInToolBoxConfig().SequenceEqual(AlltoolsListInConfig))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step 13
                IWebElement column = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                IList<string> AlltoolsListInConfig_13 = domainmanagement.GetAllToolsInToolBoxConfig();
                IList<string> AllAvailableTools_13 = domainmanagement.GetAvailableToolsInToolBoxConfig();
                string stackedTool = AllAvailableTools_13[0];
                string stackedToolHeader = AlltoolsListInConfig_13[0];
                var dictionaryAddTool = new Dictionary<String, IWebElement>();
                dictionaryAddTool.Add(AllAvailableTools_13[0], column);
                rolemanagement.AddToolsToToolbox(dictionaryAddTool);
                IList<string> ToolsListInFirstColumn_2 = basepage.GetToolsInGroupInToolBoxConfig(basepage.GetGroupsInToolBoxConfig()[0]);

                IList<string> ToolsInRoleAfterEdit = new List<string>();
                foreach (IWebElement columnTools in domainmanagement.GetGroupsInToolBoxConfig())
                {
                    IList<string> toolsInEachColumn = domainmanagement.GetToolsInGroupInToolBoxConfig(columnTools);
                    ToolsInRoleAfterEdit.Add(String.Join(",", toolsInEachColumn));
                }
                rolemanagement.ClickSaveEditRole();
                result.steps[++ExecutedSteps].StepPass();


                //Step 14
                login.Logout();
                result.steps[++ExecutedSteps].StepPass();

                //step 15
                login.LoginIConnect(DomainAdmin, DomainAdmin);
                result.steps[++ExecutedSteps].StepPass();

                //step 16
                Studies studies = (Studies)login.Navigate("Studies");              
                studies.SearchStudy(AccessionNo: Accession);
                studies.SelectStudy1("Accession", Accession);
                BluRingViewer Blueringviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();

                //step 17
                Blueringviewer.OpenViewerToolsPOPUp();
                if (basepage.GetElement("cssselector", Blueringviewer.ToolBoxOpened).Displayed && basepage.GetElement("cssselector", Blueringviewer.ToolBoxOpened) != null)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("Toolbox not opened on selected viewport ");
                }

                //Step 18
                if (Blueringviewer.GetToolsInToolBoxByGrid().SequenceEqual(ToolsInRoleAfterEdit) && (Blueringviewer.GetToolsInToolBoxByGrid().Count == ToolsInRoleAfterEdit.Count))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("ToolsinRole" + (string.Join(",", ToolsInRoleAfterEdit.ToArray())));
                    Logger.Instance.ErrorLog("ToolsInViewer" + (string.Join(",", Blueringviewer.GetToolsInToolBoxByGrid().ToArray())));
                }


                //Step 19
                IWebElement Viewport = Blueringviewer.GetElement(BasePage.SelectorType.CssSelector, Blueringviewer.Activeviewport);
                Blueringviewer.SelectViewerTool(BluRingTools.Get_Pixel_Value, isOpenToolsPOPup : false);
                IList<IWebElement> popup = BasePage.Driver.FindElements(By.CssSelector(Blueringviewer.ToolBoxOpened));
                if ( popup.Count == 0)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("Toolbox still opened on selected viewport after selecting tools");
                }

                //Step 20
                Blueringviewer.ApplyTool_PixelValue();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool stepcount20 = studies.CompareImage(result.steps[ExecutedSteps], Viewport);
                if(stepcount20)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                //step 21
                Blueringviewer.OpenViewerToolsPOPUp();
                if (basepage.GetElement("cssselector", Blueringviewer.ToolBoxOpened).Displayed && basepage.GetElement("cssselector", Blueringviewer.ToolBoxOpened) != null)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("Toolbox not opened on selected viewport ");
                }

                //step 22, 23
                if (Blueringviewer.GetToolsInToolBoxByGrid(1)[0].Split(',').SequenceEqual(ToolsListInFirstColumn_2))
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("ToolsInViewerAtGridOne" + (string.Join(",", (Blueringviewer.GetToolsInToolBoxByGrid(1)[0].Split(',').ToArray()) )));
                    Logger.Instance.ErrorLog("ToolsAtFirstColumnInRole" + (string.Join(",", (ToolsListInFirstColumn_2.ToArray()) )));
                }



                //step 24
                Blueringviewer.SelectInnerViewerTool(BluRingTools.Flip_Vertical , BluRingTools.Flip_Horizontal , isOpenToolsPOPup: false);
                popup = BasePage.Driver.FindElements(By.CssSelector(Blueringviewer.ToolBoxOpened));
                if (popup.Count == 0)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("Toolbox still opened on selected viewport after selecting tools");
                }

                //step 25
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool stepcount25 = studies.CompareImage(result.steps[ExecutedSteps], Viewport);
                if (stepcount25)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                Blueringviewer.CloseBluRingViewer();
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
        ///  Superadmin user configures the toolbox for new domain user at Domain level settings
        /// </summary>
        public TestCaseResult Test_161516(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            BluRingViewer viewer = new BluRingViewer();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String DataSource = login.GetHostName(Config.EA1);
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;              

                DomainManagement domain = new DomainManagement();

                //Step1 - Login to iCA application as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                // Step2 - Navigate to Edit domain management page and Create new Domain, Role and User
                domain.NavigateToDomainManagementTab();
                String TestDomain = BasePage.GetUniqueDomainID(prefix: "TestDomain_161516_");
                String Role = BasePage.GetUniqueRole(prefix: "Role_161516_");
                String DomainAdmin = BasePage.GetUniqueUserId(prefix: "DomainAdmin_161516_");

                domain.CreateDomain(TestDomain, TestDomain, TestDomain, DomainAdmin, null, DomainAdmin, DomainAdmin, DomainAdmin, Role, Role);
                if (domain.IsDomainExist(TestDomain))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Ste3p3 - Select newly created Test Domain and click on Edit button.
                domain.SelectDomain(TestDomain);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                ExecutedSteps++;

                //Step4 - Verify the default tools should be displayed on viewer.
                var groupsInUse = domain.GetGroupsInToolBoxConfig();
                bool step4_1 = groupsInUse.Count() == 12;
                bool step4_2 = domain.VerifyConfiguredToolsInToolBoxConfig(DefaultTools.Split(':'));
                if (step4_1 && step4_2)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step5 - Drag any tool from the Available Items and drop the dragged tool to the cell 1 into the toolbox under Toolbox configuration.
                var groups = domain.GetGroupsInToolBoxConfig();
                var availableToolsList = domain.GetAvailableToolsInToolBoxConfig();
                var dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add(availableToolsList[0], groups.ElementAt(0));
                bool step5_1 = domain.AddToolsToToolbox(dictionary);
                Logger.Instance.InfoLog(availableToolsList[0] +"is added to the ToolBox config");
                bool step5_2 = domain.GetAvailableToolsInToolBoxConfig().Contains(availableToolsList[0]);// && domain.GetAvailableToolsInToolBoxConfig().Contains("Reset");
                if (step5_1 && !step5_2)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                
                //Step6 - Place any tool in stacked tool control by drag and dropping the tool from the Available Items into the cell in the toolbox at the desired position.
                dictionary = new Dictionary<String, IWebElement>();
                availableToolsList = domain.GetAvailableToolsInToolBoxConfig();
                dictionary.Add(availableToolsList[0], groups.ElementAt(0));
                bool step6_1 = domain.AddToolsToToolbox(dictionary);
                bool step6_2 = domain.GetAvailableToolsInToolBoxConfig().Contains(availableToolsList[0]);// && domain.GetAvailableToolsInToolBoxConfig().Contains("Reset");
                if (step6_1 && !step6_2)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }


                //Step7 - Place the 5 tools in each cell in the toolbox.
                BasePage.Driver.FindElement(By.CssSelector(DomainManagement.btn_revertToDefault)).Click();
                if (domain.AddToolsToEachColumnInGroupToolBox())
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                groups = domain.GetGroupsInToolBoxConfig();
                IList<String> groupTools = new List<String>();
                IList<string> toolsInEachColumn = null;
                foreach (IWebElement ele in groups)
                {
                    toolsInEachColumn = domain.GetToolsInGroupInToolBoxConfig(ele);
                    groupTools.Add(String.Join(",", toolsInEachColumn));
                }
                IList<string> availableTools = domain.GetAvailableToolsInToolBoxConfig();
                domain.ClickSaveEditDomain();

                //Step8 - Log out from Administrator user.
                login.Logout();
                Thread.Sleep(2000);
                BasePage.Driver.SwitchTo().DefaultContent();
                if (login.UserIdTxtBox().Displayed)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step9 - Log in as Test Domain Administrator.
                login.LoginIConnect(DomainAdmin, DomainAdmin);
                if (login.IsTabSelected("User Management"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step10 - Navigate to Domain management page.
                login.NavigateToDomainManagementTab();
                groups = domain.GetGroupsInToolBoxConfig();
                IList<string> groupToolsInNewDomain = new List<String>();
                foreach (IWebElement ele in groups)
                {
                    toolsInEachColumn = domain.GetToolsInGroupInToolBoxConfig(ele);
                    groupToolsInNewDomain.Add(String.Join(",", toolsInEachColumn));
                }
                IList<string> availableToolsInNewDomain = domain.GetAvailableToolsInToolBoxConfig();
                if (groupTools.SequenceEqual(groupToolsInNewDomain) && availableTools.SequenceEqual(availableToolsInNewDomain))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step11 - Go to Role Management page for the Test Domain Administrator role.
                var roleManagement = login.Navigate<RoleManagement>();
                roleManagement.SearchRole(Role);
                roleManagement.SelectRole(Role);
                rolemanagement.ClickEditRole();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                groups = domain.GetGroupsInToolBoxConfig();
                IList<string> groupToolsInNewRole = new List<String>();
                foreach (IWebElement ele in groups)
                {
                    toolsInEachColumn = domain.GetToolsInGroupInToolBoxConfig(ele);
                    groupToolsInNewRole.Add(String.Join(",", toolsInEachColumn));
                }
                IList<string> availableToolsInNewRole = domain.GetAvailableToolsInToolBoxConfig();
                if (groupTools.SequenceEqual(groupToolsInNewRole) && availableTools.SequenceEqual(availableToolsInNewRole))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                roleManagement.CloseRoleManagement();

                //Step12 - Navigate to Studies tab.
                var studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //Step13 - Search and select a study and then click on "View Exam" button.
                studies.SearchStudy(patientID: PatientID, Modality: "CT", Datasource: DataSource);
                studies.SelectStudy("Accession", Accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step14 - Click Right Mouse Button on viewport and verify that the user shall open the toolbox
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step15 - Verify the tools available in the floating toolbox
                var toolsList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_toolGrid));
                IList<String> toolsInViewer = new List<String>();
                toolsInViewer = viewer.GetToolsInToolBoxByGrid();
                Logger.Instance.InfoLog("Tools in Viewer Page: " + String.Join(", ", toolsInViewer));
                var viewerTools = toolsInViewer;
                Logger.Instance.InfoLog("Tools in Domoain Management Page: " + String.Join(", ", groupTools));
                Logger.Instance.InfoLog("Tools in Viewer Page with the names present in management page: " + String.Join(", ", viewerTools));
                if (groupTools.SequenceEqual(viewerTools))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout Application	
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
        /// At Domain Level: Configure the tools from the Available Items into the cells in the toolbox
        /// </summary>
        public TestCaseResult Test_161509(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String DataSource = login.GetHostName(Config.EA1);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");              
                String[] Accession = AccessionList.Split(':');

                String TestDomain = BasePage.GetUniqueDomainID("TestDomain_161509_");
                String Role = BasePage.GetUniqueRole("RegularUserRole_161509_");
                String DomainAdmin = BasePage.GetUniqueUserId("DomainAdmin_161509_");
                String User1 = BasePage.GetUniqueUserId("User1_161509_");

                //Step 1 - Login to iCA application as Administrator
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminUserName);
                if (login.IsTabPresent("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 2 - Domain Management page should be displayed by default
                if (login.IsTabSelected("Domain Management"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 3 - Create new domain, role, user
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.CreateDomain(TestDomain, TestDomain, TestDomain, DomainAdmin, null, DomainAdmin, DomainAdmin, DomainAdmin, Role, Role);
                login.Navigate("UserManagement");
                usermanagement.CreateUser(User1, TestDomain, Role);
                ExecutedSteps++;

                //Step 4 - Log out from Administrator user.
                login.Logout();
                ExecutedSteps++;

                //Step 5 - Log in as Test Domain Administrator.
                login.LoginIConnect(DomainAdmin, DomainAdmin);
                ExecutedSteps++;

                //Step 6 - Go to Domain Management page by clicking on Edit button.
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                ExecutedSteps++;

                //Step 7 - Verify the default tools should be displayed on viewer.
                var groupsInUse = domainmanagement.GetGroupsInToolBoxConfig();
                bool step7_1 = groupsInUse.Count() == 12;
                bool step7_2 = domainmanagement.VerifyConfiguredToolsInToolBoxConfig(DefaultTools.Split(':'));
                if (step7_1 && step7_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step8 - Drag any tool from the Available Items and drop the dragged tool to all cells under toolbox configuration.
                var groups = domainmanagement.GetGroupsInToolBoxConfig();
				Dictionary<String, IWebElement> dictionary;
				IWebElement revertButton;
				bool step8_3 = false;
				foreach (var ele in groups)
				{
                    if (domainmanagement.GetToolsInGroupInToolBoxConfig(ele).Count < 5)
                    {
                        step8_3 = true;
                        dictionary = new Dictionary<String, IWebElement>();
                        dictionary.Add(domainmanagement.GetAvailableToolsInToolBoxConfig()[1], ele);
                        if (!domainmanagement.AddToolsToToolbox(dictionary))
                        {
                            step8_3 = false;
                            break;
                        }
                        revertButton = BasePage.Driver.FindElement(By.CssSelector(DomainManagement.btn_revertToDefault));
                        if (revertButton.Enabled)
                            revertButton.Click();
                    }
				}
				revertButton = BasePage.Driver.FindElement(By.CssSelector(DomainManagement.btn_revertToDefault));
				if (revertButton.Enabled)
					revertButton.Click();
				dictionary = new Dictionary<String, IWebElement>();
                string tool = domainmanagement.GetAvailableToolsInToolBoxConfig()[0];
                dictionary.Add(tool, groups.ElementAt(2));
                bool step8_1 = domainmanagement.AddToolsToToolbox(dictionary);
                bool step8_2 = domainmanagement.GetAvailableToolsInToolBoxConfig().Contains(tool);
				if (step8_1 && !step8_2 && step8_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                domainmanagement.ClickSaveEditDomain();
                login.NavigateToDomainManagementTab();

                //Step9 - 	Place any tool in stacked tool control by drag and dropping the tool from the Available Items into the cell in the toolbox at the desired position.
                groups = domainmanagement.GetGroupsInToolBoxConfig();
                dictionary = new Dictionary<String, IWebElement>();
                var availableToolsList = domainmanagement.GetAvailableToolsInToolBoxConfig();
                dictionary.Add(availableToolsList[0], groups.ElementAt(0));
                bool step9_1 = domainmanagement.AddToolsToToolbox(dictionary);
                IList<String> toolsInGroupColumn = domainmanagement.GetToolsInGroupInToolBoxConfig(groups.ElementAt(0));
                bool step9_2 = domainmanagement.GetAvailableToolsInToolBoxConfig().Contains(availableToolsList[0]);
                if (step9_1 && !step9_2 && toolsInGroupColumn.Contains(availableToolsList[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                domainmanagement.ClickSaveEditDomain();       
                login.NavigateToDomainManagementTab();

                //Step10 - Place the 5 tools in each cell in the toolbox by drag and dropping the tool from the Available Items into the cell in the toolbox at the desired position.
                if (domainmanagement.AddToolsToEachColumnInGroupToolBox())
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step11 - 	Click on "Save" button.
                groups = domainmanagement.GetGroupsInToolBoxConfig();
                IList<String> groupTools = new List<String>();
                IList<string> toolsInEachColumn = null;
                foreach (IWebElement ele in groups)
                {
                    toolsInEachColumn = domainmanagement.GetToolsInGroupInToolBoxConfig(ele);
                    groupTools.Add(String.Join(",", toolsInEachColumn));
                }
                var availableTools = domainmanagement.GetAvailableToolsInToolBoxConfig();
                domainmanagement.ClickSaveEditDomain();
                ExecutedSteps++;

                //Step12 - Go to Test Domain Management page by clicking on Edit button and verify the tools available under the toolbox configuration and Available items.
                login.NavigateToDomainManagementTab();
                groups = domainmanagement.GetGroupsInToolBoxConfig();
                IList<string> groupToolsInNewDomain = new List<String>();
                foreach (IWebElement ele in groups)
                {
                    toolsInEachColumn = domainmanagement.GetToolsInGroupInToolBoxConfig(ele);
                    groupToolsInNewDomain.Add(String.Join(",", toolsInEachColumn));
                }
                IList<string> availableToolsInNewDomain = domainmanagement.GetAvailableToolsInToolBoxConfig();
                if (groupTools.SequenceEqual(groupToolsInNewDomain) && availableTools.SequenceEqual(availableToolsInNewDomain))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step13 - 	Go to Role Management page for the Test Domain Administrator role and ensure that by default, the toolbox configs are always inherited from the domain settings.
                rolemanagement = login.Navigate<RoleManagement>();
                rolemanagement.SearchRole(Role);
                rolemanagement.SelectRole(Role);
                rolemanagement.ClickEditRole();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                groups = rolemanagement.GetGroupsInToolBoxConfig();
                IList<string> groupToolsInNewRole = new List<String>();
                foreach (IWebElement ele in groups)
                {
                    toolsInEachColumn = rolemanagement.GetToolsInGroupInToolBoxConfig(ele);
                    groupToolsInNewRole.Add(String.Join(",", toolsInEachColumn));
                }
                IList<string> availableToolsInNewRole = rolemanagement.GetAvailableToolsInToolBoxConfig();
                if (groupTools.SequenceEqual(groupToolsInNewRole) && availableTools.SequenceEqual(availableToolsInNewRole))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                rolemanagement.CloseRoleManagement();

                //Step14 - Navigate to Studies tab.
                var studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //Step15 - Search and select a study and then click on "View Exam" button.
                studies.SearchStudy(patientID: PatientID, Modality: "CT", Datasource: DataSource);
                studies.SelectStudy("Accession", Accession[0]);
                BluRingViewer viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step16 - 	Click Right Mouse Button on viewport and verify that the user shall open the toolbox
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step17 - 	Verify the tools available in the floating toolbox
                IList<String> toolsInViewer = new List<String>();
                toolsInViewer = viewer.GetToolsInToolBoxByGrid();
                Logger.Instance.InfoLog("Tools in Viewer Page: " + String.Join(", ", toolsInViewer));
                Logger.Instance.InfoLog("Tools in Domoain Management Page: " + String.Join(", ", groupTools));
                var viewerTools = toolsInViewer;
                Logger.Instance.InfoLog("Tools in Viewer Page with the names present in management page: " + String.Join(", ", viewerTools));
                if (groupTools.SequenceEqual(viewerTools))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                
                //Step18 - 	Select any tool from the floating toolbox (e.g. Invert or Interactive WW/WL or Line Measurement ) and verify that the floating toolbox disappears when the user selects any tool from the toolbox.
                viewer.OpenStackedTool(BluRingTools.Line_Measurement, false);
                viewer.SelectViewerTool(BluRingTools.Line_Measurement,  isOpenToolsPOPup: false);
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(BluRingViewer.div_toolboxContainer)));
                if (!viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step19 - User selected tool action should be applied to the viewport.
                IWebElement viewport = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                var viewportSize = viewport.Size;
                viewer.ApplyTool_LineMeasurement(viewportSize.Width / 3, viewportSize.Height / 3, viewportSize.Width / 5, viewportSize.Height / 5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step20 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step20)
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

                //Step20 - Click on Right Mouse Button on viewport and verify that the user shall open the toolbox
                viewer.OpenViewerToolsPOPUp();
                if (viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step21 - Right click on stacked tool and verify the configured stacked menu tools in the Domain Management Page & Role Management page should be displayed in the stacked tool.
                var viewerStackedTool = viewer.OpenStackedTool(BluRingTools.Flip_Horizontal, isOpenToolsPOPup: false, Contextclick:true);
                if(viewerStackedTool != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step22 - Verify the configured tools in stacked tool should be displayed in correct position.
                IList<String> stackedToolsInViewer = new List<String>();
                stackedToolsInViewer = viewer.GetToolsInToolBoxByGrid(3);
                Logger.Instance.InfoLog("Grid 3 Stacked Tools in Viewer Page: " + String.Join(", ", stackedToolsInViewer));
                Logger.Instance.InfoLog("Column 3 Stacked Tools in Domoain Management Page: " + String.Join(", ", groupTools));
                if (groupTools[2].Equals(stackedToolsInViewer[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.SelectViewerTool(BluRingTools.Flip_Horizontal, isOpenToolsPOPup: false);

                //Step23 - 	Select any tool from the stacked tool (e.g. Line Measurement or Cobb Angle) and verify that the floating toolbox disappears when the user selects any tool from the toolbox.
                viewer.OpenViewerToolsPOPUp();
                viewer.OpenStackedTool( BluRingTools.Add_Text , false );
                var step24 = viewer.SelectViewerTool(BluRingTools.Free_Draw, isOpenToolsPOPup: false);
                if (!viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer)) && step24)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step24 - User selected stacked tool action should be applied to the viewport.
                viewer.ApplyTool_FreeDraw();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step25 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step25)
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

                //Logout Application
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
        /// 161510  - At Role Level : Configure the tools from the toolbox configuration into the cells in the toolbox
        /// </summary>
        public TestCaseResult Test_161510(String testid, String teststeps, int stepcount)
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
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] DefaultTools = this.DefaultTools.Split(':');
                String TestDomain = BasePage.GetUniqueDomainID("TestDomain_161510_");
                String Role = BasePage.GetUniqueRole("RegularUserRole_161510_");
                String DomainAdmin = BasePage.GetUniqueUserId("DomainAdmin_161510");
                String User1 = BasePage.GetUniqueUserId("User1_161510_");

                //Step 1
                login.LoginIConnect(adminUserName, adminPassword);
                result.steps[++ExecutedSteps].StepPass();

                //Step 2
                if (login.IsTabSelected("Domain Management"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 3
                DomainManagement domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.CreateDomain(TestDomain, TestDomain, TestDomain, DomainAdmin, null, DomainAdmin, DomainAdmin, DomainAdmin, Role, Role);
                result.steps[++ExecutedSteps].StepPass();

                //Step 4
                login.Logout();
                result.steps[++ExecutedSteps].StepPass();

                //step 5 Go to Domain Management page by clicking on Edit button
                login.LoginIConnect(DomainAdmin, DomainAdmin);
                result.steps[++ExecutedSteps].StepPass();

                //step6  Go to Domain Management page by clicking on Edit button
                login.Navigate("DomainManagement");
                result.steps[++ExecutedSteps].StepPass();

                //step 7 Verify the default tools. 
                if (domainmanagement.VerifyConfiguredToolsInToolBoxConfig(DefaultTools))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("Default tools should that be displayed on viewer are mismatch");
                }

                //step 8  Drag any tool from the Available Items and drop the dragged tool to the cell 1 in the toolbox under Toolbox configuration.
                BasePage.FindElementByCss(DomainManagement.btn_revertToDefault).Click(); //Click on revert to default
                IList<string> availableToolsList = domainmanagement.GetAvailableToolsInToolBoxConfig();
                IList<IWebElement> columninToolBoxConfig = domainmanagement.GetGroupsInToolBoxConfig();
                Dictionary<string, IWebElement> dictionary = new Dictionary<string, IWebElement>();
                IWebElement column1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(" + 1 + ")"));
                Dictionary<string, IWebElement> dictionarytemp = new Dictionary<string, IWebElement>();
                dictionarytemp.Add(availableToolsList[0], column1);
                domainmanagement.AddToolsToToolbox(dictionarytemp);
                if (domainmanagement.GetToolsInGroupInToolBoxConfig(domainmanagement.GetGroupsInToolBoxConfig()[0]).Contains(availableToolsList[0]))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("Tool " + availableToolsList[0] + "is not added to the toolbox under Toolbox Configuration.");
                }

                //step 9 - Place any tool in stacked tool control by drag and dropping the tool from the Available Items into the cell in the toolbox at the desired position.
                BasePage.FindElementByCss(DomainManagement.btn_revertToDefault).Click();
                dictionary = new Dictionary<String, IWebElement>();
                IList<string> ToolsListInFirstColumn = basepage.GetToolsInGroupInToolBoxConfig(basepage.GetGroupsInToolBoxConfig()[0]);
                availableToolsList = domainmanagement.GetAvailableToolsInToolBoxConfig();
                dictionary.Add(availableToolsList[0], basepage.GetGroupsInToolBoxConfig()[0]); //Add Item to First column
                domainmanagement.AddToolsToToolbox(dictionary);
                ToolsListInFirstColumn = basepage.GetToolsInGroupInToolBoxConfig(basepage.GetGroupsInToolBoxConfig()[0]);
                if (basepage.GetToolsInGroupInToolBoxConfig(basepage.GetGroupsInToolBoxConfig()[0]).Contains(availableToolsList[0]))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 10, Place the 5 tools in each cell in the toolbox.
                if (domainmanagement.AddToolsToEachColumnInGroupToolBox())
                { 
                    result.steps[++ExecutedSteps].status = "Pass"; //Step 10
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //step 11
                BasePage.FindElementByCss(DomainManagement.btn_revertToDefault).Click();
                availableToolsList = domainmanagement.GetAvailableToolsInToolBoxConfig();
                IList<string> AlltoolsListInConfig = domainmanagement.GetAllToolsInToolBoxConfig();
                domainmanagement.ClickSaveEditDomain();
                result.steps[++ExecutedSteps].StepPass();

                //Step 12 From TestDomain Management page verify the tools available under the toolbox configuration and Available items.
                login.Navigate("DomainManagement");
                if (domainmanagement.GetAvailableToolsInToolBoxConfig().SequenceEqual(availableToolsList) && domainmanagement.GetAllToolsInToolBoxConfig().SequenceEqual(AlltoolsListInConfig))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 13 In Role Management -*>* Edit TestDomain Role page -*>* Toolbox Configuration section, select Modality as CR and make sure the toolbox configs are always inherited from the domain settings by default. 
                RoleManagement rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectRole(Role);
                rolemanagement.ClickButtonInRole("edit");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(10);

                if (rolemanagement.GetAvailableToolsInToolBoxConfig("CR").SequenceEqual(availableToolsList) && rolemanagement.GetAllToolsInToolBoxConfig().SequenceEqual(AlltoolsListInConfig))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step 14 - Modify the Toolbox Configuration for floating toolbox
                IWebElement column = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                IList<string> AlltoolsListInConfig_13 = domainmanagement.GetAllToolsInToolBoxConfig();
                IList<string> AllAvailableTools_13 = domainmanagement.GetAvailableToolsInToolBoxConfig();
                string stackedToolHeader = AlltoolsListInConfig_13[0];
                var dictionaryAddTool = new Dictionary<String, IWebElement>();
                dictionaryAddTool.Add(AllAvailableTools_13[0], column);
                rolemanagement.AddToolsToToolbox(dictionaryAddTool);
                
                IList<string> ToolsInRoleAfterEdit = new List<string>();
                foreach (IWebElement columnTools in domainmanagement.GetGroupsInToolBoxConfig())
                {
                    IList<string> toolsInEachColumn = domainmanagement.GetToolsInGroupInToolBoxConfig(columnTools);
                    ToolsInRoleAfterEdit.Add(String.Join(",", toolsInEachColumn));
                }
                IList<string> stackedTool = domainmanagement.GetToolsInGroupInToolBoxConfig(column); ;

                rolemanagement.ClickSaveEditRole();
                result.steps[++ExecutedSteps].StepPass();


                //step 15 - Navigate to Studies tab.
                Studies studies = (Studies)login.Navigate("Studies");
                result.steps[++ExecutedSteps].StepPass();

                //step 16 - Search and select a CR study and then click on "Universal" button.
                studies.SearchStudy(AccessionNo: Accession, Modality: "CR");
                studies.SelectStudy1("Accession", Accession);
                BluRingViewer Blueringviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();

                //step 17 Click Right Mouse Button on viewport and verify that the user shall open the toolbox
                Blueringviewer.OpenViewerToolsPOPUp();
                if (basepage.GetElement("cssselector", Blueringviewer.ToolBoxOpened).Displayed && basepage.GetElement("cssselector", Blueringviewer.ToolBoxOpened) != null)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("Toolbox not opened on selected viewport ");
                }

                //Step 18 Verify the tools available in the floating toolbox
                if (Blueringviewer.GetToolsInToolBoxByGrid().SequenceEqual(ToolsInRoleAfterEdit))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("ToolsInViewer"+ string.Join(",", Blueringviewer.GetToolsInToolBoxByGrid().ToArray()));
                    Logger.Instance.ErrorLog("ToolsInRole" + string.Join(",", ToolsInRoleAfterEdit.ToArray()));
                }


                //Step 19 Select any tool from the floating toolbox (e.g. Invert or Interactive WW/WL) and verify that the floating toolbox disappears when the user selects any tool from the toolbox.
                IWebElement Viewport = Blueringviewer.GetElement(BasePage.SelectorType.CssSelector, Blueringviewer.Activeviewport);
                Blueringviewer.SelectViewerTool(BluRingTools.Get_Pixel_Value, isOpenToolsPOPup : false);
                IList<IWebElement> popup = BasePage.Driver.FindElements(By.CssSelector(Blueringviewer.ToolBoxOpened));
                if (popup.Count == 0)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("Toolbox still opened on selected viewport after selecting tools");
                }

                //Step 20 - User selected tool action should be applied to the viewport.
                Blueringviewer.ApplyTool_PixelValue();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool stepcount20 = studies.CompareImage(result.steps[ExecutedSteps], Viewport);
                if (stepcount20)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                //step 21 - Click on Right Mouse Button on viewport and verify that the user shall open the toolbox
                Blueringviewer.OpenViewerToolsPOPUp();
                if (basepage.GetElement("cssselector", Blueringviewer.ToolBoxOpened).Displayed && basepage.GetElement("cssselector", Blueringviewer.ToolBoxOpened) != null)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("Toolbox not opened on selected viewport ");
                }

                //step 22 , 23 - Right click on stacked tool and verify the configured stacked menu tools in the Role Management Page should be displayed in the stacked tool.
                if (Blueringviewer.GetToolsInToolBoxByGrid(1)[0].Split(',').SequenceEqual( stackedTool ))
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("ToolsInViewer" + string.Join(",", Blueringviewer.GetToolsInToolBoxByGrid(1)[0].Split(',').ToArray()));
                    Logger.Instance.ErrorLog("stackedToolToolsInRole" + string.Join(",", stackedTool.ToArray()));
                }


                //step 24
                Blueringviewer.SelectInnerViewerTool(BluRingTools.Flip_Vertical, BluRingTools.Flip_Horizontal , isOpenToolsPOPup : false );
                popup = BasePage.Driver.FindElements(By.CssSelector(Blueringviewer.ToolBoxOpened));
                if (popup.Count == 0)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("Toolbox still opened on selected viewport after selecting tools");
                }

                //step 25
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool stepcount25 = studies.CompareImage(result.steps[ExecutedSteps], Viewport);
                if (stepcount25)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                Blueringviewer.CloseBluRingViewer();
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
                Logger.Instance.InfoLog("Errorr occured while execute the test case " + testid + " , Overall Test status--" + result.status);

                //Return Result
                return result;
            }

        }

        /// <summary>
        /// 161511   - At Role Level : Configure the tools from the toolbox configuration into the cells in the toolbox
        /// </summary>
        public TestCaseResult Test_161511(String testid, String teststeps, int stepcount)
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
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] DefaultTools = this.DefaultTools.Split(':');
                String TestDomain = BasePage.GetUniqueDomainID("TestDomain_161511_");
                String Role = BasePage.GetUniqueRole("RegularUserRole_161511_");
                String DomainAdmin = BasePage.GetUniqueUserId("DomainAdmin_161511_");
                String User1 = BasePage.GetUniqueUserId("User1_161511_");

                login.LoginIConnect(adminUserName, adminPassword);
                DomainManagement domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.CreateDomain(TestDomain, TestDomain, TestDomain, DomainAdmin, null, DomainAdmin, DomainAdmin, DomainAdmin, Role, Role);
                login.Navigate("UserManagement");
                usermanagement.CreateUser(User1, TestDomain, Role);
                login.Logout();

                //Step 1
                login.LoginIConnect(DomainAdmin, DomainAdmin);
                result.steps[++ExecutedSteps].StepPass();

                //Step 2
                login.Navigate("DomainManagement");
                result.steps[++ExecutedSteps].StepPass();

                //Step 3 - Drag any tool from the Available Items section and drop the dragged tool into any cells in the toolbox under Toolbox Configuration.
                bool step3 = false; int i = 0;
                BasePage.FindElementByCss(DomainManagement.btn_revertToDefault).Click(); //Click on revert to default
                Thread.Sleep(3000);
                IList<string> availableToolsList = domainmanagement.GetAvailableToolsInToolBoxConfig();
                IList<IWebElement> columninToolBoxConfig = domainmanagement.GetGroupsInToolBoxConfig();
                Dictionary<string, IWebElement> dictionary = new Dictionary<string, IWebElement>();
                for ( i = 0; i < availableToolsList.Count; i++)
                {
                    IWebElement columnTemp = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(" + (i + 1) + ")"));
                    Dictionary<string, IWebElement> dictionarytemp_3 = new Dictionary<string, IWebElement>();
                    dictionarytemp_3.Add(availableToolsList[i], columnTemp);
                    domainmanagement.AddToolsToToolbox(dictionarytemp_3, addToolAtEnd: true);
                    if (domainmanagement.GetToolsInGroupInToolBoxConfig(domainmanagement.GetGroupsInToolBoxConfig()[i]).Contains(availableToolsList[i]))
                    {
                        step3 = true;
                    }
                    else
                    {
                        step3 = false;
                        break;
                    }
                }
                if (step3)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("Tool " + availableToolsList[i] + "is not added to the toolbox under Toolbox Configuration.");
                }

                //step 4
                //Click on "Save" button.
                BasePage.FindElementByCss(DomainManagement.btn_revertToDefault).Click(); //Click on revert to default
                Thread.Sleep(3000);
                availableToolsList = domainmanagement.GetAvailableToolsInToolBoxConfig();
                 columninToolBoxConfig = domainmanagement.GetGroupsInToolBoxConfig();
                dictionary.Add(availableToolsList[0], columninToolBoxConfig[0]);
                domainmanagement.AddToolsToToolbox(dictionary, addToolAtEnd: true);

                availableToolsList = domainmanagement.GetAvailableToolsInToolBoxConfig();
                IList<string> AlltoolsListInConfig = new List<string>();
                AlltoolsListInConfig = domainmanagement.GetAllToolsInToolBoxConfig();
                domainmanagement.ClickSaveEditDomain();
                result.steps[++ExecutedSteps].StepPass();

                //step 5 - Verify the tools available under the toolbox configuration and Available items.
                login.Navigate("DomainManagement");
                if (domainmanagement.GetAvailableToolsInToolBoxConfig().SequenceEqual(availableToolsList) && domainmanagement.GetAllToolsInToolBoxConfig().SequenceEqual(AlltoolsListInConfig))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step 6 - Modify the Toolbox Configuration for floating toolbox and click on "Save" button

                BasePage.FindElementByCss(DomainManagement.btn_revertToDefault).Click(); //Click on revert to default
                Thread.Sleep(3000);
                dictionary = new Dictionary<string, IWebElement>();
                IWebElement column1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(" + 1 + ")"));
                Dictionary<string, IWebElement> dictionarytemp = new Dictionary<string, IWebElement>();
                dictionarytemp.Add(availableToolsList[0], column1);
                domainmanagement.AddToolsToToolbox(dictionarytemp, addToolAtEnd: true);
                bool status6 = domainmanagement.GetToolsInGroupInToolBoxConfig(domainmanagement.GetGroupsInToolBoxConfig()[0]).Contains(availableToolsList[0]);
                availableToolsList = domainmanagement.GetAvailableToolsInToolBoxConfig();
                AlltoolsListInConfig = domainmanagement.GetAllToolsInToolBoxConfig();
                AlltoolsListInConfig = new List<string>();
                foreach (IWebElement columnTools in domainmanagement.GetGroupsInToolBoxConfig())
                {
                    IList<string> toolsInEachColumn = domainmanagement.GetToolsInGroupInToolBoxConfig(columnTools);
                    AlltoolsListInConfig.Add(String.Join(",", toolsInEachColumn));
                }
                domainmanagement.ClickSaveEditDomain();
                if (status6)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("Tool " + availableToolsList[0] + "is not added to the toolbox under Toolbox Configuration in Domain edit Page.");
                }

                //step 7 - Navigate to Studies tab.
                Studies studies = (Studies)login.Navigate("Studies");
                result.steps[++ExecutedSteps].StepPass();

                //step 8 - Search and select a study and then click on "Universal" button.
                studies.SearchStudy(AccessionNo: Accession);
                studies.SelectStudy1("Accession", Accession);
                BluRingViewer Blueringviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();

                //step 9 -  Click Right Mouse Button on viewport and verify that the user shall open the toolbox
                Blueringviewer.OpenViewerToolsPOPUp();
                if (basepage.GetElement("cssselector", Blueringviewer.ToolBoxOpened).Displayed && basepage.GetElement("cssselector", Blueringviewer.ToolBoxOpened) != null)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("Toolbox not opened on selected viewport ");
                }

                //step 10 - Verify the tools available in the floating toolbox
                if (Blueringviewer.GetToolsInToolBoxByGrid().SequenceEqual(AlltoolsListInConfig))
                {
                    result.steps[++ExecutedSteps].StepPass(); //Step 10
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step 11 - Select any tool from the floating toolbox (e.g. Invert or Interactive WW/WL or Line Measurement ) and verify that the floating toolbox disappears when the user selects any tool from the toolbox.
                Blueringviewer.SelectViewerTool(BluRingTools.Get_Pixel_Value, isOpenToolsPOPup: false );
                IList<IWebElement> popup = BasePage.Driver.FindElements(By.CssSelector(Blueringviewer.ToolBoxOpened));
                if (popup.Count == 0)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("Toolbox still opened on selected viewport after selecting tools");
                }


                //step 12 - User selected tool action should be applied to the viewport.
                IWebElement Viewport = Blueringviewer.GetElement(BasePage.SelectorType.CssSelector, Blueringviewer.Activeviewport);
                Blueringviewer.ApplyTool_PixelValue();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool stepcount13 = studies.CompareImage(result.steps[ExecutedSteps], Viewport);
                if (stepcount13)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                Blueringviewer.CloseBluRingViewer();


                //Stepcount 13 - Go back to Role Management page and open to edit the role page for Regular User role.
                RoleManagement rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectRole(Role);
                rolemanagement.ClickButtonInRole("edit");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].StepPass();

                //step 14 - Modify the Toolbox Configuration for floatingtoolbox Save the changes.
                IWebElement column = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                IList<string> AlltoolsListInConfig_15 = domainmanagement.GetAllToolsInToolBoxConfig();
                IList<string> AllAvailableTools_15 = domainmanagement.GetAvailableToolsInToolBoxConfig();
                string stackedTool = AllAvailableTools_15[0];
                string stackedToolHeader = AlltoolsListInConfig_15[0];
                var dictionaryAddTool = new Dictionary<String, IWebElement>();
                dictionaryAddTool.Add(AllAvailableTools_15[0], column);
                rolemanagement.AddToolsToToolbox(dictionaryAddTool, addToolAtEnd: true);

                IList<string> ToolsInRoleAfterEdit = new List<string>();
                foreach (IWebElement columnTools in domainmanagement.GetGroupsInToolBoxConfig())
                {
                    IList<string> toolsInEachColumn = domainmanagement.GetToolsInGroupInToolBoxConfig(columnTools);
                    ToolsInRoleAfterEdit.Add(String.Join(",", toolsInEachColumn));
                }
                rolemanagement.ClickSaveEditRole();
                result.steps[++ExecutedSteps].StepPass();

                //step 15 - Log out as Test Domain Administrator
                login.Logout();
                result.steps[++ExecutedSteps].StepPass();

                //step 16 - Log in as User1
                login.LoginIConnect(User1, User1);
                result.steps[++ExecutedSteps].StepPass();

                //step 17 - 	Navigate to Studies tab.
                studies = (Studies)login.Navigate("Studies");
                result.steps[++ExecutedSteps].StepPass();

                //step 18 - Search and select a study and then click on "Universal" button.
                studies.SearchStudy(AccessionNo: Accession);
                studies.SelectStudy1("Accession", Accession);
                Blueringviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();

                //step 19
                Blueringviewer.OpenViewerToolsPOPUp();
                if (basepage.GetElement("cssselector", Blueringviewer.ToolBoxOpened).Displayed && basepage.GetElement("cssselector", Blueringviewer.ToolBoxOpened) != null)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("Toolbox not opened on selected viewport ");
                }

                //step 20 - 	Verify the tools available in the floating toolbox
                if (Blueringviewer.GetToolsInToolBoxByGrid().SequenceEqual(ToolsInRoleAfterEdit))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("ToolsInViewer"+ string.Join(",", Blueringviewer.GetToolsInToolBoxByGrid().ToArray()) );
                    Logger.Instance.ErrorLog("ToolsInRole" + string.Join(",", ToolsInRoleAfterEdit.ToArray()));
                }

               
                //step 21 - Select any tool from the floating toolbox (e.g. Invert or Interactive WW/WL) and verify that the floating toolbox disappears when the user selects any tool from the toolbox.
                Blueringviewer.SelectViewerTool(BluRingTools.Get_Pixel_Value, isOpenToolsPOPup: false);
                popup = BasePage.Driver.FindElements(By.CssSelector(Blueringviewer.ToolBoxOpened));
                if (popup.Count == 0)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("Toolbox still opened on selected viewport after selecting tools");
                }

                //step 22 - User selected tool action should be applied to the viewport.
                Viewport = Blueringviewer.GetElement(BasePage.SelectorType.CssSelector, Blueringviewer.Activeviewport);
                Blueringviewer.ApplyTool_PixelValue();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool stepcount24 = studies.CompareImage(result.steps[ExecutedSteps], Viewport);
                if (stepcount24)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                Blueringviewer.CloseBluRingViewer();
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
                Logger.Instance.InfoLog("Errorr occured while execute the test case " + testid + " , Overall Test status--" + result.status);

                //Return Result
                return result;
            }

        }

        /// <summary>
        /// 161518  - Configure the Modality Toolbar from another machine
        /// </summary>
        public TestCaseResult Test_161518(String testid, String teststeps, int stepcount)
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
                String[] Accession = ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList").ToString().Split(',');
                String[] DefaultTools = this.DefaultTools.Split(':');
                String TestDomain = BasePage.GetUniqueDomainID("TestDomain_161518_");
                String Role = BasePage.GetUniqueRole("RegularUserRole_161518_");
                String DomainAdmin = BasePage.GetUniqueUserId("DomainAdmin_161518");
                String User1 = BasePage.GetUniqueUserId("User1_161518_");

               //Precondition
               BasePage.MultiDriver.Add(BasePage.Driver);
               //Config.node = Config.Clientsys1;
               login.DriverGoTo(login.url);
               login.LoginIConnect(adminUserName, adminPassword);
               DomainManagement domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
               domainmanagement.CreateDomain(TestDomain, TestDomain, TestDomain, DomainAdmin, null, DomainAdmin, DomainAdmin, DomainAdmin, Role, Role);
               login.Navigate("UserManagement");
               usermanagement.CreateUser(User1, TestDomain, Role);
               login.Logout();


                //step 1
                BasePage.MultiDriver.Add(login.InvokeBrowser("Remote-" + Config.BrowserType));
                login.SetDriver(BasePage.MultiDriver[1]);
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminUserName);
                login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(TestDomain);
                domainmanagement.SelectDomain(TestDomain);
                domainmanagement.ClickEditDomain();

                SelectElement Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));

                Dictionary<String, IWebElement> dictionary = new Dictionary<String, IWebElement>();
                IList<string> ToolsListInFirstColumn_Default = basepage.GetToolsInGroupInToolBoxConfig(basepage.GetGroupsInToolBoxConfig()[0]);
                IList<string> availableToolsList_Default = domainmanagement.GetAvailableToolsInToolBoxConfig();
                dictionary.Add(availableToolsList_Default[1], basepage.GetGroupsInToolBoxConfig()[0]); //Add Item to First column
                domainmanagement.AddToolsToToolbox(dictionary);
                IList<string> DomainToolsInDefaultAfterEdit = new List<string>();
                foreach (IWebElement columnTools in domainmanagement.GetGroupsInToolBoxConfig())
                {
                    IList<string> toolsInEachColumn = domainmanagement.GetToolsInGroupInToolBoxConfig(columnTools);
                    DomainToolsInDefaultAfterEdit.Add(String.Join(",", toolsInEachColumn));
                }

                Modality.SelectByText("CR");
                BasePage.FindElementByCss(DomainManagement.btn_revertToDefault).Click();
                Dictionary<String, IWebElement> dictionary_CR = new Dictionary<String, IWebElement>();
                IList<string> availableToolsList_CR = domainmanagement.GetAvailableToolsInToolBoxConfig("CR");
                dictionary_CR.Add(availableToolsList_CR[0], basepage.GetGroupsInToolBoxConfig()[0]); //Add Item to First column
                domainmanagement.AddToolsToToolbox(dictionary_CR);
                IList<string> DomainToolsInCRAfterEdit = new List<string>();
                foreach (IWebElement columnTools in domainmanagement.GetGroupsInToolBoxConfig())
                {
                    IList<string> toolsInEachColumn = domainmanagement.GetToolsInGroupInToolBoxConfig(columnTools);
                    DomainToolsInCRAfterEdit.Add(String.Join(",", toolsInEachColumn));
                }

                domainmanagement.ClickSaveEditDomain();
                login.Logout();
                result.steps[++ExecutedSteps].StepPass(); //Step 1

                //step 2
                login.SetDriver(BasePage.MultiDriver[0]);
                login.LoginGrid(User1, User1);

                Studies studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Modality: "CR");
                studies.SelectStudy1("Accession", Accession[0]);
                BluRingViewer Blueringviewer = BluRingViewer.LaunchBluRingViewer();

                Blueringviewer.OpenViewerToolsPOPUp();
                bool Step2_1 = basepage.GetElement("cssselector", Blueringviewer.ToolBoxOpened).Displayed && basepage.GetElement("cssselector", Blueringviewer.ToolBoxOpened) != null;
                bool Step2_2 = Blueringviewer.GetToolsInToolBoxByGrid().SequenceEqual(DomainToolsInCRAfterEdit);
                Blueringviewer.CloseBluRingViewer();

                studies.SearchStudy(AccessionNo: Accession[1], Modality: "CT");
                studies.SelectStudy1("Accession", Accession[1]);
                Blueringviewer = BluRingViewer.LaunchBluRingViewer();


                Blueringviewer.OpenViewerToolsPOPUp();
               // bool Step2_3 = basepage.GetElement("cssselector", Blueringviewer.ToolBoxOpened).Displayed && basepage.GetElement("cssselector", Blueringviewer.ToolBoxOpened) != null;
                bool Step2_4 = Blueringviewer.GetToolsInToolBoxByGrid().SequenceEqual(DomainToolsInDefaultAfterEdit);

                if (Step2_1 && Step2_2 && Step2_4)
                {
                    result.steps[++ExecutedSteps].StepPass(); //Step 2
                    result.steps[++ExecutedSteps].StepPass(); //Step 3
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail(Step2_1 +","+ Step2_2 +","+ Step2_4); //Step 2
                    result.steps[++ExecutedSteps].StepFail(); //Step 3
                }
                Blueringviewer.CloseBluRingViewer();
                login.Logout();

                //Step 4
                login.SetDriver(BasePage.MultiDriver[1]);
                login.LoginIConnect(adminUserName, adminPassword);
                RoleManagement roleManagement = (RoleManagement)login.Navigate("RoleManagement");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(50);
                Thread.Sleep(5000);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                roleManagement.SearchRole(Role, TestDomain);
                rolemanagement.SelectRole(Role);
                rolemanagement.ClickButtonInRole("edit");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].StepPass();

                //step 5
                IWebElement column = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                IList<string> AlltoolsListInConfig_5 = domainmanagement.GetAllToolsInToolBoxConfig();
                IList<string> AllAvailableTools_5 = domainmanagement.GetAvailableToolsInToolBoxConfig();
                var dictionaryAddTool = new Dictionary<String, IWebElement>();
                dictionaryAddTool.Add(AllAvailableTools_5[0], column);
                rolemanagement.AddToolsToToolbox(dictionaryAddTool);

                IList<string> ToolsInRoleAfterEdit = new List<string>();
                foreach (IWebElement columnTools in domainmanagement.GetGroupsInToolBoxConfig())
                {
                    IList<string> toolsInEachColumn = domainmanagement.GetToolsInGroupInToolBoxConfig(columnTools);
                    ToolsInRoleAfterEdit.Add(String.Join(",", toolsInEachColumn));
                }
                rolemanagement.ClickSaveEditRole();
                login.Logout();
                result.steps[++ExecutedSteps].StepPass();


                //Step 6
                login.SetDriver(BasePage.MultiDriver[0]);
                login.LoginGrid(User1, User1);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0]);
                studies.SelectStudy1("Accession", Accession[0]);
                Blueringviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();

                //Step 7
                Blueringviewer.OpenViewerToolsPOPUp();
                if (Blueringviewer.GetToolsInToolBoxByGrid().SequenceEqual(ToolsInRoleAfterEdit))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                Blueringviewer.CloseBluRingViewer();
                login.Logout();

                //Step 8
                login.SetDriver(BasePage.MultiDriver[1]);
                login.LoginGrid(adminUserName, adminPassword);
                roleManagement = (RoleManagement)login.Navigate("RoleManagement");
                roleManagement.SearchRole(Role, TestDomain);
                rolemanagement.SelectRole(Role);
                rolemanagement.ClickButtonInRole("edit");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].StepPass();

                //step 9
                Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("CR");
                BasePage.FindElementByCss(DomainManagement.btn_revertToDefault).Click();
                dictionary_CR = new Dictionary<String, IWebElement>();
                availableToolsList_CR = domainmanagement.GetAvailableToolsInToolBoxConfig("CR");
                dictionary_CR.Add(availableToolsList_CR[0], basepage.GetGroupsInToolBoxConfig()[0]); //Add Item to First column
                IList<string> ToolsInCRAfterEdit = new List<string>();
                foreach (IWebElement columnTools in domainmanagement.GetGroupsInToolBoxConfig())
                {
                    IList<string> toolsInEachColumn = domainmanagement.GetToolsInGroupInToolBoxConfig(columnTools);
                    ToolsInRoleAfterEdit.Add(String.Join(",", toolsInEachColumn));
                }
                
                rolemanagement.ClickSaveEditRole();
                login.Logout();
                result.steps[++ExecutedSteps].StepPass();


                //Step 10
                login.SetDriver(BasePage.MultiDriver[0]);
                login.LoginIConnect(User1, User1);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Modality: "CR");
                studies.SelectStudy1("Accession", Accession[0]);
                Blueringviewer = BluRingViewer.LaunchBluRingViewer();
                 result.steps[++ExecutedSteps].StepPass();

                //step 11
                Blueringviewer.OpenViewerToolsPOPUp();
                if (Blueringviewer.GetToolsInToolBoxByGrid().SequenceEqual(ToolsInCRAfterEdit))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                Blueringviewer.CloseBluRingViewer();
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
                Logger.Instance.InfoLog("Errorr occured while execute the test case " + testid + " , Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                BasePage basePage = new BasePage();
                basePage.ResetDriver();
            }

        }

        /// <summary>
        /// Verify the default tools.
        /// </summary>
        public TestCaseResult Test_162580(String testid, String teststeps, int stepcount)
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
                String patientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] PatientId = patientIDList.Split(':');
                String[] DefaultToolsList = DefaultTools.Split(':');                

                //Step 1 - Launch the iCA application with a client browser 
                login.DriverGoTo(login.url);
                BasePage.Driver.SwitchTo().DefaultContent();
                if (login.UserIdTxtBox().Displayed &&
                    login.PasswordTxtBox().Displayed &&
                    login.LoginBtn().Displayed)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step-2 - Login to WebAccess site with any privileged user.
                login.LoginIConnect(adminUserName, adminPassword);
                if (login.IsTabPresent("Studies") && login.IsTabPresent("Domain Management"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step 3 - Select 'Domain Management' tab.
                DomainManagement domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                ExecutedSteps++;

                // Step 4 - Select the 'Super Admin Group' and then click on 'Edit' button.	
                domainmanagement.SearchDomain(Config.adminGroupName);
                domainmanagement.SelectDomain(Config.adminGroupName);
                domainmanagement.ClickEditDomain();
                ExecutedSteps++;

                // step 5 - Navigate to 'Toolbox Configuration' section and verify the default value from 'Modality' drop down.
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                var Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                var selectedOption = Modality.SelectedOption.GetAttribute("innerHTML");
                if (selectedOption.Equals("default"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // step 6 - Verify the default tools in all 12 positions.	
                var groupsInUse = domainmanagement.GetGroupsInToolBoxConfig();
                bool step6_1 = groupsInUse.Count() == 12;
                bool step6_2 = domainmanagement.VerifyConfiguredToolsInToolBoxConfig(DefaultToolsList);
                if (step6_1 && step6_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();


                // step 7 - Select 'CR' from Modality drop down and then verify the default tools in all 12 positions.	
                Modality.SelectByValue("CR");
                groupsInUse = domainmanagement.GetGroupsInToolBoxConfig();
                bool step7_1 = groupsInUse.Count() == 12;
                bool step7_2 = domainmanagement.VerifyConfiguredToolsInToolBoxConfig(DefaultToolsList);
                if (step7_1 && step7_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 8 - Similarly check for two more modalities and verify the default tools in all 12 positions.	
                Modality.SelectByValue("MR");
                groupsInUse = domainmanagement.GetGroupsInToolBoxConfig();
                bool step8_1 = groupsInUse.Count() == 12;
                bool step8_2 = domainmanagement.VerifyConfiguredToolsInToolBoxConfig(DefaultToolsList);
                Modality.SelectByValue("US");
                groupsInUse = domainmanagement.GetGroupsInToolBoxConfig();
                bool step8_3 = groupsInUse.Count() == 12;
                bool step8_4 = domainmanagement.VerifyConfiguredToolsInToolBoxConfig(DefaultToolsList);
                if (step8_1 && step8_2 && step8_3 && step8_4)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // step 9 - Click 'Close' button.                                
                domainmanagement.ClickCloseEditDomain();
                ExecutedSteps++;

                // Step 10 - Click on 'Role Management' tab.	
                RoleManagement rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                ExecutedSteps++;

                // step 11 - Select 'Super Role' and click on 'Edit' button.
                rolemanagement.SelectDomainfromDropDown("SuperAdminGroup");
                rolemanagement.SearchRole("SuperRole");
                rolemanagement.SelectRole("SuperRole");
                rolemanagement.ClickEditRole();
                ExecutedSteps++;

                // Step 12 - Navigate to 'Toolbox Configuration' section and verify the default value from 'Modality' drop down.	
                Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                selectedOption = Modality.SelectedOption.GetAttribute("innerHTML");
                if (selectedOption.Equals("default"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 13 - Verify the default tools in all 12 positions.	
                groupsInUse = rolemanagement.GetGroupsInToolBoxConfig();
                bool step13_1 = groupsInUse.Count() == 12;
                bool step13_2 = rolemanagement.VerifyConfiguredToolsInToolBoxConfig(DefaultToolsList);
                if (step13_1 && step13_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 14 - Select 'CR' from Modality drop down and then verify the default tools in all 12 positions.	
                Modality.SelectByValue("CR");
                groupsInUse = rolemanagement.GetGroupsInToolBoxConfig();
                bool step14_1 = groupsInUse.Count() == 12;
                bool step14_2 = rolemanagement.VerifyConfiguredToolsInToolBoxConfig(DefaultToolsList);
                if (step14_1 && step14_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 15 - Similarly check for two more modalities and verify the default tools in all 12 positions.	
                Modality.SelectByValue("MR");
                groupsInUse = rolemanagement.GetGroupsInToolBoxConfig();
                bool step15_1 = groupsInUse.Count() == 12;
                bool step15_2 = rolemanagement.VerifyConfiguredToolsInToolBoxConfig(DefaultToolsList);
                Modality.SelectByValue("US");
                groupsInUse = rolemanagement.GetGroupsInToolBoxConfig();
                bool step15_3 = groupsInUse.Count() == 12;
                bool step15_4 = rolemanagement.VerifyConfiguredToolsInToolBoxConfig(DefaultToolsList);
                if (step15_1 && step15_2 && step15_3 && step15_4)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 16 - Click 'Close' button.                
                rolemanagement.ClickCloseButton();
                ExecutedSteps++;

                // Step 17 - Click on 'Studies' tab.	
                Studies studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                // Step 18 - Search for any study, select and click on 'View Exam' button.	
                studies.SearchStudy(patientID: PatientId[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Patient ID", PatientId[0]);
                BluRingViewer viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                // Step 19 - Click Right Mouse Button on viewport and verify that the user shall open the toolbox	
                viewer.OpenViewerToolsPOPUp();
                var isToolBoxVisible = viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_toolboxContainer));
                if (isToolBoxVisible)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 20 - Verify the tools.	                
                bool step20 = viewer.GetToolsInToolBoxByGrid().SequenceEqual(DefaultToolsList.ToList());
                if (step20)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 21 - Right click on 'Window Level' tool and verify the available tools and icon in stacked tool.
                viewer.OpenStackedTool(BluRingTools.Window_Level, false, false);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_21 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_toolboxContainer));
                if (step_21)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 22 - Right click on 'Zoom' tool and verify the available tools and icon in stacked tool.	
                String csstool = BluRingViewer.div_studypanel + ":nth-of-type(1) div.compositeViewerComponent div.viewerContainer:nth-of-type(1) " + BluRingViewer.GetToolCss(BluRingTools.Interactive_Zoom);
                IWebElement element = BasePage.Driver.FindElement(By.CssSelector(csstool));
                TestCompleteAction action = new TestCompleteAction();
                if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("firefox")))
                {
                    action.MoveToElement(element);
                    action.Release().Perform();
                }
                else
                {
                    viewer.HoverElement(element);
                }
                Thread.Sleep(2000);
                viewer.OpenStackedTool(BluRingTools.Interactive_Zoom, isOpenToolsPOPup: false, Contextclick: false);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_22 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_toolboxContainer));
                if (step_22)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 23 - Right click on 'Line Measurement' tool and verify the available tools and icon in stacked tool.	
                csstool = BluRingViewer.div_studypanel + ":nth-of-type(1) div.compositeViewerComponent div.viewerContainer:nth-of-type(1) " + BluRingViewer.GetToolCss(BluRingTools.Line_Measurement);
                element = BasePage.Driver.FindElement(By.CssSelector(csstool));
                action = new TestCompleteAction();
                if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("firefox")))
                {
                    action.MoveToElement(element);
                    action.Release().Perform();
                }
                else
                {
                    viewer.HoverElement(element);
                }
                Thread.Sleep(2000);
                viewer.OpenStackedTool(BluRingTools.Line_Measurement, isOpenToolsPOPup: false, Contextclick: false);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_23 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_toolboxContainer));
                if (step_23)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 24 - Right click on 'Angle Measurement' tool and verify the available tools and icon in stacked tool.	
                csstool = BluRingViewer.div_studypanel + ":nth-of-type(1) div.compositeViewerComponent div.viewerContainer:nth-of-type(1) " + BluRingViewer.GetToolCss(BluRingTools.Angle_Measurement);
                element = BasePage.Driver.FindElement(By.CssSelector(csstool));
                action = new TestCompleteAction();
                if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("firefox")))
                {
                    action.MoveToElement(element);
                    action.Release().Perform();
                }
                else
                {
                    viewer.HoverElement(element);
                }
                Thread.Sleep(2000);
                viewer.OpenStackedTool(BluRingTools.Angle_Measurement, isOpenToolsPOPup: false, Contextclick: false);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_24 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_toolboxContainer));
                if (step_24)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 25 - Right click on 'Draw Ellipse' tool and verify the available tools and icon in stacked tool.
                csstool = BluRingViewer.div_studypanel + ":nth-of-type(1) div.compositeViewerComponent div.viewerContainer:nth-of-type(1) " + BluRingViewer.GetToolCss(BluRingTools.Draw_Ellipse);
                element = BasePage.Driver.FindElement(By.CssSelector(csstool));
                action = new TestCompleteAction();
                if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("firefox")))
                {
                    action.MoveToElement(element);
                    action.Release().Perform();
                }
                else
                {
                    viewer.HoverElement(element);
                }
                Thread.Sleep(2000);
                viewer.OpenStackedTool(BluRingTools.Draw_Ellipse, isOpenToolsPOPup: false, Contextclick: false);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_25 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_toolboxContainer));
                if (step_25)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 26 - Right click on 'Rotate Clockwise' tool and verify the available tools and icon in stacked tool.	
                csstool = BluRingViewer.div_studypanel + ":nth-of-type(1) div.compositeViewerComponent div.viewerContainer:nth-of-type(1) " + BluRingViewer.GetToolCss(BluRingTools.Rotate_Clockwise);
                element = BasePage.Driver.FindElement(By.CssSelector(csstool));
                action = new TestCompleteAction();
                if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("firefox")))
                {
                    action.MoveToElement(element);
                    action.Release().Perform();
                }
                else
                {
                    viewer.HoverElement(element);
                }
                Thread.Sleep(2000);
                viewer.OpenStackedTool(BluRingTools.Rotate_Clockwise, isOpenToolsPOPup: false, Contextclick: false);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_26 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_toolboxContainer));
                if (step_26)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 27 - Right click on 'Flip Horizontal' tool and verify the available tools and icon in stacked tool.	
                csstool = BluRingViewer.div_studypanel + ":nth-of-type(1) div.compositeViewerComponent div.viewerContainer:nth-of-type(1) " + BluRingViewer.GetToolCss(BluRingTools.Flip_Horizontal);
                element = BasePage.Driver.FindElement(By.CssSelector(csstool));
                action = new TestCompleteAction();
                if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("firefox")))
                {
                    action.MoveToElement(element);
                    action.Release().Perform();
                }
                else
                {
                    viewer.HoverElement(element);
                }
                Thread.Sleep(2000);
                viewer.OpenStackedTool(BluRingTools.Flip_Horizontal, isOpenToolsPOPup: false, Contextclick: false);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_27 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_toolboxContainer));
                if (step_27)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();
                viewer.SelectViewerTool(BluRingTools.Flip_Horizontal, isOpenToolsPOPup: false);

                // Step 28 - Right click on 'Add Text' tool and verify the available tools and icon in stacked tool.	
                viewer.OpenViewerToolsPOPUp();
                csstool = BluRingViewer.div_studypanel + ":nth-of-type(1) div.compositeViewerComponent div.viewerContainer:nth-of-type(1) " + BluRingViewer.GetToolCss(BluRingTools.Draw_Ellipse);
                element = BasePage.Driver.FindElement(By.CssSelector(csstool));
                action = new TestCompleteAction();
                if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("firefox")))
                {
                    action.MoveToElement(element);
                    action.Release().Perform();
                }
                else
                {
                    viewer.HoverElement(element);
                }
                Thread.Sleep(2000);
                viewer.OpenStackedTool(BluRingTools.Draw_Ellipse, isOpenToolsPOPup: false, Contextclick: false);
                Thread.Sleep(2000);
                viewer.SelectViewerTool(BluRingTools.Draw_Ellipse);
                viewer.OpenViewerToolsPOPUp();
                viewer.OpenStackedTool(BluRingTools.Add_Text, isOpenToolsPOPup: false, Contextclick: false);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_28 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_toolboxContainer));
                if (step_28)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Close the viewer and logout
                viewer.CloseBluRingViewer();
                login.Logout();

                //Return Result.
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
        /// Role Management Page: Maximum of 5 tools in a stack
        /// </summary>
        public TestCaseResult Test_161515(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            BluRingViewer viewer = new BluRingViewer();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String[] AddNewTool = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "AddNewTools")).Split(',');

                DomainManagement domain = new DomainManagement();
                //Step1 - Login to iCA application as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                result.steps[++ExecutedSteps].StepPass();

                //Step2 - Domain Management page should be displayed by default
                if (login.IsTabSelected("Domain Management"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step3 Create new:
                // 1.Test Domain
                // 2.role(Regular User) under the Test Domain(so you will have the Test Domain Admin role and the Regular User role)
                // domain admin(Administrator Test Domain)
                // regular user(User1, belonging to Test Domain and with a Regular User
                String TestDomain = "TestDomain_tb_12_" + new Random().Next(1, 10000);
                String Role = "Role_tb_12_" + new Random().Next(1, 10000);
                String DomainAdmin = "DomainAdmin_tb_12_" + new Random().Next(1, 10000);
                domain.CreateDomain(TestDomain, TestDomain, TestDomain, DomainAdmin, null, DomainAdmin, DomainAdmin, DomainAdmin, Role, Role);
                if (domain.IsDomainExist(TestDomain))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step4 - Log out from Administrator user.
                login.Logout();
                Thread.Sleep(2000);
                BasePage.Driver.SwitchTo().DefaultContent();
                if (login.UserIdTxtBox().Displayed)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step5 - Log in as Test Domain Administrator.
                login.LoginIConnect(DomainAdmin, DomainAdmin);
                if (login.IsTabSelected("User Management"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step6 - Go to Role Management page by clicking on Edit button.
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectRole(Role);
                rolemanagement.ClickEditRole();
                result.steps[++ExecutedSteps].StepPass();

                //Step7 - Verify the default tools should be displayed under Toolbox configuration section..
                var groupsInUse = rolemanagement.GetGroupsInToolBoxConfig();
                bool step2_1 = groupsInUse.Count() == 12;
                bool step2_2 = rolemanagement.VerifyConfiguredToolsInToolBoxConfig(DefaultTools.Split(':'));
                if (step2_1 && step2_2)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step8 - Configure the non stacked tools for the floating toolbox by drag and dropping the tool from the Available Items section into the cells in the toolbox under toolbox configuration..
                var groups = domain.GetGroupsInToolBoxConfig();
                var dictionary = new Dictionary<String, IWebElement>();
                var step8 = false;
                foreach (IWebElement group in groups)
                {
                    var toolsinGroup = rolemanagement.GetToolsInGroupInToolBoxConfig(group);
                    if (toolsinGroup.ToArray().Length == 1)
                    {
                        AddNewTool[0] = rolemanagement.GetAvailableToolsInToolBoxConfig()[0];
                        dictionary = new Dictionary<String, IWebElement>();
                        dictionary.Add(AddNewTool[0], group);
                        domain.AddToolsToToolbox(dictionary);
                        rolemanagement.ClickSaveEditRole();
                        rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                        rolemanagement.SelectRole(Role);
                        rolemanagement.ClickEditRole();
                        BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                        step8 = rolemanagement.GetAllToolsInToolBoxConfig().Contains(AddNewTool[0]);
                        break;
                    }
                }
                if (step8)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("Failed at Configure the Non Stacked Tool or Unable to find the Non Stacked tool");
                }

                //Step9 - Place any tool in stacked tool control by drag and dropping the tool from the Available Items into the cell in the toolbox at the desired position.               
                groups = domain.GetGroupsInToolBoxConfig();
                AddNewTool[0] = rolemanagement.GetAvailableToolsInToolBoxConfig()[0];
                dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add(AddNewTool[0], groups.ElementAt(0));
                var step9_1 = domain.AddToolsToToolbox(dictionary);
                rolemanagement.ClickSaveEditRole();
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectRole(Role);
                rolemanagement.ClickEditRole();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                var step9_2 = !(domain.GetAvailableToolsInToolBoxConfig().Contains(AddNewTool[0]));
                var step9_3 = domain.GetConfiguredToolsInToolBoxConfig().Contains(AddNewTool[0]);
                if (step9_1 && step9_2 && step9_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step10 - Place the 5 tools in each cell in the toolbox by drag and dropping the tool from the Available Items into the cell in the toolbox at the desired position.
                if (domainmanagement.AddToolsToEachColumnInGroupToolBox())
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //step 11 - Place more than 5 tools in the same cell/column by drag and dropping and verify the tool being dragged should be return to its original place where it was dragged from.
                // Place 6th Tool 
                dictionary = new Dictionary<String, IWebElement>();
                groups = domain.GetGroupsInToolBoxConfig();
                var toolsToBeAdded = rolemanagement.GetAvailableToolsInToolBoxConfig()[0];
                dictionary.Add(toolsToBeAdded, groups.ElementAt(11));
                domain.AddToolsToToolbox(dictionary);
                if (domain.GetAvailableToolsInToolBoxConfig().Contains(toolsToBeAdded))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step12 - Drag some tools to Available Items section
                var ToolsToBeRemoved = new List<String>();
                ToolsToBeRemoved.Add(rolemanagement.GetToolsInToolBoxConfigByEachColumn()[0].Split(',')[0]);
                ToolsToBeRemoved.Add(rolemanagement.GetToolsInToolBoxConfigByEachColumn()[1].Split(',')[0]);
                domain.RemoveToolsFromConfiguredSection(ToolsToBeRemoved);
                bool step12_1 = rolemanagement.GetConfiguredToolsInToolBoxConfig().Contains(ToolsToBeRemoved[0]) &&
                                    rolemanagement.GetConfiguredToolsInToolBoxConfig().Contains(ToolsToBeRemoved[1]);
                bool step12_2 = rolemanagement.GetAvailableToolsInToolBoxConfig().Contains(ToolsToBeRemoved[0]) &&
                                rolemanagement.GetAvailableToolsInToolBoxConfig().Contains(ToolsToBeRemoved[1]);
                var ConfiguredTools = rolemanagement.GetConfiguredToolsInToolBoxConfig();
                var AvailableTools = rolemanagement.GetAvailableToolsInToolBoxConfig();
                if (!step12_1 && step12_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step13 - Click on "Save" button.         
                rolemanagement.ClickSaveEditRole();
                bool step13_1 = login.IsTabSelected("Role Management");
                login.Navigate("Studies");
                bool step13_2 = login.IsTabSelected("Studies");
                login.Navigate("Role Management");
                rolemanagement.SelectRole(Role);
                rolemanagement.ClickEditRole();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                bool step13_3 = domain.GetConfiguredToolsInToolBoxConfig().SequenceEqual(ConfiguredTools);
                bool step13_4 = domain.GetAvailableToolsInToolBoxConfig().SequenceEqual(AvailableTools);

                if (step13_1 && step13_2 && step13_3 && step13_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout Application	
                rolemanagement.ClickSaveEditRole();
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


    }
}