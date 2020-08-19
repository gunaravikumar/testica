using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Reusable.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Selenium.Scripts.Tests
{
    class HighAvailability
    {
        public Login login { get; set; }
        public ExamImporter ei { get; set; }
        public string filepath { get; set; }
        private string Dest1_ID { get; set; }
        private string Dest2_ID { get; set; }
        private string Dest1_AETitle { get; set; }
        private string Dest2_AETitle { get; set; }

        public HighAvailability(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.lburl);       
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";          
            Dest1_ID = new BasePage().GetHostName(Config.LB_Dest1IP);         
            Dest2_ID = new BasePage().GetHostName(Config.LB_Dest2IP);
            Dest1_AETitle = "ECM_ARC_" + Config.LB_Dest1IP.Split('.')[3];
            Dest2_AETitle = "ECM_ARC_" + Config.LB_Dest2IP.Split('.')[3];
            try
            {
                //Copy Build info file from server
                //xcopy /y /f "\\\\" + Config.LB_ICA1IP + "\\C$\\WebAccess\\Build.Info" "D:\\"
                var proc = new Process
                {
                    StartInfo =
                        {
                            FileName = "xcopy",
                            WorkingDirectory = @"%systemroot%\System32\",
                            Arguments = "/y /f \\\\" + Config.LB_ICA1IP + "\\C$\\WebAccess\\Build.Info D:\\",
                            UseShellExecute = true,
                        }
                };
                proc.Start();
                proc.WaitForExit(30000);
                if (!proc.HasExited) { proc.CloseMainWindow(); }

                //Get the Build ID from Server's Build.info file and update in the client machine
                string line;
                string buildno = "";
                using (StreamReader sr = new StreamReader("\\\\" + Config.LB_ICA1IP + "\\C$\\WebAccess\\Build.Info"))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains("Build Number")) { buildno = line; break; }
                    }
                    buildno = buildno.Split(':')[1].Trim();
                }
                Dictionary<String, String> buildnumber = new Dictionary<String, String>();
                buildnumber.Add("buildnumber", buildno);
                Dictionary<String, String> RDMBuildNo = new Dictionary<String, String>();
                RDMBuildNo.Add("buildnumber", buildno);
                ReadXML.UpdateXML(Config.inputparameterpath, buildnumber);
                ReadXML.UpdateXML(Config.inputparameterpath, RDMBuildNo);
                Config.buildnumber = buildno;
                Config.rdm = buildno;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Error while updating Build ID");
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
            }
        }

        /// <summary>
        /// Active Directory LDAP Identity map
        /// </summary>
        public TestCaseResult Test_161380(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String ldapAdminUserName = Config.LdapAdminUserName;
                String ldapAdminPassword = Config.LdapAdminPassword;
                String credentials = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Credentials");
                String user1UserName = credentials.Split(':')[0]; //"victoria.dassen";
                String user1Password = credentials.Split(':')[1]; //".vcd.13579";
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID"); //14343                

                //variables
                Inbounds inbounds;
                String studyStatus;
                Studies studies;
                RoleManagement rolemanagement;

                //Precondition - Enable Grant access for Super Role
                login.NavigateAndLoginIConnect(login.lburl, ldapAdminUserName, ldapAdminPassword);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SearchRole("SuperRole", Config.adminGroupName);
                rolemanagement.EditRoleByName("SuperRole");
                PageLoadWait.WaitForFrameLoad(20);
                rolemanagement.GrantAccessRadioBtn_Anyone().Click();               
                rolemanagement.ClickSaveEditRole();
                login.Logout();

                //Step 1 - Set Ldap in both the ica servers.
                //Test Data: In the service tool, Enable the LDAP server, the default setting will be used to do the iConnect access tests.
                //In the service tool select the LDAP tab and then select Servers. 
                //Click on Modify then select the ica.ldap.merge.ad and click on Details to edit the setting.
                //Confirm or change the Host name = 10.4.38.27, confirm the Account name is ica.administrator
                //Click OK, Apply and then reset the IIS and windows services
                //---------------------------------
                //Note: Apply for both servers: ica - s12 - r3 and ica - s12 - r4.                             
                //Done in environment set up  
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 2 -
                //For HTTP: http://10.4.38.147/webaccess
                //For HTTPS:https://icaf5ssh.merge.com/webaccess
                //Login iCA as a registered user UID = ica.administrator PID = admin.13579
                login.NavigateAndLoginIConnect(login.lburl, ldapAdminUserName, ldapAdminPassword);
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 3 - 
                //Exit the Domain Management and select Studies tab, enter a * in the patient name and click on search
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: accession);
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 4 -
                //Select a study and Grant Access to Victoria.dassen
                studies.SelectStudy("Accession", accession);
                studies.GrantAccessToUsers(Config.adminGroupName, user1UserName);
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 5 -
                //Exit ica.administrator and logon with UID = victoria.dassen PID = .vcd.13579
                login.Logout();
                login.NavigateAndLoginIConnect(login.lburl, user1UserName, user1Password);
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 6 -
                //Select Inbound tab to view the granted study
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(AccessionNo: accession);                
                inbounds.GetMatchingRow("Accession", accession).TryGetValue("Status", out studyStatus);
                if (studyStatus.ToLower() == "shared")                
                {
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
                GC.Collect();
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
        /// Check both ICA servers are active
        /// </summary>
        public TestCaseResult Test_161376(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            //variables
            Studies studies;
            UserManagement user;
            DomainManagement domain = new DomainManagement();
            RoleManagement role;
            Inbounds inbounds;
            Outbounds outbounds;
            BluRingViewer viewer;
            UserPreferences userpref = new UserPreferences();
            String studyStatus;
            HPLogin hplogin = new HPLogin();
            HPHomePage hphome;
            WorkFlow workflow;

            int changebrowser = 0;
            String initialBrowserName = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();

            String adminUserName = Config.adminUserName;
            String adminPassword = Config.adminPassword;
            String superAdminDomain = Config.adminGroupName;
            string downloadFolder = Config.downloadpath;
            if (initialBrowserName.Contains("explorer"))
                downloadFolder = @"D:\BatchExecution\Selenium\Downloads";
            String credentials = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Credentials");
            String user1UserName = credentials.Split(':')[0]; //"ph1";
            String user1Password = credentials.Split(':')[1]; //"p";
            String user2UserName = credentials.Split(':')[2]; //"ph2";
            String user2Password = credentials.Split(':')[3]; //"p";
            String usersRole = "SuperRole";
            String newUser1UserName = user1UserName + "1";
            String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
            String[] arrAccession = accession.Split(':');
            String patientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String[] arrPatientID = patientID.Split(':');
            String zipNames = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ZipFileNameList");
            String[] arrZipNames = zipNames.Split('=');
            String downZipPath = downloadFolder + "\\" + arrZipNames[0] + ".zip";

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                //Pre-condition: 
                //1. Create two users ph1 and ph2, if not available already.
                //User can be created in any server. 
                login.NavigateAndLoginIConnect(login.lb_ica1_url, adminUserName, adminPassword);               
                user = login.Navigate<UserManagement>();
                if (!user.IsUserExist(user1UserName, superAdminDomain))
                {
                    Logger.Instance.InfoLog("Pre-condition: User '" + user1UserName + "' is not available in domain '" + superAdminDomain + "'");
                    user.CreateUser(user1UserName, superAdminDomain, usersRole, hasPass: 1, Password: user1Password);
                    Logger.Instance.InfoLog("Pre-condition: User '" + user1UserName + "' created in domain '" + superAdminDomain + "'");
                }
                else
                    Logger.Instance.InfoLog("Pre-condition: User '" + user1UserName + "' is already available in domain '" + superAdminDomain + "'");
                if (!user.IsUserExist(user2UserName, superAdminDomain))
                {
                    Logger.Instance.InfoLog("Pre-condition: User '" + user2UserName + "' is not available in domain '" + superAdminDomain + "'");
                    user.CreateUser(user2UserName, superAdminDomain, usersRole, hasPass: 1, Password: user2Password);
                    Logger.Instance.InfoLog("Pre-condition: User '" + user2UserName + "' created in domain '" + superAdminDomain + "'");
                }
                else
                    Logger.Instance.InfoLog("Pre-condition: User '" + user2UserName + "' is already available in domain '" + superAdminDomain + "'");
                login.Logout();

                //2. Step 27: Set CT layout as 2x2 as preference for user1.             
                login.NavigateAndLoginIConnect(login.lb_ica1_url, user1UserName, user1Password);
                userpref.OpenUserPreferences();
                userpref.SwitchToUserPrefFrame();               
                userpref.ModalityDropDown().SelectByText("CT");
                userpref.LayoutDropDown().SelectByText("2x2");
                userpref.CloseUserPreferences();
                Logger.Instance.InfoLog("Pre-conditon: User '" + user1UserName + "' - Layout set as 2x2 for CT in user preference");
                login.Logout();

                //3. Delete the downloaded study if exists.       
                if (File.Exists(downZipPath))
                {
                    Logger.Instance.InfoLog("Pre-conditon: Study exists at " + downZipPath);
                    File.Delete(downZipPath);
                    Logger.Instance.InfoLog("Pre-conditon: Study deleted");
                }

                //4. Delete PRs saved during step 3 & 13.
                //PID - 12310071:65432191530   
                if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer")))
                {
                    BasePage.Driver.Quit();
                    Config.BrowserType = "chrome";
                    Logger.Instance.InfoLog("Swicthing Browser Type to chrome");
                    BasePage.Driver = null;
                    login = new Login();
                    login.DriverGoTo(login.url);
                    changebrowser++;
                }
                login.DriverGoTo("https://" + Config.LB_Dest1IP + "/webadmin"); //Dest1
                hphome = (HPHomePage)hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, EA_URL: "https://" + Config.LB_Dest1IP + "/webadmin");
                workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                workflow.HPSearchStudy("PatientID", arrPatientID[0]);
                if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                {
                    workflow.DeletePaticularModality("PR");
                    Logger.Instance.InfoLog("Pre-conditon: All PRs deleted for PID - " + arrPatientID[0] + " from datasource - " + Config.LB_Dest1IP);
                }
                workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                workflow.HPSearchStudy("PatientID", arrPatientID[1]);
                if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                {
                    workflow.DeletePaticularModality("PR");
                    Logger.Instance.InfoLog("Pre-conditon: All PRs deleted for PID - " + arrPatientID[1] + " from datasource - " + Config.LB_Dest1IP);
                }
                hplogin.LogoutHPen();

                //5. Step 6: Delete study from Dest1. (transferred in previous execution)            
                BasePage.Driver.Navigate().GoToUrl("https://" + Config.LB_Dest1IP + "/webadmin");
                hphome = (HPHomePage)hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, EA_URL: "https://" + Config.LB_Dest1IP + "/webadmin");
                workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                workflow.HPSearchStudy("PatientID", arrPatientID[2]);
                if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                {
                    workflow.HPDeleteStudy();
                    Logger.Instance.InfoLog("Pre-conditon: Study transferred to Dest2 (step 6) is deleted");
                }
                hplogin.LogoutHPen();
                if (changebrowser != 0)
                {
                    BasePage.Driver.Quit();
                    Config.BrowserType = initialBrowserName;
                    Logger.Instance.InfoLog("Swicthing Back Browser to --" + initialBrowserName);
                    BasePage.Driver = null;
                    login = new Login();
                    login.DriverGoTo(login.url);
                }

                //6. Add save GSPS tool from Available items box to floating Toolbar menu's 2nd group.            
                login.NavigateAndLoginIConnect(login.lb_ica1_url, adminUserName, adminPassword);           
                login.Navigate("DomainManagement");
                domain.SearchDomain(superAdminDomain);
                domain.SelectDomain(superAdminDomain);
                domain.ClickEditDomain();     
                IWebElement group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)"));
                string saveImage = BluRingViewer.GetToolName(BluRingTools.Save_Annotated_Image);
                string saveSeries = BluRingViewer.GetToolName(BluRingTools.Save_Series);
                var dictionary1 = new Dictionary<String, IWebElement>();
                dictionary1.Add(saveImage, group2);
                dictionary1.Add(saveSeries, group2);
                domain.AddToolsToToolbox(dictionary1);
                Logger.Instance.InfoLog("Save Image and Save Series tool added to 2nd group of tools in ToolBox");
                domain.ClickSaveDomain();
                //Enable transfer for SuperRole
                role = (RoleManagement)login.Navigate("RoleManagement");
                role.SearchRole("SuperRole", Config.adminGroupName);
                role.EditRoleByName("SuperRole");
                PageLoadWait.WaitForFrameLoad(20);
                role.SetCheckboxInEditRole("transfer", 0);
                //Enable download for SuperRole 
                role.SetCheckboxInEditRole("download", 0);
                role.ClickSaveEditRole();
                login.Logout();

                //Step 1 - 
                //10.4.38.245 ica-s12-r3
                //10.4.39.99 ica-s12-r4
                //From a client machine, logon to iCA using iCA Server1 by using user u1.              
                login.NavigateAndLoginIConnect(login.lb_ica1_url, user1UserName, user1Password);
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 2 -
                //From the Studies page, load a study from one of the datasources.
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: arrAccession[0], Datasource: Dest1_ID);
                studies.SelectStudy("Accession", arrAccession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 3 - 
                //	Test Data: 
                //Save GSPS back to archive
                //-------------------------------- -
                //Draw few lines on a view port and then save.
                viewer.ClickOnViewPort(1, 1);
                viewer.OpenViewerToolsPOPUp();
                viewer.OpenStackedTool(BluRingTools.Line_Measurement, isOpenToolsPOPup: false);
                viewer.SelectViewerTool(BluRingTools.Line_Measurement, isOpenToolsPOPup: false);
                var attributes = viewer.GetElementAttributes(viewer.Activeviewport);
                Logger.Instance.InfoLog("Active VP Attributes");
                Logger.Instance.InfoLog("Width " + attributes["width"]);
                Logger.Instance.InfoLog("Height " + attributes["height"]);
                Logger.Instance.InfoLog("Start X " + attributes["width"] / 3);
                Logger.Instance.InfoLog("Start Y " + attributes["height"] / 3);
                Logger.Instance.InfoLog("End X " + attributes["width"] / 4);
                Logger.Instance.InfoLog("End Y " + attributes["height"] / 4);
                viewer.ApplyTool_LineMeasurement(attributes["width"] / 3, attributes["height"] / 3, attributes["width"] / 4, attributes["height"] / 4);              
                bool step3 = viewer.SavePresentationState(BluRingTools.Save_Series, BluRingTools.Interactive_Zoom);
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
                viewer.CloseBluRingViewer();

                //Step 4 - 
                //Test Data: 
                //Granted studies
                //---------------------------------
                //Navigate back to studylist, Grant Access this study to the user u2
                studies.SelectStudy("Accession", arrAccession[0]);
                Thread.Sleep(6000);
                PageLoadWait.WaitForFrameLoad(20);
                studies.GrantAccessToUsers(superAdminDomain, user2UserName);
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 5 - 
                //Select a study from a EA datasource.
                studies.SearchStudy(AccessionNo: arrAccession[2], Datasource: Dest2_ID);
                studies.SelectStudy("Accession", arrAccession[2]);
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 6 - 
                //Test Data: 
                //User Transfer study to different data source.
                //-------------------------------- -
                //Select Transfer button from the bottom of the study list.                
                ExecutedSteps++;
                try
                {
                    studies.TransferStudy(Dest1_ID, arrAccession[2]);
                    ExecutedSteps = ExecutedSteps + 2; //For Step 7 & 8
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Step 6 : Exception while transfering study from Dest1 to Dest2 --" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                    result.steps[ExecutedSteps].SetLogs();
                    studies.TransferStatusClose();
                }
                studies.SearchStudy(AccessionNo: arrAccession[2], Datasource: Dest1_ID);          
                Dictionary<int, string[]> SearchResults = BasePage.GetSearchResults();
                if (SearchResults.Count != 0)
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

                //Step 7 - 
                //	Test Data: 
                //User Transfer study to different data source.
                //-------------------------------- -
                //Select"Select All"button and Select MergePACS datasource from the drop down menu Transfer to:
                //Included in step 6

                //Step 8 - 
                //Select Transfer button.
                //(status: Succeeded)
                //Included in step 6

                //Step 9 - 
                //Logout from u1. Login to iCA using iCA Server2 by using user u2.
                login.Logout();          
                login.NavigateAndLoginIConnect(login.lb_ica2_url, user2UserName, user2Password);
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 10 - 
                //Go to Inbound tab and view the granted study from u1.
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(AccessionNo: arrAccession[0]);
                inbounds.SelectStudy("Accession", arrAccession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.CloseBluRingViewer();
                studies.SelectStudy("Accession", arrAccession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step10 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
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

                //Step 11 - 
                //Test Data: 
                //User downloads study to local system.
                //---------------------------------
                //Download the study to the local system by selecting Transfer button.
                inbounds.SelectStudy("Accession", arrAccession[0]);
                inbounds.TransferStudy("Local System", arrAccession[0]);                
                string[] files = Directory.GetFiles(downloadFolder + "\\");
                Logger.Instance.InfoLog("Files under download folder '" + downloadFolder + "' :");
                foreach (string file in files)
                {
                    Logger.Instance.InfoLog(Path.GetFileName(file));
                }
                bool studyDownload = false;
                if (files.Any(s => s.Contains(arrZipNames[0])))
                    studyDownload = true;           
                if (studyDownload)
                {                
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();                    
                }

                //Step 12 - 
                //Go back to the Studies page and load another study.
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: arrAccession[1], Datasource: Dest1_ID);
                studies.SelectStudy("Accession", arrAccession[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 13 - 
                //Test Data: 
                //Save GSPS back to archive
                //-------------------------------- -
                //Draw few lines on a view port and then save.
                viewer.ClickOnViewPort(1, 1);
                viewer.OpenViewerToolsPOPUp();
                viewer.OpenStackedTool(BluRingTools.Line_Measurement, isOpenToolsPOPup: false);
                viewer.SelectViewerTool(BluRingTools.Line_Measurement, isOpenToolsPOPup: false);
                attributes = viewer.GetElementAttributes(viewer.Activeviewport);
                if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer")))
                {
                    viewer.ApplyTool_LineMeasurement(20, 20, 45, 45);
                }
                else
                    viewer.ApplyTool_LineMeasurement(attributes["width"] / 3, attributes["height"] / 3, attributes["width"] / 4, attributes["height"] / 4);
                bool step13 = viewer.SavePresentationState(BluRingTools.Save_Series, BluRingTools.Interactive_Zoom);
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
                viewer.CloseBluRingViewer();

                //Step 14 - 
                //Go back to the Studies Page and reselect the study.
                studies.SelectStudy("Accession", arrAccession[1]);
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 15 - 
                //Select Grant Access button in the bottom of the Studies Page.
                PageLoadWait.WaitForFrameLoad(20);
                studies.GrantAccessToUsers(superAdminDomain, user1UserName);
                //For Step 15 & 16
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 16 - 
                //Add user u1 in Grant Access box
                //Select Grant Access button.
                //Included in step 15

                //Step 17 - 
                //Logout from u2. Login to iCA using iCA Server2 by using user u1.
                login.Logout();
                BasePage.Driver.Close();
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login = new Login();
                //login.DriverGoTo(login.lb_ica2_url);
                //login.LoginIConnect(user1UserName, user1Password);
                login.NavigateAndLoginIConnect(login.lb_ica2_url, user1UserName, user1Password);
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 18 -
                //Test Data: 
                //Granted studies
                //---------------------------------
                //Go to Inbound tab and view the granted study from u2.
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(AccessionNo: arrAccession[1]);
                inbounds.SelectStudy("Accession", arrAccession[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step18 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
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

                //Step 19 - 
                //Logout from u1. Login to iCA using iCA Server1 by using user u1.
                login.Logout();     
                login.NavigateAndLoginIConnect(login.lb_ica1_url, user1UserName, user1Password);
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 20 - 
                //From the Studies page, load a study with a report.
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: arrAccession[3], Datasource: Dest1_ID);
                studies.SelectStudy("Accession", arrAccession[3]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 21 - 
                //View the study's report.
                viewer.OpenReport_BR(0);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                IWebElement ReportContainer = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.ReportContainer_div));
                if (ReportContainer.Displayed)
                {
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

                //Step 22 - 
                //Test Data: 
                //Administrative user updates domain preferences
                //-------------------------------- -
                //While u1 continues its session in iCA Server1, from another client machine, 
                //logon to iCA using iCA Server2 by using Administrator.
                BasePage.MultiDriver = new List<IWebDriver>();
                BasePage.MultiDriver.Add(BasePage.Driver); //Add existing browser session to List                
                BasePage.MultiDriver.Add(login.InvokeBrowser(Config.BrowserType)); //Open another different Browser and add it to list
                login.SetDriver(BasePage.MultiDriver[1]);
                login.DriverGoTo(login.lb_ica2_url);
                BasePage.MultiDriver[1].Navigate().Refresh();
                //login.LoginIConnect(adminUserName, adminPassword);
                login.NavigateAndLoginIConnect(login.lb_ica2_url, adminUserName, adminPassword);
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 23 - 
                //Test Data: 
                //Administrative user updates domain preferences
                //-------------------------------- -
                //Go to Domain Management tab and edit SuperAdminGroup.
                //In the Default Settings Per Modality box, change:
                //Modality CT layout from 2x2 to 2x3
                //Select Add / Modify button to save the option.
                //Move Series Scope icon from Available items box to floating Toolbar menu.
                //Select Save button.
                login.Navigate("DomainManagement");
                domain.SearchDomain(superAdminDomain);
                domain.SelectDomain(superAdminDomain);
                domain.ClickEditDomain();
                domain.ModalityDropDown().SelectByText("CT");
                domain.LayoutDropDown().SelectByText("2x3");
                //Move series scope from available to floating Toolbar menu.
                IWebElement group = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                string seriesScope = BluRingViewer.GetToolName(BluRingTools.Series_Scope);
                var dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add(seriesScope, group);
                domain.AddToolsToToolbox(dictionary);                
                Logger.Instance.InfoLog("series scope tool  is configured in the ToolBox");
                domain.ClickSaveDomain();
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 24 -
                //Test Data: 
                //Administrative user updates role preferences
                //-------------------------------- -
                //Go to Role Management tab and edit SuperRole.
                //In the Access Filters Information box, select Modality CT from the drop down menu.Select Add button to save the option.Select Save button.               
                string modalityVal = "CT";
                role = (RoleManagement)login.Navigate("RoleManagement");              
                role.SearchRole(usersRole,superAdminDomain);
                role.SelectRole(usersRole);
                role.ClickEditRole();
                //Remove any old filter
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame(0);
                IList<IWebElement> SelectedFilter = role.SelectedFilterCriteria().Options;
                foreach (IWebElement filter in SelectedFilter)
                {
                    Logger.Instance.InfoLog("Old Access filter selected for removal - " + filter.Text);
                    filter.Click();
                    role.RoleAccessFilterRemoveBtn().Click();
                    Logger.Instance.InfoLog("Old Access filter removed");
                }
                role.AccessFiltersInformation().SelectByValue("Modality");
                role.ModalityFilter().DeselectAll();
                role.ModalityFilter().SelectByValue(modalityVal);
                role.AddAccessFilters().Click();
                role.ClickSaveEditRole();
                Logger.Instance.InfoLog("Access Filter set with Modality value - " + modalityVal);
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 25 - 
                //Back to u1 with iCA Server1, select Studies tab.
                login.SetDriver(BasePage.MultiDriver[0]);
                studies = (Studies)login.Navigate("Studies");       
                PageLoadWait.WaitForPageLoad(20);
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 26 - 
                //Test Data: 
                //\\trainingwks3 may not show all Modalities in query
                //-------------------------------- -
                //In the Last Name box, type"*"and select Search button
                bool modalityFilter = false;
                studies = (Studies)login.Navigate("Studies");             
                PageLoadWait.WaitForPageLoad(20);
                studies.SearchStudy(LastName: "*", AccessionNo: "*");
                String[] step26_Mod_Col = BasePage.GetColumnValues("Modality");
                if (step26_Mod_Col.Length > 0)
                {
                    foreach (string modality in step26_Mod_Col)
                    {
                        modalityFilter = false;
                        if (modality.Contains(modalityVal))
                        {
                            modalityFilter = true;
                        }
                    }                    
                }
                else
                {
                    Logger.Instance.ErrorLog("No study is listed after setting modality accession value as - " + modalityVal);                               
                }
                if (modalityFilter)
                {
                    Logger.Instance.InfoLog("Studies listed based on Modality value - " + modalityVal);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }                

                //Step 27 - 
                //Test Data: 
                //---------------------------------
                //Select a CT study to view.
                //The selected study is displayed successfully in 2x2 layout (Note: if U1's User Preferences has been modified/saved before, no changes in layout would be applied from the SuperAdminGroup)
                //Series Scope icon is available in the flaoting toolbox.
                studies.SelectStudy("Accession", arrAccession[5]);
                viewer = BluRingViewer.LaunchBluRingViewer();        
                if (viewer.GetViewPortCount(1) == 4)
                {
                    Logger.Instance.InfoLog("Studies listed based on Modality value - " + modalityVal);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Actual viewports count --" + viewer.GetViewPortCount(1));
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseBluRingViewer();                

                //Step 28 -
                //While u1 continues its session in iCA Server1, from another client machine, logon to iCA using iCA Server2 by using Administrator.
                login.SetDriver(BasePage.MultiDriver[1]); //Session available with admin login already from step 22
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Remove the access filter set in step 26, soo that id does not affect rest of the steps.
                role = (RoleManagement)login.Navigate("RoleManagement");
                role.SearchRole(usersRole, superAdminDomain);
                role.SelectRole(usersRole);
                role.ClickEditRole();
                //Remove any old filter
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame(0);
                SelectedFilter = role.SelectedFilterCriteria().Options;
                foreach (IWebElement filter in SelectedFilter)
                {
                    Logger.Instance.InfoLog("Old Access filter selected for removal - " + filter.Text);
                    filter.Click();
                    role.RoleAccessFilterRemoveBtn().Click();
                    Logger.Instance.InfoLog("Old Access filter removed");
                }
                role.ClickSaveEditRole();

                //Step 29 -
                //Select User Management Page and change User from u1 to u11 and select Save button.
                user = (UserManagement)login.Navigate("UserManagement");
                bool buser1 = user.SearchUser(user1UserName, superAdminDomain);
                user.SelectUser(user1UserName);
                user.ClickButtonInUser("edit");                
                user.EditUser(firstname: newUser1UserName, lastname: newUser1UserName, password: user1Password);
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(40);
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 30 -
                //Go to the Studies page and select a study.
                //Thread.Sleep(20000);
                studies = (Studies)login.Navigate("Studies");           
                studies.SearchStudy(AccessionNo: arrAccession[4], Datasource: Dest1_ID);
                studies.SelectStudy("Accession", arrAccession[4]);
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 31 -
                //Select Grant Access button in the bottom of the Studies Page.
                //For Step 31
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                Thread.Sleep(6000);
                PageLoadWait.WaitForFrameLoad(20);
                studies.GrantAccessToUsers(superAdminDomain, newUser1UserName);
                //For Step 32
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 32 -
                //Add user u11 in Grant Access box
                //Select Grant Access button
                //Included in step 31

                //Step 33 -
                //Test Data: 
                //Administrative user updates user information
                //-------------------------------- -
                //Go to Administrator's Outbound tab
                outbounds = (Outbounds)login.Navigate("Outbounds");
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(40);
                outbounds.SearchStudy(AccessionNo: arrAccession[4]);                
                outbounds.GetMatchingRow("Accession", arrAccession[4]).TryGetValue("Status", out studyStatus);
                if (studyStatus.ToLower() == "shared")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 34 -
                //Test Data: 
                //Administrative user updates user information
                //-------------------------------- -
                //Go back to u1's session and select Inbound tab
                login.SetDriver(BasePage.MultiDriver[0]);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(40);
                inbounds.SearchStudy(AccessionNo: arrAccession[4]);
                inbounds.GetMatchingRow("Accession", arrAccession[4]).TryGetValue("Status", out studyStatus);
                if (studyStatus.ToLower() == "shared")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 35 -
                //Log out all users.               
                login.SetDriver(BasePage.MultiDriver[1]); //Session 2 logout and close
                login.Logout();
                BasePage.MultiDriver[1].Close();
                BasePage.MultiDriver[1].Quit();
                BasePage.MultiDriver[1] = null;
                login.SetDriver(BasePage.MultiDriver[0]); //Session 1 logout
                login.Logout();
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Return Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                GC.Collect();
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
                //1. Revert multi driver
                try
                {
                    for (int counterX = (BasePage.MultiDriver.Count - 1); counterX > 0; counterX--)
                    {
                        if (BasePage.MultiDriver[counterX] != null)
                        {
                            BasePage.MultiDriver[counterX].Close();
                            BasePage.MultiDriver[counterX].Quit();
                            BasePage.MultiDriver[counterX] = null;
                            Logger.Instance.InfoLog("Finally: Revert multi driver - Completed for driver : " + counterX);
                        }
                        else
                            Logger.Instance.InfoLog("Finally: Revert multi driver - Driver already null for instance : " + counterX);
                    }
                    Logger.Instance.InfoLog("Finally: Revert multi driver - Completed");
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Finally: Revert multi driver - Exception");
                    Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                }

                //2. Remove access filter
                try
                {                 
                    login.NavigateAndLoginIConnect(login.lb_ica2_url, adminUserName, adminPassword);
                    role = (RoleManagement)login.Navigate("RoleManagement");
                    role.SearchRole(usersRole, superAdminDomain);
                    role.SelectRole(usersRole);                  
                    role.ClickEditRole();                    
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame(0);
                    IList<IWebElement> SelectedFilter = role.SelectedFilterCriteria().Options;
                    foreach (IWebElement filter in SelectedFilter)
                    {
                        Logger.Instance.InfoLog("Old Access filter selected for removal - " + filter.Text);
                        filter.Click();
                        role.RoleAccessFilterRemoveBtn().Click();
                        Logger.Instance.InfoLog("Old Access filter removed");
                    }
                    role.ClickSaveEditRole();
                    Logger.Instance.InfoLog("Finally: All access filters removed successfully");                    
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Finally: Remove access filter - Exception");
                    Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                }

                //3. Revert renamed user1
                try
                {              
                    login.NavigateAndLoginIConnect(login.lb_ica2_url, adminUserName, adminPassword);
                    user = (UserManagement)login.Navigate("UserManagement");                 
                    bool buser1 = user.SearchUser(newUser1UserName, superAdminDomain);
                    if (buser1)
                    {
                        user.SelectUser(newUser1UserName);
                        user.ClickButtonInUser("edit");
                        user.EditUser(firstname: user1UserName, lastname: user1UserName, password: user1Password);
                    }
                    Logger.Instance.ErrorLog("Finally: Revert renamed user1 - Not required. Updated name '" + newUser1UserName + "' is not found.");
                    login.Logout();
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Finally: Revert renamed user1 - Exception");
                    Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                }

                //4. Delete PRs from step 3 & 13
                try
                {
                    if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer")))
                    {
                        BasePage.Driver.Quit();
                        Config.BrowserType = "chrome";
                        Logger.Instance.InfoLog("Swicthing Browser Type to chrome");
                        BasePage.Driver = null;
                        login = new Login();
                        login.DriverGoTo(login.url);
                        changebrowser++;
                    }
                    //PID - 12310071:65432191530                           
                    login.DriverGoTo("https://" + Config.LB_Dest1IP + "/webadmin"); //Dest1
                    hphome = (HPHomePage)hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, EA_URL: "https://" + Config.LB_Dest1IP + "/webadmin");
                    workflow = (WorkFlow)hphome.Navigate("Workflow");
                    workflow.NavigateToLink("Workflow", "Archive Search");
                    workflow.HPSearchStudy("PatientID", arrPatientID[0]);
                    if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                    {
                        workflow.DeletePaticularModality("PR");
                        Logger.Instance.InfoLog("Finally: All PRs deleted for PID - " + arrPatientID[0] + " from datasource - "+ Config.LB_Dest1IP);
                    }
                    workflow = (WorkFlow)hphome.Navigate("Workflow");
                    workflow.NavigateToLink("Workflow", "Archive Search");
                    workflow.HPSearchStudy("PatientID", arrPatientID[1]);
                    if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                    {
                        workflow.DeletePaticularModality("PR");
                        Logger.Instance.InfoLog("Finally: All PRs deleted for PID - " + arrPatientID[1] + " from datasource - " + Config.LB_Dest1IP);
                    }
                    hplogin.LogoutHPen();
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Finally: Delete PRs from step 3 & 13 - Exception");
                    Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                }                

                //5. Revert User1 CT layout (2x2 to 2x3)
                try
                {                  
                    login.NavigateAndLoginIConnect(login.lb_ica1_url, user1UserName, user1Password);
                    userpref.OpenUserPreferences();
                    userpref.SwitchToUserPrefFrame();
                    userpref.ModalityDropDown().SelectByText("CT");
                    userpref.LayoutDropDown().SelectByText("2x3");
                    userpref.CloseUserPreferences();
                    Logger.Instance.InfoLog("Finally: User '" + user1UserName + "' - Layout set as 2x3 for CT in user preference");
                    login.Logout();
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Finally: Revert User1 CT layout (2x2 to 2x3) - Exception");
                    Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                }

                //6. Revert CT layout in domain management and remove series scope tool
                try
                {
                    login.NavigateAndLoginIConnect(login.lb_ica1_url, adminUserName, adminPassword);
                    login.Navigate("DomainManagement");
                    domain.SearchDomain(superAdminDomain);
                    domain.SelectDomain(superAdminDomain);
                    domain.ClickEditDomain();
                    domain.ModalityDropDown().SelectByText("CT");
                    domain.LayoutDropDown().SelectByText("2x2"); //Default
                    domain.ClickSaveDomain();
                    Logger.Instance.InfoLog("Finally: Revert Domain CT layout to 2x2 - Completed");
                    domain.SearchDomain(superAdminDomain);
                    domain.SelectDomain(superAdminDomain);
                    domain.ClickEditDomain();                    
                    var ToolsToBeRemoved = new List<String>();                 
                    ToolsToBeRemoved.Add(BluRingViewer.GetToolName(BluRingTools.Series_Scope));
                    domain.RemoveToolsFromConfiguredSection(ToolsToBeRemoved);
                    domain.ClickSaveDomain();
                    Logger.Instance.InfoLog("Finally: Remove series scope from configured to available - Completed");
                    login.Logout();
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Finally: Revert Domain CT layout to 2x2 and remove series scope from configured to available - Exception");
                    Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                }
                if (changebrowser != 0)
                {
                    BasePage.Driver.Quit();
                    Config.BrowserType = initialBrowserName;
                    Logger.Instance.InfoLog("Swicthing Back Browser to --" + initialBrowserName);
                    BasePage.Driver = null;
                    login = new Login();
                    login.DriverGoTo(login.url);
                }


            }
        }


        /// <summary>
        /// FailOver
        /// </summary>
        public TestCaseResult Test_161377(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            //variables
            Studies studies;
            UserManagement user;      
            BluRingViewer viewer;

            String adminUserName = Config.adminUserName;
            String adminPassword = Config.adminPassword;
            String superAdminDomain = Config.adminGroupName;
            String credentials = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Credentials");
            String user1UserName = credentials.Split(':')[0]; //"phuser01";
            String user1Password = credentials.Split(':')[1]; //"p";
            String systemUserName = credentials.Split(':')[2]; //"Administrator";
            String systemPassword = credentials.Split(':')[3]; //"Pa$$word";          
            String usersRole = "SuperRole";
            String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID"); //14343   
            String errorMsgs = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ValidationText");
            String [] arrErrorMsg = errorMsgs.Split(':');

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);               

                //Pre-condition: 
                //1. Create user phuser01, if not available already.
                //User can be created in any server. 
                login.NavigateAndLoginIConnect(login.lb_ica1_url, adminUserName, adminPassword);
                user = login.Navigate<UserManagement>();
                if (!user.IsUserExist(user1UserName, superAdminDomain))
                {
                    Logger.Instance.InfoLog("Pre-condition: User '" + user1UserName + "' is not available in domain '" + superAdminDomain + "'");
                    user.CreateUser(user1UserName, superAdminDomain, usersRole, hasPass: 1, Password: user1Password);
                    Logger.Instance.InfoLog("Pre-condition: User '" + user1UserName + "' created in domain '" + superAdminDomain + "'");
                }
                else
                    Logger.Instance.InfoLog("Pre-condition: User '" + user1UserName + "' is already available in domain '" + superAdminDomain + "'");            
                login.Logout();

                //Step 1 - 
                //Test Data: 
                //A Load Balancer is required in this section(Virtual IP: 10.4.38.147 is used).
                //For HTTP:
                //http://10.4.38.147/webaccess
                //For HTTPS:
                //https://icaf5ssh.merge.com/webaccess
                //To stop iCA web server: IISRESET / STOP is used.
                //To start iCA web server: IISRESET / START is used.
                //NOTE: 
                //1.Seamless FailOver is not supported.
                //2.ICA - LOADBALANCER_HTML4 and ICA-LOADBALANCER_HTTPS with Layer 7 are used.
                //---------------------------------
                //Remote logon to iCA Server2 and open a DOS command through a Command Prompt.
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 2
                //Type: IISRESET /STOP in the Command Prompt window. (ICA 2 stop)                           
                bool step2 = BasePage.RunRemoteCMDUsingPsExec(Config.LB_ICA2IP, systemUserName, "PQAte$t123-" + new BasePage().GetHostName(Config.LB_ICA2IP).ToLower(), "iisreset /stop"); //psexec.exe \\10.4.38.214 -u Administrator -p Pa$$word -accepteula -i 2 cmd /c "iisreset /stop" 
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
                //From a client machine, logon to iCA using Load Balancer (Virtual IP: 10.4.38.147 is used) by using user u1.              
                login.NavigateAndLoginIConnect(login.lburl, user1UserName, user1Password);
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 4
                //From the Studies page, load a study.
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: accession);
                studies.SelectStudy("Accession", accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.CloseBluRingViewer();
                studies.SelectStudy("Accession", accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step4)
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

                //Step 5
                //Logout from u1.
                login.Logout();
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 6
                //Remote logon to iCA Server1 and open a DOS command through a Command Prompt.
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 7
                //Type: IISRESET /STOP in the Command Prompt window. //Server 1 stop
                bool step7 = BasePage.RunRemoteCMDUsingPsExec(Config.LB_ICA1IP, systemUserName, "PQAte$t123-" + new BasePage().GetHostName(Config.LB_ICA1IP).ToLower(), "iisreset /stop"); //psexec.exe \\10.4.38.214 -u Administrator -p Pa$$word -accepteula -i 2 cmd /c "iisreset /stop"        
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

                //Step 8
                //From a client machine, logon to iCA using Load Balancer (Virtual IP: 10.4.38.147 is used) by using user u1. - loginpage should not be available
                login.DriverGoTo(login.lburl);
                if ( BasePage.Driver.PageSource.ToLower().Contains(arrErrorMsg[0]) || //Chrome error
                     BasePage.Driver.PageSource.ToLower().Contains(arrErrorMsg[1]) || //IE error1
                     BasePage.Driver.PageSource.ToLower().Contains(arrErrorMsg[3]) || //IE error2
                     BasePage.Driver.PageSource.ToLower().Contains(arrErrorMsg[2]) //FF error
                    )
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);                    
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 9
                //Remote logon to iCA Server2. In Command Prompt window, type IISRESET /START.
                bool step9 = BasePage.RunRemoteCMDUsingPsExec(Config.LB_ICA2IP, systemUserName, "PQAte$t123-" + new BasePage().GetHostName(Config.LB_ICA2IP).ToLower(), "iisreset /start"); //psexec.exe \\10.4.38.214 -u Administrator -p Pa$$word -accepteula -i 2 cmd /c "iisreset /start" 
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

                //Step 10
                //From a client machine, logon to iCA using Load Balancer (Virtual IP: 10.4.38.147 is used) by using user u1.
                login.NavigateAndLoginIConnect(login.lburl, user1UserName, user1Password);
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 11
                //From the Studies page, load a study.
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: accession);
                studies.SelectStudy("Accession", accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.CloseBluRingViewer();
                studies.SelectStudy("Accession", accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step11 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
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
                login.Logout();

                //Step 12
                //Remote logon to iCA Server1. In Command Prompt window, type IISRESET /START.     
                bool step12 = BasePage.RunRemoteCMDUsingPsExec(Config.LB_ICA1IP, systemUserName, "PQAte$t123-" + new BasePage().GetHostName(Config.LB_ICA1IP).ToLower(), "iisreset /start"); //psexec.exe \\10.4.38.214 -u Administrator -p Pa$$word -accepteula -i 2 cmd /c "iisreset /start" 
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

                //Return Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                GC.Collect();
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
                //Restart IIS in both ICA servers.
                try
                {
                    bool finally1 = BasePage.RunRemoteCMDUsingPsExec(Config.LB_ICA1IP, systemUserName, "PQAte$t123-" + new BasePage().GetHostName(Config.LB_ICA1IP).ToLower(), "iisreset");
                    bool finally2 = BasePage.RunRemoteCMDUsingPsExec(Config.LB_ICA2IP, systemUserName, "PQAte$t123-" + new BasePage().GetHostName(Config.LB_ICA2IP).ToLower(), "iisreset");
                    if (finally1)
                        Logger.Instance.InfoLog("Finally: IISReset completed in server 1 - " + Config.LB_ICA1IP);    
                    else
                        Logger.Instance.ErrorLog("Finally: IISReset failed in server 1 - " + Config.LB_ICA1IP);
                    if (finally2)
                        Logger.Instance.InfoLog("Finally: IISReset completed in server 2 - " + Config.LB_ICA2IP);
                    else
                        Logger.Instance.ErrorLog("Finally: IISReset failed in server 2 - " + Config.LB_ICA2IP);
                    Thread.Sleep(20000); //Wait sometime after IIS reset.   
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Finally: Restart IIS in both ICA servers - Exception");
                    Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                }
            }
        }


        /// <summary>
        /// FailOver - 2D View
        /// </summary>
        public TestCaseResult Test_161381(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            //variables
            Studies studies;
            UserManagement user;
            BluRingViewer viewer;
            DomainManagement domain = new DomainManagement();
            UserPreferences userpref = new UserPreferences();

            String adminUserName = Config.adminUserName;
            String adminPassword = Config.adminPassword;
            String superAdminDomain = Config.adminGroupName;
            String credentials = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Credentials");
            String user1UserName = credentials.Split(':')[0]; //"zuser01";
            String user1Password = credentials.Split(':')[1]; //"z";
            String systemUserName = credentials.Split(':')[2]; //"Administrator";
            String systemPassword = credentials.Split(':')[3]; //"Pa$$word";
            String domainName = "domain16381";
            String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID"); //14343   
            String errorMsgs = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ValidationText");
            String[] arrErrorMsg = errorMsgs.Split(':');
            bool newDomainCreated = false;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                //Step 1 - 
                //Pre-conditions: 
                //1.See Attachment for setup and Configuration Load Balancer
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 2 -
                //Pre - conditions:
                //1.A Load Balancer is required in this test(Virtual IP: 10.4.38.147 is used).
                //For HTTP: http://10.4.38.147/webaccess
                //For HTTPS: https://icaf5ssh.com/webaccess
                //2.Commands to stop or start iCA web server: 'IISRESET /STOP' or 'IISRESET /START'
                //3.Create a new domain/ role by logging in either iCA Server or a Load Balancer(Virtual IP: 10.4.38.147 is used).Example http://10.4.38.147/webaccess
                //NOTE:
                //1.Seamless FailOver is not supported.
                //2.ICA - LOADBALANCER_HTML4 and ICA-LOADBALANCER_HTTPS with Layer 7 are used.
                //Create Domain - if not available already.
                var domainattr = new Dictionary<Object, string>();
                domainattr.Add(DomainManagement.DomainAttr.DomainName, domainName);
                domainattr.Add(DomainManagement.DomainAttr.DomainDescription, domainName);
                domainattr.Add(DomainManagement.DomainAttr.InstitutionName, domainName);
                domainattr.Add(DomainManagement.DomainAttr.UserID, domainName);
                domainattr.Add(DomainManagement.DomainAttr.LastName, domainName);
                domainattr.Add(DomainManagement.DomainAttr.FirstName, domainName);
                domainattr.Add(DomainManagement.DomainAttr.EmailAddress, Config.CustomUser1Email);
                domainattr.Add(DomainManagement.DomainAttr.Password, domainName);
                domainattr.Add(DomainManagement.DomainAttr.RoleName, domainName);
                domainattr.Add(DomainManagement.DomainAttr.RoleDescription, domainName);
                login.NavigateAndLoginIConnect(login.lburl, adminUserName, adminPassword);            
                login.Navigate<DomainManagement>();             
                domain.SearchDomain(domainName);                
                if (domain.DomainExists(domainName))
                {
                    newDomainCreated = true;
                    Logger.Instance.InfoLog("Domain already exists with name as - " + domainName);
                }
                else
                {
                    domain.CreateDomain(domainattr, isimagesharingneeded: false, isemailstudy: false);
                    Logger.Instance.InfoLog("Domain created with name as - " + domainName);
                    newDomainCreated = true;
                }
                if (newDomainCreated)
                {
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

                //Step 3
                //Type: IISRESET /STOP in the Command Prompt window. (ICA 2 stop)                           
                bool step3 = BasePage.RunRemoteCMDUsingPsExec(Config.LB_ICA2IP, systemUserName, "PQAte$t123-" + new BasePage().GetHostName(Config.LB_ICA2IP).ToLower(), "iisreset /stop"); //psexec.exe \\10.4.38.214 -u Administrator -p Pa$$word -accepteula -i 2 cmd /c "iisreset /stop" 
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
                //From a client machine, logon to iCA using Load Balancer (Virtual IP: 10.4.38.147 is used) by as Administrator, create a new user(e.g.z1) in the newly created domain/ role from Pre-conditions.
                login.NavigateAndLoginIConnect(login.lburl, adminUserName, adminPassword);
                user = login.Navigate<UserManagement>();                
                if (!user.IsUserExist(user1UserName, domainName))
                {
                    Logger.Instance.InfoLog("Step 4: User '" + user1UserName + "' is not available in domain '" + domainName + "'");
                    user.CreateUser(user1UserName, domainName, domainName, hasPass: 1, Password: user1Password);
                    Logger.Instance.InfoLog("Step 4: User '" + user1UserName + "' created in domain '" + domainName + "'");
                }
                else
                    Logger.Instance.InfoLog("Step 4: User '" + user1UserName + "' is already available in domain '" + domainName + "'");
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                login.Logout();

                //Step 5
                //Logon to iCA using Load Balancer (Virtual IP: 10.4.38.147 is used) by as the newly created user z1.
                login.NavigateAndLoginIConnect(login.lburl, user1UserName, user1Password);
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 6
                //From User preferences page, set Universal viewer as default viewer
                userpref.OpenUserPreferences();
                userpref.SwitchToUserPrefFrame();
                login.ClickElement(userpref.BluringViewerRadioBtn());          
                userpref.CloseUserPreferences();
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                
                //Step 7
                //From the Studies page, load a study.
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: accession);
                studies.SelectStudy("Accession", accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.CloseBluRingViewer();
                studies.SelectStudy("Accession", accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
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

                //Step 8
                //Ensure the 2D viewer session remains alive. Remote logon to iCA Server2. In Command Prompt window, type IISRESET /START.
                bool step8 = BasePage.RunRemoteCMDUsingPsExec(Config.LB_ICA2IP, systemUserName, "PQAte$t123-" + new BasePage().GetHostName(Config.LB_ICA2IP).ToLower(), "iisreset /start"); //psexec.exe \\10.4.38.214 -u Administrator -p Pa$$word -accepteula -i 2 cmd /c "iisreset /start" 
                if (step8)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 9
                //Ensure the 2D viewer session remains alive. Remote logon to iCA Server1 system, in Command Prompt window, type IISRESET /STOP.
                bool step9 = BasePage.RunRemoteCMDUsingPsExec(Config.LB_ICA1IP, systemUserName, "PQAte$t123-" + new BasePage().GetHostName(Config.LB_ICA1IP).ToLower(), "iisreset /stop"); //psexec.exe \\10.4.38.214 -u Administrator -p Pa$$word -accepteula -i 2 cmd /c "iisreset /stop" 
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

                //Step 10
                //Go back to the client machine, close the viewer and verify the user z1's iCA client session opened from above steps.
                //Expected - The current logon ica user z1 is brought back from the 2D view to iCA login page upon the current active ICA server be running.
                viewer.CloseBluRingViewer();
                bool step10 = false;       
                int counterX = 0;
                do
                {
                    try
                    {                            
                        PageLoadWait.WaitForPageLoad(30);
                        PageLoadWait.WaitForFrameLoad(20);
                        BasePage.Driver.SwitchTo().DefaultContent();
                        BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id$='_LoginMasterContentPlaceHolder_Username']")));
                        if (login.LoginBtn().Displayed &&
                        login.UserIdTxtBox().Displayed &&
                        login.PasswordTxtBox().Displayed)
                            step10 = true;
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.ErrorLog("Step 10: Exception while waiting for login page after closing viewer");
                        Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);                       
                    }
                }
                while (!step10 && counterX++ < 2);                
                if (step10)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test case Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 11
                //Logon to iCA using Load Balancer (Virtual IP: 10.4.38.147 is used) by as the same user z1.
                login.NavigateAndLoginIConnect(login.lburl, user1UserName, user1Password);
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 12
                //Load a study.
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: accession);
                studies.SelectStudy("Accession", accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.CloseBluRingViewer();
                studies.SelectStudy("Accession", accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step12 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
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
                viewer.CloseBluRingViewer();

                //Return Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                GC.Collect();
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
                //Restart IIS in both ICA servers.
                try
                {
                    bool finally1 = BasePage.RunRemoteCMDUsingPsExec(Config.LB_ICA1IP, systemUserName, "PQAte$t123-" + new BasePage().GetHostName(Config.LB_ICA1IP).ToLower(), "iisreset");
                    bool finally2 = BasePage.RunRemoteCMDUsingPsExec(Config.LB_ICA2IP, systemUserName, "PQAte$t123-" + new BasePage().GetHostName(Config.LB_ICA2IP).ToLower(), "iisreset");
                    if (finally1)
                        Logger.Instance.InfoLog("Finally: IISReset completed in server 1 - " + Config.LB_ICA1IP);
                    else
                        Logger.Instance.ErrorLog("Finally: IISReset failed in server 1 - " + Config.LB_ICA1IP);
                    if (finally2)
                        Logger.Instance.InfoLog("Finally: IISReset completed in server 2 - " + Config.LB_ICA2IP);
                    else
                        Logger.Instance.ErrorLog("Finally: IISReset failed in server 2 - " + Config.LB_ICA2IP);
                    Thread.Sleep(20000); //Wait sometime after IIS reset.   
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Finally: Restart IIS in both ICA servers - Exception");
                    Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                }
            }
        }


        /// <summary>
        /// FailOver - 3D View
        /// </summary>
        public TestCaseResult Test_164776(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            //variables
            Studies studies;
            UserManagement user;
            BluRingViewer viewer;
            DomainManagement domain = new DomainManagement();
            UserPreferences userpref = new UserPreferences();
            BluRingZ3DViewerPage z3dViewer = new BluRingZ3DViewerPage();
            RoleManagement role;

            String adminUserName = Config.adminUserName;
            String adminPassword = Config.adminPassword;
            String superAdminDomain = Config.adminGroupName;
            String credentials = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Credentials");
            String user1UserName = credentials.Split(':')[0]; //"zuser01";
            String user1Password = credentials.Split(':')[1]; //"z";
            String systemUserName = credentials.Split(':')[2]; //"Administrator";
            String systemPassword = credentials.Split(':')[3]; //"Pa$$word";
            String domainName = "domain16381";
            String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID"); //14343   
            String errorMsgs = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ValidationText");
            String[] arrErrorMsg = errorMsgs.Split(':');
            bool newDomainCreated = false;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                //Step 1 - 
                //Pre - conditions -
                //1.Z3D rendering server is installed on all ICA servers (iCA Server1 and ICA Server2)
                //2.See Attachment for setup and Configuration Load Balancer
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 2 -
                //Pre - conditions:
                //1.A Load Balancer is required in this test(Virtual IP: 10.4.38.147 is used).
                //For HTTP: http://10.4.38.147/webaccess
                //For HTTPS: https://icaf5ssh.com/webaccess
                //2.Commands to stop or start iCA web server: 'IISRESET /STOP' or 'IISRESET /START'
                //3.Create a new domain/ role by logging in either iCA Server or a Load Balancer(Virtual IP: 10.4.38.147 is used).Example http://10.4.38.147/webaccess
                //NOTE:
                //1.Seamless FailOver is not supported.
                //2.ICA - LOADBALANCER_HTML4 and ICA-LOADBALANCER_HTTPS with Layer 7 are used.
                //Create Domain - if not available already.
                var domainattr = new Dictionary<Object, string>();
                domainattr.Add(DomainManagement.DomainAttr.DomainName, domainName);
                domainattr.Add(DomainManagement.DomainAttr.DomainDescription, domainName);
                domainattr.Add(DomainManagement.DomainAttr.InstitutionName, domainName);
                domainattr.Add(DomainManagement.DomainAttr.UserID, domainName);
                domainattr.Add(DomainManagement.DomainAttr.LastName, domainName);
                domainattr.Add(DomainManagement.DomainAttr.FirstName, domainName);
                domainattr.Add(DomainManagement.DomainAttr.EmailAddress, Config.CustomUser1Email);
                domainattr.Add(DomainManagement.DomainAttr.Password, domainName);
                domainattr.Add(DomainManagement.DomainAttr.RoleName, domainName);
                domainattr.Add(DomainManagement.DomainAttr.RoleDescription, domainName);
                login.NavigateAndLoginIConnect(login.lburl, adminUserName, adminPassword);
                login.Navigate<DomainManagement>();
                domain.SearchDomain(domainName);
                if (domain.DomainExists(domainName))
                {
                    newDomainCreated = true;
                    Logger.Instance.InfoLog("Domain already exists with name as - " + domainName);                    
                }
                else
                {
                    domain.CreateDomain(domainattr, isimagesharingneeded: false, isemailstudy: false);
                    Logger.Instance.InfoLog("Domain created with name as - " + domainName);
                    newDomainCreated = true;
                }             
                domain.SearchDomain(domainName);
                domain.SelectDomain(domainName);
                domain.ClickEditDomain();
                domain.Enable3DView();
                domain.ClickSaveNewDomain();
                PageLoadWait.WaitForPageLoad(20);
                Logger.Instance.InfoLog("Z3D view enabled for domain - " + domainName);
                if (newDomainCreated)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Alse Enable 3D view in Role management
                role = login.Navigate<RoleManagement>();
                role.SearchRole(domainName, domainName);
                role.SelectRole(domainName);
                role.ClickEditRole();
                role.SetCheckboxInEditRole("3dview", 0);
                role.ClickSaveEditRole();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                Logger.Instance.InfoLog("Z3D view enabled for Role - " + domainName);
                login.Logout();

                //Step 3
                //Remote logon to iCA Server2 system.Type: IISRESET /STOP in the Command Prompt window. (ICA 2 stop)                           
                bool step3 = BasePage.RunRemoteCMDUsingPsExec(Config.LB_ICA2IP, systemUserName, "PQAte$t123-" + new BasePage().GetHostName(Config.LB_ICA2IP).ToLower(), "iisreset /stop"); //psexec.exe \\10.4.38.214 -u Administrator -p Pa$$word -accepteula -i 2 cmd /c "iisreset /stop" 
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
                //From a client machine, logon to iCA using Load Balancer (Virtual IP: 10.4.38.147 is used) by as Administrator, create a new user(e.g.z1) in the newly created domain/ role from Pre-conditions.
                login.NavigateAndLoginIConnect(login.lburl, adminUserName, adminPassword);
                user = login.Navigate<UserManagement>();
                if (!user.IsUserExist(user1UserName, domainName))
                {
                    Logger.Instance.InfoLog("Step 4: User '" + user1UserName + "' is not available in domain '" + domainName + "'");
                    user.CreateUser(user1UserName, domainName, domainName, hasPass: 1, Password: user1Password);
                    Logger.Instance.InfoLog("Step 4: User '" + user1UserName + "' created in domain '" + domainName + "'");
                }
                else
                    Logger.Instance.InfoLog("Step 4: User '" + user1UserName + "' is already available in domain '" + domainName + "'");
                //Enable 3d View for the user
                user.SearchUser(user1UserName, domainName);            
                user.SelectUser(user1UserName);
                user.ClickButtonInUser("edit");
                role.SetCheckboxInEditRole("3dview", 0);
                new BasePage().ClickElement(user.SaveBtn());            
                PageLoadWait.WaitForPageLoad(40);
                PageLoadWait.WaitForFrameLoad(40);
                Logger.Instance.InfoLog("Z3D view enabled for user - " + user1UserName);
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                login.Logout();

                //Step 5
                //Logon to iCA using Load Balancer (Virtual IP: 10.4.38.147 is used) by as the newly created user z1.
                login.NavigateAndLoginIConnect(login.lburl, user1UserName, user1Password);
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 6
                //From the Studies page, load a study with Z3D supported series (CT, MR or PET, series has at least 15 images)
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: accession);
                studies.SelectStudy("Accession", accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitForPriorsToLoad();
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 7
                //Select a 3D supported series, switch to Z3D viewer by selecting 3D from the smartview dropdown menu on the study panel toolbar.
                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(1, 1)).Click();
                if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer")))
                {
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector('#mat-select-2 > div > div.mat-select-arrow-wrapper').click()");
                    Thread.Sleep(5000);
                    PageLoadWait.WaitForElementToDisplay(z3dViewer.DropDownBox3D());
                    IList<IWebElement> weli = z3dViewer.layoutlist();
                    string str;
                    foreach (IWebElement we in weli)
                    {
                        str = we.Text;
                        if (str.Equals("MPR"))
                        {
                            z3dViewer.ClickElement(we);
                            break;
                        }
                    }
                    PageLoadWait.WaitForProgressBarToDisAppear();
                    PageLoadWait.WaitForFrameLoad(20);
                }
                else
                    z3dViewer.select3dlayout("MPR", "y"); //Choose Z3D view  
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
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

                //Step 8
                //Ensure the Z3D viewer session remains alive. Remote logon to iCA Server2. In Command Prompt window, type IISRESET /START.
                bool step8 = BasePage.RunRemoteCMDUsingPsExec(Config.LB_ICA2IP, systemUserName, "PQAte$t123-" + new BasePage().GetHostName(Config.LB_ICA2IP).ToLower(), "iisreset /start"); //psexec.exe \\10.4.38.214 -u Administrator -p Pa$$word -accepteula -i 2 cmd /c "iisreset /start" 
                if (step8)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 9
                //Ensure the Z3D viewer session remains alive. Remote logon to iCA Server1 system, in Command Prompt window, type IISRESET /STOP.
                bool step9 = BasePage.RunRemoteCMDUsingPsExec(Config.LB_ICA1IP, systemUserName, "PQAte$t123-" + new BasePage().GetHostName(Config.LB_ICA1IP).ToLower(), "iisreset /stop"); //psexec.exe \\10.4.38.214 -u Administrator -p Pa$$word -accepteula -i 2 cmd /c "iisreset /stop" 
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

                //Step 10
                //Go back to the client machine, close the viewer and verify the user z1's iCA client session opened from above steps.
                //Expected - The current logon ica user z1 is brought back from the 2D view to iCA login page upon the current active ICA server be running.
                viewer.CloseBluRingViewer();
                bool step10 = false;
                try
                {
                    PageLoadWait.WaitForPageLoad(30);
                    PageLoadWait.WaitForFrameLoad(20);
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id$='_LoginMasterContentPlaceHolder_Username']")));
                    if (login.LoginBtn().Displayed &&
                    login.UserIdTxtBox().Displayed &&
                    login.PasswordTxtBox().Displayed)
                        step10 = true;
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Step 10: Exception while waiting for login page after closing viewer");
                    Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                }
                if (step10)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test case Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 11
                //Logon to iCA using Load Balancer (Virtual IP: 10.4.38.147 is used) by as the same user z1.
                login.NavigateAndLoginIConnect(login.lburl, user1UserName, user1Password);
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 12
                //Load a study with Z3D supported series. Launch Z3D
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: accession);
                studies.SelectStudy("Accession", accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitForPriorsToLoad();
                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(1, 1)).Click();
                PageLoadWait.WaitForFrameLoad(40);
                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(1, 1)).Click();
                if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer")))
                {
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector('#mat-select-2 > div > div.mat-select-arrow-wrapper').click()");                  
                    Thread.Sleep(5000);
                    PageLoadWait.WaitForElementToDisplay(z3dViewer.DropDownBox3D());                   
                    IList<IWebElement> weli = z3dViewer.layoutlist();
                    string str;
                    foreach (IWebElement we in weli)
                    {
                        str = we.Text;
                        if (str.Equals("MPR"))
                        {
                            z3dViewer.ClickElement(we);
                            break;
                        }
                    }           
                    PageLoadWait.WaitForProgressBarToDisAppear();
                    PageLoadWait.WaitForFrameLoad(20);
                }
                else
                    z3dViewer.select3dlayout("MPR", "y"); //Choose Z3D view  
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step12 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
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
                viewer.CloseBluRingViewer();

                //Return Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                GC.Collect();
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
                //Restart IIS in both ICA servers.
                try
                {
                    bool finally1 = BasePage.RunRemoteCMDUsingPsExec(Config.LB_ICA1IP, systemUserName, "PQAte$t123-" + new BasePage().GetHostName(Config.LB_ICA1IP).ToLower(), "iisreset");
                    bool finally2 = BasePage.RunRemoteCMDUsingPsExec(Config.LB_ICA2IP, systemUserName, "PQAte$t123-" + new BasePage().GetHostName(Config.LB_ICA2IP).ToLower(), "iisreset");
                    if (finally1)
                        Logger.Instance.InfoLog("Finally: IISReset completed in server 1 - " + Config.LB_ICA1IP);
                    else
                        Logger.Instance.ErrorLog("Finally: IISReset failed in server 1 - " + Config.LB_ICA1IP);
                    if (finally2)
                        Logger.Instance.InfoLog("Finally: IISReset completed in server 2 - " + Config.LB_ICA2IP);
                    else
                        Logger.Instance.ErrorLog("Finally: IISReset failed in server 2 - " + Config.LB_ICA2IP);
                    Thread.Sleep(20000); //Wait sometime after IIS reset.   
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Finally: Restart IIS in both ICA servers - Exception");
                    Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                }
            }
        }

    }//Class End

}
