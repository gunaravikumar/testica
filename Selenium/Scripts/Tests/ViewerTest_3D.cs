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
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.Putty;
using System.Globalization;
using Selenium.Scripts.Pages.iConnect;
using System.Runtime.Serialization;
using System.Diagnostics;
using Selenium.Scripts.Pages.MergeServiceTool;

namespace Selenium.Scripts.Tests
{
	class ViewerTest_3D :BasePage
	{
		public Login login { get; set; }		
		public string filepath { get; set; }
        public WpfObjects wpfobject { get; set; }
        public ServiceTool tool { get; set; }
        public DomainManagement domainmanagement { get; set; }
        public BasePage basepage { get; set; }
        public UserPreferences userpref { get; set; }
        public ViewerTest_3D(String classname)
		{
			login = new Login();
            wpfobject = new WpfObjects();
            tool = new ServiceTool();
            domainmanagement = new DomainManagement();
            basepage = new BasePage();
            userpref = new UserPreferences();
            login.DriverGoTo(login.url);			
			filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
		}	      

        public TestCaseResult Test_Z3DConfig(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String BuildPath  = Config.BuildPath;
                String Z3dBuilds = BuildPath + "\\Installer";
                String Z3DInstaller = "Z3D_ICAinstaller.msi";
                String Z3D_AGSInstaller = "Z3D_AGSinstaller.msi";
                var psi = new ProcessStartInfo(Z3dBuilds + "\\" + Z3DInstaller);
                psi.UseShellExecute = true;
                WpfObjects._application = TestStack.White.Application.AttachOrLaunch(psi);
                WpfObjects._application.WaitWhileBusy();
                Thread.Sleep(30000);
                var psi2 = new ProcessStartInfo(Z3dBuilds + "\\" + Z3D_AGSInstaller);
                psi.UseShellExecute = true;
                WpfObjects._application = TestStack.White.Application.AttachOrLaunch(psi);
                WpfObjects._application.WaitWhileBusy();
                Thread.Sleep(30000);
                ProcessStartInfo procStartInfo = new ProcessStartInfo();
                procStartInfo.FileName = "C:\\Users\\Administrator\\Desktop\\Z3DInstall.bat";
                procStartInfo.Arguments = "";
                procStartInfo.WorkingDirectory = "C:\\Users\\Administrator\\Desktop";
                Process proc = Process.Start(procStartInfo);
                proc.WaitForExit();

                result.steps[++ExecutedSteps].status = "Pass";

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
		/// Launching 3D Studies in Viewer
		/// </summary>
		public TestCaseResult Test_3DTest(String testid, String teststeps, int stepcount)
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
                String adminDomainName = Config.adminGroupName;
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
				String LastNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastNameList");
				String[] LastName = LastNameList.Split(':');
				String[] PatientID = PatientIDList.Split(':');
                //Pre-conditions
                //Adding datasource
                tool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                tool.AddEADatasource(Config.EA91, Config.EA91AETitle, "");
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                tool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                tool.CloseConfigTool();
                //Visible all search fields
                login.LoginIConnect(adminUserName, adminPassword);
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.BluringViewerRadioBtn().Click();
                userpref.CloseUserPreferences();

                login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(adminDomainName);
                domainmanagement.SelectDomain(adminDomainName);
                domainmanagement.ClickEditDomain();
                basepage.SetReceivingInstitution(adminDomainName);
                basepage.MakeAllFieldsVisibleStudySearchFieldsDomainMgmt();
                domainmanagement.ConnectAllDataSources();
                domainmanagement.ClickSaveDomain();
                login.Logout();

                //Step 1 - Launch the application with a client browser 				
                login.DriverGoTo(login.url);
				ExecutedSteps++;

				//Step 2 - Login to WebAccess site with any privileged user.
				login.LoginIConnect(adminUserName, adminPassword);
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

				//Step 3 - Navigate to Studies tab and Search for a 3D study				
				var studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(LastName: LastName[0], Modality:"CT", Datasource: login.GetHostName(Config.EA91));
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);


                //Step 4 - Select CT study and launch it Enterprise viewer                
                studies.SelectStudy("Patient ID", PatientID[0]);
				var viewer = BluRingViewer.LaunchBluRingViewer();
				PageLoadWait.WaitForFrameLoad(50);
				PageLoadWait.WaitForPageLoad(50);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1,1);
                bool step4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1,2);
                bool step4_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + ":nth-of-type(1) " + BluRingViewer.div_Studythumbnail));

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1,3);
                viewer.OpenExamListThumbnailPreview(0);
                bool step4_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement("cssselector", viewer.GetExamListThumbnailContainerCss(1)));
                if (step4 && step4_1 && step4_2)
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

                //Step 5 - Select MR study and launch it Enterprise viewer                               
                studies.SearchStudy(patientID: PatientID[1], Modality: "MR", Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Patient ID", PatientID[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(50);
                PageLoadWait.WaitForPageLoad(50);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1,1);
                bool step5 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps +1,2);
                bool step5_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + ":nth-of-type(1) " + BluRingViewer.div_Studythumbnail));

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps +1,3);
                viewer.OpenExamListThumbnailPreview(0);
                bool step5_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement("cssselector", viewer.GetExamListThumbnailContainerCss(1)));
                if (step5 && step5_1 && step5_2)
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

                //Step 6 - Select PT study and launch it Enterprise viewer               
                studies.SearchStudy(patientID: PatientID[2], Modality: "PT", Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Patient ID", PatientID[2]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(50);
                PageLoadWait.WaitForPageLoad(50);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1,1);
                bool step6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1,2);
                bool step6_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + ":nth-of-type(1) " + BluRingViewer.div_Studythumbnail));

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1,3);
                viewer.OpenExamListThumbnailPreview(0);
                bool step6_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement("cssselector", viewer.GetExamListThumbnailContainerCss(1)));
                if (step6 && step6_1 && step6_2)
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

                //Step7 - Search for  CR study and launch it Enterprise viewer
                studies.SearchStudy(patientID: PatientID[3], Modality: "CR", Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Patient ID", PatientID[3]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(50);
                PageLoadWait.WaitForPageLoad(50);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1,1);
                bool step7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1,2);
                bool step7_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + ":nth-of-type(1) " + BluRingViewer.div_Studythumbnail));

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1,3);
                viewer.OpenExamListThumbnailPreview(0);
                bool step7_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement("cssselector", viewer.GetExamListThumbnailContainerCss(1)));
                if (step7 && step7_1 && step7_2)
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

                //Step8 - Search for  XA study and launch it Enterprise viewer
                studies.SearchStudy(patientID: PatientID[4], Modality: "XA", Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Patient ID", PatientID[4]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(50);
                PageLoadWait.WaitForPageLoad(50);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1,1);
                bool step8 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1,2);
                bool step8_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + ":nth-of-type(1) " + BluRingViewer.div_Studythumbnail));

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1,3);
                viewer.OpenExamListThumbnailPreview(0);
                bool step8_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement("cssselector", viewer.GetExamListThumbnailContainerCss(1)));
                if (step8 && step8_1 && step8_2)
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

                //Step9 - Search for  DX study and launch it Enterprise viewer
                studies.SearchStudy(patientID: PatientID[5], Modality: "DX", Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Patient ID", PatientID[5]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(50);
                PageLoadWait.WaitForPageLoad(50);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1,1);
                bool step9 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1,2);
                bool step9_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + ":nth-of-type(1) " + BluRingViewer.div_Studythumbnail));

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1,3);
                viewer.OpenExamListThumbnailPreview(0);
                bool step9_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement("cssselector", viewer.GetExamListThumbnailContainerCss(1)));
                if (step9 && step9_1 && step9_2)
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

                //Step10 - Search for  NM study and launch it Enterprise viewer
                studies.SearchStudy(patientID: PatientID[6], Modality: "NM", Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Patient ID", PatientID[6]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(50);
                PageLoadWait.WaitForPageLoad(50);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1,1);
                bool step10 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1,2);
                bool step10_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + ":nth-of-type(1) " + BluRingViewer.div_Studythumbnail));

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1,3);
                viewer.OpenExamListThumbnailPreview(0);
                bool step10_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement("cssselector", viewer.GetExamListThumbnailContainerCss(1)));
                if (step10 && step10_1 && step10_2)
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

                //Step11 - Search for  MG study and launch it Enterprise viewer
                studies.SearchStudy(patientID: PatientID[7], Modality: "MG", Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Patient ID", PatientID[7]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(50);
                PageLoadWait.WaitForPageLoad(50);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1,1);
                bool step11 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1,2);
                bool step11_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + ":nth-of-type(1) " + BluRingViewer.div_Studythumbnail));

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1,3);
                viewer.OpenExamListThumbnailPreview(0);
                bool step11_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement("cssselector", viewer.GetExamListThumbnailContainerCss(1)));
                if (step11 && step11_1 && step11_2)
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

                //Step12 - Apply Pan Tool
                bool step12 = viewer.SelectViewerTool(BluRingTools.Pan);
                viewer.ApplyTool_Pan();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step12_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement("cssselector", viewer.Activeviewport));
                if (step12 && step12_1)
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

                //Step13 - Apply Line measurement tool
                bool step13 = viewer.SelectViewerTool(BluRingTools.Line_Measurement);
                viewer.ApplyTool_LineMeasurement();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step13_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement("cssselector", viewer.Activeviewport));
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

                //Step14 - Apply Ellipse tool
                bool step14 = viewer.SelectViewerTool(BluRingTools.Draw_Ellipse);
                viewer.ApplyTool_DrawEllipse();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step14_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement("cssselector", viewer.Activeviewport));
                if (step14 && step14_1)
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

                //Step15 - Apply Flip vertical tool
                bool step15 = viewer.SelectInnerViewerTool(BluRingTools.Flip_Vertical, BluRingTools.Flip_Horizontal);                
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step15_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement("cssselector", viewer.Activeviewport));
                if (step15 && step15_1)
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

                //Step16 - Drag and drop first thumbnail from study panel to second viewport
                viewer.SetViewPort(1, 1);
                IWebElement TargetElement = viewer.GetElement("cssselector", viewer.Activeviewport);
                IWebElement sourceElement = viewer.ThumbnailIndicator(0)[0];
                Actions action = new Actions(Driver);
                action.MoveToElement(sourceElement).DragAndDrop(sourceElement, TargetElement);
                //TestCompleteAction action = new TestCompleteAction();
                //action.DragAndDrop(viewer.ThumbnailIndicator(0)[0], TargetElement);
                Logger.Instance.InfoLog("Thumbnail dragged and dropped to Viewport Successful - DragThumbnailToViewport");

                //Second viewport should be active
                IWebElement element = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport)).FindElement(By.XPath(".."));
                bool step16 = element.GetAttribute("class").Contains("activeViewportContainer selected");

                // First Thumbnail should be in Focus                
                IList<IWebElement> thumbnailsList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));
                element = thumbnailsList.ElementAt(0).FindElement(By.XPath(".."));
                bool step16_1 = element.GetAttribute("class").Contains("thumbnailImageSelected") &&
                                viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)") &&
                                element.GetCssValue("background-color").Equals("rgba(90, 170, 255, 1)");

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step16_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement("cssselector", viewer.Activeviewport));
                if (step16 && step16_1 && step16_2)
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


                //Step17 - Drag and drop second thumbnail from exam list thumbnail to first viewport
                viewer.SetViewPort(0, 1);
                TargetElement = viewer.GetElement("cssselector", viewer.Activeviewport);
                viewer.OpenExamListThumbnailPreview(0);
                thumbnailsList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages));
               // action = new TestCompleteAction();
                action.DragAndDrop(thumbnailsList[1], TargetElement);
                Logger.Instance.InfoLog("Thumbnail dragged and dropped to Viewport Successful - DragThumbnailToViewport");

                //First viewport should be active
                element = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport)).FindElement(By.XPath(".."));
                bool step17 = element.GetAttribute("class").Contains("activeViewportContainer selected");

                // Second Thumbnail should be in Focus                
                thumbnailsList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages));
                element = thumbnailsList.ElementAt(0).FindElement(By.XPath(".."));
                bool step17_1 = element.GetAttribute("class").Contains("thumbnailImageSelected") &&
                                viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)") &&
                                element.GetCssValue("background-color").Equals("rgba(90, 170, 255, 1)");

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step17_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement("cssselector", viewer.Activeviewport));
                if (step17 && step17_1 && step17_2)
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
	}
}
