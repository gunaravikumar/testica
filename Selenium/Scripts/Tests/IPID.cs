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

namespace Selenium.Scripts.Tests
{
    class IPID
    {

        public Login login { get; set; }
        public string filepath { get; set; }
        //public ExamImporter ei { get; set; }
        public MpacLogin mpaclogin { get; set; }   

        public IPID(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            //ei = new ExamImporter();
            mpaclogin = new MpacLogin();
        }
                
        # region Sprint-3 Test Cases

        /// <Test Case-29482>
        /// PACS Gateway - Studies with different IPID and same Patient name, PID
        /// </summary>
        public TestCaseResult Test_29482(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Inbounds inbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int executedsteps = -1;                         

            try
            {
                //Fetch required Test data
                String arUsername = Config.ar1UserName;
                String arPassword = Config.ar1Password;
                String phUsername = Config.ph1UserName;
                String phPassword = Config.ph1Password;
                String AccessionNumbersList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = AccessionNumbersList.Split(':');
                String reason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NominationReason");                
                String studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String orderpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderFilePath");
                String lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String firstname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String ModifiedLastName = lastname + new Random().Next(1000);
                String utilitypath = Config.dicomsendpath;
                String studypacs = Config.StudyPacs;
                String arguments = studypath + " " + utilitypath + " " + studypacs;
                

                //Precodition - Patient should have an Order existing in MWL pacs but need not be an matchin order
                new BasePage().SendHL7Order(Config.MergePACsIP, int.Parse(Config.mpacport), orderpath);


                //Step-1 Setup PACS Gateway 1 and 2 with different IPIDs (Done as part of initial setup)
                executedsteps++;
                

                //Step-2 Send Study1 and 2 to Study pacs
                BasePage.RunBatchFile(Config.batchfilepath, arguments);
                executedsteps++;               
                

                //Login into Mpacs
                BasePage.Driver.Navigate().GoToUrl("http://"+Config.StudyPacs+"/merge-management");
                MPHomePage mpachome  = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);
                Tool tools = (Tool)mpachome.NavigateTopMenu("Tools"); 
                tools.NavigateToSendStudy();
                
                //Step-3 Send Study-1 to Gateway-1
                tools.SearchStudy("Accession", AccessionNumbers[0], 0);
                tools.MpacSelectStudy("Accession", AccessionNumbers[0]);
                tools.SendStudy(0);
                executedsteps++; 

                //Step-4 Send Study-1 to Gateway-2
                tools.SearchStudy("Accession", AccessionNumbers[1], 0);                
                tools.MpacSelectStudy("Accession", AccessionNumbers[1]);
                tools.SendStudy(0, Config.pacsgatway2);
                executedsteps++;
                mpaclogin.LogoutPacs();

                //Login as Physician --Step-5 and 6
                BasePage.Driver.Navigate().GoToUrl(login.url);
                login.LoginIConnect(phUsername, phPassword);
                executedsteps++;
                executedsteps++;

                //Navigate to Inbounds--Step-7 and Wait till study arrived at iConnect
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.ChooseColumns(new string[] { "Issuer of PID" });
                PageLoadWait.WaitforStudyInStatus(AccessionNumbers[0], inbounds, "Uploaded");
                inbounds.SearchStudy("Accession", AccessionNumbers[0]);   
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                var study1 = inbounds.GetMatchingRow(new string[] {"Accession", "Issuer of PID" }, new string[] {AccessionNumbers[0], Config.ipid1});
                PageLoadWait.WaitforStudyInStatus(AccessionNumbers[1], inbounds, "Uploaded");
                inbounds.SearchStudy("Accession", AccessionNumbers[1]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                inbounds.ChooseColumns(new string[] { "Issuer of PID" });
                var study2 = inbounds.GetMatchingRow(new string[] { "Accession", "Issuer of PID" }, new string[] { AccessionNumbers[1], Config.ipid2});
                if ((study1!=null) && (study2!=null))
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    result.steps[executedsteps].SetLogs();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                    throw new Exception("Either Study-1 or Study-2 not found in Inbounds");
                }
               

                //Search Study -- Step-8 Nominate Study-1 to Archive
                BasePage.Driver.FindElement(By.CssSelector("input#m_studySearchControl_m_searchInputAccession")).Clear();
                inbounds.SearchStudy("lastname", lastname);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                //Select Study
                inbounds.SelectStudy("Accession", AccessionNumbers[0]);
                //Click Nominate Button
                IWebElement ReasonField, OrderField;
                inbounds.ClickNominateButton(out ReasonField, out OrderField);
                //Validate whether Nominate dialog is opened or not for the selected study
                if (BasePage.Driver.FindElement(By.CssSelector("#NominateStudyDialogDiv")).Displayed == true)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    result.steps[executedsteps].SetLogs();
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedsteps].description);
                }

                //Select Reason and Confirm nominate--Step-9 and 10 --Select reason and Confirm Nominate
                SelectElement selector = new SelectElement(ReasonField);
                selector.SelectByText(reason);
                executedsteps++;
                inbounds.ClickConfirmNominate();
                PageLoadWait.WaitHomePage();           
                //Get StudyStatus
                String studyStatus1;
                inbounds.GetMatchingRow("Accession", AccessionNumbers[0]).TryGetValue("Status", out studyStatus1);

                //Validate reason for archive is selectable and confirm nominate for archive in Nominate dialog box
                if (studyStatus1 == "Nominated For Archive")
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    result.steps[executedsteps].SetLogs();
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedsteps].description);
                }

                //Logout as Physician--step-11
                login.Logout();
                executedsteps++;

                //Login as Archivist--Step-12
                login.LoginIConnect(arUsername, arPassword);
                executedsteps++;

                //Navigate to Inbounds --Step-13
                inbounds = (Inbounds)login.Navigate("Inbounds");
                //Search Study 
                inbounds.SearchStudy("lastname", lastname);
                //Get StudyStatus
                String studyStatus2;
                inbounds.GetMatchingRow("Accession", AccessionNumbers[0]).TryGetValue("Status", out studyStatus2);
                //Validate study Status as Nominated for archive in archivist's inbounds
                if (studyStatus2 == "Nominated For Archive")
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    result.steps[executedsteps].SetLogs();
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedsteps].description);
                }

                //Select Study and Click Archive--Step-14
                inbounds.SelectStudy("Accession", AccessionNumbers[0]);
                IWebElement CommentsField, ArchiveOrderField;
                inbounds.ClickArchiveStudy(out CommentsField, out ArchiveOrderField);
                //Validate Archive/Reconcile Study dialog is opened or not for the Study Uploaded using PACS Gateway 1
                if (BasePage.Driver.FindElement(By.CssSelector("#ReconciliationControlDialogDiv")).Displayed == true)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    result.steps[executedsteps].SetLogs();
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedsteps].description);
                }

                //Step-15 -- Search Order with Last name and Order Displayed
                inbounds.ArchiveSearch("Order", lastname, "", "", "", "", "", "", "", "All Dates");
                inbounds.ShowAllInReconcile();
                inbounds.SelectStudyFromReconcile("Accession", lastname+ ", "+ firstname);
                inbounds.ClickOkInShowAll();
                executedsteps++;

                //step-16 -- Not Automated as Test case needs to be updated.
                result.steps[++executedsteps].status = "Not Automated";
                
                //Step-17 - User click Original Details Checkbox and validate info in final details
                BasePage.Driver.FindElement(By.CssSelector("#m_ReconciliationControl_CheckBoxAll_Original")).Click();
                Dictionary<String, String> originaldetails = inbounds.GetDataInArchive("Original Details");
                Dictionary<String, String> finaldetails = inbounds.GetDataInArchive("Final Details");
                Dictionary<String, String> diff = new Dictionary<String, String>();                
                diff = originaldetails.Where(item => (!item.Value.Equals(finaldetails[item.Key]))).ToDictionary(item=>item.Key, item=>item.Value);
                if (diff.Count==0)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    result.steps[executedsteps].SetLogs();
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedsteps].description);
                }

                //Step-18 Update the Last Name and IPID in Final details
                inbounds.EditFinalDetailsInArchive("Last Name", ModifiedLastName);
                inbounds.EditFinalDetailsInArchive("IPID", Config.ipid2);
                executedsteps++;
 
                //Click archive in Archive Dialog--step-19
                inbounds.ClickArchive();
                executedsteps++;


                //Step-20, Refresh and check the archivist inbound that lastname is modified
                login.Logout();
                login.LoginIConnect(Config.ar1UserName, Config.ar1Password);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.ChooseColumns(new string[] { "Last Name" });
                inbounds.SearchStudy("Accession", AccessionNumbers[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                var updatedstudy = login.GetMatchingRow(new string[] { "Last Name", "Accession", "Status" }, new string[] { ModifiedLastName.ToUpper(), AccessionNumbers[0], "Routing Completed" });
                if (updatedstudy!=null)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    result.steps[executedsteps].SetLogs();
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedsteps].description);
                }

                //Step-21-Logout of Archivist
                login.Logout();
                executedsteps++;

                //Step-22 - Login as ph and check last name is updated for both the studies                
                login.LoginIConnect(Config.ph1UserName, Config.ph1Password);
                executedsteps++;

                //Step-23 -Naviagete to Inbounds and Check the both the studies are having the same name 
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.ChooseColumns(new string[] { "Last Name" });
                inbounds.SearchStudy("Accession", AccessionNumbers[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                var updatedstudy1 = login.GetMatchingRow(new string[] { "Last Name", "Accession" }, new string[] { ModifiedLastName.ToUpper(), AccessionNumbers[0] });
                login.SearchStudy("Accession", AccessionNumbers[1]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                var updatedstudy2 = login.GetMatchingRow(new string[] { "Last Name", "Accession" }, new string[] { ModifiedLastName.ToUpper(), AccessionNumbers[1] });

                if (updatedstudy1 != null && updatedstudy2 != null)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    result.steps[executedsteps].SetLogs();
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedsteps].description);
                }
                      

                //Report Result
                result.FinalResult(executedsteps);                

                //Logout 
                login.Logout();

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message+Environment.NewLine+e.StackTrace+Environment.NewLine+e.InnerException);

                //Report Result
                result.FinalResult(e, executedsteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <Test Case-29483>
        /// Exam Importer - Studies with same IPID and different Patient name, PID
        /// </summary>
        public TestCaseResult Test_29483(String testid, String teststeps, int stepcount)
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
                String arUsername = Config.ar1UserName;
                String arPassword = Config.ar1Password;
                String phUsername = Config.ph1UserName;
                String phPassword = Config.ph1Password;
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String reason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NominationReason");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String ModifiedLastName = LastName + new Random().Next(1000);
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String orderpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderFilePath");

                //Precodition - Patient should have an Order existing in MWL pacs but need not be an matching order
                new BasePage().SendHL7Order(Config.MergePACsIP, int.Parse(Config.mpacport), orderpath);

                //Initial Setup -- Non Automated step -1
                ExecutedSteps++;

                // Launch Uploader Tool - step 2
                ExamImporter ei = new ExamImporter();
                ei.LaunchEI();
                ExecutedSteps++;

                // Login - steps - 3,4,5,6
                ei.LoginToEi(Config.stUserName, Config.stPassword);
                ExecutedSteps = ExecutedSteps + 4;

                //Select Destination -steps - 7,8,9
                ei.EI_SelectDestination(Config.Dest1);
                ExecutedSteps = ExecutedSteps + 3;

                //Select Dicom path location - steps - 10,11,12
                ei.SelectFileFromHdd(UploadFilePath);
                ExecutedSteps = ExecutedSteps + 3;

                //Check Select all Patient's - step 13
                ei.SelectAllPatientsToUpload();
                ExecutedSteps++;

                //Clicks Send and upload the studies
                ei.Send();

                //Logout from EI
                ei.EI_Logout();

                //Closes the tool - steps - 14,15
                ei.CloseUploaderTool();
                ExecutedSteps = ExecutedSteps + 2;

                //Login as Physician 
                login.LoginIConnect(phUsername, phPassword);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search Study 
                inbounds.SearchStudy("Accession", Accession);

                //Choose columns
                inbounds.ChooseColumns(new string[] { "Issuer of PID" });

                //Get StudyStatus
                String studyStatus;
                PageLoadWait.WaitforStudyInStatus(Accession, inbounds, "Uploaded");
                inbounds.GetMatchingRow(new string[] { "Accession", "Issuer of PID"}, new string[] { Accession, Config.ipid1 }).TryGetValue("Status", out studyStatus);

                //Validate Study Status as Uploaded in Physician's inbounds for Study uploaded using Exam importer 1 - step 16
                if (studyStatus == "Uploaded")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study not found in Inbounds");
                }

                //Select Study
                inbounds.SelectStudy("Accession", Accession);

                //Click Nominate Button
                IWebElement ReasonField, OrderField;
                inbounds.ClickNominateButton(out ReasonField, out OrderField);

                //Validate whether Nominate dialog is opened or not for the selected study - step 17
                if (BasePage.Driver.FindElement(By.CssSelector("#NominateStudyDialogDiv")).Displayed == true)
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

                //Select Reason and Confirm nominate - step 18
                SelectElement selector = new SelectElement(ReasonField);
                selector.SelectByText(reason);
                inbounds.ClickConfirmNominate();
                ExecutedSteps++;

                //Get StudyStatus
                String studyStatus1;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyStatus1);

                //Validate reason for archive is selectable and confirm nominate for archive in Nominate dialog box - step 19   
                if (studyStatus1 == "Nominated For Archive")
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

                //Logout as Physician - step 20
                login.Logout();
                ExecutedSteps++;

                //Login as Archivist -step 21
                login.LoginIConnect(arUsername, arPassword);
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search Study 
                inbounds.SearchStudy("Accession", Accession);

                //Get StudyStatus
                String studyStatus2;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyStatus2);

                //Validate study Status as Nominated for archive in archivist's inbounds - step 22
                if (studyStatus2 == "Nominated For Archive")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Nominated Study not found in Archivist inbounds");
                }

                //Select Study and Click Archive
                inbounds.SelectStudy("Accession", Accession);
                IWebElement CommentsField, ArchiveOrderField;
                inbounds.ClickArchiveStudy(out CommentsField, out ArchiveOrderField);

                //Validate Archive/Reconcile Study dialog is opened or not for the Study Uploaded using EI 1 - step 23
                if (BasePage.Driver.FindElement(By.CssSelector("#ReconciliationControlDialogDiv")).Displayed == true)
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

                //Enter details in Search field and Search
                inbounds.ArchiveSearch("Order", LastName.Substring(0, 2) + "*", "", "", "", "", "", "", "", "All Dates");

                //Select order
                inbounds.ShowAllInReconcile();
                inbounds.SelectStudyFromReconcile("Patient Name", LastName + ", " + FirstName);
                inbounds.ClickOkInShowAll();

                Dictionary<String, String> MatchingDetails = inbounds.GetDataInArchive("Matching Order");

                //Validate Search data should match with the matching order - step 24
                if (MatchingDetails["Last Name"].StartsWith(LastName.Substring(0, 2)) == true)
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

                //step-25 -- Not Automated as Test case needs to be updated.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-26 - User click Original Details Checkbox and validate info in final details
                BasePage.Driver.FindElement(By.CssSelector("#m_ReconciliationControl_CheckBoxAll_Original")).Click();

                Dictionary<String, String> originaldetails = inbounds.GetDataInArchive("Original Details");
                Dictionary<String, String> finaldetails = inbounds.GetDataInArchive("Final Details");

                Dictionary<String, String> diff = new Dictionary<String, String>();
                diff = originaldetails.Where(item => (!item.Value.Equals(finaldetails[item.Key]))).ToDictionary(item => item.Key, item => item.Value);
                if (diff.Count == 0)
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

                //Step-27 Update the Last Name and IPID in Final details
                inbounds.EditFinalDetailsInArchive("Last Name", ModifiedLastName);
                ExecutedSteps++;

                //Click archive in Archive Dialog
                inbounds.ClickArchive();

                //Get StudyStatus
                PageLoadWait.WaitforStudyInStatus(Accession, inbounds, "Routing Completed");
                var updatedstudy1 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] {Accession, "Routing Completed" });

                //Validate study Status is not Nominated for archive in archivist's inbounds -step 28
                if (updatedstudy1 != null)
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

                //Step-29, Refresh and check the archivist inbound that lastname is modified
                login.Logout();
                login.LoginIConnect(arUsername, arPassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.ChooseColumns(new string[] { "Last Name" });
                inbounds.SearchStudy("Accession", Accession);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                var updatedstudy = inbounds.GetMatchingRow(new string[] { "Last Name", "Accession", "Status" }, new string[] { ModifiedLastName.ToUpper(), Accession, "Routing Completed" });

                if (updatedstudy != null)
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

                //Step-30-Logout of Archivist
                login.Logout();
                ExecutedSteps++;

                //Login as Physician - step 31
                login.LoginIConnect(phUsername, phPassword);
                ExecutedSteps++;

                //Navigate to inbounds and choose last name column
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.ChooseColumns(new string[] { "Last Name" });

                //Search Study
                inbounds.SearchStudy("Accession", Accession);

                var updatedstudy2 = inbounds.GetMatchingRow(new string[] { "Last Name", "Accession", "Status" }, new string[] { ModifiedLastName.ToUpper(), Accession, "Routing Completed" });

                //Valiadate study last name gets modified
                if (updatedstudy2 != null)
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

                //Logout
                login.Logout();

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }


        #endregion Sprint-3 Test Cases

        #region Sprint-4 Test Cases

        /// <Test Case-29480>
        /// Exam Importer - Studies with different IPID and same Patient name, PID
        /// </summary>
        public TestCaseResult Test_29480(String testid, String teststeps, int stepcount)
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
                String arUsername = Config.ar1UserName;
                String arPassword = Config.ar1Password;
                String phUsername = Config.ph1UserName;
                String phPassword = Config.ph1Password;
                String AccessionIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String Accession = AccessionIDList.Split(':')[0];
                String Accession2 = AccessionIDList.Split(':')[1];
                String reason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NominationReason");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String[] FilePaths = UploadFilePath.Split('=');
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String ModifiedLastName = LastName + new Random().Next(1000);
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String orderpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderFilePath");

                //Precodition - Patient should have an Order existing in MWL pacs but need not be an matching order
                new BasePage().SendHL7Order(Config.MergePACsIP, int.Parse(Config.mpacport), orderpath);

                //Login as Physician 
                //Initial Setup -- step -1
                ExecutedSteps++;

                // Launch Uploader Tool - step 2
                ExamImporter ei = new ExamImporter();
                ei.LaunchEI();
                ExecutedSteps++;

                // Login - steps - 3,4,5,6
                ei.LoginToEi(Config.stUserName, Config.stPassword);
                ExecutedSteps = ExecutedSteps + 4;

                //Select Destination -steps - 7,8,9
                ei.EI_SelectDestination(Config.Dest1);
                ExecutedSteps = ExecutedSteps + 3;

                //Select Dicom path location - steps - 10,11,12
                ei.SelectFileFromHdd(FilePaths[0]);
                ExecutedSteps = ExecutedSteps + 3;

                //Check Select all Patient's - step 13
                ei.SelectAllPatientsToUpload();
                ExecutedSteps++;

                //Clicks Send and upload the studies
                ei.Send();

                //Logout from EI
                ei.EI_Logout();

                //Closes the tool - steps - 14,15
                ei.CloseUploaderTool();
                ExecutedSteps = ExecutedSteps + 2;

                //Navigate to url - step 16
                login.DriverGoTo(login.url);
                ExecutedSteps++;

                //Login as Physician 
                login.LoginIConnect(phUsername, phPassword);

                //Navigate to Inbounds - step 17
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;

                //Search Study 
                inbounds.SearchStudy("Accession", Accession);

                //Choose columns
                inbounds.ChooseColumns(new string[] { "Issuer of PID" });

                //Get StudyStatus
                Dictionary<string, string> study = inbounds.GetMatchingRow(new string[] { "Accession", "Issuer of PID", "Status" }, new string[] { Accession, Config.ipid1, "Uploaded" });

                //Validate Study Status as Uploaded in Physician's inbounds for Study uploaded using Exam importer 1 - step 18
                if (study != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study not found in Inbounds");
                }

                //Select Study
                inbounds.SelectStudy("Accession", Accession);

                //Click Nominate Button
                IWebElement ReasonField, OrderField;
                inbounds.ClickNominateButton(out ReasonField, out OrderField);

                //Validate whether Nominate dialog is opened or not for the selected study - step 19
                if (BasePage.Driver.FindElement(By.CssSelector("#NominateStudyDialogDiv")).Displayed == true)
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

                //Select Reason and Confirm nominate - step 20
                SelectElement selector = new SelectElement(ReasonField);
                selector.SelectByText(reason);
                ExecutedSteps++;

                inbounds.ClickConfirmNominate();
                PageLoadWait.WaitHomePage();

                //Get StudyStatus
                String studyStatus1;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyStatus1);

                //Validate reason for archive is selectable and confirm nominate for archive in Nominate dialog box - step 21
                if (studyStatus1 == "Nominated For Archive")
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

                //Logout as Physician - step 22
                login.Logout();
                ExecutedSteps++;

                //Login as Physician -step 23
                login.LoginIConnect(arUsername, arPassword);
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search Study 
                inbounds.SearchStudy("Accession", Accession);

                //Get StudyStatus
                Dictionary<string, string> study1 = inbounds.GetMatchingRow("Accession", Accession);

                //Validate study Status as Nominated for archive in archivist's inbounds - step 24
                if (study1["Status"] == "Nominated For Archive" && study1["Status Reason"] == reason)
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

                //Select Study and Click Archive
                inbounds.SelectStudy("Accession", Accession);
                IWebElement CommentsField, ArchiveOrderField;
                inbounds.ClickArchiveStudy(out CommentsField, out ArchiveOrderField);

                //Validate Archive/Reconcile Study dialog is opened or not for the Study Uploaded using EI 1 - step 25
                if (BasePage.Driver.FindElement(By.CssSelector("#ReconciliationControlDialogDiv")).Displayed == true)
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

                //Enter details in Search field and Search - step 26
                inbounds.ArchiveSearch("Order", LastName, "", "", "", "", "", "", "", "All Dates");

                Dictionary<String, String> OrderDetails = inbounds.GetDataInArchive("Matching Order");
                if (OrderDetails["Last Name"] == LastName)
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

                //step-27 -- Not Automated as Test case needs to be updated.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-28 - User click Original Details Checkbox and validate info in final details
                BasePage.Driver.FindElement(By.CssSelector("#m_ReconciliationControl_CheckBoxAll_Original")).Click();

                Dictionary<String, String> originaldetails = inbounds.GetDataInArchive("Original Details");
                Dictionary<String, String> finaldetails = inbounds.GetDataInArchive("Final Details");

                Dictionary<String, String> diff = new Dictionary<String, String>();
                diff = originaldetails.Where(item => (!item.Value.Equals(finaldetails[item.Key]))).ToDictionary(item => item.Key, item => item.Value);
                if (diff.Count == 0)
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

                //Step-29 Update the Last Name in Final details
                inbounds.EditFinalDetailsInArchive("Last Name", ModifiedLastName);
                ExecutedSteps++;

                //Click archive in Archive Dialog
                inbounds.ClickArchive();

                //Get StudyStatus
                PageLoadWait.WaitforStudyInStatus(Accession, inbounds, "Routing Completed");                
                var updatedstudy1 = inbounds.GetMatchingRow(new string[] {"Accession", "Status" }, new string[] { Accession, "Routing Completed" });

                //Validate study Status is not Nominated for archive in archivist's inbounds -step 30
                if (updatedstudy1 != null)
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

                //Step-31, Refresh and check the archivist inbound that lastname is modified                
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.ChooseColumns(new string[] { "Last Name" });
                inbounds.SearchStudy("Accession", Accession);
                var updatedstudy = inbounds.GetMatchingRow(new string[] { "Last Name", "Accession", "Status" }, new string[] { ModifiedLastName.ToUpper(), Accession, "Routing Completed" });

                //Validate Patient Last name is modified after Archive
                if (updatedstudy != null)
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

                //Logout as Archivist
                login.Logout();

                //Upload study usimg Exam Importer 2 - steps 32 to 46
                ei.EIDicomUpload(Config.stUserName, Config.stPassword, Config.Dest1, FilePaths[1], 2);
                ExecutedSteps = ExecutedSteps + 15;

                //Login as Physician - step 47
                login.LoginIConnect(phUsername, phPassword);
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search Study 
                inbounds.SearchStudy("firstname", "");

                //Get Patient's Last name
                String PatientName2;
                inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession2, "Uploaded" }).TryGetValue("Patient Name", out PatientName2);
                
                //Validate Two studies with diff IPID and Same PID but with diff Patient name are listed - step 48
                if (updatedstudy["Patient Name"] != PatientName2)
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

                //Logout as Physician
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

                //Logout
                login.Logout();

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }

        /// <Test Case-29481>
        /// Exam Importer - Studies with same IPID, patient name and PID
        /// </summary>
        public TestCaseResult Test_29481(String testid, String teststeps, int stepcount)
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
                String arUsername = Config.ar1UserName;
                String arPassword = Config.ar1Password;
                String phUsername = Config.ph1UserName;
                String phPassword = Config.ph1Password;
                String AccessionIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String Accession = AccessionIDList.Split(':')[0];
                String Accession2 = AccessionIDList.Split(':')[1];
                String reason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NominationReason");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String[] FilePaths = UploadFilePath.Split('=');
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String ModifiedLastName = LastName + new Random().Next(1000);
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String orderpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderFilePath");

                //Precodition - Patient should have an Order existing in MWL pacs but need not be an matching order
                new BasePage().SendHL7Order(Config.MergePACsIP, int.Parse(Config.mpacport), orderpath);

                //Login as Physician 
                //Initial Setup -- step -1
                ExecutedSteps++;

                // Launch Uploader Tool - step 2
                ExamImporter ei = new ExamImporter();
                ei.LaunchEI();
                ExecutedSteps++;

                // Login - steps - 3,4,5,6
                ei.LoginToEi(Config.stUserName, Config.stPassword);
                ExecutedSteps = ExecutedSteps + 4;

                //Select Destination -steps - 7,8,9
                ei.EI_SelectDestination(Config.Dest1);
                ExecutedSteps = ExecutedSteps + 3;

                //Select Dicom path location - steps - 10,11,12
                ei.SelectFileFromHdd(FilePaths[0]);
                ExecutedSteps = ExecutedSteps + 3;

                //Check Select all Patient's - step 13
                ei.SelectAllPatientsToUpload();
                ExecutedSteps++;

                //Clicks Send and upload the studies
                ei.Send();

                //Logout from EI
                ei.EI_Logout();

                //Closes the tool - steps - 14,15
                ei.CloseUploaderTool();
                ExecutedSteps = ExecutedSteps + 2;

                //Navigate to url - step 16
                login.DriverGoTo(login.url);
                ExecutedSteps++;

                //Login as Physician 
                login.LoginIConnect(phUsername, phPassword);

                //Navigate to Inbounds - step 17
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;

                //Search Study 
                inbounds.SearchStudy("Accession", Accession);

                //Get StudyStatus
                Dictionary<string, string> study = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Uploaded" });

                //Validate Study Status as Uploaded in Physician's inbounds for Study uploaded using Exam importer 1 - step 18
                if (study != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study not found in Inbounds");
                }

                //Select Study
                inbounds.SelectStudy("Accession", Accession);

                //Click Nominate Button
                IWebElement ReasonField, OrderField;
                inbounds.ClickNominateButton(out ReasonField, out OrderField);

                //Validate whether Nominate dialog is opened or not for the selected study - step 19
                if (BasePage.Driver.FindElement(By.CssSelector("#NominateStudyDialogDiv")).Displayed == true)
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

                //Select Reason and Confirm nominate - step 20
                SelectElement selector = new SelectElement(ReasonField);
                selector.SelectByText(reason);
                ExecutedSteps++;

                //Click Nominate button
                inbounds.ClickConfirmNominate();

                //Get StudyStatus
                String studyStatus1;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyStatus1);

                //Validate reason for archive is selectable and confirm nominate for archive in Nominate dialog box - step 21
                if (studyStatus1 == "Nominated For Archive")
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

                //Logout as Physician - step 22
                login.Logout();
                ExecutedSteps++;

                //Login as Physician -step 23
                login.LoginIConnect(arUsername, arPassword);
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search Study 
                inbounds.SearchStudy("Accession", Accession);

                //Get StudyStatus
                Dictionary<string, string> study1 = inbounds.GetMatchingRow("Accession", Accession);

                //Validate study Status as Nominated for archive in archivist's inbounds - step 24
                if (study1["Status"] == "Nominated For Archive" && study1["Status Reason"] == reason)
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

                //Select Study and Click Archive
                inbounds.SelectStudy("Accession", Accession);
                inbounds.ClickArchiveStudy("", "");

                //Validate Archive/Reconcile Study dialog is opened or not for the Study Uploaded using EI 1 - step 25
                if (BasePage.Driver.FindElement(By.CssSelector("#ReconciliationControlDialogDiv")).Displayed == true)
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

                //Enter details in Search field and Search - step 26
                inbounds.ArchiveSearch("Order", LastName, "", "", "", "", "", "", "", "All Dates");

                Dictionary<String, String> OrderDetails = inbounds.GetDataInArchive("Matching Order");
                if (OrderDetails["Last Name"] == LastName)
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

                //step-27 -- Not Automated as Test case needs to be updated.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-28 - User click Original Details Checkbox and validate info in final details
                BasePage.Driver.FindElement(By.CssSelector("#m_ReconciliationControl_CheckBoxAll_Original")).Click();

                Dictionary<String, String> originaldetails = inbounds.GetDataInArchive("Original Details");
                Dictionary<String, String> finaldetails = inbounds.GetDataInArchive("Final Details");

                Dictionary<String, String> diff = new Dictionary<String, String>();
                diff = originaldetails.Where(item => (!item.Value.Equals(finaldetails[item.Key]))).ToDictionary(item => item.Key, item => item.Value);
                if (diff.Count == 0)
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

                //Step-29 Update the Last Name in Final details
                inbounds.EditFinalDetailsInArchive("Last Name", ModifiedLastName);
                ExecutedSteps++;

                //Click archive in Archive Dialog
                inbounds.ClickArchive();

                //Get StudyStatus
                PageLoadWait.WaitforStudyInStatus(Accession, inbounds, "Routing Completed");
                var updatedstudy1 = inbounds.GetMatchingRow(new string[] { "Status", "Accession" }, new string[] { "Routing Completed", Accession });

                //Validate study Status is not Nominated for archive in archivist's inbounds -step 30
                if (updatedstudy1 != null)
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

                //Step-31, Refresh and check the archivist inbound that lastname is modified                
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("Accession", Accession);

                inbounds.ChooseColumns(new string[] { "Last Name" });
                inbounds.SearchStudy("Accession", Accession);

                var updatedstudy = inbounds.GetMatchingRow(new string[] { "Last Name", "Accession" }, new string[] { ModifiedLastName.ToUpper(), Accession });

                //Validate Patient Last name is modified after Archive
                if (updatedstudy != null)
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

                //Logout as Archivist
                login.Logout();

                //Upload study usimg Exam Importer 1(same) - steps 32 to 46
                ei.EIDicomUpload(Config.stUserName, Config.stPassword, Config.Dest1, FilePaths[1]);
                ExecutedSteps = ExecutedSteps + 15;

                //Login as Physician - step 47
                login.LoginIConnect(phUsername, phPassword);
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search Study 
                inbounds.SearchStudy("firstname", "");

                //Get Patient's Last name
                String PatientName2;
                inbounds.GetMatchingRow("Accession", Accession2).TryGetValue("Patient Name", out PatientName2);

                String[] PatientNames = new String[] { updatedstudy["Patient Name"], PatientName2 };

                //Validate Two studies with same Patient name and Same PID are listed - step 48
                ExecutedSteps++;
                foreach (String patientname in PatientNames)
                {
                    if (patientname.Split(',')[0] == ModifiedLastName)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        throw new Exception("Patient Name not updated");
                    }
                }

                //Logout as Physician
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

                //Logout
                login.Logout();

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }
        
        #endregion Sprint-4 Test Cases

        /// <summary>
        /// Cleanup script to close browser
        /// </summary>
        public void Test_Cleanup()
        {
            login.CloseBrowser();
        }

    }
}
