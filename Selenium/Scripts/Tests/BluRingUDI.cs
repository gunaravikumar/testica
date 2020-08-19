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
using Selenium.Scripts.Pages.MergeServiceTool;
using Microsoft.Win32;
using System.Diagnostics;


namespace Selenium.Scripts.Tests
{
	class BluRingUDI
	{
		public Login login { get; set; }
		public Configure configure { get; set; }
		public HPHomePage hphomepage { get; set; }
		public string filepath { get; set; }
		public ServiceTool servicetool { get; set; }

		public BluRingUDI(String classname)
		{
			login = new Login();
			login.DriverGoTo(login.url);
			configure = new Configure();
			hphomepage = new HPHomePage();
			servicetool = new ServiceTool();
			filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
		}

		/// <summary>
		/// Verify that About box displays UDI Device Identifer, Build Info and Manufacture date
		/// </summary>
		public TestCaseResult Test_161541(String testid, String teststeps, int stepcount)
		{

			//Declare and initialize variables         
			TestCaseResult result = null;
			int ExecutedSteps = -1;

			try
			{
				result = new TestCaseResult(stepcount);
				result.SetTestStepDescription(teststeps);
				
				//Fetch required Test data  
				String adminUserName = Config.adminUserName;
				String adminPassword = Config.adminPassword;
				String DeviceIdentifierNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Device Identifier");

				//Step 1 - Login as Administrator
				login.DriverGoTo(login.url);
				if (login.LoginStylesheetLink().GetAttribute("href").Contains(Config.buildversion))
				{
					login.LoginIConnect(adminUserName, adminPassword);
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				
				//step2 - Click on Help icon
				bool helpmenu = login.Verify_HelpMenu();
				if (helpmenu)
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//step3 - Click on About iConnect(r) Access and verifying the title, version and merge logo
				login.OpenHelpAboutSplashScreen();
				string title = login.GetElementAttribute("cssselector", "#HelpAboutDiv div:nth-of-type(1) p[style*='text-align']", "innerHTML");
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 29);
				bool mergelogo = login.CompareImage(result.steps[ExecutedSteps], BasePage.Driver.FindElement(By.CssSelector("div.whiteRounded div:nth-of-type(1) img[alt*='Merge']")));
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 30);
				bool version = login.CompareImage(result.steps[ExecutedSteps], BasePage.Driver.FindElement(By.CssSelector("div.whiteRounded div:nth-of-type(3) img[alt*='iConnect® Access']")));
				string company = login.GetElementAttribute("cssselector", "#HelpAboutDiv div:nth-of-type(3) p[style*='text-align']", "innerHTML");
				if (title.Equals("About IBM iConnect® Access") && mergelogo && version && company.Equals("IBM") )
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

				//step4 - Verify the UDI text has DI(Device Identifier) followed by number (01) and Build Info followed by number (10) and Build Manufacture date followed by (11)

				//Get UDI text
				String UDItext = login.UDIText().Text.Trim();

				//Verify UDI is displayed in About iConnect Access splash screen as **UDI:(01)00842000100126(10)7.0.0.258(11)170811.**
				if (login.UDIText().Displayed && UDItext.StartsWith("UDI:(01)") && UDItext.Contains("(10)" + Config.buildversion)
					&& UDItext.Contains("(11)"))
				{
					//Get Build Date 
					String buildDate = BasePage.GetBuildDetails()["Date"];
					DateTime Date = DateTime.ParseExact(buildDate.Split(new String[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries)[1], "mm/dd/yyyy", System.Globalization.CultureInfo.CurrentUICulture);
					String BuildDate = Date.ToString("yymmdd");//Date.Year.ToString().Replace("20", String.Empty) + Date.Month.ToString() + Date.Day;

					//Get Build number 
					string buildno =login.GetBuildID();

					//Verify UDI is displayed in About iConnect Access splash screen with correct details
					if (UDItext.StartsWith("UDI:(01)" + DeviceIdentifierNo) && UDItext.Contains("(10)" + Config.buildversion + "." + buildno)
						&& UDItext.EndsWith("(11)" + BuildDate))
					{
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
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Close the About iConnect Access
				login.CloseHelpAboutSplashScreen();
				if (!login.IsElementVisible(login.By_HelpWebAccessMergeLogo))
				{
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
        /// Verify that Login Splash page shows correct version of the application
        /// </summary>
        
        public TestCaseResult Test_161543(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            try
            {
                //Set up Validation Steps
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');
                
                //Fetch required Test data  
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;

                //Step 1 - Login as Administrator and ensure that Brand application name
                login.LoginIConnect(adminusername, adminpassword);               
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 20);
                var brandlogo = login.CompareImage(result.steps[ExecutedSteps], BasePage.Driver.FindElement(login.By_Brandlogo));
                if (brandlogo)
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
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0]);
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 21);
                var viewscreenbrandlogo = login.CompareImage(result.steps[ExecutedSteps], BasePage.Driver.FindElement(login.By_viewerscreenMergeLogo));
                if (viewscreenbrandlogo)
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

                //step 2 Logout of the web application. Verify the version number in Login splash page.
                viewer.CloseBluRingViewer();
                login.Logout();
                BasePage.Driver.SwitchTo().DefaultContent();
                if (login.LoginStylesheetLink().GetAttribute("href").Contains(Config.buildversion) && BasePage.Driver.Title.Contains("IBM iConnect® Access"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step3 - Login to tablet and verify the login page splash screen
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Return Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
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
		/// Verify that all programs and services installed should show correct version and name
		/// </summary>
		public TestCaseResult Test_161539(String testid, String teststeps, int stepcount)
		{

			//Declare and initialize variables         
			TestCaseResult result = null;
			int ExecutedSteps = -1;
			string line;
			string version = "";
			int i = 2;

			try
			{
				result = new TestCaseResult(stepcount);
				result.SetTestStepDescription(teststeps);

				//Step 1 - On the application server machine, click WebAccess\Setup.exe(in any mode) to install the application. Review the splash page to install the application.
				result.steps[++ExecutedSteps].status = "Not Automated";

				//step2 - Open the Service Tool application. Ensure the name on the window title of the application is appropriate.
				servicetool.LaunchServiceTool();
				servicetool.CloseServiceTool();
				ExecutedSteps++;

				//step3 - Navigate to the installation folder (i.e. C:\WebAccess ). Ensure there are no reference to any old version of the application
				using (StreamReader sr = new StreamReader("C:\\WebAccess\\UDI.txt"))
				{
					while ((line = sr.ReadLine()) != null)
					{
						if (line.Contains("Version")) { version = line; break; }
					}
					version = version.Split(':')[1].Trim();
				}
				if (version.Equals(Config.buildversion))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//step4 - Open regedit (the Registry). Navigate to HKEY_LOCAL_MACHINE &gt; SOFTWARE &gt; Wow6432Node &gt; Cedara &gt; WebAccess. Verify the registry items for the application.

				//Getting host name
				string hostname = login.GetHostName(Config.IConnectIP);

				version = (string)Registry.GetValue(Registry.LocalMachine + @"\SOFTWARE\Wow6432Node\Cedara\WebAccess", "Version", null) ?? string.Empty;
				string productcode = (string)Registry.GetValue(Registry.LocalMachine + @"\SOFTWARE\Wow6432Node\Cedara\WebAccess", "ProductCode", null) ?? string.Empty;
				string instancename = (string)Registry.GetValue(Registry.LocalMachine + @"\SOFTWARE\Wow6432Node\Cedara\WebAccess", "InstanceName", null) ?? string.Empty;
				string installdir = (string)Registry.GetValue(Registry.LocalMachine + @"\SOFTWARE\Wow6432Node\Cedara\WebAccess", "InstallDir", null) ?? string.Empty;
				if (Config.buildversion.Contains(version) && productcode.Equals("{72ADFA5C-EAF7-4CCD-930B-7CBC7D486738}") && instancename.Equals(hostname + "\\WEBACCESS") && installdir.Equals("C:\\WebAccess\\"))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//step-5 - Go to Control Panel &gt; Program and Features &gt; Uninstall or change a program. Verify the application name is appropriate.
				if (File.Exists(@"c:\Installlist.txt"))
				{
					File.Delete(@"c:\Installlist.txt");
				}
				string strcmd = @"wmic  > C:\InstallList.txt product get name,version";
				System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + strcmd);
				procStartInfo.RedirectStandardOutput = true;
				procStartInfo.UseShellExecute = false;
				procStartInfo.CreateNoWindow = true;
				System.Diagnostics.Process proc = new System.Diagnostics.Process();
				proc.StartInfo = procStartInfo;
				proc.Start();
				proc.WaitForExit();

				using (StreamReader sr = new StreamReader("C:\\Installlist.txt"))
				{
					while ((line = sr.ReadLine()) != null && i > 0)
					{
						if (line.Contains("IBM iConnect Access Service Tool") || line.Contains("IBM iConnect Access"))
						{
							if (line.Contains("7.0"))
								i--;
						}
					}
				}
				if (i == 0)
				{
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
		}

		/// <summary>  
		/// Verify that Online Help page show correct version of application  
		/// </summary>  
		public TestCaseResult Test_161540(String testid, String teststeps, int stepcount)
		{
			//Declare and initialize variables          
			TestCaseResult result = null;
			UserManagement usermanagement = null;
			DomainManagement domainmanagement = null;
			int ExecutedSteps = -1;
			try
			{
				result = new TestCaseResult(stepcount);
				result.SetTestStepDescription(teststeps);
				String adminUserName = Config.adminUserName;
				String adminPassword = Config.adminPassword;
				String UserID = "User" + new Random().Next(1000);
				String LastName = "LastName" + new Random().Next(1000);
				String FirstName = "FirstName" + new Random().Next(1000);
				String UserPassword = "UserPassword" + new Random().Next(1000);
				String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
				String[] Accession = AccessionList.Split(':');

				// Step 1 - Verifying application version in login page  
				var step1 = login.LoginStylesheetLink().GetAttribute("href").Contains(Config.buildversion);
				String step1_1 = BasePage.Driver.Title;
				if (step1 && step1_1.Equals("IBM iConnect® Access"))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step 2 - Login as administrator  
				login.LoginIConnect(adminUserName, adminPassword);

				//Open About iConnect Access splash screen  
				login.OpenHelpAboutSplashScreen();
				var step2 = login.HelpWebAccessLoginLogo().GetAttribute("src").Contains(Config.buildversion);
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
				var step2_2 = login.CompareImage(result.steps[ExecutedSteps], login.GetElement(BasePage.SelectorType.CssSelector, ".whiteRounded img[alt='IBM iConnect® Access']"));
				if (step2 && step2_2)
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

				//Close About iConnect Access splash screen  
				login.CloseHelpAboutSplashScreen();

                // step 3 - verifying Application name and build version in Contents
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer"))
                {
                    login.CloseBrowser();
                    login.ClearBrowserCache();
                    login.DriverGoTo(login.url);
                    login.LoginIConnect(adminUserName, adminPassword);
                }
                var windows = BasePage.Driver.WindowHandles;
				OnlineHelp onlinehelp = new OnlineHelp().OpenHelpandSwitchtoIT(0);
				onlinehelp.NavigateToOnlineHelpFrame("topic");
				BasePage.wait.Until(ExpectedConditions.ElementExists(onlinehelp.By_OnlineHelpVersion));
				var step3_1 = onlinehelp.OnlineHelpVersion().Text.Contains(Config.buildversion.Remove(3));
				var step3_2 = BasePage.Driver.FindElement(onlinehelp.By_ProductName).GetAttribute("innerHTML").Contains("iConnect® Access");

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

				//Close Help Tool  
				BasePage.Driver.Close();
				BasePage.Driver.SwitchTo().Window(windows[0]);
				PageLoadWait.WaitForFrameLoad(20);

				// step 4 - Verifying Merge logo and Application name in Studies page and Viewer page.
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");				
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                var step4_1 = login.CompareImage(result.steps[ExecutedSteps], login.GetElement(BasePage.SelectorType.CssSelector, "#LogoDiv"));

                var studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(AccessionNo: Accession[0]);
				studies.SelectStudy("Accession", Accession[0]);
				var viewer = BluRingViewer.LaunchBluRingViewer();
				PageLoadWait.WaitForFrameLoad(20);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                var step4_2 = login.CompareImage(result.steps[ExecutedSteps], BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_mergeLogo)), totalImageCount: 2, IsFinal: 1);
                if (step4_1 && step4_2)
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

				// Closing the bluring viewer
				viewer.CloseBluRingViewer();

				// Creating a domain
				domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
				Dictionary<Object, String> createDomain = domainmanagement.CreateDomainAttr();
				domainmanagement.CreateDomain(createDomain);
				string DomainName = createDomain[DomainManagement.DomainAttr.DomainName];
				string RoleName = createDomain[DomainManagement.DomainAttr.RoleName];

				// Creating a non administrative user
				usermanagement = (UserManagement)login.Navigate("UserManagement");
				usermanagement.SelectDomainFromDropdownList(DomainName);
				usermanagement.CreateUser(UserID, DomainName, RoleName, 0, "", 1, UserPassword);

				//Logout                  
				login.Logout();

				//Step 5 - Login as non administrator user  
				login.LoginIConnect(UserID, UserPassword);

				//Open About iConnect Access splash screen  
				login.OpenHelpAboutSplashScreen();
				var step5_1 = login.HelpWebAccessLoginLogo().GetAttribute("src").Contains(Config.buildversion);
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
				var step5_2 = login.CompareImage(result.steps[ExecutedSteps], login.GetElement(BasePage.SelectorType.CssSelector, ".whiteRounded img[alt='IBM iConnect® Access']"));
				if (step5_1 && step5_2)
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

				//Close About iConnect Access splash screen  
				login.CloseHelpAboutSplashScreen();

				// step 6 - verifying Application name and build version in Contents    
				onlinehelp.OpenHelpandSwitchtoIT(0);
				onlinehelp.NavigateToOnlineHelpFrame("topic");
				BasePage.wait.Until(ExpectedConditions.ElementExists(onlinehelp.By_OnlineHelpVersion));
				var step6_1 = onlinehelp.OnlineHelpVersion().Text.Contains(Config.buildversion.Remove(3));
				var step6_2 = BasePage.Driver.FindElement(onlinehelp.By_ProductName).GetAttribute("innerHTML").Contains("iConnect® Access");

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

				//Close Help Tool  
				BasePage.Driver.Close();
				BasePage.Driver.SwitchTo().Window(windows[0]);
				PageLoadWait.WaitForFrameLoad(20);

				// step 7 - Verifying Merge logo and Application name  
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");				
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                var step7_1 = login.CompareImage(result.steps[ExecutedSteps], login.GetElement(BasePage.SelectorType.CssSelector, "#LogoDiv"));

				studies.SearchStudy(AccessionNo: Accession[0]);
				studies.SelectStudy("Accession", Accession[0]);
				BluRingViewer.LaunchBluRingViewer();
				PageLoadWait.WaitForFrameLoad(20);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                var step7_2 = login.CompareImage(result.steps[ExecutedSteps], BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_mergeLogo)), totalImageCount: 2, IsFinal: 1);
                if (step7_1 && step7_2)
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

				// Closing the bluring viewer
				viewer.CloseBluRingViewer(); 

				// Step 8 - Logout and verify Application Name and version in Login page              
				login.Logout();
                BasePage.Driver.SwitchTo().DefaultContent();
                var step8 = login.LoginStylesheetLink().GetAttribute("href").Contains(Config.buildversion);
				String step8_1 = BasePage.Driver.Title;
				if (step8 && step8_1.Equals("IBM iConnect® Access"))
				{
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
		}

		/// <summary>  
		/// Verify that About dialog Splash Screen shows correct Version of application  
		/// </summary>  
		public TestCaseResult Test_161542(String testid, String teststeps, int stepcount)
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

				// Step 1 & 2 - Launch the Bluring Application and  Verifying application version in login page  
				var step2_1 = login.LoginStylesheetLink().GetAttribute("href").Contains(Config.buildversion);
				ExecutedSteps++;
				String step2_2 = BasePage.Driver.Title;
				if (step2_1 && step2_2.Equals("IBM iConnect® Access"))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step 3 - Login as administrator  
				login.LoginIConnect(adminUserName, adminPassword);
				ExecutedSteps++;

				//Step 4 - Open About iConnect Access splash screen    
				login.OpenHelpAboutSplashScreen();
				ExecutedSteps++;

				//Step 5 - Verifying Application name and version in About iConnect Access splash screen  
				var step5_1 = login.HelpWebAccessLoginLogo().GetAttribute("src").Contains(Config.buildversion);
				var step5_2 = login.HelpWebAccessLoginLogo().GetAttribute("alt").Contains("iConnect® Access");
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

				//Step 6 - Close About iConnect Access splash screen  
				login.CloseHelpAboutSplashScreen();
				var step6 = login.IsElementVisible(login.By_AboutIConnectAccessIcon);
				if (!step6)
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				// Step 7 - Logout from the Application                
				login.Logout();
				ExecutedSteps++;

                //Step 8 - Verifying application version in login page
                BasePage.Driver.SwitchTo().DefaultContent();
                var step8 = login.LoginStylesheetLink().GetAttribute("href").Contains(Config.buildversion);
				String step8_1 = BasePage.Driver.Title;
				if (step8 && step8_1.Equals("IBM iConnect® Access"))
				{
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
		}
	}
}
