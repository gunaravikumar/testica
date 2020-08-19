using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.Putty;
using Selenium.Scripts.Pages.MergeServiceTool;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Reusable.Generic;
using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.ListBoxItems;


namespace Selenium.Scripts.Tests
{
    class PatientComparisonStrategy
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public ExamImporter ei { get; set; }
        public HPHomePage hphomepage { get; set; }
        public HPLogin hplogin { get; set; }
        public MpacLogin mpaclogin { get; set; }

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        public PatientComparisonStrategy(String classname)
        {
            login = new Login();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            ei = new ExamImporter();

            mpaclogin = new MpacLogin();
            hplogin = new HPLogin();
            //configure = new Configure();
            hphomepage = new HPHomePage();
        }


        /// <summary> 
        /// This Test Case is Verification of Viewer Layout Scope "Initial Setup and LastName+PID"
        /// </summary>
        public TestCaseResult Test_86303(String testid, String teststeps, int stepcount)
        {
            //-Initial Setup--
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            Configure configure = new Configure();
            HPHomePage hphome = new HPHomePage();
            WorkFlow workflow = new WorkFlow();
            DomainManagement domainmanagement;
            RoleManagement rolemanagement;
            UserManagement usermanagement;
            Inbounds inbounds;
            Outbounds outbounds;
            StudyViewer viewer;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            String UserName = Config.adminUserName;
            String Password = Config.adminPassword;

            String arUsername = Config.ar1UserName;
            String arPassword = Config.ar1Password;

            String phUsername = Config.ph1UserName;
            String phPassword = Config.ph1Password;

            String hpUserName = Config.hpUserName;
            String hpPassword = Config.hpPassword;

            String ST = "ST_PCS1_" + new Random().Next(1000);

            String datasource1 = login.GetHostName(Config.PACS2);//10.5.38.27(A6)
            String datasource2 = login.GetHostName(Config.SanityPACS);//10.5.38.28(A7)   
            String ipid1 = Config.ipid1;
            String ipid2 = Config.ipid2;

            String Dest1 = Config.Dest1;
            String Dest2 = Config.Dest2;

            String DefaultDomain = "SuperAdminGroup";
            String Staff = "StaffRole_PCS1_" + new Random().Next(1000);

            String PatientComparisonStrategyClassid_snapshot = "";
            String key = "InternalMergeDefaultMode";
            String value = "PCS_ONLY";
            try
            {

                //Step-1
                //HP -setup
                String newtag = "imageon.archive.core.database.jdbc.LastNameOnlyComparisonStrategy";

                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String[] LastNames = LastName.Split(':');
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String[] FirstNames = FirstName.Split(':');
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] PatientIds = PatientID.Split(':');
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String AccessionList1 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList_OneUniqueDiffParam");
                String[] Accessions1 = AccessionList1.Split(':');
                String Acc_DiffPID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Acc_DiffPID");
                String[] Acc_DiffPIDList = Acc_DiffPID.Split(':');
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String[] Filepaths = UploadFilePath.Split('=');

                String Acc_SamePD_Pname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Acc_SamePD_Pname");
                String[] Acc_SamePD_PnameList = Acc_SamePD_Pname.Split(':');


                String UploadFilePath_2 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath_2");
                String[] Filepaths_2 = UploadFilePath_2.Split('=');

                String Acc_DiffPID_LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Acc_DiffPID_LastName");
                String[] Acc_DiffPID_LastNameList = Acc_DiffPID_LastName.Split(':');

                String Acc_DiffPID_FullName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Acc_DiffPID_FullName");
                String[] Acc_DiffPID_FullNameList = Acc_DiffPID_FullName.Split(':');

                String EIPath = Config.EIFilePath;


                int changebrowser = 0;
                String browserName = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
                if (!(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer")))
                {
                    BasePage.Driver.Quit();
                    Config.BrowserType = "internet explorer";
                    Logger.Instance.InfoLog("Swicthing Browser Type to IE");
                    BasePage.Driver = null;
                    hplogin = new HPLogin();
                    BasePage.Driver.Navigate().GoToUrl(hplogin.hpurl);
                    HPHomePage homepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                    PageLoadWait.WaitForHPPageLoad(20);
                    configure = (Configure)homepage.Navigate("Configure");
                    configure.NavigateToTab("properties");
                    changebrowser++;
                }
                else
                {
                    BasePage.Driver.Navigate().GoToUrl(hplogin.hpurl);
                    HPHomePage homepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                    PageLoadWait.WaitForHPPageLoad(20);
                    configure = (Configure)homepage.Navigate("Configure");
                    configure.NavigateToTab("properties");
                }
                PageLoadWait.WaitForHPPageLoad(20);
                configure.NavigateToPropertySubTab("Database");
                PageLoadWait.WaitForHPPageLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("PropertiesContext");

                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#PatientComparisonStrategyClass_txt")));

                //Get Snapshot of query id tags before updating
                PatientComparisonStrategyClassid_snapshot = BasePage.Driver.FindElement(By.CssSelector("input[id='PatientComparisonStrategyClass_txt']")).GetAttribute("value");
                Logger.Instance.InfoLog("Patient Comparison Strategy Class Id snapshot before modification--" + PatientComparisonStrategyClassid_snapshot);

                BasePage.Driver.FindElement(By.CssSelector("input[id='PatientComparisonStrategyClass_txt']")).Clear();
                Logger.Instance.InfoLog("Patient Comparison Strategy Class updated to-- " + newtag);
                BasePage.Driver.FindElement(By.CssSelector("input[id='PatientComparisonStrategyClass_txt']")).SendKeys(newtag);

                //add a new Property to Database|InternalMergeDefaultMode;;PCS_ONLY

                configure.AddProperty(key, value);

                //Save the transaction
                configure.ClickSubmitChangesBtn();
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("img#PatientComparisonStrategyClass_statusimg")));
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("img#InternalMergeDefaultMode_statusimg")));
                //logout
                BasePage.Driver.SwitchTo().DefaultContent();
                hplogin.LogoutHPen();

                //Revert back to the same driver type or logout
                if (changebrowser == 1)
                {
                    BasePage.Driver.Quit();
                    Config.BrowserType = browserName;
                    Logger.Instance.InfoLog("Swicthing Back Browser to --" + browserName);
                    BasePage.Driver = null;
                    new HPLogin();
                }

                Putty putty = new Putty();
                putty.RestartService();
                ExecutedSteps++;

                //Step-2:Initial Setups
                //Create a new imagesharing-domain

                login.DriverGoTo(login.url);
                login.LoginIConnect(UserName, Password);

                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("patientid", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientfullname", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientlastname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientdob", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientipid", 0);
                domainmanagement.ClickSaveDomain();


                //Create roles and users as specified and set Quersy search parameters               
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.CreateRole(DefaultDomain, Staff, "both");

                bool strole = rolemanagement.RoleExists(Staff);
                bool phrole = rolemanagement.RoleExists("Physician");
                bool arrole = rolemanagement.RoleExists("Archivist");

                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(ST, DefaultDomain, Staff);

                bool newst = usermanagement.SearchUser(ST, DefaultDomain);
                bool ph = usermanagement.SearchUser(Config.ph1UserName, DefaultDomain);
                bool ar = usermanagement.SearchUser(Config.ar1UserName, DefaultDomain);
                //Navigate to Image Sharing-->Institution tab

                Image_Sharing imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                Image_Sharing.Destination dest = (Image_Sharing.Destination)imagesharing.NavigateToSubTab("Destination");
                dest.EditDestination(DefaultDomain, Dest1, ST, ST);

                if (strole && phrole && arrole && newst && ph && ar)
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
                login.Logout();

                //Step-3
                //Created INST1 and INST2
                ExecutedSteps++;

                //Step-4
                //From ICA service tool\Image Sharing tab\Image Sharing Options sub-tab, 
                //set the Patient Comparison Strategy to IPID+PatientId+LastName
                //Restart IIS and Windows Services.
                //Open WebAccessConfiguration.xml under C;-\WebAccess\WebAccess\Config folder, 
                //verify the changes is saved in the file.

                ServiceTool st = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();
                Taskbar bar = new Taskbar();
                bar.Hide();
                st.LaunchServiceTool();
                st.NavigateToTab("Image Sharing");
                wpfobject.WaitTillLoad();
                st.NavigateSubTab("Image Sharing Options");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);
                ComboBox comboBox = WpfObjects._mainWindow.Get<ComboBox>(SearchCriteria.ByClassName("ComboBox"));
                comboBox.Select("IPID+PatientId+LastName");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("6");
                wpfobject.WaitTillLoad();
                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();
                bar.Show();

                //This configuration is saved in "WebAccessConfgiruation" file, `<PatientComparisonStrategy>
                //<IPID>True</IPID> 
                //<PatientId>True</PatientId>
                //<LastName>True</LastName>
                //<PatientName>False</PatientName>
                //</PatientComparisonStrategy>

                String WebAccessConfigPath = @"C:\WebAccess\WebAccess\Config\WebAccessConfiguration.xml";
                String NodePath = "Configuration/ImageSharing/PatientComparisonStrategy/";

                if (st.GetNodeValue(WebAccessConfigPath, NodePath + "IPID") == "True" &&
                    st.GetNodeValue(WebAccessConfigPath, NodePath + "PatientId") == "True" &&
                    st.GetNodeValue(WebAccessConfigPath, NodePath + "LastName") == "True" &&
                    st.GetNodeValue(WebAccessConfigPath, NodePath + "PatientName") == "False")
                {
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

                ei.EIDicomUpload(phUsername, phPassword, Dest1, Filepaths[0], 1, EIPath);

                //Login as physician1 
                login.DriverGoTo(login.url);
                login.LoginIConnect(phUsername, phPassword);

                //Navigate to inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search Study
                PageLoadWait.WaitforStudyInStatus(AccessionID, inbounds, "Uploaded");
                inbounds.SearchStudy(AccessionNo: AccessionID);

                //Validate study is present in physician's inbounds
                Dictionary<string, string> row5 = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status" }, new string[] { AccessionID, PatientID, "Uploaded" });
                if (row5 != null)//&& (row5["Status"].Equals("Uploaded") || row5["Status"].Equals("Uploading")) )
                {
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


                //Step-6:Uploading (prior)-study to holding pen as staff user             
                ei.EIDicomUpload(ST, ST, Dest1, Filepaths[1], 1, EIPath);
                ei.EIDicomUpload(ST, ST, Dest1, Filepaths[2], 1, EIPath);
                ei.EIDicomUpload(ST, ST, Dest1, Filepaths[3], 1, EIPath);
                ei.EIDicomUpload(ST, ST, Dest1, Filepaths[4], 1, EIPath);

                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                //Navigate to inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                //Search Study
                PageLoadWait.WaitforStudyInStatus(Accessions1[0], inbounds, "Uploaded");
                PageLoadWait.WaitforStudyInStatus(Accessions1[1], inbounds, "Uploaded");
                PageLoadWait.WaitforStudyInStatus(Accessions1[2], inbounds, "Uploaded");
                PageLoadWait.WaitforStudyInStatus(Accessions1[3], inbounds, "Uploaded");
                PageLoadWait.WaitForFrameLoad(5);
                inbounds.ClearButton().Click();
                inbounds.SearchStudy(patientID: PatientID);

                //Validate study is present in physician's inbounds
                Dictionary<string, string> diff_Firstnameonly = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status" }, new string[] { Accessions1[0], PatientID, "Uploaded" });
                Dictionary<string, string> diff_Midnameonly = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status" }, new string[] { Accessions1[1], PatientID, "Uploaded" });
                Dictionary<string, string> diff_Prefixonly = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status" }, new string[] { Accessions1[2], PatientID, "Uploaded" });
                Dictionary<string, string> diff_Suffixonly = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status" }, new string[] { Accessions1[3], PatientID, "Uploaded" });
                if (diff_Firstnameonly != null && diff_Midnameonly != null && diff_Prefixonly != null && diff_Suffixonly != null)
                {
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

                //Step-7:Login iCA as receiver (ph), go to Inbounds page, search all patients, 
                //verify patient name, patient ID and IPID of uploaded studies.
                login.DriverGoTo(login.url);
                login.LoginIConnect(phUsername, phPassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(patientID: PatientID);
                inbounds.ChooseColumns(new string[] { "Issuer of PID" });

                Dictionary<int, string[]> results = BasePage.GetSearchResults();
                string[] columnnames = BasePage.GetColumnNames();
                string[] PatientName = BasePage.GetColumnValues(results, "Patient Name", columnnames);
                string[] PID = BasePage.GetColumnValues(results, "Patient ID", columnnames);
                string[] IPID = BasePage.GetColumnValues(results, "Issuer of PID", columnnames);
                String[] ACC = BasePage.GetColumnValues(results, "Accession", columnnames);
                String UploadedPatientname = LastNames[0] + ", " + FirstNames[0];
                //When iCA and Holding pen is set to use LastName+PID+IPID PCS,
                //for studies that have the same patient Lastname, Patient ID and IPID as the base patient study,
                //patient names of these studies are merged to the base patient.
                String[] uploadedAccession = { AccessionID, Accessions1[0], Accessions1[1], Accessions1[2], Accessions1[3] };

                if (uploadedAccession.All(item => ACC.Contains(item)) && Array.Exists(PatientName, s => s.Equals(UploadedPatientname, StringComparison.CurrentCultureIgnoreCase)) && Array.Exists(PID, s => s.Equals(PatientID)) && Array.Exists(IPID, s => s.Equals(ipid1)))
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

                //Step-8:Load one of study from this patient. Open History Panel
                inbounds.SelectStudy("Accession", AccessionID);
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results6 = BasePage.GetSearchResults();
                string[] columnnames8 = BasePage.GetColumnNames();
                string[] PID_8 = BasePage.GetColumnValues(results6, "Patient ID", columnnames8);
                string[] ACC_8 = BasePage.GetColumnValues(results6, "Accession", columnnames8);

                //All uploaded studies are listed in the History Panel.              

                if (Array.Exists(PID_8, s => s.Equals(PatientIds[0])) && uploadedAccession.All(item => ACC_8.Contains(item)))
                {
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


                //Step-9:From EI1 (IPID1) upload a study that has the same patient name and patient ID as the base patient but has different LastName, verify the patient name in Inbounds page.
                ei.EIDicomUpload(ST, ST, Dest1, Filepaths[5], 1, EIPath);

                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                //Navigate to inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                //Search Study
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitforStudyInStatus(Accessions1[4], inbounds, "Uploaded");
                inbounds.SearchStudy(AccessionNo: Accessions1[4]);

                //Validate study is present in physician's inbounds 
                inbounds.ChooseColumns(new string[] { "Issuer of PID" });
                Dictionary<string, string> row9 = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status", "Issuer of PID" }, new string[] { Accessions1[4], PatientID, "Uploaded", ipid1 });
                if (row9 != null)
                {
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


                //Step-10:Repeat above step, from EI1 (IPID1) upload a study that has different Patient ID from the base patient study.
                ei.EIDicomUpload(ST, ST, Dest1, Filepaths[6], 1, EIPath);
                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                //Navigate to inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                //Search Study
                PageLoadWait.WaitforStudyInStatus(Acc_DiffPIDList[0], inbounds, "Uploaded");
                inbounds.SearchStudy(AccessionNo: Acc_DiffPIDList[0]);

                //Validate study is present in physician's inbounds 
                inbounds.ChooseColumns(new string[] { "Issuer of PID" });
                Dictionary<string, string> row10 = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status", "Issuer of PID" }, new string[] { Acc_DiffPIDList[0], Acc_DiffPIDList[1], "Uploaded", ipid1 });
                if (row10 != null)
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
                login.Logout();

                //Step-11:Repeat above step, upload a study via POP1 (IPID-IPID2), that has the same patient name, patient ID as the base study.
                BasePage.RunBatchFile(Config.batchfilepath, Filepaths_2[0] + " " + Config.dicomsendpath + " " + Config.StudyPacs);

                //Send the study to dicom devices from MergePacs management page 
                //login to PACS#1 as admin
                mpaclogin.DriverGoTo(mpaclogin.mpacstudyurl);
                MPHomePage mpachome = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);

                //Send Study with matching HL7 order
                Tool tool = (Tool)mpachome.NavigateTopMenu("Tools");
                tool.NavigateToSendStudy();
                tool.SearchStudy("Accession", Acc_SamePD_PnameList[0], 0);

                //Get study details
                Dictionary<string, string> MpacDetails = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());

                tool.MpacSelectStudy("Accession", Acc_SamePD_PnameList[0]);
                tool.SendStudy(1, Config.pacsgatway2);
                mpaclogin.LogoutPacs();

                //Login as receiver
                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                PageLoadWait.WaitforStudyInStatus(Acc_SamePD_PnameList[0], inbounds, "Uploaded");
                PageLoadWait.WaitforUpload(Acc_SamePD_PnameList[0], inbounds);

                //Search and Select Study
                inbounds.SearchStudy(AccessionNo: Acc_SamePD_PnameList[0]);

                //Valiadate study is present in Physician's inbounds
                inbounds.ChooseColumns(new string[] { "Issuer of PID" });
                Dictionary<string, string> row11 = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Issuer of PID" }, new string[] { Acc_SamePD_PnameList[0], PatientID, ipid2 });

                if (row11 != null && row11["Status"].Equals("Uploading") || row11["Status"].Equals("Uploaded"))
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
                login.Logout();


                //Step-12:Login EA holding pen as webadmin, verify uploaded patients at patient and study levels.             

                //Login in Holding Pen and Navigate to archive search menu 

                login.DriverGoTo(login.hpurl);
                hplogin = new HPLogin();
                hphomepage = hplogin.LoginHPen(hpUserName, hpPassword);
                workflow = (WorkFlow)hphomepage.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                //Search study               
                workflow.HPSearchStudy("Firstname", FirstNames[0]);
                Dictionary<int, string[]> searchresults = workflow.GetResultsInHP();
                workflow.Clearform();

                workflow.HPSearchStudy("Lastname", LastNames[0]);
                workflow.HPSearchStudy("PatientID", PatientID);
                workflow.HPSearchStudy("Issuer", ipid1);
                Dictionary<string, string> StudyDetails12_1 = workflow.GetStudyDetailsInHP();

                workflow.Clearform();
                workflow.HPSearchStudy("Lastname", LastNames[1]);
                Dictionary<string, string> StudyDetails12_2 = workflow.GetStudyDetailsInHP();
                workflow.Clearform();

                workflow.HPSearchStudy("PatientID", Acc_DiffPIDList[1]);
                Dictionary<string, string> StudyDetails12_3 = workflow.GetStudyDetailsInHP();
                workflow.Clearform();

                workflow.HPSearchStudy("PatientID", PatientID);
                workflow.HPSearchStudy("Issuer", ipid2);
                Dictionary<string, string> StudyDetails12_4 = workflow.GetStudyDetailsInHP();

                //Validate study is present in Holding pen 
                if (searchresults.Count == 4 && StudyDetails12_1["Number of Studies"].Equals("5") && StudyDetails12_2["Number of Studies"].Equals("1") && StudyDetails12_3["Number of Studies"].Equals("1") && StudyDetails12_4["Number of Studies"].Equals("1"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout Holding pen
                hplogin.LogoutHPen();

                //Step-13


                //Login iCA as a user who have access to studies in Inbounds and Outbounds, go to Inbounds page, 
                //search studies in Last Name and Patient ID fields (lastname and 90000103)
                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(LastName: LastNames[0], patientID: PatientID);
                inbounds.ChooseColumns(new string[] { "Last Name" });
                //Only studies with matching the search criteria are listed.
                Dictionary<int, string[]> results13 = BasePage.GetSearchResults();
                string[] columnnames13 = BasePage.GetColumnNames();
                string[] PatientLastName13 = BasePage.GetColumnValues(results13, "Last Name", columnnames13);
                bool flag13 = true;

                foreach (String str in PatientLastName13)
                {
                    if (!str.Equals(LastNames[0], StringComparison.CurrentCultureIgnoreCase))
                    {
                        flag13 = false;
                        break;
                    }
                }

                if (flag13)
                {
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
                //Search the study that has a different last name in Last Name field, e.g. di
                PageLoadWait.WaitForFrameLoad(5);
                inbounds.ClearButton().Click();
                inbounds.SearchStudy(LastName: "di");
                //Only study with matching the search criteria is listed.
                Dictionary<int, string[]> results14 = BasePage.GetSearchResults();
                string[] columnnames14 = BasePage.GetColumnNames();
                string[] PatientLastName14 = BasePage.GetColumnValues(results14, "Last Name", columnnames14);
                bool flag14 = true;

                foreach (String s in PatientLastName14)
                {
                    if (!s.ToLower().Contains("di"))
                    {
                        flag14 = false;
                        break;
                    }
                }

                if (flag14 && PatientLastName14.Length >= 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-15
                //Search the study that has a patient ID in Patient ID field, e.g. pidd
                PageLoadWait.WaitForFrameLoad(5);
                inbounds.ClearButton().Click();
                inbounds.SearchStudy(patientID: "pidd");
                //Only study with matching the search criteria is listed.
                Dictionary<int, string[]> results15 = BasePage.GetSearchResults();
                string[] columnnames15 = BasePage.GetColumnNames();
                string[] Patientid15 = BasePage.GetColumnValues(results15, "Patient ID", columnnames15);
                bool flag15 = true;

                foreach (String s in Patientid15)
                {
                    if (!s.ToLower().Contains("pidd"))
                    {
                        flag15 = false;
                        break;
                    }
                }

                if (flag15 && Patientid15.Length == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-16
                //From Inbounds page delete a study (e.g. a study had a different prefix)
                PageLoadWait.WaitForFrameLoad(5);
                inbounds.ClearButton().Click();
                inbounds.SearchStudy(patientID: PatientID);
                inbounds.SelectStudy("Accession", Accessions1[2]);
                inbounds.DeleteStudy();

                //The selected study is deleted from Inbounds page successfully.
                if (inbounds.CheckStudy("Accession", Accessions1[2]) != true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-17
                //From Outbounds page delete a study (e.g. a study has a different suffix)
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy(patientID: PatientID);
                outbounds.SelectStudy("Accession", Accessions1[3]);
                outbounds.DeleteStudy();

                //The selected study is deleted from Outbounds page successfully.                              
                if (outbounds.CheckStudy("Accession", Accessions1[3]) != true)
                {
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

                //Step-18:Go to EA Holding Pen webadmin, open patient list, verify that deleted 
                //studies are no-longer listed under the patient.
                login.DriverGoTo(login.hpurl);
                hplogin = new HPLogin();
                hphome = hplogin.LoginHPen(hpUserName, hpPassword);

                workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                workflow.HPSearchStudy("Accessionno", Accessions1[2]);
                bool diff_Prefix = workflow.HPCheckStudy(Accessions1[2]);
                workflow.HPSearchStudy("Accessionno", Accessions1[3]);
                bool diff_Suffix = workflow.HPCheckStudy(Accessions1[3]);
                //((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"input[name='accessionNumber']\").value=''");
                //PageLoadWait.WaitForHPPageLoad(20);

                if (diff_Prefix != true && diff_Suffix != true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                hplogin.LogoutHPen();

                //Step-19:Reroute a study to a different data source.
                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(AccessionNo: Accessions1[1]);
                inbounds.SelectStudy("Accession", Accessions1[1]);
                inbounds.RerouteStudy(Config.Dest2);
                inbounds.SearchStudy(AccessionNo: Accessions1[1]);

                if (inbounds.CheckStudy("Accession", Accessions1[1]) != true)
                {
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

                //Step-20:Login iCA as a receiver (ph), from Inbounds page select and nominate the base study for archive.
                //Login iCA as an archivist (ar), from Inbounds page selected and archive the nominated study without modification. 
                //(before archive ensure these test data do not exist in destination to be used.)

                login.DriverGoTo(login.url);
                login.LoginIConnect(phUsername, phPassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(AccessionNo: AccessionID);
                inbounds.SelectStudy("Accession", AccessionID);
                inbounds.NominateForArchive("reason");
                login.Logout();

                mpaclogin.DriverGoTo(mpaclogin.mpacdesturl);
                MPHomePage mpachome1 = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);

                //Send Study with matching HL7 order
                Tool tool1 = (Tool)mpachome1.NavigateTopMenu("Tools");
                tool1.NavigateToSendStudy();
                tool1.SearchStudy("Accession", AccessionID, 0);

                //Get study details
                Dictionary<string, string> studyresults = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());
                //Logout from Dest PACS
                mpaclogin.LogoutPacs();

                login.DriverGoTo(login.url);
                login.LoginIConnect(arUsername, arPassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(AccessionNo: AccessionID);
                inbounds.SelectStudy("Accession", AccessionID);
                inbounds.ArchiveStudy("", "");
                inbounds.SearchStudy(AccessionNo: AccessionID);
                String studyStatus;
                inbounds.GetMatchingRow("Accession", AccessionID).TryGetValue("Status", out studyStatus);
                //The study is archived successfully, Study shows Routing Completed.
                if (studyresults == null && inbounds.CheckStudy("Accession", AccessionID) == true && studyStatus == "Routing Completed")
                {
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
                //Step-21:Login ICA as staff (st), from Inbounds page select the study that has a different patient ID, 
                //Archive Study, change patient ID to be the same as the base study, e.g. from pidDiffonly90000103 to 90000103, 
                //and update the Study Description to a new value.
                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(patientID: Acc_DiffPIDList[1]);
                inbounds.SelectStudy("Accession", Acc_DiffPIDList[0]);
                inbounds.ClickArchiveStudy("", "");
                inbounds.ArchiveSearch("order", "All Dates");
                inbounds.EditFinalDetailsInArchive("pid", PatientID);
                inbounds.EditFinalDetailsInArchive("description", "New_Des");
                inbounds.ClickArchive();

                //The study is archived successfully, Study shows Routing Completed. 
                //Patient ID and Study Description are updated using the newly modified values.
                PageLoadWait.WaitForFrameLoad(5);
                inbounds.ClearButton().Click();
                inbounds.SearchStudy(AccessionNo: Acc_DiffPIDList[0]);
                Dictionary<string, string> study21 = inbounds.GetMatchingRow(new string[] { "Accession", "Status", "Patient ID", "Description" }, new string[] { Acc_DiffPIDList[0], "Routing Completed", PatientID, "New_Des" });
                //The study is archived successfully, Study shows Routing Completed.
                if (inbounds.CheckStudy("Accession", Acc_DiffPIDList[0]) == true && study21 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-22:Archive the study that has a different IPID (IPID2), change the IPID value to be the same as in the base study (IPID1),
                //update Study Description to a new value.

                //inbounds.SearchStudy(patientID: "--Base page IPID --- ");
                PageLoadWait.WaitForFrameLoad(5);
                inbounds.ClearButton().Click();
                inbounds.SearchStudy(AccessionNo: Acc_SamePD_PnameList[0]);
                inbounds.SelectStudy("Accession", Acc_SamePD_PnameList[0]);
                inbounds.ClickArchiveStudy("", "");
                inbounds.ArchiveSearch("order", "All Dates");
                inbounds.EditFinalDetailsInArchive("ipid", ipid1);
                inbounds.EditFinalDetailsInArchive("description", "New_Des1");
                inbounds.ClickArchive();

                //The study is archived successfully, Study shows Routing Completed. 
                //IPID is IPID1 and Study Description is updated using the newly modified values.

                inbounds.ChooseColumns(new string[] { "Issuer of PID" });
                PageLoadWait.WaitForFrameLoad(5);
                inbounds.ClearButton().Click();
                PageLoadWait.WaitForFrameLoad(30);
                inbounds.SearchStudy(AccessionNo: Acc_SamePD_PnameList[0]);
                Dictionary<string, string> study22 = inbounds.GetMatchingRow(new string[] { "Accession", "Status", "Issuer of PID", "Description" }, new string[] { Acc_SamePD_PnameList[0], "Routing Completed", ipid1, "New_Des1" });
                //The study is archived successfully, Study shows Routing Completed.
                if (inbounds.CheckStudy("Accession", Acc_SamePD_PnameList[0]) == true && study22 != null)
                {
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
                //Step-23:From EA Holding Pen webadmin, verify the patients and studies.
                login.DriverGoTo(login.hpurl);
                hplogin = new HPLogin();
                hphome = hplogin.LoginHPen(hpUserName, hpPassword);
                workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                workflow.HPSearchStudy("PatientID", PatientID);
                Dictionary<int, string[]> searchresults23 = workflow.GetResultsInHP();

                workflow.Clearform();
                workflow.HPSearchStudy("Lastname", LastNames[0]);
                workflow.HPSearchStudy("PatientID", PatientID);
                workflow.HPSearchStudy("Issuer", ipid1);
                Dictionary<string, string> StudyDetails23_1 = workflow.GetStudyDetailsInHP();

                workflow.Clearform();
                workflow.HPSearchStudy("Lastname", LastNames[1]);
                Dictionary<string, string> StudyDetails23_2 = workflow.GetStudyDetailsInHP();

                //((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"input[name='accessionNumber']\").value=''");
                //PageLoadWait.WaitForHPPageLoad(20);

                //These two archived/reconciled studies are merged into the base patient name; 
                //the patient has a different patient last name remains unchanged.

                //** Have to verify merged and two last name..

                if (searchresults23.Count == 2 && StudyDetails23_1["Number of Studies"].Equals("5") && StudyDetails23_2["Number of Studies"].Equals("1"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                hplogin.LogoutHPen();


                //Step-24:Archive the study that has a different last name, change Last Name to match the base study's last name
                // and update Study Description to a new value
                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(patientID: PatientID);
                inbounds.SelectStudy("Accession", Accessions1[4]);
                inbounds.ClickArchiveStudy("", "");
                inbounds.ArchiveSearch("order", "All Dates");
                inbounds.EditFinalDetailsInArchive("last name", LastNames[0]);
                inbounds.EditFinalDetailsInArchive("description", "New_Des2");
                inbounds.ClickArchive();

                //The study is archived successfully, Study shows Routing Completed. 
                //Patient Last Name and Study Description are updated using the newly modified values.
                inbounds.ChooseColumns(new string[] { "Last Name" });
                PageLoadWait.WaitForFrameLoad(5);
                inbounds.ClearButton().Click();
                PageLoadWait.WaitForFrameLoad(30);
                inbounds.SearchStudy(AccessionNo: Accessions1[4]);
                inbounds.SelectStudy("Accession", Accessions1[4]);
                Dictionary<string, string> study24 = inbounds.GetMatchingRow(new string[] { "Accession", "Status", "Last Name", "Description" }, new string[] { Accessions1[4], "Routing Completed", LastNames[0].ToUpper(), "New_Des2" });
                //The study is archived successfully, Study shows Routing Completed.

                if (inbounds.CheckStudy("Accession", Accessions1[4]) == true && study24 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-25:Load one of study and open History Panel.                
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results25 = BasePage.GetSearchResults();
                String[] uploadedAccession25 = { AccessionID, Accessions1[0], Accessions1[1], Accessions1[4], Acc_DiffPIDList[0], Acc_SamePD_PnameList[0] };
                string[] columnnames25 = BasePage.GetColumnNames();
                string[] PID25 = BasePage.GetColumnValues(results25, "Patient ID", columnnames25);
                string[] ACC25 = BasePage.GetColumnValues(results25, "Accession", columnnames25);

                //All uploaded studies are listed in the History Panel.          
                if (Array.Exists(PID25, s => s.Equals(PatientIds[0])) && uploadedAccession25.All(item => ACC25.Contains(item)))
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
                viewer.CloseStudy();

                //Step-26:Archive the study that has a different first name, 
                //change First Name to a new value e.g. FNnew  
                PageLoadWait.WaitForFrameLoad(5);
                inbounds.ClearButton().Click();
                inbounds.SearchStudy(patientID: PatientID);
                inbounds.SelectStudy("Accession", Accessions1[0]);
                inbounds.ClickArchiveStudy("", "");
                inbounds.ArchiveSearch("order", "All Dates");
                inbounds.EditFinalDetailsInArchive("first name", "FNnew");
                inbounds.ClickArchive();

                //The study is archived successfully, Study shows Routing Completed. 
                //Patient names of all uploaded studies are updated using the newly modified value in first name, 
                //the Study Description is updated only for this study just archived.
                inbounds.ChooseColumns(new string[] { "First Name" });
                PageLoadWait.WaitForFrameLoad(30);
                inbounds.SearchStudy(patientID: PatientID);
                Dictionary<int, string[]> results26 = BasePage.GetSearchResults();

                string[] columnnames26 = BasePage.GetColumnNames();
                string[] FN26 = BasePage.GetColumnValues(results26, "First Name", columnnames26);

                Dictionary<string, string> study26 = inbounds.GetMatchingRow(new string[] { "Accession", "Status", "First Name" }, new string[] { Accessions1[0], "Routing Completed", "FNNEW" });

                //The study is archived successfully, Study shows Routing Completed.
                if (Array.Exists(FN26, s => s.Equals("FNNEW")) && study26 != null)
                {
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


                //Step-27:From EA Holding Pen webadmin, verify the patients and studies.
                login.DriverGoTo(login.hpurl);
                hplogin = new HPLogin();
                hphome = hplogin.LoginHPen(hpUserName, hpPassword);
                workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                workflow.HPSearchStudy("PatientID", PatientID);

                Dictionary<string, string> StudyDetails27_1 = workflow.GetStudyDetailsInHP();

                //These two archived/reconciled studies are merged into the base patient name; 
                //the patient has a different patient last name remains unchanged.

                //** Have to verify merged and two last name..

                if (StudyDetails27_1["Number of Studies"].Equals("6") && StudyDetails27_1["Patient Name"].Contains("FNNEW"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();

                }
                hplogin.LogoutHPen();

                //Step-28:From EI1 upload a study that only has the different Patient ID and Last Name from the base study.
                ei.EIDicomUpload(ST, ST, Dest1, Filepaths[7], 1, EIPath);

                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                //Navigate to inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                //Search Study
                PageLoadWait.WaitforStudyInStatus(Acc_DiffPID_LastNameList[0], inbounds, "Uploaded");
                inbounds.SearchStudy(AccessionNo: Acc_DiffPID_LastNameList[0]);
                inbounds.SearchStudy(patientID: Acc_DiffPID_LastNameList[1]);

                //Validate study is present in physician's inbounds                
                Dictionary<string, string> row28 = inbounds.GetMatchingRow(new string[] { "Accession", "Patient Name", "Patient ID", "Status" }, new string[] { Acc_DiffPID_LastNameList[0], Acc_DiffPID_LastNameList[2], Acc_DiffPID_LastNameList[1], "Uploaded" });
                if (row28 != null)
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
                login.Logout();

                //Step-29:From EA holding pen WebAdmin page, verify the study is not merged.
                //Login in Holding Pen and Navigate to archive search menu 
                login.DriverGoTo(login.hpurl);
                hplogin = new HPLogin();
                hphomepage = hplogin.LoginHPen(hpUserName, hpPassword);
                workflow = (WorkFlow)hphomepage.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                //Search study               
                workflow.HPSearchStudy("PatientID", PatientID);
                Dictionary<string, string> StudyDetails29_1 = workflow.GetStudyDetailsInHP();

                workflow.Clearform();
                workflow.HPSearchStudy("PatientID", Acc_DiffPID_LastNameList[1]);
                Dictionary<string, string> StudyDetails29_2 = workflow.GetStudyDetailsInHP();

                //Validate study is present in Holding pen 
                if (StudyDetails29_1["Number of Studies"].Equals("6") && StudyDetails29_1["Patient Name"].Contains("FNNEW")
                     && StudyDetails29_2["Number of Studies"].Equals("1") && StudyDetails29_2["Patient Name"].Contains("FIRSTNAME_SD3"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();

                }
                //Logout Holding pen
                hplogin.LogoutHPen();

                //Step-30:Archive the study by modifying all values to be the same as the base study except patient Last Name to use a new value, e.g. LastName-LNnew
                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(patientID: Acc_DiffPID_LastNameList[1]);
                inbounds.SelectStudy("Accession", Acc_DiffPID_LastNameList[0]);
                inbounds.ClickArchiveStudy("", "");
                inbounds.ArchiveSearch("order", "All Dates");
                inbounds.EditFinalDetailsInArchive("pid", PatientID);
                inbounds.EditFinalDetailsInArchive("last name", "LNNEW");
                inbounds.ClickArchive();

                //The study is archived successfully, Study shows Routing Completed. 
                //Patient Last Name and Study Description are updated using the newly modified values.
                inbounds.ChooseColumns(new string[] { "Last Name" });
                PageLoadWait.WaitForFrameLoad(5);
                inbounds.ClearButton().Click();
                PageLoadWait.WaitForFrameLoad(30);
                inbounds.SearchStudy(LastName: "LNNEW");
                inbounds.SearchStudy(AccessionNo: Acc_DiffPID_LastNameList[0]);

                Dictionary<string, string> study30 = inbounds.GetMatchingRow(new string[] { "Accession", "Status", "Last Name", "Patient ID" }, new string[] { Acc_DiffPID_LastNameList[0], "Routing Completed", "LNNEW", PatientID });
                //The study is archived successfully, Study shows Routing Completed.
                if (inbounds.CheckStudy("Accession", Acc_DiffPID_LastNameList[0]) == true && study30 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-31:Load the study and open History Panel.
                inbounds.SelectStudy("Last Name", "LNNEW");
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                Dictionary<int, string[]> results31 = BasePage.GetSearchResults();
                string[] columnnames31 = BasePage.GetColumnNames();
                string[] PID31 = BasePage.GetColumnValues(results31, "Patient ID", columnnames31);

                //There is only one study listed under this patient.
                if (PID31.Length == 1 && PID31[0].Equals(PatientID))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseHistoryPanel();
                viewer.CloseStudy();
                login.Logout();


                //Step-32:Upload any study that does not have the same patient name and patient ID.
                ei.EIDicomUpload(ST, ST, Dest1, Filepaths[8], 1, EIPath);
                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                //Navigate to inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                //Search Study
                PageLoadWait.WaitforStudyInStatus(Acc_DiffPID_FullNameList[0], inbounds, "Uploaded");
                inbounds.SearchStudy(AccessionNo: Acc_DiffPID_FullNameList[0]);

                //Validate study is present in physician's inbounds                
                Dictionary<string, string> row32 = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status" }, new string[] { Acc_DiffPID_FullNameList[0], Acc_DiffPID_FullNameList[1], "Uploaded" });
                if (row32 != null)
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

                //Step-33:Archive patient to match all fields (Last Name, First Name, Middle Name, Prefix, Suffix, Gender, DOB, IPID and PID) as the patient with new last name (e.g. LNnew)

                inbounds.SelectStudy("Accession", Acc_DiffPID_FullNameList[0]);
                inbounds.ClickArchiveStudy("", "");
                inbounds.ArchiveSearch("patient", "All Dates");
                inbounds.EditFinalDetailsInArchive("pid", PatientID);
                inbounds.EditFinalDetailsInArchive("description", "New_Desc_5");
                inbounds.EditFinalDetailsInArchive("last name", "LNnew");
                inbounds.ClickArchive();

                //The study is archived successfully, Study shows Routing Completed. 
                //Patient Last Name and Study Description are updated using the newly modified values.
                inbounds.ChooseColumns(new string[] { "Last Name" });
                PageLoadWait.WaitForFrameLoad(5);
                inbounds.ClearButton().Click();
                PageLoadWait.WaitForFrameLoad(30);
                inbounds.SearchStudy(AccessionNo: Acc_DiffPID_FullNameList[0]);
                inbounds.SelectStudy("Accession", Acc_DiffPID_FullNameList[0]);
                Dictionary<string, string> study33 = inbounds.GetMatchingRow(new string[] { "Accession", "Status", "Last Name", "Patient ID", "Description" }, new string[] { Acc_DiffPID_FullNameList[0], "Routing Completed", "LNNEW", PatientID, "New_Desc_5" });

                //The study is archived successfully, Study shows Routing Completed.
                if (inbounds.CheckStudy("Accession", Acc_DiffPID_FullNameList[0]) == true && study33 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-34:Load the study and open History Panel. (e.g. Last Name - LNnew)
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                Dictionary<int, string[]> results34 = BasePage.GetSearchResults();
                string[] columnnames34 = BasePage.GetColumnNames();
                string[] PID34 = BasePage.GetColumnValues(results34, "Patient ID", columnnames34);

                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<string, string> study34 = viewer.GetMatchingRow("Accession", AccessionID);

                if (PID34.Length == 2 && study34 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-35:Load the 2nd study into the 2nd viewer.

                viewer.OpenPriors(new string[] { "Accession" }, new string[] { AccessionID });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo35 = viewer.StudyInfo(2);
                String PatientInfo35 = viewer.PatientInfoTab(2);
                if (viewer.studyPanel(2).Displayed && (PatientInfo35.Split(',')[0].ToUpper()).Equals(study33["Last Name"].ToUpper()) && (PatientInfo35.Split(',')[2]).Equals(study33["Patient ID"]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                viewer.CloseStudy();
                login.Logout();
                //----------------------------------------

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
                    ServiceTool st = new ServiceTool();
                    WpfObjects wpfobject = new WpfObjects();
                    Taskbar bar = new Taskbar();
                    bar.Hide();
                    st.LaunchServiceTool();
                    st.NavigateToTab("Image Sharing");
                    wpfobject.WaitTillLoad();
                    st.NavigateSubTab("Image Sharing Options");
                    wpfobject.WaitTillLoad();
                    wpfobject.ClickButton("Modify", 1);
                    ComboBox comboBox = WpfObjects._mainWindow.Get<ComboBox>(SearchCriteria.ByClassName("ComboBox"));
                    comboBox.Select("IPID+PatientId");
                    wpfobject.WaitTillLoad();
                    wpfobject.ClickButton("Apply", 1);
                    wpfobject.WaitTillLoad();
                    wpfobject.ClickButton("6");
                    wpfobject.WaitTillLoad();
                    st.RestartService();
                    wpfobject.WaitTillLoad();
                    st.CloseServiceTool();
                    wpfobject.WaitTillLoad();
                    bar.Show();

                    String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");

                    //Delete All studies
                    login.DriverGoTo(login.url);
                    login.LoginIConnect(Config.ph1UserName, Config.ph1Password);
                    inbounds = (Inbounds)login.Navigate("Inbounds");
                    inbounds.SearchStudy("LastName", "*");
                    inbounds.DeleteAllStudies();
                    login.Logout();

                    try
                    {
                        login.LoginIConnect(ST, ST);
                        outbounds = (Outbounds)login.Navigate("Outbounds");
                        outbounds.SearchStudy("PatientID", PatientID);
                        outbounds.DeleteAllStudies();
                        login.Logout();
                    }
                    catch (Exception) { }
                    //Cleanup Destination PACS
                    SocketClient.Send(Config.DestinationPACS, 7777, "db2cmd -c C:\\SQLLIB\\BIN\\PACSCleanup.bat");
                    SocketClient.Close();

                    //Delete Studies from EA(Holding Pen)
                    Putty putty1 = new Putty();
                    putty1.EA_Cleanup();

                    int changebrowser = 0;

                    String browserName = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
                    if (!(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer")))
                    {
                        BasePage.Driver.Quit();
                        Config.BrowserType = "internet explorer";
                        Logger.Instance.InfoLog("Swicthing Browser Type to IE");
                        BasePage.Driver = null;
                        hplogin = new HPLogin();
                        BasePage.Driver.Navigate().GoToUrl(hplogin.hpurl);
                        HPHomePage homepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                        PageLoadWait.WaitForHPPageLoad(20);
                        configure = (Configure)homepage.Navigate("Configure");
                        configure.NavigateToTab("properties");
                        changebrowser++;
                    }
                    else
                    {
                        hplogin = new HPLogin();
                        BasePage.Driver.Navigate().GoToUrl(hplogin.hpurl);
                        HPHomePage homepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                        PageLoadWait.WaitForHPPageLoad(20);
                        configure = (Configure)homepage.Navigate("Configure");
                        configure.NavigateToTab("properties");
                    }
                    PageLoadWait.WaitForHPPageLoad(20);
                    configure.NavigateToPropertySubTab("Database");
                    PageLoadWait.WaitForHPPageLoad(20);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("PropertiesContext");

                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#PatientComparisonStrategyClass_txt")));

                    //Update Snapshot Patient Comparison Strategy Class
                    String PCS_snapshot = BasePage.Driver.FindElement(By.CssSelector("input[id='PatientComparisonStrategyClass_txt']")).GetAttribute("value");
                    if (PCS_snapshot != PatientComparisonStrategyClassid_snapshot)
                    {
                        BasePage.Driver.FindElement(By.CssSelector("input[id='PatientComparisonStrategyClass_txt']")).Clear();
                        Logger.Instance.InfoLog("Patient Comparison Strategy Class updated in finally block " + PatientComparisonStrategyClassid_snapshot);
                        BasePage.Driver.FindElement(By.CssSelector("input[id='PatientComparisonStrategyClass_txt']")).SendKeys(PatientComparisonStrategyClassid_snapshot);
                        configure.ClickSubmitChangesBtn();
                        BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("img#PatientComparisonStrategyClass_statusimg")));
                    }
                    //delete Property Database|InternalMergeDefaultMode;;PCS_ONLY
                    //BasePage.Driver.FindElement(By.CssSelector("#InternalMergeDefaultMode_img")).Click();

                    //Save the transaction


                    //logout
                    BasePage.Driver.SwitchTo().DefaultContent();
                    hplogin.LogoutHPen();

                    //Revert back to the same driver type or logout
                    if (changebrowser == 1)
                    {
                        BasePage.Driver.Quit();
                        Config.BrowserType = browserName;
                        Logger.Instance.InfoLog("Swicthing Back Browser to --" + browserName);
                        BasePage.Driver = null;
                        new HPLogin();
                    }

                    Putty putty = new Putty();
                    putty.RestartService();
                }
                catch (Exception)
                {

                    int changebrowser = 0;

                    String browserName = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
                    if (!(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer")))
                    {
                        BasePage.Driver.Quit();
                        Config.BrowserType = "internet explorer";
                        Logger.Instance.InfoLog("Swicthing Browser Type to IE");
                        BasePage.Driver = null;
                        hplogin = new HPLogin();
                        BasePage.Driver.Navigate().GoToUrl(hplogin.hpurl);
                        HPHomePage homepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                        PageLoadWait.WaitForHPPageLoad(20);
                        configure = (Configure)homepage.Navigate("Configure");
                        configure.NavigateToTab("properties");
                        changebrowser++;
                    }
                    else
                    {
                        hplogin = new HPLogin();
                        BasePage.Driver.Navigate().GoToUrl(hplogin.hpurl);
                        HPHomePage homepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                        PageLoadWait.WaitForHPPageLoad(20);
                        configure = (Configure)homepage.Navigate("Configure");
                        configure.NavigateToTab("properties");
                    }
                    PageLoadWait.WaitForHPPageLoad(20);
                    configure.NavigateToPropertySubTab("Database");
                    PageLoadWait.WaitForHPPageLoad(20);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("PropertiesContext");

                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#PatientComparisonStrategyClass_txt")));

                    //Update Snapshot Patient Comparison Strategy Class
                    String PCS_snapshot = BasePage.Driver.FindElement(By.CssSelector("input[id='PatientComparisonStrategyClass_txt']")).GetAttribute("value");
                    if (PCS_snapshot != PatientComparisonStrategyClassid_snapshot)
                    {
                        BasePage.Driver.FindElement(By.CssSelector("input[id='PatientComparisonStrategyClass_txt']")).Clear();
                        Logger.Instance.InfoLog("Patient Comparison Strategy Class updated in finally block " + PatientComparisonStrategyClassid_snapshot);
                        BasePage.Driver.FindElement(By.CssSelector("input[id='PatientComparisonStrategyClass_txt']")).SendKeys(PatientComparisonStrategyClassid_snapshot);
                        configure.ClickSubmitChangesBtn();
                        BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("img#PatientComparisonStrategyClass_statusimg")));
                    }
                    //delete Property Database|InternalMergeDefaultMode;;PCS_ONLY
                    //BasePage.Driver.FindElement(By.CssSelector("#InternalMergeDefaultMode_img")).Click();

                    //Save the transaction


                    //logout
                    BasePage.Driver.SwitchTo().DefaultContent();
                    hplogin.LogoutHPen();

                    //Revert back to the same driver type or logout
                    if (changebrowser == 1)
                    {
                        BasePage.Driver.Quit();
                        Config.BrowserType = browserName;
                        Logger.Instance.InfoLog("Swicthing Back Browser to --" + browserName);
                        BasePage.Driver = null;
                        new HPLogin();
                    }

                    Putty putty = new Putty();
                    putty.RestartService();
                }
            }
        }


        /// <summary> 
        /// This Test Case is Verification of Viewer Layout Scope "FirstName + PID"
        /// </summary>
        public TestCaseResult Test_86304(String testid, String teststeps, int stepcount)
        {
            //-Initial Setup--
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            Configure configure = new Configure();
            HPHomePage hphome = new HPHomePage();
            WorkFlow workflow = new WorkFlow();
            DomainManagement domainmanagement;
            RoleManagement rolemanagement;
            UserManagement usermanagement;
            Inbounds inbounds;
            Outbounds outbounds;
            StudyViewer viewer;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            String UserName = Config.adminUserName;
            String Password = Config.adminPassword;

            String arUsername = Config.ar1UserName;
            String arPassword = Config.ar1Password;

            String phUsername = Config.ph1UserName;
            String phPassword = Config.ph1Password;

            String hpUserName = Config.hpUserName;
            String hpPassword = Config.hpPassword;

            String ST = "ST_PCS2_" + new Random().Next(1000);

            String datasource1 = login.GetHostName(Config.PACS2);//10.5.38.27(A6)
            String datasource2 = login.GetHostName(Config.SanityPACS);//10.5.38.28(A7)   
            String ipid1 = Config.ipid1;
            String ipid2 = Config.ipid2;

            String Dest1 = Config.Dest1;
            String Dest2 = Config.Dest2;

            String DefaultDomain = "SuperAdminGroup";
            String Staff = "StaffRole_PCS2_" + new Random().Next(1000);

            String PatientComparisonStrategyClassid_snapshot = "";
            String key = "InternalMergeDefaultMode";
            String value = "PCS_ONLY";

            try
            {
                //Step-1
                //HP -setup
                String newtag = "imageon.archive.core.database.jdbc.StrictComparisonStrategy";

                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String[] LastNames = LastName.Split(':');
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String[] FirstNames = FirstName.Split(':');
                String MiddleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "MiddleName");
                String[] MiddleNames = MiddleName.Split(':');

                String Prefix = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Prefix");
                String[] Prefixs = Prefix.Split(':');

                String Suffix = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Suffix");
                String[] Suffixs = Suffix.Split(':');

                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");

                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String[] PatientIds = PatientIDList.Split(':');

                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String AccessionList1 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList_OneUniqueDiffParam");
                String[] Accessions1 = AccessionList1.Split(':');
                String Acc_DiffPID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Acc_DiffPID");
                String[] Acc_DiffPIDList = Acc_DiffPID.Split(':');
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String[] Filepaths = UploadFilePath.Split('=');

                String Acc_SamePD_Pname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Acc_SamePD_Pname");
                String[] Acc_SamePD_PnameList = Acc_SamePD_Pname.Split(':');


                String UploadFilePath_2 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath_2");
                String[] Filepaths_2 = UploadFilePath_2.Split('=');

                String Acc_DiffPID_LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Acc_DiffPID_LastName");
                String[] Acc_DiffPID_LastNameList = Acc_DiffPID_LastName.Split(':');

                String Acc_DiffPID_FullName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Acc_DiffPID_FullName");
                String[] Acc_DiffPID_FullNameList = Acc_DiffPID_FullName.Split(':');

                String EIPath = Config.EIFilePath;


                int changebrowser = 0;
                String browserName = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
                if (!(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer")))
                {
                    BasePage.Driver.Quit();
                    Config.BrowserType = "internet explorer";
                    Logger.Instance.InfoLog("Swicthing Browser Type to IE");
                    BasePage.Driver = null;
                    hplogin = new HPLogin();
                    BasePage.Driver.Navigate().GoToUrl(hplogin.hpurl);
                    HPHomePage homepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                    PageLoadWait.WaitForHPPageLoad(20);
                    configure = (Configure)homepage.Navigate("Configure");
                    configure.NavigateToTab("properties");
                    changebrowser++;
                }
                else
                {
                    BasePage.Driver.Navigate().GoToUrl(hplogin.hpurl);
                    HPHomePage homepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                    PageLoadWait.WaitForHPPageLoad(20);
                    configure = (Configure)homepage.Navigate("Configure");
                    configure.NavigateToTab("properties");
                }
                PageLoadWait.WaitForHPPageLoad(20);
                configure.NavigateToPropertySubTab("Database");
                PageLoadWait.WaitForHPPageLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("PropertiesContext");

                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#PatientComparisonStrategyClass_txt")));

                //Get Snapshot of query id tags before updating
                PatientComparisonStrategyClassid_snapshot = BasePage.Driver.FindElement(By.CssSelector("input[id='PatientComparisonStrategyClass_txt']")).GetAttribute("value");
                Logger.Instance.InfoLog("Patient Comparison Strategy Class Id snapshot before modification--" + PatientComparisonStrategyClassid_snapshot);

                if (!PatientComparisonStrategyClassid_snapshot.Equals(newtag))
                {
                    BasePage.Driver.FindElement(By.CssSelector("input[id='PatientComparisonStrategyClass_txt']")).Clear();
                    Logger.Instance.InfoLog("Patient Comparison Strategy Class updated to-- " + newtag);
                    BasePage.Driver.FindElement(By.CssSelector("input[id='PatientComparisonStrategyClass_txt']")).SendKeys(newtag);

                    configure.ClickSubmitChangesBtn();
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("img#PatientComparisonStrategyClass_statusimg")));
                }
                //add a new Property to Database|InternalMergeDefaultMode;;PCS_ONLY


                try
                {
                    IWebElement IMDM = BasePage.Driver.FindElement(By.CssSelector("input#InternalMergeDefaultMode_txt"));
                    if (IMDM.Displayed == true)
                    {
                        if (IMDM.GetAttribute("value").Equals(value))
                        {
                            Logger.Instance.InfoLog("Property already exist ");
                        }
                        else
                        {
                            IMDM.SendKeys(value);
                            configure.ClickSubmitChangesBtn();
                            BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("img#InternalMergeDefaultMode_statusimg")));
                        }
                    }
                    else
                    {
                        configure.AddProperty(key, value);
                        BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("img#InternalMergeDefaultMode_statusimg")));
                    }


                }
                catch (Exception e)
                {
                    configure.AddProperty(key, value);
                    configure.ClickSubmitChangesBtn();
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("img#InternalMergeDefaultMode_statusimg")));
                }

                //logout
                BasePage.Driver.SwitchTo().DefaultContent();
                hplogin.LogoutHPen();

                //Revert back to the same driver type or logout
                if (changebrowser == 1)
                {
                    BasePage.Driver.Quit();
                    Config.BrowserType = browserName;
                    Logger.Instance.InfoLog("Swicthing Back Browser to --" + browserName);
                    BasePage.Driver = null;
                    new HPLogin();
                }

                Putty putty = new Putty();
                putty.RestartService();
                ExecutedSteps++;

                //Step-2:Initial Setups
                //Create a new imagesharing-domain

                login.DriverGoTo(login.url);
                login.LoginIConnect(UserName, Password);

                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("patientid", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientfullname", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientlastname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientdob", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientipid", 0);
                domainmanagement.ClickSaveDomain();


                //Create roles and users as specified and set Quersy search parameters               
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.CreateRole(DefaultDomain, Staff, "both");

                bool strole = rolemanagement.RoleExists(Staff);
                bool phrole = rolemanagement.RoleExists("Physician");
                bool arrole = rolemanagement.RoleExists("Archivist");

                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(ST, DefaultDomain, Staff);

                bool newst = usermanagement.SearchUser(ST, DefaultDomain);
                bool ph = usermanagement.SearchUser(Config.ph1UserName, DefaultDomain);
                bool ar = usermanagement.SearchUser(Config.ar1UserName, DefaultDomain);
                //Navigate to Image Sharing-->Institution tab

                Image_Sharing imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                Image_Sharing.Destination dest = (Image_Sharing.Destination)imagesharing.NavigateToSubTab("Destination");
                dest.EditDestination(DefaultDomain, Dest1, ST, ST);

                if (strole && phrole && arrole && newst && ph && ar)
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
                login.Logout();

                //Step-3
                //Created INST1 and INST2
                ExecutedSteps++;

                //Step-4
                //From ICA service tool\Image Sharing tab\Image Sharing Options sub-tab, 
                //set the Patient Comparison Strategy to  IPID + PatientId + FullName
                //Restart IIS and Windows Services.
                //Open WebAccessConfiguration.xml under C;-\WebAccess\WebAccess\Config folder, 
                //verify the changes is saved in the file.

                ServiceTool st = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();
                Taskbar bar = new Taskbar();
                bar.Hide();
                st.LaunchServiceTool();
                st.NavigateToTab("Image Sharing");
                wpfobject.WaitTillLoad();
                st.NavigateSubTab("Image Sharing Options");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);
                ComboBox comboBox = WpfObjects._mainWindow.Get<ComboBox>(SearchCriteria.ByClassName("ComboBox"));
                comboBox.Select("IPID+PatientId+FullName");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("6");
                wpfobject.WaitTillLoad();
                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();
                bar.Show();

                //This configuration is saved in "WebAccessConfgiruation" file, `<PatientComparisonStrategy>

                //<IPID> True </IPID>
                //<PatientId> True </PatientId>
                //<LastName> False </LastName>
                //<PatientName> True </PatientName>
                //</PatientComparisonStrategy>'

                String WebAccessConfigPath = @"C:\WebAccess\WebAccess\Config\WebAccessConfiguration.xml";
                String NodePath = "Configuration/ImageSharing/PatientComparisonStrategy/";

                if (st.GetNodeValue(WebAccessConfigPath, NodePath + "IPID") == "True" &&
                    st.GetNodeValue(WebAccessConfigPath, NodePath + "PatientId") == "True" &&
                    st.GetNodeValue(WebAccessConfigPath, NodePath + "LastName") == "False" &&
                    st.GetNodeValue(WebAccessConfigPath, NodePath + "PatientName") == "True")
                {
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
                //Upload base study--
                ei.EIDicomUpload(phUsername, phPassword, Dest1, Filepaths[0], 1, EIPath);

                //Login as physician1 
                login.DriverGoTo(login.url);
                login.LoginIConnect(phUsername, phPassword);

                //Navigate to inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search Study
                PageLoadWait.WaitforStudyInStatus(AccessionID, inbounds, "Uploaded");
                //PageLoadWait.WaitforUpload(AccessionID,inbounds);
                inbounds.SearchStudy(AccessionNo: AccessionID);

                //Validate study is present in physician's inbounds
                Dictionary<string, string> row5 = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status" }, new string[] { AccessionID, PatientID, "Uploaded" });
                if (row5 != null)
                {
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


                //Step-6:Uploading (prior)-study to holding pen as staff user        
                //1. a study has different FirstName only
                //2.a study has different middle name only
                //3.a study has different Prefix only
                //4.a study has different Suffix only
                //5.a study has different Patient ID only
                //6.a study has a different LastName only

                ei.EIDicomUpload(ST, ST, Dest1, Filepaths[1], 1, EIPath);
                ei.EIDicomUpload(ST, ST, Dest1, Filepaths[2], 1, EIPath);
                ei.EIDicomUpload(ST, ST, Dest1, Filepaths[3], 1, EIPath);
                ei.EIDicomUpload(ST, ST, Dest1, Filepaths[4], 1, EIPath);
                ei.EIDicomUpload(ST, ST, Dest1, Filepaths[5], 1, EIPath);
                ei.EIDicomUpload(ST, ST, Dest1, Filepaths[6], 1, EIPath);

                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                //Navigate to inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                //Search Study
                PageLoadWait.WaitforStudyInStatus(Accessions1[0], inbounds, "Uploaded");
                PageLoadWait.WaitforStudyInStatus(Accessions1[1], inbounds, "Uploaded");
                PageLoadWait.WaitforStudyInStatus(Accessions1[2], inbounds, "Uploaded");
                PageLoadWait.WaitforStudyInStatus(Accessions1[3], inbounds, "Uploaded");
                PageLoadWait.WaitforStudyInStatus(Accessions1[4], inbounds, "Uploaded");
                PageLoadWait.WaitforStudyInStatus(Acc_DiffPIDList[0], inbounds, "Uploaded");
                PageLoadWait.WaitForFrameLoad(5);
                inbounds.ClearButton().Click();
                inbounds.SearchStudy(patientID: PatientID);

                //Validate study is present in physician's inbounds
                Dictionary<string, string> diff_Firstnameonly = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status" }, new string[] { Accessions1[0], PatientID, "Uploaded" });
                Dictionary<string, string> diff_Midnameonly = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status" }, new string[] { Accessions1[1], PatientID, "Uploaded" });
                Dictionary<string, string> diff_Prefixonly = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status" }, new string[] { Accessions1[2], PatientID, "Uploaded" });
                Dictionary<string, string> diff_Suffixonly = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status" }, new string[] { Accessions1[3], PatientID, "Uploaded" });

                Dictionary<string, string> diff_LastNameonly = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status" }, new string[] { Accessions1[4], PatientID, "Uploaded" });
                PageLoadWait.WaitForFrameLoad(5);
                inbounds.ClearButton().Click();
                inbounds.SearchStudy(AccessionNo: Acc_DiffPIDList[0]);
                Dictionary<string, string> diff_Patientonly = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status" }, new string[] { Acc_DiffPIDList[0], Acc_DiffPIDList[1], "Uploaded" });
                if (diff_Firstnameonly != null && diff_Midnameonly != null && diff_Prefixonly != null
                    && diff_Suffixonly != null && diff_Patientonly != null && diff_LastNameonly != null)
                {
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

                //Step-7:Login iCA as receiver (ph), go to Inbounds page, search all patients, 
                //verify patient name, patient ID and IPID of uploaded studies.

                login.DriverGoTo(login.url);
                login.LoginIConnect(phUsername, phPassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(patientID: PatientID);
                inbounds.ChooseColumns(new string[] { "Issuer of PID" });

                String PN_Patientname = LastNames[0].ToUpper() + ", " + FirstNames[0].ToUpper();
                String PN_Diff_LastName = LastNames[1].ToUpper() + ", " + FirstNames[0].ToUpper();
                String PN_Diff_Firstname = LastNames[0].ToUpper() + ", " + FirstNames[1].ToUpper();


                //Validate study is present in physician's inbounds
                Dictionary<string, string> detail_basestudy_7 = inbounds.GetMatchingRow(new string[] { "Issuer of PID", "Accession", "Status", "Patient ID", "Patient Name" }, new string[] { ipid1, AccessionID, "Uploaded", PatientIds[0], PN_Patientname });
                Dictionary<string, string> dif_Firstnameoly_7 = inbounds.GetMatchingRow(new string[] { "Issuer of PID", "Accession", "Status", "Patient ID", "Patient Name" }, new string[] { ipid1, Accessions1[0], "Uploaded", PatientIds[0], PN_Diff_Firstname });
                Dictionary<string, string> diff_Midnameonly_7 = inbounds.GetMatchingRow(new string[] { "Issuer of PID", "Accession", "Status", "Patient ID", "Patient Name" }, new string[] { ipid1, Accessions1[1], "Uploaded", PatientIds[0], PN_Patientname });
                Dictionary<string, string> diff_Prefixonly__7 = inbounds.GetMatchingRow(new string[] { "Issuer of PID", "Accession", "Status", "Patient ID", "Patient Name" }, new string[] { ipid1, Accessions1[2], "Uploaded", PatientIds[0], PN_Patientname });
                Dictionary<string, string> diff_Suffixonly__7 = inbounds.GetMatchingRow(new string[] { "Issuer of PID", "Accession", "Status", "Patient ID", "Patient Name" }, new string[] { ipid1, Accessions1[3], "Uploaded", PatientIds[0], PN_Patientname });

                Dictionary<string, string> dif_LastNameonly_7 = inbounds.GetMatchingRow(new string[] { "Issuer of PID", "Accession", "Status", "Patient ID", "Patient Name" }, new string[] { ipid1, Accessions1[4], "Uploaded", PatientIds[0], PN_Diff_LastName });

                PageLoadWait.WaitForFrameLoad(5);
                inbounds.ClearButton().Click();
                inbounds.SearchStudy(AccessionNo: Acc_DiffPIDList[0]);
                Dictionary<string, string> diff_Patientonly_7 = inbounds.GetMatchingRow(new string[] { "Issuer of PID", "Accession", "Status", "Patient ID", "Patient Name" }, new string[] { ipid1, Acc_DiffPIDList[0], "Uploaded", Acc_DiffPIDList[1], PN_Patientname });

                //When iCA and Holding pen is set to use FullName+PID+IPID PCS, no patient name, patient ID and
                //IPID updates occur to these uploaded studies.



                if (detail_basestudy_7 != null && dif_Firstnameoly_7 != null && diff_Midnameonly_7 != null &&
                    diff_Prefixonly__7 != null && diff_Suffixonly__7 != null &&
                    diff_Patientonly_7 != null && dif_LastNameonly_7 != null)
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
                login.Logout();

                //Step-8
                //Upload a study via POP1 (IPID;;IPID2) with a study that has the same patient name, patient ID as the base study.

                BasePage.RunBatchFile(Config.batchfilepath, Filepaths_2[0] + " " + Config.dicomsendpath + " " + Config.StudyPacs);

                //Send the study to dicom devices from MergePacs management page 
                //login to PACS#1 as admin
                mpaclogin.DriverGoTo(mpaclogin.mpacstudyurl);
                MPHomePage mpachome = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);

                //Send Study with matching HL7 order
                Tool tool = (Tool)mpachome.NavigateTopMenu("Tools");
                tool.NavigateToSendStudy();
                tool.SearchStudy("Accession", Acc_SamePD_PnameList[0], 0);

                //Get study details
                Dictionary<string, string> MpacDetails = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());

                tool.MpacSelectStudy("Accession", Acc_SamePD_PnameList[0]);
                tool.SendStudy(1, Config.pacsgatway2);
                mpaclogin.LogoutPacs();

                //Login as receiver
                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                PageLoadWait.WaitforStudyInStatus(Acc_SamePD_PnameList[0], inbounds, "Uploaded");
                PageLoadWait.WaitforUpload(Acc_SamePD_PnameList[0], inbounds);

                //Search and Select Study
                inbounds.SearchStudy(AccessionNo: Acc_SamePD_PnameList[0]);

                //Valiadate study is present in Physician's inbounds
                inbounds.ChooseColumns(new string[] { "Issuer of PID" });
                Dictionary<string, string> study_8 = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Issuer of PID", "Status" }, new string[] { Acc_SamePD_PnameList[0], PatientID, ipid2, "Uploaded" });

                if (study_8 != null)//&& study_8["Status"].Equals("Uploading") || study_8["Status"].Equals("Uploaded"))
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
                login.Logout();


                //Step-9:
                //Login EA holding pen as webadmin, verify uploaded patients at patient and study levels.             

                //Login in Holding Pen and Navigate to archive search menu 

                login.DriverGoTo(login.hpurl);
                hplogin = new HPLogin();
                hphomepage = hplogin.LoginHPen(hpUserName, hpPassword);
                workflow = (WorkFlow)hphomepage.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                //Search study               
                workflow.HPSearchStudy("Firstname", "%SD3%");
                Dictionary<int, string[]> searchresults = workflow.GetResultsInHP();

                bool flag_9 = true;
                foreach (string[] value1 in searchresults.Values)
                {
                    if (!value1[value1.Length - 1].Equals("1"))
                    {
                        flag_9 = false;
                        break;
                    }
                }
                String[] uploadedAccession_9 = { AccessionID, Accessions1[0], Accessions1[1], Accessions1[2], Accessions1[3], Accessions1[4], Acc_SamePD_PnameList[0] };
                foreach (String s in uploadedAccession_9)
                {
                    workflow.Clearform();
                    workflow.HPSearchStudy("Accessionno", s);
                    if (!workflow.HPCheckStudy(s))
                    {
                        flag_9 = false;
                        break;
                    }
                }

                //Validate study is present in Holding pen 
                if (searchresults.Count == 8 && flag_9)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout Holding pen
                hplogin.LogoutHPen();


                //Step-10
                //Login iCA as a user who have access to studies in Inbounds and Outbounds, go to Inbounds page, 
                //search studies in Last Name field e.g. lastname, and Patient ID field e.g. 90000103

                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(LastName: LastNames[0], patientID: PatientID);                
                inbounds.ChooseColumns(new string[] { "Last Name" });

                //Inbounds page only lists studies with patient last name and patient ID matching the searching criteria,
                //e.g. studies with different last name and different Patient ID are not listed.

                Dictionary<int, string[]> results10 = BasePage.GetSearchResults();
                string[] columnnames10 = BasePage.GetColumnNames();
                string[] PatientLastName10 = BasePage.GetColumnValues(results10, "Last Name", columnnames10);
                string[] PatientID10 = BasePage.GetColumnValues(results10, "Patient ID", columnnames10);


                if (results10.Count == 6 && PatientLastName10.All(i => i.Equals(LastNames[0].ToUpper())) && PatientID10.All(i => i.Equals(PatientID)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-11
                //From Inbounds page search study that has a different last name in Last Name field, e.g. search for 'di'
                PageLoadWait.WaitForFrameLoad(5);
                inbounds.ClearButton().Click();
                inbounds.SearchStudy(LastName: "di");
                //Only study with matching the search criteria is listed.

                Dictionary<int, string[]> results11 = BasePage.GetSearchResults();
                string[] columnnames11 = BasePage.GetColumnNames();
                string[] PatientLastName11 = BasePage.GetColumnValues(results11, "Last Name", columnnames11);
                bool flag11 = true;

                foreach (String s in PatientLastName11)
                {
                    if (!s.ToLower().StartsWith("di"))
                    {
                        flag11 = false;
                        break;
                    }
                }

                if (results11.Count == 1 && flag11 && PatientLastName11.Length == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-12
                //From Inbounds page search study that has a different first name in First Name field, e.g.search for 'di'
                PageLoadWait.WaitForFrameLoad(5);
                inbounds.ClearButton().Click();
                inbounds.SearchStudy(FirstName: "di");
                inbounds.ChooseColumns(new string[] { "First Name" });
                //Only study with matching the search criteria is listed.

                Dictionary<int, string[]> results12 = BasePage.GetSearchResults();
                string[] columnnames12 = BasePage.GetColumnNames();
                string[] PatientFirstName12 = BasePage.GetColumnValues(results12, "First Name", columnnames12);
                bool flag12 = true;

                foreach (String s in PatientFirstName12)
                {
                    if (!s.ToLower().StartsWith("di"))
                    {
                        flag12 = false;
                        break;
                    }
                }

                if (results12.Count == 1 && flag12 && PatientFirstName12.Length == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-13
                //From Inbounds page search study that has a patient ID (piddiff)
                PageLoadWait.WaitForFrameLoad(5);
                inbounds.ClearButton().Click();
                inbounds.SearchStudy(patientID: "piddiff");
                // Inbounds page only list the study with patient ID matching the searching criteria.

                Dictionary<int, string[]> results13 = BasePage.GetSearchResults();
                string[] columnnames13 = BasePage.GetColumnNames();
                string[] PatientID13 = BasePage.GetColumnValues(results13, "Patient ID", columnnames13);
                bool flag13 = true;

                foreach (String s in PatientID13)
                {
                    if (!s.ToLower().StartsWith("piddiff"))
                    {
                        flag13 = false;
                        break;
                    }
                }

                if (results13.Count == 1 && flag13 && PatientID13.Length == 1)
                {
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
                //From Inbounds page delete a study (e.g. a study has a different prefix)
                PageLoadWait.WaitForFrameLoad(5);
                inbounds.ClearButton().Click();
                inbounds.SearchStudy(patientID: PatientID);
                inbounds.SelectStudy("Accession", Accessions1[2]);
                inbounds.DeleteStudy();

                //The selected study is deleted from Inbounds page successfully.
                inbounds.SearchStudy(patientID: PatientID);
                if (inbounds.CheckStudy("Accession", Accessions1[2]) != true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-15
                //Login ICA as the user uploaded these studies (st), from Outbounds page delete a study 
                //(e.g. a study has a different suffix)

                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy(patientID: PatientID);
                outbounds.SelectStudy("Accession", Accessions1[3]);
                outbounds.DeleteStudy();

                //The selected study is deleted from Outbounds page successfully.    
                outbounds.SearchStudy(patientID: PatientID);
                if (outbounds.CheckStudy("Accession", Accessions1[3]) != true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-16
                //Reroute a study to a different destination.
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(AccessionNo: Accessions1[1]);
                inbounds.SelectStudy("Accession", Accessions1[1]);
                inbounds.RerouteStudy(Config.Dest2);
                inbounds.SearchStudy(AccessionNo: Accessions1[1]);

                //Destination of the selected study is updated successfully.

                if (inbounds.CheckStudy("Accession", Accessions1[1]) != true)
                {
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



                //Step-17
                //Go to EA Holding Pen webadmin, open patient list, 
                //Verify that deleted studies are no-longer listed in under these patients.

                login.DriverGoTo(login.hpurl);
                hplogin = new HPLogin();
                hphome = hplogin.LoginHPen(hpUserName, hpPassword);

                workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                workflow.HPSearchStudy("Accessionno", Accessions1[2]);
                bool diff_Prefix = workflow.HPCheckStudy(Accessions1[2]);
                workflow.HPSearchStudy("Accessionno", Accessions1[3]);
                bool diff_Suffix = workflow.HPCheckStudy(Accessions1[3]);

                if (diff_Prefix != true && diff_Suffix != true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                hplogin.LogoutHPen();

                //Step-18
                //Login iCA as a receiver (ph), from Inbounds page nominate the base study for archive; 
                //then login iCA as an archivist(ar), from Inbounds page archive the nominated study without modification
                //(before archive ensure these test data do not exist in destination to be used.)

                login.DriverGoTo(login.url);
                login.LoginIConnect(phUsername, phPassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(AccessionNo: AccessionID);
                inbounds.SelectStudy("Accession", AccessionID);
                inbounds.NominateForArchive("reason");
                login.Logout();

                mpaclogin.DriverGoTo(mpaclogin.mpacdesturl);
                MPHomePage mpachome1 = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);
                Tool tool1 = (Tool)mpachome1.NavigateTopMenu("Tools");
                tool1.NavigateToSendStudy();
                tool1.SearchStudy("Accession", AccessionID, 0);
                //Get study details
                Dictionary<string, string> studyresults = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());
                //Logout from Dest PACS
                mpaclogin.LogoutPacs();

                login.DriverGoTo(login.url);
                login.LoginIConnect(arUsername, arPassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(AccessionNo: AccessionID);
                inbounds.SelectStudy("Accession", AccessionID);
                inbounds.ArchiveStudy("", "");
                inbounds.SearchStudy(AccessionNo: AccessionID);
                inbounds.SelectStudy("Accession", AccessionID);
                String studyStatus;
                inbounds.GetMatchingRow("Accession", AccessionID).TryGetValue("Status", out studyStatus);
                //The study is archived successfully, Study shows Routing Completed.
                if (studyresults == null && inbounds.CheckStudy("Accession", AccessionID) == true && studyStatus == "Routing Completed")
                {
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

                //Step-19
                //Login ICA as physician (ph), from Inbounds page archive the study that has a different patient ID, 
                //and change patient ID to match the base study, e.g. from pidDiffonly90000103 to 90000103, 
                //and update the Study Description to a new value
                //** using staff login, not using physician

                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(patientID: Acc_DiffPIDList[1]);
                inbounds.SelectStudy("Accession", Acc_DiffPIDList[0]);
                inbounds.ClickArchiveStudy("", "");
                inbounds.ArchiveSearch("order", "All Dates");
                inbounds.EditFinalDetailsInArchive("pid", PatientID);
                inbounds.EditFinalDetailsInArchive("description", "New_Des");
                inbounds.ClickArchive();

                //The study is archived successfully, Study shows Routing Completed.
                //Patient ID and Study Description are updated using the newly modified values for this study only.
                PageLoadWait.WaitForFrameLoad(5);
                inbounds.ClearButton().Click();
                inbounds.SearchStudy(AccessionNo: Acc_DiffPIDList[0]);
                inbounds.SelectStudy("Accession", Acc_DiffPIDList[0]);
                Dictionary<string, string> study19 = inbounds.GetMatchingRow(new string[] { "Accession", "Status", "Patient ID", "Description" }, new string[] { Acc_DiffPIDList[0], "Routing Completed", PatientID, "New_Des" });
                //The study is archived successfully, Study shows Routing Completed.
                if (inbounds.CheckStudy("Accession", Acc_DiffPIDList[0]) == true && study19 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-20
                //Archive the study that has a different IPID (IPID2), change the IPID value to match the base study 
                //(IPID1), update Study Description to a new value.


                //inbounds.SearchStudy(patientID: "--Base page IPID --- ");
                PageLoadWait.WaitForFrameLoad(5);
                inbounds.ClearButton().Click();
                inbounds.SearchStudy(AccessionNo: Acc_SamePD_PnameList[0]);
                inbounds.SelectStudy("Accession", Acc_SamePD_PnameList[0]);
                inbounds.ClickArchiveStudy("", "");
                inbounds.ArchiveSearch("order", "All Dates");
                inbounds.EditFinalDetailsInArchive("ipid", ipid1);
                inbounds.EditFinalDetailsInArchive("description", "New_Des1");
                inbounds.ClickArchive();

                //The study is archived successfully, Study shows Routing Completed. 
                //IPID is IPID1 and Study Description is updated using the newly modified values.

                inbounds.ChooseColumns(new string[] { "Issuer of PID" });
                PageLoadWait.WaitForFrameLoad(5);
                inbounds.ClearButton().Click();
                PageLoadWait.WaitForFrameLoad(30);
                inbounds.SearchStudy(AccessionNo: Acc_SamePD_PnameList[0]);
                inbounds.SelectStudy("Accession", Acc_SamePD_PnameList[0]);
                Dictionary<string, string> study20 = inbounds.GetMatchingRow(new string[] { "Accession", "Status", "Issuer of PID", "Description" }, new string[] { Acc_SamePD_PnameList[0], "Routing Completed", ipid1, "New_Des1" });
                //The study is archived successfully, Study shows Routing Completed.
                if (inbounds.CheckStudy("Accession", Acc_SamePD_PnameList[0]) == true && study20 != null)
                {
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


                //Step-21
                //Archive the study that has a different last name, 
                //update Last Name to match the base patient's last name, and update Study Description to a new value.

                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(patientID: PatientID);
                inbounds.SelectStudy("Accession", Accessions1[4]);
                inbounds.ClickArchiveStudy("", "");
                inbounds.ArchiveSearch("order", "All Dates");
                inbounds.EditFinalDetailsInArchive("last name", LastNames[0]);
                inbounds.EditFinalDetailsInArchive("description", "New_Des_LastName");
                inbounds.ClickArchive();

                //The study is archived successfully, Study shows Routing Completed. 
                //Patient Last Name and Study Desc are updated using the newly modified values for this study only.

                inbounds.ChooseColumns(new string[] { "Last Name" });
                PageLoadWait.WaitForFrameLoad(5);
                inbounds.ClearButton().Click();
                PageLoadWait.WaitForFrameLoad(30);
                inbounds.SearchStudy(AccessionNo: Accessions1[4]);
                Dictionary<string, string> study21 = inbounds.GetMatchingRow(new string[] { "Accession", "Status", "Last Name", "Description" }, new string[] { Accessions1[4], "Routing Completed", LastNames[0].ToUpper(), "New_Des_LastName" });
                //The study is archived successfully, Study shows Routing Completed.

                if (inbounds.CheckStudy("Accession", Accessions1[4]) == true && study21 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-22
                //Archive the study that has a different first name, change First Name to match 
                //the base patient's first name and update Study Description to new value.


                inbounds = (Inbounds)login.Navigate("Inbounds");
                PageLoadWait.WaitForFrameLoad(5);
                inbounds.ClearButton().Click();
                inbounds.SearchStudy(patientID: PatientID);
                inbounds.SelectStudy("Accession", Accessions1[0]);
                inbounds.ClickArchiveStudy("", "");
                inbounds.ArchiveSearch("order", "All Dates");
                inbounds.EditFinalDetailsInArchive("first name", FirstNames[0]);
                inbounds.EditFinalDetailsInArchive("description", "New_Des_FirstName");
                inbounds.ClickArchive();

                //The study is archived successfully, Study shows Routing Completed. 
                //Patient First Name and Study Desc are updated using the newly modified values for this study only.

                inbounds.ChooseColumns(new string[] { "First Name" });
                PageLoadWait.WaitForFrameLoad(5);
                inbounds.ClearButton().Click();
                PageLoadWait.WaitForFrameLoad(20);
                inbounds.SearchStudy(AccessionNo: Accessions1[0]);
                Dictionary<string, string> study22 = inbounds.GetMatchingRow(new string[] { "Accession", "Status", "First Name", "Description" }, new string[] { Accessions1[0], "Routing Completed", FirstNames[0].ToUpper(), "New_Des_FirstName" });

                if (inbounds.CheckStudy("Accession", Accessions1[0]) == true && study22 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }



                //Step-23

                //Archive the study that has a different middle name, 
                //change Middle Name to match the base study's middle name, and update Study Description to new values.

                outbounds = (Outbounds)login.Navigate("Outbounds");

                outbounds.SearchStudy(patientID: PatientID);
                outbounds.SelectStudy("Accession", Accessions1[1]);
                outbounds.ClickArchiveStudy("", "");
                outbounds.ArchiveSearch("order", "All Dates");
                outbounds.EditFinalDetailsInArchive("middle name", MiddleNames[0]);
                outbounds.EditFinalDetailsInArchive("description", "New_Des_MiddleName");
                outbounds.ClickArchive();

                //The study is archived successfully, Study shows Routing Completed. 
                //Patient Middle Name and Study Desc are updated using the newly modified values for this study only.

                //outbounds.ChooseColumns(new string[] { "Last Name" });

                PageLoadWait.WaitForFrameLoad(5);
                outbounds.ClearButton().Click();
                PageLoadWait.WaitForFrameLoad(20);
                outbounds.SearchStudy(AccessionNo: Accessions1[1]);
                Dictionary<string, string> study23 = outbounds.GetMatchingRow(new string[] { "Accession", "Status", "Description" }, new string[] { Accessions1[1], "Routing Completed", "New_Des_MiddleName" });

                if (outbounds.CheckStudy("Accession", Accessions1[1]) == true && study23 != null)
                {
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

                //Step-24:
                //From EA Holding Pen webadmin, verify the patients and studies.

                login.DriverGoTo(login.hpurl);
                hplogin = new HPLogin();
                hphome = hplogin.LoginHPen(hpUserName, hpPassword);
                workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                workflow.HPSearchStudy("PatientID", PatientID);

                Dictionary<int, string[]> searchresults24_1 = workflow.GetResultsInHP();
                bool flag_24 = false;
                foreach (string[] value1 in searchresults24_1.Values)
                {
                    if (value1[value1.Length - 1].Equals("6"))
                    {
                        flag_24 = true;
                        break;
                    }
                }


                //These two archived/reconciled studies are merged into the base patient name; 
                //the patient has a different patient last name remains unchanged.

                if (searchresults24_1.Count == 3 && flag_24)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();

                }

                hplogin.LogoutHPen();

                //Step-25
                //Load one of study from the patient from Inbounds page.

                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(patientID: PatientID);
                inbounds.SelectStudy("Accession", Accessions1[0]);
                viewer = StudyViewer.LaunchStudy();

                //Patient name and Patient ID are displayed with same as the base patient study.


                String PatientInfo25 = viewer.PatientInfoTab(1);
                if (viewer.studyPanel(1).Displayed && (PatientInfo25.Split(',')[0].ToUpper()).Equals(LastNames[0].ToUpper()) &&
                    (PatientInfo25.Split(',')[1].ToUpper()).Equals(FirstNames[0].ToUpper()) && (PatientInfo25.Split(',')[2]).Equals(PatientID))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-26
                //Load one of study and open History Panel.                

                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results26 = BasePage.GetSearchResults();

                String[] uploadedAccession26 = { AccessionID, Accessions1[0], Accessions1[1], Accessions1[2], Accessions1[3], Accessions1[4], Acc_DiffPIDList[0], Acc_SamePD_PnameList[0] };
                string[] columnnames26 = BasePage.GetColumnNames();
                string[] PID26 = BasePage.GetColumnValues(results26, "Patient ID", columnnames26);
                string[] ACC26 = BasePage.GetColumnValues(results26, "Accession", columnnames26);

                //Six studies are listed and patient name and ID are the latest updated values Name;- 
                //LASTNAME_UNITY, FIRSTNAME_SD3 Patient ID;- 90000103 DOB;- 08-Jul-1975

                IJavaScriptExecutor executor1 = (IJavaScriptExecutor)BasePage.Driver;
                var Name_26 = executor1.ExecuteScript("return document.querySelector('#m_patientHistory_patientNameTextBox').value");
                var id_26 = executor1.ExecuteScript("return document.querySelector('#m_patientHistory_patientIDTextBox').value");
                var dob_26 = executor1.ExecuteScript("return document.querySelector('#m_patientHistory_patientDOBTextBox').value");

                if (PID26.All(s => s.Equals(PatientIds[0])) && ACC26.All(item => uploadedAccession26.Contains(item))
                    && Name_26.ToString().Equals(LastNames[0].ToUpper() + ", " + FirstNames[0].ToUpper()) &&
                    id_26.ToString().Equals(PatientID) && dob_26.ToString().Equals("08-Jul-1975"))
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



                //Step-27
                //Load each studies from Hisptory panel.

                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions1[0] });
                PageLoadWait.WaitForFrameLoad(20);

                String PatientInfo27_2 = viewer.PatientInfoTab(2);

                viewer.NavigateToHistoryPanel();
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions1[1] });
                PageLoadWait.WaitForFrameLoad(20);

                String PatientInfo27_3 = viewer.PatientInfoTab(3);

                //Images are displayed without error or missing slices.

                if (viewer.studyPanel(2).Displayed &&
                    (PatientInfo27_2.Split(',')[0].ToUpper()).Equals(LastNames[0].ToUpper()) &&
                    (PatientInfo27_2.Split(',')[1].ToUpper()).Equals(FirstNames[0].ToUpper()) &&
                    (PatientInfo27_2.Split(',')[2]).Equals(PatientID) &&
                    viewer.studyPanel(3).Displayed &&
                    (PatientInfo27_3.Split(',')[0].ToUpper()).Equals(LastNames[0].ToUpper()) &&
                    (PatientInfo27_3.Split(',')[1].ToUpper()).Equals(FirstNames[0].ToUpper()) &&
                    (PatientInfo27_3.Split(',')[2]).Equals(PatientID))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseStudy();
                login.Logout();
                //Step-28
                //From EI1 upload a study that only has the same Patient ID and Last Name as the base study,
                //e.g. a study that has different first name and middle name compare to the base study.

                ei.EIDicomUpload(ST, ST, Dest1, Filepaths[7], 1, EIPath);

                //The study is uploaded successful with status Uploaded. Patient name and ID remains unchanged.

                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                //Navigate to inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                //Search Study
                inbounds.SearchStudy(FirstName: FirstNames[2]);
                inbounds.SearchStudy(patientID: PatientID);
                inbounds.ChooseColumns(new string[] { "First Name" });
                inbounds.ChooseColumns(new string[] { "Last Name" });

                //Validate study is present in physician's inbounds                
                Dictionary<string, string> row28 = inbounds.GetMatchingRow(new string[] { "Accession", "Last Name", "First Name", "Patient ID", "Status" }, new string[] { AccessionID, LastNames[0].ToUpper(), FirstNames[2].ToUpper(), PatientID, "Uploaded" });
                if (row28 != null)
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
                login.Logout();


                //Step-29:
                //From EA holding pen WebAdmin page, verify the study is not merged.

                login.DriverGoTo(login.hpurl);
                hplogin = new HPLogin();
                hphomepage = hplogin.LoginHPen(hpUserName, hpPassword);
                workflow = (WorkFlow)hphomepage.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                //Search study               
                workflow.HPSearchStudy("PatientID", PatientID);

                Dictionary<int, string[]> searchresults29_1 = workflow.GetResultsInHP();
                bool flag_29_1 = false;
                foreach (string[] value1 in searchresults29_1.Values)
                {
                    if (value1[value1.Length - 1].Equals("6"))
                    {
                        flag_29_1 = true;
                        break;
                    }
                }

                workflow.Clearform();
                workflow.HPSearchStudy("Firstname", FirstNames[2]);

                Dictionary<int, string[]> searchresults29_2 = workflow.GetResultsInHP();
                bool flag_29_2 = false;
                foreach (string[] value1 in searchresults29_2.Values)
                {
                    if (value1[value1.Length - 1].Equals("1"))
                    {
                        flag_29_2 = true;
                        break;
                    }
                }

                //Validate study is present in Holding pen 
                if (flag_29_1 && flag_29_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                hplogin.LogoutHPen();

                //Step-30
                //Archive the study by modifying all values to be the same as the base study 
                //except patient Last Name to use a new value, e.g. LastName;;LastNew

                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(FirstName: FirstNames[2]);
                inbounds.SelectStudy("Accession", AccessionID);
                inbounds.ClickArchiveStudy("", "");
                inbounds.ArchiveSearch("order", "All Dates");

                inbounds.EditFinalDetailsInArchive("pid", PatientID);
                inbounds.EditFinalDetailsInArchive("first name", FirstNames[0]);
                inbounds.EditFinalDetailsInArchive("middle name", MiddleNames[0]);
                inbounds.EditFinalDetailsInArchive("last name", "LASTNEW");
                inbounds.ClickArchive();

                //The study is archived successfully, Study shows Routing Completed. 
                //Patient Last Name and Study Description are updated using the newly modified values.
                inbounds.ChooseColumns(new string[] { "Last Name" });
                PageLoadWait.WaitForFrameLoad(5);
                inbounds.ClearButton().Click();
                inbounds.SearchStudy(LastName: "LASTNEW");
                PageLoadWait.WaitForFrameLoad(30);
                inbounds.SearchStudy(AccessionNo: AccessionID);
                Dictionary<string, string> study30 = inbounds.GetMatchingRow(new string[] { "Accession", "Status", "Last Name", "Patient ID" }, new string[] { AccessionID, "Routing Completed", "LASTNEW", PatientID });
                //The study is archived successfully, Study shows Routing Completed.
                if (inbounds.CheckStudy("Accession", Acc_DiffPID_LastNameList[0]) == true && study30 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-31:Load the study and open History Panel.
                inbounds.SearchStudy(AccessionNo: Acc_DiffPID_LastNameList[0]);
                inbounds.SelectStudy("Last Name", "LASTNEW");
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                Dictionary<int, string[]> results31 = BasePage.GetSearchResults();
                string[] columnnames31 = BasePage.GetColumnNames();
                string[] PID31 = BasePage.GetColumnValues(results31, "Patient ID", columnnames31);

                //There is only one study listed under this patient with last name LastNew.

                if (PID31.Length == 1 && PID31[0].Equals(PatientID))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseHistoryPanel();
                viewer.CloseStudy();
                login.Logout();


                //Step-32:
                //From EI1 upload a study that only has the same Patient ID and First Name as the base study, 
                //e.g. a study that has different last name and middle name compare to the base study.

                ei.EIDicomUpload(ST, ST, Dest1, Filepaths[8], 1, EIPath);

                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                //Navigate to inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                //Search Study
                inbounds.SearchStudy(LastName: LastNames[2]);
                inbounds.SearchStudy(patientID: PatientID);
                inbounds.ChooseColumns(new string[] { "Last Name", "First Name" });

                //The study is uploaded successful with status Uploaded. Patient name and ID remains unchanged.

                Dictionary<string, string> row32 = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status", "Last Name", "First Name" }, new string[] { AccessionID, PatientID, "Uploaded", LastNames[2].ToUpper(), FirstNames[0].ToUpper() });
                if (row32 != null)
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
                login.Logout();

                //Step-33
                //From EA holding pen WebAdmin page, verify the study is not merged.

                login.DriverGoTo(login.hpurl);
                hplogin = new HPLogin();
                hphomepage = hplogin.LoginHPen(hpUserName, hpPassword);
                workflow = (WorkFlow)hphomepage.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                //Search study               
                workflow.HPSearchStudy("PatientID", PatientID);

                Dictionary<int, string[]> searchresults33_1 = workflow.GetResultsInHP();
                bool flag_33_1 = false;
                foreach (string[] value1 in searchresults33_1.Values)
                {
                    if (value1[value1.Length - 1].Equals("6"))
                    {
                        flag_33_1 = true;
                        break;
                    }
                }

                workflow.Clearform();
                workflow.HPSearchStudy("Lastname", LastNames[2]);

                Dictionary<int, string[]> searchresults33_3 = workflow.GetResultsInHP();
                bool flag_33_3 = false;
                foreach (string[] value1 in searchresults33_3.Values)
                {
                    if (value1[value1.Length - 1].Equals("1"))
                    {
                        flag_33_3 = true;
                        break;
                    }
                }

                //The study is not merged in EA HP patient list.

                if (flag_33_1 && flag_33_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                hplogin.LogoutHPen();



                //Step-34
                //Archive the study by modifying all values to be the same as the base study 
                //except patient First Name to use a new value, e.g. FirstName;;FirstNew

                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(LastName: LastNames[2]);
                inbounds.SelectStudy("Accession", AccessionID);
                inbounds.ClickArchiveStudy("", "");
                inbounds.ArchiveSearch("order", "All Dates");

                inbounds.EditFinalDetailsInArchive("pid", PatientID);
                inbounds.EditFinalDetailsInArchive("last name", LastNames[0]);
                inbounds.EditFinalDetailsInArchive("middle name", MiddleNames[0]);

                inbounds.EditFinalDetailsInArchive("Description", "New Description_34");
                inbounds.EditFinalDetailsInArchive("first name", "FIRSTNEW");
                inbounds.ClickArchive();

                //The study is archived successfully, Study shows Routing Completed. 
                //Patient First Name is updated using the newly modified value for this study only, 
                //and the study does not merged into the base study or
                //other patients with difference in patient name, patient ID and IPID.

                inbounds.ChooseColumns(new string[] { "First Name" });
                PageLoadWait.WaitForFrameLoad(5);
                inbounds.ClearButton().Click();
                inbounds.SearchStudy(FirstName: "FIRSTNEW");
                inbounds.SearchStudy(AccessionNo: AccessionID);
                Dictionary<string, string> study34 = inbounds.GetMatchingRow(new string[] { "Accession", "Status", "First Name", "Patient ID", "Description" }, new string[] { AccessionID, "Routing Completed", "FIRSTNEW", PatientID, "New Description_34" });
                //The study is archived successfully, Study shows Routing Completed.

                if (study34 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }



                //Step-35:Load the study and open History Panel.
                inbounds.SelectStudy("First Name", "FIRSTNEW");
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                Dictionary<int, string[]> results35 = BasePage.GetSearchResults();
                string[] columnnames35 = BasePage.GetColumnNames();
                string[] PID35 = BasePage.GetColumnValues(results35, "Patient ID", columnnames35);

                //There is only one study listed under this patient with first name FirstNew.

                if (PID35.Length == 1 && PID35[0].Equals(PatientID))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseHistoryPanel();
                viewer.CloseStudy();
                login.Logout();


                //Step-36
                //Upload any study that does not have the same patient name and patient ID.

                ei.EIDicomUpload(ST, ST, Dest1, Filepaths[9], 1, EIPath);

                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                //Navigate to inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                //Search Study
                inbounds.SearchStudy(AccessionNo: Acc_DiffPID_FullNameList[0]);

                //The study is uploaded successful with status Uploaded. Patient name and ID remains unchanged.

                Dictionary<string, string> row36 = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Patient Name", "Status" }, new string[] { Acc_DiffPID_FullNameList[0], Acc_DiffPID_FullNameList[1], Acc_DiffPID_FullNameList[2].ToUpper() + ", " + Acc_DiffPID_FullNameList[3].ToUpper(), "Uploaded" });
                if (row36 != null)
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


                //Step-37

                //Archive patient to match all fields (Last Name, First Name, Middle Name, 
                //Prefix, Suffix, Gender, DOB, IPID and PID) as the patient with new last name (e.g. LastNew)
                PageLoadWait.WaitForFrameLoad(5);
                inbounds.ClearButton().Click();
                inbounds.SearchStudy(LastName: Acc_DiffPID_FullNameList[2]);
                inbounds.SearchStudy(AccessionNo: Acc_DiffPID_FullNameList[0]);
                inbounds.SelectStudy("Accession", Acc_DiffPID_FullNameList[0]);

                inbounds.ClickArchiveStudy("", "");
                inbounds.ArchiveSearch("patient", "All Dates");
                inbounds.EditFinalDetailsInArchive("pid", PatientID);
                inbounds.EditFinalDetailsInArchive("middle name", MiddleNames[0]);
                inbounds.EditFinalDetailsInArchive("first name", FirstNames[0]);
                inbounds.EditFinalDetailsInArchive("prefix", Prefixs[0]);
                inbounds.EditFinalDetailsInArchive("suffix", Suffixs[0]);

                inbounds.EditFinalDetailsInArchive("description", "New_Desc_37");
                inbounds.EditFinalDetailsInArchive("last name", "LASTNEW");

                inbounds.ClickArchive();

                //The study is archived successfully, Study shows Routing Completed. 
                //Patient Last Name and Study Description are updated using the newly modified values.

                inbounds.ChooseColumns(new string[] { "Last Name" });
                PageLoadWait.WaitForFrameLoad(5);
                inbounds.ClearButton().Click();
                PageLoadWait.WaitForFrameLoad(30);
                inbounds.SearchStudy(AccessionNo: Acc_DiffPID_FullNameList[0]);
                inbounds.SelectStudy("Accession", Acc_DiffPID_FullNameList[0]);
                Dictionary<string, string> study37 = inbounds.GetMatchingRow(new string[] { "Accession", "Status", "Last Name", "Patient ID", "Description" }, new string[] { Acc_DiffPID_FullNameList[0], "Routing Completed", "LASTNEW", PatientID, "New_Desc_37" });

                //The study is archived successfully, Study shows Routing Completed.
                if (inbounds.CheckStudy("Accession", Acc_DiffPID_FullNameList[0]) == true && study37 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-38
                //Load the study and open History Panel. (Last Name ;; LastNew)
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                inbounds.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results38 = BasePage.GetSearchResults();
                string[] columnnames38 = BasePage.GetColumnNames();
                string[] PID38 = BasePage.GetColumnValues(results38, "Patient ID", columnnames38);
                String[] ACC38 = BasePage.GetColumnValues(results38, "Accession", columnnames38);
                //There are two studies listed under this patient, e.g. the newly archived/reconciled 
                //patient is merged to the existing patient. (Last Name ;; LastNew)

                String[] uploadedAccession38 = { AccessionID, Acc_DiffPID_FullNameList[0] };

                if (PID38.Length == 2 && uploadedAccession38.All(item => ACC38.Contains(item)))
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

                //Step-39
                //Load the 2nd study into the 2nd viewer.

                viewer.OpenPriors(new string[] { "Accession" }, new string[] { AccessionID });
                PageLoadWait.WaitForFrameLoad(20);

                String PatientInfo39 = viewer.PatientInfoTab(2);
                if (viewer.studyPanel(2).Displayed && (PatientInfo39.Split(',')[0].ToUpper()).Equals("LASTNEW") && (PatientInfo39.Split(',')[1].ToUpper()).Equals(FirstNames[0].ToUpper()) && (PatientInfo39.Split(',')[2]).Equals(PatientID))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseStudy();
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
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                //login.Logout();

                //Return Result
                return result;
            }
            finally
            {
                try
                {
                    ServiceTool st = new ServiceTool();
                    WpfObjects wpfobject = new WpfObjects();
                    Taskbar bar = new Taskbar();
                    bar.Hide();
                    st.LaunchServiceTool();
                    st.NavigateToTab("Image Sharing");
                    wpfobject.WaitTillLoad();
                    st.NavigateSubTab("Image Sharing Options");
                    wpfobject.WaitTillLoad();
                    wpfobject.ClickButton("Modify", 1);
                    ComboBox comboBox = WpfObjects._mainWindow.Get<ComboBox>(SearchCriteria.ByClassName("ComboBox"));
                    comboBox.Select("IPID+PatientId");
                    wpfobject.WaitTillLoad();
                    wpfobject.ClickButton("Apply", 1);
                    wpfobject.WaitTillLoad();
                    wpfobject.ClickButton("6");
                    wpfobject.WaitTillLoad();
                    st.RestartService();
                    wpfobject.WaitTillLoad();
                    st.CloseServiceTool();
                    wpfobject.WaitTillLoad();
                    bar.Show();

                    String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");

                    //Delete All studies
                    login.DriverGoTo(login.url);
                    login.LoginIConnect(Config.ph1UserName, Config.ph1Password);
                    inbounds = (Inbounds)login.Navigate("Inbounds");
                    inbounds.SearchStudy("LastName", "*");
                    inbounds.DeleteAllStudies();
                    login.Logout();

                    try
                    {
                        login.LoginIConnect(ST, ST);
                        outbounds = (Outbounds)login.Navigate("Outbounds");
                        outbounds.SearchStudy("PatientID", PatientID);
                        outbounds.DeleteAllStudies();
                        login.Logout();
                    }
                    catch (Exception) { }
                    //Cleanup Destination PACS
                    SocketClient.Send(Config.DestinationPACS, 7777, "db2cmd -c C:\\SQLLIB\\BIN\\PACSCleanup.bat");
                    SocketClient.Close();

                    //Delete Studies from HP
                    Putty putty1 = new Putty();
                    putty1.EA_Cleanup();

                    int changebrowser = 0;

                    String browserName = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
                    if (!(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer")))
                    {
                        BasePage.Driver.Quit();
                        Config.BrowserType = "internet explorer";
                        Logger.Instance.InfoLog("Swicthing Browser Type to IE");
                        BasePage.Driver = null;
                        hplogin = new HPLogin();
                        BasePage.Driver.Navigate().GoToUrl(hplogin.hpurl);
                        HPHomePage homepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                        PageLoadWait.WaitForHPPageLoad(20);
                        configure = (Configure)homepage.Navigate("Configure");
                        configure.NavigateToTab("properties");
                        changebrowser++;
                    }
                    else
                    {
                        hplogin = new HPLogin();
                        BasePage.Driver.Navigate().GoToUrl(hplogin.hpurl);
                        HPHomePage homepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                        PageLoadWait.WaitForHPPageLoad(20);
                        configure = (Configure)homepage.Navigate("Configure");
                        configure.NavigateToTab("properties");
                    }
                    PageLoadWait.WaitForHPPageLoad(20);
                    configure.NavigateToPropertySubTab("Database");
                    PageLoadWait.WaitForHPPageLoad(20);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("PropertiesContext");

                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#PatientComparisonStrategyClass_txt")));

                    //Update Snapshot Patient Comparison Strategy Class
                    String PCS_snapshot = BasePage.Driver.FindElement(By.CssSelector("input[id='PatientComparisonStrategyClass_txt']")).GetAttribute("value");
                    if (PCS_snapshot != PatientComparisonStrategyClassid_snapshot)
                    {
                        BasePage.Driver.FindElement(By.CssSelector("input[id='PatientComparisonStrategyClass_txt']")).Clear();
                        Logger.Instance.InfoLog("Patient Comparison Strategy Class updated in finally block " + PatientComparisonStrategyClassid_snapshot);
                        BasePage.Driver.FindElement(By.CssSelector("input[id='PatientComparisonStrategyClass_txt']")).SendKeys(PatientComparisonStrategyClassid_snapshot);
                        configure.ClickSubmitChangesBtn();
                        BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("img#PatientComparisonStrategyClass_statusimg")));
                    }
                    //delete Property Database|InternalMergeDefaultMode;;PCS_ONLY
                    //BasePage.Driver.FindElement(By.CssSelector("#InternalMergeDefaultMode_img")).Click();

                    //Save the transaction


                    //logout
                    BasePage.Driver.SwitchTo().DefaultContent();
                    hplogin.LogoutHPen();

                    //Revert back to the same driver type or logout
                    if (changebrowser == 1)
                    {
                        BasePage.Driver.Quit();
                        Config.BrowserType = browserName;
                        Logger.Instance.InfoLog("Swicthing Back Browser to --" + browserName);
                        BasePage.Driver = null;
                        new HPLogin();
                    }

                    Putty putty = new Putty();
                    putty.RestartService();
                }
                catch (Exception)
                {
                    int changebrowser = 0;

                    String browserName = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
                    if (!(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer")))
                    {
                        BasePage.Driver.Quit();
                        Config.BrowserType = "internet explorer";
                        Logger.Instance.InfoLog("Swicthing Browser Type to IE");
                        BasePage.Driver = null;
                        hplogin = new HPLogin();
                        BasePage.Driver.Navigate().GoToUrl(hplogin.hpurl);
                        HPHomePage homepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                        PageLoadWait.WaitForHPPageLoad(20);
                        configure = (Configure)homepage.Navigate("Configure");
                        configure.NavigateToTab("properties");
                        changebrowser++;
                    }
                    else
                    {
                        hplogin = new HPLogin();
                        BasePage.Driver.Navigate().GoToUrl(hplogin.hpurl);
                        HPHomePage homepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                        PageLoadWait.WaitForHPPageLoad(20);
                        configure = (Configure)homepage.Navigate("Configure");
                        configure.NavigateToTab("properties");
                    }
                    PageLoadWait.WaitForHPPageLoad(20);
                    configure.NavigateToPropertySubTab("Database");
                    PageLoadWait.WaitForHPPageLoad(20);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("PropertiesContext");

                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#PatientComparisonStrategyClass_txt")));

                    //Update Snapshot Patient Comparison Strategy Class
                    String PCS_snapshot = BasePage.Driver.FindElement(By.CssSelector("input[id='PatientComparisonStrategyClass_txt']")).GetAttribute("value");
                    if (PCS_snapshot != PatientComparisonStrategyClassid_snapshot)
                    {
                        BasePage.Driver.FindElement(By.CssSelector("input[id='PatientComparisonStrategyClass_txt']")).Clear();
                        Logger.Instance.InfoLog("Patient Comparison Strategy Class updated in finally block " + PatientComparisonStrategyClassid_snapshot);
                        BasePage.Driver.FindElement(By.CssSelector("input[id='PatientComparisonStrategyClass_txt']")).SendKeys(PatientComparisonStrategyClassid_snapshot);
                        configure.ClickSubmitChangesBtn();
                        BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("img#PatientComparisonStrategyClass_statusimg")));
                    }
                    //delete Property Database|InternalMergeDefaultMode;;PCS_ONLY
                    //BasePage.Driver.FindElement(By.CssSelector("#InternalMergeDefaultMode_img")).Click();

                    //Save the transaction


                    //logout
                    BasePage.Driver.SwitchTo().DefaultContent();
                    hplogin.LogoutHPen();

                    //Revert back to the same driver type or logout
                    if (changebrowser == 1)
                    {
                        BasePage.Driver.Quit();
                        Config.BrowserType = browserName;
                        Logger.Instance.InfoLog("Swicthing Back Browser to --" + browserName);
                        BasePage.Driver = null;
                        new HPLogin();
                    }

                    Putty putty = new Putty();
                    putty.RestartService();
                }
            }
        }

        /// <summary> 
        /// LastName+PID+IPID- patient name only has last name, last+first name with special characters
        /// </summary>
        public TestCaseResult Test_86305(String testid, String teststeps, int stepcount)
        {
            //-Initial Setup--
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            Configure configure = new Configure();
            HPHomePage hphome = new HPHomePage();
            WorkFlow workflow = new WorkFlow();
            DomainManagement domainmanagement;
            RoleManagement rolemanagement;
            UserManagement usermanagement;
            Inbounds inbounds;
            Outbounds outbounds;
            StudyViewer viewer;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            String UserName = Config.adminUserName;
            String Password = Config.adminPassword;

            String arUsername = Config.ar1UserName;
            String arPassword = Config.ar1Password;

            String phUsername = Config.ph1UserName;
            String phPassword = Config.ph1Password;

            String hpUserName = Config.hpUserName;
            String hpPassword = Config.hpPassword;

            String ST = "ST_PCS3_" + new Random().Next(1000);

            String datasource1 = login.GetHostName(Config.PACS2);//10.5.38.27(A6)
            String datasource2 = login.GetHostName(Config.SanityPACS);//10.5.38.28(A7)   
            String ipid1 = Config.ipid1;
            String ipid2 = Config.ipid2;

            String Dest1 = Config.Dest1;
            String Dest2 = Config.Dest2;

            String DefaultDomain = "SuperAdminGroup";
            String Staff = "StaffRole_PCS3_" + new Random().Next(1000);

            String PatientComparisonStrategyClassid_snapshot = "";
            String key = "InternalMergeDefaultMode";
            String value = "PCS_ONLY";

            String MergeExpirationLevel_snapshot = "";
            try
            {

                //Step-1
                //HP -setup
                String newtag = "imageon.archive.core.database.jdbc.LastNameOnlyComparisonStrategy";

                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String[] LastNames = LastName.Split(':');
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String[] FirstNames = FirstName.Split(':');
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String[] Accessions1 = AccessionID.Split(':');
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String[] PatientIDs = PatientIDList.Split(':');
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String[] Filepaths = UploadFilePath.Split('=');
                String UploadFilePath_2 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath_2");
                String[] Filepaths_2 = UploadFilePath_2.Split('=');
                String EIPath = Config.EIFilePath;


                int changebrowser = 0;
                String browserName = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
                if (!(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer")))
                {
                    BasePage.Driver.Quit();
                    Config.BrowserType = "internet explorer";
                    Logger.Instance.InfoLog("Swicthing Browser Type to IE");
                    BasePage.Driver = null;
                    hplogin = new HPLogin();
                    BasePage.Driver.Navigate().GoToUrl(hplogin.hpurl);
                    HPHomePage homepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                    PageLoadWait.WaitForHPPageLoad(20);
                    configure = (Configure)homepage.Navigate("Configure");
                    configure.NavigateToTab("properties");
                    changebrowser++;
                }
                else
                {
                    BasePage.Driver.Navigate().GoToUrl(hplogin.hpurl);
                    HPHomePage homepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                    PageLoadWait.WaitForHPPageLoad(20);
                    configure = (Configure)homepage.Navigate("Configure");
                    configure.NavigateToTab("properties");
                }
                PageLoadWait.WaitForHPPageLoad(20);
                configure.NavigateToPropertySubTab("Database");
                PageLoadWait.WaitForHPPageLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("PropertiesContext");

                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#PatientComparisonStrategyClass_txt")));

                //Get Snapshot of query id tags before updating
                PatientComparisonStrategyClassid_snapshot = BasePage.Driver.FindElement(By.CssSelector("input[id='PatientComparisonStrategyClass_txt']")).GetAttribute("value");
                Logger.Instance.InfoLog("Patient Comparison Strategy Class Id snapshot before modification--" + PatientComparisonStrategyClassid_snapshot);

                BasePage.Driver.FindElement(By.CssSelector("input[id='PatientComparisonStrategyClass_txt']")).Clear();
                Logger.Instance.InfoLog("Patient Comparison Strategy Class updated to-- " + newtag);
                BasePage.Driver.FindElement(By.CssSelector("input[id='PatientComparisonStrategyClass_txt']")).SendKeys(newtag);

                //add a new Property to Database|InternalMergeDefaultMode;;PCS_ONLY

                configure.AddProperty(key, value);

                //Set MergeExpirationLevel as 0
                MergeExpirationLevel_snapshot = BasePage.Driver.FindElement(By.CssSelector("input[id='MergeExpirationInterval_txt']")).GetAttribute("value");
                Logger.Instance.InfoLog("MergeExpirationLevel_snapshot before modification--" + MergeExpirationLevel_snapshot);
                BasePage.Driver.FindElement(By.CssSelector("input[id='MergeExpirationInterval_txt']")).Clear();
                BasePage.Driver.FindElement(By.CssSelector("input[id='MergeExpirationInterval_txt']")).SendKeys("0");
                Logger.Instance.InfoLog("MergeExpirationLevel_snapshot updated to--0");

                //Save the transaction
                configure.ClickSubmitChangesBtn();
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("img#PatientComparisonStrategyClass_statusimg")));
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("img#InternalMergeDefaultMode_statusimg")));
                //logout
                BasePage.Driver.SwitchTo().DefaultContent();
                hplogin.LogoutHPen();

                //Revert back to the same driver type or logout
                if (changebrowser == 1)
                {
                    BasePage.Driver.Quit();
                    Config.BrowserType = browserName;
                    Logger.Instance.InfoLog("Swicthing Back Browser to --" + browserName);
                    BasePage.Driver = null;
                    new HPLogin();
                }

                Putty putty = new Putty();
                putty.RestartService();
                ExecutedSteps++;

                //Step-2
                //From ICA service tool\Image Sharing tab\Image Sharing Options sub-tab, 
                //set the Patient Comparison Strategy to IPID+PatientId+LastName
                //Restart IIS and Windows Services.
                //Open WebAccessConfiguration.xml under C;-\WebAccess\WebAccess\Config folder, 
                //verify the changes is saved in the file.

                ServiceTool st = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();
                Taskbar bar = new Taskbar();
                bar.Hide();
                st.LaunchServiceTool();
                st.NavigateToTab("Image Sharing");
                wpfobject.WaitTillLoad();
                st.NavigateSubTab("Image Sharing Options");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);
                ComboBox comboBox = WpfObjects._mainWindow.Get<ComboBox>(SearchCriteria.ByClassName("ComboBox"));
                comboBox.Select("IPID+PatientId+LastName");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("6");
                wpfobject.WaitTillLoad();
                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();
                bar.Show();

                //This configuration is saved in "WebAccessConfgiruation" file, `<PatientComparisonStrategy>
                //<IPID>True</IPID> 
                //<PatientId>True</PatientId>
                //<LastName>True</LastName>
                //<PatientName>False</PatientName>
                //</PatientComparisonStrategy>

                String WebAccessConfigPath = @"C:\WebAccess\WebAccess\Config\WebAccessConfiguration.xml";
                String NodePath = "Configuration/ImageSharing/PatientComparisonStrategy/";

                if (st.GetNodeValue(WebAccessConfigPath, NodePath + "IPID") == "True" &&
                    st.GetNodeValue(WebAccessConfigPath, NodePath + "PatientId") == "True" &&
                    st.GetNodeValue(WebAccessConfigPath, NodePath + "LastName") == "True" &&
                    st.GetNodeValue(WebAccessConfigPath, NodePath + "PatientName") == "False")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-3:Initial Setups
                //Create a new imagesharing-domain

                login.DriverGoTo(login.url);
                login.LoginIConnect(UserName, Password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("patientid", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientfullname", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientlastname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientdob", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientipid", 0);
                domainmanagement.ClickSaveDomain();


                //Create roles and users as specified and set Quersy search parameters               
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.CreateRole(DefaultDomain, Staff, "both");

                bool strole = rolemanagement.RoleExists(Staff);
                bool phrole = rolemanagement.RoleExists("Physician");
                bool arrole = rolemanagement.RoleExists("Archivist");

                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(ST, DefaultDomain, Staff);

                bool newst = usermanagement.SearchUser(ST, DefaultDomain);
                bool ph = usermanagement.SearchUser(Config.ph1UserName, DefaultDomain);
                bool ar = usermanagement.SearchUser(Config.ar1UserName, DefaultDomain);
                //Navigate to Image Sharing-->Institution tab

                Image_Sharing imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                Image_Sharing.Destination dest = (Image_Sharing.Destination)imagesharing.NavigateToSubTab("Destination");
                dest.EditDestination(DefaultDomain, Dest1, ST, ST);

                if (strole && phrole && arrole && newst && ph && ar)
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
                login.Logout();

                //Step-4
                //Created INST1 and INST2
                ExecutedSteps++;


                //Step-5:From EI1 (IPID1) upload a study with Last and First Name contain a Hyphen.
                //Patient Name;- Hypen-A^Hypen-B
                //Patient ID;- 123
                //IPID;- IPID1
                //Study Description;- only has last+first name segments
                ei.EIDicomUpload(ST, ST, Dest1, Filepaths[0], 1, EIPath);

                //Login as ST 
                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                PageLoadWait.WaitforStudyInStatus(Accessions1[0], inbounds, "Uploaded");
                inbounds.SearchStudy(AccessionNo: Accessions1[0]);

                //Validate study is present in physician's inbounds
                Dictionary<string, string> row5 = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status" }, new string[] { Accessions1[0], PatientID, "Uploaded" });
                if (row5 != null)
                {
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


                //Step-6:Login ICA as user who can access to Inbounds studies. From Inbounds page archive the study by update the following in Reconcile/Archive Study dialog;-
                //Patient Name;- Hypen-A1^Hypen-B1
                //Patient ID;- 1231
                //IPID;- IPID1a
                //Study Description;- has hyphen and only last+first name segments
                //Archive the study              

                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                //Navigate to inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.ChooseColumns(new string[] { "Issuer of PID" });
                inbounds.SearchStudy(AccessionNo: Accessions1[0]);
                inbounds.SelectStudy("Accession", Accessions1[0]);
                //Reconcile
                inbounds.ClickArchiveStudy("", "");
                inbounds.ArchiveSearch("order", "All Dates");
                inbounds.EditFinalDetailsInArchive("last name", "Hypen-A1");
                inbounds.EditFinalDetailsInArchive("first name", "Hypen-B1");
                inbounds.EditFinalDetailsInArchive("pid", "1231");
                inbounds.EditFinalDetailsInArchive("ipid", "IPID1a");
                inbounds.EditFinalDetailsInArchive("description", "has hyphen and only last+first name segments");
                inbounds.ClickArchive();
                inbounds.SearchStudy(AccessionNo: Accessions1[0]);
                Dictionary<string, string> Rec_HypenStudy = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status", "Issuer of PID", "Description" }, new string[] { Accessions1[0], "1231", "Routing Completed", "IPID1a", "has hyphen and only last+first name segments" });

                if (Rec_HypenStudy != null)
                {
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

                //Step-7:From EI1 (IPID1) upload one of studies of the patient with Last Name and First Name contain a Period (study1).
                //Patient Name;- Period.^A.
                //Patient ID;- 123
                //IPID;- IPID1
                //Study Description;- only has last and first name
                ei.EIDicomUpload(ST, ST, Dest1, Filepaths[1], 1, EIPath);
                //Login as ST 
                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                PageLoadWait.WaitforStudyInStatus(Accessions1[1], inbounds, "Uploaded");
                inbounds.SearchStudy(AccessionNo: Accessions1[1]);

                //Validate study is present in physician's inbounds
                Dictionary<string, string> row7 = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status" }, new string[] { Accessions1[1], PatientID, "Uploaded" });
                if (row7 != null)
                {
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

                //Step-8:Login ICA as the user who sent the study. From Outbounds page archive the study by update the following in Reconcile/Archive Study dialog:
                //Last Name: Period.Mr.
                //Patient ID: 1231
                //Study Description: has a period and only has last and first name
                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                //Navigate to inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.ChooseColumns(new string[] { "Issuer of PID" });
                inbounds.SearchStudy(AccessionNo: Accessions1[1]);
                inbounds.SelectStudy("Accession", Accessions1[1]);
                //Reconcile
                inbounds.ClickArchiveStudy("", "");
                inbounds.ArchiveSearch("order", "All Dates");
                inbounds.EditFinalDetailsInArchive("last name", "Period.Mr.");
                inbounds.EditFinalDetailsInArchive("pid", "1231");
                inbounds.EditFinalDetailsInArchive("description", "has a period and only has last and first name");
                inbounds.ClickArchive();
                inbounds.SearchStudy(AccessionNo: Accessions1[1]);
                Dictionary<string, string> Rec_HypenStudy8 = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status", "Description" }, new string[] { Accessions1[1], "1231", "Routing Completed", "has a period and only has last and first name" });

                if (Rec_HypenStudy8 != null)
                {
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


                //Step-9:From EI1 (IPID1) upload the 2nd study of the patient with Last Name and First Name contain a Period (study2).
                //Patient Name: Period.^A.
                //Patient ID: 123
                //IPID: IPID1
                //Study Description: only has last and first name
                ei.EIDicomUpload(ST, ST, Dest1, Filepaths[2], 1, EIPath);

                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                //Navigate to inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                //Search Study
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitforStudyInStatus(Accessions1[2], inbounds, "Uploaded");
                inbounds.SearchStudy(AccessionNo: Accessions1[2]);

                //Validate study is present in physician's inbounds                
                Dictionary<string, string> row9 = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status" }, new string[] { Accessions1[2], PatientID, "Uploaded" });
                if (row9 != null)
                {
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


                //Step-10:Login ICA as a receiver. Verify the uploaded study in Inbounds page.                
                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(AccessionNo: Accessions1[2]);
                inbounds.ChooseColumns(new string[] { "Last Name", "First Name" });
                Dictionary<string, string> row10 = inbounds.GetMatchingRow(new string[] { "Last Name", "First Name" }, new string[] { LastNames[3].ToUpper(), FirstNames[1] });
                if (BasePage.GetSearchResults().Count == 1 && row10 != null)
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


                //Step-11:Load the study and open History Panel.
                inbounds.SelectStudy("Accession", Accessions1[2]);
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results11 = BasePage.GetSearchResults();
                string[] columnnames8 = BasePage.GetColumnNames();
                string[] PID_8 = BasePage.GetColumnValues(results11, "Patient ID", columnnames8);
                string[] ACC_8 = BasePage.GetColumnValues(results11, "Accession", columnnames8);
                if (results11.Count == 1 && Array.Exists(PID_8, s => s.Equals(PatientID)) && Array.Exists(ACC_8, s => s.Equals(Accessions1[2])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseHistoryPanel();
                viewer.CloseStudy();


                //Step-12:Reconcile the study by change the following in Reconcile/Archive Study dialog to match the previous archived study;-
                //Last Name;- Period.Mr.
                //Patient ID;- 1231
                //Study Description;- 2nd has a period and only has last and first name
                inbounds.SearchStudy(AccessionNo: Accessions1[2]);
                inbounds.SelectStudy("Accession", Accessions1[2]);
                //Reconcile
                inbounds.ClickArchiveStudy("", "");
                inbounds.ArchiveSearch("order", "All Dates");
                inbounds.EditFinalDetailsInArchive("last name", "Period.Mr.");
                inbounds.EditFinalDetailsInArchive("pid", "1231");
                inbounds.EditFinalDetailsInArchive("description", "2nd has a period and only has last and first name");
                inbounds.ClickArchive();
                inbounds.SearchStudy(AccessionNo: Accessions1[2]);
                Dictionary<string, string> Rec_HypenStudy12 = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status", "Description" }, new string[] { Accessions1[2], "1231", "Routing Completed", "2nd has a period and only has last and first name" });

                if (Rec_HypenStudy12 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                }


                //Step-13:Load the study and verify this study is merged to patient with the same patient Last Name, patient ID and IPID (check in EA HP or iCA History Panel)
                inbounds.SearchStudy(AccessionNo: Accessions1[2]);
                inbounds.SelectStudy("Accession", Accessions1[2]);
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                Dictionary<int, string[]> results13 = BasePage.GetSearchResults();
                string[] columnnames13 = BasePage.GetColumnNames();
                string[] PID_13 = BasePage.GetColumnValues(results13, "Patient ID", columnnames13);
                if (results13.Count == 2 && Array.Exists(PID_13, s => s.Equals("1231")))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseHistoryPanel();
                viewer.CloseStudy();
                login.Logout();


                //Step-14:From EI (IPID1) upload a study and archive it by change the following in Reconcile/Archive Study dialog to have the same patient last and first names as the study just archived e.g.
                //Patient Name;- Period.Mr.^A.
                //Patient ID;- not match number e.g. 66666666
                ei.EIDicomUpload(ST, ST, Dest1, Filepaths[3], 1, EIPath);
                //Login as ST 
                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                PageLoadWait.WaitforStudyInStatus(Accessions1[3], inbounds, "Uploaded");
                inbounds.SearchStudy(AccessionNo: Accessions1[3]);
                //Validate study is present in physician's inbounds
                Dictionary<string, string> row14 = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status" }, new string[] { Accessions1[3], PatientIDs[0], "Uploaded" });
                //Reconcile
                inbounds.SearchStudy(AccessionNo: Accessions1[3]);
                inbounds.SelectStudy("Accession", Accessions1[3]);
                inbounds.ClickArchiveStudy("", "");
                inbounds.ArchiveSearch("order", "All Dates");
                inbounds.EditFinalDetailsInArchive("last name", "Period.Mr.");
                inbounds.EditFinalDetailsInArchive("pid", "6666");
                inbounds.ClickArchive();
                inbounds.SearchStudy(AccessionNo: Accessions1[3]);
                Dictionary<string, string> Rec_HypenStudy14 = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status" }, new string[] { Accessions1[3], "6666", "Routing Completed" });

                if (row14 != null && Rec_HypenStudy14 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-15:Login ICA as a receiver, from Inbounds page load the study and verify this study is not merged to other patient.
                inbounds.SearchStudy(AccessionNo: Accessions1[3]);
                inbounds.SelectStudy("Accession", Accessions1[3]);
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                Dictionary<int, string[]> results15 = BasePage.GetSearchResults();
                if (results15.Count == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseHistoryPanel();
                viewer.CloseStudy();
                login.Logout();


                //Step-16:Start a WebUploader (the purpose is to have a different IPID previously uploaded study from EI;;IPID1), login as a non-registered user, upload a study from the WU.
                //via POP tool (to have different ipid)
                BasePage.RunBatchFile(Config.batchfilepath, Filepaths_2[0] + " " + Config.dicomsendpath + " " + Config.StudyPacs);

                //Send the study to dicom devices from MergePacs management page 
                //login to PACS#1 as admin
                mpaclogin.DriverGoTo(mpaclogin.mpacstudyurl);
                MPHomePage mpachome = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);

                //Send Study with matching HL7 order
                Tool tool = (Tool)mpachome.NavigateTopMenu("Tools");
                tool.NavigateToSendStudy();
                tool.SearchStudy("Accession", Accessions1[4], 0);

                //Get study details
                Dictionary<string, string> MpacDetails = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());

                tool.MpacSelectStudy("Accession", Accessions1[4]);
                tool.SendStudy(1, Config.pacsgatway2);
                mpaclogin.LogoutPacs();

                //Login as receiver
                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                PageLoadWait.WaitforStudyInStatus(Accessions1[4], inbounds, "Uploaded");
                PageLoadWait.WaitforUpload(Accessions1[4], inbounds);

                //Search and Select Study
                inbounds.SearchStudy(AccessionNo: Accessions1[4]);

                //Valiadate study is present in Physician's inbounds
                inbounds.ChooseColumns(new string[] { "Issuer of PID" });
                Dictionary<string, string> row16 = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Issuer of PID" }, new string[] { Accessions1[4], PatientIDs[1], ipid2 });

                if (row16 != null && row16["Status"].Equals("Uploading") || row16["Status"].Equals("Uploaded"))
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
                login.Logout(); ;

                //Step-17:From ICA Inbounds page archive the study uploaded from WU to match an archived patient by change its patient last, first name and PID, e.g.;-
                //Patient Name;- Period.Mr.^A.
                //Patient ID;- 1231
                //and confirm this study does not merge into other patient that has the same Patient name and ID.
                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(AccessionNo: Accessions1[4]);
                inbounds.SelectStudy("Accession", Accessions1[4]);
                inbounds.ClickArchiveStudy("", "");
                inbounds.ArchiveSearch("order", "All Dates");
                inbounds.EditFinalDetailsInArchive("last name", "Period.Mr.");
                inbounds.EditFinalDetailsInArchive("first name", "A");
                inbounds.EditFinalDetailsInArchive("pid", "1231");
                inbounds.ClickArchive();
                inbounds.SearchStudy(AccessionNo: Accessions1[4]);
                Dictionary<string, string> Rec_HypenStudy17 = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status" }, new string[] { Accessions1[4], "1231", "Routing Completed" });
                inbounds.SelectStudy("Accession", Accessions1[4]);
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                Dictionary<int, string[]> results17 = BasePage.GetSearchResults();

                if (results17.Count == 1 && Rec_HypenStudy17 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseHistoryPanel();
                viewer.CloseStudy();
                login.Logout();

                //Step-18:From EI1 (IPID1) upload a study of the patient with Last Name only and it contains double quotes;-
                //Patient Name;- Double "Quote"
                //Patient ID;- 123
                //IPID;- IPID1
                //Study Description;- only has lastname segment
                ei.EIDicomUpload(ST, ST, Dest1, Filepaths[4], 1, EIPath);

                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                PageLoadWait.WaitforStudyInStatus(Accessions1[5], inbounds, "Uploaded");
                inbounds.SearchStudy(AccessionNo: Accessions1[5]);
                //Validate study is present in physician's inbounds
                Dictionary<string, string> row18 = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status" }, new string[] { Accessions1[5], PatientID, "Uploaded" });
                if (row18 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-19:Login as a receiver and archive the study by change study description to a new value.
                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(AccessionNo: Accessions1[5]);
                inbounds.SelectStudy("Accession", Accessions1[5]);
                inbounds.ClickArchiveStudy("", "");
                inbounds.ArchiveSearch("order", "All Dates");
                inbounds.EditFinalDetailsInArchive("description", "has double quote and only has lastname segment");
                inbounds.ClickArchive();
                inbounds.SearchStudy(AccessionNo: Accessions1[5]);
                Dictionary<string, string> Rec_HypenStudy19 = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status", "Description" }, new string[] { Accessions1[5], PatientID, "Routing Completed", "has double quote and only has lastname segment" });
                inbounds.SelectStudy("Accession", Accessions1[5]);
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                Dictionary<int, string[]> results19 = BasePage.GetSearchResults();
                if (results19.Count == 1 && Rec_HypenStudy19 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseHistoryPanel();
                viewer.CloseStudy();
                login.Logout();

                //Step-20:From EI1 (IPID1) upload a study of the patient with Last Name only and it contains Single quotes;-
                //Patient Name;- O'Hara
                //Patient ID;- 123
                //IPID;- IPID1
                //Study Description;- only has lastname segment
                ei.EIDicomUpload(ST, ST, Dest1, Filepaths[5], 1, EIPath);
                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                PageLoadWait.WaitforStudyInStatus(Accessions1[2], inbounds, "Uploaded");
                inbounds.SearchStudy(AccessionNo: Accessions1[2]);
                //Validate study is present in physician's inbounds
                Dictionary<string, string> row20 = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status" }, new string[] { Accessions1[2], PatientID, "Uploaded" });
                if (row20 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-21:Login as a receiver and archive the study by change study description to a new value.
                login.DriverGoTo(login.url);
                login.LoginIConnect(ST, ST);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(AccessionNo: Accessions1[2]);
                inbounds.SelectStudy("Accession", Accessions1[2]);
                inbounds.ClickArchiveStudy("", "");
                inbounds.ArchiveSearch("order", "All Dates");
                inbounds.EditFinalDetailsInArchive("description", "has single quote and only has lastname segment");
                inbounds.ClickArchive();

                //The study is archived successfully, Study shows Routing Completed. 
                //Patient ID and Study Description are updated using the newly modified values.
                PageLoadWait.WaitForFrameLoad(5);
                inbounds.ClearButton().Click();
                inbounds.SearchStudy(AccessionNo: Accessions1[2]);
                Dictionary<string, string> study21 = inbounds.GetMatchingRow(new string[] { "Accession", "Status", "Patient ID", "Description" }, new string[] { Accessions1[2], "Routing Completed", PatientID, "has single quote and only has lastname segment" });
                //The study is archived successfully, Study shows Routing Completed.
                if (study21 != null)
                {
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
                    ServiceTool st = new ServiceTool();
                    WpfObjects wpfobject = new WpfObjects();
                    Taskbar bar = new Taskbar();
                    bar.Hide();
                    st.LaunchServiceTool();
                    st.NavigateToTab("Image Sharing");
                    wpfobject.WaitTillLoad();
                    st.NavigateSubTab("Image Sharing Options");
                    wpfobject.WaitTillLoad();
                    wpfobject.ClickButton("Modify", 1);
                    ComboBox comboBox = WpfObjects._mainWindow.Get<ComboBox>(SearchCriteria.ByClassName("ComboBox"));
                    comboBox.Select("IPID+PatientId");
                    wpfobject.WaitTillLoad();
                    wpfobject.ClickButton("Apply", 1);
                    wpfobject.WaitTillLoad();
                    wpfobject.ClickButton("6");
                    wpfobject.WaitTillLoad();
                    st.RestartService();
                    wpfobject.WaitTillLoad();
                    st.CloseServiceTool();
                    wpfobject.WaitTillLoad();
                    bar.Show();

                    String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");

                    //Delete All studies
                    login.DriverGoTo(login.url);
                    login.LoginIConnect(Config.ph1UserName, Config.ph1Password);
                    inbounds = (Inbounds)login.Navigate("Inbounds");
                    inbounds.SearchStudy("LastName", "*");
                    inbounds.DeleteAllStudies();
                    login.Logout();

                    try
                    {
                        login.LoginIConnect(ST, ST);
                        outbounds = (Outbounds)login.Navigate("Outbounds");
                        outbounds.SearchStudy("LastName", "*");
                        outbounds.DeleteAllStudies();
                        login.Logout();
                    }
                    catch (Exception) { }
                    //Cleanup Destination PACS
                    SocketClient.Send(Config.DestinationPACS, 7777, "db2cmd -c C:\\SQLLIB\\BIN\\PACSCleanup.bat");
                    SocketClient.Close();

                    //Delete Studies from EA(Holding Pen)
                    Putty putty1 = new Putty();
                    putty1.EA_Cleanup();

                    int changebrowser = 0;

                    String browserName = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
                    if (!(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer")))
                    {
                        BasePage.Driver.Quit();
                        Config.BrowserType = "internet explorer";
                        Logger.Instance.InfoLog("Swicthing Browser Type to IE");
                        BasePage.Driver = null;
                        hplogin = new HPLogin();
                        BasePage.Driver.Navigate().GoToUrl(hplogin.hpurl);
                        HPHomePage homepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                        PageLoadWait.WaitForHPPageLoad(20);
                        configure = (Configure)homepage.Navigate("Configure");
                        configure.NavigateToTab("properties");
                        changebrowser++;
                    }
                    else
                    {
                        hplogin = new HPLogin();
                        BasePage.Driver.Navigate().GoToUrl(hplogin.hpurl);
                        HPHomePage homepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                        PageLoadWait.WaitForHPPageLoad(20);
                        configure = (Configure)homepage.Navigate("Configure");
                        configure.NavigateToTab("properties");
                    }
                    PageLoadWait.WaitForHPPageLoad(20);
                    configure.NavigateToPropertySubTab("Database");
                    PageLoadWait.WaitForHPPageLoad(20);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("PropertiesContext");

                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#PatientComparisonStrategyClass_txt")));

                    //Update Snapshot Patient Comparison Strategy Class
                    String PCS_snapshot = BasePage.Driver.FindElement(By.CssSelector("input[id='PatientComparisonStrategyClass_txt']")).GetAttribute("value");
                    if (PCS_snapshot != PatientComparisonStrategyClassid_snapshot)
                    {
                        BasePage.Driver.FindElement(By.CssSelector("input[id='PatientComparisonStrategyClass_txt']")).Clear();
                        Logger.Instance.InfoLog("Patient Comparison Strategy Class updated in finally block " + PatientComparisonStrategyClassid_snapshot);
                        BasePage.Driver.FindElement(By.CssSelector("input[id='PatientComparisonStrategyClass_txt']")).SendKeys(PatientComparisonStrategyClassid_snapshot);
                        configure.ClickSubmitChangesBtn();
                        BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("img#PatientComparisonStrategyClass_statusimg")));
                    }
                    //delete Property Database|InternalMergeDefaultMode;;PCS_ONLY
                    //BasePage.Driver.FindElement(By.CssSelector("#InternalMergeDefaultMode_img")).Click();

                    //Resetting the MergeExpirationLevel
                    BasePage.Driver.FindElement(By.CssSelector("input[id='MergeExpirationInterval_txt']")).Clear();
                    BasePage.Driver.FindElement(By.CssSelector("input[id='MergeExpirationInterval_txt']")).SendKeys(MergeExpirationLevel_snapshot);
                    Logger.Instance.InfoLog("MergeExpirationLevel_snapshot updated to--" + MergeExpirationLevel_snapshot);

                    //Save the transaction
                    configure.ClickSubmitChangesBtn();
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("img#PatientComparisonStrategyClass_statusimg")));


                    //logout
                    BasePage.Driver.SwitchTo().DefaultContent();
                    hplogin.LogoutHPen();

                    //Revert back to the same driver type or logout
                    if (changebrowser == 1)
                    {
                        BasePage.Driver.Quit();
                        Config.BrowserType = browserName;
                        Logger.Instance.InfoLog("Swicthing Back Browser to --" + browserName);
                        BasePage.Driver = null;
                        new HPLogin();
                    }

                    Putty putty = new Putty();
                    putty.RestartService();
                }
                catch (Exception)
                {

                    int changebrowser = 0;

                    String browserName = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
                    if (!(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer")))
                    {
                        BasePage.Driver.Quit();
                        Config.BrowserType = "internet explorer";
                        Logger.Instance.InfoLog("Swicthing Browser Type to IE");
                        BasePage.Driver = null;
                        hplogin = new HPLogin();
                        BasePage.Driver.Navigate().GoToUrl(hplogin.hpurl);
                        HPHomePage homepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                        PageLoadWait.WaitForHPPageLoad(20);
                        configure = (Configure)homepage.Navigate("Configure");
                        configure.NavigateToTab("properties");
                        changebrowser++;
                    }
                    else
                    {
                        hplogin = new HPLogin();
                        BasePage.Driver.Navigate().GoToUrl(hplogin.hpurl);
                        HPHomePage homepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                        PageLoadWait.WaitForHPPageLoad(20);
                        configure = (Configure)homepage.Navigate("Configure");
                        configure.NavigateToTab("properties");
                    }
                    PageLoadWait.WaitForHPPageLoad(20);
                    configure.NavigateToPropertySubTab("Database");
                    PageLoadWait.WaitForHPPageLoad(20);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("PropertiesContext");

                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#PatientComparisonStrategyClass_txt")));

                    //Update Snapshot Patient Comparison Strategy Class
                    String PCS_snapshot = BasePage.Driver.FindElement(By.CssSelector("input[id='PatientComparisonStrategyClass_txt']")).GetAttribute("value");
                    if (PCS_snapshot != PatientComparisonStrategyClassid_snapshot)
                    {
                        BasePage.Driver.FindElement(By.CssSelector("input[id='PatientComparisonStrategyClass_txt']")).Clear();
                        Logger.Instance.InfoLog("Patient Comparison Strategy Class updated in finally block " + PatientComparisonStrategyClassid_snapshot);
                        BasePage.Driver.FindElement(By.CssSelector("input[id='PatientComparisonStrategyClass_txt']")).SendKeys(PatientComparisonStrategyClassid_snapshot);
                        configure.ClickSubmitChangesBtn();
                        BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("img#PatientComparisonStrategyClass_statusimg")));
                    }
                    //delete Property Database|InternalMergeDefaultMode;;PCS_ONLY
                    //BasePage.Driver.FindElement(By.CssSelector("#InternalMergeDefaultMode_img")).Click();

                    //Resetting the MergeExpirationLevel
                    BasePage.Driver.FindElement(By.CssSelector("input[id='MergeExpirationInterval_txt']")).Clear();
                    BasePage.Driver.FindElement(By.CssSelector("input[id='MergeExpirationInterval_txt']")).SendKeys(MergeExpirationLevel_snapshot);
                    Logger.Instance.InfoLog("MergeExpirationLevel_snapshot updated to--" + MergeExpirationLevel_snapshot);

                    //Save the transaction
                    configure.ClickSubmitChangesBtn();
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("img#PatientComparisonStrategyClass_statusimg")));



                    //logout
                    BasePage.Driver.SwitchTo().DefaultContent();
                    hplogin.LogoutHPen();

                    //Revert back to the same driver type or logout
                    if (changebrowser == 1)
                    {
                        BasePage.Driver.Quit();
                        Config.BrowserType = browserName;
                        Logger.Instance.InfoLog("Swicthing Back Browser to --" + browserName);
                        BasePage.Driver = null;
                        new HPLogin();
                    }

                    Putty putty = new Putty();
                    putty.RestartService();
                }
            }
        }


    }
}
