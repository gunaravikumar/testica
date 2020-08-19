using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.MergeServiceTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Drawing;
using System.Threading;
using Selenium.Scripts.Reusable.Generic;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.eHR;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.Putty;
using TestStack.White.Configuration;
using TestStack.White.Factory;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.TableItems;
using TestStack.White.InputDevices;
using TestStack.White.WindowsAPI;
using Application = TestStack.White.Application;
using System.Diagnostics;
using System.Collections.ObjectModel;
using ComboBox = TestStack.White.UIItems.ListBoxItems.ComboBox;
using System.Xml;
using OpenQA.Selenium.Remote;
using Dicom.Network;
using System.Windows.Automation;

namespace Selenium.Scripts.Tests
{    

    class ExternalApplications : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
                
        public ExternalApplications(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            wpfobject = new WpfObjects();
        }

        /// <summary>
        /// UserRole-Halo Viewer and Versics
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_163191(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables                      
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            int changebrowser = 0;
            String initialBrowserName = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();

            try
            {

                //Fetch required Test data                
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String accession1 = accession.Split(':')[0];
                String accession2 = accession.Split(':')[1];
                String accession3 = accession.Split(':')[2];
                String patientID3 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String studyPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                string[] arrStudyPath = studyPath.Split('=');
                String imagesName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "imageName");
                string[] arrImagesName = imagesName.Split('=');
                String domainName = "SuperAdminGroup";
                String roleName1 = BasePage.GetUniqueRole("Role");
                String userName1 = BasePage.GetUniqueUserId("User");
                String avLiteHost = Config.RadSuiteIp;
                String avLite = Config.RadSuiteId;
                String avLiteSelectTxt = Config.RadsuiteName;
                String rsUserName = Config.RadSuiteUser;
                String rsPassword = Config.RadSuitePass;
                String halo = Config.HaloId;
                String haloSelectTxt = Config.HaloName;
                String mpacsUserName = Config.HaloUser;
                String mpacsPassword = Config.HaloPass;
                String vericisSelectTxt = Config.VericisName;
                String cardioUserName = Config.VericisUser;
                String cardioPassword = Config.VericisPass;

                //Objects
                RoleManagement roleManagement;
                UserManagement userManagement;
                Studies studies;

                //Change to IE browser                
                if (!SBrowserName.ToLower().Contains("explorer"))
                {
                    BasePage.Driver.Quit();
                    Config.BrowserType = "internet explorer";
                    Logger.Instance.InfoLog("Swicthing Browser Type to internet explorer");
                    BasePage.Driver = null;
                    login = new Login();
                    login.DriverGoTo(login.url);
                    changebrowser++;
                }

                //Step 1 - pre-condition: Refer to test case '1.0 External Application: Configure'
                //Test_27586
                //Send Studies
                try
                {

                    //To Halo
                    DicomClient client = new DicomClient();
                    BasePage.RunBatchFile(Config.batchfilepath, arrStudyPath[1] + " " + Config.dicomsendpath + " " + Config.HaloIp);
                    Logger.Instance.InfoLog("Study sent to Halo");

                    //To Cardio EA                
                    client.AddRequest(new DicomCStoreRequest(string.Concat(arrStudyPath[2], arrImagesName[2])));
                    client.Send(Config.VericisEAIp, 12000, false, "SCU", "ECM_ARC_" + Config.VericisEAIp.Split('.')[3]);
                    Logger.Instance.InfoLog("Study sent to Cardio EA");
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Error while sending studies " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                }
                ExecutedSteps++;

                //Step 2 - Login iConnect Access as Administrator and select Role Management
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                roleManagement = (RoleManagement)login.Navigate("RoleManagement");
                ExecutedSteps++;
                //Step 3 - Edit System Admin Role.  
                roleManagement.SelectDomainfromDropDown(Config.adminGroupName);
                PageLoadWait.WaitForFrameLoad(10);
                roleManagement.SearchRole("SuperRole");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                roleManagement.EditRoleByName("SuperRole");
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame(0);
                ExecutedSteps++;
                //Step 4 - In External Applications section, add all the external applications from the Disconnected box to Connected box by selecting the applications and then Connect button.               
                bool saveNeeded = false;
                if (roleManagement.List_ConnectExternalApplications().Count != 0)
                {

                    roleManagement.ConnectExternalApplications();
                    saveNeeded = true;
                }
                if (roleManagement.List_ConnectExternalApplications().Count == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 5 - Select Save                  
                if (saveNeeded)
                    roleManagement.ClickSaveEditRole();
                else
                    roleManagement.CloseRoleManagement();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                ExecutedSteps++;

                //Step 6 - Select  New Role button.
                //In next step 

                //Step 7 - In Domain Name, select SuperAdminGroup. In Role Information, select SuperRole In Role Name, type user1 In Role Description: user1 In External Applications section, disconnect all external applications.Save the settings                               
                roleManagement.CreateRoleByCopy("SuperAdminGroup", "SuperRole", roleName1, roleName1);
                ExecutedSteps++;
                roleManagement.SelectDomainfromDropDown(Config.adminGroupName);
                PageLoadWait.WaitForFrameLoad(10);
                roleManagement.SearchRole(roleName1);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                roleManagement.EditRoleByName(roleName1);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                roleManagement.DisconnectExternalApplications();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(3000);
                roleManagement.ClickSaveEditRole();
                ExecutedSteps++;

                //Step 8 - Select  User Management page.
                userManagement = (UserManagement)login.Navigate("UserManagement");
                ExecutedSteps++;

                //Step 9 - Select New User button                
                //In next step

                //Step 10 - Type u1 for the following boxes: User ID, Last Name, First Name, Password, and Confirm Password.In Role Information, select pull down menu and select user1 and leave other fields as default settings.Save the settings                
                userManagement.CreateUser(userName1, roleName1, hasPass: 1, Password: userName1, FName: userName1, LName: userName1);
                ExecutedSteps = ExecutedSteps + 2;

                //Step 11 - Logout Administrator and Login as u1.
                login.Logout();
                login.LoginIConnect(userName1, userName1);
                ExecutedSteps++;
                //Step 12 - Load a study. External application drop down not available             
               // studies = (Studies)login.Navigate("Studies");
                Studies studies1 = new Studies();
                Boolean step12 = false;
                try
                {
                    if (studies1.ExternalApp_Select().Displayed)
                        step12 = true;
                }
                catch (Exception)
                { }

                //if (!studies.ExternalApp_Select().Displayed)
                if (!step12)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //load a study
                StudyViewer viewer;
                try
                {
                    studies1.SearchStudy(AccessionNo: accession1);
                    studies1.SelectStudy("Accession", accession1);
                    viewer = StudyViewer.LaunchStudy();
                    PageLoadWait.WaitForThumbnailsToLoad(90);
                    PageLoadWait.WaitForAllViewportsToLoad(90);
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Step12: Unable to launch study . " + e);
                }

                //Step 13 - Logout u1 and Login as Administrator.Edit user1 Role under Role Management pageIn External Applications section, select AV-Lite from Disconnected box and then Connect button.Save the settings.
                login.Logout();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                roleManagement = (RoleManagement)login.Navigate("RoleManagement");
                roleManagement.DomainDropDown().SelectByValue(domainName);
                PageLoadWait.WaitForFrameLoad(10);
                roleManagement.SearchRole(roleName1);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                roleManagement.EditRoleByName(roleName1);
                //roleManagement.ConnectExternalApplications(Config.HaloName1);
                roleManagement.ClickSaveEditRole();
                ExecutedSteps++;

                //Step 14 - Edit User u1. u1 User page is displayed.In External Applications section, AV-Lite is in Connected box and all others external applications are in Disconnected box.
                userManagement = (UserManagement)login.Navigate("UserManagement");
                userManagement.SearchUser(userName1, domainName);
                userManagement.SelectUser(userName1);
                userManagement.ClickEditUser();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame(0);
                if (userManagement.List_ConnectedExternalApplications().Count == 1 && userManagement.List_ConnectedExternalApplications()[0].Text.Equals(halo))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 15 - Select  Close button.
                PageLoadWait.WaitForPageLoad(10);
                //userManagement.CloseBtn().Click();
                ClickElement(userManagement.CloseBtn());
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                ExecutedSteps++;

                //Step 16 - Logout Administrator and Login as u1.The drop down menu box and Launch button are displayed but grey out.
                login.Logout();
                login.LoginIConnect(userName1, userName1);
                //studies = (Studies)login.Navigate("Studies");
                if (!studies1.ExternalApp_Select().Enabled && !studies1.LauchStudyExtApp_Btn().Enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step 17 - Select a study by single click. The selected study is highlighted and the --Select Applications-- box and Launch button are enabled.Only one external application is available is AV - Lite.
                studies1.SearchStudy(AccessionNo: accession1);
                studies1.SelectStudy("Accession", accession1);
                SelectElement externalAppDropDown = new SelectElement(studies1.ExternalApp_Select());
                IList<IWebElement> ExternalAppOptions = externalAppDropDown.Options;
                if (studies1.ExternalApp_Select().Enabled &&
                    studies1.LauchStudyExtApp_Btn().Enabled &&
                    ExternalAppOptions.Count == 1 &&
                    ExternalAppOptions[0].Text.ToUpper().Equals(haloSelectTxt.ToUpper()))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step 18 - Select AV-Lite application and click on Launch button. Credential pop-up appears
                ExecutedSteps++;
                

                //Step 19 - Logout u1 and Login as Administrator.Select Studies tab,Select a study by single click.

                login.Logout();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                roleManagement = (RoleManagement)login.Navigate("RoleManagement");
                roleManagement.DomainDropDown().SelectByValue(domainName);
                PageLoadWait.WaitForFrameLoad(10);
                roleManagement.SearchRole("SuperRole");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                roleManagement.EditRoleByName("SuperRole");
                roleManagement.ConnectExternalApplications();
                roleManagement.ClickSaveEditRole();
                ExecutedSteps++;
                studies1 = (Studies)login.Navigate("Studies");
                studies1.SearchStudy(AccessionNo: accession1);
                studies1.SelectStudy("Accession", accession1);
                //Step:20- Load the selected study on HTML5/Universal viewer
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForThumbnailsToLoad(90);
                PageLoadWait.WaitForAllViewportsToLoad(90);
                ExecutedSteps++;


                //Step 21 & 22 - 1. Make sure test client log in as Administrator on the  test machine. test machine.
                //2.Make sure th e test browser is ActiveX contrl enable.For IE browser can be enabled from Internet Options-*^>^ *Security tab - *^>^ *custom LevelSelect Vericis application from the drop down box and select Launch button.

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame(0);
                String vericisLaunchText = "Launch" + Config.VericisName.Replace(" ", "");
                ExecutedSteps++;
                ExecutedSteps++;

                //Step 23 - Enter the followings:user: products\qaphysician and password: Pa$$word.The images are loaded in the Vericis's browser
                //Step verfied in next step:
                ExecutedSteps++;

                //Step 24 - Close the Vericis's browser. Go back to the Study List and select another study by single click
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame(0);
                viewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                studies1.SearchStudy(AccessionNo: accession2);
                studies1.SelectStudy("Accession", accession2);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForThumbnailsToLoad(90);
                PageLoadWait.WaitForAllViewportsToLoad(90);

                ExecutedSteps++;

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
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
                try
                {
                    if (changebrowser != 0)
                    {
                        BasePage.Driver.Quit();
                        Config.BrowserType = initialBrowserName;
                        Logger.Instance.InfoLog("Swicthing Back Browser to --" + initialBrowserName);
                        BasePage.Driver = null;
                        login = new Login();
                        login.DriverGoTo(login.url);
                    }
                    BasePage.KillProcess("jp2launcher");
                    BasePage.KillProcess("aViewer");
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Exception in finally.. " + e);
                }
            }


        }
        /// <summary>
        /// Encrypt URL
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_163193(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables                      
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {

                //Fetch required Test data                
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String encryptionKey = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ConfigDetails");
                DateTime dateNow = DateTime.Now;
                string date = dateNow.ToString("yyMMddHmm");
                String encryptionProvider = "HALOTripleDESKeyProvider_" + date;

                //Objects        
                ServiceTool servicetool;

                //Step 1 - pre-condition: Refer to test case '1.0 External Application
                //Setup Halo External Application
                ExecutedSteps++;

                //Step 2 - Set up Default encryption provider
                //CEcEtTMC3NT1+y0iStlKIYY6oatqdibP
                //Add encryption service
                new Taskbar().Hide();
                servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEncryption();
                servicetool.SetEncryptionEncryptionService();
                servicetool.Add().Click();
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindowByTitle("Service Entry Form");
                servicetool.key_txt().Text = "TripDES.Halo";
                servicetool.assembly_txt().Text = "OpenContent.Generic.Core.dll";
                servicetool.class_txt().Text = "OpenContent.Core.Security.Services.TripleDES";
                wpfobject.WaitTillLoad();
                wpfobject.GetButton("Apply", 1).Click();
                servicetool.EnterServiceParameters("Key", "string", encryptionKey);
                servicetool.EnterServiceParameters("Iv", "string", "");
                servicetool.EnterServiceParameters("characterset", "string", "Windows-1252");
                wpfobject.GetMainWindowByTitle("Service Entry Form");
                servicetool.EnterServiceParameters("operationMode", "string", "CBC");
                servicetool.EnterServiceParameters("paddingMode", "string", "Zeros");
                wpfobject.GetMainWindowByTitle("Service Entry Form");
                wpfobject.GetButton("OK", 1).Click();
                wpfobject.WaitTillLoad();
                //servicetool.RestartIISandWindowsServices();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                //new Taskbar().Show();
                Logger.Instance.InfoLog("TripDES.Halo Encryption service entry completed");
                //Add created encryption to Default encryption
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEncryption();
                servicetool.EnterEncryptionProviders(encryptionProvider, "auth", "Cryptographic.TripDES.Halo");
                ComboBox DefaultEncryptionProvider = wpfobject.GetUIItem<ITabPage, ComboBox>(servicetool.GetCurrentTabItem(), 0);
                DefaultEncryptionProvider.Enter(encryptionProvider);
                wpfobject.WaitTillLoad();
                servicetool.ClickApplyButtonFromTab();
                //servicetool.ClickApplyButtonFromTab();
                //servicetool.AcceptDialogWindow();
                wpfobject.WaitTillLoad();
                try
                {
                    wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                }
                catch (Exception e)
                {
                    Logger.Instance.InfoLog(e.Message);
                }
                //servicetool.RestartIISandWindowsServices();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                Logger.Instance.InfoLog("TripDES.Halo added as default encryption");
                ExecutedSteps++;

                //Step 3 - ..\webaccess\IntegratorAuthenticationSTS\web.config uncomment Bypass
                login.UncommentXMLnode("id", "Bypass");
                ExecutedSteps++;

                //Step 4 - From test server, run the Test EHR application 
                EHR ehr = new EHR();
                ehr.LaunchEHR();
                ExecutedSteps++;

                //Step 5 - In Image Load tab, enter the either Patient ID or Accession number of a study that matches one study that is in one of the data source domains. 
                //The Use Encryption box is checked. Ensure that Show Report is enabled, Show selector is not enabled, and click Load
                //Image Opens
                StudyViewer viewer = new StudyViewer();
                wpfobject.GetMainWindow("Test WebAccess EHR");
                wpfobject.SelectTabFromTabItems("Image Load");
                wpfobject.WaitTillLoad();
                //ehr.SetCommonParameters(user: adminUserName, domain: "SuperAdminGroup", AuthProvider: "Bypass", SecurityID: securityID);
                ehr.SetCommonParameters(AuthProvider: "Bypass");
                ehr.SetSelectorOptions(showSelector: "False", showReport: "True");
                ehr.SetSearchKeys_Study(accession);
                ehr.EncryptionCB().Checked = true;
                wpfobject.WaitTillLoad();
                ehr.Combobox_EncryptionProvider().Select(encryptionProvider);
                wpfobject.WaitTillLoad();
                String url_1 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login = new Login();
                StudyViewer studyviewer = login.NavigateToIntegratorURL(url_1);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame(0);
                //Check review tool bar is avilable
                BluRingViewer Viewer = new BluRingViewer();
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    Viewer.OpenViewerToolsPOPUp();
                    IList<String> totaltools = Viewer.GetToolsInToolBoxByGrid();
                    if (totaltools.Count > 0)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                else
                {

                    IList<IWebElement> reviewToolbarsList = studyviewer.AllReviewTools();
                    if (reviewToolbarsList.Count > 0)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                BasePage.Driver.Close();

                //Step 6 -  Close the browser.
                //At the Image Load tab, enter the either Patient ID or Accession number of a study that matches one study that is in one of the data source domains. 
                //Uncheck the Use Encryption box, and click Load.
                //Error msg
                ehr = new EHR();
                ehr.LaunchEHR();
                wpfobject.GetMainWindow("Test WebAccess EHR");
                wpfobject.SelectTabFromTabItems("Image Load");
                wpfobject.WaitTillLoad();
                //ehr.SetCommonParameters(user: adminUserName, domain: "SuperAdminGroup", AuthProvider: "Bypass", SecurityID: securityID);
                ehr.SetCommonParameters(AuthProvider: "Bypass");
                ehr.SetSelectorOptions(showSelector: "False", showReport: "True");
                ehr.SetSearchKeys_Study(accession);
                wpfobject.WaitTillLoad();
                ehr.EncryptionCB().Checked = false;
                wpfobject.WaitTillLoad();
                ehr.Combobox_EncryptionProvider().Select(encryptionProvider);
                wpfobject.WaitTillLoad();
                String url_2 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.InvokeBrowser(Config.BrowserType);
                login.NavigateToIntegratorURL(url_2);
                bool errorMsgFound = false;
                try
                {
                    IWebElement errorTitle = BasePage.Driver.FindElement(By.CssSelector("#m_title"));
                    if (errorTitle.GetAttribute("innerHTML").ToLower().Contains("url is not properly encrypted"))
                        errorMsgFound = true;
                }
                catch (Exception) { }
                if (errorMsgFound)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
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
                login.CommentXMLnode("id", "Bypass");
                new Taskbar().Show();
            }
        }
        /// <summary>
        /// OrthoCASE - Passing Domain information in the URL launch parameter - Universal viewer
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_166516(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables                      
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
               
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                string fiddlerOutput1 = @"c:\Temp\Fiddlerlog\log.txt";
                String fiddlerpath = "C:\\Users\\Administrator\\AppData\\Local\\Programs\\Fiddler\\Fiddler.exe";
                String ExecActionpath = "C:\\Users\\Administrator\\AppData\\Local\\Programs\\Fiddler\\ExecAction.exe ";
                String inaccessionno = null;
                // Preconditions
                // Verifying OrthoCase is already added or not in Service tool
                var servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                if (servicetool.IsApplicationListExists("OrthoCase"))
                {
                    // Verifying Domain parameter is already added in OrthoCase or not
                    wpfobject.SelectFromListView(0, "OrthoCase");
                    var wnd =  wpfobject.GetMainWindowByTitle("External Application URL Configuration");
                    wpfobject.WaitTillLoad();
                    var listview = wpfobject.GetAnyUIItem<TestStack.White.UIItems.WindowItems.Window, ListView>(wnd, "ListView");
                    bool flag_dominPresent = false;
                    var count = listview.Items.Count;
                    for (int i = 0; i < count;i++)
                    {
                        if (listview.Rows[i].Cells[0].Text.Equals("domain"))
                        {
                            flag_dominPresent = true;
                            Logger.Instance.InfoLog("Parameter domain is already exist in URL Parameter list");
                            break;
                        }
                    }
                    if(!flag_dominPresent)
                    {
                        servicetool.EnterExternalApplicationUrlParameterEntryForm("domain", "Dynamic", "DomainId", false);
                        wpfobject.GetMainWindowByTitle("External Application URL Configuration");
                        wpfobject.WaitTillLoad();
                        wpfobject.ClickButton("OK", 1);
                        wpfobject.WaitTillLoad();
                        wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                    
                        //Restarting IIS and Services 
                        servicetool.RestartService();
                    }
                    servicetool.CloseServiceTool();
                }
                else
                {
                    // Adding "OrthoCase" External Application in Service tool
                    wpfobject.ClickButton("Add", 1);
                    wpfobject.WaitTillLoad();
                    wpfobject.MoveWindowToDesktopTop("External Application URL Configuration");
                    servicetool.EnterExternalApplicationSettingsParameters(Config.OrthoPacsId, Config.OrthoPacsName, "localhost", Config.OrthoPacsPort, false, "", "Ajax", "", false, installationurl: "ExternalDependencies/OrthoCase/launch_page.htm</installurl>");

                    // Adding all the parameters in Url Parameter list
                    servicetool.EnterExternalApplicationUrlParameterEntryForm("PID", "Dynamic", "Patient.PatientID", false);
                    servicetool.EnterExternalApplicationUrlParameterEntryForm("accno", "Dynamic", "Study.AccessionNumber", false);
                    servicetool.EnterExternalApplicationUrlParameterEntryForm("studyId", "Dynamic", "Study.UID", false);
                    servicetool.EnterExternalApplicationUrlParameterEntryForm("patientName", "Dynamic", "Patient.PatienName", false);
                    servicetool.EnterExternalApplicationUrlParameterEntryForm("userID", "Dynamic", "User.Id", false);
                    servicetool.EnterExternalApplicationUrlParameterEntryForm("ServerHost", "Dynamic", "ServerHost", false);
                    servicetool.EnterExternalApplicationUrlParameterEntryForm("DataSourceIDs", "Dynamic", "Study.Datasource.ID", false);
                    servicetool.EnterExternalApplicationUrlParameterEntryForm("scheme", "Dynamic", "UriScheme", false);
                    servicetool.EnterExternalApplicationUrlParameterEntryForm("userFilter", "Static", "False", false);
                    servicetool.EnterExternalApplicationUrlParameterEntryForm("securityID", "Dynamic", "User.SecurityID", false);
                    servicetool.EnterExternalApplicationUrlParameterEntryForm("STSID", "Dynamic", "STS.ID", false);
                    servicetool.EnterExternalApplicationUrlParameterEntryForm("domain", "Dynamic", "DomainId", false);
                    wpfobject.GetMainWindowByTitle("External Application URL Configuration");
                    wpfobject.WaitTillLoad();
                    wpfobject.ClickButton("OK", 1);
                    wpfobject.WaitTillLoad();
                    wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                    
                    //Restarting IIS and Services 
                    servicetool.RestartService();
                    servicetool.CloseServiceTool();
                    new Taskbar().Show();
                }

                // Verifying "OrthoCase" is added or not as External Application in Role Management tab
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                var rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectDomainfromDropDown(Config.adminGroupName);
                PageLoadWait.WaitForFrameLoad(10);
                rolemanagement.SearchRole("SuperRole");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                rolemanagement.EditRoleByName("SuperRole");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                bool saveNeed = false;
                if (rolemanagement.List_ConnectExternalApplications().Count != 0)
                {
                    rolemanagement.ConnectExternalApplications();
                    saveNeed = true;
                }
                if (saveNeed)
                    rolemanagement.ClickSaveEditRole();
                else
                    rolemanagement.CloseRoleManagement();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                login.Logout();

                // Step  1 : Launch iCA application, login as any OrthoPACS domain user for which Universal viewer is set as default and search for any CT/MR modality study
                login.LoginIConnect(username, password);
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo:accession);
                
                studies.GetMatchingRow("Accession", accession).TryGetValue("Accession", out inaccessionno);
                if (inaccessionno.Equals(accession))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();
                Logger.Instance.InfoLog("Test Step Failed " + accession);

                // Step 2 : Launch fiddler tool
                BasePage.StartfiddlerAction(fiddlerpath, null);
                Thread.Sleep(25000);
                BasePage.StartfiddlerAction(ExecActionpath, "starthide");
                Thread.Sleep(5000);
                ExecutedSteps++;

                // Step 3 : Now select any study from the studylist in iCA and click on Launch button after selecting OrthoPACS as external application from the drop-down
                studies.SelectStudy("Accession", accession);
                var  externalAppDropDown = new SelectElement(studies.ExternalApp_Select());
                externalAppDropDown.SelectByText(Config.OrthoPacsName);
                studies.ClickExternalAppLaunch();
                // as the ortho case application is not yet implemented
                Thread.Sleep(5000);
                BasePage.StartfiddlerAction(ExecActionpath, "savesession");
                Thread.Sleep(10000);
                ExecutedSteps++;

                // Step 4 & 5 : Verify the launched URL in the URL list 
                //5. Copy the entry, paste in notepad to verify the contents in the URL
                string cmdOutput = System.IO.File.ReadAllText(fiddlerOutput1);
                if (cmdOutput.Contains("localhost:1234") && cmdOutput.Contains("domain=" + Config.adminGroupName))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                }

                if (File.Exists(fiddlerOutput1))                
                    File.Delete(fiddlerOutput1);       
                
                // Step 6 : launch the study in Universal viewer and click on OrthoCase option from the External Application options in study panel toolbar
                var viewer = BluRingViewer.LaunchBluRingViewer();
                BasePage.StartfiddlerAction(fiddlerpath, null);
                Thread.Sleep(25000);
                BasePage.StartfiddlerAction(ExecActionpath, "starthide");
                Thread.Sleep(5000);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".toolIconContainer.d1-toolbar")));
                viewer.ClickElement(viewer.ExternalApp_Orthouv());
                BasePage.Driver.FindElements(By.CssSelector(".toolIconControl .content.fontSmall_m.ng-star-inserted .fontSmall_m")).
                Single<IWebElement>(element => element.GetAttribute("innerHTML").Contains("Merge OrthoCase")).Click();
                // as the ortho case application is not yet implemented
                Thread.Sleep(2000);
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, ".dialogHeader.fontDefault_m .closeButton"));
                Thread.Sleep(2000);
                ExecutedSteps++;

                //Step 7 : Note down the launched URL
                BasePage.StartfiddlerAction(ExecActionpath, "savesession");
                Thread.Sleep(2000);
                cmdOutput = System.IO.File.ReadAllText(fiddlerOutput1);
                if (cmdOutput.Contains("localhost:1234") && cmdOutput.Contains("domain=" + Config.adminGroupName))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                }

                if (File.Exists(fiddlerOutput1))
                    File.Delete(fiddlerOutput1);
                viewer.CloseBluRingViewer();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                login.Logout();

                //Report Result
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
            
        }

        /// <summary>
        /// 1.0 External Application:  Configure
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27586(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables                      
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {

                //Fetch required Test data                
                String configDetails = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ConfigDetails");
                string[] arrConfigDetails = configDetails.Split(':');


                //HALO
                String haloIP = Config.HaloIp;
                String haloPort = Config.HaloPort;

                //VERICIS
                String vericisIP = Config.VericisIp;
                String vericisPort = Config.VericisPort;

                //Objects
                GroupBox group;
                ListView datagrid;
                ServiceTool servicetool;

                //Counter Vars
                int rowcount;

                //Step 1 - Preconditions. Test Data sent to data sources. (Set up done)              
                new Taskbar().Hide();
                ExecutedSteps++;

                //Step 2 - From Server run Merge iConnect Access Service Tool. Select"Encryption"tab
                servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                Thread.Sleep(5000);
                servicetool.NavigateToEncryption();
                ExecutedSteps++;


                //Step 3 - At the"Encryption Service"sub-tab, select Add button again. (To Add HALO)
                servicetool.SetEncryptionEncryptionService();
                servicetool.Add().Click();
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step 4 - Enter the followings: Key: HALO.RC4 Assembly: OpenContent.Generic.Core.dll Class: OpenContent.Core.Security.Services.HaloCryptographyWrapper
                wpfobject.GetMainWindowByTitle("Service Entry Form");
                wpfobject.MoveWindowToDesktopTop("Service Entry Form");
                servicetool.key_txt().Text = "HALO.RC4";
                servicetool.assembly_txt().Text = "OpenContent.Generic.Core.dll";
                servicetool.class_txt().Text = "OpenContent.Core.Security.Services.HaloCryptographyWrapper";
                wpfobject.WaitTillLoad();
                Logger.Instance.InfoLog("HALO Encryption service entry started");
                wpfobject.GetButton("Apply", 1).Click();
                ExecutedSteps++;

                //Step 5 - Continue update with the followings:Name: password Type: string Value: amicas Click OK button
                servicetool.EnterServiceParameters("password", "string", "amicas");
                //Check one entry is added
                wpfobject.GetMainWindowByTitle("Service Entry Form");
                group = WpfObjects._mainWindow.Get<GroupBox>(SearchCriteria.ByText("Service Parameters"));
                datagrid = wpfobject.GetAnyUIItem<GroupBox, ListView>(group, "ListView");
                rowcount = datagrid.Rows.Count;
                if (rowcount == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 6 - At Service Entry form, select OK button
                wpfobject.GetMainWindowByTitle("Service Entry Form");
                wpfobject.GetButton("OK", 1).Click();
                wpfobject.WaitTillLoad();
                Logger.Instance.InfoLog("HALO Encryption service entry completed");
                ExecutedSteps++;

                //Step 7 - External Application tab - AV-Lite: Select"External Application"tab.                       
                servicetool.NavigateToExternalApplication();
                ExecutedSteps++;

                //Step 8 - Add External application HALO. Select Add button
                wpfobject.ClickButton("Add", 1);
                wpfobject.WaitTillLoad();
                wpfobject.MoveWindowToDesktopTop("External Application URL Configuration");
                ExecutedSteps++;

                //Step 9 - Enter the followings for external application AMICAS HALO                
                servicetool.EnterExternalApplicationSettingsParameters(Config.HaloId, Config.HaloName, haloIP, haloPort, false, "servlet/com.amicas.servlet.integration.EmbeddedGateway", "Browser", "unixTicks,milliseconds,0", false);
                servicetool.EnterExternalApplicationEncryptionParameters("auth", "Cryptographic.HALO.RC4");
                servicetool.EnterExternalApplicationUrlParameterEntryForm("TS", "Dynamic", "Application.TimeStamp", true);
                servicetool.EnterExternalApplicationUrlParameterEntryForm("SID", "Dynamic", "Study.UID", true);
                servicetool.EnterExternalApplicationUrlParameterEntryForm("LOGIN", "Dynamic", "Application.User.Id", true);
                group = WpfObjects._mainWindow.Get<GroupBox>(SearchCriteria.ByText("Url Parameter List"));
                datagrid = wpfobject.GetAnyUIItem<GroupBox, ListView>(group, "ListView");
                if (datagrid.Rows.Count == 3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("External Application - HALO/MPACS configuration completed");
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                wpfobject.GetMainWindowByTitle("External Application URL Configuration");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("OK", 1);
                wpfobject.WaitTillLoad();

                //Step 10 - Add External Application VERICIS(CARDIO)
                servicetool.NavigateToExternalApplication();
                wpfobject.ClickButton("Add", 1);
                wpfobject.MoveWindowToDesktopTop("External Application URL Configuration");
                servicetool.EnterExternalApplicationSettingsParameters(Config.VericisId, Config.VericisName, vericisIP, vericisPort, false, "vericis_web/studyreviewdefault.asp", "Browser", "", false);
                servicetool.EnterExternalApplicationUrlParameterEntryForm("PID", "Dynamic", "Patient.PatientID", false);
                group = WpfObjects._mainWindow.Get<GroupBox>(SearchCriteria.ByText("Url Parameter List"));
                datagrid = wpfobject.GetAnyUIItem<GroupBox, ListView>(group, "ListView");
                if (datagrid.Rows.Count == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("External Application - VERICIS/CARDIO configuration completed");
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindowByTitle("External Application URL Configuration");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("OK", 1);
                wpfobject.WaitTillLoad();

                //Step 11 - In Service Tool click Restart IIS and Windows Services
                //servicetool.RestartIISandWindowsServices();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                ExecutedSteps++;
                servicetool.CloseServiceTool();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
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
                //Add all external applications to be visible in role management
                try
                {
                    login.DriverGoTo(login.url);
                    login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                    RoleManagement roleManagement = (RoleManagement)login.Navigate("RoleManagement");
                    roleManagement.SelectDomainfromDropDown(Config.adminGroupName);
                    PageLoadWait.WaitForFrameLoad(10);
                    roleManagement.SearchRole("SuperRole");
                    PageLoadWait.WaitForPageLoad(10);
                    PageLoadWait.WaitForFrameLoad(10);
                    roleManagement.EditRoleByName("SuperRole");
                    PageLoadWait.WaitForPageLoad(10);
                    PageLoadWait.WaitForFrameLoad(10);
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame(0);
                    bool saveNeeded = false;
                    if (roleManagement.List_ConnectExternalApplications().Count != 0)
                    {
                        roleManagement.ConnectExternalApplications();
                        saveNeeded = true;
                    }
                    if (saveNeeded)
                        roleManagement.ClickSaveEditRole();
                    else
                        roleManagement.CloseRoleManagement();
                    PageLoadWait.WaitForPageLoad(10);
                    PageLoadWait.WaitForFrameLoad(10);
                    Logger.Instance.InfoLog("External Applications are connected in Role Management");
                    login.Logout();
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Error while connecting External Applications in Role Management. E=" + e);
                }
                //Add all data sources related to external apps
                new Taskbar().Hide();
                try
                {
                    ServiceTool servicetool = new ServiceTool();
                    servicetool.LaunchServiceTool();
                    //Halo
                    if (!servicetool.IsDataSourceExists(GetHostName(Config.HaloIp)))
                        servicetool.AddPacsDatasource(Config.HaloIp, GetHostName(Config.HaloIp), "12", Config.HaloUser, Config.HaloPass);

                    //VericisEA
                    string vericisEA_ID = "VMSSA-" + Config.VericisEAIp.Split('.')[1] + "-" + Config.VericisEAIp.Split('.')[2] + "-" + Config.VericisEAIp.Split('.')[3];
                    string VericisEA_AE = "ECM_ARC_" + Config.VericisEAIp.Split('.')[3];
                    if (!servicetool.IsDataSourceExists(vericisEA_ID))
                        servicetool.AddEADatasource("10.5.38.138", VericisEA_AE , "22","12000");
                    //servicetool.RestartIISandWindowsServices();
                    wpfobject.WaitTillLoad();
                    wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                    servicetool.RestartService(); // Restart IIS service
                    servicetool.CloseServiceTool();
                    Logger.Instance.InfoLog("External Applications data sources are added in service tool");
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Error while Adding External apps data sources. E=" + e);
                }
                //Add Exception Site For JavaSecurity
                FileUtils.AddExceptionSiteForJavaSecurity("http://" + Config.HaloIp);
                FileUtils.AddExceptionSiteForJavaSecurity("http://" + Config.VericisEAIp);
                FileUtils.AddExceptionSiteForJavaSecurity("http://" + Config.VericisIp);
                Logger.Instance.InfoLog("Java Security Exceptions added");
                //Connect Datasources in Domain Management 
                try
                {
                    login.DriverGoTo(login.url);
                    login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                    DomainManagement domainManagement = (DomainManagement)login.Navigate("DomainManagement");
                    domainManagement.SearchDomain(Config.adminGroupName);
                    PageLoadWait.WaitForFrameLoad(20);
                    domainManagement.SelectDomain(Config.adminGroupName);
                    domainManagement.ClickEditDomain();
                    domainManagement.ConnectAllDataSources();
                    /*               
                    domainManagement.ConnectDataSource(GetHostName(Config.HaloIp));
                    string rsID = "VMSSA-" + Config.RadSuiteIp.Split('.')[1] + "-" + Config.RadSuiteIp.Split('.')[2] + "-" + Config.RadSuiteIp.Split('.')[3];
                    domainManagement.ConnectDataSource(rsID);
                    string vericisEA_ID = "VMSSA-" + Config.VericisEAIp.Split('.')[1] + "-" + Config.VericisEAIp.Split('.')[2] + "-" + Config.VericisEAIp.Split('.')[3];
                    domainManagement.ConnectDataSource(vericisEA_ID);
                    */
                    PageLoadWait.WaitForFrameLoad(20);
                    string receivingInst = domainManagement.ReceivingInstTxtBox().GetAttribute("value");
                    if (receivingInst == "")
                        domainManagement.ReceivingInstTxtBox().SendKeys("Institutuon");
                    PageLoadWait.WaitForFrameLoad(20);
                    domainManagement.ClickSaveEditDomain();
                    PageLoadWait.WaitForPageLoad(30);
                    PageLoadWait.WaitForFrameLoad(20);
                    Logger.Instance.InfoLog("External Applications data sources are connected in domain management");
                    login.Logout();
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Error while Connecting data sources in domain management. E=" + e);
                }
                new Taskbar().Show();
            }
        }

    }
}
