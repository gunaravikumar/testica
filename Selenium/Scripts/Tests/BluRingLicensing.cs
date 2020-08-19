using OpenQA.Selenium;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MergeServiceTool;
using Selenium.Scripts.Reusable.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestStack.White.UIItems;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Xml;
using Selenium.Scripts.Pages.eHR;
using Selenium.Scripts.Pages.iCN;
using System.Threading;
using System.Diagnostics;
using System.Data;

namespace Selenium.Scripts.Tests
{
    class BluRingLicensing
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public ServiceTool servicetool { get; set; }
        public EHR ehr { get; set; }
        public BluRingViewer bluringviewer { get; set; }


        public BluRingLicensing(String classname)
        {
            login = new Login();
			servicetool = new ServiceTool();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            ehr = new EHR();
        }

        /// <summary>
        /// Install and verify that Pre-Release License is supported
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161179(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            int ExecutedSteps = -1;
            WpfObjects wpfobject = new WpfObjects();
            
            result = new TestCaseResult(stepcount);                        

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                //Fetch required Test data                       
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;

                //Step 1, 2 Open Service Tool and Navigate to License Tab
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                ExecutedSteps += 2;

				//Current Directory path				
				String licenseFileDirectory = System.IO.Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "ServerConfigFiles" + Path.DirectorySeparatorChar + login.GetHostName(Config.IConnectIP);				
				//Config Files - File Path
				String licensePath = licenseFileDirectory + Path.DirectorySeparatorChar + "PreReleaseLicense" + Path.DirectorySeparatorChar + "BluRingLicense.xml";

				// Step 3 - Navigate to License Tab and Import license
				servicetool.AddLicenseInServiceTool(licensePath);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                // Step 4 - Verifying columns in the service tool under licence tab
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                ListView table = wpfobject.GetTable("ListView");                                             
                if (table.Header.Columns[0].ToString().ToLower().Contains("key") && table.Header.Columns[1].ToString().ToLower().Contains("version") &&
                    table.Header.Columns[2].ToString().ToLower().Contains("number of license") && table.Header.Columns[3].ToString().ToLower().Contains("days remaining") &&
                    table.Header.Columns[4].ToString().ToLower().Contains("mac address") && table.Rows.Count == 12)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }               
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }                

                //Step 5 - Restart IIS 
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Step 6 - Login as Administrator
                login.DriverGoTo(login.url);
                login.LoginIConnect(Username, Password);
                ExecutedSteps++;

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
        ///Verify that "Not for Clinical Use" font size can be changed
        /// </summary>
        public TestCaseResult Test_161182(String testid, String teststeps, int stepcount)
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
               

                // Step 1 - Login to ICA as an Administrator
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //step 2,3 - select study and launch the BR viewer
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;
                ExecutedSteps++;

                //step 4 - Verify that the "Not for Clinical Use" warning message with the Warning icon is displayed
                var step4 = false;
                step4 = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_LicenseWarningandMedium)).GetAttribute("innerHTML").Equals("Not for Clinical Use");
                bool step_4 = viewer.IsElementVisible((By.CssSelector(BluRingViewer.div_Warningsymbol)));
                if (step4 && step_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
              
                // step 5 ,6 - Click on the User Setting and select UI-Large, UI-Medium and UI-Small then verify that Warning Message "Not for Clinical Use" font size   
                var step5 =viewer.UserSettings("select", "LARGE");
                ExecutedSteps++;
                bool step6_1 = false;
                bool step6_2 = false;
                bool step6_3 = false;

                if ((!viewer.IsElementVisible(By.CssSelector(BluRingViewer.div_LicenseWarningandMedium))) && (!viewer.IsElementVisible(By.CssSelector(BluRingViewer.div_Licenseiconsmall))))
                {
                    step6_1 = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_LicenseiconLarge)).GetAttribute("innerHTML").Equals("Not for Clinical Use");
                }

                viewer.UserSettings("select", "MEDIUM");
                if ((!viewer.IsElementVisible(By.CssSelector(BluRingViewer.div_LicenseiconLarge))) && (!viewer.IsElementVisible(By.CssSelector(BluRingViewer.div_Licenseiconsmall))))
                {
                    step6_2 = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_LicenseWarningandMedium)).GetAttribute("innerHTML").Equals("Not for Clinical Use");
                }

                viewer.UserSettings("select", "SMALL");
                if ((!viewer.IsElementVisible(By.CssSelector(BluRingViewer.div_LicenseiconLarge))) && (!viewer.IsElementVisible(By.CssSelector(BluRingViewer.div_LicenseWarningandMedium))))
                {
                    step6_3 = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_Licenseiconsmall)).GetAttribute("innerHTML").Equals("Not for Clinical Use");
                }
                if (step5 && step6_1 && step6_2 && step6_3)
                {
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
        ///Verify that “Not for Clinical Use” is NOT displayed when launching BRViewer with a standard License
        /// </summary>
        public TestCaseResult Test_161181(String testid, String teststeps, int stepcount)
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
                //string Onebase0adminpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "1base0adminpath");
                //string Onebase0adminpath = "C:\\Users\\Administrator\\Desktop\\BluRingLicense.xml";
                //Config file Directory Path
                String ConfigFileDirectory = System.IO.Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "ServerConfigFiles" + Path.DirectorySeparatorChar + login.GetHostName(Config.IConnectIP);
                //Config Files - File Path
                String License_Backup = ConfigFileDirectory + Path.DirectorySeparatorChar + "BluRingLicense.xml";

                // Precondition install standard license
                ServiceTool servicetool = new ServiceTool();
                servicetool.InvokeServiceTool();
                WpfObjects wpfobject = new WpfObjects();
                wpfobject.WaitTillLoad();
                servicetool.AddLicenseInServiceTool(License_Backup);
                //servicetool.AddLicenseInServiceTool(Onebase0adminpath);


                // Restart IIS 
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
               
                // Step 1 - Login to ICA as an Administrator
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //Step 2 - Open User Preferences and select Bluring as a default
                UserPreferences userPrefer = new UserPreferences();
                userPrefer.OpenUserPreferences();
                userPrefer.SwitchToUserPrefFrame();
                userPrefer.BluringViewerRadioBtn().Click();
                userPrefer.CloseUserPreferences();
                ExecutedSteps++;

                //step 3 - select study and verfiy that "Not for Clinical Use" is not displayed in BluRing Viewer
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0]);
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                if (!viewer.IsElementPresent(By.CssSelector(BluRingViewer.div_prereleaseWarningicon)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 4 -Clcik on the "X" exit button to close the screen
                viewer.CloseBluRingViewer();
                Thread.Sleep(5000);
                if (!viewer.IsElementPresent(By.CssSelector(BluRingViewer.div_ShowHideTool)))
                {
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
		/// Verify that “Not for Clinical Use” is displayed when launching BR Viewer with a Pre-Rel license
		/// </summary>
		public TestCaseResult Test_161180(String testid, String teststeps, int stepcount)
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

				// Precondition:
				// Pre Release License is Imported in the test 138131

				//Step 1 - Login as Administrator 
				login.LoginIConnect(adminUserName, adminPassword);
				ExecutedSteps++;

				//Step 2 - Navigate to Search                
				var studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
				ExecutedSteps++;

				//Step 3 - Load study in BluRing Viewer
				studies.SelectStudy("Accession", Accession[0]);
				var viewer = BluRingViewer.LaunchBluRingViewer();
				PageLoadWait.WaitForFrameLoad(20);
				BluRingViewer.WaitforViewports();

				// Verify 'Not for Clinical Use' warning message
				var step3 = viewer.GetElement("cssselector", BluRingViewer.div_clinicalRelease).GetAttribute("innerHTML").Contains("Not for Clinical Use");
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

				//Close the bluring viewer
				viewer.CloseBluRingViewer();

				// Change default viewer as HTML4 viewer
				UserPreferences userPrefer = new UserPreferences();
				userPrefer.OpenUserPreferences();
				userPrefer.SwitchToUserPrefFrame();
				userPrefer.HTML4RadioBtn().Click();
				userPrefer.CloseUserPreferences();

				//Step 4 - Search for another study 
				studies.SearchStudy(AccessionNo: Accession[1], Datasource: login.GetHostName(Config.EA91));
				ExecutedSteps++;

				//Step 5 -  Load study in HTML4 Viewer
				studies.SelectStudy("Accession", Accession[1]);
				StudyViewer studyViewer = new Studies().LaunchStudy();

				// Verify 'Not for Clinical Use' warning message 
				var step5 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_clinicalRelease)).Count == 0;					
				if (step5)
				{
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
				studyViewer.CloseStudy();				

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
				// Change default viewer as HTML4 viewer
				UserPreferences userPrefer = new UserPreferences();
				userPrefer.OpenUserPreferences();
				userPrefer.SwitchToUserPrefFrame();
				userPrefer.BluringViewerRadioBtn().Click();
				userPrefer.CloseUserPreferences();
			}
		}

        /// <summary>
		/// Verifying the contents of the license - from Service tool and text editor
		/// </summary>
		public TestCaseResult Test_161183(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            WpfObjects wpfobject = new WpfObjects();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                
                //Config file Directory Path
                String ConfigFileDirectory = System.IO.Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "ServerConfigFiles" + Path.DirectorySeparatorChar + login.GetHostName(Config.IConnectIP);
                //Config Files - File Path
                String licencePath = ConfigFileDirectory + Path.DirectorySeparatorChar + "BluRingLicense.xml";
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");

                // Step 1 - Read and verify the Bluring Licence xml
                var licenceDoc = new XmlDocument();
                licenceDoc.Load(licencePath);
                var nodevalues = licenceDoc.SelectNodes("/License");
                int i = 0;
                String[] licenceComments = new String[100];
                foreach (XmlNode member in nodevalues)
                {
                    foreach (var child in member.ChildNodes)
                    {
                        if (i == 24)
                        {
                            break;
                        }
                        else
                        {
                            if (child is XmlComment)
                            {
                                XmlComment element = (XmlComment)child;
                                string nodeName = element.Name;
                                String value = element.InnerText;

                                licenceComments[i] = value;
                                i++;
                            }                            
                        }
                    }
                }

                int h = 0;
                String[] featuresName = new string[18];

                for (int s = 5; s <= 22; s++)
                {
                    String featurName = licenceComments[s].Substring(14);
                    featuresName[h] = featurName;
                    h++;
                }

                //String[] WebAccess3d = featuresName[12].Split(',');
                //String webAccess3dVersion = WebAccess3d[1].Substring(9);
                //String webAccess3dLicence = WebAccess3d[2].Substring(17);

                String[] WebAccessBase = featuresName[12].Split(',');
                String webAccessBaseVersion = WebAccessBase[1].Substring(9);
                String webAccessBaseLicence = WebAccessBase[2].Substring(17);

                String[] WebAccessAdmin = featuresName[13].Split(',');
                String webAccessAdminVersion = WebAccessAdmin[1].Substring(9);
                String webAccessAdminLicence = WebAccessAdmin[2].Substring(17);

                String[] WebAccessIpod = featuresName[14].Split(',');
                String webAccessIpodVersion = WebAccessIpod[1].Substring(9);
                String webAccessIpodLicence = WebAccessIpod[2].Substring(17);

                String[] WebAccessBlackBerry = featuresName[15].Split(',');
                String webAccessBlackBerryVersion = WebAccessBlackBerry[1].Substring(9);
                String webAccessBlackBerryLicence = WebAccessBlackBerry[2].Substring(17);

                String[] WebAccessMaxUsers1 = featuresName[16].Split(',');
                String webAccessMaxUsers1Version = WebAccessMaxUsers1[1].Substring(9);
                String webAccessMaxUsers1Licence = WebAccessMaxUsers1[2].Substring(17);

                String[] WebAccessMaxUsers2 = featuresName[17].Split(',');
                String webAccessMaxUsers2Version = WebAccessMaxUsers2[1].Substring(9);
                String webAccessMaxUsers2Licence = WebAccessMaxUsers2[2].Substring(17);

                String macAddresses = null;
                foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    //if (nic.OperationalStatus == OperationalStatus.Up)
                    {
                        macAddresses = nic.GetPhysicalAddress().ToString();
                        break;
                    }
                }

                String macAddressInLicence = licenceComments[0].Substring(12);
                Logger.Instance.InfoLog("The MAC address in the Licenece is "+ macAddressInLicence + " and the MAC address of the system is " + macAddresses);
                var systemName = System.Environment.MachineName;
                String systemNameInLicence = licenceComments[1].Substring(10);
                Logger.Instance.InfoLog("The system name in the Licenece is " + systemNameInLicence + " and the system name is " + systemName);
                Logger.Instance.InfoLog("The duration in licence is " + licenceComments[4]);
                var step1 = macAddressInLicence.Equals(macAddresses) &&
                            systemNameInLicence.Equals(systemName) && (licenceComments[4].Equals("duration 1Y"));
                if (step1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step 2 - Verify the Features present in the Bluring licence xml file
				Logger.Instance.InfoLog("Ica image share feature in licence is " + licenceComments[7]);
                Logger.Instance.InfoLog("Ica xds feature in licence is " + licenceComments[8]);
                Logger.Instance.InfoLog("Ica base 3d feature in licence is " + licenceComments[9]);
                Logger.Instance.InfoLog("webaccess 3d feature in licence is " + licenceComments[17]);
                Logger.Instance.InfoLog("Ica coronary feature in licence is " + licenceComments[10]);
                Logger.Instance.InfoLog("Ica ortho case feature in licence is " + licenceComments[11]);
                var step2 = licenceComments[7].Equals("Feature: Name=ica.image.sharing") && licenceComments[8].Equals("Feature: Name=ica.xds") && licenceComments[9].Equals("Feature: Name=ica.base.3d") &&
                            /*licenceComments[17].Equals("Feature: Name=webaccess.3d, Version=1.0, NumberOfLicense=4") &&*/ licenceComments[10].Equals("Feature: Name=ica.ct.coronary") && licenceComments[11].Equals("Feature: Name=ica.ortho.case");
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

                //Step 3 & 4 - Launch service tool and import the Licence and verify the contents in the service tool with the bluring licence file
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToTab("License");
                wpfobject.WaitTillLoad();
                servicetool.AddLicenseInServiceTool(licencePath);
                wpfobject.WaitTillLoad();
                servicetool.RestartIIS();
                wpfobject.WaitTillLoad();
                ExecutedSteps++;
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                ListView table = wpfobject.GetTable("ListView");
                int rowCount = table.Rows.Count;                
                var expiryDates = licenceComments[3].Split(' ');
                var expiryDate = expiryDates[2] + " " + expiryDates[3] + " " + expiryDates[4];
                Logger.Instance.InfoLog("The expiry date in licence is" + expiryDate);
                DateTime endDate = Convert.ToDateTime(expiryDate);
                DateTime startDate = DateTime.Now;
                TimeSpan daysLeft = endDate - startDate;
                String daysRemaining = daysLeft.Days.ToString();
                Logger.Instance.InfoLog("The no. of days remaining is " + daysRemaining);

                bool step4_1 = true;
                for (int d = 0; d < rowCount; d++)
                {

                    String macValue = table.Rows[d].Cells[4].Text;
                    if (!(macValue.Equals(macAddressInLicence)))
                    {
                        step4_1 = false;
                        Logger.Instance.InfoLog("The macaddress in licence is "+ macAddressInLicence + " and its not matching with mac Address with the mac address in service tool"+ macValue);
                    }
                }
                if(step4_1)
                {
                    Logger.Instance.InfoLog("Step4_1 Pass");
                }

                bool step4_2 = true;
                for (int d = 0; d < rowCount; d++)
                {

                    String daysRemainingInServiceTool = table.Rows[d].Cells[3].Text;
                    if (!(daysRemainingInServiceTool.Equals(daysRemaining)))
                    {
                        step4_2 = false;
						Logger.Instance.InfoLog("The no. of days remaining calculated is  " + daysRemaining + " and its not matching with no. of days remaining in service tool" + daysRemainingInServiceTool);
                    }                   
                }
                if (step4_2)
                {
                    Logger.Instance.InfoLog("Step4_2 Pass");
                }

                bool step4_3 = true;
                int r = 0;
                String[] keysInServiceTool = new String[rowCount];
                for (int d = 0; d < rowCount; d++)
                {
                    String keyInServiceTool = table.Rows[d].Cells[0].Text;
                    keysInServiceTool[r] = keyInServiceTool;
                    r++;
                    if (d < 12)
                        if (!(keyInServiceTool.Equals(featuresName[d])))
                        {
                            step4_3 = false;
                            Logger.Instance.InfoLog("The keysin service tool is not matching");
                        }                      
                }
                if (step4_3)
                {
                    Logger.Instance.InfoLog("Step4_3 Pass");
                }

                var step4_4 = /*keysInServiceTool[12].Equals(WebAccess3d[0]) &&*/ keysInServiceTool[12].Equals(WebAccessBase[0]) &&
                              keysInServiceTool[13].Equals(WebAccessAdmin[0]) && keysInServiceTool[14].Equals(WebAccessIpod[0]) &&
                              keysInServiceTool[15].Equals(WebAccessBlackBerry[0]) && keysInServiceTool[16].Equals(WebAccessMaxUsers1[0]) &&
                              keysInServiceTool[17].Equals(WebAccessMaxUsers2[0]);
                if(step4_4)
                {
                    Logger.Instance.InfoLog("Step4_4 Pass");
                }

                int e = 0;
                String[] versionInServiceTool = new String[6];
                for (int d = 12; d < rowCount; d++)
                {
                    String versioninServiceTool = table.Rows[d].Cells[1].Text;
                    versionInServiceTool[e] = versioninServiceTool;
                    e++;
                }

                var step4_5 = /*versionInServiceTool[0].Equals(webAccess3dVersion) &&*/ versionInServiceTool[0].Equals(webAccessBaseVersion) &&
                              versionInServiceTool[1].Equals(webAccessAdminVersion) && versionInServiceTool[2].Equals(webAccessIpodVersion) &&
                              versionInServiceTool[3].Equals(webAccessBlackBerryVersion) && versionInServiceTool[4].Equals(webAccessMaxUsers1Version) &&
                              versionInServiceTool[5].Equals(webAccessMaxUsers2Version);
                if(step4_5)
                {
                    Logger.Instance.InfoLog("Step4_5 Pass");
                }
                int f = 0;
                String[] numberOfLicenceInServiceTool = new String[7];
                for (int d = 12; d < rowCount; d++)
                {
                    String numberOfLicenceinServiceTool = table.Rows[d].Cells[2].Text;
                    numberOfLicenceInServiceTool[f] = numberOfLicenceinServiceTool;
                    f++;
                }

                var step4_6 = /*numberOfLicenceInServiceTool[0].Equals(webAccess3dLicence) &&*/ numberOfLicenceInServiceTool[0].Equals(webAccessBaseLicence) &&
                              numberOfLicenceInServiceTool[1].Equals(webAccessAdminLicence) && numberOfLicenceInServiceTool[2].Equals(webAccessIpodLicence) &&
                              numberOfLicenceInServiceTool[3].Equals(webAccessBlackBerryLicence) && numberOfLicenceInServiceTool[4].Equals(webAccessMaxUsers1Licence) &&
                              numberOfLicenceInServiceTool[5].Equals(webAccessMaxUsers2Licence);
                if(step4_6)
                {
                    Logger.Instance.InfoLog("Step4_6 Pass");
                }

                bool step4_7 = true;

                versionInServiceTool = new String[19];
                for (int d = 0; d < rowCount; d++)
                {
                    String versioninServiceTool = table.Rows[d].Cells[1].Text;
                    versionInServiceTool[d] = versioninServiceTool;                   
                }

                numberOfLicenceInServiceTool = new String[19];
                for (int d = 0; d < rowCount; d++)
                {
                    String numberOfLicenceinServiceTool = table.Rows[d].Cells[2].Text;
                    numberOfLicenceInServiceTool[d] = numberOfLicenceinServiceTool;                    
                }

                for (int a = 0; a < featuresName.Count(); a++)
                {
                    if (!featuresName[a].Contains("Version") && !featuresName[a].Contains("NumberOfLicense"))
                    {
                        if (!versionInServiceTool[a].Equals("1") && !numberOfLicenceInServiceTool[a].Equals("2147483647"))
                            step4_7 = false;
                    }
                }
                if (step4_7)
                {
                    Logger.Instance.InfoLog("Step4_7 Pass");
                }

                if (step4_1 && step4_2 && step4_3 && step4_4 && step4_5 && step4_6 && step4_7)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Step 5 - Launch ICA Application and Launch any study 
                servicetool.CloseServiceTool();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step5 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step5)
                {
                    result.steps[ExecutedSteps].status = "Pass";
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
        /// Verifying Pre-Release License - Integrator
        /// </summary>
        public TestCaseResult Test_161184(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            WpfObjects wpfobject = new WpfObjects();
            
            //Config file Directory Path
            String ConfigFileDirectory = System.IO.Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "ServerConfigFiles" + Path.DirectorySeparatorChar + login.GetHostName(Config.IConnectIP);
            //Config Files - File Path
            String licensepath = ConfigFileDirectory + Path.DirectorySeparatorChar + "BluRingLicense.xml";
            String License_Name = "BluRingLicense.xml";
            String LicensePath = "C:\\WebAccess\\WebAccess\\Config\\" + License_Name;
            String PreReleseLicensepath = ConfigFileDirectory + Path.DirectorySeparatorChar + "PreReleaseLicense" + Path.DirectorySeparatorChar + "BluRingLicense.xml";

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

               
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String Studyuid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyUID");              
                String securityID_Local = Config.adminUserName + "-" + Config.adminUserName;
                String URL = "http://localhost/webaccess";

                //Precondition:
                TestFixtures.UpdateFeatureFixture("bluring", value: "true:BluRing", restart: true);

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpref.BluringViewerRadioBtn().Click();
                userpref.CloseUserPreferences();
                login.Logout();

                //Step-1:From service tool in iCA server ‐> License tab, apply pre‐release license generated for that server and restart services
                //ServiceTool servicetool = new ServiceTool();
                //servicetool.LaunchServiceTool();
                //servicetool.NavigateToConfigToolLicenseTab();
                //servicetool.WaitWhileBusy();
                //servicetool.AddLicenseInServiceTool(PreReleseLicensepath);
                //wpfobject.WaitTillLoad();
                //servicetool.RestartService();
                //wpfobject.WaitTillLoad();
                //servicetool.CloseServiceTool();               
                try { File.Copy(PreReleseLicensepath, LicensePath, true); }
                catch (Exception) { }
                login.RestartIISUsingexe();
                ExecutedSteps++;

                //Step-2:Log in to ICA application as Administrator
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                ExecutedSteps++;

                //Step-3:From User Preferences page ‐> Set the default viewer as Universal viewer
                userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpref.BluringViewerRadioBtn().Click();
                userpref.CloseUserPreferences();
                ExecutedSteps++;

                //Step-4:After logging in as Administrator navigate to Studies tab, search for any study and load it in viewer
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step8 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step8)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseBluRingViewer();

                //Step-5:From User Preferences page ‐ Set the default viewer as Enterprise viewer
                userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpref.HTML4RadioBtn().Click();
                userpref.CloseUserPreferences();
                login.Logout();
                ExecutedSteps++;                

                //Step-6:Launch TestEHR application from iCA server, navigate to Launch Image Load tab
                //and enter the following in the fields listed
                //Enter the followings-
                //Address - http -//*^<^*server Ip*^>^*/WebAccess
                //User ID - Administrator
                //Security ID-Administrator / Administrator
                //Enable User Sharing - Blank(empty)
                //Auto End Session - True
                //Auth Provider-ByPass
                //Study Instance UID of any study
                //Other fields are set to default
                login.EnableBypass();
                ehr.LaunchEHR();
                wpfobject.GetMainWindow("Test WebAccess EHR");
                ehr.SetCommonParameters(address: URL, SecurityID: securityID_Local);
                //ehr.SetSelectorOptions(showSelector: "", selectorsearch: "");
                ehr.SetSearchKeys_Study(Studyuid, "Study_UID");
                ExecutedSteps++;

                //Step-7:Click Cmd Line button to generate the EMR link and paste the url in any
                //browser
                String url_7 = ehr.clickCmdLine("ImageLoad");
                bluringviewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", url: url_7);
                var viewport = bluringviewer.GetElement(BasePage.SelectorType.CssSelector, bluringviewer.Activeviewport);
                var step7 = result.steps[++ExecutedSteps];
                step7.SetPath(testid, ExecutedSteps);
                var step7_1 = bluringviewer.CompareImage(step7, viewport);
                if (step7_1)
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

                //Step-8:Change the default viewer to Enterprise viewer from iCA service tool and restart services
                TestFixtures.UpdateFeatureFixture("bluring", value: "true:Legacy", restart: true);
                ExecutedSteps++;                

                //Step-9:Load the generated URL from TestEHR in any browser
                login.NavigateToIntegratorURL(url_7);
                var viewer9 = (StudyViewer)login.NavigateToIntegratorFrame();

                if (viewer9.SeriesViewer_1X1().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10:Set User Sharing - Enabled in Integrator of service tool and load any study in viewer
                ehr.LaunchEHR();
                wpfobject.GetMainWindow("Test WebAccess EHR");
                ehr.SetCommonParameters(address: URL, SecurityID: securityID_Local,usersharing:"True", user: Config.adminUserName);
                ehr.SetSearchKeys_Study(Studyuid, "Study_UID");
                url_7 = ehr.clickCmdLine("ImageLoad");
                login.NavigateToIntegratorURL(url_7);
                viewer9 = (StudyViewer)login.NavigateToIntegratorFrame();

                if (viewer9.SeriesViewer_1X1().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-11:Change the user viewer preference as Universal for the Administrator user and load the generated URL in browser
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);              
                userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpref.BluringViewerRadioBtn().Click();
                userpref.CloseUserPreferences();
                login.Logout();

                bluringviewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", url: url_7);
                viewport = bluringviewer.GetElement(BasePage.SelectorType.CssSelector, bluringviewer.Activeviewport);
                step7 = result.steps[++ExecutedSteps];
                step7.SetPath(testid, ExecutedSteps);
                step7_1 = bluringviewer.CompareImage(step7, viewport);
                if (step7_1)
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
                try { File.Copy(licensepath, LicensePath, true); }
                catch (Exception) { }
                login.RestartIISUsingexe();

                TestFixtures.UpdateFeatureFixture("bluring", value: "true:BluRing", restart: true);

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);               
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpref.BluringViewerRadioBtn().Click();
                userpref.CloseUserPreferences();
                login.Logout();
            }
        }

        /// <summary>
        ///Fresh install iCA 7.x - without 3D viewing license
        /// </summary>
        public TestCaseResult Test_163490(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            WpfObjects wpfobject = new WpfObjects();
            DomainManagement domain = new DomainManagement();
            RoleManagement role = new RoleManagement();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');

                //Step1 - Generate a license without 3D feature
                ExecutedSteps++;

                //Step2 - Navigate to License tab from service tool.(Service tool > License tab )
                //Step3 - Click on Import License to add license.  Restart IIS.
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();

                String licenseFileDirectory = System.IO.Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "ServerConfigFiles" + Path.DirectorySeparatorChar + login.GetHostName(Config.IConnectIP);
                String licensePath = licenseFileDirectory + Path.DirectorySeparatorChar + "Without3DLicense" + Path.DirectorySeparatorChar + "BluRingLicense.xml";
                servicetool.AddLicenseInServiceTool(licensePath);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.RestartIIS();
                ExecutedSteps += 2;

                // Step 4 - From Service Tool > License > verify ica.base.3D is not listed in the Current License list. 
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                ListView table = wpfobject.GetTable("ListView");
                int rowCount = table.Rows.Count;
                bool step4_1 = false;
                for (int i = 0; i < rowCount; i++)
                {
                    String Key = table.Rows[i].Cells[0].Text;
                    if (!Key.Equals("ica.base.3D"))
                    {
                        step4_1 = true;
                    }
                    else
                    {
                        step4_1 = false;
                        break;
                    }
                }
                servicetool.CloseServiceTool();
                Logger.Instance.InfoLog("Result of step4_1 " + step4_1);

                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("DomainManagement");
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                bool step4_2 = BasePage.Driver.FindElements(By.CssSelector(DomainManagement.checkbox_enable3DView)).Count == 0;
                domain.ClickSaveEditDomain();
                Logger.Instance.InfoLog("Result of step4_2 " + step4_2);

                login.Navigate("RoleManagement");
                role.SelectDomainfromDropDown(Config.adminGroupName);
                role.SearchRole(Config.adminRoleName);
                role.SelectRole(Config.adminRoleName);
                role.ClickEditRole();
                bool step4_3 = BasePage.Driver.FindElements(By.CssSelector(RoleManagement.checkbox_enable3DView)).Count == 0;
                role.ClickSaveEditRole();
                Logger.Instance.InfoLog("Result of step4_3 " + step4_3);

                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                bool step4_4 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_3DViewDropdown)).Count == 0;
                Logger.Instance.InfoLog("Result of step4_4 " + step4_4);

                bluringviewer.ClickOnUSerSettings();
                IList<String> userSettingValues = new List<String>();
                userSettingValues = BasePage.Driver.FindElements(By.CssSelector("div[class*='globalSettingPanel'] ul li")).Select<IWebElement, String>
                            (tool => tool.Text.Trim()).ToList();
                bool step4_5 = userSettingValues.Contains("3D SETTINGS");
                Logger.Instance.InfoLog("Result of step4_5 " + step4_5);

                IWebElement showhide = bluringviewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_ShowHideTool);
                bluringviewer.ClickElement(showhide);
                IList<String> showHideValues = new List<String>();
                showHideValues = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ShowHideDropdown)).
                Select<IWebElement, String>(element => element.GetAttribute("innerHTML").Trim()).ToList();
                bool step4_6 = showHideValues.Contains("HIDE 3D CONTROLS");
                Logger.Instance.InfoLog("Result of step4_6 " + step4_6);

                if (step4_1 && step4_2 && step4_3 && step4_4 && !step4_5 && !step4_6)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Logout Application
                bluringviewer.CloseBluRingViewer();
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
                //Revert to standard license
                String licenseFileDirectory = System.IO.Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "ServerConfigFiles" + Path.DirectorySeparatorChar + login.GetHostName(Config.IConnectIP);
                String licensePath = licenseFileDirectory + Path.DirectorySeparatorChar + "BluRingLicense.xml";
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.AddLicenseInServiceTool(licensePath);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.RestartIIS();
                servicetool.CloseServiceTool();
            }
        }

        /// <summary>
        /// Concurrent license log
        /// </summary>
        public TestCaseResult Test_164651(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            TimeSpan timeout = new TimeSpan(0, 2, 0);
            Stopwatch stopwatch = new Stopwatch();
            String Currenttime = DateTime.Now.ToString("HH:mm:ss");
            String CurrentDate = DateTime.Today.ToString("MM/dd/yy");
            String CurrentDate_1 = DateTime.Today.ToString("yyyy-MM-dd");
            BluRingViewer viewer = new BluRingViewer();
            BasePage basepage = new BasePage();
            String Month = DateTime.Now.Month.ToString("d2");
            String Year = DateTime.Now.Year.ToString();
            String SystemConfigXmlFileLocation = "C:\\WebAccess\\WebAccess\\Config\\SystemConfiguration.xml";
            String NodePath = @"Configuration/LicenseUsageLoggingInterval";
            String FileName = "C:\\Windows\\Temp\\WebAccessLicense-" + Year + "-" + Month + ".log";

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;

                // Step 1 
                // Open license usage log file:
                // WINDOWS\Temp\WebAccessLicense - YYYY - MM.log  where YYYY is the current year and MM is the current month.
                FileName = FileName.Trim();
                List<String> text = viewer.ReadLogFile(FileName);
                Logger.Instance.InfoLog("To log the text count values: "+text.Count());
                string[] LogValues = null;
                for (int i = text.Count()-1; i > 0; i--)
                {
                    if (text[i].Contains("ica.free.standing"))
                    {
                        LogValues = text[i].Split(new String[] { "ica" }, StringSplitOptions.None);
                        Logger.Instance.InfoLog("Print total lines value: "+text.Count()+ "And current i value: "+i);
                        break;
                    }
                }
                //string[] LogValues = text[text.Count() - 1].Split(new String[] { "ica" }, StringSplitOptions.None);
                string[] LogValues_1 = LogValues[0].Split(';');
                Logger.Instance.InfoLog("LogValues_1[0] value is: "+LogValues_1[0]);
                Logger.Instance.InfoLog("LogValues_1[2] value is: " + LogValues_1[2] +"and current date value is:"+CurrentDate_1 + Environment.NewLine +
                    "LogValues[1] value is: "+LogValues[1] + Environment.NewLine + "LogValues[2] value is: "+LogValues[2] + Environment.NewLine +
                    "LogValues[3] value is: " + LogValues[3] + Environment.NewLine + "LogValues[4] value is: " + LogValues[4]);
                if (LogValues_1[0].Contains("LicenseTrace Verbose") && LogValues_1[2].Contains(CurrentDate_1)
                    && LogValues[1].Contains(".free.standing 1.0;") && LogValues[2].Contains(".base.embedded 1.0;0;")
                    && LogValues[3].Contains(".free.standing.prerelease 1.0;0;") && LogValues[4].Contains(".base.embedded.prerelease 1.0;0"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 2  	Check the time interval.
                basepage.ChangeNodeValue(SystemConfigXmlFileLocation, NodePath, "1");
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(SystemConfigXmlFileLocation);
                XmlNode Node = xmlDocument.SelectSingleNode("/" + NodePath);
                String value = Node.InnerText;
                if (value.Equals("1"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 3
                basepage.RestartIISUsingexe();
                ExecutedSteps++;

                // Step 4,5,6	
                //Select the system Date and time.
                //Select the last date of the month.
                //In the time box, type 11:58:30 PM. Select Apply button.
                //DateTime.
                var lastDayOfMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                Process proc = new Process();
                proc.StartInfo.FileName = "cmd";
                proc.StartInfo.WorkingDirectory = "C:\\Windows\\System32";
                proc.StartInfo.Arguments = "/C date " + DateTime.Now.Month + "-" + lastDayOfMonth + "-" + DateTime.Now.Year + " & time 23:58:30";
                proc.Start();
                proc.WaitForExit(20000);
                ExecutedSteps += 3;

                // Step 7 	Wait for 2 minutes until the a new month is displayed.
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout)) { /*Stay Idle for 2 Miniutes*/ }
                int actualtimeout = stopwatch.Elapsed.Minutes;
                stopwatch.Stop();
                stopwatch.Reset();
                String CurrentDate_2 = DateTime.Today.ToString("MM/dd/yy");
                if (!(CurrentDate_2 == CurrentDate))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 8
                // Logout and login the iConnect Access.
                // Check the log file.
                login.LoginIConnect(Username, Password);
                Month = DateTime.Now.Month.ToString("d2");
                Year = DateTime.Now.Year.ToString();
                bool FileExists = File.Exists(("C:\\Windows\\Temp\\WebAccessLicense-" + Year + "-" + Month + ".log").Trim());
                if (FileExists)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                Process proc1 = new Process();
                proc1.StartInfo.FileName = "cmd";
                proc1.StartInfo.WorkingDirectory = "C:\\Windows\\System32";
                proc1.StartInfo.Arguments = "/C date " + CurrentDate + " & time " + Currenttime;
                proc1.Start();
                proc1.WaitForExit(20000);
                basepage.ChangeNodeValue(SystemConfigXmlFileLocation, NodePath, "60");

                //Return Result;
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
                Process proc1 = new Process();
                proc1.StartInfo.FileName = "cmd";
                proc1.StartInfo.WorkingDirectory = "C:\\Windows\\System32";
                proc1.StartInfo.Arguments = "/C date " + CurrentDate + " & time " + Currenttime;
                proc1.Start();
                proc1.WaitForExit(20000);
                basepage.ChangeNodeValue(SystemConfigXmlFileLocation, NodePath, "60");
            }
        }


        /// <summary>
        /// iCA login fails after license expiry
        /// </summary>
        public TestCaseResult Test_163489(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            String Currenttime = DateTime.Now.ToString("HH:mm:ss");
            String CurrentDate = DateTime.Today.ToString("MM/dd/yy");
            WpfObjects wpfobject = new WpfObjects();
            BluRingViewer viewer = new BluRingViewer();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;

                //Step 1 Generate iCA license.
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                //Current Directory path				
                String licenseFileDirectory = System.IO.Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "ServerConfigFiles" + Path.DirectorySeparatorChar + login.GetHostName(Config.IConnectIP);
                //Config Files - File Path
                String licensePath = licenseFileDirectory + Path.DirectorySeparatorChar + "Dec31ExpiryLicense" + Path.DirectorySeparatorChar + "BluRingLicense.xml";
                servicetool.AddLicenseInServiceTool(licensePath);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //step2 Login to ica webconsole.
                login.LoginIConnect(Username, Password);
                if (login.IsTabPresent("Studies"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();
                login.Logout();

                //step3 On the ICA server and client machines, change the current date ahead by 29 days. 
                Process proc1 = new Process();
                proc1.StartInfo.FileName = "cmd";
                proc1.StartInfo.WorkingDirectory = "C:\\Windows\\System32";
                proc1.StartInfo.Arguments = "/C date " + "12/30/19" + " & time " + Currenttime;
                proc1.Start();
                proc1.WaitForExit(20000);
                ExecutedSteps++;

                //step4 From a client machine: Login to ica webconsole.
                login.LoginIConnect(Username, Password);
                if (login.IsTabPresent("Studies"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();
                login.Logout();

                //step5 On the ICA server and client machines, change the date ahead by 30 days - so the license can expire.
                proc1.StartInfo.FileName = "cmd";
                proc1.StartInfo.WorkingDirectory = "C:\\Windows\\System32";
                proc1.StartInfo.Arguments = "/C date " + "12/31/19" + " & time 13:05:30";
                proc1.Start();
                proc1.WaitForExit(20000);
                ExecutedSteps++;

                //step6 From a client machine: Login to ica webconsole.
                login.DriverGoTo(login.url);
                login.LoginIConnect(Username, Password);
                string ErrorMessage = viewer.GetElement(BasePage.SelectorType.CssSelector, "span#ctl00_LoginMasterContentPlaceHolder_ErrorMessage").GetAttribute("innerHTML").ToString();
                if (ErrorMessage.Equals("The system cannot log you in. Please try again.: No license is available. Please contact the system administrator."))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step7 On the ICA server and client machines, change the date ahead by 31 days.
                proc1.StartInfo.FileName = "cmd";
                proc1.StartInfo.WorkingDirectory = "C:\\Windows\\System32";
                proc1.StartInfo.Arguments = "/C date " + "01/01/20" + " & time 13:00:00";
                proc1.Start();
                proc1.WaitForExit(20000);
                ExecutedSteps++;

                //step8 From a client machine: Login to ica webconsole.
                login.DriverGoTo(login.url);
                login.LoginIConnect(Username, Password);
                ErrorMessage = viewer.GetElement(BasePage.SelectorType.CssSelector, "span#ctl00_LoginMasterContentPlaceHolder_ErrorMessage").GetAttribute("innerHTML").ToString();
                if (ErrorMessage.Equals("The system cannot log you in. Please try again.: No license is available. Please contact the system administrator."))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step9  Objective: Renew license after it expires.
                servicetool.LaunchServiceTool(HandleWindowPopup: true);
                wpfobject.WaitTillLoad();
                //Current Directory path				
                licenseFileDirectory = System.IO.Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "ServerConfigFiles" + Path.DirectorySeparatorChar + login.GetHostName(Config.IConnectIP);
                //Config Files - File Path
                licensePath = licenseFileDirectory + Path.DirectorySeparatorChar + "BluRingLicense.xml";
                servicetool.AddLicenseInServiceTool(licensePath);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //step10  From a client machine: Login to ica webconsole.
                login.DriverGoTo(login.url);
                login.LoginIConnect(Username, Password);
                if (login.IsTabPresent("Studies"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                proc1 = new Process();
                proc1.StartInfo.FileName = "cmd";
                proc1.StartInfo.WorkingDirectory = "C:\\Windows\\System32";
                proc1.StartInfo.Arguments = "/C date " + CurrentDate + " & time " + Currenttime;
                proc1.Start();
                proc1.WaitForExit(20000);

                //Return Result;
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
                Process proc1 = new Process();
                proc1.StartInfo.FileName = "cmd";
                proc1.StartInfo.WorkingDirectory = "C:\\Windows\\System32";
                proc1.StartInfo.Arguments = "/C date " + CurrentDate + " & time " + Currenttime;
                proc1.Start();
                proc1.WaitForExit(20000);

                //Revert to standard license
                String licenseFileDirectory = System.IO.Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "ServerConfigFiles" + Path.DirectorySeparatorChar + login.GetHostName(Config.IConnectIP);
                String licensePath = licenseFileDirectory + Path.DirectorySeparatorChar + "BluRingLicense.xml";
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.AddLicenseInServiceTool(licensePath);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.RestartIIS();
                servicetool.CloseServiceTool();
            }
        }

        /// <summary>
        /// Verify User and Viewer counts in Maintenance tab
        /// </summary>
        public TestCaseResult Test_164596(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            WpfObjects wpfobject = new WpfObjects();
            DomainManagement domain = new DomainManagement();
            RoleManagement role = new RoleManagement();
            bluringviewer = new BluRingViewer();
            BasePage basePage = new BasePage();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                DataTable datatable;
                DataTable datatable1;
                BasePage.MultiDriver.Clear();
                BasePage.MultiDriver.Add(BasePage.Driver);
                string BrowserType = Config.BrowserType;
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');

                //Step1 - iCA installed and configured for testing.
                ExecutedSteps++;

                //Step2 - Login to iCA webconsole. Navigate to User Management > -create a local user;-create an ldap user;  Verify the Number of Users count via Maintenace > Statistics tab
                login.SetDriver(BasePage.MultiDriver[0]);
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                var maintenancePage = (Maintenance)login.Navigate("Maintenance");
                maintenancePage.Navigate("Statistics");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                int noOfUserBeforeUserCreation = Int32.Parse(BasePage.Driver.FindElement(By.CssSelector(Maintenance.span_noOfUsers)).Text);

                //Create local user
                String User1 = "LicensingU1164596" + new Random().Next(10000);
                var usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(User1, Config.adminGroupName, Config.adminRoleName);


                maintenancePage = (Maintenance)login.Navigate("Maintenance");
                maintenancePage.Navigate("Statistics");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                int noOfUserAfterUserCreation = Int32.Parse(BasePage.Driver.FindElement(By.CssSelector(Maintenance.span_noOfUsers)).Text);
                if (noOfUserAfterUserCreation == noOfUserBeforeUserCreation + 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step3
                BasePage.MultiDriver.Add(login.InvokeBrowser(BrowserType));
                login.SetDriver(BasePage.MultiDriver[1]);
                login.DriverGoTo(login.url);
                login.LoginIConnect(User1, User1);
                login.SetDriver(BasePage.MultiDriver[0]);
                maintenancePage.Navigate("Audit");
                maintenancePage.Navigate("Statistics");
                datatable = basePage.CollectRecordsInTable(maintenancePage.StatisticsTable(), maintenancePage.TableHeader(), maintenancePage.TableRow(), maintenancePage.TableColumn());
                String[] ExpectedUsers = new String[] { "Administrator", User1 };
                String[] Users = basePage.GetColumnValues(datatable, "User Id");
                bool step3_1 = ExpectedUsers.SequenceEqual(Users);
                Logger.Instance.InfoLog("Result of Step3_1 :" + step3_1);

                bool step3_2 = basePage.GetColumnValues(datatable, "Host Name").Count() == 2;
                Logger.Instance.InfoLog("Result of Step3_2 :" + step3_2);

                bool step3_3 = basePage.GetColumnValues(datatable, "Feature Name").Count() == 2;
                Logger.Instance.InfoLog("Result of Step3_3 :" + step3_3);

                bool step3_4 = basePage.GetColumnValues(datatable, "Expiry Date/Time").Count() == 2;
                Logger.Instance.InfoLog("Result of Step3_4 :" + step3_4);
                if (step3_1 && step3_2 && step3_3 && step3_4)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step4 - Logout one user and verify License Usage details are updated.
                login.SetDriver(BasePage.MultiDriver[1]);
                login.Logout();
                login.SetDriver(BasePage.MultiDriver[0]);
                maintenancePage.Navigate("Audit");
                maintenancePage.Navigate("Statistics");
                datatable = basePage.CollectRecordsInTable(maintenancePage.StatisticsTable(), maintenancePage.TableHeader(), maintenancePage.TableRow(), maintenancePage.TableColumn());
                ExpectedUsers = new String[] { "Administrator" };
                Users = basePage.GetColumnValues(datatable, "User Id");
                if (ExpectedUsers.SequenceEqual(Users))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step5 - Create an additional user via User Management. Verify the Number of Users count via Maintenance > Statistics tab
                String User2 = "LicensingU2164596" + new Random().Next(10000);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(User2, Config.adminGroupName, Config.adminRoleName);
                maintenancePage = (Maintenance)login.Navigate("Maintenance");
                maintenancePage.Navigate("Statistics");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                int noOfUserInStep5 = Int32.Parse(BasePage.Driver.FindElement(By.CssSelector(Maintenance.span_noOfUsers)).Text);
                if (noOfUserInStep5 == noOfUserAfterUserCreation + 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step6 - Login to iCA webconsole with newly created users in additon to already logged in users. Verify License Usage via Maintenance > Statistics tab.
                BasePage.MultiDriver.Add(login.InvokeBrowser(BrowserType));
                login.SetDriver(BasePage.MultiDriver[2]);
                login.DriverGoTo(login.url);
                login.LoginIConnect(User2, User2);

                login.SetDriver(BasePage.MultiDriver[0]);
                maintenancePage.Navigate("Audit");
                maintenancePage.Navigate("Statistics");
                datatable = basePage.CollectRecordsInTable(maintenancePage.StatisticsTable(), maintenancePage.TableHeader(), maintenancePage.TableRow(), maintenancePage.TableColumn());
                ExpectedUsers = new String[] { "Administrator", User2 };
                Users = basePage.GetColumnValues(datatable, "User Id");
                bool step6_1 = ExpectedUsers.SequenceEqual(Users);
                Logger.Instance.InfoLog("Result of Step6_1 :" + step6_1);

                bool step6_2 = basePage.GetColumnValues(datatable, "Host Name").Count() == 2;
                Logger.Instance.InfoLog("Result of Step6_2 :" + step6_2);

                bool step6_3 = basePage.GetColumnValues(datatable, "Feature Name").Count() == 2;
                Logger.Instance.InfoLog("Result of Step6_3 :" + step6_3);

                bool step6_4 = basePage.GetColumnValues(datatable, "Expiry Date/Time").Count() == 2;
                Logger.Instance.InfoLog("Result of Step6_4 :" + step6_4);
                if (step6_1 && step6_2 && step6_3 && step6_4)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //  Step7 - Delete the logged out user. Verify the Number of Users count via Maintenace > Statistics tab
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.DeleteUser(Config.adminGroupName, User1);
                maintenancePage = (Maintenance)login.Navigate("Maintenance");
                maintenancePage.Navigate("Statistics");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                int noOfUserInStep7 = Int32.Parse(BasePage.Driver.FindElement(By.CssSelector(Maintenance.span_noOfUsers)).Text);
                if (noOfUserInStep7 == noOfUserInStep5 - 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step8 - Create an additional user via User Management. Verify the Number of Users count via Maintenace > Statistics tab
                String User3 = "LicensingU3164596" + new Random().Next(10000);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(User3, Config.adminGroupName, Config.adminRoleName);
                maintenancePage = (Maintenance)login.Navigate("Maintenance");
                maintenancePage.Navigate("Statistics");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                int noOfUserInStep8 = Int32.Parse(BasePage.Driver.FindElement(By.CssSelector(Maintenance.span_noOfUsers)).Text);
                if (noOfUserInStep8 == noOfUserInStep7 + 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step9 - 
                maintenancePage.Navigate("Viewer Services");
                datatable1 = basePage.CollectRecordsInTable(maintenancePage.Tbl_ViewerServicesTable(), maintenancePage.TableHeaderInViewerServices(), maintenancePage.TableRowInViewerServices(), maintenancePage.TableColumnInViewerServices());
                String[] Clients = basePage.GetColumnValues(datatable1, "Clients");
                bool step9_1 = Clients[0].Equals("0");
                Logger.Instance.InfoLog("Result of step9_1 := " + step9_1);

                login.SetDriver(BasePage.MultiDriver[1]);
                login.LoginIConnect(User3, User3);
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer Viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForAllViewportsToLoad(120);

                login.SetDriver(BasePage.MultiDriver[2]);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer Viewer1 = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForAllViewportsToLoad(120);

                login.SetDriver(BasePage.MultiDriver[0]);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                maintenancePage = (Maintenance)login.Navigate("Maintenance");
                maintenancePage.Navigate("Viewer Services");
                datatable1 = basePage.CollectRecordsInTable(maintenancePage.Tbl_ViewerServicesTable(), maintenancePage.TableHeaderInViewerServices(), maintenancePage.TableRowInViewerServices(), maintenancePage.TableColumnInViewerServices());
                String[] actualClients = basePage.GetColumnValues(datatable1, "Clients");
                bool step9_2 = actualClients[0].Equals("2");
                Logger.Instance.InfoLog("Result of step9_2 := " + step9_2);

                if (step9_1 && step9_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step10 - 
                login.SetDriver(BasePage.MultiDriver[1]);
                studies.CloseStudy();
                login.Logout();

                login.SetDriver(BasePage.MultiDriver[0]);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                maintenancePage = (Maintenance)login.Navigate("Maintenance");
                maintenancePage.Navigate("Viewer Services");
                datatable1 = basePage.CollectRecordsInTable(maintenancePage.Tbl_ViewerServicesTable(), maintenancePage.TableHeaderInViewerServices(), maintenancePage.TableRowInViewerServices(), maintenancePage.TableColumnInViewerServices());
                actualClients = basePage.GetColumnValues(datatable1, "Clients");
                bool step10 = actualClients[0].Equals("1");
                Logger.Instance.InfoLog("Result of step10 := " + step10);
                if (step10)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step11 - 
                maintenancePage.Navigate("Viewer Services");
                datatable1 = basePage.CollectRecordsInTable(maintenancePage.Tbl_ViewerServicesTable(), maintenancePage.TableHeaderInViewerServices(), maintenancePage.TableRowInViewerServices(), maintenancePage.TableColumnInViewerServices());
                Clients = basePage.GetColumnValues(datatable1, "Clients");
                bool step11_1 = Clients[1].Equals("0");
                Logger.Instance.InfoLog("Result of step11_1 := " + step11_1);

                login.SetDriver(BasePage.MultiDriver[2]);
                studies.CloseStudy();
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                BluRingViewer viewer = BluRingViewer.LaunchBluRingViewer();

                login.SetDriver(BasePage.MultiDriver[1]);
                login.DriverGoTo(login.url);
                login.LoginIConnect(User2, User2);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();

                login.SetDriver(BasePage.MultiDriver[0]);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                maintenancePage = (Maintenance)login.Navigate("Maintenance");
                maintenancePage.Navigate("Viewer Services");
                datatable1 = basePage.CollectRecordsInTable(maintenancePage.Tbl_ViewerServicesTable(), maintenancePage.TableHeaderInViewerServices(), maintenancePage.TableRowInViewerServices(), maintenancePage.TableColumnInViewerServices());
                actualClients = basePage.GetColumnValues(datatable1, "Clients");
                bool step11_2 = actualClients[1].Equals("2");
                Logger.Instance.InfoLog("Result of step11_2 := " + step11_2);

                if (step11_1 && step11_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step12 - 
                login.SetDriver(BasePage.MultiDriver[1]);
                viewer.CloseBluRingViewer();
                login.Logout();

                login.SetDriver(BasePage.MultiDriver[0]);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                maintenancePage = (Maintenance)login.Navigate("Maintenance");
                maintenancePage.Navigate("Viewer Services");
                datatable1 = basePage.CollectRecordsInTable(maintenancePage.Tbl_ViewerServicesTable(), maintenancePage.TableHeaderInViewerServices(), maintenancePage.TableRowInViewerServices(), maintenancePage.TableColumnInViewerServices());
                actualClients = basePage.GetColumnValues(datatable1, "Clients");
                bool step12 = actualClients[1].Equals("1");
                Logger.Instance.InfoLog("Result of step10 := " + step10);
                if (step10)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();


                //Logout and close Application
                login.Logout();
                login.SetDriver(BasePage.MultiDriver[2]);
                viewer.CloseBluRingViewer();
                login.Logout();
                bluringviewer.closeallbrowser();

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
        /// Upgrade iCA - without 3D license
        /// </summary>
        public TestCaseResult Test_163492(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            WpfObjects wpfobject = new WpfObjects();
            DomainManagement domain = new DomainManagement();
            RoleManagement role = new RoleManagement();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');

                //Step1 - 
                // 1. Use a license without 3D feature.
                // 2.Run upgrade utility test case 167821.Follow the latest FPS for supported upgrade path            
                ExecutedSteps++;    // This will be covered in upgrade utility test


                // Step2 - Login to iCA webconsole after upgrade is completed without any errors.
                login.LoginIConnect(adminUserName, adminPassword);
                if (login.IsTabPresent("Studies"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step3 - From Service Tool > License > verify ica.base.3D is not listed in the Current License list.
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                ListView table = wpfobject.GetTable("ListView");
                int rowCount = table.Rows.Count;
                bool step3_1 = false;
                for (int i = 0; i < rowCount; i++)
                {
                    String Key = table.Rows[i].Cells[0].Text;
                    if (!Key.Equals("ica.base.3D"))
                    {
                        step3_1 = true;
                    }
                    else
                    {
                        step3_1 = false;
                        break;
                    }
                }
                servicetool.CloseServiceTool();
                Logger.Instance.InfoLog("Result of step3_1 " + step3_1);

                // Login to iCA webconsole > enable 3d functionality in Domain and Roles Mgmt.               
                login.Navigate("DomainManagement");
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                bool step3_2 = BasePage.Driver.FindElements(By.CssSelector(DomainManagement.checkbox_enable3DView)).Count == 0;
                domain.ClickSaveEditDomain();
                Logger.Instance.InfoLog("Result of step3_2 " + step3_2);

                login.Navigate("RoleManagement");
                role.SelectDomainfromDropDown(Config.adminGroupName);
                role.SearchRole(Config.adminRoleName);
                role.SelectRole(Config.adminRoleName);
                role.ClickEditRole();
                bool step3_3 = BasePage.Driver.FindElements(By.CssSelector(RoleManagement.checkbox_enable3DView)).Count == 0;
                role.ClickSaveEditRole();
                Logger.Instance.InfoLog("Result of step3_3 " + step3_3);

                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                bool step3_4 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_3DViewDropdown)).Count == 0;
                Logger.Instance.InfoLog("Result of step3_4 " + step3_4);

                bluringviewer.ClickOnUSerSettings();
                IList<String> userSettingValues = new List<String>();
                userSettingValues = BasePage.Driver.FindElements(By.CssSelector("div[class*='globalSettingPanel'] ul li")).Select<IWebElement, String>
                            (tool => tool.Text.Trim()).ToList();
                bool step3_5 = userSettingValues.Contains("3D SETTINGS");
                Logger.Instance.InfoLog("Result of step3_5 " + step3_5);

                IWebElement showhide = bluringviewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_ShowHideTool);
                bluringviewer.ClickElement(showhide);
                IList<String> showHideValues = new List<String>();
                showHideValues = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ShowHideDropdown)).
                Select<IWebElement, String>(element => element.GetAttribute("innerHTML").Trim()).ToList();
                bool step3_6 = showHideValues.Contains("HIDE 3D CONTROLS");
                Logger.Instance.InfoLog("Result of step3_6 " + step3_6);

                if (step3_1 && step3_2 && step3_3 && step3_4 && !step3_5 && !step3_6)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Logout Application
                bluringviewer.CloseBluRingViewer();
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
        /// Verifying Pre-Release License - Integrator
        /// </summary>
        public TestCaseResult Old_161184(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            WpfObjects wpfobject = new WpfObjects();
            iCNOrderingPortal OrderingPortal = new iCNOrderingPortal();
            iCNPortal iCNPortal = new iCNPortal();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String licencePath = "D:\\BluRingLicense.xml";
                String PreReleseLicensepath = "D:\\BluRingLicense_SRV1.xml";
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String Studyuid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyUID");
                String lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Lastname");
                String usernames = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Username");
                String pwds = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Password");
                String customers = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Customers");
                String[] username = usernames.Split(':');
                String[] pwd = pwds.Split(':');
                String[] customer = customers.Split(':');
                String icnOrderingPortal = iCNOrderingPortal.URL;
                String icnPortal = iCNPortal.URL;
                String iConnectURL = "http://10.9.39.181/webaccess";
                String securityID_Local = Config.adminUserName + "-" + Config.adminUserName;
                String URL = "http://localhost/webaccess";


                //Step-1:Launch the URL https-//icnsandbox.merge.com/Portal/, login as superadmin [Password - Pa$$word] and navigate to Administration page
                login.DriverGoTo(icnPortal);
                iCNPortal.LoginICNPortal(username[0], pwd[0], customer[0]);
                if (BasePage.Driver.FindElement(By.CssSelector(iCNPortal.AdministrationLink)).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-2:Select "Find Customers" link in the Administration page and search for ICNPQA-RIS Standalone Customer in the description field
                ExecutedSteps++;

                //Step-3:Select "Configure iConnect Access" option and save the IP address of the iCA server [For e.g- http-//10.4.37.6/webaccess] in the "Configure iConnect Access URL" page 
                iCNPortal.ConfigureiCA(customer[1], iConnectURL);
                if (BasePage.Driver.FindElement(By.CssSelector(iCNPortal.iCAURL)).GetAttribute("innerHTML").Contains(iConnectURL))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4:Logout as Super Admin
                iCNPortal.LogoutICNPortal();
                ExecutedSteps++;

                //Step-5:From service tool in iCA server ‐> License tab, apply pre‐release license generated for that server and restart services
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolLicenseTab();
                servicetool.WaitWhileBusy();
                servicetool.AddLicenseInServiceTool(PreReleseLicensepath);
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Step-6:Log in to ICA application as Administrator
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                ExecutedSteps++;

                //Step-7:From User Preferences page ‐> Set the default viewer as Enterprise [BluRing] viewer
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpref.BluringViewerRadioBtn().Click();
                userpref.CloseUserPreferences();
                ExecutedSteps++;

                //Step-8:After logging in as Administrator navigate to Studies tab, search for any study and load it in enterprise viewer
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step8 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step8)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-9:From User Preferences page ‐ Set the default viewer as HTML4 viewer
                userpref.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpref.HTML4RadioBtn().Click();
                userpref.CloseUserPreferences();
                ExecutedSteps++;


                //Step-10:From iCA service tool, navigate to Integrator tab and set User Sharing as Disabled
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.Integrator_Tab);
                servicetool.WaitWhileBusy();
                servicetool.EanbleUserSharing_ShadowUser(usersharing: "Always disabled");
                ExecutedSteps++;

                //Step-11:Change the default viewer to Enterprise [BluRing] viewer and restart services
                servicetool.NavigateToTab(ServiceTool.Viewer_Tab);
                wpfobject.WaitTillLoad();
                servicetool.NavigateSubTab(ServiceTool.Viewer.Name.Miscellaneous_tab);
                wpfobject.GetButton(ServiceTool.ModifyBtn_Name, 1).Click();
                wpfobject.WaitTillLoad();
                servicetool.SetBluringViewer();
                wpfobject.GetButton(ServiceTool.ApplyBtn_Name, 1).Click();
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Step-12:Launch TestEHR application from iCA server, navigate to Launch Image Load tab
                //and enter the following in the fields listed
                //Enter the followings-
                //Address - http -//*^<^*server Ip*^>^*/WebAccess
                //User ID - Administrator
                //Security ID-Administrator / Administrator
                //Enable User Sharing - Blank(empty)
                //Auto End Session - True
                //Auth Provider-ByPass
                //Study Instance UID of any study
                //Other fields are set to default
                login.EnableBypass();
                ehr.LaunchEHR();
                wpfobject.GetMainWindow("Test WebAccess EHR");
                ehr.SetCommonParameters(address: URL, SecurityID: securityID_Local);
                //ehr.SetSelectorOptions(showSelector: "", selectorsearch: "");
                ehr.SetSearchKeys_Study(Studyuid, "Study_UID");
                ExecutedSteps++;

                //Step-13:Click Cmd Line button to generate the EMR link and paste the url in any
                //browser
                String url_13 = ehr.clickCmdLine("ImageLoad");
                bluringviewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", url: url_13);
                var viewport = bluringviewer.GetElement(BasePage.SelectorType.CssSelector, bluringviewer.Activeviewport);
                var step13 = result.steps[++ExecutedSteps];
                step13.SetPath(testid, ExecutedSteps);
                var step13_1 = bluringviewer.CompareImage(step13, viewport);
                if (step13_1)
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

                //Step-14:Launch the below link
                //https -//icnsandbox.merge.com/OrderingPortal/Order/OrderSummary?OrderId-18431
                //and log in as jonathanbowyer / Pa$$word1 after choosing the third drop down "ICNPQA-RIS Standalone Customer"
                login.DriverGoTo(icnOrderingPortal);
                OrderingPortal.LoginICNOrderingPortal(username[1], pwd[1], customer[1]);
                if (BasePage.Driver.FindElement(By.CssSelector(iCNOrderingPortal.Pat_LastName)).GetAttribute("innerHTML").Contains(lastname))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-15:From Order summary page, Click on button "view images(2) and select the study [Accession # - 00012793 or 00012794 or 00012795]
                OrderingPortal.LoadImage();
                IList<string> tabs = new List<string>(BasePage.Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);

                //Need to do
                //result.steps[++ExecutedSteps].status = "On-Hold"; //Exam list didnt load

                viewport = bluringviewer.GetElement(BasePage.SelectorType.CssSelector, bluringviewer.Activeviewport);
                var step15 = result.steps[++ExecutedSteps];
                step15.SetPath(testid, ExecutedSteps);
                var step15_1 = bluringviewer.CompareImage(step15, viewport);
                if (step15_1)
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
                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();

                //Step-16:Change the default viewer to iCA [HTML4] viewer from iCA service tool and restart services
                TestFixtures.UpdateFeatureFixture("bluring", value: "false:BluRing", restart: true);
                ExecutedSteps++;

                //Step-17:From Order Summary page in iCN portal, load the same study
                login.DriverGoTo(icnOrderingPortal);
                OrderingPortal.LoginICNOrderingPortal(username[1], pwd[1], customer[1]);
                OrderingPortal.LoadImage();
                tabs = new List<string>(BasePage.Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                PageLoadWait.WaitForAllViewportsToLoad(40);
                if (BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg")).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();

                //Step-18:Load the generated URL from TestEHR in any browser
                login.NavigateToIntegratorURL(url_13);
                var viewer18 = (StudyViewer)login.NavigateToIntegratorFrame();

                if (viewer18.SeriesViewer_1X1().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-19:From iCA server ‐> License tab, apply standard license generated for that
                //server and restart services
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolLicenseTab();
                servicetool.WaitWhileBusy();
                servicetool.AddLicenseInServiceTool(licencePath);
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Step-20:Load the generated URL from TestEHR in any browser again after clearing the cache
                login.NavigateToIntegratorURL(url_13);
                var viewer20 = (StudyViewer)login.NavigateToIntegratorFrame();

                if (viewer20.SeriesViewer_1X1().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-21:From Order Summary page in iCN portal, load the same study again after clearing the cache
                login.DriverGoTo(icnOrderingPortal);
                OrderingPortal.LoginICNOrderingPortal(username[1], pwd[1], customer[1]);
                OrderingPortal.LoadImage();
                tabs = new List<string>(BasePage.Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                PageLoadWait.WaitForAllViewportsToLoad(40);
                if (BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg")).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();

                //Step-22:Change the default viewer to Enterprise [BluRing] viewer and restart services from service tool
                TestFixtures.UpdateFeatureFixture("bluring", value: "true:BluRing", restart: true);
                ExecutedSteps++;

                //Step-23:From Order Summary page in iCN portal, load the same study again after clearing the cache
                login.DriverGoTo(icnOrderingPortal);
                OrderingPortal.LoginICNOrderingPortal(username[1], pwd[1], customer[1]);
                OrderingPortal.LoadImage();
                tabs = new List<string>(BasePage.Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                //Need to do
                //result.steps[++ExecutedSteps].status = "On-Hold"; //Exam list didnt load

                viewport = bluringviewer.GetElement(BasePage.SelectorType.CssSelector, bluringviewer.Activeviewport);
                var step23 = result.steps[++ExecutedSteps];
                step15.SetPath(testid, ExecutedSteps);
                var step23_1 = bluringviewer.CompareImage(step23, viewport);
                if (step23_1)
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
                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();

                //Step-24:Load the generated URL from TestEHR in any browser again after clearing the cache
                bluringviewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", url: url_13);
                viewport = bluringviewer.GetElement(BasePage.SelectorType.CssSelector, bluringviewer.Activeviewport);
                var step24 = result.steps[++ExecutedSteps];
                step24.SetPath(testid, ExecutedSteps);
                var step24_1 = bluringviewer.CompareImage(step24, viewport);
                if (step24_1)
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

                //Step-25:Launch iCA application in any browser, login as Administrator and launch any study from Studies tab
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step22_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));

                if (step22_1)
                {
                    result.steps[ExecutedSteps].status = "Pass";
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
