    using System;
    using System.Drawing;
    using System.Threading;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Selenium.Scripts.Pages;
    using Selenium.Scripts.Reusable.Generic;
    using System.IO;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Interactions;
    using OpenQA.Selenium.Internal;
    using OpenQA.Selenium.Support.UI;
    using Selenium.Scripts.Pages.HoldingPen;
    using Selenium.Scripts.Pages.MPAC;
    using Selenium.Scripts.Pages.Putty;
    using System.Globalization;
    using Selenium.Scripts.Pages.iConnect;

    namespace Selenium.Scripts.Tests
    {
        class Reconciliation
        {
            public Login login { get; set; }
            public MpacLogin mpaclogin { get; set; }
            public HPLogin hplogin { get; set; }
            public Configure configure { get; set; }
            public HPHomePage hphomepage { get; set; }
            public ExamImporter ei { get; set; }
            public string filepath { get; set; }
            const String DefaultQueryIDTag = "00100020,00080050";

        public Reconciliation(String classname)
            {
                login = new Login();
                login.DriverGoTo(login.url);
                mpaclogin = new MpacLogin();
                hplogin = new HPLogin();
                configure = new Configure();
                hphomepage = new HPHomePage();
                ei = new ExamImporter();
                filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            }

        #region Sprint-3 Test Cases

        /// <summary>
        /// Manual Reconciliation - Automatic Reconciliation Fails Due To Multiple Existing Orders
        /// </summary>
        public TestCaseResult Test_29489(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables          
            Inbounds inbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            String DefaultTag = "";

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String username = Config.ph1UserName;
                String password = Config.ph1Password;
                String username1 = Config.ar1UserName;
                String password1 = Config.ar1Password;
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                String pacusername = Config.pacsadmin;
                String pacpassword = Config.pacspassword;
                String PID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String StudyPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String OrderPathList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderFilePath");
                String[] OrderPaths = OrderPathList.Split('=');
                String NominateReason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NominationReason");
                String OrderAccList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderAccession");
                String[] OrderAccNos = OrderAccList.Split(':');
                String QueryID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "QueryID");

                /***Login to EA weadmin and configure by removing existing details in queryIdTags and enter Query ID***/
                login.DriverGoTo(login.hpurl);

                //Login Holding Pen - step 1
                hphomepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                configure = (Configure)hphomepage.Navigate("Configure");
                ExecutedSteps++;

                //Navigate to Properties browser tab
                configure.NavigateToTab("properties");

                //Update QueryID tag - step 2
                DefaultTag = configure.UpdateQueryIDTags("removealladd", QueryID);
                ExecutedSteps++;

                //Restart Clarc Service - step 3
                Putty putty = new Putty();
                putty.RestartService();
                ExecutedSteps++;

                //Send  mutiple HL7 orders - step 4
                ExecutedSteps++;
                try
                {
                    foreach (String OrderPath in OrderPaths)
                    {
                        Boolean hl7order = mpaclogin.SendHL7Order(Config.MergePACsIP, int.Parse(Config.mpacport), OrderPath);
                        if (hl7order == true)
                        {
                            result.steps[ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            result.steps[ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                            throw new Exception("one of the HL7 order not sent");
                        }
                    }
                }
                catch (Exception e)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("one of the HL7 order not sent" + e, e);
                }

                //Step-5:Examine the order is routed in EA(HP)
                login.DriverGoTo(login.hpurl);
                hphomepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                WorkFlow workflow = (WorkFlow)hphomepage.Navigate("Workflow");

                ExecutedSteps++;

                Dictionary<string, string> OrderResults = null;
                try
                {
                    for (int i = 0; i < OrderAccNos.Length; i++)
                    {
                        //search study using acc no'
                        workflow.NavigateToLink("Workflow", "Queue Worklist");

                        //Check Order in Holding Pen
                        Boolean order = workflow.HPCheckOrder(OrderAccNos[i]);

                        if (order == true)
                        {
                            result.steps[ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            result.steps[ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                            throw new Exception("One of the HL7 orders not reached Holding pen");
                        }
                        if (i == 0)
                        {
                            //Get Order details 
                            OrderResults = workflow.GetOrderDetailsInHP();
                        }
                    }
                }
                catch (Exception e)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("One of the HL7 orders not reached Holding pen" + e, e);
                }


                //Logout in HP
                hplogin.LogoutHPen();

                //Step-6:Modify the study with description as Abdomen  and send such study to Merge PACs#2
                BasePage.RunBatchFile(Config.batchfilepath, StudyPath + " " + Config.dicomsendpath + " " + Config.StudyPacs);
                ExecutedSteps++;

                //Send the study to dicom devices from MergePacs management page - step 7
                //login to PACS#1 as admin
                mpaclogin.DriverGoTo(mpaclogin.mpacstudyurl);
                MPHomePage mpachome = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);

                //Send Study with matching HL7 order
                Tool tool = (Tool)mpachome.NavigateTopMenu("Tools");
                tool.NavigateToSendStudy();
                tool.SearchStudy("Accession", Accession, 0);

                //Get study details
                Dictionary<string, string> MpacDetails = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());

                tool.MpacSelectStudy("Accession", Accession);
                tool.SendStudy(1);
                mpaclogin.LogoutPacs();
                ExecutedSteps++;

                //To wait Until study reaches Holding pen
                //Login as physician(Destination-1)
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                PageLoadWait.WaitforReceivingStudy(180, PID);
                PageLoadWait.WaitforUpload(Accession, inbounds);
                //Search study
                inbounds.SearchStudy("Accession", Accession);

                //Validate studi is listed in ph inbounds 
                if (inbounds.CheckStudy("Accession", Accession) == true)
                {
                    Logger.Instance.InfoLog("Study with Accession " + Accession + " found in iCA.");
                }
                else
                {
                    Logger.Instance.InfoLog("Study with Accession " + Accession + " not found in iCA.");
                }

                //Logout
                login.Logout();


                //Search for such study in HP exists without any updation
                //Login in HP
                login.DriverGoTo(login.hpurl);
                HPHomePage hphomepage1 = hplogin.LoginHPen(hpUserName, hpPassword);
                WorkFlow workflow1 = (WorkFlow)hphomepage1.Navigate("Workflow");

                //search study using acc no' - step 8
                workflow1.NavigateToLink("Workflow", "Archive Search");
                workflow1.HPSearchStudy("Accessionno", Accession);
                ExecutedSteps++;

                Dictionary<string, string> StudyDetails = workflow1.GetStudyDetailsInHP();

                string mpacdate = DateTime.ParseExact(MpacDetails["StudyDate"], "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString();
                string studydate = DateTime.ParseExact(StudyDetails["Study Date"], "MM/dd/yyyy", CultureInfo.InvariantCulture).ToShortDateString();

                //Validate study details are in sync with details when uploaded - step 9
                if ((MpacDetails["PatientName"].ToUpper().Contains(StudyDetails["Patient Name"].Split(',')[0].ToUpper())) && (StudyDetails["Accession Number"].Equals(MpacDetails["Accession"])) &&
                    ((StudyDetails["Study Description"].Replace(" ", "")).Equals((MpacDetails["StudyDescription"].Replace(" ", ""))))
                    && mpacdate.Equals(studydate))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout in HP
                hplogin.LogoutHPen();

                //Non Automated Steps - step 10 & 11 --Checking Reconcilation status and log file using putty
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Login as physician(Destination-1)
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Navigate to Inbounds - step 12
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;

                //Search study - step 13
                inbounds.SearchStudy(StudyDetails["Patient Name"].Split(',')[0].Trim(), StudyDetails["Patient Name"].Split(',')[1].Trim(), "", "", "", "", "", "");

                if (inbounds.CheckStudy("Accession", Accession) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select study
                inbounds.SelectStudy("Accession", Accession);

                //Validate status as uploaded and Reconciliation State as Matched to Order - step 14
                String studyState1;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Reconciliation State", out studyState1);

                if (studyState1.Equals("Multiple Matching Orders"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                String studyStatus2;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyStatus2);

                //Validate study status as Uploaded - step 15
                if (studyStatus2 == "Uploaded")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                IWebElement nominate1 = BasePage.Driver.FindElement(By.CssSelector("#m_nominateStudyButton"));

                //Valiadate Nominate for archive button is enabled - step 16
                if (nominate1.Enabled == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout physician(Destination-1)
                login.Logout();

                //login to PACS#3(Destination PACS) as admin
                mpaclogin.DriverGoTo(mpaclogin.mpacdesturl);
                MPHomePage mpachome1 = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);

                //Send Study with matching HL7 order
                Tool tool1 = (Tool)mpachome1.NavigateTopMenu("Tools");
                tool1.NavigateToSendStudy();
                tool1.SearchStudy("Accession", Accession, 0);

                //Get study details
                Dictionary<string, string> study = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());

                //Validate study nor reached to destination -step 17
                if (study == null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout from Dest PACS
                mpaclogin.LogoutPacs();

                //Login as physician(Destination-1)
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Navigate to Inbounds - 
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study - 
                inbounds.SearchStudy(StudyDetails["Patient Name"].Split(',')[0].Trim(), StudyDetails["Patient Name"].Split(',')[1].Trim(), "", "", "", "", "", "");

                //Select study
                inbounds.SelectStudy("Accession", Accession);

                //Click Nominate for archive button - step 18
                IWebElement OrderField, ReasonField;
                inbounds.ClickNominateButton(out ReasonField, out OrderField);

                if (BasePage.Driver.FindElement(By.CssSelector("#NominateStudyDialogDiv")).Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select reason field
                OrderField.SendKeys("Testing");
                SelectElement selector = new SelectElement(ReasonField);
                selector.SelectByText(NominateReason);
                Logger.Instance.InfoLog("Reason Field is filled Sucessfully");

                //Click Nominate in confirmation window -step 19
                inbounds.ClickConfirmNominate();
                ExecutedSteps++;

                //Find Study Status 
                String studyStatus6;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyStatus6);
                String statusReason;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status Reason", out statusReason);

                //Validate Study status as Nominated for archive - step 20
                if (studyStatus6 == "Nominated For Archive")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Validate Nominate reason - step 21
                if (statusReason == NominateReason)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout physician(Destination-1)
                login.Logout();

                //Login as archivist(Destination-1)
                login.LoginIConnect(username1, password1);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study
                inbounds.SearchStudy("Accession", Accession);

                //Validate Study is listed in archivist inbounds - step 22
                if (inbounds.CheckStudy("Accession", Accession) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Find Study Status and Status Reason
                String studyStatus4;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyStatus4);

                //Validate Study status as Nominated for archive - step 23
                if (studyStatus4 == "Nominated For Archive")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Remove all columns
                inbounds.RemoveAllColumns();

                //Choose columns
                inbounds.ChooseColumns(new String[]{"Last Name", "First Name", "Gender", "Patient DOB", "Issuer of PID",
                    "Patient ID", "Description", "Study Date", "Accession"});

                //Details of a study
                Dictionary<string, string> rowValues = inbounds.GetMatchingRow("Accession", Accession);

                //Select study
                inbounds.SelectStudy1("Accession", Accession);

                //Archive the study by order and All Dates,Edit last name,study description and apply 
                inbounds.ClickArchiveStudy("", "");

                //Validate Archive/Reconcile Study dialog is opened - step 24
                if (BasePage.Driver.FindElement(By.CssSelector("#ReconciliationControlDialogDiv")).Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Search order - step 
                inbounds.ArchiveSearch("order", "All Dates");

                //Click Show all
                inbounds.ShowAllInReconcile();

                ExecutedSteps++;
                //Validate all matching Orders are listed - step 25
                try
                {
                    foreach (String OrderAcc in OrderAccNos)
                    {
                        Dictionary<string, string> Order = inbounds.GetMatchingRowReconcile("Accession", OrderAcc);

                        if (Order != null)
                        {
                            result.steps[ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            result.steps[ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                            throw new Exception("Order Not found");
                        }
                    }
                }
                catch (Exception e)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Order Not found" + e);
                }

                //Select Order 
                inbounds.SelectStudyFromReconcile("Accession", OrderAccNos[0]);

                //Click Ok in show all window
                inbounds.ClickOkInShowAll();

                //Details in Original details column
                Dictionary<String, String> OriginalDetails = inbounds.GetDataInArchive("Original Details");

                //Validate the details in original details column are in sync with study details - step 26
                if ((OriginalDetails["Last Name"].Equals(rowValues["Last Name"])) && (OriginalDetails["First Name"].Equals(rowValues["First Name"])) &&
                    (OriginalDetails["Gender"].Equals(rowValues["Gender"])) && (OriginalDetails["DOB"].Equals(rowValues["Patient DOB"])) &&
                    (OriginalDetails["Issuer of PID"].Equals(rowValues["Issuer of PID"])) && (OriginalDetails["PID / MRN"].Equals(rowValues["Patient ID"])) &&
                    (OriginalDetails["Description"].Equals(rowValues["Description"])) && (OriginalDetails["Study Date"].Equals(rowValues["Study Date"])) &&
                    (OriginalDetails["Accession"].Equals(rowValues["Accession"])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Validate Matching order details are listed in 'Matching order column' related to studies' last name - step 27
                Dictionary<String, String> MatchingValues = inbounds.GetDataInArchive("Matching Order");

                //Validate the details listed in Matching order column are in sync with order details
                if ((MatchingValues["Last Name"].Equals(OrderResults["Patient Name"].Split(',')[0].Trim())) && (MatchingValues["First Name"].Equals(OrderResults["Patient Name"].Split(',')[1].Trim())) &&
                    (MatchingValues["Gender"].Equals(OrderResults["Patient's Sex"])) && inbounds.CompareDates(MatchingValues["DOB"], OrderResults["Patient's Birth Date"]) == true &&
                    (MatchingValues["Issuer of PID"].Equals(OrderResults["IssuerOfPatientID"])) && (MatchingValues["PID / MRN"].Equals(OrderResults["Patient ID"])) &&
                    (MatchingValues["Description"].Equals(OrderResults["Requested Procedure Description"])) && inbounds.CompareDates(MatchingValues["Study Date"], OrderResults["Scheduled Start Date"]) == true &&
                    (MatchingValues["Accession"].Equals(OrderResults["Accession Number"])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Details in Final details column 
                Dictionary<String, String> FinalDetails = inbounds.GetDataInArchive("Final Details");

                //Validate order details are automatically updated in Final Details column - step 28
                result.steps[++ExecutedSteps].status = "Not Automated";

                /*            if ((MatchingValues["Last Name"].Equals(FinalDetails["Last Name"])) || (MatchingValues["First Name"].Equals(FinalDetails["First Name"])) ||
                                (MatchingValues["Gender"].Equals(FinalDetails["Gender"])) || (MatchingValues["DOB"].Equals(FinalDetails["DOB"])) ||
                                (MatchingValues["Issuer of PID"].Equals(FinalDetails["Issuer of PID"])) || (MatchingValues["PID / MRN"].Equals(FinalDetails["PID / MRN"])) ||
                                (MatchingValues["Description"].Equals(FinalDetails["Description"])) || (MatchingValues["Study Date"].Equals(FinalDetails["Study Date"])) ||
                                (MatchingValues["Accession"].Equals(FinalDetails["Accession"])) &&
                                (OriginalDetails["Prefix"].Equals(FinalDetails["Prefix"])))
                            {
                                result.steps[++ExecutedSteps].status = "Pass";
                                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                            }
                            else
                            {
                                result.steps[++ExecutedSteps].status = "Fail";
                                Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                                result.steps[ExecutedSteps].SetLogs();
                            }
                            */
                //Check  in Original details column after selecting checkbox,validate and archive
                inbounds.SetBlankFinalDetailsInArchive();

                //Details in Original details column - step 29
                Dictionary<String, String> FinalDetails1 = inbounds.GetDataInArchive("Final Details");

                if (FinalDetails1["PID / MRN"].Equals(OriginalDetails["PID / MRN"]) && FinalDetails1["Accession"].Equals(OriginalDetails["Accession"]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Confirm archive in Reconcilition window - step 30
                inbounds.ClickArchive();
                ExecutedSteps++;

                //Non Automated step 31 -- Validate status as "Routing Started"
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Search study
                inbounds.SearchStudy("Accession", Accession);
                inbounds.SearchStudy("Accession", Accession);

                //Reset Columns
                inbounds.ResetColumns();

                //Validate status is changed to Routing Completed - Step 32
                String studyStatus1;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyStatus1);

                if (studyStatus1 == "Routing Completed")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout archivist
                login.Logout();

                //Login to Merge PACs#3 to check for archived properly after  reconciliation   

                //login to PACS#3 as admin
                mpaclogin.DriverGoTo(mpaclogin.mpacdesturl);
                MPHomePage mpachome2 = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);

                //Send Study with matching HL7 order
                Tool tool2 = (Tool)mpachome1.NavigateTopMenu("Tools");
                tool2.NavigateToSendStudy();
                tool2.SearchStudy("Accession", Accession, 0);

                //Get study details
                Dictionary<string, string> MpacResults = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());

                String mpacdate1 = DateTime.ParseExact(MpacDetails["StudyDate"], "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString();
                String studydate1 = DateTime.ParseExact(FinalDetails1["Study Date"], "dd-MMM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture).ToShortDateString();

                //Validate Details in Final details column on archive window should match with details in Dest PACS 33
                if ((MpacResults["PatientName"].Split(' ')[0].ToUpper().Equals(FinalDetails1["Last Name"].ToUpper())) && (MpacResults["PatientName"].Split(' ')[1].Equals(FinalDetails1["First Name"])) &&
                    (MpacResults["Sex"].Equals(FinalDetails1["Gender"])) && (inbounds.CompareDates(MpacResults["DOB"], FinalDetails1["DOB"], "yyyyMMdd", "dd-MMM-yyyy")) &&
                    (MpacResults["IPID"].Equals(FinalDetails1["Issuer of PID"])) && (MpacResults["PatientID"].Equals(FinalDetails1["PID / MRN"])) &&
                    ((MpacResults["StudyDescription"].Replace(" ", "")).Equals(FinalDetails1["Description"].Replace(" ", ""))) && mpacdate1.Equals(studydate1) &&
                    (MpacResults["Accession"].Equals(FinalDetails1["Accession"])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout from Dest PACS
                mpaclogin.LogoutPacs();

                ////**************Reset the QuerID tag changed*********************
                //login.DriverGoTo(login.hpurl);
                //hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                //configure = (Configure)hphomepage.Navigate("Configure");

                ////Navigate to Properties browser tab
                //configure.NavigateToTab("properties");

                ////Update QueryID tag with default tag
                //configure.UpdateQueryIDTags("removealladd", DefaultTag);

                ////Restart Clarc service
                //putty.RestartService();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                ////**************Reset the QuerID tag changed*********************
                //login.DriverGoTo(login.hpurl);
                //hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                //configure = (Configure)hphomepage.Navigate("Configure");

                ////Navigate to Properties browser tab
                //configure.NavigateToTab("properties");

                ////Update QueryID tag with default tag
                //configure.UpdateQueryIDTags("removealladd", DefaultTag);

                ////Restart Clarc Service - step 3
                //Putty putty = new Putty();
                //putty.RestartService();

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }


        }

        /// <summary>
        /// Manual Reconciliation Via Upload Tool- Automatic Reconciliation Fails Due To Multiple Existing Orders
        /// </summary>
        public TestCaseResult Test_29490(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables          
            Inbounds inbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            String DefaultTag = "";

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String username = Config.ph1UserName;
                String password = Config.ph1Password;
                String username1 = Config.ar1UserName;
                String password1 = Config.ar1Password;
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                String pacusername = Config.pacsadmin;
                String pacpassword = Config.pacspassword;
                String PID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String StudyPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String OrderPathList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderFilePath");
                String[] OrderPaths = OrderPathList.Split('=');
                String NominateReason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NominationReason");
                String OrderAccList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderAccession");
                String[] OrderAccNos = OrderAccList.Split(':');
                String QueryID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "QueryID");

                //Login to EA weadmin and configure by removing existing details in queryIdTags and enter Query ID***/
                login.DriverGoTo(login.hpurl);

                //Login Holding Pen - step 1
                hphomepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                configure = (Configure)hphomepage.Navigate("Configure");
                ExecutedSteps++;

                //Navigate to Properties browser tab
                configure.NavigateToTab("properties");

                //Update QueryID tag  - step 2
                DefaultTag = configure.UpdateQueryIDTags("removealladd", QueryID);
                ExecutedSteps++;

                //Restart Clarc Service - step 3
                Putty putty = new Putty();
                putty.RestartService();
                ExecutedSteps++;

                //Send  mutiple HL7 orders - step 4
                ExecutedSteps++;
                try
                {
                    foreach (String OrderPath in OrderPaths)
                    {
                        Boolean hl7order = mpaclogin.SendHL7Order(Config.MergePACsIP, int.Parse(Config.mpacport), OrderPath);
                        if (hl7order == true)
                        {
                            result.steps[ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            result.steps[ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                            throw new Exception("one of the HL7 order not sent");
                        }
                    }
                }
                catch (Exception e)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("one of the HL7 order not sent" + e, e);
                }

                //Step-5:Examine the order is routed in EA(HP)
                login.DriverGoTo(login.hpurl);
                hphomepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                WorkFlow workflow = (WorkFlow)hphomepage.Navigate("Workflow");

                ExecutedSteps++;

                Dictionary<string, string> OrderResults = null;
                try
                {
                    for (int i = 0; i < OrderAccNos.Length; i++)
                    {
                        //search study using acc no'
                        workflow.NavigateToLink("Workflow", "Queue Worklist");

                        //Check Order in Holding Pen
                        Boolean order = workflow.HPCheckOrder(OrderAccNos[i]);

                        if (order == true)
                        {
                            result.steps[ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            result.steps[ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                            throw new Exception("One of the HL7 orders not reached Holding pen");
                        }
                        if (i == 0)
                        {
                            //Get Order details 
                            OrderResults = workflow.GetOrderDetailsInHP();
                        }
                    }
                }
                catch (Exception e)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("One of the HL7 orders not reached Holding pen" + e, e);
                }

                //Logout in HP
                hplogin.LogoutHPen();

                // Launch Uploader Tool
                ExamImporter ei = new ExamImporter();
                ei.LaunchEI();

                // Login as user - step 6
                ei.LoginToEi(Config.stUserName, Config.stPassword);
                ExecutedSteps++;

                //Select Destination - step 7
                ei.EI_SelectDestination(Config.Dest1);
                ExecutedSteps++;

                //Select Dicom path location - step 8
                ei.SelectFileFromHdd(StudyPath);
                ExecutedSteps++;

                //Check Select all Patients - step 9
                ei.SelectAllPatientsToUpload();
                ExecutedSteps++;

                //Clicks Send and upload the studies - step 10
                ei.Send();
                ExecutedSteps++;

                //Logout 
                ei.EI_Logout();

                //Closes the tool
                ei.CloseUploaderTool();

                //Search for such study in HP exists without any updation
                //Login in HP
                login.DriverGoTo(login.hpurl);
                HPHomePage hphomepage1 = hplogin.LoginHPen(hpUserName, hpPassword);
                WorkFlow workflow1 = (WorkFlow)hphomepage1.Navigate("Workflow");

                //search study using acc no' - step 11
                workflow1.NavigateToLink("Workflow", "Archive Search");
                workflow1.HPSearchStudy("Accessionno", Accession);
                ExecutedSteps++;

                //Validate Uploaded study is present - step 12
                if (workflow1.HPCheckStudy(Accession) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout in HP
                hplogin.LogoutHPen();

                //Non Automated Steps - step 13 & 14 --Checking Reconcilation status and log file using putty
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Login as physician(Destination-1)
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Navigate to Inbounds - step 15
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;

                //Search study - step 16
                inbounds.SearchStudy("Accession", Accession);

                if (inbounds.CheckStudy("Accession", Accession) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select study
                inbounds.SelectStudy("Accession", Accession);

                //Validate status as uploaded and Reconciliation State as Matched to Order - step 17
                String studyState1;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Reconciliation State", out studyState1);

                if (studyState1 == "Multiple Matching Orders")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                String studyStatus2;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyStatus2);

                //Validate study status as Uploaded - step 18
                if (studyStatus2 == "Uploaded")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                IWebElement nominate1 = BasePage.Driver.FindElement(By.CssSelector("#m_nominateStudyButton"));

                //Valiadate Nominate for archive button is enabled - step 19
                if (nominate1.Enabled == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout physician(Destination-1)
                login.Logout();

                //login to PACS#3(Destination PACS) as admin
                mpaclogin.DriverGoTo(mpaclogin.mpacdesturl);
                MPHomePage mpachome1 = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);

                //Send Study with matching HL7 order
                Tool tool1 = (Tool)mpachome1.NavigateTopMenu("Tools");
                tool1.NavigateToSendStudy();
                tool1.SearchStudy("Accession", Accession, 0);

                //Get study details
                Dictionary<string, string> study = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());

                //Validate Details in Final details column on archive window should match with details in Dest PACS -step 20
                if (study == null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout from Dest PACS
                mpaclogin.LogoutPacs();

                //Login as physician(Destination-1)
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Navigate to Inbounds - 
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study - 
                inbounds.SearchStudy("Accession", Accession);

                //Select study
                inbounds.SelectStudy("Accession", Accession);

                //Click Nominate for archive button - step 21
                IWebElement OrderField, ReasonField;
                inbounds.ClickNominateButton(out ReasonField, out OrderField);

                if (BasePage.Driver.FindElement(By.CssSelector("#NominateStudyDialogDiv")).Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select reason field
                OrderField.SendKeys("Testing");
                SelectElement selector = new SelectElement(ReasonField);
                selector.SelectByText(NominateReason);
                Logger.Instance.InfoLog("Reason Field is filled Sucessfully");

                //Click Nominate in confirmation window -step 22
                inbounds.ClickConfirmNominate();
                ExecutedSteps++;

                //Find Study Status 
                String studyStatus6;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyStatus6);
                String statusReason;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status Reason", out statusReason);

                //Validate Study status as Nominated for archive - step 23
                if (studyStatus6 == "Nominated For Archive")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Validate Nominate reason - step 24
                if (statusReason == NominateReason)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout physician(Destination-1)
                login.Logout();

                //Login as archivist(Destination-1)
                login.LoginIConnect(username1, password1);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study
                inbounds.SearchStudy("Accession", Accession);

                //Validate Study is listed in archivist inbounds - step 25
                if (inbounds.CheckStudy("Accession", Accession) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Find Study Status and Status Reason
                String studyStatus4;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyStatus4);

                //Validate Study status as Nominated for archive - step 26
                if (studyStatus4 == "Nominated For Archive")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Remove all columns
                inbounds.RemoveAllColumns();

                //Choose columns
                inbounds.ChooseColumns(new String[]{"Last Name", "First Name", "Gender", "Patient DOB", "Issuer of PID",
                    "Patient ID", "Description", "Study Date", "Accession"});

                //Details of a study
                Dictionary<string, string> rowValues = inbounds.GetMatchingRow("Accession", Accession);

                //Select study
                inbounds.SelectStudy1("Accession", Accession);

                //Archive the study by order and All Dates,Edit last name,study description and apply 
                inbounds.ClickArchiveStudy("", "");

                //Validate Archive/Reconcile Study dialog is opened - step 27
                if (BasePage.Driver.FindElement(By.CssSelector("#ReconciliationControlDialogDiv")).Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Search order - step 
                inbounds.ArchiveSearch("order", "All Dates");

                //Click Show all
                inbounds.ShowAllInReconcile();

                ExecutedSteps++;
                //Validate all matching Orders are listed - step 28
                try
                {
                    foreach (String OrderAcc in OrderAccNos)
                    {
                        Dictionary<string, string> Order = inbounds.GetMatchingRowReconcile("Accession", OrderAcc);

                        if (Order != null)
                        {
                            result.steps[ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            result.steps[ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                            throw new Exception("Order Not found");
                        }
                    }
                }
                catch (Exception e)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Order Not found" + e, e);
                }

                //Select Order 
                inbounds.SelectStudyFromReconcile("Accession", OrderAccNos[0]);

                //Click Ok in show all window
                inbounds.ClickOkInShowAll();

                //Details in Original details column
                Dictionary<String, String> OriginalDetails = inbounds.GetDataInArchive("Original Details");

                //Validate the details in original details column are in sync with study details - step 29
                if ((OriginalDetails["Last Name"].Equals(rowValues["Last Name"])) && (OriginalDetails["First Name"].Equals(rowValues["First Name"])) &&
                    (OriginalDetails["Gender"].Equals(rowValues["Gender"])) && (OriginalDetails["DOB"].Equals(rowValues["Patient DOB"])) &&
                    (OriginalDetails["Issuer of PID"].Equals(rowValues["Issuer of PID"])) && (OriginalDetails["PID / MRN"].Equals(rowValues["Patient ID"])) &&
                    (OriginalDetails["Description"].Equals(rowValues["Description"])) && (OriginalDetails["Study Date"].Equals(rowValues["Study Date"])) &&
                    (OriginalDetails["Accession"].Equals(rowValues["Accession"])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Validate Matching order details are listed in 'Matching order column' related to studies' last name - step 30
                Dictionary<String, String> MatchingValues = inbounds.GetDataInArchive("Matching Order");

                //Validate the details listed in Matching order column are in sync with order details
                if ((MatchingValues["Last Name"].Equals(OrderResults["Patient Name"].Split(',')[0].Trim())) && (MatchingValues["First Name"].Equals(OrderResults["Patient Name"].Split(',')[1].Trim())) &&
                    (MatchingValues["Gender"].Equals(OrderResults["Patient's Sex"])) && inbounds.CompareDates(MatchingValues["DOB"], OrderResults["Patient's Birth Date"]) &&
                    (MatchingValues["Issuer of PID"].Equals(OrderResults["IssuerOfPatientID"])) && (MatchingValues["PID / MRN"].Equals(OrderResults["Patient ID"])) &&
                    (MatchingValues["Description"].Equals(OrderResults["Requested Procedure Description"])) && inbounds.CompareDates(MatchingValues["Study Date"], OrderResults["Scheduled Start Date"]) &&
                    (MatchingValues["Accession"].Equals(OrderResults["Accession Number"])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Details in Final details column 
                Dictionary<String, String> FinalDetails = inbounds.GetDataInArchive("Final Details");

                //Validate order details are automatically updated in Final Details column - step 31
                result.steps[++ExecutedSteps].status = "Not Automated";

                /*        if ((MatchingValues["Last Name"].Equals(FinalDetails["Last Name"])) || (MatchingValues["First Name"].Equals(FinalDetails["First Name"])) ||
                            (MatchingValues["Gender"].Equals(FinalDetails["Gender"])) || (MatchingValues["DOB"].Equals(FinalDetails["DOB"])) ||
                            (MatchingValues["Issuer of PID"].Equals(FinalDetails["Issuer of PID"])) || (MatchingValues["PID / MRN"].Equals(FinalDetails["PID / MRN"])) ||
                            (MatchingValues["Description"].Equals(FinalDetails["Description"])) || (MatchingValues["Study Date"].Equals(FinalDetails["Study Date"])) ||
                            (MatchingValues["Accession"].Equals(FinalDetails["Accession"])) &&
                            (OriginalDetails["Prefix"].Equals(FinalDetails["Prefix"])))
                        {
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            result.steps[++ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }
                        */

                //Check  in Original details column after selecting checkbox,validate and archive
                inbounds.SetBlankFinalDetailsInArchive();

                //Details in Original details column - step 32
                Dictionary<String, String> FinalDetails1 = inbounds.GetDataInArchive("Final Details");

                if (FinalDetails1["PID / MRN"].Equals(OriginalDetails["PID / MRN"]) && FinalDetails1["Accession"].Equals(OriginalDetails["Accession"]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Confirm archive in Reconcilition window - step 33
                inbounds.ClickArchive();
                ExecutedSteps++;

                //Non Automated step 34 -- Validate status as "Routing Started"
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Search study
                inbounds.SearchStudy("Accession", Accession);
                inbounds.SearchStudy("Accession", Accession);

                //Reset Columns
                inbounds.ResetColumns();

                //Validate status is changed to Routing Completed - Step 35
                String studyStatus1;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyStatus1);

                if (studyStatus1 == "Routing Completed")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout archivist
                login.Logout();

                //Login to Merge PACs#3 to check for archived properly after  reconciliation   

                //login to PACS#3 as admin
                mpaclogin.DriverGoTo(mpaclogin.mpacdesturl);
                MPHomePage mpachome2 = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);

                //Send Study with matching HL7 order
                Tool tool2 = (Tool)mpachome1.NavigateTopMenu("Tools");
                tool2.NavigateToSendStudy();
                tool2.SearchStudy("Accession", Accession, 0);

                //Get study details
                Dictionary<string, string> MpacResults = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());

                String mpacdate1 = DateTime.ParseExact(MpacResults["StudyDate"], "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString();
                String studydate1 = DateTime.ParseExact(FinalDetails1["Study Date"], "dd-MMM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture).ToShortDateString();

                //Validate Details in Final details column on archive window should match with details in Dest PACS 36
                if ((MpacResults["PatientName"].Split(' ')[0].ToUpper().Equals(FinalDetails1["Last Name"].ToUpper())) && (MpacResults["PatientName"].Split(' ')[1].Equals(FinalDetails1["First Name"])) &&
                    (MpacResults["Sex"].Equals(FinalDetails1["Gender"])) && (inbounds.CompareDates(MpacResults["DOB"], FinalDetails1["DOB"], "yyyyMMdd", "dd-MMM-yyyy")) &&
                    (MpacResults["IPID"].Equals(FinalDetails1["Issuer of PID"])) && (MpacResults["PatientID"].Equals(FinalDetails1["PID / MRN"])) &&
                    (MpacResults["StudyDescription"].Replace(" ", "").Equals(FinalDetails1["Description"].Replace(" ", ""))) && mpacdate1.Equals(studydate1) &&
                    (MpacResults["Accession"].Equals(FinalDetails1["Accession"])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout from Dest PACS
                mpaclogin.LogoutPacs();

                //**************Reset the QuerID tag changed*********************
                login.DriverGoTo(login.hpurl);
                hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                configure = (Configure)hphomepage.Navigate("Configure");

                //Navigate to Properties browser tab
                configure.NavigateToTab("properties");

                //Update QueryID tag with default tag
                configure.UpdateQueryIDTags("removealladd", DefaultQueryIDTag);

                //Restart Clarc service
                putty.RestartService();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //**************Reset the QuerID tag changed*********************
                login.DriverGoTo(login.hpurl);
                hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                configure = (Configure)hphomepage.Navigate("Configure");

                //Navigate to Properties browser tab
                configure.NavigateToTab("properties");

                //Update QueryID tag with default tag
                configure.UpdateQueryIDTags("removealladd", DefaultQueryIDTag);

                //Restart Clarc Service - step 3
                Putty putty = new Putty();
                putty.RestartService();

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Study Deletion From EA server [Holding Pen] without nomination
        /// </summary>
        public TestCaseResult Test1_29491(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables          
            Inbounds inbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;            

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {

                //Fetch required Test data
                String username = Config.ph1UserName;
                String password = Config.ph1Password;
                String username1 = Config.ar1UserName;
                String password1 = Config.ar1Password;
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                String pacusername = Config.pacsadmin;
                String pacpassword = Config.pacspassword;
                String PID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String StudyPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String NominateReason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NominationReason");
                String QueryID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "QueryID");

                /***Login to EA weadmin and configure by removing existing details in queryIdTags and enter Query ID***/
                login.DriverGoTo(login.hpurl);

                //Login Holding Pen - step 1
                hphomepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                configure = (Configure)hphomepage.Navigate("Configure");
                ExecutedSteps++;

                //Navigate to Properties browser tab
                configure.NavigateToTab("properties");

                //Update QueryID tag  - step 2
                configure.UpdateQueryIDTags("removealladd", QueryID);
                ExecutedSteps++;

                //Restart Clarc Service - step 3
                Putty putty = new Putty();
                putty.RestartService();
                ExecutedSteps++;

                //Step-4:Modify the study with description as Abdomen  and send such study to Merge PACs#2
                BasePage.RunBatchFile(Config.batchfilepath, StudyPath + " " + Config.dicomsendpath + " " + Config.StudyPacs);
                ExecutedSteps++;

                //Send the study to dicom devices from MergePacs management page - step 5
                //login to PACS#1 as admin
                mpaclogin.DriverGoTo(mpaclogin.mpacstudyurl);
                MPHomePage mpachome = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);

                //Send Study with matching HL7 order
                Tool tool = (Tool)mpachome.NavigateTopMenu("Tools");
                tool.NavigateToSendStudy();
                tool.SearchStudy("Accession", Accession, 0);

                //Get study details
                Dictionary<string, string> MpacDetails = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());

                tool.MpacSelectStudy("Accession", Accession);
                tool.SendStudy(1);
                mpaclogin.LogoutPacs();
                ExecutedSteps++;

                //To wait Until study reaches Holding pen
                //Login as physician(Destination-1)
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                PageLoadWait.WaitforReceivingStudy(180, PID);
                PageLoadWait.WaitforUpload(Accession, inbounds);
                //Search study
                inbounds.SearchStudy("Accession", Accession);

                //Validate study is listed in ph inbounds 
                if (inbounds.CheckStudy("Accession", Accession) == true)
                {
                    Logger.Instance.InfoLog("Study with Accession " + Accession + " found in iCA.");
                }
                else
                {
                    Logger.Instance.InfoLog("Study with Accession " + Accession + " not found in iCA.");
                }

                //Logout
                login.Logout();
                
                //Search for such study in HP exists without any updation
                //Login in HP
                login.DriverGoTo(login.hpurl);
                HPHomePage hphomepage1 = hplogin.LoginHPen(hpUserName, hpPassword);
                WorkFlow workflow1 = (WorkFlow)hphomepage1.Navigate("Workflow");

                //Naviagate to archive search tab - step 6
                workflow1.NavigateToLink("Workflow", "Archive Search");
                ExecutedSteps++;

                //search study using acc no'
                workflow1.HPSearchStudy("Accessionno", Accession);
                
                Dictionary<string, string> StudyDetails = workflow1.GetStudyDetailsInHP();

                //Get Study date
                String date = DateTime.ParseExact(MpacDetails["StudyDate"], "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString();
                String date1 = DateTime.ParseExact(StudyDetails["Study Date"], "MM/dd/yyyy", CultureInfo.InvariantCulture).ToShortDateString();
                
                //Validate study details are in sync with details when uploaded - step 7
                if (StudyDetails["Patient Name"].Split(',')[0].Trim().Equals(MpacDetails["PatientName"].Split(' ')[0].Trim()) &&
                    StudyDetails["Patient Name"].Split(',')[1].Trim().Equals(MpacDetails["PatientName"].Split(' ')[1].Trim()) &&
                    StudyDetails["Accession Number"].Equals(MpacDetails["Accession"]) && date.Equals(date1) &&
                    StudyDetails["Study Description"].Equals(MpacDetails["StudyDescription"]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout in HP
                hplogin.LogoutHPen();

                //Non Automated Steps - step 8 & 9 --Checking Reconcilation status and log file using putty
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Login as physician(Destination-1)
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Navigate to Inbounds - step 10
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;

                //Search study 
                inbounds.SearchStudy(StudyDetails["Patient Name"].Split(',')[0].Trim(), StudyDetails["Patient Name"].Split(',')[1].Trim(), "", "", "", "", "", "");

                //Validate study is listed in ph's inbounds - step 11
                if (inbounds.CheckStudy("Accession", Accession) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select study
                inbounds.SelectStudy("Accession", Accession);

                //Validate status as uploaded and Reconciliation State as Matched to Order
                String studyState1;

                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Reconciliation State", out studyState1);

                //Validate reconiliation state as "No Matching order" - step 12
                if (studyState1 == "No Matching Order")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                String studyStatus2;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyStatus2);

                //Validate study status as Uploaded - step 13
                if (studyStatus2 == "Uploaded")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                IWebElement nominate1 = BasePage.Driver.FindElement(By.CssSelector("input#m_nominateStudyButton"));

                //Valiadate Nominate for archive button is enabled - step 14
                if (nominate1.Enabled == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout physician(Destination-1)
                login.Logout();

                //Delete the above study in EA
                //Login in HP
                login.DriverGoTo(login.hpurl);
                HPLogin hplogin1 = new HPLogin();
                hphomepage = hplogin.LoginHPen(hpUserName, hpPassword);
                WorkFlow workflow = (WorkFlow)hphomepage.Navigate("Workflow");

                //search study using acc no'
                workflow1.NavigateToLink("Workflow", "Archive Search");
                workflow1.HPSearchStudy("Accessionno", Accession);

                //Delete study in holding pen - step 15
                workflow1.HPDeleteStudy();
                ExecutedSteps++;

                //Logout in HP
                hplogin1.LogoutHPen();

                //Login as physician(Destination-1)
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Navigate to Inbounds - step 16
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;

                //Search study and Select study
                inbounds.SearchStudy("Accession", Accession);
                inbounds.SelectStudy("Accession", Accession);

                //Validate the status of the study as Deleted - step 17
                String studyStatus3;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyStatus3);

                if (studyStatus3.Equals("Deleted"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Validate nominate and delete buttons - step 18
                IWebElement nominate2 = BasePage.Driver.FindElement(By.CssSelector("input#m_nominateStudyButton"));
                IWebElement del = BasePage.Driver.FindElement(By.CssSelector("input#m_deleteStudiesButton"));
                
                if (nominate2.Enabled == false && del.Enabled == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as physician(Destination-1)
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
             
                //Return Result
                return result;

            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog("Exception in Test Method--"+e.Message+Environment.NewLine+e.StackTrace+Environment.NewLine+e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Logout of application
                login.Logout();
                
                //Return Result
                return result;                
            }
        }

        /// <summary>
        /// Study Deletion From EA server [Holding Pen] after nominating
        /// </summary>
        public TestCaseResult Test2_29491(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables          
            Inbounds inbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String username = Config.ph1UserName;
                String password = Config.ph1Password;
                String username1 = Config.ar1UserName;
                String password1 = Config.ar1Password;
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                String pacusername = Config.pacsadmin;
                String pacpassword = Config.pacspassword;
                String PID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String StudyPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String NominateReason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NominationReason");
                String QueryID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "QueryID");

                //Step-1:Modify the study with description as Abdomen  and send such study to Merge PACs#2
                BasePage.RunBatchFile(Config.batchfilepath, StudyPath + " " + Config.dicomsendpath + " " + Config.StudyPacs);
                ExecutedSteps++;

                //Send the study to dicom devices from MergePacs management page - step 2
                //login to PACS#1 as admin
                mpaclogin.DriverGoTo(mpaclogin.mpacstudyurl);
                MPHomePage mpachome = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);

                //Send Study with matching HL7 order
                Tool tool = (Tool)mpachome.NavigateTopMenu("Tools");
                tool.NavigateToSendStudy();
                tool.SearchStudy("Accession", Accession, 0);

                //Get study details
                Dictionary<string, string> MpacDetails = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());

                tool.MpacSelectStudy("Accession", Accession);
                tool.SendStudy(1);
                mpaclogin.LogoutPacs();
                ExecutedSteps++;

                //To wait Until study reaches Holding pen
                //Login as physician(Destination-1)
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                PageLoadWait.WaitforReceivingStudy(180, PID);
                PageLoadWait.WaitforUpload(Accession, inbounds);
                //Search study
                inbounds.SearchStudy("Accession", Accession);

                //Validate studi is listed in ph inbounds 
                if (inbounds.CheckStudy("Accession", Accession) == true)
                {
                    Logger.Instance.InfoLog("Study with Accession " + Accession + " found in iCA.");
                }
                else
                {
                    Logger.Instance.InfoLog("Study with Accession " + Accession + " not found in iCA.");
                }

                //Logout
                login.Logout();
                
                //Search for such study in HP exists without any updation
                //Login in HP
                login.DriverGoTo(login.hpurl);
                HPHomePage hphomepage1 = hplogin.LoginHPen(hpUserName, hpPassword);
                WorkFlow workflow1 = (WorkFlow)hphomepage1.Navigate("Workflow");

                //search study using acc no' - step 3
                workflow1.NavigateToLink("Workflow", "Archive Search");
                workflow1.HPSearchStudy("Accessionno", Accession);
                ExecutedSteps++;

                Dictionary<string, string> StudyDetails = workflow1.GetStudyDetailsInHP();

                //Get Study date
                String date = DateTime.ParseExact(MpacDetails["StudyDate"], "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString();
                String date1 = DateTime.ParseExact(StudyDetails["Study Date"], "MM/dd/yyyy", CultureInfo.InvariantCulture).ToShortDateString();


                //Validate study details are in sync with details when uploaded - step 7
                if (StudyDetails["Patient Name"].Split(',')[0].Trim().Equals(MpacDetails["PatientName"].Split(' ')[0].Trim()) &&
                   StudyDetails["Patient Name"].Split(',')[1].Trim().Equals(MpacDetails["PatientName"].Split(' ')[1].Trim()) &&
                    StudyDetails["Accession Number"].Equals(MpacDetails["Accession"]) && date.Equals(date1) &&
                    StudyDetails["Study Description"].Equals(MpacDetails["StudyDescription"]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout in HP
                hplogin.LogoutHPen();
                
                //Non Automated Steps - step 5 & 6 --Checking Reconcilation status and log file using putty
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Login as physician(Destination-1)
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Navigate to Inbounds - step 7
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;

                //Search study 
                inbounds.SearchStudy(StudyDetails["Patient Name"].Split(',')[0].Trim(), StudyDetails["Patient Name"].Split(',')[1].Trim(), "", "", "", "", "", "");

                //Validate study is listed in ph's inbounds - step 8
                if (inbounds.CheckStudy("Accession", Accession) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select study
                inbounds.SelectStudy("Accession", Accession);

                //Validate status as uploaded and Reconciliation State as Matched to Order
                String studyState1;

                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Reconciliation State", out studyState1);

                //Validate reconiliation state as "No Matching order" - step 9
                if (studyState1 == "No Matching Order")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                String studyStatus2;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyStatus2);

                //Validate study status as Uploaded - step 10
                if (studyStatus2 == "Uploaded")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
               }

                IWebElement nominate1 = BasePage.Driver.FindElement(By.CssSelector("input#m_nominateStudyButton"));

                //Valiadate Nominate for archive button is enabled - step 11
                if (nominate1.Enabled == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Click Nominate for archive button - step 12
                IWebElement OrderField, ReasonField;
                inbounds.ClickNominateButton(out ReasonField, out OrderField);

                if (BasePage.Driver.FindElement(By.CssSelector("#NominateStudyDialogDiv")).Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select reason field
                OrderField.SendKeys("Testing");
                SelectElement selector = new SelectElement(ReasonField);
                selector.SelectByText(NominateReason);
                Logger.Instance.InfoLog("Reason Field is filled Sucessfully");

                //Click Nominate in confirmation window -step 13
                inbounds.ClickConfirmNominate();
                ExecutedSteps++;

                //Find Study Status 
                String studyStatus6;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyStatus6);
                String statusReason;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status Reason", out statusReason);

                //Validate Study status as Nominated for archive - step 14
                if (studyStatus6 == "Nominated For Archive")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Validate Nominate reason - step 15
                if (statusReason == NominateReason)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                
                //Logout physician(Destination-1)
                login.Logout();

                //Login as archivist(Destination-1)
                login.LoginIConnect(username1, password1);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study
                inbounds.SearchStudy("Accession", Accession);

                //Validate study is listed in ar's inbounds - step 16
                if (inbounds.CheckStudy("Accession", Accession) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Find Study Status and Status Reason
                String studyStatus7;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyStatus7);

                //Validate Study status as Nominated for archive - step 17
                if (studyStatus7 == "Nominated For Archive")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout archivist(Destination-1)
                login.Logout();

                //Delete the above study in EA
                //Login in HP
                login.DriverGoTo(login.hpurl);
                HPHomePage hphomepage = hplogin.LoginHPen(hpUserName, hpPassword);
                WorkFlow workflow = (WorkFlow)hphomepage1.Navigate("Workflow");

                //search study using acc no'
                workflow.NavigateToLink("Workflow", "Archive Search");
                workflow.HPSearchStudy("Accessionno", Accession);
                
                //Delete study in holding pen - step 18
                workflow1.HPDeleteStudy();
                ExecutedSteps++;

                //Logout in HP
                hplogin.LogoutHPen();

                //Login as physician(Destination-1)
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Navigate to Inbounds - step 19
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;

                //Search study and Select study
                inbounds.SearchStudy("Accession", Accession);
                inbounds.SelectStudy("Accession", Accession);

                //Validate the status of the study as Deleted - step 20
                String studyStatus8;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyStatus8);

                if (studyStatus8 == "Deleted")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                
                //Logout physician(Destination-1)
                login.Logout();

                //Login as archivist(Destination-1)
                login.LoginIConnect(username1, password1);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study and Select study
                inbounds.SearchStudy("Accession", Accession);
                inbounds.SelectStudy("Accession", Accession);

                //Validate the status of the study as Deleted - step 21
                String studyStatus9;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyStatus9);

                if (studyStatus9 == "Deleted")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout archivist(Destination-1)
                login.Logout();
                ExecutedSteps++;

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);               

                //Return Result
                return result;
            }

            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                
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
        /// Search Order- Reconciliation window validation with No matching order [Manual Reconciliation]
        /// </summary>
        public TestCaseResult Test_29493(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables          
            Inbounds inbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);         

            try
            {
                //Fetch required Test data
                String username = Config.ph1UserName;
                String password = Config.ph1Password;
                String username1 = Config.ar1UserName;
                String password1 = Config.ar1Password;
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                String pacusername = Config.pacsadmin;
                String pacpassword = Config.pacspassword;
                String pid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String StudyPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String OrderPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderFilePath");
                String NominateReason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NominationReason");
                String OrderAcc = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderAccession");
                
                //Send HL7 order
                Boolean hl7order = mpaclogin.SendHL7Order(Config.MergePACsIP, int.Parse(Config.mpacport), OrderPath);
                
                //Step-2:Examine the order is routed in EA(HP)
                login.DriverGoTo(login.hpurl);
                HPLogin hplogin = new HPLogin();
                HPHomePage hphomepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                WorkFlow workflow = (WorkFlow)hphomepage.Navigate("Workflow");

                //search study using acc no'
                workflow.NavigateToLink("Workflow", "Queue Worklist");
                Boolean order = workflow.HPCheckOrder(OrderAcc);

                if (order == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Get Order details 
                Dictionary<string, string> OrderResults = workflow.GetOrderDetailsInHP();

                //Logout in HP
                hplogin.LogoutHPen();

                //Step-3:Modify the study with description as Abdomen  and send such study to Merge PACs#2
                BasePage.RunBatchFile(Config.batchfilepath,StudyPath+" "+Config.dicomsendpath+" "+Config.StudyPacs);
                ExecutedSteps++;

                //Send the study to dicom devices from MergePacs management page
                //login to PACS#1 as admin
                mpaclogin.DriverGoTo(mpaclogin.mpacstudyurl);
                MPHomePage mpachome = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);

                //Send Study with matching HL7 order
                Tool tool = (Tool)mpachome.NavigateTopMenu("Tools");
                tool.NavigateToSendStudy();
                tool.SearchStudy("Accession", Accession, 0);
                tool.MpacSelectStudy("Accession", Accession);
                tool.SendStudy(1);
                mpaclogin.LogoutPacs();
                ExecutedSteps++;

                //Login as physician(Destination-1)
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                PageLoadWait.WaitforReceivingStudy(180, pid);
                PageLoadWait.WaitforUpload(Accession, inbounds);
                //Search study
                inbounds.SearchStudy("Accession", Accession);

                //Validate studi is listed in ph inbounds - step 4
                if (inbounds.CheckStudy("Accession", Accession) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select study
                inbounds.SelectStudy("Accession", Accession);

                //Validate status as uploaded - step 5
                String studyStatus5;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyStatus5);

                if (studyStatus5 == "Uploaded")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Click Nominate for archive button - step 6
                IWebElement OrderField, ReasonField;
                inbounds.ClickNominateButton(out ReasonField, out OrderField);

                if (BasePage.Driver.FindElement(By.CssSelector("#NominateStudyDialogDiv")).Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select reason field
                OrderField.SendKeys("Testing");
                SelectElement selector = new SelectElement(ReasonField);
                selector.SelectByText(NominateReason);
                Logger.Instance.InfoLog("Reason Field is filled Sucessfully");

                //Click Nominate in confirmation window - step 7
                inbounds.ClickConfirmNominate();
                ExecutedSteps++;

                //Find Study Status 
                String studyStatus6;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyStatus6);
                String statusReason;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status Reason", out statusReason);

                //Validate Study status as Nominated for archive - step 8
                if (studyStatus6 == "Nominated For Archive" && statusReason == NominateReason)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout physician(Destination-1)
                login.Logout();

                //Login as archivist(Destination-1)
                login.LoginIConnect(username1, password1);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study
                inbounds.SearchStudy("Accession", Accession);

                //Validate  study is present in ar inbounds - step 9
                if (inbounds.CheckStudy("Accession", Accession) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                String studyState4;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Reconciliation State", out studyState4);

                //Validate Reconciliation State as No matching Order - step 10
                if (studyState4 == "No Matching Order")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Remove all columns
                inbounds.RemoveAllColumns();

                //Choose columns
                inbounds.ChooseColumns(new String[]{"Last Name", "First Name", "Gender", "Patient DOB", "Issuer of PID", 
                    "Patient ID", "Description", "Study Date", "Accession"});

                //Details of a study
                Dictionary<string, string> rowValues = inbounds.GetMatchingRow("Accession", Accession);
                
                //Select study
                inbounds.SelectStudy1("Accession", Accession);
                
                //Click Archive study button
                inbounds.ClickArchiveStudy("Accession", Accession);

                //Validate Archive/Reconcile Study dialog is opened - step 11
                if (BasePage.Driver.FindElement(By.CssSelector("#ReconciliationControlDialogDiv")).Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Search by order and All Dates,Edit last name,study description and apply  
                inbounds.ArchiveSearch("order", "All Dates");

                //Select Order
                inbounds.ShowAllandSelect("Accession", OrderAcc);

                //Validate Matching order is listed in 'Matching order column' related to studies' last name - step 12
                Dictionary<String, String> MatchingValues = inbounds.GetDataInArchive("Matching Order");
                
                if (MatchingValues["Last Name"].Equals(OrderResults["Patient Name"].Split(',')[0].Trim()))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Details in Original details column
                Dictionary<String, String> OriginalDetails = inbounds.GetDataInArchive("Original Details");
                
                //Validate the details in original details column are in sync with study details - step 13
                if ((OriginalDetails["Last Name"].Equals(rowValues["Last Name"])) && (OriginalDetails["First Name"].Equals(rowValues["First Name"]))&&
                    (OriginalDetails["Gender"].Equals(rowValues["Gender"])) && (OriginalDetails["DOB"].Equals(rowValues["Patient DOB"])) &&
                    (OriginalDetails["Issuer of PID"].Equals(rowValues["Issuer of PID"])) && (OriginalDetails["PID / MRN"].Equals(rowValues["Patient ID"])) &&
                    (OriginalDetails["Description"].Equals(rowValues["Description"])) && (OriginalDetails["Study Date"].Equals(rowValues["Study Date"])) &&
                    (OriginalDetails["Accession"].Equals(rowValues["Accession"])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //In Matching order column - step 14
                //Validate the details listed in Matching order column are in sync with order details- step 14
                if ((MatchingValues["Last Name"].Equals(OrderResults["Patient Name"].Split(',')[0].Trim())) && (MatchingValues["First Name"].Equals(OrderResults["Patient Name"].Split(',')[1].Trim())) &&
                    (MatchingValues["Gender"].Equals(OrderResults["Patient's Sex"])) && inbounds.CompareDates(MatchingValues["DOB"], OrderResults["Patient's Birth Date"]) == true &&
                    (MatchingValues["Issuer of PID"].Equals(OrderResults["IssuerOfPatientID"])) && (MatchingValues["PID / MRN"].Equals(OrderResults["Patient ID"])) &&
                    (MatchingValues["Description"].Equals(OrderResults["Requested Procedure Description"])) && inbounds.CompareDates(MatchingValues["Study Date"], OrderResults["Scheduled Start Date"]) == true &&
                    (MatchingValues["Accession"].Equals(OrderResults["Accession Number"])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Details in Original details column - step 
                Dictionary<String, String> FinalDetails = inbounds.GetDataInArchive("Final Details");
               
                //Validate order details are automatically updated in Final Details column - step 15
                result.steps[++ExecutedSteps].status = "Not Automated";

    /*            if ((MatchingValues["Last Name"].Equals(FinalDetails["Last Name"])) || (MatchingValues["First Name"].Equals(FinalDetails["First Name"])) ||
                    (MatchingValues["Gender"].Equals(FinalDetails["Gender"])) || (MatchingValues["DOB"].Equals(FinalDetails["DOB"])) ||
                    (MatchingValues["Issuer of PID"].Equals(FinalDetails["Issuer of PID"])) || (MatchingValues["PID / MRN"].Equals(FinalDetails["PID / MRN"])) ||
                    (MatchingValues["Description"].Equals(FinalDetails["Description"])) || (MatchingValues["Study Date"].Equals(FinalDetails["Study Date"])) ||
                    (MatchingValues["Accession"].Equals(FinalDetails["Accession"])) &&
                    (OriginalDetails["Prefix"].Equals(FinalDetails["Prefix"])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                */

                //Check  in Original details column after selecting checkbox,validate and archive
                inbounds.SetCheckBoxInArchive("original details", "pid");
                inbounds.SetCheckBoxInArchive("original details", "Accession");

                //Check if Study Date is blank is yes check either study or order
                if (String.IsNullOrEmpty(inbounds.GetDataInArchive("Final Details")["Study Date"]))
                {
                    if (!String.IsNullOrEmpty(inbounds.GetDataInArchive("Original Details")["Study Date"]))
                    {
                        inbounds.SetCheckBoxInArchive("original details", "StudyDate");
                    }
                    else
                    {
                        inbounds.SetCheckBoxInArchive("matching patient", "StudyDate");
                    }
                }

                //Details in Original details column - step 16
                Dictionary<String, String> FinalDetails1 = inbounds.GetDataInArchive("Final Details");

                if (FinalDetails1["PID / MRN"].Equals(OriginalDetails["PID / MRN"]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Confirm archive in Reconcilition window 
                inbounds.ClickArchive();

                //Search study
                inbounds.SearchStudy("Accession", Accession);
                inbounds.SearchStudy("Accession", Accession);

                //Reset Columns
                inbounds.ResetColumns();

                //Validate status is changed to Routing Completed - Step 17
                String studyStatus1;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyStatus1);

                if (studyStatus1 == "Routing Completed")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout archivist
                login.Logout();

                //Login to Merge PACs#3 to check for archived properly after  reconciliation - step 18 
                /**Check MRN/PId value in Merge PACs#3**/

                //login to PACS#1 as admin
                mpaclogin.DriverGoTo(mpaclogin.mpacdesturl);
                MPHomePage mpachome1 = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);

                //Send Study with matching HL7 order
                Tool tool1 = (Tool)mpachome1.NavigateTopMenu("Tools");
                tool1.NavigateToSendStudy();
                tool1.SearchStudy("Accession", Accession, 0);

                //Get study details
                Dictionary<string, string> MpacResults = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());

                String date = DateTime.ParseExact(MpacResults["StudyDate"], "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString();
                String date1 = DateTime.ParseExact(FinalDetails1["Study Date"], "dd-MMM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture).ToShortDateString();
                //date format in Final details - 17-Jan-2006 8:24:20 AM                

                //Validate Details in Final details column on archive window should match with details in Dest PACS
                if ((MpacResults["PatientName"].Split(' ')[0].Equals(FinalDetails1["Last Name"])) && (MpacResults["PatientName"].Split(' ')[1].Equals(FinalDetails1["First Name"])) &&
                    (MpacResults["Sex"].Equals(FinalDetails1["Gender"])) && (inbounds.CompareDates(MpacResults["DOB"], FinalDetails1["DOB"], "yyyyMMdd", "dd-MMM-yyyy")) &&
                    (MpacResults["IPID"].Equals(FinalDetails1["Issuer of PID"])) && (MpacResults["PatientID"].Equals(FinalDetails1["PID / MRN"])) &&
                    (MpacResults["StudyDescription"].Equals(FinalDetails1["Description"])) && (date.Equals(date1)) &&
                    (MpacResults["Accession"].Equals(FinalDetails1["Accession"])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Validate PID in Original details column on Archive window should match with PID in Dest PACS - step 19
                if (OriginalDetails["PID / MRN"].Equals(MpacResults["PatientID"]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout from Dest PACS
                mpaclogin.LogoutPacs();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                
                //Return Result
                return result;

            }

            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);
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
        /// Manual Reconciliation- Automatic Reconciliation Fails Due To No Existing Order for a study with report using Uploader Tool
        /// </summary>
        public TestCaseResult Test2_29488(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables          
            Inbounds inbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String username = Config.ph1UserName;
                String password = Config.ph1Password;
                String username1 = Config.ar1UserName;
                String password1 = Config.ar1Password;
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                String pid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String NominateReason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NominationReason");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");
                String ReportPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ReportPath");
                
                //Step-1 to 6:Upload a study through EI with report by CD,destination1
                //Launch Exam Importer
                ei.LaunchEI();

                //Login as Unregistered user--Step-1
                ei.LoginToEiunReg(email);
                ExecutedSteps++;

                //Select Destination --Step-2-Step-3
                ei.EI_SelectDestination(Config.Dest1);
                ExecutedSteps++;
                ExecutedSteps++;

                //Choose file/study path--Study-4
                ei.SelectFileFromHdd(UploadFilePath);
                ExecutedSteps++;
                

                //Attach report--Step-5
                ei.AttachPDF(ReportPath);
                ExecutedSteps++;
                

                //Selct all patients to upload
                ei.SelectAllPatientsToUpload();
                

                //Send study --Step-6
                ei.Send();
                ExecutedSteps++;

                //Logout EI
                ei.EI_Logout();

                //Close Exam importer
                ei.CloseUploaderTool();

                //Search for such study in HP exists without any updation
                //Step-7Login in HP
                login.DriverGoTo(login.hpurl);
                HPLogin hplogin1 = new HPLogin();
                HPHomePage hphomepage1 = hplogin1.LoginHPen(hpUserName, hpPassword);
                WorkFlow workflow1 = (WorkFlow)hphomepage1.Navigate("Workflow");
                ExecutedSteps++;

                //Search study 
                workflow1.NavigateToLink("Workflow", "Archive Search");
                workflow1.HPSearchStudy("Accessionno", Accession);
                workflow1.HPSearchStudy("PatientID", "*");

                Dictionary<string, string> StudyDetails = workflow1.GetStudyDetailsInHP();

                //Validate study details are in sync with details when uploaded - step 8
                if ((StudyDetails["Accession Number"].Equals(Accession)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                
                //Logout in HP
                hplogin1.LogoutHPen();

                //Step 9 & 10:Check reconcilation status in EA server
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-11:Login as physician
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;

                //Search study and select study
                inbounds.SearchStudy("Accession", Accession);
                inbounds.SelectStudy("Accession", Accession);

                //Lauch study and navigate to history panel tab
                bool reportAvailable = false;
                BluRingViewer bluRingViewer = new BluRingViewer();
                if (Config.isEnterpriseViewer.ToLower() == "y")
                {
                    bluRingViewer = BluRingViewer.LaunchBluRingViewer();
                    PageLoadWait.WaitForFrameLoad(20);
                    var reportIcon = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_activeReportIcon));
                    BasePage.SetCursorPos(0, 0);
                    bluRingViewer.JSMouseHover(reportIcon);
                    reportAvailable = reportIcon.GetCssValue("cursor").Equals("pointer");
                }
                else
                {
                    inbounds.LaunchStudy();
                    inbounds.NavigateToHistoryPanel();
                    inbounds.ChooseColumns(new string[] { "Accession" });
                    Dictionary<string, string> study = inbounds.GetMatchingRow(new string[] { "Accession", "Report" }, new string[] { Accession, "Yes" });
                    if (study != null)
                    {
                        reportAvailable = true;
                    }
                }
                
                //Validate study contains report - step 12
                if (reportAvailable)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //close study viewer
                if (Config.isEnterpriseViewer.ToLower() == "y")
                {
                    bluRingViewer.CloseBluRingViewer();
                }
                else
                {
                    inbounds.CloseStudy();
                }

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study and select study
                inbounds.SearchStudy("Accession", Accession);
                inbounds.SelectStudy("Accession", Accession);

                //Validate status as uploaded and Reconciliation State as No Matching Order - step 13
                Dictionary<string, string> studyState6 = inbounds.GetMatchingRow(new string[] { "Accession", "Reconciliation State" }, new string[] { Accession, "No Matching Order" });

                if (studyState6 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-14:Validate status as Uploading 
                result.steps[++ExecutedSteps].status = "Not Automatable";
                
                Dictionary<string, string> studyStatus7 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Uploaded" });
                
                //Step-15:Validate status as Uploaded
                if (studyStatus7 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {

                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-16:Validate Nominate For Archive button is enabled
                IWebElement nominate1 = BasePage.Driver.FindElement(By.CssSelector("#m_nominateStudyButton"));
                if (nominate1.Enabled == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Logout as Physician
                login.Logout();

                //Step-17:Check the study is not sent to destination

                //login to PACS#3(Destination PACS) as admin
                mpaclogin.DriverGoTo(mpaclogin.mpacdesturl);
                MPHomePage mpachome1 = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);

                //Send Study with matching HL7 order
                Tool tool1 = (Tool)mpachome1.NavigateTopMenu("Tools");
                tool1.NavigateToSendStudy();
                tool1.SearchStudy("Accession", Accession, 0);

                //Get study details
                Dictionary<string, string> studyresults = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());
                                
                //Validate Details in Final details column on archive window should match with details in Dest PACS -step 20
                if (studyresults == null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout from Dest PACS
                mpaclogin.LogoutPacs();

                //Login as Physician
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study
                inbounds.SearchStudy("Accession", Accession);

                //Select study
                inbounds.SelectStudy("Accession", Accession);
                
                //Step-18 :Click on Nominate for Archive
                IWebElement OrderField, ReasonField;
                inbounds.ClickNominateButton(out ReasonField, out OrderField);

                if (BasePage.Driver.FindElement(By.CssSelector("#NominateStudyDialogDiv")).Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select reason field
                OrderField.SendKeys("Testing");
                SelectElement selector = new SelectElement(ReasonField);
                selector.SelectByText(NominateReason);
                Logger.Instance.InfoLog("Reason Field is filled Sucessfully");

                //Step-19:Click Nominate in confirmation window
                inbounds.ClickConfirmNominate();
                ExecutedSteps++;

                //Step-20:Validate  Study Status 
                Dictionary<string, string> studyStatus8 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Nominated For Archive" });

                //Validate Study status as Nominated for archive
                if (studyStatus8 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-21:Validate status Reason as Prior or Exam for Comparison
                Dictionary<string, string> statusReason1 = inbounds.GetMatchingRow(new string[] { "Accession", "Status Reason" }, new string[] { Accession, NominateReason });
                
                if (statusReason1 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout physician
                login.Logout();

                //Step-22:Login as archivist
                login.LoginIConnect(username1, password1);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study
                inbounds.SearchStudy("Accession", Accession);

                //Validate Study is listed in archivist inbounds
                if (inbounds.CheckStudy("Accession", Accession) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Find Study Status and Status Reason
                Dictionary<string, string> studyStatus9 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Nominated For Archive" });

                //Step-23:Validate Study status as Nominated for archive 
                if (studyStatus9 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-24:Select study
                inbounds.SelectStudy("Accession", Accession);

                //Click ArchiveStudy
                inbounds.ClickArchiveStudy("", "");

                //Validate Archive/Reconcile Study dialog is opened 
                if (BasePage.Driver.FindElement(By.CssSelector("#ReconciliationControlDialogDiv")).Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-25:Search order 
                inbounds.ArchiveSearch("order", "All Dates");

                //Validate No Matching Order is listed in Last name criteria
                //Details in Matching Order column
                Dictionary<String, String> OrderDetails = inbounds.GetDataInArchive("Matching Order");

                if (OrderDetails["Last Name"].Equals(""))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-26:Edit in Final field column
                inbounds.EditFinalDetailsInArchive("last name", "firsttest1");
                inbounds.EditFinalDetailsInArchive("description", "RightHand");
                ExecutedSteps++;

                //Check if Study Date is blank is yes check either from study or order
                if (String.IsNullOrEmpty(inbounds.GetDataInArchive("Final Details")["Study Date"]))
                {
                    if (!String.IsNullOrEmpty(inbounds.GetDataInArchive("Original Details")["Study Date"]))
                    {
                        inbounds.SetCheckBoxInArchive("original details", "StudyDate");
                    }
                    else
                    {
                        inbounds.SetCheckBoxInArchive("matching patient", "StudyDate");
                    }
                }
                Dictionary<String, String> FinalDetails1 = inbounds.GetDataInArchive("Final Details");

                //Step-27:Click Archive
                inbounds.ClickArchive();
                ExecutedSteps++;

                //Search study
                inbounds.SearchStudy("Accession", Accession);
                inbounds.SearchStudy("Accession", Accession);

                //Step-28:Check the status as Routing started(Not automated)
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-29:Validate status as Routing Completed 
                String studyStatus5;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyStatus5);

                if (studyStatus5 == "Routing Completed")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {

                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Logout archivist
                login.Logout();

                //Step-30:Login to Merge PACs#3 to check for archived properly after reconciliation

                //login to PACS#1 as admin
                mpaclogin.DriverGoTo(mpaclogin.mpacdesturl);
                MPHomePage mpachome = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);

                //Send Study with matching HL7 order
                Tool tool = (Tool)mpachome1.NavigateTopMenu("Tools");
                tool.NavigateToSendStudy();
                tool.SearchStudy("Accession", Accession, 0);

                //Get study details
                Dictionary<string, string> MpacResults = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());
                String mpacdate = DateTime.ParseExact(MpacResults["StudyDate"], "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString();
                String finaldate = DateTime.ParseExact(FinalDetails1["Study Date"], "dd-MMM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture).ToShortDateString();

                //Validate Details in Final details column on archive window should match with details in Dest PACS 
                if ((MpacResults["PatientName"].Split(' ')[0].ToLower().Equals(FinalDetails1["Last Name"].ToLower())) && (MpacResults["PatientName"].Split(' ')[1].Equals(FinalDetails1["First Name"])) &&
                    (MpacResults["Sex"].Equals(FinalDetails1["Gender"])) && inbounds.CompareDates(MpacResults["DOB"], FinalDetails1["DOB"], "yyyyMMdd", "dd-MMM-yyyy") &&
                    (MpacResults["IPID"].Equals(FinalDetails1["Issuer of PID"])) && (MpacResults["PatientID"].Equals(FinalDetails1["PID / MRN"])) &&
                    (MpacResults["StudyDescription"].Equals(FinalDetails1["Description"])) && mpacdate.Equals(finaldate) &&
                    (MpacResults["Accession"].Equals(FinalDetails1["Accession"])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout from Dest PACS
                mpaclogin.LogoutPacs();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                
                //Return Result
                return result;
            }
           catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //logout
                login.Logout();
                
                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Manual Reconciliation- Automatic Reconciliation Fails Due To No Existing Order for a study with report using Uploader Tool
        /// </summary>
        public TestCaseResult Test2_162427(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables          
            Inbounds inbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String username = Config.ph1UserName;
                String password = Config.ph1Password;
                String username1 = Config.ar1UserName;
                String password1 = Config.ar1Password;
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                String pid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String NominateReason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NominationReason");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");
                String ReportPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ReportPath");

                //Step-1 to 6:Upload a study through EI with report by CD,destination1
                //Launch Exam Importer
                ei.LaunchEI();

                //Login as Unregistered user--Step-1
                ei.LoginToEiunReg(email);
                ExecutedSteps++;

                //Select Destination --Step-2-Step-3
                ei.EI_SelectDestination(Config.Dest1);
                ExecutedSteps++;
                ExecutedSteps++;

                //Choose file/study path--Study-4
                ei.SelectFileFromHdd(UploadFilePath);
                ExecutedSteps++;


                //Attach report--Step-5
                ei.AttachPDF(ReportPath);
                ExecutedSteps++;


                //Selct all patients to upload
                ei.SelectAllPatientsToUpload();


                //Send study --Step-6
                ei.Send();
                ExecutedSteps++;

                //Logout EI
                ei.EI_Logout();

                //Close Exam importer
                ei.CloseUploaderTool();

                //Search for such study in HP exists without any updation
                //Step-7Login in HP
                login.DriverGoTo(login.hpurl);
                HPLogin hplogin1 = new HPLogin();
                HPHomePage hphomepage1 = hplogin1.LoginHPen(hpUserName, hpPassword);
                WorkFlow workflow1 = (WorkFlow)hphomepage1.Navigate("Workflow");
                ExecutedSteps++;

                //Search study 
                workflow1.NavigateToLink("Workflow", "Archive Search");
                workflow1.HPSearchStudy("Accessionno", Accession);
                workflow1.HPSearchStudy("PatientID", "*");

                Dictionary<string, string> StudyDetails = workflow1.GetStudyDetailsInHP();

                //Validate study details are in sync with details when uploaded - step 8
                if ((StudyDetails["Accession Number"].Equals(Accession)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout in HP
                hplogin1.LogoutHPen();

                //Step 9 & 10:Check reconcilation status in EA server
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-11:Login as physician
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;

                //Search study and select study
                inbounds.SearchStudy("Accession", Accession);
                inbounds.SelectStudy("Accession", Accession);

                //Lauch study and navigate to history panel tab
                bool reportAvailable = false;
                BluRingViewer bluRingViewer = new BluRingViewer();
                if (Config.isEnterpriseViewer.ToLower() == "y")
                {
                    bluRingViewer = BluRingViewer.LaunchBluRingViewer();
                    PageLoadWait.WaitForFrameLoad(20);
                    var reportIcon = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_activeReportIcon));
                    BasePage.SetCursorPos(0, 0);
                    bluRingViewer.JSMouseHover(reportIcon);
                    reportAvailable = reportIcon.GetCssValue("cursor").Equals("pointer");
                }
                else
                {
                    inbounds.LaunchStudy();
                    inbounds.NavigateToHistoryPanel();
                    inbounds.ChooseColumns(new string[] { "Accession" });
                    Dictionary<string, string> study = inbounds.GetMatchingRow(new string[] { "Accession", "Report" }, new string[] { Accession, "Yes" });
                    if (study != null)
                    {
                        reportAvailable = true;
                    }
                }

                //Validate study contains report - step 12
                if (reportAvailable)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //close study viewer
                if (Config.isEnterpriseViewer.ToLower() == "y")
                {
                    bluRingViewer.CloseBluRingViewer();
                }
                else
                {
                    inbounds.CloseStudy();
                }

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study and select study
                inbounds.SearchStudy("Accession", Accession);
                inbounds.SelectStudy("Accession", Accession);

                //Validate status as uploaded and Reconciliation State as No Matching Order - step 13
                Dictionary<string, string> studyState6 = inbounds.GetMatchingRow(new string[] { "Accession", "Reconciliation State" }, new string[] { Accession, "No Matching Order" });

                if (studyState6 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-14:Validate status as Uploading 
                result.steps[++ExecutedSteps].status = "Not Automatable";

                Dictionary<string, string> studyStatus7 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Uploaded" });

                //Step-15:Validate status as Uploaded
                if (studyStatus7 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {

                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-16:Validate Nominate For Archive button is enabled
                IWebElement nominate1 = BasePage.Driver.FindElement(By.CssSelector("#m_nominateStudyButton"));
                if (nominate1.Enabled == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Logout as Physician
                login.Logout();

                //Step-17:Check the study is not sent to destination

                //login to PACS#3(Destination PACS) as admin
                mpaclogin.DriverGoTo(mpaclogin.mpacdesturl);
                MPHomePage mpachome1 = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);

                //Send Study with matching HL7 order
                Tool tool1 = (Tool)mpachome1.NavigateTopMenu("Tools");
                tool1.NavigateToSendStudy();
                tool1.SearchStudy("Accession", Accession, 0);

                //Get study details
                Dictionary<string, string> studyresults = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());

                //Validate Details in Final details column on archive window should match with details in Dest PACS -step 20
                if (studyresults == null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout from Dest PACS
                mpaclogin.LogoutPacs();

                //Login as Physician
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study
                inbounds.SearchStudy("Accession", Accession);

                //Select study
                inbounds.SelectStudy("Accession", Accession);

                //Step-18 :Click on Nominate for Archive
                IWebElement OrderField, ReasonField;
                inbounds.ClickNominateButton(out ReasonField, out OrderField);

                if (BasePage.Driver.FindElement(By.CssSelector("#NominateStudyDialogDiv")).Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select reason field
                OrderField.SendKeys("Testing");
                SelectElement selector = new SelectElement(ReasonField);
                selector.SelectByText(NominateReason);
                Logger.Instance.InfoLog("Reason Field is filled Sucessfully");

                //Step-19:Click Nominate in confirmation window
                inbounds.ClickConfirmNominate();
                ExecutedSteps++;

                //Step-20:Validate  Study Status 
                Dictionary<string, string> studyStatus8 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Nominated For Archive" });

                //Validate Study status as Nominated for archive
                if (studyStatus8 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-21:Validate status Reason as Prior or Exam for Comparison
                Dictionary<string, string> statusReason1 = inbounds.GetMatchingRow(new string[] { "Accession", "Status Reason" }, new string[] { Accession, NominateReason });

                if (statusReason1 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout physician
                login.Logout();

                //Step-22:Login as archivist
                login.LoginIConnect(username1, password1);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study
                inbounds.SearchStudy("Accession", Accession);

                //Validate Study is listed in archivist inbounds
                if (inbounds.CheckStudy("Accession", Accession) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Find Study Status and Status Reason
                Dictionary<string, string> studyStatus9 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Nominated For Archive" });

                //Step-23:Validate Study status as Nominated for archive 
                if (studyStatus9 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-24:Select study
                inbounds.SelectStudy("Accession", Accession);

                //Click ArchiveStudy
                inbounds.ClickArchiveStudy("", "");

                //Validate Archive/Reconcile Study dialog is opened 
                if (BasePage.Driver.FindElement(By.CssSelector("#ReconciliationControlDialogDiv")).Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-25:Search order 
                inbounds.ArchiveSearch("order", "All Dates");

                //Validate No Matching Order is listed in Last name criteria
                //Details in Matching Order column
                Dictionary<String, String> OrderDetails = inbounds.GetDataInArchive("Matching Order");

                if (OrderDetails["Last Name"].Equals(""))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-26:Edit in Final field column
                inbounds.EditFinalDetailsInArchive("last name", "firsttest1");
                inbounds.EditFinalDetailsInArchive("description", "RightHand");
                ExecutedSteps++;

                //Check if Study Date is blank is yes check either from study or order
                if (String.IsNullOrEmpty(inbounds.GetDataInArchive("Final Details")["Study Date"]))
                {
                    if (!String.IsNullOrEmpty(inbounds.GetDataInArchive("Original Details")["Study Date"]))
                    {
                        inbounds.SetCheckBoxInArchive("original details", "StudyDate");
                    }
                    else
                    {
                        inbounds.SetCheckBoxInArchive("matching patient", "StudyDate");
                    }
                }
                Dictionary<String, String> FinalDetails1 = inbounds.GetDataInArchive("Final Details");

                //Step-27:Click Archive
                inbounds.ClickArchive();
                ExecutedSteps++;

                //Search study
                inbounds.SearchStudy("Accession", Accession);
                inbounds.SearchStudy("Accession", Accession);

                //Step-28:Check the status as Routing started(Not automated)
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-29:Validate status as Routing Completed 
                String studyStatus5;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyStatus5);

                if (studyStatus5 == "Routing Completed")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {

                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Logout archivist
                login.Logout();

                //Step-30:Login to Merge PACs#3 to check for archived properly after reconciliation

                //login to PACS#1 as admin
                mpaclogin.DriverGoTo(mpaclogin.mpacdesturl);
                MPHomePage mpachome = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);

                //Send Study with matching HL7 order
                Tool tool = (Tool)mpachome1.NavigateTopMenu("Tools");
                tool.NavigateToSendStudy();
                tool.SearchStudy("Accession", Accession, 0);

                //Get study details
                Dictionary<string, string> MpacResults = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());
                String mpacdate = DateTime.ParseExact(MpacResults["StudyDate"], "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString();
                String finaldate = DateTime.ParseExact(FinalDetails1["Study Date"], "dd-MMM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture).ToShortDateString();

                //Validate Details in Final details column on archive window should match with details in Dest PACS 
                if ((MpacResults["PatientName"].Split(' ')[0].ToLower().Equals(FinalDetails1["Last Name"].ToLower())) && (MpacResults["PatientName"].Split(' ')[1].Equals(FinalDetails1["First Name"])) &&
                    (MpacResults["Sex"].Equals(FinalDetails1["Gender"])) && inbounds.CompareDates(MpacResults["DOB"], FinalDetails1["DOB"], "yyyyMMdd", "dd-MMM-yyyy") &&
                    (MpacResults["IPID"].Equals(FinalDetails1["Issuer of PID"])) && (MpacResults["PatientID"].Equals(FinalDetails1["PID / MRN"])) &&
                    (MpacResults["StudyDescription"].Equals(FinalDetails1["Description"])) && mpacdate.Equals(finaldate) &&
                    (MpacResults["Accession"].Equals(FinalDetails1["Accession"])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout from Dest PACS
                mpaclogin.LogoutPacs();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Automatic Reconciliation - Reconciliation matches the Order and Study based on  PatientID and Accession number combination
        /// </summary>
        public TestCaseResult Test_29485(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables          
            Inbounds inbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {

                //Fetch required Test data
                String username = Config.ph1UserName;
                String password = Config.ph1Password;
                String username1 = Config.ar1UserName;
                String password1 = Config.ar1Password;
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                String pacusername = Config.pacsadmin;
                String pacpassword = Config.pacspassword;
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String pid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String acc = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String acclist = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = acclist.Split(':');
                String name = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                String[] namesplit = name.Split(' ');
                String orderpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderFilePath");
                String studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                //Step-1:Send an order to Merge PACs#1
                //Send HL7 order
                Boolean hl7order = mpaclogin.SendHL7Order(Config.MergePACsIP, int.Parse(Config.mpacport), orderpath);
                if (hl7order == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("HL7 Order not sent");

                }

                //Step-2:Examine the order is routed in EA(HP)
                login.DriverGoTo(login.hpurl);
                HPLogin hplogin = new HPLogin();
                HPHomePage hphomepage = hplogin.LoginHPen(hpUserName, hpPassword);
                WorkFlow workflow = (WorkFlow)hphomepage.Navigate("Workflow");

                //search order using acc no'
                workflow.NavigateToLink("Workflow", "Queue Worklist");
                Boolean order = workflow.HPCheckOrder(acc);

                if (order == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Order is not in Holding Pen");
                }

                Dictionary<string, string> orderresults = workflow.GetOrderDetailsInHP();

                //Logout in HP
                hplogin.LogoutHPen();

                //Step-3:Modify the study with description as Abdomen  and send such study to Merge PACs#2
                BasePage.RunBatchFile(Config.batchfilepath, studypath + " " + Config.dicomsendpath + " " + Config.StudyPacs);
                ExecutedSteps++;

                //Step-4:Send the study to dicom devices from MergePacs management page
                //Login MPacs
                login.DriverGoTo(login.mpacstudyurl);
                MpacLogin mplogin = new MpacLogin();
                MPHomePage homepage = new MPHomePage();
                mplogin.Loginpacs(pacusername, pacpassword);
                Tool tools = (Tool)homepage.NavigateTopMenu("Tools");
                homepage.NavigateTopMenu("Tools");
                tools.NavigateToSendStudy();
                tools.SearchStudy("Accession", acc, 0);
                tools.MpacSelectStudy("Patient ID", pid);
                tools.SendStudy(1);

                //Logout MPacs
                mpaclogin.LogoutPacs();
                ExecutedSteps++;

                //Wait till study reaches iConnect and it status gets updated to Routing Completed
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                PageLoadWait.WaitforStudyInStatus(acc, (Inbounds)login.Navigate("Inbounds"), "Routing Completed");
                login.Logout();

                //Search for the study in HP with updation
                //Step-5:Login in HP
                login.DriverGoTo(login.hpurl);
                HPLogin hplogin1 = new HPLogin();
                HPHomePage hphomepage1 = hplogin1.LoginHPen(hpUserName, hpPassword);

                WorkFlow workflow1 = (WorkFlow)hphomepage1.Navigate("Workflow");
                ExecutedSteps++;

                //Step-6:Search for the study using acc no'
                workflow1.NavigateToLink("Workflow", "Archive Search");
                PageLoadWait.WaitForStudyInHp(180, acc, workflow1);
                workflow1.HPSearchStudy("Accessionno", acc);

                Dictionary<string, string> studyresults = workflow.GetStudyDetailsInHP();
                Boolean study = workflow1.HPCheckStudy(acc);


                if (study == true && studyresults["Study Description"].Equals((orderresults["Requested Procedure Description"])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }



                //Logout in HP
                hplogin1.LogoutHPen();


                //Step-7 & 8:Check reconcilation status in EA server
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-9:Login as admin
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;


                //Step-10:Search study
                inbounds.SearchStudy("Accession", acc);

                if (inbounds.CheckStudy("Accession", acc) == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout admin
                login.Logout();


                //Step-11:Login as physician
                login.LoginIConnect(username, password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;

                //Step-12:Search study

                inbounds.SearchStudy(studyresults["Patient Name"].Split(',')[0].Trim(), studyresults["Patient Name"].Split(',')[1].Trim(), "", "", "", "", "", "");
                if (inbounds.CheckStudy("Accession", acc) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-13:Select study
                inbounds.SelectStudy("Accession", acc);

                //Validate status as uploaded and Reconciliation State as Matched to Order


                Dictionary<string, string> study0 = inbounds.GetMatchingRow(new string[] { "Accession", "Reconciliation State" }, new string[] { acc, "Matched to Order" });

                if (study0 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-14:Non automatable for status as Uploaded
                result.steps[++ExecutedSteps].status = "Not Automated";


                //Step-15:Validate status is  Routing Completed and nominate button is disabled
                Dictionary<string, string> study1 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { acc, "Routing Completed" });

                if (study1 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {

                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-16:Validate Nominate button is disabled
                IWebElement nominate1 = BasePage.Driver.FindElement(By.CssSelector("#m_nominateStudyButton"));
                if (nominate1.Enabled == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout physician
                login.Logout();


                //Step-17:Login as archivist
                login.LoginIConnect(username1, password1);


                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;

                //Step-18:Search study
                inbounds.SearchStudy("Accession", acc);
                if (inbounds.CheckStudy("Accession", acc) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-19:Select study
                inbounds.SelectStudy("Accession", acc);

                //Validate Matched to Order, routing completed and archive button is disabled

                Dictionary<string, string> study2 = inbounds.GetMatchingRow(new string[] { "Accession", "Reconciliation State" }, new string[] { acc, "Matched to Order" });

                if (study2 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-20:
                Dictionary<string, string> study3 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { acc, "Routing Completed" });

                if (study3 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {

                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();

                }


                //Step-21:
                IWebElement archive = BasePage.Driver.FindElement(By.CssSelector("#m_archiveStudyButton"));

                if (archive.Enabled == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout archivist(Destination-1)
                login.Logout();


                //Step-22:Login to Merge PACs#3 to check for archived properly after automatic reconciliation**/
                login.DriverGoTo(login.mpacdesturl);
                mplogin = new MpacLogin();
                homepage = new MPHomePage();
                mplogin.Loginpacs(pacusername, pacpassword);
                tools = (Tool)homepage.NavigateTopMenu("Tools");
                homepage.NavigateTopMenu("Tools");
                tools.NavigateToSendStudy();
                tools.SearchStudy("Accession", acc, 0);
                Dictionary<String, String> StudyInfoMapcs = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());
                if ((StudyInfoMapcs["StudyDescription"].Equals(studyresults["Study Description"])) && (StudyInfoMapcs["PatientID"].Equals(pid)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[0].SetLogs();

                }

                //Logout 
                mplogin.LogoutPacs();


                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Logout 
                login.Logout();

                //Return Result
                return result;

            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

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
        /// Automatic Reconciliation -Reconciliation matches the Order and Study based on  Patient Full Name combination
        /// By configuring in EA
        /// </summary>
        public TestCaseResult Test1_29486(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables          
            Inbounds inbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            String DefaultTag = "";

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                //Fetch required Test data
                String username = Config.ph1UserName;
                String password = Config.ph1Password;
                String username1 = Config.ar1UserName;
                String password1 = Config.ar1Password;
                String pacusername = Config.pacsadmin;
                String pacpassword = Config.pacspassword;
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String pid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String acc = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String acclist = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNo_List");
                String[] AccessionNumbers = acclist.Split(':');
                String name = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                String[] namesplit = name.Split(' ');
                String orderacc = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderAccession");
                String orderpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderFilePath");
                String studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String QueryId = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "QueryID");
                String refphy = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyReferingPhysician");
                String orderrefphy = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderReferingPhysician");


                //Step-1:Login to EA weadmin 
                login.DriverGoTo(login.hpurl);
                HPLogin hplogin = new HPLogin();
                HPHomePage hphomepage = hplogin.LoginHPen(hpUserName, hpPassword);
                Configure configure = (Configure)hphomepage.Navigate("Configure");
                ExecutedSteps++;

                //Step-2:configure by removing existing details in queryIdTags and enter as  00100010
                configure.NavigateToTab("properties");
                DefaultTag = configure.UpdateQueryIDTags("removealladd", QueryId);
                ExecutedSteps++;

                //Step-3:Restart Clarc Service 
                Putty putty = new Putty();
                putty.RestartService();
                ExecutedSteps++;

                //Step-4:Send an order with patient's fullname as Reconcile2 and ref.physician DONALD,J to Merge PACs#1 
                Boolean hl7order = mpaclogin.SendHL7Order(Config.MergePACsIP, int.Parse(Config.mpacport), orderpath);
                if (hl7order == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("HL7 Order not sent");

                }

                //Step-5:Examine order is routed in EA(HP)
                login.DriverGoTo(login.hpurl);
                hplogin = new HPLogin();
                hphomepage = hplogin.LoginHPen(hpUserName, hpPassword);
                WorkFlow workflow = (WorkFlow)hphomepage.Navigate("Workflow");

                //search order using acc no'
                workflow.NavigateToLink("Workflow", "Queue Worklist");
                Boolean order = workflow.HPCheckOrder(orderacc);

                if (order == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Order is not in Holding Pen");
                }

                Dictionary<string, string> orderresults = workflow.GetOrderDetailsInHP();

                //Logout HP
                hplogin.LogoutHPen();


                //Step-6: Modify the order as different PatientID,AccNo and Ref.Ph as DOCTOR 3D and send such study to Merge PACs#2
                BasePage.RunBatchFile(Config.batchfilepath, studypath + " " + Config.dicomsendpath + " " + Config.StudyPacs);
                ExecutedSteps++;

                //Step-7:Send the study to dicom devices from MergePacs management page
                //Login MPacs
                login.DriverGoTo(login.mpacstudyurl);
                MpacLogin mplogin = new MpacLogin();
                MPHomePage homepage = new MPHomePage();
                mplogin.Loginpacs(pacusername, pacpassword);
                Tool tools = (Tool)homepage.NavigateTopMenu("Tools");
                homepage.NavigateTopMenu("Tools");
                tools.NavigateToSendStudy();
                tools.SearchStudy("Accession", acc, 0);
                tools.MpacSelectStudy("Patient ID", pid);
                tools.SendStudy(1);

                //Logout MPacs
                mpaclogin.LogoutPacs();
                ExecutedSteps++;

                //Wait till study reaches iConnect and it status gets updated to Routing Completed
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                PageLoadWait.WaitforStudyInStatus(acc, (Inbounds)login.Navigate("Inbounds"), "Routing Completed");
                login.Logout();


                //Step-8:Search for such study in HP with updation
                //Login in HP
                login.DriverGoTo(login.hpurl);
                hplogin = new HPLogin();
                hphomepage = new HPHomePage();
                workflow = new WorkFlow();
                hplogin.LoginHPen(hpUserName, hpPassword);
                hphomepage.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                ExecutedSteps++;

                //Step-9:search study using PatientID
                PageLoadWait.WaitForStudyInHp(180, acc, workflow);
                Boolean study = workflow.HPCheckStudy(acc);
                Dictionary<string, string> studyresults = workflow.GetStudyDetailsInHP();

                BasePage.Driver.FindElement(By.CssSelector("input[name='accessionNumber']")).Clear();
                workflow.HPSearchStudy("PatientID", pid);


                //Select then click edit and take Ref Phy value
                //BasePage.Driver.FindElement(By.CssSelector("tr.odd>td:nth-child(4)>a")).Click();
                BasePage.Driver.FindElements(By.CssSelector("tr.odd>td"))[3].FindElement(By.CssSelector("a")).Click();

                PageLoadWait.WaitForHPPageLoad(20);
                IWebElement edit1 = BasePage.Driver.FindElement(By.CssSelector("a>img[title='Edit']"));
                edit1.Click();
                PageLoadWait.WaitForHPPageLoad(20);
                var javascript = "document.querySelector(\"table:nth-child(6) .deviceDetailsTableHeader td>img\").click()";

                if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && (BasePage.BrowserVersion.ToLower().Equals("8") || BasePage.BrowserVersion.ToLower().Equals("9")))
                    BasePage.Driver.FindElements(By.CssSelector(".deviceDetailsTableHeader td> img"))[1].Click();
                else
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(javascript);

                PageLoadWait.WaitForHPPageLoad(20);
                String studyrefph = BasePage.Driver.FindElement(By.CssSelector("input[name='ReferringPhysician']")).GetAttribute("value").Replace("^", " ").TrimEnd();
                String ordrefphy = orderresults["Referring Physician"].ToUpper().Replace(",", "");

                if (study == true && studyrefph.Equals(ordrefphy))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout in HP
                hplogin.LogoutHPen();


                //Step-10 & 11:Check reconcilation status in EA server
                result.steps[++ExecutedSteps].status = "Non Automatable";
                result.steps[++ExecutedSteps].status = "Non Automatable";


                //Step-12:Login as physician
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;

                //Step-13:Search study
                inbounds.SearchStudy(studyresults["Patient Name"].Split(',')[0].Trim(), studyresults["Patient Name"].Split(',')[1].Trim(), "", "", "", "", "", "");


                if (inbounds.CheckStudy("Accession", acc) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-14:Select study and Validate Reconciliation State as Matched to Order
                inbounds.SelectStudy("Accession", acc);

                Dictionary<string, string> studyState1 = inbounds.GetMatchingRow(new string[] { "Accession", "Reconciliation State" }, new string[] { acc, "Matched to Order" });


                if (studyState1 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 15-Non automatable
                result.steps[++ExecutedSteps].status = "Non Automatable";


                //Step-16:Validate status is as Routing Completed

                Dictionary<string, string> studyStatus3 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { acc, "Routing Completed" });

                if (studyStatus3 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {

                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();

                }


                //Step-17:Validate whether Nominate button is disabled
                IWebElement nominate1 = BasePage.Driver.FindElement(By.CssSelector("#m_nominateStudyButton"));

                if (nominate1.Enabled == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout physician
                login.Logout();


                //Step-18:Login as archivist
                login.LoginIConnect(username1, password1);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;

                //Step-19:Search study
                inbounds.SearchStudy("Accession", acc);
                if (inbounds.CheckStudy("Accession", acc) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + acc + "study is in ph2(Destination-1)");
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + acc + "study is not in ph2(Destination-1)");
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-20:Select study
                inbounds.SelectStudy("Accession", acc);

                //Validate Matched to Order

                Dictionary<string, string> studyState4 = inbounds.GetMatchingRow(new string[] { "Accession", "Reconciliation State" }, new string[] { acc, "Matched to Order" });


                if (studyState4 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-21:Validate whether status is as Routing Completed

                Dictionary<string, string> studyStatus5 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { acc, "Routing Completed" });
                if (studyStatus5 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {

                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();

                }


                //Step-22:Validate whether archive button is disabled
                IWebElement archive = BasePage.Driver.FindElement(By.CssSelector("#m_archiveStudyButton"));

                if (archive.Enabled == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout archivist(Destination-1)
                login.Logout();


                //Step-23:Login to Merge PACs#3 to check for archived properly after automatic reconciliation
                login.DriverGoTo(login.mpacdesturl);
                mplogin = new MpacLogin();
                homepage = new MPHomePage();
                mplogin.Loginpacs(pacusername, pacpassword);
                tools = (Tool)homepage.NavigateTopMenu("Tools");
                homepage.NavigateTopMenu("Tools");
                tools.NavigateToSendStudy();
                tools.SearchStudy("Accession", acc, 0);
                Dictionary<String, String> StudyInfoMapcs = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());
                if ((StudyInfoMapcs["StudyDescription"].Equals(studyresults["Study Description"])) && (StudyInfoMapcs["PatientID"].Equals(pid)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();

                }

                //Logout 
                mplogin.LogoutPacs();


                ////Reset the QueryIdTag
                //login.DriverGoTo(login.hpurl);
                //hplogin.LoginHPen(hpUserName, hpPassword);
                //hphomepage.Navigate("Configure");

                //configure.NavigateToTab("properties");
                //configure.UpdateQueryIDTags("removealladd", DefaultTag);


                ////Restart Clarc Service 
                //putty.RestartService();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);


                //Return Result
                return result;
            }

            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);


                ////Reset the QueryIdTag
                //login.DriverGoTo(login.hpurl);
                //HPLogin hplogin = new HPLogin();
                //HPHomePage hphomepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                //Configure configure = (Configure)hphomepage.Navigate("Configure");

                //configure.NavigateToTab("properties");
                //configure.UpdateQueryIDTags("removealladd", DefaultTag);


                ////Restart Clarc Service 
                //Putty putty = new Putty();
                //putty.RestartService();


                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Automatic Reconciliation -Reconciliation matches the Order and Study based on  Patient Full Name combination
        /// By ExamImporter
        /// </summary>
        public TestCaseResult Test2_29486(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables          
            Inbounds inbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            String DefaultTag = "";

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String username = Config.ph1UserName;
                String password = Config.ph1Password;
                String username1 = Config.ar1UserName;
                String password1 = Config.ar1Password;
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                String pacusername = Config.pacsadmin;
                String pacpassword = Config.pacspassword;
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String pid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String acc = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String acclist = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNo_List");
                String[] AccessionNumbers = acclist.Split(':');
                String name = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                String[] namesplit = name.Split(' ');
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");
                String ImageFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ImageFilePath");
                String description = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Description");
                String refphy = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyReferingPhysician");
                String orderrefphy = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderReferingPhysician");
                String orderdescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderDescription");
                String QueryId = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "QueryID");


                //Login to EA weadmin 
                login.DriverGoTo(login.hpurl);
                HPLogin hplogin = new HPLogin();
                HPHomePage hphomepage = hplogin.LoginHPen(hpUserName, hpPassword);
                Configure configure = (Configure)hphomepage.Navigate("Configure");
                
                //configure by removing existing details in queryIdTags and enter as  00100010
                configure.NavigateToTab("properties");
                DefaultTag = configure.UpdateQueryIDTags("removealladd", QueryId);

                //Restart Clarc Service 
                Putty putty = new Putty();
                putty.RestartService(); 


                //Step-1 to 6:Upload a study through EI as unregistered user,destination1
                ei.LaunchEI();
                ei.LoginToEiunReg(email);
                ExecutedSteps++;
                ei.EI_SelectDestination(Config.Dest1);
                ExecutedSteps++;
                ei.SelectFileFromHdd(UploadFilePath);
                ExecutedSteps++;
                ExecutedSteps++;
                ei.SelectAllPatientsToUpload();
                ExecutedSteps++;
                ei.Send();
                ei.EI_Logout();
                ei.CloseUploaderTool();
                ExecutedSteps++;

                //Wait till study uploaded in iConnect and it status gets updated to Routing Completed
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                PageLoadWait.WaitforStudyInStatus(acc, (Inbounds)login.Navigate("Inbounds"), "Routing Completed");
                login.Logout();
                                
                //Step-7:Search for such study in HP with updation
                //Login in HP
                login.DriverGoTo(login.hpurl);
                HPLogin hplogin1 = new HPLogin();
                hphomepage = (HPHomePage)hplogin1.LoginHPen(hpUserName, hpPassword);
                WorkFlow workflow1 = (WorkFlow)hphomepage.Navigate("Workflow");
                workflow1.NavigateToLink("Workflow", "Archive Search");
                ExecutedSteps++;

                //Step-8:Search study using PatientID
                workflow1.HPSearchStudy("PatientID", pid);

                Dictionary<string, string> studyresults = workflow1.GetStudyDetailsInHP();

                //Select then click edit and take Ref Phy value
                //BasePage.Driver.FindElement(By.CssSelector("tr.odd>td:nth-child(4)>a")).Click();
                BasePage.Driver.FindElements(By.CssSelector("tr.odd>td"))[3].FindElement(By.CssSelector("a")).Click();

                PageLoadWait.WaitForHPPageLoad(20);
                IWebElement edit1 = BasePage.Driver.FindElement(By.CssSelector("a>img[title='Edit']"));
                edit1.Click();
                PageLoadWait.WaitForHPPageLoad(20);
                var javascript = "document.querySelector(\"table:nth-child(6) .deviceDetailsTableHeader td>img\").click()";
                
                if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && (BasePage.BrowserVersion.ToLower().Equals("8") || BasePage.BrowserVersion.ToLower().Equals("9")))
                    BasePage.Driver.FindElements(By.CssSelector(".deviceDetailsTableHeader td> img"))[1].Click();
                else
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(javascript);

                String studyrefph = BasePage.Driver.FindElement(By.CssSelector("input[name='ReferringPhysician']")).GetAttribute("value").Replace("^", " ").TrimEnd();
                String ordrefphy = orderrefphy.ToUpper().Replace(",", "");

                if (studyrefph.Equals(ordrefphy))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout in HP
                hplogin1.LogoutHPen();


                //Step-9 & 10:***Check reconcilation status in EA server**/
                result.steps[++ExecutedSteps].status = "Non Automatable";
                result.steps[++ExecutedSteps].status = "Non Automatable";


                //Step-11:Login as physician
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;

                //Step-12:Search study
                inbounds.SearchStudy(studyresults["Patient Name"].Split(',')[0].Trim(), studyresults["Patient Name"].Split(',')[1].Trim(), "", "", "", "", "", "");

                if (inbounds.CheckStudy("Accession", acc) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-13:Select study
                inbounds.SelectStudy("Accession", acc);


                //Validate Reconciliation State as Matched to Order
                Dictionary<string, string> study0 = inbounds.GetMatchingRow(new string[] { "Accession", "Reconciliation State" }, new string[] { acc, "Matched to Order" });

                if (study0 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-14:Non Automatable
                result.steps[++ExecutedSteps].status = "Non Automatable";



                //Step-15:Validate status is as Routing Completed and nominate button is disabled
                Dictionary<string, string> study1 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { acc, "Routing Completed" });

                if (study1 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {

                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();

                }

                //Step-16:Validate Nominate button is disabled
                IWebElement nominate = BasePage.Driver.FindElement(By.CssSelector("#m_nominateStudyButton"));

                if (nominate.Enabled == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout physician
                login.Logout();

                //Step-17:Login as archivist
                login.LoginIConnect(username1, password1);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;

                //Step-18:Search study
                inbounds.SearchStudy("Accession", acc);
                if (inbounds.CheckStudy("Accession", acc) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + acc + "study is in ph2(Destination-1)");
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + acc + "study is not in ph2(Destination-1)");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-19:Select study
                inbounds.SelectStudy("Accession", acc);

                //Validate Matched to Order
                Dictionary<string, string> study2 = inbounds.GetMatchingRow(new string[] { "Accession", "Reconciliation State" }, new string[] { acc, "Matched to Order" });

                if (study2 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-20:Validate status as  routing completed 
                Dictionary<string, string> study3 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { acc, "Routing Completed" });

                if (study3 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {

                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-21:Validate Archive study button is disabled
                IWebElement archive1 = BasePage.Driver.FindElement(By.CssSelector("#m_archiveStudyButton"));

                if (archive1.Enabled == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout archivist
                login.Logout();

                //Step-22:Login to Merge PACs#3 to check for archived properly after automatic reconciliation**/
                login.DriverGoTo(login.mpacdesturl);
                mpaclogin = new MpacLogin();                
                MPHomePage mphomepage =  mpaclogin.Loginpacs(pacusername, pacpassword);
                Tool mpactool = (Tool)mphomepage.NavigateTopMenu("Tools");
                mphomepage.NavigateTopMenu("Tools");
                mpactool.NavigateToSendStudy();
                mpactool.SearchStudy("Accession", acc, 0);
                Dictionary<String, String> StudyInfoMapcs = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());
                if ((StudyInfoMapcs["StudyDescription"].Equals(orderdescription)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();

                }

                //Logout 
                mpaclogin.LogoutPacs();

                //Reset the QueryId

               // //Login to EA weadmin 
               //login.DriverGoTo(login.hpurl);
               //hplogin = new HPLogin();
               //hphomepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
               //configure = (Configure)hphomepage.Navigate("Configure");
                
               
               //configure.NavigateToTab("properties");
               //configure.UpdateQueryIDTags("removealladd",  DefaultTag);

               ////Restart Clarc Service 
               //putty = new Putty();
               //putty.RestartService(); 

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);


                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);
                //login.Logout();

               // //Reset the QueryId

               ////Login to EA weadmin 
               //login.DriverGoTo(login.hpurl);
               //HPLogin hplogin = new HPLogin();
               //HPHomePage hphomepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
               //Configure configure = (Configure)hphomepage.Navigate("Configure");
                
               
               //configure.NavigateToTab("properties");
               //configure.UpdateQueryIDTags("removealladd",  DefaultTag);

               ////Restart Clarc Service 
               //Putty putty = new Putty();
               //putty.RestartService(); 


                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                
                //log out--
                login.Logout();


                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Automatic Reconciliation -Reconciliation matches the Order and Study based on Issuer of PatientID, Patient ID and Accession combination
        /// </summary>
        public TestCaseResult Test_29487(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables          
            Inbounds inbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            String DefaultTag = "";

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                //Fetch required Test data
                String username = Config.ph1UserName;
                String password = Config.ph1Password;
                String username1 = Config.ar1UserName;
                String password1 = Config.ar1Password;
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                String pacusername = Config.pacsadmin;
                String pacpassword = Config.pacspassword;
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String pid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String acc = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");

                String acclist = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNo_List");
                String[] AccessionNumbers = acclist.Split(':');
                String name = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                String[] namesplit = name.Split(' ');
                String QueryIds = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "QueryID");
                String[] QueryId = QueryIds.Split(':');
                String orderpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderFilePath");
                String studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");


                //Step-1:Login to EA weadmin 
                login.DriverGoTo(login.hpurl);
                HPLogin hplogin = new HPLogin();
                HPHomePage hphomepage = hplogin.LoginHPen(hpUserName, hpPassword);
                Configure configure = (Configure)hphomepage.Navigate("Configure");
                ExecutedSteps++;

                //Step-2:configure by removing existing details in queryIdTags and enter as 00100020,00100021,00080050
                configure.NavigateToTab("properties");
                DefaultTag = configure.UpdateQueryIDTags("removealladd", QueryId[0] + "," + QueryId[1] + "," + QueryId[2]);
                ExecutedSteps++;


                //Step-3:Restart Clarc Service 
                Putty putty = new Putty();
                putty.RestartService();
                ExecutedSteps++;

                //Step-4:Send an order with Patient name as Reconcile 3,Patient ID as RE-0028B ,Acc no' RE000022B and IPID as 1234 to Merge PACs#1 ***/
                Boolean hl7order = mpaclogin.SendHL7Order(Config.MergePACsIP, int.Parse(Config.mpacport), orderpath);
                if (hl7order == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("HL7 Order not sent");

                }


                //Step-5:Examine  order is routed in EA(HP)
                login.DriverGoTo(login.hpurl);
                HPLogin hplogin1 = new HPLogin();
                HPHomePage hphomepage1 = hplogin1.LoginHPen(hpUserName, hpPassword);
                WorkFlow workflow = (WorkFlow)hphomepage1.Navigate("Workflow");

                //search study using acc no'
                workflow.NavigateToLink("Workflow", "Queue Worklist");
                Boolean order = workflow.HPCheckOrder(acc);

                if (order == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("HL7 Order not sent");
                }

                Dictionary<string, string> orderresults = workflow.GetOrderDetailsInHP();

                //Logout HP
                hplogin1.LogoutHPen();

                //Step-6:Modify the order with different name as Reconcile 3467 but same ID,IPID,AccNo and send such study to Merge PACs#2
                BasePage.RunBatchFile(Config.batchfilepath, studypath + " " + Config.dicomsendpath + " " + Config.StudyPacs);
                ExecutedSteps++;

                //Step-7:Send the study to dicom devices from MergePacs management page
                //Login MPacs
                login.DriverGoTo(login.mpacstudyurl);
                MPHomePage mphomepage = mpaclogin.Loginpacs(pacusername, pacpassword);                
               Tool mpactool = (Tool)mphomepage.NavigateTopMenu("Tools");
                mphomepage.NavigateTopMenu("Tools");
                mpactool.NavigateToSendStudy();
                mpactool.SearchStudy("Accession", acc, 0);
                mpactool.MpacSelectStudy("Patient ID", pid);
                mpactool.SendStudy(1);

                //Logout MPacs
                mpaclogin.LogoutPacs();
                ExecutedSteps++;

                //Wait till study reaches iConnect and it status gets updated to Routing Completed
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                PageLoadWait.WaitforStudyInStatus(acc, (Inbounds)login.Navigate("Inbounds"), "Routing Completed");
                login.Logout();

                //Search for such study in HP with updation
                //Step-8:Login in HP
                login.DriverGoTo(login.hpurl);
                hplogin = new HPLogin();
                hphomepage = new HPHomePage();
                workflow = new WorkFlow();
                hplogin.LoginHPen(hpUserName, hpPassword);
                hphomepage.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                ExecutedSteps++;

                //Step-9:Search study using acc no' 
                PageLoadWait.WaitForStudyInHp(180, acc, workflow);
                workflow.HPSearchStudy("Accessionno", acc);
                //IWebElement studydes = BasePage.Driver.FindElement(By.CssSelector(".odd td:nth-child(7)>a"));
                IWebElement studydes = BasePage.Driver.FindElements(By.CssSelector(".odd td"))[6].FindElement(By.CssSelector("a"));
                String studydescription = studydes.Text;
                Boolean study = workflow.HPCheckStudy(acc);

                Dictionary<string, string> studyresults = workflow.GetStudyDetailsInHP();

                if (study == true && pid.Equals(orderresults["Patient ID"]) && acc.Equals(orderresults["Accession Number"]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout in HP
                hplogin.LogoutHPen();


                //Step-10 & 11:Check reconcilation status in EA server
                result.steps[++ExecutedSteps].status = "Non Automatable";
                result.steps[++ExecutedSteps].status = "Non Automatable";

                //Step-12:Login as physician
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;

                //Step-13:Search study
                inbounds.SearchStudy(studyresults["Patient Name"].Split(',')[0].Trim(), studyresults["Patient Name"].Split(',')[1].Trim(), "", "", "", "", "", "");

                if (inbounds.CheckStudy("Accession", acc) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-14:Select study
                inbounds.ChooseColumns(new string[] { "Issuer of PID" });
                Dictionary<string, string> studyresults1 = inbounds.GetMatchingRow("Accession", acc);
                inbounds.SelectStudy1("Patient ID", pid);

                //Validate status as uploaded and Reconciliation State as Matched to Order
                Dictionary<string, string> studyState1 = inbounds.GetMatchingRow(new string[] { "Accession", "Reconciliation State" }, new string[] { acc, "Matched to Order" });

                if (studyState1 != null && orderresults["IssuerOfPatientID"].Equals(studyresults1["Issuer of PID"]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-15:Non automatable
                result.steps[++ExecutedSteps].status = "Non Automatable";

                //Step-16:Validate status is as Routing Completed 
                Dictionary<string, string> studyStatus3 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { acc, "Routing Completed" });

                if (studyStatus3 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {

                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-17:Validate the nominate button is disabled
                IWebElement nominate1 = BasePage.Driver.FindElement(By.CssSelector("#m_nominateStudyButton"));

                if (nominate1.Enabled == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout physician
                login.Logout();

                //Step-18:Login as archivist
                login.LoginIConnect(username1, password1);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;

                //Step-19:Search study
                inbounds.SearchStudy("Accession", acc);

                if (inbounds.CheckStudy("Accession", acc) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step-20:Select study
                inbounds.SelectStudy("Accession", acc);

                //Validate Reconcile state as Matched to Order
                Dictionary<string, string> studyState4 = inbounds.GetMatchingRow(new string[] { "Accession", "Reconciliation State" }, new string[] { acc, "Matched to Order" });

                if (studyState4 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-21:Validate the status as routing completed
                Dictionary<string, string> studyStatus5 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { acc, "Routing Completed" });

                if (studyStatus5 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {

                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-22:Validate the archive button is disabled
                IWebElement archive = BasePage.Driver.FindElement(By.CssSelector("#m_archiveStudyButton"));

                if (archive.Enabled == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout archivist
                login.Logout();

                //Step-23:Login to Merge PACs#3 to check for archived properly after automatic reconciliation
                login.DriverGoTo(login.mpacdesturl);
                mpaclogin = new MpacLogin();
                mphomepage = new MPHomePage();
                mpaclogin.Loginpacs(pacusername, pacpassword);
                mpactool = (Tool)mphomepage.NavigateTopMenu("Tools");
                mphomepage.NavigateTopMenu("Tools");
                mpactool.NavigateToSendStudy();
                mpactool.SearchStudy("Accession", acc, 0);
                Dictionary<String, String> StudyInfoMapcs = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());
                if ((StudyInfoMapcs["IPID"].Equals(orderresults["IssuerOfPatientID"])) && (StudyInfoMapcs["PatientID"].Equals(pid)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[0].SetLogs();

                }

                //Logout 
                mpaclogin.LogoutPacs();

                ////Reset QueryIdTag
                //login.DriverGoTo(login.hpurl);
                //hplogin = new HPLogin();
                //hphomepage = new HPHomePage();
                //configure = new Configure();
                //hplogin.LoginHPen(hpUserName, hpPassword);
                //hphomepage.Navigate("Configure");

                //configure.NavigateToTab("properties");
                //configure.UpdateQueryIDTags("removealladd", DefaultTag);


                ////Restart Clarc Service 
                //putty = new Putty();
                //putty.RestartService();


                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);
                login.Logout();


                ////Reset the QueryId
                //login.DriverGoTo(login.hpurl);
                //HPLogin hplogin = new HPLogin();
                //hphomepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                //Configure configure = (Configure)hphomepage.Navigate("Configure");

                //configure.NavigateToTab("properties");
                //configure.UpdateQueryIDTags("removealladd", DefaultTag);
                
                ////Restart Clarc Service 
                //Putty putty = new Putty();
                //putty.RestartService();
                //ExecutedSteps++;

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
        /// Manual Reconciliation- Automatic Reconciliation Fails Due To No Existing Order for a study using PACS
        /// </summary>
        public TestCaseResult Test1_29488(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables          
            Inbounds inbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            String DefaultTag = "";

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String username = Config.ph1UserName;
                String password = Config.ph1Password;
                String username1 = Config.ar1UserName;
                String password1 = Config.ar1Password;
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                String pacusername = Config.pacsadmin;
                String pacpassword = Config.pacspassword;
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String pid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String acc = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String orderacc = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderAccession");
                String acclist = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNo_List");
                String[] AccessionNumbers = acclist.Split(':');
                String name = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                String[] namesplit = name.Split(' ');
                String NominateReason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NominationReason");
                String[] reasonsplit = NominateReason.Split(';');
                String QueryIds = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "QueryID");
                String[] QueryId = QueryIds.Split(':');
                String orderpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderFilePath");
                String studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");


                //Step-1:Login to EA weadmin 
                login.DriverGoTo(login.hpurl);
                HPLogin hplogin = new HPLogin();
                HPHomePage hphomepage = hplogin.LoginHPen(hpUserName, hpPassword);
                Configure configure = (Configure)hphomepage.Navigate("Configure");
                ExecutedSteps++;

                //Step-2:configure by removing existing details in queryIdTags and enter as 00100020,00100021,00080050
                configure.NavigateToTab("properties");
                DefaultTag = configure.UpdateQueryIDTags("removealladd", QueryId[0] + "," + QueryId[2]);
                ExecutedSteps++;


                //Step-3:Restart Clarc Service 
                Putty putty = new Putty();
                putty.RestartService();
                ExecutedSteps++;

                //Step-4:Send an order with Patient ID as RE93038 and Acc no'as RE01809  to Merge PACs#1
                Boolean hl7order = mpaclogin.SendHL7Order(Config.MergePACsIP, int.Parse(Config.mpacport), orderpath);
                if (hl7order == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("HL7 Order not sent");

                }


                //Step-5:Examine such order is routed in EA(HP)
                login.DriverGoTo(login.hpurl);
                hplogin = new HPLogin();
                hphomepage = hplogin.LoginHPen(hpUserName, hpPassword);
                WorkFlow workflow = (WorkFlow)hphomepage.Navigate("Workflow");

                //search study using acc no'
                workflow.NavigateToLink("Workflow", "Queue Worklist");
                Boolean order = workflow.HPCheckOrder(orderacc);
                Dictionary<string, string> orderresults = workflow.GetOrderDetailsInHP();

                if (order == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Logout HP
                hplogin.LogoutHPen();


                //Step-6:Import a study which does not match the existing order Patient ID and Acc no' to Merge PACs#2
                BasePage.RunBatchFile(Config.batchfilepath, studypath + " " + Config.dicomsendpath + " " + Config.StudyPacs);
                ExecutedSteps++;

                //Step-7:Send the study to dicom devices from MergePacs management page
                //Login MPacs
                login.DriverGoTo(login.mpacstudyurl);
                MPHomePage mphomepage = mpaclogin.Loginpacs(pacusername, pacpassword);                
                Tool mpactool = (Tool)mphomepage.NavigateTopMenu("Tools");
                mphomepage.NavigateTopMenu("Tools");
                mpactool.NavigateToSendStudy();
                mpactool.SearchStudy("Accession", acc, 0);
                mpactool.MpacSelectStudy("Patient ID", pid);
                mpactool.SendStudy(1);

                //Logout MPacs
                mpaclogin.LogoutPacs();
                ExecutedSteps++;

                //Search for such study in HP exists without any updation
                //Step-8:Login in HP
                login.DriverGoTo(login.hpurl);
                hplogin = new HPLogin();
                hphomepage = new HPHomePage();
                workflow = new WorkFlow();
                hplogin.LoginHPen(hpUserName, hpPassword);
                hphomepage.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                ExecutedSteps++;

                //Step-9:search study using acc no'
                PageLoadWait.WaitForStudyInHp(180, acc, workflow);
                workflow.HPSearchStudy("Accessionno", acc);

                Dictionary<string, string> studyresults = workflow.GetStudyDetailsInHP();
                Boolean study = workflow.HPCheckStudy(acc);

                if (study == true && (studyresults["Study Description"].Equals(orderresults["Requested Procedure Description"])) == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout in HP
                hplogin.LogoutHPen();

                //Step-10 & 11:Check reconcilation status in EA server**
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";


                //Step-12:Login as physician
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;

                //Step-13:Search study
                inbounds.SearchStudy(studyresults["Patient Name"].Split(',')[0].Trim(), studyresults["Patient Name"].Split(',')[1].Trim(), "", "", "", "", "", "");


                if (inbounds.CheckStudy("Accession", acc) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-14:Select study
                inbounds.SelectStudy("Accession", acc);

                //Validate Reconciliation State as No Matching Order
                Dictionary<string, string> study0 = inbounds.GetMatchingRow(new string[] { "Accession", "Reconciliation State" }, new string[] { acc, "No Matching Order" });

                if (study0 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-15:Validate status as uploaded 

                Dictionary<string, string> study1 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { acc, "Uploaded" });

                if (study1 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {

                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-16:Validate Nominate button is enabled
                IWebElement nominate1 = BasePage.Driver.FindElement(By.CssSelector("#m_nominateStudyButton"));

                if (nominate1.Enabled == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as Physician
                login.Logout();

                //Step-17:Check the study is not sent to destination

                //login to PACS#3(Destination PACS) as admin
                login.DriverGoTo(login.mpacdesturl);
                mpaclogin = new MpacLogin();
                MPHomePage mpachome1 = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);

                //Send Study with matching HL7 order
                Tool tool1 = (Tool)mpachome1.NavigateTopMenu("Tools");
                tool1.NavigateToSendStudy();
                tool1.SearchStudy("Accession", acc, 0);

                //Get study details
                Dictionary<string, string> studyresults1 = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());

                //Validate Details in Final details column on archive window should match with details in Dest PACS -step 20
                if (studyresults1 == null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout from Dest PACS
                mpaclogin.LogoutPacs();

                //Login as Physician
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study
                inbounds.SearchStudy(studyresults["Patient Name"].Split(',')[0].Trim(), studyresults["Patient Name"].Split(',')[1].Trim(), "", "", "", "", "", "");

                //Select study
                inbounds.SelectStudy("Accession", acc);


                //Step-18 :Click on Nominate for Archive
                IWebElement OrderField, ReasonField;
                inbounds.ClickNominateButton(out ReasonField, out OrderField);

                if (BasePage.Driver.FindElement(By.CssSelector("#NominateStudyDialogDiv")).Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select reason field
                OrderField.SendKeys("Testing");
                SelectElement selector = new SelectElement(ReasonField);
                selector.SelectByText(reasonsplit[0]);
                Logger.Instance.InfoLog("Reason Field is filled Sucessfully");

                //Step-19:Click Nominate in confirmation window
                inbounds.ClickConfirmNominate();
                ExecutedSteps++;


                Dictionary<string, string> studyStatus3 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { acc, "Nominated For Archive" });

                //Step-20 :Validate the Study Status  as Nominated for archive
                if (studyStatus3 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                Dictionary<string, string> statusReason = inbounds.GetMatchingRow(new string[] { "Accession", "Status Reason" }, new string[] { acc, reasonsplit[0] });
                //Step-21:Validate the Status Reason as Interpretation Required
                if (statusReason != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout physician
                login.Logout();

                //Step-22:Login as archivist
                login.LoginIConnect(username1, password1);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study
                inbounds.SearchStudy("Accession", acc);

                //Validate Study is listed in archivist inbounds
                if (inbounds.CheckStudy("Accession", acc) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Find Study Status
                Dictionary<string, string> studyStatus4 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { acc, "Nominated For Archive" });


                //Step-23:Validate Study status as Nominated for archive 
                if (studyStatus4 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-24:Select study
                inbounds.SelectStudy("Accession", acc);

                //Click ArchiveStudy
                inbounds.ClickArchiveStudy("", "");

                //Validate Archive/Reconcile Study dialog is opened 
                if (BasePage.Driver.FindElement(By.CssSelector("#ReconciliationControlDialogDiv")).Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-25:Search order 
                inbounds.ArchiveSearch("order", "All Dates");

                //Validate No Matching Order is listed in Last name criteria
                //Details in Matching Order column
                Dictionary<String, String> OrderDetails = inbounds.GetDataInArchive("Matching Order");

                if (OrderDetails["Last Name"].Equals(""))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }



                //Step-26:Edit in Final field column
                inbounds.EditFinalDetailsInArchive("last name", "firsttest1");
                inbounds.EditFinalDetailsInArchive("description", "RightHand");
                ExecutedSteps++;

                //Check if Study Date is blank is yes check either study or order
                if (String.IsNullOrEmpty(inbounds.GetDataInArchive("Final Details")["Study Date"]))
                {
                    if (!String.IsNullOrEmpty(inbounds.GetDataInArchive("Original Details")["Study Date"]))
                    {
                        inbounds.SetCheckBoxInArchive("original details", "StudyDate");
                    }
                    else
                    {
                        inbounds.SetCheckBoxInArchive("matching patient", "StudyDate");
                    }
                }

                Dictionary<String, String> FinalDetails1 = inbounds.GetDataInArchive("Final Details");

                //Step-27:Click Archive
                inbounds.ClickArchive();
                ExecutedSteps++;


                //Search study
                inbounds.SearchStudy("acc", acc);
                inbounds.SearchStudy("acc", acc);

                //Step-28:Check the status as Routing started(Not automated)
                result.steps[++ExecutedSteps].status = "Not Automated";


                Dictionary<string, string> studyStatus5 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { acc, "Routing Completed" });

                //Step-29:Validate status as Routing Completed 

                if (studyStatus5 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {

                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Logout archivist
                login.Logout();

                //Step-30:Login to Merge PACs#3 to check for archived properly after reconciliation
                login.DriverGoTo(login.mpacdesturl);
                mpaclogin = new MpacLogin();
                mpachome1 = new MPHomePage();
                mpachome1 = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);

                //Send Study with matching HL7 order
                tool1 = new Tool();
                tool1 = (Tool)mpachome1.NavigateTopMenu("Tools");
                tool1.NavigateToSendStudy();
                tool1.SearchStudy("Accession", acc, 0);

                //Get study details
                Dictionary<string, string> MpacResults = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());

                var mpacdate = DateTime.ParseExact(MpacResults["StudyDate"], "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString();
                var finaldate = DateTime.ParseExact(FinalDetails1["Study Date"], "dd-MMM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture).ToShortDateString();

                //Validate Details in Final details column on archive window should match with details in Dest PACS 36
                if ((MpacResults["PatientName"].Split(' ')[0].ToUpper().Equals(FinalDetails1["Last Name"].ToUpper())) && (MpacResults["PatientName"].Split(' ')[1].ToUpper().Equals(FinalDetails1["First Name"].ToUpper())) &&
                    (MpacResults["Sex"].Equals(FinalDetails1["Gender"])) && (inbounds.CompareDates(MpacResults["DOB"], FinalDetails1["DOB"], "yyyyMMdd", "dd-MMM-yyyy")) &&
                    (MpacResults["IPID"].Equals(FinalDetails1["Issuer of PID"])) && (MpacResults["PatientID"].Equals(FinalDetails1["PID / MRN"])) &&
                    (MpacResults["StudyDescription"].Equals(FinalDetails1["Description"])) && mpacdate.Equals(finaldate) &&
                    (MpacResults["Accession"].Equals(FinalDetails1["Accession"])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout from Dest PACS
                mpaclogin.LogoutPacs();

                ////**************Reset the QuerID tag changed*********************
                //login.DriverGoTo(login.hpurl);
                //hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                //configure = (Configure)hphomepage.Navigate("Configure");

                ////Navigate to Properties browser tab
                //configure.NavigateToTab("properties");

                ////Update QueryID tag as 00100010
                //configure.UpdateQueryIDTags("removealladd", DefaultTag);


                ////Restart Clarc service
                //putty.RestartService();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);


                //Return Result
                return result;
            }

            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);
                login.Logout();

                ////**************Reset the QuerID tag changed*********************
                //login.DriverGoTo(login.hpurl);
                //hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                //configure = (Configure)hphomepage.Navigate("Configure");

                ////Navigate to Properties browser tab
                //configure.NavigateToTab("properties");

                ////Update QueryID tag as 00100010
                //configure.UpdateQueryIDTags("removealladd", DefaultTag);



                ////Restart Clarc service
                //Putty putty = new Putty();
                //putty.RestartService();


                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);


                //Return Result
                return result;

            }

        }

        /// <summary>
        /// Manual Reconciliation- Automatic Reconciliation Fails Due To No Existing Order for a study using PACS
        /// </summary>
        public TestCaseResult Test1_162427(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables          
            Inbounds inbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            String DefaultTag = "";

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String username = Config.ph1UserName;
                String password = Config.ph1Password;
                String username1 = Config.ar1UserName;
                String password1 = Config.ar1Password;
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                String pacusername = Config.pacsadmin;
                String pacpassword = Config.pacspassword;
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String pid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String acc = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String orderacc = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderAccession");
                String acclist = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNo_List");
                String[] AccessionNumbers = acclist.Split(':');
                String name = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                String[] namesplit = name.Split(' ');
                String NominateReason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NominationReason");
                String[] reasonsplit = NominateReason.Split(';');
                String QueryIds = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "QueryID");
                String[] QueryId = QueryIds.Split(':');
                String orderpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderFilePath");
                String studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");


                //Step-1:Login to EA weadmin 
                login.DriverGoTo(login.hpurl);
                HPLogin hplogin = new HPLogin();
                HPHomePage hphomepage = hplogin.LoginHPen(hpUserName, hpPassword);
                Configure configure = (Configure)hphomepage.Navigate("Configure");
                ExecutedSteps++;

                //Step-2:configure by removing existing details in queryIdTags and enter as 00100020,00100021,00080050
                configure.NavigateToTab("properties");
                DefaultTag = configure.UpdateQueryIDTags("removealladd", QueryId[0] + "," + QueryId[2]);
                ExecutedSteps++;


                //Step-3:Restart Clarc Service 
                Putty putty = new Putty();
                putty.RestartService();
                ExecutedSteps++;

                //Step-4:Send an order with Patient ID as RE93038 and Acc no'as RE01809  to Merge PACs#1
                Boolean hl7order = mpaclogin.SendHL7Order(Config.MergePACsIP, int.Parse(Config.mpacport), orderpath);
                if (hl7order == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("HL7 Order not sent");

                }


                //Step-5:Examine such order is routed in EA(HP)
                login.DriverGoTo(login.hpurl);
                hplogin = new HPLogin();
                hphomepage = hplogin.LoginHPen(hpUserName, hpPassword);
                WorkFlow workflow = (WorkFlow)hphomepage.Navigate("Workflow");

                //search study using acc no'
                workflow.NavigateToLink("Workflow", "Queue Worklist");
                Boolean order = workflow.HPCheckOrder(orderacc);
                Dictionary<string, string> orderresults = workflow.GetOrderDetailsInHP();

                if (order == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Logout HP
                hplogin.LogoutHPen();


                //Step-6:Import a study which does not match the existing order Patient ID and Acc no' to Merge PACs#2
                BasePage.RunBatchFile(Config.batchfilepath, studypath + " " + Config.dicomsendpath + " " + Config.StudyPacs);
                ExecutedSteps++;

                //Step-7:Send the study to dicom devices from MergePacs management page
                //Login MPacs
                login.DriverGoTo(login.mpacstudyurl);
                MPHomePage mphomepage = mpaclogin.Loginpacs(pacusername, pacpassword);
                Tool mpactool = (Tool)mphomepage.NavigateTopMenu("Tools");
                mphomepage.NavigateTopMenu("Tools");
                mpactool.NavigateToSendStudy();
                mpactool.SearchStudy("Accession", acc, 0);
                mpactool.MpacSelectStudy("Patient ID", pid);
                mpactool.SendStudy(1);

                //Logout MPacs
                mpaclogin.LogoutPacs();
                ExecutedSteps++;

                //Search for such study in HP exists without any updation
                //Step-8:Login in HP
                login.DriverGoTo(login.hpurl);
                hplogin = new HPLogin();
                hphomepage = new HPHomePage();
                workflow = new WorkFlow();
                hplogin.LoginHPen(hpUserName, hpPassword);
                hphomepage.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                ExecutedSteps++;

                //Step-9:search study using acc no'
                PageLoadWait.WaitForStudyInHp(180, acc, workflow);
                workflow.HPSearchStudy("Accessionno", acc);

                Dictionary<string, string> studyresults = workflow.GetStudyDetailsInHP();
                Boolean study = workflow.HPCheckStudy(acc);

                if (study == true && (studyresults["Study Description"].Equals(orderresults["Requested Procedure Description"])) == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout in HP
                hplogin.LogoutHPen();

                //Step-10 & 11:Check reconcilation status in EA server**
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";


                //Step-12:Login as physician
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;

                //Step-13:Search study
                inbounds.SearchStudy(studyresults["Patient Name"].Split(',')[0].Trim(), studyresults["Patient Name"].Split(',')[1].Trim(), "", "", "", "", "", "");


                if (inbounds.CheckStudy("Accession", acc) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-14:Select study
                inbounds.SelectStudy("Accession", acc);

                //Validate Reconciliation State as No Matching Order
                Dictionary<string, string> study0 = inbounds.GetMatchingRow(new string[] { "Accession", "Reconciliation State" }, new string[] { acc, "No Matching Order" });

                if (study0 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-15:Validate status as uploaded 

                Dictionary<string, string> study1 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { acc, "Uploaded" });

                if (study1 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {

                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-16:Validate Nominate button is enabled
                IWebElement nominate1 = BasePage.Driver.FindElement(By.CssSelector("#m_nominateStudyButton"));

                if (nominate1.Enabled == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as Physician
                login.Logout();

                //Step-17:Check the study is not sent to destination

                //login to PACS#3(Destination PACS) as admin
                login.DriverGoTo(login.mpacdesturl);
                mpaclogin = new MpacLogin();
                MPHomePage mpachome1 = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);

                //Send Study with matching HL7 order
                Tool tool1 = (Tool)mpachome1.NavigateTopMenu("Tools");
                tool1.NavigateToSendStudy();
                tool1.SearchStudy("Accession", acc, 0);

                //Get study details
                Dictionary<string, string> studyresults1 = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());

                //Validate Details in Final details column on archive window should match with details in Dest PACS -step 20
                if (studyresults1 == null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout from Dest PACS
                mpaclogin.LogoutPacs();

                //Login as Physician
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study
                inbounds.SearchStudy(studyresults["Patient Name"].Split(',')[0].Trim(), studyresults["Patient Name"].Split(',')[1].Trim(), "", "", "", "", "", "");

                //Select study
                inbounds.SelectStudy("Accession", acc);


                //Step-18 :Click on Nominate for Archive
                IWebElement OrderField, ReasonField;
                inbounds.ClickNominateButton(out ReasonField, out OrderField);

                if (BasePage.Driver.FindElement(By.CssSelector("#NominateStudyDialogDiv")).Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select reason field
                OrderField.SendKeys("Testing");
                SelectElement selector = new SelectElement(ReasonField);
                selector.SelectByText(reasonsplit[0]);
                Logger.Instance.InfoLog("Reason Field is filled Sucessfully");

                //Step-19:Click Nominate in confirmation window
                inbounds.ClickConfirmNominate();
                ExecutedSteps++;


                Dictionary<string, string> studyStatus3 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { acc, "Nominated For Archive" });

                //Step-20 :Validate the Study Status  as Nominated for archive
                if (studyStatus3 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                Dictionary<string, string> statusReason = inbounds.GetMatchingRow(new string[] { "Accession", "Status Reason" }, new string[] { acc, reasonsplit[0] });
                //Step-21:Validate the Status Reason as Interpretation Required
                if (statusReason != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout physician
                login.Logout();

                //Step-22:Login as archivist
                login.LoginIConnect(username1, password1);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study
                inbounds.SearchStudy("Accession", acc);

                //Validate Study is listed in archivist inbounds
                if (inbounds.CheckStudy("Accession", acc) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Find Study Status
                Dictionary<string, string> studyStatus4 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { acc, "Nominated For Archive" });


                //Step-23:Validate Study status as Nominated for archive 
                if (studyStatus4 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-24:Select study
                inbounds.SelectStudy("Accession", acc);

                //Click ArchiveStudy
                inbounds.ClickArchiveStudy("", "");

                //Validate Archive/Reconcile Study dialog is opened 
                if (BasePage.Driver.FindElement(By.CssSelector("#ReconciliationControlDialogDiv")).Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-25:Search order 
                inbounds.ArchiveSearch("order", "All Dates");

                //Validate No Matching Order is listed in Last name criteria
                //Details in Matching Order column
                Dictionary<String, String> OrderDetails = inbounds.GetDataInArchive("Matching Order");

                if (OrderDetails["Last Name"].Equals(""))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }



                //Step-26:Edit in Final field column
                inbounds.EditFinalDetailsInArchive("last name", "firsttest1");
                inbounds.EditFinalDetailsInArchive("description", "RightHand");
                ExecutedSteps++;

                //Check if Study Date is blank is yes check either study or order
                if (String.IsNullOrEmpty(inbounds.GetDataInArchive("Final Details")["Study Date"]))
                {
                    if (!String.IsNullOrEmpty(inbounds.GetDataInArchive("Original Details")["Study Date"]))
                    {
                        inbounds.SetCheckBoxInArchive("original details", "StudyDate");
                    }
                    else
                    {
                        inbounds.SetCheckBoxInArchive("matching patient", "StudyDate");
                    }
                }

                Dictionary<String, String> FinalDetails1 = inbounds.GetDataInArchive("Final Details");

                //Step-27:Click Archive
                inbounds.ClickArchive();
                ExecutedSteps++;


                //Search study
                inbounds.SearchStudy("acc", acc);
                inbounds.SearchStudy("acc", acc);

                //Step-28:Check the status as Routing started(Not automated)
                result.steps[++ExecutedSteps].status = "Not Automated";


                Dictionary<string, string> studyStatus5 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { acc, "Routing Completed" });

                //Step-29:Validate status as Routing Completed 

                if (studyStatus5 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {

                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Logout archivist
                login.Logout();

                //Step-30:Login to Merge PACs#3 to check for archived properly after reconciliation
                login.DriverGoTo(login.mpacdesturl);
                mpaclogin = new MpacLogin();
                mpachome1 = new MPHomePage();
                mpachome1 = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);

                //Send Study with matching HL7 order
                tool1 = new Tool();
                tool1 = (Tool)mpachome1.NavigateTopMenu("Tools");
                tool1.NavigateToSendStudy();
                tool1.SearchStudy("Accession", acc, 0);

                //Get study details
                Dictionary<string, string> MpacResults = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());

                var mpacdate = DateTime.ParseExact(MpacResults["StudyDate"], "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString();
                var finaldate = DateTime.ParseExact(FinalDetails1["Study Date"], "dd-MMM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture).ToShortDateString();

                //Validate Details in Final details column on archive window should match with details in Dest PACS 36
                if ((MpacResults["PatientName"].Split(' ')[0].ToUpper().Equals(FinalDetails1["Last Name"].ToUpper())) && (MpacResults["PatientName"].Split(' ')[1].ToUpper().Equals(FinalDetails1["First Name"].ToUpper())) &&
                    (MpacResults["Sex"].Equals(FinalDetails1["Gender"])) && (inbounds.CompareDates(MpacResults["DOB"], FinalDetails1["DOB"], "yyyyMMdd", "dd-MMM-yyyy")) &&
                    (MpacResults["IPID"].Equals(FinalDetails1["Issuer of PID"])) && (MpacResults["PatientID"].Equals(FinalDetails1["PID / MRN"])) &&
                    (MpacResults["StudyDescription"].Equals(FinalDetails1["Description"])) && mpacdate.Equals(finaldate) &&
                    (MpacResults["Accession"].Equals(FinalDetails1["Accession"])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout from Dest PACS
                mpaclogin.LogoutPacs();

                ////**************Reset the QuerID tag changed*********************
                //login.DriverGoTo(login.hpurl);
                //hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                //configure = (Configure)hphomepage.Navigate("Configure");

                ////Navigate to Properties browser tab
                //configure.NavigateToTab("properties");

                ////Update QueryID tag as 00100010
                //configure.UpdateQueryIDTags("removealladd", DefaultTag);


                ////Restart Clarc service
                //putty.RestartService();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);


                //Return Result
                return result;
            }

            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);
                login.Logout();

                ////**************Reset the QuerID tag changed*********************
                //login.DriverGoTo(login.hpurl);
                //hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                //configure = (Configure)hphomepage.Navigate("Configure");

                ////Navigate to Properties browser tab
                //configure.NavigateToTab("properties");

                ////Update QueryID tag as 00100010
                //configure.UpdateQueryIDTags("removealladd", DefaultTag);



                ////Restart Clarc service
                //Putty putty = new Putty();
                //putty.RestartService();


                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);


                //Return Result
                return result;

            }

        }

        #endregion Sprint-3 Test Cases

        #region Sprint-4 Test Cases

        /// <summary>
        /// Search Order- Reconciliation Window validation with multiple orders
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_29494(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Inbounds inbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            String mpUsername = Config.pacsadmin;
            String mpPassword = Config.pacspassword;
            String phusername = Config.ph1UserName;
            String phpassword = Config.ph1Password;
            String arusername = Config.ar1UserName;
            String arpassword = Config.ar1Password;
            String hpUserName = Config.hpUserName;
            String hpPassword = Config.hpPassword;

            String patientId = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
            String orderAccession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderAccession");
            String[] OrderAcc = orderAccession.Split(':');
            String Orderpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderFilePath");
            String studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
            String QueryID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "QueryID");
            String NominateReason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NominationReason");
            String[] orderpaths = Orderpath.Split('=');
            String DefaultTag = "";
            try
            {
                login.DriverGoTo(login.hpurl);
                //step 1--Login Holding Pen             
                hphomepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                configure = (Configure)hphomepage.Navigate("Configure");
                ExecutedSteps++;

                //Navigate to Properties browser tab
                configure.NavigateToTab("properties");

                //step 2--Update QueryID tag as 00100010 
                DefaultTag = configure.UpdateQueryIDTags("removealladd", QueryID);
                ExecutedSteps++;

                //step 3--Restart Clarc Service
                Putty putty = new Putty();
                putty.RestartService();
                ExecutedSteps++;

                //Step 4--Sent multiple Orders with different accession number  for a patient to MergePacs#1 server
                ExecutedSteps++;
                foreach (String order in orderpaths)
                {
                    Boolean hl7order = mpaclogin.SendHL7Order(Config.MergePACsIP, int.Parse(Config.mpacport), order);
                    if (hl7order == true)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        throw new Exception("HL7 Order not sent");
                    }
                }

                //Step 5--Send the corresponding study to MergePacs#2 server
                BasePage.RunBatchFile(Config.batchfilepath, studypath + " " + Config.dicomsendpath + " " + Config.StudyPacs);
                ExecutedSteps++;

                //Step 6--Sending study from source mpacs to iConnect                
                login.DriverGoTo(login.mpacstudyurl);
                MpacLogin mplogin = new MpacLogin();
                MPHomePage homepage = new MPHomePage();
                mplogin.Loginpacs(mpUsername, mpPassword);
                Tool tools = (Tool)homepage.NavigateTopMenu("Tools");
                homepage.NavigateTopMenu("Tools");
                tools.NavigateToSendStudy();
                tools.SearchStudy("Accession", Accession, 0);
                tools.MpacSelectStudy("Accession", Accession);
                tools.SendStudy(1);
                ExecutedSteps++;
                mplogin.LogoutPacs();

                login.DriverGoTo(login.url);

                //Step 7--Login as physician
                login.LoginIConnect(phusername, phpassword);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                //PageLoadWait.WaitforReceivingStudy(180, patientId);
                PageLoadWait.WaitforStudyInStatus(Accession, inbounds, "Uploaded");
                PageLoadWait.WaitforUpload(Accession, inbounds);

                //Search and Select Study
                inbounds.SearchStudy("Accession", Accession);

                //Valiadate study is present in Physician's inbounds
                if (inbounds.CheckStudy("Accession", Accession) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study not present");
                }

                //Step 8--Validate status as uploaded 
                String studyState;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyState);

                if (studyState == "Uploading" || studyState == "Uploaded")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                }

                //Step 9 and 10--Nominate study with reason
                inbounds.SelectStudy("Accession", Accession);
                inbounds.NominateForArchive(NominateReason);
                ExecutedSteps++;
                ExecutedSteps++;

                //Step 11--Validate Study status as Nominated for archive 
                String studyStatus2;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyStatus2);
                if (studyStatus2 == "Nominated For Archive")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                }
                login.Logout();

                //Step 12--Login as ar
                login.LoginIConnect(arusername, arpassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("Accession", Accession);

                //Check nominated study in ar's inbounds
                if (inbounds.CheckStudy("Accession", Accession) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                }

                //Step 13--Validate Reconciliation State as Multiple matching Order
                inbounds.SelectStudy("Accession", Accession);
                String studyState4;
                //Validate Reconciliation State as Multiple matching Order
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Reconciliation State", out studyState4);

                if (studyState4 == "Multiple Matching Orders")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 14-- Click on Archive Study
                IWebElement UploadCommentsField, ArchiveOrderField;
                inbounds.ClickArchiveStudy(out UploadCommentsField, out ArchiveOrderField);
                UploadCommentsField.SendKeys("test");
                ArchiveOrderField.SendKeys("");
                if (BasePage.Driver.FindElement(By.CssSelector("#ReconciliationControlDialogDiv")).Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 15--Select Search Order and selectAll Dates and Click on Search
                inbounds.ArchiveSearch("order", "All Dates");

                string s = orderpaths.Length.ToString();
                PageLoadWait.WaitForPageLoad(20);
                string Value = BasePage.Driver.FindElement(By.CssSelector("#m_ReconciliationControl_LabelOrderNumber")).Text;
                String[] value = Value.Split(' ');

                //Verify number of orders listed in matching patients column
                if (value[2] == s)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 16--Validate the details are listed in Matching Order Column with numberings under Find Matching Order.                
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 17--Click on the ShowAll button under the Matching order column and select an order
                inbounds.ShowAllandSelect("Accession", OrderAcc[0]);
                ExecutedSteps++;

                //Validate all matching Orders are listed 
                foreach (String acc in OrderAcc)
                {
                    Dictionary<string, string> Order = inbounds.GetMatchingRowReconcile("Accession", acc);

                    if (Order != null)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        throw new Exception("Order Not found");
                    }
                }

                Dictionary<string, string> rowValues1 = inbounds.GetMatchingRowReconcile("Accession", OrderAcc[0]);
                //Step 18--click on the Ok button
                inbounds.ClickOkInShowAll();
                //Validate the details listed in Matching order column are in sync with order details               
                Dictionary<String, String> MatchingValues = inbounds.GetDataInArchive("Matching Order");
                Dictionary<String, String> OriginalDetails = inbounds.GetDataInArchive("Original Details");

                if ((MatchingValues["Last Name"].Equals(rowValues1["Patient Name"].Split(',')[0].Trim())) &&
                    (MatchingValues["First Name"].Equals(rowValues1["Patient Name"].Split(',')[1].Trim())) &&
                    (MatchingValues["Gender"].Equals(rowValues1["Gender"])) && (MatchingValues["Issuer of PID"].Equals(rowValues1["Issuer of PID"])) &&
                    (MatchingValues["PID / MRN"].Equals(rowValues1["Patient ID"])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                }

                //Set Mandatory details
                inbounds.SetBlankFinalDetailsInArchive();

                // step 19--Validate order details are automatically updated in Final Details column 
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Details in Final details column 
                Dictionary<String, String> FinalDetails = inbounds.GetDataInArchive("Final Details");

                /* if ((MatchingValues["Last Name"].Equals(FinalDetails["Last Name"])) && (MatchingValues["First Name"].Equals(FinalDetails["First Name"])) &&
                     (MatchingValues["Gender"].Equals(FinalDetails["Gender"])) && (MatchingValues["DOB"].Equals(FinalDetails["DOB"])) &&
                     (MatchingValues["Issuer of PID"].Equals(FinalDetails["Issuer of PID"])) && (MatchingValues["PID / MRN"].Equals(FinalDetails["PID / MRN"])) &&
                     (MatchingValues["Description"].Equals(FinalDetails["Description"])) && (MatchingValues["Study Date"].Equals(FinalDetails["Study Date"])) &&
                     (MatchingValues["Accession"].Equals(FinalDetails["Accession"])) &&
                     (OriginalDetails["Prefix"].Equals(FinalDetails["Prefix"])))
                 {
                     result.steps[++ExecutedSteps].status = "Pass";
                     Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                 }
                 else
                 {
                     result.steps[++ExecutedSteps].status = "Fail";
                     Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                     result.steps[ExecutedSteps].SetLogs();
                 }
                 */

                //Step 20--click on archive
                inbounds.ClickArchive();
                ExecutedSteps++;

                //Step 21--Check the Status displayed in the Status column 
                String studyState1;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyState1);

                if (studyState1 == "Archiving" || studyState1 == "Routing Completed")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                }

                //Step 22--Login to the MergePacs#3 and Make sure that the study is archived properly 
                //with the configured Final details in Reconcile/Archive Study Popup.
                login.DriverGoTo(login.mpacdesturl);
                mplogin = new MpacLogin();
                homepage = new MPHomePage();
                mplogin.Loginpacs(mpUsername, mpPassword);
                tools = (Tool)homepage.NavigateTopMenu("Tools");
                homepage.NavigateTopMenu("Tools");
                tools.NavigateToSendStudy();
                tools.SearchStudy("Accession", Accession, 0);

                //Get study details
                Dictionary<string, string> MpacResults = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());

                String date = DateTime.ParseExact(MpacResults["StudyDate"], "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString();
                String date1 = DateTime.ParseExact(FinalDetails["Study Date"], "dd-MMM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture).ToShortDateString();
                //date format in Final details - 17-Jan-2006 8:24:20 AM                

                String dob = DateTime.ParseExact(MpacResults["DOB"], "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString();
                String dob1 = DateTime.ParseExact(FinalDetails["DOB"], "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToShortDateString();

                //Validate Details in Final details column on archive window should match with details in Dest PACS
                if ((MpacResults["PatientName"].Split(' ')[0].ToUpper().Equals(FinalDetails["Last Name"].ToUpper())) && (MpacResults["PatientName"].Split(' ')[1].Equals(FinalDetails["First Name"])) &&
                    (MpacResults["Sex"].Equals(FinalDetails["Gender"])) && dob.Equals(dob1) &&
                    (MpacResults["IPID"].Equals(FinalDetails["Issuer of PID"])) && (MpacResults["PatientID"].Equals(FinalDetails["PID / MRN"])) &&
                    (MpacResults["StudyDescription"].Replace(" ", "").Equals(FinalDetails["Description"].Replace(" ", ""))) && date.Equals(date1) &&
                    (MpacResults["Accession"].Equals(FinalDetails["Accession"])))
                {
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
                mplogin.LogoutPacs();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Reset QueryID Tag
                login.DriverGoTo(login.hpurl);
                //step 1--Login Holding Pen             
                hphomepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                configure = (Configure)hphomepage.Navigate("Configure");

                //Navigate to Properties browser tab
                configure.NavigateToTab("properties");

                //step 2--Update QueryID tag as 00100010 
                configure.UpdateQueryIDTags("removealladd", DefaultTag);

                //step 3--Restart Clarc Service
                putty.RestartService();

                //Return result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.StackTrace);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Reset QueryID Tag
                login.DriverGoTo(login.hpurl);
                //step 1--Login Holding Pen             
                hphomepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                configure = (Configure)hphomepage.Navigate("Configure");

                //Navigate to Properties browser tab
                configure.NavigateToTab("properties");

                //step 2--Update QueryID tag as 00100010 
                configure.UpdateQueryIDTags("removealladd", DefaultTag);

                //step 3--Restart Clarc Service
                Putty putty = new Putty();
                putty.RestartService();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Search Order in Archive study page
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_29495(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Inbounds inbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            String mpUsername = Config.pacsadmin;
            String mpPassword = Config.pacspassword;
            String phusername = Config.ph1UserName;
            String phpassword = Config.ph1Password;
            String arusername = Config.ar1UserName;
            String arpassword = Config.ar1Password;
            String firstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
            String lastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
            String patientId = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
            String orderAccession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderAccession");
            String Orderpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderFilePath");
            String studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
            String[] orderpath = Orderpath.Split('=');
            String[] OrderAcc = orderAccession.Split(':');
            String DefaultTag = "";
            String QueryID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "QueryID");


            login.DriverGoTo(login.hpurl);
            //step 1--Login Holding Pen             
            hphomepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
            configure = (Configure)hphomepage.Navigate("Configure");

            //Navigate to Properties browser tab
            configure.NavigateToTab("properties");

            //step 2--Update QueryID tag as 00100020 
            configure.UpdateQueryIDTags("removealladd", QueryID);

            //step 3--Restart Clarc Service
            Putty putty = new Putty();
            putty.RestartService();

            try
            {
                //Step 1--Sent multiple Orders with different accession number for a patient to MergePacs#1 server.
                ExecutedSteps++;
                foreach (String order in orderpath)
                {
                    Boolean hl7order = mpaclogin.SendHL7Order(Config.MergePACsIP, int.Parse(Config.mpacport), order);
                    if (hl7order == true)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        throw new Exception("HL7 Order not sent");
                    }
                }



                //Step 2--Send the corresponding study to MergePacs#2 server.
                BasePage.RunBatchFile(Config.batchfilepath, studypath + " " + Config.dicomsendpath + " " + Config.StudyPacs);
                ExecutedSteps++;

                login.DriverGoTo(login.hpurl);
                //Login Holding Pen             
                hphomepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                WorkFlow workflow = (WorkFlow)hphomepage.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Queue Worklist");
                workflow.HPCheckOrder(OrderAcc[0]);
                Dictionary<string, string> hpdetails = workflow.GetOrderDetailsInHP();
                hplogin.LogoutHPen();

                //Step 3--Sending study from source mpacs to iConnect                
                login.DriverGoTo(login.mpacstudyurl);
                MpacLogin mplogin = new MpacLogin();
                MPHomePage homepage = new MPHomePage();
                mplogin.Loginpacs(mpUsername, mpPassword);
                Tool tools = (Tool)homepage.NavigateTopMenu("Tools");
                homepage.NavigateTopMenu("Tools");
                tools.NavigateToSendStudy();
                tools.SearchStudy("Accession", Accession, 0);
                tools.MpacSelectStudy("Accession", Accession);
                tools.SendStudy(1);
                ExecutedSteps++;
                mplogin.LogoutPacs();

                login.DriverGoTo(login.url);

                //Step 4--Login as physician
                login.LoginIConnect(phusername, phpassword);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                //PageLoadWait.WaitforReceivingStudy(180, patientId);
                PageLoadWait.WaitforStudyInStatus(Accession, inbounds, "Uploaded");
                PageLoadWait.WaitforUpload(Accession, inbounds);

                //Search and Select Study
                inbounds.SearchStudy("AccessionNo", Accession);
                PageLoadWait.WaitForPageLoad(20);
                if (inbounds.CheckStudy("Accession", Accession) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study not present");
                }
                //Step 5--Validate status as uploaded 
                inbounds.SelectStudy("Accession", Accession);
                String studyState;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyState);

                if (studyState == "Uploading" || studyState == "Uploaded")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                }

                //Step 6 & 7--Nominate study with reason
                inbounds.SelectStudy("Accession", Accession);
                IWebElement OrderField, ReasonField;
                inbounds.ClickNominateButton(out ReasonField, out OrderField);

                if (BasePage.Driver.FindElement(By.CssSelector("#NominateStudyDialogDiv")).Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                OrderField.SendKeys("Testing");
                SelectElement selector = new SelectElement(ReasonField);
                selector.SelectByText("Interpretation Required");
                Logger.Instance.InfoLog("Reason Field is filled Sucessfully");

                //Click Nominate in confirmation window
                inbounds.ClickConfirmNominate();
                ExecutedSteps++;

                //Step 8--Validate Study status as Nominated for archive 
                String studyStatus2;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyStatus2);
                if (studyStatus2 == "Nominated For Archive")
                {
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

                //Step 9--Login as ar
                login.LoginIConnect(arusername, arpassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("AccessionNo", Accession);
                PageLoadWait.WaitForPageLoad(20);
                if (inbounds.CheckStudy("Accession", Accession) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study not present");
                }

                //Step 10--Validate Reconciliation State as Multiple matching Order
                inbounds.SelectStudy("Accession", Accession);
                String studyState4;
                //Validate Reconciliation State as Multiple matching Order
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Reconciliation State", out studyState4);

                if (studyState4 == "Multiple Matching Orders")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 11-- Click on Archive Study
                IWebElement UploadCommentsField, ArchiveOrderField;
                inbounds.ClickArchiveStudy(out UploadCommentsField, out ArchiveOrderField);
                UploadCommentsField.SendKeys("test");
                ArchiveOrderField.SendKeys("");
                if (BasePage.Driver.FindElement(By.CssSelector("#ReconciliationControlDialogDiv")).Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 12 -click the search button and validate if the search parameters are displayed
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 13--Select Search Order and selectAll Dates and Click on Search
                inbounds.ArchiveSearch("order", "All Dates");
                string s = orderpath.Length.ToString();
                PageLoadWait.WaitForPageLoad(20);
                string Value = BasePage.Driver.FindElement(By.CssSelector("#m_ReconciliationControl_LabelOrderNumber")).Text;
                String[] value = Value.Split(' ');
                if (value[2] == s)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 14--Validate the details are listed in Matching Order Column with numberings under Find Matching Order.

                result.steps[++ExecutedSteps].status = "Not Automated";


                //Step 15               
                //inbounds.ArchiveSearch("order", "", "", "", "", "", patientId[1], "", accession[1], "");
                IWebElement lastname = BasePage.Driver.FindElement(By.CssSelector("#m_ReconciliationControl_TextboxLastName_Find"));
                IWebElement firstname1 = BasePage.Driver.FindElement(By.CssSelector("#m_ReconciliationControl_TextboxFirstName_Find"));
                IWebElement pid = BasePage.Driver.FindElement(By.CssSelector("#m_ReconciliationControl_TextboxPID_Find"));
                IWebElement accession1 = BasePage.Driver.FindElement(By.CssSelector("#m_ReconciliationControl_TextboxAccession_Find"));
                inbounds.ArchiveSearch("order", "All Dates");
                PageLoadWait.WaitForLoadInArchive(20);
                lastname.Clear(); firstname1.Clear();
                pid.SendKeys(patientId);
                accession1.SendKeys(OrderAcc[0]);
                inbounds.ClickButton("#m_ReconciliationControl_ButtonSearch");
                PageLoadWait.WaitForLoadInArchive(20);

                Dictionary<String, String> MatchingValues = inbounds.GetDataInArchive("Matching Order");
                if ((MatchingValues["Last Name"].Equals(hpdetails["Patient Name"].Split(',')[0].Trim())) && (MatchingValues["First Name"].Equals(hpdetails["Patient Name"].Split(',')[1].Trim())) &&
                    (MatchingValues["Gender"].Equals(hpdetails["Patient's Sex"])) && inbounds.CompareDates(MatchingValues["DOB"], hpdetails["Patient's Birth Date"]) &&
                    (MatchingValues["Issuer of PID"].Equals(hpdetails["IssuerOfPatientID"])) && (MatchingValues["PID / MRN"].Equals(hpdetails["Patient ID"])) &&
                    (MatchingValues["Description"].Equals(hpdetails["Requested Procedure Description"])) && inbounds.CompareDates(MatchingValues["Study Date"], hpdetails["Scheduled Start Date"]) &&
                    (MatchingValues["Accession"].Equals(hpdetails["Accession Number"])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                inbounds.SetBlankFinalDetailsInArchive();
                //Step 16--click on archive
                Dictionary<String, String> FinalDetails1 = inbounds.GetDataInArchive("Final Details");
                inbounds.ClickArchive();
                ExecutedSteps++;

                //Step 17--Login to MergePacs#3 and Make sure that the study is archived properly as of Final details in Reconcile
                login.DriverGoTo(login.mpacdesturl);
                mplogin = new MpacLogin();
                homepage = new MPHomePage();
                mplogin.Loginpacs(mpUsername, mpPassword);
                tools = (Tool)homepage.NavigateTopMenu("Tools");
                homepage.NavigateTopMenu("Tools");
                tools.NavigateToSendStudy();
                tools.SearchStudy("Accession", Accession, 0);

                //Get study details
                Dictionary<string, string> MpacResults = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());

                //Comparing study date and DOB
                String mpacdate1 = DateTime.ParseExact(MpacResults["StudyDate"], "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString();
                String studydate1 = DateTime.ParseExact(FinalDetails1["Study Date"], "dd-MMM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture).ToShortDateString();
                Boolean DOBdate = login.CompareDates(MpacResults["DOB"], FinalDetails1["DOB"], "yyyyMMdd", "dd-MMM-yyyy");

                //Validate Details in Final details column on archive window should match with details in Dest PACS
                if ((MpacResults["PatientName"].ToUpper().Split(' ')[0].Equals(FinalDetails1["Last Name"].ToUpper())) &&
                    (MpacResults["PatientName"].ToUpper().Split(' ')[1].Equals(FinalDetails1["First Name"].ToUpper())) &&
                    (MpacResults["Sex"].Equals(FinalDetails1["Gender"])) && mpacdate1.Equals(studydate1) &&
                    (MpacResults["IPID"].Equals(FinalDetails1["Issuer of PID"])) && (MpacResults["PatientID"].Equals(FinalDetails1["PID / MRN"])) &&
                    (MpacResults["StudyDescription"].Replace(" ", "").Equals(FinalDetails1["Description"].Replace(" ", ""))) && DOBdate == true &&
                    (MpacResults["Accession"].Equals(FinalDetails1["Accession"])))
                {
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
                mplogin.LogoutPacs();
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.StackTrace);

                login.DriverGoTo(login.hpurl);
                //step 1--Login Holding Pen             
                hphomepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                configure = (Configure)hphomepage.Navigate("Configure");

                //Navigate to Properties browser tab
                configure.NavigateToTab("properties");

                //step 2--Update QueryID tag as 00100020 
                configure.UpdateQueryIDTags("removealladd", DefaultTag);

                //step 3--Restart Clarc Service

                putty.RestartService();
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
        /// Search Patient in Archive study page based on Lastname
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_29496(String testid, String teststeps, int stepcount)
            {
                //Declare and initialize variables
                Inbounds inbounds = null;
                TestCaseResult result;
                int ExecutedSteps = -1;
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String mpUsername = Config.pacsadmin;
                String mpPassword = Config.pacspassword;
                String phusername = Config.ph1UserName;
                String phpassword = Config.ph1Password;
                String arusername = Config.ar1UserName;
                String arpassword = Config.ar1Password;

                String firstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String lastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String PatientId = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String Studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String[] firstname = firstName.Split(':');
                String[] studypath = Studypath.Split('=');
                String[] patientId = PatientId.Split(':');


                try
                {
                    //Import the Studies to MergePacs#3 Server --Step-1
                    BasePage.RunBatchFile(Config.batchfilepath, studypath[1] + " " + Config.dicomsendpath + " " + Config.DestinationPACS);
                    BasePage.RunBatchFile(Config.batchfilepath, studypath[2] + " " + Config.dicomsendpath + " " + Config.DestinationPACS);
                    ExecutedSteps++;

                    //Import the Study John Smith to MergePacs#2 Server--Step-2
                    BasePage.RunBatchFile(Config.batchfilepath, studypath[0] + " " + Config.dicomsendpath + " " + Config.StudyPacs);
                    ExecutedSteps++;

                    //Sending study from source mpacs to iConnect --Step-3           
                    login.DriverGoTo(login.mpacstudyurl);
                    MpacLogin mplogin = new MpacLogin();
                    MPHomePage homepage = new MPHomePage();
                    mplogin.Loginpacs(mpUsername, mpPassword);
                    Tool tools = (Tool)homepage.NavigateTopMenu("Tools");
                    homepage.NavigateTopMenu("Tools");
                    tools.NavigateToSendStudy();
                    tools.SearchStudy("Patient Name", lastName, 0);
                    tools.MpacSelectStudy("Accession", accession);
                    tools.SendStudy(1);
                    mplogin.LogoutPacs();
                    ExecutedSteps++;

                    //Login as physician --step-4
                    login.DriverGoTo(login.url);
                    login.LoginIConnect(phusername, phpassword);
                    //Navigate to Inbounds
                    inbounds = (Inbounds)login.Navigate("Inbounds");
                    PageLoadWait.WaitforReceivingStudy(180, patientId[0]);
                    PageLoadWait.WaitforUpload(accession, inbounds);
                    inbounds.SearchStudy("AccessionNo", accession);
                    if (inbounds.CheckStudy("Patient ID", patientId[0]) == true)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        throw new Exception("Study not present");
                    }


                    //Step--5 Select and Nominate the study 
                    inbounds.SelectStudy("Accession", accession);
                    IWebElement OrderField, ReasonField;
                    inbounds.ClickNominateButton(out ReasonField, out OrderField);

                    if (BasePage.Driver.FindElement(By.CssSelector("#NominateStudyDialogDiv")).Displayed == true)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Step--6 choosing archive reason as Interpretation Required.
                    OrderField.SendKeys("Testing");
                    SelectElement selector = new SelectElement(ReasonField);
                    selector.SelectByText("Interpretation Required");
                    Logger.Instance.InfoLog("Reason Field is filled Sucessfully");

                    //Click Nominate in confirmation window
                    inbounds.ClickConfirmNominate();
                    ExecutedSteps++;
                    login.Logout();

                    //Step-7 Login as ar and Examine whether the nominated studies are listed in the Inbounds page
                    login.LoginIConnect(arusername, arpassword);
                    inbounds = (Inbounds)login.Navigate("Inbounds");
                    inbounds.SearchStudy("Accession", accession);
                    PageLoadWait.WaitForPageLoad(20);
                    var study = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { accession, "Nominated For Archive" });
                    if (study != null)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Step-8 -- Archive the study (& click arrow button)
                    inbounds.SelectStudy1("Accession", accession);
                    IWebElement UploadCommentsField, ArchiveOrderField;
                    inbounds.ClickArchiveStudy(out UploadCommentsField, out ArchiveOrderField);
                    UploadCommentsField.SendKeys("test");
                    ArchiveOrderField.SendKeys("");
                    if (BasePage.Driver.FindElement(By.CssSelector("#ReconciliationControlDialogDiv")).Displayed == true)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Step 9--click arrow button                
                    result.steps[++ExecutedSteps].status = "Not Automated";

                    //Step-10
                    inbounds.ArchiveSearch("patient", lastName, "", "", "", "", "", "", "", "");
                    PageLoadWait.WaitForPageLoad(20);
                    string Value = BasePage.Driver.FindElement(By.CssSelector("#m_ReconciliationControl_LabelOrderNumber")).Text;
                    String[] value = Value.Split(' ');
                    if ((value[2].Contains('0')) == false)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Step-11 Click show all and select the patient
                    inbounds.ShowAllandSelect("Patient ID", patientId[1], "patient");
                    if (BasePage.Driver.FindElement(By.CssSelector("#ReconciliationFindPatientControlDialogDiv")).Displayed == true)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //string ipid;
                    //inbounds.GetMatchingRowReconcile("Patient ID", patientId).TryGetValue("Issuer of PID", out ipid);

                    Dictionary<string, string> rowValues = inbounds.GetMatchingRowReconcile("Patient ID", patientId[1]);

                    //Step 12--Click ok in ShowAll and validate details in matching patient column and Final details column
                    inbounds.ClickOkInShowAll();
                    Dictionary<String, String> FinalDetails = inbounds.GetDataInArchive("Final Details");
                    Dictionary<String, String> OriginalDetails = inbounds.GetDataInArchive("Original Details");
                    Dictionary<String, String> MatchingValues = inbounds.GetDataInArchive("Matching Order");
                    ExecutedSteps++;
                    //Comparing Matching Patient Column and Final Detail column
                    if ((MatchingValues["Last Name"].Equals(rowValues["Patient Name"].Split(',')[0].Trim())) &&
                        (MatchingValues["First Name"].Equals(rowValues["Patient Name"].Split(',')[1].Trim())) &&
                        (MatchingValues["Gender"].Equals(rowValues["Gender"])) && (MatchingValues["Issuer of PID"].Equals(rowValues["Issuer of PID"])) &&
                        (MatchingValues["PID / MRN"].Equals(rowValues["Patient ID"]))
                        && (MatchingValues["DOB"].Equals(rowValues["Patient DOB"])) && (FinalDetails["Last Name"].ToUpper().Equals(OriginalDetails["Last Name"])))
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

                    //Step-13 Edit First name in Final Details Column                
                    inbounds.SetCheckBoxInArchive("original details", "first");

                    inbounds.SetBlankFinalDetailsInArchive();
                    Dictionary<String, String> OriginalValues = inbounds.GetDataInArchive("Original Details");
                    Dictionary<String, String> FinalDetails1 = inbounds.GetDataInArchive("Final Details");
                    ExecutedSteps++;
                    
                    if (FinalDetails1["First Name"].Equals(OriginalValues["First Name"]))
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

                    //click on archive--Step-14
                    inbounds.ClickArchive();
                    ExecutedSteps++;
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    login.Logout();

                    //Step-15
                    //Login to MergePacs#3 and Make sure that the study is archived properly as of Final details in Reconcile
                    login.DriverGoTo(login.mpacdesturl);
                    mplogin = new MpacLogin();
                    homepage = new MPHomePage();
                    mplogin.Loginpacs(mpUsername, mpPassword);
                    tools = (Tool)homepage.NavigateTopMenu("Tools");
                    homepage.NavigateTopMenu("Tools");
                    tools.NavigateToSendStudy();
                    tools.SearchStudy("Accession", accession, 0);

                    Dictionary<string, string> MpacResults = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());

                    //Comparing study date and DOB
                    String mpacdate1 = DateTime.ParseExact(MpacResults["StudyDate"], "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString();
                    String studydate1 = DateTime.ParseExact(FinalDetails1["Study Date"], "dd-MMM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture).ToShortDateString();
                    Boolean DOBdate = login.CompareDates(MpacResults["DOB"], FinalDetails1["DOB"], "yyyyMMdd", "dd-MMM-yyyy");

                    //Validate Details in Final details column on archive window should match with details in Dest PACS
                    if ((MpacResults["PatientName"].ToUpper().Split(' ')[0].Equals(FinalDetails1["Last Name"].ToUpper())) &&
                        (MpacResults["PatientName"].ToUpper().Split(' ')[1].Equals(FinalDetails1["First Name"].ToUpper())) &&
                        (MpacResults["Sex"].Equals(FinalDetails1["Gender"])) && mpacdate1.Equals(studydate1) &&
                        (MpacResults["IPID"].Equals(FinalDetails1["Issuer of PID"])) && (MpacResults["PatientID"].Equals(FinalDetails1["PID / MRN"])) &&
                        (MpacResults["StudyDescription"].ToUpper().Equals(FinalDetails1["Description"].ToUpper())) && DOBdate == true &&
                        (MpacResults["Accession"].Equals(FinalDetails1["Accession"])))
                    {
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
                    mplogin.LogoutPacs();
                    //Report Result
                    result.FinalResult(ExecutedSteps);
                    Logger.Instance.InfoLog("Overall Test status--" + result.status);
                    return result;

                }
                catch (Exception e)
                {

                    //Log exception
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
            /// Search Patient in Archive study page based on Patient Demographics(based on first name)
            /// </summary>
            /// <param name="testid"></param>
            /// <param name="teststeps"></param>
            /// <param name="stepcount"></param>
            /// <returns></returns>
            public TestCaseResult Test1_29497(String testid, String teststeps, int stepcount)
            {
                //Declare and initialize variables
                Inbounds inbounds = null;
                int ExecutedSteps = -1;
                TestCaseResult result;
                result = new TestCaseResult(stepcount);
                //Set up Validation Steps
                result.SetTestStepDescription(teststeps);

                String mpUsername = Config.pacsadmin;
                String mpPassword = Config.pacspassword;
                String phusername = Config.ph1UserName;
                String phpassword = Config.ph1Password;
                String arusername = Config.ar1UserName;
                String arpassword = Config.ar1Password;

                String firstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String lastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String patientId = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String Studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String[] firstname = firstName.Split(':');
                String[] patientid = patientId.Split(':');
                String[] lastname = lastName.Split(':');
                String[] studypath = Studypath.Split('=');

                try
                {

                    //Import the Studies to MergePacs#3 Server --Step-1
                    BasePage.RunBatchFile(Config.batchfilepath, studypath[1] + " " + Config.dicomsendpath + " " + Config.DestinationPACS);
                    BasePage.RunBatchFile(Config.batchfilepath, studypath[2] + " " + Config.dicomsendpath + " " + Config.DestinationPACS);
                    ExecutedSteps++;

                    //Import the Study John Smith to MergePacs#2 Server--Step-2
                    BasePage.RunBatchFile(Config.batchfilepath, studypath[0] + " " + Config.dicomsendpath + " " + Config.StudyPacs);
                    ExecutedSteps++;

                    //Sending study from source mpacs to iConnect --Step-3
                    login.DriverGoTo(login.mpacstudyurl);
                    MpacLogin mplogin = new MpacLogin();
                    MPHomePage homepage = new MPHomePage();
                    mplogin.Loginpacs(mpUsername, mpPassword);
                    Tool tools = (Tool)homepage.NavigateTopMenu("Tools");
                    homepage.NavigateTopMenu("Tools");
                    tools.NavigateToSendStudy();
                    tools.SearchStudy("Accession", Accession, 0);
                    tools.MpacSelectStudy("Accession", Accession);
                    tools.SendStudy(1);
                    ExecutedSteps++;
                    mplogin.LogoutPacs();

                    //Login as physician --step-4
                    login.DriverGoTo(login.url);
                    login.LoginIConnect(phusername, phpassword);
                    //Navigate to Inbounds
                    inbounds = (Inbounds)login.Navigate("Inbounds");
                    PageLoadWait.WaitforReceivingStudy(180, patientid[0]);
                    PageLoadWait.WaitforUpload(Accession, inbounds);
                    inbounds.SearchStudy("AccessionNo", Accession);
                    if (inbounds.CheckStudy("Patient ID", patientid[0]) == true)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        throw new Exception("Study not present");
                    }

                    //Step--5 Select and Nominate the study
                    inbounds.SelectStudy("Accession", Accession);
                    IWebElement OrderField, ReasonField;
                    inbounds.ClickNominateButton(out ReasonField, out OrderField);

                    if (BasePage.Driver.FindElement(By.CssSelector("#NominateStudyDialogDiv")).Displayed == true)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Step--6 choosing archive reason as Interpretation Required.
                    OrderField.SendKeys("Testing");
                    SelectElement selector = new SelectElement(ReasonField);
                    selector.SelectByText("Interpretation Required");
                    Logger.Instance.InfoLog("Reason Field is filled Sucessfully");

                    //Click Nominate in confirmation window
                    inbounds.ClickConfirmNominate();
                    //Find Study Status 
                    String studyStatus6;
                    inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyStatus6);
                    String statusReason;
                    inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status Reason", out statusReason);

                    //Validate Study status as Nominated for archive 
                    if (studyStatus6.Equals("Nominated For Archive"))
                    {
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


                    //Login as ar--Step-7
                    login.LoginIConnect(arusername, arpassword);
                    inbounds = (Inbounds)login.Navigate("Inbounds");
                    inbounds.SearchStudy("Accession", Accession);
                    PageLoadWait.WaitForPageLoad(20);
                    if (inbounds.CheckStudy("Patient ID", patientid[0]) == true)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Step-8 -- Archive the study (& click arrow button)
                    inbounds.SelectStudy1("Accession", Accession);
                    IWebElement UploadCommentsField, ArchiveOrderField;
                    inbounds.ClickArchiveStudy(out UploadCommentsField, out ArchiveOrderField);
                    UploadCommentsField.SendKeys("test");
                    ArchiveOrderField.SendKeys("");
                    if (BasePage.Driver.FindElement(By.CssSelector("#ReconciliationControlDialogDiv")).Displayed == true)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Step 9 -click the search button and validate if the search parameters are displayed
                    result.steps[++ExecutedSteps].status = "Not Automated";

                    //Step-10
                    inbounds.ArchiveSearch("patient", "", firstname[0], "", "", "", "", "", "", "");
                    PageLoadWait.WaitForPageLoad(20);
                    string Value = BasePage.Driver.FindElement(By.CssSelector("#m_ReconciliationControl_LabelOrderNumber")).Text;
                    String[] value = Value.Split(' ');
                    if ((value[2].Contains('0')) == false)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Step-11 Click show all and select the patient
                    inbounds.ShowAllandSelect("Patient ID", patientid[1], "patient");
                    if (BasePage.Driver.FindElement(By.CssSelector("#ReconciliationFindPatientControlDialogDiv")).Displayed == true)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    Dictionary<string, string> rowValues = inbounds.GetMatchingRowReconcile("Patient ID", patientid[1]);

                    //Step 12--Click ok in ShowAll and validate details in matching patient column and Final details column
                    inbounds.ClickOkInShowAll();
                    Dictionary<String, String> FinalDetails = inbounds.GetDataInArchive("Final Details");
                    Dictionary<String, String> MatchingValues = inbounds.GetDataInArchive("Matching Patient");
                    
                    //Comparing Matching Patient Column and Final Details Column
                    result.steps[++ExecutedSteps].status = "Not Automated";
                    /*
                    if ((MatchingValues["Last Name"].Equals(rowValues["Patient Name"].Split(',')[0].Trim())) &&
                        (MatchingValues["First Name"].Equals(rowValues["Patient Name"].Split(',')[1].Trim())) &&
                        (MatchingValues["Gender"].Equals(rowValues["Gender"])) && (MatchingValues["Issuer of PID"].Equals(rowValues["Issuer of PID"])) &&
                        (MatchingValues["PID / MRN"].Equals(rowValues["Patient ID"]))&& (MatchingValues["DOB"].Equals(rowValues["Patient DOB"])) &&
                        (FinalDetails["First Name"].Equals(MatchingValues["First Name"])))
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
                    */
                    //edit--Step-13
                    inbounds.EditFinalDetailsInArchive("first name", firstname[1]);
                    ExecutedSteps++;

                    inbounds.SetBlankFinalDetailsInArchive();
                    Dictionary<String, String> FinalDetails1 = inbounds.GetDataInArchive("Final Details");

                    //click on archive--step-14
                    inbounds.ClickArchive();
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    ExecutedSteps++;
                    login.Logout();


                    //Step-15
                    //Login to MergePacs#3 and Make sure that the study is archived properly as of Final details in Reconcile
                    login.DriverGoTo(login.mpacdesturl);
                    mplogin = new MpacLogin();
                    homepage = new MPHomePage();
                    mplogin.Loginpacs(mpUsername, mpPassword);
                    tools = (Tool)homepage.NavigateTopMenu("Tools");
                    homepage.NavigateTopMenu("Tools");
                    tools.NavigateToSendStudy();
                    tools.SearchStudy("Accession", Accession, 0);
                    Dictionary<string, string> MpacResults = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());

                    //Comparing study date and DOB
                    String mpacdate1 = DateTime.ParseExact(MpacResults["StudyDate"], "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString();
                    String studydate1 = DateTime.ParseExact(FinalDetails1["Study Date"], "dd-MMM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture).ToShortDateString();
                    Boolean DOBdate = login.CompareDates(MpacResults["DOB"], FinalDetails1["DOB"], "yyyyMMdd", "dd-MMM-yyyy");

                    //Validate Details in Final details column on archive window should match with details in Dest PACS
                    if ((MpacResults["PatientName"].ToUpper().Split(' ')[0].Equals(FinalDetails1["Last Name"].ToUpper())) && 
                        (MpacResults["PatientName"].ToUpper().Split(' ')[1].Equals(FinalDetails1["First Name"].ToUpper())) &&
                        (MpacResults["Sex"].Equals(FinalDetails1["Gender"])) && mpacdate1.Equals(studydate1) &&
                        (MpacResults["IPID"].Equals(FinalDetails1["Issuer of PID"])) && (MpacResults["PatientID"].Equals(FinalDetails1["PID / MRN"])) &&
                        (MpacResults["StudyDescription"].Equals(FinalDetails1["Description"])) && DOBdate == true &&
                        (MpacResults["Accession"].Equals(FinalDetails1["Accession"])))
                    {
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
                    mplogin.LogoutPacs();

                    //Report Result
                    result.FinalResult(ExecutedSteps);
                    Logger.Instance.InfoLog("Overall Test status--" + result.status);
                    return result;


                }
                catch (Exception e)
                {
                    //Log exception
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
        /// Search Patient in Archive study page based on Patient Demographics(based on MRN and ipid)
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test2_29497(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Inbounds inbounds = null;
            int ExecutedSteps = -1;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            String mpUsername = Config.pacsadmin;
            String mpPassword = Config.pacspassword;
            String phusername = Config.ph1UserName;
            String phpassword = Config.ph1Password;
            String arusername = Config.ar1UserName;
            String arpassword = Config.ar1Password;

            String firstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
            String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
            String patientId = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
            String Studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
            String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
            String[] firstname = firstName.Split(':');
            String[] patientid = patientId.Split(':');
            String[] lastname = LastName.Split(':');
            String[] studypath = Studypath.Split('=');
            try
            {
                //Import the Studies to MergePacs#3 Server --Step-1
                BasePage.RunBatchFile(Config.batchfilepath, studypath[0] + " " + Config.dicomsendpath + " " + Config.DestinationPACS);
                BasePage.RunBatchFile(Config.batchfilepath, studypath[1] + " " + Config.dicomsendpath + " " + Config.DestinationPACS);
                ExecutedSteps++;

                //Import the Study John Smith to MergePacs#2 Server--Step-2
                BasePage.RunBatchFile(Config.batchfilepath, studypath[2] + " " + Config.dicomsendpath + " " + Config.StudyPacs);
                ExecutedSteps++;

                //Sendding study from source mpacs to iConnect --Step-3
                login.DriverGoTo(login.mpacstudyurl);
                MpacLogin mplogin = new MpacLogin();
                MPHomePage homepage = new MPHomePage();
                mplogin.Loginpacs(mpUsername, mpPassword);
                Tool tools = (Tool)homepage.NavigateTopMenu("Tools");
                homepage.NavigateTopMenu("Tools");
                tools.NavigateToSendStudy();
                tools.SearchStudy("Accession", Accession, 0);
                tools.MpacSelectStudy("Accession", Accession);
                tools.SendStudy(1);
                ExecutedSteps++;
                mplogin.LogoutPacs();
                IWebElement UploadCommentsField, ArchiveOrderField;
                login.DriverGoTo(login.url);

                //Login as physician --step-4
                login.LoginIConnect(phusername, phpassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search and Select Study
                PageLoadWait.WaitforStudyInStatus(Accession, inbounds, "Uploaded");
                //PageLoadWait.WaitforReceivingStudy(180, patientid[2]);
                PageLoadWait.WaitforUpload(Accession, inbounds);

                inbounds.SearchStudy("AccessionNo", Accession);
                PageLoadWait.WaitForPageLoad(20);
                if (inbounds.CheckStudy("Accession", Accession) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step--5 Select and Nominate the study
                inbounds.SelectStudy1("Accession", Accession);
                inbounds.NominateForArchive("test");
                ExecutedSteps++;
                login.Logout();

                //login as ar--step-6
                login.LoginIConnect(arusername, arpassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("accessionNo", Accession);
                if (inbounds.CheckStudy("Accession", Accession) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study not present");
                }
                PageLoadWait.WaitForPageLoad(20);

                //step--7 click archive button
                inbounds.SelectStudy("Accession", Accession);
                inbounds.ClickArchiveStudy(out UploadCommentsField, out ArchiveOrderField);
                UploadCommentsField.SendKeys("test");
                ArchiveOrderField.SendKeys("");
                if (BasePage.Driver.FindElement(By.CssSelector("#ReconciliationControlDialogDiv")).Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step--8 click the search button and validate if the search parameters are displayed
                inbounds.ArchiveSearch("patient", lastname[0], "", "", "", "", "", "", "", "");
                PageLoadWait.WaitForLoadInArchive(10);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#m_ReconciliationControl_TextboxLastName_Find")));
                IWebElement lastname1 = BasePage.Driver.FindElement(By.CssSelector("#m_ReconciliationControl_TextboxLastName_Find"));
                IWebElement firstname1 = BasePage.Driver.FindElement(By.CssSelector("#m_ReconciliationControl_TextboxFirstName_Find"));
                IWebElement gender1 = BasePage.Driver.FindElement(By.CssSelector("#m_ReconciliationControl_DropDownListSex_Find"));
                IWebElement dob1 = BasePage.Driver.FindElement(By.CssSelector("#m_ReconciliationControl_TextboxDOB_Find"));
                IWebElement ipid1 = BasePage.Driver.FindElement(By.CssSelector("#m_ReconciliationControl_TextboxIPID_Find"));
                IWebElement pid1 = BasePage.Driver.FindElement(By.CssSelector("#m_ReconciliationControl_TextboxPID_Find"));
                if (lastname1.Displayed && firstname1.Displayed && gender1.Displayed && dob1.Displayed && ipid1.Displayed
                    && pid1.Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step--9 Clear the value in Lastname tab and enter a value in PID/MRN tab and 
                //click on the Search button
                inbounds.ClearText("cssselector", "#m_ReconciliationControl_TextboxLastName_Find");
                inbounds.ClearText("cssselector", "#m_ReconciliationControl_TextboxFirstName_Find");
                inbounds.ArchiveSearch("patient", "", "", "", "", "", patientid[0], "", "", "");
                inbounds.ShowAllandSelect1("Patient ID", patientid[0]);
                Dictionary<string, string> rowValues = inbounds.GetMatchingRowReconcile("Patient ID", patientid[0]);
                inbounds.ClickOkInShowAll();

                Dictionary<String, String> MatchingValues = inbounds.GetDataInArchive("Matching Patient");
                //Comparing Matching Patient Column
                if ((MatchingValues["Last Name"].Equals(rowValues["Patient Name"].Split(',')[0].Trim())) &&
                    (MatchingValues["First Name"].Equals(rowValues["Patient Name"].Split(',')[1].Trim())) &&
                    (MatchingValues["Gender"].Equals(rowValues["Gender"])) && (MatchingValues["Issuer of PID"].Equals(rowValues["Issuer of PID"])) &&
                    (MatchingValues["PID / MRN"].Equals(rowValues["Patient ID"]))
                    && (MatchingValues["DOB"].Equals(rowValues["Patient DOB"])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //step--10 Clear all the values and enter the value Home in PID Issuer tab and click on the Ok button
                inbounds.ClearText("cssselector", "#m_ReconciliationControl_TextboxLastName_Find");
                inbounds.ClearText("cssselector", "#m_ReconciliationControl_TextboxFirstName_Find");
                inbounds.ClearText("cssselector", "#m_ReconciliationControl_TextboxPID_Find");
                inbounds.ArchiveSearch("patient", "", "", "", "", Config.ipid1, "", "", "", "");
                inbounds.ShowAllandSelect1("Patient ID", patientid[1]);
                Dictionary<string, string> rowValues1 = inbounds.GetMatchingRowReconcile("Patient ID", patientid[1]);
                inbounds.ClickOkInShowAll();

                Dictionary<String, String> MatchingValues1 = inbounds.GetDataInArchive("Matching Patient");
                //Comparing Matching Patient Column
                if ((MatchingValues1["Last Name"].Equals(rowValues1["Patient Name"].Split(',')[0].Trim())) &&
                    (MatchingValues1["First Name"].Equals(rowValues1["Patient Name"].Split(',')[1].Trim())) &&
                    (MatchingValues1["Gender"].Equals(rowValues1["Gender"])) && (MatchingValues1["Issuer of PID"].Equals(rowValues1["Issuer of PID"])) &&
                    (MatchingValues1["PID / MRN"].Equals(rowValues1["Patient ID"]))
                    && (MatchingValues1["DOB"].Equals(rowValues1["Patient DOB"])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Close the Reconcile\Archive study pop up window--step-11
                inbounds.ClickCancelArchive();
                if (BasePage.Driver.FindElement(By.CssSelector("#ReconciliationControlDialogDiv")).Displayed == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //click archivestudy--step-12
                inbounds.SelectStudy1("Accession", Accession);
                inbounds.ClickArchiveStudy(out UploadCommentsField, out ArchiveOrderField);
                UploadCommentsField.SendKeys("test");
                ArchiveOrderField.SendKeys("");
                if (BasePage.Driver.FindElement(By.CssSelector("#ReconciliationControlDialogDiv")).Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //edit and update values in final details column--step-13
                //inbounds.EditFinalDetailsInArchive("gender", "M");
                inbounds.EditFinalDetailsInArchive("dob", "23-Dec-1889");
                inbounds.EditFinalDetailsInArchive("ipid", "Merge");
                inbounds.EditFinalDetailsInArchive("pid", "672012");
                inbounds.EditFinalDetailsInArchive("description", "Test");
                ExecutedSteps++;
                Dictionary<String, String> FinalDetails1 = inbounds.GetDataInArchive("Final Details");
                //click on archive--step 14
                inbounds.ClickArchive();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;
                login.Logout();

                //step 15--Login to MergePacs#3 and Make sure that the study is archived properly as of Final details in Reconcile
                login.DriverGoTo(login.mpacdesturl);
                mplogin = new MpacLogin();
                homepage = new MPHomePage();
                mplogin.Loginpacs(mpUsername, mpPassword);
                tools = (Tool)homepage.NavigateTopMenu("Tools");
                homepage.NavigateTopMenu("Tools");
                tools.NavigateToSendStudy();
                tools.SearchStudy("Accession", Accession, 0);
                Dictionary<string, string> MpacResults = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());

                String date = DateTime.ParseExact(MpacResults["StudyDate"], "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString();
                String date1 = DateTime.ParseExact(FinalDetails1["Study Date"], "dd-MMM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture).ToShortDateString();

                //Validate Details in Final details column on archive window should match with details in Dest PACS
                if ((MpacResults["PatientName"].Split(' ')[0].Equals(FinalDetails1["Last Name"])) && (MpacResults["PatientName"].Split(' ')[1].Equals(FinalDetails1["First Name"])) &&
                    (MpacResults["Sex"].Equals(FinalDetails1["Gender"])) &&
                    (MpacResults["IPID"].Equals(FinalDetails1["Issuer of PID"])) && (MpacResults["PatientID"].Equals(FinalDetails1["PID / MRN"])) &&
                    (MpacResults["StudyDescription"].Replace(" ","").Equals(FinalDetails1["Description"].Replace(" ", ""))) && date.Equals(date1) &&
                    (MpacResults["Accession"].Equals(FinalDetails1["Accession"])))
                {
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
                mplogin.LogoutPacs();
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;

            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.StackTrace);

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
        /// Reconcile window validation with matching order
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test3_29497(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Inbounds inbounds = null;
            TestCaseResult result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set steps description
            result.SetTestStepDescription(teststeps);

            try
            {
                //User Details
                String phusername = Config.ph1UserName;
                String phpassword = Config.ph1Password;
                String arusername = Config.ar1UserName;
                String arpassword = Config.ar1Password;
                String mpUsername = Config.pacsadmin;
                String mpPassword = Config.pacspassword;

                //Data Details
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String Studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String orderAccession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderAccession");
                String Orderpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderFilePath");
                String NominateReason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NominationReason");

                //Upload a Dicom Study -- step 1
                ei.EIDicomUpload(Config.stUserName, Config.stPassword, Config.Dest1, Studypath);

                //Send a matching HL7 order
                Boolean hl7order = mpaclogin.SendHL7Order(Config.MergePACsIP, int.Parse(Config.mpacport), Orderpath);
                ExecutedSteps++;

                //Login as physician
                login.DriverGoTo(login.url);
                login.LoginIConnect(phusername, phpassword);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study
                inbounds.SearchStudy("Accession", Accession);

                //Validate study is listed in ph inbounds - step 2
                if (inbounds.CheckStudy("Accession", Accession) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study not uploaded");
                }

                //Select study
                inbounds.SelectStudy("Accession", Accession);

                //Click Nominate for archive button - step 3
                IWebElement OrderField, ReasonField;
                inbounds.ClickNominateButton(out ReasonField, out OrderField);

                if (BasePage.Driver.FindElement(By.CssSelector("#NominateStudyDialogDiv")).Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select reason field
                OrderField.SendKeys("Testing");
                SelectElement selector = new SelectElement(ReasonField);
                selector.SelectByText(NominateReason);
                Logger.Instance.InfoLog("Reason Field is filled Sucessfully");

                //Click Nominate in confirmation window -step 4
                inbounds.ClickConfirmNominate();
                ExecutedSteps++;

                //Validate study status as Nominated for archive
                Dictionary<string, string> study = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Nominated For Archive" });
                if (study != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as Physician
                login.Logout();

                //Login as archivist
                login.LoginIConnect(arusername, arpassword);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study
                inbounds.SearchStudy("Accession", Accession);

                //Validate Study is listed in archivist inbounds - step 6
                if (inbounds.CheckStudy("Accession", Accession) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select study
                inbounds.SelectStudy("Accession", Accession);

                //Click Archive study button
                inbounds.ClickArchiveStudy("", "");

                //Validate Archive/Reconcile Study dialog is opened - step 7
                if (BasePage.Driver.FindElement(By.CssSelector("#ReconciliationControlDialogDiv")).Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Search order in archive window
                inbounds.ArchiveSearch("order", "", "", "", "", "", "", "", orderAccession, "All Dates");

                //Get details in Matching patient column
                Dictionary<String, String> MatchingValues = inbounds.GetDataInArchive("Matching Order");

                //Verify the Order searched by using Order Accession number- step 8
                if ((MatchingValues["Accession"].Equals(orderAccession)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Get details in Original Details column and Final Details column
                Dictionary<String, String> OriginalDetails = inbounds.GetDataInArchive("Original Details");
                Dictionary<String, String> FinalDetails = inbounds.GetDataInArchive("Final Details");

                var OriginalDiff = OriginalDetails.Where(entry => MatchingValues[entry.Key] == entry.Value)
             .ToDictionary(entry => entry.Key, entry => entry.Value);

                //Step 9 - Compare fields having same value in matching order and original details with final details column- step 9
                ExecutedSteps++;
                foreach (String Key in OriginalDiff.Keys)
                {
                    if (FinalDetails[Key].Equals(MatchingValues[Key]))
                    {

                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        break;
                    }
                }

                var MatchingDiff = OriginalDetails.Where(entry => MatchingValues[entry.Key] != entry.Value)
             .ToDictionary(entry => entry.Key, entry => entry.Value);

                //Step 10 - Validate no values are listed in final details fields with different value in matching order and original details
                ExecutedSteps++;
                foreach (String Key in MatchingDiff.Keys)
                {
                    if (MatchingValues[Key].Equals("") || OriginalDetails[Key].Equals(""))
                    {
                        continue;
                    }
                    if (FinalDetails[Key].Equals(""))
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        break;
                    }
                }

                var MatchingEmpty = OriginalDetails.Where(entry => MatchingValues[entry.Key] == "")
             .ToDictionary(entry => entry.Key, entry => entry.Value);

                //Step 11 - Validate fields having value in original detail with no value in matching order are entered with original details value automatically
                ExecutedSteps++;
                foreach (String Key in MatchingEmpty.Keys)
                {
                    if (MatchingValues[Key].Equals("") && OriginalDetails[Key].Equals(""))
                    {
                        continue;
                    }
                    if (FinalDetails[Key].Equals(OriginalDetails[Key]))
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        break;
                    }
                }

                var OriginalEmpty = MatchingValues.Where(entry => OriginalDetails[entry.Key] == "")
             .ToDictionary(entry => entry.Key, entry => entry.Value);

                //Step 12 - Validate fields having value in matching order with no value in matching order are entered with matching order value automatically
                ExecutedSteps++;
                foreach (String Key in OriginalEmpty.Keys)
                {
                    if (MatchingValues[Key].Equals("") && OriginalDetails[Key].Equals(""))
                    {
                        continue;
                    }
                    if (FinalDetails[Key].Equals(MatchingValues[Key]))
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        break;
                    }
                }

                inbounds.SetBlankFinalDetailsInArchive();
                Dictionary<String, String> FinalDetails1 = inbounds.GetDataInArchive("Final Details");

                //step 13 - Click archive in reconcile window
                inbounds.ClickArchive();

                int counter = 0;
                while (true)
                {
                    counter++;
                    //Search study
                    inbounds.SearchStudy("Accession", Accession);

                    Dictionary<string, string> Studyarchived = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Routing Completed" });

                    if (Studyarchived != null)
                        break;

                    if (counter > 4)
                        break;
                }

                //Search study
                inbounds.SearchStudy("Accession", Accession);

                //Validate status is changed to Routing Completed - Step 
                Dictionary<string, string> archivedStudy = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Routing Completed" });

                if (archivedStudy != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout archivist
                login.Logout();

                //Step- 14
                //Login to MergePacs#3 and Make sure that the study is archived properly as of Final details in Reconcile
                login.DriverGoTo(login.mpacdesturl);
                MpacLogin mplogin = new MpacLogin();
                MPHomePage homepage = new MPHomePage();
                mplogin.Loginpacs(mpUsername, mpPassword);
                Tool tools = (Tool)homepage.NavigateTopMenu("Tools");
                homepage.NavigateTopMenu("Tools");
                tools.NavigateToSendStudy();
                tools.SearchStudy("Accession", Accession, 0);

                Dictionary<string, string> MpacResults = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());

                //Comparing study date and DOB
                String mpacdate1 = DateTime.ParseExact(MpacResults["StudyDate"], "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString();
                String studydate1 = DateTime.ParseExact(FinalDetails1["Study Date"], "dd-MMM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture).ToShortDateString();
                Boolean DOBdate = login.CompareDates(MpacResults["DOB"], FinalDetails1["DOB"], "yyyyMMdd", "dd-MMM-yyyy");

                //Validate Details in Final details column on archive window should match with details in Dest PACS
                if ((MpacResults["PatientName"].ToUpper().Split(' ')[0].Equals(FinalDetails1["Last Name"].ToUpper())) &&
                    (MpacResults["PatientName"].ToUpper().Split(' ')[1].Equals(FinalDetails1["First Name"].ToUpper())) &&
                    (MpacResults["Sex"].Equals(FinalDetails1["Gender"])) && mpacdate1.Equals(studydate1) &&
                    (MpacResults["IPID"].Equals(FinalDetails1["Issuer of PID"])) && (MpacResults["PatientID"].Equals(FinalDetails1["PID / MRN"])) &&
                    (MpacResults["StudyDescription"].ToUpper().Equals(FinalDetails1["Description"].ToUpper())) && DOBdate == true &&
                    (MpacResults["Accession"].Equals(FinalDetails1["Accession"])))
                {
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
                mplogin.LogoutPacs();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;

            }
            catch (Exception e)
            {

                //Log exception
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
        /// Reconcile window validation with matching order
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test4_29497(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Inbounds inbounds = null;
            TestCaseResult result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set steps description
            result.SetTestStepDescription(teststeps);

            try
            {
                //User Details
                String phusername = Config.ph1UserName;
                String phpassword = Config.ph1Password;
                String arusername = Config.ar1UserName;
                String arpassword = Config.ar1Password;
                String mpUsername = Config.pacsadmin;
                String mpPassword = Config.pacspassword;

                //Data Details
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String Studypaths = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String[] StudyPath = Studypaths.Split('=');
                String NominateReason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NominationReason");
                String Name = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");

                //Upload a Dicom Study -- step 1
                ei.EIDicomUpload(Config.stUserName, Config.stPassword, Config.Dest1, StudyPath[0]);

                //Upload a dicom study to destination Pacs
                BasePage.RunBatchFile(Config.batchfilepath, StudyPath[1] + " " + Config.dicomsendpath + " " + Config.DestinationPACS);
                ExecutedSteps++;

                //Login as physician
                login.DriverGoTo(login.url);
                login.LoginIConnect(phusername, phpassword);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study
                inbounds.SearchStudy("Accession", Accession);

                //Validate study is listed in ph inbounds - step 2
                if (inbounds.CheckStudy("Accession", Accession) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study not uploaded");
                }

                //Select study
                inbounds.SelectStudy("Accession", Accession);

                //Click Nominate for archive button - step 3
                IWebElement OrderField, ReasonField;
                inbounds.ClickNominateButton(out ReasonField, out OrderField);

                if (BasePage.Driver.FindElement(By.CssSelector("#NominateStudyDialogDiv")).Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select reason field
                OrderField.SendKeys("Testing");
                SelectElement selector = new SelectElement(ReasonField);
                selector.SelectByText(NominateReason);
                Logger.Instance.InfoLog("Reason Field is filled Sucessfully");

                //Click Nominate in confirmation window -step 4
                inbounds.ClickConfirmNominate();
                ExecutedSteps++;

                Dictionary<string, string> study = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Nominated For Archive" });
                if (study != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as Physician
                login.Logout();

                //Login as archivist
                login.LoginIConnect(arusername, arpassword);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study
                inbounds.SearchStudy("Accession", Accession);

                //Validate Study is listed in archivist inbounds - step 6
                if (inbounds.CheckStudy("Accession", Accession) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select study
                inbounds.SelectStudy("Accession", Accession);

                //Click Archive study button
                inbounds.ClickArchiveStudy("", "");

                //Validate Archive/Reconcile Study dialog is opened - step 7
                if (BasePage.Driver.FindElement(By.CssSelector("#ReconciliationControlDialogDiv")).Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Search order in archive window
                inbounds.ArchiveSearch("patient", Name.Split(',')[0], Name.Split(',')[1], "", "", "", "", "", "", "All Dates");

                //Get details in Matching patient column
                Dictionary<String, String> MatchingValues = inbounds.GetDataInArchive("Matching Patient");

                //Verify the patient details are listed as per the name searched- step 8
                if ((MatchingValues["Last Name"].Equals(Name.Split(',')[0])) && (MatchingValues["First Name"].Equals(Name.Split(',')[1])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Get details in Original Details column and Final Details column
                Dictionary<String, String> OriginalDetails = inbounds.GetDataInArchive("Original Details");
                Dictionary<String, String> FinalDetails = inbounds.GetDataInArchive("Final Details");

                var OriginalDiff = OriginalDetails.Where(entry => MatchingValues[entry.Key] == entry.Value)
             .ToDictionary(entry => entry.Key, entry => entry.Value);

                //Step 9 - Compare fields having same value in matching order and original details with final details column- step 9
                ExecutedSteps++;
                foreach (String Key in OriginalDiff.Keys)
                {
                    if (FinalDetails[Key].Equals(MatchingValues[Key]))
                    {

                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        break;
                    }
                }

                var MatchingDiff = OriginalDetails.Where(entry => MatchingValues[entry.Key] != entry.Value)
             .ToDictionary(entry => entry.Key, entry => entry.Value);

                //Step 10 - Validate no values are listed in final details fields with different value in matching order and original details
                ExecutedSteps++;
                foreach (String Key in MatchingDiff.Keys)
                {
                    if (MatchingValues[Key].Equals("") || OriginalDetails[Key].Equals(""))
                    {
                        continue;
                    }
                    if (FinalDetails[Key].Equals(""))
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        break;
                    }
                }

                var MatchingEmpty = OriginalDetails.Where(entry => MatchingValues[entry.Key] == "")
             .ToDictionary(entry => entry.Key, entry => entry.Value);

                //Step 11 - Validate fields having value in original detail with no value in matching order are entered with original details value automatically
                ExecutedSteps++;
                foreach (String Key in MatchingEmpty.Keys)
                {
                    if (MatchingValues[Key].Equals("") && OriginalDetails[Key].Equals(""))
                    {
                        continue;
                    }
                    if (FinalDetails[Key].Equals(OriginalDetails[Key]))
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        break;
                    }
                }

                var OriginalEmpty = MatchingValues.Where(entry => OriginalDetails[entry.Key] == "")
             .ToDictionary(entry => entry.Key, entry => entry.Value);

                //Step 12 - Validate fields having value in matching order with no value in matching order are entered with matching order value automatically
                ExecutedSteps++;
                foreach (String Key in OriginalEmpty.Keys)
                {
                    if (MatchingValues[Key].Equals("") && OriginalDetails[Key].Equals(""))
                    {
                        continue;
                    }
                    if (FinalDetails[Key].Equals(MatchingValues[Key]))
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        break;
                    }
                }

                inbounds.SetBlankFinalDetailsInArchive();
                Dictionary<String, String> FinalDetails1 = inbounds.GetDataInArchive("Final Details");

                //step 13 - Click archive in reconcile window
                inbounds.ClickArchive();

                int counter = 0;
                while (true)
                {
                    counter++;
                    //Search study
                    inbounds.SearchStudy("Accession", Accession);

                    Dictionary<string, string> Studyarchived = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Routing Completed" });

                    if (Studyarchived != null)
                        break;

                    if (counter > 4)
                        break;
                }

                //Search study
                inbounds.SearchStudy("Accession", Accession);

                //Validate status is changed to Routing Completed - Step 
                Dictionary<string, string> archivedStudy = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Routing Completed" });

                if (archivedStudy != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout archivist
                login.Logout();

                //Step- 14
                //Login to MergePacs#3 and Make sure that the study is archived properly as of Final details in Reconcile
                login.DriverGoTo(login.mpacdesturl);
                MpacLogin mplogin = new MpacLogin();
                MPHomePage homepage = new MPHomePage();
                mplogin.Loginpacs(mpUsername, mpPassword);
                Tool tools = (Tool)homepage.NavigateTopMenu("Tools");
                homepage.NavigateTopMenu("Tools");
                tools.NavigateToSendStudy();
                tools.SearchStudy("Accession", Accession, 0);

                Dictionary<string, string> MpacResults = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());

                //Comparing study date and DOB
                String mpacdate1 = DateTime.ParseExact(MpacResults["StudyDate"], "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString();
                String studydate1 = DateTime.ParseExact(FinalDetails1["Study Date"], "dd-MMM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture).ToShortDateString();
                Boolean DOBdate = login.CompareDates(MpacResults["DOB"], FinalDetails1["DOB"], "yyyyMMdd", "dd-MMM-yyyy");

                //Validate Details in Final details column on archive window should match with details in Dest PACS
                if ((MpacResults["PatientName"].ToUpper().Split(' ')[0].Equals(FinalDetails1["Last Name"].ToUpper())) &&
                    (MpacResults["PatientName"].ToUpper().Split(' ')[1].Equals(FinalDetails1["First Name"].ToUpper())) &&
                    (MpacResults["Sex"].Equals(FinalDetails1["Gender"])) && mpacdate1.Equals(studydate1) &&
                    (MpacResults["IPID"].Equals(FinalDetails1["Issuer of PID"])) && (MpacResults["PatientID"].Equals(FinalDetails1["PID / MRN"])) &&
                    (MpacResults["StudyDescription"].ToUpper().Equals(FinalDetails1["Description"].ToUpper())) && DOBdate == true &&
                    (MpacResults["Accession"].Equals(FinalDetails1["Accession"])))
                {
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
                mplogin.LogoutPacs();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;

            }
            catch (Exception e)
            {

                //Log exception
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
        /// Study Deletion from ICA Server
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_29498(String testid, String teststeps, int stepcount)
            {
                //Declare and initialize variables
                Inbounds inbounds = null;
                Outbounds outbounds = null;
                TestCaseResult result = new TestCaseResult(stepcount);
                int ExecutedSteps = -1;

                //Set steps description
                result.SetTestStepDescription(teststeps);

                try
                {
                    //User Details
                    String mpUsername = Config.pacsadmin;
                    String mpPassword = Config.pacspassword;
                    String phusername = Config.ph1UserName;
                    String phpassword = Config.ph1Password;
                    String arusername = Config.ar1UserName;
                    String arpassword = Config.ar1Password;
                    String hpUserName = Config.hpUserName;
                    String hpPassword = Config.hpPassword;

                    //Data Details
                    String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                    String PID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                    String NominateReason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NominationReason");
                    String Studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                    String PatientName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");

                    //Import the Study MergePacs#2 Server--Step-1
                    BasePage.RunBatchFile(Config.batchfilepath, Studypath + " " + Config.dicomsendpath + " " + Config.StudyPacs);

                    login.DriverGoTo(login.hpurl);
                    hphomepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                    WorkFlow workflow = (WorkFlow)hphomepage.Navigate("Workflow");

                    //search study using acc no'
                    workflow.NavigateToLink("Workflow", "Queue Worklist");

                    //Check Order in Holding Pen
                    if (workflow.HPCheckOrder("PatientName", PatientName) != true)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        throw new Exception("Order with study details already present");
                    }

                    //Logout in HP
                    hplogin.LogoutHPen();

                    //Sending study from source mpacs to iConnect --Step-2           
                    login.DriverGoTo(login.mpacstudyurl);
                    MpacLogin mplogin = new MpacLogin();
                    MPHomePage homepage = new MPHomePage();
                    mplogin.Loginpacs(mpUsername, mpPassword);
                    Tool tools = (Tool)homepage.NavigateTopMenu("Tools");
                    homepage.NavigateTopMenu("Tools");
                    tools.NavigateToSendStudy();
                    tools.SearchStudy("Accession", Accession, 0);
                    tools.MpacSelectStudy("Accession", Accession);
                    tools.SendStudy(1);
                    mplogin.LogoutPacs();
                    ExecutedSteps++;

                    //Login as physician
                    login.DriverGoTo(login.url);
                    login.LoginIConnect(phusername, phpassword);

                    //Navigate to Inbounds
                    inbounds = (Inbounds)login.Navigate("Inbounds");

                    //To wait Until study reaches Holding pen
                    PageLoadWait.WaitforReceivingStudy(180, PID);
                    PageLoadWait.WaitforUpload(Accession, inbounds);

                    //Search study
                    inbounds.SearchStudy("Accession", Accession);

                    //Validate study is listed in ph inbounds - step 3
                    if (inbounds.CheckStudy("Accession", Accession) == true)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        throw new Exception("Study not uploaded");
                    }

                    //Select study
                    inbounds.SelectStudy("Accession", Accession);

                    //Click Nominate for archive button - step 4
                    IWebElement OrderField, ReasonField;
                    inbounds.ClickNominateButton(out ReasonField, out OrderField);

                    if (BasePage.Driver.FindElement(By.CssSelector("#NominateStudyDialogDiv")).Displayed == true)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Select reason field
                    OrderField.SendKeys("Testing");
                    SelectElement selector = new SelectElement(ReasonField);
                    selector.SelectByText(NominateReason);
                    Logger.Instance.InfoLog("Reason Field is filled Sucessfully");

                    //Click Nominate in confirmation window -step 5
                    inbounds.ClickConfirmNominate();
                    ExecutedSteps++;

                    //Logout as Physician
                    login.Logout();

                    //Login as archivist
                    login.LoginIConnect(arusername, arpassword);

                    //Navigate to Inbounds
                    inbounds = (Inbounds)login.Navigate("Inbounds");

                    //Search study
                    inbounds.SearchStudy("Accession", Accession);

                    //Validate Study is listed in archivist inbounds - step 6
                    if (inbounds.CheckStudy("Accession", Accession) == true)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Select study
                    inbounds.SelectStudy("Accession", Accession);

                    //Click Delete Button
                    PageLoadWait.WaitForFrameLoad(20);
                    BasePage.Driver.FindElement(By.CssSelector("#m_deleteStudiesButton")).Click();
                    PageLoadWait.WaitForPageLoad(20);

                    //Validate Delete dialog is displayed - step 7 
                    if (BasePage.Driver.FindElement(By.CssSelector("#ssDeleteDialog")).Displayed == true)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Click Ok in delete confimation dialog
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#m_ssDeleteControl_Button1")));
                    BasePage.Driver.FindElement(By.CssSelector("#m_ssDeleteControl_Button1")).Click();
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    PageLoadWait.WaitHomePage();

                    //Search study
                    inbounds.SearchStudy("Accession", Accession);
                    Dictionary<string, string> Study1 = inbounds.GetMatchingRow("Accession", Accession);

                    //Navigate to Outbounds
                    outbounds = (Outbounds)login.Navigate("Outbounds");

                    //Search study
                    outbounds.SearchStudy("Accession", Accession);
                    Dictionary<string, string> Study2 = outbounds.GetMatchingRow("Accession", Accession);

                    //Validate deleted study is not present in ar's inbounds and outbounds - step 8
                    if (Study1 == null && Study2 == null)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Logout as archivist
                    login.Logout();

                    //Login as physician
                    login.LoginIConnect(phusername, phpassword);

                    //Navigate to Inbounds
                    inbounds = (Inbounds)login.Navigate("Inbounds");

                    //Search study
                    inbounds.SearchStudy("Accession", Accession);
                    Dictionary<string, string> phStudy1 = inbounds.GetMatchingRow("Accession", Accession);

                    //Navigate to Outbounds
                    outbounds = (Outbounds)login.Navigate("Outbounds");

                    //Search study
                    outbounds.SearchStudy("Accession", Accession);
                    Dictionary<string, string> phStudy2 = outbounds.GetMatchingRow("Accession", Accession);

                    //Validate deleted study is not present in ph's inbounds and outbounds - step 9
                    if (phStudy1 == null && phStudy2 == null)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Logout as Physician
                    login.Logout();

                    //Sending same study from source mpacs to iConnect         
                    login.DriverGoTo(login.mpacstudyurl);
                    mplogin = new MpacLogin();
                    homepage = new MPHomePage();
                    mplogin.Loginpacs(mpUsername, mpPassword);
                    tools = (Tool)homepage.NavigateTopMenu("Tools");
                    homepage.NavigateTopMenu("Tools");
                    tools.NavigateToSendStudy();
                    tools.SearchStudy("Accession", Accession, 0);
                    tools.MpacSelectStudy("Accession", Accession);
                    tools.SendStudy(1);
                    mplogin.LogoutPacs();

                    //Navigate to Holding Pen 
                    login.DriverGoTo(login.hpurl);

                    //Login in Holding Pen and Navigate to archive search menu 
                    hplogin = new HPLogin();
                    hphomepage = hplogin.LoginHPen(hpUserName, hpPassword);
                    workflow = (WorkFlow)hphomepage.Navigate("Workflow");
                    workflow.NavigateToLink("Workflow", "Archive Search");

                    //Search study
                    PageLoadWait.WaitForStudyInHp(220, Accession, workflow);
                    workflow.HPSearchStudy("Accessionno", Accession);

                    //Validate study is present in Holding pen - step 10
                    if (workflow.HPCheckStudy(Accession) == true)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        throw new Exception("Study is not present in Holding pen");
                    }

                    //Logout Holding pen
                    hplogin.LogoutHPen();

                    //Login as physician
                    login.DriverGoTo(login.url);
                    login.LoginIConnect(phusername, phpassword);

                    //Navigate to Inbounds
                    inbounds = (Inbounds)login.Navigate("Inbounds");

                    //Search study
                    inbounds.SearchStudy("Accession", Accession);

                    //Validate study is listed in ph inbounds - step 11
                    if (inbounds.CheckStudy("Accession", Accession) == true)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Select study
                    inbounds.SelectStudy("Accession", Accession);

                    //Click Delete Button
                    PageLoadWait.WaitForFrameLoad(20);
                    BasePage.Driver.FindElement(By.CssSelector("#m_deleteStudiesButton")).Click();
                    PageLoadWait.WaitForPageLoad(20);

                    //Validate Delete dialog is displayed - step 12 
                    if (BasePage.Driver.FindElement(By.CssSelector("#ssDeleteDialog")).Displayed == true)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Click Ok in delete confimation dialog
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#m_ssDeleteControl_Button1")));
                    BasePage.Driver.FindElement(By.CssSelector("#m_ssDeleteControl_Button1")).Click();
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    PageLoadWait.WaitHomePage();

                    //Search study
                    inbounds.SearchStudy("Accession", Accession);
                    Dictionary<string, string> phStudy3 = inbounds.GetMatchingRow("Accession", Accession);

                    //Navigate to Outbounds
                    outbounds = (Outbounds)login.Navigate("Outbounds");

                    //Search study
                    outbounds.SearchStudy("Accession", Accession);
                    Dictionary<string, string> phStudy4 = outbounds.GetMatchingRow("Accession", Accession);

                    //Validate deleted study is not present in ar's inbounds and outbounds - step 13
                    if (Study1 == null && Study2 == null)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Logout as Physician
                    login.Logout();

                    //Login as archivist
                    login.LoginIConnect(arusername, arpassword);

                    //Navigate to Inbounds
                    inbounds = (Inbounds)login.Navigate("Inbounds");

                    //Search study
                    inbounds.SearchStudy("Accession", Accession);
                    Dictionary<string, string> arStudy3 = inbounds.GetMatchingRow("Accession", Accession);

                    //Navigate to Outbounds
                    outbounds = (Outbounds)login.Navigate("Outbounds");

                    //Search study
                    outbounds.SearchStudy("Accession", Accession);
                    Dictionary<string, string> arStudy4 = outbounds.GetMatchingRow("Accession", Accession);

                    //Validate deleted study is not present in ar's inbounds and outbounds - step 14
                    if (Study1 == null && Study2 == null)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Logout as archivist
                    login.Logout();

                    //Report Result
                    result.FinalResult(ExecutedSteps);
                    Logger.Instance.InfoLog("Overall Test status--" + result.status);
                    return result;

                }
                catch (Exception e)
                {

                    //Log exception
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
            /// Reconciliation using destination as EA#2 server
            /// </summary>
            /// <param name="testid"></param>
            /// <param name="teststeps"></param>
            /// <param name="stepcount"></param>
            /// <returns></returns>
            public TestCaseResult Test_29499(String testid, String teststeps, int stepcount)
            {
                //Declare and initialize variables
                Inbounds inbounds = null;
                TestCaseResult result = new TestCaseResult(stepcount);
                int ExecutedSteps = -1;

                //Set steps description
                result.SetTestStepDescription(teststeps);

                try
                {
                    //User Details
                    String mpUsername = Config.pacsadmin;
                    String mpPassword = Config.pacspassword;
                    String phusername = Config.ph2UserName;
                    String phpassword = Config.ph2Password;
                    String arusername = Config.ar2UserName;
                    String arpassword = Config.ar2Password;
                    String hpUserName = Config.hpUserName;
                    String hpPassword = Config.hpPassword;

                    //Data Details
                    String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                    String TestAcc = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                    String PID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                    String Studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                    String NominateReason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NominationReason");
                    String Name = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");

                    //Initial Setup - Step 1
                    ExecutedSteps++;

                    //Upload a Dicom Study to Destination-2 -- step 2 to 5
                    ei.EIDicomUpload(Config.stUserName, Config.stPassword, Config.Dest2, Studypath);
                    ExecutedSteps = ExecutedSteps + 4;

                    //Login as physician
                    login.DriverGoTo(login.url);
                    login.LoginIConnect(phusername, phpassword);

                    //Navigate to Inbounds
                    inbounds = (Inbounds)login.Navigate("Inbounds");

                    //Search study
                    inbounds.SearchStudy("Accession", Accession);

                    //Validate study is listed in ph inbounds - step 6
                    if (inbounds.CheckStudy("Accession", Accession) == true)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        throw new Exception("Study not uploaded");
                    }

                    //Select study
                    inbounds.SelectStudy("Accession", Accession);

                    //Click Nominate for archive button - step 7
                    IWebElement OrderField, ReasonField;
                    inbounds.ClickNominateButton(out ReasonField, out OrderField);

                    if (BasePage.Driver.FindElement(By.CssSelector("#NominateStudyDialogDiv")).Displayed == true)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Select reason field
                    OrderField.SendKeys("Testing");
                    SelectElement selector = new SelectElement(ReasonField);
                    selector.SelectByText(NominateReason);
                    Logger.Instance.InfoLog("Reason Field is filled Sucessfully");

                    //Click Nominate in confirmation window -step 8
                    inbounds.ClickConfirmNominate();
                    ExecutedSteps++;

                    //Nominate the test study
                    inbounds.SearchStudy("Accession", TestAcc);
                    inbounds.SelectStudy("Accession", TestAcc);
                    IWebElement OrderField1, ReasonField1;
                    inbounds.ClickNominateButton(out ReasonField1, out OrderField1);
                    OrderField1.SendKeys("Testing");
                    SelectElement selector1 = new SelectElement(ReasonField1);
                    selector1.SelectByText(NominateReason);
                    Logger.Instance.InfoLog("Reason Field is filled Sucessfully");
                    inbounds.ClickConfirmNominate();

                    //Logout as Physician
                    login.Logout();

                    //Login as archivist
                    login.LoginIConnect(arusername, arpassword);

                    //Navigate to Inbounds
                    inbounds = (Inbounds)login.Navigate("Inbounds");

                    //Archive test study
                    inbounds.SearchStudy("Accession", TestAcc);
                    inbounds.SelectStudy("Accession", TestAcc);
                    inbounds.ArchiveStudy("", "");

                    //Search study
                    inbounds.SearchStudy("Accession", Accession);

                    //Validate Study is listed in archivist inbounds - step 9
                    if (inbounds.CheckStudy("Accession", Accession) == true)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Select study
                    inbounds.SelectStudy("Accession", Accession);

                    //Click Archive study button
                    inbounds.ClickArchiveStudy("", "");

                    //Validate Archive/Reconcile Study dialog is opened - step 10
                    if (BasePage.Driver.FindElement(By.CssSelector("#ReconciliationControlDialogDiv")).Displayed == true)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Search patient in archive window
                    inbounds.ArchiveSearch("patient", Name.Split(',')[0], "", "", "", "", "", "", "", "All Dates");

                    //Get details in Matching patient column
                    Dictionary<String, String> MatchingValues = inbounds.GetDataInArchive("Matching Patient");

                    //Compare name in matching patient column with name searched - step 11
                    if ((MatchingValues["Last Name"].Equals(Name.Split(',')[0])) && (MatchingValues["First Name"].Equals(Name.Split(',')[1])))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Select first name in matching patient column
                    inbounds.SetCheckBoxInArchive("matching patient", "firstname");

                    //Get details in Final Details column
                    Dictionary<String, String> FinalDetails = inbounds.GetDataInArchive("Final Details");

                    //Verify the first name in Final Details column - step 12
                    if (MatchingValues["First Name"].Equals(FinalDetails["First Name"]))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Edit the study description in Final details column
                    inbounds.EditFinalDetailsInArchive("Description", "leg".ToUpper());

                    //Get details in Final Details column
                    Dictionary<String, String> FinalDetails1 = inbounds.GetDataInArchive("Final Details");

                    //Verify updated description in Final details coulmn - step 13
                    if ("leg".ToUpper().Equals(FinalDetails1["Description"]))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Click archive in reconcile window
                    inbounds.ClickArchive();

                    int counter = 0;
                    while (true)
                    {
                        counter++;
                        //Search study
                        inbounds.SearchStudy("Accession", Accession);

                        Dictionary<string, string> Studyarchived = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Routing Completed" });

                        if (Studyarchived != null)
                            break;

                        if (counter > 4)
                            break;
                    }

                    //Search study
                    inbounds.SearchStudy("Accession", Accession);

                    //Validate status is changed to Routing Completed - Step 14
                    Dictionary<string, string> archivedStudy = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Routing Completed" });

                    if (archivedStudy != null)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Logout archivist
                    login.Logout();

                    //Login in HP
                    login.DriverGoTo(login.destEAurl);
                    HPHomePage hphomepage1 = hplogin.LoginHPen(hpUserName, hpPassword, true);
                    WorkFlow workflow1 = (WorkFlow)hphomepage1.Navigate("Workflow");

                    //search study using acc no'
                    workflow1.NavigateToLink("Workflow", "Archive Search");
                    workflow1.HPSearchStudy("Accessionno", Accession);

                    Dictionary<string, string> StudyDetails = workflow1.GetStudyDetailsInHP();

                    string finaldate = DateTime.Parse(FinalDetails1["Study Date"]).ToShortDateString();
                    string studydate = DateTime.Parse(StudyDetails["Study Date"]).ToShortDateString();

                    //Compare study details in Final details column with details in Destination EA - step 15
                    if ((StudyDetails["Patient Name"].Split(',')[0].Trim().Equals(FinalDetails1["Last Name"])) && (StudyDetails["Patient Name"].Split(',')[1].Trim().Equals(FinalDetails1["First Name"])) &&
                        (StudyDetails["Accession Number"].Equals(FinalDetails1["Accession"])) &&
                        (StudyDetails["Study Description"].Equals(FinalDetails1["Description"])) && finaldate.Equals(studydate))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Logout in HP
                    hplogin.LogoutHPen();

                    //Report Result
                    result.FinalResult(ExecutedSteps);
                    Logger.Instance.InfoLog("Overall Test status--" + result.status);
                    return result;

                }
                catch (Exception e)
                {

                    //Log exception
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
            
            #endregion Sprint-4 Test Cases

        }
    }